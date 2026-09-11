<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2025-2026
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
    import type { Bucket } from "$lib/models/s3";
    import { Template } from "$lib/renderables";
    import { Toasts } from "$lib/toasts.svelte";
    import { S3Service } from "$services/s3Service";

    let buckets = $state<Bucket[]>([]);
    let loading = $state(true);

    let createBucketModal: ModalComponent | undefined = $state();
    let newBucketName: string | null | undefined = $state();

    const viewBuckets = $derived(buckets.map(b => ({
        Name: new Template(link, { text: b.name, href: `/S3/${encodeURIComponent(b.name)}` }),
        "Creation Date": b.creationDate,
        Actions: new Template(actions, b),
    })));

    async function fetchBuckets() {
        loading = true;
        buckets = [];

        try {
            buckets = await S3Service.GetBucketsAsync();
        }
        catch(error) {
            Toasts.exception("Failed to fetch buckets", error);
        }

        loading = false;
    }

    function promptCreateBucket() {
        createBucketModal?.show();
    }

    function promptDeleteBucket(bucket: Bucket) {
        Modal.show(`Are you sure you want to delete "${bucket.name}"?`, "This cannot be undone!", [
            { text: "Yes", onclick: async (modal) => await onDeleteBucketModalYes(modal, bucket) },
            { text: "No",  onclick: (modal) => modal.close() }
        ], false);
    }

    async function createBucket(name: string) {
        try {
            await S3Service.CreateBucketAsync(name);
            Toasts.success(`Bucket "${name}" created successfully`, null, 4000);
            fetchBuckets();
        }
        catch(error) {
            Toasts.exception("Failed to create new bucket", error);
        }
    }

    async function deleteBucket(bucket: Bucket) {
        try {
            await S3Service.DeleteBucketAsync(bucket.name);
            Toasts.success(`Bucket "${bucket.name}" deleted successfully`, null, 4000);
            fetchBuckets();
        }
        catch(error) {
            Toasts.exception(`Failed to delete bucket "${bucket.name}"`, error);
        }
    }

    async function onCreateBucketModalOK(modal: ModalComponent) {
        if(!newBucketName)
            return;

        await createBucket(newBucketName);
        modal.close();
    }

    async function onDeleteBucketModalYes(modal: ModalComponent, bucket: Bucket) {
        await deleteBucket(bucket);
        modal.close();
    }

    fetchBuckets();
</script>

<svelte:head>
    <title>S3 Buckets - {ProjectName}</title>
</svelte:head>

<ModalComponent
    bind:this={createBucketModal}
    title="Create bucket"
    onclose={() => newBucketName = ""}
    buttons={[{ text: "OK", onclick: onCreateBucketModalOK, disabled: !newBucketName || newBucketName.length == 0 }]}
>
    <p class="text-center">
        Name: <input type="text" bind:value={newBucketName} />
    </p>
</ModalComponent>

<div class="flex items-center">
    <h1>S3 buckets</h1>
    <div class="grow"></div>
    <IconButton onclick={() => promptCreateBucket()} title="Create new bucket" icon="bi bi-bucket" class="neon-green" size="1.5em">
        <IconOverlay icon="bi bi-plus-circle-fill" class="text-black stroked" />
    </IconButton>
    <IconButton onclick={() => fetchBuckets()} title="Refresh" icon="fa-solid fa-arrows-rotate" class="neon-green" size="1.5em" />
</div>

{#if !loading}
    <Table content={viewBuckets} class="gs-list padded autosize-all-cols-but-first">
        No buckets to show.
    </Table>
{:else}
    <LoadingSpinner text="Loading..." centered={false} />
{/if}

{#snippet link(props: { text: string, href: string })}
    <a href={props.href}>{props.text}</a>
{/snippet}

{#snippet actions(bucket: Bucket)}
    <Icons>
        <IconButton onclick={() => promptDeleteBucket(bucket)} icon="fa-solid fa-trash-can" class="red" size={21} />
    </Icons>
{/snippet}
