document.addEventListener("DOMContentLoaded", () => {
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

/* ---------------------------------------
   Sections below #life-inside must wait for its pinned ScrollTrigger
   (created asynchronously in lifeinside-script.js, after the 120-frame
   sequence preloads) before THEY create their own ScrollTriggers.
   Otherwise these get measured against the page's pre-pin height and
   ScrollTrigger.refresh() afterwards does not correct them — the CTA
   parallax, culture tile parallax and man-cutout reveal all end up
   scrubbing against stale start/end offsets (looks frozen/broken on
   both desktop and mobile). Creating them fresh once the pin exists
   always measures correctly, so we simply delay creation instead.
--------------------------------------- */
  function initSectionScrollFx() {

/* ---------------------------------------
   PARALLAX IMAGE
--------------------------------------- */
  if (!reduceMotion && typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    /* CTA band — wrap bleed parallax (no image zoom / 130%) */
    const ctaSection = document.querySelector('.bg-parallax-section');
    const ctaWrap = ctaSection && ctaSection.querySelector('.parallax-wrap');
    const ctaImg = ctaWrap && ctaWrap.querySelector('.parallax-img');

    if (ctaImg && ctaWrap) {
      const frame = ctaSection.querySelector('.bg-parallax-frame') || ctaSection;
      const bleed = () => {
        const raw = getComputedStyle(frame).getPropertyValue('--cta-parallax-bleed');
        const value = parseFloat(raw);
        return Number.isFinite(value) ? value : 40;
      };

      gsap.fromTo(
        ctaWrap,
        { y: () => bleed() },
        {
          y: () => -bleed(),
          ease: 'none',
          force3D: true,
          scrollTrigger: {
            trigger: ctaSection,
            start: 'top bottom',
            end: 'bottom top',
            scrub: 0.8,
            invalidateOnRefresh: true,
          },
        }
      );

      /* Re-measure after the lazy bg image loads (start/end can be wrong before) */
      if (!ctaImg.complete) {
        ctaImg.addEventListener(
          'load',
          () => ScrollTrigger.refresh(),
          { once: true }
        );
      }
    }

    gsap.utils.toArray('.parallax-wrap').forEach((wrap) => {
      // Enlarge section owns motion on desktop — skip y-parallax there
      if (wrap.classList.contains('enlarge-wrapper')) return;
      // Culture collage: keep photos static (no crop/scale on scroll)
      if (wrap.closest('.workplaceculture-section')) return;
      // CTA handled above with a stronger range
      if (wrap.closest('.bg-parallax-section')) return;

      const img = wrap.querySelector('.parallax-img');
      if (!img) return;

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
            start: 'top bottom',
            end: 'bottom top',
            scrub: 0.8,
            invalidateOnRefresh: true,
          },
        }
      );
    });
  }

/* ---------------------------------------
   WORKPLACE CULTURE — desktop tile parallax
   Moves the photo frames (not the img). Soft/off on zoomed
   (narrower CSS) viewports so the collage stays tidy.
--------------------------------------- */
  if (!reduceMotion && typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    const cultureMm = gsap.matchMedia();

    function initCultureTileParallax(travelScale) {
      const section = document.querySelector('.workplaceculture-section');
      if (!section) return () => {};

      const tweens = [];
      const travelByClass = {
        'culture-photo--1': 28,
        'culture-photo--2': 42,
        'culture-photo--3': 22,
        'culture-photo--4': 32,
      };

      section.querySelectorAll('.culture-photo').forEach((photo) => {
        const key = Object.keys(travelByClass).find((c) => photo.classList.contains(c));
        const travel = (key ? travelByClass[key] : 28) * travelScale;

        const tween = gsap.fromTo(
          photo,
          { y: travel },
          {
            y: -travel,
            ease: 'none',
            force3D: true,
            scrollTrigger: {
              trigger: section,
              start: 'top bottom',
              end: 'bottom top',
              scrub: 0.9,
              invalidateOnRefresh: true,
            },
          }
        );

        tweens.push(tween);
      });

      return () => {
        tweens.forEach((t) => {
          if (t.scrollTrigger) t.scrollTrigger.kill();
          t.kill();
        });
        section.querySelectorAll('.culture-photo').forEach((photo) => {
          gsap.set(photo, { clearProps: 'transform' });
        });
      };
    }

    // Full travel — large / 100% desktop
    cultureMm.add('(min-width: 1601px)', () => initCultureTileParallax(1));

    // Soft travel — ~125% zoom / laptop widths
    cultureMm.add('(min-width: 1367px) and (max-width: 1600px)', () =>
      initCultureTileParallax(0.45)
    );

    // Off below ~1366 CSS px (~150% on 1920 / smaller laptops)
    // collage still shows; tiles stay locked in the grid
  }


