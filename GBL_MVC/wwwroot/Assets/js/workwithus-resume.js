document.addEventListener("DOMContentLoaded", function () {
  var input = document.getElementById("resume");
  var name = document.querySelector("[data-file-name]");
  if (!input || !name) return;

  input.addEventListener("change", function () {
    name.textContent = (input.files && input.files[0]) ? input.files[0].name : "Upload resume";
  });
});
