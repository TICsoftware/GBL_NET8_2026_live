const headerWrap = document.getElementById('headerWrap');
  const navBar = document.getElementById('navBar');
  const navViewport = document.getElementById('navViewport');
  const dropdownAnchor = document.getElementById('dropdownAnchor');
  const desktopNav = document.getElementById('desktopNav');
  let menuItems = [...desktopNav.querySelectorAll('.nav-item[data-menu]')];
  const panels = [...document.querySelectorAll('.nav-panel')];

  let activeMenu = null;
  let prevIndex = null;
  let closeTimer = null;

  function bindDesktopMenuEvents(){
    menuItems.forEach(item => {
      item.addEventListener('mouseenter', () => openMenu(item));
      item.addEventListener('focusin', () => openMenu(item));
      const btn = item.querySelector('button');
      if (!btn) return;
      btn.addEventListener('click', e => {
        e.preventDefault();
        activeMenu === item.dataset.menu ? closeMenu() : openMenu(item);
      });
    });
  }

  function wireMobileAccordions(){
    document.querySelectorAll('[data-accordion]').forEach(row => {
      const trigger = row.querySelector('.accordion-trigger');
      const panel = row.querySelector('.accordion-panel');
      const inner = row.querySelector('.accordion-panel-inner');
      if (!trigger || !panel || !inner) return;

      trigger.addEventListener('click', () => {
        const isOpen = row.classList.contains('open');

        document.querySelectorAll('[data-accordion]').forEach(r => {
          r.classList.remove('open');
          const p = r.querySelector('.accordion-panel');
          const t = r.querySelector('.accordion-trigger');
          if (p) p.style.maxHeight = '0';
          if (t) t.setAttribute('aria-expanded', 'false');
        });

        if (!isOpen) {
          row.classList.add('open');
          panel.style.maxHeight = inner.scrollHeight + 'px';
          trigger.setAttribute('aria-expanded', 'true');
        }
      });
    });
  }

  function measurePanel(panel){
    const clone = panel.cloneNode(true);
    clone.classList.add('is-active');
    clone.style.cssText = 'position:fixed;left:-9999px;top:0;display:block;visibility:hidden;pointer-events:none;';
    document.body.appendChild(clone);
    const w = clone.offsetWidth;
    const h = clone.offsetHeight;
    document.body.removeChild(clone);
    return { w, h };
  }

  function applySize(panel){
    const { w, h } = measurePanel(panel);
    navViewport.style.width = w + 'px';
    navViewport.style.minHeight = h + 'px';
  }

  function showPanel(panel, direction){
    panels.forEach(p => p.classList.remove('is-active','from-start','from-end'));
    panel.classList.add('is-active');
    if (direction) panel.classList.add(direction);
    applySize(panel);
  }

  function positionViewportToItem(item){
    const btn = item.querySelector('button');
    if (!btn) return;
    const btnRect = btn.getBoundingClientRect();
    const anchorRect = dropdownAnchor.getBoundingClientRect();
    const centerX = btnRect.left + (btnRect.width / 2) - anchorRect.left;
    navViewport.style.left = centerX + 'px';
  }

  function openMenu(item){
    clearTimeout(closeTimer);
    const menu = item.dataset.menu;
    const index = Number(item.dataset.index);
    const panel = document.getElementById('panel-' + menu);
    if (!panel) return;

    const direction = activeMenu && prevIndex !== null
      ? (index > prevIndex ? 'from-end' : 'from-start') : '';

    menuItems.forEach(i => {
      i.classList.toggle('open', i === item);
      const btn = i.querySelector('button');
      if (btn) btn.setAttribute('aria-expanded', i === item ? 'true' : 'false');
    });

    positionViewportToItem(item);
    showPanel(panel, direction);
    navViewport.classList.add('open');
    navViewport.setAttribute('aria-hidden', 'false');
    activeMenu = menu;
    prevIndex = index;
  }

  function closeMenu(){
    clearTimeout(closeTimer);
    menuItems.forEach(i => {
      i.classList.remove('open');
      const btn = i.querySelector('button');
      if (btn) btn.setAttribute('aria-expanded', 'false');
    });
    navViewport.classList.remove('open');
    navViewport.setAttribute('aria-hidden', 'true');
    activeMenu = null;
    prevIndex = null;
    closeTimer = setTimeout(() => {
      panels.forEach(p => p.classList.remove('is-active','from-start','from-end'));
      navViewport.style.width = '';
      navViewport.style.minHeight = '';
    }, 160);
  }

  function scheduleClose(){
    clearTimeout(closeTimer);
    closeTimer = setTimeout(closeMenu, 220);
  }

  headerWrap.addEventListener('mouseleave', scheduleClose);
  headerWrap.addEventListener('focusout', e => {
    if (!headerWrap.contains(e.relatedTarget)) scheduleClose();
  });
  dropdownAnchor.addEventListener('mouseenter', () => clearTimeout(closeTimer));

  // Header content is now static in _Header.cshtml; JS handles behavior only.
  menuItems = [...desktopNav.querySelectorAll('.nav-item[data-menu]')];
  bindDesktopMenuEvents();
  wireMobileAccordions();

