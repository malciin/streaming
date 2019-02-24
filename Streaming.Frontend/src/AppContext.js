import Auth from './Auth';
import React from 'react';
import StreamingApiService from './StreamingApiService';

const auth = new Auth();
const streamingApiService = new StreamingApiService({ auth: auth });

export const AppContext = React.createContext({
    auth: auth,
    streamingApi: streamingApiService,
});