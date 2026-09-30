document.addEventListener('DOMContentLoaded', function () {
  /* ---------------------------------------
     STORIES IMAGE PARALLAX
     Image is taller than the frame. Scroll
     moves it up only — never down — so the
     top of the box stays covered and the
     extra height cannot sit on the text.
  --------------------------------------- */
  const wraps = Array.from(
    document.querySelectorAll('.storiesofChange .story-card__media.parallax-wrap')
  );
  if (!wraps.length) return;

  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  if (reduceMotion || typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') {
    return;
  }

  gsap.registerPlugin(ScrollTrigger);

  const isMobile = window.matchMedia('(max-width: 992px)').matches;
  const stScroller = isMobile ? window : document.documentElement;

  const cards = Array.from(document.querySelectorAll('.storiesofChange .stories-grid .story-card'));
  const mm = typeof gsap.matchMedia === 'function' ? gsap.matchMedia() : null;

  if (mm) {
    mm.add('(min-width: 1024px)', function () {
      var enters = [];
      cards.forEach(function (card) {
        var reverse = card.classList.contains('story-card--reverse');
        var tween = gsap.fromTo(
          card,
          { autoAlpha: 0, y: reverse ? -72 : 72 },
          {
            autoAlpha: 1,
            y: 0,
            ease: 'none',
            force3D: true,
            immediateRender: true,
            scrollTrigger: {
              trigger: card,
              scroller: stScroller,
              start: 'top 92%',
              end: 'top 58%',
              scrub: 1.1,
              invalidateOnRefresh: true,
            },
          }
        );
        enters.push(tween);
      });
      return function () {
        enters.forEach(function (tween) {
          if (tween.scrollTrigger) tween.scrollTrigger.kill();
          tween.kill();
        });
        gsap.set(cards, { clearProps: 'transform,opacity,visibility' });
      };
    });
  }

  function extraHeight(wrap, img) {
    var box = wrap.offsetHeight || 0;
    var photo = img.offsetHeight || 0;
    return Math.max(0, photo - box);
  }

  wraps.forEach(function (wrap) {
    const img = wrap.querySelector('.parallax-img') || wrap.querySelector('img');
    if (!img) return;

    gsap.fromTo(
      img,
      { y: 0 },
      {
        y: function () {
          return -extraHeight(wrap, img);
        },
        ease: 'none',
        force3D: true,
        immediateRender: true,
        scrollTrigger: {
          trigger: wrap,
          scroller: stScroller,
          start: 'top bottom',
          end: 'bottom top',
          scrub: 0.8,
          invalidateOnRefresh: true,
        },
      }
    );
  });

  function refreshStories() {
    ScrollTrigger.refresh();
  }

  requestAnimationFrame(refreshStories);
  window.addEventListener('load', refreshStories);
});
