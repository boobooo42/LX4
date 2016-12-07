var manageApp = angular.module("manageApp", ['ngRoute']);

manageApp.directive('highlight', function() {
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

manageApp.controller("ManageController", function ($scope, $http) {
    $scope.init = function () {
        getExistingScrapers();
    }
    $scope.init();

    var existingScrapers = [];

    $scope.editScraper = function (e) {
        var target = $(e.target);
        var guid = target.parent().parent().parent().attr("id");
        console.log(guid);
        //localStorage.setItem("guid", guid);
        //window.location.href = "Scraper";
    }

    $scope.deleteScraper = function (e) {
        var target = $(e.target);
        target.parent().parent().parent().hide();
        var guid = target.parent().parent().parent().attr("id");
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

    $scope.pauseScraper = function (e) {
        var target = $(e.target);
        var guid = target.parent().parent().parent().attr("id");
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
        var guid = target.parent().parent().parent().attr("id");
        var type = target.parent().parent().siblings(".type").text().trim();
        if (isTwitterandAuth(type, guid)) {
            console.log("twitAuth");
            getTwitterAuth(guid, e);
        } else {
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
    }

    function isTwitterandAuth(type, guid) {
        if (type == "Twitter Scraper") {
            twitScraper = getScraperByGuid(guid);
            console.log(twitScraper);
            return !twitScraper["Authorized"];
        } else
            return false;
    }

    function getTwitterAuth(guid, e) {
        $("#twitterPin").value = "";
        $http({
            method: 'get',
            url: '/api/scraper/twitter/' + guid
        })
        .success(function (response) {
            console.log(response);
            $scope.twitterAuthURL = response;
            $("#twitterAuth").modal('show');
            $("#submitPin").click(function () {
                var tPin = $("#twitterPin").val().trim(); 
                $http({
                    method: 'get',
                    url: '/api/scraper/twitter/' + tPin + '/' + guid
                })
                .success(function (response) {
                    console.log(response);
                    $("#twitterAuth").modal('hide');
                    twitScraper["Authorized"] = true;
                    //$scope.startScraper(e);
                })
                .error(function (response) {
                    $("#twitterAuth").modal('hide');
                    alert("auth failed" + response);
                });
            });
        })
        .error(function (response) {
            console.log(response);
        });
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
        console.log(existingScrapers);
        for (var key in existingScrapers) {
            sc = {}
            sc.guid = existingScrapers[key]["Guid"];
            sc.status = existingScrapers[key]["Status"];
            sc.priority = existingScrapers[key]["Priority"];
            sc.progress = existingScrapers[key]["Progress"];
            sc.name = existingScrapers[key]["UserGivenName"];
            sc.type = existingScrapers[key]["TypeName"];
            sc.desc = existingScrapers[key]["Desc"];
            localSC.push(sc);
        }
        $scope.currentScraperList = localSC;
    }

    function getScraperByGuid(guid) {
        for (var key in existingScrapers)
            if (existingScrapers[key]["Guid"] == guid)
                return existingScrapers[key];
        return {};
    }
});