(function ($) {
    function _1(_2) {
        var _3 = $.data(_2, "hdatebox");
        var _4 = _3.options;
        _4.hijriConverter = _gsAppHDP;
        $(_2).addClass("datebox-f").combo($.extend({}, _4, {
            onShowPanel: function () {
                _5(this);
                _6(this);
                _7(this);
                _18(this, $(this).hdatebox("getText"), true);
                _4.onShowPanel.call(this);
            }
        })).combo('textbox').on('input', function (e) {
            var self = $(this);
            self.val(self.val().replace(/[^0-9\/]/g, ''));
        });
        if (!_3.calendar) {
            var _8 = $(_2).combo("panel").css("overflow", "hidden");
            _8.panel("options").onBeforeDestroy = function () {
                var c = $(this).find(".calendar-shared");
                if (c.length) {
                    c.insertBefore(c[0].pholder);
                }
            };
            var cc = $("<div class=\"datebox-calendar-inner\"></div>").prependTo(_8);
            if (_4.sharedCalendar) {
                var c = $(_4.sharedCalendar);
                if (!c[0].pholder) {
                    c[0].pholder = $("<div class=\"calendar-pholder\" style=\"display:none\"></div>").insertAfter(c);
                }
                c.addClass("calendar-shared").appendTo(cc);
                if (!c.hasClass("calendar")) {
                    c.hcalendar();
                }
                _3.calendar = c;
            } else {
                _3.calendar = $("<div></div>").appendTo(cc).hcalendar({
                    hijriConverter: _4.hijriConverter,
                    width: 180,
                    height: 180,
                    year: _3.options.hijriConverter.year(),
                    month: parseInt(_3.options.hijriConverter.month()),
                    current: {
                        year: _3.options.hijriConverter.year(),
                        month: _3.options.hijriConverter.month(),
                        day: _3.options.hijriConverter.day()
                    }
                });
            }
            $.extend(_3.calendar.hcalendar("options"), {
                fit: true,
                border: false,
                onSelect: function (_9) {
                    var _a = this.target;
                    var _b = $(_a).hdatebox("options");
                    _18(_a, _b.formatter.call(_a, _9));
                    $(_a).combo("hidePanel");
                    _b.onSelect.call(_a, _9);
                }
            });
        }
        $(_2).combo("textbox").parent().addClass("datebox");
        $(_2).hdatebox("initValue", _4.value);

        function _5(_c) {
            var _d = $(_c).hdatebox("options");
            var _e = $(_c).combo("panel");
            _e.unbind(".datebox").bind("click.datebox", function (e) {
                if ($(e.target).hasClass("datebox-button-a")) {
                    var _f = parseInt($(e.target).attr("datebox-button-index"));
                    _d.buttons[_f].handler.call(e.target, _c);
                }
            });
        };

        function _6(_10) {
            var _11 = $(_10).combo("panel");
            if (_11.children("div.datebox-button").length) {
                return;
            }
            var _12 = $("<div class=\"datebox-button\"><table cellspacing=\"0\" cellpadding=\"0\" style=\"width:100%\"><tr></tr></table></div>").appendTo(_11);
            var tr = _12.find("tr");
            for (var i = 0; i < _4.buttons.length; i++) {
                var td = $("<td></td>").appendTo(tr);
                var btn = _4.buttons[i];
                var t = $("<a class=\"datebox-button-a\" href=\"javascript:void(0)\"></a>").html($.isFunction(btn.text) ? btn.text(_10) : btn.text).appendTo(td);
                t.attr("datebox-button-index", i);
            }
            tr.find("td").css("width", (100 / _4.buttons.length) + "%");
        };

        function _7(_13) {
            var _14 = $(_13).combo("panel");
            var cc = _14.children("div.datebox-calendar-inner");
            _14.children()._outerWidth(_14.width());
            _3.calendar.appendTo(cc);
            _3.calendar[0].target = _13;
            if (_4.panelHeight != "auto") {
                var _15 = _14.height();
                _14.children().not(cc).each(function () {
                    _15 -= $(this).outerHeight();
                });
                cc._outerHeight(_15);
            }
            _3.calendar.hcalendar("resize");
        };
    };

    function _16(_17, q) {
        _18(_17, q, true);
    };

    function _19(_1a) {
        var _1b = $.data(_1a, "hdatebox");
        var _1c = _1b.options;
        var _1d = _1b.calendar.hcalendar("options").current;
        if (_1d) {
            _18(_1a, _1c.formatter.call(_1a, _1d));
            $(_1a).combo("hidePanel");
        }
    };

    function _18(_1e, _1f, _20) {
        var _21 = $.data(_1e, "hdatebox");
        var _22 = _21.options;
        var _23 = _21.calendar;
        _23.hcalendar("moveTo", _22.parser.call(_1e, _1f));
        if (_20) {
            $(_1e).combo("setValue", _1f);
        } else {
            if (_1f) {
                _1f = _22.formatter.call(_1e, _23.hcalendar("options").current);
            }
            $(_1e).combo("setText", _1f).combo("setValue", _1f);
        }
    };
    $.fn.hdatebox = function (_24, _25) {
        if (typeof _24 == "string") {
            var _26 = $.fn.hdatebox.methods[_24];
            if (_26) {
                return _26(this, _25);
            } else {
                return this.combo(_24, _25);
            }
        }
        _24 = _24 || {};
        return this.each(function () {
            var _27 = $.data(this, "hdatebox");
            if (_27) {
                $.extend(_27.options, _24);
            } else {
                $.data(this, "hdatebox", {
                    options: $.extend({}, $.fn.hdatebox.defaults, $.fn.hdatebox.parseOptions(this), _24)
                });
            }
            _1(this);
        });
    };
    $.fn.hdatebox.methods = {
        options: function (jq) {
            var _28 = jq.combo("options");
            return $.extend($.data(jq[0], "hdatebox").options, {
                width: _28.width,
                height: _28.height,
                originalValue: _28.originalValue,
                disabled: _28.disabled,
                readonly: _28.readonly
            });
        },
        cloneFrom: function (jq, _29) {
            return jq.each(function () {
                $(this).combo("cloneFrom", _29);
                $.data(this, "hdatebox", {
                    options: $.extend(true, {}, $(_29).hdatebox("options")),
                    calendar: $(_29).hdatebox("calendar")
                });
                $(this).addClass("datebox-f");
            });
        },
        calendar: function (jq) {
            return $.data(jq[0], "hdatebox").calendar;
        },
        initValue: function (jq, _2a) {
            return jq.each(function () {
                var _2b = $(this).hdatebox("options");
                var _2c = _2b.value;
                if (_2c) {
                    _2c = _2b.formatter.call(this, _2b.parser.call(this, _2c));
                }
                $(this).combo("initValue", _2c).combo("setText", _2c);
            });
        },
        getValueF: function (jq, _2c) {
            if ($(jq).combo("getValue")) {
                var va = $(jq).combo("getValue").split('/');
                var rv = va.join('');
                if (_2c.indexOf('dd') > -1 || _2c.indexOf('mm') > -1 || _2c.indexOf('yyyy') > -1)
                    rv = _2c.replace('dd', va[2]).replace('mm', va[1]).replace('yyyy', va[0]);
                return rv;
            }
            else { return ''; }
        },
        setValue: function (jq, _2d) {
            return jq.each(function () {
                _18(this, _2d);
            });
        },
        reset: function (jq) {
            return jq.each(function () {
                var _2e = $(this).hdatebox("options");
                $(this).hdatebox("setValue", _2e.originalValue);
            });
        }
    };
    $.fn.hdatebox.parseOptions = function (_2f) {
        return $.extend({}, $.fn.combo.parseOptions(_2f), $.parser.parseOptions(_2f, ["sharedCalendar"]));
    };
    $.fn.hdatebox.defaults = $.extend({}, $.fn.combo.defaults, {
        panelWidth: 195,
        panelHeight: "auto",
        sharedCalendar: null,
        keyHandler: {
            up: function (e) { },
            down: function (e) { },
            left: function (e) { },
            right: function (e) { },
            enter: function (e) {
                _19(this);
            },
            query: function (q, e) {
                _16(this, q);
            }
        },
        currentText: "Today",
        closeText: "Close",
        okText: "Ok",
        buttons: [{
            text: function (_30) {
                return $(_30).hdatebox("options").currentText;
            },
            handler: function (_31) {
                var hdc = $(_31).hdatebox("options").hijriConverter;
                var now = hdc.gHDate(new Date());
                $(_31).hdatebox("calendar").hcalendar({
                    year: hdc.year(),
                    month: hdc.month(),
                    current: now
                });
                _19(_31);
            }
        }, {
            text: function (_32) {
                return $(_32).hdatebox("options").closeText;
            },
            handler: function (_33) {
                $(this).closest("div.combo-panel").panel("close");
            }
        }],
        formatter: function (_34) {
            var y = _34.year;
            var m = _34.month;
            var d = _34.day;
            return y + "/" + zeroPad(m, 2) + "/" + zeroPad(d, 2); // for arabic UI alignment
        },
        parser: function (s) {
            if (!s) {
                return $(this).hdatebox("options").hijriConverter.gHDate(new Date());
            }
            var m, d, y;
            if (s.indexOf('/') > -1) {
                var ss = s.split("/");
                if (ss[0].length == 4)
                    y = parseInt(ss[0]);
                else
                    d = parseInt(ss[0]);
                m = ss[1] ? parseInt(ss[1]) : null;
                if (ss[2]) {
                    if (ss[2].length == 4)
                        y = parseInt(ss[2]);
                    else
                        d = parseInt(ss[2]);
                }
            }
            else {
                d = parseInt(s.substr(0, 2));
                m = parseInt(s.substr(2, 2));
                y = parseInt(s.substr(4, 4));
            }
            if (!isNaN(y) && !isNaN(m) && !isNaN(d) && d <= 31 && m <= 12) {
                return { year: y, month: m, day: d };
            } else {
                return $(this).hdatebox("options").hijriConverter.gHDate(new Date());
            }
        },
        onSelect: function (_35) { }
    });
})(jQuery);
(function ($) {
	function _1(_2, _3) {
		var _4 = $.data(_2, "hcalendar").options;
		var t = $(_2);
		if (_3) {
			$.extend(_4, {
				width: _3.width,
				height: _3.height
			});
		}
		t._size(_4, t.parent());
		t.find(".calendar-body")._outerHeight(t.height() - t.find(".calendar-header")._outerHeight());
		if (t.find(".calendar-menu").is(":visible")) {
			_5(_2);
		}
	};

	function _6(_7) {
		$(_7).addClass("calendar").html("<div class=\"calendar-header\">" + "<div class=\"calendar-nav calendar-prevmonth\"></div>" + "<div class=\"calendar-nav calendar-nextmonth\"></div>" + "<div class=\"calendar-nav calendar-prevyear\"></div>" + "<div class=\"calendar-nav calendar-nextyear\"></div>" + "<div class=\"calendar-title\">" + "<span class=\"calendar-text\"></span>" + "</div>" + "</div>" + "<div class=\"calendar-body\">" + "<div class=\"calendar-menu\">" + "<div class=\"calendar-menu-year-inner\">" + "<span class=\"calendar-nav calendar-menu-prev\"></span>" + "<span><input class=\"calendar-menu-year\" type=\"text\"></input></span>" + "<span class=\"calendar-nav calendar-menu-next\"></span>" + "</div>" + "<div class=\"calendar-menu-month-inner\">" + "</div>" + "</div>" + "</div>");
		$(_7).bind("_resize", function (e, _8) {
			if ($(this).hasClass("easyui-fluid") || _8) {
				_1(_7);
			}
			return false;
		});
	};

	function _9(_a) {
		var _b = $.data(_a, "hcalendar").options;
		var _c = $(_a).find(".calendar-menu");
		_c.find(".calendar-menu-year").unbind(".calendar").bind("keypress.calendar", function (e) {
			if (e.keyCode == 13) {
				_d(true);
			}
		});
		$(_a).unbind(".calendar").bind("mouseover.calendar", function (e) {
			var t = _e(e.target);
			if (t.hasClass("calendar-nav") || t.hasClass("calendar-text") || (t.hasClass("calendar-day") && !t.hasClass("calendar-disabled"))) {
				t.addClass("calendar-nav-hover");
			}
		}).bind("mouseout.calendar", function (e) {
			var t = _e(e.target);
			if (t.hasClass("calendar-nav") || t.hasClass("calendar-text") || (t.hasClass("calendar-day") && !t.hasClass("calendar-disabled"))) {
				t.removeClass("calendar-nav-hover");
			}
		}).bind("click.calendar", function (e) {
			var t = _e(e.target);
			if (t.hasClass("calendar-menu-next") || t.hasClass("calendar-nextyear")) {
				_f(1);
			} else {
				if (t.hasClass("calendar-menu-prev") || t.hasClass("calendar-prevyear")) {
					_f(-1);
				} else {
					if (t.hasClass("calendar-menu-month")) {
						_c.find(".calendar-selected").removeClass("calendar-selected");
						t.addClass("calendar-selected");
						_d(true);
					} else {
						if (t.hasClass("calendar-prevmonth")) {
							_10(-1);
						} else {
							if (t.hasClass("calendar-nextmonth")) {
								_10(1);
							} else {
								if (t.hasClass("calendar-text")) {
									if (_c.is(":visible")) {
										_c.hide();
									} else {
										_5(_a);
									}
								} else {
									if (t.hasClass("calendar-day")) {
										if (t.hasClass("calendar-disabled")) {
											return;
										}
										var _11 = _b.current;
										t.closest("div.calendar-body").find(".calendar-selected").removeClass("calendar-selected");
										t.addClass("calendar-selected");
										var _12 = t.attr("abbr").split(",");
										var y = parseInt(_12[0]);
										var m = parseInt(_12[1]);
										var d = parseInt(_12[2]);
										_b.current = {
											year: y,
											month: m,
											day: d
										};
										_b.onSelect.call(_a, _b.current);
										var _12 = _b.hijriConverter.gDate(_11.year, _11.month, _11.day);
										var _13 = _b.hijriConverter.gDate(_b.current.year, _b.current.month, _b.current.day);
										if (!_11 || _12.getTime() != _13.getTime()) {
											_b.onChange.call(_a, _b.current, _11);
										}
										if (_b.year != y || _b.month != m) {
											_b.year = y;
											_b.month = m;
											_19(_a);
										}
									}
								}
							}
						}
					}
				}
			}
		});

		function _e(t) {
			var day = $(t).closest(".calendar-day");
			if (day.length) {
				return day;
			} else {
				return $(t);
			}
		};

		function _d(_13) {
			var _14 = $(_a).find(".calendar-menu");
			var _15 = _14.find(".calendar-menu-year").val();
			var _16 = _14.find(".calendar-selected").attr("abbr");
			if (!isNaN(_15)) {
				_b.year = parseInt(_15);
				_b.month = parseInt(_16);
				_19(_a);
			}
			if (_13) {
				_14.hide();
			}
		};

		function _f(_17) {
			_b.year += _17;
			_19(_a);
			_c.find(".calendar-menu-year").val(_b.year);
		};

		function _10(_18) {
			_b.month += _18;
			if (_b.month > 12) {
				_b.year++;
				_b.month = 1;
			} else {
				if (_b.month < 1) {
					_b.year--;
					_b.month = 12;
				}
			}
			_19(_a);
			_c.find("td.calendar-selected").removeClass("calendar-selected");
			_c.find("td:eq(" + (_b.month - 1) + ")").addClass("calendar-selected");
		};
	};

	function _5(_1a) {
		var _1b = $.data(_1a, "hcalendar").options;
		$(_1a).find(".calendar-menu").show();
		if ($(_1a).find(".calendar-menu-month-inner").is(":empty")) {
			$(_1a).find(".calendar-menu-month-inner").empty();
			var t = $("<table class=\"calendar-mtable\"></table>").appendTo($(_1a).find(".calendar-menu-month-inner"));
			var idx = 0;
			for (var i = 0; i < 3; i++) {
				var tr = $("<tr></tr>").appendTo(t);
				for (var j = 0; j < 4; j++) {
					$("<td class=\"calendar-nav calendar-menu-month\"></td>").html(_1b.months[idx++]).attr("abbr", idx).appendTo(tr);
				}
			}
		}
		var _1c = $(_1a).find(".calendar-body");
		var _1d = $(_1a).find(".calendar-menu");
		var _1e = _1d.find(".calendar-menu-year-inner");
		var _1f = _1d.find(".calendar-menu-month-inner");
		_1e.find("input").val(_1b.year).focus();
		_1f.find("td.calendar-selected").removeClass("calendar-selected");
		_1f.find("td:eq(" + (_1b.month - 1) + ")").addClass("calendar-selected");
		_1d._outerWidth(_1c._outerWidth());
		_1d._outerHeight(_1c._outerHeight());
		_1f._outerHeight(_1d.height() - _1e._outerHeight());
	};

	function _20(_21, _22, _23) {
		var _24 = $.data(_21, "hcalendar").options;
		var _25 = [];
		var _26 = _24.hijriConverter.gMonthDays(_22, _23);
		for (var i = 1; i <= _26; i++) {
			_25.push([_22, _23, i]);
		}
		var _27 = [],
            _28 = [];
		var _29 = -1;
		while (_25.length > 0) {
			var _2a = _25.shift();
			_28.push(_2a);
			var day = _24.hijriConverter.gDay(_2a[0], _2a[1], _2a[2]);
			if (_29 == day) {
				day = 0;
			} else {
				if (day == (_24.firstDay == 0 ? 7 : _24.firstDay) - 1) {
					_27.push(_28);
					_28 = [];
				}
			}
			_29 = day;
		}
		if (_28.length) {
			_27.push(_28);
		}
		var _2b = _27[0];
		if (_2b.length < 7) {
			while (_2b.length < 7) {
				var _2c = _2b[0];
				var _2a = _24.hijriConverter.gHDate(g_addDays(_24.hijriConverter.gDate(_2c[0], _2c[1], _2c[2]), -1));
				_2b.unshift([_2a.year, _2a.month, _2a.day]);
			}
		} else {
			var _2c = _2b[0];
			var _28 = [];
			for (var i = 1; i <= 7; i++) {
				var _2a = _24.hijriConverter.gHDate(g_addDays(_24.hijriConverter.gDate(_2c[0], _2c[1], _2c[2]), -i));
				_28.unshift([_2a.year, _2a.month, _2a.day]);
			}
			_27.unshift(_28);
		}
		var _2d = _27[_27.length - 1];
		while (_2d.length < 7) {
			var _2e = _2d[_2d.length - 1];
			var _2a = _24.hijriConverter.gHDate(g_addDays(_24.hijriConverter.gDate(_2e[0], _2e[1], _2e[2]), 1));
			_2d.push([_2a.year, _2a.month, _2a.day]);
		}
		if (_27.length < 6) {
			var _2e = _2d[_2d.length - 1];
			var _28 = [];
			for (var i = 1; i <= 7; i++) {
				var _2a = _24.hijriConverter.gHDate(g_addDays(_24.hijriConverter.gDate(_2e[0], _2e[1], _2e[2]), i));
				_28.push([_2a.year, _2a.month, _2a.day]);
			}
			_27.push(_28);
		}
		return _27;
	};

	function _19(_2f) {
		var _30 = $.data(_2f, "hcalendar").options;
		if (_30.current && !_30.validator.call(_2f, _30.current)) {
			_30.current = null;
		}
		var now = _30.hijriConverter.gHDate(new Date());
		var _31 = now.year + "," + now.month + "," + now.day;
		var _32 = _30.current ? (_30.current.year + "," + _30.current.month + "," + _30.current.day) : "";
		var _33 = 6 - _30.firstDay;
		var _34 = _33 + 1;
		if (_33 >= 7) {
			_33 -= 7;
		}
		if (_34 >= 7) {
			_34 -= 7;
		}
		$(_2f).find(".calendar-title span").html(_30.months[_30.month - 1] + " " + _30.year);
		var _35 = $(_2f).find("div.calendar-body");
		_35.children("table").remove();
		var _36 = ["<table class=\"calendar-dtable\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">"];
		_36.push("<thead><tr>");
		for (var i = _30.firstDay; i < _30.weeks.length; i++) {
			_36.push("<th>" + _30.weeks[i] + "</th>");
		}
		for (var i = 0; i < _30.firstDay; i++) {
			_36.push("<th>" + _30.weeks[i] + "</th>");
		}
		_36.push("</tr></thead>");
		_36.push("<tbody>");
		var _37 = _20(_2f, _30.year, _30.month);
		for (var i = 0; i < _37.length; i++) {
			var _38 = _37[i];
			var cls = "";
			if (i == 0) {
				cls = "calendar-first";
			} else {
				if (i == _37.length - 1) {
					cls = "calendar-last";
				}
			}
			_36.push("<tr class=\"" + cls + "\">");
			for (var j = 0; j < _38.length; j++) {
				var day = _38[j];
				var s = day[0] + "," + day[1] + "," + day[2];
				var _39 = {
					year: day[0],
					month: day[1],
					day: day[2]
				};
				var d = _30.formatter.call(_2f, _39);
				var css = _30.styler.call(_2f, _39);
				var _3a = "";
				var _3b = "";
				if (typeof css == "string") {
					_3b = css;
				} else {
					if (css) {
						_3a = css["class"] || "";
						_3b = css["style"] || "";
					}
				}
				var cls = "calendar-day";
				if (!(_30.year == day[0] && _30.month == day[1])) {
					cls += " calendar-other-month";
				}
				if (s == _31) {
					cls += " calendar-today";
				}
				if (s == _32) {
					cls += " calendar-selected";
				}
				if (j == _33) {
					cls += " calendar-saturday";
				} else {
					if (j == _34) {
						cls += " calendar-sunday";
					}
				}
				if (j == 0) {
					cls += " calendar-first";
				} else {
					if (j == _38.length - 1) {
						cls += " calendar-last";
					}
				}
				cls += " " + _3a;
				if (!_30.validator.call(_2f, _39)) {
					cls += " calendar-disabled";
				}
				_36.push("<td class=\"" + cls + "\" abbr=\"" + s + "\" style=\"" + _3b + "\">" + d + "</td>");
			}
			_36.push("</tr>");
		}
		_36.push("</tbody>");
		_36.push("</table>");
		_35.append(_36.join(""));
		_35.children("table.calendar-dtable").prependTo(_35);
		_30.onNavigate.call(_2f, _30.year, _30.month);
	};
	$.fn.hcalendar = function (_3c, _3d) {
		if (typeof _3c == "string") {
			return $.fn.hcalendar.methods[_3c](this, _3d);
		}
		_3c = _3c || {};
		return this.each(function () {
			var _3e = $.data(this, "hcalendar");
			if (_3e) {
				$.extend(_3e.options, _3c);
			} else {
				_3e = $.data(this, "hcalendar", {
					options: $.extend({}, $.fn.hcalendar.defaults, $.fn.hcalendar.parseOptions(this), _3c)
				});
				_6(this);
			}
			if (_3e.options.border == false) {
				$(this).addClass("calendar-noborder");
			}
			_1(this);
			_9(this);
			_19(this);
			$(this).find("div.calendar-menu").hide();
		});
	};
	$.fn.hcalendar.methods = {
		options: function (jq) {
			return $.data(jq[0], "hcalendar").options;
		},
		resize: function (jq, _3f) {
			return jq.each(function () {
				_1(this, _3f);
			});
		},
		moveTo: function (jq, _40) {
			return jq.each(function () {
				var _41 = $(this).hcalendar("options");
				if (!_40) {
					var now = _41.hijriConverter.gHDate(new Date());
					$(this).hcalendar({
						year: now.year,
						month: now.month,
						current: _40
					});
					return;
				}
				if (_41.validator.call(this, _40)) {
					var _42 = _41.current;
					$(this).hcalendar({
						year: _40.year,
						month: _40.month,
						current: _40
					});
					var _43 = _41.hijriConverter.gDate(_42.year, _42.month, _42.day);
					var _44 = _41.hijriConverter.gDate(_40.year, _40.month, _40.day);
					if (!_42 || _43.getTime() != _44.getTime()) {
						_41.onChange.call(this, _41.current, _42);
					}
				}
			});
		}
	};
	$.fn.hcalendar.parseOptions = function (_43) {
		var t = $(_43);
		return $.extend({}, $.parser.parseOptions(_43, [{
			firstDay: "number",
			fit: "boolean",
			border: "boolean"
		}]));
	};
	$.fn.hcalendar.defaults = {
		width: 180,
		height: 180,
		fit: false,
		border: true,
		firstDay: 0,
		weeks: ["S", "M", "T", "W", "T", "F", "S"],
		months: ["Muharram", "Safar", "Rabi’ al-Awwal", "Rabi’ al-Thani", "Jumada al-Ula", "Jumada al-Alkhirah", "Rajab", "Sha’ban", "Ramadhan", "Shawwal", "Thul-Qi’dah", "Thul-Hijjah"],
		year: new Date().getFullYear(),
		month: new Date().getMonth() + 1,
		current: (function () {
			var d = new Date();
			return new Date(d.getFullYear(), d.getMonth(), d.getDate());
		})(),
		formatter: function (_44) {
			return _44.day;
		},
		styler: function (_45) {
			return "";
		},
		validator: function (_46) {
			return true;
		},
		onSelect: function (_47) { },
		onChange: function (_48, _49) { },
		onNavigate: function (_4a, _4b) { }
	};
})(jQuery);