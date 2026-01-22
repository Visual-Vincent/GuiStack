<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2025-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    let { children = undefined } = $props();
</script>

<div class="icons">
    {@render children?.()}
</div>

<style>
    .icons
    {
        display: flex;
        flex-wrap: nowrap;
        align-content: center;
        justify-content: center;
        align-items: center;
    }
</style>
