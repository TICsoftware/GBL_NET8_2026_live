/**
 * About Us page
 * Card and heading animations come from card-animation.js and title-animation.js.
 * This file adds the benchmark card hover fill, the soft image parallax and the
 * guiding principle icon stroke draw used on the CSR and Our Vision pages.
 */

/* Benchmark cards: same hover fill as the CSR focus cards */
document.addEventListener("DOMContentLoaded", function () {
  var cards = document.querySelectorAll(".about-benchmark");

  function placeFill(card, e) {
    var media = card.querySelector(".about-benchmark__media");
    var cardRect = card.getBoundingClientRect();
    var mediaRect = media ? media.getBoundingClientRect() : null;
    var top = mediaRect ? Math.max(0, mediaRect.bottom - cardRect.top) : cardRect.height * 0.7;
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
    card.addEventListener("mouseenter", function (e) {
      placeFill(card, e);
      card.classList.add("is-filled");
    });
    card.addEventListener("mouseleave", function () {
      card.classList.remove("is-filled");
    });
  });
});

/* Soft image parallax */
document.addEventListener("DOMContentLoaded", function () {
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  gsap.registerPlugin(ScrollTrigger);

  var stScroller = window.matchMedia("(max-width: 992px)").matches
    ? window
    : document.documentElement;

  document.querySelectorAll(".about-parallax").forEach(function (wrap) {
    var img = wrap.querySelector("img");
    if (!img) return;
    gsap.fromTo(
      img,
      { yPercent: -10 },
      {
        yPercent: 10,
        ease: "none",
        force3D: true,
        scrollTrigger: {
          trigger: wrap,
          scroller: stScroller,
          start: "top bottom",
          end: "bottom top",
          scrub: 0.8,
          invalidateOnRefresh: true,
        },
      }
    );
  });
});

/* Guiding principle icons: stroke draw as on the CSR and Our Vision pages */
document.addEventListener("DOMContentLoaded", function () {
  var grid = document.querySelector(".about-principles__grid");
  if (!grid) return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  var items = Array.from(grid.querySelectorAll(".about-principle"));
  var ICON_COLOR = "#67815a";

  function inlineIcon(img) {
    return fetch(img.currentSrc || img.src)
      .then(function (res) { return res.text(); })
      .then(function (text) {
        var holder = document.createElement("div");
        holder.innerHTML = text.trim();
        var svg = holder.querySelector("svg");
        if (!svg) return;
        svg.querySelectorAll("[id]").forEach(function (el) { el.removeAttribute("id"); });
        svg.querySelectorAll("[clip-path]").forEach(function (el) { el.removeAttribute("clip-path"); });
        svg.setAttribute("aria-hidden", "true");
        svg.setAttribute("focusable", "false");
        svg.setAttribute("preserveAspectRatio", "xMidYMid meet");
        img.replaceWith(svg);
      })
      .catch(function () {});
  }

  /** One timeline and trigger for a group of cards that draw together. */
  function createIconDraw(group, trigger, stScroller) {
    var iconTl = gsap.timeline({
      paused: true,
      defaults: { ease: "power1.inOut" }
    });
    var allPaths = [];

    group.forEach(function (item, i) {
      item.querySelectorAll(".about-principle__icon path").forEach(function (path, p) {
        var len = path.getTotalLength();
        if (!len) return;
        allPaths.push(path);
        gsap.set(path, {
          stroke: ICON_COLOR,
          strokeWidth: 1,
          strokeLinecap: "round",
          strokeLinejoin: "round",
          strokeDasharray: len,
          strokeDashoffset: len,
          strokeOpacity: 0,
          fillOpacity: 0
        });
        /* Round caps show as dots while the dash is hidden, so keep the stroke off until it draws. */
        iconTl.set(path, { strokeOpacity: 1 }, i * 0.12 + p * 0.08);
        iconTl.fromTo(
          path,
          { strokeDashoffset: len },
          { strokeDashoffset: 0, duration: 0.7 },
          i * 0.12 + p * 0.08
        );
      });
    });

    /* The icons are outlined shapes, so the stroke traces both edges of each line.
       Filling at the end closes that into a single solid line. */
    iconTl.to(allPaths, { fillOpacity: 1, duration: 0.35, ease: "none" }, iconTl.duration() - 0.1);

    ScrollTrigger.create({
      trigger: trigger,
      scroller: stScroller,
      start: "top 70%",
      onEnter: function () { iconTl.restart(); },
      onEnterBack: function () { iconTl.restart(); },
      onLeaveBack: function () { iconTl.pause(0); }
    });
  }

  Promise.all(Array.from(grid.querySelectorAll(".about-principle__icon img")).map(inlineIcon)).then(function () {
    gsap.registerPlugin(ScrollTrigger);
    var stScroller = window.matchMedia("(max-width: 992px)").matches
      ? window
      : document.documentElement;

    /* Stacked on small screens: each icon draws as its own card arrives. */
    if (window.matchMedia("(max-width: 1279px)").matches) {
      items.forEach(function (item) {
        createIconDraw([item], item, stScroller);
      });
    } else {
      createIconDraw(items, grid, stScroller);
    }

    ScrollTrigger.refresh();
  });
});
