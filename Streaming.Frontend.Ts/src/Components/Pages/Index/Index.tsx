import * as React from 'react';
import VideoListItem from '../../Blocks/VideoListItem/VideoListItem';
import { AppContext } from '../../../AppContext';
import VideoMetadata from '../../../Models/VideoMetadata';

export default class Index extends React.Component<{}, { videos: VideoMetadata[] }>{
    constructor(props) {
        super(props);
        this.state = {
            videos: []
        }

        this.deleteVideoRequest = this.deleteVideoRequest.bind(this);
    }
    
    async componentDidMount() {
        let jsonData = await this.context.streamingApi.getVideos({});
        this.setState ({
            videos: jsonData
        });
    }

    async deleteVideoRequest(videoId) {
        await this.context.streamingApi.deleteVideo(videoId);
        this.setState({
            videos: this.state.videos.filter(x => x.videoId !== videoId)
        });
    }

    render() {
        return (
            <div className="indexPage">
                <div className="container">
                    {
                        this.state.videos.map((video, i) => {
                            return <VideoListItem key={video.videoId}
                                                  title={video.title} 
                                                  description={video.description}
                                                  videoId={video.videoId}
                                                  thumbnailUrl={video.thumbnailUrl}
                                                  onDeleteVideo={this.deleteVideoRequest} />
                        })
                    }
                </div>
            </div>
        );
    }
}
Index.contextType = AppContext;