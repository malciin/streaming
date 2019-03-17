import React from 'react';
import Navbar from '../../components/navbar/Navbar';
import { AppContext } from '../../AppContext';
import UploadVideoForm from '../../components/forms/uploadVideoForm/uploadVideoForm';
import UploadingVideoStatus from '../../components/blocks/uploadingVideoStatus/uploadingVideoStatus';

class UploadVideoPage extends React.Component{
    constructor(props) {
        super(props);
        this.uploadVideo = this.uploadVideo.bind(this);
        this.state = {
            uploading: false,
            progress: 0.0,
            output: ["- - - Upload Video Shell 0.22 - - -"]
        };

        this.uploadingVideoState = <UploadingVideoStatus />
    }

    async uploadVideo(data) {
        this.videoInfo = {
            title: data.videoTitle,
            description: data.videoDescription,
            file: data.video
        }
        this.setState({
            uploading: true
        });
        await this.context.streamingApi.uploadVideo(data, progress => this.setState({
            progress: progress
        }));
        
    }

    render() {
        return (
            <div className="uploadVideoPage">
                <Navbar />
                { !this.state.uploading && <UploadVideoForm submit={this.uploadVideo}/> }
                { this.state.uploading && <UploadingVideoStatus video={this.videoInfo} progress={this.state.progress} output={this.state.output} /> }
            </div>
        );
    }
}

UploadVideoPage.contextType = AppContext;
export default UploadVideoPage;