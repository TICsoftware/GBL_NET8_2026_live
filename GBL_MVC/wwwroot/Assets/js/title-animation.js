document.addEventListener("DOMContentLoaded", () => {
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  gsap.registerPlugin(ScrollTrigger);

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

  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  const splitTypes = document.querySelectorAll(".reveal-text");

  splitTypes.forEach((container) => {
    const { chars } = splitText(container);
    const total = chars.length;
    if (!total) return;

    if (reduceMotion) {
      chars.forEach((c) => c.classList.add("revealed"));
      return;
    }

    // Keep total wave ~0.6–1.1s so headings finish while still in view
  const stagger = total > 45 ? 0.028 : total > 25 ? 0.035 : 0.04;
  const duration = Math.min(2.2, Math.max(1.4, total * stagger));

    const syncChars = (progress) => {
      chars.forEach((c, i) => {
        if (progress > i / total) c.classList.add("revealed");
        else c.classList.remove("revealed");
      });
    };

    const tl = gsap.timeline({
      scrollTrigger: {
        trigger: container,
        start: "top 85%",
        // onEnter / onLeave / onEnterBack / onLeaveBack
        toggleActions: "play none none reverse",
        markers: false,
      },
      onUpdate() {
        syncChars(this.progress());
      },
      onReverseComplete() {
        syncChars(0);
      },
    });

    // Dummy tween gives the timeline a real duration for stagger fill
    tl.to({}, { duration });
  });
});
