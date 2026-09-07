document.addEventListener('DOMContentLoaded', () => {
  const wrapper = document.querySelector('.map-outer-warapper');
  const states = document.querySelectorAll('.map-state');
  const items = document.querySelectorAll('.map-item');

  // Touch/no-mouse devices get inconsistent synthetic mouseenter events
  // for SVG elements (especially plain <path> state shapes) — rather
  // than chase that, skip hover-driven open/close there entirely and
  // rely solely on the click handlers (step 4), which map reliably to
  // touchend on mobile. Desktop/mouse devices are unaffected.
  const isTouchDevice = window.matchMedia('(hover: none)').matches;

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

  // Tracks whichever location is "sticky" (click-selected, or the
  // default-open one) — the only thing that keeps the red .active fill
  // and the has-active dimming of other states after the mouse moves
  // away. Read/cleared by showInfoCard() so hovering a different pin
  // doesn't leave the old sticky one stuck highlighted underneath it.
  let stickyActiveLocation = null;

  function clearAllActive() {
    states.forEach((s) => s.classList.remove('active'));
    items.forEach((i) => i.classList.remove('active', 'linked-active'));
    wrapper.classList.remove('has-active');
    stickyActiveLocation = null;
  }

  function activateLocation(location) {
    clearAllActive();

    const state = getStateByLocation(location);
    const item = getItemByLocation(location);

    if (state) state.classList.add('active');   // <-- map-state gets "active" class here
    if (item) item.classList.add('linked-active');

    wrapper.classList.add('has-active');
    stickyActiveLocation = location;
  }

  // ---------- Hover/tap info card ----------
  // Card content is static HTML per region (written directly in the
  // .cshtml, tagged data-location="..."). This script only ever shows,
  // hides and positions whichever card matches the hovered pin/state —
  // it never builds or fills in markup.
  const mapInsideWrapper = document.querySelector('.map-inside-wrapper');
  const infoCards = document.querySelectorAll('.map-info-card');
  const svgEl = mapInsideWrapper && mapInsideWrapper.querySelector('svg');
  let hideCardTimer = null;
  let activeInfoCard = null;
  let linkedState = null; // the .map-state currently carrying "hover-linked", kept in sync with the card
  let openItem = null; // the .map-item currently carrying "card-open", kept in sync with the card
  let lastPointer = { x: 0, y: 0 };
  // After X / click-close, ignore hover reopen until the pointer leaves
  // that location (Odisha card sits over the state, so hide instantly
  // re-fires mouseenter on the shape underneath).
  let closeLockLocation = null;

  if (mapInsideWrapper) {
    mapInsideWrapper.addEventListener('mousemove', (e) => {
      lastPointer.x = e.clientX;
      lastPointer.y = e.clientY;
    }, { passive: true });
  }

  function getInfoCardByLocation(location) {
    return Array.from(infoCards).find(
      (card) => card.getAttribute('data-location') === location
    );
  }

  function positionInfoCard(card, circle) {
    if (!card || !svgEl || !mapInsideWrapper) return;

    card.classList.remove(
      'is-docked', 'is-dock-tr', 'is-dock-tl', 'is-dock-br', 'is-dock-bl',
      'is-side-right', 'is-side-left', 'is-above', 'is-below',
      'is-mobile-centered'
    );

    // Mobile: center card in the map area (not beside the pin)
    if (window.matchMedia('(max-width: 992px)').matches) {
      // Apply center transform without transitioning, or it slides in from the right
      card.style.setProperty('transition', 'none', 'important');
      card.classList.add('is-mobile-centered');
      card.style.left = '';
      card.style.top = '';
      card.style.transform = '';
      void card.offsetWidth;
      card.style.removeProperty('transition');
      return;
    }

    const cx = parseFloat(circle.getAttribute('cx'));
    const cy = parseFloat(circle.getAttribute('cy'));
    const svgRect = svgEl.getBoundingClientRect();
    const wrapRect = mapInsideWrapper.getBoundingClientRect();
    const viewBox = svgEl.viewBox.baseVal;
    const scale = svgRect.width / (viewBox.width || 689);

    const pinX = svgRect.left - wrapRect.left + cx * scale;
    const pinY = svgRect.top - wrapRect.top + cy * scale;

    const cardWidth = card.offsetWidth || 240;
    const cardHeight = card.offsetHeight || 200;
    const pad = 12;
    const gap = 40;

    const canRight = pinX + gap + cardWidth <= wrapRect.width - pad;
    const canLeft = pinX - gap - cardWidth >= pad;

    let left;
    let side;

    if (canRight) {
      left = pinX + gap;
      side = 'side-right';
    } else if (canLeft) {
      left = pinX - cardWidth - gap;
      side = 'side-left';
    } else {
      left = Math.max(pad, Math.min(pinX + gap, wrapRect.width - cardWidth - pad));
      side = left >= pinX ? 'side-right' : 'side-left';
    }

    // Pin sits just outside the top of the card — nearby, not covering it
    let top = pinY - 40;
    if (top < pad) top = pad;
    if (top + cardHeight > wrapRect.height - pad) {
      top = Math.max(pad, wrapRect.height - cardHeight - pad);
    }

    const coversPin =
      pinX >= left && pinX <= left + cardWidth &&
      pinY >= top && pinY <= top + cardHeight;

    if (coversPin) {
      if (pinY - gap - cardHeight >= pad) {
        top = pinY - cardHeight - gap;
        left = Math.max(pad, Math.min(pinX - 48, wrapRect.width - cardWidth - pad));
        side = 'above';
      } else {
        top = Math.min(pinY + gap, Math.max(pad, wrapRect.height - cardHeight - pad));
        left = Math.max(pad, Math.min(pinX - 48, wrapRect.width - cardWidth - pad));
        side = 'below';
      }
    }

    card.classList.add(`is-${side}`);
    card.style.left = left + 'px';
    card.style.top = top + 'px';
    card.style.transform = '';
  }

  // Single source of truth: .map-state carries "hover-linked" exactly
  // when its linked info card is visible — no matter whether a pin
  // hover, a state hover, or a click is what opened it.
  function linkStateToCard(location) {
    const state = getStateByLocation(location) || null;
    if (linkedState && linkedState !== state) {
      linkedState.classList.remove('hover-linked');
    }
    linkedState = state;
    if (linkedState) linkedState.classList.add('hover-linked');
  }

  function unlinkState() {
    if (!linkedState) return;
    linkedState.classList.remove('hover-linked');
    linkedState = null;
  }

  // Same idea, but on the pin itself: guarantees the dot gets a visibly
  // different fill (see .map-item.card-open .map-hover in map-style.css)
  // whenever its card is open — instead of depending on whichever of
  // :hover / linked-hover / active happened to also be set.
  function markItemOpen(item) {
    if (openItem && openItem !== item) {
      openItem.classList.remove('card-open');
    }
    openItem = item;
    if (openItem) openItem.classList.add('card-open');
    if (wrapper) wrapper.classList.toggle('has-open-card', !!openItem);
  }

  function unmarkItemOpen() {
    if (openItem) openItem.classList.remove('card-open');
    openItem = null;
    if (wrapper) wrapper.classList.remove('has-open-card');
  }

  function eventTargetIsOpenUi(target) {
    if (!target || target.nodeType !== 1) {
      target = target && target.parentElement;
    }
    if (!target) return false;
    if (activeInfoCard && (activeInfoCard === target || activeInfoCard.contains(target))) return true;
    if (openItem && (openItem === target || openItem.contains(target))) return true;
    if (linkedState && (linkedState === target || linkedState.contains(target))) return true;
    return false;
  }

  function isPointerStillInside() {
    try {
      if (openItem && openItem.matches(':hover')) return true;
      if (activeInfoCard && activeInfoCard.matches(':hover')) return true;
      if (linkedState && linkedState.matches(':hover')) return true;
    } catch (err) { /* :hover on detached/SVG is fine to ignore */ }
    return false;
  }

  function rectContainsPoint(rect, x, y, pad) {
    return (
      x >= rect.left - pad &&
      x <= rect.right + pad &&
      y >= rect.top - pad &&
      y <= rect.bottom + pad
    );
  }

  function isPointerNearOpenUi() {
    if (isPointerStillInside()) return true;
    const x = lastPointer.x;
    const y = lastPointer.y;
    const pad = 44;
    if (activeInfoCard && rectContainsPoint(activeInfoCard.getBoundingClientRect(), x, y, pad)) {
      return true;
    }
    if (openItem) {
      const hit = openItem.querySelector('.map-hit') || openItem.querySelector('.map-hover');
      if (hit && rectContainsPoint(hit.getBoundingClientRect(), x, y, pad)) return true;
    }
    return false;
  }

  function hideInfoCardNow() {
    clearTimeout(hideCardTimer);
    if (activeInfoCard) {
      const card = activeInfoCard;
      // Keep is-mobile-centered during fade so the card doesn't jump to 0,0
      card.classList.remove('is-visible');
      card.setAttribute('aria-hidden', 'true');
      activeInfoCard = null;

      let cleaned = false;
      const cleanup = () => {
        if (cleaned) return;
        cleaned = true;
        card.classList.remove('is-mobile-centered');
        card.removeEventListener('transitionend', onEnd);
      };
      const onEnd = (e) => {
        if (e.target !== card) return;
        if (e.propertyName !== 'opacity') return;
        cleanup();
      };
      card.addEventListener('transitionend', onEnd);
      setTimeout(cleanup, 280);
    }
    unlinkState();
    unmarkItemOpen();
  }

  function lockClose(location) {
    closeLockLocation = location || true;
  }

  function clearCloseLock() {
    closeLockLocation = null;
  }

  function showInfoCard(item) {
    const circle = item.querySelector('.map-hover');
    const location = circle && circle.getAttribute('data-location');
    const card = location && getInfoCardByLocation(location);
    if (!card) return;

    if (closeLockLocation) {
      if (closeLockLocation === true || closeLockLocation === location) return;
      clearCloseLock();
    }

    // A different location is being shown (hover or click) — drop the
    // old sticky/default-open highlight so it doesn't stay lit up
    // underneath whatever's being previewed now.
    if (stickyActiveLocation && stickyActiveLocation !== location) {
      clearAllActive();
    }

    if (activeInfoCard && activeInfoCard !== card) {
      const prev = activeInfoCard;
      prev.classList.remove('is-visible');
      prev.setAttribute('aria-hidden', 'true');
      // Drop center class after fade so it doesn't flash at 0,0
      setTimeout(() => {
        if (prev !== activeInfoCard) prev.classList.remove('is-mobile-centered');
      }, 280);
    }

    clearTimeout(hideCardTimer);
    activeInfoCard = card;
    // Position while still invisible so the card never flashes at 0,0
    // or gets is-visible with a wrong off-map top/left.
    positionInfoCard(card, circle);
    linkStateToCard(location);
    markItemOpen(item);

    const reveal = () => {
      card.classList.add('is-visible');
      card.setAttribute('aria-hidden', 'false');
    };
    // Next frame so mobile center transform is settled before open transition
    if (card.classList.contains('is-mobile-centered')) {
      requestAnimationFrame(reveal);
    } else {
      reveal();
    }
  }

  function scheduleHideInfoCard(e) {
    if (e && eventTargetIsOpenUi(e.relatedTarget)) return;
    clearTimeout(hideCardTimer);
    hideCardTimer = setTimeout(() => {
      if (isPointerNearOpenUi()) return;
      hideInfoCardNow();
    }, 250);
  }

  if (infoCards.length) {
    infoCards.forEach((card) => {
      const flag = card.querySelector('.map-info-flag');
      if (flag && !flag.querySelector('.map-info-close')) {
        const closeBtn = document.createElement('button');
        closeBtn.type = 'button';
        closeBtn.className = 'map-info-close';
        closeBtn.setAttribute('aria-label', 'Close');
        closeBtn.innerHTML = '<svg viewBox="0 0 12 12" fill="none" aria-hidden="true"><path d="M1 1l10 10M11 1L1 11" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>';
        closeBtn.addEventListener('click', (e) => {
          e.preventDefault();
          e.stopPropagation();
          lockClose(card.getAttribute('data-location'));
          clearAllActive();
          hideInfoCardNow();
        });
        flag.appendChild(closeBtn);
      }

      card.addEventListener('mouseenter', () => clearTimeout(hideCardTimer));
      card.addEventListener('mouseleave', scheduleHideInfoCard);
    });

    if (mapInsideWrapper && !isTouchDevice) {
      mapInsideWrapper.addEventListener('mouseleave', (e) => {
        clearCloseLock();
        scheduleHideInfoCard(e);
      });
    }

    // Tap outside (mobile) closes the open card
    document.addEventListener('click', (e) => {
      if (!activeInfoCard) return;
      if (
        activeInfoCard.contains(e.target) ||
        e.target.closest('.map-item') ||
        e.target.closest('.map-state')
      ) return;
      hideInfoCardNow();
    });

    items.forEach((item) => {
      const circle = item.querySelector('.map-hover');
      const location = circle && circle.getAttribute('data-location');
      if (location && getInfoCardByLocation(location)) {
        item.classList.add('has-info-card');
      }
    });
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

  // Stable hit target: pulse animates stroke-width on .map-hover, which
  // would otherwise fire spurious mouseleave. Visual circle ignores
  // pointer events; this transparent circle does not animate.
  items.forEach((item) => {
    const circle = item.querySelector('.map-hover');
    if (!circle) return;
    let hit = item.querySelector('.map-hit');
    if (!hit) {
      hit = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
      hit.classList.add('map-hit');
      hit.setAttribute('fill', 'transparent');
      item.insertBefore(hit, circle);
    }
    hit.setAttribute('cx', circle.getAttribute('cx'));
    hit.setAttribute('cy', circle.getAttribute('cy'));
    const svgW = svgEl ? svgEl.getBoundingClientRect().width : 689;
    const scale = svgW / 689;
    hit.setAttribute('r', String(Math.max(18, 22 / (scale || 1))));
  });

  // ---------- 2. Pin hover -> highlight linked state ----------
  // (desktop/mouse only — see isTouchDevice note above)
  if (!isTouchDevice) items.forEach((item) => {
    const circle = item.querySelector('.map-hover');
    const location = circle?.getAttribute('data-location');
    if (!location) return;

    item.addEventListener('mouseenter', () => {
      showInfoCard(item); // also links/unlinks the matching .map-state's hover-linked class
    });

    item.addEventListener('mouseleave', (e) => {
      clearCloseLock();
      scheduleHideInfoCard(e);
    });
  });

  // ---------- 3. State hover -> highlight linked pin/line/label ----------
  // (desktop/mouse only — see isTouchDevice note above)
  if (!isTouchDevice) states.forEach((state) => {
    const location = state.getAttribute('data-location');

    state.addEventListener('mouseenter', () => {
      const item = getItemByLocation(location);
      if (item) {
        item.classList.add('linked-hover');
        showInfoCard(item);
      }
    });

    state.addEventListener('mouseleave', (e) => {
      const item = getItemByLocation(location);
      if (item) item.classList.remove('linked-hover');
      clearCloseLock();
      scheduleHideInfoCard(e);
    });
  });

  // ---------- 4. Click on state OR pin -> toggle active class for both ----------
  states.forEach((state) => {
    state.addEventListener('click', () => {
      const location = state.getAttribute('data-location');
      const isAlreadyActive = state.classList.contains('active');

      if (isAlreadyActive) {
        lockClose(location);
        clearAllActive();
        hideInfoCardNow();
      } else {
        clearCloseLock();
        activateLocation(location);
        const item = getItemByLocation(location);
        if (item) showInfoCard(item);
        console.log('Selected:', location);
      }
    });
  });

  items.forEach((item) => {
    const circle = item.querySelector('.map-hover');
    if (!circle) return;

    // Click on the item (hit circle), not .map-hover — that circle has
    // pointer-events:none so the pulse stroke cannot steal hover.
    item.addEventListener('click', (e) => {
      e.stopPropagation();
      const location = circle.getAttribute('data-location');
      const isAlreadyActive = item.classList.contains('linked-active');

      if (isAlreadyActive) {
        lockClose(location);
        clearAllActive();
        hideInfoCardNow();
      } else {
        clearCloseLock();
        activateLocation(location);
        showInfoCard(item);
        console.log('Selected:', location);
      }
    });
  });

  // ---------- 5. Default-open pin ----------
  // Telangana starts pre-selected, exactly as if it had been clicked —
  // stays open until the user clicks it again, clicks elsewhere, or
  // hovers a different pin/state.
  const DEFAULT_OPEN_LOCATION = 'Telangana';
  if (DEFAULT_OPEN_LOCATION) {
    const defaultItem = getItemByLocation(DEFAULT_OPEN_LOCATION);
    if (defaultItem) {
      activateLocation(DEFAULT_OPEN_LOCATION);
      showInfoCard(defaultItem);
    }
  }

  window.addEventListener('resize', () => {
    if (!activeInfoCard || !openItem) return;
    const circle = openItem.querySelector('.map-hover');
    if (circle) positionInfoCard(activeInfoCard, circle);
  }, { passive: true });
});