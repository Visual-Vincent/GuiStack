<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import type { HTMLAttributes } from "svelte/elements";

    let { children, ...props }: {
        children?: any,
    } & HTMLAttributes<HTMLDivElement> = $props();

    let newProps = $derived.by(() => {
        let newProps = { ...props };
        delete newProps.class;
        delete newProps.style;
        return newProps;
    });
</script>

<div class={["drop-container", props.class]} style={props.style} {...newProps}>
    {@render children?.()}
</div>

<style>
    .drop-container
    {
        position: relative !important;
    }

    .drop-container > :global(.drop-zone)
    {
        z-index: initial;
    }
</style>
