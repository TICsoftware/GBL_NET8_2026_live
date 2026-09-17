document.addEventListener("DOMContentLoaded", function () {
  /* ---------------------------------------
     Industry cards only (.industries-list).
     Desktop (≥1180): scrubbed row rise (unchanged).
     Mobile (<1180): each card slides up once when
     it enters the viewport — one by one.
  --------------------------------------- */
  var CARD_RISE = {
    y: 140,
    duration: 1.15,
    stagger: 0.28,
    scrub: 1.15,
    listStart: "top 90%",
    listEnd: "top 38%"
  };

  var MOBILE_RISE = {
    y: 96,
    duration: 0.9,
    ease: "power2.out",
    start: "top 92%"
  };

  var list = document.querySelector(".industries-list");
  if (!list) return;

  var cards = Array.from(list.querySelectorAll(".industry-card"));
  if (!cards.length) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

  gsap.registerPlugin(ScrollTrigger);

  /* Lenis only above 992; native window scroll on smaller devices. */
  var useNativeScroll = window.matchMedia("(max-width: 992px)").matches;
  var stScroller = useNativeScroll ? window : document.documentElement;
  var isMobileLayout = window.matchMedia("(max-width: 1179px)").matches;

  if (isMobileLayout) {
    /* One card at a time: slide up when that card enters the viewport. */
    cards.forEach(function (card, i) {
      gsap.set(card, { autoAlpha: 0, y: MOBILE_RISE.y, force3D: true });

      function playRise() {
        if (card.dataset.risePlayed === "1") return;
        card.dataset.risePlayed = "1";
        gsap.to(card, {
          autoAlpha: 1,
          y: 0,
          duration: MOBILE_RISE.duration,
          ease: MOBILE_RISE.ease,
          force3D: true,
          overwrite: "auto"
        });
      }

      card._playIndustryRise = playRise;

      ScrollTrigger.create({
        id: "industry-card-mobile-rise-" + i,
        trigger: card,
        scroller: stScroller,
        start: MOBILE_RISE.start,
        once: true,
        onEnter: playRise
      });
    });

    /* Image parallax — phone only (≤767). Desktop / tablet unchanged. */
    if (window.matchMedia("(max-width: 767px)").matches) {
      cards.forEach(function (card, i) {
        var media = card.querySelector(".industry-card__media");
        var img = media && media.querySelector("img");
        if (!img) return;

        gsap.set(img, {
          force3D: true,
          z: 0.01
        });

        gsap.fromTo(
          img,
          { yPercent: -6 },
          {
            yPercent: 6,
            ease: "none",
            force3D: true,
            scrollTrigger: {
              id: "industry-card-mobile-parallax-" + i,
              trigger: media,
              scroller: stScroller,
              start: "top bottom",
              end: "bottom top",
              scrub: 1.4,
              invalidateOnRefresh: true
            }
          }
        );
      });
    }
  } else {
    /** Grid columns — matches industries-list.css desktop breakpoints. */
    function columns() {
      if (window.matchMedia("(max-width: 1279px)").matches) return 2; /* 1180–1279 */
      return 4; /* 1280+ */
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
  }

  /** Recalc start/end after layout, images, or font load. */
  function refreshTriggers() {
    ScrollTrigger.refresh();

    /* Mobile: cards already in the viewport on load should still rise. */
    if (!isMobileLayout) return;
    cards.forEach(function (card) {
      if (typeof card._playIndustryRise !== "function") return;
      var rect = card.getBoundingClientRect();
      if (rect.top < window.innerHeight * 0.92 && rect.bottom > 0) {
        card._playIndustryRise();
      }
    });
  }

  requestAnimationFrame(refreshTriggers);
  window.addEventListener("load", refreshTriggers);
  window.setTimeout(refreshTriggers, 400);

  /* Suppress false :hover while cards slide under a still cursor.
     Unlock as soon as Lenis velocity drops — do not wait for full settle. */
  (function suppressHoverWhileScrolling() {
    var velocityMin = 0.2;
    var wheelIdleMs = 80;
    var timer = null;
    var lenisBound = false;

    function setScrolling(on) {
      list.classList.toggle("is-scrolling", !!on);
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
