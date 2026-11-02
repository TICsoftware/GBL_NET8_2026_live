/* =========================================================
   AWARDS SLIDER
   Markup: [data-awards] > [data-aw-prev], [data-aw-track], [data-aw-next]
   - Native scroll-snap track (smooth touch swipe on mobile)
   - Arrows move one card; wrap around at either end
   - Keyboard <- ->, arrows hidden when all cards fit
   - Cards reveal with a stagger when the slider scrolls into view
========================================================= */
(function () {
  'use strict';

  function initAwards(root) {
    var track = root.querySelector('[data-aw-track]');
    var prev = root.querySelector('[data-aw-prev]');
    var next = root.querySelector('[data-aw-next]');
    if (!track) return;

    var cards = Array.prototype.slice.call(track.children);
    cards.forEach(function (card, i) { card.style.setProperty('--aw-i', i); });

    function step() {
      var card = cards[0];
      if (!card) return track.clientWidth;
      var gap = parseFloat(window.getComputedStyle(track).columnGap) || 0;
      return card.getBoundingClientRect().width + gap;
    }

    function atStart() { return track.scrollLeft <= 2; }
    function atEnd() { return track.scrollLeft + track.clientWidth >= track.scrollWidth - 2; }

    function goNext() {
      if (atEnd()) track.scrollTo({ left: 0, behavior: 'smooth' });
      else track.scrollBy({ left: step(), behavior: 'smooth' });
    }
    function goPrev() {
      if (atStart()) track.scrollTo({ left: track.scrollWidth, behavior: 'smooth' });
      else track.scrollBy({ left: -step(), behavior: 'smooth' });
    }

    if (next) next.addEventListener('click', goNext);
    if (prev) prev.addEventListener('click', goPrev);

    root.addEventListener('keydown', function (e) {
      if (e.key === 'ArrowRight') { e.preventDefault(); goNext(); }
      if (e.key === 'ArrowLeft') { e.preventDefault(); goPrev(); }
    });

    function checkStatic() {
      root.classList.toggle('is-static', track.scrollWidth <= track.clientWidth + 2);
    }
    checkStatic();
    var t = null;
    window.addEventListener('resize', function () {
      clearTimeout(t);
      t = setTimeout(checkStatic, 120);
    });
    window.addEventListener('load', checkStatic);

    // Reveal on scroll
    if ('IntersectionObserver' in window) {
      var io = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
          if (entry.isIntersecting) {
            root.classList.add('is-inview');
            io.disconnect();
          }
        });
      }, { threshold: 0.2 });
      io.observe(root);
    } else {
      root.classList.add('is-inview');
    }
  }

  function init() {
    document.querySelectorAll('[data-awards]').forEach(initAwards);
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();




/* =========================================================
   AWARDS SLIDER
   Markup: [data-awards] > [data-aw-prev], [data-aw-track], [data-aw-next]
   - Native scroll-snap track (smooth touch swipe on mobile)
   - Arrows move one card; wrap around at either end
   - Keyboard <- ->, arrows hidden when all cards fit
   - Cards reveal with a stagger when the slider scrolls into view
========================================================= */
(function () {
  'use strict';

  function initAwards(root) {
    if (root.hasAttribute("data-awards-ready")) return;  // never set up twice
    root.setAttribute("data-awards-ready", "");
    var track = root.querySelector('[data-aw-track]');
    var prev = root.querySelector('[data-aw-prev]');
    var next = root.querySelector('[data-aw-next]');
    if (!track) return;

    var cards = Array.prototype.slice.call(track.children);
    cards.forEach(function (card, i) { card.style.setProperty('--aw-i', i); });

    function step() {
      var card = cards[0];
      if (!card) return track.clientWidth;
      var gap = parseFloat(window.getComputedStyle(track).columnGap) || 0;
      return card.getBoundingClientRect().width + gap;
    }

    function atStart() { return track.scrollLeft <= 2; }
    function atEnd() { return track.scrollLeft + track.clientWidth >= track.scrollWidth - 2; }

    function goNext() {
      if (atEnd()) track.scrollTo({ left: 0, behavior: 'smooth' });
      else track.scrollBy({ left: step(), behavior: 'smooth' });
    }
    function goPrev() {
      if (atStart()) track.scrollTo({ left: track.scrollWidth, behavior: 'smooth' });
      else track.scrollBy({ left: -step(), behavior: 'smooth' });
    }

    if (next) next.addEventListener('click', goNext);
    if (prev) prev.addEventListener('click', goPrev);

    root.addEventListener('keydown', function (e) {
      if (e.key === 'ArrowRight') { e.preventDefault(); goNext(); }
      if (e.key === 'ArrowLeft') { e.preventDefault(); goPrev(); }
    });

    function checkStatic() {
      root.classList.toggle('is-static', track.scrollWidth <= track.clientWidth + 2);
    }
    checkStatic();
    var t = null;
    window.addEventListener('resize', function () {
      clearTimeout(t);
      t = setTimeout(checkStatic, 120);
    });
    window.addEventListener('load', checkStatic);

    // Reveal on scroll
    if ('IntersectionObserver' in window) {
      var io = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
          if (entry.isIntersecting) {
            root.classList.add('is-inview');
            io.disconnect();
          }
        });
      }, { threshold: 0.2 });
      io.observe(root);
    } else {
      root.classList.add('is-inview');
    }
  }

  function init() {
    document.querySelectorAll('[data-awards]').forEach(initAwards);
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();


/* =========================================================
   CERTIFICATIONS
   Markup: [data-certs] > [data-cert] > [data-cert-track] > .cert-item
   - A card with more than 3 items becomes a slider:
     arrows are added next to the title, 3 items show at a time,
     arrows move one item and wrap around, touch swipe works.
   - Cards reveal with a stagger when the grid scrolls into view.
========================================================= */
(function () {
  'use strict';

  var PER_VIEW = 3;

  var ICON_PREV = '<svg viewBox="0 0 16 16" fill="none" aria-hidden="true"><path d="M10 3L5 8l5 5" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>';
  var ICON_NEXT = '<svg viewBox="0 0 16 16" fill="none" aria-hidden="true"><path d="M6 3l5 5-5 5" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>';

  function makeArrow(cls, label, icon) {
    var b = document.createElement('button');
    b.type = 'button';
    b.className = 'cert-card__arrow ' + cls;
    b.setAttribute('aria-label', label);
    b.innerHTML = icon;
    return b;
  }

  function initCard(card) {
    var track = card.querySelector('[data-cert-track]');
    if (!track) return;
    var items = Array.prototype.slice.call(track.children);
    if (items.length <= PER_VIEW) return;   // 1–3 items: plain row

    card.classList.add('is-slider');
    var title = card.querySelector('.cert-card__title');
    card.setAttribute('role', 'region');
    card.setAttribute('aria-roledescription', 'carousel');
    if (title) card.setAttribute('aria-label', title.textContent.trim());

    var nav = document.createElement('div');
    nav.className = 'cert-card__nav';
    var prev = makeArrow('cert-card__arrow--prev', 'Previous certificate', ICON_PREV);
    var next = makeArrow('cert-card__arrow--next', 'Next certificate', ICON_NEXT);
    nav.appendChild(prev);
    nav.appendChild(next);
    (card.querySelector('.cert-card__head') || card).appendChild(nav);

    function step() { return items[0].getBoundingClientRect().width; }
    function atStart() { return track.scrollLeft <= 2; }
    function atEnd() { return track.scrollLeft + track.clientWidth >= track.scrollWidth - 2; }

    next.addEventListener('click', function () {
      if (atEnd()) track.scrollTo({ left: 0, behavior: 'smooth' });
      else track.scrollBy({ left: step(), behavior: 'smooth' });
    });
    prev.addEventListener('click', function () {
      if (atStart()) track.scrollTo({ left: track.scrollWidth, behavior: 'smooth' });
      else track.scrollBy({ left: -step(), behavior: 'smooth' });
    });

    card.addEventListener('keydown', function (e) {
      if (e.key === 'ArrowRight') { e.preventDefault(); next.click(); }
      if (e.key === 'ArrowLeft') { e.preventDefault(); prev.click(); }
    });
  }

  function initGrid(grid) {
    if (grid.hasAttribute("data-certs-ready")) return;   // never set up twice
    grid.setAttribute("data-certs-ready", "");
    var cards = Array.prototype.slice.call(grid.querySelectorAll('[data-cert]'));
    cards.forEach(initCard);

    // Number each card's items so their reveal can be staggered in CSS
    cards.forEach(function (card) {
      var items = card.querySelectorAll('.cert-item');
      Array.prototype.forEach.call(items, function (item, i) {
        item.style.setProperty('--item-i', Math.min(i, 3));
      });
    });

    grid.classList.add('has-reveal');

    if (!('IntersectionObserver' in window)) {
      cards.forEach(function (c) { c.classList.add('is-inview'); });
      return;
    }

    // Reveal each card as it enters the screen. Cards that enter together
    // (one row on desktop) are staggered left to right.
    var io = new IntersectionObserver(function (entries) {
      var entering = entries
        .filter(function (e) { return e.isIntersecting; })
        .sort(function (a, b) {
          return (a.boundingClientRect.top - b.boundingClientRect.top) ||
                 (a.boundingClientRect.left - b.boundingClientRect.left);
        });
      entering.forEach(function (entry, i) {
        entry.target.style.setProperty('--cert-d', (i * 120) + 'ms');
        entry.target.classList.add('is-inview');
        io.unobserve(entry.target);
      });
    }, { threshold: 0.2, rootMargin: '0px 0px -40px 0px' });

    cards.forEach(function (card) { io.observe(card); });
  }

  function init() {
    document.querySelectorAll('[data-certs]').forEach(initGrid);
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();