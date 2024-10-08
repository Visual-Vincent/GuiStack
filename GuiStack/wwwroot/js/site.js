﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

const __gs_DynamicFunctions = {};

function isNull(obj)
{
    return obj === undefined || obj === null;
}

function isElement(obj)
{
    return obj instanceof HTMLElement;
}

function gs_DisplayError(message)
{
    document.getElementById("gs-error-banner").classList.add("visible");
    document.getElementById("gs-error-text").innerText = message;
}

function gs_CloseError()
{
    document.getElementById("gs-error-banner").classList.remove("visible");
}

function gs_BuildTemplate(template, parameters)
{
    if(isNull(template))
        throw "'template' cannot be null";

    if(isNull(parameters))
        throw "'parameters' cannot be null";

    if(typeof template !== "string")
        throw "'template' must be a string";

    if(!Array.isArray(parameters))
        throw "'parameters' must be an array";

    var result = template;

    for(var i in parameters)
    {
        var parameter = parameters[i];

        // No sanitization for *_HTML parameters
        if(parameter.name.endsWith("_HTML"))
        {
            result = result.replaceAll(`{{${parameter.name}}}`, parameter.value ?? "");
            continue;
        }
        
        var value = !isNull(parameter.value)
            ? gs_SanitizeHtml(parameter.value)
            : "";

        result = result.replaceAll(`{{${parameter.name}}}`, value);
    }

    return result + "\n";
}

function gs_SanitizeHtml(value)
{
    if(isNull(value))
        return null;

    value = value.toString();

    // Crude sanitization
    return value
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&apos;");
}

function gs_GetParentTable(element, throwNotFound)
{
    if(typeof element === "string")
    {
        var id = element;
        element = document.getElementById(id);

        if(isNull(element))
            throw "Element '" + id + "' not found";
    }
    else if(isNull(element))
    {
        throw "'element' cannot be null";
    }
    else if(!isElement(element))
    {
        throw "'element' must be an HTMLElement";
    }

    var parent = element;
    while(!isNull(parent = parent.parentElement) && !(parent instanceof HTMLTableElement));

    if(isNull(parent))
        if(throwNotFound)
            throw "No parent table element found";
        else
            return null;

    return parent;
}

function gs_GetParentTableRow(element, throwNotFound)
{
    if(typeof element === "string")
    {
        var id = element;
        element = document.getElementById(id);

        if(isNull(element))
            throw "Element '" + id + "' not found";
    }
    else if(isNull(element))
    {
        throw "'element' cannot be null";
    }
    else if(!isElement(element))
    {
        throw "'element' must be an HTMLElement";
    }

    var parent = element;
    while(!isNull(parent = parent.parentElement) && !(parent instanceof HTMLTableRowElement));

    if(isNull(parent))
        if(throwNotFound)
            throw "No parent table row element found";
        else
            return null;

    return parent;
}

function gs_GetParentTabContainer(element, throwNotFound)
{
    var parent = gs_FindParent(element, ".gs-tab-container", false);

    if(isNull(parent) && throwNotFound)
        throw "No parent tab container found";

    return parent;
}

function gs_GetParentTabControl(element, throwNotFound)
{
    var parent = gs_FindParent(element, ".gs-tab-control", false);

    if(isNull(parent) && throwNotFound)
        throw "No parent tab control found";

    return parent;
}

function gs_GetParentTabPage(element, throwNotFound)
{
    var parent = gs_FindParent(element, ".gs-tabpage", false);

    if(isNull(parent) && throwNotFound)
        throw "No parent tab page found";

    return parent;
}

function gs_FindParent(element, selector, throwNotFound)
{
    if(typeof element === "string")
    {
        var id = element;
        element = document.getElementById(id);

        if(isNull(element))
            throw "Element '" + id + "' not found";
    }
    else if(isNull(element))
    {
        throw "'element' cannot be null";
    }
    else if(!isElement(element))
    {
        throw "'element' must be an HTMLElement";
    }

    if(isNull(selector) || selector === "")
        return element.parentElement;

    if(typeof selector !== "string")
        throw "'selector' must be a string";

    var parent = element;
    while(!isNull(parent = parent.parentElement) && !parent.matches(selector));

    if(isNull(parent))
        if(throwNotFound)
            throw "No parent element found that matches selector '" + selector + "'";
        else
            return null;

    return parent;
}

