

function initContactModal() {
    const modal = document.querySelector("#contactModal");
    if (!modal) return;

    const panel = modal.querySelector(".modal-panel");

    const openButtons = document.querySelectorAll("[data-modal-open]");
    const closeButtons = modal.querySelectorAll("[data-modal-close]");

    let lastFocused = null;

    function openModal() {
        lastFocused = document.activeElement;

        modal.hidden = false;
        document.body.classList.add("modal-open");

        if (window.gsap) {
            gsap.fromTo(
                panel,
                {
                    autoAlpha: 0,
                    x: 60,
                },
                {
                    autoAlpha: 1,
                    x: 0,
                    duration: 0.45,
                    ease: "power3.out",
                }
            );
        }

        requestAnimationFrame(() => panel.focus());
    }

    function closeModal() {
        if (modal.hidden) return;

        const finish = () => {
            modal.hidden = true;
            document.body.classList.remove("modal-open");

            lastFocused?.focus();
        };

        if (window.gsap) {
            gsap.to(panel, {
                autoAlpha: 0,
                x: 60,
                duration: 0.25,
                ease: "power2.out",
                onComplete: () => {
                    gsap.set(panel, { clearProps: "all" });
                    finish();
                },
            });
        } else {
            finish();
        }
    }

    openButtons.forEach((button) => {
        button.addEventListener("click", openModal);
    });

    closeButtons.forEach((button) => {
        button.addEventListener("click", closeModal);
    });

    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            closeModal();
        }
    });
}

document.addEventListener("DOMContentLoaded", () => {
    initContactModal();
});