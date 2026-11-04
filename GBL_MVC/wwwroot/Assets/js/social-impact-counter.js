document.addEventListener("DOMContentLoaded", function () {
  /* ---------------------------------------
     Impact counters play only once their tile
     has finished sliding in and is on screen, so
     the whole roll is seen. They reset below
     the viewport or when the tile fades out,
     ready to play again on the way back down.
     (Load after social-impact-animation.js)
  --------------------------------------- */
  const SHOWN_OPACITY = 0.95;
  const HIDDEN_OPACITY = 0.05;
  const VIEW_LINE = 0.9; // tile top must pass this share of the viewport

  if (!window.PriyaOdometer) return;

  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  const tiles = Array.from(
    document.querySelectorAll("[data-counter-deferred] .social-impact__stat")
  ).filter(function (tile) {
    return tile.querySelector(".counter");
  });
  if (!tiles.length) return;

  const played = new WeakSet();

  function check() {
    tiles.forEach(function (tile) {
      const rect = tile.getBoundingClientRect();
      const height = window.innerHeight;

      if (rect.top >= height) {
        if (played.has(tile)) {
          window.PriyaOdometer.reset(tile);
          played.delete(tile);
        }
        return;
      }
      if (rect.bottom <= 0) return;

      const opacity = parseFloat(getComputedStyle(tile).opacity);
      if (!played.has(tile) && rect.top < height * VIEW_LINE && opacity >= SHOWN_OPACITY) {
        window.PriyaOdometer.play(tile, { animate: !reduceMotion });
        played.add(tile);
      } else if (played.has(tile) && opacity <= HIDDEN_OPACITY) {
        window.PriyaOdometer.reset(tile);
        played.delete(tile);
      }
    });
    requestAnimationFrame(check);
  }

  requestAnimationFrame(check);
});
