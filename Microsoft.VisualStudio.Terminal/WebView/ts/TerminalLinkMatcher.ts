import { Terminal } from 'xterm';
import { TermView } from './TermView';
import { Marshal } from './marshals';
const LOCAL_LINK_PRIORITY = -2;

function handleLocalLink(marshal: Marshal, event: MouseEvent, uri: string) {
    // We call the handle function on a small timeout to cause it to happen after the click event has fully
    // propogated. This ensures that focus properly transfers to the editor.
    setTimeout(() => marshal.handleLocalLink(uri), 1);
    event.preventDefault();
}

function validateLocalLink(marshal: Marshal, uri: string, callback: { (isValid: boolean): void }) {
    marshal.validateLocalLink(uri)
        .then(callback);
}

export async function registerLocalLinkHandler(marshal: Marshal, terminal: Terminal): Microsoft.Plugin.Promise<void> {
    let regex = new RegExp(await marshal.getLinkRegex());

    terminal.registerLinkMatcher(regex, (event, uri) => handleLocalLink(marshal, event, uri), {
        validationCallback: (uri, callback) => validateLocalLink(marshal, uri, callback),
        priority: LOCAL_LINK_PRIORITY
    });
}