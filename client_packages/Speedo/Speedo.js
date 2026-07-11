"use strict";

const localPlayer = mp.players.local;

let speedoBrowser = null;
let visible = false;
let speedoReady = false;
let lastRenderUpdate = 0;

const speedoState = {
    speed: 0,
    maxSpeed: 240,
    gear: 0,
    fuel: null,
    maxFuel: null,
    mileage: null,
    locked: false,
    engine: false
};

function finiteNumber(value) {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : null;
}

function normalizeLock(value) {
    if (typeof value === "boolean") {
        return value;
    }

    const numeric = Number(value);
    return numeric === 2 || numeric === 4 || numeric === 7 || numeric === 10;
}

function executeSpeedo(command) {
    if (!speedoBrowser || !speedoReady) {
        return;
    }

    speedoBrowser.execute(command);
}

function syncSpeedo() {
    executeSpeedo(`window.KSpeedo && window.KSpeedo.update(${JSON.stringify(speedoState)});`);
}

function setVisible(state) {
    if (visible === state) {
        return;
    }

    visible = state;
    if (state) {
        speedoBrowser.active = true;
        executeSpeedo("window.KSpeedo && window.KSpeedo.setVisible(true);");
        syncSpeedo();
        return;
    }

    executeSpeedo("window.KSpeedo && window.KSpeedo.setVisible(false);");
    setTimeout(() => {
        if (!visible) {
            speedoBrowser.active = false;
        }
    }, 200);
}

function isLocalPlayerDriver() {
    const vehicle = localPlayer.vehicle;
    return Boolean(vehicle && vehicle.getPedInSeat(-1) === localPlayer.handle);
}

function readSharedVehicleData(vehicle) {
    const fuel = finiteNumber(vehicle.getVariable("fuel"));
    const maxFuel = finiteNumber(vehicle.getVariable("maxFuel"));
    const mileage = finiteNumber(vehicle.getVariable("mileage"));
    const lockedStatus = vehicle.getVariable("lockedStatus");
    const engineStatus = vehicle.getVariable("engineStatus");

    if (fuel !== null) {
        speedoState.fuel = fuel;
    }
    if (maxFuel !== null) {
        speedoState.maxFuel = maxFuel;
    }
    if (mileage !== null) {
        speedoState.mileage = mileage;
    }
    if (typeof lockedStatus === "boolean") {
        speedoState.locked = lockedStatus;
    }
    if (typeof engineStatus === "boolean") {
        speedoState.engine = engineStatus;
    }
}

mp.events.add("initialVehicleData", (fuel, maxFuel, health, maxHealth, maxSpeed, locked, mileage, engine) => {
    speedoState.fuel = finiteNumber(fuel);
    speedoState.maxFuel = finiteNumber(maxFuel);
    speedoState.maxSpeed = finiteNumber(maxSpeed) || speedoState.maxSpeed;
    speedoState.locked = normalizeLock(locked);
    speedoState.mileage = finiteNumber(mileage);
    speedoState.engine = Boolean(engine);
    syncSpeedo();
});

mp.events.add("speedo:ready", () => {
    speedoReady = true;
    syncSpeedo();
    executeSpeedo(`window.KSpeedo && window.KSpeedo.setVisible(${visible ? "true" : "false"});`);

    if (!visible) {
        speedoBrowser.active = false;
    }
});

mp.events.add("updateVehicleData", (newFuel, newDistance, newHealth, locked, engine) => {
    speedoState.fuel = finiteNumber(newFuel);
    speedoState.mileage = finiteNumber(newDistance);
    speedoState.locked = normalizeLock(locked);
    speedoState.engine = Boolean(engine);
    syncSpeedo();
});

mp.events.add("playerLeaveVehicle", () => {
    setVisible(false);
});

mp.events.add("render", () => {
    if (!isLocalPlayerDriver()) {
        setVisible(false);
        return;
    }

    setVisible(true);
    const now = Date.now();
    if (now - lastRenderUpdate < 100) {
        return;
    }

    lastRenderUpdate = now;
    const vehicle = localPlayer.vehicle;
    readSharedVehicleData(vehicle);
    speedoState.speed = Math.max(0, vehicle.getSpeed() * 3.6);
    speedoState.gear = vehicle.gear || 0;
    speedoState.engine = vehicle.getIsEngineRunning();

    if (typeof vehicle.getDoorLockStatus === "function") {
        speedoState.locked = normalizeLock(vehicle.getDoorLockStatus());
    }

    syncSpeedo();
});

// Erst nach der Registrierung des Ready-Events erzeugen, damit ein sehr
// schneller CEF-Load den Handshake nicht vorzeitig senden kann.
speedoBrowser = mp.browsers.new("package://Speedo/index.html");
