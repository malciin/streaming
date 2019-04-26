import '../node_modules/bootstrap/dist/css/bootstrap.css'
import React, { Component } from 'react';
import { Router, Route } from "react-router-dom";
import IndexPage from './pages/indexPage/indexPage';
import UploadVideoPage from './pages/uploadVideoPage/uploadVideoPage';
import './app.scss'
import VideoPage from './pages/videoPage/videoPage';
import { AppContext } from './appContext';
import Callback from './pages/callback/callback';
import history from './history';
import AdminPage from './pages/adminPage/adminPage';
import EditVideoPage from './pages/editVideoPage/editVideoPage';
import SignalRPage from './pages/signalRPage/signalRPage';
import StreamPage from './pages/streamPage/streamPage';
import LiveStreamsPage from './pages/liveStreamsPage/liveStreamsPage';
import LiveStreamPage from './pages/liveStreamPage/liveStreamPage';

class App extends Component {

  async componentWillMount()
  {
    await this.context.auth.silentLogin();
  }

  render() {
    console.log(process.env.REACT_APP_API_URL);
    return (
      <Router history={history}>
          <Route exact path="/" component={() => <IndexPage tea={this.updated}/>} />
          <Route exact path="/Upload" component={UploadVideoPage} />
          <Route exact path="/Vid/:id" component={VideoPage} />
          <Route exact path="/Edit/:id" component={EditVideoPage} />
          <Route exact path="/Admin" component={AdminPage} />
          <Route exact path="/SignalR" component={SignalRPage} />
          <Route exact path="/Stream" component={StreamPage} />
          <Route exact path="/Live" component={LiveStreamsPage} />
          <Route exact path="/Live/:id" component={LiveStreamPage} />
          <Route path="/sign-in" render={(props) => {
            this.context.auth.loginCallback(props);
            return <Callback />
          }} />
      </Router>
    );
  }
}
App.contextType = AppContext;
export default App;