<!--
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.

    Copyright © Vincent Bengtsson & Contributors 2022-2026
    https://github.com/Visual-Vincent/GuiStack
-->
<script lang="ts">
    let { name, icon, url }: {
        name: string,
        url: string,
        icon: string,
    } = $props();
</script>

<a href={url}>
    <i class="image {icon}"></i>
    <p>{name}</p>
</a>

<style>
    a
    {
        display: flex;
        padding: 8px;
        background: #404040;
        box-shadow: #000000 0px 0px 4px 0px;
        border: 1px solid #585858;
        border-radius: 16px;
        text-align: center;
        align-items: center;
        align-content: center;
        justify-content: center;
        flex-wrap: wrap;
    }

    a, a:hover, a:active, a:visited
    {
        color: #FFAA00;
    }

    a:hover
    {
        background: #606060;
        border: 1px solid #787878;
    }

    a > *
    {
        flex: 1 1 100%;
    }

    a > .image
    {
        display: block;
        width: 32px;
        font-size: 32px;
    }

    a > p
    {
        margin-top: 4px;
        margin-bottom: 0px;
        font-weight: bold;
        line-height: initial;
    }

    a > i
    {
        /* Aligns Font Awesome icons properly */
        line-height: inherit;
    }
</style>
