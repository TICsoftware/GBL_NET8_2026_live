const NABL_TEXTBOX_REGEX = /^[^~<>|/\\!@#]*$/;
const NABL_TEXTBOX_MESSAGE =
    "Special characters ~ < > | / \\ ! @ # are not allowed.";

function validateMasterName(name) {
    const value = (name || "").trim();

    if (!value) {
        return "Name is required.";
    }
    if (value.length < 2) {
        return "Name must be at least 2 characters.";
    }
    if (value.length > 200) {
        return "Name cannot exceed 200 characters.";
    }
    if (!NABL_TEXTBOX_REGEX.test(value)) {
        return NABL_TEXTBOX_MESSAGE;
    }

    return "";
}

function validateSequence(seq) {
    if (seq === "" || seq === null || seq === undefined) {
        return "Sequence is required.";
    }

    const value = parseInt(seq, 10);
    if (Number.isNaN(value) || value < 1 || value > 9999) {
        return "Sequence must be between 1 and 9999.";
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

function validateAddForm() {
    const nameInput = document.getElementById("newName");
    const seqInput = document.getElementById("newSeq");
    const nameError = document.getElementById("newNameError");
    const seqError = document.getElementById("newSeqError");

    const nameValid = setFieldError(
        nameInput,
        nameError,
        validateMasterName(nameInput?.value)
    );
    const seqValid = setFieldError(
        seqInput,
        seqError,
        validateSequence(seqInput?.value)
    );

    return nameValid && seqValid;
}

function validateInlineRow(id) {
    const nameInput = document.getElementById("txt_" + id);
    const seqInput = document.getElementById("txtseq_" + id);

    const nameMessage = validateMasterName(nameInput?.value);
    const seqMessage = validateSequence(seqInput?.value);

    if (nameMessage) {
        alert(nameMessage);
        nameInput?.classList.add("is-invalid");
        nameInput?.focus();
        return false;
    }

    if (seqMessage) {
        alert(seqMessage);
        seqInput?.classList.add("is-invalid");
        seqInput?.focus();
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

    let wrapper = document.querySelector("[data-master-type]");
    let masterType = null;

    if (wrapper) {
        masterType = wrapper.getAttribute("data-master-type");
    } else if (document.body) {
        masterType = document.body.getAttribute("data-master-type");
    }

    if (!masterType) {
        alert("MasterType is missing. Check your view data-master-type.");
        return;
    }

    function toggleEdit(id, isEdit) {
        document.getElementById("lbl_" + id).classList.toggle("d-none", isEdit);
        document.getElementById("txt_" + id).classList.toggle("d-none", !isEdit);

        document.getElementById("lblseq_" + id).classList.toggle("d-none", isEdit);
        document.getElementById("txtseq_" + id).classList.toggle("d-none", !isEdit);

        document.querySelector(".edit-btn[data-id='" + id + "']").classList.toggle("d-none", isEdit);
        document.querySelector(".save-btn[data-id='" + id + "']").classList.toggle("d-none", !isEdit);
    }

    function openModal() {
        const form = document.getElementById("addMasterForm");
        if (form) {
            form.reset();
        }

        setFieldError(
            document.getElementById("newName"),
            document.getElementById("newNameError"),
            ""
        );
        setFieldError(
            document.getElementById("newSeq"),
            document.getElementById("newSeqError"),
            ""
        );

        var modal = new bootstrap.Modal(document.getElementById("addModal"));
        modal.show();
    }

    function saveNew() {
        if (!validateAddForm()) {
            return;
        }

        let name = document.getElementById("newName").value.trim();
        let seq = document.getElementById("newSeq").value;

        postMaster("/NABLtest_master/AddAjax", {
            Name: name,
            Sequence: parseInt(seq, 10),
            MasterType: masterType
        });
    }

    document.querySelector(".js-open-modal")?.addEventListener("click", openModal);
    document.querySelector(".js-save-new")?.addEventListener("click", saveNew);

    document.addEventListener("click", function (e) {

        if (e.target.classList.contains("edit-btn")) {
            let id = e.target.dataset.id;
            toggleEdit(id, true);
        }

        if (e.target.classList.contains("save-btn")) {
            let id = e.target.dataset.id;

            if (!validateInlineRow(id)) {
                return;
            }

            let name = document.getElementById("txt_" + id).value.trim();
            let seq = document.getElementById("txtseq_" + id).value;

            postMaster("/NABLtest_master/UpdateAjax", {
                ID: parseInt(id, 10),
                Name: name,
                Sequence: parseInt(seq, 10),
                MasterType: masterType
            });
        }

        if (e.target.classList.contains("deactivate-btn") ||
            e.target.classList.contains("activate-btn")) {

            let id = e.target.dataset.id;
            let status = e.target.classList.contains("activate-btn") ? 1 : 0;
            let msg = status === 1
                ? "Activate this record?"
                : "Are you sure you want to deactivate?";

            if (!confirm(msg))
                return;

            fetch('/NABLtest_master/ChangeStatus', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    Id: parseInt(id, 10),
                    Status: status,
                    Type: masterType
                })
            })
            .then(res => res.ok ? location.reload() : alert("Status update failed"));
        }

    });

    let sortableElement = document.getElementById('sortable');

    if (sortableElement) {
        new Sortable(sortableElement, {
            animation: 150,
            handle: '.drag-handle',
            onEnd: function () {
                let order = [];

                document.querySelectorAll("#sortable tr").forEach((row, index) => {
                    order.push({
                        ID: row.dataset.id,
                        Sequence: index + 1,
                        MasterType: masterType
                    });
                });

                fetch('/NABLtest_master/UpdateSequence', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(order)
                });
            }
        });
    }

});
