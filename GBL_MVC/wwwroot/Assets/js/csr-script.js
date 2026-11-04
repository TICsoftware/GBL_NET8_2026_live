document.addEventListener("DOMContentLoaded", function () {
  var stats = document.querySelector(".csr-stats");
  if (!stats) return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  var items = stats.querySelectorAll(".csr-stat");
  if (!items.length) return;

  function inlineIcon(img) {
    return fetch(img.src)
      .then(function (res) { return res.text(); })
      .then(function (text) {
        var holder = document.createElement("div");
        holder.innerHTML = text.trim();
        var svg = holder.querySelector("svg");
        if (!svg) return;
        svg.querySelectorAll("[id]").forEach(function (el) { el.removeAttribute("id"); });
        svg.classList.add("csr-stat__icon");
        svg.setAttribute("aria-hidden", "true");
        svg.setAttribute("focusable", "false");
        img.replaceWith(svg);
      })
      .catch(function () {});
  }

  Promise.all(Array.from(stats.querySelectorAll("img.csr-stat__icon")).map(inlineIcon)).then(function () {
    gsap.registerPlugin(ScrollTrigger);
    var stScroller = window.matchMedia("(max-width: 992px)").matches
      ? window
      : document.documentElement;

    var tl = gsap.timeline({
      defaults: { force3D: true, ease: "power2.out" },
      scrollTrigger: {
        id: "csr-impact-rise",
        trigger: stats,
        scroller: stScroller,
        start: "top 90%",
        end: "top 45%",
        scrub: 1.15,
        invalidateOnRefresh: true
      }
    });

    items.forEach(function (item, i) {
      tl.fromTo(
        item,
        { autoAlpha: 0, y: 80 },
        { autoAlpha: 1, y: 0, duration: 1, immediateRender: true },
        i * 0.16
      );
    });

    var iconTl = gsap.timeline({
      paused: true,
      defaults: { ease: "power1.inOut" }
    });

    items.forEach(function (item, i) {
      var paths = item.querySelectorAll(".csr-stat__icon path");
      paths.forEach(function (path, p) {
        var len = path.getTotalLength();
        if (!len) return;
        gsap.set(path, { strokeDasharray: len, strokeDashoffset: len });
        iconTl.fromTo(
          path,
          { strokeDashoffset: len },
          { strokeDashoffset: 0, duration: 0.7 },
          i * 0.12 + p * 0.08
        );
      });
    });

    ScrollTrigger.create({
      id: "csr-impact-icons",
      trigger: stats,
      scroller: stScroller,
      start: "bottom 85%",
      onEnter: function () { iconTl.restart(); },
      onEnterBack: function () { iconTl.restart(); },
      onLeaveBack: function () { iconTl.pause(0); }
    });
  });
});

document.addEventListener("DOMContentLoaded", function () {
  var panel = document.querySelector(".csr-progress__panel");
  if (!panel) return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  var pieces = [];
  var copy = panel.querySelector(".csr-progress__copy");
  if (copy) pieces.push(copy);
  panel.querySelectorAll(".csr-sdg").forEach(function (tile) {
    pieces.push(tile);
  });
  if (!pieces.length) return;

  gsap.registerPlugin(ScrollTrigger);
  var stScroller = window.matchMedia("(max-width: 992px)").matches
    ? window
    : document.documentElement;

  var tl = gsap.timeline({
    defaults: { force3D: true, ease: "power2.out" },
    scrollTrigger: {
      id: "csr-progress-rise",
      trigger: panel,
      scroller: stScroller,
      start: "top 90%",
      end: "top 50%",
      scrub: 1.15,
      invalidateOnRefresh: true
    }
  });

  pieces.forEach(function (piece, i) {
    tl.fromTo(
      piece,
      { autoAlpha: 0, y: 70 },
      { autoAlpha: 1, y: 0, duration: 1, immediateRender: true },
      i * 0.18
    );
  });
});

document.addEventListener("DOMContentLoaded", function () {
  var grid = document.querySelector(".csr-approach .csr-grid");
  if (!grid) return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  var cards = grid.querySelectorAll(".csr-approach-card");
  if (cards.length < 2) return;

  gsap.registerPlugin(ScrollTrigger);
  var stScroller = window.matchMedia("(max-width: 992px)").matches
    ? window
    : document.documentElement;

  var tl = gsap.timeline({
    defaults: { force3D: true, ease: "power2.out" },
    scrollTrigger: {
      id: "csr-approach-card-rise",
      trigger: grid,
      scroller: stScroller,
      start: "top 90%",
      end: "top 38%",
      scrub: 1.15,
      invalidateOnRefresh: true
    }
  });

  cards.forEach(function (card, i) {
    tl.fromTo(
      card,
      { autoAlpha: 0, y: 140 },
      { autoAlpha: 1, y: 0, duration: 1.15, immediateRender: true },
      i * 0.28
    );
  });
});

