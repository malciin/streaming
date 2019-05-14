import * as React from 'react';
import './UserListItem.scss';
import Auth0User from '../../../Models/Auth0User';
import Helpers from '../../../Shared/Helpers';
import UpdateClaims from '../../../Models/Services/Auth0.Api/UpdateClaims';

interface UserListItemProps {
    userModel: Auth0User,
    updateClaimRequest: (UpdateClaims) => Promise<any>
}

enum ApiState {
    None, Adding, Add
}

interface UserListItemState {
    apiState: ApiState
    userClaims: string[]
}

export default class UserListItem extends React.Component<UserListItemProps, UserListItemState> {
    private addClaimInput: HTMLInputElement;

    constructor(props) {
        super(props);

        this.removeClaim = this.removeClaim.bind(this);
        this.addClaim = this.addClaim.bind(this);
        this.addClaimState = this.addClaimState.bind(this);
        this.state = {
            apiState: ApiState.None,
            userClaims: this.props.userModel.appMetadata.claims
        };
    }

    addClaimState(data) {
        this.setState({
            apiState: ApiState.Add
        });
    }

    async addClaim() {
        let requestedClaim = this.addClaimInput.value;
        let requestedClaims = [...this.state.userClaims, requestedClaim]; 
        this.setState({
            apiState: ApiState.Adding,
        });
        await this.props.updateClaimRequest({
            userId: this.props.userModel.userId,
            requestedClaims: requestedClaims
        });
        this.setState({
            apiState: ApiState.None,
            userClaims: requestedClaims
        });
    }

    async removeClaim(removeClaim) {
        let removeClaimName = removeClaim.target.parentElement.firstChild.textContent;
        let requestedClaims = this.state.userClaims.filter(x => x !== removeClaimName); 
        await this.props.updateClaimRequest({
            userId: this.props.userModel.userId,
            requestedClaims: requestedClaims
        });
        this.setState({
            apiState: ApiState.None,
            userClaims: requestedClaims
        });
    }

    render() {
        return <div className="user-list-card">
            <div className="main-user-info">
                <div className="gravatar">
                    <img src={this.props.userModel.picture} alt="gravatar" />
                </div>
                <div className="user-metadata">
                    <div>{this.props.userModel.nickname}, {this.props.userModel.email}</div>
                    <div>Account created: {`${Helpers.getHumanizedDateAgoDifference(this.props.userModel.createdAt)} ago`}</div>
                </div>
            </div>
            <div className="claims">
                <h6>Claims</h6> 
                {
                    this.state.userClaims.map((claim, i) => {
                        return <div key={i} className="claim">{claim} <i className="icon-minus-circled delete" onClick={this.removeClaim}></i></div>
                    })
                }
                { this.state.apiState === ApiState.None &&
                    <div className="claim add-claim-field add-claim-button" onClick={this.addClaimState}>Add claim</div>}
                { this.state.apiState === ApiState.Add && 
                    <div className="claim add-claim-field add-claim-input">
                        <div className="add-claim-input-container">
                            <div>
                                <input ref={input => { this.addClaimInput = input;}} type={'text'} className="add-claim-input" placeholder={'Type claim'} />
                            </div>
                            <i className="icon-plus add-claim" onClick={this.addClaim}></i>
                        </div>
                    </div> }
                { this.state.apiState === ApiState.Adding &&
                    <div className="claim add-claim-field add-claim-loader">Adding...</div>
                }
            </div>
        </div>;
    }

}