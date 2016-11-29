var neuralNetApp = angular.module("neuralNetApp", ['ngRoute']);

neuralNetApp.controller('NeuralNetController', function ($scope, $http) {
    $scope.tempData =
            {
                type: "tnse",
                data: [{
                    word: "Hello",
                    x: 428.0823572201431,
                    y: 380.4647276203807
                },
                      {
                          word: "Evil",
                          x: 400,
                          y: 300.4647276203807
                      },
                      {
                          word: "Surprise",
                          x: 300,
                          y: 380.4647276203807
                      },
                      {
                          word: "Funny",
                          x: 50,
                          y: 50
                      }]
            }


    //Display Graph
    $scope.display = function () {
        if ($scope.tempData.type == "tnse")
            displayTestNet($scope.tempData);
    }
});

function displayTestNet(temp) {
    $(".sidebar").show();
    $("#neural-net").empty();

    var width = 847,
        height = 665;

    var randomX = d3.random.normal(width / 2, 80),
        randomY = d3.random.normal(height / 2, 80);

    var data = d3.range(10).map(function () {
        return [
          randomX(),
          randomY()
        ];
    });

    console.log(data);
    console.log("Temp");
    console.log(temp);

    var x = d3.scale.linear()
        .domain([0, width])
        .range([0, width]);

    var y = d3.scale.linear()
        .domain([0, height])
        .range([height, 0]);

    var canvas = d3.select("section").append("canvas")
        .attr("width", width)
        .attr("height", height)
        .call(d3.behavior.zoom().x(x).y(y).scaleExtent([1, 100]).on("zoom", zoom))
      .node().getContext("2d");

    draw();

    function zoom() {
        canvas.clearRect(0, 0, width, height);
        draw();
    }

    function draw() {
        var i = -1, n = temp.data.length, d, cx, cy;
        canvas.beginPath();
        while (++i < n) {
            d = temp.data[i];
            cx = x(d.x);
            console.log("X Coor - " + cx);
            cy = y(d.y);
            console.log("Y Coor - " + cy);
            canvas.moveTo(cx, cy);
            canvas.arc(cx, cy, 2.5, 0, 2 * Math.PI);
            canvas.font = "10px Georgia";
            canvas.fillText(d.word, cx - 7, cy + 10);
        }
        canvas.fill();
    }
}
