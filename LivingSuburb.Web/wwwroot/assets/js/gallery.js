angular.module('galleriesModule', ['fileModule', 'suburbModule', 'stateModule', 'ui.bootstrap'])
    .controller('listGalleryController', ['$scope', '$timeout', 'galleriesService', function ($scope, $timeout, galleriesService) {
        $scope.btnClick = function () {
            $scope.istriggered = !$scope.istriggered;
        };
        $scope.orderbyList = [
            { id: 0, name: 'Id' },
            { id: 1, name: 'Filename' },
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
    .component('listEditGallery', {
        templateUrl: '/assets/js/templates/list-edit-gallery.html',
        controller: function ($scope, $timeout, $uibModal, galleriesService) {
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
            $ctrl.imgClick = function (o) {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/pop-image.html',
                    controller: function (url, $scope, $uibModalInstance) {
                        $scope.url = url;
                        $scope.closeClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        url: function () {
                            return o.urL_570_320;
                        }
                    }
                }).result.then(function () {                    
                });
            };
            $ctrl.galleries = [];
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
            $ctrl.editClick = function (o) {
                window.location = '/Management/Galleries/Edit/' + o.id;
            };
            $ctrl.deleteClick = function (o) {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/confirm.html',
                    controller: function (message, $parentScope, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.yesClick = function () {
                            $parentScope.isprogressing = true;
                            galleriesService.delete(o.id)
                            .then(function (res) {
                                $parentScope.refresh();
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
                galleriesService.search(obj)
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
                $ctrl.galleries = [];
                if ($ctrl.data.galleries != undefined) {
                    if ($ctrl.data.galleries.length > 0) {
                        if (idx < $ctrl.data.galleries.length) {
                            for (var i = 0; i < $ctrl.data.galleries[idx].length; i++) {
                                $ctrl.galleries.push($ctrl.data.galleries[idx][i]);
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
    .controller('editController', ['$filter', '$scope', '$http', '$timeout', 'galleriesService', '$uibModal', 'suburbService', 'stateService', function ($filter, $scope, $http, $timeout, galleriesService, $uibModal, suburbService, stateService) {
        var timer = null;
        $scope.onSelected = function (o) {
            $scope.suburbList = [];
            var temp = o.split(',');
            if (temp.length != 2) {
                $scope.suburb = temp[0];
            }
            else {
                var o = $filter('filter')($scope.stateList, { 'code': $.trim(temp[1]) });
                $scope.state = { "id": o[0].id, "name": o[0].name, "code": o[0].code };
                $scope.suburb = temp[0];
            }
        };
        $scope.updateClick = function () {
            if ($scope.state.id == 0) {
                $scope.galleriesForm.state.$error.required = true;
                $scope.galleriesForm.state.$touched = true;
            }
            else {
                $scope.isprogressing = true;
                var obj = {
                    Id: $scope.id,
                    GUID: $scope.data.guid,
                    Filename: $scope.data.filename,
                    Extension: $scope.data.extension,
                    Suburb: $scope.suburb,
                    State: $scope.state.id,
                    Note: $scope.note
                };
                galleriesService.update(obj)
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
                            return 'Image has been saved.';
                        }
                    }
                    }).result.then(function () {
                        $scope.isprogressing = false;
                    });
                },
                function (res) {
                    console.log(JSON.stringify(res));
                });
            }
        };
        $scope.onKeyup = function () {
            $timeout.cancel(timer);
            timer = $timeout(function () {
                $scope.suburbList = [];
                if ($scope.suburb != '') {
                    var obj = {
                        Keywords: $scope.suburb,
                        StateId: $scope.state.id,
                        Take: 5
                    };

                    if ($scope.state.id == 0) {
                        suburbService.search(obj)
                            .then(function (res) {
                                for (var i = 0; i < res.data.length; i++) {
                                    $scope.suburbList.push(res.data[i].name + ', ' + res.data[i].state);
                                };
                            }, function () { });
                    }
                    else {
                        suburbService.search2(obj)
                            .then(function (res) {
                                for (var i = 0; i < res.data.length; i++) {
                                    $scope.suburbList.push(res.data[i].name);
                                };
                            }, function () { });
                    }
                }
            }, 600);
        };
        $scope.onMouseEnter = function () {
            $timeout.cancel(timer);
        };
        $scope.onMouseLeave = function () {
            timer = $timeout(function () {
                $scope.$apply(function () {
                    $timeout.cancel(timer);
                    $scope.suburbList = [];
                });
            }, 500);
        };
        $scope.uploadClick = function () {
            $("input[type='file']").trigger("click");
        };
        $scope.isprogressing = false;
        $scope.isChanged = false;
        $scope.url = '/assets/img/galleries/default.jpg';
        $scope.file = null;
        $scope.suburblist = [];
        $scope.state = { "id": 0, "name": "Any State", "code": "" };
        $scope.$on('selectedFile', function (event, args) {
            $scope.$apply(function () {
                $scope.isprogressing = true;
                $scope.file = args.file;
                var apiURL = "/API/GALLERIES/UPLOAD";
                $http({
                    method: 'POST',
                    url: apiURL,
                    headers: {
                        'Content-Type': undefined
                    },
                    transformRequest: function (data) {
                        var formData = new FormData();
                        formData.append("gallery", data.file);
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
            $scope.id = $('#id').val();
            galleriesService.read($scope.id)
                .then(function (res) {
                    $scope.data = res.data;
                    $scope.url = $scope.data.url;
                    $scope.suburb = $scope.data.suburb;
                    $scope.state = {
                        id: $scope.data.state,
                        name:''
                    };
                    $scope.note = $scope.data.note;
                    $scope.isChanged = true;
                }, function (res) { });
            stateService.getList2()
                .then(function (res) {
                    $scope.stateList = [];
                    for (var i = 0; i < res.data.length; i++) {
                        var obj = {
                            id: res.data[i].id,
                            name: res.data[i].name,
                            code: res.data[i].code
                        };
                        $scope.stateList.push(obj);
                    }
                }, function (res) { });
        };
    }])
    .controller('addController', ['$filter', '$scope', '$http', '$timeout', 'galleriesService', '$uibModal', 'suburbService', 'stateService', function ($filter, $scope, $http, $timeout, galleriesService, $uibModal, suburbService, stateService) {
        var timer = null;
        $scope.onSelected = function (o) {
            $scope.suburbList = [];
            var temp = o.split(',');
            if (temp.length != 2) {
                $scope.suburb = temp[0];
            }
            else {
                var o = $filter('filter')($scope.stateList, { 'code': $.trim(temp[1]) });
                $scope.state = { "id": o[0].id, "name": o[0].name, "code": o[0].code };
                $scope.suburb = temp[0];
            }
        };
        $scope.clearClick = function () {
            $scope.url = '/assets/img/galleries/default.jpg';
            $scope.suburb = undefined;
            $scope.state = { "id": $scope.stateList[0].id, "name": $scope.stateList[0].name, "code": $scope.stateList[0].code };
            $scope.note = undefined;
            $scope.isprogressing = false;
            $scope.galleriesForm.$setUntouched();
            $scope.galleriesForm.$setPristine();
        };
        $scope.saveClick = function () {
            if ($scope.state.id == 0) {
                $scope.galleriesForm.state.$error.required = true;
                $scope.galleriesForm.state.$touched = true;
            }
            else {
                $scope.isprogressing = true;
                var obj = {
                    GUID: $scope.data.guid,
                    Filename: $scope.data.filename,
                    Extension: $scope.data.extension,
                    Suburb: $scope.suburb,
                    State: $scope.state.id,
                    Note: $scope.note
                };

                galleriesService.create(obj)
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
                                    return 'Image has been saved.';
                                }
                            }
                        }).result.then(function () {
                            $scope.clearClick();
                        });

                },
                function (res) {
                    console.log(JSON.stringify(res));
                });
            }
        };
        $scope.onKeyup = function () {
            $timeout.cancel(timer);
            timer = $timeout(function () {
                $scope.suburbList = [];
                if ($scope.suburb != '') {
                    var obj = {
                        Keywords: $scope.suburb,
                        StateId: $scope.state.id,
                        Take: 5
                    };

                    if ($scope.state.id == 0) {
                        suburbService.search(obj)
                            .then(function (res) {
                                for (var i = 0; i < res.data.length; i++) {
                                    $scope.suburbList.push(res.data[i].name + ', ' + res.data[i].state);
                                };
                            }, function () { });
                    }
                    else {
                        suburbService.search2(obj)
                            .then(function (res) {
                                for (var i = 0; i < res.data.length; i++) {
                                    $scope.suburbList.push(res.data[i].name);
                                };
                            }, function () { });
                    }
                }
            }, 600);
        };
        $scope.onMouseEnter = function () {
            $timeout.cancel(timer);
        };
        $scope.onMouseLeave = function () {
            timer = $timeout(function () {
                $scope.$apply(function () {
                    $timeout.cancel(timer);
                    $scope.suburbList = [];
                });
            },500);
        };
        $scope.uploadClick = function () {
            $("input[type='file']").trigger("click");
        };
        $scope.isprogressing = false;
        $scope.isChanged = false;
        $scope.url = '/assets/img/galleries/default.jpg';
        $scope.file = null;
        $scope.suburblist = [];
        $scope.state = { "id": 0, "name": "Any State", "code": "" };
        $scope.$on('selectedFile', function (event, args) {
            $scope.$apply(function () {
                $scope.isprogressing = true;
                $scope.file = args.file;
                var apiURL = "/API/GALLERIES/UPLOAD";
                $http({
                    method: 'POST',
                    url: apiURL,
                    headers: {
                        'Content-Type': undefined
                    },
                    transformRequest: function (data) {
                        var formData = new FormData();
                        formData.append("gallery", data.file);
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
            stateService.getList2()
            .then(function (res) {
                $scope.stateList = [];
                for (var i = 0; i < res.data.length; i++) {
                    var obj = {
                        id: res.data[i].id,
                        name: res.data[i].name,
                        code: res.data[i].code
                    };
                    $scope.stateList.push(obj);
                }
            }, function (res) { });
        };
    }])
    .factory('galleriesService', ['$http', function ($http) {
        var galleriesService = {
            delete: function (id) {
                return $http.get('/API/GALLERIES/DELETE/' + id);
            },
            read: function (id) {
                return $http.get('/API/GALLERIES/READ/' + id);
            },
            create: function (data) {
                return $http.post('/API/GALLERIES/CREATE', data);
            },
            update: function (data) {
                return $http.post('/API/GALLERIES/UPDATE', data);
            },
            search: function (data) {
                return $http.post('/API/GALLERIES/SEARCH', data);
            }
        };
        return galleriesService;
    }]);

