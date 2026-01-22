<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2022-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import type { LoadingOverlayElementSettings } from "$lib/overlay.svelte";
    import type { HTMLAttributes } from "svelte/elements";

    let {
        children,
        spinner = true,
        spinnerRight = false,
        visible = false,
        ...props
    }: {
        children?: any,
    } & HTMLAttributes<HTMLDivElement>
        & LoadingOverlayElementSettings = $props();

    let newProps = $derived.by(() => {
        let newProps = { ...props };
        delete newProps.class;
        return newProps;
    });
</script>

{#if visible}
    <div class={["loading-overlay", props.class]} {...newProps}>
        {#if spinner && !spinnerRight}
            <img class="spinner" alt="loading" src="/img/loading_wheel.apng" />
        {/if}
        {#if children}
            {@render children?.()}
        {:else}
            Loading...
        {/if}
        {#if spinner && spinnerRight}
            <img class="spinner right" alt="loading" src="/img/loading_wheel.apng" />
        {/if}
    </div>
{/if}

<style>
    .loading-overlay
    {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: repeating-linear-gradient(-45deg, rgba(34, 34, 34, 0.75), rgba(34, 34, 34, 0.75) 10px, rgba(51, 51, 51, 0.75) 10px, rgba(51, 51, 51, 0.75) 20px); /* Thanks to CSS-Tricks: https://css-tricks.com/stripes-css/ */
        color: #FFFFFF;
        font-size: 2em;
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 19999;
    }

    .loading-overlay > img.spinner
    {
        width: 1em;
    }

    .loading-overlay > img.spinner:not(.right)
    {
        margin-right: 0.375em;
    }

    .loading-overlay > img.spinner.right
    {
        margin-left: 0.375em;
    }
</style>
