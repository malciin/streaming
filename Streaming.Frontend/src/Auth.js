import auth0 from 'auth0-js';
import { Config } from './shared/config';
import history from './History';
import { AsyncFunctions } from './shared/AsyncFunctions';

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

    async loginCallback() {
        var authResult = await AsyncFunctions.auth0.parseHash(this.auth0);
        if (authResult && authResult.accessToken && authResult.idToken) {
            authResult.managementApiIdToken = await this.getManagementApiToken(authResult.idToken);
            this.setSession(authResult);
            history.replace('/');
        }
    }

    async getManagementApiToken(idToken) {
        const response = await fetch(Config.apiPath + '/Auth0', {
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${idToken}`
            }
        });

        const jsonData = await response.json();
        return jsonData.token;
    }

    async silentLogin()
    {
        if (localStorage.getItem(this.loggedInSessionKey) === 'true' && !this.idToken)
        {
            this.pendingSilentLogin = true;
            var authResult = await AsyncFunctions.auth0.checkSession(this.auth0);
            authResult.managementApiIdToken = await this.getManagementApiToken(authResult.idToken);
            this.setSession(authResult);
            this.pendingSilentLogin = false;
            history.replace(history.location.pathname);
        }
    }

    setSession(authResult) {
        localStorage.setItem(this.loggedInSessionKey, 'true');

        let expiresAt = (authResult.expiresIn * 1000) + new Date().getTime();
        this.idTokenPayload = authResult.idTokenPayload;
        this.accessToken = authResult.accessToken;
        this.managementApiIdToken = authResult.managementApiIdToken;
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