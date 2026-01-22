<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2022-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import type { HTMLAttributes } from "svelte/elements";

    let {
        text,
        centered = true,
        spinnerRight = false,
        ...props
    }: {
        text?: string | null,
        centered?: boolean,
        spinnerRight?: boolean,
    } & HTMLAttributes<HTMLDivElement> = $props();

    let newProps = $derived.by(() => {
        let newProps = { ...props };
        delete newProps.class;
        return newProps;
    });
</script>

<div class={["loading-box", (centered ? "centered" : null), props.class]} {...newProps}>
    {spinnerRight ? text : ""}<img class={["spinner", (text && text.length > 0 ? "has-text" : null), (spinnerRight ? "right" : null)]} alt="loading" src="/img/loading_wheel.apng" />{!spinnerRight ? text : ""}
</div>

<style>
    .loading-box
    {
        display: flex;
        align-items: center;
    }

    .loading-box.centered
    {
        justify-content: center;
    }

    .loading-box > img.spinner
    {
        width: 1em;
    }

    .loading-box > img.spinner.has-text:not(.right)
    {
        margin-right: 0.375em;
    }

    .loading-box > img.spinner.has-text.right
    {
        margin-left: 0.375em;
    }
</style>
