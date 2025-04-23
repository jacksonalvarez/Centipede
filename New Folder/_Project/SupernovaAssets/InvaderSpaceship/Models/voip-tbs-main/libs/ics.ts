import { ICalendarEvent } from '$types/data.ts';

export function parseICS(file: File, userId: number): Promise<Map<number, ICalendarEvent>> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();

    reader.onload = (e) => {
      const icsData = e.target?.result as string;
      const events = parseICSData(icsData, userId);
      resolve(events);
    };

    reader.onerror = (error) => {
      reject("Error reading file: " + error);
    };

    reader.readAsText(file);
  });
}

function parseICSData(icsData: string, userId: number): Map<number, ICalendarEvent> {
  const events: Map<number, ICalendarEvent> = new Map();

  const eventRegex = /BEGIN:VEVENT([\s\S]*?)END:VEVENT/g;
  let match: RegExpExecArray | null;

  while ((match = eventRegex.exec(icsData)) !== null) {
    const eventData = match[1];
    
    // Extract the event details using regular expressions
    const dateRegex = /DTSTART;TZID=[^:]+:(\d{8}T\d{4})/;
    const endDateRegex = /DTEND;TZID=[^:]+:(\d{8}T\d{4})/;
    const summaryRegex = /SUMMARY:([^\n]+)/;
    const rruleRegex = /RRULE:FREQ=([A-Za-z]+);UNTIL=(\d{8}T\d{4}Z)/;
    const attendingRegex = /ATTENDEE;RSVP=TRUE;CN=([^\n]+)/g;
    
    const startMatch = dateRegex.exec(eventData);
    const endMatch = endDateRegex.exec(eventData);
    const summaryMatch = summaryRegex.exec(eventData);
    const rruleMatch = rruleRegex.exec(eventData);

    const event: ICalendarEvent = {
      id: Math.floor(Math.random() * 1000000), // Generate a unique id
      user_id: userId, // Use the authenticated user's id
      date: startMatch ? startMatch[1].substring(0, 8) : '',
      time: startMatch ? startMatch[1].substring(9, 11) + ':' + startMatch[1].substring(11, 13) : '',
      header: summaryMatch ? summaryMatch[1] : 'No title',
      description: summaryMatch ? summaryMatch[1] : 'No description',
      color: '#000000', // Default color or extract from the data
      isRR: rruleMatch ? 1 : 0, // Set isRR based on the presence of RRULE
      rrule: rruleMatch ? { freq: rruleMatch[1], until: rruleMatch[2] } : null,
      attendees: [], // Initialize the attendees array
    };

    // Parse attendees if present
    let attendeeMatch;
    while ((attendeeMatch = attendingRegex.exec(eventData)) !== null) {
      event.attendees!.push(attendeeMatch[1]);
    }

    // Store event in the map with event.id as the key
    events.set(event.id, event);
  }

  return events;
}