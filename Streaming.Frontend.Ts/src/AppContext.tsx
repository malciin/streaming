import * as React from 'react';
import Auth0Service from "./Services/Auth0Service";
import StreamingApiService from "./Services/StreamingApiService";
import { Store } from './Redux';
import Auth0ApiService from './Services/Auth0ApiService';


const auth = new Auth0Service(Store);
const streamingApiService = new StreamingApiService(auth);
const auth0Api = new Auth0ApiService(auth);

export interface AppContextInterface {
    auth: Auth0Service,
    streamingApi: StreamingApiService,
    auth0Api: Auth0ApiService
}

export const AppContext = React.createContext<AppContextInterface>({
    auth: auth,
    streamingApi: streamingApiService,
    auth0Api: auth0Api
});