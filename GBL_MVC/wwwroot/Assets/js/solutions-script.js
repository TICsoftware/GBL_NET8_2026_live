document.addEventListener("DOMContentLoaded", () => {
  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  if (!reduceMotion && typeof gsap !== "undefined" && typeof ScrollTrigger !== "undefined") {
    if (typeof gsap.registerPlugin === "function") gsap.registerPlugin(ScrollTrigger);

    /* ---------------------------------------
       Lion stroke draw → fill (intro + enquiry)
    --------------------------------------- */
    function initLionDraw(lionWrap, options = {}) {
      if (!lionWrap) return;
      const lionSvg =
        lionWrap.querySelector(".lion-logo-svg") || lionWrap.querySelector("svg");
      const lionFill = lionSvg && lionSvg.querySelector(".lion-logo-fill");
      if (!lionSvg || !lionFill) return;

      const strokeColor = options.stroke || "#C8C8C8";
      const start = options.start || "top 85%";
      const trigger =
        options.trigger ||
        lionWrap.closest("#solutions-enquiry") ||
        lionWrap.closest(".page-intro-outer") ||
        lionWrap;

      let lionStroke = lionSvg.querySelector(".lion-logo-stroke");
      if (!lionStroke) {
        lionStroke = lionFill.cloneNode();
        lionStroke.removeAttribute("fill");
        lionStroke.removeAttribute("fill-opacity");
        lionStroke.classList.remove("lion-logo-fill");
        lionStroke.classList.add("lion-logo-stroke");
        lionStroke.setAttribute("fill", "none");
        lionStroke.setAttribute("stroke", strokeColor);
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
      if (pathLen <= 0) return;

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

      gsap
        .timeline({
          scrollTrigger: {
            trigger,
            start,
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

    initLionDraw(document.querySelector(".page-intro-outer .lion-logo-wrap"), {
      stroke: "#C8C8C8",
      start: "top 85%",
    });
    initLionDraw(document.querySelector("#solutions-enquiry .lion-logo-wrap"), {
      stroke: "rgba(255,255,255,0.55)",
      start: "top 75%",
    });

    /* ---------------------------------------
       Left/right image parallax (same as Careers)
       Needs .parallax-wrap + .parallax-img (height 130% in common-style)
    --------------------------------------- */
    gsap.utils.toArray(".parallax-wrap").forEach((wrap) => {
      if (wrap.classList.contains("enlarge-wrapper")) return;
      if (wrap.closest(".bg-parallax-section")) return;

      const img = wrap.querySelector(".parallax-img");
      if (!img) return;

      gsap.fromTo(
        img,
        { yPercent: -12 },
        {
          yPercent: 12,
          ease: "none",
          force3D: true,
          scrollTrigger: {
            trigger: wrap,
            start: "top bottom",
            end: "bottom top",
            scrub: 0.8,
            invalidateOnRefresh: true,
          },
        }
      );
    });
  }

  /* ---------------------------------------
     Why partner cards — rise on scroll (our-products-cards pattern)
  --------------------------------------- */
  if (typeof gsap !== "undefined" && typeof ScrollTrigger !== "undefined") {
    const whypartnerGrid = document.querySelector(".whypartner-section .whypartner-grid");
    const whypartnerCards = whypartnerGrid
      ? gsap.utils.toArray(whypartnerGrid.querySelectorAll(".whypartner-card"))
      : [];

    if (whypartnerGrid && whypartnerCards.length) {
      if (reduceMotion) {
        gsap.set(whypartnerCards, { clearProps: "transform" });
      } else {
        whypartnerGrid.classList.add("is-rise-anim");

        function getWhypartnerColCount() {
          const w = window.innerWidth;
          if (w >= 1200) return 4;
          if (w >= 768) return 2;
          return 1;
        }

        whypartnerCards.forEach((card, i) => {
          const fromY = 60 * (i + 1);
          const col = i % getWhypartnerColCount();
          const start = `top ${90 - col * 4}%`;
          const end = `top ${58 - col * 3}%`;

          gsap.fromTo(
            card,
            { y: fromY, force3D: true },
            {
              y: 0,
              ease: "none",
              force3D: true,
              scrollTrigger: {
                trigger: card,
                start,
                end,
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
     Brochure language cards — rise on scroll
     (match whypartner-card feel)
  --------------------------------------- */
  if (typeof gsap !== "undefined" && typeof ScrollTrigger !== "undefined") {
    const brochureGrid = document.querySelector(
      ".whypartner-brochure-section .solutions-brochure-grid"
    );
    const brochureCards = brochureGrid
      ? gsap.utils.toArray(
          brochureGrid.querySelectorAll(".solutions-brochure-block")
        )
      : [];

    if (brochureGrid && brochureCards.length) {
      if (reduceMotion) {
        gsap.set(brochureCards, { clearProps: "transform,opacity" });
      } else {
        brochureGrid.classList.add("is-rise-anim");

        function getBrochureColCount() {
          const w = window.innerWidth;
          if (w >= 1024) return 6;
          if (w >= 768) return 4;
          if (w >= 640) return 3;
          return 2;
        }

        brochureCards.forEach((card, i) => {
          const col = i % getBrochureColCount();
          const fromY = 60 * (i + 1);
          const start = `top ${90 - col * 4}%`;
          const end = `top ${58 - col * 3}%`;

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
                start,
                end,
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
     FAQ accordion — one open at a time (CSS grid expand)
  --------------------------------------- */
  const faqRoot = document.querySelector("[data-solutions-faq]");
  if (faqRoot) {
    const items = [...faqRoot.querySelectorAll(".solutions-faq-item")];

    function setOpen(item, open) {
      const btn = item.querySelector(".solutions-faq-trigger");
      const panel = item.querySelector(".solutions-faq-panel");
      if (!btn || !panel) return;
      item.classList.toggle("is-open", open);
      btn.setAttribute("aria-expanded", open ? "true" : "false");
      panel.setAttribute("aria-hidden", open ? "false" : "true");
    }

    items.forEach((item) => {
      const panel = item.querySelector(".solutions-faq-panel");
      if (panel) {
        panel.removeAttribute("hidden");
        panel.style.height = "";
        panel.setAttribute(
          "aria-hidden",
          item.classList.contains("is-open") ? "false" : "true"
        );
      }

      const btn = item.querySelector(".solutions-faq-trigger");
      if (!btn) return;
      btn.addEventListener("click", () => {
        const willOpen = !item.classList.contains("is-open");
        items.forEach((other) => setOpen(other, other === item && willOpen));
      });
    });
  }

  /* ---------------------------------------
     Enquiry form — native <select> cselect + sqft / occupation others
  --------------------------------------- */
  const enquiryRoot = document.getElementById("solutions-enquiry");
  if (enquiryRoot) {
    function enhanceNativeCselect(root, scopeSelector) {
      const select = root.querySelector("select");
      if (!select || root.dataset.cselectReady === "1") return;
      root.dataset.cselectReady = "1";
      select.classList.add("cselect-native");

      const id = "solutions-cs-" + (select.id || select.name || Math.random().toString(36).slice(2));

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
          gsap.fromTo(menu, { autoAlpha: 0, y: -6 }, { autoAlpha: 1, y: 0, duration: 0.22, ease: "power2.out" });
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

    enquiryRoot.querySelectorAll(".cselect").forEach((root) => {
      enhanceNativeCselect(root, "#solutionsEnquiryForm, #solutions-enquiry");
    });
    document.addEventListener("click", (e) => {
      enquiryRoot.querySelectorAll(".cselect.is-open").forEach((el) => {
        if (!el.contains(e.target) && el._close) el._close();
      });
    });

    const sqftField = enquiryRoot.querySelector("[data-sqft-field]");
    const spaceRadios = enquiryRoot.querySelectorAll('input[name="HaveSpaceForStoreSetup"]');
    function syncSqft() {
      if (!sqftField) return;
      const yes = enquiryRoot.querySelector('input[name="HaveSpaceForStoreSetup"][value="Yes"]');
      const show = !!(yes && yes.checked);
      sqftField.classList.toggle("is-collapsed", !show);
      const input = sqftField.querySelector("#StoreSizeSqFt");
      if (input) {
        input.disabled = !show;
        if (!show) input.value = "";
      }
    }
    spaceRadios.forEach((radio) => radio.addEventListener("change", syncSqft));
    syncSqft();

    const othersField = enquiryRoot.querySelector("[data-occupation-others]");
    const occupationSelect = enquiryRoot.querySelector("#CurrentOccupation");
    function syncOccupationOthers() {
      if (!othersField) return;
      const show = occupationSelect && occupationSelect.value === "Other";
      othersField.classList.toggle("is-collapsed", !show);
      const input = othersField.querySelector("#CurrentOccupationOthers");
      if (input) {
        input.disabled = !show;
        if (!show) input.value = "";
      }
    }
    if (occupationSelect) occupationSelect.addEventListener("change", syncOccupationOthers);
    syncOccupationOthers();

    const enquiryForm = document.getElementById("solutionsEnquiryForm");
    if (enquiryForm) {
      enquiryForm.addEventListener("reset", () => {
        requestAnimationFrame(() => {
          syncSqft();
          syncOccupationOthers();
        });
      });
    }
  }

  /* ---------------------------------------
     Partner dock — right-edge sticky tab + panel
  --------------------------------------- */
  (function initPartnerDock() {
    const root = document.getElementById("partner-dock");
    if (!root) return;

    const tab = document.getElementById("partner-dock-tab");
    const panel = document.getElementById("partner-dock-panel");
    const closeBtn = document.getElementById("partner-dock-close");
    const backdrop = document.getElementById("partner-dock-backdrop");
    if (!tab || !panel) return;

    let open = false;

    function setOpen(next) {
      open = !!next;
      tab.setAttribute("aria-expanded", open ? "true" : "false");
      panel.setAttribute("aria-hidden", open ? "false" : "true");

      if (open) {
        panel.hidden = false;
        if (backdrop) {
          backdrop.hidden = false;
          backdrop.setAttribute("aria-hidden", "false");
        }
        document.documentElement.classList.add("partner-dock-open");
        requestAnimationFrame(() => {
          root.classList.add("is-open");
          const firstLink = panel.querySelector("a");
          if (firstLink) firstLink.focus({ preventScroll: true });
        });
      } else {
        root.classList.remove("is-open");
        document.documentElement.classList.remove("partner-dock-open");
        if (backdrop) backdrop.setAttribute("aria-hidden", "true");
        const hide = () => {
          if (open) return;
          panel.hidden = true;
          if (backdrop) backdrop.hidden = true;
        };
        if (reduceMotion) hide();
        else window.setTimeout(hide, 360);
        tab.focus({ preventScroll: true });
      }
    }

    tab.addEventListener("click", () => setOpen(!open));
    if (closeBtn) closeBtn.addEventListener("click", () => setOpen(false));
    if (backdrop) backdrop.addEventListener("click", () => setOpen(false));

    document.addEventListener("keydown", (e) => {
      if (e.key === "Escape" && open) {
        e.preventDefault();
        setOpen(false);
      }
    });
  })();
});
