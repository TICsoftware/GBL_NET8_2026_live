document.addEventListener("DOMContentLoaded", function () {
    // Preserve the original `onclick="return prepareJSON()"` behaviour:
    // cancel submit only when prepareJSON() explicitly returns false.
    document.querySelectorAll(".js-prepare-json").forEach(function (btn) {
        btn.addEventListener("click", function (e) {
            if (typeof prepareJSON === "function" && prepareJSON() === false) {
                e.preventDefault();
            }
        });
    });

    document.querySelectorAll(".js-add-option").forEach(function (btn) {
        btn.addEventListener("click", function () {
            if (typeof addOptionRow === "function") {
                addOptionRow();
            }
        });
    });
});
