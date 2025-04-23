<script>
    import emailjs from 'emailjs-com';
    import { onMount } from 'svelte';
    import { writable } from 'svelte/store';

    // Initialize EmailJS on mount
    onMount(() => {
        emailjs.init('FSSyM2wdBcC14IGPQ'); // Replace with your actual EmailJS user ID
    });

    // Function to send email
    async function sendEmail(event) {
        event.preventDefault();

        const name = document.getElementById('name').value;
        const email = document.getElementById('email').value;
        const message = document.getElementById('message').value;

        try {
            const response = await emailjs.send('service_gxfowql', 'template_cxovil8', {
                from_name: name,
                from_email: email,
                message: message
            });
            document.getElementById('email-form').reset();
            closeModal();
        } catch (error) {
            alert('Failed to send email. I probably hit the max token limit @-@ ' + error.text);
        }
    }

    // State for modal visibility
    let isModalVisible = false;
    
    // Functions to open and close modal
    function openModal() {
        isModalVisible = true;
    }

    function closeModal() {
        isModalVisible = false;
    }

    const username = 'jacksonalvarez'; // Replace with your GitHub username
    const token = import.meta.env.VITE_GITHUB_TOKEN; // Replace with your GitHub token if needed

    const projects = writable([]); 
    const commitData = writable([]);

    onMount(async () => {
        await fetchGitHubData();
    });

    async function fetchGitHubData() {
        const reposResponse = await fetch(`https://api.github.com/users/${username}/repos`, {
            headers: token ? { Authorization: `token ${token}` } : {}
        });
        const repos = await reposResponse.json();

        const commitsResponse = await fetch(`https://api.github.com/users/${username}/events`, {
            headers: token ? { Authorization: `token ${token}` } : {}
        });
        const commits = await commitsResponse.json();

        projects.set(repos);
        processCommitData(commits);
    }

    function processCommitData(commits) {
        const commitCounts = {};
        
        commits.forEach(commit => {
            const date = new Date(commit.created_at).toDateString();
            commitCounts[date] = (commitCounts[date] || 0) + 1;
        });

        commitData.set(commitCounts);
    }

    // Prepare data for the chart
    let labels = [];
    let data = [];
    commitData.subscribe(commitCounts => {
        labels = Object.keys(commitCounts);
        data = Object.values(commitCounts);
    });
</script>
<div class="glowing-line-break"></div>

<div class="py-4 container mx-auto flex justify-center items-center">
    <div class="space-y-10 text-center flex flex-col items-center">
        <h2 class="h2">- Jackson Alvarez -</h2>
        
        <!-- Animated Background and Profile Image -->
        <figure class="relative">
            <section class="img-bg"></section>
            <img
                src="C0019T01.jpg" 
                alt="Jackson Alvarez" 
                class="w-48 h-48 md:w-64 md:h-64 rounded-full object-cover shadow-lg relative z-10"
            />
        </figure>
        
        <p class="text-lg">
            My dream is to develop software that makes a difference. I enjoy creating engaging,
            responsive applications and exploring new technologies. Currently looking for full-time work.
        </p>

        <!-- Button with Personal Link -->
        <div class="flex justify-center space-x-2">
            <a
                href="https://www.linkedin.com/in/jackson-alvarez-911b12187/"
                target="_blank"
                rel="noreferrer"
            >
                <img
                    src="174857.png" 
                    alt="LinkedIn"
                    class="w-10 h-10 md:w-10 md:h-10" 
                />
            </a>

            <a
                href="https://github.com/jacksonalvarez"
                target="_blank"
                rel="noreferrer"
            >
                <img
                    src="25231.png" 
                    alt="GitHub"
                    class="w-10 h-10 md:w-10 md:h-10" 
                />
            </a>
            <a class="btn variant-filled" href="Resume-JacksonAlvarez-Developer.pdf" download>Résumé</a>
            <button class="btn variant-filled" on:click={openModal}>Email me</button>
        </div>
    </div>
</div>

<!-- Add a Line Break or Spacing Between Intro and Projects -->
<div class="mt-4"></div> <!-- This adds margin-top between sections -->
<div class="glowing-line-break"></div>

