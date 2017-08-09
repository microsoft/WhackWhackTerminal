'use strict';

var os = require('os');
var pty = require('node-pty');

// you may need to edit a file that gets pulled down, was a bug when i did. search and remove the obvious 'noif'
var rpc = require('vscode-jsonrpc');

const userHome = process.env[(process.platform == 'win32') ? 'USERPROFILE' : 'HOME'];
const shell = 'powershell.exe';

function ServicePty(stream, _host) {
    this.connection = rpc.createMessageConnection(new rpc.StreamMessageReader(stream), new rpc.StreamMessageWriter(stream));
    this.ptyConnection = null;

    this.connection.onRequest('initTerm', (shell, cols, rows, start) => this.initTerm(shell, cols, rows, start));
    this.connection.onRequest('termData', (data) => this.termData(data));
    this.connection.onRequest('resizeTerm', (cols, rows) => this.resizeTerm(cols, rows));
    this.connection.listen();
}

ServicePty.prototype.initTerm = function (shell, cols, rows, startDir) {
    switch (shell) {
        case 'Powershell':
            var shelltospawn = "powershell.exe";
            break;
        case 'CMD':
            var shelltospawn = "cmd.exe";
            break;
        default:
            var shelltospawn = "C:\\Windows\\sysnative\\bash.exe";
    }

    if (this.ptyConnection != null) {
        this.ptyConnection.destroy();
    }

    this.ptyConnection = pty.spawn(shelltospawn, [], {
        name: 'vs-integrated-terminal',
        cols: cols,
        rows: rows,
        cwd: startDir,
        env: process.env
    });

    this.ptyConnection.on('data', (data) => this.ptyData(data));
    this.ptyConnection.on('exit', (code) => this.ptyExit(code));
}

ServicePty.prototype.termData = function (data) {
    if (this.ptyConnection != null) {
        this.ptyConnection.write(data);
    }
}

ServicePty.prototype.resizeTerm = function (cols, rows) {
    if (this.ptyConnection != null) {
        this.ptyConnection.resize(cols, rows);
    }
}

ServicePty.prototype.ptyData = function (data) {
    this.connection.sendRequest('PtyData', [data]);
}

ServicePty.prototype.ptyExit = function (code) {
    this.connection.sendRequest('ReInitTerm', [code]);
}

exports.ServicePty = ServicePty;