/* ---------------------------------------
   MAN CUTOUT — rise + fade on viewport enter
--------------------------------------- */
  if (typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    const manWrap = document.querySelector('.bg-parallax-section .man-image-wrap');
    const section = document.querySelector('.bg-parallax-section');

    if (manWrap && section) {
      const manMm = gsap.matchMedia();

      manMm.add('(min-width: 768px)', () => {
        if (reduceMotion) {
          gsap.set(manWrap, { clearProps: 'transform,opacity,visibility' });
          return;
        }

        gsap.set(manWrap, {
          y: 100,
          autoAlpha: 0.95,
          force3D: true,
        });

        const tween = gsap.to(manWrap, {
          y: 0,
          autoAlpha: 1,
          duration: 1.15,
          ease: 'power3.out',
          force3D: true,
          scrollTrigger: {
            trigger: section,
            start: 'top 80%',
            toggleActions: 'play none none reverse',
            invalidateOnRefresh: true,
          },
        });

        return () => {
          if (tween.scrollTrigger) tween.scrollTrigger.kill();
          tween.kill();
          gsap.set(manWrap, { clearProps: 'transform,opacity,visibility' });
        };
      });

      /* Mobile: no rise offset — it left a gap above the people */
      manMm.add('(max-width: 767px)', () => {
        gsap.set(manWrap, { clearProps: 'transform,opacity,visibility' });
      });
    }
  }

  } // end initSectionScrollFx

  (function scheduleSectionScrollFx() {
    let started = false;
    function start() {
      if (started) return;
      started = true;
      initSectionScrollFx();
    }

    if (document.getElementById('life-inside')) {
      window.addEventListener('lifeinside:ready', start, { once: true });
      // Safety net in case the frame sequence never resolves (e.g. every
      // frame request fails) so the CTA/culture/man-cutout effects still
      // end up correctly measured rather than never initializing at all.
      window.setTimeout(start, 4000);
    } else {
      start();
    }
  })();

/* ---------------------------------------
   PRODUCTS LION — scale up + stroke draw → fill
--------------------------------------- */
  if (!reduceMotion && typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    const lionWrap = document.querySelector('.lion-logo-wrap');
    const lionSvg = lionWrap && lionWrap.querySelector('.lion-logo-svg');
    const lionFill = lionSvg && lionSvg.querySelector('.lion-logo-fill');

    if (lionWrap && lionSvg && lionFill) {
      // Clone fill path as a stroke outline for the line-draw
      let lionStroke = lionSvg.querySelector('.lion-logo-stroke');
      if (!lionStroke) {
        lionStroke = lionFill.cloneNode();
        lionStroke.removeAttribute('fill');
        lionStroke.classList.remove('lion-logo-fill');
        lionStroke.classList.add('lion-logo-stroke');
        lionStroke.setAttribute('fill', 'none');
        lionStroke.setAttribute('stroke', '#C8C8C8');
        lionStroke.setAttribute('stroke-width', '1.75');
        lionStroke.setAttribute('stroke-linecap', 'round');
        lionStroke.setAttribute('stroke-linejoin', 'round');
        lionStroke.setAttribute('vector-effect', 'non-scaling-stroke');
        lionSvg.insertBefore(lionStroke, lionFill);
      }

      const pathLen = (() => {
        try {
          return lionStroke.getTotalLength();
        } catch (e) {
          return 0;
        }
      })();

      if (pathLen > 0) {
        gsap.set(lionWrap, {
          scale: 0.05,
          transformOrigin: '50% 50%',
          force3D: true,
        });
        gsap.set(lionStroke, {
          strokeDasharray: pathLen,
          strokeDashoffset: pathLen,
          autoAlpha: 1,
        });
        gsap.set(lionFill, { autoAlpha: 0 });

        // Trigger on the lion itself — section top fires too early
        // while the logo sits at the bottom of the products grid.
        gsap
          .timeline({
            scrollTrigger: {
              trigger: lionWrap,
              start: 'top 85%',
              toggleActions: 'play none none reverse',
              invalidateOnRefresh: true,
            },
          })
          .to(
            lionWrap,
            {
              scale: 1,
              duration: 1.35,
              ease: 'power2.out',
              force3D: true,
            },
            0
          )
          .to(
            lionStroke,
            {
              strokeDashoffset: 0,
              duration: 1.35,
              ease: 'power2.inOut',
            },
            0
          )
          .to(
            lionFill,
            { autoAlpha: 1, duration: 0.55, ease: 'power2.out' },
            '-=0.3'
          )
          .to(
            lionStroke,
            { autoAlpha: 0, duration: 0.4, ease: 'power1.out' },
            '-=0.35'
          );
      }
    }
  }

