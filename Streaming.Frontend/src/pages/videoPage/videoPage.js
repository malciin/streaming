import React from 'react'
import Navbar from '../../components/navbar/navbar';
import VideoPlayer from '../../components/blocks/videoPlayer/videoPlayer';
import { Config } from '../../shared/config';
import './videoPage.scss';
import moment from 'moment/moment.js'

export default class VideoPage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            videoInfo: {}
        }
    }

    componentWillMount() {
        fetch(`${Config.apiPath}/Video/${this.props.match.params.id}`)
            .then(responsePromise => responsePromise.json())
            .then(jsonData => {
                this.setState ({
                    videoInfo: jsonData
                });
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