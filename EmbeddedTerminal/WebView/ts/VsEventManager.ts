export namespace VisualStudio {
    const eventStrings = {
        themeChanged: '',
        ptyData: '',
        directoryChanged: ''
    };

    export type EventType = keyof typeof eventStrings;

    export function isEventType(event: string): event is EventType {
        return eventStrings.hasOwnProperty(event);
    }

    export class Events {
        static handlers: { [event: string]: { (data: any): void }[] } = {};
        static on(event: EventType, handler: { (data: any): void }) {
            if (Events.handlers[event as string] === undefined) {
                Events.handlers[event as string] = new Array();
            }
            Events.handlers[event as string].push(handler);
        }

        static off(event: EventType, handler: { (data: any): void }) {
            Events.handlers[event as string] = Events.handlers[event as string].filter(h => h !== handler);
        }

        static triggerEvent(event: 'themeChanged' | 'ptyData' | 'directoryChanged', data: any) {
            Events.handlers[event as string].forEach(handler => handler(data));
        }
    }
}