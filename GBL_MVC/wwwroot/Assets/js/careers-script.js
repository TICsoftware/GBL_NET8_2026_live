document.addEventListener("DOMContentLoaded", function () {
  var el = document.querySelector(".whyJoin-cards-outer");
  if (!el) return;

  function placeFill(card, e) {
    var media = card.querySelector(".whyJoin-card__media");
    var cardRect = card.getBoundingClientRect();
    var mediaRect = media ? media.getBoundingClientRect() : null;
    var top = mediaRect ? Math.max(0, mediaRect.bottom - cardRect.top) : cardRect.height * 0.46;
    var fillW = cardRect.width;
    var fillH = Math.max(0, cardRect.height - top);
    var x = e ? e.clientX - cardRect.left : fillW / 2;
    var y = e ? e.clientY - cardRect.top - top : fillH / 2;
    x = Math.min(Math.max(x, 0), fillW);
    y = Math.min(Math.max(y, 0), fillH);
    var radius = Math.ceil(Math.hypot(Math.max(x, fillW - x), Math.max(y, fillH - y))) + 16;
    card.style.setProperty("--fill-top", top + "px");
    card.style.setProperty("--hx", x + "px");
    card.style.setProperty("--hy", y + "px");
    card.style.setProperty("--hr", radius + "px");
  }

  el.querySelectorAll(".whyJoin-card").forEach(function (card) {
    placeFill(card, null);
    card.addEventListener("mouseenter", function (e) {
      placeFill(card, e);
      card.classList.add("is-filled");
    });
    card.addEventListener("mouseleave", function () {
      card.classList.remove("is-filled");
    });
  });

  window.addEventListener("resize", function () {
    el.querySelectorAll(".whyJoin-card").forEach(function (card) {
      placeFill(card, null);
    });
  });

  if (typeof Swiper === "undefined") return;

  var swiper = null;
  var MOBILE_MAX = 767;

  function isMobileView() {
    return window.innerWidth <= MOBILE_MAX;
  }

  function enableSlider() {
    if (swiper) return;
    swiper = new Swiper(el, {
      slidesPerView: 1,
      spaceBetween: 16,
      watchOverflow: true,
      pagination: {
        el: ".whyJoin-pagination",
        clickable: true,
      },
      navigation: {
        nextEl: ".whyJoin-nav--next",
        prevEl: ".whyJoin-nav--prev",
      },
    });
  }

  function disableSlider() {
    if (!swiper) return;
    swiper.destroy(true, true);
    swiper = null;
  }

  function syncMode() {
    if (isMobileView()) enableSlider();
    else disableSlider();
  }

  syncMode();
  window.addEventListener("resize", function () {
    syncMode();
  });
});
