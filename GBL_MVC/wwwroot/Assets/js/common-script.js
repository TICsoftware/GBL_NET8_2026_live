document.addEventListener("DOMContentLoaded", (event) => {

  // --------------------------------------------
  // GSAP + ScrollTrigger + Lenis Setup
  // --------------------------------------------
  gsap.registerPlugin(ScrollTrigger);
  
  const isMobile = window.matchMedia("(max-width: 992px)").matches;
  
  /** Same scroll root as Lenis default (wrapper: window ÃƒÂ¢Ã¢â‚¬ Ã¢â‚¬â„¢ classes + scroll on documentElement). */
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
        return node.closest('.testimonial-content')
          || node.closest('#products-section')
          || node.closest('.cselect-menu')
          || node.closest('.cselect');
      }
    });
  
    function raf(time) {
      lenis.raf(time);
      requestAnimationFrame(raf);
    }
    requestAnimationFrame(raf);
  
    window.lenis = lenis;

    // Nested scrollables (cselect dropdown): Lenis steals wheel before bubble handlers.
    // Capture on window, block Lenis, and scroll the menu ourselves.
    window.addEventListener(
      "wheel",
      (e) => {
        const menu = e.target && e.target.closest && e.target.closest(".cselect-menu");
        if (!menu) return;
        e.preventDefault();
        e.stopPropagation();
        if (typeof e.stopImmediatePropagation === "function") e.stopImmediatePropagation();
        menu.scrollTop += e.deltaY;
      },
      { capture: true, passive: false }
    );

  
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
  
    // ScrollTriggers must use the same element Lenis proxies ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â otherwise scrub/toggle use native scroll and wonÃƒÂ¢Ã¢â€šÂ¬Ã¢â€žÂ¢t match smooth scroll.
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


  // --------------------------------------------
  // BACK TO TOP + SCROLL PROGRESS RING
  // --------------------------------------------
  (function initBackToTop() {
    const btn = document.querySelector('.back-to-top');
    const circle = document.querySelector('.progress-ring-circle');
    if (!btn || !circle) return;

    const radius = Number(circle.getAttribute('r')) || 45;
    const circumference = 2 * Math.PI * radius;
    const SHOW_AFTER = 420;

    circle.style.strokeDasharray = String(circumference);
    circle.style.strokeDashoffset = String(circumference);

    const getScrollTop = () => {
      if (window.lenis && typeof window.lenis.scroll === 'number') {
        return window.lenis.scroll;
      }
      return window.scrollY || document.documentElement.scrollTop || 0;
    };

    const getScrollPercent = () => {
      const scrollable = Math.max(
        document.documentElement.scrollHeight - window.innerHeight,
        1
      );
      return Math.min(Math.max(getScrollTop() / scrollable, 0), 1);
    };

    const updateScrollUI = () => {
      const percent = getScrollPercent();
      circle.style.strokeDashoffset = String(circumference * (1 - percent));

      const show = getScrollTop() > SHOW_AFTER;
      // If the button currently holds focus and we're about to hide it,
      // move focus away first â€” setting aria-hidden on a focused element
      // is invalid (the browser blocks it and logs a console warning).
      if (!show && document.activeElement === btn) {
        btn.blur();
      }
      btn.classList.toggle('active', show);
      btn.setAttribute('aria-hidden', show ? 'false' : 'true');
      btn.tabIndex = show ? 0 : -1;
    };

    const scrollToTop = (e) => {
      e.preventDefault();
      if (window.lenis && typeof window.lenis.scrollTo === 'function') {
        window.lenis.scrollTo(0, { duration: 1.1 });
      } else {
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    };

    btn.addEventListener('click', scrollToTop);

    if (window.lenis && typeof window.lenis.on === 'function') {
      window.lenis.on('scroll', updateScrollUI);
    }
    window.addEventListener('scroll', updateScrollUI, { passive: true });
    window.addEventListener('resize', updateScrollUI);
    updateScrollUI();
  })();


// --------------------------------------------
// HEADER ANIMATION
// --------------------------------------------

  
// --------------------------------------------
// FOOTER YEAR
// --------------------------------------------
const yearFoot = document.getElementById("year-foot");
if (yearFoot) yearFoot.innerHTML = String(new Date().getFullYear());

// --------------------------------------------
// FOOTER VECTOR — animate every time footer enters viewport
// --------------------------------------------
(function initFooterVector() {
  const vector = document.querySelector('.footer-vector');
  const footer = document.querySelector('footer');
  if (!vector || !footer) return;

  // Always keep a visible fallback if motion libs are missing
  if (typeof gsap === 'undefined') {
    vector.style.opacity = '0.9';
    vector.style.transform = 'none';
    return;
  }

  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  if (reduceMotion) {
    gsap.set(vector, { clearProps: 'transform', opacity: 0.9, y: 0 });
    return;
  }

  const anim = gsap.fromTo(
    vector,
    { y: 120, opacity: 0, force3D: true },
    {
      y: 0,
      opacity: 0.9,
      duration: 1.15,
      ease: 'power3.out',
      force3D: true,
      paused: true,
    }
  );

  let inView = false;

  const playIn = () => {
    if (inView) return;
    inView = true;
    anim.restart();
  };

  const resetOut = () => {
    if (!inView) return;
    inView = false;
    anim.pause(0);
  };

  // IntersectionObserver is reliable with Lenis + native scroll
  if ('IntersectionObserver' in window) {
    const io = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) playIn();
          else resetOut();
        });
      },
      { threshold: 0.12, rootMargin: '0px 0px -8% 0px' }
    );
    io.observe(footer);
    return;
  }

  // Fallback without IO
  if (typeof ScrollTrigger !== 'undefined') {
    ScrollTrigger.create({
      trigger: footer,
      start: 'top 90%',
      end: 'bottom top',
      onEnter: playIn,
      onEnterBack: playIn,
      onLeave: resetOut,
      onLeaveBack: resetOut,
      onRefresh: (self) => {
        if (self.isActive) playIn();
        else resetOut();
      },
    });
    requestAnimationFrame(() => ScrollTrigger.refresh());
  } else {
    gsap.set(vector, { y: 0, opacity: 0.9 });
  }
})();


