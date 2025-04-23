import React from "react"; // Import React OR come up with a different way to handle file uploads....
import Navbar from "../islands/Navbar.tsx";
import { Handlers, PageProps } from "$fresh/server.ts";
import { getCookies, setCookie } from "$std/http/cookie.ts";
import { CustomDB } from "$libs/database.ts";
import { ICalendarEvent } from "$types/data.ts";
import { authenticateToken } from "$libs/authentication.ts";
import Calendar from "../islands/Calendar.tsx";
import { parseICS } from "$libs/ics.ts"; // ICS parser

interface Data {
  username: string;
  verified: boolean;
  subscribed: boolean;
  events: ICalendarEvent[];
  user_id: number; // Add user_id for ICS parsing
}

export const handler: Handlers = {
  GET(req, ctx) {
    const db = new CustomDB();
    const cookies = getCookies(req.headers);
    const url = new URL(req.url);
    const headers = new Headers();

    if (!cookies.token) {
      db.close();
      headers.set("location", "/login");
      setCookie(headers, {
        name: "auth",
        value: "unauthorized",
        maxAge: 120,
        sameSite: "Lax",
        domain: url.hostname,
      });
      return new Response(null, { status: 303, headers });
    }
    
    const user = authenticateToken(cookies.token, headers, req);

    if (!user) {
      db.close();
      headers.set("location", "/login");
      return new Response(null, { status: 303, headers });
    }

    const events = db.calendarEvents.getCalendarEvents(user.id);

    db.close();
    return ctx.render!({
      username: user.username,
      verified: user.verified,
      subscribed: user.subscribed, 
      events: events,
      user_id: user.id // Pass the user_id
    });
  },
};

export default function AnalyticsPage({ data }: PageProps<Data>) {
  const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      try {
        const events = await parseICS(file, data.user_id);
        console.log("Parsed events:", events);
        // Here you would typically send the events to your backend to store them
      } catch (error) {
        console.error("Error parsing ICS file:", error);
      }
    }
  };

  return (
    <div class="h-screen px-4 py-8 mx-auto bg-dark mt-28">
      <div class="mb-4">
        <label class="block text-white mb-2" htmlFor="ics-upload">Import ICS File:</label>
        <input
          type="file"
          id="ics-upload"
          accept=".ics"
          onChange={handleFileUpload}
          class="text-white"
        />
      </div>
      <Calendar events={data.events}/>
      <div class="max-w-screen-md mx-auto fixed"> 
      </div>
      <div class="max-w-screen-md mx-auto flex flex-col items-center justify-start h-full">
        <Navbar />
        <div class="fixed bottom-2 text-center">
          <p class=" text-white">You are logged in as {data.username}.</p>
          {data.verified ? null : <p class=" text-white">Please check your email to verify your account.</p>}
          {data.subscribed ? null : <p class="text-white">You have not yet subscribed. Please subscribe to retain your features.</p>}
        </div>
      </div>
    </div>
  );
}
