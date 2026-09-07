(function () {
    function validateDynamicFormTemp() {
        var isValid = true;
        $(".error-msg").remove();

        $(".dynamic-field").each(function () {
            var value = ($(this).val() || "").toString();
            var required = $(this).data("required") === true || $(this).data("required") === "true";
            var validation = $(this).data("validation");
            var message = $(this).data("message");
            var name = $(this).data("name");

            if (required && (!value || value.trim() === "")) {
                var msg = message || (name + " is required");
                $(this).after('<span class="error-msg text-danger">' + msg + "</span>");
                this.scrollIntoView({ behavior: "smooth", block: "center" });
                $(this).focus();
                isValid = false;
                return false;
            }

            if (validation && value) {
                try {
                    var regex = new RegExp(validation);
                    if (!regex.test(value)) {
                        var invalidMsg = message || (name + " is invalid");
                        $(this).after('<span class="error-msg text-danger">' + invalidMsg + "</span>");
                        this.scrollIntoView({ behavior: "smooth", block: "center" });
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

    window.validateDynamicFormTemp = validateDynamicFormTemp;

    $(document)
        .off("submit.addDataTemp", "#addContextDetailsFormTemp")
        .on("submit.addDataTemp", "#addContextDetailsFormTemp", function (e) {
            e.preventDefault();

            if (window.syncCkEditorsToSource) {
                window.syncCkEditorsToSource(
                    document.getElementById("addContextDetailsFormTemp")
                );
            }

            if (!validateDynamicFormTemp()) {
                return;
            }

            var $form = $(this);
            $.ajax({
                url: $form.attr("action"),
                type: "POST",
                data: $form.serialize(),
                success: function (response) {
                    if (response.success) {
                        alert("Data added successfully");
                        if (typeof window.hideSpotTemplatesModal === "function") {
                            var modalEl = document.getElementById("modal_spottemplates");
                            var modal = bootstrap.Modal.getInstance(modalEl);
                            if (modal) {
                                modal.hide();
                            }
                        }
                        if (typeof window.Refresh_context_details === "function") {
                            window.Refresh_context_details(false);
                        } else if (
                            window.parent &&
                            typeof window.parent.hideSpotTemplatesModal === "function"
                        ) {
                            window.parent.hideSpotTemplatesModal();
                            window.parent.Refresh_context_details(false);
                        }
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
