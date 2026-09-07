(function () {
    var el = document.getElementById("edit-content-alert");
    if (!el) {
        return;
    }

    var message = el.getAttribute("data-message");
    if (message) {
        alert(message);
    }
})();
