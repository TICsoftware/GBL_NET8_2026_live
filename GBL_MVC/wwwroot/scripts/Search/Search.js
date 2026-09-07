$(document).ready(function () {

    

    $(document).on("click", "#btnLoadMore", function (e) {
        e.preventDefault();
    
        var button = $(this);
    
        var page =
            parseInt($("#SearchCurrentPage").val()) + 1;
    
        var keyword =
            $("#SearchKeyword").val();
    
        var totalCount =
            parseInt($("#SearchTotalCount").val());
    
        button.text("Loading...");
        button.css("pointer-events", "none");
    
        $.ajax({
            url: '/Search/LoadMoreSearch',
            type: 'GET',
            data:
            {
                keyword: keyword,
                page: page
            },
    
            success: function (response) {
    
                if (response == null ||
                    response.trim() == "") {
                    button.hide();
                    return;
                }
    
                $("#SearchResultDiv").append(response);
                $("#SearchCurrentPage").val(page);
    
                button.text("Load more");
                button.css("pointer-events", "auto");
    
                // Total loaded items
                var loadedItems = $("#SearchResultDiv .search-list-group").length;

                // Hide button when completed
                if (loadedItems >= totalCount) {
                    button.hide();
                }
                else {
                    button.show();
                }
            },
            error: function (xhr) {
                button.text("Load more");
                button.css("pointer-events", "auto");
                console.log(xhr.responseText);
            }
        });
    
    });




    $(document).on("submit", "#frmSearchPage", function (e) {

        e.preventDefault();
    
        var keyword = ($(this).find("#search").val() || "").trim();
    
        if (keyword === "") {
            alert("Please enter search keyword.");
    
            $(this).find("#search").focus();
    
            return false;
        }
    
        window.location.href = "/search/" + encodeURIComponent(keyword);
    });

    
    // $(document).on("submit", "#frmSearchPage", function (e) {
    //     e.preventDefault();
    
    //     //var keyword = $("#search").val().trim();
    //     var keyword = ($(this).find("#searchInput").val() || "").trim();
    
    //     alert(keyword);
    //     if (keyword == "") {
    //         alert("Please enter search keyword.");
    //         $("#search").focus();
    //         return false;
    //     }
    
    //     window.location.href =
    //         "/search/" + encodeURIComponent(keyword);
    // });


  





});