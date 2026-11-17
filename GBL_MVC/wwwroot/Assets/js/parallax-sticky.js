/* ---------------------------------------
   PARALLAX STICKY — site-wide
   The image stays in view while the sibling column scrolls,
   then releases when that column ends.

   Markup (parent is the row that holds the image and the copy):

     <div class="any-row">
       <figure class="parallax-wrap parallax-sticky" data-parallax-travel="5">
         <img class="parallax-img" alt="">
       </figure>
       <div class="any-row__copy">...</div>
     </div>

   data-sticky-root=".selector"  optional ancestor to stick within
                                 (default is the parent row)
   Loaded after image-parallax.js.
--------------------------------------- */
document.addEventListener("DOMContentLoaded", function () {
  if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  gsap.registerPlugin(ScrollTrigger);

  var desktopMq = window.matchMedia("(min-width: 992px)");

  function scroller() {
    return desktopMq.matches ? document.documentElement : window;
  }

  function stickyRoot(wrap) {
    var selector = wrap.getAttribute("data-sticky-root");
    if (selector) {
      var found = wrap.closest(selector);
      if (found) return found;
    }
    return wrap.parentElement;
  }

  function clearStickyTriggers() {
    ScrollTrigger.getAll().slice().forEach(function (st) {
      var trigger = st.trigger;
      var ours = st.vars && st.vars.id === "parallax-sticky";
      var onStickyWrap = trigger && trigger.classList && trigger.classList.contains("parallax-sticky");
      if (!ours && !onStickyWrap) return;
      if (st.animation) st.animation.kill();
      st.kill();
    });
  }

  function bindWrap(wrap) {
    var img = wrap.querySelector(".parallax-img");
    var root = stickyRoot(wrap);
    if (!img || !root) return;

    var travel = parseFloat(wrap.dataset.parallaxTravel) || 12;
    var useRow = desktopMq.matches;

    gsap.fromTo(
      img,
      { yPercent: -travel },
      {
        yPercent: travel,
        ease: "none",
        force3D: true,
        scrollTrigger: {
          trigger: useRow ? root : wrap,
          scroller: scroller(),
          start: "top bottom",
          end: "bottom top",
          scrub: useRow ? 1.35 : 0.8,
          invalidateOnRefresh: true,
          id: "parallax-sticky"
        }
      }
    );
  }

  function bindAll() {
    clearStickyTriggers();
    document.querySelectorAll(".parallax-wrap.parallax-sticky").forEach(bindWrap);
  }

  bindAll();

  window.addEventListener("gbl:after-resize", function () {
    bindAll();
    ScrollTrigger.refresh();
  });
});
