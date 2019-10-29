angular.module('eventTypeModule', ['ui.bootstrap'])
    .factory('eventTypeService', ['$http', function ($http) {
        var eventTypeService = {
            getList: function (id) {
                return $http.get('/API/EVENTTYPES/LIST/' + id);
            }
        };
        return eventTypeService;
    }]);