function gs_GetFieldEditorName(element)
{
    if(isNull(element))
        throw "'element' cannot be null";

    if(!isElement(element) || !(element instanceof HTMLTableRowElement))
        throw "'element' must be a <tr> element";

    var type = element.getAttribute("data-type");

    if(isNull(type))
        return null;

    var nameColumn = element.querySelector(":scope > td.name-column");

    if(isNull(nameColumn))
        throw "Element is not a valid Field Editor field: Missing name column";

    var nameElement = nameColumn.querySelector(`:scope > input[type="text"]`);

    if(isNull(nameElement))
        throw "Element is not a valid Field Editor field: Missing name input field";

    return nameElement.value;
}

function gs_GetFieldEditorValue(element)
{
    if(isNull(element))
        throw "'element' cannot be null";

    if(!isElement(element) || !(element instanceof HTMLTableRowElement))
        throw "'element' must be a <tr> element";

    var type = element.getAttribute("data-type");

    if(isNull(type))
        return null;

    var valueColumn = element.querySelector(":scope > td.value-column");

    if(isNull(valueColumn))
        throw "Element is not a valid Field Editor field: Missing value column";

    var valueElement = null;

    switch(type)
    {
        case "String":
            valueElement = valueColumn.querySelector(`:scope > input[type="text"]`) ?? valueColumn.querySelector(":scope > textarea");

            if(isNull(valueElement))
                throw "Element is not a valid Field Editor field: Missing value input field";

            return valueElement.value;
        
        // TODO: Add support for more types
    }

    throw "Failed to get field value: Unknown data type '" + type + "'";
}

function gs_GetProtobufMessage(path)
{
    if(isNull(path))
        throw "'path' cannot be null";

    if(typeof path !== "string")
        throw "'path' must be a string";

    if(!path.startsWith("proto/"))
        throw "'" + path + "' is not a valid protobuf definition path";

    return localStorage.getItem(path);
}

function gs_GetProtobufRootDefinition(path)
{
    if(isNull(path))
        throw "'path' cannot be null";

    if(typeof path !== "string")
        throw "'path' must be a string";

    if(!path.startsWith("proto/"))
        throw "'" + path + "' is not a valid protobuf definition path";

    root = path.split("/")[1];

    if(root.length <= 0)
        return null;

    return localStorage.getItem("protoroot/" + root);
}

function gs_GetSelectedTableItem(element)
{
    if(isNull(element))
        throw "'element' cannot be null";

    if(!isElement(element) || (!(element instanceof HTMLDivElement) && !(element instanceof HTMLTableElement)))
        throw "'element' must be a <table> element or a <div> element containing a <table class=\"gs-selector-table\"> element";

    if(element instanceof HTMLDivElement)
    {
        element = element.querySelector(":scope > table.gs-selector-table");

        if(isNull(element))
            throw "'element' must contain a <table class=\"gs-selector-table\"> element";
    }

    var oldElement = element;
    element = element.querySelector(":scope > tbody");

    if(isNull(element))
        element = oldElement;

    return element.querySelector(":scope > tr.selected");
}

function gs_RefreshProtobufList(element)
{
    if(isNull(element))
        throw "'element' cannot be null";

    if(!isElement(element) || !(element instanceof HTMLUListElement))
        throw "'element' must be an <ul> element";

    if(
        !element.classList.contains("gs-tree") || 
        !element.classList.contains("gs-protobuf-list") ||
        !element.hasAttribute("data-proto-list-id") ||
        element.getAttribute("data-proto-list-id").length <= 0
    )
        throw "The specified element is not a valid protobuf definitions list (id=" + element.id + ")";

    __gs_DynamicFunctions["gs_RefreshProtobufList_" + element.getAttribute("data-proto-list-id")]();
}

