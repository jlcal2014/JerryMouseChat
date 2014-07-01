
var httpRequester = (function () {
    function get(url, success, error) {
        $.ajax({
            url: url,
            contentType: "application/json",
            type: "GET",
            success: success,
            error: error,
            timeout: 15000
        });
    }

    function post(url, data, success, error) {
        $.ajax({
            url: url,
            data: JSON.stringify(data),
            contentType: "application/json",
            type: "POST",
            success: success,
            error: error,
            timeout: 15000
        });
    }
    
    function del(url, success, error) {
        $.ajax({
            url: url,
            contentType: "application/json",
            type: "DELETE",
            success: success,
            error: error,
            timeout: 15000
        });
    }
    
    function put(url, data, success, error) {
        $.ajax({
            url: url,
            data: JSON.stringify(data),
            contentType: "application/json",
            type: "PUT",
            success: success,
            error: error,
            timeout: 15000
        });
    }

    return {
        get: get,
        post: post,
        del: del,
        put: put
    }
})();