function updateScrolledState(){
    const isScrolled = window.scrollY > 8;
    navBar.classList.toggle('elevated', isScrolled);
    headerWrap.classList.toggle('scrolled', isScrolled);
  }

  window.addEventListener('scroll', updateScrolledState);
  updateScrolledState(); // set correct state immediately on page load

  const hamburgerBtn = document.getElementById('hamburgerBtn');
  const drawer = document.getElementById('drawer');
  const overlay = document.getElementById('overlay');
  const drawerClose = document.getElementById('drawerClose');

  function openDrawer(){
    drawer.classList.add('active');
    overlay.classList.add('active');
    overlay.setAttribute('aria-hidden', 'false');
    hamburgerBtn.setAttribute('aria-expanded', 'true');
    document.body.style.overflow = 'hidden';
  }
  function closeDrawer(){
    drawer.classList.remove('active');
    overlay.classList.remove('active');
    overlay.setAttribute('aria-hidden', 'true');
    hamburgerBtn.setAttribute('aria-expanded', 'false');
    document.body.style.overflow = '';
  }

  hamburgerBtn.addEventListener('click', openDrawer);
  drawerClose.addEventListener('click', closeDrawer);
  overlay.addEventListener('click', closeDrawer);

  document.addEventListener('keydown', e => {
    if (e.key !== 'Escape') return;
    if (drawer.classList.contains('active')) closeDrawer();
    else closeMenu();
  });

  window.addEventListener('resize', () => {
    const p = panels.find(x => x.classList.contains('is-active'));
    if (p && navViewport.classList.contains('open')) applySize(p);
    if (activeMenu) {
      const activeItem = menuItems.find(i => i.dataset.menu === activeMenu);
      if (activeItem) positionViewportToItem(activeItem);
    }
    document.querySelectorAll('[data-accordion].open').forEach(row => {
      const panel = row.querySelector('.accordion-panel');
      const inner = row.querySelector('.accordion-panel-inner');
      if (!panel || !inner) return;
      panel.style.maxHeight = inner.scrollHeight + 'px';
    });
  });


