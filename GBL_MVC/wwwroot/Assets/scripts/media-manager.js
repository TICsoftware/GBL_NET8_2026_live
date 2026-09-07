(function () {

    // ===== GLOBAL TRACKING WHICH BUTTON OPENED POPUP =====
    window.sourceControl = window.sourceControl || null;

    // ===== VARIABLES =====
    let selectedFiles = [];
    const allowedExtensions = ["jpg", "jpeg", "png", "webp", "pdf", "docx", "ppt", "pptx", "mp4","svg"];
    const defaultText = "Click or Drag 'n' Drop Files here";
    let cropper = null;
    let filetypes = null;

    // ================================================================
    //                       DRAG + DROP + FILE INPUT
    // ================================================================
 

     $(document).on("click", "#dropZone", function (e) {
        e.stopImmediatePropagation();

         if (e.target === this) {
    
             if (selectedFiles.length > 0) {
                 selectedFiles = [];
                 $("#files").val("");
                 renderPreview();
             }
    
             $("#files").click();
         }
     });

    $(document).on("dragover dragenter", "#dropZone", function (e) {
        e.preventDefault();
        e.stopPropagation();
        $(this).addClass("drag-active");
    });

    $(document).on("dragleave", "#dropZone", function () {
        $(this).removeClass("drag-active");
    });

    $(document).on("drop", "#dropZone", function (e) {
        e.preventDefault();
        e.stopPropagation();
        $(this).removeClass("drag-active");
        addFiles([...e.originalEvent.dataTransfer.files]);
    });

    $(document).on("change", "#files", function () {
        addFiles([...this.files]);
    });

    function addFiles(files) {
       
        selectedFiles = [];   // ← THIS CLEARS PREVIOUS FILES completely
    
        files.forEach(file => {
            let ext = file.name.split(".").pop().toLowerCase();
            if (!allowedExtensions.includes(ext)) {
                alert("File not allowed: " + file.name);
                return;
            }
            selectedFiles.push(file);
        });
    
        syncInput();
        renderPreview();
    }

    function syncInput() {
        const dt = new DataTransfer();
        selectedFiles.forEach(f => dt.items.add(f));
        document.getElementById("files").files = dt.files;
    }

    function renderPreview() {
        const dropBox = document.getElementById("dropZone");
        dropBox.innerHTML = "";

        if (selectedFiles.length === 0) {
            dropBox.innerHTML = defaultText;
            document.getElementById("files").value = "";
            return;
        }

        const wrapper = document.createElement("div");
        wrapper.className = "preview-wrapper";

        selectedFiles.forEach((file, index) => {
            let ext = file.name.split(".").pop().toLowerCase();

            let item = document.createElement("div");
            item.className = "preview-item";
            item.dataset.index = index;

            item.innerHTML = `
                <span class="remove-image">×</span>
                <div class="thumb"></div>
                <div class="filename" title="${file.name}">${file.name}</div>
                <span class="file-badge badge-${ext}">${ext}</span>
            `;

            let thumb = item.querySelector(".thumb");
            if (file.type.startsWith("image/")) {
                let reader = new FileReader();
                reader.onload = e => thumb.innerHTML = `<img src="${e.target.result}">`;
                reader.readAsDataURL(file);
            } else {
                thumb.innerHTML = `<div class="file-icon">${ext.toUpperCase()}</div>`;
            }

            wrapper.appendChild(item);
        });

        dropBox.appendChild(wrapper);
    }

    // REMOVE FILE
    $(document).on("click", "#dropZone .remove-image", function () {
        let index = $(this).closest(".preview-item").data("index");
        selectedFiles.splice(index, 1);
        syncInput();
        renderPreview();
    });

    // MEDIA CARD PREVIEW

   
    $(document).on("click", ".media-card", async function () {

        // ================= GET DATA =================
        const mediaId = $(this).data("id");        // ✅ FIXED
        const fileUrl = $(this).data("url");
        const fileName = $(this).find(".media-title").text().trim();

       
        $("#txtFileUrl").val( window.location.origin +fileUrl);
    
        $("#hdnMediaId").val(mediaId);
        $("#hdnImagePreview").val(fileUrl);
    
        // ================= ACTIVE SELECTION =================
        $(".media-card").removeClass("active");
        $(this).addClass("active");
    
        const previewPanel = $("#previewPanel");
        const previewArea = $("#previewArea");
        const previewName = $("#previewName");
        const previewAlt = $("#previewAlt");
    
        previewPanel.show().attr("data-id", mediaId);
    
        previewName.val(" " + fileName);
        previewAlt.val("");
        previewArea.html("");
    
        const imgTag = $(this).find("img")[0];
        const ext = fileName.split('.').pop().toLowerCase();
    
        const isImage = ["jpg","jpeg","png","webp","svg"].includes(ext);
    
        // ================= SHOW / HIDE CUSTOMIZE BUTTON =================
        if (isImage) {
            $("#btnOpenCrop").show();   // ✅ show for images
        } else {
            $("#btnOpenCrop").hide();   // ❌ hide for pdf/video/etc
        }
    
        // ================= IMAGE =================
        if (imgTag && isImage) {
    
            try {
                const response = await fetch(fileUrl);
                const blob = await response.blob();
    
                const file = new File([blob], fileName, { type: blob.type });
    
                selectedFiles = [file];   // ✅ assign global
    
            } catch (err) {
                console.error("Error converting URL to file:", err);
            }
    
            previewArea.html(`
                <img id="cropImage" src="${imgTag.src}" style="max-width:100%;" />
            `);
        }
    
        // ================= NON IMAGE =================
        else {
    
            selectedFiles = null;
    
            previewArea.html(`
                <a href="${fileUrl}" target="_blank" class="btn btn-sm btn-dark">
                    <i class="bi bi-file-earmark"></i> Open ${ext.toUpperCase()}
                </a>
            `);
        }
    });


    // $(document).on("click", ".media-card", async function () {

    //     const mediaId = $(this).data("id");   // ✅ FIXED
    //     const fileUrl = $(this).data("url");
    //     const fileName = $(this).find(".media-title").text().trim();
    
    //     $("#hdnMediaId").val(mediaId);        // ✅ now works
    //     $("#hdnImagePreview").val(fileUrl);
    
    //     $(".media-card").removeClass("active");
    //     $(this).addClass("active");
    
    //     const previewPanel = $("#previewPanel");
    //     const previewArea = $("#previewArea");
    //     const previewName = $("#previewName");
    //     const previewAlt = $("#previewAlt");
    
    //     previewPanel.show().attr("data-id", mediaId);
    
    //     previewName.val(" " + fileName);
    //     previewAlt.val("");
    //     previewArea.html("");
    
    //     const imgTag = $(this).find("img")[0];
    //     const ext = fileName.split('.').pop().toLowerCase();
    
    //     const isImage = ["jpg","jpeg","png","webp"].includes(ext);
    
    //     // ================= IMAGE =================
    //     if (imgTag && isImage) {
    
    //         try {
    //             const response = await fetch(fileUrl);
    //             const blob = await response.blob();
    
    //             const file = new File([blob], fileName, { type: blob.type });
    //             selectedFiles = [file];
    
    //         } catch (err) {
    //             console.error("Error converting URL to file:", err);
    //         }
    
    //         previewArea.html(`<img id="cropImage" src="${imgTag.src}" style="max-width:100%;" />`);
    //     }
    
    //     // ================= NON IMAGE =================
    //     else {
    
    //         selectedFiles = null;
    
    //         previewArea.html(`
    //             <a href="${fileUrl}" target="_blank" class="btn btn-sm btn-dark">
    //                 <i class="bi bi-file-earmark"></i> Open ${ext.toUpperCase()}
    //             </a>
    //         `);
    //     }
    // });


    

    

    // ================================================================
    // DELEGATED EVENTS FOR BUTTONS INSIDE PARTIAL
    // ================================================================
    const modalBody = $("#mediaManagerModalBody");

    // Save temp button
    $(document)
    .off("click.saveTemp", "#btnAddTemp")
    .on("click.saveTemp", "#btnAddTemp", function (e) {
  
          e.preventDefault();
          e.stopImmediatePropagation();  // ✅ stop duplicate firing
  
          const id = $("#previewPanel").data("id");
          const fileName = $("#previewName").val().trim();
          const altText = $("#previewAlt").val().trim();
          const img = $("#previewArea img")[0];
  
          if (!fileName) {
              alert("File name cannot be empty.");
              return;
          }
  
          const payload = {
              media_id: id,
              file_alt_text: altText,
              FileUrl: img ? img.src : ""
          };
  
          fetch("/MediaManager/UpdateMedia", {
              method: "POST",
              headers: { "Content-Type": "application/json" },
              body: JSON.stringify(payload)
          })
          .then(r => r.json())
          .then(res => {
              if (res.success) {
                  //$("#hdnMediaTempId").val(res.data.id);
                  //$("#hdnImagePreview").val(res.data.fileUrl);
                  alert("Update successfully!");
              } else {
                  alert("Save failed: " + res.message);
              }
          });
  
    });

    // Upload button
    $(document).off("click", "#btnUpload").on("click", "#btnUpload", function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();  // ✅ stop duplicate firing

        
        let fileType = $("#fileType").val();
        let fileInput = document.getElementById("files");
    
        if (!fileType) {
            $("#uploadMessage").html("<span class='text-danger'>Please select folder.</span>");
            return;
        }
    
        if (fileInput.files.length === 0) {
            $("#uploadMessage").html("<span class='text-danger'>Please select files.</span>");
            return;
        }
    
        let formData = new FormData();
        formData.append("__RequestVerificationToken", $('input[name="__RequestVerificationToken"]').val());
        formData.append("fileType", fileType);
        [...fileInput.files].forEach(f => formData.append("files", f));
    
        $.ajax({
            url: "/MediaManager/Upload",
            method: "POST",
            contentType: false,
            processData: false,
            data: formData,
            success: function (res) {
                if (res.success) {
    
                    selectedFiles = [];
                    $("#files").val("");
    
                    $("#dropZone").html(defaultText);
    
                    $("#uploadMessage").html(`<span class='text-success'>${res.message}</span>`);

                    var $target = $("#mediaManagerModalBody");
                    

                    if ($target.length > 0) {
                        $target.load("/MediaManager/IndexPartial");
                    } else {
                        $("#DivMedia").load("/MediaManager/IndexPartial");
                    }

                    //$("#mediaManagerModalBody").load("/MediaManager/IndexPartial");
    
                } else {
                    $("#uploadMessage").html(`<span class='text-danger'>${res.message}</span>`);
                }
            },
            error: function () {
                $("#uploadMessage").html("<span class='text-danger'>Upload failed.</span>");
            }
        });
    });

    

    // ================================================================
    // SELECT IMAGE BUTTON
    // ================================================================
    $(document).off("click", "#btnAddImage").on("click", "#btnAddImage", function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
    
        const tempId = $("#hdnMediaTempId").val();
        const select_mediaId = $("#hdnMediaId").val();
        const fileurl = $("#hdnImagePreview").val();
       
        if (!select_mediaId || select_mediaId === "" || select_mediaId === "0") {
            alert("Please select a file before clicking Add.");
            return;
        }
    
        if (!window.sourceControl) return;
    
        // extension
        const ext = fileurl.split('.').pop().toLowerCase();
    
        // non-image types
        const nonImageTypes = ["pdf", "mp3", "mp4", "zip"];
        const isNonImage = nonImageTypes.includes(ext);
    
        // hidden fields
        $(window.sourceControl).next("input[type='hidden']").val(select_mediaId);
        //$(window.sourceControl).nextAll(".img-value:first").val(fileurl);
    
        const imgPreview = $(window.sourceControl).nextAll(".imgPreview:first");

       
        const filePreview = $(window.sourceControl).nextAll(".filePreview:first");
        const fileLink = filePreview.find(".fileLink");
       
        if (isNonImage) {
            // icon based on file type
            let iconClass = "bi bi-file-earmark";
    
            switch (ext) {
                case "pdf":
                    iconClass = "bi bi-file-earmark-pdf";
                    break;
                case "mp3":
                    iconClass = "bi bi-music-note-beamed";
                    break;
                case "mp4":
                    iconClass = "bi bi-film";
                    break;
                case "zip":
                    iconClass = "bi bi-file-zip";
                    break;
            }
    
            // update icon + link
            fileLink.html(`<i class="${iconClass}" style="font-size:18px;"></i> Open ${ext.toUpperCase()}`);
            fileLink.attr("href", fileurl);
    
            filePreview.show();
            imgPreview.hide();
    
        } else {
            // if it's an image
            filePreview.hide();
            imgPreview.attr("src", fileurl).show();
        }


          // ✅ ADD REMOVE ICON dynamically
            // imgPreview.after(`
            //     <span class="remove-icon">
            //         <i class="bi bi-x-circle-fill text-danger"></i>
            //     </span>
            // `);
    
        // close modal (getOrCreateInstance: hide works even if getInstance is null)
        let modalEl = document.getElementById("btnSelectFiles");
        if (modalEl) {
            let modal = bootstrap.Modal.getInstance(modalEl) || bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.hide();
        }
    
        $("#mediaManagerModalBody").off("click", "#btnAddTemp");
    });
    
    
    

    // ================================================================
    // OPEN MODAL
    // ================================================================

    // $(document).off("click", ".select-media").on("click", ".select-media", function (e) {
    //     e.preventDefault();
    //     e.stopPropagation();
   
    //     window.sourceControl = this;
    
    //     const modalEl = document.getElementById("mediaManagerModal");
    
    //     if (!modalEl) {
    //         console.error("❌ Modal element #mediaManagerModal not found");
    //         return;
    //     }
    
    //     // Get existing instance safely
    //     let modal = bootstrap.Modal.getInstance(modalEl);
    
    //     // If already open, hide first (clean state)
    //     if (modal) {
    //         modal.hide();
    //     }
    
    //     // Clear previous content
    //     $("#mediaManagerModalBody").html("<div class='text-center p-3'>Loading...</div>");
    
    //     // Load partial view
    //     $("#mediaManagerModalBody").load("/MediaManager/IndexPartial", function (response, status, xhr) {
    
    //         if (status === "error") {
    //             console.error("❌ Load failed:", xhr.status, xhr.statusText);
    //             $("#mediaManagerModalBody").html("<div class='text-danger'>Failed to load data</div>");
    //             return;
    //         }
    
    //         // Re-create modal instance AFTER content load
    //         modal = new bootstrap.Modal(modalEl, {
    //             backdrop: true,
    //             keyboard: true
    //         });
    
    //         modal.show();
    //     });
    // });


    

     $(document).off("click", ".select-media").on("click", ".select-media", function (e) {
         e.preventDefault();
         e.stopPropagation();

         window.sourceControl = this;

         // Do not remove all .modal-backdrop or modal-open here — that breaks the parent
         // modal (e.g. spot templates). Bootstrap 5 stacks nested modals correctly.

         // Clear previous content
         $("#mediaManagerModalBody").html("");

      
         // Load partial view
         $("#mediaManagerModalBody").load("/MediaManager/IndexPartial", function () {

             // After loading, show modal
             var modalEl = document.getElementById("btnSelectFiles");
             var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
             modal.show();
         });

     });

    // Clear injected partial (nested crop modal, etc.) so the next open is clean; avoids stuck UI.
    $(document).on("hidden.bs.modal", "#btnSelectFiles", function () {
        $("#mediaManagerModalBody").empty();
    });
    
   
