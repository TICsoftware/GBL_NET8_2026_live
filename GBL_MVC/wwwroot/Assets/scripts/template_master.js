(function () {
    var alertEl = document.getElementById("template-master-alert-message");
    if (alertEl) {
        var message = alertEl.getAttribute("data-message");
        if (message) {
            alert(message);
        }
    }

    if (!window.jQuery || !$.validator) return;

    $.validator.addMethod("regex", function (value, element, pattern) {
        if (this.optional(element)) return true;
        var regex = new RegExp(pattern);
        return regex.test(value);
    });

    $("form").validate({
        rules: {
            Language_Master_ID: {
                required: true
            },
            Name: {
                required: true,
                minlength: 2,
                maxlength: 120,
                regex: "^(?! )[A-Za-z0-9 .'\\-&()]+(?<! )$"
            },
            Status: {
                required: true
            }
        },
        messages: {
            Language_Master_ID: {
                required: "Please select a language."
            },
            Name: {
                required: "Please enter template name.",
                minlength: "Template name must be at least 2 characters.",
                maxlength: "Template name cannot exceed 120 characters.",
                regex: "Please enter only valid characters (letters, numbers, spaces, and .'-&())."
            },
            Status: {
                required: "Please select status."
            }
        }
    });

    setTimeout(function () {
        $(".alert").fadeOut("slow");
    }, 3000);
})();
