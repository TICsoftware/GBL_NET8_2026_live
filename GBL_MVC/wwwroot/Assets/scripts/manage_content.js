/** Closes the spot-templates dialog (used after AJAX save from partials such as AddData_Temp). */
window.hideSpotTemplatesModal = function () {
    var el = document.getElementById("modal_spottemplates");
    if (!el) return;
    var inst = bootstrap.Modal.getInstance(el);
    if (inst) {
        inst.hide();
    } else {
        bootstrap.Modal.getOrCreateInstance(el).hide();
    }
};

window.Refresh_context_details = function (isrefresh) {
    // alert('Refresh_context_details');
    if (isrefresh) {

        var modalEl = document.getElementById('modal_spottemplates');

        if (modalEl && modalEl.classList.contains('show')) {

            var modal = bootstrap.Modal.getInstance(modalEl);

            if (modal) {
                modal.hide();
            }
        }
    }
    var _templateid = $("#ddTemplates").val();
    var _language_id = $("#ddLanguage").val();
    var _spot_temp_id = $("#Spot_temp_id").val();
    if (_templateid != null && _templateid > 0) {
        $.ajax({
            url: '/Content/Add_Context_List',
            type: 'GET',
            data: { template_Id: _templateid, language_id: _language_id, Spot_temp_id: _spot_temp_id },
            success: function (data) {
                if (data != null)
                    $("#div_contentspotmapping").html(data);
                else
                    $("#div_contentspotmapping").html("");
            },
            error: function (xhr) {
                alert(xhr.status + " : " + xhr.responseText);
            },
            complete: function (jqXHR) {
            }
        });

    }
    else
        $("#div_contentspotmapping").html("");


};

window.Delete_Spot_templates = function (cont_id) {
    var tempId = $("#Spot_temp_id").val();
    if (!confirm("Are you sure to discard selected spots?")) {
        return false;
    }

    $.ajax({
        url: '/ContentSpotMapping/Delete_Spot_Mapping',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            contId: cont_id,
            Spottempid: tempId,
        }),
        success: function (html) {
            var el = document.getElementById("spotTemplatesModalBody");
            if (el && typeof window.destroyCkEditors === "function") {
                window.destroyCkEditors(el);
            }
            $("#spotTemplatesModalBody").html(html);
            $("#modal_spottemplates").modal("hide");
        },
        error: function () {
            alert("Something went wrong. Please try again.");
        }
    });
};



window.Load_Edit_context_Temp_details = function (isrefresh, _templateid, _language_id, _Id_encrypt_val) {

    //alert('Load_Edit_context_Temp_details');

    if (isrefresh) {

        _templateid = $("#Template_Master_ID").val();
        _language_id = $("#ddLanguage").val();
        _Id_encrypt_val = $("#Id_encrypt_val").val();
        //alert(_templateid + " | " + _language_id + " | " + _Id_encrypt_val);

        if ($('#modal_spottemplates').hasClass('show')) {
            $("#modal_spottemplates").modal("hide");
        }

    }
    if (_templateid != null && _templateid > 0) {
        $.ajax({
            url: '/Content/Edit_Context_Temp',
            type: 'GET',
            data: { template_Id: _templateid, language_id: _language_id, Id_encrypt_val: _Id_encrypt_val },
            success: function (data) {
                if (data != null)
                {
                    $("#div_contentspotmapping").html(data);
                    $('.list_context').each(function () {
                        var id = $(this).data('paging-id');
                        generatePagination(
                            '.js-pagination-div' + id,
                            '.js-pagination-div-item' + id,
                            10,
                            '<<',
                            '>>'
                        );
                    });
                }
                else
                    $("#div_contentspotmapping").html("");
            },
            error: function (xhr) {
                alert(xhr.status + " : " + xhr.responseText);
            },
            complete: function (jqXHR) {
            }
        });

    }
    else
        $("#div_contentspotmapping").html("");


};




