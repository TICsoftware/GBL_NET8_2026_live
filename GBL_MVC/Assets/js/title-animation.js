document.addEventListener("DOMContentLoaded", (event) => {
  gsap.registerPlugin(ScrollTrigger); // safe to call even if already registered

  function createScrollTrigger(triggerElement, timeline) {
    ScrollTrigger.create({
      trigger: triggerElement,
      start: "top bottom",
      onLeaveBack: () => {
        timeline.progress(0);
        timeline.pause();
      },
    });
    ScrollTrigger.create({
      trigger: triggerElement,
      start: "top 80%",
      onEnter: () => timeline.play(),
    });
  }

  /**
   * Tiny replacement for SplitType (types: "words, chars", tagName: "span").
   * Walks the element's text nodes and wraps every word in <span class="word">
   * and every character in <span class="char">, keeping nested markup
   * (e.g. <br>, <em>, nested spans) intact.
   */
  function splitText(root) {
    // Save original markup so it could be restored later if ever needed
    if (!root.dataset.splitOriginal) {
      root.dataset.splitOriginal = root.innerHTML;
    }

    const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
    const textNodes = [];
    let node;
    while ((node = walker.nextNode())) {
      if (node.nodeValue.trim() !== "") textNodes.push(node);
    }

    textNodes.forEach((textNode) => {
      const fragment = document.createDocumentFragment();
      // Split into words and whitespace runs, keeping the whitespace
      const parts = textNode.nodeValue.split(/(\s+)/);

      parts.forEach((part) => {
        if (part === "") return;

        if (/^\s+$/.test(part)) {
          // Preserve spaces between words as plain text
          fragment.appendChild(document.createTextNode(" "));
          return;
        }

        const wordSpan = document.createElement("span");
        wordSpan.className = "word";
        wordSpan.style.display = "inline-block";

        // Array.from splits by code points (handles emoji/accents better than .split(""))
        Array.from(part).forEach((ch) => {
          const charSpan = document.createElement("span");
          charSpan.className = "char";
          charSpan.style.display = "inline-block";
          charSpan.textContent = ch;
          wordSpan.appendChild(charSpan);
        });

        fragment.appendChild(wordSpan);
      });

      textNode.parentNode.replaceChild(fragment, textNode);
    });
  }

  // 1. Split the text FIRST — this creates the .char spans
  const splitTypeElements = document.querySelectorAll("[text-split]");
  splitTypeElements.forEach((splitTypeElement) => {
    splitText(splitTypeElement);
  });

  // 2. Now the .char elements exist and can be animated
  const lettersSlideDownElements = document.querySelectorAll("[letters-slide-down]");
  lettersSlideDownElements.forEach((element) => {
    const tl = gsap.timeline({ paused: true });
    const chars = element.querySelectorAll(".char");
    tl.from(chars, {
      yPercent: -120,
      duration: 0.3,
      ease: "power1.out",
      stagger: { amount: 0.7 },
    });
    createScrollTrigger(element, tl);
  });

  // 3. Reveal the elements (they're usually hidden via CSS until split is done)
  gsap.set("[text-split]", { opacity: 1 });
});