

export const index = 1;
let component_cache;
export const component = async () => component_cache ??= (await import('../entries/fallbacks/error.svelte.js')).default;
export const imports = ["_app/immutable/nodes/1.BCKjqEIW.js","_app/immutable/chunks/scheduler.C9xG8wYf.js","_app/immutable/chunks/index.B3KkDgTn.js","_app/immutable/chunks/entry.B45YwSZQ.js","_app/immutable/chunks/index.U-Bm33l8.js"];
export const stylesheets = [];
export const fonts = [];
