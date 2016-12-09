var statusApp = angular.module("statusApp", ['ngRoute']);

statusApp.controller("status", function ($scope, $http, $interval) {
    $scope.scrappers;
    $scope.learningModels;

    //Gets all scrappers and learning models. 
    $scope.init = function () {
        $http({
            method: 'get',
            url: UrlContent('/api/scraper/'),
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
            url: UrlContent('/api/learningmodel'),
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
                if ($scope.scrappers[i].Progress <= 1) {
                    $scope.scrappers[i].Progress *= 100;
                    $scope.scrappers[i].Progress = Math.floor($scope.scrappers[i].Progress);
                }
            }

        if ($scope.learningModels != undefined)
            for (var i = 0; i < $scope.learningModels.length; i++) {
                if ($scope.learningModels[i].Progress <= 1) {
                    $scope.learningModels[i].Progress *= 100;
                    $scope.learningModels[i].Progress = Math.floor($scope.learningModels[i].Progress);
                }
            }
    }

    //Starts or stop given scrapper.
    $scope.toggleScrapper = function (scrapper) {

        if (scrapper.Status == "init") {
            var route = UrlContent("/api/scraper/" + scrapper.Guid + "/start");
            $http({
                method: 'post',
                url: route,
            }).success(function (response) {
                console.log("Successfully paused scrapper.");
            })
               .error(function () {
                   console.log("Failed to pause scrapper")
               });
        }else if (scrapper.Status != "paused") {
            var route = "/api/scraper/" + scrapper.Guid + "/pause";

            $http({
                method: 'post',
                url: route,
            }).success(function (response) {
                console.log("Successfully paused scrapper.");
            })
               .error(function () {
                   console.log("Failed to pause scrapper")
               });

        } else {
            var route = UrlContent("/api/scraper/" + scrapper.Guid + "/start");
            $http({
                method: 'post',
                url: route,
            }).success(function (response) {
                console.log("Successfully paused scrapper.");
            })
               .error(function () {
                   console.log("Failed to pause scrapper")
               });
        }
    }

    //Starts or stop given learning model.
    $scope.togglelearningModel = function (lm) {
        var route = UrlContent("/api/learningmodel/" + lm.Guid);
        if (lm.Status != "paused") {
            lm.Status = "pause";
            $http.put(route, lm)
                .success(function (data, status, headers, config) {
                    console.log("Successfully paused scrapper");
                })
                .error(function (data, status, header, config) {
                    console.log("Failed to pause scrapper");
                });
        } else {
            lm.Status = "start";
            $http.put(route, lm)
               .success(function (data, status, headers, config) {
                   console.log("Successfully started scrapper");
               })
               .error(function (data, status, header, config) {
                   console.log("Failed to start scrapper");
               });
        }
    }

    $interval($scope.init, 2000);
});