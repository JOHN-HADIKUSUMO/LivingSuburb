angular.module('stateModule', [])
    .factory('stateService', ['$http', function ($http) {
        var stateService = {
            getList: function (data) {
                return $http.get('/API/STATES/LIST');
            },
            getList2: function (data) {
                return $http.get('/API/STATES/LIST2');
            }
        };
        return stateService;
    }]);

