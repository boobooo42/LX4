var statusApp = angular.module("statusApp", ['ngRoute']);

statusApp.controller("status", function ($scope, $http, $interval) {
    $scope.scrappers;

    //Gets all scrappers and neural nets. 
    $scope.init = function () {
        $http({
            method: 'get',
            url: '/api/scraper',
        })
           .success(function (response) {
               $scope.scrappers = response;
               $scope.setProgress();
               console.log($scope.scrappers);

               //console.log($scope.literatureList);
           })
           .error(function () {
               console.log("Failed to get status.")
           });
    }


    //Initialize the scope
    $scope.init();

    //Set value of each scrapper progress
    $scope.setProgress = function () {
        for (var i = 0; i < $scope.scrappers.length; i++) {
            $scope.scrappers[i].Progress *= 100;
            $scope.scrappers[i].Progress = Math.floor($scope.scrappers[i].Progress);
        }
    }

    $interval($scope.init, 2000);
});