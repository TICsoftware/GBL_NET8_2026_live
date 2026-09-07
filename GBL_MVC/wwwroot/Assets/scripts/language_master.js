(function () {
    if (!window.jQuery || !$.validator) return;

    $.validator.addMethod("regex", function (value, element, pattern) {
        if (this.optional(element)) return true;
        var regex = new RegExp(pattern);
        return regex.test(value);
    });

    $("form").validate({
        rules: {
            Language_Name: {
                required: true,
                minlength: 2,
                maxlength: 50,
                regex: "^[A-Za-z\\s\\.\\-']+$"
            },
            Language_Sequence: {
                required: true,
                digits: true,
                min: 1,
                max: 9999
            },
            Status: {
                required: true
            }
        },
        messages: {
            Language_Name: {
                required: "Language name is required.",
                minlength: "Language name must be at least 2 characters.",
                maxlength: "Language name cannot exceed 50 characters.",
                regex: "Language name contains invalid characters."
            },
            Language_Sequence: {
                required: "Sequence is required.",
                digits: "Sequence must be a number.",
                min: "Sequence must be at least 1.",
                max: "Sequence cannot exceed 9999."
            },
            Status: {
                required: "Status is required."
            }
        }
    });
})();
