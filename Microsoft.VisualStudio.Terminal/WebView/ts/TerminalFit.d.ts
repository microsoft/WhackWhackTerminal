// This interface is applied to Terminal by fit's apply()

interface TerminalFit {
    fit(): void;
    proposeGeometry(): Geometry;
}

interface Geometry {
    cols: number;
    rows: number;
}
