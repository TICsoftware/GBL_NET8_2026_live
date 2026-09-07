// $(document).on("submit", ".search-form", function (e) {

//     e.preventDefault();

//     var keyword = $("#searchInput").val().trim();

//     if (keyword == "") {

//         alert("Please enter search keyword.");

//         $("#searchInput").focus();

//         return false;
//     }

//     window.location.href =
//         "/search/" + encodeURIComponent(keyword);
// });




$(document).on("submit", ".search-form", function (e) {

    e.preventDefault();

    var keyword = $(this).find("#searchInput").val().trim();

    console.log("Keyword:", keyword);

    if (!keyword) {
        alert("Please enter search keyword.");

        $(this).find("#searchInput").focus();

        return false;
    }

    window.location.href = "/search/" + encodeURIComponent(keyword);
});