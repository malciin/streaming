import Auth from './Auth';
import React from 'react';
import StreamingApiService from './StreamingApiService';
import Auth0ApiService from './Auth0ApiService';

const auth = new Auth();
const streamingApiService = new StreamingApiService({ auth: auth });
const auth0apiService = new Auth0ApiService({auth: auth});

export const AppContext = React.createContext({
    auth: auth,
    streamingApi: streamingApiService,
    auth0Api: auth0apiService
});