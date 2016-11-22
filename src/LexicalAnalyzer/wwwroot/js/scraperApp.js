var manageApp = angular.module("scraperApp", ['ngRoute']);

manageApp.controller("ScraperController", function ($scope, $http) {
    var editScraper;
    $scope.init = function () {
        if (localStorage.getItem("guid")) {
            editScraper = JSON.parse(localStorage.getItem("guid"))
            localStorage.clear();
        } else
            editScraper = null;
        getTypes();
    }

    $scope.init();

    var types = {};
    var nameConversion = {};

    function getTypes() {
        $http({
            method: 'get',
            url: '/api/scraper/types'
        })
            .success(function (response) {
                if (response !== 'undefined') {
                    for (var key in response) {
                        types[response[key]["type"]] = response[key];
                        nameConversion[response[key]["displayName"]] = response[key]["type"];
                        nameConversion[response[key]["type"]] = response[key]["displayName"];
                        types[response[key]["type"]] = {};
                        for (var key2 in response[key]) {
                            if (key2 !== "type") {
                                types[response[key]["type"]][key2] = response[key][key2];
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
                tempArr[key] = types[key]["displayName"];
            }
            $scope.scrapers = tempArr;
            if(editScraper)
                $('#scrapers option').filter(function () {
                    console.log(types);
                    console.log(editScraper["type"]);
                    console.log(types[editScraper["type"]]);
                    return ($(this).text() == types[editScraper["type"]]["displayName"]); //To select Blue
                }).prop('selected', true);
            $("#scrapers").change(updateDescription);
        }

        function updateDescription() {
            $("#scraperContent").empty();
            var selected = $("#scrapers").find(":selected").val();
            if (selected) {
                var localBuild = "";
                for (var key in types[nameConversion[selected]]) {
                    if (key !== "properties") {
                        localBuild += "<div><h4>" + key + "</h4>";
                        localBuild += types[nameConversion[selected]][key] + "<hr /></div>";
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
                properties = types[nameConversion[selected]]["properties"];
                for (var i = 0; i < properties.length; i++) {
                    build += '<label>' + properties[i]["key"] + "(" + properties[i]["type"] + "): " + '</label><input type="text" class="form-control" id="' + properties[i]["key"] + '" placeholder="' + properties[i]["value"] + '"><hr />';
                }
            }
            if (editScraper) {
                build += '<label> Priority: </label><input type="text" class="form-control" id="Priority placehoder="' + editScraper["Priority"] + '"/></hr>"';
            }
            $(build).appendTo("#scraperProperties");
        }
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

    $scope.createScraper = function () {
        var sitesToScrape = [];
        var scraperType = nameConversion[$("#scrapers").find(":selected").val()];
        console.log(scraperType);
        var scraperName = $("#scraperName").val();
        var tempProperties = types[scraperType]["properties"];
        var data = {
            "status": "init",
            "progress": 0,
            "priority": 0,
            "properties": []
        }
        for (var i = 0; i < tempProperties.length; i++) {
            var tempProps = tempProperties[i];
            data["properties"].push({ "key": tempProps["key"], "type": tempProps["type"], "value": $("#" + tempProps["key"]).val() });
        }
        console.log(data);
        $http({
            method: 'post',
            url: '/api/scraper/' + scraperType,
            data: data
        })
        .success(function (response) {
            console.log(response);
        })
        .error(function () {

        });
    };
});