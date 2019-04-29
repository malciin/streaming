import * as React from 'react'
import VideoMetadata from '../../../Models/VideoMetadata';
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
        return <div className="videoPage">
            { this.state.video && <div className="container">
                <VideoPlayer 
                    manifestUrl={ `${Config.apiPath}/Video/Manifest/${this.state.video.videoId}` } 
                    autoplay={false} />

                <div className="videoInfo">
                    <div className="videoTitle">
                        <h2>{this.state.video.title}</h2>
                    </div>
                    <div className="videoCreation">
                        <h3>{`${Helpers.getHumanizedDateAgoDifference(this.state.video.createdDate)} ago`}</h3>
                    </div>
                </div>
                
            </div>}
        </div>
    }
}

VideoPage.contextType = AppContext;