import React from 'react';
import Navbar from '../../components/navbar/Navbar';
import UploadVideoForm from '../../components/forms/uploadVideoForm/uploadVideoForm'
import { Config } from '../../shared/config';
import { AppContext } from '../../AppContext';

class UploadVideoPage extends React.Component{
    constructor(props) {
        super(props);
        this.uploadVideo = this.uploadVideo.bind(this);
    }

    async uploadVideo(data) {
        await this.context.streamingApi.uploadVideo(data);
    }

    render() {
        return (
            <div className="uploadVideoPage">
                <Navbar />
                <UploadVideoForm submit={this.uploadVideo} apiDefinition = {{
                    'post': `${Config.apiPath}/video`
                }} />
            </div>
        );
    }
}

UploadVideoPage.contextType = AppContext;
export default UploadVideoPage;