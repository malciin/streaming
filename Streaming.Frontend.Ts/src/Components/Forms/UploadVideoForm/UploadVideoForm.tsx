import * as React from 'react';
import VideoFileDropzone from '../../Blocks/Dropzone/VideoFileDropzone';
import TextField from '../../Blocks/TextField/TextField';
import ButtonField from '../../Blocks/ButtonField/ButtonField';
import VideoFormData from '../../../Models/Forms/VideoFormData';
import FileUploadCard from '../../Blocks/Dropzone/FileUploadCard';

export interface UploadVideoFormProps {
    submit: (video: VideoFormData) => void;
}

export interface UploadVideoFormState {
    videoTitle: string,
    videoDescription: string,
    videoFile: File
}

class UploadVideoForm extends React.Component<UploadVideoFormProps, UploadVideoFormState> {
    constructor(props) {
        super(props);
        
        this.state = {
            videoFile: null,
            videoDescription: "",
            videoTitle: ""
        }

        this.submit = this.submit.bind(this);
        this.onDropFile = this.onDropFile.bind(this);
        this.handleFieldChange = this.handleFieldChange.bind(this);
    }

    handleFieldChange (fieldName: string, value: string): void {
        // @ts-ignore
        this.setState({
            [fieldName]: value
        });
    }

    onDropFile(videoFile: File): void {
        this.setState({
            videoFile: videoFile
        });
    };

    submit(): void {
        let videoSubmit: VideoFormData = {
            title: this.state.videoTitle,
            description: this.state.videoDescription,
            file: this.state.videoFile
        };
        this.props.submit(videoSubmit);
    }
    
    render() {
        return (
            <div className="container">
                <TextField onChange={this.handleFieldChange} label="Please enter a video title" name="videoTitle" />
                <TextField onChange={this.handleFieldChange} label="Please enter a video description" name="videoDescription"/>
                { !this.state.videoFile && <VideoFileDropzone 
                    onDropFile={this.onDropFile} 
                    acceptedExtensions={['.mp4']}
                    acceptedFileTypes={['video/mp4']} /> }
                { this.state.videoFile && <FileUploadCard 
                    file={this.state.videoFile}    
                    onDeleteFile={(file: File) => this.setState({ videoFile: null })} />
                }
                { this.state.videoFile && <ButtonField 
                    style={{
                        marginTop: "5"
                    }} onClick={this.submit} buttonType="btn-primary" label="Upload" center /> }
            </div>
        );
    }
}

export default UploadVideoForm;