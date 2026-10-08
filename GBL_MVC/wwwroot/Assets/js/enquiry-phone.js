(function () {
    var countries = [
        { name: "India", code: "+91", flag: "🇮🇳" },
        { name: "United States", code: "+1", flag: "🇺🇸" },
        { name: "United Kingdom", code: "+44", flag: "🇬🇧" },
        { name: "United Arab Emirates", code: "+971", flag: "🇦🇪" },
        { name: "Singapore", code: "+65", flag: "🇸🇬" },
        { name: "Germany", code: "+49", flag: "🇩🇪" },
        { name: "France", code: "+33", flag: "🇫🇷" },
        { name: "Australia", code: "+61", flag: "🇦🇺" },
        { name: "Japan", code: "+81", flag: "🇯🇵" },
        { name: "China", code: "+86", flag: "🇨🇳" },
        { name: "Brazil", code: "+55", flag: "🇧🇷" },
        { name: "South Africa", code: "+27", flag: "🇿🇦" },
        { name: "Saudi Arabia", code: "+966", flag: "🇸🇦" },
        { name: "Canada", code: "+1", flag: "🇨🇦" },
        { name: "Netherlands", code: "+31", flag: "🇳🇱" }
    ];

    function closeAll() {
        document.querySelectorAll(".enquiry-phone__list").forEach(function (list) {
            list.hidden = true;
        });
        document.querySelectorAll(".enquiry-country__btn, .enquiry-phone__code-btn").forEach(function (btn) {
            btn.setAttribute("aria-expanded", "false");
        });
    }

    function fillCountryList() {
        var list = document.getElementById("countryList");
        if (!list) return;
        list.innerHTML = "";
        countries.forEach(function (c) {
            var li = document.createElement("li");
            li.setAttribute("role", "option");
            li.tabIndex = 0;
            li.textContent = c.name;
            li.dataset.name = c.name;
            li.dataset.code = c.code;
            li.dataset.flag = c.flag;
            list.appendChild(li);
        });
    }

    function fillCodeList() {
        var list = document.getElementById("countryCodeList");
        if (!list) return;
        list.innerHTML = "";
        countries.forEach(function (c) {
            var li = document.createElement("li");
            li.setAttribute("role", "option");
            li.tabIndex = 0;
            li.innerHTML = '<span aria-hidden="true">' + c.flag + "</span> " + c.name + " " + c.code;
            li.dataset.name = c.name;
            li.dataset.code = c.code;
            li.dataset.flag = c.flag;
            list.appendChild(li);
        });
    }

    function setCountry(name, code, flag) {
        var countryInput = document.getElementById("Country");
        var countryValue = document.querySelector(".enquiry-country__value");
        var countryBtn = document.querySelector(".enquiry-country__btn");
        if (countryInput) countryInput.value = name;
        if (countryValue) countryValue.textContent = name;
        if (countryBtn) countryBtn.setAttribute("aria-label", "Country, " + name);

        var codeInput = document.getElementById("CountryCode");
        var codeValue = document.querySelector(".enquiry-phone__code-value");
        var codeFlag = document.querySelector(".enquiry-phone__flag");
        var codeBtn = document.querySelector(".enquiry-phone__code-btn");
        if (codeInput) codeInput.value = code;
        if (codeValue) codeValue.textContent = code;
        if (codeFlag) codeFlag.textContent = flag || "";
        if (codeBtn) codeBtn.setAttribute("aria-label", "Country code, " + name + " " + code);
    }

    document.addEventListener("DOMContentLoaded", function () {
        fillCountryList();
        fillCodeList();

        var countryBtn = document.querySelector(".enquiry-country__btn");
        var codeBtn = document.querySelector(".enquiry-phone__code-btn");
        var countryList = document.getElementById("countryList");
        var codeList = document.getElementById("countryCodeList");

        if (countryBtn && countryList) {
            countryBtn.addEventListener("click", function (e) {
                e.stopPropagation();
                var open = countryList.hidden;
                closeAll();
                countryList.hidden = !open;
                countryBtn.setAttribute("aria-expanded", open ? "true" : "false");
            });

            countryList.addEventListener("click", function (e) {
                var li = e.target.closest("li[role='option']");
                if (!li) return;
                setCountry(li.dataset.name, li.dataset.code, li.dataset.flag);
                closeAll();
            });
        }

        if (codeBtn && codeList) {
            codeBtn.addEventListener("click", function (e) {
                e.stopPropagation();
                var open = codeList.hidden;
                closeAll();
                codeList.hidden = !open;
                codeBtn.setAttribute("aria-expanded", open ? "true" : "false");
            });

            codeList.addEventListener("click", function (e) {
                var li = e.target.closest("li[role='option']");
                if (!li) return;
                setCountry(li.dataset.name, li.dataset.code, li.dataset.flag);
                closeAll();
            });
        }

        document.addEventListener("click", closeAll);

        // Restrict phone-like fields to digits / optional leading +
        ["Phone", "Mobile", "Fax"].forEach(function (id) {
            var el = document.getElementById(id);
            if (!el) return;
            el.addEventListener("input", function () {
                var v = el.value || "";
                var hasPlus = v.trim().charAt(0) === "+";
                var digits = v.replace(/\D/g, "").slice(0, 15);
                el.value = hasPlus ? "+" + digits : digits;
            });
        });
    });
})();
