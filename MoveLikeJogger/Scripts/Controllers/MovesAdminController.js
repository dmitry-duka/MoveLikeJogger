var MovesAdminController = function ($scope, $rootScope, $http) {
    $scope.users = [];
    $scope.selectedUser = null;

    $scope.totalCount = 0;
    $scope.bulkLimit = 5;

    $scope.usersFilter = {
        search: ''
    };

    var getUsersFilterString = function () {
        return "&$filter=(Role eq null or length(Role) le 0) and (indexof(UserName,'" + $scope.usersFilter.search + "') ge 0 or indexof(Email,'" + $scope.usersFilter.search + "') ge 0)";
    };

    $scope.selectUser = function(id, name, email) {
        $scope.selectedUser = {
            id,
            name,
            email: email
        };
    };

    $scope.resetUser = function() {
        $scope.selectedUser = null;
    };

    $scope.loadUsers = function () {
        $http.get('/api/users?$count=true&$orderby=UserName,Email&$top='+$scope.bulkLimit+getUsersFilterString())
            .then(function (response) {
                if (!!response && !!response.data && !!response.data.value) {
                    $scope.users = response.data.value;
                    $scope.totalCount = response.data["@odata.count"];
                }
            },
            function() {
                
            });
    }

}

MovesAdminController.$inject = ['$scope', '$rootScope', '$http'];