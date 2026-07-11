"use strict";

const AUTHORIZATION_PREFIX = "WC_AUTH_";
const AUTHORIZATION_LIFETIME_MS = 5000;

function serializeComponentSet(dataSet) {
    return Array.from(dataSet)
        .map(hash => hash.toString(36))
        .join("|");
}

function normalizeUInt(value) {
    const parsed = Number(value);

    if (!Number.isInteger(parsed) || parsed < 0 || parsed > 0xffffffff) {
        return null;
    }

    return parsed;
}

function ensureComponentStore(player) {
    if (!player.__weaponComponents ||
        typeof player.__weaponComponents !== "object") {
        player.__weaponComponents = {};
    }
}

function consumeAuthorization(
    player,
    authorizationKey,
    action,
    weaponHash,
    componentHash
) {
    if (!player || !mp.players.exists(player) ||
        typeof authorizationKey !== "string" ||
        !authorizationKey.startsWith(AUTHORIZATION_PREFIX) ||
        !/^WC_AUTH_[a-f0-9]{32}$/.test(authorizationKey)) {
        return false;
    }

    const authorization = player.getVariable(authorizationKey);
    if (typeof authorization !== "string") {
        return false;
    }

    // Die Freigabe wird vor jeder weiteren Prüfung verbraucht. Dadurch kann
    // ein gültiger Aufruf nicht wiederholt werden.
    player.setVariable(authorizationKey, null);

    const parts = authorization.split("|");
    if (parts.length !== 4) {
        return false;
    }

    const expiresAt = Number(parts[3]);
    const now = Date.now();

    return parts[0] === action &&
        parts[1] === String(weaponHash) &&
        parts[2] === String(componentHash) &&
        Number.isFinite(expiresAt) &&
        expiresAt >= now &&
        expiresAt <= now + AUTHORIZATION_LIFETIME_MS;
}

mp.Player.prototype.giveWeaponComponent = function(
    weaponHash,
    componentHash
) {
    ensureComponentStore(this);

    if (!Object.prototype.hasOwnProperty.call(
        this.__weaponComponents,
        weaponHash
    )) {
        this.__weaponComponents[weaponHash] = new Set();
    }

    this.__weaponComponents[weaponHash].add(componentHash);

    if (this.weapon === weaponHash) {
        this.setVariable(
            "currentWeaponComponents",
            weaponHash.toString(36) + "." +
            serializeComponentSet(this.__weaponComponents[weaponHash])
        );
    } else {
        mp.players.callInRange(
            this.position,
            mp.config["stream-distance"],
            "updatePlayerWeaponComponent",
            [
                this,
                weaponHash.toString(36),
                componentHash.toString(36),
                false
            ]
        );
    }
};

mp.Player.prototype.removeWeaponComponent = function(
    weaponHash,
    componentHash
) {
    ensureComponentStore(this);

    if (!Object.prototype.hasOwnProperty.call(
        this.__weaponComponents,
        weaponHash
    )) {
        return;
    }

    this.__weaponComponents[weaponHash].delete(componentHash);

    if (this.weapon === weaponHash) {
        this.setVariable(
            "currentWeaponComponents",
            weaponHash.toString(36) + "." +
            serializeComponentSet(this.__weaponComponents[weaponHash])
        );
    } else {
        mp.players.callInRange(
            this.position,
            mp.config["stream-distance"],
            "updatePlayerWeaponComponent",
            [
                this,
                weaponHash.toString(36),
                componentHash.toString(36),
                true
            ]
        );
    }
};

mp.Player.prototype.removeAllWeaponComponents = function(weaponHash) {
    ensureComponentStore(this);

    if (!Object.prototype.hasOwnProperty.call(
        this.__weaponComponents,
        weaponHash
    )) {
        return;
    }

    if (this.weapon === weaponHash) {
        this.setVariable(
            "currentWeaponComponents",
            weaponHash.toString(36) + "."
        );
    } else {
        mp.players.callInRange(
            this.position,
            mp.config["stream-distance"],
            "resetPlayerWeaponComponents",
            [this, weaponHash.toString(36)]
        );
    }

    delete this.__weaponComponents[weaponHash];
};

mp.Player.prototype.resetAllWeaponComponents = function() {
    ensureComponentStore(this);

    if (Object.prototype.hasOwnProperty.call(
        this.__weaponComponents,
        this.weapon
    )) {
        this.setVariable(
            "currentWeaponComponents",
            this.weapon.toString(36) + "."
        );
    }

    mp.players.callInRange(
        this.position,
        mp.config["stream-distance"],
        "nukePlayerWeaponComponents",
        [this]
    );
    this.__weaponComponents = {};
};

mp.events.add(
    "weaponComponentAction",
    (player, authorizationKey, action, rawWeaponHash, rawComponentHash) => {
        const weaponHash = normalizeUInt(rawWeaponHash);
        const componentHash = normalizeUInt(rawComponentHash);

        if (weaponHash === null || componentHash === null ||
            !["give", "remove", "removeAll", "resetAll"].includes(action) ||
            !consumeAuthorization(
                player,
                authorizationKey,
                action,
                weaponHash,
                componentHash
            )) {
            return;
        }

        switch (action) {
            case "give":
                player.giveWeaponComponent(weaponHash, componentHash);
                break;
            case "remove":
                player.removeWeaponComponent(weaponHash, componentHash);
                break;
            case "removeAll":
                player.removeAllWeaponComponents(weaponHash);
                break;
            case "resetAll":
                player.resetAllWeaponComponents();
                break;
        }
    }
);

mp.events.add("playerJoin", player => {
    player.__weaponComponents = {};
});

mp.events.add("playerWeaponChange", (player, oldWeapon, newWeapon) => {
    ensureComponentStore(player);
    const componentSet = Object.prototype.hasOwnProperty.call(
        player.__weaponComponents,
        newWeapon
    )
        ? serializeComponentSet(player.__weaponComponents[newWeapon])
        : "";

    player.setVariable(
        "currentWeaponComponents",
        newWeapon.toString(36) + "." + componentSet
    );
});
