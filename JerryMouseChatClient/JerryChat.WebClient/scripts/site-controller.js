/// <reference path="http://code.jquery.com/jquery-1.10.2.js" />
/// <reference path="class.js" />
/// <reference path="persister.js" />

var controller = (function () {
    var rootUrl = "http://jerrymousechat.apphb.com/api/";

    var controller = Class.create({
        init: function (selector) {
            this.selector = selector;
            this.persister = new persister.get(rootUrl);
            this.updater = null;
            this.interval = 5000;
        },
        start: function () {
            if (this.persister.isUserLoggedIn()) {
                this.loadChat();
            }
            else {
                this.loadLoginForm();
            }
            
            this.attachEventHandlers();

            $("#messages-box").scrollTop(2000);
        },
        attachEventHandlers: function () {
            var self = this;
            var wrapper = $(this.selector);

            wrapper.on("click", "#login-form #btn-register-form", function () {
                self.loadRegisterForm();
                return false;
            });

            wrapper.on("click", "#register-form #btn-login-form", function () {
                self.loadLoginForm();
                return false;
            });

            wrapper.on("click", "#register-form #btn-register", function () {
                var username = $("#tb-reg-username").val();
                var password = $("#tb-reg-password").val();
                var passwordRetyped = $("#tb-reg-password-retype").val();
                
                if (password === passwordRetyped) {
                    var user = {
                        username: username,
                        password: password
                    };

                    self.persister.users.register(user, function() {
                        self.loadChat();
                    }, function () {
                        
                    });
                }
                else {
                    // error
                }

                return false;
            });
            
            wrapper.on("click", "#login-form #btn-login", function () {
                var username = $("#tb-login-username").val().trim();
                var password = $("#tb-login-password").val().trim();

                if (password.length > 0 && username.length > 0) {
                    var user = {
                        username: username,
                        password: password
                    };

                    self.persister.users.login(user, function () {
                        self.loadChat();
                    }, function (message) {
                        //noty({ type: 'error', text: message });
                    });
                }
                else {
                    // error
                }

                return false;
            });
            
            wrapper.on("click", "#btn-logout", function () {
                self.persister.users.logout(function () {
                    self.loadLoginForm();
                }, function (message) {
                    //noty({ type: 'error', text: message });
                });

                return false;
            });

            wrapper.on("mouseenter", "#rooms-tabs li", function() {
                $(this).children("a").toggle();
                return false;
            });
            
            wrapper.on("mouseleave", "#rooms-tabs li", function () {
                $(this).children("a").toggle();
                return false;
            });
            
            wrapper.on("click", "#btn-send-file", function () {
                $('#create-game-form').modal('show');
                return false;
            });
            
            wrapper.on("click", "#btn-create-room", function () {
                var username = $("#tb-login-username").val().trim();
                var password = $("#tb-login-password").val().trim();

                if (password.length > 0 && username.length > 0) {
                    var user = {
                        username: username,
                        password: password
                    };

                    self.persister.users.login(user, function () {
                        self.loadChat();
                    }, function (message) {
                        //noty({ type: 'error', text: message });
                    });
                }
                else {
                    // error
                }

                return false;
            });

            wrapper.on("click", "#rooms-tabs li", function () {
                self.loadMessages($(this).attr("data-id"));
                $(this).parent().children().removeClass("label-info");
                $(this).addClass("label-info");
                $(this).children("i").remove();
                return false;
            });
            
            wrapper.on("click", "#rooms-tabs li a", function () {
                var roomId = $(this).parent().attr("data-id");
                self.persister.rooms.leave(roomId, function () {
                    self.loadOpenedRooms();
                }, function() {

                });
                return false;
            });
            
            wrapper.on("click", "#btn-send-message", function() {
                var message = $("#message-text").val();
                var roomId = $("#message-text").attr("data-id");
                self.persister.messages.send(message, roomId, function() {
                    self.loadMessages(roomId);
                }, function() {

                });
                return false;
            });

            wrapper.on("click", "#active-rooms-box li a", function() {
                var gameId = $(this).parent().parent().attr("data-id");
                self.persister.rooms.join(gameId, function() {
                    self.loadOpenedRooms();
                }, function() {
                    //
                });
            });

            wrapper.on("click", "#btn-create-room-dialog", function() {
                $("#create-room-box").modal("show");
                return false;
            });
            
            wrapper.on("click", "#btn-create-room", function () {
                var roomName = $("#tb-room-name").val();
                var password = $("#tb-room-password").val();

                var roomData = {
                    name: roomName,
                    password: password
                };

                self.persister.rooms.create(roomData, function() {
                    self.loadOpenedRooms();
                    $("#create-room-box").modal("hide");
                }, function() {

                });
            });
        },
        loadChat: function () {
            $("#login-form").hide();
            $("#register-form").hide();
            $("#chat-container").show();
            
            $("#sp-username").text(this.persister.username);
            
            this.loadActiveRooms();
            this.loadActiveUsers();
            this.loadOpenedRooms();

            var self = this;
            this.updater = setInterval(function () {
                self.loadActiveRooms();
                self.loadActiveUsers();
            }, this.interval);
        },
        loadLoginForm: function() {
            $("#login-form").show();
            $("#register-form").hide();
            $("#chat-container").hide();
        },
        loadRegisterForm: function() {
            $("#register-form").show();
            $("#login-form").hide();
            $("#chat-container").hide();
        },
        loadActiveRooms: function() {
            this.persister.rooms.all(function(data) {
                var list = $("#active-rooms-box ul");
                list.empty();

                var li, span, icon, a;
                for (var i = 0; i < data.length; i++) {
                    li = $("<li/>");
                    li.addClass("label");
                    li.text(" " + data[i].name);
                    li.attr("data-id", data[i].id);

                    icon = $("<i/>");
                    if (!data[i].isLocked) {
                        icon.addClass("icon-th-list");
                    } else {
                        icon.addClass("icon-lock");
                    }
                    li.prepend(icon);

                    span = $("<span/>");
                    span.addClass("pull-right");
                    span.text(" " + data[i].usersCount);

                    a = $("<a/>");
                    icon = $("<i/>");
                    icon.addClass("icon-plus");
                    a.append(icon);
                    
                    icon = $("<i/>");
                    icon.addClass("icon-user");

                    span.prepend(icon);
                    span.prepend(a);

                    li.append(span);
                    list.append(li);;
                }
            }, function() {
                //
            });
        },
        loadActiveUsers: function() {
            this.persister.users.online(function (data) {
                var list = $("#active-users-box ul");
                list.empty();

                var li, span, icon, input;
                for (var i = 0; i < data.length; i++) {
                    li = $("<li/>");
                    li.addClass("label");
                    li.addClass("label-info");
                    li.text(" " + data[i].username);
                    li.attr("data-id", data[i].id);

                    icon = $("<i/>");
                    icon.addClass("icon-user");
                    li.prepend(icon);

                    list.append(li);;
                }
            }, function () {
                //
            });
        },
        loadOpenedRooms: function() {
            this.persister.users.me(function (data) {
                var list = $("#rooms-tabs ul");
                list.empty();

                var pubnub = PUBNUB.init({
                    publish_key: 'pub-c-579e5400-3dc9-4ec4-829c-9cd214313647',
                    subscribe_key: 'sub-c-3507fb0e-057f-11e3-991c-02ee2ddab7fe'
                });

                var li, a, icon;
                for (var i = 0; i < data.rooms.length; i++) {
                    li = $("<li/>");
                    li.addClass("label");
                    li.text(data.rooms[i].name + " ");
                    li.attr("data-id", data.rooms[i].id);
                    $("#message-text").attr("data-id", data.rooms[i].id);
                    a = $("<a/>");

                    icon = $("<i/>");
                    icon.addClass("icon-remove");
                    icon.addClass("icon-white");
                    icon.addClass("pull-right");
                    a.append(icon);

                    li.append(a);
                    list.append(li);

                    pubnub.subscribe({
                        channel: "room-" + data.rooms[i].id,
                        message: function(id) {
                            var list = $("#rooms-tabs ul li");
                            for (var j = 0; j < list.length; j++) {
                                if ($(list[j]).attr("data-id") == id) {
                                    $(list[j]).append("<i class='icon-envelope'></i>");
                                }
                            }
                        }
                    });
                }
            }, function () {
                //
            });
        },
        loadMessages: function (roomId) {
            $("#message-text").attr("data-id", roomId);

            this.persister.messages.all(roomId, function (data) {
                var list = $("#messages-list");
                list.empty();
                
                var li;
                for (var i = 0; i < data.length; i++) {
                    li = $("<li/>");
                    li.text(data[i].author + ": (" + data[i].date + ") " + data[i].content);
                    list.append(li);
                }
            }, function() {

            });
        }
    });

    return {
        get: function (selector) {
            return new controller(selector);
        }
    };
}());

$(function () {
    var client = controller.get("body");
    client.start();
});

