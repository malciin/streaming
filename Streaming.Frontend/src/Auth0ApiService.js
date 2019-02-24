export default class Auth0ApiService {
    constructor(props) {
        this.authContext = props.auth;
        this.waitForAuth = this.waitForAuth.bind(this);
    }

    // TODO: Propably not the correct way for waiting to silent authentication
    // Waiting for authentication
    waitForAuth(callback) {
        if (this.authContext.pendingSilentLogin)
        {
            setTimeout(this.waitForAuth.bind(null, callback), 10);
        }
        else
        {
            callback();
        }
    }
}