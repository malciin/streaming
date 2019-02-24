import React from 'react';
import './indexPage.css'
import Navbar from '../../components/navbar/Navbar';
import VideoListItem from '../../components/blocks/videoListItem/videoListItem';
import { AppContext } from '../../AppContext';

class IndexPage extends React.Component{
    constructor(props) {
        super(props);
        this.state = {
            videos: []
        }
    }
    componentDidMount() {
        this.context.streamingApi.getVideos({}, jsonData =>
            this.setState ({
                videos: jsonData
            }));
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

IndexPage.contextType = AppContext;

export default IndexPage;