document.addEventListener("DOMContentLoaded", () => {

  const accordions = document.querySelectorAll(".faq-container");

  accordions.forEach((container) => {

    container.addEventListener("click", (e) => {
      const toggle = e.target.closest(".faq-toggle");
      if (!toggle) return;

      const item = toggle.parentElement;
      const content = item.querySelector(".faq-answer");
      const isOpen = item.classList.contains("open");

      // CLOSE ALL (within THIS container only)
      container.querySelectorAll(".faq-item").forEach((el) => {
        const elContent = el.querySelector(".faq-answer");
        const verticalLine = el.querySelector(".line-vertical");

        el.classList.remove("open");
        elContent.style.height = "0px";
        elContent.style.opacity = "0";

        if (verticalLine) {
          verticalLine.style.transform = "scaleY(1)";
        }
      });

      // OPEN CLICKED
      if (!isOpen) {
        item.classList.add("open");

        content.style.height = content.scrollHeight + "px";
        content.style.opacity = "1";

        const verticalLine = item.querySelector(".line-vertical");
        if (verticalLine) {
          verticalLine.style.transform = "scaleY(0)";
        }
      }
    });

    // INIT DEFAULT OPEN (per container)
    container.querySelectorAll(".faq-item.open").forEach((item) => {
      const content = item.querySelector(".faq-answer");
      const verticalLine = item.querySelector(".line-vertical");

      content.style.height = content.scrollHeight + "px";
      content.style.opacity = "1";

      if (verticalLine) {
        verticalLine.style.transform = "scaleY(0)";
      }
    });

  });

});