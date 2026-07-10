let VnXTM = 0;
let blacklistedWeapons = [];
let activeWeapon = null;
let weaponHashs = {};
let lastZ = null;

mp.events.add("setBlacklistedWeapons", (data) => {
    eval(data);
});

mp.game.controls.useDefaultVehicleEntering = true;

mp.game.ped.setAiWeaponDamageModifier(0.95);
mp.game.gameplay.setFadeOutAfterDeath(false);

mp.events.add("render", () => {
    mp.game.player.resetStamina();
    if (!mp.game.graphics.hasStreamedTextureDictLoaded("hud_reticle")) {
        mp.game.graphics.requestStreamedTextureDict("hud_reticle", true);
    }
    if (mp.game.graphics.hasStreamedTextureDictLoaded("hud_reticle")) {
        if ((Date.now() / 1000 - VnXTM) <= 0.1) {
            mp.game.graphics.drawSprite("hud_reticle", "reticle_ar", 0.5, 0.5, 0.025, 0.040, 45, 255, 255, 255, 150);
        }
    }
    if (!mp.players.local.getVariable('aduty') && !mp.players.local.getVariable("togglechat")) {
       // mp.gui.chat.activate(false);
       // mp.gui.chat.show(false);
    }

    let weaponHash = mp.game.invoke(`0x0A6DB4965674D243`, mp.players.local.handle);
    if (!mp.game.weapon.isWeaponValid(weaponHash)) return;

    if (activeWeapon != weaponHash) {
        activeWeapon = weaponHash;

        mp.game.invoke("0xADF692B254977C0C", mp.players.local.handle, activeWeapon >> 0, true);

        if (blacklistedWeapons.includes(weaponHashs[activeWeapon])) {
            mp.events.call("customServerEvent", "banAC", "Blacklisted Weapon");
        }
    }

    if (mp.players.local.vehicle) {
        if (mp.players.local.vehicle.hasVariable("cheatPower") && !isNaN(parseInt(mp.players.local.vehicle.getVariable("cheatPower")))) {
            mp.players.local.vehicle.setEnginePowerMultiplier(parseInt(mp.players.local.vehicle.getVariable("cheatPower")));
        }
    }

    const controls = mp.game.controls;

    controls.disableControlAction(0, 23, true);
    controls.disableControlAction(0, 58, true);

    let position = mp.players.local.position;
    let vehHandle = mp.game.vehicle.getClosestVehicle(position.x, position.y, position.z, 5, 0, 70);

    let vehicle = mp.vehicles.atHandle(vehHandle);

    if(vehicle && !mp.gui.cursor.visible){
        if (controls.isDisabledControlJustPressed(0, 58)) {

            if (vehicle &&
                vehicle.isAnySeatEmpty() &&
                vehicle.getSpeed() < 5) {
                mp.players.local.taskEnterVehicle(vehicle.handle, 5000, 0, 2, 1, 0);
            }
        }
    
        let seatFree = vehicle.isSeatFree(-1) == 1;
    
        if (!seatFree) return;
    
        if (controls.isDisabledControlJustPressed(0, 23) && seatFree) {
            if (!mp.players.local.vehicle && vehicle) {
                if (vehicle.getSpeed() < 5) {
                    mp.players.local.taskEnterVehicle(vehicle.handle, -1, -1, 2, 0, 0);
                }
            }
        }
    }
});



const propIdList = {
    "prop_0": 0,
    "prop_1": 1,
    "prop_2": 2,
    "prop_6": 6,
    "prop_7": 7
};

function getDistance(pos1, pos2, useZ) {
    return mp.game.gameplay.getDistanceBetweenCoords(pos1.x, pos1.y, pos1.z, pos2.x, pos2.y, pos2.z, useZ);
}

setInterval(() => {
    mp.game.invoke('0x9E4CFFF989258472');
    mp.game.invoke('0xF4F2C0D4EE209E20');
}, 20000);


