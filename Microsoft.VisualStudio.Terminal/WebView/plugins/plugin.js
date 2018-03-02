(function () {
    var cookie,
        pluginUrl = window.external.pluginUrl || (cookie = document.cookie.match(/(?:^|;)\s?pluginUrl=(.*?)(?:;|$)/)) && unescape(cookie[1]) || "plugin.b.js",
        scripts = document.getElementsByTagName('script');
    if (scripts && pluginUrl) {
        for (var i = 0; i < scripts.length; i++) {
            var script = scripts[i];
            var src = script.src;
            if (/(^|\\|\/)plugin.js$/.test(src)) {
                var newScript = document.createElement("script");
                newScript.src = pluginUrl;
                script.parentElement.insertBefore(newScript, script.nextSibling);
                break;
            }
        }
    }
})();
