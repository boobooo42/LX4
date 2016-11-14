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

    $scope.manageScrapers = function () {
        $("#newEditScraper").modal("hide");
        getExistingScrapers();
    }

    $scope.editScraper = function (e) {
        var target = $(e.target);
        $("#twitterPin").value = "";
        var guid = target.parent().parent().siblings(".guid").text().trim();
        $http({
            method: 'get',
            url: '/api/scraper/twitter/' + guid
        })
        .success(function (response) {
            $("#twitterPinBody").empty();
            $('<a target="_blank" href= "' + response + '">Twitter Auth</a>').appendTo("#twitterPinBody");
            $("#twitterAuth").modal('show');
            $("#submitPin").click(function () {
                var tPin = $("#twitterPin").val().trim();
                $http({
                    method: 'get',
                    url: '/api/scraper/twitter/' + tPin + "/" + guid
                })
                .success(function (response) {
                    console.log(response);
                })
                .error(function () {

                });
            });
        })
        .error(function () {

        });
    }

    $scope.deleteScraper = function (e) {
        var target = $(e.target);
        target.parent().parent().parent().hide();
        var guid = target.parent().parent().siblings(".guid").text().trim();
        console.log(guid);
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
        var guid = target.parent().parent().siblings(".guid").text().trim();
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
        var guid = target.parent().parent().siblings(".guid").text().trim();
        var url = '/api/scraper/' + guid + '/start';
        console.log(url);
        $http({
            method: 'post',
            url: url
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
        var scraperType = $("#scrapers").find(":selected").val();
        var scraperName = $("#scraperName").val();
        var scraperExplicit = types[scraperType]["type"];
        var tempProperties = types[scraperType]["properties"];
        var data = {
            "status": "init",
            "progress": 0,
            "priority": 0,
            "properties": []
        }
        for (var i = 0; i < tempProperties.length; i++) {
            var tempProps = tempProperties[i];
            data["properties"].push({ "key": tempProps["key"], "type": tempProps["type"], "value" : $("#" + tempProps["key"]).val() });
        }
        console.log(data);
        $http({
            method: 'post',
            url: '/api/scraper/' + scraperExplicit,
            data: data
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
                    build += '<label>' + properties[i]["key"] + "(" + properties[i]["type"] + "): " + '</label><input type="text" class="form-control" id="' + properties[i]["key"] + '" placeholder="' + properties[i]["value"] + '"><hr />';
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
        var localSC = [];
        var sc = {}
        for (var key in existingScrapers) {
            sc = {}
            sc.guid = existingScrapers[key]["Guid"];
            sc.status = existingScrapers[key]["Status"];
            sc.name = "name";
            sc.type = "type";
            sc.desc = "description";
            localSC.push(sc);
        }
        $scope.currentScraperList = localSC;
    }

    function getScraperByGuid(guid) {
        $http({
            method: 'get',
            url: '/api/scraper/',
            data: JSON.stringify(guid)
        })
        .success(function (response) {
            console.log(response);
        })

        .error(function (response) {
            console.log(response);
        });
    }
}]);

workflowApp.controller('TrainController', function ($scope) {
    $scope.message = "TrainController";
    $('#containElements').empty();
});