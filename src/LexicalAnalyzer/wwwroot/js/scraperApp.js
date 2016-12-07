var manageApp = angular.module("scraperApp", ['ngRoute']);

manageApp.controller("ScraperController", function ($scope, $http, $interval) {
    $scope.init = function () {
        getTypes();
        getExistingCorpora();
    }

    $scope.init();

    var types;
    var nameConversion = {};

    function getExistingCorpora() {
        $http({
            method: 'get',
            url: '/api/corpus/'
        })
        .success(function (response) {
            console.log(response);
            var tempCorpora = [];
            for (var i = 0; i < response.length; i++) {
                tempCorpora.push(response[i]["name"]);
            }
            $scope.corpora = tempCorpora;
        })
        .error(function () {

        });
    }

    function getTypes() {
        types = {};
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

    function setupForm() {
        var tempArr = {};
        for (var key in types) {
            tempArr[key] = types[key]["displayName"];
        }
        $scope.scrapers = tempArr;
        $("#scrapers").change(updateDescription);
    }

    function updateDescription() {
        redInput = [];
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

    var redInput = [];
    $scope.createScraper = function () {
        var sitesToScrape = [];
        var scraperType = nameConversion[$scope.selectedScraper];
        var scraperName = $("#scraperName").val();
        var tempProperties = types[scraperType]["properties"];
        var data = {
            "status": "init",
            "progress": 0,
            "priority": 0,
            "properties": []
        }
        var complete = true
        var name = $("#scraperName").val().trim();
        if (name == "") {
            redInput.push("#scraperName");
            $("#scraperName").css("border", "solid 1px red");
            complete = false;
        } else {
            data["properties"].push({ "key": "UserGivenName", "type": "UserGivenName", "value": name });
        }
        for (var i = 0; i < tempProperties.length; i++) {
            var tempProps = tempProperties[i];
            var val = ($("#" + tempProps["key"]).val());

            if (val == "") {
                val = $("#" + tempProps["key"]).attr('placeholder');
                if (!val) {
                    $("#" + tempProps["key"]).css("border", "solid 1px red");
                    redInput.push("#" + tempProps["key"]);
                    complete = false;
                }
            }
            data["properties"].push({ "key": tempProps["key"], "type": tempProps["type"], "value": val });
        }
        data["properties"].push({ "key": "CorpusId", "type": "id", "value": "1" });
        console.log(data["properties"]);
        if (complete) {
            if (redInput) {
                for (var i = 0; i < redInput.length; i++) {
                    $(redInput[i]).removeAttr("style");
                }
            }
            $http({
                method: 'post',
                url: '/api/scraper/' + scraperType,
                data: data
            })
            .success(function (response) {
                //$("#scraperNew").closest('form').find("input[type=text], textarea").val("");
                console.log(response);
                localStorage.setItem("guid", response["Guid"]);
                window.location.href = "Manage";
                
            })
            .error(function () {

            });
        } else {
            alert("Fill in all fields");
        }
    };
});