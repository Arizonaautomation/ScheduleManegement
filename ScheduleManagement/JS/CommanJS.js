
// Only Alpha Value Function
function onlyAlphaValue() {

    if ((event.keyCode > 64 && event.keyCode < 91) || (event.keyCode > 96 && event.keyCode < 123) || event.keyCode == 32) {
        return true;
    }
    else {
        return false;
    }
}

/*Only Numbers Function*/
function validateNumericValue(key) {
    var keycode = (key.which) ? key.which : key.keyCode;

    if (!(keycode == 8) && (keycode < 48 || keycode > 57)) {
        return false;
    }
    else {

        return true;
    }
}

//AlphaNumeric value Function
function checkAlphaNumeric() {

    if ((event.keyCode > 64 && event.keyCode < 91) || (event.keyCode > 96 && event.keyCode < 123) || (event.keyCode > 47 && event.keyCode < 58) || event.keyCode == 32) {
        return true;
    }
    else {
        return false;
    }
}

$("input").on("keypress", function (e) {
    if (e.which === 32 && !this.value.length)/*&& e.which === 48*/
        e.preventDefault();
});