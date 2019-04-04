import React from 'react'
import './videoListItem.scss'
import { Link } from "react-router-dom";
import { AppContext } from '../../../appContext';
import { Claims } from '../../../shared/claims';

export default class VideoListItem extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            inProgress: false
        }

        this.deleteVideo = this.deleteVideo.bind(this);
    }

    async deleteVideo(id) {
        this.setState({
            inProgress:true
        });
        await this.props.deleteVideoRequest(id, () => {
            this.setState({
                inProgress: false
            })
        });
    }
    
    render() {
        return <div className="video-list-item">
            <Link to={"/Vid/" + this.props.model.videoId }>
            <div className="thumbnail" onClick={this.handleClick}>
                <div className="thumbnail-box">
                    <img className="thumbnail-image icon-video" alt="Video thumbnail" src={this.props.model.thumbnailUrl} />
                </div>
            </div>
            </Link>
            <div className="video-metadata">
                <div className="video-title">
                    {this.props.model.title} 
                </div>
                <div className="description text-secondary-color">
                    {this.props.model.description}
                </div>
            </div>
            <div className="management">
                { !this.state.inProgress && this.context.auth.haveClaim(Claims.canDeleteVideo) && 
                    <i className="icon-trash cursor-pointer" onClick={this.deleteVideo.bind(this, this.props.model.videoId)}></i> }
            </div>
        </div>;
    }
}

VideoListItem.contextType = AppContext;