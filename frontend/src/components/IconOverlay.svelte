<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2025-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import type { HTMLAttributes } from "svelte/elements";

    let { icon, size, ...props }: {
        icon: string,
        size?: string | number,
    } & HTMLAttributes<HTMLSpanElement> = $props();

    let style = $derived.by(() => {
        const styles: string[] = [];

        if(size)
        {
            if(typeof size === "string")
                styles.push(`--size: ${size}`);
            else
                styles.push(`--size: ${size}px`);
        }
        else
        {
            styles.push("--size: 0.5em");
        }

        if(props?.style)
            styles.push(props.style);

        return styles.join(" ");
    });

    let newProps = $derived.by(() => {
        let newProps = { ...props };
        delete newProps.class;
        delete newProps.style;
        return newProps;
    });
</script>

<i class={[icon, "gs-icon-overlay", props.class]} style={style} {...newProps}></i>

<style>
    i
    {
        position: absolute;
        font-size: var(--size);
        bottom: -2px;
        right: 4px;
    }
</style>