<section class="content-section mb-8 text-center">

    <div class="grid px-4 grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-3 xl:grid-cols-3 gap-6">
        {#each $projects as repo}
            <a href="{repo.html_url}" target="_blank" class="card shadow-md bg-tertiary-bg text-tertiary-text border border-base-200 rounded-lg p-2 transition-all hover:shadow-lg hover:border-2 hover:border-primary-500">
                <div class="card-content">
                    <h2 class="font-semibold text-xl secondary-text py-2">{repo.name}</h2>
                    <p class="py-2">{repo.description || 'No description available.'}</p>
                </div>
            </a>
        {/each}
    </div>
</section>

<!-- Email Modal -->
{#if isModalVisible}
    <div id="email-modal" class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
        <div class="bg-white p-6 rounded-lg max-w-md w-full relative space-y-4 animate-wobble-in">
            <h3 class="text-xl font-semibold text-center">Contact Me</h3>
            <form id="email-form" class="space-y-4" on:submit={sendEmail}>
                <div>
                    <label for="name" class="block text-sm font-medium text-gray-700">Your Name</label>
                    <input type="text" id="name" class="w-full border-gray-300 rounded-md text-gray-700" required />
                </div>
                <div>
                    <label for="email" class="block text-sm font-medium text-gray-700">Your Email</label>
                    <input type="email" id="email" class="w-full border-gray-300 rounded-md text-gray-700" required />
                </div>
                <div>
                    <label for="message" class="block text-sm font-medium text-gray-700">Message</label>
                    <textarea id="message" class="w-full border-gray-300 rounded-md text-gray-700" rows="4" required></textarea>
                </div>
                <button type="submit" class="btn variant-filled w-full">Send</button>
            </form>
            <!-- Close Button -->
            <button class="absolute top-4 right-4 text-gray-500 hover:text-gray-700" on:click={closeModal}>
                &times;
            </button>
        </div>
    </div>
{/if}

<!-- Tailwind + Custom CSS for Wobble Animation -->
<style>
    @keyframes wobble-in {
        0% {
            transform: translateX(-50%) rotate(5deg);
        }
        15% {
            transform: translateX(3%) rotate(-3deg);
        }
        30% {
            transform: translateX(-2%) rotate(2deg);
        }
        45% {
            transform: translateX(1%) rotate(-1deg);
        }
        60% {
            transform: translateX(-1%) rotate(0deg);
        }
        100% {
            transform: translateX(0) rotate(0deg);
        }
    }

    .animate-wobble-in {
        animation: wobble-in 1s ease-in-out;
    }

    /* General figure styles */
    figure {
        @apply flex relative flex-col items-center;
    }

    /* SVG and background styling for logo */
    figure svg,
    .img-bg {
        @apply w-64 h-64 md:w-80 md:h-80;
    }

    /* Background animation */
    .img-bg {
        @apply absolute z-[-1] rounded-full blur-[50px] transition-all;
        animation: pulse 5s cubic-bezier(0, 0, 0, 0.5) infinite,
        glow 5s linear infinite;
    }

    /* Keyframes for the glowing and pulsing background */
    @keyframes glow {
        0% {
            @apply bg-primary-400/50;
        }
        33% {
            @apply bg-secondary-400/50;
        }
        66% {
            @apply bg-tertiary-400/50;
        }
        100% {
            @apply bg-primary-400/50;
        }
    }

    /* Glowing Line Break */
    .glowing-line-break {
        height: 2px;
        width: 100%;
        background: linear-gradient(to right, rgba(240, 124, 73, 0.7), rgba(90, 107, 250, 0.7), rgba(240, 124, 73, 0.7));
        box-shadow: 0 0 10px rgba(240, 124, 73, 0.7), 0 0 20px rgba(90, 107, 250, 0.7);
        animation: glowing 5s infinite alternate;
        margin-bottom: 30px;
    }

    /* Keyframes for Glowing Animation */
    @keyframes glowing {
        0% {
            box-shadow: 0 0 10px rgba(240, 124, 73, 0.7), 0 0 20px rgba(90, 107, 250, 0.7);
        }
        50% {
            box-shadow: 0 0 20px rgba(240, 124, 73, 0.8), 0 0 40px rgba(90, 107, 250, 0.8);
        }
        100% {
            box-shadow: 0 0 10px rgba(240, 124, 73, 0.7), 0 0 20px rgba(90, 107, 250, 0.7);
        }
    }

    /* Custom Tailwind colors */
    .secondary-bg { background-color: #38c172; } /* Green */
    .tertiary-bg { background-color: #f9f9f9; } /* Light Grey */
    .primary-text { color: #f9f9f9; }
    .secondary-text { color: #38c172; }
    .tertiary-text { color: #6c757d; }

    /* Glowing Effect for Buttons and Cards */
    .btn:hover {
        box-shadow: 0 0 8px rgba(240, 124, 73, 0.7), 0 0 16px rgba(90, 107, 250, 0.7);
    }

    .card:hover {
        box-shadow: 0 0 8px rgba(240, 124, 73, 0.7), 0 0 16px rgba(90, 107, 250, 0.7);
    }

    .card:hover {
        border: 2px solid rgba(240, 124, 73, 0.7);
    }
</style>