document.addEventListener("DOMContentLoaded", function () {
  /* ---------------------------------------
     Industry cards only (.industries-list).
     Same CARD_RISE as homepage card-animation.js:
     fade + rise, scrubbed to scroll, stagger
     within each row. Other pages are untouched.
  --------------------------------------- */
  var CARD_RISE = {
    y: 140,
    duration: 1.15,
    stagger: 0.28,
    scrub: 1.15,
    listStart: "top 90%",
    listEnd: "top 38%"
  };

  var list = document.querySelector(".industries-list");
  if (!list) return;

  var cards = Array.from(list.querySelectorAll(".industry-card"));
  if (!cards.length) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

  gsap.registerPlugin(ScrollTrigger);

  var isMobile = window.matchMedia("(max-width: 992px)").matches;
  var stScroller = isMobile ? window : document.documentElement;

  function columns() {
    if (window.matchMedia("(max-width: 1179px)").matches) return 1;
    if (window.matchMedia("(max-width: 1279px)").matches) return 2;
    return 4;
  }

  var cols = columns();

  for (var r = 0; r < cards.length; r += cols) {
    var row = cards.slice(r, r + cols);
    var rowIndex = r / cols;

    var tl = gsap.timeline({
      defaults: { force3D: true, ease: "power2.out" },
      scrollTrigger: {
        id: "industry-card-rise-row-" + rowIndex,
        trigger: row[0],
        scroller: stScroller,
        start: CARD_RISE.listStart,
        end: CARD_RISE.listEnd,
        scrub: CARD_RISE.scrub,
        invalidateOnRefresh: true
      }
    });

    row.forEach(function (card, i) {
      tl.fromTo(
        card,
        { autoAlpha: 0, y: CARD_RISE.y },
        {
          autoAlpha: 1,
          y: 0,
          duration: CARD_RISE.duration,
          immediateRender: true
        },
        i * CARD_RISE.stagger
      );
    });
  }

  function refreshTriggers() {
    ScrollTrigger.refresh();
  }

  requestAnimationFrame(refreshTriggers);
  window.addEventListener("load", refreshTriggers);
  window.setTimeout(refreshTriggers, 400);
});
