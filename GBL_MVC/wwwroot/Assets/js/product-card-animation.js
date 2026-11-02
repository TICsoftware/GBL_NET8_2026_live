document.addEventListener("DOMContentLoaded", function () {
  var grid = document.querySelector(".product-filter-grid");
  if (!grid) return;

  function placeFill(card, e) {
    var media = card.querySelector(".product-card__media");
    var cardRect = card.getBoundingClientRect();
    var mediaRect = media ? media.getBoundingClientRect() : null;
    var top = mediaRect ? Math.max(0, mediaRect.bottom - cardRect.top) : cardRect.height * 0.46;
    var fillW = cardRect.width;
    var fillH = Math.max(0, cardRect.height - top);
    var x = e ? e.clientX - cardRect.left : fillW / 2;
    var y = e ? e.clientY - cardRect.top - top : fillH / 2;
    x = Math.min(Math.max(x, 0), fillW);
    y = Math.min(Math.max(y, 0), fillH);
    var radius = Math.ceil(Math.hypot(Math.max(x, fillW - x), Math.max(y, fillH - y))) + 16;
    card.style.setProperty("--fill-top", top + "px");
    card.style.setProperty("--hx", x + "px");
    card.style.setProperty("--hy", y + "px");
    card.style.setProperty("--hr", radius + "px");
  }

  grid.querySelectorAll(".product-card").forEach(function (card) {
    placeFill(card, null);
    card.addEventListener("mouseenter", function (e) {
      placeFill(card, e);
      card.classList.add("is-filled");
    });
    card.addEventListener("mouseleave", function () {
      card.classList.remove("is-filled");
    });
  });

  window.addEventListener("resize", function () {
    grid.querySelectorAll(".product-card").forEach(function (card) {
      placeFill(card, null);
    });
  });
});

document.addEventListener("DOMContentLoaded", function () {
  /* Same travel-on-scroll as industries-script.js desktop CARD_RISE. */
  var CARD_RISE = {
    y: 140,
    duration: 1.15,
    stagger: 0.28,
    scrub: 1.15,
    listStart: "top 90%",
    listEnd: "top 38%"
  };

  var grid = document.querySelector(".product-filter-grid");
  if (!grid) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

  gsap.registerPlugin(ScrollTrigger);

  var useNativeScroll = window.matchMedia("(max-width: 992px)").matches;
  var stScroller = useNativeScroll ? window : document.documentElement;
  var triggerPrefix = "product-card-";

  function allCards() {
    return Array.from(grid.querySelectorAll(".product-card"));
  }

  function visibleCards() {
    return allCards().filter(function (card) {
      return !card.hidden;
    });
  }

  function killCardTriggers() {
    ScrollTrigger.getAll().forEach(function (st) {
      var id = (st.vars && st.vars.id) || st.id || "";
      if (String(id).indexOf(triggerPrefix) === 0) st.kill();
    });
  }

  function columns() {
    if (window.matchMedia("(max-width: 767px)").matches) return 1;
    if (window.matchMedia("(max-width: 1279px)").matches) return 2;
    return 3;
  }

  function setup(cards) {
    killCardTriggers();
    if (!cards.length) return;

    var cols = columns();
    for (var r = 0; r < cards.length; r += cols) {
      var row = cards.slice(r, r + cols);
      var rowIndex = r / cols;
      var tl = gsap.timeline({
        defaults: { force3D: true, ease: "power2.out" },
        scrollTrigger: {
          id: triggerPrefix + "rise-row-" + rowIndex,
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
  }

  function refreshTriggers() {
    ScrollTrigger.refresh();
  }

  function rebuild() {
    allCards().forEach(function (card) {
      gsap.set(card, { clearProps: "transform,opacity,visibility" });
    });
    setup(visibleCards());
    requestAnimationFrame(refreshTriggers);
  }

  window.refreshProductCardAnimation = rebuild;

  setup(visibleCards());
  requestAnimationFrame(refreshTriggers);
  window.addEventListener("load", refreshTriggers);
  window.setTimeout(refreshTriggers, 400);

  (function suppressHoverWhileScrolling() {
    var velocityMin = 0.2;
    var wheelIdleMs = 80;
    var timer = null;
    var lenisBound = false;

    function setScrolling(on) {
      grid.classList.toggle("is-scrolling", !!on);
    }

    function onLenisScroll(e) {
      var velocity = Math.abs((e && e.velocity) || 0);
      if (velocity > velocityMin) {
        if (timer) {
          window.clearTimeout(timer);
          timer = null;
        }
        setScrolling(true);
        return;
      }
      setScrolling(false);
    }

    function onWheel() {
      setScrolling(true);
      if (timer) window.clearTimeout(timer);
      timer = window.setTimeout(function () {
        if (!window.lenis || Math.abs(window.lenis.velocity || 0) <= velocityMin) {
          setScrolling(false);
        }
        timer = null;
      }, wheelIdleMs);
    }

    function bindLenis() {
      if (lenisBound || !window.lenis || typeof window.lenis.on !== "function") return;
      window.lenis.on("scroll", onLenisScroll);
      lenisBound = true;
    }

    bindLenis();
    if (!lenisBound) {
      window.setTimeout(bindLenis, 0);
      window.setTimeout(bindLenis, 100);
    }

    window.addEventListener("wheel", onWheel, { passive: true });
    window.addEventListener(
      "scroll",
      function () {
        if (lenisBound) return;
        setScrolling(true);
        if (timer) window.clearTimeout(timer);
        timer = window.setTimeout(function () {
          setScrolling(false);
          timer = null;
        }, wheelIdleMs);
      },
      { passive: true }
    );
  })();
});
