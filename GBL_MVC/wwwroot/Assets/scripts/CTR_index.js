
$("#btnAddComponent").click(function () {

    var contextId = $("#ddContext").val();
    var templateId = $("#ddTemplate").val();
    var sequence = $("#txtSequence").val();
alert(contextId);
    if (!contextId || !templateId || !sequence) {
        alert("Please fill all fields");
        return;
    }

    $.ajax({
        url: '@Url.Action("AddComponent", "ContextTemplateReference")',
        type: 'POST',
        data: {
            Context_Master_ID: contextId,
            Template_Master_ID: templateId,
            Sequence: sequence
        },
        success: function (res) {

            if (res.success) {

                // $("#msgArea").html(
                //     '<div class="alert alert-success">' +
                //     res.message + '</div>');
                alert(res.message);

                // reload list
                location.reload();
            }
            else {
                alert(res.message);
            }
        },
        error: function () {
            alert("Server error");
        }
    });

});


function openDataEditor(url) {


    alert('hi');
    // show modal
    var modal = new bootstrap.Modal(document.getElementById('dataModal'));
    modal.show();

    // load page into modal body
    $("#dataModalBody").html("Loading...");

    $("#dataModalBody").load(url);
}

