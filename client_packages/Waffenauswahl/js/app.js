$("#kit-1").click(function() {
    mp.trigger("client:selectKit:chooseGW", 1);
    mp.trigger("client:deleteKitBrowser");
});

$("#kit-2").click(function() {
    mp.trigger("client:selectKit:chooseGW", 2);
    mp.trigger("client:deleteKitBrowser");
});

$("#kit-3").click(function() {
    mp.trigger("client:selectKit:chooseGW", 3);
    mp.trigger("client:deleteKitBrowser");
});

$("#kit-4").click(function() {
    mp.trigger("client:selectKit:chooseGW", 4);
    mp.trigger("client:deleteKitBrowser");
});

$("#exitButton").click(function() {
    mp.trigger("client:deleteKitBrowser");
});
