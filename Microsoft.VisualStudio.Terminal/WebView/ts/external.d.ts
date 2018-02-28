interface External {
    GetTheme(): string;
    GetFontFamily(): string;
    GetFontSize(): number;
    GetSolutionDir(): string;
    InitPty(cols: number, rows: number, dir: string): void;
    ClosePty(): void;
    CopyStringToClipboard(stringToCopy: string): void;
    GetClipboard(): string;
    TermData(data: string): void;
    ResizePty(cols: number, rows: number): void;
    GetLinkRegex(): string;
    HandleLocalLink(uri: string): void;
    ValidateLocalLink(link: string): boolean;
}

declare var triggerEvent: {(event: string, data: any): void };