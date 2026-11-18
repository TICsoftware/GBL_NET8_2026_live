(function () {
    function mediaGroup($el) {
        var $group = $el.closest(".dynamic-media-field, .form-group");
        if (!$group.length) {
            $group = $el.closest(".mb-3");
        }
        return $group;
    }

    function clearDynamicMedia($group) {
        if (!$group || !$group.length) return;

        $group.find(".imgPreview")
            .attr("src", "")
            .removeClass("is-visible")
            .addClass("d-none")
            .hide();

        $group.find(".js-clear-dynamic-media, .content-thumb-link")
            .removeClass("is-visible")
            .addClass("d-none")
            .hide();

        $group.find("input[type='hidden']").each(function () {
            var name = (this.name || "").toLowerCase();
            if (!name) return;

            if (name.endsWith("_new") || name.endsWith("_existing") || name.indexOf("media_id") >= 0) {
                $(this).val("");
            } else if (name.endsWith("_isdelete") || name.indexOf("isdelete") >= 0) {
                $(this).val("true");
            }
        });
    }

    function showDynamicMediaDelete($group) {
        if (!$group || !$group.length) return;
        $group.find(".js-clear-dynamic-media, .content-thumb-link")
            .addClass("is-visible")
            .removeClass("d-none")
            .show();
    }

    $(document)
        .off("click.dynamicMediaClear", ".js-clear-dynamic-media")
        .on("click.dynamicMediaClear", ".js-clear-dynamic-media", function (e) {
            e.preventDefault();
            e.stopPropagation();
            clearDynamicMedia(mediaGroup($(this)));
        });

    // After media picker assigns an image, ensure Delete is visible.
    $(document)
        .off("click.dynamicMediaAddHook", "#btnAddImage")
        .on("click.dynamicMediaAddHook", "#btnAddImage", function () {
            setTimeout(function () {
                if (!window.sourceControl) return;
                showDynamicMediaDelete(mediaGroup($(window.sourceControl)));
            }, 0);
        });

    window.clearDynamicMediaField = clearDynamicMedia;
})();
