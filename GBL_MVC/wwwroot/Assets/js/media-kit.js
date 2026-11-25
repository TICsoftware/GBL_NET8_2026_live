/**
 * Media kit — sticky section nav, with the tab for the section
 * currently under the bar marked active while scrolling.
 */
(function () {
  function init() {
    var bar = document.querySelector(".mk-jump");
    var sentinel = document.querySelector(".mk-jump-sentinel");
    var sections = Array.prototype.map.call(document.querySelectorAll(".mk-jump__btn"), function (link) {
      var id = (link.getAttribute("href") || "").replace(/^#/, "");
      var section = id ? document.getElementById(id) : null;
      if (!section) return null;
      var head = section.querySelector("h2") || section;
      return { link: link, section: section, head: head };
    }).filter(Boolean);

    if (!bar || !sections.length) return;

    if (sentinel && "IntersectionObserver" in window) {
      var headerH = parseFloat(getComputedStyle(document.documentElement).getPropertyValue("--header-h")) || 88;
      var observer = new IntersectionObserver(function (entries) {
        bar.classList.toggle("is-stuck", !entries[0].isIntersecting);
      }, { rootMargin: "-" + headerH + "px 0px 0px 0px", threshold: 0 });
      observer.observe(sentinel);
    }

    var nav = document.querySelector(".mk-jump__nav");
    var prev = document.querySelector(".mk-jump__arrow--prev");
    var next = document.querySelector(".mk-jump__arrow--next");
    var mobileQuery = window.matchMedia("(max-width: 1199px)");
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
      next.disabled = index < 0 || index === sections.length - 1;
    }

    function currentScroll() {
      if (window.lenis && typeof window.lenis.scroll === "number") return window.lenis.scroll;
      return window.scrollY || document.documentElement.scrollTop || 0;
    }

    function scrollToSection(section) {
      var headerH = parseFloat(getComputedStyle(document.documentElement).getPropertyValue("--header-h")) || 88;
      var top = section.getBoundingClientRect().top + currentScroll() - headerH - bar.offsetHeight - 16;
      top = Math.max(0, top);
      if (window.lenis && typeof window.lenis.scrollTo === "function") {
        window.lenis.scrollTo(top, { duration: reduceMotion ? 0 : 0.9 });
      } else {
        window.scrollTo({ top: top, behavior: reduceMotion ? "auto" : "smooth" });
      }
      if (history.replaceState) history.replaceState(null, "", "#" + section.id);
    }

    sections.forEach(function (item) {
      item.link.addEventListener("click", function (event) {
        event.preventDefault();
        centerLink(item.link);
        scrollToSection(item.section);
      });
    });

    function goTo(index) {
      var item = sections[index];
      if (!item) return;
      centerLink(item.link);
      scrollToSection(item.section);
    }

    if (prev) prev.addEventListener("click", function () {
      var index = -1;
      sections.forEach(function (item, i) {
        if (item.section === activeSection) index = i;
      });
      if (index > 0) goTo(index - 1);
      else if (index < 0) goTo(0);
    });

    if (next) next.addEventListener("click", function () {
      var index = -1;
      sections.forEach(function (item, i) {
        if (item.section === activeSection) index = i;
      });
      if (index < 0) goTo(0);
      else if (index < sections.length - 1) goTo(index + 1);
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
      moveThumb();
    }

    // Green pill that slides under the active tab (same as .cg-jump__thumb)
    function moveThumb() {
      if (!nav) return;
      var thumb = nav.querySelector(".mk-jump__thumb");
      if (!thumb) return;
      var link = nav.querySelector(".mk-jump__btn.is-active");
      if (!link) {
        thumb.style.opacity = "0";
        nav.classList.remove("is-ready");
        return;
      }
      nav.classList.add("is-ready");
      thumb.style.opacity = "1";
      thumb.style.width = link.offsetWidth + "px";
      thumb.style.transform = "translateX(" + link.offsetLeft + "px)";
    }

    /* Scroll-spy (same rules as corporate governance): the first tab is active at the top;
       a tab activates when its section heading reaches ~45% down the viewport. */
    var ticking = false;
    function update() {
      ticking = false;
      if (nav && !mobileQuery.matches) {
        nav.style.paddingLeft = "";
        nav.style.paddingRight = "";
      }
      var line = window.innerHeight * 0.45;
      var current = null;
      var doc = document.documentElement;
      var atBottom = window.innerHeight + currentScroll() >= doc.scrollHeight - 4;

      if (atBottom) {
        current = sections[sections.length - 1].section;
      } else {
        sections.forEach(function (item) {
          if (item.head.getBoundingClientRect().top <= line) current = item.section;
        });
      }

      setActive(current || sections[0].section);
    }

    function onScroll() {
      if (ticking) return;
      ticking = true;
      window.requestAnimationFrame(update);
    }

    window.addEventListener("scroll", onScroll, { passive: true });
    window.addEventListener("resize", onScroll);
    if (window.lenis && typeof window.lenis.on === "function") {
      window.lenis.on("scroll", onScroll);
    }
    update();
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();

/**
 * Media kit cards — same row rise as the annual report cards
 * (annual-reports.js). Each row scrubs in as it reaches the viewport,
 * staggered left to right. Rows already on screen at load play once.
 */
(function () {
  var CARD_RISE = {
    y: 140,
    duration: 0.85,
    stagger: 0.1,
    scrub: 0.85,
    start: "top 90%",
    end: "top 38%"
  };
  var triggerPrefix = "mk-card-rise-";

  function init() {
    var grids = Array.prototype.slice.call(document.querySelectorAll(".mk-grid"));
    if (!grids.length) return;
    if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

    gsap.registerPlugin(ScrollTrigger);

    var stScroller = window.matchMedia("(max-width: 992px)").matches ? window : document.documentElement;
    var played = false;

    // Group a grid's cards into rows by where they sit, so the column count
    // follows the CSS at every breakpoint.
    function rowsOf(grid) {
      var rows = [];
      var lastTop = null;
      Array.prototype.forEach.call(grid.children, function (card) {
        if (card.offsetParent === null) return;
        var top = card.offsetTop;
        if (lastTop === null || Math.abs(top - lastTop) > 4) {
          rows.push([]);
          lastTop = top;
        }
        rows[rows.length - 1].push(card);
      });
      return rows;
    }

    function addRise(tl, row) {
      row.forEach(function (card, i) {
        tl.fromTo(
          card,
          { autoAlpha: 0, y: CARD_RISE.y },
          { autoAlpha: 1, y: 0, duration: CARD_RISE.duration, immediateRender: true },
          i * CARD_RISE.stagger
        );
      });
    }

    function setup() {
      ScrollTrigger.getAll().forEach(function (st) {
        var id = (st.vars && st.vars.id) || "";
        if (String(id).indexOf(triggerPrefix) === 0) st.kill();
      });

      var all = [];
      grids.forEach(function (grid) {
        Array.prototype.forEach.call(grid.children, function (card) { all.push(card); });
      });
      gsap.killTweensOf(all);
      gsap.set(all, { clearProps: "transform,opacity,visibility" });

      grids.forEach(function (grid, g) {
        rowsOf(grid).forEach(function (row, r) {
          var onScreen = row[0].getBoundingClientRect().top < window.innerHeight * 0.98;
          if (onScreen && !played) {
            gsap.fromTo(row, { autoAlpha: 0, y: 36 }, {
              autoAlpha: 1, y: 0, duration: 0.32, stagger: 0.04,
              ease: "power2.out", force3D: true
            });
            return;
          }
          if (onScreen) return; // already revealed; leave it in place on rebuild

          var tl = gsap.timeline({
            defaults: { force3D: true, ease: "power2.out" },
            scrollTrigger: {
              id: triggerPrefix + g + "-" + r,
              // The first card is translated, so the grid triggers the first row.
              trigger: r === 0 ? grid : row[0],
              scroller: stScroller,
              start: CARD_RISE.start,
              end: CARD_RISE.end,
              scrub: CARD_RISE.scrub,
              invalidateOnRefresh: true
            }
          });
          addRise(tl, row);
        });
      });
      played = true;
      requestAnimationFrame(function () { ScrollTrigger.refresh(); });
    }

    setup();
    window.addEventListener("load", function () { ScrollTrigger.refresh(); });

    // Columns change at breakpoints, so regroup rows when the width changes.
    var lastWidth = window.innerWidth;
    var resizeTimer = null;
    window.addEventListener("resize", function () {
      if (window.innerWidth === lastWidth) return;
      lastWidth = window.innerWidth;
      clearTimeout(resizeTimer);
      resizeTimer = setTimeout(setup, 250);
    });
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
