$(document).ready(function () {

    $(document).on("submit", "#solutionsEnquiryForm", function (e) {
        e.preventDefault();

        if (!validateSolutionsEnquiryForm())
            return;

        $.ajax({
            url: $(this).attr("action"),
            type: "POST",
            data: $(this).serialize(),

            beforeSend: function () {
                $("#solutionsEnquiryMessage").empty();
                $("#solutionsEnquiryForm button[type='submit']")
                    .prop("disabled", true)
                    .text("Submitting...");
            },

            success: function (response) {
                restoreSubmitButton();

                if (response.status) {
                    $("#solutionsEnquiryForm")[0].reset();
                    $("#solutionsEnquiryForm").trigger("reset");
                    $('[data-valmsg-for]').text("");

                    $("#solutionsEnquiryMessage").html(
                        `<div class="cu-form-alert cu-form-alert--success">${response.message}</div>`
                    );

                    $("html, body").animate({
                        scrollTop: $("#solutionsEnquiryMessage").offset().top - 100
                    }, 500);
                } else {
                    if (response.errors) {
                        $.each(response.errors, function (key, msg) {
                            $('[data-valmsg-for="' + key + '"]').text(msg || "");
                        });
                    }
                    $("#solutionsEnquiryMessage").html(
                        `<div class="cu-form-alert cu-form-alert--error">${response.message}</div>`
                    );
                }
            },

            error: function () {
                restoreSubmitButton();
                $("#solutionsEnquiryMessage").html(
                    `<div class="cu-form-alert cu-form-alert--error">Something went wrong. Please try again.</div>`
                );
            }
        });
    });

    function restoreSubmitButton() {
        $("#solutionsEnquiryForm button[type='submit']")
            .prop("disabled", false)
            .html('Request a Call Back <svg width="16" height="10" viewBox="0 0 16 10" fill="none" aria-hidden="true"><path d="M0 5h14M10 1l4 4-4 4" stroke="#231f20" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round" /></svg>');
    }

    function setError(name, message) {
        var $el = $('[data-valmsg-for="' + name + '"]');
        $el.text(message || "");
        if (message) {
            isValidFlag = false;
        }
    }

    var isValidFlag = true;

    function validateSolutionsEnquiryForm() {
        $('[data-valmsg-for]').text("");
        isValidFlag = true;

        var name = ($("#FullName").val() || "").trim();
        if (name === "") {
            setError("FullName", "Please enter your full name.");
        } else if (!/^[A-Za-z][A-Za-z\s.]*$/.test(name)) {
            setError("FullName", "Only alphabets, spaces and '.' are allowed.");
        }

        var phone = ($("#PhoneNumber").val() || "").trim();
        if (phone === "") {
            setError("PhoneNumber", "Please enter your phone number.");
        } else if (!/^\d{10}$/.test(phone)) {
            setError("PhoneNumber", "Enter a valid 10-digit mobile number.");
        }

        var whatsapp = ($("#WhatsAppNumber").val() || "").trim();

        if (whatsapp !== "" && !/^\d{10}$/.test(whatsapp)) {
            setError("WhatsAppNumber", "Enter a valid 10-digit WhatsApp number.");
        }

        // var whatsapp = ($("#WhatsAppNumber").val() || "").trim();
        // // if (whatsapp === "") {
        // //     setError("WhatsAppNumber", "Please enter your WhatsApp number.");
        // // } 
        // if (whatsapp != "") {
        // if (!/^\d{10}$/.test(whatsapp)) {
        //     setError("WhatsAppNumber", "Enter a valid 10-digit WhatsApp number.");
        // }
        //    }

        var email = ($("#EmailAddress").val() || "").trim();

        if (email === "") {
            setError("EmailAddress", "Please enter your email address.");
        } else if (!/^[^\s@]+@[^\s@]+\.[a-zA-Z]{2,}$/.test(email)) {
            setError("EmailAddress", "Please enter a valid email address.");
        }

        if (!$("#Gender").val()) setError("Gender", "Please select gender.");
        if (!$("#AgeGroup").val()) setError("AgeGroup", "Please select age group.");
        //if (!$("#District").val()) setError("District", "Please select district.");

        if (!$("#District").val()) {
            setError("District", "Please select district.");
        } 
        else if ($("#District").val() === "Other" && !$("#OtherDistrict").val().trim()) {
            setError("OtherDistrict", "Please enter other district.");
        }

        var town = ($("#TownVillage").val() || "").trim();
       // if (town === "") setError("TownVillage", "Please enter town / village.");

        var occupation = $("#CurrentOccupation").val();
        if (!occupation) {
            setError("CurrentOccupation", "Please select current occupation.");
        } else if (occupation === "Other") {
            var others = ($("#CurrentOccupationOthers").val() || "").trim();
            if (others === "") setError("CurrentOccupationOthers", "Please specify your occupation.");
        }

        if (!$("input[name='OwnShopOrCommercialSpace']:checked").length) {
            setError("OwnShopOrCommercialSpace", "Please select an option.");
        }
        if (!$("input[name='PreviouslyRunBusiness']:checked").length) {
            setError("PreviouslyRunBusiness", "Please select an option.");
        }

        var space = $("input[name='HaveSpaceForStoreSetup']:checked").val();
        if (!space) {
            setError("HaveSpaceForStoreSetup", "Please select an option.");
        } else if (space === "Yes") {
            var sqft = ($("#StoreSizeSqFt").val() || "").trim();
            if (sqft === "") {
                setError("StoreSizeSqFt", "Please enter store size in sq.ft.");
            } else if (!/^\d+(\.\d+)?$/.test(sqft)) {
                setError("StoreSizeSqFt", "Enter a valid size in sq.ft.");
            }
        }

        // Consent
        if (!$("#Consent").prop("checked")) {
            $('[data-valmsg-for="Consent"]').text("Please accept the consent.");
            isValid = false;
        }
        else {
            $('[data-valmsg-for="Consent"]').text("");
        }

        // Google reCAPTCHA
        var captchaResponse = grecaptcha.getResponse();

        if (!captchaResponse || captchaResponse.length === 0) {
            $("#captchaError").text("Please complete the captcha.");
            isValid = false;
        } else {
            $("#captchaError").text("");
        }

        return isValidFlag;
    }


    function toggleOtherDistrict() {
        if ($("#District").val() === "Other") {
            $("#otherDistrictDiv").show();
        } else {
            $("#otherDistrictDiv").hide();
            $("#OtherDistrict").val(""); // Clear value when hidden
        }
    }

    // On page load
    toggleOtherDistrict();

    // On dropdown change
    $("#District").change(function () {
        toggleOtherDistrict();
    });

});
