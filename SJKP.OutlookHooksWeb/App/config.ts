
module angular.ui {
    export interface IState {
        requireADLogin?: boolean;
    }
}

module angular.route {
    export interface IRoute {
        requireADLogin?: boolean;
    }
}

module SJKP.OutlookHooks {
    export class Config {
        public static $inject = ['$routeProvider', '$httpProvider', 'adalAuthenticationServiceProvider', '$locationProvider', 'appClientId', 'graphUrl'];

        constructor(private $routeProvider: angular.route.IRouteProvider, private $httpProvider: ng.IHttpProvider, private adalAuthenticationServiceProvider: any, private $locationProvider: ng.ILocationProvider, private appClientId: string, private graphUrl: string) {
            this.initAdal();
            this.initStates();
        }
        
        private initAdal = () => {
            console.log(this.appClientId);
            var config = {
                instance: 'https://login.microsoftonline.com/',
                tenant: 'common',
                clientId: this.appClientId,
                extraQueryParameter: 'nux=1',
                endpoints: {
                },
                cacheLocation: 'localStorage', // enable this for IE, as sessionStorage does not work for localhost.
            };
            //You don't have to use this, as the app used for authentication is protecting the 
            //same resource as we are accessing, but if you wanted to access, e.g. other resources, like Office 365 API or Exchange API you would specify it here
            // <Path to do token insert for>             : '<Azure AD resource Id>
            config.endpoints[this.graphUrl] = this.graphUrl;
            this.adalAuthenticationServiceProvider.init(config
                ,
                this.$httpProvider
            );


        }

        private initStates = () => {
            this.$routeProvider.when('/home', {
                templateUrl: '/App/views/home.html',
                requireADLogin: true,
                controller: 'homeController'
            }).otherwise('/home');
        }
    }
}