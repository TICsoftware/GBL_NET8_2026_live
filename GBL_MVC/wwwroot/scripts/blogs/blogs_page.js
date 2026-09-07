$(document).on("click", "#btnLoadMore", function (e) {
    e.preventDefault();

    var page = parseInt($("#hdnPage").val());
    var contentId = $("#hdnContentId").val();
    var totalCount = parseInt($("#hdnTotalCount").val());

    $.ajax({
        url: '/Blogs/BlogLoadMore',
        type: 'GET',
        data: {
            contentId: contentId,
            page: page,
            pageSize: 1
        },
        success: function (html) {

            if (!html || html.trim() === "") {
                $("#DivLoad").hide();
                return;
            }

            $("#articleContainer").append(html);

            var loadedCount = $("#articleContainer .blog-item").length;

            $("#hdnPage").val(page + 1);

            if (loadedCount >= totalCount) {
                $("#DivLoad").hide();
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr.status);
            console.log(xhr.responseText);
            console.log(error);
            alert("Error: " + xhr.status);
        }
    });

});