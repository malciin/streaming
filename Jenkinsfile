def runSshCommand(connectionData, command)
{
    sh "sshpass -p '${connectionData.serverPassword}' ssh -o StrictHostKeyChecking=no ${connectionData.serverLogin}@${connectionData.serverIp} '${command}'";
}

def pushSshDirectoryToRemote(connectionData, directoryFrom, directoryTo)
{
    sh "sshpass -p '${connectionData.serverPassword}' scp -r \"${directoryFrom}\" ${connectionData.serverLogin}@${connectionData.serverIp}:${directoryTo}"
}

node('host') {
    stage('Build-backend') 
    {
        sh "cd Streaming.Api"
        sh "dotnet restore"
        sh "dotnet publish --configuration Release --output ../Build"
    }

    stage('Build-frontend')
    {
        sh "cd Streaming.Frontend"
        sh "npm i"
        sh "npm run build"
    }

    stage('Deploy')
    {
        def uploadServerDirectory = "/home/marcin/streaminDemo"

        def connectionData = [
            serverIp: "${env.SERVER_IP}",
            serverLogin: null,
            serverPassword: null
        ]

        def screenName = "${env.SCREEN_NAME}"
        def currentDirectory = pwd()
        withCredentials([usernamePassword(credentialsId: "${CREDENTIALS_ID}", usernameVariable: 'USERNAME', passwordVariable: 'SERVER_PASSWORD')])
        {
            connectionData.serverLogin         = env.USERNAME
            connectionData.serverPassword      = env.SERVER_PASSWORD
        }

        try {
            runSshCommand(connectionData, "screen -X -S ${screenName} quit")
        }
        catch (Exception e) {
            echo 'Error when removing the screen. Propably screen is not created, trying to preceed...'
        }
        runSshCommand(connectionData, "rm -rf ${uploadServerDirectory}")
        pushSshDirectoryToRemote(connectionData, "${currentDirectory}/build", "${uploadServerDirectory}")
        runSshCommand(connectionData, "screen -dmSL ${screenName} dotnet ${uploadServerDirectory}/jenkinsHello.dll")
    }
}