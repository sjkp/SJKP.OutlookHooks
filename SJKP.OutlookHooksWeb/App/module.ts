module SJKP.OutlookHooks {
    'use strict'

    export var app = angular.module('outlookaddin', ['ngRoute', 'AdalAngular', 'officeuifabric.core', 'officeuifabric.components']);
        
    app.config(['$logProvider', function ($logProvider) {
        // set debug logging to on
        if ($logProvider.debugEnabled) {
            $logProvider.debugEnabled(true);
        }
    }]);
    app.constant('graphUrl', 'https://graph.microsoft.com/'); 
    app.constant('appClientId', '2c5ab9c1-acc8-47ce-9a60-67823f5ab953'); 

    if (window.location.host == 'localhost') {
        app.constant('backendUrl', 'https://localhost:44302/api');
    }
    else {
        app.constant('backendUrl', '/api');
    }
    app.config(Config);

}