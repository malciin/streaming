import { Config } from "../shared/config";
import { AsyncFunctions } from "../shared/asyncFunctions";
import ApiService from "./apiService";
var SparkMD5 = require('spark-md5');


export default class StreamingApiService extends ApiService {
    constructor(props) {
        super(props);

        this.getVideos = this.getVideos.bind(this);
        this.getVideo = this.getVideo.bind(this);
        this.uploadVideo = this.uploadVideo.bind(this);
        this.deleteVideo = this.deleteVideo.bind(this);
        this.getStreamToken = this.getStreamToken.bind(this);
    }

    async getStreams() {
        return await this.makeApiRequest(Config.apiPath + '/Live', 
            'POST', {
                offset: 0,
                howMuch: 10
            });
    }

    async getVideos(filterObject) {
        return await this.makeApiRequest(Config.apiPath + '/Video/Search',
            'POST', {
                keywords: [],
                offset: 0,
                howMuch: 10
            });
    }

    async getLiveStream(id) {
        return await this.makeApiRequest(`${Config.apiPath}/Live/${id}`, 
            'GET', null);
    }

    async getVideo(id) {
        return await this.makeApiRequest(`${Config.apiPath}/Video/${id}`,
            'GET', null);
    }

    async getUploadToken() {
        return await this.makeApiRequest(`${Config.apiPath}/Video/UploadToken`,
            'GET', null, true);
    }

    async deleteVideo(videoId) {
        return await this.makeApiRequest(`${Config.apiPath}/Video/${videoId}`,
            'DELETE', null, true, 'raw');
    }

    async getStreamToken() {
        return await this.makeApiRequest(`${Config.apiPath}/Live/Token`,
            'GET', null, true);
    }

    async uploadVideo(data, progressFunc) {
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
        for(var i = 0; i<data.video.size; i += singlePartLength) {
            var startByte = i;
            var endByte = startByte + singlePartLength;
            if (endByte > data.video.size)
                endByte = data.video.size;
            await sendPart(token, data.video.slice(startByte, endByte), startByte, data.video.size);
            progressFunc(endByte / data.video.size * 100);
        }

        return await this.makeApiRequest(`${Config.apiPath}/Video`, 'POST',
            {
                uploadToken: token,
                title: data.videoTitle,
                description: data.videoDescription
            }, true, 'raw');
    }
}