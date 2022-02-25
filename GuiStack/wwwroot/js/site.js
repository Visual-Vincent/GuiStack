function gs_DisplayError(message)
{
    document.getElementById("gs-error-banner").classList.add("visible");
    document.getElementById("gs-error-text").innerText = message;
}

function gs_CloseError()
{
    document.getElementById("gs-error-banner").classList.remove("visible");
}