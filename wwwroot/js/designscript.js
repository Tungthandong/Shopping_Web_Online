

const canvas = document.querySelector("canvas"),
	toolBtns = document.querySelectorAll(".tool"),
	fillColor = document.querySelector("#fill-color"),
	sizeSlider = document.querySelector("#size-slider"),
	colorBtns = document.querySelectorAll(".colors .option"),
	colorPicker = document.querySelector("#color-picker"),
	clearCanvas = document.querySelector(".clear-canvas"),
	saveImg = document.querySelector(".save-canvas"),
	ctx = canvas.getContext("2d");

// global var

let prevMouseX, prevMouseY, snapshot,
isDrawing = false;
selectedTool = "brush",
	brushWidth = 5;
selectedColor = "#000";

const setCanvasBackground = () => {
	ctx.fillStyle = "#fff";// draw the whole canvas white
	ctx.fillRect(0, 0, canvas.width, canvas.height);
	ctx.fillStyle = selectedColor;//setting fillstyle back to the selected color
}

window.addEventListener("load", () => {
	//setting canvas width/height .. offsetwidth/height returns viewable width/height of an element

	canvas.width = canvas.offsetWidth;
	canvas.height = canvas.offsetHeight;
	setCanvasBackground();
})

const drawRect = (e) => {
	// if fillColor isnt checked draw a rect with boder else draw rect with background
	if (!fillColor.checked) {
		// creating rect according to the mouse pointer
		return ctx.strokeRect(e.offsetX, e.offsetY, prevMouseX - e.offsetX, prevMouseY - e.offsetY);
	}
	ctx.fillRect(e.offsetX, e.offsetY, prevMouseX - e.offsetX, prevMouseY - e.offsetY);
}

const drawCircle = (e) => {
	ctx.beginPath();// creating new path to draw circle
	//getting radius for circle according to the mouse pointer
	let radius = Math.sqrt(Math.pow((prevMouseX - e.offsetX), 2) + Math.pow((prevMouseY - e.offsetY), 2));
	ctx.arc(prevMouseX, prevMouseY, radius, 0, 2 * Math.PI);
	ctx.stroke();
	fillColor.checked ? ctx.fill() : ctx.stroke();
}

const drawTriangle = (e) => {
	ctx.beginPath();
	ctx.moveTo(prevMouseX, prevMouseY);
	ctx.lineTo(e.offsetX, e.offsetY);
	ctx.lineTo(prevMouseX * 2 - e.offsetX, e.offsetY);
	ctx.closePath();
	fillColor.checked ? ctx.fill() : ctx.stroke();
}

const startDraw = (e) => {
	isDrawing = true;
	prevMouseX = e.offsetX;// passing cur mouseX pos as prevMouseX value
	prevMouseY = e.offsetY;// passing cur mouseY pos as prevMouseY value
	ctx.beginPath();// creating new path to draw
	ctx.lineWidth = brushWidth; // passing brushSize as line width
	ctx.strokeStyle = selectedColor; // passing selected Color as stroke style
	ctx.fillStyle = selectedColor; //  passing selected Color as fill style
	snapshot = ctx.getImageData(0, 0, canvas.width, canvas.height);
}

const drawing = (e) => {
	if (!isDrawing) return; // if isDrawing is false return from here
	ctx.putImageData(snapshot, 0, 0); //adding copied canvas data on to this canvas
	if (selectedTool === "brush" || selectedTool === "eraser") {
		//if selected tool is eraser then set strokeStyle to white
		//to paint white color on to the existing canvas content else set the mouse pointer
		ctx.strokeStyle = selectedTool === "eraser" ? "#fff" : selectedColor;
		ctx.lineTo(e.offsetX, e.offsetY); //creating line according to the mouse pointer
		ctx.stroke();//drawing /filling line with color
	} else if (selectedTool === "rectangle") {
		drawRect(e);
	} else if (selectedTool === "circle") {
		drawCircle(e);
	} else {
		drawTriangle(e);
	}
}

toolBtns.forEach(btn => {
	btn.addEventListener("click", () => { // adding click event to all tool option
		document.querySelector(".options .active").classList.remove("active");
		btn.classList.add("active");
		selectedTool = btn.id;
		console.log(selectedTool);
	});
})

sizeSlider.addEventListener("change", () => brushWidth = sizeSlider.value);

colorBtns.forEach(btn => {
	btn.addEventListener("click", () => {// adding click event to all color button
		document.querySelector(".options .selected").classList.remove("selected");
		btn.classList.add("selected");
		//passing selected btn background color as selected color value
		selectedColor = window.getComputedStyle(btn).getPropertyValue("background-color");

	});
});

colorPicker.addEventListener("change", () => {
	colorPicker.parentElement.style.background = colorPicker.value;
	colorPicker.parentElement.click();
});

clearCanvas.addEventListener("click", () => {
	ctx.clearRect(0, 0, canvas.width, canvas.height);
	setCanvasBackground();
});

saveImg.addEventListener("click", () => {
	const link = document.createElement("a");
	link.download = `${Date.now()}.jpg`;
	link.href = canvas.toDataURL();
	link.click();
});

canvas.addEventListener("mousedown", startDraw);
canvas.addEventListener("mousemove", drawing);
canvas.addEventListener("mouseup", () => isDrawing = false);