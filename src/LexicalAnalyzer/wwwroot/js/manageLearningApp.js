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

learningApp.controller("ManageLearningController", function ($scope, $http) {
    var nameConversion = {};
    $scope.init = function () {
        getTypes();        
    }

    function getTypes() {
        types = {};
        $http({
            method: 'get',
            url: '/api/learningmodel/types'
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

    var existingLearnings = [];

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
            url: '/api/learningmodel/' + guid
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
            url: '/api/learningmodel/' + guid + '/pause'
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
            url: '/api/learningmodel/' + guid + '/start'
        })
        .success(function (response) {
            console.log(response);
            getExistingLearnings();
        })
        .error(function () {

        });
    }

    function getExistingLearnings() {
        existingLearnings = [];
        current = 0;
        $http({
            method: 'get',
            url: '/api/learningmodel/'
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
            lm.desc = existingLearnings[key]["description"];
            lm.name = existingLearnings[key]["UserGivenName"];
            localLM.push(lm);
        }
        $scope.currentLearningList = localLM;
    }

    function getLearningByGuid(guid) {
        for (var key in existingLearnings)
            if (existingLearnings[key]["Guid"] == guid)
                return existingLearnings[key];
        return {};
    }
});