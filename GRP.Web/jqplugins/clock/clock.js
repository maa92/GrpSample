(function ($) {

    $.fn.extend({

        jClocksGMT: function (options) {

            var defaults = {
                offset: '0', // default gmt offset
                angleSec: 0,
                angleMin: 0,
                angleHour: 0,
                hour24: false,
                digital: true,
                analog: true
            }

            var options = $.extend(defaults, options);

            return this.each(function () {
                var offset = options.offset;
                var id = $(this).attr('id');

                // initial hand rotation
                $('#' + id + ' .sec').rotate(options.angleSec);
                $('#' + id + ' .min').rotate(options.angleMin);
                $('#' + id + ' .hour').rotate(options.angleHour);

                // check if daylight saving time is in effect
                Date.prototype.stdTimezoneOffset = function () {
                    var jan = new Date(this.getFullYear(), 0, 1);
                    var janUTC = jan.getTime() + (jan.getTimezoneOffset() * 60000);
                    var janOffset = new Date(janUTC + (3600000 * options.offset));
                    var jul = new Date(this.getFullYear(), 6, 1);
                    var julUTC = jul.getTime() + (jul.getTimezoneOffset() * 60000);
                    var julOffset = new Date(julUTC + (3600000 * options.offset));
                    return Math.max(jan.getTimezoneOffset(), jul.getTimezoneOffset());
                    //return Math.max(janOffset, julOffset);
                }
                Date.prototype.dst = function () {
                    if (parseFloat(options.offset) <= -4 && parseFloat(options.offset) >= -10) {
                        var dCheck = new Date;
                        var utcCheck = dCheck.getTime() + (dCheck.getTimezoneOffset() * 60000);
                        var newCheck = new Date(utcCheck + (3600000 * options.offset));
                        return this.getTimezoneOffset() < this.stdTimezoneOffset();
                        //return newCheck.getTimezoneOffset() < this.stdTimezoneOffset();
                    }
                }
                // create new date object
                var dateCheck = new Date;

                if (dateCheck.dst()) {
                    offset = parseFloat(offset) + 1;
                };

                setInterval(function () {
                    // create new date object
                    var d = new Date;
                    // convert to msec
                    // add local time offset
                    // get UTC time in msec
                    var utc = d.getTime() + (d.getTimezoneOffset() * 60000);

                    /*if( d.dst() ) {
                        offset = offset + 1;
                    };*/

                    // create new Date object for different city
                    // using supplied offset
                    var nd = new Date(utc + (3600000 * offset));

                    // format numbers
                    var s = nd.getSeconds();
                    var m = nd.getMinutes();
                    var hh = nd.getHours();
                    var h = hh;

                    // format for 12 hour
                    if (!options.hour24) {
                        var dd = "صباحا";
                        if (h >= 12) {
                            h = hh - 12;
                            dd = "مساءا";
                        }
                        if (h == 0) {
                            h = 12;
                        }
                    }

                    if (options.analog) {
                        // rotate second hand
                        options.angleSec = (nd.getSeconds() * 6);
                        $('#' + id + ' .sec').rotate(options.angleSec);
                        // rotate minute hand
                        options.angleMin = (nd.getMinutes() * 6);
                        $('#' + id + ' .min').rotate(options.angleMin);
                        // rotate hour hand
                        options.angleHour = ((nd.getHours() * 5 + nd.getMinutes() / 12) * 6);
                        $('#' + id + ' .hour').rotate(options.angleHour);
                    }

                    // display digital clock if enabled
                    if (options.digital) {
                        $('#' + id + ' .digital .hr').html(h + ':');
                        $('#' + id + ' .digital .minute').html(m < 10 ? "0" + m : m);
                        if (!options.hour24) {
                            $('#' + id + ' .digital .period').html(dd);
                        }
                    }
                }, 1000);
            });
        }
    });
})(jQuery);