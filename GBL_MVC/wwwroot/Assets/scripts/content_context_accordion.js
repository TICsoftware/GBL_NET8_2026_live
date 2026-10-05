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

    function ensureSortableLoaded(callback) {
        if (typeof Sortable !== "undefined") {
            callback();
            return;
        }
        if (window.__sortableLoading) {
            window.__sortableLoading.push(callback);
            return;
        }
        window.__sortableLoading = [callback];
        var s = document.createElement("script");
        s.src = "https://cdn.jsdelivr.net/npm/sortablejs@1.15.0/Sortable.min.js";
        s.onload = function () {
            var queue = window.__sortableLoading || [];
            window.__sortableLoading = null;
            queue.forEach(function (fn) {
                try { fn(); } catch (e) { console.error(e); }
            });
        };
        s.onerror = function () {
            window.__sortableLoading = null;
            console.error("Failed to load SortableJS");
        };
        document.head.appendChild(s);
    }

    function renumberBlockSeq($list) {
        $list.children(".js-block-sort-item").each(function (idx) {
            $(this).find(".js-block-seq").first().text(String(idx + 1));
        });
    }

    function saveBlockSequence($list) {
        var mode = ($list.attr("data-mode") || "main").toLowerCase();
        var items = [];
        $list.children(".js-block-sort-item").each(function (idx) {
            var gid = $(this).attr("data-groupid");
            if (!gid) return;
            items.push({ context_group_id: gid, sequence: idx + 1 });
        });
        if (!items.length) return;

        fetch("/Content/UpdateBlockSequence", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ mode: mode, items: items })
        }).catch(function (err) {
            console.error(err);
            alert("Failed to update sequence. Please try again.");
        });
    }

    window.initBlockSortable = function () {
        ensureSortableLoaded(function () {
            document.querySelectorAll(".js-block-sortable").forEach(function (el) {
                if (el.getAttribute("data-sortable-ready") === "1") return;
                el.setAttribute("data-sortable-ready", "1");
                new Sortable(el, {
                    animation: 150,
                    handle: ".drag-handle",
                    draggable: ".js-block-sort-item",
                    onEnd: function () {
                        var $list = $(el);
                        renumberBlockSeq($list);
                        saveBlockSequence($list);
                    }
                });
            });
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

    $(function () {
        window.initBlockSortable();
    });

    var target = document.getElementById("div_contentspotmapping");
    if (target && window.MutationObserver) {
        var timer = null;
        new MutationObserver(function () {
            clearTimeout(timer);
            timer = setTimeout(function () {
                window.initBlockSortable();
            }, 50);
        }).observe(target, { childList: true, subtree: false });
    }
})();
