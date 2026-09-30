document.addEventListener("DOMContentLoaded", function() {


/* ---------------------------------------------
   ODOMETER ANIMATION FOR COUNTERS
--------------------------------------------- */
const BASE_ROLLS = 2;              // minimum full 0-9 cycles per digit
const EXTRA_ROLLS_PER_POS = 1;     // extra cycles added towards leftmost digits
const BASE_DURATION = 900;         // ms for rightmost digit
const DURATION_PER_ROLL = 220;     // ms per full 10-digit roll

/* ---------------------------------------------
   FORMAT NUMBER WITH COMMAS
--------------------------------------------- */
function formatNumberString(nStr, locale) {
  const num = Number(nStr);
  if (isNaN(num)) return "0";

  if (locale) {
    try {
      return new Intl.NumberFormat(locale).format(Math.abs(num));
    } catch (e) {
      /* fall through to default */
    }
  }

  const abs = Math.abs(num);
  const [intPartRaw, decPartRaw] = abs.toString().split(".");

  // format integer with commas
  const intPart = intPartRaw.replace(/\B(?=(\d{3})+(?!\d))/g, ",");

  // no decimals -> return integer only
  if (!decPartRaw) return intPart;

  // trim trailing zeros in decimal
  const decPart = decPartRaw.replace(/0+$/, "");

  // all decimals were zero (e.g. .00)
  if (!decPart) return intPart;

  return `${intPart}.${decPart}`;
}

/* ---------------------------------------------
   BUILD ODOMETER DOM (runs once per element)
--------------------------------------------- */
function buildOdometer(counterEl) {
  const rawTarget = counterEl.getAttribute("data-target") || "0";
  const suffix = counterEl.getAttribute("data-suffix") || "";
  const locale = counterEl.getAttribute("data-locale") || "";
  const targetStr = formatNumberString(rawTarget, locale);

  counterEl.textContent = "";

  const odometer = document.createElement("span");
  odometer.className = "counter-odometer";

  const chars = targetStr.split("");

  for (let i = 0; i < chars.length; i++) {
    const char = chars[i];

    /* -------- COMMA / DOT (STATIC) -------- */
    if (char === "," || char === ".") {
      const staticChar = document.createElement("span");
      staticChar.className = "odometer-separator";
      staticChar.textContent = char;
      odometer.appendChild(staticChar);
      continue;
    }

    /* -------- DIGIT (ANIMATED) -------- */
    const digit = parseInt(char, 10);

    const slot = document.createElement("span");
    slot.className = "odometer-digit";

    const column = document.createElement("span");
    column.className = "odometer-column";

    // count numeric digits only (ignore commas)
    const numericIndex =
       chars.slice(i).filter(c => c !== "," && c !== ".").length - 1;

    const rolls = BASE_ROLLS + numericIndex * EXTRA_ROLLS_PER_POS;

    for (let r = 0; r <= rolls; r++) {
      for (let d = 0; d < 10; d++) {
        const line = document.createElement("span");
        line.className = "odometer-digit-line";
        line.textContent = d;
        column.appendChild(line);
      }
    }

    slot.appendChild(column);
    odometer.appendChild(slot);

    slot._finalIndex = rolls * 10 + digit;
    slot._rolls = rolls;
  }

  /* -------- SUFFIX (STATIC) -------- */
  if (suffix) {
    const suf = document.createElement("span");
    suf.className = "counter-suffix";
    suf.textContent = suffix;
    odometer.appendChild(suf);
  }

  counterEl.appendChild(odometer);

  return Array.from(odometer.querySelectorAll(".odometer-digit"));
}

/* ---------------------------------------------
   RESET DIGITS BACK TO START (no transition)
--------------------------------------------- */
function resetOdometerSlots(slots) {
  slots.forEach(function (slot) {
    const column = slot.querySelector(".odometer-column");
    column.style.transition = "none";
    column.style.transform = "translateY(0)";
  });
}

function animateOdometerSlots(slots) {
  if (!slots.length) return;

  slots.forEach(function (slot, idx) {
    const column = slot.querySelector(".odometer-column");
    const duration = BASE_DURATION + slot._rolls * DURATION_PER_ROLL;
    const delay = Math.round(duration * 0.08 * (slots.length - idx - 1));
    column.style.transition =
      "transform " + duration + "ms cubic-bezier(.22,.9,.35,1) " + delay + "ms";
    column.style.transform = "translateY(-" + slot._finalIndex + "em)";
  });
}

function snapOdometerSlotsToFinal(slots) {
  if (!slots.length) return;

  slots.forEach(function (slot) {
    const column = slot.querySelector(".odometer-column");
    column.style.transition = "none";
    column.style.transform = "translateY(-" + slot._finalIndex + "em)";
  });
}

function getScrollY() {
  if (window.lenis && typeof window.lenis.scroll === "number") return window.lenis.scroll;
  return window.scrollY || document.documentElement.scrollTop || 0;
}

let lastScrollY = getScrollY();
let scrollDirection = "down";

function onScrollDir() {
  const currentScrollY = getScrollY();
  if (currentScrollY > lastScrollY + 0.5) scrollDirection = "down";
  else if (currentScrollY < lastScrollY - 0.5) scrollDirection = "up";
  lastScrollY = currentScrollY;
}

window.addEventListener("scroll", onScrollDir, { passive: true });
document.documentElement.addEventListener("scroll", onScrollDir, { passive: true });
if (window.lenis && typeof window.lenis.on === "function") {
  window.lenis.on("scroll", onScrollDir);
} else {
  window.addEventListener(
    "load",
    function () {
      if (window.lenis && typeof window.lenis.on === "function") {
        window.lenis.on("scroll", onScrollDir);
      }
    },
    { once: true }
  );
}

/* ---------------------------------------------
   INIT WITH INTERSECTION OBSERVER
   -> Roll-up animation only plays when the page
      is scrolling top-to-bottom (element entering
      from below). Scrolling back up (bottom-to-top)
      just reveals the final value instantly, and
      resets the counter so it's ready to animate
      again next time you scroll down to it.

   Counters inside .home-sustainability-section are
   deferred — they wait for the section scroll anim
   to reveal content (see PriyaOdometer API below).
--------------------------------------------- */
const counters = document.querySelectorAll(".counter");
const slotsMap = new WeakMap(); // el -> built digit slots
const playedMap = new WeakMap(); // el -> has played this pass
const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

function ensureSlots(el) {
  let slots = slotsMap.get(el);
  if (!slots) {
    slots = buildOdometer(el);
    slotsMap.set(el, slots);
  }
  return slots;
}

function playCounter(el, { animate = true } = {}) {
  if (playedMap.get(el)) return;
  const slots = ensureSlots(el);
  if (animate) {
    resetOdometerSlots(slots);
    void el.offsetHeight;
    animateOdometerSlots(slots);
  } else {
    snapOdometerSlotsToFinal(slots);
  }
  playedMap.set(el, true);
}

function resetCounter(el) {
  const slots = slotsMap.get(el);
  if (!slots) return;
  resetOdometerSlots(slots);
  playedMap.set(el, false);
}

/** Public API for scroll-synced sections (sustainability, etc.) */
window.PriyaOdometer = {
  play(root, opts) {
    if (!root) return;
    root.querySelectorAll(".counter").forEach((el) => {
      if (playedMap.get(el)) return;
      playCounter(el, opts);
    });
  },
  reset(root) {
    if (!root) return;
    root.querySelectorAll(".counter").forEach((el) => resetCounter(el));
  },
  isDeferred(el) {
      // Only defer while the scroll expand anim is active
      return !!(el && el.closest(".home-sustainability-section.is-sustain-anim"));
    },
};

function bindCounter(el) {
  if (window.PriyaOdometer.isDeferred(el)) {
    ensureSlots(el);
    return;
  }

  ensureSlots(el);

  const isMobile = window.matchMedia("(max-width: 992px)").matches;
  const stScroller = isMobile ? window : document.documentElement;
  const triggerEl = el.closest(".giveback-cell") || el;

  if (typeof gsap !== "undefined" && typeof ScrollTrigger !== "undefined") {
    gsap.registerPlugin(ScrollTrigger);
    ScrollTrigger.create({
      trigger: triggerEl,
      scroller: stScroller,
      start: "top 72%",
      end: "bottom 12%",
      invalidateOnRefresh: true,
      onEnter: function () {
        playCounter(el, { animate: !reduceMotion && scrollDirection === "down" });
      },
      onEnterBack: function () {
        playCounter(el, { animate: false });
      },
      onLeaveBack: function () {
        if (!reduceMotion) resetCounter(el);
      },
    });
    return;
  }

  const io = new IntersectionObserver(
    function (entries) {
      entries.forEach(function (entry) {
        if (window.PriyaOdometer.isDeferred(el)) return;
        if (entry.isIntersecting) {
          playCounter(el, {
            animate: !reduceMotion && scrollDirection === "down",
          });
        } else if (!reduceMotion) {
          resetCounter(el);
        }
      });
    },
    { threshold: 0.35 }
  );
  io.observe(triggerEl);
}

if (counters.length) {
  counters.forEach(bindCounter);
  window.addEventListener("load", function () {
    if (typeof ScrollTrigger !== "undefined") ScrollTrigger.refresh();
  });
}


});