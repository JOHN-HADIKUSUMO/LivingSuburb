var fileModule = angular.module('fileModule', ['ui.bootstrap'])
var uploadFiles = fileModule.directive('uploadFiles', function () {
    return {
        scope: true,
        link: function (scope, el) {
            el.bind('change', function (event) {
                var files = event.target.files;
                scope.$emit("selectedFile", { file: files[0] });
            });
        }
    };
});

