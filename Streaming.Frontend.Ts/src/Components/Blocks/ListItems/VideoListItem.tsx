import * as React from 'react';
import { Link } from "react-router-dom";
import Claims from '../../../Models/Claims';
import './VideoListItem.scss';
import { AppContext } from '../../../AppContext';
import VideoMetadata from '../../../Models/VideoMetadata';
import { Config } from '../../../Shared/Config';
import * as moment from 'moment';
import Helpers from '../../../Shared/Helpers';

interface VideoListItemProps {
    videoModel: VideoMetadata,
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
        let len = moment.duration(this.props.videoModel.length);
        return <div className="video-list-item">
            <Link to={"/Vid/" + this.props.videoModel.videoId }>
            <div className="thumbnail">
                <div className="thumbnail-box">
                    { /* TODO: Remove temporary absolute url for video thumbnail */ }
                    <img className="thumbnail-image icon-video" alt="Video thumbnail" src={Config.apiPath + "\\" + this.props.videoModel.thumbnailUrl} />
                    <div className="video-length">{`${len.minutes()}m ${len.seconds()}s`}</div>
                </div>
            </div>
            </Link>
            <div className="video-metadata">
                <div className="video-title">
                    {this.props.videoModel.title} 
                </div>
                <div className="description text-secondary-color">
                    Uploaded by <Link to={"/User/" + this.props.videoModel.ownerNickname}>
                    {this.props.videoModel.ownerNickname}</Link>
                    {" " + Helpers.getHumanizedDateAgoDifference(this.props.videoModel.createdDate) + " ago"}
                </div>
                <div className="description text-secondary-color">
                    {this.props.videoModel.description}
                </div>
            </div>
            <div className="management">
                { !this.state.callbackInProgress && this.context.auth.haveClaim(Claims.canDeleteVideo) && 
                    <i className="icon-trash cursor-pointer" onClick={(any) => this.props.onDeleteVideo(this.props.videoModel.videoId)}></i> }
            </div>
        </div>;
    }
}
VideoListItem.contextType = AppContext;