/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root of https://github.com/Microsoft/vscode for license information.
 *--------------------------------------------------------------------------------------------*/


function registerLinkMatcher(xterm) {
    const pathPrefix = '(\\.\\.?|\\~)';
    const pathSeparatorClause = '\\/';
    const excludedPathCharactersClause = '[^\\0\\s!$`&*()\\[\\]+\'":;]'; // '":; are allowed in paths but they are often separators so ignore them
    const escapedExcludedPathCharactersClause = '(\\\\s|\\\\!|\\\\$|\\\\`|\\\\&|\\\\*|(|)|\\+)';
    /** A regex that matches paths in the form /foo, ~/foo, ./foo, ../foo, foo/bar */
    const unixLocalLinkClause = '((' + pathPrefix + '|(' + excludedPathCharactersClause + '|' + escapedExcludedPathCharactersClause + ')+)?(' + pathSeparatorClause + '(' + excludedPathCharactersClause + '|' + escapedExcludedPathCharactersClause + ')+)+)';

    const winDrivePrefix = '[a-zA-Z]:';
    const winPathPrefix = '(' + winDrivePrefix + '|\\.\\.?|\\~)';
    const winPathSeparatorClause = '(\\\\|\\/)';
    const winExcludedPathCharactersClause = '[^\\0<>\\?\\|\\/\\s!$`&*()\\[\\]+\'":;]';
    /** A regex that matches paths in the form c:\foo, ~\foo, .\foo, ..\foo, foo\bar */
    const winLocalLinkClause = '((' + winPathPrefix + '|(' + winExcludedPathCharactersClause + ')+)?(' + winPathSeparatorClause + '(' + winExcludedPathCharactersClause + ')+)+)';

    /** As xterm reads from DOM, space in that case is nonbreaking char ASCII code - 160,
    replacing space with nonBreakningSpace or space ASCII code - 32. */
    const lineAndColumnClause = [
        '((\\S*) on line ((\\d+)(, column (\\d+))?))', // (file path) on line 8, column 13
        '((\\S*):line ((\\d+)(, column (\\d+))?))', // (file path):line 8, column 13
        '(([^\\s\\(\\)]*)(\\s?[\\(\\[](\\d+)(,\\s?(\\d+))?)[\\)\\]])', // (file path)(45), (file path) (45), (file path)(45,18), (file path) (45,18), (file path)(45, 18), (file path) (45, 18), also with []
        '(([^:\\s\\(\\)<>\'\"\\[\\]]*)(:(\\d+))?(:(\\d+))?)' // (file path):336, (file path):336:9
    ].join('|').replace(/ /g, '[' + '\u00A0' + ']');

    // Changing any regex may effect this value, hence changes this as well if required.
    const winLineAndColumnMatchIndex = 12;
    const unixLineAndColumnMatchIndex = 15;

    // Each line and column clause have 6 groups (ie no. of expressions in round brackets)
    const lineAndColumnClauseGroupCount = 6;

    /** Higher than local link, lower than hypertext */
    const CUSTOM_LINK_PRIORITY = -1;
    /** Lowest */
    const LOCAL_LINK_PRIORITY = -2;

    function TerminalLinkHandler(terminal) {
        this.terminal = terminal;
        this.localLinkRegex = new RegExp(winLocalLinkClause + '(' + lineAndColumnClause + ')');
    }

    TerminalLinkHandler.prototype.registerLocalLinkHandler = function() {

        return this.terminal.registerLinkMatcher(this.localLinkRegex, this.handleLocalLink, {
            validationCallback: this.validateLocalLink,
            priority: LOCAL_LINK_PRIORITY
        });
    }

    TerminalLinkHandler.prototype.handleLocalLink = function(event, uri) {
        window.external.HandleLocalLink(uri);
    }

    TerminalLinkHandler.prototype.validateLocalLink = function (link, element, callback) {
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