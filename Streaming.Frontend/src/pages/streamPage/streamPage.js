import React from 'react'
import Navbar from '../../components/navbar/Navbar';
import VideoPlayer from '../../components/blocks/videoPlayer/videoPlayer';
import { Config } from '../../shared/config';
import './streamPage.scss';
import { AppContext } from '../../AppContext';

export default class StreamPage extends React.Component {
    render() {
        return <div className="videoPage">
            <Navbar />
            <div className="container">
                <VideoPlayer manifestUrl={`http://localhost:8086/Live/Manifest/00000000-0001-0000-0000-000000000000` }/>                
            </div>
        </div>
    }
}

StreamPage.contextType = AppContext;