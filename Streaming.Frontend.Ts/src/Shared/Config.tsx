export const Config = {
    apiPath: process.env['REACT_APP_API_URL'],
    auth0: {
        claimsNamespace: 'http://streaming.com/claims',
        clientID: "kkF9cfb31YAOvr8R5q1FQ8byH9YiQXyr",
        domain: "id0.eu.auth0.com",
        redirectUri: "http://localhost:3000/sign-in",
        responseType: "token id_token",
        scope: "openid profile email"
    }
}