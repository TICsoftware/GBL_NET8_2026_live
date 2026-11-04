/**
 * Leadership page — director cards
 * Same hover fill and card rise as the CSR focus cards. The grid runs to several
 * rows, so each card has its own scroll trigger and appears as it reaches the
 * viewport, with cards further along a row starting slightly later.
 */
document.addEventListener("DOMContentLoaded", function () {
  var grid = document.querySelector(".board-grid");
  if (!grid) return;

  var cards = Array.from(grid.querySelectorAll(".leader-card"));
  if (!cards.length) return;

  function placeFill(card, e) {
    var media = card.querySelector(".leader-card__media");
    var cardRect = card.getBoundingClientRect();
    var mediaRect = media ? media.getBoundingClientRect() : null;
    var top = mediaRect ? Math.max(0, mediaRect.bottom - cardRect.top) : cardRect.height * 0.8;
    var fillW = cardRect.width;
    var fillH = Math.max(0, cardRect.height - top);
    var x = e ? e.clientX - cardRect.left : fillW / 2;
    var y = e ? e.clientY - cardRect.top - top : fillH / 2;
    x = Math.min(Math.max(x, 0), fillW);
    y = Math.min(Math.max(y, 0), fillH);
    var radius = Math.ceil(Math.hypot(Math.max(x, fillW - x), Math.max(y, fillH - y))) + 16;
    card.style.setProperty("--fill-top", top + "px");
    card.style.setProperty("--hx", x + "px");
    card.style.setProperty("--hy", y + "px");
    card.style.setProperty("--hr", radius + "px");
  }

  cards.forEach(function (card) {
    placeFill(card, null);
    card.addEventListener("mouseenter", function (e) {
      placeFill(card, e);
      card.classList.add("is-filled");
    });
    card.addEventListener("mouseleave", function () {
      card.classList.remove("is-filled");
    });
  });

  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

  gsap.registerPlugin(ScrollTrigger);

  var stScroller = window.matchMedia("(max-width: 992px)").matches
    ? window
    : document.documentElement;
  var CARD_RISE = { y: 140, scrub: 1.15, start: 92, end: 58, columnDelay: 5 };

  /** Position in the row, read on each refresh so it follows the responsive columns. */
  function columnOf(card) {
    var column = 0;
    while (cards[cards.indexOf(card) - column - 1] &&
      cards[cards.indexOf(card) - column - 1].offsetTop === card.offsetTop) {
      column++;
    }
    return column;
  }

  cards.forEach(function (card) {
    gsap.fromTo(
      card,
      { autoAlpha: 0, y: CARD_RISE.y },
      {
        autoAlpha: 1,
        y: 0,
        ease: "power2.out",
        force3D: true,
        immediateRender: true,
        scrollTrigger: {
          trigger: card,
          scroller: stScroller,
          start: function () {
            return "top " + (CARD_RISE.start - columnOf(card) * CARD_RISE.columnDelay) + "%";
          },
          end: function () {
            return "top " + (CARD_RISE.end - columnOf(card) * CARD_RISE.columnDelay) + "%";
          },
          scrub: CARD_RISE.scrub,
          invalidateOnRefresh: true,
        },
      }
    );
  });

  /* Photos settle from a slight zoom as the card scrolls in. Photos are cut-outs
     ending at the card edge, so sliding them like the chairman portrait would
     leave a gap under the shoulders. */
  cards.forEach(function (card) {
    var photo = card.querySelector(".leader-card__photo");
    if (!photo) return;
    gsap.fromTo(
      photo,
      { scale: 1.14 },
      {
        scale: 1,
        ease: "none",
        force3D: true,
        scrollTrigger: {
          trigger: card,
          scroller: stScroller,
          start: "top bottom",
          end: "top 25%",
          scrub: 0.8,
          invalidateOnRefresh: true,
        },
      }
    );
  });

  window.addEventListener("load", function () {
    ScrollTrigger.refresh();
  });
});

/**
 * Leader profile modal
 * One shared <dialog>: each card or the chairman block keeps its own bio, and
 * the clicked person's name, role, photo and bio are copied in before it opens.
 */
