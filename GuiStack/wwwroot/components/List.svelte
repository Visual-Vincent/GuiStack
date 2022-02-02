<svelte:options tag="guistack-list"/>
<script>
    export let columns = [];
    export let items = []; // Jagged array [["column 1", "column 2", ...], ["column 1", "column 2", ...], ...]
</script>

<main>
    <table class="gs-list">
        <thead>
            <tr>
            {#each columns as column, i}
                <th>{column}</th>
            {/each}
            </tr>
        </thead>
        <tbody>
        {#each items as item, i}
            <tr>
            {#each item as value, j}
                <td>{@html value}</td>
            {/each}
            </tr>
        {/each}
        </tbody>
    </table>
</main>

<style>
    table.gs-list
    {
        width: 100%;
        border: 1px solid rgba(255, 255, 255, 0.5);
        border-collapse: collapse;
    }

    table.gs-list th, table.gs-list td
    {
        border: 1px solid rgba(255, 255, 255, 0.5);
        border-collapse: collapse;
    }
</style>