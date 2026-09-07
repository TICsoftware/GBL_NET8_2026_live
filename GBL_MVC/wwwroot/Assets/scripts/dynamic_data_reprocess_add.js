(function () {
    function validateDynamicForm() {
        var isValid = true;
        $(".error-msg").text("");

        $(".dynamic-field").each(function () {
            var value = $(this).val();
            var required = $(this).data("required") === true || $(this).data("required") === "true";
            var validation = $(this).data("validation");
            var message = $(this).data("message");
            var name = $(this).data("name");

            if (required && (!value || value.trim() === "")) {
                var msg = message || (name + " is required");
                $(this).next(".error-msg").remove();
                $(this).after('<span class="error-msg text-danger">' + msg + "</span>");
                $(this).focus();
                isValid = false;
                return false;
            }

            if (validation && value) {
                try {
                    var regex = new RegExp(validation);
                    if (!regex.test(value)) {
                        var invalidMsg = message || (name + " is invalid");
                        $(this).next(".error-msg").remove();
                        $(this).after('<span class="error-msg text-danger">' + invalidMsg + "</span>");
                        $(this).focus();
                        isValid = false;
                        return false;
                    }
                } catch (e) {
                    console.error("Invalid regex for field:", name);
                }
            }
        });

        return isValid;
    }

    window.validateDynamicForm = validateDynamicForm;

    $("#addContextDetailsForm").off("submit.dynamicDataReprocessAdd").on("submit.dynamicDataReprocessAdd", function (e) {
        e.preventDefault();

        if (window.syncCkEditorsToSource) {
            window.syncCkEditorsToSource(document.getElementById("addContextDetailsForm"));
        }

        if (!validateDynamicForm()) {
            return;
        }

        $.ajax({
            url: $(this).attr("action"),
            type: "POST",
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    alert("Data added successfully");
                    window.parent.Load_Edit_context_Published_details(true);
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Error occurred");
            }
        });
    });
})();
