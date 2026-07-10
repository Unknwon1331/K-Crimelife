let kitBrowser = null;
let player = mp.players.local;
let ffaselection = false;

mp.events.add("client:createKitBrowser", (ffa) => {
    if (kitBrowser != null) return;
    if (mp.gui.cursor.visible) return;

    kitBrowser = mp.browsers.new("package://Waffenauswahl/index.html");
    ffaselection = ffa;
    mp.gui.cursor.show(true, true);
});

mp.events.add("client:deleteKitBrowser", () => {
    if (kitBrowser == null) return;

    kitBrowser.destroy();
    kitBrowser = null;
    mp.gui.cursor.show(false, false);
});

mp.events.add("client:selectKit2", (kitNumber) => {
    if (kitNumber == 0 || kitNumber <= 0) return;

    mp.events.callRemote("server:selectKit", kitNumber);
});

mp.events.add("client:selectKit:chooseGW", (GWid) => {
    if (GWid <= 0 || GWid == undefined) return;
    if (ffaselection) {
        mp.events.callRemote("client:selectKit:chooseGW", parseInt(GWid));
    } else {
        mp.events.callRemote("client:selectKit:chooseGW", parseInt(GWid));
    }
});