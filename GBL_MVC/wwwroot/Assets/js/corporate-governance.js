document.addEventListener("DOMContentLoaded", function () {
  var page = document.querySelector(".cg-page");
  if (!page) return;

  initJumpNav(page);
  initSelects(page);
  initDocGrids(page);

  function initDocGrids(root) {
    root.querySelectorAll("[data-cg-docs-grid]").forEach(function (grid) {
      var size = parseInt(grid.getAttribute("data-page-size") || "12", 10);
      var section = grid.closest(".cg-docs");
      var wrap = section && section.querySelector("[data-cg-load-more-wrap]");
      var btn = wrap && wrap.querySelector("[data-cg-load-more]");
      var empty = section && section.querySelector("[data-cg-empty]");
      var yearSelect = section && section.querySelector("[data-cg-year]");
      var cards = Array.prototype.slice.call(grid.children);
      var shown = size;

      function selectedYear() {
        if (yearSelect && yearSelect.value) return yearSelect.value;
        return grid.getAttribute("data-cg-year-value") || "";
      }

      function matchingCards() {
        var year = selectedYear();
        return cards.filter(function (card) {
          var cardYear = card.getAttribute("data-year") || "";
          return !year || cardYear === year;
        });
      }

      function apply() {
        var year = selectedYear();
        grid.setAttribute("data-cg-year-value", year);
        var match = matchingCards();
        cards.forEach(function (card) {
          card.hidden = true;
        });
        match.forEach(function (card, index) {
          card.hidden = index >= shown;
        });
        if (empty) empty.hidden = match.length > 0;
        if (wrap) wrap.hidden = shown >= match.length;
        if (typeof ScrollTrigger !== "undefined") {
          ScrollTrigger.refresh();
        }
      }

      apply();

      if (yearSelect) {
        yearSelect.addEventListener("change", function () {
          shown = size;
          apply();
        });
      }

      if (btn) {
        btn.addEventListener("click", function () {
          shown += size;
          apply();
        });
      }
    });
  }

  function initJumpNav(root) {
    var jump = root.querySelector(".cg-jump");
    var nav = root.querySelector(".cg-jump__nav");
    var buttons = Array.prototype.slice.call(root.querySelectorAll(".cg-jump__btn"));

    function hashId() {
      return (location.hash || "").replace(/^#/, "");
    }

    function setActive(id, scroll) {
      if (!id) return;
      buttons.forEach(function (btn) {
        btn.classList.toggle("is-active", (btn.getAttribute("href") || "") === "#" + id);
      });
      if (scroll) {
        var target = document.getElementById(id);
        if (target) {
          target.scrollIntoView({ behavior: "smooth", block: "start" });
        }
      }
    }

    function applyHash(scroll) {
      var id = hashId();
      if (id && document.getElementById(id)) {
        setActive(id, scroll);
        return;
      }
      if (buttons[0]) {
        setActive((buttons[0].getAttribute("href") || "").replace(/^#/, ""), false);
      }
    }

    buttons.forEach(function (btn) {
      btn.addEventListener("click", function (event) {
        var id = (btn.getAttribute("href") || "").replace(/^#/, "");
        if (!id) return;
        event.preventDefault();
        if (history.replaceState) {
          history.replaceState(null, "", "#" + id);
        } else {
          location.hash = id;
        }
        setActive(id, true);
      });
    });

    window.addEventListener("hashchange", function () {
      applyHash(true);
    });

    applyHash(true);

    var sentinel = root.querySelector(".cg-jump-sentinel");
    if (jump && sentinel && "IntersectionObserver" in window) {
      new IntersectionObserver(function (entries) {
        jump.classList.toggle("is-stuck", entries[0] && !entries[0].isIntersecting);
      }).observe(sentinel);
    }

    var prev = root.querySelector(".cg-jump__arrow--prev");
    var next = root.querySelector(".cg-jump__arrow--next");
    if (nav && prev && next) {
      prev.addEventListener("click", function () {
        nav.scrollBy({ left: -180, behavior: "smooth" });
      });
      next.addEventListener("click", function () {
        nav.scrollBy({ left: 180, behavior: "smooth" });
      });
    }
  }

  function initSelects(root) {
    root.querySelectorAll(".cselect").forEach(function (wrap) {
      var native = wrap.querySelector(".cselect-native");
      if (!native || wrap.dataset.cselectReady === "1") return;
      wrap.dataset.cselectReady = "1";

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
        else openSelect(wrap);
      });
    });

    var openMenu = null;

    function closeSelect(wrap) {
      if (!wrap) return;
      wrap.classList.remove("is-open");
      var menu = wrap.querySelector(".cselect-menu");
      var trigger = wrap.querySelector(".cselect-trigger");
      if (menu) menu.hidden = true;
      if (trigger) trigger.setAttribute("aria-expanded", "false");
      if (openMenu === wrap) openMenu = null;
    }

    function openSelect(wrap) {
      if (openMenu && openMenu !== wrap) closeSelect(openMenu);
      wrap.classList.add("is-open");
      var menu = wrap.querySelector(".cselect-menu");
      var trigger = wrap.querySelector(".cselect-trigger");
      if (menu) menu.hidden = false;
      if (trigger) trigger.setAttribute("aria-expanded", "true");
      openMenu = wrap;
    }

    document.addEventListener("click", function (event) {
      if (!openMenu) return;
      if (!openMenu.contains(event.target)) closeSelect(openMenu);
    });

    document.addEventListener("keydown", function (event) {
      if (event.key === "Escape" && openMenu) {
        var trigger = openMenu.querySelector(".cselect-trigger");
        closeSelect(openMenu);
        if (trigger) trigger.focus();
      }
    });
  }
});
