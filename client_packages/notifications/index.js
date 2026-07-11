"use strict";

let notificationBrowser = null;
let notificationReady = false;
let lastHelpNotification = "";
const pendingNotifications = [];

function ensureNotificationBrowser() {
    if (notificationBrowser) {
        return;
    }

    notificationReady = false;
    notificationBrowser = mp.browsers.new("package://notifications/ui.html");
}

function deliverNotification(payload) {
    ensureNotificationBrowser();

    if (!notificationReady) {
        pendingNotifications.push(payload);
        return;
    }

    notificationBrowser.execute(`window.KNotifications && window.KNotifications.push(${JSON.stringify(payload)});`);
}

function flushNotifications() {
    if (!notificationBrowser || !notificationReady) {
        return;
    }

    while (pendingNotifications.length > 0) {
        const payload = pendingNotifications.shift();
        notificationBrowser.execute(`window.KNotifications && window.KNotifications.push(${JSON.stringify(payload)});`);
    }
}

mp.events.add("notifications:ready", () => {
    notificationReady = true;
    flushNotifications();
});

mp.events.add("sendPlayerNotification", (message, duration, color, title) => {
    const normalizedMessage = String(message == null ? "" : message);
    if (normalizedMessage.indexOf("1337Allahuakbar") === 0 ||
        normalizedMessage === "1337$LOTTOHabibi") {
        return;
    }

    deliverNotification({
        scope: "player",
        message: normalizedMessage,
        duration,
        color,
        title
    });
});

mp.events.add("sendGlobalNotification", (message, duration, color, icon) => {
    deliverNotification({
        scope: "global",
        message,
        duration,
        color,
        icon,
        title: "Ankündigung"
    });
});

mp.events.add("showHelpNotification", message => {
    const normalizedMessage = String(message == null ? "" : message);
    if (mp.gui.cursor.visible || normalizedMessage === lastHelpNotification) {
        return;
    }

    lastHelpNotification = normalizedMessage;
    mp.game.audio.playSoundFrontend(1, "ATM_WINDOW", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);

    deliverNotification({
        scope: "player",
        message: normalizedMessage,
        duration: 5000,
        color: "lightblue",
        title: "Information"
    });
});

setInterval(() => {
    lastHelpNotification = "";
}, 25000);

mp.events.add("client:close", () => {
    if (!notificationBrowser) {
        return;
    }

    notificationBrowser.destroy();
    notificationBrowser = null;
    notificationReady = false;
    pendingNotifications.length = 0;
});
