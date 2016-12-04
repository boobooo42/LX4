var manageApp = angular.module("scraperApp", ['ngRoute']);

manageApp.controller("ScraperController", function ($scope, $http) {
    var editScraper = {};
    $scope.init = function () {
        if (localStorage.getItem("guid")) {
            getScraperByGuid(localStorage.getItem("guid"));
            localStorage.clear();
        } else {
            editScraper = null;
            getTypes();
        }

    }

    $scope.init();

    var types;
    var nameConversion = {};

    function getTypes() {
        types = {};
        if (editScraper) {
            
            types[editScraper["Type"]] = {
                "displayName": editScraper["DName"],
                "description": editScraper["Desc"],
                "properties": editScraper["Properties"],
                "guid": editScraper["Guid"],
                "priority": editScraper["Priority"],
                "progress": editScraper["Progress"],
                "status": editScraper["Status"]
            }
            var properties = [];
          
            for (var key in editScraper["Properties"]) {
                properties.push(
                    {
                        "key": editScraper["Properties"][key]["Key"],
                        "type" : editScraper["Properties"][key]["Type"],
                        "value": editScraper["Properties"][key]["Value"]
                    });
            }
            types[editScraper["Type"]]["properties"] = properties;
            nameConversion[editScraper["Type"]] = editScraper["DName"];
            nameConversion[editScraper["DName"]] = editScraper["Type"];
            setupForm();
        } else {

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
                            //types[response[key]["type"]] = {};
                            for (var key2 in response[key]) {
                                if (key2 !== "type") {
                                    types[response[key]["type"]][key2] = response[key][key2];
                                }
                            }
                        }
                        console.log(types);
                        setupForm();
                    }
                })
                .error(function () {

                });
        }
    }

    function setupForm() {
        var tempArr = {};
        for (var key in types) {
            tempArr[key] = types[key]["displayName"];
        }
        $scope.scrapers = tempArr;
        $("#scrapers").change(updateDescription);
    }

    function updateDescription() {
        $("#scraperContent").empty();
        var selected = $scope.selectedScraper;
        if (selected) {
            var localBuild = "";
            for (var key in types[nameConversion[selected]]) {
                if (key !== "properties" && key !== "displayName") {
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
        var selected = $scope.selectedScraper;
        if (selected) {
            $("#scraperProperties").empty();
            properties = types[nameConversion[selected]]["properties"];
            for (var i = 0; i < properties.length; i++) {
                build += '<label>' + properties[i]["key"] + "(" + properties[i]["type"] + "): " + '</label><input type="text" class="form-control" id="' + properties[i]["key"] + '" placeholder="' + properties[i]["value"] + '"><hr />';
            }
        }
        $(build).appendTo("#scraperProperties");
    }

    function getScraperByGuid(guid) {
        console.log(guid);
        $http({
            method: 'get',
            url: '/api/scraper/' + guid
        })
        .success(function (response) {
            console.log(response);
            editScraper = response;
            getTypes();
        })

        .error(function (response) {
            console.log(response);
        });
    }

    $scope.createScraper = function () {
        var sitesToScrape = [];
        var scraperType = nameConversion[$scope.selectedScraper];
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
            var val = ($("#" + tempProps["key"]).val());
            if (val == "")
                val = $("#" + tempProps["key"]).attr('placeholder');
            data["properties"].push({ "key": tempProps["key"], "type": tempProps["type"], "value": val });
        }
        console.log(data);
        $http({
            method: 'post',
            url: '/api/scraper/' + scraperType,
            data: data
        })
        .success(function (response) {
            console.log(response);
            $("#scraperNew").closest('form').find("input[type=text], textarea").val("");
        })
        .error(function () {

        });
    };
});