// --------------------------------------------
// HEADER
// --------------------------------------------
 const navTemplate = document.getElementById('navSource');
  const sections = navTemplate.content.querySelectorAll('.nav-data');

  function getSection(key){
    return navTemplate.content.querySelector(`.nav-data[data-key="${key}"]`);
  }

  const chevSVG = '<svg viewBox="0 0 12 8" fill="none"><path d="M1 1.5L6 6.5L11 1.5" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>';

  /* ---------- build desktop nav triggers ---------- */
  const primaryNav = document.getElementById('primaryNav');
  sections.forEach(section=>{
    const el = document.createElement('div');
    el.className = 'nav-item';
    el.dataset.key = section.dataset.key;
    const href = section.dataset.href;
    if (href) {
      el.classList.add('is-link');
      el.innerHTML = `<a class="nav-trigger" href="${href}">${section.dataset.label}</a>`;
    } else {
      el.innerHTML = `<button class="nav-trigger" aria-expanded="false">${section.dataset.label}${chevSVG}</button>`;
    }
    primaryNav.appendChild(el);
  });

  /* mega panel content is just the matching section's markup, cloned in */
  function fillMegaPanel(key){
    const section = getSection(key);
    if(!section) return;
    megaInner.innerHTML = '';
    section.querySelectorAll('.mega-links, .mega-promo').forEach(node=>{
      megaInner.appendChild(node.cloneNode(true));
    });
  }

  /* ---------- sync header height to CSS custom property ---------- */
  /* Lock to EXPANDED height only. Compact is visual â€” remasuring on
     scroll shrinks margin-top / offsets and flickers the banner. */
  const header = document.getElementById('siteHeader');
  function syncHeaderHeight() {
    if (!header) return;
    const hadCompact = header.classList.contains('is-compact');
    if (hadCompact) header.classList.remove('is-compact');
    const h = Math.round(header.getBoundingClientRect().height);
    if (hadCompact) header.classList.add('is-compact');
    document.documentElement.style.setProperty('--header-h', `${h}px`);
  }
  syncHeaderHeight();
  window.addEventListener('resize', syncHeaderHeight, { passive: true });

  /* ---------- header load appear ---------- */
  (function initHeaderIntro() {
    if (!header || typeof gsap === 'undefined') {
      header?.classList.remove('is-intro');
      return;
    }

    const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (reduceMotion) {
      header.classList.remove('is-intro');
      syncHeaderHeight();
      return;
    }

    function playHeaderIntro() {
      const parts = header.querySelectorAll('.logo-link, .primary-nav, .header-actions');

      // Whole bar fades in after loader — no empty header strip during load
      gsap.set(header, { autoAlpha: 0, y: -14, force3D: true });
      gsap.set(parts, { y: -8, autoAlpha: 0, force3D: true });

      const tl = gsap.timeline({
        delay: 0.12,
        onComplete() {
          header.classList.remove('is-intro');
          gsap.set(header, { clearProps: 'transform,opacity,visibility' });
          gsap.set(parts, { clearProps: 'transform,opacity,visibility' });
          syncHeaderHeight();
        },
      });

      tl.to(header, {
        autoAlpha: 1,
        y: 0,
        duration: 0.7,
        ease: 'power3.out',
        force3D: true,
      }).to(
        parts,
        {
          y: 0,
          autoAlpha: 1,
          duration: 0.55,
          stagger: 0.07,
          ease: 'power2.out',
          force3D: true,
        },
        '-=0.4'
      );
    }

    // Homepage preloader owns the header handoff — don't fight it with a second intro
    if (document.documentElement.classList.contains('site-preloader-pending') ||
        document.getElementById('sitePreloader')) {
      const skipIntro = () => {
        header.classList.remove('is-intro');
        syncHeaderHeight();
      };
      document.addEventListener('homepagePreloaderDone', skipIntro, { once: true });
      // Safety if preloader already gone / skipped
      if (document.documentElement.classList.contains('site-preloader-skip') ||
          !document.getElementById('sitePreloader')) {
        skipIntro();
      }
      return;
    }

    playHeaderIntro();
  })();

  /* ---------- desktop interaction ---------- */
  const megaPanel = document.getElementById('megaPanel');
  const megaInner = document.getElementById('megaInner');
  const megaBackdrop = document.getElementById('megaBackdrop');
  const navPill = document.getElementById('navPill');
  const navItems = Array.from(primaryNav.querySelectorAll('.nav-item'));

  let activeKey = null;
  let closeTimer = null;

  function positionPill(btn){
    if(!btn){ navPill.classList.remove('is-visible'); return; }
    const navRect = primaryNav.getBoundingClientRect();
    const r = btn.getBoundingClientRect();
    navPill.style.width = r.width + 'px';
    navPill.style.transform = `translate(${r.left - navRect.left}px, -50%)`;
    navPill.classList.add('is-visible');
  }

  function openMenu(key){
    clearTimeout(closeTimer);
    if(!getSection(key)) return;
    activeKey = key;

    navItems.forEach(el=>{
      const isMatch = el.dataset.key === key;
      el.classList.toggle('is-active', isMatch);
      if(isMatch) positionPill(el.querySelector('.nav-trigger'));
    });

    fillMegaPanel(key);
    megaPanel.classList.add('is-open');
    megaBackdrop.classList.add('is-visible');

    // measure natural height for smooth transition
    megaPanel.style.height = 'auto';
    const target = megaPanel.scrollHeight;
    megaPanel.style.height = '0px';
    requestAnimationFrame(()=>{
      megaPanel.style.height = target + 'px';
    });
  }

  function closeMenu(){
    activeKey = null;
    navItems.forEach(el=>el.classList.remove('is-active'));
    navPill.classList.remove('is-visible');
    megaPanel.classList.remove('is-open');
    megaBackdrop.classList.remove('is-visible');
    megaPanel.style.height = '0px';
  }

  navItems.forEach(el=>{
    if (el.classList.contains('is-link')) {
      el.addEventListener('mouseenter', ()=> closeMenu());
      return;
    }
    el.addEventListener('mouseenter', ()=> openMenu(el.dataset.key));
    el.querySelector('.nav-trigger').addEventListener('focus', ()=> openMenu(el.dataset.key));
  });
  primaryNav.addEventListener('mouseleave', ()=>{
    closeTimer = setTimeout(closeMenu, 160);
  });
  megaPanel.addEventListener('mouseenter', ()=> clearTimeout(closeTimer));
  megaPanel.addEventListener('mouseleave', ()=>{
    closeTimer = setTimeout(closeMenu, 160);
  });
  megaBackdrop.addEventListener('click', closeMenu);
  document.addEventListener('keydown', e=>{ if(e.key==='Escape') closeMenu(); });

  window.addEventListener('resize', ()=>{
    if(activeKey){
      const el = navItems.find(n=>n.dataset.key===activeKey);
      positionPill(el ? el.querySelector('.nav-trigger') : null);
    }else{
      navPill.classList.remove('is-visible');
    }
  });

  /* ---------- scroll behavior: shadow, compact, hide-on-scroll-down ---------- */
  let lastY = window.scrollY;
  window.addEventListener('scroll', ()=>{
    const y = window.scrollY;
    header.classList.toggle('is-scrolled', y > 4);
    header.classList.toggle('is-compact', y > 80);
    // Do NOT sync --header-h or ScrollTrigger.refresh() on compact â€”
    // that changes layout offsets and flickers the top banner.

    const scrollingDown = y > lastY;
    const pastThreshold = y > 140;
    const menusClosed = !activeKey && !document.getElementById('mobileNav').classList.contains('is-open');

    if(scrollingDown && pastThreshold && menusClosed){
      header.classList.add('is-hidden');
      closeMenu();
    }else if(!scrollingDown){
      header.classList.remove('is-hidden');
    }
    lastY = y;
  }, {passive:true});

  /* ---------- language dropdown ---------- */