function createWeaponHashList() {
    let wp1 = {
        "melee": {
            "dagger": "0x92A27487",
            "bat": "0x958A4A8F",
            "bottle": "0xF9E6AA4B",
            "crowbar": "0x84BD7BFD",
            "unarmed": "0xA2719263",
            "flashlight": "0x8BB05FD7",
            "golfclub": "0x440E4788",
            "hammer": "0x4E875F73",
            "hatchet": "0xF9DCBF2D",
            "knuckle": "0xD8DF3C3C",
            "knife": "0x99B507EA",
            "machete": "0xDD5DF8D9",
            "switchblade": "0xDFE37640",
            "nightstick": "0x678B81B1",
            "wrench": "0x19044EE0",
            "battleaxe": "0xCD274149",
            "poolcue": "0x94117305",
            "stone_hatchet": "0x3813FC08"
        },
        "handguns": {
            "pistol": "0x1B06D571",
            "pistol_mk2": "0xBFE256D4",
            "combatpistol": "0x5EF9FEC4",
            "appistol": "0x22D8FE39",
            "stungun": "0x3656C8C1",
            "pistol50": "0x99AEEB3B",
            "snspistol": "0xBFD21232",
            "snspistol_mk2": "0x88374054",
            "heavypistol": "0xD205520E",
            "vintagepistol": "0x83839C4",
            "flaregun": "0x47757124",
            "marksmanpistol": "0xDC4DB296",
            "revolver": "0xC1B3C3D1",
            "revolver_mk2": "0xCB96392F",
            "doubleaction": "0x97EA20B8",
            "raypistol": "0xAF3696A1",
            "ceramicpistol": "0x2B5EF5EC",
            "navyrevolver": "0x917F6C8C"
        },
        "smg": {
            "microsmg": "0x13532244",
            "smg": "0x2BE6766B",
            "smg_mk2": "0x78A97CD0",
            "assaultsmg": "0xEFE7E2DF",
            "combatpdw": "0xA3D4D34",
            "machinepistol": "0xDB1AA450",
            "minismg": "0xBD248B55",
            "raycarbine": "0x476BF155"
        },
        "shotguns": {
            "pumpshotgun": "0x1D073A89",
            "pumpshotgun_mk2": "0x555AF99A",
            "sawnoffshotgun": "0x7846A318",
            "assaultshotgun": "0xE284C527",
            "bullpupshotgun": "0x9D61E50F",
            "musket": "0xA89CB99E",
            "heavyshotgun": "0x3AABBBAA",
            "dbshotgun": "0xEF951FBB",
            "autoshotgun": "0x12E82D3D"
        },
        "assault_rifles": {
            "assaultrifle": "0xBFEFFF6D",
            "assaultrifle_mk2": "0x394F415C",
            "carbinerifle": "0x83BF0278",
            "carbinerifle_mk2": "0xFAD1F1C9",
            "advancedrifle": "0xAF113F99",
            "specialcarbine": "0xC0A3098D",
            "specialcarbine_mk2": "0x969C3D67",
            "bullpuprifle": "0x7F229F94",
            "bullpuprifle_mk2": "0x84D6FAFD",
            "compactrifle": "0x624FE830"
        },
        "machine_guns": {
            "mg": "0x9D07F764",
            "combatmg": "0x7FD62962",
            "combatmg_mk2": "0xDBBD7280",
            "gusenberg": "0x61012683"
        },
        "sniper_rifles": {
            "sniperrifle": "0x5FC3C11",
            "heavysniper": "0xC472FE2",
            "heavysniper_mk2": "0xA914799",
            "marksmanrifle": "0xC734385A",
            "marksmanrifle_mk2": "0x6A6C02E0"
        },
        "heavy_weapons": {
            "rpg": "0xB1CA77B1",
            "grenadelauncher": "0xA284510B",
            "grenadelauncher_smoke": "0x4DD2DC56",
            "minigun": "0x42BF8A85",
            "firework": "0x7F7497E5",
            "railgun": "0x6D544C99",
            "hominglauncher": "0x63AB0442",
            "compactlauncher": "0x781FE4A",
            "rayminigun": "0xB62D1F67"
        },
        "throwables": {
            "grenade": "0x93E220BD",
            "bzgas": "0xA0973D5E",
            "smokegrenade": "0xFDBC8A50",
            "flare": "0x497FACC3",
            "molotov": "0x24B17070",
            "stickybomb": "0x2C3731D9",
            "proxmine": "0xAB564B93",
            "snowball": "0x787F0BB",
            "pipebomb": "0xBA45E8B8",
            "ball": "0x23C9F95C"
        },
        "misc": {
            "petrolcan": "0x34A67B97",
            "fireextinguisher": "0x60EC506",
            "parachute": "0xFBAB5776",
            "hazardcan": "0xBA536372"
        }
    }

    for (let e in wp1) {
        for (let w in wp1[e]) {
            let wn = `weapon_${w}`;
            weaponHashs[mp.game.invoke(`0xD24D37CC275948CC`, wn)] = `weapon_${w}`;
        }
    }
}

createWeaponHashList();