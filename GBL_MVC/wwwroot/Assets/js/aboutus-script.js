document.addEventListener("DOMContentLoaded", () => {
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

/* ---------------------------------------
   BELOW VALUES — CTA + Built on Trust
   Wait until Values pin exists (same pattern as Careers + Life Inside).
--------------------------------------- */
  function initBelowValuesScrollFx() {
    if (reduceMotion || typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') return;

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

      // Move the image inside the frame (not the wrap) so cover always fills —
      // avoids black/white gaps at top/bottom while scrolling.
      gsap.fromTo(
        ctaImg,
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

      if (!ctaImg.complete) {
        ctaImg.addEventListener('load', () => ScrollTrigger.refresh(), { once: true });
      }
    }

    const projectsScope = document.querySelector('.aboutus-projects-section [data-rail-scope]');

    document.querySelectorAll('.aboutus-projects-section [data-rail]').forEach((rail) => {
      const thumb = rail.querySelector('[data-rail-thumb]');
      if (!thumb) return;
      gsap.fromTo(
        thumb,
        { y: 0 },
        {
          y: () => Math.max(rail.offsetHeight - 40, 0),
          ease: 'none',
          scrollTrigger: {
            trigger: rail.closest('[data-rail-scope]'),
            start: 'top 60%',
            end: 'bottom 70%',
            scrub: 0.4,
            invalidateOnRefresh: true,
          },
        }
      );
    });

    /* Use set + to (not from): Values pin / ScrollTrigger.refresh re-applies
       from() start values and causes flicker. Avoid autoAlpha on .text-ghost
       (background-clip text + visibility toggle flickers in Chromium). */
    gsap.utils.toArray('.aboutus-projects-section [data-fade]').forEach((el) => {
      gsap.set(el, { y: 42, autoAlpha: 0, force3D: true });
      gsap.to(el, {
        y: 0,
        autoAlpha: 1,
        duration: 0.8,
        ease: 'power3.out',
        force3D: true,
        scrollTrigger: {
          trigger: el,
          start: 'top 88%',
          toggleActions: 'play none none reverse',
        },
      });
    });

    gsap.utils.toArray('.aboutus-projects-section [data-count]').forEach((el) => {
      gsap.set(el, { x: -40, opacity: 0, force3D: true });
      gsap.to(el, {
        x: 0,
        opacity: 1,
        duration: 0.9,
        ease: 'power3.out',
        force3D: true,
        scrollTrigger: {
          trigger: el,
          start: 'top 85%',
          toggleActions: 'play none none reverse',
        },
      });
    });

    gsap.utils.toArray('.aboutus-projects-section .section-sticky-title').forEach((title) => {
      const scope = title.closest('[data-rail-scope]') || projectsScope;
      if (!scope) return;
      gsap.set(title, { autoAlpha: 0, y: 24, force3D: true });
      const titleTl = gsap.timeline({
        scrollTrigger: {
          trigger: scope,
          start: 'top 80%',
          end: 'bottom 25%',
          scrub: 0.6,
        },
      });
      /* One timeline — two competing scrub tweens on the same node flicker */
      titleTl.to(title, { autoAlpha: 1, y: 0, ease: 'power2.out', duration: 1 }, 0);
      titleTl.to(title, { autoAlpha: 0, y: -24, ease: 'power2.in', duration: 1 }, 0.72);
    });
  }

  (function scheduleBelowValuesScrollFx() {
    let started = false;
    function start() {
      if (started) return;
      started = true;
      initBelowValuesScrollFx();
    }

    if (!document.getElementById('aboutus-values') || window.__aboutusValuesReady) {
      start();
    } else {
      window.addEventListener('aboutus-values:ready', start, { once: true });
      window.setTimeout(start, 4000);
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
   LEADERSHIP — quote stamp + photo rise + copy stagger
   Use set + to (not from + reverse): stats/Values pin refresh
   re-applies from() start values and causes flicker / invisible play.
--------------------------------------- */
  if (!reduceMotion && typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    const leadSection = document.querySelector('.aboutus-leadership-section');

    if (leadSection) {
      const inner = leadSection.querySelector('.aboutus-leadership-inner') || leadSection;
      const quoteWrap = leadSection.querySelector('.aboutus-leadership-quote');
      const quoteImgs = leadSection.querySelectorAll('.aboutus-leadership-quote img');
      const photo = leadSection.querySelector('.aboutus-leadership-photo');
      const photoImg = photo && photo.querySelector('img');
      const copy = leadSection.querySelector('.aboutus-leadership-copy');
      const copyParas = leadSection.querySelectorAll(
        '.aboutus-leadership-copy p:not(.aboutus-leadership-name):not(.aboutus-leadership-role)'
      );
      const signEls = [
        leadSection.querySelector('.aboutus-leadership-name'),
        leadSection.querySelector('.aboutus-leadership-role'),
      ].filter(Boolean);

      const riseEls = [quoteWrap, photo].filter(Boolean);
      const trigger = inner;

      if (riseEls.length) {
        gsap.set(riseEls, { y: 60, autoAlpha: 0, force3D: true });
      }
      if (quoteImgs.length) {
        gsap.set(quoteImgs, { scale: 0.65, transformOrigin: '50% 50%', force3D: true });
      }
      if (copyParas.length) {
        gsap.set(copyParas, { y: 25, autoAlpha: 0 });
      }
      if (signEls.length) {
        gsap.set(signEls, { y: 60, autoAlpha: 0 });
      }

      const tl = gsap.timeline({
        defaults: { ease: 'power3.out', force3D: true },
        scrollTrigger: {
          trigger,
          start: 'top 78%',
          toggleActions: 'play none none reverse',
          invalidateOnRefresh: true,
        },
      });

      if (riseEls.length) {
        tl.to(riseEls, { y: 0, autoAlpha: 1, duration: 0.9 }, 0);
      }
      if (quoteImgs.length) {
        tl.to(quoteImgs, { scale: 1, duration: 0.9, stagger: 0.1 }, 0.05);
      }
      if (copyParas.length) {
        tl.to(copyParas, { y: 0, autoAlpha: 1, duration: 0.75, stagger: 0.12 }, 0.2);
      }
      if (signEls.length) {
        tl.to(signEls, { y: 0, autoAlpha: 1, duration: 0.65, stagger: 0.08 }, 0.55);
      }

      if (photoImg && !photoImg.complete) {
        photoImg.addEventListener('load', () => ScrollTrigger.refresh(), { once: true });
      }
    }
  }

/* ---------------------------------------
   STATS STACK — pin + scale deck (CodePen RNbxoQR)
--------------------------------------- */
  if (typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    const statsSection = document.querySelector('[data-stats-scope]');
    const statsStage = statsSection && statsSection.querySelector('[data-stats-stage]');
    const statsCards = statsStage ? gsap.utils.toArray(statsStage.querySelectorAll('[data-stats-card]')) : [];

    if (statsSection && statsStage && statsCards.length) {
      if (reduceMotion) {
        statsSection.classList.add('is-stats-reduced');
      } else {
        const scaleFactor = 0.9;
        const headerOffset = () =>
          parseFloat(getComputedStyle(document.documentElement).getPropertyValue('--header-h')) || 106;

        statsCards.forEach((card, index) => {
          const finalScale = Math.pow(scaleFactor, statsCards.length - index - 1);
          const next = statsCards[index + 1];
          const bgWrap = card.querySelector('.aboutus-stats-bg-wrap');
          const bg = card.querySelector('.aboutus-stats-bg');

          gsap.set(card, {
            zIndex: index + 1,
            transformOrigin: 'top center',
            force3D: true,
          });

          gsap.to(card, {
            scale: finalScale,
            ease: 'none',
            scrollTrigger: {
              trigger: card,
              start: () => `top top+=${headerOffset()}`,
              endTrigger: statsStage,
              end: 'bottom bottom',
              scrub: true,
              pin: true,
              pinSpacing: false,
              anticipatePin: 1,
              invalidateOnRefresh: true,
            },
          });

          if (bgWrap) {
            gsap.from(bgWrap, {
              y: 50,
              autoAlpha: 0,
              duration: 0.7,
              ease: 'power2.out',
              force3D: true,
              scrollTrigger: {
                trigger: card,
                start: 'top 60%',
                toggleActions: 'play none none reverse',
                invalidateOnRefresh: true,
              },
            });
          }

          if (bg) {
            gsap.fromTo(
              bg,
              { y: 24 },
              {
                y: -24,
                ease: 'none',
                force3D: true,
                scrollTrigger: {
                  trigger: card,
                  start: () => `top top+=${headerOffset()}`,
                  endTrigger: next || statsStage,
                  end: next ? () => `top top+=${headerOffset()}` : 'bottom bottom',
                  scrub: true,
                  invalidateOnRefresh: true,
                },
              }
            );
          }
        });
      }
    }
  }

/* ---------------------------------------
   WHAT WE STAND FOR — lion + card stagger
--------------------------------------- */
  if (!reduceMotion && typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    const standSection = document.querySelector('.aboutus-stand-section');

    if (standSection) {
      const visual = standSection.querySelector('.aboutus-stand-visual');
      const visualImg = visual && visual.querySelector('.aboutus-stand-visual-img');
      const cards = standSection.querySelectorAll('.aboutus-stand-card');

      if (visualImg) {
        gsap
          .timeline({
            defaults: { force3D: true },
            scrollTrigger: {
              trigger: visual,
              start: 'top 85%',
              toggleActions: 'play none none reverse',
              invalidateOnRefresh: true,
            },
          })
          .from(visualImg, { xPercent: -100, duration: 1.1, ease: 'power3.out' }, 0)
          .from(visualImg, { autoAlpha: 0, duration: 0.95, ease: 'power2.out' }, 0);
      }

      cards.forEach((card) => {
        gsap.from(card, {
          y: 42,
          autoAlpha: 0,
          duration: 0.8,
          ease: 'power3.out',
          scrollTrigger: {
            trigger: card,
            start: 'top 88%',
            toggleActions: 'play none none reverse',
          },
        });

        const num = card.querySelector('.aboutus-stand-num');
        if (!num) return;

        gsap.from(num, {
          x: -40,
          autoAlpha: 0,
          duration: 0.9,
          ease: 'power3.out',
          scrollTrigger: {
            trigger: num,
            start: 'top 85%',
            toggleActions: 'play none none reverse',
          },
        });
      });
    }
  }

});
