export const manifest = (() => {
function __memo(fn) {
	let value;
	return () => value ??= (value = fn());
}

return {
	appDir: "_app",
	appPath: "_app",
	assets: new Set(["174857.png","25231.png","C0019T01.jpg","favicon.png","Resume-JacksonAlvarez-Developer.pdf"]),
	mimeTypes: {".png":"image/png",".jpg":"image/jpeg",".pdf":"application/pdf"},
	_: {
		client: {"start":"_app/immutable/entry/start.A7H2UH-F.js","app":"_app/immutable/entry/app.BrF4UfsO.js","imports":["_app/immutable/entry/start.A7H2UH-F.js","_app/immutable/chunks/entry.B45YwSZQ.js","_app/immutable/chunks/scheduler.C9xG8wYf.js","_app/immutable/chunks/index.U-Bm33l8.js","_app/immutable/entry/app.BrF4UfsO.js","_app/immutable/chunks/scheduler.C9xG8wYf.js","_app/immutable/chunks/index.B3KkDgTn.js"],"stylesheets":[],"fonts":[],"uses_env_dynamic_public":false},
		nodes: [
			__memo(() => import('./nodes/0.js')),
			__memo(() => import('./nodes/1.js')),
			__memo(() => import('./nodes/2.js')),
			__memo(() => import('./nodes/3.js')),
			__memo(() => import('./nodes/4.js'))
		],
		routes: [
			{
				id: "/",
				pattern: /^\/$/,
				params: [],
				page: { layouts: [0,], errors: [1,], leaf: 2 },
				endpoint: null
			},
			{
				id: "/Blog",
				pattern: /^\/Blog\/?$/,
				params: [],
				page: { layouts: [0,], errors: [1,], leaf: 3 },
				endpoint: null
			},
			{
				id: "/Work",
				pattern: /^\/Work\/?$/,
				params: [],
				page: { layouts: [0,], errors: [1,], leaf: 4 },
				endpoint: null
			}
		],
		matchers: async () => {
			
			return {  };
		},
		server_assets: {}
	}
}
})();
