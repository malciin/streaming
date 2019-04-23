import * as React from 'react';
import { render } from 'react-dom';
import { createStore } from 'redux';
import { connect, Provider } from 'react-redux';

export interface ReduxState {
    pendingLogin: boolean,
    loggedIn: boolean
}

export enum Type {
    StartPendingLogin,
    EndPendingLogin,
    UserLoggedIn,
    UserLoggedOut
}

const Reducer = (State, Action) => {
    State = State as ReduxState;
    Action = Action as Type;
    console.log("Change state...");
    console.log(State);
    switch (Action.type) {
        case Type.StartPendingLogin:
            return { ...State, pendingLogin: true };
        case Type.EndPendingLogin:
            return { ...State, pendingLogin: false };
        case Type.UserLoggedIn:
            return { ...State, loggedIn: true };
        case Type.UserLoggedOut:
            return { ...State, loggedIn: false };
        default:
            return State;
    }
};

let initialState = {
    pendingLogin: false,
    loggedIn: false
}

export const Store = createStore(Reducer, initialState as any);