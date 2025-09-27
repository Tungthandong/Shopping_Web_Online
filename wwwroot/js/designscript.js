const canvas = document.querySelector("canvas"),
    ctx = canvas.getContext("2d");

let isDrawing = false;
brushWidth = 5;

window.addEventListener("load", () => {
    //setting canvas width/height .. offsetwidth/height returns viewable width/height of an element
    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;
})

const startDraw = () => {
    isDrawing = true;
    ctx.beginPath();// creating new path to draw
    ctx.lineWidth = brushWidth; // passing brushSize as line width
}

const drawing = (e) => {
    if (!isDrawing) return; // if isDrawing is false return from here
    ctx.lineTo(e.offsetX, e.offsetY); //creating line according to the mouse pointer
    ctx.stroke();//drawing /filling line with color
}

canvas.addEventListener("mousedown", startDraw);
canvas.addEventListener("mousemove", drawing);
canvas.addEventListener("mouseup", () => isDrawing = false);
