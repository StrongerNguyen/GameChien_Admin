var Alert = {
    Loading: function (title = null, message = null, func) {
        this.Hide();
        let element = '<div id="alert">';
        element += '<div class="alert_mask"></div>';
        element += '<div class="alert_content">';
        element += '<div class="alert_icon alert_spin"></div>';
        element += '<h4 class="text-info">' + (title ?? 'Đang xử lý, chờ chút nhé...') + '</h4>';
        if (message != null && message != '') {
            element += '<div class="alert_message">' + message + '</div>';
        }
        element += '</div>';
        element += '</div>';
        document.getElementsByTagName('body')[0].insertAdjacentHTML('beforebegin', element);
        if (func != null) {
            func();
        }
    },
    Success: function (title = null, message = null, func = null) {
        this.Hide();
        let element = '<div id="alert">';
        element += '<div class="alert_mask"></div>';
        element += '<div class="alert_content">';
        element += '<div class="alert_icon text-success"><i class="fa fa-check" aria-hidden="true"></i></div>';
        element += '<h4 class="text-success">' + (title ?? 'Thành công.') + '</h4>';
        element += '<div class="alert_close position-absolute" onclick="document.getElementById(\'alert\').remove()"><i class="fa fa-remove" aria-hidden="true"></i></div>';
        if (message != null && message != '') {
            element += '<div class="alert_message">' + message + '</div>';
        }
        element += '</div>';
        element += '</div>';
        document.getElementsByTagName('body')[0].insertAdjacentHTML('beforebegin', element);
        if (func != null) {
            func();
        }
        else {
            setTimeout(function () {
                Alert.Hide();
            }, 1000);
        }
    },
    Error: function (title = null, message = null, func = null) {
        this.Hide();
        let element = '<div id="alert">';
        element += '<div class="alert_mask"></div>';
        element += '<div class="alert_content">';
        element += '<div class="alert_icon text-danger" style="font-size:50px;text-align:center"><i class="fa fa-exclamation-triangle" aria-hidden="true"></i></div>';
        element += '<h3 class="text-danger">' + (title ?? 'Thất bại.') + '</h3>';
        element += '<div class="alert_close position-absolute" onclick="document.getElementById(\'alert\').remove()"><i class="fa fa-remove" aria-hidden="true"></i></div>';
        if (message != null && message != '') {
            element += '<div class="alert_message">' + message + '</div>';
        }
        element += '</div>';
        element += '</div>';
        document.getElementsByTagName('body')[0].insertAdjacentHTML('beforebegin', element);
        if (func != null) {
            func();
        }
    },
    Show: function (isSuccess = null, content = null, isShowCloseButton = false, message = null, func = null) {
        this.Hide();
        let element = '<div id="alert">';
        element += '<div class="alert_mask"></div>';
        element += '<div class="alert_content">';
        if (isSuccess == null) {
            element += '<div class="alert_icon alert_spin"></div>';
            element += '<h4 class="text-info">' + (content ?? 'Đang xử lý, chờ chút nhé...') + '</h4>';
        }
        else if (isSuccess) {
            element += '<div class="alert_icon text-success"><i class="fa fa-check" aria-hidden="true"></i></div>';
            element += '<h4 class="text-success">' + (content ?? 'Thành công.') + '</h4>';
        }
        else if (!isSuccess) {
            element += '<div class="alert_icon text-danger" style="font-size:50px;text-align:center"><i class="fa fa-exclamation-triangle" aria-hidden="true"></i></div>';
            element += '<h3 class="text-danger">' + (content ?? 'Thất bại.') + '</h3>';
        }
        else {
            console.log('Alert.Show -> isSuccess unknown');
        }
        if (isShowCloseButton) {
            element += '<div class="alert_close position-absolute" onclick="document.getElementById(\'alert\').remove()"><i class="fa fa-remove" aria-hidden="true"></i></div>';
        }
        if (message != null && message != '') {
            element += '<div class="alert_message">' + message + '</div>';
        }
        element += '</div>';
        element += '</div>';
        document.getElementsByTagName('body')[0].insertAdjacentHTML('beforebegin', element);
        if (func != null) {
            func();
        }
    },
    Hide: function () {
        var alert = document.getElementById("alert");
        if (alert != null) alert.remove();
    }
}