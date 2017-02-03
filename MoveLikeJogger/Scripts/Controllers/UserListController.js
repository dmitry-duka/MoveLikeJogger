var UserListController = function ($scope, $rootScope, $http) {
    $scope.users = [];
    $scope.selectedUser = null;

    $scope.totalCount = 0;
    $scope.pageSize = 10;

    $scope.currentPage = 1;

    $scope.getTotalPages = function() {
        return Math.ceil($scope.totalCount / $scope.pageSize);
    };
    
    var getPagingString = function () {
        var ps = '&$top=' + $scope.pageSize;

        if ($scope.currentPage > 1) {
            ps += '&$skip=' + ($scope.currentPage - 1) * $scope.pageSize;
        }

        return ps;
    };

    $scope.canEditUser = function(userId, userRole) {
        return !!$rootScope.identity && $rootScope.identity.Id !== userId && (!userRole || ($rootScope.identity.Role === 'Admin' || !userRole));
    };

    $scope.$watch('currentPage', function (val, oldVal) {
        $scope.loadUsers();
    });

    $scope.usersFilter = {
        includeDeleted: false,
        search: '',
        role: ''
    };

    var getUsersFilterString = function () {
        var filterString = '';

        if (!!$scope.usersFilter) {
            if (!!$scope.usersFilter.includeDeleted) {
                filterString += "&includeDeleted=true";
            }

            if (!!$scope.usersFilter.search || !!$scope.usersFilter.role) {
                filterString += "&$filter=";

                if (!!$scope.usersFilter.search) {
                    filterString += "(indexof(UserName,'" + $scope.usersFilter.search + "') ge 0 or indexof(Email,'" + $scope.usersFilter.search + "') ge 0)";
                }

                if (!!$scope.usersFilter.role) {
                    if (!!$scope.usersFilter.search) {
                        filterString += " and ";
                    }

                    if ($scope.usersFilter.role === '!') {
                        filterString += "(Role eq null or length(Role) le 0)";
                    } else {
                        filterString += "Role eq '" + $scope.usersFilter.role + "'";
                    }
                }
            }
        }

        return filterString;
    };

    $scope.loadUsers = function () {
        $http.get('/api/users?$count=true&$orderby=UserName,Email'+getPagingString()+getUsersFilterString())
            .then(function (response) {
                if (!!response && !!response.data && !!response.data.value) {
                    $scope.users = response.data.value;
                    $scope.totalCount = response.data["@odata.count"];

                    var totalPages = $scope.getTotalPages();

                    if ($scope.currentPage > totalPages) {
                        $scope.currentPage = totalPages;
                    }

                    if ($scope.currentPage < 1) {
                        $scope.currentPage = 1;
                    }
                }
            },
            function() {
                
            });
    }

    $scope.loadUsers();

    $scope.delete = function (key, role) {
        if (!$scope.canEditUser(key, role)) {
            return;
        }

        if (confirm('Are you sure you want to delete this user?')) {
            if ($scope.selectedUser == key) {
                $scope.selectedUser = null;
            }

            $http.delete("/api/users('" + key + "')").then($scope.loadUsers);
        }
    }

    $scope.restore = function (key, role) {
        if (!$scope.canEditUser(key, role)) {
            return;
        }

        $http.delete("/api/users('" + key + "')?restore=true").then($scope.loadUsers);
    }

    $scope.userDetailsModalHandler = {
        onClosedSubmitted: function() {
            $scope.loadUsers();
        }
    }; // to open User Details modal dialog

    $scope.openUserDetails = function (id, role) {
        if (!!id && !$scope.canEditUser(id, role)) {
            return;
        }

        $scope.selectedUser = !!id ? id : null;

        $scope.userDetailsModalHandler.open($scope.selectedUser);
    }
}

UserListController.$inject = ['$scope', '$rootScope', '$http'];