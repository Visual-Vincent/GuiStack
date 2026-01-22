<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2025-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import DashboardItem from "./DashboardItem.svelte";

    export interface IDashboardItem {
        name: string;
        url: string;
        icon: string;
    }

    let { items }: { items: IDashboardItem[] } = $props();
</script>

<div class="dashboard">
    {#each items as item}
        {#if item.name && item.icon}
            <DashboardItem name={item.name} icon={item.icon} url={item.url} />
        {/if}
    {/each}
</div>

<style>
    .dashboard
    {
        display: grid;
        grid-auto-rows: 120px;
        grid-template-columns: repeat(auto-fit, 120px);
        grid-gap: 16px;
    }
</style>
