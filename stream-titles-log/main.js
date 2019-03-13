//TODO: Comply with JSLint's strict formatting specs
//Lines with "XXX" after the margin indicate lines with bad code (eg: for in)
const game_rows = 3;

function ajax(operation, parameters) {
    return new Promise(function ajaxPromise(resolve,reject) {
        var has_callback = (typeof callback === "function");
        var has_error = (typeof error === "function");
        var pdata = [];
        var keys = null;
        var key;
        var i;
        
        if (typeof parameters === "object") {
            keys = Object.keys(parameters);
            for (i = 0; i < keys.length; i += 1) {
                key = keys[i];
                pdata.push(`${encodeURIComponent(key)}=${encodeURIComponent(parameters[key])}`)
            }
            pdata = pdata.join("&");
        } else if (typeof parameters === "string") {
            pdata = parameters;
        } else {
            pdata = null;
        }
        
        var xhr = new XMLHttpRequest();
        xhr.open(
            "POST",
            `resources.php?module=${encodeURIComponent(operation)}`,
            true
        );
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhr.addEventListener("readystatechange",function () {
            if (xhr.readyState === 4) {
                ((xhr.status >= 400) ? reject : resolve)(xhr);
            }
        });
        xhr.send(pdata); //default value of body is "null", hence this is ok
    });
}

//2=textcontent, 0=innerhtml

function sanitize(str) {
    return str.replace(/\</g,"&LT;").replace(/\>/g,"&GT;");
}

function toggleRow(row) {
    var rows = parseInt(row.getAttribute("x-rows"));
    var state = row.hasAttribute("x-state") ? row.getAttribute("x-state") === "expand" : true;
    var i;

    state = !state;
    row.setAttribute("x-state", state ? "expand" : "collapse");
    for (i = 0; i < rows; i += 1) {
        row = row.nextElementSibling;
        row.style.display = state ? "table-row" : "none";
    }
}

function prettyStamps(iso) {
    try {
        if (!iso) {
            return "";
        }
        function pad(a) {
            var b = a.toString();
            while (b.length < 2) {
                b = `0${b}`;
            }
            return b;
        }
        var date = new Date(Date.parse(iso)); //shouldn't use Date.parse
        var mm = (["January","February","March","April","May","June","July","August","September","October","November","December"])[date.getMonth()];
        var d = date.getDate();
        var dd = (["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"])[date.getDay()];
        var y = date.getFullYear();
        var h = date.getHours(); //0-23
        var m = date.getMinutes(); //0-59
        var s = date.getSeconds(); //0-59
        var z = (h >= 12) ? "PM" : "AM";
        h %= 12;
        if ( h === 0) {
            h = 12;
        }
        return `${dd}, ${mm} ${d}, ${y} at ${h}:${pad(m)}:${pad(s)} ${z}`;
    } catch (e) {
        return iso;
    }
}

