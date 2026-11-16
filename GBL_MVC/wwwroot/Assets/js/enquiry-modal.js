(function () {
  var modal = document.getElementById("enquiry-modal");
  if (!modal || typeof modal.showModal !== "function") return;

  var panel = modal.querySelector(".enquiry-modal__panel");
  var backdrop = modal.querySelector(".enquiry-modal__backdrop");
  var scroller = modal.querySelector(".enquiry-modal__scroll");
  var productLabel = modal.querySelector("[data-enquiry-product-label]");
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

  function setProductContext(trigger) {
    if (!trigger) return;

    var nameInput = document.getElementById("ProductName");
    var pageInput = document.getElementById("ProductPageName");
    var productName = trigger.getAttribute("data-product-name") || (nameInput && nameInput.value) || "";
    var productPage = trigger.getAttribute("data-product-page") || (pageInput && pageInput.value) || "";

    if (nameInput) nameInput.value = productName;
    if (pageInput) pageInput.value = productPage;
    if (productLabel) productLabel.textContent = productName;
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

  function openModal(trigger) {
    window.clearTimeout(closeTimer);
    closing = false;
    setProductContext(trigger);
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
    var openBtn = event.target.closest("[data-enquiry-open]");
    if (openBtn) {
      event.preventDefault();
      openModal(openBtn);
      return;
    }

    var closeBtn = event.target.closest("[data-enquiry-close]");
    if (closeBtn && modal.contains(closeBtn)) {
      event.preventDefault();
      closeModal();
    }
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