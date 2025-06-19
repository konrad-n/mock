export class Duration {
    constructor(hours, minutes) {
        this.hours = hours;
        this.minutes = minutes;
        if (hours < 0 || minutes < 0) {
            throw new Error('Duration values cannot be negative');
        }
        // Note: We allow minutes >= 60 for flexible input (per business requirements)
    }
    static fromMinutes(totalMinutes) {
        const hours = Math.floor(totalMinutes / 60);
        const minutes = totalMinutes % 60;
        return new Duration(hours, minutes);
    }
    static fromHoursAndMinutes(hours, minutes) {
        return new Duration(hours, minutes);
    }
    toTotalMinutes() {
        return this.hours * 60 + this.minutes;
    }
    toString() {
        return `${this.hours}h ${this.minutes}m`;
    }
    toJSON() {
        return {
            hours: this.hours,
            minutes: this.minutes
        };
    }
}
//# sourceMappingURL=Duration.js.map