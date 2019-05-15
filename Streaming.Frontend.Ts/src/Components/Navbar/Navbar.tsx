import * as React from 'react';
import './Navbar.scss'
import { NavLink } from "react-router-dom";
import LoginControl from '../Blocks/LoginControl/LoginControl';
import { connect } from 'react-redux';
import { ReduxState } from '../../Redux';
import Claims from '../../Models/Claims';
import LoggedUser from '../../Models/LoggedUser';


class Navbar extends React.Component<{userClaims: string[]}> {
    private menu: HTMLDivElement;

    constructor(props) {
        super(props);
        this.getStreamKey = this.getStreamKey.bind(this);
        this.toggleMenu = this.toggleMenu.bind(this);
    }

    async getStreamKey() {
        var key = await this.context.streamingApi.getStreamToken();
    }

    toggleMenu() {
        this.menu.classList.toggle('hidden');
    }

    render() {
        return (
            <nav className="navbar">
                <div className="navbar-brand">
                    <img src= {"/pageLogo.png"} alt="logo" />  
                </div>
                <div className="navbar-links" ref={el => {this.menu = el}}>
                    <ul>
                        <li><NavLink to="/">Homepage</NavLink></li>
                        { this.props.userClaims.includes(Claims.canUploadVideo) && 
                        <li><NavLink to="/Upload">Upload video</NavLink></li> }
                        { this.props.userClaims.includes(Claims.canAccessAuth0Api) && 
                        <li><NavLink to="/Admin">Admin panel</NavLink></li> }
                    </ul>
                </div>
                <LoginControl className="navbar-user cursor-pointer" loggedUserMessage={(user: LoggedUser) => <img src={user.avatarUrl} />} />
                <div className="navbar-toggle" onClick={this.toggleMenu}>
                    <i className="icon-menu-outline" />
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