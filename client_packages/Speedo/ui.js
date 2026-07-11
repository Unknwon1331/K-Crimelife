(function () {
    "use strict";

    const numberFormat = new Intl.NumberFormat("de-DE", {
        minimumFractionDigits: 0,
        maximumFractionDigits: 1
    });

    function element(id) {
        return document.getElementById(id);
    }

    function finiteNumber(value, fallback) {
        const parsed = Number(value);
        return Number.isFinite(parsed) ? parsed : fallback;
    }

    function setText(id, value) {
        const target = element(id);
        if (target) {
            target.textContent = value;
        }
    }

    function updateFuel(fuel, maxFuel) {
        const target = element("fuel-status");
        if (!target) {
            return;
        }

        const currentFuel = finiteNumber(fuel, null);
        const capacity = finiteNumber(maxFuel, null);
        target.classList.remove("is-warning", "is-critical");

        if (currentFuel === null) {
            setText("fuel", "--");
            return;
        }

        const ratio = capacity && capacity > 0 ? currentFuel / capacity : null;
        if (ratio !== null && ratio <= 0.1) {
            target.classList.add("is-critical");
        } else if (ratio !== null && ratio <= 0.25) {
            target.classList.add("is-warning");
        }

        setText("fuel", `${numberFormat.format(Math.max(0, currentFuel))} L`);
    }

    function updateLock(locked) {
        const target = element("lock-status");
        if (!target) {
            return;
        }

        const isLocked = Boolean(locked);
        target.classList.toggle("is-locked", isLocked);
        target.classList.toggle("is-unlocked", !isLocked);
        target.title = isLocked ? "Fahrzeug verriegelt" : "Fahrzeug entriegelt";
        setText("lock-label", isLocked ? "ZU" : "OFFEN");

        const shackle = element("lock-shackle");
        if (shackle) {
            shackle.setAttribute("d", isLocked
                ? "M8 10V7a4 4 0 0 1 8 0v3"
                : "M8 10V7a4 4 0 0 1 7.4-2.1");
        }
    }

    function updateSpeed(speed, maxSpeed) {
        const safeSpeed = Math.max(0, Math.round(finiteNumber(speed, 0)));
        const safeMaximum = Math.max(1, finiteNumber(maxSpeed, 240));
        const percentage = Math.min(100, safeSpeed / safeMaximum * 100);
        setText("speed", safeSpeed);

        const progress = element("speed-progress");
        if (progress) {
            progress.style.strokeDasharray = `${percentage} 100`;
        }
    }

    window.KSpeedo = {
        update(state) {
            const data = state || {};
            updateSpeed(data.speed, data.maxSpeed);
            updateFuel(data.fuel, data.maxFuel);
            updateLock(data.locked);
            setText("gear", data.gear == null || data.gear === 0 ? "N" : String(data.gear));

            const mileage = finiteNumber(data.mileage, null);
            setText("mileage", mileage === null ? "--" : numberFormat.format(Math.max(0, mileage)));

            const engine = element("engine-indicator");
            if (engine) {
                engine.classList.toggle("is-running", Boolean(data.engine));
            }
        },

        setVisible(visible) {
            const hud = element("vehicle-hud");
            if (hud) {
                hud.classList.toggle("is-hidden", !visible);
            }
        }
    };

    if (typeof mp !== "undefined" && typeof mp.trigger === "function") {
        mp.trigger("speedo:ready");
    }
}());
