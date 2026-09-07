document.addEventListener("DOMContentLoaded", () => {
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  const banner = document.querySelector(".inside-banner-outer");
  const image = document.querySelector(".innerbanner-image");
  const caption = document.querySelector(".innerbanner-caption");
  const title = document.querySelector(".innerbanner-title");
  const breadcrumb = document.querySelector(".breadcrumb-nav");
  if (!banner || !image) return;

  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

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

  // Natural size on load — zoom only happens while scrolling
  gsap.set(image, {
    scale: 1,
    yPercent: 0,
    transformOrigin: "50% 50%",
    force3D: true,
  });

  if (!reduceMotion) {
    gsap.to(image, {
      yPercent: 12,
      scale: 1.12,
      ease: "none",
      force3D: true,
      scrollTrigger: {
        trigger: banner,
        start: "top top",
        end: "bottom top",
        scrub: true,
      },
    });
  }

  // Title char fill on load + light breadcrumb fade (no whole-caption slide)
  if (!caption) return;

  if (reduceMotion) {
    if (title) {
      const { chars } = splitText(title);
      chars.forEach((c) => c.classList.add("revealed"));
    }
    return;
  }

  if (breadcrumb) {
    gsap.set(breadcrumb, { autoAlpha: 0, y: 12 });
  }

  if (title) {
    const { chars } = splitText(title);
    const total = chars.length;

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

    // Snappy banner fill — readable fast, still reads as a wave
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
  } else if (breadcrumb) {
    gsap.to(breadcrumb, {
      autoAlpha: 1,
      y: 0,
      duration: 0.7,
      ease: "power2.out",
      delay: 0.2,
    });
  }
});
