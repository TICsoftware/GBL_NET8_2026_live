(function () {
    var addModalEl = document.getElementById("modalAddTag");
    var addForm = document.getElementById("frmAddTag");
    if (addModalEl && addForm) {
        addModalEl.addEventListener("show.bs.modal", function () {
            addForm.reset();
        });
    }

    var editUrl = $("#frmEditTag").data("edit-url");
    var editModalEl = document.getElementById("modalEditTag");

    $(document).on("click", ".js-edit-tag", function (e) {
        e.preventDefault();
        var id = $(this).attr("data-id");
        if (!id || !editUrl) return;

        $.ajax({
            url: editUrl,
            type: "GET",
            data: { Id: id },
            dataType: "json",
            success: function (data) {
                if (!data || !data.success) {
                    alert((data && data.message) || "Unable to load tag.");
                    return;
                }
                $("#Edit_ID").val(data.id);
                $("#Edit_Status").val(data.status);
                $("#Edit_Tag_Name").val(data.tagName || "");
                $("#ddEditTagLanguage").val(data.languageMasterId || "");
                bootstrap.Modal.getOrCreateInstance(editModalEl).show();
            },
            error: function () {
                alert("Unable to load tag.");
            }
        });
    });

    $("#frmEditTag").on("submit", function (e) {
        e.preventDefault();
        if (!editUrl) return;

        $.ajax({
            url: editUrl,
            type: "POST",
            data: $(this).serialize(),
            dataType: "json",
            success: function (data) {
                if (data && data.success) {
                    if (editModalEl) {
                        bootstrap.Modal.getOrCreateInstance(editModalEl).hide();
                    }
                    alert(data.message || "Tag updated successfully.");
                    window.location.reload();
                } else {
                    alert((data && data.message) || "Unable to update tag.");
                }
            },
            error: function () {
                alert("Unable to update tag.");
            }
        });
    });

    var statusUrl = $("#frmEditTag").data("status-url");
    $(document).on("click", ".js-update-status", function (e) {
        e.preventDefault();
        var id = $(this).attr("data-id");
        var status = $(this).attr("data-status");
        var message = $(this).attr("data-confirm-message") || "Are you sure you want to update this tag status?";
        if (!id || !statusUrl) return;
        if (!confirm(message)) return;

        $.ajax({
            url: statusUrl,
            type: "POST",
            data: {
                Id: id,
                status: status,
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").first().val()
            },
            dataType: "json",
            success: function (data) {
                if (data && data.success) {
                    alert(data.message || "Tag status updated successfully.");
                    window.location.reload();
                } else {
                    alert((data && data.message) || "Unable to update tag status.");
                }
            },
            error: function () {
                alert("Unable to update tag status.");
            }
        });
    });
})();