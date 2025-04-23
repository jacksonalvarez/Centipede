import{s as O,n as S,c as P,o as W}from"../chunks/scheduler.C9xG8wYf.js";import{S as F,i as L,e as x,s as j,c as _,a as E,k as $,f as H,d as y,l as f,g as U,h as l,m as Q,t as k,b as A,j as V}from"../chunks/index.B3KkDgTn.js";import{w as N}from"../chunks/index.U-Bm33l8.js";function M(r){return(r==null?void 0:r.length)!==void 0?r:Array.from(r)}function T(r,t,a){const d=r.slice();return d[8]=t[a],d}function z(r){let t,a,d,g=r[8].name+"",p,w,c,m=(r[8].description||"No description available.")+"",u,b,o,v,C,I;return{c(){t=x("div"),a=x("div"),d=x("h2"),p=k(g),w=j(),c=x("p"),u=k(m),b=j(),o=x("a"),v=k("View on GitHub"),I=j(),this.h()},l(i){t=_(i,"DIV",{class:!0});var e=E(t);a=_(e,"DIV",{class:!0});var s=E(a);d=_(s,"H2",{class:!0});var h=E(d);p=A(h,g),h.forEach(y),w=H(s),c=_(s,"P",{class:!0});var n=E(c);u=A(n,m),n.forEach(y),b=H(s),o=_(s,"A",{href:!0,target:!0,class:!0});var D=E(o);v=A(D,"View on GitHub"),D.forEach(y),s.forEach(y),I=H(e),e.forEach(y),this.h()},h(){f(d,"class","text-xl font-semibold text-2xl mb-2 green-title svelte-yr90lh"),f(c,"class","text-sm text-base-content mb-4"),f(o,"href",C=r[8].html_url),f(o,"target","_blank"),f(o,"class","btn bg-primary text-primary-content font-medium py-2 rounded-md w-full hover:bg-primary-focus"),f(a,"class","card-content"),f(t,"class","card shadow-md bg-base-100 text-base-content border border-base-200 rounded-lg p-6 transition-all hover:shadow-lg hover:bg-base-200")},m(i,e){U(i,t,e),l(t,a),l(a,d),l(d,p),l(a,w),l(a,c),l(c,u),l(a,b),l(a,o),l(o,v),l(t,I)},p(i,e){e&1&&g!==(g=i[8].name+"")&&V(p,g),e&1&&m!==(m=(i[8].description||"No description available.")+"")&&V(u,m),e&1&&C!==(C=i[8].html_url)&&f(o,"href",C)},d(i){i&&y(t)}}}function q(r){let t,a,d='<h2 class="text-2xl font-semibold text-primary-600 mb-2">About My Work</h2> <p class="text-lg max-w-xl mx-auto">Update this junk</p>',g,p,w,c,m,u="About My Work",b,o,v,C,I=`import Chart from 'chart.js/auto';

        // Create the chart
        $: if (labels.length && data.length) {
            const ctx = document.getElementById('commitChart').getContext('2d');
            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Commits',
                        data: data,
                        borderColor: 'var(--primary-color)',
                        backgroundColor: 'rgba(59, 130, 246, 0.2)',
                        borderWidth: 1,
                    }]
                },
                options: {
                    scales: {
                        x: {
                            title: {
                                display: true,
                                text: 'Date'
                            }
                        },
                        y: {
                            title: {
                                display: true,
                                text: 'Number of Commits'
                            }
                        }
                    }
                }
            });
        }`,i=M(r[0]),e=[];for(let s=0;s<i.length;s+=1)e[s]=z(T(r,i,s));return{c(){t=x("div"),a=x("section"),a.innerHTML=d,g=j(),p=x("hr"),w=j(),c=x("section"),m=x("h2"),m.textContent=u,b=j(),o=x("div");for(let s=0;s<e.length;s+=1)e[s].c();v=j(),C=x("script"),C.textContent=I,this.h()},l(s){t=_(s,"DIV",{class:!0});var h=E(t);a=_(h,"SECTION",{class:!0,"data-svelte-h":!0}),$(a)!=="svelte-of3mgb"&&(a.innerHTML=d),g=H(h),p=_(h,"HR",{class:!0}),w=H(h),c=_(h,"SECTION",{class:!0});var n=E(c);m=_(n,"H2",{class:!0,"data-svelte-h":!0}),$(m)!=="svelte-umst2n"&&(m.textContent=u),b=H(n),o=_(n,"DIV",{class:!0});var D=E(o);for(let R=0;R<e.length;R+=1)e[R].l(D);D.forEach(y),n.forEach(y),v=H(h),C=_(h,"SCRIPT",{"data-svelte-h":!0}),$(C)!=="svelte-5yez9a"&&(C.textContent=I),h.forEach(y),this.h()},h(){f(a,"class","content-section mb-8 text-center"),f(p,"class","w-full border-t-2 border-gray-300 my-6"),f(m,"class","text-2xl font-semibold text-primary-600 mb-2"),f(o,"class","container grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"),f(c,"class","content-section mb-8 text-center"),f(t,"class","container h-full mx-auto flex flex-col items-center")},m(s,h){U(s,t,h),l(t,a),l(t,g),l(t,p),l(t,w),l(t,c),l(c,m),l(c,b),l(c,o);for(let n=0;n<e.length;n+=1)e[n]&&e[n].m(o,null);l(t,v),l(t,C)},p(s,[h]){if(h&1){i=M(s[0]);let n;for(n=0;n<i.length;n+=1){const D=T(s,i,n);e[n]?e[n].p(D,h):(e[n]=z(D),e[n].c(),e[n].m(o,null))}for(;n<e.length;n+=1)e[n].d(1);e.length=i.length}},i:S,o:S,d(s){s&&y(t),Q(e,s)}}}const G="jacksonalvarez";function B(r,t,a){let d;const g="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",p=N([]);P(r,p,u=>a(0,d=u));const w=N([]);W(async()=>{await c()});async function c(){const b=await(await fetch(`https://api.github.com/users/${G}/repos`,{headers:{Authorization:`token ${g}`}})).json(),v=await(await fetch(`https://api.github.com/users/${G}/events`,{headers:{Authorization:`token ${g}`}})).json();p.set(b),m(v)}function m(u){const b={};u.forEach(o=>{const v=new Date(o.created_at).toDateString();b[v]=(b[v]||0)+1}),w.set(b)}return w.subscribe(u=>{Object.values(u)}),[d,p]}class X extends F{constructor(t){super(),L(this,t,B,q,O,{})}}export{X as component};
