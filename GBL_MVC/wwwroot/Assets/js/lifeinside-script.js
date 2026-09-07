/* =====================================================================
   LIFE INSIDE PRIYA CEMENT â€” cinematic scroll story
   GSAP + ScrollTrigger + HTML5 Canvas image sequence (120 frames)
   ===================================================================== */
(() => {
  "use strict";

  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;
  gsap.registerPlugin(ScrollTrigger);

  const FRAME_COUNT = 120;

  const section = document.getElementById("life-inside");
  const stickyStage = section && section.querySelector(".sticky-stage");
  const stage = document.getElementById("stage");
  const canvas = document.getElementById("flower");
  const title = document.getElementById("section-title");
  const logo = document.getElementById("center-logo");

  if (!section || !stickyStage || !stage || !canvas || !title) return;

  const ctx = canvas.getContext("2d", { alpha: true });
  if (!ctx) return;

  /* Razor ~/ does not work in JS â€” use data-frames-base from the section */
  const framesBase = (
    section.dataset.framesBase || "/Assets/images/careers/images/"
  ).replace(/\/?$/, "/");
  const FRAME_PATH = (i) =>
    `${framesBase}frame_${String(i).padStart(4, "0")}.png`;

  /* Reveal order around the flower (clockwise from Learning).
     Must stay in this order — do not sort by CMS Sequence. */
  const ORDER = [
    "learning",
    "leadership",
    "recognition",
    "purpose",
    "culture",
    "safety",
    "wellness",
  ];

  function normalizeKey(s) {
    return String(s || "")
      .toLowerCase()
      .replace(/[^a-z0-9]+/g, "-")
      .replace(/^-+|-+$/g, "");
  }

  /** Resolve icon/label even if CMS title is "Learning & Growth" etc. */
  function findByOrderKey(key, selector) {
    const exact = section.querySelector(`${selector}[data-key="${key}"]`);
    if (exact) return exact;
    const nodes = Array.from(section.querySelectorAll(selector));
    return (
      nodes.find((el) => {
        const k = normalizeKey(el.getAttribute("data-key"));
        return k === key || k.startsWith(key + "-") || k.includes(key);
      }) || null
    );
  }

  const icons = ORDER.map((k) => findByOrderKey(k, ".icon")).filter(Boolean);
  const labels = ORDER.map((k) => findByOrderKey(k, ".label")).filter(Boolean);
  const pairs = ORDER.map((k) => ({
    key: k,
    icon: findByOrderKey(k, ".icon"),
    label: findByOrderKey(k, ".label"),
  })).filter((p) => p.icon || p.label);

  const images = new Array(FRAME_COUNT);
  const state = { frame: 0 };
  let needsRender = true;

  const SEQ_DURATION = 3.6;
  let master = null;
  let seqStart = 0;

  /* Original Figma layout â€” matched to petal tips before any canvas nudge */
  const FIGMA_LAYOUT = {
    nodes: {
      logo: { x: 47.81, y: 51.36, size: 25.23 },
      wellness: { x: 52.8, y: 13.9, size: 24.76 },
      learning: { x: 80.17, y: 30.77, size: 24.76 },
      leadership: { x: 84.92, y: 64.13, size: 24.76 },
      recognition: { x: 62.22, y: 86.9, size: 24.76 },
      purpose: { x: 28.41, y: 83.9, size: 24.76 },
      culture: { x: 9.7, y: 56.13, size: 24.76 },
      safety: { x: 19.95, y: 26.78, size: 24.76 },
    },
    labelOffset: {
      wellness: { x: 14.2, y: -14.7 },
      learning: { x: 14.8, y: -5.9 },
      leadership: { x: 14.9, y: -5.2 },
      recognition: { x: 12.4, y: 5.3 },
      purpose: { x: -39.1, y: 5.4 },
      culture: { x: -34.3, y: 3.2 },
      safety: { x: -23.4, y: -28.0 },
    },
    /* Mobile — push labels into gutters with clearer vertical stagger */
    labelOffsetMobile: {
      wellness: { x: -23, y: -34 },
      learning: { x: 28, y: -34 },
      leadership: { x: 30, y: 10 },
      recognition: { x: 5, y: 15 },
      purpose: { x: -34, y: 13 },
      culture: { x: -42, y: 8 },
      safety: { x: -42, y: -14 },
    },
    /*
     * 1920px display @ ~150% browser zoom only (CSS viewport ~1280).
     * Does not replace desktop/mobile maps — selected only by isZoom150On1920().
     * Tune x/y in % of stage: +x right, +y down.
     */
    labelOffsetZoom150: {
      wellness: { x: 14.6, y: -16.2 },
      learning: { x: 15.6, y: -2.2 },
      leadership: { x: 15.8, y: -1.4 },
      recognition: { x: 16.8, y: 6.5 },
      purpose: { x: -42.8, y: 3.6 },
      culture: { x: -38.0, y: 1.6 },
      safety: { x: -12.2, y: -30.2 },
    },
  };

  const mqNarrow = window.matchMedia("(max-width: 767px)");
  const EDGE_PAD = () => (mqNarrow.matches ? 6 : 12);
  /* Larger nodes on phone so petal icons stay readable */
  const NODE_SIZE_SCALE = () => (mqNarrow.matches ? 1.22 : 1);

  /** 1920-class screen with ~150% page zoom (CSS viewport ≈ 1280×720). */
  function isZoom150On1920() {
    if (mqNarrow.matches) return false;
    const sw = window.screen && window.screen.width ? window.screen.width : 0;
    // Accept 1920-class displays (and common laptop variants)
    if (sw < 1800 || sw > 2100) return false;

    const outerW = window.outerWidth || sw;
    const innerW = window.innerWidth || 0;
    const innerH = window.innerHeight || 0;
    if (!innerW) return false;

    const zoomW = outerW / Math.max(innerW, 1);
    // 1920/1.5 ≈ 1280; height ~720 but browser chrome can make it lower
    const cssWidth150 = innerW >= 1180 && innerW <= 1380;
    const cssHeight150 = !innerH || (innerH >= 560 && innerH <= 900);
    const zoom150 = zoomW >= 1.35 && zoomW <= 1.7;

    const matched = (zoom150 || cssWidth150) && cssHeight150;
    // DevTools: document.documentElement.dataset.lifeinsideZoom150 === "1"
    document.documentElement.dataset.lifeinsideZoom150 = matched ? "1" : "0";
    return matched;
  }

  function getLabelOffsets() {
    if (mqNarrow.matches) return FIGMA_LAYOUT.labelOffsetMobile;
    if (isZoom150On1920() && FIGMA_LAYOUT.labelOffsetZoom150) {
      return FIGMA_LAYOUT.labelOffsetZoom150;
    }
    return FIGMA_LAYOUT.labelOffset;
  }

  /*
   * Per-icon fine-tune in % of stage (added on top of Figma %).
   * x: + right / âˆ’ left | y: + down / âˆ’ up | ~1 â‰ˆ 1% of stage width
   */
  const NODE_NUDGE = {
    learning: { x: 0, y: 0 },
    leadership: { x: 0, y: 0 },
    recognition: { x: 0, y: 0 },
    purpose: { x: 0, y: 0 },
    culture: { x: 0, y: 0 },
    safety: { x: 0, y: 0 },
    wellness: { x: 0, y: 0 },
    logo: { x: 0, y: 0 },
  };

  /*
   * Shift drawn frames so the flower hole lands on Figma logo center.
   * Icons/labels keep Figma % â€” they already match the (nudged) petals.
   */
  const FRAME_NUDGE = { x: 0.0341, y: 0.0466 };

  const nodes = [...icons, logo].filter(Boolean);

  /** Center nodes on their left/top anchor â€” GSAP only (no CSS translate) */
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
    const labelOffsets = getLabelOffsets();

    for (const [key, p] of Object.entries(FIGMA_LAYOUT.nodes)) {
      const el =
        key === "logo"
          ? logo
          : findByOrderKey(key, ".icon");
      if (!el) continue;

      const nudge = NODE_NUDGE[key] || { x: 0, y: 0 };
      const left = ((p.x + nudge.x) / 100) * w;
      const top = ((p.y + nudge.y) / 100) * h;
      const size = ((p.size * sizeScale) / 100) * w;

      el.style.left = left + "px";
      el.style.top = top + "px";
      el.style.width = size + "px";

      if (key !== "logo") {
        iconCenters[key] = { x: left, y: top, size };
      }
    }

    for (const key of ORDER) {
      const labelEl = findByOrderKey(key, ".label");
      const icon = iconCenters[key];
      const off = labelOffsets[key];
      if (!labelEl || !icon || !off) continue;

      const side = labelEl.dataset.side || "right";

      labelEl.style.left = "0px";
      labelEl.style.top = "0px";
      // Layout box (not getBoundingClientRect) — avoids zoom/transform skew
      const lw = labelEl.offsetWidth;
      const lh = labelEl.offsetHeight;

      /*
       * labelOffset is % of stage from the icon CENTER.
       * Edit FIGMA_LAYOUT.labelOffset / labelOffsetMobile / labelOffsetZoom150.
       */
      const ox = (off.x / 100) * w;
      const oy = (off.y / 100) * h;

      let left = stageToHostX + icon.x + ox;
      let top = stageToHostY + icon.y + oy;

      /* Left-side copy: offset is to the text start — shift by label width
         so the block sits fully on the left of the orb */
      if (side === "left") {
        left -= lw;
      }

      // Full x/y from labelOffsetZoom150 must apply — do not re-center and
      // ignore offsets. Softer edge pad at 150% so large ±x still moves.
      const pad = isZoom150On1920() ? 4 : EDGE_PAD();
      left = Math.max(pad, Math.min(left, hostRect.width - pad - lw));
      top = Math.max(pad, Math.min(top, hostRect.height - pad - lh));

      labelEl.style.left = Math.round(left) + "px";
      labelEl.style.top = Math.round(top) + "px";
    }
  }

  const reduceMotion = window.matchMedia(
    "(prefers-reduced-motion: reduce)"
  ).matches;

  function preloadImages() {
    let loaded = 0;
    let failed = 0;

    return new Promise((resolve) => {
      const finishOne = () => {
        loaded++;
        if (loaded === FRAME_COUNT) resolve({ failed });
      };

      for (let i = 1; i <= FRAME_COUNT; i++) {
        const img = new Image();
        img.decoding = "async";
        img.src = FRAME_PATH(i);
        images[i - 1] = img;
        img.onload = () => {
          if (img.decode) {
            img.decode().then(finishOne).catch(finishOne);
          } else {
            finishOne();
          }
        };
        img.onerror = () => {
          failed++;
          finishOne();
        };
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
       fading them from 0 left a blank viewport at scrub progress 0
       (same fix as aboutus-values). */
    gsap.set(title, { opacity: 1, scale: 1, y: 0 });
    gsap.set(canvas, { opacity: 1, filter: "blur(0px)" });
    /* GSAP alone centers nodes — never mix with CSS translate(-50%,-50%) */
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
        start: "top top",
        end: "+=300%",
        pin: stickyStage,
        pinSpacing: true,
        scrub: 1.05,
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
          duration: 0.55,
          ease: "power2.out",
        },
        "seq+=0.4"
      );

    /*
     * Strict 1→6 (then wellness): space pairs evenly across the flower
     * bloom window so scrubbing always reads Learning → … → Safety.
     * Stagger ≥ duration so the next pair starts as the previous settles.
     */
    const pairCount = Math.max(pairs.length, 1);
    const PAIR_INTRO = 0.55; /* after logo starts */
    const PAIR_WINDOW = Math.max(1.2, SEQ_DURATION - PAIR_INTRO - 0.2);
    const PAIR_STAGGER = PAIR_WINDOW / pairCount;
    const PAIR_DUR = Math.min(0.72, PAIR_STAGGER * 0.9);

    pairs.forEach((pair, i) => {
      const at = `seq+=${(PAIR_INTRO + i * PAIR_STAGGER).toFixed(3)}`;
      if (pair.icon) {
        tl.to(
          pair.icon,
          {
            opacity: 1,
            scale: 1,
            filter: "blur(0px)",
            duration: PAIR_DUR,
            ease: "power2.out",
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
        gsap.set(
          [title, canvas, ...icons, ...labels, logo].filter(Boolean),
          {
            opacity: 1,
            scale: 1,
            x: 0,
            y: 0,
            filter: "none",
          }
        );
        centerNodes({ opacity: 1, scale: 1, filter: "none" });
        // No pin gets created on this path, but sections below still wait
        // for this signal (see careers-script.js) â€” fire it regardless.
        window.dispatchEvent(new Event("lifeinside:ready"));
        return;
      }

      createTimeline();
      ScrollTrigger.refresh();
      /* Sections further down the page (CTA parallax, workplace culture
         tile parallax, man-cutout reveal) create their own ScrollTriggers
         only after this fires â€” creating them earlier measures against
         the pre-pin page height, and a later ScrollTrigger.refresh() does
         not correct that mismatch. */
      window.dispatchEvent(new Event("lifeinside:ready"));
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