function gs_SelectOption(selectElement, option)
{
    if(isNull(selectElement))
        throw "'selectElement' cannot be null";

    if(isElement(selectElement))
    {
        if(!(selectElement instanceof HTMLSelectElement))
            throw "'selectElement' must be a <select> element";

        selectElement.value = option;
        selectElement.dispatchEvent(new Event("change"));
    }
    else if(selectElement instanceof jQuery || selectElement instanceof $ || selectElement.jquery)
    {
        if(!selectElement.is("select"))
            throw "'selectElement' must be a <select> element";

        selectElement.val(option);
        selectElement.trigger("click");
    }
    else
    {
        throw "'selectElement' must be a <select> element";
    }
}

function gsevent_InfoTable_ToggleButton_Click(event)
{
    var table = gs_GetParentTable(event.currentTarget, true);
    table.classList.toggle("expanded");
}

function gsevent_Expander_Click(event)
{
    var expander = event.currentTarget.parentElement;
    expander.classList.toggle("expanded");
}

function gsevent_TableItem_Click(event)
{
    var table = gs_GetParentTable(event.currentTarget, true);
    var body = table.querySelector(":scope > tbody");
    var element = table;

    if(!isNull(body))
        element = body;

    var items = element.querySelectorAll(":scope > tr");

    for(var i = 0; i < items.length; i++)
    {
        var item = items[i];

        if(event.currentTarget == item)
            item.classList.add("selected")
        else
            item.classList.remove("selected");
    }
}

function gsevent_TabItem_Click(event)
{
    var tabpageId = event.currentTarget.getAttribute("data-tabpage");
    var tabcontrol = gs_GetParentTabControl(event.currentTarget, true);
    var tabcontainer = gs_GetParentTabContainer(event.currentTarget, true);

    if(isNull(tabpageId) || tabpageId.length <= 0)
        throw "No tabpage specified";

    var tabpage = document.getElementById(tabpageId);

    if(isNull(tabpage))
        throw "Element with id '" + tabpageId + "' not found";

    var tabitems = tabcontainer.querySelectorAll(".gs-tabitem");
    for(var i = 0; i < tabitems.length; i++)
    {
        tabitems[i].classList.remove("selected");
    }

    var tabpages = tabcontrol.querySelectorAll(".gs-tabpage");
    for(var i = 0; i < tabpages.length; i++)
    {
        tabpages[i].classList.remove("selected");
    }

    tabpage.classList.add("selected");
    event.currentTarget.classList.add("selected");
}

function gsevent_TreeNode_Click(event)
{
    if(event.currentTarget !== event.target)
        return;

    event.currentTarget.classList.toggle("expanded");
}

function gsevent_AjaxError(request, status, errorThrown)
{
    if(!isNull(request) && !isNull(request.responseText))
    {
        if(!(request.getResponseHeader("Content-Type") || "").includes("application/json"))
        {
            gs_DisplayError(request.responseText || "Server returned HTTP status " + request.status);
            return;
        }

        var obj = JSON.parse(request.responseText);

        if(!isNull(obj.error))
            gs_DisplayError(obj.error);
        else
            gs_DisplayError(request.responseText);
        
        return;
    }

    if(!isNull(errorThrown) && errorThrown !== "")
        gs_DisplayError(errorThrown);
    else
        gs_DisplayError("An unknown error occurred");
}

async function gsevent_FetchError(response)
{
    if(isNull(response))
        gs_DisplayError("An unknown error occurred");

    var responseText = await response.text();

    if(isNull(responseText) || responseText == "")
        gs_DisplayError("Server returned HTTP status " + response.status);

    if(!(response.headers.get("Content-Type") || "").includes("application/json"))
    {
        gs_DisplayError("Server returned HTTP status " + response.status + " with contents: " + responseText);
        return;
    }

    var obj = JSON.parse(responseText);

    if(!isNull(obj.error))
        gs_DisplayError(obj.error);
    else
        gs_DisplayError("Server returned HTTP status " + response.status + " with contents: " + responseText);
}

function __gs_RefreshTreeHandlers()
{
    $("ul.gs-tree > li[data-title]").unbind("click", gsevent_TreeNode_Click);
    $("ul.gs-tree > li[data-title]").click(gsevent_TreeNode_Click);
}

