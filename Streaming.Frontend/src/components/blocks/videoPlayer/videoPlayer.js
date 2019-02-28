import React from 'react'
import videojs from 'video.js'
import './videoPlayer.scss'

export default class VideoPlayer extends React.Component {

    constructor(props) {
        super(props);
        this.videoNode = React.createRef();
    }

    componentDidMount() {
        var manifestUrl = this.props.manifestUrl;
        this.player = videojs(this.videoNode.current,
            {
                sources: [{
                  src: manifestUrl,
                  type: 'application/x-mpegURL'
                }]
            }, 
            function onPlayerReady() {
                var customSpinner = document.createElement('div');
                customSpinner.className = 'vjs-custom-spinner';
                customSpinner.innerHTML = '<div class="vjs-custom-spinner-bounce-1"></div><div class="vjs-custom-spinner-bounce-2"></div>'
                document.getElementsByClassName('vjs-loading-spinner')[0].appendChild(customSpinner)
                this.autoplay(true);
                this.controls(true);
            });
    }

    componentWillUnmount() {
        this.player.dispose();
    }

    render() {
        return (
            <div>
                <video className="video-js" ref={this.videoNode} controls>
                </video>
            </div>
        )
    }
}