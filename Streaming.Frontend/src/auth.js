import auth0 from 'auth0-js';
import { Config } from './shared/config';

export default class Auth {

    constructor(props)
    {
        this.auth0 = new auth0.WebAuth(Config.auth0)
    }
    

    login() {
        this.auth0.authorize();
    }

    loginCallback(props)
    {
        this.auth0.parseHash((err, authResult) => {
            localStorage.setItem('isLoggedIn', 'true');
            this.accessToken = authResult.accessToken;
            console.log(authResult);
            this.idToken = authResult.idToken;
            props.history.push('/');
        });
    }

    getToken()
    {
        return this.idToken;
    }
}