<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import IconButton from "$components/IconButton.svelte";
    import IconOverlay from "$components/IconOverlay.svelte";
    import Icons from "$components/Icons.svelte";
    import LoadingSpinner from "$components/LoadingSpinner.svelte";
    import ModalComponent from "$components/Modal.svelte";
    import Table from "$components/Table.svelte";
    import { ProjectName } from "$lib";
    import { Modal } from "$lib/modals";
    import type { Queue } from "$lib/models/sqs";
    import { Template } from "$lib/renderables";
    import { Toasts } from "$lib/toasts.svelte";
    import { SQSService } from "$services/sqsService";

    let queues = $state<Queue[]>([]);
    let loading = $state(true);

    let createQueueModal: ModalComponent | undefined = $state();
    let newQueueName: string | null | undefined = $state();
    let newQueueFifo = $state(false);

    const viewQueues = $derived(queues.map(q => ({
        Name: new Template(link, { text: q.name, href: `/SQS/${encodeURIComponent(q.name)}` }),
        Actions: new Template(actions, q)
    })));

    async function fetchQueues() {
        loading = true;

        try {
            queues = await SQSService.GetQueuesAsync();
        }
        catch(error) {
            Toasts.exception("Failed to fetch queues", error);
        }

        loading = false;
    }

    async function createQueue(name: string, isFifo: boolean) {
        try {
            await SQSService.CreateQueueAsync(name, isFifo);
            Toasts.success(`SQS queue "${name}" created successfully`, null, 4000);
            fetchQueues();
        }
        catch(error) {
            Toasts.exception("Failed to create new SQS queue", error);
        }
    }

    async function deleteQueue(queue: Queue) {
        try {
            console.log(queue);
            await SQSService.DeleteQueueAsync(queue.url)
            Toasts.success(`Queue "${queue.name}" deleted successfully`, null, 4000);
            fetchQueues();
        }
        catch(error) {
            Toasts.exception(`Failed to delete SQS queue "${queue.name}"`, error);
        }
    }

    function promptCreateQueue() {
        createQueueModal?.show();
    }

    function promptDeleteQueue(queue: Queue) {
        Modal.show(`Are you sure you want to delete "${queue.name}"?`, "This cannot be undone!", [
            { text: "Yes", onclick: async (modal) => await onDeleteQueueModalYes(modal, queue) },
            { text: "No",  onclick: (modal) => modal.close() }
        ], false);
    }

    async function onCreateQueueModalClose() {
        newQueueName = "";
        newQueueFifo = false;
    }

    async function onCreateQueueModalOK(modal: ModalComponent) {
        if(!newQueueName)
            return;

        await createQueue(newQueueName, newQueueFifo);
        modal.close();
    }

    async function onDeleteQueueModalYes(modal: ModalComponent, queue: Queue) {
        await deleteQueue(queue);
        modal.close();
    }

    fetchQueues();
</script>

<svelte:head>
    <title>SQS Queues - {ProjectName}</title>
</svelte:head>

<ModalComponent
    bind:this={createQueueModal}
    title="Create SQS queue"
    onclose={onCreateQueueModalClose}
    buttons={[{ text: "OK", onclick: onCreateQueueModalOK, disabled: !newQueueName || newQueueName.length == 0 }]}
>
    <p class="text-center">
        Name: <input type="text" bind:value={newQueueName} />
    </p>
    <p class="text-center">
        <input type="checkbox" id="sqs-queue-fifo-checkbox" bind:checked={newQueueFifo} />
        <label for="sqs-queue-fifo-checkbox">FIFO</label>
    </p>
</ModalComponent>

<div class="flex items-center">
    <h1>SQS Queues</h1>
    <div class="grow"></div>
    <IconButton onclick={() => promptCreateQueue()} title="Create new queue" icon="fa-solid fa-database -rotate-90 mr-[4px]" class="neon-green" size="1.5em">
        <IconOverlay icon="bi bi-plus-circle-fill" class="text-black stroked" />
    </IconButton>
    <IconButton onclick={() => fetchQueues()} title="Refresh" icon="fa-solid fa-arrows-rotate" class="neon-green" size="1.5em" />
</div>

{#if !loading}
    <Table content={viewQueues} class="gs-list padded autosize-all-cols-but-first">
        No queues to show.
    </Table>
{:else}
    <LoadingSpinner text="Loading..." centered={false} />
{/if}

{#snippet link(props: { text: string, href: string })}
    <a href={props.href}>{props.text}</a>
{/snippet}

{#snippet actions(queue: Queue)}
    <Icons>
        <IconButton onclick={() => promptDeleteQueue(queue)} icon="fa-solid fa-trash-can" class="red" size={21} />
    </Icons>
{/snippet}
