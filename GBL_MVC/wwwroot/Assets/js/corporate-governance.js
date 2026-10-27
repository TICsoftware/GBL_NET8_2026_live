document.addEventListener("DOMContentLoaded", function () {
  document.querySelectorAll("[data-cg-docs-grid]").forEach(function (grid) {
    var size = parseInt(grid.getAttribute("data-page-size") || "12", 10);
    var wrap = grid.parentElement.querySelector("[data-cg-load-more-wrap]");
    var btn = wrap && wrap.querySelector("[data-cg-load-more]");
    var cards = Array.prototype.slice.call(grid.children);
    var shown = size;

    function apply() {
      cards.forEach(function (card, index) {
        card.hidden = index >= shown;
      });
      if (wrap) wrap.hidden = shown >= cards.length;
    }

    apply();

    if (!btn) return;

    btn.addEventListener("click", function () {
      shown += size;
      apply();

      if (typeof ScrollTrigger !== "undefined") {
        ScrollTrigger.refresh();
      }
    });
  });
});
