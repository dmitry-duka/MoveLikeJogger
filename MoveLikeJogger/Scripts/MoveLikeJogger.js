var MoveLikeJogger = angular.module('MoveLikeJogger', ['ngRoute']);

MoveLikeJogger.controller('MainController', MainController);
//MoveLikeJogger.controller('LoginController', LoginController);
//MoveLikeJogger.controller('RegisterController', LoginController);
//MoveLikeJogger.controller('AccountDetailsController', LoginController);
//MoveLikeJogger.controller('UserListController', UserListController);

MoveLikeJogger.factory('AuthHttpResponseInterceptor', AuthHttpResponseInterceptor);

var routeConfig = function ($routeProvider, $httpProvider) {
    $routeProvider.
        when('/login', {
            templateUrl: '/Templates/LoginTpl.html',
            controller: LoginController
        })
        .when('/register', {
            templateUrl: '/Templates/RegisterTpl.html',
            controller: RegisterController
        })
        .when('/account', {
            templateUrl: '/Templates/AccountDetailsTpl.html',
            controller: AccountDetailsController
        })
        .when('/users', {
            templateUrl: '/Templates/UserListTpl.html',
            controller: UserListController
        })
        .when('/movesadmin', {
            templateUrl: '/Templates/MovesAdminTpl.html',
            controller: MovesAdminController
        })
        .otherwise({
            templateUrl: '/Templates/LandingPageTpl.html'
        });

    $httpProvider.interceptors.push('AuthHttpResponseInterceptor');
}

routeConfig.$inject = ['$routeProvider', '$httpProvider'];

MoveLikeJogger.config(routeConfig);