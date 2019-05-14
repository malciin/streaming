import * as React from 'react';
import Auth0User from '../../../Models/Auth0User';
import UpdateClaims from '../../../Models/Services/Auth0.Api/UpdateClaims';
import UserListItem from '../../Blocks/ListItems/UserListItem';
import { AppContext } from '../../../AppContext';
import './AdminPage.scss';

export default class AdminPage extends React.Component<{}, { users: Auth0User[] }> {
    constructor(props) {
        super(props);
        this.state = {
            users: []
        }

        this.updateClaimRequest = this.updateClaimRequest.bind(this);
    }
    async componentDidMount() {
        var data = await this.context.auth0Api.getUsers();
        this.setState({
            users: data
        })
    }

    async updateClaimRequest(updateObject: UpdateClaims) {
        await this.context.auth0Api.updateClaims(updateObject);
    }

    render() {
        return <div>
            <div className="container">
            <h1>User list</h1>
            <hr />                
                {
                    this.state.users.map((user, i) => {
                        return <UserListItem key={i} userModel={user} updateClaimRequest={this.updateClaimRequest} />
                    })
                }
            </div>
        </div>
    }
}

AdminPage.contextType = AppContext;