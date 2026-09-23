const PKG_TEXTBOX_REGEX = /^[^~<>|/\\!@#]*$/;
const PKG_TEXTBOX_MESSAGE =
    "Special characters ~ < > | / \\ ! @ # are not allowed.";

function validateMasterName(name) {
    const value = (name || "").trim();
    if (!value) return "Name is required.";
    if (value.length < 2) return "Name must be at least 2 characters.";
    if (value.length > 250) return "Name cannot exceed 250 characters.";
    if (!PKG_TEXTBOX_REGEX.test(value)) return PKG_TEXTBOX_MESSAGE;
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
    if (!$hidden.length) $hidden = $btn.next("input[type='hidden']");
    if ($hidden.length && mediaId) $hidden.val(mediaId);

    let $img = previewSel ? $(previewSel) : $btn.nextAll("img.imgPreview").first();
    if (!$img.length) $img = $btn.parent().find("img.imgPreview").first();
    if ($img.length && fileUrl) setImagePreview($img.get(0), fileUrl);
}

function bindPackagingMediaPreviewFix() {
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

function resetPackagingForm() {
    document.getElementById("packagingId").value = "0";
    document.getElementById("packagingName").value = "";
    document.getElementById("packagingLanguageId").value = "";
    document.getElementById("packagingSeq").value = "";
    document.getElementById("packagingThumbMediaId").value = "";
    document.getElementById("packagingThumbAlt").value = "";
    setImagePreview(document.getElementById("packagingThumbPreview"), "");
    setFieldError(document.getElementById("packagingName"), document.getElementById("packagingNameError"), "");
    setFieldError(document.getElementById("packagingSeq"), document.getElementById("packagingSeqError"), "");
}

function validatePackagingForm() {
    const nameValid = setFieldError(
        document.getElementById("packagingName"),
        document.getElementById("packagingNameError"),
        validateMasterName(document.getElementById("packagingName")?.value)
    );
    const seqValid = setFieldError(
        document.getElementById("packagingSeq"),
        document.getElementById("packagingSeqError"),
        validateSequence(document.getElementById("packagingSeq")?.value)
    );
    return nameValid && seqValid;
}

function buildPackagingPayload() {
    return {
        product_packaging_MasterId: parseInt(document.getElementById("packagingId").value || "0", 10),
        Name: document.getElementById("packagingName").value.trim(),
        Sequence: parseInt(document.getElementById("packagingSeq").value, 10),
        Language_Master_Id: parseOptionalInt(document.getElementById("packagingLanguageId").value),
        Thumbnailimage_Id: (document.getElementById("packagingThumbMediaId").value || "").trim(),
        Thumbnailimage_alt: (document.getElementById("packagingThumbAlt").value || "").trim(),
        Status: 1
    };
}

async function openPackagingModal(editId) {
    const modalEl = document.getElementById("packagingModal");
    const titleEl = document.getElementById("packagingModalTitle");
    resetPackagingForm();

    if (editId) {
        titleEl.textContent = "Edit Packaging";
        const res = await fetch("/Product_Packaging_Master/GetById?id=" + editId);
        if (!res.ok) {
            alert("Failed to load packaging.");
            return;
        }
        const data = await res.json();
        document.getElementById("packagingId").value = pickValue(data, "product_packaging_MasterId") || editId;
        document.getElementById("packagingName").value = pickValue(data, "name", "Name");
        document.getElementById("packagingLanguageId").value = pickValue(data, "language_Master_Id", "Language_Master_Id");
        document.getElementById("packagingSeq").value = pickValue(data, "sequence", "Sequence");
        document.getElementById("packagingThumbMediaId").value = pickValue(data, "thumbnailimage_Id", "Thumbnailimage_Id");
        document.getElementById("packagingThumbAlt").value = pickValue(data, "thumbnailimage_alt", "Thumbnailimage_alt");
        setImagePreview(
            document.getElementById("packagingThumbPreview"),
            pickValue(data, "thumbnail_Image_Url", "Thumbnail_Image_Url")
        );
    } else {
        titleEl.textContent = "Add Packaging";
    }

    bootstrap.Modal.getOrCreateInstance(modalEl).show();
}

document.addEventListener("DOMContentLoaded", function () {
    bindPackagingMediaPreviewFix();

    document.querySelector(".js-open-modal")?.addEventListener("click", function () {
        openPackagingModal(null);
    });

    document.querySelector(".js-save-packaging")?.addEventListener("click", function () {
        if (!validatePackagingForm()) return;
        const payload = buildPackagingPayload();
        const url = payload.product_packaging_MasterId > 0
            ? "/Product_Packaging_Master/UpdateAjax"
            : "/Product_Packaging_Master/AddAjax";
        postMaster(url, payload);
    });

    document.addEventListener("click", function (e) {
        if (e.target.classList.contains("packaging-edit-btn")) {
            openPackagingModal(e.target.dataset.id);
        }

        if (e.target.classList.contains("deactivate-btn") || e.target.classList.contains("activate-btn")) {
            const id = e.target.dataset.id;
            const status = e.target.classList.contains("activate-btn") ? 1 : 0;
            const msg = status === 1 ? "Activate this record?" : "Are you sure you want to deactivate?";
            if (!confirm(msg)) return;

            fetch("/Product_Packaging_Master/ChangeStatus", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Id: parseInt(id, 10), Status: status })
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
            fetch("/Product_Packaging_Master/Delete", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Id: parseInt(id, 10) })
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
                        product_packaging_MasterId: parseInt(row.dataset.id, 10),
                        Sequence: index + 1
                    });
                });

                fetch("/Product_Packaging_Master/UpdateSequence", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(order)
                }).then(() => {
                    document.querySelectorAll("#sortable tr[data-id]").forEach((row, index) => {
                        const lbl = document.getElementById("lblseq_" + row.dataset.id);
                        if (lbl) lbl.textContent = String(index + 1);
                    });
                });
            }
        });
    }
});
