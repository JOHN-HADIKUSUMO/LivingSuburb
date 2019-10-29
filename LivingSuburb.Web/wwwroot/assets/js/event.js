angular.module('eventModule', ['commonModule','tagModule', 'countryModule', 'eventCategoryModule', 'eventTypeModule', 'ui.bootstrap'])
    .controller('listEventController', ['$scope', '$timeout', 'eventService', function ($scope, $timeout, eventService) {
        $scope.btnClick = function () {
            $scope.istriggered = !$scope.istriggered;
        };
        $scope.orderbyList = [
            { id: 0, name: 'Period' },
            { id: 1, name: 'Title' },
            { id: 2, name: 'Category' },
            { id: 3, name: 'Id' }
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
    .controller('detailController', ['$scope', '$timeout', 'eventService', function ($scope, $timeout, eventService) {
        $scope.btnClick = function () {
            window.history.back();
        };
        $scope.visitClick = function (url) {
            console.log(url);
            window.location = url;
        };
    }])
    .controller('eventController', ['$scope', '$timeout', 'eventService', 'eventCategoryService', 'eventTypeService', function ($scope, $timeout, eventService, eventCategoryService,eventTypeService){
        this.$onInit = function () {
            $scope.pageno = 1;
            $scope.pagesize = 10;
            $scope.blocksize = 10;
            $scope.istriggered = false;
            $scope.isprogressing = false;
            eventCategoryService.getList()
                .then(function (res) {
                    $scope.categoryList = res.data;
                    $scope.category = $scope.categoryList[0];
                }, function (res) { });
        };
        $scope.btnClick = function () {
            $scope.pageno = 1;
            $scope.pagesize = 10;
            $scope.blocksize = 10;
            $scope.istriggered = !$scope.istriggered;
        };
        $scope.category = { id: 0, name: null };
        $scope.eventType = { id: 0, name: null };
        $scope.$watch('category', function (n, o, scope) {
            $scope.eventTypeList = [];
            eventTypeService.getList($scope.category.id)
                .then(function (res) {
                    var data = res.data;
                    $scope.eventTypeList = data;
                    $scope.eventType = $scope.eventTypeList[0];
                }, function (res) { });
        });
    }])
    .controller('eventL1Controller', ['$scope', '$timeout', 'eventService', 'eventTypeService', function ($scope, $timeout, eventService, eventTypeService) {
        this.$onInit = function () {
            $scope.pageno = 1;
            $scope.pagesize = 10;
            $scope.blocksize = 10;
            $scope.istriggered = false;
            $scope.isprogressing = false;
            $timeout(function () {
                eventTypeService.getList($scope.category.id)
                    .then(function (res) {
                        $scope.eventTypeList = res.data;
                        $scope.eventType = $scope.eventTypeList[0];
                    }, function (res) {
                        console.log(res.status);
                    });
            }, 600);
        };
        $scope.btnClick = function () {
            $scope.pageno = 1;
            $scope.pagesize = 10;
            $scope.blocksize = 10;
            $scope.istriggered = !$scope.istriggered;
        };
        $scope.category = { id: 0, name: null };
        $scope.eventType = { id: 0, name: null };
    }])
    .controller('eventL2Controller', ['$scope', '$timeout', 'eventService', 'eventTypeService', function ($scope, $timeout, eventService, eventTypeService) {
        this.$onInit = function () {
            $scope.pageno = 1;
            $scope.pagesize = 10;
            $scope.blocksize = 10;
            $scope.istriggered = false;
            $scope.isprogressing = false;
        };
        $scope.btnClick = function () {
            $scope.pageno = 1;
            $scope.pagesize = 10;
            $scope.blocksize = 10;
            $scope.istriggered = !$scope.istriggered;
        };
        $scope.category = { id: 0, name: null };
        $scope.eventType = { id: 0, name: null };
    }])
    .directive('addEvent', ['$uibModal', '$filter', '$timeout', 'tagService', 'eventService', 'countryService', 'eventCategoryService', 'eventTypeService', 'commonService', function ($uibModal, $filter, $timeout, tagService, eventService, countryService, eventCategoryService, eventTypeService, commonService) {
        var ctrl = function ($scope) {
            this.$onInit = function () {
                $scope.categoryList = [];
                eventCategoryService.getList()
                    .then(function (res) {
                        $scope.categoryList = res.data;
                        $scope.categoryList.splice(0, 1);
                        $scope.category = $scope.categoryList[0];
                    }, function (res) { });
            };
            $scope.country = {
                id: null,
                name: null
            };
            $scope.onTitleMouseLeave = function () {
                if ($scope.timer != null)
                    $timeout.cancel($scope.timer);

                $scope.timer = $timeout(function () {
                    var obj = {
                        keywords : $scope.titleURL
                    };
                    commonService.geturlformat(obj)
                        .then(function (res) {
                            console.log(JSON.stringify(res));
                            $scope.titleURL = res.data;
                        }, function (res) { });
                }, 1500);
            };
            $scope.category = {
                id: null,
                name: null
            };
            $scope.timer = null;
            $scope.onCountryListMouseEnter = function () {
                if ($scope.timer != null)
                    $timeout.cancel($scope.timer);
            };
            $scope.onCountryListMouseLeave = function () {
                $scope.timer = $timeout(function () {
                    $scope.$apply(function () {
                        $scope.countryList = [];
                    });
                }, 1500);
            };
            $scope.onTagListMouseEnter = function () {
                if ($scope.timer != null)
                    $timeout.cancel($scope.timer);
            };
            $scope.onTagListMouseLeave = function () {
                $scope.timer = $timeout(function () {
                    $scope.$apply(function () {
                        $scope.tagList = [];
                    });
                }, 1500);
            };
            $scope.onCountryKeyup = function () {
                var timer = null;
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.searching = true;
                    var obj = {
                        Keywords: $scope.country.name,
                        Take: 5
                    };
                    countryService.search2(obj)
                        .then(function (res) {
                            $scope.countryList = [];
                            for (var i = 0; i < res.data.length; i++) {
                                var obj = {
                                    id: res.data[i].id,
                                    name: res.data[i].name
                                }
                                $scope.countryList.push(obj);
                            };
                        }, function () { });
                }, 600);
            };
            $scope.onCountrySelected = function (o) {
                $scope.country = o;
                $timeout(function () {
                    $scope.$apply(function () {
                        $scope.countryList = [];
                    });
                }, 500);
            };
            $scope.clearClick = function () {
                $scope.title = null;
                $scope.titleURL = null;
                $scope.shortDescription = null;
                $('#fullDescription').summernote('code', null);
                $scope.fullDescription = null;
                $scope.country = { id: null, name: null };
                $scope.location = null;
                $scope.category = { id: null, name: null };
                $scope.eventType = { id: null, name: null };
                $scope.fromDate = null;
                $scope.toDate = null;
                $scope.datePublished = null;
                $scope.url = null;
                $scope.tag = {
                    id: null,
                    name: null,
                    isnew: true,
                    isdeleted: false
                };
                $scope.tagList = [];
                $scope.tags = [];
                $scope.addEventForm.$setUntouched();
                $scope.addEventForm.$setPristine();
            };
            $scope.searching = false;
            $scope.submitClick = function () {
                var obj = {
                    Title: $scope.title,
                    TitleURL: $scope.titleURL,
                    ShortDescription:$scope.shortDescription,
                    FullDescription: $('#fullDescription').summernote('code'),
                    Country:$scope.country,
                    Location:$scope.location,
                    Category:$scope.category,
                    EventType:$scope.eventType,
                    FromDate:$scope.fromDate,
                    ToDate: $scope.toDate,
                    DatePublished: $scope.datePublished,
                    Url: $scope.url,
                    DeletedTags: [],
                    NewTags: []
                };

                for (var i = 0; i < $scope.tags.length; i++) {
                    if (!$scope.tags[i].isnew && $scope.tags[i].isdeleted) {
                        var delObj = { id: $scope.tags[i].id, name: $scope.tags[i].name };
                        obj.DeletedTags.push(delObj);
                    };
                    if ($scope.tags[i].isnew && !$scope.tags[i].isdeleted) {
                        var newObj = { id: $scope.tags[i].id, name: $scope.tags[i].name };
                        obj.NewTags.push(newObj);
                    };
                }

                eventService.add(obj)
                    .then(function (res) {
                        $scope.clearClick();
                    }, function (res) {
                        console.log(res.status);
                    });
            };
            $scope.deleteTag = function (o) {
                o.isdeleted = true;
            };
            $scope.tag = {
                id: null,
                name: null,
                isnew: true,
                isdeleted: false
            };
            $scope.onTagKeyup = function () {
                var timer = null;
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.searching = true;
                    var obj = {
                        Keywords: $scope.tag.name,
                        GroupId: 2,
                        Take: 5
                    };
                    tagService.searchKO2(obj)
                        .then(function (res) {
                            var data = res.data;
                            $scope.tagList = [];
                            for (var i = 0; i < data.length; i++) {
                                var obj = {
                                    id: data[i].id,
                                    name: data[i].name,
                                    isnew: true,
                                    isdeleted: false
                                }
                                $scope.tagList.push(obj);
                            };
                        }, function () { });
                }, 500);
            };
            $scope.tagList = [];
            $scope.onTagSelected = function (o) {
                $scope.tag = o;
                $timeout(function () {
                    $scope.$apply(function () {
                        $scope.tagList = [];
                    });
                }, 500);
            };
            $scope.tags = [];
            $scope.addTagClick = function () {
                $timeout(function () {
                    $scope.$apply(function () {
                        var obj = {
                            id: $scope.tag.id,
                            name: $scope.tag.name,
                            isnew: true,
                            isdeleted: false
                        };
                        $scope.tags.push(obj);
                        $scope.tag = { id: null, name: null };
                    });
                }, 500);
            };
        };
        return {
            restrict: 'AE',
            replace: 'true',
            scope: {},
            templateUrl: '/assets/js/templates/add-event.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
                el.find('#fromDate').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement: 'bottom'
                });

                $(document).on('dp.change', function (e) {
                    scope.fromDate = el.find('#fromDate').val();
                });

                el.find('#toDate').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement: 'bottom'
                });

                $(document).on('dp.change', function (e) {
                    scope.toDate = el.find('#toDate').val();
                });

                el.find('#datePublished').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement: 'bottom'
                });

                $(document).on('dp.change', function (e) {
                    scope.datePublished = el.find('#datePublished').val();
                });

                el.find('#fullDescription').summernote({ minHeight: 600, maxHeight: 800 });               
                scope.$watch('category', function (o, n, scope) {
                    eventTypeService.getList(scope.category.id)
                        .then(function (res) {
                            scope.eventTypeList = res.data;
                            scope.eventTypeList.splice(0, 1);
                            scope.eventType = scope.eventTypeList[0];
                        }, function (res) { });
                });
            }
        };
    }])
    .directive('editEvent', ['$uibModal', '$filter', '$timeout', 'tagService', 'eventService', 'countryService', 'eventCategoryService', 'eventTypeService', 'commonService', function ($uibModal, $filter, $timeout, tagService, eventService, countryService, eventCategoryService, eventTypeService, commonService) {
        var ctrl = function ($scope) {
            this.$onInit = function () {
                $scope.categoryList = [];
                eventCategoryService.getList()
                    .then(function (res) {
                        $scope.categoryList = res.data;
                        $scope.categoryList.splice(0, 1);
                    }, function (res) { });

                eventService.edit($scope.id)
                    .then(function (res) {
                        $scope.eventList = [];
                        var data = res.data;
                        $scope.title = data.title;
                        $scope.titleURL = data.titleURL;
                        $scope.shortDescription = data.shortDescription;
                        $('#fullDescription').summernote('code', data.fullDescription);
                        $scope.country = data.country;
                        $scope.location = data.location;
                        $scope.category = data.category;
                        $scope.eventType = data.eventType;
                        $scope.fromDate = data.fromDateStr;
                        $scope.toDate = data.toDateStr;
                        $scope.datePublished = data.datePublishedStr;
                        $scope.url = data.url;
                        for (var i = 0; i < data.editTags.length; i++) {
                            var obj = {
                                id: data.editTags[i].id,
                                name: data.editTags[i].name,
                                isnew: false,
                                isdeleted: false
                            };
                            $scope.tags.push(obj);
                        };                        
                    },
                    function (res) {
                        console.log(JSON.stringify(res));
                    });
            };
            $scope.timer = null;
            $scope.onTitleMouseLeave = function () {
                if ($scope.timer != null)
                    $timeout.cancel($scope.timer);

                $scope.timer = $timeout(function () {
                    var obj = {
                        keywords: $scope.titleURL
                    };
                    commonService.geturlformat(obj)
                        .then(function (res) {
                            console.log(JSON.stringify(res));
                            $scope.titleURL = res.data;
                        }, function (res) { });
                }, 1500);
            };
            $scope.onCountryListMouseEnter = function () {
                if ($scope.timer != null)
                    $timeout.cancel($scope.timer);
            };
            $scope.onCountryListMouseLeave = function () {
                $scope.timer = $timeout(function () {
                    $scope.$apply(function () {
                        $scope.countryList = [];
                    });
                }, 1500);
            };
            $scope.onTagListMouseEnter = function () {
                if ($scope.timer != null)
                    $timeout.cancel($scope.timer);
            };
            $scope.onTagListMouseLeave = function () {
                $scope.timer = $timeout(function () {
                    $scope.$apply(function () {
                        $scope.tagList = [];
                    });
                }, 1500);
            };
            $scope.country = {
                id: null,
                name: null
            };
            $scope.category = {
                id: null,
                name: null
            };
            $scope.onCountryKeyup = function () {
                var timer = null;
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.searching = true;
                    var obj = {
                        Keywords: $scope.country.name,
                        Take: 5
                    };
                    countryService.search2(obj)
                        .then(function (res) {
                            $scope.countryList = [];
                            for (var i = 0; i < res.data.length; i++) {
                                var obj = {
                                    id: res.data[i].id,
                                    name: res.data[i].name
                                }
                                $scope.countryList.push(obj);
                            };
                        }, function () { });
                }, 600);
            };
            $scope.onCountrySelected = function (o) {
                $scope.country = o;
                $timeout(function () {
                    $scope.$apply(function () {
                        $scope.countryList = [];
                    });
                }, 500);
            };
            $scope.searching = false;
            $scope.updateClick = function () {
                var obj = {
                    EventId: $scope.id,
                    Title: $scope.title,
                    TitleURL: $scope.titleURL,
                    ShortDescription: $scope.shortDescription,
                    FullDescription: $('#fullDescription').summernote('code'),
                    Country: $scope.country,
                    Location: $scope.location,
                    Category: $scope.category,
                    EventType: $scope.eventType,
                    FromDate: $scope.fromDate,
                    ToDate: $scope.toDate,
                    DatePublished: $scope.datePublished,
                    Url: $scope.url,
                    DeletedTags: [],
                    NewTags: []
                };

                for (var i = 0; i < $scope.tags.length; i++) {
                    if (!$scope.tags[i].isnew && $scope.tags[i].isdeleted) {
                        var delObj = { id: $scope.tags[i].id, name: $scope.tags[i].name };
                        obj.DeletedTags.push(delObj);
                    };
                    if ($scope.tags[i].isnew && !$scope.tags[i].isdeleted) {
                        var newObj = { id: $scope.tags[i].id, name: $scope.tags[i].name };
                        obj.NewTags.push(newObj);
                    };
                }

                eventService.update(obj)
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
                                    return obj.Title + ' has been saved.';
                                }
                            }
                        }).result.then(function () {

                        });
                    }, function (res) { });
            };
            $scope.deleteTag = function (o) {
                o.isdeleted = true;
            };
            $scope.backClick = function () {
                window.history.back();
            };
            $scope.tag = {
                id: null,
                name: null,
                isnew: true,
                isdeleted: false
            };
            $scope.onTagKeyup = function () {
                var timer = null;
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.searching = true;
                    var obj = {
                        Keywords: $scope.tag.name,
                        GroupId: 2,
                        Take: 5
                    };
                    tagService.searchKO2(obj)
                        .then(function (res) {
                            var data = res.data;
                            $scope.tagList = [];
                            for (var i = 0; i < data.length; i++) {
                                var obj = {
                                    id: data[i].id,
                                    name: data[i].name,
                                    isnew: true,
                                    isdeleted: false
                                }
                                $scope.tagList.push(obj);
                            };
                        }, function () { });
                }, 500);
            };
            $scope.tagList = [];
            $scope.onTagSelected = function (o) {
                $scope.tag = o;
                $timeout(function () {
                    $scope.$apply(function () {
                        $scope.tagList = [];
                    });
                }, 500);
            };
            $scope.tags = [];
            $scope.addTagClick = function () {
                $timeout(function () {
                    $scope.$apply(function () {
                        var obj = {
                            id: $scope.tag.id,
                            name: $scope.tag.name,
                            isnew: true,
                            isdeleted: false
                        };
                        $scope.tags.push(obj);
                        $scope.tag = { id: null, name: null };
                    });
                }, 500);
            };
        };
        return {
            restrict: 'AE',
            replace: 'true',
            scope: {
                id:'='
            },
            templateUrl: '/assets/js/templates/edit-event.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
                el.find('#fromDate').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement: 'bottom'
                });

                $(document).on('dp.change', function (e) {
                    scope.fromDate = el.find('#fromDate').val();
                });

                el.find('#toDate').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement: 'bottom'
                });

                $(document).on('dp.change', function (e) {
                    scope.toDate = el.find('#toDate').val();
                });

                el.find('#datePublished').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement: 'bottom'
                });

                $(document).on('dp.change', function (e) {
                    scope.datePublished = el.find('#datePublished').val();
                });

                el.find('#fullDescription').summernote({ minHeight: 600, maxHeight: 800 });       
                scope.$watch('category', function (o, n, scope) {
                    eventTypeService.getList(scope.category.id)
                        .then(function (res) {
                            scope.eventTypeList = res.data;
                            scope.eventTypeList.splice(0, 1);
                            scope.eventType = scope.eventTypeList[0];
                        }, function (res) { });
                });
            }
        };
    }])
    .component('listEditEvent', {
        templateUrl: '/assets/js/templates/list-edit-event.html',
        controller: function ($scope, $timeout, $uibModal, eventService) {
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
            $ctrl.events = [];
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
                window.location = '/Management/Events/Edit/' + obj.id;
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
                            eventService.delete(obj)
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
                eventService.search2(obj)
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
                $ctrl.events = [];
                if ($ctrl.data.events != undefined) {
                    if ($ctrl.data.events.length > 0) {
                        if (idx < $ctrl.data.events.length) {
                            for (var i = 0; i < $ctrl.data.events[idx].length; i++) {
                                $ctrl.events.push($ctrl.data.events[idx][i]);
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
            category: '=',
            eventtype: '=',
            keywords: '=',
            orderby: '=',
            istriggered: '=',
            isprogressing: '=',
            pageno: '=',
            pagesize: '=',
            blocksize: '='
        }
    })
    .component('listViewEvent', {
        templateUrl: '/assets/js/templates/list-event.html',
        controller: function ($scope, $timeout, $uibModal, eventService) {
            $ctrl = this;
            $ctrl.getParams = function () {
                var obj = {
                    Keywords: $ctrl.keywords,
                    CategoryId: $ctrl.category.id,
                    EventTypeId: $ctrl.eventtype.id,
                    OrderBy: 0,
                    PageNo: $ctrl.pageno,
                    PageSize: $ctrl.pagesize,
                    BlockSize: $ctrl.blocksize
                }
                return obj;
            };
            $ctrl.events = [];
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
                window.location = '/Management/Events/Edit/' + obj.id;
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
                            eventService.delete(obj)
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
                eventService.search2(obj)
                    .then(function(res) {
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
                $ctrl.events = [];
                if ($ctrl.data.events != undefined) {
                    if ($ctrl.data.events.length > 0) {
                        if (idx < $ctrl.data.events.length) {
                            for (var i = 0; i < $ctrl.data.events[idx].length; i++) {
                                $ctrl.events.push($ctrl.data.events[idx][i]);
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
            category: '=',
            eventtype: '=',
            keywords: '=',
            orderby: '=',
            istriggered: '=',
            isprogressing: '=',
            pageno: '=',
            pagesize: '=',
            blocksize: '='
        }
    })
    .factory('eventService', ['$http', function ($http) {
        var eventService = {
            search: function (data) {
                return $http.post('/API/EVENTS/SEARCH', data);
            },
            search2: function (data) {
                return $http.post('/API/EVENTS/SEARCH2', data);
            },
            add: function (data) {
                return $http.post('/API/EVENTS/ADD', data);
            },
            update: function (data) {
                return $http.post('/API/EVENTS/UPDATE', data);
            },
            delete: function (data) {
                return $http.post('/API/EVENTS/DELETE', data);
            },
            edit: function (id) {
                return $http.get('/API/EVENTS/EDIT/' + id);
            }
        };
        return eventService;
    }]);
