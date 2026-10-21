$(function () {
    const $grid = $("[data-investor-grid]");
    const $loadMoreWrap = $("[data-investor-load-more-wrap]");
    const $loadMoreButton = $("[data-investor-load-more]");
    const $empty = $("[data-investor-empty]");

    if (!$grid.length) return;

    const pageSize = parseInt($grid.attr("data-page-size"), 10) || 12;
    const loadUrl = $grid.attr("data-load-url");
    const contentId = $grid.attr("data-content-id");

    let page = 1;
    let totalCount = parseInt($grid.attr("data-total-count"), 10) || 0;
    let loading = false;
    let requestId = 0;

    function updateButton() {
        const loadedCount = $grid.children("li").length;

        if (loadedCount === 0) {
            $empty.prop("hidden", false);
        } else {
            $empty.prop("hidden", true);
        }

        if (loadedCount >= totalCount || totalCount <= pageSize || loadedCount === 0) {
            $loadMoreWrap.prop("hidden", true).hide();
        } else {
            $loadMoreWrap.prop("hidden", false).show();
        }
    }

    function loadDocuments() {
        if (loading || !loadUrl) return;

        loading = true;
        $loadMoreButton.prop("disabled", true);

        const requestedPage = page + 1;
        const currentRequestId = ++requestId;

        $.ajax({
            url: loadUrl,
            type: "GET",
            dataType: "json",
            data: {
                contentId: contentId,
                pageNumber: requestedPage,
                pageSize: pageSize
            },
            success: function (response) {
                if (currentRequestId !== requestId) return;

                if (response.html && response.html.trim() !== "") {
                    $grid.append(response.html);
                }

                page = requestedPage;
                totalCount = parseInt(response.totalCount, 10) || 0;
                $grid.attr("data-total-count", totalCount);

                updateButton();

                if (typeof window.initCardRise === "function") {
                    window.initCardRise();
                }
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
        loadDocuments();
    });

    updateButton();
});
