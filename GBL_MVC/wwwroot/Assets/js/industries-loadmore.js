(function () {
  const PAGE_SIZE = 10;
  let page = 1;

  const list = document.getElementById("industryList");
  const loadMoreBtn = document.getElementById("btnLoadMoreIndustries");
  if (!list) return;

  function escapeHtml(value) {
    return String(value || "").replace(/[&<>"']/g, function (ch) {
      return ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" })[ch];
    });
  }

  function buildCard(item) {
    const name = item.industryName || item.IndustryName || "";
    const pageName = item.industry_pagename || item.Industry_pagename || "";
    const intro = item.intro || item.Intro || "";
    const thumb = item.thumbnailUrl || item.ThumbnailUrl || "";
    const alt = item.thumbnailAlt || item.ThumbnailAlt || name;
    const href = pageName ? "/IndustriesInside/" + encodeURIComponent(pageName) : "/IndustriesInside";
    const img = thumb
      ? '<img src="' + escapeHtml(thumb) + '" alt="' + escapeHtml(alt) + '">'
      : "";
    const introHtml = intro ? "<p>" + escapeHtml(intro) + "</p>" : "";

    return (
      '<a href="' + escapeHtml(href) + '" class="industry-card" data-industry="' + escapeHtml(pageName) + '">' +
        '<div class="industry-card__media">' + img + "</div>" +
        '<div class="industry-card__caption">' +
          '<h3 class="text-h5">' + escapeHtml(name) + "</h3>" +
          '<div class="industry-card__details">' +
            '<div class="industry-card__details-inner">' +
              introHtml +
              '<div class="site-btn site-btn--line">' +
                '<span class="site-btn__label">Explore Portfolio</span>' +
                '<span class="site-btn__arrow" aria-hidden="true">' +
                  '<svg viewBox="0 0 24 24" fill="none"><path d="M5 12h14M13 6l6 6-6 6" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>' +
                "</span>" +
              "</div>" +
            "</div>" +
          "</div>" +
        "</div>" +
      "</a>"
    );
  }

  function toggleLoadMore(total, currentPage) {
    if (!loadMoreBtn) return;
    loadMoreBtn.style.display = (currentPage * PAGE_SIZE) < total ? "" : "none";
  }

  function postJson(url, payload) {
    const body = new URLSearchParams();
    Object.keys(payload).forEach(function (key) {
      if (payload[key] !== null && payload[key] !== undefined && payload[key] !== "") {
        body.append(key, payload[key]);
      }
    });
    return fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8" },
      body: body.toString()
    }).then(function (res) { return res.json(); });
  }

  function loadMore() {
    page += 1;
    const payload = {
      PageNumber: page,
      PageSize: PAGE_SIZE
    };
    postJson("/Industries/LoadMore", payload)
      .then(function (res) {
        const items = res && res.items ? res.items : [];
        if (!items.length) {
          toggleLoadMore(0, page);
          return;
        }
        const empty = list.querySelector(".js-industry-empty");
        if (empty) empty.remove();
        list.insertAdjacentHTML("beforeend", items.map(buildCard).join(""));
        toggleLoadMore(res.total || 0, page);
        list.setAttribute("data-page", String(page));
      })
      .catch(function () {
        page -= 1;
      });
  }

  const initialTotal = parseInt(list.getAttribute("data-total") || "0", 10);
  const initialPage = parseInt(list.getAttribute("data-page") || "1", 10);
  page = initialPage > 0 ? initialPage : 1;
  toggleLoadMore(initialTotal, page);

  loadMoreBtn?.addEventListener("click", function () {
    loadMore();
  });
})();
