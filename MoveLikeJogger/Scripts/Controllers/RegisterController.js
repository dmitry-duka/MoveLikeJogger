var RegisterController = function($scope, $rootScope, $routeParams, $http, $location) {
    $scope.registerForm = {
        userName: '',
        email: '',
        password: '',
        confirmPassword: ''
    };

    $scope.register = function () {
        if ($scope.registerForm.confirmPassword !== $scope.registerForm.password) {
            $scope.registerForm.failed = true;
            return;
        }

        $http.post(
            '/api/register', {
                data: {
                    UserName: $scope.registerForm.userName,
                    Email: $scope.registerForm.email,
                    Password: $scope.registerForm.password,
                    RememberMe: true
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
            function (response) {
                $scope.registerForm.errorMessage = !!response && !!response.data && !!response.data.error && !!response.data.error.message
                            ? response.data.error.message
                            : "Maybe this User Name or Email already registered.";

                $scope.registerForm.failed = true;
            });
    }
}

RegisterController.$inject = ['$scope', '$rootScope', '$routeParams', '$http', '$location'];