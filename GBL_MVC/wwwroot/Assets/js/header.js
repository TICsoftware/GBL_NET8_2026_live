document.addEventListener("DOMContentLoaded", function () {
  const header = document.querySelector(".site-header");
  if (!header) return;

  const MQ = window.matchMedia("(max-width: 1199px)");
  const isMobile = () => MQ.matches;

  const backdrop = header.querySelector("[data-header-backdrop]");
  const toggleBtn = header.querySelector("[data-nav-toggle]");
  const searchPanel = header.querySelector("#headerSearch");
  const searchToggle = header.querySelector("[data-search-toggle]");
  const searchInput = header.querySelector("#headerSearchInput");
  const triggers = Array.from(header.querySelectorAll("[data-mega-trigger]"));
  const menus = Array.from(header.querySelectorAll(".mega-menu"));
  const navWrap = header.querySelector(".site-header__nav-wrap");
  const headerInner = header.querySelector(".site-header__inner");
  const headerUtility = header.querySelector(".site-header__utility");
  const headerTools = header.querySelector(".site-header__tools");
  const somaiyaLogo = header.querySelector(".site-header__logo--somaiya");

  let openTimer = 0;
  let closeTimer = 0;
  let closeAnimTimer = 0;
  let activeMenu = null;
  const l3Homes = new Map();

  const placeHeaderTools = () => {
    if (!headerTools || !headerUtility || !headerInner) return;
    if (isMobile()) {
      if (somaiyaLogo) headerInner.insertBefore(headerTools, somaiyaLogo);
      else headerInner.appendChild(headerTools);
      return;
    }
    if (headerTools.parentElement !== headerUtility) headerUtility.appendChild(headerTools);
  };

  placeHeaderTools();
  if (typeof MQ.addEventListener === "function") MQ.addEventListener("change", placeHeaderTools);
  else MQ.addListener(placeHeaderTools);

  header.querySelectorAll(".mega-l3-panel").forEach((panel) => {
    l3Homes.set(panel, panel.parentElement);
  });

  const restoreL3Panels = (scope) => {
    const root = scope || header;
    root.querySelectorAll(".mega-l3-panel").forEach((panel) => {
      const home = l3Homes.get(panel);
      if (home && panel.parentElement !== home) home.appendChild(panel);
    });
  };

  const placeL3Panel = (item, panel) => {
    const li = item.closest("li");
    if (li && panel.parentElement !== li) li.appendChild(panel);
  };

  const getScrollY = () => {
    if (window.lenis && typeof window.lenis.scroll === "number") {
      return window.lenis.scroll;
    }
    return window.scrollY || document.documentElement.scrollTop || 0;
  };

  const getScrollbarWidth = () => {
    return Math.max(0, window.innerWidth - document.documentElement.clientWidth);
  };

  const lockPage = (lock) => {
    const root = document.documentElement;
    const body = document.body;
    const already = root.classList.contains("is-header-locked");

    if (lock) {
      if (!already) {
        const gap = getScrollbarWidth();
        root.style.setProperty("--header-scrollbar-w", gap + "px");
        root.classList.add("is-header-locked");
        body.style.overflow = "hidden";
        body.style.paddingRight = gap + "px";
        header.style.paddingRight = gap + "px";
      }
      if (window.lenis && typeof window.lenis.stop === "function") window.lenis.stop();
      return;
    }

    root.classList.remove("is-header-locked");
    root.style.removeProperty("--header-scrollbar-w");
    body.style.overflow = "";
    body.style.paddingRight = "";
    header.style.paddingRight = "";
    if (window.lenis && typeof window.lenis.start === "function") window.lenis.start();
  };

  const setExpanded = (trigger, expanded) => {
    if (trigger) trigger.setAttribute("aria-expanded", expanded ? "true" : "false");
  };

  const positionCaret = (trigger, instant) => {
    if (!trigger || !navWrap || isMobile()) return;
    const wrapRect = navWrap.getBoundingClientRect();
    const trigRect = trigger.getBoundingClientRect();
    const left = trigRect.left + trigRect.width / 2 - wrapRect.left;
    const pad = 16;
    const clamped = Math.min(wrapRect.width - pad, Math.max(pad, left));
    if (instant) navWrap.classList.add("is-caret-instant");
    navWrap.style.setProperty("--mega-caret-left", clamped + "px");
    if (instant) {
      requestAnimationFrame(() => navWrap.classList.remove("is-caret-instant"));
    }
  };

  const positionMega = (trigger, instant) => {
    if (!trigger || !navWrap || isMobile()) return;
    const key = trigger.getAttribute("data-mega-trigger");
    const menu = header.querySelector('.mega-menu[data-mega="' + key + '"]');
    if (!menu) return;

    const wrapRect = navWrap.getBoundingClientRect();
    const trigRect = trigger.getBoundingClientRect();
    const width = Math.min(616, wrapRect.width, Math.max(320, window.innerWidth - 40));
    const center = trigRect.left + trigRect.width / 2 - wrapRect.left;
    const left = Math.max(0, Math.min(center - width / 2, wrapRect.width - width));
    menu.style.width = width + "px";
    menu.style.setProperty("--mega-left", left + "px");
    positionCaret(trigger, instant);
  };

  const syncMegaPosition = () => {
    if (!activeMenu || isMobile()) return;
    const trigger = header.querySelector('[data-mega-trigger][aria-expanded="true"]');
    if (trigger) positionMega(trigger, true);
  };

  const stillInMegaHover = (event) => {
    const next = event.relatedTarget;
    if (next && next.nodeType === 1 && (
      next.closest("[data-nav-item]") ||
      next.closest(".mega-menu") ||
      next.closest(".site-header__nav")
    )) return true;
    if (typeof event.clientX !== "number") return false;
    const el = document.elementFromPoint(event.clientX, event.clientY);
    return !!(el && (
      el.closest("[data-nav-item]") ||
      el.closest(".mega-menu") ||
      el.closest(".site-header__nav")
    ));
  };

  const closeAllL2 = (menu) => {
    menu.querySelectorAll("[data-l2]").forEach((el) => el.classList.remove("is-active"));
    menu.querySelectorAll(".mega-l3-panel").forEach((panel) => {
      panel.classList.remove("is-active");
      panel.hidden = true;
    });
  };

  const activateL2 = (item) => {
    const menu = item.closest(".mega-menu");
    if (!menu) return;
    closeAllL2(menu);
    item.classList.add("is-active");
    const id = item.getAttribute("data-panel");
    if (!id) return;
    const panel = menu.querySelector('.mega-l3-panel[data-panel="' + id + '"]');
    if (!panel) return;
    panel.hidden = false;
    panel.classList.add("is-active");
    placeL3Panel(item, panel);
  };

  const closeMega = () => {
    clearTimeout(openTimer);
    clearTimeout(closeTimer);
    clearTimeout(closeAnimTimer);
    triggers.forEach((trigger) => {
      setExpanded(trigger, false);
      trigger.closest("[data-nav-item]")?.classList.remove("is-open");
    });
    header.classList.remove("is-mega-open", "is-mega-switching");
    activeMenu = null;

    const hideMenus = () => {
      menus.forEach((menu) => {
        menu.hidden = true;
      });
    };

    if (isMobile()) {
      hideMenus();
    } else {
      closeAnimTimer = window.setTimeout(hideMenus, 300);
    }

    if (!header.classList.contains("is-nav-open") && !header.classList.contains("is-search-open")) {
      lockPage(false);
    }
  };

  const openMega = (trigger) => {
    const key = trigger.getAttribute("data-mega-trigger");
    const menu = header.querySelector('.mega-menu[data-mega="' + key + '"]');
    if (!menu) return;

    clearTimeout(closeAnimTimer);
    const switching = !!(activeMenu && activeMenu !== menu);
    header.classList.toggle("is-mega-switching", switching && !isMobile());

    menus.forEach((item) => {
      item.hidden = item !== menu;
    });
    triggers.forEach((item) => {
      const on = item === trigger;
      setExpanded(item, on);
      item.closest("[data-nav-item]")?.classList.toggle("is-open", on);
    });

    menu.hidden = false;
    header.classList.add("is-mega-open");
    activeMenu = menu;
    if (!isMobile()) lockPage(true);
    requestAnimationFrame(() => {
      requestAnimationFrame(() => positionMega(trigger, !switching));
    });
  };

  const closeMobileNav = () => {
    header.classList.remove("is-nav-open");
    if (toggleBtn) {
      toggleBtn.setAttribute("aria-expanded", "false");
      toggleBtn.setAttribute("aria-label", "Open menu");
    }
    closeMega();
    lockPage(header.classList.contains("is-search-open"));
  };

  const openMobileNav = () => {
    closeSearch();
    header.classList.add("is-nav-open");
    if (toggleBtn) {
      toggleBtn.setAttribute("aria-expanded", "true");
      toggleBtn.setAttribute("aria-label", "Close menu");
    }
    lockPage(true);
  };

  const closeSearch = () => {
    if (!searchPanel) return;
    header.classList.remove("is-search-open");
    if (searchToggle) searchToggle.setAttribute("aria-expanded", "false");
    window.setTimeout(() => {
      if (!header.classList.contains("is-search-open")) searchPanel.hidden = true;
    }, 320);
    if (!header.classList.contains("is-nav-open") && !header.classList.contains("is-mega-open")) {
      lockPage(false);
    }
    updateTheme();
  };

  const openSearch = () => {
    if (!searchPanel) return;
    if (isMobile()) closeMobileNav();
    closeMega();
    searchPanel.hidden = false;
    header.classList.add("is-search-open");
    if (searchToggle) searchToggle.setAttribute("aria-expanded", "true");
    lockPage(true);
    updateTheme();
    window.setTimeout(() => searchInput && searchInput.focus(), 80);
  };

  const updateTheme = () => {
    const scrolled = getScrollY() > 24;
    if (!header.classList.contains("is-nav-open")) {
      const wasCompact = header.classList.contains("is-compact");
      header.classList.toggle("is-compact", scrolled);
      if (wasCompact !== scrolled) {
        requestAnimationFrame(syncMegaPosition);
        window.setTimeout(syncMegaPosition, 420);
      }
    }

    if (header.classList.contains("is-nav-open")) {
      header.classList.add("is-light");
      header.classList.remove("is-dark");
      return;
    }
    header.classList.toggle("is-light", scrolled);
    header.classList.toggle("is-dark", !scrolled);
  };

  triggers.forEach((trigger) => {
    const item = trigger.closest("[data-nav-item]");

    trigger.addEventListener("click", (event) => {
      event.preventDefault();
      if (isMobile()) {
        const already = item.classList.contains("is-open");
        closeMega();
        if (!already) openMega(trigger);
        return;
      }
      if (activeMenu && !activeMenu.hidden && item.classList.contains("is-open")) {
        closeMega();
        return;
      }
      openMega(trigger);
    });

    item.addEventListener("mouseenter", () => {
      if (isMobile()) return;
      clearTimeout(closeTimer);
      clearTimeout(openTimer);
      const delay = header.classList.contains("is-mega-open") ? 0 : 80;
      openTimer = window.setTimeout(() => openMega(trigger), delay);
    });

    item.addEventListener("mouseleave", (event) => {
      if (isMobile()) return;
      clearTimeout(openTimer);
      if (stillInMegaHover(event)) return;
      closeTimer = window.setTimeout(closeMega, 220);
    });
  });

  menus.forEach((menu) => {
    menu.addEventListener("mouseenter", () => {
      if (isMobile()) return;
      clearTimeout(closeTimer);
    });
    menu.addEventListener("mouseleave", (event) => {
      if (isMobile()) return;
      if (stillInMegaHover(event)) return;
      closeTimer = window.setTimeout(closeMega, 220);
    });

    menu.querySelectorAll("[data-l2]").forEach((item) => {
      item.addEventListener("mouseenter", () => {
        if (!isMobile()) activateL2(item);
      });
      item.addEventListener("click", (event) => {
        if (item.getAttribute("href") === "#") event.preventDefault();
        if (!item.hasAttribute("data-panel")) return;
        event.preventDefault();
        if (isMobile() && item.classList.contains("is-active")) {
          closeAllL2(item.closest(".mega-menu"));
          return;
        }
        activateL2(item);
      });
    });

    menu.querySelectorAll("[data-l3]").forEach((item) => {
      item.addEventListener("click", (event) => {
        if (item.getAttribute("href") === "#") event.preventDefault();
      });
    });
  });

  header.querySelectorAll("[data-mega-close]").forEach((btn) => {
    btn.addEventListener("click", closeMega);
  });

  if (toggleBtn) {
    toggleBtn.addEventListener("click", () => {
      if (header.classList.contains("is-nav-open")) closeMobileNav();
      else openMobileNav();
      updateTheme();
    });
  }

  if (searchToggle) {
    searchToggle.addEventListener("click", () => {
      if (header.classList.contains("is-search-open")) closeSearch();
      else openSearch();
    });
  }

  header.querySelector("[data-search-close]")?.addEventListener("click", closeSearch);

  header.querySelector(".site-header__search-form")?.addEventListener("submit", (event) => {
    event.preventDefault();
    const q = (searchInput && searchInput.value || "").trim();
    if (!q) {
      searchInput && searchInput.focus();
      return;
    }
    window.location.href = "/search/" + encodeURIComponent(q);
  });

  backdrop?.addEventListener("click", () => {
    closeSearch();
    closeMega();
    if (isMobile()) closeMobileNav();
  });

  document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") return;
    closeSearch();
    closeMega();
    if (isMobile()) closeMobileNav();
    updateTheme();
  });

  header.addEventListener("click", (event) => {
    const link = event.target.closest('a[href="#"]');
    if (link && header.contains(link) && !link.hasAttribute("data-mega-trigger")) {
      if (!link.hasAttribute("data-l2") && !link.hasAttribute("data-l3")) {
        event.preventDefault();
      }
    }
  });

  window.addEventListener("resize", () => {
    placeHeaderTools();
    const activeL2 = header.querySelector(".mega-l2__item.is-active");
    restoreL3Panels();
    if (!isMobile()) {
      header.classList.remove("is-nav-open");
      if (toggleBtn) {
        toggleBtn.setAttribute("aria-expanded", "false");
        toggleBtn.setAttribute("aria-label", "Open menu");
      }
    }
    if (isMobile()) {
      menus.forEach((menu) => {
        menu.style.removeProperty("width");
        menu.style.removeProperty("--mega-left");
      });
    }
    if (activeL2) activateL2(activeL2);
    if (activeMenu && !activeMenu.hidden) {
      const trigger = header.querySelector('[data-mega-trigger][aria-expanded="true"]');
      positionMega(trigger, true);
    }
    updateTheme();
  });

  window.addEventListener("scroll", updateTheme, { passive: true });
  if (window.lenis && typeof window.lenis.on === "function") {
    window.lenis.on("scroll", updateTheme);
  }
  header.querySelector(".site-header__bar")?.addEventListener("transitionend", (event) => {
    if (event.propertyName === "height") syncMegaPosition();
  });
  updateTheme();
});
