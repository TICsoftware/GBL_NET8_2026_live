/**
 * Corporate governance — sticky section nav, with the tab for the section
 * currently under the bar marked active while scrolling.
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

  function bindYearFilter(select, grid, empty) {
    if (!select || !grid) return;
    var items = grid.querySelectorAll("[data-year]");

    function apply() {
      var year = select.value;
      var visible = 0;
      items.forEach(function (item) {
        var years = (item.getAttribute("data-year") || "").split(/\s*,\s*/);
        var show = years.indexOf(year) !== -1;
        item.hidden = !show;
        if (show) {
          visible += 1;
          if (typeof gsap !== "undefined") {
            gsap.set(item, { clearProps: "transform,opacity,visibility" });
          }
        }
      });
      if (empty) empty.hidden = visible > 0;
      if (typeof ScrollTrigger !== "undefined") ScrollTrigger.refresh();
    }

    select.addEventListener("change", apply);
    apply();
  }

  function initYearFilter() {
    var sections = document.querySelectorAll("[data-year-section]");
    if (sections.length) {
      sections.forEach(function (section) {
        bindYearFilter(
          section.querySelector("[data-year-filter]"),
          section.querySelector(".cg-docs__grid, .cg-committees"),
          section.querySelector(".fh-year-filter__empty")
        );
      });
      return;
    }

    bindYearFilter(
      document.getElementById("fh-year"),
      document.querySelector(".cg-docs__grid"),
      document.querySelector(".fh-year-filter__empty")
    );
  }

  function init() {
    initYearFilter();
    initCardFill();
    var bar = document.querySelector(".cg-jump");
    var sentinel = document.querySelector(".cg-jump-sentinel");
    var sections = Array.prototype.map.call(document.querySelectorAll(".cg-jump__btn"), function (link) {
      var id = (link.getAttribute("href") || "").replace(/^#/, "");
      var section = id ? document.getElementById(id) : null;
      return section ? { link: link, section: section } : null;
    }).filter(Boolean);

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

    if (!sections.length) return;

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

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
