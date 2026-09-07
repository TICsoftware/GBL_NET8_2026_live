$(function () {

    $("#sortableComponents").sortable({
        update: function () {
            var order = [];

            $("#sortableComponents tr").each(function (index) {
                var id = $(this).data("id");

                order.push({
                    id: id,
                    sequence: index + 1
                });

                $(this).find(".seq").text(index + 1);
            });

            $.ajax({
                url: '/ContextTemplateReference/UpdateOrder',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(order)
            });
        }
    });

    $("#sortableComponents").sortable({
        handle: ".drag-handle"
    });

    $("#mappingForm").on("submit", function (e) {
        var isValid = true;
        var seq = ($("#txtSequence").val() || "").toString().trim();
        var label = ($("#txtLabel").val() || "").toString().trim();
        var reLabel = /^[A-Za-z0-9\s\-_#]{2,100}$/;

        $("#ctxError, #templateError, #seqError, #labelError").text("");

        if (!$("#ddContext").val()) {
            $("#ctxError").text("Please select component layout.");
            isValid = false;
        }
        if (!$("#ddTemplate").val()) {
            $("#templateError").text("Please select template.");
            isValid = false;
        }
        if (seq.length === 0 || !/^\d+$/.test(seq) || parseInt(seq) < 1 || parseInt(seq) > 9999) {
            $("#seqError").text("Sequence must be a number between 1 and 9999.");
            isValid = false;
        }
        if (!reLabel.test(label)) {
            $("#labelError").text("Label is required (2-100 chars; letters, numbers, space, -, _, #).");
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault();
            return false;
        }

        $("#txtLabel").val(label);
        return true;
    });
});

// CLICK EDIT
$(document).on("click", ".btn-edit", function () {
    var row = $(this).closest("tr");

    row.find(".label-text").hide();
    row.find(".label-input").removeClass("d-none").focus();

    row.find(".btn-edit").hide();
    row.find(".btn-save").removeClass("d-none");
});

// CLICK SAVE
$(document).on("click", ".btn-save", function () {
    var row = $(this).closest("tr");

    var id = row.data("id");
    var label = row.find(".label-input").val();

    $.ajax({
        url: '/ContextTemplateReference/UpdateLabel',
        type: 'POST',
        data: {
            id: id,
            label: label
        },
        success: function () {
            row.find(".label-text").text(label).show();
            row.find(".label-input").addClass("d-none");

            row.find(".btn-edit").show();
            row.find(".btn-save").addClass("d-none");
        }
    });
});

// CLICK DELETE
$(document).on("click", ".btn-delete-mapping", function () {
    alert('hi')
    var btn = $(this);
    var row = btn.closest("tr");
    var id = btn.data("id") || row.data("id");
    var contextMasterId = btn.data("context-id");

    var confirmed = confirm(
        "All the details tagged to this component mapping will be moved to history and then deleted from context_details. The mapping will be deactivated.\n\nAre you sure you want to continue?"
    );

    if (!confirmed) {
        return;
    }

    $.ajax({
        url: '/ContextTemplateReference/Delete',
        type: 'POST',
        data: {
            id: id,
            contextMasterId: contextMasterId
        },
        success: function (res) {
            if (res && res.success) {
                row.fadeOut(200, function () {
                    $(this).remove();
                });
            } else {
                alert((res && res.message) || "Delete failed. Please try again.");
            }
        },
        error: function () {
            alert("Something went wrong while deleting. Please try again.");
        }
    });
});

// ENTER KEY SAVE
$(document).on("keypress", ".label-input", function (e) {
    if (e.which == 13) {
        $(this).closest("tr").find(".btn-save").click();
    }
});

// VIEW LAYOUT
$(document).on("click", ".js-view-layout", function (e) {
    e.preventDefault();
    showLayoutTemplate($(this).attr("data-enc-id"));
});

function showLayoutTemplate(encId) {
    $.ajax({
        url: '/DynamicData/PreviewLayout',
        type: 'GET',
        data: { encId: encId },
        success: function (res) {
            $("#layoutContainerTemplate").html(res);

            var modal = new bootstrap.Modal(document.getElementById('layoutModalTemplate'));
            modal.show();
        }
    });
}
