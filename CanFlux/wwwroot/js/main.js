function getDivCanvasOffsets(el) {
    var rect = el.getBoundingClientRect();
    var obj = {};
    obj.offsetLeft = rect.left - document.body.scrollLeft;
    obj.offsetTop = rect.top - document.body.scrollTop;
    return JSON.stringify(obj);
}