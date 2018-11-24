import React from 'react'
import './videoList.scss'
import { Redirect } from 'react-router-dom'

export default class VideoList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            video: props.model,
            redirect: false
        }

        this.handleClick = this.handleClick.bind(this);
    }

    handleClick(event) {
        this.setState({
            redirect: true
        })
    }
    
    render() {
        // if (this.state.redirect) {
        //     return <Redirect to="/Upload" />;
        // }

        return <div className="videoListItem">
            <a href={"/Vid/" + this.state.video.videoId }>
            <div className="thumbnail" onClick={this.handleClick}>
                {!this.state.video.thumbnail && <div className="thumbnail"><span class="thumbnailIcon icon-video"></span></div>}
            </div>
            </a>
            <div className="video-metadata">
                <div className="video-title">
                    {this.state.video.title} 
                </div>
                <div className="description text-secondary-color">
                    {this.state.video.description}
                </div>
            </div>
        </div>;
    }
}