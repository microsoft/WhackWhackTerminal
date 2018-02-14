import { TermView } from './TermView';
import { VisualStudio } from './VsEventManager';

window.triggerEvent = (event, data) => {
    if (VisualStudio.isEventType(event)) {
        VisualStudio.Events.triggerEvent(event, data);
    }
};

console.log(document.readyState);
if (document.readyState !== 'loading') {
    let termView = new TermView('Fira Code', 14, 'C:\\');
} else {
    document.addEventListener("DOMContentLoaded", function (event) {
        console.log('event load fired');
        let termView = new TermView('Fira Code', 14, 'C:\\');
    });
}