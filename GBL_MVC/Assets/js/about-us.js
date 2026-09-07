document.addEventListener("DOMContentLoaded", () => {



   

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

  // no decimals Ã¢â€ â€™ return integer only
  if (!decPartRaw) return intPart;

  // trim trailing zeros in decimal
  const decPart = decPartRaw.replace(/0+$/, "");

  // all decimals were zero (e.g. .00)
  if (!decPart) return intPart;

  return `${intPart}.${decPart}`;
}

/* ---------------------------------------------
   BUILD ODOMETER DOM
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

    /* -------- COMMA (STATIC) -------- */
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
   ANIMATE DIGITS
--------------------------------------------- */
function animateOdometerSlots(slots) {
  if (!slots.length) return;

  const firstLine = slots[0].querySelector(".odometer-digit-line");
  const digitHeight = firstLine
    ? firstLine.getBoundingClientRect().height
    : 0;

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
    column.style.transform = "translateY(0px)";

    const offset = finalIndex * digitHeight;

    setTimeout(() => {
      column.style.transform = `translateY(-${offset}px)`;
    }, 20);
  });
}

/* ---------------------------------------------
   INIT WITH INTERSECTION OBSERVER
--------------------------------------------- */
const counters = document.querySelectorAll(".counter");
const stateMap = new WeakMap();

if (counters.length) {
  const io = new IntersectionObserver(
    (entries, obs) => {
      entries.forEach(entry => {
        if (!entry.isIntersecting) return;

        const el = entry.target;
        if (stateMap.get(el)) {
          obs.unobserve(el);
          return;
        }

        const slots = buildOdometer(el);
        void el.offsetHeight; // force reflow
        animateOdometerSlots(slots);

        stateMap.set(el, true);
        obs.unobserve(el);
      });
    },
    { threshold: 0.3 }
  );

  counters.forEach(c => io.observe(c));
}

});


document.addEventListener('DOMContentLoaded', () => {
  const wrapper = document.querySelector('.map-outer-warapper');
  const states = document.querySelectorAll('.map-state');
  const items = document.querySelectorAll('.map-item');

  // ---------- Helpers ----------
  function getItemByLocation(location) {
    return Array.from(items).find(
      (item) => item.querySelector('.map-hover')?.getAttribute('data-location') === location
    );
  }

  function getStateByLocation(location) {
    return Array.from(states).find(
      (state) => state.getAttribute('data-location') === location
    );
  }

  function clearAllActive() {
    states.forEach((s) => s.classList.remove('active'));
    items.forEach((i) => i.classList.remove('active', 'linked-active'));
    wrapper.classList.remove('has-active');
  }

  function activateLocation(location) {
    clearAllActive();

    const state = getStateByLocation(location);
    const item = getItemByLocation(location);

    if (state) state.classList.add('active');   // <-- map-state gets "active" class here
    if (item) item.classList.add('linked-active');

    wrapper.classList.add('has-active');
  }

  // ---------- 1. Position line + label for every map-item ----------
items.forEach((item) => {
  const circle = item.querySelector('.map-hover');
  const line = item.querySelector('.map-line');
  const labelGroup = item.querySelector('.map-label-group');
  const labelBg = item.querySelector('.map-label-bg');
  const labelText = item.querySelector('.map-label');

  if (!circle || !line || !labelGroup || !labelBg || !labelText) return;

  const cx = parseFloat(circle.getAttribute('cx'));
  const cy = parseFloat(circle.getAttribute('cy'));
  const lx = parseFloat(item.getAttribute('data-lx')) || cx;
  const ly = parseFloat(item.getAttribute('data-ly')) || cy - 40;

  const textLength = labelText.getComputedTextLength();
  const paddingX = 16;
  const minWidth = 40;
  const labelHeight = parseFloat(labelBg.getAttribute('height')) || 25;
  const labelWidth = Math.max(textLength + paddingX, minWidth);

  line.setAttribute('d', `M${cx},${cy} L${lx},${ly}`);

  // NEW: decide which side the label falls on, and anchor the box accordingly
  const isRightSide = lx >= cx;
  const boxX = isRightSide ? lx : lx - labelWidth;

  labelGroup.setAttribute(
    'transform',
    `translate(${boxX}, ${ly - labelHeight / 2})`
  );
  labelBg.setAttribute('x', 0);
  labelBg.setAttribute('y', 0);
  labelBg.setAttribute('width', labelWidth);
  labelBg.setAttribute('height', labelHeight);
  labelText.setAttribute('x', labelWidth / 2);
  labelText.setAttribute('y', labelHeight / 2);
});

  // ---------- 2. Pin hover -> highlight linked state ----------
  items.forEach((item) => {
    const circle = item.querySelector('.map-hover');
    const location = circle?.getAttribute('data-location');
    if (!location) return;

    item.addEventListener('mouseenter', () => {
      const state = getStateByLocation(location);
      if (state) state.classList.add('hover-linked');
    });

    item.addEventListener('mouseleave', () => {
      const state = getStateByLocation(location);
      if (state) state.classList.remove('hover-linked');
    });
  });

  // ---------- 3. State hover -> highlight linked pin/line/label ----------
  states.forEach((state) => {
    const location = state.getAttribute('data-location');

    state.addEventListener('mouseenter', () => {
      const item = getItemByLocation(location);
      if (item) item.classList.add('linked-hover');
    });

    state.addEventListener('mouseleave', () => {
      const item = getItemByLocation(location);
      if (item) item.classList.remove('linked-hover');
    });
  });

  // ---------- 4. Click on state OR pin -> toggle active class for both ----------
  states.forEach((state) => {
    state.addEventListener('click', () => {
      const location = state.getAttribute('data-location');
      const isAlreadyActive = state.classList.contains('active');

      if (isAlreadyActive) {
        clearAllActive();
      } else {
        activateLocation(location);
        console.log('Selected:', location);
      }
    });
  });

  items.forEach((item) => {
    const circle = item.querySelector('.map-hover');
    if (!circle) return;

    circle.addEventListener('click', () => {
      const location = circle.getAttribute('data-location');
      const isAlreadyActive = item.classList.contains('linked-active');

      if (isAlreadyActive) {
        clearAllActive();
      } else {
        activateLocation(location);
        console.log('Selected:', location);
      }
    });
  });
});