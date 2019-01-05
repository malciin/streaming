import React from 'react';
import './indexPage.css'
import Navbar from '../../components/navbar/navbar';
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

        fetch(Config.apiPath + '/Video/Search', {
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
                </div>
            </div>
        );
    }
}

export default IndexPage;