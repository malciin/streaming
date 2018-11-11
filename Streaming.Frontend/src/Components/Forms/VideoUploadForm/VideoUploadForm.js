import React from 'react';
import './VideoUploadForm.css'

class VideoUploadForm extends React.Component {
    render() {
        return (
            <div className="videoUploadForm">
                <form enctype="multipart/form-data"></form>
                <label>
                    Video title
                    <input type="text" name="title" />
                </label>
                <label>
                    Description
                    <input type="text" name="description" />
                </label>
                <input type="file" name="file" />
                <input type="submit" />
            </div>
        );
    }
}

export default VideoUploadForm;