/**
 * Annual report cards — same row rise as the product list
 * (product-card-animation.js). Each row scrubs in when that
 * row reaches the viewport, staggered left to right.
 */
document.addEventListener("DOMContentLoaded", function () {
  var grid = document.querySelector(".ar-reports__grid");
  if (
    grid &&
    !window.matchMedia("(prefers-reduced-motion: reduce)").matches &&
    window.matchMedia("(hover: hover) and (pointer: fine)").matches
  ) {
    var maxTilt = 15;
    grid.querySelectorAll(".ar-reports__card").forEach(function (card) {
      var media = card.querySelector(".ar-reports__media");
      if (!media) return;

      card.addEventListener("mousemove", function (e) {
        if (grid.classList.contains("is-scrolling")) return;
        var rect = card.getBoundingClientRect();
        var x = (e.clientX - rect.left) / rect.width;
        var y = (e.clientY - rect.top) / rect.height;
        var rx = ((0.8 - y) * maxTilt).toFixed(2);
        var ry = ((x - 0.8) * maxTilt).toFixed(2);
        card.classList.add("is-tilting");
        media.style.transform = "rotateX(" + rx + "deg) rotateY(" + ry + "deg)";
      });

      card.addEventListener("mouseleave", function () {
        card.classList.remove("is-tilting");
        media.style.transform = "";
      });
    });
  }
});

document.addEventListener("DOMContentLoaded", function () {
  var CARD_RISE = {
    y: 140,
    duration: 1.15,
    stagger: 0.28,
    scrub: 1.15,
    listStart: "top 90%",
    listEnd: "top 38%"
  };

  var grid = document.querySelector(".ar-reports__grid");
  if (!grid) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

  gsap.registerPlugin(ScrollTrigger);

  var useNativeScroll = window.matchMedia("(max-width: 992px)").matches;
  var stScroller = useNativeScroll ? window : document.documentElement;
  var triggerPrefix = "ar-card-rise-";

  function allCards() {
    return Array.from(grid.querySelectorAll(".ar-reports__item"));
  }

  function visibleCards() {
    return allCards().filter(function (card) {
      return !card.hidden;
    });
  }

  function columns() {
    if (window.matchMedia("(max-width: 640px)").matches) return 1;
    if (window.matchMedia("(max-width: 1100px)").matches) return 2;
    return 4;
  }

  function killCardTriggers() {
    ScrollTrigger.getAll().forEach(function (st) {
      var id = (st.vars && st.vars.id) || st.id || "";
      if (String(id).indexOf(triggerPrefix) === 0) st.kill();
    });
  }

  var introDone = false;

  function addRise(tl, row, immediate) {
    row.forEach(function (card, i) {
      tl.fromTo(
        card,
        { autoAlpha: 0, y: CARD_RISE.y },
        {
          autoAlpha: 1,
          y: 0,
          duration: CARD_RISE.duration,
          immediateRender: immediate
        },
        i * CARD_RISE.stagger
      );
    });
  }

  function rowTrigger(row, rowIndex) {
    // The first card is translated, so it cannot be its own trigger.
    // Measuring the grid keeps the first row's range stable on the way back up.
    return rowIndex === 0 ? grid : row[0];
  }

  function scrubRow(row, rowIndex) {
    var tl = gsap.timeline({
      defaults: { force3D: true, ease: "power2.out" },
      scrollTrigger: {
        id: triggerPrefix + "row-" + rowIndex,
        trigger: rowTrigger(row, rowIndex),
        scroller: stScroller,
        start: CARD_RISE.listStart,
        end: CARD_RISE.listEnd,
        scrub: CARD_RISE.scrub,
        invalidateOnRefresh: true
      }
    });
    addRise(tl, row, true);
  }

  function armFirstRow(row, rowIndex) {
    var seenEnd = false;
    var armed = false;
    var tl = gsap.timeline({
      paused: true,
      defaults: { force3D: true, ease: "power2.out" }
    });
    addRise(tl, row, true);
    tl.progress(1);

    var observer = ScrollTrigger.create({
      id: triggerPrefix + "row-" + rowIndex,
      trigger: rowTrigger(row, rowIndex),
      scroller: stScroller,
      start: CARD_RISE.listStart,
      end: CARD_RISE.listEnd,
      invalidateOnRefresh: false,
      onUpdate: function (self) {
        if (armed) return;
        if (!seenEnd) {
          tl.progress(1);
          if (self.progress >= 0.98) seenEnd = true;
          return;
        }
        if (self.progress >= 0.98) {
          tl.progress(1);
          return;
        }

        armed = true;
        observer.kill();
        var linked = {
          id: triggerPrefix + "row-" + rowIndex,
          trigger: rowTrigger(row, rowIndex),
          scroller: stScroller,
          start: CARD_RISE.listStart,
          end: CARD_RISE.listEnd,
          scrub: CARD_RISE.scrub,
          animation: tl,
          invalidateOnRefresh: false,
          immediateRender: false
        };
        ScrollTrigger.create(linked);
      },
      onRefresh: function () {
        if (!armed) tl.progress(1);
      }
    });
  }

  function playIntro(row, rowIndex) {
    gsap.fromTo(
      row,
      { autoAlpha: 0, y: 36 },
      {
        autoAlpha: 1,
        y: 0,
        duration: 0.32,
        stagger: 0.04,
        ease: "power2.out",
        force3D: true,
        overwrite: "auto",
        onComplete: function () {
          introDone = true;
          gsap.set(row, { autoAlpha: 1, y: 0 });
          armFirstRow(row, rowIndex);
        }
      }
    );
  }

  function setup(cards) {
    killCardTriggers();
    cards.forEach(function (card) {
      gsap.killTweensOf(card);
    });
    if (!cards.length) return;

    var cols = columns();
    for (var r = 0; r < cards.length; r += cols) {
      var row = cards.slice(r, r + cols);
      var rowIndex = r / cols;
      var onScreen = row[0].getBoundingClientRect().top < window.innerHeight * 0.98;

      if (rowIndex === 0 && onScreen && !introDone) {
        playIntro(row, rowIndex);
      } else {
        scrubRow(row, rowIndex);
      }
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

  setup(visibleCards());
  requestAnimationFrame(refreshTriggers);
  window.addEventListener("load", refreshTriggers);
  window.setTimeout(refreshTriggers, 400);

  var more = document.querySelector("[data-ar-more]");
  if (more) {
    more.addEventListener("click", function () {
      var extras = grid.querySelectorAll("[data-ar-extra]");
      extras.forEach(function (item) {
        item.hidden = false;
      });
      var wrap = more.closest(".ar-reports__more");
      if (wrap) wrap.hidden = true;
      rebuild();
    });
  }

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
