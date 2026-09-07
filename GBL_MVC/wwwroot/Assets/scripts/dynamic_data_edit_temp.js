(function () {
    window.validateDynamicForm = function () {
        var isValid = true;
        $(".error-msg").text("");

        $(".dynamic-field").each(function () {
            var value = ($(this).val() || "").toString();
            var required = $(this).data("required") == "true";
            var validation = $(this).data("validation");
            var message = $(this).data("message");
            var name = $(this).data("name");
            var errorContainer = $(this).closest(".mb-3").find(".error-msg");

            if (required && value.trim() === "") {
                errorContainer.text(message || (name + " is required"));
                this.scrollIntoView({ behavior: "smooth", block: "center" });
                $(this).focus();
                isValid = false;
                return false;
            }

            if (validation && value) {
                try {
                    var regex = new RegExp(validation);
                    if (!regex.test(value)) {
                        errorContainer.text(message || (name + " is invalid"));
                        this.scrollIntoView({ behavior: "smooth", block: "center" });
                        $(this).focus();
                        isValid = false;
                        return false;
                    }
                } catch (e) {
                    console.error("Invalid regex for", name);
                }
            }
        });

        return isValid;
    };

    $(document)
        .off("submit.editData", "#EditContextDetailsForm_temp")
        .on("submit.editData", "#EditContextDetailsForm_temp", function (e) {
            e.preventDefault();

            if (window.syncCkEditorsToSource) {
                window.syncCkEditorsToSource(
                    document.getElementById("EditContextDetailsForm_temp")
                );
            }

            if (!validateDynamicForm()) {
                return;
            }

            var $form = $(this);
            $.ajax({
                url: $form.attr("action"),
                type: "POST",
                data: $form.serialize(),
                success: function (response) {
                    if (response.success) {
                        alert("Data updated successfully");
                        window.parent.Refresh_context_details(true);
                    } else {
                        alert(response.message);
                    }
                },
                error: function () {
                    alert("Error occurred");
                }
            });
        });

    setTimeout(function () {
        if (window.initCkEditors) {
            window.initCkEditors(document);
        }
    }, 200);
})();