$(document).ready(function() {
    var navs = document.querySelectorAll(".gs-navbar > a");
    var currentUrl = window.location.pathname.split("?")[0];

    for(var i = 0; i < navs.length; i++)
    {
        var nav = navs[i];
        var selectionRegex = nav.getAttribute("data-selected-regex");

        if(isNull(selectionRegex) || selectionRegex.length <= 0)
            continue;

        var regex = new RegExp(selectionRegex);

        if(regex.test(currentUrl))
        {
            nav.classList.add("selected");
            break;
        }
    }

    __gs_RefreshTreeHandlers();
    $(".gs-tab-control > .gs-tab-container > .gs-tabitem").click(gsevent_TabItem_Click);

    var __gs_bodyDragCounter = 0;

    $("body").on("dragenter", function() {
        if(__gs_bodyDragCounter <= 0)
        {
            document.querySelector("body").classList.add("is-dragging");
            $(".gs-drop-container .gs-upload-field").css("position", "static");

            __gs_bodyDragCounter = 0;
        }

        __gs_bodyDragCounter++;
    });

    $("body").on("dragleave dragend drop", function() {
        __gs_bodyDragCounter--;

        if(__gs_bodyDragCounter <= 0)
        {
            document.querySelector("body").classList.remove("is-dragging");
            $(".gs-drop-container .gs-upload-field").css("position", "");

            __gs_bodyDragCounter = 0;
        }
    });

    $(window).on("blur", function() {
        document.querySelector("body").classList.remove("is-dragging");
        $(".gs-drop-container .gs-upload-field").css("position", "");

        __gs_bodyDragCounter = 0;
    });

    $("table.gs-selector-table > tr").click(gsevent_TableItem_Click);
    $("table.gs-selector-table > tbody > tr").click(gsevent_TableItem_Click);
    $(".gs-expander > .gs-expander-header").click(gsevent_Expander_Click)
});

/*
    The code for the "gs_Uint8ArrayToBase64" function below was originally written
    by Jon Leighton, and can be found at: https://gist.github.com/jonleighton/958841

    This solves an issue I've been facing for many years in JavaScript.
    Thank you, Jon, for creating this function!
    - Visual Vincent, 2022-06-16


    The following snippet of code is licensed under the MIT license:

        Copyright (c) 2011 Jon Leighton

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
*/

function gs_Uint8ArrayToBase64(bytes)
{
    var base64    = ''
    var encodings = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'

    var byteLength    = bytes.byteLength
    var byteRemainder = byteLength % 3
    var mainLength    = byteLength - byteRemainder

    var a, b, c, d
    var chunk

    // Main loop deals with bytes in chunks of 3
    for (var i = 0; i < mainLength; i = i + 3)
    {
        // Combine the three bytes into a single integer
        chunk = (bytes[i] << 16) | (bytes[i + 1] << 8) | bytes[i + 2]

        // Use bitmasks to extract 6-bit segments from the triplet
        a = (chunk & 16515072) >> 18 // 16515072 = (2^6 - 1) << 18
        b = (chunk & 258048)   >> 12 // 258048   = (2^6 - 1) << 12
        c = (chunk & 4032)     >>  6 // 4032     = (2^6 - 1) << 6
        d = chunk & 63               // 63       = 2^6 - 1

        // Convert the raw binary segments to the appropriate ASCII encoding
        base64 += encodings[a] + encodings[b] + encodings[c] + encodings[d]
    }
  
    // Deal with the remaining bytes and padding
    if (byteRemainder == 1)
    {
        chunk = bytes[mainLength]

        a = (chunk & 252) >> 2 // 252 = (2^6 - 1) << 2

        // Set the 4 least significant bits to zero
        b = (chunk & 3)   << 4 // 3   = 2^2 - 1

        base64 += encodings[a] + encodings[b] + '=='
    }
    else if (byteRemainder == 2)
    {
        chunk = (bytes[mainLength] << 8) | bytes[mainLength + 1]

        a = (chunk & 64512) >> 10 // 64512 = (2^6 - 1) << 10
        b = (chunk & 1008)  >>  4 // 1008  = (2^6 - 1) << 4

        // Set the 2 least significant bits to zero
        c = (chunk & 15)    <<  2 // 15    = 2^4 - 1

        base64 += encodings[a] + encodings[b] + encodings[c] + '='
    }

    return base64
}