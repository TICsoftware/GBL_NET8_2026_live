gsap.registerPlugin(ScrollTrigger);

const signatureSection = document.querySelector(".signature-section");

if (signatureSection) {
gsap.set(".signature-bg", {
  transformOrigin: "50% 50%",
});

  // Plate + background — unchanged
  const signatureTimeline = gsap.timeline({
    scrollTrigger: {
      trigger: ".signature-section",
      start: "top 75%",
      end: "top 20%",
      scrub: 2,
      invalidateOnRefresh: true,
    },
  });

  signatureTimeline
   .fromTo(
    ".signature-plate-image",
    { rotation: 90, transformOrigin: "50% 50%" },
    { rotation: 0, ease: "none" },
    0,
  )
  .fromTo(
    ".signature-bg",
    { rotation: -90, transformOrigin: "50% 50%" },
    { rotation: 0, ease: "none" },
    0,
  );

  // Garlic + leaf — starts when section top hits 50% of viewport
  const garnishTimeline = gsap.timeline({
    scrollTrigger: {
      trigger: ".signature-section",
      start: "top 50%",
      end: "top 20%",
      scrub: 1.2,
      // markers: true,
    },
  });

  garnishTimeline
    .fromTo(
      ".signature-garlic-img",
      {
        xPercent: 20,
        yPercent: -130,
        rotation: -28,
        opacity: 0,
        transformOrigin: "50% 50%",
      },
      {
        xPercent: 15,
        yPercent: 20,
        rotation: 0,
        opacity: 1,
        ease: "none",
      },
      0.2,
    )
    .fromTo(
      ".signature-leaf-img",
      {
        xPercent: 0,
        yPercent: -150,
        rotation: 28,
        opacity: 0,
        transformOrigin: "50% 50%",
      },
      {
        xPercent: -50,
        yPercent: 30,
        rotation: 0,
        opacity: 1,
        ease: "none",
      },
      0.3,
    );
}