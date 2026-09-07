document.addEventListener('DOMContentLoaded', function () {

const modal = document.getElementById("videoModal");
const videoBox = document.getElementById("videoBox");
const closeBtn = document.getElementById("closeVideo");

const mp4 = document.getElementById("mp4Video");
const youtube = document.getElementById("youtubeVideo");

let originRect = null;
let clickPoint = { x: 0, y: 0 };
let isOpen = false;


// 👉 OPEN
document.addEventListener("click", (e) => {

  const trigger = e.target.closest("[data-video-trigger]");
  if (!trigger) return;

  e.preventDefault();
  if (isOpen) return;
  isOpen = true;

  // 👉 STORE CLICK POSITION (🔥 KEY)
  clickPoint.x = e.clientX;
  clickPoint.y = e.clientY;

  // 👉 fallback rect (for closing animation)
  const circle = trigger.querySelector(".play-btn") || trigger;
  const rect = circle.getBoundingClientRect();

  originRect = {
    top: rect.top,
    left: rect.left,
    width: rect.width,
    height: rect.height,
    borderRadius: window.getComputedStyle(circle).borderRadius
  };

  modal.classList.remove("hidden");

  if (typeof lenis !== "undefined") lenis.stop();

  // 👉 START FROM CLICK POINT (tiny circle)
  gsap.set(videoBox, {
    top: clickPoint.y,
    left: clickPoint.x,
    width: 20,
    height: 20,
    xPercent: -50,
    yPercent: -50,
    borderRadius: "50%",
    position: "fixed",
    overflow: "hidden",
    willChange: "transform, width, height"
  });

  // 👉 VIDEO SETUP
  const type = trigger.dataset.videoType;
  const src = trigger.dataset.videoSrc;

  if (type === "mp4") {
    mp4.src = src;
    mp4.play();
    mp4.classList.remove("hidden");
    youtube.classList.add("hidden");
  } else {
    youtube.src = `${src}?autoplay=1`;
    youtube.classList.remove("hidden");
    mp4.classList.add("hidden");
  }

  // 👉 RESPONSIVE SIZE
  const isMobile = window.innerWidth < 640;

  let width = isMobile
    ? window.innerWidth * 0.92
    : Math.min(window.innerWidth * 0.9, 1000);

  let height = width * 9 / 16;

  const maxHeight = window.innerHeight * 0.85;
  if (height > maxHeight) {
    height = maxHeight;
    width = height * 16 / 9;
  }

  // 👉 MORPH ANIMATION (🔥 MAGIC)
  gsap.to(videoBox, {
    duration: 0.7,
    ease: "power4.inOut",
    top: "50%",
    left: "50%",
    width: width,
    height: height,
    borderRadius: isMobile ? "12px" : "16px"
  });

  gsap.to(".modal-bg", {
    opacity: 1,
    backdropFilter: "blur(14px)",
    duration: 0.5
  });

});


// 👉 CLOSE
function closeModal() {

  if (!originRect || !isOpen) return;

  mp4.pause();
  mp4.src = "";
  youtube.src = "";

  // 👉 shrink back to click point (🔥 feels natural)
  gsap.to(videoBox, {
    duration: 0.5,
    ease: "power3.inOut",
    top: clickPoint.y,
    left: clickPoint.x,
    width: 20,
    height: 20,
    xPercent: -50,
    yPercent: -50,
    borderRadius: "50%",
    onComplete: () => {
      modal.classList.add("hidden");

      if (typeof lenis !== "undefined") lenis.start();

      isOpen = false;
    }
  });

  gsap.to(".modal-bg", {
    opacity: 0,
    backdropFilter: "blur(0px)",
    duration: 0.3
  });

}


// 👉 EVENTS
closeBtn.addEventListener("click", closeModal);
modal.querySelector(".modal-bg").addEventListener("click", closeModal);

document.addEventListener("keydown", (e) => {
  if (e.key === "Escape") closeModal();
});

});