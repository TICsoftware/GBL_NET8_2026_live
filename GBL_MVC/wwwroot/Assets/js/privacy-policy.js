/**
 * Privacy policy — left section list stays in view, and the marker
 * slides to the section currently on screen.
 */
(function () {
  function init() {
    var nav = document.querySelector(".pp-jump__nav");
    var marker = document.querySelector(".pp-jump__bar");
    var sections = Array.prototype.map.call(document.querySelectorAll(".pp-jump__btn"), function (link) {
      var id = (link.getAttribute("href") || "").replace(/^#/, "");
      var section = id ? document.getElementById(id) : null;
      return section ? { link: link, section: section } : null;
    }).filter(Boolean);

    if (!nav || !sections.length) return;

    var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    var activeSection = null;

    function headerOffset() {
      return parseFloat(getComputedStyle(document.documentElement).getPropertyValue("--header-h")) || 88;
    }

    function currentScroll() {
      if (window.lenis && typeof window.lenis.scroll === "number") return window.lenis.scroll;
      return window.scrollY || document.documentElement.scrollTop || 0;
    }

    function moveBar(link) {
      if (!marker || !link) return;
      marker.style.height = link.offsetHeight + "px";
      marker.style.transform = "translateY(" + link.offsetTop + "px)";
    }

    function scrollToSection(section) {
      var top = section.getBoundingClientRect().top + currentScroll() - headerOffset() - 24;
      top = Math.max(0, top);
      if (window.lenis && typeof window.lenis.scrollTo === "function") {
        window.lenis.scrollTo(top, { duration: reduceMotion ? 0 : 0.9 });
      } else {
        window.scrollTo({ top: top, behavior: reduceMotion ? "auto" : "smooth" });
      }
      if (history.replaceState) history.replaceState(null, "", "#" + section.id);
    }

    sections.forEach(function (item) {
      item.link.addEventListener("click", function (event) {
        event.preventDefault();
        scrollToSection(item.section);
      });
    });

    function setActive(section) {
      if (!section) section = sections[0].section;
      var changed = activeSection !== section;
      activeSection = section;
      sections.forEach(function (item) {
        var on = item.section === section;
        item.link.classList.toggle("is-active", on);
        if (on) {
          item.link.setAttribute("aria-current", "true");
          moveBar(item.link);
          if (changed && nav.scrollHeight > nav.clientHeight + 1) {
            var linkTop = item.link.offsetTop;
            var linkBottom = linkTop + item.link.offsetHeight;
            if (linkTop < nav.scrollTop || linkBottom > nav.scrollTop + nav.clientHeight) {
              nav.scrollTo({ top: Math.max(0, linkTop - 8), behavior: reduceMotion ? "auto" : "smooth" });
            }
          }
        } else {
          item.link.removeAttribute("aria-current");
        }
      });
    }

    var ticking = false;
    function update() {
      ticking = false;
      var line = headerOffset() + 48;
      var current = null;
      var doc = document.documentElement;
      var atBottom = window.innerHeight + window.scrollY >= doc.scrollHeight - 4;

      if (atBottom) {
        current = sections[sections.length - 1].section;
      } else {
        sections.forEach(function (item) {
          if (item.section.getBoundingClientRect().top <= line) current = item.section;
        });
      }

      setActive(current);
    }

    function onScroll() {
      if (ticking) return;
      ticking = true;
      window.requestAnimationFrame(update);
    }

    window.addEventListener("scroll", onScroll, { passive: true });
    window.addEventListener("resize", onScroll);
    if (window.lenis && typeof window.lenis.on === "function") {
      window.lenis.on("scroll", onScroll);
    }
    update();
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
