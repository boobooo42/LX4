var workflowApp = angular.module("workflowApp", ['ngRoute']);
workflowApp.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.

        when('/scraper', {
            templateUrl: 'scraper.htm',
            controller: 'ScraperController'
        }).

        when('/train', {
            templateUrl: 'train.htm',
            controller: 'TrainController'
        });
        
}])
    .factory('factoryWebsites', function () {
        var factory = {};
        factory.getWebsites = function () {
            return ["Twitter", "Github", "Facebook", "Site 4", "Site 5", "Site 6"];
        };
        return factory;
    });

workflowApp.controller('ScraperController', ['$scope', '$http', 'factoryWebsites', '$compile', function ($scope, $http, factoryWebsites, $compile) {
    $scope.message = "ScraperController";

    $scope.websites = factoryWebsites.getWebsites();

    $scope.init = function () {
        $("#workflowButtons").hide("slow");
        $("#newEditScraper").modal();
    }

    $scope.init();

    $scope.newScraper = function () {
        $("#newEditScraper").modal("hide");
        $("#scraperNew").show();
        getTypes();
    }
    var existingScrapers = [];

    $scope.editScraper = function () {
        $("#newEditScraper").modal("hide");    
        getExistingScrapers();
    }

    $scope.setupEdit = function (e) {
        alert("click edit");
    }

    $scope.deleteScraper = function (e) {
        var target = $(e.target);
        target.parent().parent().hide("slow");
        var guid = target.parent().siblings(".guid").text();
        $http({
            method: 'delete',
            url: '/api/scraper/' + guid
        })
        .success(function (response) {
            console.log(response);
        })
        .error(function (response) {
            console.log(response);
        });
    }

    $scope.stopScraper = function (e) {
        var target = $(e.target);
        var guid = target.parent().siblings(".guid").text();
        $http({
            method: 'post',
            url: '/api/scraper/' + guid + '/pause'
        })
        .success(function (response) {
            console.log(response);
            getExistingScrapers();
        })
        .error(function () {

        });
    }

    $scope.startScraper = function (e) {
        var target = $(e.target);
        var guid = target.parent().siblings(".guid").text();
        $http({
            method: 'post',
            url: '/api/scraper/' + guid + '/start'
        })
        .success(function (response) {
            console.log(response);
            getExistingScrapers();
        })
        .error(function () {

        });
    }

    $scope.createScraper = function () {
        var sitesToScrape = [];
        var scraperType = "";
        var scraperName = "";
        scraperName = $("#scrapers").find(":selected").val();
        var scraperExplicit = types[scraperName]["type"];
        $http({
            method: 'post',
            url: '/api/scraper/',
            data: JSON.stringify(scraperExplicit)
        })
        .success(function (response) {
            console.log(response);
        })
        .error(function () {

        });
    };

    var types = {};

    function getTypes() {
        $http({
            method: 'get',
            url: '/api/scraper/types'
        })
            .success(function (response) {
                if (response !== 'undefined') {
                    for (var key in response) {
                        types[response[key]["displayName"]] = response[key];
                        types[response[key]["displayName"]] = {};
                        for (var key2 in response[key]) {
                            if (key2 !== "displayName") {
                                types[response[key]["displayName"]][key2] = response[key][key2];
                            }
                        }
                    }

                    setupForm();
                }
            })
            .error(function () {

            });

        function setupForm() {
            // scrapers

            var tempArr = {};
            for (var key in types) {
                tempArr[key] = key;
            }
            $scope.scrapers = tempArr;
            $("#scrapers").change(updateDescription);
        }

        function updateDescription() {
            $("#scraperContent").empty();
            var selected = $("#scrapers").find(":selected").val();
            if (selected) {
                var localBuild = "";
                for (var key in types[selected]) {
                    if (key !== "properties") {
                        localBuild += "<div><h4>" + key + "</h4>";
                        localBuild += types[selected][key] + "<hr /></div>";
                    }
                }
                $(localBuild).appendTo("#scraperContent");
            }
            listProperties();
        }

        function listProperties() {
            build = "";
            var selected = $("#scrapers").find(":selected").val();
            if (selected) {
                $("#scraperProperties").empty();
                properties = types[selected]["properties"];
                for (var i = 0; i < properties.length; i++) {
                    build += '<label>' + properties[i]["key"] + "(" + properties[i]["type"] + "): " + '</label><input type="text" class="form-control" placeholder="' + properties[i]["value"] + '"><hr />';
                }
                $(build).appendTo("#scraperProperties");
            }
        }
    }

    function getExistingScrapers() {
        existingScrapers = [];
        $http({
            method: 'get',
            url: '/api/scraper/'
        })
        .success(function (response) {
            for (var key in response) {
                existingScrapers.push(response[key]);
            }
            setupTable();
        })
        .error(function (response) {
            console.log(response);
        });
    }

    function setupTable() {
        $("#scraperEdit").show();
        $("#editTBody").empty();
        var $build = "";
        for (var key in existingScrapers) {
            $build += '<tr><p><th scope="row"><span class="glyphicon glyphicon-file" ng-click="setupEdit($event)"></span>'
                    + '<span class="glyphicon glyphicon-trash" ng-click="deleteScraper($event)"></span>'
                    + '<span class="glyphicon glyphicon-stop" ng-click="stopScraper($event)"></span>'
                    + '<span class="glyphicon glyphicon-play" ng-click="startScraper($event)"></span></p></th>'
                    + '<th scope="row" class="guid">' + existingScrapers[key]["Guid"] + '</th>'
                    + '<th scope="row">' + existingScrapers[key]["Status"] + '</th>'
                    + '<th scope="row">' + "name" + '</th>'
                    + '<th scope="row">' + "type" + '</th>'
                    + '<th scope="row">' + "description" + '</th>';
        }
        $("#editTBody").append($compile($build)($scope));
    }
}]);

workflowApp.controller('TrainController', function ($scope) {
    $scope.message = "TrainController";
    $('#containElements').empty();
});
