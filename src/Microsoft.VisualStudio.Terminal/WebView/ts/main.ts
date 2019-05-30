import { TermView } from './TermView';
import { VisualStudio } from './VsEventManager';

triggerEvent = function (event: string, data: any) {
    if (VisualStudio.isEventType(event)) {
        VisualStudio.Events.triggerEvent(event, data);
    }
};

if (document.readyState !== 'loading') {
    let fontSizeFixingElement = document.createElement('div');
    fontSizeFixingElement.style.fontSize = `${window.external.GetFontSize()}pt`;

    let fontSize = parseInt(window.getComputedStyle(fontSizeFixingElement).fontSize);
    let termView = new TermView(
        JSON.parse(window.external.GetTheme()),
        window.external.GetFontFamily(),
        fontSize,
        window.external.GetSolutionDir());
} else {
    document.addEventListener("DOMContentLoaded", function (event) {
        let fontSizeFixingElement = document.createElement('div');
        fontSizeFixingElement.style.fontSize = `${window.external.GetFontSize()}pt`;

        let fontSize = parseInt(window.getComputedStyle(fontSizeFixingElement).fontSize);

        let termView = new TermView(
            JSON.parse(window.external.GetTheme()),
            window.external.GetFontFamily(),
            fontSize,
            window.external.GetSolutionDir());
    });
}