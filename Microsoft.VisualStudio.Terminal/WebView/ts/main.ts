import { TermView } from './TermView';
import { VisualStudio } from './VsEventManager';
import { ITheme } from 'xterm';
import { Marshal } from './marshals';

triggerEvent = function (event: string, data: any) {
    if (VisualStudio.isEventType(event)) {
        VisualStudio.Events.triggerEvent(event, data);
    }
};

Promise = Microsoft.Plugin.Promise;

class HostMarshal implements Marshal {
    private marshal: Microsoft.Plugin.Utilities.JSONMarshaler.MarshaledObject;
    constructor(marshal: Microsoft.Plugin.Utilities.JSONMarshaler.MarshaledObject) {
        this.marshal = marshal;
    }
    async getTheme(): Microsoft.Plugin.Promise<ITheme> {
        var x = await this.marshal._call('getTheme');
        return x;
    }

    getFontFamily(): Microsoft.Plugin.Promise<string> {
        return this.marshal._call('getFontFamily');
    }
    getFontSize(): Microsoft.Plugin.Promise<number> {
        return this.marshal._call('getFontSize');
    }
    getSolutionDir(): Microsoft.Plugin.Promise<string> {
        return this.marshal._call('getSolutionDir');
    }
    initPty(cols: number, rows: number, dir: string): Microsoft.Plugin.Promise<void> {
        return this.marshal._call('initPty', cols, rows, dir);
    }
    closePty(): Microsoft.Plugin.Promise<void> {
        return this.marshal._call('closePty');
    }
    copyStringToClipboard(stringToCopy: string): Microsoft.Plugin.Promise<void> {
        return this.marshal._call('copyStringToClipboard', stringToCopy);
    }
    getClipboard(): Microsoft.Plugin.Promise<string> {
        return this.marshal._call('getClipboard');
    }
    termData(data: string): Microsoft.Plugin.Promise<void> {
        return this.marshal._call('termData', data);
    }
    resizePty(cols: number, rows: number): Microsoft.Plugin.Promise<void> {
        return this.marshal._call('resizePty', cols, rows);
    }
    getLinkRegex(): Microsoft.Plugin.Promise<string> {
        return this.marshal._call('getLinkRegex');
    }
    handleLocalLink(uri: string): Microsoft.Plugin.Promise<void> {
        return this.marshal._call('handleLocalLink', uri);
    }
    validateLocalLink(link: string): Microsoft.Plugin.Promise<boolean> {
        return this.marshal._call('validateLocalLink', link);
    }
}

Microsoft.Plugin.addEventListener('pluginready', async () => {
    let hostProxy = Microsoft.Plugin.Utilities.JSONMarshaler.attachToPublishedObject('hostmarshal', {});
    let marshal = new HostMarshal(hostProxy);
    let theme = await marshal.getTheme();
    let fontFamily = await marshal.getFontFamily();
    let fontSize = await marshal.getFontSize();
    let solutionDir = await marshal.getSolutionDir();
    new TermView(marshal, fontFamily, fontSize, solutionDir, theme);
});