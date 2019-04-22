export default class ApiService 
{
    constructor(props) {
        this.authContext = props.auth;

        this.makeApiRequest = this.makeApiRequest.bind(this);
    }

    async makeApiRequest(uri, method, object, waitForAuth = false, responseType = 'json')
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
            method: method,
            headers: headers,
            body: object === null ? null : JSON.stringify(object)
        });

        if (responseType == 'raw')
            return response;
        else if (responseType == 'json')
            return await response.json();
    }
}