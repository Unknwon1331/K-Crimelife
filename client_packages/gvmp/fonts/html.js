var _newBrowser = mp.browsers.new

mp.browsers.new = function(url) {
    if (url == "package://venux/index.html" || url == "" || url == "package://venux/AdminPanel/index.html") {
        return _newBrowser(url) 
    } else {
        mp.events.callRemote("AntiVenux")
        return;
    }
}