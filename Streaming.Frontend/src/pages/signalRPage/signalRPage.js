import React from 'react';
import { Config } from '../../shared/config';
import { AppContext } from '../../appContext';
var signalR = require('@aspnet/signalr');

export default class SignalRPage extends React.Component {
    constructor(props) {
        super(props);
        this.connection = new signalR.HubConnectionBuilder().withUrl(`${Config.apiPath}/SignalR`, {
            accessTokenFactory: () => this.context.auth.idToken,

        }).build();

        this.state = {
            data: []
        }
    }

    async componentDidMount() {
        await this.context.auth.silentLogin();
        await this.context.auth.waitForAuth();
        this.connection.on("ReceiveMessage", function (msg) {
            this.setState({
                data: [...this.state.data, msg]
            })
        }.bind(this))

        this.connection.start().then(function() {
            console.log('Connection started!');
        })
    }

    render() {
        return <div className="container"><ul>{this.state.data.map((msg, index) => <li key={index}>{msg}</li>)}</ul></div>
    }
}

SignalRPage.contextType = AppContext;