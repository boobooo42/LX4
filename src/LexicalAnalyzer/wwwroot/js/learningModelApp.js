var neuralNetApp = angular.module("learningModelApp", ['ngRoute']);

neuralNetApp.controller('learningModelController', function ($scope, $http) {
    $scope.learningModels;
    $scope.learningModelResult;
    $scope.learningModelTypes;
    $scope.focusModel;
    $scope.focusModelType;
    $scope.focusElement;

    //Type of Learning models ajax call to get secription and name. 
    $scope.getLearningModelTypes = function () {
        $http({
            method: 'get',
            url: UrlContent('/api/learningmodel/types'),
        }).success(function (response) {
            $scope.learningModelTypes = response;
            console.log($scope.learningModelTypes);
        })
           .error(function () {
               console.log("Failed to get learning models.")
           });
    }

    //GET /api/learningmodel (Get all learning models currently instantiated).
    $scope.getLearningModels = function () {
        $http({
            method: 'get',
            url: UrlContent('/api/learningmodel'),
        }).success(function (response) {
            $scope.learningModels = response;
            console.log($scope.learningModels);
        })
           .error(function () {
               console.log("Failed to get learning models.")
           });
    }

    //Get Specific learning model [PARAM: guid]
    $scope.getLearningModelResult = function (learningModel) {
        $scope.focusModel = learningModel;
        var route = UrlContent("/api/learningmodel/" + learningModel.Guid + "/result");
        $http({
            method: 'get',
            url: route,
        }).success(function (response) {
            console.log("Successfully retrieved learning model result.")
            $scope.learningModelResult = response;
            $scope.display();
        })
           .error(function () {
               console.log("Failed to get learning model result.")
           });
    }

    $scope.tnseData =
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

    //Update focus element.
    $scope.updateFocus = function (d) {
        $scope.focusElement = d;
    }

    //Display Graph
    $scope.display = function () {
        
        //Display information about leaning model type.
        for (var i = 0; i < $scope.learningModelTypes.length; i++) {
            if ($scope.learningModelTypes[i].type == $scope.focusModel.Type) {
                $scope.focusModelType = $scope.learningModelTypes[i];
                console.log("Found learning model type.");
            }
        }

        console.log($scope.learningModelResult);

        if ($scope.learningModelResult.type == "TNSE Demo") {
            console.log($scope.learningModelResult);
            $scope.focusModel = $scope.tnseData;
            CreateTNSEPlot($scope.tempData);
            $("#elementSelector").hide();
        }
        else if ($scope.learningModelResult.type == "LexicalAnalyzer.Models.ZipfResult") {
            console.log($scope.learningModelResult);
            CreateZipfLawPlot($scope.learningModelResult);
            $("#elementSelector").show();
        } else {
            $(".sidebar").hide();
            $("#neural-net").empty();
        }
    }

    //Initialize the scope.
    $scope.getLearningModels();
    $scope.getLearningModelTypes();
});

function displayData(d) {
    var scope = angular.element(document.getElementById("maincontroller")).scope();
    scope.$apply(function () {
        scope.updateFocus(d);
    });
}

function CreateTNSEPlot(temp) {
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
