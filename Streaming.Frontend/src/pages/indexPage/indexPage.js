import React from 'react';
import './indexPage.css'
import Navbar from '../../components/navbar/navbar';
import VideoPlayer from '../../components/blocks/videoPlayer/videoPlayer';
import { Config } from '../../shared/config';
import VideoList from '../../components/videoList/videoList';

class IndexPage extends React.Component{
    render() {
        return (
            <div className="indexPage">
                <Navbar />

                <div className="container">
                    <VideoList model={{
                        title: "Siema",
                        description: "Desc",
                        length: "14:00:00:00.0000000"
                    }} />
                    {/* <VideoPlayer videoId="ceaea1ed-519b-414b-9e0d-79678ca2adcd" manifestEndpoint={`${Config.apiPath}/Video/Manifest`} /> */}
                </div>
            </div>
        );
    }
}

export default IndexPage;