import Auth0Service from "./Auth0Service";

export enum HttpMethod {
    GET,
    POST,
    DELETE,
    PUT,
    PATCH
}

export enum RespType {
    Raw, Json
}

export enum AuthLevel {
    None,
    User,
    Auth0ManagementApi
}

export default abstract class ApiService {
    private authContext: Auth0Service;

    constructor(authContext: Auth0Service) {
        this.authContext = authContext;
    }

    async makeApiRequest(uri: string, method: HttpMethod, object: any = null, authLevel: AuthLevel = AuthLevel.None, responseType = RespType.Json)
    {
        let bearerToken: string = null;
        if (authLevel != AuthLevel.None)
            await this.authContext.waitForAuth();
        console.log(this.authContext);
        switch (authLevel) {
            case AuthLevel.User:
                bearerToken = this.authContext.idToken;
                break;
            case AuthLevel.Auth0ManagementApi:
                bearerToken = this.authContext.managementApiIdToken;
        }
        console.log(bearerToken);
        var headers = {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        }

        if (bearerToken) {
            headers['Authorization'] = `Bearer ${bearerToken}`;
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