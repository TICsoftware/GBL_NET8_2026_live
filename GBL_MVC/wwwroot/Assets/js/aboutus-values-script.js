/* =====================================================================
   ABOUT US — Values that define how we work
   GSAP + ScrollTrigger + canvas frame sequence (120 frames, 5 icons)
   ===================================================================== */
(() => {
  "use strict";

  const READY = "aboutus-values:ready";
  const signalReady = () => {
    window.__aboutusValuesReady = true;
    window.dispatchEvent(new Event(READY));
  };

  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") {
    signalReady();
    return;
  }
  if (typeof gsap.registerPlugin === "function") gsap.registerPlugin(ScrollTrigger);

  const FRAME_COUNT = 120;
  const section = document.getElementById("aboutus-values");
  const stickyStage = section && section.querySelector(".aboutus-values-sticky");
  const stage = document.getElementById("aboutus-values-stage");
  const canvas = document.getElementById("aboutus-values-flower");
  const title = document.getElementById("aboutus-values-title");
  const logo = document.getElementById("aboutus-values-logo");

  if (!section || !stickyStage || !stage || !canvas || !title) {
    signalReady();
    return;
  }

  const ctx = canvas.getContext("2d", { alpha: true });
  if (!ctx) {
    signalReady();
    return;
  }

  const framesBase = (
    section.dataset.framesBase || "/Assets/images/aboutus/images/"
  ).replace(/\/?$/, "/");
  const FRAME_PATH = (i) =>
    `${framesBase}frame_${String(i).padStart(4, "0")}.png?v=petal3`;

  const ORDER = [
    "integrity",
    "sustainability",
    "strength",
    "consistency",
    "customercentricity",
  ];

  const icons = ORDER.map((k) =>
    section.querySelector(`.aboutus-values-icon[data-key="${k}"]`)
  ).filter(Boolean);
  const labels = ORDER.map((k) =>
    section.querySelector(`.aboutus-values-label[data-key="${k}"]`)
  ).filter(Boolean);
  const pairs = ORDER.map((k) => ({
    key: k,
    icon: section.querySelector(`.aboutus-values-icon[data-key="${k}"]`),
    label: section.querySelector(`.aboutus-values-label[data-key="${k}"]`),
  })).filter((p) => p.icon || p.label);

  const images = new Array(FRAME_COUNT);
  const state = { frame: 0 };
  let needsRender = true;
  const SEQ_DURATION = 2.4;
  let master = null;
  let seqStart = 0;

  // The frame_*.png sequence was regenerated as a true 5-fold rotationally
  // symmetric pinwheel (72° apart, spine angles 0/72/144/216/288 matching
  // ORDER below), so these node x/y are the exact trig projection of each
  // petal's icon point (radius 0.80 * tip, well inside the ink — verified
  // by sampling all five land at 84–91% local ink coverage on
  // frame_0120.png), not a hand-picked calibration against uneven artwork.
  const FIGMA_LAYOUT = {
    nodes: {
      logo: { x: 53, y: 53, size: 28 },
      customercentricity: { x: 51.25, y: 12.6, size: 28 },
      integrity: { x: 90.72, y: 39.80, size: 28 },
      sustainability: { x: 76.45, y: 85.83, size: 28 },
      strength: { x: 28.78, y: 85.83, size: 28 },
      consistency: { x: 13.65, y: 41.35, size: 28 },
    },
    labelOffset: {
      customercentricity: { x: 17, y: -9 },
      integrity: { x: 17, y: -6 },
      sustainability: { x: 17, y: -2 },
      strength: { x: -17, y: -2 },
      consistency: { x: -17, y: -4 },
    },
    /* iPad / tablet — flower is mid-size; push left labels further out */
    labelOffsetTablet: {
      customercentricity: { x: 12, y: -14 },
      integrity: { x: 22, y: -8 },
      sustainability: { x: 20, y: 2 },
      strength: { x: -26, y: 2 },
      consistency: { x: -32, y: -2 },
    },
    labelOffsetMobile: {
      /* Push labels outward into clear space around the smaller flower */
      customercentricity: { x: 0, y: -34 },
      integrity: { x: 28, y: 15 },
      sustainability: { x: 14, y: 14 },
      strength: { x: -16, y: 4 },
      consistency: { x: -28, y: 9 },
    },
  };

  const mqNarrow = window.matchMedia("(max-width: 767px)");
  const mqTablet = window.matchMedia("(min-width: 768px) and (max-width: 1024px)");
  const EDGE_PAD = () => (mqNarrow.matches ? 10 : mqTablet.matches ? 14 : 12);
  const NODE_SIZE_SCALE = () => (mqNarrow.matches ? 0.9 : mqTablet.matches ? 0.92 : 1);
  const FRAME_NUDGE = { x: 0, y: 0 };
  const nodes = [...icons, logo].filter(Boolean);

  const headerOffset = () =>
    parseFloat(
      getComputedStyle(document.documentElement).getPropertyValue("--header-h")
    ) || (mqNarrow.matches ? 72 : 106);

  function centerNodes(extra = {}) {
    gsap.set(nodes, {
      x: 0,
      y: 0,
      xPercent: -50,
      yPercent: -50,
      ...extra,
    });
  }

  function layoutScene() {
    const stageRect = stage.getBoundingClientRect();
    const hostRect = stickyStage.getBoundingClientRect();
    const w = stageRect.width;
    const h = stageRect.height;
    if (!w || !h) return;

    const stageToHostX = stageRect.left - hostRect.left;
    const stageToHostY = stageRect.top - hostRect.top;
    const iconCenters = {};
    const sizeScale = NODE_SIZE_SCALE();
    const labelOffsets = mqNarrow.matches
      ? FIGMA_LAYOUT.labelOffsetMobile
      : mqTablet.matches
        ? FIGMA_LAYOUT.labelOffsetTablet
        : FIGMA_LAYOUT.labelOffset;
    const titleClear = mqNarrow.matches
      ? Math.max(
          (title.getBoundingClientRect().bottom - hostRect.top) + 6,
          56
        )
      : 0;

    for (const [key, p] of Object.entries(FIGMA_LAYOUT.nodes)) {
      const el =
        key === "logo"
          ? logo
          : section.querySelector(`.aboutus-values-icon[data-key="${key}"]`);
      if (!el) continue;

      const left = (p.x / 100) * w;
      const top = (p.y / 100) * h;
      const size = ((p.size * sizeScale) / 100) * w;

      el.style.left = left + "px";
      el.style.top = top + "px";
      el.style.width = size + "px";

      if (key !== "logo") iconCenters[key] = { x: left, y: top, size };
    }

    for (const key of ORDER) {
      const labelEl = section.querySelector(
        `.aboutus-values-label[data-key="${key}"]`
      );
      const icon = iconCenters[key];
      const off = labelOffsets[key];
      if (!labelEl || !icon || !off) continue;

      const side = labelEl.dataset.side || "right";
      labelEl.style.left = "0px";
      labelEl.style.top = "0px";
      const lr = labelEl.getBoundingClientRect();
      const ox = (off.x / 100) * w;
      const oy = (off.y / 100) * h;

      let left;
      let top;

      if (mqNarrow.matches && key === "customercentricity") {
        /* Center above the flower, clear of the section title */
        left = stageToHostX + w / 2 - lr.width / 2;
        top = stageToHostY + icon.y + oy - lr.height * 0.15;
      } else {
        left = stageToHostX + icon.x + ox;
        top = stageToHostY + icon.y + oy;
        if (side === "left") left -= lr.width;
      }

      const pad = EDGE_PAD();
      const gap = mqTablet.matches ? 12 : 8;
      const iconLeftHost = stageToHostX + icon.x - icon.size / 2;
      const iconRightHost = stageToHostX + icon.x + icon.size / 2;

      /* Keep left/right labels clear of their icons (tablet clamp was causing overlap) */
      if (side === "left") {
        left = Math.min(left, iconLeftHost - gap - lr.width);
      } else if (!(mqNarrow.matches && key === "customercentricity")) {
        left = Math.max(left, iconRightHost + gap);
      }

      left = Math.max(pad, Math.min(left, hostRect.width - pad - lr.width));
      top = Math.max(
        Math.max(pad, titleClear),
        Math.min(top, hostRect.height - pad - lr.height)
      );

      labelEl.style.left = left + "px";
      labelEl.style.top = top + "px";
    }
  }

  const reduceMotion = window.matchMedia(
    "(prefers-reduced-motion: reduce)"
  ).matches;

  function preloadImages() {
    let loaded = 0;

    return new Promise((resolve) => {
      const finishOne = () => {
        loaded++;
        if (loaded === FRAME_COUNT) resolve();
      };

      for (let i = 1; i <= FRAME_COUNT; i++) {
        const img = new Image();
        img.decoding = "async";
        img.src = FRAME_PATH(i);
        images[i - 1] = img;
        img.onload = () => {
          if (img.decode) img.decode().then(finishOne).catch(finishOne);
          else finishOne();
        };
        img.onerror = finishOne;
      }
    });
  }

  function resizeCanvas() {
    const dpr = Math.min(window.devicePixelRatio || 1, 2);
    const rect = stage.getBoundingClientRect();
    const w = Math.max(1, Math.round(rect.width * dpr));
    const h = Math.max(1, Math.round(rect.height * dpr));
    if (canvas.width !== w || canvas.height !== h) {
      canvas.width = w;
      canvas.height = h;
    }
    needsRender = true;
  }

  function renderFrame(index) {
    const img = images[Math.max(0, Math.min(FRAME_COUNT - 1, index | 0))];
    if (!img || !img.naturalWidth) return;

    const cw = canvas.width;
    const ch = canvas.height;
    ctx.clearRect(0, 0, cw, ch);

    const scale = Math.min(cw / img.naturalWidth, ch / img.naturalHeight);
    const w = img.naturalWidth * scale;
    const h = img.naturalHeight * scale;
    const dx = (cw - w) / 2 + cw * FRAME_NUDGE.x;
    const dy = (ch - h) / 2 + ch * FRAME_NUDGE.y;
    ctx.drawImage(img, dx, dy, w, h);
  }

  let lastDrawn = -1;
  function tick() {
    if (master) {
      const p = gsap.utils.clamp(
        0,
        1,
        (master.time() - seqStart) / SEQ_DURATION
      );
      const f = Math.round(p * (FRAME_COUNT - 1));
      if (f !== state.frame) {
        state.frame = f;
        needsRender = true;
      }
    }
    if (needsRender || (state.frame | 0) !== lastDrawn) {
      lastDrawn = state.frame | 0;
      renderFrame(lastDrawn);
      needsRender = false;
    }
    requestAnimationFrame(tick);
  }

  function createTimeline() {
    /* Title + first flower frame visible as soon as the section pins —
       fading them from 0 left a blank white viewport at scrub progress 0. */
    gsap.set(title, { opacity: 1, scale: 1, y: 0 });
    gsap.set(canvas, { opacity: 1, filter: "blur(0px)" });
    centerNodes({
      opacity: 0,
      scale: 0.5,
      filter: "blur(10px)",
    });
    gsap.set(labels, {
      opacity: 0,
      x: (i, el) => (el.dataset.side === "left" ? -24 : 24),
    });

    const tl = gsap.timeline({
      defaults: { ease: "power3.out" },
      scrollTrigger: {
        trigger: section,
        start: () => `top top+=${headerOffset()}`,
        end: "+=240%",
        pin: stickyStage,
        pinSpacing: true,
        scrub: 1,
        anticipatePin: 1,
        invalidateOnRefresh: true,
        onRefresh: () => {
          layoutScene();
          centerNodes();
        },
      },
    });

    tl.addLabel("seq", 0)
      .to({}, { duration: SEQ_DURATION }, "seq")
      .to(
        logo,
        {
          opacity: 1,
          scale: 1,
          filter: "blur(0px)",
          duration: 0.65,
          ease: "power2.out",
        },
        "seq+=0.35"
      );

    const PAIR_STAGGER = 0.28;
    const PAIR_DUR = 0.55;
    pairs.forEach((pair, i) => {
      const at = `seq+=${(0.55 + i * PAIR_STAGGER).toFixed(2)}`;
      if (pair.icon) {
        tl.to(
          pair.icon,
          {
            opacity: 1,
            scale: 1,
            filter: "blur(0px)",
            duration: PAIR_DUR,
            ease: "back.out(1.6)",
          },
          at
        );
      }
      if (pair.label) {
        tl.to(
          pair.label,
          {
            opacity: 1,
            x: 0,
            duration: PAIR_DUR,
            ease: "power2.out",
          },
          at
        );
      }
    });

    tl.to({}, { duration: 0.9 });
    master = tl;
    seqStart = tl.labels.seq;
    return tl;
  }

  function init() {
    layoutScene();
    centerNodes({ scale: 0.5 });
    resizeCanvas();
    requestAnimationFrame(tick);

    preloadImages().then(() => {
      renderFrame(0);

      if (reduceMotion) {
        state.frame = FRAME_COUNT - 1;
        needsRender = true;
        gsap.set([title, canvas, ...icons, ...labels, logo].filter(Boolean), {
          opacity: 1,
          scale: 1,
          x: 0,
          y: 0,
          filter: "none",
        });
        centerNodes({ opacity: 1, scale: 1, filter: "none" });
        signalReady();
        return;
      }

      createTimeline();
      ScrollTrigger.refresh();
      /* Sections below (Built on Trust, CTA) wait for this — creating them
         earlier measures against the pre-pin page height, and a later
         ScrollTrigger.refresh() does not correct that mismatch. */
      signalReady();
    });

    let resizeQueued = false;
    const onViewportChange = () => {
      if (resizeQueued) return;
      resizeQueued = true;
      requestAnimationFrame(() => {
        resizeQueued = false;
        layoutScene();
        centerNodes();
        resizeCanvas();
        ScrollTrigger.refresh();
      });
    };
    window.addEventListener("resize", onViewportChange, { passive: true });
    if (typeof mqNarrow.addEventListener === "function") {
      mqNarrow.addEventListener("change", onViewportChange);
    } else if (typeof mqNarrow.addListener === "function") {
      mqNarrow.addListener(onViewportChange);
    }
    if (typeof mqTablet.addEventListener === "function") {
      mqTablet.addEventListener("change", onViewportChange);
    } else if (typeof mqTablet.addListener === "function") {
      mqTablet.addListener(onViewportChange);
    }

    if (document.fonts && document.fonts.ready) {
      document.fonts.ready.then(() => {
        layoutScene();
        centerNodes();
      });
    }
    ScrollTrigger.addEventListener("refreshInit", () => {
      layoutScene();
      centerNodes();
    });
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init, { once: true });
  } else {
    init();
  }
})();
