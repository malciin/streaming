import React from 'react';
import Navbar from '../../components/navbar/Navbar';
import UploadVideoForm from '../../components/forms/uploadVideoForm/uploadVideoForm'
import { Config } from '../../shared/config';

class UploadVideoPage extends React.Component{

    render() {
        return (
            <div className="uploadVideoPage">
                <Navbar />
                <UploadVideoForm apiDefinition = {{
                    'post': `${Config.apiPath}/video`
                }} />
            </div>
        );
    }
}

export default UploadVideoPage;