app.controller("TrainningController", function ($scope, TrainningService) {
    var init = function () {

    }
    $scope.AddTrainning = function(){
        var promise = TrainningService.AddTrainning();
        promise.then(function (result) {
            alert(result.data);
            console.log(result);
        });
    }

    init();
})