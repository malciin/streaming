import * as React from 'react';
import * as ReactDOM from "react-dom";
import { Router, Route } from "react-router-dom";
import VideoListPage from './Components/Pages/VideoListPage/VideoListPage';
import CallbackPage from './Components/Pages/CallbackPage/CallbackPage';
import History from './Shared/History';
import './App.scss';
import Navbar from './Components/Navbar/Navbar';
import { Provider } from 'react-redux';
import { Store } from './Redux';
import { AppContext } from './AppContext';
import UploadVideoPage from './Components/Pages/UploadVideoPage/UploadVideoPage';
import VideoPage from './Components/Pages/VideoPage/VideoPage';
import AdminPage from './Components/Pages/AdminPage/AdminPage';
import LiveStreamListPage from './Components/Pages/LiveStreamListPage/LiveStreamListPage';
import LiveStreamPage from './Components/Pages/LiveStreamPage/LiveStreamPage';

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
                <Route exact path="/" component={VideoListPage} />
                <Route exact path="/Upload" component={UploadVideoPage} />
                <Route exact path="/Vid/:id" component={(ctx) => {
                    return <VideoPage videoId={ctx.match.params.id} />
                
                }} />
                <Route exact path="/Admin" component={AdminPage} />
                <Route exact path="/Live" component={LiveStreamListPage} />
                <Route exact path="/Live/:id" component={(ctx) => {
                    return <LiveStreamPage liveStreamId={ctx.match.params.id} />
                }} />
                <Route path="/sign-in" render={(props) => {
                    this.context.auth.loginCallback(props);
                    return <CallbackPage />
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