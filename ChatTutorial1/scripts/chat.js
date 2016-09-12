// Declare a proxy to reference the hub.
var chat = $.connection.chatHub;
var typingTimer = Date();

function sendMessage(displayName, message) {
    if (message != "") {
        // Call the Send method on the hub.
        chat.server.send(displayName, message);
        // Clear text box and reset focus for next comment.
        $('#message').val('').focus();
    }
}

function getChatLogs(callback) {
    chat.server.getChatLogs().done(function() {
        callback();
        //updateScroll();
    });
}

function checkTyping() {
    setTimeout(checkTyping, 500);
    if (((Date.now() - typingTimer) > 500)) {
        //$('#typingIndicator').empty();
        document.getElementById("typingIndicator").style.visibility = "hidden";
    }
}

//function updateScroll() {
//    setInterval(updateScroll, 100);
//    var element = document.getElementById("container");
//    element.scrollTop = element.scrollHeight;
//}

// Start the connection.
$.connection.hub.start()
    .done(function () {
        // Get the user name and store it to prepend to messages.
        var displayName = prompt('Enter your name:', '');
        $('#displayname').val(displayName);
        document.getElementById("userName").innerHTML ="Logged in: "+$('#displayname').val() 
        // Set initial focus to message input box.
        $('#message').focus();

        //Get history
        getChatLogs(function () {
            sendMessage('', displayName + ' has entered the room');
        });
        
        checkTyping();

        //register click event
        $('#sendmessage').click(function () {
            sendMessage($("#displayname").val(), $("#message").val());
        });

        $("#message").keypress(function (event) {
            //send message on pressing enter
            if (event.which === 13) {
                sendMessage($('#displayname').val(), $('#message').val());
            } else {
                //handle typing notification
                typingTimer = Date.now();
                chat.server.typingIndicator(displayName);
            }
        });

        window.addEventListener('beforeunload', function (event) {
            sendMessage('', $('#displayname').val()+" has left the room");
        }, false);
    });

//broadcast messages.
chat.client.broadcastMessage = function (name, message) {
    // Html encode display name and message.
    var encodedName = $('<div />').text(name).html();
    var encodedMsg = $('<div />').text(message).html();
    // Add the message to the page.
    
    if (name != '') {
        $('#discussion').append('<strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;');
    }

    if (name === "") {
        $('#discussion').append('-- ');
    }

    $('#discussion').append(encodedMsg);

    if (name === "") {
        $('#discussion').append(' --');
    }

    $('#discussion').append('<br/>');

};

chat.client.someoneIsTyping = function (name) {
    document.getElementById("typingIndicator").innerHTML = name + " is typing...";
    document.getElementById("typingIndicator").style.visibility = "visible";
    typingTimer = Date.now();
}


//todo: scrolling