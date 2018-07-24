/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('util/text-metrics', ['kendo.core'], f);
}(function () {
    (function ($) {
        window.kendo.util = window.kendo.util || {};
        var LRUCache = kendo.Class.extend({
            init: function (size) {
                this._size = size;
                this._length = 0;
                this._map = {};
            },
            put: function (key, value) {
                var map = this._map;
                var entry = {
                    key: key,
                    value: value
                };
                map[key] = entry;
                if (!this._head) {
                    this._head = this._tail = entry;
                } else {
                    this._tail.newer = entry;
                    entry.older = this._tail;
                    this._tail = entry;
                }
                if (this._length >= this._size) {
                    map[this._head.key] = null;
                    this._head = this._head.newer;
                    this._head.older = null;
                } else {
                    this._length++;
                }
            },
            get: function (key) {
                var entry = this._map[key];
                if (entry) {
                    if (entry === this._head && entry !== this._tail) {
                        this._head = entry.newer;
                        this._head.older = null;
                    }
                    if (entry !== this._tail) {
                        if (entry.older) {
                            entry.older.newer = entry.newer;
                            entry.newer.older = entry.older;
                        }
                        entry.older = this._tail;
                        entry.newer = null;
                        this._tail.newer = entry;
                        this._tail = entry;
                    }
                    return entry.value;
                }
            }
        });
        var REPLACE_REGEX = /\r?\n|\r|\t/g;
        var SPACE = ' ';
        function normalizeText(text) {
            return String(text).replace(REPLACE_REGEX, SPACE);
        }
        function objectKey(object) {
            var parts = [];
            for (var key in object) {
                parts.push(key + object[key]);
            }
            return parts.sort().join('');
        }
        function hashKey(str) {
            var hash = 2166136261;
            for (var i = 0; i < str.length; ++i) {
                hash += (hash << 1) + (hash << 4) + (hash << 7) + (hash << 8) + (hash << 24);
                hash ^= str.charCodeAt(i);
            }
            return hash >>> 0;
        }
        function zeroSize() {
            return {
                width: 0,
                height: 0,
                baseline: 0
            };
        }
        var DEFAULT_OPTIONS = { baselineMarkerSize: 1 };
        var defaultMeasureBox;
        if (typeof document !== 'undefined') {
            defaultMeasureBox = document.createElement('div');
            defaultMeasureBox.style.cssText = 'position: absolute !important; top: -4000px !important; width: auto !important; height: auto !important;' + 'padding: 0 !important; margin: 0 !important; border: 0 !important;' + 'line-height: normal !important; visibility: hidden !important; white-space: pre!important;';
        }
        var TextMetrics = kendo.Class.extend({
            init: function (options) {
                this._cache = new LRUCache(1000);
                this.options = $.extend({}, DEFAULT_OPTIONS, options);
            },
            measure: function (text, style, options) {
                if (options === void 0) {
                    options = {};
                }
                if (!text) {
                    return zeroSize();
                }
                var styleKey = objectKey(style);
                var cacheKey = hashKey(text + styleKey);
                var cachedResult = this._cache.get(cacheKey);
                if (cachedResult) {
                    return cachedResult;
                }
                var size = zeroSize();
                var measureBox = options.box || defaultMeasureBox;
                var baselineMarker = this._baselineMarker().cloneNode(false);
                for (var key in style) {
                    var value = style[key];
                    if (typeof value !== 'undefined') {
                        measureBox.style[key] = value;
                    }
                }
                var textStr = options.normalizeText !== false ? normalizeText(text) : String(text);
                measureBox.textContent = textStr;
                measureBox.appendChild(baselineMarker);
                document.body.appendChild(measureBox);
                if (textStr.length) {
                    size.width = measureBox.offsetWidth - this.options.baselineMarkerSize;
                    size.height = measureBox.offsetHeight;
                    size.baseline = baselineMarker.offsetTop + this.options.baselineMarkerSize;
                }
                if (size.width > 0 && size.height > 0) {
                    this._cache.put(cacheKey, size);
                }
                measureBox.parentNode.removeChild(measureBox);
                return size;
            },
            _baselineMarker: function () {
                var marker = document.createElement('div');
                marker.style.cssText = 'display: inline-block; vertical-align: baseline;width: ' + this.options.baselineMarkerSize + 'px; height: ' + this.options.baselineMarkerSize + 'px;overflow: hidden;';
                return marker;
            }
        });
        TextMetrics.current = new TextMetrics();
        function measureText(text, style, measureBox) {
            return TextMetrics.current.measure(text, style, measureBox);
        }
        kendo.deepExtend(kendo.util, {
            LRUCache: LRUCache,
            TextMetrics: TextMetrics,
            measureText: measureText,
            objectKey: objectKey,
            hashKey: hashKey,
            normalizeText: normalizeText
        });
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/core/kendo-core', [
        'kendo.core',
        'kendo.drawing'
    ], f);
}(function () {
    (function ($) {
        window.kendo.dataviz = window.kendo.dataviz || {};
        var drawing = kendo.drawing;
        var util = drawing.util;
        var Path = drawing.Path;
        var Group = drawing.Group;
        var Class = kendo.Class;
        var geometry = kendo.geometry;
        var Rect = geometry.Rect;
        var Circle = geometry.Circle;
        var geometryTransform = geometry.transform;
        var Segment = geometry.Segment;
        var dataviz = kendo.dataviz;
        var deepExtend = kendo.deepExtend;
        var isFunction = kendo.isFunction;
        var __common_getter_js = kendo.getter;
        var X = 'x';
        var Y = 'y';
        var TOP = 'top';
        var BOTTOM = 'bottom';
        var LEFT = 'left';
        var RIGHT = 'right';
        var CENTER = 'center';
        var WIDTH = 'width';
        var HEIGHT = 'height';
        var COORD_PRECISION = 3;
        var MAX_VALUE = Number.MAX_VALUE;
        var MIN_VALUE = -Number.MAX_VALUE;
        var DEFAULT_WIDTH = 600;
        var DEFAULT_HEIGHT = 400;
        var WHITE = '#fff';
        var BLACK = '#000';
        var DEFAULT_FONT = '12px sans-serif';
        var DEFAULT_PRECISION = 10;
        var AXIS_LABEL_CLICK = 'axisLabelClick';
        var NOTE_CLICK = 'noteClick';
        var NOTE_HOVER = 'noteHover';
        var OUTSIDE = 'outside';
        var NONE = 'none';
        var CIRCLE = 'circle';
        var TRIANGLE = 'triangle';
        var CROSS = 'cross';
        var ARC = 'arc';
        var INSIDE = 'inside';
        var VALUE = 'value';
        var STRING = 'string';
        var OBJECT = 'object';
        var DATE = 'date';
        var FORMAT_REGEX = /\{\d+:?/;
        var HIGHLIGHT_ZINDEX = 100;
        var constants = {
            X: X,
            Y: Y,
            WIDTH: WIDTH,
            HEIGHT: HEIGHT,
            DEFAULT_HEIGHT: DEFAULT_HEIGHT,
            DEFAULT_WIDTH: DEFAULT_WIDTH,
            TOP: TOP,
            LEFT: LEFT,
            BOTTOM: BOTTOM,
            RIGHT: RIGHT,
            CENTER: CENTER,
            COORD_PRECISION: COORD_PRECISION,
            DEFAULT_PRECISION: DEFAULT_PRECISION,
            CIRCLE: CIRCLE,
            TRIANGLE: TRIANGLE,
            CROSS: CROSS,
            MAX_VALUE: MAX_VALUE,
            MIN_VALUE: MIN_VALUE,
            WHITE: WHITE,
            BLACK: BLACK,
            DEFAULT_FONT: DEFAULT_FONT,
            AXIS_LABEL_CLICK: AXIS_LABEL_CLICK,
            OUTSIDE: OUTSIDE,
            INSIDE: INSIDE,
            NONE: NONE,
            NOTE_CLICK: NOTE_CLICK,
            NOTE_HOVER: NOTE_HOVER,
            VALUE: VALUE,
            STRING: STRING,
            OBJECT: OBJECT,
            DATE: DATE,
            ARC: ARC,
            FORMAT_REGEX: FORMAT_REGEX,
            HIGHLIGHT_ZINDEX: HIGHLIGHT_ZINDEX
        };
        function isArray(value) {
            return Array.isArray(value);
        }
        function addClass(element, classes) {
            var classArray = isArray(classes) ? classes : [classes];
            for (var idx = 0; idx < classArray.length; idx++) {
                var className = classArray[idx];
                if (element.className.indexOf(className) === -1) {
                    element.className += ' ' + className;
                }
            }
        }
        var SPACE_REGEX = /\s+/g;
        function removeClass(element, className) {
            if (element && element.className) {
                element.className = element.className.replace(className, '').replace(SPACE_REGEX, ' ');
            }
        }
        function alignPathToPixel(path) {
            var offset = 0.5;
            if (path.options.stroke && kendo.drawing.util.defined(path.options.stroke.width)) {
                if (path.options.stroke.width % 2 === 0) {
                    offset = 0;
                }
            }
            for (var i = 0; i < path.segments.length; i++) {
                path.segments[i].anchor().round(0).translate(offset, offset);
            }
            return path;
        }
        function clockwise(angle1, angle2) {
            return -angle1.x * angle2.y + angle1.y * angle2.x < 0;
        }
        function isNumber(value) {
            return typeof value === 'number' && !isNaN(value);
        }
        function isString(value) {
            return typeof value === STRING;
        }
        function convertableToNumber(value) {
            return isNumber(value) || isString(value) && isFinite(value);
        }
        function isObject(value) {
            return typeof value === 'object';
        }
        function styleValue(value) {
            if (isNumber(value)) {
                return value + 'px';
            }
            return value;
        }
        var SIZE_STYLES_REGEX = /width|height|top|left|bottom|right/i;
        function isSizeField(field) {
            return SIZE_STYLES_REGEX.test(field);
        }
        function elementStyles(element, styles) {
            var stylesArray = isString(styles) ? [styles] : styles;
            if (isArray(stylesArray)) {
                var result = {};
                var style = window.getComputedStyle(element);
                for (var idx = 0; idx < stylesArray.length; idx++) {
                    var field = stylesArray[idx];
                    result[field] = isSizeField(field) ? parseFloat(style[field]) : style[field];
                }
                return result;
            } else if (isObject(styles)) {
                for (var field$1 in styles) {
                    element.style[field$1] = styleValue(styles[field$1]);
                }
            }
        }
        function getSpacing(value, defaultSpacing) {
            if (defaultSpacing === void 0) {
                defaultSpacing = 0;
            }
            var spacing = {
                top: 0,
                right: 0,
                bottom: 0,
                left: 0
            };
            if (typeof value === 'number') {
                spacing[TOP] = spacing[RIGHT] = spacing[BOTTOM] = spacing[LEFT] = value;
            } else {
                spacing[TOP] = value[TOP] || defaultSpacing;
                spacing[RIGHT] = value[RIGHT] || defaultSpacing;
                spacing[BOTTOM] = value[BOTTOM] || defaultSpacing;
                spacing[LEFT] = value[LEFT] || defaultSpacing;
            }
            return spacing;
        }
        var defaultImplementation = {
            format: function (format, value) {
                return value;
            },
            toString: function (value) {
                return value;
            },
            parseDate: function (value) {
                return new Date(value);
            }
        };
        var current = defaultImplementation;
        var IntlService = Class.extend({});
        IntlService.register = function (userImplementation) {
            current = userImplementation;
        };
        if (Object.defineProperties) {
            Object.defineProperties(IntlService, {
                implementation: {
                    get: function () {
                        return current;
                    }
                }
            });
        }
        var FORMAT_REPLACE_REGEX = /\{(\d+)(:[^\}]+)?\}/g;
        var FormatService = Class.extend({
            init: function (intlService) {
                this._intlService = intlService;
            },
            auto: function (formatString) {
                var values = [], len = arguments.length - 1;
                while (len-- > 0)
                    values[len] = arguments[len + 1];
                var intl = this.intlService;
                if (isString(formatString) && formatString.match(FORMAT_REGEX)) {
                    return intl.format.apply(intl, [formatString].concat(values));
                }
                return intl.toString(values[0], formatString);
            },
            localeAuto: function (formatString, values, locale) {
                var intl = this.intlService;
                var result;
                if (isString(formatString) && formatString.match(FORMAT_REGEX)) {
                    result = formatString.replace(FORMAT_REPLACE_REGEX, function (match, index, placeholderFormat) {
                        var value = values[parseInt(index, 10)];
                        return intl.toString(value, placeholderFormat ? placeholderFormat.substring(1) : '', locale);
                    });
                } else {
                    result = intl.toString(values[0], formatString, locale);
                }
                return result;
            }
        });
        if (Object.defineProperties) {
            Object.defineProperties(FormatService.fn, {
                intlService: {
                    get: function () {
                        return this._intlService || IntlService.implementation;
                    }
                }
            });
        }
        var ChartService = Class.extend({
            init: function (chart, context) {
                if (context === void 0) {
                    context = {};
                }
                this._intlService = context.intlService;
                this.sender = context.sender || chart;
                this.format = new FormatService(context.intlService);
                this.chart = chart;
                this.rtl = context.rtl;
            },
            notify: function (name, args) {
                this.chart.trigger(name, args);
            }
        });
        if (Object.defineProperties) {
            Object.defineProperties(ChartService.fn, {
                intl: {
                    get: function () {
                        return this._intlService || IntlService.implementation;
                    }
                }
            });
        }
        var current$1;
        var DomEventsBuilder = Class.extend({});
        DomEventsBuilder.register = function (userImplementation) {
            current$1 = userImplementation;
        };
        DomEventsBuilder.create = function (element, events) {
            if (current$1) {
                return current$1.create(element, events);
            }
        };
        var current$2 = {
            compile: function (template) {
                return template;
            }
        };
        var TemplateService = Class.extend({});
        TemplateService.register = function (userImplementation) {
            current$2 = userImplementation;
        };
        TemplateService.compile = function (template) {
            return current$2.compile(template);
        };
        var services = {
            ChartService: ChartService,
            DomEventsBuilder: DomEventsBuilder,
            FormatService: FormatService,
            IntlService: IntlService,
            TemplateService: TemplateService
        };
        function getTemplate(options) {
            if (options === void 0) {
                options = {};
            }
            var template;
            if (options.template) {
                options.template = template = TemplateService.compile(options.template);
            } else if (isFunction(options.content)) {
                template = options.content;
            }
            return template;
        }
        function grep(array, callback) {
            var length = array.length;
            var result = [];
            for (var idx = 0; idx < length; idx++) {
                if (callback(array[idx])) {
                    result.push(array[idx]);
                }
            }
            return result;
        }
        function hasClasses(element, classNames) {
            if (element.className) {
                var names = classNames.split(' ');
                for (var idx = 0; idx < names.length; idx++) {
                    if (element.className.indexOf(names[idx]) !== -1) {
                        return true;
                    }
                }
            }
        }
        function inArray(value, array) {
            if (array) {
                return array.indexOf(value) !== -1;
            }
        }
        function interpolateValue(start, end, progress) {
            return kendo.drawing.util.round(start + (end - start) * progress, COORD_PRECISION);
        }
        var TRIGGER = 'trigger';
        var InstanceObserver = Class.extend({
            init: function (observer, handlers) {
                this.observer = observer;
                this.handlerMap = deepExtend({}, this.handlerMap, handlers);
            },
            trigger: function (name, args) {
                var ref = this;
                var observer = ref.observer;
                var handlerMap = ref.handlerMap;
                var isDefaultPrevented;
                if (handlerMap[name]) {
                    isDefaultPrevented = this.callObserver(handlerMap[name], args);
                } else if (observer[TRIGGER]) {
                    isDefaultPrevented = this.callObserver(TRIGGER, name, args);
                }
                return isDefaultPrevented;
            },
            callObserver: function (fnName) {
                var args = [], len = arguments.length - 1;
                while (len-- > 0)
                    args[len] = arguments[len + 1];
                return this.observer[fnName].apply(this.observer, args);
            },
            requiresHandlers: function (names) {
                var this$1 = this;
                if (this.observer.requiresHandlers) {
                    return this.observer.requiresHandlers(names);
                }
                for (var idx = 0; idx < names.length; idx++) {
                    if (this$1.handlerMap[names[idx]]) {
                        return true;
                    }
                }
            }
        });
        function map(array, callback) {
            var length = array.length;
            var result = [];
            for (var idx = 0; idx < length; idx++) {
                var value = callback(array[idx]);
                if (kendo.drawing.util.defined(value)) {
                    result.push(value);
                }
            }
            return result;
        }
        function mousewheelDelta(e) {
            var delta = 0;
            if (e.wheelDelta) {
                delta = -e.wheelDelta / 120;
                delta = delta > 0 ? Math.ceil(delta) : Math.floor(delta);
            }
            if (e.detail) {
                delta = kendo.drawing.util.round(e.detail / 3);
            }
            return delta;
        }
        var ref = kendo.drawing.util;
        var append = ref.append;
        var bindEvents = ref.bindEvents;
        var defined = ref.defined;
        var deg = ref.deg;
        var elementOffset = ref.elementOffset;
        var elementSize = ref.elementSize;
        var eventElement = ref.eventElement;
        var eventCoordinates = ref.eventCoordinates;
        var last = ref.last;
        var limitValue = ref.limitValue;
        var objectKey = ref.objectKey;
        var rad = ref.rad;
        var round = ref.round;
        var unbindEvents = ref.unbindEvents;
        var valueOrDefault = ref.valueOrDefault;
        var FontLoader = Class.extend({});
        FontLoader.fetchFonts = function (options, fonts, state) {
            if (state === void 0) {
                state = { depth: 0 };
            }
            var MAX_DEPTH = 5;
            if (!options || state.depth > MAX_DEPTH || !document.fonts) {
                return;
            }
            Object.keys(options).forEach(function (key) {
                var value = options[key];
                if (key === 'dataSource' || key[0] === '$' || !value) {
                    return;
                }
                if (key === 'font') {
                    fonts.push(value);
                } else if (typeof value === 'object') {
                    state.depth++;
                    FontLoader.fetchFonts(value, fonts, state);
                    state.depth--;
                }
            });
        };
        FontLoader.loadFonts = function (fonts, callback) {
            var promises = [];
            if (fonts.length > 0 && document.fonts) {
                try {
                    promises = fonts.map(function (font) {
                        return document.fonts.load(font);
                    });
                } catch (e) {
                    kendo.logToConsole(e);
                }
                Promise.all(promises).then(callback, callback);
            } else {
                callback();
            }
        };
        FontLoader.preloadFonts = function (options, callback) {
            var fonts = [];
            FontLoader.fetchFonts(options, fonts);
            FontLoader.loadFonts(fonts, callback);
        };
        function setDefaultOptions(type, options) {
            var proto = type.prototype;
            if (proto.options) {
                proto.options = deepExtend({}, proto.options, options);
            } else {
                proto.options = options;
            }
        }
        function sparseArrayLimits(arr) {
            var min = MAX_VALUE;
            var max = MIN_VALUE;
            for (var idx = 0, length = arr.length; idx < length; idx++) {
                var value = arr[idx];
                if (value !== null && isFinite(value)) {
                    min = Math.min(min, value);
                    max = Math.max(max, value);
                }
            }
            return {
                min: min === MAX_VALUE ? undefined : min,
                max: max === MIN_VALUE ? undefined : max
            };
        }
        var Point = Class.extend({
            init: function (x, y) {
                this.x = x || 0;
                this.y = y || 0;
            },
            clone: function () {
                return new Point(this.x, this.y);
            },
            equals: function (point) {
                return point && this.x === point.x && this.y === point.y;
            },
            rotate: function (center, degrees) {
                var theta = rad(degrees);
                var cosT = Math.cos(theta);
                var sinT = Math.sin(theta);
                var cx = center.x;
                var cy = center.y;
                var ref = this;
                var x = ref.x;
                var y = ref.y;
                this.x = round(cx + (x - cx) * cosT + (y - cy) * sinT, COORD_PRECISION);
                this.y = round(cy + (y - cy) * cosT - (x - cx) * sinT, COORD_PRECISION);
                return this;
            },
            multiply: function (a) {
                this.x *= a;
                this.y *= a;
                return this;
            },
            distanceTo: function (point) {
                var dx = this.x - point.x;
                var dy = this.y - point.y;
                return Math.sqrt(dx * dx + dy * dy);
            }
        });
        Point.onCircle = function (center, angle, radius) {
            var radians = rad(angle);
            return new Point(center.x - radius * Math.cos(radians), center.y - radius * Math.sin(radians));
        };
        var Box = Class.extend({
            init: function (x1, y1, x2, y2) {
                this.x1 = x1 || 0;
                this.y1 = y1 || 0;
                this.x2 = x2 || 0;
                this.y2 = y2 || 0;
            },
            equals: function (box) {
                return this.x1 === box.x1 && this.x2 === box.x2 && this.y1 === box.y1 && this.y2 === box.y2;
            },
            width: function () {
                return this.x2 - this.x1;
            },
            height: function () {
                return this.y2 - this.y1;
            },
            translate: function (dx, dy) {
                this.x1 += dx;
                this.x2 += dx;
                this.y1 += dy;
                this.y2 += dy;
                return this;
            },
            move: function (x, y) {
                var height = this.height();
                var width = this.width();
                if (defined(x)) {
                    this.x1 = x;
                    this.x2 = this.x1 + width;
                }
                if (defined(y)) {
                    this.y1 = y;
                    this.y2 = this.y1 + height;
                }
                return this;
            },
            wrap: function (targetBox) {
                this.x1 = Math.min(this.x1, targetBox.x1);
                this.y1 = Math.min(this.y1, targetBox.y1);
                this.x2 = Math.max(this.x2, targetBox.x2);
                this.y2 = Math.max(this.y2, targetBox.y2);
                return this;
            },
            wrapPoint: function (point) {
                var arrayPoint = isArray(point);
                var x = arrayPoint ? point[0] : point.x;
                var y = arrayPoint ? point[1] : point.y;
                this.wrap(new Box(x, y, x, y));
                return this;
            },
            snapTo: function (targetBox, axis) {
                if (axis === X || !axis) {
                    this.x1 = targetBox.x1;
                    this.x2 = targetBox.x2;
                }
                if (axis === Y || !axis) {
                    this.y1 = targetBox.y1;
                    this.y2 = targetBox.y2;
                }
                return this;
            },
            alignTo: function (targetBox, anchor) {
                var height = this.height();
                var width = this.width();
                var axis = anchor === TOP || anchor === BOTTOM ? Y : X;
                var offset = axis === Y ? height : width;
                if (anchor === CENTER) {
                    var targetCenter = targetBox.center();
                    var center = this.center();
                    this.x1 += targetCenter.x - center.x;
                    this.y1 += targetCenter.y - center.y;
                } else if (anchor === TOP || anchor === LEFT) {
                    this[axis + 1] = targetBox[axis + 1] - offset;
                } else {
                    this[axis + 1] = targetBox[axis + 2];
                }
                this.x2 = this.x1 + width;
                this.y2 = this.y1 + height;
                return this;
            },
            shrink: function (dw, dh) {
                this.x2 -= dw;
                this.y2 -= dh;
                return this;
            },
            expand: function (dw, dh) {
                this.shrink(-dw, -dh);
                return this;
            },
            pad: function (padding) {
                var spacing = getSpacing(padding);
                this.x1 -= spacing.left;
                this.x2 += spacing.right;
                this.y1 -= spacing.top;
                this.y2 += spacing.bottom;
                return this;
            },
            unpad: function (padding) {
                var spacing = getSpacing(padding);
                spacing.left = -spacing.left;
                spacing.top = -spacing.top;
                spacing.right = -spacing.right;
                spacing.bottom = -spacing.bottom;
                return this.pad(spacing);
            },
            clone: function () {
                return new Box(this.x1, this.y1, this.x2, this.y2);
            },
            center: function () {
                return new Point(this.x1 + this.width() / 2, this.y1 + this.height() / 2);
            },
            containsPoint: function (point) {
                return point.x >= this.x1 && point.x <= this.x2 && point.y >= this.y1 && point.y <= this.y2;
            },
            points: function () {
                return [
                    new Point(this.x1, this.y1),
                    new Point(this.x2, this.y1),
                    new Point(this.x2, this.y2),
                    new Point(this.x1, this.y2)
                ];
            },
            getHash: function () {
                return [
                    this.x1,
                    this.y1,
                    this.x2,
                    this.y2
                ].join(',');
            },
            overlaps: function (box) {
                return !(box.y2 < this.y1 || this.y2 < box.y1 || box.x2 < this.x1 || this.x2 < box.x1);
            },
            rotate: function (rotation) {
                var width = this.width();
                var height = this.height();
                var ref = this.center();
                var cx = ref.x;
                var cy = ref.y;
                var r1 = rotatePoint(0, 0, cx, cy, rotation);
                var r2 = rotatePoint(width, 0, cx, cy, rotation);
                var r3 = rotatePoint(width, height, cx, cy, rotation);
                var r4 = rotatePoint(0, height, cx, cy, rotation);
                width = Math.max(r1.x, r2.x, r3.x, r4.x) - Math.min(r1.x, r2.x, r3.x, r4.x);
                height = Math.max(r1.y, r2.y, r3.y, r4.y) - Math.min(r1.y, r2.y, r3.y, r4.y);
                this.x2 = this.x1 + width;
                this.y2 = this.y1 + height;
                return this;
            },
            toRect: function () {
                return new Rect([
                    this.x1,
                    this.y1
                ], [
                    this.width(),
                    this.height()
                ]);
            },
            hasSize: function () {
                return this.width() !== 0 && this.height() !== 0;
            },
            align: function (targetBox, axis, alignment) {
                var c1 = axis + 1;
                var c2 = axis + 2;
                var sizeFunc = axis === X ? WIDTH : HEIGHT;
                var size = this[sizeFunc]();
                if (inArray(alignment, [
                        LEFT,
                        TOP
                    ])) {
                    this[c1] = targetBox[c1];
                    this[c2] = this[c1] + size;
                } else if (inArray(alignment, [
                        RIGHT,
                        BOTTOM
                    ])) {
                    this[c2] = targetBox[c2];
                    this[c1] = this[c2] - size;
                } else if (alignment === CENTER) {
                    this[c1] = targetBox[c1] + (targetBox[sizeFunc]() - size) / 2;
                    this[c2] = this[c1] + size;
                }
            }
        });
        function rotatePoint(x, y, cx, cy, angle) {
            var theta = rad(angle);
            return new Point(cx + (x - cx) * Math.cos(theta) + (y - cy) * Math.sin(theta), cy - (x - cx) * Math.sin(theta) + (y - cy) * Math.cos(theta));
        }
        var Ring = Class.extend({
            init: function (center, innerRadius, radius, startAngle, angle) {
                this.center = center;
                this.innerRadius = innerRadius;
                this.radius = radius;
                this.startAngle = startAngle;
                this.angle = angle;
            },
            clone: function () {
                return new Ring(this.center, this.innerRadius, this.radius, this.startAngle, this.angle);
            },
            middle: function () {
                return this.startAngle + this.angle / 2;
            },
            setRadius: function (newRadius, innerRadius) {
                if (innerRadius) {
                    this.innerRadius = newRadius;
                } else {
                    this.radius = newRadius;
                }
                return this;
            },
            point: function (angle, innerRadius) {
                var radianAngle = rad(angle);
                var ax = Math.cos(radianAngle);
                var ay = Math.sin(radianAngle);
                var radius = innerRadius ? this.innerRadius : this.radius;
                var x = round(this.center.x - ax * radius, COORD_PRECISION);
                var y = round(this.center.y - ay * radius, COORD_PRECISION);
                return new Point(x, y);
            },
            adjacentBox: function (distance, width, height) {
                var sector = this.clone().expand(distance);
                var midAndle = sector.middle();
                var midPoint = sector.point(midAndle);
                var hw = width / 2;
                var hh = height / 2;
                var sa = Math.sin(rad(midAndle));
                var ca = Math.cos(rad(midAndle));
                var x = midPoint.x - hw;
                var y = midPoint.y - hh;
                if (Math.abs(sa) < 0.9) {
                    x += hw * -ca / Math.abs(ca);
                }
                if (Math.abs(ca) < 0.9) {
                    y += hh * -sa / Math.abs(sa);
                }
                return new Box(x, y, x + width, y + height);
            },
            containsPoint: function (p) {
                var center = this.center;
                var innerRadius = this.innerRadius;
                var radius = this.radius;
                var startAngle = this.startAngle;
                var endAngle = this.startAngle + this.angle;
                var dx = p.x - center.x;
                var dy = p.y - center.y;
                var vector = new Point(dx, dy);
                var startPoint = this.point(startAngle);
                var startVector = new Point(startPoint.x - center.x, startPoint.y - center.y);
                var endPoint = this.point(endAngle);
                var endVector = new Point(endPoint.x - center.x, endPoint.y - center.y);
                var dist = round(dx * dx + dy * dy, COORD_PRECISION);
                return (startVector.equals(vector) || clockwise(startVector, vector)) && !clockwise(endVector, vector) && dist >= innerRadius * innerRadius && dist <= radius * radius;
            },
            getBBox: function () {
                var this$1 = this;
                var box = new Box(MAX_VALUE, MAX_VALUE, MIN_VALUE, MIN_VALUE);
                var startAngle = round(this.startAngle % 360);
                var endAngle = round((startAngle + this.angle) % 360);
                var innerRadius = this.innerRadius;
                var allAngles = [
                    0,
                    90,
                    180,
                    270,
                    startAngle,
                    endAngle
                ].sort(numericComparer);
                var startAngleIndex = allAngles.indexOf(startAngle);
                var endAngleIndex = allAngles.indexOf(endAngle);
                var angles;
                if (startAngle === endAngle) {
                    angles = allAngles;
                } else {
                    if (startAngleIndex < endAngleIndex) {
                        angles = allAngles.slice(startAngleIndex, endAngleIndex + 1);
                    } else {
                        angles = [].concat(allAngles.slice(0, endAngleIndex + 1), allAngles.slice(startAngleIndex, allAngles.length));
                    }
                }
                for (var i = 0; i < angles.length; i++) {
                    var point = this$1.point(angles[i]);
                    box.wrapPoint(point);
                    box.wrapPoint(point, innerRadius);
                }
                if (!innerRadius) {
                    box.wrapPoint(this.center);
                }
                return box;
            },
            expand: function (value) {
                this.radius += value;
                return this;
            }
        });
        function numericComparer(a, b) {
            return a - b;
        }
        var Sector = Ring.extend({
            init: function (center, radius, startAngle, angle) {
                Ring.fn.init.call(this, center, 0, radius, startAngle, angle);
            },
            expand: function (value) {
                return Ring.fn.expand.call(this, value);
            },
            clone: function () {
                return new Sector(this.center, this.radius, this.startAngle, this.angle);
            },
            setRadius: function (newRadius) {
                this.radius = newRadius;
                return this;
            }
        });
        var DIRECTION_ANGLE = 0.001;
        var ShapeBuilder = Class.extend({
            createRing: function (sector, options) {
                var startAngle = sector.startAngle + 180;
                var endAngle = sector.angle + startAngle;
                if (sector.angle > 0 && startAngle === endAngle) {
                    endAngle += DIRECTION_ANGLE;
                }
                var center = new geometry.Point(sector.center.x, sector.center.y);
                var radius = Math.max(sector.radius, 0);
                var innerRadius = Math.max(sector.innerRadius, 0);
                var arc = new geometry.Arc(center, {
                    startAngle: startAngle,
                    endAngle: endAngle,
                    radiusX: radius,
                    radiusY: radius
                });
                var path = Path.fromArc(arc, options).close();
                if (innerRadius) {
                    arc.radiusX = arc.radiusY = innerRadius;
                    var innerEnd = arc.pointAt(endAngle);
                    path.lineTo(innerEnd.x, innerEnd.y);
                    path.arc(endAngle, startAngle, innerRadius, innerRadius, true);
                } else {
                    path.lineTo(center.x, center.y);
                }
                return path;
            }
        });
        ShapeBuilder.current = new ShapeBuilder();
        var ChartElement = Class.extend({
            init: function (options) {
                this.children = [];
                this.options = deepExtend({}, this.options, options);
            },
            reflow: function (targetBox) {
                var children = this.children;
                var box;
                for (var i = 0; i < children.length; i++) {
                    var currentChild = children[i];
                    currentChild.reflow(targetBox);
                    box = box ? box.wrap(currentChild.box) : currentChild.box.clone();
                }
                this.box = box || targetBox;
            },
            destroy: function () {
                var children = this.children;
                if (this.animation) {
                    this.animation.destroy();
                }
                for (var i = 0; i < children.length; i++) {
                    children[i].destroy();
                }
            },
            getRoot: function () {
                var parent = this.parent;
                return parent ? parent.getRoot() : null;
            },
            getSender: function () {
                var service = this.getService();
                if (service) {
                    return service.sender;
                }
            },
            getService: function () {
                var element = this;
                while (element) {
                    if (element.chartService) {
                        return element.chartService;
                    }
                    element = element.parent;
                }
            },
            translateChildren: function (dx, dy) {
                var children = this.children;
                var childrenCount = children.length;
                for (var i = 0; i < childrenCount; i++) {
                    children[i].box.translate(dx, dy);
                }
            },
            append: function () {
                var arguments$1 = arguments;
                var this$1 = this;
                for (var i = 0; i < arguments.length; i++) {
                    var item = arguments$1[i];
                    this$1.children.push(item);
                    item.parent = this$1;
                }
            },
            renderVisual: function () {
                if (this.options.visible === false) {
                    return;
                }
                this.createVisual();
                this.addVisual();
                this.renderChildren();
                this.createAnimation();
                this.renderComplete();
            },
            addVisual: function () {
                if (this.visual) {
                    this.visual.chartElement = this;
                    if (this.parent) {
                        this.parent.appendVisual(this.visual);
                    }
                }
            },
            renderChildren: function () {
                var children = this.children;
                var length = children.length;
                for (var i = 0; i < length; i++) {
                    children[i].renderVisual();
                }
            },
            createVisual: function () {
                this.visual = new Group({
                    zIndex: this.options.zIndex,
                    visible: valueOrDefault(this.options.visible, true)
                });
            },
            createAnimation: function () {
                if (this.visual) {
                    this.animation = drawing.Animation.create(this.visual, this.options.animation);
                }
            },
            appendVisual: function (childVisual) {
                if (!childVisual.chartElement) {
                    childVisual.chartElement = this;
                }
                if (childVisual.options.noclip) {
                    this.clipRoot().visual.append(childVisual);
                } else if (defined(childVisual.options.zIndex)) {
                    this.stackRoot().stackVisual(childVisual);
                } else if (this.isStackRoot) {
                    this.stackVisual(childVisual);
                } else if (this.visual) {
                    this.visual.append(childVisual);
                } else {
                    this.parent.appendVisual(childVisual);
                }
            },
            clipRoot: function () {
                if (this.parent) {
                    return this.parent.clipRoot();
                }
                return this;
            },
            stackRoot: function () {
                if (this.parent) {
                    return this.parent.stackRoot();
                }
                return this;
            },
            stackVisual: function (childVisual) {
                var zIndex = childVisual.options.zIndex || 0;
                var visuals = this.visual.children;
                var length = visuals.length;
                var pos;
                for (pos = 0; pos < length; pos++) {
                    var sibling = visuals[pos];
                    var here = valueOrDefault(sibling.options.zIndex, 0);
                    if (here > zIndex) {
                        break;
                    }
                }
                this.visual.insert(pos, childVisual);
            },
            traverse: function (callback) {
                var children = this.children;
                var length = children.length;
                for (var i = 0; i < length; i++) {
                    var child = children[i];
                    callback(child);
                    if (child.traverse) {
                        child.traverse(callback);
                    }
                }
            },
            closest: function (match) {
                var element = this;
                var matched = false;
                while (element && !matched) {
                    matched = match(element);
                    if (!matched) {
                        element = element.parent;
                    }
                }
                if (matched) {
                    return element;
                }
            },
            renderComplete: function () {
            },
            hasHighlight: function () {
                var options = (this.options || {}).highlight;
                return !(!this.createHighlight || options && options.visible === false);
            },
            toggleHighlight: function (show) {
                var this$1 = this;
                var options = (this.options || {}).highlight || {};
                var customVisual = options.visual;
                var highlight = this._highlight;
                if (!highlight) {
                    var highlightOptions = {
                        fill: {
                            color: WHITE,
                            opacity: 0.2
                        },
                        stroke: {
                            color: WHITE,
                            width: 1,
                            opacity: 0.2
                        }
                    };
                    if (customVisual) {
                        highlight = this._highlight = customVisual($.extend(this.highlightVisualArgs(), {
                            createVisual: function () {
                                return this$1.createHighlight(highlightOptions);
                            },
                            sender: this.getSender(),
                            series: this.series,
                            dataItem: this.dataItem,
                            category: this.category,
                            value: this.value,
                            percentage: this.percentage,
                            runningTotal: this.runningTotal,
                            total: this.total
                        }));
                        if (!highlight) {
                            return;
                        }
                    } else {
                        highlight = this._highlight = this.createHighlight(highlightOptions);
                    }
                    if (!defined(highlight.options.zIndex)) {
                        highlight.options.zIndex = valueOrDefault(options.zIndex, this.options.zIndex);
                    }
                    this.appendVisual(highlight);
                }
                highlight.visible(show);
            },
            createGradientOverlay: function (element, options, gradientOptions) {
                var overlay = new Path($.extend({
                    stroke: { color: 'none' },
                    fill: this.createGradient(gradientOptions),
                    closed: element.options.closed
                }, options));
                overlay.segments.elements(element.segments.elements());
                return overlay;
            },
            createGradient: function (options) {
                if (this.parent) {
                    return this.parent.createGradient(options);
                }
            }
        });
        ChartElement.prototype.options = {};
        var BoxElement = ChartElement.extend({
            init: function (options) {
                ChartElement.fn.init.call(this, options);
                this.options.margin = getSpacing(this.options.margin);
                this.options.padding = getSpacing(this.options.padding);
            },
            reflow: function (targetBox) {
                var this$1 = this;
                var options = this.options;
                var width = options.width;
                var height = options.height;
                var shrinkToFit = options.shrinkToFit;
                var hasSetSize = width && height;
                var margin = options.margin;
                var padding = options.padding;
                var borderWidth = options.border.width;
                var box;
                var reflowPaddingBox = function () {
                    this$1.align(targetBox, X, options.align);
                    this$1.align(targetBox, Y, options.vAlign);
                    this$1.paddingBox = box.clone().unpad(margin).unpad(borderWidth);
                };
                var contentBox = targetBox.clone();
                if (hasSetSize) {
                    contentBox.x2 = contentBox.x1 + width;
                    contentBox.y2 = contentBox.y1 + height;
                }
                if (shrinkToFit) {
                    contentBox.unpad(margin).unpad(borderWidth).unpad(padding);
                }
                ChartElement.fn.reflow.call(this, contentBox);
                if (hasSetSize) {
                    box = this.box = new Box(0, 0, width, height);
                } else {
                    box = this.box;
                }
                if (shrinkToFit && hasSetSize) {
                    reflowPaddingBox();
                    contentBox = this.contentBox = this.paddingBox.clone().unpad(padding);
                } else {
                    contentBox = this.contentBox = box.clone();
                    box.pad(padding).pad(borderWidth).pad(margin);
                    reflowPaddingBox();
                }
                this.translateChildren(box.x1 - contentBox.x1 + margin.left + borderWidth + padding.left, box.y1 - contentBox.y1 + margin.top + borderWidth + padding.top);
                var children = this.children;
                for (var i = 0; i < children.length; i++) {
                    var item = children[i];
                    item.reflow(item.box);
                }
            },
            align: function (targetBox, axis, alignment) {
                this.box.align(targetBox, axis, alignment);
            },
            hasBox: function () {
                var options = this.options;
                return options.border.width || options.background;
            },
            createVisual: function () {
                ChartElement.fn.createVisual.call(this);
                var options = this.options;
                if (options.visible && this.hasBox()) {
                    this.visual.append(Path.fromRect(this.paddingBox.toRect(), this.visualStyle()));
                }
            },
            visualStyle: function () {
                var options = this.options;
                var border = options.border || {};
                return {
                    stroke: {
                        width: border.width,
                        color: border.color,
                        opacity: valueOrDefault(border.opacity, options.opacity),
                        dashType: border.dashType
                    },
                    fill: {
                        color: options.background,
                        opacity: options.opacity
                    },
                    cursor: options.cursor
                };
            }
        });
        setDefaultOptions(BoxElement, {
            align: LEFT,
            vAlign: TOP,
            margin: {},
            padding: {},
            border: {
                color: BLACK,
                width: 0
            },
            background: '',
            shrinkToFit: false,
            width: 0,
            height: 0,
            visible: true
        });
        var ShapeElement = BoxElement.extend({
            init: function (options, pointData) {
                BoxElement.fn.init.call(this, options);
                this.pointData = pointData;
            },
            getElement: function () {
                var ref = this;
                var options = ref.options;
                var box = ref.paddingBox;
                var type = options.type;
                var rotation = options.rotation;
                var center = box.center();
                var halfWidth = box.width() / 2;
                if (!options.visible || !this.hasBox()) {
                    return null;
                }
                var style = this.visualStyle();
                var element;
                if (type === CIRCLE) {
                    element = new drawing.Circle(new Circle([
                        round(box.x1 + halfWidth, COORD_PRECISION),
                        round(box.y1 + box.height() / 2, COORD_PRECISION)
                    ], halfWidth), style);
                } else if (type === TRIANGLE) {
                    element = Path.fromPoints([
                        [
                            box.x1 + halfWidth,
                            box.y1
                        ],
                        [
                            box.x1,
                            box.y2
                        ],
                        [
                            box.x2,
                            box.y2
                        ]
                    ], style).close();
                } else if (type === CROSS) {
                    element = new drawing.MultiPath(style);
                    element.moveTo(box.x1, box.y1).lineTo(box.x2, box.y2);
                    element.moveTo(box.x1, box.y2).lineTo(box.x2, box.y1);
                } else {
                    element = Path.fromRect(box.toRect(), style);
                }
                if (rotation) {
                    element.transform(geometryTransform().rotate(-rotation, [
                        center.x,
                        center.y
                    ]));
                }
                element.options.zIndex = options.zIndex;
                return element;
            },
            createElement: function () {
                var this$1 = this;
                var customVisual = this.options.visual;
                var pointData = this.pointData || {};
                var visual;
                if (customVisual) {
                    visual = customVisual({
                        value: pointData.value,
                        dataItem: pointData.dataItem,
                        sender: this.getSender(),
                        series: pointData.series,
                        category: pointData.category,
                        rect: this.paddingBox.toRect(),
                        options: this.visualOptions(),
                        createVisual: function () {
                            return this$1.getElement();
                        }
                    });
                } else {
                    visual = this.getElement();
                }
                return visual;
            },
            visualOptions: function () {
                var options = this.options;
                return {
                    background: options.background,
                    border: options.border,
                    margin: options.margin,
                    padding: options.padding,
                    type: options.type,
                    size: options.width,
                    visible: options.visible
                };
            },
            createVisual: function () {
                this.visual = this.createElement();
            }
        });
        setDefaultOptions(ShapeElement, {
            type: CIRCLE,
            align: CENTER,
            vAlign: CENTER
        });
        var LINEAR = 'linear';
        var RADIAL = 'radial';
        var GRADIENTS = {
            glass: {
                type: LINEAR,
                rotation: 0,
                stops: [
                    {
                        offset: 0,
                        color: WHITE,
                        opacity: 0
                    },
                    {
                        offset: 0.25,
                        color: WHITE,
                        opacity: 0.3
                    },
                    {
                        offset: 1,
                        color: WHITE,
                        opacity: 0
                    }
                ]
            },
            sharpBevel: {
                type: RADIAL,
                stops: [
                    {
                        offset: 0,
                        color: WHITE,
                        opacity: 0.55
                    },
                    {
                        offset: 0.65,
                        color: WHITE,
                        opacity: 0
                    },
                    {
                        offset: 0.95,
                        color: WHITE,
                        opacity: 0.25
                    }
                ]
            },
            roundedBevel: {
                type: RADIAL,
                stops: [
                    {
                        offset: 0.33,
                        color: WHITE,
                        opacity: 0.06
                    },
                    {
                        offset: 0.83,
                        color: WHITE,
                        opacity: 0.2
                    },
                    {
                        offset: 0.95,
                        color: WHITE,
                        opacity: 0
                    }
                ]
            },
            roundedGlass: {
                type: RADIAL,
                supportVML: false,
                stops: [
                    {
                        offset: 0,
                        color: WHITE,
                        opacity: 0
                    },
                    {
                        offset: 0.5,
                        color: WHITE,
                        opacity: 0.3
                    },
                    {
                        offset: 0.99,
                        color: WHITE,
                        opacity: 0
                    }
                ]
            },
            sharpGlass: {
                type: RADIAL,
                supportVML: false,
                stops: [
                    {
                        offset: 0,
                        color: WHITE,
                        opacity: 0.2
                    },
                    {
                        offset: 0.15,
                        color: WHITE,
                        opacity: 0.15
                    },
                    {
                        offset: 0.17,
                        color: WHITE,
                        opacity: 0.35
                    },
                    {
                        offset: 0.85,
                        color: WHITE,
                        opacity: 0.05
                    },
                    {
                        offset: 0.87,
                        color: WHITE,
                        opacity: 0.15
                    },
                    {
                        offset: 0.99,
                        color: WHITE,
                        opacity: 0
                    }
                ]
            },
            bubbleShadow: {
                type: RADIAL,
                center: [
                    0.5,
                    0.5
                ],
                radius: 0.5
            }
        };
        function boxDiff(r, s) {
            if (r.x1 === s.x1 && r.y1 === s.y1 && r.x2 === s.x2 && r.y2 === s.y2) {
                return s;
            }
            var a = Math.min(r.x1, s.x1);
            var b = Math.max(r.x1, s.x1);
            var c = Math.min(r.x2, s.x2);
            var d = Math.max(r.x2, s.x2);
            var e = Math.min(r.y1, s.y1);
            var f = Math.max(r.y1, s.y1);
            var g = Math.min(r.y2, s.y2);
            var h = Math.max(r.y2, s.y2);
            var boxes = [];
            boxes[0] = new Box(b, e, c, f);
            boxes[1] = new Box(a, f, b, g);
            boxes[2] = new Box(c, f, d, g);
            boxes[3] = new Box(b, g, c, h);
            if (r.x1 === a && r.y1 === e || s.x1 === a && s.y1 === e) {
                boxes[4] = new Box(a, e, b, f);
                boxes[5] = new Box(c, g, d, h);
            } else {
                boxes[4] = new Box(c, e, d, f);
                boxes[5] = new Box(a, g, b, h);
            }
            return grep(boxes, function (box) {
                return box.height() > 0 && box.width() > 0;
            })[0];
        }
        var RootElement = ChartElement.extend({
            init: function (options) {
                ChartElement.fn.init.call(this, options);
                var rootOptions = this.options;
                rootOptions.width = parseInt(rootOptions.width, 10);
                rootOptions.height = parseInt(rootOptions.height, 10);
                this.gradients = {};
            },
            reflow: function () {
                var ref = this;
                var options = ref.options;
                var children = ref.children;
                var currentBox = new Box(0, 0, options.width, options.height);
                this.box = currentBox.unpad(options.margin);
                for (var i = 0; i < children.length; i++) {
                    children[i].reflow(currentBox);
                    currentBox = boxDiff(currentBox, children[i].box) || new Box();
                }
            },
            createVisual: function () {
                this.visual = new Group();
                this.createBackground();
            },
            createBackground: function () {
                var options = this.options;
                var border = options.border || {};
                var box = this.box.clone().pad(options.margin).unpad(border.width);
                var background = Path.fromRect(box.toRect(), {
                    stroke: {
                        color: border.width ? border.color : '',
                        width: border.width,
                        dashType: border.dashType
                    },
                    fill: {
                        color: options.background,
                        opacity: options.opacity
                    },
                    zIndex: -10
                });
                this.visual.append(background);
            },
            getRoot: function () {
                return this;
            },
            createGradient: function (options) {
                var gradients = this.gradients;
                var hashCode = objectKey(options);
                var gradient = GRADIENTS[options.gradient];
                var drawingGradient;
                if (gradients[hashCode]) {
                    drawingGradient = gradients[hashCode];
                } else {
                    var gradientOptions = $.extend({}, gradient, options);
                    if (gradient.type === 'linear') {
                        drawingGradient = new drawing.LinearGradient(gradientOptions);
                    } else {
                        if (options.innerRadius) {
                            gradientOptions.stops = innerRadialStops(gradientOptions);
                        }
                        drawingGradient = new drawing.RadialGradient(gradientOptions);
                        drawingGradient.supportVML = gradient.supportVML !== false;
                    }
                    gradients[hashCode] = drawingGradient;
                }
                return drawingGradient;
            }
        });
        setDefaultOptions(RootElement, {
            width: DEFAULT_WIDTH,
            height: DEFAULT_HEIGHT,
            background: WHITE,
            border: {
                color: BLACK,
                width: 0
            },
            margin: getSpacing(5),
            zIndex: -2
        });
        function innerRadialStops(options) {
            var stops = options.stops;
            var usedSpace = options.innerRadius / options.radius * 100;
            var length = stops.length;
            var currentStops = [];
            for (var i = 0; i < length; i++) {
                var currentStop = $.extend({}, stops[i]);
                currentStop.offset = (currentStop.offset * (100 - usedSpace) + usedSpace) / 100;
                currentStops.push(currentStop);
            }
            return currentStops;
        }
        var FloatElement = ChartElement.extend({
            init: function (options) {
                ChartElement.fn.init.call(this, options);
                this._initDirection();
            },
            _initDirection: function () {
                var options = this.options;
                if (options.vertical) {
                    this.groupAxis = X;
                    this.elementAxis = Y;
                    this.groupSizeField = WIDTH;
                    this.elementSizeField = HEIGHT;
                    this.groupSpacing = options.spacing;
                    this.elementSpacing = options.vSpacing;
                } else {
                    this.groupAxis = Y;
                    this.elementAxis = X;
                    this.groupSizeField = HEIGHT;
                    this.elementSizeField = WIDTH;
                    this.groupSpacing = options.vSpacing;
                    this.elementSpacing = options.spacing;
                }
            },
            reflow: function (targetBox) {
                this.box = targetBox.clone();
                this.reflowChildren();
            },
            reflowChildren: function () {
                var this$1 = this;
                var ref = this;
                var box = ref.box;
                var elementAxis = ref.elementAxis;
                var groupAxis = ref.groupAxis;
                var elementSizeField = ref.elementSizeField;
                var groupSizeField = ref.groupSizeField;
                var ref$1 = this.groupOptions();
                var groups = ref$1.groups;
                var groupsSize = ref$1.groupsSize;
                var maxGroupElementsSize = ref$1.maxGroupElementsSize;
                var groupsCount = groups.length;
                var groupsStart = box[groupAxis + 1] + this.alignStart(groupsSize, box[groupSizeField]());
                if (groupsCount) {
                    var groupStart = groupsStart;
                    for (var groupIdx = 0; groupIdx < groupsCount; groupIdx++) {
                        var group = groups[groupIdx];
                        var groupElements = group.groupElements;
                        var elementStart = box[elementAxis + 1];
                        var groupElementsCount = groupElements.length;
                        for (var idx = 0; idx < groupElementsCount; idx++) {
                            var element = groupElements[idx];
                            var elementSize$$1 = this$1.elementSize(element);
                            var groupElementStart = groupStart + this$1.alignStart(elementSize$$1[groupSizeField], group.groupSize);
                            var elementBox = new Box();
                            elementBox[groupAxis + 1] = groupElementStart;
                            elementBox[groupAxis + 2] = groupElementStart + elementSize$$1[groupSizeField];
                            elementBox[elementAxis + 1] = elementStart;
                            elementBox[elementAxis + 2] = elementStart + elementSize$$1[elementSizeField];
                            element.reflow(elementBox);
                            elementStart += elementSize$$1[elementSizeField] + this$1.elementSpacing;
                        }
                        groupStart += group.groupSize + this$1.groupSpacing;
                    }
                    box[groupAxis + 1] = groupsStart;
                    box[groupAxis + 2] = groupsStart + groupsSize;
                    box[elementAxis + 2] = box[elementAxis + 1] + maxGroupElementsSize;
                }
            },
            alignStart: function (size, maxSize) {
                var start = 0;
                var align = this.options.align;
                if (align === RIGHT || align === BOTTOM) {
                    start = maxSize - size;
                } else if (align === CENTER) {
                    start = (maxSize - size) / 2;
                }
                return start;
            },
            groupOptions: function () {
                var this$1 = this;
                var ref = this;
                var box = ref.box;
                var children = ref.children;
                var elementSizeField = ref.elementSizeField;
                var groupSizeField = ref.groupSizeField;
                var elementSpacing = ref.elementSpacing;
                var groupSpacing = ref.groupSpacing;
                var maxSize = round(box[elementSizeField]());
                var childrenCount = children.length;
                var groups = [];
                var groupSize = 0;
                var groupElementsSize = 0;
                var groupsSize = 0;
                var maxGroupElementsSize = 0;
                var groupElements = [];
                for (var idx = 0; idx < childrenCount; idx++) {
                    var element = children[idx];
                    if (!element.box) {
                        element.reflow(box);
                    }
                    var elementSize$$1 = this$1.elementSize(element);
                    if (this$1.options.wrap && round(groupElementsSize + elementSpacing + elementSize$$1[elementSizeField]) > maxSize) {
                        groups.push({
                            groupElements: groupElements,
                            groupSize: groupSize,
                            groupElementsSize: groupElementsSize
                        });
                        maxGroupElementsSize = Math.max(maxGroupElementsSize, groupElementsSize);
                        groupsSize += groupSpacing + groupSize;
                        groupSize = 0;
                        groupElementsSize = 0;
                        groupElements = [];
                    }
                    groupSize = Math.max(groupSize, elementSize$$1[groupSizeField]);
                    if (groupElementsSize > 0) {
                        groupElementsSize += elementSpacing;
                    }
                    groupElementsSize += elementSize$$1[elementSizeField];
                    groupElements.push(element);
                }
                groups.push({
                    groupElements: groupElements,
                    groupSize: groupSize,
                    groupElementsSize: groupElementsSize
                });
                maxGroupElementsSize = Math.max(maxGroupElementsSize, groupElementsSize);
                groupsSize += groupSize;
                return {
                    groups: groups,
                    groupsSize: groupsSize,
                    maxGroupElementsSize: maxGroupElementsSize
                };
            },
            elementSize: function (element) {
                return {
                    width: element.box.width(),
                    height: element.box.height()
                };
            },
            createVisual: function () {
            }
        });
        setDefaultOptions(FloatElement, {
            vertical: true,
            wrap: true,
            vSpacing: 0,
            spacing: 0
        });
        var DrawingText = drawing.Text;
        var Text = ChartElement.extend({
            init: function (content, options) {
                ChartElement.fn.init.call(this, options);
                this.content = content;
                this.reflow(new Box());
            },
            reflow: function (targetBox) {
                var options = this.options;
                var size = options.size = util.measureText(this.content, { font: options.font });
                this.baseline = size.baseline;
                this.box = new Box(targetBox.x1, targetBox.y1, targetBox.x1 + size.width, targetBox.y1 + size.height);
            },
            createVisual: function () {
                var ref = this.options;
                var font = ref.font;
                var color = ref.color;
                var opacity = ref.opacity;
                var cursor = ref.cursor;
                this.visual = new DrawingText(this.content, this.box.toRect().topLeft(), {
                    font: font,
                    fill: {
                        color: color,
                        opacity: opacity
                    },
                    cursor: cursor
                });
            }
        });
        setDefaultOptions(Text, {
            font: DEFAULT_FONT,
            color: BLACK
        });
        function rectToBox(rect) {
            var origin = rect.origin;
            var bottomRight = rect.bottomRight();
            return new Box(origin.x, origin.y, bottomRight.x, bottomRight.y);
        }
        var ROWS_SPLIT_REGEX = /\n/m;
        var TextBox = BoxElement.extend({
            init: function (content, options) {
                BoxElement.fn.init.call(this, options);
                this.content = content;
                this._initContainer();
                if (this.options._autoReflow !== false) {
                    this.reflow(new Box());
                }
            },
            _initContainer: function () {
                var options = this.options;
                var rows = String(this.content).split(ROWS_SPLIT_REGEX);
                var floatElement = new FloatElement({
                    vertical: true,
                    align: options.align,
                    wrap: false
                });
                var textOptions = deepExtend({}, options, {
                    opacity: 1,
                    animation: null
                });
                this.container = floatElement;
                this.append(floatElement);
                for (var rowIdx = 0; rowIdx < rows.length; rowIdx++) {
                    var text = new Text(rows[rowIdx].trim(), textOptions);
                    floatElement.append(text);
                }
            },
            reflow: function (targetBox) {
                var options = this.options;
                var visualFn = options.visual;
                this.container.options.align = options.align;
                if (visualFn && !this._boxReflow) {
                    var visualBox = targetBox;
                    if (!visualBox.hasSize()) {
                        this._boxReflow = true;
                        this.reflow(visualBox);
                        this._boxReflow = false;
                        visualBox = this.box;
                    }
                    var visual = this.visual = visualFn(this.visualContext(visualBox));
                    if (visual) {
                        visualBox = rectToBox(visual.clippedBBox() || new Rect());
                        visual.options.zIndex = options.zIndex;
                    }
                    this.box = this.contentBox = this.paddingBox = visualBox;
                } else {
                    BoxElement.fn.reflow.call(this, targetBox);
                    if (options.rotation) {
                        var margin = getSpacing(options.margin);
                        var box = this.box.unpad(margin);
                        this.targetBox = targetBox;
                        this.normalBox = box.clone();
                        box = this.rotate();
                        box.translate(margin.left - margin.right, margin.top - margin.bottom);
                        this.rotatedBox = box.clone();
                        box.pad(margin);
                    }
                }
            },
            createVisual: function () {
                var options = this.options;
                if (!options.visible) {
                    return;
                }
                this.visual = new Group({
                    transform: this.rotationTransform(),
                    zIndex: options.zIndex,
                    noclip: options.noclip
                });
                if (this.hasBox()) {
                    var box = Path.fromRect(this.paddingBox.toRect(), this.visualStyle());
                    this.visual.append(box);
                }
            },
            renderVisual: function () {
                if (this.options.visual) {
                    var visual = this.visual;
                    if (visual && !defined(visual.options.noclip)) {
                        visual.options.noclip = this.options.noclip;
                    }
                    this.addVisual();
                    this.createAnimation();
                } else {
                    BoxElement.fn.renderVisual.call(this);
                }
            },
            visualOptions: function () {
                var options = this.options;
                return {
                    background: options.background,
                    border: options.border,
                    color: options.color,
                    font: options.font,
                    margin: options.margin,
                    padding: options.padding,
                    visible: options.visible
                };
            },
            visualContext: function (targetBox) {
                var this$1 = this;
                return {
                    text: this.content,
                    rect: targetBox.toRect(),
                    sender: this.getSender(),
                    options: this.visualOptions(),
                    createVisual: function () {
                        this$1._boxReflow = true;
                        this$1.reflow(targetBox);
                        this$1._boxReflow = false;
                        return this$1.getDefaultVisual();
                    }
                };
            },
            getDefaultVisual: function () {
                this.createVisual();
                this.renderChildren();
                var visual = this.visual;
                delete this.visual;
                return visual;
            },
            rotate: function () {
                var options = this.options;
                this.box.rotate(options.rotation);
                this.align(this.targetBox, X, options.align);
                this.align(this.targetBox, Y, options.vAlign);
                return this.box;
            },
            rotationTransform: function () {
                var rotation = this.options.rotation;
                if (!rotation) {
                    return null;
                }
                var ref = this.normalBox.center();
                var cx = ref.x;
                var cy = ref.y;
                var boxCenter = this.rotatedBox.center();
                return geometryTransform().translate(boxCenter.x - cx, boxCenter.y - cy).rotate(rotation, [
                    cx,
                    cy
                ]);
            }
        });
        var Title = ChartElement.extend({
            init: function (options) {
                ChartElement.fn.init.call(this, options);
                this.append(new TextBox(this.options.text, $.extend({}, this.options, { vAlign: this.options.position })));
            },
            reflow: function (targetBox) {
                ChartElement.fn.reflow.call(this, targetBox);
                this.box.snapTo(targetBox, X);
            }
        });
        Title.buildTitle = function (options, parent, defaultOptions) {
            var titleOptions = options;
            if (typeof options === 'string') {
                titleOptions = { text: options };
            }
            titleOptions = $.extend({ visible: true }, defaultOptions, titleOptions);
            var title;
            if (titleOptions && titleOptions.visible && titleOptions.text) {
                title = new Title(titleOptions);
                parent.append(title);
            }
            return title;
        };
        setDefaultOptions(Title, {
            color: BLACK,
            position: TOP,
            align: CENTER,
            margin: getSpacing(5),
            padding: getSpacing(5)
        });
        var AxisLabel = TextBox.extend({
            init: function (value, text, index, dataItem, options) {
                TextBox.fn.init.call(this, text, options);
                this.text = text;
                this.value = value;
                this.index = index;
                this.dataItem = dataItem;
                this.reflow(new Box());
            },
            visualContext: function (targetBox) {
                var context = TextBox.fn.visualContext.call(this, targetBox);
                context.value = this.value;
                context.dataItem = this.dataItem;
                context.format = this.options.format;
                context.culture = this.options.culture;
                return context;
            },
            click: function (widget, e) {
                widget.trigger(AXIS_LABEL_CLICK, {
                    element: eventElement(e),
                    value: this.value,
                    text: this.text,
                    index: this.index,
                    dataItem: this.dataItem,
                    axis: this.parent.options
                });
            },
            rotate: function () {
                if (this.options.alignRotation !== CENTER) {
                    var box = this.normalBox.toRect();
                    var transform = this.rotationTransform();
                    this.box = rectToBox(box.bbox(transform.matrix()));
                } else {
                    TextBox.fn.rotate.call(this);
                }
                return this.box;
            },
            rotationTransform: function () {
                var options = this.options;
                var rotation = options.rotation;
                if (!rotation) {
                    return null;
                }
                if (options.alignRotation === CENTER) {
                    return TextBox.fn.rotationTransform.call(this);
                }
                var rotationMatrix = geometryTransform().rotate(rotation).matrix();
                var box = this.normalBox.toRect();
                var rect = this.targetBox.toRect();
                var rotationOrigin = options.rotationOrigin || TOP;
                var alignAxis = rotationOrigin === TOP || rotationOrigin === BOTTOM ? X : Y;
                var distanceAxis = rotationOrigin === TOP || rotationOrigin === BOTTOM ? Y : X;
                var axisAnchor = rotationOrigin === TOP || rotationOrigin === LEFT ? rect.origin : rect.bottomRight();
                var topLeft = box.topLeft().transformCopy(rotationMatrix);
                var topRight = box.topRight().transformCopy(rotationMatrix);
                var bottomRight = box.bottomRight().transformCopy(rotationMatrix);
                var bottomLeft = box.bottomLeft().transformCopy(rotationMatrix);
                var rotatedBox = Rect.fromPoints(topLeft, topRight, bottomRight, bottomLeft);
                var translate = {};
                translate[distanceAxis] = rect.origin[distanceAxis] - rotatedBox.origin[distanceAxis];
                var distanceLeft = Math.abs(topLeft[distanceAxis] + translate[distanceAxis] - axisAnchor[distanceAxis]);
                var distanceRight = Math.abs(topRight[distanceAxis] + translate[distanceAxis] - axisAnchor[distanceAxis]);
                var alignStart, alignEnd;
                if (round(distanceLeft, DEFAULT_PRECISION) === round(distanceRight, DEFAULT_PRECISION)) {
                    alignStart = topLeft;
                    alignEnd = topRight;
                } else if (distanceRight < distanceLeft) {
                    alignStart = topRight;
                    alignEnd = bottomRight;
                } else {
                    alignStart = topLeft;
                    alignEnd = bottomLeft;
                }
                var alignCenter = alignStart[alignAxis] + (alignEnd[alignAxis] - alignStart[alignAxis]) / 2;
                translate[alignAxis] = rect.center()[alignAxis] - alignCenter;
                return geometryTransform().translate(translate.x, translate.y).rotate(rotation);
            }
        });
        setDefaultOptions(AxisLabel, { _autoReflow: false });
        var DEFAULT_ICON_SIZE = 7;
        var DEFAULT_LABEL_COLOR = '#fff';
        var Note = BoxElement.extend({
            init: function (fields, options, chartService) {
                BoxElement.fn.init.call(this, options);
                this.fields = fields;
                this.chartService = chartService;
                this.render();
            },
            hide: function () {
                this.options.visible = false;
            },
            show: function () {
                this.options.visible = true;
            },
            render: function () {
                var options = this.options;
                if (options.visible) {
                    var label = options.label;
                    var icon = options.icon;
                    var box = new Box();
                    var size = icon.size;
                    var text = this.fields.text;
                    var width, height;
                    if (defined(label) && label.visible) {
                        var noteTemplate = getTemplate(label);
                        if (noteTemplate) {
                            text = noteTemplate(this.fields);
                        } else if (label.format) {
                            text = this.chartService.format.auto(label.format, text);
                        }
                        if (!label.color) {
                            label.color = label.position === INSIDE ? DEFAULT_LABEL_COLOR : icon.background;
                        }
                        this.label = new TextBox(text, deepExtend({}, label));
                        if (label.position === INSIDE && !defined(size)) {
                            if (icon.type === CIRCLE) {
                                size = Math.max(this.label.box.width(), this.label.box.height());
                            } else {
                                width = this.label.box.width();
                                height = this.label.box.height();
                            }
                            box.wrap(this.label.box);
                        }
                    }
                    icon.width = width || size || DEFAULT_ICON_SIZE;
                    icon.height = height || size || DEFAULT_ICON_SIZE;
                    var marker = new ShapeElement(deepExtend({}, icon));
                    this.marker = marker;
                    this.append(marker);
                    if (this.label) {
                        this.append(this.label);
                    }
                    marker.reflow(new Box());
                    this.wrapperBox = box.wrap(marker.box);
                }
            },
            reflow: function (targetBox) {
                var ref = this;
                var options = ref.options;
                var label = ref.label;
                var marker = ref.marker;
                var wrapperBox = ref.wrapperBox;
                var center = targetBox.center();
                var length = options.line.length;
                var position = options.position;
                if (options.visible) {
                    var lineStart, box, contentBox;
                    if (inArray(position, [
                            LEFT,
                            RIGHT
                        ])) {
                        if (position === LEFT) {
                            contentBox = wrapperBox.alignTo(targetBox, position).translate(-length, targetBox.center().y - wrapperBox.center().y);
                            if (options.line.visible) {
                                lineStart = [
                                    targetBox.x1,
                                    center.y
                                ];
                                this.linePoints = [
                                    lineStart,
                                    [
                                        contentBox.x2,
                                        center.y
                                    ]
                                ];
                                box = contentBox.clone().wrapPoint(lineStart);
                            }
                        } else {
                            contentBox = wrapperBox.alignTo(targetBox, position).translate(length, targetBox.center().y - wrapperBox.center().y);
                            if (options.line.visible) {
                                lineStart = [
                                    targetBox.x2,
                                    center.y
                                ];
                                this.linePoints = [
                                    lineStart,
                                    [
                                        contentBox.x1,
                                        center.y
                                    ]
                                ];
                                box = contentBox.clone().wrapPoint(lineStart);
                            }
                        }
                    } else {
                        if (position === BOTTOM) {
                            contentBox = wrapperBox.alignTo(targetBox, position).translate(targetBox.center().x - wrapperBox.center().x, length);
                            if (options.line.visible) {
                                lineStart = [
                                    center.x,
                                    targetBox.y2
                                ];
                                this.linePoints = [
                                    lineStart,
                                    [
                                        center.x,
                                        contentBox.y1
                                    ]
                                ];
                                box = contentBox.clone().wrapPoint(lineStart);
                            }
                        } else {
                            contentBox = wrapperBox.alignTo(targetBox, position).translate(targetBox.center().x - wrapperBox.center().x, -length);
                            if (options.line.visible) {
                                lineStart = [
                                    center.x,
                                    targetBox.y1
                                ];
                                this.linePoints = [
                                    lineStart,
                                    [
                                        center.x,
                                        contentBox.y2
                                    ]
                                ];
                                box = contentBox.clone().wrapPoint(lineStart);
                            }
                        }
                    }
                    if (marker) {
                        marker.reflow(contentBox);
                    }
                    if (label) {
                        label.reflow(contentBox);
                        if (marker) {
                            if (options.label.position === OUTSIDE) {
                                label.box.alignTo(marker.box, position);
                            }
                            label.reflow(label.box);
                        }
                    }
                    this.contentBox = contentBox;
                    this.targetBox = targetBox;
                    this.box = box || contentBox;
                }
            },
            createVisual: function () {
                BoxElement.fn.createVisual.call(this);
                this.visual.options.noclip = this.options.noclip;
                if (this.options.visible) {
                    this.createLine();
                }
            },
            renderVisual: function () {
                var this$1 = this;
                var options = this.options;
                var customVisual = options.visual;
                if (options.visible && customVisual) {
                    this.visual = customVisual($.extend(this.fields, {
                        sender: this.getSender(),
                        rect: this.targetBox.toRect(),
                        options: {
                            background: options.background,
                            border: options.background,
                            icon: options.icon,
                            label: options.label,
                            line: options.line,
                            position: options.position,
                            visible: options.visible
                        },
                        createVisual: function () {
                            this$1.createVisual();
                            this$1.renderChildren();
                            var defaultVisual = this$1.visual;
                            delete this$1.visual;
                            return defaultVisual;
                        }
                    }));
                    this.addVisual();
                } else {
                    BoxElement.fn.renderVisual.call(this);
                }
            },
            createLine: function () {
                var options = this.options.line;
                if (this.linePoints) {
                    var path = Path.fromPoints(this.linePoints, {
                        stroke: {
                            color: options.color,
                            width: options.width,
                            dashType: options.dashType
                        }
                    });
                    alignPathToPixel(path);
                    this.visual.append(path);
                }
            },
            click: function (widget, e) {
                var args = this.eventArgs(e);
                if (!widget.trigger(NOTE_CLICK, args)) {
                    e.preventDefault();
                }
            },
            hover: function (widget, e) {
                var args = this.eventArgs(e);
                if (!widget.trigger(NOTE_HOVER, args)) {
                    e.preventDefault();
                }
            },
            leave: function (widget) {
                widget._unsetActivePoint();
            },
            eventArgs: function (e) {
                var options = this.options;
                return $.extend(this.fields, {
                    element: eventElement(e),
                    text: defined(options.label) ? options.label.text : '',
                    visual: this.visual
                });
            }
        });
        setDefaultOptions(Note, {
            icon: {
                visible: true,
                type: CIRCLE
            },
            label: {
                position: INSIDE,
                visible: true,
                align: CENTER,
                vAlign: CENTER
            },
            line: { visible: true },
            visible: true,
            position: TOP,
            zIndex: 2
        });
        function createAxisTick(options, tickOptions) {
            var tickX = options.tickX;
            var tickY = options.tickY;
            var position = options.position;
            var tick = new Path({
                stroke: {
                    width: tickOptions.width,
                    color: tickOptions.color
                }
            });
            if (options.vertical) {
                tick.moveTo(tickX, position).lineTo(tickX + tickOptions.size, position);
            } else {
                tick.moveTo(position, tickY).lineTo(position, tickY + tickOptions.size);
            }
            alignPathToPixel(tick);
            return tick;
        }
        function createAxisGridLine(options, gridLine) {
            var lineStart = options.lineStart;
            var lineEnd = options.lineEnd;
            var position = options.position;
            var line = new Path({
                stroke: {
                    width: gridLine.width,
                    color: gridLine.color,
                    dashType: gridLine.dashType
                }
            });
            if (options.vertical) {
                line.moveTo(lineStart, position).lineTo(lineEnd, position);
            } else {
                line.moveTo(position, lineStart).lineTo(position, lineEnd);
            }
            alignPathToPixel(line);
            return line;
        }
        var Axis = ChartElement.extend({
            init: function (options, chartService) {
                if (chartService === void 0) {
                    chartService = new ChartService();
                }
                ChartElement.fn.init.call(this, options);
                this.chartService = chartService;
                if (!this.options.visible) {
                    this.options = deepExtend({}, this.options, {
                        labels: { visible: false },
                        line: { visible: false },
                        margin: 0,
                        majorTickSize: 0,
                        minorTickSize: 0
                    });
                }
                this.options.minorTicks = deepExtend({}, {
                    color: this.options.line.color,
                    width: this.options.line.width,
                    visible: this.options.minorTickType !== NONE
                }, this.options.minorTicks, {
                    size: this.options.minorTickSize,
                    align: this.options.minorTickType
                });
                this.options.majorTicks = deepExtend({}, {
                    color: this.options.line.color,
                    width: this.options.line.width,
                    visible: this.options.majorTickType !== NONE
                }, this.options.majorTicks, {
                    size: this.options.majorTickSize,
                    align: this.options.majorTickType
                });
                if (!this.options._deferLabels) {
                    this.createLabels();
                }
                this.createTitle();
                this.createNotes();
            },
            labelsRange: function () {
                return {
                    min: this.options.labels.skip,
                    max: this.labelsCount()
                };
            },
            createLabels: function () {
                var this$1 = this;
                var options = this.options;
                var align = options.vertical ? RIGHT : CENTER;
                var labelOptions = deepExtend({}, options.labels, {
                    align: align,
                    zIndex: options.zIndex
                });
                var step = Math.max(1, labelOptions.step);
                this.children = grep(this.children, function (child) {
                    return !(child instanceof AxisLabel);
                });
                this.labels = [];
                if (labelOptions.visible) {
                    var range = this.labelsRange();
                    var rotation = labelOptions.rotation;
                    if (isObject(rotation)) {
                        labelOptions.alignRotation = rotation.align;
                        labelOptions.rotation = rotation.angle;
                    }
                    if (labelOptions.rotation === 'auto') {
                        labelOptions.rotation = 0;
                        options.autoRotateLabels = true;
                    }
                    for (var idx = range.min; idx < range.max; idx += step) {
                        var label = this$1.createAxisLabel(idx, labelOptions);
                        if (label) {
                            this$1.append(label);
                            this$1.labels.push(label);
                        }
                    }
                }
            },
            lineBox: function () {
                var ref = this;
                var options = ref.options;
                var box = ref.box;
                var vertical = options.vertical;
                var mirror = options.labels.mirror;
                var axisX = mirror ? box.x1 : box.x2;
                var axisY = mirror ? box.y2 : box.y1;
                var lineWidth = options.line.width || 0;
                return vertical ? new Box(axisX, box.y1, axisX, box.y2 - lineWidth) : new Box(box.x1, axisY, box.x2 - lineWidth, axisY);
            },
            createTitle: function () {
                var options = this.options;
                var titleOptions = deepExtend({
                    rotation: options.vertical ? -90 : 0,
                    text: '',
                    zIndex: 1,
                    visualSize: true
                }, options.title);
                if (titleOptions.visible && titleOptions.text) {
                    var title = new TextBox(titleOptions.text, titleOptions);
                    this.append(title);
                    this.title = title;
                }
            },
            createNotes: function () {
                var this$1 = this;
                var options = this.options;
                var notes = options.notes;
                var items = notes.data || [];
                this.notes = [];
                for (var i = 0; i < items.length; i++) {
                    var item = deepExtend({}, notes, items[i]);
                    item.value = this$1.parseNoteValue(item.value);
                    var note = new Note({
                        value: item.value,
                        text: item.label.text,
                        dataItem: item
                    }, item, this$1.chartService);
                    if (note.options.visible) {
                        if (defined(note.options.position)) {
                            if (options.vertical && !inArray(note.options.position, [
                                    LEFT,
                                    RIGHT
                                ])) {
                                note.options.position = options.reverse ? LEFT : RIGHT;
                            } else if (!options.vertical && !inArray(note.options.position, [
                                    TOP,
                                    BOTTOM
                                ])) {
                                note.options.position = options.reverse ? BOTTOM : TOP;
                            }
                        } else {
                            if (options.vertical) {
                                note.options.position = options.reverse ? LEFT : RIGHT;
                            } else {
                                note.options.position = options.reverse ? BOTTOM : TOP;
                            }
                        }
                        this$1.append(note);
                        this$1.notes.push(note);
                    }
                }
            },
            parseNoteValue: function (value) {
                return value;
            },
            renderVisual: function () {
                ChartElement.fn.renderVisual.call(this);
                this.createPlotBands();
            },
            createVisual: function () {
                ChartElement.fn.createVisual.call(this);
                this.createBackground();
                this.createLine();
            },
            gridLinesVisual: function () {
                var gridLines = this._gridLines;
                if (!gridLines) {
                    gridLines = this._gridLines = new Group({ zIndex: -2 });
                    this.appendVisual(this._gridLines);
                }
                return gridLines;
            },
            createTicks: function (lineGroup) {
                var options = this.options;
                var lineBox = this.lineBox();
                var mirror = options.labels.mirror;
                var majorUnit = options.majorTicks.visible ? options.majorUnit : 0;
                var tickLineOptions = { vertical: options.vertical };
                function render(tickPositions, tickOptions, skipUnit) {
                    var count = tickPositions.length;
                    var step = Math.max(1, tickOptions.step);
                    if (tickOptions.visible) {
                        for (var i = tickOptions.skip; i < count; i += step) {
                            if (defined(skipUnit) && i % skipUnit === 0) {
                                continue;
                            }
                            tickLineOptions.tickX = mirror ? lineBox.x2 : lineBox.x2 - tickOptions.size;
                            tickLineOptions.tickY = mirror ? lineBox.y1 - tickOptions.size : lineBox.y1;
                            tickLineOptions.position = tickPositions[i];
                            lineGroup.append(createAxisTick(tickLineOptions, tickOptions));
                        }
                    }
                }
                render(this.getMajorTickPositions(), options.majorTicks);
                render(this.getMinorTickPositions(), options.minorTicks, majorUnit / options.minorUnit);
            },
            createLine: function () {
                var options = this.options;
                var line = options.line;
                var lineBox = this.lineBox();
                if (line.width > 0 && line.visible) {
                    var path = new Path({
                        stroke: {
                            width: line.width,
                            color: line.color,
                            dashType: line.dashType
                        }
                    });
                    path.moveTo(lineBox.x1, lineBox.y1).lineTo(lineBox.x2, lineBox.y2);
                    if (options._alignLines) {
                        alignPathToPixel(path);
                    }
                    var group = this._lineGroup = new Group();
                    group.append(path);
                    this.visual.append(group);
                    this.createTicks(group);
                }
            },
            getActualTickSize: function () {
                var options = this.options;
                var tickSize = 0;
                if (options.majorTicks.visible && options.minorTicks.visible) {
                    tickSize = Math.max(options.majorTicks.size, options.minorTicks.size);
                } else if (options.majorTicks.visible) {
                    tickSize = options.majorTicks.size;
                } else if (options.minorTicks.visible) {
                    tickSize = options.minorTicks.size;
                }
                return tickSize;
            },
            createBackground: function () {
                var ref = this;
                var options = ref.options;
                var box = ref.box;
                var background = options.background;
                if (background) {
                    this._backgroundPath = Path.fromRect(box.toRect(), {
                        fill: { color: background },
                        stroke: null
                    });
                    this.visual.append(this._backgroundPath);
                }
            },
            createPlotBands: function () {
                var this$1 = this;
                var options = this.options;
                var plotBands = options.plotBands || [];
                var vertical = options.vertical;
                var plotArea = this.plotArea;
                if (plotBands.length === 0) {
                    return;
                }
                var group = this._plotbandGroup = new Group({ zIndex: -1 });
                var altAxis = grep(this.pane.axes, function (axis) {
                    return axis.options.vertical !== this$1.options.vertical;
                })[0];
                for (var idx = 0; idx < plotBands.length; idx++) {
                    var item = plotBands[idx];
                    var slotX = void 0, slotY = void 0;
                    if (vertical) {
                        slotX = (altAxis || plotArea.axisX).lineBox();
                        slotY = this$1.getSlot(item.from, item.to, true);
                    } else {
                        slotX = this$1.getSlot(item.from, item.to, true);
                        slotY = (altAxis || plotArea.axisY).lineBox();
                    }
                    if (slotX.width() !== 0 && slotY.height() !== 0) {
                        var bandRect = new Rect([
                            slotX.x1,
                            slotY.y1
                        ], [
                            slotX.width(),
                            slotY.height()
                        ]);
                        var path = Path.fromRect(bandRect, {
                            fill: {
                                color: item.color,
                                opacity: item.opacity
                            },
                            stroke: null
                        });
                        group.append(path);
                    }
                }
                this.appendVisual(group);
            },
            createGridLines: function (altAxis) {
                var options = this.options;
                var minorGridLines = options.minorGridLines;
                var majorGridLines = options.majorGridLines;
                var minorUnit = options.minorUnit;
                var vertical = options.vertical;
                var axisLineVisible = altAxis.options.line.visible;
                var majorUnit = majorGridLines.visible ? options.majorUnit : 0;
                var lineBox = altAxis.lineBox();
                var linePos = lineBox[vertical ? 'y1' : 'x1'];
                var lineOptions = {
                    lineStart: lineBox[vertical ? 'x1' : 'y1'],
                    lineEnd: lineBox[vertical ? 'x2' : 'y2'],
                    vertical: vertical
                };
                var majorTicks = [];
                var container = this.gridLinesVisual();
                function render(tickPositions, gridLine, skipUnit) {
                    var count = tickPositions.length;
                    var step = Math.max(1, gridLine.step);
                    if (gridLine.visible) {
                        for (var i = gridLine.skip; i < count; i += step) {
                            var pos = round(tickPositions[i]);
                            if (!inArray(pos, majorTicks)) {
                                if (i % skipUnit !== 0 && (!axisLineVisible || linePos !== pos)) {
                                    lineOptions.position = pos;
                                    container.append(createAxisGridLine(lineOptions, gridLine));
                                    majorTicks.push(pos);
                                }
                            }
                        }
                    }
                }
                render(this.getMajorTickPositions(), majorGridLines);
                render(this.getMinorTickPositions(), minorGridLines, majorUnit / minorUnit);
                return container.children;
            },
            reflow: function (box) {
                var ref = this;
                var options = ref.options;
                var labels = ref.labels;
                var title = ref.title;
                var vertical = options.vertical;
                var count = labels.length;
                var sizeFn = vertical ? WIDTH : HEIGHT;
                var titleSize = title ? title.box[sizeFn]() : 0;
                var space = this.getActualTickSize() + options.margin + titleSize;
                var rootBox = (this.getRoot() || {}).box || box;
                var boxSize = rootBox[sizeFn]();
                var maxLabelSize = 0;
                for (var i = 0; i < count; i++) {
                    var labelSize = labels[i].box[sizeFn]();
                    if (labelSize + space <= boxSize) {
                        maxLabelSize = Math.max(maxLabelSize, labelSize);
                    }
                }
                if (vertical) {
                    this.box = new Box(box.x1, box.y1, box.x1 + maxLabelSize + space, box.y2);
                } else {
                    this.box = new Box(box.x1, box.y1, box.x2, box.y1 + maxLabelSize + space);
                }
                this.arrangeTitle();
                this.arrangeLabels();
                this.arrangeNotes();
            },
            getLabelsTickPositions: function () {
                return this.getMajorTickPositions();
            },
            labelTickIndex: function (label) {
                return label.index;
            },
            arrangeLabels: function () {
                var this$1 = this;
                var ref = this;
                var options = ref.options;
                var labels = ref.labels;
                var labelsBetweenTicks = !options.justified;
                var vertical = options.vertical;
                var lineBox = this.lineBox();
                var mirror = options.labels.mirror;
                var tickPositions = this.getLabelsTickPositions();
                var labelOffset = this.getActualTickSize() + options.margin;
                for (var idx = 0; idx < labels.length; idx++) {
                    var label = labels[idx];
                    var tickIx = this$1.labelTickIndex(label);
                    var labelSize = vertical ? label.box.height() : label.box.width();
                    var labelPos = tickPositions[tickIx] - labelSize / 2;
                    var labelBox = void 0, firstTickPosition = void 0, nextTickPosition = void 0;
                    if (vertical) {
                        if (labelsBetweenTicks) {
                            firstTickPosition = tickPositions[tickIx];
                            nextTickPosition = tickPositions[tickIx + 1];
                            var middle = firstTickPosition + (nextTickPosition - firstTickPosition) / 2;
                            labelPos = middle - labelSize / 2;
                        }
                        var labelX = lineBox.x2;
                        if (mirror) {
                            labelX += labelOffset;
                            label.options.rotationOrigin = LEFT;
                        } else {
                            labelX -= labelOffset + label.box.width();
                            label.options.rotationOrigin = RIGHT;
                        }
                        labelBox = label.box.move(labelX, labelPos);
                    } else {
                        if (labelsBetweenTicks) {
                            firstTickPosition = tickPositions[tickIx];
                            nextTickPosition = tickPositions[tickIx + 1];
                        } else {
                            firstTickPosition = labelPos;
                            nextTickPosition = labelPos + labelSize;
                        }
                        var labelY = lineBox.y1;
                        if (mirror) {
                            labelY -= labelOffset + label.box.height();
                            label.options.rotationOrigin = BOTTOM;
                        } else {
                            labelY += labelOffset;
                            label.options.rotationOrigin = TOP;
                        }
                        labelBox = new Box(firstTickPosition, labelY, nextTickPosition, labelY + label.box.height());
                    }
                    label.reflow(labelBox);
                }
            },
            autoRotateLabels: function () {
                if (this.options.autoRotateLabels && !this.options.vertical) {
                    var tickPositions = this.getMajorTickPositions();
                    var labels = this.labels;
                    var angle;
                    for (var idx = 0; idx < labels.length; idx++) {
                        var width = Math.abs(tickPositions[idx + 1] - tickPositions[idx]);
                        var labelBox = labels[idx].box;
                        if (labelBox.width() > width) {
                            if (labelBox.height() > width) {
                                angle = -90;
                                break;
                            }
                            angle = -45;
                        }
                    }
                    if (angle) {
                        for (var idx$1 = 0; idx$1 < labels.length; idx$1++) {
                            labels[idx$1].options.rotation = angle;
                            labels[idx$1].reflow(new Box());
                        }
                        return true;
                    }
                }
            },
            arrangeTitle: function () {
                var ref = this;
                var options = ref.options;
                var title = ref.title;
                var mirror = options.labels.mirror;
                var vertical = options.vertical;
                if (title) {
                    if (vertical) {
                        title.options.align = mirror ? RIGHT : LEFT;
                        title.options.vAlign = title.options.position;
                    } else {
                        title.options.align = title.options.position;
                        title.options.vAlign = mirror ? TOP : BOTTOM;
                    }
                    title.reflow(this.box);
                }
            },
            arrangeNotes: function () {
                var this$1 = this;
                for (var idx = 0; idx < this.notes.length; idx++) {
                    var item = this$1.notes[idx];
                    var value = item.options.value;
                    var slot = void 0;
                    if (defined(value)) {
                        if (this$1.shouldRenderNote(value)) {
                            item.show();
                        } else {
                            item.hide();
                        }
                        slot = this$1.noteSlot(value);
                    } else {
                        item.hide();
                    }
                    item.reflow(slot || this$1.lineBox());
                }
            },
            noteSlot: function (value) {
                return this.getSlot(value);
            },
            alignTo: function (secondAxis) {
                var lineBox = secondAxis.lineBox();
                var vertical = this.options.vertical;
                var pos = vertical ? Y : X;
                this.box.snapTo(lineBox, pos);
                if (vertical) {
                    this.box.shrink(0, this.lineBox().height() - lineBox.height());
                } else {
                    this.box.shrink(this.lineBox().width() - lineBox.width(), 0);
                }
                this.box[pos + 1] -= this.lineBox()[pos + 1] - lineBox[pos + 1];
                this.box[pos + 2] -= this.lineBox()[pos + 2] - lineBox[pos + 2];
            },
            axisLabelText: function (value, dataItem, options) {
                var tmpl = getTemplate(options);
                var text = value;
                if (tmpl) {
                    text = tmpl({
                        value: value,
                        dataItem: dataItem,
                        format: options.format,
                        culture: options.culture
                    });
                } else if (options.format) {
                    text = this.chartService.format.localeAuto(options.format, [value], options.culture);
                }
                return text;
            },
            slot: function (from, to, limit) {
                var slot = this.getSlot(from, to, limit);
                if (slot) {
                    return slot.toRect();
                }
            },
            contentBox: function () {
                var box = this.box.clone();
                var labels = this.labels;
                if (labels.length) {
                    if (labels[0].options.visible) {
                        box.wrap(labels[0].box);
                    }
                    var lastLabel = labels[labels.length - 1];
                    if (lastLabel.options.visible) {
                        box.wrap(lastLabel.box);
                    }
                }
                return box;
            },
            limitRange: function (from, to, min, max, offset) {
                var options = this.options;
                if (from < min && offset < 0 && (!defined(options.min) || options.min <= min) || max < to && offset > 0 && (!defined(options.max) || max <= options.max)) {
                    return null;
                }
                if (to < min && offset > 0 || max < from && offset < 0) {
                    return {
                        min: from,
                        max: to
                    };
                }
                var rangeSize = to - from;
                var minValue = from;
                var maxValue = to;
                if (from < min) {
                    minValue = limitValue(from, min, max);
                    maxValue = limitValue(from + rangeSize, min + rangeSize, max);
                } else if (to > max) {
                    maxValue = limitValue(to, min, max);
                    minValue = limitValue(to - rangeSize, min, max - rangeSize);
                }
                return {
                    min: minValue,
                    max: maxValue
                };
            },
            valueRange: function () {
                return {
                    min: this.seriesMin,
                    max: this.seriesMax
                };
            }
        });
        setDefaultOptions(Axis, {
            labels: {
                visible: true,
                rotation: 0,
                mirror: false,
                step: 1,
                skip: 0
            },
            line: {
                width: 1,
                color: BLACK,
                visible: true
            },
            title: {
                visible: true,
                position: CENTER
            },
            majorTicks: {
                align: OUTSIDE,
                size: 4,
                skip: 0,
                step: 1
            },
            minorTicks: {
                align: OUTSIDE,
                size: 3,
                skip: 0,
                step: 1
            },
            axisCrossingValue: 0,
            majorTickType: OUTSIDE,
            minorTickType: NONE,
            majorGridLines: {
                skip: 0,
                step: 1
            },
            minorGridLines: {
                visible: false,
                width: 1,
                color: BLACK,
                skip: 0,
                step: 1
            },
            margin: 5,
            visible: true,
            reverse: false,
            justified: true,
            notes: { label: { text: '' } },
            _alignLines: true,
            _deferLabels: false
        });
        var MILLISECONDS = 'milliseconds';
        var SECONDS = 'seconds';
        var MINUTES = 'minutes';
        var HOURS = 'hours';
        var DAYS = 'days';
        var WEEKS = 'weeks';
        var MONTHS = 'months';
        var YEARS = 'years';
        var TIME_PER_MILLISECOND = 1;
        var TIME_PER_SECOND = 1000;
        var TIME_PER_MINUTE = 60 * TIME_PER_SECOND;
        var TIME_PER_HOUR = 60 * TIME_PER_MINUTE;
        var TIME_PER_DAY = 24 * TIME_PER_HOUR;
        var TIME_PER_WEEK = 7 * TIME_PER_DAY;
        var TIME_PER_MONTH = 31 * TIME_PER_DAY;
        var TIME_PER_YEAR = 365 * TIME_PER_DAY;
        var TIME_PER_UNIT = {
            'years': TIME_PER_YEAR,
            'months': TIME_PER_MONTH,
            'weeks': TIME_PER_WEEK,
            'days': TIME_PER_DAY,
            'hours': TIME_PER_HOUR,
            'minutes': TIME_PER_MINUTE,
            'seconds': TIME_PER_SECOND,
            'milliseconds': TIME_PER_MILLISECOND
        };
        function absoluteDateDiff(a, b) {
            var diff = a.getTime() - b;
            var offsetDiff = a.getTimezoneOffset() - b.getTimezoneOffset();
            return diff - offsetDiff * TIME_PER_MINUTE;
        }
        function addTicks(date, ticks) {
            return new Date(date.getTime() + ticks);
        }
        function toDate(value) {
            var result;
            if (value instanceof Date) {
                result = value;
            } else if (value) {
                result = new Date(value);
            }
            return result;
        }
        function startOfWeek(date, weekStartDay) {
            if (weekStartDay === void 0) {
                weekStartDay = 0;
            }
            var daysToSubtract = 0;
            var day = date.getDay();
            if (!isNaN(day)) {
                while (day !== weekStartDay) {
                    if (day === 0) {
                        day = 6;
                    } else {
                        day--;
                    }
                    daysToSubtract++;
                }
            }
            return addTicks(date, -daysToSubtract * TIME_PER_DAY);
        }
        function adjustDST(date, hours) {
            if (hours === 0 && date.getHours() === 23) {
                date.setHours(date.getHours() + 2);
                return true;
            }
            return false;
        }
        function addHours(date, hours) {
            var roundedDate = new Date(date);
            roundedDate.setMinutes(0, 0, 0);
            var tzDiff = (date.getTimezoneOffset() - roundedDate.getTimezoneOffset()) * TIME_PER_MINUTE;
            return addTicks(roundedDate, tzDiff + hours * TIME_PER_HOUR);
        }
        function addDuration(dateValue, value, unit, weekStartDay) {
            var result = dateValue;
            if (dateValue) {
                var date = toDate(dateValue);
                var hours = date.getHours();
                if (unit === YEARS) {
                    result = new Date(date.getFullYear() + value, 0, 1);
                    adjustDST(result, 0);
                } else if (unit === MONTHS) {
                    result = new Date(date.getFullYear(), date.getMonth() + value, 1);
                    adjustDST(result, hours);
                } else if (unit === WEEKS) {
                    result = addDuration(startOfWeek(date, weekStartDay), value * 7, DAYS);
                    adjustDST(result, hours);
                } else if (unit === DAYS) {
                    result = new Date(date.getFullYear(), date.getMonth(), date.getDate() + value);
                    adjustDST(result, hours);
                } else if (unit === HOURS) {
                    result = addHours(date, value);
                } else if (unit === MINUTES) {
                    result = addTicks(date, value * TIME_PER_MINUTE);
                    if (result.getSeconds() > 0) {
                        result.setSeconds(0);
                    }
                } else if (unit === SECONDS) {
                    result = addTicks(date, value * TIME_PER_SECOND);
                } else if (unit === MILLISECONDS) {
                    result = addTicks(date, value);
                }
                if (unit !== MILLISECONDS && result.getMilliseconds() > 0) {
                    result.setMilliseconds(0);
                }
            }
            return result;
        }
        function floorDate(date, unit, weekStartDay) {
            return addDuration(toDate(date), 0, unit, weekStartDay);
        }
        function ceilDate(dateValue, unit, weekStartDay) {
            var date = toDate(dateValue);
            if (date && floorDate(date, unit, weekStartDay).getTime() === date.getTime()) {
                return date;
            }
            return addDuration(date, 1, unit, weekStartDay);
        }
        function dateComparer(a, b) {
            if (a && b) {
                return a.getTime() - b.getTime();
            }
            return -1;
        }
        function dateDiff(a, b) {
            return a.getTime() - b;
        }
        function toTime(value) {
            if (isArray(value)) {
                var result = [];
                for (var idx = 0; idx < value.length; idx++) {
                    result.push(toTime(value[idx]));
                }
                return result;
            } else if (value) {
                return toDate(value).getTime();
            }
        }
        function dateEquals(a, b) {
            if (a && b) {
                return toTime(a) === toTime(b);
            }
            return a === b;
        }
        function timeIndex(date, start, baseUnit) {
            return absoluteDateDiff(date, start) / TIME_PER_UNIT[baseUnit];
        }
        function dateIndex(value, start, baseUnit, baseUnitStep) {
            var date = toDate(value);
            var startDate = toDate(start);
            var index;
            if (baseUnit === MONTHS) {
                index = date.getMonth() - startDate.getMonth() + (date.getFullYear() - startDate.getFullYear()) * 12 + timeIndex(date, new Date(date.getFullYear(), date.getMonth()), DAYS) / new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate();
            } else if (baseUnit === YEARS) {
                index = date.getFullYear() - startDate.getFullYear() + dateIndex(date, new Date(date.getFullYear(), 0), MONTHS, 1) / 12;
            } else if (baseUnit === DAYS || baseUnit === WEEKS) {
                index = timeIndex(date, startDate, baseUnit);
            } else {
                index = dateDiff(date, start) / TIME_PER_UNIT[baseUnit];
            }
            return index / baseUnitStep;
        }
        function duration(a, b, unit) {
            var diff;
            if (unit === YEARS) {
                diff = b.getFullYear() - a.getFullYear();
            } else if (unit === MONTHS) {
                diff = duration(a, b, YEARS) * 12 + b.getMonth() - a.getMonth();
            } else if (unit === DAYS) {
                diff = Math.floor(dateDiff(b, a) / TIME_PER_DAY);
            } else {
                diff = Math.floor(dateDiff(b, a) / TIME_PER_UNIT[unit]);
            }
            return diff;
        }
        function lteDateIndex(date, sortedDates) {
            var low = 0;
            var high = sortedDates.length - 1;
            var index;
            while (low <= high) {
                index = Math.floor((low + high) / 2);
                var currentDate = sortedDates[index];
                if (currentDate < date) {
                    low = index + 1;
                    continue;
                }
                if (currentDate > date) {
                    high = index - 1;
                    continue;
                }
                while (dateEquals(sortedDates[index - 1], date)) {
                    index--;
                }
                return index;
            }
            if (sortedDates[index] <= date) {
                return index;
            }
            return index - 1;
        }
        function parseDate(intlService, date) {
            var result;
            if (isString(date)) {
                result = intlService.parseDate(date) || toDate(date);
            } else {
                result = toDate(date);
            }
            return result;
        }
        function parseDates(intlService, dates) {
            if (isArray(dates)) {
                var result = [];
                for (var idx = 0; idx < dates.length; idx++) {
                    result.push(parseDate(intlService, dates[idx]));
                }
                return result;
            }
            return parseDate(intlService, dates);
        }
        var MIN_CATEGORY_POINTS_RANGE = 0.01;
        function indexOf(value, arr) {
            if (value instanceof Date) {
                var length = arr.length;
                for (var idx = 0; idx < length; idx++) {
                    if (dateEquals(arr[idx], value)) {
                        return idx;
                    }
                }
                return -1;
            }
            return arr.indexOf(value);
        }
        var CategoryAxis = Axis.extend({
            init: function (options, chartService) {
                Axis.fn.init.call(this, options, chartService);
                this._ticks = {};
                this._initCategories(this.options);
            },
            _initCategories: function (options) {
                var categories = (options.categories || []).slice(0);
                var definedMin = defined(options.min);
                var definedMax = defined(options.max);
                options.categories = categories;
                if ((definedMin || definedMax) && categories.length) {
                    options.srcCategories = options.categories;
                    var min = definedMin ? Math.floor(options.min) : 0;
                    var max;
                    if (definedMax) {
                        max = options.justified ? Math.floor(options.max) + 1 : Math.ceil(options.max);
                    } else {
                        max = categories.length;
                    }
                    options.categories = options.categories.slice(min, max);
                }
            },
            rangeIndices: function () {
                var options = this.options;
                var length = options.categories.length || 1;
                var min = isNumber(options.min) ? options.min % 1 : 0;
                var max;
                if (isNumber(options.max) && options.max % 1 !== 0 && options.max < this.totalRange().max) {
                    max = length - (1 - options.max % 1);
                } else {
                    max = length - (options.justified ? 1 : 0);
                }
                return {
                    min: min,
                    max: max
                };
            },
            totalRangeIndices: function (limit) {
                var options = this.options;
                var min = isNumber(options.min) ? options.min : 0;
                var max;
                if (isNumber(options.max)) {
                    max = options.max;
                } else if (isNumber(options.min)) {
                    max = min + options.categories.length;
                } else {
                    max = (options.srcCategories || options.categories).length - (options.justified ? 1 : 0) || 1;
                }
                if (limit) {
                    var totalRange = this.totalRange();
                    min = limitValue(min, 0, totalRange.max);
                    max = limitValue(max, 0, totalRange.max);
                }
                return {
                    min: min,
                    max: max
                };
            },
            range: function () {
                var options = this.options;
                return {
                    min: isNumber(options.min) ? options.min : 0,
                    max: isNumber(options.max) ? options.max : options.categories.length
                };
            },
            totalRange: function () {
                var options = this.options;
                return {
                    min: 0,
                    max: Math.max(this._seriesMax || 0, (options.srcCategories || options.categories).length) - (options.justified ? 1 : 0)
                };
            },
            getScale: function () {
                var ref = this.rangeIndices();
                var min = ref.min;
                var max = ref.max;
                var lineBox = this.lineBox();
                var size = this.options.vertical ? lineBox.height() : lineBox.width();
                var scale = size / (max - min || 1);
                return scale * (this.options.reverse ? -1 : 1);
            },
            getTickPositions: function (stepSize) {
                var ref = this.options;
                var vertical = ref.vertical;
                var reverse = ref.reverse;
                var ref$1 = this.rangeIndices();
                var min = ref$1.min;
                var max = ref$1.max;
                var lineBox = this.lineBox();
                var scale = this.getScale();
                var pos = lineBox[(vertical ? Y : X) + (reverse ? 2 : 1)];
                var positions = [];
                var current = min % 1 !== 0 ? Math.floor(min / 1) + stepSize : min;
                while (current <= max) {
                    positions.push(pos + round(scale * (current - min), COORD_PRECISION));
                    current += stepSize;
                }
                return positions;
            },
            getLabelsTickPositions: function () {
                var tickPositions = this.getMajorTickPositions().slice(0);
                var range = this.rangeIndices();
                var scale = this.getScale();
                var box = this.lineBox();
                var options = this.options;
                var axis = options.vertical ? Y : X;
                var start = options.reverse ? 2 : 1;
                var end = options.reverse ? 1 : 2;
                if (range.min % 1 !== 0) {
                    tickPositions.unshift(box[axis + start] - scale * (range.min % 1));
                }
                if (range.max % 1 !== 0) {
                    tickPositions.push(box[axis + end] + scale * (1 - range.max % 1));
                }
                return tickPositions;
            },
            labelTickIndex: function (label) {
                var range = this.rangeIndices();
                var index = label.index;
                if (range.min > 0) {
                    index = index - Math.floor(range.min);
                }
                return index;
            },
            arrangeLabels: function () {
                Axis.fn.arrangeLabels.call(this);
                this.hideOutOfRangeLabels();
            },
            hideOutOfRangeLabels: function () {
                var ref = this;
                var box = ref.box;
                var labels = ref.labels;
                if (labels.length) {
                    var valueAxis = this.options.vertical ? Y : X;
                    var start = box[valueAxis + 1];
                    var end = box[valueAxis + 2];
                    var firstLabel = labels[0];
                    var lastLabel = last(labels);
                    if (firstLabel.box[valueAxis + 1] > end || firstLabel.box[valueAxis + 2] < start) {
                        firstLabel.options.visible = false;
                    }
                    if (lastLabel.box[valueAxis + 1] > end || lastLabel.box[valueAxis + 2] < start) {
                        lastLabel.options.visible = false;
                    }
                }
            },
            getMajorTickPositions: function () {
                return this.getTicks().majorTicks;
            },
            getMinorTickPositions: function () {
                return this.getTicks().minorTicks;
            },
            getTicks: function () {
                var ref = this.options;
                var reverse = ref.reverse;
                var justified = ref.justified;
                var cache = this._ticks;
                var range = this.rangeIndices();
                var lineBox = this.lineBox();
                var hash = lineBox.getHash() + range.min + ',' + range.max + reverse + justified;
                if (cache._hash !== hash) {
                    cache._hash = hash;
                    cache.majorTicks = this.getTickPositions(1);
                    cache.minorTicks = this.getTickPositions(0.5);
                }
                return cache;
            },
            getSlot: function (from, to, limit) {
                var options = this.options;
                var reverse = options.reverse;
                var justified = options.justified;
                var vertical = options.vertical;
                var ref = this.rangeIndices();
                var min = ref.min;
                var valueAxis = vertical ? Y : X;
                var lineBox = this.lineBox();
                var scale = this.getScale();
                var lineStart = lineBox[valueAxis + (reverse ? 2 : 1)];
                var slotBox = lineBox.clone();
                var singleSlot = !defined(to);
                var start = valueOrDefault(from, 0);
                var end = valueOrDefault(to, start);
                end = Math.max(end - 1, start);
                end = Math.max(start, end);
                var p1 = lineStart + (start - min) * scale;
                var p2 = lineStart + (end + 1 - min) * scale;
                if (singleSlot && justified) {
                    p2 = p1;
                }
                if (limit) {
                    p1 = limitValue(p1, lineBox[valueAxis + 1], lineBox[valueAxis + 2]);
                    p2 = limitValue(p2, lineBox[valueAxis + 1], lineBox[valueAxis + 2]);
                }
                slotBox[valueAxis + 1] = reverse ? p2 : p1;
                slotBox[valueAxis + 2] = reverse ? p1 : p2;
                return slotBox;
            },
            limitSlot: function (slot) {
                var vertical = this.options.vertical;
                var valueAxis = vertical ? Y : X;
                var lineBox = this.lineBox();
                var limittedSlot = slot.clone();
                limittedSlot[valueAxis + 1] = limitValue(slot[valueAxis + 1], lineBox[valueAxis + 1], lineBox[valueAxis + 2]);
                limittedSlot[valueAxis + 2] = limitValue(slot[valueAxis + 2], lineBox[valueAxis + 1], lineBox[valueAxis + 2]);
                return limittedSlot;
            },
            slot: function (from, to, limit) {
                var start = from;
                var end = to;
                if (typeof start === 'string') {
                    start = this.categoryIndex(start);
                }
                if (typeof end === 'string') {
                    end = this.categoryIndex(end);
                }
                return Axis.fn.slot.call(this, start, end, limit);
            },
            pointCategoryIndex: function (point) {
                var ref = this.options;
                var reverse = ref.reverse;
                var justified = ref.justified;
                var vertical = ref.vertical;
                var valueAxis = vertical ? Y : X;
                var lineBox = this.lineBox();
                var range = this.rangeIndices();
                var startValue = reverse ? range.max : range.min;
                var scale = this.getScale();
                var lineStart = lineBox[valueAxis + 1];
                var lineEnd = lineBox[valueAxis + 2];
                var pos = point[valueAxis];
                if (pos < lineStart || pos > lineEnd) {
                    return null;
                }
                var value = startValue + (pos - lineStart) / scale;
                var diff = value % 1;
                if (justified) {
                    value = Math.round(value);
                } else if (diff === 0 && value > 0) {
                    value--;
                }
                return Math.floor(value);
            },
            getCategory: function (point) {
                var index = this.pointCategoryIndex(point);
                if (index === null) {
                    return null;
                }
                return this.options.categories[index];
            },
            categoryIndex: function (value) {
                var options = this.options;
                var index = indexOf(value, options.srcCategories || options.categories);
                return index - Math.floor(options.min || 0);
            },
            translateRange: function (delta) {
                var options = this.options;
                var lineBox = this.lineBox();
                var size = options.vertical ? lineBox.height() : lineBox.width();
                var range = options.categories.length;
                var scale = size / range;
                var offset = round(delta / scale, DEFAULT_PRECISION);
                return {
                    min: offset,
                    max: range + offset
                };
            },
            zoomRange: function (rate) {
                var rangeIndices = this.totalRangeIndices();
                var ref = this.totalRange();
                var totalMin = ref.min;
                var totalMax = ref.max;
                var min = limitValue(rangeIndices.min + rate, totalMin, totalMax);
                var max = limitValue(rangeIndices.max - rate, totalMin, totalMax);
                if (max - min > 0) {
                    return {
                        min: min,
                        max: max
                    };
                }
            },
            scaleRange: function (scale) {
                var range = this.options.categories.length;
                var delta = scale * range;
                return {
                    min: -delta,
                    max: range + delta
                };
            },
            labelsCount: function () {
                var labelsRange = this.labelsRange();
                return labelsRange.max - labelsRange.min;
            },
            labelsRange: function () {
                var options = this.options;
                var justified = options.justified;
                var labelOptions = options.labels;
                var ref = this.totalRangeIndices(true);
                var min = ref.min;
                var max = ref.max;
                var start = Math.floor(min);
                if (!justified) {
                    min = Math.floor(min);
                    max = Math.ceil(max);
                } else {
                    min = Math.ceil(min);
                    max = Math.floor(max);
                }
                var skip;
                if (min > labelOptions.skip) {
                    skip = labelOptions.skip + labelOptions.step * Math.ceil((min - labelOptions.skip) / labelOptions.step);
                } else {
                    skip = labelOptions.skip;
                }
                return {
                    min: skip - start,
                    max: (options.categories.length ? max + (justified ? 1 : 0) : 0) - start
                };
            },
            createAxisLabel: function (index, labelOptions) {
                var options = this.options;
                var dataItem = options.dataItems ? options.dataItems[index] : null;
                var category = valueOrDefault(options.categories[index], '');
                var text = this.axisLabelText(category, dataItem, labelOptions);
                return new AxisLabel(category, text, index, dataItem, labelOptions);
            },
            shouldRenderNote: function (value) {
                var range = this.totalRangeIndices();
                return Math.floor(range.min) <= value && value <= Math.ceil(range.max);
            },
            noteSlot: function (value) {
                var options = this.options;
                var index = value - Math.floor(options.min || 0);
                return this.getSlot(index);
            },
            arrangeNotes: function () {
                Axis.fn.arrangeNotes.call(this);
                this.hideOutOfRangeNotes();
            },
            hideOutOfRangeNotes: function () {
                var ref = this;
                var notes = ref.notes;
                var box = ref.box;
                if (notes && notes.length) {
                    var valueAxis = this.options.vertical ? Y : X;
                    var start = box[valueAxis + 1];
                    var end = box[valueAxis + 2];
                    for (var idx = 0; idx < notes.length; idx++) {
                        var note = notes[idx];
                        if (note.box && (end < note.box[valueAxis + 1] || note.box[valueAxis + 2] < start)) {
                            note.hide();
                        }
                    }
                }
            },
            pan: function (delta) {
                var range = this.totalRangeIndices(true);
                var scale = this.getScale();
                var offset = round(delta / scale, DEFAULT_PRECISION);
                var totalRange = this.totalRange();
                var min = range.min + offset;
                var max = range.max + offset;
                return this.limitRange(min, max, 0, totalRange.max, offset);
            },
            pointsRange: function (start, end) {
                var ref = this.options;
                var reverse = ref.reverse;
                var vertical = ref.vertical;
                var valueAxis = vertical ? Y : X;
                var lineBox = this.lineBox();
                var range = this.totalRangeIndices(true);
                var scale = this.getScale();
                var lineStart = lineBox[valueAxis + (reverse ? 2 : 1)];
                var diffStart = start[valueAxis] - lineStart;
                var diffEnd = end[valueAxis] - lineStart;
                var min = range.min + diffStart / scale;
                var max = range.min + diffEnd / scale;
                var rangeMin = Math.min(min, max);
                var rangeMax = Math.max(min, max);
                if (rangeMax - rangeMin >= MIN_CATEGORY_POINTS_RANGE) {
                    return {
                        min: rangeMin,
                        max: rangeMax
                    };
                }
            },
            valueRange: function () {
                return this.range();
            }
        });
        setDefaultOptions(CategoryAxis, {
            type: 'category',
            categories: [],
            vertical: false,
            majorGridLines: {
                visible: false,
                width: 1,
                color: BLACK
            },
            labels: { zIndex: 1 },
            justified: false,
            _deferLabels: true
        });
        var COORDINATE_LIMIT = 300000;
        var DateLabelFormats = {
            milliseconds: 'HH:mm:ss.fff',
            seconds: 'HH:mm:ss',
            minutes: 'HH:mm',
            hours: 'HH:mm',
            days: 'M/d',
            weeks: 'M/d',
            months: 'MMM \'yy',
            years: 'yyyy'
        };
        var ZERO_THRESHOLD = 0.2;
        var AUTO = 'auto';
        var BASE_UNITS = [
            MILLISECONDS,
            SECONDS,
            MINUTES,
            HOURS,
            DAYS,
            WEEKS,
            MONTHS,
            YEARS
        ];
        var FIT = 'fit';
        var DateCategoryAxis = CategoryAxis.extend({
            init: function (axisOptions, chartService) {
                CategoryAxis.fn.init.call(this, axisOptions, chartService);
                var intlService = chartService.intl;
                var options = this.options;
                options = deepExtend({ roundToBaseUnit: true }, options, {
                    categories: parseDates(intlService, options.categories),
                    min: parseDate(intlService, options.min),
                    max: parseDate(intlService, options.max)
                });
                options.userSetBaseUnit = options.userSetBaseUnit || options.baseUnit;
                options.userSetBaseUnitStep = options.userSetBaseUnitStep || options.baseUnitStep;
                if (options.categories && options.categories.length > 0) {
                    var baseUnit = (options.baseUnit || '').toLowerCase();
                    var useDefault = baseUnit !== FIT && !inArray(baseUnit, BASE_UNITS);
                    if (useDefault) {
                        options.baseUnit = this.defaultBaseUnit(options);
                    }
                    if (baseUnit === FIT || options.baseUnitStep === AUTO) {
                        this.autoBaseUnit(options);
                    }
                    this._groupsStart = addDuration(options.categories[0], 0, options.baseUnit, options.weekStartDay);
                    this.groupCategories(options);
                } else {
                    options.baseUnit = options.baseUnit || DAYS;
                }
                this.options = options;
            },
            _initCategories: function () {
            },
            shouldRenderNote: function (value) {
                var range = this.range();
                var categories = this.options.categories || [];
                return dateComparer(value, range.min) >= 0 && dateComparer(value, range.max) <= 0 && categories.length;
            },
            parseNoteValue: function (value) {
                return parseDate(this.chartService.intl, value);
            },
            noteSlot: function (value) {
                return this.getSlot(value);
            },
            translateRange: function (delta) {
                var options = this.options;
                var baseUnit = options.baseUnit;
                var weekStartDay = options.weekStartDay;
                var vertical = options.vertical;
                var lineBox = this.lineBox();
                var size = vertical ? lineBox.height() : lineBox.width();
                var range = this.range();
                var scale = size / (range.max - range.min);
                var offset = round(delta / scale, DEFAULT_PRECISION);
                if (range.min && range.max) {
                    var from = addTicks(options.min || range.min, offset);
                    var to = addTicks(options.max || range.max, offset);
                    range = {
                        min: addDuration(from, 0, baseUnit, weekStartDay),
                        max: addDuration(to, 0, baseUnit, weekStartDay)
                    };
                }
                return range;
            },
            scaleRange: function (delta) {
                var rounds = Math.abs(delta);
                var result = this.range();
                var from = result.min;
                var to = result.max;
                if (from && to) {
                    while (rounds--) {
                        var range = dateDiff(from, to);
                        var step = Math.round(range * 0.1);
                        if (delta < 0) {
                            from = addTicks(from, step);
                            to = addTicks(to, -step);
                        } else {
                            from = addTicks(from, -step);
                            to = addTicks(to, step);
                        }
                    }
                    result = {
                        min: from,
                        max: to
                    };
                }
                return result;
            },
            defaultBaseUnit: function (options) {
                var categories = options.categories;
                var count = defined(categories) ? categories.length : 0;
                var minDiff = MAX_VALUE;
                var lastCategory, unit;
                for (var categoryIx = 0; categoryIx < count; categoryIx++) {
                    var category = categories[categoryIx];
                    if (category && lastCategory) {
                        var diff = absoluteDateDiff(category, lastCategory);
                        if (diff > 0) {
                            minDiff = Math.min(minDiff, diff);
                            if (minDiff >= TIME_PER_YEAR) {
                                unit = YEARS;
                            } else if (minDiff >= TIME_PER_MONTH - TIME_PER_DAY * 3) {
                                unit = MONTHS;
                            } else if (minDiff >= TIME_PER_WEEK) {
                                unit = WEEKS;
                            } else if (minDiff >= TIME_PER_DAY) {
                                unit = DAYS;
                            } else if (minDiff >= TIME_PER_HOUR) {
                                unit = HOURS;
                            } else if (minDiff >= TIME_PER_MINUTE) {
                                unit = MINUTES;
                            } else {
                                unit = SECONDS;
                            }
                        }
                    }
                    lastCategory = category;
                }
                return unit || DAYS;
            },
            _categoryRange: function (categories) {
                var range = categories._range;
                if (!range) {
                    range = categories._range = sparseArrayLimits(categories);
                }
                return range;
            },
            totalRange: function () {
                return {
                    min: 0,
                    max: this.options.categories.length
                };
            },
            rangeIndices: function () {
                var options = this.options;
                var categories = options.categories;
                var baseUnit = options.baseUnit;
                var baseUnitStep = options.baseUnitStep || 1;
                var categoryLimits = this.categoriesRange();
                var min = toDate(options.min || categoryLimits.min);
                var max = toDate(options.max || categoryLimits.max);
                var minIdx = 0, maxIdx = 0;
                if (categories.length) {
                    minIdx = dateIndex(min, categories[0], baseUnit, baseUnitStep);
                    maxIdx = dateIndex(max, categories[0], baseUnit, baseUnitStep);
                    if (options.roundToBaseUnit) {
                        minIdx = Math.floor(minIdx);
                        maxIdx = options.justified ? Math.floor(maxIdx) : Math.ceil(maxIdx);
                    }
                }
                return {
                    min: minIdx,
                    max: maxIdx
                };
            },
            labelsRange: function () {
                var options = this.options;
                var labelOptions = options.labels;
                var range = this.rangeIndices();
                var min = Math.floor(range.min);
                var max = Math.ceil(range.max);
                return {
                    min: min + labelOptions.skip,
                    max: options.categories.length ? max + (options.justified ? 1 : 0) : 0
                };
            },
            categoriesRange: function () {
                var options = this.options;
                var range = this._categoryRange(options.srcCategories || options.categories);
                var max = toDate(range.max);
                if (!options.justified && dateEquals(max, this._roundToTotalStep(max, options, false))) {
                    max = this._roundToTotalStep(max, options, true, true);
                }
                return {
                    min: toDate(range.min),
                    max: max
                };
            },
            currentRange: function () {
                var options = this.options;
                var round$$1 = options.roundToBaseUnit !== false;
                var totalRange = this.categoriesRange();
                var min = options.min;
                var max = options.max;
                if (!min) {
                    min = round$$1 ? this._roundToTotalStep(totalRange.min, options, false) : totalRange.min;
                }
                if (!max) {
                    max = round$$1 ? this._roundToTotalStep(totalRange.max, options, !options.justified) : totalRange.max;
                }
                return {
                    min: min,
                    max: max
                };
            },
            datesRange: function () {
                var range = this._categoryRange(this.options.srcCategories || this.options.categories);
                return {
                    min: toDate(range.min),
                    max: toDate(range.max)
                };
            },
            pan: function (delta) {
                var options = this.options;
                var lineBox = this.lineBox();
                var size = options.vertical ? lineBox.height() : lineBox.width();
                var ref = this.currentRange();
                var min = ref.min;
                var max = ref.max;
                var totalLimits = this.totalLimits();
                var scale = size / (max - min);
                var offset = round(delta / scale, DEFAULT_PRECISION);
                var from = addTicks(min, offset);
                var to = addTicks(max, offset);
                var panRange = this.limitRange(toTime(from), toTime(to), toTime(totalLimits.min), toTime(totalLimits.max), offset);
                if (panRange) {
                    panRange.min = toDate(panRange.min);
                    panRange.max = toDate(panRange.max);
                    panRange.baseUnit = options.baseUnit;
                    panRange.baseUnitStep = options.baseUnitStep || 1;
                    panRange.userSetBaseUnit = options.userSetBaseUnit;
                    panRange.userSetBaseUnitStep = options.userSetBaseUnitStep;
                    return panRange;
                }
            },
            pointsRange: function (start, end) {
                var pointsRange = CategoryAxis.fn.pointsRange.call(this, start, end);
                var datesRange = this.currentRange();
                var indicesRange = this.rangeIndices();
                var scale = dateDiff(datesRange.max, datesRange.min) / (indicesRange.max - indicesRange.min);
                var options = this.options;
                var min = addTicks(datesRange.min, pointsRange.min * scale);
                var max = addTicks(datesRange.min, pointsRange.max * scale);
                return {
                    min: min,
                    max: max,
                    baseUnit: options.userSetBaseUnit,
                    baseUnitStep: options.userSetBaseUnitStep
                };
            },
            zoomRange: function (delta) {
                var options = this.options;
                var totalLimits = this.totalLimits();
                var weekStartDay = options.weekStartDay;
                var baseUnit = options.baseUnit;
                var baseUnitStep = options.baseUnitStep || 1;
                var ref = this.currentRange();
                var rangeMin = ref.min;
                var rangeMax = ref.max;
                var min = addDuration(rangeMin, delta * baseUnitStep, baseUnit, weekStartDay);
                var max = addDuration(rangeMax, -delta * baseUnitStep, baseUnit, weekStartDay);
                if (options.userSetBaseUnit === FIT) {
                    var autoBaseUnitSteps = options.autoBaseUnitSteps;
                    var maxDateGroups = options.maxDateGroups;
                    var maxDiff = last(autoBaseUnitSteps[baseUnit]) * maxDateGroups * TIME_PER_UNIT[baseUnit];
                    var rangeDiff = dateDiff(rangeMax, rangeMin);
                    var diff = dateDiff(max, min);
                    var baseUnitIndex = BASE_UNITS.indexOf(baseUnit);
                    var autoBaseUnitStep, ticks;
                    if (diff < TIME_PER_UNIT[baseUnit] && baseUnit !== MILLISECONDS) {
                        baseUnit = BASE_UNITS[baseUnitIndex - 1];
                        autoBaseUnitStep = last(autoBaseUnitSteps[baseUnit]);
                        ticks = (rangeDiff - (maxDateGroups - 1) * autoBaseUnitStep * TIME_PER_UNIT[baseUnit]) / 2;
                        min = addTicks(rangeMin, ticks);
                        max = addTicks(rangeMax, -ticks);
                    } else if (diff > maxDiff && baseUnit !== YEARS) {
                        var stepIndex = 0;
                        do {
                            baseUnitIndex++;
                            baseUnit = BASE_UNITS[baseUnitIndex];
                            stepIndex = 0;
                            ticks = 2 * TIME_PER_UNIT[baseUnit];
                            do {
                                autoBaseUnitStep = autoBaseUnitSteps[baseUnit][stepIndex];
                                stepIndex++;
                            } while (stepIndex < autoBaseUnitSteps[baseUnit].length && ticks * autoBaseUnitStep < rangeDiff);
                        } while (baseUnit !== YEARS && ticks * autoBaseUnitStep < rangeDiff);
                        ticks = (ticks * autoBaseUnitStep - rangeDiff) / 2;
                        if (ticks > 0) {
                            min = addTicks(rangeMin, -ticks);
                            max = addTicks(rangeMax, ticks);
                            min = addTicks(min, limitValue(max, totalLimits.min, totalLimits.max) - max);
                            max = addTicks(max, limitValue(min, totalLimits.min, totalLimits.max) - min);
                        }
                    }
                }
                min = toDate(limitValue(min, totalLimits.min, totalLimits.max));
                max = toDate(limitValue(max, totalLimits.min, totalLimits.max));
                if (min && max && dateDiff(max, min) > 0) {
                    return {
                        min: min,
                        max: max,
                        baseUnit: options.userSetBaseUnit,
                        baseUnitStep: options.userSetBaseUnitStep
                    };
                }
            },
            totalLimits: function () {
                var options = this.options;
                var datesRange = this.datesRange();
                var min = this._roundToTotalStep(toDate(datesRange.min), options, false);
                var max = datesRange.max;
                if (!options.justified) {
                    max = this._roundToTotalStep(max, options, true, dateEquals(max, this._roundToTotalStep(max, options, false)));
                }
                return {
                    min: min,
                    max: max
                };
            },
            range: function (rangeOptions) {
                var options = rangeOptions || this.options;
                var categories = options.categories;
                var autoUnit = options.baseUnit === FIT;
                var baseUnit = autoUnit ? BASE_UNITS[0] : options.baseUnit;
                var baseUnitStep = options.baseUnitStep || 1;
                var stepOptions = {
                    baseUnit: baseUnit,
                    baseUnitStep: baseUnitStep,
                    weekStartDay: options.weekStartDay
                };
                var categoryLimits = this._categoryRange(categories);
                var min = toDate(options.min || categoryLimits.min);
                var max = toDate(options.max || categoryLimits.max);
                return {
                    min: this._roundToTotalStep(min, stepOptions, false),
                    max: this._roundToTotalStep(max, stepOptions, true, true)
                };
            },
            autoBaseUnit: function (options) {
                var categoryLimits = this._categoryRange(options.categories);
                var span = toDate(options.max || categoryLimits.max) - toDate(options.min || categoryLimits.min);
                var maxDateGroups = options.maxDateGroups || this.options.maxDateGroups;
                var autoUnit = options.baseUnit === FIT;
                var autoUnitIx = 0;
                var baseUnit = autoUnit ? BASE_UNITS[autoUnitIx++] : options.baseUnit;
                var units = span / TIME_PER_UNIT[baseUnit];
                var totalUnits = units;
                var autoBaseUnitSteps = deepExtend({}, this.options.autoBaseUnitSteps, options.autoBaseUnitSteps);
                var unitSteps, step, nextStep;
                while (!step || units >= maxDateGroups) {
                    unitSteps = unitSteps || autoBaseUnitSteps[baseUnit].slice(0);
                    nextStep = unitSteps.shift();
                    if (nextStep) {
                        step = nextStep;
                        units = totalUnits / step;
                    } else if (baseUnit === last(BASE_UNITS)) {
                        step = Math.ceil(totalUnits / maxDateGroups);
                        break;
                    } else if (autoUnit) {
                        baseUnit = BASE_UNITS[autoUnitIx++] || last(BASE_UNITS);
                        totalUnits = span / TIME_PER_UNIT[baseUnit];
                        unitSteps = null;
                    } else {
                        if (units > maxDateGroups) {
                            step = Math.ceil(totalUnits / maxDateGroups);
                        }
                        break;
                    }
                }
                options.baseUnitStep = step;
                options.baseUnit = baseUnit;
            },
            groupCategories: function (options) {
                var categories = options.categories;
                var baseUnit = options.baseUnit;
                var baseUnitStep = options.baseUnitStep || 1;
                var maxCategory = toDate(sparseArrayLimits(categories).max);
                var ref = this.range(options);
                var min = ref.min;
                var max = ref.max;
                var groups = [];
                var nextDate;
                for (var date = min; date < max; date = nextDate) {
                    groups.push(date);
                    nextDate = addDuration(date, baseUnitStep, baseUnit, options.weekStartDay);
                    if (nextDate > maxCategory && !options.max) {
                        break;
                    }
                }
                options.srcCategories = categories;
                options.categories = groups;
            },
            _roundToTotalStep: function (value, axisOptions, upper, roundToNext) {
                var options = axisOptions || this.options;
                var baseUnit = options.baseUnit;
                var baseUnitStep = options.baseUnitStep || 1;
                var start = this._groupsStart;
                if (start) {
                    var step = dateIndex(value, start, baseUnit, baseUnitStep);
                    var roundedStep = upper ? Math.ceil(step) : Math.floor(step);
                    if (roundToNext) {
                        roundedStep++;
                    }
                    return addDuration(start, roundedStep * baseUnitStep, baseUnit, options.weekStartDay);
                }
                return addDuration(value, upper ? baseUnitStep : 0, baseUnit, options.weekStartDay);
            },
            createAxisLabel: function (index, labelOptions) {
                var options = this.options;
                var dataItem = options.dataItems ? options.dataItems[index] : null;
                var date = options.categories[index];
                var baseUnit = options.baseUnit;
                var unitFormat = labelOptions.dateFormats[baseUnit];
                var visible = true;
                if (options.justified) {
                    var roundedDate = floorDate(date, baseUnit, options.weekStartDay);
                    visible = dateEquals(roundedDate, date);
                } else if (!options.roundToBaseUnit) {
                    visible = !dateEquals(this.range().max, date);
                }
                if (visible) {
                    labelOptions.format = labelOptions.format || unitFormat;
                    var text = this.axisLabelText(date, dataItem, labelOptions);
                    if (text) {
                        return new AxisLabel(date, text, index, dataItem, labelOptions);
                    }
                }
            },
            categoryIndex: function (value) {
                var options = this.options;
                var categories = options.categories;
                var index = -1;
                if (categories.length) {
                    index = Math.floor(dateIndex(toDate(value), categories[0], options.baseUnit, options.baseUnitStep || 1));
                }
                return index;
            },
            getSlot: function (a, b, limit) {
                var start = a;
                var end = b;
                if (typeof start === OBJECT) {
                    start = this.categoryIndex(start);
                }
                if (typeof end === OBJECT) {
                    end = this.categoryIndex(end);
                }
                return CategoryAxis.fn.getSlot.call(this, start, end, limit);
            },
            valueRange: function () {
                var options = this.options;
                var range = this._categoryRange(options.srcCategories || options.categories);
                return {
                    min: toDate(range.min),
                    max: toDate(range.max)
                };
            }
        });
        setDefaultOptions(DateCategoryAxis, {
            type: DATE,
            labels: { dateFormats: DateLabelFormats },
            autoBaseUnitSteps: {
                milliseconds: [
                    1,
                    10,
                    100
                ],
                seconds: [
                    1,
                    2,
                    5,
                    15,
                    30
                ],
                minutes: [
                    1,
                    2,
                    5,
                    15,
                    30
                ],
                hours: [
                    1,
                    2,
                    3
                ],
                days: [
                    1,
                    2,
                    3
                ],
                weeks: [
                    1,
                    2
                ],
                months: [
                    1,
                    2,
                    3,
                    6
                ],
                years: [
                    1,
                    2,
                    3,
                    5,
                    10,
                    25,
                    50
                ]
            },
            maxDateGroups: 10
        });
        function autoMajorUnit(min, max) {
            var diff = round(max - min, DEFAULT_PRECISION - 1);
            if (diff === 0) {
                if (max === 0) {
                    return 0.1;
                }
                diff = Math.abs(max);
            }
            var scale = Math.pow(10, Math.floor(Math.log(diff) / Math.log(10)));
            var relativeValue = round(diff / scale, DEFAULT_PRECISION);
            var scaleMultiplier = 1;
            if (relativeValue < 1.904762) {
                scaleMultiplier = 0.2;
            } else if (relativeValue < 4.761904) {
                scaleMultiplier = 0.5;
            } else if (relativeValue < 9.523809) {
                scaleMultiplier = 1;
            } else {
                scaleMultiplier = 2;
            }
            return round(scale * scaleMultiplier, DEFAULT_PRECISION);
        }
        function autoAxisMin(min, max, narrow) {
            if (!min && !max) {
                return 0;
            }
            var axisMin;
            if (min >= 0 && max >= 0) {
                var minValue = min === max ? 0 : min;
                var diff = (max - minValue) / max;
                if (narrow === false || !narrow && diff > ZERO_THRESHOLD) {
                    return 0;
                }
                axisMin = Math.max(0, minValue - (max - minValue) / 2);
            } else {
                axisMin = min;
            }
            return axisMin;
        }
        function autoAxisMax(min, max, narrow) {
            if (!min && !max) {
                return 1;
            }
            var axisMax;
            if (min <= 0 && max <= 0) {
                var maxValue = min === max ? 0 : max;
                var diff = Math.abs((maxValue - min) / maxValue);
                if (narrow === false || !narrow && diff > ZERO_THRESHOLD) {
                    return 0;
                }
                axisMax = Math.min(0, maxValue - (min - maxValue) / 2);
            } else {
                axisMax = max;
            }
            return axisMax;
        }
        function floor(value, step) {
            return round(Math.floor(value / step) * step, DEFAULT_PRECISION);
        }
        function ceil(value, step) {
            return round(Math.ceil(value / step) * step, DEFAULT_PRECISION);
        }
        function limitCoordinate(value) {
            return Math.max(Math.min(value, COORDINATE_LIMIT), -COORDINATE_LIMIT);
        }
        var MIN_VALUE_RANGE = Math.pow(10, -DEFAULT_PRECISION + 1);
        var NumericAxis = Axis.extend({
            init: function (seriesMin, seriesMax, options, chartService) {
                var autoOptions = autoAxisOptions(seriesMin, seriesMax, options);
                var totalOptions = totalAxisOptions(autoOptions, options);
                Axis.fn.init.call(this, axisOptions(autoOptions, options), chartService);
                this.totalMin = totalOptions.min;
                this.totalMax = totalOptions.max;
                this.totalMajorUnit = totalOptions.majorUnit;
                this.seriesMin = seriesMin;
                this.seriesMax = seriesMax;
            },
            startValue: function () {
                return 0;
            },
            range: function () {
                var options = this.options;
                return {
                    min: options.min,
                    max: options.max
                };
            },
            getDivisions: function (stepValue) {
                if (stepValue === 0) {
                    return 1;
                }
                var options = this.options;
                var range = options.max - options.min;
                return Math.floor(round(range / stepValue, COORD_PRECISION)) + 1;
            },
            getTickPositions: function (unit, skipUnit) {
                var options = this.options;
                var vertical = options.vertical;
                var reverse = options.reverse;
                var lineBox = this.lineBox();
                var lineSize = vertical ? lineBox.height() : lineBox.width();
                var range = options.max - options.min;
                var scale = lineSize / range;
                var step = unit * scale;
                var divisions = this.getDivisions(unit);
                var dir = (vertical ? -1 : 1) * (reverse ? -1 : 1);
                var startEdge = dir === 1 ? 1 : 2;
                var positions = [];
                var pos = lineBox[(vertical ? Y : X) + startEdge];
                var skipStep = 0;
                if (skipUnit) {
                    skipStep = skipUnit / unit;
                }
                for (var idx = 0; idx < divisions; idx++) {
                    if (idx % skipStep !== 0) {
                        positions.push(round(pos, COORD_PRECISION));
                    }
                    pos = pos + step * dir;
                }
                return positions;
            },
            getMajorTickPositions: function () {
                return this.getTickPositions(this.options.majorUnit);
            },
            getMinorTickPositions: function () {
                return this.getTickPositions(this.options.minorUnit);
            },
            getSlot: function (a, b, limit) {
                if (limit === void 0) {
                    limit = false;
                }
                var options = this.options;
                var vertical = options.vertical;
                var reverse = options.reverse;
                var valueAxis = vertical ? Y : X;
                var lineBox = this.lineBox();
                var lineStart = lineBox[valueAxis + (reverse ? 2 : 1)];
                var lineSize = vertical ? lineBox.height() : lineBox.width();
                var dir = reverse ? -1 : 1;
                var step = dir * (lineSize / (options.max - options.min));
                var slotBox = new Box(lineBox.x1, lineBox.y1, lineBox.x1, lineBox.y1);
                var start = a;
                var end = b;
                if (!defined(start)) {
                    start = end || 0;
                }
                if (!defined(end)) {
                    end = start || 0;
                }
                if (limit) {
                    start = Math.max(Math.min(start, options.max), options.min);
                    end = Math.max(Math.min(end, options.max), options.min);
                }
                var p1, p2;
                if (vertical) {
                    p1 = options.max - Math.max(start, end);
                    p2 = options.max - Math.min(start, end);
                } else {
                    p1 = Math.min(start, end) - options.min;
                    p2 = Math.max(start, end) - options.min;
                }
                slotBox[valueAxis + 1] = limitCoordinate(lineStart + step * (reverse ? p2 : p1));
                slotBox[valueAxis + 2] = limitCoordinate(lineStart + step * (reverse ? p1 : p2));
                return slotBox;
            },
            getValue: function (point) {
                var options = this.options;
                var vertical = options.vertical;
                var reverse = options.reverse;
                var max = Number(options.max);
                var min = Number(options.min);
                var valueAxis = vertical ? Y : X;
                var lineBox = this.lineBox();
                var lineStart = lineBox[valueAxis + (reverse ? 2 : 1)];
                var lineSize = vertical ? lineBox.height() : lineBox.width();
                var dir = reverse ? -1 : 1;
                var offset = dir * (point[valueAxis] - lineStart);
                var step = (max - min) / lineSize;
                var valueOffset = offset * step;
                if (offset < 0 || offset > lineSize) {
                    return null;
                }
                var value = vertical ? max - valueOffset : min + valueOffset;
                return round(value, DEFAULT_PRECISION);
            },
            translateRange: function (delta) {
                var options = this.options;
                var vertical = options.vertical;
                var reverse = options.reverse;
                var max = options.max;
                var min = options.min;
                var lineBox = this.lineBox();
                var size = vertical ? lineBox.height() : lineBox.width();
                var range = max - min;
                var scale = size / range;
                var offset = round(delta / scale, DEFAULT_PRECISION);
                if ((vertical || reverse) && !(vertical && reverse)) {
                    offset = -offset;
                }
                return {
                    min: min + offset,
                    max: max + offset
                };
            },
            scaleRange: function (delta) {
                var options = this.options;
                var offset = -delta * options.majorUnit;
                return {
                    min: options.min - offset,
                    max: options.max + offset
                };
            },
            labelsCount: function () {
                return this.getDivisions(this.options.majorUnit);
            },
            createAxisLabel: function (index, labelOptions) {
                var options = this.options;
                var value = round(options.min + index * options.majorUnit, DEFAULT_PRECISION);
                var text = this.axisLabelText(value, null, labelOptions);
                return new AxisLabel(value, text, index, null, labelOptions);
            },
            shouldRenderNote: function (value) {
                var range = this.range();
                return range.min <= value && value <= range.max;
            },
            pan: function (delta) {
                var range = this.translateRange(delta);
                return this.limitRange(range.min, range.max, this.totalMin, this.totalMax);
            },
            pointsRange: function (start, end) {
                var startValue = this.getValue(start);
                var endValue = this.getValue(end);
                var min = Math.min(startValue, endValue);
                var max = Math.max(startValue, endValue);
                if (this.isValidRange(min, max)) {
                    return {
                        min: min,
                        max: max
                    };
                }
            },
            zoomRange: function (delta) {
                var ref = this;
                var totalMin = ref.totalMin;
                var totalMax = ref.totalMax;
                var newRange = this.scaleRange(delta);
                var min = limitValue(newRange.min, totalMin, totalMax);
                var max = limitValue(newRange.max, totalMin, totalMax);
                if (this.isValidRange(min, max)) {
                    return {
                        min: min,
                        max: max
                    };
                }
            },
            isValidRange: function (min, max) {
                return max - min > MIN_VALUE_RANGE;
            }
        });
        function autoAxisOptions(seriesMin, seriesMax, options) {
            var narrowRange = options.narrowRange;
            var autoMin = autoAxisMin(seriesMin, seriesMax, narrowRange);
            var autoMax = autoAxisMax(seriesMin, seriesMax, narrowRange);
            var majorUnit = autoMajorUnit(autoMin, autoMax);
            var autoOptions = { majorUnit: majorUnit };
            if (options.roundToMajorUnit !== false) {
                if (autoMin < 0 && remainderClose(autoMin, majorUnit, 1 / 3)) {
                    autoMin -= majorUnit;
                }
                if (autoMax > 0 && remainderClose(autoMax, majorUnit, 1 / 3)) {
                    autoMax += majorUnit;
                }
            }
            autoOptions.min = floor(autoMin, majorUnit);
            autoOptions.max = ceil(autoMax, majorUnit);
            return autoOptions;
        }
        function totalAxisOptions(autoOptions, options) {
            return {
                min: defined(options.min) ? Math.min(autoOptions.min, options.min) : autoOptions.min,
                max: defined(options.max) ? Math.max(autoOptions.max, options.max) : autoOptions.max,
                majorUnit: autoOptions.majorUnit
            };
        }
        function axisOptions(autoOptions, userOptions) {
            var options = userOptions;
            var userSetMin, userSetMax;
            if (userOptions) {
                userSetMin = defined(userOptions.min);
                userSetMax = defined(userOptions.max);
                var userSetLimits = userSetMin || userSetMax;
                if (userSetLimits) {
                    if (userOptions.min === userOptions.max) {
                        if (userOptions.min > 0) {
                            userOptions.min = 0;
                        } else {
                            userOptions.max = 1;
                        }
                    }
                }
                if (userOptions.majorUnit) {
                    autoOptions.min = floor(autoOptions.min, userOptions.majorUnit);
                    autoOptions.max = ceil(autoOptions.max, userOptions.majorUnit);
                } else if (userSetLimits) {
                    options = deepExtend(autoOptions, userOptions);
                    autoOptions.majorUnit = autoMajorUnit(options.min, options.max);
                }
            }
            autoOptions.minorUnit = (options.majorUnit || autoOptions.majorUnit) / 5;
            var result = deepExtend(autoOptions, options);
            if (result.min >= result.max) {
                if (userSetMin && !userSetMax) {
                    result.max = result.min + result.majorUnit;
                } else if (!userSetMin && userSetMax) {
                    result.min = result.max - result.majorUnit;
                }
            }
            return result;
        }
        function remainderClose(value, divisor, ratio) {
            var remainder = round(Math.abs(value % divisor), DEFAULT_PRECISION);
            var threshold = divisor * (1 - ratio);
            return remainder === 0 || remainder > threshold;
        }
        setDefaultOptions(NumericAxis, {
            type: 'numeric',
            min: 0,
            max: 1,
            vertical: true,
            majorGridLines: {
                visible: true,
                width: 1,
                color: BLACK
            },
            labels: { format: '#.####################' },
            zIndex: 1
        });
        var DateValueAxis = Axis.extend({
            init: function (seriesMin, seriesMax, axisOptions, chartService) {
                var min = toDate(seriesMin);
                var max = toDate(seriesMax);
                var intlService = chartService.intl;
                var options = axisOptions || {};
                options = deepExtend(options || {}, {
                    min: parseDate(intlService, options.min),
                    max: parseDate(intlService, options.max),
                    axisCrossingValue: parseDates(intlService, options.axisCrossingValues || options.axisCrossingValue)
                });
                options = applyDefaults(min, max, options);
                Axis.fn.init.call(this, options, chartService);
                this.intlService = intlService;
                this.seriesMin = min;
                this.seriesMax = max;
                this.totalMin = toTime(floorDate(toTime(min) - 1, options.baseUnit));
                this.totalMax = toTime(ceilDate(toTime(max) + 1, options.baseUnit));
            },
            range: function () {
                var options = this.options;
                return {
                    min: options.min,
                    max: options.max
                };
            },
            getDivisions: function (stepValue) {
                var options = this.options;
                return Math.floor(duration(options.min, options.max, options.baseUnit) / stepValue + 1);
            },
            getTickPositions: function (step) {
                var options = this.options;
                var vertical = options.vertical;
                var lineBox = this.lineBox();
                var dir = (vertical ? -1 : 1) * (options.reverse ? -1 : 1);
                var startEdge = dir === 1 ? 1 : 2;
                var start = lineBox[(vertical ? Y : X) + startEdge];
                var divisions = this.getDivisions(step);
                var timeRange = dateDiff(options.max, options.min);
                var lineSize = vertical ? lineBox.height() : lineBox.width();
                var scale = lineSize / timeRange;
                var positions = [start];
                for (var i = 1; i < divisions; i++) {
                    var date = addDuration(options.min, i * step, options.baseUnit);
                    var pos = start + dateDiff(date, options.min) * scale * dir;
                    positions.push(round(pos, COORD_PRECISION));
                }
                return positions;
            },
            getMajorTickPositions: function () {
                return this.getTickPositions(this.options.majorUnit);
            },
            getMinorTickPositions: function () {
                return this.getTickPositions(this.options.minorUnit);
            },
            getSlot: function (a, b, limit) {
                return NumericAxis.prototype.getSlot.call(this, parseDate(this.intlService, a), parseDate(this.intlService, b), limit);
            },
            getValue: function (point) {
                var value = NumericAxis.prototype.getValue.call(this, point);
                return value !== null ? toDate(value) : null;
            },
            labelsCount: function () {
                return this.getDivisions(this.options.majorUnit);
            },
            createAxisLabel: function (index, labelOptions) {
                var options = this.options;
                var offset = index * options.majorUnit;
                var date = options.min;
                if (offset > 0) {
                    date = addDuration(date, offset, options.baseUnit);
                }
                var unitFormat = labelOptions.dateFormats[options.baseUnit];
                labelOptions.format = labelOptions.format || unitFormat;
                var text = this.axisLabelText(date, null, labelOptions);
                return new AxisLabel(date, text, index, null, labelOptions);
            },
            translateRange: function (delta, exact) {
                var options = this.options;
                var baseUnit = options.baseUnit;
                var weekStartDay = options.weekStartDay;
                var lineBox = this.lineBox();
                var size = options.vertical ? lineBox.height() : lineBox.width();
                var range = this.range();
                var scale = size / dateDiff(range.max, range.min);
                var offset = round(delta / scale, DEFAULT_PRECISION);
                var from = addTicks(options.min, offset);
                var to = addTicks(options.max, offset);
                if (!exact) {
                    from = addDuration(from, 0, baseUnit, weekStartDay);
                    to = addDuration(to, 0, baseUnit, weekStartDay);
                }
                return {
                    min: from,
                    max: to
                };
            },
            scaleRange: function (delta) {
                var ref = this.options;
                var from = ref.min;
                var to = ref.max;
                var rounds = Math.abs(delta);
                while (rounds--) {
                    var range = dateDiff(from, to);
                    var step = Math.round(range * 0.1);
                    if (delta < 0) {
                        from = addTicks(from, step);
                        to = addTicks(to, -step);
                    } else {
                        from = addTicks(from, -step);
                        to = addTicks(to, step);
                    }
                }
                return {
                    min: from,
                    max: to
                };
            },
            shouldRenderNote: function (value) {
                var range = this.range();
                return dateComparer(value, range.min) >= 0 && dateComparer(value, range.max) <= 0;
            },
            pan: function (delta) {
                var range = this.translateRange(delta, true);
                var limittedRange = this.limitRange(toTime(range.min), toTime(range.max), this.totalMin, this.totalMax);
                if (limittedRange) {
                    return {
                        min: toDate(limittedRange.min),
                        max: toDate(limittedRange.max)
                    };
                }
            },
            pointsRange: function (start, end) {
                var startValue = this.getValue(start);
                var endValue = this.getValue(end);
                var min = Math.min(startValue, endValue);
                var max = Math.max(startValue, endValue);
                return {
                    min: toDate(min),
                    max: toDate(max)
                };
            },
            zoomRange: function (delta) {
                var range = this.scaleRange(delta);
                var min = toDate(limitValue(toTime(range.min), this.totalMin, this.totalMax));
                var max = toDate(limitValue(toTime(range.max), this.totalMin, this.totalMax));
                return {
                    min: min,
                    max: max
                };
            }
        });
        function timeUnits(delta) {
            var unit = HOURS;
            if (delta >= TIME_PER_YEAR) {
                unit = YEARS;
            } else if (delta >= TIME_PER_MONTH) {
                unit = MONTHS;
            } else if (delta >= TIME_PER_WEEK) {
                unit = WEEKS;
            } else if (delta >= TIME_PER_DAY) {
                unit = DAYS;
            }
            return unit;
        }
        function applyDefaults(seriesMin, seriesMax, options) {
            var min = options.min || seriesMin;
            var max = options.max || seriesMax;
            var baseUnit = options.baseUnit || (max && min ? timeUnits(absoluteDateDiff(max, min)) : HOURS);
            var baseUnitTime = TIME_PER_UNIT[baseUnit];
            var autoMin = floorDate(toTime(min) - 1, baseUnit) || toDate(max);
            var autoMax = ceilDate(toTime(max) + 1, baseUnit);
            var userMajorUnit = options.majorUnit ? options.majorUnit : undefined;
            var majorUnit = userMajorUnit || ceil(autoMajorUnit(autoMin.getTime(), autoMax.getTime()), baseUnitTime) / baseUnitTime;
            var actualUnits = duration(autoMin, autoMax, baseUnit);
            var totalUnits = ceil(actualUnits, majorUnit);
            var unitsToAdd = totalUnits - actualUnits;
            var head = Math.floor(unitsToAdd / 2);
            var tail = unitsToAdd - head;
            if (!options.baseUnit) {
                delete options.baseUnit;
            }
            options.baseUnit = options.baseUnit || baseUnit;
            options.min = options.min || addDuration(autoMin, -head, baseUnit);
            options.max = options.max || addDuration(autoMax, tail, baseUnit);
            options.minorUnit = options.minorUnit || majorUnit / 5;
            options.majorUnit = majorUnit;
            return options;
        }
        setDefaultOptions(DateValueAxis, {
            type: DATE,
            majorGridLines: {
                visible: true,
                width: 1,
                color: BLACK
            },
            labels: { dateFormats: DateLabelFormats }
        });
        var DEFAULT_MAJOR_UNIT = 10;
        var LogarithmicAxis = Axis.extend({
            init: function (seriesMin, seriesMax, options, chartService) {
                var axisOptions = deepExtend({
                    majorUnit: DEFAULT_MAJOR_UNIT,
                    min: seriesMin,
                    max: seriesMax
                }, options);
                var base = axisOptions.majorUnit;
                var autoMax = autoAxisMax$1(seriesMax, base);
                var autoMin = autoAxisMin$1(seriesMin, seriesMax, axisOptions);
                var range = initRange(autoMin, autoMax, axisOptions, options);
                axisOptions.max = range.max;
                axisOptions.min = range.min;
                axisOptions.minorUnit = options.minorUnit || round(base - 1, DEFAULT_PRECISION);
                Axis.fn.init.call(this, axisOptions, chartService);
                this.totalMin = defined(options.min) ? Math.min(autoMin, options.min) : autoMin;
                this.totalMax = defined(options.max) ? Math.max(autoMax, options.max) : autoMax;
                this.logMin = round(log(range.min, base), DEFAULT_PRECISION);
                this.logMax = round(log(range.max, base), DEFAULT_PRECISION);
                this.seriesMin = seriesMin;
                this.seriesMax = seriesMax;
                this.createLabels();
            },
            startValue: function () {
                return this.options.min;
            },
            getSlot: function (a, b, limit) {
                var ref = this;
                var options = ref.options;
                var logMin = ref.logMin;
                var logMax = ref.logMax;
                var reverse = options.reverse;
                var vertical = options.vertical;
                var base = options.majorUnit;
                var valueAxis = vertical ? Y : X;
                var lineBox = this.lineBox();
                var lineStart = lineBox[valueAxis + (reverse ? 2 : 1)];
                var lineSize = vertical ? lineBox.height() : lineBox.width();
                var dir = reverse ? -1 : 1;
                var step = dir * (lineSize / (logMax - logMin));
                var slotBox = new Box(lineBox.x1, lineBox.y1, lineBox.x1, lineBox.y1);
                var start = a;
                var end = b;
                if (!defined(start)) {
                    start = end || 1;
                }
                if (!defined(end)) {
                    end = start || 1;
                }
                if (start <= 0 || end <= 0) {
                    return null;
                }
                if (limit) {
                    start = Math.max(Math.min(start, options.max), options.min);
                    end = Math.max(Math.min(end, options.max), options.min);
                }
                start = log(start, base);
                end = log(end, base);
                var p1, p2;
                if (vertical) {
                    p1 = logMax - Math.max(start, end);
                    p2 = logMax - Math.min(start, end);
                } else {
                    p1 = Math.min(start, end) - logMin;
                    p2 = Math.max(start, end) - logMin;
                }
                slotBox[valueAxis + 1] = limitCoordinate(lineStart + step * (reverse ? p2 : p1));
                slotBox[valueAxis + 2] = limitCoordinate(lineStart + step * (reverse ? p1 : p2));
                return slotBox;
            },
            getValue: function (point) {
                var ref = this;
                var options = ref.options;
                var logMin = ref.logMin;
                var logMax = ref.logMax;
                var reverse = options.reverse;
                var vertical = options.vertical;
                var base = options.majorUnit;
                var lineBox = this.lineBox();
                var dir = vertical === reverse ? 1 : -1;
                var startEdge = dir === 1 ? 1 : 2;
                var lineSize = vertical ? lineBox.height() : lineBox.width();
                var step = (logMax - logMin) / lineSize;
                var valueAxis = vertical ? Y : X;
                var lineStart = lineBox[valueAxis + startEdge];
                var offset = dir * (point[valueAxis] - lineStart);
                var valueOffset = offset * step;
                if (offset < 0 || offset > lineSize) {
                    return null;
                }
                var value = logMin + valueOffset;
                return round(Math.pow(base, value), DEFAULT_PRECISION);
            },
            range: function () {
                var options = this.options;
                return {
                    min: options.min,
                    max: options.max
                };
            },
            scaleRange: function (delta) {
                var base = this.options.majorUnit;
                var offset = -delta;
                return {
                    min: Math.pow(base, this.logMin - offset),
                    max: Math.pow(base, this.logMax + offset)
                };
            },
            translateRange: function (delta) {
                var ref = this;
                var options = ref.options;
                var logMin = ref.logMin;
                var logMax = ref.logMax;
                var reverse = options.reverse;
                var vertical = options.vertical;
                var base = options.majorUnit;
                var lineBox = this.lineBox();
                var size = vertical ? lineBox.height() : lineBox.width();
                var scale = size / (logMax - logMin);
                var offset = round(delta / scale, DEFAULT_PRECISION);
                if ((vertical || reverse) && !(vertical && reverse)) {
                    offset = -offset;
                }
                return {
                    min: Math.pow(base, logMin + offset),
                    max: Math.pow(base, logMax + offset)
                };
            },
            labelsCount: function () {
                var floorMax = Math.floor(this.logMax);
                var count = Math.floor(floorMax - this.logMin) + 1;
                return count;
            },
            getMajorTickPositions: function () {
                var ticks = [];
                this.traverseMajorTicksPositions(function (position) {
                    ticks.push(position);
                }, {
                    step: 1,
                    skip: 0
                });
                return ticks;
            },
            createTicks: function (lineGroup) {
                var options = this.options;
                var majorTicks = options.majorTicks;
                var minorTicks = options.minorTicks;
                var vertical = options.vertical;
                var mirror = options.labels.mirror;
                var lineBox = this.lineBox();
                var ticks = [];
                var tickLineOptions = { vertical: vertical };
                function render(tickPosition, tickOptions) {
                    tickLineOptions.tickX = mirror ? lineBox.x2 : lineBox.x2 - tickOptions.size;
                    tickLineOptions.tickY = mirror ? lineBox.y1 - tickOptions.size : lineBox.y1;
                    tickLineOptions.position = tickPosition;
                    lineGroup.append(createAxisTick(tickLineOptions, tickOptions));
                }
                if (majorTicks.visible) {
                    this.traverseMajorTicksPositions(render, majorTicks);
                }
                if (minorTicks.visible) {
                    this.traverseMinorTicksPositions(render, minorTicks);
                }
                return ticks;
            },
            createGridLines: function (altAxis) {
                var options = this.options;
                var minorGridLines = options.minorGridLines;
                var majorGridLines = options.majorGridLines;
                var vertical = options.vertical;
                var lineBox = altAxis.lineBox();
                var lineOptions = {
                    lineStart: lineBox[vertical ? 'x1' : 'y1'],
                    lineEnd: lineBox[vertical ? 'x2' : 'y2'],
                    vertical: vertical
                };
                var majorTicks = [];
                var container = this.gridLinesVisual();
                function render(tickPosition, gridLine) {
                    if (!inArray(tickPosition, majorTicks)) {
                        lineOptions.position = tickPosition;
                        container.append(createAxisGridLine(lineOptions, gridLine));
                        majorTicks.push(tickPosition);
                    }
                }
                if (majorGridLines.visible) {
                    this.traverseMajorTicksPositions(render, majorGridLines);
                }
                if (minorGridLines.visible) {
                    this.traverseMinorTicksPositions(render, minorGridLines);
                }
                return container.children;
            },
            traverseMajorTicksPositions: function (callback, tickOptions) {
                var ref = this._lineOptions();
                var lineStart = ref.lineStart;
                var step = ref.step;
                var ref$1 = this;
                var logMin = ref$1.logMin;
                var logMax = ref$1.logMax;
                for (var power = Math.ceil(logMin) + tickOptions.skip; power <= logMax; power += tickOptions.step) {
                    var position = round(lineStart + step * (power - logMin), DEFAULT_PRECISION);
                    callback(position, tickOptions);
                }
            },
            traverseMinorTicksPositions: function (callback, tickOptions) {
                var this$1 = this;
                var ref = this.options;
                var min = ref.min;
                var max = ref.max;
                var minorUnit = ref.minorUnit;
                var base = ref.majorUnit;
                var ref$1 = this._lineOptions();
                var lineStart = ref$1.lineStart;
                var step = ref$1.step;
                var ref$2 = this;
                var logMin = ref$2.logMin;
                var logMax = ref$2.logMax;
                var start = Math.floor(logMin);
                for (var power = start; power < logMax; power++) {
                    var minorOptions = this$1._minorIntervalOptions(power);
                    for (var idx = tickOptions.skip; idx < minorUnit; idx += tickOptions.step) {
                        var value = minorOptions.value + idx * minorOptions.minorStep;
                        if (value > max) {
                            break;
                        }
                        if (value >= min) {
                            var position = round(lineStart + step * (log(value, base) - logMin), DEFAULT_PRECISION);
                            callback(position, tickOptions);
                        }
                    }
                }
            },
            createAxisLabel: function (index, labelOptions) {
                var power = Math.ceil(this.logMin + index);
                var value = Math.pow(this.options.majorUnit, power);
                var text = this.axisLabelText(value, null, labelOptions);
                return new AxisLabel(value, text, index, null, labelOptions);
            },
            shouldRenderNote: function (value) {
                var range = this.range();
                return range.min <= value && value <= range.max;
            },
            pan: function (delta) {
                var range = this.translateRange(delta);
                return this.limitRange(range.min, range.max, this.totalMin, this.totalMax, -delta);
            },
            pointsRange: function (start, end) {
                var startValue = this.getValue(start);
                var endValue = this.getValue(end);
                var min = Math.min(startValue, endValue);
                var max = Math.max(startValue, endValue);
                return {
                    min: min,
                    max: max
                };
            },
            zoomRange: function (delta) {
                var ref = this;
                var options = ref.options;
                var totalMin = ref.totalMin;
                var totalMax = ref.totalMax;
                var newRange = this.scaleRange(delta);
                var min = limitValue(newRange.min, totalMin, totalMax);
                var max = limitValue(newRange.max, totalMin, totalMax);
                var base = options.majorUnit;
                var acceptOptionsRange = max > min && options.min && options.max && round(log(options.max, base) - log(options.min, base), DEFAULT_PRECISION) < 1;
                var acceptNewRange = !(options.min === totalMin && options.max === totalMax) && round(log(max, base) - log(min, base), DEFAULT_PRECISION) >= 1;
                if (acceptOptionsRange || acceptNewRange) {
                    return {
                        min: min,
                        max: max
                    };
                }
            },
            _minorIntervalOptions: function (power) {
                var ref = this.options;
                var minorUnit = ref.minorUnit;
                var base = ref.majorUnit;
                var value = Math.pow(base, power);
                var nextValue = Math.pow(base, power + 1);
                var difference = nextValue - value;
                var minorStep = difference / minorUnit;
                return {
                    value: value,
                    minorStep: minorStep
                };
            },
            _lineOptions: function () {
                var ref = this.options;
                var reverse = ref.reverse;
                var vertical = ref.vertical;
                var valueAxis = vertical ? Y : X;
                var lineBox = this.lineBox();
                var dir = vertical === reverse ? 1 : -1;
                var startEdge = dir === 1 ? 1 : 2;
                var lineSize = vertical ? lineBox.height() : lineBox.width();
                var step = dir * (lineSize / (this.logMax - this.logMin));
                var lineStart = lineBox[valueAxis + startEdge];
                return {
                    step: step,
                    lineStart: lineStart,
                    lineBox: lineBox
                };
            }
        });
        function initRange(autoMin, autoMax, axisOptions, options) {
            var min = axisOptions.min;
            var max = axisOptions.max;
            if (defined(axisOptions.axisCrossingValue) && axisOptions.axisCrossingValue <= 0) {
                throwNegativeValuesError();
            }
            if (!defined(options.max)) {
                max = autoMax;
            } else if (options.max <= 0) {
                throwNegativeValuesError();
            }
            if (!defined(options.min)) {
                min = autoMin;
            } else if (options.min <= 0) {
                throwNegativeValuesError();
            }
            return {
                min: min,
                max: max
            };
        }
        function autoAxisMin$1(min, max, options) {
            var base = options.majorUnit;
            var autoMin = min;
            if (min <= 0) {
                autoMin = max <= 1 ? Math.pow(base, -2) : 1;
            } else if (!options.narrowRange) {
                autoMin = Math.pow(base, Math.floor(log(min, base)));
            }
            return autoMin;
        }
        function autoAxisMax$1(max, base) {
            var logMaxRemainder = round(log(max, base), DEFAULT_PRECISION) % 1;
            var autoMax;
            if (max <= 0) {
                autoMax = base;
            } else if (logMaxRemainder !== 0 && (logMaxRemainder < 0.3 || logMaxRemainder > 0.9)) {
                autoMax = Math.pow(base, log(max, base) + 0.2);
            } else {
                autoMax = Math.pow(base, Math.ceil(log(max, base)));
            }
            return autoMax;
        }
        function throwNegativeValuesError() {
            throw new Error('Non positive values cannot be used for a logarithmic axis');
        }
        function log(y, x) {
            return Math.log(y) / Math.log(x);
        }
        setDefaultOptions(LogarithmicAxis, {
            type: 'log',
            majorUnit: DEFAULT_MAJOR_UNIT,
            minorUnit: 1,
            axisCrossingValue: 1,
            vertical: true,
            majorGridLines: {
                visible: true,
                width: 1,
                color: BLACK
            },
            zIndex: 1,
            _deferLabels: true
        });
        var GridLinesMixin = {
            createGridLines: function (altAxis) {
                var options = this.options;
                var radius = Math.abs(this.box.center().y - altAxis.lineBox().y1);
                var gridLines = [];
                var skipMajor = false;
                var majorAngles, minorAngles;
                if (options.majorGridLines.visible) {
                    majorAngles = this.majorGridLineAngles(altAxis);
                    skipMajor = true;
                    gridLines = this.renderMajorGridLines(majorAngles, radius, options.majorGridLines);
                }
                if (options.minorGridLines.visible) {
                    minorAngles = this.minorGridLineAngles(altAxis, skipMajor);
                    append(gridLines, this.renderMinorGridLines(minorAngles, radius, options.minorGridLines, altAxis, skipMajor));
                }
                return gridLines;
            },
            renderMajorGridLines: function (angles, radius, options) {
                return this.renderGridLines(angles, radius, options);
            },
            renderMinorGridLines: function (angles, radius, options, altAxis, skipMajor) {
                var radiusCallback = this.radiusCallback && this.radiusCallback(radius, altAxis, skipMajor);
                return this.renderGridLines(angles, radius, options, radiusCallback);
            },
            renderGridLines: function (angles, radius, options, radiusCallback) {
                var style = {
                    stroke: {
                        width: options.width,
                        color: options.color,
                        dashType: options.dashType
                    }
                };
                var center = this.box.center();
                var circle = new Circle([
                    center.x,
                    center.y
                ], radius);
                var container = this.gridLinesVisual();
                for (var i = 0; i < angles.length; i++) {
                    var line = new Path(style);
                    if (radiusCallback) {
                        circle.radius = radiusCallback(angles[i]);
                    }
                    line.moveTo(circle.center).lineTo(circle.pointAt(angles[i] + 180));
                    container.append(line);
                }
                return container.children;
            },
            gridLineAngles: function (altAxis, size, skip, step, skipAngles) {
                var this$1 = this;
                var divs = this.intervals(size, skip, step, skipAngles);
                var options = altAxis.options;
                var altAxisVisible = options.visible && (options.line || {}).visible !== false;
                return map(divs, function (d) {
                    var alpha = this$1.intervalAngle(d);
                    if (!altAxisVisible || alpha !== 90) {
                        return alpha;
                    }
                });
            }
        };
        var RadarCategoryAxis = CategoryAxis.extend({
            range: function () {
                return {
                    min: 0,
                    max: this.options.categories.length
                };
            },
            reflow: function (box) {
                this.box = box;
                this.reflowLabels();
            },
            lineBox: function () {
                return this.box;
            },
            reflowLabels: function () {
                var this$1 = this;
                var ref = this;
                var labels = ref.labels;
                var labelOptions = ref.options.labels;
                var skip = labelOptions.skip || 0;
                var step = labelOptions.step || 1;
                var measureBox = new Box();
                for (var i = 0; i < labels.length; i++) {
                    labels[i].reflow(measureBox);
                    var labelBox = labels[i].box;
                    labels[i].reflow(this$1.getSlot(skip + i * step).adjacentBox(0, labelBox.width(), labelBox.height()));
                }
            },
            intervals: function (size, skipOption, stepOption, skipAngles) {
                if (skipAngles === void 0) {
                    skipAngles = false;
                }
                var options = this.options;
                var categories = options.categories.length;
                var divCount = categories / size || 1;
                var divAngle = 360 / divCount;
                var skip = skipOption || 0;
                var step = stepOption || 1;
                var divs = [];
                var angle = 0;
                for (var i = skip; i < divCount; i += step) {
                    if (options.reverse) {
                        angle = 360 - i * divAngle;
                    } else {
                        angle = i * divAngle;
                    }
                    angle = round(angle, COORD_PRECISION) % 360;
                    if (!(skipAngles && inArray(angle, skipAngles))) {
                        divs.push(angle);
                    }
                }
                return divs;
            },
            majorIntervals: function () {
                return this.intervals(1);
            },
            minorIntervals: function () {
                return this.intervals(0.5);
            },
            intervalAngle: function (interval) {
                return (360 + interval + this.options.startAngle) % 360;
            },
            majorAngles: function () {
                var this$1 = this;
                return map(this.majorIntervals(), function (interval) {
                    return this$1.intervalAngle(interval);
                });
            },
            createLine: function () {
                return [];
            },
            majorGridLineAngles: function (altAxis) {
                var majorGridLines = this.options.majorGridLines;
                return this.gridLineAngles(altAxis, 1, majorGridLines.skip, majorGridLines.step);
            },
            minorGridLineAngles: function (altAxis, skipMajor) {
                var ref = this.options;
                var minorGridLines = ref.minorGridLines;
                var majorGridLines = ref.majorGridLines;
                var majorGridLineAngles = skipMajor ? this.intervals(1, majorGridLines.skip, majorGridLines.step) : null;
                return this.gridLineAngles(altAxis, 0.5, minorGridLines.skip, minorGridLines.step, majorGridLineAngles);
            },
            radiusCallback: function (radius, altAxis, skipMajor) {
                if (altAxis.options.type !== ARC) {
                    var minorAngle = rad(360 / (this.options.categories.length * 2));
                    var minorRadius = Math.cos(minorAngle) * radius;
                    var majorAngles = this.majorAngles();
                    var radiusCallback = function (angle) {
                        if (!skipMajor && inArray(angle, majorAngles)) {
                            return radius;
                        }
                        return minorRadius;
                    };
                    return radiusCallback;
                }
            },
            createPlotBands: function () {
                var this$1 = this;
                var plotBands = this.options.plotBands || [];
                var group = this._plotbandGroup = new Group({ zIndex: -1 });
                for (var i = 0; i < plotBands.length; i++) {
                    var band = plotBands[i];
                    var slot = this$1.plotBandSlot(band);
                    var singleSlot = this$1.getSlot(band.from);
                    var head = band.from - Math.floor(band.from);
                    slot.startAngle += head * singleSlot.angle;
                    var tail = Math.ceil(band.to) - band.to;
                    slot.angle -= (tail + head) * singleSlot.angle;
                    var ring = ShapeBuilder.current.createRing(slot, {
                        fill: {
                            color: band.color,
                            opacity: band.opacity
                        },
                        stroke: { opacity: band.opacity }
                    });
                    group.append(ring);
                }
                this.appendVisual(group);
            },
            plotBandSlot: function (band) {
                return this.getSlot(band.from, band.to - 1);
            },
            getSlot: function (from, to) {
                var options = this.options;
                var justified = options.justified;
                var box = this.box;
                var divs = this.majorAngles();
                var totalDivs = divs.length;
                var slotAngle = 360 / totalDivs;
                var fromValue = from;
                if (options.reverse && !justified) {
                    fromValue = (fromValue + 1) % totalDivs;
                }
                fromValue = limitValue(Math.floor(fromValue), 0, totalDivs - 1);
                var slotStart = divs[fromValue];
                if (justified) {
                    slotStart = slotStart - slotAngle / 2;
                    if (slotStart < 0) {
                        slotStart += 360;
                    }
                }
                var toValue = limitValue(Math.ceil(to || fromValue), fromValue, totalDivs - 1);
                var slots = toValue - fromValue + 1;
                var angle = slotAngle * slots;
                return new Ring(box.center(), 0, box.height() / 2, slotStart, angle);
            },
            slot: function (from, to) {
                var slot = this.getSlot(from, to);
                var startAngle = slot.startAngle + 180;
                var endAngle = startAngle + slot.angle;
                return new geometry.Arc([
                    slot.center.x,
                    slot.center.y
                ], {
                    startAngle: startAngle,
                    endAngle: endAngle,
                    radiusX: slot.radius,
                    radiusY: slot.radius
                });
            },
            pointCategoryIndex: function (point) {
                var this$1 = this;
                var length = this.options.categories.length;
                var index = null;
                for (var i = 0; i < length; i++) {
                    var slot = this$1.getSlot(i);
                    if (slot.containsPoint(point)) {
                        index = i;
                        break;
                    }
                }
                return index;
            }
        });
        setDefaultOptions(RadarCategoryAxis, {
            startAngle: 90,
            labels: { margin: getSpacing(10) },
            majorGridLines: { visible: true },
            justified: true
        });
        deepExtend(RadarCategoryAxis.prototype, GridLinesMixin);
        var PolarAxis = Axis.extend({
            init: function (options, chartService) {
                Axis.fn.init.call(this, options, chartService);
                var instanceOptions = this.options;
                instanceOptions.minorUnit = instanceOptions.minorUnit || instanceOptions.majorUnit / 2;
            },
            getDivisions: function (stepValue) {
                return NumericAxis.prototype.getDivisions.call(this, stepValue) - 1;
            },
            reflow: function (box) {
                this.box = box;
                this.reflowLabels();
            },
            reflowLabels: function () {
                var this$1 = this;
                var ref = this;
                var options = ref.options;
                var labels = ref.labels;
                var labelOptions = ref.options.labels;
                var skip = labelOptions.skip || 0;
                var step = labelOptions.step || 1;
                var measureBox = new Box();
                var divs = this.intervals(options.majorUnit, skip, step);
                for (var i = 0; i < labels.length; i++) {
                    labels[i].reflow(measureBox);
                    var labelBox = labels[i].box;
                    labels[i].reflow(this$1.getSlot(divs[i]).adjacentBox(0, labelBox.width(), labelBox.height()));
                }
            },
            lineBox: function () {
                return this.box;
            },
            intervals: function (size, skipOption, stepOption, skipAngles) {
                if (skipAngles === void 0) {
                    skipAngles = false;
                }
                var min = this.options.min;
                var divisions = this.getDivisions(size);
                var divs = [];
                var skip = skipOption || 0;
                var step = stepOption || 1;
                for (var i = skip; i < divisions; i += step) {
                    var current = (360 + min + i * size) % 360;
                    if (!(skipAngles && inArray(current, skipAngles))) {
                        divs.push(current);
                    }
                }
                return divs;
            },
            majorIntervals: function () {
                return this.intervals(this.options.majorUnit);
            },
            minorIntervals: function () {
                return this.intervals(this.options.minorUnit);
            },
            intervalAngle: function (i) {
                return (540 - i - this.options.startAngle) % 360;
            },
            createLine: function () {
                return [];
            },
            majorGridLineAngles: function (altAxis) {
                var majorGridLines = this.options.majorGridLines;
                return this.gridLineAngles(altAxis, this.options.majorUnit, majorGridLines.skip, majorGridLines.step);
            },
            minorGridLineAngles: function (altAxis, skipMajor) {
                var options = this.options;
                var minorGridLines = options.minorGridLines;
                var majorGridLines = options.majorGridLines;
                var majorGridLineAngles = skipMajor ? this.intervals(options.majorUnit, majorGridLines.skip, majorGridLines.step) : null;
                return this.gridLineAngles(altAxis, options.minorUnit, minorGridLines.skip, minorGridLines.step, majorGridLineAngles);
            },
            plotBandSlot: function (band) {
                return this.getSlot(band.from, band.to);
            },
            getSlot: function (a, b) {
                var ref = this;
                var options = ref.options;
                var box = ref.box;
                var startAngle = options.startAngle;
                var start = limitValue(a, options.min, options.max);
                var end = limitValue(b || start, start, options.max);
                if (options.reverse) {
                    start *= -1;
                    end *= -1;
                }
                start = (540 - start - startAngle) % 360;
                end = (540 - end - startAngle) % 360;
                if (end < start) {
                    var tmp = start;
                    start = end;
                    end = tmp;
                }
                return new Ring(box.center(), 0, box.height() / 2, start, end - start);
            },
            slot: function (from, to) {
                if (to === void 0) {
                    to = from;
                }
                var options = this.options;
                var start = 360 - options.startAngle;
                var slot = this.getSlot(from, to);
                var min = Math.min(from, to);
                var max = Math.max(from, to);
                var startAngle, endAngle;
                if (options.reverse) {
                    startAngle = min;
                    endAngle = max;
                } else {
                    startAngle = 360 - max;
                    endAngle = 360 - min;
                }
                startAngle = (startAngle + start) % 360;
                endAngle = (endAngle + start) % 360;
                return new geometry.Arc([
                    slot.center.x,
                    slot.center.y
                ], {
                    startAngle: startAngle,
                    endAngle: endAngle,
                    radiusX: slot.radius,
                    radiusY: slot.radius
                });
            },
            getValue: function (point) {
                var options = this.options;
                var center = this.box.center();
                var dx = point.x - center.x;
                var dy = point.y - center.y;
                var theta = Math.round(deg(Math.atan2(dy, dx)));
                var start = options.startAngle;
                if (!options.reverse) {
                    theta *= -1;
                    start *= -1;
                }
                return (theta + start + 360) % 360;
            },
            valueRange: function () {
                return {
                    min: 0,
                    max: Math.PI * 2
                };
            }
        });
        setDefaultOptions(PolarAxis, {
            type: 'polar',
            startAngle: 0,
            reverse: false,
            majorUnit: 60,
            min: 0,
            max: 360,
            labels: { margin: getSpacing(10) },
            majorGridLines: {
                color: BLACK,
                visible: true,
                width: 1
            },
            minorGridLines: { color: '#aaa' }
        });
        deepExtend(PolarAxis.prototype, GridLinesMixin, {
            createPlotBands: RadarCategoryAxis.prototype.createPlotBands,
            majorAngles: RadarCategoryAxis.prototype.majorAngles,
            range: NumericAxis.prototype.range,
            labelsCount: NumericAxis.prototype.labelsCount,
            createAxisLabel: NumericAxis.prototype.createAxisLabel
        });
        var RadarNumericAxisMixin = {
            options: { majorGridLines: { visible: true } },
            createPlotBands: function () {
                var this$1 = this;
                var ref = this.options;
                var type = ref.majorGridLines.type;
                var plotBands = ref.plotBands;
                if (plotBands === void 0) {
                    plotBands = [];
                }
                var altAxis = this.plotArea.polarAxis;
                var majorAngles = altAxis.majorAngles();
                var center = altAxis.box.center();
                var group = this._plotbandGroup = new Group({ zIndex: -1 });
                for (var i = 0; i < plotBands.length; i++) {
                    var band = plotBands[i];
                    var bandStyle = {
                        fill: {
                            color: band.color,
                            opacity: band.opacity
                        },
                        stroke: { opacity: band.opacity }
                    };
                    var slot = this$1.getSlot(band.from, band.to, true);
                    var ring = new Ring(center, center.y - slot.y2, center.y - slot.y1, 0, 360);
                    var shape = void 0;
                    if (type === ARC) {
                        shape = ShapeBuilder.current.createRing(ring, bandStyle);
                    } else {
                        shape = Path.fromPoints(this$1.plotBandPoints(ring, majorAngles), bandStyle).close();
                    }
                    group.append(shape);
                }
                this.appendVisual(group);
            },
            plotBandPoints: function (ring, angles) {
                var innerPoints = [];
                var outerPoints = [];
                var center = [
                    ring.center.x,
                    ring.center.y
                ];
                var innerCircle = new Circle(center, ring.innerRadius);
                var outerCircle = new Circle(center, ring.radius);
                for (var i = 0; i < angles.length; i++) {
                    innerPoints.push(innerCircle.pointAt(angles[i] + 180));
                    outerPoints.push(outerCircle.pointAt(angles[i] + 180));
                }
                innerPoints.reverse();
                innerPoints.push(innerPoints[0]);
                outerPoints.push(outerPoints[0]);
                return outerPoints.concat(innerPoints);
            },
            createGridLines: function (altAxis) {
                var options = this.options;
                var majorTicks = this.radarMajorGridLinePositions();
                var majorAngles = altAxis.majorAngles();
                var center = altAxis.box.center();
                var gridLines = [];
                if (options.majorGridLines.visible) {
                    gridLines = this.renderGridLines(center, majorTicks, majorAngles, options.majorGridLines);
                }
                if (options.minorGridLines.visible) {
                    var minorTicks = this.radarMinorGridLinePositions();
                    append(gridLines, this.renderGridLines(center, minorTicks, majorAngles, options.minorGridLines));
                }
                return gridLines;
            },
            renderGridLines: function (center, ticks, angles, options) {
                var style = {
                    stroke: {
                        width: options.width,
                        color: options.color,
                        dashType: options.dashType
                    }
                };
                var skip = options.skip;
                if (skip === void 0) {
                    skip = 0;
                }
                var step = options.step;
                if (step === void 0) {
                    step = 0;
                }
                var container = this.gridLinesVisual();
                for (var tickIx = skip; tickIx < ticks.length; tickIx += step) {
                    var tickRadius = center.y - ticks[tickIx];
                    if (tickRadius > 0) {
                        var circle = new Circle([
                            center.x,
                            center.y
                        ], tickRadius);
                        if (options.type === ARC) {
                            container.append(new drawing.Circle(circle, style));
                        } else {
                            var line = new Path(style);
                            for (var angleIx = 0; angleIx < angles.length; angleIx++) {
                                line.lineTo(circle.pointAt(angles[angleIx] + 180));
                            }
                            line.close();
                            container.append(line);
                        }
                    }
                }
                return container.children;
            },
            getValue: function (point) {
                var lineBox = this.lineBox();
                var altAxis = this.plotArea.polarAxis;
                var majorAngles = altAxis.majorAngles();
                var center = altAxis.box.center();
                var radius = point.distanceTo(center);
                var distance = radius;
                if (this.options.majorGridLines.type !== ARC && majorAngles.length > 1) {
                    var dx = point.x - center.x;
                    var dy = point.y - center.y;
                    var theta = (deg(Math.atan2(dy, dx)) + 540) % 360;
                    majorAngles.sort(function (a, b) {
                        return angularDistance(a, theta) - angularDistance(b, theta);
                    });
                    var midAngle = angularDistance(majorAngles[0], majorAngles[1]) / 2;
                    var alpha = angularDistance(theta, majorAngles[0]);
                    var gamma = 90 - midAngle;
                    var beta = 180 - alpha - gamma;
                    distance = radius * (Math.sin(rad(beta)) / Math.sin(rad(gamma)));
                }
                return this.axisType().prototype.getValue.call(this, new Point(lineBox.x1, lineBox.y2 - distance));
            }
        };
        function angularDistance(a, b) {
            return 180 - Math.abs(Math.abs(a - b) - 180);
        }
        var RadarNumericAxis = NumericAxis.extend({
            radarMajorGridLinePositions: function () {
                return this.getTickPositions(this.options.majorUnit);
            },
            radarMinorGridLinePositions: function () {
                var options = this.options;
                var minorSkipStep = 0;
                if (options.majorGridLines.visible) {
                    minorSkipStep = options.majorUnit;
                }
                return this.getTickPositions(options.minorUnit, minorSkipStep);
            },
            axisType: function () {
                return NumericAxis;
            }
        });
        deepExtend(RadarNumericAxis.prototype, RadarNumericAxisMixin);
        var RadarLogarithmicAxis = LogarithmicAxis.extend({
            radarMajorGridLinePositions: function () {
                var positions = [];
                this.traverseMajorTicksPositions(function (position) {
                    positions.push(position);
                }, this.options.majorGridLines);
                return positions;
            },
            radarMinorGridLinePositions: function () {
                var positions = [];
                this.traverseMinorTicksPositions(function (position) {
                    positions.push(position);
                }, this.options.minorGridLines);
                return positions;
            },
            axisType: function () {
                return LogarithmicAxis;
            }
        });
        deepExtend(RadarLogarithmicAxis.prototype, RadarNumericAxisMixin);
        var WEIGHT = 0.333;
        var EXTREMUM_ALLOWED_DEVIATION = 0.01;
        var CurveProcessor = Class.extend({
            init: function (closed) {
                this.closed = closed;
            },
            process: function (dataPoints) {
                var this$1 = this;
                var points = dataPoints.slice(0);
                var segments = [];
                var closed = this.closed;
                var length = points.length;
                if (length > 2) {
                    this.removeDuplicates(0, points);
                    length = points.length;
                }
                if (length < 2 || length === 2 && points[0].equals(points[1])) {
                    return segments;
                }
                var p0 = points[0];
                var p1 = points[1];
                var p2 = points[2];
                segments.push(new Segment(p0));
                while (p0.equals(points[length - 1])) {
                    closed = true;
                    points.pop();
                    length--;
                }
                if (length === 2) {
                    var tangent = this.tangent(p0, p1, X, Y);
                    last(segments).controlOut(this.firstControlPoint(tangent, p0, p1, X, Y));
                    segments.push(new Segment(p1, this.secondControlPoint(tangent, p0, p1, X, Y)));
                    return segments;
                }
                var initialControlPoint, lastControlPoint;
                if (closed) {
                    p0 = points[length - 1];
                    p1 = points[0];
                    p2 = points[1];
                    var controlPoints = this.controlPoints(p0, p1, p2);
                    initialControlPoint = controlPoints[1];
                    lastControlPoint = controlPoints[0];
                } else {
                    var tangent$1 = this.tangent(p0, p1, X, Y);
                    initialControlPoint = this.firstControlPoint(tangent$1, p0, p1, X, Y);
                }
                var cp0 = initialControlPoint;
                for (var idx = 0; idx <= length - 3; idx++) {
                    this$1.removeDuplicates(idx, points);
                    length = points.length;
                    if (idx + 3 <= length) {
                        p0 = points[idx];
                        p1 = points[idx + 1];
                        p2 = points[idx + 2];
                        var controlPoints$1 = this$1.controlPoints(p0, p1, p2);
                        last(segments).controlOut(cp0);
                        cp0 = controlPoints$1[1];
                        var cp1 = controlPoints$1[0];
                        segments.push(new Segment(p1, cp1));
                    }
                }
                if (closed) {
                    p0 = points[length - 2];
                    p1 = points[length - 1];
                    p2 = points[0];
                    var controlPoints$2 = this.controlPoints(p0, p1, p2);
                    last(segments).controlOut(cp0);
                    segments.push(new Segment(p1, controlPoints$2[0]));
                    last(segments).controlOut(controlPoints$2[1]);
                    segments.push(new Segment(p2, lastControlPoint));
                } else {
                    var tangent$2 = this.tangent(p1, p2, X, Y);
                    last(segments).controlOut(cp0);
                    segments.push(new Segment(p2, this.secondControlPoint(tangent$2, p1, p2, X, Y)));
                }
                return segments;
            },
            removeDuplicates: function (idx, points) {
                while (points[idx + 1] && (points[idx].equals(points[idx + 1]) || points[idx + 1].equals(points[idx + 2]))) {
                    points.splice(idx + 1, 1);
                }
            },
            invertAxis: function (p0, p1, p2) {
                var invertAxis = false;
                if (p0.x === p1.x) {
                    invertAxis = true;
                } else if (p1.x === p2.x) {
                    if (p1.y < p2.y && p0.y <= p1.y || p2.y < p1.y && p1.y <= p0.y) {
                        invertAxis = true;
                    }
                } else {
                    var fn = this.lineFunction(p0, p1);
                    var y2 = this.calculateFunction(fn, p2.x);
                    if (!(p0.y <= p1.y && p2.y <= y2) && !(p1.y <= p0.y && p2.y >= y2)) {
                        invertAxis = true;
                    }
                }
                return invertAxis;
            },
            isLine: function (p0, p1, p2) {
                var fn = this.lineFunction(p0, p1);
                var y2 = this.calculateFunction(fn, p2.x);
                return p0.x === p1.x && p1.x === p2.x || round(y2, 1) === round(p2.y, 1);
            },
            lineFunction: function (p1, p2) {
                var a = (p2.y - p1.y) / (p2.x - p1.x);
                var b = p1.y - a * p1.x;
                return [
                    b,
                    a
                ];
            },
            controlPoints: function (p0, p1, p2) {
                var xField = X;
                var yField = Y;
                var restrict = false;
                var switchOrientation = false;
                var tangent;
                if (this.isLine(p0, p1, p2)) {
                    tangent = this.tangent(p0, p1, X, Y);
                } else {
                    var monotonic = {
                        x: this.isMonotonicByField(p0, p1, p2, X),
                        y: this.isMonotonicByField(p0, p1, p2, Y)
                    };
                    if (monotonic.x && monotonic.y) {
                        tangent = this.tangent(p0, p2, X, Y);
                        restrict = true;
                    } else {
                        if (this.invertAxis(p0, p1, p2)) {
                            xField = Y;
                            yField = X;
                        }
                        if (monotonic[xField]) {
                            tangent = 0;
                        } else {
                            var sign;
                            if (p2[yField] < p0[yField] && p0[yField] <= p1[yField] || p0[yField] < p2[yField] && p1[yField] <= p0[yField]) {
                                sign = numberSign((p2[yField] - p0[yField]) * (p1[xField] - p0[xField]));
                            } else {
                                sign = -numberSign((p2[xField] - p0[xField]) * (p1[yField] - p0[yField]));
                            }
                            tangent = EXTREMUM_ALLOWED_DEVIATION * sign;
                            switchOrientation = true;
                        }
                    }
                }
                var secondControlPoint = this.secondControlPoint(tangent, p0, p1, xField, yField);
                if (switchOrientation) {
                    var oldXField = xField;
                    xField = yField;
                    yField = oldXField;
                }
                var firstControlPoint = this.firstControlPoint(tangent, p1, p2, xField, yField);
                if (restrict) {
                    this.restrictControlPoint(p0, p1, secondControlPoint, tangent);
                    this.restrictControlPoint(p1, p2, firstControlPoint, tangent);
                }
                return [
                    secondControlPoint,
                    firstControlPoint
                ];
            },
            restrictControlPoint: function (p1, p2, cp, tangent) {
                if (p1.y < p2.y) {
                    if (p2.y < cp.y) {
                        cp.x = p1.x + (p2.y - p1.y) / tangent;
                        cp.y = p2.y;
                    } else if (cp.y < p1.y) {
                        cp.x = p2.x - (p2.y - p1.y) / tangent;
                        cp.y = p1.y;
                    }
                } else {
                    if (cp.y < p2.y) {
                        cp.x = p1.x - (p1.y - p2.y) / tangent;
                        cp.y = p2.y;
                    } else if (p1.y < cp.y) {
                        cp.x = p2.x + (p1.y - p2.y) / tangent;
                        cp.y = p1.y;
                    }
                }
            },
            tangent: function (p0, p1, xField, yField) {
                var x = p1[xField] - p0[xField];
                var y = p1[yField] - p0[yField];
                var tangent;
                if (x === 0) {
                    tangent = 0;
                } else {
                    tangent = y / x;
                }
                return tangent;
            },
            isMonotonicByField: function (p0, p1, p2, field) {
                return p2[field] > p1[field] && p1[field] > p0[field] || p2[field] < p1[field] && p1[field] < p0[field];
            },
            firstControlPoint: function (tangent, p0, p3, xField, yField) {
                var t1 = p0[xField];
                var t2 = p3[xField];
                var distance = (t2 - t1) * WEIGHT;
                return this.point(t1 + distance, p0[yField] + distance * tangent, xField, yField);
            },
            secondControlPoint: function (tangent, p0, p3, xField, yField) {
                var t1 = p0[xField];
                var t2 = p3[xField];
                var distance = (t2 - t1) * WEIGHT;
                return this.point(t2 - distance, p3[yField] - distance * tangent, xField, yField);
            },
            point: function (xValue, yValue, xField, yField) {
                var controlPoint = new geometry.Point();
                controlPoint[xField] = xValue;
                controlPoint[yField] = yValue;
                return controlPoint;
            },
            calculateFunction: function (fn, x) {
                var length = fn.length;
                var result = 0;
                for (var i = 0; i < length; i++) {
                    result += Math.pow(x, i) * fn[i];
                }
                return result;
            }
        });
        function numberSign(value) {
            return value <= 0 ? -1 : 1;
        }
        dataviz.Gradients = GRADIENTS;
        kendo.deepExtend(kendo.dataviz, {
            constants: constants,
            services: services,
            autoMajorUnit: autoMajorUnit,
            Point: Point,
            Box: Box,
            Ring: Ring,
            Sector: Sector,
            ShapeBuilder: ShapeBuilder,
            ShapeElement: ShapeElement,
            ChartElement: ChartElement,
            BoxElement: BoxElement,
            RootElement: RootElement,
            FloatElement: FloatElement,
            Text: Text,
            TextBox: TextBox,
            Title: Title,
            AxisLabel: AxisLabel,
            Axis: Axis,
            Note: Note,
            CategoryAxis: CategoryAxis,
            DateCategoryAxis: DateCategoryAxis,
            DateValueAxis: DateValueAxis,
            NumericAxis: NumericAxis,
            LogarithmicAxis: LogarithmicAxis,
            PolarAxis: PolarAxis,
            RadarCategoryAxis: RadarCategoryAxis,
            RadarNumericAxis: RadarNumericAxis,
            RadarLogarithmicAxis: RadarLogarithmicAxis,
            CurveProcessor: CurveProcessor,
            rectToBox: rectToBox,
            addClass: addClass,
            removeClass: removeClass,
            alignPathToPixel: alignPathToPixel,
            clockwise: clockwise,
            convertableToNumber: convertableToNumber,
            deepExtend: deepExtend,
            elementStyles: elementStyles,
            getSpacing: getSpacing,
            getTemplate: getTemplate,
            getter: __common_getter_js,
            grep: grep,
            hasClasses: hasClasses,
            inArray: inArray,
            interpolateValue: interpolateValue,
            InstanceObserver: InstanceObserver,
            isArray: isArray,
            isFunction: isFunction,
            isNumber: isNumber,
            isObject: isObject,
            isString: isString,
            map: map,
            mousewheelDelta: mousewheelDelta,
            FontLoader: FontLoader,
            setDefaultOptions: setDefaultOptions,
            sparseArrayLimits: sparseArrayLimits,
            styleValue: styleValue,
            append: append,
            bindEvents: bindEvents,
            Class: Class,
            defined: defined,
            deg: deg,
            elementOffset: elementOffset,
            elementSize: elementSize,
            eventElement: eventElement,
            eventCoordinates: eventCoordinates,
            last: last,
            limitValue: limitValue,
            logToConsole: kendo.logToConsole,
            objectKey: objectKey,
            rad: rad,
            round: round,
            unbindEvents: unbindEvents,
            valueOrDefault: valueOrDefault,
            absoluteDateDiff: absoluteDateDiff,
            addDuration: addDuration,
            addTicks: addTicks,
            ceilDate: ceilDate,
            dateComparer: dateComparer,
            dateDiff: dateDiff,
            dateEquals: dateEquals,
            dateIndex: dateIndex,
            duration: duration,
            floorDate: floorDate,
            lteDateIndex: lteDateIndex,
            startOfWeek: startOfWeek,
            toDate: toDate,
            parseDate: parseDate,
            parseDates: parseDates,
            toTime: toTime
        });
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/core/core', ['dataviz/core/kendo-core'], f);
}(function () {
    (function ($) {
        var dataviz = kendo.dataviz;
        var services = dataviz.services;
        var draw = kendo.drawing;
        dataviz.ExportMixin = {
            extend: function (proto, skipLegacy) {
                if (!proto.exportVisual) {
                    throw new Error('Mixin target has no exportVisual method defined.');
                }
                proto.exportSVG = this.exportSVG;
                proto.exportImage = this.exportImage;
                proto.exportPDF = this.exportPDF;
                if (!skipLegacy) {
                    proto.svg = this.svg;
                    proto.imageDataURL = this.imageDataURL;
                }
            },
            exportSVG: function (options) {
                return draw.exportSVG(this.exportVisual(), options);
            },
            exportImage: function (options) {
                return draw.exportImage(this.exportVisual(options), options);
            },
            exportPDF: function (options) {
                return draw.exportPDF(this.exportVisual(), options);
            },
            svg: function () {
                if (draw.svg.Surface) {
                    return draw.svg.exportGroup(this.exportVisual());
                } else {
                    throw new Error('SVG Export failed. Unable to export instantiate kendo.drawing.svg.Surface');
                }
            },
            imageDataURL: function () {
                if (!kendo.support.canvas) {
                    return null;
                }
                if (draw.canvas.Surface) {
                    var container = $('<div />').css({
                        display: 'none',
                        width: this.element.width(),
                        height: this.element.height()
                    }).appendTo(document.body);
                    var surface = new draw.canvas.Surface(container[0]);
                    surface.draw(this.exportVisual());
                    var image = surface._rootElement.toDataURL();
                    surface.destroy();
                    container.remove();
                    return image;
                } else {
                    throw new Error('Image Export failed. Unable to export instantiate kendo.drawing.canvas.Surface');
                }
            }
        };
        services.IntlService.register({
            format: function (format) {
                return kendo.format.apply(null, [format].concat(Array.prototype.slice.call(arguments, 1)));
            },
            toString: kendo.toString,
            parseDate: kendo.parseDate
        });
        services.TemplateService.register({ compile: kendo.template });
        dataviz.Point2D = dataviz.Point;
        dataviz.Box2D = dataviz.Box;
        dataviz.mwDelta = function (e) {
            return dataviz.mousewheelDelta(e.originalEvent);
        };
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.dataviz.core', [
        'dataviz/core/kendo-core',
        'dataviz/core/core'
    ], f);
}(function () {
    var __meta__ = {
        id: 'dataviz.core',
        name: 'Core',
        description: 'The DataViz core functions',
        category: 'dataviz',
        depends: [
            'core',
            'drawing'
        ],
        hidden: true
    };
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));