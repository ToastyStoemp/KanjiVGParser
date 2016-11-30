var Writer = require('./Writer.js');
var KanjiRenderer = function(xml){
this.xml = xml;
this.writer = new Writer();

this.linearCurve = function(p0, p1, w0, w1, steps)
{
		this.writer.DrawStroke(p0[0],p0[1]);
		for (var i = 0; i < steps-1; i++) {
			var t = i / steps;
			var x = (1-t)*p0[0] + t*p1[0];
			var y = (1-t)*p0[1] + t*p1[1];
			this.writer.DrawStroke(x,y);
		}
		this.writer.DrawStroke(p1[0],p1[1]);
}

this.bezierCurve = function(p0, p1, p2, w0, w1, steps)
{
	this.writer.DrawStroke(p0[0],p0[1]);
	for (var i = 0; i < steps-1; i++) {
		var t = i / steps;
		var x = (1-t)*(1-t)*p0[0] + 2*(1-t)*t*p1[0] + t*t*p2[0];
		var y = (1-t)*(1-t)*p0[1] + 2*(1-t)*t*p1[1] + t*t*p2[1];
		this.writer.DrawStroke(x,y);
	}
	this.writer.DrawStroke(p2[0],p2[1]);
}

this.quadraticCurve = function( p0, p1, p2, p3, w0, w1, steps)
{
	this.writer.DrawStroke(p0[0],p0[1]);
	for (var i = 0; i < steps-1; i++) {
		var t = i / steps;
		var x = (1-t)*(1-t)*(1-t)*p0[0] + 3*(1-t)*(1-t)*t*p1[0] + 3*(1-t)*t*t*p2[0] + t*t*t*p3[0];
		var y = (1-t)*(1-t)*(1-t)*p0[1] + 3*(1-t)*(1-t)*t*p1[1] + 3*(1-t)*t*t*p2[1] + t*t*t*p3[1];
		this.writer.DrawStroke(x,y);
	}
	this.writer.DrawStroke(p3[0],p3[1]);
}

this.renderPath = function(path, steps, kanji, callback)
{
	this.writer = new Writer();

	var firstCmd = true;

	var upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	var lower = "abcdefghijklmnopqrstuvwxyz";
	var last = null;
	var strokes = [];

	while(path.length) {
		var cmd = path.charAt(0);
		var i = 1;
		while(i < path.length) {
			var c = path.charAt(i);
			if((c>='A'&&c<='Z')||(c>='a'&&c<='z'))
				break;
			i++;
		}
		var data = path.substring(1, i);
		path = path.slice(i);

		var data = data.replace(/(\d)-(\d)/g, "$1,-$2").split(",");
		for(var i=0;i<data.length;i++)
			data[i] = parseFloat(data[i]);

		// convert from relative to absolute positions as needed.
		if(lower.indexOf(cmd) != -1)
		{
			for(var i=0;i<data.length;i++){
				if (last == null) console.log(kanji);
				data[i] += last[i%2];
			}
			cmd = upper.charAt(lower.indexOf(cmd));
		}

		if(cmd == "L")
			this.linearCurve(last, data, 9, 2, steps);
		if(cmd == "S")
			this.bezierCurve(last, [data[0],data[1]], [data[2], data[3]], 9, 1, steps);
		if(cmd == "C")
			this.quadraticCurve(last, [data[0],data[1]], [data[2], data[3]], [data[4],data[5]], 9, 1, steps);

		if(cmd == "M") {
			if(firstCmd) this.writer.BeginStroke(data[0], data[1])
			last = data;
		} else if(cmd == "L") {
			last = data;
		} else if(cmd == "S") {
			last = [data[2],data[3]];
		} else if(cmd == "C") {
			last = [data[4],data[5]];
		}

		if(firstCmd) firstCmd = false;
	}
	return this.writer.EndStroke(last[0], last[1], this.xml);
};
};
var exports = module.exports = KanjiRenderer;
