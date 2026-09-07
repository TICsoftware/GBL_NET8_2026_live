/* ======================
   PREMIUM PLAYGROUND
====================== */
document.addEventListener("DOMContentLoaded", () => {

    
// ANIMATION
const section = document.querySelector(".three-box-slider");

if (!section) return;

const cards = gsap.utils.toArray(".three-box-slider .swiper-slide");

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

  new Swiper(".mySwiper", {
            slidesPerView: 1,
            spaceBetween: 30,
            loop: true,
            pagination: {
                el: ".swiper-pagination-custom",
                clickable: true,
                renderBullet: (index, className) =>
                    index < 3 ? `<span class="${className}"></span>` : "",
            },
            navigation: {
                nextEl: ".swiper-button-next-custom",
                prevEl: ".swiper-button-prev-custom",
            },
            breakpoints: {
                640: { slidesPerView: 1 },
                768: { slidesPerView: 2 },
                1024: { slidesPerView: 3 },
            },
        });