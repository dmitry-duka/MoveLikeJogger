var AuthHttpResponseInterceptor = function ($q, $location) {
    return {
        response: function (response) {
            if (response.status === 401) {
                console.log("Response 401");
            }
            return response || $q.when(response);
        },
        responseError: function (rejection) {
            if (rejection.status === 401) {
                console.log("Response Error 401", rejection);

                var currentPath = $location.path();

                if (currentPath !== "/login" && currentPath !== "/register") {
                    var gotoLocation = $location.path('/login');

                    if (!!currentPath && currentPath !== "/") {
                        gotoLocation.search('returnUrl', currentPath);
                    }
                }
            }

            return $q.reject(rejection);
        }
    }
}

AuthHttpResponseInterceptor.$inject = ['$q', '$location'];