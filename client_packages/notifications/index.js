let browser = null;
var lastHelpNotify = "";

mp.events.add("sendPlayerNotification", (message, duration, color, title) => {
    if (browser == null) {
        browser = mp.browsers.new("package://notifications/ui.html");
    }
    
    mp.game.audio.playSoundFrontend(1, "ATM_WINDOW", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);

     if(color == "n")
        color = "#e44343";

    browser.execute(`showNotify('`+ message +`', ` + duration + `, '` + color + `', '` + title + `');`);
});

mp.events.add("showHelpNotification", (message) => {
    if (browser == null) {
        browser = mp.browsers.new("package://notifications/ui.html");
    }

    if(lastHelpNotify != message && !mp.gui.cursor.visible) {

        var color = "#e44343";
        var title = "INFORMATION";
        var duration = 5000;

        lastHelpNotify = message;

        mp.game.audio.playSoundFrontend(1, "ATM_WINDOW", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);

        browser.execute(`showNotify('`+ message +`', ` + duration + `, '` + color + `', '` + title + `');`);
    }
});

function resetHelpNotify() {
    lastHelpNotify = "";

    setTimeout(() => {
        resetHelpNotify();
    }, 25000);
}

resetHelpNotify();