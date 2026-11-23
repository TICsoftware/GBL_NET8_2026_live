$(function () {
    const $grid = $("#DivContent");
    const $loadMoreWrap = $("[data-news-load-more-wrap]");
    const $loadMoreButton = $("[data-news-load-more]");

    if (!$grid.length) return;

    const pageSize = parseInt($grid.attr("data-page-size"), 10) || 12;
    const loadUrl = $grid.attr("data-load-url");
    const contentId = $grid.attr("data-content-id");

    let page = 1;
    let totalCount = parseInt($grid.attr("data-total-count"), 10) || 0;
    let loading = false;
    let requestId = 0;

    function getFilters() {
        return {
            topic: $("#filter-topic").val() || "",
            month: $("#filter-month").val() || "",
            year: $("#filter-year").val() || ""
        };
    }

    // function updateButton() {
    //     const loadedCount = $grid.children("article").length;

    //     $loadMoreWrap.toggle(
    //         loadedCount < totalCount && loadedCount > 0
    //     );
    // }

    function updateButton() {
        const loadedCount = $grid.find("article").length;

        if (loadedCount >= totalCount || totalCount <= pageSize) {
            $loadMoreWrap.prop("hidden", true).hide();
        } else {
            $loadMoreWrap.prop("hidden", false).show();
        }
    }

    function refreshNewsCardEffects(root) {
        if (typeof window.refreshNewsCardFill === "function") {
            window.refreshNewsCardFill(root || $grid[0]);
            return;
        }
        if (typeof window.bindNewsCardFill === "function") {
            window.bindNewsCardFill(root || $grid[0]);
        }
    }

    function loadArticles(reset) {
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
                ...getFilters()
            },
            success: function (response) {
                if (currentRequestId !== requestId) return;

                const previous = reset ? [] : $grid.children().toArray();

                if (reset) {
                    $grid.html(response.html);
                } else {
                    $grid.append(response.html);
                }

                const fresh = reset
                    ? $grid.find(".industry-product-card").toArray()
                    : $grid.children().toArray().filter(function (el) {
                        return previous.indexOf(el) === -1;
                    });

                page = requestedPage;
                totalCount = parseInt(response.totalCount, 10) || 0;

                $grid.attr("data-total-count", totalCount);

                updateButton();

                if (reset && typeof window.refreshNewsCardMotion === "function") {
                    window.refreshNewsCardMotion($grid[0]);
                } else if (typeof window.bindNewsCardScroll === "function") {
                    window.bindNewsCardScroll(fresh);
                } else if (typeof window.animateNewsCards === "function") {
                    window.animateNewsCards(fresh);
                } else {
                    refreshNewsCardEffects($grid[0]);
                }
            },
            error: function (xhr) {
                console.error(
                    "Error loading media coverage:",
                    xhr.responseText
                );
            },
            complete: function () {
                if (currentRequestId !== requestId) return;

                loading = false;
                $loadMoreButton.prop("disabled", false);
                updateButton();
            }
        });
    }

    $("#filter-topic, #filter-month, #filter-year").on("change", function () {
        loadArticles(true);
    });

    $loadMoreButton.on("click", function (e) {
        e.preventDefault();
        loadArticles(false);
    });

    $("[data-clear-news-filter]").on("click", function (e) {
        e.preventDefault();

        $("#filter-topic, #filter-month, #filter-year").val("");

        loadArticles(true);
    });

    updateButton();
});