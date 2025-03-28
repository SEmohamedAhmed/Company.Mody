document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");
    const checkbox = document.getElementById("terms");

    form.addEventListener("submit", function (event) {
        if (!checkbox.checked) {
            event.preventDefault(); // Stop form submission
            alert("You must agree to the Terms of Service before signing up.");
        }
    });
});
