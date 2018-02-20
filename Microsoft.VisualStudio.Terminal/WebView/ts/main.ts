import { TermView } from './TermView';
import { VisualStudio } from './VsEventManager';

triggerEvent = function (event: string, data: any) {
    if (VisualStudio.isEventType(event)) {
        VisualStudio.Events.triggerEvent(event, data);
    }
};

if (document.readyState !== 'loading') {
    let termView = new TermView(
        JSON.parse(window.external.GetTheme()),
        window.external.GetFontFamily(),
        window.external.GetFontSize(),
        window.external.GetSolutionDir());
} else {
    document.addEventListener("DOMContentLoaded", function (event) {
        let termView = new TermView(
            JSON.parse(window.external.GetTheme()),
            window.external.GetFontFamily(),
            window.external.GetFontSize(),
            window.external.GetSolutionDir());
    });
}