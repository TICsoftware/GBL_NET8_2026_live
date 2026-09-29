(function ($) {
  $(function () {
    var $root = $(".product-filter-outer");
    if (!$root.length) return;

    var $form = $("#product-filter");
    var $grid = $("#product-grid");
    var $count = $("[data-product-count]");
    var $chips = $("[data-filter-chips]");
    var $empty = $("[data-filter-empty]");
    var $loadMore = $("#btnLoadMoreProducts");
    var pageSize = parseInt($root.attr("data-page-size") || "9", 10);
    var page = 1;
    var openDropdown = null;

    function padCount(value) {
      var n = parseInt(value, 10) || 0;
      return n < 10 ? "0" + n : String(n);
    }

    function escapeHtml(value) {
      return String(value || "").replace(/[&<>"']/g, function (ch) {
        return ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" })[ch];
      });
    }

    function selectedIds(name) {
      return $form.find('input[name="' + name + '"]:checked').map(function () {
        return this.value;
      }).get();
    }

    function getFilters() {
      return {
        IndustryIds: selectedIds("industry").join(","),
        ApplicationIds: selectedIds("application").join(","),
        CategoryIds: selectedIds("category").join(","),
        PageSize: pageSize
      };
    }

    function closeDropdown($wrap) {
      if (!$wrap || !$wrap.length) return;
      $wrap.removeClass("is-open");
      $wrap.find(".pf-dropdown__panel").attr("aria-hidden", "true");
      $wrap.find(".pf-dropdown__trigger").attr("aria-expanded", "false");
      if (openDropdown && openDropdown[0] === $wrap[0]) openDropdown = null;
    }

    function openMenu($wrap) {
      if (openDropdown && openDropdown[0] !== $wrap[0]) closeDropdown(openDropdown);
      $wrap.addClass("is-open");
      $wrap.find(".pf-dropdown__panel").attr("aria-hidden", "false");
      $wrap.find(".pf-dropdown__trigger").attr("aria-expanded", "true");
      openDropdown = $wrap;
    }

    function updateDropdownState() {
      $root.find(".pf-dropdown").each(function () {
        var $wrap = $(this);
        $wrap.toggleClass("has-value", $wrap.find("input[type='checkbox']:checked").length > 0);
      });
    }

    function renderChips() {
      $chips.empty();
      $form.find("input[type='checkbox']:checked").each(function () {
        var $input = $(this);
        var label = $input.attr("data-label") || $input.val();
        var $chip = $('<span class="product-filter-chip"></span>').text(label);
        var $remove = $('<button type="button" class="product-filter-chip__remove" aria-label="Remove ' + escapeHtml(label) + '">&times;</button>');
        $remove.on("click", function () {
          $input.prop("checked", false);
          loadProducts();
        });
        $chip.append($remove);
        $chips.append($chip);
      });
    }

    function updateCounts(filters, key) {
      if (!filters || !filters.length) return;
      var map = {};
      filters.forEach(function (item) {
        var id = item.id != null ? item.id : item.Id;
        map[String(id)] = item.count != null ? item.count : item.Count;
      });
      $form.find('input[name="' + key + '"]').each(function () {
        var count = map[this.value];
        $(this).closest(".pf-option").find(".pf-option__count").text(padCount(count || 0));
      });
    }

    function buildCard(item) {
      var name = item.productName || item.ProductName || "";
      var pageName = item.product_pagename || item.Product_pagename || "";
      var thumb = item.thumbnailUrl || item.ThumbnailUrl || "";
      var alt = item.thumbnailAlt || item.ThumbnailAlt || name;
      var industries = item.industries || item.Industries || [];
      var href = pageName ? "/Products/" + encodeURIComponent(pageName) : "/Products/Inside_html";
      var img = thumb ? '<img src="' + escapeHtml(thumb) + '" alt="' + escapeHtml(alt) + '">' : "";
      var industriesHtml = industries.length
        ? '<p class="product-card__meta-label">Industries</p>' +
          '<p class="product-card__meta">' + escapeHtml(industries.join(", ")) + "</p>"
        : "";
      return (
        '<a href="' + escapeHtml(href) + '" class="product-card">' +
          '<div class="product-card__media">' + img + "</div>" +
          '<div class="product-card__body">' +
            '<h3 class="text-h5">' + escapeHtml(name) + "</h3>" +
            industriesHtml +
          "</div>" +
        "</a>"
      );
    }

    function toggleLoadMore(total, currentPage) {
      $loadMore.toggle((currentPage * pageSize) < (total || 0));
    }

    function renderGrid(items, total, append) {
      if (!append) $grid.empty();
      if (!items || !items.length) {
        if (!append) {
          $empty.prop("hidden", false);
          toggleLoadMore(0, 1);
        }
        return;
      }
      $empty.prop("hidden", true);
      $grid.append(items.map(buildCard).join(""));
      toggleLoadMore(total || 0, page);
    }

    function post(url, payload) {
      return $.ajax({
        url: url,
        type: "POST",
        data: payload
      });
    }

    function applyResult(res, append) {
      if (!res || !res.success) {
        if (!append) renderGrid([], 0, false);
        $count.text("0");
        return;
      }
      $count.text(String(res.total || 0));
      $root.attr("data-total", String(res.total || 0));
      renderGrid(res.items || [], res.total || 0, append);
      updateCounts(res.industryFilters || res.IndustryFilters, "industry");
      updateCounts(res.applicationFilters || res.ApplicationFilters, "application");
      updateCounts(res.categoryFilters || res.CategoryFilters, "category");
    }

    function loadProducts() {
      page = 1;
      renderChips();
      updateDropdownState();
      var payload = $.extend({ PageNumber: 1 }, getFilters());
      post("/Products/LoadProducts", payload)
        .done(function (res) { applyResult(res, false); })
        .fail(function () {
          renderGrid([], 0, false);
          $count.text("0");
        });
    }

    function loadMore() {
      page += 1;
      var payload = $.extend({ PageNumber: page }, getFilters());
      post("/Products/LoadMore", payload)
        .done(function (res) {
          if (!res || !res.success || !(res.items || []).length) {
            page -= 1;
            toggleLoadMore(0, page);
            return;
          }
          applyResult(res, true);
        })
        .fail(function () { page -= 1; });
    }

    $root.find(".pf-dropdown").each(function () {
      var $wrap = $(this);
      $wrap.find(".pf-dropdown__panel").attr("aria-hidden", "true");
      $wrap.find(".pf-dropdown__trigger").on("click", function (e) {
        e.preventDefault();
        if ($wrap.hasClass("is-open")) closeDropdown($wrap);
        else openMenu($wrap);
      });
    });

    $(document).on("click", function (e) {
      if (!openDropdown) return;
      if (!$(e.target).closest(openDropdown).length) closeDropdown(openDropdown);
    });

    $(document).on("keydown", function (e) {
      if (e.key !== "Escape" || !openDropdown) return;
      var $trigger = openDropdown.find(".pf-dropdown__trigger");
      closeDropdown(openDropdown);
      $trigger.trigger("focus");
    });

    $form.on("change", "input[type='checkbox']", function () {
      loadProducts();
    });

    $form.on("submit", function (e) {
      e.preventDefault();
      loadProducts();
    });

    $root.on("click", "[data-clear-filters]", function () {
      $form.find("input[type='checkbox']").prop("checked", false);
      if (openDropdown) closeDropdown(openDropdown);
      $root.removeClass("is-mobile-filter-open");
      loadProducts();
    });

    $("[data-open-mobile-filter]").on("click", function () {
      $root.addClass("is-mobile-filter-open");
      $(this).attr("aria-expanded", "true");
    });

    $("[data-close-mobile-filter]").on("click", function () {
      $root.removeClass("is-mobile-filter-open");
      $("[data-open-mobile-filter]").attr("aria-expanded", "false");
    });

    $("[data-apply-mobile-filter]").on("click", function () {
      $root.removeClass("is-mobile-filter-open");
      $("[data-open-mobile-filter]").attr("aria-expanded", "false");
      loadProducts();
    });

    $loadMore.on("click", loadMore);

    renderChips();
    updateDropdownState();
  });
})(jQuery);
