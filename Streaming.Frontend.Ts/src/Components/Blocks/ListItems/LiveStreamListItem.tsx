import * as React from 'react';
import './LiveStreamListItem.scss';
import { Link } from "react-router-dom";
import LiveStreamMetadata from '../../../Models/LiveStreamMetadata';
import Helpers from '../../../Shared/Helpers';

export default class LiveStreamListItem extends React.Component<{ liveStreamModel: LiveStreamMetadata }> {
    constructor(props) {
        super(props);
    }
    
    render() {
        return <div className="live-stream-list-item">
            <div className="thumbnail">
                <Link to={"/Live/" + this.props.liveStreamModel.liveStreamId }>
                    <img src="video-thumbnail.jpg" />
                </Link>
            </div>
            <div className="metadata">
                <div className="title">
                    <span className="live-mark">Live</span> {this.props.liveStreamModel.title}
                </div>
                <div className="started">
                    Started {Helpers.getHumanizedDateAgoDifference(this.props.liveStreamModel.started)} ago
                </div>
            </div>
        </div>;
    }
}