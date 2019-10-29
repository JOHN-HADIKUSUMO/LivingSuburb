angular.module('suburbModule', ['commonModule','ui.bootstrap', 'stateModule'])
    .controller('listSuburbController', ['$timeout', '$scope', 'stateService', function ($timeout, $scope, stateService) {
        $scope.tabClick = function (n) {
            $scope.startwith = n;
        };
        $scope.state = { "id": 1, "name": "New South Wales" };
        $scope.startwith = 'A';
        $scope.pageno = 1;
        $scope.pagesize = 15;
        $scope.blocksize = 10;
        $scope.isprogressing = false;

        this.$onInit = function () {
            $scope.stateList = [];
            stateService.getList()
                .then(function (res) {
                    $scope.stateList = res.data;
                    $scope.stateList.splice(0, 1);
                }, function (res) {
                    console.log('error');
                });
        };
    }])
    .component('addBulkSuburb', {
        templateUrl: '/assets/js/templates/add-bulk-suburb.html',
        controller: function ($element, $uibModal, suburbService) {
            $ctrl = this;
            $ctrl.element = $($element[0]);
            $ctrl.$onInit = function () {
                var n = $ctrl.element.find("input[name=established]");
                n.datepicker({
                    dateFormat: "yy-mm-dd"
                });
            };
            $ctrl.reset = function () {
                $ctrl.names = undefined;
                $ctrl.state = undefined;
                $ctrl.established = undefined;
                $ctrl.addSuburbForm.$setUntouched();
                $ctrl.addSuburbForm.$setPristine();
                $ctrl.addSuburbForm.nameurl.$setUntouched();
                $ctrl.addSuburbForm.nameurl.$setPristine();
            };
            $ctrl.btnClick = function () {
                var obj = {
                    "names": ($ctrl.names == undefined || $ctrl.names == null) ? '' : $ctrl.names,
                    "stateid": $ctrl.state,
                    "established": ($ctrl.established == undefined || $ctrl.established == null) ? '' : $ctrl.established
                };

                $ctrl.success = 'The new suburbs have been saved.';
                suburbService.addBulk(obj)
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
                                    return $ctrl.success;
                                }
                            }
                        }).result.then(function () {
                            $ctrl.reset();
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
                                    return res.data;
                                }
                            }
                        }).result.then(function () {
                        });
                    });
            };
        }
    })
    .component('addSuburb', {
        templateUrl: '/assets/js/templates/add-suburb.html',
        controller: function ($timeout, $element, $uibModal, suburbService, commonService) {
            $ctrl = this;
            $ctrl.element = $($element[0]);
            $ctrl.$onInit = function () {
                var n = $ctrl.element.find("input[name=established]");
                n.datepicker({
                    dateFormat: "yy-mm-dd"
                });
            };
            $ctrl.onTitleMouseLeave = function () {
                if ($ctrl.timer != null)
                    $timeout.cancel($ctrl.timer);

                $ctrl.timer = $timeout(function () {
                    var obj = {
                        keywords: $ctrl.nameurl
                    };
                    commonService.geturlformat(obj)
                        .then(function (res) {
                            $ctrl.nameurl = res.data;
                        }, function (res) { });
                }, 1500);
            };
            $ctrl.reset = function () {
                $ctrl.name = undefined;
                $ctrl.nameurl = undefined;
                $ctrl.state = undefined;
                $ctrl.established = undefined;
                $ctrl.addSuburbForm.$setUntouched();
                $ctrl.addSuburbForm.$setPristine();
                $ctrl.addSuburbForm.nameurl.$setUntouched();
                $ctrl.addSuburbForm.nameurl.$setPristine();
            };
            $ctrl.btnClick = function () {
                var obj = {
                    "name": ($ctrl.name == undefined || $ctrl.name == null) ? '' : $ctrl.name,
                    "nameurl": ($ctrl.nameurl == undefined || $ctrl.nameurl == null) ? '' : $ctrl.nameurl,
                    "stateid": $ctrl.state,
                    "established": ($ctrl.established == undefined || $ctrl.established == null) ? '' : $ctrl.established
                };

                $ctrl.success = 'The new suburb has been saved.';
                suburbService.add(obj)
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
                                    return $ctrl.success;
                                }
                            }
                        }).result.then(function () {
                            $ctrl.reset();
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
                                    return res.data;
                                }
                            }
                        }).result.then(function () {
                        });
                    });
            };
        }
    })
    .component('editSuburb', {
        templateUrl: '/assets/js/templates/edit-suburb.html',
        controller: function ($element, $scope, $timeout, $filter, $uibModal, suburbService, stateService, commonService) {
            $ctrl = this;
            $ctrl.element = $($element[0]);
            $ctrl.$onInit = function () {
                stateService.getList()
                    .then(function (res) {
                        $ctrl.stateList = res.data;
                        $ctrl.stateList.splice(0, 1);
                    }, function (res) {
                        console.log('error');
                    });

                suburbService.read($ctrl.id)
                    .then(function (res) {
                        var data = res.data;
                        $ctrl.name = data.name;
                        $ctrl.nameurl = data.nameURL;
                        $ctrl.state = { "id": data.stateId, "name": "" };
                        var dt = $filter('date')(data.established, "yyyy-MM-dd");
                        $ctrl.established = dt == '10000-01-01' ? '' : dt;

                        $timeout(function () {
                            $scope.$apply();
                        }, 500);

                    }, function (res) {

                    });

                var n = $ctrl.element.find("input[name=established]");
                n.datepicker({
                    dateFormat: "yy-mm-dd"
                });
            };
            $ctrl.onTitleMouseLeave = function () {
                if ($ctrl.timer != null)
                    $timeout.cancel($ctrl.timer);

                $ctrl.timer = $timeout(function () {
                    var obj = {
                        keywords: $ctrl.nameurl
                    };
                    commonService.geturlformat(obj)
                        .then(function (res) {
                            $ctrl.nameurl = res.data;
                        }, function (res) { });
                }, 1500);
            };
            $ctrl.reset = function () {
                $ctrl.name = undefined;
                $ctrl.nameurl = undefined;
                $ctrl.state = undefined;
                $ctrl.established = undefined;
                $ctrl.editSuburbForm.$setUntouched();
                $ctrl.editSuburbForm.$setPristine();
                $ctrl.editSuburbForm.nameurl.$setUntouched();
                $ctrl.editSuburbForm.nameurl.$setPristine();
            };
            $ctrl.btnClick = function () {
                var obj = {
                    "SuburbId": $ctrl.id,
                    "Name": ($ctrl.name == undefined || $ctrl.name == null) ? '' : $ctrl.name,
                    "NameURL": ($ctrl.nameurl == undefined || $ctrl.nameurl == null) ? '' : $ctrl.nameurl,
                    "StateId": $ctrl.state.id,
                    "Established": ($ctrl.established == undefined || $ctrl.established == null) ? '' : $ctrl.established
                };

                $ctrl.success = 'The suburb has been updated.';
                suburbService.update(obj)
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
                                    return $ctrl.success;
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
                                    return res.data;
                                }
                            }
                        }).result.then(function () {
                        });
                    });
            };
            $ctrl.backClick = function () {
                window.history.back();
            };
        },
        bindings: {
            id: '='
        }
    })
    .directive('listSuburb', ['suburbService', '$timeout', '$uibModal', function (suburbService, $timeout, $uibModal) {
        ctrl = function ($scope) {
            $scope.data = [];
            $scope.editClick = function (o) {
                window.location = '/management/suburbs/edit/' + o;
            };
            $scope.deleteClick = function (o) {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/confirm.html',
                    controller: function (message, $parentScope, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.yesClick = function () {
                            $parentScope.status = true;
                            var obj = {
                                'id': o.suburbId
                            };
                            suburbService.delete(obj)
                                .then(function (res) {
                                    $parentScope.status = false;
                                }, function (res) {
                                    $parentScope.status = false;
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
                            return $scope;
                        }
                    }
                }).result.then(function () { });
            };
            $scope.refresh = function () {
                $scope.status = true;
                var obj = {
                    State: $scope.state.id,
                    StartWith: $scope.startwith,
                    PageNo: $scope.pageno,
                    PageSize: $scope.pagesize,
                    BlockSize: $scope.blocksize
                };

                suburbService.searchList(obj)
                .then(function (res) {
                    $scope.data = res.data;
                    $scope.navClick($scope.data.selectedIndex, $scope.data.selectedPageNo);
                    $scope.status = false;
                },
                function (res) {
                    $scope.status = false;
                });
            };
            $scope.suburbs = [];
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
                $scope.suburbs = [];
                if ($scope.data.suburbs.length > 0) {
                    for (var i = 0; i < $scope.data.suburbs[idx].length; i++) {
                        $scope.suburbs.push($scope.data.suburbs[idx][i]);
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
                state: '=',
                startwith: '=',
                pageno: '=',
                pagesize: '=',
                blocksize: '=',
                status: '='
            },
            templateUrl: '/assets/js/templates/list-suburb.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
                scope.$watchGroup(['state', 'startwith', 'pagesize', 'blocksize'], function (newvalue, oldvalue, scope) {
                    scope.refresh();
                });
            }
        };
    }])
    .directive('searchYourSuburb', ['$filter', '$timeout', '$window', 'suburbService', function ($filter, $timeout, $window, suburbService) {
        var ctrl = function ($scope, $element) {
            var timer = null;
            $scope.suburbs = [];
            $scope.onKeyup = function () {
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.suburb = null;
                    $scope.searching = true;
                    var obj = {
                        keywords: $scope.keywords,
                        take: 5
                    };
                    suburbService.search(obj)
                        .then(function (res) {
                            $scope.suburbs = res.data;
                            $scope.searching = false;
                        }, function (res) {
                            console.log(res.status);
                        });
                }, 600);
            };
            $scope.searching = false;
            $scope.textbox = null;
            $scope.listbox = null;
            $scope.suburb = null;
            $scope.keywords = '';
            $scope.onSelected = function (o) {
                $scope.suburb = o;
                $scope.keywords = $scope.suburb.name + ', ' + $scope.suburb.state;
                $scope.suburbs = [];
            };
            $scope.reRender = function () {
                var width = $scope.textbox.parent().width();
                $scope.listbox.width(width - 2);
            };
            this.$onInit = function () {
                $timeout(function () {
                    $scope.reRender();
                }, 500);
            };
            $scope.btnClick = function () {
                window.location = '/Suburbs/' + $scope.suburb.state + '/' + $scope.suburb.nameURL;
            };
        };

        return {
            restrict: 'AE',
            replace: 'true',
            scope: {},
            templateUrl: '/assets/js/templates/search-your-suburb.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
                scope.textbox = el.find('input[type]');
                scope.listbox = el.find('div.list');
                function onResize() {
                    {
                        scope.reRender();
                        scope.$digest();
                    }
                };

                function cleanUp() {
                    angular.element($window).off('resize', onResize);
                }

                angular.element($window).on('resize', onResize);
                scope.$on('$destroy', cleanUp);
            }
        };
    }])
    .factory('suburbService', ['$http', function ($http) {
        var suburbService = {
            searchList: function (data) {
                return $http.post('/API/SUBURBS/SEARCH-LIST', data);
            },
            search: function (data) {
                return $http.post('/API/SUBURBS/SEARCH', data);
            },
            search2: function (data) {
                return $http.post('/API/SUBURBS/SEARCH2', data);
            },
            update: function (data) {
                return $http.post('/API/SUBURBS/UPDATE', data);
            },
            add: function (data) {
                return $http.post('/API/SUBURBS/ADD', data);
            },
            addBulk: function (data) {
                return $http.post('/API/SUBURBS/ADD-BULK', data);
            },
            read: function (id) {
                return $http.get('/API/SUBURBS/READ/' + id);
            },
            delete: function (data) {
                return $http.post('/API/SUBURBS/DELETE', data);
            }
        };

        return suburbService;
    }]);
