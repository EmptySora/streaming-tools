if (user_id === undefined) {
    var user_id = "YOUR CHANNEL/USER ID HERE";
    /*
    You can obtain your client id by calling the "GetUserID" function below from
    your browser's webconsole, when you have obtained and inserted your client_id.

    eg: "GetUserID("EmptySora_")"
    the user id is under "data" > "id"
    */
}
if (client_id === undefined) {
    var client_id = "YOUR CLIENT ID HERE"; 
    /*
    You can create your client id by going here:
    https://dev.twitch.tv/dashboard/apps/create
    follow the instructions on the page.

    If you already have a client id go here:
    https://dev.twitch.tv/console/apps

    DO NOT SHARE YOUR CLIENT ID WITH OTHER PEOPLE!!!
    */ 
}


function GetUserID(username) {
    return HelixAPI("GET", "users", {"login":username}, null);
}

function HelixAPI(method, endpoint, query_params, post_data) {
    var api = "https://api.twitch.tv/helix/" + endpoint;
    var headers = {
        "Client-ID": client_id
    };
    return jcurl(method, api, query_params, post_data, headers);
}

function jcurl(m, u, q, p, h) {
    return curl(m, u, q, p, h, function (response) {
        return JSON.parse(response);
    });
}

function curl(method, urlstr, query, post_data, headers, transform) {
    var u = urlstr;
    var h = headers;
    var p = post_data;
    if (typeof transform !== "function") {
        transform = function (response) {
            return response;
        };
    }
    return new Promise(function (resolve, reject) {
        try {
            var xhr = new XMLHttpRequest();
            var allowed = ["GET","POST","DELETE","PUT"];
            var i;
            var keys;
            var key;
            if (typeof method !== "string" || allowed.indexOf(method.toUpperCase()) === -1) {
                reject({error:"invalid parameters"});
                return;
            }
            method = method.toUpperCase();
            if (query !== null && query !== undefined) {
                u = urlstr.split(/\?/g)[0] + "?" + urlencode(query);
            }
            if (headers === null || headers === undefined) {
                h = headers = {};
            }
            if (post_data === null || post_data === undefined) {
                p = post_data = {};
            }
            xhr.open(method, u, true);
            xhr.addEventListener("readystatechange",function () {
                try {
                    if (xhr.readyState === 4) {
                        var status_string = xhr.status.toString();
                        if (status_string.startsWith("4") || status_string.startsWith("5")) {
                            reject({
                               error: `API returned HTTP error: ${xhr.statusText} (${xhr.status})`,
                               response: xhr.responseText,
                               source: xhr
                            });
                        } else {
                            resolve(transform(xhr.responseText));
                        }
                    }
                } catch (error) {
                    reject({
                        error: error
                    });
                }
            });
            keys = Object.keys(h);
            for (i = 0; i < keys.length; i += 1) {
                key = keys[i];
                xhr.setRequestHeader(key,h[key]);
            }
            switch (method) {
            case "GET":
            case "DELETE":
                xhr.send();
                break;
            case "POST":
            case "PUT":
                xhr.send(urlencode(p));
                break;
            }
        } catch (error) {
            reject({
                error: error
            });
        }
    });
}

function urlencode(object) {
    var keys = Object.keys(object);
    var key;
    var i;
    var pairs = [];
    var types = ["string", "number", "boolean", "symbol"];
    for (i = 0; i < keys.length; i += 1) {
        key = keys[i];
        if (keys[i] === undefined || keys[i] === null || types.indexOf(typeof keys[i]) === -1) {
            continue;
        }
        pairs.push(`${key}=${encodeURIComponent(object[keys])}`);
    }
    return pairs.join("&");
}

function GetUptime() {
    return new Promise(function (resolve, reject) {
        var status = null;
        GetStreamStatus().then(function (response) {
            status = response;
            if (status.created_at !== undefined) {
                status = status.created_at;
            } else if (status.started_at !== undefined) {
                status = status.started_at;
            } else {
                return reject({
                    error: "Could not detect stream uptime"
                });
            }

            var date = parseTime(status).getTime();
            var now = (new Date()).getTime();
            return resolve(new TimeSpan(now - date));
            //2019-01-30T13:18:05Z
        }).catch(reject);
    });
}

