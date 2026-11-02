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

  var allCards = Array.from(list.querySelectorAll(".industry-card"));
  if (!allCards.length) return;

  function placeFill(card, e) {
    var caption = card.querySelector(".industry-card__caption");
    if (!caption) return;
    var rect = caption.getBoundingClientRect();
    var cardRect = card.getBoundingClientRect();
    var x = e ? e.clientX - rect.left : rect.width / 2;
    var y = e ? e.clientY - rect.top : 0;
    x = Math.min(Math.max(x, 0), Math.max(rect.width, 0));
    y = Math.min(Math.max(y, 0), Math.max(rect.height, 0));
    var radius = Math.ceil(Math.hypot(cardRect.width, cardRect.height)) + 16;
    caption.style.setProperty("--hx", x + "px");
    caption.style.setProperty("--hy", y + "px");
    caption.style.setProperty("--hr", radius + "px");
  }

  allCards.forEach(function (card) {
    card.addEventListener("mouseenter", function (e) {
      placeFill(card, e);
      card.classList.add("is-filled");
    });
    card.addEventListener("mouseleave", function () {
      card.classList.remove("is-filled");
    });
  });

  window.addEventListener("resize", function () {
    allCards.forEach(function (card) {
      if (card.classList.contains("is-filled")) placeFill(card, null);
    });
  });

  var pageSize = parseInt(list.getAttribute("data-page-size") || "8", 10);
  if (!pageSize || pageSize < 1) pageSize = allCards.length;

  var shown = Math.min(pageSize, allCards.length);
  var wrap = list.parentElement && list.parentElement.querySelector("[data-industries-load-more-wrap]");
  var loadBtn = wrap && wrap.querySelector("[data-industries-load-more]");

  function syncPage() {
    allCards.forEach(function (card, index) {
      card.hidden = index >= shown;
    });
    if (wrap) wrap.hidden = shown >= allCards.length;
  }

  syncPage();

  /* Scroll rise only for the first page. Later pages play on Load More. */
  var cards = allCards.filter(function (card) {
    return !card.hidden;
  });

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var canAnimate =
    typeof gsap !== "undefined" &&
    typeof ScrollTrigger !== "undefined" &&
    !reduceMotion;

  /* Lenis only above 992; native window scroll on smaller devices. */
  var useNativeScroll = window.matchMedia("(max-width: 992px)").matches;
  var stScroller = useNativeScroll ? window : document.documentElement;
  var isMobileLayout = window.matchMedia("(max-width: 1179px)").matches;

  function gridColumns() {
    if (window.matchMedia("(max-width: 767px)").matches) return 1;
    if (window.matchMedia("(max-width: 1279px)").matches) return 2;
    return 4;
  }

  function bindPhoneParallax(cardList, idPrefix) {
    if (!canAnimate) return;
    if (!window.matchMedia("(max-width: 767px)").matches) return;

    cardList.forEach(function (card, i) {
      if (card.dataset.parallaxBound === "1") return;
      var media = card.querySelector(".industry-card__media");
      var img = media && media.querySelector("img");
      if (!img) return;

      card.dataset.parallaxBound = "1";
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
            id: idPrefix + i,
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

  /* Same rise as the scroll cards, played once when Load More reveals them. */
  function playLoadedCards(incoming) {
    if (!incoming.length || !canAnimate) return;

    var rise = isMobileLayout ? MOBILE_RISE : CARD_RISE;
    var cols = gridColumns();
    var stagger = CARD_RISE.stagger;

    gsap.set(incoming, { autoAlpha: 0, y: rise.y, force3D: true });

    var tl = gsap.timeline({
      defaults: { force3D: true, ease: rise.ease || "power2.out" }
    });

    incoming.forEach(function (card, i) {
      var row = Math.floor(i / cols);
      var col = i % cols;
      tl.to(
        card,
        {
          autoAlpha: 1,
          y: 0,
          duration: rise.duration,
          overwrite: "auto"
        },
        row * stagger + col * stagger
      );
    });

    bindPhoneParallax(incoming, "industry-card-mobile-parallax-more-");

    tl.eventCallback("onComplete", function () {
      ScrollTrigger.refresh();
    });
  }

  if (loadBtn) {
    loadBtn.addEventListener("click", function () {
      var start = shown;
      shown = Math.min(shown + pageSize, allCards.length);
      var incoming = allCards.slice(start, shown);

      incoming.forEach(function (card) {
        card.hidden = false;
      });
      if (wrap) wrap.hidden = shown >= allCards.length;

      playLoadedCards(incoming);

      if (typeof ScrollTrigger !== "undefined") {
        requestAnimationFrame(function () {
          ScrollTrigger.refresh();
        });
      }
    });
  }

  if (!canAnimate) return;

  gsap.registerPlugin(ScrollTrigger);

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
    bindPhoneParallax(cards, "industry-card-mobile-parallax-");
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
