export class Duration {
  constructor(
    public readonly hours: number,
    public readonly minutes: number
  ) {
    if (hours < 0 || minutes < 0) {
      throw new Error('Duration values cannot be negative');
    }
    // Note: We allow minutes >= 60 for flexible input (per business requirements)
  }

  static fromMinutes(totalMinutes: number): Duration {
    const hours = Math.floor(totalMinutes / 60);
    const minutes = totalMinutes % 60;
    return new Duration(hours, minutes);
  }

  static fromHoursAndMinutes(hours: number, minutes: number): Duration {
    return new Duration(hours, minutes);
  }

  toTotalMinutes(): number {
    return this.hours * 60 + this.minutes;
  }

  toString(): string {
    return `${this.hours}h ${this.minutes}m`;
  }

  toJSON() {
    return {
      hours: this.hours,
      minutes: this.minutes
    };
  }
}