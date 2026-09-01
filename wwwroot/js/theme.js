(function () {
    "use strict";

    var STORAGE_KEY = "tasktracker-theme";

    function getPreferredTheme() {
        var stored = localStorage.getItem(STORAGE_KEY);
        if (stored === "light" || stored === "dark") {
            return stored;
        }
        return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
    }

    function applyTheme(theme) {
        document.documentElement.setAttribute("data-bs-theme", theme);
        document.querySelectorAll(".theme-toggle").forEach(function (btn) {
            btn.setAttribute("aria-pressed", theme === "dark" ? "true" : "false");
            btn.setAttribute("aria-label", theme === "dark" ? "Ativar modo claro" : "Ativar modo escuro");
            btn.title = theme === "dark" ? "Ativar modo claro" : "Ativar modo escuro";
        });
    }

    applyTheme(getPreferredTheme());

    document.addEventListener("DOMContentLoaded", function () {
        applyTheme(getPreferredTheme());

        document.querySelectorAll(".theme-toggle").forEach(function (btn) {
            btn.addEventListener("click", function () {
                var current = document.documentElement.getAttribute("data-bs-theme") === "dark" ? "dark" : "light";
                var next = current === "dark" ? "light" : "dark";
                localStorage.setItem(STORAGE_KEY, next);
                applyTheme(next);
            });
        });
    });
})();
