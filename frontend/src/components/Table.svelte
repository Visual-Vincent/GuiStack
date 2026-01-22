<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2025-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import type { HTMLAttributes } from 'svelte/elements';
    import Renderable from './Renderable.svelte';

    let {
        content,
        children,
        ...props
    }: {
        content: any[] | null | undefined,
        children?: any,
    } & HTMLAttributes<HTMLTableElement> = $props();

    const columns: string[] = $derived.by(() => {
        const columns: Set<string> = new Set<string>();

        for(const item of content ?? []) {
            for(const property in item) {
                columns.add(property);
            }
        }

        return Array.from(columns.keys());
    });
</script>

{#if content && content.length > 0}
    <table {...props}>
        <thead>
            <tr>
                {#each columns as column}
                    <th>{column}</th>
                {/each}
            </tr>
        </thead>
        <tbody>
            {#each content as item}
                <tr>
                    {#each columns as column}
                        <td>
                            <Renderable content={item[column] ?? ""} />
                        </td>
                    {/each}
                </tr>
            {/each}
        </tbody>
    </table>
{:else}
    {#if children}
        {@render children()}
    {:else}
        <div>No data to show.</div>
    {/if}
{/if}

<style></style>
