$(document).ready(function () {

    // Form Submit
    $(document).on("submit", "#contact-form", function (e) {
        e.preventDefault();

        var $form = $(this);
        var $button = $form.find("button[type='submit']");
        var $response = $("#contact-form-response");

        if (!validateContactForm())
            return;

        $.ajax({
            url: $form.attr("action"),
            type: "POST",
            data: $form.serialize(),

            beforeSend: function () {
                $button.prop("disabled", true);
                $button.find(".site-btn__label").text("Sending...");
            },

            success: function (response) {

                if (response.status) {

                    $form[0].reset();

                    $form.find(".cf-field").removeClass("has-error");
                    $form.find(".cf-field__error").hide();
                    $form.find(".cf-captcha__error").hide();

                    $response
                        .removeClass("error")
                        .addClass("success")
                        .text(response.message);

                } else {

                    $response
                        .removeClass("success")
                        .addClass("error")
                        .text(response.message || "Please correct the errors below.");

                    // Server-side validation errors
                    if (response.errors) {
                        $.each(response.errors, function (key, messages) {

                            var $field = $form.find(
                                '[name="' + key + '"], [name="' + key + '[]"]'
                            );

                            if ($field.length) {
                                var message = Array.isArray(messages)
                                    ? messages[0]
                                    : messages;

                                if (key.toLowerCase() === "consent") {
                                    $form.find(".cf-captcha__error").text(message).show();
                                } else {
                                    showError($field, message);
                                }
                            }
                        });
                    }
                }

                $("html, body").animate({
                    scrollTop: $response.offset().top - 100
                }, 500);
            },

            error: function (xhr) {

                var message = "Something went wrong. Please try again.";

                if (xhr.responseJSON && xhr.responseJSON.message) {
                    message = xhr.responseJSON.message;
                }

                $response
                    .removeClass("success")
                    .addClass("error")
                    .text(message);

                $("html, body").animate({
                    scrollTop: $response.offset().top - 100
                }, 500);
            },

            complete: function () {
                $button.prop("disabled", false);
                $button.find(".site-btn__label").text("Submit");
            }
        });
    });


    // Form Validation
    function validateContactForm() {

        var isValid = true;
        var $form = $("#contact-form");

        // Clear previous errors
        $form.find(".cf-field").removeClass("has-error");
        $form.find(".cf-field__error").hide();
        $form.find(".cf-captcha__error").hide();

        // Full Name
        var name = ($("#cf-name").val() || "").trim();

        if (name === "") {
            showError("#cf-name", "Please enter your full name.");
            isValid = false;
        }

        // Company Name
        var companyName = ($("#cf-company").val() || "").trim();

        if (companyName === "") {
            showError("#cf-company", "Please enter company name.");
            isValid = false;
        }

        // Phone Number
        var phone = ($("#cf-phone").val() || "").trim();
        var phonePattern = /^[+]?[0-9\s\-()]{7,20}$/;

        if (phone === "") {
            showError("#cf-phone", "Please enter your phone number.");
            isValid = false;
        } else if (!phonePattern.test(phone)) {
            showError("#cf-phone", "Please enter a valid phone number.");
            isValid = false;
        }

        // Email
        var email = ($("#cf-email").val() || "").trim();
        var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (email === "") {
            showError("#cf-email", "Please enter your email address.");
            isValid = false;
        } else if (!emailPattern.test(email)) {
            showError("#cf-email", "Please enter a valid email address.");
            isValid = false;
        }

        // Site Location
        if (!$("#cf-location").val()) {
            showError("#cf-location", "Please select a site location.");
            isValid = false;
        }

        // State
        if (!$("#cf-state").val() || $("#cf-state").val() === "0") {
            showError("#cf-state", "Please select a state.");
            isValid = false;
        }

        // Google reCAPTCHA
        if (typeof grecaptcha === "undefined" ||
            !grecaptcha.getResponse()) {

            $("#captchaError")
                .text("Please confirm you are not a robot.")
                .show();

            isValid = false;
        } else {
            $("#captchaError").hide();
        }

        return isValid;
    }


    // Display Field Error
    function showError(field, message) {

        var $field = $(field);
        var $wrapper = $field.closest(".cf-field");

        $wrapper.addClass("has-error");

        $wrapper.find(".cf-field__error")
            .text(message)
            .show();
    }


    // Clear errors when fields are corrected
    $(document).on(
        "input change",
        "#contact-form .cf-field__control",
        function () {

            var $field = $(this);
            var value = ($field.val() || "").toString().trim();
            var $wrapper = $field.closest(".cf-field");

            if (value !== "" && this.checkValidity()) {
                $wrapper.removeClass("has-error");
                $wrapper.find(".cf-field__error").hide();
            }
        }
    );


    // Clear consent error
    $(document).on("change", '#contact-form [name="Consent"]', function () {

        if (this.checked) {
            $("#contact-form .cf-captcha__error").hide();
        }
    });

});