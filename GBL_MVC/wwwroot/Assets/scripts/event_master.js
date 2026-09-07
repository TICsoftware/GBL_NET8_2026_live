window.editors = {};

function syncCkEditorFields() {
    if (window.editors?.["Intro"]) {
        document.getElementById("Intro").value =
            window.editors["Intro"].getData();
    }

    if (window.editors?.["Content"]) {
        document.getElementById("Content").value =
            window.editors["Content"].getData();
    }
}

function getCkEditorText(id) {
    if (window.editors?.[id]) {
        return window.editors[id].getData()
            .replace(/<[^>]*>/g, "")
            .replace(/&nbsp;/g, " ")
            .trim();
    }

    let el = document.getElementById(id);
    return el ? el.value.replace(/<[^>]*>/g, "").trim() : "";
}

const EVENT_TEXTBOX_REGEX = /^[^~<>|/\\!@#]*$/;
const EVENT_EDITOR_FORBIDDEN_REGEX = /[~!^]/;
const EVENT_TEXTBOX_REGEX_MESSAGE =
    "Special characters ~ < > | / \\ ! @ # are not allowed.";
const EVENT_EDITOR_REGEX_MESSAGE =
    "Characters ~, !, and ^ are not allowed.";

function initEventFormValidation() {
    let $form = $("#eventForm");
    if (!$form.length || $form.data("validator")) {
        return;
    }

    $.validator.addMethod("ckeditorRequired", function (_value, element) {
        return getCkEditorText(element.id).length > 0;
    }, "Intro is required.");

    $.validator.addMethod("noTextboxSpecial", function (value, element) {
        if (this.optional(element)) {
            return true;
        }
        return EVENT_TEXTBOX_REGEX.test(value);
    }, EVENT_TEXTBOX_REGEX_MESSAGE);

    $.validator.addMethod("noEditorSpecial", function (_value, element) {
        let text = getCkEditorText(element.id);
        if (!text) {
            return true;
        }
        return !EVENT_EDITOR_FORBIDDEN_REGEX.test(text);
    }, EVENT_EDITOR_REGEX_MESSAGE);

    $form.validate({
        ignore: [],
        rules: {
            Title: {
                required: true,
                minlength: 2,
                maxlength: 200,
                noTextboxSpecial: true
            },
            EventTypeId: {
                required: true
            },
            EventModeId: {
                required: true
            },
            EventDate: {
                required: true
            },
            EventTime: {
                maxlength: 50,
                noTextboxSpecial: true
            },
            Speakers: {
                maxlength: 1000,
                noTextboxSpecial: true
            },
            Intro: {
                ckeditorRequired: true,
                noEditorSpecial: true
            },
            Content: {
                noEditorSpecial: true
            },
            VideoUrl: {
                url: true,
                maxlength: 500
            }
        },
        messages: {
            Title: {
                required: "Title is required.",
                minlength: "Title must be at least 2 characters.",
                maxlength: "Title cannot exceed 200 characters.",
                noTextboxSpecial: EVENT_TEXTBOX_REGEX_MESSAGE
            },
            EventTypeId: {
                required: "Event type is required."
            },
            EventModeId: {
                required: "Event mode is required."
            },
            EventDate: {
                required: "Event date is required."
            },
            EventTime: {
                maxlength: "Time cannot exceed 50 characters.",
                noTextboxSpecial: EVENT_TEXTBOX_REGEX_MESSAGE
            },
            Speakers: {
                maxlength: "Speakers cannot exceed 1000 characters.",
                noTextboxSpecial: EVENT_TEXTBOX_REGEX_MESSAGE
            },
            Intro: {
                noEditorSpecial: EVENT_EDITOR_REGEX_MESSAGE
            },
            Content: {
                noEditorSpecial: EVENT_EDITOR_REGEX_MESSAGE
            },
            VideoUrl: {
                url: "Please enter a valid video URL.",
                maxlength: "Video URL cannot exceed 500 characters."
            }
        },
        errorClass: "text-danger",
        errorElement: "span",
        highlight: function (element) {
            $(element).addClass("is-invalid");
        },
        unhighlight: function (element) {
            $(element).removeClass("is-invalid");
        }
    });
}

function clearForm() {
    let eventId = document.getElementById("EventId");
    if (eventId) eventId.value = "";

    let title = document.getElementById("Title");
    if (title) title.value = "";

    let type = document.getElementById("EventTypeId");
    if (type) type.value = "";

    let mode = document.getElementById("EventModeId");
    if (mode) mode.value = "";

    let date = document.getElementById("EventDate");
    if (date) date.value = "";

    let time = document.getElementById("EventTime");
    if (time) time.value = "";

    let speakers = document.getElementById("Speakers");
    if (speakers) speakers.value = "";

    let video = document.getElementById("VideoUrl");
    if (video) video.value = "";

    let file = document.getElementById("FilePath");
    if (file) file.value = "";

    let filePreview = document.getElementById("filePreview");
    if (filePreview) filePreview.classList.add("d-none");

    if (window.editors?.["Intro"])
        window.editors["Intro"].setData("");

    if (window.editors?.["Content"])
        window.editors["Content"].setData("");
}

function openModal() {
    clearForm();

    let modal = document.getElementById("eventModal");
    if (!modal) return;

    window.initCkEditors?.(modal);

    let $form = $("#eventForm");
    if ($form.data("validator")) {
        $form.validate().resetForm();
        $form.find(".is-invalid").removeClass("is-invalid");
    }

    new bootstrap.Modal(modal).show();
}

function changeStatus(id, status) {
    let msg =
        status == 1
            ? "Activate this event?"
            : "Deactivate this event?";

    if (!confirm(msg))
        return;

    fetch("/EventMaster/ChangeStatus", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            id: id,
            status: status
        })
    })
        .then(res => res.text())
        .then(() => {
            location.reload();
        })
        .catch(err => {
            console.log(err);
            alert("Status update failed");
        });
}