//------------------------------croped-----



// ================= CROP =================

$(document).on("click", "#btnOpenCrop", function () {

    let image = document.getElementById("cropImage1");

    // destroy old cropper
    if (cropper) {
        cropper.destroy();
        cropper = null;
    }

    let imageURL = "";

    if (selectedFiles && selectedFiles.length > 0) {
        imageURL = URL.createObjectURL(selectedFiles[0]);
    } else if (selectedImageUrl) {
        image.crossOrigin = "anonymous";
        imageURL = selectedImageUrl;
    } else {
        alert("No image selected");
        return;
    }

    // set image first
    image.src = imageURL;

    // 🔥 IMPORTANT: wait for modal to fully open
    $("#cropModal").off("shown.bs.modal").on("shown.bs.modal", function () {

        let ratio = NaN;

        if (filetypes === "thumbnail") ratio = 1;
        if (filetypes === "masthead") ratio = 16 / 9;
        if (filetypes === "icon") ratio = 1;

        // cropper = new Cropper(image, {
        //     aspectRatio: ratio,
        //     viewMode: 1,
        //     autoCropArea: 1,
        //     responsive: true,
        //     background: false
        // });

        cropper = new Cropper(image, {
            aspectRatio: ratio,          // 1:1 (square) → change if needed
            viewMode: 1,
            dragMode: 'move',        // move image, not crop box
            cropBoxResizable: true, // ❌ disable resize
            cropBoxMovable: true,   // ❌ disable moving box
            scalable: false,
            zoomable: true,
            responsive: true
        });

    }).modal("show");
});






