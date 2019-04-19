import React from 'react';
import Navbar from '../../components/navbar/navbar';
import LiveStreamListItem from '../../components/blocks/liveStreamListItem/liveStreamListItem';
import { AppContext } from '../../appContext';

class LiveStreamsPage extends React.Component{
    constructor(props) {
        super(props);
        this.state = {
            liveStreams: []
        }
    }
    
    async componentDidMount() {
        var jsonData = await this.context.streamingApi.getStreams();
        console.log(jsonData);
        this.setState ({
            liveStreams: jsonData
        });
    }

    render() {
        return (
            <div className="indexPage">
                <Navbar auth={this.props.auth}/>
                <div className="container">
                    {
                        this.state.liveStreams.map((liveStream, i) => {
                            return <LiveStreamListItem key={liveStream.liveStreamId} model={liveStream} />
                        })
                    }
                </div>
            </div>
        );
    }
}

LiveStreamsPage.contextType = AppContext;
export default LiveStreamsPage;