(function () {
    var form = document.getElementById("searchForm");
    var input = document.getElementById("search");
    var message = document.getElementById("searchValidationMessage");

    if (form && input && message) {
        form.addEventListener("submit", function (e) {
            var value = (input.value || "").trim();
            message.textContent = "";

            if (value.length === 0) {
                e.preventDefault();
                message.textContent = "Search Input is required.";
                return;
            }

            input.value = value;
            if (!input.checkValidity()) {
                e.preventDefault();
                message.textContent = "Search must be 2-100 valid characters.";
            }
        });
    }

    document.addEventListener("click", function (e) {
        var link = e.target.closest(".js-confirm-deactivate");
        if (link && !confirm("Deactivate?")) {
            e.preventDefault();
        }
    });
})();
