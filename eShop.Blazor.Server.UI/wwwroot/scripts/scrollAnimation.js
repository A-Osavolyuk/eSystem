function slideLeft(step, elementId) {
    const slider = document.getElementById(elementId);
    slider.scrollBy({ left: -step, top: 0, behavior: 'smooth' });
    return isOnLeft();
}

function slideRight(step, elementId) {
    const slider = document.getElementById(elementId);
    slider.scrollBy({ left: step, top: 0, behavior: 'smooth' });
    return isOnRight();
}

function slideTop(step, elementId) {
    const slider = document.getElementById(elementId);
    slider.scrollBy({ left: 0, top: -step, behavior: 'smooth' })
    return isOnTop();
}

function slideBottom(step, elementId) {
    const slider = document.getElementById(elementId);
    slider.scrollBy({ left: 0, top: step, behavior: 'smooth' })
    return isOnBottom();
}

function isOnLeft() {
    const slider = document.getElementById("slider");
    const overflowedWidth = slider.scrollWidth - slider.clientWidth;
    return slider.scrollLeft - overflowedWidth === 0;
}

function isOnRight() {
    const slider = document.getElementById("slider");
    const overflowedWidth = slider.scrollWidth - slider.clientWidth;
    return slider.scrollLeft + slider.clientWidth >= slider.scrollWidth - overflowedWidth;
}

function isOnBottom() {
    const slider = document.getElementById("slider");
    const overflowedHeight = slider.scrollHeight - slider.clientHeight
    return slider.scrollTop + slider.clientHeight >= slider.scrollHeight - overflowedHeight;
}

function isOnTop() {
    const slider = document.getElementById("slider");
    const overflowedHeight = slider.scrollHeight - slider.clientHeight
    return slider.scrollTop - overflowedHeight === 0;
}

function some() {
    let element = document.getElementById("slider");
    let height = element.role;
    console.log(element.role)
}