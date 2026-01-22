<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2025-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import { page } from "$app/state";
    import DropContainer from "$components/DropContainer.svelte";
    import FileDropZone from "$components/FileDropZone.svelte";
    import FileUpload from "$components/FileUpload.svelte";
    import IconButton from "$components/IconButton.svelte";
    import Icons from "$components/Icons.svelte";
    import LoadingSpinner from "$components/LoadingSpinner.svelte";
    import ModalComponent from "$components/Modal.svelte";
    import Table from "$components/Table.svelte";
    import { ProjectName } from "$lib";
    import { FormatFileSize } from "$lib/extensions";
    import { Modal } from "$lib/modals";
    import type { Object } from "$lib/models/s3";
    import { LoadingOverlay } from "$lib/overlay.svelte";
    import { Template } from "$lib/renderables";
    import { Toasts } from "$lib/toasts.svelte";
    import { S3Service } from "$services/s3Service";

    const bucketName = page.params.bucket;

    let objects = $state<Object[]>([]);
    let loading = $state(true);
    let uploadProgress = $state(0);
    let uploadDisabled = $state(false);

    let renameObjectModal: ModalComponent | undefined = $state();
    let newObjectName: string | null | undefined = $state();
    let editedObject: Object | null | undefined;

    const viewObjects = $derived(objects.map(o => ({
        Name: new Template(link, { text: o.name, href: S3Service.GenerateDownloadUrl(bucketName!, o.name) }),
        Size: FormatFileSize(o.size),
        "Last Modified": o.lastModified,
        Actions: new Template(actions, o),
    })));

    async function fetchObjects() {
        loading = true;
        objects = [];

        if(!bucketName) {
            loading = false;
            return;
        }

        try {
            objects = await S3Service.GetObjectsAsync(bucketName);
        }
        catch(error) {
            Toasts.exception("Failed to fetch objects", error as string);
        }

        loading = false;
    }

    function copyS3Uri(object: Object) {
        navigator.clipboard.writeText(object.s3Uri);
        Toasts.info(`S3 URI of "${object.name}" copied to clipboard`, null, 4000);
    }

    function copyUrl(object: Object) {
        navigator.clipboard.writeText(object.url);
        Toasts.info(`URL of "${object.name}" copied to clipboard`, null, 4000);
    }

    async function deleteObject(object: Object) {
        try {
            await S3Service.DeleteObjectAsync(bucketName!, object.name);
            Toasts.success(`Object "${object.name}" deleted successfully`, null, 4000);
            fetchObjects();
        }
        catch (error) {
            Toasts.exception(`Failed to delete object "${object.name}"`, error);
        }
    }

    async function renameObject(object: Object, newName: string) {
        try {
            await S3Service.RenameObjectAsync(bucketName!, object.name, newName);
            Toasts.success(`Object "${object.name}" successfully renamed to "${newName}"`, null, 4000);
            fetchObjects();
        }
        catch (error) {
            Toasts.exception(`Failed to rename object "${object.name}"`, error);
        }
    }

    function promptDeleteObject(object: Object) {
        Modal.show(`Are you sure you want to delete "${object.name}"?`, "This cannot be undone!", [
            { text: "Yes", onclick: async (modal) => await onDeleteObjectModalYes(modal, object) },
            { text: "No",  onclick: (modal) => modal.close() }
        ], false);
    }

    function promptRenameObject(object: Object) {
        editedObject = object;
        newObjectName = object.name;
        renameObjectModal?.show();
    }

    async function onDeleteObjectModalYes(modal: ModalComponent, object: Object) {
        await deleteObject(object);
        modal.close();
    }

    async function onRenameObjectModalOK(modal: ModalComponent) {
        if(!editedObject || !newObjectName)
            return;

        await renameObject(editedObject, newObjectName);
        modal.close();
    }

    async function onUploadFile(files: File[]) {
        if(files.length <= 0)
            return;

        try {
            uploadProgress = 0;
            uploadDisabled = true;

            LoadingOverlay.show(new Template(fileUploadProgress, { fileName: files[0].name }));

            await S3Service.UploadObjectAsync(bucketName!, files[0], (uploaded, total) => {
                uploadProgress = Math.round(uploaded * 100.0 / total);
            });

            fetchObjects();
        }
        catch(error) {
            Toasts.exception(`Failed to upload files to S3 bucket`, error);
        }
        finally {
            uploadDisabled = false;
            LoadingOverlay.hide();
        }
    }

    fetchObjects();
</script>

<svelte:head>
    <title>S3 Bucket - {bucketName} - {ProjectName}</title>
</svelte:head>

<ModalComponent
    bind:this={renameObjectModal}
    title="Rename object"
    onclose={() => {
        newObjectName = "";
        editedObject = null;
    }}
    buttons={[{ text: "OK", onclick: onRenameObjectModalOK, disabled: !newObjectName || newObjectName.length == 0 }]}
>
    <p class="text-center">
        Name: <input type="text" bind:value={newObjectName} />
    </p>
</ModalComponent>

<a href="/S3">&larr; Back</a>

<div class="flex items-center pb-[1em]">
    <div>
        <h1>{bucketName}</h1>
        <p class="gs-object-type">S3 Bucket</p>
    </div>
    <div class="grow"></div>
    <FileUpload class="borderless initial-white neon-green text-[1.5em]" style="padding: 2px 4px" onfilesselected={onUploadFile} disabled={uploadDisabled}>
        <i class="fa-solid fa-arrow-up-from-bracket block!"></i>
    </FileUpload>
    <IconButton onclick={() => fetchObjects()} title="Refresh" icon="fa-solid fa-arrows-rotate" class="neon-green" size="1.5em" />
</div>

{#if !loading}
    <DropContainer>
        <Table content={viewObjects} class="gs-list padded autosize-all-cols-but-first">
            This bucket is empty.
            <FileUpload class="inline-block! ml-2" onfilesselected={onUploadFile} disabled={uploadDisabled}>
                <i class="fa-solid fa-arrow-up-from-bracket mr-2"></i>Upload file
            </FileUpload>
        </Table>
        <FileDropZone onfilesselected={onUploadFile}>Drop file(s) to upload to S3 bucket</FileDropZone>
    </DropContainer>
{:else}
    <LoadingSpinner text="Loading..." centered={false} />
{/if}

{#snippet link(props: { text: string, href: string })}
    <a href={props.href}>{props.text}</a>
{/snippet}

{#snippet actions(object: Object)}
    <Icons>
        <IconButton onclick={() => promptRenameObject(object)} title="Rename" icon="fa-solid fa-i-cursor" class="blue" size={21} />
        <IconButton onclick={() => copyS3Uri(object)} title="Copy S3 URI" icon="fa-solid fa-link" class="purple" size={21} />
        <IconButton onclick={() => copyUrl(object)} title="Copy URL" icon="bi bi-globe" class="green" size={21} />
        <IconButton onclick={() => promptDeleteObject(object)} title="Delete" icon="fa-solid fa-trash-can" class="red" size={21} />
    </Icons>
{/snippet}

{#snippet fileUploadProgress(props: { fileName: string })}
    Uploading&nbsp;<span style="color: #00FFAA">{props.fileName}</span>...&nbsp;{Math.round(uploadProgress)}%
{/snippet}

<style>
    .gs-object-type
    {
        margin-bottom: 0px;
    }
</style>
