document.addEventListener("DOMContentLoaded", function () {
  var section = document.querySelector(".translatingProducts");
  if (!section || typeof Swiper === "undefined") return;

  var sliderEl = section.querySelector(".translating-swiper");
  var sliderWrap = section.querySelector(".translating-slider");
  if (!sliderEl || !sliderWrap) return;

  var slides = sliderEl.querySelectorAll(".swiper-slide");
  var prevEl = section.querySelector(".translating-nav--prev");
  var nextEl = section.querySelector(".translating-nav--next");
  var showNav = slides.length > 3;
  var expand = sliderWrap.querySelector(".translating-expand");
  var expandImg = expand ? expand.querySelector("img") : null;
  var expandCaption = expand ? expand.querySelector(".translating-expand__caption") : null;
  var expandInner = expand ? expand.querySelector(".translating-expand__inner") : null;
  var desktopQuery = window.matchMedia("(hover: hover) and (min-width: 1024px)");
  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var hasGsap = typeof gsap !== "undefined";
  var navOffset = 0;
  var hoverTimer = 0;
  var closeTimer = 0;
  var timeline = null;
  var activeCard = null;
  var isOpen = false;
  var scrollLocked = false;
  var scrollIdleTimer = 0;
  var lastScrollY = 0;
  var pointerX = 0;
  var pointerY = 0;

  section.classList.toggle("has-nav", showNav);

  var swiper = null;
  var sliderReady = false;
  var cards = Array.from(section.querySelectorAll(".translating-card"));
  var lastVisibleCards = [];
  var sectionInView = false;
  var slideAnimTimer = 0;

  if (hasGsap && !reduceMotion && cards.length) {
    gsap.set(cards, { autoAlpha: 0, y: 140 });
  }

  function visibleCount() {
    if (!swiper) return 1;
    var spv = swiper.params.slidesPerView;
    if (typeof spv !== "number") return 1;
    return Math.max(1, Math.ceil(spv));
  }

  function visibleCards() {
    if (!swiper) return cards.slice(0, 3);
    var start = swiper.activeIndex || 0;
    var end = Math.min(start + visibleCount(), swiper.slides.length);
    var list = [];
    for (var i = start; i < end; i++) {
      var slide = swiper.slides[i];
      var card = slide && slide.querySelector(".translating-card");
      if (card) list.push(card);
    }
    return list;
  }

  function isCardReady(card) {
    return reduceMotion || (card && card.getAttribute("data-hover-ready") === "true");
  }

  function setCardReady(card, ready) {
    if (!card) return;
    if (ready) card.setAttribute("data-hover-ready", "true");
    else card.removeAttribute("data-hover-ready");
    if (isDesktopHover()) card.style.pointerEvents = ready ? "" : "none";
    else card.style.pointerEvents = "";
  }

  function cardFromPointer() {
    var el = document.elementFromPoint(pointerX, pointerY);
    if (!el || !sliderWrap.contains(el)) return null;
    var card = el.closest(".translating-card");
    if (!card || !isCardReady(card)) return null;
    return card;
  }

  function resumePointerHover() {
    if (!isDesktopHover() || scrollLocked || isOpen) return;
    var card = cardFromPointer();
    if (!card) return;
    window.clearTimeout(hoverTimer);
    window.clearTimeout(closeTimer);
    hoverTimer = window.setTimeout(function () {
      if (scrollLocked || !isCardReady(card)) return;
      openExpand(card);
    }, 16);
  }

  function playCardRise(cardList) {
    if (!cardList.length) return;

    if (!hasGsap || reduceMotion) {
      cardList.forEach(function (card) {
        setCardReady(card, true);
      });
      return;
    }

    cardList.forEach(function (card, i) {
      setCardReady(card, false);
      gsap.fromTo(
        card,
        { autoAlpha: 0, y: 140 },
        {
          autoAlpha: 1,
          y: 0,
          duration: 1.15,
          delay: i * 0.28,
          ease: "power2.out",
          overwrite: "auto",
          force3D: true,
          onUpdate: function () {
            if (this.progress() < 0.8 || isCardReady(card)) return;
            setCardReady(card, true);
            resumePointerHover();
          },
          onComplete: function () {
            setCardReady(card, true);
            resumePointerHover();
          },
        }
      );
    });
  }

  function animateIncomingSlides() {
    if (!swiper || !sectionInView) return;

    window.clearTimeout(slideAnimTimer);
    slideAnimTimer = window.setTimeout(function () {
      var now = visibleCards();
      var incoming = now.filter(function (card) {
        return lastVisibleCards.indexOf(card) === -1;
      });
      var outgoing = lastVisibleCards.filter(function (card) {
        return now.indexOf(card) === -1;
      });

      if (hasGsap && outgoing.length) {
        gsap.set(outgoing, { autoAlpha: 0, y: 140 });
      }

      if (incoming.length) playCardRise(incoming);
      lastVisibleCards = now;
    }, 20);
  }

  function createSlider() {
    if (sliderReady) return;
    sliderReady = true;
    sliderWrap.classList.add("is-ready");

    swiper = new Swiper(sliderEl, {
      slidesPerView: 1.12,
      spaceBetween: 16,
      watchOverflow: true,
      watchSlidesProgress: true,
      speed: 650,
      navigation: showNav
        ? {
            prevEl: prevEl,
            nextEl: nextEl,
          }
        : false,
      breakpoints: {
        768: {
          slidesPerView: 2,
          spaceBetween: 20,
        },
        1024: {
          slidesPerView: 3,
          spaceBetween: 24,
        },
      },
    });

    swiper.on("resize", measureHeights);
    swiper.on("slideChange", function () {
      if (isDesktopHover()) closeExpand();
      animateIncomingSlides();
    });

    window.addEventListener("resize", measureHeights);
    measureHeights();
  }

  function playEnter() {
    if (sectionInView) return;
    sectionInView = true;
    createSlider();

    window.requestAnimationFrame(function () {
      if (!sectionInView) return;
      var firstCards = visibleCards();
      var hiddenCards = cards.filter(function (card) {
        return firstCards.indexOf(card) === -1;
      });
      if (hasGsap && hiddenCards.length) {
        gsap.set(hiddenCards, { autoAlpha: 0, y: 140 });
        hiddenCards.forEach(function (card) {
          setCardReady(card, false);
        });
      }
      playCardRise(firstCards);
      lastVisibleCards = firstCards.slice();
    });
  }

  function playExit() {
    if (!sectionInView) return;
    sectionInView = false;
    scrollLocked = false;
    window.clearTimeout(scrollIdleTimer);
    cards.forEach(function (card) {
      setCardReady(card, false);
    });
    closeExpand(true);
    if (!hasGsap || reduceMotion) return;

    gsap.to(cards, {
      autoAlpha: 0,
      y: 140,
      duration: 0.45,
      stagger: 0.08,
      ease: "power2.in",
      overwrite: "auto",
      force3D: true,
    });
  }

  function watchViewport() {
    if (reduceMotion) {
      createSlider();
      if (hasGsap) gsap.set(cards, { autoAlpha: 1, y: 0 });
      cards.forEach(function (card) {
        setCardReady(card, true);
      });
      return;
    }

    if (hasGsap && typeof ScrollTrigger !== "undefined") {
      gsap.registerPlugin(ScrollTrigger);

      var isMobile = window.matchMedia("(max-width: 992px)").matches;
      var stScroller = isMobile ? window : document.documentElement;

      ScrollTrigger.create({
        trigger: sliderWrap,
        scroller: stScroller,
        start: "top 88%",
        end: "bottom 12%",
        invalidateOnRefresh: true,
        onEnter: playEnter,
        onEnterBack: playEnter,
        onLeave: playExit,
        onLeaveBack: playExit,
      });

      window.requestAnimationFrame(function () {
        ScrollTrigger.refresh();
      });
      window.addEventListener("load", function () {
        ScrollTrigger.refresh();
      });
      return;
    }

    if (!("IntersectionObserver" in window)) {
      createSlider();
      playEnter();
      return;
    }

    var observer = new IntersectionObserver(
      function (entries) {
        if (!entries[0]) return;
        if (entries[0].isIntersecting) playEnter();
        else playExit();
      },
      {
        root: null,
        threshold: 0.2,
        rootMargin: "0px 0px -8% 0px",
      }
    );

    observer.observe(sliderWrap);
  }

  function isDesktopHover() {
    return desktopQuery.matches;
  }

  function onSectionScroll(payload) {
    if (!sectionInView || !isDesktopHover()) return;

    var y =
      payload && typeof payload.scroll === "number"
        ? payload.scroll
        : window.lenis && typeof window.lenis.scroll === "number"
          ? window.lenis.scroll
          : window.pageYOffset;
    var velocity =
      payload && typeof payload.velocity === "number" ? Math.abs(payload.velocity) : 0;
    var dy = Math.abs(y - lastScrollY);
    lastScrollY = y;

    if (velocity < 0.14 && dy < 1.75) return;

    scrollLocked = true;
    window.clearTimeout(hoverTimer);
    window.clearTimeout(scrollIdleTimer);
    scrollIdleTimer = window.setTimeout(function () {
      scrollLocked = false;
      resumePointerHover();
    }, 50);
  }

  function bindScrollLock() {
    window.addEventListener(
      "pointermove",
      function (event) {
        pointerX = event.clientX;
        pointerY = event.clientY;
      },
      { passive: true }
    );

    function onNativeScroll() {
      onSectionScroll();
    }

    window.addEventListener("scroll", onNativeScroll, { passive: true });
    document.documentElement.addEventListener("scroll", onNativeScroll, { passive: true });

    var lenisBound = false;
    function bindLenis() {
      if (lenisBound || !window.lenis || typeof window.lenis.on !== "function") return lenisBound;
      window.lenis.on("scroll", onSectionScroll);
      lenisBound = true;
      window.removeEventListener("scroll", onNativeScroll);
      document.documentElement.removeEventListener("scroll", onNativeScroll);
      return true;
    }

    if (!bindLenis()) {
      window.setTimeout(bindLenis, 0);
      window.addEventListener("load", bindLenis, { once: true });
    }
  }

  function cardHeight() {
    var card = cards[0];
    return card ? Math.round(card.getBoundingClientRect().height) : 0;
  }

  function largeHeight() {
    return cardHeight();
  }

  function measureHeights() {
    if (isOpen) return;
    var h = cardHeight();
    if (!h) return;
    sliderWrap.style.setProperty("--translating-h", h + "px");
    sliderWrap.style.setProperty("--translating-large-h", largeHeight() + "px");
    if (!hasGsap) sliderWrap.style.height = h + "px";
  }

  function fillExpand(card) {
    if (!expand || !expandImg || !expandInner) return;

    var img = card.querySelector(".translating-card__media img");
    var caption = card.querySelector(".translating-card__caption-inner");
    if (!img || !caption) return;

    expandImg.src = img.currentSrc || img.src;
    expandImg.alt = img.alt || "";
    expandInner.innerHTML = caption.innerHTML;
  }

  function fromRect(card) {
    var wrapRect = sliderWrap.getBoundingClientRect();
    var cardRect = card.getBoundingClientRect();
    return {
      x: cardRect.left - wrapRect.left,
      y: cardRect.top - wrapRect.top,
      w: cardRect.width,
      h: cardRect.height,
    };
  }

  function toRect(from) {
    var wrapRect = sliderWrap.getBoundingClientRect();
    var left = showNav ? navOffset : 0;
    return {
      x: left,
      y: from ? from.y : 0,
      w: wrapRect.width - (showNav ? navOffset * 2 : 0),
      h: from ? from.h : largeHeight(),
    };
  }

  function killTimeline() {
    if (timeline) {
      timeline.kill();
      timeline = null;
    }
  }

  function hideExpand() {
    if (expand) {
      if (hasGsap) {
        gsap.set(expand, {
          autoAlpha: 0,
          visibility: "hidden",
          x: 0,
          y: 0,
          width: 0,
          height: 0,
        });
        if (expandCaption) gsap.set(expandCaption, { opacity: 0 });
        if (expandInner) gsap.set(expandInner, { opacity: 0, y: 14 });
      } else {
        expand.style.opacity = "0";
        expand.style.visibility = "hidden";
      }
      expand.setAttribute("aria-hidden", "true");
    }
    sliderWrap.classList.remove("is-expanded");
    isOpen = false;
    activeCard = null;
    timeline = null;
  }

  function currentExpandRect(fallback) {
    if (!hasGsap || !expand) return fallback;
    var x = gsap.getProperty(expand, "x");
    var y = gsap.getProperty(expand, "y");
    var w = gsap.getProperty(expand, "width");
    var h = gsap.getProperty(expand, "height");
    if (typeof x !== "number" || typeof w !== "number" || w < 2) return fallback;
    return { x: x, y: y, w: w, h: h };
  }

  function openExpand(card) {
    if (!expand || !isDesktopHover() || scrollLocked || !isCardReady(card)) return;

    if (activeCard === card && timeline) {
      isOpen = true;
      sliderWrap.classList.add("is-expanded");
      expand.setAttribute("aria-hidden", "false");
      if (timeline.reversed()) timeline.play();
      return;
    }

    var from = fromRect(card);
    var to = toRect(from);
    var visible = hasGsap && gsap.getProperty(expand, "autoAlpha") > 0.05;
    var start = visible ? currentExpandRect(from) : from;

    fillExpand(card);
    activeCard = card;
    isOpen = true;
    sliderWrap.classList.add("is-expanded");
    expand.setAttribute("aria-hidden", "false");

    if (!hasGsap || reduceMotion) {
      expand.style.visibility = "visible";
      expand.style.opacity = "1";
      expand.style.transform = "translate3d(" + to.x + "px," + to.y + "px,0)";
      expand.style.width = to.w + "px";
      expand.style.height = to.h + "px";
      if (expandCaption) expandCaption.style.opacity = "1";
      if (expandInner) expandInner.style.opacity = "1";
      return;
    }

    killTimeline();

    gsap.set(expand, {
      x: start.x,
      y: start.y,
      width: start.w,
      height: start.h,
      transformOrigin: "0 0",
      visibility: "hidden",
      autoAlpha: 0,
      force3D: true,
    });
    gsap.set(expand, {
      visibility: "visible",
      autoAlpha: 1,
    });

    timeline = gsap.timeline({
      defaults: { ease: "power2.inOut" },
      onReverseComplete: hideExpand,
    });

    timeline.to(
      expand,
      {
        x: to.x,
        y: to.y,
        width: to.w,
        height: to.h,
        duration: 0.7,
        force3D: true,
      },
      0
    );

    if (expandCaption) {
      timeline.fromTo(
        expandCaption,
        { opacity: 0 },
        { opacity: 1, duration: 0.35, ease: "power2.out" },
        0.28
      );
    }
    if (expandInner) {
      timeline.fromTo(
        expandInner,
        { opacity: 0, y: 14 },
        { opacity: 1, y: 0, duration: 0.4, ease: "power2.out" },
        0.3
      );
    }
  }

  function closeExpand(immediate) {
    if (!isOpen || !expand) return;

    if (!hasGsap || reduceMotion || immediate) {
      killTimeline();
      hideExpand();
      return;
    }

    if (timeline) {
      timeline.reverse();
      return;
    }

    var from = activeCard ? fromRect(activeCard) : null;
    timeline = gsap.timeline({
      defaults: { ease: "power2.inOut" },
      onComplete: hideExpand,
    });

    if (expandInner) {
      timeline.to(expandInner, { opacity: 0, y: 10, duration: 0.22, ease: "power2.in" }, 0);
    }
    if (expandCaption) {
      timeline.to(expandCaption, { opacity: 0, duration: 0.22, ease: "power2.in" }, 0);
    }

    if (from) {
      timeline.to(
        expand,
        {
          x: from.x,
          y: from.y,
          width: from.w,
          height: from.h,
          duration: 0.55,
          force3D: true,
        },
        0
      );
    } else {
      timeline.to(expand, { autoAlpha: 0, duration: 0.28, ease: "power2.in" }, 0);
    }
  }

  watchViewport();
  bindScrollLock();

  cards.forEach(function (card) {
    card.addEventListener("mouseenter", function () {
      if (!isDesktopHover() || scrollLocked || !isCardReady(card)) return;
      window.clearTimeout(hoverTimer);
      window.clearTimeout(closeTimer);
      hoverTimer = window.setTimeout(function () {
        if (scrollLocked || !isCardReady(card)) return;
        openExpand(card);
      }, 20);
    });
  });

  sliderWrap.addEventListener("mouseleave", function () {
    if (!isDesktopHover()) return;
    window.clearTimeout(hoverTimer);
    window.clearTimeout(closeTimer);
    closeTimer = window.setTimeout(closeExpand, 60);
  });

  if (showNav && prevEl && nextEl) {
    [prevEl, nextEl].forEach(function (btn) {
      btn.addEventListener("mouseenter", function () {
        window.clearTimeout(hoverTimer);
        closeExpand();
      });
    });
  }

  var popup = section.querySelector(".translating-popup");
  var popupImg = popup ? popup.querySelector(".translating-popup__media img") : null;
  var popupInner = popup ? popup.querySelector(".translating-popup__inner") : null;
  var popupOpen = false;

  function useMobilePopup() {
    return window.matchMedia("(max-width: 1023px)").matches;
  }

  function lockScroll(lock) {
    document.body.style.overflow = lock ? "hidden" : "";
    if (window.lenis) {
      if (lock && typeof window.lenis.stop === "function") window.lenis.stop();
      if (!lock && typeof window.lenis.start === "function") window.lenis.start();
    }
  }

  function openPopup(card) {
    if (!popup || !popupImg || !popupInner) return;

    var img = card.querySelector(".translating-card__media img");
    var caption = card.querySelector(".translating-card__caption-inner");
    if (!img || !caption) return;

    popupImg.src = img.currentSrc || img.src;
    popupImg.alt = img.alt || "";
    popupInner.innerHTML = caption.innerHTML;

    popup.hidden = false;
    popup.setAttribute("aria-hidden", "false");
    popupOpen = true;
    lockScroll(true);

    window.requestAnimationFrame(function () {
      popup.classList.add("is-open");
    });

    var closeBtn = popup.querySelector(".translating-popup__close");
    if (closeBtn) closeBtn.focus();
  }

  function closePopup() {
    if (!popup || !popupOpen) return;

    popup.classList.remove("is-open");
    popup.setAttribute("aria-hidden", "true");
    popupOpen = false;
    lockScroll(false);

    window.setTimeout(function () {
      if (!popupOpen) popup.hidden = true;
    }, 320);
  }

  cards.forEach(function (card) {
    card.addEventListener("click", function (event) {
      if (!useMobilePopup()) return;
      if (event.target.closest(".translating-card__cta")) return;
      event.preventDefault();
      openPopup(card);
    });
  });

  if (popup) {
    popup.querySelectorAll("[data-popup-close]").forEach(function (el) {
      el.addEventListener("click", closePopup);
    });
  }

  document.addEventListener("keydown", function (event) {
    if (event.key === "Escape" && popupOpen) closePopup();
  });

  window.addEventListener("resize", function () {
    if (popupOpen && !useMobilePopup()) closePopup();
  });
});

