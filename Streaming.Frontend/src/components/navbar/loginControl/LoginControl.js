import React from 'react';
import './LoginControl.scss';
import { AppContext } from '../../../AppContext';

export default class LoginControl extends React.Component {
    constructor(props)
    {
        super(props);
        this.loginCallback = this.loginCallback.bind(this);
        
    }

    componentDidMount() {
    }

    loginCallback()
    {
        this.context.authContext.login();
    }

    render() {
        if(this.context.authContext.pendingSilentLogin) {
            return <div className="nav-item"></div>
        }

        if (!this.context.authContext.isAuthenticated())
            return <div className="nav-link hoverable" onClick={this.loginCallback}>Login</div>;
        else
            return <div className="nav-link">Hello {this.context.authContext.getUserInfo().nickname}!</div>
    }
}

LoginControl.contextType = AppContext;