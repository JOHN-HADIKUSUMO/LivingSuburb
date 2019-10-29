angular.module('jobModule', ['tagModule', 'stateModule', 'suburbModule', 'jobCategoryModule', 'jobSubCategoryModule', 'ui.bootstrap'])
    .controller('jobController', ['$scope', '$timeout', 'jobService', 'tagService', 'stateService', 'jobCategoryService', function ($scope, $timeout, jobService, tagService, stateService, jobCategoryService) {
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
            $scope.pageno = 1;
            $scope.pagesize = 10;
            $scope.blocksize = 10;
            $scope.istriggered = !$scope.istriggered;
        };
        this.$onInit = function () {
            $scope.istriggered = false;
            $scope.pageno = 1;
            $scope.pagesize = 10;
            $scope.blocksize = 10;
            $scope.isprogressing = false;
            $scope.keywordlist = [];
            $scope.subcategory = { id: 0, name: null };
            $scope.keywords = '';
            stateService.getList()
                .then(function (res) {
                    $scope.stateList = res.data;
                    $scope.state = $scope.stateList[0];
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
                            message: function () {return res.data;}
                            }
                     }).result.then(function () {});
                 });

            jobCategoryService.getList()
                .then(function (res) {
                    $scope.categoryList = res.data;
                    $scope.category = $scope.categoryList[0];
                }, function (res) {
                    console.log('error');
                });
        };
    }])
    .controller('jobListController', ['$scope', '$timeout', 'jobService', 'tagService', 'stateService', 'jobCategoryService', function ($scope, $timeout, jobService, tagService, stateService, jobCategoryService) {
        var timer = null;
        $scope.onSelected = function (o) {
            $scope.keywords = o;
            $scope.keywordlist = [];
        };
        $scope.orderbyList = [
            { id: 0, name: 'Publishing Date' },
            { id: 1, name: 'Job Title' },
            { id: 2, name: 'Category' },
            { id: 3, name: 'State'}
        ];
        $scope.orderbyChange = function () {
            $scope.btnClick();
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
            $scope.orderby = $scope.orderbyList[0];
            $scope.subcategory = { id: 0, name: null };
            $scope.keywords = '';
            stateService.getList()
                .then(function (res) {
                    $scope.stateList = res.data;
                    $scope.state = $scope.stateList[0];
                }, function (res) {
                    console.log('error');
                });

            jobCategoryService.getList()
                .then(function (res) {
                    $scope.categoryList = res.data;
                    $scope.category = $scope.categoryList[0];
                }, function (res) {
                    console.log('error');
                });
        };
    }])
    .directive('editJob', ['$uibModal', '$filter', '$timeout', 'jobCategoryService', 'jobSubCategoryService', 'stateService', 'suburbService', 'tagService', 'jobService', function ($uibModal, $filter, $timeout, jobCategoryService, jobSubCategoryService, stateService, suburbService, tagService, jobService) {
        var ctrl = function ($scope) {
            this.$onInit = function () {
                jobCategoryService.getList()
                    .then(function (res) {
                        $scope.categories = [];
                        for (var i = 0; i < res.data.length; i++) {
                            var obj = {
                                id: res.data[i].id,
                                name: res.data[i].name
                            };
                            $scope.categories.push(obj);
                        };
                    }, function (res) { });
                stateService.getList()
                    .then(function (res) {
                        $scope.states = [];
                        for (var i = 0; i < res.data.length; i++) {
                            var obj = {
                                id: res.data[i].id,
                                name: res.data[i].name
                            };
                            $scope.states.push(obj);
                        };
                    }, function (res) { });
                jobService.edit($scope.id)
                    .then(function (res) {
                        $scope.tagList = [];
                        var data = res.data;
                        $scope.title = data.title;
                        $scope.code = data.code;
                        $scope.company = data.company;
                        $scope.category = data.category;
                        $scope.subcategory = data.subCategory;
                        $scope.state = data.state;
                        $scope.suburb = data.suburb;
                        $scope.shortDescription = data.shortDescription;
                        $("#fullDescription").summernote('code', data.fullDescription);
                        $scope.fullDescription = data.fullDescription;
                        $scope.url = data.url;
                        $scope.publishDate = data.publishingDate;
                        $scope.closeDate = data.closingDate;
                        for (var i = 0; i < data.editTags.length; i++) {
                            var obj = {
                                id: data.editTags[i].id,
                                name: data.editTags[i].name,
                                isnew: false,
                                isdeleted: false
                            };
                            $scope.tags.push(obj);
                        };
                    }, function (res) {
                       console.log(JSON.stringify(res));
                    });
            };
            $scope.tag = {
                id: null,
                name: null,
                isnew: true,
                isdeleted: false
            };
            $scope.category = {
                id: null,
                name: null
            };
            $scope.suburb = {
                id: null,
                name: null
            };
            $scope.suburbList = [];
            $scope.onSuburbSelected = function (o) {
                $scope.suburb = o;
                $timeout(function () {
                    $scope.$apply(function () {
                        $scope.suburbList = [];
                    });
                }, 500);
            };
            $scope.searching = false;
            $scope.onSuburbKeyup = function () {
                var timer = null;
                $scope.suburbList = [];
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.searching = true;
                    var obj = {
                        Keywords: $scope.suburb.name,
                        StateId: $scope.state.id,
                        Take: 5
                    };

                    suburbService.search2(obj)
                        .then(function (res) {
                            for (var i = 0; i < res.data.length; i++) {
                                var obj = {
                                    id: res.data[i].id,
                                    name: res.data[i].name
                                }
                                $scope.suburbList.push(obj);
                            };
                        }, function () { });
                }, 600);
            };
            $scope.onTagKeyup = function () {
                var timer = null;
                $scope.tagList = [];
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.searching = true;
                    var obj = {
                        Keywords: $scope.tag.name,
                        GroupId: 1,
                        Take: 5
                    };
                    tagService.searchKO2(obj)
                        .then(function (res) {
                            for (var i = 0; i < res.data.length; i++) {
                                var obj = {
                                    id: res.data[i].id,
                                    name: res.data[i].name,
                                    isnew: true,
                                    isdeleted: false
                                }
                                $scope.tagList.push(obj);
                            };
                        }, function () { });
                }, 600);
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
                    var obj = {
                        id: $scope.tag.id,
                        name: $scope.tag.name,
                        isnew: true,
                        isdeleted: false
                    };
                    $scope.$apply(function () {
                        $scope.tags.push(obj);
                        $scope.tag = { id: null, name: null };
                    });
                }, 500);
            };
            $scope.categoryChange = function () {
                jobSubCategoryService.getList($scope.category.id)
                    .then(function (res) {
                        $scope.subcategories = [];
                        for (var i = 0; i < res.data.length; i++) {
                            var obj = {
                                id: res.data[i].id,
                                name: res.data[i].name
                            };
                            $scope.subcategories.push(obj);
                        };
                    }, function (res) { });
            };
            $scope.updateClick = function () {
                $uibModal.open({
                    templateUrl: '/assets/js/templates/confirm.html',
                    controller: function (message, $parentScope, $scope, $uibModalInstance) {
                        $scope.content = message;
                        $scope.yesClick = function () {
                            $uibModalInstance.close();
                            $scope.isprogressing = true;
                            var obj = {
                                JobId: $parentScope.id,
                                Title: $parentScope.title,
                                Code: $parentScope.code,
                                Company: $parentScope.company,
                                Category: $parentScope.category,
                                SubCategory: $parentScope.subcategory,
                                State: $parentScope.state,
                                Suburb: $parentScope.suburb,
                                ShortDescription: $parentScope.shortDescription,
                                FullDescription: $('#fullDescription').summernote('code'),
                                Url: $parentScope.url,
                                PublishingDate: $parentScope.publishDate,
                                ClosingDate: $parentScope.closeDate,
                                DeletedTags: [],
                                NewTags: []
                            };

                            for (var i = 0; i < $parentScope.tags.length; i++) {
                                if (!$parentScope.tags[i].isnew && $parentScope.tags[i].isdeleted) {
                                    var delObj = { id: $parentScope.tags[i].id, name: $parentScope.tags[i].name };
                                    obj.DeletedTags.push(delObj);
                                };
                                if ($parentScope.tags[i].isnew && !$parentScope.tags[i].isdeleted) {
                                    var newObj = { id: $parentScope.tags[i].id, name: $parentScope.tags[i].name };
                                    obj.NewTags.push(newObj);
                                };
                            }

                            jobService.update(obj)
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
                                                return obj.Title + ' has been updated.';
                                            }
                                        }
                                    }).result.then(function () { });
                                }, function (res) { });
                        };
                        $scope.noClick = function () {
                            $uibModalInstance.close();
                        };
                    },
                    resolve: {
                        message: function () {
                            return 'Are you sure want to update \"' + $scope.title + '\" ?';
                        },
                        $parentScope: function () {
                            return $scope;
                        }
                    }
                }).result.then(function () {
                    $scope.isprogressing = false;
                });
            };
            $scope.backClick = function () {
                window.history.back();
            };
            $scope.deleteTag = function (o) {
                o.isdeleted = true;
            };
        };
        return {
            restrict: 'AE',
            replace: 'true',
            scope: {
                id: '='
            },
            templateUrl: '/assets/js/templates/edit-job.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
                el.find('#publishDate').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement: 'bottom'
                });
                el.find('#closeDate').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement: 'bottom'
                });

                $(document).on('dp.change', function (e) {
                    scope.closeDate = el.find('#closeDate').val();
                    scope.publishDate = el.find('#publishDate').val();
                });

                el.find('#fullDescription').summernote({
                    minHeight: 600, maxHeight: 800, toolbar: [
                        ['style', ['style']],
                        ['font', ['bold', 'italic', 'underline', 'clear']],
                        ['fontname', ['fontname']],
                        ['color', ['color']],
                        ['para', ['ul', 'ol', 'paragraph']],
                    ],
                }).on("summernote.change", function (content, $editable) {
                    if ($editable == '<p><br></p>') {
                        scope.$apply(function () {
                            scope.fullDescription = null;
                            $('#fullDescription').val(null);
                            scope.addJobForm.fullDescription.$setTouched();
                        });
                    }
                    else {
                        scope.fullDescription = $editable;
                    }
                });
            }
        };
    }])
    .directive('addUserJob', ['$uibModal', '$filter', '$timeout', 'jobCategoryService', 'jobSubCategoryService', 'stateService', 'suburbService', 'tagService', 'jobService', function ($uibModal, $filter, $timeout, jobCategoryService, jobSubCategoryService, stateService, suburbService, tagService, jobService) {
        var ctrl = function ($scope) {
            this.$onInit = function () {
                jobCategoryService.getList()
                    .then(function (res) {
                        $scope.categories = [];
                        for (var i = 0; i < res.data.length; i++) {
                            var obj = {
                                id: res.data[i].id,
                                name: res.data[i].name
                            };
                            $scope.categories.push(obj);
                        };
                    }, function (res) { });
                stateService.getList()
                    .then(function (res) {
                        $scope.states = [];
                        for (var i = 0; i < res.data.length; i++) {
                            var obj = {
                                id: res.data[i].id,
                                name: res.data[i].name
                            };
                            $scope.states.push(obj);
                        };
                    }, function (res) { });
            };
            $scope.clearClick = function () {
                $scope.title = null;
                $scope.code = null;
                $scope.company = null;
                $scope.jobtype = { id: null, name: null };
                $scope.jobterm = { id: null, name: null };
                $scope.category = { id: null, name: null };
                $scope.subcategory = { id: null, name: null };
                $scope.state = { id: null, name: null };
                $scope.suburb = { id: null, name: null };
                $scope.suburbList = [];
                $scope.shortDescription = null;
                $scope.closeDate = null;
                $scope.jobtype = { id: null, name: null }
                $scope.tag = { id: null, name: null }
                $scope.tagList = [];
                $scope.tags = [];
                $scope.addJobForm.$setUntouched();
                $scope.addJobForm.$setPristine();
                $('#fullDescription').summernote('code', '');
            };
            $scope.tag = {
                id: null,
                name: null,
                isnew: true,
                isdeleted: false
            };
            $scope.category = {
                id: null,
                name: null
            };
            $scope.jobtypes = [
                { id: 0, name: 'Fulltime' },
                { id: 1, name: 'Parttime' },
                { id: 2, name: 'Casual' }
            ];
            $scope.jobterms = [
                { id: 0, name: 'Permanent' },
                { id: 1, name: 'Contract' },
                { id: 2, name: 'Other' }
            ];
            $scope.suburb = {
                id: null,
                name: null
            };
            $scope.suburbList = [];
            $scope.onSuburbSelected = function (o) {
                $scope.suburb = o;
                $timeout(function () {
                    $scope.$apply(function () {
                        $scope.suburbList = [];
                    });
                }, 500);
            };
            $scope.searching = false;
            $scope.onSuburbKeyup = function () {
                var timer = null;
                $scope.suburbList = [];
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.searching = true;
                    var obj = {
                        Keywords: $scope.suburb.name,
                        StateId: $scope.state.id,
                        Take: 5
                    };

                    suburbService.search2(obj)
                        .then(function (res) {
                            for (var i = 0; i < res.data.length; i++) {
                                var obj = {
                                    id: res.data[i].id,
                                    name: res.data[i].name
                                }
                                $scope.suburbList.push(obj);
                            };
                        }, function () { });
                }, 600);
            };
            $scope.onTagKeyup = function () {
                var timer = null;
                $scope.tagList = [];
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.searching = true;
                    var obj = {
                        Keywords: $scope.tag.name,
                        GroupId: 1,
                        Take: 5
                    };
                    tagService.searchKO2(obj)
                        .then(function (res) {
                            for (var i = 0; i < res.data.length; i++) {
                                var obj = {
                                    id: res.data[i].id,
                                    name: res.data[i].name,
                                    isnew: true,
                                    isdeleted: false
                                }
                                $scope.tagList.push(obj);
                            };
                        }, function () { });
                }, 600);
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
            $scope.categoryChange = function () {
                jobSubCategoryService.getList($scope.category.id)
                    .then(function (res) {
                        $scope.subcategories = [];
                        for (var i = 0; i < res.data.length; i++) {
                            var obj = {
                                id: res.data[i].id,
                                name: res.data[i].name
                            };
                            $scope.subcategories.push(obj);
                        };
                    }, function (res) { });
            };
            $scope.searchClick = function () {
                alert('hello');
            };
            $scope.submitClick = function () {
                var obj = {
                    Title: $scope.title,
                    Code: $scope.code,
                    Company: $scope.company,
                    Type: $scope.jobtype,
                    Term: $scope.jobterm,
                    Category: $scope.category,
                    SubCategory: $scope.subcategory,
                    State: $scope.state,
                    Suburb: $scope.suburb,
                    ShortDescription: $scope.shortDescription,
                    FullDescription: $scope.fullDescription,
                    ClosingDate: $scope.closeDate,
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

                jobService.addbyuser(obj)
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
                                    return '\"' + obj.Title + '\" has been saved. Once your job is approved, notification email will be sent to your inbox.';
                                }
                            }
                        }).result.then(function () {
                            $scope.clearClick();
                        });

                    }, function (res) {
                            console.log(res.data);
                        $uibModal.open({
                            templateUrl: '/assets/js/templates/error.html',
                            controller: function (message, $scope, $uibModalInstance) {
                                $scope.content = message;
                                $scope.closeClick = function () {
                                $uibModalInstance.close();
                                };
                            },
                            resolve: {
                                message: function () { return res.data; }
                            }
                        }).result.then(function () { });
                    });
            };
            $scope.deleteTag = function (o) {
                o.isdeleted = true;
            };
        };
        return {
            restrict: 'AE',
            replace: 'true',
            scope: {},
            templateUrl: '/assets/js/templates/add-user-job.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
                el.find('#closeDate').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement:'bottom'
                });

                $(document).on('dp.change', function (e) {
                    scope.closeDate = el.find('#closeDate').val();
                });

                el.find('#fullDescription').summernote({
                    minHeight: 600, maxHeight: 800, toolbar: [
                        ['style', ['style']],
                        ['font', ['bold', 'italic', 'underline', 'clear']],
                        ['fontname', ['fontname']],
                        ['color', ['color']],
                        ['para', ['ul', 'ol', 'paragraph']],
                    ],
                }).on("summernote.change", function (content, $editable) {
                    if ($editable == '<p><br></p>') {
                        scope.$apply(function () {
                            scope.fullDescription = null;
                            $('#fullDescription').val(null);
                            scope.addJobForm.fullDescription.$setTouched();
                        });
                    }
                    else {
                        scope.$apply(function () {
                            scope.fullDescription = $editable;
                        });
                    }
                });
            }
        };
    }])
    .directive('addJob', ['$uibModal', '$filter', '$timeout', 'jobCategoryService', 'jobSubCategoryService', 'stateService', 'suburbService', 'tagService', 'jobService', function ($uibModal, $filter, $timeout, jobCategoryService, jobSubCategoryService, stateService, suburbService, tagService, jobService) {
        var ctrl = function ($scope) {
            this.$onInit = function () {
                jobCategoryService.getList()
                    .then(function (res) {
                        $scope.categories = [];
                        for (var i = 0; i < res.data.length; i++) {
                            var obj = {
                                id: res.data[i].id,
                                name: res.data[i].name
                            };
                            $scope.categories.push(obj);
                        };
                    }, function (res) { });
                stateService.getList()
                    .then(function (res) {
                        $scope.states = [];
                        for (var i = 0; i < res.data.length; i++) {
                            var obj = {
                                id: res.data[i].id,
                                name: res.data[i].name
                            };
                            $scope.states.push(obj);
                        };
                    }, function (res) { });
            };
            $scope.clearClick = function () {
                $scope.title = undefined;
                $scope.code = undefined;
                $scope.company = undefined;
                $scope.category = { id: null, name: null };
                $scope.subcategory = { id: null, name: null };
                $scope.state = { id: null, name: null };
                $scope.suburb = { id: null, name: null };
                $scope.suburbList = [];
                $scope.shortDescription = null;
                $('#fullDescription').summernote('code', '');
                $scope.url = null;
                $scope.publishDate = null;
                $scope.closeDate = null;
                $scope.tag = { id: null, name: null }
                $scope.tagList = [];
                $scope.tags = [];
                $scope.addJobForm.$setUntouched();
                $scope.addJobForm.$setPristine();
            };

            $scope.tag = {
                id: null,
                name: null,
                isnew: true,
                isdeleted: false
            };
            $scope.category = {
                id: null,
                name: null
            };
            $scope.suburb = {
                id: null,
                name: null
            };
            $scope.suburbList = [];
            $scope.onSuburbSelected = function (o) {
                $scope.suburb = o;
                $timeout(function () {
                    $scope.$apply(function () {
                        $scope.suburbList = [];
                    });
                }, 500);
            };
            $scope.searching = false;
            $scope.onSuburbKeyup = function () {
                var timer = null;
                $scope.suburbList = [];
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.searching = true;
                    var obj = {
                        Keywords: $scope.suburb.name,
                        StateId: $scope.state.id,
                        Take: 5
                    };

                    suburbService.search2(obj)
                        .then(function (res) {
                            for (var i = 0; i < res.data.length; i++) {
                                var obj = {
                                    id: res.data[i].id,
                                    name: res.data[i].name
                                }
                                $scope.suburbList.push(obj);
                            };
                        }, function () { });
                }, 600);
            };
            $scope.onTagKeyup = function () {
                var timer = null;
                $scope.tagList = [];
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.searching = true;
                    var obj = {
                        Keywords: $scope.tag.name,
                        GroupId: 1,
                        Take: 5
                    };
                    tagService.searchKO2(obj)
                        .then(function (res) {
                            for (var i = 0; i < res.data.length; i++) {
                                var obj = {
                                    id: res.data[i].id,
                                    name: res.data[i].name,
                                    isnew: true,
                                    isdeleted: false
                                }
                                $scope.tagList.push(obj);
                            };
                        }, function () { });
                }, 600);
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
            $scope.categoryChange = function () {
                jobSubCategoryService.getList($scope.category.id)
                    .then(function (res) {
                        $scope.subcategories = [];
                        for (var i = 0; i < res.data.length; i++) {
                            var obj = {
                                id: res.data[i].id,
                                name: res.data[i].name
                            };
                            $scope.subcategories.push(obj);
                        };
                    }, function (res) { });
            };
            $scope.submitClick = function () {
                var obj = {
                    Title: $scope.title,
                    Code: $scope.code,
                    Company: $scope.company,
                    Category: $scope.category,
                    SubCategory: $scope.subcategory,
                    State: $scope.state,
                    Suburb: $scope.suburb,
                    ShortDescription: $scope.shortDescription,
                    FullDescription: $('#fullDescription').summernote('code'),
                    Url: $scope.url,
                    PublishingDate: $scope.publishDate,
                    ClosingDate: $scope.closeDate,
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

                jobService.add(obj)
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
                            $scope.clearClick();
                        });

                    }, function (res) { });
            };
            $scope.deleteTag = function (o) {
                o.isdeleted = true;
            };
        };
        return {
            restrict: 'AE',
            replace: 'true',
            scope: {},
            templateUrl: '/assets/js/templates/add-job.html',
            controller: ctrl,
            link: function (scope, el, attrs) {
                el.find('#publishDate').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement: 'bottom'
                });

                el.find('#closeDate').datetimepicker({
                    format: 'YYYY-MM-DD HH:mm',
                    showClose: true,
                    sideBySide: true,
                    toolbarPlacement: 'bottom'
                });

                $(document).on('dp.change', function (e) {
                    scope.closeDate = el.find('#closeDate').val();
                    scope.publishDate = el.find('#publishDate').val();
                });

                el.find('#fullDescription').summernote({
                    minHeight: 600, maxHeight: 800, toolbar: [
                        ['style', ['style']],
                        ['font', ['bold', 'italic', 'underline', 'clear']],
                        ['fontname', ['fontname']],
                        ['color', ['color']],
                        ['para', ['ul', 'ol', 'paragraph']],
                    ],
                }).on("summernote.change", function (content, $editable) {
                    if ($editable == '<p><br></p>') {
                        scope.$apply(function () {
                            scope.fullDescription = null;
                            $('#fullDescription').val(null);
                            scope.addJobForm.fullDescription.$setTouched();
                        });
                    }
                    else {
                         scope.fullDescription = $editable;
                    }
                });
            }
        };
    }])
    .directive('listJob', ['jobService', '$timeout', '$uibModal', function (jobService, $timeout, $uibModal) {
        ctrl = function ($scope) {
            $scope.data = [];
            $scope.getParams = function () {
                var obj = {
                    Keywords: $scope.keywords,
                    Category: $scope.category.id,
                    SubCategory: $scope.subcategory.id,
                    State: $scope.state.id,
                    PageNo: $scope.pageno,
                    PageSize: $scope.pagesize,
                    BlockSize: $scope.blocksize
                }
                return obj;
            };

            $scope.refresh = function () {
                $scope.isprogressing = true;
                var obj = $scope.getParams();
                jobService.search(obj)
                    .then(function (res) {
                        $scope.data = res.data;
                        $scope.navClick(0, $scope.data.selectedPageNo);
                        $scope.isprogressing = false;
                    }, function (res) {
                        $scope.isprogressing = false;
                    })
            };
            $scope.jobs = [];
            $scope.firstClick = function () {
                $scope.pageno = 1;
                $scope.refresh();
                $timeout(function () {
                    $scope.navClick(0, 1);
                }, 500)
            };
            $scope.prevClick = function () {
                $scope.pageno = $scope.data.pages[0] - 1;
                $scope.refresh();
                $timeout(function () {
                    $scope.navClick($scope.data.pages.length - 1, $scope.pageno);
                }, 500);
            };
            $scope.nextClick = function () {
                $scope.pageno = $scope.data.pages[$scope.data.pages.length - 1] + 1;
                $scope.refresh();
                $scope.isprogressing = false;
                $timeout(function () {
                    $scope.navClick(0, $scope.pageno);
                }, 500);
            };

            $scope.lastClick = function () {
                $scope.pageno = $scope.data.numberOfPages;
                $scope.refresh();
                $timeout(function () {
                    $scope.navClick($scope.data.pages.length - 1, $scope.pageno);
                    $scope.$apply();
                }, 500);
            };

            $scope.navClick = function (idx, no) {
                $scope.pageno = no;
                $scope.data.selectedPageNo = no;
                $scope.jobs = [];
                if ($scope.data.jobs != undefined) {
                    if ($scope.data.jobs.length > 0) {
                        for (var i = 0; i < $scope.data.jobs[idx].length; i++) {
                            $scope.jobs.push($scope.data.jobs[idx][i]);
                        }
                    }
                }
            };

            this.$onInit = function () {
                $scope.pageno = 1;
                $scope.pagesize = 10;
                $scope.blocksize = 10;
                $scope.keywords = '';
                $scope.category = 0;
                $scope.state = 0;
                $scope.istriggered = true;
                $scope.isprogressing = false;
            };
        };
        return {
            restrict: 'E',
            replace: 'true',
            scope: {
                keywords: '=',
                category: '=',
                subcategory: '=',
                state: '=',
                pageno: '=',
                pagesize: '=',
                blocksize: '=',
                isprogressing: '=',
                istriggered: '='
            },
            templateUrl: '/assets/js/templates/list-job.html',
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
    .directive('listEditJob', ['jobService', '$timeout', '$uibModal', function (jobService, $timeout, $uibModal) {
        ctrl = function ($scope) {
            $scope.data = [];
            $scope.getParams = function () {
                var obj = {
                    Keywords: $scope.keywords,
                    Category: $scope.category.id,
                    SubCategory: $scope.subcategory.id,
                    State: $scope.state.id,
                    OrderBy: $scope.orderby.id,
                    PageNo: $scope.pageno,
                    PageSize: $scope.pagesize,
                    BlockSize: $scope.blocksize
                }
                return obj;
            };
            $scope.approveClick = function (obj) {
                alert(JSON.stringify(obj));
            };
            $scope.editClick = function (obj) {
                window.location = ($scope.isdashboard?'/Dashboard':'/Management') + '/Jobs/Edit/' + obj.id + '/' + obj.titleURL;
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

                            jobService.delete(obj)
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
                if ($scope.isdashboard) {
                    jobService.search3(obj)
                    .then(function (res) {
                       $scope.data = res.data;
                       $scope.navClick($scope.data.selectedIndex, $scope.data.selectedPageNo);
                       $scope.isprogressing = false;
                    }, function (res) {
                       $scope.isprogressing = false;
                    })
                }
                else {
                    jobService.search2(obj)
                    .then(function (res) {
                       $scope.data = res.data;
                       $scope.navClick($scope.data.selectedIndex, $scope.data.selectedPageNo);
                       $scope.isprogressing = false;
                    }, function (res) {
                       $scope.isprogressing = false;
                    })
                }
            };
            $scope.jobs = [];
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
                $scope.jobs = [];
                if ($scope.data.jobs != undefined) {
                    if ($scope.data.jobs.length > 0) {
                        for (var i = 0; i < $scope.data.jobs[idx].length; i++) {
                            $scope.jobs.push($scope.data.jobs[idx][i]);
                        }
                    }
                }
            };
            this.$onInit = function () {
                $scope.pageno = 1;
                $scope.pagesize = 10;
                $scope.blocksize = 10;
                $scope.keywords = '';
                $scope.category = null;
                $scope.state = null;
                $scope.istriggered = true;
                $scope.isprogressing = false;
            };
        };
        return {
            restrict: 'E',
            replace: 'true',
            scope: {
                keywords: '=',
                category: '=',
                subcategory: '=',
                state: '=',
                orderby: '=',
                pageno: '=',
                pagesize: '=',
                blocksize: '=',
                isprogressing: '=',
                istriggered: '=',
                isdashboard: '='
            },
            templateUrl: '/assets/js/templates/list-edit-job.html',
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
    .factory('jobService', ['$http', function ($http) {
        var jobService = {
            search: function (data) {
                return $http.post('/API/JOBS/SEARCH', data);
            },
            search2: function (data) {
                return $http.post('/API/JOBS/SEARCH2', data);
            },
            search3: function (data) {
                return $http.post('/API/JOBS/SEARCH3', data);
            },
            addbyuser: function (data) {
                return $http.post('/API/JOBS/ADDBYUSER', data);
            },
            add: function (data) {
                return $http.post('/API/JOBS/ADD', data);
            },
            update: function (data) {
                return $http.post('/API/JOBS/UPDATE', data);
            },
            delete: function (data) {
                return $http.post('/API/JOBS/DELETE', data);
            },
            edit: function (id) {
                return $http.get('/API/JOBS/EDIT/' + id);
            }
        };
        return jobService;
    }]);
