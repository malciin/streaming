import React from 'react';
import './Navbar.scss'
import { NavLink } from "react-router-dom";
import { AppContext } from '../../AppContext';
import LoginControl from './loginControl/LoginControl';

class Navbar extends React.Component {
    constructor(props)
    {
        super(props);
        this.onClickSmth = this.onClickSmth.bind(this);
    }

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
export default Navbar;