// ================= UPLOAD CROPPED =================
function uploadCropped(blob) {

    let fileType = $("#fileType").val();
    let mediaId = $("#hdnMediaId").val();

    let formData = new FormData();
    formData.append("file", blob);
    formData.append("fileType", fileType);
    formData.append("mediaId", mediaId);
    formData.append("__RequestVerificationToken",
        $('input[name="__RequestVerificationToken"]').val());

    let url = mediaId && mediaId !== "0"
        ? "/MediaManager/ReplaceMedia"
        : "/MediaManager/UploadCropped";

    $.ajax({
        url: url,
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {

            if (res.success) {

                $("#uploadMessage").html("<span class='text-success'>Saved successfully</span>");

                $("#mediaManagerModalBody").load("/MediaManager/IndexPartial");

                if (cropper) {
                    cropper.destroy();
                    cropper = null;
                }

            } else {
                $("#uploadMessage").html("<span class='text-danger'>Save failed</span>");
            }
        },
        error: function () {
            $("#uploadMessage").html("<span class='text-danger'>Error</span>");
        }
    });
}

$(document).off("click", "#btnCrop").on("click", "#btnCrop", function () {

    if (!cropper) {
        alert("Cropper not initialized");
        return;
    }

    let canvas = cropper.getCroppedCanvas({
        width: 800,
        height: 600
    });

    if (!canvas) {
        alert("Canvas not created");
        return;
    }

    let croppedImage = canvas.toDataURL("image/jpeg", 0.8);

    // preview
    $("#previewArea").html(`<img src="${croppedImage}" class="img-fluid"/>`);

    // update selected card
    let selectedCard = $(".media-card.active");
    if (selectedCard.length > 0) {
        selectedCard.find("img").attr("src", croppedImage);
    }

    canvas.toBlob(function (blob) {

        if (!blob) {
            alert("Blob creation failed");
            return;
        }

        let formData = new FormData();
        formData.append("file", blob, "cropped.jpg"); // ✅ VERY IMPORTANT
        formData.append("mediaId", $("#hdnMediaId").val());

        $.ajax({
            url: "/Media/ReplaceImage", // ✅ matches controller route
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,

            success: function (res) {
                if (res.success) {
                    alert("Image replaced successfully");
                    window.location.href = res.redirectUrl;
                } else {
                    alert(res.message);
                }
            },

            error: function (err) {
                console.log(err);
                alert("AJAX failed");
            }
        });

    }, "image/jpeg", 0.8);

    $("#cropModal").modal("hide");
});



