//WebSocket Client Snippet
function main() {
    //var socket = new WebSocket("ws://localhost:8069/audiopeaks");
    var socket = new WebSocket("ws://localhost:8069/AudioPeaks");
    socket.addEventListener("open", function () {
        console.info("CONNECTED SUCCESSFULLY");
        //document.body.textContent = "CONNECTED SUCCESSFULLY";
    });
    socket.addEventListener("error", function (e) {
        console.info("ERROR:", e);
        document.body.textContent = `ERROR${e}`;
    });
    socket.addEventListener("message", function (e) {
        console.info("MESSAGE:", e.data);
        var span = document.createElement("SPAN");
        span.textContent = `NEW MESSAGE: ${e.data}`;
        if (document.body.children.length === 0) {
            document.body.innerHTML = "";
            document.body.appendChild(document.createElement("BR"));
            document.body.appendChild(span);
        } else {
            document.body.insertBefore(document.createElement("BR"), document.body.children[0]);
            document.body.insertBefore(span, document.body.children[0]);
        }
            
    });
    window.testSocket = socket;
    window.setInterval(clearBody,5000);
}

function clearBody() {
    document.body.innerHTML = "";
}

if (document.readyState !== "complete") {
    window.addEventListener("load", main);
} else {
    main();
}