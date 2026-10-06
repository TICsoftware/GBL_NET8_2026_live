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
        $loadMoreWrap.prop("hidden", totalCount <= pageSize);
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

                if (reset) {
                    $grid.html(response.html);
                } else {
                    $grid.append(response.html);
                }

                page = requestedPage;
                totalCount = parseInt(response.totalCount, 10) || 0;

                $grid.attr("data-total-count", totalCount);

                updateButton();
            },
            error: function (xhr) {
                console.error(
                    "Error loading press releases:",
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