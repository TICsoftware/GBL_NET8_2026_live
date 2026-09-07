$(document).ready(function () {

    $(document).on("submit", "#contactForm", function (e) {
        e.preventDefault();

        if (!validateContactForm())
            return;

        $.ajax({
            url: $(this).attr("action"),
            type: "POST",
            data: $(this).serialize(),
            beforeSend: function () {
                $("#contactForm button[type='submit']")
                    .prop("disabled", true)
                    .text("Sending...");
            },
            success: function (response) {

                $("#contactForm button[type='submit']")
                    .prop("disabled", false)
                    .html('<span>Send enquiry</span><img src="/Assets/images/arrows/creamarrow-long-right.png" alt="">');

                if (response.status) {

                    // Option 1 - Reset the form
                    $("#contactForm")[0].reset();

                    grecaptcha.reset();
                    $("#captchaError").text("");

                    $(".cu-field-error").text("");

                    $("#contactMessage").html(
                        `<div class="cu-form-alert cu-form-alert--success">
                            ${response.message}
                        </div>`
                    );

                    $('html, body').animate({
                        scrollTop: $("#contactMessage").offset().top - 100
                    }, 500);

                }
                else {

                    $("#contactMessage").html(
                        `<div class="cu-form-alert cu-form-alert--error">
                            ${response.message}
                        </div>`
                    );
                }
            },
            error: function () {

                $("#contactForm button[type='submit']")
                    .prop("disabled", false)
                    .html('<span>Send enquiry</span><img src="/Assets/images/arrows/creamarrow-long-right.png" alt="">');

                $("#contactMessage").html(
                    `<div class="cu-form-alert cu-form-alert--error">
                        Something went wrong. Please try again.
                    </div>`
                );
                grecaptcha.reset();
                $("#captchaError").text("");
            }
        });

    });





    function validateContactForm() {

        $(".cu-field-error").text("");
    
        let isValid = true;
    
        // Full Name
        let fullName = $("#FullName").val().trim();
        if (fullName === "") {
            $('[data-valmsg-for="FullName"]').text("Please enter your full name.");
            isValid = false;
        } else if (!/^[A-Za-z][A-Za-z\s.]*$/.test(fullName)) {
            $('[data-valmsg-for="FullName"]').text("Only alphabets, spaces and '.' are allowed.");
            isValid = false;
        }
        else{
            $('[data-valmsg-for="FullName"]').text("");
        }

         // Organisation
         let designation = $("#Designation").val().trim();
         if (designation === "") {
             $('[data-valmsg-for="Designation"]').text("Please enter your designation name.");
             isValid = false;
         }
         else{
            $('[data-valmsg-for="Designation"]').text("");
        }
    
        // Organisation
        let organisation = $("#Organisation").val().trim();
        if (organisation === "") {
            $('[data-valmsg-for="Organisation"]').text("Please enter your organisation name.");
            isValid = false;
        }
        else{
            $('[data-valmsg-for="Organisation"]').text("");
        }
    
        // Email
        let email = $("#Email").val().trim();

        if (email === "") {
            $('[data-valmsg-for="Email"]').text("Please enter your work email.");
            isValid = false;
        } else {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[a-zA-Z]{2,}$/;
        
            if (!emailRegex.test(email)) {
                $('[data-valmsg-for="Email"]').text("Please enter a valid email address.");
                isValid = false;
            } else {
                $('[data-valmsg-for="Email"]').text("");
            }
        }
    
        // Phone
        let phone = $("#Phone").val().trim();

        if (phone === "") {
            $('[data-valmsg-for="Phone"]').text("Please enter your contact number.");
            isValid = false;
        } else {
        
            // Remove spaces, hyphens, and parentheses
            let cleanPhone = phone.replace(/[\s()-]/g, "");
        
            // International format: + followed by 7-15 digits, or 7-15 digits without +
            const phoneRegex = /^\+?[1-9]\d{6,14}$/;
        
            if (!phoneRegex.test(cleanPhone)) {
                $('[data-valmsg-for="Phone"]').text("Please enter a valid phone number.");
                isValid = false;
            }
            else {
                $('[data-valmsg-for="Phone"]').text("");
            }
        }
    
        // State
        if ($("#StateId").val() === "" || $("#StateId").val() === "0") {
            $('[data-valmsg-for="StateId"]').text("Please select your state.");
            isValid = false;
        }
        else {
            $('[data-valmsg-for="StateId"]').text("");
        }

        // City
        let city = $("#City").val().trim();
        if (city === "") {
            $('[data-valmsg-for="City"]').text("Please enter your city.");
            isValid = false;
        }
        else{
            $('[data-valmsg-for="City"]').text("");
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
    
        return isValid;
    }

    $(document).on("change", "#StateId", function () {

        var selectedStateText = $(this).find("option:selected").text();
    
        if ($(this).val() === "") {
            $("#State").val("");
        } else {
            $("#State").val(selectedStateText);
        }
    
    });


     $(document).on("change", "#CategoryId", function () {

        var selectedStateText = $(this).find("option:selected").text();
    
        if ($(this).val() === "") {
            $("#CategoryTitle").val("");
        } else {
            $("#CategoryTitle").val(selectedStateText);
        }
    
    });

    // $("#StateId").change(function () {

    //     var stateId = $(this).val();
    
    //     $("#CityId").html('<option value="">Loading...</option>');
    
    //     $.get("/Product/GetCities", { stateId: stateId }, function (data) {
    
    //         var options = '<option value="">Select city</option>';
    
    //         $.each(data, function (i, item) {
    //             options += '<option value="' + item.cityId + '">' + item.cityName + '</option>';
    //         });
    
    //         $("#CityId").html(options);
    //     });
    // });


});