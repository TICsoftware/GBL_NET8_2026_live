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

function validateTestName(value) {
    const name = (value || "").trim();
    if (!name) {
        return "Test Name is required.";
    }
    if (name.length > 200) {
        return "Test Name cannot exceed 200 characters.";
    }
    return "";
}

function validateNablOption(value) {
    if (value === "" || value === null || value === undefined) {
        return "NABL Option is required.";
    }
    return "";
}

function validateMasterTypeSelection() {
    const specimen = document.getElementById("SpecimenId")?.value;
    const testType = document.getElementById("TestTypeId")?.value;
    const organ = document.getElementById("OrganId")?.value;
    const department = document.getElementById("DepartmentId")?.value;

    const hasSelection =
        (specimen && specimen !== "0") ||
        (testType && testType !== "0") ||
        (organ && organ !== "0") ||
        (department && department !== "0");

    if (!hasSelection) {
        return "Select at least one: Specimen, Test Type, Organ, or Department.";
    }
    return "";
}

function clearTestMasterValidation() {
    setFieldError(
        document.getElementById("TestName"),
        document.getElementById("TestNameError"),
        ""
    );
    setFieldError(
        document.getElementById("NABL_Option"),
        document.getElementById("NABL_OptionError"),
        ""
    );

    const masterError = document.getElementById("MasterTypeError");
    const masterFields = ["SpecimenId", "TestTypeId", "OrganId", "DepartmentId"];
    masterFields.forEach(id => {
        document.getElementById(id)?.classList.remove("is-invalid");
    });
    if (masterError) {
        masterError.textContent = "";
        masterError.classList.add("d-none");
    }
}

function validateTestMasterForm() {
    const testNameInput = document.getElementById("TestName");
    const nablInput = document.getElementById("NABL_Option");
    const masterError = document.getElementById("MasterTypeError");

    const testNameValid = setFieldError(
        testNameInput,
        document.getElementById("TestNameError"),
        validateTestName(testNameInput?.value)
    );
    const nablValid = setFieldError(
        nablInput,
        document.getElementById("NABL_OptionError"),
        validateNablOption(nablInput?.value)
    );

    const masterMessage = validateMasterTypeSelection();
    const masterFields = ["SpecimenId", "TestTypeId", "OrganId", "DepartmentId"];
    masterFields.forEach(id => {
        document.getElementById(id)?.classList.toggle("is-invalid", !!masterMessage);
    });
    if (masterError) {
        if (masterMessage) {
            masterError.textContent = masterMessage;
            masterError.classList.remove("d-none");
        } else {
            masterError.textContent = "";
            masterError.classList.add("d-none");
        }
    }

    if (!testNameValid) {
        testNameInput?.focus();
        return false;
    }
    if (!nablValid) {
        nablInput?.focus();
        return false;
    }
    if (masterMessage) {
        document.getElementById("SpecimenId")?.focus();
        return false;
    }

    return true;
}

function editTest(id) {

    fetch('/TestMaster/GetById?id=' + id)

    .then(response => response.json())

    .then(data => {

        clearTestMasterValidation();

        var modalEl = document.getElementById('addModal');

        var modal = new bootstrap.Modal(modalEl);

        modal.show();

        setTimeout(() => {

            document.getElementById("TestId").value =
                data.testId ?? 0;

            document.getElementById("TestName").value =
                data.testName ?? "";

            document.getElementById("SpecimenId").value =
                data.specimenId ?? "";

            document.getElementById("TestTypeId").value =
                data.testTypeId ?? "";

            document.getElementById("OrganId").value =
                data.organId ?? "";

            document.getElementById("DepartmentId").value =
                data.departmentId ?? "";

            document.getElementById("NABL_Option").value =
                String(data.nabL_Option ?? "").trim();

         }, 300);
    })

    .catch(error => {

        console.log(error);

        alert("Failed to load data");
    });
}

function deactivate(id) {
    if (!confirm("Deactivate?")) return;

    fetch('/TestMaster/Deactivate', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ id: id })
    }).then(() => location.reload());
}

function changeStatus(id, status) {

    let msg = status == 1
        ? "Activate this record?"
        : "Deactivate this record?";

    if (!confirm(msg))
        return;

    fetch('/TestMaster/ChangeStatus', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },

        body: JSON.stringify({
            id: id,
            status: status
        })
    })
    .then(r => location.reload());
}

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("testMasterForm");
    const modal = document.getElementById("addModal");

    if (form) {
        form.addEventListener("submit", function (e) {
            if (!validateTestMasterForm()) {
                e.preventDefault();
            }
        });
    }

    if (modal) {
        modal.addEventListener("show.bs.modal", function (event) {
            const trigger = event.relatedTarget;
            if (trigger && trigger.getAttribute("data-bs-target") === "#addModal") {
                form?.reset();
                document.getElementById("TestId").value = "";
                clearTestMasterValidation();
            }
        });

        modal.addEventListener("hidden.bs.modal", function () {
            clearTestMasterValidation();
        });
    }

    ["SpecimenId", "TestTypeId", "OrganId", "DepartmentId"].forEach(id => {
        document.getElementById(id)?.addEventListener("change", function () {
            if (!validateMasterTypeSelection()) {
                const masterError = document.getElementById("MasterTypeError");
                if (masterError) {
                    masterError.textContent = "";
                    masterError.classList.add("d-none");
                }
                ["SpecimenId", "TestTypeId", "OrganId", "DepartmentId"].forEach(fieldId => {
                    document.getElementById(fieldId)?.classList.remove("is-invalid");
                });
            }
        });
    });

    document.addEventListener("click", function (e) {
        const editBtn = e.target.closest(".js-edit-test");
        if (editBtn) {
            editTest(parseInt(editBtn.dataset.id, 10));
            return;
        }

        const statusBtn = e.target.closest(".js-change-status");
        if (statusBtn) {
            changeStatus(
                parseInt(statusBtn.dataset.id, 10),
                parseInt(statusBtn.dataset.status, 10)
            );
        }
    });
});
