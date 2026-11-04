/**
 * Our Vision page — guiding principle icons
 * Same stroke draw as the CSR "Impact in Action" icons: each path is hidden
 * with a dash, then drawn when the panel scrolls into view. Replays on the
 * way back in, and resets when scrolling back above the panel.
 */
document.addEventListener("DOMContentLoaded", function () {
  var panel = document.querySelector(".guiding-principles__panel");
  if (!panel) return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  var items = Array.from(panel.querySelectorAll(".principle-item"));
  if (!items.length) return;

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
        svg.setAttribute("shape-rendering", "geometricPrecision");
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
      var paths = item.querySelectorAll(".principle-item__icon path");
      paths.forEach(function (path, p) {
        var len = path.getTotalLength();
        if (!len) return;
        allPaths.push(path);
        gsap.set(path, { strokeDasharray: len, strokeDashoffset: len, strokeOpacity: 0, fill: "#fff", fillOpacity: 0 });
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

    /* These icons are outlined shapes, so the stroke traces both edges of each
       line. Filling at the end closes that into a single solid line, like CSR. */
    iconTl.to(allPaths, { fillOpacity: 1, duration: 0.35, ease: "none" }, iconTl.duration() - 0.1);

    /* Top of the panel, not its bottom as on CSR: this panel gets much taller on
       smaller screens, and the first icons must not sit blank while in view. */
    ScrollTrigger.create({
      trigger: trigger,
      scroller: stScroller,
      start: "top 70%",
      onEnter: function () { iconTl.restart(); },
      onEnterBack: function () { iconTl.restart(); },
      onLeaveBack: function () { iconTl.pause(0); }
    });
  }

  Promise.all(Array.from(panel.querySelectorAll(".principle-item__icon img")).map(inlineIcon)).then(function () {
    gsap.registerPlugin(ScrollTrigger);
    var stScroller = window.matchMedia("(max-width: 992px)").matches
      ? window
      : document.documentElement;

    /* Stacked on phones: each icon draws as its own card arrives. */
    if (window.matchMedia("(max-width: 639px)").matches) {
      items.forEach(function (item) {
        createIconDraw([item], item, stScroller);
      });
    } else {
      createIconDraw(items, panel, stScroller);
    }

    ScrollTrigger.refresh();
  });
});

/**
 * Our Mission background — lighter parallax than the site-wide one, whose
 * travel would crop the landscape at the bottom of this image.
 */
document.addEventListener("DOMContentLoaded", function () {
  var section = document.querySelector(".our-mission");
  var img = section && section.querySelector(".our-mission__img");
  if (!img) return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  gsap.registerPlugin(ScrollTrigger);

  gsap.fromTo(
    img,
    { yPercent: -3 },
    {
      yPercent: 3,
      ease: "none",
      force3D: true,
      scrollTrigger: {
        trigger: section,
        scroller: window.matchMedia("(max-width: 992px)").matches ? window : document.documentElement,
        start: "top bottom",
        end: "bottom top",
        scrub: 0.8,
        invalidateOnRefresh: true,
      },
    }
  );
});
