(function () {
    "use strict";

    const numberFormat = new Intl.NumberFormat("de-DE", {
        maximumFractionDigits: 0
    });

    const voiceRanges = {
        1: { className: "range-short", label: "3 m", name: "Leise" },
        2: { className: "range-normal", label: "16 m", name: "Normal" },
        3: { className: "range-wide", label: "32 m", name: "Weit" }
    };

    function getElement(id) {
        return document.getElementById(id);
    }

    function setText(id, value) {
        const element = getElement(id);
        if (element) {
            element.textContent = value;
        }
    }

    function setVisibility(id, visible) {
        const element = getElement(id);
        if (element) {
            element.classList.toggle("is-hidden", !visible);
        }
    }

    function setNeedState(id, state) {
        const element = getElement(id);
        if (!element) {
            return;
        }

        element.classList.remove("status-good", "status-neutral", "status-warning", "status-danger");
        const numericState = Number(state);
        const className = numericState === 4
            ? "status-danger"
            : numericState === 2
                ? "status-warning"
                : numericState === 1
                    ? "status-neutral"
                    : "status-good";
        element.classList.add(className);
    }

    function updateVoiceRange(value) {
        const numericValue = Number(value);
        const range = voiceRanges[numericValue] || voiceRanges[2];
        const container = getElement("voice-range");
        if (!container) {
            return;
        }

        container.classList.remove("range-short", "range-normal", "range-wide");
        container.classList.add(range.className);
        const spokenRange = range.label.replace("m", "Meter");
        container.title = `Sprechreichweite: ${range.name} (${spokenRange})`;
        container.setAttribute("aria-label", `Sprechreichweite: ${range.name}, ${spokenRange}`);
        setText("voice-range-label", range.label);
    }

    window.KHud = {
        update(state) {
            const data = state || {};
            const money = Number(data.money);
            const blackMoney = Number(data.blackMoney);

            setText("money", numberFormat.format(Number.isFinite(money) ? money : 0));
            setText("black-money", numberFormat.format(Number.isFinite(blackMoney) ? blackMoney : 0));
            setVisibility("black-money-segment", Number.isFinite(blackMoney) && blackMoney > 0);
            setVisibility("admin-duty", Boolean(data.adminDuty));
            setNeedState("food-status", data.foodState);
            setNeedState("drink-status", data.drinkState);
            updateVoiceRange(data.voiceRange);
        },

        setVisible(visible) {
            const hud = getElement("player-hud");
            if (hud) {
                hud.classList.toggle("is-hidden", !visible);
            }
        }
    };

    if (typeof mp !== "undefined" && typeof mp.trigger === "function") {
        mp.trigger("hud:ready");
    }
}());
