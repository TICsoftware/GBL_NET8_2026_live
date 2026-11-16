/* ---------------------------------------
   PRODUCT INSIDE — media parallax only
   .product-inside-banner-outer .product-inside-banner__media img
--------------------------------------- */
document.addEventListener("DOMContentLoaded", function () {
  initProductBannerParallax();
  initPackagingSlider();
  initSdsDownload();
  initEnquiryModal();
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

function initEnquiryModal() {
  var modal = document.getElementById("enquiry-modal");
  if (!modal || typeof modal.showModal !== "function") return;

  var panel = modal.querySelector(".enquiry-modal__panel");
  var backdrop = modal.querySelector(".enquiry-modal__backdrop");
  var scroller = modal.querySelector(".enquiry-modal__scroll");
  var form = modal.querySelector("#enquiry-form");
  var closing = false;
  var closeTimer = 0;

  function lockPage() {
    document.documentElement.style.overflow = "hidden";
    if (window.lenis && typeof window.lenis.stop === "function") {
      window.lenis.stop();
    }
  }

  function unlockPage() {
    if (window.lenis && typeof window.lenis.start === "function") {
      window.lenis.start();
    }
    document.documentElement.style.overflow = "";
  }

  function stopMotion() {
    if (typeof gsap === "undefined") return;
    gsap.killTweensOf(panel);
    gsap.killTweensOf(backdrop);
  }

  function finishClose() {
    window.clearTimeout(closeTimer);
    closing = false;
    stopMotion();
    if (modal.open) modal.close();
    modal.classList.remove("is-closing", "is-open");
    if (typeof gsap !== "undefined") {
      gsap.set(panel, { clearProps: "transform" });
      gsap.set(backdrop, { clearProps: "opacity" });
    }
    panel.style.transition = "";
    panel.style.transform = "";
    panel.style.opacity = "";
    unlockPage();
  }

  function openModal() {
    window.clearTimeout(closeTimer);
    closing = false;
    stopMotion();
    if (!modal.open) {
      modal.showModal();
      lockPage();
    }
    if (scroller) scroller.scrollTop = 0;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
      if (typeof gsap !== "undefined") {
        gsap.set(panel, { clearProps: "transform" });
        gsap.set(backdrop, { clearProps: "opacity" });
      }
      modal.classList.remove("is-closing");
      modal.classList.add("is-open");
      return;
    }
    panel.style.transition = "";
    panel.style.transform = "";
    if (typeof gsap !== "undefined") gsap.set(panel, { clearProps: "transform" });
    modal.classList.remove("is-closing", "is-open");
    if (typeof gsap !== "undefined") gsap.set(backdrop, { opacity: 0 });
    void panel.offsetWidth;
    requestAnimationFrame(function () {
      if (closing || !modal.open) return;
      modal.classList.add("is-open");
      if (typeof gsap === "undefined") return;
      gsap.to(backdrop, { opacity: 1, duration: 0.7, ease: "power2.out", overwrite: true });
    });
  }

  function closeModal() {
    if (!modal.open || closing) return;
    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches || !modal.classList.contains("is-open") || typeof gsap === "undefined") {
      finishClose();
      return;
    }
    closing = true;
    stopMotion();
    gsap.set(panel, { y: 0 });
    gsap.set(backdrop, { opacity: 1 });
    modal.classList.add("is-closing");
    modal.classList.remove("is-open");
    var done = false;
    function onEnd() {
      if (done) return;
      done = true;
      finishClose();
    }
    var exit = gsap.timeline({ onComplete: onEnd });
    exit.to(panel, { y: "100vh", duration: 0.8, ease: "power2.inOut" }, 0);
    exit.to(backdrop, { opacity: 0, duration: 0.8, ease: "power2.in" }, 0);
    closeTimer = window.setTimeout(onEnd, 960);
  }

  document.querySelectorAll("[data-enquiry-open]").forEach(function (button) {
    button.addEventListener("click", function (event) {
      event.preventDefault();
      openModal();
    });
  });

  modal.querySelectorAll("[data-enquiry-close]").forEach(function (el) {
    el.addEventListener("click", closeModal);
  });

  modal.addEventListener("cancel", function (event) {
    event.preventDefault();
    closeModal();
  });

  if (form) {
    form.addEventListener("submit", function (event) {
      event.preventDefault();
    });
  }
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
