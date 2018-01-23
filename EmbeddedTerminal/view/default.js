var termView;

function TermView() {
    this.resizeTimeout = null;
    fit.apply(Terminal);
    this.term = new Terminal({
        cursorBlink: true,
        cols: 80,
        rows: 24
    });
    this.term.open(document.getElementById('content'));
    this.term.fit();
    this.term.on('data', this.termData.bind(this));
    this.term.setOption('fontFamily', 'Fira Code');
    window.addEventListener("resize", this.resizeHandler.bind(this), false);

    this.initTerm();
    this.registerKeyboardHandlers();
    registerLinkMatcher(this.term);
}

TermView.prototype.solutionDir = function () {
    return window.external.GetSolutionDir();
}

TermView.prototype.copyString = function (stringToCopy) {
    window.external.CopyStringToClipboard(stringToCopy);
}

TermView.prototype.getClipboard = function () {
    return window.external.GetClipboard();
}

TermView.prototype.ptyData = function (data) {
    this.term.write(data);
}

TermView.prototype.initTerm = function () {
    var term = this.term;
    window.external.InitTerm(term.cols, term.rows, this.solutionDir());
}

TermView.prototype.termData = function (data) {
    window.external.TermData(data);
}

TermView.prototype.focus = function() {
	this.term.focus();
}

TermView.prototype.resizeHandler = function () {
    var term = this.term;
    var actualHandler = function () {
        term.fit();
        window.external.ResizeTerm(term.cols, term.rows);
    };

    var timeoutCallBack = function () {
        this.resizeTimeout = null;
        actualHandler();
    }

    // ignore resize events as long as an actualResizeHandler execution is in the queue
    if (!this.resizeTimeout) {
        this.resizeTimeout = setTimeout(timeoutCallBack.bind(this), 66);
    }
}

TermView.prototype.registerKeyboardHandlers = function () {
    var term = this.term;
    var copy = this.copyString.bind(this);
    var getClipboard = this.getClipboard.bind(this);
    var termData = this.termData.bind(this);

    term.attachCustomKeyEventHandler(function (event) {
        // capture Ctrl+C
        if (event.ctrlKey && event.keyCode == 67 && term.hasSelection()) {
            copy(term.getSelection());
            term.clearSelection();
            return false;
        // capture Ctrl+V
        } else if (event.ctrlKey && event.keyCode == 86) {
            return false;
        }

        return true;
    });

    window.addEventListener('contextmenu', function (event) {
        if (term.hasSelection()) {
            copy(term.getSelection());
            term.clearSelection();
        } else {
            var content = getClipboard()
            termData(content);
        }
    });
}

function getMethods(obj) {
    var result = [];
    for (var id in obj) {
        try {
            result.push(id + '\n');
        } catch (err) {
            result.push(id + ": inaccessible");
        }
    }
    return result;
}

function invokeTerm(method, arg) {
    termView[method](arg);
}

document.addEventListener("DOMContentLoaded", function (event) {
    termView = new TermView();
});

