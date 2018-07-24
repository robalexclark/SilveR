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
    define('dataviz/stock/kendo-stock-chart', ['kendo.dataviz.chart'], f);
}(function () {
    (function () {
        window.kendo.dataviz = window.kendo.dataviz || {};
        var dataviz = kendo.dataviz;
        var elementStyles = dataviz.elementStyles;
        var deepExtend = dataviz.deepExtend;
        var toTime = dataviz.toTime;
        var datavizConstants = dataviz.constants;
        var Chart = dataviz.Chart;
        var drawing = kendo.drawing;
        var FadeOutAnimation = drawing.Animation.extend({
            setup: function () {
                this._initialOpacity = parseFloat(elementStyles(this.element, 'opacity').opacity);
            },
            step: function (pos) {
                elementStyles(this.element, { opacity: String(dataviz.interpolateValue(this._initialOpacity, 0, pos)) });
            },
            abort: function () {
                drawing.Animation.fn.abort.call(this);
                elementStyles(this.element, {
                    display: 'none',
                    opacity: String(this._initialOpacity)
                });
            },
            cancel: function () {
                drawing.Animation.fn.abort.call(this);
                elementStyles(this.element, { opacity: String(this._initialOpacity) });
            }
        });
        function createDiv(className, style) {
            var div = document.createElement('div');
            div.className = className;
            if (style) {
                div.style.cssText = style;
            }
            return div;
        }
        var NavigatorHint = dataviz.Class.extend({
            init: function (container, chartService, options) {
                this.options = deepExtend({}, this.options, options);
                this.container = container;
                this.chartService = chartService;
                var padding = elementStyles(container, [
                    'paddingLeft',
                    'paddingTop'
                ]);
                this.chartPadding = {
                    top: padding.paddingTop,
                    left: padding.paddingLeft
                };
                this.createElements();
                container.appendChild(this.element);
            },
            createElements: function () {
                var element = this.element = createDiv('k-navigator-hint', 'display: none; position: absolute; top: 1px; left: 1px;');
                var tooltip = this.tooltip = createDiv('k-tooltip k-chart-tooltip');
                var scroll = this.scroll = createDiv('k-scroll');
                tooltip.innerHTML = '&nbsp;';
                element.appendChild(tooltip);
                element.appendChild(scroll);
            },
            show: function (from, to, bbox) {
                var ref = this;
                var element = ref.element;
                var options = ref.options;
                var scroll = ref.scroll;
                var tooltip = ref.tooltip;
                var middle = dataviz.toDate(toTime(from) + toTime(to - from) / 2);
                var scrollWidth = bbox.width() * 0.4;
                var minPos = bbox.center().x - scrollWidth;
                var maxPos = bbox.center().x;
                var posRange = maxPos - minPos;
                var range = options.max - options.min;
                var scale = posRange / range;
                var offset = middle - options.min;
                var text = this.chartService.intl.format(options.format, from, to);
                var template = dataviz.getTemplate(options);
                this.clearHideTimeout();
                if (!this._visible) {
                    elementStyles(element, {
                        visibility: 'hidden',
                        display: 'block'
                    });
                    this._visible = true;
                }
                if (template) {
                    text = template({
                        from: from,
                        to: to
                    });
                }
                tooltip.innerHTML = text;
                elementStyles(tooltip, {
                    left: bbox.center().x - tooltip.offsetWidth / 2,
                    top: bbox.y1
                });
                var tooltipStyle = elementStyles(tooltip, [
                    'marginTop',
                    'borderTopWidth',
                    'height'
                ]);
                elementStyles(scroll, {
                    width: scrollWidth,
                    left: minPos + offset * scale,
                    top: bbox.y1 + tooltipStyle.marginTop + tooltipStyle.borderTopWidth + tooltipStyle.height / 2
                });
                elementStyles(element, { visibility: 'visible' });
            },
            clearHideTimeout: function () {
                if (this._hideTimeout) {
                    clearTimeout(this._hideTimeout);
                }
                if (this._hideAnimation) {
                    this._hideAnimation.cancel();
                }
            },
            hide: function () {
                var this$1 = this;
                this.clearHideTimeout();
                this._hideTimeout = setTimeout(function () {
                    this$1._visible = false;
                    this$1._hideAnimation = new FadeOutAnimation(this$1.element);
                    this$1._hideAnimation.setup();
                    this$1._hideAnimation.play();
                }, this.options.hideDelay);
            },
            destroy: function () {
                this.clearHideTimeout();
                if (this.container) {
                    this.container.removeChild(this.element);
                }
                delete this.container;
                delete this.chartService;
                delete this.element;
                delete this.tooltip;
                delete this.scroll;
            }
        });
        dataviz.setDefaultOptions(NavigatorHint, {
            format: '{0:d} - {1:d}',
            hideDelay: 500
        });
        var NAVIGATOR_PANE = '_navigator';
        var NAVIGATOR_AXIS = NAVIGATOR_PANE;
        var constants = {
            NAVIGATOR_AXIS: NAVIGATOR_AXIS,
            NAVIGATOR_PANE: NAVIGATOR_PANE
        };
        var ZOOM_ACCELERATION = 3;
        var Navigator = dataviz.Class.extend({
            init: function (chart) {
                this.chart = chart;
                var options = this.options = deepExtend({}, this.options, chart.options.navigator);
                var select = options.select;
                if (select) {
                    select.from = this.parseDate(select.from);
                    select.to = this.parseDate(select.to);
                }
                if (!dataviz.defined(options.hint.visible)) {
                    options.hint.visible = options.visible;
                }
                var obj;
                this.chartObserver = new dataviz.InstanceObserver(this, (obj = {}, obj[datavizConstants.DRAG] = '_drag', obj[datavizConstants.DRAG_END] = '_dragEnd', obj[datavizConstants.ZOOM] = '_zoom', obj[datavizConstants.ZOOM_END] = '_zoomEnd', obj));
                chart.addObserver(this.chartObserver);
            },
            parseDate: function (value) {
                return dataviz.parseDate(this.chart.chartService.intl, value);
            },
            destroy: function () {
                if (this.chart) {
                    this.chart.removeObserver(this.chartObserver);
                    delete this.chart;
                }
                if (this.selection) {
                    this.selection.destroy();
                    delete this.selection;
                }
                if (this.hint) {
                    this.hint.destroy();
                    delete this.hint;
                }
            },
            redraw: function () {
                this._redrawSelf();
                this.initSelection();
            },
            initSelection: function () {
                var ref = this;
                var chart = ref.chart;
                var options = ref.options;
                var axis = this.mainAxis();
                var ref$1 = axis.range();
                var min = ref$1.min;
                var max = ref$1.max;
                var ref$2 = options.select;
                var from = ref$2.from;
                var to = ref$2.to;
                var mousewheel = ref$2.mousewheel;
                var axisClone = clone(axis);
                var groups = axis.options.categories;
                var selection = this.selection;
                if (groups.length === 0) {
                    return;
                }
                if (selection) {
                    selection.destroy();
                }
                axisClone.box = axis.box;
                selection = this.selection = new dataviz.Selection(chart, axisClone, {
                    min: min,
                    max: max,
                    from: from || min,
                    to: to || max,
                    mousewheel: dataviz.valueOrDefault(mousewheel, { zoom: 'left' }),
                    visible: options.visible
                }, new dataviz.InstanceObserver(this, {
                    selectStart: '_selectStart',
                    select: '_select',
                    selectEnd: '_selectEnd'
                }));
                if (this.hint) {
                    this.hint.destroy();
                }
                if (options.hint.visible) {
                    this.hint = new NavigatorHint(chart.element, chart.chartService, {
                        min: min,
                        max: max,
                        template: dataviz.getTemplate(options.hint),
                        format: options.hint.format
                    });
                }
            },
            setRange: function () {
                var plotArea = this.chart._createPlotArea(true);
                var axis = plotArea.namedCategoryAxes[NAVIGATOR_AXIS];
                var ref = axis.range();
                var min = ref.min;
                var max = ref.max;
                var select = this.options.select || {};
                var from = select.from || min;
                if (from < min) {
                    from = min;
                }
                var to = select.to || max;
                if (to > max) {
                    to = max;
                }
                this.options.select = deepExtend({}, select, {
                    from: from,
                    to: to
                });
                this.filterAxes();
            },
            _redrawSelf: function (silent) {
                var plotArea = this.chart._plotArea;
                if (plotArea) {
                    plotArea.redraw(dataviz.last(plotArea.panes), silent);
                }
            },
            redrawSlaves: function () {
                var chart = this.chart;
                var plotArea = chart._plotArea;
                var slavePanes = plotArea.panes.slice(0, -1);
                plotArea.srcSeries = chart.options.series;
                plotArea.options.categoryAxis = chart.options.categoryAxis;
                plotArea.redraw(slavePanes);
            },
            _drag: function (e) {
                var ref = this;
                var chart = ref.chart;
                var selection = ref.selection;
                var coords = chart._eventCoordinates(e.originalEvent);
                var navigatorAxis = this.mainAxis();
                var naviRange = navigatorAxis.datesRange();
                var inNavigator = navigatorAxis.pane.box.containsPoint(coords);
                var axis = chart._plotArea.categoryAxis;
                var range = e.axisRanges[axis.options.name];
                var select = this.options.select;
                var duration;
                if (!range || inNavigator || !selection) {
                    return;
                }
                if (select.from && select.to) {
                    duration = toTime(select.to) - toTime(select.from);
                } else {
                    duration = toTime(selection.options.to) - toTime(selection.options.from);
                }
                var from = dataviz.toDate(dataviz.limitValue(toTime(range.min), naviRange.min, toTime(naviRange.max) - duration));
                var to = dataviz.toDate(dataviz.limitValue(toTime(from) + duration, toTime(naviRange.min) + duration, naviRange.max));
                this.options.select = {
                    from: from,
                    to: to
                };
                if (this.options.liveDrag) {
                    this.filterAxes();
                    this.redrawSlaves();
                }
                selection.set(from, to);
                this.showHint(from, to);
            },
            _dragEnd: function () {
                this.filterAxes();
                this.filter();
                this.redrawSlaves();
                if (this.hint) {
                    this.hint.hide();
                }
            },
            readSelection: function () {
                var ref = this;
                var ref_selection_options = ref.selection.options;
                var from = ref_selection_options.from;
                var to = ref_selection_options.to;
                var select = ref.options.select;
                select.from = from;
                select.to = to;
            },
            filterAxes: function () {
                var ref = this;
                var select = ref.options.select;
                if (select === void 0) {
                    select = {};
                }
                var chart = ref.chart;
                var allAxes = chart.options.categoryAxis;
                var from = select.from;
                var to = select.to;
                for (var idx = 0; idx < allAxes.length; idx++) {
                    var axis = allAxes[idx];
                    if (axis.pane !== NAVIGATOR_PANE) {
                        axis.min = from;
                        axis.max = to;
                    }
                }
            },
            filter: function () {
                var ref = this;
                var chart = ref.chart;
                var select = ref.options.select;
                if (chart.requiresHandlers(['navigatorFilter'])) {
                    var axisOptions = new dataviz.DateCategoryAxis(deepExtend({ baseUnit: 'fit' }, chart.options.categoryAxis[0], {
                        categories: [
                            select.from,
                            select.to
                        ]
                    }), chart.chartService).options;
                    this.chart.trigger('navigatorFilter', {
                        from: dataviz.addDuration(axisOptions.min, -axisOptions.baseUnitStep, axisOptions.baseUnit),
                        to: dataviz.addDuration(axisOptions.max, axisOptions.baseUnitStep, axisOptions.baseUnit)
                    });
                }
            },
            _zoom: function (e) {
                var ref = this;
                var axis = ref.chart._plotArea.categoryAxis;
                var selection = ref.selection;
                var ref_options = ref.options;
                var select = ref_options.select;
                var liveDrag = ref_options.liveDrag;
                var categories = this.mainAxis().options.categories;
                var delta = e.delta;
                if (!selection) {
                    return;
                }
                var fromIx = dataviz.lteDateIndex(selection.options.from, categories);
                var toIx = dataviz.lteDateIndex(selection.options.to, categories);
                e.originalEvent.preventDefault();
                if (Math.abs(delta) > 1) {
                    delta *= ZOOM_ACCELERATION;
                }
                if (toIx - fromIx > 1) {
                    selection.expand(delta);
                    this.readSelection();
                } else {
                    axis.options.min = select.from;
                    select.from = axis.scaleRange(-e.delta).min;
                }
                if (liveDrag) {
                    this.filterAxes();
                    this.redrawSlaves();
                }
                selection.set(select.from, select.to);
                this.showHint(this.options.select.from, this.options.select.to);
            },
            _zoomEnd: function (e) {
                this._dragEnd(e);
            },
            showHint: function (from, to) {
                var plotArea = this.chart._plotArea;
                if (this.hint) {
                    this.hint.show(from, to, plotArea.backgroundBox());
                }
            },
            _selectStart: function (e) {
                return this.chart._selectStart(e);
            },
            _select: function (e) {
                this.showHint(e.from, e.to);
                return this.chart._select(e);
            },
            _selectEnd: function (e) {
                if (this.hint) {
                    this.hint.hide();
                }
                this.readSelection();
                this.filterAxes();
                this.filter();
                this.redrawSlaves();
                return this.chart._selectEnd(e);
            },
            mainAxis: function () {
                var plotArea = this.chart._plotArea;
                if (plotArea) {
                    return plotArea.namedCategoryAxes[NAVIGATOR_AXIS];
                }
            },
            select: function (from, to) {
                var select = this.options.select;
                if (from && to) {
                    select.from = this.parseDate(from);
                    select.to = this.parseDate(to);
                    this.filterAxes();
                    this.filter();
                    this.redrawSlaves();
                    this.selection.set(from, to);
                }
                return {
                    from: select.from,
                    to: select.to
                };
            }
        });
        Navigator.setup = function (options, themeOptions) {
            if (options === void 0) {
                options = {};
            }
            if (themeOptions === void 0) {
                themeOptions = {};
            }
            if (options.__navi) {
                return;
            }
            options.__navi = true;
            var naviOptions = deepExtend({}, themeOptions.navigator, options.navigator);
            var panes = options.panes = [].concat(options.panes);
            var paneOptions = deepExtend({}, naviOptions.pane, { name: NAVIGATOR_PANE });
            if (!naviOptions.visible) {
                paneOptions.visible = false;
                paneOptions.height = 0.1;
            }
            panes.push(paneOptions);
            Navigator.attachAxes(options, naviOptions);
            Navigator.attachSeries(options, naviOptions, themeOptions);
        };
        Navigator.attachAxes = function (options, naviOptions) {
            var series = naviOptions.series || [];
            var categoryAxes = options.categoryAxis = [].concat(options.categoryAxis);
            var valueAxes = options.valueAxis = [].concat(options.valueAxis);
            var equallySpacedSeries = dataviz.filterSeriesByType(series, datavizConstants.EQUALLY_SPACED_SERIES);
            var justifyAxis = equallySpacedSeries.length === 0;
            var base = deepExtend({
                type: 'date',
                pane: NAVIGATOR_PANE,
                roundToBaseUnit: !justifyAxis,
                justified: justifyAxis,
                _collapse: false,
                majorTicks: { visible: true },
                tooltip: { visible: false },
                labels: { step: 1 },
                autoBind: naviOptions.autoBindElements,
                autoBaseUnitSteps: {
                    minutes: [1],
                    hours: [
                        1,
                        2
                    ],
                    days: [
                        1,
                        2
                    ],
                    weeks: [],
                    months: [1],
                    years: [1]
                }
            });
            var user = naviOptions.categoryAxis;
            categoryAxes.push(deepExtend({}, base, { maxDateGroups: 200 }, user, {
                name: NAVIGATOR_AXIS,
                title: null,
                baseUnit: 'fit',
                baseUnitStep: 'auto',
                labels: { visible: false },
                majorTicks: { visible: false }
            }), deepExtend({}, base, user, {
                name: NAVIGATOR_AXIS + '_labels',
                maxDateGroups: 20,
                baseUnitStep: 'auto',
                plotBands: [],
                autoBaseUnitSteps: { minutes: [] },
                _overlap: true
            }), deepExtend({}, base, user, {
                name: NAVIGATOR_AXIS + '_ticks',
                maxDateGroups: 200,
                majorTicks: { width: 0.5 },
                plotBands: [],
                title: null,
                labels: {
                    visible: false,
                    mirror: true
                },
                _overlap: true
            }));
            valueAxes.push(deepExtend({
                name: NAVIGATOR_AXIS,
                pane: NAVIGATOR_PANE,
                majorGridLines: { visible: false },
                visible: false
            }, naviOptions.valueAxis));
        };
        Navigator.attachSeries = function (options, naviOptions, themeOptions) {
            var series = options.series = options.series || [];
            var navigatorSeries = [].concat(naviOptions.series || []);
            var seriesColors = themeOptions.seriesColors;
            var defaults = naviOptions.seriesDefaults;
            for (var idx = 0; idx < navigatorSeries.length; idx++) {
                series.push(deepExtend({
                    color: seriesColors[idx % seriesColors.length],
                    categoryField: naviOptions.dateField,
                    visibleInLegend: false,
                    tooltip: { visible: false }
                }, defaults, navigatorSeries[idx], {
                    axis: NAVIGATOR_AXIS,
                    categoryAxis: NAVIGATOR_AXIS,
                    autoBind: naviOptions.autoBindElements
                }));
            }
        };
        function ClonedObject() {
        }
        function clone(obj) {
            ClonedObject.prototype = obj;
            return new ClonedObject();
        }
        var AUTO_CATEGORY_WIDTH = 28;
        var StockChart = Chart.extend({
            applyDefaults: function (options, themeOptions) {
                var width = dataviz.elementSize(this.element).width || datavizConstants.DEFAULT_WIDTH;
                var theme = themeOptions;
                var stockDefaults = {
                    seriesDefaults: { categoryField: options.dateField },
                    axisDefaults: {
                        categoryAxis: {
                            name: 'default',
                            majorGridLines: { visible: false },
                            labels: { step: 2 },
                            majorTicks: { visible: false },
                            maxDateGroups: Math.floor(width / AUTO_CATEGORY_WIDTH)
                        }
                    }
                };
                if (theme) {
                    theme = deepExtend({}, theme, stockDefaults);
                }
                Navigator.setup(options, theme);
                Chart.fn.applyDefaults.call(this, options, theme);
            },
            _setElementClass: function (element) {
                dataviz.addClass(element, 'k-chart k-stockchart');
            },
            setOptions: function (options) {
                this.destroyNavigator();
                Chart.fn.setOptions.call(this, options);
            },
            _resize: function () {
                var transitions = this.options.transitions;
                this.options.transitions = false;
                this._fullRedraw();
                this.options.transitions = transitions;
            },
            _redraw: function () {
                var navigator = this.navigator;
                if (!this._dirty() && navigator && navigator.options.partialRedraw) {
                    navigator.redrawSlaves();
                } else {
                    this._fullRedraw();
                }
            },
            _dirty: function () {
                var options = this.options;
                var series = [].concat(options.series, options.navigator.series);
                var seriesCount = dataviz.grep(series, function (s) {
                    return s && s.visible;
                }).length;
                var dirty = this._seriesCount !== seriesCount;
                this._seriesCount = seriesCount;
                return dirty;
            },
            _fullRedraw: function () {
                var navigator = this.navigator;
                if (!navigator) {
                    navigator = this.navigator = new Navigator(this);
                    this.trigger('navigatorCreated', { navigator: navigator });
                }
                navigator.setRange();
                Chart.fn._redraw.call(this);
                navigator.initSelection();
            },
            _trackSharedTooltip: function (coords) {
                var plotArea = this._plotArea;
                var pane = plotArea.paneByPoint(coords);
                if (pane && pane.options.name === NAVIGATOR_PANE) {
                    this._unsetActivePoint();
                } else {
                    Chart.fn._trackSharedTooltip.call(this, coords);
                }
            },
            bindCategories: function () {
                Chart.fn.bindCategories.call(this);
                this.copyNavigatorCategories();
            },
            copyNavigatorCategories: function () {
                var definitions = [].concat(this.options.categoryAxis);
                var categories;
                for (var axisIx = 0; axisIx < definitions.length; axisIx++) {
                    var axis = definitions[axisIx];
                    if (axis.name === NAVIGATOR_AXIS) {
                        categories = axis.categories;
                    } else if (categories && axis.pane === NAVIGATOR_PANE) {
                        axis.categories = categories;
                    }
                }
            },
            destroyNavigator: function () {
                if (this.navigator) {
                    this.navigator.destroy();
                    this.navigator = null;
                }
            },
            destroy: function () {
                this.destroyNavigator();
                Chart.fn.destroy.call(this);
            },
            _stopDragEvent: function (e) {
                var coords = this._eventCoordinates(e);
                var pane = this._plotArea.paneByPoint(coords);
                return Chart.fn._stopDragEvent.call(this, e) || pane && pane.options.name === NAVIGATOR_PANE;
            }
        });
        dataviz.setDefaultOptions(StockChart, {
            dateField: 'date',
            axisDefaults: {
                categoryAxis: {
                    type: 'date',
                    baseUnit: 'fit',
                    justified: true
                },
                valueAxis: {
                    narrowRange: true,
                    labels: { format: 'C' }
                }
            },
            navigator: {
                select: {},
                seriesDefaults: {
                    markers: { visible: false },
                    tooltip: { visible: true },
                    line: { width: 2 }
                },
                hint: {},
                visible: true
            },
            tooltip: { visible: true },
            legend: { visible: false }
        });
        kendo.deepExtend(kendo.dataviz, {
            constants: constants,
            Navigator: Navigator,
            NavigatorHint: NavigatorHint,
            StockChart: StockChart
        });
    }());
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/stock/stock-chart', ['dataviz/stock/kendo-stock-chart'], f);
}(function () {
    (function ($) {
        var kendo = window.kendo;
        var dataviz = kendo.dataviz;
        var ChartInstanceObserver = dataviz.ChartInstanceObserver;
        var Chart = dataviz.ui.Chart;
        var KendoStockChart = dataviz.StockChart;
        var constants = dataviz.constants;
        var NAVIGATOR_AXIS = constants.NAVIGATOR_AXIS;
        var NAVIGATOR_PANE = constants.NAVIGATOR_PANE;
        var deepExtend = kendo.deepExtend;
        var defined = dataviz.defined;
        var proxy = $.proxy;
        var CHANGE = 'change';
        var StockInstanceObserver = ChartInstanceObserver.extend({
            handlerMap: {
                navigatorFilter: '_onNavigatorFilter',
                navigatorCreated: '_onNavigatorCreated'
            }
        });
        var StockChart = Chart.extend({
            options: {
                name: 'StockChart',
                dateField: 'date',
                axisDefaults: {
                    categoryAxis: {
                        type: 'date',
                        baseUnit: 'fit',
                        justified: true
                    },
                    valueAxis: {
                        narrowRange: true,
                        labels: { format: 'C' }
                    }
                },
                navigator: {
                    select: {},
                    seriesDefaults: {
                        markers: { visible: false },
                        tooltip: {
                            visible: true,
                            template: '#= kendo.toString(category, \'d\') #'
                        },
                        line: { width: 2 }
                    },
                    hint: {},
                    visible: true
                },
                tooltip: { visible: true },
                legend: { visible: false },
                persistSeriesVisibility: true
            },
            _createChart: function (options, themeOptions) {
                this._initNavigatorOptions(options);
                this._instance = new KendoStockChart(this.element[0], options, themeOptions, {
                    observer: new StockInstanceObserver(this),
                    sender: this,
                    rtl: this._isRtl()
                });
            },
            _initNavigatorOptions: function (options) {
                var navigatorOptions = options.navigator || {};
                var support = kendo.support;
                var isTouch = support.touch;
                var isFirefox = support.browser.mozilla;
                deepExtend(navigatorOptions, {
                    autoBindElements: !navigatorOptions.dataSource,
                    partialRedraw: navigatorOptions.dataSource,
                    liveDrag: !isTouch && !isFirefox
                });
            },
            _initDataSource: function (userOptions) {
                var options = userOptions || {}, dataSource = options.dataSource, hasServerFiltering = dataSource && dataSource.serverFiltering, mainAxis = [].concat(options.categoryAxis)[0], naviOptions = options.navigator || {}, select = naviOptions.select, hasSelect = select && select.from && select.to;
                if (hasServerFiltering && hasSelect) {
                    var filter = [].concat(dataSource.filter || []);
                    var from = kendo.parseDate(select.from);
                    var to = kendo.parseDate(select.to);
                    var dummyAxis = new dataviz.DateCategoryAxis(deepExtend({ baseUnit: 'fit' }, mainAxis, {
                        categories: [
                            from,
                            to
                        ]
                    }), kendo);
                    dataSource.filter = buildFilter(dummyAxis.range().min, to).concat(filter);
                }
                Chart.fn._initDataSource.call(this, userOptions);
            },
            _onNavigatorCreated: function (e) {
                this._instance = e.sender;
                this.options = e.sender.options;
                this._navigator = this.navigator = e.navigator;
                this._initNavigatorDataSource();
            },
            _initNavigatorDataSource: function () {
                var navigatorOptions = this.options.navigator;
                var autoBind = navigatorOptions.autoBind;
                var dsOptions = navigatorOptions.dataSource;
                if (dsOptions) {
                    this._navigatorDataChangedHandler = this._navigatorDataChangedHandler || proxy(this._onNavigatorDataChanged, this);
                    this._navigatorDataSource = kendo.data.DataSource.create(dsOptions).bind(CHANGE, this._navigatorDataChangedHandler);
                    if (!defined(autoBind)) {
                        autoBind = this.options.autoBind;
                    }
                    if (autoBind) {
                        this._navigatorDataSource.fetch();
                    }
                }
            },
            _bindNavigatorSeries: function (series, data) {
                var seriesIx, currentSeries, seriesLength = series.length;
                for (seriesIx = 0; seriesIx < seriesLength; seriesIx++) {
                    currentSeries = series[seriesIx];
                    if (currentSeries.axis == NAVIGATOR_AXIS && this._isBindable(currentSeries)) {
                        currentSeries.data = data;
                    }
                }
            },
            _onNavigatorDataChanged: function () {
                var chart = this, instance = chart._instance, categoryAxes = chart.options.categoryAxis, axisIx, axesLength = categoryAxes.length, data = chart._navigatorDataSource.view(), currentAxis, naviCategories;
                this._bindNavigatorSeries(chart.options.series, data);
                if (chart._sourceSeries) {
                    this._bindNavigatorSeries(chart._sourceSeries, data);
                }
                for (axisIx = 0; axisIx < axesLength; axisIx++) {
                    currentAxis = categoryAxes[axisIx];
                    if (currentAxis.pane == NAVIGATOR_PANE) {
                        if (currentAxis.name == NAVIGATOR_AXIS) {
                            chart._bindCategoryAxis(currentAxis, data, axisIx);
                            naviCategories = currentAxis.categories;
                        } else {
                            currentAxis.categories = naviCategories;
                        }
                    }
                }
                if (instance._model) {
                    var navigator = this.navigator;
                    navigator.redraw();
                    navigator.setRange();
                    if (!chart.options.dataSource || chart.options.dataSource && chart._dataBound) {
                        navigator.redrawSlaves();
                    }
                }
            },
            _bindCategories: function () {
                Chart.fn._bindCategories.call(this);
                if (this._instance) {
                    this._instance.copyNavigatorCategories();
                }
            },
            _onDataChanged: function () {
                Chart.fn._onDataChanged.call(this);
                this._dataBound = true;
            },
            setOptions: function (options) {
                this._removeNavigatorDataSource();
                this._initNavigatorOptions(options);
                this._instance.destroyNavigator();
                Chart.fn.setOptions.call(this, options);
            },
            _onNavigatorFilter: function (e) {
                this.dataSource.filter(buildFilter(e.from, e.to));
            },
            requiresHandlers: function (names) {
                if (dataviz.inArray('navigatorFilter', names)) {
                    var dataSource = this.dataSource;
                    var hasServerFiltering = dataSource && dataSource.options.serverFiltering;
                    return hasServerFiltering && this.options.navigator.dataSource;
                }
                return Chart.fn.requiresHandlers.call(this, names);
            },
            _removeNavigatorDataSource: function () {
                var navigatorDataSource = this._navigatorDataSource;
                if (navigatorDataSource) {
                    navigatorDataSource.unbind(CHANGE, this._navigatorDataChangedHandler);
                    delete this._navigatorDataSource;
                }
            },
            destroy: function () {
                Chart.fn.destroy.call(this);
                this._removeNavigatorDataSource();
            }
        });
        dataviz.ui.plugin(StockChart);
        function buildFilter(from, to) {
            return [
                {
                    field: 'Date',
                    operator: 'gte',
                    value: from
                },
                {
                    field: 'Date',
                    operator: 'lt',
                    value: to
                }
            ];
        }
    }(window.kendo.jQuery));
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.dataviz.stock', [
        'dataviz/stock/kendo-stock-chart',
        'dataviz/stock/stock-chart'
    ], f);
}(function () {
    var __meta__ = {
        id: 'dataviz.stockchart',
        name: 'StockChart',
        category: 'dataviz',
        description: 'StockChart widget and associated financial series.',
        depends: ['dataviz.chart']
    };
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));