angular.module('eventCategoryModule', ['eventCategoryModule', 'ui.bootstrap'])
    .factory('eventCategoryService', ['$http', function ($http) {
        var eventCategoryService = {
            getList: function () {
                return $http.get('/API/EVENTCATEGORIES/LIST');
            }
        };
        return eventCategoryService;
    }]);
