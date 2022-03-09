function isNull(obj)
{
    return obj === undefined || obj === null;
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