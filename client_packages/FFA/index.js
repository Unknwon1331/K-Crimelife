let ffaBrowser = null;

mp.events.add("Client:ffaBrowser:createBrowser", () => {
    if (ffaBrowser == null) ffaBrowser = mp.browsers.new("package://FFA/index.html");

    setTimeout(() => {
        mp.gui.cursor.show(true, true);
    }, 50);
});

mp.events.add("Client:ffaBrowser:chooseFFA", (ffaid) => {
    if (ffaid <= 0 || ffaid == undefined) return;
    mp.events.callRemote("Server:ffaBrowser:chooseFFA", parseInt(ffaid));
});

mp.events.add("Client:ffaBrowser:destroyBrowser", () => {
    if (ffaBrowser != null) {
        mp.gui.cursor.show(false, false);
        ffaBrowser.destroy();
        ffaBrowser = null;
    }
});