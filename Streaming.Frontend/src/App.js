import '../node_modules/bootstrap/dist/css/bootstrap.css'
import React, { Component } from 'react';
import { Router, Route } from "react-router-dom";
import IndexPage from './pages/indexPage/indexPage';
import UploadVideoPage from './pages/uploadVideoPage/uploadVideoPage';
import './App.scss'
import VideoPage from './pages/videoPage/videoPage';
import { AppContext } from './AppContext';
import Callback from './pages/callback/callback';
import history from './History';

class App extends Component {

  componentDidMount()
  {
    this.context.authContext.silentLogin();
  }

  render() {
    return (
      <Router history={history}>
        <div className="app">
          <Route exact path="/" component={() => <IndexPage tea={this.updated}/>} />
          <Route exact path="/Upload" component={UploadVideoPage} />
          <Route exact path="/Vid/:id" component={VideoPage} />
          <Route path="/sign-in" render={(props) => {
            this.context.authContext.loginCallback(props);
            return <Callback />
          }} />
        </div>
      </Router>
    );
  }
}
App.contextType = AppContext;
export default App;