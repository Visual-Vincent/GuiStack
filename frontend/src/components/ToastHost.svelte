<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2022-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import { Toasts, type Toast } from "$lib/toasts.svelte";
    import { onMount } from "svelte";
    import { linear } from "svelte/easing";
    import type { HTMLAttributes } from "svelte/elements";
    import { Tween } from "svelte/motion";

    const { icons, ...props }: {
        icons: any
    } & HTMLAttributes<HTMLDivElement> = $props();

    interface Timer
    {
        handle: ReturnType<typeof setTimeout>;
        progress: Tween<number>
    }

    let timers = $state<Record<Toast["id"], Timer>>({});

    $effect(() => {
        const activeToasts = new Set(Toasts.all.map(t => t.id));

        // Add timers for new toasts
        for(const toast of Toasts.all) {
            if(!toast.duration || toast.duration <= 0 || timers[toast.id])
                continue;

            const handle = setTimeout(() => {
                Toasts.remove(toast);
            }, toast.duration);

            const progress = new Tween(0, {
                duration: toast.duration,
                easing: linear
            });

            timers[toast.id] = { handle, progress };
            progress.set(1);
        }

        // Remove timers for toasts that no longer exist
        for(const id in timers) {
            if(activeToasts.has(id))
                continue;

            clearTimeout(timers[id].handle);
            delete timers[id];
        }
    });

    onMount(() => {
        // Cleanup if the component is unmounted
        return () => {
            for(const id in timers) {
                clearTimeout(timers[id].handle);
            }

            timers = {};
        }
    })
</script>

<div {...props}>
    {#each Toasts.all as toast}
        {@const icon = icons ? icons[toast.type] : null}
        {@const timer = toast.duration && toast.duration > 0 ? timers[toast.id] : null}

        <div class="toast {toast.type}">
            <!-- svelte-ignore a11y_invalid_attribute -->
            <a class="close-button" href="javascript:void(0)" onclick={() => Toasts.remove(toast)}>&times;</a>
            <div class="title">
                {#if icon}
                    <i class={icon}></i>
                {/if}
                {toast.title}
            </div>

            {#if toast.text && toast.text.length > 0}
                <div class="text">{toast.text}</div>
            {/if}

            {#if timer}
                <progress value={1 - timer.progress.current}></progress>
            {/if}
        </div>
    {/each}
</div>

<style>
    .toast
    {
        position: relative;
        box-sizing: border-box;
        padding: 12px;
        margin: 8px auto;
        background: #E8E8E8;
        border: 1px solid #585858;
        border-radius: 8px;
        /* box-shadow: #585858 0px 0px 8px 2px; */
        color: #585858;
        width: 100%;
        overflow: hidden;
    }

    .toast > .title
    {
        font-weight: bold;
    }

    .toast > .text
    {
        margin-top: 8px;
    }

    .toast > .close-button
    {
        position: absolute;
        top: 8px;
        right: 8px;
        width: 16px;
        height: 16px;
    }

    .toast > .close-button,
    .toast > .close-button:hover,
    .toast > .close-button:active,
    .toast > .close-button:visited
    {
        color: inherit;
    }

    .toast.info
    {
        background: #D7E2F8;
        border: 1px solid #12419F;
        /* box-shadow: #12419F 0px 0px 8px 2px; */
        color: #12419F;
    }

    .toast.success
    {
        background: #D7F8DB;
        border: 1px solid #129F25;
        /* box-shadow: #129F25 0px 0px 8px 2px; */
        color: #129F25;
    }

    .toast.warning
    {
        background: #f8f8d7;
        border: 1px solid #9F5B12;
        /* box-shadow: #9F5B12 0px 0px 8px 2px; */
        color: #9F5B12;
    }

    .toast.error
    {
        background: #F8D7DA;
        border: 1px solid #9F121E;
        /* box-shadow: #9F121E 0px 0px 8px 2px; */
        color: #9F121E;
    }

    .toast > progress
    {
        appearance: none;
        background: none;
        position: absolute;
        display: block;
        width: 100%;
        height: 2px;
        left: 0;
        bottom: 0;
        transform: scaleX(-1);
    }

    .toast > progress::-webkit-progress-bar
    {
        background: none;
    }

    .toast.info    > progress::-webkit-progress-value { background: #12419F; }
    .toast.success > progress::-webkit-progress-value { background: #129F25; }
    .toast.warning > progress::-webkit-progress-value { background: #9F5B12; }
    .toast.error   > progress::-webkit-progress-value { background: #9F121E; }

    .toast.info    > progress::-moz-progress-bar { background: #12419F; }
    .toast.success > progress::-moz-progress-bar { background: #129F25; }
    .toast.warning > progress::-moz-progress-bar { background: #9F5B12; }
    .toast.error   > progress::-moz-progress-bar { background: #9F121E; }
</style>