document.addEventListener("DOMContentLoaded", function () {
  var modal = document.getElementById("leader-modal");
  if (!modal || typeof modal.showModal !== "function") return;

  var panel = modal.querySelector(".leader-modal__panel");
  var backdrop = modal.querySelector(".leader-modal__backdrop");
  var scroller = modal.querySelector(".leader-modal__scroll");
  var photo = modal.querySelector("[data-leader-modal-photo]");
  var nameEl = modal.querySelector("[data-leader-modal-name]");
  var roleEl = modal.querySelector("[data-leader-modal-role]");
  var bioEl = modal.querySelector("[data-leader-modal-bio]");
  var closing = false;
  var closeTimer = 0;

  function lockPage() {
    document.documentElement.style.overflow = "hidden";
    if (window.lenis && typeof window.lenis.stop === "function") {
      window.lenis.stop();
    }
  }

  function unlockPage() {
    if (window.lenis && typeof window.lenis.start === "function") {
      window.lenis.start();
    }
    document.documentElement.style.overflow = "";
  }

  function fill(leader) {
    var image = leader.querySelector("[data-leader-photo]");
    photo.src = image.currentSrc || image.src;
    photo.alt = image.alt;
    nameEl.textContent = leader.querySelector("[data-leader-name]").textContent;
    roleEl.textContent = leader.querySelector("[data-leader-role]").textContent;
    bioEl.innerHTML = leader.querySelector("[data-leader-bio]").innerHTML;
  }

  function stopMotion() {
    if (typeof gsap === "undefined") return;
    gsap.killTweensOf(panel);
    gsap.killTweensOf(backdrop);
  }

  function finishClose() {
    window.clearTimeout(closeTimer);
    closing = false;
    stopMotion();
    if (modal.open) modal.close();
    modal.classList.remove("is-closing", "is-open");
    if (typeof gsap !== "undefined") {
      gsap.set(panel, { clearProps: "transform" });
      gsap.set(backdrop, { clearProps: "opacity" });
    }
    panel.style.transition = "";
    panel.style.transform = "";
    panel.style.opacity = "";
    unlockPage();
  }

  function openModal(leader) {
    window.clearTimeout(closeTimer);
    closing = false;
    stopMotion();
    fill(leader);
    modal.classList.toggle("is-card", leader.classList.contains("leader-card"));
    if (!modal.open) {
      modal.showModal();
      lockPage();
    }
    scroller.scrollTop = 0;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
      if (typeof gsap !== "undefined") {
        gsap.set(panel, { clearProps: "transform" });
        gsap.set(backdrop, { clearProps: "opacity" });
      }
      modal.classList.remove("is-closing");
      modal.classList.add("is-open");
      return;
    }
    /* Drop any leftover pose, then slide in from the top. */
    panel.style.transition = "";
    panel.style.transform = "";
    if (typeof gsap !== "undefined") gsap.set(panel, { clearProps: "transform" });
    modal.classList.remove("is-closing", "is-open");
    if (typeof gsap !== "undefined") gsap.set(backdrop, { opacity: 0 });
    void panel.offsetWidth;
    requestAnimationFrame(function () {
      if (closing || !modal.open) return;
      modal.classList.add("is-open");
      if (typeof gsap === "undefined") return;
      gsap.to(backdrop, { opacity: 1, duration: 0.7, ease: "power2.out", overwrite: true });
    });
  }

  function closeModal() {
    if (!modal.open || closing) return;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches || !modal.classList.contains("is-open") || typeof gsap === "undefined") {
      finishClose();
      return;
    }
    closing = true;
    stopMotion();
    /* Hold the resting pose, then ease the panel and the overlay out together. */
    gsap.set(panel, { y: 0 });
    gsap.set(backdrop, { opacity: 1 });
    modal.classList.add("is-closing");
    modal.classList.remove("is-open");
    var done = false;
    function onEnd() {
      if (done) return;
      done = true;
      finishClose();
    }
    var exit = gsap.timeline({ onComplete: onEnd });
    exit.to(panel, { y: "100vh", duration: 0.8, ease: "power2.inOut" }, 0);
    exit.to(backdrop, { opacity: 0, duration: 0.8, ease: "power2.in" }, 0);
    closeTimer = window.setTimeout(onEnd, 960);
  }

  /* The whole card is clickable; its name button also covers keyboard users,
     since the button's click bubbles up to the card. */
  document.querySelectorAll(".leader-card[data-leader]").forEach(function (card) {
    card.addEventListener("click", function () {
      openModal(card);
    });
  });

  document.querySelectorAll(".board-chair [data-leader-open]").forEach(function (button) {
    button.addEventListener("click", function () {
      openModal(button.closest("[data-leader]"));
    });
  });

  modal.querySelectorAll("[data-leader-close]").forEach(function (el) {
    el.addEventListener("click", closeModal);
  });

  /* Esc would close the dialog at once; run the closing animation instead. */
  modal.addEventListener("cancel", function (e) {
    e.preventDefault();
    closeModal();
  });
});
