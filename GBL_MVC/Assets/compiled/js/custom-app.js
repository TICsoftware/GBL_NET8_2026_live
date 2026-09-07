document.addEventListener("DOMContentLoaded", () => {

// YEAR FOOTER - START
  document.getElementById("year-foot").innerHTML = (new Date().getFullYear());
// YEAR FOOTER - END

// SIDEBAR - START
  const burger = document.querySelector(".burger-btn");
  const sidebar = document.querySelector("#sidebar");

  // Create overlay if missing
  let overlay = document.querySelector("#sidebar-overlay");
  if (!overlay) {
    overlay = document.createElement("div");
    overlay.id = "sidebar-overlay";
    document.body.appendChild(overlay);
  }

  function openSidebar() {
    sidebar.classList.remove("closed");
    document.body.classList.remove("sidebar-collapsed");
    document.body.classList.add("sidebar-open");
    overlay.classList.add("show");
    burger.setAttribute("aria-expanded", "true");
  }

  function closeSidebar() {
    sidebar.classList.add("closed");
    document.body.classList.remove("sidebar-open");
    document.body.classList.add("sidebar-collapsed");
    overlay.classList.remove("show");
    burger.setAttribute("aria-expanded", "false");
  }

  function toggleSidebar() {
    if (sidebar.classList.contains("closed")) {
      openSidebar();
    } else {
      closeSidebar();
    }
  }

  burger.addEventListener("click", (e) => {
    e.preventDefault();
    toggleSidebar();
  });

  overlay.addEventListener("click", closeSidebar);

  document.addEventListener("keydown", (e) => {
    if (e.key === "Escape" && !sidebar.classList.contains("closed")) {
      closeSidebar();
    }
  });

  window.addEventListener("resize", () => {
    if (window.innerWidth >= 992) overlay.classList.remove("show");
  });

  // ✅ Sidebar open on load
  openSidebar();

  // ===============================
// SIDEBAR ACTIVE STATE (FINAL)
// ===============================
(function () {

  function normalize(path) {
    return path.replace(/\/$/, "").toLowerCase();
  }

  function applyActive() {
    const sidebar = document.querySelector(".sidebar-menu");
    if (!sidebar) return false;

    const currentPath = normalize(window.location.pathname);

    const links = sidebar.querySelectorAll(
      'a[href]:not([href="#"])'
    );

    // clear previous active states
    sidebar.querySelectorAll(".active").forEach(el =>
      el.classList.remove("active")
    );

    for (let link of links) {
      const linkPath = normalize(
        new URL(link.href, window.location.origin).pathname
      );

      if (
        currentPath === linkPath ||
        currentPath.startsWith(linkPath + "/")
      ) {
        activate(link);
        return true; // 🔥 stop after first match
      }
    }
    return false;
  }

  function activate(link) {
    const sidebarItem = link.closest(".sidebar-item");
    if (!sidebarItem) return;

    /* =========================
       TOP-LEVEL ACTIVE
    ========================= */
    sidebarItem.classList.add("active");

    const sidebarLink =
      sidebarItem.querySelector(":scope > .sidebar-link");
    if (sidebarLink) {
      sidebarLink.classList.add("active");
    }

    /* =========================
       SUBMENU ACTIVE (NEW)
    ========================= */
    const submenuItem = link.closest(".submenu-item");
    if (submenuItem) {
      submenuItem.classList.add("active");
    }

    link.classList.add("active");

    /* =========================
       OPEN SUBMENU
    ========================= */
    const submenu =
      sidebarItem.querySelector(":scope > .submenu");
    if (submenu) {
      submenu.classList.remove("submenu-closed");
      submenu.classList.add("submenu-open");
    }
  }

  // 1️⃣ Try immediately
  if (applyActive()) return;

  // 2️⃣ Observe for late sidebar render (template JS)
  const observer = new MutationObserver(() => {
    if (applyActive()) {
      observer.disconnect();
    }
  });

  observer.observe(document.body, {
    childList: true,
    subtree: true
  });

})();




  //STICKY - START
  const stickyCard = document.querySelector(".sticky-card");
  const section = document.querySelector(".content-main");

  if (!stickyCard || !section) return;

  const sectionTop = section.offsetTop;
  const sectionHeight = section.offsetHeight;

  window.addEventListener("scroll", () => {
    const scrollY = window.scrollY;
    const sectionBottom = sectionTop + sectionHeight;

    if (scrollY > sectionTop - 150 && scrollY < sectionBottom - 400) {
      if (!stickyCard.classList.contains("is-sticky")) {
        stickyCard.classList.add("entering");
        requestAnimationFrame(() => {
          stickyCard.classList.add("is-sticky");
          stickyCard.classList.add("entered");
        });
      }
    } else {
      stickyCard.classList.remove("is-sticky", "entering", "entered");
    }
  });




  
});