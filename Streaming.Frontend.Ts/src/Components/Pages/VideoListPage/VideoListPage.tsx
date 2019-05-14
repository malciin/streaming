import * as React from 'react';
import VideoListItem from '../../Blocks/ListItems/VideoListItem';
import { AppContext } from '../../../AppContext';
import VideoMetadata from '../../../Models/VideoMetadata';

export default class VideoListPage extends React.Component<{}, { videos: VideoMetadata[] }>{
    constructor(props) {
        super(props);
        this.state = {
            videos: []
        }

        this.deleteVideoRequest = this.deleteVideoRequest.bind(this);
    }
    
    async componentDidMount() {
        let videos = await this.context.streamingApi.getVideos({
            keywords: [],
            howMuch: 10,
            offset: 0
        });
        this.setState ({
            videos: videos.Items
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
                                videoModel={video}
                                onDeleteVideo={this.deleteVideoRequest} />
                        })
                    }
                </div>
            </div>
        );
    }
}

VideoListPage.contextType = AppContext;