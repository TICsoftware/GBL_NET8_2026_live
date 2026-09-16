document.addEventListener("DOMContentLoaded", function () {
  var root = document.querySelector(".product-filter-outer");
  if (!root) return;

  var form = root.querySelector("#product-filter");
  var cards = Array.prototype.slice.call(root.querySelectorAll(".product-card"));
  var countEl = root.querySelector("[data-product-count]");
  var chipsEl = root.querySelector("[data-filter-chips]");
  var emptyEl = root.querySelector("[data-filter-empty]");
  var clearBtn = root.querySelector("[data-clear-filters]");
  var openDropdown = null;

  function closeDropdown(wrap) {
    if (!wrap) return;
    wrap.classList.remove("is-open");
    var panel = wrap.querySelector(".pf-dropdown__panel");
    var trigger = wrap.querySelector(".pf-dropdown__trigger");
    if (panel) panel.setAttribute("aria-hidden", "true");
    if (trigger) trigger.setAttribute("aria-expanded", "false");
    if (openDropdown === wrap) openDropdown = null;
  }

  function hasChecked(wrap) {
    return Boolean(wrap && wrap.querySelector("input[type='checkbox']:checked"));
  }

  function openMenu(wrap) {
    if (hasChecked(wrap)) return;
    if (openDropdown && openDropdown !== wrap) closeDropdown(openDropdown);
    wrap.classList.add("is-open");
    var panel = wrap.querySelector(".pf-dropdown__panel");
    var trigger = wrap.querySelector(".pf-dropdown__trigger");
    if (panel) panel.setAttribute("aria-hidden", "false");
    if (trigger) trigger.setAttribute("aria-expanded", "true");
    openDropdown = wrap;
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
      if (locked && wrap.classList.contains("is-open")) closeDropdown(wrap);
    });
  }

  root.querySelectorAll(".pf-dropdown").forEach(function (wrap) {
    var trigger = wrap.querySelector(".pf-dropdown__trigger");
    var panel = wrap.querySelector(".pf-dropdown__panel");
    if (panel) panel.setAttribute("aria-hidden", "true");
    if (!trigger) return;

    trigger.addEventListener("click", function () {
      if (wrap.classList.contains("is-open")) closeDropdown(wrap);
      else if (!hasChecked(wrap)) openMenu(wrap);
    });
  });

  document.addEventListener("click", function (event) {
    if (!openDropdown) return;
    if (!openDropdown.contains(event.target)) closeDropdown(openDropdown);
  });

  document.addEventListener("keydown", function (event) {
    if (event.key !== "Escape" || !openDropdown) return;
    var trigger = openDropdown.querySelector(".pf-dropdown__trigger");
    closeDropdown(openDropdown);
    if (trigger) trigger.focus();
  });

  form.addEventListener("change", function (event) {
    var wrap = event.target && event.target.closest ? event.target.closest(".pf-dropdown") : null;
    if (wrap) closeDropdown(wrap);
    applyFilters();
  });

  form.addEventListener("submit", function (event) {
    event.preventDefault();
    applyFilters();
  });

  if (clearBtn) {
    clearBtn.addEventListener("click", function () {
      form.querySelectorAll("input[type='checkbox']").forEach(function (input) {
        input.checked = false;
      });
      if (openDropdown) closeDropdown(openDropdown);
      applyFilters();
    });
  }

  applyFilters();
});
