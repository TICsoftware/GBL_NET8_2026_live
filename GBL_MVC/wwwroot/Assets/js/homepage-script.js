document.addEventListener("DOMContentLoaded", () => {

/* ---------------------------------------
   HERO BANNER SLIDER
--------------------------------------- */
  const heroEl = document.querySelector('.hero-swiper');
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  if (heroEl && typeof Swiper !== 'undefined') {
    const PARALLAX_MAX = 10;
    let heroSwiperReady = false;
    let heroSwiper = null;
    let heroAutoplayPausedByScroll = false;

    function applyImageParallax(swiper, duration) {
      swiper.slides.forEach((slide) => {
        const media = slide.querySelector('.slide-media');
        if (!media) return;
        const progress = Math.max(-1, Math.min(1, slide.progress || 0));
        const shift = reduceMotion ? 0 : -progress * PARALLAX_MAX;
        media.style.transitionDuration = (duration != null ? duration : swiper.params.speed) + 'ms';
        media.style.transform = 'translate3d(' + shift + '%, 0, 0)';
      });
    }

    // Tag title / paragraph / CTA per slide with data-swiper-parallax at
    // runtime — same attribute the codepen reference sets in markup, just
    // applied via JS so Index.cshtml stays untouched. Depths (300/400/500)
    // match the reference 1:1: title moves least, button travels furthest.
    function applyParallaxAttrs() {
      if (reduceMotion) return;
      heroEl.querySelectorAll('.swiper-slide').forEach((slide) => {
        const title = slide.querySelector('.slide-content h1, .slide-content h2');
        const text = slide.querySelector('.slide-content .hero-para');
        const btnWrap = slide.querySelector('.outer-hero-button');
        if (title) title.setAttribute('data-swiper-parallax', '300');
        if (text) text.setAttribute('data-swiper-parallax', '400');
        if (btnWrap) btnWrap.setAttribute('data-swiper-parallax', '500');
      });
    }

    function syncHeroAutoplayToScroll(progress) {
      if (!heroSwiper || !heroSwiper.autoplay) return;
      // Pause once the scale scrub is ≥80% through (scale ≈ 0.84 → 0.8)
      const shouldPause = progress >= 0.8;
      if (shouldPause === heroAutoplayPausedByScroll) return;
      heroAutoplayPausedByScroll = shouldPause;
      if (shouldPause) heroSwiper.autoplay.pause();
      else heroSwiper.autoplay.resume();
    }

    function initHeroScrollScale() {
      if (
        reduceMotion ||
        typeof gsap === 'undefined' ||
        typeof ScrollTrigger === 'undefined'
      ) {
        return;
      }

      gsap.fromTo(
        heroEl,
        { scale: 1, transformOrigin: 'center center' },
        {
          scale: 0.8,
          ease: 'none',
          scrollTrigger: {
            trigger: heroEl,
            start: 'top top',
            end: 'bottom top',
            scrub: true,
            onUpdate(self) {
              syncHeroAutoplayToScroll(self.progress);
            },
            onRefresh(self) {
              syncHeroAutoplayToScroll(self.progress);
            },
          },
        }
      );
    }

    function initHeroSwiper() {
      if (heroSwiperReady) return;
      heroSwiperReady = true;

      applyParallaxAttrs();

      heroSwiper = new Swiper(heroEl, {
        loop: true,
        speed: reduceMotion ? 0 : 1050,
        effect: 'slide',
        parallax: !reduceMotion,
        watchSlidesProgress: true,
        grabCursor: true,
        resistanceRatio: 0.85,
        followFinger: true,
        autoplay: {
          delay: 5600,
          disableOnInteraction: false,
          pauseOnMouseEnter: true,
        },
        keyboard: { enabled: true },
        pagination: {
          el: '.hero-pagination',
          clickable: true,
          renderBullet: function (index, className) {
            return '<span class="' + className + '"></span>';
          },
        },
        navigation: {
          nextEl: '.hero-swiper .swiper-button-next',
          prevEl: '.hero-swiper .swiper-button-prev',
        },
        on: {
          init(sw) {
            applyImageParallax(sw, 0);

            const arrowSrc = 'Assets/images/common-images/button-arrow-icon.svg';
            [sw.navigation?.prevEl, sw.navigation?.nextEl].forEach((btn) => {
              if (!btn) return;
              btn.querySelectorAll('.swiper-navigation-icon, svg').forEach((el) => el.remove());
              if (!btn.querySelector('img')) {
                const img = document.createElement('img');
                img.src = arrowSrc;
                img.alt = '';
                img.className = 'w-auto object-cover';
                img.setAttribute('aria-hidden', 'true');
                btn.appendChild(img);
              }
            });
          },
          progress(sw) {
            applyImageParallax(sw, 0);
          },
          setTransition(sw, duration) {
            applyImageParallax(sw, duration);
          },
        },
      });

      initHeroScrollScale();
    }

    // Built immediately, hidden behind the preloader's opaque backdrop —
    // so the slider is already fully laid out and running (autoplay armed,
    // first slide positioned) by the time the preloader exits, instead of
    // still being under construction the instant it becomes visible.
    // homepage-preloader.js handles *revealing* the hero section on exit;
    // this only builds it.
    initHeroSwiper();
  }



/* ---------------------------------------
   PRODUCT SLIDER
   Desktop: pin + scroll slide
   Mobile: normal swipe + arrow navigation
--------------------------------------- */
  const productSwiperEl = document.querySelector('.product-swiper');
  const sectionProducts = document.getElementById('products-section');

  if (productSwiperEl && sectionProducts && typeof Swiper !== 'undefined') {
    const track = sectionProducts.querySelector('.products-pin-track');
    const sticky = sectionProducts.querySelector('.products-pin-sticky');
    const pinMq = window.matchMedia('(min-width: 1025px)');

    function isPinMode() {
      return pinMq.matches && !reduceMotion;
    }

    const CARD_RISE = 80; // px of Y per slide-width toward active
    const MAX_OFFSET = CARD_RISE * 2.5; // fully lowered when appearing at the right

    // One continuous rise: appear (right, low) → travel across view → active (Y = 0)
    // Uses on-screen position so the same motion starts the moment a card enters.
    function updateCardOffsets(swiper) {
      if (!isPinMode()) return;

      const wrapRect = swiper.el.getBoundingClientRect();
      const space = Number(swiper.params.spaceBetween) || 0;
      const maxT = swiper.maxTranslate();
      const progress = Math.min(1, Math.max(0, swiper.progress || 0));

      swiper.slides.forEach((slide, index) => {
        const rect = slide.getBoundingClientRect();
        const unit = Math.max(1, (rect.width || slide.offsetWidth || 1) + space);
        // 0 at active dock (viewport left); >0 while still to the right
        const rawP = (rect.left - wrapRect.left) / unit;
        // With >1 slide per view, Swiper clamps translate before the trailing
        // cards' left edge ever reaches the dock (it won't overscroll past the
        // last slide) — so blend out that residual as the pin sequence ends,
        // letting the last cards settle to Y = 0 like the earlier ones do.
        const pEnd = Math.max(0, index + maxT / unit);
        const p = rawP - pEnd * progress;
        const offset = Math.min(MAX_OFFSET, Math.max(0, p) * CARD_RISE);
        slide.style.setProperty('--offset', `${offset}px`);
      });
    }

    const productSwiper = new Swiper(productSwiperEl, {
      slidesPerView: 1.10,
      spaceBetween: 20,
      breakpoints: {
        640:  { slidesPerView: 1.15, spaceBetween: 20 },
        768:  { slidesPerView: 2.2, spaceBetween: 24 },
        1024: { slidesPerView: 2.95, spaceBetween: 30 },
      },
      speed: isPinMode() ? 0 : 500,
      resistanceRatio: 0.85,
      watchSlidesProgress: true,
      allowTouchMove: true,
      navigation: {
        nextEl: sectionProducts.querySelector('.product-swiper-next'),
        prevEl: sectionProducts.querySelector('.product-swiper-prev'),
      },
      on: {
        init() {
          if (isPinMode()) updateCardOffsets(this);
        },
        setTranslate() {
          if (isPinMode()) updateCardOffsets(this);
        },
        slideChangeTransitionEnd() {
          if (isPinMode()) updateCardOffsets(this);
        },
      },
    });

    let scrollDistance = 0;
    let ticking = false;

    function getHeaderH() {
      const raw = getComputedStyle(document.documentElement).getPropertyValue('--header-h').trim();
      const n = parseFloat(raw);
      return Number.isFinite(n) ? n : 0;
    }

    function getScrollDistance() {
      // One scroll "beat" per slide so the last cards get the same rise time
      const steps = Math.max(1, productSwiper.slides.length - 1);
      return Math.round(window.innerHeight * 0.7 * steps);
    }

    function resetCardMotion() {
      productSwiper.slides.forEach((slide) => {
        slide.style.removeProperty('--offset');
        const inner = slide.querySelector('.product-slide-inner');
        if (!inner) return;
        inner.style.transform = '';
        inner.style.opacity = '';
      });
      sectionProducts.classList.remove('is-products-pin');
    }

    function clearPinStyles() {
      if (track) track.style.height = '';
      productSwiper.setTransition(0);
      productSwiper.slideTo(productSwiper.activeIndex, 0, false);
      productSwiper.params.speed = 500;
      resetCardMotion();
      productSwiper.update();
    }

    function cacheLayout() {
      if (!track || !sticky) return;
      productSwiper.update();

      if (!isPinMode()) {
        clearPinStyles();
        return;
      }

      sectionProducts.classList.add('is-products-pin');
      scrollDistance = getScrollDistance();
      track.style.height = (sticky.offsetHeight + scrollDistance) + 'px';
      productSwiper.params.speed = 0;
    }

    function syncProductFromScroll() {
      if (!isPinMode() || !track || !sticky || scrollDistance <= 0) return;

      const headerH = getHeaderH();
      const trackTop = track.getBoundingClientRect().top;
      const scrolled = Math.min(Math.max(headerH - trackTop, 0), scrollDistance);
      const progress = scrolled / scrollDistance;

      const maxT = productSwiper.maxTranslate();
      const minT = productSwiper.minTranslate();
      const target = minT + (maxT - minT) * progress;

      productSwiper.setTransition(0);
      productSwiper.setTranslate(target);
      productSwiper.updateProgress(target);
      productSwiper.updateActiveIndex();
      productSwiper.updateSlidesClasses();
      updateCardOffsets(productSwiper);
    }

    function onScroll() {
      if (!isPinMode()) return;
      if (ticking) return;
      ticking = true;
      requestAnimationFrame(() => {
        syncProductFromScroll();
        ticking = false;
      });
    }

    function onResize() {
      cacheLayout();
      if (isPinMode()) syncProductFromScroll();
    }

    function onPinModeChange() {
      cacheLayout();
      if (isPinMode()) syncProductFromScroll();
    }

    requestAnimationFrame(() => {
      cacheLayout();
      if (isPinMode()) syncProductFromScroll();
    });

    sectionProducts.querySelectorAll('.product-slide-img img').forEach((img) => {
      if (!img.complete) img.addEventListener('load', onResize, { once: true });
    });

    window.addEventListener('scroll', onScroll, { passive: true });
    // Lenis desktop smooth scroll — keep card Y in sync every frame
    if (window.lenis && typeof window.lenis.on === 'function') {
      window.lenis.on('scroll', onScroll);
    }
    window.addEventListener('resize', onResize, { passive: true });
    if (typeof pinMq.addEventListener === 'function') {
      pinMq.addEventListener('change', onPinModeChange);
    } else if (typeof pinMq.addListener === 'function') {
      pinMq.addListener(onPinModeChange);
    }
    productSwiper.on('resize', onResize);
    productSwiper.on('breakpoint', onResize);
  }


/* ---------------------------------------
   PRODUCTS LION — scale up + stroke draw → fill (no travel / pin)
--------------------------------------- */
  if (
    !reduceMotion &&
    sectionProducts &&
    typeof gsap !== 'undefined' &&
    typeof ScrollTrigger !== 'undefined'
  ) {
    const lionWrap = sectionProducts.querySelector('.lion-logo-wrap');
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

        gsap
          .timeline({
            scrollTrigger: {
              trigger: sectionProducts,
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
   CERTIFICATE BOX — smooth viewport reveal
--------------------------------------- */
  const certOuter = document.querySelector('.certificates-box-outer');
  const certBoxes = certOuter
    ? gsap.utils.toArray(certOuter.querySelectorAll('.certificates-box'))
    : [];

  if (certBoxes.length && typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    if (reduceMotion) {
      gsap.set(certBoxes, { clearProps: 'all' });
    } else {
      gsap.set(certBoxes, {
        autoAlpha: 0,
        y: 22,
        scale: 0.97,
        force3D: true,
      });

      gsap.to(certBoxes, {
        autoAlpha: 1,
        y: 0,
        scale: 1,
        duration: 0.9,
        stagger: { each: 0.07, from: 'start', ease: 'power1.out' },
        ease: 'power3.out',
        force3D: true,
        overwrite: 'auto',
        scrollTrigger: {
          trigger: certOuter,
          start: 'top 82%',
          toggleActions: "play none none reverse",
          invalidateOnRefresh: true,
        },
      });
    }
  }


/* ---------------------------------------
   ENLARGE WRAPPER (shared)
   Works for:
   - left content + right image  (.leftContent-wrap)
   - left image + right content  (.rightContent-wrap)
   1080px + container-center → original place
   Content fades in after ~80% of image travel
--------------------------------------- */
  if (!reduceMotion && typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    const mmEnlarge = gsap.matchMedia();

    mmEnlarge.add('(min-width: 1024px)', () => {
      const TARGET_WIDTH = 1080;
      const cleanups = [];

      gsap.utils.toArray('.enlarge-outer-section').forEach((sectionEnlarge, index) => {
        const wrapperEnlarge = sectionEnlarge.querySelector('.enlarge-wrapper');
        const sideContent =
          sectionEnlarge.querySelector('.leftContent-wrap') ||
          sectionEnlarge.querySelector('.rightContent-wrap');
        const enlargeContainer = sectionEnlarge.querySelector('.container');

        if (!wrapperEnlarge || !sideContent || !enlargeContainer) return;

        sectionEnlarge.classList.add('is-enlarge-active');

        const getFromState = () => {
          const box = enlargeContainer.getBoundingClientRect();
          const rect = wrapperEnlarge.getBoundingClientRect();
          const curX = Number(gsap.getProperty(wrapperEnlarge, 'x')) || 0;
          const curY = Number(gsap.getProperty(wrapperEnlarge, 'y')) || 0;
          const curScale = Math.max(Number(gsap.getProperty(wrapperEnlarge, 'scale')) || 1, 0.001);

          const naturalW = Math.max(rect.width / curScale, 1);
          const layoutCenterX = rect.left + rect.width / 2 - curX;
          const layoutCenterY = rect.top + rect.height / 2 - curY;

          const maxW = Math.min(TARGET_WIDTH, box.width);
          const scale = maxW / naturalW;

          const centerX = box.left + box.width / 2;
          const centerY = box.top + box.height / 2;

          return {
            scale,
            x: centerX - layoutCenterX,
            y: centerY - layoutCenterY,
          };
        };

        gsap.set(wrapperEnlarge, {
          transformOrigin: '50% 50%',
          force3D: true,
          zIndex: 30,
        });

        gsap.set(sideContent, {
          autoAlpha: 0,
          y: 36,
          force3D: true,
        });

        const tl = gsap.timeline({
          defaults: { force3D: true },
          scrollTrigger: {
            id: `enlarge-${index}`,
            trigger: sectionEnlarge,
            start: 'top 90%',
            end: 'top 10%',
            scrub: 2,
            invalidateOnRefresh: true,
            // Recalc after pin sections (sustainability) change page height
            refreshPriority: -1,
          },
        });

        tl.fromTo(
          wrapperEnlarge,
          {
            scale: () => getFromState().scale,
            x: () => getFromState().x,
            y: () => getFromState().y,
          },
          {
            scale: 1,
            x: 0,
            y: 0,
            duration: 1,
            ease: 'power1.inOut',
          }
        ).to(
          sideContent,
          {
            autoAlpha: 1,
            y: 0,
            duration: 0.22,
            ease: 'power2.out',
          },
          0.8
        );

        const img = wrapperEnlarge.querySelector('img');
        if (img) {
          const onImgReady = () => {
            if (tl.scrollTrigger) tl.scrollTrigger.refresh();
          };
          if (img.complete) {
            onImgReady();
          } else {
            img.addEventListener('load', onImgReady, { once: true });
          }
        }

        cleanups.push(() => {
          sectionEnlarge.classList.remove('is-enlarge-active');
          if (tl.scrollTrigger) tl.scrollTrigger.kill();
          tl.kill();
          gsap.set([wrapperEnlarge, sideContent], {
            clearProps: 'transform,opacity,visibility,zIndex',
          });
        });
      });

      return () => {
        cleanups.forEach((fn) => fn());
      };
    });
  }


/* ---------------------------------------
   SUSTAINABILITY — video 50% center → full bleed
   Content settles in sync on the same scrub
--------------------------------------- */
  if (!reduceMotion && typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    const mmSustain = gsap.matchMedia();

    mmSustain.add('(min-width: 1024px)', () => {
      const section = document.querySelector('.home-sustainability-section');
      if (!section) return;

      const videoWrap = section.querySelector('.video-bg-wrap');
      const video = section.querySelector('.video-bg');
      const title = section.querySelector('.section-title-outer');
      const stats = section.querySelector('.sustainability-block');
      const cta = section.querySelector('.outer-btn-wrap');

      if (!videoWrap) return;

      const contentEls = [title, stats, cta].filter(Boolean);
      const getHeaderH = () =>
        parseFloat(
          getComputedStyle(document.documentElement).getPropertyValue('--header-h')
        ) || 0;

      section.classList.add('is-sustain-anim');

      gsap.set(videoWrap, {
        scale: 0.5,
        transformOrigin: '50% 50%',
        force3D: true,
        borderRadius: '0.625rem',
        autoAlpha: 0,
        y: 30,
      });

      gsap.set(contentEls, {
        autoAlpha: 0,
        y: 40,
        force3D: true,
      });

      const playVideo = () => {
        if (!video) return;
        const p = video.play();
        if (p && typeof p.catch === 'function') p.catch(() => {});
      };

      const playOdometers = () => {
        if (typeof window.PriyaOdometer?.play === 'function') {
          window.PriyaOdometer.play(section, { animate: true });
        }
      };

      const resetOdometers = () => {
        if (typeof window.PriyaOdometer?.reset === 'function') {
          window.PriyaOdometer.reset(section);
        }
      };

      // Entrance — starts once the section is into the viewport (top 80% —
      // was 70%, shortened so less scrolling is needed to reach it) and
      // ends exactly where the pin below picks up, so there's no dead gap:
      // video fades/settles in AND starts traveling (0.5→0.78 scale) while
      // still unpinned. Content stays hidden through this phase — it only
      // starts revealing once the video has visibly made progress in the
      // pinned phase below, so text never sits fully legible while the
      // video is still small mid-transform underneath it.
      const entranceTl = gsap.timeline({
        defaults: { force3D: true },
        scrollTrigger: {
          trigger: section,
          start: 'top 80%',
          end: () => `top top+=${getHeaderH()}`,
          // true, not a lag number — Lenis already smooths the raw scroll
          // input on desktop; stacking GSAP's own scrub lag on top of
          // Lenis's already-smoothed value compounds into a delay long
          // enough that normal-speed scrolling outruns the animation,
          // leaving it visibly stuck mid-transition.
          scrub: true,
          invalidateOnRefresh: true,
        },
      });

      entranceTl.to(videoWrap, {
        autoAlpha: 1,
        y: 0,
        scale: 0.78,
        ease: 'power2.out',
      }, 0);

      const settleDesktop = () => {
        gsap.set(videoWrap, {
          autoAlpha: 1,
          y: 0,
          scale: 1,
          borderRadius: 0,
        });
        gsap.set(contentEls, { autoAlpha: 1, y: 0 });
      };

      const tl = gsap.timeline({
        defaults: { force3D: true },
        scrollTrigger: {
          trigger: section,
          start: () => `top top+=${getHeaderH()}`,
          end: '+=55%',
          pin: true,
          // Same reasoning as the entrance timeline above — no extra GSAP
          // lag stacked on top of Lenis's own smoothing.
          scrub: true,
          invalidateOnRefresh: true,
          anticipatePin: 1,
          onEnter: playVideo,
          onEnterBack: playVideo,
          // Scrub lag can leave mid-scale + visible copy — lock end state
          onLeave: () => {
            settleDesktop();
            playOdometers();
          },
          onLeaveBack: resetOdometers,
        },
      });

      // 0.78 → full bleed first. Copy must stay hidden until scale ≈ 1,
      // otherwise white text sits on the white gutters and looks clipped.
      tl.to(
        videoWrap,
        {
          scale: 1,
          borderRadius: 0,
          duration: 0.65,
          ease: 'power1.inOut',
        },
        0
      );

      if (title) {
        tl.to(
          title,
          { autoAlpha: 1, y: 0, duration: 0.28, ease: 'power2.out' },
          0.62
        );
      }
      if (stats) {
        tl.to(
          stats,
          { autoAlpha: 1, y: 0, duration: 0.28, ease: 'power2.out' },
          0.72
        );
        tl.call(
          () => {
            if (tl.scrollTrigger && tl.scrollTrigger.direction === 1) {
              playOdometers();
            } else if (tl.scrollTrigger && tl.scrollTrigger.direction === -1) {
              resetOdometers();
            }
          },
          null,
          0.74
        );
      }
      if (cta) {
        tl.to(
          cta,
          { autoAlpha: 1, y: 0, duration: 0.25, ease: 'power2.out' },
          0.8
        );
      }

      ScrollTrigger.refresh();

      return () => {
        section.classList.remove('is-sustain-anim');
        resetOdometers();
        if (tl.scrollTrigger) tl.scrollTrigger.kill();
        tl.kill();
        if (entranceTl.scrollTrigger) entranceTl.scrollTrigger.kill();
        entranceTl.kill();
        gsap.set([videoWrap, ...contentEls], {
          clearProps: 'transform,opacity,visibility,borderRadius',
        });
      };
    });

    // Mobile / tablet: play-once content entrance + gentle video ken-burns
    // (full-bleed wrap — never scale the wrap; that clips copy)
    mmSustain.add('(max-width: 1023px)', () => {
      const section = document.querySelector('.home-sustainability-section');
      if (!section) return;

      const videoWrap = section.querySelector('.video-bg-wrap');
      const video = section.querySelector('.video-bg');
      const title = section.querySelector('.section-title-outer');
      const stats = section.querySelector('.sustainability-block');
      const cta = section.querySelector('.outer-btn-wrap');
      if (!videoWrap) return;

      const statRows = stats
        ? gsap.utils.toArray(stats.querySelectorAll('.counter-wrap-outer'))
        : [];
      const contentEls = [title, ...(statRows.length ? statRows : stats ? [stats] : []), cta].filter(
        Boolean
      );

      section.classList.add('is-sustain-anim');

      gsap.set(videoWrap, {
        scale: 1,
        x: 0,
        y: 0,
        borderRadius: 0,
        autoAlpha: 0,
      });
      if (video) {
        gsap.set(video, {
          scale: 1,
          xPercent: 0,
          yPercent: 0,
          transformOrigin: '50% 45%',
          force3D: true,
        });
      }
      gsap.set(contentEls, { autoAlpha: 0, y: 28, force3D: true });

      const playVideo = () => {
        if (!video) return;
        const p = video.play();
        if (p && typeof p.catch === 'function') p.catch(() => {});
      };

      const playOdometers = () => {
        if (typeof window.PriyaOdometer?.play === 'function') {
          window.PriyaOdometer.play(section, { animate: true });
        }
      };

      const resetOdometers = () => {
        if (typeof window.PriyaOdometer?.reset === 'function') {
          window.PriyaOdometer.reset(section);
        }
      };

      // Slow drift on the <video> (wrap stays transform:none via CSS)
      const kenBurns = video
        ? gsap.to(video, {
            scale: 1.1,
            xPercent: -2.5,
            yPercent: 1.5,
            duration: 16,
            ease: 'sine.inOut',
            repeat: -1,
            yoyo: true,
            paused: true,
            force3D: true,
          })
        : null;

      const entranceTl = gsap.timeline({
        paused: true,
        defaults: { force3D: true, ease: 'power3.out' },
        onComplete: playOdometers,
        onReverseComplete: resetOdometers,
      });

      entranceTl.to(videoWrap, { autoAlpha: 1, duration: 0.75, ease: 'power2.out' }, 0);

      if (title) {
        entranceTl.to(title, { autoAlpha: 1, y: 0, duration: 0.8 }, 0.18);
      }
      if (statRows.length) {
        entranceTl.to(
          statRows,
          { autoAlpha: 1, y: 0, duration: 0.7, stagger: 0.12 },
          0.38
        );
      } else if (stats) {
        entranceTl.to(stats, { autoAlpha: 1, y: 0, duration: 0.7 }, 0.38);
      }
      if (cta) {
        entranceTl.to(cta, { autoAlpha: 1, y: 0, duration: 0.6 }, 0.62);
      }

      const st = ScrollTrigger.create({
        trigger: section,
        start: 'top 78%',
        end: 'bottom 15%',
        invalidateOnRefresh: true,
        onEnter: () => {
          playVideo();
          entranceTl.play();
          if (kenBurns) kenBurns.play();
        },
        onEnterBack: () => {
          playVideo();
          entranceTl.play();
          if (kenBurns) kenBurns.play();
        },
        onLeave: () => {
          if (kenBurns) kenBurns.pause();
        },
        onLeaveBack: () => {
          entranceTl.reverse();
          if (kenBurns) {
            kenBurns.pause();
            kenBurns.progress(0);
          }
          if (video) gsap.set(video, { scale: 1, xPercent: 0, yPercent: 0 });
        },
      });

      return () => {
        section.classList.remove('is-sustain-anim');
        resetOdometers();
        st.kill();
        entranceTl.kill();
        if (kenBurns) kenBurns.kill();
        gsap.set([videoWrap, ...contentEls, video].filter(Boolean), {
          clearProps: 'transform,opacity,visibility,borderRadius',
        });
      };
    });

    // After pin sections init, refresh so careers/whatwestandfor enlarge
    // start/end positions stay correct
    requestAnimationFrame(() => {
      ScrollTrigger.refresh();
    });
  }


/* ---------------------------------------
   TESTIMONIALS
   Desktop: opposite vertical marquee + hover pause + drag/swipe
   Mobile: static list + Load more
--------------------------------------- */
  function makeLoopable(trackId) {
    const track = document.getElementById(trackId);
    if (!track) return;

    // Avoid double-cloning on breakpoint toggles
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
  let mobileQuoteRiseTriggers = [];

  function getOriginalQuoteCards() {
    if (!sectionMarquee) return [];
    return Array.from(
      sectionMarquee.querySelectorAll('.quote-card:not([aria-hidden="true"])')
    );
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

  /** Same rise pattern as .our-products-cards — mobile quote list only */
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

    sectionMarquee.classList.add('is-mobile-rise-anim');
    const cards = getOriginalQuoteCards().filter(
      (card) => !card.classList.contains('is-collapsed')
    );

    cards.forEach((card, i) => {
      if (card.dataset.riseBound === '1') return;
      card.dataset.riseBound = '1';
      card.classList.add('is-rise-anim-card');

      const fromY = Math.min(60 * (i + 1), 200);
      const col = i % 2;
      const start = `top ${90 - col * 4}%`;
      const end = `top ${58 - col * 3}%`;

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
    const speed = 0.42; // px per frame ≈ calm 30s loop feel

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
      // Desktop only — never capture touch on mobile (blocks page scroll)
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

    // After a drag, don't follow links accidentally
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

    // Kill any in-progress drag so touch can scroll the page
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
      requestAnimationFrame(() => bindMobileQuoteRise());
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

    requestAnimationFrame(() => bindMobileQuoteRise());
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

});
