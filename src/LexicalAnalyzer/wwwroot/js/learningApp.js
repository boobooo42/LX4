var manageApp = angular.module("learningApp", ['ngRoute']);

manageApp.controller("LearningController", function ($scope, $http) {
    var types, nameConversion = {};
    function getTypes() {
        types = {};
        $http({
            method: 'get',
            url: '/api/learningmodel/types'
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
        $scope.learnings = tempArr;
        $("#learnings").change(updateDescription);
    }

    function updateDescription() {
        $("#learningContent").empty();
        redInput = [];
        var selected = $scope.selectedLearningModel;
        if (selected) {
            var localBuild = "";
            for (var key in types[nameConversion[selected]]) {
                if (key !== "properties" && key !== "displayName") {
                    localBuild += "<div><h4>" + key + "</h4>";
                    localBuild += types[nameConversion[selected]][key] + "<hr /></div>";
                }
            }
            $(localBuild).appendTo("#learningContent");
        }
        listProperties();
    }

    function listProperties() {
        build = "";
        var selected = $scope.selectedLearningModel;
        if (selected) {
            $("#learningProperties").empty();
            properties = types[nameConversion[selected]]["properties"];
            for (var i = 0; i < properties.length; i++) {
                build += '<label>' + properties[i]["key"] + "(" + properties[i]["type"] + "): " + '</label><input type="text" class="form-control" id="' + properties[i]["key"] + '" placeholder="' + properties[i]["value"] + '"><hr />';
            }
        }
        $(build).appendTo("#learningProperties");
    }

    var redInput = [];
    $scope.createLearningModel = function () {
        var learningModel = nameConversion[$scope.selectedLearningModel];
        var tempProperties = types[learningModel]["properties"];
        var data = {
            "status": "init",
            "progress": 0,
            "priority": 0,
            "properties": []
        }
        var complete = true;
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
        if (complete) {
            if (redInput) {
                for (var i = 0; i < redInput.length; i++) {
                    $(redInput[i]).removeAttr("style");
                }
            }
            $http({
                method: 'post',
                url: '/api/learningmodel/' + learningModel,
                data: data
            })
            .success(function (response) {
                $("#learningForm").closest('form').find("input[type=text], textarea").val("");
                console.log(response);
            })
            .error(function () {
                console.log(response);
            });
        }
    };

    getTypes()
});