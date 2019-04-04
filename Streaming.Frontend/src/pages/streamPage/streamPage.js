import React from 'react'
import Navbar from '../../components/navbar/navbar';
import VideoPlayer from '../../components/blocks/videoPlayer/videoPlayer';
import { Config } from '../../shared/config';
import './streamPage.scss';
import { AppContext } from '../../appContext';

export default class StreamPage extends React.Component {
    render() {
        return <div className="videoPage">
            <Navbar />
            <div className="container">
                <VideoPlayer manifestUrl={`${Config.apiPath}/Live/Manifest/00000000-0001-0000-0000-000000000000` }/>                
            </div>
        </div>
    }
}

StreamPage.contextType = AppContext;