// --------------------------------------------
// FOOTER YEAR
// --------------------------------------------
document.getElementById("year-foot").innerHTML = (new Date().getFullYear());

  // --------------------------------------------
  // GSAP + ScrollTrigger + Lenis Setup
  // --------------------------------------------
  gsap.registerPlugin(ScrollTrigger);
  
  const isMobile = window.matchMedia("(max-width: 992px)").matches;
  
  /** Same scroll root as Lenis default (wrapper: window Ã¢â€ â€™ classes + scroll on documentElement). */
  const scrollRootEl = document.documentElement;
  
  let lenis;
  
  if (!isMobile) {
  
    lenis = new Lenis({
      smoothWheel: true,
      smoothTouch: false,
  
      // PERFECT NO-LAG SETTINGS
      lerp: 0.05,              // fast response, no delay
      wheelMultiplier: 1.02,   // mouse feels natural
      normalizeWheel: true,
      syncTouch: false,
        prevent: (node) => {
        return node.closest('.testimonial-content');
      }
    });
  
    function raf(time) {
      lenis.raf(time);
      requestAnimationFrame(raf);
    }
    requestAnimationFrame(raf);
  
    window.lenis = lenis;
  
    // ---- GSAP SYNC ----
    ScrollTrigger.scrollerProxy(scrollRootEl, {
      scrollTop(value) {
        return arguments.length
          ? lenis.scrollTo(value, { immediate: true })
          : lenis.scroll;
      },
      getBoundingClientRect() {
        return {
          top: 0,
          left: 0,
          width: scrollRootEl.clientWidth,
          height: scrollRootEl.clientHeight
        };
      }
    });
  
    // ScrollTriggers must use the same element Lenis proxies Ã¢â‚¬â€ otherwise scrub/toggle use native scroll and wonÃ¢â‚¬â„¢t match smooth scroll.
    ScrollTrigger.defaults({ scroller: scrollRootEl });
  
    lenis.on("scroll", ScrollTrigger.update);
    ScrollTrigger.addEventListener("refresh", () => lenis.resize());
    ScrollTrigger.refresh();
  
  } else {
    document.body.classList.add("native-scroll");
    // Mobile: keep true native window/document scroll (no scrollerProxy).
    // Proxying documentElement can interfere with touch scrolling on some mobile browsers.
    ScrollTrigger.defaults({ scroller: window });
    ScrollTrigger.refresh();
  }
  

/* ======================
   FOOTER SECTION
====================== */
document.addEventListener("DOMContentLoaded", () => {

const footer =
document.querySelector("footer");

if(!footer) return;

function initFooterAccordions(){
  const sections = [...footer.querySelectorAll('.footer-quick-link-click')];
  if (!sections.length) return;

  const desktopMq = window.matchMedia('(min-width: 768px)');

  function closeAll(){
    sections.forEach(section => {
      section.classList.remove('open');
      const list = section.querySelector('.footer-nav-list');
      const heading = section.querySelector('h4');
      if (list) list.style.maxHeight = '0';
      if (heading) heading.setAttribute('aria-expanded', 'false');
    });
  }

  function resetForDesktop(){
    if (!desktopMq.matches) return;
    sections.forEach(section => {
      section.classList.remove('open');
      const list = section.querySelector('.footer-nav-list');
      if (list) list.style.maxHeight = '';
    });
  }

  sections.forEach(section => {
    const heading = section.querySelector('h4');
    const list = section.querySelector('.footer-nav-list');
    if (!heading || !list) return;

    heading.setAttribute('role', 'button');
    heading.setAttribute('tabindex', '0');
    heading.setAttribute('aria-expanded', 'false');

    const toggle = () => {
      if (desktopMq.matches) return;

      const isOpen = section.classList.contains('open');
      closeAll();

      if (!isOpen) {
        section.classList.add('open');
        list.style.maxHeight = list.scrollHeight + 'px';
        heading.setAttribute('aria-expanded', 'true');
      }
    };

    heading.addEventListener('click', toggle);
    heading.addEventListener('keydown', e => {
      if (e.key !== 'Enter' && e.key !== ' ') return;
      e.preventDefault();
      toggle();
    });
  });

  desktopMq.addEventListener('change', resetForDesktop);
  resetForDesktop();
}

initFooterAccordions();

gsap.registerPlugin(ScrollTrigger);


/* reset */

gsap.set(
[
".footer-link",
".social-link",
".legal-link"
],
{
clearProps:"all"
}
);


const tl =
gsap.timeline({

scrollTrigger:{

trigger:footer,

start:"top 85%",

once:true

}

});



/* columns */

tl.from(

footer.querySelectorAll(
".md\\:col-span-4, .footer-quick-links, .md\\:col-span-3, .md\\:col-span-2"
),

{

opacity:0,

y:100,

duration:1.1,

stagger:.18,

ease:"expo.out"

}

);



/* links */

tl.from(

".footer-link",

{

opacity:0,

y:18,

duration:.45,

stagger:.04,

ease:"power2.out"

},

"-=.6"

);



/* social */

tl.from(

".social-link",

{

opacity:0,

scale:.85,

duration:.4,

stagger:.05,

ease:"power2.out"

},

"-=.4"

);



/* bottom */

tl.from(

".footer-bottom",

{

opacity:0,

y:30,

duration:.7,

ease:"power2.out"

},

"-=.3"

);

});