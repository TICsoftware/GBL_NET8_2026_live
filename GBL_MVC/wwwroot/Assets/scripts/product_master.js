const PM_TEXTBOX_REGEX = /^[^<>@#$~^`!*]*$/;
const PM_FORBIDDEN_CHARS = /[<>@#$~^`!*]/g;
const PM_TEXTBOX_MESSAGE =
    "Special characters < > @ # $ ~ ^ ` ! * are not allowed.";
const PM_UNIQUE_MESSAGE = "This name already exists for the selected language.";

function validateTextValue(value, required, minLen, maxLen) {
    const text = (value || "").trim();
    if (!text) return required ? "Name is required." : "";
    if (minLen && text.length < minLen) return "Name must be at least " + minLen + " characters.";
    if (maxLen && text.length > maxLen) return "Cannot exceed " + maxLen + " characters.";
    if (!PM_TEXTBOX_REGEX.test(text)) return PM_TEXTBOX_MESSAGE;
    return "";
}

function validateMasterName(name) {
    return validateTextValue(name, true, 2, 500);
}

function validatePageName(name) {
    const value = (name || "").trim();
    if (!value) return "Page name is required.";
    if (value.length < 2) return "Page name must be at least 2 characters.";
    if (value.length > 300) return "Page name cannot exceed 300 characters.";
    if (!PM_TEXTBOX_REGEX.test(value)) return PM_TEXTBOX_MESSAGE;
    return "";
}

function validateOptionalText(value, maxLen) {
    return validateTextValue(value, false, 0, maxLen);
}

async function checkNameUnique(name, languageId, id) {
    const params = new URLSearchParams({
        name: name || "",
        languageId: languageId || "",
        id: String(id || 0)
    });
    const res = await fetch("/Product_Master/CheckNameExists?" + params.toString());
    if (!res.ok) return "";
    const data = await res.json();
    return data.exists || data.Exists ? PM_UNIQUE_MESSAGE : "";
}

function bindTextBoxGuards(root) {
    const scope = root && root.querySelectorAll ? root : document;
    scope.querySelectorAll("input[type='text']:not([name='search']), textarea:not(.editor-full)").forEach((el) => {
        if (el.dataset.specialBound === "1") return;
        el.dataset.specialBound = "1";
        el.addEventListener("input", function () {
            this.value = this.value.replace(PM_FORBIDDEN_CHARS, "");
        });
    });
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

function moveSelectedOptions(fromSel, toSel) {
    if (!fromSel || !toSel) return;
    Array.from(fromSel.selectedOptions).forEach((opt) => toSel.appendChild(opt));
    sortSelectOptions(toSel);
    sortSelectOptions(fromSel);
}

function sortSelectOptions(sel) {
    if (!sel) return;
    const options = Array.from(sel.options);
    options.sort((a, b) => (a.text || "").localeCompare(b.text || "", undefined, { sensitivity: "base" }));
    options.forEach((opt) => sel.appendChild(opt));
}

function getSelectValues(sel) {
    if (!sel) return [];
    return Array.from(sel.options).map((opt) => parseInt(opt.value, 10)).filter((n) => !Number.isNaN(n));
}

function resetListboxPair(availableId, selectedId) {
    const available = document.getElementById(availableId);
    const selected = document.getElementById(selectedId);
    if (!available || !selected) return;
    Array.from(selected.options).forEach((opt) => available.appendChild(opt));
    sortSelectOptions(available);
}

function moveIdsToSelected(availableId, selectedId, ids) {
    const available = document.getElementById(availableId);
    const selected = document.getElementById(selectedId);
    if (!available || !selected || !ids || ids.length === 0) return;
    const idSet = new Set(ids.map((n) => String(n)));
    Array.from(available.options).forEach((opt) => {
        if (idSet.has(opt.value)) selected.appendChild(opt);
    });
    sortSelectOptions(selected);
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

function pickArray(data, ...keys) {
    const value = pickValue(data, ...keys);
    if (Array.isArray(value)) {
        return value.map((n) => parseInt(n, 10)).filter((n) => !Number.isNaN(n));
    }
    return [];
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

function setFilePreview(url) {
    const preview = document.querySelector("#productMasterForm .filePreview");
    const link = preview ? preview.querySelector(".fileLink") : null;
    const clean = (url || "").toString().trim();
    if (preview && link && clean) {
        link.setAttribute("href", clean);
        preview.style.display = "block";
    } else if (preview) {
        preview.style.display = "none";
        if (link) link.setAttribute("href", "#");
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

function bindProductMediaPreviewFix() {
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

async function loadSubcategories(keepSelectedIds) {
    const industryIds = getSelectValues(document.getElementById("selectedIndustries"));
    const available = document.getElementById("availableSubcategories");
    const selected = document.getElementById("selectedSubcategories");
    if (!available || !selected) return;

    const preserve = keepSelectedIds && keepSelectedIds.length
        ? keepSelectedIds
        : getSelectValues(selected);

    available.innerHTML = "";
    selected.innerHTML = "";

    if (industryIds.length === 0) return;

    const res = await fetch("/Product_Master/GetSubcategories?industryIds=" + industryIds.join(","));
    if (!res.ok) return;
    const items = await res.json();
    const preserveSet = new Set((preserve || []).map((n) => String(n)));

    (items || []).forEach((item) => {
        const id = item.id ?? item.Id;
        const name = item.name ?? item.Name;
        const opt = new Option(name, id);
        if (preserveSet.has(String(id))) selected.appendChild(opt);
        else available.appendChild(opt);
    });
    sortSelectOptions(available);
    sortSelectOptions(selected);
}

function bindListboxes() {
    document.querySelectorAll(".js-move-right, .js-move-left").forEach((btn) => {
        btn.addEventListener("click", function () {
            const from = document.querySelector(btn.getAttribute("data-from"));
            const to = document.querySelector(btn.getAttribute("data-to"));
            moveSelectedOptions(from, to);
            const fromId = btn.getAttribute("data-from") || "";
            const toId = btn.getAttribute("data-to") || "";
            if (fromId.indexOf("Industries") >= 0 || toId.indexOf("Industries") >= 0) {
                loadSubcategories();
            }
        });
    });
}

function resetProductForm() {
    document.getElementById("productId").value = "0";
    document.getElementById("productName").value = "";
    document.getElementById("productPageName").value = "";
    document.getElementById("productLanguageId").value = "";
    document.getElementById("productSeq").value = "";
    document.getElementById("productBannerMediaId").value = "";
    document.getElementById("productThumbMediaId").value = "";
    document.getElementById("productSdsMediaId").value = "";
    setImagePreview(document.getElementById("productBannerPreview"), "");
    setImagePreview(document.getElementById("productThumbPreview"), "");
    setImagePreview(document.getElementById("productSdsPreview"), "");
    setFilePreview("");
    setEditorData("productIntro", "");
    setEditorData("productContent", "");
    setEditorData("productTechnicalOverview", "");
    setFieldError(document.getElementById("productName"), document.getElementById("productNameError"), "");
    setFieldError(document.getElementById("productPageName"), document.getElementById("productPageNameError"), "");
    setFieldError(document.getElementById("productSeq"), document.getElementById("productSeqError"), "");
    resetListboxPair("availableIndustries", "selectedIndustries");
    resetListboxPair("availableApplications", "selectedApplications");
    resetListboxPair("availablePackaging", "selectedPackaging");
    document.getElementById("availableSubcategories").innerHTML = "";
    document.getElementById("selectedSubcategories").innerHTML = "";
}

function validateProductForm() {
    const nameValid = setFieldError(
        document.getElementById("productName"),
        document.getElementById("productNameError"),
        validateMasterName(document.getElementById("productName")?.value)
    );
    const pageValid = setFieldError(
        document.getElementById("productPageName"),
        document.getElementById("productPageNameError"),
        validatePageName(document.getElementById("productPageName")?.value)
    );
    const seqValid = setFieldError(
        document.getElementById("productSeq"),
        document.getElementById("productSeqError"),
        validateSequence(document.getElementById("productSeq")?.value)
    );
    return nameValid && pageValid && seqValid;
}

function buildProductPayload() {
    const form = document.getElementById("productMasterForm");
    if (typeof window.syncCkEditorsToSource === "function") {
        window.syncCkEditorsToSource(form);
    }

    return {
        ProductId: parseInt(document.getElementById("productId").value || "0", 10),
        ProductName: document.getElementById("productName").value.trim(),
        Product_pagename: (document.getElementById("productPageName").value || "").trim(),
        Sequence: parseInt(document.getElementById("productSeq").value, 10),
        Language_Master_Id: parseOptionalInt(document.getElementById("productLanguageId").value),
        Banner_Image_media_id: parseOptionalInt(document.getElementById("productBannerMediaId").value),
        Thumbnail_Image_media_id: parseOptionalInt(document.getElementById("productThumbMediaId").value),
        SafetyDataSheet_media_id: parseOptionalInt(document.getElementById("productSdsMediaId").value),
        Intro: document.getElementById("productIntro").value || "",
        Content: document.getElementById("productContent").value || "",
        Technical_Overview: document.getElementById("productTechnicalOverview").value || "",
        Status: 1,
        IndustryIds: getSelectValues(document.getElementById("selectedIndustries")),
        ApplicationIds: getSelectValues(document.getElementById("selectedApplications")),
        SubcategoryIds: getSelectValues(document.getElementById("selectedSubcategories")),
        PackagingIds: getSelectValues(document.getElementById("selectedPackaging"))
    };
}

async function openProductModal(editId) {
    const modalEl = document.getElementById("productModal");
    const titleEl = document.getElementById("productModalTitle");
    resetProductForm();

    if (editId) {
        titleEl.textContent = "Edit Product";
        const res = await fetch("/Product_Master/GetById?id=" + editId);
        if (!res.ok) {
            alert("Failed to load product.");
            return;
        }
        const data = await res.json();
        document.getElementById("productId").value = pickValue(data, "productId", "ProductId") || editId;
        document.getElementById("productName").value = pickValue(data, "productName", "ProductName");
        document.getElementById("productPageName").value = pickValue(data, "product_pagename", "Product_pagename");
        document.getElementById("productLanguageId").value = pickValue(data, "language_Master_Id", "Language_Master_Id");
        document.getElementById("productSeq").value = pickValue(data, "sequence", "Sequence");
        document.getElementById("productBannerMediaId").value = pickValue(data, "banner_Image_media_id", "Banner_Image_media_id");
        document.getElementById("productThumbMediaId").value = pickValue(data, "thumbnail_Image_media_id", "Thumbnail_Image_media_id");
        document.getElementById("productSdsMediaId").value = pickValue(data, "safetyDataSheet_media_id", "SafetyDataSheet_media_id");
        setImagePreview(document.getElementById("productBannerPreview"), pickValue(data, "banner_Image_Url", "Banner_Image_Url"));
        setImagePreview(document.getElementById("productThumbPreview"), pickValue(data, "thumbnail_Image_Url", "Thumbnail_Image_Url"));
        setFilePreview(pickValue(data, "safetyDataSheet_Url", "SafetyDataSheet_Url"));

        moveIdsToSelected("availableIndustries", "selectedIndustries", pickArray(data, "industryIds", "IndustryIds"));
        moveIdsToSelected("availableApplications", "selectedApplications", pickArray(data, "applicationIds", "ApplicationIds"));
        moveIdsToSelected("availablePackaging", "selectedPackaging", pickArray(data, "packagingIds", "PackagingIds"));
        await loadSubcategories(pickArray(data, "subcategoryIds", "SubcategoryIds"));

        if (typeof window.initCkEditors === "function") {
            window.initCkEditors(modalEl);
        }
        setTimeout(() => {
            setEditorData("productIntro", pickValue(data, "intro", "Intro"));
            setEditorData("productContent", pickValue(data, "content", "Content"));
            setEditorData("productTechnicalOverview", pickValue(data, "technical_Overview", "Technical_Overview"));
        }, 300);
    } else {
        titleEl.textContent = "Add Product";
        if (typeof window.initCkEditors === "function") {
            window.initCkEditors(modalEl);
        }
    }

    bootstrap.Modal.getOrCreateInstance(modalEl).show();
}

document.addEventListener("DOMContentLoaded", function () {
    bindProductMediaPreviewFix();
    bindListboxes();
    bindTextBoxGuards(document);

    document.querySelector(".js-open-modal")?.addEventListener("click", function () {
        openProductModal(null);
    });

    document.querySelector(".js-save-product")?.addEventListener("click", async function () {
        if (!validateProductForm()) return;
        const payload = buildProductPayload();
        const uniqueMessage = await checkNameUnique(payload.ProductName, payload.Language_Master_Id, payload.ProductId);
        if (uniqueMessage) {
            setFieldError(
                document.getElementById("productName"),
                document.getElementById("productNameError"),
                uniqueMessage
            );
            return;
        }
        const url = payload.ProductId > 0
            ? "/Product_Master/UpdateAjax"
            : "/Product_Master/AddAjax";
        postMaster(url, payload);
    });

    document.addEventListener("click", function (e) {
        if (e.target.classList.contains("product-edit-btn")) {
            openProductModal(e.target.dataset.id);
        }

        if (e.target.classList.contains("deactivate-btn") || e.target.classList.contains("activate-btn")) {
            const id = e.target.dataset.id;
            const status = e.target.classList.contains("activate-btn") ? 1 : 0;
            const msg = status === 1 ? "Activate this record?" : "Are you sure you want to deactivate?";
            if (!confirm(msg)) return;

            fetch("/Product_Master/ChangeStatus", {
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
            fetch("/Product_Master/Delete", {
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
                        ProductId: parseInt(row.dataset.id, 10),
                        Sequence: index + 1
                    });
                });

                fetch("/Product_Master/UpdateSequence", {
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
