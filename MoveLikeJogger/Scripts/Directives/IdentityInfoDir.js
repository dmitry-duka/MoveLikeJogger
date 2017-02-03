MoveLikeJogger.directive('identityInfo', function () {
    return {
        restrict: 'A',
        templateUrl: '/Templates/IdentityInfoTpl.html',
        controller: ['$scope', '$rootScope', '$http', function ($scope, $rootScope, $http) {
            var onIdentityUpdated = function() {
                $rootScope.$broadcast('updateIdentityData');
            };

            $scope.logOut = function() {
                $http.post('/api/logout').then(onIdentityUpdated, onIdentityUpdated);
            }
        }],
        link: function (scope, element, attrs) {
            
        }
    };
});