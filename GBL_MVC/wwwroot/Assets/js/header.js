document.addEventListener("DOMContentLoaded", function () {
  const header = document.querySelector(".site-header");
  if (!header) return;

  const MQ = window.matchMedia("(max-width: 1199px)");
  const isMobile = () => MQ.matches;

  const backdrop = header.querySelector("[data-header-backdrop]");
  const toggleBtn = header.querySelector("[data-nav-toggle]");
  const searchPanel = document.getElementById("searchOverlay");
  const searchToggle = header.querySelector("[data-search-toggle]");
  const searchInput = searchPanel ? searchPanel.querySelector("#headerSearchInput") : null;
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

  const markUtilityCurrent = () => {
    const path = (location.pathname || "/").replace(/\/+$/, "").toLowerCase() || "/";
    const hash = (location.hash || "").toLowerCase();
    header.querySelectorAll(".site-header__media-item").forEach((item) => {
      let any = false;
      item.querySelectorAll(".site-header__media-dd a").forEach((link) => {
        let url;
        try { url = new URL(link.getAttribute("href") || "", location.origin); }
        catch (error) { return; }
        const linkPath = (url.pathname || "/").replace(/\/+$/, "").toLowerCase() || "/";
        const linkHash = (url.hash || "").toLowerCase();
        const active = linkPath === path && linkHash === hash;
        link.classList.toggle("is-active", active);
        if (active) link.setAttribute("aria-current", "page");
        else link.removeAttribute("aria-current");
        if (active) any = true;
      });
      item.classList.toggle("is-current", any);
    });
  };

  markUtilityCurrent();
  window.addEventListener("hashchange", markUtilityCurrent);

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
    fitGridToL3(panel);
  };

  // Hover fill (same as the corporate governance doc cards): circle grows from where the mouse enters
  header.addEventListener("mouseover", (e) => {
    const item = e.target.closest && e.target.closest(".mega-l2__item, .mega-l3-item > .mega-l3, .site-header__media-dd a");
    if (!item || item.contains(e.relatedTarget)) return;
    const rect = item.getBoundingClientRect();
    const x = Math.min(Math.max(e.clientX - rect.left, 0), rect.width);
    const y = Math.min(Math.max(e.clientY - rect.top, 0), rect.height);
    const radius = Math.ceil(Math.hypot(Math.max(x, rect.width - x), Math.max(y, rect.height - y))) + 16;
    item.style.setProperty("--hx", x + "px");
    item.style.setProperty("--hy", y + "px");
    item.style.setProperty("--hr", radius + "px");
  });

  // The L3 panel is absolutely positioned, so grow the grid to fit it (keeps bottom space equal to top)
  const fitGridToL3 = (panel) => {
    const grid = panel.closest(".mega-menu__grid");
    if (!grid) return;
    grid.style.minHeight = "";
    if (isMobile() || !panel.classList.contains("is-active")) return;
    const cs = getComputedStyle(grid);
    const padY = (parseFloat(cs.paddingTop) || 0) + (parseFloat(cs.paddingBottom) || 0);
    grid.style.minHeight = panel.scrollHeight + padY + "px";
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

  const syncScrollTriggers = () => {
    if (typeof ScrollTrigger === "undefined") return;
    requestAnimationFrame(() => {
      ScrollTrigger.update();
      ScrollTrigger.refresh();
    });
  };

  const lockPage = (lock) => {
    const root = document.documentElement;
    const body = document.body;
    const already = root.classList.contains("is-header-locked");
    const freezeLayout = isMobile();

    if (lock) {
      if (!already) {
        const gap = getScrollbarWidth();
        root.style.setProperty("--header-scrollbar-w", gap + "px");
        root.classList.add("is-header-locked");
        if (freezeLayout) {
          root.classList.add("is-header-lock-scroll");
          body.style.overflow = "hidden";
          body.style.paddingRight = gap + "px";
          header.style.paddingRight = gap + "px";
        }
      }
      if (window.lenis && typeof window.lenis.stop === "function") window.lenis.stop();
      if (typeof ScrollTrigger !== "undefined") ScrollTrigger.update();
      return;
    }

    root.classList.remove("is-header-locked", "is-header-lock-scroll");
    root.style.removeProperty("--header-scrollbar-w");
    body.style.overflow = "";
    body.style.paddingRight = "";
    header.style.paddingRight = "";
    if (window.lenis && typeof window.lenis.start === "function") window.lenis.start();
    syncScrollTriggers();
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

    // Pill behind the open item: wraps the label, ignoring the link's tall hover-bridge padding
    const cs = getComputedStyle(trigger);
    const padL = parseFloat(cs.paddingLeft) || 0;
    const padR = parseFloat(cs.paddingRight) || 0;
    const padT = parseFloat(cs.paddingTop) || 0;
    const lineH = parseFloat(cs.fontSize) || 16;
    // Compact (scrolled) bar is shorter, so keep the pill inside it
    const compact = header.classList.contains("is-compact");
    const padX = compact ? 12 : 14;
    const padY = compact ? 5 : 9;
    const pillLeft = trigRect.left + padL - wrapRect.left - padX;
    const pillWidth = trigRect.width - padL - padR + padX * 2;
    const pillTop = trigRect.top + padT - wrapRect.top - padY;
    navWrap.style.setProperty("--mega-pill-left", pillLeft + "px");
    navWrap.style.setProperty("--mega-pill-width", pillWidth + "px");
    navWrap.style.setProperty("--mega-pill-top", pillTop + "px");
    navWrap.style.setProperty("--mega-pill-height", lineH + padY * 2 + "px");
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
    const maxWidth = menu.classList.contains("mega-menu--list") ? 900 : 1000;
    const width = Math.min(maxWidth, Math.max(320, window.innerWidth - 40));
    const center = trigRect.left + trigRect.width / 2 - wrapRect.left;
    // Keep the right edge inside the nav; let a wide panel extend left, but not off-screen
    const maxLeft = wrapRect.width - width;
    const minLeft = Math.max(20 - wrapRect.left, Math.min(0, maxLeft));
    const left = Math.min(maxLeft, Math.max(minLeft, center - width / 2));
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
    const grid = menu.querySelector(".mega-menu__grid");
    if (grid) grid.style.minHeight = "";
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

  const closeMega = (instant) => {
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

    if (isMobile() || instant) {
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
    const isListMenu = menu.classList.contains("mega-menu--list");
    // List-style menu opens with the feature showing; sub-menus appear on hover.
    // On mobile, inner accordions start closed too (opened by tapping).
    if (isListMenu || isMobile()) {
      closeAllL2(menu);
    } else {
      const preferred = menu.querySelector("[data-l2][data-panel]");
      if (preferred) activateL2(preferred);
    }
    if (!isMobile()) lockPage(true);
    requestAnimationFrame(() => {
      requestAnimationFrame(() => positionMega(trigger, !switching));
    });
  };

  const closeUtilityMenus = () => {
    header.querySelectorAll(".site-header__media-item.is-open").forEach((el) => {
      el.classList.remove("is-open");
      el.querySelector(":scope > a")?.setAttribute("aria-expanded", "false");
    });
  };

  const closeMobileNav = () => {
    header.classList.remove("is-nav-open");
    if (toggleBtn) {
      toggleBtn.setAttribute("aria-expanded", "false");
      toggleBtn.setAttribute("aria-label", "Open menu");
    }
    closeUtilityMenus();
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

  let searchReturnFocus = null;

  // The search panel drops down right under the visible header bar.
  const positionSearch = () => {
    if (!searchPanel) return;
    const bar = header.querySelector(".site-header__bar");
    const bottom = bar ? Math.max(0, Math.round(bar.getBoundingClientRect().bottom)) : 0;
    if (bottom) searchPanel.style.setProperty("--search-top", bottom + "px");
  };

  const setSearchToggleLabel = (open) => {
    if (!searchToggle) return;
    searchToggle.setAttribute("aria-expanded", open ? "true" : "false");
    searchToggle.setAttribute("aria-label", open ? "Close search" : "Open search");
  };

  const closeSearch = () => {
    if (!searchPanel || !header.classList.contains("is-search-open")) return;
    header.classList.remove("is-search-open");
    searchPanel.classList.remove("is-open");
    document.body.classList.remove("is-search-overlay-open");
    setSearchToggleLabel(false);
    window.setTimeout(() => {
      if (!header.classList.contains("is-search-open")) searchPanel.hidden = true;
    }, 320);
    if (!header.classList.contains("is-nav-open") && !header.classList.contains("is-mega-open")) {
      lockPage(false);
    }
    updateTheme();
    if (searchReturnFocus && typeof searchReturnFocus.focus === "function") searchReturnFocus.focus();
    searchReturnFocus = null;
  };

  const openSearch = () => {
    if (!searchPanel) return;
    if (isMobile()) closeMobileNav();
    closeMega();
    searchReturnFocus = document.activeElement;
    searchPanel.hidden = false;
    header.classList.add("is-search-open");
    document.body.classList.add("is-search-overlay-open");
    setSearchToggleLabel(true);
    lockPage(true);
    updateTheme();
    positionSearch();
    if (window.GBLSearchOverlay) window.GBLSearchOverlay.onOpen();
    requestAnimationFrame(() => {
      positionSearch();
      searchPanel.classList.add("is-open");
    });
    // the header bar may animate its height (compact/light); follow it
    window.setTimeout(positionSearch, 450);
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

    if (header.classList.contains("is-nav-open") || header.classList.contains("is-search-open")) {
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
      // Hover already opens the menu. A click on that same item must keep it open.
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

  let utilityTimer = 0;

  const openUtilityMenu = (mediaItem) => {
    clearTimeout(utilityTimer);
    header.querySelectorAll(".site-header__media-item.is-open").forEach((el) => {
      if (el === mediaItem) return;
      el.classList.remove("is-open");
      el.querySelector(":scope > a")?.setAttribute("aria-expanded", "false");
    });
    if (!isMobile()) closeMega(true);
    mediaItem.classList.add("is-open");
    mediaItem.querySelector(":scope > a")?.setAttribute("aria-expanded", "true");
  };

  const scheduleCloseUtility = (mediaItem) => {
    clearTimeout(utilityTimer);
    utilityTimer = window.setTimeout(() => {
      mediaItem.classList.remove("is-open");
      mediaItem.querySelector(":scope > a")?.setAttribute("aria-expanded", "false");
    }, 140);
  };

  header.querySelectorAll(".site-header__media-item").forEach((mediaItem) => {
    const trigger = mediaItem.querySelector(":scope > a");

    mediaItem.addEventListener("mouseenter", () => {
      if (isMobile()) return;
      openUtilityMenu(mediaItem);
    });
    mediaItem.addEventListener("mouseleave", () => {
      if (isMobile()) return;
      scheduleCloseUtility(mediaItem);
    });
    mediaItem.addEventListener("focusin", () => {
      openUtilityMenu(mediaItem);
    });
    mediaItem.addEventListener("focusout", (event) => {
      if (mediaItem.contains(event.relatedTarget)) return;
      scheduleCloseUtility(mediaItem);
    });

    trigger?.addEventListener("click", (event) => {
      if (!isMobile()) return;
      const already = mediaItem.classList.contains("is-open");
      closeUtilityMenus();
      closeMega();
      if (!already) {
        event.preventDefault();
        mediaItem.classList.add("is-open");
        trigger.setAttribute("aria-expanded", "true");
      }
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
      item.addEventListener("click", (event) => {
        if (item.getAttribute("href") === "#") event.preventDefault();
        // Desktop: an item with a submenu that has its own page links there (hover already opens the submenu)
        if (item.hasAttribute("data-panel") && !isMobile() && item.getAttribute("href") !== "#") return;
        if (item.hasAttribute("data-panel")) {
          event.preventDefault();
          // Desktop hover already reveals the submenu. Only mobile tap toggles it shut.
          if (isMobile() && item.classList.contains("is-active")) {
            closeAllL2(item.closest(".mega-menu"));
            return;
          }
        }
        activateL2(item);
      });

      if (menu.classList.contains("mega-menu--list")) {
        item.addEventListener("mouseenter", () => {
          if (isMobile()) return;
          if (item.hasAttribute("data-panel")) activateL2(item);
          else closeAllL2(menu);
        });
      }
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

  if (searchPanel) {
    window.GBLSearchOverlay = initSearchOverlay(searchPanel, searchInput, closeSearch, searchToggle);
    window.addEventListener("resize", () => {
      if (header.classList.contains("is-search-open")) positionSearch();
    });
    // A click on the dimmed page (outside the panel and the header) closes search.
    document.addEventListener("mousedown", (event) => {
      if (!header.classList.contains("is-search-open")) return;
      if (searchPanel.contains(event.target) || header.contains(event.target)) return;
      closeSearch();
    });
    header.querySelector(".site-header__bar")?.addEventListener("transitionend", (event) => {
      if (event.propertyName === "height" && header.classList.contains("is-search-open")) positionSearch();
    });
  }

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

  const blockPageScrollWhileMega = (event) => {
    if (!document.documentElement.classList.contains("is-header-locked")) return;
    if (document.documentElement.classList.contains("is-header-lock-scroll")) return;
    if (event.target && event.target.closest && event.target.closest(".mega-menu")) return;
    event.preventDefault();
  };
  window.addEventListener("wheel", blockPageScrollWhileMega, { passive: false });
  window.addEventListener("touchmove", blockPageScrollWhileMega, { passive: false });
  header.querySelector(".site-header__bar")?.addEventListener("transitionend", (event) => {
    if (event.propertyName === "height") syncMegaPosition();
  });
  updateTheme();
});

/* =========================
   Full-screen search overlay
   (live results from /search/suggest, section chips, recent searches)
========================= */
function initSearchOverlay(panel, input, close, toggle) {
  const suggestUrl = panel.getAttribute("data-suggest-url") || "/search/suggest";
  const resultsBox = panel.querySelector("[data-search-results]");
  const list = panel.querySelector("[data-search-list]");
  const countEl = panel.querySelector("[data-search-count]");
  const emptyEl = panel.querySelector("[data-search-empty]");
  const allLink = panel.querySelector("[data-search-all]");
  const defaultBox = panel.querySelector("[data-search-default]");
  const clearBtn = panel.querySelector("[data-search-clear]");
  const recentCol = panel.querySelector("[data-search-recent-col]");
  const recentList = panel.querySelector("[data-search-recent]");
  const chips = Array.from(panel.querySelectorAll("[data-search-cat]"));
  const form = panel.querySelector("[data-search-form]");

  const RECENT_KEY = "gbl.recentSearches";
  const RECENT_MAX = 5;
  const MIN_CHARS = 2;

  let cat = "All";
  let timer = 0;
  let controller = null;
  let activeIndex = -1;
  let requestId = 0;

  const ESC = { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" };
  const escapeHtml = (s) => String(s == null ? "" : s).replace(/[&<>"']/g, (c) => ESC[c]);
  const escapeRegex = (s) => s.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");

  const highlight = (text, term) => {
    const safe = escapeHtml(text);
    const words = term.split(/\s+/).filter((w) => w.length > 1).map((w) => escapeRegex(escapeHtml(w)));
    if (!words.length) return safe;
    return safe.replace(new RegExp("(" + words.join("|") + ")", "gi"), "<mark>$1</mark>");
  };

  const searchPageUrl = (q) => "/search/" + encodeURIComponent(q);

  /* ---- recent searches (kept in this browser only) ---- */
  const readRecent = () => {
    try {
      const v = JSON.parse(window.localStorage.getItem(RECENT_KEY) || "[]");
      return Array.isArray(v) ? v.filter((x) => typeof x === "string") : [];
    } catch (e) { return []; }
  };
  const writeRecent = (items) => {
    try { window.localStorage.setItem(RECENT_KEY, JSON.stringify(items.slice(0, RECENT_MAX))); } catch (e) { /* storage blocked */ }
  };
  const saveRecent = (q) => {
    const term = (q || "").trim();
    if (term.length < MIN_CHARS) return;
    writeRecent([term].concat(readRecent().filter((x) => x.toLowerCase() !== term.toLowerCase())));
  };

  const renderRecent = () => {
    if (!recentList || !recentCol) return;
    const items = readRecent();
    recentCol.hidden = items.length === 0;
    recentList.innerHTML = items.map((term, i) =>
      '<li>' +
        '<button type="button" class="search-overlay__recent-use" data-recent-use="' + i + '">' +
          '<svg width="15" height="15" viewBox="0 0 24 24" aria-hidden="true"><circle cx="12" cy="12" r="9" fill="none" stroke="currentColor" stroke-width="2"/><path d="M12 7v5l3 2" fill="none" stroke="currentColor" stroke-width="2"/></svg>' +
          escapeHtml(term) +
        '</button>' +
        '<button type="button" class="search-overlay__recent-remove" data-recent-remove="' + i + '" aria-label="Remove ' + escapeHtml(term) + ' from recent searches">' +
          '<svg width="10" height="10" viewBox="0 0 12 12" aria-hidden="true"><path d="M2 2l8 8M10 2l-8 8" stroke="currentColor" stroke-width="1.8" fill="none"/></svg>' +
        '</button>' +
      '</li>'
    ).join("");
  };

  /* ---- results ---- */
  const getItems = () => Array.from(list.querySelectorAll(".search-overlay__result"));

  const setActive = (index) => {
    const items = getItems();
    activeIndex = items.length ? Math.max(-1, Math.min(index, items.length - 1)) : -1;
    items.forEach((el, i) => {
      const on = i === activeIndex;
      el.classList.toggle("is-active", on);
      el.setAttribute("aria-selected", on ? "true" : "false");
      if (on) el.scrollIntoView({ block: "nearest" });
    });
    if (activeIndex >= 0) input.setAttribute("aria-activedescendant", items[activeIndex].id);
    else input.removeAttribute("aria-activedescendant");
  };

  const showDefault = (on) => {
    if (defaultBox) defaultBox.hidden = !on;
    resultsBox.hidden = on;
    input.setAttribute("aria-expanded", on ? "false" : "true");
    if (on) {
      list.innerHTML = "";
      activeIndex = -1;
      input.removeAttribute("aria-activedescendant");
      renderRecent();
    }
  };

  const render = (data, term) => {
    const items = (data && Array.isArray(data.items)) ? data.items : [];
    const total = (data && data.total) || items.length;
    countEl.textContent = total + (total === 1 ? " result" : " results") + (cat === "All" ? "" : " in " + cat);
    list.innerHTML = items.map((r, i) => {
      const isPdf = r.type === "PDF";
      return '<li role="presentation">' +
        '<a href="' + escapeHtml(r.url) + '" class="search-overlay__result" id="searchOverlayOpt' + i + '" role="option" aria-selected="false"' + (isPdf ? ' target="_blank" rel="noopener"' : '') + '>' +
          '<span class="search-overlay__result-main">' +
            '<span class="search-overlay__badge' + (isPdf ? ' search-overlay__badge--pdf' : '') + '">' + escapeHtml(r.type || "Page") + '</span>' +
            '<span class="search-overlay__result-title">' + highlight(r.title, term) + '</span>' +
          '</span>' +
          '<span class="search-overlay__result-path">' + escapeHtml(r.path || "") + '</span>' +
        '</a>' +
      '</li>';
    }).join("");
    emptyEl.hidden = items.length > 0;
    countEl.hidden = items.length === 0;
    allLink.hidden = items.length === 0;
    allLink.href = searchPageUrl(term);
    activeIndex = -1;
    input.removeAttribute("aria-activedescendant");
  };

  const runSearch = () => {
    const term = input.value.trim();
    clearBtn.hidden = input.value.length === 0;
    clearTimeout(timer);
    setActive(-1); // a stale highlight must not be opened by Enter once the text changes
    if (term.length < MIN_CHARS) {
      requestId++;
      if (controller) controller.abort();
      showDefault(true);
      return;
    }
    timer = window.setTimeout(() => {
      if (controller) controller.abort();
      controller = typeof AbortController === "function" ? new AbortController() : null;
      const myId = ++requestId;
      showDefault(false);
      resultsBox.classList.add("is-loading");
      const url = suggestUrl + "?q=" + encodeURIComponent(term) + "&cat=" + encodeURIComponent(cat);
      fetch(url, { headers: { Accept: "application/json" }, signal: controller ? controller.signal : undefined })
        .then((res) => (res.ok ? res.json() : { items: [], total: 0 }))
        .then((data) => { if (myId === requestId) render(data, term); })
        .catch((err) => { if (err && err.name !== "AbortError" && myId === requestId) render({ items: [], total: 0 }, term); })
        .finally(() => { if (myId === requestId) resultsBox.classList.remove("is-loading"); });
    }, 220);
  };

  const setTerm = (term) => {
    input.value = term;
    input.focus();
    runSearch();
  };

  /* ---- events ---- */
  input.addEventListener("input", runSearch);

  input.addEventListener("keydown", (event) => {
    if (resultsBox.hidden) return;
    if (event.key === "ArrowDown") { event.preventDefault(); setActive(activeIndex + 1); }
    else if (event.key === "ArrowUp") { event.preventDefault(); setActive(activeIndex - 1); }
  });

  form.addEventListener("submit", (event) => {
    event.preventDefault();
    const term = input.value.trim();
    const items = getItems();
    if (activeIndex >= 0 && items[activeIndex]) {
      saveRecent(term);
      items[activeIndex].click();
      return;
    }
    if (!term) { input.focus(); return; }
    saveRecent(term);
    window.location.href = searchPageUrl(term);
  });

  clearBtn.addEventListener("click", () => setTerm(""));

  chips.forEach((chip) => {
    chip.addEventListener("click", () => {
      cat = chip.getAttribute("data-search-cat") || "All";
      chips.forEach((c) => {
        const on = c === chip;
        c.classList.toggle("is-active", on);
        c.setAttribute("aria-pressed", on ? "true" : "false");
      });
      runSearch();
    });
  });

  panel.querySelectorAll("[data-search-term]").forEach((btn) => {
    btn.addEventListener("click", () => setTerm((btn.getAttribute("data-search-term") || "").trim()));
  });

  if (recentList) {
    recentList.addEventListener("click", (event) => {
      const use = event.target.closest("[data-recent-use]");
      const remove = event.target.closest("[data-recent-remove]");
      const items = readRecent();
      if (use) setTerm(items[+use.getAttribute("data-recent-use")] || "");
      if (remove) {
        const idx = +remove.getAttribute("data-recent-remove");
        writeRecent(items.filter((_, i) => i !== idx));
        renderRecent();
        input.focus();
      }
    });
  }

  list.addEventListener("click", (event) => {
    if (event.target.closest(".search-overlay__result")) saveRecent(input.value);
  });
  allLink.addEventListener("click", () => saveRecent(input.value));

  panel.querySelectorAll("[data-search-close]").forEach((btn) => btn.addEventListener("click", close));

  // While open, Tab cycles between the header's close button and the panel.
  document.addEventListener("keydown", (event) => {
    if (event.key !== "Tab" || panel.hidden || !panel.classList.contains("is-open")) return;
    const inPanel = Array.from(panel.querySelectorAll('a[href], button:not([disabled]), input, [tabindex]:not([tabindex="-1"])'))
      .filter((el) => el.offsetParent !== null);
    const focusables = (toggle ? [toggle] : []).concat(inPanel);
    if (!focusables.length) return;
    if (!focusables.includes(document.activeElement)) { event.preventDefault(); focusables[event.shiftKey ? focusables.length - 1 : 0].focus(); return; }
    const first = focusables[0];
    const last = focusables[focusables.length - 1];
    if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
    else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
  });

  renderRecent();

  return {
    onOpen: () => {
      renderRecent();
      if (input.value.trim().length < MIN_CHARS) showDefault(true);
    }
  };
}
