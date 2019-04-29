import * as React from 'react'
import LiveStreamMetadata from '../../../Models/LiveStreamMetadata';
import { AppContext } from '../../../AppContext';
import VideoPlayer from '../../Blocks/VideoPlayer/VideoPlayer';

interface LiveStreamPageProps {
    liveStreamId: string
}

interface LiveStreamPageState {
    liveStream: LiveStreamMetadata
}

export default class LiveStreamPage extends React.Component<LiveStreamPageProps, LiveStreamPageState> {
    constructor(props) {
        super(props);
        this.state = {
            liveStream: null
        }
    }

    async componentWillMount() {
        var jsonData = await this.context.streamingApi.getLiveStream(this.props.liveStreamId); 
        this.setState({
            liveStream: jsonData
        });
    }

    render() {
        return <div>{ 
            this.state.liveStream &&
            <VideoPlayer manifestUrl={this.state.liveStream.manifestUrl} autoplay={true} />
        }
        </div>
    }
}

LiveStreamPage.contextType = AppContext;