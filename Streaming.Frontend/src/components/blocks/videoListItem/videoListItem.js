import React from 'react'
import './videoListItem.scss'
import { Redirect } from 'react-router-dom'
import { Link } from "react-router-dom";
import { AppContext } from '../../../AppContext';

export default class VideoListItem extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
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
        console.log(this.props);
        return <div className="video-list-item">
            <Link to={"/Vid/" + this.props.model.videoId }>
            <div className="thumbnail" onClick={this.handleClick}>
                <div className="thumbnail-box">
                    <img className="thumbnail-image icon-video" src={this.props.model.thumbnailUrl} />
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
                { this.context.auth.haveClaim("canDeleteVideo") && <i style={{
                   cursor: 'pointer'
                }} className="icon-trash" onClick={this.props.deleteVideoCallback.bind(null, this.props.model.videoId)}></i> }
            </div>
        </div>;
    }
}

VideoListItem.contextType = AppContext;