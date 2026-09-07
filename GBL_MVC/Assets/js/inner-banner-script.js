document.addEventListener("DOMContentLoaded", () => {
// Background image parallax
gsap.to(".innerbanner-image", {
    yPercent: 15,
    ease: "none",
    scrollTrigger: {
        trigger: ".inside-banner-outer",
        start: "top top",
        end: "bottom top",
        scrub: true
    }
});

// Slow zoom while scrolling
gsap.fromTo(".innerbanner-image",
{
    scale: 1.15
},
{
    scale: 1,
    ease: "none",
    scrollTrigger: {
        trigger: ".inside-banner-outer",
        start: "top top",
        end: "bottom top",
        scrub: true
    }
});

// Text reveal
gsap.from(".innerbanner-caption",{
    y:80,
    opacity:0,
    duration:1.4,
    ease:"power4.out"
});

});



