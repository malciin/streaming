import * as React from 'react';
import './LoginControl.scss';
import { connect } from 'react-redux';
import LoggedUser from '../../../Models/LoggedUser';
import { AppContext } from '../../../AppContext';

interface LoginControlState {
    user: LoggedUser,
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
        console.log("Props:");
        console.log(this.props);
        console.log("USER:"); 
        console.log(this.props.user);
        if (this.props.user == null)
            return <div className="nav-link hoverable" onClick={this.loginCallback}>Login</div>;
        else
            return <div className="nav-link">Hello {this.props.user.nickname}!</div>
    }
}

LoginControl.contextType = AppContext;

const mapStateToProps = (state) => {
    var loginControlState: LoginControlState = {
        user: state.user,
        pendingLogin: state.pendingLogin
    };
    return loginControlState;
}

const loginControl = connect(function (state) {
    return {
        pendingLogin: state.pendingLogin,
        user: state.user
    }
}, {})(LoginControl);
export default loginControl;