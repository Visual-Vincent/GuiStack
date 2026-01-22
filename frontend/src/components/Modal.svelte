<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2022-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import type { ElementEvent } from "$lib";
    import type { ModalButton } from "$lib/modals";
    import type { EventHandler, HTMLAttributes } from "svelte/elements";
    import Button from "./Button.svelte";

    let {
        shown = $bindable(false),
        title = undefined,
        text = undefined,
        buttons = undefined,
        onclick = undefined,
        onclose = undefined,
        closeButton = true,
        closeOnBackdropClick = true,
        children = undefined,
        ...props
    }: {
        shown?: boolean,
        title?: string | null,
        text?: string | null,
        closeButton?: boolean,
        closeOnBackdropClick?: boolean,
        buttons?: ModalButton[],
        onclick?: EventHandler<MouseEvent, HTMLDialogElement>,
        onclose?: EventHandler<Event, HTMLDialogElement>,
        children?: any,
    } & HTMLAttributes<HTMLDialogElement> = $props();

    let dialog: HTMLDialogElement | undefined = $state();

    $effect(() => {
        if(shown)
            dialog?.showModal();
    });

    function onClick(event: ElementEvent<MouseEvent, HTMLDialogElement>) {
        onclick?.(event);

        if(!closeOnBackdropClick || event.target !== dialog)
            return;

        var bounds = dialog.getBoundingClientRect();
        var insideDialog = (
            event.clientX >= bounds.x && event.clientX <= bounds.right &&
            event.clientY >= bounds.y && event.clientY <= bounds.bottom
        );

        if(!insideDialog)
            dialog?.close();
    }

    function onClose(event: ElementEvent<Event, HTMLDialogElement>) {
        shown = false;
        onclose?.(event);
    }

    export function close() {
        dialog?.close();
    }

    export function show() {
        shown = true;
    }

    const self = {
        close,
        show
    };
</script>

<dialog bind:this={dialog} onclick={onClick} onclose={onClose} {...props}>
    {#if closeButton}    
        <div class="close-button-container">
            <!-- svelte-ignore a11y_invalid_attribute -->
            <a href="javascript:void(0)" onclick={() => dialog?.close()} class="close-button">&times;</a>
        </div>
    {/if}
    {#if title}
        <h3 class="text-center">{title}</h3>
    {/if}
    {#if children}
        {@render children?.()}
    {:else}
        <p class="text-center">{text}</p>
    {/if}
    {#if buttons && buttons.length > 0}
        <div class="modal-buttons text-center">
            {#each buttons as button, index}
                <Button onclick={(event) => button.onclick(self, event)} disabled={button.disabled}>{button.text}</Button>
                {#if index < buttons.length - 1}
                    &#8203; <!-- Zero-width space -->
                {/if}
            {/each}
        </div>
    {/if}
</dialog>

<style>
    dialog
    {
        position: relative;
        color: #FFFFFF;
        background: #000000;
        border: 1px solid #FFFFFF;
        border-radius: 8px;
        max-width: calc(100vw - 16px);
        max-height: calc(100vh - 16px);
        padding: 12px;
    }

    dialog::backdrop
    {
        background: rgba(0, 0, 0, 0.5);
        -webkit-backdrop-filter: blur(7.5px);
        backdrop-filter: blur(7.5px);
    }

    a.close-button,
    a.close-button:visited
    {
        display: block;
        height: 0.5em;
        line-height: 0.25em;
        margin-top: 2px;
        color: #FFFFFF;
        text-decoration: none;
    }

    a.close-button:hover
    {
        color: #FF7F7F;
    }

    a.close-button:active
    {
        color: #FF0000;
    }

    .close-button-container
    {
        display: flex;
        font-size: 32px;
        justify-content: right;
        align-items: center;
    }

    .close-button-container + :global(h1),
    .close-button-container + :global(h2),
    .close-button-container + :global(h3),
    .close-button-container + :global(h4),
    .close-button-container + :global(h5),
    .close-button-container + :global(h6)
    {
        margin-top: 0px;
    }
</style>
