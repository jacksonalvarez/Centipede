import { Handlers } from "$fresh/server.ts";
import { getCookies } from "$std/http/cookie.ts";
import { CustomDB } from "$libs/database.ts";
import { ICalendarEvent } from "$types/data.ts";

export const handler: Handlers = {
  async POST(req) {
    const db = new CustomDB();
    const cookies = getCookies(req.headers);
    const form = await req.formData();

    if (!cookies.token && !form.has("key")) {
      db.close();
      return new Response(null, { status: 401 });
    }

    const session = db.sessions.getSession(cookies.token);
    const api_key = db.twilioKeys.getTwilioKey(form.get("key") as string);
    if (!session && !api_key) {
      db.close();
      return new Response(null, { status: 401 });
    }

    const event = {
      user_id: session ? session.user_id : api_key.user_id,
      date: form.get("date") as string,
      time: form.get("time") as string,
      header: form.get("header") as string,
      description: form.get("description") as string,
      color: form.get("color") as string,
    } as ICalendarEvent;

    db.calendarEvents.insertCalendarEvent(event);

    console.log("Event added:");
    console.table(event);

    db.close();

    return new Response(null, { status: 200 });
  },

  GET(req) {
    const db = new CustomDB();
    const cookies = getCookies(req.headers);
    const url = new URL(req.url);
    const key = url.searchParams.get("key");

    if (!cookies.token && !key) {
      db.close();
      return new Response(null, { status: 401 });
    }

    const session = db.sessions.getSession(cookies.token);
    const api_key = db.twilioKeys.getTwilioKey(key as string);
    if (!session && !api_key) {
      db.close();
      return new Response(null, { status: 401 });
    }

    const events = db.calendarEvents.getCalendarEvents(session ? session.user_id : api_key.user_id);

    console.log(`Events retrieved for ${{uid: session ? session.user_id : api_key.user_id}}:`);

    db.close();

    return new Response(JSON.stringify(events), {
      status: 200,
      headers: {
        "Content-Type": "application/json",
      },
    });
  },

  async PUT(req) {
    const db = new CustomDB();
    const cookies = getCookies(req.headers);
    const form = await req.formData();

    if (!cookies.token && !form.has("key")) {
      db.close();
      return new Response(null, { status: 401 });
    }

    const session = db.sessions.getSession(cookies.token);
    const api_key = db.twilioKeys.getTwilioKey(form.get("key") as string);
    if (!session && !api_key) {
      db.close();
      return new Response(null, { status: 401 });
    }

    const event = {
      user_id: session ? session.user_id : api_key.user_id,
      date: form.get("date") as string,
      time: form.get("time") as string,
      header: form.get("header") as string,
      description: form.get("description") as string,
      color: form.get("color") as string
    } as ICalendarEvent;

    db.calendarEvents.updateCalendarEvent(event);

    console.log("Event updated:");
    console.table(event);

    db.close();

    return new Response(null, { status: 200 });
  },

  async DELETE(req) {
    const db = new CustomDB();
    const cookies = getCookies(req.headers);
    const form = await req.formData();

    if (!cookies.token && !form.has("key")) {
      db.close();
      return new Response(null, { status: 401 });
    }

    const session = db.sessions.getSession(cookies.token);
    const api_key = db.twilioKeys.getTwilioKey(form.get("key") as string);
    if (!session && !api_key) {
      db.close();
      return new Response(null, { status: 401 });
    }

    const event = {
      user_id: session ? session.user_id : api_key.user_id,
      date: form.get("date") as string,
      time: form.get("time") as string,
    } as ICalendarEvent;

    db.calendarEvents.deleteCalendarEvent(event);

    console.log("Event deleted:");
    console.table(event);

    return new Response(null, { status: 200 });
  }
};