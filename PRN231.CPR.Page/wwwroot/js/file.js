
const file = document.getElementById("click");
const container = document.querySelector(".import");
const btn = document.querySelector("label");
const fileName = document.querySelector(".file-name");
const deleteBtn = document.querySelector(".icon");

file.addEventListener("input", () => {
    if (file.files.length === 0) {
        console.log("no file selected");
    }
    else {
        console.log("file selected");
        container.classList.add("active");
        fileName.innerHTML = file.files.item(0).name;
    }
});
deleteBtn.addEventListener("click", () => {
    file.value = "";
    container.classList.remove("active");
})
