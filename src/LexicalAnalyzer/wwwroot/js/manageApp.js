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

manageApp.controller("ManageController", function ($scope, $http, $interval) {
    $scope.init = function () {
        progressUpdates = getUpdates();
        getExistingCorpora();
    }
    $scope.init();

    var progressUpdates;
    var existingScrapers = [];
    var existingCorpora = [];

    $scope.editScraper = function (e) {
        var target = $(e.target);
        var guid = target.parent().parent().parent().attr("id");
        $scope.modalguid = guid;
        var editScraper = getScraperByGuid(guid);
        var build = "";
        $("#editScraperModalBody").empty();
        if (editScraper["Status"] == "started") {
            build = "<p>Scraper has been started and cant be edited.</p>"
            $("#modalEditButton").attr("hidden");
        } else {
            var properties = editScraper["Properties"];
            for (var i in properties) {
                build += '<label>' + properties[i]["Key"] + "(" + properties[i]["Type"] + "): " + '</label><input type="text" class="form-control" id="' + properties[i]["Key"] + '" mytype="' + properties[i]["Type"] + '" placeholder="' + properties[i]["Value"] + '"><hr />';
            }
        }
        $(build).appendTo("#editScraperModalBody");
        $("#editScraperModal").modal('show');
    }

    $scope.submitScraperEdits = function () {
        /*var editScraper = getScraperByGuid($scope.modalguid);
        var data = {
            "Status": "paused",
            "Properties": []
        }
        $("form#editScraperModalBody :input").each(function () {
            var input = $(this); // This is the jquery object of the input, do what you will
            if (input.text == "") {
                data["Properties"].push({ "key": input.id, "type": input.mytype, "value": input.placeholder });
            } else {
                data["Properties"].push({ "key": input.id, "type": input.mytype, "value": input.text });
            }
        });

        $http({
            method: 'put',
            url: '/api/scraper/' + $scope.modalguid,
            data: data
        })
        .success(function (response) {
            console.log(response);
            $("#editScraperModal").modal('hide');
            getExistingScrapers();
        })
        .error(function (response) {
            console.log(response);
            $("#editScraperModal").modal('hide');
        });*/

    }

    $scope.deleteScraper = function (e) {
        var target = $(e.target);
        target.parent().parent().parent().hide();
        var guid = target.parent().parent().parent().attr("id");
        $http({
            method: 'delete',
            url: UrlContent('/api/scraper/' + guid)
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
            url: UrlContent('/api/scraper/' + guid + '/pause')
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
            getTwitterAuth(guid, e);
        } else {
            $http({
                method: 'post',
                url: UrlContent('/api/scraper/' + guid + '/start')
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
            return !twitScraper["Authorized"];
        } else
            return false;
    }

    function getTwitterAuth(guid, e) {
        $("#twitterPin").value = "";
        $http({
            method: 'get',
            url: UrlContent('/api/scraper/twitter/' + guid)
        })
        .success(function (response) {
            console.log(response);
            $scope.twitterAuthURL = response;
            $("#twitterAuth").modal('show');
            $("#submitPin").click(function () {
                var tPin = $("#twitterPin").val().trim(); 
                $http({
                    method: 'get',
                    url: UrlContent('/api/scraper/twitter/' + tPin + '/' + guid)
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
        $interval.cancel(progressUpdates);
        existingScrapers = [];
        $http({
            method: 'get',
            url: UrlContent('/api/scraper/')
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

    function getUpdates() {
        return $interval(function () {
            existingScrapers = [];
            $http({
                method: 'get',
                url: UrlContent('/api/scraper/')
            })
            .success(function (response) {
                for (var key in response) {
                    existingScrapers.push(response[key]);
                    if(response[key]["Status"] == "started"){
                        $("#" + response[key]["Guid"]).children("#sProgress")[0].innerHTML = response[key]["Progress"];
                        $("#" + response[key]["Guid"]).children("#seTime")[0].innerHTML = response[key]["Timer"]["Elapsed"];
                        $("#" + response[key]["Guid"]).children("#sdCount")[0].innerHTML = response[key]["DownloadCount"];
                    }
                }
            })

            .error(function (response) {
                console.log(response);
            });
        }, 1500);
    }

    function setupTable() {
        $("#scraperEdit").show();
        var localSC = [];
        var sc = {};
        for (var key in existingScrapers) {
            sc = {}
            sc.guid = existingScrapers[key]["Guid"];
            sc.status = existingScrapers[key]["Status"];
            sc.priority = existingScrapers[key]["Priority"];
            sc.progress = existingScrapers[key]["Progress"];
            sc.name = existingScrapers[key]["UserGivenName"];
            sc.type = existingScrapers[key]["TypeName"];
            sc.desc = existingScrapers[key]["Desc"];
            sc.eTime = existingScrapers[key]["Timer"]["Elapsed"];
            sc.dCount = existingScrapers[key]["DownloadCount"];
            sc.dLimit = existingScrapers[key]["DownloadLimit"];
            sc.tLimit = existingScrapers[key]["TimeLimit"];
            for (var keyC in existingCorpora) {
                for (var keyP in existingScrapers[key]["Properties"]) {
                    if (existingScrapers[key]["Properties"][keyP]["Key"] == "corpus") {
                        if (existingCorpora[keyC]["id"] == existingScrapers[key]["Properties"][keyP]["Value"]) {
                            sc.corpus = existingCorpora[keyC]["name"];
                        }
                    }
                }
            }
            localSC.push(sc);
        }
        $scope.currentScraperList = localSC;
        progressUpdates = getUpdates();
    }

    function getExistingCorpora() {
        existingCorpora = [];
        $http({
            method: 'get',
            url: UrlContent('/api/corpus/')
        })
        .success(function (response) {
            console.log(response);
            for (var i = 0; i < response.length; i++) {
                existingCorpora.push(response[i]);
            }
            getExistingScrapers();
        })
        .error(function () {
            console.log("Failed to get Corpora");
        });
    }

    function getScraperByGuid(guid) {
        for (var key in existingScrapers)
            if (existingScrapers[key]["Guid"] == guid)
                return existingScrapers[key];
        return {};
    }
});