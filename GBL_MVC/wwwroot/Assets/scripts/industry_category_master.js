const ICM_TEXTBOX_REGEX = /^[^~<>|/\\!@#]*$/;
const ICM_TEXTBOX_MESSAGE =
    "Special characters ~ < > | / \\ ! @ # are not allowed.";

function validateMasterName(name) {
    const value = (name || "").trim();
    if (!value) return "Name is required.";
    if (value.length < 2) return "Name must be at least 2 characters.";
    if (value.length > 500) return "Name cannot exceed 500 characters.";
    if (!ICM_TEXTBOX_REGEX.test(value)) return ICM_TEXTBOX_MESSAGE;
    return "";
}

function validateSequence(seq) {
    if (seq === "" || seq === null || seq === undefined) {
        return "Display order is required.";
    }
    const value = parseInt(seq, 10);
    if (Number.isNaN(value) || value < 1 || value > 9999) {
        return "Display order must be between 1 and 9999.";
    }
    return "";
}

function setFieldError(input, errorElement, message) {
    if (!input) return false;
    if (message) {
        input.classList.add("is-invalid");
        if (errorElement) {
            errorElement.textContent = message;
            errorElement.classList.remove("d-none");
        }
        return false;
    }
    input.classList.remove("is-invalid");
    if (errorElement) {
        errorElement.textContent = "";
        errorElement.classList.add("d-none");
    }
    return true;
}

function isIndustryType(masterType) {
    return (masterType || "").toLowerCase() === "industry";
}

async function postMaster(url, payload) {
    const response = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
    });

    if (response.ok) {
        location.reload();
        return;
    }

    let message = "Request failed.";
    try {
        const data = await response.json();
        if (data?.message) message = data.message;
    } catch {
        // keep default
    }
    alert(message);
}

function pickValue(data, ...keys) {
    if (!data) return "";
    for (const key of keys) {
        if (data[key] != null && data[key] !== "") {
            return data[key];
        }
    }
    const map = {};
    Object.keys(data).forEach((k) => {
        map[k.toLowerCase()] = data[k];
    });
    for (const key of keys) {
        const v = map[key.toLowerCase()];
        if (v != null && v !== "") {
            return v;
        }
    }
    return "";
}

function setImagePreview(imgEl, url) {
    if (!imgEl) return;
    const clean = (url || "").toString().trim();
    if (clean) {
        imgEl.setAttribute("src", clean);
        imgEl.classList.add("is-visible");
        imgEl.style.setProperty("display", "inline-block", "important");
    } else {
        imgEl.removeAttribute("src");
        imgEl.classList.remove("is-visible");
        imgEl.style.setProperty("display", "none", "important");
    }
}

function applyMediaToSourceControl(source, mediaId, fileUrl) {
    if (!source) return;

    const $btn = $(source);
    const hiddenSel = $btn.data("media-hidden");
    const previewSel = $btn.data("media-preview");

    let $hidden = hiddenSel ? $(hiddenSel) : $btn.nextAll("input[type='hidden']").first();
    if (!$hidden.length) {
        $hidden = $btn.next("input[type='hidden']");
    }
    if ($hidden.length && mediaId) {
        $hidden.val(mediaId);
    }

    let $img = previewSel ? $(previewSel) : $btn.nextAll("img.imgPreview").first();
    if (!$img.length) {
        $img = $btn.parent().find("img.imgPreview").first();
    }
    if ($img.length && fileUrl) {
        setImagePreview($img.get(0), fileUrl);
    }
}

/** Bind selected Media Manager image into the Industry preview fields */
function bindIndustryMediaPreviewFix() {
    // Capture phase runs before media-manager's stopImmediatePropagation,
    // so we can always apply id + preview to the Industry fields.
    document.addEventListener(
        "click",
        function (e) {
            const addBtn = e.target && e.target.closest ? e.target.closest("#btnAddImage") : null;
            if (!addBtn) return;

            const mediaId = document.getElementById("hdnMediaId")?.value;
            const fileUrl = document.getElementById("hdnImagePreview")?.value;
            const source = window.sourceControl;

            applyMediaToSourceControl(source, mediaId, fileUrl);
            setTimeout(function () {
                applyMediaToSourceControl(source, mediaId, fileUrl);
            }, 100);
        },
        true
    );
}

