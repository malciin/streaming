import * as React from 'react';
import { AppContext } from '../../../AppContext';
import Console from '../../Blocks/Console/Console';
import UploadVideoForm from '../../Forms/UploadVideoForm/UploadVideoForm';
import VideoFormData from '../../../Models/Forms/VideoFormData';
import UploadVideoStatus from '../../Blocks/UploadVideoStatus/UploadVideoStatus';

interface UploadVideoPageState {
    progress: number,
    videoFormData: VideoFormData,
    consoleOutput: string[]
}

class UploadVideoPage extends React.Component<{}, UploadVideoPageState>{

    private console: Console;

    constructor(props) {
        super(props);
        this.uploadVideo = this.uploadVideo.bind(this);

        this.state = {
            videoFormData: null,
            progress: 0.0,
            consoleOutput: ["- - - Upload Video Shell 0.22 - - -"]
        };

        this.pushOutputToConsole = this.pushOutputToConsole.bind(this);
        this.uploadVideo = this.uploadVideo.bind(this);
    }

    pushOutputToConsole(output: string) {
        this.setState({
            consoleOutput: [...this.state.consoleOutput, output]
        });
    }

    async uploadVideo(video: VideoFormData) {
        this.pushOutputToConsole("Video uploading...");
        this.setState({
            videoFormData: video
        });
        try {
            await this.context.streamingApi.uploadVideo(video, progress => this.setState({
                progress: progress
            }));
            this.pushOutputToConsole("Video successfully uploaded...");
            this.pushOutputToConsole("Video will start processing!");
        } catch (exception) {
            this.pushOutputToConsole(`Exception: ${exception.message}`);
        }
    }

    render() {
        return (
            <div>
                <div className="container">
                { !this.state.videoFormData && <UploadVideoForm submit={this.uploadVideo}/> }
                { this.state.videoFormData && 
                <UploadVideoStatus 
                    videoTitle={this.state.videoFormData.title}
                    videoDescription={this.state.videoFormData.description}
                    videoFileSize={this.state.videoFormData.file.size} 
                    progress={this.state.progress} /> }
                { this.state.videoFormData &&    
                    <Console 
                        barEnabled={false} title={'Progress'} consoleOutputBuffer={this.state.consoleOutput} inputEnabled={false}
                        handleConsoleOutput={this.pushOutputToConsole} />
                }
                </div>
            </div>
        );
    }
}

UploadVideoPage.contextType = AppContext;
export default UploadVideoPage;