document.addEventListener("DOMContentLoaded", function () {
  /* ---------------------------------------
     Industry tab cards only (.industry-product-grid).
     Same CARD_RISE as homepage card-animation.js /
     industries-script.js:
     fade + rise, scrubbed to scroll, stagger
     within each row. Other pages are untouched.
  --------------------------------------- */
  var tabRoot = document.querySelector(".tab-outer");
  var contentRoot = document.querySelector(".industries-tab-content");
  if (!tabRoot || !contentRoot) return;

  var tabsWrap = tabRoot.querySelector(".industry-tabs");
  var thumb = tabRoot.querySelector(".industry-tabs__thumb");
  var tabs = Array.from(tabRoot.querySelectorAll("[role='tab']"));
  var panels = Array.from(contentRoot.querySelectorAll("[data-tab-panel]"));

  var CARD_RISE = {
    y: 140,           /* start offset (px) below the rest position */
    duration: 1.15,   /* rise length on the scrub timeline */
    stagger: 0.28,    /* delay between cards in the same row */
    scrub: 1.15,      /* catch-up so motion follows the scrollbar */
    listStart: "top 90%",
    listEnd: "top 38%"
  };

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var canAnimate =
    typeof gsap !== "undefined" &&
    typeof ScrollTrigger !== "undefined" &&
    !reduceMotion;

  if (canAnimate) gsap.registerPlugin(ScrollTrigger);

  var isMobile = window.matchMedia("(max-width: 992px)").matches;
  /* Desktop uses Lenis on documentElement; mobile uses native window scroll. */
  var stScroller = isMobile ? window : document.documentElement;
  var productAnims = [];

  /** Grid columns — matches industries-inside.css breakpoints. */
  function columns() {
    if (window.matchMedia("(max-width: 1179px)").matches) return 1; /* mobile */
    if (window.matchMedia("(max-width: 1279px)").matches) return 2; /* 1180–1279 */
    return 3; /* desktop 1280+ */
  }

  /** First row already on screen (tab change while scrolled). */
  function isInView(el) {
    var rect = el.getBoundingClientRect();
    var vh = window.innerHeight || document.documentElement.clientHeight;
    return rect.top < vh * 0.9 && rect.bottom > 40;
  }

  /** Slide the olive pill under the active tab. */
  function moveThumb(tab, animate) {
    if (!thumb || !tabsWrap || !tab) return;

    var wrapRect = tabsWrap.getBoundingClientRect();
    var tabRect = tab.getBoundingClientRect();
    var x = tabRect.left - wrapRect.left;
    var width = tabRect.width;

    if (!canAnimate || !animate) {
      if (canAnimate) {
        gsap.set(thumb, { x: x, width: width });
      } else {
        thumb.style.width = width + "px";
        thumb.style.transform = "translateX(" + x + "px)";
      }
      return;
    }

    gsap.to(thumb, {
      x: x,
      width: width,
      duration: 0.45,
      ease: "power3.out",
      overwrite: true
    });
  }

  /** Remove product-card timelines / ScrollTriggers from the previous tab panel. */
  function killProductAnims() {
    productAnims.forEach(function (item) {
      if (item && item.kill) item.kill();
    });
    productAnims = [];
  }

  /**
   * Same scrubbed CARD_RISE as industries-script.js for one visual row.
   */
  function addScrubRow(row, rowIndex) {
    var tl = gsap.timeline({
      defaults: { force3D: true, ease: "power2.out" },
      scrollTrigger: {
        id: "industry-tab-card-rise-row-" + rowIndex,
        trigger: row[0],
        scroller: stScroller,
        start: CARD_RISE.listStart,
        end: CARD_RISE.listEnd,
        scrub: CARD_RISE.scrub,
        invalidateOnRefresh: true
      }
    });

    /* Stagger left-to-right inside the row (same as homepage card-animation.js). */
    row.forEach(function (card, i) {
      tl.fromTo(
        card,
        { autoAlpha: 0, y: CARD_RISE.y },
        {
          autoAlpha: 1,
          y: 0,
          duration: CARD_RISE.duration,
          immediateRender: true
        },
        i * CARD_RISE.stagger
      );
    });

    if (tl.scrollTrigger) productAnims.push(tl.scrollTrigger);
    return tl;
  }

  /**
   * Scroll: same fade + rise + scrub as industries-script.js.
   * Tab change: first row in view plays fade + rise only (no travel).
   * After that, scrub is attached so scroll back (bottom → top)
   * uses the same CARD_RISE travel as other cards.
   */
  function bindProductRise(panel, replay) {
    killProductAnims();
    if (!canAnimate || !panel) return;

    var grid = panel.querySelector(".industry-product-grid");
    var cards = grid
      ? Array.from(grid.querySelectorAll(".industry-product-card"))
      : [];
    if (!cards.length) return;

    var cols = columns();

    for (var r = 0; r < cards.length; r += cols) {
      var row = cards.slice(r, r + cols);
      var rowIndex = r / cols;
      var playIntro = replay && isInView(row[0]);

      if (playIntro) {
        gsap.set(row, { autoAlpha: 0, y: CARD_RISE.y, force3D: true });

        var introTl = gsap.timeline({
          defaults: { force3D: true, ease: "power2.out" }
        });

        row.forEach(function (card, i) {
          introTl.to(
            card,
            {
              autoAlpha: 1,
              y: 0,
              duration: CARD_RISE.duration
            },
            i * CARD_RISE.stagger
          );
        });

        productAnims.push(introTl);

        (function (rowCards, idx) {
          introTl.eventCallback("onComplete", function () {
            addScrubRow(rowCards, idx);
            refreshTriggers();
          });
        })(row, rowIndex);
        continue;
      }

      addScrubRow(row, rowIndex);
    }

    refreshTriggers();
  }

  /** Recalc start/end after layout, images, or font load. */
  function refreshTriggers() {
    ScrollTrigger.refresh();
  }

  /** Show the panel that matches the clicked tab. */
  function activateTab(tab, animate) {
    var id = tab.getAttribute("data-tab");
    var nextPanel = null;

    tabs.forEach(function (btn) {
      var on = btn === tab;
      btn.classList.toggle("is-active", on);
      btn.setAttribute("aria-selected", on ? "true" : "false");
    });

    panels.forEach(function (panel) {
      var on = panel.getAttribute("data-tab-panel") === id;
      panel.classList.toggle("is-active", on);
      panel.hidden = !on;
      if (on) nextPanel = panel;
    });

    moveThumb(tab, animate);

    if (nextPanel && canAnimate) {
      requestAnimationFrame(function () {
        bindProductRise(nextPanel, animate);
      });
    }
  }

  tabs.forEach(function (tab) {
    tab.addEventListener("click", function () {
      if (tab.classList.contains("is-active")) return;
      activateTab(tab, true);
    });
  });

  var startTab = tabs.find(function (tab) {
    return tab.classList.contains("is-active");
  }) || tabs[0];

  if (startTab) activateTab(startTab, false);

  window.addEventListener("load", refreshTriggers);
  window.setTimeout(refreshTriggers, 400);

  window.addEventListener("resize", function () {
    var active = tabs.find(function (tab) {
      return tab.classList.contains("is-active");
    });
    if (active) moveThumb(active, false);
  });

  /* ---------------------------------------
     Product media parallax — independent of
     CARD_RISE / tab thumb / other handlers.
  --------------------------------------- */
  var mediaParallax = [];

  function killMediaParallax() {
    mediaParallax.forEach(function (item) {
      if (item && item.kill) item.kill();
    });
    mediaParallax = [];
  }

  function bindMediaParallax(panel) {
    killMediaParallax();
    if (!canAnimate || !panel) return;

    var wraps = panel.querySelectorAll(".industry-product-card__media");
    wraps.forEach(function (wrap) {
      var img = wrap.querySelector("img");
      if (!img) return;

      var tween = gsap.fromTo(
        img,
        { yPercent: -13 },
        {
          yPercent: 13,
          ease: "none",
          force3D: true,
          scrollTrigger: {
            trigger: wrap,
            scroller: stScroller,
            start: "top bottom",
            end: "bottom top",
            scrub: 0.8,
            invalidateOnRefresh: true
          }
        }
      );

      if (tween.scrollTrigger) mediaParallax.push(tween.scrollTrigger);
    });
  }

  function activeProductPanel() {
    return panels.find(function (panel) {
      return panel.classList.contains("is-active");
    });
  }

  tabs.forEach(function (tab) {
    tab.addEventListener("click", function () {
      requestAnimationFrame(function () {
        bindMediaParallax(activeProductPanel());
      });
    });
  });

  bindMediaParallax(activeProductPanel());
});
