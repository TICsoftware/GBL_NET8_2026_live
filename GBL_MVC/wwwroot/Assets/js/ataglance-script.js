document.addEventListener("DOMContentLoaded", () => {
  const section = document.querySelector(".ataglance");
  if (!section) return;

  const items = Array.from(section.querySelectorAll(".ataglance-item"));
  const wrappers = Array.from(section.querySelectorAll(".ataglance-card-wrapper"));
  const cards = wrappers.map((wrapper) => wrapper.querySelector("[data-glance-card]"));
  if (!items.length || !wrappers.length || cards.some((card) => !card)) return;

  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  const hasGsap = typeof gsap !== "undefined" && typeof ScrollTrigger !== "undefined";
  if (hasGsap) gsap.registerPlugin(ScrollTrigger);

  const mm = typeof gsap !== "undefined" && gsap.matchMedia ? gsap.matchMedia() : null;
  const PIN_GAP = 20;
  let activeIndex = 0;
  let desktopTrack = null;
  let glanceSwiper = null;

  const getScroller = () =>
    window.matchMedia("(max-width: 992px)").matches ? window : document.documentElement;

  const setItemState = (index) => {
    if (index === activeIndex) return;
    activeIndex = index;

    items.forEach((item, i) => {
      const on = i === index;
      item.classList.toggle("is-active", on);
      item.setAttribute("aria-pressed", on ? "true" : "false");
    });

    cards.forEach((card, i) => {
      card.classList.toggle("is-active", i === index);
    });
  };

  function getHeaderPx() {
    const headerRaw =
      getComputedStyle(document.documentElement).getPropertyValue("--header-h").trim() ||
      "5.5rem";
    const probe = document.createElement("div");
    probe.style.cssText = `position:absolute;visibility:hidden;height:${headerRaw}`;
    document.body.appendChild(probe);
    const headerPx = probe.offsetHeight || 96;
    probe.remove();
    return headerPx;
  }

  function getTitleTopPx() {
    const headerPx = getHeaderPx();
    const minTop = Math.round(headerPx + PIN_GAP);

    const title = section.querySelector(".title-section");
    let titleBlock = 0;
    if (title && window.matchMedia("(min-width: 1081px)").matches) {
      const cs = getComputedStyle(title);
      titleBlock =
        title.getBoundingClientRect().height + (parseFloat(cs.marginBottom) || 0);
    }
    section.style.setProperty("--ataglance-title-h", `${Math.round(titleBlock)}px`);

    const slotH =
      Math.round(parseFloat(getComputedStyle(section).getPropertyValue("--ataglance-slot-h"))) ||
      480;
    const centered = Math.round((window.innerHeight - titleBlock - slotH) / 2);
    const titleTop = Math.max(minTop, centered);

    section.style.setProperty("--ataglance-title-top", `${titleTop}px`);
    section.style.setProperty("--ataglance-pin-top", `${Math.round(titleTop + titleBlock)}px`);
    return titleTop;
  }

  function syncSlotToPanel() {
    const panel = section.querySelector(".ataglance-panel");
    if (!panel) return;
    const h = Math.max(280, Math.round(panel.getBoundingClientRect().height));
    section.style.setProperty("--ataglance-slot-h", `${h}px`);
  }

  function slotHeight() {
    return (
      Math.round(parseFloat(getComputedStyle(section).getPropertyValue("--ataglance-slot-h"))) ||
      480
    );
  }

  const goToSlide = (index) => {
    const safeIndex = Math.max(0, Math.min(cards.length - 1, Number(index) || 0));

    if (glanceSwiper) {
      glanceSwiper.slideTo(safeIndex);
      return;
    }

    const steps = Math.max(cards.length - 1, 1);
    if (desktopTrack) {
      const start = desktopTrack.start;
      const end = desktopTrack.end;
      const y = start + (safeIndex / steps) * (end - start);
      if (window.lenis && typeof window.lenis.scrollTo === "function") {
        window.lenis.scrollTo(y, { duration: 1.05 });
        return;
      }
      window.scrollTo({ top: y, behavior: "smooth" });
      return;
    }

    setItemState(safeIndex);
  };

  items.forEach((item) => {
    item.addEventListener("click", (event) => {
      event.preventDefault();
      goToSlide(Number(item.dataset.glanceIndex));
    });
  });

  function initDesktop() {
    if (!hasGsap) return function () {};

    const triggers = [];
    const stack = section.querySelector("[data-glance-stack]");
    const container = section.querySelector(".container");
    const title = section.querySelector(".title-section");
    const layout = section.querySelector(".ataglance-layout");
    const stScroller = getScroller();

    if (!stack || !container || !title || !layout) return function () {};

    syncSlotToPanel();
    getTitleTopPx();
    section.style.setProperty("--ataglance-slides", String(Math.max(cards.length - 1, 1)));

    const hold = document.createElement("div");
    hold.className = "ataglance-hold";
    container.insertBefore(hold, title);
    hold.appendChild(title);
    hold.appendChild(layout);

    const track = document.createElement("div");
    track.className = "ataglance-track";
    track.setAttribute("aria-hidden", "true");
    container.appendChild(track);

    section.classList.add("is-pinning-desktop");

    const stage = document.createElement("div");
    stage.className = "ataglance-stage";
    stack.parentNode.insertBefore(stage, stack);
    cards.forEach((card) => stage.appendChild(card));

    cards.forEach((card, index) => {
      gsap.set(card, {
        zIndex: index + 1,
        scale: 1,
        yPercent: index === 0 ? 0 : 100,
        opacity: 1,
        force3D: true,
        transformOrigin: "50% 50%",
      });
    });

    const stStart = () => `top ${getTitleTopPx()}px`;
    const stEnd = () => "+=" + Math.max(slotHeight(), 1) * Math.max(cards.length - 1, 1);

    desktopTrack = ScrollTrigger.create({
      trigger: hold,
      scroller: stScroller,
      start: stStart,
      end: stEnd,
      invalidateOnRefresh: true,
      onUpdate: (self) => {
        const steps = Math.max(cards.length - 1, 1);
        const index = Math.min(cards.length - 1, Math.round(self.progress * steps));
        setItemState(index);
      },
    });
    triggers.push(desktopTrack);

    if (!reduceMotion) {
      const tl = gsap.timeline({
        defaults: { ease: "none", force3D: true },
        scrollTrigger: {
          trigger: hold,
          scroller: stScroller,
          start: stStart,
          end: stEnd,
          scrub: 0.55,
          invalidateOnRefresh: true,
        },
      });

      cards.forEach((card, index) => {
        if (index === 0) return;
        tl.fromTo(card, { yPercent: 100 }, { yPercent: 0, immediateRender: true }, index - 1);
        tl.fromTo(
          cards[index - 1],
          { scale: 1 },
          { scale: 0.72, immediateRender: false },
          index - 1
        );
      });

      if (tl.scrollTrigger) triggers.push(tl.scrollTrigger);
    }

    const onRefreshInit = () => {
      syncSlotToPanel();
      getTitleTopPx();
      section.style.setProperty("--ataglance-slides", String(Math.max(cards.length - 1, 1)));
    };
    ScrollTrigger.addEventListener("refreshInit", onRefreshInit);
    const onRefresh = () => ScrollTrigger.refresh();
    window.addEventListener("load", onRefresh);
    window.addEventListener("resize", onRefreshInit);
    requestAnimationFrame(() => {
      onRefreshInit();
      ScrollTrigger.refresh();
    });

    return () => {
      ScrollTrigger.removeEventListener("refreshInit", onRefreshInit);
      window.removeEventListener("load", onRefresh);
      window.removeEventListener("resize", onRefreshInit);
      desktopTrack = null;
      section.classList.remove("is-pinning-desktop");
      section.style.removeProperty("--ataglance-pin-top");
      section.style.removeProperty("--ataglance-slot-h");
      section.style.removeProperty("--ataglance-title-top");
      section.style.removeProperty("--ataglance-slides");
      triggers.forEach((t) => t && t.kill());
      cards.forEach((card, index) => {
        gsap.set(card, { clearProps: "all" });
        wrappers[index].appendChild(card);
      });
      if (stage.parentNode) stage.remove();
      container.insertBefore(title, hold);
      container.insertBefore(layout, hold);
      if (hold.parentNode) hold.remove();
      if (track.parentNode) track.remove();
    };
  }

  function initSlider() {
    const stack = section.querySelector("[data-glance-stack]");
    const media = section.querySelector(".ataglance-media");
    if (!stack || !media || typeof Swiper === "undefined") return function () {};

    section.classList.add("is-glance-slider");
    desktopTrack = null;

    cards.forEach((card, i) => {
      if (hasGsap) gsap.set(card, { clearProps: "all" });
      card.classList.toggle("is-active", i === 0);
    });

    const copies = [];
    wrappers.forEach((wrapper, i) => {
      const label = items[i] ? items[i].textContent.replace(/\s+/g, " ").trim() : "";
      const copy = document.createElement("p");
      copy.className = "ataglance-slide-copy";
      copy.textContent = label;
      wrapper.appendChild(copy);
      copies.push(copy);
    });

    const wrap = document.createElement("div");
    wrap.className = "swiper-wrapper";
    wrappers.forEach((wrapper) => {
      wrapper.classList.add("swiper-slide");
      wrap.appendChild(wrapper);
    });
    stack.classList.add("swiper", "ataglance-swiper");
    stack.appendChild(wrap);

    const controls = document.createElement("div");
    controls.className = "ataglance-controls";
    controls.innerHTML =
      '<button type="button" class="ataglance-nav ataglance-nav--prev" aria-label="Previous slide">' +
      '<span aria-hidden="true">&larr;</span></button>' +
      '<div class="swiper-pagination ataglance-pagination"></div>' +
      '<button type="button" class="ataglance-nav ataglance-nav--next" aria-label="Next slide">' +
      '<span aria-hidden="true">&rarr;</span></button>';
    media.appendChild(controls);

    const panel = section.querySelector(".ataglance-panel");
    const cta = panel ? panel.querySelector(".site-link") : null;
    if (cta) media.appendChild(cta);

    const prevEl = controls.querySelector(".ataglance-nav--prev");
    const nextEl = controls.querySelector(".ataglance-nav--next");
    const pager = controls.querySelector(".ataglance-pagination");

    glanceSwiper = new Swiper(stack, {
      slidesPerView: 1.08,
      spaceBetween: 16,
      speed: reduceMotion ? 0 : 620,
      watchOverflow: true,
      navigation: {
        prevEl: prevEl,
        nextEl: nextEl,
      },
      pagination: {
        el: pager,
        clickable: true,
      },
      breakpoints: {
        640: { slidesPerView: 1.2, spaceBetween: 18 },
        768: { slidesPerView: 1.35, spaceBetween: 20 },
        900: { slidesPerView: 1.5, spaceBetween: 22 },
      },
      on: {
        slideChange: function () {
          setItemState(this.activeIndex);
        },
      },
    });

    activeIndex = -1;
    setItemState(glanceSwiper.activeIndex || 0);
    controls.appendChild(prevEl);
    controls.appendChild(pager);
    controls.appendChild(nextEl);

    return () => {
      if (glanceSwiper && typeof glanceSwiper.destroy === "function") {
        glanceSwiper.destroy(true, true);
      }
      glanceSwiper = null;
      section.classList.remove("is-glance-slider");
      copies.forEach((copy) => copy.remove());
      wrappers.forEach((wrapper) => {
        wrapper.classList.remove("swiper-slide");
        stack.appendChild(wrapper);
      });
      if (wrap.parentNode) wrap.remove();
      stack.classList.remove("swiper", "ataglance-swiper");
      if (controls.parentNode) controls.remove();
      if (cta && panel) panel.appendChild(cta);
    };
  }

  if (mm) {
    mm.add("(min-width: 1081px)", initDesktop);
    mm.add("(max-width: 1080px)", initSlider);
  } else if (window.matchMedia("(max-width: 1080px)").matches) {
    initSlider();
  }

  setItemState(0);
});
