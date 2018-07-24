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
    define('util/main', ['kendo.core'], f);
}(function () {
    (function () {
        var kendo = window.kendo, deepExtend = kendo.deepExtend;
        function sqr(value) {
            return value * value;
        }
        var now = Date.now;
        if (!now) {
            now = function () {
                return new Date().getTime();
            };
        }
        function renderSize(size) {
            if (typeof size !== 'string') {
                size += 'px';
            }
            return size;
        }
        function renderPos(pos) {
            var result = [];
            if (pos) {
                var parts = kendo.toHyphens(pos).split('-');
                for (var i = 0; i < parts.length; i++) {
                    result.push('k-pos-' + parts[i]);
                }
            }
            return result.join(' ');
        }
        function arabicToRoman(n) {
            var literals = {
                1: 'i',
                10: 'x',
                100: 'c',
                2: 'ii',
                20: 'xx',
                200: 'cc',
                3: 'iii',
                30: 'xxx',
                300: 'ccc',
                4: 'iv',
                40: 'xl',
                400: 'cd',
                5: 'v',
                50: 'l',
                500: 'd',
                6: 'vi',
                60: 'lx',
                600: 'dc',
                7: 'vii',
                70: 'lxx',
                700: 'dcc',
                8: 'viii',
                80: 'lxxx',
                800: 'dccc',
                9: 'ix',
                90: 'xc',
                900: 'cm',
                1000: 'm'
            };
            var values = [
                1000,
                900,
                800,
                700,
                600,
                500,
                400,
                300,
                200,
                100,
                90,
                80,
                70,
                60,
                50,
                40,
                30,
                20,
                10,
                9,
                8,
                7,
                6,
                5,
                4,
                3,
                2,
                1
            ];
            var roman = '';
            while (n > 0) {
                if (n < values[0]) {
                    values.shift();
                } else {
                    roman += literals[values[0]];
                    n -= values[0];
                }
            }
            return roman;
        }
        function romanToArabic(r) {
            r = r.toLowerCase();
            var digits = {
                i: 1,
                v: 5,
                x: 10,
                l: 50,
                c: 100,
                d: 500,
                m: 1000
            };
            var value = 0, prev = 0;
            for (var i = 0; i < r.length; ++i) {
                var v = digits[r.charAt(i)];
                if (!v) {
                    return null;
                }
                value += v;
                if (v > prev) {
                    value -= 2 * prev;
                }
                prev = v;
            }
            return value;
        }
        function memoize(f) {
            var cache = Object.create(null);
            return function () {
                var id = '';
                for (var i = arguments.length; --i >= 0;) {
                    id += ':' + arguments[i];
                }
                return id in cache ? cache[id] : cache[id] = f.apply(this, arguments);
            };
        }
        function isUnicodeLetter(ch) {
            return RX_UNICODE_LETTER.test(ch);
        }
        function withExit(f, obj) {
            try {
                return f.call(obj, function (value) {
                    throw new Return(value);
                });
            } catch (ex) {
                if (ex instanceof Return) {
                    return ex.value;
                }
                throw ex;
            }
            function Return(value) {
                this.value = value;
            }
        }
        deepExtend(kendo, {
            util: {
                now: now,
                renderPos: renderPos,
                renderSize: renderSize,
                sqr: sqr,
                romanToArabic: romanToArabic,
                arabicToRoman: arabicToRoman,
                memoize: memoize,
                isUnicodeLetter: isUnicodeLetter,
                withExit: withExit
            }
        });
        var RX_UNICODE_LETTER = new RegExp('[\\u0041-\\u005A\\u0061-\\u007A\\u00AA\\u00B5\\u00BA\\u00C0-\\u00D6\\u00D8-\\u00F6\\u00F8-\\u02C1\\u02C6-\\u02D1\\u02E0-\\u02E4\\u02EC\\u02EE\\u0370-\\u0374\\u0376\\u0377\\u037A-\\u037D\\u037F\\u0386\\u0388-\\u038A\\u038C\\u038E-\\u03A1\\u03A3-\\u03F5\\u03F7-\\u0481\\u048A-\\u052F\\u0531-\\u0556\\u0559\\u0561-\\u0587\\u05D0-\\u05EA\\u05F0-\\u05F2\\u0620-\\u064A\\u066E\\u066F\\u0671-\\u06D3\\u06D5\\u06E5\\u06E6\\u06EE\\u06EF\\u06FA-\\u06FC\\u06FF\\u0710\\u0712-\\u072F\\u074D-\\u07A5\\u07B1\\u07CA-\\u07EA\\u07F4\\u07F5\\u07FA\\u0800-\\u0815\\u081A\\u0824\\u0828\\u0840-\\u0858\\u08A0-\\u08B2\\u0904-\\u0939\\u093D\\u0950\\u0958-\\u0961\\u0971-\\u0980\\u0985-\\u098C\\u098F\\u0990\\u0993-\\u09A8\\u09AA-\\u09B0\\u09B2\\u09B6-\\u09B9\\u09BD\\u09CE\\u09DC\\u09DD\\u09DF-\\u09E1\\u09F0\\u09F1\\u0A05-\\u0A0A\\u0A0F\\u0A10\\u0A13-\\u0A28\\u0A2A-\\u0A30\\u0A32\\u0A33\\u0A35\\u0A36\\u0A38\\u0A39\\u0A59-\\u0A5C\\u0A5E\\u0A72-\\u0A74\\u0A85-\\u0A8D\\u0A8F-\\u0A91\\u0A93-\\u0AA8\\u0AAA-\\u0AB0\\u0AB2\\u0AB3\\u0AB5-\\u0AB9\\u0ABD\\u0AD0\\u0AE0\\u0AE1\\u0B05-\\u0B0C\\u0B0F\\u0B10\\u0B13-\\u0B28\\u0B2A-\\u0B30\\u0B32\\u0B33\\u0B35-\\u0B39\\u0B3D\\u0B5C\\u0B5D\\u0B5F-\\u0B61\\u0B71\\u0B83\\u0B85-\\u0B8A\\u0B8E-\\u0B90\\u0B92-\\u0B95\\u0B99\\u0B9A\\u0B9C\\u0B9E\\u0B9F\\u0BA3\\u0BA4\\u0BA8-\\u0BAA\\u0BAE-\\u0BB9\\u0BD0\\u0C05-\\u0C0C\\u0C0E-\\u0C10\\u0C12-\\u0C28\\u0C2A-\\u0C39\\u0C3D\\u0C58\\u0C59\\u0C60\\u0C61\\u0C85-\\u0C8C\\u0C8E-\\u0C90\\u0C92-\\u0CA8\\u0CAA-\\u0CB3\\u0CB5-\\u0CB9\\u0CBD\\u0CDE\\u0CE0\\u0CE1\\u0CF1\\u0CF2\\u0D05-\\u0D0C\\u0D0E-\\u0D10\\u0D12-\\u0D3A\\u0D3D\\u0D4E\\u0D60\\u0D61\\u0D7A-\\u0D7F\\u0D85-\\u0D96\\u0D9A-\\u0DB1\\u0DB3-\\u0DBB\\u0DBD\\u0DC0-\\u0DC6\\u0E01-\\u0E30\\u0E32\\u0E33\\u0E40-\\u0E46\\u0E81\\u0E82\\u0E84\\u0E87\\u0E88\\u0E8A\\u0E8D\\u0E94-\\u0E97\\u0E99-\\u0E9F\\u0EA1-\\u0EA3\\u0EA5\\u0EA7\\u0EAA\\u0EAB\\u0EAD-\\u0EB0\\u0EB2\\u0EB3\\u0EBD\\u0EC0-\\u0EC4\\u0EC6\\u0EDC-\\u0EDF\\u0F00\\u0F40-\\u0F47\\u0F49-\\u0F6C\\u0F88-\\u0F8C\\u1000-\\u102A\\u103F\\u1050-\\u1055\\u105A-\\u105D\\u1061\\u1065\\u1066\\u106E-\\u1070\\u1075-\\u1081\\u108E\\u10A0-\\u10C5\\u10C7\\u10CD\\u10D0-\\u10FA\\u10FC-\\u1248\\u124A-\\u124D\\u1250-\\u1256\\u1258\\u125A-\\u125D\\u1260-\\u1288\\u128A-\\u128D\\u1290-\\u12B0\\u12B2-\\u12B5\\u12B8-\\u12BE\\u12C0\\u12C2-\\u12C5\\u12C8-\\u12D6\\u12D8-\\u1310\\u1312-\\u1315\\u1318-\\u135A\\u1380-\\u138F\\u13A0-\\u13F4\\u1401-\\u166C\\u166F-\\u167F\\u1681-\\u169A\\u16A0-\\u16EA\\u16EE-\\u16F8\\u1700-\\u170C\\u170E-\\u1711\\u1720-\\u1731\\u1740-\\u1751\\u1760-\\u176C\\u176E-\\u1770\\u1780-\\u17B3\\u17D7\\u17DC\\u1820-\\u1877\\u1880-\\u18A8\\u18AA\\u18B0-\\u18F5\\u1900-\\u191E\\u1950-\\u196D\\u1970-\\u1974\\u1980-\\u19AB\\u19C1-\\u19C7\\u1A00-\\u1A16\\u1A20-\\u1A54\\u1AA7\\u1B05-\\u1B33\\u1B45-\\u1B4B\\u1B83-\\u1BA0\\u1BAE\\u1BAF\\u1BBA-\\u1BE5\\u1C00-\\u1C23\\u1C4D-\\u1C4F\\u1C5A-\\u1C7D\\u1CE9-\\u1CEC\\u1CEE-\\u1CF1\\u1CF5\\u1CF6\\u1D00-\\u1DBF\\u1E00-\\u1F15\\u1F18-\\u1F1D\\u1F20-\\u1F45\\u1F48-\\u1F4D\\u1F50-\\u1F57\\u1F59\\u1F5B\\u1F5D\\u1F5F-\\u1F7D\\u1F80-\\u1FB4\\u1FB6-\\u1FBC\\u1FBE\\u1FC2-\\u1FC4\\u1FC6-\\u1FCC\\u1FD0-\\u1FD3\\u1FD6-\\u1FDB\\u1FE0-\\u1FEC\\u1FF2-\\u1FF4\\u1FF6-\\u1FFC\\u2071\\u207F\\u2090-\\u209C\\u2102\\u2107\\u210A-\\u2113\\u2115\\u2119-\\u211D\\u2124\\u2126\\u2128\\u212A-\\u212D\\u212F-\\u2139\\u213C-\\u213F\\u2145-\\u2149\\u214E\\u2160-\\u2188\\u2C00-\\u2C2E\\u2C30-\\u2C5E\\u2C60-\\u2CE4\\u2CEB-\\u2CEE\\u2CF2\\u2CF3\\u2D00-\\u2D25\\u2D27\\u2D2D\\u2D30-\\u2D67\\u2D6F\\u2D80-\\u2D96\\u2DA0-\\u2DA6\\u2DA8-\\u2DAE\\u2DB0-\\u2DB6\\u2DB8-\\u2DBE\\u2DC0-\\u2DC6\\u2DC8-\\u2DCE\\u2DD0-\\u2DD6\\u2DD8-\\u2DDE\\u2E2F\\u3005-\\u3007\\u3021-\\u3029\\u3031-\\u3035\\u3038-\\u303C\\u3041-\\u3096\\u309D-\\u309F\\u30A1-\\u30FA\\u30FC-\\u30FF\\u3105-\\u312D\\u3131-\\u318E\\u31A0-\\u31BA\\u31F0-\\u31FF\\u3400-\\u4DB5\\u4E00-\\u9FCC\\uA000-\\uA48C\\uA4D0-\\uA4FD\\uA500-\\uA60C\\uA610-\\uA61F\\uA62A\\uA62B\\uA640-\\uA66E\\uA67F-\\uA69D\\uA6A0-\\uA6EF\\uA717-\\uA71F\\uA722-\\uA788\\uA78B-\\uA78E\\uA790-\\uA7AD\\uA7B0\\uA7B1\\uA7F7-\\uA801\\uA803-\\uA805\\uA807-\\uA80A\\uA80C-\\uA822\\uA840-\\uA873\\uA882-\\uA8B3\\uA8F2-\\uA8F7\\uA8FB\\uA90A-\\uA925\\uA930-\\uA946\\uA960-\\uA97C\\uA984-\\uA9B2\\uA9CF\\uA9E0-\\uA9E4\\uA9E6-\\uA9EF\\uA9FA-\\uA9FE\\uAA00-\\uAA28\\uAA40-\\uAA42\\uAA44-\\uAA4B\\uAA60-\\uAA76\\uAA7A\\uAA7E-\\uAAAF\\uAAB1\\uAAB5\\uAAB6\\uAAB9-\\uAABD\\uAAC0\\uAAC2\\uAADB-\\uAADD\\uAAE0-\\uAAEA\\uAAF2-\\uAAF4\\uAB01-\\uAB06\\uAB09-\\uAB0E\\uAB11-\\uAB16\\uAB20-\\uAB26\\uAB28-\\uAB2E\\uAB30-\\uAB5A\\uAB5C-\\uAB5F\\uAB64\\uAB65\\uABC0-\\uABE2\\uAC00-\\uD7A3\\uD7B0-\\uD7C6\\uD7CB-\\uD7FB\\uF900-\\uFA6D\\uFA70-\\uFAD9\\uFB00-\\uFB06\\uFB13-\\uFB17\\uFB1D\\uFB1F-\\uFB28\\uFB2A-\\uFB36\\uFB38-\\uFB3C\\uFB3E\\uFB40\\uFB41\\uFB43\\uFB44\\uFB46-\\uFBB1\\uFBD3-\\uFD3D\\uFD50-\\uFD8F\\uFD92-\\uFDC7\\uFDF0-\\uFDFB\\uFE70-\\uFE74\\uFE76-\\uFEFC\\uFF21-\\uFF3A\\uFF41-\\uFF5A\\uFF66-\\uFFBE\\uFFC2-\\uFFC7\\uFFCA-\\uFFCF\\uFFD2-\\uFFD7\\uFFDA-\\uFFDC]');
    }());
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/location', [
        'kendo.drawing',
        'util/main'
    ], f);
}(function () {
    (function ($, undefined) {
        var math = Math, abs = math.abs, atan = math.atan, atan2 = math.atan2, cos = math.cos, max = math.max, min = math.min, sin = math.sin, tan = math.tan, kendo = window.kendo, Class = kendo.Class, dataviz = kendo.dataviz, deepExtend = kendo.deepExtend, util = kendo.drawing.util, defined = util.defined, deg = util.deg, rad = util.rad, round = util.round, valueOrDefault = util.valueOrDefault, sqr = kendo.util.sqr;
        var Location = Class.extend({
            init: function (lat, lng) {
                if (arguments.length === 1) {
                    this.lat = lat[0];
                    this.lng = lat[1];
                } else {
                    this.lat = lat;
                    this.lng = lng;
                }
            },
            DISTANCE_ITERATIONS: 100,
            DISTANCE_CONVERGENCE: 1e-12,
            DISTANCE_PRECISION: 2,
            FORMAT: '{0:N6},{1:N6}',
            toArray: function () {
                return [
                    this.lat,
                    this.lng
                ];
            },
            equals: function (loc) {
                return loc && loc.lat === this.lat && loc.lng === this.lng;
            },
            clone: function () {
                return new Location(this.lat, this.lng);
            },
            round: function (precision) {
                this.lng = round(this.lng, precision);
                this.lat = round(this.lat, precision);
                return this;
            },
            wrap: function () {
                this.lng = this.lng % 180;
                this.lat = this.lat % 90;
                return this;
            },
            distanceTo: function (dest, datum) {
                return this.greatCircleTo(dest, datum).distance;
            },
            destination: function (distance, bearing, datum) {
                bearing = rad(bearing);
                datum = datum || dataviz.map.datums.WGS84;
                var fromLat = rad(this.lat);
                var fromLng = rad(this.lng);
                var dToR = distance / kendo.dataviz.map.datums.WGS84.a;
                var lat = math.asin(sin(fromLat) * cos(dToR) + cos(fromLat) * sin(dToR) * cos(bearing));
                var lng = fromLng + atan2(sin(bearing) * sin(dToR) * cos(fromLat), cos(dToR) - sin(fromLat) * sin(lat));
                return new Location(deg(lat), deg(lng));
            },
            greatCircleTo: function (dest, datum) {
                dest = Location.create(dest);
                datum = datum || dataviz.map.datums.WGS84;
                if (!dest || this.clone().round(8).equals(dest.clone().round(8))) {
                    return {
                        distance: 0,
                        azimuthFrom: 0,
                        azimuthTo: 0
                    };
                }
                var a = datum.a;
                var b = datum.b;
                var f = datum.f;
                var L = rad(dest.lng - this.lng);
                var U1 = atan((1 - f) * tan(rad(this.lat)));
                var sinU1 = sin(U1);
                var cosU1 = cos(U1);
                var U2 = atan((1 - f) * tan(rad(dest.lat)));
                var sinU2 = sin(U2);
                var cosU2 = cos(U2);
                var lambda = L;
                var prevLambda;
                var i = this.DISTANCE_ITERATIONS;
                var converged = false;
                var sinLambda;
                var cosLambda;
                var sino;
                var cosA2;
                var coso;
                var cos2om;
                var sigma;
                while (!converged && i-- > 0) {
                    sinLambda = sin(lambda);
                    cosLambda = cos(lambda);
                    sino = math.sqrt(sqr(cosU2 * sinLambda) + sqr(cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
                    coso = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
                    sigma = atan2(sino, coso);
                    var sinA = cosU1 * cosU2 * sinLambda / sino;
                    cosA2 = 1 - sqr(sinA);
                    cos2om = 0;
                    if (cosA2 !== 0) {
                        cos2om = coso - 2 * sinU1 * sinU2 / cosA2;
                    }
                    prevLambda = lambda;
                    var C = f / 16 * cosA2 * (4 + f * (4 - 3 * cosA2));
                    lambda = L + (1 - C) * f * sinA * (sigma + C * sino * (cos2om + C * coso * (-1 + 2 * sqr(cos2om))));
                    converged = abs(lambda - prevLambda) <= this.DISTANCE_CONVERGENCE;
                }
                var u2 = cosA2 * (sqr(a) - sqr(b)) / sqr(b);
                var A = 1 + u2 / 16384 * (4096 + u2 * (-768 + u2 * (320 - 175 * u2)));
                var B = u2 / 1024 * (256 + u2 * (-128 + u2 * (74 - 47 * u2)));
                var deltao = B * sino * (cos2om + B / 4 * (coso * (-1 + 2 * sqr(cos2om)) - B / 6 * cos2om * (-3 + 4 * sqr(sino)) * (-3 + 4 * sqr(cos2om))));
                var azimuthFrom = atan2(cosU2 * sinLambda, cosU1 * sinU2 - sinU1 * cosU2 * cosLambda);
                var azimuthTo = atan2(cosU1 * sinLambda, -sinU1 * cosU2 + cosU1 * sinU2 * cosLambda);
                return {
                    distance: round(b * A * (sigma - deltao), this.DISTANCE_PRECISION),
                    azimuthFrom: deg(azimuthFrom),
                    azimuthTo: deg(azimuthTo)
                };
            }
        });
        Location.fn.toString = function () {
            return kendo.format(this.FORMAT, this.lat, this.lng);
        };
        Location.fromLngLat = function (ll) {
            return new Location(ll[1], ll[0]);
        };
        Location.fromLatLng = function (ll) {
            return new Location(ll[0], ll[1]);
        };
        Location.create = function (a, b) {
            if (defined(a)) {
                if (a instanceof Location) {
                    return a.clone();
                } else if (arguments.length === 1 && a.length === 2) {
                    return Location.fromLatLng(a);
                } else {
                    return new Location(a, b);
                }
            }
        };
        var Extent = Class.extend({
            init: function (nw, se) {
                nw = Location.create(nw);
                se = Location.create(se);
                if (nw.lng + 180 > se.lng + 180 && nw.lat + 90 < se.lat + 90) {
                    this.se = nw;
                    this.nw = se;
                } else {
                    this.se = se;
                    this.nw = nw;
                }
            },
            contains: function (loc) {
                var nw = this.nw, se = this.se, lng = valueOrDefault(loc.lng, loc[1]), lat = valueOrDefault(loc.lat, loc[0]);
                return loc && lng + 180 >= nw.lng + 180 && lng + 180 <= se.lng + 180 && lat + 90 >= se.lat + 90 && lat + 90 <= nw.lat + 90;
            },
            center: function () {
                var nw = this.nw;
                var se = this.se;
                var lng = nw.lng + (se.lng - nw.lng) / 2;
                var lat = nw.lat + (se.lat - nw.lat) / 2;
                return new Location(lat, lng);
            },
            containsAny: function (locs) {
                var result = false;
                for (var i = 0; i < locs.length; i++) {
                    result = result || this.contains(locs[i]);
                }
                return result;
            },
            include: function (loc) {
                var nw = this.nw, se = this.se, lng = valueOrDefault(loc.lng, loc[1]), lat = valueOrDefault(loc.lat, loc[0]);
                nw.lng = min(nw.lng, lng);
                nw.lat = max(nw.lat, lat);
                se.lng = max(se.lng, lng);
                se.lat = min(se.lat, lat);
            },
            includeAll: function (locs) {
                for (var i = 0; i < locs.length; i++) {
                    this.include(locs[i]);
                }
            },
            edges: function () {
                var nw = this.nw, se = this.se;
                return {
                    nw: this.nw,
                    ne: new Location(nw.lat, se.lng),
                    se: this.se,
                    sw: new Location(se.lat, nw.lng)
                };
            },
            toArray: function () {
                var nw = this.nw, se = this.se;
                return [
                    nw,
                    new Location(nw.lat, se.lng),
                    se,
                    new Location(se.lat, nw.lng)
                ];
            },
            overlaps: function (extent) {
                return this.containsAny(extent.toArray()) || extent.containsAny(this.toArray());
            }
        });
        Extent.World = new Extent([
            90,
            -180
        ], [
            -90,
            180
        ]);
        Extent.create = function (a, b) {
            if (a instanceof Extent) {
                return a;
            } else if (a && b) {
                return new Extent(a, b);
            } else if (a && a.length === 4 && !b) {
                return new Extent([
                    a[0],
                    a[1]
                ], [
                    a[2],
                    a[3]
                ]);
            }
        };
        deepExtend(dataviz, {
            map: {
                Extent: Extent,
                Location: Location
            }
        });
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/attribution', ['kendo.drawing'], f);
}(function () {
    (function () {
        var kendo = window.kendo, Widget = kendo.ui.Widget, template = kendo.template, util = kendo.drawing.util, valueOrDefault = util.valueOrDefault, defined = util.defined;
        var Attribution = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                this._initOptions(options);
                this.items = [];
                this.element.addClass('k-widget k-attribution');
            },
            options: {
                name: 'Attribution',
                separator: '&nbsp;|&nbsp;',
                itemTemplate: '#= text #'
            },
            filter: function (extent, zoom) {
                this._extent = extent;
                this._zoom = zoom;
                this._render();
            },
            add: function (item) {
                if (defined(item)) {
                    if (typeof item === 'string') {
                        item = { text: item };
                    }
                    this.items.push(item);
                    this._render();
                }
            },
            remove: function (text) {
                var result = [];
                for (var i = 0; i < this.items.length; i++) {
                    var item = this.items[i];
                    if (item.text !== text) {
                        result.push(item);
                    }
                }
                this.items = result;
                this._render();
            },
            clear: function () {
                this.items = [];
                this.element.empty();
            },
            _render: function () {
                var result = [];
                var itemTemplate = template(this.options.itemTemplate);
                for (var i = 0; i < this.items.length; i++) {
                    var item = this.items[i];
                    var text = this._itemText(item);
                    if (text !== '') {
                        result.push(itemTemplate({ text: text }));
                    }
                }
                if (result.length > 0) {
                    this.element.empty().append(result.join(this.options.separator)).show();
                } else {
                    this.element.hide();
                }
            },
            _itemText: function (item) {
                var text = '';
                var inZoomLevel = this._inZoomLevel(item.minZoom, item.maxZoom);
                var inArea = this._inArea(item.extent);
                if (inZoomLevel && inArea) {
                    text += item.text;
                }
                return text;
            },
            _inZoomLevel: function (min, max) {
                var result = true;
                min = valueOrDefault(min, -Number.MAX_VALUE);
                max = valueOrDefault(max, Number.MAX_VALUE);
                result = this._zoom > min && this._zoom < max;
                return result;
            },
            _inArea: function (area) {
                var result = true;
                if (area) {
                    result = area.contains(this._extent);
                }
                return result;
            }
        });
        kendo.dataviz.ui.plugin(Attribution);
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/navigator', ['kendo.core'], f);
}(function () {
    (function ($) {
        var kendo = window.kendo;
        var Widget = kendo.ui.Widget;
        var keys = kendo.keys;
        var proxy = $.proxy;
        var NS = '.kendoNavigator';
        function button(dir) {
            return kendo.format('<button class="k-button k-navigator-{0}" aria-label="move {0}">' + '<span class="k-icon k-i-arrow-60-{0}"/>' + '</button>', dir);
        }
        var BUTTONS = button('up') + button('right') + button('down') + button('left');
        var Navigator = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                this._initOptions(options);
                this.element.addClass('k-widget k-header k-shadow k-navigator').append(BUTTONS).on('click' + NS, '.k-button', proxy(this, '_click'));
                var parentElement = this.element.parent().closest('[' + kendo.attr('role') + ']');
                this._keyroot = parentElement.length > 0 ? parentElement : this.element;
                this._tabindex(this._keyroot);
                this._keydown = proxy(this._keydown, this);
                this._keyroot.on('keydown', this._keydown);
            },
            options: {
                name: 'Navigator',
                panStep: 1
            },
            events: ['pan'],
            dispose: function () {
                this._keyroot.off('keydown', this._keydown);
            },
            _pan: function (x, y) {
                var panStep = this.options.panStep;
                this.trigger('pan', {
                    x: x * panStep,
                    y: y * panStep
                });
            },
            _click: function (e) {
                var x = 0;
                var y = 0;
                var button = $(e.currentTarget);
                if (button.is('.k-navigator-up')) {
                    y = 1;
                } else if (button.is('.k-navigator-down')) {
                    y = -1;
                } else if (button.is('.k-navigator-right')) {
                    x = 1;
                } else if (button.is('.k-navigator-left')) {
                    x = -1;
                }
                this._pan(x, y);
                e.preventDefault();
            },
            _keydown: function (e) {
                switch (e.which) {
                case keys.UP:
                    this._pan(0, 1);
                    e.preventDefault();
                    break;
                case keys.DOWN:
                    this._pan(0, -1);
                    e.preventDefault();
                    break;
                case keys.RIGHT:
                    this._pan(1, 0);
                    e.preventDefault();
                    break;
                case keys.LEFT:
                    this._pan(-1, 0);
                    e.preventDefault();
                    break;
                }
            }
        });
        kendo.dataviz.ui.plugin(Navigator);
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/zoom', ['kendo.core'], f);
}(function () {
    (function ($) {
        var kendo = window.kendo;
        var Widget = kendo.ui.Widget;
        var keys = kendo.keys;
        var proxy = $.proxy;
        function button(dir, iconClass) {
            return kendo.format('<button class="k-button k-zoom-{0}" title="zoom-{0}" aria-label="zoom-{0}"><span class="k-icon {1}"></span></button>', dir, iconClass);
        }
        var NS = '.kendoZoomControl';
        var BUTTONS = button('in', 'k-i-plus') + button('out', 'k-i-minus');
        var PLUS = 187;
        var MINUS = 189;
        var FF_PLUS = 61;
        var FF_MINUS = 173;
        var ZoomControl = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                this._initOptions(options);
                this.element.addClass('k-widget k-zoom-control k-button-wrap k-buttons-horizontal k-button-group k-group-horizontal').append(BUTTONS).on('click' + NS, '.k-button', proxy(this, '_click'));
                var parentElement = this.element.parent().closest('[' + kendo.attr('role') + ']');
                this._keyroot = parentElement.length > 0 ? parentElement : this.element;
                this._tabindex(this._keyroot);
                this._keydown = proxy(this._keydown, this);
                this._keyroot.on('keydown', this._keydown);
            },
            options: {
                name: 'ZoomControl',
                zoomStep: 1
            },
            events: ['change'],
            _change: function (dir) {
                var zoomStep = this.options.zoomStep;
                this.trigger('change', { delta: dir * zoomStep });
            },
            _click: function (e) {
                var button = $(e.currentTarget);
                var dir = 1;
                if (button.is('.k-zoom-out')) {
                    dir = -1;
                }
                this._change(dir);
                e.preventDefault();
            },
            _keydown: function (e) {
                switch (e.which) {
                case keys.NUMPAD_PLUS:
                case PLUS:
                case FF_PLUS:
                    this._change(1);
                    break;
                case keys.NUMPAD_MINUS:
                case MINUS:
                case FF_MINUS:
                    this._change(-1);
                    break;
                }
            }
        });
        kendo.dataviz.ui.plugin(ZoomControl);
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/crs', [
        'dataviz/map/location',
        'kendo.drawing'
    ], f);
}(function () {
    (function ($, undefined) {
        var math = Math, atan = math.atan, exp = math.exp, pow = math.pow, sin = math.sin, log = math.log, tan = math.tan, kendo = window.kendo, Class = kendo.Class, dataviz = kendo.dataviz, deepExtend = kendo.deepExtend, g = kendo.geometry, Point = g.Point, map = dataviz.map, Location = map.Location, util = kendo.drawing.util, rad = util.rad, deg = util.deg, limit = util.limitValue;
        var PI = math.PI, PI_DIV_2 = PI / 2, PI_DIV_4 = PI / 4, DEG_TO_RAD = PI / 180;
        var WGS84 = {
            a: 6378137,
            b: 6356752.314245179,
            f: 0.0033528106647474805,
            e: 0.08181919084262149
        };
        var Mercator = Class.extend({
            init: function (options) {
                this._initOptions(options);
            },
            MAX_LNG: 180,
            MAX_LAT: 85.0840590501,
            INVERSE_ITERATIONS: 15,
            INVERSE_CONVERGENCE: 1e-12,
            options: {
                centralMeridian: 0,
                datum: WGS84
            },
            forward: function (loc, clamp) {
                var proj = this, options = proj.options, datum = options.datum, r = datum.a, lng0 = options.centralMeridian, lat = limit(loc.lat, -proj.MAX_LAT, proj.MAX_LAT), lng = clamp ? limit(loc.lng, -proj.MAX_LNG, proj.MAX_LNG) : loc.lng, x = rad(lng - lng0) * r, y = proj._projectLat(lat);
                return new Point(x, y);
            },
            _projectLat: function (lat) {
                var datum = this.options.datum, ecc = datum.e, r = datum.a, y = rad(lat), ts = tan(PI_DIV_4 + y / 2), con = ecc * sin(y), p = pow((1 - con) / (1 + con), ecc / 2);
                return r * log(ts * p);
            },
            inverse: function (point, clamp) {
                var proj = this, options = proj.options, datum = options.datum, r = datum.a, lng0 = options.centralMeridian, lng = point.x / (DEG_TO_RAD * r) + lng0, lat = limit(proj._inverseY(point.y), -proj.MAX_LAT, proj.MAX_LAT);
                if (clamp) {
                    lng = limit(lng, -proj.MAX_LNG, proj.MAX_LNG);
                }
                return new Location(lat, lng);
            },
            _inverseY: function (y) {
                var proj = this, datum = proj.options.datum, r = datum.a, ecc = datum.e, ecch = ecc / 2, ts = exp(-y / r), phi = PI_DIV_2 - 2 * atan(ts), i;
                for (i = 0; i <= proj.INVERSE_ITERATIONS; i++) {
                    var con = ecc * sin(phi), p = pow((1 - con) / (1 + con), ecch), dphi = PI_DIV_2 - 2 * atan(ts * p) - phi;
                    phi += dphi;
                    if (math.abs(dphi) <= proj.INVERSE_CONVERGENCE) {
                        break;
                    }
                }
                return deg(phi);
            }
        });
        var SphericalMercator = Mercator.extend({
            MAX_LAT: 85.0511287798,
            _projectLat: function (lat) {
                var r = this.options.datum.a, y = rad(lat), ts = tan(PI_DIV_4 + y / 2);
                return r * log(ts);
            },
            _inverseY: function (y) {
                var r = this.options.datum.a, ts = exp(-y / r);
                return deg(PI_DIV_2 - 2 * atan(ts));
            }
        });
        var Equirectangular = Class.extend({
            forward: function (loc) {
                return new Point(loc.lng, loc.lat);
            },
            inverse: function (point) {
                return new Location(point.y, point.x);
            }
        });
        var EPSG3857 = Class.extend({
            init: function () {
                var crs = this, proj = crs._proj = new SphericalMercator();
                var c = this.c = 2 * PI * proj.options.datum.a;
                this._tm = g.transform().translate(0.5, 0.5).scale(1 / c, -1 / c);
                this._itm = g.transform().scale(c, -c).translate(-0.5, -0.5);
            },
            toPoint: function (loc, scale, clamp) {
                var point = this._proj.forward(loc, clamp);
                return point.transform(this._tm).scale(scale || 1);
            },
            toLocation: function (point, scale, clamp) {
                point = point.clone().scale(1 / (scale || 1)).transform(this._itm);
                return this._proj.inverse(point, clamp);
            }
        });
        var EPSG3395 = Class.extend({
            init: function () {
                this._proj = new Mercator();
            },
            toPoint: function (loc) {
                return this._proj.forward(loc);
            },
            toLocation: function (point) {
                return this._proj.inverse(point);
            }
        });
        var EPSG4326 = Class.extend({
            init: function () {
                this._proj = new Equirectangular();
            },
            toPoint: function (loc) {
                return this._proj.forward(loc);
            },
            toLocation: function (point) {
                return this._proj.inverse(point);
            }
        });
        deepExtend(dataviz, {
            map: {
                crs: {
                    EPSG3395: EPSG3395,
                    EPSG3857: EPSG3857,
                    EPSG4326: EPSG4326
                },
                datums: { WGS84: WGS84 },
                projections: {
                    Equirectangular: Equirectangular,
                    Mercator: Mercator,
                    SphericalMercator: SphericalMercator
                }
            }
        });
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/layers/base', [
        'kendo.core',
        'dataviz/map/location'
    ], f);
}(function () {
    (function ($, undefined) {
        var proxy = $.proxy, kendo = window.kendo, Class = kendo.Class, dataviz = kendo.dataviz, deepExtend = kendo.deepExtend, Extent = dataviz.map.Extent, util = kendo.drawing.util, defined = util.defined;
        var Layer = Class.extend({
            init: function (map, options) {
                this._initOptions(options);
                this.map = map;
                this.element = $('<div class=\'k-layer\'></div>').css({
                    'zIndex': this.options.zIndex,
                    'opacity': this.options.opacity
                }).appendTo(map.scrollElement);
                this._beforeReset = proxy(this._beforeReset, this);
                this._reset = proxy(this._reset, this);
                this._resize = proxy(this._resize, this);
                this._panEnd = proxy(this._panEnd, this);
                this._activate();
                this._updateAttribution();
            },
            destroy: function () {
                this._deactivate();
            },
            show: function () {
                this.reset();
                this._activate();
                this._applyExtent(true);
            },
            hide: function () {
                this._deactivate();
                this._setVisibility(false);
            },
            reset: function () {
                this._beforeReset();
                this._reset();
            },
            _reset: function () {
                this._applyExtent();
            },
            _beforeReset: $.noop,
            _resize: $.noop,
            _panEnd: function () {
                this._applyExtent();
            },
            _applyExtent: function () {
                var options = this.options;
                var zoom = this.map.zoom();
                var matchMinZoom = !defined(options.minZoom) || zoom >= options.minZoom;
                var matchMaxZoom = !defined(options.maxZoom) || zoom <= options.maxZoom;
                var extent = Extent.create(options.extent);
                var inside = !extent || extent.overlaps(this.map.extent());
                this._setVisibility(matchMinZoom && matchMaxZoom && inside);
            },
            _setVisibility: function (visible) {
                this.element.css('display', visible ? '' : 'none');
            },
            _activate: function () {
                var map = this.map;
                map.bind('beforeReset', this._beforeReset);
                map.bind('reset', this._reset);
                map.bind('resize', this._resize);
                map.bind('panEnd', this._panEnd);
            },
            _deactivate: function () {
                var map = this.map;
                map.unbind('beforeReset', this._beforeReset);
                map.unbind('reset', this._reset);
                map.unbind('resize', this._resize);
                map.unbind('panEnd', this._panEnd);
            },
            _updateAttribution: function () {
                var attr = this.map.attribution;
                if (attr) {
                    attr.add(this.options.attribution);
                }
            }
        });
        deepExtend(dataviz, { map: { layers: { Layer: Layer } } });
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/layers/shape', [
        'dataviz/map/layers/base',
        'dataviz/map/location'
    ], f);
}(function () {
    (function ($, undefined) {
        var proxy = $.proxy, kendo = window.kendo, Class = kendo.Class, DataSource = kendo.data.DataSource, dataviz = kendo.dataviz, deepExtend = kendo.deepExtend, g = kendo.geometry, d = kendo.drawing, Group = d.Group, last = d.util.last, defined = d.util.defined, map = dataviz.map, Location = map.Location, Layer = map.layers.Layer;
        var ShapeLayer = Layer.extend({
            init: function (map, options) {
                this._pan = proxy(this._pan, this);
                Layer.fn.init.call(this, map, options);
                this.surface = d.Surface.create(this.element, {
                    width: map.scrollElement.width(),
                    height: map.scrollElement.height()
                });
                this._initRoot();
                this.movable = new kendo.ui.Movable(this.surface.element);
                this._markers = [];
                this._click = this._handler('shapeClick');
                this.surface.bind('click', this._click);
                this._mouseenter = this._handler('shapeMouseEnter');
                this.surface.bind('mouseenter', this._mouseenter);
                this._mouseleave = this._handler('shapeMouseLeave');
                this.surface.bind('mouseleave', this._mouseleave);
                this._initDataSource();
            },
            options: { autoBind: true },
            destroy: function () {
                Layer.fn.destroy.call(this);
                this.surface.destroy();
                this.dataSource.unbind('change', this._dataChange);
            },
            setDataSource: function (dataSource) {
                if (this.dataSource) {
                    this.dataSource.unbind('change', this._dataChange);
                }
                this.dataSource = kendo.data.DataSource.create(dataSource);
                this.dataSource.bind('change', this._dataChange);
                if (this.options.autoBind) {
                    this.dataSource.fetch();
                }
            },
            _reset: function () {
                Layer.fn._reset.call(this);
                this._translateSurface();
                if (this._data) {
                    this._load(this._data);
                }
            },
            _initRoot: function () {
                this._root = new Group();
                this.surface.draw(this._root);
            },
            _beforeReset: function () {
                this.surface.clear();
                this._initRoot();
            },
            _resize: function () {
                this.surface.size(this.map.size());
            },
            _initDataSource: function () {
                var dsOptions = this.options.dataSource;
                this._dataChange = proxy(this._dataChange, this);
                this.dataSource = DataSource.create(dsOptions).bind('change', this._dataChange);
                if (dsOptions && this.options.autoBind) {
                    this.dataSource.fetch();
                }
            },
            _dataChange: function (e) {
                this._data = e.sender.view();
                this._load(this._data);
            },
            _load: function (data) {
                this._clearMarkers();
                if (!this._loader) {
                    this._loader = new GeoJSONLoader(this.map, this.options.style, this);
                }
                var container = new Group();
                for (var i = 0; i < data.length; i++) {
                    var shape = this._loader.parse(data[i]);
                    if (shape) {
                        container.append(shape);
                    }
                }
                this._root.clear();
                this._root.append(container);
            },
            shapeCreated: function (shape) {
                var cancelled = false;
                if (shape instanceof d.Circle) {
                    cancelled = defined(this._createMarker(shape));
                }
                if (!cancelled) {
                    var args = {
                        layer: this,
                        shape: shape
                    };
                    cancelled = this.map.trigger('shapeCreated', args);
                }
                return cancelled;
            },
            featureCreated: function (e) {
                e.layer = this;
                this.map.trigger('shapeFeatureCreated', e);
            },
            _createMarker: function (shape) {
                var marker = this.map.markers.bind({ location: shape.location }, shape.dataItem);
                if (marker) {
                    this._markers.push(marker);
                }
                return marker;
            },
            _clearMarkers: function () {
                for (var i = 0; i < this._markers.length; i++) {
                    this.map.markers.remove(this._markers[i]);
                }
                this._markers = [];
            },
            _pan: function () {
                if (!this._panning) {
                    this._panning = true;
                    this.surface.suspendTracking();
                }
            },
            _panEnd: function (e) {
                Layer.fn._panEnd.call(this, e);
                this._translateSurface();
                this.surface.resumeTracking();
                this._panning = false;
            },
            _translateSurface: function () {
                var map = this.map;
                var nw = map.locationToView(map.extent().nw);
                if (this.surface.translate) {
                    this.surface.translate(nw);
                    this.movable.moveTo({
                        x: nw.x,
                        y: nw.y
                    });
                }
            },
            _handler: function (event) {
                var layer = this;
                return function (e) {
                    if (e.element) {
                        var args = {
                            layer: layer,
                            shape: e.element,
                            originalEvent: e.originalEvent
                        };
                        layer.map.trigger(event, args);
                    }
                };
            },
            _activate: function () {
                Layer.fn._activate.call(this);
                this.map.bind('pan', this._pan);
            },
            _deactivate: function () {
                Layer.fn._deactivate.call(this);
                this.map.unbind('pan', this._pan);
            }
        });
        var GeoJSONLoader = Class.extend({
            init: function (locator, defaultStyle, observer) {
                this.observer = observer;
                this.locator = locator;
                this.style = defaultStyle;
            },
            parse: function (item) {
                var root = new Group();
                var unwrap = true;
                if (item.type === 'Feature') {
                    unwrap = false;
                    this._loadGeometryTo(root, item.geometry, item);
                    this._featureCreated(root, item);
                } else {
                    this._loadGeometryTo(root, item, item);
                }
                if (unwrap && root.children.length < 2) {
                    root = root.children[0];
                }
                return root;
            },
            _shapeCreated: function (shape) {
                var cancelled = false;
                if (this.observer && this.observer.shapeCreated) {
                    cancelled = this.observer.shapeCreated(shape);
                }
                return cancelled;
            },
            _featureCreated: function (group, dataItem) {
                if (this.observer && this.observer.featureCreated) {
                    this.observer.featureCreated({
                        group: group,
                        dataItem: dataItem,
                        properties: dataItem.properties
                    });
                }
            },
            _loadGeometryTo: function (container, geometry, dataItem) {
                var coords = geometry.coordinates;
                var i;
                var path;
                switch (geometry.type) {
                case 'LineString':
                    path = this._loadPolygon(container, [coords], dataItem);
                    this._setLineFill(path);
                    break;
                case 'MultiLineString':
                    for (i = 0; i < coords.length; i++) {
                        path = this._loadPolygon(container, [coords[i]], dataItem);
                        this._setLineFill(path);
                    }
                    break;
                case 'Polygon':
                    this._loadPolygon(container, coords, dataItem);
                    break;
                case 'MultiPolygon':
                    for (i = 0; i < coords.length; i++) {
                        this._loadPolygon(container, coords[i], dataItem);
                    }
                    break;
                case 'Point':
                    this._loadPoint(container, coords, dataItem);
                    break;
                case 'MultiPoint':
                    for (i = 0; i < coords.length; i++) {
                        this._loadPoint(container, coords[i], dataItem);
                    }
                    break;
                }
            },
            _setLineFill: function (path) {
                var segments = path.segments;
                if (segments.length < 4 || !segments[0].anchor().equals(last(segments).anchor())) {
                    path.options.fill = null;
                }
            },
            _loadShape: function (container, shape) {
                if (!this._shapeCreated(shape)) {
                    container.append(shape);
                }
                return shape;
            },
            _loadPolygon: function (container, rings, dataItem) {
                var shape = this._buildPolygon(rings);
                shape.dataItem = dataItem;
                return this._loadShape(container, shape);
            },
            _buildPolygon: function (rings) {
                var type = rings.length > 1 ? d.MultiPath : d.Path;
                var path = new type(this.style);
                for (var i = 0; i < rings.length; i++) {
                    for (var j = 0; j < rings[i].length; j++) {
                        var point = this.locator.locationToView(Location.fromLngLat(rings[i][j]));
                        if (j === 0) {
                            path.moveTo(point.x, point.y);
                        } else {
                            path.lineTo(point.x, point.y);
                        }
                    }
                }
                return path;
            },
            _loadPoint: function (container, coords, dataItem) {
                var location = Location.fromLngLat(coords);
                var point = this.locator.locationToView(location);
                var circle = new g.Circle(point, 10);
                var shape = new d.Circle(circle, this.style);
                shape.dataItem = dataItem;
                shape.location = location;
                return this._loadShape(container, shape);
            }
        });
        deepExtend(kendo.data, {
            schemas: {
                geojson: {
                    type: 'json',
                    data: function (data) {
                        if (data.type === 'FeatureCollection') {
                            return data.features;
                        }
                        if (data.type === 'GeometryCollection') {
                            return data.geometries;
                        }
                        return data;
                    }
                }
            },
            transports: { geojson: { read: { dataType: 'json' } } }
        });
        deepExtend(dataviz, {
            map: {
                layers: {
                    shape: ShapeLayer,
                    ShapeLayer: ShapeLayer
                },
                GeoJSONLoader: GeoJSONLoader
            }
        });
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/layers/bubble', ['dataviz/map/layers/shape'], f);
}(function () {
    (function ($, undefined) {
        var kendo = window.kendo, getter = kendo.getter, dataviz = kendo.dataviz, deepExtend = kendo.deepExtend, g = kendo.geometry, d = kendo.drawing, util = d.util, defined = util.defined, map = dataviz.map, Location = map.Location, ShapeLayer = map.layers.ShapeLayer;
        var BubbleLayer = ShapeLayer.extend({
            options: {
                autoBind: true,
                locationField: 'location',
                valueField: 'value',
                minSize: 0,
                maxSize: 100,
                scale: 'sqrt',
                symbol: 'circle'
            },
            _load: function (data) {
                this.surface.clear();
                if (data.length === 0) {
                    return;
                }
                var opt = this.options;
                var getValue = getter(opt.valueField);
                data = data.slice(0);
                data.sort(function (a, b) {
                    return getValue(b) - getValue(a);
                });
                var scaleType = this._scaleType();
                var scale;
                for (var i = 0; i < data.length; i++) {
                    var dataItem = data[i];
                    var location = getter(opt.locationField)(dataItem);
                    var value = getter(opt.valueField)(dataItem);
                    if (defined(location) && defined(value)) {
                        if (!scale) {
                            scale = new scaleType([
                                0,
                                value
                            ], [
                                opt.minSize,
                                opt.maxSize
                            ]);
                        }
                        location = Location.create(location);
                        var center = this.map.locationToView(location);
                        var size = scale.map(value);
                        var symbol = this._createSymbol({
                            center: center,
                            size: size,
                            style: opt.style,
                            dataItem: dataItem,
                            location: location
                        });
                        symbol.dataItem = dataItem;
                        symbol.location = location;
                        symbol.value = value;
                        this._drawSymbol(symbol);
                    }
                }
            },
            _scaleType: function () {
                var scale = this.options.scale;
                if (kendo.isFunction(scale)) {
                    return scale;
                }
                return dataviz.map.scales[scale];
            },
            _createSymbol: function (args) {
                var symbol = this.options.symbol;
                if (!kendo.isFunction(symbol)) {
                    symbol = dataviz.map.symbols[symbol];
                }
                return symbol(args);
            },
            _drawSymbol: function (shape) {
                var args = {
                    layer: this,
                    shape: shape
                };
                var cancelled = this.map.trigger('shapeCreated', args);
                if (!cancelled) {
                    this.surface.draw(shape);
                }
            }
        });
        var SqrtScale = kendo.Class.extend({
            init: function (domain, range) {
                this._domain = domain;
                this._range = range;
                var domainRange = Math.sqrt(domain[1]) - Math.sqrt(domain[0]);
                var outputRange = range[1] - range[0];
                this._ratio = outputRange / domainRange;
            },
            map: function (value) {
                var rel = (Math.sqrt(value) - Math.sqrt(this._domain[0])) * this._ratio;
                return this._range[0] + rel;
            }
        });
        var Symbols = {
            circle: function (args) {
                var geo = new g.Circle(args.center, args.size / 2);
                return new d.Circle(geo, args.style);
            },
            square: function (args) {
                var path = new d.Path(args.style);
                var halfSize = args.size / 2;
                var center = args.center;
                path.moveTo(center.x - halfSize, center.y - halfSize).lineTo(center.x + halfSize, center.y - halfSize).lineTo(center.x + halfSize, center.y + halfSize).lineTo(center.x - halfSize, center.y + halfSize).close();
                return path;
            }
        };
        deepExtend(dataviz, {
            map: {
                layers: {
                    bubble: BubbleLayer,
                    BubbleLayer: BubbleLayer
                },
                scales: { sqrt: SqrtScale },
                symbols: Symbols
            }
        });
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/layers/tile', [
        'dataviz/map/layers/base',
        'dataviz/map/location'
    ], f);
}(function () {
    (function ($, undefined) {
        var math = Math, proxy = $.proxy, kendo = window.kendo, Class = kendo.Class, template = kendo.template, dataviz = kendo.dataviz, deepExtend = kendo.deepExtend, g = kendo.geometry, Point = g.Point, Layer = dataviz.map.layers.Layer, util = kendo.util, renderSize = util.renderSize, drawingUtil = kendo.drawing.util, round = drawingUtil.round, limit = drawingUtil.limitValue;
        var TileLayer = Layer.extend({
            init: function (map, options) {
                Layer.fn.init.call(this, map, options);
                if (typeof this.options.subdomains === 'string') {
                    this.options.subdomains = this.options.subdomains.split('');
                }
                var viewType = this._viewType();
                this._view = new viewType(this.element, this.options);
            },
            destroy: function () {
                Layer.fn.destroy.call(this);
                this._view.destroy();
                this._view = null;
            },
            _beforeReset: function () {
                var map = this.map;
                var origin = map.locationToLayer(map.extent().nw).round();
                this._view.viewOrigin(origin);
            },
            _reset: function () {
                Layer.fn._reset.call(this);
                this._updateView();
                this._view.reset();
            },
            _viewType: function () {
                return TileView;
            },
            _activate: function () {
                Layer.fn._activate.call(this);
                if (!kendo.support.mobileOS) {
                    if (!this._pan) {
                        this._pan = kendo.throttle(proxy(this._render, this), 100);
                    }
                    this.map.bind('pan', this._pan);
                }
            },
            _deactivate: function () {
                Layer.fn._deactivate.call(this);
                if (this._pan) {
                    this.map.unbind('pan', this._pan);
                }
            },
            _updateView: function () {
                var view = this._view, map = this.map, extent = map.extent(), extentToPoint = {
                        nw: map.locationToLayer(extent.nw).round(),
                        se: map.locationToLayer(extent.se).round()
                    };
                view.center(map.locationToLayer(map.center()));
                view.extent(extentToPoint);
                view.zoom(map.zoom());
            },
            _resize: function () {
                this._render();
            },
            _panEnd: function (e) {
                Layer.fn._panEnd.call(this, e);
                this._render();
            },
            _render: function () {
                this._updateView();
                this._view.render();
            }
        });
        var TileView = Class.extend({
            init: function (element, options) {
                this.element = element;
                this._initOptions(options);
                this.pool = new TilePool();
            },
            options: {
                tileSize: 256,
                subdomains: [
                    'a',
                    'b',
                    'c'
                ],
                urlTemplate: ''
            },
            center: function (center) {
                this._center = center;
            },
            extent: function (extent) {
                this._extent = extent;
            },
            viewOrigin: function (origin) {
                this._viewOrigin = origin;
            },
            zoom: function (zoom) {
                this._zoom = zoom;
            },
            pointToTileIndex: function (point) {
                return new Point(math.floor(point.x / this.options.tileSize), math.floor(point.y / this.options.tileSize));
            },
            tileCount: function () {
                var size = this.size(), firstTileIndex = this.pointToTileIndex(this._extent.nw), nw = this._extent.nw, point = this.indexToPoint(firstTileIndex).translate(-nw.x, -nw.y);
                return {
                    x: math.ceil((math.abs(point.x) + size.width) / this.options.tileSize),
                    y: math.ceil((math.abs(point.y) + size.height) / this.options.tileSize)
                };
            },
            size: function () {
                var nw = this._extent.nw, se = this._extent.se, diff = se.clone().translate(-nw.x, -nw.y);
                return {
                    width: diff.x,
                    height: diff.y
                };
            },
            indexToPoint: function (index) {
                var x = index.x, y = index.y;
                return new Point(x * this.options.tileSize, y * this.options.tileSize);
            },
            subdomainText: function () {
                var subdomains = this.options.subdomains;
                return subdomains[this.subdomainIndex++ % subdomains.length];
            },
            destroy: function () {
                this.element.empty();
                this.pool.empty();
            },
            reset: function () {
                this.pool.reset();
                this.subdomainIndex = 0;
                this.render();
            },
            render: function () {
                var size = this.tileCount(), firstTileIndex = this.pointToTileIndex(this._extent.nw), tile, x, y;
                for (x = 0; x < size.x; x++) {
                    for (y = 0; y < size.y; y++) {
                        tile = this.createTile({
                            x: firstTileIndex.x + x,
                            y: firstTileIndex.y + y
                        });
                        if (!tile.visible) {
                            tile.show();
                        }
                    }
                }
            },
            createTile: function (currentIndex) {
                var options = this.tileOptions(currentIndex);
                var tile = this.pool.get(this._center, options);
                if (tile.element.parent().length === 0) {
                    this.element.append(tile.element);
                }
                return tile;
            },
            tileOptions: function (currentIndex) {
                var index = this.wrapIndex(currentIndex), point = this.indexToPoint(currentIndex), origin = this._viewOrigin, offset = point.clone().translate(-origin.x, -origin.y);
                return {
                    index: index,
                    currentIndex: currentIndex,
                    point: point,
                    offset: roundPoint(offset),
                    zoom: this._zoom,
                    size: this.options.tileSize,
                    subdomain: this.subdomainText(),
                    urlTemplate: this.options.urlTemplate,
                    errorUrlTemplate: this.options.errorUrlTemplate
                };
            },
            wrapIndex: function (index) {
                var boundary = math.pow(2, this._zoom);
                return {
                    x: this.wrapValue(index.x, boundary),
                    y: limit(index.y, 0, boundary - 1)
                };
            },
            wrapValue: function (value, boundary) {
                var remainder = math.abs(value) % boundary;
                if (value >= 0) {
                    value = remainder;
                } else {
                    value = boundary - (remainder === 0 ? boundary : remainder);
                }
                return value;
            }
        });
        var ImageTile = Class.extend({
            init: function (id, options) {
                this.id = id;
                this.visible = true;
                this._initOptions(options);
                this.createElement();
                this.show();
            },
            options: {
                urlTemplate: '',
                errorUrlTemplate: ''
            },
            createElement: function () {
                this.element = $('<img style=\'position: absolute; display: block;\' alt=\'\' />').css({
                    width: this.options.size,
                    height: this.options.size
                }).on('error', proxy(function (e) {
                    if (this.errorUrl()) {
                        e.target.setAttribute('src', this.errorUrl());
                    } else {
                        e.target.removeAttribute('src');
                    }
                }, this));
            },
            show: function () {
                var element = this.element[0];
                element.style.top = renderSize(this.options.offset.y);
                element.style.left = renderSize(this.options.offset.x);
                var url = this.url();
                if (url) {
                    element.setAttribute('src', url);
                }
                element.style.visibility = 'visible';
                this.visible = true;
            },
            hide: function () {
                this.element[0].style.visibility = 'hidden';
                this.visible = false;
            },
            url: function () {
                var urlResult = template(this.options.urlTemplate);
                return urlResult(this.urlOptions());
            },
            errorUrl: function () {
                var urlResult = template(this.options.errorUrlTemplate);
                return urlResult(this.urlOptions());
            },
            urlOptions: function () {
                var options = this.options;
                return {
                    zoom: options.zoom,
                    subdomain: options.subdomain,
                    z: options.zoom,
                    x: options.index.x,
                    y: options.index.y,
                    s: options.subdomain,
                    quadkey: options.quadkey,
                    q: options.quadkey,
                    culture: options.culture,
                    c: options.culture
                };
            },
            destroy: function () {
                if (this.element) {
                    this.element.remove();
                    this.element = null;
                }
            }
        });
        var TilePool = Class.extend({
            init: function () {
                this._items = [];
            },
            options: { maxSize: 100 },
            get: function (center, options) {
                if (this._items.length >= this.options.maxSize) {
                    this._remove(center);
                }
                return this._create(options);
            },
            empty: function () {
                var items = this._items;
                for (var i = 0; i < items.length; i++) {
                    items[i].destroy();
                }
                this._items = [];
            },
            reset: function () {
                var items = this._items;
                for (var i = 0; i < items.length; i++) {
                    items[i].hide();
                }
            },
            _create: function (options) {
                var items = this._items;
                var tile;
                var id = util.hashKey(options.point.toString() + options.offset.toString() + options.zoom + options.urlTemplate);
                for (var i = 0; i < items.length; i++) {
                    if (items[i].id === id) {
                        tile = items[i];
                        break;
                    }
                }
                if (tile) {
                    tile.show();
                } else {
                    tile = new ImageTile(id, options);
                    this._items.push(tile);
                }
                return tile;
            },
            _remove: function (center) {
                var items = this._items;
                var maxDist = -1;
                var index = -1;
                for (var i = 0; i < items.length; i++) {
                    var dist = items[i].options.point.distanceTo(center);
                    if (dist > maxDist && !items[i].visible) {
                        index = i;
                        maxDist = dist;
                    }
                }
                if (index !== -1) {
                    items[index].destroy();
                    items.splice(index, 1);
                }
            }
        });
        function roundPoint(point) {
            return new Point(round(point.x), round(point.y));
        }
        deepExtend(dataviz, {
            map: {
                layers: {
                    tile: TileLayer,
                    TileLayer: TileLayer,
                    ImageTile: ImageTile,
                    TilePool: TilePool,
                    TileView: TileView
                }
            }
        });
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/layers/bing', ['dataviz/map/layers/tile'], f);
}(function () {
    (function ($, undefined) {
        var kendo = window.kendo, dataviz = kendo.dataviz, deepExtend = kendo.deepExtend, defined = kendo.drawing.util.defined, Extent = dataviz.map.Extent, Location = dataviz.map.Location, TileLayer = dataviz.map.layers.TileLayer, TileView = dataviz.map.layers.TileView;
        var BingLayer = TileLayer.extend({
            init: function (map, options) {
                this.options.baseUrl = this._scheme() + '://dev.virtualearth.net/REST/v1/Imagery/Metadata/';
                TileLayer.fn.init.call(this, map, options);
                this._onMetadata = $.proxy(this._onMetadata, this);
                this._fetchMetadata();
            },
            options: { imagerySet: 'road' },
            _fetchMetadata: function () {
                var options = this.options;
                if (!options.key) {
                    throw new Error('Bing tile layer: API key is required');
                }
                $.ajax({
                    url: options.baseUrl + options.imagerySet,
                    data: {
                        output: 'json',
                        include: 'ImageryProviders',
                        key: options.key,
                        uriScheme: this._scheme()
                    },
                    type: 'get',
                    dataType: 'jsonp',
                    jsonp: 'jsonp',
                    success: this._onMetadata
                });
            },
            _scheme: function (proto) {
                proto = proto || window.location.protocol;
                return proto.replace(':', '') === 'https' ? 'https' : 'http';
            },
            _onMetadata: function (data) {
                if (data && data.resourceSets.length) {
                    var resource = this.resource = data.resourceSets[0].resources[0];
                    deepExtend(this._view.options, {
                        urlTemplate: resource.imageUrl.replace('{subdomain}', '#= subdomain #').replace('{quadkey}', '#= quadkey #').replace('{culture}', '#= culture #'),
                        subdomains: resource.imageUrlSubdomains
                    });
                    var options = this.options;
                    if (!defined(options.minZoom)) {
                        options.minZoom = resource.zoomMin;
                    }
                    if (!defined(options.maxZoom)) {
                        options.maxZoom = resource.zoomMax;
                    }
                    this._addAttribution();
                    if (this.element.css('display') !== 'none') {
                        this._reset();
                    }
                }
            },
            _viewType: function () {
                return BingView;
            },
            _addAttribution: function () {
                var attr = this.map.attribution;
                if (attr) {
                    var items = this.resource.imageryProviders;
                    if (items) {
                        for (var i = 0; i < items.length; i++) {
                            var item = items[i];
                            for (var y = 0; y < item.coverageAreas.length; y++) {
                                var area = item.coverageAreas[y];
                                attr.add({
                                    text: item.attribution,
                                    minZoom: area.zoomMin,
                                    maxZoom: area.zoomMax,
                                    extent: new Extent(new Location(area.bbox[2], area.bbox[1]), new Location(area.bbox[0], area.bbox[3]))
                                });
                            }
                        }
                    }
                }
            },
            imagerySet: function (value) {
                if (value) {
                    this.options.imagerySet = value;
                    this.map.attribution.clear();
                    this._fetchMetadata();
                } else {
                    return this.options.imagerySet;
                }
            }
        });
        var BingView = TileView.extend({
            options: { culture: 'en-US' },
            tileOptions: function (currentIndex) {
                var options = TileView.fn.tileOptions.call(this, currentIndex);
                options.culture = this.options.culture;
                options.quadkey = this.tileQuadKey(this.wrapIndex(currentIndex));
                return options;
            },
            tileQuadKey: function (index) {
                var quadKey = '', digit, mask, i;
                for (i = this._zoom; i > 0; i--) {
                    digit = 0;
                    mask = 1 << i - 1;
                    if ((index.x & mask) !== 0) {
                        digit++;
                    }
                    if ((index.y & mask) !== 0) {
                        digit += 2;
                    }
                    quadKey += digit;
                }
                return quadKey;
            }
        });
        deepExtend(dataviz, {
            map: {
                layers: {
                    bing: BingLayer,
                    BingLayer: BingLayer,
                    BingView: BingView
                }
            }
        });
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/layers/marker', [
        'dataviz/map/layers/base',
        'dataviz/map/location',
        'kendo.data',
        'kendo.tooltip'
    ], f);
}(function () {
    (function ($, undefined) {
        var doc = document, math = Math, indexOf = $.inArray, proxy = $.proxy, kendo = window.kendo, Class = kendo.Class, DataSource = kendo.data.DataSource, Tooltip = kendo.ui.Tooltip, dataviz = kendo.dataviz, deepExtend = kendo.deepExtend, map = dataviz.map, Location = map.Location, Layer = map.layers.Layer;
        var MarkerLayer = Layer.extend({
            init: function (map, options) {
                Layer.fn.init.call(this, map, options);
                this._markerClick = proxy(this._markerClick, this);
                this.element.on('click', '.k-marker', this._markerClick);
                this.items = [];
                this._initDataSource();
            },
            destroy: function () {
                Layer.fn.destroy.call(this);
                this.element.off('click', '.k-marker', this._markerClick);
                this.dataSource.unbind('change', this._dataChange);
                this.clear();
            },
            options: {
                zIndex: 1000,
                autoBind: true,
                dataSource: {},
                locationField: 'location',
                titleField: 'title'
            },
            add: function (arg) {
                if ($.isArray(arg)) {
                    for (var i = 0; i < arg.length; i++) {
                        this._addOne(arg[i]);
                    }
                } else {
                    return this._addOne(arg);
                }
            },
            remove: function (marker) {
                marker.destroy();
                var index = indexOf(marker, this.items);
                if (index > -1) {
                    this.items.splice(index, 1);
                }
            },
            clear: function () {
                for (var i = 0; i < this.items.length; i++) {
                    this.items[i].destroy();
                }
                this.items = [];
            },
            update: function (marker) {
                var loc = marker.location();
                if (loc) {
                    marker.showAt(this.map.locationToView(loc));
                    var args = {
                        marker: marker,
                        layer: this
                    };
                    this.map.trigger('markerActivate', args);
                }
            },
            _reset: function () {
                Layer.fn._reset.call(this);
                var items = this.items;
                for (var i = 0; i < items.length; i++) {
                    this.update(items[i]);
                }
            },
            bind: function (options, dataItem) {
                var marker = map.Marker.create(options, this.options);
                marker.dataItem = dataItem;
                var args = {
                    marker: marker,
                    layer: this
                };
                var cancelled = this.map.trigger('markerCreated', args);
                if (!cancelled) {
                    this.add(marker);
                    return marker;
                }
            },
            setDataSource: function (dataSource) {
                if (this.dataSource) {
                    this.dataSource.unbind('change', this._dataChange);
                }
                this.dataSource = kendo.data.DataSource.create(dataSource);
                this.dataSource.bind('change', this._dataChange);
                if (this.options.autoBind) {
                    this.dataSource.fetch();
                }
            },
            _addOne: function (arg) {
                var marker = Marker.create(arg, this.options);
                marker.addTo(this);
                return marker;
            },
            _initDataSource: function () {
                var dsOptions = this.options.dataSource;
                this._dataChange = proxy(this._dataChange, this);
                this.dataSource = DataSource.create(dsOptions).bind('change', this._dataChange);
                if (dsOptions && this.options.autoBind) {
                    this.dataSource.fetch();
                }
            },
            _dataChange: function (e) {
                this._load(e.sender.view());
            },
            _load: function (data) {
                this._data = data;
                this.clear();
                var getLocation = kendo.getter(this.options.locationField);
                var getTitle = kendo.getter(this.options.titleField);
                for (var i = 0; i < data.length; i++) {
                    var dataItem = data[i];
                    this.bind({
                        location: getLocation(dataItem),
                        title: getTitle(dataItem)
                    }, dataItem);
                }
            },
            _markerClick: function (e) {
                var args = {
                    marker: $(e.target).data('kendoMarker'),
                    layer: this
                };
                this.map.trigger('markerClick', args);
            }
        });
        var Marker = Class.extend({
            init: function (options) {
                this.options = options || {};
            },
            addTo: function (parent) {
                this.layer = parent.markers || parent;
                this.layer.items.push(this);
                this.layer.update(this);
            },
            location: function (value) {
                if (value) {
                    this.options.location = Location.create(value).toArray();
                    if (this.layer) {
                        this.layer.update(this);
                    }
                    return this;
                } else {
                    return Location.create(this.options.location);
                }
            },
            showAt: function (point) {
                this.render();
                this.element.css({
                    left: math.round(point.x),
                    top: math.round(point.y)
                });
                if (this.tooltip && this.tooltip.popup) {
                    this.tooltip.popup._position();
                }
            },
            hide: function () {
                if (this.element) {
                    this.element.remove();
                    this.element = null;
                }
                if (this.tooltip) {
                    this.tooltip.destroy();
                    this.tooltip = null;
                }
            },
            destroy: function () {
                this.layer = null;
                this.hide();
            },
            render: function () {
                if (!this.element) {
                    var options = this.options;
                    var layer = this.layer;
                    this.element = $(doc.createElement('span')).addClass('k-marker k-icon k-i-marker-' + kendo.toHyphens(options.shape || 'pin')).attr('title', options.title).attr(options.attributes || {}).data('kendoMarker', this).css('zIndex', options.zIndex);
                    if (layer) {
                        layer.element.append(this.element);
                    }
                    this.renderTooltip();
                }
            },
            renderTooltip: function () {
                var marker = this;
                var title = marker.options.title;
                var options = marker.options.tooltip || {};
                if (options && Tooltip) {
                    var template = options.template;
                    if (template) {
                        var contentTemplate = kendo.template(template);
                        options.content = function (e) {
                            e.location = marker.location();
                            e.marker = marker;
                            return contentTemplate(e);
                        };
                    }
                    if (title || options.content || options.contentUrl) {
                        this.tooltip = new Tooltip(this.element, options);
                        this.tooltip.marker = this;
                    }
                }
            }
        });
        Marker.create = function (arg, defaults) {
            if (arg instanceof Marker) {
                return arg;
            }
            return new Marker(deepExtend({}, defaults, arg));
        };
        deepExtend(dataviz, {
            map: {
                layers: {
                    marker: MarkerLayer,
                    MarkerLayer: MarkerLayer
                },
                Marker: Marker
            }
        });
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/map/main', [
        'dataviz/map/crs',
        'dataviz/map/location'
    ], f);
}(function () {
    (function ($, undefined) {
        var doc = document, math = Math, min = math.min, pow = math.pow, proxy = $.proxy, kendo = window.kendo, Widget = kendo.ui.Widget, deepExtend = kendo.deepExtend, dataviz = kendo.dataviz, ui = dataviz.ui, g = kendo.geometry, Point = g.Point, map = dataviz.map, Extent = map.Extent, Location = map.Location, EPSG3857 = map.crs.EPSG3857, util = kendo.util, renderPos = util.renderPos, drawingUtil = kendo.drawing.util, defined = drawingUtil.defined, limit = drawingUtil.limitValue, valueOrDefault = drawingUtil.valueOrDefault;
        var CSS_PREFIX = 'k-', FRICTION = 0.9, FRICTION_MOBILE = 0.93, MOUSEWHEEL = 'DOMMouseScroll mousewheel', VELOCITY_MULTIPLIER = 5;
        var Map = Widget.extend({
            init: function (element, options) {
                kendo.destroy(element);
                Widget.fn.init.call(this, element);
                this._initOptions(options);
                this.bind(this.events, options);
                this.crs = new EPSG3857();
                this.element.addClass(CSS_PREFIX + this.options.name.toLowerCase()).css('position', 'relative').empty().append(doc.createElement('div'));
                this._viewOrigin = this._getOrigin();
                this._initScroller();
                this._initMarkers();
                this._initControls();
                this._initLayers();
                this._reset();
                this._mousewheel = proxy(this._mousewheel, this);
                this.element.bind('click', proxy(this._click, this));
                this.element.bind(MOUSEWHEEL, this._mousewheel);
            },
            options: {
                name: 'Map',
                controls: {
                    attribution: true,
                    navigator: { panStep: 100 },
                    zoom: true
                },
                layers: [],
                layerDefaults: {
                    shape: {
                        style: {
                            fill: { color: '#fff' },
                            stroke: {
                                color: '#aaa',
                                width: 0.5
                            }
                        }
                    },
                    bubble: {
                        style: {
                            fill: {
                                color: '#fff',
                                opacity: 0.5
                            },
                            stroke: {
                                color: '#aaa',
                                width: 0.5
                            }
                        }
                    },
                    marker: {
                        shape: 'pinTarget',
                        tooltip: { position: 'top' }
                    }
                },
                center: [
                    0,
                    0
                ],
                zoom: 3,
                minSize: 256,
                minZoom: 1,
                maxZoom: 19,
                markers: [],
                markerDefaults: {
                    shape: 'pinTarget',
                    tooltip: { position: 'top' }
                },
                wraparound: true
            },
            events: [
                'beforeReset',
                'click',
                'markerActivate',
                'markerClick',
                'markerCreated',
                'pan',
                'panEnd',
                'reset',
                'shapeClick',
                'shapeCreated',
                'shapeFeatureCreated',
                'shapeMouseEnter',
                'shapeMouseLeave',
                'zoomEnd',
                'zoomStart'
            ],
            destroy: function () {
                this.scroller.destroy();
                if (this.navigator) {
                    this.navigator.destroy();
                }
                if (this.attribution) {
                    this.attribution.destroy();
                }
                if (this.zoomControl) {
                    this.zoomControl.destroy();
                }
                this.markers.destroy();
                for (var i = 0; i < this.layers.length; i++) {
                    this.layers[i].destroy();
                }
                Widget.fn.destroy.call(this);
            },
            zoom: function (level) {
                var options = this.options;
                if (defined(level)) {
                    level = math.round(limit(level, options.minZoom, options.maxZoom));
                    if (options.zoom !== level) {
                        options.zoom = level;
                        this._reset();
                    }
                    return this;
                } else {
                    return options.zoom;
                }
            },
            center: function (center) {
                if (center) {
                    this.options.center = Location.create(center).toArray();
                    this._reset();
                    return this;
                } else {
                    return Location.create(this.options.center);
                }
            },
            extent: function (extent) {
                if (extent) {
                    this._setExtent(extent);
                    return this;
                } else {
                    return this._getExtent();
                }
            },
            setOptions: function (options) {
                Widget.fn.setOptions.call(this, options);
                this._reset();
            },
            locationToLayer: function (location, zoom) {
                var clamp = !this.options.wraparound;
                location = Location.create(location);
                return this.crs.toPoint(location, this._layerSize(zoom), clamp);
            },
            layerToLocation: function (point, zoom) {
                var clamp = !this.options.wraparound;
                point = Point.create(point);
                return this.crs.toLocation(point, this._layerSize(zoom), clamp);
            },
            locationToView: function (location) {
                location = Location.create(location);
                var origin = this.locationToLayer(this._viewOrigin);
                var point = this.locationToLayer(location);
                return point.translateWith(origin.scale(-1));
            },
            viewToLocation: function (point, zoom) {
                var origin = this.locationToLayer(this._getOrigin(), zoom);
                point = Point.create(point);
                point = point.clone().translateWith(origin);
                return this.layerToLocation(point, zoom);
            },
            eventOffset: function (e) {
                var offset = this.element.offset();
                var event = e.originalEvent || e;
                var x = valueOrDefault(event.pageX, event.clientX) - offset.left;
                var y = valueOrDefault(event.pageY, event.clientY) - offset.top;
                return new g.Point(x, y);
            },
            eventToView: function (e) {
                var cursor = this.eventOffset(e);
                return this.locationToView(this.viewToLocation(cursor));
            },
            eventToLayer: function (e) {
                return this.locationToLayer(this.eventToLocation(e));
            },
            eventToLocation: function (e) {
                var cursor = this.eventOffset(e);
                return this.viewToLocation(cursor);
            },
            viewSize: function () {
                var element = this.element;
                var scale = this._layerSize();
                var width = element.width();
                if (!this.options.wraparound) {
                    width = min(scale, width);
                }
                return {
                    width: width,
                    height: min(scale, element.height())
                };
            },
            exportVisual: function () {
                this._reset();
                return false;
            },
            _setOrigin: function (origin, zoom) {
                var size = this.viewSize(), topLeft;
                origin = this._origin = Location.create(origin);
                topLeft = this.locationToLayer(origin, zoom);
                topLeft.x += size.width / 2;
                topLeft.y += size.height / 2;
                this.options.center = this.layerToLocation(topLeft, zoom).toArray();
                return this;
            },
            _getOrigin: function (invalidate) {
                var size = this.viewSize(), topLeft;
                if (invalidate || !this._origin) {
                    topLeft = this.locationToLayer(this.center());
                    topLeft.x -= size.width / 2;
                    topLeft.y -= size.height / 2;
                    this._origin = this.layerToLocation(topLeft);
                }
                return this._origin;
            },
            _setExtent: function (extent) {
                var raw = Extent.create(extent);
                var se = raw.se.clone();
                if (this.options.wraparound && se.lng < 0 && extent.nw.lng > 0) {
                    se.lng = 180 + (180 + se.lng);
                }
                extent = new Extent(raw.nw, se);
                this.center(extent.center());
                var width = this.element.width();
                var height = this.element.height();
                for (var zoom = this.options.maxZoom; zoom >= this.options.minZoom; zoom--) {
                    var topLeft = this.locationToLayer(extent.nw, zoom);
                    var bottomRight = this.locationToLayer(extent.se, zoom);
                    var layerWidth = math.abs(bottomRight.x - topLeft.x);
                    var layerHeight = math.abs(bottomRight.y - topLeft.y);
                    if (layerWidth <= width && layerHeight <= height) {
                        break;
                    }
                }
                this.zoom(zoom);
            },
            _getExtent: function () {
                var nw = this._getOrigin();
                var bottomRight = this.locationToLayer(nw);
                var size = this.viewSize();
                bottomRight.x += size.width;
                bottomRight.y += size.height;
                var se = this.layerToLocation(bottomRight);
                return new Extent(nw, se);
            },
            _zoomAround: function (pivot, level) {
                this._setOrigin(this.layerToLocation(pivot, level), level);
                this.zoom(level);
            },
            _initControls: function () {
                var controls = this.options.controls;
                if (ui.Attribution && controls.attribution) {
                    this._createAttribution(controls.attribution);
                }
                if (!kendo.support.mobileOS) {
                    if (ui.Navigator && controls.navigator) {
                        this._createNavigator(controls.navigator);
                    }
                    if (ui.ZoomControl && controls.zoom) {
                        this._createZoomControl(controls.zoom);
                    }
                }
            },
            _createControlElement: function (options, defaultPos) {
                var pos = options.position || defaultPos;
                var posSelector = '.' + renderPos(pos).replace(' ', '.');
                var wrap = $('.k-map-controls' + posSelector, this.element);
                if (wrap.length === 0) {
                    wrap = $('<div>').addClass('k-map-controls ' + renderPos(pos)).appendTo(this.element);
                }
                return $('<div>').appendTo(wrap);
            },
            _createAttribution: function (options) {
                var element = this._createControlElement(options, 'bottomRight');
                this.attribution = new ui.Attribution(element, options);
            },
            _createNavigator: function (options) {
                var element = this._createControlElement(options, 'topLeft');
                var navigator = this.navigator = new ui.Navigator(element, options);
                this._navigatorPan = proxy(this._navigatorPan, this);
                navigator.bind('pan', this._navigatorPan);
                this._navigatorCenter = proxy(this._navigatorCenter, this);
                navigator.bind('center', this._navigatorCenter);
            },
            _navigatorPan: function (e) {
                var map = this;
                var scroller = map.scroller;
                var x = scroller.scrollLeft + e.x;
                var y = scroller.scrollTop - e.y;
                var bounds = this._virtualSize;
                var height = this.element.height();
                var width = this.element.width();
                x = limit(x, bounds.x.min, bounds.x.max - width);
                y = limit(y, bounds.y.min, bounds.y.max - height);
                map.scroller.one('scroll', function (e) {
                    map._scrollEnd(e);
                });
                map.scroller.scrollTo(-x, -y);
            },
            _navigatorCenter: function () {
                this.center(this.options.center);
            },
            _createZoomControl: function (options) {
                var element = this._createControlElement(options, 'topLeft');
                var zoomControl = this.zoomControl = new ui.ZoomControl(element, options);
                this._zoomControlChange = proxy(this._zoomControlChange, this);
                zoomControl.bind('change', this._zoomControlChange);
            },
            _zoomControlChange: function (e) {
                if (!this.trigger('zoomStart', { originalEvent: e })) {
                    this.zoom(this.zoom() + e.delta);
                    this.trigger('zoomEnd', { originalEvent: e });
                }
            },
            _initScroller: function () {
                var friction = kendo.support.mobileOS ? FRICTION_MOBILE : FRICTION;
                var zoomable = this.options.zoomable !== false;
                var scroller = this.scroller = new kendo.mobile.ui.Scroller(this.element.children(0), {
                    friction: friction,
                    velocityMultiplier: VELOCITY_MULTIPLIER,
                    zoom: zoomable,
                    mousewheelScrolling: false
                });
                scroller.bind('scroll', proxy(this._scroll, this));
                scroller.bind('scrollEnd', proxy(this._scrollEnd, this));
                scroller.userEvents.bind('gesturestart', proxy(this._scaleStart, this));
                scroller.userEvents.bind('gestureend', proxy(this._scale, this));
                this.scrollElement = scroller.scrollElement;
            },
            _initLayers: function () {
                var defs = this.options.layers, layers = this.layers = [];
                for (var i = 0; i < defs.length; i++) {
                    var options = defs[i];
                    var type = options.type || 'shape';
                    var defaults = this.options.layerDefaults[type];
                    var impl = dataviz.map.layers[type];
                    layers.push(new impl(this, deepExtend({}, defaults, options)));
                }
            },
            _initMarkers: function () {
                this.markers = new map.layers.MarkerLayer(this, this.options.markerDefaults);
                this.markers.add(this.options.markers);
            },
            _scroll: function (e) {
                var origin = this.locationToLayer(this._viewOrigin).round();
                var movable = e.sender.movable;
                var offset = new g.Point(movable.x, movable.y).scale(-1).scale(1 / movable.scale);
                origin.x += offset.x;
                origin.y += offset.y;
                this._scrollOffset = offset;
                this._setOrigin(this.layerToLocation(origin));
                this.trigger('pan', {
                    originalEvent: e,
                    origin: this._getOrigin(),
                    center: this.center()
                });
            },
            _scrollEnd: function (e) {
                if (!this._scrollOffset || !this._panComplete()) {
                    return;
                }
                this._scrollOffset = null;
                this._panEndTS = new Date();
                this.trigger('panEnd', {
                    originalEvent: e,
                    origin: this._getOrigin(),
                    center: this.center()
                });
            },
            _panComplete: function () {
                return new Date() - (this._panEndTS || 0) > 50;
            },
            _scaleStart: function (e) {
                if (this.trigger('zoomStart', { originalEvent: e })) {
                    var touch = e.touches[1];
                    if (touch) {
                        touch.cancel();
                    }
                }
            },
            _scale: function (e) {
                var scale = this.scroller.movable.scale;
                var zoom = this._scaleToZoom(scale);
                var gestureCenter = new g.Point(e.center.x, e.center.y);
                var centerLocation = this.viewToLocation(gestureCenter, zoom);
                var centerPoint = this.locationToLayer(centerLocation, zoom);
                var originPoint = centerPoint.translate(-gestureCenter.x, -gestureCenter.y);
                this._zoomAround(originPoint, zoom);
                this.trigger('zoomEnd', { originalEvent: e });
            },
            _scaleToZoom: function (scaleDelta) {
                var scale = this._layerSize() * scaleDelta;
                var tiles = scale / this.options.minSize;
                var zoom = math.log(tiles) / math.log(2);
                return math.round(zoom);
            },
            _reset: function () {
                if (this.attribution) {
                    this.attribution.filter(this.center(), this.zoom());
                }
                this._viewOrigin = this._getOrigin(true);
                this._resetScroller();
                this.trigger('beforeReset');
                this.trigger('reset');
            },
            _resetScroller: function () {
                var scroller = this.scroller;
                var x = scroller.dimensions.x;
                var y = scroller.dimensions.y;
                var scale = this._layerSize();
                var nw = this.extent().nw;
                var topLeft = this.locationToLayer(nw).round();
                scroller.movable.round = true;
                scroller.reset();
                scroller.userEvents.cancel();
                var zoom = this.zoom();
                scroller.dimensions.forcedMinScale = pow(2, this.options.minZoom - zoom);
                scroller.dimensions.maxScale = pow(2, this.options.maxZoom - zoom);
                var xBounds = {
                    min: -topLeft.x,
                    max: scale - topLeft.x
                };
                var yBounds = {
                    min: -topLeft.y,
                    max: scale - topLeft.y
                };
                if (this.options.wraparound) {
                    xBounds.max = 20 * scale;
                    xBounds.min = -xBounds.max;
                }
                if (this.options.pannable === false) {
                    var viewSize = this.viewSize();
                    xBounds.min = yBounds.min = 0;
                    xBounds.max = viewSize.width;
                    yBounds.max = viewSize.height;
                }
                x.makeVirtual();
                y.makeVirtual();
                x.virtualSize(xBounds.min, xBounds.max);
                y.virtualSize(yBounds.min, yBounds.max);
                this._virtualSize = {
                    x: xBounds,
                    y: yBounds
                };
            },
            _renderLayers: function () {
                var defs = this.options.layers, layers = this.layers = [], scrollWrap = this.scrollWrap;
                scrollWrap.empty();
                for (var i = 0; i < defs.length; i++) {
                    var options = defs[i];
                    var type = options.type || 'shape';
                    var defaults = this.options.layerDefaults[type];
                    var impl = dataviz.map.layers[type];
                    layers.push(new impl(this, deepExtend({}, defaults, options)));
                }
            },
            _layerSize: function (zoom) {
                zoom = valueOrDefault(zoom, this.options.zoom);
                return this.options.minSize * pow(2, zoom);
            },
            _click: function (e) {
                if (!this._panComplete()) {
                    return;
                }
                var cursor = this.eventOffset(e);
                this.trigger('click', {
                    originalEvent: e,
                    location: this.viewToLocation(cursor)
                });
            },
            _mousewheel: function (e) {
                e.preventDefault();
                var delta = dataviz.mwDelta(e) > 0 ? -1 : 1;
                var options = this.options;
                var fromZoom = this.zoom();
                var toZoom = limit(fromZoom + delta, options.minZoom, options.maxZoom);
                if (options.zoomable !== false && toZoom !== fromZoom) {
                    if (!this.trigger('zoomStart', { originalEvent: e })) {
                        var cursor = this.eventOffset(e);
                        var location = this.viewToLocation(cursor);
                        var postZoom = this.locationToLayer(location, toZoom);
                        var origin = postZoom.translate(-cursor.x, -cursor.y);
                        this._zoomAround(origin, toZoom);
                        this.trigger('zoomEnd', { originalEvent: e });
                    }
                }
            }
        });
        dataviz.ui.plugin(Map);
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.dataviz.map', [
        'kendo.data',
        'kendo.userevents',
        'kendo.tooltip',
        'kendo.mobile.scroller',
        'kendo.draganddrop',
        'kendo.dataviz.core',
        'dataviz/map/location',
        'dataviz/map/attribution',
        'dataviz/map/navigator',
        'dataviz/map/zoom',
        'dataviz/map/crs',
        'dataviz/map/layers/base',
        'dataviz/map/layers/shape',
        'dataviz/map/layers/bubble',
        'dataviz/map/layers/tile',
        'dataviz/map/layers/bing',
        'dataviz/map/layers/marker',
        'dataviz/map/main'
    ], f);
}(function () {
    var __meta__ = {
        id: 'dataviz.map',
        name: 'Map',
        category: 'dataviz',
        description: 'The Kendo DataViz Map displays spatial data',
        depends: [
            'data',
            'userevents',
            'tooltip',
            'dataviz.core',
            'drawing',
            'mobile.scroller'
        ]
    };
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));