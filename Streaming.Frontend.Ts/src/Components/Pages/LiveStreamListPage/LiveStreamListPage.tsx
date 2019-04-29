import * as React from 'react';
import LiveStreamMetadata from '../../../Models/LiveStreamMetadata';
import LiveStreamListItem from '../../Blocks/ListItems/LiveStreamListItem';
import { AppContext } from '../../../AppContext';

interface LiveStreamListPageState {
    liveStreams: LiveStreamMetadata[]
};

class LiveStreamListPage extends React.Component<{}, LiveStreamListPageState>{
    constructor(props) {
        super(props);
        this.state = {
            liveStreams: []
        }
    }
    
    async componentDidMount() {
        var liveStreams = await this.context.streamingApi.getStreams();
        this.setState ({
            liveStreams: liveStreams
        });
    }

    render() {
        return (
            <div className="indexPage">
                <div className="container">
                    {
                        this.state.liveStreams.map((liveStream, i) => {
                            return <LiveStreamListItem key={liveStream.liveStreamId}
                                liveStreamModel={liveStream} />
                        })
                    }
                </div>
            </div>
        );
    }
}

LiveStreamListPage.contextType = AppContext;
export default LiveStreamListPage;