import React from 'react';
import StreamingApiService from './services/streamingApiService';
import Auth0ApiService from './services/auth0ApiService';
import AuthService from './services/authService';

const auth = new AuthService();
const streamingApiService = new StreamingApiService({ auth: auth });
const auth0apiService = new Auth0ApiService({auth: auth});

export const AppContext = React.createContext({
    auth: auth,
    streamingApi: streamingApiService,
    auth0Api: auth0apiService
});