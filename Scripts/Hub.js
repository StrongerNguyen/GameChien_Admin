$(document).ready(function () {
    var soundDeposit = '/Content/audio/deposit.mp3';
    var soundWithdraw = '/Content/audio/withdraw.mp3';
    function playSound(mysound) {
        var audio = new Audio(mysound);
        audio.play();
    }
    var chat = $.connection.realtimeHub;

    chat.client.newDeposit = function () {
        playSound(soundDeposit);
    };
    chat.client.newWithdraw = function () {
        playSound(soundWithdraw);
    };
    chat.client.reloadPage = function () {
        location.reload();
    };
    $.connection.hub.start().done(function () {
    });
    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 5000);
    });
});