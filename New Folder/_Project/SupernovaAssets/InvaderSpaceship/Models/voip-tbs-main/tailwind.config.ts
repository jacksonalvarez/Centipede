import { type Config } from "tailwindcss";

export default {
  content: [
    "{routes,islands,components}/**/*.{ts,tsx,js,jsx}",
  ],
  theme: {
    extend: {
      colors: {
        dark: "#313338",
        darker: "#1e1f22",
        darkest: "#0c0d0f",
      },
    },
  },
} satisfies Config;
