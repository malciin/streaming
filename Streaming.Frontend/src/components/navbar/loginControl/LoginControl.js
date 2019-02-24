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
        this.context.auth.login();
    }

    render() {
        if(this.context.auth.pendingSilentLogin) {
            return <div className="nav-item"></div>
        }        

        if (!this.context.auth.isAuthenticated())
            return <div className="nav-link hoverable" onClick={this.loginCallback}>Login</div>;
        else
            return <div className="nav-link">Hello {this.context.auth.getUserInfo().nickname}!</div>
    }
}

LoginControl.contextType = AppContext;