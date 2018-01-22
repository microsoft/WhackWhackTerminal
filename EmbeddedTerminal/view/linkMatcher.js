function registerLinkMatcher(xterm) {
    const LOCAL_LINK_PRIORITY = -2;

    function TerminalLinkHandler(terminal) {
        this.terminal = terminal;
        this.localLinkRegex = new RegExp(window.external.GetLinkRegex());
    }

    TerminalLinkHandler.prototype.registerLocalLinkHandler = function() {

        return this.terminal.registerLinkMatcher(this.localLinkRegex, this.handleLocalLink.bind(this), {
            validationCallback: this.validateLocalLink.bind(this),
            priority: LOCAL_LINK_PRIORITY
        });
    }

    TerminalLinkHandler.prototype.handleLocalLink = function(event, uri) {
		// We call the handle function on a small timeout to cause it to happen after the click event has fully
		// propogated. This ensures that focus properly transfers to the editor.
        setTimeout(function() { window.external.HandleLocalLink(uri); }, 1);
		event.preventDefault();
    }

    TerminalLinkHandler.prototype.validateLocalLink = function (link, callback) {
        document.getElementById("debug").innerText = "" + link + callback;
        if (window.external.ValidateLocalLink(link)) {
            callback(true);
        } else {
            callback(false);
        }
    }

    var linkHandler = new TerminalLinkHandler(xterm);
    linkHandler.registerLocalLinkHandler();
}