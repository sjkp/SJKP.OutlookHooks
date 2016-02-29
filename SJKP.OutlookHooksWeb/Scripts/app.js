var SJKP;
(function (SJKP) {
    var OutlookHooks;
    (function (OutlookHooks) {
        var Config = (function () {
            function Config($routeProvider, $httpProvider, adalAuthenticationServiceProvider, $locationProvider, appClientId, graphUrl) {
                var _this = this;
                this.$routeProvider = $routeProvider;
                this.$httpProvider = $httpProvider;
                this.adalAuthenticationServiceProvider = adalAuthenticationServiceProvider;
                this.$locationProvider = $locationProvider;
                this.appClientId = appClientId;
                this.graphUrl = graphUrl;
                this.initAdal = function () {
                    console.log(_this.appClientId);
                    var config = {
                        instance: 'https://login.microsoftonline.com/',
                        tenant: 'common',
                        clientId: _this.appClientId,
                        extraQueryParameter: 'nux=1',
                        endpoints: {},
                        cacheLocation: 'localStorage',
                    };
                    //You don't have to use this, as the app used for authentication is protecting the 
                    //same resource as we are accessing, but if you wanted to access, e.g. other resources, like Office 365 API or Exchange API you would specify it here
                    // <Path to do token insert for>             : '<Azure AD resource Id>
                    config.endpoints[_this.graphUrl] = _this.graphUrl;
                    _this.adalAuthenticationServiceProvider.init(config, _this.$httpProvider);
                };
                this.initStates = function () {
                    _this.$routeProvider.when('/home', {
                        templateUrl: '/App/views/home.html',
                        requireADLogin: true,
                        controller: 'homeController'
                    }).otherwise('/home');
                };
                this.initAdal();
                this.initStates();
            }
            Config.$inject = ['$routeProvider', '$httpProvider', 'adalAuthenticationServiceProvider', '$locationProvider', 'appClientId', 'graphUrl'];
            return Config;
        })();
        OutlookHooks.Config = Config;
    })(OutlookHooks = SJKP.OutlookHooks || (SJKP.OutlookHooks = {}));
})(SJKP || (SJKP = {}));
var SJKP;
(function (SJKP) {
    var OutlookHooks;
    (function (OutlookHooks) {
        'use strict';
        OutlookHooks.app = angular.module('outlookaddin', ['ngRoute', 'AdalAngular', 'officeuifabric.core', 'officeuifabric.components']);
        OutlookHooks.app.config(['$logProvider', function ($logProvider) {
                // set debug logging to on
                if ($logProvider.debugEnabled) {
                    $logProvider.debugEnabled(true);
                }
            }]);
        OutlookHooks.app.constant('graphUrl', 'https://graph.microsoft.com/');
        OutlookHooks.app.constant('appClientId', '2c5ab9c1-acc8-47ce-9a60-67823f5ab953');
        if (window.location.host == 'localhost') {
            OutlookHooks.app.constant('backendUrl', 'https://localhost:44302/api');
        }
        else {
            OutlookHooks.app.constant('backendUrl', '/api');
        }
        OutlookHooks.app.config(OutlookHooks.Config);
    })(OutlookHooks = SJKP.OutlookHooks || (SJKP.OutlookHooks = {}));
})(SJKP || (SJKP = {}));
/// <reference path="../../references.ts" />
var SJKP;
(function (SJKP) {
    var OutlookHooks;
    (function (OutlookHooks) {
        "use strict";
        var HomeController = (function () {
            function HomeController($scope, graphService) {
                var _this = this;
                this.$scope = $scope;
                this.graphService = graphService;
                this.getMails = function () {
                    _this.graphService.getMail(_this.$scope.skip).then(function (res) {
                        _this.$scope.mails = res;
                    });
                };
                this.$scope.skip = 0;
                Office.initialize = function () {
                    console.log('Office context initialized');
                };
                this.getMails();
                graphService.getSubscriptions().then(function (res) {
                    console.log(res);
                });
                $scope.createSubscription = function () {
                    _this.graphService.createSubscription().then(function (res) {
                        console.log(res);
                    });
                };
                $scope.page = function (n) {
                    _this.$scope.skip += n;
                    _this.getMails();
                };
            }
            HomeController.$inject = ["$scope", "graphService"];
            return HomeController;
        })();
        OutlookHooks.HomeController = HomeController;
        OutlookHooks.app.controller("homeController", HomeController);
    })(OutlookHooks = SJKP.OutlookHooks || (SJKP.OutlookHooks = {}));
})(SJKP || (SJKP = {}));
/// <reference path="typings/tsd.d.ts" />
/// <reference path="app/config.ts" />
/// <reference path="app/module.ts" />
/// <reference path="app/controllers/homecontroller.ts" />
var SJKP;
(function (SJKP) {
    var OutlookHooks;
    (function (OutlookHooks) {
        "use strict";
        var GraphService = (function () {
            function GraphService($http, $q) {
                var _this = this;
                this.$http = $http;
                this.$q = $q;
                this.getMail = function (skip) {
                    var defer = _this.$q.defer();
                    var url = 'https://graph.microsoft.com/v1.0/me/messages?$top=10';
                    if (angular.isDefined(skip)) {
                        url += "&$skip=" + skip;
                    }
                    _this.$http.get(url).then(function (res) {
                        console.log(res.data);
                        defer.resolve(res.data.value);
                    }).catch(function (err) {
                        defer.reject(err);
                    });
                    return defer.promise;
                };
                this.getSubscriptions = function () {
                    var defer = _this.$q.defer();
                    _this.$http.get('https://graph.microsoft.com/beta/subscriptions/b97846a5-fe4a-4b85-aa75-bc32f8bbf287').then(function (res) {
                        defer.resolve(res.data.value);
                    }).catch(function (err) {
                        defer.reject(err);
                    });
                    return defer.promise;
                };
                this.createSubscription = function () {
                    var body = {
                        "changeType": "Created",
                        "notificationUrl": "https://2d1340c2.ngrok.io/api/subscription/message",
                        "clientState": "subscription-identifier",
                        "resource": "me/messages"
                    };
                    var defer = _this.$q.defer();
                    _this.$http.post('https://graph.microsoft.com/beta/subscriptions', body).then(function (res) {
                        defer.resolve(res.data.value);
                    }).catch(function (err) {
                        defer.reject(err);
                    });
                    return defer.promise;
                };
            }
            GraphService.$inject = ["$http", "$q"];
            return GraphService;
        })();
        OutlookHooks.GraphService = GraphService;
        OutlookHooks.app.service('graphService', GraphService);
    })(OutlookHooks = SJKP.OutlookHooks || (SJKP.OutlookHooks = {}));
})(SJKP || (SJKP = {}));
//# sourceMappingURL=app.js.map