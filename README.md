# KanjiVGParser
A custom parser to convert KanjiVG (SVG) into a xml with directional data and more

# How To use
- Kanji => Database parser
To execute the Kanji's to xml database you need to install Node.js
Open the Commandprompt in the root folder of this project and run
```
$ npm install
$ node main.js out >> kanji.xml
```

That will save the database to kanji.xml

- Unity writer
Import the scripts manually, or use the included unityPackage
Included is and example 'game' file, that gives an example of how you can use the writer system.
Preferably place the script on a camera object.

Not all parameters are easily changable, -> Future updates

#TODO
-fix the meanings for the kanji
-implement writing to database inside the node project
-support regional drawing ( set boundaries )
-add more events
-update the 'almost' , and 'poor' correction algorithms
-add stroke length checker and position checker
-update the writer to be more acurate ( no corner strokes )
-more...
