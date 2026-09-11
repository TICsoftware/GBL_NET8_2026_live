/* ---------------------------------------
   INNER BANNER — scroll zoom (site-wide)
   Scales the banner image up on scroll down,
   and eases back on scroll up.
   Markup: .inside-banner-outer .innerbanner-image
--------------------------------------- */
document.addEventListener("DOMContentLoaded", function () {
  if (typeof gsap !== "undefined" && typeof ScrollTrigger !== "undefined") {
    gsap.registerPlugin(ScrollTrigger);
  }

  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  const isMobile = window.matchMedia("(max-width: 992px)").matches;
  const stScroller = isMobile ? window : document.documentElement;

  initInnerBannerZoom();
  initPageIntroFill();

  function initInnerBannerZoom() {
    const banners = document.querySelectorAll(".inside-banner-outer");
    if (!banners.length) return;
    if (reduceMotion || typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") {
      return;
    }

    banners.forEach(function (section) {
      const image =
        section.querySelector(".innerbanner-image") ||
        section.querySelector(".inside-banner-inner img");
      if (!image) return;

      gsap.fromTo(
        image,
        { scale: 1 },
        {
          scale: 1.22,
          ease: "none",
          force3D: true,
          overwrite: "auto",
          scrollTrigger: {
            trigger: section,
            scroller: stScroller,
            start: "top top",
            end: "bottom top",
            scrub: 0.8,
            invalidateOnRefresh: true,
          },
        }
      );
    });
  }
  

  /* ---------------------------------------
     PAGE INTRO — text fill on enter / enter-back
     #565B65 → #282B31 when .page-intro-inner
     comes into view, both scroll directions.
  --------------------------------------- */
  function wrapIntroWords(el) {
    const walker = document.createTreeWalker(el, NodeFilter.SHOW_TEXT, null);
    const nodes = [];
    while (walker.nextNode()) nodes.push(walker.currentNode);

    nodes.forEach(function (node) {
      const text = node.nodeValue;
      if (!text || !text.trim()) return;

      const frag = document.createDocumentFragment();
      text.split(/(\s+)/).forEach(function (part) {
        if (!part) return;
        if (/^\s+$/.test(part)) {
          frag.appendChild(document.createTextNode(part));
          return;
        }
        const span = document.createElement("span");
        span.className = "page-intro-fill-word";
        span.textContent = part;
        frag.appendChild(span);
      });
      node.parentNode.replaceChild(frag, node);
    });
  }

  function initPageIntroFill() {
    const intros = document.querySelectorAll(".page-intro-inner");
    if (!intros.length) return;

    const FILL_FROM = "#B2B2B2";
    const FILL_TO = "#282B31";

    intros.forEach(function (inner, index) {
      if (inner.dataset.pageIntroFill === "ready") return;

      const paragraphs = inner.querySelectorAll("p");
      if (!paragraphs.length) {
        wrapIntroWords(inner);
      } else {
        paragraphs.forEach(wrapIntroWords);
      }

      inner.dataset.pageIntroFill = "ready";

      const words = inner.querySelectorAll(".page-intro-fill-word");
      if (!words.length) return;

      if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") {
        words.forEach(function (word) {
          word.style.color = FILL_TO;
        });
        return;
      }

      gsap.set(words, { color: FILL_FROM });

      if (reduceMotion) {
        gsap.set(words, { color: FILL_TO });
        return;
      }

      gsap.to(words, {
        color: FILL_TO,
        stagger: 0.06,
        ease: "none",
        immediateRender: false,
        overwrite: "auto",
        scrollTrigger: {
          id: "page-intro-fill-" + index,
          trigger: inner,
          scroller: stScroller,
          start: "top 82%",
          end: "bottom 48%",
          scrub: 1.15,
          invalidateOnRefresh: true,
        },
      });
    });

    function refreshFill() {
      if (typeof ScrollTrigger !== "undefined") ScrollTrigger.refresh();
    }

    window.requestAnimationFrame(refreshFill);
    window.addEventListener("load", refreshFill);
  }
});
