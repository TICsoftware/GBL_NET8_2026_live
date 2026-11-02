/* =========================================================
   MILESTONES SLIDER (infinite loop)
   Markup: Views/Shared/_MilestonesSlider.cshtml
   - Starts on the first slide (1939) unless data-ms-start="n" is set
   - Continuous loop: 2022 -> 1939 and 1939 -> 2022, neighbours always peek
   - Arrows, year timeline, click on peeking slide
   - Swipe / drag, keyboard <- ->
   - Optional autoplay: add data-ms-autoplay="5000" (ms) to [data-milestones]
========================================================= */
(function () {
  'use strict';

  // Used to snap the track to its twin position without any visible animation
  var jumpStyle = document.createElement('style');
  jumpStyle.textContent =
    '.milestones.is-jumping, .milestones.is-jumping * { transition: none !important; }';
  document.head.appendChild(jumpStyle);

  function getTransitionMs(el) {
    var cs = window.getComputedStyle(el);
    var toMs = function (v) { return v.indexOf('ms') > -1 ? parseFloat(v) : parseFloat(v) * 1000; };
    var durs = cs.transitionDuration.split(',').map(toMs);
    var dels = cs.transitionDelay.split(',').map(toMs);
    var max = 0;
    durs.forEach(function (d, i) { max = Math.max(max, d + (dels[i] || 0)); });
    return max || 0;
  }

  function makeClone(slide) {
    var c = slide.cloneNode(true);
    c.removeAttribute('data-ms-slide');
    c.setAttribute('data-ms-clone', '');
    c.classList.remove('is-active');
    c.setAttribute('aria-hidden', 'true');
    c.removeAttribute('role');
    c.removeAttribute('aria-roledescription');
    c.removeAttribute('aria-label');
    c.querySelectorAll('[id]').forEach(function (el) { el.removeAttribute('id'); });
    return c;
  }

  function initMilestones(root) {
    var track = root.querySelector('.milestones-track');
    var viewport = root.querySelector('.milestones-viewport');
    var timeline = root.querySelector('.milestones-timeline');
    var slides = Array.prototype.slice.call(root.querySelectorAll('[data-ms-slide]'));
    var years = Array.prototype.slice.call(root.querySelectorAll('[data-ms-year]'));
    var prevBtn = root.querySelector('[data-ms-prev]');
    var nextBtn = root.querySelector('[data-ms-next]');

    if (!track || !slides.length) return;

    // Wrap each year's text in a span so only the label lifts,
    // while the button's dot/line segment stays on the timeline
    years.forEach(function (year) {
      if (year.querySelector('.milestones-year__label')) return;
      var label = document.createElement('span');
      label.className = 'milestones-year__label';
      while (year.firstChild) label.appendChild(year.firstChild);
      year.appendChild(label);
    });

    // Gradient progress line from the first year to the active year
    var yearsList = root.querySelector('.milestones-years');
    var progress = null;
    if (yearsList && years.length) {
      progress = document.createElement('li');
      progress.className = 'milestones-progress';
      progress.setAttribute('aria-hidden', 'true');
      yearsList.appendChild(progress);
      years[0].classList.add('is-start');
    }

    function offsetWithin(el, ancestor) {
      var x = 0, y = 0;
      while (el && el !== ancestor) { x += el.offsetLeft; y += el.offsetTop; el = el.offsetParent; }
      return { x: x, y: y };
    }

    function updateProgress() {
      if (!progress) return;
      var first = years[0].querySelector('.milestones-year__label');
      var current = years[index] && years[index].querySelector('.milestones-year__label');
      if (!first || !current) return;
      var gap = parseFloat(window.getComputedStyle(timeline || root).getPropertyValue('--ms-line-gap')) || 14;
      var a = offsetWithin(first, yearsList);   // offsets ignore transforms, so the lift doesn't matter
      var b = offsetWithin(current, yearsList);
      var x1 = a.x + first.offsetWidth / 2;
      var x2 = b.x + current.offsetWidth / 2;
      progress.style.left = x1 + 'px';
      progress.style.width = Math.max(0, x2 - x1) + 'px';
      progress.style.top = (a.y + first.offsetHeight + gap) + 'px';
      root.classList.toggle('is-at-start', index === 0);
    }

    var count = slides.length;

    // One full set of clones before and after the real slides
    var before = document.createDocumentFragment();
    var after = document.createDocumentFragment();
    slides.forEach(function (slide) {
      before.appendChild(makeClone(slide));
      after.appendChild(makeClone(slide));
    });
    track.insertBefore(before, track.firstChild);
    track.appendChild(after);

    var all = Array.prototype.slice.call(track.children); // clones + real + clones

    var start = parseInt(root.getAttribute('data-ms-start'), 10);
    var index = isNaN(start) ? 0 : Math.max(0, Math.min(count - 1, start)); // real index 0..count-1
    var pos = count + index;                                                 // position in `all`
    var settleTimer = null;

    function mod(n) { return ((n % count) + count) % count; }

    function centerYear(smooth) {
      var year = years[index];
      if (!year || !timeline) return;
      var left = year.offsetLeft - (timeline.clientWidth - year.offsetWidth) / 2;
      timeline.scrollTo({ left: Math.max(0, left), behavior: smooth ? 'smooth' : 'auto' });
    }

    function render(animate) {
      if (!animate) root.classList.add('is-jumping');

      track.style.setProperty('--ms-index', pos);

      all.forEach(function (slide, p) {
        slide.classList.toggle('is-active', p === pos);
      });
      slides.forEach(function (slide, n) {
        slide.setAttribute('aria-hidden', n === index ? 'false' : 'true');
      });
      years.forEach(function (year, n) {
        var active = n === index;
        year.classList.toggle('is-active', active);
        if (active) year.setAttribute('aria-current', 'step');
        else year.removeAttribute('aria-current');
      });

      if (!animate) {
        void track.offsetWidth; // flush styles so the snap happens without transition
        root.classList.remove('is-jumping');
      }

      updateProgress();
    }

    // After sliding onto a clone, jump silently to the matching real slide
    function settle() {
      clearTimeout(settleTimer);
      if (pos < count || pos >= count * 2) {
        pos = count + index;
        render(false);
      }
    }

    function moveTo(newPos) {
      if (newPos < 0 || newPos >= all.length) {
        settle();
        newPos = pos + (newPos < 0 ? -1 : 1);
      }
      pos = newPos;
      index = mod(pos);
      render(true);
      centerYear(true);

      clearTimeout(settleTimer);
      settleTimer = setTimeout(settle, getTransitionMs(track) + 50);
    }

    function step(dir) { moveTo(pos + dir); }

    function goToIndex(i) {
      settle();
      moveTo(count + mod(i));
    }

    // Arrows (never disabled in loop mode)
    if (prevBtn) { prevBtn.disabled = false; prevBtn.addEventListener('click', function () { step(-1); }); }
    if (nextBtn) { nextBtn.disabled = false; nextBtn.addEventListener('click', function () { step(1); }); }

    // Year timeline
    years.forEach(function (year) {
      year.addEventListener('click', function () {
        goToIndex(parseInt(year.getAttribute('data-ms-year'), 10));
      });
    });

    // Click a peeking slide (real or clone) to open it
    var suppressClick = false;
    all.forEach(function (slide, p) {
      slide.addEventListener('click', function () {
        if (suppressClick) { suppressClick = false; return; }
        if (p !== pos) moveTo(p);
      });
    });

    // Swipe / drag
    var startX = null;
    var startY = null;
    viewport.addEventListener('pointerdown', function (e) {
      startX = e.clientX;
      startY = e.clientY;
    });
    viewport.addEventListener('pointerup', function (e) {
      if (startX === null) return;
      var dx = e.clientX - startX;
      var dy = e.clientY - startY;
      startX = null;
      if (Math.abs(dx) > 50 && Math.abs(dx) > Math.abs(dy)) {
        suppressClick = true;
        step(dx < 0 ? 1 : -1);
        setTimeout(function () { suppressClick = false; }, 50);
      }
    });
    viewport.addEventListener('pointercancel', function () { startX = null; });

    // Stop images being dragged as files on desktop
    viewport.addEventListener('dragstart', function (e) { e.preventDefault(); });

    // Keyboard
    root.addEventListener('keydown', function (e) {
      if (e.key === 'ArrowLeft') { e.preventDefault(); step(-1); }
      if (e.key === 'ArrowRight') { e.preventDefault(); step(1); }
    });

    // Optional autoplay: data-ms-autoplay="5000"
    var autoplayMs = parseInt(root.getAttribute('data-ms-autoplay'), 10);
    var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (autoplayMs > 0 && !reduceMotion) {
      var timer = null;
      var play = function () { stop(); timer = setInterval(function () { step(1); }, autoplayMs); };
      var stop = function () { clearInterval(timer); timer = null; };
      root.addEventListener('mouseenter', stop);
      root.addEventListener('mouseleave', play);
      root.addEventListener('focusin', stop);
      root.addEventListener('focusout', play);
      document.addEventListener('visibilitychange', function () { document.hidden ? stop() : play(); });
      play();
    }

    // Equal slide heights on mobile: every text panel matches the tallest one.
    // Breakpoint can be changed with data-ms-equal-below="768" (px).
    var equalBelow = parseInt(root.getAttribute('data-ms-equal-below'), 10) || 768;
    var contents = all
      .map(function (s) { return s.querySelector('.milestone-slide__content'); })
      .filter(Boolean);

    function equalize() {
      contents.forEach(function (c) { c.style.minHeight = ''; });
      if (window.innerWidth >= equalBelow) return;
      var max = 0;
      contents.forEach(function (c) { max = Math.max(max, c.offsetHeight); });
      if (max) contents.forEach(function (c) { c.style.minHeight = max + 'px'; });
    }

    // Initial state
    render(false);
    centerYear(false);
    equalize();

    var resizeTimer = null;
    window.addEventListener('resize', function () {
      clearTimeout(resizeTimer);
      resizeTimer = setTimeout(function () { equalize(); updateProgress(); centerYear(false); }, 120);
    });
    window.addEventListener('load', function () { equalize(); updateProgress(); });
    if (document.fonts && document.fonts.ready) document.fonts.ready.then(function () { equalize(); updateProgress(); });
  }

  function init() {
    document.querySelectorAll('[data-milestones]').forEach(initMilestones);
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();