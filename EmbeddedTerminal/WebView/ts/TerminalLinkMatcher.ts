import { Terminal } from 'xterm';
import { TermView } from './TermView';

const LOCAL_LINK_PRIORITY = -2;

function handleLocalLink(event: MouseEvent, uri: string) {
    // We call the handle function on a small timeout to cause it to happen after the click event has fully
    // propogated. This ensures that focus properly transfers to the editor.
    setTimeout(() => window.external.HandleLocalLink(uri), 1);
    event.preventDefault();
}

function validateLocalLink(uri: string, callback: { (isValid: boolean): void }) {
    if (window.external.ValidateLocalLink(uri)) {
        callback(true);
    } else {
        callback(false);
    }
}

function registerLocalLinkHandler(terminal: Terminal) {
    let regex = new RegExp(window.external.GetLinkRegex());

    terminal.registerLinkMatcher(regex, handleLocalLink, {
        validationCallback: validateLocalLink,
        priority: LOCAL_LINK_PRIORITY
    });
}