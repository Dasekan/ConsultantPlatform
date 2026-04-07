window.notifications = (function () {

    function showMessage(message, type = "info", timeout = 4000) {
        const container = document.getElementById("message-container");
        if (!container) return;

        const box = document.createElement("div");
        box.className = `message-box message-${type}`;
        box.textContent = message;

        container.appendChild(box);

        setTimeout(() => {
            box.remove();
        }, timeout);
    }

    function showSuccess(message) {
        showMessage(message, "success", 4000);
    }

    function showError(message) {
        showMessage(message, "error", 5000);
    }

    function showInfo(message) {
        showMessage(message, "info", 4000);
    }

    function showWarning(message) {
        showMessage(message, "warning", 4500);
    }

    function confirmAction(message) {
        return new Promise((resolve) => {
            const backdrop = document.createElement("div");
            backdrop.className = "custom-confirm-backdrop";

            const box = document.createElement("div");
            box.className = "custom-confirm-box";

            box.innerHTML = `
                <h4>Confirm action</h4>
                <p>${message}</p>
                <div class="custom-confirm-actions">
                    <button type="button" class="btn-cancel">Cancel</button>
                    <button type="button" class="btn-confirm">Delete</button>
                </div>
            `;

            backdrop.appendChild(box);
            document.body.appendChild(backdrop);

            box.querySelector(".btn-cancel").addEventListener("click", () => {
                backdrop.remove();
                resolve(false);
            });

            box.querySelector(".btn-confirm").addEventListener("click", () => {
                backdrop.remove();
                resolve(true);
            });
        });
    }

    function setFlashMessage(message, type = "success") {
        sessionStorage.setItem("flashMessage", message);
        sessionStorage.setItem("flashType", type);
    }

    function showFlashMessage() {
        const message = sessionStorage.getItem("flashMessage");
        const type = sessionStorage.getItem("flashType");

        if (message) {
            showMessage(message, type || "success");
            sessionStorage.removeItem("flashMessage");
            sessionStorage.removeItem("flashType");
        }
    }

    return {
        showSuccess,
        showError,
        showInfo,
        showWarning,
        confirmAction,
        setFlashMessage,
        showFlashMessage
    };
})();