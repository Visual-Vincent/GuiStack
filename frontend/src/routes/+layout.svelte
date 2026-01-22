<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2022-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    import "./layout.css";
    import favicon from "$lib/assets/favicon.svg";
	import { page } from "$app/state";
    import { ProjectName, TopLevelPages } from "$lib";
    import ToastHost from "$components/ToastHost.svelte";
    import LoadingOverlayComponent from "$components/LoadingOverlay.svelte";
    import { LoadingOverlay } from "$lib/overlay.svelte";
    import Renderable from "$components/Renderable.svelte";

    let { children } = $props();

    const toastIcons = {
        info: "fa-solid fa-circle-info",
        success: "fa-solid fa-circle-check",
        warning: "fa-solid fa-triangle-exclamation",
        error: "fa-solid fa-circle-xmark"
    };

    let bodyDragCounter = 0;

    function onDragEnter() {
        if(bodyDragCounter <= 0) {
            document.querySelector("body")!.classList.add("is-dragging");
            bodyDragCounter = 0;
        }

        bodyDragCounter++;
    }

    function onDragStop() {
        bodyDragCounter--;

        if(bodyDragCounter <= 0)
            document.querySelector("body")!.classList.remove("is-dragging");
    }

    function window_lostFocus() {
        document.querySelector("body")!.classList.remove("is-dragging");
        bodyDragCounter = 0;
    }
</script>

<svelte:window onblur={window_lostFocus} />

<svelte:head>
    <link rel="icon" href={favicon} />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" integrity="sha512-9usAa10IRO0HhonpyAIVpjrylPvoDwiPUiKdWk5t3PyolY1cOd4DSE0Ga+ri4AuTroPR5aQvXU9xC6qOPnzFeg==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.8.1/font/bootstrap-icons.css">
</svelte:head>

<svelte:body
    ondragenter={onDragEnter}
    ondragleave={onDragStop}
    ondragend={onDragStop}
    ondrop={onDragStop}
/>

<LoadingOverlayComponent
    spinner={LoadingOverlay.settings.spinner}
    spinnerRight={LoadingOverlay.settings.spinnerRight}
    visible={LoadingOverlay.settings.visible}
>
    <Renderable content={LoadingOverlay.contents ?? "Loading..."} />
</LoadingOverlayComponent>

<div class="toasts">
    <ToastHost icons={toastIcons} />
</div>

<div class="main-container">
    <header>
        <h1 class="text-center">{ProjectName}</h1>
    </header>

    <nav>
        {#each TopLevelPages as item}
            <a href={item.url} class={item.selectedRegex && item.selectedRegex.test(page.url.pathname) ? "selected" : ""}>{item.navMenuName}</a>
        {/each}
    </nav>

    <main>
        {@render children()}
    </main>
</div>

<style>
    .toasts,
    .main-container
    {
        width: 1280px;
        margin: auto;
    }

    .main-container
    {
        display: flex;
        flex-wrap: wrap;
        border: 1px solid rgba(0, 0, 0, 0.25);
        border-radius: 8px;
        box-shadow: 0px 0px 2px 1px rgba(255, 255, 255, 0.1);
        overflow: hidden;
        color: #FFFFFF;
    }

    .main-container > header
    {
        background: #000000;
        background: linear-gradient(0deg, rgb(32, 32, 32), rgb(100, 100, 100));
        flex: 0 0 100%;
    }

    .main-container > main
    {
        background: #303030;
        flex: 1;
        padding: 16px;
        border-top: 1px solid rgba(255, 255, 255, 0.01);
        box-sizing: border-box;
        box-shadow: inset 0px 3px 5px rgba(0, 0, 0, 0.6);
        overflow: auto;
    }

    .main-container > nav
    {
        display: block;
        background: #35363A;
        width: 20%;
    }

    .main-container > nav > a
    {
        display: block;
        background: linear-gradient(0deg, #373737, #52555A);
        border-top: 1px solid rgba(255, 255, 255, 0.3);
        border-bottom: 1px solid rgba(0, 0, 0, 0.8);
        padding: 8px 10px;
        color: #FFFFFF;
        text-decoration: none;
    }

    .main-container > nav > a.selected
    {
        background: linear-gradient(180deg, #303030, #4d5054);
        box-shadow: inset rgba(0, 0, 0, 0.69) 0px 0px 4px 0px;
        border-top: 1px solid rgba(0, 0, 0, 0.69);
        font-weight: bold;
        color: #00FF55;
    }

    .main-container > nav > a:hover
    {
        background: #4d5054;
    }
</style>
