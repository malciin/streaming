import * as React from 'react'
import * as moment from 'moment';
import VideoMetadata from '../../../Models/VideoMetadata';
import { Config } from '../../../Shared/Config';
import VideoPlayer from '../../Blocks/VideoPlayer/VideoPlayer';
import { AppContext } from '../../../AppContext';
import './VideoPage.scss';

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

        this.getUploadDateAgoHumanize = this.getUploadDateAgoHumanize.bind(this);
    }
    
    async componentWillMount() {
        var videoInfo = await this.context.streamingApi.getVideo(this.props.videoId); 
        this.setState({
            video: videoInfo
        });
    }

    getUploadDateAgoHumanize(): string {
        var dateDifference = moment().diff(this.state.video.createdDate);
        return moment.duration(dateDifference).humanize();
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
                        <h3>{`${this.getUploadDateAgoHumanize()} ago`}</h3>
                    </div>
                </div>
                
            </div>}
        </div>
    }
}

VideoPage.contextType = AppContext;