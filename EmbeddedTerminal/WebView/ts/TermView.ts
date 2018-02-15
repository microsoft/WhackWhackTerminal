import { ITheme, Terminal } from 'xterm';
import { fit } from 'xterm/lib/addons/fit';
import { VisualStudio } from './VsEventManager';
import { registerLocalLinkHandler } from './TerminalLinkMatcher';

// This import and declaraion are necessary due to a strange issue with the way the xterm.js dist file is bundled.
// The definition file would make it seem that the xterm import contains the Terminal object, but at runtime it
// actually is the terminal object.
import * as xterm from 'xterm';
const TerminalConstructor = xterm as any as (typeof Terminal);

export class TermView {
    private term: Terminal;
    private resizeTimeout: number | null;
    private solutionDirectory: string;
    constructor(theme: ITheme, fontFamily: string, fontSize: number, solutionDirectory: string) {
        this.solutionDirectory = solutionDirectory;
        this.term = new TerminalConstructor({
            theme: theme,
            fontFamily: fontFamily + ', courier-new, courier, monospace',
            fontSize: fontSize,
            cols: 80,
            rows: 24
        });

        this.term.open(document.getElementById('content'));
        fit(this.term);
        this.term.on('data', (data) => this.termData(data));
        VisualStudio.Events.on('ptyData', (data) => this.ptyData(data));
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
        VisualStudio.Events.on('ptyExited', (data) => {
            this.term.write('\x1b[H\x1b[2J');
            this.term.writeln('the terminal exited, initializing a new instance of the terminal');
            this.initPty(this.solutionDirectory)
        });
        VisualStudio.Events.on('focus', () => {
            this.term.focus();
        });
        window.addEventListener("resize", () => this.resizeHandler())
        this.registerKeyboardHandlers();
        this.initPty(solutionDirectory);
        registerLocalLinkHandler(this.term);
    }

    private initPty(cwd: string) {
        window.external.InitPty(this.term.cols, this.term.rows, cwd);
    }

    private closePty() {
        window.external.ClosePty();
    }

    private termData(data: string) {
        window.external.TermData(data);
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
            window.external.ResizePty(this.term.cols, this.term.rows);
        };

        let timeoutCallback = () => {
            this.resizeTimeout = null;
            actualHandler();
        }

        if (!this.resizeTimeout) {
            this.resizeTimeout = setTimeout(() => timeoutCallback(), 66);
        }
    }

    private registerKeyboardHandlers() {
        this.term.attachCustomKeyEventHandler((event) => {
            // capture Ctrl+C
            if (event.ctrlKey && event.keyCode === 67 && this.term.hasSelection()) {
                window.external.CopyStringToClipboard(this.term.getSelection());
                this.term.clearSelection();
                return false;
            // capture Ctrl+V
            } else if (event.ctrlKey && event.keyCode === 86) {
                return false;
            }
        });

        window.addEventListener('contextmenu', (event) => {
            if (this.term.hasSelection()) {
                window.external.CopyStringToClipboard(this.term.getSelection());
                this.term.clearSelection();
            } else {
                let content = window.external.GetClipboard();
                this.termData(content);
            }
        });
    }
}