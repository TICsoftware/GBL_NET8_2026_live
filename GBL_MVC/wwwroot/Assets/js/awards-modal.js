(function () {
  var modal = document.getElementById("award-modal");
  if (!modal || typeof modal.showModal !== "function") return;

  var panel = modal.querySelector(".leader-modal__panel");
  var backdrop = modal.querySelector(".leader-modal__backdrop");
  var scroller = modal.querySelector(".leader-modal__scroll");
  var photo = modal.querySelector("[data-award-modal-photo]");
  var nameEl = modal.querySelector("[data-award-modal-name]");
  var roleEl = modal.querySelector("[data-award-modal-role]");
  var bioEl = modal.querySelector("[data-award-modal-bio]");
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

  function fill(card) {
    var image = card.querySelector("[data-award-photo]");
    if (photo && image) {
      photo.src = image.currentSrc || image.src || "";
      photo.alt = image.alt || "";
    }
    if (nameEl) {
      var title = card.querySelector("[data-award-name]");
      nameEl.textContent = title ? title.textContent.trim() : "";
      // Long award names drop one heading size so they don't fill the popup
      nameEl.classList.toggle("is-long", nameEl.textContent.length > 55);
    }
    if (roleEl) {
      var role = card.querySelector("[data-award-role]");
      var roleText = role ? role.textContent.trim() : "";
      roleEl.textContent = roleText;
      roleEl.hidden = !roleText;
    }
    if (bioEl) {
      var bio = card.querySelector("[data-award-bio]");
      bioEl.innerHTML = bio ? bio.innerHTML : "";
    }
  }

  function finishClose() {
    window.clearTimeout(closeTimer);
    closing = false;
    if (modal.open) modal.close();
    modal.classList.remove("is-closing", "is-open");
    if (panel) {
      panel.style.transition = "";
      panel.style.transform = "";
    }
    if (backdrop) backdrop.style.opacity = "";
    unlockPage();
  }

  function hasPopupIntro(card) {
    var bio = card && card.querySelector("[data-award-bio]");
    return !!(bio && bio.innerHTML.replace(/\s+/g, "").length);
  }

  function openModal(card) {
    if (!hasPopupIntro(card)) return;
    window.clearTimeout(closeTimer);
    closing = false;
    fill(card);
    if (!modal.open) {
      modal.showModal();
      lockPage();
    }
    if (scroller) scroller.scrollTop = 0;
    modal.classList.remove("is-closing");
    void modal.offsetWidth;
    requestAnimationFrame(function () {
      if (closing || !modal.open) return;
      modal.classList.add("is-open");
    });
  }

  function closeModal() {
    if (!modal.open || closing) return;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches || !modal.classList.contains("is-open")) {
      finishClose();
      return;
    }
    closing = true;
    modal.classList.add("is-closing");
    modal.classList.remove("is-open");
    closeTimer = window.setTimeout(finishClose, 320);
  }

  document.addEventListener("click", function (event) {
    var card = event.target.closest(".award-card[data-award]");
    if (card) {
      // Don't open when using slider arrows inside the same slider chrome
      if (event.target.closest(".awards-arrow")) return;
      event.preventDefault();
      openModal(card);
      return;
    }

    var closeBtn = event.target.closest("[data-award-close]");
    if (closeBtn && modal.contains(closeBtn)) {
      event.preventDefault();
      closeModal();
    }
  });

  document.addEventListener("keydown", function (event) {
    if (event.key !== "Enter" && event.key !== " ") return;
    var card = event.target.closest(".award-card[data-award]");
    if (!card) return;
    event.preventDefault();
    openModal(card);
  });

  modal.addEventListener("cancel", function (event) {
    event.preventDefault();
    closeModal();
  });

  modal.addEventListener("close", function () {
    unlockPage();
    modal.classList.remove("is-open", "is-closing");
  });
})();
