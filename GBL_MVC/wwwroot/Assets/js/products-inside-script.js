/* ---------------------------------------
   PRODUCTS INSIDE
   1) Product bag — GSAP pin until CTA end
   2) CTA lion — scale + stroke draw → fill
   3) Table scroll fade (mobile)
--------------------------------------- */
(() => {
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  /* ---- Mobile table scroll fade (no GSAP needed) ---- */
  const initTableScrollFade = () => {
    const wraps = document.querySelectorAll('[data-table-scroll]');
    if (!wraps.length) return;

    const update = (wrap) => {
      const scroller = wrap.querySelector('.table-scroll-x');
      if (!scroller) return;
      const max = scroller.scrollWidth - scroller.clientWidth;
      const scrollable = max > 4;
      wrap.classList.toggle('is-scrollable', scrollable);
      wrap.classList.toggle('is-at-end', !scrollable || scroller.scrollLeft >= max - 4);
    };

    wraps.forEach((wrap) => {
      const scroller = wrap.querySelector('.table-scroll-x');
      if (!scroller) return;
      scroller.addEventListener('scroll', () => update(wrap), { passive: true });
      update(wrap);
    });

    window.addEventListener('resize', () => wraps.forEach(update), { passive: true });
  };

  initTableScrollFade();

  /* ---- Download FAB: tap toggle on touch / click outside ---- */
  const fab = document.querySelector('[data-download-fab]');
  if (fab) {
    const trigger = fab.querySelector('.product-float-button');
    const setOpen = (open) => {
      fab.classList.toggle('is-open', open);
      if (trigger) trigger.setAttribute('aria-expanded', open ? 'true' : 'false');
    };

    if (trigger) {
      trigger.addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        setOpen(!fab.classList.contains('is-open'));
      });
    }

    document.addEventListener('click', (e) => {
      if (!fab.contains(e.target)) setOpen(false);
    });

    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape') setOpen(false);
    });
  }

  if (typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') return;

  /* ---- Product bag: fixed until CTA end, then release (no opacity 0 at end) ---- */
  const mm = gsap.matchMedia();

  mm.add('(min-width: 1024px)', () => {
    const bag = document.querySelector('.product-float-sticky');
    const rail = document.querySelector('.product-float-rail');
    const cta = document.querySelector('.cta-band');
    const banner = document.querySelector('.inside-banner-outer');
    const intro = document.querySelector('.page-intro-outer');
    if (!bag || !rail || !cta) return;

    gsap.set(bag, { autoAlpha: 1 });

    const show = () => {
      gsap.to(bag, {
        autoAlpha: 1,
        duration: reduceMotion ? 0 : 0.35,
        ease: 'power2.out',
        overwrite: 'auto',
      });
    };

    const hide = () => {
      gsap.to(bag, {
        autoAlpha: 1,
        duration: reduceMotion ? 0 : 0.25,
        ease: 'power1.in',
        overwrite: 'auto',
      });
    };

    const armFixed = () => {
      if (bag.parentElement !== rail) rail.appendChild(bag);
      bag.classList.remove('is-released');
      // Clear inline absolute coords from release so CSS fixed applies
      ['position', 'top', 'right', 'left', 'bottom', 'transform'].forEach((p) =>
        bag.style.removeProperty(p)
      );
      gsap.set(bag, { autoAlpha: 1 });
    };

    const releaseToCta = () => {
      const bagRect = bag.getBoundingClientRect();
      const ctaRect = cta.getBoundingClientRect();
      const top = bagRect.top - ctaRect.top;
      const right = ctaRect.right - bagRect.right;

      cta.appendChild(bag);
      bag.classList.add('is-released');
      gsap.set(bag, {
        position: 'absolute',
        top,
        right,
        left: 'auto',
        bottom: 'auto',
        x: 0,
        y: 0,
        autoAlpha: 1,
      });
    };

    // Show as soon as page intro reaches the first screen (not after banner
    // fully exits — that kept the bag invisible while content was already peeking).
    const rangeST = ScrollTrigger.create({
      trigger: intro || rail || banner,
      start: 'top bottom',
      endTrigger: cta,
      end: 'bottom bottom',
      invalidateOnRefresh: true,
      onEnter: () => {
        armFixed();
        show();
      },
      onEnterBack: () => {
        armFixed();
        show();
      },
      onLeave: () => {
        releaseToCta();
      },
      onLeaveBack: () => {
        if (bag.parentElement !== rail) rail.appendChild(bag);
        bag.classList.remove('is-released');
        ['position', 'top', 'right', 'left', 'bottom', 'transform'].forEach((p) =>
          bag.style.removeProperty(p)
        );
        hide();
      },
    });

    const syncBagVisibility = () => {
      if (rangeST.progress >= 1) {
        releaseToCta();
      } else if (rangeST.isActive || rangeST.progress > 0) {
        armFixed();
        show();
      }
    };

    requestAnimationFrame(() => {
      ScrollTrigger.refresh();
      syncBagVisibility();
    });

    return () => {
      rangeST.kill();
      if (bag.parentElement !== rail) rail.appendChild(bag);
      bag.classList.remove('is-released');
      gsap.set(bag, { clearProps: 'all' });
    };
  });

  /* ---- CTA lion ---- */
  if (!reduceMotion) {
    const lionWrap =
      document.querySelector('.cta-band .lion-logo-wrap') ||
      document.querySelector('.lion-logo-wrap');
    const lionSvg =
      lionWrap && (lionWrap.querySelector('.lion-logo-svg') || lionWrap.querySelector('svg'));
    const lionFill = lionSvg && lionSvg.querySelector('.lion-logo-fill');

    if (lionWrap && lionSvg && lionFill) {
      let lionStroke = lionSvg.querySelector('.lion-logo-stroke');
      if (!lionStroke) {
        lionStroke = lionFill.cloneNode();
        lionStroke.removeAttribute('fill');
        lionStroke.removeAttribute('fill-opacity');
        lionStroke.classList.remove('lion-logo-fill');
        lionStroke.classList.add('lion-logo-stroke');
        lionStroke.setAttribute('fill', 'none');
        lionStroke.setAttribute('stroke', 'rgba(255,255,255,0.55)');
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
          xPercent: -50,
          yPercent: -50,
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

        const trigger = lionWrap.closest('.cta-band') || lionWrap;

        gsap
          .timeline({
            scrollTrigger: {
              trigger,
              start: 'top 75%',
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
          .to(lionFill, { autoAlpha: 1, duration: 0.55, ease: 'power2.out' }, '-=0.3')
          .to(lionStroke, { autoAlpha: 0, duration: 0.4, ease: 'power1.out' }, '-=0.35');
      }
    }
  }
})();
