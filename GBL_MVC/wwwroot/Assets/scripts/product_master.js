const PM_TEXTBOX_REGEX = /^[^<>@#$~^`!*+=;]*$/;
const PM_FORBIDDEN_CHARS = /[<>@#$~^`!*+=;]/g;
const PM_TEXTBOX_MESSAGE =
    "Special characters < > @ # $ ~ ^ ` ! * + = ; are not allowed.";
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

async function checkPageNameUnique(pageName, id) {
    const params = new URLSearchParams({
        pageName: pageName || "",
        id: String(id || 0)
    });
    const res = await fetch("/Product_Master/CheckPageNameExists?" + params.toString());
    if (!res.ok) return "";
    const data = await res.json();
    return data.exists || data.Exists ? "This page name already exists." : "";
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

function setFilePreview(preview, url) {
    const link = preview ? preview.querySelector(".fileLink") : null;
    const clean = (url || "").toString().trim();
    if (preview && link && clean) {
        const ext = clean.split(".").pop().toLowerCase().split("?")[0];
        link.setAttribute("href", clean);
        link.innerHTML = '<i class="bi bi-file-earmark-pdf"></i> Open ' + (ext ? ext.toUpperCase() : "FILE");
        preview.style.display = "block";
    } else if (preview) {
        preview.style.display = "none";
        if (link) {
            link.setAttribute("href", "#");
            link.innerHTML = '<i class="bi bi-file-earmark"></i> View File';
        }
    }
}

function isPdfUrl(url) {
    return /\.pdf($|\?)/i.test((url || "").toString());
}

function hasMediaValue(field) {
    if (!field) return false;
    const hidden = field.querySelector("input[type='hidden']");
    const img = field.querySelector("img.imgPreview");
    const filePreview = field.querySelector(".filePreview");
    const link = filePreview ? filePreview.querySelector(".fileLink") : null;
    const hiddenVal = hidden && hidden.value && hidden.value !== "0";
    const hasImg = img && (img.getAttribute("src") || "").trim();
    const hasFile = filePreview && filePreview.style.display !== "none"
        && link && (link.getAttribute("href") || "") !== "#";
    return !!(hiddenVal || hasImg || hasFile);
}

function refreshMediaDeleteButton(field) {
    if (!field) return;
    const del = field.querySelector(".js-clear-media");
    if (del) del.classList.toggle("d-none", !hasMediaValue(field));
}

function refreshAllMediaDeleteButtons() {
    document.querySelectorAll("#productMasterForm .pm-media-field").forEach(refreshMediaDeleteButton);
}

function clearMediaField(field) {
    if (!field) return;
    const hidden = field.querySelector("input[type='hidden']");
    const img = field.querySelector("img.imgPreview");
    const filePreview = field.querySelector(".filePreview");
    if (hidden) hidden.value = "";
    setImagePreview(img, "");
    setFilePreview(filePreview, "");
    refreshMediaDeleteButton(field);
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
    const $file = $btn.nextAll(".filePreview").first().length
        ? $btn.nextAll(".filePreview").first()
        : $btn.parent().find(".filePreview").first();

    if (fileUrl && isPdfUrl(fileUrl)) {
        if ($img.length) setImagePreview($img.get(0), "");
        if ($file.length) setFilePreview($file.get(0), fileUrl);
    } else if ($img.length && fileUrl) {
        setImagePreview($img.get(0), fileUrl);
        if ($file.length) setFilePreview($file.get(0), "");
    }

    refreshMediaDeleteButton($btn.closest(".pm-media-field").get(0));
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
                refreshAllMediaDeleteButtons();
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

let certRowCounter = 0;

function getCertificateList() {
    return document.getElementById("productCertificateList");
}

function refreshCertificateButtons() {
    const count = document.querySelectorAll("#productCertificateList .pm-certificate-row").length;
    const addBtn = document.getElementById("btnAddCertificate");
    const moreBtn = document.getElementById("btnAddMoreCertificate");
    if (addBtn) addBtn.classList.toggle("d-none", count > 0);
    if (moreBtn) moreBtn.classList.toggle("d-none", count === 0);
}

function resetCertificateRows() {
    const list = getCertificateList();
    if (list) list.innerHTML = "";
    certRowCounter = 0;
    refreshCertificateButtons();
}

function addCertificateRow(item) {
    const list = getCertificateList();
    if (!list) return;

    certRowCounter += 1;
    const id = certRowCounter;
    const title = (item && (item.Title || item.title)) || "";
    const mediaId = (item && (item.MediaId || item.mediaId)) || "";
    const url = (item && (item.Url || item.url || item.Certificate_Url || item.certificate_Url)) || "";

    const row = document.createElement("div");
    row.className = "pm-certificate-row border rounded p-3 mb-3";
    row.innerHTML =
        '<div class="row">' +
            '<div class="col-md-6 mb-2">' +
                '<label class="form-label" for="productCertTitle_' + id + '">Certificate Title</label>' +
                '<input type="text" id="productCertTitle_' + id + '" maxlength="300" class="form-control js-cert-title" />' +
                '<span class="text-danger d-none js-cert-title-error"></span>' +
            '</div>' +
            '<div class="col-md-6 mb-2">' +
                '<label class="form-label d-block">Certificate PDF</label>' +
                '<div class="pm-media-field">' +
                    '<button type="button" class="btn btn-outline-primary select-media" ' +
                        'data-media-hidden="#productCertMediaId_' + id + '" ' +
                        'data-media-preview="#productCertPreview_' + id + '">Select PDF</button>' +
                    '<input type="hidden" id="productCertMediaId_' + id + '" class="js-cert-media-id" value="" />' +
                    '<img id="productCertPreview_' + id + '" src="" alt="Certificate preview" ' +
                        'class="imgPreview img-thumbnail industry-media-preview mt-2" />' +
                    '<div class="filePreview mt-2" style="display:none;">' +
                        '<a href="#" target="_blank" class="btn btn-sm btn-dark fileLink">' +
                            '<i class="bi bi-file-earmark"></i> View File</a>' +
                    '</div>' +
                    '<button type="button" class="btn btn-sm btn-outline-danger js-clear-media d-none mt-2">Delete file</button>' +
                '</div>' +
                '<span class="text-danger d-none js-cert-pdf-error"></span>' +
            '</div>' +
        '</div>' +
        '<button type="button" class="btn btn-sm btn-danger js-remove-certificate">Remove certificate</button>';

    list.appendChild(row);
    row.querySelector(".js-cert-title").value = title;
    row.querySelector(".js-cert-media-id").value = mediaId || "";
    if (url) {
        if (isPdfUrl(url)) setFilePreview(row.querySelector(".filePreview"), url);
        else setImagePreview(row.querySelector("img.imgPreview"), url);
    }
    bindTextBoxGuards(row);
    refreshMediaDeleteButton(row.querySelector(".pm-media-field"));
    refreshCertificateButtons();
}

function collectCertificates() {
    return Array.from(document.querySelectorAll("#productCertificateList .pm-certificate-row")).map((row) => ({
        Title: (row.querySelector(".js-cert-title")?.value || "").trim(),
        MediaId: parseOptionalInt(row.querySelector(".js-cert-media-id")?.value),
        Url: row.querySelector(".filePreview .fileLink")?.getAttribute("href") || ""
    }));
}

function validateCertificates() {
    let valid = true;
    document.querySelectorAll("#productCertificateList .pm-certificate-row").forEach((row) => {
        const titleInput = row.querySelector(".js-cert-title");
        const titleError = row.querySelector(".js-cert-title-error");
        const pdfError = row.querySelector(".js-cert-pdf-error");
        const title = (titleInput?.value || "").trim();
        const mediaId = parseOptionalInt(row.querySelector(".js-cert-media-id")?.value);
        const hasTitle = title.length > 0;
        const hasMedia = !!mediaId;

        let titleMessage = "";
        if (hasTitle || hasMedia) {
            if (!hasTitle) titleMessage = "Certificate title is required.";
            else titleMessage = validateOptionalText(title, 300);
        }
        if (!setFieldError(titleInput, titleError, titleMessage)) valid = false;

        let pdfMessage = "";
        if ((hasTitle || hasMedia) && !hasMedia) pdfMessage = "Select a certificate PDF.";
        if (pdfError) {
            if (pdfMessage) {
                pdfError.textContent = pdfMessage;
                pdfError.classList.remove("d-none");
                valid = false;
            } else {
                pdfError.textContent = "";
                pdfError.classList.add("d-none");
            }
        }
    });
    return valid;
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
    document.getElementById("productBannerImageAlt").value = "";
    document.getElementById("productThumbnailImageAlt").value = "";
    setImagePreview(document.getElementById("productBannerPreview"), "");
    setImagePreview(document.getElementById("productThumbPreview"), "");
    setImagePreview(document.getElementById("productSdsPreview"), "");
    setFilePreview(document.getElementById("productSdsFilePreview"), "");
    setEditorData("productIntro", "");
    setEditorData("productContent", "");
    setEditorData("productTechnicalOverview", "");
    setEditorData("productMainApplication", "");
    resetCertificateRows();
    refreshAllMediaDeleteButtons();
    setFieldError(document.getElementById("productName"), document.getElementById("productNameError"), "");
    setFieldError(document.getElementById("productPageName"), document.getElementById("productPageNameError"), "");
    setFieldError(document.getElementById("productBannerImageAlt"), document.getElementById("productBannerImageAltError"), "");
    setFieldError(document.getElementById("productThumbnailImageAlt"), document.getElementById("productThumbnailImageAltError"), "");
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
    const bannerAltValid = setFieldError(
        document.getElementById("productBannerImageAlt"),
        document.getElementById("productBannerImageAltError"),
        validateOptionalText(document.getElementById("productBannerImageAlt")?.value, 500)
    );
    const thumbAltValid = setFieldError(
        document.getElementById("productThumbnailImageAlt"),
        document.getElementById("productThumbnailImageAltError"),
        validateOptionalText(document.getElementById("productThumbnailImageAlt")?.value, 500)
    );
    const seqValid = setFieldError(
        document.getElementById("productSeq"),
        document.getElementById("productSeqError"),
        validateSequence(document.getElementById("productSeq")?.value)
    );
    return nameValid && pageValid && bannerAltValid && thumbAltValid && seqValid && validateCertificates();
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
        Banner_Image_Alt: (document.getElementById("productBannerImageAlt").value || "").trim(),
        Thumbnail_Image_Alt: (document.getElementById("productThumbnailImageAlt").value || "").trim(),
        Intro: document.getElementById("productIntro").value || "",
        Content: document.getElementById("productContent").value || "",
        Technical_Overview: document.getElementById("productTechnicalOverview").value || "",
        Main_Application: document.getElementById("productMainApplication").value || "",
        Certificates: collectCertificates(),
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
        document.getElementById("productBannerImageAlt").value = pickValue(data, "banner_Image_Alt", "Banner_Image_Alt");
        document.getElementById("productThumbnailImageAlt").value = pickValue(data, "thumbnail_Image_Alt", "Thumbnail_Image_Alt");
        setImagePreview(document.getElementById("productBannerPreview"), pickValue(data, "banner_Image_Url", "Banner_Image_Url"));
        setImagePreview(document.getElementById("productThumbPreview"), pickValue(data, "thumbnail_Image_Url", "Thumbnail_Image_Url"));
        const sdsUrl = pickValue(data, "safetyDataSheet_Url", "SafetyDataSheet_Url");
        if (isPdfUrl(sdsUrl)) setFilePreview(document.getElementById("productSdsFilePreview"), sdsUrl);
        else setImagePreview(document.getElementById("productSdsPreview"), sdsUrl);
        refreshAllMediaDeleteButtons();

        const certificates = pickValue(data, "certificates", "Certificates");
        if (Array.isArray(certificates) && certificates.length) {
            certificates.forEach((cert) => addCertificateRow(cert));
        }

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
            setEditorData("productMainApplication", pickValue(data, "main_Application", "Main_Application"));
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

    document.querySelectorAll(".js-add-certificate").forEach((btn) => {
        btn.addEventListener("click", function () {
            addCertificateRow();
        });
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
        const pageUniqueMessage = await checkPageNameUnique(payload.Product_pagename, payload.ProductId);
        if (pageUniqueMessage) {
            setFieldError(
                document.getElementById("productPageName"),
                document.getElementById("productPageNameError"),
                pageUniqueMessage
            );
            return;
        }
        const url = payload.ProductId > 0
            ? "/Product_Master/UpdateAjax"
            : "/Product_Master/AddAjax";
        postMaster(url, payload);
    });

    document.addEventListener("click", function (e) {
        const clearBtn = e.target.closest ? e.target.closest(".js-clear-media") : null;
        if (clearBtn) {
            e.preventDefault();
            clearMediaField(clearBtn.closest(".pm-media-field"));
            return;
        }

        const removeCert = e.target.closest ? e.target.closest(".js-remove-certificate") : null;
        if (removeCert) {
            e.preventDefault();
            const row = removeCert.closest(".pm-certificate-row");
            if (row) row.remove();
            refreshCertificateButtons();
            return;
        }

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
