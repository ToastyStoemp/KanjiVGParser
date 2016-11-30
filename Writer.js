var Vector2 = require('./Vector2.js');

﻿var Writer = function() {

    this.IsDebug = false;

    this.LastPosition = new Vector2(0,0);
    this.CurrentDirections = [];
    this.FinalPositions = [];
    this.FinalDirections = [];
    this.CurrentMouseBuffer = [];
    this.DistanceTollerance = 50.0;
    this.Resolution = 0.5;
    this.CurrentDirection = new Vector2(0,0);
    this.ShouldCaptureDirection = false;

    this.BeginStroke = function(x, y)
    {
      var pos = new Vector2(x,y);
        this.FinalPositions = [pos];
        this.CurrentDirections = [];
        this.LastPosition = pos;

        this.ShouldCaptureDirection = true;

        this.CurrentMouseBuffer = [pos];
    }

this.EndStroke = function(x, y, xml)
    {
      var pos = new Vector2(x,y);
      var lastpos =this.FinalPositions[this.FinalPositions.length - 1];
      if (lastpos.x != x && lastpos.y != y){
        //console.log(lastpos.distance(pos));
        if (lastpos.distance(pos) > 8 ){
          //console.log(x,y);
          this.FinalPositions.push(pos);
        }
      }

        var confirmDirections = [];
        for (var i = 1; i < this.FinalPositions.length; i++)
        {
            var checkDirection = new Vector2(this.FinalPositions[i].x - this.FinalPositions[i-1].x, this.FinalPositions[i].y - this.FinalPositions[i-1].y);
            //console.log(checkDirection, this.FinalPositions[i], this.FinalPositions[i-1]);
            var result = HalfRoundVec2(checkDirection.normalized()).simple();
            //console.log(result);
            if (confirmDirections.length == 0)
                confirmDirections.push(result);
            else if (confirmDirections[confirmDirections.length - 1].x != result.x && confirmDirections[confirmDirections.length - 1].y != result.y)
                {
                  //console.log(this.FinalPositions[i]);
                  confirmDirections.push(result);
                }
        }

        this.FinalDirections = confirmDirections;
        //console.log('directions');
        var resultingStroke = confirmDirections.length;
        for (stroke of confirmDirections) {
          resultingStroke +="("+stroke.x+", "+stroke.y+")";
        }
        return resultingStroke;

    }
this.DrawStroke = function(x, y)
    {
      var pos = new Vector2(x,y);
      var lastpos = this.FinalPositions[this.FinalPositions.length - 1];
      if (lastpos.x != x && lastpos.y != y){
        if (lastpos.distance(pos) > 10 ){
          this.FinalPositions.push(pos);
        }
      }
    }
  };

    function HalfRoundVec2(vec)
    {
        var x = HalfRound(vec.x);
        var y = HalfRound(vec.y) * -1;

        if (Math.abs(x) == 0.5 && Math.abs(y) == 1)
            y *= 0.5;

        if (Math.abs(y) == 0.5 && Math.abs(x) == 1)
            x *= 0.5;

        if (x == '-0') x = 0.0;
        if (y == '-0') y = 0.0;

        return new Vector2(x.toFixed(1), y.toFixed(1));
    }

    function HalfRound(value)
    {
        return Math.round(value * 2) / 2.0;
    }


var exports = module.exports = Writer;
