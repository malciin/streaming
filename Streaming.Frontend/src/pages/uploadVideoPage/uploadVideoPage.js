import React from 'react';
import Navbar from '../../components/navbar/navbar';
import UploadVideoForm from '../../components/forms/uploadVideoForm/uploadVideoForm'
import DarkTheme from '../../shared/darkTheme'
import { Config } from '../../shared/config';

class UploadVideoPage extends React.Component{

    render() {
        console.log( Config )
        return (
            <DarkTheme>
            <div className="uploadVideoPage">
                <Navbar />
                <UploadVideoForm apiDefinition = {{
                    'post': `${Config.apiPath}/video`
                }} />
            </div>
            </DarkTheme>
        );
    }
}

export default UploadVideoPage;