"use strict";

let hudBrowser = null;
let hudReady = false;

const hudState = {
    money: 0,
    blackMoney: 0,
    voiceRange: 2,
    foodState: 3,
    drinkState: 3,
    adminDuty: false
};

function executeHud(command) {
    if (!hudBrowser || !hudReady) {
        return;
    }

    hudBrowser.execute(command);
}

function syncHud() {
    executeHud(`window.KHud && window.KHud.update(${JSON.stringify(hudState)});`);
}

function createHud() {
    if (hudBrowser) {
        syncHud();
        return;
    }

    hudBrowser = mp.browsers.new("package://HUD/HUD.html");
    hudReady = false;
}

mp.events.add("client:startdatetime", createHud);

mp.events.add("hud:ready", () => {
    hudReady = true;
    syncHud();
});

mp.events.add("onPlayerLoaded", (...args) => {
    hudState.money = args[6];
    hudState.blackMoney = args[26] || 0;
    syncHud();
});

mp.events.add("updateMoney", money => {
    hudState.money = money;
    syncHud();
});

mp.events.add("updateBlackMoney", money => {
    hudState.blackMoney = money;
    syncHud();
});

mp.events.add("setVoiceType", voiceRange => {
    hudState.voiceRange = Number(voiceRange) || 2;
    syncHud();
});

mp.events.add("setNutrition", (eating, drinking) => {
    hudState.foodState = eating;
    hudState.drinkState = drinking;
    syncHud();
});

mp.events.add("setNutritionEating", state => {
    hudState.foodState = state;
    syncHud();
});

mp.events.add("setNutritionDrinking", state => {
    hudState.drinkState = state;
    syncHud();
});

mp.events.add("updateAduty", state => {
    hudState.adminDuty = Boolean(state);
    syncHud();
});

mp.events.add("setPlayerAduty", state => {
    hudState.adminDuty = Boolean(state);
    syncHud();
});

mp.events.add("client:close", () => {
    if (!hudBrowser) {
        return;
    }

    hudBrowser.destroy();
    hudBrowser = null;
    hudReady = false;
});
