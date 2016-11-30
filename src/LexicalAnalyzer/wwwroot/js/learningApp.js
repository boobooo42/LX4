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
        $scope.learnings = tempArr;
        $("#learnings").change(updateDescription);
    }

    function updateDescription() {
        $("#learningContent").empty();
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
        console.log(selected);
        if (selected) {
            $("#learningProperties").empty();
            properties = types[nameConversion[selected]]["properties"];
            for (var i = 0; i < properties.length; i++) {
                build += '<label>' + properties[i]["key"] + "(" + properties[i]["type"] + "): " + '</label><input type="text" class="form-control" id="' + properties[i]["key"] + '" placeholder="' + properties[i]["value"] + '"><hr />';
            }
        }
        $(build).appendTo("#learningProperties");
    }

    $scope.createLearningModel = function () {
        var learningModel = nameConversion[$scope.selectedLearningModel];
        console.log(learningModel);
        var tempProperties = types[learningModel]["properties"];
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
            url: '/api/learningmodel/' + learningModel,
            data: data
        })
        .success(function (response) {
            console.log(response);
        })
        .error(function () {

        });
    };

    getTypes()
});