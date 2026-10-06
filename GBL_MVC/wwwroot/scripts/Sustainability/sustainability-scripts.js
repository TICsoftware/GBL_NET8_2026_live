$(document).ready(function () {

    let pageNumber = 1;
    const pageSize = 10;
    let isLoading = false;

    const $section = $(".sustainabilityReportsList");
    const contentId = $section.data("content-id");
    const $list = $("#sustainabilityReportsList");
    const $button = $("#btnLoadMore");

    $button.on("click", function () {

        if (isLoading) return;

        isLoading = true;

        const $label = $button.find(".site-btn__label");
        $label.text("Loading...");
        $button.prop("disabled", true);

        $.ajax({
            url: "/Sustainability/LoadSustainabilityReports",
            type: "GET",
            data: {
                contentId: contentId,
                pageNumber: pageNumber + 1,
                pageSize: pageSize
            },
            success: function (response) {

                if (response.html && response.html.trim() !== "") {
                    $list.append(response.html);
                    pageNumber = response.pageNumber;
                }

                if (pageNumber * pageSize >= response.totalCount ||
                    !response.html ||
                    response.html.trim() === "") {
                    $("#loadMoreWrap").hide();
                }
            },
            error: function () {
                alert("Unable to load reports. Please try again.");
            },
            complete: function () {
                isLoading = false;
                $label.text("Load More");
                $button.prop("disabled", false);
            }
        });
    });
});