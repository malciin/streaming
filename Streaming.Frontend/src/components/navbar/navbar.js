import React from 'react';
import './navbar.scss'
import { NavLink } from "react-router-dom";
import Authorization from '../authorization/authorization';

class Navbar extends React.Component {
    render() {
        return (
            <nav className="navbar navbar-expand-lg navbar-light bg-light">
                <div className="navbar-brand" href="#">
                    <img src= {"/pageLogo.png"} alt="logo" />  
                </div>
                <div className="navbar">
                    <ul className="navbar-nav">
                        <li className="nav-item"><NavLink className="nav-link" to="/">Homepage</NavLink></li>
                        <li className="nav-item"><NavLink className="nav-link" to="/Upload">Upload video</NavLink></li>
                    </ul>
                </div>
                <div class="collapse navbar-collapse justify-content-end" id="navbarCollapse">
                    <ul class="navbar-nav">
                        <li className="nav-item"><Authorization /></li>
                    </ul>
                </div>
            </nav>
        );
    }
}

export default Navbar;