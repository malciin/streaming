import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Link, BrowserRouter } from "react-router-dom";
import '../node_modules/bootstrap/dist/css/bootstrap.css'
import Navbar from './Components/Navbar/Navbar'
import Homepage from './Components/Homepage/Homepage'
import VideoUploadForm from './Components/Forms/VideoUploadForm/VideoUploadForm';

class App extends Component {
  render() {
    return (
      <Router>
        <div className="app">
          <Navbar />
          <Route exact path="/" component={Homepage} />
          <Route exact path="/Upload" component={VideoUploadForm} />
        </div>
      </Router>
    );
  }
}

export default App;
