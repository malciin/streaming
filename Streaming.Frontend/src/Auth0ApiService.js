import { Config } from "./shared/config";
import { AsyncFunctions } from "./shared/AsyncFunctions";

export default class Auth0ApiService {

    constructor(props) {
        this.api = `https://${Config.auth0.domain}/api/v2/`;

        this.authContext = props.auth;

        this.getUsers = this.getUsers.bind(this);
        this.updateClaims = this.updateClaims.bind(this);
    }

    async getUsers(filterObject) {
        await this.authContext.waitForAuth();
        const response = await fetch(`${this.api}users`, {
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${this.authContext.managementApiIdToken}`
            }});
        return await response.json();
    }

    async updateClaims(updateClaimsObject) {
        await this.authContext.waitForAuth();
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