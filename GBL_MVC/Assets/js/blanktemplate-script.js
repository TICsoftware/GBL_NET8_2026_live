document.addEventListener("DOMContentLoaded", () => {
  if (typeof gsap === "undefined") return;

  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // Same split used by innerbanner-title (Careers / page banners)
  function splitText(el) {
    const chars = [];
    const frag = document.createDocumentFragment();
    let currentWord = null;

    function flushWord() {
      if (currentWord && currentWord.childNodes.length) {
        frag.appendChild(currentWord);
      }
      currentWord = null;
    }

    function newWord() {
      const w = document.createElement("span");
      w.className = "word";
      w.style.display = "inline-block";
      return w;
    }

    function addChar(letter, highlight) {
      if (!currentWord) currentWord = newWord();
      const c = document.createElement("span");
      c.className = "char" + (highlight ? " highlight" : "");
      c.textContent = letter;
      c.style.display = "inline-block";
      currentWord.appendChild(c);
      chars.push(c);
    }

    function processText(text, highlight) {
      text.split(/(\s+)/).forEach((part) => {
        if (part === "") return;
        if (/^\s+$/.test(part)) {
          flushWord();
          frag.appendChild(document.createTextNode(" "));
        } else {
          [...part].forEach((letter) => addChar(letter, highlight));
        }
      });
    }

    function walk(node, highlight) {
      node.childNodes.forEach((child) => {
        if (child.nodeType === Node.TEXT_NODE) {
          processText(child.textContent, highlight);
        } else if (child.nodeType === Node.ELEMENT_NODE) {
          walk(child, highlight || child.tagName === "SPAN");
        }
      });
    }

    walk(el, false);
    flushWord();

    el.textContent = "";
    el.appendChild(frag);

    return { chars };
  }

  function animateTitleReveal(title, breadcrumb) {
    if (!title || title.classList.contains("inner-split-done")) return;

    const { chars } = splitText(title);
    title.classList.add("inner-split-done");
    const total = chars.length;

    if (reduceMotion) {
      chars.forEach((c) => c.classList.add("revealed"));
      if (breadcrumb) gsap.set(breadcrumb, { autoAlpha: 1, y: 0 });
      return;
    }

    if (breadcrumb) {
      gsap.set(breadcrumb, { autoAlpha: 0, y: 12 });
    }

    if (!total) {
      if (breadcrumb) {
        gsap.to(breadcrumb, {
          autoAlpha: 1,
          y: 0,
          duration: 0.6,
          ease: "power2.out",
        });
      }
      return;
    }

    // Same wave timing as inner-banner-script.js
    const stagger = total > 40 ? 0.012 : 0.016;
    const duration = Math.min(0.75, Math.max(0.5, total * stagger));

    const syncChars = (progress) => {
      chars.forEach((c, i) => {
        if (progress > i / total) c.classList.add("revealed");
        else c.classList.remove("revealed");
      });
    };

    const tl = gsap.timeline({
      delay: 0,
      onUpdate() {
        syncChars(this.progress());
      },
      onComplete() {
        syncChars(1);
      },
    });

    tl.to({}, { duration });

    if (breadcrumb) {
      tl.to(
        breadcrumb,
        {
          autoAlpha: 1,
          y: 0,
          duration: 0.45,
          ease: "power2.out",
        },
        0.12
      );
    }
  }

  function innerBannerTitleAnimation() {
    // Prefer shared .innerbanner-title; also support legacy .innerbanner-title-heading
    const titles = document.querySelectorAll(
      ".innerbanner-title, .innerbanner-title-heading"
    );
    if (!titles.length) return;

    const seen = new Set();

    titles.forEach((title) => {
      if (seen.has(title)) return;
      seen.add(title);

      const banner =
        title.closest(".blank-template-banner, .inside-banner-outer, .banner-inner-section") ||
        document;
      const breadcrumb =
        banner.querySelector(".breadcrumb-nav") ||
        banner.querySelector(".breadcrumb_container");

      animateTitleReveal(title, breadcrumb);
    });
  }

  // Run on load so fonts/layout are ready (matches previous blanktemplate behavior)
  window.addEventListener("load", () => {
    innerBannerTitleAnimation();
  });

  // Fallback if load already fired
  if (document.readyState === "complete") {
    innerBannerTitleAnimation();
  }
});
