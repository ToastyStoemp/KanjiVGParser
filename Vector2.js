var Vector2 = function(x, y) {
  var self = this;

  this.x = x;
  this.y = y;

  this.normalized = function(){
    var length = this.length();
    var res = new Vector2(this.x / length, this.y / length);
    return res;
  }

  this.length = function(){
    var length = Math.sqrt(self.x * self.x + self.y * self.y);
    return length;
  }

  this.substract = function(vec2){
    var res = new Vector2(self.x - vec2.x, self.y - vec2.y);
    return res;
  }

  this.distance = function (vec2) {
    var midVec = new Vector2(vec2.x - self.x, vec2.y - self.y);
    return midVec.length();
  };

  this.simple = function() {
    return {x: self.x, y: self.y};
  }
}

Vector2.prototype.zero = function () {
  return new Vector2(0,0);
}

var exports = module.exports = Vector2;
