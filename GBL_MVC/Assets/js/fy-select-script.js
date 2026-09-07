function initCustomSelects() {
  const selects = document.querySelectorAll(".fy-select:not([data-customized])");

  selects.forEach((select) => {
    if (!select.options.length) return;

    select.dataset.customized = "true";

    /* Hide native select */
    select.style.position = "absolute";
    select.style.opacity = 0;
    select.style.pointerEvents = "none";
    select.style.width = "0";
    select.style.height = "0";
    select.style.display = "none";

    const wrapper = select.parentElement;
    wrapper.style.position = "relative";

    /* Selected box */
    const selected = document.createElement("div");
    selected.className =
      "selected-value border border-[#E9E9E9] rounded-xl px-2 py-2.5 pr-4 text-sm text-[#767676] cursor-pointer flex items-center justify-between gap-2 transition-all duration-200";
    selected.setAttribute("role", "button");
    selected.setAttribute("tabindex", "0");
    selected.setAttribute("aria-expanded", "false");

    const selectedText = document.createElement("span");
    selectedText.textContent =
      select.options[select.selectedIndex]?.text || "";

    if (select.value === "") {
      selectedText.classList.add("text-[#999]");
    }

    const arrow = document.createElement("img");
    arrow.src =
      "Assets/images/common-images/filter-icon.svg";
    arrow.className =
      "select-arrow transition-transform duration-200";

    selected.appendChild(selectedText);
    selected.appendChild(arrow);

    /* Dropdown */
    const dropdown = document.createElement("ul");
    dropdown.className =
      "dropdown-options absolute left-1/2 -translate-x-1/2 mt-2 w-[200px] bg-white border border-gray-300 rounded-lg shadow-lg hidden max-h-[320px] overflow-y-auto z-[100]";

    /* Populate options */
    Array.from(select.options).forEach((option, index) => {
      const li = document.createElement("li");
      li.textContent = option.text;
      li.dataset.value = option.value;
      li.className =
        "px-2 py-2 cursor-pointer text-left text-sm sm:text-base hover:bg-[#DE392E] hover:text-white";

      /* Placeholder */
      if (option.value === "") {
        li.classList.add("text-[#999]", "pointer-events-none");
      }

      /* Default selected */
      if (index === select.selectedIndex) {
        li.classList.add("active");
      }

      li.addEventListener("click", () => {
        select.value = option.value;
        select.dispatchEvent(new Event("change", { bubbles: true }));
        closeDropdown(wrapper);
      });

      dropdown.appendChild(li);
    });

    /* Toggle dropdown */
    selected.addEventListener("click", () => {
      closeAllDropdowns(wrapper);

      const isOpen = !dropdown.classList.contains("hidden");

      dropdown.classList.toggle("hidden", isOpen);
      selected.classList.toggle("active", !isOpen);
      arrow.classList.toggle("rotate-0", !isOpen);
      selected.setAttribute("aria-expanded", String(!isOpen));
    });

    /* Keyboard support */
    selected.addEventListener("keydown", (e) => {
      if (e.key === "Enter" || e.key === " ") {
        e.preventDefault();
        selected.click();
      }
    });

    /* Sync when native select changes */
    select.addEventListener("change", () => {
      const option = select.options[select.selectedIndex];
      selectedText.textContent = option.text;

      if (option.value === "") {
        selectedText.classList.add("text-[#999]");
      } else {
        selectedText.classList.remove("text-[#999]");
      }

      dropdown.querySelectorAll("li").forEach((li) => {
        li.classList.toggle("active", li.dataset.value === option.value);
      });
    });

    wrapper.appendChild(selected);
    wrapper.appendChild(dropdown);
  });
}

/* Close helpers */
function closeDropdown(wrapper) {
  const dropdown = wrapper.querySelector(".dropdown-options");
  const selected = wrapper.querySelector(".selected-value");
  const arrow = selected?.querySelector(".select-arrow");

  dropdown?.classList.add("hidden");
  selected?.classList.remove("active");
  arrow?.classList.remove("rotate-0");
  selected?.setAttribute("aria-expanded", "false");
}

function closeAllDropdowns(exceptWrapper = null) {
  document.querySelectorAll(".dropdown-options").forEach((dropdown) => {
    const wrapper = dropdown.parentElement;
    if (wrapper !== exceptWrapper) {
      closeDropdown(wrapper);
    }
  });
}

/* Outside click */
document.addEventListener("click", (e) => {
  document.querySelectorAll(".dropdown-options").forEach((dropdown) => {
    if (!dropdown.parentElement.contains(e.target)) {
      closeDropdown(dropdown.parentElement);
    }
  });
});

/* Init */
document.addEventListener("DOMContentLoaded", initCustomSelects);
window.initCustomSelects = initCustomSelects;
