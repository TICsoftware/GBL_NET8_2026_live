document.addEventListener("DOMContentLoaded", () => {

  // Reads #slidesData — each article is one real slide (image + copy).
  const timeline = [...document.querySelectorAll('#slidesData > article')].map((el) => {
    const imgEl = el.querySelector('img');
    let img = '';
    if (imgEl) {
      let raw = (imgEl.getAttribute('src') || '').trim();
      if (raw.startsWith('~/')) raw = raw.slice(1);
      img = raw || imgEl.src;
    }
    return {
      year: el.dataset.year,
      img,
      title: (el.querySelector('h3')?.textContent || '').trim(),
      desc: (el.querySelector('p')?.textContent || '').trim(),
    };
  });

  let activeIndex = 0;
  let hasRendered = false;

  const MAJOR_X = [1.4, 141.58, 281.58, 421.43, 561.43, 701.43, 841.44, 981.45, 1121.45, 1246.59, 1386.59, 1526.60, 1666.61];
  const ALIGN_X_FALLBACK = 24;
  const LINE_START_DELAY = 380;
  const mqDesktop = window.matchMedia('(min-width: 1024px)');

  const pageContainerLegacy = document.getElementById('pageContainerLegacy');
  const rulerViewport = document.getElementById('rulerViewport');
  const ticksTrack = document.getElementById('ticksTrack');
  const labelsLayer = document.getElementById('labelsLayer');
  const dotRef = document.getElementById('dotRef');
  const tickRef = document.getElementById('tickRef');
  const connectorLine = document.getElementById('connectorLine');
  const connectorDot = document.getElementById('connectorDot');
  const bgYearEl = document.getElementById('bgYear');
  const eventContent = document.getElementById('eventContent');
  const eventTitle = document.getElementById('eventTitle');
  const eventDesc = document.getElementById('eventDesc');
  const mediaViewport = document.getElementById('legacyMediaViewport');
  const mediaTrack = document.getElementById('legacyMediaTrack');
  const prevBtn = document.getElementById('prevBtn');
  const nextBtn = document.getElementById('nextBtn');

  if (!pageContainerLegacy || !timeline.length || !mediaViewport || !mediaTrack) return;

  let ALIGN_X = ALIGN_X_FALLBACK;
  let mediaSlides = [];
  let previousIndex = 0;
  const mqReduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)');
  const mediaBlock = pageContainerLegacy.querySelector('.legacy-media');

  function buildMediaTrack() {
    mediaTrack.innerHTML = '';
    mediaSlides = timeline.map((d, i) => {
      const slide = document.createElement('div');
      slide.className = 'legacy-media-slide';
      slide.dataset.index = String(i);
      const img = document.createElement('img');
      img.src = d.img;
      img.alt = d.title;
      img.decoding = 'async';
      img.loading = i === 0 ? 'eager' : 'lazy';
      slide.appendChild(img);
      mediaTrack.appendChild(slide);
      return slide;
    });
  }

  /* Depth via opacity only — X offsets opened white gaps between slides */
  function applySlideDepth(animate) {
    const reduceMotion = mqReduceMotion.matches;
    const dir = animate && !reduceMotion
      ? (activeIndex > previousIndex ? 1 : activeIndex < previousIndex ? -1 : 0)
      : 0;

    mediaSlides.forEach((slide, i) => {
      const delta = i - activeIndex;
      slide.classList.toggle('is-active', delta === 0);
      slide.classList.toggle('is-peek', delta === 1);
      slide.classList.toggle('is-away', Math.abs(delta) > 1);

      /* Keep transforms at 0 so slide gap stays exact */
      if (reduceMotion || !(animate && i === activeIndex && dir !== 0)) {
        slide.style.transform = 'translate3d(0, 0, 0)';
        return;
      }

      /* Brief settle on active only — does not leave a resting gap */
      slide.style.transition = 'none';
      slide.style.transform = `translate3d(${dir * -24}px, 0, 0)`;
      void slide.offsetWidth;
      slide.style.transition = '';
      requestAnimationFrame(() => {
        slide.style.transform = 'translate3d(0, 0, 0)';
      });
    });
  }

  function syncMediaTrack(animate) {
    const isLast = activeIndex >= timeline.length - 1;
    pageContainerLegacy.classList.toggle('is-legacy-last', isLast);

    const styles = getComputedStyle(pageContainerLegacy.querySelector('.legacy-top') || pageContainerLegacy);
    const peekRaw = styles.getPropertyValue('--legacy-peek').trim();
    const gapRaw = styles.getPropertyValue('--legacy-slide-gap').trim();
    const rootFs = parseFloat(getComputedStyle(document.documentElement).fontSize) || 16;
    const toPx = (v) => {
      if (!v) return 0;
      const n = parseFloat(v);
      if (!Number.isFinite(n)) return 0;
      if (v.endsWith('rem')) return n * rootFs;
      return n;
    };

    const peek = mqDesktop.matches ? toPx(peekRaw) : 0;
    const gap = mqDesktop.matches ? toPx(gapRaw) : 0;
    const viewportW = mediaViewport.clientWidth;
    const mainW = Math.max(1, viewportW - peek - gap);
    const lastIndex = mediaSlides.length - 1;

    mediaSlides.forEach((slide, i) => {
      const w = isLast && i === lastIndex ? viewportW : mainW;
      slide.style.flex = `0 0 ${w}px`;
      slide.style.width = w + 'px';
    });

    applySlideDepth(animate);

    const x = -activeIndex * (mainW + gap);
    if (!animate) {
      mediaTrack.style.transition = 'none';
      mediaTrack.style.transform = `translateX(${x}px)`;
      void mediaTrack.offsetWidth;
      mediaTrack.style.transition = '';
    } else {
      mediaTrack.style.transform = `translateX(${x}px)`;
    }

    previousIndex = activeIndex;
  }

  function initMediaScrollParallax() {
    if (!mediaBlock || typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') return;
    if (mqReduceMotion.matches || !mqDesktop.matches) return;

    gsap.registerPlugin(ScrollTrigger);
    gsap.fromTo(
      mediaBlock,
      { y: 28 },
      {
        y: -28,
        ease: 'none',
        scrollTrigger: {
          trigger: pageContainerLegacy,
          start: 'top bottom',
          end: 'bottom top',
          scrub: 0.45,
        },
      }
    );
  }

  function buildTicks() {
    labelsLayer.innerHTML = '';
    timeline.forEach((d, i) => {
      const x = MAJOR_X[i] !== undefined ? MAJOR_X[i] : MAJOR_X[MAJOR_X.length - 1];
      const label = document.createElement('span');
      label.className = `year-label absolute text-[11px] whitespace-nowrap tick-btn ${i === activeIndex ? 'active' : 'inactive'}`;
      label.style.left = x + 'px';
      label.style.transform = 'translateX(-50%)';
      label.textContent = d.year;
      label.dataset.label = i;
      label.addEventListener('click', () => goTo(i));
      labelsLayer.appendChild(label);
    });
  }

  function positionTrack() {
    const x = MAJOR_X[activeIndex] !== undefined ? MAJOR_X[activeIndex] : MAJOR_X[MAJOR_X.length - 1];
    ticksTrack.style.transform = `translateX(${ALIGN_X - x}px)`;
  }

  function positionConnector(animate, delay) {
    delay = delay || 0;
    const containerRect = pageContainerLegacy.getBoundingClientRect();
    const dotRect = dotRef.getBoundingClientRect();
    const rulerRect = rulerViewport.getBoundingClientRect();

    const dotX = dotRect.left - containerRect.left;
    const dotY = dotRect.top - containerRect.top;

    ALIGN_X = dotX - (rulerRect.left - containerRect.left);
    tickRef.style.left = ALIGN_X + 'px';
    positionTrack();

    const tickRect = tickRef.getBoundingClientRect();
    const tickY = tickRect.top - containerRect.top;
    const fullHeight = Math.max(0, tickY - dotY);
    const bottomOffset = containerRect.height - tickY;

    connectorDot.style.left = dotX + 'px';
    connectorDot.style.top = dotY + 'px';
    connectorLine.style.left = dotX + 'px';
    connectorLine.style.top = '';
    connectorLine.style.bottom = bottomOffset + 'px';

    if (animate) {
      connectorDot.style.transition = 'none';
      connectorDot.style.top = tickY + 'px';
      connectorDot.style.opacity = '0';
      connectorLine.style.transition = 'none';
      connectorLine.style.height = '0px';
      void connectorLine.offsetWidth;

      const start = () => requestAnimationFrame(() => {
        connectorLine.style.transition = 'height .7s cubic-bezier(.22,.61,.36,1)';
        connectorLine.style.height = fullHeight + 'px';
        connectorDot.style.transition = 'top .7s cubic-bezier(.22,.61,.36,1), opacity .25s ease';
        connectorDot.style.top = dotY + 'px';
        connectorDot.style.opacity = '1';
      });

      if (delay > 0) setTimeout(start, delay);
      else start();
    } else {
      connectorLine.style.transition = 'none';
      connectorLine.style.height = fullHeight + 'px';
      connectorDot.style.transition = 'none';
      connectorDot.style.top = dotY + 'px';
      connectorDot.style.opacity = '1';
      requestAnimationFrame(() => {
        connectorLine.style.transition = '';
        connectorDot.style.transition = '';
      });
    }
  }

  function updateLabels() {
    [...labelsLayer.querySelectorAll('.year-label')].forEach((el, i) => {
      el.classList.toggle('active', i === activeIndex);
      el.classList.toggle('inactive', i !== activeIndex);
    });
  }

  function render() {
    const d = timeline[activeIndex];
    const animateMedia = hasRendered;

    if (!hasRendered) {
      bgYearEl.textContent = d.year;
      bgYearEl.classList.add('bgyear-enter');
      eventTitle.textContent = d.title;
      eventDesc.textContent = d.desc;
      eventContent.classList.add('fade-slide-enter');
      syncMediaTrack(false);
      updateLabels();
      positionConnector(true, LINE_START_DELAY);
      prevBtn.disabled = activeIndex === 0;
      nextBtn.disabled = activeIndex === timeline.length - 1;
      hasRendered = true;
      return;
    }

    bgYearEl.classList.remove('bgyear-enter');
    void bgYearEl.offsetWidth;
    bgYearEl.textContent = d.year;
    bgYearEl.classList.add('bgyear-enter');

    eventContent.classList.remove('fade-slide-enter');
    void eventContent.offsetWidth;
    eventTitle.textContent = d.title;
    eventDesc.textContent = d.desc;
    eventContent.classList.add('fade-slide-enter');

    syncMediaTrack(animateMedia);
    updateLabels();
    positionConnector(true, LINE_START_DELAY);

    prevBtn.disabled = activeIndex === 0;
    nextBtn.disabled = activeIndex === timeline.length - 1;
  }

  function goTo(i) {
    if (i < 0 || i >= timeline.length || i === activeIndex) return;
    activeIndex = i;
    render();
  }

  prevBtn.addEventListener('click', () => goTo(activeIndex - 1));
  nextBtn.addEventListener('click', () => goTo(activeIndex + 1));

  /* Media drag/swipe → same goTo() as arrows (touch + desktop pointer) */
  (function bindMediaSwipe() {
    const swipeTarget = mediaViewport;
    if (!swipeTarget) return;

    const SWIPE_MIN = 48;
    const DECIDE_MIN = 10;
    let startX = 0;
    let startY = 0;
    let tracking = false;
    let decided = false;
    let isHorizontal = false;
    let pointerId = null;

    swipeTarget.style.touchAction = 'pan-y';
    swipeTarget.classList.add('legacy-media-draggable');

    function resetDrag() {
      tracking = false;
      decided = false;
      isHorizontal = false;
      pointerId = null;
      swipeTarget.classList.remove('is-dragging');
    }

    swipeTarget.addEventListener('pointerdown', (e) => {
      if (e.pointerType === 'mouse' && e.button !== 0) return;
      tracking = true;
      decided = false;
      isHorizontal = false;
      pointerId = e.pointerId;
      startX = e.clientX;
      startY = e.clientY;
    });

    swipeTarget.addEventListener('pointermove', (e) => {
      if (!tracking || e.pointerId !== pointerId) return;
      const dx = e.clientX - startX;
      const dy = e.clientY - startY;

      if (!decided) {
        if (Math.abs(dx) < DECIDE_MIN && Math.abs(dy) < DECIDE_MIN) return;
        decided = true;
        isHorizontal = Math.abs(dx) > Math.abs(dy);
        if (!isHorizontal) {
          resetDrag();
          return;
        }
        swipeTarget.classList.add('is-dragging');
        try { swipeTarget.setPointerCapture(e.pointerId); } catch (_) {}
      }
    });

    function endDrag(e) {
      if (!tracking || (pointerId !== null && e.pointerId !== pointerId)) return;
      const dx = e.clientX - startX;
      const dy = e.clientY - startY;
      const wasHorizontal = decided ? isHorizontal : Math.abs(dx) > Math.abs(dy);
      const didDrag = decided && isHorizontal;
      resetDrag();

      if (!wasHorizontal) return;
      if (Math.abs(dx) < SWIPE_MIN) return;
      /* Ignore tiny accidental clicks mistaken as drag */
      if (!didDrag && e.pointerType === 'mouse' && Math.abs(dx) < SWIPE_MIN) return;

      if (dx < 0) goTo(activeIndex + 1);
      else goTo(activeIndex - 1);
    }

    swipeTarget.addEventListener('pointerup', endDrag);
    swipeTarget.addEventListener('pointercancel', resetDrag);
  })();

  let resizeTimer;
  window.addEventListener('resize', () => {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(() => {
      syncMediaTrack(false);
      positionConnector(false);
    }, 120);
  });
  window.addEventListener('load', () => {
    syncMediaTrack(false);
    positionConnector(false);
  });
  if (typeof mqDesktop.addEventListener === 'function') {
    mqDesktop.addEventListener('change', () => syncMediaTrack(false));
  }

  buildMediaTrack();
  buildTicks();
  render();
  initMediaScrollParallax();
});
