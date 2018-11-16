def runSshCommand(connectionData, command)
{
    sh "sshpass -p '${connectionData.serverPassword}' ssh -o StrictHostKeyChecking=no ${connectionData.serverLogin}@${connectionData.serverIp} '${command}'";
}

def pushSshDirectoryToRemote(connectionData, directoryFrom, directoryTo)
{
    sh "sshpass -p '${connectionData.serverPassword}' scp -r \"${directoryFrom}\" ${connectionData.serverLogin}@${connectionData.serverIp}:${directoryTo}"
}

node('host') {
    stage('Checkout SCM') {
        checkout scm
    }

    stage('Build-backend') 
    {
        sh "cd Streaming.Api && dotnet restore"
        sh "cd Streaming.Api && dotnet publish --configuration Release --output ../Build"
    }

    stage('Build-frontend')
    {
        sh "cd Streaming.Frontend && npm i"
        sh "cd Streaming.Frontend && npm run build"
    }

    stage('Deploy')
    {
        def uploadBackendServerDirectory = "/home/marcin/streaming.backend"
		def uploadFrontendDirectory = "/home/marcin/streaming.frontend"
        def backendExecutableName = "Streaming.Api.dll"

        def connectionData = [
            serverIp: "${env.SERVER_IP}",
            serverLogin: null,
            serverPassword: null
        ]

        def screenName = "${env.SCREEN_NAME}"
        def currentDirectory = pwd()
        withCredentials([usernamePassword(credentialsId: "${env.CREDENTIALS_ID}", usernameVariable: 'USERNAME', passwordVariable: 'SERVER_PASSWORD')])
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
        runSshCommand(connectionData, "rm -rf ${uploadBackendServerDirectory}")
        runSshCommand(connectionData, "rm -rf ${uploadFrontendDirectory}")

        pushSshDirectoryToRemote(connectionData, "${currentDirectory}/Build", "${uploadBackendServerDirectory}")
        pushSshDirectoryToRemote(connectionData, "${currentDirectory}/Streaming.Frontend/build", "${uploadFrontendDirectory}")
        runSshCommand(connectionData, "screen -dmSL ${screenName} dotnet ${uploadBackendServerDirectory}/${backendExecutableName}")
    }
}