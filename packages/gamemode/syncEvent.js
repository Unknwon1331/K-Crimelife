"use strict";

const clothesChangeCooldowns = new Map();

const COMPONENT_MIN = 0;
const COMPONENT_MAX = 11;
const DRAWABLE_MIN = 0;
const DRAWABLE_MAX = 500;
const TEXTURE_MIN = 0;
const TEXTURE_MAX = 100;
const CHANGE_COOLDOWN_MS = 500;

function parseInteger(value) {
    const parsedValue = Number(value);

    if (!Number.isInteger(parsedValue)) {
        return null;
    }

    return parsedValue;
}

function isValidPlayer(player) {
    return player && mp.players.exists(player);
}

mp.events.add(
    "ChangeClothes",
    (player, rawComponentId, rawDrawable, rawTexture) => {
        if (!isValidPlayer(player)) {
            return;
        }

        const componentId = parseInteger(rawComponentId);
        const drawable = parseInteger(rawDrawable);
        const texture = parseInteger(rawTexture);

        if (
            componentId === null ||
            drawable === null ||
            texture === null
        ) {
            return;
        }

        if (
            componentId < COMPONENT_MIN ||
            componentId > COMPONENT_MAX
        ) {
            return;
        }

        if (
            drawable < DRAWABLE_MIN ||
            drawable > DRAWABLE_MAX
        ) {
            return;
        }

        if (
            texture < TEXTURE_MIN ||
            texture > TEXTURE_MAX
        ) {
            return;
        }

        const now = Date.now();
        const lastChange =
            clothesChangeCooldowns.get(player.id) || 0;

        if (now - lastChange < CHANGE_COOLDOWN_MS) {
            return;
        }

        clothesChangeCooldowns.set(player.id, now);

        try {
            player.changeClothes(
                componentId,
                drawable,
                texture,
                false,
                true
            );
        } catch (error) {
            console.error(
                `[ChangeClothes] Fehler bei Spieler ${player.name} (${player.id}):`,
                error
            );
        }
    }
);

mp.events.add("playerQuit", player => {
    if (!player) {
        return;
    }

    clothesChangeCooldowns.delete(player.id);
});