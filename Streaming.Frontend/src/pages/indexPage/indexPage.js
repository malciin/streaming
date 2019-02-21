import React from 'react';
import './indexPage.css'
import Navbar from '../../components/navbar/navbar';
import { Config } from '../../shared/config';
import VideoListItem from '../../components/blocks/videoListItem/videoListItem';

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
                <Navbar auth={this.props.auth}/>
                <div className="container">
                    {
                        this.state.videos.map((video, i) => {
                            return <VideoListItem key={video.videoId} model={{
                                videoId: video.videoId,
                                createdDate: video.createdDate,
                                title: video.title,
                                description: video.description,
                                length: video.length,
                                thumbnailUrl: video.thumbnailUrl
                            }} />
                        })
                    }
                </div>
            </div>
        );
    }
}

export default IndexPage;