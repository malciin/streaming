import React from 'react';
import './indexPage.css'
import Navbar from '../../components/navbar/Navbar';
import VideoListItem from '../../components/blocks/videoListItem/videoListItem';
import { AppContext } from '../../AppContext';
import ButtonField from '../../components/blocks/buttonField/buttonField';

class IndexPage extends React.Component{
    constructor(props) {
        super(props);
        this.state = {
            videos: []
        }

        this.deletedVideoCallback = this.deletedVideoCallback.bind(this);
    }
    
    async componentDidMount() {
        var jsonData = await this.context.streamingApi.getVideos({});
        this.setState ({
            videos: jsonData
        });
    }

    deletedVideoCallback(videoId) {
        this.setState({
            videos: this.state.videos.filter(x => x.videoId != videoId)
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
                            }} deletedVideoCallback={this.deletedVideoCallback} />
                        })
                    }
                </div>
            </div>
        );
    }
}

IndexPage.contextType = AppContext;
export default IndexPage;