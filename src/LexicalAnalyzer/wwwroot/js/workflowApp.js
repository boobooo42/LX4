var workflowApp = angular.module("workflowApp", ['ngRoute']);
workflowApp.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.

    when('/srapper', {
        templateUrl: 'srapper.htm',
        controller: 'ScrapperController'
    }).

    when('/train', {
        templateUrl: 'train.htm',
        controller: 'TrainController'
    }).

    otherwise({
        templateUrl: 'CreateWorkflow.cshtml',
        controller: 'TrainController'
    });
}]);

workflowApp.controller('ScrapperController', function ($scope) {
    $scope.message = "ScrapperController";
});

workflowApp.controller('TrainController', function ($scope) {
    $scope.message = "TrainController";
});

$(document).ready(function () {
    var transition = 0;
    var circleTransition = 0;
    var count = 0;
    $("#nextStep").click(function () {
        if (transition < 70) {
            transition += 114;
        }
        else {
            transition += 64;
        }
        if (transition < 540) {
            $(".nav-active-inner").css("transform", "translate(0px," + transition + "px)");
            $(".nav-active-outer").css("transform", "translate(0px," + (transition + 30) + "px)");
        }
        if (count < 7) {
            circleTransition += 62.5;
            $(".circle-outer").css("transform", "translate(0px," + (circleTransition) + "px)");
            $(".circle-inner").css("transform", "translate(0px," + (circleTransition) + "px)");
            count++;
        }

    });

    $("#prevStep").click(function () {
        if (count > 0) {
            circleTransition -= 62.5;
            $(".circle-outer").css("transform", "translate(0px," + (circleTransition) + "px)");
            $(".circle-inner").css("transform", "translate(0px," + (circleTransition) + "px)");
            count--;
        }
    });
});