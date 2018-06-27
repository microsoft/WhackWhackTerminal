import { ITheme, Terminal } from 'xterm';
import { apply as applyFit } from 'xterm/lib/addons/fit';
import { VisualStudio } from './VsEventManager';
import { registerLocalLinkHandler } from './TerminalLinkMatcher';

// This import and declaraion are necessary due to a strange issue with the way the xterm.js dist file is bundled.
// The definition file would make it seem that the xterm import contains the Terminal object, but at runtime it
// actually is the terminal object.
import * as xterm from 'xterm';
const TerminalConstructor = xterm as any as (typeof Terminal);

export class TermView {
    private autoFit: boolean;
    private term: Terminal;
    private termFit: TerminalFit;
    private resizeTimeout: number | null;
    private solutionDirectory: string;

    private borderStyle: HTMLElement;
    private backgroundColor: string;
    private borderColor: string;

    constructor(theme: ITheme, fontFamily: string, fontSize: number, solutionDirectory: string) {
        this.solutionDirectory = solutionDirectory;
        this.autoFit = true;
        applyFit(TerminalConstructor);

        this.term = new TerminalConstructor({
            theme: theme,
            fontFamily: fontFamily + ', courier-new, courier, monospace',
            fontSize: fontSize,
            cursorBlink: true,
            cols: 80,
            rows: 24
        });
        this.termFit = <any>this.term;

        this.backgroundColor = theme.background;
        this.borderColor = (<any>theme).border || theme.foreground;

        const content = document.getElementById('content');

        this.borderStyle = document.createElement('style');
        this.updateBorder();
        content.appendChild(this.borderStyle);

        this.term.open(content);
        this.termFit.fit();
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
        VisualStudio.Events.on('resize', data => {
            const size: Geometry = JSON.parse(data);
            if (size) {
                const borderChanging = this.autoFit;
                this.autoFit = false;
                if (borderChanging) {
                    this.updateBorder();
                }

                this.resizeTerm(size);
            }
        });
        window.addEventListener("resize", () => this.resizeHandler())
        this.registerKeyboardHandlers();
        this.initPty(solutionDirectory);
        registerLocalLinkHandler(this.term);
    }

    private resizeTerm(size: Geometry) {
        if (this.term.cols !== size.cols || this.term.rows !== size.rows) {
            (<any>this.term).renderer.clear();
            this.term.resize(size.cols, size.rows);
        }
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
        this.backgroundColor = theme.background;
        this.borderColor = (<any>theme).border || theme.foreground;
        this.updateBorder();
    }

    private resizeHandler() {
        let actualHandler = () => {
            const size: Geometry = this.termFit.proposeGeometry();
            if (size) {
                if (this.autoFit) {
                    this.resizeTerm(size);
                }

                window.external.ResizePty(size.cols, size.rows);
            }
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

    private updateBorder() {
        this.borderStyle.innerHTML = this.autoFit ? '' :
            '.xterm-screen { ' +
                'border-style: dotted; ' +
                'border-width: 1px; ' +
                `border-top-color: ${this.backgroundColor}; ` +
                `border-bottom-color: ${this.borderColor}; ` +
                `border-left-color: ${this.backgroundColor}; ` +
                `border-right-color: ${this.borderColor}` +
            '}';
    }
}