function parseOptionalInt(value) {
    if (value === "" || value === null || value === undefined) return null;
    const n = parseInt(value, 10);
    return Number.isNaN(n) ? null : n;
}

function setEditorData(textareaId, html) {
    const el = document.getElementById(textareaId);
    if (!el) return;
    if (el.editorInstance) {
        el.editorInstance.setData(html || "");
    } else {
        el.value = html || "";
    }
}

function resetIndustryForm() {
    document.getElementById("industryId").value = "0";
    document.getElementById("industryName").value = "";
    document.getElementById("industryPageName").value = "";
    document.getElementById("industryLanguageId").value = "";
    document.getElementById("industrySeq").value = "";
    document.getElementById("industryBannerMediaId").value = "";
    document.getElementById("industryThumbMediaId").value = "";
    document.getElementById("industryBannerImageAlt").value = "";
    document.getElementById("industryThumbnailImageAlt").value = "";
    document.getElementById("industryWindowTitle").value = "";
    document.getElementById("industryMetaTitle").value = "";
    document.getElementById("industryMetaDescription").value = "";
    setImagePreview(document.getElementById("industryBannerPreview"), "");
    setImagePreview(document.getElementById("industryThumbPreview"), "");
    setEditorData("industryIntro", "");
    setEditorData("industryContent", "");
    setFieldError(document.getElementById("industryName"), document.getElementById("industryNameError"), "");
    setFieldError(document.getElementById("industrySeq"), document.getElementById("industrySeqError"), "");
}

function validateIndustryForm() {
    const nameValid = setFieldError(
        document.getElementById("industryName"),
        document.getElementById("industryNameError"),
        validateMasterName(document.getElementById("industryName")?.value)
    );
    const seqValid = setFieldError(
        document.getElementById("industrySeq"),
        document.getElementById("industrySeqError"),
        validateSequence(document.getElementById("industrySeq")?.value)
    );
    return nameValid && seqValid;
}

function buildIndustryPayload() {
    const form = document.getElementById("industryMasterForm");
    if (typeof window.syncCkEditorsToSource === "function") {
        window.syncCkEditorsToSource(form);
    }

    const id = parseInt(document.getElementById("industryId").value || "0", 10);
    return {
        ID: id,
        Name: document.getElementById("industryName").value.trim(),
        PageName: (document.getElementById("industryPageName").value || "").trim(),
        Sequence: parseInt(document.getElementById("industrySeq").value, 10),
        Language_Master_Id: parseOptionalInt(document.getElementById("industryLanguageId").value),
        Banner_Image_media_id: parseOptionalInt(document.getElementById("industryBannerMediaId").value),
        Landing_Thumbnail_Image_media_id: parseOptionalInt(document.getElementById("industryThumbMediaId").value),
        Banner_Image_Alt: (document.getElementById("industryBannerImageAlt").value || "").trim(),
        Landing_Thumbnail_Image_Alt: (document.getElementById("industryThumbnailImageAlt").value || "").trim(),
        Window_Title: (document.getElementById("industryWindowTitle").value || "").trim(),
        Meta_Title: (document.getElementById("industryMetaTitle").value || "").trim(),
        Meta_Description: (document.getElementById("industryMetaDescription").value || "").trim(),
        Intro: document.getElementById("industryIntro").value || "",
        Content: document.getElementById("industryContent").value || "",
        Status: 1,
        MasterType: "Industry"
    };
}

