MoveLikeJogger.directive('paginator', function () {
    return {
        restrict: 'A',
        templateUrl: '/Templates/PaginatorTpl.html',
        scope: {
            current: '=',
            length: '@'
        },
        controller: ['$scope', '$http', function ($scope, $http) {
            $scope.getRepetitions = function(times) {
                return Array.from('a'.repeat(times));
            };

            $scope.pageSelected = function (page) {
                if ($scope.current != page) {
                    $scope.current = page;
                }
            }
        }],
        link: function (scope, element, attrs) {
        }
    };
});