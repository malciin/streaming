import auth0 from 'auth0-js';
import { Config } from '../Shared/Config';
import history from '../Shared/History';
import { AsyncFunctions } from '../Shared/AsyncFunctions';
import { Type, Store } from '../Redux';
import User from '../Models/User';

export default class Auth0Service {

    private loggedInSessionKey = "isLoggedIn";
    private pendingSilentLogin = false;
    private auth0: any;
    accessToken: string;
    idToken: string;
    expiresAt: any;
    idTokenPayload: any;
    managementApiIdToken: any;
    Store: any;

    constructor(Store)
    {
        this.Store = Store;
        console.log(Config);
        this.auth0 = new auth0.WebAuth(Config.auth0);

        this.setSession = this.setSession.bind(this);
        this.silentLogin = this.silentLogin.bind(this);
        this.logout = this.logout.bind(this);
        this.getUserInfo = this.getUserInfo.bind(this);
        this.isAuthenticated = this.isAuthenticated.bind(this);
        this.haveClaim = this.haveClaim.bind(this);
        this.waitForAuth = this.waitForAuth.bind(this);
    }

    // TODO: Propably not the correct way for waiting to silent authentication
    // Waiting for authentication
    async waitForAuth() {
        while (this.pendingSilentLogin)
        {
            await AsyncFunctions.timeout(10);
        }
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
            Store.dispatch({ type: Type.StartPendingLogin });
            console.log(Store.getState());
            var authResult = await AsyncFunctions.auth0.checkSession(this.auth0);
            
            if (authResult && authResult.accessToken && authResult.idToken)
            {
                authResult.managementApiIdToken = await this.getManagementApiToken(authResult.idToken);
                this.setSession(authResult);
            }
            else
            {
                this.logout();
            }
        }
        Store.dispatch({ type: Type.EndPendingLogin });
        console.log(Store.getState());
    }

    setSession(authResult) {
        localStorage.setItem(this.loggedInSessionKey, 'true');

        let expiresAt = (authResult.expiresIn * 1000) + new Date().getTime();
        this.idTokenPayload = authResult.idTokenPayload;
        this.accessToken = authResult.accessToken;
        this.managementApiIdToken = authResult.managementApiIdToken;
        this.idToken = authResult.idToken;
        this.expiresAt = expiresAt;
        console.log(this);
        var user: User = {
            nickname: this.idTokenPayload.nickname,
            claims: this.idTokenPayload[Config.auth0.claimsNamespace]
        };
        Store.dispatch({ type: Type.UserLoggedIn, user: user });
    }

    haveClaim(claim) {
        if (this.idTokenPayload)
            return this.idTokenPayload["http://streaming.com/claims"].includes(claim);
        return false;
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