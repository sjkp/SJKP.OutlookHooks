module SJKP.OutlookHooks {
    "use strict";


    export interface IGraphService {
        getMail(n?: number): ng.IPromise<Microsoft.Graph.message[]>;
        getSubscriptions(): ng.IPromise<any[]>;
        createSubscription(): ng.IPromise<any>;
    }

    export class GraphService implements IGraphService {
        static $inject = ["$http", "$q"];
        constructor(private $http: ng.IHttpService, private $q: ng.IQService) {

        }

        getMail = (skip? : number) => {
            var defer = this.$q.defer();
            var url = 'https://graph.microsoft.com/v1.0/me/messages?$top=10'
            if (angular.isDefined(skip)) {
                url += "&$skip=" + skip;
            }
            this.$http.get(url).then((res) => {
                console.log(res.data);
                defer.resolve((<any>res.data).value);
            }).catch((err) => {
                defer.reject(err);
            });

            return defer.promise;
        };

        getSubscriptions = () => {
            var defer = this.$q.defer();
            this.$http.get('https://graph.microsoft.com/beta/subscriptions/b97846a5-fe4a-4b85-aa75-bc32f8bbf287').then((res) => {
                defer.resolve((<any>res.data).value);
            }).catch((err) => {
                defer.reject(err);
            });

            return defer.promise;
        };

        createSubscription = () => {
            var body = {
                "changeType": "Created",
                "notificationUrl": "https://2d1340c2.ngrok.io/api/subscription/message",
                "clientState": "subscription-identifier",
                "resource": "me/messages"
            };

            var defer = this.$q.defer();

            this.$http.post('https://graph.microsoft.com/beta/subscriptions', body).then((res) => {
                defer.resolve((<any>res.data).value);
            }).catch((err) => {
                defer.reject(err);
            });

            return defer.promise;
        };
    }

    app.service('graphService', GraphService);
}