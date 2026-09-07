
$(function () {

    var index = @Model.CounterCards.Count;

    function updateBlockCount() {
        var total = $("#counterContainer .accordion-item").length;
        $("#blockCount").text(total);
    }

    $("#addCounter").click(function () {

        $.get('@Url.Action("LoadCounterCard", "CounterCardView")',
            { index: index },
            function (data) {

                $("#counterContainer").append(data);
                index++;
                updateBlockCount();   // 🔥 Update count
            });
    });

    $(document).on("click", ".remove-card", function () {
        $(this).closest(".accordion-item").remove();
        updateBlockCount();       // 🔥 Update count
    });

    $("#counterContainer").sortable({
        handle: ".drag-handle"
    });

    // Initial count set
    updateBlockCount();

});
