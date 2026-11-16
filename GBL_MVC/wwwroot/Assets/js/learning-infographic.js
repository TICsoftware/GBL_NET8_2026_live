document.addEventListener("DOMContentLoaded", function () {
  var root = document.querySelector(".ld-flow");
  if (!root) return;

  var steps = root.querySelectorAll(".ld-step");
  var reduce = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  function showAll() {
    steps.forEach(function (step) { step.classList.add("is-inview"); });
  }

  if (reduce || !("IntersectionObserver" in window)) {
    showAll();
    return;
  }

  var observer = new IntersectionObserver(function (entries) {
    entries.forEach(function (entry) {
      if (entry.isIntersecting) entry.target.classList.add("is-inview");
      else entry.target.classList.remove("is-inview");
    });
  }, { threshold: 0 });

  steps.forEach(function (step) { observer.observe(step); });
});
