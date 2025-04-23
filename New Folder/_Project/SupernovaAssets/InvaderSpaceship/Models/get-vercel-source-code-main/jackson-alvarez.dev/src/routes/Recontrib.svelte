<script>
    export let data = []; // Array of week data
    export let gridSize = 16; // Size of the tile
    export let fontSize = "0.875rem"; // Font size of months and weekdays
</script>

<style>
    .recontrib-graph {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(var(--grid-size), 1fr));
        gap: 2px;
    }

    .tile {
        border-radius: 3px;
    }

    .total {
        font-size: var(--font-size);
        text-align: center;
    }
</style>

<div class="recontrib-graph" style="--grid-size: {gridSize}px">
    {#each data as weekData}
        <div>
            {#each weekData.days as commits, dayIndex}
                <span
                    class="tile"
                    style="
                        width: {gridSize}px;
                        height: {gridSize}px;
                        background-color: {commits > 0 ? `rgba(0, 128, 0, ${commits / Math.max(...weekData.days)})` : '#ebedf0'};
                    "
                    use:tooltip={{ content: `${commits} commits on ${new Date(weekData.week * 1000 + dayIndex * 86400000).toLocaleDateString()}` }}
                ></span>
            {/each}
            <span class="total">{weekData.total} commits</span>
        </div>
    {/each}
</div>
