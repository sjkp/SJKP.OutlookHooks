/// <reference path="../../references.ts" />

module SJKP.OutlookHooks {
    "use strict";

    export interface IHomeScope {
        createSubscription: Function;
        mails: IMail[];
        skip: number;
        page(n: number): void;        
    }

    export class HomeController {
        public static $inject = ["$scope", "graphService"];
        constructor(private $scope: IHomeScope, private graphService: IGraphService) {
            this.$scope.skip = 0;
            Office.initialize = () => {
                console.log('Office context initialized');
            };

            this.getMails();

            graphService.getSubscriptions().then((res) => {
                console.log(res);
            });

            $scope.createSubscription = () => {
                this.graphService.createSubscription().then((res) => {
                    console.log(res);
                });
            };

            $scope.page = (n) => {
                this.$scope.skip += n;
                this.getMails();
            };
            
        }
        
        getMails = () => {
            this.graphService.getMail(this.$scope.skip).then((res) => {
                this.$scope.mails = res;
            });
        };
    
    }

    app.controller("homeController", HomeController);
    
}