import * as React from 'react'
import VideoMetadata from '../../../Models/VideoMetadata';
import { Link } from "react-router-dom";
import { Config } from '../../../Shared/Config';
import VideoPlayer from '../../Blocks/VideoPlayer/VideoPlayer';
import { AppContext } from '../../../AppContext';
import './VideoPage.scss';
import Helpers from '../../../Shared/Helpers';

interface VideoPageProps {
    videoId: string
}

interface VideoPageState {
    video: VideoMetadata
}

export default class VideoPage extends React.Component<VideoPageProps, VideoPageState> {
    constructor(props: VideoPageProps) {
        super(props);
        this.state = {
            video: null
        }
    }
    
    async componentWillMount() {
        var videoInfo = await this.context.streamingApi.getVideo(this.props.videoId);
        this.setState({
            video: videoInfo
        });
    }

    render() {
        return <div className="video-page container">
            <div className="video-player-container">
                <VideoPlayer 
                    manifestUrl={ `${Config.apiPath}/Video/Manifest/${this.props.videoId}` } 
                    autoplay={false} />
            </div>
            {this.state.video &&
            <div className="video-info">
                
                <div className="title">
                    {this.state.video.title} 
                </div>
                <div className="details text-third-color">
                    Uploaded by <Link to={"/User/" + this.state.video.ownerNickname}>
                    {this.state.video.ownerNickname}</Link>
                    {" " + Helpers.getHumanizedDateAgoDifference(this.state.video.createdDate) + " ago"}
                </div>
                <div className="description text-secondary-color">
                    {this.state.video.description}
                </div>
            </div>}
        </div>
    }
}

VideoPage.contextType = AppContext;