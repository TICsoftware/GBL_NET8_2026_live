document.addEventListener("DOMContentLoaded", () => {
  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  /* ---------------------------------------
     ENQUIRY LION â€” stroke draw â†’ fill
  --------------------------------------- */
  if (!reduceMotion && typeof gsap !== "undefined" && typeof ScrollTrigger !== "undefined") {
    const lionWrap =
      document.querySelector("#enquiry .lion-logo-wrap") ||
      document.querySelector(".lion-logo-wrap");
    const lionSvg =
      lionWrap && (lionWrap.querySelector(".lion-logo-svg") || lionWrap.querySelector("svg"));
    const lionFill = lionSvg && lionSvg.querySelector(".lion-logo-fill");

    if (lionWrap && lionSvg && lionFill) {
      let lionStroke = lionSvg.querySelector(".lion-logo-stroke");
      if (!lionStroke) {
        lionStroke = lionFill.cloneNode();
        lionStroke.removeAttribute("fill");
        lionStroke.removeAttribute("fill-opacity");
        lionStroke.classList.remove("lion-logo-fill");
        lionStroke.classList.add("lion-logo-stroke");
        lionStroke.setAttribute("fill", "none");
        lionStroke.setAttribute("stroke", "rgba(255,255,255,0.55)");
        lionStroke.setAttribute("stroke-width", "1.75");
        lionStroke.setAttribute("stroke-linecap", "round");
        lionStroke.setAttribute("stroke-linejoin", "round");
        lionStroke.setAttribute("vector-effect", "non-scaling-stroke");
        lionSvg.insertBefore(lionStroke, lionFill);
      }

      let pathLen = 0;
      try {
        pathLen = lionStroke.getTotalLength();
      } catch (e) {
        pathLen = 0;
      }

      if (pathLen > 0) {
        gsap.set(lionWrap, {
          xPercent: 0,
          yPercent: 0,
          scale: 0.05,
          transformOrigin: "50% 50%",
          force3D: true,
        });
        gsap.set(lionStroke, {
          strokeDasharray: pathLen,
          strokeDashoffset: pathLen,
          autoAlpha: 1,
        });
        gsap.set(lionFill, { autoAlpha: 0 });

        const trigger = lionWrap.closest("#enquiry") || lionWrap.closest(".bg-brand-band") || lionWrap;

        gsap
          .timeline({
            scrollTrigger: {
              trigger,
              start: "top 75%",
              toggleActions: "play none none reverse",
              invalidateOnRefresh: true,
            },
          })
          .to(
            lionWrap,
            {
              scale: 1,
              duration: 1.35,
              ease: "power2.out",
              force3D: true,
            },
            0
          )
          .to(
            lionStroke,
            {
              strokeDashoffset: 0,
              duration: 1.35,
              ease: "power2.inOut",
            },
            0
          )
          .to(lionFill, { autoAlpha: 1, duration: 0.55, ease: "power2.out" }, "-=0.3")
          .to(lionStroke, { autoAlpha: 0, duration: 0.4, ease: "power1.out" }, "-=0.35");
      }
    }
  }

  /* ---------------------------------------
     FOOTPRINT RING GRAPHIC â€” scale/fade in on scroll,
     then settles into a slow idle breathing pulse
     (pulse itself is a CSS animation on the <img>, see
     .circle-img-idle in map-style.css â€” kept off the wrapper
     so it never fights this scale/opacity tween's transform)
  --------------------------------------- */
  if (!reduceMotion && typeof gsap !== "undefined" && typeof ScrollTrigger !== "undefined") {
    const circleWrap = document.querySelector(".circle-img");

    if (circleWrap) {
      gsap.set(circleWrap, { scale: 0.05, opacity: 0, transformOrigin: "50% 50%", force3D: true });

      gsap.to(circleWrap, {
        scale: 1,
        opacity: 1,
        duration: 1.4,
        ease: "power2.out",
        force3D: true,
        scrollTrigger: {
          trigger: circleWrap,
          start: "top 85%",
          toggleActions: "play none none reverse",
          invalidateOnRefresh: true,
        },
        onComplete: () => circleWrap.classList.add("circle-img-idle"),
        onReverseComplete: () => circleWrap.classList.remove("circle-img-idle"),
      });
    }
  }

  /* ---------------------------------------
     CUSTOMER SERVICE CARDS â€” rise on scroll
     Match whypartner-card feel (progressive fromY + autoAlpha)
  --------------------------------------- */
  if (typeof gsap !== "undefined" && typeof ScrollTrigger !== "undefined") {
    const cardsGrid = document.querySelector(".customerservice-grid");
    const cards = cardsGrid
      ? gsap.utils.toArray(cardsGrid.querySelectorAll(".customerservice-block"))
      : [];

    if (cardsGrid && cards.length) {
      if (reduceMotion) {
        gsap.set(cards, { clearProps: "transform,opacity" });
      } else {
        cardsGrid.classList.add("is-rise-anim");

        function getColCount() {
          const w = window.innerWidth;
          if (w >= 1024) return 6;
          if (w >= 768) return 4;
          if (w >= 640) return 3;
          return 2;
        }

        cards.forEach((card, i) => {
          const col = i % getColCount();
          const fromY = 60 * (i + 1);

          gsap.fromTo(
            card,
            { y: fromY, autoAlpha: 0, force3D: true },
            {
              y: 0,
              autoAlpha: 1,
              ease: "none",
              force3D: true,
              scrollTrigger: {
                trigger: card,
                start: `top ${90 - col * 4}%`,
                end: `top ${58 - col * 3}%`,
                scrub: 1,
                invalidateOnRefresh: true,
              },
            }
          );
        });
      }
    }
  }

/* ---------------------------------------
     CUSTOM SELECTS — enhance native <select>
     Keeps asp-for select for submit / validation / cascade
  --------------------------------------- */
  function enhanceNativeCselect(root, scopeSelector) {
    const select = root.querySelector("select");
    if (!select || root.dataset.cselectReady === "1") return;
    root.dataset.cselectReady = "1";

    select.classList.add("cselect-native");

    const id = "cs-" + (select.id || select.name || Math.random().toString(36).slice(2));

    function getPlaceholder() {
      const first = select.options[0];
      return first && !first.value ? first.textContent.trim() : "Select";
    }

    const ui = document.createElement("div");
    ui.className = "cselect-ui";
    ui.innerHTML = `
      <button type="button" class="f-field cselect-btn" data-empty="true" role="combobox"
              aria-haspopup="listbox" aria-expanded="false" aria-controls="${id}" aria-label="${getPlaceholder()}">
        <span class="cselect-value">${getPlaceholder()}</span>
        <svg class="cselect-chevron" width="14" height="8" viewBox="0 0 14 8" fill="none" aria-hidden="true">
          <path d="M1 1l6 6 6-6" stroke="#fff" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </button>
      <div class="cselect-menu" id="${id}" role="listbox" aria-label="${getPlaceholder()}"></div>`;

    select.insertAdjacentElement("afterend", ui);

    const btn = ui.querySelector(".cselect-btn");
    const menu = ui.querySelector(".cselect-menu");
    const label = ui.querySelector(".cselect-value");

    function syncFromSelect() {
      const empty = !select.value;
      const selected = select.selectedOptions[0];
      label.textContent = empty ? getPlaceholder() : (selected ? selected.textContent.trim() : getPlaceholder());
      btn.dataset.empty = empty ? "true" : "false";
      btn.setAttribute("aria-label", label.textContent);
      menu.querySelectorAll(".cselect-option").forEach((o) => {
        o.setAttribute("aria-selected", String(o.dataset.value === select.value));
      });
    }

    function rebuildMenu() {
      const opts = [...select.options].filter((o) => o.value !== "");
      menu.innerHTML = opts
        .map(
          (o) =>
            `<button type="button" class="cselect-option" role="option" aria-selected="${
              o.value === select.value
            }" data-value="${o.value}">${o.textContent.trim()}</button>`
        )
        .join("");
      syncFromSelect();
    }

    function close() {
      if (!root.classList.contains("is-open")) return;
      root.classList.remove("is-open");
      btn.setAttribute("aria-expanded", "false");
      if (!document.querySelector(".cselect.is-open") && window.lenis && typeof window.lenis.start === "function") window.lenis.start();
      if (typeof gsap !== "undefined") {
        gsap.to(menu, { autoAlpha: 0, y: -6, duration: 0.18, ease: "power2.in" });
      } else {
        menu.style.opacity = "0";
        menu.style.visibility = "hidden";
      }
    }

    function open() {
      document.querySelectorAll(`${scopeSelector} .cselect.is-open`).forEach((el) => {
        if (el !== root && el._close) el._close();
      });
      root.classList.add("is-open");
      btn.setAttribute("aria-expanded", "true");
      if (window.lenis && typeof window.lenis.stop === "function") window.lenis.stop();
      if (typeof gsap !== "undefined") {
        gsap.fromTo(
          menu,
          { autoAlpha: 0, y: -6 },
          { autoAlpha: 1, y: 0, duration: 0.22, ease: "power2.out" }
        );
      } else {
        menu.style.opacity = "1";
        menu.style.visibility = "visible";
      }
    }

    root._close = close;
    root._rebuildCselect = rebuildMenu;
    root._syncCselect = syncFromSelect;

    btn.addEventListener("click", () =>
      root.classList.contains("is-open") ? close() : open()
    );
    menu.addEventListener(
      "wheel",
      (e) => {
        e.preventDefault();
        e.stopPropagation();
        menu.scrollTop += e.deltaY;
      },
      { passive: false }
    );

    menu.addEventListener("click", (e) => {
      const opt = e.target.closest(".cselect-option");
      if (!opt) return;
      select.value = opt.dataset.value;
      select.dispatchEvent(new Event("change", { bubbles: true }));
      syncFromSelect();
      close();
      btn.focus();
    });

    btn.addEventListener("keydown", (e) => {
      if (e.key === "Escape") close();
      if (e.key === "ArrowDown" || e.key === "Enter" || e.key === " ") {
        e.preventDefault();
        open();
        menu.querySelector(".cselect-option")?.focus();
      }
    });

    // Keep UI in sync when options are replaced (State -> City ajax)
    const mo = new MutationObserver(() => rebuildMenu());
    mo.observe(select, { childList: true });

    select.addEventListener("change", syncFromSelect);

    const form = select.closest("form");
    if (form && !form.dataset.cselectResetBound) {
      form.dataset.cselectResetBound = "1";
      form.addEventListener("reset", () => {
        requestAnimationFrame(() => {
          form.querySelectorAll(".cselect").forEach((el) => {
            if (el._rebuildCselect) el._rebuildCselect();
            else if (el._syncCselect) el._syncCselect();
          });
        });
      });
    }

    rebuildMenu();
  }

  document.querySelectorAll("#enquiry .cselect, #contactForm .cselect").forEach((root) => {
    enhanceNativeCselect(root, "#enquiry, #contactForm");
  });

  document.addEventListener("click", (e) => {
    document.querySelectorAll("#enquiry .cselect.is-open, #contactForm .cselect.is-open").forEach((el) => {
      if (!el.contains(e.target) && el._close) el._close();
    });
  });


  
});
