import * as React from 'react';
import './Navbar.scss'
import { NavLink } from "react-router-dom";
import LoginControl from '../Blocks/LoginControl/LoginControl';
import { connect } from 'react-redux';
import { ReduxState } from '../../Redux';
import Claims from '../../Models/Claims';


class Navbar extends React.Component<{userClaims: string[]}> {

    constructor(props) {
        super(props);
        this.getStreamKey = this.getStreamKey.bind(this);
    }

    async getStreamKey() {
        var key = await this.context.streamingApi.getStreamToken();
    }

    render() {
        console.log(this.props.userClaims);
        console.log(Claims);
        console.log(Claims.canUploadVideo);
        console.log(this.props.userClaims.includes(Claims.canUploadVideo));
        return (
            <nav className="navbar navbar-expand-lg navbar-light bg-light">
                <div className="navbar-brand">
                    <img src= {"/pageLogo.png"} alt="logo" />  
                </div>
                <div className="navbar">
                    <ul className="navbar-nav">
                        <li className="nav-item"><NavLink className="nav-link" to="/">Homepage</NavLink></li>
                        { this.props.userClaims.includes(Claims.canUploadVideo) && 
                        <li className="nav-item"><NavLink className="nav-link" to="/Upload">Upload video</NavLink></li> }
                        { this.props.userClaims.includes(Claims.canAccessAuth0Api) && 
                        <li className="nav-item"><NavLink className="nav-link" to="/Admin">Admin panel</NavLink></li> }
                        { this.props.userClaims.includes(Claims.canAccessAuth0Api) &&
                        <li className="nav-item" onClick={this.getStreamKey}>GetKey</li> }
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
const navbar = connect(function (state: ReduxState) {
    if (state.user)
        return {
            userClaims: state.user.claims
        }
    return {
        userClaims: []
    }
})(Navbar);
export default navbar;