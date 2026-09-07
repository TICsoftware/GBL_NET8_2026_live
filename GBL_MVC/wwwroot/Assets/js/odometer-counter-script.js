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
function formatNumberString(nStr) {
  const num = Number(nStr);
  if (isNaN(num)) return "0";

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
  const targetStr = formatNumberString(rawTarget);

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
    slot._finalDigit = digit;
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
  slots.forEach(slot => {
    const column = slot.querySelector(".odometer-column");
    column.style.transition = "none";
    column.style.transform = "translate3d(0, 0, 0)";
  });
}

/* ---------------------------------------------
   LOCK INTEGER DIGIT HEIGHTS
   Browser zoom (125%/150%) makes 1em fractional; glyph paint then
   leaks into the clip window as a thin line of the neighbor digit.
--------------------------------------------- */
function lockDigitHeights(slots) {
  if (!slots.length) return 0;

  const probe = slots[0];
  const cs = window.getComputedStyle(probe);
  let h = Math.ceil(parseFloat(cs.fontSize) || 0);
  if (!h) h = Math.ceil(probe.offsetHeight || 0);
  if (!h) return 0;

  slots.forEach((slot) => {
    slot.style.height = h + "px";
    slot.style.lineHeight = h + "px";
    slot.querySelectorAll(".odometer-digit-line").forEach((line) => {
      line.style.height = h + "px";
      line.style.lineHeight = h + "px";
      line.style.fontSize = h + "px";
    });
  });

  return h;
}

/* ---------------------------------------------
   ANIMATE DIGITS TO TARGET
--------------------------------------------- */
function getDigitHeight(slots) {
  return lockDigitHeights(slots);
}

function collapseOdometerSlot(slot) {
  const column = slot.querySelector(".odometer-column");
  if (!column) return;
  const digit = slot._finalDigit;
  if (digit === undefined || digit === null) return;
  column.style.transition = "none";
  column.style.transform = "translate3d(0, 0, 0)";
  column.innerHTML = "";
  const line = document.createElement("span");
  line.className = "odometer-digit-line";
  line.textContent = String(digit);
  const h = slot.style.height || (Math.ceil(parseFloat(getComputedStyle(slot).fontSize)) + "px");
  line.style.height = h;
  line.style.lineHeight = h;
  line.style.fontSize = h;
  column.appendChild(line);
}

function animateOdometerSlots(slots) {
  if (!slots.length) return;

  const digitHeight = getDigitHeight(slots);

  slots.forEach((slot, idx) => {
    const column = slot.querySelector(".odometer-column");
    const finalIndex = slot._finalIndex;

    const duration =
      BASE_DURATION + slot._rolls * DURATION_PER_ROLL;

    const totalSlots = slots.length;
    const staggerFactor = 0.08;
    const delay = Math.round(
      duration * staggerFactor * (totalSlots - idx - 1)
    );

    column.style.transition = `transform ${duration}ms cubic-bezier(.22,.9,.35,1) ${delay}ms`;

    const offset = Math.round(finalIndex * digitHeight);

    setTimeout(() => {
      column.style.transform = `translate3d(0, -${offset}px, 0)`;
    }, 20);

    // After roll settles, keep only the final digit — removes neighbor-digit bleed at zoom
    clearTimeout(slot._collapseTimer);
    slot._collapseTimer = setTimeout(() => {
      collapseOdometerSlot(slot);
    }, delay + duration + 60);
  });
}

/* ---------------------------------------------
   SNAP DIGITS DIRECTLY TO FINAL VALUE (no roll)
--------------------------------------------- */
function snapOdometerSlotsToFinal(slots) {
  if (!slots.length) return;

  getDigitHeight(slots);

  slots.forEach(slot => {
    collapseOdometerSlot(slot);
  });
}

/* ---------------------------------------------
   TRACK SCROLL DIRECTION (top-to-bottom vs
   bottom-to-top)
--------------------------------------------- */
let lastScrollY = window.scrollY || window.pageYOffset;
let scrollDirection = "down"; // assume down for the initial load

