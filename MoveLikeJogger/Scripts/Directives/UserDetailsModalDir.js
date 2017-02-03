MoveLikeJogger.directive('userDetailsModal', function () {
    return {
        restrict: 'A',
        templateUrl: '/Templates/UserDetailsModalTpl.html',
        scope: {
            dialogHandler: '=userDetailsModal'
        },
        controller: ['$scope', '$rootScope', '$http', function ($scope, $rootScope, $http) {
            $scope.userDetailsForm = {};

            $scope.loadingDataFlag = false;

            $scope.canEditRole = function() {
                return !!$rootScope.identity && $rootScope.identity.Role === 'Admin';
            };

            $scope.isNewUser = function() {
                return !$scope.userDetailsForm.id;
            };

            var initializeForm = function (data) {
                $scope.userDetailsForm = {
                    id: !!data ? data.Id : null,
                    userName: !!data ? data.UserName : '',
                    email: !!data ? data.Email : '',
                    role: !!data ? data.Role : '',
                    setPassword: false,
                    newPassword: '',
                    confirmNewPassword: ''
                };
            };

            var showModal = function (show) {
                $("#modalUserDetails").modal(show === false ? 'hide' : 'show');
            };

            var loadUserDetails = function(key, callback) {
                if (!!key) {
                    $scope.loadingDataFlag = true;

                    $http.get("api/users('" + key + "')").then(callback, callback);
                } else {
                    initializeForm();
                    showModal();
                }
            }

            var dialog = $scope.dialogHandler || {};

            dialog.open = function (key) {
                loadUserDetails(key, function (response) {
                    $scope.loadingDataFlag = false;

                    var data = !response ? null : response.data;

                    if (!!data) {
                        initializeForm(data);

                        showModal();
                    }
                });
            };

            dialog.close = function() {
                showModal(false);
            };

            $scope.saveUserDetails = function () {
                if (!!$scope.userDetailsForm.setPassword && $scope.userDetailsForm.newPassword !== $scope.userDetailsForm.confirmNewPassword) {
                    return;
                }

                $scope.submittingDataFlag = true;

                var userData = {
                    Id: $scope.isNewUser() ? '' : $scope.userDetailsForm.id,
                    UserName: $scope.userDetailsForm.userName,
                    Password: (!!$scope.userDetailsForm.setPassword || $scope.isNewUser()) ? $scope.userDetailsForm.newPassword : '',
                    Role: $scope.userDetailsForm.role,
                    Email: $scope.userDetailsForm.email
                };

                var request = $scope.isNewUser()
                    ? $http.post("/api/users", userData)
                    : $http.put("/api/users('" + $scope.userDetailsForm.id + "')", userData);

                request.then(function() {
                        $scope.submittingDataFlag = false;

                        dialog.close();

                        if (!!dialog.onClosedSubmitted) {
                            dialog.onClosedSubmitted();
                        }
                    },
                    function(response) {
                        $scope.submittingDataFlag = false;

                        $scope.userDetailsForm.errorMessage = !!response && !!response.data && !!response.data.error && !!response.data.error.message
                            ? response.data.error.message
                            : "Maybe User Name or Email you specified is already registered by another user.";

                        $scope.userDetailsForm.failed = true;
                    });
            };
        }],
        link: function (scope, element, attrs) {
            
        }
    };
});