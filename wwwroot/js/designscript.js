

const canvas = document.querySelector("#canvas"),
	backgroundCanvas = document.querySelector("#backgroundCanvas"),
	toolBtns = document.querySelectorAll(".tool"),
	fillColor = document.querySelector("#fill-color"),
	sizeSlider = document.querySelector("#size-slider"),
	colorBtns = document.querySelectorAll(".colors .option"),
	colorPicker = document.querySelector("#color-picker"),
	clearCanvas = document.querySelector(".clear-canvas"),
	saveImg = document.querySelector("#send-image"),
	ctx = canvas.getContext("2d"),
	backgroundCtx = backgroundCanvas.getContext("2d");


var background = new Image();
background.src = "images/design-sample/mau-ao-1.jfif";


const imgSlider = document.querySelector("#img-slider");
var	imgOut = document.getElementById('img-out');

let isDrawingImg = false;
// global var

//gui qua controller
saveImg.addEventListener("click", () => {
	const fullname = document.getElementById("FullName").value;
	const phone = document.getElementById("Phone").value;
	const email = document.getElementById("Email").value;
	const quantity = document.getElementById("Quantity").value;
	const note = document.getElementById("Note").value;
	const files = document.getElementById("File").files;

	backgroundCtx.drawImage(canvas, 0, 0);
	ctx.clearRect(0, 0, canvas.width, canvas.height);
	backgroundCanvas.toBlob((blob) => {
		const formData = new FormData();
		formData.append("canvasFile", blob, "canvas-file.jpg");
		for (let i = 0; i < files.length; i++) {
			formData.append("userFiles", files[i]);
		}
		formData.append("fullname", fullname);
		formData.append("phone", phone);
		formData.append("email", email);
		formData.append("quantity", quantity);
		formData.append("note", note);

		fetch("/Design/SaveImage", {
			method: "POST",
			body: formData
		})
			.then(res => res.json()) // return this!
			.then(data => {
				alert("success");
				window.location.href = "/Design";
			})
			.catch(err => console.error("Error:", err));
	});
});



// Start dragging if mouse is on image
imgOut.addEventListener("mousedown", (e) => {
	snapshot = ctx.getImageData(0, 0, canvas.width, canvas.height);
	isDrawingImg = true;
});


const drawingImg = (e) => {
	if (!isDrawingImg) return; // if isDrawing is false return from here
	ctx.putImageData(snapshot, 0, 0); //adding copied canvas data on to this canvas
	ctx.drawImage(imgOut, e.offsetX - imgOut.width / 2, e.offsetY - imgOut.height / 2, imgOut.width, imgOut.height);
}

// img input resize


imgSlider.addEventListener("change", () => {
	imgOut.width = imgSlider.value;
});
//

// dropdown start
const dropdown = document.querySelector(".image-dropdown");
const selectedImage = document.getElementById("selected-image");

dropdown.querySelector(".dropdown-btn").addEventListener("click", () => {
	dropdown.classList.toggle("open");
});

dropdown.querySelectorAll(".dropdown-content img").forEach(img => {
	img.addEventListener("click", (e) => {
		selectedImage.src = e.target.src;
		dropdown.classList.remove("open");
		background.src = selectedImage.src;
		setCanvasBackground();
	});
});

window.addEventListener("click", (e) => {
	if (!dropdown.contains(e.target)) dropdown.classList.remove("open");
});

// dropdown end


let prevMouseX, prevMouseY, snapshot,
isDrawing = false;
selectedTool = "brush",
	brushWidth = 5;
selectedColor = "#000";

const setCanvasBackground = () => {
	backgroundCtx.drawImage(background, (canvas.width - canvas.height) / 2, 0, canvas.height, canvas.height);
	//ctx.fillStyle = "#fff";// draw the whole canvas white
	//ctx.fillRect(0, 0, canvas.width, canvas.height);
	ctx.fillStyle = selectedColor;//setting fillstyle back to the selected color
	snapshot = ctx.getImageData(0, 0, canvas.width, canvas.height);
}

window.addEventListener("load", () => {
	//setting canvas width/height .. offsetwidth/height returns viewable width/height of an element
	backgroundCanvas.width = backgroundCanvas.offsetWidth;
	backgroundCanvas.height = backgroundCanvas.offsetHeight;
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
	if (selectedTool === "brush") {
		ctx.globalCompositeOperation = 'source-over';
		ctx.strokeStyle = selectedColor;
		ctx.lineTo(e.offsetX, e.offsetY); //creating line according to the mouse pointer
		ctx.stroke();//drawing /filling line with color
	} else if (selectedTool === "eraser") {
		ctx.globalCompositeOperation = 'destination-out'
		ctx.lineTo(e.offsetX, e.offsetY);
		ctx.stroke();
	} else if (selectedTool === "rectangle") {
		ctx.globalCompositeOperation = 'source-over';
		drawRect(e);
	} else if (selectedTool === "circle") {
		ctx.globalCompositeOperation = 'source-over';
		drawCircle(e);
	} else if (selectedTool === "triangle") {
		ctx.globalCompositeOperation = 'source-over';
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
	snapshot = ctx.getImageData(0, 0, canvas.width, canvas.height);
});


canvas.addEventListener("mousedown", (e) => {
	if (isDrawingImg) {
		isDrawingImg = false;
	}
	startDraw(e);
});
canvas.addEventListener("mouseleave", () => {
	isDrawing = false;
});
canvas.addEventListener("mousemove", (e) => {
	if (isDrawingImg) {
		ctx.globalCompositeOperation = 'source-over';
		drawingImg(e);
		return;
	}
	drawing(e);
});
canvas.addEventListener("mouseup", () => {
	isDrawing = false;
});
