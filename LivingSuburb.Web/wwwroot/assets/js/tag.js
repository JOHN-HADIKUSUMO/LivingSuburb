angular.module('tagModule', ['ui.bootstrap'])
    .controller('listTagController', ['$scope', '$uibModal', 'tagService', function ($scope, $uibModal, tagService) {
        $scope.tabClick = function (n) {
            $scope.startwith = n;
            $scope.keywords = '';
            $scope.pageno = 1;
            $scope.isprogressing = true;
            $scope.istriggered = !$scope.istriggered;
        };
        $scope.searchClick = function () {
            var temps = $scope.keywords.split(',');
            $scope.startwith = temps.length == 1 ? temps[0].substr(0, 1) : '';
            $scope.pageno = 1;
            $scope.isprogressing = true;
            $scope.istriggered = !$scope.istriggered;
        };
        $scope.addClick = function () {
            $scope.isprogressing = true;
            var obj = {
                TagGroupId:$scope.taggroupid,
                Name: $scope.keywords
            };

            tagService.create(obj)
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
                                return res.data;
                            }
                        }
                    }).result.then(function () {
                        var n = $scope.keywords.substr(0, 1);
                        $scope.tabClick(n);
                        $scope.isprogressing = true;
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
                        $scope.keywords = '';
                    }); 
                });
        };
        this.$onInit = function () {
            $scope.taggroupid = '1';
            $scope.startwith = 'A';
            $scope.pageno = 1;
            $scope.pagesize = 15;
            $scope.blocksize = 10;
            $scope.keywords = '';
            $scope.isprogressing = false;
        };
    }])
    .directive('listTag', ['tagService', '$timeout', '$uibModal', function (tagService, $timeout, $uibModal) {
        ctrl = function ($scope) {
            $scope.data = [];
            $scope.deleteClick = function (o) {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/confirm.html',
                    controller: function (message,$parentScope,$scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.yesClick = function () {
                            $scope.isprogressing = true;
                            var obj = {
                                'Id': o.tagId
                            };

                            tagService.delete(obj)
                                .then(function (res) {
                                    $parentScope.pageno = 1;
                                    $parentScope.refresh();
                                    $timeout(function () {
                                        $parentScope.navClick(0, 1);
                                        $uibModalInstance.close();
                                    }, 500)
                                }, function (res) {
                                    $uibModalInstance.close();
                                });
                        };
                        $scope.noClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        message: function () {
                            return 'Are you sure want to delete \"' + o.name + '\" ?';
                        },
                        $parentScope: function () {
                            return $scope;
                        }
                    }
                }).result.then(function () {
                    $scope.isprogressing = false;
                });
            };
            $scope.getParams = function () {
                var obj = {
                    TagGroupId: $scope.group,
                    Keywords: $scope.keywords,
                    StartWith: $scope.startwith,
                    PageNo: $scope.pageno,
                    PageSize: $scope.pagesize,
                    BlockSize: $scope.blocksize
                }
                return obj;
            };
            $scope.refresh = function () {
                $scope.isprogressing = true;
                var obj = $scope.getParams();
                tagService.search(obj)
                    .then(function (res) {
                        $scope.data = res.data;
                        $scope.navClick($scope.data.selectedIndex, $scope.data.selectedPageNo);
                        $scope.isprogressing = false;
                    }, function (res) {
                        $scope.isprogressing = false;
                    })
            };
            $scope.tags = [];
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
                $scope.tags = [];
                if ($scope.data.tags != undefined) {
                    if ($scope.data.tags.length > 0) {
                        for (var i = 0; i < $scope.data.tags[idx].length; i++) {
                            $scope.tags.push($scope.data.tags[idx][i]);
                        }
                    }
                }
            };
            this.$onInit = function () {
            };
        };
        return {
            restrict: 'E',
            replace: 'true',
            scope: {
                group: '=',
                keywords: '=',
                startwith: '=',
                pageno: '=',
                pagesize: '=',
                blocksize: '=',
                isprogressing: '=',
                istriggered: '='
            },
            templateUrl: '/assets/js/templates/list-tag.html',
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
    .factory('tagService', ['$http', function ($http) {
        var tagService = {
            search: function (data) {
                return $http.post('/API/TAGS/SEARCH', data);
            },
            searchKO: function (data) {
                return $http.post('/API/TAGS/SEARCH-BY-KEYWORD-ONLY', data);
            },
            searchKO2: function (data) {
                return $http.post('/API/TAGS/SEARCH-BY-KEYWORD-ONLY2', data);
            },
            create: function (data) {
                return $http.post('/API/TAGS/CREATE', data);
            },
            update: function (data) {
                return $http.post('/API/TAGS/UPDATE', data);
            },
            delete: function (data) {
                return $http.post('/API/TAGS/DELETE', data);
            }
        };
        return tagService;
    }]);