document.addEventListener("DOMContentLoaded", function () {
  var explore = document.querySelector(".translating-explore");
  if (!explore) return;

  var openSelect = null;

  function closeSelect(wrap) {
    if (!wrap) return;
    wrap.classList.remove("is-open");
    var list = wrap.querySelector(".translating-select__list");
    var trigger = wrap.querySelector(".translating-select__trigger");
    if (list) list.hidden = true;
    if (trigger) trigger.setAttribute("aria-expanded", "false");
    if (openSelect === wrap) openSelect = null;
  }

  function openSelectMenu(wrap) {
    if (openSelect && openSelect !== wrap) closeSelect(openSelect);
    wrap.classList.add("is-open");
    var list = wrap.querySelector(".translating-select__list");
    var trigger = wrap.querySelector(".translating-select__trigger");
    if (list) list.hidden = false;
    if (trigger) trigger.setAttribute("aria-expanded", "true");
    openSelect = wrap;
  }

  explore.querySelectorAll(".translating-select").forEach(function (wrap, index) {
    var native = wrap.querySelector(".translating-select__control");
    if (!native) return;

    var listId = (native.id || "translating-select") + "-list";
    var trigger = document.createElement("button");
    trigger.type = "button";
    trigger.className = "translating-select__trigger";
    trigger.setAttribute("aria-haspopup", "listbox");
    trigger.setAttribute("aria-expanded", "false");
    trigger.setAttribute("aria-controls", listId);

    var valueEl = document.createElement("span");
    valueEl.className = "translating-select__value";
    valueEl.textContent = native.options[native.selectedIndex]
      ? native.options[native.selectedIndex].text
      : "";

    var chevron = document.createElement("span");
    chevron.className = "translating-select__chevron";
    chevron.setAttribute("aria-hidden", "true");

    trigger.appendChild(valueEl);
    trigger.appendChild(chevron);

    var list = document.createElement("ul");
    list.className = "translating-select__list";
    list.id = listId;
    list.setAttribute("role", "listbox");
    list.hidden = true;

    Array.prototype.forEach.call(native.options, function (option, optionIndex) {
      var item = document.createElement("li");
      var btn = document.createElement("button");
      btn.type = "button";
      btn.className = "translating-select__option";
      btn.setAttribute("role", "option");
      btn.setAttribute("data-value", option.value);
      btn.textContent = option.text;
      if (optionIndex === native.selectedIndex) {
        btn.setAttribute("aria-selected", "true");
        btn.classList.add("is-active");
      }
      btn.addEventListener("click", function () {
        native.value = option.value;
        native.dispatchEvent(new Event("change", { bubbles: true }));
        valueEl.textContent = option.text;
        list.querySelectorAll(".translating-select__option").forEach(function (el) {
          el.classList.toggle("is-active", el === btn);
          el.setAttribute("aria-selected", el === btn ? "true" : "false");
        });
        closeSelect(wrap);
        trigger.focus();
      });
      item.appendChild(btn);
      list.appendChild(item);
    });

    native.setAttribute("tabindex", "-1");
    native.setAttribute("aria-hidden", "true");
    wrap.classList.add("is-enhanced");
    wrap.appendChild(trigger);
    wrap.appendChild(list);

    trigger.addEventListener("click", function () {
      if (wrap.classList.contains("is-open")) closeSelect(wrap);
      else openSelectMenu(wrap);
    });
  });

  document.addEventListener("click", function (event) {
    if (!openSelect) return;
    if (!openSelect.contains(event.target)) closeSelect(openSelect);
  });

  document.addEventListener("keydown", function (event) {
    if (event.key === "Escape" && openSelect) {
      var trigger = openSelect.querySelector(".translating-select__trigger");
      closeSelect(openSelect);
      if (trigger) trigger.focus();
    }
  });

  var form = explore.querySelector(".translating-explore__filters");
  if (form) {
    form.addEventListener("submit", function (event) {
      event.preventDefault();
    });
  }
});
