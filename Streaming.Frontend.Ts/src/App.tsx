import * as React from 'react';
import * as ReactDOM from "react-dom";
import { Router, Route } from "react-router-dom";
import Index from './Components/Pages/Index/Index';
import Callback from './Components/Pages/Callback/Callback';
import History from './Shared/History';
import './App.scss';
import Navbar from './Components/Navbar/Navbar';
import { Provider } from 'react-redux';
import { Store } from './Redux';
import { AppContext } from './AppContext';

class App extends React.Component {
    constructor(props) {
        super(props);
    }
    async componentWillMount()
    {
        await this.context.auth.silentLogin();
    }

    render() {
        return (
        <Router history={History}>
            <div className="app">
                <Navbar />
                <Route exact path="/" component={() => <Index />} />
                <Route path="/sign-in" render={(props) => {
                    this.context.auth.loginCallback();
                    return <Callback />
                }} />
            </div>
        </Router>
        );
    }
}
App.contextType = AppContext;

ReactDOM.render(
    <Provider store={Store}>
        <App />
    </Provider>,
    document.getElementById("app")
);