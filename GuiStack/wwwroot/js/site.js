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

function gs_GetParentTable(element)
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
        return null;

    return parent;
}

function gsevent_InfoTable_ToggleButton_Click(event)
{
    var table = gs_GetParentTable(event.currentTarget);

    if(isNull(table))
        throw "No parent table element found";

    table.classList.toggle("expanded");
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
})();