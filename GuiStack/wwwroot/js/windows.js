//Thanks to: https://developer.mozilla.org/en-US/docs/Web/API/CustomEvent/CustomEvent#Polyfill
(function() {
    if(typeof window.CustomEvent === "function") return;

    function CustomEvent(event, params) {
        params = params || { bubbles: false, cancelable: false, detail: null };
        var evt = document.createEvent("CustomEvent");
        evt.initCustomEvent(event, params.bubbles, params.cancelable, params.detail);
        return evt;
    }

    window.CustomEvent = CustomEvent;
})();

/**
 * Opens the specified window. The element representing the window must have the "cssWindow" class applied.
 * @param {HTMLElement|string} element The element or the ID of the element representing the window to open.
 */
function showWindow(element)
{
    if(typeof element === "string") {
        var windows = document.getElementsByClassName("cssWindow");
        for(var i = 0; i < windows.length; i++) {
            if(windows[i].id == element) {
                showWindow(windows[i]);
                return;
            }
        }
        throw "'" + element + "' not found!";
    }
    else if(element === undefined || element === null || !(element instanceof Element) || !element.classList.contains("cssWindow")) {
        throw "'element' must be a HTMLElement with the 'cssWindow' class, or a string representing the ID of such an element!";
    }
    else if(element.id === undefined || element.id === null || element.id.length <= 0) {
        throw "'element' must have an ID!";
    }

    if(__isElementVisible(element)) {
        return;
    }

    var overlayDiv = document.createElement("div");
    overlayDiv.className = "cssWindowBackground" + (element.classList.contains("backdropblur") ? " backdropblur" : "");
    overlayDiv.id = "__cssWindow_" + element.id;
    overlayDiv.style.zIndex = __getElementZIndex(element) - 1;
    element.parentElement.appendChild(overlayDiv);

    //Doesn't work with nested windows.
    //document.body.appendChild(overlayDiv);

    element.style.display = "block";
    element.dispatchEvent(new CustomEvent("windowopened"));
}

/**
 * Closes the specified window. The element representing the window must have the "cssWindow" class applied.
 * @param {HTMLElement|string} element The element or the ID of the element representing the window to close.
 */
function closeWindow(element)
{
    if(typeof element === "string") {
        var windows = document.getElementsByClassName("cssWindow");
        for(var i = 0; i < windows.length; i++) {
            if(windows[i].id == element) {
                closeWindow(windows[i]);
                return;
            }
        }
        throw "'" + element + "' not found!";
    }
    else if(element === undefined || element === null || !(element instanceof Element) || !element.classList.contains("cssWindow")) {
        throw "'element' must be a HTMLElement with the 'cssWindow' class, or a string representing the ID of such an element!";
    }
    else if(element.id === undefined || element.id === null || element.id.length <= 0) {
        throw "'element' must have an ID!";
    }

    var overlay = document.getElementById("__cssWindow_" + element.id);
    if(overlay !== null) { overlay.parentElement.removeChild(overlay); }

    if(!__isElementVisible(element)) {
        return;
    }

    element.style.display = "none";
    element.dispatchEvent(new CustomEvent("windowclosed"));
}

/**
 * Closes the parent window of the clicked element.
 * @param {Event} event The event data from the onclick event.
 */
function closeParentWindow(event)
{
    if(event === undefined || event === null) {
        throw "'event' cannot be null!";
    }
    else if(!(event instanceof Event)) {
        throw "'event' must be an event type!";
    }

    var parent = event.currentTarget;
    while((parent = parent.parentElement) != null && !parent.classList.contains("cssWindow"));

    if(parent === null)
        throw "No parent window found!";

    closeWindow(parent);
}

/**
 * Gets the Z-index of the specified element as specified through CSS.
 * @param {HTMLElement} element The element which's Z-index to get.
 */
function __getElementZIndex(element)
{
    if(element === undefined || element === null || !(element instanceof Element)) {
        throw "'element' must be a valid HTMLElement!";
    }

    var z = null;

    if(element.style.zIndex === undefined || element.style.zIndex === null || isNaN(z = parseInt(element.style.zIndex))) {
        if(isNaN(z = parseInt(window.getComputedStyle(element).zIndex))) {
            z = 0;
        }
    }

    return z;
}

/**
 * Returns whether an element is visible or not.
 * @param {HTMLElement} element The element to check.
 */
function __isElementVisible(element)
{
    if(element === undefined || element === null || !(element instanceof Element)) {
        throw "'element' must be a valid HTMLElement!";
    }

    return element.style.display === undefined || element.style.display === null || element.style.display.length <= 0
        ? window.getComputedStyle(element).display.toLowerCase() != "none"
        : element.style.display.toLowerCase() != "none";
}