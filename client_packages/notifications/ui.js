(function () {
    "use strict";

    const maximumVisible = {
        player: 3,
        global: 1
    };

    const icons = {
        info: '<circle cx="12" cy="12" r="9"></circle><path d="M12 10v6M12 7.25h.01"></path>',
        success: '<circle cx="12" cy="12" r="9"></circle><path d="m8 12 2.6 2.6L16.5 9"></path>',
        warning: '<path d="M10.3 4.1 2.6 18a2 2 0 0 0 1.8 3h15.2a2 2 0 0 0 1.8-3L13.7 4.1a2 2 0 0 0-3.4 0Z"></path><path d="M12 9v4M12 17h.01"></path>',
        error: '<circle cx="12" cy="12" r="9"></circle><path d="m9 9 6 6M15 9l-6 6"></path>',
        global: '<path d="M4 13h3l8 4V5L7 9H4v4ZM7 13l1 6h3"></path><path d="M18 8.5a5 5 0 0 1 0 5"></path>',
        glob: '<path d="M10.3 4.1 2.6 18a2 2 0 0 0 1.8 3h15.2a2 2 0 0 0 1.8-3L13.7 4.1a2 2 0 0 0-3.4 0Z"></path><path d="M12 9v4M12 17h.01"></path>',
        gov: '<path d="M4 13h3l8 4V5L7 9H4v4ZM7 13l1 6h3"></path><path d="M18 8.5a5 5 0 0 1 0 5"></path>',
        dev: '<path d="M12 3 5 6v5c0 4.6 2.8 8 7 10 4.2-2 7-5.4 7-10V6l-7-3Z"></path><path d="M9 11h6M10 8.5h4"></path>',
        wed: '<path d="M6 9a6 6 0 0 1 12 0c0 7 3 7 3 7H3s3 0 3-7"></path><path d="M10 20h4"></path>',
        casino: '<path d="m12 3 8 7-8 11-8-11 8-7Z"></path><path d="m4 10 5-1 3-6 3 6 5 1M9 9l3 12 3-12"></path>'
    };

    function cleanText(value, fallback, maximumLength) {
        const text = String(value == null ? "" : value).replace(/\s+/g, " ").trim();
        const normalized = text || fallback;
        return normalized.length > maximumLength
            ? `${normalized.slice(0, maximumLength - 1)}…`
            : normalized;
    }

    function safeDuration(value) {
        const duration = Number(value);
        if (!Number.isFinite(duration)) {
            return 5000;
        }

        return Math.min(60000, Math.max(1200, Math.round(duration)));
    }

    function normalizeKind(color, title, message) {
        const hint = `${color || ""} ${title || ""} ${message || ""}`.toLowerCase();

        if (/red|#e44343|#b50707|fehler|error|gescheitert|verboten/.test(hint)) {
            return "error";
        }
        if (/orange|yellow|warn|achtung/.test(hint)) {
            return "warning";
        }
        if (/green|#1dd604|erfolg|success|erhalten|gekauft/.test(hint)) {
            return "success";
        }

        return "info";
    }

    function createIcon(kind, global, requestedIcon) {
        const holder = document.createElement("div");
        holder.className = "notification-icon";
        holder.setAttribute("aria-hidden", "true");

        const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
        svg.setAttribute("viewBox", "0 0 24 24");
        const iconName = cleanText(requestedIcon, "global", 20).toLowerCase();
        svg.innerHTML = global
            ? (icons[iconName] || icons.global)
            : (icons[kind] || icons.info);
        holder.appendChild(svg);
        return holder;
    }

    function removeNotification(notification, immediate) {
        if (!notification || !notification.isConnected) {
            return;
        }

        if (immediate) {
            notification.remove();
            return;
        }

        notification.classList.add("is-leaving");
        setTimeout(() => notification.remove(), 190);
    }

    function enforceLimit(region, limit) {
        const notifications = region.querySelectorAll(".ui-notification");
        while (notifications.length >= limit && region.firstElementChild) {
            removeNotification(region.firstElementChild, true);
            break;
        }
    }

    function pushNotification(payload) {
        const data = payload || {};
        const global = data.scope === "global";
        const region = document.getElementById(global ? "global-region" : "player-region");
        if (!region) {
            return;
        }

        const duration = safeDuration(data.duration);
        const title = cleanText(data.title, global ? "Ankündigung" : "Information", 80);
        const message = cleanText(data.message, "Keine weiteren Informationen.", 600);
        const kind = normalizeKind(data.color, title, message);

        enforceLimit(region, maximumVisible[global ? "global" : "player"]);

        const notification = document.createElement("article");
        notification.className = `ui-notification kind-${kind}${global ? " is-global" : ""}`;
        notification.setAttribute("role", global ? "alert" : "status");
        notification.appendChild(createIcon(kind, global, data.icon));

        const copy = document.createElement("div");
        copy.className = "notification-copy";

        const titleNode = document.createElement("strong");
        titleNode.className = "notification-title";
        titleNode.textContent = title;

        const messageNode = document.createElement("p");
        messageNode.className = "notification-message";
        messageNode.textContent = message;

        copy.appendChild(titleNode);
        copy.appendChild(messageNode);
        notification.appendChild(copy);

        const timer = document.createElement("div");
        timer.className = "notification-timer";
        timer.setAttribute("aria-hidden", "true");
        const timerBar = document.createElement("span");
        timerBar.style.animationDuration = `${duration}ms`;
        timer.appendChild(timerBar);
        notification.appendChild(timer);

        region.appendChild(notification);
        requestAnimationFrame(() => notification.classList.add("is-visible"));
        setTimeout(() => removeNotification(notification, false), duration);
    }

    window.KNotifications = {
        push: pushNotification
    };

    if (typeof mp !== "undefined" && typeof mp.trigger === "function") {
        mp.trigger("notifications:ready");
    }
}());
