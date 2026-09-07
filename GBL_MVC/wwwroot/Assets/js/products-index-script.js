document.addEventListener("DOMContentLoaded", () => {
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

/* ---------------------------------------
   PARALLAX IMAGE
--------------------------------------- */
  if (!reduceMotion && typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    gsap.utils.toArray('.parallax-wrap').forEach((wrap) => {
      // Enlarge section owns motion on desktop — skip y-parallax there
      if (wrap.classList.contains('enlarge-wrapper')) return;

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
            scrub: 0.8, // smoother with Lenis than scrub:true
            invalidateOnRefresh: true,
          },
        }
      );
    });
  }

/* ---------------------------------------
   PRODUCT CARDS — rise on scroll (no pin / no slider)
   Same feel as homepage product slides: low → settle
--------------------------------------- */
  if (typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    const cardsGrid = document.querySelector('.our-products-landing-cards');
    const cards = cardsGrid
      ? gsap.utils.toArray(cardsGrid.querySelectorAll('.our-products-cards'))
      : [];

    if (cardsGrid && cards.length) {
      if (reduceMotion) {
        gsap.set(cards, { clearProps: 'transform' });
      } else {
        cardsGrid.classList.add('is-rise-anim');

        cards.forEach((card, i) => {
          // Progressive rise: 60 → 120 → 180 → … (each card starts lower)
          const fromY = 60 * (i + 1);
          // Slight column delay so a row doesn't lockstep
          const col = i % 3;
          const start = `top ${90 - col * 4}%`;
          const end = `top ${58 - col * 3}%`;

          gsap.fromTo(
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
        });
      }
    }
  }

/* ---------------------------------------
   MAN CUTOUT — rise + fade on viewport enter
--------------------------------------- */
  if (typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    const manWrap = document.querySelector('.bg-parallax-section .man-image-wrap');
    const section = document.querySelector('.bg-parallax-section');

    if (manWrap && section) {
      if (reduceMotion) {
        gsap.set(manWrap, { clearProps: 'transform,opacity,visibility' });
      } else {
        gsap.set(manWrap, {
          y: 100,
          autoAlpha: 0.95,
          force3D: true,
        });

        gsap.to(manWrap, {
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
      }
    }
  }

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
});
