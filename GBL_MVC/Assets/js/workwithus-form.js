$(document).ready(function () {

    // Resume filename display
    $(document).on("change", "#Resume", function () {
        var file = this.files && this.files[0];
        var $label = $(this).closest(".career-file").find("[data-file-name]");
        if (file) {
            $label.text(file.name);
        } else {
            $label.text("Upload resume");
        }
        $('[data-valmsg-for="Resume"]').text("");
    });

    $(document).on("submit", "#workWithUsForm", function (e) {
        e.preventDefault();

        if (!validateWorkWithUsForm())
            return;

        var $form = $(this);
        var formData = new FormData(this);

        // Ensure checkbox posts correctly when checked
        if ($("#NotRobot").prop("checked")) {
            formData.set("NotRobot", "true");
        } else {
            formData.set("NotRobot", "false");
        }

        $.ajax({
            url: $form.attr("action"),
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function () {
                $("#workWithUsMessage").empty();
                $("#workWithUsSubmitBtn")
                    .prop("disabled", true)
                    .find(".site-btn__label")
                    .text("Submitting...");
            },
            success: function (response) {
                restoreSubmitButton();

                if (response.status) {
                    $form[0].reset();
                    $('[data-file-name]').text("Upload resume");
                    $('[data-valmsg-for]').text("");

                    $("#workWithUsMessage").html(
                        '<div class="cu-form-alert cu-form-alert--success">' + response.message + "</div>"
                    );

                    $("html, body").animate({
                        scrollTop: $("#workWithUsMessage").offset().top - 100
                    }, 500);
                } else {
                    if (response.errors) {
                        $.each(response.errors, function (key, msg) {
                            $('[data-valmsg-for="' + key + '"]').text(msg || "");
                        });
                    }

                    $("#workWithUsMessage").html(
                        '<div class="cu-form-alert cu-form-alert--error">' + (response.message || "Something went wrong.") + "</div>"
                    );
                }
            },
            error: function () {
                restoreSubmitButton();
                $("#workWithUsMessage").html(
                    '<div class="cu-form-alert cu-form-alert--error">Something went wrong. Please try again.</div>'
                );
            }
        });
    });

    function restoreSubmitButton() {
        $("#workWithUsSubmitBtn")
            .prop("disabled", false)
            .find(".site-btn__label")
            .text("Submit");
    }

    function setError(name, message) {
        $('[data-valmsg-for="' + name + '"]').text(message || "");
        if (message) {
            isValid = false;
        }
    }

    var isValid = true;

    function validateWorkWithUsForm() {
        $('[data-valmsg-for]').text("");
        isValid = true;

        var fullName = ($("#FullName").val() || "").trim();
        if (fullName === "") {
            setError("FullName", "Please enter your full name.");
        } else if (!/^[A-Za-z][A-Za-z\s.]*$/.test(fullName)) {
            setError("FullName", "Only alphabets, spaces and '.' are allowed.");
        } else if (fullName.length > 100) {
            setError("FullName", "Name cannot exceed 100 characters.");
        }

        var email = ($("#Email").val() || "").trim();
        if (email === "") {
            setError("Email", "Please enter your email.");
        } else if (!/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(email)) {
            setError("Email", "Enter a valid email address.");
        } else if (email.length > 500) {
            setError("Email", "Email cannot exceed 500 characters.");
        }

        var expertise = ($("#Expertise").val() || "").trim();
        if (expertise.length > 1000) {
            setError("Expertise", "Expertise cannot exceed 1000 characters.");
        } else if (expertise !== "" && /[`^~<>{}]/.test(expertise)) {
            setError("Expertise", "Please enter valid characters.");
        }

        var address = ($("#Address").val() || "").trim();
        if (address.length > 2000) {
            setError("Address", "Address cannot exceed 2000 characters.");
        } else if (address !== "" && /[`^~<>{}]/.test(address)) {
            setError("Address", "Please enter valid characters.");
        }

        var designation = ($("#Designation").val() || "").trim();
        if (designation.length > 1000) {
            setError("Designation", "Designation cannot exceed 1000 characters.");
        } else if (designation !== "" && /[`^~<>{}]/.test(designation)) {
            setError("Designation", "Please enter valid characters.");
        }

        var message = ($("#Message").val() || "").trim();
        if (message !== "" && /[`^~<>{}]/.test(message)) {
            setError("Message", "Please enter valid characters.");
        }

        var resumeInput = document.getElementById("Resume");
        if (resumeInput && resumeInput.files && resumeInput.files.length > 0) {
            var file = resumeInput.files[0];
            var name = (file.name || "").toLowerCase();
            var allowed = [".pdf", ".doc", ".docx"];
            var ext = name.substring(name.lastIndexOf("."));
            var maxBytes = 5 * 1024 * 1024;

            if (allowed.indexOf(ext) === -1) {
                setError("Resume", "Only PDF and Word files are accepted.");
            } else if (file.size > maxBytes) {
                setError("Resume", "Maximum allowed file size is 5 MB.");
            }
        }

        if (!$("#NotRobot").prop("checked")) {
            setError("NotRobot", "Please confirm you are not a robot.");
        }

        return isValid;
    }
});
