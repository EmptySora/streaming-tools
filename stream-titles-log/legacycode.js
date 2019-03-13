






/*
function DoNode(props) {
    if (props === undefined) {
        throw new TypeError("Props is undefined!");
    } else if (props === null) {
        throw new TypeError("Props is null!");
    } else if (typeof props !== "object") {
        throw new TypeError("Props is not an object!");
    } else if (props.type === undefined) {
        throw new TypeError("Props is missing required key \"type\"!");
    } else if (props.type === null) {
        throw new TypeError("Props.type is null!");
    } else if (typeof props.type !== "number") {
        throw new TypeError("Props.type is not a number!");
    }
    var node = null;
    var pkeys = Object.keys(props);
    var skeys = null;
    var tmp = null;
    var i = 0;
    switch (props.type) {
    case Node.ELEMENT_NODE: //1
        if (props.name === undefined) {
            throw new TypeError("Props is missing required key \"name\"!");
        } else if (props.name === null) {
            throw new TypeError("Props.name is null!");
        } else if (typeof props.name !== "string") {
            throw new TypeError("Props.name is not a string!");
        }
        node = document.createElement(props.name);
        if (pkeys.indexOf("innerHTML") !== -1) {
            node.innerHTML = props.innerHTML;
        }
        if (pkeys.indexOf("outerHTML") !== -1) {
            node.outerHTML = props.outerHTML;
        }
        if (pkeys.indexOf("textContent") !== -1) {
            node.textContent = props.textContent;
        }
        if (pkeys.indexOf("innerText") !== -1) {
            node.innerText = props.innerText;
        }
        if (pkeys.indexOf("value") !== -1) {
            if (node.value !== undefined) {
                node.value = props.value;
            }
        }
        if (pkeys.indexOf("attributes") !== -1) {
            skeys = Object.keys(props.attributes);
            for (i = 0; i < skeys.length; i += 1) {
                node.setAttribute(
                    skeys[i],
                    props.attributes[skeys[i]]
                );
            }
        }
        if (pkeys.indexOf("styles") !== -1) {
            skeys = Object.keys(props.styles);
            for (i = 0; i < skeys.length; i += 1) {
                node.style[skeys[i]] = props.styles[skeys[i]];
            }
        }
        if (pkeys.indexOf("callbacks") !== -1) {
            skeys = Object.keys(props.callbacks);
            for (i = 0; i < skeys.length; i += 1) {
                node.addEventListener(
                    skeys[i],
                    props.callbacks[skeys[i]]
                );
            }
        }
        if (pkeys.indexOf("children") !== -1) {
            for (i = 0; i < props.children.length; i += 1) {
                tmp = props.children[i];
                if (tmp instanceof Node) {
                    node.appendChild(tmp);
                } else if (typeof tmp === "object") {
                    node.appendChild(DoNode(tmp));
                }
            }
        }
        break;
    case Node.TEXT_NODE: //3
        if (props.text === undefined) {
            throw new TypeError("Props is missing required key \"text\"!");
        } else if (props.text === null) {
            throw new TypeError("Props.text is null!");
        } else if (typeof props.text !== "string") {
            throw new TypeError("Props.text is not a string!");
        }
        node = document.createTextNode(props.text);
        break;
    case Node.COMMENT_NODE: //8
        if (props.text === undefined) {
            throw new TypeError("Props is missing required key \"text\"!");
        } else if (props.text === null) {
            throw new TypeError("Props.text is null!");
        } else if (typeof props.text !== "string") {
            throw new TypeError("Props.text is not a string!");
        }
        node = document.createComment(props.text);
        break;
    default:
        throw new Error("Props.type is not a valid value!");
    }
    return node;
//keys:
//    *type = int enum (one of the Node.*_NODE) constants
//    *name = tag name (required if type = NODE.ELEMENT_NODE)
//    *text = node text content (required if type == 
//                               NODE.TEXT_NODE or
//                               NODE.COMMENT_NODE)
//    attributes = object[string,string]
//    styles = object[string,string]
//    callbacks = object[string,function|string]
//    children = array[Node]
//
//
//    innerHTML = string
//    outerHTML = string
//    textContent = string
//    innerText = string
//    value = string (sets property "value" instead of attribute "value")
//* = required
}
*/
/*
ALLOW     1 Node.ELEMENT_NODE,
ALLOW     3 Node.TEXT_NODE,
ALLOW     8 Node.COMMENT_NODE, (although why would you use this...?)
ALLOW    11 Node.DOCUMENT_FRAGMENT_NODE, (works with appendChild)

BLOCK    Node.PROCESSING_INSTRUCTION_NODE,
BLOCK    Node.NOTATION_NODE,
BLOCK    Node.ENTITY_REFERENCE_NODE,
BLOCK    Node.ENTITY_NODE,
BLOCK    Node.DOCUMENT_TYPE_NODE,
BLOCK    Node.DOCUMENT_NODE,
BLOCK    Node.CDATA_SECTION_NODE,
BLOCK    Node.ATTRIBUTE_NODE
*/
/*
add_title_form

		
		c.appendChild(
			$(4,"form").SetAttributes({action:"javascript:void(0)",formtype:"add_title",onsubmit:"doForm(this)",id:"addTitleForm"}).AppendChildren(
				$(4,"table").AppendChildren(
					$(4,"thead").AppendChildren(
						$(4,"tr").AppendChildren(
							$(4,"th").SetAttributes({colspan:"2"}).SetText(2,"Add Stream Title")
						)
					),
					$(4,"tbody").AppendChildren(
						$(4,"tr").SetAttributes({style:"background-color:white;"}).AppendChildren(
							$(4,"td").AppendChildren(
								$(5,"Game")
							),
							$(4,"td").AppendChildren(
								$(4,"input").SetAttributes({name:"game",type:"text",placeholder:"[Game Name]",required:"required"})
							)
						),
						$(4,"tr").SetAttributes({style:"background-color:white;"}).AppendChildren(
							$(4,"td").AppendChildren(
								$(5,"Title")
							),
							$(4,"td").AppendChildren(
								$(4,"input").SetAttributes({name:"title",type:"text",placeholder:"[Stream Title]",maxlength:"140",required:"required"})
							)
						),
						$(4,"tr").SetAttributes({style:"background-color:white;"}).AppendChildren(
							$(4,"td").AppendChildren(
								$(5,"Tags")
							),
							$(4,"td").AppendChildren(
								$(4,"input").SetAttributes({name:"tags",type:"text",placeholder:"[Tags]",title:"Comma deliminated list of tags",maxlength:"65535"})
							)
						)
					),
					$(4,"tfoot").AppendChildren(
						$(4,"tr").AppendChildren(
							$(4,"td").SetAttributes({style:"background-color:white;"}).SetAttributes({colspan:"2"}).AppendChildren(
								$(4,"center").AppendChildren(
									$(4,"input").SetAttributes({name:"submit",type:"submit",value:"Submit"}),
									$(4,"span").SetText(0,"&nbsp;&bullet;&nbsp;"),
									$(4,"input").SetAttributes({name:"reset",type:"reset",value:"Reset"}),
									$(4,"span").SetText(0,"&nbsp;&bullet;&nbsp;"),
									$(4,"input").SetAttributes({name:"cancel",type:"button",value:"Cancel",onclick:"mainloadcancel()"})
								)
							)
						)
					)
				)
			)
		);


pending

		c.appendChild(
			$(4,"table").AppendChildren(
				$(4,"thead").AppendChildren(
					$(4,"tr").AppendChildren(
						$(4,"th").SetAttributes({style:"background-color:white;"}).SetText(2,"Request Sent")
					)
				),
				$(4,"tfoot").AppendChildren(
					$(4,"tr").AppendChildren(
						$(4,"td").SetAttributes({style:"background-color:white;"}).AppendChildren(
							$(5,"Your request has successfully been"),
							$(4,"br"),
							$(5,"sent to the server. Awaiting Response...")
						)
					)
				)
			)
		);


success

		c.AppendChildren(
			$(4,"table").AppendChildren(
				$(4,"thead").AppendChildren(
					$(4,"tr").AppendChildren(
						$(4,"th").SetAttributes({style:"background-color:white;"}).SetText(2,"Success")
					)
				),
				$(4,"tfoot").AppendChildren(
					$(4,"tr").AppendChildren(
						$(4,"td").SetAttributes({style:"background-color:white;border-bottom:0px solid transparent;"}).AppendChildren(
							$(5,"Your request has been successfully completed!")
						)
					),
					$(4,"tr").AppendChildren(
						$(4,"td").SetAttributes({style:"background-color:white;"}).AppendChildren(
			                $(4,"input").SetAttributes({type:"button",value:"Back",onclick:"mainload()",title:"Back to Main Area"})
						)
					),
				)
			)
		);

error

		c.appendChild(
			$(4,"table").AppendChildren(
				$(4,"thead").AppendChildren(
					$(4,"tr").AppendChildren(
						$(4,"th").SetAttributes({style:"background-color:white;"}).SetText(2,"Error")
					)
				),
				$(4,"tfoot").AppendChildren(
					$(4,"tr").AppendChildren(
						$(4,"td").SetAttributes({style:"background-color:white;border-bottom:0px solid transparent;"}).AppendChildren(
							$(5,"Your request completed with an unsatisfactory response!")
						)
					),
					$(4,"tr").AppendChildren(
						$(4,"td").SetAttributes({style:"background-color:white;"}).AppendChildren(
			                $(4,"input").SetAttributes({type:"button",value:"Back",onclick:"mainloadcancel()",title:"Back to Main Area"})
						)
					),
				)
			)
		);
*/
/*
		case "add_vol_form":
		  scrolling(false);
	      c.parentElement.style.display = "block";
		    c.innerHTML =`
<form action="javascript:void(0)" formtype="add_level" onSubmit="doForm(this)" id="addVolumeForm">
    <table>
        <thead>
            <tr>
                <th colspan="2">Add Volume Level</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Context</td>
                <td>
                    <input name="context" type="text" placeholder="[Context]" required="required"/>
                </td>
            </tr>
            <tr>
                <td>Application</td>
                <td>
                    <input name="application" type="text" placeholder="[Application]" required="required"/>
                </td>
            </tr>
            <tr>
                <td>Name</td>
                <td>
                    <input name="name" type="text" placeholder="[Name]" required="required"/>
                </td>
            </tr>
            <tr>
              <td colspan="2">
                  <center>
                      <table id="addVolLevelTable">
                          <thead>
                              <tr>
                                  <th colspan="2">Volume Levels</th>
                              </tr>
                              <tr>
                                  <th>Name</th>
                                  <th>Level</th>
                              </tr>
                          </thead>
                          <tbody></tbody>
                          <tfoot>
                              <tr>
                                  <td colspan="2" onClick="vol_form_add()" class="addrow">
                                      Add Level
                                  </td>
                              </tr>
                          </tfoot>
                      </table>
                  </center>
              </td>
          </tr>
        </tbody>
        <tfoot>
            <tr>
                <td colspan="2">
                    <center>
                        <input name="submit" type="submit" value="Submit"/>
                        <span>&nbsp;&bullet;&nbsp;</span>
                        <input name="reset" type="reset" value="Reset" onClick="vol_form_clear()"/>
                        <span>&nbsp;&bullet;&nbsp;</span>
                        <input name="cancel" type="button" value="Cancel" onClick="mainloadcancel()"/>
                    </center>
                </td>
            </tr>
        </tfoot>
    </table>
</form>
`;
			break;
*/

