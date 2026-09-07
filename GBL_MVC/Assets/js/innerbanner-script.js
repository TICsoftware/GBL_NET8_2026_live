document.addEventListener("DOMContentLoaded", () => {

// ==========================
// DEVICE CHECK
// ==========================
const isDesktop = window.innerWidth >= 1024;


// ==========================
// WRAP TEXT (SAFE)
// ==========================
function wrapText(node) {
  if (node.nodeType === 3) {
    const frag = document.createDocumentFragment();
    const parts = node.textContent.match(/\S+|\s+/g);

    parts.forEach(part => {
      if (part.trim() === "") {
        frag.appendChild(document.createTextNode(part));
      } else {
        const wordSpan = document.createElement("span");
        wordSpan.className = "word";

        part.split("").forEach(char => {
          const span = document.createElement("span");
          span.className = "char";
          span.textContent = char;
          wordSpan.appendChild(span);
        });

        frag.appendChild(wordSpan);
      }
    });

    return frag;
  }

  if (node.nodeType === 1) {
    const el = node.cloneNode(false);
    node.childNodes.forEach(child => {
      el.appendChild(wrapText(child));
    });
    return el;
  }

  return document.createTextNode("");
}


// ==========================
// INNER TITLE ANIMATION (COMMON)
// ==========================
function innerBannerTitleAnimation() {

  const titles = document.querySelectorAll(".innerbanner-title-heading");
  if (!titles.length) return;

  titles.forEach(title => {

    if (title.classList.contains("inner-split-done")) return;

    const frag = document.createDocumentFragment();
    title.childNodes.forEach(node => {
      frag.appendChild(wrapText(node));
    });

    title.innerHTML = "";
    title.appendChild(frag);
    title.classList.add("inner-split-done");

    const chars = title.querySelectorAll(".char");

    if (isDesktop) {
      gsap.set(chars, {
        opacity: 0,
        x: 100,
        rotateY: 50,
        filter: "blur(6px)"
      });

      gsap.to(chars, {
        x: 0,
        rotateY: 0,
        opacity: 1,
        filter: "blur(0px)",
        duration: 0.6,
        ease: "power3.out",
        stagger: 0.025
      });
    } else {
      gsap.set(chars, {
        opacity: 0,
        y: 30
      });

      gsap.to(chars, {
        opacity: 1,
        y: 0,
        duration: 0.4,
        ease: "power2.out",
        stagger: 0.02
      });
    }

  });
}


// ==========================
// PAGE LOAD ANIMATION
// ==========================
window.addEventListener("load", () => {

  // ✅ FIX: add is-ready AFTER full load (prevents flicker)
  document.body.classList.add("is-ready");

  innerBannerTitleAnimation();

  const tl = gsap.timeline();

  tl.from(".breadcrumb_container", {
    opacity: 0,
    x: 30,
    duration: 0.6,
    ease: "power2.out"
  }, 0);

  tl.from(".inner-banner-vector", {
    x: -80,
    duration: 1,
    ease: "power3.out"
  }, 0);

  if (isDesktop) {

    tl.fromTo(".bannerinner-picture img",
      { scale: 1.2 },
      { scale: 1, duration: 2, ease: "power3.out" },
      0
    );

    tl.from(".masked-img", {
      rotate: 5,
      duration: 1.2,
      ease: "power3.out"
    }, 0.3);

  } else {

    tl.from(".bannerinner-picture img", {
      scale: 1.2,
      duration: 0.8,
      ease: "power2.out"
    }, 0);

  }

});


// ==========================
// SCROLL EFFECTS
// ==========================
if (isDesktop) {

  gsap.to(".bannerinner-picture img", {
    scale: 1.15,
    ease: "none",
    scrollTrigger: {
      trigger: ".banner-inner-section",
      start: "top top",
      end: "bottom top",
      scrub: 1.5
    }
  });

  gsap.to(".inner-banner-vector", {
    x: -80,
    scrollTrigger: {
      trigger: ".banner-inner-section",
      start: "top top",
      end: "bottom top",
      scrub: 1.5
    }
  });

  gsap.to(".banner-mask, .banner-vector-img", {
    y: "+=10",
    duration: isDesktop ? 3 : 2,
    repeat: -1,
    yoyo: true,
    ease: "sine.inOut"
  });

}

});