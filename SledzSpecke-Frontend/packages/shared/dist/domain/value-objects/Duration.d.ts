export declare class Duration {
    readonly hours: number;
    readonly minutes: number;
    constructor(hours: number, minutes: number);
    static fromMinutes(totalMinutes: number): Duration;
    static fromHoursAndMinutes(hours: number, minutes: number): Duration;
    toTotalMinutes(): number;
    toString(): string;
    toJSON(): {
        hours: number;
        minutes: number;
    };
}
//# sourceMappingURL=Duration.d.ts.map