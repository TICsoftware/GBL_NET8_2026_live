document.addEventListener('DOMContentLoaded', function () {
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
  --------------------------------------- */
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

  
  if (typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') return;

  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  if (reduceMotion) return;

  gsap.registerPlugin(ScrollTrigger);

  const isMobile = window.matchMedia('(max-width: 992px)').matches;
  const stScroller = isMobile ? window : document.documentElement;

  /** Skip story cards — they are animated in stories-card-animation.js. */
  function isStoryCard(el) {
    return el.classList.contains('story-card') || !!el.closest('.stories-grid');
  }

  /** LinkedIn / embed cards need a smaller Y so the iframe stays stable. */
  function isIframeCard(el) {
    return el.classList.contains('latest-li-card') || !!el.querySelector('iframe');
  }

  /** Direct .card-rise children of one list (not nested lists, not stories). */
  function listCards(list) {
    return Array.from(list.querySelectorAll('.card-rise')).filter(function (el) {
      if (isStoryCard(el)) return false;
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
