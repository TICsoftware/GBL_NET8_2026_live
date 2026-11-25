/**
 * Media kit — sticky section nav, with the tab for the section
 * currently under the bar marked active while scrolling.
 */
(function () {
  function init() {
    var bar = document.querySelector(".mk-jump");
    var sentinel = document.querySelector(".mk-jump-sentinel");
    var sections = Array.prototype.map.call(document.querySelectorAll(".mk-jump__btn"), function (link) {
      var id = (link.getAttribute("href") || "").replace(/^#/, "");
      var section = id ? document.getElementById(id) : null;
      if (!section) return null;
      var head = section.querySelector("h2") || section;
      return { link: link, section: section, head: head };
    }).filter(Boolean);

    if (!bar || !sections.length) return;

    if (sentinel && "IntersectionObserver" in window) {
      var headerH = parseFloat(getComputedStyle(document.documentElement).getPropertyValue("--header-h")) || 88;
      var observer = new IntersectionObserver(function (entries) {
        bar.classList.toggle("is-stuck", !entries[0].isIntersecting);
      }, { rootMargin: "-" + headerH + "px 0px 0px 0px", threshold: 0 });
      observer.observe(sentinel);
    }

    var nav = document.querySelector(".mk-jump__nav");
    var prev = document.querySelector(".mk-jump__arrow--prev");
    var next = document.querySelector(".mk-jump__arrow--next");
    var mobileQuery = window.matchMedia("(max-width: 1199px)");
    var reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    var activeSection = null;

    function centerLink(link) {
      if (!nav) return;
      var isFirst = !!(link && sections[0] && link === sections[0].link);
      if (!mobileQuery.matches || !link || isFirst) {
        nav.style.paddingLeft = "";
        nav.style.paddingRight = "";
        nav.scrollTo({
          left: 0,
          behavior: mobileQuery.matches && link && !reduceMotion ? "smooth" : "auto"
        });
        return;
      }
      var half = nav.clientWidth / 2;
      nav.style.paddingLeft = half + "px";
      nav.style.paddingRight = half + "px";
      var left = link.offsetLeft + link.offsetWidth / 2 - half;
      nav.scrollTo({ left: Math.max(0, left), behavior: reduceMotion ? "auto" : "smooth" });
    }

    function syncArrows(section) {
      if (!prev || !next) return;
      var index = -1;
      sections.forEach(function (item, i) {
        if (item.section === section) index = i;
      });
      prev.disabled = index <= 0;
      next.disabled = index < 0 || index === sections.length - 1;
    }

    function currentScroll() {
      if (window.lenis && typeof window.lenis.scroll === "number") return window.lenis.scroll;
      return window.scrollY || document.documentElement.scrollTop || 0;
    }

    function scrollToSection(section) {
      var headerH = parseFloat(getComputedStyle(document.documentElement).getPropertyValue("--header-h")) || 88;
      var top = section.getBoundingClientRect().top + currentScroll() - headerH - bar.offsetHeight - 16;
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
        centerLink(item.link);
        scrollToSection(item.section);
      });
    });

    function goTo(index) {
      var item = sections[index];
      if (!item) return;
      centerLink(item.link);
      scrollToSection(item.section);
    }

    if (prev) prev.addEventListener("click", function () {
      var index = -1;
      sections.forEach(function (item, i) {
        if (item.section === activeSection) index = i;
      });
      if (index > 0) goTo(index - 1);
      else if (index < 0) goTo(0);
    });

    if (next) next.addEventListener("click", function () {
      var index = -1;
      sections.forEach(function (item, i) {
        if (item.section === activeSection) index = i;
      });
      if (index < 0) goTo(0);
      else if (index < sections.length - 1) goTo(index + 1);
    });

    function setActive(section) {
      var changed = activeSection !== section;
      activeSection = section;
      sections.forEach(function (item) {
        var on = item.section === section;
        item.link.classList.toggle("is-active", on);
        if (on) item.link.setAttribute("aria-current", "true");
        else item.link.removeAttribute("aria-current");
        if (on && changed) centerLink(item.link);
      });
      if (!section && changed) centerLink(null);
      syncArrows(section);
      moveThumb();
    }

    // Green pill that slides under the active tab (same as .cg-jump__thumb)
    function moveThumb() {
      if (!nav) return;
      var thumb = nav.querySelector(".mk-jump__thumb");
      if (!thumb) return;
      var link = nav.querySelector(".mk-jump__btn.is-active");
      if (!link) {
        thumb.style.opacity = "0";
        nav.classList.remove("is-ready");
        return;
      }
      nav.classList.add("is-ready");
      thumb.style.opacity = "1";
      thumb.style.width = link.offsetWidth + "px";
      thumb.style.transform = "translateX(" + link.offsetLeft + "px)";
    }

    /* Scroll-spy (same rules as corporate governance): the first tab is active at the top;
       a tab activates when its section heading reaches ~45% down the viewport. */
    var ticking = false;
    function update() {
      ticking = false;
      if (nav && !mobileQuery.matches) {
        nav.style.paddingLeft = "";
        nav.style.paddingRight = "";
      }
      var line = window.innerHeight * 0.45;
      var current = null;
      var doc = document.documentElement;
      var atBottom = window.innerHeight + currentScroll() >= doc.scrollHeight - 4;

      if (atBottom) {
        current = sections[sections.length - 1].section;
      } else {
        sections.forEach(function (item) {
          if (item.head.getBoundingClientRect().top <= line) current = item.section;
        });
      }

      setActive(current || sections[0].section);
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
