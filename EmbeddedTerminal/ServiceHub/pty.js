'use strict';

var os = require('os');
var pty = require('node-pty');

// you may need to edit a file that gets pulled down, was a bug when i did. search and remove the obvious 'noif'
var rpc = require('vscode-jsonrpc');

const is32ProcessOn64Windows = process.env.hasOwnProperty('PROCESSOR_ARCHITEW6432');

const powerShellPath = `${process.env.windir}\\${is32ProcessOn64Windows ? 'Sysnative' : 'System32'}\\WindowsPowerShell\\v1.0\\powershell.exe`;
const cmdPath = `${process.env.windir}\\${is32ProcessOn64Windows ? 'Sysnative' : 'System32'}\\cmd.exe`;
const bashPath = `${process.env.windir}\\${is32ProcessOn64Windows ? 'Sysnative' : 'System32'}\\bash.exe`;

function ServicePty(stream, _host) {
    this.connection = rpc.createMessageConnection(new rpc.StreamMessageReader(stream), new rpc.StreamMessageWriter(stream));
    this.ptyConnection = null;

    this.connection.onRequest('initTerm', (shell, cols, rows, start, argument) => this.initTerm(shell, cols, rows, start, argument));
    this.connection.onRequest('termData', (data) => this.termData(data));
    this.connection.onRequest('resizeTerm', (cols, rows) => this.resizeTerm(cols, rows));
    this.connection.onRequest('closeTerm', () => this.closeTerm());
    this.connection.listen();
}

ServicePty.prototype.initTerm = function (shell, cols, rows, startDir, argument) {
    if (this.ptyConnection !== null) {
        return;
    }

    var startupArg = ' ' + argument;
    switch (shell) {
        case 'Powershell':
            var shelltospawn = powerShellPath;
            break;
        case 'CMD':
            var shelltospawn = cmdPath;
            break;
        case 'WSLBash':
            var shelltospawn = bashPath;
            break;
        default:
            var shelltospawn = powerShellPath;
    }

    this.ptyConnection = pty.spawn(shelltospawn, startupArg, {
        name: 'vs-integrated-terminal',
        cols: cols,
        rows: rows,
        cwd: startDir,
        env: process.env
    });

    this.ptyConnection.on('data', (data) => this.connection.sendRequest('PtyData', [data]));
    this.ptyConnection.on('exit', (code) => this.connection.sendRequest('PtyExit', [code]));
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

ServicePty.prototype.closeTerm = function () {
    if (this.ptyConnection != null) {
        this.ptyConnection.destroy();
    }
}

exports.ServicePty = ServicePty;