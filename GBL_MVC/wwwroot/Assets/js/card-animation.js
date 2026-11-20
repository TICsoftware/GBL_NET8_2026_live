/* ---------------------------------------
   CARD_RISE — smooth travel on scroll
   Cards in a .card-list-items group share
   one scrub, with a delay between each.

   Scroll down: cards rise in.
   Scroll back: cards travel back down
   with the scroll (previous behaviour).

   Iframe cards (.latest-li-card) use a
   lighter Y so embeds stay stable.

   Story cards use stories-card-animation.js

   Load More / AJAX: call window.gblAnimateCardRise(cards)

   Optional per-list overrides:
   data-card-start / data-card-end on .card-list-items
--------------------------------------- */
(function () {
  const CARD_RISE = {
    y: 140,
    yIframe: 72,
    duration: 1.15,
    stagger: 0.28,
    scrub: 1.15,
    listStart: 'top 90%',
    listEnd: 'top 38%',
    soloStart: 'top 92%',
    soloEnd: 'top 58%',
  };

  function isStoryCard(el) {
    return el.classList.contains('story-card') || !!el.closest('.stories-grid');
  }

  function isIframeCard(el) {
    return el.classList.contains('latest-li-card') || !!el.querySelector('iframe');
  }

  function gridColumns(list) {
    if (list && list.classList.contains('cg-docs__grid')) {
      if (window.matchMedia('(max-width: 640px)').matches) return 1;
      if (window.matchMedia('(max-width: 900px)').matches) return 2;
      return 3;
    }
    if (window.matchMedia('(max-width: 640px)').matches) return 1;
    if (window.matchMedia('(max-width: 1100px)').matches) return 2;
    return 3;
  }

  var dynamicRiseId = 0;

  function isRowInView(el) {
    var rect = el.getBoundingClientRect();
    var vh = window.innerHeight || document.documentElement.clientHeight;
    return rect.top < vh * 0.9 && rect.bottom > vh * 0.08;
  }

  function syncScrollLayout() {
    if (window.lenis && typeof window.lenis.resize === 'function') {
      try {
        window.lenis.resize();
      } catch (err) {}
    }
    if (typeof ScrollTrigger !== 'undefined') {
      ScrollTrigger.refresh();
    }
  }

  function addRiseTweens(tl, row, immediateRender) {
    row.forEach(function (card, i) {
      var iframe = isIframeCard(card);
      tl.fromTo(
        card,
        {
          autoAlpha: iframe ? 0.25 : 0,
          y: iframe ? CARD_RISE.yIframe : CARD_RISE.y,
        },
        {
          autoAlpha: 1,
          y: 0,
          duration: CARD_RISE.duration,
          immediateRender: !!immediateRender,
        },
        i * CARD_RISE.stagger
      );
    });
  }

  function listRiseRange(list) {
    return {
      start: (list && list.dataset.cardStart) || CARD_RISE.listStart,
      end: (list && list.dataset.cardEnd) || CARD_RISE.listEnd,
    };
  }

  /** Scrub rise — reverses on scroll up, plays again on scroll down. */
  function bindScrubRow(row, stScroller, options) {
    options = options || {};
    var list = row[0] && row[0].closest('.card-list-items');
    var range = listRiseRange(list);
    var tl = gsap.timeline({
      defaults: { force3D: true, ease: 'power2.out' },
      scrollTrigger: {
        id: 'card-rise-more-' + dynamicRiseId++,
        trigger: row[0],
        scroller: stScroller,
        start: range.start,
        end: range.end,
        scrub: CARD_RISE.scrub,
        invalidateOnRefresh: true,
      },
    });
    addRiseTweens(tl, row, options.immediateRender !== false);
    if (typeof options.progress === 'number') {
      tl.progress(options.progress);
    }
    return tl;
  }

  /**
   * After an intro, keep cards fully risen until the user scrolls up,
   * then link scrub so up/down animation works without a mid-range jump.
   */
  function armScrubAfterIntro(row, stScroller) {
    var list = row[0] && row[0].closest('.card-list-items');
    var range = listRiseRange(list);
    var rowId = 'card-rise-more-' + dynamicRiseId++;

    var tl = gsap.timeline({
      paused: true,
      defaults: { force3D: true, ease: 'power2.out' },
    });
    addRiseTweens(tl, row, false);
    tl.progress(1);
    gsap.set(row, { autoAlpha: 1, y: 0 });

    var lastProgress = null;
    var linked = false;

    ScrollTrigger.create({
      id: rowId,
      trigger: row[0],
      scroller: stScroller,
      start: range.start,
      end: range.end,
      invalidateOnRefresh: true,
      onUpdate: function (self) {
        if (linked) return;
        tl.progress(1);

        if (lastProgress === null) {
          lastProgress = self.progress;
          return;
        }

        /* User scrolled up — hand control to scrub. */
        if (self.progress < lastProgress - 0.03) {
          linked = true;
          self.kill();
          ScrollTrigger.create({
            id: rowId,
            trigger: row[0],
            scroller: stScroller,
            start: range.start,
            end: range.end,
            scrub: CARD_RISE.scrub,
            animation: tl,
            invalidateOnRefresh: true,
            immediateRender: false,
          });
          return;
        }

        lastProgress = Math.max(lastProgress, self.progress);
      },
      onRefresh: function () {
        if (!linked) tl.progress(1);
      },
    });
  }

  function hideCards(cards) {
    cards.forEach(function (card) {
      gsap.killTweensOf(card);
      var iframe = isIframeCard(card);
      gsap.set(card, {
        autoAlpha: iframe ? 0.25 : 0,
        y: iframe ? CARD_RISE.yIframe : CARD_RISE.y,
        force3D: true,
      });
    });
  }

  function playIntroThenScrub(row, stScroller) {
    hideCards(row);

    var intro = gsap.timeline({
      defaults: { force3D: true, ease: 'power2.out' },
    });
    row.forEach(function (card, i) {
      intro.to(
        card,
        {
          autoAlpha: 1,
          y: 0,
          duration: CARD_RISE.duration,
          overwrite: 'auto',
        },
        i * CARD_RISE.stagger
      );
    });

    intro.eventCallback('onComplete', function () {
      armScrubAfterIntro(row, stScroller);
      syncScrollLayout();
    });
  }

  /**
   * Year filter / full grid replace: one staggered intro for every on-screen
   * card, then arm scrub so scroll up/down reverses without fighting old triggers.
   */
  function playFilterIntroThenScrub(cards, cols, stScroller) {
    var inView = [];
    var below = [];

    for (var r = 0; r < cards.length; r += cols) {
      var row = cards.slice(r, r + cols);
      if (isRowInView(row[0])) inView = inView.concat(row);
      else below = below.concat(row);
    }

    hideCards(cards);

    function armAllScrubs() {
      for (var i = 0; i < inView.length; i += cols) {
        armScrubAfterIntro(inView.slice(i, i + cols), stScroller);
      }
      for (var j = 0; j < below.length; j += cols) {
        bindScrubRow(below.slice(j, j + cols), stScroller);
      }
      syncScrollLayout();
    }

    if (!inView.length) {
      armAllScrubs();
      return;
    }

    var intro = gsap.timeline({
      defaults: { force3D: true, ease: 'power2.out' },
    });

    inView.forEach(function (card, i) {
      intro.to(
        card,
        {
          autoAlpha: 1,
          y: 0,
          duration: CARD_RISE.duration,
          overwrite: 'auto',
        },
        /* One-by-one across every on-screen card. */
        i * CARD_RISE.stagger
      );
    });

    intro.eventCallback('onComplete', armAllScrubs);
  }

  function killListRiseTriggers(list) {
    if (!list || typeof ScrollTrigger === 'undefined') return;
    ScrollTrigger.getAll().forEach(function (st) {
      if (st.trigger === list) {
        st.kill();
        return;
      }
      var id = String((st.vars && st.vars.id) || st.id || '');
      if (id.indexOf('card-rise-more-') !== 0) return;
      if (st.trigger && list.contains(st.trigger)) st.kill();
    });
  }

  /**
   * Load More / year filter / AJAX cards.
   * options.replace = true for year filter (full grid rebuild).
   */
  window.gblAnimateCardRise = function (nodes, options) {
    options = options || {};
    var cards = Array.prototype.slice.call(nodes || []).filter(function (el) {
      return el && el.nodeType === 1 && !isStoryCard(el);
    });
    if (!cards.length) return;

    if (typeof gsap === 'undefined') {
      cards.forEach(function (card) {
        card.style.opacity = '1';
        card.style.transform = 'none';
        card.style.visibility = 'visible';
      });
      return;
    }

    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
      gsap.set(cards, { clearProps: 'transform,opacity,visibility' });
      return;
    }

    gsap.registerPlugin(ScrollTrigger);

    var list = cards[0].closest('.card-list-items') || cards[0].parentElement;
    var cols = gridColumns(list);
    var isMobile = window.matchMedia('(max-width: 992px)').matches;
    var stScroller = isMobile ? window : document.documentElement;

    var visibleInList = list
      ? Array.prototype.slice.call(list.querySelectorAll('.card-rise')).filter(function (el) {
          return !el.hidden && !isStoryCard(el);
        })
      : cards;
    var replacingAll =
      !!options.replace ||
      (visibleInList.length > 0 &&
        visibleInList.every(function (el) {
          return cards.indexOf(el) !== -1;
        }));

    if (replacingAll) {
      killListRiseTriggers(list);
    }

    function bindRows() {
      syncScrollLayout();

      if (replacingAll) {
        playFilterIntroThenScrub(cards, cols, stScroller);
        return;
      }

      for (var r = 0; r < cards.length; r += cols) {
        (function (row) {
          hideCards(row);
          if (isRowInView(row[0])) {
            playIntroThenScrub(row, stScroller);
          } else {
            bindScrubRow(row, stScroller);
          }
        })(cards.slice(r, r + cols));
      }

      requestAnimationFrame(syncScrollLayout);
    }

    requestAnimationFrame(function () {
      requestAnimationFrame(bindRows);
    });
  };

  document.addEventListener('DOMContentLoaded', function () {
    if (typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') return;

    const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (reduceMotion) return;

    gsap.registerPlugin(ScrollTrigger);

    const isMobile = window.matchMedia('(max-width: 992px)').matches;
    const stScroller = isMobile ? window : document.documentElement;

    /** Direct .card-rise children of one list (not nested lists, not stories). */
    function listCards(list) {
      return Array.from(list.querySelectorAll('.card-rise')).filter(function (el) {
        if (isStoryCard(el)) return false;
        if (el.hidden) return false;
        return el.closest('.card-list-items') === list;
      });
    }

    /** Grouped lists: one scrubbed timeline, stagger between cards in the row. */
    document.querySelectorAll('.card-list-items').forEach(function (list, listIndex) {
      const cards = listCards(list);
      if (!cards.length) return;

      const tl = gsap.timeline({
        defaults: { force3D: true, ease: 'power2.out' },
        scrollTrigger: {
          id: 'card-rise-list-' + listIndex,
          trigger: list,
          scroller: stScroller,
          /* Optional per-list override, e.g. data-card-end="top 75%" for a list
             that sits low in its section and would otherwise stop part-way. */
          start: list.dataset.cardStart || CARD_RISE.listStart,
          end: list.dataset.cardEnd || CARD_RISE.listEnd,
          scrub: CARD_RISE.scrub,
          invalidateOnRefresh: true,
        },
      });

      cards.forEach(function (card, i) {
        const iframe = isIframeCard(card);
        tl.fromTo(
          card,
          {
            autoAlpha: iframe ? 0.25 : 0,
            y: iframe ? CARD_RISE.yIframe : CARD_RISE.y,
          },
          {
            autoAlpha: 1,
            y: 0,
            duration: CARD_RISE.duration,
            immediateRender: true,
          },
          i * CARD_RISE.stagger
        );
        card.dataset.cardRisePlayed = '1';
      });
    });

    /** Standalone .card-rise cards (not inside a .card-list-items group). */
    Array.from(document.querySelectorAll('.card-rise')).forEach(function (card, i) {
      if (isStoryCard(card)) return;
      if (card.closest('.card-list-items')) return;

      gsap.fromTo(
        card,
        { autoAlpha: 0, y: CARD_RISE.y },
        {
          autoAlpha: 1,
          y: 0,
          ease: 'power2.out',
          force3D: true,
          immediateRender: true,
          scrollTrigger: {
            id: 'card-rise-solo-' + i,
            trigger: card,
            scroller: stScroller,
            start: CARD_RISE.soloStart,
            end: CARD_RISE.soloEnd,
            scrub: CARD_RISE.scrub,
            invalidateOnRefresh: true,
          },
        }
      );
      card.dataset.cardRisePlayed = '1';
    });

    /** Recalc start/end after layout, fonts, or iframe height change. */
    function refreshTriggers() {
      ScrollTrigger.refresh();
    }

    requestAnimationFrame(refreshTriggers);
    window.addEventListener('load', refreshTriggers);
    window.setTimeout(refreshTriggers, 400);
    window.setTimeout(refreshTriggers, 1200);

    document.querySelectorAll('.latest-li-card iframe').forEach(function (frame) {
      frame.addEventListener('load', refreshTriggers);
    });
  });
})();
