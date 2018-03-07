import { ITheme } from 'xterm';

export interface Marshal {
    getTheme(): Microsoft.Plugin.Promise<ITheme>;
    getFontFamily(): Microsoft.Plugin.Promise<string>;
    getFontSize(): Microsoft.Plugin.Promise<number>;
    getSolutionDir(): Microsoft.Plugin.Promise<string>;
    initPty(cols: number, rows: number, dir: string): Microsoft.Plugin.Promise<void>;
    closePty(): Microsoft.Plugin.Promise<void>;
    copyStringToClipboard(stringToCopy: string): Microsoft.Plugin.Promise<void>;
    getClipboard(): Microsoft.Plugin.Promise<string>;
    termData(data: string): Microsoft.Plugin.Promise<void>;
    resizePty(cols: number, rows: number): Microsoft.Plugin.Promise<void>;
    getLinkRegex(): Microsoft.Plugin.Promise<string>;
    handleLocalLink(uri: string): Microsoft.Plugin.Promise<void>;
    validateLocalLink(link: string): Microsoft.Plugin.Promise<boolean>;
    sendFocus(): Microsoft.Plugin.Promise<void>;
}

export interface VSEvents {

}

export interface PtyEvents {

}