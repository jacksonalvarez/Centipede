

export const index = 4;
let component_cache;
export const component = async () => component_cache ??= (await import('../entries/pages/Work/_page.svelte.js')).default;
export const imports = ["_app/immutable/nodes/4.4d9DZecl.js","_app/immutable/chunks/scheduler.C9xG8wYf.js","_app/immutable/chunks/index.B3KkDgTn.js","_app/immutable/chunks/index.U-Bm33l8.js"];
export const stylesheets = ["_app/immutable/assets/4.EqHX5bC4.css"];
export const fonts = [];
