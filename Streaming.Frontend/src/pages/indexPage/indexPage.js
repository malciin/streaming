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

        this.deleteVideo = this.deleteVideo.bind(this);
    }
    componentDidMount() {
        this.context.streamingApi.getVideos({}, jsonData =>
            this.setState ({
                videos: jsonData
            }));
    }

    deleteVideo(id) {
        this.context.streamingApi.deleteVideo(id, function(id) {
            this.setState({
                videos: this.state.videos.filter(x => x.videoId != id)
            });
        }.bind(this, id));
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
                            }} deleteVideoCallback={this.deleteVideo} />
                        })
                    }
                </div>
            </div>
        );
    }
}

IndexPage.contextType = AppContext;

export default IndexPage;