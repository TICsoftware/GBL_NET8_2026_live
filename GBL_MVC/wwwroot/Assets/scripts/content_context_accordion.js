(function () {
    window.toggleAccordion = function (event, id) {
        if (event && event.target && event.target.closest("a, button")) {
            return;
        }

        var el = document.getElementById(id);
        if (!el) {
            return;
        }

        var icon = document.getElementById("icon_" + id);
        if (el.classList.contains("show")) {
            el.classList.remove("show");
            if (icon) {
                icon.style.transform = "rotate(0deg)";
            }
        } else {
            el.classList.add("show");
            if (icon) {
                icon.style.transform = "rotate(90deg)";
            }
        }
    };

    window.showLayout = function (encId) {
        var modalEl = document.getElementById("layoutModal");
        var container = document.getElementById("layoutContainer");
        if (!modalEl || !container) {
            console.warn("Layout preview modal not found on this page.");
            return;
        }

        fetch("/ContextReference/Render?encId=" + encodeURIComponent(encId))
            .then(function (res) {
                return res.json();
            })
            .then(function (data) {
                container.innerHTML = data.finalHtml || "<p>No layout defined</p>";
                new bootstrap.Modal(modalEl).show();
            })
            .catch(function (err) {
                console.error(err);
                alert("Failed to load layout");
            });
    };

    $(document)
        .off("click.contentAccordion", ".js-toggle-accordion")
        .on("click.contentAccordion", ".js-toggle-accordion", function (e) {
            var collapseId = $(this).attr("data-collapse-id");
            if (collapseId) {
                window.toggleAccordion(e.originalEvent || e, collapseId);
            }
        });

    $(document)
        .off("click.contentShowLayout", ".js-show-layout")
        .on("click.contentShowLayout", ".js-show-layout", function () {
            var encId = $(this).attr("data-enc-id");
            if (encId) {
                window.showLayout(encId);
            }
        });
})();
