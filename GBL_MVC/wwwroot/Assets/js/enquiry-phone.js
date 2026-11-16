(function () {
  var root = document.querySelector(".enquiry-phone__code");
  if (!root) return;

  var countries = [
    { name: "India", dial: "+91", iso: "in" },
    { name: "Australia", dial: "+61", iso: "au" },
    { name: "Austria", dial: "+43", iso: "at" },
    { name: "Bahrain", dial: "+973", iso: "bh" },
    { name: "Bangladesh", dial: "+880", iso: "bd" },
    { name: "Belgium", dial: "+32", iso: "be" },
    { name: "Brazil", dial: "+55", iso: "br" },
    { name: "Canada", dial: "+1", iso: "ca" },
    { name: "China", dial: "+86", iso: "cn" },
    { name: "Denmark", dial: "+45", iso: "dk" },
    { name: "Egypt", dial: "+20", iso: "eg" },
    { name: "France", dial: "+33", iso: "fr" },
    { name: "Germany", dial: "+49", iso: "de" },
    { name: "Hong Kong", dial: "+852", iso: "hk" },
    { name: "Indonesia", dial: "+62", iso: "id" },
    { name: "Ireland", dial: "+353", iso: "ie" },
    { name: "Italy", dial: "+39", iso: "it" },
    { name: "Japan", dial: "+81", iso: "jp" },
    { name: "Kenya", dial: "+254", iso: "ke" },
    { name: "Kuwait", dial: "+965", iso: "kw" },
    { name: "Malaysia", dial: "+60", iso: "my" },
    { name: "Mexico", dial: "+52", iso: "mx" },
    { name: "Nepal", dial: "+977", iso: "np" },
    { name: "Netherlands", dial: "+31", iso: "nl" },
    { name: "New Zealand", dial: "+64", iso: "nz" },
    { name: "Nigeria", dial: "+234", iso: "ng" },
    { name: "Norway", dial: "+47", iso: "no" },
    { name: "Oman", dial: "+968", iso: "om" },
    { name: "Pakistan", dial: "+92", iso: "pk" },
    { name: "Philippines", dial: "+63", iso: "ph" },
    { name: "Portugal", dial: "+351", iso: "pt" },
    { name: "Qatar", dial: "+974", iso: "qa" },
    { name: "Russia", dial: "+7", iso: "ru" },
    { name: "Saudi Arabia", dial: "+966", iso: "sa" },
    { name: "Singapore", dial: "+65", iso: "sg" },
    { name: "South Africa", dial: "+27", iso: "za" },
    { name: "South Korea", dial: "+82", iso: "kr" },
    { name: "Spain", dial: "+34", iso: "es" },
    { name: "Sri Lanka", dial: "+94", iso: "lk" },
    { name: "Sweden", dial: "+46", iso: "se" },
    { name: "Switzerland", dial: "+41", iso: "ch" },
    { name: "Thailand", dial: "+66", iso: "th" },
    { name: "United Arab Emirates", dial: "+971", iso: "ae" },
    { name: "United Kingdom", dial: "+44", iso: "gb" },
    { name: "United States", dial: "+1", iso: "us" },
    { name: "Vietnam", dial: "+84", iso: "vn" }
  ];

  var countryRoot = document.querySelector(".enquiry-country");
  var countryInput = document.getElementById("country");
  var countryBtn = countryRoot && countryRoot.querySelector(".enquiry-country__btn");
  var countryValue = countryRoot && countryRoot.querySelector(".enquiry-country__value");
  var countryList = document.getElementById("countryList");
  var input = root.querySelector("#countryCode");
  var button = root.querySelector(".enquiry-phone__code-btn");
  var flag = root.querySelector(".enquiry-phone__flag");
  var value = root.querySelector(".enquiry-phone__code-value");
  var list = root.querySelector(".enquiry-phone__list");
  var active = 0;

  function addOption(target, country, index, withDial) {
    var item = document.createElement("li");
    item.className = "enquiry-phone__option" + (index === 0 ? " is-active" : "");
    item.setAttribute("role", "option");
    item.setAttribute("tabindex", "-1");
    item.setAttribute("aria-selected", index === 0 ? "true" : "false");
    item.dataset.index = String(index);

    var name = document.createElement("span");
    name.className = "enquiry-phone__option-name";
    name.textContent = country.name;

    if (withDial) {
      var img = document.createElement("img");
      img.src = "/Assets/images/flags/" + country.iso + ".svg";
      img.alt = "";
      img.width = 22;
      img.height = 16;
      item.append(img);
    }

    item.append(name);
    if (withDial) {
      var dial = document.createElement("span");
      dial.className = "enquiry-phone__option-dial";
      dial.textContent = country.dial;
      item.append(dial);
    }
    target.appendChild(item);
  }

  countries.forEach(function (country, index) {
    if (countryList) addOption(countryList, country, index, false);
    addOption(list, country, index, true);
  });

  function isOpen(btn) {
    return btn.getAttribute("aria-expanded") === "true";
  }

  function scrollOptionIntoMenu(menu, option) {
    if (!menu || !option) return;
    var top = option.offsetTop;
    var bottom = top + option.offsetHeight;
    if (top < menu.scrollTop) menu.scrollTop = top;
    else if (bottom > menu.scrollTop + menu.clientHeight) menu.scrollTop = bottom - menu.clientHeight;
  }

  function setOpen(btn, menu, open) {
    btn.setAttribute("aria-expanded", open ? "true" : "false");
    menu.hidden = !open;
    if (open) scrollOptionIntoMenu(menu, menu.querySelector(".is-active"));
  }

  function closeAll() {
    setOpen(button, list, false);
    if (countryBtn && countryList) setOpen(countryBtn, countryList, false);
  }

  function highlight(index) {
    active = index;
    [list, countryList].forEach(function (menu) {
      if (!menu) return;
      var options = menu.querySelectorAll(".enquiry-phone__option");
      options.forEach(function (option, i) {
        var on = i === index;
        option.classList.toggle("is-active", on);
        option.setAttribute("aria-selected", on ? "true" : "false");
      });
      if (options[index] && !menu.hidden) scrollOptionIntoMenu(menu, options[index]);
    });
  }

  function choose(index) {
    var country = countries[index];
    highlight(index);
    input.value = country.dial;
    value.textContent = country.dial;
    flag.src = "/Assets/images/flags/" + country.iso + ".svg";
    button.setAttribute("aria-label", "Country code, " + country.name + " " + country.dial);
    if (countryInput) countryInput.value = country.name;
    if (countryValue) countryValue.textContent = country.name;
    if (countryBtn) countryBtn.setAttribute("aria-label", "Country, " + country.name);
    closeAll();
  }

  function bindMenu(btn, menu) {
    btn.addEventListener("click", function () {
      var open = !isOpen(btn);
      closeAll();
      setOpen(btn, menu, open);
    });

    btn.addEventListener("keydown", function (event) {
      if (event.key === "ArrowDown") {
        event.preventDefault();
        if (!isOpen(btn)) {
          closeAll();
          setOpen(btn, menu, true);
        } else highlight(Math.min(active + 1, countries.length - 1));
      } else if (event.key === "ArrowUp") {
        event.preventDefault();
        if (!isOpen(btn)) {
          closeAll();
          setOpen(btn, menu, true);
        } else highlight(Math.max(active - 1, 0));
      } else if (event.key === "Enter" || event.key === " ") {
        event.preventDefault();
        if (isOpen(btn)) choose(active);
        else {
          closeAll();
          setOpen(btn, menu, true);
        }
      } else if (event.key === "Escape" && isOpen(btn)) {
        event.preventDefault();
        setOpen(btn, menu, false);
      }
    });

    menu.addEventListener("click", function (event) {
      var option = event.target.closest("[data-index]");
      if (!option) return;
      choose(Number(option.dataset.index));
    });
  }

  function keepListScroll(menu) {
    menu.addEventListener("wheel", function (event) {
      event.stopPropagation();
    }, { passive: true });
  }

  keepListScroll(list);
  if (countryList) keepListScroll(countryList);

  bindMenu(button, list);
  if (countryBtn && countryList) bindMenu(countryBtn, countryList);

  document.addEventListener("click", function (event) {
    var inPhone = root.contains(event.target);
    var inCountry = countryRoot && countryRoot.contains(event.target);
    if (!inPhone && !inCountry) closeAll();
  });
})();
