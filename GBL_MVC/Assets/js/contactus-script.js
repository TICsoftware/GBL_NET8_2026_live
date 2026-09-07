document.addEventListener("DOMContentLoaded", function () {
// --------------------------------------------
// FLOAT ANIMATION
// --------------------------------------------
function initFloatIcons() {
  const elements = document.querySelectorAll(".floating-icon");

  elements.forEach((el, i) => {
    const depth = (i % 3) + 1;

    // small variation per element
    const moveX = gsap.utils.random(8, 15) * depth * 0.3;
    const moveY = gsap.utils.random(10, 18) * depth * 0.3;
    const rotate = gsap.utils.random(2, 5);
    const duration = gsap.utils.random(3, 5);

    // initial slight offset
    gsap.set(el, {
      x: gsap.utils.random(-20, 20),
      y: gsap.utils.random(-20, 20),
      rotation: gsap.utils.random(-5, 5)
    });

    // smooth floating loop (no harsh jumps)
    gsap.to(el, {
      x: `+=${moveX}`,
      y: `+=${moveY}`,
      rotation: `+=${rotate}`,
      duration: duration,
      ease: "sine.inOut",
      yoyo: true,
      repeat: -1
    });
  });
}

initFloatIcons();

gsap.fromTo(
  ".location-wrapper .map-wrapper",
  { scale: 0.75, opacity: 0 },
  {
    scale: 1,
    opacity: 1,
    duration: 0.9,
    ease: "power2.out",
    scrollTrigger: {
      trigger: ".location-wrapper",
      start: "top 80%",
      toggleActions: "play none none reset"
    }
  }
);


  const mapWrapper = document.querySelector(".map-wrapper");
  if (!mapWrapper) return;

  // Activate on click
  mapWrapper.addEventListener("click", function () {
    mapWrapper.classList.add("active");
  });

  // Disable when clicking outside
  document.addEventListener("click", function (e) {
    if (!mapWrapper.contains(e.target)) {
      mapWrapper.classList.remove("active");
    }
  });

  // âœ… Disable ONLY when section leaves viewport
  ScrollTrigger.create({
    trigger: ".location-wrapper",
    start: "top 45%",
    end: "bottom 15%",
    onLeave: () => mapWrapper.classList.remove("active"),
    onLeaveBack: () => mapWrapper.classList.remove("active")
  });


/* ===========================================================
   IMAGE PARALLAX
=========================================================== */
ScrollTrigger.matchMedia({
  "(min-width: 1024px)": () => {
    gsap.utils.toArray(".bg-parallax").forEach((section) => {
      const image = section.querySelector("img");

      gsap.fromTo(
        image,
        { y: -100 },
        {
          y: 100,
          ease: "none",
          scrollTrigger: {
            trigger: section,
            start: "top bottom",
            end: "bottom top",
            scrub: true
          }
        }
      );
    });
  }
});



});