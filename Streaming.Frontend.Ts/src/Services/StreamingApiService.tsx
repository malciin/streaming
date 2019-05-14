import { Config } from '../Shared/Config';
import { AsyncFunctions } from "../Shared/AsyncFunctions";
import ApiService, { HttpMethod, RespType, AuthLevel } from "./ApiService";
import * as SparkMD5 from 'spark-md5';
import Auth0Service from './Auth0Service';
import VideoFormData from '../Models/Forms/VideoFormData';
import VideoMetadata from '../Models/VideoMetadata';
import LiveStreamMetadata from '../Models/LiveStreamMetadata';
import Mapper from './Mapper';
import VideoFilter from '../Models/Services/Streaming.Api/VideoFilter';
import Packaged from '../Models/Packaged';

export default class StreamingApiService extends ApiService {
    constructor(authContext: Auth0Service) {
        super(authContext);
        
        this.getVideos = this.getVideos.bind(this);
        this.getVideo = this.getVideo.bind(this);
        this.uploadVideo = this.uploadVideo.bind(this);
        this.deleteVideo = this.deleteVideo.bind(this);
        this.getStreamToken = this.getStreamToken.bind(this);
    }
 
    async getVideo(id:string): Promise<VideoMetadata> {
        let json = await this.makeApiRequest(`${Config.apiPath}/Video/${id}`,
            HttpMethod.GET);
        return Mapper.mapVideoMetadata(json);
    }
    
    async getVideos(filterObject: VideoFilter): Promise<Packaged<VideoMetadata>> {
        let json = await this.makeApiRequest(Config.apiPath + '/Video/Search',
            HttpMethod.POST, filterObject);
        let result: Packaged<VideoMetadata> = {
            Items: json.items.map(x => Mapper.mapVideoMetadata(x)),
            Details: {
                Count: json.details.count,
                TotalCount: json.details.totalCount
            }
        };
        return result;
    }
    
    async getLiveStream(id: string): Promise<void> {
        return await this.makeApiRequest(`${Config.apiPath}/Live/${id}`, 
            HttpMethod.GET, null);
    }
    
    async getLiveStreams(): Promise<LiveStreamMetadata[]> {
        let liveStreamsJson = await this.makeApiRequest(Config.apiPath + '/Live', 
            HttpMethod.POST, {
                offset: 0,
                howMuch: 10
            });
        
        return liveStreamsJson.map(x => Mapper.mapLiveStreamMetadata(x));
    }

    async getUploadToken() {
        return await this.makeApiRequest(`${Config.apiPath}/Video/UploadToken`,
            HttpMethod.GET, null, AuthLevel.User);
    }

    async deleteVideo(videoId: string) {
        return await this.makeApiRequest(`${Config.apiPath}/Video/${videoId}`,
            HttpMethod.DELETE, null, AuthLevel.User, RespType.Raw);
    }

    async getStreamToken() {
        return await this.makeApiRequest(`${Config.apiPath}/Live/Token`,
            HttpMethod.GET, null, AuthLevel.User);
    }

    async uploadVideo(video: VideoFormData, progressFunc: (percentProgress: Number) => void) {
        var token = (await this.getUploadToken()).token;

        var sendPartXmlRequest = function (idToken, uploadToken, bytes, md5Hash,
            soFarTransferedBytes, bytesToTransfer) {
            return new Promise((resolve, reject) => {
                var formData = new FormData();
                formData.append("UploadToken", uploadToken);
                formData.append("PartBytes", bytes);
                formData.append("PartMD5Hash", md5Hash);

                var xhr = new XMLHttpRequest();
                xhr.onload = function (e) {
                    if (xhr.readyState === 4) {
                        if (xhr.status === 204) {
                            resolve(xhr.status);
                        } else {
                            reject(xhr.status);
                        }
                    }
                };
                xhr.upload.addEventListener('progress', (info) => 
                    progressFunc((soFarTransferedBytes + info.loaded) / bytesToTransfer * 100));
                xhr.open("POST", `${Config.apiPath}/Video/UploadPart`, true);
                xhr.setRequestHeader('Authorization', `Bearer ${idToken}`);
                xhr.send(formData);
            });
        };

        var sendPart = async function (uploadToken, partBytes,
            soFarTransferedBytes, bytesToTransfer) {
            var partBinaryString = await AsyncFunctions.blob.readAsBinaryString(partBytes);
            var hasher = new SparkMD5(); 
            hasher.appendBinary(partBinaryString);
            var md5Hash = btoa(hasher.end(true));

            await sendPartXmlRequest(this.authContext.idToken, 
                uploadToken, partBytes, md5Hash, soFarTransferedBytes, bytesToTransfer);
        }.bind(this);

        var singlePartLength = 5000000;
        for(var i = 0; i<video.file.size; i += singlePartLength) {
            var startByte = i;
            var endByte = startByte + singlePartLength;
            if (endByte > video.file.size)
                endByte = video.file.size;
            await sendPart(token, video.file.slice(startByte, endByte), startByte, video.file.size);
            progressFunc(endByte / video.file.size * 100);
        }

        return await this.makeApiRequest(`${Config.apiPath}/Video`, HttpMethod.POST,
            {
                uploadToken: token,
                title: video.title,
                description: video.description
            }, AuthLevel.User, RespType.Raw);
    }
}