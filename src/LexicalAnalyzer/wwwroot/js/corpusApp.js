var corpusApp = angular.module("corpusApp", ['ngRoute']);
corpusApp.controller("corpus", function ($scope, $http, $interval) {
    $scope.coprusList;
    $scope.corpusContent;
    $scope.corpus;
    $scope.selectedCorpus;
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
        $scope.selectedCorpus = c;
        for (var i = 0; i < $scope.corpusList.length; i++) {
            if ($scope.corpusList[i].name == c) {
                $scope.corpus = $scope.corpusList[i];
                $scope.corpusContent = $scope.getCorpusContent($scope.corpusList[i].id);
                break;
            }
        }

        console.log($scope.corpus);
    }

    $scope.deleteContent = function (contentId, corpusId) {
        var route = "/api/CorpusContent/delete/" + contentId;

        $http({
            method: 'get',
            url: route
        })
           .success(function (response) {
               console.log(response);
               alert("Successfully Deleted");
               $scope.display($scope.selectedCorpus);
           })
           .error(function () {
               console.log("Failed to get corpus content.")
           });
    }

    $scope.createContent = function () {
        var conn = $http.post('/api/CorpusContent/add', $scope.newContent,
        {
            headers: { 'Content-Type': 'application/json' }
        })
        .success(function () {
            $scope.display($scope.selectedCorpus);
        })
        .error(function (e) {
            alert("Error: " + e);
        });
    }

    //Initialize the scope
    $scope.getCorpusList();
});