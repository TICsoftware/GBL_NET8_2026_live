$(document).ready(function () {

    $(document).on("submit", "#enquiryForm", function (e) {
        e.preventDefault();

        if (!validateEnquiryForm())
            return;

        var $form = $(this);
        var formData = $form.serializeArray();

        // Ensure checkboxes post correctly
        formData = formData.filter(function (item) {
            return item.name !== "AcceptTerms" && item.name !== "NotRobot";
        });
        formData.push({ name: "AcceptTerms", value: $("#AcceptTerms").prop("checked") ? "true" : "false" });
        formData.push({ name: "NotRobot", value: $("#NotRobot").prop("checked") ? "true" : "false" });

        var token = $form.find('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: "/enquiry/submit",
            type: "POST",
            data: $.param(formData),
            headers: token ? { "RequestVerificationToken": token } : {},
            beforeSend: function () {
                $("#enquiryMessage").empty();
                $("#enquirySubmitBtn")
                    .prop("disabled", true)
                    .find(".site-btn__label")
                    .text("Submitting...");
            },
            success: function (response) {
                restoreSubmitButton();

                if (response && response.status) {
                    var productName = $("#ProductName").val();
                    var productPage = $("#ProductPageName").val();
                    $form[0].reset();
                    $("#ProductName").val(productName);
                    $("#ProductPageName").val(productPage);
                    $("#Country").val("India");
                    $("#CountryCode").val("+91");
                    $(".enquiry-country__value").text("India");
                    $(".enquiry-phone__code-value").text("+91");
                    $(".enquiry-phone__flag").text("🇮🇳");
                    $('[data-valmsg-for]').text("");

                    $("#enquiryMessage").html(
                        '<div class="cu-form-alert cu-form-alert--success">' + response.message + "</div>"
                    );

                    $("html, body").animate({
                        scrollTop: $("#enquiryMessage").offset().top - 100
                    }, 500);
                } else {
                    if (response && response.errors) {
                        $.each(response.errors, function (key, msg) {
                            $('[data-valmsg-for="' + key + '"]').text(msg || "");
                        });
                    }
                    $("#enquiryMessage").html(
                        '<div class="cu-form-alert cu-form-alert--error">' + ((response && response.message) || "Something went wrong.") + "</div>"
                    );
                }
            },
            error: function (xhr) {
                restoreSubmitButton();
                var msg = "Something went wrong. Please try again.";
                if (xhr && xhr.status === 400) {
                    msg = "Security token expired. Please refresh the page and try again.";
                } else if (xhr && xhr.responseJSON && xhr.responseJSON.message) {
                    msg = xhr.responseJSON.message;
                }
                $("#enquiryMessage").html(
                    '<div class="cu-form-alert cu-form-alert--error">' + msg + "</div>"
                );
            }
        });
    });

    function restoreSubmitButton() {
        $("#enquirySubmitBtn")
            .prop("disabled", false)
            .find(".site-btn__label")
            .text("Submit");
    }

    var isValid = true;

    function setError(name, message) {
        $('[data-valmsg-for="' + name + '"]').text(message || "");
        if (message) isValid = false;
    }

    function digitsOnly(value) {
        return (value || "").replace(/\D/g, "");
    }

    function validatePhoneField(id, label, required, minLen) {
        var raw = ($("#" + id).val() || "").trim();
        var digits = digitsOnly(raw);

        if (!raw) {
            if (required) setError(id, "Please enter " + label + ".");
            return;
        }

        if (digits.length > 15) {
            setError(id, label + " cannot exceed 15 digits.");
        } else if (digits.length < (minLen || 1)) {
            setError(id, "Enter a valid " + label.toLowerCase() + " (up to 15 digits).");
        } else if (!/^\+?[0-9]+$/.test(raw)) {
            setError(id, "Enter a valid " + label.toLowerCase() + ".");
        }
    }

    function validateEnquiryForm() {
        $('[data-valmsg-for]').text("");
        isValid = true;

        var name = ($("#FullName").val() || "").trim();
        if (!name) setError("FullName", "Please enter your name.");
        else if (!/^[A-Za-z][A-Za-z\s.]*$/.test(name)) setError("FullName", "Only alphabets, spaces and '.' are allowed.");
        else if (name.length > 100) setError("FullName", "Name cannot exceed 100 characters.");

        var company = ($("#CompanyName").val() || "").trim();
        if (!company) setError("CompanyName", "Please enter company name.");
        else if (/[`^~<>{}]/.test(company)) setError("CompanyName", "Please enter valid characters.");

        var street = ($("#StreetAddress").val() || "").trim();
        if (!street) setError("StreetAddress", "Please enter street address.");
        else if (/[`^~<>{}]/.test(street)) setError("StreetAddress", "Please enter valid characters.");

        var city = ($("#City").val() || "").trim();
        if (!city) setError("City", "Please enter city.");
        else if (/[`^~<>{}]/.test(city)) setError("City", "Please enter valid characters.");

        var state = ($("#State").val() || "").trim();
        if (!state) setError("State", "Please enter state.");
        else if (/[`^~<>{}]/.test(state)) setError("State", "Please enter valid characters.");

        var country = ($("#Country").val() || "").trim();
        if (!country) setError("Country", "Please select country.");

        validatePhoneField("Phone", "Phone", false, 1);
        validatePhoneField("Fax", "Fax", true, 1);
        validatePhoneField("Mobile", "Mobile", true, 7);

        var code = ($("#CountryCode").val() || "").trim();
        if (!code || !/^\+?[0-9]{1,4}$/.test(code)) {
            setError("CountryCode", "Enter a valid country code.");
        }

        var email = ($("#Email").val() || "").trim();
        if (!email) setError("Email", "Please enter email.");
        else if (!/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(email)) {
            setError("Email", "Enter a valid email address.");
        }

        var business = ($("#BusinessType").val() || "").trim();
        if (!business) setError("BusinessType", "Please enter business type.");
        else if (/[`^~<>{}]/.test(business)) setError("BusinessType", "Please enter valid characters.");

        var details = ($("#EnquiryDetails").val() || "").trim();
        if (!details) setError("EnquiryDetails", "Please enter enquiry details.");
        else if (/[`^~<>{}]/.test(details)) setError("EnquiryDetails", "Please enter valid characters.");

        if (!$("#AcceptTerms").prop("checked")) {
            setError("AcceptTerms", "Please accept the privacy policy and terms of use.");
        }

        if (!$("#NotRobot").prop("checked")) {
            setError("NotRobot", "Please confirm you are not a robot.");
        }

        if (!$("#ProductName").val()) {
            setError("ProductName", "Product is required.");
        }

        return isValid;
    }
});
