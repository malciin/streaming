import React from 'react'
import Navbar from '../../components/navbar/Navbar';
import VideoPlayer from '../../components/blocks/videoPlayer/videoPlayer';
import { Config } from '../../shared/config';
import './VideoPage.scss';
import moment from 'moment/moment.js'
import { AppContext } from '../../AppContext';

export default class VideoPage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            videoInfo: {}
        }
    }

    async componentWillMount() {
        var videoInfo = await this.context.streamingApi.getVideo(this.props.match.params.id); 
        this.setState({
            videoInfo: videoInfo
        });
    }

    render() {

        var dateDifference = moment().diff(moment(this.state.videoInfo.createdDate));

        return <div className="videoPage">
            <Navbar />
            <div className="container">
                <VideoPlayer manifestUrl={`${Config.apiPath}/Video/Manifest/${this.props.match.params.id}` }/>
                <div className="videoInfo">
                    <div className="videoTitle">
                        <h2>{this.state.videoInfo.title}</h2>
                    </div>
                    <div className="videoCreation">
                        <h3>{this.state.videoInfo.createdDate && `${moment.duration(dateDifference).humanize()} ago`}</h3>
                    </div>
                </div>
                
            </div>
        </div>
    }
}

VideoPage.contextType = AppContext;