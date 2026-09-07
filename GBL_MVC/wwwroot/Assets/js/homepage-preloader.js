/* ---------------------------------------
   SITE PRELOADER
   Lion stroke-draws while loading; wordmark stays fully colored.
   Then both fly to the header logo and swap to priyacement-logo-red.
--------------------------------------- */
(function () {
  const LOADER_SEEN_KEY = 'priyaHomepageLoaderSeen';
  // Lion + wordmark regions inside priyacement-logo-red.svg (viewBox 438×98)
  const LION_SLOT = { x: 0.0, y: 0.08, w: 0.27, h: 0.85 };
  const WORDMARK_SLOT = { x: 0.28, y: 0.12, w: 0.70, h: 0.78 };

  function whenReady(fn) {
    // Start as soon as #sitePreloader exists (script sits right after it).
    // Waiting for full DOMContentLoaded caused a long blank gray screen.
    if (document.getElementById('sitePreloader')) {
      fn();
      return;
    }
    if (document.readyState === 'loading') {
      document.addEventListener('DOMContentLoaded', fn, { once: true });
    } else {
      fn();
    }
  }

  function isPageReload() {
    try {
      const nav = performance.getEntriesByType && performance.getEntriesByType('navigation')[0];
      if (nav) return nav.type === 'reload';
    } catch (e) { /* ignore */ }
    try {
      return !!(performance.navigation && performance.navigation.type === 1);
    } catch (e) {
      return false;
    }
  }

  function hasSeenLoaderThisSession() {
    try {
      return sessionStorage.getItem(LOADER_SEEN_KEY) === '1';
    } catch (e) {
      return false;
    }
  }

  function markLoaderSeen() {
    try {
      sessionStorage.setItem(LOADER_SEEN_KEY, '1');
    } catch (e) { /* private mode / blocked */ }
  }

  function skipLoader() {
    if (new URLSearchParams(window.location.search).get('loader') === '0') return true;
    if (isPageReload()) return false;
    return hasSeenLoaderThisSession();
  }

  function announceDone() {
    document.dispatchEvent(new CustomEvent('homepagePreloaderDone'));
  }

  function lockScroll(html) {
    html.classList.add('site-preloader-lock');
    const block = (e) => e.preventDefault();
    html._preloaderScrollBlock = block;
    window.addEventListener('wheel', block, { passive: false });
    window.addEventListener('touchmove', block, { passive: false });
    window.addEventListener('keydown', html._preloaderKeyBlock = (e) => {
      const keys = ['ArrowUp', 'ArrowDown', 'PageUp', 'PageDown', 'Home', 'End', ' ', 'Spacebar'];
      if (keys.indexOf(e.key) !== -1) e.preventDefault();
    });
  }

  function unlockScroll(html) {
    html.classList.remove('site-preloader-lock');
    if (html._preloaderScrollBlock) {
      window.removeEventListener('wheel', html._preloaderScrollBlock);
      window.removeEventListener('touchmove', html._preloaderScrollBlock);
      html._preloaderScrollBlock = null;
    }
    if (html._preloaderKeyBlock) {
      window.removeEventListener('keydown', html._preloaderKeyBlock);
      html._preloaderKeyBlock = null;
    }
  }

  /** Page must be fully painted under the cover before it lifts — no white gap. */
  function preparePageUnderCover() {
    const hero = document.querySelector('.hero-banner-section');
    const swiper = document.querySelector('.hero-swiper');
    if (hero) {
      hero.style.transition = 'none';
      hero.style.opacity = '1';
      hero.style.transform = 'none';
      hero.style.filter = 'none';
      hero.style.visibility = 'visible';
    }
    if (swiper) {
      swiper.style.opacity = '1';
      swiper.style.visibility = 'visible';
    }

    document.querySelectorAll('.hero-swiper .slide-media, .hero-swiper .slide-bg, .hero-swiper .bannerinner-picture').forEach((el) => {
      el.style.opacity = '1';
      el.style.visibility = 'visible';
    });

    const activeSlide = document.querySelector('.hero-swiper .swiper-slide-active') ||
      document.querySelector('.hero-swiper .swiper-slide');
    if (!activeSlide) return;

    const content = activeSlide.querySelector('.slide-content');
    const btnWrap = activeSlide.querySelector('.outer-hero-button');
    if (content) {
      content.style.transition = 'none';
      content.style.opacity = '1';
      content.style.transform = 'none';
    }
    if (btnWrap) {
      btnWrap.style.transition = 'none';
      btnWrap.style.opacity = '1';
    }
  }

  /** Do not lift the cover until the first hero image can paint (avoids white blank). */
  function waitForHeroReady(timeoutMs) {
    const img =
      document.querySelector('.hero-swiper .swiper-slide-active .slide-bg') ||
      document.querySelector('.hero-swiper .slide-bg');

    return new Promise((resolve) => {
      let done = false;
      const finish = () => {
        if (done) return;
        done = true;
        resolve(!!(img && img.naturalWidth > 0));
      };

      if (!img) {
        window.setTimeout(finish, 0);
        return;
      }

      if (img.complete && img.naturalWidth > 0) {
        finish();
        return;
      }

      img.addEventListener('load', finish, { once: true });
      img.addEventListener('error', finish, { once: true });
      if (typeof img.decode === 'function') {
        img.decode().then(finish).catch(() => {
          /* keep waiting for load/error/timeout */
        });
      }

      // Absolute fallback only — prefer real image paint
      window.setTimeout(finish, timeoutMs || 5000);
    });
  }

  function nextPaint() {
    return new Promise((resolve) => {
      requestAnimationFrame(() => requestAnimationFrame(resolve));
    });
  }

  /** Soft text lift once the cover starts leaving (hero media already visible). */
  function softRevealHeroCopy() {
    const activeSlide =
      document.querySelector('.hero-swiper .swiper-slide-active') ||
      document.querySelector('.hero-swiper .swiper-slide');
    if (!activeSlide) return;

    const content = activeSlide.querySelector('.slide-content');
    const btnWrap = activeSlide.querySelector('.outer-hero-button');

    if (content && typeof gsap !== 'undefined') {
      gsap.fromTo(
        content,
        { y: 12, opacity: 0.92 },
        { y: 0, opacity: 1, duration: 0.55, ease: 'power2.out', clearProps: 'transform,opacity' }
      );
    }
    if (btnWrap && typeof gsap !== 'undefined') {
      gsap.fromTo(
        btnWrap,
        { opacity: 0.85 },
        { opacity: 1, duration: 0.45, delay: 0.08, ease: 'power2.out', clearProps: 'opacity' }
      );
    }
  }

  function getHeaderLogoEl() {
    return (
      document.querySelector('#siteHeader .logo-link img') ||
      document.querySelector('.site-header .logo-link img')
    );
  }

  function getLionSlotRect(logoRect) {
    return {
      left: logoRect.left + logoRect.width * LION_SLOT.x,
      top: logoRect.top + logoRect.height * LION_SLOT.y,
      width: logoRect.width * LION_SLOT.w,
      height: logoRect.height * LION_SLOT.h,
    };
  }

  function stripFillsKeepOrig(svg) {
    svg.querySelectorAll('path').forEach((p) => {
      const f = p.getAttribute('data-orig-fill') || p.getAttribute('fill');
      if (f && f !== 'none') p.setAttribute('data-orig-fill', f);
      p.setAttribute('fill', 'none');
      p.style.fill = 'none';
    });
  }

  function restoreFilledPaths(svg) {
    svg.querySelectorAll('path').forEach((p) => {
      const orig = p.getAttribute('data-orig-fill') || p.getAttribute('fill');
      if (orig && orig !== 'none') {
        p.setAttribute('fill', orig);
        p.style.fill = orig;
      }
      p.setAttribute('stroke', 'none');
      p.style.stroke = 'none';
    });
  }

  function prepareLoaderSvg(svg, keepFills) {
    svg.removeAttribute('width');
    svg.removeAttribute('height');
    svg.setAttribute('preserveAspectRatio', 'xMidYMid meet');
    if (keepFills) restoreFilledPaths(svg);
    else stripFillsKeepOrig(svg);
    return svg;
  }

  function loadSvg(container, fallbackSrc, cacheKey, keepFills) {
    // Prefer SVG already inlined in HTML (first paint — no blank wait)
    const existing = container && container.querySelector('svg');
    if (existing) {
      return Promise.resolve(prepareLoaderSvg(existing, keepFills));
    }

    const src = container.getAttribute('data-svg-src') || fallbackSrc;
    const cached =
      (typeof window !== 'undefined' &&
        window.__priyaLoaderSvgs &&
        cacheKey &&
        window.__priyaLoaderSvgs[cacheKey]) ||
      null;
    const textPromise = cached
      ? cached
      : fetch(src, { credentials: 'same-origin' }).then((res) => {
          if (!res.ok) throw new Error('SVG fetch failed');
          return res.text();
        });

    return textPromise.then((text) => {
      container.innerHTML = text;
      const svg = container.querySelector('svg');
      if (!svg) throw new Error('No SVG root');
      return prepareLoaderSvg(svg, keepFills);
    });
  }

  function setupStrokeDraw(path, strokeColor, strokeWidth) {
    // Stroke only — never show fill until fillPath() at the end
    path.setAttribute('fill', 'none');
    path.style.fill = 'none';
    path.setAttribute('stroke', strokeColor);
    path.style.stroke = strokeColor;
    path.setAttribute('stroke-width', String(strokeWidth));
    path.setAttribute('stroke-linecap', 'round');
    path.setAttribute('stroke-linejoin', 'round');
    path.setAttribute('vector-effect', 'non-scaling-stroke');
    path.style.strokeOpacity = '1';
    let len = 0;
    try {
      len = path.getTotalLength();
    } catch (e) {
      len = 3000;
    }
    path.style.strokeDasharray = String(len);
    path.style.strokeDashoffset = String(len); // fully undrawn
    return len;
  }

  function setDrawProgress(path, len, progress01) {
    const p = Math.max(0, Math.min(1, progress01));
    path.style.strokeDashoffset = String(len * (1 - p));
  }

  function fillPath(path, logoEl) {
    path.style.transition = 'fill 0.4s ease, stroke-opacity 0.4s ease';
    path.setAttribute('fill', '#ED1C24');
    path.setAttribute('stroke', 'none');
    path.style.strokeOpacity = '0';
    path.style.strokeDasharray = 'none';
    path.style.strokeDashoffset = '0';
    if (logoEl) logoEl.classList.add('is-filled');
  }

  function init() {
    if (window.__priyaPreloaderStarted) return;
    window.__priyaPreloaderStarted = true;

    const html = document.documentElement;
    const preloader = document.getElementById('sitePreloader');
    const fill = document.getElementById('sitePreloaderFill');
    const counter = document.getElementById('sitePreloaderCounter');
    const logoWrap = preloader && preloader.querySelector('.site-preloader__logo-wrap');
    const progressEl = preloader && preloader.querySelector('.site-preloader__progress');
    const logo = document.getElementById('sitePreloaderLogo');
    const wordmark = document.getElementById('sitePreloaderWordmark');

    const finishImmediate = () => {
      html.classList.remove('site-preloader-pending');
      unlockScroll(html);
      if (preloader) preloader.remove();
      announceDone();
    };

    if (!preloader || !fill || !counter || !logoWrap || !logo) {
      finishImmediate();
      return;
    }

    if (skipLoader() || window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
      finishImmediate();
      return;
    }

    lockScroll(html);
    document.getElementById('siteHeader')?.setAttribute('inert', '');
    document.getElementById('main-content')?.setAttribute('inert', '');

    function unlockPage() {
      document.getElementById('siteHeader')?.removeAttribute('inert');
      document.getElementById('main-content')?.removeAttribute('inert');
    }

    function setProgress(n) {
      fill.style.width = n + '%';
      counter.firstChild.textContent = n;
    }

    let finished = false;
    let exitStarted = false;
    let revealStarted = false;
    let pathLen = 0;
    let lionPath = null;
    // Shorter brand hold — ABET-like pace
    const MIN_MS = 1400;
    const MAX_MS = 3200;
    let pageLoaded = document.readyState === 'complete';
    window.addEventListener('load', () => { pageLoaded = true; }, { once: true });

    let startTime = 0;
    let shown = 0;

    function applyDraw(progress01) {
      // Ease so lion stroke spends more time in the middle (easier to watch)
      const t = Math.max(0, Math.min(1, progress01));
      const eased = t < 0.5 ? 2 * t * t : 1 - Math.pow(-2 * t + 2, 2) / 2;
      if (lionPath && pathLen) setDrawProgress(lionPath, pathLen, eased);
    }

    function flyToHeaderThenExit() {
      if (exitStarted) return;
      exitStarted = true;

      const header = document.getElementById('siteHeader');
      const headerLogo = getHeaderLogoEl();

      function resetHeaderLogo() {
        if (!headerLogo) return;
        try {
          if (typeof gsap !== 'undefined') gsap.killTweensOf(headerLogo);
        } catch (e) { /* ignore */ }
        headerLogo.style.opacity = '1';
        headerLogo.style.visibility = 'visible';
        headerLogo.style.transform = '';
        headerLogo.style.transition = '';
        headerLogo.style.clipPath = '';
        if (typeof gsap !== 'undefined') {
          gsap.set(headerLogo, { clearProps: 'opacity,visibility,x,y,scale,transform' });
        }
      }

      const cleanup = () => {
        try {
          if (typeof gsap !== 'undefined') {
            gsap.killTweensOf(logo);
            if (wordmark) gsap.killTweensOf(wordmark);
            if (progressEl) gsap.killTweensOf(progressEl);
            if (preloader) gsap.killTweensOf(preloader);
            if (headerLogo) gsap.killTweensOf(headerLogo);
            if (header) gsap.killTweensOf(header);
          }
        } catch (e) { /* ignore */ }

        html.classList.remove('site-preloader-travel');
        html.classList.remove('site-preloader-exiting');
        document.querySelectorAll('.site-preloader__logo.is-flying, .site-preloader__wordmark.is-flying').forEach((el) => el.remove());
        if (logo && logo.parentNode) logo.remove();
        if (wordmark && wordmark.parentNode) wordmark.remove();
        if (preloader && preloader.parentNode) preloader.remove();

        if (header) {
          header.removeAttribute('style');
          header.style.opacity = '1';
          header.style.visibility = 'visible';
          header.classList.remove('is-intro');
        }
        resetHeaderLogo();
        unlockScroll(html);
      };

      function armHeaderForHandoff() {
        if (header && typeof gsap !== 'undefined') {
          try {
            gsap.killTweensOf(header);
            header.querySelectorAll('.logo-link, .primary-nav, .header-actions').forEach((el) => {
              gsap.killTweensOf(el);
              gsap.set(el, { clearProps: 'opacity,visibility,transform' });
              el.style.opacity = '1';
              el.style.visibility = 'visible';
            });
            gsap.set(header, { clearProps: 'opacity,visibility,transform' });
          } catch (e) { /* ignore */ }
        }

        if (header) {
          header.style.transition = 'none';
          header.style.opacity = '1';
          header.style.visibility = 'visible';
          header.style.transform = 'none';
          header.style.pointerEvents = 'none';
          header.classList.remove('is-intro');
        }
        if (headerLogo) {
          headerLogo.style.transition = 'none';
          headerLogo.style.opacity = '0';
          headerLogo.style.visibility = 'visible';
          headerLogo.style.transform = 'none';
          headerLogo.style.clipPath = 'none';
        }
        void (header && header.offsetHeight);
      }

      function runFlight() {
        return new Promise((resolve) => {
          if (!headerLogo || typeof gsap === 'undefined') {
            if (headerLogo) headerLogo.style.opacity = '1';
            resolve();
            return;
          }

          const fromLion = logo.getBoundingClientRect();
          const fromWm = wordmark ? wordmark.getBoundingClientRect() : null;
          const logoBox = headerLogo.getBoundingClientRect();
          if (!fromLion.width || !fromLion.height || !logoBox.width || !logoBox.height) {
            headerLogo.style.opacity = '1';
            resolve();
            return;
          }

          function regionTarget(fromRect, box, slot) {
            const left = box.left + box.width * slot.x;
            const top = box.top + box.height * slot.y;
            const width = box.width * slot.w;
            const height = box.height * slot.h;
            const s = Math.min(width / fromRect.width, height / fromRect.height);
            return {
              scale: s,
              x: left + (width - fromRect.width * s) / 2 - fromRect.left,
              y: top + (height - fromRect.height * s) / 2 - fromRect.top,
            };
          }

          const lionStart = regionTarget(fromLion, logoBox, LION_SLOT);
          const wmStart =
            fromWm && fromWm.width ? regionTarget(fromWm, logoBox, WORDMARK_SLOT) : null;
          const flightDur = 0.95;

          // Lift flyers first, then drop gray sheet (!important lock was keeping it)
          logo.classList.add('is-flying');
          document.body.appendChild(logo);
          gsap.set(logo, {
            position: 'fixed',
            left: fromLion.left,
            top: fromLion.top,
            width: fromLion.width,
            height: fromLion.height,
            x: 0,
            y: 0,
            scale: 1,
            opacity: 1,
            margin: 0,
            zIndex: 10550,
            transformOrigin: '0% 0%',
            force3D: true,
          });

          if (wordmark && wmStart) {
            wordmark.classList.add('is-flying');
            document.body.appendChild(wordmark);
            gsap.set(wordmark, {
              position: 'fixed',
              left: fromWm.left,
              top: fromWm.top,
              width: fromWm.width,
              height: fromWm.height,
              x: 0,
              y: 0,
              scale: 1,
              opacity: 1,
              margin: 0,
              zIndex: 10551,
              transformOrigin: '0% 0%',
              force3D: true,
            });
          }

          // Unlock lock class — it forced background:#f5f5f5 !important during travel
          unlockScroll(html);
          html.classList.add('site-preloader-travel');
          preloader.classList.add('is-traveling');
          preloader.style.setProperty('background', 'transparent', 'important');
          preloader.style.setProperty('background-color', 'transparent', 'important');
          Array.prototype.forEach.call(preloader.children, (child) => {
            gsap.set(child, { autoAlpha: 0 });
          });

          const tl = gsap.timeline({
            onComplete: () => {
              if (logo && logo.parentNode) logo.remove();
              if (wordmark && wordmark.parentNode) wordmark.remove();
              if (headerLogo) {
                headerLogo.style.opacity = '1';
                headerLogo.style.visibility = 'visible';
              }
              resetHeaderLogo();
              resolve();
            },
          });

          tl.to(
            logo,
            {
              x: lionStart.x,
              y: lionStart.y,
              scale: lionStart.scale,
              duration: flightDur,
              ease: 'power3.inOut',
              force3D: true,
            },
            0
          );

          if (wordmark && wordmark.classList.contains('is-flying') && wmStart) {
            tl.to(
              wordmark,
              {
                x: wmStart.x,
                y: wmStart.y,
                scale: wmStart.scale,
                duration: flightDur,
                ease: 'power3.inOut',
                force3D: true,
              },
              0
            );
          }
        });
      }

      // Hero ready under cover → travel with NO gray sheet behind
      waitForHeroReady(5000)
        .then(() => {
          preparePageUnderCover();
          return nextPaint();
        })
        .then(() => {
          html.classList.remove('site-preloader-pending');
          markLoaderSeen();
          unlockPage();
          armHeaderForHandoff();

          if (!revealStarted) {
            revealStarted = true;
            softRevealHeroCopy();
            announceDone();
          }

          if (typeof gsap === 'undefined') {
            if (header) header.style.pointerEvents = '';
            resetHeaderLogo();
            cleanup();
            return;
          }

          return runFlight();
        })
        .then(() => {
          if (header) header.style.pointerEvents = '';
          resetHeaderLogo();
          cleanup();
        });
    }

    function beginExit() {
      if (exitStarted) return;

      setProgress(100);
      applyDraw(1);
      if (lionPath) fillPath(lionPath, logo);

      window.setTimeout(() => {
        const afterFade = () => flyToHeaderThenExit();

        if (progressEl && typeof gsap !== 'undefined') {
          gsap.to(progressEl, {
            autoAlpha: 0,
            duration: 0.18,
            ease: 'power2.out',
            onComplete: afterFade,
          });
        } else if (progressEl) {
          progressEl.style.transition = 'opacity 0.18s ease';
          progressEl.style.opacity = '0';
          window.setTimeout(afterFade, 180);
        } else {
          afterFade();
        }
      }, 100);
    }

    function finishAtFull() {
      if (finished) return;
      finished = true;
      beginExit();
    }

    function tick() {
      if (finished) return;
      const elapsed = performance.now() - startTime;
      const minRatio = Math.min(1, elapsed / MIN_MS);
      const target = pageLoaded ? 100 : Math.min(88, Math.round(minRatio * 88));
      shown += (target - shown) * 0.18;
      if (target - shown < 0.35) shown = target;
      const displayed = Math.round(shown);
      setProgress(displayed);

      // Draw exactly with % — 100% means fully drawn
      applyDraw(displayed / 100);

      if (displayed >= 100 || elapsed > MAX_MS) {
        finishAtFull();
        return;
      }
      requestAnimationFrame(tick);
    }

    function startLoaderUi() {
      if (preloader.classList.contains('is-ready')) return;
      preloader.classList.add('is-ready');
      // Draw starts at 0 — stroke only; fill happens later in beginExit
      startTime = performance.now();
      shown = 0;
      setProgress(0);
      applyDraw(0);
      requestAnimationFrame(tick);
    }

    // Start lion as soon as it lands — don't wait on wordmark (cuts initial blank)
    const lionReady = loadSvg(logo, '/Assets/images/logo/lion-vector-loader.svg', 'lion').then((svg) => {
      const path = svg.querySelector('path');
      if (!path) throw new Error('No lion path');
      lionPath = path;
      pathLen = setupStrokeDraw(path, '#ED1C24', 1.75);
      startLoaderUi();
    });

    const wordmarkReady = wordmark
      ? loadSvg(wordmark, '/Assets/images/logo/logo-vector-loader.svg', 'wordmark', true)
          .catch(() => { /* optional — colored wordmark already inlined */ })
      : Promise.resolve();

    Promise.all([lionReady, wordmarkReady]).catch(() => {
      startLoaderUi();
    });

    window.setTimeout(finishAtFull, MAX_MS + 800);
  }

  whenReady(init);
})();
