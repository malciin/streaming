def runSshCommand(connectionData, command)
{
    sh "sshpass -p '${connectionData.serverPassword}' ssh -o StrictHostKeyChecking=no ${connectionData.serverLogin}@${connectionData.serverIp} '${command}'";
}

def pushSshDirectoryToRemote(connectionData, directoryFrom, directoryTo)
{
    sh "sshpass -p '${connectionData.serverPassword}' scp -r \"${directoryFrom}\" ${connectionData.serverLogin}@${connectionData.serverIp}:${directoryTo}"
}

node('host') {
    stage('Build') 
    {
        sh "cd Streaming.Api"
        sh "dotnet restore"
        sh "dotnet publish --configuration Release --output ../Build"
    }

    stage('Deploy')
    {
        def uploadServerDirectory = "/home/marcin/streaminDemo"

        def connectionData = [
            serverIp: "142.93.173.48",
            serverLogin: null,
            serverPassword: null
        ]
        def serverIp = "142.93.173.48"
        def screenName = 'streamingDemo'
        def currentDirectory = pwd()
        withCredentials([usernamePassword(credentialsId: 'digitalOcean_ubuntu18', usernameVariable: 'USERNAME', passwordVariable: 'SERVER_PASSWORD')])
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