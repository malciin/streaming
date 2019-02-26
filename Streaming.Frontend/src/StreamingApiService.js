import { Config } from "./shared/config";
import { request } from "https";
import { AsyncFunctions } from "./shared/AsyncFunctions";
var crypto = require("crypto-js");

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

    async getVideo(id, callback) {
        const response = await fetch(`${Config.apiPath}/Video/${id}`, {
            headers: this.getJsonRequestHeaders()
        });

        return await response.json();
    }

    async getUploadToken() {
        await this.waitForAuth();
        const resp = await fetch(`${Config.apiPath}/Video/UploadToken`);
        return await resp.text();
    }

    async uploadVideo(data) {
        await this.waitForAuth();
        var token = await this.getUploadToken();

        var sendPartXmlRequest = function (idToken, uploadToken, bytes, md5Hash) {
            return new Promise((resolve, reject) => {
                var formData = new FormData();
                formData.append("UploadToken", uploadToken);
                formData.append("PartBytes", bytes);
                formData.append("PartMD5Hash", md5Hash);

                var xhr = new XMLHttpRequest();
                xhr.onload = function (e) {
                    if (xhr.readyState === 4) {
                        if (xhr.status === 200) {
                            resolve(xhr.responseText);
                        } else {
                            reject(xhr.statusText);
                        }
                    }
                };
                xhr.open("POST", `${Config.apiPath}/Video/UploadPart`, true);
                xhr.setRequestHeader('Authorization', `Bearer ${idToken}`);
                xhr.send(formData);
            });
        }

        var sendPart = async function (uploadToken, partBytes) {
            var partBytesArray = await AsyncFunctions.blob.readAsArrayBuffer(partBytes);
            var wordArray = crypto.lib.WordArray.create(partBytesArray);
            var md5Hash = crypto.MD5(wordArray).toString(crypto.enc.Base64);

            await sendPartXmlRequest(this.authContext.idToken, 
                uploadToken, partBytes, md5Hash);
        }.bind(this);

        var singlePartLength = 4000000;
        for(var i = 0; i<data.video.size; i += singlePartLength) {
            var startByte = i;
            var endByte = startByte + singlePartLength;
            if (endByte > data.video.size)
                endByte = data.video.size;
            await sendPart(token, data.video.slice(startByte, endByte));
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

    async deleteVideo(videoId, callback) {
        await this.waitForAuth();
        return await fetch(`${Config.apiPath}/Video/${videoId}`, {
            method: 'DELETE',
            headers: this.getJsonRequestHeaders()
        })
    }
}