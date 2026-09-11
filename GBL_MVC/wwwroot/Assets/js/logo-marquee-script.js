document.addEventListener('DOMContentLoaded', function () {
  const marquee = document.querySelector('.logo-marquee');
  if (!marquee) return;

  const track = marquee.querySelector('.logo-marquee-track');
  if (!track) return;

  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  const items = Array.from(track.querySelectorAll('.logo-item'));

  function getClone() {
    return marquee.querySelector('.logo-marquee-track[aria-hidden="true"]');
  }

  // Append a loop copy of the original track only (no GSAP styles)
  function cloneTrack() {
    if (track.dataset.marqueeCloned === '1') return;

    if (typeof gsap !== 'undefined') {
      gsap.set(items, { clearProps: 'transform,opacity,visibility' });
    }

    const clone = track.cloneNode(true);
    clone.setAttribute('aria-hidden', 'true');
    clone.querySelectorAll('img').forEach(function (img) {
      img.alt = '';
    });
    marquee.appendChild(clone);
    track.dataset.marqueeCloned = '1';
  }

  function stopMarquee() {
    marquee.classList.remove('is-marquee-ready');
    const clone = getClone();
    if (clone) clone.style.display = 'none';
  }

  function startMarquee() {
    cloneTrack();
    const clone = getClone();
    if (clone) clone.style.display = '';
    marquee.classList.add('is-marquee-ready');
  }

  function isInViewport(el) {
    const rect = el.getBoundingClientRect();
    const vh = window.innerHeight || document.documentElement.clientHeight;
    return rect.top < vh * 0.92 && rect.bottom > 40;
  }

  if (!items.length) return;

  if (reduceMotion || typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') {
    startMarquee();
    return;
  }

  gsap.registerPlugin(ScrollTrigger);

  const revealTl = gsap.timeline({
    paused: true,
    defaults: { force3D: true },
    onComplete: startMarquee,
  });

  // Original cards only: fade + slide in from the right
  revealTl.from(items, {
    autoAlpha: 0,
    x: 80,
    duration: 0.8,
    stagger: { each: 0.15, from: 'start', ease: 'power1.out' },
    ease: 'power3.out',
    overwrite: 'auto',
  });

  function playReveal() {
    stopMarquee();
    revealTl.timeScale(1);
    revealTl.play();
  }

  // Smooth exit: reverse the same fade / slide (not an instant hide)
  function exitReveal() {
    stopMarquee();
    revealTl.timeScale(0.75);
    revealTl.reverse();
  }

  const isMobile = window.matchMedia('(max-width: 992px)').matches;
  const stScroller = isMobile ? window : document.documentElement;

  ScrollTrigger.create({
    trigger: marquee,
    scroller: stScroller,
    start: 'top 80%',
    end: 'bottom 20%',
    invalidateOnRefresh: true,
    onEnter: playReveal,
    onEnterBack: playReveal,
    onLeave: exitReveal,
    onLeaveBack: exitReveal,
  });

  requestAnimationFrame(function () {
    ScrollTrigger.refresh();
    if (isInViewport(marquee)) playReveal();
  });

  window.addEventListener('load', function () {
    ScrollTrigger.refresh();
    if (isInViewport(marquee)) playReveal();
  });
});
