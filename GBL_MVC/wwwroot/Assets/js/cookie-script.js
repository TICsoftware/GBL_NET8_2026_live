$(document).ready(function () {
  const popup = $("#popup-privacypolicy");

  // Initial hidden state (prevent flash)
  popup.css({
    position: "fixed",
    bottom: "-100px",
    left: "0",
    right: "0",
    // display: "none",
    zIndex: "9999"
  });

  // Check localStorage
  if (localStorage.getItem("popState") !== "shown") {

    // Show popup after delay
    setTimeout(function () {
      popup
        .css({ display: "flex", opacity: 0 })
        .animate({ bottom: "0", opacity: 1 }, 600);
    }, 2000);

    $(".gcode-div").empty();

  } else {
    loadGAScript();
  }

  // ACCEPT BUTTON
  $("#acceptCookie, .privacy_close").on("click", function () {
    popup.animate({ bottom: "-100px", opacity: 0 }, 400, function () {
      popup.hide();
    });

    // Save state (IMPORTANT FIX)
    localStorage.setItem("popState", "shown");

    loadGAScript();
  });

  // Load GA script function
  function loadGAScript() {
    if (!document.querySelector('script[src*="GA_Header_Script.js"]')) {
      let script = document.createElement("script");
      script.type = "text/javascript";
      script.src = "/Assets/js/GA_Header_Script.js";
      document.head.appendChild(script);
    }
  }
});