window.Load_Edit_context_details = function (isrefresh, _templateid, _language_id, _Id_encrypt_val) {
    if (isrefresh) {
        if ($('#modal_spottemplates').hasClass('show')) {
            $("#modal_spottemplates").modal("hide");
        }

        _templateid = $("#ddTemplates").val();
        _language_id = $("#ddLanguage").val();
        _Id_encrypt_val = $("#Id_encrypt_val").val();
    }
    if (_templateid != null && _templateid > 0) {
        $.ajax({
            url: '/Content/Edit_Context_List',
            type: 'GET',
            data: { template_Id: _templateid, language_id: _language_id, Id_encrypt_val: _Id_encrypt_val },
            success: function (data) {
                if (data != null)
                {
                    $("#div_contentspotmapping").html(data);
                    $('.list_context').each(function () {
                        var id = $(this).data('paging-id');
                        generatePagination(
                            '.js-pagination-div' + id,
                            '.js-pagination-div-item' + id,
                            10,
                            '<<',
                            '>>'
                        );
                    });
                }
                else
                    $("#div_contentspotmapping").html("");
            },
            error: function (xhr) {
                alert(xhr.status + " : " + xhr.responseText);
            },
            complete: function (jqXHR) {
            }
        });

    }
    else
        $("#div_contentspotmapping").html("");


};




