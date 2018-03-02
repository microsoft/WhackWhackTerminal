declare var __n: (...params: any[]) => void;
declare module Microsoft.Plugin {
    module Host {
        interface ICore {
            hostDescription(): string;
            postMessage(message: string): void;
            messageReceived: (message: string) => void;
        }
    }
    module Utilities {
        class EventManager {
            private target;
            private listeners;
            public addEventListener(type: string, listener: (e: Event) => void): void;
            public dispatchEvent(type: any, eventArg?: any): boolean;
            public removeEventListener(type: string, listener: (e: Event) => void): void;
            public setTarget(value: any): void;
        }
        interface MarshalledError extends Error {
            innerError: Error;
            source: string;
            helpLink: string;
        }
        function marshalHostError(hostErrorObject: any): MarshalledError;
        function formatString(message: string, optionalParams: any[]): string;
    }
    interface IPromise<TValue> {
        cancel(): void;
        done<TResult>(completed?: (result: TValue) => void, error?: (value: Error) => void, progress?: (value: any) => void): void;
        then<TResult>(completed?: (result: TValue) => IPromise<TResult>, error?: (value: Error) => IPromise<TResult>, progress?: (value: any) => void): IPromise<TResult>;
        then<TResult>(completed?: (result: TValue) => IPromise<TResult>, error?: (value: Error) => TResult, progress?: (value: any) => void): IPromise<TResult>;
        then<TResult>(completed?: (result: TValue) => TResult, error?: (value: Error) => IPromise<TResult>, progress?: (value: any) => void): IPromise<TResult>;
        then<TResult>(completed?: (result: TValue) => TResult, error?: (value: Error) => TResult, progress?: (value: any) => void): IPromise<TResult>;
    }
    module Utilities {
        interface IStringMap<TValue> {
            [key: string]: TValue;
        }
        interface IXHROptions {
            type?: string;
            url: string;
            user?: string;
            password?: string;
            headers?: IStringMap<string>;
            responseType?: string;
            data?: any;
            customRequestInitializer?: (XMLHttpRequest: any) => void;
        }
        function xhr(options: IXHROptions): IPromise<XMLHttpRequest>;
    }
    class Promise<TValue> {
        constructor(init: (completed: {
            (): void;
            (value: TValue): void;
            (value: Promise<TValue>): void;
        }, error?: (value: Error) => void, progress?: (value: any) => void) => void, oncancel?: () => void);
        constructor(init: (completed: () => void, error?: (value: Error) => void, progress?: (value: any) => void) => void, oncancel?: () => void);
        constructor(init: (completed: (value: TValue) => void, error?: (value: Error) => void, progress?: (value: any) => void) => void, oncancel?: () => void);
        public cancel(): void;
        public then<TResult>(completed?: (result: TValue) => Promise<TResult>, error?: (value: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult>;
        public then<TResult>(completed?: (result: TValue) => Promise<TResult>, error?: (value: Error) => TResult, progress?: (value: any) => void): Promise<TResult>;
        public then<TResult>(completed?: (result: TValue) => TResult, error?: (value: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult>;
        public then<TResult>(completed?: (result: TValue) => TResult, error?: (value: Error) => TResult, progress?: (value: any) => void): Promise<TResult>;
        public done(completed?: (value: TValue) => void, error?: (value: Error) => void, progress?: (value: any) => void): void;
        static addEventListener(eventType: string, listener: (args: Event) => void, capture?: boolean): void;
        static any(values: Promise<any>[]): Promise<any>;
        static as<TValue>(value: Promise<TValue>): Promise<TValue>;
        static as<TValue>(value: TValue): Promise<TValue>;
        static as<TValue>(value: any): Promise<TValue>;
        static cancel: Promise<any>;
        static dispatchEvent<EventProps>(eventType: "error", details?: EventProps): boolean;
        static dispatchEvent<EventProps>(eventType: string, details?: EventProps): boolean;
        static is(value: any): boolean;
        static join(values: Promise<void>[]): Promise<void>;
        static join<TValue>(values: Promise<TValue>[]): Promise<TValue[]>;
        static join<TValue>(values: any[]): Promise<TValue>;
        static join(values: any[]): Promise<any[]>;
        static join<TValue>(values: {
            [keys: string]: Promise<TValue>;
        }): Promise<{
            [keys: string]: TValue;
        }>;
        static removeEventListener(eventType: string, listener: (args: Event) => void, capture?: boolean): void;
        static supportedForProcessing: boolean;
        static then<TValue, TResult>(promise: Promise<TValue>, completed?: (value: TValue) => Promise<TResult>, error?: (value: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult>;
        static then<TValue, TResult>(promise: Promise<TValue>, completed?: (value: TValue) => Promise<TResult>, error?: (value: Error) => TResult, progress?: (value: any) => void): Promise<TResult>;
        static then<TValue, TResult>(promise: Promise<TValue>, completed?: (value: TValue) => TResult, error?: (value: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult>;
        static then<TValue, TResult>(promise: Promise<TValue>, completed?: (value: TValue) => TResult, error?: (value: Error) => TResult, progress?: (value: any) => void): Promise<TResult>;
        static then<TValue, TResult>(value: TValue, completed?: (value: TValue) => Promise<TResult>, error?: (value: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult>;
        static then<TValue, TResult>(value: TValue, completed?: (value: TValue) => Promise<TResult>, error?: (value: Error) => TResult, progress?: (value: any) => void): Promise<TResult>;
        static then<TValue, TResult>(value: TValue, completed?: (value: TValue) => TResult, error?: (value: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult>;
        static then<TValue, TResult>(value: TValue, completed?: (value: TValue) => TResult, error?: (value: Error) => TResult, progress?: (value: any) => void): Promise<TResult>;
        static thenEach<TValue, TResult>(promises: Promise<TValue>[], completed?: (value: TValue) => Promise<TResult>, error?: (error: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(promises: Promise<TValue>[], completed?: (value: TValue) => Promise<TResult>, error?: (error: Error) => TResult, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(values: TValue[], completed?: (value: TValue) => Promise<TResult>, error?: (error: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(values: TValue[], completed?: (value: TValue) => Promise<TResult>, error?: (error: Error) => TResult, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(values: any[], completed?: (value: TValue) => Promise<TResult>, error?: (error: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(values: any[], completed?: (value: TValue) => Promise<TResult>, error?: (error: Error) => TResult, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(promises: Promise<TValue>[], completed?: (value: TValue) => TResult, error?: (error: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(promises: Promise<TValue>[], completed?: (value: TValue) => TResult, error?: (error: Error) => TResult, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(values: TValue[], completed?: (value: TValue) => TResult, error?: (error: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(values: TValue[], completed?: (value: TValue) => TResult, error?: (error: Error) => TResult, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(values: any[], completed?: (value: TValue) => TResult, error?: (error: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach<TValue, TResult>(values: any[], completed?: (value: TValue) => TResult, error?: (error: Error) => TResult, progress?: (value: any) => void): Promise<TResult[]>;
        static thenEach(promises: Promise<void>[], completed?: () => Promise<void>, error?: (error: Error) => Promise<void>, progress?: (value: any) => void): Promise<void>;
        static thenEach(promises: Promise<void>[], completed?: () => Promise<void>, error?: (error: Error) => void, progress?: (value: any) => void): Promise<void>;
        static thenEach(promises: Promise<void>[], completed?: () => void, error?: (error: Error) => Promise<void>, progress?: (value: any) => void): Promise<void>;
        static thenEach(promises: Promise<void>[], completed?: () => void, error?: (error: Error) => void, progress?: (value: any) => void): Promise<void>;
        static thenEach<TValue, TResult>(values: {
            [key: string]: Promise<TValue>;
        }, completed?: (value: TValue) => Promise<TResult>, error?: (error: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<{
            [key: string]: TValue;
        }>;
        static thenEach<TValue, TResult>(values: {
            [key: string]: Promise<TValue>;
        }, completed?: (value: TValue) => Promise<TResult>, error?: (error: Error) => TResult, progress?: (value: any) => void): Promise<{
            [key: string]: TValue;
        }>;
        static thenEach<TValue, TResult>(values: {
            [key: string]: TValue;
        }, completed?: (value: TValue) => Promise<TResult>, error?: (error: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<{
            [key: string]: TValue;
        }>;
        static thenEach<TValue, TResult>(values: {
            [key: string]: TValue;
        }, completed?: (value: TValue) => Promise<TResult>, error?: (error: Error) => TResult, progress?: (value: any) => void): Promise<{
            [key: string]: TValue;
        }>;
        static thenEach<TValue, TResult>(values: {
            [key: string]: Promise<TValue>;
        }, completed?: (value: TValue) => TResult, error?: (error: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<{
            [key: string]: TValue;
        }>;
        static thenEach<TValue, TResult>(values: {
            [key: string]: Promise<TValue>;
        }, completed?: (value: TValue) => TResult, error?: (error: Error) => TResult, progress?: (value: any) => void): Promise<{
            [key: string]: TValue;
        }>;
        static thenEach<TValue, TResult>(values: {
            [key: string]: TValue;
        }, completed?: (value: TValue) => TResult, error?: (error: Error) => Promise<TResult>, progress?: (value: any) => void): Promise<{
            [key: string]: TValue;
        }>;
        static thenEach<TValue, TResult>(values: {
            [key: string]: TValue;
        }, completed?: (value: TValue) => TResult, error?: (error: Error) => TResult, progress?: (value: any) => void): Promise<{
            [key: string]: TValue;
        }>;
        static timeout<TValue>(time?: number, promise?: Promise<TValue>): Promise<TValue>;
        static wrap<TValue>(value: TValue): Promise<TValue>;
        static wrapError(value: Error): Promise<Error>;
    }
    enum PortState {
        connected = 0,
        disconnected = 1,
        closed = 2,
    }
    interface Port extends EventTarget {
        state: PortState;
        connect(): void;
        postMessage(message: string): void;
        sendMessage(message: string): Promise<string>;
        close(): void;
    }
    interface MarshalledError extends Error {
        innerError: Error;
        source: string;
        helpLink: string;
    }
    interface PublishedObject {
        _forceConnect(): boolean;
        _postMessage(message: string): void;
        _sendMessage(message: string): Promise<string>;
    }
    function attachToPublishedObject(name: string, objectDefinition: any, messageHandler: (message: string) => void, closeHandler?: (error: Event) => void, createOnFirstUse?: boolean): PublishedObject;
    function _logError(message: string): void;
    function addEventListener(type: string, listener: (e: Event) => void): void;
    function removeEventListener(type: string, listener: (e: Event) => void): void;
    function createPort(name: string): Port;
}
declare module Microsoft.Plugin.Utilities.JSONMarshaler {
    interface MarshaledObject extends PublishedObject, EventTarget {
        _call(name: string, ...args: any[]): Promise<any>;
        _post(name: string, ...args: any[]): void;
    }
    function attachToPublishedObject(name: string, objectDefinition?: any, createOnFirstUse?: boolean): MarshaledObject;
}
declare module Microsoft.Plugin.Diagnostics {
    module Host {
        interface IDiagnostics {
            reportError(message: string, url: string, lineNumber: string, additionalInfo: any, columnNumber?: string): number;
            terminate(): void;
        }
    }
    function onerror(message: any, uri: string, lineNumber: number, columnNumber?: number, error?: Error): boolean;
    function reportError(error: Error, uri?: string, lineNumber?: number, additionalInfo?: any, columnNumber?: number): number;
    function reportError(message: string, uri?: string, lineNumber?: number, additionalInfo?: any, columnNumber?: number): number;
    function terminate(): void;
}
declare module Microsoft.Plugin.Culture {
    interface CultureInfoEvent extends Event {
        language: string;
        direction: string;
        formatRegion: string;
        dateTimeFormat: any;
        numberFormat: any;
    }
    module Host {
        interface ICulture {
            addEventListener(eventType: string, listener: (e: any) => void): void;
        }
    }
    var dir: string;
    var lang: string;
    var formatRegion: string;
    var DateTimeFormat: any;
    var NumberFormat: any;
    function addEventListener(type: string, listener: (e: Event) => void): void;
    function removeEventListener(type: string, listener: (e: Event) => void): void;
}
declare module Microsoft.Plugin {
    module Host {
        interface IOutput {
            log(message: string): void;
        }
    }
    function log(message: any, ...optionalParams: any[]): void;
}
declare module Microsoft.Plugin.Resources {
    enum ResourceType {
        resx = 0,
        resjson = 1,
        embedded = 2,
    }
    interface ResourceAlias {
        alias: string;
        isRelative?: boolean;
        path: string;
        name?: string;
        type?: ResourceType;
        isDefault?: boolean;
    }
    module Host {
        interface ResourceEvent extends Event {
            ResourceMap?: {
                [key: string]: {
                    [key: string]: string;
                };
            };
            DefaultAlias?: string;
            GenericError?: string;
        }
        interface IResources {
            loadResources(resourceAlias: ResourceAlias): Promise<{
                [key: string]: string;
            }>;
            addEventListener(name: string, callback: (e: ResourceEvent) => void): void;
            removeEventListener(name: string, callback: (e: ResourceEvent) => void): void;
        }
    }
    function getString(resourceId: string, ...args: any[]): string;
    function getErrorString(errorId: string): string;
    function loadResourceFile(resourceAlias: ResourceAlias): Promise<void>;
    function addEventListener(name: string, callback: (e: Event) => void): void;
    function removeEventListener(name: string, callback: (e: Event) => void): void;
}
declare module Microsoft.Plugin.Storage {
    module Host {
        interface IStorage {
            closeFile(streamId: string): Promise<void>;
            fileDialog(mode: FileDialogMode, dialogOptions?: FileDialogOptions, fileOptions?: FileOptions): Promise<string>;
            getFileList(path?: string, persistence?: FilePersistence, index?: number, count?: number): Promise<string[]>;
            openFile(path?: string, options?: FileOptions): Promise<string>;
            seek(streamId: string, offset: number, origin: SeekOrigin): Promise<number>;
            read(streamId: string, count?: number, type?: FileType): Promise<any>;
            write(streamId: string, data: any, offset?: number, count?: number, type?: FileType): Promise<void>;
        }
    }
    enum FileAccess {
        read = 1,
        write = 2,
        readWrite = 3,
    }
    enum FileDialogMode {
        open = 0,
        save = 1,
    }
    enum FileMode {
        createNew = 1,
        create = 2,
        open = 3,
        openOrCreate = 4,
        truncate = 5,
        append = 6,
    }
    enum FileShare {
        none = 0,
        read = 1,
        write = 2,
        readWrite = 3,
        delete = 4,
    }
    enum FileType {
        binary = 0,
        text = 1,
    }
    enum FilePersistence {
        permanent = 0,
        temporary = 1,
    }
    enum SeekOrigin {
        begin = 0,
        current = 1,
        end = 2,
    }
    interface FileDialogOptions {
        name?: string;
        extensions?: string[];
        extensionsIndex?: number;
        initialDirectory?: string;
        title?: string;
    }
    interface FileOptions {
        access?: FileAccess;
        encoding?: string;
        mode?: FileMode;
        share?: FileShare;
        persistence?: FilePersistence;
        type?: FileType;
    }
    interface File {
        streamId: string;
        close(): Promise<void>;
        read(count?: number): Promise<any>;
        seek(offset: number, origin: SeekOrigin): Promise<number>;
        write(data: any, offset?: number, count?: number): Promise<void>;
    }
    function getFileList(path?: string, persistence?: FilePersistence, index?: number, count?: number): Promise<string[]>;
    function createFile(path?: string, options?: FileOptions): Promise<File>;
    function openFile(path: string, options?: FileOptions): Promise<File>;
    function openFileDialog(dialogOptions?: FileDialogOptions, fileOptions?: FileOptions): Promise<File>;
    function saveFileDialog(dialogOptions?: FileDialogOptions, fileOptions?: FileOptions): Promise<File>;
}
declare module Microsoft.Plugin.Theme {
    interface ThemeEvent extends Event {
        PluginCss: string;
        themeMap: {
            [key: string]: string;
        };
        isHighContrastTheme?: boolean;
    }
    module Host {
        interface ITheme {
            addEventListener(name: string, callback: (e: ThemeEvent) => void): void;
            fireThemeReady(): void;
            getCssFile(name: string, requirePluginRelativeLocation: boolean): Promise<string>;
        }
    }
    function getValue(key: any): string;
    function processInjectedSvg(targetEle: HTMLElement): void;
    function processInjectedSvg(targetDocFrag: DocumentFragment): void;
    function processInjectedSvg(targetDoc?: HTMLDocument): void;
    function processCSSFileForThemeing(path: string): Promise<string>;
    module _cssHelpers {
        function processCssFileContents(href: string, targetDoc: any, refNode?: any, fireThemeReady?: boolean, isHighContrast?: boolean, additionalAttributes?: Utilities.IStringMap<string>): void;
        function processImages(targetDoc: Document): void;
    }
    function addEventListener(type: string, listener: (e: Event) => void): void;
    function removeEventListener(type: string, listener: (e: Event) => void): void;
}
declare module Microsoft.Plugin.VS.Commands {
    interface CommandState {
        name: string;
        enabled?: boolean;
        visible?: boolean;
    }
    interface NameSet {
        indexOf(name: string): number;
    }
    interface CommandsInitializedEvent extends Event {
        menuAliases: NameSet;
        commandAliases: NameSet;
    }
    interface CommandInvokeEvent extends Event {
        CommandName: string;
    }
    module Host {
        interface ICommands {
            showContextMenu(menuName: string, xPosition: number, yPosition: number): Promise<void>;
            setCommandsStates(states: CommandState[]): Promise<void>;
            addEventListener(eventType: string, listener: (e: any) => void): void;
        }
    }
    class ContextMenuBinding {
        private name;
        constructor(name: string);
        public show(xPosition: number, yPosition: number): Promise<void>;
    }
    interface CommandBindingState {
        enabled?: boolean;
        visible?: boolean;
    }
    class CommandBinding {
        public _name: string;
        public _onexecute: () => void;
        public _enabled: boolean;
        public _visible: boolean;
        constructor(name: string, onexecute: () => void, enabled: boolean, visible: boolean);
        public setState(state: CommandBindingState): void;
    }
    function bindContextMenu(name: string): ContextMenuBinding;
    interface CommandBindingDefinition {
        name: string;
        onexecute: () => void;
        enabled?: boolean;
        visible?: boolean;
    }
    function bindCommand(command: CommandBindingDefinition): CommandBinding;
    interface CommandStateRequest {
        command: CommandBinding;
        enabled?: boolean;
        visible?: boolean;
    }
    function setStates(...states: CommandStateRequest[]): void;
}
declare module Microsoft.Plugin.VS.Internal.CodeMarkers {
    module Host {
        interface ICodeMarkers {
            fireCodeMarker(marker: number): void;
        }
    }
    function fire(marker: number): void;
}
declare module Microsoft.Plugin.Host {
    interface Version {
        major: number;
        minor: number;
        build: number;
        revision: number;
    }
    interface IHost {
        version: Version;
        showDocument(documentPath: string, line: number, col: number): Promise<void>;
        getDocumentLocation(documentPath: string): Promise<string>;
        supportsAllowSetForeground(): boolean;
        allowSetForeground(processId: number): boolean;
    }
    var version: Version;
    function showDocument(documentPath: string, line: number, col: number): Promise<void>;
    function getDocumentLocation(documentPath: string): Promise<string>;
    function supportsAllowSetForeground(): boolean;
    function allowSetForeground(processId: number): boolean;
}
declare module Microsoft.Plugin.VS.Keyboard {
    function setClipboardState(state: boolean): void;
    function setZoomState(state: boolean): void;
}
declare module Microsoft.Plugin.Tooltip {
    interface Point {
        X: number;
        Y: number;
    }
    interface Size {
        Width: number;
        Height: number;
    }
    interface PopupDisplayParameters {
        content: string;
        clientCoordinates: Point;
        contentSize: Size;
        useCachedDocument?: boolean;
        ensureNotUnderMouseCursor?: boolean;
        placementTargetIsMouseRect?: boolean;
    }
    module Host {
        interface ITooltip {
            getDblClickTime(): number;
            canCreatePopup(): boolean;
            getScreenSizeForXY(screenX: number, screenY: number): Size;
            hostContentInPopup(displayParameters: PopupDisplayParameters): void;
            dismissPopup(): void;
        }
    }
    var defaultTooltipContentToHTML: boolean;
    function invalidatePopupTooltipDocumentCache(): void;
    function initializeElementTooltip(element: Element): void;
    interface TooltipConfiguration {
        content?: string;
        resource?: string;
        delay?: number;
        duration?: number;
        x?: number;
        y?: number;
    }
    function show(config: TooltipConfiguration): void;
    function dismiss(reset?: boolean): void;
}
declare module Microsoft.Plugin.Settings {
    module Host {
        interface ISettings {
            get(collection?: string, requestedProperties?: string[]): Promise<any>;
            set(collection?: string, toSet?: any): any;
        }
    }
    function get(collection?: string, requestedProperties?: string[]): Promise<any>;
    function set(collection?: string, toSet?: any): any;
}
declare module Microsoft.Plugin.VS.ActivityLog {
    module Host {
        interface IActivityLog {
            logEntry(entryType: number, message: string): any;
        }
    }
    function info(message: string, ...args: string[]): void;
    function warn(message: string, ...args: string[]): void;
    function error(message: string, ...args: string[]): void;
}
declare module Microsoft.Plugin.ContextMenu {
    interface ContextMenuClickEvent extends Event {
        Id: string;
    }
    interface ContextMenuDismissEvent extends Event {
        Id: string;
    }
    interface ContextMenuInitializeEvent extends Event {
        Id: string;
        AriaLabel: string;
        ContextMenus: string;
    }
    module Host {
        interface IContextMenu {
            adjustShowCoordinates(coordinates: Point): Point;
            callback(id: string): Promise<void>;
            canCreatePopup(hasSubmenu?: boolean): boolean;
            disableZoom(): void;
            dismiss(): Promise<void>;
            dismissCurrent(ignoreDismissForRoot: boolean): Promise<void>;
            dismissSubmenus(currentCoordinates: Point): Promise<void>;
            fireContentReady(): Promise<void>;
            show(menuId: string, ariaLabel: string, contextMenus: HTMLElement, positionInfo: _positionHelpers.PositionInfo): Promise<void>;
            addEventListener(name: string, callback: (e: any) => void): any;
        }
    }
    enum MenuItemType {
        checkbox = 0,
        command = 1,
        radio = 2,
        separator = 3,
    }
    interface ContextMenu {
        attach(element: HTMLElement): void;
        detach(element: HTMLElement): void;
        dismiss(): void;
        dispose(): void;
        show(xPosition: number, yPosition: number, widthOffset?: number, targetId?: string): void;
        addEventListener(type: string, listener: (e: Event) => void): any;
        removeEventListener(type: string, listener: (e: Event) => void): any;
    }
    interface ContextMenuItem {
        id?: string;
        callback?: (menuId?: string, menuItem?: ContextMenuItem, targetId?: string) => void;
        label?: string;
        type?: MenuItemType;
        iconEnabled?: string;
        iconDisabled?: string;
        accessKey?: string;
        hidden?: () => boolean;
        disabled?: () => boolean;
        checked?: () => boolean;
        cssClass?: string;
        submenu?: ContextMenuItem[];
    }
    var isShowing: boolean;
    function dismissAll(): Promise<void>;
    function create(menuItems: ContextMenuItem[], id?: string, ariaLabel?: string, cssClass?: string, callback?: (menuId?: string, menuItem?: ContextMenuItem, targetId?: string) => void): ContextMenu;
    function canCreatePopup(): boolean;
    interface Point {
        X: number;
        Y: number;
    }
    module _positionHelpers {
        interface PositionInfo {
            clientCoordinates: Point;
            width: number;
            height: number;
            viewPortWidth: number;
            viewPortHeight: number;
            scrollOffsetLeft: number;
            scrollOffsetTop: number;
            elementOffsetTop: number;
            widthOffset: number;
        }
        function show(element: HTMLElement, ariaLabel: string, xPosition: number, yPosition: number, elementOffsetTop?: number, widthOffset?: number, displayType?: string, tryAdjustCoordinates?: (positionInfo: PositionInfo) => PositionInfo, showOutsideOfAirspace?: (id: string, ariaLabel: string, contextMenus: HTMLElement, positionInfo: PositionInfo) => void): void;
    }
}
declare module Microsoft.Plugin.VS.Utilities {
    function createExternalObject(fileAlias: string, clsid: string): any;
}
declare module Microsoft.Plugin.PerfAnalytics {
    module Host {
        interface IPerfAnalytics {
            raiseEvent: (id: number, data: any[]) => void;
        }
    }
    function raiseEvent(id: number, data: any[]): void;
    function raiseEventWithKey(id: number, key: any, data: any[]): void;
}
declare module Microsoft.Plugin.SQMAnalytics {
    module Host {
        interface ISQMAnalytics {
            addDataToStream(streamId: number, data: any[]): void;
            logBooleanData(dataPointId: number, data: boolean): void;
            logNumericData(dataPointId: number, data: number): void;
            logStringData(dataPointId: number, data: string): void;
        }
    }
    function addDataToStream(dataPointId: number, data: any[]): void;
    function logBooleanData(dataPointId: number, data: boolean): void;
    function logNumericData(dataPointId: number, data: number): void;
    function logStringData(dataPointId: number, data: string): void;
}
