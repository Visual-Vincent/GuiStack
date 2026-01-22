<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2022-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import type { HTMLAttributes } from "svelte/elements";

    let { children, disabled = false, ...props }: {
        disabled?: boolean,
        children: any
    } & HTMLAttributes<HTMLButtonElement> = $props();
</script>

<button {...props} disabled={disabled}>
    {@render children?.()}
</button>

<style>
    button
    {
        display: inline-block;
        background: #0099D4;
        color: #FFFFFF;
        border-radius: 4px;
        padding: 4px 16px;
    }

    button:hover    { background: #007EAF; }
    button:active   { background: #005577; }
    button:disabled { background: #888888; }
</style>
