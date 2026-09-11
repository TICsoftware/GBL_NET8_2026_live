document.addEventListener("DOMContentLoaded", () => {
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  gsap.registerPlugin(ScrollTrigger);

  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  const titles = document.querySelectorAll(".text-h2, .title-animation, .reveal-text");
  const TITLE_FROM = {
    y: 120,
    opacity: 0,
    skewY: 7,
    filter: "blur(12px)",
    clipPath: "inset(0 0 100% 0)",
  };
  const TITLE_TO = {
    y: 0,
    opacity: 1,
    skewY: 0,
    filter: "blur(0px)",
    clipPath: "inset(0 0 0% 0)",
  };

  window.GBLTitleReveal = {
    from: TITLE_FROM,
    to: TITLE_TO,
    play: function (el) {
      if (!el) return;
      return gsap.to(el, {
        y: 0,
        opacity: 1,
        skewY: 0,
        filter: "blur(0px)",
        clipPath: "inset(0 0 0% 0)",
        duration: 1.3,
        ease: "expo.out",
        overwrite: "auto",
        force3D: true,
      });
    },
    reverse: function (el) {
      if (!el) return;
      return gsap.to(el, {
        y: 120,
        opacity: 0,
        skewY: 7,
        filter: "blur(12px)",
        clipPath: "inset(0 0 100% 0)",
        duration: 0.7,
        ease: "expo.in",
        overwrite: "auto",
        force3D: true,
      });
    },
  };

  /* ==========================================================
     EXACT WEBFLOW STYLE LETTER SLIDE DOWN
  ========================================================== */
  titles.forEach((el) => {
    if (el.closest && (el.closest(".heroBanner") || el.closest(".beginsBelief"))) return;

    if (reduceMotion) {
      gsap.set(el, TITLE_TO);
      return;
    }

    gsap.fromTo(el, TITLE_FROM, {
      y: 0,
      opacity: 1,
      skewY: 0,
      filter: "blur(0px)",
      clipPath: "inset(0 0 0% 0)",
      duration: 1.3,
      ease: "expo.out",
      scrollTrigger: {
        trigger: el,
        start: "top 90%",
        toggleActions: "play none none reverse",
      },
    });
  });
});
