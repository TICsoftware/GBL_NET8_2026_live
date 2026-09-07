$(document).ready(function () {

    // ================= GLOBAL =================
    let overlay = null;
    let activeModal = null;
    let isAnimating = false;

    /* =========================
       CREATE OVERLAY
    ========================= */
    function createOverlay() {

        overlay = document.createElement('div');

        overlay.className = "fixed inset-0 z-[1000] bg-black/50";
        overlay.style.backdropFilter = "blur(10px)";
        overlay.style.webkitBackdropFilter = "blur(10px)";

        document.body.appendChild(overlay);

        gsap.set(overlay, { opacity: 0 });

        overlay.addEventListener('click', function (e) {
            if (e.target === overlay) closeModal();
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

        modal.classList.remove('pointer-events-none', 'opacity-0');
        modal.classList.add('pointer-events-auto');

        const inner = modal.querySelector('.popup-wrapper-inner');

        gsap.set(modal, { opacity: 0 });

        gsap.set(inner, {
            scale: 0.94,
            opacity: 0,
            filter: "blur(8px)"
        });

        const tl = gsap.timeline({
            onComplete: function () {
                isAnimating = false;
            }
        });

        tl.to(overlay, { opacity: 1, duration: 0.3 })
          .to(modal, { opacity: 1, duration: 0.3 }, "<")
          .to(inner, {
              scale: 1,
              opacity: 1,
              filter: "blur(0px)",
              duration: 0.4
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
            onComplete: function () {

                overlay?.remove();
                overlay = null;

                modal.classList.add('pointer-events-none');
                modal.classList.remove('pointer-events-auto');

                activeModal = null;

                document.documentElement.classList.remove('overflow-hidden');
                document.body.classList.remove('overflow-hidden');

                isAnimating = false;
            }
        });

        tl.to(inner, {
            scale: 0.8,
            opacity: 0,
            filter: "blur(6px)",
            duration: 0.3
        })
        .to(modal, {
            opacity: 0,
            duration: 0.2
        }, "-=0.2")
        .to(overlay, {
            opacity: 0,
            duration: 0.3
        }, "-=0.2");
    }

    /* =========================
       AJAX CLICK
    ========================= */
    $(document).on("click", ".cancer-diagnosis", function () {

        var groupId = $(this).data('id');
        var contentId = $("#hdncontent").val();

        $.ajax({
            url: '/Home/GetCancerDiagnosisDetails',
            type: 'GET',
            data: { contentId: contentId, groupId: groupId },

            success: function (res) {
                
                $("#cancerdiagnosistitle").html(res.popuptitle);
                $("#cancerdiagnosisContent").html(res.popupcontent);

               

                const modal = document.querySelector("#cancerdiagnosis-pop");
                openModal(modal); // ✅ OPEN

            },

            error: function () {
                console.log("Error loading team data");
            }
        });

    });

    /* =========================
       CLOSE BUTTON
    ========================= */
    $(document).on("click", "[data-close]", function () {
        closeModal();
    });

    /* =========================
       CLICK OUTSIDE
    ========================= */
    $(document).on("click", "#ourteam-popup", function (e) {
        if (e.target.id === "ourteam-popup") {
            closeModal();
        }
    });

    /* =========================
       ESC KEY
    ========================= */
    $(document).on("keydown", function (e) {
        if (e.key === "Escape") {
            closeModal();
        }
    });

});