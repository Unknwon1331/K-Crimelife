let aufsatzBrowser = null;

mp.events.add("Client:aufsatzBrowser:createBrowser", () => {
    if (aufsatzBrowser == null) aufsatzBrowser = mp.browsers.new("package://Aufsatz/index.html");

    setTimeout(() => {
        mp.gui.cursor.show(true, true);
    }, 50);
});

//Advancedrifle
mp.events.add("Client:aufsatzBrowser:chooseWeaponCompADV", (compid) => {
    if (compid <= 0 || compid == undefined) return;
    mp.events.callRemote("Server:aufsatzBrowser:chooseWeaponCompADV", parseInt(compid));
});

//Bullpuprifle
mp.events.add("Client:aufsatzBrowser:chooseWeaponCompBulli", (compid) => {
    if (compid <= 0 || compid == undefined) return;
    mp.events.callRemote("Server:aufsatzBrowser:chooseWeaponCompBulli", parseInt(compid));
});

//Specialcarbine
mp.events.add("Client:aufsatzBrowser:chooseWeaponCompSpezi", (compid) => {
    if (compid <= 0 || compid == undefined) return;
    mp.events.callRemote("Server:aufsatzBrowser:chooseWeaponCompSpezi", parseInt(compid));
});

//Gusenberg
mp.events.add("Client:aufsatzBrowser:chooseWeaponCompGusi", (compid) => {
    if (compid <= 0 || compid == undefined) return;
    mp.events.callRemote("Server:aufsatzBrowser:chooseWeaponCompGusi", parseInt(compid));
});

mp.events.add("Client:aufsatzBrowser:destroyBrowser", () => {
    if (aufsatzBrowser != null) {
        mp.gui.cursor.show(false, false);
        aufsatzBrowser.destroy();
        aufsatzBrowser = null;
    }
});