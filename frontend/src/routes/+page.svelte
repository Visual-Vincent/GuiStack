<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2025-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import Dashboard from "$components/Dashboard.svelte"
	import { ProjectName, TopLevelPages } from "$lib";

    const items = TopLevelPages
        .filter(item => item.dashboardName && item.dashboardIcon)
        .map(item => ({
            name: item.dashboardName!,
            icon: item.dashboardIcon!,
            url: item.url
        }));
</script>

<svelte:head>
    <title>{ProjectName}</title>
</svelte:head>

<h1>Welcome to {ProjectName}</h1>
<Dashboard items={items} />