async function openIndustryModal(editId) {
    const modalEl = document.getElementById("industryModal");
    const titleEl = document.getElementById("industryModalTitle");
    resetIndustryForm();

    if (editId) {
        titleEl.textContent = "Edit Industry";
        const res = await fetch(`/Industry_Category_master/GetById?id=${editId}&type=Industry`);
        if (!res.ok) {
            alert("Failed to load industry.");
            return;
        }
        const data = await res.json();
        document.getElementById("industryId").value = pickValue(data, "id", "ID") || editId;
        document.getElementById("industryName").value = pickValue(data, "name", "Name");
        document.getElementById("industryPageName").value = pickValue(data, "pageName", "PageName");
        document.getElementById("industryLanguageId").value = pickValue(data, "language_Master_Id", "Language_Master_Id");
        document.getElementById("industrySeq").value = pickValue(data, "sequence", "Sequence");
        document.getElementById("industryBannerMediaId").value = pickValue(
            data,
            "banner_Image_media_id",
            "Banner_Image_media_id"
        );
        document.getElementById("industryThumbMediaId").value = pickValue(
            data,
            "landing_Thumbnail_Image_media_id",
            "Landing_Thumbnail_Image_media_id"
        );
        document.getElementById("industryBannerImageAlt").value = pickValue(
            data,
            "banner_Image_Alt",
            "Banner_Image_Alt"
        );
        document.getElementById("industryThumbnailImageAlt").value = pickValue(
            data,
            "landing_Thumbnail_Image_Alt",
            "Landing_Thumbnail_Image_Alt"
        );
        document.getElementById("industryWindowTitle").value = pickValue(
            data,
            "window_Title",
            "Window_Title"
        );
        document.getElementById("industryMetaTitle").value = pickValue(
            data,
            "meta_Title",
            "Meta_Title"
        );
        document.getElementById("industryMetaDescription").value = pickValue(
            data,
            "meta_Description",
            "Meta_Description"
        );
        setImagePreview(
            document.getElementById("industryBannerPreview"),
            pickValue(data, "banner_Image_Url", "Banner_Image_Url", "banner_image_url")
        );
        setImagePreview(
            document.getElementById("industryThumbPreview"),
            pickValue(data, "landing_Thumbnail_Image_Url", "Landing_Thumbnail_Image_Url", "landing_thumbnail_image_url")
        );

        // Init editors first, then set data after a short delay if needed
        if (typeof window.initCkEditors === "function") {
            window.initCkEditors(modalEl);
        }
        setTimeout(() => {
            setEditorData("industryIntro", pickValue(data, "intro", "Intro"));
            setEditorData("industryContent", pickValue(data, "content", "Content"));
        }, 300);
    } else {
        titleEl.textContent = "Add Industry";
        if (typeof window.initCkEditors === "function") {
            window.initCkEditors(modalEl);
        }
    }

    bootstrap.Modal.getOrCreateInstance(modalEl).show();
}

