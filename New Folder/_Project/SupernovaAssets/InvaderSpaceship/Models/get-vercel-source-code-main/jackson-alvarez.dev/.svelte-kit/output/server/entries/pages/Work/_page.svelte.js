import { c as create_ssr_component, h as subscribe, i as each, e as escape, a as add_attribute } from "../../../chunks/ssr.js";
import { w as writable } from "../../../chunks/index.js";
const css = {
  code: ":root{--primary-color:#3b82f6;--secondary-color:#ef4444;--tertiary-color:#3f83f8;--green-color:#22c55e}.green-title.svelte-yr90lh{color:var(--green-color)}",
  map: `{"version":3,"file":"+page.svelte","sources":["+page.svelte"],"sourcesContent":["<script>\\n    import { onMount } from 'svelte';\\n    import { writable } from 'svelte/store';\\n\\n    const username = 'jacksonalvarez'; // Replace with your GitHub username\\n    const token = import.meta.env.VITE_GITHUB_TOKEN; // Replace with your GitHub token if needed\\n\\n    const projects = writable([]);\\n    const commitData = writable([]);\\n\\n    onMount(async () => {\\n        await fetchGitHubData();\\n    });\\n\\n    async function fetchGitHubData() {\\n        const reposResponse = await fetch(\`https://api.github.com/users/\${username}/repos\`, {\\n            headers: token ? { Authorization: \`token \${token}\` } : {}\\n        });\\n        const repos = await reposResponse.json();\\n\\n        const commitsResponse = await fetch(\`https://api.github.com/users/\${username}/events\`, {\\n            headers: token ? { Authorization: \`token \${token}\` } : {}\\n        });\\n        const commits = await commitsResponse.json();\\n\\n        projects.set(repos);\\n        processCommitData(commits);\\n    }\\n\\n    function processCommitData(commits) {\\n        const commitCounts = {};\\n        \\n        commits.forEach(commit => {\\n            const date = new Date(commit.created_at).toDateString();\\n            commitCounts[date] = (commitCounts[date] || 0) + 1;\\n        });\\n\\n        commitData.set(commitCounts);\\n    }\\n\\n    // Prepare data for the chart\\n    let labels = [];\\n    let data = [];\\n    commitData.subscribe(commitCounts => {\\n        labels = Object.keys(commitCounts);\\n        data = Object.values(commitCounts);\\n    });\\n<\/script>\\n\\n<style>\\n    :root {\\n        --primary-color: #3b82f6;\\n        --secondary-color: #ef4444;\\n        --tertiary-color: #3f83f8;\\n        --green-color: #22c55e; /* Custom green color */\\n    }\\n\\n    .green-title {\\n        color: var(--green-color); /* Apply the green color */\\n    }\\n</style>\\n\\n<div class=\\"container h-full mx-auto flex flex-col items-center\\">    \\n    <!-- New Content Section -->\\n    <section class=\\"content-section mb-8 text-center\\">\\n        <h2 class=\\"text-2xl font-semibold text-primary-600 mb-2\\">About My Work</h2>\\n        <p class=\\"text-lg max-w-xl mx-auto\\">\\n            Update this junk\\n        </p>\\n    </section>\\n    \\n    <hr class=\\"w-full border-t-2 border-gray-300 my-6\\" />\\n        <section class=\\"content-section mb-8 text-center\\">\\n        <h2 class=\\"text-2xl font-semibold text-primary-600 mb-2\\">About My Work</h2>\\n\\n    <!-- Projects Section -->\\n    <div class=\\"container grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6\\">\\n    {#each $projects as repo}\\n        <div class=\\"card shadow-md bg-base-100 text-base-content border border-base-200 rounded-lg p-6 transition-all hover:shadow-lg hover:bg-base-200\\">\\n            <div class=\\"card-content\\">\\n                <h2 class=\\"text-xl font-semibold text-2xl mb-2 green-title\\">{repo.name}</h2>\\n                <p class=\\"text-sm text-base-content mb-4\\">{repo.description || 'No description available.'}</p>\\n                <a href=\\"{repo.html_url}\\" target=\\"_blank\\" class=\\"btn bg-primary text-primary-content font-medium py-2 rounded-md w-full hover:bg-primary-focus\\">\\n                    View on GitHub\\n                </a>\\n            </div>\\n        </div>\\n    {/each}\\n</div>\\n</section>\\n\\n\\n    <!-- Chart Script Section -->\\n    <script>\\n        import Chart from 'chart.js/auto';\\n\\n        // Create the chart\\n        $: if (labels.length && data.length) {\\n            const ctx = document.getElementById('commitChart').getContext('2d');\\n            new Chart(ctx, {\\n                type: 'line',\\n                data: {\\n                    labels: labels,\\n                    datasets: [{\\n                        label: 'Commits',\\n                        data: data,\\n                        borderColor: 'var(--primary-color)',\\n                        backgroundColor: 'rgba(59, 130, 246, 0.2)',\\n                        borderWidth: 1,\\n                    }]\\n                },\\n                options: {\\n                    scales: {\\n                        x: {\\n                            title: {\\n                                display: true,\\n                                text: 'Date'\\n                            }\\n                        },\\n                        y: {\\n                            title: {\\n                                display: true,\\n                                text: 'Number of Commits'\\n                            }\\n                        }\\n                    }\\n                }\\n            });\\n        }\\n    <\/script>\\n</div>\\n"],"names":[],"mappings":"AAkDI,KAAM,CACF,eAAe,CAAE,OAAO,CACxB,iBAAiB,CAAE,OAAO,CAC1B,gBAAgB,CAAE,OAAO,CACzB,aAAa,CAAE,OACnB,CAEA,0BAAa,CACT,KAAK,CAAE,IAAI,aAAa,CAC5B"}`
};
const Page = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let $projects, $$unsubscribe_projects;
  const projects = writable([]);
  $$unsubscribe_projects = subscribe(projects, (value) => $projects = value);
  const commitData = writable([]);
  commitData.subscribe((commitCounts) => {
    Object.values(commitCounts);
  });
  $$result.css.add(css);
  $$unsubscribe_projects();
  return `<div class="container h-full mx-auto flex flex-col items-center"> <section class="content-section mb-8 text-center" data-svelte-h="svelte-of3mgb"><h2 class="text-2xl font-semibold text-primary-600 mb-2">About My Work</h2> <p class="text-lg max-w-xl mx-auto">Update this junk</p></section> <hr class="w-full border-t-2 border-gray-300 my-6"> <section class="content-section mb-8 text-center"><h2 class="text-2xl font-semibold text-primary-600 mb-2" data-svelte-h="svelte-umst2n">About My Work</h2>  <div class="container grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">${each($projects, (repo) => {
    return `<div class="card shadow-md bg-base-100 text-base-content border border-base-200 rounded-lg p-6 transition-all hover:shadow-lg hover:bg-base-200"><div class="card-content"><h2 class="text-xl font-semibold text-2xl mb-2 green-title svelte-yr90lh">${escape(repo.name)}</h2> <p class="text-sm text-base-content mb-4">${escape(repo.description || "No description available.")}</p> <a${add_attribute("href", repo.html_url, 0)} target="_blank" class="btn bg-primary text-primary-content font-medium py-2 rounded-md w-full hover:bg-primary-focus">View on GitHub
                </a></div> </div>`;
  })}</div></section>  <script data-svelte-h="svelte-5yez9a">import Chart from 'chart.js/auto';

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
        }<\/script></div>`;
});
export {
  Page as default
};
