
var persister = (function () {
    var username = localStorage.getItem("username");
    var sessionKey = localStorage.getItem("sessionKey");
    var userId = localStorage.getItem("userId");

    function saveUserData(data) {
        localStorage.setItem("username", data.username);
        localStorage.setItem("sessionKey", data.sessionKey);
        localStorage.setItem("userId", data.id);
        username = data.username;
        sessionKey = data.sessionKey;
        userId = data.id;
    }

    function removeUserData() {
        localStorage.removeItem("username");
        localStorage.removeItem("sessionKey");
        localStorage.removeItem("userId");
        username = null;
        sessionKey = null;
        userId = null;
    }

    var main = Class.create({
        init: function (url) {
            if (url[url.length - 1] != "/") {
                url += "/";
            }

            this.rootUrl = url;
            this.users = new users(this.rootUrl);
            this.rooms = new rooms(this.rootUrl);
            this.messages = new messages(this.rootUrl);
        },
        isUserLoggedIn: function () {
            var n = username != "" && username != null && username != undefined;
            var sk = sessionKey != "" && sessionKey != null && sessionKey != undefined;
            var u = userId != "" && userId != null && userId != undefined;
            return (n && sk && u) == true;
        },
        username: function() {
            return username;
        },
        userId: function() {
            
        }
    });
    
    var users = Class.create({
        init: function (url) {
            this.rootUrl = url + "users/";
        },
        register: function (user, success, error) {
            var url = this.rootUrl + "register";
            var userData = {
                username: user.username,
                password: CryptoJS.SHA1(user.username + user.password).toString()
            };

            httpRequester.post(url, userData, function(data) {
                saveUserData(data);
                success(data);
            }, function(err) {
                // TODO
                console.log(err);
            });
        },
        login: function (user, success, error) {
            var url = this.rootUrl + "login";
            var userData = {
                username: user.username,
                password: CryptoJS.SHA1(user.username + user.password).toString()
            };

            httpRequester.post(url, userData, function(data){
                saveUserData(data);
                success(data);
            }, function(err){
                // TODO
                console.log(err);
            })
        },
        logout: function (success, error) {
            var url = this.rootUrl + "logout/" + sessionKey;
            httpRequester.get(url, function() {
                removeUserData();
                success();
            }, function(err) {
                // TODO
                console.log(err);
            });
        },
        online: function (success, error) {
            var url = this.rootUrl + "online";
            httpRequester.get(url, function (data) {
                success(data);
            }, function (err) {
                // TODO
                console.log(err);
            });
        },
        me: function(success, error) {
            var url = this.rootUrl + "user/" + sessionKey;
            httpRequester.get(url, function(data) {
                success(data);
            }, function() {

            });
        },
        update: function (data, success, error) {
            // TODO
        }
    });
    
    var rooms = Class.create({
        init: function (url) {
            this.rootUrl = url + "rooms/";
        },
        create: function (room, success, error) {
            var url = this.rootUrl + "create";
            var roomData = {
                name: room.name,
                password: room.password,
                adminId: userId
            };
            httpRequester.post(url, roomData, function(data){
                success();
            }, function (err) {
                // TODO
                console.log(err);
            })
        },
        join: function (roomId, success, error) {
            var url = this.rootUrl + "join/" + roomId;
            var userData = {
                username: username,
                id: userId
            };

            httpRequester.put(url, userData, function() {
                success();
            }, function(err) {
                // TODO
                console.log(err);
            });
        },
        leave: function (roomId, success, error) {
            var url = this.rootUrl + "leave/" + roomId;
            var userData = {
                username: username,
                id: userId
            };
            httpRequester.put(url, userData, function () {
                success();
            }, function (err) {
                // TODO
                console.log(err);
            })
        },
        all: function (success, error) {
            var url = this.rootUrl + "get";
            httpRequester.get(url, function(data){
                success(data);
            }, function (err) {
                // TODO
                console.log(err);
            })
        }
    });
    
    var messages = Class.create({
        init: function (url) {
            this.rootUrl = url + "messages/";
        },
        send: function (message, roomId, success, error) {
            var url = this.rootUrl + "send/" + roomId;
            var messageBody = {
                content: message,
                author: username,
                date: new Date()
            };
            httpRequester.post(url, messageBody, function() {
                success();
            }, function() {

            });
        },
        all: function (roomId, success, error) {
            var url = this.rootUrl + "get/" + roomId;
            httpRequester.get(url, function (data) {
                success(data);
            }, function (err) {
                // TODO
                console.log(err);
            })
        }
    });

    return {
        get: function(url) {
            return new main(url);
        }
    };
})();