document.addEventListener("DOMContentLoaded", function () {
    const wrapper = document.querySelector("[data-master-type]");
    const masterType = wrapper ? wrapper.getAttribute("data-master-type") : null;

    if (!masterType) {
        alert("MasterType is missing.");
        return;
    }

    const industryMode = isIndustryType(masterType);
    if (industryMode) {
        bindIndustryMediaPreviewFix();
    }

    document.querySelector(".js-open-modal")?.addEventListener("click", function () {
        if (industryMode) {
            openIndustryModal(null);
            return;
        }

        document.getElementById("addMasterForm")?.reset();
        setFieldError(document.getElementById("newName"), document.getElementById("newNameError"), "");
        setFieldError(document.getElementById("newSeq"), document.getElementById("newSeqError"), "");
        bootstrap.Modal.getOrCreateInstance(document.getElementById("addModal")).show();
    });

    document.querySelector(".js-save-new")?.addEventListener("click", function () {
        const nameInput = document.getElementById("newName");
        const seqInput = document.getElementById("newSeq");
        const nameValid = setFieldError(nameInput, document.getElementById("newNameError"), validateMasterName(nameInput?.value));
        const seqValid = setFieldError(seqInput, document.getElementById("newSeqError"), validateSequence(seqInput?.value));
        if (!nameValid || !seqValid) return;

        postMaster("/Industry_Category_master/AddAjax", {
            Name: nameInput.value.trim(),
            Sequence: parseInt(seqInput.value, 10),
            Language_Master_Id: parseOptionalInt(document.getElementById("newLanguageId")?.value),
            Status: 1,
            MasterType: masterType
        });
    });

    document.querySelector(".js-save-industry")?.addEventListener("click", function () {
        if (!validateIndustryForm()) return;
        const payload = buildIndustryPayload();
        const url = payload.ID > 0
            ? "/Industry_Category_master/UpdateAjax"
            : "/Industry_Category_master/AddAjax";
        postMaster(url, payload);
    });

    document.addEventListener("click", function (e) {
        if (e.target.classList.contains("industry-edit-btn")) {
            openIndustryModal(e.target.dataset.id);
        }

        if (e.target.classList.contains("edit-btn") && !industryMode) {
            const id = e.target.dataset.id;
            document.getElementById("lbl_" + id)?.classList.add("d-none");
            document.getElementById("txt_" + id)?.classList.remove("d-none");
            document.getElementById("lblseq_" + id)?.classList.add("d-none");
            document.getElementById("txtseq_" + id)?.classList.remove("d-none");
            e.target.classList.add("d-none");
            document.querySelector(".save-btn[data-id='" + id + "']")?.classList.remove("d-none");
        }

        if (e.target.classList.contains("save-btn") && !industryMode) {
            const id = e.target.dataset.id;
            const nameInput = document.getElementById("txt_" + id);
            const seqInput = document.getElementById("txtseq_" + id);
            const nameMessage = validateMasterName(nameInput?.value);
            const seqMessage = validateSequence(seqInput?.value);
            if (nameMessage) {
                alert(nameMessage);
                return;
            }
            if (seqMessage) {
                alert(seqMessage);
                return;
            }

            postMaster("/Industry_Category_master/UpdateAjax", {
                ID: parseInt(id, 10),
                Name: nameInput.value.trim(),
                Sequence: parseInt(seqInput.value, 10),
                MasterType: masterType
            });
        }

        if (e.target.classList.contains("deactivate-btn") || e.target.classList.contains("activate-btn")) {
            const id = e.target.dataset.id;
            const status = e.target.classList.contains("activate-btn") ? 1 : 0;
            const msg = status === 1 ? "Activate this record?" : "Are you sure you want to deactivate?";
            if (!confirm(msg)) return;

            fetch("/Industry_Category_master/ChangeStatus", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Id: parseInt(id, 10), Status: status, Type: masterType })
            }).then(async (res) => {
                if (res.ok) location.reload();
                else {
                    let message = "Status update failed.";
                    try {
                        const data = await res.json();
                        if (data?.message) message = data.message;
                    } catch { /* ignore */ }
                    alert(message);
                }
            });
        }

        if (e.target.classList.contains("delete-btn")) {
            const id = e.target.dataset.id;
            if (!confirm("Permanently delete this record?")) return;
            fetch("/Industry_Category_master/Delete", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Id: parseInt(id, 10), Type: masterType })
            }).then(async (res) => {
                if (res.ok) location.reload();
                else {
                    let message = "Delete failed.";
                    try {
                        const data = await res.json();
                        if (data?.message) message = data.message;
                    } catch { /* ignore */ }
                    alert(message);
                }
            });
        }
    });

    const sortableElement = document.getElementById("sortable");
    if (sortableElement) {
        new Sortable(sortableElement, {
            animation: 150,
            handle: ".drag-handle",
            onEnd: function () {
                const order = [];
                document.querySelectorAll("#sortable tr[data-id]").forEach((row, index) => {
                    order.push({
                        ID: parseInt(row.dataset.id, 10),
                        Sequence: index + 1,
                        MasterType: masterType
                    });
                });

                fetch("/Industry_Category_master/UpdateSequence", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(order)
                }).then(() => {
                    document.querySelectorAll("#sortable tr[data-id]").forEach((row, index) => {
                        const id = row.dataset.id;
                        const lbl = document.getElementById("lblseq_" + id);
                        const txt = document.getElementById("txtseq_" + id);
                        if (lbl) lbl.textContent = String(index + 1);
                        if (txt) txt.value = String(index + 1);
                    });
                });
            }
        });
    }
});
