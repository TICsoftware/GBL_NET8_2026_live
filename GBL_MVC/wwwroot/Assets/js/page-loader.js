/* =====================================================================
   Homepage intro loader — modelled on prototypestudio.fr
   1. white stage with thin guide lines + counter 00 -> 100 sliding down
   2. column of image cards rises through the screen (inner parallax)
   3. the first hero banner image rises in as a card, then expands to
      full screen; cards drift up and fade
   4. the loader fades, revealing the real hero (same image underneath)
   Phones (<768px), reduced-motion and repeat visits in the same browser
   session get a short white fade instead, like the reference site.
   Needs: markup right after <body>, page-loader.css in <head>.
   ===================================================================== */
(function () {
  var PLAY_ONCE_PER_SESSION = false;    /* false = full intro on every visit */
  var MIN_WIDTH = 768;                  /* below this, simple fade (hero isn't full-screen on phones) */
  var SESSION_KEY = 'gblIntroPlayed';
  var SLOW_DOWN = 1.4;                  /* 1 = original speed, 1.4 = 40% slower, 2 = half speed */

  var root = document.documentElement;
  var loader = document.getElementById('gbl-loader');
  if (!loader) { root.classList.remove('gbl-lock'); return; }

  var reduce = window.matchMedia && matchMedia('(prefers-reduced-motion: reduce)').matches;
  var played = false;
  try { played = sessionStorage.getItem(SESSION_KEY) === '1'; } catch (e) {}
  /* skip the intro when arriving from another page of this site (link click
     or back/forward); play it on refresh / hard refresh and on direct visits */
  var navType = '';
  try {
    var navEntry = performance.getEntriesByType('navigation')[0];
    navType = navEntry ? navEntry.type
      : ({0: 'navigate', 1: 'reload', 2: 'back_forward'})[performance.navigation.type] || '';
  } catch (e) {}
  var fromSamesite = false;
  try { fromSamesite = !!document.referrer && new URL(document.referrer).origin === location.origin; } catch (e) {}
  var internalNav = navType === 'back_forward' || (navType !== 'reload' && fromSamesite);

  var full = !reduce && innerWidth >= MIN_WIDTH && !internalNav && !(PLAY_ONCE_PER_SESSION && played);
  try { sessionStorage.setItem(SESSION_KEY, '1'); } catch (e) {}

  /* ---------------- shared: hero slider + smooth scroll hooks ---------------- */
  var heroSwiper = null, autoplayHeld = false, finished = false;
  function lenis(method) {
    if (window.lenis && typeof window.lenis[method] === 'function') window.lenis[method]();
  }
  function onDomReady(fn) {
    if (document.readyState === 'loading') addEventListener('DOMContentLoaded', fn); else fn();
  }
  /* runs after every other DOMContentLoaded handler (hero slider init, Lenis) */
  onDomReady(function () {
    setTimeout(function () {
      if (finished) return;
      lenis('stop');
      if (!full) return;
      var el = document.querySelector('.hero-swiper');
      heroSwiper = el && el.swiper;
      if (heroSwiper && heroSwiper.autoplay && heroSwiper.autoplay.running) {
        heroSwiper.autoplay.stop(); autoplayHeld = true;   /* keep slide 1 until the reveal */
      }
    }, 0);
  });

  function finish() {
    if (finished) return; finished = true;
    loader.classList.add('gbl-done');
    root.classList.remove('gbl-lock');
    lenis('start');
    if (autoplayHeld && heroSwiper && heroSwiper.autoplay) heroSwiper.autoplay.start();
    setTimeout(function () { if (loader.parentNode) loader.parentNode.removeChild(loader); }, 700);
  }

  /* ---------------- simple mode: white curtain fades away ---------------- */
  /* coming from another page of the site: no loader at all */
  if (internalNav) {
    finished = true;               /* also cancels the scroll-pause hook above */
    if (loader.parentNode) loader.parentNode.removeChild(loader);
    root.classList.remove('gbl-lock');
    return;
  }

  if (!full) {
    loader.classList.add('gl-simple');
    onDomReady(function () { setTimeout(finish, 250); });
    setTimeout(finish, 4000);
    return;
  }

  /* ========================= full intro ========================= */
  try { history.scrollRestoration = 'manual'; } catch (e) {}
  window.scrollTo(0, 0);

  var items = [].slice.call(loader.querySelectorAll('.gl-item'));
  var inners = items.map(function (i) { return i.querySelector('.gl-item-inner'); });
  var images = items.map(function (i) { return i.querySelector('.gl-item-image'); });
  var counter = loader.querySelector('.gl-counter');
  var countEl = loader.querySelector('.gl-count');
  var cover = loader.querySelector('.gl-cover');
  var coverImg = cover.querySelector('img');
  var sunEl = loader.querySelector('.gl-sun');        /* optional logo scene */
  var caneEl = loader.querySelector('.gl-cane');

  /* lazy images: only fetched in full mode */
  var imgs = [].slice.call(loader.querySelectorAll('img[data-src]'));
  imgs.forEach(function (im) { im.src = im.getAttribute('data-src'); });

  /* ---------------- easing (same curves as the reference) ---------------- */
  function bezierLUT(segs) {               /* segs: [[x0,y0,x1,y1,x2,y2,x3,y3], ...] */
    var pts = [];
    segs.forEach(function (s) {
      for (var i = 0; i <= 60; i++) {
        var t = i / 60, u = 1 - t;
        pts.push([u*u*u*s[0] + 3*u*u*t*s[2] + 3*u*t*t*s[4] + t*t*t*s[6],
                  u*u*u*s[1] + 3*u*u*t*s[3] + 3*u*t*t*s[5] + t*t*t*s[7]]);
      }
    });
    pts.sort(function (a, b) { return a[0] - b[0]; });
    return function (x) {
      if (x <= 0) return 0; if (x >= 1) return 1;
      var lo = 0, hi = pts.length - 1;
      while (hi - lo > 1) { var m = (lo + hi) >> 1; if (pts[m][0] < x) lo = m; else hi = m; }
      var a = pts[lo], b = pts[hi], d = b[0] - a[0];
      return d ? a[1] + (b[1] - a[1]) * (x - a[0]) / d : b[1];
    };
  }
  var inOut = bezierLUT([[0,0, .094,.026, .124,.127, .157,.29],
                         [.157,.29, .197,.486, .243,.81, .337,.894],
                         [.337,.894, .409,.959, .374,1, 1,1]]);
  var alpha = bezierLUT([[0,0, .25,.46, .45,.94, 1,1]]);
  function expoInOut(t) {
    if (t <= 0) return 0; if (t >= 1) return 1;
    return t < .5 ? Math.pow(2, 20 * t - 10) / 2 : (2 - Math.pow(2, -20 * t + 10)) / 2;
  }
  function lerp(a, b, p) { return a + (b - a) * p; }

  /* ---------------- tiny timeline ---------------- */
  var tweens = [], END = 0;
  function add(start, dur, ease, fn) {
    tweens.push({s: start, d: dur, e: ease, f: fn});
    END = Math.max(END, start + dur);
  }

  /* per-element state, written to the DOM once per frame */
  var st = {
    counterY: 0, counterA: 1, count: 0,
    item: items.map(function () { return {y: 0, a: 1, innerY: 0, imgY: -10}; }),
    coverY: 0, clipT: 0, clipL: 0, coverS: 1, coverImgY: -10,
    sunY: 30, caneY: 12
  };

  function build() {
    var W = innerWidth, H = innerHeight;
    var rem = parseFloat(getComputedStyle(root).fontSize) || 16;
    var n = items.length, e = 0.32 * n;     /* main travel duration */
    var last = items[n - 1];
    /* layout positions without transforms */
    items.forEach(function (it) { it.style.transform = 'none'; });
    counter.style.transform = 'none';
    var r = last.getBoundingClientRect();
    var w = r.width, h = r.height;
    var animEnd = r.bottom + h + 2 * rem - h / 2 - H / 2;
    var cr = counter.getBoundingClientRect();
    var counterTravel = H - 2 * rem - cr.bottom;
    var insetT = (H - h) / 2, insetL = (W - w) / 2;
    var coverY0 = animEnd + H / 2 + 1.5 * h + 2 * rem;
    var coverS0 = Math.max(w / W, h / H) + 0.3;

    tweens = []; END = 0;
    var D = 0.1;                                       /* small start delay */

    add(D, e, inOut, function (p) { st.counterY = lerp(0, counterTravel, p); st.count = Math.round(100 * p); });
    items.forEach(function (it, i) {
      add(D + 0.025 * i, e, inOut, function (p) { st.item[i].y = lerp(H, -animEnd, p); });
      add(D + 0.4 + 0.025 * i, e, inOut, function (p) { st.item[i].imgY = lerp(-10, 10, p); });
    });
    add(D, e + 0.4, inOut, function (p) { st.sunY = lerp(30, 0, p); });    /* sun rises */
    add(D + 0.2, e, inOut, function (p) { st.caneY = lerp(12, 0, p); });   /* cane grows */
    add(D + 0.025 * n, e, inOut, function (p) { st.coverY = lerp(coverY0, 0, p); });
    add(D + 0.4 + 0.025 * n, e, inOut, function (p) { st.coverImgY = lerp(-10, 0, p); });

    var clipStart = D + 0.025 * n + 0.47 * e;
    add(clipStart, 1.6, expoInOut, function (p) {
      st.clipT = lerp(insetT, 0, p); st.clipL = lerp(insetL, 0, p); st.coverS = lerp(coverS0, 1, p);
      st.item.forEach(function (s) { s.innerY = lerp(0, -60, p); });
    });
    add(clipStart + 0.64, 0.5, alpha, function (p) { st.item.forEach(function (s) { s.a = 1 - p; }); });
    add(clipStart + 0.6, 0.3, alpha, function (p) { st.counterA = 1 - p; });

    /* initial state */
    st.clipT = insetT; st.clipL = insetL; st.coverS = coverS0; st.coverY = coverY0;
    st.item.forEach(function (s) { s.y = H; });
    st.sunY = 30; st.caneY = 12;
  }

  function render() {
    if (sunEl) sunEl.style.transform = 'translate3d(0,' + st.sunY + '%,0)';
    if (caneEl) caneEl.style.transform = 'translate3d(0,' + st.caneY + '%,0)';
    counter.style.transform = 'translate3d(0,' + st.counterY + 'px,0)';
    counter.style.opacity = st.counterA;
    countEl.textContent = st.count < 10 ? '0' + st.count : String(st.count);
    items.forEach(function (it, i) {
      var s = st.item[i];
      it.style.transform = 'translate3d(0,' + s.y + 'px,0)';
      it.style.opacity = s.a;
      inners[i].style.transform = 'translate3d(0,' + s.innerY + '%,0)';
      images[i].style.transform = 'translate3d(0,' + s.imgY + '%,0) scale(1.2)';
    });
    var clip = 'inset(' + st.clipT + 'px ' + st.clipL + 'px)';
    cover.style.clipPath = clip; cover.style.webkitClipPath = clip;
    cover.style.transform = 'translate3d(0,' + st.coverY + 'px,0)';
    coverImg.style.transform = 'translate3d(0,' + st.coverImgY + '%,0) scale(' + st.coverS + ')';
  }

  var t0 = 0, running = false;
  function frame(now) {
    if (finished) return;
    var t = (now - t0) / 1000 / SLOW_DOWN;
    for (var i = 0; i < tweens.length; i++) {
      var tw = tweens[i];
      if (t < tw.s) continue;                       /* not started: keep initial state */
      tw.f(tw.e(Math.min(1, (t - tw.s) / tw.d)));
    }
    render();
    if (t < END) requestAnimationFrame(frame); else reveal();
  }

  /* replay the hero heading animation once the page is uncovered */
  function reveal() {
    var cap = document.querySelector('.hero-swiper .swiper-slide-active .hero-slide__caption');
    var anim = window.GBLTextAnim;
    if (cap && anim && anim.hide && anim.playIn) {
      try { anim.hide(cap); } catch (e) {}
      setTimeout(function () {
        try { anim.playIn(cap, {duration: 1.05, stagger: 0.1, ease: 'power4.out'}); } catch (e) {}
      }, 150);
    }
    finish();
  }

  function start() {
    if (running || finished) return; running = true;
    build(); render();
    t0 = performance.now();
    requestAnimationFrame(frame);
  }

  /* hold on "00" until the loader images are ready (max 2.5s), then play */
  build(); render();
  var pending = imgs.length;
  function one() { if (--pending <= 0) start(); }
  imgs.forEach(function (im) {
    if (im.complete && im.naturalWidth) one();
    else { im.addEventListener('load', one); im.addEventListener('error', one); }
  });
  if (!imgs.length) start();
  setTimeout(start, 2500);
  setTimeout(finish, 12000);                          /* safety: never block the site */

  var rt;
  addEventListener('resize', function () {
    clearTimeout(rt);
    rt = setTimeout(function () { if (!running && !finished) { build(); render(); } }, 120);
  });
})();
