import * as React from 'react';
import './LoginControl.scss';
import { connect } from 'react-redux';
import { AppContext } from '../../../AppContext';

interface LoginControlState {
    loggedIn: boolean,
    pendingLogin: boolean
}

class LoginControl extends React.Component<LoginControlState> {
    constructor(props)
    {
        console.log("PROPS");
        console.log(props);
        super(props);
        this.loginCallback = this.loginCallback.bind(this);
    }

    loginCallback()
    {
        this.context.auth.login();
    }

    render() {
        if(this.props.pendingLogin) {
            return <div className="nav-item"></div>
        }        

        if (!this.props.loggedIn)
            return <div className="nav-link hoverable" onClick={this.loginCallback}>Login</div>;
        else
            return <div className="nav-link">Hello {this.context.auth.getUserInfo().nickname}!</div>
    }
}
LoginControl.contextType = AppContext;

const mapStateToProps = (state) => {
    var loginControlState: LoginControlState = {
        loggedIn: state.loggedIn,
        pendingLogin: state.pendingLogin
    };
    return loginControlState;
}

const loginControl = connect(function (state) {
    return {
        pendingLogin: state.pendingLogin,
        loggedIn: state.loggedIn
    }
}, {})(LoginControl);
export default loginControl;