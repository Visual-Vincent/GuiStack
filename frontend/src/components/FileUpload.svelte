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

    let { children, title, disabled = false, onfilesselected, ...props }: {
        title?: string,
        disabled?: boolean,
        onfilesselected?: (files: File[]) => void,
        children?: any,
    } & HTMLAttributes<HTMLInputElement> = $props();

    let newProps = $derived.by(() => {
        let newProps = { ...props };
        delete newProps.class;
        delete newProps.style;
        delete newProps.onchange;
        return newProps;
    });

    function filesSelected(event: ElementEvent<Event, HTMLInputElement>) {
        if(disabled)
            return;

        props.onchange?.(event);

        if(!onfilesselected || !event.currentTarget?.files)
            return;

        const files: File[] = [];

        for(const file of event.currentTarget.files) {
            files.push(file);
        }

        // Clear the input so that the onchange event will fire every time, even though the same file(s) are selected again
        event.currentTarget.value = "";

        onfilesselected(files);
    }
</script>

<!-- svelte-ignore a11y_invalid_attribute -->
<a href="javascript:void(0)" title={title} class={["upload-box", (disabled ? "disabled" : null), props.class]} style={props.style}>
    <input type="file" onchange={filesSelected} {...newProps} />
    {@render children?.()}
</a>

<style>
    .upload-box
    {
        display: block;
    }

    .upload-box:not(.borderless)
    {
        border: 1px dashed;
    }

    .upload-box:not(.invisible)
    {
        position: relative;
        padding: 8px;
        cursor: pointer;
    }

    .upload-box.invisible
    {
        padding: 0;
        opacity: 0;
        border: none;
    }

    .upload-box > input[type="file"]:first-of-type
    {
        position: absolute;
        display: block;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        opacity: 0;
        cursor: pointer;
    }

    .upload-box > input[type="file"]:first-of-type::file-selector-button
    {
        display: none;
        visibility: hidden;
        width: 0px;
        height: 0px;
    }

    .upload-box.disabled,
    .upload-box.disabled:hover,
    .upload-box.disabled:active,
    .upload-box.disabled:visited
    {
        color: #888888;
        cursor: default;
        pointer-events: none;
    }

    .upload-box.disabled > input[type="file"]:first-of-type
    {
        cursor: default;
        pointer-events: none;
    }
</style>
