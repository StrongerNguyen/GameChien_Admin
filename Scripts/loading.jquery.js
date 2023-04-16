var Loading = {
    show: function (element) {
        $(element).append("<div class='loading position-absolute w-100 h-100' style='top: 0; left: 0;'><div class='position-absolute w-100 h-100' style='opacity: 0.2; background:#111; '></div><div class='spinner-border text-primary position-absolute' style='top: 50%; left: 50%;opacity:1;margin-left:-12.5px;margin-top:-12.5px' role='status'><span class='sr-only'>Loading...</span></div></div>");
    },
    hide: function (element) {
        $(element).find(".loading").remove();
    }
}