/* ---------------------------------------
   IMAGE PARALLAX — site-wide
   Markup: <div class="parallax-wrap"><img class="parallax-img" ...></div>
   (see .parallax-wrap / .parallax-img / .bg-parallax-section in common-style.css)
--------------------------------------- */
document.addEventListener('DOMContentLoaded', function () {
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  if (reduceMotion || typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') {
    return;
  }

  gsap.registerPlugin(ScrollTrigger);

  gsap.utils.toArray('.parallax-wrap').forEach(function (wrap) {
    // Enlarge section owns motion on desktop — skip y-parallax there
    if (wrap.classList.contains('enlarge-wrapper')) return;
    if (wrap.closest('.ourPhilosophy')) return;
    if (wrap.closest('.storiesofChange')) return;

    const img = wrap.querySelector('.parallax-img');
    if (!img) return;

    var isMobileView = window.matchMedia('(max-width: 992px)').matches;
    var stScroller = isMobileView ? window : document.documentElement;

    // Balanced travel (covers overflow:hidden + height:130% CSS)
    gsap.fromTo(
      img,
      { yPercent: -12 },
      {
        yPercent: 12,
        ease: 'none',
        force3D: true,
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
});
