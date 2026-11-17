document.addEventListener("DOMContentLoaded", (event) => {

  // --------------------------------------------
  // GSAP + ScrollTrigger + Lenis Setup
  // --------------------------------------------
  gsap.registerPlugin(ScrollTrigger);
  
  const mobileMq = window.matchMedia("(max-width: 992px)");
  const scrollRootEl = document.documentElement;
  let lenis = null;
  let scrollModeIsMobile = null;
  let stResizeTimer = 0;

  let stRefreshing = false;
  const safeRefresh = () => {
    if (stRefreshing || typeof ScrollTrigger === "undefined") return;
    stRefreshing = true;
    try {
      ScrollTrigger.refresh();
    } catch (err) {}
    stRefreshing = false;
  };

  function enableDesktopScroll() {
    document.body.classList.remove("native-scroll");

    const instance = new Lenis({
      smoothWheel: true,
      smoothTouch: false,
      lerp: 0.05,
      wheelMultiplier: 1.02,
      normalizeWheel: true,
      syncTouch: false,
      prevent: (node) => {
        return node.closest(".testimonial-content")
          || node.closest("#products-section")
          || node.closest(".cselect-menu")
          || node.closest(".cselect")
          || node.closest(".enquiry-phone__list")
          || node.closest(".pf-dropdown__panel-inner")
          || node.closest(".mega-menu")
          || node.closest(".site-header__nav-wrap");
      }
    });
    lenis = instance;
    window.lenis = instance;

    function raf(time) {
      if (window.lenis !== instance) return;
      instance.raf(time);
      requestAnimationFrame(raf);
    }
    requestAnimationFrame(raf);

    ScrollTrigger.scrollerProxy(scrollRootEl, {
      scrollTop(value) {
        if (!lenis) {
          if (arguments.length) window.scrollTo(0, value);
          return window.pageYOffset || document.documentElement.scrollTop || 0;
        }
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

    ScrollTrigger.defaults({ scroller: scrollRootEl });
    instance.on("scroll", ScrollTrigger.update);
  }

  function enableMobileScroll() {
    if (lenis) {
      try {
        lenis.destroy();
      } catch (err) {}
    }
    lenis = null;
    window.lenis = null;
    document.body.classList.add("native-scroll");
    ScrollTrigger.defaults({ scroller: window });
  }

  function applyScrollMode() {
    const mobile = mobileMq.matches;
    if (mobile === scrollModeIsMobile) return;
    scrollModeIsMobile = mobile;
    if (mobile) {
      enableMobileScroll();
    } else {
      enableDesktopScroll();
    }
  }

  applyScrollMode();
  ScrollTrigger.refresh();

  window.addEventListener(
    "wheel",
    (e) => {
      const menu =
        e.target &&
        e.target.closest &&
        (e.target.closest(".cselect-menu") || e.target.closest(".pf-dropdown__panel-inner"));
      if (!menu) return;
      e.preventDefault();
      e.stopPropagation();
      if (typeof e.stopImmediatePropagation === "function") e.stopImmediatePropagation();
      menu.scrollTop += e.deltaY;
    },
    { capture: true, passive: false }
  );

  // Breakpoint crossings while the page is live (no reload) briefly run CSS
  // transitions meant only for user-triggered open/close on fixed, full-screen
  // off-canvas panels (e.g. .filter-inner, .product-filter-sidebar) — suppress
  // those transitions for the duration of the resize so they don't flash open.
  const onViewportResize = () => {
    window.clearTimeout(stResizeTimer);
    stResizeTimer = window.setTimeout(() => {
      applyScrollMode();
      if (window.lenis && typeof window.lenis.resize === "function") {
        window.lenis.resize();
      }
      const bar = document.querySelector(".site-header__bar");
      if (bar) {
        const h = Math.round(bar.getBoundingClientRect().height);
        if (h > 0) document.documentElement.style.setProperty("--header-h", h + "px");
      }
      const banner = document.querySelector(".heroBanner");
      if (banner && !window.matchMedia("(max-width: 767px)").matches) {
        banner.style.height = "";
      }
      safeRefresh();
      window.dispatchEvent(new CustomEvent("gbl:after-resize"));
      window.setTimeout(() => {
        document.documentElement.classList.remove("is-resizing");
      }, 50);
    }, 160);
  };

  window.addEventListener("resize", () => {
    document.documentElement.classList.add("is-resizing");
    onViewportResize();
  });


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
    const bar = header.querySelector(".site-header__bar") || header;
    const h = Math.round(bar.getBoundingClientRect().height);
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