(function () {

    function clearSpotTemplatesModalBody() {
        var el = document.getElementById("spotTemplatesModalBody");
        if (el && typeof window.destroyCkEditors === "function") {
            window.destroyCkEditors(el);
        }
        $("#spotTemplatesModalBody").html("");
    }

    /**
     * Opens the spot-templates modal and inits CKEditor on .editor-full inside the loaded partial.
     * Must not depend on initCkEditors existing at registration time (async ckeditor5 load).
     * shown.bs.modal may not fire if the modal is already .show — use timed fallbacks.
     */
    function showSpotTemplatesModalAndInitEditors() {
        var modalEl = document.getElementById("modal_spottemplates");
        if (!modalEl) {
            return;
        }
        var modal = bootstrap.Modal.getOrCreateInstance(modalEl);

        function runSpotModalCkInit() {
            var body = document.getElementById("spotTemplatesModalBody");
            if (!body || typeof window.initCkEditors !== "function") {
                return false;
            }
            window.initCkEditors(body);
            return true;
        }

        function tryInitCkEditorsWithRetry() {
            var attempts = 0;
            function attempt() {
                if (runSpotModalCkInit()) {
                    requestAnimationFrame(runSpotModalCkInit);
                    setTimeout(runSpotModalCkInit, 50);
                    setTimeout(runSpotModalCkInit, 200);
                    return;
                }
                attempts += 1;
                if (attempts < 80) {
                    setTimeout(attempt, 50);
                }
            }
            attempt();
        }

        modalEl.addEventListener(
            "shown.bs.modal",
            function onSpotModalShown() {
                tryInitCkEditorsWithRetry();
            },
            { once: true }
        );

        modal.show();

        // If shown.bs.modal does not fire (edge cases), init once the modal is visible
        setTimeout(function () {
            if (modalEl.classList.contains("show")) {
                tryInitCkEditorsWithRetry();
            }
        }, 0);
        setTimeout(function () {
            if (modalEl.classList.contains("show")) {
                tryInitCkEditorsWithRetry();
            }
        }, 400);
    }

    $("#ddSections").on("change", function () {

        let cont_Id = $(this).val();

        if (cont_Id > 0) {
            $.ajax({
                url: '/Content/Load_sections',
                type: 'GET',
                data: { cont_id: cont_Id },
                success: function (data) {

                    // --------------------
                    // Bind subsections
                    // --------------------                   
                    let ddsubsections = $("#ddSubSections");
                    ddsubsections.empty();
                    if (data.subSections == null) {
                        ddsubsections.append(`<option value="0">Select</option>`);
                    }
                    else {
                        $.each(data.subSections, function (i, item) {
                            ddsubsections.append(`<option value="${item.value}">${item.text}</option>`);
                        });
                    }

                    // --------------------
                    // Bind articles
                    // --------------------
                    let ddarticles = $("#ddArticle");
                    ddarticles.empty();
                    if (data.Articles == null) {
                        ddarticles.append(`<option value="0">Select</option>`);
                    }
                    else {
                        $.each(data.Articles, function (i, item) {
                            ddarticles.append(`<option value="${item.Value}">${item.Text}</option>`);
                        });
                    }
                },
                error: function (xhr) {
                    alert(xhr.status + " : " + xhr.responseText);
                },
                complete: function (jqXHR) {
                }
            });
        }

    });

    $("#ddLanguage").on("change", function () {

        let language_id = $(this).val();
        if (language_id > 0) {
            $.ajax({
                url: '/Content/Load_Language_sections',
                type: 'GET',
                data: { language_id: language_id },
                success: function (data) {
                    $("#div_language_sections").show();
                    // --------------------
                    // Bind subsections
                    // --------------------
                    let ddsections = $("#ddLanguage_Sections");
                    ddsections.empty();
                    if (data.Sections == null) {
                        ddsections.append(`<option value="0">Select</option>`);
                    }
                    else {
                        $.each(data.Sections, function (i, item) {
                            ddsections.append(`<option value="${item.value}">${item.text}</option>`);
                        });
                    }

                    let ddsubsections = $("#ddLanguage_SubSections");
                    ddsubsections.empty().append('<option value="0">Select</option>');

                },
                error: function (xhr) {
                    alert(xhr.status + " : " + xhr.responseText);
                },
                complete: function (jqXHR) {
                }
            });
        }
    });

    $("#ddLanguage_Sections").on("change", function () {

        let language_id = $("#ddLanguage").val();
        let cont_Id = $(this).val();
        if (cont_Id > 0) {
            $.ajax({
                url: '/Content/Load_Language_subsections',
                type: 'GET',
                data: { language_id: language_id, cont_id: cont_Id },
                success: function (data) {

                    // --------------------
                    // Bind subsections
                    // --------------------
                    let ddsubsections = $("#ddLanguage_SubSections");
                    ddsubsections.empty();
                    if (data.subsections == null) {
                        ddsubsections.append(`<option value="0">Select</option>`);
                    }
                    else {
                        $.each(data.subsections, function (i, item) {
                            ddsubsections.append(`<option value="${item.value}">${item.text}</option>`);
                        });
                    }
                },
                error: function (xhr) {
                    alert(xhr.status + " : " + xhr.responseText);
                },
                complete: function (jqXHR) {
                }
            });
        }
    });

    $("#ddtemplate_type").on("change", function () {
        var _templateid = $(this).val();
        if (_templateid == "1") {
            $("#div_addspot_templates").hide();
            $(".card-body-intro-content").show();
        }
        else {
            $("#div_addspot_templates").show();
            $(".card-body-intro-content").hide();
        }



    });

    $("#ddTemplates").on("change", function () {
        Refresh_context_details(false);
    });

    // ================================================================
    // OPEN MODAL
    // ================================================================
    $(document).on("click", ".add-spottemplate", function (e) {
        e.preventDefault();
        e.stopPropagation();
        var tempId = $("#Spot_temp_id").val();
        var encryptId = $(this).attr("data-encId");
        // Remove any existing backdrops safely
        //   $(".modal-backdrop").remove();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();

        // Load partial view
        var partialurl = "/DynamicData/AddData_Temp?encId=" + encodeURIComponent(tempId + '|$|' + encryptId) +
            "&returnUrl=" + encodeURI(window.location.pathname);

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });

    $(document).on("click", ".add-temp-spottemplate", function (e) {
        e.preventDefault();
        e.stopPropagation();
        var encryptcontId = $("#Id_encrypt_val").val();
        var encryptId = $(this).attr("data-encId");
        // Remove any existing backdrops safely
        //   $(".modal-backdrop").remove();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();

        // Load partial view
        var partialurl = "/DynamicData/AddData?encId=" + encodeURIComponent(encryptcontId + '|$|' + encryptId) +
            "&returnUrl=" + encodeURI(window.location.pathname);

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });

    $(document).on("click", ".edit-spottemplate", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var encryptId = $(this).attr("data-encId");
        var group_Id = $(this).attr("data-groupid");
        var content_Id = $("#Id_encrypt_val").val();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();

        // Load partial view


        var partialurl = "/DynamicData/EditData?encId=" + encodeURIComponent(encryptId) + "&gid=" + group_Id + "&cont_id=" + content_Id;

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });

    $(document).on("click", ".edit-spottemplate-temp", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var encryptId = $(this).attr("data-encId");
        var group_Id = $(this).attr("data-groupid");
        var cont_temp_id = $("#Spot_temp_id").val();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();

        // Load partial view

        var partialurl = "/DynamicData/EditData_Temp?encId=" + encodeURIComponent(encryptId) + "&gid=" + group_Id + "&temp_cont_id=" + cont_temp_id;

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });


    $(document).on("click", ".edit-component-temp", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var encryptId = $(this).attr("data-encId");
        var group_Id = $(this).attr("data-groupid");
        var cont_temp_id = $("#Spot_temp_id").val();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();

        // Load partial view

        var partialurl = "/DynamicData/EditMainComponent_Temp?encId=" + encodeURIComponent(encryptId) + "&gid=" + group_Id + "&temp_cont_id=" + cont_temp_id;

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });


    $(document).on("click", ".delete-spottemplate", function (e) {

        e.preventDefault();
        e.stopPropagation();
        if(!confirm("Are you sure you want to delete data?"))
        {
            return false;
        }

        var encryptId = $(this).attr("data-encId");
        var group_Id = $(this).attr("data-groupid");

        $.ajax({
            url: '/DynamicData/DeleteData',
            type: 'Post',
            data: { CTR_Id: encryptId, gid: group_Id },
            success: function (data) {
                alert("Data deleted successfully");
                Load_Edit_context_Temp_details(true);
            },
            error: function (xhr) {
                alert(xhr.status + " : " + xhr.responseText);
            },
            complete: function (jqXHR) {
            }
        });
    });



    $(document).on("click", ".add_component_temp", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var tempId = $("#Spot_temp_id").val();
        var encryptId = $(this).attr("data-encId");
        // Remove any existing backdrops safely
        //   $(".modal-backdrop").remove();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();

        // Load partial view
        var partialurl = "/DynamicData/AddMainComponent_Temp?encId=" + encodeURIComponent(tempId + '|$|' + encryptId) +
            "&returnUrl=" + encodeURI(window.location.pathname);

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });

    $(document).on("click", ".edit_component", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var encryptId = $(this).attr("data-encId");
        var group_Id = $(this).attr("data-groupid");
        var content_Id = $("#Id_encrypt_val").val();
        // Remove any existing backdrops safely
        //   $(".modal-backdrop").remove();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();
        // Load partial view
        var partialurl = "/DynamicData/EditComponent?encId=" + encodeURIComponent(encryptId) +
            "&gid=" + group_Id + "&cont_id=" + content_Id;

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });


    /*$(document).on("click", ".add-temp-spottemplate", function (e) {
       e.preventDefault();
       e.stopPropagation();
       var encryptcontId = $("#Id_encrypt_val").val();
       var encryptId = $(this).attr("data-encId");
       // Remove any existing backdrops safely
       //   $(".modal-backdrop").remove();
       $("body").removeClass("modal-open");

       clearSpotTemplatesModalBody();

       // Load partial view
       var partialurl = "/DynamicData/AddData?encId=" + encodeURIComponent(encryptcontId + '|$|' + encryptId) +
           "&returnUrl=" + encodeURI(window.location.pathname);

       $("#spotTemplatesModalBody").load(partialurl, function () {
           showSpotTemplatesModalAndInitEditors();
       }); 
   });*/

    $(document).on("click", ".add_component", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var encryptcontId = $("#Id_encrypt_val").val();
        var encryptId = $(this).attr("data-encId");
        // Remove any existing backdrops safely
        //   $(".modal-backdrop").remove();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();

        // Load partial view
        var partialurl = "/DynamicData/AddMainComponent?encId=" + encodeURIComponent(encryptcontId + '|$|' + encryptId) +
            "&returnUrl=" + encodeURI(window.location.pathname);

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });

    $(document).on("click", ".delete-content", function (e) {
        if(!confirm("Are you sure you want to delete page?"))
        {
            return false;
        }
        var trcontent = $(this);
        $.ajax({
            url: '/Content/UpdateContentStatus',
            type: 'Post',
            data: { cont_id: trcontent.attr("data-routeId") },
            success: function (data) {
                alert(data.message);
                if (data.refreshstatus == 1) {
                    if (trcontent.attr("data-routeId") == "section") {
                        trcontent.closest("tr.js-pagination-section-item").remove();
                    }
                    else {
                        trcontent.closest("tr.js-pagination-article-item").remove();
                    }
                }
            },
            error: function (xhr) {
                alert(xhr.status + " : " + xhr.responseText);
            },
            complete: function (jqXHR) {
            }
        });

    });

    $(document).on("click", "#btnsearchpublishedlist", function (e) {
        $("#page_no").val("1");
        $("#PublishedContentForm").submit();
    });

    $(document).on("submit", "#PublishedContentForm", function (e) {
        e.preventDefault();
        var formData = new FormData(this);
        var pg_no = 1;
        if ($("#page_no").length > 0) {
            pg_no = $("#page_no").val();
        }
        $.ajax({
            url: "/EditContent/ListPublished?page_no=" + encodeURIComponent(pg_no),
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (html) {
                $("#divDisplaySections").html(html);
            },
            error: function (xhr, ajaxOptions, response) {
                alert("Error " + xhr.status + ": " + xhr.responseText);
                alert("Something wrong. Please try again.");
            },
            finally: function (xhr) {
                $(".btn-loader").css('display', 'none');
            }
        });
    });

    if ($("#PublishedContentForm").length) {
        $("#PublishedContentForm").submit();
    };

    $(document).on("click", ".publish-add-spottemplate", function (e) {
        e.preventDefault();
        e.stopPropagation();
        var encryptcontId = $("#Id_encrypt_val").val();
        var encryptId = $(this).attr("data-encId");
        // Remove any existing backdrops safely
        //   $(".modal-backdrop").remove();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();

        // Load partial view
        var partialurl = "/DynamicDataReprocess/AddData?encId=" + encodeURIComponent(encryptcontId + '|$|' + encryptId) +
            "&returnUrl=" + encodeURI(window.location.pathname);

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });

    $(document).on("click", ".publish_add_component", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var encryptcontId = $("#Id_encrypt_val").val();
        var encryptId = $(this).attr("data-encId");
        // Remove any existing backdrops safely
        //   $(".modal-backdrop").remove();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();

        // Load partial view
        var partialurl = "/DynamicDataReprocess/AddMainComponent?encId=" + encodeURIComponent(encryptcontId + '|$|' + encryptId) +
            "&returnUrl=" + encodeURI(window.location.pathname);

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });

    $(document).on("click", ".publish_edit_component", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var encryptId = $(this).attr("data-encId");
        var group_Id = $(this).attr("data-groupid");
        var content_Id = $("#Id_encrypt_val").val();
        // Remove any existing backdrops safely
        //   $(".modal-backdrop").remove();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();
        // Load partial view
        var partialurl = "/DynamicDataReprocess/EditComponent?encId=" + encodeURIComponent(encryptId) +
            "&gid=" + group_Id + "&cont_id=" + content_Id;

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });

    $(document).on("click", ".publish-edit-spottemplate", function (e) {

        e.preventDefault();
        e.stopPropagation();
        var encryptId = $(this).attr("data-encId");
        var group_Id = $(this).attr("data-groupid");
        var content_Id = $("#Id_encrypt_val").val();
        $("body").removeClass("modal-open");

        clearSpotTemplatesModalBody();

        // Load partial view


        var partialurl = "/DynamicDataReprocess/EditData?encId=" + encodeURIComponent(encryptId) + "&gid=" + group_Id + "&cont_id=" + content_Id;

        $("#spotTemplatesModalBody").load(partialurl, function () {
            showSpotTemplatesModalAndInitEditors();
        });
    });

    $(document).on("click", ".publish-delete-spottemplate", function (e) {
        if(!confirm("Are you sure you want to delete data?"))
        {
            return false;
        }

        e.preventDefault();
        e.stopPropagation();

        var encryptId = $(this).attr("data-encId");
        var group_Id = $(this).attr("data-groupid");

        $.ajax({
            url: '/DynamicDataReprocess/DeleteData',
            type: 'Post',
            data: { CTR_Id: encryptId, gid: group_Id },
            success: function (data) {
                alert("Data deleted successfully");
                Load_Edit_context_Published_details(true);
            },
            error: function (xhr) {
                alert(xhr.status + " : " + xhr.responseText);
            },
            complete: function (jqXHR) {
            }
        });
    });

   


})();

