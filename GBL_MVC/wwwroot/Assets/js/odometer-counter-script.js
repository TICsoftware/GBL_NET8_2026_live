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
  slots.forEach(slot => {
    const column = slot.querySelector(".odometer-column");
    column.style.transition = "none";
    column.style.transform = "translateY(0px)";
  });
}

/* ---------------------------------------------
   ANIMATE DIGITS TO TARGET
--------------------------------------------- */
function getDigitHeight(slots) {
  const firstLine = slots[0] && slots[0].querySelector(".odometer-digit-line");
  if (!firstLine) return 0;
  // Stats cards scale while pinned — use layout height, not scaled rect
  if (slots[0].closest("[data-stats-scope]")) return firstLine.offsetHeight;
  return firstLine.getBoundingClientRect().height;
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

    const offset = finalIndex * digitHeight;

    setTimeout(() => {
      column.style.transform = `translateY(-${offset}px)`;
    }, 20);
  });
}

/* ---------------------------------------------
   SNAP DIGITS DIRECTLY TO FINAL VALUE (no roll)
--------------------------------------------- */
function snapOdometerSlotsToFinal(slots) {
  if (!slots.length) return;

  const digitHeight = getDigitHeight(slots);

  slots.forEach(slot => {
    const column = slot.querySelector(".odometer-column");
    const offset = slot._finalIndex * digitHeight;
    column.style.transition = "none";
    column.style.transform = `translateY(-${offset}px)`;
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