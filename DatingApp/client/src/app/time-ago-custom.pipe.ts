import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeAgoCustom',
  standalone: true,
  pure: false,
})
export class TimeAgoCustomPipe implements PipeTransform {
  transform(value: Date | string): string {
    if (!value) return '';

    const date = typeof value === 'string' ? new Date(value) : value;
    const now = new Date();
    const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);
    if (seconds < 29) return 'Just Now';

    const intervals: { [key: string]: number } = {
      year: 31536000,
      month: 2592000,
      week: 604800,
      day: 86400,
      hour: 3600,
      minute: 60,
      second: 1,
    };

    for (const interval in intervals) {
      const counter = Math.floor(seconds / intervals[interval]);
      if (counter > 0) {
        return `${counter} ${interval}${counter > 1 ? 's' : ''} ago`;
      }
    }
    return value.toString();
  }
}
