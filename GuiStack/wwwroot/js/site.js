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
            gs_DisplayError(request.responseText);
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
    $("ul.gs-tree > li[data-title]").click(gsevent_TreeNode_Click);

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
})();