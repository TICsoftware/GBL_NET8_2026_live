/* ======================
   TESTIMONIAL SWIPER
====================== */

(function () {
    if (typeof Swiper === "undefined") return;

    const root = document.querySelector(".testimonialSwiper");
    const wrapper = root?.querySelector(".swiper-wrapper");
    const paginationEl = document.querySelector(".testimonial-pagination");
    if (!root || !wrapper) return;

    const DESKTOP_SLIDES_PER_VIEW = 3.5;
    const originalSlides = [
        ...wrapper.querySelectorAll(":scope > .swiper-slide"),
    ];
    const originalCount = originalSlides.length;
    if (!originalCount) return;

    // Remove minSlidesForLoop and force loop to work by cloning enough slides
    // for centeredSlides at the desktop slidesPerView.
    const requiredTotalSlides = Math.max(
        originalCount * 2,
        Math.ceil(DESKTOP_SLIDES_PER_VIEW) * 2 + 2,
    );
    let cloneIndex = 0;
    while (
        wrapper.querySelectorAll(":scope > .swiper-slide").length <
        requiredTotalSlides
    ) {
        const clone = originalSlides[cloneIndex % originalCount].cloneNode(
            true,
        );
        clone.setAttribute("data-testimonial-clone", "true");
        clone.setAttribute("aria-hidden", "true");
        wrapper.appendChild(clone);
        cloneIndex += 1;
    }

    const totalCount = wrapper.querySelectorAll(":scope > .swiper-slide").length;

    const testimonialSwiper = new Swiper(root, {
        initialSlide: 0,
        slidesPerView: 1.15,
        spaceBetween: 24,
        centeredSlides: true,
        // With requiredTotalSlides cloning, loop should always be safe.
        loop: originalCount > 1,
        loopedSlides: totalCount,
        loopAdditionalSlides: Math.ceil(DESKTOP_SLIDES_PER_VIEW),
        speed: 850,
        grabCursor: true,
        watchSlidesProgress: true,
        slideToClickedSlide: true,
        navigation: {
            nextEl: ".testimonial-next",
            prevEl: ".testimonial-prev",
        },
        breakpoints: {
            640: {
                slidesPerView: Math.min(1.8, Math.max(originalCount - 0.4, 1.1)),
                spaceBetween: 28,
                centeredSlides: true,
            },
            1024: {
                slidesPerView: DESKTOP_SLIDES_PER_VIEW,
                spaceBetween: 32,
                centeredSlides: true,
            },
            1280: {
                slidesPerView: DESKTOP_SLIDES_PER_VIEW,
                spaceBetween: 32,
                centeredSlides: true,
            },
        },
        on: {
            init(swiper) {
                // Keep first logical slide in center at startup.
                if (swiper.params.loop) swiper.slideToLoop(0, 0, false);
                buildAndSyncPagination(swiper);
                updateTestimonialDepth(swiper);
                requestAnimationFrame(() => syncPagination(swiper));
            },
            touchStart() {
                root.classList.add("is-dragging");
            },
            touchEnd() {
                root.classList.remove("is-dragging");
            },
            setTranslate(swiper) {
                updateTestimonialDepth(swiper);
            },
            slideChange(swiper) {
                syncPagination(swiper);
                updateTestimonialDepth(swiper);
            },
            realIndexChange(swiper) {
                syncPagination(swiper);
            },
            transitionEnd(swiper) {
                syncPagination(swiper);
            },
            resize(swiper) {
                if (swiper.params.loop) swiper.slideToLoop(swiper.realIndex, 0, false);
                syncPagination(swiper);
                updateTestimonialDepth(swiper);
            },
        },
    });

    function updateTestimonialDepth(swiper = testimonialSwiper) {
        swiper.slides.forEach((slide) => {
            const card = slide.querySelector(".testimonial-card");
            if (!card) return;

            const distance = Math.min(Math.abs(slide.progress), 1);
            const opacity = Math.max(0.3, 1 - distance * 0.7);
            const scale = Math.max(0.95, 1 - distance * 0.05);
            const blur = Math.min(1, distance * 1.5);

            card.style.opacity = opacity;
            card.style.transform = `scale(${scale})`;
            card.style.filter = blur > 0.05 ? `blur(${blur}px)` : "none";
            card.classList.toggle("is-muted", distance > 0.35);
        });
    }

    function buildAndSyncPagination(swiper) {
        if (!paginationEl) return;

        paginationEl.innerHTML = "";
        for (let i = 0; i < originalCount; i += 1) {
            const bullet = document.createElement("span");
            bullet.className = "swiper-pagination-bullet";
            bullet.setAttribute("role", "button");
            bullet.setAttribute("aria-label", `Go to slide ${i + 1}`);
            bullet.addEventListener("click", () => {
                if (swiper.params.loop) {
                    swiper.slideToLoop(i);
                } else {
                    swiper.slideTo(i);
                }
                syncPagination(swiper);
            });
            paginationEl.appendChild(bullet);
        }

        syncPagination(swiper);
    }

    function syncPagination(swiper) {
        if (!paginationEl) return;

        const activeSlide = swiper.slides?.[swiper.activeIndex];
        const slideIndexAttr = Number(
            activeSlide?.getAttribute("data-swiper-slide-index"),
        );

        const logicalIndex = Number.isFinite(slideIndexAttr)
            ? (slideIndexAttr % originalCount + originalCount) % originalCount
            : (swiper.realIndex % originalCount + originalCount) % originalCount;

        const bullets = paginationEl.querySelectorAll(".swiper-pagination-bullet");
        bullets.forEach((bullet, index) => {
            bullet.classList.toggle(
                "swiper-pagination-bullet-active",
                index === logicalIndex,
            );
        });
    }
})();