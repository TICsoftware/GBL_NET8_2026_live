/**
 * Sustainability page-intro — landscape mantra panel
 * Background travels with scroll; quote rises in; body copy fills.
 */
document.addEventListener("DOMContentLoaded", function () {
  var section = document.querySelector(".page-intro-outer--mantra");
  if (!section) return;

  var img = section.querySelector(".page-intro-mantra__img");
  var quote = section.querySelector(".page-intro-mantra__quote");
  var copy = section.querySelector(".page-intro-mantra__copy");
  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var isMobile = window.matchMedia("(max-width: 992px)").matches;
  var stScroller = isMobile ? window : document.documentElement;
  var FILL_FROM = "#4a4e55";
  var FILL_TO = "#4a4e55";

  function wrapWords(el) {
    if (!el || el.dataset.introFill === "ready") return;
    var walker = document.createTreeWalker(el, NodeFilter.SHOW_TEXT, null);
    var nodes = [];
    while (walker.nextNode()) nodes.push(walker.currentNode);
    nodes.forEach(function (node) {
      var text = node.nodeValue;
      if (!text || !text.trim()) return;
      var frag = document.createDocumentFragment();
      text.split(/(\s+)/).forEach(function (part) {
        if (!part) return;
        if (/^\s+$/.test(part)) {
          frag.appendChild(document.createTextNode(part));
          return;
        }
        var span = document.createElement("span");
        span.className = "page-intro-fill-word";
        span.textContent = part;
        frag.appendChild(span);
      });
      node.parentNode.replaceChild(frag, node);
    });
    el.dataset.introFill = "ready";
  }

  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined" || reduceMotion) {
    if (copy) wrapWords(copy);
    return;
  }

  gsap.registerPlugin(ScrollTrigger);

  if (img) {
    gsap.fromTo(
      img,
      { yPercent: -10 },
      {
        yPercent: 10,
        ease: "none",
        force3D: true,
        scrollTrigger: {
          trigger: section,
          scroller: stScroller,
          start: "top bottom",
          end: "bottom top",
          scrub: 1.2,
          invalidateOnRefresh: true,
        },
      }
    );
  }

  if (quote) {
    gsap.fromTo(
      quote,
      { y: 48, autoAlpha: 0.35 },
      {
        y: 0,
        autoAlpha: 1,
        ease: "none",
        force3D: true,
        scrollTrigger: {
          trigger: quote,
          scroller: stScroller,
          start: "top 88%",
          end: "top 52%",
          scrub: 1.25,
          invalidateOnRefresh: true,
        },
      }
    );
  }

  var isPhone = window.matchMedia("(max-width: 767px)").matches;

  if (copy) {
    wrapWords(copy);
    var words = copy.querySelectorAll(".page-intro-fill-word");
    if (words.length && !isPhone) {
      gsap.set(words, { color: FILL_FROM });
      gsap.to(words, {
        color: FILL_TO,
        stagger: 0.05,
        ease: "none",
        immediateRender: false,
        scrollTrigger: {
          trigger: copy,
          scroller: stScroller,
          start: "top 82%",
          end: "bottom 46%",
          scrub: 1.15,
          invalidateOnRefresh: true,
        },
      });
    }
  }
});
document.addEventListener("DOMContentLoaded", function () {
  var el = document.querySelector(".turningCommitment-cards-outer, .carbonCircularity-cards");
  if (!el || typeof Swiper === "undefined") return;

  var swiper = null;
  var riseTl = null;
  var sliderOn = null;
  var MOBILE_MAX = 767;
  var CARD_RISE = {
    y: 140,
    duration: 1.15,
    stagger: 0.28,
    scrub: 1.15,
    listStart: "top 90%",
    listEnd: "top 38%"
  };

  function commitmentCards() {
    return el.querySelectorAll(".turningCommitment-card, .carbonCircularity-card");
  }

  function playCardRise() {
    var cards = commitmentCards();
    if (!cards.length || typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;

    gsap.registerPlugin(ScrollTrigger);
    if (riseTl) {
      if (riseTl.scrollTrigger) riseTl.scrollTrigger.kill();
      riseTl.kill();
      riseTl = null;
    }
    gsap.set(cards, { clearProps: "transform,opacity,visibility" });

    var stScroller = window.matchMedia("(max-width: 992px)").matches
      ? window
      : document.documentElement;

    riseTl = gsap.timeline({
      defaults: { force3D: true, ease: "power2.out" },
      scrollTrigger: {
        id: "turning-commitment-card-rise",
        trigger: el,
        scroller: stScroller,
        start: CARD_RISE.listStart,
        end: CARD_RISE.listEnd,
        scrub: CARD_RISE.scrub,
        invalidateOnRefresh: true
      }
    });

    cards.forEach(function (card, i) {
      riseTl.fromTo(
        card,
        { autoAlpha: 0, y: CARD_RISE.y },
        {
          autoAlpha: 1,
          y: 0,
          duration: CARD_RISE.duration,
          immediateRender: true
        },
        i * CARD_RISE.stagger
      );
    });
  }

  function isMobileView() {
    return window.innerWidth <= MOBILE_MAX;
  }

  function enableSlider() {
    if (swiper) return;
    var section = el.closest("section") || el.parentElement;
    swiper = new Swiper(el, {
      slidesPerView: 1,
      spaceBetween: 16,
      watchOverflow: true,
      pagination: {
        el: section.querySelector(".turningCommitment-pagination, .carbonCircularity-pagination"),
        clickable: true,
      },
      navigation: {
        nextEl: section.querySelector(".turningCommitment-nav--next, .carbonCircularity-nav--next"),
        prevEl: section.querySelector(".turningCommitment-nav--prev, .carbonCircularity-nav--prev"),
      },
    });
  }

  function disableSlider() {
    if (!swiper) return;
    swiper.destroy(true, true);
    swiper = null;
  }

  function syncMode() {
    var mobile = isMobileView();
    if (mobile === sliderOn) return;
    sliderOn = mobile;
    if (mobile) enableSlider();
    else disableSlider();
    playCardRise();
    if (typeof ScrollTrigger !== "undefined") ScrollTrigger.refresh();
  }

  syncMode();
  window.addEventListener("resize", function () {
    syncMode();
  });
});
