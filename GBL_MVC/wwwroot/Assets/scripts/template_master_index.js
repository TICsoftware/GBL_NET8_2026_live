(function () {
    if (!window.jQuery) return;

    setTimeout(function () {
        $(".alert").fadeOut("slow");
    }, 3000);

    $(document).on("click", "a.js-deactivate-confirm", function (e) {
        var message = this.getAttribute("data-confirm-message") || "Are you sure you want to deactivate this template?";
        if (!confirm(message)) {
            e.preventDefault();
        }
    });
})();
