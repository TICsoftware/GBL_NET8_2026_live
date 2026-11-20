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

  /** Scrub rise — reverses on scroll up, plays again on scroll down. */
  function bindScrubRow(row, stScroller, options) {
    options = options || {};
    var tl = gsap.timeline({
      defaults: { force3D: true, ease: 'power2.out' },
      scrollTrigger: {
        id: 'card-rise-more-' + dynamicRiseId++,
        trigger: row[0],
        scroller: stScroller,
        start: CARD_RISE.listStart,
        end: CARD_RISE.listEnd,
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

  function playIntroThenScrub(row, stScroller) {
    row.forEach(function (card) {
      var iframe = isIframeCard(card);
      gsap.set(card, {
        autoAlpha: iframe ? 0.25 : 0,
        y: iframe ? CARD_RISE.yIframe : CARD_RISE.y,
        force3D: true,
      });
    });

    var intro = gsap.timeline({
      defaults: { force3D: true, ease: 'power2.out' },
    });
    row.forEach(function (card, i) {
      var iframe = isIframeCard(card);
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
      gsap.set(row, { autoAlpha: 1, y: 0 });
      /* Arm scrub at fully-risen so scroll-up reverses and scroll-down rises again. */
      bindScrubRow(row, stScroller, { immediateRender: false, progress: 1 });
      syncScrollLayout();
    });
  }

  /**
   * Load More / year filter / AJAX cards.
   * In-view: intro rise, then scrub for up/down.
   * Below-fold: scrub only (rise on scroll down, reverse on scroll up).
   */
  window.gblAnimateCardRise = function (nodes) {
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

    /* Year filter / full replace: drop prior list + load-more triggers. */
    if (list && typeof ScrollTrigger !== 'undefined') {
      var visibleInList = Array.prototype.slice.call(list.querySelectorAll('.card-rise')).filter(function (el) {
        return !el.hidden && !isStoryCard(el);
      });
      var replacingAll =
        visibleInList.length > 0 &&
        visibleInList.every(function (el) {
          return cards.indexOf(el) !== -1;
        });
      if (replacingAll) {
        ScrollTrigger.getAll().forEach(function (st) {
          if (st.trigger === list) {
            st.kill();
            return;
          }
          var id = (st.vars && st.vars.id) || st.id || '';
          if (String(id).indexOf('card-rise-more-') !== 0) return;
          if (st.trigger && list.contains(st.trigger)) st.kill();
        });
      }
    }

    function bindRows() {
      syncScrollLayout();

      for (var r = 0; r < cards.length; r += cols) {
        (function (row) {
          row.forEach(function (card) {
            var iframe = isIframeCard(card);
            gsap.set(card, {
              autoAlpha: iframe ? 0.25 : 0,
              y: iframe ? CARD_RISE.yIframe : CARD_RISE.y,
              force3D: true,
            });
          });

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
          start: CARD_RISE.listStart,
          end: CARD_RISE.listEnd,
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
