var URL = "http://localhost:50631/";
app.service("TrainningService", function ($http) {
    this.AddTrainning = function () {
        var response = $http({
            method: 'GET',
            url: URL + 'Home/AddTrainning',
            header:{'content-Type':'application/json charset=UTF-8'}
        }).then(function successCallback(result){
            return result;
        },function errorCallback(result){
            return result;
        });
    }
})