(function () {
    function boot() {
        var el = document.getElementById("content-edit-boot");
        if (!el) {
            return;
        }

        var mode = el.getAttribute("data-mode");
        var templateId = parseInt(el.getAttribute("data-template-id") || "0", 10);
        var languageId = parseInt(el.getAttribute("data-language-id") || "0", 10);
        var encryptId = el.getAttribute("data-encrypt-id") || "";

        if (mode === "temp" && typeof window.Load_Edit_context_Temp_details === "function") {
            window.Load_Edit_context_Temp_details(false, templateId, languageId, encryptId);
            return;
        }

        if (mode === "published" && typeof window.Load_Edit_context_Published_details === "function") {
            window.Load_Edit_context_Published_details(false, templateId, languageId, encryptId);
        }
    }

    if (document.readyState === "complete") {
        boot();
    } else {
        window.addEventListener("load", boot);
    }
})();
