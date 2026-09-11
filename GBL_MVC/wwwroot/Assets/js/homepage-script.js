document.addEventListener("DOMContentLoaded", () => {



/* ---------------------------------------
   PARALLAX IMAGE — now handled site-wide by image-parallax.js
--------------------------------------- */


/* ---------------------------------------
   CERTIFICATE BOX — smooth viewport reveal
--------------------------------------- */


/* ---------------------------------------
   ENLARGE WRAPPER (shared)
   Works for:
   - left content + right image  (.leftContent-wrap)
   - left image + right content  (.rightContent-wrap)
   1080px + container-center → original place
   Content fades in after ~80% of image travel
--------------------------------------- */


/* ---------------------------------------
   02. It Begins with a Belief — dissolve + slide-up
--------------------------------------- */
  var mediaEl = document.querySelector(".belief-media-swiper");
  var textEl = document.querySelector(".belief-text-swiper");
  var beliefLayout = document.querySelector(".belief-layout");
  if (mediaEl && textEl && typeof Swiper !== "undefined") {
    var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    var mediaSpeed = reduceMotion ? 0 : 1300;
    var beliefInView = false;

    var syncingBelief = false;
    var beliefCopy = document.querySelector(".belief-copy");

    var textSwiper = new Swiper(textEl, {
      effect: "fade",
      fadeEffect: { crossFade: true },
      speed: reduceMotion ? 0 : 700,
      allowTouchMove: false,
      autoHeight: true,
    });

    var mediaSwiper = new Swiper(mediaEl, {
      effect: "fade",
      fadeEffect: { crossFade: true },
      speed: mediaSpeed,
      loop: true,
      allowTouchMove: true,
      simulateTouch: true,
      grabCursor: true,
      autoplay: reduceMotion
        ? false
        : {
            delay: 4800,
            disableOnInteraction: false,
            pauseOnMouseEnter: true,
          },
      pagination: {
        el: document.querySelector(".belief-pagination"),
        clickable: true,
      },
      on: {
        init: function () {
          if (this.autoplay && this.autoplay.stop) this.autoplay.stop();
          bindBeliefVideos(this);
        },
        slideChange: function () {
          if (!syncingBelief) {
            syncingBelief = true;
            textSwiper.slideTo(this.realIndex);
            syncingBelief = false;
          }
          syncBeliefVideos(this);
        },
      },
    });

    function beliefVideos() {
      return mediaEl.querySelectorAll("video.belief-slide__video");
    }

    function pauseBeliefVideos() {
      Array.prototype.forEach.call(beliefVideos(), function (video) {
        video.pause();
      });
    }

    function activeBeliefVideo(swiper) {
      var slide = swiper && swiper.slides ? swiper.slides[swiper.activeIndex] : null;
      return slide ? slide.querySelector("video.belief-slide__video") : null;
    }

    function playActiveBeliefVideo(swiper) {
      pauseBeliefVideos();
      if (reduceMotion || !beliefInView) return;
      var video = activeBeliefVideo(swiper || mediaSwiper);
      if (!video || video.readyState < 2) return;
      var playPromise = video.play();
      if (playPromise && playPromise.catch) playPromise.catch(function () {});
    }

    function syncBeliefVideos(swiper) {
      pauseBeliefVideos();
      playActiveBeliefVideo(swiper);
    }

    function bindBeliefVideos(swiper) {
      Array.prototype.forEach.call(beliefVideos(), function (video) {
        video.loop = true;
        video.muted = true;
        video.playsInline = true;
        video.addEventListener("loadeddata", function () {
          if (video.closest(".swiper-slide-active")) playActiveBeliefVideo(swiper);
        });
      });
    }

    textSwiper.on("slideChange", function () {
      if (syncingBelief) return;
      syncingBelief = true;
      mediaSwiper.slideToLoop(this.realIndex);
      syncingBelief = false;
    });

    function bindBeliefCopySwipe(el) {
      if (!el) return;
      var pointerId = null;
      var startX = 0;
      var startY = 0;
      var locked = false;

      el.addEventListener("pointerdown", function (event) {
        if (event.button != null && event.button !== 0) return;
        if (event.target.closest(".belief-pagination, a, button")) return;
        pointerId = event.pointerId;
        startX = event.clientX;
        startY = event.clientY;
        locked = false;
      });

      el.addEventListener("pointermove", function (event) {
        if (pointerId == null || event.pointerId !== pointerId) return;
        var dx = event.clientX - startX;
        var dy = event.clientY - startY;
        if (!locked) {
          if (Math.abs(dx) < 10 && Math.abs(dy) < 10) return;
          if (Math.abs(dy) > Math.abs(dx)) {
            pointerId = null;
            return;
          }
          locked = true;
        }
      });

      function endPointer(event) {
        if (pointerId == null || (event.pointerId != null && event.pointerId !== pointerId)) return;
        var dx = event.clientX - startX;
        var wasLocked = locked;
        pointerId = null;
        locked = false;
        if (!wasLocked || Math.abs(dx) < 40) return;
        if (dx < 0) mediaSwiper.slideNext();
        else mediaSwiper.slidePrev();
      }

      el.addEventListener("pointerup", endPointer);
      el.addEventListener("pointercancel", endPointer);
    }

    bindBeliefCopySwipe(beliefCopy);

    function playBeliefEnter() {
      mediaEl.classList.add("is-inview");
      if (beliefLayout) beliefLayout.classList.add("is-inview");
      beliefInView = true;
      if (mediaSwiper.autoplay && mediaSwiper.autoplay.start && !reduceMotion) {
        mediaSwiper.autoplay.start();
      }
      playActiveBeliefVideo(mediaSwiper);
    }

    function pauseBeliefAutoplay() {
      if (mediaSwiper.autoplay && mediaSwiper.autoplay.stop) {
        mediaSwiper.autoplay.stop();
      }
      pauseBeliefVideos();
    }

    function bindBeliefTravel() {
      if (reduceMotion || typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") {
        playBeliefEnter();
        return;
      }

      gsap.registerPlugin(ScrollTrigger);

      var isMobile = window.matchMedia("(max-width: 992px)").matches;
      var stScroller = isMobile ? window : document.documentElement;
      var fromX = isMobile ? 0 : 96;
      var fromY = isMobile ? 120 : 140;
      var titleReveal = window.GBLTitleReveal;
      var titleFrom = titleReveal
        ? titleReveal.from
        : {
            y: 120,
            opacity: 0,
            skewY: 7,
            filter: "blur(12px)",
            clipPath: "inset(0 0 100% 0)",
          };
      var beliefTitle = document.querySelector(".belief-slide__title");
      var beliefBody = [];
      if (textEl) beliefBody.push(textEl);
      var beliefPager = document.querySelector(".belief-pagination");
      if (beliefPager) beliefBody.push(beliefPager);
      var titleDelay = null;

      gsap.set(mediaEl, { x: -fromX, y: fromY, force3D: true });
      if (beliefTitle) gsap.set(beliefTitle, titleFrom);
      if (beliefBody.length) gsap.set(beliefBody, { x: fromX, y: fromY, force3D: true });

      function playBeliefTitle() {
        if (!beliefTitle) return;
        if (titleReveal && titleReveal.play) {
          titleReveal.play(beliefTitle);
          return;
        }
        gsap.to(beliefTitle, {
          y: 0,
          opacity: 1,
          skewY: 0,
          filter: "blur(0px)",
          clipPath: "inset(0 0 0% 0)",
          duration: 1.3,
          ease: "expo.out",
          overwrite: "auto",
          force3D: true,
        });
      }

      function reverseBeliefTitle() {
        if (!beliefTitle) return;
        if (titleReveal && titleReveal.reverse) {
          titleReveal.reverse(beliefTitle);
          return;
        }
        gsap.to(beliefTitle, Object.assign({}, titleFrom, {
          duration: 0.7,
          ease: "expo.in",
          overwrite: "auto",
          force3D: true,
        }));
      }

      var travel = gsap.timeline({
        defaults: { ease: "none", force3D: true },
        scrollTrigger: {
          trigger: beliefLayout || mediaEl,
          scroller: stScroller,
          start: "top 90%",
          end: "top 42%",
          scrub: 1.2,
          invalidateOnRefresh: true,
          onEnter: function () {
            playBeliefTitle();
            if (titleDelay) titleDelay.kill();
            titleDelay = gsap.delayedCall(0.28, playBeliefEnter);
          },
          onEnterBack: function () {
            playBeliefTitle();
            playBeliefEnter();
          },
          onLeave: pauseBeliefAutoplay,
          onLeaveBack: function () {
            if (titleDelay) titleDelay.kill();
            reverseBeliefTitle();
            pauseBeliefAutoplay();
            if (beliefLayout) beliefLayout.classList.remove("is-inview");
            mediaEl.classList.remove("is-inview");
            beliefInView = false;
          },
        },
      });

      travel.to(mediaEl, { x: 0, y: 0, duration: 1 }, 0);
      if (beliefBody.length) {
        travel.to(beliefBody, { x: 0, y: 0, duration: 0.7 }, 0.22);
      }

      function refreshBeliefTravel() {
        ScrollTrigger.refresh();
      }

      window.requestAnimationFrame(refreshBeliefTravel);
      window.addEventListener("load", refreshBeliefTravel);
    }

    bindBeliefTravel();
  }

});
