document.addEventListener("DOMContentLoaded", function () {
  var form = document.getElementById("industry-filter");
  if (!form) return;

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

  form.querySelectorAll(".cselect").forEach(function (wrap) {
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
    if (event.key !== "Escape" || !openSelect) return;
    var trigger = openSelect.querySelector(".cselect-trigger");
    closeSelect(openSelect);
    if (trigger) trigger.focus();
  });

  form.addEventListener("submit", function (event) {
    event.preventDefault();
    form.dispatchEvent(
      new CustomEvent("industryfilter:apply", {
        bubbles: true,
        detail: {
          industry: form.industry.value,
          application: form.application.value,
          productCategories: form.productCategories.value
        }
      })
    );
  });
});
