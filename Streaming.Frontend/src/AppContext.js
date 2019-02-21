import Auth from './Auth';
import React from 'react';

const auth = new Auth();

export const AppContext = React.createContext({
    authContext: auth
});