import { Config } from "./shared/config";

export default class Auth0ApiService {

    constructor(props) {
        this.api = `https://${Config.auth0.domain}/api/v2/`;

        this.authContext = props.auth;
        this.waitForAuth = this.waitForAuth.bind(this);

        this.getUsers = this.getUsers.bind(this);
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

    getUsers(filterObject, callback) {
       
        this.waitForAuth(
            function (filterObject) {
                fetch(`${this.api}users`, {
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.authContext.managementApiIdToken}`
                }}).then(responsePromise => responsePromise.json())
                .then(callback);
            }.bind(this,filterObject)
        );
    }
}