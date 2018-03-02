import { ITheme, Terminal } from 'xterm';
import { fit } from 'xterm/lib/addons/fit';
import { VisualStudio } from './VsEventManager';
import { registerLocalLinkHandler } from './TerminalLinkMatcher';
import { Marshal } from './marshals';

// This import and declaraion are necessary due to a strange issue with the way the xterm.js dist file is bundled.
// The definition file would make it seem that the xterm import contains the Terminal object, but at runtime it
// actually is the terminal object.
import * as xterm from 'xterm';
const TerminalConstructor = xterm as any as (typeof Terminal);

export class TermView {
    private term: Terminal;
    private resizeTimeout: number | null;
    private solutionDirectory: string;
    private marshal: Marshal;

    constructor(marshal: Marshal, fontFamily: string, fontSize: number, solutionDirectory: string, theme: ITheme) {
        this.solutionDirectory = solutionDirectory;
        this.term = new TerminalConstructor({
            theme: theme,
            fontFamily: fontFamily + ', courier-new, courier, monospace',
            fontSize: fontSize,
            cursorBlink: true,
            cols: 80,
            rows: 24
        });
        this.marshal = marshal;

        this.term.open(document.getElementById('content'));
        fit(this.term);
        this.term.on('data', (data) => this.termData(data));

        let events = Microsoft.Plugin.Utilities.JSONMarshaler.attachToPublishedObject('terminalEvents', {});
        events.addEventListener('ptyData', (data) => {
            this.ptyData(data.Value);
        });

        events.addEventListener('ptyExited', (data) => {
            this.term.write('\x1b[H\x1b[2J');
            this.term.writeln('the terminal exited, initializing a new instance of the terminal');
            this.initPty(this.solutionDirectory);
        });

        VisualStudio.Events.on('themeChanged', (data) => {
            let theme = JSON.parse(data) as ITheme;
            this.setTheme(theme)
        });
        VisualStudio.Events.on('directoryChanged', (data) => {
            this.solutionDirectory = data;

            this.term.write('\x1b[H\x1b[2J');
            this.term.writeln('solution changed, restarting terminal in new directory');
            this.closePty();
            this.initPty(data)
        });
        VisualStudio.Events.on('focus', () => {
            this.term.focus();
        });
        window.addEventListener("resize", () => this.resizeHandler())
        this.registerKeyboardHandlers();
        this.initPty(this.solutionDirectory);
        registerLocalLinkHandler(marshal, this.term);
    }

    private initPty(cwd: string) {
        this.marshal.initPty(this.term.cols, this.term.rows, cwd);
    }

    private closePty() {
        this.marshal.closePty();
    }

    private termData(data: string) {
        this.marshal.termData(data);
    }

    private ptyData(data: string) {
        this.term.write(data);
    }

    private setTheme(theme: ITheme) {
        this.term.setOption('theme', theme);
    }

    private resizeHandler() {
        let actualHandler = () => {
            fit(this.term);
            this.marshal.resizePty(this.term.cols, this.term.rows);
        };

        let timeoutCallback = () => {
            this.resizeTimeout = null;
            actualHandler();
        }

        if (!this.resizeTimeout) {
            this.resizeTimeout = setTimeout(() => timeoutCallback(), 66) as any as number;
        }
    }

    private registerKeyboardHandlers() {
        this.term.attachCustomKeyEventHandler((event) => {
            // capture Ctrl+C
            if (event.ctrlKey && event.keyCode === 67 && this.term.hasSelection()) {
                this.marshal.copyStringToClipboard(this.term.getSelection());
                this.term.clearSelection();
                return false;
            // capture Ctrl+V
            } else if (event.ctrlKey && event.keyCode === 86) {
                return false;
            }
        });

        window.addEventListener('contextmenu', async (event) => {
            if (this.term.hasSelection()) {
                await this.marshal.copyStringToClipboard(this.term.getSelection());
                this.term.clearSelection();
            } else {
                let content = await this.marshal.getClipboard();
                this.termData(content);
            }
        });
    }
}