angular.module('carouselModule', ['galleriesModule', 'ui.bootstrap'])
    .controller('carouselListController', ['$scope', '$timeout', 'carouselService', function ($scope, $timeout, carouselService) {
        var timer = null;
        $scope.onSelected = function (o) {
            $scope.keywords = o;
            $scope.keywordlist = [];
        };
        $scope.onKeyup = function () {
            $timeout.cancel(timer);
            timer = $timeout(function () {
                $scope.keywordlist = [];
                if ($scope.keywords != '') {
                    $scope.isprogressing = true;
                    var obj = {
                        groupid: 1,
                        keywords: $scope.keywords,
                        take: 5
                    };
                    tagService.searchKO(obj)
                        .then(function (res) {
                            $scope.keywordlist = res.data;
                            $scope.isprogressing = false;
                        }, function (res) {
                        });
                }
            }, 600);
        };
        $scope.btnClick = function () {
            $scope.istriggered = !$scope.istriggered;
        };
        this.$onInit = function () {
            $scope.istriggered = false;
            $scope.pageno = 1;
            $scope.pagesize = 5;
            $scope.blocksize = 10;
            $scope.isprogressing = false;
            $scope.keywordlist = [];
            $scope.keywords = '';
        };
    }])
    .directive('addCarousel', ['$http', '$uibModal', '$filter', '$timeout', 'carouselService', 'galleriesService', function ($http, $uibModal, $filter, $timeout, carouselService, galleriesService) {
        var ctrl = function ($scope) {
            $scope.clearClick = function () {
                $scope.url = '/assets/img/galleries/default.jpg';
                $scope.location = undefined;
                $scope.proverb = undefined;
                $scope.source = undefined;
                $scope.publishedDate = undefined;
                $scope.addCarouselForm.$setUntouched();
                $scope.addCarouselForm.$setPristine();
            };
            $scope.uploadClick = function () {
                $("input[type='file']").trigger("click");
            };
            $scope.isprogressing = false;
            $scope.isChanged = false;
            $scope.url = '/assets/img/galleries/default.jpg';
            $scope.file = null;
            $scope.submitClick = function () {
                var obj = {
                    ImageURL: $scope.url,
                    Location: $scope.location,
                    Proverb: $scope.proverb,
                    Source: $scope.source,
                    PublishedDate: $filter('date')($scope.publishedDate, "yyyy-MM-dd")
                };

                carouselService.add(obj)
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
                                    return 'New item has been saved.';
                                }
                            }
                        }).result.then(function () {
                            $scope.clearClick();
                        });
                    }, function (res) { });
            };
            $scope.$on('selectedFile', function (event, args) {
                $scope.$apply(function () {
                    $scope.isprogressing = true;
                    $scope.file = args.file;
                    var apiURL = "/API/CAROUSELS/UPLOAD";
                    $http({
                        method: 'POST',
                        url: apiURL,
                        headers: {
                            'Content-Type': undefined
                        },
                        transformRequest: function (data) {
                            var formData = new FormData();
                            formData.append("carousel", data.file);
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
        };
        return {
            restrict: 'AE',
            replace: 'true',
            scope: {},
            templateUrl: '/assets/js/templates/add-carousel.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
                el.find('#publishedDate').datepicker({
                    dateFormat: 'yy-mm-dd'
                });
            }
        };
    }])
    .directive('editCarousel', ['$http', '$uibModal', '$filter', '$timeout', 'carouselService', 'galleriesService', function ($http, $uibModal, $filter, $timeout, carouselService, galleriesService) {
        var ctrl = function ($scope) {
            $scope.uploadClick = function () {
                $("input[type='file']").trigger("click");
            };
            $scope.$on('selectedFile', function (event, args) {
                $scope.$apply(function () {
                    $scope.isprogressing = true;
                    $scope.file = args.file;
                    var apiURL = "/API/CAROUSELS/UPLOAD";
                    $http({
                        method: 'POST',
                        url: apiURL,
                        headers: {
                            'Content-Type': undefined
                        },
                        transformRequest: function (data) {
                            var formData = new FormData();
                            formData.append("carousel", data.file);
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
            $scope.url = '/assets/img/galleries/default.jpg';
            $scope.file = null;
            $scope.updateClick = function () {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/confirm.html',
                    controller: function (message, $parentScope, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.yesClick = function () {
                            $parentScope.isprogressing = true;
                            var obj = {
                                Id: $parentScope.id,
                                GUID: $parentScope.data.guid,
                                Filename: $parentScope.data.filename,
                                Extension: $parentScope.data.extension,
                                Location: $parentScope.location,
                                Proverb: $parentScope.proverb,
                                Source: $parentScope.source,
                                PublishedDate: $filter('date')($parentScope.publishedDate, "yyyy-MM-dd")
                            };

                            carouselService.update(obj)
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
                                                return 'Item id ' + $parentScope.id + ' has been updated.';
                                            }
                                        }
                                    }).result.then(function () {
                                        $parentScope.isprogressing = false;
                                        $uibModalInstance.close();
                                    });
                                }, function (res) {
                                    console.log(JSON.stringify(res));
                                });
                            $uibModalInstance.close();
                        };
                        $scope.noClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        message: function () {
                            return 'Are you sure want to update item id \"' + $scope.id + '\" ?';
                        },
                        $parentScope: function () {
                            return $scope;
                        }
                    }
                }).result.then(function () { });
            };
            $scope.backClick = function () {
                window.history.back();
            };
            this.$onInit = function () {
                carouselService.read($scope.id)
                .then(function (res) {
                        var data = res.data;
                        $scope.url = data.url;
                        $scope.location = data.location;
                        $scope.proverb = data.proverb;
                        $scope.source = data.source;
                        $scope.publishedDate = $filter('date')(data.publishedDate, "yyyy-MM-dd");
                    }, function (res) {
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
                                    return 'Id ' + $scope.id + ' can not be found.';
                                }
                            }
                        }).result.then(function () { });
                });
            };
        };
        return {
            restrict: 'AE',
            replace: 'true',
            scope: {
                id: '='
            },
            templateUrl: '/assets/js/templates/edit-carousel.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
                el.find('#publishedDate').datepicker({
                    dateFormat: 'yy-mm-dd'
                });
            }
        };
    }])
    .directive('listEditCarousel', ['carouselService', '$timeout', '$uibModal', function (carouselService, $timeout, $uibModal) {
        ctrl = function ($scope) {
            $scope.data = [];
            $scope.getParams = function () {
                var obj = {
                    Keywords: $scope.keywords,
                    PageNo: $scope.pageno,
                    PageSize: $scope.pagesize,
                    BlockSize: $scope.blocksize
                }
                return obj;
            };
            $scope.editClick = function (obj) {
                window.location = '/Management/Carousels/Edit/' + obj.id;
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

                            carouselService.delete(obj)
                                .then(function (res) {
                                    $parentScope.refresh();
                                    $parentScope.isprogressing = false;
                                }, function (res) {
                                    $parentScope.isprogressing = false;
                                });
                            $uibModalInstance.close();
                        };
                        $scope.noClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        message: function () {
                            return 'Are you sure want to delete \"' + o.title + '\" ?';
                        },
                        $parentScope: function () {
                            return $scope;
                        }
                    }
                }).result.then(function () { });
            };
            $scope.refresh = function () {
                $scope.isprogressing = true;
                var obj = $scope.getParams();
                carouselService.search2(obj)
                    .then(function (res) {
                        $scope.data = res.data;
                        $scope.navClick($scope.data.selectedIndex, $scope.data.selectedPageNo);
                        $scope.isprogressing = false;
                    }, function (res) {
                        $scope.isprogressing = false;
                    })
            };
            $scope.carousels = [];
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
                $scope.carousels = [];
                if ($scope.data.carousels != undefined) {
                    if ($scope.data.carousels.length > 0) {
                        for (var i = 0; i < $scope.data.carousels[idx].length; i++) {
                            $scope.carousels.push($scope.data.carousels[idx][i]);
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
                pageno: '=',
                pagesize: '=',
                blocksize: '=',
                isprogressing: '=',
                istriggered: '='
            },
            templateUrl: '/assets/js/templates/list-edit-carousel.html',
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
    .factory('carouselService', ['$http', function ($http) {
        var carouselService = {
            add: function (data) {
                return $http.post('/API/CAROUSELS/CREATE', data);
            },
            update: function (data) {
                return $http.post('/API/CAROUSELS/UPDATE', data);
            },
            delete: function (data) {
                return $http.post('/API/CAROUSELS/DELETE', data);
            },
            read: function (id) {
                return $http.get('/API/CAROUSELS/READ/' + id);
            },
            search2: function (data) {
                return $http.post('/API/CAROUSELS/SEARCH2', data);
            },
        };
        return carouselService;
    }]);
