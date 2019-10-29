angular.module('avatarModule', ['fileModule', 'ui.bootstrap'])
    .controller('avatarController', ['$scope', '$http', '$timeout', 'avatarService', '$uibModal', function ($scope, $http, $timeout, avatarService, $uibModal) {
        $scope.saveClick = function () {
            $scope.isprogressing = true;
            var obj = {
                GUID: $scope.data.guid,
                Filename: $scope.data.filename,
                Extension: $scope.data.extension
            };

            avatarService.update(obj)
            .then(function (res) {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/info.html',
                    controller: function (message, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.closeClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        message: function () {
                            return 'Your avatar has been updated.';
                        }
                    }
                }).result.then(function () {
                    $scope.isChanged = false;
                    $scope.isprogressing = false;
                });
            },
            function (res) {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/error.html',
                    controller: function (message, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.closeClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        message: function () {
                            return res.statusText;
                        }
                    }
                }).result.then(function () {
                });
            });
        };
        $scope.uploadClick = function () {
            $("input[type='file']").trigger("click");
        };
        $scope.isprogressing = false;
        $scope.isChanged = false;
        $scope.url = null;
        $scope.file = null;
        $scope.$on('selectedFile', function (event, args) {
            $scope.$apply(function () {
                $scope.isprogressing = true;
                $scope.file = args.file;
                var apiURL = "/API/AVATAR/UPLOAD";
                $http({
                    method: 'POST',
                    url: apiURL,
                    headers: {
                        'Content-Type': undefined
                    },
                    transformRequest: function (data) {
                        var formData = new FormData();
                        formData.append("avatar", data.file);
                        return formData;
                    },
                    data: { file: $scope.file }
                }).then(function (res) {
                    $scope.data = res.data;
                    $scope.url = $scope.data.url;
                    $scope.isChanged = true;
                    $scope.isprogressing = false;
                },
                function (res) {
                    $uibModal.open({
                        templateUrl: '/assets/js/templates/error.html',
                        controller: function (message, $scope, $uibModalInstance) {
                            $scope.content = message;
                            $scope.closeClick = function () {
                                $uibModalInstance.close();
                            };
                        },
                        resolve: {
                            message: function () {
                                return res.data;
                            }
                        }
                    }).result.then(function () {
                        $scope.isprogressing = false;
                    });
                });
            });
        });
        this.$onInit = function () {
            avatarService.read()
            .then(function (res) {
                $scope.url = res.data;
            },
            function (res) {
            });
        };
    }])
    .factory('avatarService', ['$http', function ($http) {
        var avatarService = {
            read: function () {
                return $http.get('/API/AVATAR/READ');
            },
            update: function (data) {
                return $http.post('/API/AVATAR/UPDATE', data);
            }
        };
        return avatarService;
    }]);

