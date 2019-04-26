import * as React from 'react';
import { render } from 'react-dom';
import { createStore } from 'redux';
import { connect, Provider } from 'react-redux';
import User from './Models/User';

export interface ReduxState {
    pendingLogin: boolean,
    user: User
}

export enum Type {
    StartPendingLogin,
    EndPendingLogin,
    UserLoggedIn,
    UserLoggedOut
}

const Reducer = (State: ReduxState, Action) : ReduxState => {
    switch (Action.type) {
        case Type.StartPendingLogin:
            return { ...State, pendingLogin: true };
        case Type.EndPendingLogin:
            return { ...State, pendingLogin: false };
        case Type.UserLoggedIn:
            return { ...State, user: Action.user };
        case Type.UserLoggedOut:
            return { ...State, user: null };
        default:
            return State;
    }
};

let initialState = {
    pendingLogin: false,
    loggedIn: false
}

export const Store = createStore(Reducer, initialState as any);