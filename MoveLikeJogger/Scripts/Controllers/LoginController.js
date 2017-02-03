var LoginController = function ($scope, $rootScope, $routeParams, $http, $location) {
    $scope.loginForm = {
        userName: '',
        password: '',
        rememberMe: true
    };

    $scope.login = function () {
        $http.post(
            '/api/login', {
                data: {
                    UserName: $scope.loginForm.userName,
                    Password: $scope.loginForm.password,
                    RememberMe: $scope.loginForm.rememberMe
                }
            }
        ).then(function() {
                $rootScope.$broadcast('updateIdentityData');

                if (!!$routeParams.returnUrl && $routeParams.returnUrl != $location.path()) {
                    $location.path($routeParams.returnUrl);
                } else {
                    $location.path('/');
                }
            },
            function() {
                $scope.loginForm.failed = true;
            });
    }
}

LoginController.$inject = ['$scope', '$rootScope', '$routeParams', '$http', '$location'];