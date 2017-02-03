var AccountDetailsController = function ($scope, $http, $location) {
    $scope.accountForm = {
        userName: '',
        email: '',
        role: '',
        password: '',
        newPassword: '',
        confirmNewPassword: ''
    };

    $scope.loadingDataFlag = false;
    $scope.submittingDataFlag = false;

    var populateAccountDetails = function (response) {
        $scope.loadingDataFlag = false;

        var data = !response ? null : response.data;

        if (!data || !data.UserName) {
            $scope.accountForm = null;

            return;
        }

        $scope.accountForm.userName = data.UserName;
        $scope.accountForm.email = data.Email;
        $scope.accountForm.role = data.Role;
    };

    var loadAccountData = function () {
        $scope.loadingDataFlag = true;
        $http.get('api/identity').then(populateAccountDetails, populateAccountDetails);
    };

    loadAccountData();

    $scope.updateAccount = function () {
        if ($scope.accountForm.confirmNewPassword !== $scope.accountForm.newPassword) {
            return;
        }

        $scope.submittingDataFlag = true;

        $http.put(
            '/api/account', {
                data: {
                    UserName: $scope.accountForm.userName,
                    Email: $scope.accountForm.email,
                    Password: $scope.accountForm.password,
                    NewPassword: $scope.accountForm.newPassword
                }
            }
        ).then(function () {
                $scope.submittingDataFlag = false;

                $location.path('/');
            },
            function (response) {
                $scope.submittingDataFlag = false;
                $scope.accountForm.failed = true;

                $scope.accountForm.errorMessage = !!response && !!response.data && !!response.data.error && !!response.data.error.message
                            ? response.data.error.message
                            : "Maybe this Email already registered by another user. Also please make sure you specified correct current password.";
            });
    }
}

AccountDetailsController.$inject = ['$scope', '$http', '$location'];