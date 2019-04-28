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
import UploadVideo from './Components/Pages/UploadVideo/UploadVideo';
import VideoPage from './Components/Pages/VideoPage/VideoPage';

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
                <Route exact path="/" component={Index} />
                <Route exact path="/Upload" component={UploadVideo} />
                <Route exact path="/Vid/:id" component={(ctx) => {
                    return <VideoPage videoId={ctx.match.params.id} />
                }} />
                {/* <Route exact path="/Upload" component={UploadVideo} />
                <Route exact path="/Edit/:id" component={EditVideo} />
                <Route exact path="/Admin" component={Admin} />
                <Route exact path="/Live" component={LiveStreams} />
                <Route exact path="/Live/:id" component={LiveStream} /> */}
                <Route path="/sign-in" render={(props) => {
                    this.context.auth.loginCallback(props);
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