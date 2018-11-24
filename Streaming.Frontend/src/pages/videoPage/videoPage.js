import React from 'react'
import Navbar from '../../components/navbar/navbar';
import VideoPlayer from '../../components/blocks/videoPlayer/videoPlayer';
import { Config } from '../../shared/config';

export default class VideoPage extends React.Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        fetch(`${Config.apiPath}/Video/${this.props.match.params.id}`)
            .then(x => console.log(x));
    }

    render() {
        return <div className="videoPage">
            <Navbar />
            <div className="container">
                <VideoPlayer manifestUrl={`${Config.apiPath}/Video/Manifest/${this.props.match.params.id}` }/>
            </div>
        </div>
    }
}