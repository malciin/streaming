import Auth0Service from "./Auth0Service";

export enum HttpMethod {
    GET, POST, DELETE, PUT
}

export enum RespType {
    Raw, Json
}

export default abstract class ApiService {
    private authContext: Auth0Service;

    constructor(authContext: Auth0Service) {
        this.authContext = authContext;
    }
    async makeApiRequest(uri: string, method: HttpMethod, object: any, waitForAuth = false, responseType = RespType.Json)
    {
        if (waitForAuth === true)
            await this.authContext.waitForAuth();
        
        var headers = {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        }

        if (this.authContext.idToken) {
            headers['Authorization'] = `Bearer ${this.authContext.idToken}`;
        }

        
        var response = await fetch(uri, {
            method: HttpMethod[method],
            headers: headers,
            body: object === null ? null : JSON.stringify(object)
        });

        if (responseType === RespType.Raw)
            return response;
        else if (responseType === RespType.Json)
            return await response.json();
    }
}