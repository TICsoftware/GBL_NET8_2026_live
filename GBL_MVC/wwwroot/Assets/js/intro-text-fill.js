document.addEventListener("DOMContentLoaded", function () {
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  gsap.registerPlugin(ScrollTrigger);

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var isMobile = window.matchMedia("(max-width: 992px)").matches;
  var stScroller = isMobile ? window : document.documentElement;
  var FILL_FROM = "#ccc";
  var FILL_TO = "#282b31";

  function wrapWords(el) {
    var walker = document.createTreeWalker(el, NodeFilter.SHOW_TEXT, null);
    var nodes = [];
    while (walker.nextNode()) nodes.push(walker.currentNode);

    nodes.forEach(function (node) {
      var text = node.nodeValue;
      if (!text || !text.trim()) return;

      var frag = document.createDocumentFragment();
      text.split(/(\s+)/).forEach(function (part) {
        if (!part) return;
        if (/^\s+$/.test(part)) {
          frag.appendChild(document.createTextNode(part));
          return;
        }
        var span = document.createElement("span");
        span.className = "intro-fill-word";
        span.textContent = part;
        frag.appendChild(span);
      });
      node.parentNode.replaceChild(frag, node);
    });
  }

  document.querySelectorAll(".section-intro__content p").forEach(function (paragraph, index) {
    if (paragraph.dataset.introFill === "ready") return;
    wrapWords(paragraph);
    paragraph.dataset.introFill = "ready";

    var words = paragraph.querySelectorAll(".intro-fill-word");
    if (!words.length) return;

    var fillTo = paragraph.closest(".white-copy") ? "#fff" : FILL_TO;
    gsap.set(words, { color: FILL_FROM });

    if (reduceMotion) {
      gsap.set(words, { color: fillTo });
      return;
    }

    gsap.to(words, {
      color: fillTo,
      stagger: 0.06,
      ease: "none",
      immediateRender: false,
      scrollTrigger: {
        id: "intro-text-fill-" + index,
        trigger: paragraph,
        scroller: stScroller,
        start: "top 82%",
        end: "bottom 48%",
        scrub: 1.15,
        invalidateOnRefresh: true,
      },
    });
  });

  function refreshFill() {
    ScrollTrigger.refresh();
  }

  window.requestAnimationFrame(refreshFill);
  window.addEventListener("load", refreshFill);
});
