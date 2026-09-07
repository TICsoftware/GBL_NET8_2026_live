(function () {
    var el = document.getElementById("content-alert");
    if (!el) {
        return;
    }

    var message = el.getAttribute("data-message");
    if (message) {
        alert(message);
    }
})();
