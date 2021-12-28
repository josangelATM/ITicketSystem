if (matchMedia) {
    const mq = window.matchMedia("(min-width: 500px)");
    mq.addListener(WidthChange);
    WidthChange(mq);
}

// media query change
function WidthChange(mq) {
    if (mq.matches) {
        // window width is at least 500px
        
    } else {
        console.log("WTF?")
        $("#body").append(`
        )
    }

}