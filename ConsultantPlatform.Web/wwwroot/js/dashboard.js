(async function () {
    const apiBaseUrl = window.auth.apiBase;

    if (window.auth && !window.currentUser) {
        await window.auth.initNavbar();
    }

    const isAdmin = window.auth.isAdmin(window.currentUser);
    const recentActivitySection = document.getElementById("recentActivitySection");
    const loading = document.getElementById("dashLoading");
    const content = document.getElementById("dashContent");

    try {
        const res = await fetch(`${apiBaseUrl}/api/dashboard/summary`, {
            credentials: "include"
        });

        if (!res.ok) {
            const errorMessage = await window.auth.getErrorMessage(res);
            throw new Error(errorMessage);
        }

        const data = await res.json();

        document.getElementById("cTotal").textContent = data.customers?.total ?? 0;
        document.getElementById("cLead").textContent = data.customers?.lead ?? 0;
        document.getElementById("cActive").textContent = data.customers?.active ?? 0;
        document.getElementById("cProjectClient").textContent = data.customers?.projectClient ?? 0;
        document.getElementById("cInactive").textContent = data.customers?.inactive ?? 0;

        document.getElementById("pTotal").textContent = data.projects?.total ?? 0;
        document.getElementById("pLead").textContent = data.projects?.lead ?? 0;
        document.getElementById("pActive").textContent = data.projects?.active ?? 0;
        document.getElementById("pCompleted").textContent = data.projects?.completed ?? 0;
        document.getElementById("pInvoiced").textContent = data.projects?.invoiced ?? 0;

        const tbody = document.querySelector("#deadlinesTable tbody");
        tbody.innerHTML = "";

        const list = data.upcomingDeadlines ?? [];

        list.forEach(p => {
            const tr = document.createElement("tr");

            const endDate = p.endDate
                ? new Date(p.endDate).toLocaleDateString()
                : "";

            const statusText = (p.status ?? "").toString();
            const statusClass = getStatusClass(statusText);

            tr.innerHTML = `
                <td>${escapeHtml(p.projectName ?? "")}</td>
                <td>${escapeHtml(p.customerName ?? "")}</td>
                <td><span class="pill ${statusClass}">${escapeHtml(statusText)}</span></td>
                <td>${escapeHtml(endDate)}</td>
            `;

            tbody.appendChild(tr);
        });

        if (isAdmin) {
            renderRecentActivity(data.recentActivity ?? []);
        }

        if (recentActivitySection) {
            recentActivitySection.style.display = isAdmin ? "" : "none";
        }

        loading.style.display = "none";
        content.style.display = "grid";
    } catch (err) {
        if (window.notifications) {
            window.notifications.showError(err.message || "Could not load dashboard.");
        }

        loading.innerHTML =
            `<b>Could not load dashboard.</b><div class="muted">${escapeHtml(err.message || "An unexpected error occurred.")}</div>`;
    }

    function formatDateTime(value) {
        if (!value) return "";

        let utcValue = value;

        if (typeof utcValue === "string" &&
            !utcValue.endsWith("Z") &&
            !utcValue.includes("+")) {
            utcValue += "Z";
        }

        const now = new Date();
        const time = new Date(utcValue);

        const diffMs = now - time;

        const minutes = Math.floor(diffMs / 60000);
        const hours = Math.floor(diffMs / 3600000);
        const days = Math.floor(diffMs / 86400000);

        if (minutes < 1) return "Just now";
        if (minutes === 1) return "1 minute ago";
        if (minutes < 60) return `${minutes} minutes ago`;

        if (hours === 1) return "1 hour ago";
        if (hours < 24) return `${hours} hours ago`;

        if (days === 1) return "1 day ago";
        if (days < 7) return `${days} days ago`;

        return time.toLocaleDateString("en-GB");
    }

    function renderRecentActivity(items) {
        const tbody = document.querySelector("#recentActivityTable tbody");
        if (!tbody) return;

        tbody.innerHTML = "";

        if (!Array.isArray(items) || items.length === 0) {
            tbody.innerHTML = `
            <tr>
                <td colspan="3" class="text-muted">No recent activity found.</td>
            </tr>
        `;
            return;
        }

        items.forEach(item => {

            const tr = document.createElement("tr");

            const rawSummary = item.summary ?? "";
            let summary = escapeHtml(rawSummary);

            const isDeleted = rawSummary.toLowerCase().includes("was deleted");

            let entityLink = null;
            let customerLink = null;

            // Entity link
            if (!isDeleted) {
                if (item.entityType === "Customer") {
                    entityLink = `/CustomerDetails?id=${item.entityId}`;
                }

                if (item.entityType === "Project" && item.relatedCustomerId) {
                    entityLink = `/CustomerProjects?id=${item.relatedCustomerId}`;
                }

                if (item.entityType === "Document" && item.relatedCustomerId) {
                    entityLink = `/CustomerDocuments?id=${item.relatedCustomerId}`;
                }
            }

            // Customer link for project/document activity
            if ((item.entityType === "Project" || item.entityType === "Document") && item.relatedCustomerId) {
                customerLink = `/CustomerDetails?id=${item.relatedCustomerId}`;
            }

            // Find all quoted names in the summary
            const matches = [...rawSummary.matchAll(/'(.*?)'/g)];

            if (matches.length > 0) {
                // First quoted value = entity name
                const entityName = matches[0][1];
                const escapedEntityQuotedName = escapeHtml(`'${entityName}'`);

                if (entityLink) {
                    const linkedEntityName =
                        `'` +
                        `<a href="${entityLink}" class="fw-semibold text-decoration-none">${escapeHtml(entityName)}</a>` +
                        `'`;

                    summary = summary.replace(escapedEntityQuotedName, linkedEntityName);
                }
            }

            if (matches.length > 1) {
                // Second quoted value = customer name
                const customerName = matches[1][1];
                const escapedCustomerQuotedName = escapeHtml(`'${customerName}'`);

                if (customerLink) {
                    const linkedCustomerName =
                        `'` +
                        `<a href="${customerLink}" class="fw-semibold text-decoration-none">${escapeHtml(customerName)}</a>` +
                        `'`;

                    summary = summary.replace(escapedCustomerQuotedName, linkedCustomerName);
                }
            }

            tr.innerHTML = `
        <td data-time="${item.createdUtc}">
            ${escapeHtml(formatDateTime(item.createdUtc))}
        </td>
        <td>${summary}</td>
        <td>${escapeHtml(item.userEmail ?? "")}</td>
    `;

            tbody.appendChild(tr);
        });
    }

    function getStatusClass(status) {
        const s = String(status).toLowerCase().trim();

        if (s === "lead") return "pill-lead";
        if (s === "active") return "pill-active";
        if (s === "completed") return "pill-completed";
        if (s === "invoiced") return "pill-invoiced";

        return "pill-neutral";
    }

    function escapeHtml(str) {
        return String(str)
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#039;");
    }

    function refreshRecentActivityTimes() {
        const cells = document.querySelectorAll("#recentActivityTable td[data-time]");

        cells.forEach(cell => {
            const value = cell.getAttribute("data-time");
            cell.textContent = formatDateTime(value);
        });
    }

    setInterval(() => {
        refreshRecentActivityTimes();
    }, 60000);
})();