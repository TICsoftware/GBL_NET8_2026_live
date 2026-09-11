(() => {
  const section = document.querySelector(".ataglance");
  if (!section || typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") {
    return;
  }

  gsap.registerPlugin(ScrollTrigger);

  const items = Array.from(section.querySelectorAll(".ataglance-item"));
  const wrappers = Array.from(section.querySelectorAll(".ataglance-card-wrapper"));
  if (!items.length || !wrappers.length) return;

  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  const mm = gsap.matchMedia();
  let activeIndex = 0;

  const setItemState = (index) => {
    if (index === activeIndex) return;
    activeIndex = index;

    items.forEach((item, i) => {
      const on = i === index;
      item.classList.toggle("is-active", on);
      item.setAttribute("aria-pressed", on ? "true" : "false");
    });

    wrappers.forEach((wrapper, i) => {
      wrapper.querySelector("[data-glance-card]")?.classList.toggle("is-active", i === index);
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

  function getPinTopPx() {
    const title = section.querySelector(".title-section");
    let titleBlock = 0;
    if (title) {
      const cs = getComputedStyle(title);
      titleBlock =
        title.getBoundingClientRect().height + (parseFloat(cs.marginBottom) || 0);
    }

    /* Pin image + list directly under the title */
    const pinTop = Math.round(getHeaderPx() + titleBlock);
    section.style.setProperty("--ataglance-pin-top", `${pinTop}px`);
    return pinTop;
  }

  const scrollToCard = (index) => {
    const target = wrappers[index];
    if (!target) return;

    const pinTop = getPinTopPx();

    if (window.lenis && typeof window.lenis.scrollTo === "function") {
      window.lenis.scrollTo(target, { offset: -pinTop, duration: 1.05 });
      return;
    }

    const top = target.getBoundingClientRect().top + window.pageYOffset - pinTop;
    window.scrollTo({ top, behavior: "smooth" });
  };

  items.forEach((item) => {
    item.addEventListener("click", (event) => {
      event.preventDefault();
      scrollToCard(Number(item.dataset.glanceIndex));
    });
  });

  /**
   * Desktop: CodePen sticky cards
   * pin + pinSpacing:false → cards overlap
   * scrub fade/scale → outgoing card goes opacity 0 / scale 0.6
   * Panel stays CSS-sticky beside the stack
   */
  mm.add("(min-width: 1024px)", () => {
    const triggers = [];
    const title = section.querySelector(".title-section");
    const lastWrapper = wrappers[wrappers.length - 1];
    const pinStart = () => `top top+=${getPinTopPx()}px`;
    const pinEnd = () => `bottom top+=${getPinTopPx()}px`;

    /* Title pins under the header, then leaves with the last card */
    if (title && lastWrapper) {
      triggers.push(
        ScrollTrigger.create({
          trigger: title,
          start: () => `top top+=${getHeaderPx()}px`,
          endTrigger: lastWrapper,
          end: pinEnd,
          pin: true,
          pinSpacing: false,
          anticipatePin: 1,
          invalidateOnRefresh: true,
        })
      );
    }

    wrappers.forEach((wrapper, index) => {
      const card = wrapper.querySelector("[data-glance-card]");
      if (!card) return;

      gsap.set(card, { zIndex: index + 1, force3D: true });

      const isLast = index === wrappers.length - 1;

      // Sync right-hand list while this card is the focused stack layer.
      // Last item stays active until the stack (and panel) leave together.
      triggers.push(
        ScrollTrigger.create({
          trigger: wrapper,
          start: "top 55%",
          end: isLast ? "bottom top" : "bottom 45%",
          onEnter: () => setItemState(index),
          onEnterBack: () => setItemState(index),
        })
      );

      if (reduceMotion) {
        gsap.set(card, { opacity: 1, scale: 1 });
        return;
      }

      const pinConfig = {
        trigger: wrapper,
        start: pinStart,
        end: pinEnd,
        pin: true,
        pinSpacing: false,
        anticipatePin: 1,
        invalidateOnRefresh: true,
      };

      // Last card stays full size beside the list. pinSpacing keeps the next
      // section from covering it until the panel is ready to leave with it.
      if (isLast) {
        gsap.set(card, { opacity: 1, scale: 1 });
        triggers.push(
          ScrollTrigger.create({
            ...pinConfig,
            pinSpacing: true,
          })
        );
        return;
      }

      // Sticky Cards: Fade & Scale Overlap
      const tl = gsap.timeline({
        scrollTrigger: {
          ...pinConfig,
          scrub: true,
        },
      });

      tl.set(card, { opacity: 1, scale: 1 }).to(
        card,
        { opacity: 0, scale: 0.6, ease: "none" },
        0.01
      );

      if (tl.scrollTrigger) triggers.push(tl.scrollTrigger);
    });

    const onRefreshInit = () => {
      getPinTopPx();
    };
    ScrollTrigger.addEventListener("refreshInit", onRefreshInit);
    const onRefresh = () => ScrollTrigger.refresh();
    window.addEventListener("load", onRefresh);
    requestAnimationFrame(onRefresh);

    return () => {
      ScrollTrigger.removeEventListener("refreshInit", onRefreshInit);
      window.removeEventListener("load", onRefresh);
      section.style.removeProperty("--ataglance-pin-top");
      triggers.forEach((t) => t && t.kill());
      wrappers.forEach((wrapper) => {
        const card = wrapper.querySelector("[data-glance-card]");
        if (card) gsap.set(card, { clearProps: "all" });
      });
    };
  });

  // Tablet / mobile: no pin stack — keep list sync only
  mm.add("(max-width: 1023px)", () => {
    const triggers = wrappers.map((wrapper, index) =>
      ScrollTrigger.create({
        trigger: wrapper,
        start: "top 65%",
        end: "bottom 40%",
        onEnter: () => setItemState(index),
        onEnterBack: () => setItemState(index),
      })
    );

    ScrollTrigger.refresh();
    return () => triggers.forEach((t) => t.kill());
  });

  setItemState(0);
})();