function mainload() {
    var b = document.getElementById("main-content");
    b.innerHTML = ``;
    loadTemplate(b, "template_main");
    document.getElementById("tAdd").addEventListener("click",load.bind(this,"add_title_form"));
    window.cb_rest = 1; //when zero...
    
    var tbody = document.querySelector("#titlesTable tbody");
    var cover = document.getElementById("cover");
    
    ajax("list_titles").then(function titles_callback(response) {
        var data = response.responseText;
        window.cb_rest -= 1;
        if (data.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="${game_rows}">
                        Error: Server responded with no data!
                    </td>
                </tr>`;
        } else {
            data = JSON.parse(data).data;
            var html = "";
            var games = { "{{{{GO LIVE}}}}": [] };
            var game;
            var i;
            var j;

            var keys;
            var key;
            
            for (i = 0; i < data.length; i++) {
                game = data[i];
                if (Object.keys(games).indexOf(game.game) === -1) {
                    games[game.game] = [];
                }
                games[game.game].push({
                    "tags": game.tags,
                    "stamp": game.stamp,
                    "title": game.title
                });
            }
            
            keys = Object.keys(games);
            for (i = 0; i < keys.length; i += 1) {
                game = games[keys[i]];
                html += `
                    <tr class="group-header-row" onClick="toggleRow(this)" x-rows="${game.length}">
                        <td colspan="${game_rows}">${sanitize(keys[i])}</td>
                    </tr>`;
                for (j = 0; j < game.length; j += 1) {
                    html += `
                        <tr class="group-content-row">
                            <td>${sanitize(game[j].title || "")}</td>
                            <td>${((game[j].tags !== null) && (game[j].tags.length > 0)) ? `<input type="button" value="Tags" onclick="load('tags',this.getAttribute('x-tags'))" x-tags="${sanitize(game[j].tags || "")}"$/>` : ""}</td>
                            <td>${(game[j].stamp === "2018-06-21 16:55:53") ? "" : prettyStamps(game[j].stamp || "")}</td>
                        </tr>`;
                }
            }
            if (data.length === 0) {
                html = `
                    <tr>
                        <td colspan="${game_rows}">No Stream Titles</td>
                    </tr>`;
            }
            tbody.innerHTML = html;
            
            data = document.querySelectorAll(".group-header-row");
            for (i = 0; i < data.length; i += 1) {
                data[i].click();
            }
        }
        if (window.cb_rest === 0) {
            cover.parentElement.removeChild(cover);
        }
    }).catch(function titles_onerror(xhr) {
        window.cb_rest -= 1;
        tbody.innerHTML = `
            <tr>
                <td colspan="${game_rows}">
                    Error: Server responded with status ${xhr.status} (${sanitize(xhr.statusText)})!
                </td>
            </tr>`;
        if (window.cb_rest === 0) {
            cover.parentElement.removeChild(cover);
        }
    });
    //ajax list_levels here
    scrolling(true);
}

function mainloadcancel() {
    var c = document.querySelector("#modal center");
    c.innerHTML = "";
    c.parentElement.style.display = "none";
    scrolling(true);
}

function fixAttributes(node) {
    var attribute = null;
    var children = node.children;
    var i;
    for (i = 0; i < node.attributes.length; i += 1) {
        attribute = node.attributes[i];
        if (attribute.name.startsWith("x-")) {
            node.setAttribute(attribute.name.substr(2),attribute.value);
            node.removeAttribute(attribute.name);
            i -= 1;
        }
    }
    for (i = 0; i < node.children.length; i += 1) {
        fixAttributes(node.children[i]);
    }
}

function loadTemplate(target,template) {
    template = document.getElementById(template);
    var nodes = template.children;
    var i;
    var node;
    var clone;
    for (i = 0; i < nodes.length; i += 1) {
        node = nodes[i];
        clone = node.cloneNode(true);
        fixAttributes(clone);
        target.appendChild(clone);
    }
}


function load(type,tags) {
    var c = document.querySelector("#modal center");
    var ntype = (type || "").toLowerCase();
    c.innerHTML = "";
    c.parentElement.style.display = "none";
    
    switch (ntype) {
    case "add_title_form":
        loadTemplate(c,"template_addtitleform");
        document.getElementById("addTitleForm").addEventListener("submit", doForm.bind(this,this));
        document.getElementById("atfCancel").addEventListener("click", mainloadcancel);
        break;
    case "pending":
        loadTemplate(c,"template_pending");
        break;
    case "success":
        loadTemplate(c,"template_success");
        document.getElementById("sBack").addEventListener("click", mainload);
        break;
    case "error":
        loadTemplate(c,"template_error");
        document.getElementById("eBack").addEventListener("click", mainloadcancel);
        break;
    case "tags":
        loadTemplate(c,"template_tags");
        document.getElementById("tBack").addEventListener("click", mainloadcancel);
        var target = document.getElementById("tTaglist").children[0];
        var tgs = tags.split(",");
        var i;
        var t;
        for (i = 0; i < tgs.length; i += 1) {
            t = document.createElement("LI");
            t.textContent = tgs[i];
            target.appendChild(t);
        }
        break;
    }
    switch (ntype) {
    case "tags":
    case "add_title_form":
    case "pending":
    case "success":
    case "error":
        scrolling(false);
        c.parentElement.style.display = "block";
        break;
    }
}

function scrollhandler(e) {
    e.preventDefault();
}

function scrolling(enabled) {
    var body = document.body;
    if (enabled) {
        body.removeEventListener("touchmove", scrollhandler);
        body.removeEventListener("scroll", scrollhandler);
        body.style = "";
    } else {
        body.removeEventListener("touchmove", scrollhandler);
        body.removeEventListener("scroll", scrollhandler);
        body.addEventListener("touchmove", scrollhandler);
        body.addEventListener("scroll", scrollhandler);
        body.style.overflow = "hidden";
    }
}

function doForm(form) {
	var ex = form.elements;
	var props = [];
	var i;
	for (i = 0; i < ex.length; i += 1) {
		props.push(`${encodeURIComponent(ex[i].name)}=${encodeURIComponent(ex[i].value)}`);
	}
	props = props.join("&");
	ajax(form.getAttribute("formtype"), props)
	    .then(load.bind(this, "success"))
	    .catch(load.bind(this, "error"));
	load("pending");
}

function resizeHandler() {
    var modal = document.querySelector(".popup");
    if (!modal) {
        return;
    }
    
    var rect = modal.getBoundingClientRect();
    //width,height
    var y = Math.round((window.innerHeight - rect.height) / 2);
    var x = Math.round((window.innerWidth - rect.width) / 2);
    modal.style.top = `${y}px`;
    modal.style.left = `${x}px`;
    modal.style.position = "fixed";
}
window.addEventListener("resize",resizeHandler);

if (document.readyState !== "complete") {
    window.addEventListener("load", function () {
	    mainload();
        window.setInterval(resizeHandler,1000);
    });
} else {
    mainload();
}