const langWrap = document.getElementById('langWrap');
const langBtn = document.getElementById('langBtn');

if (langWrap && langBtn) {
  langBtn.addEventListener('click', (e) => {
    e.stopPropagation();
    langWrap.classList.toggle('is-open');
  });
  document.addEventListener('click', () => langWrap.classList.remove('is-open'));
}

  /* ---------- cmd-k style search overlay ---------- */
  const searchTrigger = document.getElementById('searchTrigger');
  const searchOverlay = document.getElementById('searchOverlay');
  const searchModal = document.getElementById('searchModal');
  const searchInput = document.getElementById('searchInput');
  const searchInputWrap = document.getElementById('searchInputWrap');
  const searchClear = document.getElementById('searchClear');
  const searchModalClose = document.getElementById('searchModalClose');

  function openSearch(){
    searchOverlay.classList.add('is-open');
    searchTrigger.classList.add('is-active');
    document.body.style.overflow = 'hidden';
    setTimeout(()=> searchInput.focus(), 150);
  }

  function closeSearch(){
    searchOverlay.classList.remove('is-open');
    searchTrigger.classList.remove('is-active');
    searchModal.classList.remove('is-focused');
    document.body.style.overflow = '';
    searchInput.value = '';
    searchInputWrap.classList.remove('has-value');
    searchInput.focus();
  }

  searchInput.addEventListener('focus', ()=> searchModal.classList.add('is-focused'));
  searchInput.addEventListener('blur', ()=> searchModal.classList.remove('is-focused'));

  searchTrigger.addEventListener('click', openSearch);
  searchModalClose.addEventListener('click', closeSearch);
  searchClear.addEventListener('click', ()=>{
    searchInput.value = '';
    searchInputWrap.classList.remove('has-value');
    searchInput.focus();
  });
  searchInput.addEventListener('input', ()=>{
    searchInputWrap.classList.toggle('has-value', searchInput.value.length > 0);
  });
  searchOverlay.addEventListener('click', (e)=>{
    if(e.target === searchOverlay) closeSearch();
  });
  document.addEventListener('keydown', (e)=>{
    if((e.metaKey || e.ctrlKey) && e.key.toLowerCase()==='k'){
      e.preventDefault();
      searchOverlay.classList.contains('is-open') ? closeSearch() : openSearch();
    }
    if(!searchOverlay.classList.contains('is-open')) return;
    if(e.key === 'Escape') closeSearch();
  });

  /* ---------- mobile drawer (drill-down) ---------- */
  const mobileToggle = document.getElementById('mobileToggle');
  const mobileNav = document.getElementById('mobileNav');
  const drawerOverlay = document.getElementById('drawerOverlay');
  const drawerBody = document.getElementById('drawerBody');
  const drawerRoot = document.getElementById('drawerRoot');
  const drawerSub = document.getElementById('drawerSub');
  const drawerClose = document.getElementById('drawerClose');

  const backSVG = '<svg viewBox="0 0 16 16" fill="none"><path d="M10 3L4 8l6 5" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round"/></svg>';

  function buildDrawerRoot(){
    drawerRoot.innerHTML = '';
    sections.forEach(section=>{
      const href = section.dataset.href;
      if (href) {
        const row = document.createElement('a');
        row.className = 'drawer-row';
        row.href = href;
        row.textContent = section.dataset.label;
        drawerRoot.appendChild(row);
        return;
      }
      const row = document.createElement('button');
      row.className = 'drawer-row';
      row.innerHTML = `${section.dataset.label}${chevSVG}`;
      row.addEventListener('click', ()=> openSubmenu(section.dataset.key));
      drawerRoot.appendChild(row);
    });
  }

  function openSubmenu(key){
    const section = getSection(key);
    if(!section) return;

    drawerSub.innerHTML = `
      <div class="drawer-sub-header">
        <button class="drawer-back" aria-label="Back to menu">${backSVG}</button>
        <span class="drawer-sub-title">${section.dataset.label}</span>
      </div>`;

    // same source markup as desktop — links list + optional promo card
    const links = section.querySelector('.mega-links');
    if (links) {
      const linksClone = links.cloneNode(true);
      linksClone.className = 'drawer-links';
      drawerSub.appendChild(linksClone);
    }

    const promo = section.querySelector('.mega-promo');
    if (promo) {
      drawerSub.appendChild(promo.cloneNode(true));
    }

    drawerSub.querySelector('.drawer-back').addEventListener('click', closeSubmenu);
    drawerSub.scrollTop = 0;
    drawerBody.classList.add('is-sub');
  }

  function closeSubmenu(){
    drawerBody.classList.remove('is-sub');
  }

  buildDrawerRoot();

  function openDrawer(){
    mobileNav.classList.add('is-open');
    drawerOverlay.classList.add('is-open');
    mobileToggle.classList.add('is-active');
    document.body.style.overflow = 'hidden';
  }
  function closeDrawer(){
    mobileNav.classList.remove('is-open');
    drawerOverlay.classList.remove('is-open');
    mobileToggle.classList.remove('is-active');
    document.body.style.overflow = '';
    setTimeout(closeSubmenu, 400); // reset to root only after the drawer is off-screen
  }

  mobileToggle.addEventListener('click', ()=>{
    mobileNav.classList.contains('is-open') ? closeDrawer() : openDrawer();
  });
  drawerClose.addEventListener('click', closeDrawer);
  drawerOverlay.addEventListener('click', closeDrawer);
  document.addEventListener('keydown', e=>{
    if(e.key !== 'Escape' || !mobileNav.classList.contains('is-open')) return;
    drawerBody.classList.contains('is-sub') ? closeSubmenu() : closeDrawer();
  });

  // swipe-right: back out of a submenu, or close the drawer from the root
  let touchStartX = 0, touchDeltaX = 0, dragging = false;
  mobileNav.addEventListener('touchstart', e=>{
    touchStartX = e.touches[0].clientX;
    dragging = true;
    mobileNav.style.transition = 'none';
  }, {passive:true});
  mobileNav.addEventListener('touchmove', e=>{
    if(!dragging) return;
    touchDeltaX = Math.max(0, e.touches[0].clientX - touchStartX);
    mobileNav.style.transform = `translateX(${touchDeltaX}px)`;
  }, {passive:true});
  mobileNav.addEventListener('touchend', ()=>{
    dragging = false;
    mobileNav.style.transition = '';
    mobileNav.style.transform = '';
    if(touchDeltaX > 90){
      drawerBody.classList.contains('is-sub') ? closeSubmenu() : closeDrawer();
    }
    touchDeltaX = 0;
  });

  // Footer link columns accordion (mobile only — one open at a time)
  (function initFooterLinkAccordions() {
    const outer = document.querySelector('.linkcolumns-outer');
    if (!outer) return;

    const blocks = Array.from(outer.querySelectorAll('.linkcolumns-block'));
    if (!blocks.length) return;

    const mq = window.matchMedia('(max-width: 767px)');

    function panel(block) {
      return block.querySelector(':scope > ul');
    }

    function closeBlock(block) {
      const ul = panel(block);
      const title = block.querySelector('h4');
      if (ul && block.classList.contains('is-open')) {
        ul.style.maxHeight = ul.scrollHeight + 'px';
        void ul.offsetHeight;
        ul.style.maxHeight = '0px';
      } else if (ul) {
        ul.style.maxHeight = '0px';
      }
      block.classList.remove('is-open');
      if (title) title.setAttribute('aria-expanded', 'false');
    }

    function openBlock(block) {
      const ul = panel(block);
      const title = block.querySelector('h4');
      block.classList.add('is-open');
      if (ul) {
        // measure full open height (with spacing) then animate from 0
        const transition = ul.style.transition;
        ul.style.transition = 'none';
        ul.style.maxHeight = 'none';
        const fullHeight = ul.scrollHeight;
        ul.style.maxHeight = '0px';
        void ul.offsetHeight;
        ul.style.transition = transition;
        ul.style.maxHeight = fullHeight + 'px';
      }
      if (title) title.setAttribute('aria-expanded', 'true');
    }

    function clearInlineHeights() {
      blocks.forEach(block => {
        const ul = panel(block);
        if (ul) ul.style.maxHeight = '';
        block.classList.remove('is-open');
        const title = block.querySelector('h4');
        if (title) {
          title.removeAttribute('role');
          title.removeAttribute('tabindex');
          title.removeAttribute('aria-expanded');
        }
      });
    }

    function syncA11y() {
      if (!mq.matches) {
        clearInlineHeights();
        return;
      }
      blocks.forEach(block => {
        const title = block.querySelector('h4');
        const ul = panel(block);
        if (title) {
          title.setAttribute('role', 'button');
          title.setAttribute('tabindex', '0');
          title.setAttribute('aria-expanded', block.classList.contains('is-open') ? 'true' : 'false');
        }
        if (ul && !block.classList.contains('is-open')) {
          ul.style.maxHeight = '0px';
        }
      });
    }

    blocks.forEach(block => {
      const title = block.querySelector('h4');
      if (!title) return;

      title.addEventListener('click', () => {
        if (!mq.matches) return;
        const willOpen = !block.classList.contains('is-open');
        blocks.forEach(b => {
          if (b !== block) closeBlock(b);
        });
        if (willOpen) openBlock(block);
        else closeBlock(block);
      });

      title.addEventListener('keydown', e => {
        if (!mq.matches) return;
        if (e.key !== 'Enter' && e.key !== ' ') return;
        e.preventDefault();
        title.click();
      });
    });

    // keep open panel height correct on resize/orientation
    window.addEventListener('resize', () => {
      if (!mq.matches) return;
      blocks.forEach(block => {
        if (!block.classList.contains('is-open')) return;
        const ul = panel(block);
        if (ul) ul.style.maxHeight = ul.scrollHeight + 'px';
      });
    });

    syncA11y();
    if (mq.addEventListener) mq.addEventListener('change', syncA11y);
    else mq.addListener(syncA11y);
  })();
});