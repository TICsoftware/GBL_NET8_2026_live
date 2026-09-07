(function () {
    function initCenterModeSlider() {
        if (typeof Swiper === "undefined") return;

        const sliderRoot = document.querySelector(".edgeSwiper");
        if (!sliderRoot) return;

        const qsa = (selector, scope = document) => [
            ...scope.querySelectorAll(selector),
        ];
        const qs = (selector, scope = document) => scope.querySelector(selector);
        const navContainer = qs(".edge-nav-arrows");
        const prevControl = navContainer
            ? qs(".edge-swiper-prev", navContainer)
            : null;
        const nextControl = navContainer
            ? qs(".edge-swiper-next", navContainer)
            : null;

        const wrapperEl = qs(".swiper-wrapper", sliderRoot);
        const originalSlides = qsa(".swiper-slide", sliderRoot);
        const originalCount = originalSlides.length;

        if (!wrapperEl || originalCount === 0) return;

        // 3-set track: [copy][original][copy] — order always a,b,c,d,a,b,c,d...
        // Native Swiper loop is avoided (one-direction / first-wrap bugs with
        // centeredSlides + fractional slidesPerView).
        const enableManualLoop = originalCount > 1;
        if (enableManualLoop) {
            const cloneSlide = (slide) => {
                const clone = slide.cloneNode(true);
                clone.setAttribute("data-duplicated", "true");
                // Avoid first-wrap layout shift from lazy images on clones.
                qsa("img", clone).forEach((img) => {
                    img.loading = "eager";
                    img.removeAttribute("loading");
                });
                return clone;
            };

            for (let i = originalCount - 1; i >= 0; i--) {
                wrapperEl.insertBefore(
                    cloneSlide(originalSlides[i]),
                    wrapperEl.firstChild,
                );
            }
            originalSlides.forEach((slide) => {
                wrapperEl.appendChild(cloneSlide(slide));
            });
        }

        const middleStart = enableManualLoop ? originalCount : 0;
        const middleEnd = enableManualLoop
            ? originalCount * 2 - 1
            : originalCount - 1;
        let jumping = false;
        let userInteracted = false;
        let directionArmed = false;

        const silentJump = (swiper, target) => {
            if (!enableManualLoop || jumping) return false;
            const idx = swiper.activeIndex;
            if (target === idx || target < 0 || target >= swiper.slides.length) {
                return false;
            }

            jumping = true;
            sliderRoot.classList.add("edge-swiper-jumping");
            swiper.setTransition(0);

            const currentSlide = swiper.slides[idx];
            const targetSlide = swiper.slides[target];
            if (currentSlide && targetSlide) {
                const diff =
                    targetSlide.getBoundingClientRect().left -
                    currentSlide.getBoundingClientRect().left;
                swiper.setTranslate(swiper.getTranslate() - diff);
            } else {
                const from = swiper.slidesGrid[idx];
                const to = swiper.slidesGrid[target];
                if (typeof from === "number" && typeof to === "number") {
                    swiper.setTranslate(swiper.getTranslate() - (to - from));
                }
            }

            swiper.updateActiveIndex(target);
            swiper.updateSlidesClasses();

            // Keep an in-progress swipe aligned after the invisible reposition.
            if (swiper.touchEventsData) {
                const t = swiper.getTranslate();
                swiper.touchEventsData.startTranslate = t;
                swiper.touchEventsData.currentTranslate = t;
            }

            // Force sync layout before the animated step that follows.
            void wrapperEl.offsetWidth;
            sliderRoot.classList.remove("edge-swiper-jumping");
            jumping = false;
            return true;
        };

        // Before moving next/prev at a middle-set edge, teleport to the twin
        // slide in the buffer set (same content, same on-screen position).
        // The following animated step then lands inside a continuous set —
        // no post-animation corrective snap, so the first wrap isn't jerky.
        const ensureLoopRoom = (swiper, direction) => {
            if (!enableManualLoop || jumping) return;
            const idx = swiper.activeIndex;

            if (direction > 0 && idx >= middleEnd) {
                silentJump(swiper, idx - originalCount);
            } else if (direction < 0 && idx <= middleStart) {
                silentJump(swiper, idx + originalCount);
            }
        };

        // Safety net for free-swipes that still land in a buffer set.
        const snapToMiddleSet = (swiper) => {
            if (!enableManualLoop || jumping) return;
            const idx = swiper.activeIndex;
            let target = null;
            if (idx < originalCount) target = idx + originalCount;
            else if (idx >= originalCount * 2) target = idx - originalCount;
            if (target === null) return;
            silentJump(swiper, target);
        };

        const edgeSwiper = new Swiper(".edgeSwiper", {
            slidesPerView: 1,
            centeredSlides: true,
            spaceBetween: 0,
            loop: false,
            rewind: false,
            slidesPerGroup: 1,
            watchOverflow: false,
            speed: 700,
            initialSlide: middleStart,
            allowTouchMove: true,
            navigation: false,

            breakpoints: {
                290: {
                    slidesPerView: 1.1,
                    centeredSlides: true,
                    spaceBetween: 2,
                },
                768: {
                    slidesPerView: 2.6,
                    centeredSlides: true,
                    spaceBetween: 30,
                },
            },

            observer: true,
            observeParents: true,
            observeSlideChildren: true,

            on: {
                init(swiper) {
                    swiper.update();
                    if (!enableManualLoop) return;

                    swiper.slideTo(middleStart, 0, false);
                    // Force layout of every slide (incl. clones) before first wrap.
                    swiper.slides.forEach((slide) => {
                        slide.getBoundingClientRect();
                    });
                    qsa("img", wrapperEl).forEach((img) => {
                        if (typeof img.decode === "function") {
                            img.decode().catch(() => {});
                        }
                    });
                },
                touchStart() {
                    userInteracted = true;
                    directionArmed = false;
                },
                sliderFirstMove(swiper) {
                    if (!enableManualLoop || directionArmed) return;
                    directionArmed = true;
                    userInteracted = true;
                    const diff =
                        (swiper.touches?.currentX ?? 0) -
                        (swiper.touches?.startX ?? 0);
                    if (Math.abs(diff) < 1) return;
                    // Finger left → next; finger right → prev.
                    ensureLoopRoom(swiper, diff < 0 ? 1 : -1);
                },
                slideChangeTransitionEnd(swiper) {
                    snapToMiddleSet(swiper);
                },
                transitionEnd(swiper) {
                    snapToMiddleSet(swiper);
                },
            },
        });

        qsa(".edgeArrow", sliderRoot).forEach((item) =>
            item.classList.add("hidden"),
        );

        const goNext = () => {
            userInteracted = true;
            ensureLoopRoom(edgeSwiper, 1);
            edgeSwiper.slideNext();
        };
        const goPrev = () => {
            userInteracted = true;
            ensureLoopRoom(edgeSwiper, -1);
            edgeSwiper.slidePrev();
        };

        nextControl?.addEventListener("click", goNext);
        prevControl?.addEventListener("click", goPrev);

        let resyncScheduled = false;
        const forceResync = () => {
            if (resyncScheduled) return;
            resyncScheduled = true;
            requestAnimationFrame(() => {
                edgeSwiper.update();
                if (!userInteracted) {
                    edgeSwiper.slideTo(middleStart, 0, false);
                } else {
                    snapToMiddleSet(edgeSwiper);
                }
                resyncScheduled = false;
            });
        };

        qsa("img", wrapperEl).forEach((img) => {
            if (!img.complete) {
                img.addEventListener("load", forceResync, { once: true });
            }
        });
        window.addEventListener("load", forceResync, { once: true });

        if (typeof IntersectionObserver !== "undefined") {
            const io = new IntersectionObserver(
                (entries, obs) => {
                    entries.forEach((entry) => {
                        if (entry.isIntersecting) {
                            forceResync();
                            obs.disconnect();
                        }
                    });
                },
                { threshold: 0.1 },
            );
            io.observe(sliderRoot);
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initCenterModeSlider, {
            once: true,
        });
    } else {
        initCenterModeSlider();
    }
})();
