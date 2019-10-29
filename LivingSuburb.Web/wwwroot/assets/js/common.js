angular.module('commonModule', ['ui.bootstrap'])
    .factory('commonService', ['$http', function ($http) {
        var commonService = {
            geturlformat: function (data) {
                return $http.post('/API/COMMON/URL-FRIENDLY', data);
            }
        };
        return commonService;
    }]);
