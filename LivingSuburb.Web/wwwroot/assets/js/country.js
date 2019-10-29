angular.module('countryModule', ['ui.bootstrap']    )
    .controller('listCountryController', ['$scope', '$timeout', 'countryService', function ($scope, $timeout, countryService) {
        $scope.btnClick = function () {
            $scope.istriggered = !$scope.istriggered;
        };
        $scope.orderbyList = [
            { id: 0, name: 'Name' },
            { id: 1, name: 'Code' },
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
    .component('addCountry', {
        templateUrl: '/assets/js/templates/add-country.html',
        controller: function ($element, $uibModal, countryService) {
            $ctrl = this;
            $ctrl.$onInit = function () {
                $ctrl.code = null;
                $ctrl.name = null;
            };
            $ctrl.reset = function () {
                $ctrl.code = null;
                $ctrl.name = null;
                $ctrl.addCountryForm.$setUntouched();
                $ctrl.addCountryForm.$setPristine();
            };
            $ctrl.clearClick = function () {
                $ctrl.reset();
            };
            $ctrl.submitClick = function () {
                var obj = {
                    "Code": $ctrl.code,
                    "Name": $ctrl.name
                };

                countryService.add(obj)
                    .then(function (resp) {
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
                    }, function (resp) {
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
    .directive('editCountry', ['$uibModal', '$filter', '$timeout', 'countryService', function ($uibModal, $filter, $timeout, countryService) {
        var ctrl = function ($scope) {
            $scope.backClick = function () {
                window.history.back();
            };
            $scope.updateClick = function () {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/confirm.html',
                    controller: function (message, $parentScope, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.yesClick = function () {
                            $parentScope.isprogressing = true;
                            var obj = {
                                CountryId: $parentScope.id,
                                Name: $parentScope.name,
                                Code: $parentScope.code
                            };

                            countryService.update(obj)
                                .then(function (resp) {
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

                                }, function (resp) { });
                            $uibModalInstance.close();
                        };
                        $scope.noClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        message: function () {
                            return 'Are you sure want to update \"' + $scope.name + '\" ?';
                        },
                        $parentScope: function () {
                            return $scope;
                        }
                    }
                }).result.then(function () { });
            };
            this.$onInit = function () {
                countryService.read($scope.id)
                    .then(function (resp) {
                        var data = resp.data;
                        $scope.name = data.name;
                        $scope.code = data.code;
                    }, function (resp) {
                    });
            };
        };
        return {
            restrict: 'AE',
            replace: 'true',
            scope: {
                id: '='
            },
            templateUrl: '/assets/js/templates/edit-country.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
            }
        };
    }])
    .component('listEditCountry', {
        templateUrl: '/assets/js/templates/list-edit-country.html',
        controller: function ($scope, $timeout, $uibModal, countryService) {
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
            $ctrl.countries = [];
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
                window.location = '/Management/Countries/Edit/' + obj.id;
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
                            countryService.delete(obj)
                                .then(function (resp) {
                                    $parentScope.refresh();
                                    $parentScope.isprogressing = false;
                                }, function (resp) {
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
                countryService.search(obj)
                    .then(function (resp) {
                        $ctrl.data = resp.data;
                        $ctrl.navClick($ctrl.data.selectedIndex, $ctrl.data.selectedPageNo);
                        $ctrl.isprogressing = false;
                    }, function (resp) {
                        $ctrl.isprogressing = false;
                    })
            };
            $ctrl.navClick = function (idx, no) {
                $ctrl.pageno = no;
                $ctrl.data.selectedPageNo = no;
                $ctrl.countries = [];
                if ($ctrl.data.countries != undefined) {
                    if ($ctrl.data.countries.length > 0) {
                        if (idx < $ctrl.data.countries.length) {
                            for (var i = 0; i < $ctrl.data.countries[idx].length; i++) {
                                $ctrl.countries.push($ctrl.data.countries[idx][i]);
                            }
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
    .factory('countryService', ['$http', function ($http) {
        var countryService = {
            getList: function () {
                return $http.get('/API/COUNTRIES/LIST');
            },
            search: function (data) {
                return $http.post('/API/COUNTRIES/SEARCH', data);
            },   
            search2: function (data) {
                return $http.post('/API/COUNTRIES/SEARCH-BY-KEYWORD', data);
            }, 
            add: function (data) {
                return $http.post('/API/COUNTRIES/ADD', data);
            },
            read: function (id) {
                return $http.get('/API/COUNTRIES/READ/' + id);
            },
            delete: function (data) {
                return $http.post('/API/COUNTRIES/DELETE', data);
            },
            update: function (data) {
                return $http.post('/API/COUNTRIES/UPDATE', data);
            }
        };
        return countryService;
    }]);

