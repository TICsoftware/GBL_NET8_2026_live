document.addEventListener("DOMContentLoaded", function () {
  var heroEl = document.querySelector(".hero-swiper");
  var thumbsEl = document.querySelector(".hero-thumbs");
  if (!heroEl || !thumbsEl || typeof Swiper === "undefined") return;

  /* ----------------------------------------------------------
     Setup: motion prefs, slide count, clip-path wipe values
     ---------------------------------------------------------- */
  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var counterEl = document.querySelector(".hero-counter");
  var counterCurrent = document.querySelector(".hero-counter__current");
  var counterTotal = document.querySelector(".hero-counter__total");
  var totalSlides = heroEl.querySelectorAll(":scope > .swiper-wrapper > .swiper-slide").length;
  var thumbSpeed = reduceMotion ? 0 : 700;
  var clipDuration = reduceMotion ? 0 : 1.7;
  var CLIP_EASE = "cubic-bezier(0.4, 0.0, 0.15, 1)";
  var CLIP_EASE_FINISH = "power2.out";
  var CLIP_OPEN = "inset(0% 0% 0% 0%)";
  var CLIP_FROM_RIGHT = "inset(0% 0% 0% 100%)";
  var CLIP_FROM_LEFT = "inset(0% 100% 0% 0%)";
  var lastRealIndex = 0;
  var lastActiveSlide = null;
  var clipTween = null;
  var autoplayStoppedByScroll = false;
  var wipeFromLeft = null;
  var heroReady = false;
  var videoHoldsAutoplay = false;

  /* Pad 1 → "01" for the hero counter */
  function padSlide(value) {
    return String(value).padStart(2, "0");
  }

  /* Update "01 / 09" and the accessible slide label */
  function setHeroCounter(index) {
    var current = index + 1;
    if (counterCurrent) {
      counterCurrent.textContent = padSlide(current);
    }
    if (counterTotal) {
      counterTotal.textContent = " / " + padSlide(totalSlides);
    }
    if (counterEl) {
      counterEl.setAttribute("aria-label", "Slide " + current + " of " + totalSlides);
    }
  }

  /* Thumbnails show the *next* hero slide, not the current one */
  function nextThumbIndex(index) {
    return (index + 1) % totalSlides;
  }

  /* Highlight the thumbnail that matches the upcoming hero slide */
  function setThumbActive(heroIndex) {
    var upcoming = nextThumbIndex(heroIndex);
    thumbsEl.querySelectorAll(".swiper-slide").forEach(function (slide) {
      var slideIndex = Number(slide.getAttribute("data-hero-index"));
      slide.classList.toggle("is-active", slideIndex === upcoming);
    });
  }

  var thumbsDragging = false;

  /* Scroll the thumbs strip so the upcoming slide sits in view */
  function showUpcomingThumbs(heroIndex, speed) {
    var start = nextThumbIndex(heroIndex);
    setThumbActive(heroIndex);
    if (thumbsSwiper.realIndex === start) return;

    var current = thumbsSwiper.realIndex;
    var forwardSteps = (start - current + totalSlides) % totalSlides;
    var backSteps = (current - start + totalSlides) % totalSlides;

    /* Prefer a single next/prev step so looped thumbs stay smooth */
    if (forwardSteps === 1) {
      thumbsSwiper.slideNext(speed);
      return;
    }
    if (backSteps === 1) {
      thumbsSwiper.slidePrev(speed);
      return;
    }
    if (typeof thumbsSwiper.slideToLoop === "function") {
      thumbsSwiper.slideToLoop(start, speed);
    } else {
      thumbsSwiper.slideTo(start, speed);
    }
  }

  /* ----------------------------------------------------------
     Slide DOM helpers
     ---------------------------------------------------------- */
  function slideMedia(slide) {
    return slide ? slide.querySelector(".hero-slide__media") : null;
  }

  function slideImage(slide) {
    if (!slide) return null;
    return slide.querySelector(".hero-slide__media img, .hero-slide__media video");
  }

  function heroVideos() {
    return heroEl.querySelectorAll(".hero-slide__media video");
  }

  function pauseHeroVideos() {
    Array.prototype.forEach.call(heroVideos(), function (video) {
      video.pause();
    });
  }

  function resumeHeroAutoplayAfterVideo() {
    if (autoplayStoppedByScroll || reduceMotion || !heroSwiper || !heroSwiper.autoplay) return;
    if (heroSwiper.autoplay.start) heroSwiper.autoplay.start();
  }

  function playActiveHeroVideo(swiper) {
    pauseHeroVideos();
    if (reduceMotion || !swiper) {
      videoHoldsAutoplay = false;
      return false;
    }
    var slide = swiper.slides[swiper.activeIndex];
    var video = slide ? slide.querySelector(".hero-slide__media video") : null;
    if (!video) {
      videoHoldsAutoplay = false;
      resumeHeroAutoplayAfterVideo();
      return false;
    }

    video.loop = false;
    video.muted = true;
    video.playsInline = true;
    try { video.currentTime = 0; } catch (err) {}
    videoHoldsAutoplay = true;
    if (swiper.autoplay && swiper.autoplay.stop) swiper.autoplay.stop();

    var playPromise = video.play();
    if (playPromise && playPromise.catch) {
      playPromise.catch(function () {
        videoHoldsAutoplay = false;
        resumeHeroAutoplayAfterVideo();
      });
    }
    return true;
  }

  function bindHeroVideos(swiper) {
    Array.prototype.forEach.call(heroVideos(), function (video) {
      if (video.dataset.heroVideoBound === "1") return;
      video.dataset.heroVideoBound = "1";
      video.loop = false;
      video.muted = true;
      video.playsInline = true;
      video.addEventListener("ended", function () {
        var active = swiper.slides[swiper.activeIndex];
        if (!active || !active.contains(video)) return;
        videoHoldsAutoplay = false;
        if (autoplayStoppedByScroll || reduceMotion) return;
        swiper.slideNext();
      });
      video.addEventListener("error", function () {
        videoHoldsAutoplay = false;
        resumeHeroAutoplayAfterVideo();
      });
    });
  }

  function syncHeroMobileHeight(swiper) {
    var banner = document.querySelector(".heroBanner");
    swiper = swiper || heroSwiper;
    if (!banner || !swiper || !swiper.slides) return;
    if (!window.matchMedia("(max-width: 767px)").matches) {
      banner.style.height = "";
      return;
    }
    var slide = swiper.slides[swiper.activeIndex];
    var content = slide && slide.querySelector(".hero-slide__content");
    if (!content) return;
    var nextHeight = Math.ceil(content.scrollHeight);
    if (nextHeight < 1) return;
    banner.style.height = nextHeight + "px";
  }

  function scheduleHeroMobileHeight(swiper) {
    syncHeroMobileHeight(swiper);
    window.requestAnimationFrame(function () {
      syncHeroMobileHeight(swiper);
    });
    window.setTimeout(function () {
      syncHeroMobileHeight(swiper);
    }, 80);
    window.setTimeout(function () {
      syncHeroMobileHeight(swiper);
    }, 400);
  }

  function slideCaption(slide) {
    return slide ? slide.querySelector(".hero-slide__caption") : null;
  }

  function slideCta(slide) {
    return slide ? slide.querySelector(".hero-slide__cta") : null;
  }

  function setClip(el, value) {
    if (!el) return;
    el.style.clipPath = value;
    el.style.webkitClipPath = value;
  }

  /* True for the live slide, including Swiper loop duplicates */
  function isActiveHeroSlide(swiper, slide) {
    if (!slide) return false;
    return slide === swiper.slides[swiper.activeIndex]
      || slide.classList.contains("swiper-slide-active")
      || slide.classList.contains("swiper-slide-duplicate-active");
  }

  /* Snap media to a clean rest state: active fully open, others clipped shut */
  function revealActiveMedia(swiper) {
    var active = swiper.slides[swiper.activeIndex];
    if (clipTween) {
      clipTween.kill();
      clipTween = null;
    }
    swiper.slides.forEach(function (slide) {
      var media = slideMedia(slide);
      var img = slideImage(slide);
      var on = isActiveHeroSlide(swiper, slide);
      if (media) {
        media.style.zIndex = on ? "3" : "1";
        setClip(media, on ? CLIP_OPEN : CLIP_FROM_RIGHT);
      }
      if (img && typeof gsap !== "undefined") {
        gsap.set(img, { scale: on ? 1 : 1.18 });
      }
    });
    lastActiveSlide = active;
    lastRealIndex = swiper.realIndex;
  }

  /* Caption line-reveal + CTA fade when the hero slide changes */
  function animateCaption(outgoingSlide, incomingSlide) {
    var textAnim = window.GBLTextAnim;
    var outgoingCaption = slideCaption(outgoingSlide);
    var incomingCaption = slideCaption(incomingSlide);
    var outgoingCta = slideCta(outgoingSlide);
    var incomingCta = slideCta(incomingSlide);

    if (typeof gsap !== "undefined") {
      if (outgoingCta) {
        gsap.to(outgoingCta, {
          y: 24,
          opacity: 0,
          duration: reduceMotion ? 0 : 0.35,
          ease: "power2.in",
          overwrite: "auto",
        });
      }
      if (incomingCta) {
        gsap.fromTo(
          incomingCta,
          { y: 28, opacity: 0 },
          {
            y: 0,
            opacity: 1,
            duration: reduceMotion ? 0 : 0.7,
            delay: reduceMotion ? 0 : 0.45,
            ease: "power3.out",
            overwrite: "auto",
          }
        );
      }
    }

    if (!textAnim) return;
    if (outgoingCaption && outgoingCaption !== incomingCaption) {
      textAnim.playOut(outgoingCaption);
    }
    if (incomingCaption) {
      textAnim.split(incomingCaption);
      textAnim.hide(incomingCaption);
      textAnim.playIn(incomingCaption, {
        delay: reduceMotion ? 0 : 0.18,
        duration: 1.05,
        stagger: 0.1,
        ease: "power4.out",
      });
    }
  }

  /* ----------------------------------------------------------
     Clip-path wipe: incoming uncovers, outgoing is eaten away
     ---------------------------------------------------------- */
  function closedClip(fromLeft) {
    return fromLeft ? CLIP_FROM_LEFT : CLIP_FROM_RIGHT;
  }

  /* Incoming starts fully clipped; progress 1 = fully visible */
  function clipIncoming(progress, fromLeft) {
    var closed = (1 - Math.min(1, Math.max(0, progress))) * 100;
    if (fromLeft) return "inset(0% " + closed + "% 0% 0%)";
    return "inset(0% 0% 0% " + closed + "%)";
  }

  /* Outgoing starts fully visible; progress 1 = fully clipped */
  function clipOutgoing(progress, fromLeft) {
    var eaten = Math.min(1, Math.max(0, progress)) * 100;
    if (fromLeft) return "inset(0% 0% 0% " + eaten + "%)";
    return "inset(0% " + eaten + "% 0% 0%)";
  }

  /* Read how far a live clip-path has already opened (for swipe → autoplay handoff) */
  function wipeProgressFromClip(clip, fromLeft) {
    var match = /inset\(\s*([\d.]+)%\s+([\d.]+)%\s+([\d.]+)%\s+([\d.]+)%/.exec(clip || "");
    if (!match) return 0;
    var closed = fromLeft ? parseFloat(match[2]) : parseFloat(match[4]);
    return 1 - Math.min(100, Math.max(0, closed)) / 100;
  }

  function applyWipe(outgoingMedia, incomingMedia, progress, fromLeft) {
    if (incomingMedia) setClip(incomingMedia, clipIncoming(progress, fromLeft));
    if (outgoingMedia) setClip(outgoingMedia, clipOutgoing(progress, fromLeft));
  }

  /* Wipe direction: backward / wrap from first→last comes in from the left */
  function shouldWipeFromLeft(swiper) {
    if (wipeFromLeft !== null) return wipeFromLeft;
    var prev = lastRealIndex;
    var next = swiper.realIndex;
    if (prev === 0 && next === totalSlides - 1) return true;
    if (prev === totalSlides - 1 && next === 0) return false;
    return next < prev;
  }

  /* Find the loop-safe slide that would appear if we keep dragging this way */
  function incomingSlide(swiper, goingNext) {
    var targetReal = (swiper.realIndex + (goingNext ? 1 : -1) + totalSlides) % totalSlides;
    var active = swiper.slides[swiper.activeIndex];
    for (var i = 0; i < swiper.slides.length; i += 1) {
      var slide = swiper.slides[i];
      if (slide === active) continue;
      if (Number(slide.getAttribute("data-swiper-slide-index")) === targetReal) {
        return slide;
      }
    }
    return swiper.slides[swiper.activeIndex + (goingNext ? 1 : -1)] || null;
  }

  /* Live wipe preview while the user is dragging (before a slide commit) */
  function previewIncoming(swiper, goingNext, progress) {
    var current = lastActiveSlide || swiper.slides[swiper.activeIndex];
    var incoming = incomingSlide(swiper, goingNext);
    var currentMedia = slideMedia(current);
    var incomingMedia = slideMedia(incoming);
    if (!incomingMedia || incoming === current) return;
    var fromLeft = !goingNext;
    var eased = 1 - Math.pow(1 - Math.min(1, Math.max(0, progress)), 1.25);

    if (clipTween) {
      clipTween.kill();
      clipTween = null;
    }

    swiper.slides.forEach(function (slide) {
      var media = slideMedia(slide);
      if (!media || slide === current || slide === incoming) return;
      media.style.zIndex = "1";
      setClip(media, closedClip(fromLeft));
    });

    if (currentMedia) currentMedia.style.zIndex = "2";
    incomingMedia.style.zIndex = "3";
    applyWipe(currentMedia, incomingMedia, eased, fromLeft);
  }

  /* Cancel an incomplete swipe: reopen the current slide, close the rest */
  function resetInactiveMedia(swiper, keepOpen) {
    var keepMedia = slideMedia(keepOpen);
    var others = [];
    swiper.slides.forEach(function (slide) {
      var media = slideMedia(slide);
      if (!media) return;
      if (slide === keepOpen) {
        media.style.zIndex = "3";
        return;
      }
      media.style.zIndex = "1";
      others.push(media);
    });

    if (reduceMotion || typeof gsap === "undefined") {
      if (keepMedia) setClip(keepMedia, CLIP_OPEN);
      others.forEach(function (media) { setClip(media, CLIP_FROM_RIGHT); });
      return;
    }

    if (clipTween) clipTween.kill();
    if (keepMedia) {
      gsap.to(keepMedia, {
        clipPath: CLIP_OPEN,
        webkitClipPath: CLIP_OPEN,
        duration: 0.55,
        ease: CLIP_EASE_FINISH,
        overwrite: "auto",
      });
    }
    others.forEach(function (media) {
      gsap.to(media, {
        clipPath: CLIP_FROM_RIGHT,
        webkitClipPath: CLIP_FROM_RIGHT,
        duration: 0.55,
        ease: CLIP_EASE_FINISH,
        overwrite: "auto",
      });
    });
  }

  /* Commit a slide change: finish the wipe, Ken Burns the image, run captions */
  function animateClip(swiper) {
    var incoming = swiper.slides[swiper.activeIndex];
    if (!incoming || incoming === lastActiveSlide) return;
    var outgoing = lastActiveSlide && lastActiveSlide !== incoming
      ? lastActiveSlide
      : swiper.slides[swiper.previousIndex];
    var incomingMedia = slideMedia(incoming);
    var outgoingMedia = slideMedia(outgoing);
    var incomingImg = slideImage(incoming);
    var fromLeft = shouldWipeFromLeft(swiper);
    wipeFromLeft = null;
    lastRealIndex = swiper.realIndex;
    lastActiveSlide = incoming;

    animateCaption(outgoing, incoming);

    if (!incomingMedia) return;

    if (clipTween) clipTween.kill();

    swiper.slides.forEach(function (slide) {
      var media = slideMedia(slide);
      if (!media || slide === incoming || slide === outgoing) return;
      media.style.zIndex = "1";
      setClip(media, closedClip(fromLeft));
    });

    if (outgoingMedia && outgoingMedia !== incomingMedia) {
      outgoingMedia.style.zIndex = "2";
    }
    incomingMedia.style.zIndex = "3";

    var startProgress = wipeProgressFromClip(incomingMedia.style.clipPath, fromLeft);

    if (reduceMotion || typeof gsap === "undefined") {
      applyWipe(outgoingMedia, incomingMedia, 1, fromLeft);
      if (incomingImg && typeof gsap !== "undefined") gsap.set(incomingImg, { scale: 1 });
      return;
    }

    /* If swipe already opened part of the wipe, shorten the remaining tween */
    var remaining = Math.max(0.08, 1 - startProgress);
    var duration = startProgress > 0.04
      ? Math.max(0.55, clipDuration * remaining)
      : clipDuration;
    var ease = startProgress > 0.04 ? CLIP_EASE_FINISH : CLIP_EASE;
    var wipeState = { p: startProgress };

    if (incomingImg) gsap.set(incomingImg, { scale: 1.18 });

    clipTween = gsap.to(wipeState, {
      p: 1,
      duration: duration,
      ease: ease,
      overwrite: "auto",
      onUpdate: function () {
        applyWipe(outgoingMedia, incomingMedia, wipeState.p, fromLeft);
      },
      onComplete: function () {
        applyWipe(outgoingMedia, incomingMedia, 1, fromLeft);
        if (outgoingMedia && outgoingMedia !== incomingMedia) {
          outgoingMedia.style.zIndex = "1";
        }
      },
    });

    if (incomingImg) {
      gsap.to(incomingImg, {
        scale: 1,
        duration: duration + 0.35,
        ease: CLIP_EASE_FINISH,
        overwrite: "auto",
      });
    }
  }

  /* ----------------------------------------------------------
     Thumbs Swiper: looping strip of upcoming slides
     ---------------------------------------------------------- */
  var thumbsSwiper = new Swiper(thumbsEl, {
    effect: "slide",
    slidesPerView: 3,
    slidesPerGroup: 1,
    spaceBetween: 8,
    speed: thumbSpeed,
    loop: true,
    loopedSlides: totalSlides,
    initialSlide: 1,
    watchSlidesProgress: false,
    allowTouchMove: true,
    breakpoints: {
      0: {
        slidesPerView: 2,
        spaceBetween: 8,
      },
      768: {
        slidesPerView: 3,
        spaceBetween: 8,
      },
    },
  });

  /* ----------------------------------------------------------
     Main hero Swiper: fade shell; visual motion is the custom wipe
     ---------------------------------------------------------- */
  var heroSwiper = new Swiper(heroEl, {
    effect: "fade",
    fadeEffect: { crossFade: false },
    speed: 0,
    loop: true,
    allowTouchMove: false,
    simulateTouch: false,
    autoplay: reduceMotion
      ? false
      : {
          delay: 5600,
          disableOnInteraction: false,
          pauseOnMouseEnter: false,
        },
    navigation: {
      prevEl: document.querySelector(".hero-nav--prev"),
      nextEl: document.querySelector(".hero-nav--next"),
    },
    on: {
      init: function () {
        revealActiveMedia(this);
        setHeroCounter(this.realIndex);
        showUpcomingThumbs(this.realIndex, 0);

        /* Prepare captions/CTAs: only the first slide is visible at rest */
        this.slides.forEach(function (slide) {
          var caption = slideCaption(slide);
          var cta = slideCta(slide);
          var isActive = isActiveHeroSlide(this, slide);
          if (window.GBLTextAnim && caption) {
            window.GBLTextAnim.split(caption);
            if (isActive) window.GBLTextAnim.show(caption);
            else window.GBLTextAnim.hide(caption);
          }
          if (cta && typeof gsap !== "undefined") {
            gsap.set(cta, { opacity: isActive ? 1 : 0, y: isActive ? 0 : 24 });
          }
        }, this);

        if (window.GBLTextAnim) {
          var firstCaption = slideCaption(this.slides[this.activeIndex]);
          if (firstCaption) {
            window.GBLTextAnim.playIn(firstCaption, {
              duration: 1.05,
              stagger: 0.1,
              ease: "power4.out",
            });
          }
        }

        var swiper = this;
        bindHeroVideos(swiper);
        heroReady = true;
        requestAnimationFrame(function () {
          revealActiveMedia(swiper);
          playActiveHeroVideo(swiper);
          scheduleHeroMobileHeight(swiper);
        });
      },
      realIndexChange: function () {
        setHeroCounter(this.realIndex);
        showUpcomingThumbs(this.realIndex, thumbSpeed);
      },
      slideChange: function () {
        if (!heroReady) return;
        /* Same real slide (loop duplicate) — just resync clips, don't wipe */
        if (this.realIndex === lastRealIndex) {
          revealActiveMedia(this);
          playActiveHeroVideo(this);
          scheduleHeroMobileHeight(this);
          return;
        }
        animateClip(this);
        playActiveHeroVideo(this);
        scheduleHeroMobileHeight(this);
      },
    },
  });

  /* ----------------------------------------------------------
     Custom pointer swipe (Swiper touch is off so we own the wipe)
     ---------------------------------------------------------- */
  (function bindHeroSwipe() {
    var pointerId = null;
    var startX = 0;
    var startY = 0;
    var lastX = 0;
    var lastTime = 0;
    var velocity = 0;
    var dragging = false;
    var locked = false;
    var goingNext = true;
    var ignoreSelector = ".hero-nav, .hero-thumbs-wrap, .hero-slide__cta, a, button";

    function pointerX(event) {
      return event.clientX;
    }

    function pointerY(event) {
      return event.clientY;
    }

    function onPointerDown(event) {
      if (event.button != null && event.button !== 0) return;
      if (event.target.closest(ignoreSelector)) return;
      pointerId = event.pointerId;
      startX = lastX = pointerX(event);
      startY = pointerY(event);
      lastTime = Date.now();
      velocity = 0;
      dragging = true;
      locked = false;
      heroEl.classList.add("is-swiping");
      if (heroEl.setPointerCapture) {
        try { heroEl.setPointerCapture(pointerId); } catch (err) {}
      }
    }

    function onPointerMove(event) {
      if (!dragging || event.pointerId !== pointerId) return;
      var x = pointerX(event);
      var y = pointerY(event);
      var dx = x - startX;
      var dy = y - startY;
      var now = Date.now();
      var dt = Math.max(1, now - lastTime);
      velocity = (x - lastX) / dt;
      lastX = x;
      lastTime = now;

      /* Decide horizontal vs vertical; vertical scroll cancels the swipe */
      if (!locked) {
        if (Math.abs(dx) < 8 && Math.abs(dy) < 8) return;
        if (Math.abs(dy) > Math.abs(dx)) {
          dragging = false;
          heroEl.classList.remove("is-swiping");
          return;
        }
        locked = true;
        if (heroSwiper.autoplay) {
          if (heroSwiper.autoplay.pause) heroSwiper.autoplay.pause();
        }
      }

      event.preventDefault();
      goingNext = dx < 0;
      var progress = Math.min(1, Math.abs(dx) / Math.max(1, heroSwiper.width * 0.38));
      previewIncoming(heroSwiper, goingNext, progress);
    }

    function onPointerUp(event) {
      if (!dragging || (event.pointerId != null && event.pointerId !== pointerId)) return;
      dragging = false;
      heroEl.classList.remove("is-swiping");
      if (heroEl.releasePointerCapture && pointerId != null) {
        try { heroEl.releasePointerCapture(pointerId); } catch (err) {}
      }
      pointerId = null;

      if (!locked) {
        resetInactiveMedia(heroSwiper, lastActiveSlide);
        return;
      }

      var dx = pointerX(event) - startX;
      var distance = Math.abs(dx);
      var fastFlick = Math.abs(velocity) > 0.45;
      var passed = distance > Math.max(56, heroSwiper.width * 0.12) || fastFlick;

      if (passed) {
        wipeFromLeft = dx > 0;
        if (dx < 0) heroSwiper.slideNext();
        else heroSwiper.slidePrev();
      } else {
        resetInactiveMedia(heroSwiper, lastActiveSlide);
      }

      if (heroSwiper.autoplay && !autoplayStoppedByScroll && !reduceMotion) {
        if (heroSwiper.autoplay.resume) heroSwiper.autoplay.resume();
        else if (heroSwiper.autoplay.start) heroSwiper.autoplay.start();
      }
    }

    heroEl.addEventListener("pointerdown", onPointerDown);
    heroEl.addEventListener("pointermove", onPointerMove, { passive: false });
    heroEl.addEventListener("pointerup", onPointerUp);
    heroEl.addEventListener("pointercancel", onPointerUp);
  })();

  /* Dragging thumbs should drive the main hero to that slide */
  thumbsSwiper.on("sliderMove", function () {
    thumbsDragging = true;
  });

  thumbsSwiper.on("realIndexChange", function () {
    if (!thumbsDragging) return;
    thumbsDragging = false;
    var heroIndex = (this.realIndex - 1 + totalSlides) % totalSlides;
    setThumbActive(heroIndex);
    if (heroSwiper.realIndex !== heroIndex) {
      heroSwiper.slideToLoop(heroIndex);
    }
  });

  /* Click a thumbnail to jump the hero to that index */
  thumbsEl.addEventListener("click", function (event) {
    var slide = event.target.closest(".swiper-slide");
    if (!slide || !thumbsEl.contains(slide)) return;
    var index = Number(slide.getAttribute("data-hero-index"));
    if (Number.isNaN(index)) return;
    heroSwiper.slideToLoop(index);
  });

  /* ----------------------------------------------------------
     Hover nav: left half → prev arrow, right half → next arrow
  ---------------------------------------------------------- */
  (function bindHoverArrows() {
    var banner = document.querySelector(".heroBanner");
    var prevNav = document.querySelector(".hero-nav--prev");
    var nextNav = document.querySelector(".hero-nav--next");
    var canHover = window.matchMedia("(hover: hover) and (pointer: fine)").matches;
    if (!banner || !prevNav || !nextNav || !canHover) return;

    var hoverSide = "";
    var moveRaf = 0;
    var pendingX = null;
    var pendingY = null;

    function setSide(side) {
      if (hoverSide === side) return;
      hoverSide = side;
      banner.classList.toggle("is-hover-left", side === "left");
      banner.classList.toggle("is-hover-right", side === "right");
    }

    function clearSide() {
      hoverSide = "";
      banner.classList.remove("is-hover-left", "is-hover-right");
    }

    function placeArrows(clientX, clientY) {
      var rect = banner.getBoundingClientRect();
      var x = clientX - rect.left;
      var y = clientY - rect.top;
      var pad = 40;
      x = Math.min(rect.width - pad, Math.max(pad, x));
      y = Math.min(rect.height - pad, Math.max(pad, y));
      prevNav.style.left = x + "px";
      prevNav.style.top = y + "px";
      nextNav.style.left = x + "px";
      nextNav.style.top = y + "px";
    }

    function isOverHeroCopy(event) {
      if (event.target.closest(".hero-thumbs-wrap, .hero-slide__content, .hero-slide__caption, .hero-slide__cta, a, button:not(.hero-nav)")) {
        return true;
      }

      var slide = banner.querySelector(".swiper-slide-active");
      if (!slide) return false;

      var copy = slide.querySelectorAll(".hero-slide__caption, .hero-slide__cta");
      var pad = 36;
      var x = event.clientX;
      var y = event.clientY;
      for (var i = 0; i < copy.length; i++) {
        var box = copy[i].getBoundingClientRect();
        if (
          x >= box.left - pad &&
          x <= box.right + pad &&
          y >= box.top - pad &&
          y <= box.bottom + pad
        ) {
          return true;
        }
      }
      return false;
    }

    banner.addEventListener("mousemove", function (event) {
      if (isOverHeroCopy(event)) {
        clearSide();
        return;
      }

      var rect = banner.getBoundingClientRect();
      setSide(event.clientX < rect.left + rect.width / 2 ? "left" : "right");

      pendingX = event.clientX;
      pendingY = event.clientY;
      if (moveRaf) return;
      moveRaf = requestAnimationFrame(function () {
        moveRaf = 0;
        if (pendingX != null && pendingY != null) placeArrows(pendingX, pendingY);
      });
    });

    banner.addEventListener("mouseleave", function () {
      clearSide();
      pendingX = null;
      pendingY = null;
    });
  })();

  /* ----------------------------------------------------------
    Page scroll: scale the banner, lift captions out, stop autoplay
  ---------------------------------------------------------- */
  var bannerEl = document.querySelector(".heroBanner");
  var liftEls = bannerEl
    ? bannerEl.querySelectorAll(".hero-slide__content, .hero-thumbs-wrap")
    : [];
  var SCALE_MIN = 0.75;
  var AUTOPLAY_STOP_AT_VH = 0.5;
  var CAPTION_FADE_BY = 0.42;
  var CAPTION_LIFT_PX = 140;
  var scrollTick = false;

  function heroScrollProgress() {
    if (!bannerEl) return 0;
    var height = bannerEl.offsetHeight || 1;
    return Math.min(1, Math.max(0, -bannerEl.getBoundingClientRect().top / height));
  }

  function syncHeroAutoplay() {
    if (!heroSwiper.autoplay || reduceMotion) return;
    var vh = window.innerHeight || 1;
    var scrolled = bannerEl
      ? Math.max(0, -bannerEl.getBoundingClientRect().top)
      : window.pageYOffset || 0;
    var shouldStop = scrolled >= vh * AUTOPLAY_STOP_AT_VH;
    if (shouldStop && !autoplayStoppedByScroll) {
      heroSwiper.autoplay.stop();
      autoplayStoppedByScroll = true;
      pauseHeroVideos();
    } else if (!shouldStop && autoplayStoppedByScroll) {
      autoplayStoppedByScroll = false;
      if (!playActiveHeroVideo(heroSwiper)) {
        if (heroSwiper.autoplay && heroSwiper.autoplay.start) heroSwiper.autoplay.start();
      }
    }
  }

  /* Caption, CTA, and thumbs travel up and fade as the banner leaves */
  function syncHeroCaption(progress) {
    var t = Math.min(1, progress / CAPTION_FADE_BY);
    var y = reduceMotion ? 0 : -(t * CAPTION_LIFT_PX);
    var opacity = 1 - t;
    var hidden = t > 0.92;

    for (var i = 0; i < liftEls.length; i += 1) {
      var el = liftEls[i];
      el.style.transform = y ? "translate3d(0, " + y + "px, 0)" : "none";
      el.style.opacity = String(opacity);
      el.style.pointerEvents = hidden ? "none" : "";
    }
  }

  function updateHeroOnScroll() {
    scrollTick = false;
    var progress = heroScrollProgress();
    if (!reduceMotion && bannerEl) {
      var scale = 1 - progress * (1 - SCALE_MIN);
      bannerEl.style.transform = scale >= 0.999 ? "none" : "scale(" + scale + ")";
    }
    syncHeroCaption(progress);
    syncHeroAutoplay();
  }

  function onHeroScroll() {
    if (scrollTick) return;
    scrollTick = true;
    requestAnimationFrame(updateHeroOnScroll);
  }

  window.addEventListener("scroll", onHeroScroll, { passive: true });
  window.addEventListener("resize", function () {
    onHeroScroll();
    scheduleHeroMobileHeight();
  });
  window.addEventListener("load", scheduleHeroMobileHeight);
  updateHeroOnScroll();
});
