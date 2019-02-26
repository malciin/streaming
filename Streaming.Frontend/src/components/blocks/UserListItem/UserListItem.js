import React from 'react';
import './UserListItem.scss';
import moment from 'moment/moment.js'
export default class UserListItem extends React.Component {
    constructor(props) {
        super(props);

        this.removeClaim = this.removeClaim.bind(this);
        this.addClaim = this.addClaim.bind(this);
    }

    removeClaim() {
        alert('Are you sure to remove claim?');
    }

    addClaim(data) {
        console.log(data);
    }

    render() {
        var dateDifference = moment().diff(moment(this.props.model.created_at));

        return <div className="user-list-card-container">
            <div className="gravatar-container">
                <img src={this.props.model.picture} alt="gravatar" />
            </div>
            <div className="user-metadata">
                <div>{this.props.model.nickname}, {this.props.model.email}</div>
                <div>Account created: {`${moment.duration(dateDifference).humanize()} ago`}</div>
            </div>
            <div className="claims">
                <h6>Claims</h6> 
                    {
                        this.props.model.app_metadata.claims.map((claim, i) => {
                            return <span key={i} className="claim">{claim} <i className="icon-minus-circled delete" onClick={this.removeClaim}></i></span>
                        })
                    }
                    <i className="icon-plus addClaim" onClick={this.addClaim}></i>
                </div>
        </div>;
    }

}