const express = require('express');
const app = express();
const bodyParser = require('body-parser');
const util = require('util');
const exec = util.promisify(require('child_process').exec);



app.use(bodyParser.json());

app.post('/SendFile', async (req, resp) => {
    let tsInfo = `grep -i -B2 '${req.body.url.split('/')[1]}' ${req.body.m3u8} | grep \\#`
    let command = `curl -F "file=@${req.body.file}" -F "file_info=\`${tsInfo}\`" ${process.env.UPLOAD_FILE_ENDPOINT}`;
    console.log(`[LOCAL HTTP SERVER] Sending video ${req.body.file}`);
    const { stdout, stderr } = await exec(command);
    resp.send('0');
})

app.listen(8080, () => {
})