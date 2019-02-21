import React from 'react'
import './videoListItem.scss'
import { Redirect } from 'react-router-dom'
import { Link } from "react-router-dom";

export default class VideoListItem extends React.Component {
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
        return <div className="video-list-item">
            <Link to={"/Vid/" + this.state.video.videoId }>
            <div className="thumbnail" onClick={this.handleClick}>
                <div className="thumbnail-box">
                    <img className="thumbnail-image icon-video" src={this.state.video.thumbnailUrl} />
                </div>
            </div>
            </Link>
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