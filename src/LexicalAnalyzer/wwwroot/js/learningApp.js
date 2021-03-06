var manageApp = angular.module("learningApp", ['ngRoute']);

manageApp.controller("LearningController", function ($scope, $http) {
    var types = {};
    var nameConversion = {};
    var existingCorpora = [];

    function getTypes() {
        getExistingCorpora();
        types = {};
        $http({
            method: 'get',
            url: UrlContent( '/api/learningmodel/types')
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

    function getExistingCorpora() {
        console.log("corpora");
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
            console.log(existingCorpora);
            $scope.corpora = tempCorpora;
        })
        .error(function () {

        });
    }

    var redInput = [];
    $scope.createLearningModel = function () {
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
        var complete = true;

        // checks name field
        var name = $("#learningName").val().trim();
        if (name == "") {
            redInput.push("#learningName");
            $("#learningName").css("border", "solid 1px red");
            complete = false;
        } else {
            data["properties"].push({ "key": "UserGivenName", "type": "UserGivenName", "value": name });
        }

        // checks learning model field
        if ($scope.selectedLearningModel) {
            var learningModel = nameConversion[$scope.selectedLearningModel];
            var tempProperties = types[learningModel]["properties"];

            // checks all property fields
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
            $("#learnings").css("border", "solid 1px red");
            redInput.push("#learnings");
        }

        // checks corpus field
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

        if (complete) {
            $http({
                method: 'post',
                url: UrlContent( '/api/learningmodel/' + learningModel),
                data: data
            })
            .success(function (response) {
                localStorage.setItem("guid", response["Guid"]);
                window.location.href = "ManageLearning";
                console.log(response);
            })
            .error(function () {
                console.log(response);
            });
        } else {
            alert("Fill in all fields");
        }
    };

    // Page Setup
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

    //initilizes page
    getTypes();
});