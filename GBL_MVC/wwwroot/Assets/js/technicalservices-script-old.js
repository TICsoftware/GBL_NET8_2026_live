document.addEventListener("DOMContentLoaded", () => {
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  (() => {
    /* ---- CTA lion ---- */
  if (!reduceMotion) {
    const lionWrap =
      document.querySelector('.bg-brand-band .lion-logo-wrap') ||
      document.querySelector('.lion-logo-wrap');
    const lionSvg =
      lionWrap && (lionWrap.querySelector('.lion-logo-svg') || lionWrap.querySelector('svg'));
    const lionFill = lionSvg && lionSvg.querySelector('.lion-logo-fill');

    if (lionWrap && lionSvg && lionFill) {
      let lionStroke = lionSvg.querySelector('.lion-logo-stroke');
      if (!lionStroke) {
        lionStroke = lionFill.cloneNode();
        lionStroke.removeAttribute('fill');
        lionStroke.removeAttribute('fill-opacity');
        lionStroke.classList.remove('lion-logo-fill');
        lionStroke.classList.add('lion-logo-stroke');
        lionStroke.setAttribute('fill', 'none');
        lionStroke.setAttribute('stroke', 'rgba(255,255,255,0.55)');
        lionStroke.setAttribute('stroke-width', '1.75');
        lionStroke.setAttribute('stroke-linecap', 'round');
        lionStroke.setAttribute('stroke-linejoin', 'round');
        lionStroke.setAttribute('vector-effect', 'non-scaling-stroke');
        lionSvg.insertBefore(lionStroke, lionFill);
      }

      const pathLen = (() => {
        try {
          return lionStroke.getTotalLength();
        } catch (e) {
          return 0;
        }
      })();

      if (pathLen > 0) {
        gsap.set(lionWrap, {
          xPercent: 0,
          yPercent: 0,
          scale: 0.05,
          transformOrigin: '50% 50%',
          force3D: true,
        });
        gsap.set(lionStroke, {
          strokeDasharray: pathLen,
          strokeDashoffset: pathLen,
          autoAlpha: 1,
        });
        gsap.set(lionFill, { autoAlpha: 0 });

        const trigger = lionWrap.closest('.bg-brand-band') || lionWrap;

        gsap
          .timeline({
            scrollTrigger: {
              trigger,
              start: 'top 75%',
              toggleActions: 'play none none reverse',
              invalidateOnRefresh: true,
            },
          })
          .to(
            lionWrap,
            {
              scale: 1,
              duration: 1.35,
              ease: 'power2.out',
              force3D: true,
            },
            0
          )
          .to(
            lionStroke,
            {
              strokeDashoffset: 0,
              duration: 1.35,
              ease: 'power2.inOut',
            },
            0
          )
          .to(lionFill, { autoAlpha: 1, duration: 0.55, ease: 'power2.out' }, '-=0.3')
          .to(lionStroke, { autoAlpha: 0, duration: 0.4, ease: 'power1.out' }, '-=0.35');
      }
    }
  }
})();

/* ---------------------------------------
   CUSTOM SELECTS
--------------------------------------- */

function buildSelect(root) {
  const name = root.dataset.name;
  const placeholder = root.dataset.placeholder;
  const options = (root.dataset.options || "").split("|").filter(Boolean);
  const id = "cs-" + name;

  root.innerHTML = `
    <input type="hidden" name="${name}" value="" />
    <button type="button" class="f-field cselect-btn" data-empty="true" role="combobox"
            aria-haspopup="listbox" aria-expanded="false" aria-controls="${id}" aria-label="${placeholder}">
      <span class="cselect-value">${placeholder}</span>
      <svg class="cselect-chevron" width="14" height="8" viewBox="0 0 14 8" fill="none" aria-hidden="true">
        <path d="M1 1l6 6 6-6" stroke="#fff" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
    </button>
    <div class="cselect-menu" id="${id}" role="listbox" aria-label="${placeholder}">
      ${options.map(o => `<button type="button" class="cselect-option" role="option" aria-selected="false" data-value="${o}">${o}</button>`).join("")}
    </div>`;

  const btn = root.querySelector(".cselect-btn");
  const menu = root.querySelector(".cselect-menu");
  const hidden = root.querySelector("input[type=hidden]");
  const label = root.querySelector(".cselect-value");

  function close() {
    if (!root.classList.contains("is-open")) return;
    root.classList.remove("is-open");
    btn.setAttribute("aria-expanded", "false");
    gsap.to(menu, { autoAlpha: 0, y: -6, duration: .18, ease: "power2.in" });
  }
  function open() {
    document.querySelectorAll("[data-cselect].is-open").forEach(el => el !== root && el._close && el._close());
    root.classList.add("is-open");
    btn.setAttribute("aria-expanded", "true");
    gsap.fromTo(menu, { autoAlpha: 0, y: -6 }, { autoAlpha: 1, y: 0, duration: .22, ease: "power2.out" });
  }
  root._close = close;

  btn.addEventListener("click", () => (root.classList.contains("is-open") ? close() : open()));
  menu.addEventListener("click", (e) => {
    const opt = e.target.closest(".cselect-option");
    if (!opt) return;
    menu.querySelectorAll(".cselect-option").forEach(o => o.setAttribute("aria-selected", String(o === opt)));
    hidden.value = opt.dataset.value;
    label.textContent = opt.dataset.value;
    btn.dataset.empty = "false";
    close();
    btn.focus();
  });
  btn.addEventListener("keydown", (e) => {
    if (e.key === "Escape") close();
    if (e.key === "ArrowDown" || e.key === "Enter" || e.key === " ") { e.preventDefault(); open(); menu.querySelector(".cselect-option")?.focus(); }
  });

  root.setOptions = (list, ph) => {
    hidden.value = "";
    label.textContent = ph;
    btn.dataset.empty = "true";
    btn.setAttribute("aria-label", ph);
    menu.innerHTML = list.map(o => `<button type="button" class="cselect-option" role="option" aria-selected="false" data-value="${o}">${o}</button>`).join("");
  };
}
document.querySelectorAll("[data-cselect]").forEach(buildSelect);
document.addEventListener("click", (e) => {
  document.querySelectorAll("[data-cselect].is-open").forEach(el => { if (!el.contains(e.target)) el._close(); });
});

/* ---------- radio filter: swaps the requirement select ---------- */
const requirementSelect = document.querySelector('[data-cselect][data-name="requirement"]');
const REQUIREMENTS = {
  onsite: { placeholder: "Type of tests required", options: ["Cement tests", "Aggregate tests", "Concrete tests", "Water quality testing", "Other tests"] },
  support: { placeholder: "Type of support required", options: ["Concrete mix design", "Material selection and quality control", "Curing recommendations", "Construction troubleshooting", "Site personnel training"] },
};
document.querySelectorAll('input[name="serviceType"]').forEach(radio => {
  radio.addEventListener("change", () => {
    const cfg = REQUIREMENTS[radio.value];
    if (!cfg || !requirementSelect) return;
    requirementSelect.setOptions(cfg.options, cfg.placeholder);
    gsap.fromTo(requirementSelect, { autoAlpha: .3, y: 6 }, { autoAlpha: 1, y: 0, duration: .3, ease: "power2.out" });
  });
});

document.getElementById("enquiry-form").addEventListener("submit", e => e.preventDefault());

/* ---------------------------------------
   PARALLAX IMAGE
--------------------------------------- */
  if (!reduceMotion && typeof gsap !== 'undefined' && typeof ScrollTrigger !== 'undefined') {
    gsap.utils.toArray('.parallax-wrap').forEach((wrap) => {
  

      const img = wrap.querySelector('.parallax-img');
      if (!img) return;

      // Balanced travel (covers overflow:hidden + height:130% CSS)
      gsap.fromTo(
        img,
        { yPercent: -12 },
        {
          yPercent: 12,
          ease: 'none',
          force3D: true,
          scrollTrigger: {
            trigger: wrap,
            start: 'top bottom',
            end: 'bottom top',
            scrub: 0.8, // smoother with Lenis than scrub:true
            invalidateOnRefresh: true,
          },
        }
      );
    });
  }

/* ---------------------------------------
   TABS
--------------------------------------- */
/* ---------------- tabs ---------------- */
const tabBar = document.getElementById("tabbar");
const tabPill = document.getElementById("tabpill");
const tabWrap = document.querySelector("[data-tab-wrap]");
const tabAnchor = document.querySelector("[data-tab-anchor]");
const tabs = [...document.querySelectorAll(".tab")];
let currentTab = null;

function tabDockTop() {
  if (window.innerWidth < 768) return 64;
  if (window.innerWidth < 1024) return 70;
  return 76;
}

function movePill(el, animate) {
  if (!el || !tabPill) return;
  const to = { x: el.offsetLeft, y: el.offsetTop, width: el.offsetWidth, height: el.offsetHeight };
  gsap.to(tabPill, {
    ...to,
    duration: animate ? 0.45 : 0,
    ease: "power3.out",
  });
}

function setActive(id, animate = true) {
  if (currentTab === id) return;
  currentTab = id;
  tabs.forEach(t => {
    const on = t.dataset.tab === id;
    t.setAttribute("aria-selected", String(on));
    t.classList.toggle("text-white", on);
    t.classList.toggle("hover:text-primary", !on);
    if (on) {
      movePill(t, animate);
      // on narrow screens the pill bar scrolls horizontally: keep the active tab visible
      const track = tabBar && tabBar.parentElement;
      if (track && track.scrollWidth > track.clientWidth) {
        track.scrollTo({ left: Math.max(t.offsetLeft - 16, 0), behavior: "smooth" });
      }
    }
  });
}

function dockTabs(active) {
  if (!tabWrap || !tabAnchor || !tabBar) return;
  if (active) {
    if (!tabWrap.classList.contains("is-docked")) {
      tabAnchor.style.height = tabWrap.offsetHeight + "px";
      tabWrap.classList.add("is-docked");
    }
    tabWrap.style.setProperty("--tab-dock-top", tabDockTop() + "px");
    tabBar.classList.add("shadow-tabs");
    requestAnimationFrame(() => movePill(tabs.find(t => t.dataset.tab === currentTab), false));
  } else {
    tabWrap.classList.remove("is-docked");
    tabAnchor.style.height = "";
    tabBar.classList.remove("shadow-tabs");
    requestAnimationFrame(() => movePill(tabs.find(t => t.dataset.tab === currentTab), false));
  }
}

setActive("onsite", false);
const remeasurePill = () => movePill(tabs.find(t => t.dataset.tab === currentTab), false);
window.addEventListener("resize", remeasurePill);
window.addEventListener("load", remeasurePill);
if (document.fonts && document.fonts.ready) document.fonts.ready.then(remeasurePill);
tabs.forEach(t => t.addEventListener("click", () => {
  const el = document.getElementById(t.dataset.tab);
  if (!el) return;
  const offset = (window.innerWidth < 768 ? 120 : 150);
  window.scrollTo({ top: el.getBoundingClientRect().top + window.scrollY - offset, behavior: "smooth" });
}));


/* ---------------- GSAP ---------------- */
gsap.registerPlugin(ScrollTrigger);

// keep tabs in intro-copy in document flow; dock fixed under header while services scroll
if (tabWrap && tabAnchor) {
  ScrollTrigger.create({
    trigger: tabAnchor,
    start: () => "top " + tabDockTop() + "px",
    endTrigger: "[data-services-end]",
    end: () => "bottom " + (tabDockTop() + 8) + "px",
    invalidateOnRefresh: true,
    onToggle: self => dockTabs(self.isActive),
  });
}

// active tab follows the section in view
["onsite", "support"].forEach(id => {
  ScrollTrigger.create({
    trigger: "#" + id,
    start: "top 45%",
    end: "bottom 45%",
    onToggle: self => { if (self.isActive) setActive(id); },
  });
});

// scroll-driven vertical progress rails (all breakpoints)
document.querySelectorAll("[data-rail]").forEach(rail => {
  const thumb = rail.querySelector("[data-rail-thumb]");
  if (!thumb) return;
  gsap.fromTo(thumb, { y: 0 }, {
    y: () => Math.max(rail.offsetHeight - 40, 0),
    ease: "none",
    scrollTrigger: {
      trigger: rail.closest("[data-rail-scope]"),
      start: "top 60%",
      end: "bottom 70%",
      scrub: 0.4,
      invalidateOnRefresh: true,
    },
  });
});

// keep measurements correct across breakpoint / orientation changes
let refreshTimer;
window.addEventListener("resize", () => {
  clearTimeout(refreshTimer);
  refreshTimer = setTimeout(() => ScrollTrigger.refresh(), 200);
});
window.addEventListener("orientationchange", () => ScrollTrigger.refresh());

// entrance animations
gsap.utils.toArray("[data-fade]").forEach(el => {
  gsap.from(el, { y: 42, autoAlpha: 0, duration: .8, ease: "power3.out", scrollTrigger: { trigger: el, start: "top 88%",  toggleActions: "play none none reverse", } });
});
gsap.utils.toArray("[data-count]").forEach(el => {
  gsap.from(el, { x: -40, autoAlpha: 0, duration: .9, ease: "power3.out", scrollTrigger: { trigger: el, start: "top 85%",  toggleActions: "play none none reverse", } });
});

// sticky section headings: smooth fade in on enter, fade out near section end
gsap.utils.toArray(".section-sticky-title").forEach(title => {
  const scope = title.closest("[data-rail-scope]");
  if (!scope) return;
  gsap.set(title, { autoAlpha: 0, y: 24 });
  gsap.to(title, {
    autoAlpha: 1, y: 0, ease: "power2.out",
    scrollTrigger: { trigger: scope, start: "top 80%", end: "top 45%", scrub: 0.6 }
  });
  gsap.to(title, {
    autoAlpha: 0, y: -24, ease: "power2.in",
    scrollTrigger: { trigger: scope, start: "bottom 55%", end: "bottom 25%", scrub: 0.6 }
  });
});



});