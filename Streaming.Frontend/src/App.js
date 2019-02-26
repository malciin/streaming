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
import AdminPage from './pages/adminPage/AdminPage';

class App extends Component {

  async componentWillMount()
  {
    await this.context.auth.silentLogin();
  }

  render() {
    return (
      <Router history={history}>
        <div className="app">
          <Route exact path="/" component={() => <IndexPage tea={this.updated}/>} />
          <Route exact path="/Upload" component={UploadVideoPage} />
          <Route exact path="/Vid/:id" component={VideoPage} />
          <Route exact path="/Admin" component={AdminPage} />
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
export default App;