import Auth from './Auth';
import React from 'react';
import ApiService from './ApiService';

const auth = new Auth();
const apiService = new ApiService({ authContext: auth });

export const AppContext = React.createContext({
    authContext: auth,
    apiService: apiService
});