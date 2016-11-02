var workflowApp = angular.module("workflowApp", ['ngRoute']);
workflowApp.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.

        when('/scrapper', {
            templateUrl: 'scrapper.htm',
            controller: 'ScrapperController'
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

workflowApp.controller('ScrapperController', ['$scope', '$http', 'factoryWebsites', function ($scope, $http, factoryWebsites) {
    $scope.message = "ScrapperController";

    $scope.websites = factoryWebsites.getWebsites();

    $scope.init = function () {
        $("#workflowButtons").hide("slow");
        $("#newEditScraper").modal();
    }

    $scope.init();

    $scope.newScraper = function () {
        $("#newEditScraper").modal("hide");
        getTypes();
    }

    $scope.editScraper = function () {
        $("#newEditScraper").modal("hide");
        var existingScrapers = [];
        getTypes();
        $http({
            method: 'get',
            url: '/api/scraper/'
        })
        .success(function (response) {
            for (var key in response) {
                existingScrapers.push(response[key]);
            }
        })
        .error(function (response) {
            console.log(response);
        });
        if (existingScrapers.length > 0) {
            $("#name").remove();
            var scraperSelect = "<select>";
            for (var key in existingScrapers)
                scraperSelect += "<option>" + key + "</option>";
            scraperSelect += "</select>";
            $(scraperSelect).appendTo("#scraperForm");
        } else {
            alert("no existing scrapers");
        }
    }

    $scope.runScrape = function () {
        var sitesToScrape = [];
        var scraperType = "";
        var scraperName = "";
        $('#containElements').empty();
        scraperType = $("#scraperTypes").find(":selected").text();
        $("#exampleSelect1 :selected").each(function () {
            sitesToScrape.push($(this).val());
        });
        for (var i = 0; i < sitesToScrape.length; i++)
            scrapeSites(sitesToScrape[i]);
        $http({
            method: 'POST',
            url: '/api/scraper/',
            data: JSON.stringify(scraperType)
        })
        .success(function (response) {
            $http({
                method: 'post',
                url: '/api/scraper/' + response["Guid"] + '/start'
            });
        })
        .error(function () {

        });
    };

    var transition = 0;
    var circleTransition = 0;
    var count = 0;

    $scope.nextStep = function () {
        if (transition < 70) {
            transition += 114;
        }
        else {
            transition += 64;
        }
        if (transition < 540) {
            $(".nav-active-inner").css("transform", "translate(0px," + transition + "px)");
            $(".nav-active-outer").css("transform", "translate(0px," + (transition + 30) + "px)");
        }
        if (count < 2) {
            circleTransition += 61;
            $(".circle-outer").css("transform", "translate(0px," + (circleTransition) + "px)");
            $(".circle-inner").css("transform", "translate(0px," + (circleTransition) + "px)");
            count++;
        }
    }

    $scope.prevStep = function () {
        if (count > 0) {
            circleTransition -= 61;
            $(".circle-outer").css("transform", "translate(0px," + (circleTransition) + "px)");
            $(".circle-inner").css("transform", "translate(0px," + (circleTransition) + "px)");
            count--;
        }
    }

    function getTypes() {
        $http({
            method: 'get',
            url: '/api/scraper/types'
        })
            .success(function (response) {
                if (response !== 'undefined') {
                    var build = ""
                    for (var i = 0; i < response.length; i++) {
                        build = ""
                        var hold = response[i];
                        build += '<div class="panel-body">'
                        for (var key in hold) {
                            if (key == "properties") {
                                build += '</div>';
                                $(build).appendTo("#scraperInfo");
                                break;
                            }
                            if (key !== "displayName") {
                                build += '<label>' + key + ": " + hold[key] + '</label>';
                            }
                        }

                        build = "";
                        var prop = hold["properties"];
                        if (prop.length > 0) {
                            build += '<div class="panel-body">'
                            for (var k = 0; k < prop.length; k++) {
                                hold = prop[k];
                                build += '<hr /><label>' + response[i]["type"] + '</label>';
                                build += '<label>' + hold["key"] + "(" + hold["type"] + "): " + '</label><input type="text" class="form-control" placeholder="' + hold["value"] + '"><hr />';
                            }
                            build += '</div>'

                            $(build).appendTo("#scraperProperties");
                        }
                    }
                }
            })
            .error(function () {

            });
    }
}]);

workflowApp.controller('TrainController', function ($scope) {
    $scope.message = "TrainController";
    $('#containElements').empty();
});
