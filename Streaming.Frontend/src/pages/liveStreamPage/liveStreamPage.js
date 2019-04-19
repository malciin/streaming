import React from 'react'
import { AppContext } from '../../appContext';
import VideoPlayer from '../../components/blocks/videoPlayer/videoPlayer';

export default class LiveStreamPage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            liveStream: null
        }
    }
    async componentWillMount() {
        var jsonData = await this.context.streamingApi.getLiveStream(this.props.match.params.id); 
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