var learningApp = angular.module("manageLearningApp", ['ngRoute']);

learningApp.directive('highlight', function () {
    return function (scope, element, attrs) {
        var guid = localStorage.getItem("guid");
        angular.element(element).removeAttr("hidden");
        if (guid !== null) {
            if (attrs.id == guid) {
                angular.element(element).addClass("new");
                localStorage.removeItem("guid");
            }
        }
    };
})

learningApp.controller("ManageLearningController", function ($scope, $http, $interval) {
    var nameConversion = {};
    var existingLearnings = [];
    var progressUpdates;
    var existingCorpora = []

    $scope.init = function () {
        progressUpdates = getUpdates();
        getExistingCorpora();
        getTypes();
    }

    function getTypes() {
        $http({
            method: 'get',
            url: UrlContent( '/api/learningmodel/types')
        })
            .success(function (response) {
                for (var key in response) {
                    nameConversion[response[key]["displayName"]] = response[key]["type"];
                    nameConversion[response[key]["type"]] = response[key]["displayName"];
                }
                getExistingLearnings();
            })
            .error(function () {

            });
    }

    $scope.init();

    $scope.editLearning = function (e) {
        var target = $(e.target);
        var guid = target.parent().parent().parent().attr("id");
        localStorage.setItem("guid", guid);
        //window.location.href = "Learning";
    }

    $scope.deleteLearning = function (e) {
        var target = $(e.target);
        target.parent().parent().parent().hide();
        var guid = target.parent().parent().parent().attr("id");
        console.log(guid);
        $http({
            method: 'delete',
            url: UrlContent( '/api/learningmodel/' + guid)
        })
        .success(function (response) {
            console.log(response);
        })
        .error(function (response) {
            console.log(response);
        });
    }

    $scope.pauseLearning = function (e) {
        var target = $(e.target);
        var guid = target.parent().parent().parent().attr("id");
        $http({
            method: 'post',
            url: UrlContent( '/api/learningmodel/' + guid + '/pause')
        })
        .success(function (response) {
            console.log(response);
            getExistingLearnings();
        })
        .error(function () {

        });
    }

    $scope.startLearning = function (e) {
        var target = $(e.target);
        var guid = target.parent().parent().parent().attr("id");
        $http({
            method: 'post',
            url: UrlContent( '/api/learningmodel/' + guid + '/start')
        })
        .success(function (response) {
            console.log(response);
            getExistingLearnings();
        })
        .error(function () {

        });
    }

    function getExistingLearnings() {
        $interval.cancel(progressUpdates);
        existingLearnings = [];
        current = 0;
        $http({
            method: 'get',
            url: UrlContent('/api/learningmodel/')
        })
        .success(function (response) {
            for (var key in response) {
                existingLearnings.push(response[key]);
            }
            setupTable();
        })

        .error(function (response) {
            console.log(response);
        });
    }

    function getUpdates() {
        return $interval(function () {
            existingScrapers = [];
            $http({
                method: 'get',
                url: UrlContent( '/api/learningmodel/')
            })
            .success(function (response) {
                for (var key in response) {
                    existingScrapers.push(response[key]);
                    if (response[key]["Status"] == "started") {
                        $("#" + response[key]["Guid"]).children("#lProgress")[0].innerHTML = response[key]["Progress"];
                    }
                }
            })

            .error(function (response) {
                console.log(response);
            });
        }, 1500);
    }

    function setupTable() {
        $("#learningEdit").show();
        var localLM = [];
        var lm = {}
        console.log(existingLearnings);
        for (var key in existingLearnings) {
            lm = {}
            lm.guid = existingLearnings[key]["Guid"];
            lm.status = existingLearnings[key]["Status"];
            lm.priority = existingLearnings[key]["Priority"];
            lm.progress = existingLearnings[key]["Progress"];
            lm.result = existingLearnings[key]["Result"]["Data"];
            lm.type = nameConversion[existingLearnings[key]["Type"]];
            lm.name = existingLearnings[key]["Properties"][0]["Value"];
            for (var keyC in existingCorpora) {
                for (var keyS in existingLearnings) {
                    for (var keyP in existingLearnings[keyS]["Properties"]) {
                        if (existingLearnings[keyS]["Properties"][keyP]["Key"] == "corpus") {
                            if (existingCorpora[keyC]["id"] == existingLearnings[keyS]["Properties"][keyP]["Value"]) {
                                lm.corpus = existingCorpora[key]["name"];
                            }
                        }
                    }
                }
            }
            localLM.push(lm);
        }
        $scope.currentLearningList = localLM;
        progressUpdates = getUpdates();
    }
    
    function getExistingCorpora() {
        $http({
            method: 'get',
            url: UrlContent('/api/corpus/')
        })
        .success(function (response) {
            console.log(response);
            for (var i = 0; i < response.length; i++) {
                existingCorpora.push(response[i]);
            }
        })
        .error(function () {

        });
    }

    function getLearningByGuid(guid) {
        for (var key in existingLearnings)
            if (existingLearnings[key]["Guid"] == guid)
                return existingLearnings[key];
        return {};
    }
});