<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2022-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import type { ElementEvent } from "$lib";
    import type { HTMLAttributes } from "svelte/elements";

    let { children, onfilesselected, ...props }: {
        onfilesselected?: (files: File[]) => void;
        children?: any,
    } & HTMLAttributes<HTMLInputElement> = $props();

    let newProps = $derived.by(() => {
        let newProps = { ...props };
        delete newProps.class;
        delete newProps.style;
        return newProps;
    });

    function filesSelected(event: ElementEvent<Event, HTMLInputElement>) {
        props.onchange?.(event);

        if(!onfilesselected || !event.currentTarget?.files)
            return;

        const files: File[] = [];

        for(const file of event.currentTarget.files) {
            files.push(file);
        }

        onfilesselected(files);
    }
</script>

<div class={["drop-zone", "file", props.class]} style={props.style}>
    <input type="file" onchange={filesSelected} {...newProps} />
    <div class="drop-zone-overlay">
        {#if children}
            {@render children?.()}
        {:else}
            Drop file(s)
        {/if}
    </div>
</div>

<style>
    .drop-zone.file
    {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        opacity: 0;
        transition: opacity 0.5s;
        background: repeating-linear-gradient(-45deg, rgba(34, 34, 34, 0.75), rgba(34, 34, 34, 0.75) 10px, rgba(51, 51, 51, 0.75) 10px, rgba(51, 51, 51, 0.75) 20px); /* Thanks to CSS-Tricks: https://css-tricks.com/stripes-css/ */
        display: flex;
        align-items: center;
        justify-content: center;
        pointer-events: none;
        z-index: 9999;
    }

    .drop-zone.file > input[type="file"]:first-of-type
    {
        position: absolute;
        display: block;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        opacity: 0;
    }

    :global(body.is-dragging) .drop-zone.file
    {
        opacity: 1;
        pointer-events: all;
    }
</style>
