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
    define('dataviz/sparkline/kendo-sparkline', ['kendo.dataviz.chart'], f);
}(function () {
    (function () {
        window.kendo.dataviz = window.kendo.dataviz || {};
        var dataviz = kendo.dataviz;
        var constants = dataviz.constants;
        var Chart = dataviz.Chart;
        var elementSize = dataviz.elementSize;
        var deepExtend = dataviz.deepExtend;
        var TOP_OFFSET = -2;
        var SharedTooltip$1 = dataviz.SharedTooltip.extend({
            _slotAnchor: function (coords, slot) {
                var axis = this.plotArea.categoryAxis;
                var vertical = axis.options.vertical;
                var align = vertical ? {
                    horizontal: 'left',
                    vertical: 'center'
                } : {
                    horizontal: 'center',
                    vertical: 'bottom'
                };
                var point;
                if (vertical) {
                    point = new dataviz.Point(this.plotArea.box.x2, slot.center().y);
                } else {
                    point = new dataviz.Point(slot.center().x, TOP_OFFSET);
                }
                return {
                    point: point,
                    align: align
                };
            },
            _defaultAnchor: function (point, slot) {
                return this._slotAnchor({}, slot);
            }
        });
        var DEAULT_BAR_WIDTH = 150;
        var DEAULT_BULLET_WIDTH = 150;
        var NO_CROSSHAIR = [
            constants.BAR,
            constants.BULLET
        ];
        function hide(children) {
            var state = [];
            for (var idx = 0; idx < children.length; idx++) {
                var child = children[idx];
                state[idx] = child.style.display;
                child.style.display = 'none';
            }
            return state;
        }
        function show(children, state) {
            for (var idx = 0; idx < children.length; idx++) {
                children[idx].style.display = state[idx];
            }
        }
        function wrapNumber(value) {
            return dataviz.isNumber(value) ? [value] : value;
        }
        var Sparkline = Chart.extend({
            _setElementClass: function (element) {
                dataviz.addClass(element, 'k-sparkline');
            },
            _initElement: function (element) {
                Chart.fn._initElement.call(this, element);
                this._initialWidth = Math.floor(elementSize(element).width);
            },
            _resize: function () {
                var element = this.element;
                var state = hide(element.childNodes);
                this._initialWidth = Math.floor(elementSize(element).width);
                show(element.childNodes, state);
                Chart.fn._resize.call(this);
            },
            _modelOptions: function () {
                var chartOptions = this.options;
                var stage = this._surfaceWrap();
                var displayState = hide(stage.childNodes);
                var space = document.createElement('span');
                space.innerHTML = '&nbsp;';
                stage.appendChild(space);
                var options = deepExtend({
                    width: this._autoWidth,
                    height: elementSize(stage).height,
                    transitions: chartOptions.transitions
                }, chartOptions.chartArea, {
                    inline: true,
                    align: false
                });
                elementSize(stage, {
                    width: options.width,
                    height: options.height
                });
                stage.removeChild(space);
                show(stage.childNodes, displayState);
                this.surface.resize();
                return options;
            },
            _surfaceWrap: function () {
                if (!this.stage) {
                    var stage = this.stage = document.createElement('span');
                    this.element.appendChild(stage);
                }
                return this.stage;
            },
            _createPlotArea: function (skipSeries) {
                var plotArea = Chart.fn._createPlotArea.call(this, skipSeries);
                this._autoWidth = this._initialWidth || this._calculateWidth(plotArea);
                return plotArea;
            },
            _calculateWidth: function (plotArea) {
                var options = this.options;
                var margin = dataviz.getSpacing(options.chartArea.margin);
                var charts = plotArea.charts;
                var stage = this._surfaceWrap();
                var total = 0;
                for (var i = 0; i < charts.length; i++) {
                    var currentChart = charts[i];
                    var firstSeries = (currentChart.options.series || [])[0];
                    if (!firstSeries) {
                        continue;
                    }
                    if (firstSeries.type === constants.BAR) {
                        return DEAULT_BAR_WIDTH;
                    }
                    if (firstSeries.type === constants.BULLET) {
                        return DEAULT_BULLET_WIDTH;
                    }
                    if (firstSeries.type === constants.PIE) {
                        return elementSize(stage).height;
                    }
                    var categoryAxis = currentChart.categoryAxis;
                    if (categoryAxis) {
                        var pointsCount = categoryAxis.options.categories.length * (!currentChart.options.isStacked && dataviz.inArray(firstSeries.type, [
                            constants.COLUMN,
                            constants.VERTICAL_BULLET
                        ]) ? currentChart.seriesOptions.length : 1);
                        total = Math.max(total, pointsCount);
                    }
                }
                var size = total * options.pointWidth;
                if (size > 0) {
                    size += margin.left + margin.right;
                }
                return size;
            },
            _createSharedTooltip: function (options) {
                return new SharedTooltip$1(this._plotArea, options);
            }
        });
        Sparkline.normalizeOptions = function (userOptions) {
            var options = wrapNumber(userOptions);
            if (dataviz.isArray(options)) {
                options = { seriesDefaults: { data: options } };
            } else {
                options = deepExtend({}, options);
            }
            if (!options.series) {
                options.series = [{ data: wrapNumber(options.data) }];
            }
            deepExtend(options, { seriesDefaults: { type: options.type } });
            if (dataviz.inArray(options.series[0].type, NO_CROSSHAIR) || dataviz.inArray(options.seriesDefaults.type, NO_CROSSHAIR)) {
                options = deepExtend({}, { categoryAxis: { crosshair: { visible: false } } }, options);
            }
            return options;
        };
        dataviz.setDefaultOptions(Sparkline, {
            chartArea: { margin: 2 },
            axisDefaults: {
                visible: false,
                majorGridLines: { visible: false },
                valueAxis: { narrowRange: true }
            },
            seriesDefaults: {
                type: 'line',
                area: { line: { width: 0.5 } },
                bar: { stack: true },
                padding: 2,
                width: 0.5,
                overlay: { gradient: null },
                highlight: { visible: false },
                border: { width: 0 },
                markers: {
                    size: 2,
                    visible: false
                }
            },
            tooltip: {
                visible: true,
                shared: true
            },
            categoryAxis: {
                crosshair: {
                    visible: true,
                    tooltip: { visible: false }
                }
            },
            legend: { visible: false },
            transitions: false,
            pointWidth: 5,
            panes: [{ clip: false }]
        });
        kendo.deepExtend(kendo.dataviz, { Sparkline: Sparkline });
    }());
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/sparkline/sparkline', ['dataviz/sparkline/kendo-sparkline'], f);
}(function () {
    (function ($) {
        var dataviz = kendo.dataviz;
        var Chart = dataviz.ui.Chart;
        var KendoSparkline = dataviz.Sparkline;
        var ChartInstanceObserver = dataviz.ChartInstanceObserver;
        var extend = $.extend;
        var Sparkline = Chart.extend({
            init: function (element, userOptions) {
                var options = userOptions;
                if (options instanceof kendo.data.ObservableArray) {
                    options = { seriesDefaults: { data: options } };
                }
                Chart.fn.init.call(this, element, KendoSparkline.normalizeOptions(options));
            },
            _createChart: function (options, themeOptions) {
                this._instance = new KendoSparkline(this.element[0], options, themeOptions, {
                    observer: new ChartInstanceObserver(this),
                    sender: this,
                    rtl: this._isRtl()
                });
            },
            _createTooltip: function () {
                return new SparklineTooltip(this.element, extend({}, this.options.tooltip, { rtl: this._isRtl() }));
            },
            options: {
                name: 'Sparkline',
                chartArea: { margin: 2 },
                axisDefaults: {
                    visible: false,
                    majorGridLines: { visible: false },
                    valueAxis: { narrowRange: true }
                },
                seriesDefaults: {
                    type: 'line',
                    area: { line: { width: 0.5 } },
                    bar: { stack: true },
                    padding: 2,
                    width: 0.5,
                    overlay: { gradient: null },
                    highlight: { visible: false },
                    border: { width: 0 },
                    markers: {
                        size: 2,
                        visible: false
                    }
                },
                tooltip: {
                    visible: true,
                    shared: true
                },
                categoryAxis: {
                    crosshair: {
                        visible: true,
                        tooltip: { visible: false }
                    }
                },
                legend: { visible: false },
                transitions: false,
                pointWidth: 5,
                panes: [{ clip: false }]
            }
        });
        dataviz.ui.plugin(Sparkline);
        var SparklineTooltip = dataviz.Tooltip.extend({
            options: { animation: { duration: 0 } },
            _hideElement: function () {
                if (this.element) {
                    this.element.hide().remove();
                }
            }
        });
        dataviz.SparklineTooltip = SparklineTooltip;
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.dataviz.sparkline', [
        'dataviz/sparkline/kendo-sparkline',
        'dataviz/sparkline/sparkline'
    ], f);
}(function () {
    var __meta__ = {
        id: 'dataviz.sparkline',
        name: 'Sparkline',
        category: 'dataviz',
        description: 'Sparkline widgets.',
        depends: ['dataviz.chart']
    };
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));