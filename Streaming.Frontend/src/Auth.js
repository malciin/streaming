import auth0 from 'auth0-js';
import { Config } from './shared/config';
import history from './History';

export default class Auth {

    loggedInSessionKey = "isLoggedIn";
    pendingSilentLogin = false;

    constructor(props)
    {
        this.auth0 = new auth0.WebAuth(Config.auth0);

        this.login = this.login.bind(this);
        this.getIdToken = this.getIdToken.bind(this);
        this.setSession = this.setSession.bind(this);
        this.silentLogin = this.silentLogin.bind(this);
        this.logout = this.logout.bind(this);
        this.getUserInfo = this.getUserInfo.bind(this);
        this.isAuthenticated = this.isAuthenticated.bind(this);
        this.haveClaim = this.haveClaim.bind(this);
    }
    
    login() {
        this.auth0.authorize();
    }

    logout() {
        this.accessToken = null;
        this.idToken = null;
        this.expiresAt = 0;

        localStorage.removeItem(this.loggedInSessionKey);

        // Todo: how to elegantly refresh page to make components
        // using AuthContext to update if the user is logged in
        history.replace(history.location.pathname);
    }

    loginCallback(props)
    {
        this.auth0.parseHash((err, authResult) => {
            if (authResult && authResult.accessToken && authResult.idToken) {
                this.getManagementApiToken(authResult.idToken, function (token) {
                    this.setSession(authResult);
                    this.managementApiIdToken = token;
                    history.replace('/');
                }.bind(this));
            }
        });
    }

    getManagementApiToken(idToken, callback) {
        
        fetch(Config.apiPath + '/Auth0', {
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${idToken}`
            }
        }).then(responsePromise => responsePromise.json())
        .then(jsonData => {
            callback(jsonData.token);
        });
    }

    silentLogin() {
        if (localStorage.getItem(this.loggedInSessionKey) === 'true' && !this.idToken)
        {
            this.pendingSilentLogin = true;
            this.auth0.checkSession({}, function (err, authResult) {
                if (authResult && authResult.accessToken && authResult.idToken) {
                    this.getManagementApiToken(authResult.idToken, function (token) {
                        this.setSession(authResult);
                        this.managementApiIdToken = token;
                        this.pendingSilentLogin = false;

                        // Todo: how to elegantly refresh page to make components
                        // using AuthContext to update if the user is logged in
                        history.replace(history.location.pathname);
                    }.bind(this));
                    
                } else if (err) {
                    this.logout();
                }
             }.bind(this));
        }
    }

    setSession(authResult) {
        localStorage.setItem(this.loggedInSessionKey, 'true');

        let expiresAt = (authResult.expiresIn * 1000) + new Date().getTime();
        this.idTokenPayload = authResult.idTokenPayload;
        console.log('token');
        console.log(this.idTokenPayload);
        this.accessToken = authResult.accessToken;
        this.idToken = authResult.idToken;
        this.expiresAt = expiresAt;
    }

    haveClaim(claim) {
        if (this.idTokenPayload)
            return this.idTokenPayload["http://streaming.com/claims"].includes(claim);
        return false;
    }

    getIdToken()
    {
        return this.idToken;
    }

    getUserInfo()
    {
        return this.idTokenPayload;
    }

    isAuthenticated() {
        let expiresAt = this.expiresAt;
        return new Date().getTime() < expiresAt;
    }
}