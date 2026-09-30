document.addEventListener("DOMContentLoaded", function () {
  var root = document.querySelector(".product-filter-outer");
  if (!root) return;

  var form = root.querySelector("#product-filter");
  var cards = Array.prototype.slice.call(root.querySelectorAll(".product-card"));
  var countEl = root.querySelector("[data-product-count]");
  var chipsEl = root.querySelector("[data-filter-chips]");
  var emptyEl = root.querySelector("[data-filter-empty]");
  var clearBtns = root.querySelectorAll("[data-clear-filters]");
  var openMobileBtn = root.querySelector("[data-open-mobile-filter]");
  var applyMobileBtn = root.querySelector("[data-apply-mobile-filter]");
  var closeMobileBtns = root.querySelectorAll("[data-close-mobile-filter]");
  var backdrop = root.querySelector(".filter-mobile-backdrop");

  function isMobileFilter() {
    return window.matchMedia("(max-width: 1079px)").matches;
  }

  function setMobileFilterOpen(open) {
    root.classList.toggle("is-mobile-filter-open", open);
    if (backdrop) backdrop.setAttribute("aria-hidden", open ? "false" : "true");
    if (openMobileBtn) openMobileBtn.setAttribute("aria-expanded", open ? "true" : "false");
    document.body.style.overflow = open ? "hidden" : "";
    if (window.lenis) {
      if (open && typeof window.lenis.stop === "function") window.lenis.stop();
      if (!open && typeof window.lenis.start === "function") window.lenis.start();
    }
  }

  function closeDropdown(wrap) {
    if (!wrap) return;
    wrap.classList.remove("is-open");
    var panel = wrap.querySelector(".pf-dropdown__panel");
    var trigger = wrap.querySelector(".pf-dropdown__trigger");
    if (panel) panel.setAttribute("aria-hidden", "true");
    if (trigger) trigger.setAttribute("aria-expanded", "false");
  }

  function hasChecked(wrap) {
    return Boolean(wrap && wrap.querySelector("input[type='checkbox']:checked"));
  }

  function openMenu(wrap) {
    wrap.classList.add("is-open");
    var panel = wrap.querySelector(".pf-dropdown__panel");
    var trigger = wrap.querySelector(".pf-dropdown__trigger");
    if (panel) panel.setAttribute("aria-hidden", "false");
    if (trigger) trigger.setAttribute("aria-expanded", "true");
  }

  function selectedValues(name) {
    return Array.prototype.map.call(
      form.querySelectorAll('input[name="' + name + '"]:checked'),
      function (input) {
        return input.value;
      }
    );
  }

  function tokenList(value) {
    return (value || "")
      .split(",")
      .map(function (item) {
        return item.trim();
      })
      .filter(Boolean);
  }

  function matchesGroup(cardTokens, selected) {
    if (!selected.length) return true;
    return selected.some(function (value) {
      return cardTokens.indexOf(value) !== -1;
    });
  }

  function applyFilters() {
    var industry = selectedValues("industry");
    var application = selectedValues("application");
    var category = selectedValues("category");
    var visible = 0;

    cards.forEach(function (card) {
      var match =
        matchesGroup(tokenList(card.getAttribute("data-industry")), industry) &&
        matchesGroup(tokenList(card.getAttribute("data-application")), application) &&
        matchesGroup(tokenList(card.getAttribute("data-category")), category);

      card.hidden = !match;
      if (match) visible += 1;
    });

    if (countEl) countEl.textContent = String(visible);
    if (emptyEl) emptyEl.hidden = visible !== 0;
    renderChips();
    updateOptionCounts();
    updateDropdownState();
    if (typeof window.refreshProductCardAnimation === "function") {
      window.refreshProductCardAnimation();
    }
  }

  function padCount(value) {
    return value < 10 ? "0" + value : String(value);
  }

  function attrForInput(input) {
    if (input.name === "industry") return "data-industry";
    if (input.name === "application") return "data-application";
    return "data-category";
  }

  function updateOptionCounts() {
    form.querySelectorAll(".pf-option input").forEach(function (input) {
      var attr = attrForInput(input);
      var count = cards.filter(function (card) {
        return tokenList(card.getAttribute(attr)).indexOf(input.value) !== -1;
      }).length;
      var countNode = input.closest(".pf-option").querySelector(".pf-option__count");
      if (countNode) countNode.textContent = padCount(count);
    });
  }

  function renderChips() {
    if (!chipsEl) return;
    chipsEl.innerHTML = "";

    Array.prototype.forEach.call(form.querySelectorAll("input[type='checkbox']:checked"), function (input) {
      var chip = document.createElement("span");
      chip.className = "product-filter-chip";
      chip.appendChild(document.createTextNode(input.getAttribute("data-label") || input.value));

      var remove = document.createElement("button");
      remove.type = "button";
      remove.className = "product-filter-chip__remove";
      remove.setAttribute("aria-label", "Remove " + (input.getAttribute("data-label") || input.value));
      remove.innerHTML = "&times;";
      remove.addEventListener("click", function () {
        input.checked = false;
        applyFilters();
      });

      chip.appendChild(remove);
      chipsEl.appendChild(chip);
    });
  }

  function updateDropdownState() {
    root.querySelectorAll(".pf-dropdown").forEach(function (wrap) {
      var locked = hasChecked(wrap);
      var trigger = wrap.querySelector(".pf-dropdown__trigger");
      wrap.classList.toggle("has-value", locked);
      if (trigger) {
        trigger.setAttribute("aria-disabled", locked ? "true" : "false");
      }
    });
  }

  root.querySelectorAll(".pf-dropdown").forEach(function (wrap) {
    var trigger = wrap.querySelector(".pf-dropdown__trigger");
    var panel = wrap.querySelector(".pf-dropdown__panel");
    if (panel) panel.setAttribute("aria-hidden", "true");

    wrap.addEventListener("click", function (event) {
      var onTrigger = event.target.closest && event.target.closest(".pf-dropdown__trigger");
      if (wrap.classList.contains("is-open")) {
        if (onTrigger) closeDropdown(wrap);
        return;
      }
      openMenu(wrap);
    });
  });

  form.addEventListener("change", function () {
    applyFilters();
  });

  form.addEventListener("submit", function (event) {
    event.preventDefault();
    applyFilters();
  });

  if (openMobileBtn) {
    openMobileBtn.addEventListener("click", function () {
      setMobileFilterOpen(true);
    });
  }

  closeMobileBtns.forEach(function (btn) {
    btn.addEventListener("click", function () {
      setMobileFilterOpen(false);
    });
  });

  if (applyMobileBtn) {
    applyMobileBtn.addEventListener("click", function () {
      applyFilters();
      setMobileFilterOpen(false);
    });
  }

  document.addEventListener("keydown", function (event) {
    if (event.key === "Escape" && root.classList.contains("is-mobile-filter-open")) {
      setMobileFilterOpen(false);
    }
  });

  window.addEventListener("resize", function () {
    if (!isMobileFilter() && root.classList.contains("is-mobile-filter-open")) {
      setMobileFilterOpen(false);
    }
  });

  clearBtns.forEach(function (btn) {
    btn.addEventListener("click", function () {
      form.querySelectorAll("input[type='checkbox']").forEach(function (input) {
        input.checked = false;
      });
      applyFilters();
    });
  });

  applyFilters();
});
