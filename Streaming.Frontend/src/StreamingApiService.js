import { Config } from "./shared/config";
import { AsyncFunctions } from "./shared/AsyncFunctions";
var SparkMD5 = require('spark-md5');

export default class ApiService {
    constructor(props) {
        this.authContext = props.auth;
        this.waitForAuth = this.waitForAuth.bind(this);
        this.getJsonRequestHeaders = this.getJsonRequestHeaders.bind(this);

        this.getVideos = this.getVideos.bind(this);
        this.getVideo = this.getVideo.bind(this);
        this.uploadVideo = this.uploadVideo.bind(this);
        this.deleteVideo = this.deleteVideo.bind(this);
    }

    // TODO: Propably not the correct way for waiting to silent authentication
    // Waiting for authentication
    async waitForAuth() {
        while (this.authContext.pendingSilentLogin)
        {
            await AsyncFunctions.timeout(10);
        }
    }

    getJsonRequestHeaders() {
        var headers = {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        }
        if (this.authContext.idToken) {
            headers['Authorization'] = `Bearer ${this.authContext.idToken}`;
        }
        return headers;
    }

    async getVideos(filterObject) {
        const response = await fetch(Config.apiPath + '/Video/Search', {
            method: 'POST',
            headers: this.getJsonRequestHeaders(),
            body: JSON.stringify({
                keywords: [],
                offset: 0,
                howMuch: 10
            })
        });

        return await response.json();
    }

    async getVideo(id) {
        const response = await fetch(`${Config.apiPath}/Video/${id}`, {
            headers: this.getJsonRequestHeaders()
        });
        return await response.json();
    }

    async getUploadToken() {
        await this.waitForAuth();
        const resp = await fetch(`${Config.apiPath}/Video/UploadToken`, {
            headers: this.getJsonRequestHeaders()
        });
        var jsonData = await resp.json();
        return jsonData.token;
    }

    async uploadVideo(data, progressFunc) {
        await this.waitForAuth();
        var token = await this.getUploadToken();
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

        return await fetch(`${Config.apiPath}/Video`, {
            method: 'POST',
            headers: this.getJsonRequestHeaders(),
            body: JSON.stringify({
                uploadToken: token,
                title: data.videoTitle,
                description: data.videoDescription
            })
        })
    }

    async deleteVideo(videoId) {
        await this.waitForAuth();
        return await fetch(`${Config.apiPath}/Video/${videoId}`, {
            method: 'DELETE',
            headers: this.getJsonRequestHeaders()
        })
    }
}