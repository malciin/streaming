import React from 'react'
import Hls from 'hls.js'
import './videoPlayer.scss'

export default class VideoPlayer extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            manifestUrl: this.props.manifestUrl,
            videoPlayed: false,
            hover: false
        }
        this.stopStartVideo = this.stopStartVideo.bind(this);
        this.handleMouseOver = this.handleMouseOver.bind(this);

        this.video = React.createRef();
    }

    componentDidMount() {
        var video = document.getElementById('video')
        this.setState({
            video: video
        })
        if(Hls.isSupported()) {
            var hls = new Hls();
            hls.loadSource(this.state.manifestUrl);
            hls.attachMedia(video);
            hls.on(Hls.Events.MANIFEST_PARSED,function() {
                video.play();
            });
            
            this.setState({videoPlayed: true});
        }
    }

    handleMouseOver() {
        console.log("Before: " + this.state.hover);
        this.setState({
            hover: !this.state.hover
        });
        
    }

    stopStartVideo(event)
    {
        console.log(this.video);
        if (this.video.current.paused)
        {
            this.video.current.play();
            this.setState({videoPlayed: true});
        }
        else
        {
            this.video.current.pause();
            this.setState({videoPlayed: false});
        }
    }

    render() {
        let icon = "";
        if (this.state.videoPlayed)
        {
            icon = "icon-pause-outline";
        }
        else
        {
            icon = "icon-play-outline";
        }
        return <div id="video-wrapper" className="row" onMouseEnter={this.handleMouseOver}
        onMouseLeave={this.handleMouseOver}>
            <video 
                id="video"
                onClick={this.stopStartVideo} 
                ref={this.video}
                ></video>
            <div className={ `video-control${this.state.hover ? " showed-controls" : ""}`} >
                <i className={ icon + " play"} onClick={this.stopStartVideo}></i>
            </div>   
        </div>
    }
}