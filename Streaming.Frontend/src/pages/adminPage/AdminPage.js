import Navbar from "../../components/navbar/Navbar";
import React from 'react';
import { AppContext } from "../../AppContext";
import './AdminPage.scss';
import UserListItem from "../../components/blocks/UserListItem/UserListItem";

export default class AdminPage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            users: []
        }
    }
    async componentDidMount() {
        var data = await this.context.auth0Api.getUsers({});
        this.setState({
            users: data
        })
    }

    render() {
        return <div>
            <Navbar />
            <div className="container">
            <h1>User list</h1>
            <hr />                
                {
                    this.state.users.map((user, i) => {
                        return <UserListItem key={i} model={user} />
                    })
                }
            </div>
        </div>
    }
}

AdminPage.contextType = AppContext;