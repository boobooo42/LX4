var statusApp = angular.module("statusApp", ['ngRoute']);

statusApp.controller("status", function ($scope, $http, $interval) {
    $scope.scrappers;
    $scope.learningModels;

    //Gets all scrappers and learning models. 
    $scope.init = function () {
        $http({
            method: 'get',
            url: '/api/scraper',
        })
           .success(function (response) {
               $scope.scrappers = response;
               $scope.setProgress();
               console.log("Scrappers");
               console.log($scope.scrappers);
           })
           .error(function () {
               console.log("Failed to get status of scrappers.")
           });

        $http({
            method: 'get',
            url: '/api/learningmodel',
        })
           .success(function (response) {
               $scope.learningModels = response;
               $scope.setProgress();
               console.log("Learning Models");
               console.log($scope.learningModels);
           })
           .error(function () {
               console.log("Failed to get status of learning models.")
           });
    }


    //Initialize the scope
    $scope.init();

    //Set value of each scrapper progress
    $scope.setProgress = function () {

        if ($scope.scrappers != undefined)
        for (var i = 0; i < $scope.scrappers.length; i++) {
            $scope.scrappers[i].Progress *= 100;
            $scope.scrappers[i].Progress = Math.floor($scope.scrappers[i].Progress);
        }

        if ($scope.learningModels != undefined)
        for (var i = 0; i < $scope.learningModels.length; i++) {
            $scope.learningModels[i].Progress *= 100;
            $scope.learningModels[i].Progress = Math.floor($scope.learningModels[i].Progress);
        }
    }

    $interval($scope.init, 2000);
});