// wwwroot/js/auth.js
window.auth = (function () {
    const apiBase = "https://localhost:7143";

    async function me() {
        const res = await fetch(`${apiBase}/api/auth/me`, {
            method: "GET",
            credentials: "include"
        });

        if (!res.ok) return null;
        return await res.json(); // { email, roles }
    }



    function isAdmin(user) {
        return user && Array.isArray(user.roles) && user.roles.includes("Admin");
    }

    function renderUserInfo(user) {
        const el = document.getElementById("userInfo");
        if (!el) return;

        if (!user) {
            el.textContent = "";
            el.style.display = "none";
            return;
        }

        el.style.display = "inline-block";

        const rolesText = (user.roles && user.roles.length > 0)
            ? user.roles.join(", ")
            : "";

        el.textContent = `${user.email} (${rolesText})`;
    }

    // Kører på hver side og opdaterer navbar
    async function initNavbar() {
        const user = await me();
        window.currentUser = user;

        renderUserInfo(user);

        // Logout link
        const logoutLink = document.getElementById("logoutLink");
        if (logoutLink) {
            logoutLink.style.display = user ? "inline-block" : "none";
        }

        // Login link
        const loginLink = document.getElementById("loginLink");
        if (loginLink) {
            loginLink.style.display = user ? "none" : "inline-block";
        }

        // Dashboard link
        const dash = document.getElementById("navDashboard");
        if (dash) {
            dash.style.display = user ? "inline-block" : "none";
        }

        // Customers link
        const cust = document.getElementById("navCustomers");
        if (cust) {
            cust.style.display = user ? "inline-block" : "none";
        }
    }

    async function getErrorMessage(response) {
        try {
            const data = await response.json();

            if (data.message)
                return data.message;

            if (data.errors) {
                const allErrors = Object.values(data.errors).flat();
                if (allErrors.length > 0)
                    return allErrors.join(" ");
            }

            return "An unexpected error occurred.";
        }
        catch {
            return "An unexpected error occurred.";
        }
    }

    return {
        apiBase,
        me,
        isAdmin,
        initNavbar,
        getErrorMessage
    };

})();