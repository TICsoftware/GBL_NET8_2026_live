/**
 * Philosophy & Guiding Principles — scroll animation
 *
 * Desktop (1024px+):
 *   1. Image sits in a box under the title/copy (dark type on white).
 *   2. On scroll it travels, then expands to full screen.
 *   3. Type stays black until the photo sits under the copy, then inverts to white.
 *   4. Infographic cards then travel in at different speeds.
 *
 * Mobile / reduced motion: skip the pin sequence; show image + infographic static.
 */
document.addEventListener("DOMContentLoaded", function () {
  var section = document.querySelector(".ourPhilosophy");
  if (!section) return;

  var track = section.querySelector(".philosophy-track");
  var sticky = section.querySelector(".philosophy-sticky");
  var frame = section.querySelector("[data-philosophy-frame]");
  var frameImg = frame ? frame.querySelector("img") : null;
  var veil = section.querySelector(".philosophy-frame__veil");
  var copy = section.querySelector(".philosophy-copy");
  var infographic = section.querySelector("[data-philosophy-infographic]");
  if (!track || !sticky || !frame || !copy || !infographic) return;

  var cardTl = infographic.querySelector('.philosophy-card[data-principle="tl"]');
  var cardTr = infographic.querySelector('.philosophy-card[data-principle="tr"]');
  var cardBl = infographic.querySelector('.philosophy-card[data-principle="bl"]');
  var cardBr = infographic.querySelector('.philosophy-card[data-principle="br"]');
  var cards = [cardTl, cardTr, cardBl, cardBr].filter(Boolean);
  var hubInner = infographic.querySelector(".philosophy-hub__inner");
  var titleEl = copy.querySelector(".text-h2");
  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var hasGsap = typeof gsap !== "undefined" && typeof ScrollTrigger !== "undefined";
  var START_Y = 120;

  function boxedRect() {
    var stickyRect = sticky.getBoundingClientRect();
    var copyRect = copy.getBoundingClientRect();
    var stickyH = sticky.offsetHeight;
    var width = copy.offsetWidth;
    var left = Math.max(0, copyRect.left - stickyRect.left);
    var top = Math.max(0, copyRect.bottom - stickyRect.top + 20);
    var height = Math.max(220, stickyH - top - START_Y - 16);
    return { left: left, top: top, width: width, height: height };
  }

  function travelY(rect) {
    var maxY = Math.max(0, sticky.offsetHeight - rect.top - rect.height - 8);
    return Math.min(START_Y, maxY);
  }

  function applyBox(rect, y) {
    gsap.set(frame, {
      left: rect.left,
      top: rect.top,
      width: rect.width,
      height: rect.height,
      y: y == null ? travelY(rect) : y,
    });
  }

  /**
   * Black while the photo is still below the headline/copy.
   * Invert to white only when the image edge has reached the text row.
   */
  function imageIsUnderCopy() {
    var fr = frame.getBoundingClientRect();
    var intro = copy.querySelector(".section-intro") || copy;
    var cr = intro.getBoundingClientRect();
    return fr.top <= cr.top + 8;
  }

  function syncTypeColor() {
    var invert = imageIsUnderCopy();
    var words = copy.querySelectorAll(".intro-fill-word");
    section.classList.toggle("is-on-media", invert);
    if (invert) {
      if (titleEl) gsap.set(titleEl, { color: "#fff" });
      if (words.length) gsap.set(words, { color: "#fff" });
    } else {
      if (titleEl) gsap.set(titleEl, { clearProps: "color" });
      if (words.length) gsap.set(words, { clearProps: "color" });
    }
  }

  if (!hasGsap || reduceMotion) {
    if (hasGsap) {
      gsap.set(frame, { left: 0, top: 0, width: "100%", height: "100%", y: 0 });
    } else {
      frame.style.left = "0";
      frame.style.top = "0";
      frame.style.width = "100%";
      frame.style.height = "100%";
    }
    section.classList.add("is-on-media");
    infographic.style.opacity = "1";
    infographic.style.visibility = "visible";
    if (veil) veil.style.opacity = "0.5";
    return;
  }

  gsap.registerPlugin(ScrollTrigger);

  var mm = gsap.matchMedia();
  var isMobileView = window.matchMedia("(max-width: 992px)").matches;
  var stScroller = isMobileView ? window : document.documentElement;

  mm.add("(min-width: 1024px)", function () {
    var startBox = boxedRect();
    applyBox(startBox, travelY(startBox));
    if (frameImg) gsap.set(frameImg, { scale: 1.16, transformOrigin: "50% 50%" });
    gsap.set(veil, { opacity: 0 });
    gsap.set(infographic, { autoAlpha: 0 });
    if (hubInner) gsap.set(hubInner, { scale: 0.68, autoAlpha: 0 });
    if (cardTl) gsap.set(cardTl, { autoAlpha: 0, x: -120, y: -90 });
    if (cardTr) gsap.set(cardTr, { autoAlpha: 0, x: 140, y: -70 });
    if (cardBl) gsap.set(cardBl, { autoAlpha: 0, x: -110, y: 120 });
    if (cardBr) gsap.set(cardBr, { autoAlpha: 0, x: 130, y: 100 });
    section.classList.remove("is-on-media");

    var EXPAND_AT = 0.12;
    var FULL_AT = 0.4;

    var tl = gsap.timeline({
      defaults: { ease: "none" },
      scrollTrigger: {
        trigger: track,
        scroller: stScroller,
        start: "top 70%",
        end: "bottom 20%",
        scrub: 0.45,
        invalidateOnRefresh: true,
      },
      onUpdate: syncTypeColor,
    });

    tl.to(frame, { y: 0, duration: EXPAND_AT }, 0)
      .to(
        frame,
        {
          left: 0,
          top: 0,
          width: "100%",
          height: "100%",
          duration: 0.28,
        },
        EXPAND_AT
      )
      .to(frameImg, { scale: 1, duration: 0.28 }, EXPAND_AT)
      .to(veil, { opacity: 0.55, duration: 0.28 }, EXPAND_AT)
      .to(infographic, { autoAlpha: 1, duration: 0.2 }, FULL_AT);

    if (cardTl) tl.to(cardTl, { autoAlpha: 1, x: 0, y: 0, duration: 0.22 }, FULL_AT);
    if (cardBl) tl.to(cardBl, { autoAlpha: 1, x: 0, y: 0, duration: 0.28 }, FULL_AT + 0.02);
    if (hubInner) tl.to(hubInner, { autoAlpha: 1, scale: 1, duration: 0.16 }, FULL_AT + 0.04);
    if (cardTr) tl.to(cardTr, { autoAlpha: 1, x: 0, y: 0, duration: 0.18 }, FULL_AT + 0.06);
    if (cardBr) tl.to(cardBr, { autoAlpha: 1, x: 0, y: 0, duration: 0.22 }, FULL_AT + 0.08);
    tl.to({}, { duration: 0.16 });

    var onRefresh = function () {
      if (tl.scrollTrigger && tl.scrollTrigger.progress < 0.02) {
        var box = boxedRect();
        applyBox(box, travelY(box));
      }
    };
    ScrollTrigger.addEventListener("refresh", onRefresh);
    requestAnimationFrame(function () {
      ScrollTrigger.refresh();
    });

    return function () {
      ScrollTrigger.removeEventListener("refresh", onRefresh);
      if (tl.scrollTrigger) tl.scrollTrigger.kill();
      tl.kill();
      gsap.set([frame, frameImg, veil, infographic, hubInner, titleEl].concat(cards), { clearProps: "all" });
      gsap.set(copy.querySelectorAll(".intro-fill-word"), { clearProps: "color" });
      section.classList.remove("is-on-media");
    };
  });

  mm.add("(max-width: 1023px)", function () {
    frame.removeAttribute("style");
    if (frameImg) frameImg.removeAttribute("style");
    infographic.style.opacity = "1";
    infographic.style.visibility = "visible";
    if (veil) veil.style.opacity = "0.42";
    section.classList.remove("is-on-media");
    return function () {
      infographic.removeAttribute("style");
    };
  });
});
