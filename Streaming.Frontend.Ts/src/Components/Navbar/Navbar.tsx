import * as React from 'react';
import './Navbar.scss'
import { NavLink } from "react-router-dom";
import LoginControl from '../Blocks/LoginControl/LoginControl';
import { connect } from 'react-redux';
import { AppContext } from '../../AppContext';
import { Type } from '../../Redux';


class Navbar extends React.Component {

    constructor(props) {
        super(props);
        this.getStreamKey = this.getStreamKey.bind(this);
    }

    async getStreamKey() {
        var key = await this.context.streamingApi.getStreamToken();
        console.log(key);
    }

    render() {
        return (
            <nav className="navbar navbar-expand-lg navbar-light bg-light">
                <div className="navbar-brand">
                    <img src= {"/pageLogo.png"} alt="logo" />  
                </div>
                <div className="navbar">
                    <ul className="navbar-nav">
                        <li className="nav-item"><NavLink className="nav-link" to="/">Homepage</NavLink></li>
                        { this.context.auth.isAuthenticated() && 
                        <li className="nav-item"><NavLink className="nav-link" to="/Upload">Upload video</NavLink></li> }
                        { this.context.auth.isAuthenticated() && 
                        <li className="nav-item"><NavLink className="nav-link" to="/Admin">Admin panel</NavLink></li> }
                        <li className="nav-item" onClick={this.getStreamKey}>GetKey</li>
                    </ul>
                </div>
                <div className="collapse navbar-collapse justify-content-end" id="navbarCollapse">
                    <ul className="navbar-nav">
                        <li className="nav-item"><LoginControl /></li>
                    </ul>
                </div>
            </nav>
        );
    }
}
Navbar.contextType = AppContext;
const navbar = connect((state) => {
    pendingLogin: state.pendingLogin
}, (dispatch) => {
    onLogin: () => dispatch({ type: Type.UserLoggedIn});
})(Navbar);
export default navbar;