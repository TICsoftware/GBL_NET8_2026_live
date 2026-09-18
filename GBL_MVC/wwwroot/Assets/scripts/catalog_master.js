const CATALOG_TEXTBOX_REGEX = /^[^~<>|/\\!@#]*$/;
const CATALOG_TEXTBOX_MESSAGE =
    "Special characters ~ < > | / \\ ! @ # are not allowed.";

function validateMasterName(name) {
    const value = (name || "").trim();

    if (!value) {
        return "Name is required.";
    }
    if (value.length < 2) {
        return "Name must be at least 2 characters.";
    }
    if (value.length > 500) {
        return "Name cannot exceed 500 characters.";
    }
    if (!CATALOG_TEXTBOX_REGEX.test(value)) {
        return CATALOG_TEXTBOX_MESSAGE;
    }

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
    if (!input) {
        return false;
    }

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

function isMappingType(masterType) {
    return (masterType || "").toLowerCase() === "mapping";
}

function showPageName(masterType) {
    const t = (masterType || "").toLowerCase();
    return t === "industry" || t === "product";
}

function validateAddForm(masterType) {
    const seqInput = document.getElementById("newSeq");
    const seqError = document.getElementById("newSeqError");
    const seqValid = setFieldError(seqInput, seqError, validateSequence(seqInput?.value));

    if (isMappingType(masterType)) {
        const industryInput = document.getElementById("newIndustryId");
        const categoryInput = document.getElementById("newCategoryId");
        const industryError = document.getElementById("newIndustryError");
        const categoryError = document.getElementById("newCategoryError");

        const industryValid = setFieldError(
            industryInput,
            industryError,
            industryInput?.value ? "" : "Industry is required."
        );
        const categoryValid = setFieldError(
            categoryInput,
            categoryError,
            categoryInput?.value ? "" : "Subcategory is required."
        );

        return industryValid && categoryValid && seqValid;
    }

    const nameInput = document.getElementById("newName");
    const nameError = document.getElementById("newNameError");
    const nameValid = setFieldError(nameInput, nameError, validateMasterName(nameInput?.value));
    return nameValid && seqValid;
}

function validateInlineRow(id, masterType) {
    const seqInput = document.getElementById("txtseq_" + id);
    const seqMessage = validateSequence(seqInput?.value);

    if (seqMessage) {
        alert(seqMessage);
        seqInput?.classList.add("is-invalid");
        seqInput?.focus();
        return false;
    }

    if (isMappingType(masterType)) {
        const industry = document.getElementById("ddlIndustry_" + id);
        const category = document.getElementById("ddlCategory_" + id);
        if (!industry?.value) {
            alert("Industry is required.");
            return false;
        }
        if (!category?.value) {
            alert("Subcategory is required.");
            return false;
        }
        seqInput?.classList.remove("is-invalid");
        return true;
    }

    const nameInput = document.getElementById("txt_" + id);
    const nameMessage = validateMasterName(nameInput?.value);
    if (nameMessage) {
        alert(nameMessage);
        nameInput?.classList.add("is-invalid");
        nameInput?.focus();
        return false;
    }

    nameInput?.classList.remove("is-invalid");
    seqInput?.classList.remove("is-invalid");
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
        if (data?.message) {
            message = data.message;
        }
    } catch {
        // keep default message
    }

    alert(message);
}

document.addEventListener("DOMContentLoaded", function () {
    const wrapper = document.querySelector("[data-master-type]");
    let masterType = wrapper ? wrapper.getAttribute("data-master-type") : null;

    if (!masterType) {
        alert("MasterType is missing. Check your view data-master-type.");
        return;
    }

    function toggleEdit(id, isEdit) {
        if (isMappingType(masterType)) {
            document.getElementById("lbl_" + id)?.classList.toggle("d-none", isEdit);
            document.getElementById("ddlIndustry_" + id)?.classList.toggle("d-none", !isEdit);
            document.getElementById("lblrel_" + id)?.classList.toggle("d-none", isEdit);
            document.getElementById("ddlCategory_" + id)?.classList.toggle("d-none", !isEdit);
        } else {
            document.getElementById("lbl_" + id)?.classList.toggle("d-none", isEdit);
            document.getElementById("txt_" + id)?.classList.toggle("d-none", !isEdit);

            if (showPageName(masterType)) {
                document.getElementById("lblpage_" + id)?.classList.toggle("d-none", isEdit);
                document.getElementById("txtpage_" + id)?.classList.toggle("d-none", !isEdit);
            }
        }

        document.getElementById("lblseq_" + id)?.classList.toggle("d-none", isEdit);
        document.getElementById("txtseq_" + id)?.classList.toggle("d-none", !isEdit);

        document.querySelector(".edit-btn[data-id='" + id + "']")?.classList.toggle("d-none", isEdit);
        document.querySelector(".save-btn[data-id='" + id + "']")?.classList.toggle("d-none", !isEdit);
    }

    function openModal() {
        const form = document.getElementById("addMasterForm");
        if (form) {
            form.reset();
        }

        setFieldError(document.getElementById("newName"), document.getElementById("newNameError"), "");
        setFieldError(document.getElementById("newSeq"), document.getElementById("newSeqError"), "");
        setFieldError(document.getElementById("newIndustryId"), document.getElementById("newIndustryError"), "");
        setFieldError(document.getElementById("newCategoryId"), document.getElementById("newCategoryError"), "");

        const modal = new bootstrap.Modal(document.getElementById("addModal"));
        modal.show();
    }

    function saveNew() {
        if (!validateAddForm(masterType)) {
            return;
        }

        const seq = parseInt(document.getElementById("newSeq").value, 10);
        let payload = {
            Sequence: seq,
            Status: 1,
            MasterType: masterType
        };

        if (isMappingType(masterType)) {
            payload.IndustryId = parseInt(document.getElementById("newIndustryId").value, 10);
            payload.Category_Master_Id = parseInt(document.getElementById("newCategoryId").value, 10);
        } else {
            payload.Name = document.getElementById("newName").value.trim();
            if (showPageName(masterType)) {
                payload.PageName = (document.getElementById("newPageName")?.value || "").trim();
            }
        }

        postMaster("/Catalog_master/AddAjax", payload);
    }

    document.querySelector(".js-open-modal")?.addEventListener("click", openModal);
    document.querySelector(".js-save-new")?.addEventListener("click", saveNew);

    document.addEventListener("click", function (e) {
        if (e.target.classList.contains("edit-btn")) {
            toggleEdit(e.target.dataset.id, true);
        }

        if (e.target.classList.contains("save-btn")) {
            const id = e.target.dataset.id;
            if (!validateInlineRow(id, masterType)) {
                return;
            }

            const seq = parseInt(document.getElementById("txtseq_" + id).value, 10);
            let payload = {
                ID: parseInt(id, 10),
                Sequence: seq,
                MasterType: masterType
            };

            if (isMappingType(masterType)) {
                payload.IndustryId = parseInt(document.getElementById("ddlIndustry_" + id).value, 10);
                payload.Category_Master_Id = parseInt(document.getElementById("ddlCategory_" + id).value, 10);
            } else {
                payload.Name = document.getElementById("txt_" + id).value.trim();
                if (showPageName(masterType)) {
                    payload.PageName = (document.getElementById("txtpage_" + id)?.value || "").trim();
                }
            }

            postMaster("/Catalog_master/UpdateAjax", payload);
        }

        if (e.target.classList.contains("deactivate-btn") ||
            e.target.classList.contains("activate-btn")) {

            const id = e.target.dataset.id;
            const status = e.target.classList.contains("activate-btn") ? 1 : 0;
            const msg = status === 1
                ? "Activate this record?"
                : "Are you sure you want to deactivate?";

            if (!confirm(msg)) {
                return;
            }

            fetch("/Catalog_master/ChangeStatus", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    Id: parseInt(id, 10),
                    Status: status,
                    Type: masterType
                })
            })
                .then(async (res) => {
                    if (res.ok) {
                        location.reload();
                        return;
                    }
                    let message = "Status update failed.";
                    try {
                        const data = await res.json();
                        if (data?.message) {
                            message = data.message;
                        }
                    } catch {
                        // keep default
                    }
                    alert(message);
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

                fetch("/Catalog_master/UpdateSequence", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(order)
                }).then(() => {
                    document.querySelectorAll("#sortable tr[data-id]").forEach((row, index) => {
                        const id = row.dataset.id;
                        const lbl = document.getElementById("lblseq_" + id);
                        const txt = document.getElementById("txtseq_" + id);
                        if (lbl) {
                            lbl.textContent = String(index + 1);
                        }
                        if (txt) {
                            txt.value = String(index + 1);
                        }
                    });
                });
            }
        });
    }
});
