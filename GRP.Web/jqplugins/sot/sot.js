﻿var JoelPurra = JoelPurra || {}; !function (a, b) { function h(a, b, c) { d = { isTab: a, isReverse: b, $target: c } } function i() { h(!1, !1, void 0) } function j() { return !(!d.isTab || void 0 === d.$target || 1 !== d.$target.length || d.isReverse) && (d.$target.emulateTab(1), !0) } function k(b) { var c = a(b.target); if (!(c.is(g) || c.parents(g).length > 0 || !c.is(f) && 0 === c.parents(f).length)) { h(d.isTab, d.isReverse, c); var e = j(); return e ? (b.preventDefault(), b.stopPropagation(), b.stopImmediatePropagation(), !1) : void 0 } } function l(a) { return !(a.altKey || a.ctrlKey || a.metaKey || a.which !== e) } function m(a) { l(a) ? (h(!0, a.shiftKey), setTimeout(i, 1)) : i() } function n() { i(), a(document).on("keydown" + c, m).on("focusin" + c, k) } b.SkipOnTab = function () { }; var c = ".SkipOnTab", d = {}, e = 9, f = ".skip-on-tab, [data-skip-on-tab=true]", g = ".disable-skip-on-tab, [data-skip-on-tab=false]"; b.SkipOnTab.skipOnTab = function (b, c) { return c = void 0 === c || c === !0, b.each(function () { var b = a(this); b.not(g).not(f).attr("data-skip-on-tab", c ? "true" : "false") }) }, a.fn.extend({ skipOnTab: function (a) { return b.SkipOnTab.skipOnTab(this, a) } }), a(n) }(jQuery, JoelPurra);