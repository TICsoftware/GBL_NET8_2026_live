document.addEventListener('DOMContentLoaded', function () {
  /* ---------------------------------------
     STORIES CARD ANIMATION
     Odd cards drop from above; even cards
     rise from below. Plays on enter and
     re-enter; reverses smoothly on leave.
  --------------------------------------- */
  const cards = Array.from(document.querySelectorAll('.stories-grid .story-card'));
  if (!cards.length) return;

  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  if (reduceMotion || typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') {
    return;
  }

  gsap.registerPlugin(ScrollTrigger);

  // Match Lenis / common-script ScrollTrigger scroller
  const isMobile = window.matchMedia('(max-width: 992px)').matches;
  const stScroller = isMobile ? window : document.documentElement;

  const cardApis = cards.map(function (card, i) {
    // Odd cards drop from above; even cards rise from below
    const fromY = i % 2 === 0 ? -56 : 56;

    const tl = gsap.timeline({
      paused: true,
      defaults: { force3D: true },
    });

    tl.fromTo(
      card,
      { autoAlpha: 0, y: fromY },
      {
        autoAlpha: 1,
        y: 0,
        duration: 0.8,
        ease: 'power3.out',
        overwrite: 'auto',
      }
    );

    function playEnter() {
      tl.timeScale(1);
      tl.play();
    }

    function playExit() {
      tl.timeScale(0.75);
      tl.reverse();
    }

    ScrollTrigger.create({
      trigger: card,
      scroller: stScroller,
      start: 'top 88%',
      end: 'bottom 12%',
      invalidateOnRefresh: true,
      onEnter: playEnter,
      onEnterBack: playEnter,
      onLeave: playExit,
      onLeaveBack: playExit,
    });

    return { card: card, playEnter: playEnter };
  });

  function isInViewport(el) {
    const rect = el.getBoundingClientRect();
    const vh = window.innerHeight || document.documentElement.clientHeight;
    return rect.top < vh * 0.88 && rect.bottom > 40;
  }

  function syncVisible() {
    cardApis.forEach(function (api) {
      if (isInViewport(api.card)) api.playEnter();
    });
  }

  requestAnimationFrame(function () {
    ScrollTrigger.refresh();
    syncVisible();
  });

  window.addEventListener('load', function () {
    ScrollTrigger.refresh();
    syncVisible();
  });
});
