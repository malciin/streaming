import React from 'react';
import './indexPage.css'
import Navbar from '../../components/navbar/navbar';
import VideoPlayer from '../../components/blocks/videoPlayer/videoPlayer';
import { Config } from '../../shared/config';
import VideoList from '../../components/videoList/videoList';

class IndexPage extends React.Component{
    constructor(props) {
        super(props);
        this.state = {
            videos: []
        }
    }
    componentDidMount() {

        fetch('http://localhost:54321/Video/Search', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                Offset: 0,
                HowMuch: 10,
                Keywords: []
            })
        }).then(responsePromise => responsePromise.json())
        .then(jsonData => {
            this.setState ({
                videos: jsonData
            });
        })
    }

    render() {
        
        return (
            <div className="indexPage">
                <Navbar />
                <div className="container">
                    {
                        this.state.videos.map((video, i) => {
                            return <VideoList model={{
                                videoId: video.videoId,
                                createdDate: video.createdDate,
                                title: video.title,
                                description: video.description,
                                length: video.length
                            }} />
                        })
                    }
                    {/* <VideoPlayer videoId="ceaea1ed-519b-414b-9e0d-79678ca2adcd" manifestEndpoint={`${Config.apiPath}/Video/Manifest`} /> */}
                </div>
            </div>
        );
    }
}

export default IndexPage;