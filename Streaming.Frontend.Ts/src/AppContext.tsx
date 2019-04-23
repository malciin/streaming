import * as React from 'react';
import Auth0Service from "./Services/Auth0Service";
import StreamingApiService from "./Services/StreamingApiService";
import { Store } from './Redux';


const auth = new Auth0Service(Store);
const streamingApiService = new StreamingApiService(auth);

export interface AppContextInterface {
    auth: Auth0Service,
    streamingApi: StreamingApiService
}

export const AppContext = React.createContext<AppContextInterface>({
    auth: auth,
    streamingApi: streamingApiService,
});