/* ==========================================================
   HERO MEDIA SLIDER (image + video slides)
   - Controls (arrows + pagination) only show when there is
     more than one slide.
   - A video slide always plays to the end before the
     slider auto-advances to the next slide.
   - Slide changes (and the initial page load) use a "slide
     break": two panels meet in the centre to hide the swap,
     then part ways (left/right) to reveal the slide beneath.
     The panels are closed by default in CSS, so there's never
     a blank flash before this script even runs.
   ========================================================== */
document.addEventListener("DOMContentLoaded", function () {
  var heroEl = document.querySelector(".hero-media-swiper");
  if (!heroEl || typeof Swiper === "undefined") return;

  var slides = heroEl.querySelectorAll(".hero-slide");
  var controls = document.querySelector(".hero-swiper-controls");
  var captionEl = document.querySelector(".hero-caption-text");
  var breakEl = document.querySelector(".hero-slide-break");
  var IMAGE_DELAY = 5000; // ms an image slide stays before advancing
  var DOOR_DURATION = 380; // ms — must match the CSS transition on .hero-slide-break
  var SWAP_BUFFER = 70; // ms pause once the panels meet, before they part again

  function setCaption(slide) {
    if (!captionEl) return;
    captionEl.classList.remove("is-visible");
    window.setTimeout(function () {
      var text = (slide && slide.dataset.caption) || "";
      captionEl.textContent = text;
      captionEl.classList.toggle("is-visible", !!text);
    }, 120);
  }

  // Reveal the active slide's media with a quick left-to-right ease-in,
  // restarted every time a slide becomes active. This finishes while the
  // panels are still shut, so it's never visible mid-motion.
  function revealMedia(slide) {
    heroEl.querySelectorAll(".hero-media").forEach(function (media) {
      media.classList.remove("is-revealed");
    });
    var media = slide && slide.querySelector(".hero-media");
    if (!media) return;
    void media.offsetWidth; // force reflow so the transition restarts cleanly
    media.classList.add("is-revealed");
  }

  // Runs `applyChanges` while the two panels are together, then parts them
  // again. `alreadyClosed` skips the initial "close" step — used only for
  // the very first call, since the panels start closed by default in CSS.
  function runSlideBreak(applyChanges, alreadyClosed) {
    if (!breakEl) {
      applyChanges();
      return;
    }

    function swapThenOpen() {
      applyChanges();
      window.setTimeout(function () {
        breakEl.classList.add("is-open");
      }, SWAP_BUFFER);
    }

    if (alreadyClosed) {
      swapThenOpen();
    } else {
      breakEl.classList.remove("is-open");
      window.setTimeout(swapThenOpen, DOOR_DURATION);
    }
  }

  // Only one slide: skip Swiper entirely, no controls, just loop the media.
  if (slides.length <= 1) {
    var soloVideo = heroEl.querySelector("video");
    if (soloVideo) {
      soloVideo.loop = true;
      soloVideo.play().catch(function () {});
    }
    runSlideBreak(function () {
      setCaption(slides[0]);
      revealMedia(slides[0]);
    }, true);
    return;
  }

  var heroSwiper = new Swiper(heroEl, {
    loop: true,
    speed: 60, // near-instant swap: the "slide break" panels carry the transition
    effect: "fade",
    fadeEffect: { crossFade: false },
    autoplay: { delay: IMAGE_DELAY, disableOnInteraction: false },
    pagination: { el: ".hero-swiper-pagination", clickable: true },
    navigation: { nextEl: ".hero-swiper-next", prevEl: ".hero-swiper-prev" },
  });

  if (controls) controls.classList.add("is-active");

  var isFirstSync = true;

  function syncActiveSlide() {
    var activeSlide = heroSwiper.slides[heroSwiper.activeIndex];

    runSlideBreak(function () {
      setCaption(activeSlide);
      revealMedia(activeSlide);
    }, isFirstSync);
    isFirstSync = false;

    var activeVideo = activeSlide && activeSlide.querySelector("video");

    // Pause/rewind every video that isn't the active one.
    heroEl.querySelectorAll("video").forEach(function (video) {
      if (video !== activeVideo) {
        video.pause();
        video.currentTime = 0;
      }
    });

    if (!activeVideo) {
      heroSwiper.autoplay.start();
      return;
    }

    // Hold the timed autoplay until this video finishes playing.
    heroSwiper.autoplay.stop();
    activeVideo.currentTime = 0;

    var resume = function () {
      activeVideo.removeEventListener("ended", resume);
      heroSwiper.autoplay.start();
      heroSwiper.slideNext();
    };
    activeVideo.addEventListener("ended", resume);

    activeVideo.play().catch(function () {
      // Autoplay blocked (e.g. no user interaction yet) — fall back to timed autoplay.
      heroSwiper.autoplay.start();
    });
  }

  heroSwiper.on("slideChangeTransitionEnd", syncActiveSlide);
  syncActiveSlide();
});
