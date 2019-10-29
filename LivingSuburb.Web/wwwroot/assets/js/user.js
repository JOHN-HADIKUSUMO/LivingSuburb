angular.module('userModule', ['ui.bootstrap'])
    .controller('listUserController', ['$scope', '$uibModal', 'userService', function ($scope, $uibModal, userService) {
        $scope.searchClick = function () {
            $scope.pageno = 1;
            $scope.isprogressing = true;
            $scope.istriggered = !$scope.istriggered;
        };
        this.$onInit = function () {
            $scope.orderby = '1';
            $scope.pageno = 1;
            $scope.pagesize = 15;
            $scope.blocksize = 10;
            $scope.keywords = '';
            $scope.isprogressing = false;
        };
    }])
    .directive('listUser', ['userService', '$timeout', '$uibModal', function (userService, $timeout, $uibModal) {
        ctrl = function ($scope) {
            $scope.data = [];
            $scope.sendClick = function (o) {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/send-user-message.html',
                    controller: function ($parentScope, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.sendClick = function () {
                            $parentScope.isprogressing = true;
                            $uibModalInstance.close();
                        };
                        $scope.cancelClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        $parentScope: function () {
                            return $scope;
                        }
                    }
                }).result.then(function () { });
            };
            $scope.deleteClick = function (o) {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/confirm.html',
                    controller: function (message, $parentScope, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.yesClick = function () {
                            $parentScope.isprogressing = true;
                            var obj = {
                                Id: o.id
                            };
                            userService.delete(obj)
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
                                            return '\"' + o.fullname + '\" has been deleted.';
                                        }
                                    }
                                    }).result.then(function () {
                                        $parentScope.refresh();
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
                                                    return res.data;
                                                }
                                            }
                                        }).result.then(function () {
                                            $scope.isprogressing = false;
                                        });
                                    });
                            $uibModalInstance.close();
                        };
                        $scope.noClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        message: function () {
                            return 'Are you sure want to delete \"' + o.fullname + '\" ?';
                        },
                        $parentScope: function () {
                            return $scope;
                        }
                    }
                }).result.then(function () { });
            };
            $scope.getParams = function () {
                var obj = {
                    Keywords: $scope.keywords,
                    OrderBy: $scope.orderby,
                    PageNo: $scope.pageno,
                    PageSize: $scope.pagesize,
                    BlockSize: $scope.blocksize
                }
                return obj;
            };
            $scope.refresh = function () {
                $scope.isprogressing = true;
                var obj = $scope.getParams();
                userService.search(obj)
                    .then(function (res) {
                        $scope.data = res.data;
                        $scope.navClick($scope.data.selectedIndex, $scope.data.selectedPageNo);
                        $scope.isprogressing = false;
                    },
                    function (res)
                    {
                        $scope.isprogressing = false;
                    });
            };
            $scope.users = [];
            $scope.firstClick = function () {
                $scope.pageno = 1;
                $scope.refresh();
            };
            $scope.prevClick = function () {
                $scope.pageno = $scope.data.pages[0] - 1;
                $scope.refresh();
            };
            $scope.nextClick = function () {
                $scope.pageno = $scope.data.pages[$scope.data.pages.length - 1] + 1;
                $scope.refresh();
            };
            $scope.lastClick = function () {
                $scope.pageno = $scope.data.numberOfPages;
                $scope.refresh();
            };
            $scope.navClick = function (idx, no) {
                $scope.pageno = no;
                $scope.data.selectedPageNo = no;
                $scope.users = [];
                if ($scope.data.users != undefined) {
                    if ($scope.data.users.length > 0) {
                        for (var i = 0; i < $scope.data.users[idx].length; i++) {
                            $scope.users.push($scope.data.users[idx][i]);
                        }
                    }
                }
            };
            this.$onInit = function () {
                $scope.pageno = 1;
                $scope.pagesize = 10;
                $scope.blocksize = 10;
                $scope.keywords = '';
                $scope.istriggered = true;
                $scope.isprogressing = false;
            };
        };
        return {
            restrict: 'E',
            replace: 'true',
            scope: {
                keywords: '=',
                orderby: '=',
                pageno: '=',
                pagesize: '=',
                blocksize: '=',
                isprogressing: '=',
                istriggered: '='
            },
            templateUrl: '/assets/js/templates/list-user.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
                scope.$watchGroup(['istriggered', 'pagesize', 'blocksize'], function (newvalue, oldvalue, scope) {
                    $timeout(function () {
                        scope.refresh();
                    }, 800);
                });
            }
        };
    }])
    .factory('userService', ['$http', function ($http) {
        var userService = {
            search: function (data) {
                return $http.post('/API/USERS/SEARCH', data);
            },
            update: function (data) {
                return $http.post('/API/USERS/UPDATE', data);
            },
            delete: function (data) {
                return $http.post('/API/USERS/DELETE', data);
            }
        };
        return userService;
    }]);

