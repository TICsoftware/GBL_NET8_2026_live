document.addEventListener("DOMContentLoaded", function () {
  var grid = document.querySelector(".giveback-grid");
  if (!grid) return;

  var cells = Array.from(grid.querySelectorAll(":scope > .giveback-cell"));
  if (cells.length < 2) return;

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var desktopTriggers = [];
  var swiper = null;
  var controlsEl = null;
  var wrapperEl = null;
  var sliderReady = false;
  var isMobileMode = null;
  var resizeTimer = 0;
  var MOBILE_MAX = 767;

  function isMobileView() {
    return window.innerWidth <= MOBILE_MAX;
  }

  /* Mobile slider styles live here so the grid CSS file stays desktop-first. */
  function injectSliderStyles() {
    var style = document.getElementById("giveback-slider-styles");
    if (!style) {
      style = document.createElement("style");
      style.id = "giveback-slider-styles";
      document.head.appendChild(style);
    }
    style.textContent = [
      "/* Give Back — mobile Swiper (arrows + Instagram-style dots) */",
      "@media (max-width: 767px) {",
      "  .giveback-grid.giveback-grid--slider {",
      "    display: block;",
      "    overflow: hidden;",
      "    width: 100%;",
      "  }",
      "  .giveback-grid.giveback-grid--slider .swiper-wrapper {",
      "    display: flex;",
      "    align-items: stretch;",
      "  }",
      "  .giveback-grid.giveback-grid--slider .giveback-cell.swiper-slide {",
      "    grid-column: auto;",
      "    height: auto;",
      "    min-height: 11.5rem;",
      "    margin-right: 0;",
      "    margin-bottom: 0;",
      "  }",
      "  .giveback-grid.giveback-grid--slider .giveback-cell--media {",
      "    min-height: 11.5rem;",
      "    aspect-ratio: auto;",
      "    overflow: hidden;",
      "  }",
      "  /* 200px centered bar: [prev] [dots] [next]. Dots stay in the middle cell. */",
      "  .giveback-slider-controls {",
      "    display: grid;",
      "    grid-template-columns: 36px minmax(0, 1fr) 36px;",
      "    align-items: center;",
      "    column-gap: 8px;",
      "    width: 200px;",
      "    max-width: 100%;",
      "    margin: 1.1rem auto 0;",
      "  }",
      "  .giveback-nav {",
      "    width: 36px;",
      "    height: 36px;",
      "    padding: 0;",
      "    border: 1px solid var(--line, #e4e4e4);",
      "    border-radius: 50%;",
      "    background: var(--color-white, #fff);",
      "    color: var(--color-primary, #282b31);",
      "    display: inline-flex;",
      "    align-items: center;",
      "    justify-content: center;",
      "    cursor: pointer;",
      "    position: relative;",
      "    z-index: 2;",
      "  }",
      "  .giveback-nav svg {",
      "    width: 0.85rem;",
      "    height: 0.85rem;",
      "    display: block;",
      "  }",
      "  .giveback-nav.swiper-button-disabled {",
      "    opacity: 0.35;",
      "    cursor: default;",
      "    pointer-events: none;",
      "  }",
      "  .giveback-pagination-wrap {",
      "    position: relative;",
      "    overflow: hidden;",
      "    height: 20px;",
      "    min-width: 0;",
      "  }",
      "  /* Swiper pagination is position:absolute by default — that overlapped the left arrow */",
      "  .giveback-slider-controls .giveback-pagination.swiper-pagination {",
      "    position: relative !important;",
      "    left: 0 !important;",
      "    right: auto !important;",
      "    top: 6px !important;",
      "    bottom: auto !important;",
      "    margin: 0 auto;",
      "    height: 8px;",
      "    text-align: center;",
      "  }",
      "  .giveback-pagination.swiper-pagination-bullets-dynamic {",
      "    overflow: hidden;",
      "    font-size: 0;",
      "  }",
      "  .giveback-pagination .swiper-pagination-bullet {",
      "    width: 6px;",
      "    height: 6px;",
      "    margin: 0 3px !important;",
      "    background: #cfcfcf;",
      "    opacity: 1;",
      "    vertical-align: middle;",
      "  }",
      "  .giveback-pagination .swiper-pagination-bullet-active {",
      "    background: var(--color-primary, #282b31);",
      "  }",
      "}",
      "@media (min-width: 768px) {",
      "  .giveback-slider-controls { display: none; }",
      "}",
    ].join("\n");
  }

  function isInViewport(el) {
    var rect = el.getBoundingClientRect();
    var vh = window.innerHeight || document.documentElement.clientHeight;
    return rect.top < vh * 0.8 && rect.bottom > 40;
  }

  function killDesktopAnim() {
    desktopTriggers.forEach(function (st) {
      if (st && typeof st.kill === "function") st.kill();
    });
    desktopTriggers = [];
    if (typeof gsap !== "undefined") {
      gsap.set(cells, { clearProps: "transform,opacity,visibility" });
    }
  }

  function bindGroup(items, fromX, stScroller) {
    if (!items.length) return;

    gsap.set(items, { autoAlpha: 0, x: fromX, force3D: true });

    var tl = gsap.timeline({
      paused: true,
      defaults: { force3D: true },
    });

    tl.to(items, {
      autoAlpha: 1,
      x: 0,
      duration: 0.8,
      ease: "power3.out",
      stagger: { each: 0.15, from: "start", ease: "power1.out" },
      overwrite: "auto",
    });

    function playReveal() {
      tl.timeScale(1).play();
    }

    function exitReveal() {
      tl.timeScale(0.75).reverse();
    }

    var trigger = items[0];

    desktopTriggers.push(
      ScrollTrigger.create({
        trigger: trigger,
        scroller: stScroller,
        start: "top 80%",
        end: "bottom 20%",
        invalidateOnRefresh: true,
        onEnter: playReveal,
        onEnterBack: playReveal,
        onLeave: exitReveal,
        onLeaveBack: exitReveal,
      })
    );

    requestAnimationFrame(function () {
      if (isInViewport(trigger)) playReveal();
    });
  }

  function initDesktopAnim() {
    if (reduceMotion || typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") {
      return;
    }

    gsap.registerPlugin(ScrollTrigger);

    var firstFive = cells.slice(0, 5);
    var lastFive = cells.slice(5);
    var isTablet = window.matchMedia("(max-width: 992px)").matches;
    var stScroller = isTablet ? window : document.documentElement;

    bindGroup(firstFive, 160, stScroller);
    bindGroup(lastFive, -160, stScroller);

    requestAnimationFrame(function () {
      ScrollTrigger.refresh();
    });
  }

  function wrapForSlider() {
    if (wrapperEl) return;

    wrapperEl = document.createElement("div");
    wrapperEl.className = "swiper-wrapper";
    cells.forEach(function (cell) {
      cell.classList.add("swiper-slide");
      wrapperEl.appendChild(cell);
    });
    grid.appendChild(wrapperEl);
    grid.classList.add("swiper", "giveback-grid--slider");

    controlsEl = document.createElement("div");
    controlsEl.className = "giveback-slider-controls";
    controlsEl.innerHTML =
      '<button type="button" class="giveback-nav giveback-nav--prev" aria-label="Previous slide">' +
      '<svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M15.5 19.5 8 12l7.5-7.5 1.4 1.4L10.8 12l6.1 6.1z"/></svg>' +
      "</button>" +
      '<div class="giveback-pagination-wrap"><div class="giveback-pagination"></div></div>' +
      '<button type="button" class="giveback-nav giveback-nav--next" aria-label="Next slide">' +
      '<svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="m8.5 4.5 7.5 7.5-7.5 7.5-1.4-1.4 6.1-6.1-6.1-6.1z"/></svg>' +
      "</button>";
    grid.insertAdjacentElement("afterend", controlsEl);
  }

  function unwrapSlider() {
    if (!wrapperEl) return;

    cells.forEach(function (cell) {
      cell.classList.remove("swiper-slide");
      grid.appendChild(cell);
    });
    wrapperEl.remove();
    wrapperEl = null;
    if (controlsEl) {
      controlsEl.remove();
      controlsEl = null;
    }
    grid.classList.remove("swiper", "giveback-grid--slider");
  }

  function initMobileSlider() {
    if (sliderReady || typeof Swiper === "undefined") return;

    injectSliderStyles();
    wrapForSlider();

    swiper = new Swiper(grid, {
      slidesPerView: 1,
      spaceBetween: 0,
      speed: reduceMotion ? 0 : 450,
      watchOverflow: true,
      autoHeight: true,
      observer: true,
      observeParents: true,
      resizeObserver: true,
      navigation: {
        prevEl: controlsEl.querySelector(".giveback-nav--prev"),
        nextEl: controlsEl.querySelector(".giveback-nav--next"),
      },
      pagination: {
        el: controlsEl.querySelector(".giveback-pagination"),
        clickable: true,
        dynamicBullets: true,
        dynamicMainBullets: 5,
      },
    });

    sliderReady = true;
  }

  function destroyMobileSlider() {
    if (swiper) {
      swiper.destroy(true, true);
      swiper = null;
    }
    unwrapSlider();
    sliderReady = false;
  }

  function applyMode() {
    var wantMobile = isMobileView();

    /* Same breakpoint: keep slider in sync while resizing inside mobile */
    if (wantMobile === isMobileMode) {
      if (wantMobile && swiper) {
        swiper.update();
        swiper.updateAutoHeight(0);
      }
      return;
    }

    isMobileMode = wantMobile;

    if (wantMobile) {
      killDesktopAnim();
      initMobileSlider();
    } else {
      destroyMobileSlider();
      initDesktopAnim();
    }
  }

  applyMode();

  window.addEventListener("resize", function () {
    window.clearTimeout(resizeTimer);
    resizeTimer = window.setTimeout(applyMode, 120);
  });

  window.addEventListener("load", function () {
    applyMode();
    if (!isMobileView() && typeof ScrollTrigger !== "undefined") {
      ScrollTrigger.refresh();
    }
  });
});
