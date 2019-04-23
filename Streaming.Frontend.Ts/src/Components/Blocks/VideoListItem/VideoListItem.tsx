import * as React from 'react';
import { Link } from "react-router-dom";
import Claims from '../../../Models/Claims';
import './VideoListItem.scss';
import { AppContext } from '../../../AppContext';

interface VideoListItemProps {
    videoId: string,
    thumbnailUrl: string,
    title: string,
    description: string,
    onDeleteVideo: (videoId: string) => Promise<void>;
}

export default class VideoListItem extends React.Component<VideoListItemProps, { callbackInProgress: boolean}> {
    constructor(props: VideoListItemProps) {
        super(props);
        this.state = {
            callbackInProgress: false
        };
    }

    render() {
        return <div className="video-list-item">
            <Link to={"/Vid/" + this.props.videoId }>
            <div className="thumbnail">
                <div className="thumbnail-box">
                    <img className="thumbnail-image icon-video" alt="Video thumbnail" src={this.props.thumbnailUrl} />
                </div>
            </div>
            </Link>
            <div className="video-metadata">
                <div className="video-title">
                    {this.props.title} 
                </div>
                <div className="description text-secondary-color">
                    {this.props.description}
                </div>
            </div>
            <div className="management">
                { !this.state.callbackInProgress && this.context.auth.haveClaim(Claims.canDeleteVideo) && 
                    <i className="icon-trash cursor-pointer" onClick={(any) => this.props.onDeleteVideo(this.props.videoId)}></i> }
            </div>
        </div>;
    }
}
VideoListItem.contextType = AppContext;