$(document).on("click", "#zoomIn", function () {
    if (!cropper) return alert("Cropper not ready");
    cropper.zoom(0.1);
});

$(document).on("click", "#zoomOut", function () {
    if (!cropper) return alert("Cropper not ready");
    cropper.zoom(-0.1);
});

$(document).on("click", "#rotateLeft", function () {
    if (!cropper) return alert("Cropper not ready");
    cropper.rotate(-45);
});

$(document).on("click", "#rotateRight", function () {
    if (!cropper) return alert("Cropper not ready");
    cropper.rotate(45);
});

$(document).on("click", "#resetCrop", function () {
    if (!cropper) return alert("Cropper not ready");
    cropper.reset();
});


$(document).on("click", ".btn-delete-media", function (e) {

    e.preventDefault();
    e.stopImmediatePropagation();

    let button = $(this);
    let id = button.data("id");

    if (!id) {
        alert("Invalid file");
        return;
    }

    if (!confirm("Are you sure you want to move this file to Deleted folder?")) {
        return;
    }

    $.ajax({
        url: "/MediaManager/DeleteMedia", // 👈 match controller
        type: "POST",
        data: {
            id: id,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },

        beforeSend: function () {
            button.text("Deleting...");
            button.prop("disabled", true);
        },

        success: function (res) {

            if (res.success) {

                // ✅ remove card instantly (better UX)
                button.closest(".col-md-3").fadeOut(300, function () {
                    $(this).remove();
                });

                // OR reload full list (your existing pattern)
                // $("#mediaManagerModalBody").load("/MediaManager/IndexPartial");

            } else {
                alert(res.message);
                button.text("Delete").prop("disabled", false);
            }
        },

        error: function () {
            alert("Delete failed");
            button.text("Delete").prop("disabled", false);
        }
    });
});


