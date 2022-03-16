﻿function isNull(obj)
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
    while(!isNull(parent = parent.parentElement) && !parent.classList.contains("gs-tab-container"));

    if(isNull(parent))
        if(throwNotFound)
            throw "No parent tab container found";
        else
            return null;

    return parent;
}

function gs_GetParentTabControl(element, throwNotFound)
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
    while(!isNull(parent = parent.parentElement) && !parent.classList.contains("gs-tab-control"));

    if(isNull(parent))
        if(throwNotFound)
            throw "No parent tab control found";
        else
            return null;

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

function gsevent_InfoTable_ToggleButton_Click(event)
{
    var table = gs_GetParentTable(event.currentTarget, true);
    table.classList.toggle("expanded");
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

(function() {
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

    $(".gs-tab-control > .gs-tab-container > .gs-tabitem").click(gsevent_TabItem_Click);
})();