/*
ajax(
    "list_levels",
    null,
    function titles_callback(data){
window.cb_rest--;
if(data.length==0)
{
$(6,"#levelsTable tbody").AppendChildren(
$(4,"tr").AppendChildren(
$(4,"td").SetAttributes({colspan:"4"}).SetText(2,"Error: Server responded with no data!")
)
);
}
else
{
var xdata=JSON.parse(data).data;
console.error(xdata);
for(var i=0;i<xdata.length;i++)
{
var levels=[];
for(var j=0;j<xdata[i].levels.length;j++)
levels.push($(5,xdata[i].levels[j].name+": "+xdata[i].levels[j].level+"%"),$(4,"br"));
$(6,"#levelsTable tbody").AppendChildren(
$(4,"tr").AppendChildren(
$(4,"td").SetText(2,xdata[i].context),
$(4,"td").SetText(2,xdata[i].application),
$(4,"td").SetText(2,xdata[i].name),
$(4,"td").AppendChildren(levels)
)
);
}
if(xdata.length==0)
{
$(6,"#levelsTable tbody").AppendChildren(
$(4,"tr").AppendChildren(
$(4,"td").SetAttributes({colspan:"4"}).SetText(2,"No Volume Levels")
)
);
}
}
if(window.cb_rest==0)
$(0,"cover").parentElement.removeChild($(0,"cover"));
},function titles_onerror(xhr){
window.cb_rest--;
$(6,"#levelsTable tbody").AppendChildren(
$(4,"tr").AppendChildren(
$(4,"td").SetAttributes({colspan:"4"}).SetText(2,"Error: Server responded with status "+xhr.status+" ("+xhr.statusText+")")
)
);
if(window.cb_rest==0)
$(0,"cover").parentElement.removeChild($(0,"cover"));
});
*/
/*

function vol_form_add()
{
	$(6,"#addVolLevelTable tbody").appendChild($(4,"tr").AppendChildren(
		$(4,"td").AppendChildren(
			$(4,"select").SetAttributes({onchange:"vol_form_shhi(this.parentElement)",name:"presetname[]",title:"Select a Preset Name or select CUSTOM",required:"required"}).AppendChildren(
				$(4,"option").SetAttributes({value:"Application"}).SetText(2,"Application"),
				$(4,"option").SetAttributes({value:"OBS Microphone"}).SetText(2,"OBS Microphone"),
				$(4,"option").SetAttributes({value:"OBS System"}).SetText(2,"OBS System"),
				$(4,"option").SetAttributes({value:"System"}).SetText(2,"System"),
				$(4,"option").SetText(2,"==========="),
				$(4,"option").SetAttributes({value:"custom"}).SetText(2,"[CUSTOM]")
			),
			$(4,"span").SetText(0," "),
			$(4,"input").SetStyles({display:"none"}).SetAttributes({name:"customname[]",type:"text",value:"[Custom Name]",title:"Custom name, if preset set to Custom",required:"required"})
		),
		$(4,"td").AppendChildren(
			$(4,"input").SetAttributes({name:"level[]",type:"number",placeholder:"[Level]",title:"Level, 0-100 inclusive.",min:"0",max:"100",required:"required"})
		),
		$(4,"td").SetAttributes({"class":"removerow noborder",onClick:"vol_form_rem(this.parentElement)"}).SetText(0,"&ndash;")
	));
}
function vol_form_rem(row) {
	row.parentElement.removeChild(row);
}
function vol_form_shhi(cell) {
	cell.children[2].style.display = (cell.children[0].value=="custom")
	    ? "initial"
	    : "none";
}
function vol_form_clear() {
	$(6,"#addVolLevelTable tbody").SetText(0,"");
}


*/
/*

            <!--<div id="levels" class="section">
                <table id="levelsTable">
                    <thead>
                        <tr>
                            <th colspan="4">Volume Levels</th>
                        </tr>
                        <tr>
                            <th>Context</th>
                            <th>Application</th>
                            <th>Name</th>
                            <th>Volume Levels</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                    <tfoot>
                        <tr>
                            <td colspan="4" class="addrow" onClick="load('add_vol_form');">
                                Add Volume Level
                            </td>
                        </tr>
                    </tfoot>
                </table>
                <p/>
                <br/>
            </div>-->
*/
/*
HTMLElement.prototype.SetAttributes = function SetAttributes(attribs) {
    var keys;
    var key;
    var i;
    if (attribs) {
        keys = Object.keys(attribs);
        for (i = 0; i < keys.length; i += 1) {
            key = keys[i];
            this.setAttribute(key,attribs[key]);
        }
    }
    return this;
};

HTMLElement.prototype.SetStyles = function SetStyles(styles) {
    var keys;
    var key;
    var i;
    if (styles) {
        keys = Object.keys(styles);
        for (i = 0; i < keys.length; i += 1) {
            key = keys[i];
            this.style[key] = styles[key];
        }
    }
    return this;
};

HTMLElement.prototype.SetText = function SetText(type, text) {
    switch(type) {
    default:
    case 0:
        this.innerHTML = text;
        break;
    case 1:
        this.outerHTML = text;
        break;
    case 2:
        this.textContent = text;
        break;
    case 3:
        this.innerText = text;
        break;
    case 4:
        this.value = text;
        break;
    }
    return this;
};
*/
/*


function $(a,b,c) {
    switch (a) {
    case 0:
        return document.getElementById(b);
    case 1:
        return (!c)
                ? document.getElementsByTagName(b)
                : document.getElementsByTagName(b)[c];
    case 2:
        return (!c)
                ? document.getElementsByClassName(b)
                : document.getElementsByClassName(b)[c];
    case 3:
        return (!c)
                ? document.getElementsByName(b)
                : document.getElementsByName(b)[c];
    case 4:
        return document.createElement(b);
    case 5:
        return document.createTextNode(b);
    case 6:
        return document.querySelector(b);
    case 7:
        return (!c)
                ? document.querySelectorAll(b)
                : document.querySelectorAll(b)[c];
    //case 8:
    //    return DoNode(b);
    }
}
*/