export const AsyncFunctions = {
    timeout: function (ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    },
    auth0: {
        checkSession: function(auth0) {
            return new Promise((resolve, reject) => auth0.checkSession({}, (err, authResult) => 
                {
                    if (authResult)
                        resolve(authResult);
                    else
                        reject(err);
                }))
        },

        parseHash: function(auth0) {
            return new Promise((resolve, reject) => auth0.parseHash((err, authResult) => resolve(authResult)))
        }
    },
    blob: {
        readAsArrayBuffer(blob) {
            return new Promise((resolve) => {
                var a = new FileReader();
                a.readAsArrayBuffer(blob);
                a.onloadend = function() {
                    resolve(a.result);
                }
            })
        }
    }
}