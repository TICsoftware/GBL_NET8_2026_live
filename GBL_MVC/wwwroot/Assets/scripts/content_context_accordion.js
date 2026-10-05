(function () {
    window.toggleAccordion = function (event, id) {
        if (event && event.target && event.target.closest("a, button")) {
            return;
        }

        var el = document.getElementById(id);
        if (!el) return;

        var icon = document.getElementById("icon_" + id);
        if (el.classList.contains("show")) {
            el.classList.remove("show");
            if (icon) icon.style.transform = "rotate(0deg)";
        } else {
            el.classList.add("show");
            if (icon) icon.style.transform = "rotate(90deg)";
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
            .then(function (res) { return res.json(); })
            .then(function (data) {
                container.innerHTML = data.finalHtml || "<p>No layout defined</p>";
                new bootstrap.Modal(modalEl).show();
            })
            .catch(function (err) {
                console.error(err);
                alert("Failed to load layout");
            });
    };

    function renumberBlockSeq(list) {
        Array.prototype.forEach.call(list.querySelectorAll(":scope > .js-block-sort-item"), function (item, idx) {
            var seq = item.querySelector(".js-block-seq");
            if (seq) seq.textContent = String(idx + 1);
        });
    }

    function saveBlockSequence(list) {
        var mode = (list.getAttribute("data-mode") || "main").toLowerCase();
        var items = [];
        Array.prototype.forEach.call(list.querySelectorAll(":scope > .js-block-sort-item"), function (item, idx) {
            var gid = item.getAttribute("data-groupid");
            if (!gid) return;
            items.push({ context_group_id: gid, sequence: idx + 1 });
        });
        if (!items.length) return;

        fetch("/Content/UpdateBlockSequence", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ mode: mode, items: items })
        }).then(function (res) {
            if (!res.ok) throw new Error("Sequence update failed");
        }).catch(function (err) {
            console.error(err);
            alert("Failed to update sequence. Please try again.");
        });
    }

    function getDragAfterElement(list, y) {
        var els = Array.prototype.filter.call(
            list.querySelectorAll(":scope > .js-block-sort-item:not(.is-dragging)"),
            function (el) {
                return el.style.display !== "none" && el.offsetParent !== null;
            }
        );
        return els.reduce(function (closest, child) {
            var box = child.getBoundingClientRect();
            var offset = y - box.top - box.height / 2;
            if (offset < 0 && offset > closest.offset) {
                return { offset: offset, element: child };
            }
            return closest;
        }, { offset: Number.NEGATIVE_INFINITY, element: null }).element;
    }

    function bindPointerSortable(list) {
        if (list.getAttribute("data-sortable-ready") === "1") return;
        list.setAttribute("data-sortable-ready", "1");

        var dragging = null;

        list.addEventListener("mousedown", function (e) {
            if (e.button !== 0) return;
            var handle = e.target.closest(".drag-handle");
            if (!handle || !list.contains(handle)) return;
            if (e.target.closest("a, button, input, textarea, select")) return;

            e.preventDefault();
            e.stopPropagation();

            dragging = handle.closest(".js-block-sort-item");
            if (!dragging) return;

            dragging.classList.add("is-dragging");
            document.body.classList.add("is-block-sorting");

            function onMove(ev) {
                if (!dragging) return;
                ev.preventDefault();
                var after = getDragAfterElement(list, ev.clientY);
                if (after == null) {
                    list.appendChild(dragging);
                } else {
                    list.insertBefore(dragging, after);
                }
            }

            function onUp() {
                document.removeEventListener("mousemove", onMove);
                document.removeEventListener("mouseup", onUp);
                document.body.classList.remove("is-block-sorting");
                if (!dragging) return;
                dragging.classList.remove("is-dragging");
                renumberBlockSeq(list);
                saveBlockSequence(list);
                dragging = null;
            }

            document.addEventListener("mousemove", onMove);
            document.addEventListener("mouseup", onUp);
        });
    }

    window.initBlockSortable = function () {
        document.querySelectorAll(".js-block-sortable").forEach(bindPointerSortable);
    };

    window.refreshContextBlockList = function () {
        if (typeof window.hideSpotTemplatesModal === "function") {
            window.hideSpotTemplatesModal();
        }

        var boot = document.getElementById("content-edit-boot")
            || document.getElementById("edit-content-boot");
        var mode = boot ? (boot.getAttribute("data-mode") || "") : "";

        if (mode === "temp" && typeof window.Load_Edit_context_Temp_details === "function") {
            window.Load_Edit_context_Temp_details(true);
            return;
        }
        if ((mode === "published" || mode === "reprocess")
            && typeof window.Load_Edit_context_Published_details === "function") {
            window.Load_Edit_context_Published_details(true);
            return;
        }
        if (typeof window.Refresh_context_details === "function") {
            window.Refresh_context_details(true);
        }
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
            if (encId) window.showLayout(encId);
        });

    function bootSortable() {
        window.initBlockSortable();
    }

    $(function () {
        bootSortable();
        setTimeout(bootSortable, 400);
        setTimeout(bootSortable, 1200);
    });

    var target = document.getElementById("div_contentspotmapping");
    if (target && window.MutationObserver) {
        var timer = null;
        new MutationObserver(function () {
            clearTimeout(timer);
            timer = setTimeout(bootSortable, 120);
        }).observe(target, { childList: true, subtree: true });
    }
})();
