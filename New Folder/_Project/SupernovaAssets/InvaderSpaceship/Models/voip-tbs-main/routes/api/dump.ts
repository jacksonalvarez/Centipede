import { Handlers } from "$fresh/server.ts";
import { Database } from "jsr:@db/sqlite";

export const handler: Handlers = {
  GET(_req) {
    const db = new Database("main.sqlite");

    const tables = db.prepare("SELECT * FROM sqlite_master WHERE type='table'").all();

    tables.forEach((table) => {
      if (table.name === "sqlite_sequence") return;
      db.prepare(`DROP TABLE IF EXISTS ${table.name}`).run();
    });

    console.log("Database dumped");

    db.close();

    return new Response(null, { status: 303, headers: { location: "/" } });
  },
};
