
//Dropzone.autoDiscover = false;

//var myDropzone = new Dropzone("#my-dropzone", {
//    url: "/image/UploadFile",
//    paramName: "file",  // This should match the parameter in your controller
//    autoProcessQueue: false,
//    addRemoveLinks: true
//});
//$(document).ready(function() {
//$("#submitButton").click(function () {

//    alert('hi');
//    myDropzone.processQueue();  // Trigger the file upload
//});
//});



function appendStaticFormFields(formData, form) {
    form.querySelectorAll("input, select, textarea").forEach(function (el) {
        if (!el.name || el.disabled) {
            return;
        }
        if (el.name.indexOf("FilesList") === 0) {
            return;
        }
        if (el.type === "file" || el.type === "button" || el.type === "submit") {
            return;
        }
        if ((el.type === "checkbox" || el.type === "radio") && !el.checked) {
            return;
        }
        if (el.tagName === "SELECT" && el.multiple) {
            Array.from(el.selectedOptions).forEach(function (opt) {
                formData.append(el.name, opt.value);
            });
            return;
        }
        formData.append(el.name, el.value);
    });
}

Dropzone.options.myDropzone = {
    url: '/ManageFiles/Upload',
    uploadMultiple: true,
    // A function keeps the field name as "files". A string becomes files[0], files[1],
    // which ASP.NET Core does not bind to List<IFormFile> files.
    paramName: function () { return "files"; },
    autoProcessQueue: false,        // Prevent auto upload, we'll trigger manually
    addRemoveLinks: true,           // Optional: allows file removal from Dropzone UI
    maxFilesize: 5000,                 // Max file size in MB
    maxFiles: 25,
    parallelUploads: 25,
    timeout: 36000000,
    // acceptedFiles: 'image/*',       // Accepted file types
    acceptedFiles: ".pdf",
    init: function () {
        var myDropzone = this;
        if (this.element && this.element.getAttribute("action")) {
            this.options.url = this.element.getAttribute("action");
        }

        // Names must match SelectedFiles so UploadFiles.FilesList binds.
        // FilesList[i] is the same file as List<IFormFile> files[i].
        this.on("addedfile", function (file) {
            function addInput(type, placeholder) {
                var input = document.createElement("input");
                input.type = type;
                input.className = "form-control";
                input.placeholder = placeholder + " " + file.name;
                file.previewElement.appendChild(input);
                return input;
            }

            file._meta = {
                title: addInput("text", "Enter title"),
                pagename: addInput("text", "Enter pagename"),
                sequence: addInput("number", "Enter sequence"),
                date: addInput("date", "Enter date for")
            };
        });

        this.on("sendingmultiple", function (files, xhr, formData) {
            files.forEach(function (file, index) {
                if (!file._meta) {
                    return;
                }
                formData.append("FilesList.Index", String(index));
                formData.append("FilesList[" + index + "].Title", file._meta.title.value);
                formData.append("FilesList[" + index + "].Filepagename", file._meta.pagename.value);
                formData.append("FilesList[" + index + "].Sequence", file._meta.sequence.value || "0");
                if (file._meta.date.value) {
                    formData.append("FilesList[" + index + "].Filedate", file._meta.date.value);
                }
            });
            appendStaticFormFields(formData, this.element);
        });

        document.getElementById("uploadButton").addEventListener("click", function (e) {
            e.preventDefault();
            if (myDropzone.getQueuedFiles().length > 0) {
                myDropzone.processQueue();
            } else {
                alert("Please select a file to upload.");
            }
        });

        this.on("complete", function (file) {
            if (myDropzone.getQueuedFiles().length > 0 || myDropzone.getUploadingFiles().length > 0) {
                return;
            }

            var response = {};
            try {
                var raw = file.xhr && file.xhr.response ? file.xhr.response : "";
                response = raw ? JSON.parse(raw) : {};
            } catch (ex) {
                response = { success: false, message: "File upload failed." };
            }

            alert(response.message || "File upload failed.");
            if (response.success) {
                window.location.href = "/Content/Drafts";
            }
        });

        // Optional: Handle error for failed file upload
        this.on("error", function (file, response) {
            console.error("Error uploading file", file, response);
        });

        this.on("success", function (file, response) {
            console.log("Success:", file.name, response);
        });
        this.on("error", function (file, errorMessage) {
            console.error("Upload error:", file.name, errorMessage);
        });
        this.on("timeout", function (file) {
            console.error("Timeout on file:", file.name);
        });





    }
};

 

