<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2022-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import type { HTMLAttributes } from "svelte/elements";

    let { icon, href, size, children, ...props }: {
        icon: string,
        href?: string,
        size?: string | number,
        children?: any,
    } & HTMLAttributes<HTMLAnchorElement> = $props();

    let style = $derived.by(() => {
        const styles: string[] = [];

        if(size)
        {
            if(typeof size === "string")
                styles.push(`--size: ${size}`);
            else
                styles.push(`--size: ${size}px`);
        }

        if(props?.style)
            styles.push(props.style);

        return styles.join(" ");
    });

    let newProps = $derived.by(() => {
        let newProps = { ...props };
        delete newProps.style;
        return newProps;
    });
</script>

<a href={href ?? "javascript:void(0)"} style={style} {...newProps}>
    <i class={icon}></i>
    {@render children?.()}
</a>

<style>
    a
    {
        display: block;
        position: relative;
        font-size: var(--size);
        text-align: center;
        box-sizing: content-box;
        padding: 2px 4px 2px 4px;
    }

    a, a:visited
    {
        color: #FFFFFF;
        text-decoration: none;
    }

    a > i, a > i::before
    {
        display: block;
        box-sizing: content-box;
    }
</style>
