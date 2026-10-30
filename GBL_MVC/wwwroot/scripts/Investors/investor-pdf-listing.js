$(function () {
    const $grid = $("[data-investor-grid]");
    const $loadMoreWrap = $("[data-investor-load-more-wrap]");
    const $loadMoreButton = $("[data-investor-load-more]");
    const $empty = $("[data-investor-empty]");
    const $yearSelect = $("[data-investor-year]");

    if (!$grid.length) return;

    const pageSize = parseInt($grid.attr("data-page-size"), 10) || 12;
    const loadUrl = $grid.attr("data-load-url");
    const contentId = $grid.attr("data-content-id");

    let page = 1;
    let totalCount = parseInt($grid.attr("data-total-count"), 10) || 0;
    let year = $grid.attr("data-year") || ($yearSelect.length ? $yearSelect.val() : "") || "";
    let loading = false;
    let requestId = 0;

    function updateButton() {
        const loadedCount = $grid.children("li").length;

        $empty.prop("hidden", loadedCount > 0);

        if (loadedCount >= totalCount || totalCount <= pageSize) {
            $loadMoreWrap.prop("hidden", true).hide();
        } else {
            $loadMoreWrap.prop("hidden", false).show();
        }
    }

    function loadDocuments(reset) {
        if (loading || !loadUrl) return;

        loading = true;
        $loadMoreButton.prop("disabled", true);

        const requestedPage = reset ? 1 : page + 1;
        const currentRequestId = ++requestId;

        $.ajax({
            url: loadUrl,
            type: "GET",
            dataType: "json",
            data: {
                contentId: contentId,
                pageNumber: requestedPage,
                pageSize: pageSize,
                year: year || 0
            },
            success: function (response) {
                if (currentRequestId !== requestId) return;

                if (reset) {
                    $grid.html(response.html || "");
                } else if (response.html && response.html.trim() !== "") {
                    $grid.append(response.html);
                }

                page = requestedPage;
                totalCount = parseInt(response.totalCount, 10) || 0;
                $grid.attr("data-total-count", totalCount);
                $grid.attr("data-year", year);

                updateButton();
            },
            error: function (xhr) {
                console.error("Error loading investor documents:", xhr.responseText);
            },
            complete: function () {
                if (currentRequestId !== requestId) return;

                loading = false;
                $loadMoreButton.prop("disabled", false);
                updateButton();
            }
        });
    }

    $loadMoreButton.on("click", function (e) {
        e.preventDefault();
        loadDocuments(false);
    });

    $yearSelect.on("change", function () {
        year = $(this).val() || "";
        $grid.attr("data-year", year);
        loadDocuments(true);
    });

    initSelects(document.querySelector(".cg-year-filter") || document);
    updateButton();

    function initSelects(root) {
        if (!root) return;
        root.querySelectorAll(".cselect").forEach(function (wrap) {
            var native = wrap.querySelector(".cselect-native");
            if (!native || wrap.dataset.cselectReady === "1") return;
            wrap.dataset.cselectReady = "1";

            var listId = (native.id || "cselect") + "-menu";
            var trigger = document.createElement("button");
            trigger.type = "button";
            trigger.className = "cselect-trigger";
            trigger.setAttribute("aria-haspopup", "listbox");
            trigger.setAttribute("aria-expanded", "false");
            trigger.setAttribute("aria-controls", listId);

            var valueEl = document.createElement("span");
            valueEl.className = "cselect-value";
            valueEl.textContent = native.options[native.selectedIndex]
                ? native.options[native.selectedIndex].text
                : "";

            var chevron = document.createElement("span");
            chevron.className = "cselect-chevron";
            chevron.setAttribute("aria-hidden", "true");
            trigger.appendChild(valueEl);
            trigger.appendChild(chevron);

            var menu = document.createElement("ul");
            menu.className = "cselect-menu";
            menu.id = listId;
            menu.setAttribute("role", "listbox");
            menu.hidden = true;

            Array.prototype.forEach.call(native.options, function (option, optionIndex) {
                var item = document.createElement("li");
                var btn = document.createElement("button");
                btn.type = "button";
                btn.className = "cselect-option";
                btn.setAttribute("role", "option");
                btn.setAttribute("data-value", option.value);
                btn.textContent = option.text;
                if (optionIndex === native.selectedIndex) {
                    btn.setAttribute("aria-selected", "true");
                    btn.classList.add("is-active");
                }
                btn.addEventListener("click", function () {
                    native.value = option.value;
                    native.dispatchEvent(new Event("change", { bubbles: true }));
                    valueEl.textContent = option.text;
                    menu.querySelectorAll(".cselect-option").forEach(function (el) {
                        var selected = el === btn;
                        el.classList.toggle("is-active", selected);
                        el.setAttribute("aria-selected", selected ? "true" : "false");
                    });
                    closeSelect(wrap);
                    trigger.focus();
                });
                item.appendChild(btn);
                menu.appendChild(item);
            });

            native.setAttribute("tabindex", "-1");
            native.setAttribute("aria-hidden", "true");
            wrap.classList.add("is-enhanced");
            wrap.appendChild(trigger);
            wrap.appendChild(menu);

            trigger.addEventListener("click", function () {
                if (wrap.classList.contains("is-open")) closeSelect(wrap);
                else openSelect(wrap);
            });
        });

        var openMenu = null;

        function closeSelect(wrap) {
            if (!wrap) return;
            wrap.classList.remove("is-open");
            var menu = wrap.querySelector(".cselect-menu");
            var trigger = wrap.querySelector(".cselect-trigger");
            if (menu) menu.hidden = true;
            if (trigger) trigger.setAttribute("aria-expanded", "false");
            if (openMenu === wrap) openMenu = null;
        }

        function openSelect(wrap) {
            if (openMenu && openMenu !== wrap) closeSelect(openMenu);
            wrap.classList.add("is-open");
            var menu = wrap.querySelector(".cselect-menu");
            var trigger = wrap.querySelector(".cselect-trigger");
            if (menu) menu.hidden = false;
            if (trigger) trigger.setAttribute("aria-expanded", "true");
            openMenu = wrap;
        }

        document.addEventListener("click", function (event) {
            if (!openMenu) return;
            if (!openMenu.contains(event.target)) closeSelect(openMenu);
        });

        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape" && openMenu) {
                var trigger = openMenu.querySelector(".cselect-trigger");
                closeSelect(openMenu);
                if (trigger) trigger.focus();
            }
        });
    }
});
