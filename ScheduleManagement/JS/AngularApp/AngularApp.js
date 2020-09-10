/// <reference path="C:\Users\Karthikeyan\Projects\Zuber Demo\TranningManagement\TrainningManagement\TrainningManagement\Pages/Tranning.html" />
var app = angular.module("AngularApp", ['ngRoute'])

app.config(['$routeProvider', '$locationProvider', function ($routeProvider,$locationProvider) {

    $locationProvider.html5Mode(true);

    $routeProvider.when('', {
        templateUrl: '../Pages/Tranning.html',
        controller: 'TrainningController',
        resolve: {
            ctrlOptions: function () {
                actionName: "AddTraining";
            }
        }
    })

}])