function GetStartTime() {
    return new Promise(function (resolve, reject) {
        var status = null;
        GetStreamStatus().then(function (response) {
            status = response;
            if (status.created_at !== undefined) {
                status = status.created_at;
            } else if (status.started_at !== undefined) {
                status = status.started_at;
            } else {
                return reject({
                    error: "Could not detect stream start time"
                });
            }
            return resolve(parseTime(status).getTime());
            //2019-01-30T13:18:05Z
        }).catch(reject);
    });
}

/*

  def GetUptime(self):
    dt = datetime.datetime.strptime(status,"%Y-%m-%dT%H:%M:%SZ")
    ep = int(dt.strftime("%s"))
    now = int(datetime.datetime.now().strftime("%s"))
    diff = now - ep
    return "Current uptime is: " + self.__formatTime(diff)
*/

function GetStreamStatus() {
    return new Promise(function (resolve, reject) {
        var stream = null;
        HelixAPI("GET", "streams", {user_id: user_id}, null).then(function (response) {
            if (response.data.length === 0) {
                //stream is not live
                return reject({
                   error: "Stream is not live"
                });
            } else {
                stream = response.data[0];
            }
            return HelixAPI("GET", "games", {id: stream.game_id}, null);
        }).then(function (response) {
            if (response.data.length === 0) {
                //stream is not live
                return reject({
                   error: "Stream is not live"
                });
            } else {
                stream.game = response.data[0].name;
                stream.game_boxart = response.data[0].box_art_url;
                //replace "{width}" and "{height}" with the desired width and height
            }
            return resolve(stream);
        }).catch(reject);
    });
    //GET https://api.twitch.tv/helix/streams?user_id=157578041
}

function parseTime(time) {
    //2019-01-30T13:18:05Z
    if(time.endsWith("Z")) {
        time = time.substr(0, time.length - 1);
    }
    time = time.split(/[\-T\:\.]/g);
    var year = parseInt(time[0]);
    var month = parseInt(time[1]) - 1;
    var day = parseInt(time[2]);
    var hours = parseInt(time[3]);
    var minutes = parseInt(time[4]);
    var seconds = parseInt(time[5]);
    var milliseconds = 0;
    if (time.length > 6) {
        milliseconds = parseInt(time[6]);
    }
    //(new Date((new Date(2019,0,30,13,18,5,0)).getTime() - 18000000)).toString()
    var date = new Date(year, month, day, hours, minutes, seconds, milliseconds);
    return new Date(date.getTime() - (60000 * (new Date()).getTimezoneOffset()));
    //offset for the timezone so that when it gets parsed as a local time
    //it's actually the correct time.
}

function TimeSpan(milliseconds) {
    function def(name, getf, setf) {
        var descriptor = {};
        if (typeof getf === "function") {
            descriptor.get = getf;
        }
        if (typeof setf === "function") {
            descriptor.set = getf;
        }
        Object.defineProperty(this, name, descriptor);
    }
    var gs = def.bind(this);
    

    var d = milliseconds + 0;
    var ms = d % 1000;
    d = (d - ms) / 1000;
    var s = d % 60;
    d = (d - s) / 60;
    var m = d % 60;
    d = (d - m) / 60;
    var h = d % 24;
    d = (d - h) / 24;
    

    gs("TotalMilliseconds", function () {
        return milliseconds;
    });
    gs("TotalSeconds", function () {
        return milliseconds / 1000;
    });
    gs("TotalMinutes", function () {
        return this.TotalSeconds / 60;
    });
    gs("TotalHours", function () {
        return this.TotalMinutes / 60;
    });
    gs("TotalDays", function () {
        return this.TotalHours / 24;
    });
    gs("Milliseconds", function () {
        return ms;
    });
    gs("Seconds", function () {
        return s;
    });
    gs("Minutes", function () {
        return m;
    });
    gs("Hours", function () {
        return h;
    });
    gs("Days", function () {
        return d;
    });

    this.toString = function toString() {
        function pad(len,n) {
            var r = n.toString();
            while (r.length < len) {
                r = "0" + r;
            }
            return r;
        }
        return `${d}:${pad(2,h)}:${pad(2,m)}:${pad(2,s)}.${pad(3,ms)}`;
    };
}
TimeSpan.parse = function parse(obj) {
    if (typeof obj === "number") {
        return new TimeSpan(obj);
    } else if (typeof obj === "string") {
        if (obj.indexOf(".") !== -1) {
            var tmp = obj.split(/[\.]/g);
            tmp = tmp[tmp.length - 1];
            while (tmp.length < 3) {
                obj += "0";
                tmp += "0";
            }
        }
    
        var parts = obj.split(/[\:]/g);
        var i;
        var j; //              s    m         h                    d
        var multipliers = [0,1000,1000 * 60,1000 * 60 * 60,1000 * 60 * 60 * 24];
        var n = 0;
        parts[parts.length - 1] = parts[parts.length - 1].split(/[\.]/g);
        for (i = parts.length - 1; i >= 0; i -= 1) {
            j = parts.length - i;
            if (i === parts.length - 1) {
                n += parseInt(parts[i][0]) * 1000;
                if(parts[i].length > 1) {
                    n += parseInt(parts[i][1]);
                }
            } else {
                n += parseInt(parts[i]) * multipliers[j];
            }
        }
        return new TimeSpan(n);
    }
};



