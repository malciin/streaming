import React from 'react';
import './liveStreamListItem.scss';
import { Link } from "react-router-dom";
import moment from 'moment/moment.js';

export default class LiveStreamListItem extends React.Component {
    constructor(props) {
        super(props);
        console.log(props.model);
    }
    
    render() {
        var dateDifference = moment().diff(moment(this.props.model.started));

        return <div className="live-stream-list-item">
            <div className="thumbnail">
                <Link to={"/Live/" + this.props.model.liveStreamId }>
                    <img src="video-thumbnail.jpg" />
                </Link>
            </div>
            <div className="metadata">
                <div className="title">
                    <span className="live-mark">Live</span> {this.props.model.title}
                </div>
                <div className="started">
                    Started {moment.duration(dateDifference).humanize()} ago
                </div>
            </div>
        </div>;
    }
}