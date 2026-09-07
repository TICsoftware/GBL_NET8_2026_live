document.addEventListener('DOMContentLoaded', function () {
let overlay = null;
let activeModal = null;
let isAnimating = false;

/* =========================
   TRIGGER CLICK
========================= */
document.querySelectorAll('[data-trigger="modal"]').forEach(btn => {
  btn.addEventListener('click', () => {
    if (isAnimating) return;

    const modal = document.querySelector(btn.dataset.target);
    openModal(modal);
  });
});

/* =========================
   CREATE OVERLAY
========================= */
function createOverlay() {
  overlay = document.createElement('div');

  overlay.className = `
    fixed inset-0 bg-black/60 backdrop-blur-md z-[1000]
  `;

  document.body.appendChild(overlay);

  gsap.set(overlay, { opacity: 0 });

overlay.addEventListener('click', (e) => {
  if (e.target === overlay) {
    closeModal();
  }
});

}

/* =========================
   OPEN MODAL
========================= */
function openModal(modal) {
  if (!modal || isAnimating) return;

  isAnimating = true;
  activeModal = modal;

  if (!overlay) createOverlay();

  document.documentElement.classList.add('overflow-hidden');
  document.body.classList.add('overflow-hidden');
  window.lenis?.stop();

  // ✅ IMPORTANT FIX
  modal.classList.remove('pointer-events-none', 'opacity-0');
  modal.classList.add('pointer-events-auto');

  const inner = modal.querySelector('.popup-wrapper-inner');

  // RESET
  gsap.set(modal, { opacity: 0 });
  gsap.set(inner, {
    y: 0,
    scale: 0.94,
    opacity: 0,
    rotateX: 8,
    filter: "blur(8px)"
  });

  const tl = gsap.timeline({
    defaults: { ease: "power3.out" },
    onComplete: () => isAnimating = false
  });

  tl.to(overlay, {
    opacity: 1,
    duration: 0.4
  })

  .to(modal, {
    opacity: 1,
    duration: 0.3
  }, "<")

  .to(inner, {
    scale: 1,
    opacity: 1,
    rotateX: 0,
    filter: "blur(0px)",
    duration: 0.6,
    ease: "power4.out"
  }, "-=0.2");
}

/* =========================
   CLOSE MODAL
========================= */
function closeModal() {
  if (!activeModal || isAnimating) return;

  isAnimating = true;

  const modal = activeModal;
  const inner = modal.querySelector('.popup-wrapper-inner');

  const tl = gsap.timeline({
    defaults: { ease: "power2.inOut" },
    onComplete: () => {
      overlay?.remove();
      overlay = null;

      // ✅ IMPORTANT FIX
      modal.classList.add('pointer-events-none');
      modal.classList.remove('pointer-events-auto');

      activeModal = null;

      document.documentElement.classList.remove('overflow-hidden');
      document.body.classList.remove('overflow-hidden');

      window.lenis?.start();
      isAnimating = false;
    }
  });

  tl.to(inner, {
    scale: 0.75,
    opacity: 0,
    rotateX: 6,
    filter: "blur(6px)",
    duration: 0.35,
    ease: "power3.in"
  })

  .to(modal, {
    opacity: 0,
    duration: 0.25
  }, "-=0.2")

  .to(overlay, {
    opacity: 0,
    duration: 0.3
  }, "-=0.2");
}

/* =========================
   CLOSE BUTTON
========================= */
document.addEventListener('click', e => {
  if (e.target.closest('[data-close]')) {
    closeModal();
  }
});

/* =========================
   ESC KEY
========================= */
document.addEventListener('keydown', e => {
  if (e.key === 'Escape') closeModal();
});

 document.querySelectorAll('.popup-wrapper-inner').forEach(el => {
    el.addEventListener('click', e => e.stopPropagation());
  });


});