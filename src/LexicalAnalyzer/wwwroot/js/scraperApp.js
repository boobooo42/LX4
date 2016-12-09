var manageApp = angular.module("scraperApp", ['ngRoute']);

manageApp.controller("ScraperController", function ($scope, $http, $interval) {

    var types;
    var nameConversion = {};
    var existingCorpora = [];

    $scope.init = function () {
        getTypes();
        getExistingCorpora();
    }

    $scope.init();
    
    function getExistingCorpora() {
        $http({
            method: 'get',
            url: UrlContent( '/api/corpus/')
        })
        .success(function (response) {
            console.log(response);
            var tempCorpora = [];
            for (var i = 0; i < response.length; i++) {
                existingCorpora.push(response[i]);
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
            url: UrlContent( '/api/scraper/types')
        })
            .success(function (response) {
                if (response !== 'undefined') {
                    for (var key in response) {
                        types[response[key]["type"]] = response[key];
                        nameConversion[response[key]["displayName"]] = response[key]["type"];
                        nameConversion[response[key]["type"]] = response[key]["displayName"];

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
        
    }

    var redInput = [];
    $scope.createScraper = function () {
        if (redInput) {
            for (var i = 0; i < redInput.length; i++) {
                $(redInput[i]).removeAttr("style");
            }
        }
        var data = {
            "status": "init",
            "progress": 0,
            "priority": 0,
            "properties": []
        }
        var complete = true

        // Check Scraper Name Field
        var name = $("#scraperName").val().trim();
        if (name == "") {
            redInput.push("#scraperName");
            $("#scraperName").css("border", "solid 1px red");
            complete = false;
        } else {
            data["properties"].push({ "key": "UserGivenName", "type": "UserGivenName", "value": name });
        }

        // Check Scraper Type Field
        var scraperType = nameConversion[$scope.selectedScraper];
        if (scraperType) {
            var tempProperties = types[scraperType]["properties"];
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
        } else {
            complete = false;
            $("#scrapers").css("border", "solid 1px red");
            redInput.push("#scrapers");
        }

        // Check Corpus Field
        if ($scope.selectedCorpora) {
            for (var key in existingCorpora) {
                if (existingCorpora[key]["name"] == $scope.selectedCorpora) {
                    data["properties"].push({ "key": "corpus", "type": "id", "value": existingCorpora[key]["id"] });
                }
            }
        } else {
            complete = false;
            $("#corpora").css("border", "solid 1px red");
            redInput.push("#corpora");
        }
        console.log(data["properties"]);
        if (complete) {
            $http({
                method: 'post',
                url: UrlContent( '/api/scraper/' + scraperType),
                data: data
            })
            .success(function (response) {
                console.log(response);
                localStorage.setItem("guid", response["Guid"]);
                window.location.href = "Manage";
                
            })
            .error(function () {

            });
        } else {
            alert("Fill in all fields");
        }
    }
    
    //DOM manipulation
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
});