/**
 * Corporate governance — sticky section nav, year filter, load more, cselect.
 */
(function () {
  function initCardFill() {
    function bind(selector) {
      document.querySelectorAll(selector).forEach(function (card) {
        function placeFill(e) {
          var rect = card.getBoundingClientRect();
          var x = e ? e.clientX - rect.left : rect.width / 2;
          var y = e ? e.clientY - rect.top : rect.height / 2;
          x = Math.min(Math.max(x, 0), rect.width);
          y = Math.min(Math.max(y, 0), rect.height);
          var radius = Math.ceil(Math.hypot(Math.max(x, rect.width - x), Math.max(y, rect.height - y))) + 16;
          card.style.setProperty("--hx", x + "px");
          card.style.setProperty("--hy", y + "px");
          card.style.setProperty("--hr", radius + "px");
        }

        placeFill(null);
        card.addEventListener("mouseenter", function (e) {
          placeFill(e);
          card.classList.add("is-filled");
        });
        card.addEventListener("mouseleave", function () {
          card.classList.remove("is-filled");
        });
      });
    }

    bind(".cg-doc");
  }

  function initSelects(root) {
    if (!root) return;
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

  function initDocGrids(root) {
    if (!root) return;
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
        var match = cards.filter(function (card) {
          var cardYear = card.getAttribute("data-year") || "";
          return !year || !cardYear || cardYear === year;
        });
        if (year && match.length === 0) {
          return cards;
        }
        return match;
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

  function initJumpNav() {
    var bar = document.querySelector(".cg-jump");
    var sentinel = document.querySelector(".cg-jump-sentinel");
    var sections = Array.prototype.map.call(document.querySelectorAll(".cg-jump__btn"), function (link) {
      var id = (link.getAttribute("href") || "").replace(/^#/, "");
      var section = id ? document.getElementById(id) : null;
      return section ? { link: link, section: section } : null;
    }).filter(Boolean);

    if (!sections.length) return;

    if (bar && sentinel && "IntersectionObserver" in window) {
      var headerH = parseFloat(getComputedStyle(document.documentElement).getPropertyValue("--header-h")) || 88;
      var observer = new IntersectionObserver(function (entries) {
        bar.classList.toggle("is-stuck", !entries[0].isIntersecting);
      }, { rootMargin: "-" + headerH + "px 0px 0px 0px", threshold: 0 });

      observer.observe(sentinel);
    }

    var nav = document.querySelector(".cg-jump__nav");
    var prev = document.querySelector(".cg-jump__arrow--prev");
    var next = document.querySelector(".cg-jump__arrow--next");
    var mobileQuery = window.matchMedia("(max-width: 640px)");
    var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    var activeSection = null;

    function centerLink(link) {
      if (!nav) return;
      var isFirst = !!(link && sections[0] && link === sections[0].link);
      if (!mobileQuery.matches || !link || isFirst) {
        nav.style.paddingLeft = "";
        nav.style.paddingRight = "";
        nav.scrollTo({
          left: 0,
          behavior: mobileQuery.matches && link && !reduceMotion ? "smooth" : "auto"
        });
        return;
      }
      var half = nav.clientWidth / 2;
      nav.style.paddingLeft = half + "px";
      nav.style.paddingRight = half + "px";
      var left = link.offsetLeft + link.offsetWidth / 2 - half;
      nav.scrollTo({ left: Math.max(0, left), behavior: reduceMotion ? "auto" : "smooth" });
    }

    function syncArrows(section) {
      if (!prev || !next) return;
      var index = -1;
      sections.forEach(function (item, i) {
        if (item.section === section) index = i;
      });
      prev.disabled = index <= 0;
      next.disabled = index === sections.length - 1;
    }

    function goTo(index) {
      var item = sections[index];
      if (!item) return;
      centerLink(item.link);
      item.section.scrollIntoView({ behavior: reduceMotion ? "auto" : "smooth" });
    }

    if (prev) prev.addEventListener("click", function () {
      var index = -1;
      sections.forEach(function (item, i) {
        if (item.section === activeSection) index = i;
      });
      if (index > 0) goTo(index - 1);
    });

    if (next) next.addEventListener("click", function () {
      var index = -1;
      sections.forEach(function (item, i) {
        if (item.section === activeSection) index = i;
      });
      if (index < sections.length - 1) goTo(index + 1);
    });

    function setActive(section) {
      var changed = activeSection !== section;
      activeSection = section;
      sections.forEach(function (item) {
        var on = item.section === section;
        item.link.classList.toggle("is-active", on);
        if (on) item.link.setAttribute("aria-current", "true");
        else item.link.removeAttribute("aria-current");
        if (on && changed) centerLink(item.link);
      });
      if (!section && changed) centerLink(null);
      syncArrows(section);
    }

    var ticking = false;
    function update() {
      ticking = false;
      if (nav && !mobileQuery.matches) {
        nav.style.paddingLeft = "";
        nav.style.paddingRight = "";
      }
      var line = 305;
      var current = null;
      var doc = document.documentElement;
      var atBottom = window.innerHeight + window.scrollY >= doc.scrollHeight - 4;

      if (atBottom) {
        current = sections[sections.length - 1].section;
      } else {
        sections.forEach(function (item) {
          if (item.section.getBoundingClientRect().top <= line) current = item.section;
        });
      }

      setActive(current);
    }

    function onScroll() {
      if (ticking) return;
      ticking = true;
      window.requestAnimationFrame(update);
    }

    window.addEventListener("scroll", onScroll, { passive: true });
    window.addEventListener("resize", onScroll);
    update();
  }

  function init() {
    var page = document.querySelector(".cg-page") || document;
    initSelects(page);
    initDocGrids(page);
    initCardFill();
    initJumpNav();
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
