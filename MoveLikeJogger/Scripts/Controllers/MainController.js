var MainController = function ($scope, $rootScope, $http) {
    $rootScope.identity = null;

    var onIdentityResponse = function (response) {
        $rootScope.identity = null;

        var data = !response ? null : response.data;

        if (!!data && !!data.Id) {
            $rootScope.identity = {
                Id: data.Id,
                UserName: data.UserName,
                Email: data.Email,
                Role: data.Role
            };
        }
    };



    var getIdentity = function() {
        $http.get('api/identity').then(onIdentityResponse, onIdentityResponse);
    };

    getIdentity();

    $rootScope.$on('updateIdentityData', function () {
        getIdentity();
    });
}

MainController.$inject = ['$scope', '$rootScope', '$http'];