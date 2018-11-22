import React from 'react'
import Hls from 'hls.js'
import './videoPlayer.scss'

export default class VideoPlayer extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            videoId: this.props.videoId,
            manifestEndpoint: this.props.manifestEndpoint
        }
        console.log(this.props.manifestEndpoint);
        
    }

    componentDidMount() {
        var video = document.getElementById('video');
        
        if(Hls.isSupported()) {
            var hls = new Hls();
            hls.loadSource(this.state.manifestEndpoint + "/" + this.state.videoId);
            hls.attachMedia(video);
            hls.on(Hls.Events.MANIFEST_PARSED,function() {
                video.play();
            });
        }
    }

    render() {
        return <video id="video"></video>
    }
}