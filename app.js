'use strict';
var clipboard;
var initialized = false;
async function Init() {
    if (!initialized) {
        const statusSpan = document.getElementById('status');
        statusSpan.textContent = "Initializing";
        clipboard = new ClipboardJS('input.cb-copy');
        //const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        //[...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
        initialized = true;
    }
}
document.addEventListener("DOMContentLoaded", Init);
window.addEventListener("load", Init);
//# sourceMappingURL=app.js.map