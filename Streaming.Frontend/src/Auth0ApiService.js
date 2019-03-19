import { Config } from "./shared/config";
import { AsyncFunctions } from "./shared/AsyncFunctions";

export default class Auth0ApiService {

    constructor(props) {
        this.api = `https://${Config.auth0.domain}/api/v2/`;

        this.authContext = props.auth;
        this.waitForAuth = this.waitForAuth.bind(this);

        this.getUsers = this.getUsers.bind(this);
        this.updateClaims = this.updateClaims.bind(this);
    }

    // TODO: Propably not the correct way for waiting to silent authentication
    // Waiting for authentication
    async waitForAuth() {
        while (this.authContext.pendingSilentLogin)
        {
            await AsyncFunctions.timeout(10);
        }
    }

    async getUsers(filterObject) {
        await this.waitForAuth();
        const response = await fetch(`${this.api}users`, {
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${this.authContext.managementApiIdToken}`
            }});
        return await response.json();
    }

    async updateClaims(updateClaimsObject) {
        await this.waitForAuth();
        const response = await fetch(`${this.api}users/${updateClaimsObject.userId}`, {
            method: 'PATCH',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${this.authContext.managementApiIdToken}`
            },
            body: JSON.stringify({
                app_metadata: {
                    claims: updateClaimsObject.requestedClaims
                }
            })
        });
    }
}