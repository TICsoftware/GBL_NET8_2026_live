document.addEventListener("DOMContentLoaded", function () {
  var grid = document.querySelector(".giveback-grid");
  if (!grid) return;

  var cells = Array.from(grid.querySelectorAll(":scope > .giveback-cell"));
  var slideCells = cells.filter(function (cell) {
    return !cell.classList.contains("giveback-cell--media");
  });
  if (slideCells.length < 2) return;

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var desktopAnims = [];
  var swiper = null;
  var controlsEl = null;
  var wrapperEl = null;
  var sliderReady = false;
  var isMobileMode = null;
  var resizeTimer = 0;
  var SLIDER_MAX = 1023;

  function isSliderView() {
    return window.innerWidth <= SLIDER_MAX;
  }

  function killDesktopAnim() {
    desktopAnims.forEach(function (anim) {
      anim.observers.forEach(function (io) { io.disconnect(); });
      anim.tl.kill();
    });
    desktopAnims = [];
    if (typeof gsap !== "undefined") {
      gsap.set(cells, { clearProps: "transform,opacity,visibility" });
    }
  }

  /* Plays once the row reaches 85% down the viewport; resets only after it has
     dropped fully below the viewport, so it plays again on the way back down. */
  function bindGroup(items, fromX) {
    if (!items.length) return;

    var tl = gsap.timeline({
      paused: true,
      defaults: { force3D: true, ease: "power2.out" },
    });

    /* Lead with the cell nearest the direction of travel, so cells still
       sliding in never pass over one that has already landed. */
    var order = fromX < 0 ? items.slice().reverse() : items;

    order.forEach(function (item, i) {
      tl.fromTo(
        item,
        { autoAlpha: 0, x: fromX },
        { autoAlpha: 1, x: 0, duration: 1, immediateRender: true },
        i * 0.15
      );
    });

    /* Side margins keep the cells counted as on screen while shifted sideways. */
    var playIo = new IntersectionObserver(function (entries) {
      entries.forEach(function (entry) {
        if (entry.isIntersecting) tl.play();
      });
    }, { rootMargin: "0px 200px -15% 200px" });

    var resetIo = new IntersectionObserver(function (entries) {
      entries.forEach(function (entry) {
        if (!entry.isIntersecting && entry.boundingClientRect.top > 0) tl.pause(0);
      });
    }, { rootMargin: "0px 200px 0px 200px" });

    playIo.observe(items[0]);
    resetIo.observe(items[0]);

    desktopAnims.push({ tl: tl, observers: [playIo, resetIo] });
  }

  function initDesktopAnim() {
    if (reduceMotion || typeof gsap === "undefined" || !("IntersectionObserver" in window)) {
      return;
    }

    bindGroup(cells.slice(0, 5), 160);
    bindGroup(cells.slice(5), -160);
  }

  function wrapForSlider() {
    if (wrapperEl) return;

    wrapperEl = document.createElement("div");
    wrapperEl.className = "swiper-wrapper";
    slideCells.forEach(function (cell) {
      cell.classList.add("swiper-slide");
      wrapperEl.appendChild(cell);
    });
    grid.appendChild(wrapperEl);
    grid.classList.add("swiper", "giveback-grid--slider");

    controlsEl = document.createElement("div");
    controlsEl.className = "giveback-slider-controls";
    controlsEl.innerHTML =
      '<button type="button" class="giveback-nav giveback-nav--prev" aria-label="Previous slide">' +
      '<span aria-hidden="true">&larr;</span></button>' +
      '<div class="swiper-pagination giveback-pagination"></div>' +
      '<button type="button" class="giveback-nav giveback-nav--next" aria-label="Next slide">' +
      '<span aria-hidden="true">&rarr;</span></button>';
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

    wrapForSlider();

    var prevEl = controlsEl.querySelector(".giveback-nav--prev");
    var nextEl = controlsEl.querySelector(".giveback-nav--next");
    var pager = controlsEl.querySelector(".giveback-pagination");

    swiper = new Swiper(grid, {
      slidesPerView: 1.5,
      spaceBetween: 10,
      speed: reduceMotion ? 0 : 450,
      watchOverflow: true,
      observer: true,
      observeParents: true,
      resizeObserver: true,
      breakpoints: {
        768: {
          slidesPerView: 2.75,
          spaceBetween: 12,
        },
      },
      navigation: {
        prevEl: prevEl,
        nextEl: nextEl,
      },
      pagination: {
        el: pager,
        clickable: true,
      },
    });

    controlsEl.appendChild(prevEl);
    controlsEl.appendChild(pager);
    controlsEl.appendChild(nextEl);

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
    var wantSlider = isSliderView();

    if (wantSlider === isMobileMode) {
      if (wantSlider && swiper) {
        swiper.update();
      }
      return;
    }

    isMobileMode = wantSlider;

    if (wantSlider) {
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

  window.addEventListener("load", applyMode);
});
