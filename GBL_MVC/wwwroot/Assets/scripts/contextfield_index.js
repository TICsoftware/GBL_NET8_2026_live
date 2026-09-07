document.addEventListener("DOMContentLoaded", function () {
    document.addEventListener("click", function (e) {
        var link = e.target.closest(".js-confirm-deactivate");
        if (link && !confirm("Deactivate this field?")) {
            e.preventDefault();
        }
    });
});