/* ---------------------------------------
   TESTIMONIALS
   Desktop: opposite vertical marquee + hover pause + drag/swipe
   Mobile: static list + Load more
   Careers-only — homepage-script.js untouched
--------------------------------------- */
  function makeLoopable(trackId) {
    const track = document.getElementById(trackId);
    if (!track) return;

    if (track.dataset.loopReady === '1') return;

    const originalCards = Array.from(track.children).filter(
      (el) => el.classList.contains('quote-card') && el.getAttribute('aria-hidden') !== 'true'
    );

    originalCards.forEach((card) => {
      const clone = card.cloneNode(true);
      clone.setAttribute('aria-hidden', 'true');
      clone.querySelectorAll('a, button, input, [tabindex]').forEach((el) =>
        el.setAttribute('tabindex', '-1')
      );
      track.appendChild(clone);
    });

    track.dataset.loopReady = '1';
  }

  const sectionMarquee = document.getElementById('marquee-section');
  const loadMoreBtn = document.getElementById('load-more-btn');
  const testimonialsDesktopMq = window.matchMedia('(min-width: 1024px)');
  const MOBILE_BATCH_SIZE = 3;
  let mobileRevealedCount = 0;
  let marqueeObserver = null;
  const marqueeColumns = [];

  function getOriginalQuoteCards() {
    if (!sectionMarquee) return [];
    return Array.from(
      sectionMarquee.querySelectorAll('.quote-card:not([aria-hidden="true"])')
    );
  }

  let mobileQuoteRiseTriggers = [];
  /* Life Inside pin changes document height; creating rise STs before
     lifeinside:ready leaves stale start/end (refresh does not fix — same
     as CTA/culture in initSectionScrollFx). */
  let pinLayoutReady = !document.getElementById('life-inside');

  function whenPinLayoutReady(fn) {
    if (pinLayoutReady) {
      fn();
      return;
    }
    let done = false;
    const run = () => {
      if (done) return;
      done = true;
      pinLayoutReady = true;
      fn();
    };
    window.addEventListener('lifeinside:ready', run, { once: true });
    window.setTimeout(run, 4000);
  }

  function killMobileQuoteRise() {
    mobileQuoteRiseTriggers.forEach((st) => {
      if (st && typeof st.kill === 'function') st.kill();
    });
    mobileQuoteRiseTriggers = [];
    if (!sectionMarquee) return;
    sectionMarquee.classList.remove('is-mobile-rise-anim');
    getOriginalQuoteCards().forEach((card) => {
      card.dataset.riseBound = '';
      card.classList.remove('is-rise-anim-card');
      if (typeof gsap !== 'undefined') gsap.set(card, { clearProps: 'transform' });
    });
  }

  /** Same rise pattern as homepage marquee / product cards — mobile quote list only */
  function bindMobileQuoteRise() {
    if (
      reduceMotion ||
      testimonialsDesktopMq.matches ||
      !sectionMarquee ||
      typeof gsap === 'undefined' ||
      typeof ScrollTrigger === 'undefined'
    ) {
      return;
    }

    if (typeof gsap.registerPlugin === 'function') {
      gsap.registerPlugin(ScrollTrigger);
    }

    sectionMarquee.classList.add('is-mobile-rise-anim');
    const cards = getOriginalQuoteCards().filter(
      (card) => !card.classList.contains('is-collapsed')
    );

    cards.forEach((card, i) => {
      if (card.dataset.riseBound === '1') return;
      card.dataset.riseBound = '1';
      card.classList.add('is-rise-anim-card');

      const fromY = Math.min(80 * (i + 1), 240);
      const col = i % 2;
      const start = `top ${92 - col * 4}%`;
      const end = `top ${55 - col * 3}%`;

      const tween = gsap.fromTo(
        card,
        { y: fromY, force3D: true },
        {
          y: 0,
          ease: 'none',
          force3D: true,
          scrollTrigger: {
            trigger: card,
            start,
            end,
            scrub: 1,
            invalidateOnRefresh: true,
          },
        }
      );
      if (tween.scrollTrigger) mobileQuoteRiseTriggers.push(tween.scrollTrigger);
    });

    ScrollTrigger.refresh();
  }

  function remountMobileQuoteRise() {
    if (testimonialsDesktopMq.matches || !sectionMarquee) return;
    killMobileQuoteRise();
    requestAnimationFrame(() => bindMobileQuoteRise());
  }

  function getLoopHeight(track) {
    return Math.max(1, track.scrollHeight / 2);
  }

  function createMarqueeColumn(track, direction) {
    const viewport = track.closest('.marquee-viewport');
    if (!viewport || track.dataset.marqueeBound === '1') {
      return marqueeColumns.find((c) => c.track === track) || null;
    }
    track.dataset.marqueeBound = '1';

    let offset = 0;
    let dragging = false;
    let hoverPaused = false;
    let startY = 0;
    let startOffset = 0;
    let dragDist = 0;
    let rafId = null;
    const speed = 0.42;

    function wrapOffset() {
      const half = getLoopHeight(track);
      while (offset <= -half) offset += half;
      while (offset > 0) offset -= half;
    }

    function apply() {
      track.style.transform = 'translate3d(0,' + offset + 'px,0)';
    }

    function resetOffset() {
      offset = direction === 'down' ? -getLoopHeight(track) : 0;
      apply();
    }

    function tick() {
      if (!testimonialsDesktopMq.matches) {
        rafId = null;
        return;
      }

      const inView = sectionMarquee.classList.contains('section-in-view');
      if (inView && !dragging && !hoverPaused && !reduceMotion) {
        offset += direction === 'up' ? -speed : speed;
        wrapOffset();
        apply();
      }

      rafId = requestAnimationFrame(tick);
    }

    function start() {
      if (rafId == null) rafId = requestAnimationFrame(tick);
    }

    function stop() {
      if (rafId != null) cancelAnimationFrame(rafId);
      rafId = null;
    }

    function clearMotion() {
      stop();
      track.style.transform = '';
      viewport.classList.remove('is-dragging');
      dragging = false;
      hoverPaused = false;
    }

    viewport.addEventListener('pointerenter', () => {
      if (testimonialsDesktopMq.matches) hoverPaused = true;
    });
    viewport.addEventListener('pointerleave', () => {
      if (!dragging) hoverPaused = false;
    });

    viewport.addEventListener('pointerdown', (e) => {
      if (!testimonialsDesktopMq.matches || e.button === 2) return;
      dragging = true;
      hoverPaused = true;
      dragDist = 0;
      startY = e.clientY;
      startOffset = offset;
      viewport.classList.add('is-dragging');
      try {
        viewport.setPointerCapture(e.pointerId);
      } catch (err) {}
    });

    viewport.addEventListener('pointermove', (e) => {
      if (!dragging || !testimonialsDesktopMq.matches) return;
      const dy = e.clientY - startY;
      dragDist = Math.max(dragDist, Math.abs(dy));
      offset = startOffset + dy;
      wrapOffset();
      apply();
      e.preventDefault();
    });

    function endDrag() {
      if (!dragging) return;
      dragging = false;
      viewport.classList.remove('is-dragging');
      hoverPaused = testimonialsDesktopMq.matches && viewport.matches(':hover');
    }

    viewport.addEventListener('pointerup', endDrag);
    viewport.addEventListener('pointercancel', endDrag);

    viewport.addEventListener(
      'click',
      (e) => {
        if (dragDist > 6) {
          e.preventDefault();
          e.stopPropagation();
          dragDist = 0;
        }
      },
      true
    );

    resetOffset();

    const api = { track, start, stop, clearMotion, resetOffset, refresh: resetOffset };
    marqueeColumns.push(api);
    return api;
  }

  function startDesktopMarqueeMotion() {
    if (!sectionMarquee) return;
    sectionMarquee.classList.add('is-js-marquee');
    if (reduceMotion) return;

    const up = document.getElementById('track-up');
    const down = document.getElementById('track-down');
    if (up) createMarqueeColumn(up, 'up')?.start();
    if (down) createMarqueeColumn(down, 'down')?.start();
  }

  function stopDesktopMarqueeMotion() {
    if (!sectionMarquee) return;
    sectionMarquee.classList.remove('is-js-marquee');
    marqueeColumns.forEach((col) => col.clearMotion());
  }

  function enableDesktopMarquee() {
    if (!sectionMarquee) return;

    killMobileQuoteRise();
    sectionMarquee.classList.remove('is-mobile-static');
    getOriginalQuoteCards().forEach((card) => card.classList.remove('is-collapsed'));
    if (loadMoreBtn) loadMoreBtn.hidden = true;

    makeLoopable('track-up');
    makeLoopable('track-down');
    startDesktopMarqueeMotion();

    if (!marqueeObserver) {
      marqueeObserver = new IntersectionObserver(
        (entries) => {
          entries.forEach((entry) => {
            if (entry.isIntersecting && testimonialsDesktopMq.matches) {
              sectionMarquee.classList.add('section-in-view');
            }
          });
        },
        { threshold: 0.2 }
      );
      marqueeObserver.observe(sectionMarquee);
    }

    const rect = sectionMarquee.getBoundingClientRect();
    if (rect.top < window.innerHeight * 0.8 && rect.bottom > 0) {
      sectionMarquee.classList.add('section-in-view');
    }
  }

  function enableMobileLoadMore() {
    if (!sectionMarquee) return;

    sectionMarquee.classList.remove('section-in-view');
    sectionMarquee.classList.remove('is-js-marquee');
    sectionMarquee.classList.add('is-mobile-static');
    stopDesktopMarqueeMotion();
    killMobileQuoteRise();

    marqueeColumns.forEach((col) => {
      col.clearMotion();
      if (col.track) col.track.style.transform = '';
    });

    const cards = getOriginalQuoteCards();
    mobileRevealedCount = 0;

    function applyReveal() {
      cards.forEach((card, i) => {
        card.classList.toggle('is-collapsed', i >= mobileRevealedCount);
      });
      if (loadMoreBtn) {
        loadMoreBtn.hidden = mobileRevealedCount >= cards.length;
      }
      whenPinLayoutReady(remountMobileQuoteRise);
    }

    mobileRevealedCount = Math.min(MOBILE_BATCH_SIZE, cards.length);
    applyReveal();
  }

  function revealMoreTestimonials() {
    if (testimonialsDesktopMq.matches || !sectionMarquee) return;

    const cards = getOriginalQuoteCards();
    mobileRevealedCount = Math.min(
      mobileRevealedCount + MOBILE_BATCH_SIZE,
      cards.length
    );

    cards.forEach((card, i) => {
      card.classList.toggle('is-collapsed', i >= mobileRevealedCount);
    });

    if (loadMoreBtn) {
      loadMoreBtn.hidden = mobileRevealedCount >= cards.length;
    }

    whenPinLayoutReady(() => {
      requestAnimationFrame(() => bindMobileQuoteRise());
    });
  }

  function syncTestimonialsLayout() {
    if (testimonialsDesktopMq.matches) {
      enableDesktopMarquee();
    } else {
      enableMobileLoadMore();
    }
  }

  if (sectionMarquee) {
    syncTestimonialsLayout();
    if (typeof testimonialsDesktopMq.addEventListener === 'function') {
      testimonialsDesktopMq.addEventListener('change', syncTestimonialsLayout);
    } else if (typeof testimonialsDesktopMq.addListener === 'function') {
      testimonialsDesktopMq.addListener(syncTestimonialsLayout);
    }
  }

  if (loadMoreBtn) {
    loadMoreBtn.addEventListener('click', revealMoreTestimonials);
  }