function validate_form() {
    if ($("#ddtemplate_type").val() == "0" || $("#ddTemplates").val() == "0") {
        alert("Please select Template");
        return false;
    }
    return true;
}

window.validate_form = validate_form;

$(document)
    .off("click.contentThumbPrevent", ".content-thumb-link")
    .on("click.contentThumbPrevent", ".content-thumb-link", function (e) {
        e.preventDefault();
    });

$(document)
    .off("click.contentValidate", ".js-validate-content-form")
    .on("click.contentValidate", ".js-validate-content-form", function (e) {
        if (!validate_form()) {
            e.preventDefault();
            return false;
        }
    });

$(document)
    .off("click.contentSpotClose", "#btnsubmit_spot_templates")
    .on("click.contentSpotClose", "#btnsubmit_spot_templates", function () {
        var action = $(this).attr("data-close-action");
        if (action === "refresh" && typeof window.Refresh_context_details === "function") {
            window.Refresh_context_details(true);
        } else if (action === "temp" && typeof window.Load_Edit_context_Temp_details === "function") {
            window.Load_Edit_context_Temp_details(true);
        } else if (action === "published" && typeof window.Load_Edit_context_Published_details === "function") {
            window.Load_Edit_context_Published_details(true);
        }
    });


window.Load_Edit_context_Published_details = function (isrefresh, _templateid, _language_id, _Id_encrypt_val) {

    //alert('Load_Edit_context_Temp_details');

    if (isrefresh) {

        _templateid = $("#Template_Master_ID").val();
        _language_id = $("#ddLanguage").val();
        _Id_encrypt_val = $("#Id_encrypt_val").val();
        //alert(_templateid + " | " + _language_id + " | " + _Id_encrypt_val);

        if ($('#modal_spottemplates').hasClass('show')) {
            $("#modal_spottemplates").modal("hide");
        }

    }
    if (_templateid != null && _templateid > 0) {
        $.ajax({
            url: '/EditContent/Edit_Context_Published_Reprocess',
            type: 'GET',
            data: { template_Id: _templateid, language_id: _language_id, Id_encrypt_val: _Id_encrypt_val },
            success: function (data) {
                if (data != null) {
                    $("#div_contentspotmapping").html(data);
                    $('.list_context').each(function () {
                        var id = $(this).data('paging-id');
                        generatePagination(
                            '.js-pagination-div' + id,
                            '.js-pagination-div-item' + id,
                            10,
                            '<<',
                            '>>'
                        );
                    });
                }
                else
                    $("#div_contentspotmapping").html("");
            },
            error: function (xhr) {
                alert(xhr.status + " : " + xhr.responseText);
            },
            complete: function (jqXHR) {
            }
        });

    }
    else
        $("#div_contentspotmapping").html("");


};

