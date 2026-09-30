document.addEventListener("DOMContentLoaded", function () {
  var form = document.querySelector("form.filter-inner");
  var selects = document.querySelectorAll(".cselect");
  if (!selects.length) return;

  var openSelect = null;

  function closeSelect(wrap) {
    if (!wrap) return;
    wrap.classList.remove("is-open");
    var menu = wrap.querySelector(".cselect-menu");
    var trigger = wrap.querySelector(".cselect-trigger");
    if (menu) menu.hidden = true;
    if (trigger) trigger.setAttribute("aria-expanded", "false");
    if (openSelect === wrap) openSelect = null;
  }

  function openSelectMenu(wrap) {
    if (openSelect && openSelect !== wrap) closeSelect(openSelect);
    wrap.classList.add("is-open");
    var menu = wrap.querySelector(".cselect-menu");
    var trigger = wrap.querySelector(".cselect-trigger");
    if (menu) menu.hidden = false;
    if (trigger) trigger.setAttribute("aria-expanded", "true");
    openSelect = wrap;
  }

  selects.forEach(function (wrap) {
    var native = wrap.querySelector(".cselect-native");
    if (!native) return;

    var listId = (native.id || "cselect") + "-menu";
    var trigger = document.createElement("button");
    trigger.type = "button";
    trigger.className = "cselect-trigger";
    trigger.setAttribute("aria-haspopup", "listbox");
    trigger.setAttribute("aria-expanded", "false");
    trigger.setAttribute("aria-controls", listId);

    var valueEl = document.createElement("span");
    valueEl.className = "cselect-value";
    valueEl.textContent = native.options[native.selectedIndex]
      ? native.options[native.selectedIndex].text
      : "";

    var chevron = document.createElement("span");
    chevron.className = "cselect-chevron";
    chevron.setAttribute("aria-hidden", "true");

    trigger.appendChild(valueEl);
    trigger.appendChild(chevron);

    var menu = document.createElement("ul");
    menu.className = "cselect-menu";
    menu.id = listId;
    menu.setAttribute("role", "listbox");
    menu.hidden = true;

    Array.prototype.forEach.call(native.options, function (option, optionIndex) {
      var item = document.createElement("li");
      var btn = document.createElement("button");
      btn.type = "button";
      btn.className = "cselect-option";
      btn.setAttribute("role", "option");
      btn.setAttribute("data-value", option.value);
      btn.textContent = option.text;
      if (optionIndex === native.selectedIndex) {
        btn.setAttribute("aria-selected", "true");
        btn.classList.add("is-active");
      }
      btn.addEventListener("click", function () {
        native.value = option.value;
        native.dispatchEvent(new Event("change", { bubbles: true }));
        valueEl.textContent = option.text;
        menu.querySelectorAll(".cselect-option").forEach(function (el) {
          var selected = el === btn;
          el.classList.toggle("is-active", selected);
          el.setAttribute("aria-selected", selected ? "true" : "false");
        });
        closeSelect(wrap);
        trigger.focus();
      });
      item.appendChild(btn);
      menu.appendChild(item);
    });

    native.setAttribute("tabindex", "-1");
    native.setAttribute("aria-hidden", "true");
    wrap.classList.add("is-enhanced");
    wrap.appendChild(trigger);
    wrap.appendChild(menu);

    trigger.addEventListener("click", function () {
      if (wrap.classList.contains("is-open")) closeSelect(wrap);
      else openSelectMenu(wrap);
    });
  });

  document.addEventListener("click", function (event) {
    if (!openSelect) return;
    if (!openSelect.contains(event.target)) closeSelect(openSelect);
  });

  document.addEventListener("keydown", function (event) {
    if (event.key !== "Escape") return;
    if (openSelect) {
      var trigger = openSelect.querySelector(".cselect-trigger");
      closeSelect(openSelect);
      if (trigger) trigger.focus();
      return;
    }
    setFilterOpen(false);
  });

  var filterOuter = (form && form.closest(".filter-outer")) || document.querySelector(".filter-outer");
  var openBtn = filterOuter && filterOuter.querySelector("[data-open-industry-filter]");

  function setFilterOpen(open) {
    if (!filterOuter) return;
    filterOuter.classList.toggle("is-filter-open", open);
    document.body.classList.toggle("is-industry-filter-open", open);
    if (openBtn) openBtn.setAttribute("aria-expanded", open ? "true" : "false");
    document.body.style.overflow = open ? "hidden" : "";
    if (window.lenis) {
      if (open && typeof window.lenis.stop === "function") window.lenis.stop();
      if (!open && typeof window.lenis.start === "function") window.lenis.start();
    }
  }

  function resetNativeSelect(native) {
    if (!native) return;
    native.selectedIndex = 0;
    native.dispatchEvent(new Event("change", { bubbles: true }));
    var wrap = native.closest(".cselect");
    if (!wrap) return;
    closeSelect(wrap);
    var valueEl = wrap.querySelector(".cselect-value");
    if (valueEl && native.options[0]) valueEl.textContent = native.options[0].text;
    wrap.querySelectorAll(".cselect-option").forEach(function (optionEl, index) {
      optionEl.classList.toggle("is-active", index === 0);
      optionEl.setAttribute("aria-selected", index === 0 ? "true" : "false");
    });
  }

  if (filterOuter) {
    if (openBtn) {
      openBtn.addEventListener("click", function () {
        setFilterOpen(true);
      });
    }

    filterOuter.querySelectorAll("[data-close-industry-filter]").forEach(function (el) {
      el.addEventListener("click", function () {
        setFilterOpen(false);
      });
    });
  }

  if (form) {
    var clearBtn = form.querySelector("[data-clear-industry-filter]");
    if (clearBtn) {
      clearBtn.addEventListener("click", function () {
        form.querySelectorAll(".cselect-native").forEach(resetNativeSelect);
      });
    }

    var applyBtn = form.querySelector("[data-apply-industry-filter]");
    if (applyBtn) {
      applyBtn.addEventListener("click", function () {
        form.requestSubmit ? form.requestSubmit() : form.dispatchEvent(new Event("submit", { bubbles: true, cancelable: true }));
        setFilterOpen(false);
      });
    }

    form.addEventListener("submit", function (event) {
      event.preventDefault();
      form.dispatchEvent(
        new CustomEvent("industryfilter:apply", {
          bubbles: true,
          detail: {
            industry: form.industry ? form.industry.value : "",
            application: form.application ? form.application.value : "",
            productCategories: form.productCategories ? form.productCategories.value : "",
            product: form.product ? form.product.value : ""
          }
        })
      );
      setFilterOpen(false);
    });
  }
});
