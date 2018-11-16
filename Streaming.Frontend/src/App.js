import '../node_modules/bootstrap/dist/css/bootstrap.css'
import React, { Component } from 'react';
import { BrowserRouter as Router, Route } from "react-router-dom";
import IndexPage from './pages/indexPage/indexPage';
import UploadVideoPage from './pages/uploadVideoPage/uploadVideoPage';

class App extends Component {
  render() {
    return (
      <Router>
        <div className="app">
          <Route exact path="/" component={IndexPage} />
          <Route exact path="/Upload" component={UploadVideoPage} />
        </div>
      </Router>
    );
  }
}

export default App;