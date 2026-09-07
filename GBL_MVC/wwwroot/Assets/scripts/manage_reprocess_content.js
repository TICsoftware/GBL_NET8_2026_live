(function () {

    $(document).on("click", ".publish_reprocess_content_List", function (e) {
        e.preventDefault();
        e.stopPropagation();
        var encryptId = $(this).attr("data-encId");
        $("#PublishcontentreprocessId").val(encryptId);
        $("#btnsubmit").click();
    });

    $(document).on("click", ".reprocessed-delete-content", function (e) {
        e.preventDefault();
        e.stopPropagation();
        var encryptId = $(this).attr("data-encId");
        $("#PublishcontentreprocessId").val(encryptId);
        $("#btndelete").click();
    });

})();

