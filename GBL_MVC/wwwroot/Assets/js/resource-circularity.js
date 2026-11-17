/**
 * Resource circularity — connector paths and energy-mix donuts.
 */
(function () {
  var SVG_NS = "http://www.w3.org/2000/svg";
  var LINK_LIME = "#cddd4a";
  var LINK_GREEN = "#5f8a3c";
  var STACKED_MQ = "(max-width: 1100px)";

  function init() {
    drawLinks();
    paintCharts();
    var frame;
    window.addEventListener("resize", function () {
      cancelAnimationFrame(frame);
      frame = requestAnimationFrame(drawLinks);
    });
  }

  function drawLinks() {
    var stage = document.querySelector(".rc-pillars__stage");
    var svg = stage && stage.querySelector(".rc-pillars__links");
    if (!stage || !svg) return;

    while (svg.firstChild) svg.removeChild(svg.firstChild);

    if (window.matchMedia(STACKED_MQ).matches) return;

    var energy = box(stage, stage.querySelector(".rc-pillar--energy .rc-capsule__frame"));
    var water = box(stage, stage.querySelector(".rc-water"));
    var hub = box(stage, stage.querySelector(".rc-hub"));
    var waste = box(stage, stage.querySelector(".rc-pillar--waste .rc-capsule__frame"));
    if (!energy || !water || !hub || !waste) return;

    var w = stage.clientWidth;
    var h = stage.clientHeight;
    svg.setAttribute("viewBox", "0 0 " + w + " " + h);

    var leftJoin = { x: energy.right + 34, y: energy.bottom - 22 };
    var rightJoin = { x: waste.left - 34, y: waste.bottom - 22 };

    addPath(svg, tail(energy, 1, leftJoin), LINK_GREEN, 6);
    addPath(svg, tail(waste, -1, rightJoin), LINK_GREEN, 6);
    addPath(svg, snake(water.left + 3, water.top + water.h * 0.42, leftJoin.x, leftJoin.y, -1), LINK_LIME, 7);
    addPath(svg, snake(water.right - 3, water.top + water.h * 0.42, rightJoin.x, rightJoin.y, 1), LINK_LIME, 7);
    addDot(svg, leftJoin.x, leftJoin.y, LINK_LIME);
    addDot(svg, rightJoin.x, rightJoin.y, LINK_GREEN);
  }

  function addPath(svg, d, color, width) {
    var path = document.createElementNS(SVG_NS, "path");
    path.setAttribute("d", d);
    path.setAttribute("fill", "none");
    path.setAttribute("stroke", color);
    path.setAttribute("stroke-width", String(width));
    path.setAttribute("stroke-linecap", "round");
    path.setAttribute("stroke-linejoin", "round");
    svg.appendChild(path);
  }

  function addDot(svg, x, y, color) {
    var dot = document.createElementNS(SVG_NS, "circle");
    dot.setAttribute("cx", x);
    dot.setAttribute("cy", y);
    dot.setAttribute("r", "6.5");
    dot.setAttribute("fill", color);
    svg.appendChild(dot);
  }

  function snake(x1, y1, x2, y2, dir) {
    var span = Math.abs(x2 - x1);
    var midX = x1 + dir * Math.max(54, span * 0.46);
    var knee = y1 + Math.max(64, (y2 - y1) * 0.5);
    var low = Math.max(knee + 36, y2 + 18);
    return [
      "M", x1, y1,
      "C", x1 + dir * 30, y1, midX, y1 + 6, midX, knee,
      "C", midX, low, x2 - dir * 12, low, x2, y2
    ].join(" ");
  }

  function tail(frame, dir, join) {
    var x = dir > 0 ? frame.right - 1 : frame.left + 1;
    var y = frame.bottom - 10;
    return [
      "M", x, y,
      "C", x + dir * 26, y + 8, join.x, join.y + 22, join.x, join.y
    ].join(" ");
  }

  function box(stage, el) {
    if (!el) return null;
    var stageRect = stage.getBoundingClientRect();
    var rect = el.getBoundingClientRect();
    return {
      left: rect.left - stageRect.left,
      right: rect.right - stageRect.left,
      top: rect.top - stageRect.top,
      bottom: rect.bottom - stageRect.top,
      cx: rect.left - stageRect.left + rect.width / 2,
      cy: rect.top - stageRect.top + rect.height / 2,
      w: rect.width,
      h: rect.height
    };
  }

  function paintCharts() {
    var reduce = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    document.querySelectorAll("[data-rc-chart]").forEach(function (figure) {
      var spec = readChart(figure);
      if (!spec.slices.length) return;
      paintLegend(figure, spec);
      bindHover(figure);
      if (reduce) {
        renderDonut(figure, spec, 1);
        return;
      }
      var played = false;
      if (!("IntersectionObserver" in window)) {
        renderDonut(figure, spec, 1);
        return;
      }
      var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
          if (!entry.isIntersecting || played) return;
          played = true;
          observer.disconnect();
          animateDonut(figure, spec);
        });
      }, { threshold: 0.35 });
      observer.observe(figure);
    });
  }

  function readChart(figure) {
    var items = Array.prototype.slice.call(
      figure.querySelectorAll(".rc-chart__legend [data-key][data-value]")
    );
    items.sort(function (a, b) {
      return (Number(a.getAttribute("data-draw")) || 0) - (Number(b.getAttribute("data-draw")) || 0);
    });
    return {
      start: Number(figure.getAttribute("data-start")) || 0,
      slices: items.map(function (item) {
        return {
          key: item.getAttribute("data-key"),
          value: Number(item.getAttribute("data-value")),
          color: item.getAttribute("data-color") || "#ccc"
        };
      })
    };
  }

  function paintLegend(figure, spec) {
    var shares = {};
    var colors = {};
    spec.slices.forEach(function (slice) {
      shares[slice.key] = formatShare(slice.value);
      colors[slice.key] = slice.color;
    });
    figure.querySelectorAll(".rc-chart__legend [data-key]").forEach(function (item) {
      var key = item.getAttribute("data-key");
      if (colors[key]) item.style.setProperty("--swatch", colors[key]);
      var name = item.querySelector("span");
      if (name && shares[key]) {
        item.setAttribute("aria-label", name.textContent + ", " + shares[key]);
      }
    });
  }

  function formatShare(value) {
    var rounded = Math.round(value * 10) / 10;
    var text = String(rounded);
    if (text.indexOf(".") !== -1) text = text.replace(/\.0$/, "");
    return text + "%";
  }

  function bindHover(figure) {
    if (figure.getAttribute("data-hover") === "1") return;
    figure.setAttribute("data-hover", "1");

    function keyFrom(target) {
      var node = target && target.closest ? target.closest("[data-key]") : null;
      return node ? node.getAttribute("data-key") : "";
    }

    function show(event) {
      var key = keyFrom(event.target);
      if (!key || figure.getAttribute("data-active") === key) return;
      figure.setAttribute("data-active", key);
      placeHoverLabel(figure);
    }

    function hide(event) {
      var key = keyFrom(event.target);
      if (!key) return;
      var next = event.relatedTarget && event.relatedTarget.closest
        ? event.relatedTarget.closest("[data-key]")
        : null;
      if (next && figure.contains(next)) return;
      if (figure.getAttribute("data-active") !== key) return;
      figure.removeAttribute("data-active");
      placeHoverLabel(figure);
    }

    [".rc-chart__slices", ".rc-chart__legend"].forEach(function (selector) {
      var root = figure.querySelector(selector);
      if (!root) return;
      root.addEventListener("mouseover", show);
      root.addEventListener("mouseout", hide);
    });
  }

  function placeHoverLabel(figure) {
    var label = figure.querySelector(".rc-chart__value");
    if (!label) return;
    var key = figure.getAttribute("data-active");
    var path = key ? figure.querySelector('.rc-chart__slices path[data-key="' + key + '"]') : null;
    if (!path) {
      label.textContent = "";
      figure.classList.remove("is-hover");
      return;
    }
    var point = polar(100, 100, 78, parseFloat(path.getAttribute("data-mid")));
    label.setAttribute("x", point.x.toFixed(2));
    label.setAttribute("y", point.y.toFixed(2));
    label.setAttribute("dominant-baseline", "central");
    label.textContent = path.getAttribute("data-label") || "";
    figure.classList.add("is-hover");
  }

  function animateDonut(figure, spec) {
    var start = performance.now();
    var duration = 1100;
    function frame(now) {
      var t = Math.min(1, (now - start) / duration);
      var eased = 1 - Math.pow(1 - t, 3);
      renderDonut(figure, spec, eased);
      if (t < 1) requestAnimationFrame(frame);
    }
    requestAnimationFrame(frame);
  }

  function renderDonut(figure, spec, progress) {
    var svg = figure.querySelector(".rc-chart__svg");
    var group = figure.querySelector(".rc-chart__slices");
    var label = figure.querySelector(".rc-chart__value");
    if (!svg || !group) return;

    while (group.firstChild) group.removeChild(group.firstChild);

    var budget = Math.max(0, Math.min(1, progress)) * 360;
    var angle = spec.start;

    spec.slices.forEach(function (slice) {
      var full = (slice.value / 100) * 360;
      var sweep = Math.min(full, budget);
      budget -= sweep;
      if (sweep > 0.15) {
        var path = document.createElementNS(SVG_NS, "path");
        path.setAttribute("d", ringSlice(100, 100, 52, 96, angle, angle + sweep));
        path.setAttribute("fill", slice.color || "#ccc");
        path.setAttribute("data-key", slice.key);
        path.setAttribute("data-mid", String(angle + sweep / 2));
        path.setAttribute("data-label", formatShare(slice.value));
        group.appendChild(path);
      }
      angle += sweep > 0 ? sweep : 0;
      if (budget <= 0) return;
    });

    if (label) placeHoverLabel(figure);
  }

  function polar(cx, cy, r, deg) {
    var rad = ((deg - 90) * Math.PI) / 180;
    return { x: cx + r * Math.cos(rad), y: cy + r * Math.sin(rad) };
  }

  function ringSlice(cx, cy, rInner, rOuter, a0, a1) {
    var delta = a1 - a0;
    while (delta < 0) delta += 360;
    while (delta >= 360) delta -= 360;
    var large = delta > 180 ? 1 : 0;
    var p1 = polar(cx, cy, rOuter, a0);
    var p2 = polar(cx, cy, rOuter, a1);
    var p3 = polar(cx, cy, rInner, a1);
    var p4 = polar(cx, cy, rInner, a0);
    return [
      "M", p1.x, p1.y,
      "A", rOuter, rOuter, 0, large, 1, p2.x, p2.y,
      "L", p3.x, p3.y,
      "A", rInner, rInner, 0, large, 0, p4.x, p4.y,
      "Z"
    ].join(" ");
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();




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
