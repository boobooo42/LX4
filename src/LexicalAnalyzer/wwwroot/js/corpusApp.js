var corpusApp = angular.module("corpusApp", ['ngRoute']);
corpusApp.controller("corpus", function ($scope, $http, $interval) {

    document.getElementById('fileinput').addEventListener('change', readSingleFile, false);

    $scope.coprusList;
    $scope.corpusContent;
    $scope.corpus;
    $scope.newContent = {
        name: "",
        content: "",
        type: ""
    };
    $scope.selectedCorpus;
    $scope.newCorpus = {
        "id": 0,
        "name": "string",
        "description": "string",
        "locked": true,
        "hash": "string"
    };
    $scope.contentName;
    $scope.contentType;

    $scope.createCorpus = function () {

        var conn = $http.post(UrlContent('/api/corpus'), $scope.newCorpus,
        {
            headers: { 'Content-Type': 'application/json' }
        })
        .success(function () {
            $scope.getCorpusList();
        })
        .error(function (e) {
            alert("Error: " + e);
        });
    }


    $scope.deleteCorpus = function (corpusId) {
    var route = UrlContent("/delete/" + corpusId);

        $http({
            method: 'get',
                    url: route
            })
           .success(function(response) {
               console.log(response);
               location.reload();
               })
           .error(function () {
               console.log("Failed to delete corpus.")
           });
    }

    //Gets all corpora
    $scope.getCorpusList = function () {

        $http({
            method: 'get',
            url: UrlContent('/api/corpus'),
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

        var route = UrlContent("/api/CorpusContent/list/" + corpusId);

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
        $scope.corpus = c;
        $scope.corpusContent = $scope.getCorpusContent(c.id);
        console.log($scope.corpus);
    }

    $scope.setNewContent = function (name, content) {
        $scope.newContent.name = name;
        $scope.newContent.content = content;
        $scope.newContent.type = "text";
    }

    $scope.deleteContent = function (contentId, c) {

        var route = UrlContent("/api/CorpusContent/delete/" + contentId);

        $http({
            method: 'get',
            url: route
        })
           .success(function (response) {
               console.log(response);
               $scope.display(c);
           })
           .error(function () {
               console.log("Failed to get corpus content.")
           });

    }

    $scope.createContent = function (c) {
        $scope.newContent.corpusId = c.id;

        $http.post(UrlContent('/api/CorpusContent/add/'), $scope.newContent,
        {
            headers: { 'Content-Type': 'application/json' }
        })
        .success(function () {
            $scope.display(c);
        })
        .error(function (e) {
            alert("Error: " + e);
        });
    }
    //Initialize the scope
    $scope.getCorpusList();
});

function setFileContent(name, content) {
    var scope = angular.element(document.getElementById("maincontroller")).scope();
    scope.$apply(function () {
        scope.setNewContent(name, content);
    });
}

function readSingleFile(evt) {
    //Retrieve the first (and only!) File from the FileList object
    var f = evt.target.files[0];

    if (f) {
        var r = new FileReader();
        r.onload = function (e) {
            var contents = e.target.result;
            alert("Got the file.n"
                  + "name: " + f.name + "n"
                  + "type: " + f.type + "n"
                  + "size: " + f.size + " bytesn"
                  + "starts with: " + contents
            );

            setFileContent(f.name, contents);
        }
        r.readAsText(f);
    } else {
        alert("Failed to load file");
    }
}