import * as React from 'react';
import './LoginControl.scss';
import { connect } from 'react-redux';
import LoggedUser from '../../../Models/LoggedUser';
import { AppContext } from '../../../AppContext';

interface LoginControlProps {
    className: string,
    loggedUserMessage: (user: LoggedUser) => any
}

interface LoginControlState {
    user: LoggedUser,
    pendingLogin: boolean
}

class LoginControl extends React.Component<LoginControlProps, LoginControlState> {
    constructor(props)
    {
        super(props);
        this.loginCallback = this.loginCallback.bind(this);
        this.state ={
            pendingLogin: false,
            user: null
        };
    }

    componentWillReceiveProps(newState) {
        this.setState({
            pendingLogin: newState.pendingLogin,
            user: newState.user
        });
    }

    loginCallback()
    {
        this.context.auth.login();
    }

    render() {
        if(this.state.pendingLogin) {
            return <div className={this.props.className}></div>
        }
        if (this.state.user == null)
            return <div className={this.props.className} onClick={this.loginCallback}><i className="icon-login"></i></div>;
        else
            return <div className={this.props.className}>{this.props.loggedUserMessage(this.state.user)}</div>
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

const loginControl = connect(function (state, ownProps) {
    return {
        pendingLogin: state.pendingLogin,
        user: state.user,
        loggedUserMessage: ownProps.loggedUserMessage,
        className: ownProps.className
    }
}, {})(LoginControl);
export default loginControl;