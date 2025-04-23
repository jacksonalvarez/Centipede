import { CustomDB } from "./database.ts";
import { IUser } from "$types/data.ts";
import { setCookie } from "$std/http/cookie.ts";

export const authenticateToken = (token: string, headers: Headers, req: Request) => {
  const db = new CustomDB();
  const url = new URL(req.url);

  const session = db.sessions.getSession(token);
  if (!session) {
    db.close();
    setCookie(headers, {
      name: "auth",
      value: "unauthorized",
      maxAge: 120,
      sameSite: "Lax",
      domain: url.hostname,
    });
    return null;
  }

  if (
    session.unix_timestamp + (60 * 60 * 24 * 7) <
      Math.floor(Date.now() / 1000)
  ) {
    db.sessions.deleteSession(token);
    db.close();
    setCookie(headers, {
      name: "auth",
      value: "expired",
      maxAge: 120,
      sameSite: "Lax",
      domain: url.hostname,
    });
    return null;
  }

  const user = db.users.getUser(session.user_id) as Omit<IUser, "password">;

  db.close();

  return user;
};
