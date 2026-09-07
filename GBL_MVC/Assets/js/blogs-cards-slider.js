  document.addEventListener("DOMContentLoaded", () => {
    gsap.registerPlugin(ScrollTrigger);

  function initSliders() {

    const slider = document.querySelector(".blogs-cards-slider");

    if (!slider) return;

    const totalSlides = slider.querySelectorAll(".swiper-slide").length;

    const swiper = new Swiper(slider, {
        slidesPerView: 1.1,
        spaceBetween: 20,
        centeredSlides: true,
        loop: totalSlides > 3,

        navigation: {
            nextEl: ".blogs-next",
            prevEl: ".blogs-prev",
        },

        breakpoints: {
            768: {
                slidesPerView: 2.2,
                centeredSlides: true,
            },
            1024: {
                slidesPerView: 3,
                centeredSlides: false,
                spaceBetween: 30,
            },
        },
    });

    const nav = document.querySelector(".blogs-slider-nav");

    function toggleNav() {
        if (window.innerWidth >= 1024) {
            nav.style.display = totalSlides > 3 ? "flex" : "none";
        } else {
            nav.style.display = totalSlides > 1 ? "flex" : "none";
        }
    }

    toggleNav();
    window.addEventListener("resize", toggleNav);
}

initSliders();

// ANIMATION
const section = document.querySelector(".blogs-cards-slider");

if (!section) return;

const cards = gsap.utils.toArray(".blogs-cards-slider .swiper-slide");

gsap.set(cards, {
  opacity: 0,
  y: 120,
  scale: 0.9,
  rotateX: 8,
  transformPerspective: 1000,
  transformOrigin: "center bottom",
});

const tl = gsap.timeline({
  scrollTrigger: {
    trigger: section,
    start: "top 78%",
    toggleActions: "play none none reverse",
    // markers: true,
  },
});

tl.to(cards, {
  opacity: 1,
  y: 0,
  scale: 1,
  rotateX: 0,
  duration: 1.15,
  ease: "power4.out",
  stagger: {
    each: 0.22,
  },
});



});