$(document).on("click", "#btnCopyUrl", function () {

    const input = document.getElementById("txtFileUrl");

    if (!input.value) {
        alert("No file URL available");
        return;
    }

    navigator.clipboard.writeText(input.value)
        .then(() => {
            let btn = $("#btnCopyUrl");
            btn.text("Copied!");

            setTimeout(() => {
                btn.text("Copy");
            }, 1500);
        })
        .catch(() => {
            input.select();
            document.execCommand("copy");
            alert("Copied to clipboard");
        });
});


$(document).off("click", "#btnLoadMore").on("click", "#btnLoadMore", function () {

    let btn = $(this);
    let page = parseInt($("#currentPage").val()) + 1;

    let sortOrder = $("select[name='sortOrder']").val();
    let fileType = $("select[name='fileType']").val();
    let search = $("input[name='search']").val();

    btn.text("Loading...").prop("disabled", true);

    $.ajax({
        url: "/MediaManager/LoadMoreMedia", // ✅ NEW ACTION
        type: "GET",
        data: {
            page: page,
            sortOrder: sortOrder,
            fileType: fileType,
            search: search
        },
        success: function (res) {

            // remove old button
            $("#btnLoadMore").remove();

            // append new items ONLY
            $(".row.g-3").append(res);

            // update page
            $("#currentPage").val(page);
        },
        error: function () {
            alert("Load failed");
            btn.text("Load More").prop("disabled", false);
        }
    });
});





    $(document).on("click", ".remove-icon, .remove-icon *", function (e) {
        e.preventDefault();

        var btn = $(this).closest(".remove-icon");
        var formGroup = btn.closest(".form-group");

        // Hide image
        formGroup.find(".imgPreview").attr("src", "").hide();

        // Hide remove icon
        btn.hide();

        // Reset hidden field
        var hiddenInput = formGroup.find(".media-id");

        if (hiddenInput.length === 0) {
            hiddenInput = formGroup.find('input[type="hidden"]').first();
        }

        hiddenInput.val("0");
    });



    $(document).off("click", "#btnSearchMedia").on("click", "#btnSearchMedia", function (e) {
        e.preventDefault();
    
        $.ajax({
            url: "/MediaManager/SearchMedia",
            type: "GET",
            data: {
                sortOrder: $("#sortOrder").val(),
                fileType: $("#searchFileType").val(),
                search: $("#searchText").val(),
                page: 1
            },
            beforeSend: function () {
                $(".row.g-3").html("<div class='text-center p-3'>Loading...</div>");
            },
            success: function (html) {
                
                // remove old button
                $("#btnLoadMore").remove();

                // append new items ONLY
                $(".row.g-3").empty();
                $(".row.g-3").append(html);

                // update page
                $("#currentPage").val(1);
    
            },
            error: function () {
                alert("Search failed.");
            }
        });
    
    });






})();
