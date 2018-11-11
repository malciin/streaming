import React from 'react';
import logo from './pageLogo.png'
import './Navbar.css'
import { Link } from "react-router-dom";

class Navbar extends React.Component {
    render() {
        return (
            <nav className="navbar navbar-light bg-light">
                <div className="navbar-brand" href="#">
                    <img src={logo} alt="logo" />
                    
                </div>
                <div className="navbarNav">
                <Link className="nav-item" to="/">Homepage</Link>
                <Link className="nav-item" to="/Upload">Upload video</Link>
                </div>
            </nav>
        );
    }
}

export default Navbar;