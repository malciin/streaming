import React from 'react';
import './IndexPage.scss'
import Navbar from '../../components/navbar/Navbar';
import VideoListItem from '../../components/blocks/videoListItem/VideoListItem';
import { AppContext } from '../../AppContext';

class IndexPage extends React.Component{
    constructor(props) {
        super(props);
        this.state = {
            videos: []
        }

        this.deleteVideoRequest = this.deleteVideoRequest.bind(this);
    }
    
    async componentDidMount() {
        var jsonData = await this.context.streamingApi.getVideos({});
        this.setState ({
            videos: jsonData
        });
    }

    async deleteVideoRequest(videoId, callerCallback) {
        await this.context.streamingApi.deleteVideo(videoId);
        callerCallback();
        this.setState({
            videos: this.state.videos.filter(x => x.videoId !== videoId)
        });
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
                            }} deleteVideoRequest={this.deleteVideoRequest} />
                        })
                    }
                </div>
            </div>
        );
    }
}

IndexPage.contextType = AppContext;
export default IndexPage;