var startTime = null;
var querying = false;
var lastFetch = null;
var lastError = null;
var live = false;

function update_time() {
    return new Promise(function (resolve, reject) {
        var timeRequired = TimeSpan.parse("1:00").TotalMilliseconds;
        if (lastError !== null) {
            if ((new TimeSpan((new Date()).getTime() - lastError.getTime())).TotalMilliseconds < timeRequired) {
                console.info("Cannot update start time: Throttling due to previous error.");
                window.setTimeout(reject.bind(this,{error:"Throttling"}), timeRequired - (new TimeSpan((new Date()).getTime() - lastError.getTime())).TotalMilliseconds);
                return;
            }
            lastError = null;
        }
        timeRequired = TimeSpan.parse("1:00:00").TotalMilliseconds;
        if (querying === true) {
            console.info("Cannot update start time: Already updating start time.");
            window.setTimeout(reject.bind(this,{error:"Already querying"}), TimeSpan.parse("1:00").TotalMilliseconds);
            return;
        }
        if (
                startTime === null 
                || (new TimeSpan((new Date()).getTime() - lastFetch.getTime())).TotalMilliseconds >= timeRequired
        ) { //dumb ass ^^^
            querying = true;
            GetStartTime().then(function (result) {
                console.info("Successfully updated start time:",new Date(result));
                startTime = new Date(result);
                lastFetch = new Date();
                lastError = null;
                live = true;
                querying = false;
                window.setTimeout(resolve, timeRequired);
            }).catch(function (error) {
                if (error.error === "Stream is not live") {
                    console.info("Stream is currently not live. Sleeping for",timeRequired.toString());
                    live = false;
                    lastFetch = new Date();
                    starTime = null;
                    lastError = null;
                    querying = false;
                    window.setTimeout(resolve, timeRequired);
                } else {
                    lastError = new Date();
                    querying = false;
                    window.setTimeout(reject.bind(this,error), TimeSpan.parse("1:00").TotalMilliseconds);
                }
            });
            return;
        }
        console.info("Cannot update start time: Throttling API calls.");
        window.setTimeout(reject.bind(this,{error:"Throttling"}), timeRequired - (new TimeSpan((new Date()).getTime() - (lastFetch === null ? 0 :lastFetch.getTime()))).TotalMilliseconds);
    });
}

function start_updating_time() {
    update_time().then(function () {
        start_updating_time();
    }).catch(function (error) {
        console.error(error);
        start_updating_time();
    });
}

function update_clock() {
    var clock = document.querySelector("#clock");
    if (live) {
        clock.textContent = fixTime((new TimeSpan((new Date()).getTime() - startTime.getTime())).toString().split(".")[0]);
        clock.classList.add("live");
        clock.classList.remove("offline");
    } else {
        clock.textContent = "OFFLINE";
        clock.classList.add("offline");
        clock.classList.remove("live");
    }
}

function fixTime(t) {
    var nt = t.replace(t.split(/\:/g)[0] + ":","");
    var h = parseInt(t.split(/\:/g)[0]) * 24 + parseInt(nt.split(/\:/g)[0]);
    return nt.replace(nt.split(/\:/g)[0] + ":",h + ":");
}

function main() {
    start_updating_time();
    window.setInterval(update_clock, 1);
}

main();

/*

CORS shouldn't be a bitch because Twitch API sets "access-controll-allow-origin: *"
effectively disabling CORS.

*/
