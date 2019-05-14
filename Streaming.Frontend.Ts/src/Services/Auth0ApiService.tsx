import ApiService, { HttpMethod, RespType, AuthLevel } from "./ApiService";
import Auth0Service from "./Auth0Service";
import Mapper from "./Mapper";
import Auth0User from "../Models/Auth0User";
import UpdateClaims from "../Models/Services/Auth0.Api/UpdateClaims";
import { Config } from "../Shared/Config";

export default class Auth0ApiService extends ApiService {
    private apiUrl: string;
    constructor(authContext: Auth0Service) {
        super(authContext);

        this.apiUrl = `https://${Config.auth0.domain}/api/v2/`;
    }

    async getUsers(): Promise<Auth0User[]> {
        let json = await this.makeApiRequest(`${this.apiUrl}users`, HttpMethod.GET, null, AuthLevel.Auth0ManagementApi);
        return json.map(x => Mapper.mapAuth0User(x));
    }

    async updateClaims(updateClaims: UpdateClaims) {
        let json = await this.makeApiRequest(`${this.apiUrl}users/${updateClaims.userId}`, 
            HttpMethod.PATCH, {
                app_metadata: {
                    claims: updateClaims.requestedClaims
                }
            }, AuthLevel.Auth0ManagementApi);
    }
}