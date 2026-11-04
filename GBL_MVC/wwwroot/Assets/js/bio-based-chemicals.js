/**
 * Bio-Based Chemicals page
 * Card and heading animations come from card-animation.js and title-animation.js,
 * the product filter from industries-filter.js.
 * This file adds the soft image parallax used on the About Us and Business pages.
 */

/* Soft image parallax */
document.addEventListener("DOMContentLoaded", function () {
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  gsap.registerPlugin(ScrollTrigger);

  var stScroller = window.matchMedia("(max-width: 992px)").matches
    ? window
    : document.documentElement;

  document.querySelectorAll(".bbc-parallax").forEach(function (wrap) {
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
