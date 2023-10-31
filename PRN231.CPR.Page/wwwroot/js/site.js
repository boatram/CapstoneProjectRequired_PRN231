// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
/**
    * User Profile
    */
const input = document.getElementById('inputUpload');
const image = document.getElementById('user-photo');

input.addEventListener('change', (e) => {
    const reader = new FileReader();

    if (e.target.files.length) {
        const src = URL.createObjectURL(e.target.files[0]);
        image.setAttribute('src', src)
    }
    reader.addEventListener("load", () => {
        localStorage.setItem("recent-image", reader.result);
    });
    reader.readAsDataURL(e.target.files[0]);
});