/* ---------------------------------------
   WORKPLACE CULTURE — mobile scroll-snap nav
   (dots + prev/next; inactive on desktop collage)
--------------------------------------- */
  (function initCultureSlider() {
    const section = document.querySelector('.workplaceculture-section');
    const track = section && section.querySelector('[data-culture-photos]');
    const nav = section && section.querySelector('[data-culture-nav]');
    const dotsWrap = section && section.querySelector('[data-culture-dots]');
    const prevBtn = section && section.querySelector('[data-culture-prev]');
    const nextBtn = section && section.querySelector('[data-culture-next]');
    if (!section || !track || !nav || !dotsWrap || !prevBtn || !nextBtn) return;

    const mobileMq = window.matchMedia('(max-width: 1023px)');
    const slides = () => Array.from(track.querySelectorAll('.culture-photo'));
    let activeIndex = 0;
    let scrolling = false;

    function buildDots() {
      dotsWrap.innerHTML = '';
      slides().forEach((_, i) => {
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.className = 'culture-slider-dot' + (i === 0 ? ' is-active' : '');
        btn.setAttribute('aria-label', 'Go to photo ' + (i + 1));
        btn.setAttribute('role', 'tab');
        btn.addEventListener('click', () => goTo(i));
        dotsWrap.appendChild(btn);
      });
    }

    function getIndexFromScroll() {
      const list = slides();
      if (!list.length) return 0;
      const left = track.scrollLeft;
      let best = 0;
      let bestDist = Infinity;
      list.forEach((slide, i) => {
        const dist = Math.abs(slide.offsetLeft - left);
        if (dist < bestDist) {
          bestDist = dist;
          best = i;
        }
      });
      return best;
    }

    function syncUI(index) {
      activeIndex = index;
      const list = slides();
      const dots = dotsWrap.querySelectorAll('.culture-slider-dot');
      dots.forEach((dot, i) => {
        dot.classList.toggle('is-active', i === index);
        dot.setAttribute('aria-selected', i === index ? 'true' : 'false');
      });
      prevBtn.disabled = index <= 0;
      nextBtn.disabled = index >= list.length - 1;
    }

    function goTo(index) {
      if (!mobileMq.matches) return;
      const list = slides();
      if (!list.length) return;
      const clamped = Math.max(0, Math.min(index, list.length - 1));
      const target = list[clamped];
      scrolling = true;
      track.scrollTo({
        left: target.offsetLeft,
        behavior: reduceMotion ? 'auto' : 'smooth',
      });
      syncUI(clamped);
      window.setTimeout(() => {
        scrolling = false;
      }, reduceMotion ? 0 : 420);
    }

    function onScroll() {
      if (!mobileMq.matches || scrolling) return;
      syncUI(getIndexFromScroll());
    }

    function onBreakpoint() {
      if (mobileMq.matches) {
        nav.hidden = false;
        buildDots();
        syncUI(getIndexFromScroll());
      } else {
        nav.hidden = true;
        track.scrollLeft = 0;
        activeIndex = 0;
      }
    }

    prevBtn.addEventListener('click', () => goTo(activeIndex - 1));
    nextBtn.addEventListener('click', () => goTo(activeIndex + 1));
    track.addEventListener('scroll', onScroll, { passive: true });

    buildDots();
    onBreakpoint();
    if (typeof mobileMq.addEventListener === 'function') {
      mobileMq.addEventListener('change', onBreakpoint);
    } else if (typeof mobileMq.addListener === 'function') {
      mobileMq.addListener(onBreakpoint);
    }
  })();

});
