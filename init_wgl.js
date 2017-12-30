var	context;
var	shader_program;
var	time;
var	vertex;
var	fragment;
var tempDir = "";
var nbrShaders = 1;

function	init(canvas)
{
	context = canvas.getContext("webgl");
	context.viewportWidth = canvas.width;
	context.viewportHeight = canvas.height;
	if (!context)
		alert("Could not initialize WEBGL context");
}

function loadShader(shaderName, cb) {
	var file = new XMLHttpRequest();
	file.open("GET", tempDir + shaderName, true);
	file.setRequestHeader('Content-Type', 'application/text');
	file.setRequestHeader('Access-Control-Allow-Origin', '*');
	file.onreadystatechange = function () {
		if (file.readyState == 4 && file.status == 200) {
			cb(file.responseText);
		}
	};
	file.send();
}

function	loadShaders(button) {
	if (shader_program)
	{
		console.log("bonjour");
		context.detachShader(shader_program, fragment);
		context.detachShader(shader_program, vertex);
		context.deleteShader(shader_program, fragment);
		context.deleteShader(shader_program, vertex);
		context.deleteProgram(shader_program);
	}
	shader_program = context.createProgram();
	console.log(button.id);
	var shaders = [
		{"fragment" : "shader"+button.id+".fs", "vertex" : "shader0.vs"}
	];
	var count = 0;
	for (var s of shaders) {
		console.log("loading s : " + s);
		console.log("" + s.fragment);
		loadShader(s.fragment, function (res) {
			fragment = context.createShader(context.FRAGMENT_SHADER);
			context.shaderSource(fragment, res);
			context.compileShader(fragment);
			context.attachShader(shader_program, fragment);
			if (!fragment)
				console.log("fragment is NULL");
			if (!context.getShaderParameter(fragment, context.COMPILE_STATUS))
				console.log(context.getShaderInfoLog(fragment));
			loadShader(s.vertex, function (res) {
				vertex = context.createShader(context.VERTEX_SHADER);
				context.shaderSource(vertex, res);
				context.compileShader(vertex);
				context.attachShader(shader_program, vertex);
				if (!vertex)
					console.log("shader is NULL");
				if (!context.getShaderParameter(vertex, context.COMPILE_STATUS))
					console.log(context.getShaderInfoLog(vertex));
				count++;
				if (count == nbrShaders)
					initShaders();
			});
		});
	}
}

function initShaders() {
	context.linkProgram(shader_program);

	if (!context.getProgramParameter(shader_program, context.LINK_STATUS))
		alert("Could not initialize shaders");

	shader_program.vertexPositionAttribute = context.getAttribLocation(shader_program, "aVertexPosition");
	context.enableVertexAttribArray(shader_program.vertexPositionAttribute);

	shader_program.vertexColorAttribute = context.getAttribLocation(shader_program, "aVertexColor");
	context.enableVertexAttribArray(shader_program.vertexColorAttribute);

	var	pos_buff = context.createBuffer();
	context.bindBuffer(context.ARRAY_BUFFER, pos_buff);
	var	pos = [
				1., 1. ,
				-1., 1.,
				1., -1.,
				-1., -1.
				];
	context.bufferData(context.ARRAY_BUFFER, new Float32Array(pos), context.STATIC_DRAW);

	pos_buff.itemSize = 2;
	pos_buff.numItems = 4;

	var	col_buff = context.createBuffer();
	context.bindBuffer(context.ARRAY_BUFFER, col_buff);
	var	col = [
				0.5, 0.5, 1., 1.0,
				0.5, 0.5, 1., 1.0,
				0.5, 0.5, 1., 1.0,
				0.5, 0.5, 1., 1.0
				];
	context.bufferData(context.ARRAY_BUFFER, new Float32Array(col), context.STATIC_DRAW);
	col_buff.itemSize = 3;
	col_buff.numItems = 4;

	time = context.getUniformLocation(shader_program, "time");
	context.viewport(0, 0, context.viewportWidth, context.viewportHeight);
	context.clearColor(1,0,0,1);
	context.clear(context.COLOR_BUFFER_BIT);

	context.useProgram(shader_program);

	context.bindBuffer(context.ARRAY_BUFFER, col_buff);
	context.vertexAttribPointer(shader_program.vertexColorAttribute, col_buff.itemSize, context.FLOAT, false, 0, 0);
	context.bindBuffer(context.ARRAY_BUFFER, pos_buff);
	context.vertexAttribPointer(shader_program.vertexPositionAttribute, pos_buff.itemSize, context.FLOAT, false, 0, 0);

	context.drawArrays(context.TRIANGLES, 0, 3);
	context.drawArrays(context.TRIANGLES, 1, 3);
	main_loop();
}

var n = 0.0;
var d = new Date();

function	main_loop()
{
	n += d.getMilliseconds();
	context.uniform1f(time, n);
	context.drawArrays(context.TRIANGLES, 0, 3);
	context.drawArrays(context.TRIANGLES, 1, 3);
	requestAnimationFrame(main_loop);
}

function clicked(button) {

	loadShaders(button);
	}

function	start_wgl()
{
	var	canvas = document.getElementById("window_wgl");
	init(canvas);
	loadShaders(document.getElementById("0"));
}