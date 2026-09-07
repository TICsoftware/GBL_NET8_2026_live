// --------------------------------------------
// REVEAL ITEM
// --------------------------------------------

function initReveal() {
  gsap.utils.toArray(".reveal-item").forEach(item => {

    gsap.set(item, {
      autoAlpha: 0,
      y: 60,
      scale: 0.96,
      filter: "blur(10px)",
      willChange: "transform, opacity, filter"
    });

    const rect = item.getBoundingClientRect();
    const alreadyVisible = rect.top < window.innerHeight * 0.85;

    const tween = gsap.fromTo(item,
      { autoAlpha: 0, y: 60, scale: 0.96, filter: "blur(10px)" },
      {
        autoAlpha: 1,
        y: 0,
        scale: 1,
        filter: "none",
        duration: 0.8,
        ease: "expo.out",
        clearProps: "willChange,filter",
        paused: true
      }
    );

    if (alreadyVisible) {
      tween.play();
      return;
    }

    ScrollTrigger.create({
      trigger: item,
      start: "top 85%",
      toggleActions: "play none none reverse",
      invalidateOnRefresh: true,
      animation: tween
    });
  });
}

window.addEventListener("load", () => {
  initReveal();
  ScrollTrigger.refresh();
});