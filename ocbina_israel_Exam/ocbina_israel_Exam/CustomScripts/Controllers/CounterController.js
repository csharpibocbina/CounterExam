/*********************************************
 * Developer    : Israel Ocbina
 * Date         : 9/23/2016
 * Description  : This is a counter that count 1...10 but
 *              it cancels >10 hits. This saves record every hits to the SQL local Database.
 *              The Database was created at runtime using 
 *              CODEFIRST approach in EF5. The DB file attached to
 *              the App_Data root folder in the project. Also, AngularJS v1.5.8.
 *              was used for AJAX, REST calls.
 * Note         : README.txt was added in this project.
 * Copyright    : Cyberocbina ™ 2016.
 * 
 * *******************************************/

/// <reference path="../../Scripts/angular.min.js" />
/// <reference path="../Modules/ngAppCounter.js" />
ngAppCounter.controller('CounterController', function ($scope, $http) {
    $scope.initCounterVal = 0;
    var RootService = "http://localhost/";
    $scope.counterClick = function () {
        $scope.isFirstLoad = false;
        var prevValue = $scope.initCounterVal;
        var url = RootService + "ocbina_israel_exam/home/IncrementValue?prevVal=" + prevValue;
        return $http.get(url)
            .then(function (response) {
                $scope.newCounterVal = JSON.parse(response.data);
                $scope.initCounterVal = $scope.newCounterVal;
            })
            .catch(function (err) {
                alert(err);
            });
    }

    $scope.initCtrVal = function () {
        $scope.isFirstLoad = true;
        var url = RootService + "ocbina_israel_exam/home/GetCurrentValue/";
        return $http.get(url)
            .then(function (response) {
                $scope.initCounterVal = JSON.parse(response.data);
                if ($scope.initCounterVal < 1)
                    $scope.newCounterVal = 0;
            })
            .catch(function (err) {
                alert(err);
            });
    }

});