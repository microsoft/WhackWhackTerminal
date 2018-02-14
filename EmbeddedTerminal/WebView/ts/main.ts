import { TermView } from './TermView';
import { VisualStudio } from './VsEventManager';

window.triggerEvent = (event, data) => {
    if (VisualStudio.isEventType(event)) {
        VisualStudio.Events.triggerEvent(event, data);
    }
};

document.addEventListener("DOMContentLoaded", function (event) {
    let termView = new TermView('Fira Code', 14, 'C:\\');
});