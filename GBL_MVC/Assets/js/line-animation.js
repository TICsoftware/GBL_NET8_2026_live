document.addEventListener("DOMContentLoaded", () => {
// --------------------------------------------
// LINE ANIMATION
// --------------------------------------------

function initLineAnimation() {

  const sections = document.querySelectorAll(".tq-line-vector");

  sections.forEach((section) => {

    const path = section.querySelector(".linePath");
    const dot1 = section.querySelector(".dot1");
    const dot2 = section.querySelector(".dot2");

    if (!path || !dot1 || !dot2) return;

    const length = path.getTotalLength();

    const startOffset = 0.001;
    const endOffset = 0.999;

    gsap.set(path, {
      strokeDasharray: length,
      strokeDashoffset: length
    });

    function move(dot, progress) {
      const point = path.getPointAtLength(progress * length);
      gsap.set(dot, {
        x: point.x,
        y: point.y
      });
    }

    let dotAnim1, dotAnim2;

    function resetAll() {
      gsap.set(path, { strokeDashoffset: length });

      move(dot1, startOffset);
      move(dot2, endOffset);

      gsap.set([dot1, dot2], { opacity: 0 });

      dotAnim1 && dotAnim1.kill();
      dotAnim2 && dotAnim2.kill();
    }

    function startDots() {

      dotAnim1 = gsap.to({}, {
        duration: 15,
        repeat: -1,
        ease: "none",
        onUpdate() {
          const p = startOffset + (endOffset - startOffset) * this.progress();
          move(dot1, p);
        }
      });

      dotAnim2 = gsap.to({}, {
        duration: 15,
        repeat: -1,
        ease: "none",
        onUpdate() {
          const p = endOffset - (endOffset - startOffset) * this.progress();
          move(dot2, p);
        }
      });
    }

    resetAll(); // 👈 IMPORTANT (initial state)

    const tl = gsap.timeline({
      scrollTrigger: {
        trigger: section,
        start: "top 60%",
        end: "bottom top",

        // 🔥 FIXED BEHAVIOR
        toggleActions: "play none none none",

        // optional:
        // once: true, // use this if you NEVER want replay

        onEnter: () => {
          resetAll();
          tl.restart();
        },

        onLeaveBack: () => {
          resetAll();
        },

        invalidateOnRefresh: true
      }
    });

    tl.to(path, {
      strokeDashoffset: 0,
      duration: 2,
      ease: "power2.out"
    });

    tl.to([dot1, dot2], {
      opacity: 1,
      duration: 0.3,
      onComplete: startDots
    });

  });

}

initLineAnimation();

// LINE ANIMATION END

});