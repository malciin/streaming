import * as React from 'react'
import videojs from 'video.js'
import './VideoPlayer.scss'

interface VideoPlayerProps {
    autoplay: boolean
    manifestUrl: string
}

export default class VideoPlayer extends React.Component<VideoPlayerProps> {
    private videoJs: any;
    private htmlVideoElement: HTMLVideoElement;

    constructor(props) {
        super(props);
    }

    componentDidMount() {
        var manifestUrl = this.props.manifestUrl;
        this.videoJs = videojs(this.htmlVideoElement,
            {
                sources: [{
                  src: manifestUrl,
                  type: 'application/x-mpegURL'
                }]
            },
            async function onPlayerReady() {
                var customSpinner = document.createElement('div');
                customSpinner.className = 'vjs-custom-spinner';
                customSpinner.innerHTML = '<div class="vjs-custom-spinner-bounce-1"></div><div class="vjs-custom-spinner-bounce-2"></div>'
                document.getElementsByClassName('vjs-loading-spinner')[0].appendChild(customSpinner);
            });
        
        if (this.props.autoplay)
        {
            this.videoJs.on('canplay', function() {
                this.muted(true);
                this.play();
            })
        }
    }

    componentWillUnmount() {
        this.videoJs.dispose();
    }

    render() {
        return (
            <div>
                <video className="video-js" ref={el => {this.htmlVideoElement = el;}} controls>
                </video>
            </div>
        )
    }
}