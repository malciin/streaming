import React from 'react';
import './authorization.scss';
import Auth from '../../auth';
export default class Authorization extends React.Component {
    constructor(props)
    {
        super(props);
        this.loginCallback = this.loginCallback.bind(this);
    }

    loginCallback()
    {
        const auth = new Auth();
        auth.login();
    }

    render() {
        return <div className="nav-link hoverable" onClick={this.loginCallback}>Login</div>;
    }
}