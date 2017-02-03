MoveLikeJogger.directive('movesList', function () {
    return {
        restrict: 'A',
        templateUrl: '/Templates/MovesListTpl.html',
        scope: {
            userId: '@movesList',
            optionIncludeDeleted: '@',
            optionShowStatistics: '@'
        },
        controller: ['$scope', '$http', '$filter', function ($scope, $http, $filter) {
            $scope.submittingDataFlag = false;
            $scope.humanSpeedWorldRecord = 44.71; // Usain Bolt at 100m distance, 2009 Olympics

            $scope.filterDate = function(value, format, timezone) {
                return $filter('date')(value, format, timezone);
            };

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

            $scope.avgSpeedKmh = function (distMeters, durMinutes, durHours) {
                if (!distMeters || !durMinutes && !durHours) {
                    return null;
                }

                var totalDurMinutes = 1 * (durMinutes || 0) + 60 * (durHours || 0);

                return totalDurMinutes > 0 ? distMeters / (totalDurMinutes / 60) / 1000 : 0;
            };

            var destroyDatePicker = function (element) {
                if (!!element) {
                    $('#'+element).datetimepicker('destroy');
                }
            };

            var createDatePicker = function (element, callback, initialValue) {
                if (!element) {
                    return;
                }

                $('#' + element).datetimepicker({
                    format: 'Y-m-d H:i',
                    //mask: true,
                    step: 1,
                    weeks: true,
                    onChangeDateTime: function (ct, $i) {
                        callback(ct);
                    },
                    value: $scope.filterDate(initialValue || new Date(), 'yyyy-MM-dd HH:mm'),
                    maxDate: '0'
                });
            };

            $scope.optionIncludeDeleted = !!$scope.optionIncludeDeleted;
            $scope.optionShowStatistics = !!$scope.optionShowStatistics;

            $scope.moves = [];
            $scope.selectedMove = null;
            $scope.newMove = null;

            var blinkSavedIndicatorTimeout;

            var blinkSavedIndicator = function () {
                clearTimeout(blinkSavedIndicatorTimeout);

                $scope.savedIndicator = true;

                blinkSavedIndicatorTimeout = setTimeout(function () {
                    $scope.savedIndicator = false;

                    $scope.$apply();
                }, 1000);
            };

            $scope.showAddNewMove = function(show) {
                if (show === false) {
                    $scope.newMove = null;
                    //destroyDatePicker('newMoveDate'); // no need to destroy/create this one each time!
                    
                    return;
                }

                $scope.newMove = {
                    userId: $scope.userId,
                    date: new Date(),
                    distance: null,
                    durationHr: null,
                    durationMin: null
                };

                setTimeout(function() {
                    createDatePicker('newMoveDate', function(val) {
                        if (!!$scope.newMove) {
                            $scope.newMove.date = val;
                        }
                    });

                    $('#newMoveDate').val($scope.filterDate(new Date(), 'yyyy-MM-dd HH:mm'));
                });
            }

            $scope.speedOverHumanAbilities = function (speedKmh) {
                return speedKmh > $scope.humanSpeedWorldRecord * 1.1;
            }

            $scope.displaySpeedTooltipIfInvalid = function (elementId, speedKmh) {
                if (!!elementId) {
                    if ($scope.speedOverHumanAbilities(speedKmh)) {
                        var pe = $('#' + elementId);

                        pe.popover({
                            html: true,
                            placement: 'bottom',
                            trigger: 'focus',
                            title: '<strong class="text-danger">This is a super-human speed!</strong>',
                            content: '<span class="text-danger">Only <strong>the<span class="glyphicon glyphicon-flash"></span>FLASH</strong> can move that fast!</span>'
                        }).on('show.bs.popover', function (e) {
                            setTimeout(function () {
                                pe.popover('destroy');
                            }, 3000);
                        });

                        pe.popover('show');
                    }
                }
            }

            $scope.newMoveValid = function() {
                var m = $scope.newMove;

                return !!m
                    && m.userId === $scope.userId
                    && !!m.date
                    && m.distance > 0
                    && (m.durationHr > 0 || m.durationMin > 0)
                    && !$scope.speedOverHumanAbilities($scope.avgSpeedKmh(m.distance, m.durationMin, m.durationHr));
            };

            $scope.selectedMoveValid = function() {
                var m = $scope.selectedMove;

                return !!m
                    && !!m.id
                    && m.userId === $scope.userId
                    && !!m.date
                    && m.distance > 0
                    && (m.durationHr > 0 || m.durationMin > 0)
                    && !$scope.speedOverHumanAbilities($scope.avgSpeedKmh(m.distance, m.durationMin, m.durationHr));
            };

            var errorCallback = function(response) {
                $scope.submittingDataFlag = false;

                $scope.errorMessage = !!response && !!response.data && !!response.data.error && !!response.data.error.message
                    ? response.data.error.message
                    : "Something went wrong. Please check the data you entered.";
            };
            
            $scope.totalCount = 0;
            $scope.pageSize = 10;

            $scope.currentPage = 1;

            $scope.getTotalPages = function () {
                return Math.ceil($scope.totalCount / $scope.pageSize);
            };

            var getPagingString = function () {
                var ps = '&$top=' + $scope.pageSize;

                if ($scope.currentPage > 1) {
                    ps += '&$skip=' + ($scope.currentPage - 1) * $scope.pageSize;
                }

                return ps;
            };

            var moveItemAction = function (id, action) {
                var item = $filter('filter')($scope.moves, function(m) {
                    return m.Id === id;
                })[0];

                if (!!item) {
                    action(item);
                }

                return !!item;
            }

            $scope.saveNewMove = function () {
                var m = $scope.newMove;

                var data = {
                    UserId: m.userId,
                    Date: m.date,
                    Distance: m.distance,
                    Duration: (!!m.durationHr ? 60 * m.durationHr : 0) + (!!m.durationMin ? 0 + m.durationMin : 0)
                };

                $scope.submittingDataFlag = true;

                $http.post("/api/moves", data)
                    .then(function () {
                        $scope.submittingDataFlag = false;
                        $scope.showAddNewMove(false);
                        $scope.errorMessage = null;

                        blinkSavedIndicator();
                        $scope.loadMoves();
                    }, errorCallback);
            };

            $scope.updateMove = function() {
                var m = $scope.selectedMove;

                var data = {
                    Id: m.id,
                    UserId: m.userId,
                    Date: m.date,
                    Distance: m.distance,
                    Duration: (!!m.durationHr ? 60 * m.durationHr : 0) + (!!m.durationMin ? 0 + m.durationMin : 0)
                };

                $http.put("/api/moves(" + data.Id + ")", data)
                    .then(function() {
                        $scope.submittingDataFlag = false;
                        $scope.showEditMove(null);
                        $scope.errorMessage = null;

                        blinkSavedIndicator();
                        $scope.loadMoves();
                    }, errorCallback);
            };

            $scope.isEditMode = function(key) {
                return !!$scope.selectedMove && $scope.selectedMove.id === key;
            };

            $scope.showEditMove = function (key) {
                if (!!$scope.selectedMove && !!$scope.selectedMove.id && $scope.selectedMove.id !== key) {
                    destroyDatePicker('selectedMoveDate' + $scope.selectedMove.id);
                }

                if (!key) {
                    $scope.selectedMove = null;

                    return;
                }

                moveItemAction(key, function (m) {
                    $scope.selectedMove = {
                        id: m.Id,
                        userId: m.UserId,
                        date: m.Date,
                        distance: m.Distance,
                        durationHr: $scope.numberFloor(m.Duration, 60),
                        durationMin: $scope.numberRest(m.Duration, 60)
                    };

                    setTimeout(function () {
                        createDatePicker('selectedMoveDate' + key, function (val) {
                            if (!!$scope.selectedMove) {
                                $scope.selectedMove.date = val;
                            }
                        }, $scope.selectedMove.date);
                    });
                });
            };

            var getMovesFilterString = function () {
                var filterString = "&$filter=UserId eq '" + $scope.userId + "'";

                if (!!$scope.movesFilter) {
                    

                    if (!!$scope.movesFilter.dateFrom || !!$scope.movesFilter.dateTill) {
                        filterString += " and ";

                        if (!!$scope.movesFilter.dateFrom) {
                            filterString += "Date ge " + $scope.filterDate($scope.movesFilter.dateFrom, 'yyyy-MM-dd');
                        }

                        if (!!$scope.movesFilter.dateTill) {
                            if (!!$scope.movesFilter.dateFrom) {
                                filterString += " and ";
                            }

                            var filterEndDate = new Date();
                            
                            filterEndDate.setDate($scope.movesFilter.dateTill.getDate() + 1);

                            filterString += "Date lt " + $scope.filterDate(filterEndDate, 'yyyy-MM-dd');
                        }
                    }

                    if (!!$scope.movesFilter.includeDeleted) {
                        filterString += "&includeDeleted=true";
                    }
                }

                return filterString;
            };

            $scope.$watch('currentPage', function (val, oldVal) {
                $scope.showEditMove(null);
                $scope.loadMoves();
            });

            $scope.movesFilter = {
                includeDeleted: false,
                dateFrom: null,
                dateTill: null
            };

            $scope.clearDateFilter = function () {
                if (!!$scope.movesFilter) {
                    $('#dtFrom').val(null);
                    $('#dtTill').val(null);

                    $scope.movesFilter.dateFrom = null;
                    $scope.movesFilter.dateTill = null;

                    $scope.showEditMove(null);
                    $scope.loadMoves();
                }
            };

            $scope.loadMoves = function() {
                $http.get('/api/moves?$count=true&$orderby=Date desc' + getPagingString() + getMovesFilterString())
                    .then(function(response) {
                            if (!!response && !!response.data && !!response.data.value) {
                                $scope.moves = response.data.value;
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
            };

            $scope.delete = function (key) {
                if (confirm('Are you sure you want to delete this entry?')) {
                    if (!!$scope.selectedMove && $scope.selectedMove.id === key) {
                        $scope.showEditMove(null);
                    }

                    $http.delete("/api/moves(" + key + ")").then(function() {
                        moveItemAction(key, function(item) {
                            item.IsDeleted = true;
                        });

                        blinkSavedIndicator();
                    });
                }
            }

            $scope.restore = function (key) {
                $http.delete("/api/moves(" + key + ")?restore=true").then(function() {
                    moveItemAction(key, function (item) {
                        item.IsDeleted = false;
                    });

                    blinkSavedIndicator();
                });
            }
        }],
        link: function (scope, element, attrs) {

            setTimeout(function () {
                $('#dtFrom').datetimepicker({
                    timepicker: false,
                    format: 'Y-m-d',
                    weeks: true,
                    onShow: function (ct) {
                        this.setOptions({
                            maxDate: $('#dtTill').val() ? $('#dtTill').val() : '0', formatDate: 'Y-m-d'
                        });
                    },
                    onSelectDate: function (ct, $i) {
                        scope.movesFilter.dateFrom = ct;

                        scope.showEditMove(null);
                        scope.loadMoves();
                    },
                    maxDate: '0'
                });

                $('#dtTill').datetimepicker({
                    timepicker: false,
                    format: 'Y-m-d',
                    weeks: true,
                    onShow: function (ct) {
                        this.setOptions({
                            minDate: $('#dtFrom').val() ? $('#dtFrom').val() : false, formatDate: 'Y-m-d'
                        });
                    },
                    onSelectDate: function (ct, $i) {
                        scope.movesFilter.dateTill = ct;

                        scope.loadMoves();
                    },
                    maxDate: '0'
                });
            });
        }
    };
});