function toggleRecurring() {
    const section = document.getElementById("recurringSection");
    const checkbox = document.getElementById("IsRecurring");
    if (!section || !checkbox) return;

    section.classList.toggle("d-none", !checkbox.checked);
}

function initFileUpload() {
    const btnFile = document.getElementById("btnFile");
    const fileInput = document.getElementById("fileInput");
    const removeFile = document.getElementById("removeFile");

    btnFile?.addEventListener("click", function () {
        fileInput?.click();
    });

    fileInput?.addEventListener("change", function () {
        let file = this.files[0];
        if (!file) return;

        const maxSize = 5 * 1024 * 1024;
        if (file.size > maxSize) {
            alert("File size must not exceed 5MB.");
            this.value = "";
            return;
        }

        let formData = new FormData();
        formData.append("file", file);

        fetch("/EventMaster/UploadFile", {
            method: "POST",
            body: formData
        })
            .then(res => res.text())
            .then(path => {
                document.getElementById("FilePath").value = path;
                const filePreview = document.getElementById("filePreview");
                const fileLink = document.getElementById("fileLink");
                filePreview?.classList.remove("d-none");
                if (fileLink) fileLink.href = path;
            });
    });

    removeFile?.addEventListener("click", function () {
        document.getElementById("FilePath").value = "";
        if (fileInput) fileInput.value = "";
        document.getElementById("filePreview")?.classList.add("d-none");
    });
}

function initAutoOpenModal() {
    const page = document.getElementById("eventMasterPage");
    if (!page || page.getAttribute("data-auto-open-modal") !== "true") {
        return;
    }

    const modal = document.getElementById("eventModal");
    if (!modal) return;

    window.initCkEditors?.(modal);
    new bootstrap.Modal(modal).show();
}

$(function () {
    initEventFormValidation();
    initFileUpload();
    initAutoOpenModal();
    toggleRecurring();

    $("#eventForm").on("submit", function (e) {
        syncCkEditorFields();

        if (!$(this).valid()) {
            e.preventDefault();
        }
    });

    $("#IsRecurring").on("change", toggleRecurring);

    document.querySelector(".js-open-modal")?.addEventListener("click", openModal);

    document.addEventListener("click", function (e) {
        const btn = e.target.closest(".js-change-status");
        if (!btn) return;

        const id = parseInt(btn.dataset.id, 10);
        const status = parseInt(btn.dataset.status, 10);
        changeStatus(id, status);
    });
});

function edit(id) {
    fetch("/EventMaster/GetById?id=" + id)
        .then(res => res.json())
        .then(data => {
            document.getElementById("EventId").value = data.eventId || "";
            document.getElementById("Title").value = data.title || "";
            document.getElementById("EventTypeId").value =
                data.eventTypeId ? data.eventTypeId.toString() : "";
            document.getElementById("EventModeId").value =
                data.eventModeId ? data.eventModeId.toString() : "";
            document.getElementById("EventDate").value =
                data.eventDate ? data.eventDate.split("T")[0] : "";
            document.getElementById("EventTime").value = data.eventTime || "";
            document.getElementById("Speakers").value = data.speakers || "";
            document.getElementById("VideoUrl").value = data.videoUrl || "";
            document.getElementById("FilePath").value = data.filePath || "";

            if (data.filePath) {
                document.getElementById("filePreview")?.classList.remove("d-none");
                const fileLink = document.getElementById("fileLink");
                if (fileLink) fileLink.href = data.filePath;
            }

            if (window.editors?.["Intro"])
                window.editors["Intro"].setData(data.intro || "");

            if (window.editors?.["Content"])
                window.editors["Content"].setData(data.content || "");

            new bootstrap.Modal(document.getElementById("eventModal")).show();
        });
}
