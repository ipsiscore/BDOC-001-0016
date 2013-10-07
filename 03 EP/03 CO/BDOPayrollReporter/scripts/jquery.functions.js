function validate(n) {
    var len = n.toString().length;
    if (len == 1)
        return '0' + n;
    else
        return n;
}
function getDate() {
    var currentTime = new Date();
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    return validate(day) + '/' + validate(month) + '/' + year;
}

function menuSelect(menuname) {
    $('#menu > ul > li').removeAttr('class');
    switch (menuname) {
        case 'mlayouts':
            $('#mlayouts').attr('class', 'barcurrent');
            $('#reports').hide();
            $('#anteriores').hide();
            $('#layouts').fadeIn("slow");
            break;

        case 'manteriores':
            $('#manteriores').attr('class', 'barcurrent');
            $('#reports').hide();
            $('#layouts').hide();
            $('#anteriores').fadeIn("slow");
            break;

        default:
            $('#mreports').attr('class', 'barcurrent');
            $('#layouts').hide();
            $('#anteriores').hide();
            $('#reports').fadeIn("slow");
            break;
    }
}

$(document).ready(function () {
    $('.date').mask("99/99/9999");
    $('.date').val(getDate());
    $('.numeric').forceNumeric();

    $('#layouts').hide();
    $('#anteriores').hide();

    $('#menu > ul > li').click(function () {
        menuSelect((this).id);
    });
});

jQuery.fn.forceNumeric = function () {
    return this.each(function () {
        $(this).keydown(function (e) {
            var key = e.which || e.keyCode;

            if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
            // numbers
                         key >= 48 && key <= 57 ||
            // Numeric keypad
                         key >= 96 && key <= 105 ||
            // comma, period and minus
                        key == 190 || key == 188 || key == 109 ||
            // Backspace and Tab and Enter
                        key == 8 || key == 9 || key == 13 ||
            // Home and End
                        key == 35 || key == 36 ||
            // left and right arrows
                        key == 37 || key == 39 ||
            // Del and Ins
                        key == 46 || key == 45)
                return true;

            return false;
        });
    });
}