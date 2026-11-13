(function () {
  var bar = document.getElementById("cookie-bar");
  if (!bar) return;

  var key = "gbl-cookie-consent";
  var stored = null;
  try {
    stored = localStorage.getItem(key);
  } catch (e) {
    stored = null;
  }

  if (stored) return;

  bar.hidden = false;

  function dismiss(value) {
    try {
      localStorage.setItem(key, value);
    } catch (e) {}
    bar.hidden = true;
  }

  var accept = bar.querySelector("[data-cookie-accept]");
  var close = bar.querySelector("[data-cookie-close]");
  if (accept) accept.addEventListener("click", function () { dismiss("accepted"); });
  if (close) close.addEventListener("click", function () { dismiss("closed"); });
})();
