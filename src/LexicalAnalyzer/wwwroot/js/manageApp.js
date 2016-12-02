var manageApp = angular.module("manageApp", ['ngRoute']);

manageApp.controller("ManageController", function ($scope, $http) {
    $scope.init = function () {
        getExistingScrapers();
    }
    $scope.init();

    var existingScrapers = [];

    $scope.editScraper = function (e) {
        var target = $(e.target);
        var guid = target.parent().parent().siblings(".guid").text().trim();
        //var tempObj;
        //for(var key in existingScrapers) {
        //    if(existingScrapers[key]["Guid"] == guid)
        //        tempObj = existingScrapers[key]
        //}
        //console.log(tempObj);
        localStorage.setItem("guid", guid);
        window.location.href = "Scraper";
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

    $scope.pauseScraper = function (e) {
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
        current = 0;
        $http({
            method: 'get',
            url: '/api/scraper/'
        })
        .success(function (response) {
            for (var key in response) {
                existingScrapers.push(response[key]);
            }
            getScraperDetails();
        })

        .error(function (response) {
            console.log(response);
        });
    }
    var count = 0;

    function getScraperDetails() {
        count = existingScrapers.length;
        for (var key in existingScrapers) {
            $http({
                method: 'get',
                url: '/api/scraper/' + existingScrapers[key]["Guid"]
            })
            .success(function (response) {
                incrementCount();
            })
            .error(function (response) {
                console.log("getScraperDetails() failed: " + response)
            });
        }
    }

    var current = 0;
    function incrementCount() {
        current++;
        if (current == count) {
            setupTable();
        }
    }

    function setupTable() {
        $("#scraperEdit").show();
        var localSC = [];
        var sc = {}
        for (var key in existingScrapers) {
            sc = {}
            sc.guid = existingScrapers[key]["Guid"];
            sc.status = existingScrapers[key]["Status"];
            sc.priority = existingScrapers[key]["Priority"];
            sc.progress = existingScrapers[key]["Progress"];
            sc.name = "name";
            sc.type = existingScrapers[key]["DName"];
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


    //var types = {};

    //function getTypes() {
    //    $http({
    //        method: 'get',
    //        url: '/api/scraper/types'
    //    })
    //        .success(function (response) {
    //            if (response !== 'undefined') {
    //                for (var key in response) {
    //                    types[response[key]["type"]] = response[key];
    //                    types[response[key]["type"]] = {};
    //                    for (var key2 in response[key]) {
    //                        if (key2 !== "type") {
    //                            types[response[key]["type"]][key2] = response[key][key2];
    //                        }
    //                    }
    //                }

    //                setupForm();
    //            }
    //        })
    //        .error(function () {

    //        });

    //    function setupForm() {
    //        // scrapers

    //        var tempArr = {};
    //        for (var key in types) {
    //            tempArr[key] = key;
    //        }
    //        $scope.scrapers = tempArr;
    //        $("#scrapers").change(updateDescription);
    //    }

    //    function updateDescription() {
    //        $("#scraperContent").empty();
    //        var selected = $("#scrapers").find(":selected").val();
    //        if (selected) {
    //            var localBuild = "";
    //            for (var key in types[selected]) {
    //                if (key !== "properties") {
    //                    localBuild += "<div><h4>" + key + "</h4>";
    //                    localBuild += types[selected][key] + "<hr /></div>";
    //                }
    //            }
    //            $(localBuild).appendTo("#scraperContent");
    //        }
    //        listProperties();
    //    }

    //    function listProperties() {
    //        build = "";
    //        var selected = $("#scrapers").find(":selected").val();
    //        if (selected) {
    //            $("#scraperProperties").empty();
    //            properties = types[selected]["properties"];
    //            for (var i = 0; i < properties.length; i++) {
    //                build += '<label>' + properties[i]["key"] + "(" + properties[i]["type"] + "): " + '</label><input type="text" class="form-control" id="' + properties[i]["key"] + '" placeholder="' + properties[i]["value"] + '"><hr />';
    //            }
    //            $(build).appendTo("#scraperProperties");
    //        }
    //    }
    //}
});