window.addEventListener("scroll", function() {
  const currentScrollY = window.scrollY || window.pageYOffset;
  if (currentScrollY > lastScrollY) {
    scrollDirection = "down";
  } else if (currentScrollY < lastScrollY) {
    scrollDirection = "up";
  }
  lastScrollY = currentScrollY;
}, { passive: true });

/* ---------------------------------------------
   INIT WITH INTERSECTION OBSERVER
   -> Roll-up animation only plays when the page
      is scrolling top-to-bottom (element entering
      from below). Scrolling back up (bottom-to-top)
      just reveals the final value instantly, and
      resets the counter so it's ready to animate
      again next time you scroll down to it.

   Counters inside .home-sustainability-section are
   deferred â€” they wait for the section scroll anim
   to reveal content (see PriyaOdometer API below).
--------------------------------------------- */
const counters = document.querySelectorAll(".counter");
const slotsMap = new WeakMap(); // el -> built digit slots
const playedMap = new WeakMap(); // el -> has played this pass
const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

function clearCollapseTimers(slots) {
  if (!slots) return;
  slots.forEach((slot) => {
    if (slot && slot._collapseTimer) {
      clearTimeout(slot._collapseTimer);
      slot._collapseTimer = null;
    }
  });
}

function isCollapsedSlots(slots) {
  if (!slots || !slots.length) return true;
  const col = slots[0].querySelector(".odometer-column");
  if (!col) return true;
  return col.querySelectorAll(".odometer-digit-line").length <= 1;
}

function ensureSlots(el) {
  let slots = slotsMap.get(el);
  if (!slots) {
    slots = buildOdometer(el);
    slotsMap.set(el, slots);
  }
  return slots;
}

function rebuildSlots(el) {
  const prev = slotsMap.get(el);
  clearCollapseTimers(prev);
  slotsMap.delete(el);
  el.textContent = "";
  const slots = buildOdometer(el);
  slotsMap.set(el, slots);
  return slots;
}

function playCounter(el, { animate = true } = {}) {
  if (animate) {
    // Always rebuild full roll strips before animating.
    // Collapsed single-digit columns would translate off-screen (blank).
    const slots = rebuildSlots(el);
    void el.offsetHeight;
    animateOdometerSlots(slots);
  } else {
    let slots = ensureSlots(el);
    if (isCollapsedSlots(slots)) {
      // already final digit — just ensure height lock / visibility
      getDigitHeight(slots);
    } else {
      snapOdometerSlotsToFinal(slots);
    }
  }
  playedMap.set(el, true);
}

function resetCounter(el) {
  // Do NOT leave the counter empty — keep final value visible while
  // scrolling aboutus-stats cards up/down, but allow replay next time.
  clearCollapseTimers(slotsMap.get(el));
  const slots = rebuildSlots(el);
  snapOdometerSlotsToFinal(slots);
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

if (counters.length) {
  const io = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        const el = entry.target;

        // Sustainability counters: driven by homepage scroll timeline
        if (window.PriyaOdometer.isDeferred(el)) return;

        if (entry.isIntersecting) {
          if (reduceMotion || scrollDirection !== "down") {
            playCounter(el, { animate: false });
          } else {
            playCounter(el, { animate: true });
          }
        } else if (!reduceMotion) {
          const statsSection = el.closest("[data-stats-scope]");
          if (statsSection) {
            const rect = statsSection.getBoundingClientRect();
            const stillInView = rect.bottom > 0 && rect.top < window.innerHeight;
            if (stillInView) {
              playCounter(el, { animate: false });
              return;
            }
          }
          resetCounter(el);
        }
      });
    },
    { threshold: 0.3 }
  );

  counters.forEach((c) => {
    if (window.PriyaOdometer.isDeferred(c)) {
      // Pre-build DOM so first play is instant when content appears
      ensureSlots(c);
      return;
    }
    io.observe(c);
  });
}


});