document.addEventListener("DOMContentLoaded", function () {
  var grid = document.querySelector(".csr-focus-grid");
  if (!grid) return;

  var cards = Array.from(grid.querySelectorAll(":scope > .csr-focus-card"));
  if (cards.length < 2) return;

  function placeFill(card, e) {
    var media = card.querySelector(".csr-focus-card__media");
    var cardRect = card.getBoundingClientRect();
    var mediaRect = media ? media.getBoundingClientRect() : null;
    var top = mediaRect ? Math.max(0, mediaRect.bottom - cardRect.top) : cardRect.height * 0.46;
    var fillW = cardRect.width;
    var fillH = Math.max(0, cardRect.height - top);
    var x = e ? e.clientX - cardRect.left : fillW / 2;
    var y = e ? e.clientY - cardRect.top - top : fillH / 2;
    x = Math.min(Math.max(x, 0), fillW);
    y = Math.min(Math.max(y, 0), fillH);
    var radius = Math.ceil(Math.hypot(Math.max(x, fillW - x), Math.max(y, fillH - y))) + 16;
    card.style.setProperty("--fill-top", top + "px");
    card.style.setProperty("--hx", x + "px");
    card.style.setProperty("--hy", y + "px");
    card.style.setProperty("--hr", radius + "px");
  }

  cards.forEach(function (card) {
    placeFill(card, null);
    card.addEventListener("mouseenter", function (e) {
      placeFill(card, e);
      card.classList.add("is-filled");
    });
    card.addEventListener("mouseleave", function () {
      card.classList.remove("is-filled");
    });
  });

  var riseTl = null;

  function playCardRise() {
    if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
    gsap.registerPlugin(ScrollTrigger);
    if (riseTl) {
      if (riseTl.scrollTrigger) riseTl.scrollTrigger.kill();
      riseTl.kill();
      riseTl = null;
    }
    gsap.set(cards, { clearProps: "transform,opacity,visibility" });
    var stScroller = window.matchMedia("(max-width: 992px)").matches
      ? window
      : document.documentElement;
    riseTl = gsap.timeline({
      defaults: { force3D: true, ease: "power2.out" },
      scrollTrigger: {
        id: "csr-focus-card-rise",
        trigger: grid,
        scroller: stScroller,
        start: "top 90%",
        end: "top 38%",
        scrub: 1.15,
        invalidateOnRefresh: true
      }
    });
    cards.forEach(function (card, i) {
      riseTl.fromTo(
        card,
        { autoAlpha: 0, y: 140 },
        { autoAlpha: 1, y: 0, duration: 1.15, immediateRender: true },
        i * 0.28
      );
    });
  }

  function killCardRise() {
    if (riseTl) {
      if (riseTl.scrollTrigger) riseTl.scrollTrigger.kill();
      riseTl.kill();
      riseTl = null;
    }
    if (typeof gsap !== "undefined") {
      gsap.set(cards, { clearProps: "transform,opacity,visibility" });
    }
  }

  if (typeof Swiper === "undefined") {
    playCardRise();
    return;
  }

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var swiper = null;
  var controlsEl = null;
  var wrapperEl = null;
  var isSlider = null;
  var resizeTimer = 0;
  var SLIDER_MAX = 767;

  function wrap() {
    if (wrapperEl) return;
    wrapperEl = document.createElement("div");
    wrapperEl.className = "swiper-wrapper";
    cards.forEach(function (card) {
      card.classList.add("swiper-slide");
      wrapperEl.appendChild(card);
    });
    grid.appendChild(wrapperEl);
    grid.classList.add("swiper", "csr-focus-grid--slider");

    controlsEl = document.createElement("div");
    controlsEl.className = "csr-focus-slider-controls";
    controlsEl.innerHTML =
      '<button type="button" class="csr-focus-nav csr-focus-nav--prev" aria-label="Previous focus area">' +
      '<span aria-hidden="true">&larr;</span></button>' +
      '<div class="swiper-pagination csr-focus-pagination"></div>' +
      '<button type="button" class="csr-focus-nav csr-focus-nav--next" aria-label="Next focus area">' +
      '<span aria-hidden="true">&rarr;</span></button>';
    grid.insertAdjacentElement("afterend", controlsEl);
  }

  function unwrap() {
    if (!wrapperEl) return;
    cards.forEach(function (card) {
      card.classList.remove("swiper-slide");
      grid.appendChild(card);
    });
    wrapperEl.remove();
    wrapperEl = null;
    if (controlsEl) {
      controlsEl.remove();
      controlsEl = null;
    }
    grid.classList.remove("swiper", "csr-focus-grid--slider");
  }

  function enable() {
    if (swiper) return;
    wrap();
    swiper = new Swiper(grid, {
      slidesPerView: 1,
      spaceBetween: 16,
      speed: reduceMotion ? 0 : 450,
      watchOverflow: true,
      navigation: {
        prevEl: controlsEl.querySelector(".csr-focus-nav--prev"),
        nextEl: controlsEl.querySelector(".csr-focus-nav--next"),
      },
      pagination: {
        el: controlsEl.querySelector(".csr-focus-pagination"),
        clickable: true,
      },
    });
  }

  function disable() {
    if (swiper) {
      swiper.destroy(true, true);
      swiper = null;
    }
    unwrap();
  }

  function apply() {
    var want = window.innerWidth <= SLIDER_MAX;
    if (want === isSlider) {
      if (want && swiper) swiper.update();
      return;
    }
    isSlider = want;
    if (want) {
      killCardRise();
      enable();
    } else {
      disable();
      playCardRise();
    }
  }

  apply();
  window.addEventListener("resize", function () {
    window.clearTimeout(resizeTimer);
    resizeTimer = window.setTimeout(apply, 120);
  });
});
