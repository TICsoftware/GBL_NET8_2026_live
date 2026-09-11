document.addEventListener("DOMContentLoaded", (event) => {

  // --------------------------------------------
  // GSAP + ScrollTrigger + Lenis Setup
  // --------------------------------------------
  gsap.registerPlugin(ScrollTrigger);
  
  const isMobile = window.matchMedia("(max-width: 992px)").matches;
  
  /** Same scroll root as Lenis default (wrapper: window ÃƒÂ¢Ã¢â‚¬ Ã¢â‚¬â„¢ classes + scroll on documentElement). */
  const scrollRootEl = document.documentElement;
  
  let lenis;
  
  if (!isMobile) {
  
    lenis = new Lenis({
      smoothWheel: true,
      smoothTouch: false,
  
      // PERFECT NO-LAG SETTINGS
      lerp: 0.05,              // fast response, no delay
      wheelMultiplier: 1.02,   // mouse feels natural
      normalizeWheel: true,
      syncTouch: false,
        prevent: (node) => {
        return node.closest('.testimonial-content')
          || node.closest('#products-section')
          || node.closest('.cselect-menu')
          || node.closest('.cselect');
      }
    });
  
    function raf(time) {
      lenis.raf(time);
      requestAnimationFrame(raf);
    }
    requestAnimationFrame(raf);
  
    window.lenis = lenis;

    // Nested scrollables (cselect dropdown): Lenis steals wheel before bubble handlers.
    // Capture on window, block Lenis, and scroll the menu ourselves.
    window.addEventListener(
      "wheel",
      (e) => {
        const menu = e.target && e.target.closest && e.target.closest(".cselect-menu");
        if (!menu) return;
        e.preventDefault();
        e.stopPropagation();
        if (typeof e.stopImmediatePropagation === "function") e.stopImmediatePropagation();
        menu.scrollTop += e.deltaY;
      },
      { capture: true, passive: false }
    );

  
    // ---- GSAP SYNC ----
    ScrollTrigger.scrollerProxy(scrollRootEl, {
      scrollTop(value) {
        return arguments.length
          ? lenis.scrollTo(value, { immediate: true })
          : lenis.scroll;
      },
      getBoundingClientRect() {
        return {
          top: 0,
          left: 0,
          width: scrollRootEl.clientWidth,
          height: scrollRootEl.clientHeight
        };
      }
    });
  
    // ScrollTriggers must use the same element Lenis proxies ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â otherwise scrub/toggle use native scroll and wonÃƒÂ¢Ã¢â€šÂ¬Ã¢â€žÂ¢t match smooth scroll.
    ScrollTrigger.defaults({ scroller: scrollRootEl });
  
    lenis.on("scroll", ScrollTrigger.update);
    ScrollTrigger.addEventListener("refresh", () => lenis.resize());
    ScrollTrigger.refresh();
  
  } else {
    document.body.classList.add("native-scroll");
    // Mobile: keep true native window/document scroll (no scrollerProxy).
    // Proxying documentElement can interfere with touch scrolling on some mobile browsers.
    ScrollTrigger.defaults({ scroller: window });
    ScrollTrigger.refresh();
  }


  // --------------------------------------------
  // BACK TO TOP + SCROLL PROGRESS RING
  // --------------------------------------------
  (function initBackToTop() {
    const btn = document.querySelector('.back-to-top');
    const circle = document.querySelector('.progress-ring-circle');
    if (!btn || !circle) return;

    const radius = Number(circle.getAttribute('r')) || 45;
    const circumference = 2 * Math.PI * radius;
    const SHOW_AFTER = 420;

    circle.style.strokeDasharray = String(circumference);
    circle.style.strokeDashoffset = String(circumference);

    const getScrollTop = () => {
      if (window.lenis && typeof window.lenis.scroll === 'number') {
        return window.lenis.scroll;
      }
      return window.scrollY || document.documentElement.scrollTop || 0;
    };

    const getScrollPercent = () => {
      const scrollable = Math.max(
        document.documentElement.scrollHeight - window.innerHeight,
        1
      );
      return Math.min(Math.max(getScrollTop() / scrollable, 0), 1);
    };

    const updateScrollUI = () => {
      const percent = getScrollPercent();
      circle.style.strokeDashoffset = String(circumference * (1 - percent));

      const show = getScrollTop() > SHOW_AFTER;
      // If the button currently holds focus and we're about to hide it,
      // move focus away first â€” setting aria-hidden on a focused element
      // is invalid (the browser blocks it and logs a console warning).
      if (!show && document.activeElement === btn) {
        btn.blur();
      }
      btn.classList.toggle('active', show);
      btn.setAttribute('aria-hidden', show ? 'false' : 'true');
      btn.tabIndex = show ? 0 : -1;
    };

    const scrollToTop = (e) => {
      e.preventDefault();
      if (window.lenis && typeof window.lenis.scrollTo === 'function') {
        window.lenis.scrollTo(0, { duration: 1.1 });
      } else {
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    };

    btn.addEventListener('click', scrollToTop);

    if (window.lenis && typeof window.lenis.on === 'function') {
      window.lenis.on('scroll', updateScrollUI);
    }
    window.addEventListener('scroll', updateScrollUI, { passive: true });
    window.addEventListener('resize', updateScrollUI);
    updateScrollUI();
  })();


// --------------------------------------------
// HEADER ANIMATION
// --------------------------------------------

  
// --------------------------------------------
// FOOTER YEAR
// --------------------------------------------
const yearFoot = document.getElementById("year-foot");
if (yearFoot) yearFoot.innerHTML = String(new Date().getFullYear());

// --------------------------------------------
// FOOTER VECTOR â€” smooth scroll reveal
// --------------------------------------------
(function initFooterVector() {
  const vector = document.querySelector('.footer-vector');
  if (!vector || typeof gsap === 'undefined' || typeof ScrollTrigger === 'undefined') return;

  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  if (reduceMotion) {
    gsap.set(vector, { clearProps: 'all', opacity: 0.7, y: 0 });
    return;
  }

  gsap.set(vector, {
    y: 140,
    opacity: 0,
    force3D: true,
  });

  gsap.to(vector, {
    y: 0,
    opacity: 0.7,
    ease: 'none',
    force3D: true,
    overwrite: 'auto',
    scrollTrigger: {
      trigger: 'footer',
      start: 'top 92%',
      end: 'top 45%',
      scrub: 1.1, // soft lag = smoother with Lenis than reverse play/pause
      invalidateOnRefresh: true,
    },
  });

  const refresh = () => ScrollTrigger.refresh();
  if (!vector.complete) {
    vector.addEventListener('load', refresh, { once: true });
  } else {
    refresh();
  }
})();


// --------------------------------------------
// HEADER HEIGHT (sets --header-h from live header)
// --------------------------------------------
(function initHeaderHeight() {
  const header =
    document.querySelector(".site-header") ||
    document.querySelector("#header") ||
    document.querySelector("header");
  if (!header) return;

  const setHeaderH = () => {
    const h = Math.round(header.getBoundingClientRect().height);
    if (h > 0) {
      document.documentElement.style.setProperty("--header-h", h + "px");
    }
  };

  setHeaderH();
  window.addEventListener("resize", setHeaderH);
  if (typeof ResizeObserver !== "undefined") {
    new ResizeObserver(setHeaderH).observe(header);
  }
})();

});