/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('kendo.core', ['jquery'], f);
}(function () {
    var __meta__ = {
        id: 'core',
        name: 'Core',
        category: 'framework',
        description: 'The core of the Kendo framework.'
    };
    (function ($, window, undefined) {
        var kendo = window.kendo = window.kendo || { cultures: {} }, extend = $.extend, each = $.each, isArray = $.isArray, proxy = $.proxy, noop = $.noop, math = Math, Template, JSON = window.JSON || {}, support = {}, percentRegExp = /%/, formatRegExp = /\{(\d+)(:[^\}]+)?\}/g, boxShadowRegExp = /(\d+(?:\.?)\d*)px\s*(\d+(?:\.?)\d*)px\s*(\d+(?:\.?)\d*)px\s*(\d+)?/i, numberRegExp = /^(\+|-?)\d+(\.?)\d*$/, FUNCTION = 'function', STRING = 'string', NUMBER = 'number', OBJECT = 'object', NULL = 'null', BOOLEAN = 'boolean', UNDEFINED = 'undefined', getterCache = {}, setterCache = {}, slice = [].slice;
        kendo.version = '2018.2.613'.replace(/^\s+|\s+$/g, '');
        function Class() {
        }
        Class.extend = function (proto) {
            var base = function () {
                }, member, that = this, subclass = proto && proto.init ? proto.init : function () {
                    that.apply(this, arguments);
                }, fn;
            base.prototype = that.prototype;
            fn = subclass.fn = subclass.prototype = new base();
            for (member in proto) {
                if (proto[member] != null && proto[member].constructor === Object) {
                    fn[member] = extend(true, {}, base.prototype[member], proto[member]);
                } else {
                    fn[member] = proto[member];
                }
            }
            fn.constructor = subclass;
            subclass.extend = that.extend;
            return subclass;
        };
        Class.prototype._initOptions = function (options) {
            this.options = deepExtend({}, this.options, options);
        };
        var isFunction = kendo.isFunction = function (fn) {
            return typeof fn === 'function';
        };
        var preventDefault = function () {
            this._defaultPrevented = true;
        };
        var isDefaultPrevented = function () {
            return this._defaultPrevented === true;
        };
        var Observable = Class.extend({
            init: function () {
                this._events = {};
            },
            bind: function (eventName, handlers, one) {
                var that = this, idx, eventNames = typeof eventName === STRING ? [eventName] : eventName, length, original, handler, handlersIsFunction = typeof handlers === FUNCTION, events;
                if (handlers === undefined) {
                    for (idx in eventName) {
                        that.bind(idx, eventName[idx]);
                    }
                    return that;
                }
                for (idx = 0, length = eventNames.length; idx < length; idx++) {
                    eventName = eventNames[idx];
                    handler = handlersIsFunction ? handlers : handlers[eventName];
                    if (handler) {
                        if (one) {
                            original = handler;
                            handler = function () {
                                that.unbind(eventName, handler);
                                original.apply(that, arguments);
                            };
                            handler.original = original;
                        }
                        events = that._events[eventName] = that._events[eventName] || [];
                        events.push(handler);
                    }
                }
                return that;
            },
            one: function (eventNames, handlers) {
                return this.bind(eventNames, handlers, true);
            },
            first: function (eventName, handlers) {
                var that = this, idx, eventNames = typeof eventName === STRING ? [eventName] : eventName, length, handler, handlersIsFunction = typeof handlers === FUNCTION, events;
                for (idx = 0, length = eventNames.length; idx < length; idx++) {
                    eventName = eventNames[idx];
                    handler = handlersIsFunction ? handlers : handlers[eventName];
                    if (handler) {
                        events = that._events[eventName] = that._events[eventName] || [];
                        events.unshift(handler);
                    }
                }
                return that;
            },
            trigger: function (eventName, e) {
                var that = this, events = that._events[eventName], idx, length;
                if (events) {
                    e = e || {};
                    e.sender = that;
                    e._defaultPrevented = false;
                    e.preventDefault = preventDefault;
                    e.isDefaultPrevented = isDefaultPrevented;
                    events = events.slice();
                    for (idx = 0, length = events.length; idx < length; idx++) {
                        events[idx].call(that, e);
                    }
                    return e._defaultPrevented === true;
                }
                return false;
            },
            unbind: function (eventName, handler) {
                var that = this, events = that._events[eventName], idx;
                if (eventName === undefined) {
                    that._events = {};
                } else if (events) {
                    if (handler) {
                        for (idx = events.length - 1; idx >= 0; idx--) {
                            if (events[idx] === handler || events[idx].original === handler) {
                                events.splice(idx, 1);
                            }
                        }
                    } else {
                        that._events[eventName] = [];
                    }
                }
                return that;
            }
        });
        function compilePart(part, stringPart) {
            if (stringPart) {
                return '\'' + part.split('\'').join('\\\'').split('\\"').join('\\\\\\"').replace(/\n/g, '\\n').replace(/\r/g, '\\r').replace(/\t/g, '\\t') + '\'';
            } else {
                var first = part.charAt(0), rest = part.substring(1);
                if (first === '=') {
                    return '+(' + rest + ')+';
                } else if (first === ':') {
                    return '+$kendoHtmlEncode(' + rest + ')+';
                } else {
                    return ';' + part + ';$kendoOutput+=';
                }
            }
        }
        var argumentNameRegExp = /^\w+/, encodeRegExp = /\$\{([^}]*)\}/g, escapedCurlyRegExp = /\\\}/g, curlyRegExp = /__CURLY__/g, escapedSharpRegExp = /\\#/g, sharpRegExp = /__SHARP__/g, zeros = [
                '',
                '0',
                '00',
                '000',
                '0000'
            ];
        Template = {
            paramName: 'data',
            useWithBlock: true,
            render: function (template, data) {
                var idx, length, html = '';
                for (idx = 0, length = data.length; idx < length; idx++) {
                    html += template(data[idx]);
                }
                return html;
            },
            compile: function (template, options) {
                var settings = extend({}, this, options), paramName = settings.paramName, argumentName = paramName.match(argumentNameRegExp)[0], useWithBlock = settings.useWithBlock, functionBody = 'var $kendoOutput, $kendoHtmlEncode = kendo.htmlEncode;', fn, parts, idx;
                if (isFunction(template)) {
                    return template;
                }
                functionBody += useWithBlock ? 'with(' + paramName + '){' : '';
                functionBody += '$kendoOutput=';
                parts = template.replace(escapedCurlyRegExp, '__CURLY__').replace(encodeRegExp, '#=$kendoHtmlEncode($1)#').replace(curlyRegExp, '}').replace(escapedSharpRegExp, '__SHARP__').split('#');
                for (idx = 0; idx < parts.length; idx++) {
                    functionBody += compilePart(parts[idx], idx % 2 === 0);
                }
                functionBody += useWithBlock ? ';}' : ';';
                functionBody += 'return $kendoOutput;';
                functionBody = functionBody.replace(sharpRegExp, '#');
                try {
                    fn = new Function(argumentName, functionBody);
                    fn._slotCount = Math.floor(parts.length / 2);
                    return fn;
                } catch (e) {
                    throw new Error(kendo.format('Invalid template:\'{0}\' Generated code:\'{1}\'', template, functionBody));
                }
            }
        };
        function pad(number, digits, end) {
            number = number + '';
            digits = digits || 2;
            end = digits - number.length;
            if (end) {
                return zeros[digits].substring(0, end) + number;
            }
            return number;
        }
        (function () {
            var escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, gap, indent, meta = {
                    '\b': '\\b',
                    '\t': '\\t',
                    '\n': '\\n',
                    '\f': '\\f',
                    '\r': '\\r',
                    '"': '\\"',
                    '\\': '\\\\'
                }, rep, toString = {}.toString;
            if (typeof Date.prototype.toJSON !== FUNCTION) {
                Date.prototype.toJSON = function () {
                    var that = this;
                    return isFinite(that.valueOf()) ? pad(that.getUTCFullYear(), 4) + '-' + pad(that.getUTCMonth() + 1) + '-' + pad(that.getUTCDate()) + 'T' + pad(that.getUTCHours()) + ':' + pad(that.getUTCMinutes()) + ':' + pad(that.getUTCSeconds()) + 'Z' : null;
                };
                String.prototype.toJSON = Number.prototype.toJSON = Boolean.prototype.toJSON = function () {
                    return this.valueOf();
                };
            }
            function quote(string) {
                escapable.lastIndex = 0;
                return escapable.test(string) ? '"' + string.replace(escapable, function (a) {
                    var c = meta[a];
                    return typeof c === STRING ? c : '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                }) + '"' : '"' + string + '"';
            }
            function str(key, holder) {
                var i, k, v, length, mind = gap, partial, value = holder[key], type;
                if (value && typeof value === OBJECT && typeof value.toJSON === FUNCTION) {
                    value = value.toJSON(key);
                }
                if (typeof rep === FUNCTION) {
                    value = rep.call(holder, key, value);
                }
                type = typeof value;
                if (type === STRING) {
                    return quote(value);
                } else if (type === NUMBER) {
                    return isFinite(value) ? String(value) : NULL;
                } else if (type === BOOLEAN || type === NULL) {
                    return String(value);
                } else if (type === OBJECT) {
                    if (!value) {
                        return NULL;
                    }
                    gap += indent;
                    partial = [];
                    if (toString.apply(value) === '[object Array]') {
                        length = value.length;
                        for (i = 0; i < length; i++) {
                            partial[i] = str(i, value) || NULL;
                        }
                        v = partial.length === 0 ? '[]' : gap ? '[\n' + gap + partial.join(',\n' + gap) + '\n' + mind + ']' : '[' + partial.join(',') + ']';
                        gap = mind;
                        return v;
                    }
                    if (rep && typeof rep === OBJECT) {
                        length = rep.length;
                        for (i = 0; i < length; i++) {
                            if (typeof rep[i] === STRING) {
                                k = rep[i];
                                v = str(k, value);
                                if (v) {
                                    partial.push(quote(k) + (gap ? ': ' : ':') + v);
                                }
                            }
                        }
                    } else {
                        for (k in value) {
                            if (Object.hasOwnProperty.call(value, k)) {
                                v = str(k, value);
                                if (v) {
                                    partial.push(quote(k) + (gap ? ': ' : ':') + v);
                                }
                            }
                        }
                    }
                    v = partial.length === 0 ? '{}' : gap ? '{\n' + gap + partial.join(',\n' + gap) + '\n' + mind + '}' : '{' + partial.join(',') + '}';
                    gap = mind;
                    return v;
                }
            }
            if (typeof JSON.stringify !== FUNCTION) {
                JSON.stringify = function (value, replacer, space) {
                    var i;
                    gap = '';
                    indent = '';
                    if (typeof space === NUMBER) {
                        for (i = 0; i < space; i += 1) {
                            indent += ' ';
                        }
                    } else if (typeof space === STRING) {
                        indent = space;
                    }
                    rep = replacer;
                    if (replacer && typeof replacer !== FUNCTION && (typeof replacer !== OBJECT || typeof replacer.length !== NUMBER)) {
                        throw new Error('JSON.stringify');
                    }
                    return str('', { '': value });
                };
            }
        }());
        (function () {
            var dateFormatRegExp = /dddd|ddd|dd|d|MMMM|MMM|MM|M|yyyy|yy|HH|H|hh|h|mm|m|fff|ff|f|tt|ss|s|zzz|zz|z|"[^"]*"|'[^']*'/g, standardFormatRegExp = /^(n|c|p|e)(\d*)$/i, literalRegExp = /(\\.)|(['][^']*[']?)|(["][^"]*["]?)/g, commaRegExp = /\,/g, EMPTY = '', POINT = '.', COMMA = ',', SHARP = '#', ZERO = '0', PLACEHOLDER = '??', EN = 'en-US', objectToString = {}.toString;
            kendo.cultures['en-US'] = {
                name: EN,
                numberFormat: {
                    pattern: ['-n'],
                    decimals: 2,
                    ',': ',',
                    '.': '.',
                    groupSize: [3],
                    percent: {
                        pattern: [
                            '-n %',
                            'n %'
                        ],
                        decimals: 2,
                        ',': ',',
                        '.': '.',
                        groupSize: [3],
                        symbol: '%'
                    },
                    currency: {
                        name: 'US Dollar',
                        abbr: 'USD',
                        pattern: [
                            '($n)',
                            '$n'
                        ],
                        decimals: 2,
                        ',': ',',
                        '.': '.',
                        groupSize: [3],
                        symbol: '$'
                    }
                },
                calendars: {
                    standard: {
                        days: {
                            names: [
                                'Sunday',
                                'Monday',
                                'Tuesday',
                                'Wednesday',
                                'Thursday',
                                'Friday',
                                'Saturday'
                            ],
                            namesAbbr: [
                                'Sun',
                                'Mon',
                                'Tue',
                                'Wed',
                                'Thu',
                                'Fri',
                                'Sat'
                            ],
                            namesShort: [
                                'Su',
                                'Mo',
                                'Tu',
                                'We',
                                'Th',
                                'Fr',
                                'Sa'
                            ]
                        },
                        months: {
                            names: [
                                'January',
                                'February',
                                'March',
                                'April',
                                'May',
                                'June',
                                'July',
                                'August',
                                'September',
                                'October',
                                'November',
                                'December'
                            ],
                            namesAbbr: [
                                'Jan',
                                'Feb',
                                'Mar',
                                'Apr',
                                'May',
                                'Jun',
                                'Jul',
                                'Aug',
                                'Sep',
                                'Oct',
                                'Nov',
                                'Dec'
                            ]
                        },
                        AM: [
                            'AM',
                            'am',
                            'AM'
                        ],
                        PM: [
                            'PM',
                            'pm',
                            'PM'
                        ],
                        patterns: {
                            d: 'M/d/yyyy',
                            D: 'dddd, MMMM dd, yyyy',
                            F: 'dddd, MMMM dd, yyyy h:mm:ss tt',
                            g: 'M/d/yyyy h:mm tt',
                            G: 'M/d/yyyy h:mm:ss tt',
                            m: 'MMMM dd',
                            M: 'MMMM dd',
                            s: 'yyyy\'-\'MM\'-\'ddTHH\':\'mm\':\'ss',
                            t: 'h:mm tt',
                            T: 'h:mm:ss tt',
                            u: 'yyyy\'-\'MM\'-\'dd HH\':\'mm\':\'ss\'Z\'',
                            y: 'MMMM, yyyy',
                            Y: 'MMMM, yyyy'
                        },
                        '/': '/',
                        ':': ':',
                        firstDay: 0,
                        twoDigitYearMax: 2029
                    }
                }
            };
            function findCulture(culture) {
                if (culture) {
                    if (culture.numberFormat) {
                        return culture;
                    }
                    if (typeof culture === STRING) {
                        var cultures = kendo.cultures;
                        return cultures[culture] || cultures[culture.split('-')[0]] || null;
                    }
                    return null;
                }
                return null;
            }
            function getCulture(culture) {
                if (culture) {
                    culture = findCulture(culture);
                }
                return culture || kendo.cultures.current;
            }
            kendo.culture = function (cultureName) {
                var cultures = kendo.cultures, culture;
                if (cultureName !== undefined) {
                    culture = findCulture(cultureName) || cultures[EN];
                    culture.calendar = culture.calendars.standard;
                    cultures.current = culture;
                } else {
                    return cultures.current;
                }
            };
            kendo.findCulture = findCulture;
            kendo.getCulture = getCulture;
            kendo.culture(EN);
            function formatDate(date, format, culture) {
                culture = getCulture(culture);
                var calendar = culture.calendars.standard, days = calendar.days, months = calendar.months;
                format = calendar.patterns[format] || format;
                return format.replace(dateFormatRegExp, function (match) {
                    var minutes;
                    var result;
                    var sign;
                    if (match === 'd') {
                        result = date.getDate();
                    } else if (match === 'dd') {
                        result = pad(date.getDate());
                    } else if (match === 'ddd') {
                        result = days.namesAbbr[date.getDay()];
                    } else if (match === 'dddd') {
                        result = days.names[date.getDay()];
                    } else if (match === 'M') {
                        result = date.getMonth() + 1;
                    } else if (match === 'MM') {
                        result = pad(date.getMonth() + 1);
                    } else if (match === 'MMM') {
                        result = months.namesAbbr[date.getMonth()];
                    } else if (match === 'MMMM') {
                        result = months.names[date.getMonth()];
                    } else if (match === 'yy') {
                        result = pad(date.getFullYear() % 100);
                    } else if (match === 'yyyy') {
                        result = pad(date.getFullYear(), 4);
                    } else if (match === 'h') {
                        result = date.getHours() % 12 || 12;
                    } else if (match === 'hh') {
                        result = pad(date.getHours() % 12 || 12);
                    } else if (match === 'H') {
                        result = date.getHours();
                    } else if (match === 'HH') {
                        result = pad(date.getHours());
                    } else if (match === 'm') {
                        result = date.getMinutes();
                    } else if (match === 'mm') {
                        result = pad(date.getMinutes());
                    } else if (match === 's') {
                        result = date.getSeconds();
                    } else if (match === 'ss') {
                        result = pad(date.getSeconds());
                    } else if (match === 'f') {
                        result = math.floor(date.getMilliseconds() / 100);
                    } else if (match === 'ff') {
                        result = date.getMilliseconds();
                        if (result > 99) {
                            result = math.floor(result / 10);
                        }
                        result = pad(result);
                    } else if (match === 'fff') {
                        result = pad(date.getMilliseconds(), 3);
                    } else if (match === 'tt') {
                        result = date.getHours() < 12 ? calendar.AM[0] : calendar.PM[0];
                    } else if (match === 'zzz') {
                        minutes = date.getTimezoneOffset();
                        sign = minutes < 0;
                        result = math.abs(minutes / 60).toString().split('.')[0];
                        minutes = math.abs(minutes) - result * 60;
                        result = (sign ? '+' : '-') + pad(result);
                        result += ':' + pad(minutes);
                    } else if (match === 'zz' || match === 'z') {
                        result = date.getTimezoneOffset() / 60;
                        sign = result < 0;
                        result = math.abs(result).toString().split('.')[0];
                        result = (sign ? '+' : '-') + (match === 'zz' ? pad(result) : result);
                    }
                    return result !== undefined ? result : match.slice(1, match.length - 1);
                });
            }
            function formatNumber(number, format, culture) {
                culture = getCulture(culture);
                var numberFormat = culture.numberFormat, decimal = numberFormat[POINT], precision = numberFormat.decimals, pattern = numberFormat.pattern[0], literals = [], symbol, isCurrency, isPercent, customPrecision, formatAndPrecision, negative = number < 0, integer, fraction, integerLength, fractionLength, replacement = EMPTY, value = EMPTY, idx, length, ch, hasGroup, hasNegativeFormat, decimalIndex, sharpIndex, zeroIndex, hasZero, hasSharp, percentIndex, currencyIndex, startZeroIndex, start = -1, end;
                if (number === undefined) {
                    return EMPTY;
                }
                if (!isFinite(number)) {
                    return number;
                }
                if (!format) {
                    return culture.name.length ? number.toLocaleString() : number.toString();
                }
                formatAndPrecision = standardFormatRegExp.exec(format);
                if (formatAndPrecision) {
                    format = formatAndPrecision[1].toLowerCase();
                    isCurrency = format === 'c';
                    isPercent = format === 'p';
                    if (isCurrency || isPercent) {
                        numberFormat = isCurrency ? numberFormat.currency : numberFormat.percent;
                        decimal = numberFormat[POINT];
                        precision = numberFormat.decimals;
                        symbol = numberFormat.symbol;
                        pattern = numberFormat.pattern[negative ? 0 : 1];
                    }
                    customPrecision = formatAndPrecision[2];
                    if (customPrecision) {
                        precision = +customPrecision;
                    }
                    if (format === 'e') {
                        return customPrecision ? number.toExponential(precision) : number.toExponential();
                    }
                    if (isPercent) {
                        number *= 100;
                    }
                    number = round(number, precision);
                    negative = number < 0;
                    number = number.split(POINT);
                    integer = number[0];
                    fraction = number[1];
                    if (negative) {
                        integer = integer.substring(1);
                    }
                    value = groupInteger(integer, 0, integer.length, numberFormat);
                    if (fraction) {
                        value += decimal + fraction;
                    }
                    if (format === 'n' && !negative) {
                        return value;
                    }
                    number = EMPTY;
                    for (idx = 0, length = pattern.length; idx < length; idx++) {
                        ch = pattern.charAt(idx);
                        if (ch === 'n') {
                            number += value;
                        } else if (ch === '$' || ch === '%') {
                            number += symbol;
                        } else {
                            number += ch;
                        }
                    }
                    return number;
                }
                if (negative) {
                    number = -number;
                }
                if (format.indexOf('\'') > -1 || format.indexOf('"') > -1 || format.indexOf('\\') > -1) {
                    format = format.replace(literalRegExp, function (match) {
                        var quoteChar = match.charAt(0).replace('\\', ''), literal = match.slice(1).replace(quoteChar, '');
                        literals.push(literal);
                        return PLACEHOLDER;
                    });
                }
                format = format.split(';');
                if (negative && format[1]) {
                    format = format[1];
                    hasNegativeFormat = true;
                } else if (number === 0) {
                    format = format[2] || format[0];
                    if (format.indexOf(SHARP) == -1 && format.indexOf(ZERO) == -1) {
                        return format;
                    }
                } else {
                    format = format[0];
                }
                percentIndex = format.indexOf('%');
                currencyIndex = format.indexOf('$');
                isPercent = percentIndex != -1;
                isCurrency = currencyIndex != -1;
                if (isPercent) {
                    number *= 100;
                }
                if (isCurrency && format[currencyIndex - 1] === '\\') {
                    format = format.split('\\').join('');
                    isCurrency = false;
                }
                if (isCurrency || isPercent) {
                    numberFormat = isCurrency ? numberFormat.currency : numberFormat.percent;
                    decimal = numberFormat[POINT];
                    precision = numberFormat.decimals;
                    symbol = numberFormat.symbol;
                }
                hasGroup = format.indexOf(COMMA) > -1;
                if (hasGroup) {
                    format = format.replace(commaRegExp, EMPTY);
                }
                decimalIndex = format.indexOf(POINT);
                length = format.length;
                if (decimalIndex != -1) {
                    fraction = number.toString().split('e');
                    if (fraction[1]) {
                        fraction = round(number, Math.abs(fraction[1]));
                    } else {
                        fraction = fraction[0];
                    }
                    fraction = fraction.split(POINT)[1] || EMPTY;
                    zeroIndex = format.lastIndexOf(ZERO) - decimalIndex;
                    sharpIndex = format.lastIndexOf(SHARP) - decimalIndex;
                    hasZero = zeroIndex > -1;
                    hasSharp = sharpIndex > -1;
                    idx = fraction.length;
                    if (!hasZero && !hasSharp) {
                        format = format.substring(0, decimalIndex) + format.substring(decimalIndex + 1);
                        length = format.length;
                        decimalIndex = -1;
                        idx = 0;
                    }
                    if (hasZero && zeroIndex > sharpIndex) {
                        idx = zeroIndex;
                    } else if (sharpIndex > zeroIndex) {
                        if (hasSharp && idx > sharpIndex) {
                            idx = sharpIndex;
                        } else if (hasZero && idx < zeroIndex) {
                            idx = zeroIndex;
                        }
                    }
                    if (idx > -1) {
                        number = round(number, idx);
                    }
                } else {
                    number = round(number);
                }
                sharpIndex = format.indexOf(SHARP);
                startZeroIndex = zeroIndex = format.indexOf(ZERO);
                if (sharpIndex == -1 && zeroIndex != -1) {
                    start = zeroIndex;
                } else if (sharpIndex != -1 && zeroIndex == -1) {
                    start = sharpIndex;
                } else {
                    start = sharpIndex > zeroIndex ? zeroIndex : sharpIndex;
                }
                sharpIndex = format.lastIndexOf(SHARP);
                zeroIndex = format.lastIndexOf(ZERO);
                if (sharpIndex == -1 && zeroIndex != -1) {
                    end = zeroIndex;
                } else if (sharpIndex != -1 && zeroIndex == -1) {
                    end = sharpIndex;
                } else {
                    end = sharpIndex > zeroIndex ? sharpIndex : zeroIndex;
                }
                if (start == length) {
                    end = start;
                }
                if (start != -1) {
                    value = number.toString().split(POINT);
                    integer = value[0];
                    fraction = value[1] || EMPTY;
                    integerLength = integer.length;
                    fractionLength = fraction.length;
                    if (negative && number * -1 >= 0) {
                        negative = false;
                    }
                    number = format.substring(0, start);
                    if (negative && !hasNegativeFormat) {
                        number += '-';
                    }
                    for (idx = start; idx < length; idx++) {
                        ch = format.charAt(idx);
                        if (decimalIndex == -1) {
                            if (end - idx < integerLength) {
                                number += integer;
                                break;
                            }
                        } else {
                            if (zeroIndex != -1 && zeroIndex < idx) {
                                replacement = EMPTY;
                            }
                            if (decimalIndex - idx <= integerLength && decimalIndex - idx > -1) {
                                number += integer;
                                idx = decimalIndex;
                            }
                            if (decimalIndex === idx) {
                                number += (fraction ? decimal : EMPTY) + fraction;
                                idx += end - decimalIndex + 1;
                                continue;
                            }
                        }
                        if (ch === ZERO) {
                            number += ch;
                            replacement = ch;
                        } else if (ch === SHARP) {
                            number += replacement;
                        }
                    }
                    if (hasGroup) {
                        number = groupInteger(number, start + (negative && !hasNegativeFormat ? 1 : 0), Math.max(end, integerLength + start), numberFormat);
                    }
                    if (end >= start) {
                        number += format.substring(end + 1);
                    }
                    if (isCurrency || isPercent) {
                        value = EMPTY;
                        for (idx = 0, length = number.length; idx < length; idx++) {
                            ch = number.charAt(idx);
                            value += ch === '$' || ch === '%' ? symbol : ch;
                        }
                        number = value;
                    }
                    length = literals.length;
                    if (length) {
                        for (idx = 0; idx < length; idx++) {
                            number = number.replace(PLACEHOLDER, literals[idx]);
                        }
                    }
                }
                return number;
            }
            var groupInteger = function (number, start, end, numberFormat) {
                var decimalIndex = number.indexOf(numberFormat[POINT]);
                var groupSizes = numberFormat.groupSize.slice();
                var groupSize = groupSizes.shift();
                var integer, integerLength;
                var idx, parts, value;
                var newGroupSize;
                end = decimalIndex !== -1 ? decimalIndex : end + 1;
                integer = number.substring(start, end);
                integerLength = integer.length;
                if (integerLength >= groupSize) {
                    idx = integerLength;
                    parts = [];
                    while (idx > -1) {
                        value = integer.substring(idx - groupSize, idx);
                        if (value) {
                            parts.push(value);
                        }
                        idx -= groupSize;
                        newGroupSize = groupSizes.shift();
                        groupSize = newGroupSize !== undefined ? newGroupSize : groupSize;
                        if (groupSize === 0) {
                            if (idx > 0) {
                                parts.push(integer.substring(0, idx));
                            }
                            break;
                        }
                    }
                    integer = parts.reverse().join(numberFormat[COMMA]);
                    number = number.substring(0, start) + integer + number.substring(end);
                }
                return number;
            };
            var round = function (value, precision) {
                precision = precision || 0;
                value = value.toString().split('e');
                value = Math.round(+(value[0] + 'e' + (value[1] ? +value[1] + precision : precision)));
                value = value.toString().split('e');
                value = +(value[0] + 'e' + (value[1] ? +value[1] - precision : -precision));
                return value.toFixed(Math.min(precision, 20));
            };
            var toString = function (value, fmt, culture) {
                if (fmt) {
                    if (objectToString.call(value) === '[object Date]') {
                        return formatDate(value, fmt, culture);
                    } else if (typeof value === NUMBER) {
                        return formatNumber(value, fmt, culture);
                    }
                }
                return value !== undefined ? value : '';
            };
            kendo.format = function (fmt) {
                var values = arguments;
                return fmt.replace(formatRegExp, function (match, index, placeholderFormat) {
                    var value = values[parseInt(index, 10) + 1];
                    return toString(value, placeholderFormat ? placeholderFormat.substring(1) : '');
                });
            };
            kendo._extractFormat = function (format) {
                if (format.slice(0, 3) === '{0:') {
                    format = format.slice(3, format.length - 1);
                }
                return format;
            };
            kendo._activeElement = function () {
                try {
                    return document.activeElement;
                } catch (e) {
                    return document.documentElement.activeElement;
                }
            };
            kendo._round = round;
            kendo._outerWidth = function (element, includeMargin) {
                return $(element).outerWidth(includeMargin || false) || 0;
            };
            kendo._outerHeight = function (element, includeMargin) {
                return $(element).outerHeight(includeMargin || false) || 0;
            };
            kendo.toString = toString;
        }());
        (function () {
            var nonBreakingSpaceRegExp = /\u00A0/g, exponentRegExp = /[eE][\-+]?[0-9]+/, shortTimeZoneRegExp = /[+|\-]\d{1,2}/, longTimeZoneRegExp = /[+|\-]\d{1,2}:?\d{2}/, dateRegExp = /^\/Date\((.*?)\)\/$/, offsetRegExp = /[+-]\d*/, FORMATS_SEQUENCE = [
                    [],
                    [
                        'G',
                        'g',
                        'F'
                    ],
                    [
                        'D',
                        'd',
                        'y',
                        'm',
                        'T',
                        't'
                    ]
                ], STANDARD_FORMATS = [
                    [
                        'yyyy-MM-ddTHH:mm:ss.fffffffzzz',
                        'yyyy-MM-ddTHH:mm:ss.fffffff',
                        'yyyy-MM-ddTHH:mm:ss.fffzzz',
                        'yyyy-MM-ddTHH:mm:ss.fff',
                        'ddd MMM dd yyyy HH:mm:ss',
                        'yyyy-MM-ddTHH:mm:sszzz',
                        'yyyy-MM-ddTHH:mmzzz',
                        'yyyy-MM-ddTHH:mmzz',
                        'yyyy-MM-ddTHH:mm:ss',
                        'yyyy-MM-dd HH:mm:ss',
                        'yyyy/MM/dd HH:mm:ss'
                    ],
                    [
                        'yyyy-MM-ddTHH:mm',
                        'yyyy-MM-dd HH:mm',
                        'yyyy/MM/dd HH:mm'
                    ],
                    [
                        'yyyy/MM/dd',
                        'yyyy-MM-dd',
                        'HH:mm:ss',
                        'HH:mm'
                    ]
                ], numberRegExp = {
                    2: /^\d{1,2}/,
                    3: /^\d{1,3}/,
                    4: /^\d{4}/
                }, objectToString = {}.toString;
            function outOfRange(value, start, end) {
                return !(value >= start && value <= end);
            }
            function designatorPredicate(designator) {
                return designator.charAt(0);
            }
            function mapDesignators(designators) {
                return $.map(designators, designatorPredicate);
            }
            function adjustDST(date, hours) {
                if (!hours && date.getHours() === 23) {
                    date.setHours(date.getHours() + 2);
                }
            }
            function lowerArray(data) {
                var idx = 0, length = data.length, array = [];
                for (; idx < length; idx++) {
                    array[idx] = (data[idx] + '').toLowerCase();
                }
                return array;
            }
            function lowerLocalInfo(localInfo) {
                var newLocalInfo = {}, property;
                for (property in localInfo) {
                    newLocalInfo[property] = lowerArray(localInfo[property]);
                }
                return newLocalInfo;
            }
            function parseExact(value, format, culture, strict) {
                if (!value) {
                    return null;
                }
                var lookAhead = function (match) {
                        var i = 0;
                        while (format[idx] === match) {
                            i++;
                            idx++;
                        }
                        if (i > 0) {
                            idx -= 1;
                        }
                        return i;
                    }, getNumber = function (size) {
                        var rg = numberRegExp[size] || new RegExp('^\\d{1,' + size + '}'), match = value.substr(valueIdx, size).match(rg);
                        if (match) {
                            match = match[0];
                            valueIdx += match.length;
                            return parseInt(match, 10);
                        }
                        return null;
                    }, getIndexByName = function (names, lower) {
                        var i = 0, length = names.length, name, nameLength, matchLength = 0, matchIdx = 0, subValue;
                        for (; i < length; i++) {
                            name = names[i];
                            nameLength = name.length;
                            subValue = value.substr(valueIdx, nameLength);
                            if (lower) {
                                subValue = subValue.toLowerCase();
                            }
                            if (subValue == name && nameLength > matchLength) {
                                matchLength = nameLength;
                                matchIdx = i;
                            }
                        }
                        if (matchLength) {
                            valueIdx += matchLength;
                            return matchIdx + 1;
                        }
                        return null;
                    }, checkLiteral = function () {
                        var result = false;
                        if (value.charAt(valueIdx) === format[idx]) {
                            valueIdx++;
                            result = true;
                        }
                        return result;
                    }, calendar = culture.calendars.standard, year = null, month = null, day = null, hours = null, minutes = null, seconds = null, milliseconds = null, idx = 0, valueIdx = 0, literal = false, date = new Date(), twoDigitYearMax = calendar.twoDigitYearMax || 2029, defaultYear = date.getFullYear(), ch, count, length, pattern, pmHour, UTC, matches, amDesignators, pmDesignators, hoursOffset, minutesOffset, hasTime, match;
                if (!format) {
                    format = 'd';
                }
                pattern = calendar.patterns[format];
                if (pattern) {
                    format = pattern;
                }
                format = format.split('');
                length = format.length;
                for (; idx < length; idx++) {
                    ch = format[idx];
                    if (literal) {
                        if (ch === '\'') {
                            literal = false;
                        } else {
                            checkLiteral();
                        }
                    } else {
                        if (ch === 'd') {
                            count = lookAhead('d');
                            if (!calendar._lowerDays) {
                                calendar._lowerDays = lowerLocalInfo(calendar.days);
                            }
                            if (day !== null && count > 2) {
                                continue;
                            }
                            day = count < 3 ? getNumber(2) : getIndexByName(calendar._lowerDays[count == 3 ? 'namesAbbr' : 'names'], true);
                            if (day === null || outOfRange(day, 1, 31)) {
                                return null;
                            }
                        } else if (ch === 'M') {
                            count = lookAhead('M');
                            if (!calendar._lowerMonths) {
                                calendar._lowerMonths = lowerLocalInfo(calendar.months);
                            }
                            month = count < 3 ? getNumber(2) : getIndexByName(calendar._lowerMonths[count == 3 ? 'namesAbbr' : 'names'], true);
                            if (month === null || outOfRange(month, 1, 12)) {
                                return null;
                            }
                            month -= 1;
                        } else if (ch === 'y') {
                            count = lookAhead('y');
                            year = getNumber(count);
                            if (year === null) {
                                return null;
                            }
                            if (count == 2) {
                                if (typeof twoDigitYearMax === 'string') {
                                    twoDigitYearMax = defaultYear + parseInt(twoDigitYearMax, 10);
                                }
                                year = defaultYear - defaultYear % 100 + year;
                                if (year > twoDigitYearMax) {
                                    year -= 100;
                                }
                            }
                        } else if (ch === 'h') {
                            lookAhead('h');
                            hours = getNumber(2);
                            if (hours == 12) {
                                hours = 0;
                            }
                            if (hours === null || outOfRange(hours, 0, 11)) {
                                return null;
                            }
                        } else if (ch === 'H') {
                            lookAhead('H');
                            hours = getNumber(2);
                            if (hours === null || outOfRange(hours, 0, 23)) {
                                return null;
                            }
                        } else if (ch === 'm') {
                            lookAhead('m');
                            minutes = getNumber(2);
                            if (minutes === null || outOfRange(minutes, 0, 59)) {
                                return null;
                            }
                        } else if (ch === 's') {
                            lookAhead('s');
                            seconds = getNumber(2);
                            if (seconds === null || outOfRange(seconds, 0, 59)) {
                                return null;
                            }
                        } else if (ch === 'f') {
                            count = lookAhead('f');
                            match = value.substr(valueIdx, count).match(numberRegExp[3]);
                            milliseconds = getNumber(count);
                            if (milliseconds !== null) {
                                milliseconds = parseFloat('0.' + match[0], 10);
                                milliseconds = kendo._round(milliseconds, 3);
                                milliseconds *= 1000;
                            }
                            if (milliseconds === null || outOfRange(milliseconds, 0, 999)) {
                                return null;
                            }
                        } else if (ch === 't') {
                            count = lookAhead('t');
                            amDesignators = calendar.AM;
                            pmDesignators = calendar.PM;
                            if (count === 1) {
                                amDesignators = mapDesignators(amDesignators);
                                pmDesignators = mapDesignators(pmDesignators);
                            }
                            pmHour = getIndexByName(pmDesignators);
                            if (!pmHour && !getIndexByName(amDesignators)) {
                                return null;
                            }
                        } else if (ch === 'z') {
                            UTC = true;
                            count = lookAhead('z');
                            if (value.substr(valueIdx, 1) === 'Z') {
                                checkLiteral();
                                continue;
                            }
                            matches = value.substr(valueIdx, 6).match(count > 2 ? longTimeZoneRegExp : shortTimeZoneRegExp);
                            if (!matches) {
                                return null;
                            }
                            matches = matches[0].split(':');
                            hoursOffset = matches[0];
                            minutesOffset = matches[1];
                            if (!minutesOffset && hoursOffset.length > 3) {
                                valueIdx = hoursOffset.length - 2;
                                minutesOffset = hoursOffset.substring(valueIdx);
                                hoursOffset = hoursOffset.substring(0, valueIdx);
                            }
                            hoursOffset = parseInt(hoursOffset, 10);
                            if (outOfRange(hoursOffset, -12, 13)) {
                                return null;
                            }
                            if (count > 2) {
                                minutesOffset = matches[0][0] + minutesOffset;
                                minutesOffset = parseInt(minutesOffset, 10);
                                if (isNaN(minutesOffset) || outOfRange(minutesOffset, -59, 59)) {
                                    return null;
                                }
                            }
                        } else if (ch === '\'') {
                            literal = true;
                            checkLiteral();
                        } else if (!checkLiteral()) {
                            return null;
                        }
                    }
                }
                if (strict && !/^\s*$/.test(value.substr(valueIdx))) {
                    return null;
                }
                hasTime = hours !== null || minutes !== null || seconds || null;
                if (year === null && month === null && day === null && hasTime) {
                    year = defaultYear;
                    month = date.getMonth();
                    day = date.getDate();
                } else {
                    if (year === null) {
                        year = defaultYear;
                    }
                    if (day === null) {
                        day = 1;
                    }
                }
                if (pmHour && hours < 12) {
                    hours += 12;
                }
                if (UTC) {
                    if (hoursOffset) {
                        hours += -hoursOffset;
                    }
                    if (minutesOffset) {
                        minutes += -minutesOffset;
                    }
                    value = new Date(Date.UTC(year, month, day, hours, minutes, seconds, milliseconds));
                } else {
                    value = new Date(year, month, day, hours, minutes, seconds, milliseconds);
                    adjustDST(value, hours);
                }
                if (year < 100) {
                    value.setFullYear(year);
                }
                if (value.getDate() !== day && UTC === undefined) {
                    return null;
                }
                return value;
            }
            function parseMicrosoftFormatOffset(offset) {
                var sign = offset.substr(0, 1) === '-' ? -1 : 1;
                offset = offset.substring(1);
                offset = parseInt(offset.substr(0, 2), 10) * 60 + parseInt(offset.substring(2), 10);
                return sign * offset;
            }
            function getDefaultFormats(culture) {
                var length = math.max(FORMATS_SEQUENCE.length, STANDARD_FORMATS.length);
                var patterns = culture.calendar.patterns;
                var cultureFormats, formatIdx, idx;
                var formats = [];
                for (idx = 0; idx < length; idx++) {
                    cultureFormats = FORMATS_SEQUENCE[idx];
                    for (formatIdx = 0; formatIdx < cultureFormats.length; formatIdx++) {
                        formats.push(patterns[cultureFormats[formatIdx]]);
                    }
                    formats = formats.concat(STANDARD_FORMATS[idx]);
                }
                return formats;
            }
            function internalParseDate(value, formats, culture, strict) {
                if (objectToString.call(value) === '[object Date]') {
                    return value;
                }
                var idx = 0;
                var date = null;
                var length;
                var tzoffset;
                if (value && value.indexOf('/D') === 0) {
                    date = dateRegExp.exec(value);
                    if (date) {
                        date = date[1];
                        tzoffset = offsetRegExp.exec(date.substring(1));
                        date = new Date(parseInt(date, 10));
                        if (tzoffset) {
                            tzoffset = parseMicrosoftFormatOffset(tzoffset[0]);
                            date = kendo.timezone.apply(date, 0);
                            date = kendo.timezone.convert(date, 0, -1 * tzoffset);
                        }
                        return date;
                    }
                }
                culture = kendo.getCulture(culture);
                if (!formats) {
                    formats = getDefaultFormats(culture);
                }
                formats = isArray(formats) ? formats : [formats];
                length = formats.length;
                for (; idx < length; idx++) {
                    date = parseExact(value, formats[idx], culture, strict);
                    if (date) {
                        return date;
                    }
                }
                return date;
            }
            kendo.parseDate = function (value, formats, culture) {
                return internalParseDate(value, formats, culture, false);
            };
            kendo.parseExactDate = function (value, formats, culture) {
                return internalParseDate(value, formats, culture, true);
            };
            kendo.parseInt = function (value, culture) {
                var result = kendo.parseFloat(value, culture);
                if (result) {
                    result = result | 0;
                }
                return result;
            };
            kendo.parseFloat = function (value, culture, format) {
                if (!value && value !== 0) {
                    return null;
                }
                if (typeof value === NUMBER) {
                    return value;
                }
                value = value.toString();
                culture = kendo.getCulture(culture);
                var number = culture.numberFormat, percent = number.percent, currency = number.currency, symbol = currency.symbol, percentSymbol = percent.symbol, negative = value.indexOf('-'), parts, isPercent;
                if (exponentRegExp.test(value)) {
                    value = parseFloat(value.replace(number['.'], '.'));
                    if (isNaN(value)) {
                        value = null;
                    }
                    return value;
                }
                if (negative > 0) {
                    return null;
                } else {
                    negative = negative > -1;
                }
                if (value.indexOf(symbol) > -1 || format && format.toLowerCase().indexOf('c') > -1) {
                    number = currency;
                    parts = number.pattern[0].replace('$', symbol).split('n');
                    if (value.indexOf(parts[0]) > -1 && value.indexOf(parts[1]) > -1) {
                        value = value.replace(parts[0], '').replace(parts[1], '');
                        negative = true;
                    }
                } else if (value.indexOf(percentSymbol) > -1) {
                    isPercent = true;
                    number = percent;
                    symbol = percentSymbol;
                }
                value = value.replace('-', '').replace(symbol, '').replace(nonBreakingSpaceRegExp, ' ').split(number[','].replace(nonBreakingSpaceRegExp, ' ')).join('').replace(number['.'], '.');
                value = parseFloat(value);
                if (isNaN(value)) {
                    value = null;
                } else if (negative) {
                    value *= -1;
                }
                if (value && isPercent) {
                    value /= 100;
                }
                return value;
            };
        }());
        function getShadows(element) {
            var shadow = element.css(kendo.support.transitions.css + 'box-shadow') || element.css('box-shadow'), radius = shadow ? shadow.match(boxShadowRegExp) || [
                    0,
                    0,
                    0,
                    0,
                    0
                ] : [
                    0,
                    0,
                    0,
                    0,
                    0
                ], blur = math.max(+radius[3], +(radius[4] || 0));
            return {
                left: -radius[1] + blur,
                right: +radius[1] + blur,
                bottom: +radius[2] + blur
            };
        }
        function wrap(element, autosize) {
            var browser = support.browser, percentage, outerWidth = kendo._outerWidth, outerHeight = kendo._outerHeight;
            if (!element.parent().hasClass('k-animation-container')) {
                var width = element[0].style.width, height = element[0].style.height, percentWidth = percentRegExp.test(width), percentHeight = percentRegExp.test(height);
                percentage = percentWidth || percentHeight;
                if (!percentWidth && (!autosize || autosize && width)) {
                    width = autosize ? outerWidth(element) + 1 : outerWidth(element);
                }
                if (!percentHeight && (!autosize || autosize && height)) {
                    height = outerHeight(element);
                }
                element.wrap($('<div/>').addClass('k-animation-container').css({
                    width: width,
                    height: height
                }));
                if (percentage) {
                    element.css({
                        width: '100%',
                        height: '100%',
                        boxSizing: 'border-box',
                        mozBoxSizing: 'border-box',
                        webkitBoxSizing: 'border-box'
                    });
                }
            } else {
                var wrapper = element.parent('.k-animation-container'), wrapperStyle = wrapper[0].style;
                if (wrapper.is(':hidden')) {
                    wrapper.css({
                        display: '',
                        position: ''
                    });
                }
                percentage = percentRegExp.test(wrapperStyle.width) || percentRegExp.test(wrapperStyle.height);
                if (!percentage) {
                    wrapper.css({
                        width: autosize ? outerWidth(element) + 1 : outerWidth(element),
                        height: outerHeight(element),
                        boxSizing: 'content-box',
                        mozBoxSizing: 'content-box',
                        webkitBoxSizing: 'content-box'
                    });
                }
            }
            if (browser.msie && math.floor(browser.version) <= 7) {
                element.css({ zoom: 1 });
                element.children('.k-menu').width(element.width());
            }
            return element.parent();
        }
        function deepExtend(destination) {
            var i = 1, length = arguments.length;
            for (i = 1; i < length; i++) {
                deepExtendOne(destination, arguments[i]);
            }
            return destination;
        }
        function deepExtendOne(destination, source) {
            var ObservableArray = kendo.data.ObservableArray, LazyObservableArray = kendo.data.LazyObservableArray, DataSource = kendo.data.DataSource, HierarchicalDataSource = kendo.data.HierarchicalDataSource, property, propValue, propType, propInit, destProp;
            for (property in source) {
                propValue = source[property];
                propType = typeof propValue;
                if (propType === OBJECT && propValue !== null) {
                    propInit = propValue.constructor;
                } else {
                    propInit = null;
                }
                if (propInit && propInit !== Array && propInit !== ObservableArray && propInit !== LazyObservableArray && propInit !== DataSource && propInit !== HierarchicalDataSource && propInit !== RegExp) {
                    if (propValue instanceof Date) {
                        destination[property] = new Date(propValue.getTime());
                    } else if (isFunction(propValue.clone)) {
                        destination[property] = propValue.clone();
                    } else {
                        destProp = destination[property];
                        if (typeof destProp === OBJECT) {
                            destination[property] = destProp || {};
                        } else {
                            destination[property] = {};
                        }
                        deepExtendOne(destination[property], propValue);
                    }
                } else if (propType !== UNDEFINED) {
                    destination[property] = propValue;
                }
            }
            return destination;
        }
        function testRx(agent, rxs, dflt) {
            for (var rx in rxs) {
                if (rxs.hasOwnProperty(rx) && rxs[rx].test(agent)) {
                    return rx;
                }
            }
            return dflt !== undefined ? dflt : agent;
        }
        function toHyphens(str) {
            return str.replace(/([a-z][A-Z])/g, function (g) {
                return g.charAt(0) + '-' + g.charAt(1).toLowerCase();
            });
        }
        function toCamelCase(str) {
            return str.replace(/\-(\w)/g, function (strMatch, g1) {
                return g1.toUpperCase();
            });
        }
        function getComputedStyles(element, properties) {
            var styles = {}, computedStyle;
            if (document.defaultView && document.defaultView.getComputedStyle) {
                computedStyle = document.defaultView.getComputedStyle(element, '');
                if (properties) {
                    $.each(properties, function (idx, value) {
                        styles[value] = computedStyle.getPropertyValue(value);
                    });
                }
            } else {
                computedStyle = element.currentStyle;
                if (properties) {
                    $.each(properties, function (idx, value) {
                        styles[value] = computedStyle[toCamelCase(value)];
                    });
                }
            }
            if (!kendo.size(styles)) {
                styles = computedStyle;
            }
            return styles;
        }
        function isScrollable(element) {
            if (element && element.className && typeof element.className === 'string' && element.className.indexOf('k-auto-scrollable') > -1) {
                return true;
            }
            var overflow = getComputedStyles(element, ['overflow']).overflow;
            return overflow == 'auto' || overflow == 'scroll';
        }
        function scrollLeft(element, value) {
            var webkit = support.browser.webkit;
            var mozila = support.browser.mozilla;
            var el = element instanceof $ ? element[0] : element;
            var isRtl;
            if (!element) {
                return;
            }
            isRtl = support.isRtl(element);
            if (value !== undefined) {
                if (isRtl && webkit) {
                    el.scrollLeft = el.scrollWidth - el.clientWidth - value;
                } else if (isRtl && mozila) {
                    el.scrollLeft = -value;
                } else {
                    el.scrollLeft = value;
                }
            } else {
                if (isRtl && webkit) {
                    return el.scrollWidth - el.clientWidth - el.scrollLeft;
                } else {
                    return Math.abs(el.scrollLeft);
                }
            }
        }
        (function () {
            support._scrollbar = undefined;
            support.scrollbar = function (refresh) {
                if (!isNaN(support._scrollbar) && !refresh) {
                    return support._scrollbar;
                } else {
                    var div = document.createElement('div'), result;
                    div.style.cssText = 'overflow:scroll;overflow-x:hidden;zoom:1;clear:both;display:block';
                    div.innerHTML = '&nbsp;';
                    document.body.appendChild(div);
                    support._scrollbar = result = div.offsetWidth - div.scrollWidth;
                    document.body.removeChild(div);
                    return result;
                }
            };
            support.isRtl = function (element) {
                return $(element).closest('.k-rtl').length > 0;
            };
            var table = document.createElement('table');
            try {
                table.innerHTML = '<tr><td></td></tr>';
                support.tbodyInnerHtml = true;
            } catch (e) {
                support.tbodyInnerHtml = false;
            }
            support.touch = 'ontouchstart' in window;
            var docStyle = document.documentElement.style;
            var transitions = support.transitions = false, transforms = support.transforms = false, elementProto = 'HTMLElement' in window ? HTMLElement.prototype : [];
            support.hasHW3D = 'WebKitCSSMatrix' in window && 'm11' in new window.WebKitCSSMatrix() || 'MozPerspective' in docStyle || 'msPerspective' in docStyle;
            support.cssFlexbox = 'flexWrap' in docStyle || 'WebkitFlexWrap' in docStyle || 'msFlexWrap' in docStyle;
            each([
                'Moz',
                'webkit',
                'O',
                'ms'
            ], function () {
                var prefix = this.toString(), hasTransitions = typeof table.style[prefix + 'Transition'] === STRING;
                if (hasTransitions || typeof table.style[prefix + 'Transform'] === STRING) {
                    var lowPrefix = prefix.toLowerCase();
                    transforms = {
                        css: lowPrefix != 'ms' ? '-' + lowPrefix + '-' : '',
                        prefix: prefix,
                        event: lowPrefix === 'o' || lowPrefix === 'webkit' ? lowPrefix : ''
                    };
                    if (hasTransitions) {
                        transitions = transforms;
                        transitions.event = transitions.event ? transitions.event + 'TransitionEnd' : 'transitionend';
                    }
                    return false;
                }
            });
            table = null;
            support.transforms = transforms;
            support.transitions = transitions;
            support.devicePixelRatio = window.devicePixelRatio === undefined ? 1 : window.devicePixelRatio;
            try {
                support.screenWidth = window.outerWidth || window.screen ? window.screen.availWidth : window.innerWidth;
                support.screenHeight = window.outerHeight || window.screen ? window.screen.availHeight : window.innerHeight;
            } catch (e) {
                support.screenWidth = window.screen.availWidth;
                support.screenHeight = window.screen.availHeight;
            }
            support.detectOS = function (ua) {
                var os = false, minorVersion, match = [], notAndroidPhone = !/mobile safari/i.test(ua), agentRxs = {
                        wp: /(Windows Phone(?: OS)?)\s(\d+)\.(\d+(\.\d+)?)/,
                        fire: /(Silk)\/(\d+)\.(\d+(\.\d+)?)/,
                        android: /(Android|Android.*(?:Opera|Firefox).*?\/)\s*(\d+)\.(\d+(\.\d+)?)/,
                        iphone: /(iPhone|iPod).*OS\s+(\d+)[\._]([\d\._]+)/,
                        ipad: /(iPad).*OS\s+(\d+)[\._]([\d_]+)/,
                        meego: /(MeeGo).+NokiaBrowser\/(\d+)\.([\d\._]+)/,
                        webos: /(webOS)\/(\d+)\.(\d+(\.\d+)?)/,
                        blackberry: /(BlackBerry|BB10).*?Version\/(\d+)\.(\d+(\.\d+)?)/,
                        playbook: /(PlayBook).*?Tablet\s*OS\s*(\d+)\.(\d+(\.\d+)?)/,
                        windows: /(MSIE)\s+(\d+)\.(\d+(\.\d+)?)/,
                        tizen: /(tizen).*?Version\/(\d+)\.(\d+(\.\d+)?)/i,
                        sailfish: /(sailfish).*rv:(\d+)\.(\d+(\.\d+)?).*firefox/i,
                        ffos: /(Mobile).*rv:(\d+)\.(\d+(\.\d+)?).*Firefox/
                    }, osRxs = {
                        ios: /^i(phone|pad|pod)$/i,
                        android: /^android|fire$/i,
                        blackberry: /^blackberry|playbook/i,
                        windows: /windows/,
                        wp: /wp/,
                        flat: /sailfish|ffos|tizen/i,
                        meego: /meego/
                    }, formFactorRxs = { tablet: /playbook|ipad|fire/i }, browserRxs = {
                        omini: /Opera\sMini/i,
                        omobile: /Opera\sMobi/i,
                        firefox: /Firefox|Fennec/i,
                        mobilesafari: /version\/.*safari/i,
                        ie: /MSIE|Windows\sPhone/i,
                        chrome: /chrome|crios/i,
                        webkit: /webkit/i
                    };
                for (var agent in agentRxs) {
                    if (agentRxs.hasOwnProperty(agent)) {
                        match = ua.match(agentRxs[agent]);
                        if (match) {
                            if (agent == 'windows' && 'plugins' in navigator) {
                                return false;
                            }
                            os = {};
                            os.device = agent;
                            os.tablet = testRx(agent, formFactorRxs, false);
                            os.browser = testRx(ua, browserRxs, 'default');
                            os.name = testRx(agent, osRxs);
                            os[os.name] = true;
                            os.majorVersion = match[2];
                            os.minorVersion = match[3].replace('_', '.');
                            minorVersion = os.minorVersion.replace('.', '').substr(0, 2);
                            os.flatVersion = os.majorVersion + minorVersion + new Array(3 - (minorVersion.length < 3 ? minorVersion.length : 2)).join('0');
                            os.cordova = typeof window.PhoneGap !== UNDEFINED || typeof window.cordova !== UNDEFINED;
                            os.appMode = window.navigator.standalone || /file|local|wmapp/.test(window.location.protocol) || os.cordova;
                            if (os.android && (support.devicePixelRatio < 1.5 && os.flatVersion < 400 || notAndroidPhone) && (support.screenWidth > 800 || support.screenHeight > 800)) {
                                os.tablet = agent;
                            }
                            break;
                        }
                    }
                }
                return os;
            };
            var mobileOS = support.mobileOS = support.detectOS(navigator.userAgent);
            support.wpDevicePixelRatio = mobileOS.wp ? screen.width / 320 : 0;
            support.hasNativeScrolling = false;
            if (mobileOS.ios || mobileOS.android && mobileOS.majorVersion > 2 || mobileOS.wp) {
                support.hasNativeScrolling = mobileOS;
            }
            support.delayedClick = function () {
                if (support.touch) {
                    if (mobileOS.ios) {
                        return true;
                    }
                    if (mobileOS.android) {
                        if (!support.browser.chrome) {
                            return true;
                        }
                        if (support.browser.version < 32) {
                            return false;
                        }
                        return !($('meta[name=viewport]').attr('content') || '').match(/user-scalable=no/i);
                    }
                }
                return false;
            };
            support.mouseAndTouchPresent = support.touch && !(support.mobileOS.ios || support.mobileOS.android);
            support.detectBrowser = function (ua) {
                var browser = false, match = [], browserRxs = {
                        edge: /(edge)[ \/]([\w.]+)/i,
                        webkit: /(chrome)[ \/]([\w.]+)/i,
                        safari: /(webkit)[ \/]([\w.]+)/i,
                        opera: /(opera)(?:.*version|)[ \/]([\w.]+)/i,
                        msie: /(msie\s|trident.*? rv:)([\w.]+)/i,
                        mozilla: /(mozilla)(?:.*? rv:([\w.]+)|)/i
                    };
                for (var agent in browserRxs) {
                    if (browserRxs.hasOwnProperty(agent)) {
                        match = ua.match(browserRxs[agent]);
                        if (match) {
                            browser = {};
                            browser[agent] = true;
                            browser[match[1].toLowerCase().split(' ')[0].split('/')[0]] = true;
                            browser.version = parseInt(document.documentMode || match[2], 10);
                            break;
                        }
                    }
                }
                return browser;
            };
            support.browser = support.detectBrowser(navigator.userAgent);
            support.detectClipboardAccess = function () {
                var commands = {
                    copy: document.queryCommandSupported ? document.queryCommandSupported('copy') : false,
                    cut: document.queryCommandSupported ? document.queryCommandSupported('cut') : false,
                    paste: document.queryCommandSupported ? document.queryCommandSupported('paste') : false
                };
                if (support.browser.chrome) {
                    commands.paste = false;
                    if (support.browser.version >= 43) {
                        commands.copy = true;
                        commands.cut = true;
                    }
                }
                return commands;
            };
            support.clipboard = support.detectClipboardAccess();
            support.zoomLevel = function () {
                try {
                    var browser = support.browser;
                    var ie11WidthCorrection = 0;
                    var docEl = document.documentElement;
                    if (browser.msie && browser.version == 11 && docEl.scrollHeight > docEl.clientHeight && !support.touch) {
                        ie11WidthCorrection = support.scrollbar();
                    }
                    return support.touch ? docEl.clientWidth / window.innerWidth : browser.msie && browser.version >= 10 ? ((top || window).document.documentElement.offsetWidth + ie11WidthCorrection) / (top || window).innerWidth : 1;
                } catch (e) {
                    return 1;
                }
            };
            support.cssBorderSpacing = typeof docStyle.borderSpacing != 'undefined' && !(support.browser.msie && support.browser.version < 8);
            (function (browser) {
                var cssClass = '', docElement = $(document.documentElement), majorVersion = parseInt(browser.version, 10);
                if (browser.msie) {
                    cssClass = 'ie';
                } else if (browser.mozilla) {
                    cssClass = 'ff';
                } else if (browser.safari) {
                    cssClass = 'safari';
                } else if (browser.webkit) {
                    cssClass = 'webkit';
                } else if (browser.opera) {
                    cssClass = 'opera';
                } else if (browser.edge) {
                    cssClass = 'edge';
                }
                if (cssClass) {
                    cssClass = 'k-' + cssClass + ' k-' + cssClass + majorVersion;
                }
                if (support.mobileOS) {
                    cssClass += ' k-mobile';
                }
                if (!support.cssFlexbox) {
                    cssClass += ' k-no-flexbox';
                }
                docElement.addClass(cssClass);
            }(support.browser));
            support.eventCapture = document.documentElement.addEventListener;
            var input = document.createElement('input');
            support.placeholder = 'placeholder' in input;
            support.propertyChangeEvent = 'onpropertychange' in input;
            support.input = function () {
                var types = [
                    'number',
                    'date',
                    'time',
                    'month',
                    'week',
                    'datetime',
                    'datetime-local'
                ];
                var length = types.length;
                var value = 'test';
                var result = {};
                var idx = 0;
                var type;
                for (; idx < length; idx++) {
                    type = types[idx];
                    input.setAttribute('type', type);
                    input.value = value;
                    result[type.replace('-', '')] = input.type !== 'text' && input.value !== value;
                }
                return result;
            }();
            input.style.cssText = 'float:left;';
            support.cssFloat = !!input.style.cssFloat;
            input = null;
            support.stableSort = function () {
                var threshold = 513;
                var sorted = [{
                        index: 0,
                        field: 'b'
                    }];
                for (var i = 1; i < threshold; i++) {
                    sorted.push({
                        index: i,
                        field: 'a'
                    });
                }
                sorted.sort(function (a, b) {
                    return a.field > b.field ? 1 : a.field < b.field ? -1 : 0;
                });
                return sorted[0].index === 1;
            }();
            support.matchesSelector = elementProto.webkitMatchesSelector || elementProto.mozMatchesSelector || elementProto.msMatchesSelector || elementProto.oMatchesSelector || elementProto.matchesSelector || elementProto.matches || function (selector) {
                var nodeList = document.querySelectorAll ? (this.parentNode || document).querySelectorAll(selector) || [] : $(selector), i = nodeList.length;
                while (i--) {
                    if (nodeList[i] == this) {
                        return true;
                    }
                }
                return false;
            };
            support.pushState = window.history && window.history.pushState;
            var documentMode = document.documentMode;
            support.hashChange = 'onhashchange' in window && !(support.browser.msie && (!documentMode || documentMode <= 8));
            support.customElements = 'registerElement' in window.document;
            var chrome = support.browser.chrome, mozilla = support.browser.mozilla;
            support.msPointers = !chrome && window.MSPointerEvent;
            support.pointers = !chrome && !mozilla && window.PointerEvent;
            support.kineticScrollNeeded = mobileOS && (support.touch || support.msPointers || support.pointers);
        }());
        function size(obj) {
            var result = 0, key;
            for (key in obj) {
                if (obj.hasOwnProperty(key) && key != 'toJSON') {
                    result++;
                }
            }
            return result;
        }
        function getOffset(element, type, positioned) {
            if (!type) {
                type = 'offset';
            }
            var offset = element[type]();
            var result = {
                top: offset.top,
                right: offset.right,
                bottom: offset.bottom,
                left: offset.left
            };
            if (support.browser.msie && (support.pointers || support.msPointers) && !positioned) {
                var sign = support.isRtl(element) ? 1 : -1;
                result.top -= window.pageYOffset - document.documentElement.scrollTop;
                result.left -= window.pageXOffset + sign * document.documentElement.scrollLeft;
            }
            return result;
        }
        var directions = {
            left: { reverse: 'right' },
            right: { reverse: 'left' },
            down: { reverse: 'up' },
            up: { reverse: 'down' },
            top: { reverse: 'bottom' },
            bottom: { reverse: 'top' },
            'in': { reverse: 'out' },
            out: { reverse: 'in' }
        };
        function parseEffects(input) {
            var effects = {};
            each(typeof input === 'string' ? input.split(' ') : input, function (idx) {
                effects[idx] = this;
            });
            return effects;
        }
        function fx(element) {
            return new kendo.effects.Element(element);
        }
        var effects = {};
        $.extend(effects, {
            enabled: true,
            Element: function (element) {
                this.element = $(element);
            },
            promise: function (element, options) {
                if (!element.is(':visible')) {
                    element.css({ display: element.data('olddisplay') || 'block' }).css('display');
                }
                if (options.hide) {
                    element.data('olddisplay', element.css('display')).hide();
                }
                if (options.init) {
                    options.init();
                }
                if (options.completeCallback) {
                    options.completeCallback(element);
                }
                element.dequeue();
            },
            disable: function () {
                this.enabled = false;
                this.promise = this.promiseShim;
            },
            enable: function () {
                this.enabled = true;
                this.promise = this.animatedPromise;
            }
        });
        effects.promiseShim = effects.promise;
        function prepareAnimationOptions(options, duration, reverse, complete) {
            if (typeof options === STRING) {
                if (isFunction(duration)) {
                    complete = duration;
                    duration = 400;
                    reverse = false;
                }
                if (isFunction(reverse)) {
                    complete = reverse;
                    reverse = false;
                }
                if (typeof duration === BOOLEAN) {
                    reverse = duration;
                    duration = 400;
                }
                options = {
                    effects: options,
                    duration: duration,
                    reverse: reverse,
                    complete: complete
                };
            }
            return extend({
                effects: {},
                duration: 400,
                reverse: false,
                init: noop,
                teardown: noop,
                hide: false
            }, options, {
                completeCallback: options.complete,
                complete: noop
            });
        }
        function animate(element, options, duration, reverse, complete) {
            var idx = 0, length = element.length, instance;
            for (; idx < length; idx++) {
                instance = $(element[idx]);
                instance.queue(function () {
                    effects.promise(instance, prepareAnimationOptions(options, duration, reverse, complete));
                });
            }
            return element;
        }
        function toggleClass(element, classes, options, add) {
            if (classes) {
                classes = classes.split(' ');
                each(classes, function (idx, value) {
                    element.toggleClass(value, add);
                });
            }
            return element;
        }
        if (!('kendoAnimate' in $.fn)) {
            extend($.fn, {
                kendoStop: function (clearQueue, gotoEnd) {
                    return this.stop(clearQueue, gotoEnd);
                },
                kendoAnimate: function (options, duration, reverse, complete) {
                    return animate(this, options, duration, reverse, complete);
                },
                kendoAddClass: function (classes, options) {
                    return kendo.toggleClass(this, classes, options, true);
                },
                kendoRemoveClass: function (classes, options) {
                    return kendo.toggleClass(this, classes, options, false);
                },
                kendoToggleClass: function (classes, options, toggle) {
                    return kendo.toggleClass(this, classes, options, toggle);
                }
            });
        }
        var ampRegExp = /&/g, ltRegExp = /</g, quoteRegExp = /"/g, aposRegExp = /'/g, gtRegExp = />/g;
        function htmlEncode(value) {
            return ('' + value).replace(ampRegExp, '&amp;').replace(ltRegExp, '&lt;').replace(gtRegExp, '&gt;').replace(quoteRegExp, '&quot;').replace(aposRegExp, '&#39;');
        }
        var eventTarget = function (e) {
            return e.target;
        };
        if (support.touch) {
            eventTarget = function (e) {
                var touches = 'originalEvent' in e ? e.originalEvent.changedTouches : 'changedTouches' in e ? e.changedTouches : null;
                return touches ? document.elementFromPoint(touches[0].clientX, touches[0].clientY) : e.target;
            };
            each([
                'swipe',
                'swipeLeft',
                'swipeRight',
                'swipeUp',
                'swipeDown',
                'doubleTap',
                'tap'
            ], function (m, value) {
                $.fn[value] = function (callback) {
                    return this.bind(value, callback);
                };
            });
        }
        if (support.touch) {
            if (!support.mobileOS) {
                support.mousedown = 'mousedown touchstart';
                support.mouseup = 'mouseup touchend';
                support.mousemove = 'mousemove touchmove';
                support.mousecancel = 'mouseleave touchcancel';
                support.click = 'click';
                support.resize = 'resize';
            } else {
                support.mousedown = 'touchstart';
                support.mouseup = 'touchend';
                support.mousemove = 'touchmove';
                support.mousecancel = 'touchcancel';
                support.click = 'touchend';
                support.resize = 'orientationchange';
            }
        } else if (support.pointers) {
            support.mousemove = 'pointermove';
            support.mousedown = 'pointerdown';
            support.mouseup = 'pointerup';
            support.mousecancel = 'pointercancel';
            support.click = 'pointerup';
            support.resize = 'orientationchange resize';
        } else if (support.msPointers) {
            support.mousemove = 'MSPointerMove';
            support.mousedown = 'MSPointerDown';
            support.mouseup = 'MSPointerUp';
            support.mousecancel = 'MSPointerCancel';
            support.click = 'MSPointerUp';
            support.resize = 'orientationchange resize';
        } else {
            support.mousemove = 'mousemove';
            support.mousedown = 'mousedown';
            support.mouseup = 'mouseup';
            support.mousecancel = 'mouseleave';
            support.click = 'click';
            support.resize = 'resize';
        }
        var wrapExpression = function (members, paramName) {
                var result = paramName || 'd', index, idx, length, member, count = 1;
                for (idx = 0, length = members.length; idx < length; idx++) {
                    member = members[idx];
                    if (member !== '') {
                        index = member.indexOf('[');
                        if (index !== 0) {
                            if (index == -1) {
                                member = '.' + member;
                            } else {
                                count++;
                                member = '.' + member.substring(0, index) + ' || {})' + member.substring(index);
                            }
                        }
                        count++;
                        result += member + (idx < length - 1 ? ' || {})' : ')');
                    }
                }
                return new Array(count).join('(') + result;
            }, localUrlRe = /^([a-z]+:)?\/\//i;
        extend(kendo, {
            widgets: [],
            _widgetRegisteredCallbacks: [],
            ui: kendo.ui || {},
            fx: kendo.fx || fx,
            effects: kendo.effects || effects,
            mobile: kendo.mobile || {},
            data: kendo.data || {},
            dataviz: kendo.dataviz || {},
            drawing: kendo.drawing || {},
            spreadsheet: { messages: {} },
            keys: {
                INSERT: 45,
                DELETE: 46,
                BACKSPACE: 8,
                TAB: 9,
                ENTER: 13,
                ESC: 27,
                LEFT: 37,
                UP: 38,
                RIGHT: 39,
                DOWN: 40,
                END: 35,
                HOME: 36,
                SPACEBAR: 32,
                PAGEUP: 33,
                PAGEDOWN: 34,
                F2: 113,
                F10: 121,
                F12: 123,
                NUMPAD_PLUS: 107,
                NUMPAD_MINUS: 109,
                NUMPAD_DOT: 110
            },
            support: kendo.support || support,
            animate: kendo.animate || animate,
            ns: '',
            attr: function (value) {
                return 'data-' + kendo.ns + value;
            },
            getShadows: getShadows,
            wrap: wrap,
            deepExtend: deepExtend,
            getComputedStyles: getComputedStyles,
            webComponents: kendo.webComponents || [],
            isScrollable: isScrollable,
            scrollLeft: scrollLeft,
            size: size,
            toCamelCase: toCamelCase,
            toHyphens: toHyphens,
            getOffset: kendo.getOffset || getOffset,
            parseEffects: kendo.parseEffects || parseEffects,
            toggleClass: kendo.toggleClass || toggleClass,
            directions: kendo.directions || directions,
            Observable: Observable,
            Class: Class,
            Template: Template,
            template: proxy(Template.compile, Template),
            render: proxy(Template.render, Template),
            stringify: proxy(JSON.stringify, JSON),
            eventTarget: eventTarget,
            htmlEncode: htmlEncode,
            isLocalUrl: function (url) {
                return url && !localUrlRe.test(url);
            },
            expr: function (expression, safe, paramName) {
                expression = expression || '';
                if (typeof safe == STRING) {
                    paramName = safe;
                    safe = false;
                }
                paramName = paramName || 'd';
                if (expression && expression.charAt(0) !== '[') {
                    expression = '.' + expression;
                }
                if (safe) {
                    expression = expression.replace(/"([^.]*)\.([^"]*)"/g, '"$1_$DOT$_$2"');
                    expression = expression.replace(/'([^.]*)\.([^']*)'/g, '\'$1_$DOT$_$2\'');
                    expression = wrapExpression(expression.split('.'), paramName);
                    expression = expression.replace(/_\$DOT\$_/g, '.');
                } else {
                    expression = paramName + expression;
                }
                return expression;
            },
            getter: function (expression, safe) {
                var key = expression + safe;
                return getterCache[key] = getterCache[key] || new Function('d', 'return ' + kendo.expr(expression, safe));
            },
            setter: function (expression) {
                return setterCache[expression] = setterCache[expression] || new Function('d,value', kendo.expr(expression) + '=value');
            },
            accessor: function (expression) {
                return {
                    get: kendo.getter(expression),
                    set: kendo.setter(expression)
                };
            },
            guid: function () {
                var id = '', i, random;
                for (i = 0; i < 32; i++) {
                    random = math.random() * 16 | 0;
                    if (i == 8 || i == 12 || i == 16 || i == 20) {
                        id += '-';
                    }
                    id += (i == 12 ? 4 : i == 16 ? random & 3 | 8 : random).toString(16);
                }
                return id;
            },
            roleSelector: function (role) {
                return role.replace(/(\S+)/g, '[' + kendo.attr('role') + '=$1],').slice(0, -1);
            },
            directiveSelector: function (directives) {
                var selectors = directives.split(' ');
                if (selectors) {
                    for (var i = 0; i < selectors.length; i++) {
                        if (selectors[i] != 'view') {
                            selectors[i] = selectors[i].replace(/(\w*)(view|bar|strip|over)$/, '$1-$2');
                        }
                    }
                }
                return selectors.join(' ').replace(/(\S+)/g, 'kendo-mobile-$1,').slice(0, -1);
            },
            triggeredByInput: function (e) {
                return /^(label|input|textarea|select)$/i.test(e.target.tagName);
            },
            onWidgetRegistered: function (callback) {
                for (var i = 0, len = kendo.widgets.length; i < len; i++) {
                    callback(kendo.widgets[i]);
                }
                kendo._widgetRegisteredCallbacks.push(callback);
            },
            logToConsole: function (message, type) {
                var console = window.console;
                if (!kendo.suppressLog && typeof console != 'undefined' && console.log) {
                    console[type || 'log'](message);
                }
            }
        });
        var Widget = Observable.extend({
            init: function (element, options) {
                var that = this;
                that.element = kendo.jQuery(element).handler(that);
                that.angular('init', options);
                Observable.fn.init.call(that);
                var dataSource = options ? options.dataSource : null;
                if (dataSource) {
                    options = extend({}, options, { dataSource: {} });
                }
                options = that.options = extend(true, {}, that.options, options);
                if (dataSource) {
                    options.dataSource = dataSource;
                }
                if (!that.element.attr(kendo.attr('role'))) {
                    that.element.attr(kendo.attr('role'), (options.name || '').toLowerCase());
                }
                that.element.data('kendo' + options.prefix + options.name, that);
                that.bind(that.events, options);
            },
            events: [],
            options: { prefix: '' },
            _hasBindingTarget: function () {
                return !!this.element[0].kendoBindingTarget;
            },
            _tabindex: function (target) {
                target = target || this.wrapper;
                var element = this.element, TABINDEX = 'tabindex', tabindex = target.attr(TABINDEX) || element.attr(TABINDEX);
                element.removeAttr(TABINDEX);
                target.attr(TABINDEX, !isNaN(tabindex) ? tabindex : 0);
            },
            setOptions: function (options) {
                this._setEvents(options);
                $.extend(this.options, options);
            },
            _setEvents: function (options) {
                var that = this, idx = 0, length = that.events.length, e;
                for (; idx < length; idx++) {
                    e = that.events[idx];
                    if (that.options[e] && options[e]) {
                        that.unbind(e, that.options[e]);
                    }
                }
                that.bind(that.events, options);
            },
            resize: function (force) {
                var size = this.getSize(), currentSize = this._size;
                if (force || (size.width > 0 || size.height > 0) && (!currentSize || size.width !== currentSize.width || size.height !== currentSize.height)) {
                    this._size = size;
                    this._resize(size, force);
                    this.trigger('resize', size);
                }
            },
            getSize: function () {
                return kendo.dimensions(this.element);
            },
            size: function (size) {
                if (!size) {
                    return this.getSize();
                } else {
                    this.setSize(size);
                }
            },
            setSize: $.noop,
            _resize: $.noop,
            destroy: function () {
                var that = this;
                that.element.removeData('kendo' + that.options.prefix + that.options.name);
                that.element.removeData('handler');
                that.unbind();
            },
            _destroy: function () {
                this.destroy();
            },
            angular: function () {
            },
            _muteAngularRebind: function (callback) {
                this._muteRebind = true;
                callback.call(this);
                this._muteRebind = false;
            }
        });
        var DataBoundWidget = Widget.extend({
            dataItems: function () {
                return this.dataSource.flatView();
            },
            _angularItems: function (cmd) {
                var that = this;
                that.angular(cmd, function () {
                    return {
                        elements: that.items(),
                        data: $.map(that.dataItems(), function (dataItem) {
                            return { dataItem: dataItem };
                        })
                    };
                });
            }
        });
        kendo.dimensions = function (element, dimensions) {
            var domElement = element[0];
            if (dimensions) {
                element.css(dimensions);
            }
            return {
                width: domElement.offsetWidth,
                height: domElement.offsetHeight
            };
        };
        kendo.notify = noop;
        var templateRegExp = /template$/i, jsonRegExp = /^\s*(?:\{(?:.|\r\n|\n)*\}|\[(?:.|\r\n|\n)*\])\s*$/, jsonFormatRegExp = /^\{(\d+)(:[^\}]+)?\}|^\[[A-Za-z_]+\]$/, dashRegExp = /([A-Z])/g;
        function parseOption(element, option) {
            var value;
            if (option.indexOf('data') === 0) {
                option = option.substring(4);
                option = option.charAt(0).toLowerCase() + option.substring(1);
            }
            option = option.replace(dashRegExp, '-$1');
            value = element.getAttribute('data-' + kendo.ns + option);
            if (value === null) {
                value = undefined;
            } else if (value === 'null') {
                value = null;
            } else if (value === 'true') {
                value = true;
            } else if (value === 'false') {
                value = false;
            } else if (numberRegExp.test(value) && option != 'mask') {
                value = parseFloat(value);
            } else if (jsonRegExp.test(value) && !jsonFormatRegExp.test(value)) {
                value = new Function('return (' + value + ')')();
            }
            return value;
        }
        function parseOptions(element, options, source) {
            var result = {}, option, value;
            for (option in options) {
                value = parseOption(element, option);
                if (value !== undefined) {
                    if (templateRegExp.test(option)) {
                        if (typeof value === 'string') {
                            if ($('#' + value).length) {
                                value = kendo.template($('#' + value).html());
                            } else if (source) {
                                value = kendo.template(source[value]);
                            }
                        } else {
                            value = element.getAttribute(option);
                        }
                    }
                    result[option] = value;
                }
            }
            return result;
        }
        kendo.initWidget = function (element, options, roles) {
            var result, option, widget, idx, length, role, value, dataSource, fullPath, widgetKeyRegExp;
            if (!roles) {
                roles = kendo.ui.roles;
            } else if (roles.roles) {
                roles = roles.roles;
            }
            element = element.nodeType ? element : element[0];
            role = element.getAttribute('data-' + kendo.ns + 'role');
            if (!role) {
                return;
            }
            fullPath = role.indexOf('.') === -1;
            if (fullPath) {
                widget = roles[role];
            } else {
                widget = kendo.getter(role)(window);
            }
            var data = $(element).data(), widgetKey = widget ? 'kendo' + widget.fn.options.prefix + widget.fn.options.name : '';
            if (fullPath) {
                widgetKeyRegExp = new RegExp('^kendo.*' + role + '$', 'i');
            } else {
                widgetKeyRegExp = new RegExp('^' + widgetKey + '$', 'i');
            }
            for (var key in data) {
                if (key.match(widgetKeyRegExp)) {
                    if (key === widgetKey) {
                        result = data[key];
                    } else {
                        return data[key];
                    }
                }
            }
            if (!widget) {
                return;
            }
            dataSource = parseOption(element, 'dataSource');
            options = $.extend({}, parseOptions(element, widget.fn.options), options);
            if (dataSource) {
                if (typeof dataSource === STRING) {
                    options.dataSource = kendo.getter(dataSource)(window);
                } else {
                    options.dataSource = dataSource;
                }
            }
            for (idx = 0, length = widget.fn.events.length; idx < length; idx++) {
                option = widget.fn.events[idx];
                value = parseOption(element, option);
                if (value !== undefined) {
                    options[option] = kendo.getter(value)(window);
                }
            }
            if (!result) {
                result = new widget(element, options);
            } else if (!$.isEmptyObject(options)) {
                result.setOptions(options);
            }
            return result;
        };
        kendo.rolesFromNamespaces = function (namespaces) {
            var roles = [], idx, length;
            if (!namespaces[0]) {
                namespaces = [
                    kendo.ui,
                    kendo.dataviz.ui
                ];
            }
            for (idx = 0, length = namespaces.length; idx < length; idx++) {
                roles[idx] = namespaces[idx].roles;
            }
            return extend.apply(null, [{}].concat(roles.reverse()));
        };
        kendo.init = function (element) {
            var roles = kendo.rolesFromNamespaces(slice.call(arguments, 1));
            $(element).find('[data-' + kendo.ns + 'role]').addBack().each(function () {
                kendo.initWidget(this, {}, roles);
            });
        };
        kendo.destroy = function (element) {
            $(element).find('[data-' + kendo.ns + 'role]').addBack().each(function () {
                var data = $(this).data();
                for (var key in data) {
                    if (key.indexOf('kendo') === 0 && typeof data[key].destroy === FUNCTION) {
                        data[key].destroy();
                    }
                }
            });
        };
        function containmentComparer(a, b) {
            return $.contains(a, b) ? -1 : 1;
        }
        function resizableWidget() {
            var widget = $(this);
            return $.inArray(widget.attr('data-' + kendo.ns + 'role'), [
                'slider',
                'rangeslider'
            ]) > -1 || widget.is(':visible');
        }
        kendo.resize = function (element, force) {
            var widgets = $(element).find('[data-' + kendo.ns + 'role]').addBack().filter(resizableWidget);
            if (!widgets.length) {
                return;
            }
            var widgetsArray = $.makeArray(widgets);
            widgetsArray.sort(containmentComparer);
            $.each(widgetsArray, function () {
                var widget = kendo.widgetInstance($(this));
                if (widget) {
                    widget.resize(force);
                }
            });
        };
        kendo.parseOptions = parseOptions;
        extend(kendo.ui, {
            Widget: Widget,
            DataBoundWidget: DataBoundWidget,
            roles: {},
            progress: function (container, toggle, options) {
                var mask = container.find('.k-loading-mask'), support = kendo.support, browser = support.browser, isRtl, leftRight, webkitCorrection, containerScrollLeft, cssClass;
                options = $.extend({}, {
                    width: '100%',
                    height: '100%',
                    top: container.scrollTop(),
                    opacity: false
                }, options);
                cssClass = options.opacity ? 'k-loading-mask k-opaque' : 'k-loading-mask';
                if (toggle) {
                    if (!mask.length) {
                        isRtl = support.isRtl(container);
                        leftRight = isRtl ? 'right' : 'left';
                        containerScrollLeft = container.scrollLeft();
                        webkitCorrection = browser.webkit ? !isRtl ? 0 : container[0].scrollWidth - container.width() - 2 * containerScrollLeft : 0;
                        mask = $(kendo.format('<div class=\'{0}\'><span class=\'k-loading-text\'>{1}</span><div class=\'k-loading-image\'/><div class=\'k-loading-color\'/></div>', cssClass, kendo.ui.progress.messages.loading)).width(options.width).height(options.height).css('top', options.top).css(leftRight, Math.abs(containerScrollLeft) + webkitCorrection).prependTo(container);
                    }
                } else if (mask) {
                    mask.remove();
                }
            },
            plugin: function (widget, register, prefix) {
                var name = widget.fn.options.name, getter;
                register = register || kendo.ui;
                prefix = prefix || '';
                register[name] = widget;
                register.roles[name.toLowerCase()] = widget;
                getter = 'getKendo' + prefix + name;
                name = 'kendo' + prefix + name;
                var widgetEntry = {
                    name: name,
                    widget: widget,
                    prefix: prefix || ''
                };
                kendo.widgets.push(widgetEntry);
                for (var i = 0, len = kendo._widgetRegisteredCallbacks.length; i < len; i++) {
                    kendo._widgetRegisteredCallbacks[i](widgetEntry);
                }
                $.fn[name] = function (options) {
                    var value = this, args;
                    if (typeof options === STRING) {
                        args = slice.call(arguments, 1);
                        this.each(function () {
                            var widget = $.data(this, name), method, result;
                            if (!widget) {
                                throw new Error(kendo.format('Cannot call method \'{0}\' of {1} before it is initialized', options, name));
                            }
                            method = widget[options];
                            if (typeof method !== FUNCTION) {
                                throw new Error(kendo.format('Cannot find method \'{0}\' of {1}', options, name));
                            }
                            result = method.apply(widget, args);
                            if (result !== undefined) {
                                value = result;
                                return false;
                            }
                        });
                    } else {
                        this.each(function () {
                            return new widget(this, options);
                        });
                    }
                    return value;
                };
                $.fn[name].widget = widget;
                $.fn[getter] = function () {
                    return this.data(name);
                };
            }
        });
        kendo.ui.progress.messages = { loading: 'Loading...' };
        var ContainerNullObject = {
            bind: function () {
                return this;
            },
            nullObject: true,
            options: {}
        };
        var MobileWidget = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                this.element.autoApplyNS();
                this.wrapper = this.element;
                this.element.addClass('km-widget');
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.element.kendoDestroy();
            },
            options: { prefix: 'Mobile' },
            events: [],
            view: function () {
                var viewElement = this.element.closest(kendo.roleSelector('view splitview modalview drawer'));
                return kendo.widgetInstance(viewElement, kendo.mobile.ui) || ContainerNullObject;
            },
            viewHasNativeScrolling: function () {
                var view = this.view();
                return view && view.options.useNativeScrolling;
            },
            container: function () {
                var element = this.element.closest(kendo.roleSelector('view layout modalview drawer splitview'));
                return kendo.widgetInstance(element.eq(0), kendo.mobile.ui) || ContainerNullObject;
            }
        });
        extend(kendo.mobile, {
            init: function (element) {
                kendo.init(element, kendo.mobile.ui, kendo.ui, kendo.dataviz.ui);
            },
            appLevelNativeScrolling: function () {
                return kendo.mobile.application && kendo.mobile.application.options && kendo.mobile.application.options.useNativeScrolling;
            },
            roles: {},
            ui: {
                Widget: MobileWidget,
                DataBoundWidget: DataBoundWidget.extend(MobileWidget.prototype),
                roles: {},
                plugin: function (widget) {
                    kendo.ui.plugin(widget, kendo.mobile.ui, 'Mobile');
                }
            }
        });
        deepExtend(kendo.dataviz, {
            init: function (element) {
                kendo.init(element, kendo.dataviz.ui);
            },
            ui: {
                roles: {},
                themes: {},
                views: [],
                plugin: function (widget) {
                    kendo.ui.plugin(widget, kendo.dataviz.ui);
                }
            },
            roles: {}
        });
        kendo.touchScroller = function (elements, options) {
            if (!options) {
                options = {};
            }
            options.useNative = true;
            return $(elements).map(function (idx, element) {
                element = $(element);
                if (support.kineticScrollNeeded && kendo.mobile.ui.Scroller && !element.data('kendoMobileScroller')) {
                    element.kendoMobileScroller(options);
                    return element.data('kendoMobileScroller');
                } else {
                    return false;
                }
            })[0];
        };
        kendo.preventDefault = function (e) {
            e.preventDefault();
        };
        kendo.widgetInstance = function (element, suites) {
            var role = element.data(kendo.ns + 'role'), widgets = [], i, length;
            if (role) {
                if (role === 'content') {
                    role = 'scroller';
                }
                if (role === 'editortoolbar') {
                    var editorToolbar = element.data('kendoEditorToolbar');
                    if (editorToolbar) {
                        return editorToolbar;
                    }
                }
                if (suites) {
                    if (suites[0]) {
                        for (i = 0, length = suites.length; i < length; i++) {
                            widgets.push(suites[i].roles[role]);
                        }
                    } else {
                        widgets.push(suites.roles[role]);
                    }
                } else {
                    widgets = [
                        kendo.ui.roles[role],
                        kendo.dataviz.ui.roles[role],
                        kendo.mobile.ui.roles[role]
                    ];
                }
                if (role.indexOf('.') >= 0) {
                    widgets = [kendo.getter(role)(window)];
                }
                for (i = 0, length = widgets.length; i < length; i++) {
                    var widget = widgets[i];
                    if (widget) {
                        var instance = element.data('kendo' + widget.fn.options.prefix + widget.fn.options.name);
                        if (instance) {
                            return instance;
                        }
                    }
                }
            }
        };
        kendo.onResize = function (callback) {
            var handler = callback;
            if (support.mobileOS.android) {
                handler = function () {
                    setTimeout(callback, 600);
                };
            }
            $(window).on(support.resize, handler);
            return handler;
        };
        kendo.unbindResize = function (callback) {
            $(window).off(support.resize, callback);
        };
        kendo.attrValue = function (element, key) {
            return element.data(kendo.ns + key);
        };
        kendo.days = {
            Sunday: 0,
            Monday: 1,
            Tuesday: 2,
            Wednesday: 3,
            Thursday: 4,
            Friday: 5,
            Saturday: 6
        };
        function focusable(element, isTabIndexNotNaN) {
            var nodeName = element.nodeName.toLowerCase();
            return (/input|select|textarea|button|object/.test(nodeName) ? !element.disabled : 'a' === nodeName ? element.href || isTabIndexNotNaN : isTabIndexNotNaN) && visible(element);
        }
        function visible(element) {
            return $.expr.filters.visible(element) && !$(element).parents().addBack().filter(function () {
                return $.css(this, 'visibility') === 'hidden';
            }).length;
        }
        $.extend($.expr[':'], {
            kendoFocusable: function (element) {
                var idx = $.attr(element, 'tabindex');
                return focusable(element, !isNaN(idx) && idx > -1);
            }
        });
        var MOUSE_EVENTS = [
            'mousedown',
            'mousemove',
            'mouseenter',
            'mouseleave',
            'mouseover',
            'mouseout',
            'mouseup',
            'click'
        ];
        var EXCLUDE_BUST_CLICK_SELECTOR = 'label, input, [data-rel=external]';
        var MouseEventNormalizer = {
            setupMouseMute: function () {
                var idx = 0, length = MOUSE_EVENTS.length, element = document.documentElement;
                if (MouseEventNormalizer.mouseTrap || !support.eventCapture) {
                    return;
                }
                MouseEventNormalizer.mouseTrap = true;
                MouseEventNormalizer.bustClick = false;
                MouseEventNormalizer.captureMouse = false;
                var handler = function (e) {
                    if (MouseEventNormalizer.captureMouse) {
                        if (e.type === 'click') {
                            if (MouseEventNormalizer.bustClick && !$(e.target).is(EXCLUDE_BUST_CLICK_SELECTOR)) {
                                e.preventDefault();
                                e.stopPropagation();
                            }
                        } else {
                            e.stopPropagation();
                        }
                    }
                };
                for (; idx < length; idx++) {
                    element.addEventListener(MOUSE_EVENTS[idx], handler, true);
                }
            },
            muteMouse: function (e) {
                MouseEventNormalizer.captureMouse = true;
                if (e.data.bustClick) {
                    MouseEventNormalizer.bustClick = true;
                }
                clearTimeout(MouseEventNormalizer.mouseTrapTimeoutID);
            },
            unMuteMouse: function () {
                clearTimeout(MouseEventNormalizer.mouseTrapTimeoutID);
                MouseEventNormalizer.mouseTrapTimeoutID = setTimeout(function () {
                    MouseEventNormalizer.captureMouse = false;
                    MouseEventNormalizer.bustClick = false;
                }, 400);
            }
        };
        var eventMap = {
            down: 'touchstart mousedown',
            move: 'mousemove touchmove',
            up: 'mouseup touchend touchcancel',
            cancel: 'mouseleave touchcancel'
        };
        if (support.touch && (support.mobileOS.ios || support.mobileOS.android)) {
            eventMap = {
                down: 'touchstart',
                move: 'touchmove',
                up: 'touchend touchcancel',
                cancel: 'touchcancel'
            };
        } else if (support.pointers) {
            eventMap = {
                down: 'pointerdown',
                move: 'pointermove',
                up: 'pointerup',
                cancel: 'pointercancel pointerleave'
            };
        } else if (support.msPointers) {
            eventMap = {
                down: 'MSPointerDown',
                move: 'MSPointerMove',
                up: 'MSPointerUp',
                cancel: 'MSPointerCancel MSPointerLeave'
            };
        }
        if (support.msPointers && !('onmspointerenter' in window)) {
            $.each({
                MSPointerEnter: 'MSPointerOver',
                MSPointerLeave: 'MSPointerOut'
            }, function (orig, fix) {
                $.event.special[orig] = {
                    delegateType: fix,
                    bindType: fix,
                    handle: function (event) {
                        var ret, target = this, related = event.relatedTarget, handleObj = event.handleObj;
                        if (!related || related !== target && !$.contains(target, related)) {
                            event.type = handleObj.origType;
                            ret = handleObj.handler.apply(this, arguments);
                            event.type = fix;
                        }
                        return ret;
                    }
                };
            });
        }
        var getEventMap = function (e) {
                return eventMap[e] || e;
            }, eventRegEx = /([^ ]+)/g;
        kendo.applyEventMap = function (events, ns) {
            events = events.replace(eventRegEx, getEventMap);
            if (ns) {
                events = events.replace(eventRegEx, '$1.' + ns);
            }
            return events;
        };
        var on = $.fn.on;
        function kendoJQuery(selector, context) {
            return new kendoJQuery.fn.init(selector, context);
        }
        extend(true, kendoJQuery, $);
        kendoJQuery.fn = kendoJQuery.prototype = new $();
        kendoJQuery.fn.constructor = kendoJQuery;
        kendoJQuery.fn.init = function (selector, context) {
            if (context && context instanceof $ && !(context instanceof kendoJQuery)) {
                context = kendoJQuery(context);
            }
            return $.fn.init.call(this, selector, context, rootjQuery);
        };
        kendoJQuery.fn.init.prototype = kendoJQuery.fn;
        var rootjQuery = kendoJQuery(document);
        extend(kendoJQuery.fn, {
            handler: function (handler) {
                this.data('handler', handler);
                return this;
            },
            autoApplyNS: function (ns) {
                this.data('kendoNS', ns || kendo.guid());
                return this;
            },
            on: function () {
                var that = this, ns = that.data('kendoNS');
                if (arguments.length === 1) {
                    return on.call(that, arguments[0]);
                }
                var context = that, args = slice.call(arguments);
                if (typeof args[args.length - 1] === UNDEFINED) {
                    args.pop();
                }
                var callback = args[args.length - 1], events = kendo.applyEventMap(args[0], ns);
                if (support.mouseAndTouchPresent && events.search(/mouse|click/) > -1 && this[0] !== document.documentElement) {
                    MouseEventNormalizer.setupMouseMute();
                    var selector = args.length === 2 ? null : args[1], bustClick = events.indexOf('click') > -1 && events.indexOf('touchend') > -1;
                    on.call(this, {
                        touchstart: MouseEventNormalizer.muteMouse,
                        touchend: MouseEventNormalizer.unMuteMouse
                    }, selector, { bustClick: bustClick });
                }
                if (typeof callback === STRING) {
                    context = that.data('handler');
                    callback = context[callback];
                    args[args.length - 1] = function (e) {
                        callback.call(context, e);
                    };
                }
                args[0] = events;
                on.apply(that, args);
                return that;
            },
            kendoDestroy: function (ns) {
                ns = ns || this.data('kendoNS');
                if (ns) {
                    this.off('.' + ns);
                }
                return this;
            }
        });
        kendo.jQuery = kendoJQuery;
        kendo.eventMap = eventMap;
        kendo.timezone = function () {
            var months = {
                Jan: 0,
                Feb: 1,
                Mar: 2,
                Apr: 3,
                May: 4,
                Jun: 5,
                Jul: 6,
                Aug: 7,
                Sep: 8,
                Oct: 9,
                Nov: 10,
                Dec: 11
            };
            var days = {
                Sun: 0,
                Mon: 1,
                Tue: 2,
                Wed: 3,
                Thu: 4,
                Fri: 5,
                Sat: 6
            };
            function ruleToDate(year, rule) {
                var date;
                var targetDay;
                var ourDay;
                var month = rule[3];
                var on = rule[4];
                var time = rule[5];
                var cache = rule[8];
                if (!cache) {
                    rule[8] = cache = {};
                }
                if (cache[year]) {
                    return cache[year];
                }
                if (!isNaN(on)) {
                    date = new Date(Date.UTC(year, months[month], on, time[0], time[1], time[2], 0));
                } else if (on.indexOf('last') === 0) {
                    date = new Date(Date.UTC(year, months[month] + 1, 1, time[0] - 24, time[1], time[2], 0));
                    targetDay = days[on.substr(4, 3)];
                    ourDay = date.getUTCDay();
                    date.setUTCDate(date.getUTCDate() + targetDay - ourDay - (targetDay > ourDay ? 7 : 0));
                } else if (on.indexOf('>=') >= 0) {
                    date = new Date(Date.UTC(year, months[month], on.substr(5), time[0], time[1], time[2], 0));
                    targetDay = days[on.substr(0, 3)];
                    ourDay = date.getUTCDay();
                    date.setUTCDate(date.getUTCDate() + targetDay - ourDay + (targetDay < ourDay ? 7 : 0));
                }
                return cache[year] = date;
            }
            function findRule(utcTime, rules, zone) {
                rules = rules[zone];
                if (!rules) {
                    var time = zone.split(':');
                    var offset = 0;
                    if (time.length > 1) {
                        offset = time[0] * 60 + Number(time[1]);
                    }
                    return [
                        -1000000,
                        'max',
                        '-',
                        'Jan',
                        1,
                        [
                            0,
                            0,
                            0
                        ],
                        offset,
                        '-'
                    ];
                }
                var year = new Date(utcTime).getUTCFullYear();
                rules = jQuery.grep(rules, function (rule) {
                    var from = rule[0];
                    var to = rule[1];
                    return from <= year && (to >= year || from == year && to == 'only' || to == 'max');
                });
                rules.push(utcTime);
                rules.sort(function (a, b) {
                    if (typeof a != 'number') {
                        a = Number(ruleToDate(year, a));
                    }
                    if (typeof b != 'number') {
                        b = Number(ruleToDate(year, b));
                    }
                    return a - b;
                });
                var rule = rules[jQuery.inArray(utcTime, rules) - 1] || rules[rules.length - 1];
                return isNaN(rule) ? rule : null;
            }
            function findZone(utcTime, zones, timezone) {
                var zoneRules = zones[timezone];
                if (typeof zoneRules === 'string') {
                    zoneRules = zones[zoneRules];
                }
                if (!zoneRules) {
                    throw new Error('Timezone "' + timezone + '" is either incorrect, or kendo.timezones.min.js is not included.');
                }
                for (var idx = zoneRules.length - 1; idx >= 0; idx--) {
                    var until = zoneRules[idx][3];
                    if (until && utcTime > until) {
                        break;
                    }
                }
                var zone = zoneRules[idx + 1];
                if (!zone) {
                    throw new Error('Timezone "' + timezone + '" not found on ' + utcTime + '.');
                }
                return zone;
            }
            function zoneAndRule(utcTime, zones, rules, timezone) {
                if (typeof utcTime != NUMBER) {
                    utcTime = Date.UTC(utcTime.getFullYear(), utcTime.getMonth(), utcTime.getDate(), utcTime.getHours(), utcTime.getMinutes(), utcTime.getSeconds(), utcTime.getMilliseconds());
                }
                var zone = findZone(utcTime, zones, timezone);
                return {
                    zone: zone,
                    rule: findRule(utcTime, rules, zone[1])
                };
            }
            function offset(utcTime, timezone) {
                if (timezone == 'Etc/UTC' || timezone == 'Etc/GMT') {
                    return 0;
                }
                var info = zoneAndRule(utcTime, this.zones, this.rules, timezone);
                var zone = info.zone;
                var rule = info.rule;
                return kendo.parseFloat(rule ? zone[0] - rule[6] : zone[0]);
            }
            function abbr(utcTime, timezone) {
                var info = zoneAndRule(utcTime, this.zones, this.rules, timezone);
                var zone = info.zone;
                var rule = info.rule;
                var base = zone[2];
                if (base.indexOf('/') >= 0) {
                    return base.split('/')[rule && +rule[6] ? 1 : 0];
                } else if (base.indexOf('%s') >= 0) {
                    return base.replace('%s', !rule || rule[7] == '-' ? '' : rule[7]);
                }
                return base;
            }
            function convert(date, fromOffset, toOffset) {
                var tempToOffset = toOffset;
                var diff;
                if (typeof fromOffset == STRING) {
                    fromOffset = this.offset(date, fromOffset);
                }
                if (typeof toOffset == STRING) {
                    toOffset = this.offset(date, toOffset);
                }
                var fromLocalOffset = date.getTimezoneOffset();
                date = new Date(date.getTime() + (fromOffset - toOffset) * 60000);
                var toLocalOffset = date.getTimezoneOffset();
                if (typeof tempToOffset == STRING) {
                    tempToOffset = this.offset(date, tempToOffset);
                }
                diff = toLocalOffset - fromLocalOffset + (toOffset - tempToOffset);
                return new Date(date.getTime() + diff * 60000);
            }
            function apply(date, timezone) {
                return this.convert(date, date.getTimezoneOffset(), timezone);
            }
            function remove(date, timezone) {
                return this.convert(date, timezone, date.getTimezoneOffset());
            }
            function toLocalDate(time) {
                return this.apply(new Date(time), 'Etc/UTC');
            }
            return {
                zones: {},
                rules: {},
                offset: offset,
                convert: convert,
                apply: apply,
                remove: remove,
                abbr: abbr,
                toLocalDate: toLocalDate
            };
        }();
        kendo.date = function () {
            var MS_PER_MINUTE = 60000, MS_PER_DAY = 86400000;
            function adjustDST(date, hours) {
                if (hours === 0 && date.getHours() === 23) {
                    date.setHours(date.getHours() + 2);
                    return true;
                }
                return false;
            }
            function setDayOfWeek(date, day, dir) {
                var hours = date.getHours();
                dir = dir || 1;
                day = (day - date.getDay() + 7 * dir) % 7;
                date.setDate(date.getDate() + day);
                adjustDST(date, hours);
            }
            function dayOfWeek(date, day, dir) {
                date = new Date(date);
                setDayOfWeek(date, day, dir);
                return date;
            }
            function firstDayOfMonth(date) {
                return new Date(date.getFullYear(), date.getMonth(), 1);
            }
            function lastDayOfMonth(date) {
                var last = new Date(date.getFullYear(), date.getMonth() + 1, 0), first = firstDayOfMonth(date), timeOffset = Math.abs(last.getTimezoneOffset() - first.getTimezoneOffset());
                if (timeOffset) {
                    last.setHours(first.getHours() + timeOffset / 60);
                }
                return last;
            }
            function moveDateToWeekStart(date, weekStartDay) {
                if (weekStartDay !== 1) {
                    return addDays(dayOfWeek(date, weekStartDay, -1), 4);
                }
                return addDays(date, 4 - (date.getDay() || 7));
            }
            function calcWeekInYear(date, weekStartDay) {
                var firstWeekInYear = new Date(date.getFullYear(), 0, 1, -6);
                var newDate = moveDateToWeekStart(date, weekStartDay);
                var diffInMS = newDate.getTime() - firstWeekInYear.getTime();
                var days = Math.floor(diffInMS / MS_PER_DAY);
                return 1 + Math.floor(days / 7);
            }
            function weekInYear(date, weekStartDay) {
                if (weekStartDay === undefined) {
                    weekStartDay = kendo.culture().calendar.firstDay;
                }
                var prevWeekDate = addDays(date, -7);
                var nextWeekDate = addDays(date, 7);
                var weekNumber = calcWeekInYear(date, weekStartDay);
                if (weekNumber === 0) {
                    return calcWeekInYear(prevWeekDate, weekStartDay) + 1;
                }
                if (weekNumber === 53 && calcWeekInYear(nextWeekDate, weekStartDay) > 1) {
                    return 1;
                }
                return weekNumber;
            }
            function getDate(date) {
                date = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0);
                adjustDST(date, 0);
                return date;
            }
            function toUtcTime(date) {
                return Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds(), date.getMilliseconds());
            }
            function getMilliseconds(date) {
                return toInvariantTime(date).getTime() - getDate(toInvariantTime(date));
            }
            function isInTimeRange(value, min, max) {
                var msMin = getMilliseconds(min), msMax = getMilliseconds(max), msValue;
                if (!value || msMin == msMax) {
                    return true;
                }
                if (min >= max) {
                    max += MS_PER_DAY;
                }
                msValue = getMilliseconds(value);
                if (msMin > msValue) {
                    msValue += MS_PER_DAY;
                }
                if (msMax < msMin) {
                    msMax += MS_PER_DAY;
                }
                return msValue >= msMin && msValue <= msMax;
            }
            function isInDateRange(value, min, max) {
                var msMin = min.getTime(), msMax = max.getTime(), msValue;
                if (msMin >= msMax) {
                    msMax += MS_PER_DAY;
                }
                msValue = value.getTime();
                return msValue >= msMin && msValue <= msMax;
            }
            function addDays(date, offset) {
                var hours = date.getHours();
                date = new Date(date);
                setTime(date, offset * MS_PER_DAY);
                adjustDST(date, hours);
                return date;
            }
            function setTime(date, milliseconds, ignoreDST) {
                var offset = date.getTimezoneOffset();
                var difference;
                date.setTime(date.getTime() + milliseconds);
                if (!ignoreDST) {
                    difference = date.getTimezoneOffset() - offset;
                    date.setTime(date.getTime() + difference * MS_PER_MINUTE);
                }
            }
            function setHours(date, time) {
                date = new Date(kendo.date.getDate(date).getTime() + kendo.date.getMilliseconds(time));
                adjustDST(date, time.getHours());
                return date;
            }
            function today() {
                return getDate(new Date());
            }
            function isToday(date) {
                return getDate(date).getTime() == today().getTime();
            }
            function toInvariantTime(date) {
                var staticDate = new Date(1980, 1, 1, 0, 0, 0);
                if (date) {
                    staticDate.setHours(date.getHours(), date.getMinutes(), date.getSeconds(), date.getMilliseconds());
                }
                return staticDate;
            }
            return {
                adjustDST: adjustDST,
                dayOfWeek: dayOfWeek,
                setDayOfWeek: setDayOfWeek,
                getDate: getDate,
                isInDateRange: isInDateRange,
                isInTimeRange: isInTimeRange,
                isToday: isToday,
                nextDay: function (date) {
                    return addDays(date, 1);
                },
                previousDay: function (date) {
                    return addDays(date, -1);
                },
                toUtcTime: toUtcTime,
                MS_PER_DAY: MS_PER_DAY,
                MS_PER_HOUR: 60 * MS_PER_MINUTE,
                MS_PER_MINUTE: MS_PER_MINUTE,
                setTime: setTime,
                setHours: setHours,
                addDays: addDays,
                today: today,
                toInvariantTime: toInvariantTime,
                firstDayOfMonth: firstDayOfMonth,
                lastDayOfMonth: lastDayOfMonth,
                weekInYear: weekInYear,
                getMilliseconds: getMilliseconds
            };
        }();
        kendo.stripWhitespace = function (element) {
            if (document.createNodeIterator) {
                var iterator = document.createNodeIterator(element, NodeFilter.SHOW_TEXT, function (node) {
                    return node.parentNode == element ? NodeFilter.FILTER_ACCEPT : NodeFilter.FILTER_REJECT;
                }, false);
                while (iterator.nextNode()) {
                    if (iterator.referenceNode && !iterator.referenceNode.textContent.trim()) {
                        iterator.referenceNode.parentNode.removeChild(iterator.referenceNode);
                    }
                }
            } else {
                for (var i = 0; i < element.childNodes.length; i++) {
                    var child = element.childNodes[i];
                    if (child.nodeType == 3 && !/\S/.test(child.nodeValue)) {
                        element.removeChild(child);
                        i--;
                    }
                    if (child.nodeType == 1) {
                        kendo.stripWhitespace(child);
                    }
                }
            }
        };
        var animationFrame = window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.oRequestAnimationFrame || window.msRequestAnimationFrame || function (callback) {
            setTimeout(callback, 1000 / 60);
        };
        kendo.animationFrame = function (callback) {
            animationFrame.call(window, callback);
        };
        var animationQueue = [];
        kendo.queueAnimation = function (callback) {
            animationQueue[animationQueue.length] = callback;
            if (animationQueue.length === 1) {
                kendo.runNextAnimation();
            }
        };
        kendo.runNextAnimation = function () {
            kendo.animationFrame(function () {
                if (animationQueue[0]) {
                    animationQueue.shift()();
                    if (animationQueue[0]) {
                        kendo.runNextAnimation();
                    }
                }
            });
        };
        kendo.parseQueryStringParams = function (url) {
            var queryString = url.split('?')[1] || '', params = {}, paramParts = queryString.split(/&|=/), length = paramParts.length, idx = 0;
            for (; idx < length; idx += 2) {
                if (paramParts[idx] !== '') {
                    params[decodeURIComponent(paramParts[idx])] = decodeURIComponent(paramParts[idx + 1]);
                }
            }
            return params;
        };
        kendo.elementUnderCursor = function (e) {
            if (typeof e.x.client != 'undefined') {
                return document.elementFromPoint(e.x.client, e.y.client);
            }
        };
        kendo.wheelDeltaY = function (jQueryEvent) {
            var e = jQueryEvent.originalEvent, deltaY = e.wheelDeltaY, delta;
            if (e.wheelDelta) {
                if (deltaY === undefined || deltaY) {
                    delta = e.wheelDelta;
                }
            } else if (e.detail && e.axis === e.VERTICAL_AXIS) {
                delta = -e.detail * 10;
            }
            return delta;
        };
        kendo.throttle = function (fn, delay) {
            var timeout;
            var lastExecTime = 0;
            if (!delay || delay <= 0) {
                return fn;
            }
            var throttled = function () {
                var that = this;
                var elapsed = +new Date() - lastExecTime;
                var args = arguments;
                function exec() {
                    fn.apply(that, args);
                    lastExecTime = +new Date();
                }
                if (!lastExecTime) {
                    return exec();
                }
                if (timeout) {
                    clearTimeout(timeout);
                }
                if (elapsed > delay) {
                    exec();
                } else {
                    timeout = setTimeout(exec, delay - elapsed);
                }
            };
            throttled.cancel = function () {
                clearTimeout(timeout);
            };
            return throttled;
        };
        kendo.caret = function (element, start, end) {
            var rangeElement;
            var isPosition = start !== undefined;
            if (end === undefined) {
                end = start;
            }
            if (element[0]) {
                element = element[0];
            }
            if (isPosition && element.disabled) {
                return;
            }
            try {
                if (element.selectionStart !== undefined) {
                    if (isPosition) {
                        element.focus();
                        var mobile = support.mobileOS;
                        if (mobile.wp || mobile.android) {
                            setTimeout(function () {
                                element.setSelectionRange(start, end);
                            }, 0);
                        } else {
                            element.setSelectionRange(start, end);
                        }
                    } else {
                        start = [
                            element.selectionStart,
                            element.selectionEnd
                        ];
                    }
                } else if (document.selection) {
                    if ($(element).is(':visible')) {
                        element.focus();
                    }
                    rangeElement = element.createTextRange();
                    if (isPosition) {
                        rangeElement.collapse(true);
                        rangeElement.moveStart('character', start);
                        rangeElement.moveEnd('character', end - start);
                        rangeElement.select();
                    } else {
                        var rangeDuplicated = rangeElement.duplicate(), selectionStart, selectionEnd;
                        rangeElement.moveToBookmark(document.selection.createRange().getBookmark());
                        rangeDuplicated.setEndPoint('EndToStart', rangeElement);
                        selectionStart = rangeDuplicated.text.length;
                        selectionEnd = selectionStart + rangeElement.text.length;
                        start = [
                            selectionStart,
                            selectionEnd
                        ];
                    }
                }
            } catch (e) {
                start = [];
            }
            return start;
        };
        kendo.compileMobileDirective = function (element, scope) {
            var angular = window.angular;
            element.attr('data-' + kendo.ns + 'role', element[0].tagName.toLowerCase().replace('kendo-mobile-', '').replace('-', ''));
            angular.element(element).injector().invoke([
                '$compile',
                function ($compile) {
                    $compile(element)(scope);
                    if (!/^\$(digest|apply)$/.test(scope.$$phase)) {
                        scope.$digest();
                    }
                }
            ]);
            return kendo.widgetInstance(element, kendo.mobile.ui);
        };
        kendo.antiForgeryTokens = function () {
            var tokens = {}, csrf_token = $('meta[name=csrf-token],meta[name=_csrf]').attr('content'), csrf_param = $('meta[name=csrf-param],meta[name=_csrf_header]').attr('content');
            $('input[name^=\'__RequestVerificationToken\']').each(function () {
                tokens[this.name] = this.value;
            });
            if (csrf_param !== undefined && csrf_token !== undefined) {
                tokens[csrf_param] = csrf_token;
            }
            return tokens;
        };
        kendo.cycleForm = function (form) {
            var firstElement = form.find('input, .k-widget').first();
            var lastElement = form.find('button, .k-button').last();
            function focus(el) {
                var widget = kendo.widgetInstance(el);
                if (widget && widget.focus) {
                    widget.focus();
                } else {
                    el.focus();
                }
            }
            lastElement.on('keydown', function (e) {
                if (e.keyCode == kendo.keys.TAB && !e.shiftKey) {
                    e.preventDefault();
                    focus(firstElement);
                }
            });
            firstElement.on('keydown', function (e) {
                if (e.keyCode == kendo.keys.TAB && e.shiftKey) {
                    e.preventDefault();
                    focus(lastElement);
                }
            });
        };
        kendo.focusElement = function (element) {
            var scrollTopPositions = [];
            var scrollableParents = element.parentsUntil('body').filter(function (index, element) {
                var computedStyle = kendo.getComputedStyles(element, ['overflow']);
                return computedStyle.overflow !== 'visible';
            }).add(window);
            scrollableParents.each(function (index, parent) {
                scrollTopPositions[index] = $(parent).scrollTop();
            });
            try {
                element[0].setActive();
            } catch (e) {
                element[0].focus();
            }
            scrollableParents.each(function (index, parent) {
                $(parent).scrollTop(scrollTopPositions[index]);
            });
        };
        (function () {
            function postToProxy(dataURI, fileName, proxyURL, proxyTarget) {
                var form = $('<form>').attr({
                    action: proxyURL,
                    method: 'POST',
                    target: proxyTarget
                });
                var data = kendo.antiForgeryTokens();
                data.fileName = fileName;
                var parts = dataURI.split(';base64,');
                data.contentType = parts[0].replace('data:', '');
                data.base64 = parts[1];
                for (var name in data) {
                    if (data.hasOwnProperty(name)) {
                        $('<input>').attr({
                            value: data[name],
                            name: name,
                            type: 'hidden'
                        }).appendTo(form);
                    }
                }
                form.appendTo('body').submit().remove();
            }
            var fileSaver = document.createElement('a');
            var downloadAttribute = 'download' in fileSaver && !kendo.support.browser.edge;
            function saveAsBlob(dataURI, fileName) {
                var blob = dataURI;
                if (typeof dataURI == 'string') {
                    var parts = dataURI.split(';base64,');
                    var contentType = parts[0];
                    var base64 = atob(parts[1]);
                    var array = new Uint8Array(base64.length);
                    for (var idx = 0; idx < base64.length; idx++) {
                        array[idx] = base64.charCodeAt(idx);
                    }
                    blob = new Blob([array.buffer], { type: contentType });
                }
                navigator.msSaveBlob(blob, fileName);
            }
            function saveAsDataURI(dataURI, fileName) {
                if (window.Blob && dataURI instanceof Blob) {
                    dataURI = URL.createObjectURL(dataURI);
                }
                fileSaver.download = fileName;
                fileSaver.href = dataURI;
                var e = document.createEvent('MouseEvents');
                e.initMouseEvent('click', true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
                fileSaver.dispatchEvent(e);
                setTimeout(function () {
                    URL.revokeObjectURL(dataURI);
                });
            }
            kendo.saveAs = function (options) {
                var save = postToProxy;
                if (!options.forceProxy) {
                    if (downloadAttribute) {
                        save = saveAsDataURI;
                    } else if (navigator.msSaveBlob) {
                        save = saveAsBlob;
                    }
                }
                save(options.dataURI, options.fileName, options.proxyURL, options.proxyTarget);
            };
        }());
        kendo.proxyModelSetters = function proxyModelSetters(data) {
            var observable = {};
            Object.keys(data || {}).forEach(function (property) {
                Object.defineProperty(observable, property, {
                    get: function () {
                        return data[property];
                    },
                    set: function (value) {
                        data[property] = value;
                        data.dirty = true;
                    }
                });
            });
            return observable;
        };
    }(jQuery, window));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.fx', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'fx',
        name: 'Effects',
        category: 'framework',
        description: 'Required for animation effects in all Kendo UI widgets.',
        depends: ['core']
    };
    (function ($, undefined) {
        var kendo = window.kendo, fx = kendo.effects, each = $.each, extend = $.extend, proxy = $.proxy, support = kendo.support, browser = support.browser, transforms = support.transforms, transitions = support.transitions, scaleProperties = {
                scale: 0,
                scalex: 0,
                scaley: 0,
                scale3d: 0
            }, translateProperties = {
                translate: 0,
                translatex: 0,
                translatey: 0,
                translate3d: 0
            }, hasZoom = typeof document.documentElement.style.zoom !== 'undefined' && !transforms, matrix3dRegExp = /matrix3?d?\s*\(.*,\s*([\d\.\-]+)\w*?,\s*([\d\.\-]+)\w*?,\s*([\d\.\-]+)\w*?,\s*([\d\.\-]+)\w*?/i, cssParamsRegExp = /^(-?[\d\.\-]+)?[\w\s]*,?\s*(-?[\d\.\-]+)?[\w\s]*/i, translateXRegExp = /translatex?$/i, oldEffectsRegExp = /(zoom|fade|expand)(\w+)/, singleEffectRegExp = /(zoom|fade|expand)/, unitRegExp = /[xy]$/i, transformProps = [
                'perspective',
                'rotate',
                'rotatex',
                'rotatey',
                'rotatez',
                'rotate3d',
                'scale',
                'scalex',
                'scaley',
                'scalez',
                'scale3d',
                'skew',
                'skewx',
                'skewy',
                'translate',
                'translatex',
                'translatey',
                'translatez',
                'translate3d',
                'matrix',
                'matrix3d'
            ], transform2d = [
                'rotate',
                'scale',
                'scalex',
                'scaley',
                'skew',
                'skewx',
                'skewy',
                'translate',
                'translatex',
                'translatey',
                'matrix'
            ], transform2units = {
                'rotate': 'deg',
                scale: '',
                skew: 'px',
                translate: 'px'
            }, cssPrefix = transforms.css, round = Math.round, BLANK = '', PX = 'px', NONE = 'none', AUTO = 'auto', WIDTH = 'width', HEIGHT = 'height', HIDDEN = 'hidden', ORIGIN = 'origin', ABORT_ID = 'abortId', OVERFLOW = 'overflow', TRANSLATE = 'translate', POSITION = 'position', COMPLETE_CALLBACK = 'completeCallback', TRANSITION = cssPrefix + 'transition', TRANSFORM = cssPrefix + 'transform', BACKFACE = cssPrefix + 'backface-visibility', PERSPECTIVE = cssPrefix + 'perspective', DEFAULT_PERSPECTIVE = '1500px', TRANSFORM_PERSPECTIVE = 'perspective(' + DEFAULT_PERSPECTIVE + ')', directions = {
                left: {
                    reverse: 'right',
                    property: 'left',
                    transition: 'translatex',
                    vertical: false,
                    modifier: -1
                },
                right: {
                    reverse: 'left',
                    property: 'left',
                    transition: 'translatex',
                    vertical: false,
                    modifier: 1
                },
                down: {
                    reverse: 'up',
                    property: 'top',
                    transition: 'translatey',
                    vertical: true,
                    modifier: 1
                },
                up: {
                    reverse: 'down',
                    property: 'top',
                    transition: 'translatey',
                    vertical: true,
                    modifier: -1
                },
                top: { reverse: 'bottom' },
                bottom: { reverse: 'top' },
                'in': {
                    reverse: 'out',
                    modifier: -1
                },
                out: {
                    reverse: 'in',
                    modifier: 1
                },
                vertical: { reverse: 'vertical' },
                horizontal: { reverse: 'horizontal' }
            };
        kendo.directions = directions;
        extend($.fn, {
            kendoStop: function (clearQueue, gotoEnd) {
                if (transitions) {
                    return fx.stopQueue(this, clearQueue || false, gotoEnd || false);
                } else {
                    return this.stop(clearQueue, gotoEnd);
                }
            }
        });
        if (transforms && !transitions) {
            each(transform2d, function (idx, value) {
                $.fn[value] = function (val) {
                    if (typeof val == 'undefined') {
                        return animationProperty(this, value);
                    } else {
                        var that = $(this)[0], transformValue = value + '(' + val + transform2units[value.replace(unitRegExp, '')] + ')';
                        if (that.style.cssText.indexOf(TRANSFORM) == -1) {
                            $(this).css(TRANSFORM, transformValue);
                        } else {
                            that.style.cssText = that.style.cssText.replace(new RegExp(value + '\\(.*?\\)', 'i'), transformValue);
                        }
                    }
                    return this;
                };
                $.fx.step[value] = function (fx) {
                    $(fx.elem)[value](fx.now);
                };
            });
            var curProxy = $.fx.prototype.cur;
            $.fx.prototype.cur = function () {
                if (transform2d.indexOf(this.prop) != -1) {
                    return parseFloat($(this.elem)[this.prop]());
                }
                return curProxy.apply(this, arguments);
            };
        }
        kendo.toggleClass = function (element, classes, options, add) {
            if (classes) {
                classes = classes.split(' ');
                if (transitions) {
                    options = extend({
                        exclusive: 'all',
                        duration: 400,
                        ease: 'ease-out'
                    }, options);
                    element.css(TRANSITION, options.exclusive + ' ' + options.duration + 'ms ' + options.ease);
                    setTimeout(function () {
                        element.css(TRANSITION, '').css(HEIGHT);
                    }, options.duration);
                }
                each(classes, function (idx, value) {
                    element.toggleClass(value, add);
                });
            }
            return element;
        };
        kendo.parseEffects = function (input, mirror) {
            var effects = {};
            if (typeof input === 'string') {
                each(input.split(' '), function (idx, value) {
                    var redirectedEffect = !singleEffectRegExp.test(value), resolved = value.replace(oldEffectsRegExp, function (match, $1, $2) {
                            return $1 + ':' + $2.toLowerCase();
                        }), effect = resolved.split(':'), direction = effect[1], effectBody = {};
                    if (effect.length > 1) {
                        effectBody.direction = mirror && redirectedEffect ? directions[direction].reverse : direction;
                    }
                    effects[effect[0]] = effectBody;
                });
            } else {
                each(input, function (idx) {
                    var direction = this.direction;
                    if (direction && mirror && !singleEffectRegExp.test(idx)) {
                        this.direction = directions[direction].reverse;
                    }
                    effects[idx] = this;
                });
            }
            return effects;
        };
        function parseInteger(value) {
            return parseInt(value, 10);
        }
        function parseCSS(element, property) {
            return parseInteger(element.css(property));
        }
        function keys(obj) {
            var acc = [];
            for (var propertyName in obj) {
                acc.push(propertyName);
            }
            return acc;
        }
        function strip3DTransforms(properties) {
            for (var key in properties) {
                if (transformProps.indexOf(key) != -1 && transform2d.indexOf(key) == -1) {
                    delete properties[key];
                }
            }
            return properties;
        }
        function normalizeCSS(element, properties) {
            var transformation = [], cssValues = {}, lowerKey, key, value, isTransformed;
            for (key in properties) {
                lowerKey = key.toLowerCase();
                isTransformed = transforms && transformProps.indexOf(lowerKey) != -1;
                if (!support.hasHW3D && isTransformed && transform2d.indexOf(lowerKey) == -1) {
                    delete properties[key];
                } else {
                    value = properties[key];
                    if (isTransformed) {
                        transformation.push(key + '(' + value + ')');
                    } else {
                        cssValues[key] = value;
                    }
                }
            }
            if (transformation.length) {
                cssValues[TRANSFORM] = transformation.join(' ');
            }
            return cssValues;
        }
        if (transitions) {
            extend(fx, {
                transition: function (element, properties, options) {
                    var css, delay = 0, oldKeys = element.data('keys') || [], timeoutID;
                    options = extend({
                        duration: 200,
                        ease: 'ease-out',
                        complete: null,
                        exclusive: 'all'
                    }, options);
                    var stopTransitionCalled = false;
                    var stopTransition = function () {
                        if (!stopTransitionCalled) {
                            stopTransitionCalled = true;
                            if (timeoutID) {
                                clearTimeout(timeoutID);
                                timeoutID = null;
                            }
                            element.removeData(ABORT_ID).dequeue().css(TRANSITION, '').css(TRANSITION);
                            options.complete.call(element);
                        }
                    };
                    options.duration = $.fx ? $.fx.speeds[options.duration] || options.duration : options.duration;
                    css = normalizeCSS(element, properties);
                    $.merge(oldKeys, keys(css));
                    element.data('keys', $.unique(oldKeys)).height();
                    element.css(TRANSITION, options.exclusive + ' ' + options.duration + 'ms ' + options.ease).css(TRANSITION);
                    element.css(css).css(TRANSFORM);
                    if (transitions.event) {
                        element.one(transitions.event, stopTransition);
                        if (options.duration !== 0) {
                            delay = 500;
                        }
                    }
                    timeoutID = setTimeout(stopTransition, options.duration + delay);
                    element.data(ABORT_ID, timeoutID);
                    element.data(COMPLETE_CALLBACK, stopTransition);
                },
                stopQueue: function (element, clearQueue, gotoEnd) {
                    var cssValues, taskKeys = element.data('keys'), retainPosition = !gotoEnd && taskKeys, completeCallback = element.data(COMPLETE_CALLBACK);
                    if (retainPosition) {
                        cssValues = kendo.getComputedStyles(element[0], taskKeys);
                    }
                    if (completeCallback) {
                        completeCallback();
                    }
                    if (retainPosition) {
                        element.css(cssValues);
                    }
                    return element.removeData('keys').stop(clearQueue);
                }
            });
        }
        function animationProperty(element, property) {
            if (transforms) {
                var transform = element.css(TRANSFORM);
                if (transform == NONE) {
                    return property == 'scale' ? 1 : 0;
                }
                var match = transform.match(new RegExp(property + '\\s*\\(([\\d\\w\\.]+)')), computed = 0;
                if (match) {
                    computed = parseInteger(match[1]);
                } else {
                    match = transform.match(matrix3dRegExp) || [
                        0,
                        0,
                        0,
                        0,
                        0
                    ];
                    property = property.toLowerCase();
                    if (translateXRegExp.test(property)) {
                        computed = parseFloat(match[3] / match[2]);
                    } else if (property == 'translatey') {
                        computed = parseFloat(match[4] / match[2]);
                    } else if (property == 'scale') {
                        computed = parseFloat(match[2]);
                    } else if (property == 'rotate') {
                        computed = parseFloat(Math.atan2(match[2], match[1]));
                    }
                }
                return computed;
            } else {
                return parseFloat(element.css(property));
            }
        }
        var EffectSet = kendo.Class.extend({
            init: function (element, options) {
                var that = this;
                that.element = element;
                that.effects = [];
                that.options = options;
                that.restore = [];
            },
            run: function (effects) {
                var that = this, effect, idx, jdx, length = effects.length, element = that.element, options = that.options, deferred = $.Deferred(), start = {}, end = {}, target, children, childrenLength;
                that.effects = effects;
                deferred.done($.proxy(that, 'complete'));
                element.data('animating', true);
                for (idx = 0; idx < length; idx++) {
                    effect = effects[idx];
                    effect.setReverse(options.reverse);
                    effect.setOptions(options);
                    that.addRestoreProperties(effect.restore);
                    effect.prepare(start, end);
                    children = effect.children();
                    for (jdx = 0, childrenLength = children.length; jdx < childrenLength; jdx++) {
                        children[jdx].duration(options.duration).run();
                    }
                }
                for (var effectName in options.effects) {
                    extend(end, options.effects[effectName].properties);
                }
                if (!element.is(':visible')) {
                    extend(start, { display: element.data('olddisplay') || 'block' });
                }
                if (transforms && !options.reset) {
                    target = element.data('targetTransform');
                    if (target) {
                        start = extend(target, start);
                    }
                }
                start = normalizeCSS(element, start);
                if (transforms && !transitions) {
                    start = strip3DTransforms(start);
                }
                element.css(start).css(TRANSFORM);
                for (idx = 0; idx < length; idx++) {
                    effects[idx].setup();
                }
                if (options.init) {
                    options.init();
                }
                element.data('targetTransform', end);
                fx.animate(element, end, extend({}, options, { complete: deferred.resolve }));
                return deferred.promise();
            },
            stop: function () {
                $(this.element).kendoStop(true, true);
            },
            addRestoreProperties: function (restore) {
                var element = this.element, value, i = 0, length = restore.length;
                for (; i < length; i++) {
                    value = restore[i];
                    this.restore.push(value);
                    if (!element.data(value)) {
                        element.data(value, element.css(value));
                    }
                }
            },
            restoreCallback: function () {
                var element = this.element;
                for (var i = 0, length = this.restore.length; i < length; i++) {
                    var value = this.restore[i];
                    element.css(value, element.data(value));
                }
            },
            complete: function () {
                var that = this, idx = 0, element = that.element, options = that.options, effects = that.effects, length = effects.length;
                element.removeData('animating').dequeue();
                if (options.hide) {
                    element.data('olddisplay', element.css('display')).hide();
                }
                this.restoreCallback();
                if (hasZoom && !transforms) {
                    setTimeout($.proxy(this, 'restoreCallback'), 0);
                }
                for (; idx < length; idx++) {
                    effects[idx].teardown();
                }
                if (options.completeCallback) {
                    options.completeCallback(element);
                }
            }
        });
        fx.promise = function (element, options) {
            var effects = [], effectClass, effectSet = new EffectSet(element, options), parsedEffects = kendo.parseEffects(options.effects), effect;
            options.effects = parsedEffects;
            for (var effectName in parsedEffects) {
                effectClass = fx[capitalize(effectName)];
                if (effectClass) {
                    effect = new effectClass(element, parsedEffects[effectName].direction);
                    effects.push(effect);
                }
            }
            if (effects[0]) {
                effectSet.run(effects);
            } else {
                if (!element.is(':visible')) {
                    element.css({ display: element.data('olddisplay') || 'block' }).css('display');
                }
                if (options.init) {
                    options.init();
                }
                element.dequeue();
                effectSet.complete();
            }
        };
        extend(fx, {
            animate: function (elements, properties, options) {
                var useTransition = options.transition !== false;
                delete options.transition;
                if (transitions && 'transition' in fx && useTransition) {
                    fx.transition(elements, properties, options);
                } else {
                    if (transforms) {
                        elements.animate(strip3DTransforms(properties), {
                            queue: false,
                            show: false,
                            hide: false,
                            duration: options.duration,
                            complete: options.complete
                        });
                    } else {
                        elements.each(function () {
                            var element = $(this), multiple = {};
                            each(transformProps, function (idx, value) {
                                var params, currentValue = properties ? properties[value] + ' ' : null;
                                if (currentValue) {
                                    var single = properties;
                                    if (value in scaleProperties && properties[value] !== undefined) {
                                        params = currentValue.match(cssParamsRegExp);
                                        if (transforms) {
                                            extend(single, { scale: +params[0] });
                                        }
                                    } else {
                                        if (value in translateProperties && properties[value] !== undefined) {
                                            var position = element.css(POSITION), isFixed = position == 'absolute' || position == 'fixed';
                                            if (!element.data(TRANSLATE)) {
                                                if (isFixed) {
                                                    element.data(TRANSLATE, {
                                                        top: parseCSS(element, 'top') || 0,
                                                        left: parseCSS(element, 'left') || 0,
                                                        bottom: parseCSS(element, 'bottom'),
                                                        right: parseCSS(element, 'right')
                                                    });
                                                } else {
                                                    element.data(TRANSLATE, {
                                                        top: parseCSS(element, 'marginTop') || 0,
                                                        left: parseCSS(element, 'marginLeft') || 0
                                                    });
                                                }
                                            }
                                            var originalPosition = element.data(TRANSLATE);
                                            params = currentValue.match(cssParamsRegExp);
                                            if (params) {
                                                var dX = value == TRANSLATE + 'y' ? +null : +params[1], dY = value == TRANSLATE + 'y' ? +params[1] : +params[2];
                                                if (isFixed) {
                                                    if (!isNaN(originalPosition.right)) {
                                                        if (!isNaN(dX)) {
                                                            extend(single, { right: originalPosition.right - dX });
                                                        }
                                                    } else {
                                                        if (!isNaN(dX)) {
                                                            extend(single, { left: originalPosition.left + dX });
                                                        }
                                                    }
                                                    if (!isNaN(originalPosition.bottom)) {
                                                        if (!isNaN(dY)) {
                                                            extend(single, { bottom: originalPosition.bottom - dY });
                                                        }
                                                    } else {
                                                        if (!isNaN(dY)) {
                                                            extend(single, { top: originalPosition.top + dY });
                                                        }
                                                    }
                                                } else {
                                                    if (!isNaN(dX)) {
                                                        extend(single, { marginLeft: originalPosition.left + dX });
                                                    }
                                                    if (!isNaN(dY)) {
                                                        extend(single, { marginTop: originalPosition.top + dY });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (!transforms && value != 'scale' && value in single) {
                                        delete single[value];
                                    }
                                    if (single) {
                                        extend(multiple, single);
                                    }
                                }
                            });
                            if (browser.msie) {
                                delete multiple.scale;
                            }
                            element.animate(multiple, {
                                queue: false,
                                show: false,
                                hide: false,
                                duration: options.duration,
                                complete: options.complete
                            });
                        });
                    }
                }
            }
        });
        fx.animatedPromise = fx.promise;
        var Effect = kendo.Class.extend({
            init: function (element, direction) {
                var that = this;
                that.element = element;
                that._direction = direction;
                that.options = {};
                that._additionalEffects = [];
                if (!that.restore) {
                    that.restore = [];
                }
            },
            reverse: function () {
                this._reverse = true;
                return this.run();
            },
            play: function () {
                this._reverse = false;
                return this.run();
            },
            add: function (additional) {
                this._additionalEffects.push(additional);
                return this;
            },
            direction: function (value) {
                this._direction = value;
                return this;
            },
            duration: function (duration) {
                this._duration = duration;
                return this;
            },
            compositeRun: function () {
                var that = this, effectSet = new EffectSet(that.element, {
                        reverse: that._reverse,
                        duration: that._duration
                    }), effects = that._additionalEffects.concat([that]);
                return effectSet.run(effects);
            },
            run: function () {
                if (this._additionalEffects && this._additionalEffects[0]) {
                    return this.compositeRun();
                }
                var that = this, element = that.element, idx = 0, restore = that.restore, length = restore.length, value, deferred = $.Deferred(), start = {}, end = {}, target, children = that.children(), childrenLength = children.length;
                deferred.done($.proxy(that, '_complete'));
                element.data('animating', true);
                for (idx = 0; idx < length; idx++) {
                    value = restore[idx];
                    if (!element.data(value)) {
                        element.data(value, element.css(value));
                    }
                }
                for (idx = 0; idx < childrenLength; idx++) {
                    children[idx].duration(that._duration).run();
                }
                that.prepare(start, end);
                if (!element.is(':visible')) {
                    extend(start, { display: element.data('olddisplay') || 'block' });
                }
                if (transforms) {
                    target = element.data('targetTransform');
                    if (target) {
                        start = extend(target, start);
                    }
                }
                start = normalizeCSS(element, start);
                if (transforms && !transitions) {
                    start = strip3DTransforms(start);
                }
                element.css(start).css(TRANSFORM);
                that.setup();
                element.data('targetTransform', end);
                fx.animate(element, end, {
                    duration: that._duration,
                    complete: deferred.resolve
                });
                return deferred.promise();
            },
            stop: function () {
                var idx = 0, children = this.children(), childrenLength = children.length;
                for (idx = 0; idx < childrenLength; idx++) {
                    children[idx].stop();
                }
                $(this.element).kendoStop(true, true);
                return this;
            },
            restoreCallback: function () {
                var element = this.element;
                for (var i = 0, length = this.restore.length; i < length; i++) {
                    var value = this.restore[i];
                    element.css(value, element.data(value));
                }
            },
            _complete: function () {
                var that = this, element = that.element;
                element.removeData('animating').dequeue();
                that.restoreCallback();
                if (that.shouldHide()) {
                    element.data('olddisplay', element.css('display')).hide();
                }
                if (hasZoom && !transforms) {
                    setTimeout($.proxy(that, 'restoreCallback'), 0);
                }
                that.teardown();
            },
            setOptions: function (options) {
                extend(true, this.options, options);
            },
            children: function () {
                return [];
            },
            shouldHide: $.noop,
            setup: $.noop,
            prepare: $.noop,
            teardown: $.noop,
            directions: [],
            setReverse: function (reverse) {
                this._reverse = reverse;
                return this;
            }
        });
        function capitalize(word) {
            return word.charAt(0).toUpperCase() + word.substring(1);
        }
        function createEffect(name, definition) {
            var effectClass = Effect.extend(definition), directions = effectClass.prototype.directions;
            fx[capitalize(name)] = effectClass;
            fx.Element.prototype[name] = function (direction, opt1, opt2, opt3) {
                return new effectClass(this.element, direction, opt1, opt2, opt3);
            };
            each(directions, function (idx, theDirection) {
                fx.Element.prototype[name + capitalize(theDirection)] = function (opt1, opt2, opt3) {
                    return new effectClass(this.element, theDirection, opt1, opt2, opt3);
                };
            });
        }
        var FOUR_DIRECTIONS = [
                'left',
                'right',
                'up',
                'down'
            ], IN_OUT = [
                'in',
                'out'
            ];
        createEffect('slideIn', {
            directions: FOUR_DIRECTIONS,
            divisor: function (value) {
                this.options.divisor = value;
                return this;
            },
            prepare: function (start, end) {
                var that = this, tmp, element = that.element, outerWidth = kendo._outerWidth, outerHeight = kendo._outerHeight, direction = directions[that._direction], offset = -direction.modifier * (direction.vertical ? outerHeight(element) : outerWidth(element)), startValue = offset / (that.options && that.options.divisor || 1) + PX, endValue = '0px';
                if (that._reverse) {
                    tmp = start;
                    start = end;
                    end = tmp;
                }
                if (transforms) {
                    start[direction.transition] = startValue;
                    end[direction.transition] = endValue;
                } else {
                    start[direction.property] = startValue;
                    end[direction.property] = endValue;
                }
            }
        });
        createEffect('tile', {
            directions: FOUR_DIRECTIONS,
            init: function (element, direction, previous) {
                Effect.prototype.init.call(this, element, direction);
                this.options = { previous: previous };
            },
            previousDivisor: function (value) {
                this.options.previousDivisor = value;
                return this;
            },
            children: function () {
                var that = this, reverse = that._reverse, previous = that.options.previous, divisor = that.options.previousDivisor || 1, dir = that._direction;
                var children = [kendo.fx(that.element).slideIn(dir).setReverse(reverse)];
                if (previous) {
                    children.push(kendo.fx(previous).slideIn(directions[dir].reverse).divisor(divisor).setReverse(!reverse));
                }
                return children;
            }
        });
        function createToggleEffect(name, property, defaultStart, defaultEnd) {
            createEffect(name, {
                directions: IN_OUT,
                startValue: function (value) {
                    this._startValue = value;
                    return this;
                },
                endValue: function (value) {
                    this._endValue = value;
                    return this;
                },
                shouldHide: function () {
                    return this._shouldHide;
                },
                prepare: function (start, end) {
                    var that = this, startValue, endValue, out = this._direction === 'out', startDataValue = that.element.data(property), startDataValueIsSet = !(isNaN(startDataValue) || startDataValue == defaultStart);
                    if (startDataValueIsSet) {
                        startValue = startDataValue;
                    } else if (typeof this._startValue !== 'undefined') {
                        startValue = this._startValue;
                    } else {
                        startValue = out ? defaultStart : defaultEnd;
                    }
                    if (typeof this._endValue !== 'undefined') {
                        endValue = this._endValue;
                    } else {
                        endValue = out ? defaultEnd : defaultStart;
                    }
                    if (this._reverse) {
                        start[property] = endValue;
                        end[property] = startValue;
                    } else {
                        start[property] = startValue;
                        end[property] = endValue;
                    }
                    that._shouldHide = end[property] === defaultEnd;
                }
            });
        }
        createToggleEffect('fade', 'opacity', 1, 0);
        createToggleEffect('zoom', 'scale', 1, 0.01);
        createEffect('slideMargin', {
            prepare: function (start, end) {
                var that = this, element = that.element, options = that.options, origin = element.data(ORIGIN), offset = options.offset, margin, reverse = that._reverse;
                if (!reverse && origin === null) {
                    element.data(ORIGIN, parseFloat(element.css('margin-' + options.axis)));
                }
                margin = element.data(ORIGIN) || 0;
                end['margin-' + options.axis] = !reverse ? margin + offset : margin;
            }
        });
        createEffect('slideTo', {
            prepare: function (start, end) {
                var that = this, element = that.element, options = that.options, offset = options.offset.split(','), reverse = that._reverse;
                if (transforms) {
                    end.translatex = !reverse ? offset[0] : 0;
                    end.translatey = !reverse ? offset[1] : 0;
                } else {
                    end.left = !reverse ? offset[0] : 0;
                    end.top = !reverse ? offset[1] : 0;
                }
                element.css('left');
            }
        });
        createEffect('expand', {
            directions: [
                'horizontal',
                'vertical'
            ],
            restore: [OVERFLOW],
            prepare: function (start, end) {
                var that = this, element = that.element, options = that.options, reverse = that._reverse, property = that._direction === 'vertical' ? HEIGHT : WIDTH, setLength = element[0].style[property], oldLength = element.data(property), length = parseFloat(oldLength || setLength), realLength = round(element.css(property, AUTO)[property]());
                start.overflow = HIDDEN;
                length = options && options.reset ? realLength || length : length || realLength;
                end[property] = (reverse ? 0 : length) + PX;
                start[property] = (reverse ? length : 0) + PX;
                if (oldLength === undefined) {
                    element.data(property, setLength);
                }
            },
            shouldHide: function () {
                return this._reverse;
            },
            teardown: function () {
                var that = this, element = that.element, property = that._direction === 'vertical' ? HEIGHT : WIDTH, length = element.data(property);
                if (length == AUTO || length === BLANK) {
                    setTimeout(function () {
                        element.css(property, AUTO).css(property);
                    }, 0);
                }
            }
        });
        var TRANSFER_START_STATE = {
            position: 'absolute',
            marginLeft: 0,
            marginTop: 0,
            scale: 1
        };
        createEffect('transfer', {
            init: function (element, target) {
                this.element = element;
                this.options = { target: target };
                this.restore = [];
            },
            setup: function () {
                this.element.appendTo(document.body);
            },
            prepare: function (start, end) {
                var that = this, element = that.element, outerBox = fx.box(element), innerBox = fx.box(that.options.target), currentScale = animationProperty(element, 'scale'), scale = fx.fillScale(innerBox, outerBox), transformOrigin = fx.transformOrigin(innerBox, outerBox);
                extend(start, TRANSFER_START_STATE);
                end.scale = 1;
                element.css(TRANSFORM, 'scale(1)').css(TRANSFORM);
                element.css(TRANSFORM, 'scale(' + currentScale + ')');
                start.top = outerBox.top;
                start.left = outerBox.left;
                start.transformOrigin = transformOrigin.x + PX + ' ' + transformOrigin.y + PX;
                if (that._reverse) {
                    start.scale = scale;
                } else {
                    end.scale = scale;
                }
            }
        });
        var CLIPS = {
            top: 'rect(auto auto $size auto)',
            bottom: 'rect($size auto auto auto)',
            left: 'rect(auto $size auto auto)',
            right: 'rect(auto auto auto $size)'
        };
        var ROTATIONS = {
            top: {
                start: 'rotatex(0deg)',
                end: 'rotatex(180deg)'
            },
            bottom: {
                start: 'rotatex(-180deg)',
                end: 'rotatex(0deg)'
            },
            left: {
                start: 'rotatey(0deg)',
                end: 'rotatey(-180deg)'
            },
            right: {
                start: 'rotatey(180deg)',
                end: 'rotatey(0deg)'
            }
        };
        function clipInHalf(container, direction) {
            var vertical = kendo.directions[direction].vertical, size = container[vertical ? HEIGHT : WIDTH]() / 2 + 'px';
            return CLIPS[direction].replace('$size', size);
        }
        createEffect('turningPage', {
            directions: FOUR_DIRECTIONS,
            init: function (element, direction, container) {
                Effect.prototype.init.call(this, element, direction);
                this._container = container;
            },
            prepare: function (start, end) {
                var that = this, reverse = that._reverse, direction = reverse ? directions[that._direction].reverse : that._direction, rotation = ROTATIONS[direction];
                start.zIndex = 1;
                if (that._clipInHalf) {
                    start.clip = clipInHalf(that._container, kendo.directions[direction].reverse);
                }
                start[BACKFACE] = HIDDEN;
                end[TRANSFORM] = TRANSFORM_PERSPECTIVE + (reverse ? rotation.start : rotation.end);
                start[TRANSFORM] = TRANSFORM_PERSPECTIVE + (reverse ? rotation.end : rotation.start);
            },
            setup: function () {
                this._container.append(this.element);
            },
            face: function (value) {
                this._face = value;
                return this;
            },
            shouldHide: function () {
                var that = this, reverse = that._reverse, face = that._face;
                return reverse && !face || !reverse && face;
            },
            clipInHalf: function (value) {
                this._clipInHalf = value;
                return this;
            },
            temporary: function () {
                this.element.addClass('temp-page');
                return this;
            }
        });
        createEffect('staticPage', {
            directions: FOUR_DIRECTIONS,
            init: function (element, direction, container) {
                Effect.prototype.init.call(this, element, direction);
                this._container = container;
            },
            restore: ['clip'],
            prepare: function (start, end) {
                var that = this, direction = that._reverse ? directions[that._direction].reverse : that._direction;
                start.clip = clipInHalf(that._container, direction);
                start.opacity = 0.999;
                end.opacity = 1;
            },
            shouldHide: function () {
                var that = this, reverse = that._reverse, face = that._face;
                return reverse && !face || !reverse && face;
            },
            face: function (value) {
                this._face = value;
                return this;
            }
        });
        createEffect('pageturn', {
            directions: [
                'horizontal',
                'vertical'
            ],
            init: function (element, direction, face, back) {
                Effect.prototype.init.call(this, element, direction);
                this.options = {};
                this.options.face = face;
                this.options.back = back;
            },
            children: function () {
                var that = this, options = that.options, direction = that._direction === 'horizontal' ? 'left' : 'top', reverseDirection = kendo.directions[direction].reverse, reverse = that._reverse, temp, faceClone = options.face.clone(true).removeAttr('id'), backClone = options.back.clone(true).removeAttr('id'), element = that.element;
                if (reverse) {
                    temp = direction;
                    direction = reverseDirection;
                    reverseDirection = temp;
                }
                return [
                    kendo.fx(options.face).staticPage(direction, element).face(true).setReverse(reverse),
                    kendo.fx(options.back).staticPage(reverseDirection, element).setReverse(reverse),
                    kendo.fx(faceClone).turningPage(direction, element).face(true).clipInHalf(true).temporary().setReverse(reverse),
                    kendo.fx(backClone).turningPage(reverseDirection, element).clipInHalf(true).temporary().setReverse(reverse)
                ];
            },
            prepare: function (start, end) {
                start[PERSPECTIVE] = DEFAULT_PERSPECTIVE;
                start.transformStyle = 'preserve-3d';
                start.opacity = 0.999;
                end.opacity = 1;
            },
            teardown: function () {
                this.element.find('.temp-page').remove();
            }
        });
        createEffect('flip', {
            directions: [
                'horizontal',
                'vertical'
            ],
            init: function (element, direction, face, back) {
                Effect.prototype.init.call(this, element, direction);
                this.options = {};
                this.options.face = face;
                this.options.back = back;
            },
            children: function () {
                var that = this, options = that.options, direction = that._direction === 'horizontal' ? 'left' : 'top', reverseDirection = kendo.directions[direction].reverse, reverse = that._reverse, temp, element = that.element;
                if (reverse) {
                    temp = direction;
                    direction = reverseDirection;
                    reverseDirection = temp;
                }
                return [
                    kendo.fx(options.face).turningPage(direction, element).face(true).setReverse(reverse),
                    kendo.fx(options.back).turningPage(reverseDirection, element).setReverse(reverse)
                ];
            },
            prepare: function (start) {
                start[PERSPECTIVE] = DEFAULT_PERSPECTIVE;
                start.transformStyle = 'preserve-3d';
            }
        });
        var RESTORE_OVERFLOW = !support.mobileOS.android;
        var IGNORE_TRANSITION_EVENT_SELECTOR = '.km-touch-scrollbar, .km-actionsheet-wrapper';
        createEffect('replace', {
            _before: $.noop,
            _after: $.noop,
            init: function (element, previous, transitionClass) {
                Effect.prototype.init.call(this, element);
                this._previous = $(previous);
                this._transitionClass = transitionClass;
            },
            duration: function () {
                throw new Error('The replace effect does not support duration setting; the effect duration may be customized through the transition class rule');
            },
            beforeTransition: function (callback) {
                this._before = callback;
                return this;
            },
            afterTransition: function (callback) {
                this._after = callback;
                return this;
            },
            _both: function () {
                return $().add(this._element).add(this._previous);
            },
            _containerClass: function () {
                var direction = this._direction, containerClass = 'k-fx k-fx-start k-fx-' + this._transitionClass;
                if (direction) {
                    containerClass += ' k-fx-' + direction;
                }
                if (this._reverse) {
                    containerClass += ' k-fx-reverse';
                }
                return containerClass;
            },
            complete: function (e) {
                if (!this.deferred || e && $(e.target).is(IGNORE_TRANSITION_EVENT_SELECTOR)) {
                    return;
                }
                var container = this.container;
                container.removeClass('k-fx-end').removeClass(this._containerClass()).off(transitions.event, this.completeProxy);
                this._previous.hide().removeClass('k-fx-current');
                this.element.removeClass('k-fx-next');
                if (RESTORE_OVERFLOW) {
                    container.css(OVERFLOW, '');
                }
                if (!this.isAbsolute) {
                    this._both().css(POSITION, '');
                }
                this.deferred.resolve();
                delete this.deferred;
            },
            run: function () {
                if (this._additionalEffects && this._additionalEffects[0]) {
                    return this.compositeRun();
                }
                var that = this, element = that.element, previous = that._previous, container = element.parents().filter(previous.parents()).first(), both = that._both(), deferred = $.Deferred(), originalPosition = element.css(POSITION), originalOverflow;
                if (!container.length) {
                    container = element.parent();
                }
                this.container = container;
                this.deferred = deferred;
                this.isAbsolute = originalPosition == 'absolute';
                if (!this.isAbsolute) {
                    both.css(POSITION, 'absolute');
                }
                if (RESTORE_OVERFLOW) {
                    originalOverflow = container.css(OVERFLOW);
                    container.css(OVERFLOW, 'hidden');
                }
                if (!transitions) {
                    this.complete();
                } else {
                    element.addClass('k-fx-hidden');
                    container.addClass(this._containerClass());
                    this.completeProxy = $.proxy(this, 'complete');
                    container.on(transitions.event, this.completeProxy);
                    kendo.animationFrame(function () {
                        element.removeClass('k-fx-hidden').addClass('k-fx-next');
                        previous.css('display', '').addClass('k-fx-current');
                        that._before(previous, element);
                        kendo.animationFrame(function () {
                            container.removeClass('k-fx-start').addClass('k-fx-end');
                            that._after(previous, element);
                        });
                    });
                }
                return deferred.promise();
            },
            stop: function () {
                this.complete();
            }
        });
        var Animation = kendo.Class.extend({
            init: function () {
                var that = this;
                that._tickProxy = proxy(that._tick, that);
                that._started = false;
            },
            tick: $.noop,
            done: $.noop,
            onEnd: $.noop,
            onCancel: $.noop,
            start: function () {
                if (!this.enabled()) {
                    return;
                }
                if (!this.done()) {
                    this._started = true;
                    kendo.animationFrame(this._tickProxy);
                } else {
                    this.onEnd();
                }
            },
            enabled: function () {
                return true;
            },
            cancel: function () {
                this._started = false;
                this.onCancel();
            },
            _tick: function () {
                var that = this;
                if (!that._started) {
                    return;
                }
                that.tick();
                if (!that.done()) {
                    kendo.animationFrame(that._tickProxy);
                } else {
                    that._started = false;
                    that.onEnd();
                }
            }
        });
        var Transition = Animation.extend({
            init: function (options) {
                var that = this;
                extend(that, options);
                Animation.fn.init.call(that);
            },
            done: function () {
                return this.timePassed() >= this.duration;
            },
            timePassed: function () {
                return Math.min(this.duration, new Date() - this.startDate);
            },
            moveTo: function (options) {
                var that = this, movable = that.movable;
                that.initial = movable[that.axis];
                that.delta = options.location - that.initial;
                that.duration = typeof options.duration == 'number' ? options.duration : 300;
                that.tick = that._easeProxy(options.ease);
                that.startDate = new Date();
                that.start();
            },
            _easeProxy: function (ease) {
                var that = this;
                return function () {
                    that.movable.moveAxis(that.axis, ease(that.timePassed(), that.initial, that.delta, that.duration));
                };
            }
        });
        extend(Transition, {
            easeOutExpo: function (t, b, c, d) {
                return t == d ? b + c : c * (-Math.pow(2, -10 * t / d) + 1) + b;
            },
            easeOutBack: function (t, b, c, d, s) {
                s = 1.70158;
                return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
            }
        });
        fx.Animation = Animation;
        fx.Transition = Transition;
        fx.createEffect = createEffect;
        fx.box = function (element) {
            element = $(element);
            var result = element.offset();
            result.width = kendo._outerWidth(element);
            result.height = kendo._outerHeight(element);
            return result;
        };
        fx.transformOrigin = function (inner, outer) {
            var x = (inner.left - outer.left) * outer.width / (outer.width - inner.width), y = (inner.top - outer.top) * outer.height / (outer.height - inner.height);
            return {
                x: isNaN(x) ? 0 : x,
                y: isNaN(y) ? 0 : y
            };
        };
        fx.fillScale = function (inner, outer) {
            return Math.min(inner.width / outer.width, inner.height / outer.height);
        };
        fx.fitScale = function (inner, outer) {
            return Math.max(inner.width / outer.width, inner.height / outer.height);
        };
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.data.odata', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'data.odata',
        name: 'OData',
        category: 'framework',
        depends: ['core'],
        hidden: true
    };
    (function ($, undefined) {
        var kendo = window.kendo, extend = $.extend, NEWLINE = '\r\n', DOUBLELINE = '\r\n\r\n', isFunction = kendo.isFunction, odataFilters = {
                eq: 'eq',
                neq: 'ne',
                gt: 'gt',
                gte: 'ge',
                lt: 'lt',
                lte: 'le',
                contains: 'substringof',
                doesnotcontain: 'substringof',
                endswith: 'endswith',
                startswith: 'startswith',
                isnull: 'eq',
                isnotnull: 'ne',
                isempty: 'eq',
                isnotempty: 'ne'
            }, odataFiltersVersionFour = extend({}, odataFilters, { contains: 'contains' }), mappers = {
                pageSize: $.noop,
                page: $.noop,
                filter: function (params, filter, useVersionFour) {
                    if (filter) {
                        filter = toOdataFilter(filter, useVersionFour);
                        if (filter) {
                            params.$filter = filter;
                        }
                    }
                },
                sort: function (params, orderby) {
                    var expr = $.map(orderby, function (value) {
                        var order = value.field.replace(/\./g, '/');
                        if (value.dir === 'desc') {
                            order += ' desc';
                        }
                        return order;
                    }).join(',');
                    if (expr) {
                        params.$orderby = expr;
                    }
                },
                skip: function (params, skip) {
                    if (skip) {
                        params.$skip = skip;
                    }
                },
                take: function (params, take) {
                    if (take) {
                        params.$top = take;
                    }
                }
            }, defaultDataType = { read: { dataType: 'jsonp' } };
        function toOdataFilter(filter, useOdataFour) {
            var result = [], logic = filter.logic || 'and', idx, length, field, type, format, operator, value, ignoreCase, filters = filter.filters;
            for (idx = 0, length = filters.length; idx < length; idx++) {
                filter = filters[idx];
                field = filter.field;
                value = filter.value;
                operator = filter.operator;
                if (filter.filters) {
                    filter = toOdataFilter(filter, useOdataFour);
                } else {
                    ignoreCase = filter.ignoreCase;
                    field = field.replace(/\./g, '/');
                    filter = odataFilters[operator];
                    if (useOdataFour) {
                        filter = odataFiltersVersionFour[operator];
                    }
                    if (operator === 'isnull' || operator === 'isnotnull') {
                        filter = kendo.format('{0} {1} null', field, filter);
                    } else if (operator === 'isempty' || operator === 'isnotempty') {
                        filter = kendo.format('{0} {1} \'\'', field, filter);
                    } else if (filter && value !== undefined) {
                        type = $.type(value);
                        if (type === 'string') {
                            format = '\'{1}\'';
                            value = value.replace(/'/g, '\'\'');
                            if (ignoreCase === true) {
                                field = 'tolower(' + field + ')';
                            }
                        } else if (type === 'date') {
                            if (useOdataFour) {
                                format = '{1:yyyy-MM-ddTHH:mm:ss+00:00}';
                                value = kendo.timezone.apply(value, 'Etc/UTC');
                            } else {
                                format = 'datetime\'{1:yyyy-MM-ddTHH:mm:ss}\'';
                            }
                        } else {
                            format = '{1}';
                        }
                        if (filter.length > 3) {
                            if (filter !== 'substringof') {
                                format = '{0}({2},' + format + ')';
                            } else {
                                format = '{0}(' + format + ',{2})';
                                if (operator === 'doesnotcontain') {
                                    if (useOdataFour) {
                                        format = '{0}({2},\'{1}\') eq -1';
                                        filter = 'indexof';
                                    } else {
                                        format += ' eq false';
                                    }
                                }
                            }
                        } else {
                            format = '{2} {0} ' + format;
                        }
                        filter = kendo.format(format, filter, value, field);
                    }
                }
                result.push(filter);
            }
            filter = result.join(' ' + logic + ' ');
            if (result.length > 1) {
                filter = '(' + filter + ')';
            }
            return filter;
        }
        function stripMetadata(obj) {
            for (var name in obj) {
                if (name.indexOf('@odata') === 0) {
                    delete obj[name];
                }
            }
        }
        function hex16() {
            return Math.floor((1 + Math.random()) * 65536).toString(16).substr(1);
        }
        function createBoundary(prefix) {
            return prefix + hex16() + '-' + hex16() + '-' + hex16();
        }
        function createDelimeter(boundary, close) {
            var result = NEWLINE + '--' + boundary;
            if (close) {
                result += '--';
            }
            return result;
        }
        function createCommand(transport, item, httpVerb, command) {
            var transportUrl = transport.options[command].url;
            var commandPrefix = kendo.format('{0} ', httpVerb);
            if (isFunction(transportUrl)) {
                return commandPrefix + transportUrl(item);
            } else {
                return commandPrefix + transportUrl;
            }
        }
        function getOperationHeader(changeset, changeId) {
            var header = '';
            header += createDelimeter(changeset, false);
            header += NEWLINE + 'Content-Type: application/http';
            header += NEWLINE + 'Content-Transfer-Encoding: binary';
            header += NEWLINE + 'Content-ID: ' + changeId;
            return header;
        }
        function getOperationContent(item) {
            var content = '';
            content += NEWLINE + 'Content-Type: application/json;odata=minimalmetadata';
            content += NEWLINE + 'Prefer: return=representation';
            content += DOUBLELINE + kendo.stringify(item);
            return content;
        }
        function getOperations(collection, changeset, changeId, command, transport, skipContent) {
            var requestBody = '';
            for (var i = 0; i < collection.length; i++) {
                requestBody += getOperationHeader(changeset, changeId);
                requestBody += DOUBLELINE + createCommand(transport, collection[i], transport.options[command].type, command) + ' HTTP/1.1';
                if (!skipContent) {
                    requestBody += getOperationContent(collection[i]);
                }
                requestBody += NEWLINE;
                changeId++;
            }
            return requestBody;
        }
        function processCollection(colection, boundary, changeset, changeId, transport, command, skipContent) {
            var requestBody = '';
            requestBody += getBoundary(boundary, changeset);
            requestBody += getOperations(colection, changeset, changeId, command, transport, skipContent);
            requestBody += createDelimeter(changeset, true);
            requestBody += NEWLINE;
            return requestBody;
        }
        function getBoundary(boundary, changeset) {
            var requestBody = '';
            requestBody += '--' + boundary + NEWLINE;
            requestBody += 'Content-Type: multipart/mixed; boundary=' + changeset + NEWLINE;
            return requestBody;
        }
        function createBatchRequest(transport, colections) {
            var options = {};
            var boundary = createBoundary('sf_batch_');
            var requestBody = '';
            var changeId = 0;
            var batchURL = transport.options.batch.url;
            var changeset = createBoundary('sf_changeset_');
            options.type = transport.options.batch.type;
            options.url = isFunction(batchURL) ? batchURL() : batchURL;
            options.headers = { 'Content-Type': 'multipart/mixed; boundary=' + boundary };
            if (colections.updated.length) {
                requestBody += processCollection(colections.updated, boundary, changeset, changeId, transport, 'update', false);
                changeId += colections.updated.length;
                changeset = createBoundary('sf_changeset_');
            }
            if (colections.destroyed.length) {
                requestBody += processCollection(colections.destroyed, boundary, changeset, changeId, transport, 'destroy', true);
                changeId += colections.destroyed.length;
                changeset = createBoundary('sf_changeset_');
            }
            if (colections.created.length) {
                requestBody += processCollection(colections.created, boundary, changeset, changeId, transport, 'create', false);
            }
            requestBody += createDelimeter(boundary, true);
            options.data = requestBody;
            return options;
        }
        function parseBatchResponse(responseText) {
            var responseMarkers = responseText.match(/--changesetresponse_[a-z0-9-]+$/gm);
            var markerIndex = 0;
            var collections = [];
            var changeBody;
            var status;
            var code;
            var marker;
            var jsonModel;
            collections.push({
                models: [],
                passed: true
            });
            for (var i = 0; i < responseMarkers.length; i++) {
                marker = responseMarkers[i];
                if (marker.lastIndexOf('--', marker.length - 1)) {
                    if (i < responseMarkers.length - 1) {
                        collections.push({
                            models: [],
                            passed: true
                        });
                    }
                    continue;
                }
                if (!markerIndex) {
                    markerIndex = responseText.indexOf(marker);
                } else {
                    markerIndex = responseText.indexOf(marker, markerIndex + marker.length);
                }
                changeBody = responseText.substring(markerIndex, responseText.indexOf('--', markerIndex + 1));
                status = changeBody.match(/^HTTP\/1\.\d (\d{3}) (.*)$/gm).pop();
                code = kendo.parseFloat(status.match(/\d{3}/g).pop());
                if (code >= 200 && code <= 299) {
                    jsonModel = changeBody.match(/\{.*\}/gm);
                    if (jsonModel) {
                        collections[collections.length - 1].models.push(JSON.parse(jsonModel[0]));
                    }
                } else {
                    collections[collections.length - 1].passed = false;
                }
            }
            return collections;
        }
        extend(true, kendo.data, {
            schemas: {
                odata: {
                    type: 'json',
                    data: function (data) {
                        return data.d.results || [data.d];
                    },
                    total: 'd.__count'
                }
            },
            transports: {
                odata: {
                    read: {
                        cache: true,
                        dataType: 'jsonp',
                        jsonp: '$callback'
                    },
                    update: {
                        cache: true,
                        dataType: 'json',
                        contentType: 'application/json',
                        type: 'PUT'
                    },
                    create: {
                        cache: true,
                        dataType: 'json',
                        contentType: 'application/json',
                        type: 'POST'
                    },
                    destroy: {
                        cache: true,
                        dataType: 'json',
                        type: 'DELETE'
                    },
                    parameterMap: function (options, type, useVersionFour) {
                        var params, value, option, dataType;
                        options = options || {};
                        type = type || 'read';
                        dataType = (this.options || defaultDataType)[type];
                        dataType = dataType ? dataType.dataType : 'json';
                        if (type === 'read') {
                            params = { $inlinecount: 'allpages' };
                            if (dataType != 'json') {
                                params.$format = 'json';
                            }
                            for (option in options) {
                                if (mappers[option]) {
                                    mappers[option](params, options[option], useVersionFour);
                                } else {
                                    params[option] = options[option];
                                }
                            }
                        } else {
                            if (dataType !== 'json') {
                                throw new Error('Only json dataType can be used for ' + type + ' operation.');
                            }
                            if (type !== 'destroy') {
                                for (option in options) {
                                    value = options[option];
                                    if (typeof value === 'number') {
                                        options[option] = value + '';
                                    }
                                }
                                params = kendo.stringify(options);
                            }
                        }
                        return params;
                    }
                }
            }
        });
        extend(true, kendo.data, {
            schemas: {
                'odata-v4': {
                    type: 'json',
                    data: function (data) {
                        if ($.isArray(data)) {
                            for (var i = 0; i < data.length; i++) {
                                stripMetadata(data[i]);
                            }
                            return data;
                        } else {
                            data = $.extend({}, data);
                            stripMetadata(data);
                            if (data.value) {
                                return data.value;
                            }
                            return [data];
                        }
                    },
                    total: function (data) {
                        return data['@odata.count'];
                    }
                }
            },
            transports: {
                'odata-v4': {
                    batch: { type: 'POST' },
                    read: {
                        cache: true,
                        dataType: 'json'
                    },
                    update: {
                        cache: true,
                        dataType: 'json',
                        contentType: 'application/json;IEEE754Compatible=true',
                        type: 'PUT'
                    },
                    create: {
                        cache: true,
                        dataType: 'json',
                        contentType: 'application/json;IEEE754Compatible=true',
                        type: 'POST'
                    },
                    destroy: {
                        cache: true,
                        dataType: 'json',
                        type: 'DELETE'
                    },
                    parameterMap: function (options, type) {
                        var result = kendo.data.transports.odata.parameterMap(options, type, true);
                        if (type == 'read') {
                            result.$count = true;
                            delete result.$inlinecount;
                        }
                        return result;
                    },
                    submit: function (e) {
                        var that = this;
                        var options = createBatchRequest(that, e.data);
                        var collections = e.data;
                        if (!collections.updated.length && !collections.destroyed.length && !collections.created.length) {
                            return;
                        }
                        $.ajax(extend(true, {}, {
                            success: function (response) {
                                var responses = parseBatchResponse(response);
                                var index = 0;
                                var current;
                                if (collections.updated.length) {
                                    current = responses[index];
                                    if (current.passed) {
                                        e.success(current.models.length ? current.models : [], 'update');
                                    }
                                    index++;
                                }
                                if (collections.destroyed.length) {
                                    current = responses[index];
                                    if (current.passed) {
                                        e.success([], 'destroy');
                                    }
                                    index++;
                                }
                                if (collections.created.length) {
                                    current = responses[index];
                                    if (current.passed) {
                                        e.success(current.models, 'create');
                                    }
                                }
                            },
                            error: function (response, status, error) {
                                e.error(response, status, error);
                            }
                        }, options));
                    }
                }
            }
        });
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.data.xml', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'data.xml',
        name: 'XML',
        category: 'framework',
        depends: ['core'],
        hidden: true
    };
    (function ($, undefined) {
        var kendo = window.kendo, isArray = $.isArray, isPlainObject = $.isPlainObject, map = $.map, each = $.each, extend = $.extend, getter = kendo.getter, Class = kendo.Class;
        var XmlDataReader = Class.extend({
            init: function (options) {
                var that = this, total = options.total, model = options.model, parse = options.parse, errors = options.errors, serialize = options.serialize, data = options.data;
                if (model) {
                    if (isPlainObject(model)) {
                        var base = options.modelBase || kendo.data.Model;
                        if (model.fields) {
                            each(model.fields, function (field, value) {
                                if (isPlainObject(value) && value.field) {
                                    if (!$.isFunction(value.field)) {
                                        value = extend(value, { field: that.getter(value.field) });
                                    }
                                } else {
                                    value = { field: that.getter(value) };
                                }
                                model.fields[field] = value;
                            });
                        }
                        var id = model.id;
                        if (id) {
                            var idField = {};
                            idField[that.xpathToMember(id, true)] = { field: that.getter(id) };
                            model.fields = extend(idField, model.fields);
                            model.id = that.xpathToMember(id);
                        }
                        model = base.define(model);
                    }
                    that.model = model;
                }
                if (total) {
                    if (typeof total == 'string') {
                        total = that.getter(total);
                        that.total = function (data) {
                            return parseInt(total(data), 10);
                        };
                    } else if (typeof total == 'function') {
                        that.total = total;
                    }
                }
                if (errors) {
                    if (typeof errors == 'string') {
                        errors = that.getter(errors);
                        that.errors = function (data) {
                            return errors(data) || null;
                        };
                    } else if (typeof errors == 'function') {
                        that.errors = errors;
                    }
                }
                if (data) {
                    if (typeof data == 'string') {
                        data = that.xpathToMember(data);
                        that.data = function (value) {
                            var result = that.evaluate(value, data), modelInstance;
                            result = isArray(result) ? result : [result];
                            if (that.model && model.fields) {
                                modelInstance = new that.model();
                                return map(result, function (value) {
                                    if (value) {
                                        var record = {}, field;
                                        for (field in model.fields) {
                                            record[field] = modelInstance._parse(field, model.fields[field].field(value));
                                        }
                                        return record;
                                    }
                                });
                            }
                            return result;
                        };
                    } else if (typeof data == 'function') {
                        that.data = data;
                    }
                }
                if (typeof parse == 'function') {
                    var xmlParse = that.parse;
                    that.parse = function (data) {
                        var xml = parse.call(that, data);
                        return xmlParse.call(that, xml);
                    };
                }
                if (typeof serialize == 'function') {
                    that.serialize = serialize;
                }
            },
            total: function (result) {
                return this.data(result).length;
            },
            errors: function (data) {
                return data ? data.errors : null;
            },
            serialize: function (data) {
                return data;
            },
            parseDOM: function (element) {
                var result = {}, parsedNode, node, nodeType, nodeName, member, attribute, attributes = element.attributes, attributeCount = attributes.length, idx;
                for (idx = 0; idx < attributeCount; idx++) {
                    attribute = attributes[idx];
                    result['@' + attribute.nodeName] = attribute.nodeValue;
                }
                for (node = element.firstChild; node; node = node.nextSibling) {
                    nodeType = node.nodeType;
                    if (nodeType === 3 || nodeType === 4) {
                        result['#text'] = node.nodeValue;
                    } else if (nodeType === 1) {
                        parsedNode = this.parseDOM(node);
                        nodeName = node.nodeName;
                        member = result[nodeName];
                        if (isArray(member)) {
                            member.push(parsedNode);
                        } else if (member !== undefined) {
                            member = [
                                member,
                                parsedNode
                            ];
                        } else {
                            member = parsedNode;
                        }
                        result[nodeName] = member;
                    }
                }
                return result;
            },
            evaluate: function (value, expression) {
                var members = expression.split('.'), member, result, length, intermediateResult, idx;
                while (member = members.shift()) {
                    value = value[member];
                    if (isArray(value)) {
                        result = [];
                        expression = members.join('.');
                        for (idx = 0, length = value.length; idx < length; idx++) {
                            intermediateResult = this.evaluate(value[idx], expression);
                            intermediateResult = isArray(intermediateResult) ? intermediateResult : [intermediateResult];
                            result.push.apply(result, intermediateResult);
                        }
                        return result;
                    }
                }
                return value;
            },
            parse: function (xml) {
                var documentElement, tree, result = {};
                documentElement = xml.documentElement || $.parseXML(xml).documentElement;
                tree = this.parseDOM(documentElement);
                result[documentElement.nodeName] = tree;
                return result;
            },
            xpathToMember: function (member, raw) {
                if (!member) {
                    return '';
                }
                member = member.replace(/^\//, '').replace(/\//g, '.');
                if (member.indexOf('@') >= 0) {
                    return member.replace(/\.?(@.*)/, raw ? '$1' : '["$1"]');
                }
                if (member.indexOf('text()') >= 0) {
                    return member.replace(/(\.?text\(\))/, raw ? '#text' : '["#text"]');
                }
                return member;
            },
            getter: function (member) {
                return getter(this.xpathToMember(member), true);
            }
        });
        $.extend(true, kendo.data, {
            XmlDataReader: XmlDataReader,
            readers: { xml: XmlDataReader }
        });
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.data', [
        'kendo.core',
        'kendo.data.odata',
        'kendo.data.xml'
    ], f);
}(function () {
    var __meta__ = {
        id: 'data',
        name: 'Data source',
        category: 'framework',
        description: 'Powerful component for using local and remote data.Fully supports CRUD, Sorting, Paging, Filtering, Grouping, and Aggregates.',
        depends: ['core'],
        features: [
            {
                id: 'data-odata',
                name: 'OData',
                description: 'Support for accessing Open Data Protocol (OData) services.',
                depends: ['data.odata']
            },
            {
                id: 'data-signalr',
                name: 'SignalR',
                description: 'Support for binding to SignalR hubs.',
                depends: ['data.signalr']
            },
            {
                id: 'data-XML',
                name: 'XML',
                description: 'Support for binding to XML.',
                depends: ['data.xml']
            }
        ]
    };
    (function ($, undefined) {
        var extend = $.extend, proxy = $.proxy, isPlainObject = $.isPlainObject, isEmptyObject = $.isEmptyObject, isArray = $.isArray, grep = $.grep, ajax = $.ajax, map, each = $.each, noop = $.noop, kendo = window.kendo, isFunction = kendo.isFunction, Observable = kendo.Observable, Class = kendo.Class, STRING = 'string', FUNCTION = 'function', CREATE = 'create', READ = 'read', UPDATE = 'update', DESTROY = 'destroy', CHANGE = 'change', SYNC = 'sync', GET = 'get', ERROR = 'error', REQUESTSTART = 'requestStart', PROGRESS = 'progress', REQUESTEND = 'requestEnd', crud = [
                CREATE,
                READ,
                UPDATE,
                DESTROY
            ], identity = function (o) {
                return o;
            }, getter = kendo.getter, stringify = kendo.stringify, math = Math, push = [].push, join = [].join, pop = [].pop, splice = [].splice, shift = [].shift, slice = [].slice, unshift = [].unshift, toString = {}.toString, stableSort = kendo.support.stableSort, dateRegExp = /^\/Date\((.*?)\)\/$/;
        var ObservableArray = Observable.extend({
            init: function (array, type) {
                var that = this;
                that.type = type || ObservableObject;
                Observable.fn.init.call(that);
                that.length = array.length;
                that.wrapAll(array, that);
            },
            at: function (index) {
                return this[index];
            },
            toJSON: function () {
                var idx, length = this.length, value, json = new Array(length);
                for (idx = 0; idx < length; idx++) {
                    value = this[idx];
                    if (value instanceof ObservableObject) {
                        value = value.toJSON();
                    }
                    json[idx] = value;
                }
                return json;
            },
            parent: noop,
            wrapAll: function (source, target) {
                var that = this, idx, length, parent = function () {
                        return that;
                    };
                target = target || [];
                for (idx = 0, length = source.length; idx < length; idx++) {
                    target[idx] = that.wrap(source[idx], parent);
                }
                return target;
            },
            wrap: function (object, parent) {
                var that = this, observable;
                if (object !== null && toString.call(object) === '[object Object]') {
                    observable = object instanceof that.type || object instanceof Model;
                    if (!observable) {
                        object = object instanceof ObservableObject ? object.toJSON() : object;
                        object = new that.type(object);
                    }
                    object.parent = parent;
                    object.bind(CHANGE, function (e) {
                        that.trigger(CHANGE, {
                            field: e.field,
                            node: e.node,
                            index: e.index,
                            items: e.items || [this],
                            action: e.node ? e.action || 'itemloaded' : 'itemchange'
                        });
                    });
                }
                return object;
            },
            push: function () {
                var index = this.length, items = this.wrapAll(arguments), result;
                result = push.apply(this, items);
                this.trigger(CHANGE, {
                    action: 'add',
                    index: index,
                    items: items
                });
                return result;
            },
            slice: slice,
            sort: [].sort,
            join: join,
            pop: function () {
                var length = this.length, result = pop.apply(this);
                if (length) {
                    this.trigger(CHANGE, {
                        action: 'remove',
                        index: length - 1,
                        items: [result]
                    });
                }
                return result;
            },
            splice: function (index, howMany, item) {
                var items = this.wrapAll(slice.call(arguments, 2)), result, i, len;
                result = splice.apply(this, [
                    index,
                    howMany
                ].concat(items));
                if (result.length) {
                    this.trigger(CHANGE, {
                        action: 'remove',
                        index: index,
                        items: result
                    });
                    for (i = 0, len = result.length; i < len; i++) {
                        if (result[i] && result[i].children) {
                            result[i].unbind(CHANGE);
                        }
                    }
                }
                if (item) {
                    this.trigger(CHANGE, {
                        action: 'add',
                        index: index,
                        items: items
                    });
                }
                return result;
            },
            shift: function () {
                var length = this.length, result = shift.apply(this);
                if (length) {
                    this.trigger(CHANGE, {
                        action: 'remove',
                        index: 0,
                        items: [result]
                    });
                }
                return result;
            },
            unshift: function () {
                var items = this.wrapAll(arguments), result;
                result = unshift.apply(this, items);
                this.trigger(CHANGE, {
                    action: 'add',
                    index: 0,
                    items: items
                });
                return result;
            },
            indexOf: function (item) {
                var that = this, idx, length;
                for (idx = 0, length = that.length; idx < length; idx++) {
                    if (that[idx] === item) {
                        return idx;
                    }
                }
                return -1;
            },
            forEach: function (callback, thisArg) {
                var idx = 0;
                var length = this.length;
                var context = thisArg || window;
                for (; idx < length; idx++) {
                    callback.call(context, this[idx], idx, this);
                }
            },
            map: function (callback, thisArg) {
                var idx = 0;
                var result = [];
                var length = this.length;
                var context = thisArg || window;
                for (; idx < length; idx++) {
                    result[idx] = callback.call(context, this[idx], idx, this);
                }
                return result;
            },
            reduce: function (callback) {
                var idx = 0, result, length = this.length;
                if (arguments.length == 2) {
                    result = arguments[1];
                } else if (idx < length) {
                    result = this[idx++];
                }
                for (; idx < length; idx++) {
                    result = callback(result, this[idx], idx, this);
                }
                return result;
            },
            reduceRight: function (callback) {
                var idx = this.length - 1, result;
                if (arguments.length == 2) {
                    result = arguments[1];
                } else if (idx > 0) {
                    result = this[idx--];
                }
                for (; idx >= 0; idx--) {
                    result = callback(result, this[idx], idx, this);
                }
                return result;
            },
            filter: function (callback, thisArg) {
                var idx = 0;
                var result = [];
                var item;
                var length = this.length;
                var context = thisArg || window;
                for (; idx < length; idx++) {
                    item = this[idx];
                    if (callback.call(context, item, idx, this)) {
                        result[result.length] = item;
                    }
                }
                return result;
            },
            find: function (callback, thisArg) {
                var idx = 0;
                var item;
                var length = this.length;
                var context = thisArg || window;
                for (; idx < length; idx++) {
                    item = this[idx];
                    if (callback.call(context, item, idx, this)) {
                        return item;
                    }
                }
            },
            every: function (callback, thisArg) {
                var idx = 0;
                var item;
                var length = this.length;
                var context = thisArg || window;
                for (; idx < length; idx++) {
                    item = this[idx];
                    if (!callback.call(context, item, idx, this)) {
                        return false;
                    }
                }
                return true;
            },
            some: function (callback, thisArg) {
                var idx = 0;
                var item;
                var length = this.length;
                var context = thisArg || window;
                for (; idx < length; idx++) {
                    item = this[idx];
                    if (callback.call(context, item, idx, this)) {
                        return true;
                    }
                }
                return false;
            },
            remove: function (item) {
                var idx = this.indexOf(item);
                if (idx !== -1) {
                    this.splice(idx, 1);
                }
            },
            empty: function () {
                this.splice(0, this.length);
            }
        });
        if (typeof Symbol !== 'undefined' && Symbol.iterator && !ObservableArray.prototype[Symbol.iterator]) {
            ObservableArray.prototype[Symbol.iterator] = [][Symbol.iterator];
        }
        var LazyObservableArray = ObservableArray.extend({
            init: function (data, type) {
                Observable.fn.init.call(this);
                this.type = type || ObservableObject;
                for (var idx = 0; idx < data.length; idx++) {
                    this[idx] = data[idx];
                }
                this.length = idx;
                this._parent = proxy(function () {
                    return this;
                }, this);
            },
            at: function (index) {
                var item = this[index];
                if (!(item instanceof this.type)) {
                    item = this[index] = this.wrap(item, this._parent);
                } else {
                    item.parent = this._parent;
                }
                return item;
            }
        });
        function eventHandler(context, type, field, prefix) {
            return function (e) {
                var event = {}, key;
                for (key in e) {
                    event[key] = e[key];
                }
                if (prefix) {
                    event.field = field + '.' + e.field;
                } else {
                    event.field = field;
                }
                if (type == CHANGE && context._notifyChange) {
                    context._notifyChange(event);
                }
                context.trigger(type, event);
            };
        }
        var ObservableObject = Observable.extend({
            init: function (value) {
                var that = this, member, field, parent = function () {
                        return that;
                    };
                Observable.fn.init.call(this);
                this._handlers = {};
                for (field in value) {
                    member = value[field];
                    if (typeof member === 'object' && member && !member.getTime && field.charAt(0) != '_') {
                        member = that.wrap(member, field, parent);
                    }
                    that[field] = member;
                }
                that.uid = kendo.guid();
            },
            shouldSerialize: function (field) {
                return this.hasOwnProperty(field) && field !== '_handlers' && field !== '_events' && typeof this[field] !== FUNCTION && field !== 'uid';
            },
            forEach: function (f) {
                for (var i in this) {
                    if (this.shouldSerialize(i)) {
                        f(this[i], i);
                    }
                }
            },
            toJSON: function () {
                var result = {}, value, field;
                for (field in this) {
                    if (this.shouldSerialize(field)) {
                        value = this[field];
                        if (value instanceof ObservableObject || value instanceof ObservableArray) {
                            value = value.toJSON();
                        }
                        result[field] = value;
                    }
                }
                return result;
            },
            get: function (field) {
                var that = this, result;
                that.trigger(GET, { field: field });
                if (field === 'this') {
                    result = that;
                } else {
                    result = kendo.getter(field, true)(that);
                }
                return result;
            },
            _set: function (field, value) {
                var that = this;
                var composite = field.indexOf('.') >= 0;
                if (composite) {
                    var paths = field.split('.'), path = '';
                    while (paths.length > 1) {
                        path += paths.shift();
                        var obj = kendo.getter(path, true)(that);
                        if (obj instanceof ObservableObject) {
                            obj.set(paths.join('.'), value);
                            return composite;
                        }
                        path += '.';
                    }
                }
                kendo.setter(field)(that, value);
                return composite;
            },
            set: function (field, value) {
                var that = this, isSetPrevented = false, composite = field.indexOf('.') >= 0, current = kendo.getter(field, true)(that);
                if (current !== value) {
                    if (current instanceof Observable && this._handlers[field]) {
                        if (this._handlers[field].get) {
                            current.unbind(GET, this._handlers[field].get);
                        }
                        current.unbind(CHANGE, this._handlers[field].change);
                    }
                    isSetPrevented = that.trigger('set', {
                        field: field,
                        value: value
                    });
                    if (!isSetPrevented) {
                        if (!composite) {
                            value = that.wrap(value, field, function () {
                                return that;
                            });
                        }
                        if (!that._set(field, value) || field.indexOf('(') >= 0 || field.indexOf('[') >= 0) {
                            that.trigger(CHANGE, { field: field });
                        }
                    }
                }
                return isSetPrevented;
            },
            parent: noop,
            wrap: function (object, field, parent) {
                var that = this;
                var get;
                var change;
                var type = toString.call(object);
                if (object != null && (type === '[object Object]' || type === '[object Array]')) {
                    var isObservableArray = object instanceof ObservableArray;
                    var isDataSource = object instanceof DataSource;
                    if (type === '[object Object]' && !isDataSource && !isObservableArray) {
                        if (!(object instanceof ObservableObject)) {
                            object = new ObservableObject(object);
                        }
                        get = eventHandler(that, GET, field, true);
                        object.bind(GET, get);
                        change = eventHandler(that, CHANGE, field, true);
                        object.bind(CHANGE, change);
                        that._handlers[field] = {
                            get: get,
                            change: change
                        };
                    } else if (type === '[object Array]' || isObservableArray || isDataSource) {
                        if (!isObservableArray && !isDataSource) {
                            object = new ObservableArray(object);
                        }
                        change = eventHandler(that, CHANGE, field, false);
                        object.bind(CHANGE, change);
                        that._handlers[field] = { change: change };
                    }
                    object.parent = parent;
                }
                return object;
            }
        });
        function equal(x, y) {
            if (x === y) {
                return true;
            }
            var xtype = $.type(x), ytype = $.type(y), field;
            if (xtype !== ytype) {
                return false;
            }
            if (xtype === 'date') {
                return x.getTime() === y.getTime();
            }
            if (xtype !== 'object' && xtype !== 'array') {
                return false;
            }
            for (field in x) {
                if (!equal(x[field], y[field])) {
                    return false;
                }
            }
            return true;
        }
        var parsers = {
            'number': function (value) {
                if (typeof value === STRING && value.toLowerCase() === 'null') {
                    return null;
                }
                return kendo.parseFloat(value);
            },
            'date': function (value) {
                if (typeof value === STRING && value.toLowerCase() === 'null') {
                    return null;
                }
                return kendo.parseDate(value);
            },
            'boolean': function (value) {
                if (typeof value === STRING) {
                    if (value.toLowerCase() === 'null') {
                        return null;
                    } else {
                        return value.toLowerCase() === 'true';
                    }
                }
                return value != null ? !!value : value;
            },
            'string': function (value) {
                if (typeof value === STRING && value.toLowerCase() === 'null') {
                    return null;
                }
                return value != null ? value + '' : value;
            },
            'default': function (value) {
                return value;
            }
        };
        var defaultValues = {
            'string': '',
            'number': 0,
            'date': new Date(),
            'boolean': false,
            'default': ''
        };
        function getFieldByName(obj, name) {
            var field, fieldName;
            for (fieldName in obj) {
                field = obj[fieldName];
                if (isPlainObject(field) && field.field && field.field === name) {
                    return field;
                } else if (field === name) {
                    return field;
                }
            }
            return null;
        }
        var Model = ObservableObject.extend({
            init: function (data) {
                var that = this;
                if (!data || $.isEmptyObject(data)) {
                    data = $.extend({}, that.defaults, data);
                    if (that._initializers) {
                        for (var idx = 0; idx < that._initializers.length; idx++) {
                            var name = that._initializers[idx];
                            data[name] = that.defaults[name]();
                        }
                    }
                }
                ObservableObject.fn.init.call(that, data);
                that.dirty = false;
                that.dirtyFields = {};
                if (that.idField) {
                    that.id = that.get(that.idField);
                    if (that.id === undefined) {
                        that.id = that._defaultId;
                    }
                }
            },
            shouldSerialize: function (field) {
                return ObservableObject.fn.shouldSerialize.call(this, field) && field !== 'uid' && !(this.idField !== 'id' && field === 'id') && field !== 'dirty' && field !== 'dirtyFields' && field !== '_accessors';
            },
            _parse: function (field, value) {
                var that = this, fieldName = field, fields = that.fields || {}, parse;
                field = fields[field];
                if (!field) {
                    field = getFieldByName(fields, fieldName);
                }
                if (field) {
                    parse = field.parse;
                    if (!parse && field.type) {
                        parse = parsers[field.type.toLowerCase()];
                    }
                }
                return parse ? parse(value) : value;
            },
            _notifyChange: function (e) {
                var action = e.action;
                if (action == 'add' || action == 'remove') {
                    this.dirty = true;
                    this.dirtyFields[e.field] = true;
                }
            },
            editable: function (field) {
                field = (this.fields || {})[field];
                return field ? field.editable !== false : true;
            },
            set: function (field, value, initiator) {
                var that = this;
                var dirty = that.dirty;
                if (that.editable(field)) {
                    value = that._parse(field, value);
                    if (!equal(value, that.get(field))) {
                        that.dirty = true;
                        that.dirtyFields[field] = true;
                        if (ObservableObject.fn.set.call(that, field, value, initiator) && !dirty) {
                            that.dirty = dirty;
                            if (!that.dirty) {
                                that.dirtyFields[field] = false;
                            }
                        }
                    } else {
                        that.trigger('equalSet', {
                            field: field,
                            value: value
                        });
                    }
                }
            },
            accept: function (data) {
                var that = this, parent = function () {
                        return that;
                    }, field;
                for (field in data) {
                    var value = data[field];
                    if (field.charAt(0) != '_') {
                        value = that.wrap(data[field], field, parent);
                    }
                    that._set(field, value);
                }
                if (that.idField) {
                    that.id = that.get(that.idField);
                }
                that.dirty = false;
                that.dirtyFields = {};
            },
            isNew: function () {
                return this.id === this._defaultId;
            }
        });
        Model.define = function (base, options) {
            if (options === undefined) {
                options = base;
                base = Model;
            }
            var model, proto = extend({ defaults: {} }, options), name, field, type, value, idx, length, fields = {}, originalName, id = proto.id, functionFields = [];
            if (id) {
                proto.idField = id;
            }
            if (proto.id) {
                delete proto.id;
            }
            if (id) {
                proto.defaults[id] = proto._defaultId = '';
            }
            if (toString.call(proto.fields) === '[object Array]') {
                for (idx = 0, length = proto.fields.length; idx < length; idx++) {
                    field = proto.fields[idx];
                    if (typeof field === STRING) {
                        fields[field] = {};
                    } else if (field.field) {
                        fields[field.field] = field;
                    }
                }
                proto.fields = fields;
            }
            for (name in proto.fields) {
                field = proto.fields[name];
                type = field.type || 'default';
                value = null;
                originalName = name;
                name = typeof field.field === STRING ? field.field : name;
                if (!field.nullable) {
                    value = proto.defaults[originalName !== name ? originalName : name] = field.defaultValue !== undefined ? field.defaultValue : defaultValues[type.toLowerCase()];
                    if (typeof value === 'function') {
                        functionFields.push(name);
                    }
                }
                if (options.id === name) {
                    proto._defaultId = value;
                }
                proto.defaults[originalName !== name ? originalName : name] = value;
                field.parse = field.parse || parsers[type];
            }
            if (functionFields.length > 0) {
                proto._initializers = functionFields;
            }
            model = base.extend(proto);
            model.define = function (options) {
                return Model.define(model, options);
            };
            if (proto.fields) {
                model.fields = proto.fields;
                model.idField = proto.idField;
            }
            return model;
        };
        var Comparer = {
            selector: function (field) {
                return isFunction(field) ? field : getter(field);
            },
            compare: function (field) {
                var selector = this.selector(field);
                return function (a, b) {
                    a = selector(a);
                    b = selector(b);
                    if (a == null && b == null) {
                        return 0;
                    }
                    if (a == null) {
                        return -1;
                    }
                    if (b == null) {
                        return 1;
                    }
                    if (a.localeCompare) {
                        return a.localeCompare(b);
                    }
                    return a > b ? 1 : a < b ? -1 : 0;
                };
            },
            create: function (sort) {
                var compare = sort.compare || this.compare(sort.field);
                if (sort.dir == 'desc') {
                    return function (a, b) {
                        return compare(b, a, true);
                    };
                }
                return compare;
            },
            combine: function (comparers) {
                return function (a, b) {
                    var result = comparers[0](a, b), idx, length;
                    for (idx = 1, length = comparers.length; idx < length; idx++) {
                        result = result || comparers[idx](a, b);
                    }
                    return result;
                };
            }
        };
        var StableComparer = extend({}, Comparer, {
            asc: function (field) {
                var selector = this.selector(field);
                return function (a, b) {
                    var valueA = selector(a);
                    var valueB = selector(b);
                    if (valueA && valueA.getTime && valueB && valueB.getTime) {
                        valueA = valueA.getTime();
                        valueB = valueB.getTime();
                    }
                    if (valueA === valueB) {
                        return a.__position - b.__position;
                    }
                    if (valueA == null) {
                        return -1;
                    }
                    if (valueB == null) {
                        return 1;
                    }
                    if (valueA.localeCompare) {
                        return valueA.localeCompare(valueB);
                    }
                    return valueA > valueB ? 1 : -1;
                };
            },
            desc: function (field) {
                var selector = this.selector(field);
                return function (a, b) {
                    var valueA = selector(a);
                    var valueB = selector(b);
                    if (valueA && valueA.getTime && valueB && valueB.getTime) {
                        valueA = valueA.getTime();
                        valueB = valueB.getTime();
                    }
                    if (valueA === valueB) {
                        return a.__position - b.__position;
                    }
                    if (valueA == null) {
                        return 1;
                    }
                    if (valueB == null) {
                        return -1;
                    }
                    if (valueB.localeCompare) {
                        return valueB.localeCompare(valueA);
                    }
                    return valueA < valueB ? 1 : -1;
                };
            },
            create: function (sort) {
                return this[sort.dir](sort.field);
            }
        });
        map = function (array, callback) {
            var idx, length = array.length, result = new Array(length);
            for (idx = 0; idx < length; idx++) {
                result[idx] = callback(array[idx], idx, array);
            }
            return result;
        };
        var operators = function () {
            function quote(str) {
                if (typeof str == 'string') {
                    str = str.replace(/[\r\n]+/g, '');
                }
                return JSON.stringify(str);
            }
            function textOp(impl) {
                return function (a, b, ignore) {
                    b += '';
                    if (ignore) {
                        a = '(' + a + ' || \'\').toLowerCase()';
                        b = b.toLowerCase();
                    }
                    return impl(a, quote(b), ignore);
                };
            }
            function operator(op, a, b, ignore) {
                if (b != null) {
                    if (typeof b === STRING) {
                        var date = dateRegExp.exec(b);
                        if (date) {
                            b = new Date(+date[1]);
                        } else if (ignore) {
                            b = quote(b.toLowerCase());
                            a = '((' + a + ' || \'\')+\'\').toLowerCase()';
                        } else {
                            b = quote(b);
                        }
                    }
                    if (b.getTime) {
                        a = '(' + a + '&&' + a + '.getTime?' + a + '.getTime():' + a + ')';
                        b = b.getTime();
                    }
                }
                return a + ' ' + op + ' ' + b;
            }
            function getMatchRegexp(pattern) {
                for (var rx = '/^', esc = false, i = 0; i < pattern.length; ++i) {
                    var ch = pattern.charAt(i);
                    if (esc) {
                        rx += '\\' + ch;
                    } else if (ch == '~') {
                        esc = true;
                        continue;
                    } else if (ch == '*') {
                        rx += '.*';
                    } else if (ch == '?') {
                        rx += '.';
                    } else if ('.+^$()[]{}|\\/\n\r\u2028\u2029\xA0'.indexOf(ch) >= 0) {
                        rx += '\\' + ch;
                    } else {
                        rx += ch;
                    }
                    esc = false;
                }
                return rx + '$/';
            }
            return {
                quote: function (value) {
                    if (value && value.getTime) {
                        return 'new Date(' + value.getTime() + ')';
                    }
                    return quote(value);
                },
                eq: function (a, b, ignore) {
                    return operator('==', a, b, ignore);
                },
                neq: function (a, b, ignore) {
                    return operator('!=', a, b, ignore);
                },
                gt: function (a, b, ignore) {
                    return operator('>', a, b, ignore);
                },
                gte: function (a, b, ignore) {
                    return operator('>=', a, b, ignore);
                },
                lt: function (a, b, ignore) {
                    return operator('<', a, b, ignore);
                },
                lte: function (a, b, ignore) {
                    return operator('<=', a, b, ignore);
                },
                startswith: textOp(function (a, b) {
                    return a + '.lastIndexOf(' + b + ', 0) == 0';
                }),
                doesnotstartwith: textOp(function (a, b) {
                    return a + '.lastIndexOf(' + b + ', 0) == -1';
                }),
                endswith: textOp(function (a, b) {
                    var n = b ? b.length - 2 : 0;
                    return a + '.indexOf(' + b + ', ' + a + '.length - ' + n + ') >= 0';
                }),
                doesnotendwith: textOp(function (a, b) {
                    var n = b ? b.length - 2 : 0;
                    return a + '.indexOf(' + b + ', ' + a + '.length - ' + n + ') < 0';
                }),
                contains: textOp(function (a, b) {
                    return a + '.indexOf(' + b + ') >= 0';
                }),
                doesnotcontain: textOp(function (a, b) {
                    return a + '.indexOf(' + b + ') == -1';
                }),
                matches: textOp(function (a, b) {
                    b = b.substring(1, b.length - 1);
                    return getMatchRegexp(b) + '.test(' + a + ')';
                }),
                doesnotmatch: textOp(function (a, b) {
                    b = b.substring(1, b.length - 1);
                    return '!' + getMatchRegexp(b) + '.test(' + a + ')';
                }),
                isempty: function (a) {
                    return a + ' === \'\'';
                },
                isnotempty: function (a) {
                    return a + ' !== \'\'';
                },
                isnull: function (a) {
                    return '(' + a + ' == null)';
                },
                isnotnull: function (a) {
                    return '(' + a + ' != null)';
                },
                isnullorempty: function (a) {
                    return '(' + a + ' === null) || (' + a + ' === \'\')';
                },
                isnotnullorempty: function (a) {
                    return '(' + a + ' !== null) && (' + a + ' !== \'\')';
                }
            };
        }();
        function Query(data) {
            this.data = data || [];
        }
        Query.filterExpr = function (expression) {
            var expressions = [], logic = {
                    and: ' && ',
                    or: ' || '
                }, idx, length, filter, expr, fieldFunctions = [], operatorFunctions = [], field, operator, filters = expression.filters;
            for (idx = 0, length = filters.length; idx < length; idx++) {
                filter = filters[idx];
                field = filter.field;
                operator = filter.operator;
                if (filter.filters) {
                    expr = Query.filterExpr(filter);
                    filter = expr.expression.replace(/__o\[(\d+)\]/g, function (match, index) {
                        index = +index;
                        return '__o[' + (operatorFunctions.length + index) + ']';
                    }).replace(/__f\[(\d+)\]/g, function (match, index) {
                        index = +index;
                        return '__f[' + (fieldFunctions.length + index) + ']';
                    });
                    operatorFunctions.push.apply(operatorFunctions, expr.operators);
                    fieldFunctions.push.apply(fieldFunctions, expr.fields);
                } else {
                    if (typeof field === FUNCTION) {
                        expr = '__f[' + fieldFunctions.length + '](d)';
                        fieldFunctions.push(field);
                    } else {
                        expr = kendo.expr(field);
                    }
                    if (typeof operator === FUNCTION) {
                        filter = '__o[' + operatorFunctions.length + '](' + expr + ', ' + operators.quote(filter.value) + ')';
                        operatorFunctions.push(operator);
                    } else {
                        filter = operators[(operator || 'eq').toLowerCase()](expr, filter.value, filter.ignoreCase !== undefined ? filter.ignoreCase : true);
                    }
                }
                expressions.push(filter);
            }
            return {
                expression: '(' + expressions.join(logic[expression.logic]) + ')',
                fields: fieldFunctions,
                operators: operatorFunctions
            };
        };
        function normalizeSort(field, dir) {
            if (field) {
                var descriptor = typeof field === STRING ? {
                        field: field,
                        dir: dir
                    } : field, descriptors = isArray(descriptor) ? descriptor : descriptor !== undefined ? [descriptor] : [];
                return grep(descriptors, function (d) {
                    return !!d.dir;
                });
            }
        }
        var operatorMap = {
            '==': 'eq',
            equals: 'eq',
            isequalto: 'eq',
            equalto: 'eq',
            equal: 'eq',
            '!=': 'neq',
            ne: 'neq',
            notequals: 'neq',
            isnotequalto: 'neq',
            notequalto: 'neq',
            notequal: 'neq',
            '<': 'lt',
            islessthan: 'lt',
            lessthan: 'lt',
            less: 'lt',
            '<=': 'lte',
            le: 'lte',
            islessthanorequalto: 'lte',
            lessthanequal: 'lte',
            '>': 'gt',
            isgreaterthan: 'gt',
            greaterthan: 'gt',
            greater: 'gt',
            '>=': 'gte',
            isgreaterthanorequalto: 'gte',
            greaterthanequal: 'gte',
            ge: 'gte',
            notsubstringof: 'doesnotcontain',
            isnull: 'isnull',
            isempty: 'isempty',
            isnotempty: 'isnotempty'
        };
        function normalizeOperator(expression) {
            var idx, length, filter, operator, filters = expression.filters;
            if (filters) {
                for (idx = 0, length = filters.length; idx < length; idx++) {
                    filter = filters[idx];
                    operator = filter.operator;
                    if (operator && typeof operator === STRING) {
                        filter.operator = operatorMap[operator.toLowerCase()] || operator;
                    }
                    normalizeOperator(filter);
                }
            }
        }
        function normalizeFilter(expression) {
            if (expression && !isEmptyObject(expression)) {
                if (isArray(expression) || !expression.filters) {
                    expression = {
                        logic: 'and',
                        filters: isArray(expression) ? expression : [expression]
                    };
                }
                normalizeOperator(expression);
                return expression;
            }
        }
        Query.normalizeFilter = normalizeFilter;
        function compareDescriptor(f1, f2) {
            if (f1.logic || f2.logic) {
                return false;
            }
            return f1.field === f2.field && f1.value === f2.value && f1.operator === f2.operator;
        }
        function normalizeDescriptor(filter) {
            filter = filter || {};
            if (isEmptyObject(filter)) {
                return {
                    logic: 'and',
                    filters: []
                };
            }
            return normalizeFilter(filter);
        }
        function fieldComparer(a, b) {
            if (b.logic || a.field > b.field) {
                return 1;
            } else if (a.field < b.field) {
                return -1;
            } else {
                return 0;
            }
        }
        function compareFilters(expr1, expr2) {
            expr1 = normalizeDescriptor(expr1);
            expr2 = normalizeDescriptor(expr2);
            if (expr1.logic !== expr2.logic) {
                return false;
            }
            var f1, f2;
            var filters1 = (expr1.filters || []).slice();
            var filters2 = (expr2.filters || []).slice();
            if (filters1.length !== filters2.length) {
                return false;
            }
            filters1 = filters1.sort(fieldComparer);
            filters2 = filters2.sort(fieldComparer);
            for (var idx = 0; idx < filters1.length; idx++) {
                f1 = filters1[idx];
                f2 = filters2[idx];
                if (f1.logic && f2.logic) {
                    if (!compareFilters(f1, f2)) {
                        return false;
                    }
                } else if (!compareDescriptor(f1, f2)) {
                    return false;
                }
            }
            return true;
        }
        Query.compareFilters = compareFilters;
        function normalizeAggregate(expressions) {
            return isArray(expressions) ? expressions : [expressions];
        }
        function normalizeGroup(field, dir) {
            var descriptor = typeof field === STRING ? {
                    field: field,
                    dir: dir
                } : field, descriptors = isArray(descriptor) ? descriptor : descriptor !== undefined ? [descriptor] : [];
            return map(descriptors, function (d) {
                return {
                    field: d.field,
                    dir: d.dir || 'asc',
                    aggregates: d.aggregates
                };
            });
        }
        Query.prototype = {
            toArray: function () {
                return this.data;
            },
            range: function (index, count) {
                return new Query(this.data.slice(index, index + count));
            },
            skip: function (count) {
                return new Query(this.data.slice(count));
            },
            take: function (count) {
                return new Query(this.data.slice(0, count));
            },
            select: function (selector) {
                return new Query(map(this.data, selector));
            },
            order: function (selector, dir, inPlace) {
                var sort = { dir: dir };
                if (selector) {
                    if (selector.compare) {
                        sort.compare = selector.compare;
                    } else {
                        sort.field = selector;
                    }
                }
                if (inPlace) {
                    return new Query(this.data.sort(Comparer.create(sort)));
                }
                return new Query(this.data.slice(0).sort(Comparer.create(sort)));
            },
            orderBy: function (selector, inPlace) {
                return this.order(selector, 'asc', inPlace);
            },
            orderByDescending: function (selector, inPlace) {
                return this.order(selector, 'desc', inPlace);
            },
            sort: function (field, dir, comparer, inPlace) {
                var idx, length, descriptors = normalizeSort(field, dir), comparers = [];
                comparer = comparer || Comparer;
                if (descriptors.length) {
                    for (idx = 0, length = descriptors.length; idx < length; idx++) {
                        comparers.push(comparer.create(descriptors[idx]));
                    }
                    return this.orderBy({ compare: comparer.combine(comparers) }, inPlace);
                }
                return this;
            },
            filter: function (expressions) {
                var idx, current, length, compiled, predicate, data = this.data, fields, operators, result = [], filter;
                expressions = normalizeFilter(expressions);
                if (!expressions || expressions.filters.length === 0) {
                    return this;
                }
                compiled = Query.filterExpr(expressions);
                fields = compiled.fields;
                operators = compiled.operators;
                predicate = filter = new Function('d, __f, __o', 'return ' + compiled.expression);
                if (fields.length || operators.length) {
                    filter = function (d) {
                        return predicate(d, fields, operators);
                    };
                }
                for (idx = 0, length = data.length; idx < length; idx++) {
                    current = data[idx];
                    if (filter(current)) {
                        result.push(current);
                    }
                }
                return new Query(result);
            },
            group: function (descriptors, allData) {
                descriptors = normalizeGroup(descriptors || []);
                allData = allData || this.data;
                var that = this, result = new Query(that.data), descriptor;
                if (descriptors.length > 0) {
                    descriptor = descriptors[0];
                    result = result.groupBy(descriptor).select(function (group) {
                        var data = new Query(allData).filter([{
                                field: group.field,
                                operator: 'eq',
                                value: group.value,
                                ignoreCase: false
                            }]);
                        return {
                            field: group.field,
                            value: group.value,
                            items: descriptors.length > 1 ? new Query(group.items).group(descriptors.slice(1), data.toArray()).toArray() : group.items,
                            hasSubgroups: descriptors.length > 1,
                            aggregates: data.aggregate(descriptor.aggregates)
                        };
                    });
                }
                return result;
            },
            groupBy: function (descriptor) {
                if (isEmptyObject(descriptor) || !this.data.length) {
                    return new Query([]);
                }
                var field = descriptor.field, sorted = this._sortForGrouping(field, descriptor.dir || 'asc'), accessor = kendo.accessor(field), item, groupValue = accessor.get(sorted[0], field), group = {
                        field: field,
                        value: groupValue,
                        items: []
                    }, currentValue, idx, len, result = [group];
                for (idx = 0, len = sorted.length; idx < len; idx++) {
                    item = sorted[idx];
                    currentValue = accessor.get(item, field);
                    if (!groupValueComparer(groupValue, currentValue)) {
                        groupValue = currentValue;
                        group = {
                            field: field,
                            value: groupValue,
                            items: []
                        };
                        result.push(group);
                    }
                    group.items.push(item);
                }
                return new Query(result);
            },
            _sortForGrouping: function (field, dir) {
                var idx, length, data = this.data;
                if (!stableSort) {
                    for (idx = 0, length = data.length; idx < length; idx++) {
                        data[idx].__position = idx;
                    }
                    data = new Query(data).sort(field, dir, StableComparer).toArray();
                    for (idx = 0, length = data.length; idx < length; idx++) {
                        delete data[idx].__position;
                    }
                    return data;
                }
                return this.sort(field, dir).toArray();
            },
            aggregate: function (aggregates) {
                var idx, len, result = {}, state = {};
                if (aggregates && aggregates.length) {
                    for (idx = 0, len = this.data.length; idx < len; idx++) {
                        calculateAggregate(result, aggregates, this.data[idx], idx, len, state);
                    }
                }
                return result;
            }
        };
        function groupValueComparer(a, b) {
            if (a && a.getTime && b && b.getTime) {
                return a.getTime() === b.getTime();
            }
            return a === b;
        }
        function calculateAggregate(accumulator, aggregates, item, index, length, state) {
            aggregates = aggregates || [];
            var idx, aggr, functionName, len = aggregates.length;
            for (idx = 0; idx < len; idx++) {
                aggr = aggregates[idx];
                functionName = aggr.aggregate;
                var field = aggr.field;
                accumulator[field] = accumulator[field] || {};
                state[field] = state[field] || {};
                state[field][functionName] = state[field][functionName] || {};
                accumulator[field][functionName] = functions[functionName.toLowerCase()](accumulator[field][functionName], item, kendo.accessor(field), index, length, state[field][functionName]);
            }
        }
        var functions = {
            sum: function (accumulator, item, accessor) {
                var value = accessor.get(item);
                if (!isNumber(accumulator)) {
                    accumulator = value;
                } else if (isNumber(value)) {
                    accumulator += value;
                }
                return accumulator;
            },
            count: function (accumulator) {
                return (accumulator || 0) + 1;
            },
            average: function (accumulator, item, accessor, index, length, state) {
                var value = accessor.get(item);
                if (state.count === undefined) {
                    state.count = 0;
                }
                if (!isNumber(accumulator)) {
                    accumulator = value;
                } else if (isNumber(value)) {
                    accumulator += value;
                }
                if (isNumber(value)) {
                    state.count++;
                }
                if (index == length - 1 && isNumber(accumulator)) {
                    accumulator = accumulator / state.count;
                }
                return accumulator;
            },
            max: function (accumulator, item, accessor) {
                var value = accessor.get(item);
                if (!isNumber(accumulator) && !isDate(accumulator)) {
                    accumulator = value;
                }
                if (accumulator < value && (isNumber(value) || isDate(value))) {
                    accumulator = value;
                }
                return accumulator;
            },
            min: function (accumulator, item, accessor) {
                var value = accessor.get(item);
                if (!isNumber(accumulator) && !isDate(accumulator)) {
                    accumulator = value;
                }
                if (accumulator > value && (isNumber(value) || isDate(value))) {
                    accumulator = value;
                }
                return accumulator;
            }
        };
        function isNumber(val) {
            return typeof val === 'number' && !isNaN(val);
        }
        function isDate(val) {
            return val && val.getTime;
        }
        function toJSON(array) {
            var idx, length = array.length, result = new Array(length);
            for (idx = 0; idx < length; idx++) {
                result[idx] = array[idx].toJSON();
            }
            return result;
        }
        Query.process = function (data, options, inPlace) {
            options = options || {};
            var query = new Query(data), group = options.group, sort = normalizeGroup(group || []).concat(normalizeSort(options.sort || [])), total, filterCallback = options.filterCallback, filter = options.filter, skip = options.skip, take = options.take;
            if (sort && inPlace) {
                query = query.sort(sort, undefined, undefined, inPlace);
            }
            if (filter) {
                query = query.filter(filter);
                if (filterCallback) {
                    query = filterCallback(query);
                }
                total = query.toArray().length;
            }
            if (sort && !inPlace) {
                query = query.sort(sort);
                if (group) {
                    data = query.toArray();
                }
            }
            if (skip !== undefined && take !== undefined) {
                query = query.range(skip, take);
            }
            if (group) {
                query = query.group(group, data);
            }
            return {
                total: total,
                data: query.toArray()
            };
        };
        var LocalTransport = Class.extend({
            init: function (options) {
                this.data = options.data;
            },
            read: function (options) {
                options.success(this.data);
            },
            update: function (options) {
                options.success(options.data);
            },
            create: function (options) {
                options.success(options.data);
            },
            destroy: function (options) {
                options.success(options.data);
            }
        });
        var RemoteTransport = Class.extend({
            init: function (options) {
                var that = this, parameterMap;
                options = that.options = extend({}, that.options, options);
                each(crud, function (index, type) {
                    if (typeof options[type] === STRING) {
                        options[type] = { url: options[type] };
                    }
                });
                that.cache = options.cache ? Cache.create(options.cache) : {
                    find: noop,
                    add: noop
                };
                parameterMap = options.parameterMap;
                if (options.submit) {
                    that.submit = options.submit;
                }
                if (isFunction(options.push)) {
                    that.push = options.push;
                }
                if (!that.push) {
                    that.push = identity;
                }
                that.parameterMap = isFunction(parameterMap) ? parameterMap : function (options) {
                    var result = {};
                    each(options, function (option, value) {
                        if (option in parameterMap) {
                            option = parameterMap[option];
                            if (isPlainObject(option)) {
                                value = option.value(value);
                                option = option.key;
                            }
                        }
                        result[option] = value;
                    });
                    return result;
                };
            },
            options: { parameterMap: identity },
            create: function (options) {
                return ajax(this.setup(options, CREATE));
            },
            read: function (options) {
                var that = this, success, error, result, cache = that.cache;
                options = that.setup(options, READ);
                success = options.success || noop;
                error = options.error || noop;
                result = cache.find(options.data);
                if (result !== undefined) {
                    success(result);
                } else {
                    options.success = function (result) {
                        cache.add(options.data, result);
                        success(result);
                    };
                    $.ajax(options);
                }
            },
            update: function (options) {
                return ajax(this.setup(options, UPDATE));
            },
            destroy: function (options) {
                return ajax(this.setup(options, DESTROY));
            },
            setup: function (options, type) {
                options = options || {};
                var that = this, parameters, operation = that.options[type], data = isFunction(operation.data) ? operation.data(options.data) : operation.data;
                options = extend(true, {}, operation, options);
                parameters = extend(true, {}, data, options.data);
                options.data = that.parameterMap(parameters, type);
                if (isFunction(options.url)) {
                    options.url = options.url(parameters);
                }
                return options;
            }
        });
        var Cache = Class.extend({
            init: function () {
                this._store = {};
            },
            add: function (key, data) {
                if (key !== undefined) {
                    this._store[stringify(key)] = data;
                }
            },
            find: function (key) {
                return this._store[stringify(key)];
            },
            clear: function () {
                this._store = {};
            },
            remove: function (key) {
                delete this._store[stringify(key)];
            }
        });
        Cache.create = function (options) {
            var store = {
                'inmemory': function () {
                    return new Cache();
                }
            };
            if (isPlainObject(options) && isFunction(options.find)) {
                return options;
            }
            if (options === true) {
                return new Cache();
            }
            return store[options]();
        };
        function serializeRecords(data, getters, modelInstance, originalFieldNames, fieldNames) {
            var record, getter, originalName, idx, setters = {}, length;
            for (idx = 0, length = data.length; idx < length; idx++) {
                record = data[idx];
                for (getter in getters) {
                    originalName = fieldNames[getter];
                    if (originalName && originalName !== getter) {
                        if (!setters[originalName]) {
                            setters[originalName] = kendo.setter(originalName);
                        }
                        setters[originalName](record, getters[getter](record));
                        delete record[getter];
                    }
                }
            }
        }
        function convertRecords(data, getters, modelInstance, originalFieldNames, fieldNames) {
            var record, getter, originalName, idx, length;
            for (idx = 0, length = data.length; idx < length; idx++) {
                record = data[idx];
                for (getter in getters) {
                    record[getter] = modelInstance._parse(getter, getters[getter](record));
                    originalName = fieldNames[getter];
                    if (originalName && originalName !== getter) {
                        delete record[originalName];
                    }
                }
            }
        }
        function convertGroup(data, getters, modelInstance, originalFieldNames, fieldNames) {
            var record, idx, fieldName, length;
            for (idx = 0, length = data.length; idx < length; idx++) {
                record = data[idx];
                fieldName = originalFieldNames[record.field];
                if (fieldName && fieldName != record.field) {
                    record.field = fieldName;
                }
                record.value = modelInstance._parse(record.field, record.value);
                if (record.hasSubgroups) {
                    convertGroup(record.items, getters, modelInstance, originalFieldNames, fieldNames);
                } else {
                    convertRecords(record.items, getters, modelInstance, originalFieldNames, fieldNames);
                }
            }
        }
        function wrapDataAccess(originalFunction, model, converter, getters, originalFieldNames, fieldNames) {
            return function (data) {
                data = originalFunction(data);
                return wrapDataAccessBase(model, converter, getters, originalFieldNames, fieldNames)(data);
            };
        }
        function wrapDataAccessBase(model, converter, getters, originalFieldNames, fieldNames) {
            return function (data) {
                if (data && !isEmptyObject(getters)) {
                    if (toString.call(data) !== '[object Array]' && !(data instanceof ObservableArray)) {
                        data = [data];
                    }
                    converter(data, getters, new model(), originalFieldNames, fieldNames);
                }
                return data || [];
            };
        }
        var DataReader = Class.extend({
            init: function (schema) {
                var that = this, member, get, model, base;
                schema = schema || {};
                for (member in schema) {
                    get = schema[member];
                    that[member] = typeof get === STRING ? getter(get) : get;
                }
                base = schema.modelBase || Model;
                if (isPlainObject(that.model)) {
                    that.model = model = base.define(that.model);
                }
                var dataFunction = proxy(that.data, that);
                that._dataAccessFunction = dataFunction;
                if (that.model) {
                    var groupsFunction = proxy(that.groups, that), serializeFunction = proxy(that.serialize, that), originalFieldNames = {}, getters = {}, serializeGetters = {}, fieldNames = {}, shouldSerialize = false, fieldName;
                    model = that.model;
                    if (model.fields) {
                        each(model.fields, function (field, value) {
                            var fromName;
                            fieldName = field;
                            if (isPlainObject(value) && value.field) {
                                fieldName = value.field;
                            } else if (typeof value === STRING) {
                                fieldName = value;
                            }
                            if (isPlainObject(value) && value.from) {
                                fromName = value.from;
                            }
                            shouldSerialize = shouldSerialize || fromName && fromName !== field || fieldName !== field;
                            getters[field] = getter(fromName || fieldName);
                            serializeGetters[field] = getter(field);
                            originalFieldNames[fromName || fieldName] = field;
                            fieldNames[field] = fromName || fieldName;
                        });
                        if (!schema.serialize && shouldSerialize) {
                            that.serialize = wrapDataAccess(serializeFunction, model, serializeRecords, serializeGetters, originalFieldNames, fieldNames);
                        }
                    }
                    that._dataAccessFunction = dataFunction;
                    that._wrapDataAccessBase = wrapDataAccessBase(model, convertRecords, getters, originalFieldNames, fieldNames);
                    that.data = wrapDataAccess(dataFunction, model, convertRecords, getters, originalFieldNames, fieldNames);
                    that.groups = wrapDataAccess(groupsFunction, model, convertGroup, getters, originalFieldNames, fieldNames);
                }
            },
            errors: function (data) {
                return data ? data.errors : null;
            },
            parse: identity,
            data: identity,
            total: function (data) {
                return data.length;
            },
            groups: identity,
            aggregates: function () {
                return {};
            },
            serialize: function (data) {
                return data;
            }
        });
        function fillLastGroup(originalGroup, newGroup) {
            var currOriginal;
            var currentNew;
            if (newGroup.items && newGroup.items.length) {
                for (var i = 0; i < newGroup.items.length; i++) {
                    currOriginal = originalGroup.items[i];
                    currentNew = newGroup.items[i];
                    if (currOriginal && currentNew) {
                        if (currOriginal.hasSubgroups) {
                            fillLastGroup(currOriginal, currentNew);
                        } else if (currOriginal.field && currOriginal.value == currentNew.value) {
                            currOriginal.items.push.apply(currOriginal.items, currentNew.items);
                        } else {
                            originalGroup.items.push.apply(originalGroup.items, [currentNew]);
                        }
                    } else if (currentNew) {
                        originalGroup.items.push.apply(originalGroup.items, [currentNew]);
                    }
                }
            }
        }
        function mergeGroups(target, dest, skip, take) {
            var group, idx = 0, items;
            while (dest.length && take) {
                group = dest[idx];
                items = group.items;
                var length = items.length;
                if (target && target.field === group.field && target.value === group.value) {
                    if (target.hasSubgroups && target.items.length) {
                        mergeGroups(target.items[target.items.length - 1], group.items, skip, take);
                    } else {
                        items = items.slice(skip, skip + take);
                        target.items = target.items.concat(items);
                    }
                    dest.splice(idx--, 1);
                } else if (group.hasSubgroups && items.length) {
                    mergeGroups(group, items, skip, take);
                    if (!group.items.length) {
                        dest.splice(idx--, 1);
                    }
                } else {
                    items = items.slice(skip, skip + take);
                    group.items = items;
                    if (!group.items.length) {
                        dest.splice(idx--, 1);
                    }
                }
                if (items.length === 0) {
                    skip -= length;
                } else {
                    skip = 0;
                    take -= items.length;
                }
                if (++idx >= dest.length) {
                    break;
                }
            }
            if (idx < dest.length) {
                dest.splice(idx, dest.length - idx);
            }
        }
        function flattenGroups(data) {
            var idx, result = [], length, items, itemIndex;
            for (idx = 0, length = data.length; idx < length; idx++) {
                var group = data.at(idx);
                if (group.hasSubgroups) {
                    result = result.concat(flattenGroups(group.items));
                } else {
                    items = group.items;
                    for (itemIndex = 0; itemIndex < items.length; itemIndex++) {
                        result.push(items.at(itemIndex));
                    }
                }
            }
            return result;
        }
        function wrapGroupItems(data, model) {
            var idx, length, group;
            if (model) {
                for (idx = 0, length = data.length; idx < length; idx++) {
                    group = data.at(idx);
                    if (group.hasSubgroups) {
                        wrapGroupItems(group.items, model);
                    } else {
                        group.items = new LazyObservableArray(group.items, model);
                    }
                }
            }
        }
        function eachGroupItems(data, func) {
            for (var idx = 0, length = data.length; idx < length; idx++) {
                if (data[idx].hasSubgroups) {
                    if (eachGroupItems(data[idx].items, func)) {
                        return true;
                    }
                } else if (func(data[idx].items, data[idx])) {
                    return true;
                }
            }
        }
        function replaceInRanges(ranges, data, item, observable) {
            for (var idx = 0; idx < ranges.length; idx++) {
                if (ranges[idx].data === data) {
                    break;
                }
                if (replaceInRange(ranges[idx].data, item, observable)) {
                    break;
                }
            }
        }
        function replaceInRange(items, item, observable) {
            for (var idx = 0, length = items.length; idx < length; idx++) {
                if (items[idx] && items[idx].hasSubgroups) {
                    return replaceInRange(items[idx].items, item, observable);
                } else if (items[idx] === item || items[idx] === observable) {
                    items[idx] = observable;
                    return true;
                }
            }
        }
        function replaceWithObservable(view, data, ranges, type, serverGrouping) {
            for (var viewIndex = 0, length = view.length; viewIndex < length; viewIndex++) {
                var item = view[viewIndex];
                if (!item || item instanceof type) {
                    continue;
                }
                if (item.hasSubgroups !== undefined && !serverGrouping) {
                    replaceWithObservable(item.items, data, ranges, type, serverGrouping);
                } else {
                    for (var idx = 0; idx < data.length; idx++) {
                        if (data[idx] === item) {
                            view[viewIndex] = data.at(idx);
                            replaceInRanges(ranges, data, item, view[viewIndex]);
                            break;
                        }
                    }
                }
            }
        }
        function removeModel(data, model) {
            var length = data.length;
            var dataItem;
            var idx;
            for (idx = 0; idx < length; idx++) {
                dataItem = data[idx];
                if (dataItem.uid && dataItem.uid == model.uid) {
                    data.splice(idx, 1);
                    return dataItem;
                }
            }
        }
        function indexOfPristineModel(data, model) {
            if (model) {
                return indexOf(data, function (item) {
                    return item.uid && item.uid == model.uid || item[model.idField] === model.id && model.id !== model._defaultId;
                });
            }
            return -1;
        }
        function indexOfModel(data, model) {
            if (model) {
                return indexOf(data, function (item) {
                    return item.uid == model.uid;
                });
            }
            return -1;
        }
        function indexOf(data, comparer) {
            var idx, length;
            for (idx = 0, length = data.length; idx < length; idx++) {
                if (comparer(data[idx])) {
                    return idx;
                }
            }
            return -1;
        }
        function fieldNameFromModel(fields, name) {
            if (fields && !isEmptyObject(fields)) {
                var descriptor = fields[name];
                var fieldName;
                if (isPlainObject(descriptor)) {
                    fieldName = descriptor.from || descriptor.field || name;
                } else {
                    fieldName = fields[name] || name;
                }
                if (isFunction(fieldName)) {
                    return name;
                }
                return fieldName;
            }
            return name;
        }
        function convertFilterDescriptorsField(descriptor, model) {
            var idx, length, target = {};
            for (var field in descriptor) {
                if (field !== 'filters') {
                    target[field] = descriptor[field];
                }
            }
            if (descriptor.filters) {
                target.filters = [];
                for (idx = 0, length = descriptor.filters.length; idx < length; idx++) {
                    target.filters[idx] = convertFilterDescriptorsField(descriptor.filters[idx], model);
                }
            } else {
                target.field = fieldNameFromModel(model.fields, target.field);
            }
            return target;
        }
        function convertDescriptorsField(descriptors, model) {
            var idx, length, result = [], target, descriptor;
            for (idx = 0, length = descriptors.length; idx < length; idx++) {
                target = {};
                descriptor = descriptors[idx];
                for (var field in descriptor) {
                    target[field] = descriptor[field];
                }
                target.field = fieldNameFromModel(model.fields, target.field);
                if (target.aggregates && isArray(target.aggregates)) {
                    target.aggregates = convertDescriptorsField(target.aggregates, model);
                }
                result.push(target);
            }
            return result;
        }
        var DataSource = Observable.extend({
            init: function (options) {
                var that = this, model, data;
                if (options) {
                    data = options.data;
                }
                options = that.options = extend({}, that.options, options);
                that._map = {};
                that._prefetch = {};
                that._data = [];
                that._pristineData = [];
                that._ranges = [];
                that._view = [];
                that._pristineTotal = 0;
                that._destroyed = [];
                that._pageSize = options.pageSize;
                that._page = options.page || (options.pageSize ? 1 : undefined);
                that._sort = normalizeSort(options.sort);
                that._filter = normalizeFilter(options.filter);
                that._group = normalizeGroup(options.group);
                that._aggregate = options.aggregate;
                that._total = options.total;
                that._shouldDetachObservableParents = true;
                Observable.fn.init.call(that);
                that.transport = Transport.create(options, data, that);
                if (isFunction(that.transport.push)) {
                    that.transport.push({
                        pushCreate: proxy(that._pushCreate, that),
                        pushUpdate: proxy(that._pushUpdate, that),
                        pushDestroy: proxy(that._pushDestroy, that)
                    });
                }
                if (options.offlineStorage != null) {
                    if (typeof options.offlineStorage == 'string') {
                        var key = options.offlineStorage;
                        that._storage = {
                            getItem: function () {
                                return JSON.parse(localStorage.getItem(key));
                            },
                            setItem: function (item) {
                                localStorage.setItem(key, stringify(that.reader.serialize(item)));
                            }
                        };
                    } else {
                        that._storage = options.offlineStorage;
                    }
                }
                that.reader = new kendo.data.readers[options.schema.type || 'json'](options.schema);
                model = that.reader.model || {};
                that._detachObservableParents();
                that._data = that._observe(that._data);
                that._online = true;
                that.bind([
                    'push',
                    ERROR,
                    CHANGE,
                    REQUESTSTART,
                    SYNC,
                    REQUESTEND,
                    PROGRESS
                ], options);
            },
            options: {
                data: null,
                schema: { modelBase: Model },
                offlineStorage: null,
                serverSorting: false,
                serverPaging: false,
                serverFiltering: false,
                serverGrouping: false,
                serverAggregates: false,
                batch: false,
                inPlaceSort: false
            },
            clone: function () {
                return this;
            },
            online: function (value) {
                if (value !== undefined) {
                    if (this._online != value) {
                        this._online = value;
                        if (value) {
                            return this.sync();
                        }
                    }
                    return $.Deferred().resolve().promise();
                } else {
                    return this._online;
                }
            },
            offlineData: function (state) {
                if (this.options.offlineStorage == null) {
                    return null;
                }
                if (state !== undefined) {
                    return this._storage.setItem(state);
                }
                return this._storage.getItem() || [];
            },
            _isServerGrouped: function () {
                var group = this.group() || [];
                return this.options.serverGrouping && group.length;
            },
            _pushCreate: function (result) {
                this._push(result, 'pushCreate');
            },
            _pushUpdate: function (result) {
                this._push(result, 'pushUpdate');
            },
            _pushDestroy: function (result) {
                this._push(result, 'pushDestroy');
            },
            _push: function (result, operation) {
                var data = this._readData(result);
                if (!data) {
                    data = result;
                }
                this[operation](data);
            },
            _flatData: function (data, skip) {
                if (data) {
                    if (this._isServerGrouped()) {
                        return flattenGroups(data);
                    }
                    if (!skip) {
                        for (var idx = 0; idx < data.length; idx++) {
                            data.at(idx);
                        }
                    }
                }
                return data;
            },
            parent: noop,
            get: function (id) {
                var idx, length, data = this._flatData(this._data, this.options.useRanges);
                for (idx = 0, length = data.length; idx < length; idx++) {
                    if (data[idx].id == id) {
                        return data[idx];
                    }
                }
            },
            getByUid: function (id) {
                return this._getByUid(id, this._data);
            },
            _getByUid: function (id, dataItems) {
                var idx, length, data = this._flatData(dataItems, this.options.useRanges);
                if (!data) {
                    return;
                }
                for (idx = 0, length = data.length; idx < length; idx++) {
                    if (data[idx].uid == id) {
                        return data[idx];
                    }
                }
            },
            indexOf: function (model) {
                return indexOfModel(this._data, model);
            },
            at: function (index) {
                return this._data.at(index);
            },
            data: function (value) {
                var that = this;
                if (value !== undefined) {
                    that._detachObservableParents();
                    that._data = this._observe(value);
                    that._pristineData = value.slice(0);
                    that._storeData();
                    that._ranges = [];
                    that.trigger('reset');
                    that._addRange(that._data);
                    that._total = that._data.length;
                    that._pristineTotal = that._total;
                    that._process(that._data);
                } else {
                    if (that._data) {
                        for (var idx = 0; idx < that._data.length; idx++) {
                            that._data.at(idx);
                        }
                    }
                    return that._data;
                }
            },
            view: function (value) {
                if (value === undefined) {
                    return this._view;
                } else {
                    this._view = this._observeView(value);
                }
            },
            _observeView: function (data) {
                var that = this;
                replaceWithObservable(data, that._data, that._ranges, that.reader.model || ObservableObject, that._isServerGrouped());
                var view = new LazyObservableArray(data, that.reader.model);
                view.parent = function () {
                    return that.parent();
                };
                return view;
            },
            flatView: function () {
                var groups = this.group() || [];
                if (groups.length) {
                    return flattenGroups(this._view);
                } else {
                    return this._view;
                }
            },
            add: function (model) {
                return this.insert(this._data.length, model);
            },
            _createNewModel: function (model) {
                if (this.reader.model) {
                    return new this.reader.model(model);
                }
                if (model instanceof ObservableObject) {
                    return model;
                }
                return new ObservableObject(model);
            },
            insert: function (index, model) {
                if (!model) {
                    model = index;
                    index = 0;
                }
                if (!(model instanceof Model)) {
                    model = this._createNewModel(model);
                }
                if (this._isServerGrouped()) {
                    this._data.splice(index, 0, this._wrapInEmptyGroup(model));
                } else {
                    this._data.splice(index, 0, model);
                }
                this._insertModelInRange(index, model);
                return model;
            },
            pushInsert: function (index, items) {
                var that = this;
                var rangeSpan = that._getCurrentRangeSpan();
                if (!items) {
                    items = index;
                    index = 0;
                }
                if (!isArray(items)) {
                    items = [items];
                }
                var pushed = [];
                var autoSync = this.options.autoSync;
                this.options.autoSync = false;
                try {
                    for (var idx = 0; idx < items.length; idx++) {
                        var item = items[idx];
                        var result = this.insert(index, item);
                        pushed.push(result);
                        var pristine = result.toJSON();
                        if (this._isServerGrouped()) {
                            pristine = this._wrapInEmptyGroup(pristine);
                        }
                        this._pristineData.push(pristine);
                        if (rangeSpan && rangeSpan.length) {
                            $(rangeSpan).last()[0].pristineData.push(pristine);
                        }
                        index++;
                    }
                } finally {
                    this.options.autoSync = autoSync;
                }
                if (pushed.length) {
                    this.trigger('push', {
                        type: 'create',
                        items: pushed
                    });
                }
            },
            pushCreate: function (items) {
                this.pushInsert(this._data.length, items);
            },
            pushUpdate: function (items) {
                if (!isArray(items)) {
                    items = [items];
                }
                var pushed = [];
                for (var idx = 0; idx < items.length; idx++) {
                    var item = items[idx];
                    var model = this._createNewModel(item);
                    var target = this.get(model.id);
                    if (target) {
                        pushed.push(target);
                        target.accept(item);
                        target.trigger(CHANGE);
                        this._updatePristineForModel(target, item);
                    } else {
                        this.pushCreate(item);
                    }
                }
                if (pushed.length) {
                    this.trigger('push', {
                        type: 'update',
                        items: pushed
                    });
                }
            },
            pushDestroy: function (items) {
                var pushed = this._removeItems(items);
                if (pushed.length) {
                    this.trigger('push', {
                        type: 'destroy',
                        items: pushed
                    });
                }
            },
            _removeItems: function (items) {
                if (!isArray(items)) {
                    items = [items];
                }
                var destroyed = [];
                var autoSync = this.options.autoSync;
                this.options.autoSync = false;
                try {
                    for (var idx = 0; idx < items.length; idx++) {
                        var item = items[idx];
                        var model = this._createNewModel(item);
                        var found = false;
                        this._eachItem(this._data, function (items) {
                            for (var idx = 0; idx < items.length; idx++) {
                                var item = items.at(idx);
                                if (item.id === model.id) {
                                    destroyed.push(item);
                                    items.splice(idx, 1);
                                    found = true;
                                    break;
                                }
                            }
                        });
                        if (found) {
                            this._removePristineForModel(model);
                            this._destroyed.pop();
                        }
                    }
                } finally {
                    this.options.autoSync = autoSync;
                }
                return destroyed;
            },
            remove: function (model) {
                var result, that = this, hasGroups = that._isServerGrouped();
                this._eachItem(that._data, function (items) {
                    result = removeModel(items, model);
                    if (result && hasGroups) {
                        if (!result.isNew || !result.isNew()) {
                            that._destroyed.push(result);
                        }
                        return true;
                    }
                });
                this._removeModelFromRanges(model);
                return model;
            },
            destroyed: function () {
                return this._destroyed;
            },
            created: function () {
                var idx, length, result = [], data = this._flatData(this._data, this.options.useRanges);
                for (idx = 0, length = data.length; idx < length; idx++) {
                    if (data[idx].isNew && data[idx].isNew()) {
                        result.push(data[idx]);
                    }
                }
                return result;
            },
            updated: function () {
                var idx, length, result = [], data = this._flatData(this._data, this.options.useRanges);
                for (idx = 0, length = data.length; idx < length; idx++) {
                    if (data[idx].isNew && !data[idx].isNew() && data[idx].dirty) {
                        result.push(data[idx]);
                    }
                }
                return result;
            },
            sync: function () {
                var that = this, created = [], updated = [], destroyed = that._destroyed;
                var promise = $.Deferred().resolve().promise();
                if (that.online()) {
                    if (!that.reader.model) {
                        return promise;
                    }
                    created = that.created();
                    updated = that.updated();
                    var promises = [];
                    if (that.options.batch && that.transport.submit) {
                        promises = that._sendSubmit(created, updated, destroyed);
                    } else {
                        promises.push.apply(promises, that._send('create', created));
                        promises.push.apply(promises, that._send('update', updated));
                        promises.push.apply(promises, that._send('destroy', destroyed));
                    }
                    promise = $.when.apply(null, promises).then(function () {
                        var idx, length;
                        for (idx = 0, length = arguments.length; idx < length; idx++) {
                            if (arguments[idx]) {
                                that._accept(arguments[idx]);
                            }
                        }
                        that._storeData(true);
                        that._change({ action: 'sync' });
                        that.trigger(SYNC);
                    });
                } else {
                    that._storeData(true);
                    that._change({ action: 'sync' });
                }
                return promise;
            },
            cancelChanges: function (model) {
                var that = this;
                if (model instanceof kendo.data.Model) {
                    that._cancelModel(model);
                } else {
                    that._destroyed = [];
                    that._detachObservableParents();
                    that._data = that._observe(that._pristineData);
                    if (that.options.serverPaging) {
                        that._total = that._pristineTotal;
                    }
                    that._ranges = [];
                    that._addRange(that._data, 0);
                    that._change();
                    that._markOfflineUpdatesAsDirty();
                }
            },
            _markOfflineUpdatesAsDirty: function () {
                var that = this;
                if (that.options.offlineStorage != null) {
                    that._eachItem(that._data, function (items) {
                        for (var idx = 0; idx < items.length; idx++) {
                            var item = items.at(idx);
                            if (item.__state__ == 'update' || item.__state__ == 'create') {
                                item.dirty = true;
                            }
                        }
                    });
                }
            },
            hasChanges: function () {
                var idx, length, data = this._flatData(this._data, this.options.useRanges);
                if (this._destroyed.length) {
                    return true;
                }
                for (idx = 0, length = data.length; idx < length; idx++) {
                    if (data[idx].isNew && data[idx].isNew() || data[idx].dirty) {
                        return true;
                    }
                }
                return false;
            },
            _accept: function (result) {
                var that = this, models = result.models, response = result.response, idx = 0, serverGroup = that._isServerGrouped(), pristine = that._pristineData, type = result.type, length;
                that.trigger(REQUESTEND, {
                    response: response,
                    type: type
                });
                if (response && !isEmptyObject(response)) {
                    response = that.reader.parse(response);
                    if (that._handleCustomErrors(response)) {
                        return;
                    }
                    response = that.reader.data(response);
                    if (!isArray(response)) {
                        response = [response];
                    }
                } else {
                    response = $.map(models, function (model) {
                        return model.toJSON();
                    });
                }
                if (type === 'destroy') {
                    that._destroyed = [];
                }
                for (idx = 0, length = models.length; idx < length; idx++) {
                    if (type !== 'destroy') {
                        models[idx].accept(response[idx]);
                        if (type === 'create') {
                            pristine.push(serverGroup ? that._wrapInEmptyGroup(models[idx]) : response[idx]);
                        } else if (type === 'update') {
                            that._updatePristineForModel(models[idx], response[idx]);
                        }
                    } else {
                        that._removePristineForModel(models[idx]);
                    }
                }
            },
            _updatePristineForModel: function (model, values) {
                this._executeOnPristineForModel(model, function (index, items) {
                    kendo.deepExtend(items[index], values);
                });
            },
            _executeOnPristineForModel: function (model, callback) {
                this._eachPristineItem(function (items) {
                    var index = indexOfPristineModel(items, model);
                    if (index > -1) {
                        callback(index, items);
                        return true;
                    }
                });
            },
            _removePristineForModel: function (model) {
                this._executeOnPristineForModel(model, function (index, items) {
                    items.splice(index, 1);
                });
            },
            _readData: function (data) {
                var read = !this._isServerGrouped() ? this.reader.data : this.reader.groups;
                return read.call(this.reader, data);
            },
            _eachPristineItem: function (callback) {
                var that = this;
                var options = that.options;
                var rangeSpan = that._getCurrentRangeSpan();
                that._eachItem(that._pristineData, callback);
                if (options.serverPaging && options.useRanges) {
                    each(rangeSpan, function (i, range) {
                        that._eachItem(range.pristineData, callback);
                    });
                }
            },
            _eachItem: function (data, callback) {
                if (data && data.length) {
                    if (this._isServerGrouped()) {
                        eachGroupItems(data, callback);
                    } else {
                        callback(data);
                    }
                }
            },
            _pristineForModel: function (model) {
                var pristine, idx, callback = function (items) {
                        idx = indexOfPristineModel(items, model);
                        if (idx > -1) {
                            pristine = items[idx];
                            return true;
                        }
                    };
                this._eachPristineItem(callback);
                return pristine;
            },
            _cancelModel: function (model) {
                var that = this;
                var pristine = this._pristineForModel(model);
                this._eachItem(this._data, function (items) {
                    var idx = indexOfModel(items, model);
                    if (idx >= 0) {
                        if (pristine && (!model.isNew() || pristine.__state__)) {
                            items[idx].accept(pristine);
                            if (pristine.__state__ == 'update') {
                                items[idx].dirty = true;
                            }
                        } else {
                            items.splice(idx, 1);
                            that._removeModelFromRanges(model);
                        }
                    }
                });
            },
            _submit: function (promises, data) {
                var that = this;
                that.trigger(REQUESTSTART, { type: 'submit' });
                that.trigger(PROGRESS);
                that.transport.submit(extend({
                    success: function (response, type) {
                        var promise = $.grep(promises, function (x) {
                            return x.type == type;
                        })[0];
                        if (promise) {
                            promise.resolve({
                                response: response,
                                models: promise.models,
                                type: type
                            });
                        }
                    },
                    error: function (response, status, error) {
                        for (var idx = 0; idx < promises.length; idx++) {
                            promises[idx].reject(response);
                        }
                        that.error(response, status, error);
                    }
                }, data));
            },
            _sendSubmit: function (created, updated, destroyed) {
                var that = this, promises = [];
                if (that.options.batch) {
                    if (created.length) {
                        promises.push($.Deferred(function (deferred) {
                            deferred.type = 'create';
                            deferred.models = created;
                        }));
                    }
                    if (updated.length) {
                        promises.push($.Deferred(function (deferred) {
                            deferred.type = 'update';
                            deferred.models = updated;
                        }));
                    }
                    if (destroyed.length) {
                        promises.push($.Deferred(function (deferred) {
                            deferred.type = 'destroy';
                            deferred.models = destroyed;
                        }));
                    }
                    that._submit(promises, {
                        data: {
                            created: that.reader.serialize(toJSON(created)),
                            updated: that.reader.serialize(toJSON(updated)),
                            destroyed: that.reader.serialize(toJSON(destroyed))
                        }
                    });
                }
                return promises;
            },
            _promise: function (data, models, type) {
                var that = this;
                return $.Deferred(function (deferred) {
                    that.trigger(REQUESTSTART, { type: type });
                    that.trigger(PROGRESS);
                    that.transport[type].call(that.transport, extend({
                        success: function (response) {
                            deferred.resolve({
                                response: response,
                                models: models,
                                type: type
                            });
                        },
                        error: function (response, status, error) {
                            deferred.reject(response);
                            that.error(response, status, error);
                        }
                    }, data));
                }).promise();
            },
            _send: function (method, data) {
                var that = this, idx, length, promises = [], converted = that.reader.serialize(toJSON(data));
                if (that.options.batch) {
                    if (data.length) {
                        promises.push(that._promise({ data: { models: converted } }, data, method));
                    }
                } else {
                    for (idx = 0, length = data.length; idx < length; idx++) {
                        promises.push(that._promise({ data: converted[idx] }, [data[idx]], method));
                    }
                }
                return promises;
            },
            read: function (data) {
                var that = this, params = that._params(data);
                var deferred = $.Deferred();
                that._queueRequest(params, function () {
                    var isPrevented = that.trigger(REQUESTSTART, { type: 'read' });
                    if (!isPrevented) {
                        that.trigger(PROGRESS);
                        that._ranges = [];
                        that.trigger('reset');
                        if (that.online()) {
                            that.transport.read({
                                data: params,
                                success: function (data) {
                                    that._ranges = [];
                                    that.success(data, params);
                                    deferred.resolve();
                                },
                                error: function () {
                                    var args = slice.call(arguments);
                                    that.error.apply(that, args);
                                    deferred.reject.apply(deferred, args);
                                }
                            });
                        } else if (that.options.offlineStorage != null) {
                            that.success(that.offlineData(), params);
                            deferred.resolve();
                        }
                    } else {
                        that._dequeueRequest();
                        deferred.resolve(isPrevented);
                    }
                });
                return deferred.promise();
            },
            _readAggregates: function (data) {
                return this.reader.aggregates(data);
            },
            success: function (data) {
                var that = this, options = that.options;
                that.trigger(REQUESTEND, {
                    response: data,
                    type: 'read'
                });
                if (that.online()) {
                    data = that.reader.parse(data);
                    if (that._handleCustomErrors(data)) {
                        that._dequeueRequest();
                        return;
                    }
                    that._total = that.reader.total(data);
                    if (that._pageSize > that._total) {
                        that._pageSize = that._total;
                        if (that.options.pageSize && that.options.pageSize > that._pageSize) {
                            that._pageSize = that.options.pageSize;
                        }
                    }
                    if (that._aggregate && options.serverAggregates) {
                        that._aggregateResult = that._readAggregates(data);
                    }
                    data = that._readData(data);
                    that._destroyed = [];
                } else {
                    data = that._readData(data);
                    var items = [];
                    var itemIds = {};
                    var model = that.reader.model;
                    var idField = model ? model.idField : 'id';
                    var idx;
                    for (idx = 0; idx < this._destroyed.length; idx++) {
                        var id = this._destroyed[idx][idField];
                        itemIds[id] = id;
                    }
                    for (idx = 0; idx < data.length; idx++) {
                        var item = data[idx];
                        var state = item.__state__;
                        if (state == 'destroy') {
                            if (!itemIds[item[idField]]) {
                                this._destroyed.push(this._createNewModel(item));
                            }
                        } else {
                            items.push(item);
                        }
                    }
                    data = items;
                    that._total = data.length;
                }
                that._pristineTotal = that._total;
                that._pristineData = data.slice(0);
                that._detachObservableParents();
                if (that.options.endless) {
                    that._data.unbind(CHANGE, that._changeHandler);
                    if (that._isServerGrouped() && that._data[that._data.length - 1].value === data[0].value) {
                        fillLastGroup(that._data[that._data.length - 1], data[0]);
                        data.shift();
                    }
                    data = that._observe(data);
                    for (var i = 0; i < data.length; i++) {
                        that._data.push(data[i]);
                    }
                    that._data.bind(CHANGE, that._changeHandler);
                } else {
                    that._data = that._observe(data);
                }
                that._markOfflineUpdatesAsDirty();
                that._storeData();
                that._addRange(that._data);
                that._process(that._data);
                that._dequeueRequest();
            },
            _detachObservableParents: function () {
                if (this._data && this._shouldDetachObservableParents) {
                    for (var idx = 0; idx < this._data.length; idx++) {
                        if (this._data[idx].parent) {
                            this._data[idx].parent = noop;
                        }
                    }
                }
            },
            _storeData: function (updatePristine) {
                var serverGrouping = this._isServerGrouped();
                var model = this.reader.model;
                function items(data) {
                    var state = [];
                    for (var idx = 0; idx < data.length; idx++) {
                        var dataItem = data.at(idx);
                        var item = dataItem.toJSON();
                        if (serverGrouping && dataItem.items) {
                            item.items = items(dataItem.items);
                        } else {
                            item.uid = dataItem.uid;
                            if (model) {
                                if (dataItem.isNew()) {
                                    item.__state__ = 'create';
                                } else if (dataItem.dirty) {
                                    item.__state__ = 'update';
                                }
                            }
                        }
                        state.push(item);
                    }
                    return state;
                }
                if (this.options.offlineStorage != null) {
                    var state = items(this._data);
                    var destroyed = [];
                    for (var idx = 0; idx < this._destroyed.length; idx++) {
                        var item = this._destroyed[idx].toJSON();
                        item.__state__ = 'destroy';
                        destroyed.push(item);
                    }
                    this.offlineData(state.concat(destroyed));
                    if (updatePristine) {
                        this._pristineData = this.reader._wrapDataAccessBase(state);
                    }
                }
            },
            _addRange: function (data, skip) {
                var that = this, start = typeof skip !== 'undefined' ? skip : that._skip || 0, end = start + that._flatData(data, true).length;
                that._ranges.push({
                    start: start,
                    end: end,
                    data: data,
                    pristineData: data.toJSON(),
                    timestamp: that._timeStamp()
                });
                that._sortRanges();
            },
            _sortRanges: function () {
                this._ranges.sort(function (x, y) {
                    return x.start - y.start;
                });
            },
            error: function (xhr, status, errorThrown) {
                this._dequeueRequest();
                this.trigger(REQUESTEND, {});
                this.trigger(ERROR, {
                    xhr: xhr,
                    status: status,
                    errorThrown: errorThrown
                });
            },
            _params: function (data) {
                var that = this, options = extend({
                        take: that.take(),
                        skip: that.skip(),
                        page: that.page(),
                        pageSize: that.pageSize(),
                        sort: that._sort,
                        filter: that._filter,
                        group: that._group,
                        aggregate: that._aggregate
                    }, data);
                if (!that.options.serverPaging) {
                    delete options.take;
                    delete options.skip;
                    delete options.page;
                    delete options.pageSize;
                }
                if (!that.options.serverGrouping) {
                    delete options.group;
                } else if (that.reader.model && options.group) {
                    options.group = convertDescriptorsField(options.group, that.reader.model);
                }
                if (!that.options.serverFiltering) {
                    delete options.filter;
                } else if (that.reader.model && options.filter) {
                    options.filter = convertFilterDescriptorsField(options.filter, that.reader.model);
                }
                if (!that.options.serverSorting) {
                    delete options.sort;
                } else if (that.reader.model && options.sort) {
                    options.sort = convertDescriptorsField(options.sort, that.reader.model);
                }
                if (!that.options.serverAggregates) {
                    delete options.aggregate;
                } else if (that.reader.model && options.aggregate) {
                    options.aggregate = convertDescriptorsField(options.aggregate, that.reader.model);
                }
                return options;
            },
            _queueRequest: function (options, callback) {
                var that = this;
                if (!that._requestInProgress) {
                    that._requestInProgress = true;
                    that._pending = undefined;
                    callback();
                } else {
                    that._pending = {
                        callback: proxy(callback, that),
                        options: options
                    };
                }
            },
            _dequeueRequest: function () {
                var that = this;
                that._requestInProgress = false;
                if (that._pending) {
                    that._queueRequest(that._pending.options, that._pending.callback);
                }
            },
            _handleCustomErrors: function (response) {
                if (this.reader.errors) {
                    var errors = this.reader.errors(response);
                    if (errors) {
                        this.trigger(ERROR, {
                            xhr: null,
                            status: 'customerror',
                            errorThrown: 'custom error',
                            errors: errors
                        });
                        return true;
                    }
                }
                return false;
            },
            _shouldWrap: function (data) {
                var model = this.reader.model;
                if (model && data.length) {
                    return !(data[0] instanceof model);
                }
                return false;
            },
            _observe: function (data) {
                var that = this, model = that.reader.model;
                that._shouldDetachObservableParents = true;
                if (data instanceof ObservableArray) {
                    that._shouldDetachObservableParents = false;
                    if (that._shouldWrap(data)) {
                        data.type = that.reader.model;
                        data.wrapAll(data, data);
                    }
                } else {
                    var arrayType = that.pageSize() && !that.options.serverPaging ? LazyObservableArray : ObservableArray;
                    data = new arrayType(data, that.reader.model);
                    data.parent = function () {
                        return that.parent();
                    };
                }
                if (that._isServerGrouped()) {
                    wrapGroupItems(data, model);
                }
                if (that._changeHandler && that._data && that._data instanceof ObservableArray) {
                    that._data.unbind(CHANGE, that._changeHandler);
                } else {
                    that._changeHandler = proxy(that._change, that);
                }
                return data.bind(CHANGE, that._changeHandler);
            },
            _updateTotalForAction: function (action, items) {
                var that = this;
                var total = parseInt(that._total, 10);
                if (!isNumber(that._total)) {
                    total = parseInt(that._pristineTotal, 10);
                }
                if (action === 'add') {
                    total += items.length;
                } else if (action === 'remove') {
                    total -= items.length;
                } else if (action !== 'itemchange' && action !== 'sync' && !that.options.serverPaging) {
                    total = that._pristineTotal;
                } else if (action === 'sync') {
                    total = that._pristineTotal = parseInt(that._total, 10);
                }
                that._total = total;
            },
            _change: function (e) {
                var that = this, idx, length, action = e ? e.action : '';
                if (action === 'remove') {
                    for (idx = 0, length = e.items.length; idx < length; idx++) {
                        if (!e.items[idx].isNew || !e.items[idx].isNew()) {
                            that._destroyed.push(e.items[idx]);
                        }
                    }
                }
                if (that.options.autoSync && (action === 'add' || action === 'remove' || action === 'itemchange')) {
                    var handler = function (args) {
                        if (args.action === 'sync') {
                            that.unbind('change', handler);
                            that._updateTotalForAction(action, e.items);
                        }
                    };
                    that.first('change', handler);
                    that.sync();
                } else {
                    that._updateTotalForAction(action, e ? e.items : []);
                    that._process(that._data, e);
                }
            },
            _calculateAggregates: function (data, options) {
                options = options || {};
                var query = new Query(data), aggregates = options.aggregate, filter = options.filter;
                if (filter) {
                    query = query.filter(filter);
                }
                return query.aggregate(aggregates);
            },
            _process: function (data, e) {
                var that = this, options = {}, result;
                if (that.options.serverPaging !== true) {
                    options.skip = that._skip;
                    options.take = that._take || that._pageSize;
                    if (options.skip === undefined && that._page !== undefined && that._pageSize !== undefined) {
                        options.skip = (that._page - 1) * that._pageSize;
                    }
                    if (that.options.useRanges) {
                        options.skip = that.currentRangeStart();
                    }
                }
                if (that.options.serverSorting !== true) {
                    options.sort = that._sort;
                }
                if (that.options.serverFiltering !== true) {
                    options.filter = that._filter;
                }
                if (that.options.serverGrouping !== true) {
                    options.group = that._group;
                }
                if (that.options.serverAggregates !== true) {
                    options.aggregate = that._aggregate;
                    that._aggregateResult = that._calculateAggregates(data, options);
                }
                result = that._queryProcess(data, options);
                that.view(result.data);
                if (result.total !== undefined && !that.options.serverFiltering) {
                    that._total = result.total;
                }
                e = e || {};
                e.items = e.items || that._view;
                that.trigger(CHANGE, e);
            },
            _queryProcess: function (data, options) {
                if (this.options.inPlaceSort) {
                    return Query.process(data, options, this.options.inPlaceSort);
                } else {
                    return Query.process(data, options);
                }
            },
            _mergeState: function (options) {
                var that = this;
                if (options !== undefined) {
                    that._pageSize = options.pageSize;
                    that._page = options.page;
                    that._sort = options.sort;
                    that._filter = options.filter;
                    that._group = options.group;
                    that._aggregate = options.aggregate;
                    that._skip = that._currentRangeStart = options.skip;
                    that._take = options.take;
                    if (that._skip === undefined) {
                        that._skip = that._currentRangeStart = that.skip();
                        options.skip = that.skip();
                    }
                    if (that._take === undefined && that._pageSize !== undefined) {
                        that._take = that._pageSize;
                        options.take = that._take;
                    }
                    if (options.sort) {
                        that._sort = options.sort = normalizeSort(options.sort);
                    }
                    if (options.filter) {
                        that._filter = options.filter = normalizeFilter(options.filter);
                    }
                    if (options.group) {
                        that._group = options.group = normalizeGroup(options.group);
                    }
                    if (options.aggregate) {
                        that._aggregate = options.aggregate = normalizeAggregate(options.aggregate);
                    }
                }
                return options;
            },
            query: function (options) {
                var result;
                var remote = this.options.serverSorting || this.options.serverPaging || this.options.serverFiltering || this.options.serverGrouping || this.options.serverAggregates;
                if (remote || (this._data === undefined || this._data.length === 0) && !this._destroyed.length) {
                    if (this.options.endless) {
                        var moreItemsCount = options.pageSize - this.pageSize();
                        if (moreItemsCount > 0) {
                            moreItemsCount = this.pageSize();
                            options.page = options.pageSize / moreItemsCount;
                            options.pageSize = moreItemsCount;
                        } else {
                            options.page = 1;
                            this.options.endless = false;
                        }
                    }
                    return this.read(this._mergeState(options));
                }
                var isPrevented = this.trigger(REQUESTSTART, { type: 'read' });
                if (!isPrevented) {
                    this.trigger(PROGRESS);
                    result = this._queryProcess(this._data, this._mergeState(options));
                    if (!this.options.serverFiltering) {
                        if (result.total !== undefined) {
                            this._total = result.total;
                        } else {
                            this._total = this._data.length;
                        }
                    }
                    this._aggregateResult = this._calculateAggregates(this._data, options);
                    this.view(result.data);
                    this.trigger(REQUESTEND, { type: 'read' });
                    this.trigger(CHANGE, { items: result.data });
                }
                return $.Deferred().resolve(isPrevented).promise();
            },
            fetch: function (callback) {
                var that = this;
                var fn = function (isPrevented) {
                    if (isPrevented !== true && isFunction(callback)) {
                        callback.call(that);
                    }
                };
                return this._query().then(fn);
            },
            _query: function (options) {
                var that = this;
                return that.query(extend({}, {
                    page: that.page(),
                    pageSize: that.pageSize(),
                    sort: that.sort(),
                    filter: that.filter(),
                    group: that.group(),
                    aggregate: that.aggregate()
                }, options));
            },
            next: function (options) {
                var that = this, page = that.page(), total = that.total();
                options = options || {};
                if (!page || total && page + 1 > that.totalPages()) {
                    return;
                }
                that._skip = that._currentRangeStart = page * that.take();
                page += 1;
                options.page = page;
                that._query(options);
                return page;
            },
            prev: function (options) {
                var that = this, page = that.page();
                options = options || {};
                if (!page || page === 1) {
                    return;
                }
                that._skip = that._currentRangeStart = that._skip - that.take();
                page -= 1;
                options.page = page;
                that._query(options);
                return page;
            },
            page: function (val) {
                var that = this, skip;
                if (val !== undefined) {
                    val = math.max(math.min(math.max(val, 1), that.totalPages()), 1);
                    that._query({ page: val });
                    return;
                }
                skip = that.skip();
                return skip !== undefined ? math.round((skip || 0) / (that.take() || 1)) + 1 : undefined;
            },
            pageSize: function (val) {
                var that = this;
                if (val !== undefined) {
                    that._query({
                        pageSize: val,
                        page: 1
                    });
                    return;
                }
                return that.take();
            },
            sort: function (val) {
                var that = this;
                if (val !== undefined) {
                    that._query({ sort: val });
                    return;
                }
                return that._sort;
            },
            filter: function (val) {
                var that = this;
                if (val === undefined) {
                    return that._filter;
                }
                that.trigger('reset');
                that._query({
                    filter: val,
                    page: 1
                });
            },
            group: function (val) {
                var that = this;
                if (val !== undefined) {
                    that._query({ group: val });
                    return;
                }
                return that._group;
            },
            total: function () {
                return parseInt(this._total || 0, 10);
            },
            aggregate: function (val) {
                var that = this;
                if (val !== undefined) {
                    that._query({ aggregate: val });
                    return;
                }
                return that._aggregate;
            },
            aggregates: function () {
                var result = this._aggregateResult;
                if (isEmptyObject(result)) {
                    result = this._emptyAggregates(this.aggregate());
                }
                return result;
            },
            _emptyAggregates: function (aggregates) {
                var result = {};
                if (!isEmptyObject(aggregates)) {
                    var aggregate = {};
                    if (!isArray(aggregates)) {
                        aggregates = [aggregates];
                    }
                    for (var idx = 0; idx < aggregates.length; idx++) {
                        aggregate[aggregates[idx].aggregate] = 0;
                        result[aggregates[idx].field] = aggregate;
                    }
                }
                return result;
            },
            _wrapInEmptyGroup: function (model) {
                var groups = this.group(), parent, group, idx, length;
                for (idx = groups.length - 1, length = 0; idx >= length; idx--) {
                    group = groups[idx];
                    parent = {
                        value: model.get(group.field),
                        field: group.field,
                        items: parent ? [parent] : [model],
                        hasSubgroups: !!parent,
                        aggregates: this._emptyAggregates(group.aggregates)
                    };
                }
                return parent;
            },
            totalPages: function () {
                var that = this, pageSize = that.pageSize() || that.total();
                return math.ceil((that.total() || 0) / pageSize);
            },
            inRange: function (skip, take) {
                var that = this, end = math.min(skip + take, that.total());
                if (!that.options.serverPaging && that._data.length > 0) {
                    return true;
                }
                return that._findRange(skip, end).length > 0;
            },
            lastRange: function () {
                var ranges = this._ranges;
                return ranges[ranges.length - 1] || {
                    start: 0,
                    end: 0,
                    data: []
                };
            },
            firstItemUid: function () {
                var ranges = this._ranges;
                return ranges.length && ranges[0].data.length && ranges[0].data[0].uid;
            },
            enableRequestsInProgress: function () {
                this._skipRequestsInProgress = false;
            },
            _timeStamp: function () {
                return new Date().getTime();
            },
            range: function (skip, take, callback) {
                this._currentRequestTimeStamp = this._timeStamp();
                this._skipRequestsInProgress = true;
                skip = math.min(skip || 0, this.total());
                callback = isFunction(callback) ? callback : noop;
                var that = this, pageSkip = math.max(math.floor(skip / take), 0) * take, size = math.min(pageSkip + take, that.total()), data;
                data = that._findRange(skip, math.min(skip + take, that.total()));
                if (data.length || that.total() === 0) {
                    that._processRangeData(data, skip, take, pageSkip, size);
                    callback();
                    return;
                }
                if (take !== undefined) {
                    if (!that._rangeExists(pageSkip, size)) {
                        that.prefetch(pageSkip, take, function () {
                            if (skip > pageSkip && size < that.total() && !that._rangeExists(size, math.min(size + take, that.total()))) {
                                that.prefetch(size, take, function () {
                                    that.range(skip, take, callback);
                                });
                            } else {
                                that.range(skip, take, callback);
                            }
                        });
                    } else if (pageSkip < skip) {
                        that.prefetch(size, take, function () {
                            that.range(skip, take, callback);
                        });
                    }
                }
            },
            _findRange: function (start, end) {
                var that = this, ranges = that._ranges, range, data = [], skipIdx, takeIdx, startIndex, endIndex, rangeData, rangeEnd, processed, options = that.options, remote = options.serverSorting || options.serverPaging || options.serverFiltering || options.serverGrouping || options.serverAggregates, flatData, count, length;
                for (skipIdx = 0, length = ranges.length; skipIdx < length; skipIdx++) {
                    range = ranges[skipIdx];
                    if (start >= range.start && start <= range.end) {
                        count = 0;
                        for (takeIdx = skipIdx; takeIdx < length; takeIdx++) {
                            range = ranges[takeIdx];
                            flatData = that._flatData(range.data, true);
                            if (flatData.length && start + count >= range.start) {
                                rangeData = range.data;
                                rangeEnd = range.end;
                                if (!remote) {
                                    if (options.inPlaceSort) {
                                        processed = that._queryProcess(range.data, { filter: that.filter() });
                                    } else {
                                        var sort = normalizeGroup(that.group() || []).concat(normalizeSort(that.sort() || []));
                                        processed = that._queryProcess(range.data, {
                                            sort: sort,
                                            filter: that.filter()
                                        });
                                    }
                                    flatData = rangeData = processed.data;
                                    if (processed.total !== undefined) {
                                        rangeEnd = processed.total;
                                    }
                                }
                                startIndex = 0;
                                if (start + count > range.start) {
                                    startIndex = start + count - range.start;
                                }
                                endIndex = flatData.length;
                                if (rangeEnd > end) {
                                    endIndex = endIndex - (rangeEnd - end);
                                }
                                count += endIndex - startIndex;
                                data = that._mergeGroups(data, rangeData, startIndex, endIndex);
                                if (end <= range.end && count == end - start) {
                                    return data;
                                }
                            }
                        }
                        break;
                    }
                }
                return [];
            },
            _mergeGroups: function (data, range, skip, take) {
                if (this._isServerGrouped()) {
                    var temp = range.toJSON(), prevGroup;
                    if (data.length) {
                        prevGroup = data[data.length - 1];
                    }
                    mergeGroups(prevGroup, temp, skip, take);
                    return data.concat(temp);
                }
                return data.concat(range.slice(skip, take));
            },
            _processRangeData: function (data, skip, take, pageSkip, size) {
                var that = this;
                that._pending = undefined;
                that._skip = skip > that.skip() ? math.min(size, (that.totalPages() - 1) * that.take()) : pageSkip;
                that._currentRangeStart = skip;
                that._take = take;
                var paging = that.options.serverPaging;
                var sorting = that.options.serverSorting;
                var filtering = that.options.serverFiltering;
                var aggregates = that.options.serverAggregates;
                try {
                    that.options.serverPaging = true;
                    if (!that._isServerGrouped() && !(that.group() && that.group().length)) {
                        that.options.serverSorting = true;
                    }
                    that.options.serverFiltering = true;
                    that.options.serverPaging = true;
                    that.options.serverAggregates = true;
                    if (paging) {
                        that._detachObservableParents();
                        that._data = data = that._observe(data);
                    }
                    that._process(data);
                } finally {
                    that.options.serverPaging = paging;
                    that.options.serverSorting = sorting;
                    that.options.serverFiltering = filtering;
                    that.options.serverAggregates = aggregates;
                }
            },
            skip: function () {
                var that = this;
                if (that._skip === undefined) {
                    return that._page !== undefined ? (that._page - 1) * (that.take() || 1) : undefined;
                }
                return that._skip;
            },
            currentRangeStart: function () {
                return this._currentRangeStart || 0;
            },
            take: function () {
                return this._take || this._pageSize;
            },
            _prefetchSuccessHandler: function (skip, size, callback, force) {
                var that = this;
                var timestamp = that._timeStamp();
                return function (data) {
                    var found = false, range = {
                            start: skip,
                            end: size,
                            data: [],
                            timestamp: that._timeStamp()
                        }, idx, length, temp;
                    that._dequeueRequest();
                    that.trigger(REQUESTEND, {
                        response: data,
                        type: 'read'
                    });
                    data = that.reader.parse(data);
                    temp = that._readData(data);
                    if (temp.length) {
                        for (idx = 0, length = that._ranges.length; idx < length; idx++) {
                            if (that._ranges[idx].start === skip) {
                                found = true;
                                range = that._ranges[idx];
                                range.pristineData = temp;
                                range.data = that._observe(temp);
                                range.end = range.start + that._flatData(range.data, true).length;
                                that._sortRanges();
                                break;
                            }
                        }
                        if (!found) {
                            that._addRange(that._observe(temp), skip);
                        }
                    }
                    that._total = that.reader.total(data);
                    if (force || (timestamp >= that._currentRequestTimeStamp || !that._skipRequestsInProgress)) {
                        if (callback && temp.length) {
                            callback();
                        } else {
                            that.trigger(CHANGE, {});
                        }
                    }
                };
            },
            prefetch: function (skip, take, callback) {
                var that = this, size = math.min(skip + take, that.total()), options = {
                        take: take,
                        skip: skip,
                        page: skip / take + 1,
                        pageSize: take,
                        sort: that._sort,
                        filter: that._filter,
                        group: that._group,
                        aggregate: that._aggregate
                    };
                if (!that._rangeExists(skip, size)) {
                    clearTimeout(that._timeout);
                    that._timeout = setTimeout(function () {
                        that._queueRequest(options, function () {
                            if (!that.trigger(REQUESTSTART, { type: 'read' })) {
                                that.transport.read({
                                    data: that._params(options),
                                    success: that._prefetchSuccessHandler(skip, size, callback),
                                    error: function () {
                                        var args = slice.call(arguments);
                                        that.error.apply(that, args);
                                    }
                                });
                            } else {
                                that._dequeueRequest();
                            }
                        });
                    }, 100);
                } else if (callback) {
                    callback();
                }
            },
            _multiplePrefetch: function (skip, take, callback) {
                var that = this, size = math.min(skip + take, that.total()), options = {
                        take: take,
                        skip: skip,
                        page: skip / take + 1,
                        pageSize: take,
                        sort: that._sort,
                        filter: that._filter,
                        group: that._group,
                        aggregate: that._aggregate
                    };
                if (!that._rangeExists(skip, size)) {
                    if (!that.trigger(REQUESTSTART, { type: 'read' })) {
                        that.transport.read({
                            data: that._params(options),
                            success: that._prefetchSuccessHandler(skip, size, callback, true)
                        });
                    }
                } else if (callback) {
                    callback();
                }
            },
            _rangeExists: function (start, end) {
                var that = this, ranges = that._ranges, idx, length;
                for (idx = 0, length = ranges.length; idx < length; idx++) {
                    if (ranges[idx].start <= start && ranges[idx].end >= end) {
                        return true;
                    }
                }
                return false;
            },
            _getCurrentRangeSpan: function () {
                var that = this;
                var ranges = that._ranges;
                var start = that.currentRangeStart();
                var end = start + (that.take() || 0);
                var rangeSpan = [];
                var range;
                var idx;
                var length = ranges.length;
                for (idx = 0; idx < length; idx++) {
                    range = ranges[idx];
                    if (range.start <= start && range.end >= start || range.start >= start && range.start <= end) {
                        rangeSpan.push(range);
                    }
                }
                return rangeSpan;
            },
            _removeModelFromRanges: function (model) {
                var that = this;
                var result, range;
                for (var idx = 0, length = this._ranges.length; idx < length; idx++) {
                    range = this._ranges[idx];
                    this._eachItem(range.data, function (items) {
                        result = removeModel(items, model);
                    });
                    if (result) {
                        break;
                    }
                }
                that._updateRangesLength();
            },
            _insertModelInRange: function (index, model) {
                var that = this;
                var ranges = that._ranges || [];
                var rangesLength = ranges.length;
                var range;
                var i;
                for (i = 0; i < rangesLength; i++) {
                    range = ranges[i];
                    if (range.start <= index && range.end >= index) {
                        if (!that._getByUid(model.uid, range.data)) {
                            if (that._isServerGrouped()) {
                                range.data.splice(index, 0, that._wrapInEmptyGroup(model));
                            } else {
                                range.data.splice(index, 0, model);
                            }
                        }
                        break;
                    }
                }
                that._updateRangesLength();
            },
            _updateRangesLength: function () {
                var that = this;
                var ranges = that._ranges || [];
                var rangesLength = ranges.length;
                var mismatchFound = false;
                var mismatchLength = 0;
                var lengthDifference = 0;
                var range;
                var i;
                for (i = 0; i < rangesLength; i++) {
                    range = ranges[i];
                    lengthDifference = that._flatData(range.data, true).length - math.abs(range.end - range.start);
                    if (!mismatchFound && lengthDifference !== 0) {
                        mismatchFound = true;
                        mismatchLength = lengthDifference;
                        range.end += mismatchLength;
                        continue;
                    }
                    if (mismatchFound) {
                        range.start += mismatchLength;
                        range.end += mismatchLength;
                    }
                }
            }
        });
        var Transport = {};
        Transport.create = function (options, data, dataSource) {
            var transport, transportOptions = options.transport ? $.extend({}, options.transport) : null;
            if (transportOptions) {
                transportOptions.read = typeof transportOptions.read === STRING ? { url: transportOptions.read } : transportOptions.read;
                if (options.type === 'jsdo') {
                    transportOptions.dataSource = dataSource;
                }
                if (options.type) {
                    kendo.data.transports = kendo.data.transports || {};
                    kendo.data.schemas = kendo.data.schemas || {};
                    if (!kendo.data.transports[options.type]) {
                        kendo.logToConsole('Unknown DataSource transport type \'' + options.type + '\'.\nVerify that registration scripts for this type are included after Kendo UI on the page.', 'warn');
                    } else if (!isPlainObject(kendo.data.transports[options.type])) {
                        transport = new kendo.data.transports[options.type](extend(transportOptions, { data: data }));
                    } else {
                        transportOptions = extend(true, {}, kendo.data.transports[options.type], transportOptions);
                    }
                    options.schema = extend(true, {}, kendo.data.schemas[options.type], options.schema);
                }
                if (!transport) {
                    transport = isFunction(transportOptions.read) ? transportOptions : new RemoteTransport(transportOptions);
                }
            } else {
                transport = new LocalTransport({ data: options.data || [] });
            }
            return transport;
        };
        DataSource.create = function (options) {
            if (isArray(options) || options instanceof ObservableArray) {
                options = { data: options };
            }
            var dataSource = options || {}, data = dataSource.data, fields = dataSource.fields, table = dataSource.table, select = dataSource.select, idx, length, model = {}, field;
            if (!data && fields && !dataSource.transport) {
                if (table) {
                    data = inferTable(table, fields);
                } else if (select) {
                    data = inferSelect(select, fields);
                    if (dataSource.group === undefined && data[0] && data[0].optgroup !== undefined) {
                        dataSource.group = 'optgroup';
                    }
                }
            }
            if (kendo.data.Model && fields && (!dataSource.schema || !dataSource.schema.model)) {
                for (idx = 0, length = fields.length; idx < length; idx++) {
                    field = fields[idx];
                    if (field.type) {
                        model[field.field] = field;
                    }
                }
                if (!isEmptyObject(model)) {
                    dataSource.schema = extend(true, dataSource.schema, { model: { fields: model } });
                }
            }
            dataSource.data = data;
            select = null;
            dataSource.select = null;
            table = null;
            dataSource.table = null;
            return dataSource instanceof DataSource ? dataSource : new DataSource(dataSource);
        };
        function inferSelect(select, fields) {
            select = $(select)[0];
            var options = select.options;
            var firstField = fields[0];
            var secondField = fields[1];
            var data = [];
            var idx, length;
            var optgroup;
            var option;
            var record;
            var value;
            for (idx = 0, length = options.length; idx < length; idx++) {
                record = {};
                option = options[idx];
                optgroup = option.parentNode;
                if (optgroup === select) {
                    optgroup = null;
                }
                if (option.disabled || optgroup && optgroup.disabled) {
                    continue;
                }
                if (optgroup) {
                    record.optgroup = optgroup.label;
                }
                record[firstField.field] = option.text;
                value = option.attributes.value;
                if (value && value.specified) {
                    value = option.value;
                } else {
                    value = option.text;
                }
                record[secondField.field] = value;
                data.push(record);
            }
            return data;
        }
        function inferTable(table, fields) {
            var tbody = $(table)[0].tBodies[0], rows = tbody ? tbody.rows : [], idx, length, fieldIndex, fieldCount = fields.length, data = [], cells, record, cell, empty;
            for (idx = 0, length = rows.length; idx < length; idx++) {
                record = {};
                empty = true;
                cells = rows[idx].cells;
                for (fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++) {
                    cell = cells[fieldIndex];
                    if (cell.nodeName.toLowerCase() !== 'th') {
                        empty = false;
                        record[fields[fieldIndex].field] = cell.innerHTML;
                    }
                }
                if (!empty) {
                    data.push(record);
                }
            }
            return data;
        }
        var Node = Model.define({
            idField: 'id',
            init: function (value) {
                var that = this, hasChildren = that.hasChildren || value && value.hasChildren, childrenField = 'items', childrenOptions = {};
                kendo.data.Model.fn.init.call(that, value);
                if (typeof that.children === STRING) {
                    childrenField = that.children;
                }
                childrenOptions = {
                    schema: {
                        data: childrenField,
                        model: {
                            hasChildren: hasChildren,
                            id: that.idField,
                            fields: that.fields
                        }
                    }
                };
                if (typeof that.children !== STRING) {
                    extend(childrenOptions, that.children);
                }
                childrenOptions.data = value;
                if (!hasChildren) {
                    hasChildren = childrenOptions.schema.data;
                }
                if (typeof hasChildren === STRING) {
                    hasChildren = kendo.getter(hasChildren);
                }
                if (isFunction(hasChildren)) {
                    var hasChildrenObject = hasChildren.call(that, that);
                    if (hasChildrenObject && hasChildrenObject.length === 0) {
                        that.hasChildren = false;
                    } else {
                        that.hasChildren = !!hasChildrenObject;
                    }
                }
                that._childrenOptions = childrenOptions;
                if (that.hasChildren) {
                    that._initChildren();
                }
                that._loaded = !!(value && value._loaded);
            },
            _initChildren: function () {
                var that = this;
                var children, transport, parameterMap;
                if (!(that.children instanceof HierarchicalDataSource)) {
                    children = that.children = new HierarchicalDataSource(that._childrenOptions);
                    transport = children.transport;
                    parameterMap = transport.parameterMap;
                    transport.parameterMap = function (data, type) {
                        data[that.idField || 'id'] = that.id;
                        if (parameterMap) {
                            data = parameterMap(data, type);
                        }
                        return data;
                    };
                    children.parent = function () {
                        return that;
                    };
                    children.bind(CHANGE, function (e) {
                        e.node = e.node || that;
                        that.trigger(CHANGE, e);
                    });
                    children.bind(ERROR, function (e) {
                        var collection = that.parent();
                        if (collection) {
                            e.node = e.node || that;
                            collection.trigger(ERROR, e);
                        }
                    });
                    that._updateChildrenField();
                }
            },
            append: function (model) {
                this._initChildren();
                this.loaded(true);
                this.children.add(model);
            },
            hasChildren: false,
            level: function () {
                var parentNode = this.parentNode(), level = 0;
                while (parentNode && parentNode.parentNode) {
                    level++;
                    parentNode = parentNode.parentNode ? parentNode.parentNode() : null;
                }
                return level;
            },
            _updateChildrenField: function () {
                var fieldName = this._childrenOptions.schema.data;
                this[fieldName || 'items'] = this.children.data();
            },
            _childrenLoaded: function () {
                this._loaded = true;
                this._updateChildrenField();
            },
            load: function () {
                var options = {};
                var method = '_query';
                var children, promise;
                if (this.hasChildren) {
                    this._initChildren();
                    children = this.children;
                    options[this.idField || 'id'] = this.id;
                    if (!this._loaded) {
                        children._data = undefined;
                        method = 'read';
                    }
                    children.one(CHANGE, proxy(this._childrenLoaded, this));
                    if (this._matchFilter) {
                        options.filter = {
                            field: '_matchFilter',
                            operator: 'eq',
                            value: true
                        };
                    }
                    promise = children[method](options);
                } else {
                    this.loaded(true);
                }
                return promise || $.Deferred().resolve().promise();
            },
            parentNode: function () {
                var array = this.parent();
                return array.parent();
            },
            loaded: function (value) {
                if (value !== undefined) {
                    this._loaded = value;
                } else {
                    return this._loaded;
                }
            },
            shouldSerialize: function (field) {
                return Model.fn.shouldSerialize.call(this, field) && field !== 'children' && field !== '_loaded' && field !== 'hasChildren' && field !== '_childrenOptions';
            }
        });
        function dataMethod(name) {
            return function () {
                var data = this._data, result = DataSource.fn[name].apply(this, slice.call(arguments));
                if (this._data != data) {
                    this._attachBubbleHandlers();
                }
                return result;
            };
        }
        var HierarchicalDataSource = DataSource.extend({
            init: function (options) {
                var node = Node.define({ children: options });
                if (options.filter && !options.serverFiltering) {
                    this._hierarchicalFilter = options.filter;
                    options.filter = null;
                }
                DataSource.fn.init.call(this, extend(true, {}, {
                    schema: {
                        modelBase: node,
                        model: node
                    }
                }, options));
                this._attachBubbleHandlers();
            },
            _attachBubbleHandlers: function () {
                var that = this;
                that._data.bind(ERROR, function (e) {
                    that.trigger(ERROR, e);
                });
            },
            read: function (data) {
                var result = DataSource.fn.read.call(this, data);
                if (this._hierarchicalFilter) {
                    if (this._data && this._data.length > 0) {
                        this.filter(this._hierarchicalFilter);
                    } else {
                        this.options.filter = this._hierarchicalFilter;
                        this._filter = normalizeFilter(this.options.filter);
                        this._hierarchicalFilter = null;
                    }
                }
                return result;
            },
            remove: function (node) {
                var parentNode = node.parentNode(), dataSource = this, result;
                if (parentNode && parentNode._initChildren) {
                    dataSource = parentNode.children;
                }
                result = DataSource.fn.remove.call(dataSource, node);
                if (parentNode && !dataSource.data().length) {
                    parentNode.hasChildren = false;
                }
                return result;
            },
            success: dataMethod('success'),
            data: dataMethod('data'),
            insert: function (index, model) {
                var parentNode = this.parent();
                if (parentNode && parentNode._initChildren) {
                    parentNode.hasChildren = true;
                    parentNode._initChildren();
                }
                return DataSource.fn.insert.call(this, index, model);
            },
            filter: function (val) {
                if (val === undefined) {
                    return this._filter;
                }
                if (!this.options.serverFiltering && this._markHierarchicalQuery(val)) {
                    val = {
                        logic: 'or',
                        filters: [
                            val,
                            {
                                field: '_matchFilter',
                                operator: 'equals',
                                value: true
                            }
                        ]
                    };
                }
                this.trigger('reset');
                this._query({
                    filter: val,
                    page: 1
                });
            },
            _markHierarchicalQuery: function (expressions) {
                var compiled;
                var predicate;
                var fields;
                var operators;
                var filter;
                expressions = normalizeFilter(expressions);
                if (!expressions || expressions.filters.length === 0) {
                    this._updateHierarchicalFilter(function () {
                        return true;
                    });
                    return false;
                }
                compiled = Query.filterExpr(expressions);
                fields = compiled.fields;
                operators = compiled.operators;
                predicate = filter = new Function('d, __f, __o', 'return ' + compiled.expression);
                if (fields.length || operators.length) {
                    filter = function (d) {
                        return predicate(d, fields, operators);
                    };
                }
                this._updateHierarchicalFilter(filter);
                return true;
            },
            _updateHierarchicalFilter: function (filter) {
                var current;
                var data = this._data;
                var result = false;
                for (var idx = 0; idx < data.length; idx++) {
                    current = data[idx];
                    if (current.hasChildren) {
                        current._matchFilter = current.children._updateHierarchicalFilter(filter);
                        if (!current._matchFilter) {
                            current._matchFilter = filter(current);
                        }
                    } else {
                        current._matchFilter = filter(current);
                    }
                    if (current._matchFilter) {
                        result = true;
                    }
                }
                return result;
            },
            _find: function (method, value) {
                var idx, length, node, children;
                var data = this._data;
                if (!data) {
                    return;
                }
                node = DataSource.fn[method].call(this, value);
                if (node) {
                    return node;
                }
                data = this._flatData(this._data);
                for (idx = 0, length = data.length; idx < length; idx++) {
                    children = data[idx].children;
                    if (!(children instanceof HierarchicalDataSource)) {
                        continue;
                    }
                    node = children[method](value);
                    if (node) {
                        return node;
                    }
                }
            },
            get: function (id) {
                return this._find('get', id);
            },
            getByUid: function (uid) {
                return this._find('getByUid', uid);
            }
        });
        function inferList(list, fields) {
            var items = $(list).children(), idx, length, data = [], record, textField = fields[0].field, urlField = fields[1] && fields[1].field, spriteCssClassField = fields[2] && fields[2].field, imageUrlField = fields[3] && fields[3].field, item, id, textChild, className, children;
            function elements(collection, tagName) {
                return collection.filter(tagName).add(collection.find(tagName));
            }
            for (idx = 0, length = items.length; idx < length; idx++) {
                record = { _loaded: true };
                item = items.eq(idx);
                textChild = item[0].firstChild;
                children = item.children();
                list = children.filter('ul');
                children = children.filter(':not(ul)');
                id = item.attr('data-id');
                if (id) {
                    record.id = id;
                }
                if (textChild) {
                    record[textField] = textChild.nodeType == 3 ? textChild.nodeValue : children.text();
                }
                if (urlField) {
                    record[urlField] = elements(children, 'a').attr('href');
                }
                if (imageUrlField) {
                    record[imageUrlField] = elements(children, 'img').attr('src');
                }
                if (spriteCssClassField) {
                    className = elements(children, '.k-sprite').prop('className');
                    record[spriteCssClassField] = className && $.trim(className.replace('k-sprite', ''));
                }
                if (list.length) {
                    record.items = inferList(list.eq(0), fields);
                }
                if (item.attr('data-hasChildren') == 'true') {
                    record.hasChildren = true;
                }
                data.push(record);
            }
            return data;
        }
        HierarchicalDataSource.create = function (options) {
            options = options && options.push ? { data: options } : options;
            var dataSource = options || {}, data = dataSource.data, fields = dataSource.fields, list = dataSource.list;
            if (data && data._dataSource) {
                return data._dataSource;
            }
            if (!data && fields && !dataSource.transport) {
                if (list) {
                    data = inferList(list, fields);
                }
            }
            dataSource.data = data;
            return dataSource instanceof HierarchicalDataSource ? dataSource : new HierarchicalDataSource(dataSource);
        };
        var Buffer = kendo.Observable.extend({
            init: function (dataSource, viewSize, disablePrefetch) {
                kendo.Observable.fn.init.call(this);
                this._prefetching = false;
                this.dataSource = dataSource;
                this.prefetch = !disablePrefetch;
                var buffer = this;
                dataSource.bind('change', function () {
                    buffer._change();
                });
                dataSource.bind('reset', function () {
                    buffer._reset();
                });
                this._syncWithDataSource();
                this.setViewSize(viewSize);
            },
            setViewSize: function (viewSize) {
                this.viewSize = viewSize;
                this._recalculate();
            },
            at: function (index) {
                var pageSize = this.pageSize, itemPresent = true;
                if (index >= this.total()) {
                    this.trigger('endreached', { index: index });
                    return null;
                }
                if (!this.useRanges) {
                    return this.dataSource.view()[index];
                }
                if (this.useRanges) {
                    if (index < this.dataOffset || index >= this.skip + pageSize) {
                        itemPresent = this.range(Math.floor(index / pageSize) * pageSize);
                    }
                    if (index === this.prefetchThreshold) {
                        this._prefetch();
                    }
                    if (index === this.midPageThreshold) {
                        this.range(this.nextMidRange, true);
                    } else if (index === this.nextPageThreshold) {
                        this.range(this.nextFullRange);
                    } else if (index === this.pullBackThreshold) {
                        if (this.offset === this.skip) {
                            this.range(this.previousMidRange);
                        } else {
                            this.range(this.previousFullRange);
                        }
                    }
                    if (itemPresent) {
                        return this.dataSource.at(index - this.dataOffset);
                    } else {
                        this.trigger('endreached', { index: index });
                        return null;
                    }
                }
            },
            indexOf: function (item) {
                return this.dataSource.data().indexOf(item) + this.dataOffset;
            },
            total: function () {
                return parseInt(this.dataSource.total(), 10);
            },
            next: function () {
                var buffer = this, pageSize = buffer.pageSize, offset = buffer.skip - buffer.viewSize + pageSize, pageSkip = math.max(math.floor(offset / pageSize), 0) * pageSize;
                this.offset = offset;
                this.dataSource.prefetch(pageSkip, pageSize, function () {
                    buffer._goToRange(offset, true);
                });
            },
            range: function (offset, nextRange) {
                if (this.offset === offset) {
                    return true;
                }
                var buffer = this, pageSize = this.pageSize, pageSkip = math.max(math.floor(offset / pageSize), 0) * pageSize, dataSource = this.dataSource;
                if (nextRange) {
                    pageSkip += pageSize;
                }
                if (dataSource.inRange(offset, pageSize)) {
                    this.offset = offset;
                    this._recalculate();
                    this._goToRange(offset);
                    return true;
                } else if (this.prefetch) {
                    dataSource.prefetch(pageSkip, pageSize, function () {
                        buffer.offset = offset;
                        buffer._recalculate();
                        buffer._goToRange(offset, true);
                    });
                    return false;
                }
                return true;
            },
            syncDataSource: function () {
                var offset = this.offset;
                this.offset = null;
                this.range(offset);
            },
            destroy: function () {
                this.unbind();
            },
            _prefetch: function () {
                var buffer = this, pageSize = this.pageSize, prefetchOffset = this.skip + pageSize, dataSource = this.dataSource;
                if (!dataSource.inRange(prefetchOffset, pageSize) && !this._prefetching && this.prefetch) {
                    this._prefetching = true;
                    this.trigger('prefetching', {
                        skip: prefetchOffset,
                        take: pageSize
                    });
                    dataSource.prefetch(prefetchOffset, pageSize, function () {
                        buffer._prefetching = false;
                        buffer.trigger('prefetched', {
                            skip: prefetchOffset,
                            take: pageSize
                        });
                    });
                }
            },
            _goToRange: function (offset, expanding) {
                if (this.offset !== offset) {
                    return;
                }
                this.dataOffset = offset;
                this._expanding = expanding;
                this.dataSource.range(offset, this.pageSize);
                this.dataSource.enableRequestsInProgress();
            },
            _reset: function () {
                this._syncPending = true;
            },
            _change: function () {
                var dataSource = this.dataSource;
                this.length = this.useRanges ? dataSource.lastRange().end : dataSource.view().length;
                if (this._syncPending) {
                    this._syncWithDataSource();
                    this._recalculate();
                    this._syncPending = false;
                    this.trigger('reset', { offset: this.offset });
                }
                this.trigger('resize');
                if (this._expanding) {
                    this.trigger('expand');
                }
                delete this._expanding;
            },
            _syncWithDataSource: function () {
                var dataSource = this.dataSource;
                this._firstItemUid = dataSource.firstItemUid();
                this.dataOffset = this.offset = dataSource.skip() || 0;
                this.pageSize = dataSource.pageSize();
                this.useRanges = dataSource.options.serverPaging;
            },
            _recalculate: function () {
                var pageSize = this.pageSize, offset = this.offset, viewSize = this.viewSize, skip = Math.ceil(offset / pageSize) * pageSize;
                this.skip = skip;
                this.midPageThreshold = skip + pageSize - 1;
                this.nextPageThreshold = skip + viewSize - 1;
                this.prefetchThreshold = skip + Math.floor(pageSize / 3 * 2);
                this.pullBackThreshold = this.offset - 1;
                this.nextMidRange = skip + pageSize - viewSize;
                this.nextFullRange = skip;
                this.previousMidRange = offset - viewSize;
                this.previousFullRange = skip - pageSize;
            }
        });
        var BatchBuffer = kendo.Observable.extend({
            init: function (dataSource, batchSize) {
                var batchBuffer = this;
                kendo.Observable.fn.init.call(batchBuffer);
                this.dataSource = dataSource;
                this.batchSize = batchSize;
                this._total = 0;
                this.buffer = new Buffer(dataSource, batchSize * 3);
                this.buffer.bind({
                    'endreached': function (e) {
                        batchBuffer.trigger('endreached', { index: e.index });
                    },
                    'prefetching': function (e) {
                        batchBuffer.trigger('prefetching', {
                            skip: e.skip,
                            take: e.take
                        });
                    },
                    'prefetched': function (e) {
                        batchBuffer.trigger('prefetched', {
                            skip: e.skip,
                            take: e.take
                        });
                    },
                    'reset': function () {
                        batchBuffer._total = 0;
                        batchBuffer.trigger('reset');
                    },
                    'resize': function () {
                        batchBuffer._total = Math.ceil(this.length / batchBuffer.batchSize);
                        batchBuffer.trigger('resize', {
                            total: batchBuffer.total(),
                            offset: this.offset
                        });
                    }
                });
            },
            syncDataSource: function () {
                this.buffer.syncDataSource();
            },
            at: function (index) {
                var buffer = this.buffer, skip = index * this.batchSize, take = this.batchSize, view = [], item;
                if (buffer.offset > skip) {
                    buffer.at(buffer.offset - 1);
                }
                for (var i = 0; i < take; i++) {
                    item = buffer.at(skip + i);
                    if (item === null) {
                        break;
                    }
                    view.push(item);
                }
                return view;
            },
            total: function () {
                return this._total;
            },
            destroy: function () {
                this.buffer.destroy();
                this.unbind();
            }
        });
        extend(true, kendo.data, {
            readers: { json: DataReader },
            Query: Query,
            DataSource: DataSource,
            HierarchicalDataSource: HierarchicalDataSource,
            Node: Node,
            ObservableObject: ObservableObject,
            ObservableArray: ObservableArray,
            LazyObservableArray: LazyObservableArray,
            LocalTransport: LocalTransport,
            RemoteTransport: RemoteTransport,
            Cache: Cache,
            DataReader: DataReader,
            Model: Model,
            Buffer: Buffer,
            BatchBuffer: BatchBuffer
        });
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.data.signalr', ['kendo.data'], f);
}(function () {
    var __meta__ = {
        id: 'data.signalr',
        name: 'SignalR',
        category: 'framework',
        depends: ['data'],
        hidden: true
    };
    (function ($) {
        var kendo = window.kendo;
        var isFunction = kendo.isFunction;
        function isJQueryPromise(promise) {
            return promise && isFunction(promise.done) && isFunction(promise.fail);
        }
        function isNativePromise(promise) {
            return promise && isFunction(promise.then) && isFunction(promise.catch);
        }
        var transport = kendo.data.RemoteTransport.extend({
            init: function (options) {
                var signalr = options && options.signalr ? options.signalr : {};
                var promise = signalr.promise;
                if (!promise) {
                    throw new Error('The "promise" option must be set.');
                }
                if (!isJQueryPromise(promise) && !isNativePromise(promise)) {
                    throw new Error('The "promise" option must be a Promise.');
                }
                this.promise = promise;
                var hub = signalr.hub;
                if (!hub) {
                    throw new Error('The "hub" option must be set.');
                }
                if (typeof hub.on != 'function' || typeof hub.invoke != 'function') {
                    throw new Error('The "hub" option is not a valid SignalR hub proxy.');
                }
                this.hub = hub;
                kendo.data.RemoteTransport.fn.init.call(this, options);
            },
            push: function (callbacks) {
                var client = this.options.signalr.client || {};
                if (client.create) {
                    this.hub.on(client.create, callbacks.pushCreate);
                }
                if (client.update) {
                    this.hub.on(client.update, callbacks.pushUpdate);
                }
                if (client.destroy) {
                    this.hub.on(client.destroy, callbacks.pushDestroy);
                }
            },
            _crud: function (options, type) {
                var hub = this.hub;
                var promise = this.promise;
                var server = this.options.signalr.server;
                if (!server || !server[type]) {
                    throw new Error(kendo.format('The "server.{0}" option must be set.', type));
                }
                var args = [server[type]];
                var data = this.parameterMap(options.data, type);
                if (!$.isEmptyObject(data)) {
                    args.push(data);
                }
                if (isJQueryPromise(promise)) {
                    promise.done(function () {
                        hub.invoke.apply(hub, args).done(options.success).fail(options.error);
                    });
                } else if (isNativePromise(promise)) {
                    promise.then(function () {
                        hub.invoke.apply(hub, args).then(options.success).catch(options.error);
                    });
                }
            },
            read: function (options) {
                this._crud(options, 'read');
            },
            create: function (options) {
                this._crud(options, 'create');
            },
            update: function (options) {
                this._crud(options, 'update');
            },
            destroy: function (options) {
                this._crud(options, 'destroy');
            }
        });
        $.extend(true, kendo.data, { transports: { signalr: transport } });
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.binder', [
        'kendo.core',
        'kendo.data'
    ], f);
}(function () {
    var __meta__ = {
        id: 'binder',
        name: 'MVVM',
        category: 'framework',
        description: 'Model View ViewModel (MVVM) is a design pattern which helps developers separate the Model (the data) from the View (the UI).',
        depends: [
            'core',
            'data'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, Observable = kendo.Observable, ObservableObject = kendo.data.ObservableObject, ObservableArray = kendo.data.ObservableArray, toString = {}.toString, binders = {}, Class = kendo.Class, proxy = $.proxy, VALUE = 'value', SOURCE = 'source', EVENTS = 'events', CHECKED = 'checked', CSS = 'css', deleteExpando = true, FUNCTION = 'function', CHANGE = 'change';
        (function () {
            var a = document.createElement('a');
            try {
                delete a.test;
            } catch (e) {
                deleteExpando = false;
            }
        }());
        var Binding = Observable.extend({
            init: function (parents, path) {
                var that = this;
                Observable.fn.init.call(that);
                that.source = parents[0];
                that.parents = parents;
                that.path = path;
                that.dependencies = {};
                that.dependencies[path] = true;
                that.observable = that.source instanceof Observable;
                that._access = function (e) {
                    that.dependencies[e.field] = true;
                };
                if (that.observable) {
                    that._change = function (e) {
                        that.change(e);
                    };
                    that.source.bind(CHANGE, that._change);
                }
            },
            _parents: function () {
                var parents = this.parents;
                var value = this.get();
                if (value && typeof value.parent == 'function') {
                    var parent = value.parent();
                    if ($.inArray(parent, parents) < 0) {
                        parents = [parent].concat(parents);
                    }
                }
                return parents;
            },
            change: function (e) {
                var dependency, ch, field = e.field, that = this;
                if (that.path === 'this') {
                    that.trigger(CHANGE, e);
                } else {
                    for (dependency in that.dependencies) {
                        if (dependency.indexOf(field) === 0) {
                            ch = dependency.charAt(field.length);
                            if (!ch || ch === '.' || ch === '[') {
                                that.trigger(CHANGE, e);
                                break;
                            }
                        }
                    }
                }
            },
            start: function (source) {
                source.bind('get', this._access);
            },
            stop: function (source) {
                source.unbind('get', this._access);
            },
            get: function () {
                var that = this, source = that.source, index = 0, path = that.path, result = source;
                if (!that.observable) {
                    return result;
                }
                that.start(that.source);
                result = source.get(path);
                while (result === undefined && source) {
                    source = that.parents[++index];
                    if (source instanceof ObservableObject) {
                        result = source.get(path);
                    }
                }
                if (result === undefined) {
                    source = that.source;
                    while (result === undefined && source) {
                        source = source.parent();
                        if (source instanceof ObservableObject) {
                            result = source.get(path);
                        }
                    }
                }
                if (typeof result === 'function') {
                    index = path.lastIndexOf('.');
                    if (index > 0) {
                        source = source.get(path.substring(0, index));
                    }
                    that.start(source);
                    if (source !== that.source) {
                        result = result.call(source, that.source);
                    } else {
                        result = result.call(source);
                    }
                    that.stop(source);
                }
                if (source && source !== that.source) {
                    that.currentSource = source;
                    source.unbind(CHANGE, that._change).bind(CHANGE, that._change);
                }
                that.stop(that.source);
                return result;
            },
            set: function (value) {
                var source = this.currentSource || this.source;
                var field = kendo.getter(this.path)(source);
                if (typeof field === 'function') {
                    if (source !== this.source) {
                        field.call(source, this.source, value);
                    } else {
                        field.call(source, value);
                    }
                } else {
                    source.set(this.path, value);
                }
            },
            destroy: function () {
                if (this.observable) {
                    this.source.unbind(CHANGE, this._change);
                    if (this.currentSource) {
                        this.currentSource.unbind(CHANGE, this._change);
                    }
                }
                this.unbind();
            }
        });
        var EventBinding = Binding.extend({
            get: function () {
                var source = this.source, path = this.path, index = 0, handler;
                handler = source.get(path);
                while (!handler && source) {
                    source = this.parents[++index];
                    if (source instanceof ObservableObject) {
                        handler = source.get(path);
                    }
                }
                return proxy(handler, source);
            }
        });
        var TemplateBinding = Binding.extend({
            init: function (source, path, template) {
                var that = this;
                Binding.fn.init.call(that, source, path);
                that.template = template;
            },
            render: function (value) {
                var html;
                this.start(this.source);
                html = kendo.render(this.template, value);
                this.stop(this.source);
                return html;
            }
        });
        var Binder = Class.extend({
            init: function (element, bindings, options) {
                this.element = element;
                this.bindings = bindings;
                this.options = options;
            },
            bind: function (binding, attribute) {
                var that = this;
                binding = attribute ? binding[attribute] : binding;
                binding.bind(CHANGE, function (e) {
                    that.refresh(attribute || e);
                });
                that.refresh(attribute);
            },
            destroy: function () {
            }
        });
        var TypedBinder = Binder.extend({
            dataType: function () {
                var dataType = this.element.getAttribute('data-type') || this.element.type || 'text';
                return dataType.toLowerCase();
            },
            parsedValue: function () {
                return this._parseValue(this.element.value, this.dataType());
            },
            _parseValue: function (value, dataType) {
                if (dataType == 'date') {
                    value = kendo.parseDate(value, 'yyyy-MM-dd');
                } else if (dataType == 'datetime-local') {
                    value = kendo.parseDate(value, [
                        'yyyy-MM-ddTHH:mm:ss',
                        'yyyy-MM-ddTHH:mm'
                    ]);
                } else if (dataType == 'number') {
                    value = kendo.parseFloat(value);
                } else if (dataType == 'boolean') {
                    value = value.toLowerCase();
                    if (kendo.parseFloat(value) !== null) {
                        value = Boolean(kendo.parseFloat(value));
                    } else {
                        value = value.toLowerCase() === 'true';
                    }
                }
                return value;
            }
        });
        binders.attr = Binder.extend({
            refresh: function (key) {
                this.element.setAttribute(key, this.bindings.attr[key].get());
            }
        });
        binders.css = Binder.extend({
            init: function (element, bindings, options) {
                Binder.fn.init.call(this, element, bindings, options);
                this.classes = {};
            },
            refresh: function (className) {
                var element = $(this.element), binding = this.bindings.css[className], hasClass = this.classes[className] = binding.get();
                if (hasClass) {
                    element.addClass(className);
                } else {
                    element.removeClass(className);
                }
            }
        });
        binders.style = Binder.extend({
            refresh: function (key) {
                this.element.style[key] = this.bindings.style[key].get() || '';
            }
        });
        binders.enabled = Binder.extend({
            refresh: function () {
                if (this.bindings.enabled.get()) {
                    this.element.removeAttribute('disabled');
                } else {
                    this.element.setAttribute('disabled', 'disabled');
                }
            }
        });
        binders.readonly = Binder.extend({
            refresh: function () {
                if (this.bindings.readonly.get()) {
                    this.element.setAttribute('readonly', 'readonly');
                } else {
                    this.element.removeAttribute('readonly');
                }
            }
        });
        binders.disabled = Binder.extend({
            refresh: function () {
                if (this.bindings.disabled.get()) {
                    this.element.setAttribute('disabled', 'disabled');
                } else {
                    this.element.removeAttribute('disabled');
                }
            }
        });
        binders.events = Binder.extend({
            init: function (element, bindings, options) {
                Binder.fn.init.call(this, element, bindings, options);
                this.handlers = {};
            },
            refresh: function (key) {
                var element = $(this.element), binding = this.bindings.events[key], handler = this.handlers[key];
                if (handler) {
                    element.off(key, handler);
                }
                handler = this.handlers[key] = binding.get();
                element.on(key, binding.source, handler);
            },
            destroy: function () {
                var element = $(this.element), handler;
                for (handler in this.handlers) {
                    element.off(handler, this.handlers[handler]);
                }
            }
        });
        binders.text = Binder.extend({
            refresh: function () {
                var text = this.bindings.text.get();
                var dataFormat = this.element.getAttribute('data-format') || '';
                if (text == null) {
                    text = '';
                }
                $(this.element).text(kendo.toString(text, dataFormat));
            }
        });
        binders.visible = Binder.extend({
            refresh: function () {
                if (this.bindings.visible.get()) {
                    this.element.style.display = '';
                } else {
                    this.element.style.display = 'none';
                }
            }
        });
        binders.invisible = Binder.extend({
            refresh: function () {
                if (!this.bindings.invisible.get()) {
                    this.element.style.display = '';
                } else {
                    this.element.style.display = 'none';
                }
            }
        });
        binders.html = Binder.extend({
            refresh: function () {
                this.element.innerHTML = this.bindings.html.get();
            }
        });
        binders.value = TypedBinder.extend({
            init: function (element, bindings, options) {
                TypedBinder.fn.init.call(this, element, bindings, options);
                this._change = proxy(this.change, this);
                this.eventName = options.valueUpdate || CHANGE;
                $(this.element).on(this.eventName, this._change);
                this._initChange = false;
            },
            change: function () {
                this._initChange = this.eventName != CHANGE;
                this.bindings[VALUE].set(this.parsedValue());
                this._initChange = false;
            },
            refresh: function () {
                if (!this._initChange) {
                    var value = this.bindings[VALUE].get();
                    if (value == null) {
                        value = '';
                    }
                    var type = this.dataType();
                    if (type == 'date') {
                        value = kendo.toString(value, 'yyyy-MM-dd');
                    } else if (type == 'datetime-local') {
                        value = kendo.toString(value, 'yyyy-MM-ddTHH:mm:ss');
                    }
                    this.element.value = value;
                }
                this._initChange = false;
            },
            destroy: function () {
                $(this.element).off(this.eventName, this._change);
            }
        });
        binders.source = Binder.extend({
            init: function (element, bindings, options) {
                Binder.fn.init.call(this, element, bindings, options);
                var source = this.bindings.source.get();
                if (source instanceof kendo.data.DataSource && options.autoBind !== false) {
                    source.fetch();
                }
            },
            refresh: function (e) {
                var that = this, source = that.bindings.source.get();
                if (source instanceof ObservableArray || source instanceof kendo.data.DataSource) {
                    e = e || {};
                    if (e.action == 'add') {
                        that.add(e.index, e.items);
                    } else if (e.action == 'remove') {
                        that.remove(e.index, e.items);
                    } else if (e.action != 'itemchange') {
                        that.render();
                    }
                } else {
                    that.render();
                }
            },
            container: function () {
                var element = this.element;
                if (element.nodeName.toLowerCase() == 'table') {
                    if (!element.tBodies[0]) {
                        element.appendChild(document.createElement('tbody'));
                    }
                    element = element.tBodies[0];
                }
                return element;
            },
            template: function () {
                var options = this.options, template = options.template, nodeName = this.container().nodeName.toLowerCase();
                if (!template) {
                    if (nodeName == 'select') {
                        if (options.valueField || options.textField) {
                            template = kendo.format('<option value="#:{0}#">#:{1}#</option>', options.valueField || options.textField, options.textField || options.valueField);
                        } else {
                            template = '<option>#:data#</option>';
                        }
                    } else if (nodeName == 'tbody') {
                        template = '<tr><td>#:data#</td></tr>';
                    } else if (nodeName == 'ul' || nodeName == 'ol') {
                        template = '<li>#:data#</li>';
                    } else {
                        template = '#:data#';
                    }
                    template = kendo.template(template);
                }
                return template;
            },
            add: function (index, items) {
                var element = this.container(), parents, idx, length, child, clone = element.cloneNode(false), reference = element.children[index];
                $(clone).html(kendo.render(this.template(), items));
                if (clone.children.length) {
                    parents = this.bindings.source._parents();
                    for (idx = 0, length = items.length; idx < length; idx++) {
                        child = clone.children[0];
                        element.insertBefore(child, reference || null);
                        bindElement(child, items[idx], this.options.roles, [items[idx]].concat(parents));
                    }
                }
            },
            remove: function (index, items) {
                var idx, element = this.container();
                for (idx = 0; idx < items.length; idx++) {
                    var child = element.children[index];
                    unbindElementTree(child, true);
                    if (child.parentNode == element) {
                        element.removeChild(child);
                    }
                }
            },
            render: function () {
                var source = this.bindings.source.get(), parents, idx, length, element = this.container(), template = this.template();
                if (source == null) {
                    return;
                }
                if (source instanceof kendo.data.DataSource) {
                    source = source.view();
                }
                if (!(source instanceof ObservableArray) && toString.call(source) !== '[object Array]') {
                    source = [source];
                }
                if (this.bindings.template) {
                    unbindElementChildren(element, true);
                    $(element).html(this.bindings.template.render(source));
                    if (element.children.length) {
                        parents = this.bindings.source._parents();
                        for (idx = 0, length = source.length; idx < length; idx++) {
                            bindElement(element.children[idx], source[idx], this.options.roles, [source[idx]].concat(parents));
                        }
                    }
                } else {
                    $(element).html(kendo.render(template, source));
                }
            }
        });
        binders.input = {
            checked: TypedBinder.extend({
                init: function (element, bindings, options) {
                    TypedBinder.fn.init.call(this, element, bindings, options);
                    this._change = proxy(this.change, this);
                    $(this.element).change(this._change);
                },
                change: function () {
                    var element = this.element;
                    var value = this.value();
                    if (element.type == 'radio') {
                        value = this.parsedValue();
                        this.bindings[CHECKED].set(value);
                    } else if (element.type == 'checkbox') {
                        var source = this.bindings[CHECKED].get();
                        var index;
                        if (source instanceof ObservableArray) {
                            value = this.parsedValue();
                            if (value instanceof Date) {
                                for (var i = 0; i < source.length; i++) {
                                    if (source[i] instanceof Date && +source[i] === +value) {
                                        index = i;
                                        break;
                                    }
                                }
                            } else {
                                index = source.indexOf(value);
                            }
                            if (index > -1) {
                                source.splice(index, 1);
                            } else {
                                source.push(value);
                            }
                        } else {
                            this.bindings[CHECKED].set(value);
                        }
                    }
                },
                refresh: function () {
                    var value = this.bindings[CHECKED].get(), source = value, type = this.dataType(), element = this.element;
                    if (element.type == 'checkbox') {
                        if (source instanceof ObservableArray) {
                            var index = -1;
                            value = this.parsedValue();
                            if (value instanceof Date) {
                                for (var i = 0; i < source.length; i++) {
                                    if (source[i] instanceof Date && +source[i] === +value) {
                                        index = i;
                                        break;
                                    }
                                }
                            } else {
                                index = source.indexOf(value);
                            }
                            element.checked = index >= 0;
                        } else {
                            element.checked = source;
                        }
                    } else if (element.type == 'radio') {
                        if (type == 'date') {
                            value = kendo.toString(value, 'yyyy-MM-dd');
                        } else if (type == 'datetime-local') {
                            value = kendo.toString(value, 'yyyy-MM-ddTHH:mm:ss');
                        }
                        if (value !== null && typeof value !== 'undefined' && element.value === value.toString()) {
                            element.checked = true;
                        } else {
                            element.checked = false;
                        }
                    }
                },
                value: function () {
                    var element = this.element, value = element.value;
                    if (element.type == 'checkbox') {
                        value = element.checked;
                    }
                    return value;
                },
                destroy: function () {
                    $(this.element).off(CHANGE, this._change);
                }
            })
        };
        binders.select = {
            source: binders.source.extend({
                refresh: function (e) {
                    var that = this, source = that.bindings.source.get();
                    if (source instanceof ObservableArray || source instanceof kendo.data.DataSource) {
                        e = e || {};
                        if (e.action == 'add') {
                            that.add(e.index, e.items);
                        } else if (e.action == 'remove') {
                            that.remove(e.index, e.items);
                        } else if (e.action == 'itemchange' || e.action === undefined) {
                            that.render();
                            if (that.bindings.value) {
                                if (that.bindings.value) {
                                    var val = retrievePrimitiveValues(that.bindings.value.get(), $(that.element).data('valueField'));
                                    if (val === null) {
                                        that.element.selectedIndex = -1;
                                    } else {
                                        that.element.value = val;
                                    }
                                }
                            }
                        }
                    } else {
                        that.render();
                    }
                }
            }),
            value: TypedBinder.extend({
                init: function (target, bindings, options) {
                    TypedBinder.fn.init.call(this, target, bindings, options);
                    this._change = proxy(this.change, this);
                    $(this.element).change(this._change);
                },
                parsedValue: function () {
                    var dataType = this.dataType();
                    var values = [];
                    var value, option, idx, length;
                    for (idx = 0, length = this.element.options.length; idx < length; idx++) {
                        option = this.element.options[idx];
                        if (option.selected) {
                            value = option.attributes.value;
                            if (value && value.specified) {
                                value = option.value;
                            } else {
                                value = option.text;
                            }
                            values.push(this._parseValue(value, dataType));
                        }
                    }
                    return values;
                },
                change: function () {
                    var values = [], element = this.element, source, field = this.options.valueField || this.options.textField, valuePrimitive = this.options.valuePrimitive, option, valueIndex, value, idx, length;
                    for (idx = 0, length = element.options.length; idx < length; idx++) {
                        option = element.options[idx];
                        if (option.selected) {
                            value = option.attributes.value;
                            if (value && value.specified) {
                                value = option.value;
                            } else {
                                value = option.text;
                            }
                            if (field) {
                                values.push(value);
                            } else {
                                values.push(this._parseValue(value, this.dataType()));
                            }
                        }
                    }
                    if (field) {
                        source = this.bindings.source.get();
                        if (source instanceof kendo.data.DataSource) {
                            source = source.view();
                        }
                        for (valueIndex = 0; valueIndex < values.length; valueIndex++) {
                            for (idx = 0, length = source.length; idx < length; idx++) {
                                var sourceValue = source[idx].get(field);
                                var match = String(sourceValue) === values[valueIndex];
                                if (match) {
                                    values[valueIndex] = source[idx];
                                    break;
                                }
                            }
                        }
                    }
                    value = this.bindings[VALUE].get();
                    if (value instanceof ObservableArray) {
                        value.splice.apply(value, [
                            0,
                            value.length
                        ].concat(values));
                    } else if (!valuePrimitive && (value instanceof ObservableObject || value === null || value === undefined || !field)) {
                        this.bindings[VALUE].set(values[0]);
                    } else {
                        this.bindings[VALUE].set(values[0].get(field));
                    }
                },
                refresh: function () {
                    var optionIndex, element = this.element, options = element.options, value = this.bindings[VALUE].get(), values = value, field = this.options.valueField || this.options.textField, found = false, type = this.dataType(), optionValue;
                    if (!(values instanceof ObservableArray)) {
                        values = new ObservableArray([value]);
                    }
                    element.selectedIndex = -1;
                    for (var valueIndex = 0; valueIndex < values.length; valueIndex++) {
                        value = values[valueIndex];
                        if (field && value instanceof ObservableObject) {
                            value = value.get(field);
                        }
                        if (type == 'date') {
                            value = kendo.toString(values[valueIndex], 'yyyy-MM-dd');
                        } else if (type == 'datetime-local') {
                            value = kendo.toString(values[valueIndex], 'yyyy-MM-ddTHH:mm:ss');
                        }
                        for (optionIndex = 0; optionIndex < options.length; optionIndex++) {
                            optionValue = options[optionIndex].value;
                            if (optionValue === '' && value !== '') {
                                optionValue = options[optionIndex].text;
                            }
                            if (value != null && optionValue == value.toString()) {
                                options[optionIndex].selected = true;
                                found = true;
                            }
                        }
                    }
                },
                destroy: function () {
                    $(this.element).off(CHANGE, this._change);
                }
            })
        };
        function dataSourceBinding(bindingName, fieldName, setter) {
            return Binder.extend({
                init: function (widget, bindings, options) {
                    var that = this;
                    Binder.fn.init.call(that, widget.element[0], bindings, options);
                    that.widget = widget;
                    that._dataBinding = proxy(that.dataBinding, that);
                    that._dataBound = proxy(that.dataBound, that);
                    that._itemChange = proxy(that.itemChange, that);
                },
                itemChange: function (e) {
                    bindElement(e.item[0], e.data, this._ns(e.ns), [e.data].concat(this.bindings[bindingName]._parents()));
                },
                dataBinding: function (e) {
                    var idx, length, widget = this.widget, items = e.removedItems || widget.items();
                    for (idx = 0, length = items.length; idx < length; idx++) {
                        unbindElementTree(items[idx], false);
                    }
                },
                _ns: function (ns) {
                    ns = ns || kendo.ui;
                    var all = [
                        kendo.ui,
                        kendo.dataviz.ui,
                        kendo.mobile.ui
                    ];
                    all.splice($.inArray(ns, all), 1);
                    all.unshift(ns);
                    return kendo.rolesFromNamespaces(all);
                },
                dataBound: function (e) {
                    var idx, length, widget = this.widget, items = e.addedItems || widget.items(), dataSource = widget[fieldName], view, parents, hds = kendo.data.HierarchicalDataSource;
                    if (hds && dataSource instanceof hds) {
                        return;
                    }
                    if (items.length) {
                        view = e.addedDataItems || dataSource.flatView();
                        parents = this.bindings[bindingName]._parents();
                        for (idx = 0, length = view.length; idx < length; idx++) {
                            if (items[idx]) {
                                bindElement(items[idx], view[idx], this._ns(e.ns), [view[idx]].concat(parents));
                            }
                        }
                    }
                },
                refresh: function (e) {
                    var that = this, source, widget = that.widget, select, multiselect, dropdowntree;
                    e = e || {};
                    if (!e.action) {
                        that.destroy();
                        widget.bind('dataBinding', that._dataBinding);
                        widget.bind('dataBound', that._dataBound);
                        widget.bind('itemChange', that._itemChange);
                        source = that.bindings[bindingName].get();
                        if (widget[fieldName] instanceof kendo.data.DataSource && widget[fieldName] != source) {
                            if (source instanceof kendo.data.DataSource) {
                                widget[setter](source);
                            } else if (source && source._dataSource) {
                                widget[setter](source._dataSource);
                            } else {
                                select = kendo.ui.Select && widget instanceof kendo.ui.Select;
                                multiselect = kendo.ui.MultiSelect && widget instanceof kendo.ui.MultiSelect;
                                dropdowntree = kendo.ui.DropDownTree && widget instanceof kendo.ui.DropDownTree;
                                if (!dropdowntree) {
                                    widget[fieldName].data(source);
                                } else {
                                    widget.treeview[fieldName].data(source);
                                }
                                if (that.bindings.value && (select || multiselect)) {
                                    widget.value(retrievePrimitiveValues(that.bindings.value.get(), widget.options.dataValueField));
                                }
                            }
                        }
                    }
                },
                destroy: function () {
                    var widget = this.widget;
                    widget.unbind('dataBinding', this._dataBinding);
                    widget.unbind('dataBound', this._dataBound);
                    widget.unbind('itemChange', this._itemChange);
                }
            });
        }
        binders.widget = {
            events: Binder.extend({
                init: function (widget, bindings, options) {
                    Binder.fn.init.call(this, widget.element[0], bindings, options);
                    this.widget = widget;
                    this.handlers = {};
                },
                refresh: function (key) {
                    var binding = this.bindings.events[key], handler = this.handlers[key];
                    if (handler) {
                        this.widget.unbind(key, handler);
                    }
                    handler = binding.get();
                    this.handlers[key] = function (e) {
                        e.data = binding.source;
                        handler(e);
                        if (e.data === binding.source) {
                            delete e.data;
                        }
                    };
                    this.widget.bind(key, this.handlers[key]);
                },
                destroy: function () {
                    var handler;
                    for (handler in this.handlers) {
                        this.widget.unbind(handler, this.handlers[handler]);
                    }
                }
            }),
            checked: Binder.extend({
                init: function (widget, bindings, options) {
                    Binder.fn.init.call(this, widget.element[0], bindings, options);
                    this.widget = widget;
                    this._change = proxy(this.change, this);
                    this.widget.bind(CHANGE, this._change);
                },
                change: function () {
                    this.bindings[CHECKED].set(this.value());
                },
                refresh: function () {
                    this.widget.check(this.bindings[CHECKED].get() === true);
                },
                value: function () {
                    var element = this.element, value = element.value;
                    if (value == 'on' || value == 'off') {
                        value = element.checked;
                    }
                    return value;
                },
                destroy: function () {
                    this.widget.unbind(CHANGE, this._change);
                }
            }),
            visible: Binder.extend({
                init: function (widget, bindings, options) {
                    Binder.fn.init.call(this, widget.element[0], bindings, options);
                    this.widget = widget;
                },
                refresh: function () {
                    var visible = this.bindings.visible.get();
                    this.widget.wrapper[0].style.display = visible ? '' : 'none';
                }
            }),
            invisible: Binder.extend({
                init: function (widget, bindings, options) {
                    Binder.fn.init.call(this, widget.element[0], bindings, options);
                    this.widget = widget;
                },
                refresh: function () {
                    var invisible = this.bindings.invisible.get();
                    this.widget.wrapper[0].style.display = invisible ? 'none' : '';
                }
            }),
            enabled: Binder.extend({
                init: function (widget, bindings, options) {
                    Binder.fn.init.call(this, widget.element[0], bindings, options);
                    this.widget = widget;
                },
                refresh: function () {
                    if (this.widget.enable) {
                        this.widget.enable(this.bindings.enabled.get());
                    }
                }
            }),
            disabled: Binder.extend({
                init: function (widget, bindings, options) {
                    Binder.fn.init.call(this, widget.element[0], bindings, options);
                    this.widget = widget;
                },
                refresh: function () {
                    if (this.widget.enable) {
                        this.widget.enable(!this.bindings.disabled.get());
                    }
                }
            }),
            source: dataSourceBinding('source', 'dataSource', 'setDataSource'),
            value: Binder.extend({
                init: function (widget, bindings, options) {
                    Binder.fn.init.call(this, widget.element[0], bindings, options);
                    this.widget = widget;
                    this._change = $.proxy(this.change, this);
                    this.widget.first(CHANGE, this._change);
                    var value = this.bindings.value.get();
                    this._valueIsObservableObject = !options.valuePrimitive && (value == null || value instanceof ObservableObject);
                    this._valueIsObservableArray = value instanceof ObservableArray;
                    this._initChange = false;
                },
                _source: function () {
                    var source;
                    if (this.widget.dataItem) {
                        source = this.widget.dataItem();
                        if (source && source instanceof ObservableObject) {
                            return [source];
                        }
                    }
                    if (this.bindings.source) {
                        source = this.bindings.source.get();
                    }
                    if (!source || source instanceof kendo.data.DataSource) {
                        source = this.widget.dataSource.flatView();
                    }
                    return source;
                },
                change: function () {
                    var value = this.widget.value(), field = this.options.dataValueField || this.options.dataTextField, isArray = toString.call(value) === '[object Array]', isObservableObject = this._valueIsObservableObject, valueIndex, valueLength, values = [], sourceItem, sourceValue, idx, length, source;
                    this._initChange = true;
                    if (field) {
                        if (value === '' && (isObservableObject || this.options.valuePrimitive)) {
                            value = null;
                        } else {
                            source = this._source();
                            if (isArray) {
                                valueLength = value.length;
                                values = value.slice(0);
                            }
                            for (idx = 0, length = source.length; idx < length; idx++) {
                                sourceItem = source[idx];
                                sourceValue = sourceItem.get(field);
                                if (isArray) {
                                    for (valueIndex = 0; valueIndex < valueLength; valueIndex++) {
                                        if (sourceValue == values[valueIndex]) {
                                            values[valueIndex] = sourceItem;
                                            break;
                                        }
                                    }
                                } else if (sourceValue == value) {
                                    value = isObservableObject ? sourceItem : sourceValue;
                                    break;
                                }
                            }
                            if (values[0]) {
                                if (this._valueIsObservableArray) {
                                    value = values;
                                } else if (isObservableObject || !field) {
                                    value = values[0];
                                } else {
                                    value = values[0].get(field);
                                }
                            }
                        }
                    }
                    this.bindings.value.set(value);
                    this._initChange = false;
                },
                refresh: function () {
                    if (!this._initChange) {
                        var widget = this.widget;
                        var options = widget.options;
                        var textField = options.dataTextField;
                        var valueField = options.dataValueField || textField;
                        var value = this.bindings.value.get();
                        var text = options.text || '';
                        var idx = 0, length;
                        var values = [];
                        if (value === undefined) {
                            value = null;
                        }
                        if (valueField) {
                            if (value instanceof ObservableArray) {
                                for (length = value.length; idx < length; idx++) {
                                    values[idx] = value[idx].get(valueField);
                                }
                                value = values;
                            } else if (value instanceof ObservableObject) {
                                text = value.get(textField);
                                value = value.get(valueField);
                            }
                        }
                        if (options.autoBind === false && !options.cascadeFrom && widget.listView && !widget.listView.bound()) {
                            if (textField === valueField && !text) {
                                text = value;
                            }
                            if (!text && (value || value === 0) && options.valuePrimitive) {
                                widget.value(value);
                            } else {
                                widget._preselect(value, text);
                            }
                        } else {
                            widget.value(value);
                        }
                    }
                    this._initChange = false;
                },
                destroy: function () {
                    this.widget.unbind(CHANGE, this._change);
                }
            }),
            dropdowntree: {
                value: Binder.extend({
                    init: function (widget, bindings, options) {
                        Binder.fn.init.call(this, widget.element[0], bindings, options);
                        this.widget = widget;
                        this._change = $.proxy(this.change, this);
                        this.widget.first(CHANGE, this._change);
                        this._initChange = false;
                    },
                    change: function () {
                        var that = this, oldValues = that.bindings[VALUE].get(), valuePrimitive = that.options.valuePrimitive, selectedNode = that.widget.treeview.select(), nonPrimitiveValues = that.widget._isMultipleSelection() ? that.widget._getAllChecked() : that.widget.treeview.dataItem(selectedNode) || that.widget.value(), newValues = valuePrimitive || that.widget.options.autoBind === false ? that.widget.value() : nonPrimitiveValues;
                        var field = this.options.dataValueField || this.options.dataTextField;
                        newValues = newValues.slice ? newValues.slice(0) : newValues;
                        that._initChange = true;
                        if (oldValues instanceof ObservableArray) {
                            var remove = [];
                            var newLength = newValues.length;
                            var i = 0, j = 0;
                            var old = oldValues[i];
                            var same = false;
                            var removeIndex;
                            var newValue;
                            var found;
                            while (old !== undefined) {
                                found = false;
                                for (j = 0; j < newLength; j++) {
                                    if (valuePrimitive) {
                                        same = newValues[j] == old;
                                    } else {
                                        newValue = newValues[j];
                                        newValue = newValue.get ? newValue.get(field) : newValue;
                                        same = newValue == (old.get ? old.get(field) : old);
                                    }
                                    if (same) {
                                        newValues.splice(j, 1);
                                        newLength -= 1;
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found) {
                                    remove.push(old);
                                    arraySplice(oldValues, i, 1);
                                    removeIndex = i;
                                } else {
                                    i += 1;
                                }
                                old = oldValues[i];
                            }
                            arraySplice(oldValues, oldValues.length, 0, newValues);
                            if (remove.length) {
                                oldValues.trigger('change', {
                                    action: 'remove',
                                    items: remove,
                                    index: removeIndex
                                });
                            }
                            if (newValues.length) {
                                oldValues.trigger('change', {
                                    action: 'add',
                                    items: newValues,
                                    index: oldValues.length - 1
                                });
                            }
                        } else {
                            that.bindings[VALUE].set(newValues);
                        }
                        that._initChange = false;
                    },
                    refresh: function () {
                        if (!this._initChange) {
                            var options = this.options, widget = this.widget, field = options.dataValueField || options.dataTextField, value = this.bindings.value.get(), data = value, idx = 0, length, values = [], selectedValue;
                            if (field) {
                                if (value instanceof ObservableArray) {
                                    for (length = value.length; idx < length; idx++) {
                                        selectedValue = value[idx];
                                        values[idx] = selectedValue.get ? selectedValue.get(field) : selectedValue;
                                    }
                                    value = values;
                                } else if (value instanceof ObservableObject) {
                                    value = value.get(field);
                                }
                            }
                            if (options.autoBind === false && options.valuePrimitive !== true) {
                                widget._preselect(data, value);
                            } else {
                                widget.value(value);
                            }
                        }
                    },
                    destroy: function () {
                        this.widget.unbind(CHANGE, this._change);
                    }
                })
            },
            gantt: { dependencies: dataSourceBinding('dependencies', 'dependencies', 'setDependenciesDataSource') },
            multiselect: {
                value: Binder.extend({
                    init: function (widget, bindings, options) {
                        Binder.fn.init.call(this, widget.element[0], bindings, options);
                        this.widget = widget;
                        this._change = $.proxy(this.change, this);
                        this.widget.first(CHANGE, this._change);
                        this._initChange = false;
                    },
                    change: function () {
                        var that = this, oldValues = that.bindings[VALUE].get(), valuePrimitive = that.options.valuePrimitive, newValues = valuePrimitive ? that.widget.value() : that.widget.dataItems();
                        var field = this.options.dataValueField || this.options.dataTextField;
                        newValues = newValues.slice(0);
                        that._initChange = true;
                        if (oldValues instanceof ObservableArray) {
                            var remove = [];
                            var newLength = newValues.length;
                            var i = 0, j = 0;
                            var old = oldValues[i];
                            var same = false;
                            var removeIndex;
                            var newValue;
                            var found;
                            while (old !== undefined) {
                                found = false;
                                for (j = 0; j < newLength; j++) {
                                    if (valuePrimitive) {
                                        same = newValues[j] == old;
                                    } else {
                                        newValue = newValues[j];
                                        newValue = newValue.get ? newValue.get(field) : newValue;
                                        same = newValue == (old.get ? old.get(field) : old);
                                    }
                                    if (same) {
                                        newValues.splice(j, 1);
                                        newLength -= 1;
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found) {
                                    remove.push(old);
                                    arraySplice(oldValues, i, 1);
                                    removeIndex = i;
                                } else {
                                    i += 1;
                                }
                                old = oldValues[i];
                            }
                            arraySplice(oldValues, oldValues.length, 0, newValues);
                            if (remove.length) {
                                oldValues.trigger('change', {
                                    action: 'remove',
                                    items: remove,
                                    index: removeIndex
                                });
                            }
                            if (newValues.length) {
                                oldValues.trigger('change', {
                                    action: 'add',
                                    items: newValues,
                                    index: oldValues.length - 1
                                });
                            }
                        } else {
                            that.bindings[VALUE].set(newValues);
                        }
                        that._initChange = false;
                    },
                    refresh: function () {
                        if (!this._initChange) {
                            var options = this.options, widget = this.widget, field = options.dataValueField || options.dataTextField, value = this.bindings.value.get(), data = value, idx = 0, length, values = [], selectedValue;
                            if (value === undefined) {
                                value = null;
                            }
                            if (field) {
                                if (value instanceof ObservableArray) {
                                    for (length = value.length; idx < length; idx++) {
                                        selectedValue = value[idx];
                                        values[idx] = selectedValue.get ? selectedValue.get(field) : selectedValue;
                                    }
                                    value = values;
                                } else if (value instanceof ObservableObject) {
                                    value = value.get(field);
                                }
                            }
                            if (options.autoBind === false && options.valuePrimitive !== true && !widget._isBound()) {
                                widget._preselect(data, value);
                            } else {
                                widget.value(value);
                            }
                        }
                    },
                    destroy: function () {
                        this.widget.unbind(CHANGE, this._change);
                    }
                })
            },
            scheduler: {
                source: dataSourceBinding('source', 'dataSource', 'setDataSource').extend({
                    dataBound: function (e) {
                        var idx;
                        var length;
                        var widget = this.widget;
                        var elements = e.addedItems || widget.items();
                        var data, parents;
                        if (elements.length) {
                            data = e.addedDataItems || widget.dataItems();
                            parents = this.bindings.source._parents();
                            for (idx = 0, length = data.length; idx < length; idx++) {
                                bindElement(elements[idx], data[idx], this._ns(e.ns), [data[idx]].concat(parents));
                            }
                        }
                    }
                })
            }
        };
        var arraySplice = function (arr, idx, remove, add) {
            add = add || [];
            remove = remove || 0;
            var addLength = add.length;
            var oldLength = arr.length;
            var shifted = [].slice.call(arr, idx + remove);
            var shiftedLength = shifted.length;
            var index;
            if (addLength) {
                addLength = idx + addLength;
                index = 0;
                for (; idx < addLength; idx++) {
                    arr[idx] = add[index];
                    index++;
                }
                arr.length = addLength;
            } else if (remove) {
                arr.length = idx;
                remove += idx;
                while (idx < remove) {
                    delete arr[--remove];
                }
            }
            if (shiftedLength) {
                shiftedLength = idx + shiftedLength;
                index = 0;
                for (; idx < shiftedLength; idx++) {
                    arr[idx] = shifted[index];
                    index++;
                }
                arr.length = shiftedLength;
            }
            idx = arr.length;
            while (idx < oldLength) {
                delete arr[idx];
                idx++;
            }
        };
        var BindingTarget = Class.extend({
            init: function (target, options) {
                this.target = target;
                this.options = options;
                this.toDestroy = [];
            },
            bind: function (bindings) {
                var key, hasValue, hasSource, hasEvents, hasChecked, hasCss, widgetBinding = this instanceof WidgetBindingTarget, specificBinders = this.binders();
                for (key in bindings) {
                    if (key == VALUE) {
                        hasValue = true;
                    } else if (key == SOURCE) {
                        hasSource = true;
                    } else if (key == EVENTS && !widgetBinding) {
                        hasEvents = true;
                    } else if (key == CHECKED) {
                        hasChecked = true;
                    } else if (key == CSS) {
                        hasCss = true;
                    } else {
                        this.applyBinding(key, bindings, specificBinders);
                    }
                }
                if (hasSource) {
                    this.applyBinding(SOURCE, bindings, specificBinders);
                }
                if (hasValue) {
                    this.applyBinding(VALUE, bindings, specificBinders);
                }
                if (hasChecked) {
                    this.applyBinding(CHECKED, bindings, specificBinders);
                }
                if (hasEvents && !widgetBinding) {
                    this.applyBinding(EVENTS, bindings, specificBinders);
                }
                if (hasCss && !widgetBinding) {
                    this.applyBinding(CSS, bindings, specificBinders);
                }
            },
            binders: function () {
                return binders[this.target.nodeName.toLowerCase()] || {};
            },
            applyBinding: function (name, bindings, specificBinders) {
                var binder = specificBinders[name] || binders[name], toDestroy = this.toDestroy, attribute, binding = bindings[name];
                if (binder) {
                    binder = new binder(this.target, bindings, this.options);
                    toDestroy.push(binder);
                    if (binding instanceof Binding) {
                        binder.bind(binding);
                        toDestroy.push(binding);
                    } else {
                        for (attribute in binding) {
                            binder.bind(binding, attribute);
                            toDestroy.push(binding[attribute]);
                        }
                    }
                } else if (name !== 'template') {
                    throw new Error('The ' + name + ' binding is not supported by the ' + this.target.nodeName.toLowerCase() + ' element');
                }
            },
            destroy: function () {
                var idx, length, toDestroy = this.toDestroy;
                for (idx = 0, length = toDestroy.length; idx < length; idx++) {
                    toDestroy[idx].destroy();
                }
            }
        });
        var WidgetBindingTarget = BindingTarget.extend({
            binders: function () {
                return binders.widget[this.target.options.name.toLowerCase()] || {};
            },
            applyBinding: function (name, bindings, specificBinders) {
                var binder = specificBinders[name] || binders.widget[name], toDestroy = this.toDestroy, attribute, binding = bindings[name];
                if (binder) {
                    binder = new binder(this.target, bindings, this.target.options);
                    toDestroy.push(binder);
                    if (binding instanceof Binding) {
                        binder.bind(binding);
                        toDestroy.push(binding);
                    } else {
                        for (attribute in binding) {
                            binder.bind(binding, attribute);
                            toDestroy.push(binding[attribute]);
                        }
                    }
                } else {
                    throw new Error('The ' + name + ' binding is not supported by the ' + this.target.options.name + ' widget');
                }
            }
        });
        function bindingTargetForRole(element, roles) {
            var widget = kendo.initWidget(element, {}, roles);
            if (widget) {
                return new WidgetBindingTarget(widget);
            }
        }
        var keyValueRegExp = /[A-Za-z0-9_\-]+:(\{([^}]*)\}|[^,}]+)/g, whiteSpaceRegExp = /\s/g;
        function parseBindings(bind) {
            var result = {}, idx, length, token, colonIndex, key, value, tokens;
            tokens = bind.match(keyValueRegExp);
            for (idx = 0, length = tokens.length; idx < length; idx++) {
                token = tokens[idx];
                colonIndex = token.indexOf(':');
                key = token.substring(0, colonIndex);
                value = token.substring(colonIndex + 1);
                if (value.charAt(0) == '{') {
                    value = parseBindings(value);
                }
                result[key] = value;
            }
            return result;
        }
        function createBindings(bindings, source, type) {
            var binding, result = {};
            for (binding in bindings) {
                result[binding] = new type(source, bindings[binding]);
            }
            return result;
        }
        function bindElement(element, source, roles, parents) {
            if (!element) {
                return;
            }
            var role = element.getAttribute('data-' + kendo.ns + 'role'), idx, bind = element.getAttribute('data-' + kendo.ns + 'bind'), childrenCopy = [], deep = true, bindings, options = {}, target;
            parents = parents || [source];
            if (role || bind) {
                unbindElement(element, false);
            }
            if (role) {
                target = bindingTargetForRole(element, roles);
            }
            if (bind) {
                bind = parseBindings(bind.replace(whiteSpaceRegExp, ''));
                if (!target) {
                    options = kendo.parseOptions(element, {
                        textField: '',
                        valueField: '',
                        template: '',
                        valueUpdate: CHANGE,
                        valuePrimitive: false,
                        autoBind: true
                    }, source);
                    options.roles = roles;
                    target = new BindingTarget(element, options);
                }
                target.source = source;
                bindings = createBindings(bind, parents, Binding);
                if (options.template) {
                    bindings.template = new TemplateBinding(parents, '', options.template);
                }
                if (bindings.click) {
                    bind.events = bind.events || {};
                    bind.events.click = bind.click;
                    bindings.click.destroy();
                    delete bindings.click;
                }
                if (bindings.source) {
                    deep = false;
                }
                if (bind.attr) {
                    bindings.attr = createBindings(bind.attr, parents, Binding);
                }
                if (bind.style) {
                    bindings.style = createBindings(bind.style, parents, Binding);
                }
                if (bind.events) {
                    bindings.events = createBindings(bind.events, parents, EventBinding);
                }
                if (bind.css) {
                    bindings.css = createBindings(bind.css, parents, Binding);
                }
                target.bind(bindings);
            }
            if (target) {
                element.kendoBindingTarget = target;
            }
            var children = element.children;
            if (deep && children) {
                for (idx = 0; idx < children.length; idx++) {
                    childrenCopy[idx] = children[idx];
                }
                for (idx = 0; idx < childrenCopy.length; idx++) {
                    bindElement(childrenCopy[idx], source, roles, parents);
                }
            }
        }
        function bind(dom, object) {
            var idx, length, node, roles = kendo.rolesFromNamespaces([].slice.call(arguments, 2));
            object = kendo.observable(object);
            dom = $(dom);
            for (idx = 0, length = dom.length; idx < length; idx++) {
                node = dom[idx];
                if (node.nodeType === 1) {
                    bindElement(node, object, roles);
                }
            }
        }
        function unbindElement(element, destroyWidget) {
            var bindingTarget = element.kendoBindingTarget;
            if (bindingTarget) {
                bindingTarget.destroy();
                if (deleteExpando) {
                    delete element.kendoBindingTarget;
                } else if (element.removeAttribute) {
                    element.removeAttribute('kendoBindingTarget');
                } else {
                    element.kendoBindingTarget = null;
                }
            }
            if (destroyWidget) {
                var widget = kendo.widgetInstance($(element));
                if (widget && typeof widget.destroy === FUNCTION) {
                    widget.destroy();
                }
            }
        }
        function unbindElementTree(element, destroyWidgets) {
            unbindElement(element, destroyWidgets);
            unbindElementChildren(element, destroyWidgets);
        }
        function unbindElementChildren(element, destroyWidgets) {
            var children = element.children;
            if (children) {
                for (var idx = 0, length = children.length; idx < length; idx++) {
                    unbindElementTree(children[idx], destroyWidgets);
                }
            }
        }
        function unbind(dom) {
            var idx, length;
            dom = $(dom);
            for (idx = 0, length = dom.length; idx < length; idx++) {
                unbindElementTree(dom[idx], false);
            }
        }
        function notify(widget, namespace) {
            var element = widget.element, bindingTarget = element[0].kendoBindingTarget;
            if (bindingTarget) {
                bind(element, bindingTarget.source, namespace);
            }
        }
        function retrievePrimitiveValues(value, valueField) {
            var values = [];
            var idx = 0;
            var length;
            var item;
            if (!valueField) {
                return value;
            }
            if (value instanceof ObservableArray) {
                for (length = value.length; idx < length; idx++) {
                    item = value[idx];
                    values[idx] = item.get ? item.get(valueField) : item[valueField];
                }
                value = values;
            } else if (value instanceof ObservableObject) {
                value = value.get(valueField);
            }
            return value;
        }
        kendo.unbind = unbind;
        kendo.bind = bind;
        kendo.data.binders = binders;
        kendo.data.Binder = Binder;
        kendo.notify = notify;
        kendo.observable = function (object) {
            if (!(object instanceof ObservableObject)) {
                object = new ObservableObject(object);
            }
            return object;
        };
        kendo.observableHierarchy = function (array) {
            var dataSource = kendo.data.HierarchicalDataSource.create(array);
            function recursiveRead(data) {
                var i, children;
                for (i = 0; i < data.length; i++) {
                    data[i]._initChildren();
                    children = data[i].children;
                    children.fetch();
                    data[i].items = children.data();
                    recursiveRead(data[i].items);
                }
            }
            dataSource.fetch();
            recursiveRead(dataSource.data());
            dataSource._data._dataSource = dataSource;
            return dataSource._data;
        };
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.validator', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'validator',
        name: 'Validator',
        category: 'web',
        description: 'The Validator offers an easy way to do a client-side form validation.',
        depends: ['core']
    };
    (function ($, undefined) {
        var kendo = window.kendo, Widget = kendo.ui.Widget, NS = '.kendoValidator', INVALIDMSG = 'k-invalid-msg', invalidMsgRegExp = new RegExp(INVALIDMSG, 'i'), INVALIDINPUT = 'k-invalid', VALIDINPUT = 'k-valid', emailRegExp = /^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/i, urlRegExp = /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i, INPUTSELECTOR = ':input:not(:button,[type=submit],[type=reset],[disabled],[readonly])', CHECKBOXSELECTOR = ':checkbox:not([disabled],[readonly])', NUMBERINPUTSELECTOR = '[type=number],[type=range]', BLUR = 'blur', NAME = 'name', FORM = 'form', NOVALIDATE = 'novalidate', VALIDATE = 'validate', CHANGE = 'change', VALIDATE_INPUT = 'validateInput', proxy = $.proxy, patternMatcher = function (value, pattern) {
                if (typeof pattern === 'string') {
                    pattern = new RegExp('^(?:' + pattern + ')$');
                }
                return pattern.test(value);
            }, matcher = function (input, selector, pattern) {
                var value = input.val();
                if (input.filter(selector).length && value !== '') {
                    return patternMatcher(value, pattern);
                }
                return true;
            }, hasAttribute = function (input, name) {
                if (input.length) {
                    return input[0].attributes[name] != null;
                }
                return false;
            };
        if (!kendo.ui.validator) {
            kendo.ui.validator = {
                rules: {},
                messages: {}
            };
        }
        function resolveRules(element) {
            var resolvers = kendo.ui.validator.ruleResolvers || {}, rules = {}, name;
            for (name in resolvers) {
                $.extend(true, rules, resolvers[name].resolve(element));
            }
            return rules;
        }
        function decode(value) {
            return value.replace(/&amp/g, '&amp;').replace(/&quot;/g, '"').replace(/&#39;/g, '\'').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
        }
        function numberOfDecimalDigits(value) {
            value = (value + '').split('.');
            if (value.length > 1) {
                return value[1].length;
            }
            return 0;
        }
        function parseHtml(text) {
            if ($.parseHTML) {
                return $($.parseHTML(text));
            }
            return $(text);
        }
        function searchForMessageContainer(elements, fieldName) {
            var containers = $(), element, attr;
            for (var idx = 0, length = elements.length; idx < length; idx++) {
                element = elements[idx];
                if (invalidMsgRegExp.test(element.className)) {
                    attr = element.getAttribute(kendo.attr('for'));
                    if (attr === fieldName) {
                        containers = containers.add(element);
                    }
                }
            }
            return containers;
        }
        var Validator = Widget.extend({
            init: function (element, options) {
                var that = this, resolved = resolveRules(element), validateAttributeSelector = '[' + kendo.attr('validate') + '!=false]';
                options = options || {};
                options.rules = $.extend({}, kendo.ui.validator.rules, resolved.rules, options.rules);
                options.messages = $.extend({}, kendo.ui.validator.messages, resolved.messages, options.messages);
                Widget.fn.init.call(that, element, options);
                that._errorTemplate = kendo.template(that.options.errorTemplate);
                if (that.element.is(FORM)) {
                    that.element.attr(NOVALIDATE, NOVALIDATE);
                }
                that._inputSelector = INPUTSELECTOR + validateAttributeSelector;
                that._checkboxSelector = CHECKBOXSELECTOR + validateAttributeSelector;
                that._errors = {};
                that._attachEvents();
                that._isValidated = false;
            },
            events: [
                VALIDATE,
                CHANGE,
                VALIDATE_INPUT
            ],
            options: {
                name: 'Validator',
                errorTemplate: '<span class="k-widget k-tooltip k-tooltip-validation">' + '<span class="k-icon k-i-warning"> </span> #=message#</span>',
                messages: {
                    required: '{0} is required',
                    pattern: '{0} is not valid',
                    min: '{0} should be greater than or equal to {1}',
                    max: '{0} should be smaller than or equal to {1}',
                    step: '{0} is not valid',
                    email: '{0} is not valid email',
                    url: '{0} is not valid URL',
                    date: '{0} is not valid date',
                    dateCompare: 'End date should be greater than or equal to the start date'
                },
                rules: {
                    required: function (input) {
                        var checkbox = input.filter('[type=checkbox]').length && !input.is(':checked'), value = input.val();
                        return !(hasAttribute(input, 'required') && (!value || value === '' || value.length === 0 || checkbox));
                    },
                    pattern: function (input) {
                        if (input.filter('[type=text],[type=email],[type=url],[type=tel],[type=search],[type=password]').filter('[pattern]').length && input.val() !== '') {
                            return patternMatcher(input.val(), input.attr('pattern'));
                        }
                        return true;
                    },
                    min: function (input) {
                        if (input.filter(NUMBERINPUTSELECTOR + ',[' + kendo.attr('type') + '=number]').filter('[min]').length && input.val() !== '') {
                            var min = parseFloat(input.attr('min')) || 0, val = kendo.parseFloat(input.val());
                            return min <= val;
                        }
                        return true;
                    },
                    max: function (input) {
                        if (input.filter(NUMBERINPUTSELECTOR + ',[' + kendo.attr('type') + '=number]').filter('[max]').length && input.val() !== '') {
                            var max = parseFloat(input.attr('max')) || 0, val = kendo.parseFloat(input.val());
                            return max >= val;
                        }
                        return true;
                    },
                    step: function (input) {
                        if (input.filter(NUMBERINPUTSELECTOR + ',[' + kendo.attr('type') + '=number]').filter('[step]').length && input.val() !== '') {
                            var min = parseFloat(input.attr('min')) || 0, step = parseFloat(input.attr('step')) || 1, val = parseFloat(input.val()), decimals = numberOfDecimalDigits(step), raise;
                            if (decimals) {
                                raise = Math.pow(10, decimals);
                                return Math.floor((val - min) * raise) % (step * raise) / Math.pow(100, decimals) === 0;
                            }
                            return (val - min) % step === 0;
                        }
                        return true;
                    },
                    email: function (input) {
                        return matcher(input, '[type=email],[' + kendo.attr('type') + '=email]', emailRegExp);
                    },
                    url: function (input) {
                        return matcher(input, '[type=url],[' + kendo.attr('type') + '=url]', urlRegExp);
                    },
                    date: function (input) {
                        if (input.filter('[type^=date],[' + kendo.attr('type') + '=date]').length && input.val() !== '') {
                            return kendo.parseDate(input.val(), input.attr(kendo.attr('format'))) !== null;
                        }
                        return true;
                    }
                },
                validateOnBlur: true
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.element.off(NS);
            },
            value: function () {
                if (!this._isValidated) {
                    return false;
                }
                return this.errors().length === 0;
            },
            _submit: function (e) {
                if (!this.validate()) {
                    e.stopPropagation();
                    e.stopImmediatePropagation();
                    e.preventDefault();
                    return false;
                }
                return true;
            },
            _checkElement: function (element) {
                var state = this.value();
                this.validateInput(element);
                if (this.value() !== state) {
                    this.trigger(CHANGE);
                }
            },
            _attachEvents: function () {
                var that = this;
                if (that.element.is(FORM)) {
                    that.element.on('submit' + NS, proxy(that._submit, that));
                }
                if (that.options.validateOnBlur) {
                    if (!that.element.is(INPUTSELECTOR)) {
                        that.element.on(BLUR + NS, that._inputSelector, function () {
                            that._checkElement($(this));
                        });
                        that.element.on('click' + NS, that._checkboxSelector, function () {
                            that._checkElement($(this));
                        });
                    } else {
                        that.element.on(BLUR + NS, function () {
                            that._checkElement(that.element);
                        });
                        if (that.element.is(CHECKBOXSELECTOR)) {
                            that.element.on('click' + NS, function () {
                                that._checkElement(that.element);
                            });
                        }
                    }
                }
            },
            validate: function () {
                var inputs;
                var idx;
                var result = false;
                var length;
                var isValid = this.value();
                this._errors = {};
                if (!this.element.is(INPUTSELECTOR)) {
                    var invalid = false;
                    inputs = this.element.find(this._inputSelector);
                    for (idx = 0, length = inputs.length; idx < length; idx++) {
                        if (!this.validateInput(inputs.eq(idx))) {
                            invalid = true;
                        }
                    }
                    result = !invalid;
                } else {
                    result = this.validateInput(this.element);
                }
                this.trigger(VALIDATE, { valid: result });
                if (isValid !== result) {
                    this.trigger(CHANGE);
                }
                return result;
            },
            validateInput: function (input) {
                input = $(input);
                this._isValidated = true;
                var that = this, template = that._errorTemplate, result = that._checkValidity(input), valid = result.valid, className = '.' + INVALIDMSG, fieldName = input.attr(NAME) || '', lbl = that._findMessageContainer(fieldName).add(input.next(className).filter(function () {
                        var element = $(this);
                        if (element.filter('[' + kendo.attr('for') + ']').length) {
                            return element.attr(kendo.attr('for')) === fieldName;
                        }
                        return true;
                    })).hide(), messageText, wasValid = !input.attr('aria-invalid');
                input.removeAttr('aria-invalid');
                if (!valid) {
                    messageText = that._extractMessage(input, result.key);
                    that._errors[fieldName] = messageText;
                    var messageLabel = parseHtml(template({ message: decode(messageText) }));
                    var lblId = lbl.attr('id');
                    that._decorateMessageContainer(messageLabel, fieldName);
                    if (lblId) {
                        messageLabel.attr('id', lblId);
                    }
                    if (!lbl.replaceWith(messageLabel).length) {
                        messageLabel.insertAfter(input);
                    }
                    messageLabel.show();
                    input.attr('aria-invalid', true);
                } else {
                    delete that._errors[fieldName];
                }
                if (wasValid !== valid) {
                    this.trigger(VALIDATE_INPUT, {
                        valid: valid,
                        input: input
                    });
                }
                input.toggleClass(INVALIDINPUT, !valid);
                input.toggleClass(VALIDINPUT, valid);
                return valid;
            },
            hideMessages: function () {
                var that = this, className = '.' + INVALIDMSG, element = that.element;
                if (!element.is(INPUTSELECTOR)) {
                    element.find(className).hide();
                } else {
                    element.next(className).hide();
                }
            },
            _findMessageContainer: function (fieldName) {
                var locators = kendo.ui.validator.messageLocators, name, containers = $();
                for (var idx = 0, length = this.element.length; idx < length; idx++) {
                    containers = containers.add(searchForMessageContainer(this.element[idx].getElementsByTagName('*'), fieldName));
                }
                for (name in locators) {
                    containers = containers.add(locators[name].locate(this.element, fieldName));
                }
                return containers;
            },
            _decorateMessageContainer: function (container, fieldName) {
                var locators = kendo.ui.validator.messageLocators, name;
                container.addClass(INVALIDMSG).attr(kendo.attr('for'), fieldName || '');
                for (name in locators) {
                    locators[name].decorate(container, fieldName);
                }
                container.attr('role', 'alert');
            },
            _extractMessage: function (input, ruleKey) {
                var that = this, customMessage = that.options.messages[ruleKey], fieldName = input.attr(NAME), nonDefaultMessage;
                if (!kendo.ui.Validator.prototype.options.messages[ruleKey]) {
                    nonDefaultMessage = kendo.isFunction(customMessage) ? customMessage(input) : customMessage;
                }
                customMessage = kendo.isFunction(customMessage) ? customMessage(input) : customMessage;
                return kendo.format(input.attr(kendo.attr(ruleKey + '-msg')) || input.attr('validationMessage') || nonDefaultMessage || input.attr('title') || customMessage || '', fieldName, input.attr(ruleKey) || input.attr(kendo.attr(ruleKey)));
            },
            _checkValidity: function (input) {
                var rules = this.options.rules, rule;
                for (rule in rules) {
                    if (!rules[rule].call(this, input)) {
                        return {
                            valid: false,
                            key: rule
                        };
                    }
                }
                return { valid: true };
            },
            errors: function () {
                var results = [], errors = this._errors, error;
                for (error in errors) {
                    results.push(errors[error]);
                }
                return results;
            }
        });
        kendo.ui.plugin(Validator);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.router', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'router',
        name: 'Router',
        category: 'framework',
        description: 'The Router class is responsible for tracking the application state and navigating between the application states.',
        depends: ['core'],
        hidden: false
    };
    (function ($, undefined) {
        var kendo = window.kendo, CHANGE = 'change', BACK = 'back', SAME = 'same', support = kendo.support, location = window.location, history = window.history, CHECK_URL_INTERVAL = 50, BROKEN_BACK_NAV = kendo.support.browser.msie, hashStrip = /^#*/, document = window.document;
        function absoluteURL(path, pathPrefix) {
            if (!pathPrefix) {
                return path;
            }
            if (path + '/' === pathPrefix) {
                path = pathPrefix;
            }
            var regEx = new RegExp('^' + pathPrefix, 'i');
            if (!regEx.test(path)) {
                path = pathPrefix + '/' + path;
            }
            return location.protocol + '//' + (location.host + '/' + path).replace(/\/\/+/g, '/');
        }
        function hashDelimiter(bang) {
            return bang ? '#!' : '#';
        }
        function locationHash(hashDelimiter) {
            var href = location.href;
            if (hashDelimiter === '#!' && href.indexOf('#') > -1 && href.indexOf('#!') < 0) {
                return null;
            }
            return href.split(hashDelimiter)[1] || '';
        }
        function stripRoot(root, url) {
            if (url.indexOf(root) === 0) {
                return url.substr(root.length).replace(/\/\//g, '/');
            } else {
                return url;
            }
        }
        var HistoryAdapter = kendo.Class.extend({
            back: function () {
                if (BROKEN_BACK_NAV) {
                    setTimeout(function () {
                        history.back();
                    });
                } else {
                    history.back();
                }
            },
            forward: function () {
                if (BROKEN_BACK_NAV) {
                    setTimeout(function () {
                        history.forward();
                    });
                } else {
                    history.forward();
                }
            },
            length: function () {
                return history.length;
            },
            replaceLocation: function (url) {
                location.replace(url);
            }
        });
        var PushStateAdapter = HistoryAdapter.extend({
            init: function (root) {
                this.root = root;
            },
            navigate: function (to) {
                history.pushState({}, document.title, absoluteURL(to, this.root));
            },
            replace: function (to) {
                history.replaceState({}, document.title, absoluteURL(to, this.root));
            },
            normalize: function (url) {
                return stripRoot(this.root, url);
            },
            current: function () {
                var current = location.pathname;
                if (location.search) {
                    current += location.search;
                }
                return stripRoot(this.root, current);
            },
            change: function (callback) {
                $(window).bind('popstate.kendo', callback);
            },
            stop: function () {
                $(window).unbind('popstate.kendo');
            },
            normalizeCurrent: function (options) {
                var fixedUrl, root = options.root, pathname = location.pathname, hash = locationHash(hashDelimiter(options.hashBang));
                if (root === pathname + '/') {
                    fixedUrl = root;
                }
                if (root === pathname && hash) {
                    fixedUrl = absoluteURL(hash.replace(hashStrip, ''), root);
                }
                if (fixedUrl) {
                    history.pushState({}, document.title, fixedUrl);
                }
            }
        });
        function fixHash(url) {
            return url.replace(/^(#)?/, '#');
        }
        function fixBang(url) {
            return url.replace(/^(#(!)?)?/, '#!');
        }
        var HashAdapter = HistoryAdapter.extend({
            init: function (bang) {
                this._id = kendo.guid();
                this.prefix = hashDelimiter(bang);
                this.fix = bang ? fixBang : fixHash;
            },
            navigate: function (to) {
                location.hash = this.fix(to);
            },
            replace: function (to) {
                this.replaceLocation(this.fix(to));
            },
            normalize: function (url) {
                if (url.indexOf(this.prefix) < 0) {
                    return url;
                } else {
                    return url.split(this.prefix)[1];
                }
            },
            change: function (callback) {
                if (support.hashChange) {
                    $(window).on('hashchange.' + this._id, callback);
                } else {
                    this._interval = setInterval(callback, CHECK_URL_INTERVAL);
                }
            },
            stop: function () {
                $(window).off('hashchange.' + this._id);
                clearInterval(this._interval);
            },
            current: function () {
                return locationHash(this.prefix);
            },
            normalizeCurrent: function (options) {
                var pathname = location.pathname, root = options.root;
                if (options.pushState && root !== pathname) {
                    this.replaceLocation(root + this.prefix + stripRoot(root, pathname));
                    return true;
                }
                return false;
            }
        });
        var History = kendo.Observable.extend({
            start: function (options) {
                options = options || {};
                this.bind([
                    CHANGE,
                    BACK,
                    SAME
                ], options);
                if (this._started) {
                    return;
                }
                this._started = true;
                options.root = options.root || '/';
                var adapter = this.createAdapter(options), current;
                if (adapter.normalizeCurrent(options)) {
                    return;
                }
                current = adapter.current();
                $.extend(this, {
                    adapter: adapter,
                    root: options.root,
                    historyLength: adapter.length(),
                    current: current,
                    locations: [current]
                });
                adapter.change($.proxy(this, '_checkUrl'));
            },
            createAdapter: function (options) {
                return support.pushState && options.pushState ? new PushStateAdapter(options.root) : new HashAdapter(options.hashBang);
            },
            stop: function () {
                if (!this._started) {
                    return;
                }
                this.adapter.stop();
                this.unbind(CHANGE);
                this._started = false;
            },
            change: function (callback) {
                this.bind(CHANGE, callback);
            },
            replace: function (to, silent) {
                this._navigate(to, silent, function (adapter) {
                    adapter.replace(to);
                    this.locations[this.locations.length - 1] = this.current;
                });
            },
            navigate: function (to, silent) {
                if (to === '#:back') {
                    this.backCalled = true;
                    this.adapter.back();
                    return;
                }
                this._navigate(to, silent, function (adapter) {
                    adapter.navigate(to);
                    this.locations.push(this.current);
                });
            },
            _navigate: function (to, silent, callback) {
                var adapter = this.adapter;
                to = adapter.normalize(to);
                if (this.current === to || this.current === decodeURIComponent(to)) {
                    this.trigger(SAME);
                    return;
                }
                if (!silent) {
                    if (this.trigger(CHANGE, {
                            url: to,
                            decode: false
                        })) {
                        return;
                    }
                }
                this.current = to;
                callback.call(this, adapter);
                this.historyLength = adapter.length();
            },
            _checkUrl: function () {
                var adapter = this.adapter, current = adapter.current(), newLength = adapter.length(), navigatingInExisting = this.historyLength === newLength, back = current === this.locations[this.locations.length - 2] && navigatingInExisting, backCalled = this.backCalled, prev = this.current;
                if (current === null || this.current === current || this.current === decodeURIComponent(current)) {
                    return true;
                }
                this.historyLength = newLength;
                this.backCalled = false;
                this.current = current;
                if (back && this.trigger('back', {
                        url: prev,
                        to: current
                    })) {
                    adapter.forward();
                    this.current = prev;
                    return;
                }
                if (this.trigger(CHANGE, {
                        url: current,
                        backButtonPressed: !backCalled
                    })) {
                    if (back) {
                        adapter.forward();
                    } else {
                        adapter.back();
                        this.historyLength--;
                    }
                    this.current = prev;
                    return;
                }
                if (back) {
                    this.locations.pop();
                } else {
                    this.locations.push(current);
                }
            }
        });
        kendo.History = History;
        kendo.History.HistoryAdapter = HistoryAdapter;
        kendo.History.HashAdapter = HashAdapter;
        kendo.History.PushStateAdapter = PushStateAdapter;
        kendo.absoluteURL = absoluteURL;
        kendo.history = new History();
    }(window.kendo.jQuery));
    (function () {
        var kendo = window.kendo, history = kendo.history, Observable = kendo.Observable, INIT = 'init', ROUTE_MISSING = 'routeMissing', CHANGE = 'change', BACK = 'back', SAME = 'same', optionalParam = /\((.*?)\)/g, namedParam = /(\(\?)?:\w+/g, splatParam = /\*\w+/g, escapeRegExp = /[\-{}\[\]+?.,\\\^$|#\s]/g;
        function namedParamReplace(match, optional) {
            return optional ? match : '([^/]+)';
        }
        function routeToRegExp(route, ignoreCase) {
            return new RegExp('^' + route.replace(escapeRegExp, '\\$&').replace(optionalParam, '(?:$1)?').replace(namedParam, namedParamReplace).replace(splatParam, '(.*?)') + '$', ignoreCase ? 'i' : '');
        }
        function stripUrl(url) {
            return url.replace(/(\?.*)|(#.*)/g, '');
        }
        var Route = kendo.Class.extend({
            init: function (route, callback, ignoreCase) {
                if (!(route instanceof RegExp)) {
                    route = routeToRegExp(route, ignoreCase);
                }
                this.route = route;
                this._callback = callback;
            },
            callback: function (url, back, decode) {
                var params, idx = 0, length, queryStringParams = kendo.parseQueryStringParams(url);
                queryStringParams._back = back;
                url = stripUrl(url);
                params = this.route.exec(url).slice(1);
                length = params.length;
                if (decode) {
                    for (; idx < length; idx++) {
                        if (typeof params[idx] !== 'undefined') {
                            params[idx] = decodeURIComponent(params[idx]);
                        }
                    }
                }
                params.push(queryStringParams);
                this._callback.apply(null, params);
            },
            worksWith: function (url, back, decode) {
                if (this.route.test(stripUrl(url))) {
                    this.callback(url, back, decode);
                    return true;
                } else {
                    return false;
                }
            }
        });
        var Router = Observable.extend({
            init: function (options) {
                if (!options) {
                    options = {};
                }
                Observable.fn.init.call(this);
                this.routes = [];
                this.pushState = options.pushState;
                this.hashBang = options.hashBang;
                this.root = options.root;
                this.ignoreCase = options.ignoreCase !== false;
                this.bind([
                    INIT,
                    ROUTE_MISSING,
                    CHANGE,
                    SAME
                ], options);
            },
            destroy: function () {
                history.unbind(CHANGE, this._urlChangedProxy);
                history.unbind(SAME, this._sameProxy);
                history.unbind(BACK, this._backProxy);
                this.unbind();
            },
            start: function () {
                var that = this, sameProxy = function () {
                        that._same();
                    }, backProxy = function (e) {
                        that._back(e);
                    }, urlChangedProxy = function (e) {
                        that._urlChanged(e);
                    };
                history.start({
                    same: sameProxy,
                    change: urlChangedProxy,
                    back: backProxy,
                    pushState: that.pushState,
                    hashBang: that.hashBang,
                    root: that.root
                });
                var initEventObject = {
                    url: history.current || '/',
                    preventDefault: $.noop
                };
                if (!that.trigger(INIT, initEventObject)) {
                    that._urlChanged(initEventObject);
                }
                this._urlChangedProxy = urlChangedProxy;
                this._backProxy = backProxy;
            },
            route: function (route, callback) {
                this.routes.push(new Route(route, callback, this.ignoreCase));
            },
            navigate: function (url, silent) {
                kendo.history.navigate(url, silent);
            },
            replace: function (url, silent) {
                kendo.history.replace(url, silent);
            },
            _back: function (e) {
                if (this.trigger(BACK, {
                        url: e.url,
                        to: e.to
                    })) {
                    e.preventDefault();
                }
            },
            _same: function () {
                this.trigger(SAME);
            },
            _urlChanged: function (e) {
                var url = e.url;
                var decode = !!e.decode;
                var back = e.backButtonPressed;
                if (!url) {
                    url = '/';
                }
                if (this.trigger(CHANGE, {
                        url: e.url,
                        params: kendo.parseQueryStringParams(e.url),
                        backButtonPressed: back
                    })) {
                    e.preventDefault();
                    return;
                }
                var idx = 0, routes = this.routes, route, length = routes.length;
                for (; idx < length; idx++) {
                    route = routes[idx];
                    if (route.worksWith(url, back, decode)) {
                        return;
                    }
                }
                if (this.trigger(ROUTE_MISSING, {
                        url: url,
                        params: kendo.parseQueryStringParams(url),
                        backButtonPressed: back
                    })) {
                    e.preventDefault();
                }
            }
        });
        kendo.Router = Router;
    }());
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.view', [
        'kendo.core',
        'kendo.binder',
        'kendo.fx'
    ], f);
}(function () {
    var __meta__ = {
        id: 'view',
        name: 'View',
        category: 'framework',
        description: 'The View class instantiates and handles the events of a certain screen from the application.',
        depends: [
            'core',
            'binder',
            'fx'
        ],
        hidden: false
    };
    (function ($, undefined) {
        var kendo = window.kendo, Observable = kendo.Observable, SCRIPT = 'SCRIPT', INIT = 'init', SHOW = 'show', HIDE = 'hide', TRANSITION_START = 'transitionStart', TRANSITION_END = 'transitionEnd', ATTACH = 'attach', DETACH = 'detach', sizzleErrorRegExp = /unrecognized expression/;
        var View = Observable.extend({
            init: function (content, options) {
                var that = this;
                options = options || {};
                Observable.fn.init.call(that);
                that.content = content;
                that.id = kendo.guid();
                that.tagName = options.tagName || 'div';
                that.model = options.model;
                that._wrap = options.wrap !== false;
                this._evalTemplate = options.evalTemplate || false;
                that._fragments = {};
                that.bind([
                    INIT,
                    SHOW,
                    HIDE,
                    TRANSITION_START,
                    TRANSITION_END
                ], options);
            },
            render: function (container) {
                var that = this, notInitialized = !that.element;
                if (notInitialized) {
                    that.element = that._createElement();
                }
                if (container) {
                    $(container).append(that.element);
                }
                if (notInitialized) {
                    kendo.bind(that.element, that.model);
                    that.trigger(INIT);
                }
                if (container) {
                    that._eachFragment(ATTACH);
                    that.trigger(SHOW);
                }
                return that.element;
            },
            clone: function () {
                return new ViewClone(this);
            },
            triggerBeforeShow: function () {
                return true;
            },
            triggerBeforeHide: function () {
                return true;
            },
            showStart: function () {
                this.element.css('display', '');
            },
            showEnd: function () {
            },
            hideEnd: function () {
                this.hide();
            },
            beforeTransition: function (type) {
                this.trigger(TRANSITION_START, { type: type });
            },
            afterTransition: function (type) {
                this.trigger(TRANSITION_END, { type: type });
            },
            hide: function () {
                this._eachFragment(DETACH);
                this.element.detach();
                this.trigger(HIDE);
            },
            destroy: function () {
                var element = this.element;
                if (element) {
                    kendo.unbind(element);
                    kendo.destroy(element);
                    element.remove();
                }
            },
            fragments: function (fragments) {
                $.extend(this._fragments, fragments);
            },
            _eachFragment: function (methodName) {
                for (var placeholder in this._fragments) {
                    this._fragments[placeholder][methodName](this, placeholder);
                }
            },
            _createElement: function () {
                var that = this, wrapper = '<' + that.tagName + ' />', element, content;
                try {
                    content = $(document.getElementById(that.content) || that.content);
                    if (content[0].tagName === SCRIPT) {
                        content = content.html();
                    }
                } catch (e) {
                    if (sizzleErrorRegExp.test(e.message)) {
                        content = that.content;
                    }
                }
                if (typeof content === 'string') {
                    content = content.replace(/^\s+|\s+$/g, '');
                    if (that._evalTemplate) {
                        content = kendo.template(content)(that.model || {});
                    }
                    element = $(wrapper).append(content);
                    if (!that._wrap) {
                        element = element.contents();
                    }
                } else {
                    element = content;
                    if (that._evalTemplate) {
                        var result = $(kendo.template($('<div />').append(element.clone(true)).html())(that.model || {}));
                        if ($.contains(document, element[0])) {
                            element.replaceWith(result);
                        }
                        element = result;
                    }
                    if (that._wrap) {
                        element = element.wrapAll(wrapper).parent();
                    }
                }
                return element;
            }
        });
        var ViewClone = kendo.Class.extend({
            init: function (view) {
                $.extend(this, {
                    element: view.element.clone(true),
                    transition: view.transition,
                    id: view.id
                });
                view.element.parent().append(this.element);
            },
            hideEnd: function () {
                this.element.remove();
            },
            beforeTransition: $.noop,
            afterTransition: $.noop
        });
        var Layout = View.extend({
            init: function (content, options) {
                View.fn.init.call(this, content, options);
                this.containers = {};
            },
            container: function (selector) {
                var container = this.containers[selector];
                if (!container) {
                    container = this._createContainer(selector);
                    this.containers[selector] = container;
                }
                return container;
            },
            showIn: function (selector, view, transition) {
                this.container(selector).show(view, transition);
            },
            _createContainer: function (selector) {
                var root = this.render(), element = root.find(selector), container;
                if (!element.length && root.is(selector)) {
                    if (root.is(selector)) {
                        element = root;
                    } else {
                        throw new Error('can\'t find a container with the specified ' + selector + ' selector');
                    }
                }
                container = new ViewContainer(element);
                container.bind('accepted', function (e) {
                    e.view.render(element);
                });
                return container;
            }
        });
        var Fragment = View.extend({
            attach: function (view, placeholder) {
                view.element.find(placeholder).replaceWith(this.render());
            },
            detach: function () {
            }
        });
        var transitionRegExp = /^(\w+)(:(\w+))?( (\w+))?$/;
        function parseTransition(transition) {
            if (!transition) {
                return {};
            }
            var matches = transition.match(transitionRegExp) || [];
            return {
                type: matches[1],
                direction: matches[3],
                reverse: matches[5] === 'reverse'
            };
        }
        var ViewContainer = Observable.extend({
            init: function (container) {
                Observable.fn.init.call(this);
                this.container = container;
                this.history = [];
                this.view = null;
                this.running = false;
            },
            after: function () {
                this.running = false;
                this.trigger('complete', { view: this.view });
                this.trigger('after');
            },
            end: function () {
                this.view.showEnd();
                this.previous.hideEnd();
                this.after();
            },
            show: function (view, transition, locationID) {
                if (!view.triggerBeforeShow() || this.view && !this.view.triggerBeforeHide()) {
                    this.trigger('after');
                    return false;
                }
                locationID = locationID || view.id;
                var that = this, current = view === that.view ? view.clone() : that.view, history = that.history, previousEntry = history[history.length - 2] || {}, back = previousEntry.id === locationID, theTransition = transition || (back ? history[history.length - 1].transition : view.transition), transitionData = parseTransition(theTransition);
                if (that.running) {
                    that.effect.stop();
                }
                if (theTransition === 'none') {
                    theTransition = null;
                }
                that.trigger('accepted', { view: view });
                that.view = view;
                that.previous = current;
                that.running = true;
                if (!back) {
                    history.push({
                        id: locationID,
                        transition: theTransition
                    });
                } else {
                    history.pop();
                }
                if (!current) {
                    view.showStart();
                    view.showEnd();
                    that.after();
                    return true;
                }
                if (!theTransition || !kendo.effects.enabled) {
                    view.showStart();
                    that.end();
                } else {
                    view.element.addClass('k-fx-hidden');
                    view.showStart();
                    if (back && !transition) {
                        transitionData.reverse = !transitionData.reverse;
                    }
                    that.effect = kendo.fx(view.element).replace(current.element, transitionData.type).beforeTransition(function () {
                        view.beforeTransition('show');
                        current.beforeTransition('hide');
                    }).afterTransition(function () {
                        view.afterTransition('show');
                        current.afterTransition('hide');
                    }).direction(transitionData.direction).setReverse(transitionData.reverse);
                    that.effect.run().then(function () {
                        that.end();
                    });
                }
                return true;
            }
        });
        kendo.ViewContainer = ViewContainer;
        kendo.Fragment = Fragment;
        kendo.Layout = Layout;
        kendo.View = View;
        kendo.ViewClone = ViewClone;
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.userevents', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'userevents',
        name: 'User Events',
        category: 'framework',
        depends: ['core'],
        hidden: true
    };
    (function ($, undefined) {
        var kendo = window.kendo, support = kendo.support, Class = kendo.Class, Observable = kendo.Observable, now = $.now, extend = $.extend, OS = support.mobileOS, invalidZeroEvents = OS && OS.android, DEFAULT_MIN_HOLD = 800, DEFAULT_THRESHOLD = support.browser.msie ? 5 : 0, PRESS = 'press', HOLD = 'hold', SELECT = 'select', START = 'start', MOVE = 'move', END = 'end', CANCEL = 'cancel', TAP = 'tap', RELEASE = 'release', GESTURESTART = 'gesturestart', GESTURECHANGE = 'gesturechange', GESTUREEND = 'gestureend', GESTURETAP = 'gesturetap';
        var THRESHOLD = {
            'api': 0,
            'touch': 0,
            'mouse': 9,
            'pointer': 9
        };
        var ENABLE_GLOBAL_SURFACE = !support.touch || support.mouseAndTouchPresent;
        function touchDelta(touch1, touch2) {
            var x1 = touch1.x.location, y1 = touch1.y.location, x2 = touch2.x.location, y2 = touch2.y.location, dx = x1 - x2, dy = y1 - y2;
            return {
                center: {
                    x: (x1 + x2) / 2,
                    y: (y1 + y2) / 2
                },
                distance: Math.sqrt(dx * dx + dy * dy)
            };
        }
        function getTouches(e) {
            var touches = [], originalEvent = e.originalEvent, currentTarget = e.currentTarget, idx = 0, length, changedTouches, touch;
            if (e.api) {
                touches.push({
                    id: 2,
                    event: e,
                    target: e.target,
                    currentTarget: e.target,
                    location: e,
                    type: 'api'
                });
            } else if (e.type.match(/touch/)) {
                changedTouches = originalEvent ? originalEvent.changedTouches : [];
                for (length = changedTouches.length; idx < length; idx++) {
                    touch = changedTouches[idx];
                    touches.push({
                        location: touch,
                        event: e,
                        target: touch.target,
                        currentTarget: currentTarget,
                        id: touch.identifier,
                        type: 'touch'
                    });
                }
            } else if (support.pointers || support.msPointers) {
                touches.push({
                    location: originalEvent,
                    event: e,
                    target: e.target,
                    currentTarget: currentTarget,
                    id: originalEvent.pointerId,
                    type: 'pointer'
                });
            } else {
                touches.push({
                    id: 1,
                    event: e,
                    target: e.target,
                    currentTarget: currentTarget,
                    location: e,
                    type: 'mouse'
                });
            }
            return touches;
        }
        var TouchAxis = Class.extend({
            init: function (axis, location) {
                var that = this;
                that.axis = axis;
                that._updateLocationData(location);
                that.startLocation = that.location;
                that.velocity = that.delta = 0;
                that.timeStamp = now();
            },
            move: function (location) {
                var that = this, offset = location['page' + that.axis], timeStamp = now(), timeDelta = timeStamp - that.timeStamp || 1;
                if (!offset && invalidZeroEvents) {
                    return;
                }
                that.delta = offset - that.location;
                that._updateLocationData(location);
                that.initialDelta = offset - that.startLocation;
                that.velocity = that.delta / timeDelta;
                that.timeStamp = timeStamp;
            },
            _updateLocationData: function (location) {
                var that = this, axis = that.axis;
                that.location = location['page' + axis];
                that.client = location['client' + axis];
                that.screen = location['screen' + axis];
            }
        });
        var Touch = Class.extend({
            init: function (userEvents, target, touchInfo) {
                extend(this, {
                    x: new TouchAxis('X', touchInfo.location),
                    y: new TouchAxis('Y', touchInfo.location),
                    type: touchInfo.type,
                    useClickAsTap: userEvents.useClickAsTap,
                    threshold: userEvents.threshold || THRESHOLD[touchInfo.type],
                    userEvents: userEvents,
                    target: target,
                    currentTarget: touchInfo.currentTarget,
                    initialTouch: touchInfo.target,
                    id: touchInfo.id,
                    pressEvent: touchInfo,
                    _moved: false,
                    _finished: false
                });
            },
            press: function () {
                this._holdTimeout = setTimeout($.proxy(this, '_hold'), this.userEvents.minHold);
                this._trigger(PRESS, this.pressEvent);
            },
            _hold: function () {
                this._trigger(HOLD, this.pressEvent);
            },
            move: function (touchInfo) {
                var that = this;
                if (that._finished) {
                    return;
                }
                that.x.move(touchInfo.location);
                that.y.move(touchInfo.location);
                if (!that._moved) {
                    if (that._withinIgnoreThreshold()) {
                        return;
                    }
                    if (!UserEvents.current || UserEvents.current === that.userEvents) {
                        that._start(touchInfo);
                    } else {
                        return that.dispose();
                    }
                }
                if (!that._finished) {
                    that._trigger(MOVE, touchInfo);
                }
            },
            end: function (touchInfo) {
                this.endTime = now();
                if (this._finished) {
                    return;
                }
                this._finished = true;
                this._trigger(RELEASE, touchInfo);
                if (this._moved) {
                    this._trigger(END, touchInfo);
                } else {
                    if (!this.useClickAsTap) {
                        this._trigger(TAP, touchInfo);
                    }
                }
                clearTimeout(this._holdTimeout);
                this.dispose();
            },
            dispose: function () {
                var userEvents = this.userEvents, activeTouches = userEvents.touches;
                this._finished = true;
                this.pressEvent = null;
                clearTimeout(this._holdTimeout);
                activeTouches.splice($.inArray(this, activeTouches), 1);
            },
            skip: function () {
                this.dispose();
            },
            cancel: function () {
                this.dispose();
            },
            isMoved: function () {
                return this._moved;
            },
            _start: function (touchInfo) {
                clearTimeout(this._holdTimeout);
                this.startTime = now();
                this._moved = true;
                this._trigger(START, touchInfo);
            },
            _trigger: function (name, touchInfo) {
                var that = this, jQueryEvent = touchInfo.event, data = {
                        touch: that,
                        x: that.x,
                        y: that.y,
                        target: that.target,
                        event: jQueryEvent
                    };
                if (that.userEvents.notify(name, data)) {
                    jQueryEvent.preventDefault();
                }
            },
            _withinIgnoreThreshold: function () {
                var xDelta = this.x.initialDelta, yDelta = this.y.initialDelta;
                return Math.sqrt(xDelta * xDelta + yDelta * yDelta) <= this.threshold;
            }
        });
        function withEachUpEvent(callback) {
            var downEvents = kendo.eventMap.up.split(' '), idx = 0, length = downEvents.length;
            for (; idx < length; idx++) {
                callback(downEvents[idx]);
            }
        }
        var UserEvents = Observable.extend({
            init: function (element, options) {
                var that = this, filter, ns = kendo.guid();
                options = options || {};
                filter = that.filter = options.filter;
                that.threshold = options.threshold || DEFAULT_THRESHOLD;
                that.minHold = options.minHold || DEFAULT_MIN_HOLD;
                that.touches = [];
                that._maxTouches = options.multiTouch ? 2 : 1;
                that.allowSelection = options.allowSelection;
                that.captureUpIfMoved = options.captureUpIfMoved;
                that.useClickAsTap = !options.fastTap && !support.delayedClick();
                that.eventNS = ns;
                element = $(element).handler(that);
                Observable.fn.init.call(that);
                extend(that, {
                    element: element,
                    surface: options.global && ENABLE_GLOBAL_SURFACE ? $(element[0].ownerDocument.documentElement) : $(options.surface || element),
                    stopPropagation: options.stopPropagation,
                    pressed: false
                });
                that.surface.handler(that).on(kendo.applyEventMap('move', ns), '_move').on(kendo.applyEventMap('up cancel', ns), '_end');
                element.on(kendo.applyEventMap('down', ns), filter, '_start');
                if (that.useClickAsTap) {
                    element.on(kendo.applyEventMap('click', ns), filter, '_click');
                }
                if (support.pointers || support.msPointers) {
                    if (support.browser.version < 11) {
                        var defaultAction = 'pinch-zoom double-tap-zoom';
                        element.css('-ms-touch-action', options.touchAction && options.touchAction != 'none' ? defaultAction + ' ' + options.touchAction : defaultAction);
                    } else {
                        element.css('touch-action', options.touchAction || 'none');
                    }
                }
                if (options.preventDragEvent) {
                    element.on(kendo.applyEventMap('dragstart', ns), kendo.preventDefault);
                }
                element.on(kendo.applyEventMap('mousedown', ns), filter, { root: element }, '_select');
                if (that.captureUpIfMoved && support.eventCapture) {
                    var surfaceElement = that.surface[0], preventIfMovingProxy = $.proxy(that.preventIfMoving, that);
                    withEachUpEvent(function (eventName) {
                        surfaceElement.addEventListener(eventName, preventIfMovingProxy, true);
                    });
                }
                that.bind([
                    PRESS,
                    HOLD,
                    TAP,
                    START,
                    MOVE,
                    END,
                    RELEASE,
                    CANCEL,
                    GESTURESTART,
                    GESTURECHANGE,
                    GESTUREEND,
                    GESTURETAP,
                    SELECT
                ], options);
            },
            preventIfMoving: function (e) {
                if (this._isMoved()) {
                    e.preventDefault();
                }
            },
            destroy: function () {
                var that = this;
                if (that._destroyed) {
                    return;
                }
                that._destroyed = true;
                if (that.captureUpIfMoved && support.eventCapture) {
                    var surfaceElement = that.surface[0];
                    withEachUpEvent(function (eventName) {
                        surfaceElement.removeEventListener(eventName, that.preventIfMoving);
                    });
                }
                that.element.kendoDestroy(that.eventNS);
                that.surface.kendoDestroy(that.eventNS);
                that.element.removeData('handler');
                that.surface.removeData('handler');
                that._disposeAll();
                that.unbind();
                delete that.surface;
                delete that.element;
                delete that.currentTarget;
            },
            capture: function () {
                UserEvents.current = this;
            },
            cancel: function () {
                this._disposeAll();
                this.trigger(CANCEL);
            },
            notify: function (eventName, data) {
                var that = this, touches = that.touches;
                if (this._isMultiTouch()) {
                    switch (eventName) {
                    case MOVE:
                        eventName = GESTURECHANGE;
                        break;
                    case END:
                        eventName = GESTUREEND;
                        break;
                    case TAP:
                        eventName = GESTURETAP;
                        break;
                    }
                    extend(data, { touches: touches }, touchDelta(touches[0], touches[1]));
                }
                return this.trigger(eventName, extend(data, { type: eventName }));
            },
            press: function (x, y, target) {
                this._apiCall('_start', x, y, target);
            },
            move: function (x, y) {
                this._apiCall('_move', x, y);
            },
            end: function (x, y) {
                this._apiCall('_end', x, y);
            },
            _isMultiTouch: function () {
                return this.touches.length > 1;
            },
            _maxTouchesReached: function () {
                return this.touches.length >= this._maxTouches;
            },
            _disposeAll: function () {
                var touches = this.touches;
                while (touches.length > 0) {
                    touches.pop().dispose();
                }
            },
            _isMoved: function () {
                return $.grep(this.touches, function (touch) {
                    return touch.isMoved();
                }).length;
            },
            _select: function (e) {
                if (!this.allowSelection || this.trigger(SELECT, { event: e })) {
                    e.preventDefault();
                }
            },
            _start: function (e) {
                var that = this, idx = 0, filter = that.filter, target, touches = getTouches(e), length = touches.length, touch, which = e.which;
                if (which && which > 1 || that._maxTouchesReached()) {
                    return;
                }
                UserEvents.current = null;
                that.currentTarget = e.currentTarget;
                if (that.stopPropagation) {
                    e.stopPropagation();
                }
                for (; idx < length; idx++) {
                    if (that._maxTouchesReached()) {
                        break;
                    }
                    touch = touches[idx];
                    if (filter) {
                        target = $(touch.currentTarget);
                    } else {
                        target = that.element;
                    }
                    if (!target.length) {
                        continue;
                    }
                    touch = new Touch(that, target, touch);
                    that.touches.push(touch);
                    touch.press();
                    if (that._isMultiTouch()) {
                        that.notify('gesturestart', {});
                    }
                }
            },
            _move: function (e) {
                this._eachTouch('move', e);
            },
            _end: function (e) {
                this._eachTouch('end', e);
            },
            _click: function (e) {
                var data = {
                    touch: {
                        initialTouch: e.target,
                        target: $(e.currentTarget),
                        endTime: now(),
                        x: {
                            location: e.pageX,
                            client: e.clientX
                        },
                        y: {
                            location: e.pageY,
                            client: e.clientY
                        }
                    },
                    x: e.pageX,
                    y: e.pageY,
                    target: $(e.currentTarget),
                    event: e,
                    type: 'tap'
                };
                if (this.trigger('tap', data)) {
                    e.preventDefault();
                }
            },
            _eachTouch: function (methodName, e) {
                var that = this, dict = {}, touches = getTouches(e), activeTouches = that.touches, idx, touch, touchInfo, matchingTouch;
                for (idx = 0; idx < activeTouches.length; idx++) {
                    touch = activeTouches[idx];
                    dict[touch.id] = touch;
                }
                for (idx = 0; idx < touches.length; idx++) {
                    touchInfo = touches[idx];
                    matchingTouch = dict[touchInfo.id];
                    if (matchingTouch) {
                        matchingTouch[methodName](touchInfo);
                    }
                }
            },
            _apiCall: function (type, x, y, target) {
                this[type]({
                    api: true,
                    pageX: x,
                    pageY: y,
                    clientX: x,
                    clientY: y,
                    target: $(target || this.element)[0],
                    stopPropagation: $.noop,
                    preventDefault: $.noop
                });
            }
        });
        UserEvents.defaultThreshold = function (value) {
            DEFAULT_THRESHOLD = value;
        };
        UserEvents.minHold = function (value) {
            DEFAULT_MIN_HOLD = value;
        };
        kendo.getTouches = getTouches;
        kendo.touchDelta = touchDelta;
        kendo.UserEvents = UserEvents;
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.draganddrop', [
        'kendo.core',
        'kendo.userevents'
    ], f);
}(function () {
    var __meta__ = {
        id: 'draganddrop',
        name: 'Drag & drop',
        category: 'framework',
        description: 'Drag & drop functionality for any DOM element.',
        depends: [
            'core',
            'userevents'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, support = kendo.support, document = window.document, $window = $(window), Class = kendo.Class, Widget = kendo.ui.Widget, Observable = kendo.Observable, UserEvents = kendo.UserEvents, proxy = $.proxy, extend = $.extend, getOffset = kendo.getOffset, draggables = {}, dropTargets = {}, dropAreas = {}, lastDropTarget, elementUnderCursor = kendo.elementUnderCursor, KEYUP = 'keyup', CHANGE = 'change', DRAGSTART = 'dragstart', HOLD = 'hold', DRAG = 'drag', DRAGEND = 'dragend', DRAGCANCEL = 'dragcancel', HINTDESTROYED = 'hintDestroyed', DRAGENTER = 'dragenter', DRAGLEAVE = 'dragleave', DROP = 'drop';
        function contains(parent, child) {
            try {
                return $.contains(parent, child) || parent == child;
            } catch (e) {
                return false;
            }
        }
        function numericCssPropery(element, property) {
            return parseInt(element.css(property), 10) || 0;
        }
        function within(value, range) {
            return Math.min(Math.max(value, range.min), range.max);
        }
        function containerBoundaries(container, element) {
            var offset = getOffset(container), outerWidth = kendo._outerWidth, outerHeight = kendo._outerHeight, minX = offset.left + numericCssPropery(container, 'borderLeftWidth') + numericCssPropery(container, 'paddingLeft'), minY = offset.top + numericCssPropery(container, 'borderTopWidth') + numericCssPropery(container, 'paddingTop'), maxX = minX + container.width() - outerWidth(element, true), maxY = minY + container.height() - outerHeight(element, true);
            return {
                x: {
                    min: minX,
                    max: maxX
                },
                y: {
                    min: minY,
                    max: maxY
                }
            };
        }
        function checkTarget(target, targets, areas) {
            var theTarget, theFilter, i = 0, targetLen = targets && targets.length, areaLen = areas && areas.length;
            while (target && target.parentNode) {
                for (i = 0; i < targetLen; i++) {
                    theTarget = targets[i];
                    if (theTarget.element[0] === target) {
                        return {
                            target: theTarget,
                            targetElement: target
                        };
                    }
                }
                for (i = 0; i < areaLen; i++) {
                    theFilter = areas[i];
                    if ($.contains(theFilter.element[0], target) && support.matchesSelector.call(target, theFilter.options.filter)) {
                        return {
                            target: theFilter,
                            targetElement: target
                        };
                    }
                }
                target = target.parentNode;
            }
            return undefined;
        }
        var TapCapture = Observable.extend({
            init: function (element, options) {
                var that = this, domElement = element[0];
                that.capture = false;
                if (domElement.addEventListener) {
                    $.each(kendo.eventMap.down.split(' '), function () {
                        domElement.addEventListener(this, proxy(that._press, that), true);
                    });
                    $.each(kendo.eventMap.up.split(' '), function () {
                        domElement.addEventListener(this, proxy(that._release, that), true);
                    });
                } else {
                    $.each(kendo.eventMap.down.split(' '), function () {
                        domElement.attachEvent(this, proxy(that._press, that));
                    });
                    $.each(kendo.eventMap.up.split(' '), function () {
                        domElement.attachEvent(this, proxy(that._release, that));
                    });
                }
                Observable.fn.init.call(that);
                that.bind([
                    'press',
                    'release'
                ], options || {});
            },
            captureNext: function () {
                this.capture = true;
            },
            cancelCapture: function () {
                this.capture = false;
            },
            _press: function (e) {
                var that = this;
                that.trigger('press');
                if (that.capture) {
                    e.preventDefault();
                }
            },
            _release: function (e) {
                var that = this;
                that.trigger('release');
                if (that.capture) {
                    e.preventDefault();
                    that.cancelCapture();
                }
            }
        });
        var PaneDimension = Observable.extend({
            init: function (options) {
                var that = this;
                Observable.fn.init.call(that);
                that.forcedEnabled = false;
                $.extend(that, options);
                that.scale = 1;
                if (that.horizontal) {
                    that.measure = 'offsetWidth';
                    that.scrollSize = 'scrollWidth';
                    that.axis = 'x';
                } else {
                    that.measure = 'offsetHeight';
                    that.scrollSize = 'scrollHeight';
                    that.axis = 'y';
                }
            },
            makeVirtual: function () {
                $.extend(this, {
                    virtual: true,
                    forcedEnabled: true,
                    _virtualMin: 0,
                    _virtualMax: 0
                });
            },
            virtualSize: function (min, max) {
                if (this._virtualMin !== min || this._virtualMax !== max) {
                    this._virtualMin = min;
                    this._virtualMax = max;
                    this.update();
                }
            },
            outOfBounds: function (offset) {
                return offset > this.max || offset < this.min;
            },
            forceEnabled: function () {
                this.forcedEnabled = true;
            },
            getSize: function () {
                return this.container[0][this.measure];
            },
            getTotal: function () {
                return this.element[0][this.scrollSize];
            },
            rescale: function (scale) {
                this.scale = scale;
            },
            update: function (silent) {
                var that = this, total = that.virtual ? that._virtualMax : that.getTotal(), scaledTotal = total * that.scale, size = that.getSize();
                if (total === 0 && !that.forcedEnabled) {
                    return;
                }
                that.max = that.virtual ? -that._virtualMin : 0;
                that.size = size;
                that.total = scaledTotal;
                that.min = Math.min(that.max, size - scaledTotal);
                that.minScale = size / total;
                that.centerOffset = (scaledTotal - size) / 2;
                that.enabled = that.forcedEnabled || scaledTotal > size;
                if (!silent) {
                    that.trigger(CHANGE, that);
                }
            }
        });
        var PaneDimensions = Observable.extend({
            init: function (options) {
                var that = this;
                Observable.fn.init.call(that);
                that.x = new PaneDimension(extend({ horizontal: true }, options));
                that.y = new PaneDimension(extend({ horizontal: false }, options));
                that.container = options.container;
                that.forcedMinScale = options.minScale;
                that.maxScale = options.maxScale || 100;
                that.bind(CHANGE, options);
            },
            rescale: function (newScale) {
                this.x.rescale(newScale);
                this.y.rescale(newScale);
                this.refresh();
            },
            centerCoordinates: function () {
                return {
                    x: Math.min(0, -this.x.centerOffset),
                    y: Math.min(0, -this.y.centerOffset)
                };
            },
            refresh: function () {
                var that = this;
                that.x.update();
                that.y.update();
                that.enabled = that.x.enabled || that.y.enabled;
                that.minScale = that.forcedMinScale || Math.min(that.x.minScale, that.y.minScale);
                that.fitScale = Math.max(that.x.minScale, that.y.minScale);
                that.trigger(CHANGE);
            }
        });
        var PaneAxis = Observable.extend({
            init: function (options) {
                var that = this;
                extend(that, options);
                Observable.fn.init.call(that);
            },
            outOfBounds: function () {
                return this.dimension.outOfBounds(this.movable[this.axis]);
            },
            dragMove: function (delta) {
                var that = this, dimension = that.dimension, axis = that.axis, movable = that.movable, position = movable[axis] + delta;
                if (!dimension.enabled) {
                    return;
                }
                if (position < dimension.min && delta < 0 || position > dimension.max && delta > 0) {
                    delta *= that.resistance;
                }
                movable.translateAxis(axis, delta);
                that.trigger(CHANGE, that);
            }
        });
        var Pane = Class.extend({
            init: function (options) {
                var that = this, x, y, resistance, movable;
                extend(that, { elastic: true }, options);
                resistance = that.elastic ? 0.5 : 0;
                movable = that.movable;
                that.x = x = new PaneAxis({
                    axis: 'x',
                    dimension: that.dimensions.x,
                    resistance: resistance,
                    movable: movable
                });
                that.y = y = new PaneAxis({
                    axis: 'y',
                    dimension: that.dimensions.y,
                    resistance: resistance,
                    movable: movable
                });
                that.userEvents.bind([
                    'press',
                    'move',
                    'end',
                    'gesturestart',
                    'gesturechange'
                ], {
                    gesturestart: function (e) {
                        that.gesture = e;
                        that.offset = that.dimensions.container.offset();
                    },
                    press: function (e) {
                        if ($(e.event.target).closest('a').is('[data-navigate-on-press=true]')) {
                            e.sender.cancel();
                        }
                    },
                    gesturechange: function (e) {
                        var previousGesture = that.gesture, previousCenter = previousGesture.center, center = e.center, scaleDelta = e.distance / previousGesture.distance, minScale = that.dimensions.minScale, maxScale = that.dimensions.maxScale, coordinates;
                        if (movable.scale <= minScale && scaleDelta < 1) {
                            scaleDelta += (1 - scaleDelta) * 0.8;
                        }
                        if (movable.scale * scaleDelta >= maxScale) {
                            scaleDelta = maxScale / movable.scale;
                        }
                        var offsetX = movable.x + that.offset.left, offsetY = movable.y + that.offset.top;
                        coordinates = {
                            x: (offsetX - previousCenter.x) * scaleDelta + center.x - offsetX,
                            y: (offsetY - previousCenter.y) * scaleDelta + center.y - offsetY
                        };
                        movable.scaleWith(scaleDelta);
                        x.dragMove(coordinates.x);
                        y.dragMove(coordinates.y);
                        that.dimensions.rescale(movable.scale);
                        that.gesture = e;
                        e.preventDefault();
                    },
                    move: function (e) {
                        if (e.event.target.tagName.match(/textarea|input/i)) {
                            return;
                        }
                        if (x.dimension.enabled || y.dimension.enabled) {
                            x.dragMove(e.x.delta);
                            y.dragMove(e.y.delta);
                            e.preventDefault();
                        } else {
                            e.touch.skip();
                        }
                    },
                    end: function (e) {
                        e.preventDefault();
                    }
                });
            }
        });
        var TRANSFORM_STYLE = support.transitions.prefix + 'Transform', translate;
        if (support.hasHW3D) {
            translate = function (x, y, scale) {
                return 'translate3d(' + x + 'px,' + y + 'px,0) scale(' + scale + ')';
            };
        } else {
            translate = function (x, y, scale) {
                return 'translate(' + x + 'px,' + y + 'px) scale(' + scale + ')';
            };
        }
        var Movable = Observable.extend({
            init: function (element) {
                var that = this;
                Observable.fn.init.call(that);
                that.element = $(element);
                that.element[0].style.webkitTransformOrigin = 'left top';
                that.x = 0;
                that.y = 0;
                that.scale = 1;
                that._saveCoordinates(translate(that.x, that.y, that.scale));
            },
            translateAxis: function (axis, by) {
                this[axis] += by;
                this.refresh();
            },
            scaleTo: function (scale) {
                this.scale = scale;
                this.refresh();
            },
            scaleWith: function (scaleDelta) {
                this.scale *= scaleDelta;
                this.refresh();
            },
            translate: function (coordinates) {
                this.x += coordinates.x;
                this.y += coordinates.y;
                this.refresh();
            },
            moveAxis: function (axis, value) {
                this[axis] = value;
                this.refresh();
            },
            moveTo: function (coordinates) {
                extend(this, coordinates);
                this.refresh();
            },
            refresh: function () {
                var that = this, x = that.x, y = that.y, newCoordinates;
                if (that.round) {
                    x = Math.round(x);
                    y = Math.round(y);
                }
                newCoordinates = translate(x, y, that.scale);
                if (newCoordinates != that.coordinates) {
                    if (kendo.support.browser.msie && kendo.support.browser.version < 10) {
                        that.element[0].style.position = 'absolute';
                        that.element[0].style.left = that.x + 'px';
                        that.element[0].style.top = that.y + 'px';
                    } else {
                        that.element[0].style[TRANSFORM_STYLE] = newCoordinates;
                    }
                    that._saveCoordinates(newCoordinates);
                    that.trigger(CHANGE);
                }
            },
            _saveCoordinates: function (coordinates) {
                this.coordinates = coordinates;
            }
        });
        function destroyDroppable(collection, widget) {
            var groupName = widget.options.group, droppables = collection[groupName], i;
            Widget.fn.destroy.call(widget);
            if (droppables.length > 1) {
                for (i = 0; i < droppables.length; i++) {
                    if (droppables[i] == widget) {
                        droppables.splice(i, 1);
                        break;
                    }
                }
            } else {
                droppables.length = 0;
                delete collection[groupName];
            }
        }
        var DropTarget = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                var group = that.options.group;
                if (!(group in dropTargets)) {
                    dropTargets[group] = [that];
                } else {
                    dropTargets[group].push(that);
                }
            },
            events: [
                DRAGENTER,
                DRAGLEAVE,
                DROP
            ],
            options: {
                name: 'DropTarget',
                group: 'default'
            },
            destroy: function () {
                destroyDroppable(dropTargets, this);
            },
            _trigger: function (eventName, e) {
                var that = this, draggable = draggables[that.options.group];
                if (draggable) {
                    return that.trigger(eventName, extend({}, e.event, {
                        draggable: draggable,
                        dropTarget: e.dropTarget
                    }));
                }
            },
            _over: function (e) {
                this._trigger(DRAGENTER, e);
            },
            _out: function (e) {
                this._trigger(DRAGLEAVE, e);
            },
            _drop: function (e) {
                var that = this, draggable = draggables[that.options.group];
                if (draggable) {
                    draggable.dropped = !that._trigger(DROP, e);
                }
            }
        });
        DropTarget.destroyGroup = function (groupName) {
            var group = dropTargets[groupName] || dropAreas[groupName], i;
            if (group) {
                for (i = 0; i < group.length; i++) {
                    Widget.fn.destroy.call(group[i]);
                }
                group.length = 0;
                delete dropTargets[groupName];
                delete dropAreas[groupName];
            }
        };
        DropTarget._cache = dropTargets;
        var DropTargetArea = DropTarget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                var group = that.options.group;
                if (!(group in dropAreas)) {
                    dropAreas[group] = [that];
                } else {
                    dropAreas[group].push(that);
                }
            },
            destroy: function () {
                destroyDroppable(dropAreas, this);
            },
            options: {
                name: 'DropTargetArea',
                group: 'default',
                filter: null
            }
        });
        var Draggable = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                that._activated = false;
                that.userEvents = new UserEvents(that.element, {
                    global: true,
                    allowSelection: true,
                    filter: that.options.filter,
                    threshold: that.options.distance,
                    start: proxy(that._start, that),
                    hold: proxy(that._hold, that),
                    move: proxy(that._drag, that),
                    end: proxy(that._end, that),
                    cancel: proxy(that._cancel, that),
                    select: proxy(that._select, that)
                });
                that._afterEndHandler = proxy(that._afterEnd, that);
                that._captureEscape = proxy(that._captureEscape, that);
            },
            events: [
                HOLD,
                DRAGSTART,
                DRAG,
                DRAGEND,
                DRAGCANCEL,
                HINTDESTROYED
            ],
            options: {
                name: 'Draggable',
                distance: kendo.support.touch ? 0 : 5,
                group: 'default',
                cursorOffset: null,
                axis: null,
                container: null,
                filter: null,
                ignore: null,
                holdToDrag: false,
                autoScroll: false,
                dropped: false
            },
            cancelHold: function () {
                this._activated = false;
            },
            _captureEscape: function (e) {
                var that = this;
                if (e.keyCode === kendo.keys.ESC) {
                    that._trigger(DRAGCANCEL, { event: e });
                    that.userEvents.cancel();
                }
            },
            _updateHint: function (e) {
                var that = this, coordinates, options = that.options, boundaries = that.boundaries, axis = options.axis, cursorOffset = that.options.cursorOffset;
                if (cursorOffset) {
                    coordinates = {
                        left: e.x.location + cursorOffset.left,
                        top: e.y.location + cursorOffset.top
                    };
                } else {
                    that.hintOffset.left += e.x.delta;
                    that.hintOffset.top += e.y.delta;
                    coordinates = $.extend({}, that.hintOffset);
                }
                if (boundaries) {
                    coordinates.top = within(coordinates.top, boundaries.y);
                    coordinates.left = within(coordinates.left, boundaries.x);
                }
                if (axis === 'x') {
                    delete coordinates.top;
                } else if (axis === 'y') {
                    delete coordinates.left;
                }
                that.hint.css(coordinates);
            },
            _shouldIgnoreTarget: function (target) {
                var ignoreSelector = this.options.ignore;
                return ignoreSelector && $(target).is(ignoreSelector);
            },
            _select: function (e) {
                if (!this._shouldIgnoreTarget(e.event.target)) {
                    e.preventDefault();
                }
            },
            _start: function (e) {
                var that = this, options = that.options, container = options.container ? $(options.container) : null, hint = options.hint;
                if (this._shouldIgnoreTarget(e.touch.initialTouch) || options.holdToDrag && !that._activated) {
                    that.userEvents.cancel();
                    return;
                }
                that.currentTarget = e.target;
                that.currentTargetOffset = getOffset(that.currentTarget);
                if (hint) {
                    if (that.hint) {
                        that.hint.stop(true, true).remove();
                    }
                    that.hint = kendo.isFunction(hint) ? $(hint.call(that, that.currentTarget)) : hint;
                    var offset = getOffset(that.currentTarget);
                    that.hintOffset = offset;
                    that.hint.css({
                        position: 'absolute',
                        zIndex: 20000,
                        left: offset.left,
                        top: offset.top
                    }).appendTo(document.body);
                    that.angular('compile', function () {
                        that.hint.removeAttr('ng-repeat');
                        var scopeTarget = $(e.target);
                        while (!scopeTarget.data('$$kendoScope') && scopeTarget.length) {
                            scopeTarget = scopeTarget.parent();
                        }
                        return {
                            elements: that.hint.get(),
                            scopeFrom: scopeTarget.data('$$kendoScope')
                        };
                    });
                }
                draggables[options.group] = that;
                that.dropped = false;
                if (container) {
                    that.boundaries = containerBoundaries(container, that.hint);
                }
                $(document).on(KEYUP, that._captureEscape);
                if (that._trigger(DRAGSTART, e)) {
                    that.userEvents.cancel();
                    that._afterEnd();
                }
                that.userEvents.capture();
            },
            _hold: function (e) {
                this.currentTarget = e.target;
                if (this._trigger(HOLD, e)) {
                    this.userEvents.cancel();
                } else {
                    this._activated = true;
                }
            },
            _drag: function (e) {
                e.preventDefault();
                var cursorElement = this._elementUnderCursor(e);
                if (this.options.autoScroll && this._cursorElement !== cursorElement) {
                    this._scrollableParent = findScrollableParent(cursorElement);
                    this._cursorElement = cursorElement;
                }
                this._lastEvent = e;
                this._processMovement(e, cursorElement);
                if (this.options.autoScroll) {
                    if (this._scrollableParent[0]) {
                        var velocity = autoScrollVelocity(e.x.location, e.y.location, scrollableViewPort(this._scrollableParent));
                        this._scrollCompenstation = $.extend({}, this.hintOffset);
                        this._scrollVelocity = velocity;
                        if (velocity.y === 0 && velocity.x === 0) {
                            clearInterval(this._scrollInterval);
                            this._scrollInterval = null;
                        } else if (!this._scrollInterval) {
                            this._scrollInterval = setInterval($.proxy(this, '_autoScroll'), 50);
                        }
                    }
                }
                if (this.hint) {
                    this._updateHint(e);
                }
            },
            _processMovement: function (e, cursorElement) {
                this._withDropTarget(cursorElement, function (target, targetElement) {
                    if (!target) {
                        if (lastDropTarget) {
                            lastDropTarget._trigger(DRAGLEAVE, extend(e, { dropTarget: $(lastDropTarget.targetElement) }));
                            lastDropTarget = null;
                        }
                        return;
                    }
                    if (lastDropTarget) {
                        if (targetElement === lastDropTarget.targetElement) {
                            return;
                        }
                        lastDropTarget._trigger(DRAGLEAVE, extend(e, { dropTarget: $(lastDropTarget.targetElement) }));
                    }
                    target._trigger(DRAGENTER, extend(e, { dropTarget: $(targetElement) }));
                    lastDropTarget = extend(target, { targetElement: targetElement });
                });
                this._trigger(DRAG, extend(e, {
                    dropTarget: lastDropTarget,
                    elementUnderCursor: cursorElement
                }));
            },
            _autoScroll: function () {
                var parent = this._scrollableParent[0], velocity = this._scrollVelocity, compensation = this._scrollCompenstation;
                if (!parent) {
                    return;
                }
                var cursorElement = this._elementUnderCursor(this._lastEvent);
                this._processMovement(this._lastEvent, cursorElement);
                var yIsScrollable, xIsScrollable;
                var isRootNode = parent === scrollableRoot()[0];
                if (isRootNode) {
                    yIsScrollable = document.body.scrollHeight > $window.height();
                    xIsScrollable = document.body.scrollWidth > $window.width();
                } else {
                    yIsScrollable = parent.offsetHeight <= parent.scrollHeight;
                    xIsScrollable = parent.offsetWidth <= parent.scrollWidth;
                }
                var yDelta = parent.scrollTop + velocity.y;
                var yInBounds = yIsScrollable && yDelta > 0 && yDelta < parent.scrollHeight;
                var xDelta = parent.scrollLeft + velocity.x;
                var xInBounds = xIsScrollable && xDelta > 0 && xDelta < parent.scrollWidth;
                if (yInBounds) {
                    parent.scrollTop += velocity.y;
                }
                if (xInBounds) {
                    parent.scrollLeft += velocity.x;
                }
                if (this.hint && isRootNode && (xInBounds || yInBounds)) {
                    if (yInBounds) {
                        compensation.top += velocity.y;
                    }
                    if (xInBounds) {
                        compensation.left += velocity.x;
                    }
                    this.hint.css(compensation);
                }
            },
            _end: function (e) {
                this._withDropTarget(this._elementUnderCursor(e), function (target, targetElement) {
                    if (target) {
                        target._drop(extend({}, e, { dropTarget: $(targetElement) }));
                        lastDropTarget = null;
                    }
                });
                this._cancel(this._trigger(DRAGEND, e));
            },
            _cancel: function (isDefaultPrevented) {
                var that = this;
                that._scrollableParent = null;
                this._cursorElement = null;
                clearInterval(this._scrollInterval);
                that._activated = false;
                if (that.hint && !that.dropped) {
                    setTimeout(function () {
                        that.hint.stop(true, true);
                        if (isDefaultPrevented) {
                            that._afterEndHandler();
                        } else {
                            that.hint.animate(that.currentTargetOffset, 'fast', that._afterEndHandler);
                        }
                    }, 0);
                } else {
                    that._afterEnd();
                }
            },
            _trigger: function (eventName, e) {
                var that = this;
                return that.trigger(eventName, extend({}, e.event, {
                    x: e.x,
                    y: e.y,
                    currentTarget: that.currentTarget,
                    initialTarget: e.touch ? e.touch.initialTouch : null,
                    dropTarget: e.dropTarget,
                    elementUnderCursor: e.elementUnderCursor
                }));
            },
            _elementUnderCursor: function (e) {
                var target = elementUnderCursor(e), hint = this.hint;
                if (hint && contains(hint[0], target)) {
                    hint.hide();
                    target = elementUnderCursor(e);
                    if (!target) {
                        target = elementUnderCursor(e);
                    }
                    hint.show();
                }
                return target;
            },
            _withDropTarget: function (element, callback) {
                var result, group = this.options.group, targets = dropTargets[group], areas = dropAreas[group];
                if (targets && targets.length || areas && areas.length) {
                    result = checkTarget(element, targets, areas);
                    if (result) {
                        callback(result.target, result.targetElement);
                    } else {
                        callback();
                    }
                }
            },
            destroy: function () {
                var that = this;
                Widget.fn.destroy.call(that);
                that._afterEnd();
                that.userEvents.destroy();
                this._scrollableParent = null;
                this._cursorElement = null;
                clearInterval(this._scrollInterval);
                that.currentTarget = null;
            },
            _afterEnd: function () {
                var that = this;
                if (that.hint) {
                    that.hint.remove();
                }
                delete draggables[that.options.group];
                that.trigger('destroy');
                that.trigger(HINTDESTROYED);
                $(document).off(KEYUP, that._captureEscape);
            }
        });
        kendo.ui.plugin(DropTarget);
        kendo.ui.plugin(DropTargetArea);
        kendo.ui.plugin(Draggable);
        kendo.TapCapture = TapCapture;
        kendo.containerBoundaries = containerBoundaries;
        extend(kendo.ui, {
            Pane: Pane,
            PaneDimensions: PaneDimensions,
            Movable: Movable
        });
        function scrollableViewPort(element) {
            var root = scrollableRoot()[0], offset, top, left;
            if (element[0] === root) {
                top = root.scrollTop;
                left = root.scrollLeft;
                return {
                    top: top,
                    left: left,
                    bottom: top + $window.height(),
                    right: left + $window.width()
                };
            } else {
                offset = element.offset();
                offset.bottom = offset.top + element.height();
                offset.right = offset.left + element.width();
                return offset;
            }
        }
        function scrollableRoot() {
            return $(kendo.support.browser.edge || kendo.support.browser.safari ? document.body : document.documentElement);
        }
        function findScrollableParent(element) {
            var root = scrollableRoot();
            if (!element || element === document.body || element === document.documentElement) {
                return root;
            }
            var parent = $(element)[0];
            while (parent && !kendo.isScrollable(parent) && parent !== document.body) {
                parent = parent.parentNode;
            }
            if (parent === document.body) {
                return root;
            }
            return $(parent);
        }
        function autoScrollVelocity(mouseX, mouseY, rect) {
            var velocity = {
                x: 0,
                y: 0
            };
            var AUTO_SCROLL_AREA = 50;
            if (mouseX - rect.left < AUTO_SCROLL_AREA) {
                velocity.x = -(AUTO_SCROLL_AREA - (mouseX - rect.left));
            } else if (rect.right - mouseX < AUTO_SCROLL_AREA) {
                velocity.x = AUTO_SCROLL_AREA - (rect.right - mouseX);
            }
            if (mouseY - rect.top < AUTO_SCROLL_AREA) {
                velocity.y = -(AUTO_SCROLL_AREA - (mouseY - rect.top));
            } else if (rect.bottom - mouseY < AUTO_SCROLL_AREA) {
                velocity.y = AUTO_SCROLL_AREA - (rect.bottom - mouseY);
            }
            return velocity;
        }
        kendo.ui.Draggable.utils = {
            autoScrollVelocity: autoScrollVelocity,
            scrollableViewPort: scrollableViewPort,
            findScrollableParent: findScrollableParent
        };
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.popup', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'popup',
        name: 'Pop-up',
        category: 'framework',
        depends: ['core'],
        advanced: true
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.ui, Widget = ui.Widget, Class = kendo.Class, support = kendo.support, getOffset = kendo.getOffset, outerWidth = kendo._outerWidth, outerHeight = kendo._outerHeight, OPEN = 'open', CLOSE = 'close', DEACTIVATE = 'deactivate', ACTIVATE = 'activate', CENTER = 'center', LEFT = 'left', RIGHT = 'right', TOP = 'top', BOTTOM = 'bottom', ABSOLUTE = 'absolute', HIDDEN = 'hidden', BODY = 'body', LOCATION = 'location', POSITION = 'position', VISIBLE = 'visible', EFFECTS = 'effects', ACTIVE = 'k-state-active', ACTIVEBORDER = 'k-state-border', ACTIVEBORDERREGEXP = /k-state-border-(\w+)/, ACTIVECHILDREN = '.k-picker-wrap, .k-dropdown-wrap, .k-link', MOUSEDOWN = 'down', DOCUMENT_ELEMENT = $(document.documentElement), proxy = $.proxy, WINDOW = $(window), SCROLL = 'scroll', cssPrefix = support.transitions.css, TRANSFORM = cssPrefix + 'transform', extend = $.extend, NS = '.kendoPopup', styles = [
                'font-size',
                'font-family',
                'font-stretch',
                'font-style',
                'font-weight',
                'line-height'
            ];
        function contains(container, target) {
            if (!container || !target) {
                return false;
            }
            return container === target || $.contains(container, target);
        }
        var Popup = Widget.extend({
            init: function (element, options) {
                var that = this, parentPopup;
                options = options || {};
                if (options.isRtl) {
                    options.origin = options.origin || BOTTOM + ' ' + RIGHT;
                    options.position = options.position || TOP + ' ' + RIGHT;
                }
                Widget.fn.init.call(that, element, options);
                element = that.element;
                options = that.options;
                that.collisions = options.collision ? options.collision.split(' ') : [];
                that.downEvent = kendo.applyEventMap(MOUSEDOWN, kendo.guid());
                if (that.collisions.length === 1) {
                    that.collisions.push(that.collisions[0]);
                }
                parentPopup = $(that.options.anchor).closest('.k-popup,.k-group').filter(':not([class^=km-])');
                options.appendTo = $($(options.appendTo)[0] || parentPopup[0] || document.body);
                that.element.hide().addClass('k-popup k-group k-reset').toggleClass('k-rtl', !!options.isRtl).css({ position: ABSOLUTE }).appendTo(options.appendTo).attr('aria-hidden', true).on('mouseenter' + NS, function () {
                    that._hovered = true;
                }).on('wheel' + NS, function (e) {
                    var list = $(e.target).find('.k-list');
                    var scrollArea = list.parent();
                    if (list.length && list.is(':visible') && (scrollArea.scrollTop() === 0 && e.originalEvent.deltaY < 0 || scrollArea.scrollTop() === scrollArea.prop('scrollHeight') - scrollArea.prop('offsetHeight') && e.originalEvent.deltaY > 0)) {
                        e.preventDefault();
                    }
                }).on('mouseleave' + NS, function () {
                    that._hovered = false;
                });
                that.wrapper = $();
                if (options.animation === false) {
                    options.animation = {
                        open: { effects: {} },
                        close: {
                            hide: true,
                            effects: {}
                        }
                    };
                }
                extend(options.animation.open, {
                    complete: function () {
                        that.wrapper.css({ overflow: VISIBLE });
                        that._activated = true;
                        that._trigger(ACTIVATE);
                    }
                });
                extend(options.animation.close, {
                    complete: function () {
                        that._animationClose();
                    }
                });
                that._mousedownProxy = function (e) {
                    that._mousedown(e);
                };
                if (support.mobileOS.android) {
                    that._resizeProxy = function (e) {
                        setTimeout(function () {
                            that._resize(e);
                        }, 600);
                    };
                } else {
                    that._resizeProxy = function (e) {
                        that._resize(e);
                    };
                }
                if (options.toggleTarget) {
                    $(options.toggleTarget).on(options.toggleEvent + NS, $.proxy(that.toggle, that));
                }
            },
            events: [
                OPEN,
                ACTIVATE,
                CLOSE,
                DEACTIVATE
            ],
            options: {
                name: 'Popup',
                toggleEvent: 'click',
                origin: BOTTOM + ' ' + LEFT,
                position: TOP + ' ' + LEFT,
                anchor: BODY,
                appendTo: null,
                collision: 'flip fit',
                viewport: window,
                copyAnchorStyles: true,
                autosize: false,
                modal: false,
                adjustSize: {
                    width: 0,
                    height: 0
                },
                animation: {
                    open: {
                        effects: 'slideIn:down',
                        transition: true,
                        duration: 200
                    },
                    close: {
                        duration: 100,
                        hide: true
                    }
                }
            },
            _animationClose: function () {
                var that = this;
                var location = that.wrapper.data(LOCATION);
                that.wrapper.hide();
                if (location) {
                    that.wrapper.css(location);
                }
                if (that.options.anchor != BODY) {
                    that._hideDirClass();
                }
                that._closing = false;
                that._trigger(DEACTIVATE);
            },
            destroy: function () {
                var that = this, options = that.options, element = that.element.off(NS), parent;
                Widget.fn.destroy.call(that);
                if (options.toggleTarget) {
                    $(options.toggleTarget).off(NS);
                }
                if (!options.modal) {
                    DOCUMENT_ELEMENT.unbind(that.downEvent, that._mousedownProxy);
                    that._toggleResize(false);
                }
                kendo.destroy(that.element.children());
                element.removeData();
                if (options.appendTo[0] === document.body) {
                    parent = element.parent('.k-animation-container');
                    if (parent[0]) {
                        parent.remove();
                    } else {
                        element.remove();
                    }
                }
            },
            open: function (x, y) {
                var that = this, fixed = {
                        isFixed: !isNaN(parseInt(y, 10)),
                        x: x,
                        y: y
                    }, element = that.element, options = that.options, animation, wrapper, anchor = $(options.anchor), mobile = element[0] && element.hasClass('km-widget');
                if (!that.visible()) {
                    if (options.copyAnchorStyles) {
                        if (mobile && styles[0] == 'font-size') {
                            styles.shift();
                        }
                        element.css(kendo.getComputedStyles(anchor[0], styles));
                    }
                    if (element.data('animating') || that._trigger(OPEN)) {
                        return;
                    }
                    that._activated = false;
                    if (!options.modal) {
                        DOCUMENT_ELEMENT.unbind(that.downEvent, that._mousedownProxy).bind(that.downEvent, that._mousedownProxy);
                        that._toggleResize(false);
                        that._toggleResize(true);
                    }
                    that.wrapper = wrapper = kendo.wrap(element, options.autosize).css({
                        overflow: HIDDEN,
                        display: 'block',
                        position: ABSOLUTE
                    }).attr('aria-hidden', false);
                    if (support.mobileOS.android) {
                        wrapper.css(TRANSFORM, 'translatez(0)');
                    }
                    wrapper.css(POSITION);
                    if ($(options.appendTo)[0] == document.body) {
                        wrapper.css(TOP, '-10000px');
                    }
                    that.flipped = that._position(fixed);
                    animation = that._openAnimation();
                    if (options.anchor != BODY) {
                        that._showDirClass(animation);
                    }
                    element.data(EFFECTS, animation.effects).kendoStop(true).kendoAnimate(animation).attr('aria-hidden', false);
                }
            },
            _location: function (isFixed) {
                var that = this, element = that.element, options = that.options, wrapper, anchor = $(options.anchor), mobile = element[0] && element.hasClass('km-widget');
                if (options.copyAnchorStyles) {
                    if (mobile && styles[0] == 'font-size') {
                        styles.shift();
                    }
                    element.css(kendo.getComputedStyles(anchor[0], styles));
                }
                that.wrapper = wrapper = kendo.wrap(element, options.autosize).css({
                    overflow: HIDDEN,
                    display: 'block',
                    position: ABSOLUTE
                });
                if (support.mobileOS.android) {
                    wrapper.css(TRANSFORM, 'translatez(0)');
                }
                wrapper.css(POSITION);
                if ($(options.appendTo)[0] == document.body) {
                    wrapper.css(TOP, '-10000px');
                }
                that._position(isFixed || {});
                var offset = wrapper.offset();
                return {
                    width: kendo._outerWidth(wrapper),
                    height: kendo._outerHeight(wrapper),
                    left: offset.left,
                    top: offset.top
                };
            },
            _openAnimation: function () {
                var animation = extend(true, {}, this.options.animation.open);
                animation.effects = kendo.parseEffects(animation.effects, this.flipped);
                return animation;
            },
            _hideDirClass: function () {
                var anchor = $(this.options.anchor);
                var direction = ((anchor.attr('class') || '').match(ACTIVEBORDERREGEXP) || [
                    '',
                    'down'
                ])[1];
                var dirClass = ACTIVEBORDER + '-' + direction;
                anchor.removeClass(dirClass).children(ACTIVECHILDREN).removeClass(ACTIVE).removeClass(dirClass);
                this.element.removeClass(ACTIVEBORDER + '-' + kendo.directions[direction].reverse);
            },
            _showDirClass: function (animation) {
                var direction = animation.effects.slideIn ? animation.effects.slideIn.direction : 'down';
                var dirClass = ACTIVEBORDER + '-' + direction;
                $(this.options.anchor).addClass(dirClass).children(ACTIVECHILDREN).addClass(ACTIVE).addClass(dirClass);
                this.element.addClass(ACTIVEBORDER + '-' + kendo.directions[direction].reverse);
            },
            position: function () {
                if (this.visible()) {
                    this.flipped = this._position();
                }
            },
            toggle: function () {
                var that = this;
                that[that.visible() ? CLOSE : OPEN]();
            },
            visible: function () {
                return this.element.is(':' + VISIBLE);
            },
            close: function (skipEffects) {
                var that = this, options = that.options, wrap, animation, openEffects, closeEffects;
                if (that.visible()) {
                    wrap = that.wrapper[0] ? that.wrapper : kendo.wrap(that.element).hide();
                    that._toggleResize(false);
                    if (that._closing || that._trigger(CLOSE)) {
                        that._toggleResize(true);
                        return;
                    }
                    that.element.find('.k-popup').each(function () {
                        var that = $(this), popup = that.data('kendoPopup');
                        if (popup) {
                            popup.close(skipEffects);
                        }
                    });
                    DOCUMENT_ELEMENT.unbind(that.downEvent, that._mousedownProxy);
                    if (skipEffects) {
                        animation = {
                            hide: true,
                            effects: {}
                        };
                    } else {
                        animation = extend(true, {}, options.animation.close);
                        openEffects = that.element.data(EFFECTS);
                        closeEffects = animation.effects;
                        if (!closeEffects && !kendo.size(closeEffects) && openEffects && kendo.size(openEffects)) {
                            animation.effects = openEffects;
                            animation.reverse = true;
                        }
                        that._closing = true;
                    }
                    that.element.kendoStop(true).attr('aria-hidden', true);
                    wrap.css({ overflow: HIDDEN }).attr('aria-hidden', true);
                    that.element.kendoAnimate(animation);
                    if (skipEffects) {
                        that._animationClose();
                    }
                }
            },
            _trigger: function (ev) {
                return this.trigger(ev, { type: ev });
            },
            _resize: function (e) {
                var that = this;
                if (support.resize.indexOf(e.type) !== -1) {
                    clearTimeout(that._resizeTimeout);
                    that._resizeTimeout = setTimeout(function () {
                        that._position();
                        that._resizeTimeout = null;
                    }, 50);
                } else {
                    if (!that._hovered || that._activated && that.element.hasClass('k-list-container')) {
                        that.close();
                    }
                }
            },
            _toggleResize: function (toggle) {
                var method = toggle ? 'on' : 'off';
                var eventNames = support.resize;
                if (!(support.mobileOS.ios || support.mobileOS.android)) {
                    eventNames += ' ' + SCROLL;
                }
                this._scrollableParents()[method](SCROLL, this._resizeProxy);
                WINDOW[method](eventNames, this._resizeProxy);
            },
            _mousedown: function (e) {
                var that = this, container = that.element[0], options = that.options, anchor = $(options.anchor)[0], toggleTarget = options.toggleTarget, target = kendo.eventTarget(e), popup = $(target).closest('.k-popup'), mobile = popup.parent().parent('.km-shim').length;
                popup = popup[0];
                if (!mobile && popup && popup !== that.element[0]) {
                    return;
                }
                if ($(e.target).closest('a').data('rel') === 'popover') {
                    return;
                }
                if (!contains(container, target) && !contains(anchor, target) && !(toggleTarget && contains($(toggleTarget)[0], target))) {
                    that.close();
                }
            },
            _fit: function (position, size, viewPortSize) {
                var output = 0;
                if (position + size > viewPortSize) {
                    output = viewPortSize - (position + size);
                }
                if (position < 0) {
                    output = -position;
                }
                return output;
            },
            _flip: function (offset, size, anchorSize, viewPortSize, origin, position, boxSize) {
                var output = 0;
                boxSize = boxSize || size;
                if (position !== origin && position !== CENTER && origin !== CENTER) {
                    if (offset + boxSize > viewPortSize) {
                        output += -(anchorSize + size);
                    }
                    if (offset + output < 0) {
                        output += anchorSize + size;
                    }
                }
                return output;
            },
            _scrollableParents: function () {
                return $(this.options.anchor).parentsUntil('body').filter(function (index, element) {
                    return kendo.isScrollable(element);
                });
            },
            _position: function (fixed) {
                var that = this, element = that.element, wrapper = that.wrapper, options = that.options, viewport = $(options.viewport), zoomLevel = support.zoomLevel(), isWindow = !!(viewport[0] == window && window.innerWidth && zoomLevel <= 1.02), anchor = $(options.anchor), origins = options.origin.toLowerCase().split(' '), positions = options.position.toLowerCase().split(' '), collisions = that.collisions, siblingContainer, parents, parentZIndex, zIndex = 10002, idx = 0, docEl = document.documentElement, length, viewportOffset, viewportWidth, viewportHeight;
                if (options.viewport === window) {
                    viewportOffset = {
                        top: window.pageYOffset || document.documentElement.scrollTop || 0,
                        left: window.pageXOffset || document.documentElement.scrollLeft || 0
                    };
                } else {
                    viewportOffset = viewport.offset();
                }
                if (isWindow) {
                    viewportWidth = window.innerWidth;
                    viewportHeight = window.innerHeight;
                } else {
                    viewportWidth = viewport.width();
                    viewportHeight = viewport.height();
                }
                if (isWindow && docEl.scrollHeight - docEl.clientHeight > 0) {
                    var sign = options.isRtl ? -1 : 1;
                    viewportWidth -= sign * kendo.support.scrollbar();
                }
                siblingContainer = anchor.parents().filter(wrapper.siblings());
                if (siblingContainer[0]) {
                    parentZIndex = Math.max(Number(siblingContainer.css('zIndex')), 0);
                    if (parentZIndex) {
                        zIndex = parentZIndex + 10;
                    } else {
                        parents = anchor.parentsUntil(siblingContainer);
                        for (length = parents.length; idx < length; idx++) {
                            parentZIndex = Number($(parents[idx]).css('zIndex'));
                            if (parentZIndex && zIndex < parentZIndex) {
                                zIndex = parentZIndex + 10;
                            }
                        }
                    }
                }
                wrapper.css('zIndex', zIndex);
                if (fixed && fixed.isFixed) {
                    wrapper.css({
                        left: fixed.x,
                        top: fixed.y
                    });
                } else {
                    wrapper.css(that._align(origins, positions));
                }
                var pos = getOffset(wrapper, POSITION, anchor[0] === wrapper.offsetParent()[0]), offset = getOffset(wrapper), anchorParent = anchor.offsetParent().parent('.k-animation-container,.k-popup,.k-group');
                if (anchorParent.length) {
                    pos = getOffset(wrapper, POSITION, true);
                    offset = getOffset(wrapper);
                }
                offset.top -= viewportOffset.top;
                offset.left -= viewportOffset.left;
                if (!that.wrapper.data(LOCATION)) {
                    wrapper.data(LOCATION, extend({}, pos));
                }
                var offsets = extend({}, offset), location = extend({}, pos), adjustSize = options.adjustSize;
                if (collisions[0] === 'fit') {
                    location.top += that._fit(offsets.top, outerHeight(wrapper) + adjustSize.height, viewportHeight / zoomLevel);
                }
                if (collisions[1] === 'fit') {
                    location.left += that._fit(offsets.left, outerWidth(wrapper) + adjustSize.width, viewportWidth / zoomLevel);
                }
                var flipPos = extend({}, location);
                var elementHeight = outerHeight(element);
                var wrapperHeight = outerHeight(wrapper);
                if (!wrapper.height() && elementHeight) {
                    wrapperHeight = wrapperHeight + elementHeight;
                }
                if (collisions[0] === 'flip') {
                    location.top += that._flip(offsets.top, elementHeight, outerHeight(anchor), viewportHeight / zoomLevel, origins[0], positions[0], wrapperHeight);
                }
                if (collisions[1] === 'flip') {
                    location.left += that._flip(offsets.left, outerWidth(element), outerWidth(anchor), viewportWidth / zoomLevel, origins[1], positions[1], outerWidth(wrapper));
                }
                element.css(POSITION, ABSOLUTE);
                wrapper.css(location);
                return location.left != flipPos.left || location.top != flipPos.top;
            },
            _align: function (origin, position) {
                var that = this, element = that.wrapper, anchor = $(that.options.anchor), verticalOrigin = origin[0], horizontalOrigin = origin[1], verticalPosition = position[0], horizontalPosition = position[1], anchorOffset = getOffset(anchor), appendTo = $(that.options.appendTo), appendToOffset, width = outerWidth(element), height = outerHeight(element) || outerHeight(element.children().first()), anchorWidth = outerWidth(anchor), anchorHeight = outerHeight(anchor), top = anchorOffset.top, left = anchorOffset.left, round = Math.round;
                if (appendTo[0] != document.body) {
                    appendToOffset = getOffset(appendTo);
                    top -= appendToOffset.top;
                    left -= appendToOffset.left;
                }
                if (verticalOrigin === BOTTOM) {
                    top += anchorHeight;
                }
                if (verticalOrigin === CENTER) {
                    top += round(anchorHeight / 2);
                }
                if (verticalPosition === BOTTOM) {
                    top -= height;
                }
                if (verticalPosition === CENTER) {
                    top -= round(height / 2);
                }
                if (horizontalOrigin === RIGHT) {
                    left += anchorWidth;
                }
                if (horizontalOrigin === CENTER) {
                    left += round(anchorWidth / 2);
                }
                if (horizontalPosition === RIGHT) {
                    left -= width;
                }
                if (horizontalPosition === CENTER) {
                    left -= round(width / 2);
                }
                return {
                    top: top,
                    left: left
                };
            }
        });
        ui.plugin(Popup);
        var stableSort = kendo.support.stableSort;
        var tabKeyTrapNS = 'kendoTabKeyTrap';
        var focusableNodesSelector = 'a[href], area[href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), button:not([disabled]), iframe, object, embed, [tabindex], *[contenteditable]';
        var TabKeyTrap = Class.extend({
            init: function (element) {
                this.element = $(element);
                this.element.autoApplyNS(tabKeyTrapNS);
            },
            trap: function () {
                this.element.on('keydown', proxy(this._keepInTrap, this));
            },
            removeTrap: function () {
                this.element.kendoDestroy(tabKeyTrapNS);
            },
            destroy: function () {
                this.element.kendoDestroy(tabKeyTrapNS);
                this.element = undefined;
            },
            shouldTrap: function () {
                return true;
            },
            _keepInTrap: function (e) {
                if (e.which !== 9 || !this.shouldTrap() || e.isDefaultPrevented()) {
                    return;
                }
                var elements = this._focusableElements();
                var sortedElements = this._sortFocusableElements(elements);
                var next = this._nextFocusable(e, sortedElements);
                this._focus(next);
                e.preventDefault();
            },
            _focusableElements: function () {
                var elements = this.element.find(focusableNodesSelector).filter(function (i, item) {
                    return item.tabIndex >= 0 && $(item).is(':visible') && !$(item).is('[disabled]');
                });
                if (this.element.is('[tabindex]')) {
                    elements.push(this.element[0]);
                }
                return elements;
            },
            _sortFocusableElements: function (elements) {
                var sortedElements;
                if (stableSort) {
                    sortedElements = elements.sort(function (prev, next) {
                        return prev.tabIndex - next.tabIndex;
                    });
                } else {
                    var attrName = '__k_index';
                    elements.each(function (i, item) {
                        item.setAttribute(attrName, i);
                    });
                    sortedElements = elements.sort(function (prev, next) {
                        return prev.tabIndex === next.tabIndex ? parseInt(prev.getAttribute(attrName), 10) - parseInt(next.getAttribute(attrName), 10) : prev.tabIndex - next.tabIndex;
                    });
                    elements.removeAttr(attrName);
                }
                return sortedElements;
            },
            _nextFocusable: function (e, elements) {
                var count = elements.length;
                var current = elements.index(e.target);
                return elements.get((current + (e.shiftKey ? -1 : 1)) % count);
            },
            _focus: function (element) {
                if (element.nodeName == 'IFRAME') {
                    element.contentWindow.document.body.focus();
                    return;
                }
                element.focus();
                if (element.nodeName == 'INPUT' && element.setSelectionRange && this._haveSelectionRange(element)) {
                    element.setSelectionRange(0, element.value.length);
                }
            },
            _haveSelectionRange: function (element) {
                var elementType = element.type.toLowerCase();
                return elementType === 'text' || elementType === 'search' || elementType === 'url' || elementType === 'tel' || elementType === 'password';
            }
        });
        ui.Popup.TabKeyTrap = TabKeyTrap;
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.touch', [
        'kendo.core',
        'kendo.userevents'
    ], f);
}(function () {
    var __meta__ = {
        id: 'touch',
        name: 'Touch',
        category: 'mobile',
        description: 'The kendo Touch widget provides a cross-platform compatible API for handling user-initiated touch events, multi-touch gestures and event sequences (drag, swipe, etc.). ',
        depends: [
            'core',
            'userevents'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, Widget = kendo.ui.Widget, proxy = $.proxy, abs = Math.abs, MAX_DOUBLE_TAP_DISTANCE = 20;
        var Touch = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                options = that.options;
                element = that.element;
                that.wrapper = element;
                function eventProxy(name) {
                    return function (e) {
                        that._triggerTouch(name, e);
                    };
                }
                function gestureEventProxy(name) {
                    return function (e) {
                        that.trigger(name, {
                            touches: e.touches,
                            distance: e.distance,
                            center: e.center,
                            event: e.event
                        });
                    };
                }
                that.events = new kendo.UserEvents(element, {
                    filter: options.filter,
                    surface: options.surface,
                    minHold: options.minHold,
                    multiTouch: options.multiTouch,
                    allowSelection: true,
                    fastTap: options.fastTap,
                    press: eventProxy('touchstart'),
                    hold: eventProxy('hold'),
                    tap: proxy(that, '_tap'),
                    gesturestart: gestureEventProxy('gesturestart'),
                    gesturechange: gestureEventProxy('gesturechange'),
                    gestureend: gestureEventProxy('gestureend')
                });
                if (options.enableSwipe) {
                    that.events.bind('start', proxy(that, '_swipestart'));
                    that.events.bind('move', proxy(that, '_swipemove'));
                } else {
                    that.events.bind('start', proxy(that, '_dragstart'));
                    that.events.bind('move', eventProxy('drag'));
                    that.events.bind('end', eventProxy('dragend'));
                }
                kendo.notify(that);
            },
            events: [
                'touchstart',
                'dragstart',
                'drag',
                'dragend',
                'tap',
                'doubletap',
                'hold',
                'swipe',
                'gesturestart',
                'gesturechange',
                'gestureend'
            ],
            options: {
                name: 'Touch',
                surface: null,
                global: false,
                fastTap: false,
                filter: null,
                multiTouch: false,
                enableSwipe: false,
                minXDelta: 30,
                maxYDelta: 20,
                maxDuration: 1000,
                minHold: 800,
                doubleTapTimeout: 800
            },
            cancel: function () {
                this.events.cancel();
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.events.destroy();
            },
            _triggerTouch: function (type, e) {
                if (this.trigger(type, {
                        touch: e.touch,
                        event: e.event
                    })) {
                    e.preventDefault();
                }
            },
            _tap: function (e) {
                var that = this, lastTap = that.lastTap, touch = e.touch;
                if (lastTap && touch.endTime - lastTap.endTime < that.options.doubleTapTimeout && kendo.touchDelta(touch, lastTap).distance < MAX_DOUBLE_TAP_DISTANCE) {
                    that._triggerTouch('doubletap', e);
                    that.lastTap = null;
                } else {
                    that._triggerTouch('tap', e);
                    that.lastTap = touch;
                }
            },
            _dragstart: function (e) {
                this._triggerTouch('dragstart', e);
            },
            _swipestart: function (e) {
                if (abs(e.x.velocity) * 2 >= abs(e.y.velocity)) {
                    e.sender.capture();
                }
            },
            _swipemove: function (e) {
                var that = this, options = that.options, touch = e.touch, duration = e.event.timeStamp - touch.startTime, direction = touch.x.initialDelta > 0 ? 'right' : 'left';
                if (abs(touch.x.initialDelta) >= options.minXDelta && abs(touch.y.initialDelta) < options.maxYDelta && duration < options.maxDuration) {
                    that.trigger('swipe', {
                        direction: direction,
                        touch: e.touch
                    });
                    touch.cancel();
                }
            }
        });
        kendo.ui.plugin(Touch);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.scroller', [
        'kendo.fx',
        'kendo.draganddrop'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.scroller',
        name: 'Scroller',
        category: 'mobile',
        description: 'The Kendo Mobile Scroller widget enables touch friendly kinetic scrolling for the contents of a given DOM element.',
        depends: [
            'fx',
            'draganddrop'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, mobile = kendo.mobile, fx = kendo.effects, ui = mobile.ui, proxy = $.proxy, extend = $.extend, Widget = ui.Widget, Class = kendo.Class, Movable = kendo.ui.Movable, Pane = kendo.ui.Pane, PaneDimensions = kendo.ui.PaneDimensions, Transition = fx.Transition, Animation = fx.Animation, abs = Math.abs, SNAPBACK_DURATION = 500, SCROLLBAR_OPACITY = 0.7, FRICTION = 0.96, VELOCITY_MULTIPLIER = 10, MAX_VELOCITY = 55, OUT_OF_BOUNDS_FRICTION = 0.5, ANIMATED_SCROLLER_PRECISION = 5, RELEASECLASS = 'km-scroller-release', REFRESHCLASS = 'km-scroller-refresh', PULL = 'pull', CHANGE = 'change', RESIZE = 'resize', SCROLL = 'scroll', MOUSE_WHEEL_ID = 2;
        var ZoomSnapBack = Animation.extend({
            init: function (options) {
                var that = this;
                Animation.fn.init.call(that);
                extend(that, options);
                that.userEvents.bind('gestureend', proxy(that.start, that));
                that.tapCapture.bind('press', proxy(that.cancel, that));
            },
            enabled: function () {
                return this.movable.scale < this.dimensions.minScale;
            },
            done: function () {
                return this.dimensions.minScale - this.movable.scale < 0.01;
            },
            tick: function () {
                var movable = this.movable;
                movable.scaleWith(1.1);
                this.dimensions.rescale(movable.scale);
            },
            onEnd: function () {
                var movable = this.movable;
                movable.scaleTo(this.dimensions.minScale);
                this.dimensions.rescale(movable.scale);
            }
        });
        var DragInertia = Animation.extend({
            init: function (options) {
                var that = this;
                Animation.fn.init.call(that);
                extend(that, options, {
                    transition: new Transition({
                        axis: options.axis,
                        movable: options.movable,
                        onEnd: function () {
                            that._end();
                        }
                    })
                });
                that.tapCapture.bind('press', function () {
                    that.cancel();
                });
                that.userEvents.bind('end', proxy(that.start, that));
                that.userEvents.bind('gestureend', proxy(that.start, that));
                that.userEvents.bind('tap', proxy(that.onEnd, that));
            },
            onCancel: function () {
                this.transition.cancel();
            },
            freeze: function (location) {
                var that = this;
                that.cancel();
                that._moveTo(location);
            },
            onEnd: function () {
                var that = this;
                if (that.paneAxis.outOfBounds()) {
                    that._snapBack();
                } else {
                    that._end();
                }
            },
            done: function () {
                return abs(this.velocity) < 1;
            },
            start: function (e) {
                var that = this, velocity;
                if (!that.dimension.enabled) {
                    return;
                }
                if (that.paneAxis.outOfBounds()) {
                    that._snapBack();
                } else {
                    velocity = e.touch.id === MOUSE_WHEEL_ID ? 0 : e.touch[that.axis].velocity;
                    that.velocity = Math.max(Math.min(velocity * that.velocityMultiplier, MAX_VELOCITY), -MAX_VELOCITY);
                    that.tapCapture.captureNext();
                    Animation.fn.start.call(that);
                }
            },
            tick: function () {
                var that = this, dimension = that.dimension, friction = that.paneAxis.outOfBounds() ? OUT_OF_BOUNDS_FRICTION : that.friction, delta = that.velocity *= friction, location = that.movable[that.axis] + delta;
                if (!that.elastic && dimension.outOfBounds(location)) {
                    location = Math.max(Math.min(location, dimension.max), dimension.min);
                    that.velocity = 0;
                }
                that.movable.moveAxis(that.axis, location);
            },
            _end: function () {
                this.tapCapture.cancelCapture();
                this.end();
            },
            _snapBack: function () {
                var that = this, dimension = that.dimension, snapBack = that.movable[that.axis] > dimension.max ? dimension.max : dimension.min;
                that._moveTo(snapBack);
            },
            _moveTo: function (location) {
                this.transition.moveTo({
                    location: location,
                    duration: SNAPBACK_DURATION,
                    ease: Transition.easeOutExpo
                });
            }
        });
        var AnimatedScroller = Animation.extend({
            init: function (options) {
                var that = this;
                kendo.effects.Animation.fn.init.call(this);
                extend(that, options, {
                    origin: {},
                    destination: {},
                    offset: {}
                });
            },
            tick: function () {
                this._updateCoordinates();
                this.moveTo(this.origin);
            },
            done: function () {
                return abs(this.offset.y) < ANIMATED_SCROLLER_PRECISION && abs(this.offset.x) < ANIMATED_SCROLLER_PRECISION;
            },
            onEnd: function () {
                this.moveTo(this.destination);
                if (this.callback) {
                    this.callback.call();
                }
            },
            setCoordinates: function (from, to) {
                this.offset = {};
                this.origin = from;
                this.destination = to;
            },
            setCallback: function (callback) {
                if (callback && kendo.isFunction(callback)) {
                    this.callback = callback;
                } else {
                    callback = undefined;
                }
            },
            _updateCoordinates: function () {
                this.offset = {
                    x: (this.destination.x - this.origin.x) / 4,
                    y: (this.destination.y - this.origin.y) / 4
                };
                this.origin = {
                    y: this.origin.y + this.offset.y,
                    x: this.origin.x + this.offset.x
                };
            }
        });
        var ScrollBar = Class.extend({
            init: function (options) {
                var that = this, horizontal = options.axis === 'x', element = $('<div class="km-touch-scrollbar km-' + (horizontal ? 'horizontal' : 'vertical') + '-scrollbar" />');
                extend(that, options, {
                    element: element,
                    elementSize: 0,
                    movable: new Movable(element),
                    scrollMovable: options.movable,
                    alwaysVisible: options.alwaysVisible,
                    size: horizontal ? 'width' : 'height'
                });
                that.scrollMovable.bind(CHANGE, proxy(that.refresh, that));
                that.container.append(element);
                if (options.alwaysVisible) {
                    that.show();
                }
            },
            refresh: function () {
                var that = this, axis = that.axis, dimension = that.dimension, paneSize = dimension.size, scrollMovable = that.scrollMovable, sizeRatio = paneSize / dimension.total, position = Math.round(-scrollMovable[axis] * sizeRatio), size = Math.round(paneSize * sizeRatio);
                if (sizeRatio >= 1) {
                    this.element.css('display', 'none');
                } else {
                    this.element.css('display', '');
                }
                if (position + size > paneSize) {
                    size = paneSize - position;
                } else if (position < 0) {
                    size += position;
                    position = 0;
                }
                if (that.elementSize != size) {
                    that.element.css(that.size, size + 'px');
                    that.elementSize = size;
                }
                that.movable.moveAxis(axis, position);
            },
            show: function () {
                this.element.css({
                    opacity: SCROLLBAR_OPACITY,
                    visibility: 'visible'
                });
            },
            hide: function () {
                if (!this.alwaysVisible) {
                    this.element.css({ opacity: 0 });
                }
            }
        });
        var Scroller = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                element = that.element;
                that._native = that.options.useNative && kendo.support.hasNativeScrolling;
                if (that._native) {
                    element.addClass('km-native-scroller').prepend('<div class="km-scroll-header"/>');
                    extend(that, {
                        scrollElement: element,
                        fixedContainer: element.children().first()
                    });
                    return;
                }
                element.css('overflow', 'hidden').addClass('km-scroll-wrapper').wrapInner('<div class="km-scroll-container"/>').prepend('<div class="km-scroll-header"/>');
                var inner = element.children().eq(1), tapCapture = new kendo.TapCapture(element), movable = new Movable(inner), dimensions = new PaneDimensions({
                        element: inner,
                        container: element,
                        forcedEnabled: that.options.zoom
                    }), avoidScrolling = this.options.avoidScrolling, userEvents = new kendo.UserEvents(element, {
                        touchAction: 'pan-y',
                        fastTap: true,
                        allowSelection: true,
                        preventDragEvent: true,
                        captureUpIfMoved: true,
                        multiTouch: that.options.zoom,
                        start: function (e) {
                            dimensions.refresh();
                            var velocityX = abs(e.x.velocity), velocityY = abs(e.y.velocity), horizontalSwipe = velocityX * 2 >= velocityY, originatedFromFixedContainer = $.contains(that.fixedContainer[0], e.event.target), verticalSwipe = velocityY * 2 >= velocityX;
                            if (!originatedFromFixedContainer && !avoidScrolling(e) && that.enabled && (dimensions.x.enabled && horizontalSwipe || dimensions.y.enabled && verticalSwipe)) {
                                userEvents.capture();
                            } else {
                                userEvents.cancel();
                            }
                        }
                    }), pane = new Pane({
                        movable: movable,
                        dimensions: dimensions,
                        userEvents: userEvents,
                        elastic: that.options.elastic
                    }), zoomSnapBack = new ZoomSnapBack({
                        movable: movable,
                        dimensions: dimensions,
                        userEvents: userEvents,
                        tapCapture: tapCapture
                    }), animatedScroller = new AnimatedScroller({
                        moveTo: function (coordinates) {
                            that.scrollTo(coordinates.x, coordinates.y);
                        }
                    });
                movable.bind(CHANGE, function () {
                    that.scrollTop = -movable.y;
                    that.scrollLeft = -movable.x;
                    that.trigger(SCROLL, {
                        scrollTop: that.scrollTop,
                        scrollLeft: that.scrollLeft
                    });
                });
                if (that.options.mousewheelScrolling) {
                    element.on('DOMMouseScroll mousewheel', proxy(this, '_wheelScroll'));
                }
                extend(that, {
                    movable: movable,
                    dimensions: dimensions,
                    zoomSnapBack: zoomSnapBack,
                    animatedScroller: animatedScroller,
                    userEvents: userEvents,
                    pane: pane,
                    tapCapture: tapCapture,
                    pulled: false,
                    enabled: true,
                    scrollElement: inner,
                    scrollTop: 0,
                    scrollLeft: 0,
                    fixedContainer: element.children().first()
                });
                that._initAxis('x');
                that._initAxis('y');
                that._wheelEnd = function () {
                    that._wheel = false;
                    that.userEvents.end(0, that._wheelY);
                };
                dimensions.refresh();
                if (that.options.pullToRefresh) {
                    that._initPullToRefresh();
                }
            },
            _wheelScroll: function (e) {
                if (!this._wheel) {
                    this._wheel = true;
                    this._wheelY = 0;
                    this.userEvents.press(0, this._wheelY);
                }
                clearTimeout(this._wheelTimeout);
                this._wheelTimeout = setTimeout(this._wheelEnd, 50);
                var delta = kendo.wheelDeltaY(e);
                if (delta) {
                    this._wheelY += delta;
                    this.userEvents.move(0, this._wheelY);
                }
                e.preventDefault();
            },
            makeVirtual: function () {
                this.dimensions.y.makeVirtual();
            },
            virtualSize: function (min, max) {
                this.dimensions.y.virtualSize(min, max);
            },
            height: function () {
                return this.dimensions.y.size;
            },
            scrollHeight: function () {
                return this.scrollElement[0].scrollHeight;
            },
            scrollWidth: function () {
                return this.scrollElement[0].scrollWidth;
            },
            options: {
                name: 'Scroller',
                zoom: false,
                pullOffset: 140,
                visibleScrollHints: false,
                elastic: true,
                useNative: false,
                mousewheelScrolling: true,
                avoidScrolling: function () {
                    return false;
                },
                pullToRefresh: false,
                messages: {
                    pullTemplate: 'Pull to refresh',
                    releaseTemplate: 'Release to refresh',
                    refreshTemplate: 'Refreshing'
                }
            },
            events: [
                PULL,
                SCROLL,
                RESIZE
            ],
            _resize: function () {
                if (!this._native) {
                    this.contentResized();
                }
            },
            setOptions: function (options) {
                var that = this;
                Widget.fn.setOptions.call(that, options);
                if (options.pullToRefresh) {
                    that._initPullToRefresh();
                }
            },
            reset: function () {
                if (this._native) {
                    this.scrollElement.scrollTop(0);
                } else {
                    this.movable.moveTo({
                        x: 0,
                        y: 0
                    });
                    this._scale(1);
                }
            },
            contentResized: function () {
                this.dimensions.refresh();
                if (this.pane.x.outOfBounds()) {
                    this.movable.moveAxis('x', this.dimensions.x.min);
                }
                if (this.pane.y.outOfBounds()) {
                    this.movable.moveAxis('y', this.dimensions.y.min);
                }
            },
            zoomOut: function () {
                var dimensions = this.dimensions;
                dimensions.refresh();
                this._scale(dimensions.fitScale);
                this.movable.moveTo(dimensions.centerCoordinates());
            },
            enable: function () {
                this.enabled = true;
            },
            disable: function () {
                this.enabled = false;
            },
            scrollTo: function (x, y) {
                if (this._native) {
                    this.scrollElement.scrollLeft(abs(x));
                    this.scrollElement.scrollTop(abs(y));
                } else {
                    this.dimensions.refresh();
                    this.movable.moveTo({
                        x: x,
                        y: y
                    });
                }
            },
            animatedScrollTo: function (x, y, callback) {
                var from, to;
                if (this._native) {
                    this.scrollTo(x, y);
                } else {
                    from = {
                        x: this.movable.x,
                        y: this.movable.y
                    };
                    to = {
                        x: x,
                        y: y
                    };
                    this.animatedScroller.setCoordinates(from, to);
                    this.animatedScroller.setCallback(callback);
                    this.animatedScroller.start();
                }
            },
            pullHandled: function () {
                var that = this;
                that.refreshHint.removeClass(REFRESHCLASS);
                that.hintContainer.html(that.pullTemplate({}));
                that.yinertia.onEnd();
                that.xinertia.onEnd();
                that.userEvents.cancel();
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                if (this.userEvents) {
                    this.userEvents.destroy();
                }
            },
            _scale: function (scale) {
                this.dimensions.rescale(scale);
                this.movable.scaleTo(scale);
            },
            _initPullToRefresh: function () {
                var that = this;
                that.dimensions.y.forceEnabled();
                that.pullTemplate = kendo.template(that.options.messages.pullTemplate);
                that.releaseTemplate = kendo.template(that.options.messages.releaseTemplate);
                that.refreshTemplate = kendo.template(that.options.messages.refreshTemplate);
                that.scrollElement.prepend('<span class="km-scroller-pull"><span class="km-icon"></span><span class="km-loading-left"></span><span class="km-loading-right"></span><span class="km-template">' + that.pullTemplate({}) + '</span></span>');
                that.refreshHint = that.scrollElement.children().first();
                that.hintContainer = that.refreshHint.children('.km-template');
                that.pane.y.bind('change', proxy(that._paneChange, that));
                that.userEvents.bind('end', proxy(that._dragEnd, that));
            },
            _dragEnd: function () {
                var that = this;
                if (!that.pulled) {
                    return;
                }
                that.pulled = false;
                that.refreshHint.removeClass(RELEASECLASS).addClass(REFRESHCLASS);
                that.hintContainer.html(that.refreshTemplate({}));
                that.yinertia.freeze(that.options.pullOffset / 2);
                that.trigger('pull');
            },
            _paneChange: function () {
                var that = this;
                if (that.movable.y / OUT_OF_BOUNDS_FRICTION > that.options.pullOffset) {
                    if (!that.pulled) {
                        that.pulled = true;
                        that.refreshHint.removeClass(REFRESHCLASS).addClass(RELEASECLASS);
                        that.hintContainer.html(that.releaseTemplate({}));
                    }
                } else if (that.pulled) {
                    that.pulled = false;
                    that.refreshHint.removeClass(RELEASECLASS);
                    that.hintContainer.html(that.pullTemplate({}));
                }
            },
            _initAxis: function (axis) {
                var that = this, movable = that.movable, dimension = that.dimensions[axis], tapCapture = that.tapCapture, paneAxis = that.pane[axis], scrollBar = new ScrollBar({
                        axis: axis,
                        movable: movable,
                        dimension: dimension,
                        container: that.element,
                        alwaysVisible: that.options.visibleScrollHints
                    });
                dimension.bind(CHANGE, function () {
                    scrollBar.refresh();
                });
                paneAxis.bind(CHANGE, function () {
                    scrollBar.show();
                });
                that[axis + 'inertia'] = new DragInertia({
                    axis: axis,
                    paneAxis: paneAxis,
                    movable: movable,
                    tapCapture: tapCapture,
                    userEvents: that.userEvents,
                    dimension: dimension,
                    elastic: that.options.elastic,
                    friction: that.options.friction || FRICTION,
                    velocityMultiplier: that.options.velocityMultiplier || VELOCITY_MULTIPLIER,
                    end: function () {
                        scrollBar.hide();
                        that.trigger('scrollEnd', {
                            axis: axis,
                            scrollTop: that.scrollTop,
                            scrollLeft: that.scrollLeft
                        });
                    }
                });
            }
        });
        ui.plugin(Scroller);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.view', [
        'kendo.core',
        'kendo.fx',
        'kendo.mobile.scroller',
        'kendo.view'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.view',
        name: 'View',
        category: 'mobile',
        description: 'Mobile View',
        depends: [
            'core',
            'fx',
            'mobile.scroller',
            'view'
        ],
        hidden: true
    };
    (function ($, undefined) {
        var kendo = window.kendo, mobile = kendo.mobile, ui = mobile.ui, attr = kendo.attr, Widget = ui.Widget, ViewClone = kendo.ViewClone, INIT = 'init', UI_OVERLAY = '<div style="height: 100%; width: 100%; position: absolute; top: 0; left: 0; z-index: 20000; display: none" />', BEFORE_SHOW = 'beforeShow', SHOW = 'show', AFTER_SHOW = 'afterShow', BEFORE_HIDE = 'beforeHide', TRANSITION_END = 'transitionEnd', TRANSITION_START = 'transitionStart', HIDE = 'hide', DESTROY = 'destroy', attrValue = kendo.attrValue, roleSelector = kendo.roleSelector, directiveSelector = kendo.directiveSelector, compileMobileDirective = kendo.compileMobileDirective;
        function initPopOvers(element) {
            var popovers = element.find(roleSelector('popover')), idx, length, roles = ui.roles;
            for (idx = 0, length = popovers.length; idx < length; idx++) {
                kendo.initWidget(popovers[idx], {}, roles);
            }
        }
        function preventScrollIfNotInput(e) {
            if (!kendo.triggeredByInput(e)) {
                e.preventDefault();
            }
        }
        var View = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                this.params = {};
                $.extend(this, options);
                this.transition = this.transition || this.defaultTransition;
                this._id();
                if (!this.options.$angular) {
                    this._layout();
                    this._overlay();
                    this._scroller();
                    this._model();
                } else {
                    this._overlay();
                }
            },
            events: [
                INIT,
                BEFORE_SHOW,
                SHOW,
                AFTER_SHOW,
                BEFORE_HIDE,
                HIDE,
                DESTROY,
                TRANSITION_START,
                TRANSITION_END
            ],
            options: {
                name: 'View',
                title: '',
                layout: null,
                getLayout: $.noop,
                reload: false,
                transition: '',
                defaultTransition: '',
                useNativeScrolling: false,
                stretch: false,
                zoom: false,
                model: null,
                modelScope: window,
                scroller: {},
                initWidgets: true
            },
            enable: function (enable) {
                if (typeof enable == 'undefined') {
                    enable = true;
                }
                if (enable) {
                    this.overlay.hide();
                } else {
                    this.overlay.show();
                }
            },
            destroy: function () {
                if (this.layout) {
                    this.layout.detach(this);
                }
                this.trigger(DESTROY);
                Widget.fn.destroy.call(this);
                if (this.scroller) {
                    this.scroller.destroy();
                }
                if (this.options.$angular) {
                    this.element.scope().$destroy();
                }
                kendo.destroy(this.element);
            },
            purge: function () {
                this.destroy();
                this.element.remove();
            },
            triggerBeforeShow: function () {
                if (this.trigger(BEFORE_SHOW, { view: this })) {
                    return false;
                }
                return true;
            },
            triggerBeforeHide: function () {
                if (this.trigger(BEFORE_HIDE, { view: this })) {
                    return false;
                }
                return true;
            },
            showStart: function () {
                var element = this.element;
                element.css('display', '');
                if (!this.inited) {
                    this.inited = true;
                    this.trigger(INIT, { view: this });
                } else {
                    this._invokeNgController();
                }
                if (this.layout) {
                    this.layout.attach(this);
                }
                this._padIfNativeScrolling();
                this.trigger(SHOW, { view: this });
                kendo.resize(element);
            },
            showEnd: function () {
                this.trigger(AFTER_SHOW, { view: this });
                this._padIfNativeScrolling();
            },
            hideEnd: function () {
                var that = this;
                that.element.hide();
                that.trigger(HIDE, { view: that });
                if (that.layout) {
                    that.layout.trigger(HIDE, {
                        view: that,
                        layout: that.layout
                    });
                }
            },
            beforeTransition: function (type) {
                this.trigger(TRANSITION_START, { type: type });
            },
            afterTransition: function (type) {
                this.trigger(TRANSITION_END, { type: type });
            },
            _padIfNativeScrolling: function () {
                if (mobile.appLevelNativeScrolling()) {
                    var isAndroid = kendo.support.mobileOS && kendo.support.mobileOS.android, skin = mobile.application.skin() || '', isAndroidForced = mobile.application.os.android || skin.indexOf('android') > -1, hasPlatformIndependentSkin = skin === 'flat' || skin.indexOf('material') > -1, topContainer = (isAndroid || isAndroidForced) && !hasPlatformIndependentSkin ? 'footer' : 'header', bottomContainer = (isAndroid || isAndroidForced) && !hasPlatformIndependentSkin ? 'header' : 'footer';
                    this.content.css({
                        paddingTop: this[topContainer].height(),
                        paddingBottom: this[bottomContainer].height()
                    });
                }
            },
            contentElement: function () {
                var that = this;
                return that.options.stretch ? that.content : that.scrollerContent;
            },
            clone: function () {
                return new ViewClone(this);
            },
            _scroller: function () {
                var that = this;
                if (mobile.appLevelNativeScrolling()) {
                    return;
                }
                if (that.options.stretch) {
                    that.content.addClass('km-stretched-view');
                } else {
                    that.content.kendoMobileScroller($.extend(that.options.scroller, {
                        zoom: that.options.zoom,
                        useNative: that.options.useNativeScrolling
                    }));
                    that.scroller = that.content.data('kendoMobileScroller');
                    that.scrollerContent = that.scroller.scrollElement;
                }
                if (kendo.support.kineticScrollNeeded) {
                    $(that.element).on('touchmove', '.km-header', preventScrollIfNotInput);
                    if (!that.options.useNativeScrolling && !that.options.stretch) {
                        $(that.element).on('touchmove', '.km-content', preventScrollIfNotInput);
                    }
                }
            },
            _model: function () {
                var that = this, element = that.element, model = that.options.model;
                if (typeof model === 'string') {
                    model = kendo.getter(model)(that.options.modelScope);
                }
                that.model = model;
                initPopOvers(element);
                that.element.css('display', '');
                if (that.options.initWidgets) {
                    if (model) {
                        kendo.bind(element, model, ui, kendo.ui, kendo.dataviz.ui);
                    } else {
                        mobile.init(element.children());
                    }
                }
                that.element.css('display', 'none');
            },
            _id: function () {
                var element = this.element, idAttrValue = element.attr('id') || '';
                this.id = attrValue(element, 'url') || '#' + idAttrValue;
                if (this.id == '#') {
                    this.id = kendo.guid();
                    element.attr('id', this.id);
                }
            },
            _layout: function () {
                var contentSelector = roleSelector('content'), element = this.element;
                element.addClass('km-view');
                this.header = element.children(roleSelector('header')).addClass('km-header');
                this.footer = element.children(roleSelector('footer')).addClass('km-footer');
                if (!element.children(contentSelector)[0]) {
                    element.wrapInner('<div ' + attr('role') + '="content"></div>');
                }
                this.content = element.children(roleSelector('content')).addClass('km-content');
                this.element.prepend(this.header).append(this.footer);
                this.layout = this.options.getLayout(this.layout);
                if (this.layout) {
                    this.layout.setup(this);
                }
            },
            _overlay: function () {
                this.overlay = $(UI_OVERLAY).appendTo(this.element);
            },
            _invokeNgController: function () {
                var controller, scope;
                if (this.options.$angular) {
                    controller = this.element.controller();
                    scope = this.options.$angular[0];
                    if (controller) {
                        var callback = $.proxy(this, '_callController', controller, scope);
                        if (/^\$(digest|apply)$/.test(scope.$$phase)) {
                            callback();
                        } else {
                            scope.$apply(callback);
                        }
                    }
                }
            },
            _callController: function (controller, scope) {
                this.element.injector().invoke(controller.constructor, controller, { $scope: scope });
            }
        });
        function initWidgets(collection) {
            collection.each(function () {
                kendo.initWidget($(this), {}, ui.roles);
            });
        }
        var Layout = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                element = this.element;
                this.header = element.children(this._locate('header')).addClass('km-header');
                this.footer = element.children(this._locate('footer')).addClass('km-footer');
                this.elements = this.header.add(this.footer);
                initPopOvers(element);
                if (!this.options.$angular) {
                    kendo.mobile.init(this.element.children());
                }
                this.element.detach();
                this.trigger(INIT, { layout: this });
            },
            _locate: function (selectors) {
                return this.options.$angular ? directiveSelector(selectors) : roleSelector(selectors);
            },
            options: {
                name: 'Layout',
                id: null,
                platform: null
            },
            events: [
                INIT,
                SHOW,
                HIDE
            ],
            setup: function (view) {
                if (!view.header[0]) {
                    view.header = this.header;
                }
                if (!view.footer[0]) {
                    view.footer = this.footer;
                }
            },
            detach: function (view) {
                var that = this;
                if (view.header === that.header && that.header[0]) {
                    view.element.prepend(that.header.detach()[0].cloneNode(true));
                }
                if (view.footer === that.footer && that.footer.length) {
                    view.element.append(that.footer.detach()[0].cloneNode(true));
                }
            },
            attach: function (view) {
                var that = this, previousView = that.currentView;
                if (previousView) {
                    that.detach(previousView);
                }
                if (view.header === that.header) {
                    that.header.detach();
                    view.element.children(roleSelector('header')).remove();
                    view.element.prepend(that.header);
                }
                if (view.footer === that.footer) {
                    that.footer.detach();
                    view.element.children(roleSelector('footer')).remove();
                    view.element.append(that.footer);
                }
                that.trigger(SHOW, {
                    layout: that,
                    view: view
                });
                that.currentView = view;
            }
        });
        var Observable = kendo.Observable, bodyRegExp = /<body[^>]*>(([\u000a\u000d\u2028\u2029]|.)*)<\/body>/i, LOAD_START = 'loadStart', LOAD_COMPLETE = 'loadComplete', SHOW_START = 'showStart', SAME_VIEW_REQUESTED = 'sameViewRequested', VIEW_SHOW = 'viewShow', VIEW_TYPE_DETERMINED = 'viewTypeDetermined', AFTER = 'after';
        var ViewEngine = Observable.extend({
            init: function (options) {
                var that = this, views, errorMessage, container, collection;
                Observable.fn.init.call(that);
                $.extend(that, options);
                that.sandbox = $('<div />');
                container = that.container;
                views = that._hideViews(container);
                that.rootView = views.first();
                if (!that.rootView[0] && options.rootNeeded) {
                    if (container[0] == kendo.mobile.application.element[0]) {
                        errorMessage = 'Your kendo mobile application element does not contain any direct child elements with data-role="view" attribute set. Make sure that you instantiate the mobile application using the correct container.';
                    } else {
                        errorMessage = 'Your pane element does not contain any direct child elements with data-role="view" attribute set.';
                    }
                    throw new Error(errorMessage);
                }
                that.layouts = {};
                that.viewContainer = new kendo.ViewContainer(that.container);
                that.viewContainer.bind('accepted', function (e) {
                    e.view.params = that.params;
                });
                that.viewContainer.bind('complete', function (e) {
                    that.trigger(VIEW_SHOW, { view: e.view });
                });
                that.viewContainer.bind(AFTER, function () {
                    that.trigger(AFTER);
                });
                this.getLayoutProxy = $.proxy(this, '_getLayout');
                that._setupLayouts(container);
                collection = container.children(that._locate('modalview drawer'));
                if (that.$angular) {
                    that.$angular[0].viewOptions = {
                        defaultTransition: that.transition,
                        loader: that.loader,
                        container: that.container,
                        getLayout: that.getLayoutProxy
                    };
                    collection.each(function (idx, element) {
                        compileMobileDirective($(element), options.$angular[0]);
                    });
                } else {
                    initWidgets(collection);
                }
                this.bind(this.events, options);
            },
            events: [
                SHOW_START,
                AFTER,
                VIEW_SHOW,
                LOAD_START,
                LOAD_COMPLETE,
                SAME_VIEW_REQUESTED,
                VIEW_TYPE_DETERMINED
            ],
            destroy: function () {
                kendo.destroy(this.container);
                for (var id in this.layouts) {
                    this.layouts[id].destroy();
                }
            },
            view: function () {
                return this.viewContainer.view;
            },
            showView: function (url, transition, params) {
                url = url.replace(new RegExp('^' + this.remoteViewURLPrefix), '');
                if (url === '' && this.remoteViewURLPrefix) {
                    url = '/';
                }
                if (url.replace(/^#/, '') === this.url) {
                    this.trigger(SAME_VIEW_REQUESTED);
                    return false;
                }
                this.trigger(SHOW_START);
                var that = this, showClosure = function (view) {
                        return that.viewContainer.show(view, transition, url);
                    }, element = that._findViewElement(url), view = kendo.widgetInstance(element);
                that.url = url.replace(/^#/, '');
                that.params = params;
                if (view && view.reload) {
                    view.purge();
                    element = [];
                }
                this.trigger(VIEW_TYPE_DETERMINED, {
                    remote: element.length === 0,
                    url: url
                });
                if (element[0]) {
                    if (!view) {
                        view = that._createView(element);
                    }
                    return showClosure(view);
                } else {
                    if (this.serverNavigation) {
                        location.href = url;
                    } else {
                        that._loadView(url, showClosure);
                    }
                    return true;
                }
            },
            append: function (html, url) {
                var sandbox = this.sandbox, urlPath = (url || '').split('?')[0], container = this.container, views, modalViews, view;
                if (bodyRegExp.test(html)) {
                    html = RegExp.$1;
                }
                sandbox[0].innerHTML = html;
                container.append(sandbox.children('script, style'));
                views = this._hideViews(sandbox);
                view = views.first();
                if (!view.length) {
                    views = view = sandbox.wrapInner('<div data-role=view />').children();
                }
                if (urlPath) {
                    view.hide().attr(attr('url'), urlPath);
                }
                this._setupLayouts(sandbox);
                modalViews = sandbox.children(this._locate('modalview drawer'));
                container.append(sandbox.children(this._locate('layout modalview drawer')).add(views));
                initWidgets(modalViews);
                return this._createView(view);
            },
            _locate: function (selectors) {
                return this.$angular ? directiveSelector(selectors) : roleSelector(selectors);
            },
            _findViewElement: function (url) {
                var element, urlPath = url.split('?')[0];
                if (!urlPath) {
                    return this.rootView;
                }
                element = this.container.children('[' + attr('url') + '=\'' + urlPath + '\']');
                if (!element[0] && urlPath.indexOf('/') === -1) {
                    element = this.container.children(urlPath.charAt(0) === '#' ? urlPath : '#' + urlPath);
                }
                return element;
            },
            _createView: function (element) {
                if (this.$angular) {
                    return compileMobileDirective(element, this.$angular[0]);
                } else {
                    return kendo.initWidget(element, {
                        defaultTransition: this.transition,
                        loader: this.loader,
                        container: this.container,
                        getLayout: this.getLayoutProxy,
                        modelScope: this.modelScope,
                        reload: attrValue(element, 'reload')
                    }, ui.roles);
                }
            },
            _getLayout: function (name) {
                if (name === '') {
                    return null;
                }
                return name ? this.layouts[name] : this.layouts[this.layout];
            },
            _loadView: function (url, callback) {
                if (this._xhr) {
                    this._xhr.abort();
                }
                this.trigger(LOAD_START);
                this._xhr = $.get(kendo.absoluteURL(url, this.remoteViewURLPrefix), 'html').always($.proxy(this, '_xhrComplete', callback, url));
            },
            _xhrComplete: function (callback, url, response) {
                var success = true;
                if (typeof response === 'object') {
                    if (response.status === 0) {
                        if (response.responseText && response.responseText.length > 0) {
                            success = true;
                            response = response.responseText;
                        } else {
                            return;
                        }
                    }
                }
                this.trigger(LOAD_COMPLETE);
                if (success) {
                    callback(this.append(response, url));
                }
            },
            _hideViews: function (container) {
                return container.children(this._locate('view splitview')).hide();
            },
            _setupLayouts: function (element) {
                var that = this, layout;
                element.children(that._locate('layout')).each(function () {
                    if (that.$angular) {
                        layout = compileMobileDirective($(this), that.$angular[0]);
                    } else {
                        layout = kendo.initWidget($(this), {}, ui.roles);
                    }
                    var platform = layout.options.platform;
                    if (!platform || platform === mobile.application.os.name) {
                        that.layouts[layout.options.id] = layout;
                    } else {
                        layout.destroy();
                    }
                });
            }
        });
        kendo.mobile.ViewEngine = ViewEngine;
        ui.plugin(View);
        ui.plugin(Layout);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.loader', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'mobile.loader',
        name: 'Loader',
        category: 'mobile',
        description: 'Mobile Loader',
        depends: ['core'],
        hidden: true
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.mobile.ui, Widget = ui.Widget, CAPTURE_EVENTS = $.map(kendo.eventMap, function (value) {
                return value;
            }).join(' ').split(' ');
        var Loader = Widget.extend({
            init: function (container, options) {
                var that = this, element = $('<div class="km-loader"><span class="km-loading km-spin"></span><span class="km-loading-left"></span><span class="km-loading-right"></span></div>');
                Widget.fn.init.call(that, element, options);
                that.container = container;
                that.captureEvents = false;
                that._attachCapture();
                element.append(that.options.loading).hide().appendTo(container);
            },
            options: {
                name: 'Loader',
                loading: '<h1>Loading...</h1>',
                timeout: 100
            },
            show: function () {
                var that = this;
                clearTimeout(that._loading);
                if (that.options.loading === false) {
                    return;
                }
                that.captureEvents = true;
                that._loading = setTimeout(function () {
                    that.element.show();
                }, that.options.timeout);
            },
            hide: function () {
                this.captureEvents = false;
                clearTimeout(this._loading);
                this.element.hide();
            },
            changeMessage: function (message) {
                this.options.loading = message;
                this.element.find('>h1').html(message);
            },
            transition: function () {
                this.captureEvents = true;
                this.container.css('pointer-events', 'none');
            },
            transitionDone: function () {
                this.captureEvents = false;
                this.container.css('pointer-events', '');
            },
            _attachCapture: function () {
                var that = this;
                that.captureEvents = false;
                function capture(e) {
                    if (that.captureEvents) {
                        e.preventDefault();
                    }
                }
                for (var i = 0; i < CAPTURE_EVENTS.length; i++) {
                    that.container[0].addEventListener(CAPTURE_EVENTS[i], capture, true);
                }
            }
        });
        ui.plugin(Loader);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.pane', [
        'kendo.mobile.view',
        'kendo.mobile.loader'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.pane',
        name: 'Pane',
        category: 'mobile',
        description: 'Mobile Pane',
        depends: [
            'mobile.view',
            'mobile.loader'
        ],
        hidden: true
    };
    (function ($, undefined) {
        var kendo = window.kendo, mobile = kendo.mobile, roleSelector = kendo.roleSelector, ui = mobile.ui, Widget = ui.Widget, ViewEngine = mobile.ViewEngine, View = ui.View, Loader = mobile.ui.Loader, EXTERNAL = 'external', HREF = 'href', DUMMY_HREF = '#!', NAVIGATE = 'navigate', VIEW_SHOW = 'viewShow', SAME_VIEW_REQUESTED = 'sameViewRequested', OS = kendo.support.mobileOS, SKIP_TRANSITION_ON_BACK_BUTTON = OS.ios && !OS.appMode && OS.flatVersion >= 700, WIDGET_RELS = /popover|actionsheet|modalview|drawer/, BACK = '#:back', attrValue = kendo.attrValue;
        var Pane = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                options = that.options;
                element = that.element;
                element.addClass('km-pane');
                if (that.options.collapsible) {
                    element.addClass('km-collapsible-pane');
                }
                this.history = [];
                this.historyCallback = function (url, params, backButtonPressed) {
                    var transition = that.transition;
                    that.transition = null;
                    if (SKIP_TRANSITION_ON_BACK_BUTTON && backButtonPressed) {
                        transition = 'none';
                    }
                    return that.viewEngine.showView(url, transition, params);
                };
                this._historyNavigate = function (url) {
                    if (url === BACK) {
                        if (that.history.length === 1) {
                            return;
                        }
                        that.history.pop();
                        url = that.history[that.history.length - 1];
                    } else {
                        that.history.push(url);
                    }
                    that.historyCallback(url, kendo.parseQueryStringParams(url));
                };
                this._historyReplace = function (url) {
                    var params = kendo.parseQueryStringParams(url);
                    that.history[that.history.length - 1] = url;
                    that.historyCallback(url, params);
                };
                that.loader = new Loader(element, { loading: that.options.loading });
                that.viewEngine = new ViewEngine({
                    container: element,
                    transition: options.transition,
                    modelScope: options.modelScope,
                    rootNeeded: !options.initial,
                    serverNavigation: options.serverNavigation,
                    remoteViewURLPrefix: options.root || '',
                    layout: options.layout,
                    $angular: options.$angular,
                    loader: that.loader,
                    showStart: function () {
                        that.loader.transition();
                        that.closeActiveDialogs();
                    },
                    after: function () {
                        that.loader.transitionDone();
                    },
                    viewShow: function (e) {
                        that.trigger(VIEW_SHOW, e);
                    },
                    loadStart: function () {
                        that.loader.show();
                    },
                    loadComplete: function () {
                        that.loader.hide();
                    },
                    sameViewRequested: function () {
                        that.trigger(SAME_VIEW_REQUESTED);
                    },
                    viewTypeDetermined: function (e) {
                        if (!e.remote || !that.options.serverNavigation) {
                            that.trigger(NAVIGATE, { url: e.url });
                        }
                    }
                });
                this._setPortraitWidth();
                kendo.onResize(function () {
                    that._setPortraitWidth();
                });
                that._setupAppLinks();
            },
            closeActiveDialogs: function () {
                var dialogs = this.element.find(roleSelector('actionsheet popover modalview')).filter(':visible');
                dialogs.each(function () {
                    kendo.widgetInstance($(this), ui).close();
                });
            },
            navigateToInitial: function () {
                var initial = this.options.initial;
                if (initial) {
                    this.navigate(initial);
                }
                return initial;
            },
            options: {
                name: 'Pane',
                portraitWidth: '',
                transition: '',
                layout: '',
                collapsible: false,
                initial: null,
                modelScope: window,
                loading: '<h1>Loading...</h1>'
            },
            events: [
                NAVIGATE,
                VIEW_SHOW,
                SAME_VIEW_REQUESTED
            ],
            append: function (html) {
                return this.viewEngine.append(html);
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.viewEngine.destroy();
                this.userEvents.destroy();
            },
            navigate: function (url, transition) {
                if (url instanceof View) {
                    url = url.id;
                }
                this.transition = transition;
                this._historyNavigate(url);
            },
            replace: function (url, transition) {
                if (url instanceof View) {
                    url = url.id;
                }
                this.transition = transition;
                this._historyReplace(url);
            },
            bindToRouter: function (router) {
                var that = this, history = this.history, viewEngine = this.viewEngine;
                router.bind('init', function (e) {
                    var url = e.url, attrUrl = router.pushState ? url : '/';
                    viewEngine.rootView.attr(kendo.attr('url'), attrUrl);
                    var length = history.length;
                    if (url === '/' && length) {
                        router.navigate(history[length - 1], true);
                        e.preventDefault();
                    }
                });
                router.bind('routeMissing', function (e) {
                    if (!that.historyCallback(e.url, e.params, e.backButtonPressed)) {
                        e.preventDefault();
                    }
                });
                router.bind('same', function () {
                    that.trigger(SAME_VIEW_REQUESTED);
                });
                that._historyNavigate = function (url) {
                    router.navigate(url);
                };
                that._historyReplace = function (url) {
                    router.replace(url);
                };
            },
            hideLoading: function () {
                this.loader.hide();
            },
            showLoading: function () {
                this.loader.show();
            },
            changeLoadingMessage: function (message) {
                this.loader.changeMessage(message);
            },
            view: function () {
                return this.viewEngine.view();
            },
            _setPortraitWidth: function () {
                var width, portraitWidth = this.options.portraitWidth;
                if (portraitWidth) {
                    width = kendo.mobile.application.element.is('.km-vertical') ? portraitWidth : 'auto';
                    this.element.css('width', width);
                }
            },
            _setupAppLinks: function () {
                var that = this, linkRoles = 'tab', pressedButtonSelector = '[data-' + kendo.ns + 'navigate-on-press]', buttonSelectors = $.map([
                        'button',
                        'backbutton',
                        'detailbutton',
                        'listview-link'
                    ], function (role) {
                        return roleSelector(role) + ':not(' + pressedButtonSelector + ')';
                    }).join(',');
                this.element.handler(this).on('down', roleSelector(linkRoles) + ',' + pressedButtonSelector, '_mouseup').on('click', roleSelector(linkRoles) + ',' + buttonSelectors + ',' + pressedButtonSelector, '_appLinkClick');
                this.userEvents = new kendo.UserEvents(this.element, {
                    fastTap: true,
                    filter: buttonSelectors,
                    tap: function (e) {
                        e.event.currentTarget = e.touch.currentTarget;
                        that._mouseup(e.event);
                    }
                });
                this.element.css('-ms-touch-action', '');
            },
            _appLinkClick: function (e) {
                var href = $(e.currentTarget).attr('href');
                var remote = href && href[0] !== '#' && this.options.serverNavigation;
                if (!remote && attrValue($(e.currentTarget), 'rel') != EXTERNAL) {
                    e.preventDefault();
                }
            },
            _mouseup: function (e) {
                if (e.which > 1 || e.isDefaultPrevented()) {
                    return;
                }
                var pane = this, link = $(e.currentTarget), transition = attrValue(link, 'transition'), rel = attrValue(link, 'rel') || '', target = attrValue(link, 'target'), href = link.attr(HREF), delayedTouchEnd = SKIP_TRANSITION_ON_BACK_BUTTON && link[0].offsetHeight === 0, remote = href && href[0] !== '#' && this.options.serverNavigation;
                if (delayedTouchEnd || remote || rel === EXTERNAL || typeof href === 'undefined' || href === DUMMY_HREF) {
                    return;
                }
                link.attr(HREF, DUMMY_HREF);
                setTimeout(function () {
                    link.attr(HREF, href);
                });
                if (rel.match(WIDGET_RELS)) {
                    kendo.widgetInstance($(href), ui).openFor(link);
                    if (rel === 'actionsheet' || rel === 'drawer') {
                        e.stopPropagation();
                    }
                } else {
                    if (target === '_top') {
                        pane = mobile.application.pane;
                    } else if (target) {
                        pane = $('#' + target).data('kendoMobilePane');
                    }
                    pane.navigate(href, transition);
                }
                e.preventDefault();
            }
        });
        Pane.wrap = function (element) {
            if (!element.is(roleSelector('view'))) {
                element = element.wrap('<div data-' + kendo.ns + 'role="view" data-stretch="true"></div>').parent();
            }
            var paneContainer = element.wrap('<div class="km-pane-wrapper"><div></div></div>').parent(), pane = new Pane(paneContainer);
            pane.navigate('');
            return pane;
        };
        ui.plugin(Pane);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.popover', [
        'kendo.popup',
        'kendo.mobile.pane'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.popover',
        name: 'PopOver',
        category: 'mobile',
        description: 'The mobile PopOver widget represents a transient view which is displayed when the user taps on a navigational widget or area on the screen. ',
        depends: [
            'popup',
            'mobile.pane'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, mobile = kendo.mobile, ui = mobile.ui, HIDE = 'hide', OPEN = 'open', CLOSE = 'close', WRAPPER = '<div class="km-popup-wrapper" />', ARROW = '<div class="km-popup-arrow" />', OVERLAY = '<div class="km-popup-overlay" />', DIRECTION_CLASSES = 'km-up km-down km-left km-right', Widget = ui.Widget, DIRECTIONS = {
                'down': {
                    origin: 'bottom center',
                    position: 'top center'
                },
                'up': {
                    origin: 'top center',
                    position: 'bottom center'
                },
                'left': {
                    origin: 'center left',
                    position: 'center right',
                    collision: 'fit flip'
                },
                'right': {
                    origin: 'center right',
                    position: 'center left',
                    collision: 'fit flip'
                }
            }, ANIMATION = {
                animation: {
                    open: {
                        effects: 'fade:in',
                        duration: 0
                    },
                    close: {
                        effects: 'fade:out',
                        duration: 400
                    }
                }
            }, DIMENSIONS = {
                'horizontal': {
                    offset: 'top',
                    size: 'height'
                },
                'vertical': {
                    offset: 'left',
                    size: 'width'
                }
            }, REVERSE = {
                'up': 'down',
                'down': 'up',
                'left': 'right',
                'right': 'left'
            };
        var Popup = Widget.extend({
            init: function (element, options) {
                var that = this, containerPopup = element.closest('.km-modalview-wrapper'), viewport = element.closest('.km-root').children('.km-pane').first(), container = containerPopup[0] ? containerPopup : viewport, popupOptions, axis;
                if (options.viewport) {
                    viewport = options.viewport;
                } else if (!viewport[0]) {
                    viewport = window;
                }
                if (options.container) {
                    container = options.container;
                } else if (!container[0]) {
                    container = document.body;
                }
                popupOptions = {
                    viewport: viewport,
                    copyAnchorStyles: false,
                    autosize: true,
                    open: function () {
                        that.overlay.show();
                    },
                    activate: $.proxy(that._activate, that),
                    deactivate: function () {
                        that.overlay.hide();
                        if (!that._apiCall) {
                            that.trigger(HIDE);
                        }
                        that._apiCall = false;
                    }
                };
                Widget.fn.init.call(that, element, options);
                element = that.element;
                options = that.options;
                element.wrap(WRAPPER).addClass('km-popup').show();
                axis = that.options.direction.match(/left|right/) ? 'horizontal' : 'vertical';
                that.dimensions = DIMENSIONS[axis];
                that.wrapper = element.parent().css({
                    width: options.width,
                    height: options.height
                }).addClass('km-popup-wrapper km-' + options.direction).hide();
                that.arrow = $(ARROW).prependTo(that.wrapper).hide();
                that.overlay = $(OVERLAY).appendTo(container).hide();
                popupOptions.appendTo = that.overlay;
                if (options.className) {
                    that.overlay.addClass(options.className);
                }
                that.popup = new kendo.ui.Popup(that.wrapper, $.extend(true, popupOptions, ANIMATION, DIRECTIONS[options.direction]));
            },
            options: {
                name: 'Popup',
                width: 240,
                height: '',
                direction: 'down',
                container: null,
                viewport: null
            },
            events: [HIDE],
            show: function (target) {
                this.popup.options.anchor = $(target);
                this.popup.open();
            },
            hide: function () {
                this._apiCall = true;
                this.popup.close();
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.popup.destroy();
                this.overlay.remove();
            },
            target: function () {
                return this.popup.options.anchor;
            },
            _activate: function () {
                var that = this, direction = that.options.direction, dimensions = that.dimensions, offset = dimensions.offset, popup = that.popup, anchor = popup.options.anchor, anchorOffset = $(anchor).offset(), elementOffset = $(popup.element).offset(), cssClass = popup.flipped ? REVERSE[direction] : direction, min = that.arrow[dimensions.size]() * 2, max = that.element[dimensions.size]() - that.arrow[dimensions.size](), size = $(anchor)[dimensions.size](), offsetAmount = anchorOffset[offset] - elementOffset[offset] + size / 2;
                if (offsetAmount < min) {
                    offsetAmount = min;
                }
                if (offsetAmount > max) {
                    offsetAmount = max;
                }
                that.wrapper.removeClass(DIRECTION_CLASSES).addClass('km-' + cssClass);
                that.arrow.css(offset, offsetAmount).show();
            }
        });
        var PopOver = Widget.extend({
            init: function (element, options) {
                var that = this, popupOptions;
                that.initialOpen = false;
                Widget.fn.init.call(that, element, options);
                popupOptions = $.extend({
                    className: 'km-popover-root',
                    hide: function () {
                        that.trigger(CLOSE);
                    }
                }, this.options.popup);
                that.popup = new Popup(that.element, popupOptions);
                that.popup.overlay.on('move', function (e) {
                    if (e.target == that.popup.overlay[0]) {
                        e.preventDefault();
                    }
                });
                that.pane = new ui.Pane(that.element, $.extend(this.options.pane, { $angular: this.options.$angular }));
                kendo.notify(that, ui);
            },
            options: {
                name: 'PopOver',
                popup: {},
                pane: {}
            },
            events: [
                OPEN,
                CLOSE
            ],
            open: function (target) {
                this.popup.show(target);
                if (!this.initialOpen) {
                    if (!this.pane.navigateToInitial()) {
                        this.pane.navigate('');
                    }
                    this.popup.popup._position();
                    this.initialOpen = true;
                } else {
                    this.pane.view()._invokeNgController();
                }
            },
            openFor: function (target) {
                this.open(target);
                this.trigger(OPEN, { target: this.popup.target() });
            },
            close: function () {
                this.popup.hide();
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.pane.destroy();
                this.popup.destroy();
                kendo.destroy(this.element);
            }
        });
        ui.plugin(Popup);
        ui.plugin(PopOver);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.shim', ['kendo.popup'], f);
}(function () {
    var __meta__ = {
        id: 'mobile.shim',
        name: 'Shim',
        category: 'mobile',
        description: 'Mobile Shim',
        depends: ['popup'],
        hidden: true
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.mobile.ui, Popup = kendo.ui.Popup, SHIM = '<div class="km-shim"/>', HIDE = 'hide', Widget = ui.Widget;
        var Shim = Widget.extend({
            init: function (element, options) {
                var that = this, app = kendo.mobile.application, os = kendo.support.mobileOS, osname = app ? app.os.name : os ? os.name : 'ios', ioswp = osname === 'ios' || osname === 'wp' || (app ? app.os.skin : false), bb = osname === 'blackberry', align = options.align || (ioswp ? 'bottom center' : bb ? 'center right' : 'center center'), position = options.position || (ioswp ? 'bottom center' : bb ? 'center right' : 'center center'), effect = options.effect || (ioswp ? 'slideIn:up' : bb ? 'slideIn:left' : 'fade:in'), shim = $(SHIM).handler(that).hide();
                Widget.fn.init.call(that, element, options);
                that.shim = shim;
                element = that.element;
                options = that.options;
                if (options.className) {
                    that.shim.addClass(options.className);
                }
                if (!options.modal) {
                    that.shim.on('down', '_hide');
                }
                (app ? app.element : $(document.body)).append(shim);
                that.popup = new Popup(that.element, {
                    anchor: shim,
                    modal: true,
                    appendTo: shim,
                    origin: align,
                    position: position,
                    animation: {
                        open: {
                            effects: effect,
                            duration: options.duration
                        },
                        close: { duration: options.duration }
                    },
                    close: function (e) {
                        var prevented = false;
                        if (!that._apiCall) {
                            prevented = that.trigger(HIDE);
                        }
                        if (prevented) {
                            e.preventDefault();
                        }
                        that._apiCall = false;
                    },
                    deactivate: function () {
                        shim.hide();
                    },
                    open: function () {
                        shim.show();
                    }
                });
                kendo.notify(that);
            },
            events: [HIDE],
            options: {
                name: 'Shim',
                modal: false,
                align: undefined,
                position: undefined,
                effect: undefined,
                duration: 200
            },
            show: function () {
                this.popup.open();
            },
            hide: function () {
                this._apiCall = true;
                this.popup.close();
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.shim.kendoDestroy();
                this.popup.destroy();
                this.shim.remove();
            },
            _hide: function (e) {
                if (!e || !$.contains(this.shim.children().children('.k-popup')[0], e.target)) {
                    this.popup.close();
                }
            }
        });
        ui.plugin(Shim);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.modalview', [
        'kendo.mobile.shim',
        'kendo.mobile.view'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.modalview',
        name: 'ModalView',
        category: 'mobile',
        description: 'The Kendo ModalView is used to present self-contained functionality in the context of the current task.',
        depends: [
            'mobile.shim',
            'mobile.view'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.mobile.ui, Shim = ui.Shim, Widget = ui.Widget, BEFORE_OPEN = 'beforeOpen', OPEN = 'open', CLOSE = 'close', INIT = 'init', WRAP = '<div class="km-modalview-wrapper" />';
        var ModalView = ui.View.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                that._id();
                that._wrap();
                that._shim();
                if (!this.options.$angular) {
                    that._layout();
                    that._scroller();
                    that._model();
                }
                that.element.css('display', '');
                that.trigger(INIT);
            },
            events: [
                INIT,
                BEFORE_OPEN,
                OPEN,
                CLOSE
            ],
            options: {
                name: 'ModalView',
                modal: true,
                width: null,
                height: null
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.shim.destroy();
            },
            open: function (target) {
                var that = this;
                that.target = $(target);
                that.shim.show();
                that._invokeNgController();
                that.trigger('show', { view: that });
            },
            openFor: function (target) {
                if (!this.trigger(BEFORE_OPEN, { target: target })) {
                    this.open(target);
                    this.trigger(OPEN, { target: target });
                }
            },
            close: function () {
                if (this.element.is(':visible') && !this.trigger(CLOSE)) {
                    this.shim.hide();
                }
            },
            _wrap: function () {
                var that = this, element = that.element, options = that.options, width, height;
                width = element[0].style.width || 'auto';
                height = element[0].style.height || 'auto';
                element.addClass('km-modalview').wrap(WRAP);
                that.wrapper = element.parent().css({
                    width: options.width || width || 300,
                    height: options.height || height || 300
                }).addClass(height == 'auto' ? ' km-auto-height' : '');
                element.css({
                    width: '',
                    height: ''
                });
            },
            _shim: function () {
                var that = this;
                that.shim = new Shim(that.wrapper, {
                    modal: that.options.modal,
                    position: 'center center',
                    align: 'center center',
                    effect: 'fade:in',
                    className: 'km-modalview-root',
                    hide: function (e) {
                        if (that.trigger(CLOSE)) {
                            e.preventDefault();
                        }
                    }
                });
            }
        });
        ui.plugin(ModalView);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.drawer', [
        'kendo.mobile.view',
        'kendo.userevents'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.drawer',
        name: 'Drawer',
        category: 'mobile',
        description: 'The Kendo Mobile Drawer widget provides slide to reveal global application toolbox',
        depends: [
            'mobile.view',
            'userevents'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, mobile = kendo.mobile, os = kendo.support.mobileOS, Transition = kendo.effects.Transition, roleSelector = kendo.roleSelector, AXIS = 'x', ui = mobile.ui, SWIPE_TO_OPEN = !(os.ios && os.majorVersion == 7 && !os.appMode), BEFORE_SHOW = 'beforeShow', INIT = 'init', SHOW = 'show', HIDE = 'hide', AFTER_HIDE = 'afterHide', NULL_VIEW = { enable: $.noop };
        var Drawer = ui.View.extend({
            init: function (element, options) {
                $(element).parent().prepend(element);
                mobile.ui.Widget.fn.init.call(this, element, options);
                if (!this.options.$angular) {
                    this._layout();
                    this._scroller();
                }
                this._model();
                var pane = this.element.closest(roleSelector('pane')).data('kendoMobilePane'), userEvents;
                if (pane) {
                    this.pane = pane;
                    this.pane.bind('viewShow', function (e) {
                        drawer._viewShow(e);
                    });
                    this.pane.bind('sameViewRequested', function () {
                        drawer.hide();
                    });
                    userEvents = this.userEvents = new kendo.UserEvents(pane.element, {
                        fastTap: true,
                        filter: roleSelector('view splitview'),
                        allowSelection: true
                    });
                } else {
                    this.currentView = NULL_VIEW;
                    var container = $(this.options.container);
                    if (!container) {
                        throw new Error('The drawer needs a container configuration option set.');
                    }
                    userEvents = this.userEvents = new kendo.UserEvents(container, {
                        fastTap: true,
                        allowSelection: true
                    });
                    this._attachTransition(container);
                }
                var drawer = this;
                var hide = function (e) {
                    if (drawer.visible) {
                        drawer.hide();
                        e.preventDefault();
                    }
                };
                if (this.options.swipeToOpen && SWIPE_TO_OPEN) {
                    userEvents.bind('press', function () {
                        drawer.transition.cancel();
                    });
                    userEvents.bind('start', function (e) {
                        drawer._start(e);
                    });
                    userEvents.bind('move', function (e) {
                        drawer._update(e);
                    });
                    userEvents.bind('end', function (e) {
                        drawer._end(e);
                    });
                    userEvents.bind('tap', hide);
                } else {
                    userEvents.bind('press', hide);
                }
                this.leftPositioned = this.options.position === 'left';
                this.visible = false;
                this.element.hide().addClass('km-drawer').addClass(this.leftPositioned ? 'km-left-drawer' : 'km-right-drawer');
                this.trigger(INIT);
            },
            options: {
                name: 'Drawer',
                position: 'left',
                views: [],
                swipeToOpenViews: [],
                swipeToOpen: true,
                title: '',
                container: null
            },
            events: [
                BEFORE_SHOW,
                HIDE,
                AFTER_HIDE,
                INIT,
                SHOW
            ],
            show: function () {
                if (this._activate()) {
                    this._show();
                }
            },
            hide: function () {
                if (!this.currentView) {
                    return;
                }
                this.currentView.enable();
                Drawer.current = null;
                this._moveViewTo(0);
                this.trigger(HIDE, { view: this });
            },
            openFor: function () {
                if (this.visible) {
                    this.hide();
                } else {
                    this.show();
                }
            },
            destroy: function () {
                ui.View.fn.destroy.call(this);
                this.userEvents.destroy();
            },
            _activate: function () {
                if (this.visible) {
                    return true;
                }
                var visibleOnCurrentView = this._currentViewIncludedIn(this.options.views);
                if (!visibleOnCurrentView || this.trigger(BEFORE_SHOW, { view: this })) {
                    return false;
                }
                this._setAsCurrent();
                this.element.show();
                this.trigger(SHOW, { view: this });
                this._invokeNgController();
                return true;
            },
            _currentViewIncludedIn: function (views) {
                if (!this.pane || !views.length) {
                    return true;
                }
                var view = this.pane.view();
                return $.inArray(view.id.replace('#', ''), views) > -1 || $.inArray(view.element.attr('id'), views) > -1;
            },
            _show: function () {
                this.currentView.enable(false);
                this.visible = true;
                var offset = this.element.width();
                if (!this.leftPositioned) {
                    offset = -offset;
                }
                this._moveViewTo(offset);
            },
            _setAsCurrent: function () {
                if (Drawer.last !== this) {
                    if (Drawer.last) {
                        Drawer.last.element.hide();
                    }
                    this.element.show();
                }
                Drawer.last = this;
                Drawer.current = this;
            },
            _moveViewTo: function (offset) {
                this.userEvents.cancel();
                this.transition.moveTo({
                    location: offset,
                    duration: 400,
                    ease: Transition.easeOutExpo
                });
            },
            _viewShow: function (e) {
                if (this.currentView) {
                    this.currentView.enable();
                }
                if (this.currentView === e.view) {
                    this.hide();
                    return;
                }
                this.currentView = e.view;
                this._attachTransition(e.view.element);
            },
            _attachTransition: function (element) {
                var that = this, movable = this.movable, currentOffset = movable && movable.x;
                if (this.transition) {
                    this.transition.cancel();
                    this.movable.moveAxis('x', 0);
                }
                movable = this.movable = new kendo.ui.Movable(element);
                this.transition = new Transition({
                    axis: AXIS,
                    movable: this.movable,
                    onEnd: function () {
                        if (movable[AXIS] === 0) {
                            element[0].style.cssText = '';
                            that.element.hide();
                            that.trigger(AFTER_HIDE);
                            that.visible = false;
                        }
                    }
                });
                if (currentOffset) {
                    element.addClass('k-fx-hidden');
                    kendo.animationFrame(function () {
                        element.removeClass('k-fx-hidden');
                        that.movable.moveAxis(AXIS, currentOffset);
                        that.hide();
                    });
                }
            },
            _start: function (e) {
                var userEvents = e.sender;
                if (Math.abs(e.x.velocity) < Math.abs(e.y.velocity) || kendo.triggeredByInput(e.event) || !this._currentViewIncludedIn(this.options.swipeToOpenViews)) {
                    userEvents.cancel();
                    return;
                }
                var leftPositioned = this.leftPositioned, visible = this.visible, canMoveLeft = leftPositioned && visible || !leftPositioned && !Drawer.current, canMoveRight = !leftPositioned && visible || leftPositioned && !Drawer.current, leftSwipe = e.x.velocity < 0;
                if (canMoveLeft && leftSwipe || canMoveRight && !leftSwipe) {
                    if (this._activate()) {
                        userEvents.capture();
                        return;
                    }
                }
                userEvents.cancel();
            },
            _update: function (e) {
                var movable = this.movable, newPosition = movable.x + e.x.delta, limitedPosition;
                if (this.leftPositioned) {
                    limitedPosition = Math.min(Math.max(0, newPosition), this.element.width());
                } else {
                    limitedPosition = Math.max(Math.min(0, newPosition), -this.element.width());
                }
                this.movable.moveAxis(AXIS, limitedPosition);
                e.event.preventDefault();
                e.event.stopPropagation();
            },
            _end: function (e) {
                var velocity = e.x.velocity, pastHalf = Math.abs(this.movable.x) > this.element.width() / 2, velocityThreshold = 0.8, shouldShow;
                if (this.leftPositioned) {
                    shouldShow = velocity > -velocityThreshold && (velocity > velocityThreshold || pastHalf);
                } else {
                    shouldShow = velocity < velocityThreshold && (velocity < -velocityThreshold || pastHalf);
                }
                if (shouldShow) {
                    this._show();
                } else {
                    this.hide();
                }
            }
        });
        ui.plugin(Drawer);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.splitview', ['kendo.mobile.pane'], f);
}(function () {
    var __meta__ = {
        id: 'mobile.splitview',
        name: 'SplitView',
        category: 'mobile',
        description: 'The mobile SplitView is a tablet-specific view that consists of two or more mobile Pane widgets.',
        depends: ['mobile.pane']
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.mobile.ui, Widget = ui.Widget, EXPANED_PANE_SHIM = '<div class=\'km-expanded-pane-shim\' />', View = ui.View;
        var SplitView = View.extend({
            init: function (element, options) {
                var that = this, pane, modalViews;
                Widget.fn.init.call(that, element, options);
                element = that.element;
                $.extend(that, options);
                that._id();
                if (!that.options.$angular) {
                    that._layout();
                    that._overlay();
                } else {
                    that._overlay();
                }
                that._style();
                modalViews = element.children(that._locate('modalview'));
                if (!that.options.$angular) {
                    kendo.mobile.init(modalViews);
                } else {
                    modalViews.each(function (idx, element) {
                        kendo.compileMobileDirective($(element), options.$angular[0]);
                    });
                }
                that.panes = [];
                that._paramsHistory = [];
                if (!that.options.$angular) {
                    that.content.children(kendo.roleSelector('pane')).each(function () {
                        pane = kendo.initWidget(this, {}, ui.roles);
                        that.panes.push(pane);
                    });
                } else {
                    that.element.children(kendo.directiveSelector('pane')).each(function () {
                        pane = kendo.compileMobileDirective($(this), options.$angular[0]);
                        that.panes.push(pane);
                    });
                    that.element.children(kendo.directiveSelector('header footer')).each(function () {
                        kendo.compileMobileDirective($(this), options.$angular[0]);
                    });
                }
                that.expandedPaneShim = $(EXPANED_PANE_SHIM).appendTo(that.element);
                that._shimUserEvents = new kendo.UserEvents(that.expandedPaneShim, {
                    fastTap: true,
                    tap: function () {
                        that.collapsePanes();
                    }
                });
            },
            _locate: function (selectors) {
                return this.options.$angular ? kendo.directiveSelector(selectors) : kendo.roleSelector(selectors);
            },
            options: {
                name: 'SplitView',
                style: 'horizontal'
            },
            expandPanes: function () {
                this.element.addClass('km-expanded-splitview');
            },
            collapsePanes: function () {
                this.element.removeClass('km-expanded-splitview');
            },
            _layout: function () {
                var that = this, element = that.element;
                that.transition = kendo.attrValue(element, 'transition');
                kendo.mobile.ui.View.prototype._layout.call(this);
                kendo.mobile.init(this.header.add(this.footer));
                that.element.addClass('km-splitview');
                that.content.addClass('km-split-content');
            },
            _style: function () {
                var style = this.options.style, element = this.element, styles;
                if (style) {
                    styles = style.split(' ');
                    $.each(styles, function () {
                        element.addClass('km-split-' + this);
                    });
                }
            },
            showStart: function () {
                var that = this;
                that.element.css('display', '');
                if (!that.inited) {
                    that.inited = true;
                    $.each(that.panes, function () {
                        if (this.options.initial) {
                            this.navigateToInitial();
                        } else {
                            this.navigate('');
                        }
                    });
                    that.trigger('init', { view: that });
                } else {
                    this._invokeNgController();
                }
                that.trigger('show', { view: that });
            }
        });
        ui.plugin(SplitView);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.application', [
        'kendo.mobile.pane',
        'kendo.router'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.application',
        name: 'Application',
        category: 'mobile',
        description: 'The Mobile application provides a framework to build native looking web applications on mobile devices.',
        depends: [
            'mobile.pane',
            'router'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, mobile = kendo.mobile, support = kendo.support, Widget = mobile.ui.Widget, Pane = mobile.ui.Pane, DEFAULT_OS = 'ios7', OS = support.mobileOS, BERRYPHONEGAP = OS.device == 'blackberry' && OS.flatVersion >= 600 && OS.flatVersion < 1000 && OS.appMode, FONT_SIZE_COEF = 0.93, VERTICAL = 'km-vertical', CHROME = OS.browser === 'chrome', BROKEN_WEBVIEW_RESIZE = OS.ios && OS.flatVersion >= 700 && OS.flatVersion < 800 && (OS.appMode || CHROME), INITIALLY_HORIZONTAL = Math.abs(window.orientation) / 90 == 1, HORIZONTAL = 'km-horizontal', MOBILE_PLATFORMS = {
                ios7: {
                    ios: true,
                    browser: 'default',
                    device: 'iphone',
                    flatVersion: '700',
                    majorVersion: '7',
                    minorVersion: '0.0',
                    name: 'ios',
                    tablet: false
                },
                ios: {
                    ios: true,
                    browser: 'default',
                    device: 'iphone',
                    flatVersion: '612',
                    majorVersion: '6',
                    minorVersion: '1.2',
                    name: 'ios',
                    tablet: false
                },
                android: {
                    android: true,
                    browser: 'default',
                    device: 'android',
                    flatVersion: '442',
                    majorVersion: '4',
                    minorVersion: '4.2',
                    name: 'android',
                    tablet: false
                },
                blackberry: {
                    blackberry: true,
                    browser: 'default',
                    device: 'blackberry',
                    flatVersion: '710',
                    majorVersion: '7',
                    minorVersion: '1.0',
                    name: 'blackberry',
                    tablet: false
                },
                meego: {
                    meego: true,
                    browser: 'default',
                    device: 'meego',
                    flatVersion: '850',
                    majorVersion: '8',
                    minorVersion: '5.0',
                    name: 'meego',
                    tablet: false
                },
                wp: {
                    wp: true,
                    browser: 'default',
                    device: 'wp',
                    flatVersion: '800',
                    majorVersion: '8',
                    minorVersion: '0.0',
                    name: 'wp',
                    tablet: false
                }
            }, viewportTemplate = kendo.template('<meta content="initial-scale=#: data.scale #, maximum-scale=#: data.scale #, user-scalable=no#=data.height#" name="viewport" />', { usedWithBlock: false }), systemMeta = kendo.template('<meta name="apple-mobile-web-app-capable" content="#= data.webAppCapable === false ? \'no\' : \'yes\' #" /> ' + '<meta name="apple-mobile-web-app-status-bar-style" content="#=data.statusBarStyle#" /> ' + '<meta name="msapplication-tap-highlight" content="no" /> ', { usedWithBlock: false }), clipTemplate = kendo.template('<style>.km-view { clip: rect(0 #= data.width #px #= data.height #px 0); }</style>', { usedWithBlock: false }), ENABLE_CLIP = OS.android && OS.browser != 'chrome' || OS.blackberry, iconMeta = kendo.template('<link rel="apple-touch-icon' + (OS.android ? '-precomposed' : '') + '" # if(data.size) { # sizes="#=data.size#" #}# href="#=data.icon#" />', { usedWithBlock: false }), HIDEBAR = (OS.device == 'iphone' || OS.device == 'ipod') && OS.majorVersion < 7, SUPPORT_SWIPE_TO_GO_BACK = (OS.device == 'iphone' || OS.device == 'ipod') && OS.majorVersion >= 7, HISTORY_TRANSITION = SUPPORT_SWIPE_TO_GO_BACK ? 'none' : null, BARCOMPENSATION = OS.browser == 'mobilesafari' ? 60 : 0, STATUS_BAR_HEIGHT = 20, WINDOW = $(window), SCREEN = window.screen, HEAD = $('head'), INIT = 'init', proxy = $.proxy;
        function osCssClass(os, options) {
            var classes = [];
            if (OS) {
                classes.push('km-on-' + OS.name);
            }
            if (os.skin) {
                classes.push('km-' + os.skin);
            } else {
                if (os.name == 'ios' && os.majorVersion > 6) {
                    classes.push('km-ios7');
                } else {
                    classes.push('km-' + os.name);
                }
            }
            if (os.name == 'ios' && os.majorVersion < 7 || os.name != 'ios') {
                classes.push('km-' + os.name + os.majorVersion);
            }
            classes.push('km-' + os.majorVersion);
            classes.push('km-m' + (os.minorVersion ? os.minorVersion[0] : 0));
            if (os.variant && (os.skin && os.skin === os.name || !os.skin || os.setDefaultPlatform === false)) {
                classes.push('km-' + (os.skin ? os.skin : os.name) + '-' + os.variant);
            }
            if (os.cordova) {
                classes.push('km-cordova');
            }
            if (os.appMode) {
                classes.push('km-app');
            } else {
                classes.push('km-web');
            }
            if (options && options.statusBarStyle) {
                classes.push('km-' + options.statusBarStyle + '-status-bar');
            }
            return classes.join(' ');
        }
        function wp8Background(os) {
            return 'km-wp-' + (os.noVariantSet ? parseInt($('<div style=\'background: Background\' />').css('background-color').split(',')[1], 10) === 0 ? 'dark' : 'light' : os.variant + ' km-wp-' + os.variant + '-force');
        }
        function isOrientationHorizontal(element) {
            return OS.wp ? element.css('animation-name') == '-kendo-landscape' : Math.abs(window.orientation) / 90 == 1;
        }
        function getOrientationClass(element) {
            return isOrientationHorizontal(element) ? HORIZONTAL : VERTICAL;
        }
        function setMinimumHeight(pane) {
            pane.parent().addBack().css('min-height', window.innerHeight);
        }
        function applyViewportHeight() {
            $('meta[name=viewport]').remove();
            HEAD.append(viewportTemplate({ height: ', width=device-width' + (isOrientationHorizontal() ? ', height=' + window.innerHeight + 'px' : support.mobileOS.flatVersion >= 600 && support.mobileOS.flatVersion < 700 ? ', height=' + window.innerWidth + 'px' : ', height=device-height') }));
        }
        var Application = Widget.extend({
            init: function (element, options) {
                mobile.application = this;
                $($.proxy(this, 'bootstrap', element, options));
            },
            bootstrap: function (element, options) {
                element = $(element);
                if (!element[0]) {
                    element = $(document.body);
                }
                Widget.fn.init.call(this, element, options);
                this.element.removeAttr('data-' + kendo.ns + 'role');
                this._setupPlatform();
                this._attachMeta();
                this._setupElementClass();
                this._attachHideBarHandlers();
                var paneOptions = $.extend({}, this.options);
                delete paneOptions.name;
                var that = this, startHistory = function () {
                        that.pane = new Pane(that.element, paneOptions);
                        that.pane.navigateToInitial();
                        if (that.options.updateDocumentTitle) {
                            that._setupDocumentTitle();
                        }
                        that._startHistory();
                        that.trigger(INIT);
                    };
                if (this.options.$angular) {
                    setTimeout(startHistory);
                } else {
                    startHistory();
                }
            },
            options: {
                name: 'Application',
                hideAddressBar: true,
                browserHistory: true,
                historyTransition: HISTORY_TRANSITION,
                modelScope: window,
                statusBarStyle: 'black',
                transition: '',
                retina: false,
                platform: null,
                skin: null,
                updateDocumentTitle: true,
                useNativeScrolling: false
            },
            events: [INIT],
            navigate: function (url, transition) {
                this.pane.navigate(url, transition);
            },
            replace: function (url, transition) {
                this.pane.replace(url, transition);
            },
            scroller: function () {
                return this.view().scroller;
            },
            hideLoading: function () {
                if (this.pane) {
                    this.pane.hideLoading();
                } else {
                    throw new Error('The mobile application instance is not fully instantiated. Please consider activating loading in the application init event handler.');
                }
            },
            showLoading: function () {
                if (this.pane) {
                    this.pane.showLoading();
                } else {
                    throw new Error('The mobile application instance is not fully instantiated. Please consider activating loading in the application init event handler.');
                }
            },
            changeLoadingMessage: function (message) {
                if (this.pane) {
                    this.pane.changeLoadingMessage(message);
                } else {
                    throw new Error('The mobile application instance is not fully instantiated. Please consider changing the message in the application init event handler.');
                }
            },
            view: function () {
                return this.pane.view();
            },
            skin: function (skin) {
                var that = this;
                if (!arguments.length) {
                    return that.options.skin;
                }
                that.options.skin = skin || '';
                that.element[0].className = 'km-pane';
                that._setupPlatform();
                that._setupElementClass();
                return that.options.skin;
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.pane.destroy();
                if (this.options.browserHistory) {
                    this.router.destroy();
                }
            },
            _setupPlatform: function () {
                var that = this, platform = that.options.platform, skin = that.options.skin, split = [], os = OS || MOBILE_PLATFORMS[DEFAULT_OS];
                if (platform) {
                    os.setDefaultPlatform = true;
                    if (typeof platform === 'string') {
                        split = platform.split('-');
                        os = $.extend({ variant: split[1] }, os, MOBILE_PLATFORMS[split[0]]);
                    } else {
                        os = platform;
                    }
                }
                if (skin) {
                    split = skin.split('-');
                    if (!OS) {
                        os.setDefaultPlatform = false;
                    }
                    os = $.extend({}, os, {
                        skin: split[0],
                        variant: split[1]
                    });
                }
                if (!os.variant) {
                    os.noVariantSet = true;
                    os.variant = 'dark';
                }
                that.os = os;
                that.osCssClass = osCssClass(that.os, that.options);
                if (os.name == 'wp') {
                    if (!that.refreshBackgroundColorProxy) {
                        that.refreshBackgroundColorProxy = $.proxy(function () {
                            if (that.os.variant && (that.os.skin && that.os.skin === that.os.name) || !that.os.skin) {
                                that.element.removeClass('km-wp-dark km-wp-light km-wp-dark-force km-wp-light-force').addClass(wp8Background(that.os));
                            }
                        }, that);
                    }
                    $(document).off('visibilitychange', that.refreshBackgroundColorProxy);
                    $(document).off('resume', that.refreshBackgroundColorProxy);
                    if (!os.skin) {
                        that.element.parent().css('overflow', 'hidden');
                        $(document).on('visibilitychange', that.refreshBackgroundColorProxy);
                        $(document).on('resume', that.refreshBackgroundColorProxy);
                        that.refreshBackgroundColorProxy();
                    }
                }
            },
            _startHistory: function () {
                if (this.options.browserHistory) {
                    this.router = new kendo.Router({
                        pushState: this.options.pushState,
                        root: this.options.root,
                        hashBang: this.options.hashBang
                    });
                    this.pane.bindToRouter(this.router);
                    this.router.start();
                } else {
                    if (!this.options.initial) {
                        this.pane.navigate('');
                    }
                }
            },
            _resizeToScreenHeight: function () {
                var includeStatusBar = $('meta[name=apple-mobile-web-app-status-bar-style]').attr('content').match(/black-translucent|hidden/), element = this.element, height;
                if (CHROME) {
                    height = window.innerHeight;
                } else {
                    if (isOrientationHorizontal(element)) {
                        if (includeStatusBar) {
                            if (INITIALLY_HORIZONTAL) {
                                height = SCREEN.availWidth + STATUS_BAR_HEIGHT;
                            } else {
                                height = SCREEN.availWidth;
                            }
                        } else {
                            if (INITIALLY_HORIZONTAL) {
                                height = SCREEN.availWidth;
                            } else {
                                height = SCREEN.availWidth - STATUS_BAR_HEIGHT;
                            }
                        }
                    } else {
                        if (includeStatusBar) {
                            if (INITIALLY_HORIZONTAL) {
                                height = SCREEN.availHeight;
                            } else {
                                height = SCREEN.availHeight + STATUS_BAR_HEIGHT;
                            }
                        } else {
                            if (INITIALLY_HORIZONTAL) {
                                height = SCREEN.availHeight - STATUS_BAR_HEIGHT;
                            } else {
                                height = SCREEN.availHeight;
                            }
                        }
                    }
                }
                element.height(height);
            },
            _setupElementClass: function () {
                var that = this, size, element = that.element;
                element.parent().addClass('km-root km-' + (that.os.tablet ? 'tablet' : 'phone'));
                element.addClass(that.osCssClass + ' ' + getOrientationClass(element));
                if (this.options.useNativeScrolling) {
                    element.parent().addClass('km-native-scrolling');
                }
                if (CHROME) {
                    element.addClass('km-ios-chrome');
                }
                if (support.wpDevicePixelRatio) {
                    element.parent().css('font-size', support.wpDevicePixelRatio + 'em');
                }
                if (this.options.retina) {
                    element.parent().addClass('km-retina');
                    element.parent().css('font-size', support.devicePixelRatio * FONT_SIZE_COEF + 'em');
                }
                if (BERRYPHONEGAP) {
                    applyViewportHeight();
                }
                if (that.options.useNativeScrolling) {
                    element.parent().addClass('km-native-scrolling');
                } else if (ENABLE_CLIP) {
                    size = (screen.availWidth > screen.availHeight ? screen.availWidth : screen.availHeight) + 200;
                    $(clipTemplate({
                        width: size,
                        height: size
                    })).appendTo(HEAD);
                }
                if (BROKEN_WEBVIEW_RESIZE) {
                    that._resizeToScreenHeight();
                }
                kendo.onResize(function () {
                    element.removeClass('km-horizontal km-vertical').addClass(getOrientationClass(element));
                    if (that.options.useNativeScrolling) {
                        setMinimumHeight(element);
                    }
                    if (BROKEN_WEBVIEW_RESIZE) {
                        that._resizeToScreenHeight();
                    }
                    if (BERRYPHONEGAP) {
                        applyViewportHeight();
                    }
                    kendo.resize(element);
                });
            },
            _clearExistingMeta: function () {
                HEAD.find('meta').filter('[name|=\'apple-mobile-web-app\'],[name|=\'msapplication-tap\'],[name=\'viewport\']').remove();
            },
            _attachMeta: function () {
                var options = this.options, icon = options.icon, size;
                this._clearExistingMeta();
                if (!BERRYPHONEGAP) {
                    HEAD.prepend(viewportTemplate({
                        height: '',
                        scale: this.options.retina ? 1 / support.devicePixelRatio : '1.0'
                    }));
                }
                HEAD.prepend(systemMeta(options));
                if (icon) {
                    if (typeof icon === 'string') {
                        icon = { '': icon };
                    }
                    for (size in icon) {
                        HEAD.prepend(iconMeta({
                            icon: icon[size],
                            size: size
                        }));
                    }
                }
                if (options.useNativeScrolling) {
                    setMinimumHeight(this.element);
                }
            },
            _attachHideBarHandlers: function () {
                var that = this, hideBar = proxy(that, '_hideBar');
                if (support.mobileOS.appMode || !that.options.hideAddressBar || !HIDEBAR || that.options.useNativeScrolling) {
                    return;
                }
                that._initialHeight = {};
                WINDOW.on('load', hideBar);
                kendo.onResize(function () {
                    setTimeout(window.scrollTo, 0, 0, 1);
                });
            },
            _setupDocumentTitle: function () {
                var that = this, defaultTitle = document.title;
                that.pane.bind('viewShow', function (e) {
                    var title = e.view.title;
                    document.title = title !== undefined ? title : defaultTitle;
                });
            },
            _hideBar: function () {
                var that = this, element = that.element;
                element.height(kendo.support.transforms.css + 'calc(100% + ' + BARCOMPENSATION + 'px)');
                $(window).trigger(kendo.support.resize);
            }
        });
        kendo.mobile.Application = Application;
        kendo.ui.plugin(Application, kendo.mobile, 'Mobile');
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.actionsheet', [
        'kendo.mobile.popover',
        'kendo.mobile.shim'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.actionsheet',
        name: 'ActionSheet',
        category: 'mobile',
        description: 'The mobile ActionSheet widget displays a set of choices related to a task the user initiates.',
        depends: [
            'mobile.popover',
            'mobile.shim'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, support = kendo.support, ui = kendo.mobile.ui, Shim = ui.Shim, Popup = ui.Popup, Widget = ui.Widget, OPEN = 'open', CLOSE = 'close', COMMAND = 'command', BUTTONS = 'li>a', CONTEXT_DATA = 'actionsheetContext', WRAP = '<div class="km-actionsheet-wrapper" />', cancelTemplate = kendo.template('<li class="km-actionsheet-cancel"><a href="\\#">#:cancel#</a></li>');
        var ActionSheet = Widget.extend({
            init: function (element, options) {
                var that = this, ShimClass, tablet, type, os = support.mobileOS;
                Widget.fn.init.call(that, element, options);
                options = that.options;
                type = options.type;
                element = that.element;
                if (type === 'auto') {
                    tablet = os && os.tablet;
                } else {
                    tablet = type === 'tablet';
                }
                ShimClass = tablet ? Popup : Shim;
                if (options.cancelTemplate) {
                    cancelTemplate = kendo.template(options.cancelTemplate);
                }
                element.addClass('km-actionsheet').append(cancelTemplate({ cancel: that.options.cancel })).wrap(WRAP).on('up', BUTTONS, '_click').on('click', BUTTONS, kendo.preventDefault);
                that.view().bind('destroy', function () {
                    that.destroy();
                });
                that.wrapper = element.parent().addClass(type ? ' km-actionsheet-' + type : '');
                that.shim = new ShimClass(that.wrapper, $.extend({
                    modal: os.ios && os.majorVersion < 7,
                    className: 'km-actionsheet-root'
                }, that.options.popup));
                that._closeProxy = $.proxy(that, '_close');
                that._shimHideProxy = $.proxy(that, '_shimHide');
                that.shim.bind('hide', that._shimHideProxy);
                if (tablet) {
                    kendo.onResize(that._closeProxy);
                }
                kendo.notify(that, ui);
            },
            events: [
                OPEN,
                CLOSE,
                COMMAND
            ],
            options: {
                name: 'ActionSheet',
                cancel: 'Cancel',
                type: 'auto',
                popup: { height: 'auto' }
            },
            open: function (target, context) {
                var that = this;
                that.target = $(target);
                that.context = context;
                that.shim.show(target);
            },
            close: function () {
                this.context = this.target = null;
                this.shim.hide();
            },
            openFor: function (target) {
                var that = this, context = target.data(CONTEXT_DATA);
                that.open(target, context);
                that.trigger(OPEN, {
                    target: target,
                    context: context
                });
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                kendo.unbindResize(this._closeProxy);
                this.shim.destroy();
            },
            _click: function (e) {
                if (e.isDefaultPrevented()) {
                    return;
                }
                var currentTarget = $(e.currentTarget);
                var action = currentTarget.data('action');
                if (action) {
                    var actionData = {
                            target: this.target,
                            context: this.context
                        }, $angular = this.options.$angular;
                    if ($angular) {
                        this.element.injector().get('$parse')(action)($angular[0])(actionData);
                    } else {
                        kendo.getter(action)(window)(actionData);
                    }
                }
                this.trigger(COMMAND, {
                    target: this.target,
                    context: this.context,
                    currentTarget: currentTarget
                });
                e.preventDefault();
                this._close();
            },
            _shimHide: function (e) {
                if (!this.trigger(CLOSE)) {
                    this.context = this.target = null;
                } else {
                    e.preventDefault();
                }
            },
            _close: function (e) {
                if (!this.trigger(CLOSE)) {
                    this.close();
                } else {
                    e.preventDefault();
                }
            }
        });
        ui.plugin(ActionSheet);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.button', ['kendo.userevents'], f);
}(function () {
    var __meta__ = {
        id: 'mobile.button',
        name: 'Button',
        category: 'mobile',
        description: 'The Button widget navigates between mobile Application views when pressed.',
        depends: ['userevents']
    };
    (function ($, undefined) {
        var kendo = window.kendo, mobile = kendo.mobile, ui = mobile.ui, Widget = ui.Widget, support = kendo.support, os = support.mobileOS, ANDROID3UP = os.android && os.flatVersion >= 300, CLICK = 'click', DISABLED = 'disabled', DISABLEDSTATE = 'km-state-disabled';
        function highlightButton(widget, event, highlight) {
            $(event.target).closest('.km-button,.km-detail').toggleClass('km-state-active', highlight);
            if (ANDROID3UP && widget.deactivateTimeoutID) {
                clearTimeout(widget.deactivateTimeoutID);
                widget.deactivateTimeoutID = 0;
            }
        }
        function createBadge(value) {
            return $('<span class="km-badge">' + value + '</span>');
        }
        var Button = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                var useTap = that.options.clickOn === 'up';
                that._wrap();
                that._style();
                if (!useTap) {
                    that.element.attr('data-navigate-on-press', true);
                }
                that.options.enable = that.options.enable && !that.element.attr(DISABLED);
                that.enable(that.options.enable);
                that._userEvents = new kendo.UserEvents(that.element, {
                    allowSelection: !useTap,
                    fastTap: true,
                    press: function (e) {
                        that._activate(e);
                    },
                    release: function (e) {
                        highlightButton(that, e, false);
                        if (!useTap) {
                            e.event.stopPropagation();
                        }
                    }
                });
                that._userEvents.bind(useTap ? 'tap' : 'press', function (e) {
                    that._release(e);
                });
                if (ANDROID3UP) {
                    that.element.on('move', function (e) {
                        that._timeoutDeactivate(e);
                    });
                }
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this._userEvents.destroy();
            },
            events: [CLICK],
            options: {
                name: 'Button',
                icon: '',
                style: '',
                badge: '',
                clickOn: 'up',
                enable: true
            },
            badge: function (value) {
                var badge = this.badgeElement = this.badgeElement || createBadge(value).appendTo(this.element);
                if (value || value === 0) {
                    badge.html(value);
                    return this;
                }
                if (value === false) {
                    badge.empty().remove();
                    this.badgeElement = false;
                    return this;
                }
                return badge.html();
            },
            enable: function (enable) {
                var element = this.element;
                if (typeof enable == 'undefined') {
                    enable = true;
                }
                this.options.enable = enable;
                if (enable) {
                    element.removeAttr(DISABLED);
                } else {
                    element.attr(DISABLED, DISABLED);
                }
                element.toggleClass(DISABLEDSTATE, !enable);
            },
            _timeoutDeactivate: function (e) {
                if (!this.deactivateTimeoutID) {
                    this.deactivateTimeoutID = setTimeout(highlightButton, 500, this, e, false);
                }
            },
            _activate: function (e) {
                var activeElement = document.activeElement, nodeName = activeElement ? activeElement.nodeName : '';
                if (this.options.enable) {
                    highlightButton(this, e, true);
                    if (nodeName == 'INPUT' || nodeName == 'TEXTAREA') {
                        activeElement.blur();
                    }
                }
            },
            _release: function (e) {
                var that = this;
                if (e.which > 1) {
                    return;
                }
                if (!that.options.enable) {
                    e.preventDefault();
                    return;
                }
                if (that.trigger(CLICK, {
                        target: $(e.target),
                        button: that.element
                    })) {
                    e.preventDefault();
                }
            },
            _style: function () {
                var style = this.options.style, element = this.element, styles;
                if (style) {
                    styles = style.split(' ');
                    $.each(styles, function () {
                        element.addClass('km-' + this);
                    });
                }
            },
            _wrap: function () {
                var that = this, icon = that.options.icon, badge = that.options.badge, iconSpan = '<span class="km-icon km-' + icon, element = that.element.addClass('km-button'), span = element.children('span:not(.km-icon)').addClass('km-text'), image = element.find('img').addClass('km-image');
                if (!span[0] && element.html()) {
                    span = element.wrapInner('<span class="km-text" />').children('span.km-text');
                }
                if (!image[0] && icon) {
                    if (!span[0]) {
                        iconSpan += ' km-notext';
                    }
                    that.iconElement = element.prepend($(iconSpan + '" />'));
                }
                if (badge || badge === 0) {
                    that.badgeElement = createBadge(badge).appendTo(element);
                }
            }
        });
        var BackButton = Button.extend({
            options: {
                name: 'BackButton',
                style: 'back'
            },
            init: function (element, options) {
                var that = this;
                Button.fn.init.call(that, element, options);
                if (typeof that.element.attr('href') === 'undefined') {
                    that.element.attr('href', '#:back');
                }
            }
        });
        var DetailButton = Button.extend({
            options: {
                name: 'DetailButton',
                style: ''
            },
            init: function (element, options) {
                Button.fn.init.call(this, element, options);
            },
            _style: function () {
                var style = this.options.style + ' detail', element = this.element;
                if (style) {
                    var styles = style.split(' ');
                    $.each(styles, function () {
                        element.addClass('km-' + this);
                    });
                }
            },
            _wrap: function () {
                var that = this, icon = that.options.icon, iconSpan = '<span class="km-icon km-' + icon, element = that.element, span = element.children('span'), image = element.find('img').addClass('km-image');
                if (!image[0] && icon) {
                    if (!span[0]) {
                        iconSpan += ' km-notext';
                    }
                    element.prepend($(iconSpan + '" />'));
                }
            }
        });
        ui.plugin(Button);
        ui.plugin(BackButton);
        ui.plugin(DetailButton);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.buttongroup', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'mobile.buttongroup',
        name: 'ButtonGroup',
        category: 'mobile',
        description: 'The Kendo mobile ButtonGroup widget is a linear set of grouped buttons.',
        depends: ['core']
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.mobile.ui, Widget = ui.Widget, ACTIVE = 'state-active', DISABLE = 'state-disabled', SELECT = 'select', SELECTOR = 'li:not(.km-' + ACTIVE + ')';
        function className(name) {
            return 'k-' + name + ' km-' + name;
        }
        function createBadge(value) {
            return $('<span class="' + className('badge') + '">' + value + '</span>');
        }
        var ButtonGroup = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                that.element.addClass('km-buttongroup k-widget k-button-group').find('li').each(that._button);
                that.element.on(that.options.selectOn, SELECTOR, '_select');
                that._enable = true;
                that.select(that.options.index);
                if (!that.options.enable) {
                    that._enable = false;
                    that.wrapper.addClass(className(DISABLE));
                }
            },
            events: [SELECT],
            options: {
                name: 'ButtonGroup',
                selectOn: 'down',
                index: -1,
                enable: true
            },
            current: function () {
                return this.element.find('.km-' + ACTIVE);
            },
            select: function (li) {
                var that = this, index = -1;
                if (li === undefined || li === -1 || !that._enable || $(li).is('.km-' + DISABLE)) {
                    return;
                }
                that.current().removeClass(className(ACTIVE));
                if (typeof li === 'number') {
                    index = li;
                    li = $(that.element[0].children[li]);
                } else if (li.nodeType) {
                    li = $(li);
                    index = li.index();
                }
                li.addClass(className(ACTIVE));
                that.selectedIndex = index;
            },
            badge: function (item, value) {
                var buttongroup = this.element, badge;
                if (!isNaN(item)) {
                    item = buttongroup.children().get(item);
                }
                item = buttongroup.find(item);
                badge = $(item.children('.km-badge')[0] || createBadge(value).appendTo(item));
                if (value || value === 0) {
                    badge.html(value);
                    return this;
                }
                if (value === false) {
                    badge.empty().remove();
                    return this;
                }
                return badge.html();
            },
            enable: function (enable) {
                if (typeof enable == 'undefined') {
                    enable = true;
                }
                this.wrapper.toggleClass(className(DISABLE), !enable);
                this._enable = this.options.enable = enable;
            },
            _button: function () {
                var button = $(this).addClass(className('button')), icon = kendo.attrValue(button, 'icon'), badge = kendo.attrValue(button, 'badge'), span = button.children('span'), image = button.find('img').addClass(className('image'));
                if (!span[0]) {
                    span = button.wrapInner('<span/>').children('span');
                }
                span.addClass(className('text'));
                if (!image[0] && icon) {
                    button.prepend($('<span class="' + className('icon') + ' ' + className(icon) + '"/>'));
                }
                if (badge || badge === 0) {
                    createBadge(badge).appendTo(button);
                }
            },
            _select: function (e) {
                if (e.which > 1 || e.isDefaultPrevented() || !this._enable) {
                    return;
                }
                this.select(e.currentTarget);
                this.trigger(SELECT, { index: this.selectedIndex });
            }
        });
        ui.plugin(ButtonGroup);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.collapsible', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'mobile.collapsible',
        name: 'Collapsible',
        category: 'mobile',
        description: 'The Kendo mobile Collapsible widget provides ability for creating collapsible blocks of content.',
        depends: [
            'core',
            'userevents'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.mobile.ui, Widget = ui.Widget, COLLAPSIBLE = 'km-collapsible', HEADER = 'km-collapsible-header', CONTENT = 'km-collapsible-content', INSET = 'km-collapsibleinset', HEADER_WRAPPER = '<div data-role=\'collapsible-header\' class=\'' + HEADER + '\'></div>', CONTENT_WRAPPER = '<div data-role=\'collapsible-content\' class=\'' + CONTENT + '\'></div>', COLLAPSED = 'km-collapsed', EXPANDED = 'km-expanded', ANIMATED = 'km-animated', LEFT = 'left', EXAPND = 'expand', COLLAPSE = 'collapse';
        var Collapsible = Widget.extend({
            init: function (element, options) {
                var that = this, container = $(element);
                Widget.fn.init.call(that, container, options);
                container.addClass(COLLAPSIBLE);
                that._buildHeader();
                that.content = container.children().not(that.header).wrapAll(CONTENT_WRAPPER).parent();
                that._userEvents = new kendo.UserEvents(that.header, {
                    fastTap: true,
                    tap: function () {
                        that.toggle();
                    }
                });
                container.addClass(that.options.collapsed ? COLLAPSED : EXPANDED);
                if (that.options.inset) {
                    container.addClass(INSET);
                }
                if (that.options.animation) {
                    that.content.addClass(ANIMATED);
                    that.content.height(0);
                    if (that.options.collapsed) {
                        that.content.hide();
                    }
                } else if (that.options.collapsed) {
                    that.content.hide();
                }
            },
            events: [
                EXAPND,
                COLLAPSE
            ],
            options: {
                name: 'Collapsible',
                collapsed: true,
                collapseIcon: 'arrow-n',
                expandIcon: 'arrow-s',
                iconPosition: LEFT,
                animation: true,
                inset: false
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this._userEvents.destroy();
            },
            expand: function (instant) {
                var icon = this.options.collapseIcon, content = this.content, ios = kendo.support.mobileOS.ios;
                if (!this.trigger(EXAPND)) {
                    if (icon) {
                        this.header.find('.km-icon').removeClass().addClass('km-icon km-' + icon);
                    }
                    this.element.removeClass(COLLAPSED).addClass(EXPANDED);
                    if (this.options.animation && !instant) {
                        content.off('transitionend');
                        content.show();
                        if (ios) {
                            content.removeClass(ANIMATED);
                        }
                        content.height(this._getContentHeight());
                        if (ios) {
                            content.addClass(ANIMATED);
                        }
                        kendo.resize(content);
                    } else {
                        content.show();
                    }
                }
            },
            collapse: function (instant) {
                var icon = this.options.expandIcon, content = this.content;
                if (!this.trigger(COLLAPSE)) {
                    if (icon) {
                        this.header.find('.km-icon').removeClass().addClass('km-icon km-' + icon);
                    }
                    this.element.removeClass(EXPANDED).addClass(COLLAPSED);
                    if (this.options.animation && !instant) {
                        content.one('transitionend', function () {
                            content.hide();
                        });
                        content.height(0);
                    } else {
                        content.hide();
                    }
                }
            },
            toggle: function (instant) {
                if (this.isCollapsed()) {
                    this.expand(instant);
                } else {
                    this.collapse(instant);
                }
            },
            isCollapsed: function () {
                return this.element.hasClass(COLLAPSED);
            },
            resize: function () {
                if (!this.isCollapsed() && this.options.animation) {
                    this.content.height(this._getContentHeight());
                }
            },
            _buildHeader: function () {
                var header = this.element.children(':header').wrapAll(HEADER_WRAPPER), iconSpan = $('<span class="km-icon"/>'), icon = this.options.collapsed ? this.options.expandIcon : this.options.collapseIcon, iconPosition = this.options.iconPosition;
                if (icon) {
                    header.prepend(iconSpan);
                    iconSpan.addClass('km-' + icon);
                }
                this.header = header.parent();
                this.header.addClass('km-icon-' + iconPosition);
            },
            _getContentHeight: function () {
                var style = this.content.attr('style'), height;
                this.content.css({
                    position: 'absolute',
                    visibility: 'hidden',
                    height: 'auto'
                });
                height = this.content.height();
                this.content.attr('style', style ? style : '');
                return height;
            }
        });
        ui.plugin(Collapsible);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.listview', [
        'kendo.data',
        'kendo.userevents',
        'kendo.mobile.button'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.listview',
        name: 'ListView',
        category: 'mobile',
        description: 'The Kendo Mobile ListView widget is used to display flat or grouped list of items.',
        depends: [
            'data',
            'userevents',
            'mobile.button'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, Node = window.Node, mobile = kendo.mobile, ui = mobile.ui, outerHeight = kendo._outerHeight, DataSource = kendo.data.DataSource, Widget = ui.DataBoundWidget, ITEM_SELECTOR = '.km-list > li, > li:not(.km-group-container)', HIGHLIGHT_SELECTOR = '.km-listview-link, .km-listview-label', ICON_SELECTOR = '[' + kendo.attr('icon') + ']', proxy = $.proxy, attrValue = kendo.attrValue, GROUP_CLASS = 'km-group-title', ACTIVE_CLASS = 'km-state-active', GROUP_WRAPPER = '<div class="' + GROUP_CLASS + '"><div class="km-text"></div></div>', GROUP_TEMPLATE = kendo.template('<li><div class="' + GROUP_CLASS + '"><div class="km-text">#= this.headerTemplate(data) #</div></div><ul>#= kendo.render(this.template, data.items)#</ul></li>'), WRAPPER = '<div class="km-listview-wrapper" />', SEARCH_TEMPLATE = kendo.template('<form class="km-filter-form"><div class="km-filter-wrap"><input type="search" placeholder="#=placeholder#"/><a href="\\#" class="km-filter-reset" title="Clear"><span class="km-icon km-clear"></span><span class="km-text">Clear</span></a></div></form>'), NS = '.kendoMobileListView', STYLED = 'styled', DATABOUND = 'dataBound', DATABINDING = 'dataBinding', ITEM_CHANGE = 'itemChange', CLICK = 'click', CHANGE = 'change', PROGRESS = 'progress', FUNCTION = 'function', whitespaceRegExp = /^\s+$/, buttonRegExp = /button/;
        function whitespace() {
            return this.nodeType === Node.TEXT_NODE && this.nodeValue.match(whitespaceRegExp);
        }
        function addIcon(item, icon) {
            if (icon && !item[0].querySelector('.km-icon')) {
                item.prepend('<span class="km-icon km-' + icon + '"/>');
            }
        }
        function enhanceItem(item) {
            addIcon(item, attrValue(item, 'icon'));
            addIcon(item, attrValue(item.children(ICON_SELECTOR), 'icon'));
        }
        function enhanceLinkItem(item) {
            var parent = item.parent(), itemAndDetailButtons = item.add(parent.children(kendo.roleSelector('detailbutton'))), otherNodes = parent.contents().not(itemAndDetailButtons).not(whitespace);
            if (otherNodes.length) {
                return;
            }
            item.addClass('km-listview-link').attr(kendo.attr('role'), 'listview-link');
            addIcon(item, attrValue(parent, 'icon'));
            addIcon(item, attrValue(item, 'icon'));
        }
        function enhanceCheckBoxItem(label) {
            if (!label[0].querySelector('input[type=checkbox],input[type=radio]')) {
                return;
            }
            var item = label.parent();
            if (item.contents().not(label).not(function () {
                    return this.nodeType == 3;
                })[0]) {
                return;
            }
            label.addClass('km-listview-label');
            label.children('[type=checkbox],[type=radio]').addClass('km-widget km-icon km-check');
        }
        function putAt(element, top) {
            $(element).css('transform', 'translate3d(0px, ' + top + 'px, 0px)');
        }
        var HeaderFixer = kendo.Class.extend({
            init: function (listView) {
                var scroller = listView.scroller();
                if (!scroller) {
                    return;
                }
                this.options = listView.options;
                this.element = listView.element;
                this.scroller = listView.scroller();
                this._shouldFixHeaders();
                var headerFixer = this;
                var cacheHeaders = function () {
                    headerFixer._cacheHeaders();
                };
                listView.bind('resize', cacheHeaders);
                listView.bind(STYLED, cacheHeaders);
                listView.bind(DATABOUND, cacheHeaders);
                this._scrollHandler = function (e) {
                    headerFixer._fixHeader(e);
                };
                scroller.bind('scroll', this._scrollHandler);
            },
            destroy: function () {
                var that = this;
                if (that.scroller) {
                    that.scroller.unbind('scroll', that._scrollHandler);
                }
            },
            _fixHeader: function (e) {
                if (!this.fixedHeaders) {
                    return;
                }
                var i = 0, scroller = this.scroller, headers = this.headers, scrollTop = e.scrollTop, headerPair, offset, header;
                do {
                    headerPair = headers[i++];
                    if (!headerPair) {
                        header = $('<div />');
                        break;
                    }
                    offset = headerPair.offset;
                    header = headerPair.header;
                } while (offset + 1 > scrollTop);
                if (this.currentHeader != i) {
                    scroller.fixedContainer.html(header.clone());
                    this.currentHeader = i;
                }
            },
            _shouldFixHeaders: function () {
                this.fixedHeaders = this.options.type === 'group' && this.options.fixedHeaders;
            },
            _cacheHeaders: function () {
                this._shouldFixHeaders();
                if (!this.fixedHeaders) {
                    return;
                }
                var headers = [], offset = this.scroller.scrollTop;
                this.element.find('.' + GROUP_CLASS).each(function (_, header) {
                    header = $(header);
                    headers.unshift({
                        offset: header.position().top + offset,
                        header: header
                    });
                });
                this.headers = headers;
                this._fixHeader({ scrollTop: offset });
            }
        });
        var DEFAULT_PULL_PARAMETERS = function () {
            return { page: 1 };
        };
        var RefreshHandler = kendo.Class.extend({
            init: function (listView) {
                var handler = this, options = listView.options, scroller = listView.scroller(), pullParameters = options.pullParameters || DEFAULT_PULL_PARAMETERS;
                this.listView = listView;
                this.scroller = scroller;
                listView.bind('_dataSource', function (e) {
                    handler.setDataSource(e.dataSource);
                });
                scroller.setOptions({
                    pullToRefresh: true,
                    pull: function () {
                        if (!handler._pulled) {
                            handler._pulled = true;
                            handler.dataSource.read(pullParameters.call(listView, handler._first));
                        }
                    },
                    messages: {
                        pullTemplate: options.messages.pullTemplate,
                        releaseTemplate: options.messages.releaseTemplate,
                        refreshTemplate: options.messages.refreshTemplate
                    }
                });
            },
            setDataSource: function (dataSource) {
                var handler = this;
                this._first = dataSource.view()[0];
                this.dataSource = dataSource;
                dataSource.bind('change', function () {
                    handler._change();
                });
                dataSource.bind('error', function () {
                    handler._change();
                });
            },
            _change: function () {
                var scroller = this.scroller, dataSource = this.dataSource;
                if (this._pulled) {
                    scroller.pullHandled();
                }
                if (this._pulled || !this._first) {
                    var view = dataSource.view();
                    if (view[0]) {
                        this._first = view[0];
                    }
                }
                this._pulled = false;
            }
        });
        var VirtualList = kendo.Observable.extend({
            init: function (options) {
                var list = this;
                kendo.Observable.fn.init.call(list);
                list.buffer = options.buffer;
                list.height = options.height;
                list.item = options.item;
                list.items = [];
                list.footer = options.footer;
                list.buffer.bind('reset', function () {
                    list.refresh();
                });
            },
            refresh: function () {
                var buffer = this.buffer, items = this.items, endReached = false;
                while (items.length) {
                    items.pop().destroy();
                }
                this.offset = buffer.offset;
                var itemConstructor = this.item, prevItem, item;
                for (var idx = 0; idx < buffer.viewSize; idx++) {
                    if (idx === buffer.total()) {
                        endReached = true;
                        break;
                    }
                    item = itemConstructor(this.content(this.offset + items.length));
                    item.below(prevItem);
                    prevItem = item;
                    items.push(item);
                }
                this.itemCount = items.length;
                this.trigger('reset');
                this._resize();
                if (endReached) {
                    this.trigger('endReached');
                }
            },
            totalHeight: function () {
                if (!this.items[0]) {
                    return 0;
                }
                var list = this, items = list.items, top = items[0].top, bottom = items[items.length - 1].bottom, averageItemHeight = (bottom - top) / list.itemCount, remainingItemsCount = list.buffer.length - list.offset - list.itemCount;
                return (this.footer ? this.footer.height : 0) + bottom + remainingItemsCount * averageItemHeight;
            },
            batchUpdate: function (top) {
                var height = this.height(), items = this.items, item, initialOffset = this.offset;
                if (!items[0]) {
                    return;
                }
                if (this.lastDirection) {
                    while (items[items.length - 1].bottom > top + height * 2) {
                        if (this.offset === 0) {
                            break;
                        }
                        this.offset--;
                        item = items.pop();
                        item.update(this.content(this.offset));
                        item.above(items[0]);
                        items.unshift(item);
                    }
                } else {
                    while (items[0].top < top - height) {
                        var nextIndex = this.offset + this.itemCount;
                        if (nextIndex === this.buffer.total()) {
                            this.trigger('endReached');
                            break;
                        }
                        if (nextIndex === this.buffer.length) {
                            break;
                        }
                        item = items.shift();
                        item.update(this.content(this.offset + this.itemCount));
                        item.below(items[items.length - 1]);
                        items.push(item);
                        this.offset++;
                    }
                }
                if (initialOffset !== this.offset) {
                    this._resize();
                }
            },
            update: function (top) {
                var list = this, items = this.items, item, firstItem, lastItem, height = this.height(), itemCount = this.itemCount, padding = height / 2, up = (this.lastTop || 0) > top, topBorder = top - padding, bottomBorder = top + height + padding;
                if (!items[0]) {
                    return;
                }
                this.lastTop = top;
                this.lastDirection = up;
                if (up) {
                    if (items[0].top > topBorder && items[items.length - 1].bottom > bottomBorder + padding && this.offset > 0) {
                        this.offset--;
                        item = items.pop();
                        firstItem = items[0];
                        item.update(this.content(this.offset));
                        items.unshift(item);
                        item.above(firstItem);
                        list._resize();
                    }
                } else {
                    if (items[items.length - 1].bottom < bottomBorder && items[0].top < topBorder - padding) {
                        var nextIndex = this.offset + itemCount;
                        if (nextIndex === this.buffer.total()) {
                            this.trigger('endReached');
                        } else if (nextIndex !== this.buffer.length) {
                            item = items.shift();
                            lastItem = items[items.length - 1];
                            items.push(item);
                            item.update(this.content(this.offset + this.itemCount));
                            list.offset++;
                            item.below(lastItem);
                            list._resize();
                        }
                    }
                }
            },
            content: function (index) {
                return this.buffer.at(index);
            },
            destroy: function () {
                this.unbind();
            },
            _resize: function () {
                var items = this.items, top = 0, bottom = 0, firstItem = items[0], lastItem = items[items.length - 1];
                if (firstItem) {
                    top = firstItem.top;
                    bottom = lastItem.bottom;
                }
                this.trigger('resize', {
                    top: top,
                    bottom: bottom
                });
                if (this.footer) {
                    this.footer.below(lastItem);
                }
            }
        });
        kendo.mobile.ui.VirtualList = VirtualList;
        var VirtualListViewItem = kendo.Class.extend({
            init: function (listView, dataItem) {
                var element = listView.append([dataItem], true)[0], height = element.offsetHeight;
                $.extend(this, {
                    top: 0,
                    element: element,
                    listView: listView,
                    height: height,
                    bottom: height
                });
            },
            update: function (dataItem) {
                this.element = this.listView.setDataItem(this.element, dataItem);
            },
            above: function (item) {
                if (item) {
                    this.height = this.element.offsetHeight;
                    this.top = item.top - this.height;
                    this.bottom = item.top;
                    putAt(this.element, this.top);
                }
            },
            below: function (item) {
                if (item) {
                    this.height = this.element.offsetHeight;
                    this.top = item.bottom;
                    this.bottom = this.top + this.height;
                    putAt(this.element, this.top);
                }
            },
            destroy: function () {
                kendo.destroy(this.element);
                $(this.element).remove();
            }
        });
        var LOAD_ICON = '<div><span class="km-icon"></span><span class="km-loading-left"></span><span class="km-loading-right"></span></div>';
        var VirtualListViewLoadingIndicator = kendo.Class.extend({
            init: function (listView) {
                this.element = $('<li class="km-load-more km-scroller-refresh" style="display: none"></li>').appendTo(listView.element);
                this._loadIcon = $(LOAD_ICON).appendTo(this.element);
            },
            enable: function () {
                this.element.show();
                this.height = outerHeight(this.element, true);
            },
            disable: function () {
                this.element.hide();
                this.height = 0;
            },
            below: function (item) {
                if (item) {
                    this.top = item.bottom;
                    this.bottom = this.height + this.top;
                    putAt(this.element, this.top);
                }
            }
        });
        var VirtualListViewPressToLoadMore = VirtualListViewLoadingIndicator.extend({
            init: function (listView, buffer) {
                this._loadIcon = $(LOAD_ICON).hide();
                this._loadButton = $('<a class="km-load">' + listView.options.messages.loadMoreText + '</a>').hide();
                this.element = $('<li class="km-load-more" style="display: none"></li>').append(this._loadIcon).append(this._loadButton).appendTo(listView.element);
                var loadMore = this;
                this._loadButton.kendoMobileButton().data('kendoMobileButton').bind('click', function () {
                    loadMore._hideShowButton();
                    buffer.next();
                });
                buffer.bind('resize', function () {
                    loadMore._showLoadButton();
                });
                this.height = outerHeight(this.element, true);
                this.disable();
            },
            _hideShowButton: function () {
                this._loadButton.hide();
                this.element.addClass('km-scroller-refresh');
                this._loadIcon.css('display', 'block');
            },
            _showLoadButton: function () {
                this._loadButton.show();
                this.element.removeClass('km-scroller-refresh');
                this._loadIcon.hide();
            }
        });
        var VirtualListViewItemBinder = kendo.Class.extend({
            init: function (listView) {
                var binder = this;
                this.chromeHeight = outerHeight(listView.wrapper.children().not(listView.element));
                this.listView = listView;
                this.scroller = listView.scroller();
                this.options = listView.options;
                listView.bind('_dataSource', function (e) {
                    binder.setDataSource(e.dataSource, e.empty);
                });
                listView.bind('resize', function () {
                    if (!binder.list.items.length) {
                        return;
                    }
                    binder.scroller.reset();
                    binder.buffer.range(0);
                    binder.list.refresh();
                });
                this.scroller.makeVirtual();
                this._scroll = function (e) {
                    binder.list.update(e.scrollTop);
                };
                this.scroller.bind('scroll', this._scroll);
                this._scrollEnd = function (e) {
                    binder.list.batchUpdate(e.scrollTop);
                };
                this.scroller.bind('scrollEnd', this._scrollEnd);
            },
            destroy: function () {
                this.list.unbind();
                this.buffer.unbind();
                this.scroller.unbind('scroll', this._scroll);
                this.scroller.unbind('scrollEnd', this._scrollEnd);
            },
            setDataSource: function (dataSource, empty) {
                var binder = this, options = this.options, listView = this.listView, scroller = listView.scroller(), pressToLoadMore = options.loadMore, pageSize, buffer, footer;
                this.dataSource = dataSource;
                pageSize = dataSource.pageSize() || options.virtualViewSize;
                if (!pageSize && !empty) {
                    throw new Error('the DataSource does not have page size configured. Page Size setting is mandatory for the mobile listview virtual scrolling to work as expected.');
                }
                if (this.buffer) {
                    this.buffer.destroy();
                }
                buffer = new kendo.data.Buffer(dataSource, Math.floor(pageSize / 2), pressToLoadMore);
                if (pressToLoadMore) {
                    footer = new VirtualListViewPressToLoadMore(listView, buffer);
                } else {
                    footer = new VirtualListViewLoadingIndicator(listView);
                }
                if (this.list) {
                    this.list.destroy();
                }
                var list = new VirtualList({
                    buffer: buffer,
                    footer: footer,
                    item: function (dataItem) {
                        return new VirtualListViewItem(listView, dataItem);
                    },
                    height: function () {
                        return scroller.height();
                    }
                });
                list.bind('resize', function () {
                    binder.updateScrollerSize();
                    listView.updateSize();
                });
                list.bind('reset', function () {
                    binder.footer.enable();
                });
                list.bind('endReached', function () {
                    footer.disable();
                    binder.updateScrollerSize();
                });
                buffer.bind('expand', function () {
                    list.lastDirection = false;
                    list.batchUpdate(scroller.scrollTop);
                });
                $.extend(this, {
                    buffer: buffer,
                    scroller: scroller,
                    list: list,
                    footer: footer
                });
            },
            updateScrollerSize: function () {
                this.scroller.virtualSize(0, this.list.totalHeight() + this.chromeHeight);
            },
            refresh: function () {
                this.list.refresh();
            },
            reset: function () {
                this.buffer.range(0);
                this.list.refresh();
            }
        });
        var ListViewItemBinder = kendo.Class.extend({
            init: function (listView) {
                var binder = this;
                this.listView = listView;
                this.options = listView.options;
                var itemBinder = this;
                this._refreshHandler = function (e) {
                    itemBinder.refresh(e);
                };
                this._progressHandler = function () {
                    listView.showLoading();
                };
                listView.bind('_dataSource', function (e) {
                    binder.setDataSource(e.dataSource);
                });
            },
            destroy: function () {
                this._unbindDataSource();
            },
            reset: function () {
            },
            refresh: function (e) {
                var action = e && e.action, dataItems = e && e.items, listView = this.listView, dataSource = this.dataSource, prependOnRefresh = this.options.appendOnRefresh, view = dataSource.view(), groups = dataSource.group(), groupedMode = groups && groups[0], item;
                if (action === 'itemchange') {
                    if (!listView._hasBindingTarget()) {
                        item = listView.findByDataItem(dataItems)[0];
                        if (item) {
                            listView.setDataItem(item, dataItems[0]);
                        }
                    }
                    return;
                }
                var removedItems, addedItems, addedDataItems;
                var adding = action === 'add' && !groupedMode || prependOnRefresh && !listView._filter;
                var removing = action === 'remove' && !groupedMode;
                if (adding) {
                    removedItems = [];
                } else if (removing) {
                    removedItems = listView.findByDataItem(dataItems);
                }
                if (listView.trigger(DATABINDING, {
                        action: action || 'rebind',
                        items: dataItems,
                        removedItems: removedItems,
                        index: e && e.index
                    })) {
                    if (this._shouldShowLoading()) {
                        listView.hideLoading();
                    }
                    return;
                }
                if (action === 'add' && !groupedMode) {
                    var index = view.indexOf(dataItems[0]);
                    if (index > -1) {
                        addedItems = listView.insertAt(dataItems, index);
                        addedDataItems = dataItems;
                    }
                } else if (action === 'remove' && !groupedMode) {
                    addedItems = [];
                    listView.remove(dataItems);
                } else if (groupedMode) {
                    listView.replaceGrouped(view);
                } else if (prependOnRefresh && !listView._filter) {
                    addedItems = listView.prepend(view);
                    addedDataItems = view;
                } else {
                    listView.replace(view);
                }
                if (this._shouldShowLoading()) {
                    listView.hideLoading();
                }
                listView.trigger(DATABOUND, {
                    ns: ui,
                    addedItems: addedItems,
                    addedDataItems: addedDataItems
                });
            },
            setDataSource: function (dataSource) {
                if (this.dataSource) {
                    this._unbindDataSource();
                }
                this.dataSource = dataSource;
                dataSource.bind(CHANGE, this._refreshHandler);
                if (this._shouldShowLoading()) {
                    this.dataSource.bind(PROGRESS, this._progressHandler);
                }
            },
            _unbindDataSource: function () {
                this.dataSource.unbind(CHANGE, this._refreshHandler).unbind(PROGRESS, this._progressHandler);
            },
            _shouldShowLoading: function () {
                var options = this.options;
                return !options.pullToRefresh && !options.loadMore && !options.endlessScroll;
            }
        });
        var ListViewFilter = kendo.Class.extend({
            init: function (listView) {
                var filter = this, filterable = listView.options.filterable, events = 'change paste', that = this;
                this.listView = listView;
                this.options = filterable;
                listView.element.before(SEARCH_TEMPLATE({ placeholder: filterable.placeholder || 'Search...' }));
                if (filterable.autoFilter !== false) {
                    events += ' keyup';
                }
                this.element = listView.wrapper.find('.km-search-form');
                this.searchInput = listView.wrapper.find('input[type=search]').closest('form').on('submit' + NS, function (e) {
                    e.preventDefault();
                }).end().on('focus' + NS, function () {
                    filter._oldFilter = filter.searchInput.val();
                }).on(events.split(' ').join(NS + ' ') + NS, proxy(this._filterChange, this));
                this.clearButton = listView.wrapper.find('.km-filter-reset').on(CLICK, proxy(this, '_clearFilter')).hide();
                this._dataSourceChange = $.proxy(this._refreshInput, this);
                listView.bind('_dataSource', function (e) {
                    e.dataSource.bind('change', that._dataSourceChange);
                });
            },
            _refreshInput: function () {
                var appliedFilters = this.listView.dataSource.filter();
                var searchInput = this.listView._filter.searchInput;
                if (!appliedFilters || appliedFilters.filters[0].field !== this.listView.options.filterable.field) {
                    searchInput.val('');
                } else {
                    searchInput.val(appliedFilters.filters[0].value);
                }
            },
            _search: function (expr) {
                this._filter = true;
                this.clearButton[expr ? 'show' : 'hide']();
                this.listView.dataSource.filter(expr);
            },
            _filterChange: function (e) {
                var filter = this;
                if (e.type == 'paste' && this.options.autoFilter !== false) {
                    setTimeout(function () {
                        filter._applyFilter();
                    }, 1);
                } else {
                    this._applyFilter();
                }
            },
            _applyFilter: function () {
                var options = this.options, value = this.searchInput.val(), expr = value.length ? {
                        field: options.field,
                        operator: options.operator || 'startswith',
                        ignoreCase: options.ignoreCase,
                        value: value
                    } : null;
                if (value === this._oldFilter) {
                    return;
                }
                this._oldFilter = value;
                this._search(expr);
            },
            _clearFilter: function (e) {
                this.searchInput.val('');
                this._search(null);
                e.preventDefault();
            }
        });
        var ListView = Widget.extend({
            init: function (element, options) {
                var listView = this;
                Widget.fn.init.call(this, element, options);
                element = this.element;
                options = this.options;
                if (options.scrollTreshold) {
                    options.scrollThreshold = options.scrollTreshold;
                }
                element.on('down', HIGHLIGHT_SELECTOR, '_highlight').on('move up cancel', HIGHLIGHT_SELECTOR, '_dim');
                this._userEvents = new kendo.UserEvents(element, {
                    fastTap: true,
                    filter: ITEM_SELECTOR,
                    allowSelection: true,
                    tap: function (e) {
                        listView._click(e);
                    }
                });
                element.css('-ms-touch-action', 'auto');
                element.wrap(WRAPPER);
                this.wrapper = this.element.parent();
                this._headerFixer = new HeaderFixer(this);
                this._itemsCache = {};
                this._templates();
                this.virtual = options.endlessScroll || options.loadMore;
                this._style();
                if (this.options.$angular && (this.virtual || this.options.pullToRefresh)) {
                    setTimeout($.proxy(this, '_start'));
                } else {
                    this._start();
                }
            },
            _start: function () {
                var options = this.options;
                if (this.options.filterable) {
                    this._filter = new ListViewFilter(this);
                }
                if (this.virtual) {
                    this._itemBinder = new VirtualListViewItemBinder(this);
                } else {
                    this._itemBinder = new ListViewItemBinder(this);
                }
                if (this.options.pullToRefresh) {
                    this._pullToRefreshHandler = new RefreshHandler(this);
                }
                this.setDataSource(options.dataSource);
                this._enhanceItems(this.items());
                kendo.notify(this, ui);
            },
            events: [
                CLICK,
                DATABINDING,
                DATABOUND,
                ITEM_CHANGE
            ],
            options: {
                name: 'ListView',
                style: '',
                type: 'flat',
                autoBind: true,
                fixedHeaders: false,
                template: '#:data#',
                headerTemplate: '<span class="km-text">#:value#</span>',
                appendOnRefresh: false,
                loadMore: false,
                endlessScroll: false,
                scrollThreshold: 30,
                pullToRefresh: false,
                messages: {
                    loadMoreText: 'Press to load more',
                    pullTemplate: 'Pull to refresh',
                    releaseTemplate: 'Release to refresh',
                    refreshTemplate: 'Refreshing'
                },
                pullOffset: 140,
                filterable: false,
                virtualViewSize: null
            },
            refresh: function () {
                this._itemBinder.refresh();
            },
            reset: function () {
                this._itemBinder.reset();
            },
            setDataSource: function (dataSource) {
                var emptyDataSource = !dataSource;
                this.dataSource = DataSource.create(dataSource);
                this.trigger('_dataSource', {
                    dataSource: this.dataSource,
                    empty: emptyDataSource
                });
                if (this.options.autoBind && !emptyDataSource) {
                    this.items().remove();
                    this.dataSource.fetch();
                }
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                kendo.destroy(this.element);
                this._userEvents.destroy();
                if (this._itemBinder) {
                    this._itemBinder.destroy();
                }
                if (this._headerFixer) {
                    this._headerFixer.destroy();
                }
                this.element.unwrap();
                delete this.element;
                delete this.wrapper;
                delete this._userEvents;
            },
            items: function () {
                if (this.options.type === 'group') {
                    return this.element.find('.km-list').children();
                } else {
                    return this.element.children().not('.km-load-more');
                }
            },
            scroller: function () {
                if (!this._scrollerInstance) {
                    this._scrollerInstance = this.element.closest('.km-scroll-wrapper').data('kendoMobileScroller');
                }
                return this._scrollerInstance;
            },
            showLoading: function () {
                var view = this.view();
                if (view && view.loader) {
                    view.loader.show();
                }
            },
            hideLoading: function () {
                var view = this.view();
                if (view && view.loader) {
                    view.loader.hide();
                }
            },
            insertAt: function (dataItems, index, triggerChange) {
                var listView = this;
                return listView._renderItems(dataItems, function (items) {
                    if (index === 0) {
                        listView.element.prepend(items);
                    } else if (index === -1) {
                        listView.element.append(items);
                    } else {
                        listView.items().eq(index - 1).after(items);
                    }
                    if (triggerChange) {
                        for (var i = 0; i < items.length; i++) {
                            listView.trigger(ITEM_CHANGE, {
                                item: items.eq(i),
                                data: dataItems[i],
                                ns: ui
                            });
                        }
                    }
                });
            },
            append: function (dataItems, triggerChange) {
                return this.insertAt(dataItems, -1, triggerChange);
            },
            prepend: function (dataItems, triggerChange) {
                return this.insertAt(dataItems, 0, triggerChange);
            },
            replace: function (dataItems) {
                this.options.type = 'flat';
                this._angularItems('cleanup');
                kendo.destroy(this.element.children());
                this.element.empty();
                this._userEvents.cancel();
                this._style();
                return this.insertAt(dataItems, 0);
            },
            replaceGrouped: function (groups) {
                this.options.type = 'group';
                this._angularItems('cleanup');
                this.element.empty();
                var items = $(kendo.render(this.groupTemplate, groups));
                this._enhanceItems(items.children('ul').children('li'));
                this.element.append(items);
                mobile.init(items);
                this._style();
                this._angularItems('compile');
            },
            remove: function (dataItems) {
                var items = this.findByDataItem(dataItems);
                this.angular('cleanup', function () {
                    return { elements: items };
                });
                kendo.destroy(items);
                items.remove();
            },
            findByDataItem: function (dataItems) {
                var selectors = [];
                for (var idx = 0, length = dataItems.length; idx < length; idx++) {
                    selectors[idx] = '[data-' + kendo.ns + 'uid=' + dataItems[idx].uid + ']';
                }
                return this.element.find(selectors.join(','));
            },
            setDataItem: function (item, dataItem) {
                var listView = this, replaceItem = function (items) {
                        var newItem = $(items[0]);
                        kendo.destroy(item);
                        listView.angular('cleanup', function () {
                            return { elements: [$(item)] };
                        });
                        $(item).replaceWith(newItem);
                        listView.trigger(ITEM_CHANGE, {
                            item: newItem,
                            data: dataItem,
                            ns: ui
                        });
                    };
                return this._renderItems([dataItem], replaceItem)[0];
            },
            updateSize: function () {
                this._size = this.getSize();
            },
            _renderItems: function (dataItems, callback) {
                var items = $(kendo.render(this.template, dataItems));
                callback(items);
                this.angular('compile', function () {
                    return {
                        elements: items,
                        data: dataItems.map(function (data) {
                            return { dataItem: data };
                        })
                    };
                });
                mobile.init(items);
                this._enhanceItems(items);
                return items;
            },
            _dim: function (e) {
                this._toggle(e, false);
            },
            _highlight: function (e) {
                this._toggle(e, true);
            },
            _toggle: function (e, highlight) {
                if (e.which > 1) {
                    return;
                }
                var clicked = $(e.currentTarget), item = clicked.parent(), role = attrValue(clicked, 'role') || '', plainItem = !role.match(buttonRegExp), prevented = e.isDefaultPrevented();
                if (plainItem) {
                    item.toggleClass(ACTIVE_CLASS, highlight && !prevented);
                }
            },
            _templates: function () {
                var template = this.options.template, headerTemplate = this.options.headerTemplate, dataIDAttribute = ' data-uid="#=arguments[0].uid || ""#"', templateProxy = {}, groupTemplateProxy = {};
                if (typeof template === FUNCTION) {
                    templateProxy.template = template;
                    template = '#=this.template(data)#';
                }
                this.template = proxy(kendo.template('<li' + dataIDAttribute + '>' + template + '</li>'), templateProxy);
                groupTemplateProxy.template = this.template;
                if (typeof headerTemplate === FUNCTION) {
                    groupTemplateProxy._headerTemplate = headerTemplate;
                    headerTemplate = '#=this._headerTemplate(data)#';
                }
                groupTemplateProxy.headerTemplate = kendo.template(headerTemplate);
                this.groupTemplate = proxy(GROUP_TEMPLATE, groupTemplateProxy);
            },
            _click: function (e) {
                if (e.event.which > 1 || e.event.isDefaultPrevented()) {
                    return;
                }
                var dataItem, item = e.target, target = $(e.event.target), buttonElement = target.closest(kendo.roleSelector('button', 'detailbutton', 'backbutton')), button = kendo.widgetInstance(buttonElement, ui), id = item.attr(kendo.attr('uid'));
                if (id) {
                    dataItem = this.dataSource.getByUid(id);
                }
                if (this.trigger(CLICK, {
                        target: target,
                        item: item,
                        dataItem: dataItem,
                        button: button
                    })) {
                    e.preventDefault();
                }
            },
            _styleGroups: function () {
                var rootItems = this.element.children();
                rootItems.children('ul').addClass('km-list');
                rootItems.each(function () {
                    var li = $(this), groupHeader = li.contents().first();
                    li.addClass('km-group-container');
                    if (!groupHeader.is('ul') && !groupHeader.is('div.' + GROUP_CLASS)) {
                        groupHeader.wrap(GROUP_WRAPPER);
                    }
                });
            },
            _style: function () {
                var options = this.options, grouped = options.type === 'group', element = this.element, inset = options.style === 'inset';
                element.addClass('km-listview').toggleClass('km-list', !grouped).toggleClass('km-virtual-list', this.virtual).toggleClass('km-listinset', !grouped && inset).toggleClass('km-listgroup', grouped && !inset).toggleClass('km-listgroupinset', grouped && inset);
                if (!element.parents('.km-listview')[0]) {
                    element.closest('.km-content').toggleClass('km-insetcontent', inset);
                }
                if (grouped) {
                    this._styleGroups();
                }
                this.trigger(STYLED);
            },
            _enhanceItems: function (items) {
                items.each(function () {
                    var item = $(this), child, enhanced = false;
                    item.children().each(function () {
                        child = $(this);
                        if (child.is('a')) {
                            enhanceLinkItem(child);
                            enhanced = true;
                        } else if (child.is('label')) {
                            enhanceCheckBoxItem(child);
                            enhanced = true;
                        }
                    });
                    if (!enhanced) {
                        enhanceItem(item);
                    }
                });
            }
        });
        ui.plugin(ListView);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.navbar', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'mobile.navbar',
        name: 'NavBar',
        category: 'mobile',
        description: 'The Kendo mobile NavBar widget is used inside a mobile View or Layout Header element to display an application navigation bar.',
        depends: ['core']
    };
    (function ($, undefined) {
        var kendo = window.kendo, mobile = kendo.mobile, ui = mobile.ui, Widget = ui.Widget;
        function createContainer(align, element) {
            var items = element.find('[' + kendo.attr('align') + '=' + align + ']');
            if (items[0]) {
                return $('<div class="km-' + align + 'item" />').append(items).prependTo(element);
            }
        }
        function toggleTitle(centerElement) {
            var siblings = centerElement.siblings(), noTitle = !!centerElement.children('ul')[0], showTitle = !!siblings[0] && $.trim(centerElement.text()) === '', android = !!(kendo.mobile.application && kendo.mobile.application.element.is('.km-android'));
            centerElement.prevAll().toggleClass('km-absolute', noTitle);
            centerElement.toggleClass('km-show-title', showTitle);
            centerElement.toggleClass('km-fill-title', showTitle && !$.trim(centerElement.html()));
            centerElement.toggleClass('km-no-title', noTitle);
            centerElement.toggleClass('km-hide-title', android && !siblings.children().is(':visible'));
        }
        var NavBar = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                element = that.element;
                that.container().bind('show', $.proxy(this, 'refresh'));
                element.addClass('km-navbar').wrapInner($('<div class="km-view-title km-show-title" />'));
                that.leftElement = createContainer('left', element);
                that.rightElement = createContainer('right', element);
                that.centerElement = element.find('.km-view-title');
            },
            options: { name: 'NavBar' },
            title: function (value) {
                this.element.find(kendo.roleSelector('view-title')).text(value);
                toggleTitle(this.centerElement);
            },
            refresh: function (e) {
                var view = e.view;
                this.title(view.options.title);
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                kendo.destroy(this.element);
            }
        });
        ui.plugin(NavBar);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.scrollview', [
        'kendo.fx',
        'kendo.data',
        'kendo.draganddrop'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.scrollview',
        name: 'ScrollView',
        category: 'mobile',
        description: 'The Kendo Mobile ScrollView widget is used to scroll content wider than the device screen.',
        depends: [
            'fx',
            'data',
            'draganddrop'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, mobile = kendo.mobile, ui = mobile.ui, proxy = $.proxy, Transition = kendo.effects.Transition, Pane = kendo.ui.Pane, PaneDimensions = kendo.ui.PaneDimensions, Widget = ui.DataBoundWidget, DataSource = kendo.data.DataSource, Buffer = kendo.data.Buffer, BatchBuffer = kendo.data.BatchBuffer, math = Math, abs = math.abs, ceil = math.ceil, round = math.round, max = math.max, min = math.min, floor = math.floor, CHANGE = 'change', CHANGING = 'changing', REFRESH = 'refresh', CURRENT_PAGE_CLASS = 'current-page', VIRTUAL_PAGE_CLASS = 'virtual-page', FUNCTION = 'function', ITEM_CHANGE = 'itemChange', CLEANUP = 'cleanup', VIRTUAL_PAGE_COUNT = 3, LEFT_PAGE = -1, CETER_PAGE = 0, RIGHT_PAGE = 1, LEFT_SWIPE = -1, NUDGE = 0, RIGHT_SWIPE = 1;
        function className(name) {
            return 'k-' + name + ' km-' + name;
        }
        var Pager = kendo.Class.extend({
            init: function (scrollView) {
                var that = this, element = $('<ol class=\'' + className('pages') + '\'/>');
                scrollView.element.append(element);
                this._changeProxy = proxy(that, '_change');
                this._refreshProxy = proxy(that, '_refresh');
                scrollView.bind(CHANGE, this._changeProxy);
                scrollView.bind(REFRESH, this._refreshProxy);
                $.extend(that, {
                    element: element,
                    scrollView: scrollView
                });
            },
            items: function () {
                return this.element.children();
            },
            _refresh: function (e) {
                var pageHTML = '';
                for (var idx = 0; idx < e.pageCount; idx++) {
                    pageHTML += '<li/>';
                }
                this.element.html(pageHTML);
                this.items().eq(e.page).addClass(className(CURRENT_PAGE_CLASS));
            },
            _change: function (e) {
                this.items().removeClass(className(CURRENT_PAGE_CLASS)).eq(e.page).addClass(className(CURRENT_PAGE_CLASS));
            },
            destroy: function () {
                this.scrollView.unbind(CHANGE, this._changeProxy);
                this.scrollView.unbind(REFRESH, this._refreshProxy);
                this.element.remove();
            }
        });
        kendo.mobile.ui.ScrollViewPager = Pager;
        var TRANSITION_END = 'transitionEnd', DRAG_START = 'dragStart', DRAG_END = 'dragEnd';
        var ElasticPane = kendo.Observable.extend({
            init: function (element, options) {
                var that = this;
                kendo.Observable.fn.init.call(this);
                this.element = element;
                this.container = element.parent();
                var movable, transition, userEvents, dimensions, dimension, pane;
                movable = new kendo.ui.Movable(that.element);
                transition = new Transition({
                    axis: 'x',
                    movable: movable,
                    onEnd: function () {
                        that.trigger(TRANSITION_END);
                    }
                });
                userEvents = new kendo.UserEvents(element, {
                    fastTap: true,
                    start: function (e) {
                        if (abs(e.x.velocity) * 2 >= abs(e.y.velocity)) {
                            userEvents.capture();
                        } else {
                            userEvents.cancel();
                        }
                        that.trigger(DRAG_START, e);
                        transition.cancel();
                    },
                    allowSelection: true,
                    end: function (e) {
                        that.trigger(DRAG_END, e);
                    }
                });
                dimensions = new PaneDimensions({
                    element: that.element,
                    container: that.container
                });
                dimension = dimensions.x;
                dimension.bind(CHANGE, function () {
                    that.trigger(CHANGE);
                });
                pane = new Pane({
                    dimensions: dimensions,
                    userEvents: userEvents,
                    movable: movable,
                    elastic: true
                });
                $.extend(that, {
                    duration: options && options.duration || 1,
                    movable: movable,
                    transition: transition,
                    userEvents: userEvents,
                    dimensions: dimensions,
                    dimension: dimension,
                    pane: pane
                });
                this.bind([
                    TRANSITION_END,
                    DRAG_START,
                    DRAG_END,
                    CHANGE
                ], options);
            },
            size: function () {
                return {
                    width: this.dimensions.x.getSize(),
                    height: this.dimensions.y.getSize()
                };
            },
            total: function () {
                return this.dimension.getTotal();
            },
            offset: function () {
                return -this.movable.x;
            },
            updateDimension: function () {
                this.dimension.update(true);
            },
            refresh: function () {
                this.dimensions.refresh();
            },
            moveTo: function (offset) {
                this.movable.moveAxis('x', -offset);
            },
            transitionTo: function (offset, ease, instant) {
                if (instant) {
                    this.moveTo(-offset);
                } else {
                    this.transition.moveTo({
                        location: offset,
                        duration: this.duration,
                        ease: ease
                    });
                }
            }
        });
        kendo.mobile.ui.ScrollViewElasticPane = ElasticPane;
        var ScrollViewContent = kendo.Observable.extend({
            init: function (element, pane, options) {
                var that = this;
                kendo.Observable.fn.init.call(this);
                that.element = element;
                that.pane = pane;
                that._getPages();
                this.page = 0;
                this.pageSize = options.pageSize || 1;
                this.contentHeight = options.contentHeight;
                this.enablePager = options.enablePager;
                this.pagerOverlay = options.pagerOverlay;
            },
            scrollTo: function (page, instant) {
                this.page = page;
                this.pane.transitionTo(-page * this.pane.size().width, Transition.easeOutExpo, instant);
            },
            paneMoved: function (swipeType, bounce, callback, instant) {
                var that = this, pane = that.pane, width = pane.size().width * that.pageSize, approx = round, ease = bounce ? Transition.easeOutBack : Transition.easeOutExpo, snap, nextPage;
                if (swipeType === LEFT_SWIPE) {
                    approx = ceil;
                } else if (swipeType === RIGHT_SWIPE) {
                    approx = floor;
                }
                nextPage = approx(pane.offset() / width);
                snap = max(that.minSnap, min(-nextPage * width, that.maxSnap));
                if (nextPage != that.page) {
                    if (callback && callback({
                            currentPage: that.page,
                            nextPage: nextPage
                        })) {
                        snap = -that.page * pane.size().width;
                    }
                }
                pane.transitionTo(snap, ease, instant);
            },
            updatePage: function () {
                var pane = this.pane, page = round(pane.offset() / pane.size().width);
                if (page != this.page) {
                    this.page = page;
                    return true;
                }
                return false;
            },
            forcePageUpdate: function () {
                return this.updatePage();
            },
            resizeTo: function (size) {
                var pane = this.pane, width = size.width;
                this.pageElements.width(width);
                if (this.contentHeight === '100%') {
                    var containerHeight = this.element.parent().height();
                    if (this.enablePager === true) {
                        var pager = this.element.parent().find('ol.km-pages');
                        if (!this.pagerOverlay && pager.length) {
                            containerHeight -= kendo._outerHeight(pager, true);
                        }
                    }
                    this.element.css('height', containerHeight);
                    this.pageElements.css('height', containerHeight);
                }
                pane.updateDimension();
                if (!this._paged) {
                    this.page = floor(pane.offset() / width);
                }
                this.scrollTo(this.page, true);
                this.pageCount = ceil(pane.total() / width);
                this.minSnap = -(this.pageCount - 1) * width;
                this.maxSnap = 0;
            },
            _getPages: function () {
                this.pageElements = this.element.find(kendo.roleSelector('page'));
                this._paged = this.pageElements.length > 0;
            }
        });
        kendo.mobile.ui.ScrollViewContent = ScrollViewContent;
        var VirtualScrollViewContent = kendo.Observable.extend({
            init: function (element, pane, options) {
                var that = this;
                kendo.Observable.fn.init.call(this);
                that.element = element;
                that.pane = pane;
                that.options = options;
                that._templates();
                that.page = options.page || 0;
                that.pages = [];
                that._initPages();
                that.resizeTo(that.pane.size());
                that.pane.dimension.forceEnabled();
            },
            setDataSource: function (dataSource) {
                this.dataSource = DataSource.create(dataSource);
                this._buffer();
                this._pendingPageRefresh = false;
                this._pendingWidgetRefresh = false;
            },
            _viewShow: function () {
                var that = this;
                if (that._pendingWidgetRefresh) {
                    setTimeout(function () {
                        that._resetPages();
                    }, 0);
                    that._pendingWidgetRefresh = false;
                }
            },
            _buffer: function () {
                var itemsPerPage = this.options.itemsPerPage;
                if (this.buffer) {
                    this.buffer.destroy();
                }
                if (itemsPerPage > 1) {
                    this.buffer = new BatchBuffer(this.dataSource, itemsPerPage);
                } else {
                    this.buffer = new Buffer(this.dataSource, itemsPerPage * 3);
                }
                this._resizeProxy = proxy(this, '_onResize');
                this._resetProxy = proxy(this, '_onReset');
                this._endReachedProxy = proxy(this, '_onEndReached');
                this.buffer.bind({
                    'resize': this._resizeProxy,
                    'reset': this._resetProxy,
                    'endreached': this._endReachedProxy
                });
            },
            _templates: function () {
                var template = this.options.template, emptyTemplate = this.options.emptyTemplate, templateProxy = {}, emptyTemplateProxy = {};
                if (typeof template === FUNCTION) {
                    templateProxy.template = template;
                    template = '#=this.template(data)#';
                }
                this.template = proxy(kendo.template(template), templateProxy);
                if (typeof emptyTemplate === FUNCTION) {
                    emptyTemplateProxy.emptyTemplate = emptyTemplate;
                    emptyTemplate = '#=this.emptyTemplate(data)#';
                }
                this.emptyTemplate = proxy(kendo.template(emptyTemplate), emptyTemplateProxy);
            },
            _initPages: function () {
                var pages = this.pages, element = this.element, page;
                for (var i = 0; i < VIRTUAL_PAGE_COUNT; i++) {
                    page = new Page(element);
                    pages.push(page);
                }
                this.pane.updateDimension();
            },
            resizeTo: function (size) {
                var pages = this.pages, pane = this.pane;
                for (var i = 0; i < pages.length; i++) {
                    pages[i].setWidth(size.width);
                }
                if (this.options.contentHeight === 'auto') {
                    this.element.css('height', this.pages[1].element.height());
                } else if (this.options.contentHeight === '100%') {
                    var containerHeight = this.element.parent().height();
                    if (this.options.enablePager === true) {
                        var pager = this.element.parent().find('ol.km-pages');
                        if (!this.options.pagerOverlay && pager.length) {
                            containerHeight -= kendo._outerHeight(pager, true);
                        }
                    }
                    this.element.css('height', containerHeight);
                    pages[0].element.css('height', containerHeight);
                    pages[1].element.css('height', containerHeight);
                    pages[2].element.css('height', containerHeight);
                }
                pane.updateDimension();
                this._repositionPages();
                this.width = size.width;
            },
            scrollTo: function (page) {
                var buffer = this.buffer, dataItem;
                buffer.syncDataSource();
                dataItem = buffer.at(page);
                if (!dataItem) {
                    return;
                }
                this._updatePagesContent(page);
                this.page = page;
            },
            paneMoved: function (swipeType, bounce, callback, instant) {
                var that = this, pane = that.pane, width = pane.size().width, offset = pane.offset(), thresholdPassed = Math.abs(offset) >= width / 3, ease = bounce ? kendo.effects.Transition.easeOutBack : kendo.effects.Transition.easeOutExpo, isEndReached = that.page + 2 > that.buffer.total(), nextPage, delta = 0;
                if (swipeType === RIGHT_SWIPE) {
                    if (that.page !== 0) {
                        delta = -1;
                    }
                } else if (swipeType === LEFT_SWIPE && !isEndReached) {
                    delta = 1;
                } else if (offset > 0 && (thresholdPassed && !isEndReached)) {
                    delta = 1;
                } else if (offset < 0 && thresholdPassed) {
                    if (that.page !== 0) {
                        delta = -1;
                    }
                }
                nextPage = that.page;
                if (delta) {
                    nextPage = delta > 0 ? nextPage + 1 : nextPage - 1;
                }
                if (callback && callback({
                        currentPage: that.page,
                        nextPage: nextPage
                    })) {
                    delta = 0;
                }
                if (delta === 0) {
                    that._cancelMove(ease, instant);
                } else if (delta === -1) {
                    that._moveBackward(instant);
                } else if (delta === 1) {
                    that._moveForward(instant);
                }
            },
            updatePage: function () {
                var pages = this.pages;
                if (this.pane.offset() === 0) {
                    return false;
                }
                if (this.pane.offset() > 0) {
                    pages.push(this.pages.shift());
                    this.page++;
                    this.setPageContent(pages[2], this.page + 1);
                } else {
                    pages.unshift(this.pages.pop());
                    this.page--;
                    this.setPageContent(pages[0], this.page - 1);
                }
                this._repositionPages();
                this._resetMovable();
                return true;
            },
            forcePageUpdate: function () {
                var offset = this.pane.offset(), threshold = this.pane.size().width * 3 / 4;
                if (abs(offset) > threshold) {
                    return this.updatePage();
                }
                return false;
            },
            _resetMovable: function () {
                this.pane.moveTo(0);
            },
            _moveForward: function (instant) {
                this.pane.transitionTo(-this.width, kendo.effects.Transition.easeOutExpo, instant);
            },
            _moveBackward: function (instant) {
                this.pane.transitionTo(this.width, kendo.effects.Transition.easeOutExpo, instant);
            },
            _cancelMove: function (ease, instant) {
                this.pane.transitionTo(0, ease, instant);
            },
            _resetPages: function () {
                this.page = this.options.page || 0;
                this._updatePagesContent(this.page);
                this._repositionPages();
                this.trigger('reset');
            },
            _onResize: function () {
                this.pageCount = ceil(this.dataSource.total() / this.options.itemsPerPage);
                if (this._pendingPageRefresh) {
                    this._updatePagesContent(this.page);
                    this._pendingPageRefresh = false;
                }
                this.trigger('resize');
            },
            _onReset: function () {
                this.pageCount = ceil(this.dataSource.total() / this.options.itemsPerPage);
                this._resetPages();
            },
            _onEndReached: function () {
                this._pendingPageRefresh = true;
            },
            _repositionPages: function () {
                var pages = this.pages;
                pages[0].position(LEFT_PAGE);
                pages[1].position(CETER_PAGE);
                pages[2].position(RIGHT_PAGE);
            },
            _updatePagesContent: function (offset) {
                var pages = this.pages, currentPage = offset || 0;
                this.setPageContent(pages[0], currentPage - 1);
                this.setPageContent(pages[1], currentPage);
                this.setPageContent(pages[2], currentPage + 1);
            },
            setPageContent: function (page, index) {
                var buffer = this.buffer, template = this.template, emptyTemplate = this.emptyTemplate, view = null;
                if (index >= 0) {
                    view = buffer.at(index);
                    if ($.isArray(view) && !view.length) {
                        view = null;
                    }
                }
                this.trigger(CLEANUP, { item: page.element });
                if (view !== null) {
                    page.content(template(view));
                } else {
                    page.content(emptyTemplate({}));
                }
                kendo.mobile.init(page.element);
                this.trigger(ITEM_CHANGE, {
                    item: page.element,
                    data: view,
                    ns: kendo.mobile.ui
                });
            }
        });
        kendo.mobile.ui.VirtualScrollViewContent = VirtualScrollViewContent;
        var Page = kendo.Class.extend({
            init: function (container) {
                this.element = $('<div class=\'' + className(VIRTUAL_PAGE_CLASS) + '\'></div>');
                this.width = container.width();
                this.element.width(this.width);
                container.append(this.element);
            },
            content: function (theContent) {
                this.element.html(theContent);
            },
            position: function (position) {
                this.element.css('transform', 'translate3d(' + this.width * position + 'px, 0, 0)');
            },
            setWidth: function (width) {
                this.width = width;
                this.element.width(width);
            }
        });
        kendo.mobile.ui.VirtualPage = Page;
        var ScrollView = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                options = that.options;
                element = that.element;
                kendo.stripWhitespace(element[0]);
                element.wrapInner('<div/>').addClass('k-widget ' + className('scrollview'));
                if (this.options.enablePager) {
                    this.pager = new Pager(this);
                    if (this.options.pagerOverlay) {
                        element.addClass(className('scrollview-overlay'));
                    }
                }
                that.inner = element.children().first();
                that.page = 0;
                that.inner.css('height', options.contentHeight);
                that.pane = new ElasticPane(that.inner, {
                    duration: this.options.duration,
                    transitionEnd: proxy(this, '_transitionEnd'),
                    dragStart: proxy(this, '_dragStart'),
                    dragEnd: proxy(this, '_dragEnd'),
                    change: proxy(this, REFRESH)
                });
                that.bind('resize', function () {
                    that.pane.refresh();
                });
                that.page = options.page;
                var empty = this.inner.children().length === 0;
                var content = empty ? new VirtualScrollViewContent(that.inner, that.pane, options) : new ScrollViewContent(that.inner, that.pane, options);
                content.page = that.page;
                content.bind('reset', function () {
                    this._pendingPageRefresh = false;
                    that._syncWithContent();
                    that.trigger(REFRESH, {
                        pageCount: content.pageCount,
                        page: content.page
                    });
                });
                content.bind('resize', function () {
                    that.trigger(REFRESH, {
                        pageCount: content.pageCount,
                        page: content.page
                    });
                });
                content.bind(ITEM_CHANGE, function (e) {
                    that.trigger(ITEM_CHANGE, e);
                    that.angular('compile', function () {
                        return {
                            elements: e.item,
                            data: [{ dataItem: e.data }]
                        };
                    });
                });
                content.bind(CLEANUP, function (e) {
                    that.angular('cleanup', function () {
                        return { elements: e.item };
                    });
                });
                that._content = content;
                that.setDataSource(options.dataSource);
                var mobileContainer = that.container();
                if (mobileContainer.nullObject) {
                    that.viewInit();
                    that.viewShow();
                } else {
                    mobileContainer.bind('show', proxy(this, 'viewShow')).bind('init', proxy(this, 'viewInit'));
                }
            },
            options: {
                name: 'ScrollView',
                page: 0,
                duration: 400,
                velocityThreshold: 0.8,
                contentHeight: 'auto',
                pageSize: 1,
                itemsPerPage: 1,
                bounceVelocityThreshold: 1.6,
                enablePager: true,
                pagerOverlay: false,
                autoBind: true,
                template: '',
                emptyTemplate: ''
            },
            events: [
                CHANGING,
                CHANGE,
                REFRESH
            ],
            destroy: function () {
                Widget.fn.destroy.call(this);
                kendo.destroy(this.element);
            },
            viewInit: function () {
                if (this.options.autoBind) {
                    this._content.scrollTo(this._content.page, true);
                }
            },
            viewShow: function () {
                this.pane.refresh();
            },
            refresh: function () {
                var content = this._content;
                content.resizeTo(this.pane.size());
                this.page = content.page;
                this.trigger(REFRESH, {
                    pageCount: content.pageCount,
                    page: content.page
                });
            },
            content: function (html) {
                this.element.children().first().html(html);
                this._content._getPages();
                this.pane.refresh();
            },
            value: function (item) {
                var dataSource = this.dataSource;
                if (item) {
                    this.scrollTo(dataSource.indexOf(item), true);
                } else {
                    return dataSource.at(this.page);
                }
            },
            scrollTo: function (page, instant) {
                this._content.scrollTo(page, instant);
                this._syncWithContent();
            },
            prev: function () {
                var that = this, prevPage = that.page - 1;
                if (that._content instanceof VirtualScrollViewContent) {
                    that._content.paneMoved(RIGHT_SWIPE, undefined, function (eventData) {
                        return that.trigger(CHANGING, eventData);
                    });
                } else if (prevPage > -1) {
                    that.scrollTo(prevPage);
                }
            },
            next: function () {
                var that = this, nextPage = that.page + 1;
                if (that._content instanceof VirtualScrollViewContent) {
                    that._content.paneMoved(LEFT_SWIPE, undefined, function (eventData) {
                        return that.trigger(CHANGING, eventData);
                    });
                } else if (nextPage < that._content.pageCount) {
                    that.scrollTo(nextPage);
                }
            },
            setDataSource: function (dataSource) {
                if (!(this._content instanceof VirtualScrollViewContent)) {
                    return;
                }
                var emptyDataSource = !dataSource;
                this.dataSource = DataSource.create(dataSource);
                this._content.setDataSource(this.dataSource);
                if (this.options.autoBind && !emptyDataSource) {
                    this.dataSource.fetch();
                }
            },
            items: function () {
                return this.element.find('.km-' + VIRTUAL_PAGE_CLASS);
            },
            _syncWithContent: function () {
                var pages = this._content.pages, buffer = this._content.buffer, data, element;
                this.page = this._content.page;
                data = buffer ? buffer.at(this.page) : undefined;
                if (!(data instanceof Array)) {
                    data = [data];
                }
                element = pages ? pages[1].element : undefined;
                this.trigger(CHANGE, {
                    page: this.page,
                    element: element,
                    data: data
                });
            },
            _dragStart: function () {
                if (this._content.forcePageUpdate()) {
                    this._syncWithContent();
                }
            },
            _dragEnd: function (e) {
                var that = this, velocity = e.x.velocity, velocityThreshold = this.options.velocityThreshold, swipeType = NUDGE, bounce = abs(velocity) > this.options.bounceVelocityThreshold;
                if (velocity > velocityThreshold) {
                    swipeType = RIGHT_SWIPE;
                } else if (velocity < -velocityThreshold) {
                    swipeType = LEFT_SWIPE;
                }
                this._content.paneMoved(swipeType, bounce, function (eventData) {
                    return that.trigger(CHANGING, eventData);
                });
            },
            _transitionEnd: function () {
                if (this._content.updatePage()) {
                    this._syncWithContent();
                }
            }
        });
        ui.plugin(ScrollView);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.switch', [
        'kendo.fx',
        'kendo.userevents'
    ], f);
}(function () {
    var __meta__ = {
        id: 'mobile.switch',
        name: 'Switch',
        category: 'mobile',
        description: 'The mobile Switch widget is used to display two exclusive choices.',
        depends: [
            'fx',
            'userevents'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.mobile.ui, outerWidth = kendo._outerWidth, Widget = ui.Widget, support = kendo.support, CHANGE = 'change', SWITCHON = 'switch-on', SWITCHOFF = 'switch-off', MARGINLEFT = 'margin-left', ACTIVE_STATE = 'state-active', DISABLED_STATE = 'state-disabled', DISABLED = 'disabled', RESOLVEDPREFIX = support.transitions.css === undefined ? '' : support.transitions.css, TRANSFORMSTYLE = RESOLVEDPREFIX + 'transform', proxy = $.proxy;
        function className(name) {
            return 'k-' + name + ' km-' + name;
        }
        function limitValue(value, minLimit, maxLimit) {
            return Math.max(minLimit, Math.min(maxLimit, value));
        }
        var SWITCH_MARKUP = '<span class="' + className('switch') + ' ' + className('widget') + '">        <span class="' + className('switch-wrapper') + '">            <span class="' + className('switch-background') + '"></span>        </span>         <span class="' + className('switch-container') + '">            <span class="' + className('switch-handle') + '">                 <span class="' + className('switch-label-on') + '">{0}</span>                 <span class="' + className('switch-label-off') + '">{1}</span>             </span>         </span>    </span>';
        var Switch = Widget.extend({
            init: function (element, options) {
                var that = this, checked;
                Widget.fn.init.call(that, element, options);
                options = that.options;
                that.wrapper = $(kendo.format(SWITCH_MARKUP, options.onLabel, options.offLabel));
                that.handle = that.wrapper.find('.km-switch-handle');
                that.background = that.wrapper.find('.km-switch-background');
                that.wrapper.insertBefore(that.element).prepend(that.element);
                that._drag();
                that.origin = parseInt(that.background.css(MARGINLEFT), 10);
                that.constrain = 0;
                that.snapPoint = 0;
                element = that.element[0];
                element.type = 'checkbox';
                that._animateBackground = true;
                checked = that.options.checked;
                if (checked === null) {
                    checked = element.checked;
                }
                that.check(checked);
                that.options.enable = that.options.enable && !that.element.attr(DISABLED);
                that.enable(that.options.enable);
                that.refresh();
                kendo.notify(that, kendo.mobile.ui);
            },
            refresh: function () {
                var that = this, handleWidth = outerWidth(that.handle, true);
                that.width = that.wrapper.width();
                that.constrain = that.width - handleWidth;
                that.snapPoint = that.constrain / 2;
                if (typeof that.origin != 'number') {
                    that.origin = parseInt(that.background.css(MARGINLEFT), 10);
                }
                that.background.data('origin', that.origin);
                that.check(that.element[0].checked);
            },
            events: [CHANGE],
            options: {
                name: 'Switch',
                onLabel: 'on',
                offLabel: 'off',
                checked: null,
                enable: true
            },
            check: function (check) {
                var that = this, element = that.element[0];
                if (check === undefined) {
                    return element.checked;
                }
                that._position(check ? that.constrain : 0);
                element.checked = check;
                that.wrapper.toggleClass(className(SWITCHON), check).toggleClass(className(SWITCHOFF), !check);
            },
            value: function () {
                return this.check.apply(this, arguments);
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.userEvents.destroy();
            },
            toggle: function () {
                var that = this;
                that.check(!that.element[0].checked);
            },
            enable: function (enable) {
                var element = this.element, wrapper = this.wrapper;
                if (typeof enable == 'undefined') {
                    enable = true;
                }
                this.options.enable = enable;
                if (enable) {
                    element.removeAttr(DISABLED);
                } else {
                    element.attr(DISABLED, DISABLED);
                }
                wrapper.toggleClass(className(DISABLED_STATE), !enable);
            },
            _resize: function () {
                this.refresh();
            },
            _move: function (e) {
                var that = this;
                e.preventDefault();
                that._position(limitValue(that.position + e.x.delta, 0, that.width - outerWidth(that.handle, true)));
            },
            _position: function (position) {
                var that = this;
                that.position = position;
                that.handle.css(TRANSFORMSTYLE, 'translatex(' + position + 'px)');
                if (that._animateBackground) {
                    that.background.css(MARGINLEFT, that.origin + position);
                }
            },
            _start: function () {
                if (!this.options.enable) {
                    this.userEvents.cancel();
                } else {
                    this.userEvents.capture();
                    this.handle.addClass(className(ACTIVE_STATE));
                }
            },
            _stop: function () {
                var that = this;
                that.handle.removeClass(className(ACTIVE_STATE));
                that._toggle(that.position > that.snapPoint);
            },
            _toggle: function (checked) {
                var that = this, handle = that.handle, element = that.element[0], value = element.checked, duration = kendo.mobile.application && kendo.mobile.application.os.wp ? 100 : 200, distance;
                that.wrapper.toggleClass(className(SWITCHON), checked).toggleClass(className(SWITCHOFF), !checked);
                that.position = distance = checked * that.constrain;
                if (that._animateBackground) {
                    that.background.kendoStop(true, true).kendoAnimate({
                        effects: 'slideMargin',
                        offset: distance,
                        reset: true,
                        reverse: !checked,
                        axis: 'left',
                        duration: duration
                    });
                }
                handle.kendoStop(true, true).kendoAnimate({
                    effects: 'slideTo',
                    duration: duration,
                    offset: distance + 'px,0',
                    reset: true,
                    complete: function () {
                        if (value !== checked) {
                            element.checked = checked;
                            that.trigger(CHANGE, { checked: checked });
                        }
                    }
                });
            },
            _drag: function () {
                var that = this;
                that.userEvents = new kendo.UserEvents(that.wrapper, {
                    fastTap: true,
                    tap: function () {
                        if (that.options.enable) {
                            that._toggle(!that.element[0].checked);
                        }
                    },
                    start: proxy(that._start, that),
                    move: proxy(that._move, that),
                    end: proxy(that._stop, that)
                });
            }
        });
        ui.plugin(Switch);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile.tabstrip', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'mobile.tabstrip',
        name: 'TabStrip',
        category: 'mobile',
        description: 'The mobile TabStrip widget is used inside a mobile view or layout footer element to display an application-wide group of navigation buttons.',
        depends: ['core']
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.mobile.ui, Widget = ui.Widget, ACTIVE_STATE_CLASS = 'km-state-active', SELECT = 'select';
        function createBadge(value) {
            return $('<span class="km-badge">' + value + '</span>');
        }
        var TabStrip = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                that.container().bind('show', $.proxy(this, 'refresh'));
                that.element.addClass('km-tabstrip').find('a').each(that._buildButton).eq(that.options.selectedIndex).addClass(ACTIVE_STATE_CLASS);
                that.element.on('down', 'a', '_release');
            },
            events: [SELECT],
            switchTo: function (url) {
                var tabs = this.element.find('a'), tab, path, idx = 0, length = tabs.length;
                if (isNaN(url)) {
                    for (; idx < length; idx++) {
                        tab = tabs[idx];
                        path = tab.href.replace(/(\#.+)(\?.+)$/, '$1');
                        if (path.indexOf(url, path.length - url.length) !== -1) {
                            this._setActiveItem($(tab));
                            return true;
                        }
                    }
                } else {
                    this._setActiveItem(tabs.eq(url));
                    return true;
                }
                return false;
            },
            switchByFullUrl: function (url) {
                var tab;
                tab = this.element.find('a[href$=\'' + url + '\']');
                this._setActiveItem(tab);
            },
            clear: function () {
                this.currentItem().removeClass(ACTIVE_STATE_CLASS);
            },
            currentItem: function () {
                return this.element.children('.' + ACTIVE_STATE_CLASS);
            },
            badge: function (item, value) {
                var tabstrip = this.element, badge;
                if (!isNaN(item)) {
                    item = tabstrip.children().get(item);
                }
                item = tabstrip.find(item);
                badge = $(item.find('.km-badge')[0] || createBadge(value).insertAfter(item.children('.km-icon')));
                if (value || value === 0) {
                    badge.html(value);
                    return this;
                }
                if (value === false) {
                    badge.empty().remove();
                    return this;
                }
                return badge.html();
            },
            _release: function (e) {
                if (e.which > 1) {
                    return;
                }
                var that = this, item = $(e.currentTarget);
                if (item[0] === that.currentItem()[0]) {
                    return;
                }
                if (that.trigger(SELECT, { item: item })) {
                    e.preventDefault();
                } else {
                    that._setActiveItem(item);
                }
            },
            _setActiveItem: function (item) {
                if (!item[0]) {
                    return;
                }
                this.clear();
                item.addClass(ACTIVE_STATE_CLASS);
            },
            _buildButton: function () {
                var button = $(this), icon = kendo.attrValue(button, 'icon'), badge = kendo.attrValue(button, 'badge'), image = button.find('img'), iconSpan = $('<span class="km-icon"/>');
                button.addClass('km-button').attr(kendo.attr('role'), 'tab').contents().not(image).wrapAll('<span class="km-text"/>');
                if (image[0]) {
                    image.addClass('km-image').prependTo(button);
                } else {
                    button.prepend(iconSpan);
                    if (icon) {
                        iconSpan.addClass('km-' + icon);
                        if (badge || badge === 0) {
                            createBadge(badge).insertAfter(iconSpan);
                        }
                    }
                }
            },
            refresh: function (e) {
                var url = e.view.id;
                if (url && !this.switchTo(e.view.id)) {
                    this.switchTo(url);
                }
            },
            options: {
                name: 'TabStrip',
                selectedIndex: 0,
                enable: true
            }
        });
        ui.plugin(TabStrip);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.angular', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'angular',
        name: 'AngularJS Directives',
        category: 'framework',
        description: 'Adds Kendo UI for AngularJS directives',
        depends: ['core'],
        defer: true
    };
    (function ($, angular, undefined) {
        'use strict';
        if (!angular || !angular.injector) {
            return;
        }
        var module = angular.module('kendo.directives', []), $injector = angular.injector(['ng']), $parse = $injector.get('$parse'), $timeout = $injector.get('$timeout'), $defaultCompile, $log = $injector.get('$log');
        function withoutTimeout(f) {
            var save = $timeout;
            try {
                $timeout = function (f) {
                    return f();
                };
                return f();
            } finally {
                $timeout = save;
            }
        }
        var OPTIONS_NOW;
        var createDataSource = function () {
            var types = {
                TreeList: 'TreeListDataSource',
                TreeView: 'HierarchicalDataSource',
                Scheduler: 'SchedulerDataSource',
                PivotGrid: 'PivotDataSource',
                PivotConfigurator: 'PivotDataSource',
                PanelBar: 'HierarchicalDataSource',
                Menu: '$PLAIN',
                ContextMenu: '$PLAIN'
            };
            var toDataSource = function (dataSource, type) {
                if (type == '$PLAIN') {
                    return dataSource;
                }
                return kendo.data[type].create(dataSource);
            };
            return function (scope, element, role, source) {
                var type = types[role] || 'DataSource';
                var current = scope.$eval(source);
                var ds = toDataSource(current, type);
                scope.$watch(source, function (mew) {
                    var widget = kendoWidgetInstance(element);
                    if (widget && typeof widget.setDataSource == 'function') {
                        if (mew !== current) {
                            var ds = toDataSource(mew, type);
                            widget.setDataSource(ds);
                            current = mew;
                        }
                    }
                });
                return ds;
            };
        }();
        var ignoredAttributes = {
            kDataSource: true,
            kOptions: true,
            kRebind: true,
            kNgModel: true,
            kNgDelay: true
        };
        var ignoredOwnProperties = {
            name: true,
            title: true,
            style: true
        };
        function createWidget(scope, element, attrs, widget, origAttr, controllers) {
            if (!(element instanceof jQuery)) {
                throw new Error('The Kendo UI directives require jQuery to be available before AngularJS. Please include jquery before angular in the document.');
            }
            var kNgDelay = attrs.kNgDelay, delayValue = scope.$eval(kNgDelay);
            controllers = controllers || [];
            var ngModel = controllers[0], ngForm = controllers[1];
            var ctor = $(element)[widget];
            if (!ctor) {
                window.console.error('Could not find: ' + widget);
                return null;
            }
            var parsed = parseOptions(scope, element, attrs, widget, ctor);
            var options = parsed.options;
            if (parsed.unresolved.length) {
                var promises = [];
                for (var i = 0, len = parsed.unresolved.length; i < len; i++) {
                    var unresolved = parsed.unresolved[i];
                    var promise = $.Deferred(function (d) {
                        var unwatch = scope.$watch(unresolved.path, function (newValue) {
                            if (newValue !== undefined) {
                                unwatch();
                                d.resolve();
                            }
                        });
                    }).promise();
                    promises.push(promise);
                }
                $.when.apply(null, promises).then(createIt);
                return;
            }
            if (kNgDelay && !delayValue) {
                var root = scope.$root || scope;
                var register = function () {
                    var unregister = scope.$watch(kNgDelay, function (newValue) {
                        if (newValue !== undefined) {
                            unregister();
                            element.removeAttr(attrs.$attr.kNgDelay);
                            kNgDelay = null;
                            $timeout(createIt);
                        }
                    });
                };
                if (/^\$(digest|apply)$/.test(root.$$phase)) {
                    register();
                } else {
                    scope.$apply(register);
                }
                return;
            } else {
                return createIt();
            }
            function createIt() {
                var originalElement;
                if (attrs.kRebind) {
                    originalElement = $($(element)[0].cloneNode(true));
                }
                options = parseOptions(scope, element, attrs, widget, ctor).options;
                if (element.is('select')) {
                    (function (options) {
                        if (options.length > 0) {
                            var first = $(options[0]);
                            if (!/\S/.test(first.text()) && /^\?/.test(first.val())) {
                                first.remove();
                            }
                            for (var i = 0; i < options.length; i++) {
                                $(options[i]).off('$destroy');
                            }
                        }
                    }(element[0].options));
                }
                var object = ctor.call(element, OPTIONS_NOW = options).data(widget);
                exposeWidget(object, scope, attrs, widget, origAttr);
                scope.$emit('kendoWidgetCreated', object);
                var destroyRegister = destroyWidgetOnScopeDestroy(scope, object);
                if (attrs.kRebind) {
                    setupRebind(object, scope, element, originalElement, attrs.kRebind, destroyRegister, attrs);
                }
                if (attrs.kNgDisabled) {
                    var kNgDisabled = attrs.kNgDisabled;
                    var isDisabled = scope.$eval(kNgDisabled);
                    if (isDisabled) {
                        object.enable(!isDisabled);
                    }
                    bindToKNgDisabled(object, scope, element, kNgDisabled);
                }
                if (attrs.kNgReadonly) {
                    var kNgReadonly = attrs.kNgReadonly;
                    var isReadonly = scope.$eval(kNgReadonly);
                    if (isReadonly) {
                        object.readonly(isReadonly);
                    }
                    bindToKNgReadonly(object, scope, element, kNgReadonly);
                }
                if (attrs.kNgModel) {
                    bindToKNgModel(object, scope, attrs.kNgModel);
                }
                if (ngModel) {
                    bindToNgModel(object, scope, element, ngModel, ngForm);
                }
                if (object) {
                    propagateClassToWidgetWrapper(object, element);
                }
                return object;
            }
        }
        function parseOptions(scope, element, attrs, widget, ctor) {
            var role = widget.replace(/^kendo/, '');
            var unresolved = [];
            var optionsPath = attrs.kOptions || attrs.options;
            var optionsValue = scope.$eval(optionsPath);
            if (optionsPath && optionsValue === undefined) {
                unresolved.push({
                    option: 'options',
                    path: optionsPath
                });
            }
            var options = angular.extend({}, attrs.defaultOptions, optionsValue);
            function addOption(name, value) {
                var scopeValue = angular.copy(scope.$eval(value));
                if (scopeValue === undefined) {
                    unresolved.push({
                        option: name,
                        path: value
                    });
                } else {
                    options[name] = scopeValue;
                }
            }
            var widgetOptions = ctor.widget.prototype.options;
            var widgetEvents = ctor.widget.prototype.events;
            $.each(attrs, function (name, value) {
                if (name === 'source' || name === 'kDataSource' || name === 'kScopeField' || name === 'scopeField') {
                    return;
                }
                var dataName = 'data' + name.charAt(0).toUpperCase() + name.slice(1);
                if (name.indexOf('on') === 0) {
                    var eventKey = name.replace(/^on./, function (prefix) {
                        return prefix.charAt(2).toLowerCase();
                    });
                    if (widgetEvents.indexOf(eventKey) > -1) {
                        options[eventKey] = value;
                    }
                }
                if (widgetOptions.hasOwnProperty(dataName)) {
                    addOption(dataName, value);
                } else if (widgetOptions.hasOwnProperty(name) && !ignoredOwnProperties[name]) {
                    addOption(name, value);
                } else if (!ignoredAttributes[name]) {
                    var match = name.match(/^k(On)?([A-Z].*)/);
                    if (match) {
                        var optionName = match[2].charAt(0).toLowerCase() + match[2].slice(1);
                        if (match[1] && name != 'kOnLabel') {
                            options[optionName] = value;
                        } else {
                            if (name == 'kOnLabel') {
                                optionName = 'onLabel';
                            }
                            addOption(optionName, value);
                        }
                    }
                }
            });
            var dataSource = attrs.kDataSource || attrs.source;
            if (dataSource) {
                options.dataSource = createDataSource(scope, element, role, dataSource);
            }
            options.$angular = [scope];
            return {
                options: options,
                unresolved: unresolved
            };
        }
        function bindToKNgDisabled(widget, scope, element, kNgDisabled) {
            if (kendo.ui.PanelBar && widget instanceof kendo.ui.PanelBar || kendo.ui.Menu && widget instanceof kendo.ui.Menu) {
                $log.warn('k-ng-disabled specified on a widget that does not have the enable() method: ' + widget.options.name);
                return;
            }
            scope.$watch(kNgDisabled, function (newValue, oldValue) {
                if (newValue != oldValue) {
                    widget.enable(!newValue);
                }
            });
        }
        function bindToKNgReadonly(widget, scope, element, kNgReadonly) {
            if (typeof widget.readonly != 'function') {
                $log.warn('k-ng-readonly specified on a widget that does not have the readonly() method: ' + widget.options.name);
                return;
            }
            scope.$watch(kNgReadonly, function (newValue, oldValue) {
                if (newValue != oldValue) {
                    widget.readonly(newValue);
                }
            });
        }
        function exposeWidget(widget, scope, attrs, kendoWidget, origAttr) {
            if (attrs[origAttr]) {
                var set = $parse(attrs[origAttr]).assign;
                if (set) {
                    set(scope, widget);
                } else {
                    throw new Error(origAttr + ' attribute used but expression in it is not assignable: ' + attrs[kendoWidget]);
                }
            }
        }
        function formValue(element) {
            if (/checkbox|radio/i.test(element.attr('type'))) {
                return element.prop('checked');
            }
            return element.val();
        }
        var formRegExp = /^(input|select|textarea)$/i;
        function isForm(element) {
            return formRegExp.test(element[0].tagName);
        }
        function bindToNgModel(widget, scope, element, ngModel, ngForm) {
            if (!widget.value) {
                return;
            }
            var value;
            var haveChangeOnElement = false;
            if (isForm(element)) {
                value = function () {
                    return formValue(element);
                };
            } else {
                value = function () {
                    return widget.value();
                };
            }
            var viewRender = function () {
                var val = ngModel.$viewValue;
                if (val === undefined) {
                    val = ngModel.$modelValue;
                }
                if (val === undefined) {
                    val = null;
                }
                haveChangeOnElement = true;
                setTimeout(function () {
                    haveChangeOnElement = false;
                    if (widget) {
                        var kNgModel = scope[widget.element.attr('k-ng-model')];
                        if (kNgModel) {
                            val = kNgModel;
                        }
                        if (widget.options.autoBind === false && !widget.listView.bound()) {
                            if (val) {
                                widget.value(val);
                            }
                        } else {
                            widget.value(val);
                        }
                    }
                }, 0);
            };
            ngModel.$render = viewRender;
            setTimeout(function () {
                if (ngModel.$render !== viewRender) {
                    ngModel.$render = viewRender;
                    ngModel.$render();
                }
            });
            if (isForm(element)) {
                element.on('change', function () {
                    haveChangeOnElement = true;
                });
            }
            var onChange = function (pristine) {
                return function () {
                    var formPristine;
                    if (haveChangeOnElement && !element.is('select')) {
                        return;
                    }
                    if (pristine && ngForm) {
                        formPristine = ngForm.$pristine;
                    }
                    ngModel.$setViewValue(value());
                    if (pristine) {
                        ngModel.$setPristine();
                        if (formPristine) {
                            ngForm.$setPristine();
                        }
                    }
                    digest(scope);
                };
            };
            widget.first('change', onChange(false));
            widget.first('spin', onChange(false));
            if (!(kendo.ui.AutoComplete && widget instanceof kendo.ui.AutoComplete)) {
                widget.first('dataBound', onChange(true));
            }
            var currentVal = value();
            if (!isNaN(ngModel.$viewValue) && currentVal != ngModel.$viewValue) {
                if (!ngModel.$isEmpty(ngModel.$viewValue)) {
                    widget.value(ngModel.$viewValue);
                } else if (currentVal != null && currentVal !== '' && currentVal != ngModel.$viewValue) {
                    ngModel.$setViewValue(currentVal);
                }
            }
            ngModel.$setPristine();
        }
        function bindToKNgModel(widget, scope, kNgModel) {
            if (typeof widget.value != 'function') {
                $log.warn('k-ng-model specified on a widget that does not have the value() method: ' + widget.options.name);
                return;
            }
            var form = $(widget.element).parents('form');
            var ngForm = kendo.getter(form.attr('name'), true)(scope);
            var getter = $parse(kNgModel);
            var setter = getter.assign;
            var updating = false;
            var valueIsCollection = kendo.ui.MultiSelect && widget instanceof kendo.ui.MultiSelect || kendo.ui.RangeSlider && widget instanceof kendo.ui.RangeSlider;
            var length = function (value) {
                return value && valueIsCollection ? value.length : 0;
            };
            var currentValueLength = length(getter(scope));
            widget.$angular_setLogicValue(getter(scope));
            var watchHandler = function (newValue, oldValue) {
                if (newValue === undefined) {
                    newValue = null;
                }
                if (updating || newValue == oldValue && length(newValue) == currentValueLength) {
                    return;
                }
                currentValueLength = length(newValue);
                widget.$angular_setLogicValue(newValue);
            };
            if (valueIsCollection) {
                scope.$watchCollection(kNgModel, watchHandler);
            } else {
                scope.$watch(kNgModel, watchHandler);
            }
            var changeHandler = function () {
                updating = true;
                if (ngForm && ngForm.$pristine) {
                    ngForm.$setDirty();
                }
                digest(scope, function () {
                    setter(scope, widget.$angular_getLogicValue());
                    currentValueLength = length(getter(scope));
                });
                updating = false;
            };
            widget.first('change', changeHandler);
            widget.first('spin', changeHandler);
        }
        function destroyWidgetOnScopeDestroy(scope, widget) {
            var deregister = scope.$on('$destroy', function () {
                deregister();
                if (widget) {
                    kendo.destroy(widget.element);
                    widget = null;
                }
            });
            return deregister;
        }
        function propagateClassToWidgetWrapper(widget, element) {
            if (!(window.MutationObserver && widget.wrapper)) {
                return;
            }
            var prevClassList = [].slice.call($(element)[0].classList);
            var mo = new MutationObserver(function (changes) {
                suspend();
                if (!widget) {
                    return;
                }
                changes.forEach(function (chg) {
                    var w = $(widget.wrapper)[0];
                    switch (chg.attributeName) {
                    case 'class':
                        var currClassList = [].slice.call(chg.target.classList);
                        currClassList.forEach(function (cls) {
                            if (prevClassList.indexOf(cls) < 0) {
                                w.classList.add(cls);
                                if (kendo.ui.ComboBox && widget instanceof kendo.ui.ComboBox) {
                                    widget.input[0].classList.add(cls);
                                }
                            }
                        });
                        prevClassList.forEach(function (cls) {
                            if (currClassList.indexOf(cls) < 0) {
                                w.classList.remove(cls);
                                if (kendo.ui.ComboBox && widget instanceof kendo.ui.ComboBox) {
                                    widget.input[0].classList.remove(cls);
                                }
                            }
                        });
                        prevClassList = currClassList;
                        break;
                    case 'disabled':
                        if (typeof widget.enable == 'function' && !widget.element.attr('readonly')) {
                            widget.enable(!$(chg.target).attr('disabled'));
                        }
                        break;
                    case 'readonly':
                        if (typeof widget.readonly == 'function' && !widget.element.attr('disabled')) {
                            widget.readonly(!!$(chg.target).attr('readonly'));
                        }
                        break;
                    }
                });
                resume();
            });
            function suspend() {
                mo.disconnect();
            }
            function resume() {
                mo.observe($(element)[0], { attributes: true });
            }
            resume();
            widget.first('destroy', suspend);
        }
        function setupRebind(widget, scope, element, originalElement, rebindAttr, destroyRegister, attrs) {
            var unregister = scope.$watch(rebindAttr, function (newValue, oldValue) {
                if (!widget._muteRebind && newValue !== oldValue) {
                    unregister();
                    if (attrs._cleanUp) {
                        attrs._cleanUp();
                    }
                    var templateOptions = WIDGET_TEMPLATE_OPTIONS[widget.options.name];
                    if (templateOptions) {
                        templateOptions.forEach(function (name) {
                            var templateContents = scope.$eval(attrs['k' + name]);
                            if (templateContents) {
                                originalElement.append($(templateContents).attr(kendo.toHyphens('k' + name), ''));
                            }
                        });
                    }
                    var _wrapper = $(widget.wrapper)[0];
                    var _element = $(widget.element)[0];
                    var isUpload = widget.options.name === 'Upload';
                    if (isUpload) {
                        element = $(_element);
                    }
                    var compile = element.injector().get('$compile');
                    widget._destroy();
                    if (destroyRegister) {
                        destroyRegister();
                    }
                    widget = null;
                    if (_element) {
                        if (_wrapper) {
                            _wrapper.parentNode.replaceChild(_element, _wrapper);
                        }
                        $(element).replaceWith(originalElement);
                    }
                    compile(originalElement)(scope);
                }
            }, true);
            digest(scope);
        }
        function bind(f, obj) {
            return function (a, b) {
                return f.call(obj, a, b);
            };
        }
        function setTemplate(key, value) {
            this[key] = kendo.stringify(value);
        }
        module.factory('directiveFactory', [
            '$compile',
            function (compile) {
                var kendoRenderedTimeout;
                var RENDERED = false;
                $defaultCompile = compile;
                var create = function (role, origAttr) {
                    return {
                        restrict: 'AC',
                        require: [
                            '?ngModel',
                            '^?form'
                        ],
                        scope: false,
                        controller: [
                            '$scope',
                            '$attrs',
                            '$element',
                            function ($scope, $attrs) {
                                this.template = bind(setTemplate, $attrs);
                                $attrs._cleanUp = bind(function () {
                                    this.template = null;
                                    $attrs._cleanUp = null;
                                }, this);
                            }
                        ],
                        link: function (scope, element, attrs, controllers) {
                            var $element = $(element);
                            var roleattr = role.replace(/([A-Z])/g, '-$1');
                            $element.attr(roleattr, $element.attr('data-' + roleattr));
                            $element[0].removeAttribute('data-' + roleattr);
                            var widget = createWidget(scope, element, attrs, role, origAttr, controllers);
                            if (!widget) {
                                return;
                            }
                            if (kendoRenderedTimeout) {
                                clearTimeout(kendoRenderedTimeout);
                            }
                            kendoRenderedTimeout = setTimeout(function () {
                                scope.$emit('kendoRendered');
                                if (!RENDERED) {
                                    RENDERED = true;
                                    $('form').each(function () {
                                        var form = $(this).controller('form');
                                        if (form) {
                                            form.$setPristine();
                                        }
                                    });
                                }
                            });
                        }
                    };
                };
                return { create: create };
            }
        ]);
        var TAGNAMES = {
            Editor: 'textarea',
            NumericTextBox: 'input',
            DatePicker: 'input',
            DateTimePicker: 'input',
            TimePicker: 'input',
            AutoComplete: 'input',
            ColorPicker: 'input',
            MaskedTextBox: 'input',
            MultiSelect: 'input',
            Upload: 'input',
            Validator: 'form',
            Button: 'button',
            MobileButton: 'a',
            MobileBackButton: 'a',
            MobileDetailButton: 'a',
            ListView: 'ul',
            MobileListView: 'ul',
            PanelBar: 'ul',
            TreeView: 'ul',
            Menu: 'ul',
            ContextMenu: 'ul',
            ActionSheet: 'ul'
        };
        var SKIP_SHORTCUTS = [
            'MobileView',
            'MobileDrawer',
            'MobileLayout',
            'MobileSplitView',
            'MobilePane',
            'MobileModalView'
        ];
        var MANUAL_DIRECTIVES = [
            'MobileApplication',
            'MobileView',
            'MobileModalView',
            'MobileLayout',
            'MobileActionSheet',
            'MobileDrawer',
            'MobileSplitView',
            'MobilePane',
            'MobileScrollView',
            'MobilePopOver'
        ];
        angular.forEach([
            'MobileNavBar',
            'MobileButton',
            'MobileBackButton',
            'MobileDetailButton',
            'MobileTabStrip',
            'MobileScrollView',
            'MobileScroller'
        ], function (widget) {
            MANUAL_DIRECTIVES.push(widget);
            widget = 'kendo' + widget;
            module.directive(widget, function () {
                return {
                    restrict: 'A',
                    link: function (scope, element, attrs) {
                        createWidget(scope, element, attrs, widget, widget);
                    }
                };
            });
        });
        function createDirectives(klass, isMobile) {
            function make(directiveName, widgetName) {
                module.directive(directiveName, [
                    'directiveFactory',
                    function (directiveFactory) {
                        return directiveFactory.create(widgetName, directiveName);
                    }
                ]);
            }
            var name = isMobile ? 'Mobile' : '';
            name += klass.fn.options.name;
            var className = name;
            var shortcut = 'kendo' + name.charAt(0) + name.substr(1).toLowerCase();
            name = 'kendo' + name;
            var dashed = name.replace(/([A-Z])/g, '-$1');
            if (SKIP_SHORTCUTS.indexOf(name.replace('kendo', '')) == -1) {
                var names = name === shortcut ? [name] : [
                    name,
                    shortcut
                ];
                angular.forEach(names, function (directiveName) {
                    module.directive(directiveName, function () {
                        return {
                            restrict: 'E',
                            replace: true,
                            template: function (element, attributes) {
                                var tag = TAGNAMES[className] || 'div';
                                var scopeField = attributes.kScopeField || attributes.scopeField;
                                return '<' + tag + ' ' + dashed + (scopeField ? '="' + scopeField + '"' : '') + '>' + element.html() + '</' + tag + '>';
                            }
                        };
                    });
                });
            }
            if (MANUAL_DIRECTIVES.indexOf(name.replace('kendo', '')) > -1) {
                return;
            }
            make(name, name);
            if (shortcut != name) {
                make(shortcut, name);
            }
        }
        function kendoWidgetInstance(el) {
            el = $(el);
            return kendo.widgetInstance(el, kendo.ui) || kendo.widgetInstance(el, kendo.mobile.ui) || kendo.widgetInstance(el, kendo.dataviz.ui);
        }
        function digest(scope, func) {
            var root = scope.$root || scope;
            var isDigesting = /^\$(digest|apply)$/.test(root.$$phase);
            if (func) {
                if (isDigesting) {
                    func();
                } else {
                    root.$apply(func);
                }
            } else if (!isDigesting) {
                root.$digest();
            }
        }
        function destroyScope(scope, el) {
            scope.$destroy();
            if (el) {
                $(el).removeData('$scope').removeData('$$kendoScope').removeData('$isolateScope').removeData('$isolateScopeNoTemplate').removeClass('ng-scope');
            }
        }
        var pendingPatches = [];
        function defadvice(klass, methodName, func) {
            if ($.isArray(klass)) {
                return angular.forEach(klass, function (klass) {
                    defadvice(klass, methodName, func);
                });
            }
            if (typeof klass == 'string') {
                var a = klass.split('.');
                var x = kendo;
                while (x && a.length > 0) {
                    x = x[a.shift()];
                }
                if (!x) {
                    pendingPatches.push([
                        klass,
                        methodName,
                        func
                    ]);
                    return false;
                }
                klass = x.prototype;
            }
            var origMethod = klass[methodName];
            klass[methodName] = function () {
                var self = this, args = arguments;
                return func.apply({
                    self: self,
                    next: function () {
                        return origMethod.apply(self, arguments.length > 0 ? arguments : args);
                    }
                }, args);
            };
            return true;
        }
        kendo.onWidgetRegistered(function (entry) {
            pendingPatches = $.grep(pendingPatches, function (args) {
                return !defadvice.apply(null, args);
            });
            createDirectives(entry.widget, entry.prefix == 'Mobile');
        });
        defadvice([
            'ui.Widget',
            'mobile.ui.Widget'
        ], 'angular', function (cmd, arg) {
            var self = this.self;
            if (cmd == 'init') {
                if (!arg && OPTIONS_NOW) {
                    arg = OPTIONS_NOW;
                }
                OPTIONS_NOW = null;
                if (arg && arg.$angular) {
                    self.$angular_scope = arg.$angular[0];
                    self.$angular_init(self.element, arg);
                }
                return;
            }
            var scope = self.$angular_scope;
            if (scope) {
                withoutTimeout(function () {
                    var x = arg(), elements = x.elements, data = x.data;
                    if (elements.length > 0) {
                        switch (cmd) {
                        case 'cleanup':
                            angular.forEach(elements, function (el) {
                                var itemScope = $(el).data('$$kendoScope');
                                if (itemScope && itemScope !== scope && itemScope.$$kendoScope) {
                                    destroyScope(itemScope, el);
                                }
                            });
                            break;
                        case 'compile':
                            var injector = self.element.injector();
                            var compile = injector ? injector.get('$compile') : $defaultCompile;
                            angular.forEach(elements, function (el, i) {
                                var itemScope;
                                if (x.scopeFrom) {
                                    itemScope = x.scopeFrom;
                                } else {
                                    var vars = data && data[i];
                                    if (vars !== undefined) {
                                        itemScope = $.extend(scope.$new(), vars);
                                        itemScope.$$kendoScope = true;
                                    } else {
                                        itemScope = scope;
                                    }
                                }
                                $(el).data('$$kendoScope', itemScope);
                                compile(el)(itemScope);
                            });
                            digest(scope);
                            break;
                        }
                    }
                });
            }
        });
        defadvice('ui.Widget', '$angular_getLogicValue', function () {
            return this.self.value();
        });
        defadvice('ui.Widget', '$angular_setLogicValue', function (val) {
            this.self.value(val);
        });
        defadvice('ui.Select', '$angular_getLogicValue', function () {
            var item = this.self.dataItem(), valueField = this.self.options.dataValueField;
            if (item) {
                if (this.self.options.valuePrimitive) {
                    if (!!valueField) {
                        return item[valueField];
                    } else {
                        return item;
                    }
                } else {
                    return item.toJSON();
                }
            } else {
                return null;
            }
        });
        defadvice('ui.Select', '$angular_setLogicValue', function (val) {
            var self = this.self;
            var options = self.options;
            var valueField = options.dataValueField;
            var text = options.text || '';
            if (val === undefined) {
                val = '';
            }
            if (valueField && !options.valuePrimitive && val) {
                text = val[options.dataTextField] || '';
                val = val[valueField || options.dataTextField];
            }
            if (self.options.autoBind === false && !self.listView.bound()) {
                if (!text && val && options.valuePrimitive) {
                    self.value(val);
                } else {
                    self._preselect(val, text);
                }
            } else {
                self.value(val);
            }
        });
        defadvice('ui.MultiSelect', '$angular_getLogicValue', function () {
            var value = this.self.dataItems().slice(0);
            var valueField = this.self.options.dataValueField;
            if (valueField && this.self.options.valuePrimitive) {
                value = $.map(value, function (item) {
                    return item[valueField];
                });
            }
            return value;
        });
        defadvice('ui.MultiSelect', '$angular_setLogicValue', function (val) {
            if (val == null) {
                val = [];
            }
            var self = this.self;
            var options = self.options;
            var valueField = options.dataValueField;
            var data = val;
            if (valueField && !options.valuePrimitive) {
                val = $.map(val, function (item) {
                    return item[valueField];
                });
            }
            if (options.autoBind === false && !options.valuePrimitive && !self.listView.bound()) {
                self._preselect(data, val);
            } else {
                self.value(val);
            }
        });
        defadvice('ui.Widget', '$angular_init', function (element, options) {
            var self = this.self;
            if (options && !$.isArray(options)) {
                var scope = self.$angular_scope;
                for (var i = self.events.length; --i >= 0;) {
                    var event = self.events[i];
                    var handler = options[event];
                    if (handler && typeof handler == 'string') {
                        options[event] = self.$angular_makeEventHandler(event, scope, handler);
                    }
                }
            }
        });
        defadvice('ui.Widget', '$angular_makeEventHandler', function (event, scope, handler) {
            handler = $parse(handler);
            return function (e) {
                digest(scope, function () {
                    handler(scope, { kendoEvent: e });
                });
            };
        });
        defadvice([
            'ui.Grid',
            'ui.ListView',
            'ui.TreeView',
            'ui.PanelBar'
        ], '$angular_makeEventHandler', function (event, scope, handler) {
            if (event != 'change') {
                return this.next();
            }
            handler = $parse(handler);
            return function (ev) {
                var widget = ev.sender;
                var options = widget.options;
                var cell, multiple, locals = { kendoEvent: ev }, elems, items, columns, colIdx;
                if (angular.isString(options.selectable)) {
                    cell = options.selectable.indexOf('cell') !== -1;
                    multiple = options.selectable.indexOf('multiple') !== -1;
                }
                if (widget._checkBoxSelection) {
                    multiple = true;
                }
                elems = locals.selected = this.select();
                items = locals.data = [];
                columns = locals.columns = [];
                for (var i = 0; i < elems.length; i++) {
                    var item = cell ? elems[i].parentNode : elems[i];
                    var dataItem = widget.dataItem(item);
                    if (cell) {
                        if (angular.element.inArray(dataItem, items) < 0) {
                            items.push(dataItem);
                        }
                        colIdx = angular.element(elems[i]).index();
                        if (angular.element.inArray(colIdx, columns) < 0) {
                            columns.push(colIdx);
                        }
                    } else {
                        items.push(dataItem);
                    }
                }
                if (!multiple) {
                    locals.dataItem = locals.data = items[0];
                    locals.angularDataItem = kendo.proxyModelSetters(locals.dataItem);
                    locals.selected = elems[0];
                }
                digest(scope, function () {
                    handler(scope, locals);
                });
            };
        });
        defadvice('ui.Grid', '$angular_init', function (element, options) {
            this.next();
            if (options.columns) {
                var settings = $.extend({}, kendo.Template, options.templateSettings);
                angular.forEach(options.columns, function (col) {
                    if (col.field && !col.template && !col.format && !col.values && (col.encoded === undefined || col.encoded)) {
                        col.template = '<span ng-bind=\'' + kendo.expr(col.field, 'dataItem') + '\'>#: ' + kendo.expr(col.field, settings.paramName) + '#</span>';
                    }
                });
            }
        });
        {
            defadvice('mobile.ui.ButtonGroup', 'value', function (mew) {
                var self = this.self;
                if (mew != null) {
                    self.select(self.element.children('li.km-button').eq(mew));
                    self.trigger('change');
                    self.trigger('select', { index: self.selectedIndex });
                }
                return self.selectedIndex;
            });
            defadvice('mobile.ui.ButtonGroup', '_select', function () {
                this.next();
                this.self.trigger('change');
            });
        }
        module.directive('kendoMobileApplication', function () {
            return {
                terminal: true,
                link: function (scope, element, attrs) {
                    createWidget(scope, element, attrs, 'kendoMobileApplication', 'kendoMobileApplication');
                }
            };
        }).directive('kendoMobileView', function () {
            return {
                scope: true,
                link: {
                    pre: function (scope, element, attrs) {
                        attrs.defaultOptions = scope.viewOptions;
                        attrs._instance = createWidget(scope, element, attrs, 'kendoMobileView', 'kendoMobileView');
                    },
                    post: function (scope, element, attrs) {
                        attrs._instance._layout();
                        attrs._instance._scroller();
                    }
                }
            };
        }).directive('kendoMobileDrawer', function () {
            return {
                scope: true,
                link: {
                    pre: function (scope, element, attrs) {
                        attrs.defaultOptions = scope.viewOptions;
                        attrs._instance = createWidget(scope, element, attrs, 'kendoMobileDrawer', 'kendoMobileDrawer');
                    },
                    post: function (scope, element, attrs) {
                        attrs._instance._layout();
                        attrs._instance._scroller();
                    }
                }
            };
        }).directive('kendoMobileModalView', function () {
            return {
                scope: true,
                link: {
                    pre: function (scope, element, attrs) {
                        attrs.defaultOptions = scope.viewOptions;
                        attrs._instance = createWidget(scope, element, attrs, 'kendoMobileModalView', 'kendoMobileModalView');
                    },
                    post: function (scope, element, attrs) {
                        attrs._instance._layout();
                        attrs._instance._scroller();
                    }
                }
            };
        }).directive('kendoMobileSplitView', function () {
            return {
                terminal: true,
                link: {
                    pre: function (scope, element, attrs) {
                        attrs.defaultOptions = scope.viewOptions;
                        attrs._instance = createWidget(scope, element, attrs, 'kendoMobileSplitView', 'kendoMobileSplitView');
                    },
                    post: function (scope, element, attrs) {
                        attrs._instance._layout();
                    }
                }
            };
        }).directive('kendoMobilePane', function () {
            return {
                terminal: true,
                link: {
                    pre: function (scope, element, attrs) {
                        attrs.defaultOptions = scope.viewOptions;
                        createWidget(scope, element, attrs, 'kendoMobilePane', 'kendoMobilePane');
                    }
                }
            };
        }).directive('kendoMobileLayout', function () {
            return {
                link: {
                    pre: function (scope, element, attrs) {
                        createWidget(scope, element, attrs, 'kendoMobileLayout', 'kendoMobileLayout');
                    }
                }
            };
        }).directive('kendoMobileActionSheet', function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    element.find('a[k-action]').each(function () {
                        $(this).attr('data-' + kendo.ns + 'action', $(this).attr('k-action'));
                    });
                    createWidget(scope, element, attrs, 'kendoMobileActionSheet', 'kendoMobileActionSheet');
                }
            };
        }).directive('kendoMobilePopOver', function () {
            return {
                terminal: true,
                link: {
                    pre: function (scope, element, attrs) {
                        attrs.defaultOptions = scope.viewOptions;
                        createWidget(scope, element, attrs, 'kendoMobilePopOver', 'kendoMobilePopOver');
                    }
                }
            };
        }).directive('kendoViewTitle', function () {
            return {
                restrict: 'E',
                replace: true,
                template: function (element) {
                    return '<span data-' + kendo.ns + 'role=\'view-title\'>' + element.html() + '</span>';
                }
            };
        }).directive('kendoMobileHeader', function () {
            return {
                restrict: 'E',
                link: function (scope, element) {
                    element.addClass('km-header').attr('data-role', 'header');
                }
            };
        }).directive('kendoMobileFooter', function () {
            return {
                restrict: 'E',
                link: function (scope, element) {
                    element.addClass('km-footer').attr('data-role', 'footer');
                }
            };
        }).directive('kendoMobileScrollViewPage', function () {
            return {
                restrict: 'E',
                replace: true,
                template: function (element) {
                    return '<div data-' + kendo.ns + 'role=\'page\'>' + element.html() + '</div>';
                }
            };
        });
        angular.forEach([
            'align',
            'icon',
            'rel',
            'transition',
            'actionsheetContext'
        ], function (attr) {
            var kAttr = 'k' + attr.slice(0, 1).toUpperCase() + attr.slice(1);
            module.directive(kAttr, function () {
                return {
                    restrict: 'A',
                    priority: 2,
                    link: function (scope, element, attrs) {
                        element.attr(kendo.attr(kendo.toHyphens(attr)), scope.$eval(attrs[kAttr]));
                    }
                };
            });
        });
        var WIDGET_TEMPLATE_OPTIONS = {
            'TreeMap': ['Template'],
            'MobileListView': [
                'HeaderTemplate',
                'Template'
            ],
            'MobileScrollView': [
                'EmptyTemplate',
                'Template'
            ],
            'Grid': [
                'AltRowTemplate',
                'DetailTemplate',
                'RowTemplate'
            ],
            'ListView': [
                'EditTemplate',
                'Template',
                'AltTemplate'
            ],
            'Pager': [
                'SelectTemplate',
                'LinkTemplate'
            ],
            'PivotGrid': [
                'ColumnHeaderTemplate',
                'DataCellTemplate',
                'RowHeaderTemplate'
            ],
            'Scheduler': [
                'AllDayEventTemplate',
                'DateHeaderTemplate',
                'EventTemplate',
                'MajorTimeHeaderTemplate',
                'MinorTimeHeaderTemplate'
            ],
            'PanelBar': ['Template'],
            'TreeView': ['Template'],
            'Validator': ['ErrorTemplate']
        };
        (function () {
            var templateDirectives = {};
            angular.forEach(WIDGET_TEMPLATE_OPTIONS, function (templates, widget) {
                angular.forEach(templates, function (template) {
                    if (!templateDirectives[template]) {
                        templateDirectives[template] = [];
                    }
                    templateDirectives[template].push('?^^kendo' + widget);
                });
            });
            angular.forEach(templateDirectives, function (parents, directive) {
                var templateName = 'k' + directive;
                var attrName = kendo.toHyphens(templateName);
                module.directive(templateName, function () {
                    return {
                        restrict: 'A',
                        require: parents,
                        terminal: true,
                        compile: function ($element, $attrs) {
                            if ($attrs[templateName] !== '') {
                                return;
                            }
                            $element.removeAttr(attrName);
                            var template = $element[0].outerHTML;
                            return function (scope, element, attrs, controllers) {
                                var controller;
                                while (!controller && controllers.length) {
                                    controller = controllers.shift();
                                }
                                if (!controller) {
                                    $log.warn(attrName + ' without a matching parent widget found. It can be one of the following: ' + parents.join(', '));
                                } else {
                                    controller.template(templateName, template);
                                    element.remove();
                                }
                            };
                        }
                    };
                });
            });
        }());
    }(window.kendo.jQuery, window.angular));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.mobile', [
        'kendo.core',
        'kendo.fx',
        'kendo.data.odata',
        'kendo.data.xml',
        'kendo.data',
        'kendo.data.signalr',
        'kendo.binder',
        'kendo.validator',
        'kendo.router',
        'kendo.view',
        'kendo.userevents',
        'kendo.draganddrop',
        'kendo.popup',
        'kendo.touch',
        'kendo.mobile.popover',
        'kendo.mobile.loader',
        'kendo.mobile.scroller',
        'kendo.mobile.shim',
        'kendo.mobile.view',
        'kendo.mobile.modalview',
        'kendo.mobile.drawer',
        'kendo.mobile.splitview',
        'kendo.mobile.pane',
        'kendo.mobile.application',
        'kendo.mobile.actionsheet',
        'kendo.mobile.button',
        'kendo.mobile.buttongroup',
        'kendo.mobile.collapsible',
        'kendo.mobile.listview',
        'kendo.mobile.navbar',
        'kendo.mobile.scrollview',
        'kendo.mobile.switch',
        'kendo.mobile.tabstrip',
        'kendo.angular'
    ], f);
}(function () {
    'bundle all';
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));