document.addEventListener("DOMContentLoaded", function () {
  var tabRoot = document.querySelector(".tab-outer");
  var contentRoot = document.querySelector(".industries-tab-content");
  if (!tabRoot || !contentRoot) return;

  var tabsWrap = tabRoot.querySelector(".industry-tabs");
  var thumb = tabRoot.querySelector(".industry-tabs__thumb");
  var tabs = Array.from(tabRoot.querySelectorAll("[role='tab']"));
  var panels = Array.from(contentRoot.querySelectorAll("[data-tab-panel]"));

  var CARD_RISE = {
    y: 96,
    duration: 0.85,
    stagger: 0.1,
    rowDelay: 0.18,
    scrub: 0.85,
    listStart: "top 92%",
    listEnd: "top 46%"
  };

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var canAnimate =
    typeof gsap !== "undefined" &&
    typeof ScrollTrigger !== "undefined" &&
    !reduceMotion;

  if (canAnimate) gsap.registerPlugin(ScrollTrigger);

  var isMobile = window.matchMedia("(max-width: 992px)").matches;
  var stScroller = isMobile ? window : document.documentElement;
  var productAnims = [];
  var mediaParallax = [];
  var riseRowSeq = 0;

  function columns() {
    if (window.matchMedia("(max-width: 1179px)").matches) return 1;
    if (window.matchMedia("(max-width: 1279px)").matches) return 2;
    return 3;
  }

  function isInView(el) {
    var rect = el.getBoundingClientRect();
    var vh = window.innerHeight || document.documentElement.clientHeight;
    return rect.top < vh * 0.9 && rect.bottom > 40;
  }

  function moveThumb(tab, animate) {
    if (!thumb || !tabsWrap || !tab) return;

    var wrapRect = tabsWrap.getBoundingClientRect();
    var tabRect = tab.getBoundingClientRect();
    var x = tabRect.left - wrapRect.left;
    var width = tabRect.width;

    if (!canAnimate || !animate) {
      if (canAnimate) {
        gsap.set(thumb, { x: x, width: width });
      } else {
        thumb.style.width = width + "px";
        thumb.style.transform = "translateX(" + x + "px)";
      }
      return;
    }

    gsap.to(thumb, {
      x: x,
      width: width,
      duration: 0.45,
      ease: "power3.out",
      overwrite: true
    });
  }

  function killProductAnims() {
    productAnims.forEach(function (item) {
      if (item && item.kill) item.kill();
    });
    productAnims = [];
  }

  function addScrubRow(row) {
    var tl = gsap.timeline({
      defaults: { force3D: true, ease: "power3.out" },
      scrollTrigger: {
        id: "news-card-rise-row-" + (riseRowSeq++),
        trigger: row[0],
        scroller: stScroller,
        start: CARD_RISE.listStart,
        end: CARD_RISE.listEnd,
        scrub: CARD_RISE.scrub,
        invalidateOnRefresh: true
      }
    });

    row.forEach(function (card, i) {
      tl.fromTo(
        card,
        { autoAlpha: 0, y: CARD_RISE.y },
        {
          autoAlpha: 1,
          y: 0,
          duration: CARD_RISE.duration,
          immediateRender: true
        },
        i * CARD_RISE.stagger
      );
    });

    if (tl.scrollTrigger) productAnims.push(tl.scrollTrigger);
    return tl;
  }

  function bindProductRise(panel, replay) {
    killProductAnims();
    if (!canAnimate || !panel) return;

    var grid = panel.querySelector(".industry-product-grid");
    var cards = grid
      ? Array.from(grid.querySelectorAll(".industry-product-card")).filter(function (card) {
          return !card.hidden;
        })
      : [];
    if (!cards.length) return;

    var cols = columns();
    var introRows = [];

    for (var r = 0; r < cards.length; r += cols) {
      var row = cards.slice(r, r + cols);

      if (replay && isInView(row[0])) {
        introRows.push(row);
        continue;
      }

      addScrubRow(row);
    }

    if (!introRows.length) return;

    var introTl = gsap.timeline({
      defaults: { force3D: true, ease: "power3.out" }
    });

    introRows.forEach(function (row, rowIndex) {
      gsap.set(row, { autoAlpha: 0, y: CARD_RISE.y, force3D: true });
      row.forEach(function (card, i) {
        introTl.to(
          card,
          {
            autoAlpha: 1,
            y: 0,
            duration: CARD_RISE.duration
          },
          rowIndex * CARD_RISE.rowDelay + i * CARD_RISE.stagger
        );
      });
    });

    productAnims.push(introTl);
  }

  function refreshTriggers() {
    if (typeof ScrollTrigger !== "undefined") ScrollTrigger.refresh();
  }

  function killMediaParallax() {
    mediaParallax.forEach(function (item) {
      if (item && item.kill) item.kill();
    });
    mediaParallax = [];
  }

  function bindMediaParallax(panel) {
    killMediaParallax();
    if (!canAnimate || !panel) return;

    var wraps = panel.querySelectorAll(".industry-product-card__media");
    wraps.forEach(function (wrap) {
      var img = wrap.querySelector("img");
      if (!img) return;

      var tween = gsap.fromTo(
        img,
        { yPercent: -13 },
        {
          yPercent: 13,
          ease: "none",
          force3D: true,
          scrollTrigger: {
            trigger: wrap,
            scroller: stScroller,
            start: "top bottom",
            end: "bottom top",
            scrub: 0.8,
            invalidateOnRefresh: true
          }
        }
      );

      if (tween.scrollTrigger) mediaParallax.push(tween.scrollTrigger);
    });
  }

  function activeProductPanel() {
    return panels.find(function (panel) {
      return panel.classList.contains("is-active");
    });
  }

  function activateTab(tab, animate) {
    tabs.forEach(function (btn) {
      var on = btn === tab;
      btn.classList.toggle("is-active", on);
      btn.setAttribute("aria-selected", on ? "true" : "false");
    });

    moveThumb(tab, animate);

    var nextPanel = activeProductPanel();
    if (nextPanel && canAnimate) {
      requestAnimationFrame(function () {
        bindProductRise(nextPanel, animate);
      });
    }
  }

  var startTab = tabs.find(function (tab) {
    return tab.classList.contains("is-active");
  }) || tabs[0];

  if (startTab) moveThumb(startTab, false);

  var cardsStarted = false;

  function beginCardMotion() {
    if (cardsStarted) return;
    cardsStarted = true;
    refreshTriggers();
    var panel = activeProductPanel();
    if (!panel) return;
    bindProductRise(panel, true);
    bindMediaParallax(panel);
  }

  if (document.readyState === "complete") beginCardMotion();
  else window.addEventListener("load", beginCardMotion);

  window.addEventListener("resize", function () {
    var active = tabs.find(function (tab) {
      return tab.classList.contains("is-active");
    });
    if (active) moveThumb(active, false);
  });

  function animateIncomingCards(cards) {
    var list = Array.from(cards || []).filter(function (card) {
      return card && card.classList && card.classList.contains("industry-product-card");
    });
    if (!list.length) return;

    if (!canAnimate) {
      list.forEach(function (card) {
        card.style.opacity = "1";
        card.style.visibility = "visible";
        card.style.transform = "none";
      });
      bindCardFill();
      return;
    }

    gsap.killTweensOf(list);
    var cols = columns();
    gsap.set(list, { autoAlpha: 0, y: CARD_RISE.y, force3D: true });

    var introTl = gsap.timeline({
      defaults: { force3D: true, ease: "power3.out" }
    });

    list.forEach(function (card, i) {
      var row = Math.floor(i / cols);
      var col = i % cols;
      introTl.to(
        card,
        {
          autoAlpha: 1,
          y: 0,
          duration: CARD_RISE.duration
        },
        row * CARD_RISE.rowDelay + col * CARD_RISE.stagger
      );
    });

    productAnims.push(introTl);
    introTl.eventCallback("onComplete", function () {
      bindIncomingParallax(list);
    });
    bindCardFill();
  }

  function bindIncomingParallax(cards) {
    if (!canAnimate) return;
    cards.forEach(function (card) {
      var wrap = card.querySelector(".industry-product-card__media");
      var img = wrap && wrap.querySelector("img");
      if (!img) return;

      var tween = gsap.fromTo(
        img,
        { yPercent: -13 },
        {
          yPercent: 13,
          ease: "none",
          force3D: true,
          scrollTrigger: {
            trigger: wrap,
            scroller: stScroller,
            start: "top bottom",
            end: "bottom top",
            scrub: 0.8,
            invalidateOnRefresh: true
          }
        }
      );

      if (tween.scrollTrigger) mediaParallax.push(tween.scrollTrigger);
    });
  }

  function bindAppendedCardRise(cards) {
    var list = Array.from(cards || []).filter(function (card) {
      return card && card.classList && card.classList.contains("industry-product-card") && !card.hidden;
    });
    if (!list.length) return;

    if (!canAnimate) {
      list.forEach(function (card) {
        card.style.opacity = "1";
        card.style.visibility = "visible";
        card.style.transform = "none";
      });
      bindCardFill();
      return;
    }

    gsap.killTweensOf(list);
    var cols = columns();
    for (var r = 0; r < list.length; r += cols) {
      addScrubRow(list.slice(r, r + cols));
    }
    bindIncomingParallax(list);
    bindCardFill();

    if (window.lenis && typeof window.lenis.resize === "function") window.lenis.resize();
    requestAnimationFrame(function () {
      refreshTriggers();
    });
  }

  window.animateNewsCards = animateIncomingCards;
  window.bindNewsCardScroll = bindAppendedCardRise;

  window.refreshNewsCardMotion = function (root) {
    var scope = root || activeProductPanel() || document;
    killProductAnims();
    killMediaParallax();
    animateIncomingCards(scope.querySelectorAll(".industry-product-card"));
  };

  var toolbar = document.querySelector(".news-toolbar");
  if (toolbar) {
    var openBtn = toolbar.querySelector("[data-open-news-filter]");
    var filterForm = toolbar.querySelector("#news-filter");

    function setFilterOpen(open) {
      toolbar.classList.toggle("is-filter-open", open);
      document.body.classList.toggle("is-news-filter-open", open);
      if (openBtn) openBtn.setAttribute("aria-expanded", open ? "true" : "false");
      document.body.style.overflow = open ? "hidden" : "";
      if (window.lenis) {
        if (open && typeof window.lenis.stop === "function") window.lenis.stop();
        if (!open && typeof window.lenis.start === "function") window.lenis.start();
      }
    }

    if (openBtn) {
      openBtn.addEventListener("click", function () {
        setFilterOpen(true);
      });
    }

    toolbar.querySelectorAll("[data-close-news-filter]").forEach(function (el) {
      el.addEventListener("click", function () {
        setFilterOpen(false);
      });
    });

    var clearBtn = toolbar.querySelector("[data-clear-news-filter]");
    if (clearBtn && filterForm) {
      clearBtn.addEventListener("click", function () {
        filterForm.querySelectorAll(".cselect-native").forEach(function (native) {
          native.selectedIndex = 0;
          native.dispatchEvent(new Event("change", { bubbles: true }));
          var wrap = native.closest(".cselect");
          if (!wrap) return;
          wrap.classList.remove("is-open");
          var menu = wrap.querySelector(".cselect-menu");
          var trigger = wrap.querySelector(".cselect-trigger");
          if (menu) menu.hidden = true;
          if (trigger) trigger.setAttribute("aria-expanded", "false");
          var valueEl = wrap.querySelector(".cselect-value");
          if (valueEl && native.options[0]) valueEl.textContent = native.options[0].text;
          wrap.querySelectorAll(".cselect-option").forEach(function (optionEl, index) {
            optionEl.classList.toggle("is-active", index === 0);
            optionEl.setAttribute("aria-selected", index === 0 ? "true" : "false");
          });
        });
      });
    }

    document.addEventListener("keydown", function (event) {
      if (event.key === "Escape") setFilterOpen(false);
    });

    if (filterForm) {
      filterForm.addEventListener("submit", function (event) {
        event.preventDefault();
        setFilterOpen(false);
      });
    }
  }

  function placeFill(card, e) {
    var media = card.querySelector(".industry-product-card__media");
    var cardRect = card.getBoundingClientRect();
    var mediaRect = media ? media.getBoundingClientRect() : null;
    var top = mediaRect ? Math.max(0, mediaRect.bottom - cardRect.top) : 0;
    var fillW = cardRect.width;
    var fillH = Math.max(0, cardRect.height - top);
    var x = e ? e.clientX - cardRect.left : fillW / 2;
    var y = e ? e.clientY - cardRect.top - top : fillH / 2;
    x = Math.min(Math.max(x, 0), fillW);
    y = Math.min(Math.max(y, 0), fillH);
    var radius = Math.ceil(Math.hypot(Math.max(x, fillW - x), Math.max(y, fillH - y))) + 16;
    card.style.setProperty("--fill-top", top + "px");
    card.style.setProperty("--hx", x + "px");
    card.style.setProperty("--hy", y + "px");
    card.style.setProperty("--hr", radius + "px");
  }

  function bindCardFill(root) {
    var scope = root || document;
    scope.querySelectorAll(".news-inside-outer .industry-product-card, .industry-product-card.media-coverage-card, [data-news-grid] .industry-product-card").forEach(function (card) {
      if (card.dataset.fillBound === "1") return;
      card.dataset.fillBound = "1";
      placeFill(card, null);
      card.addEventListener("mouseenter", function (e) {
        placeFill(card, e);
        card.classList.add("is-filled");
      });
      card.addEventListener("mouseleave", function () {
        card.classList.remove("is-filled");
      });
    });
  }

  function refreshCardFill(panel) {
    var root = panel || document;
    bindCardFill(root);
    root.querySelectorAll(".industry-product-card").forEach(function (card) {
      placeFill(card, null);
    });
  }

  window.bindNewsCardFill = bindCardFill;
  window.refreshNewsCardFill = refreshCardFill;

  bindCardFill();
  window.addEventListener("resize", function () {
    refreshCardFill();
  });
});
