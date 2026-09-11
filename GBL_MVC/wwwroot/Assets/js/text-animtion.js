(function () {
  "use strict";

  if (typeof gsap === "undefined") return;

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var SELECTOR = ".hero-slide__caption, .js-text-anim, [data-text-anim]";
  var DURATION_IN = 1.5;
  var DURATION_OUT = 0.55;
  var STAGGER_IN = 0.18;
  var STAGGER_OUT = 0.06;
  var EASE_IN = "power3.out";
  var EASE_OUT = "power3.in";

  function isHeroTarget(el) {
    return !!(el.closest && el.closest(".heroBanner"));
  }

  function getLines(el) {
    if (!el) return [];
    return Array.prototype.slice.call(el.querySelectorAll(".text-anim__line"));
  }

  function wrapSingleLine(el) {
    if (!el || el.querySelector(":scope > .text-anim__mask")) return;

    var mask = document.createElement("span");
    mask.className = "text-anim__mask";
    var inner = document.createElement("span");
    inner.className = "text-anim__line";

    while (el.firstChild) {
      inner.appendChild(el.firstChild);
    }

    mask.appendChild(inner);
    el.appendChild(mask);
  }

  function splitByVisualLines(el) {
    if (!el || el.querySelector(".text-anim__mask")) return;

    var existingLines = el.querySelectorAll(":scope > .hero-slide__line");
    if (existingLines.length) {
      Array.prototype.forEach.call(existingLines, wrapSingleLine);
      return;
    }

    var raw = el.textContent.replace(/\s+/g, " ").trim();
    if (!raw) return;

    var words = raw.split(" ");
    el.textContent = "";

    words.forEach(function (word, index) {
      var wordEl = document.createElement("span");
      wordEl.className = "text-anim__word";
      wordEl.textContent = word;
      el.appendChild(wordEl);
      if (index < words.length - 1) {
        el.appendChild(document.createTextNode(" "));
      }
    });

    var wordEls = Array.prototype.slice.call(el.querySelectorAll(".text-anim__word"));
    var groups = [];
    var currentTop = null;
    var currentGroup = [];

    wordEls.forEach(function (wordEl) {
      var top = wordEl.offsetTop;
      if (currentTop === null) currentTop = top;
      if (Math.abs(top - currentTop) > 2) {
        groups.push(currentGroup);
        currentGroup = [wordEl];
        currentTop = top;
      } else {
        currentGroup.push(wordEl);
      }
    });
    if (currentGroup.length) groups.push(currentGroup);

    el.textContent = "";
    groups.forEach(function (group) {
      var mask = document.createElement("span");
      mask.className = "text-anim__mask";
      var inner = document.createElement("span");
      inner.className = "text-anim__line";
      group.forEach(function (wordEl, index) {
        if (index) inner.appendChild(document.createTextNode(" "));
        inner.appendChild(wordEl);
      });
      mask.appendChild(inner);
      el.appendChild(mask);
    });
  }

  function split(el) {
    if (!el || el.getAttribute("data-text-anim-ready") === "1") return el;
    if (el.querySelector(":scope > .hero-slide__line")) {
      Array.prototype.forEach.call(el.querySelectorAll(":scope > .hero-slide__line"), wrapSingleLine);
    } else {
      splitByVisualLines(el);
    }
    el.setAttribute("data-text-anim-ready", "1");
    return el;
  }

  function playIn(el, vars) {
    var lines = getLines(el);
    if (!lines.length) return null;
    if (reduceMotion || typeof gsap === "undefined") {
      gsap.set(lines, { yPercent: 0, opacity: 1 });
      return null;
    }
    return gsap.fromTo(
      lines,
      { yPercent: 115, opacity: 0 },
      Object.assign(
        {
          yPercent: 0,
          opacity: 1,
          duration: DURATION_IN,
          ease: EASE_IN,
          stagger: STAGGER_IN,
          overwrite: "auto",
        },
        vars || {}
      )
    );
  }

  function playOut(el, vars) {
    var lines = getLines(el);
    if (!lines.length) return null;
    if (reduceMotion || typeof gsap === "undefined") {
      gsap.set(lines, { yPercent: -115, opacity: 0 });
      return null;
    }
    return gsap.to(
      lines,
      Object.assign(
        {
          yPercent: -115,
          opacity: 0,
          duration: DURATION_OUT,
          ease: EASE_OUT,
          stagger: STAGGER_OUT,
          overwrite: "auto",
        },
        vars || {}
      )
    );
  }

  function hide(el) {
    var lines = getLines(el);
    if (!lines.length) return;
    gsap.set(lines, { yPercent: 115, opacity: 0 });
  }

  function show(el) {
    var lines = getLines(el);
    if (!lines.length) return;
    gsap.set(lines, { yPercent: 0, opacity: 1 });
  }

  function collectTargets(root) {
    return Array.prototype.slice.call((root || document).querySelectorAll(SELECTOR));
  }

  function initScrollTitles() {
    var hasScroll = typeof ScrollTrigger !== "undefined";
    if (hasScroll) gsap.registerPlugin(ScrollTrigger);

    collectTargets().forEach(function (el) {
      split(el);
      if (isHeroTarget(el)) return;

      if (reduceMotion) {
        show(el);
        return;
      }

      hide(el);

      if (!hasScroll) {
        playIn(el);
        return;
      }

      var lines = getLines(el);
      if (!lines.length) return;

      var tl = gsap.timeline({ paused: true });
      tl.to(lines, {
        yPercent: 0,
        opacity: 1,
        duration: DURATION_IN,
        ease: EASE_IN,
        stagger: STAGGER_IN,
      });

      ScrollTrigger.create({
        trigger: el,
        start: "top 78%",
        onEnter: function () {
          tl.timeScale(1).play();
        },
        onLeaveBack: function () {
          tl.timeScale(1.15).reverse();
        },
      });
    });
  }

  function boot() {
    collectTargets().forEach(function (el) {
      if (isHeroTarget(el) || el.querySelector(":scope > .hero-slide__line")) {
        split(el);
        if (isHeroTarget(el) && !el.closest(".swiper-slide-active")) hide(el);
      }
    });

    var fontsReady =
      document.fonts && document.fonts.ready ? document.fonts.ready : Promise.resolve();

    fontsReady.then(function () {
      initScrollTitles();
      if (typeof ScrollTrigger !== "undefined") ScrollTrigger.refresh();
    });
  }

  window.GBLTextAnim = {
    split: split,
    playIn: playIn,
    playOut: playOut,
    hide: hide,
    show: show,
    getLines: getLines,
  };

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", boot);
  } else {
    boot();
  }
})();
