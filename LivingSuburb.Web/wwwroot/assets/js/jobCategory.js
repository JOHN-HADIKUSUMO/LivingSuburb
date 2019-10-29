angular.module('jobCategoryModule', ['commonModule','ui.bootstrap']    )
    .controller('listJobCategoriesController', ['$scope', '$timeout', 'jobCategoryService', function ($scope, $timeout, jobCategoryService) {
        $scope.btnClick = function () {
            $scope.istriggered = !$scope.istriggered;
        };
        $scope.orderbyList = [
            { id: 0, name: 'Rank' },
            { id: 1, name: 'Name' },
            { id: 2, name: 'Id' },
        ];
        $scope.orderbyChange = function () {
            $scope.btnClick();
        };
        this.$onInit = function () {
            $scope.istriggered = false;
            $scope.pageno = 1;
            $scope.pagesize = 10;
            $scope.blocksize = 10;
            $scope.isprogressing = false;
            $scope.keywords = '';
            $scope.orderby = $scope.orderbyList[0];
        };
    }])
    .component('addJobCategory', {
        templateUrl: '/assets/js/templates/add-job-category.html',
        controller: function ($timeout, $uibModal,commonService,jobCategoryService) {
            $ctrl = this;
            $ctrl.$onInit = function () {
                $ctrl.name = null;
                $ctrl.nameURL = null;
                $ctrl.rank = 0;
            };
            $ctrl.onTitleMouseLeave = function () {
                if ($ctrl.timer != null)
                    $timeout.cancel($ctrl.timer);

                $ctrl.timer = $timeout(function () {
                    var obj = {
                        keywords: $ctrl.nameURL
                    };
                    commonService.geturlformat(obj)
                        .then(function (res) {
                            $ctrl.nameURL = res.data;
                        }, function (res) { });
                }, 1500);
            };
            $ctrl.reset = function () {
                $ctrl.name = null;
                $ctrl.nameURL = null;
                $ctrl.rank = 0;
                $ctrl.addJobCategoryForm.$setUntouched();
                $ctrl.addJobCategoryForm.$setPristine();
            };
            $ctrl.clearClick = function () {
                $ctrl.reset();
            };
            $ctrl.submitClick = function () {
                var obj = {
                    "Name": $ctrl.name,
                    "NameURL": $ctrl.nameURL,
                    "Rank": $ctrl.rank
                };

                jobCategoryService.add(obj)
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
                                    return obj.Name + ' has been saved.';
                                }
                            }
                        }).result.then(function () {
                            $ctrl.clearClick();
                        });
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
                                    return 'Can\'t save ' + obj.Name + ' because it has existed in our database already.';
                                }
                            }
                        }).result.then(function () {
                            $ctrl.clearClick();
                        });
                    });
            };
        }
    })
    .component('editJobCategory', {
        templateUrl: '/assets/js/templates/edit-job-category.html',
        controller: function ($timeout, $uibModal, commonService, jobCategoryService) {
            $ctrl = this;
            $ctrl.backClick = function () {
                window.history.back();
            };
            $ctrl.$onInit = function () {
                jobCategoryService.read($ctrl.id)
                    .then(function (res) {
                        var data = res.data;
                        $ctrl.name = data.name;
                        $ctrl.nameURL = data.nameURL;
                        $ctrl.rank = data.rank;
                    }, function (res) {
                    });
            };
            $ctrl.onTitleMouseLeave = function () {
                if ($ctrl.timer != null)
                    $timeout.cancel($ctrl.timer);

                $ctrl.timer = $timeout(function () {
                    var obj = {
                        keywords: $ctrl.nameURL
                    };
                    commonService.geturlformat(obj)
                        .then(function (res) {
                            $ctrl.nameURL = res.data;
                        }, function (res) { });
                }, 1500);
            };
            $ctrl.updateClick = function () {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/confirm.html',
                    controller: function (message, $parentScope, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.yesClick = function () {
                            $parentScope.isprogressing = true;
                            var obj = {
                                CategoryId: $parentScope.id,
                                Name: $parentScope.name,
                                NameURL: $parentScope.nameURL,
                                Rank: $parentScope.rank
                            };

                            jobCategoryService.update(obj)
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
                                                return '\"' + obj.Name + '\" has been updated.';
                                            }
                                        }
                                    }).result.then(function () {
                                        $parentScope.isprogressing = false;
                                        $uibModalInstance.close();
                                    });

                                }, function (res) { });
                            $uibModalInstance.close();
                        };
                        $scope.noClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        message: function () {
                            return 'Are you sure want to update \"' + $ctrl.name + '\" ?';
                        },
                        $parentScope: function () {
                            return $ctrl;
                        }
                    }
                }).result.then(function () { });
            };
        },
        bindings: {
            id: '='
        }
    })
    .component('listEditJobCategory', {
        templateUrl: '/assets/js/templates/list-edit-job-category.html',
        controller: function ($scope,$timeout,$uibModal, jobCategoryService) {
            $ctrl = this;
            $ctrl.getParams = function () {
                var obj = {
                    Keywords: $ctrl.keywords,
                    OrderBy: $ctrl.orderby.id,
                    PageNo: $ctrl.pageno,
                    PageSize: $ctrl.pagesize,
                    BlockSize: $ctrl.blocksize
                }
                return obj;
            };
            $ctrl.jobCategories = [];
            $ctrl.firstClick = function () {
                $ctrl.pageno = 1;
                $ctrl.refresh();
            };
            $ctrl.prevClick = function () {
                $ctrl.pageno = $ctrl.data.pages[0] - 1;
                $ctrl.refresh();
            };
            $ctrl.nextClick = function () {
                $ctrl.pageno = $ctrl.data.pages[$ctrl.data.pages.length - 1] + 1;
                $ctrl.refresh();
            };
            $ctrl.lastClick = function () {
                $ctrl.pageno = $ctrl.data.numberOfPages;
                $ctrl.refresh();
            };
            $ctrl.editClick = function (obj) {
                window.location = '/Management/JobCategories/Edit/' + obj.id;
            };
            $ctrl.deleteClick = function (o) {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/confirm.html',
                    controller: function (message, $parentScope, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.yesClick = function () {
                            $parentScope.isprogressing = true;
                            var obj = {
                                Id: o.id
                            };
                            jobCategoryService.delete(obj)
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
                                                return '\"' + o.name + '\" has been deleted.';
                                            }
                                        }
                                    }).result.then(function () {
                                        $parentScope.refresh();
                                        $parentScope.isprogressing = false;
                                    });
                                }, function (res) {
                                    $parentScope.isprogressing = false;
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
                            return 'Are you sure want to delete \"' + o.name + '\" ?';
                        },
                        $parentScope: function () {
                            return $ctrl;
                        }
                    }
                }).result.then(function () { });
            };
            $ctrl.refresh = function () {
                $ctrl.isprogressing = true;
                var obj = $ctrl.getParams();
                jobCategoryService.search(obj)
                    .then(function (res) {
                        $ctrl.data = res.data;
                        $ctrl.navClick($ctrl.data.selectedIndex, $ctrl.data.selectedPageNo);
                        $ctrl.isprogressing = false;
                    }, function (res) {
                        $ctrl.isprogressing = false;
                    })
            };
            $ctrl.navClick = function (idx, no) {
                $ctrl.pageno = no;
                $ctrl.data.selectedPageNo = no;
                $ctrl.jobCategories = [];
                if ($ctrl.data.jobCategories != undefined) {
                    if ($ctrl.data.jobCategories.length > 0) {
                        for (var i = 0; i < $ctrl.data.jobCategories[idx].length; i++) {
                            $ctrl.jobCategories.push($ctrl.data.jobCategories[idx][i]);
                        }
                    }
                }
            };
            $scope.$watchGroup(['$ctrl.istriggered', '$ctrl.pagesize', '$ctrl.blocksize'], function (newvalue, oldvalue, scope) {
                $timeout(function () {
                    $ctrl.refresh();        
                }, 800);
            });
        },
        bindings: {
            keywords: '=',
            orderby: '=',
            istriggered: '=',
            isprogressing: '=',
            pageno: '=',
            pagesize: '=',
            blocksize: '='
        }
    })
    .factory('jobCategoryService', ['$http', function ($http) {
        var jobCategoryService = {
            getList: function () {
                return $http.get('/API/JOB-CATEGORIES/LIST');
            },
            search: function (data) {
                return $http.post('/API/JOB-CATEGORIES/SEARCH', data);
            },            
            add: function (data) {
                return $http.post('/API/JOB-CATEGORIES/ADD', data);
            },
            read: function (id) {
                return $http.get('/API/JOB-CATEGORIES/READ/' + id);
            },
            delete: function (data) {
                return $http.post('/API/JOB-CATEGORIES/DELETE', data);
            },
            update: function (data) {
                return $http.post('/API/JOB-CATEGORIES/UPDATE', data);
            }
        };
        return jobCategoryService;
    }]);

