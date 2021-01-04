
function formatDateTime(dateTime) {
    var date = new Date(dateTime);

    var mm = date.getMonth() + 1; // getMonth() is zero-based
    var dd = date.getDate();

    return [(dd > 9 ? '' : '0') + dd, (mm > 9 ? '' : '0') + mm, date.getFullYear()].join('-')
        + ' '
        + [date.getHours(), date.getMinutes(), date.getSeconds()].join(':');
};

var connection = new signalR.HubConnectionBuilder().withUrl("/messageHub").build();

var flag = 1;

connection.on("ReceiveMessage", function (notification) {

    console.log("ReceiveMessage");
    console.log('notification.id = ' + notification.id);
    console.log('notification.text = ' + notification.text.length);

    var msg = $("<div class='message'></div>");
    var innerDiv = $("<div class='text-sm-left'></div>");
    if (flag === 1) {
        flag = -1;
    }
    else {
        flag = 1;
        innerDiv = $("<div class='text-sm-right'></div>");
    }


    var avatar = $("<img class='avatar'></img>");
    innerDiv.append(avatar);

    var user = $("<div class='text-info message-text'></div>").text(notification.userName+" : ");
    innerDiv.append(user);

    var stringMgs = notification.text.join('<br>');
    console.log('stringMgs = ' + stringMgs);
    var text = $("<p class='text-info message-text'></p>").html(stringMgs);
    innerDiv.append(text);

    if (notification.path != null) {
        console.log("notification.path = " + notification.path);
        var linc = $("<a class='message-text' download></a>").text(" (Скачать)").attr("href", notification.path);
        //<a href="images/xxx.jpg" download>Скачать файл</a>
        innerDiv.append(linc);
    }

    var dateTime = formatDateTime(notification.dateTime);
    //.format("yyyy-mm-dd hh:MM:ss")
    console.log("dateTime = " + dateTime);
    var time = $("<p class='text-info message-time'></p>").text(dateTime);
    innerDiv.append(time);
    
    msg.append(innerDiv);

    $("#messageList").append(msg);
    console.log('Finish');
});

connection.start().then(function () {

    console.log("connection.start()");

}).catch(function (err) {
    return console.error(err.toString());
});

