(function () {
    if (!window.jQuery || !$.validator) return;

    $.validator.addMethod("regex", function (value, element, pattern) {
        if (this.optional(element)) return true;
        var regex = new RegExp(pattern);
        return regex.test(value);
    });

    $("#contextFieldForm").validate({
        ignore: [],
        rules: {
            name: {
                required: true,
                minlength: 2,
                maxlength: 100,
                regex: "^[A-Za-z0-9\\s\\-_\\(\\)]+$"
            },
            name_key: {
                required: true,
                minlength: 2,
                maxlength: 100,
                regex: "^[A-Za-z0-9_#-]+$"
            },
            field_type_id: {
                required: true
            },
            order_id: {
                required: true,
                min: 1,
                max: 9999
            },
            is_required: {
                required: true
            },
            status: {
                required: true
            },
            is_block: {
                required: true
            }
        },
        messages: {
            name: {
                required: "Field name is required.",
                minlength: "Field name must be at least 2 characters.",
                maxlength: "Field name cannot exceed 100 characters.",
                regex: "Field name contains invalid characters."
            },
            name_key: {
                required: "Field key is required.",
                minlength: "Field key must be at least 2 characters.",
                maxlength: "Field key cannot exceed 100 characters.",
                regex: "Field key can contain letters, numbers, #, _ and -."
            },
            field_type_id: {
                required: "Please select field type."
            },
            order_id: {
                required: "Display order is required.",
                min: "Display order must be at least 1.",
                max: "Display order cannot exceed 9999."
            },
            is_required: {
                required: "Please select required option."
            },
            status: {
                required: "Please select status."
            },
            is_block: {
                required: "Please select field category."
            }
        }
    });
})();
