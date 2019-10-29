angular.module('jobSubCategoryModule', ['commonModule','jobCategoryModule','ui.bootstrap'])
    .controller('listJobSubCategoriesController', ['$scope', '$timeout', 'jobCategoryService', 'jobSubCategoryService', function ($scope, $timeout, jobCategoryService, jobSubCategoryService) {
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
            $scope.categoryList = [];
            jobCategoryService.getList()
                .then(function (res) {
                    $scope.categoryList = res.data;
            }, function (res) { });
            $scope.istriggered = false;
            $scope.pageno = 1;
            $scope.pagesize = 10;
            $scope.blocksize = 10;
            $scope.isprogressing = false;
            $scope.keywords = '';
            $timeout(function () {
                $scope.category = $scope.categoryList[0];
                $scope.orderby = $scope.orderbyList[0];
                $scope.$apply();
            }, 800);            
        };
    }])
    .component('addJobSubCategory', {
        templateUrl: '/assets/js/templates/add-job-sub-category.html',
        controller: function (commonService,$timeout,$element, $uibModal, jobCategoryService, jobSubCategoryService) {
            $ctrl = this;
            $ctrl.$onInit = function () {
                $ctrl.name = null;
                $ctrl.nameURL = null;
                $ctrl.rank = 0;
                $ctrl.categoryList = [];
                jobCategoryService.getList()
                    .then(function (res) {
                        for (var i = 1; i < res.data.length; i++) {
                            $ctrl.categoryList.push(res.data[i]);
                        }
                        $ctrl.category = $ctrl.categoryList[0];
                    }, function (res) {
                        console.log('error');
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
            $ctrl.reset = function () {
                $ctrl.name = null;
                $ctrl.nameURL = null;
                $ctrl.rank = 0;
                $ctrl.category = $ctrl.categoryList[0];
                $ctrl.addJobSubCategoryForm.$setUntouched();
                $ctrl.addJobSubCategoryForm.$setPristine();
            };
            $ctrl.clearClick = function () {
                $ctrl.reset();
            };
            $ctrl.submitClick = function () {
                var obj = {
                    "Name": $ctrl.name,
                    "NameURL": $ctrl.nameURL,
                    "Rank": $ctrl.rank,
                    "JobCategoryId": $ctrl.category.id
                };

                jobSubCategoryService.add(obj)
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
                                    return '\"' + obj.Name + '\" has been saved.';
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
                                    return 'Can\'t save \"' + obj.Name + '\" because it has existed in our database already.';
                                }
                            }
                        }).result.then(function () {
                            $ctrl.clearClick();
                        });
                    });
            };
        }
    })
    .component('editJobSubCategory', {
        templateUrl: '/assets/js/templates/edit-job-sub-category.html',
        controller: function ($timeout,commonService, $uibModal, jobCategoryService, jobSubCategoryService) {
            $ctrl = this;
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
            $ctrl.$onInit = function () {
                $ctrl.name = null;
                $ctrl.nameURL = null;
                $ctrl.rank = 0;
                $ctrl.categoryList = [];
                jobCategoryService.getList()
                    .then(function (res) {
                        for (var i = 1; i < res.data.length; i++) {
                            $ctrl.categoryList.push(res.data[i]);
                        }
                        //$ctrl.category = $ctrl.categoryList[0];
                    }, function (res) {
                        console.log('error');
                    });

                jobSubCategoryService.read($ctrl.id)
                    .then(function (res) {
                        var data = res.data;
                        $ctrl.jobSubCategoryId = data.jobSubCategoryId;
                        $ctrl.name = data.name;
                        $ctrl.nameURL = data.nameURL;
                        $ctrl.rank = data.rank;
                        for(var i = 0; i < $ctrl.categoryList.length; i++)
                        {
                            if ($ctrl.categoryList[i].id == data.jobCategoryId) {
                                $ctrl.category = $ctrl.categoryList[i];
                                break;
                            };
                        }
                    }, function (res) { });
            };
            $ctrl.backClick = function () {
                window.history.back();
            };
            $ctrl.updateClick = function () {
                var obj = {
                    "JobSubCategoryId": $ctrl.jobSubCategoryId,
                    "Name": $ctrl.name,
                    "NameURL": $ctrl.nameURL,
                    "Rank": $ctrl.rank,
                    "JobCategoryId": $ctrl.category.id
                };
                
                jobSubCategoryService.update(obj)
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
                                    return '\"' + obj.Name + '\" has been saved.';
                                }
                            }
                        }).result.then(function () {
                            
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
                                    return 'Can\'t save \"' + obj.Name + '\" because it has existed in our database already.';
                                }
                            }
                        }).result.then(function () {
                            $ctrl.clearClick();
                        });
                    });
            };
        },
        bindings: {
            id: '='
        }
    })
    .component('listEditJobSubCategory', {
        templateUrl: '/assets/js/templates/list-edit-job-subcategory.html',
        controller: function ($scope, $timeout, $uibModal, jobSubCategoryService) {
            $ctrl = this;
            $ctrl.getParams = function () {
                var obj = {
                    Keywords: $ctrl.keywords,
                    Category: $ctrl.category.id,
                    OrderBy: $ctrl.orderby.id,
                    PageNo: $ctrl.pageno,
                    PageSize: $ctrl.pagesize,
                    BlockSize: $ctrl.blocksize
                }
                return obj;
            };
            $ctrl.editClick = function (obj) {
                window.location = '/Management/JobSubCategories/Edit/' + obj.id;
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
                            jobSubCategoryService.delete(obj)
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
                jobSubCategoryService.search(obj)
                    .then(function (res) {
                        $ctrl.data = res.data;
                        $ctrl.data.selectedPageNo = $ctrl.data.selectedPageNo <= 0 ? 1 : $ctrl.data.selectedPageNo;
                        $ctrl.navClick($ctrl.data.selectedIndex, $ctrl.data.selectedPageNo);
                        $ctrl.isprogressing = false;
                    }, function (res) {
                        $ctrl.isprogressing = false;
                    });
            };
            $ctrl.navClick = function (idx, no) {
                $ctrl.pageno = no;
                $ctrl.data.selectedPageNo = no;
                $ctrl.jobSubCategories = [];
                if ($ctrl.data.jobSubCategories != undefined) {
                    if ($ctrl.data.jobSubCategories.length > 0) {
                        var list = $ctrl.data.jobSubCategories[idx];
                        for (var i = 0; i < list.length; i++) {
                            $ctrl.jobSubCategories.push(list[i]);
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
            category: '=',
            orderby: '=',
            istriggered: '=',
            isprogressing: '=',
            pageno: '=',
            pagesize: '=',
            blocksize: '='
        }
    })
    .factory('jobSubCategoryService', ['$http', function ($http) {
        var jobSubCategoryService = {
            getList: function (data) {
                return $http.get('/API/JOB-SUB-CATEGORIES/LIST/' + data);
            },
            search: function (data) {
                return $http.post('/API/JOB-SUB-CATEGORIES/SEARCH', data);
            }, 
            add: function (data) {
                return $http.post('/API/JOB-SUB-CATEGORIES/ADD', data);
            },
            delete: function (data) {
                return $http.post('/API/JOB-SUB-CATEGORIES/DELETE', data);
            },
            read: function (id) {
                return $http.get('/API/JOB-SUB-CATEGORIES/READ/' + id);
            },
            update: function (data) {
                return $http.post('/API/JOB-SUB-CATEGORIES/UPDATE', data);
            }
        };
        return jobSubCategoryService;
    }]);
