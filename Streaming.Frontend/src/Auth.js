import auth0 from 'auth0-js';
import { Config } from './shared/config';
import history from './History';

export default class Auth {

    loggedInSessionKey = "isLoggedIn";

    constructor(props)
    {
        this.auth0 = new auth0.WebAuth(Config.auth0);

        this.login = this.login.bind(this);
        this.getToken = this.getToken.bind(this);
        this.setSession = this.setSession.bind(this);
        this.silentLogin = this.silentLogin.bind(this);
        this.logout = this.logout.bind(this);
        this.getUserInfo = this.getUserInfo.bind(this);
    }
    
    login() {
        this.auth0.authorize();
    }

    logout() {
        this.accessToken = null;
        this.idToken = null;
        this.expiresAt = 0;

        localStorage.removeItem(this.loggedInSessionKey);

        history.replace('/');
    }

    loginCallback(props)
    {
        this.auth0.parseHash((err, authResult) => {
            if (authResult && authResult.accessToken && authResult.idToken) {
                this.setSession(authResult);
                history.replace('/');
            } else {
                history.replace('/');
            }
        });
    }

    silentLogin() {
        if (this.isPendingLogin())
        {
            this.auth0.checkSession({}, (err, authResult) => {
                if (authResult && authResult.accessToken && authResult.idToken) {
                  this.setSession(authResult);
                  this.pendingSilentLogin = false;

                  // Todo: how to elegantly refresh page to make components
                  // using AuthContext to update if the user is logged in
                  history.replace(history.location.pathname);
                } else if (err) {
                    this.logout();
                }
             });
        }
    }

    setSession(authResult) {
        localStorage.setItem(this.loggedInSessionKey, 'true');

        let expiresAt = (authResult.expiresIn * 1000) + new Date().getTime();
        this.idTokenPayload = authResult.idTokenPayload;
        this.accessToken = authResult.accessToken;
        this.idToken = authResult.idToken;
        this.expiresAt = expiresAt;
      }

    getToken()
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

    isPendingLogin() {
        return localStorage.getItem(this.loggedInSessionKey) === 'true' && !this.isAuthenticated();
    }
}