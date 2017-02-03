MoveLikeJogger.directive('movesStatistics', function () {
    return {
        restrict: 'A',
        templateUrl: '/Templates/MovesStatisticsTpl.html',
        scope: {
            userId: '@movesStatistics',
            days: '@'
        },
        controller: ['$scope', '$http', '$filter', function ($scope, $http, $filter) {
            $scope.stats = null;
            $scope.daysBeforeToday = $scope.days || 7;

            $scope.numberFloor = function (value, division) {
                if (division === 0) {
                    return 0;
                }

                return Math.floor(value / division);
            };

            $scope.numberRest = function (value, division) {
                if (division === 0) {
                    return 0;
                }

                var floor = Math.floor(value / division);

                return value - floor * division;
            };

            var showPopoverHold = true;

            var loadStats = function(successCallback, errorCallback) {
                $http.get('/api/statistics?userId=' + $scope.userId + '&daysBeforeToday=' + $scope.daysBeforeToday)
                    .then(function(response) {
                            if (!!response && !!response.data) {
                                $scope.stats = response.data;

                                if (!!successCallback) {
                                    successCallback();
                                }
                            }
                        },
                        function () {
                            $scope.stats = null;

                            if (!!errorCallback) {
                                errorCallback();
                            }
                        });
            };

            var showPopover = function() {
                setTimeout(function() {
                    showPopoverHold = false;
                    $('#statisticsPopover').popover('show');
                    showPopoverHold = true;
                });
            };

            $scope.openStatistics = function () {


                loadStats(showPopover, showPopover);
            };

            $(function () {
                $('#statisticsPopover').popover({
                    html: true,
                    title: '<strong>Moves Statistics for the last ' + $scope.daysBeforeToday + ' days</strong>',
                    content: function() {
                        return $('#popoverContent').html();
                    }
                })
                .on('show.bs.popover', function (e) {
                    return !showPopoverHold;
                });
            });
        }]
    };
});