var corpusApp = angular.module("corpusApp", ['ngRoute']);
corpusApp.controller("corpus", function ($scope, $http, $interval) {
    $scope.coprusList;
    $scope.corpusContent;
    $scope.corpus;
    $scope.newContent = {
        id: -1,
        name: "string",
        type: "string",
        downloadURL: "Manual Insert",
    };


    $scope.contentName;
    $scope.contentType;

    //Gets all corpora
    $scope.getCorpusList = function () {
        $http({
            method: 'get',
            url: '/api/corpus',
        })
           .success(function (response) {
               $scope.corpusList = response;
               console.log($scope.coprusList);
           })
           .error(function () {
               console.log("Failed to get corpus list.")
           });
    }

    //Gets all scrappers and neural nets. 
    $scope.getCorpusContent = function (corpusId) {
        var route = "/api/CorpusContent/list/" + corpusId;

        $http({
            method: 'get',
            url: route
            })
           .success(function (response) {
               $scope.corpusContent = response;
               console.log($scope.corpusContent);
           })
           .error(function () {
               console.log("Failed to get corpus content.")
           });
    }

    $scope.display = function (c) {
        for (var i = 0; i < $scope.corpusList.length; i++) {
            if ($scope.corpusList[i].name == c) {
                $scope.corpus = $scope.corpusList[i];
                $scope.corpusContent = $scope.getCorpusContent($scope.corpusList[i].id);
                break;
            }
        }

        console.log($scope.corpus);
    }

    $scope.deleteContent = function (corpusId) {
        alert("Content will be deleted: " + corpusId);
        $http({
            method: 'get',
            url: '/api/content/delete',
            params: { id: corpusId }
        })
           .success(function (response) {
               console.log(response);
           })
           .error(function () {
               console.log("Failed to get corpus content.")
           });
    }

    $scope.createContent = function () {
        alert($scope.newContent);
        var conn = $http.post('/api/content/add', $scope.newContent,
        {
            headers: { 'Content-Type': 'application/json' }
        })
        .success(function () {
            alert("good");
        })
        .error(function () {
            alert("bad");
        });
    }

    //Initialize the scope
    $scope.getCorpusList();
});