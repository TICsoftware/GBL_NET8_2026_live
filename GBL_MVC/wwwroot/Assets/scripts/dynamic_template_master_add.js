// Client-side regex validation aligned with server-side checks.
(function () {
    if (!window.jQuery || !$.validator) return;

    $.validator.addMethod("regex", function (value, element, pattern) {
        if (this.optional(element)) return true;
        var regex = new RegExp(pattern);
        return regex.test(value);
    });

    var $form = $("form[asp-action='Add'], form[action$='/Add']");
    if (!$form.length) {
        $form = $("form.form-horizontal");
    }

    $form.validate({
        ignore: [],
        rules: {
            Language_Master_ID: {
                required: true
            },
            Title: {
                required: true,
                minlength: 3,
                maxlength: 100,
                regex: "^[A-Za-z0-9\\s\\.\\,\\-\\_\\(\\)\\/&]{3,100}$"
            },
            Status: {
                required: true
            },
            Design_Layout: {
                required: false,
                minlength: 5,
                maxlength: 5000,
                regex: "^[A-Za-z0-9\\s\\.\\,\\-\\_\\(\\)\\/&\\:\\;\\!\\?\\#\\%\\+\\=\\@\\{\\}\\[\\]<>\\x22'\\r\\n]{5,5000}$"
            },
            TemplateIds: {
                required: true
            }
        },
        messages: {
            Language_Master_ID: {
                required: "Please select a language."
            },
            Title: {
                required: "Title is required.",
                minlength: "Title must be at least 3 characters.",
                maxlength: "Title cannot exceed 100 characters.",
                regex: "Title contains invalid characters."
            },
            Status: {
                required: "Please select status."
            },
            Design_Layout: {
                minlength: "Design Layout must be at least 5 characters.",
                maxlength: "Design Layout cannot exceed 5000 characters.",
                regex: "Design Layout contains invalid characters."
            },
            TemplateIds: {
                required: "Please select at least one template."
            }
        }
    });
})();
