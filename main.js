var fs = require("fs");
var httpreq = require("httpreq");
var builder = require('xml-writer');
var xml = new builder(true);
var Kanjirender = require('./kanjirender.js');
var render = new Kanjirender(xml);
var lessonData = require('./lessonData.json');

xml.startDocument();
xml.startElement('KanjiLibrary');

var testFolder = './Data/';

var GenkiLevels = true;
var OnlineMeaning = false; //This feature is not fully working

process.on('exit', (code) => {
  xml.endElement();
  xml.endDocument();
  console.log(xml.output);
});

var files = fs.readdirSync(testFolder);
var allFiles = [];
var kanjiID = 0;
for (file of files) {
    parseFile(testFolder + file);
}


function parseFile(path, callback) {
  var resultStrokes = "";
  var level = "";
  var render = new Kanjirender(xml);
  fs.readFile(path, 'utf-8', function(err, data){
    if (err) return console.log(err);
    var el = / kvg:element="+[^"]*/gm
    var kanji = el.exec(data);
    if (kanji == null) return null;// return console.log('ERROR' + path);
    if (data) kanji = kanji[0].substr(14);
    else console.log(data);

    if (GenkiLevels) {
      var found = false;
      for (lesson in lessonData) {
          if (lessonData[lesson].indexOf(kanji) != -1) {
            found = true;
            level = lesson;
            break;
          }
      }
      if (!found) return;
    }


    var url = "http://nihongo.monash.edu/cgi-bin/wwwjdic?1ZKU" + toUnicode(kanji);
    var meanings = [];
    httpreq.get(url, function(err, result){
      if (OnlineMeaning) {
      if (err) var x;//console.log(err + "-Can't retrieve meaning");
      else {
        var selector = /<pre>+[^<]*/gm
        var verbal = selector.exec(result.body)[0].substr(5);
        verbal = verbal.split(' ');
        verbal.splice(0,34);
        verbal.splice(verbal.length-1);

        var meaningdata = verbal.splice(3 , verbal.length-1);
        for (meaning of meaningdata) {
          if (meaning[0] == '{')
            meanings.push(meaning.replace(/{|}/g,''));
        }
      }
    }

      xml.startElement('Kanji');
      xml.writeAttribute('ID', kanjiID++);
      xml.writeAttribute('sign', kanji);
      if (GenkiLevels && level != "") xml.writeAttribute('level', level);
      if (meanings) xml.writeAttribute('meaning', meanings.join(' '));

      var re = / d="+[^"]*/gm;
      var m;

      do {
        m = re.exec(data);
        if (m) {
          var path = m[0].substr(4);
          resultStrokes += render.renderPath(path, 3, kanji);
        }
      } while (m);
      xml.writeAttribute('strokes', resultStrokes);
      xml.endElement();
    });
  });
}

function NextKanji() {
  var current = allFiles.shift();
}

function toUnicode(theString) {
  for (var i=0; i < theString.length; i++) {
    var theUnicode = theString.charCodeAt(i).toString(16).toUpperCase();
    while (theUnicode.length < 4) {
      theUnicode = '0' + theUnicode;
    }
    return theUnicode;
  }
}
