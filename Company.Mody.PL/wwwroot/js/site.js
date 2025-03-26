// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.addEventListener("DOMContentLoaded", function () {
    //let searchInput = document.getElementById("Keyword");

    //// Detect the page type based on the URL path
    //let pageType = window.location.pathname.includes("Employee") ? "Employee" : "Department";
    //console.log(pageType);
    //// Select the correct results div
    //let resultsDiv = document.getElementById(pageType === "Employee" ? "employeeResults" : "departmentResults");

    //if (!searchInput || !resultsDiv) {
    //    console.error("Search input or results container not found!");
    //    return;
    //}

    let searchInput = document.getElementById("Keyword");

    // Get the search type from the data attribute
    if (!searchInput) {
        console.error("Search input not found!");
        return;
    }

    let searchType = searchInput.getAttribute("data-search-type"); // Get dynamic search type
    let resultsDiv = document.getElementById(searchType.toLowerCase() + "Results"); // Find the right div

    if (!resultsDiv) {
        console.error(`Results container for ${searchType} not found!`);
        return;
    }

    searchInput.addEventListener("keyup", () => {
        let keyword = searchInput.value.trim();
        //if (keyword === "") {
        //    resultsDiv.innerHTML = ""; // Clear results if input is empty
        //    return;
        //}

        let xhr = new XMLHttpRequest();
        let url = `/${searchType}?keyword=${encodeURIComponent(keyword)}`;

        xhr.open("GET", url, true);
        xhr.setRequestHeader(`X-Requested-${searchType}Search`, "XMLHttpRequest");

        xhr.onreadystatechange = function () {
            if (this.readyState === 4) {
                if (this.status === 200) {
                    resultsDiv.innerHTML = this.responseText;
                } else {
                    console.error(`Error: ${this.status} while fetching ${searchType} data`);
                }
            }
        };

        xhr.send();
    });
});





//SearchInput.addEventListener("keyup", () => {

//    console.log("addEventListener 1");

//    // Creating XMLHttpRequest object 
//    let xhr = new XMLHttpRequest();
//    console.log("addEventListener 2");


//    // Ensure the API is running on the correct port
//    let url = `https://localhost:44392/Employee?keyword=${SearchInput.value}`;
//    console.log("addEventListener 3");

//    xhr.open("GET", url, true);

//    console.log("addEventListener 4");


//    // Execute function after request is successful 
//    xhr.onreadystatechange = function () {
//        if (this.readyState === 4 && this.status === 200) {
//            console.log("Done");
//        }
//    };
//    console.log("addEventListener 5");

//    // Sending request 
//    xhr.send();

//    console.log("addEventListener 6");

//});
