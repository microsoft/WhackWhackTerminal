import { Terminal, ITheme } from 'xterm';
import { fit } from 'xterm/lib/addons/fit';
import { VisualStudio } from './VsEventManager';

export class TermView {
    term: Terminal;
    resizeTimeout: number | null;
    constructor(fontFamily: string, fontSize: number, solutionDirectory: string) {
        this.term = new Terminal({
            fontFamily: fontFamily + ', courier-new, courier, monospace',
            fontSize: fontSize,
            cols: 80,
            rows: 24
        });

        this.term.open(document.getElementById('content'));
        fit(this.term);
        this.term.on('data', window.external.TermData);
        VisualStudio.Events.on('ptyData', (data) => this.ptyData(data));
        VisualStudio.Events.on('themeChanged', (data) => {
            let theme = JSON.parse(data) as ITheme;
            this.setTheme(theme)
        });
        VisualStudio.Events.on('directoryChanged', (data) => {
            this.closePty();
            this.initPty(data)
        });
        window.addEventListener("resize", () => this.resizeHandler())
    }

    private initPty(cwd: string) {
        window.external.InitPty(this.term.cols, this.term.rows, cwd);
    }

    private closePty() {
        window.external.ClosePty();
    }

    termData(data: string) {
        window.external.TermData(data);
    }

    ptyData(data: string) {
        this.term.write(data);
    }

    setTheme(theme: ITheme) {
        this.term.setOption('theme', theme);
    }

    resizeHandler() {
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
}