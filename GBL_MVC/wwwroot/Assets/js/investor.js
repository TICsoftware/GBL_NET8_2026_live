/**
 * Investor relations — share quote up/down, plus hover fill on linked cards.
 */
(function () {
  function initQuote() {
    var quote = document.querySelector("[data-inv-quote]");
    if (!quote) return;

    var change = quote.querySelector("[data-inv-change]");
    var value = change ? change.textContent.replace(/^\s+/, "") : "";
    quote.classList.toggle("is-down", value.charAt(0) === "-");
    quote.classList.toggle("is-up", value.charAt(0) === "+");
  }

  function initCardFill() {
    document.querySelectorAll(
      ".whatsNew__card:has(> a), .qr-files a, .si-list a, .cg-list a"
    ).forEach(function (card) {
      function placeFill(e) {
        var media = card.querySelector(".whatsNew__media");
        var rect = card.getBoundingClientRect();
        var mediaRect = media ? media.getBoundingClientRect() : null;
        var top = mediaRect ? Math.max(0, mediaRect.bottom - rect.top) : 0;
        var fillW = rect.width;
        var fillH = Math.max(0, rect.height - top);
        var x = e ? e.clientX - rect.left : fillW / 2;
        var y = e ? e.clientY - rect.top - top : fillH / 2;
        x = Math.min(Math.max(x, 0), fillW);
        y = Math.min(Math.max(y, 0), fillH);
        var radius = Math.ceil(Math.hypot(Math.max(x, fillW - x), Math.max(y, fillH - y))) + 16;
        card.style.setProperty("--fill-top", top + "px");
        card.style.setProperty("--hx", x + "px");
        card.style.setProperty("--hy", y + "px");
        card.style.setProperty("--hr", radius + "px");
      }

      placeFill(null);
      card.addEventListener("mouseenter", function (e) {
        placeFill(e);
        card.classList.add("is-filled");
      });
      card.addEventListener("mouseleave", function () {
        card.classList.remove("is-filled");
      });
    });
  }

  function init() {
    initQuote();
    initCardFill();
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
