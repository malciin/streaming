import React from 'react';
import './UserListItem.scss';
import moment from 'moment/moment.js'
export default class UserListItem extends React.Component {
    constructor(props) {
        super(props);

        this.removeClaim = this.removeClaim.bind(this);
        this.addClaim = this.addClaim.bind(this);
        this.addClaimState = this.addClaimState.bind(this);
        this.state = {
            addClaimState: 'none',
            claims: this.props.model.app_metadata.claims
        };

        console.log(this.props);
    }

    addClaimState(data) {
        this.setState({
            addClaimState: 'add'
        });
    }

    async addClaim() {
        let requestedClaim = this.addClaimInput.value;
        this.setState({
            addClaimState: 'adding',
        });
        await this.props.updateClaimRequest({
            userId: this.props.model.user_id,
            requestedClaims: [...this.state.claims, requestedClaim]
        });
        this.setState({
            addClaimState: 'none',
            claims: [...this.state.claims, requestedClaim]
        });
    }

    async removeClaim(removeClaim) {
        let removeClaimName = removeClaim.target.parentElement.firstChild.textContent;
        await this.props.updateClaimRequest({
            userId: this.props.model.user_id,
            requestedClaims: this.state.claims.filter(x => x !== removeClaimName)
        });
        this.setState({
            addClaimState: 'none',
            claims: this.state.claims.filter(x => x !== removeClaimName)
        });
    }

    render() {
        var dateDifference = moment().diff(moment(this.props.model.created_at));

        return <div className="user-list-card">
            <div className="main-user-info">
                <div className="gravatar">
                    <img src={this.props.model.picture} alt="gravatar" />
                </div>
                <div className="user-metadata">
                    <div>{this.props.model.nickname}, {this.props.model.email}</div>
                    <div>Account created: {`${moment.duration(dateDifference).humanize()} ago`}</div>
                </div>
            </div>
            <div className="claims">
                <h6>Claims</h6> 
                {
                    this.state.claims.map((claim, i) => {
                        return <div key={i} className="claim">{claim} <i className="icon-minus-circled delete" onClick={this.removeClaim}></i></div>
                    })
                }
                { this.state.addClaimState === 'none' &&
                    <div className="claim add-claim-field add-claim-button" onClick={this.addClaimState}>Add claim</div>}
                { this.state.addClaimState === 'add' && 
                    <div className="claim add-claim-field add-claim-input">
                        <div className="add-claim-input-container">
                            <div>
                                <input ref={input => { this.addClaimInput = input;}} type={'text'} className="add-claim-input" placeholder={'Type claim'} />
                            </div>
                            <i className="icon-plus add-claim" onClick={this.addClaim}></i>
                        </div>
                    </div> }
                { this.state.addClaimState === 'adding' &&
                    <div className="claim add-claim-field add-claim-loader">Adding...</div>
                }
            </div>
        </div>;
    }

}