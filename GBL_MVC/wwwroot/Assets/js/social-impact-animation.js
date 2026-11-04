/**
 * Social page — Impact tiles
 * Same slide-in as the homepage "Designed to Give Back" grid
 * (giveback-animation.js): when a row comes into view, the first row slides in
 * from the right and the second from the left, tile by tile. Desktop only;
 * below 992px the rows collapse into a two-column grid and the tiles stay static.
 */
document.addEventListener("DOMContentLoaded", function () {
  var tiles = document.querySelector(".social-impact__tiles");
  if (!tiles) return;

  var rows = Array.from(tiles.querySelectorAll(":scope > .social-impact__row"));
  if (!rows.length) return;

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var DESKTOP_MIN = 992;
  var anims = [];
  var isDesktopMode = null;
  var resizeTimer = 0;

  function cellsOf(row) {
    return Array.from(row.children);
  }

  function killAnim() {
    anims.forEach(function (anim) {
      anim.observers.forEach(function (io) { io.disconnect(); });
      anim.tl.kill();
    });
    anims = [];
    if (typeof gsap !== "undefined") {
      rows.forEach(function (row) {
        gsap.set(cellsOf(row), { clearProps: "transform,opacity,visibility" });
      });
    }
  }

  /* Plays once the row reaches 85% down the viewport; resets only after it has
     dropped fully below the viewport, so it plays again on the way back down. */
  function bindRow(row, fromX) {
    var items = cellsOf(row);
    if (!items.length) return;

    var tl = gsap.timeline({
      paused: true,
      defaults: { force3D: true, ease: "power2.out" },
    });

    /* Lead with the tile nearest the direction of travel, so tiles still
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

    var playIo = new IntersectionObserver(function (entries) {
      entries.forEach(function (entry) {
        if (entry.isIntersecting) tl.play();
      });
    }, { rootMargin: "0px 0px -15% 0px" });

    var resetIo = new IntersectionObserver(function (entries) {
      entries.forEach(function (entry) {
        if (!entry.isIntersecting && entry.boundingClientRect.top > 0) tl.pause(0);
      });
    });

    playIo.observe(row);
    resetIo.observe(row);

    anims.push({ tl: tl, observers: [playIo, resetIo] });
  }

  function initAnim() {
    if (reduceMotion || typeof gsap === "undefined" || !("IntersectionObserver" in window)) {
      return;
    }

    rows.forEach(function (row, i) {
      bindRow(row, i % 2 === 0 ? 160 : -160);
    });
  }

  function applyMode() {
    var wantDesktop = window.innerWidth >= DESKTOP_MIN;
    if (wantDesktop === isDesktopMode) return;
    isDesktopMode = wantDesktop;

    if (wantDesktop) {
      initAnim();
    } else {
      killAnim();
    }
  }

  applyMode();

  window.addEventListener("resize", function () {
    window.clearTimeout(resizeTimer);
    resizeTimer = window.setTimeout(applyMode, 120);
  });
});
