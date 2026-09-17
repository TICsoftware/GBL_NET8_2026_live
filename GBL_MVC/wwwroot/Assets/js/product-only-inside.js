/* ---------------------------------------
   PRODUCT INSIDE — media parallax only
   .product-inside-banner-outer .product-inside-banner__media img
--------------------------------------- */
document.addEventListener("DOMContentLoaded", function () {
  initProductBannerParallax();
  initPackagingSlider();
  initSdsDownload();
});

function initProductBannerParallax() {
  var section = document.querySelector(".product-inside-banner-outer");
  if (!section) return;

  var wrap = section.querySelector(".product-inside-banner__media");
  var img = wrap && wrap.querySelector("img");
  if (!img) return;

  var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  if (reduceMotion || typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") {
    return;
  }

  gsap.registerPlugin(ScrollTrigger);

  var isMobile = window.matchMedia("(max-width: 992px)").matches;
  var stScroller = isMobile ? window : document.documentElement;

  gsap.fromTo(
    img,
    { yPercent: -12 },
    {
      yPercent: 12,
      ease: "none",
      force3D: true,
      scrollTrigger: {
        trigger: wrap,
        scroller: stScroller,
        start: "top bottom",
        end: "bottom top",
        scrub: 0.8,
        invalidateOnRefresh: true
      }
    }
  );
}

function initPackagingSlider() {
  var block = document.querySelector(".packaging-application-outer .packaging-block");
  if (!block) return;

  var slider = block.querySelector(".packaging-swiper");
  var slides = slider ? slider.querySelectorAll(".swiper-slide") : [];
  var nav = block.querySelector(".packaging-block__nav");
  if (!slider || slides.length <= 2) return;
  if (typeof Swiper === "undefined") return;

  block.classList.add("is-slider");
  if (nav) nav.hidden = false;

  function createSlider() {
    if (slider.swiper) {
      slider.swiper.update();
      return slider.swiper;
    }

    return new Swiper(slider, {
      slidesPerView: 1,
      spaceBetween: 16,
      watchOverflow: true,
      observer: true,
      observeParents: true,
      observeSlideChildren: true,
      autoHeight: true,
      navigation: {
        prevEl: block.querySelector(".packaging-nav--prev"),
        nextEl: block.querySelector(".packaging-nav--next")
      },
      pagination: {
        el: block.querySelector(".packaging-pagination"),
        clickable: true
      },
      breakpoints: {
        700: {
          slidesPerView: 2,
          spaceBetween: 16,
          autoHeight: false
        }
      }
    });
  }

  var packagingSwiper;
  window.requestAnimationFrame(function () {
    packagingSwiper = createSlider();
  });

  window.addEventListener("load", function () {
    if (packagingSwiper && packagingSwiper.update) packagingSwiper.update();
  });

  window.addEventListener("resize", function () {
    if (packagingSwiper && packagingSwiper.update) packagingSwiper.update();
  });
}

function initSdsDownload() {
  var links = document.querySelectorAll(".technical-Safety-outer [data-sds-download]");
  if (!links.length) return;

  links.forEach(function (link) {
    link.addEventListener("click", function (event) {
      event.preventDefault();
      var url = link.getAttribute("href");
      var filename = link.getAttribute("download") || "SDS.pdf";
      if (!url || url === "#") return;
      downloadSdsFile(url, filename);
    });
  });
}

function downloadSdsFile(url, filename) {
  fetch(url, { credentials: "same-origin" })
    .then(function (response) {
      if (!response.ok) throw new Error("SDS file not found");
      return response.blob();
    })
    .then(function (blob) {
      triggerSdsBlobDownload(blob, filename);
    })
    .catch(function () {
      var fallback = document.createElement("a");
      fallback.href = url;
      fallback.setAttribute("download", filename);
      fallback.rel = "noopener";
      document.body.appendChild(fallback);
      fallback.click();
      fallback.remove();
    });
}

function triggerSdsBlobDownload(blob, filename) {
  var objectUrl = URL.createObjectURL(blob);
  var a = document.createElement("a");
  a.href = objectUrl;
  a.download = filename;
  document.body.appendChild(a);
  a.click();
  a.remove();
  window.setTimeout(function () {
    URL.revokeObjectURL(objectUrl);
  }, 1000);
}
