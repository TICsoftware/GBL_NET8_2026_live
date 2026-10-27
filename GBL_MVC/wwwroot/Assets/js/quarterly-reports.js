document.addEventListener("DOMContentLoaded", function () {
  var root = document.querySelector("[data-qr-reports]");
  if (!root) return;

  var tabs = Array.prototype.slice.call(root.querySelectorAll("[data-qr-year]"));
  var panels = Array.prototype.slice.call(root.querySelectorAll("[data-qr-panel]"));
  var list = root.querySelector(".qr-years__list");
  var viewport = root.querySelector(".qr-years__viewport");
  var prev = root.querySelector(".qr-years__arrow--prev");
  var next = root.querySelector(".qr-years__arrow--next");
  var start = 0;

  function currentIndex() {
    for (var i = 0; i < tabs.length; i++) {
      if (tabs[i].classList.contains("is-active")) return i;
    }
    return 0;
  }

  function visibleCount() {
    if (window.matchMedia("(max-width: 720px)").matches) return 3;
    return 5;
  }

  function layout() {
    if (!viewport || !list || !tabs.length) return;
    var visible = Math.min(visibleCount(), tabs.length);
    var itemWidth = viewport.clientWidth / visible;
    var maxStart = Math.max(0, tabs.length - visible);
    if (start > maxStart) start = maxStart;
    if (start < 0) start = 0;

    tabs.forEach(function (tab) {
      tab.style.flex = "0 0 " + itemWidth + "px";
      tab.style.maxWidth = itemWidth + "px";
    });

    list.style.transform = "translateX(" + (-start * itemWidth) + "px)";
  }

  function ensureVisible(index) {
    var visible = Math.min(visibleCount(), tabs.length);
    if (index < start) start = index;
    else if (index >= start + visible) start = index - visible + 1;
    layout();
  }

  function updateArrows() {
    var index = currentIndex();
    if (prev) prev.disabled = index <= 0;
    if (next) next.disabled = index >= tabs.length - 1;
  }

  function select(year) {
    var activeIndex = 0;
    tabs.forEach(function (tab, index) {
      var on = tab.getAttribute("data-qr-year") === year;
      tab.classList.toggle("is-active", on);
      tab.setAttribute("aria-selected", on ? "true" : "false");
      tab.tabIndex = on ? 0 : -1;
      if (on) activeIndex = index;
    });

    panels.forEach(function (panel) {
      panel.hidden = panel.getAttribute("data-qr-panel") !== year;
    });

    ensureVisible(activeIndex);
    updateArrows();
  }

  tabs.forEach(function (tab) {
    if (tab.getAttribute("aria-selected") !== "true") tab.tabIndex = -1;

    tab.addEventListener("click", function () {
      select(tab.getAttribute("data-qr-year"));
    });
  });

  if (prev) {
    prev.addEventListener("click", function () {
      var index = currentIndex();
      if (index > 0) select(tabs[index - 1].getAttribute("data-qr-year"));
    });
  }

  if (next) {
    next.addEventListener("click", function () {
      var index = currentIndex();
      if (index < tabs.length - 1) select(tabs[index + 1].getAttribute("data-qr-year"));
    });
  }

  updateArrows();
  layout();

  window.addEventListener("resize", function () {
    ensureVisible(currentIndex());
  });
});
