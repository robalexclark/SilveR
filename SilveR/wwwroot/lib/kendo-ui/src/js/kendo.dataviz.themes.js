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
    define('dataviz/themes/chart-base-theme', ['kendo.dataviz.core'], f);
}(function () {
    (function () {
        window.kendo.dataviz = window.kendo.dataviz || {};
        var BAR_GAP = 1.5;
        var BAR_SPACING = 0.4;
        var BLACK = '#000';
        var SANS = 'Arial, Helvetica, sans-serif';
        var SANS11 = '11px ' + SANS;
        var SANS12 = '12px ' + SANS;
        var SANS16 = '16px ' + SANS;
        var TRANSPARENT = 'transparent';
        var WHITE = '#fff';
        var notes = function () {
            return {
                icon: { border: { width: 1 } },
                label: {
                    font: SANS12,
                    padding: 3
                },
                line: {
                    length: 10,
                    width: 2
                },
                visible: true
            };
        };
        var axisDefaults = function () {
            return {
                labels: { font: SANS12 },
                notes: notes(),
                title: {
                    font: SANS16,
                    margin: 5
                }
            };
        };
        var areaSeries = function () {
            return {
                highlight: { markers: { border: {} } },
                line: {
                    opacity: 1,
                    width: 0
                },
                markers: {
                    size: 6,
                    visible: false
                },
                opacity: 0.4
            };
        };
        var rangeAreaSeries = function () {
            return {
                highlight: { markers: { border: {} } },
                line: {
                    opacity: 1,
                    width: 0
                },
                markers: {
                    size: 6,
                    visible: false
                },
                opacity: 0.4
            };
        };
        var barSeries = function () {
            return {
                gap: BAR_GAP,
                spacing: BAR_SPACING
            };
        };
        var boxPlotSeries = function () {
            return {
                outliersField: '',
                meanField: '',
                border: {
                    _brightness: 0.8,
                    width: 1
                },
                downColor: WHITE,
                gap: 1,
                highlight: {
                    border: {
                        opacity: 1,
                        width: 2
                    },
                    whiskers: { width: 3 },
                    mean: { width: 2 },
                    median: { width: 2 }
                },
                mean: { width: 2 },
                median: { width: 2 },
                spacing: 0.3,
                whiskers: { width: 2 }
            };
        };
        var bubbleSeries = function () {
            return {
                border: { width: 0 },
                labels: { background: TRANSPARENT },
                opacity: 0.6
            };
        };
        var bulletSeries = function () {
            return {
                gap: BAR_GAP,
                spacing: BAR_SPACING,
                target: { color: '#ff0000' }
            };
        };
        var candlestickSeries = function () {
            return {
                border: {
                    _brightness: 0.8,
                    width: 1
                },
                downColor: WHITE,
                gap: 1,
                highlight: {
                    border: {
                        opacity: 1,
                        width: 2
                    },
                    line: { width: 2 }
                },
                line: {
                    color: BLACK,
                    width: 1
                },
                spacing: 0.3
            };
        };
        var columnSeries = function () {
            return {
                gap: BAR_GAP,
                spacing: BAR_SPACING
            };
        };
        var donutSeries = function () {
            return { margin: 1 };
        };
        var lineSeries = function () {
            return { width: 2 };
        };
        var ohlcSeries = function () {
            return {
                gap: 1,
                highlight: {
                    line: {
                        opacity: 1,
                        width: 3
                    }
                },
                line: { width: 1 },
                spacing: 0.3
            };
        };
        var radarAreaSeries = function () {
            return {
                line: {
                    opacity: 1,
                    width: 0
                },
                markers: {
                    size: 6,
                    visible: false
                },
                opacity: 0.5
            };
        };
        var radarLineSeries = function () {
            return {
                markers: { visible: false },
                width: 2
            };
        };
        var rangeBarSeries = function () {
            return {
                gap: BAR_GAP,
                spacing: BAR_SPACING
            };
        };
        var rangeColumnSeries = function () {
            return {
                gap: BAR_GAP,
                spacing: BAR_SPACING
            };
        };
        var scatterLineSeries = function () {
            return { width: 1 };
        };
        var waterfallSeries = function () {
            return {
                gap: 0.5,
                line: {
                    color: BLACK,
                    width: 1
                },
                spacing: BAR_SPACING
            };
        };
        var pieSeries = function () {
            return {
                labels: {
                    background: '',
                    color: '',
                    padding: {
                        top: 5,
                        bottom: 5,
                        left: 7,
                        right: 7
                    }
                }
            };
        };
        var funnelSeries = function () {
            return {
                labels: {
                    background: '',
                    color: '',
                    padding: {
                        top: 5,
                        bottom: 5,
                        left: 7,
                        right: 7
                    }
                }
            };
        };
        var seriesDefaults = function (options) {
            return {
                visible: true,
                labels: { font: SANS11 },
                overlay: options.gradients ? {} : { gradient: 'none' },
                area: areaSeries(),
                rangeArea: rangeAreaSeries(),
                verticalRangeArea: rangeAreaSeries(),
                bar: barSeries(),
                boxPlot: boxPlotSeries(),
                bubble: bubbleSeries(),
                bullet: bulletSeries(),
                candlestick: candlestickSeries(),
                column: columnSeries(),
                pie: pieSeries(),
                donut: donutSeries(),
                funnel: funnelSeries(),
                horizontalWaterfall: waterfallSeries(),
                line: lineSeries(),
                notes: notes(),
                ohlc: ohlcSeries(),
                radarArea: radarAreaSeries(),
                radarLine: radarLineSeries(),
                polarArea: radarAreaSeries(),
                polarLine: radarLineSeries(),
                rangeBar: rangeBarSeries(),
                rangeColumn: rangeColumnSeries(),
                scatterLine: scatterLineSeries(),
                verticalArea: areaSeries(),
                verticalBoxPlot: boxPlotSeries(),
                verticalBullet: bulletSeries(),
                verticalLine: lineSeries(),
                waterfall: waterfallSeries()
            };
        };
        var title = function () {
            return { font: SANS16 };
        };
        var legend = function () {
            return { labels: { font: SANS12 } };
        };
        var baseTheme = function (options) {
            if (options === void 0) {
                options = {};
            }
            return {
                axisDefaults: axisDefaults(),
                categoryAxis: { majorGridLines: { visible: true } },
                navigator: {
                    pane: {
                        height: 90,
                        margin: { top: 10 }
                    }
                },
                seriesDefaults: seriesDefaults(options),
                title: title(),
                legend: legend()
            };
        };
        kendo.deepExtend(kendo.dataviz, { chartBaseTheme: baseTheme });
    }());
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/themes/auto-theme', ['kendo.dataviz.core'], f);
}(function () {
    var cache;
    function autoTheme(force) {
        if (!force && cache) {
            return cache;
        }
        var theme = { chart: kendo.dataviz.chartBaseTheme() };
        var hook = $('<div style="display: none">' + '  <div class="k-var--accent"></div>' + '  <div class="k-var--accent-contrast"></div>' + '  <div class="k-var--base"></div>' + '  <div class="k-var--background"></div>' + '  <div class="k-var--normal-background"></div>' + '  <div class="k-var--normal-text-color"></div>' + '  <div class="k-var--hover-background"></div>' + '  <div class="k-var--hover-text-color"></div>' + '  <div class="k-var--selected-background"></div>' + '  <div class="k-var--selected-text-color"></div>' + '  <div class="k-var--chart-error-bars-background"></div>' + '  <div class="k-var--chart-notes-background"></div>' + '  <div class="k-var--chart-notes-border"></div>' + '  <div class="k-var--chart-notes-lines"></div>' + '  <div class="k-var--chart-crosshair-background"></div>' + '  <div class="k-var--chart-inactive"></div>' + '  <div class="k-var--chart-major-lines"></div>' + '  <div class="k-var--chart-minor-lines"></div>' + '  <div class="k-var--chart-area-opacity"></div>' + '  <div class="k-widget">' + '      <div class="k-var--chart-font"></div>' + '      <div class="k-var--chart-title-font"></div>' + '      <div class="k-var--chart-label-font"></div>' + '  </div>' + '  <div class="k-var--series">' + '    <div class="k-var--series-a"></div>' + '    <div class="k-var--series-b"></div>' + '    <div class="k-var--series-c"></div>' + '    <div class="k-var--series-d"></div>' + '    <div class="k-var--series-e"></div>' + '    <div class="k-var--series-f"></div>' + '  </div>' + '</div>').appendTo(document.body);
        function mapColor(key, varName) {
            set(key, queryStyle(varName, 'backgroundColor'));
        }
        function queryStyle(varName, prop) {
            return hook.find('.k-var--' + varName).css(prop);
        }
        function set(path, value) {
            var store = theme;
            var parts = path.split('.');
            var key = parts.shift();
            while (parts.length > 0) {
                store = store[key] = store[key] || {};
                key = parts.shift();
            }
            store[key] = value;
        }
        (function setColors() {
            mapColor('chart.axisDefaults.crosshair.color', 'chart-crosshair-background');
            mapColor('chart.axisDefaults.labels.color', 'normal-text-color');
            mapColor('chart.axisDefaults.line.color', 'chart-major-lines');
            mapColor('chart.axisDefaults.majorGridLines.color', 'chart-major-lines');
            mapColor('chart.axisDefaults.minorGridLines.color', 'chart-minor-lines');
            mapColor('chart.axisDefaults.notes.icon.background', 'chart-notes-background');
            mapColor('chart.axisDefaults.notes.icon.border.color', 'chart-notes-border');
            mapColor('chart.axisDefaults.notes.line.color', 'chart-notes-lines');
            mapColor('chart.axisDefaults.title.color', 'normal-text-color');
            mapColor('chart.chartArea.background', 'background');
            mapColor('chart.legend.inactiveItems.labels.color', 'chart-inactive');
            mapColor('chart.legend.inactiveItems.markers.color', 'chart-inactive');
            mapColor('chart.legend.labels.color', 'normal-text-color');
            mapColor('chart.seriesDefaults.boxPlot.downColor', 'chart-major-lines');
            mapColor('chart.seriesDefaults.boxPlot.mean.color', 'base');
            mapColor('chart.seriesDefaults.boxPlot.median.color', 'base');
            mapColor('chart.seriesDefaults.boxPlot.whiskers.color', 'accent');
            mapColor('chart.seriesDefaults.bullet.target.color', 'accent');
            mapColor('chart.seriesDefaults.candlestick.downColor', 'normal-text-color');
            mapColor('chart.seriesDefaults.candlestick.line.color', 'normal-text-color');
            mapColor('chart.seriesDefaults.errorBars.color', 'chart-error-bars-background');
            mapColor('chart.seriesDefaults.horizontalWaterfall.line.color', 'chart-major-lines');
            mapColor('chart.seriesDefaults.icon.border.color', 'chart-major-lines');
            mapColor('chart.seriesDefaults.labels.background', 'background');
            mapColor('chart.seriesDefaults.labels.color', 'normal-text-color');
            mapColor('chart.seriesDefaults.notes.icon.background', 'chart-notes-background');
            mapColor('chart.seriesDefaults.notes.icon.border.color', 'chart-notes-border');
            mapColor('chart.seriesDefaults.notes.line.color', 'chart-notes-lines');
            mapColor('chart.seriesDefaults.verticalBoxPlot.downColor', 'chart-major-lines');
            mapColor('chart.seriesDefaults.verticalBoxPlot.mean.color', 'base');
            mapColor('chart.seriesDefaults.verticalBoxPlot.median.color', 'base');
            mapColor('chart.seriesDefaults.verticalBoxPlot.whiskers.color', 'accent');
            mapColor('chart.seriesDefaults.verticalBullet.target.color', 'accent');
            mapColor('chart.seriesDefaults.waterfall.line.color', 'chart-major-lines');
            mapColor('chart.title.color', 'normal-text-color');
            set('chart.seriesDefaults.labels.opacity', queryStyle('chart-area-opacity', 'opacity'));
            mapColor('diagram.shapeDefaults.fill.color', 'accent');
            mapColor('diagram.shapeDefaults.content.color', 'accent-contrast');
            mapColor('diagram.shapeDefaults.connectorDefaults.fill.color', 'normal-text-color');
            mapColor('diagram.shapeDefaults.connectorDefaults.stroke.color', 'accent-contrast');
            mapColor('diagram.shapeDefaults.connectorDefaults.hover.fill.color', 'accent-contrast');
            mapColor('diagram.shapeDefaults.connectorDefaults.hover.stroke.color', 'normal-text-color');
            mapColor('diagram.editable.resize.handles.stroke.color', 'normal-text-color');
            mapColor('diagram.editable.resize.handles.fill.color', 'normal-background');
            mapColor('diagram.editable.resize.handles.hover.stroke.color', 'normal-text-color');
            mapColor('diagram.editable.resize.handles.hover.fill.color', 'normal-text-color');
            mapColor('diagram.selectable.stroke.color', 'normal-text-color');
            mapColor('diagram.connectionDefaults.stroke.color', 'normal-text-color');
            mapColor('diagram.connectionDefaults.content.color', 'normal-text-color');
            mapColor('diagram.connectionDefaults.selection.handles.fill.color', 'accent-contrast');
            mapColor('diagram.connectionDefaults.selection.handles.stroke.color', 'normal-text-color');
            mapColor('diagram.connectionDefaults.selection.stroke.color', 'normal-text-color');
        }());
        (function setFonts() {
            function font(varName) {
                return queryStyle(varName, 'fontSize') + ' ' + queryStyle(varName, 'fontFamily');
            }
            var defaultFont = font('chart-font');
            var titleFont = font('chart-title-font');
            var labelFont = font('chart-label-font');
            set('chart.axisDefaults.labels.font', labelFont);
            set('chart.axisDefaults.notes.label.font', defaultFont);
            set('chart.axisDefaults.title.font', defaultFont);
            set('chart.legend.labels.font', defaultFont);
            set('chart.seriesDefaults.labels.font', labelFont);
            set('chart.seriesDefaults.notes.label.font', defaultFont);
            set('chart.title.font', titleFont);
        }());
        (function setSeriesColors() {
            function letterPos(letter) {
                return letter.toLowerCase().charCodeAt(0) - 'a'.charCodeAt(0);
            }
            function seriesPos(name) {
                return letterPos(name.match(/series-([a-z])$/)[1]);
            }
            var series = $('.k-var--series div').toArray();
            var seriesColors = series.reduce(function (arr, el) {
                var pos = seriesPos(el.className);
                arr[pos] = $(el).css('backgroundColor');
                return arr;
            }, []);
            set('chart.seriesColors', seriesColors);
        }());
        hook.remove();
        cache = theme;
        return theme;
    }
    kendo.dataviz.autoTheme = autoTheme;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('dataviz/themes/themes', ['dataviz/themes/chart-base-theme'], f);
}(function () {
    (function ($) {
        var kendo = window.kendo, ui = kendo.dataviz.ui, deepExtend = kendo.deepExtend;
        var BLACK = '#000', SANS = 'Arial,Helvetica,sans-serif', SANS12 = '12px ' + SANS, WHITE = '#fff';
        var chartBaseTheme = kendo.dataviz.chartBaseTheme({ gradients: true });
        var gaugeBaseTheme = { scale: { labels: { font: SANS12 } } };
        var diagramBaseTheme = {
            shapeDefaults: {
                hover: { opacity: 0.2 },
                stroke: { width: 0 }
            },
            editable: {
                resize: {
                    handles: {
                        width: 7,
                        height: 7
                    }
                }
            },
            selectable: {
                stroke: {
                    width: 1,
                    dashType: 'dot'
                }
            },
            connectionDefaults: {
                stroke: { width: 2 },
                selection: {
                    handles: {
                        width: 8,
                        height: 8
                    }
                },
                editable: {
                    tools: [
                        'edit',
                        'delete'
                    ]
                }
            }
        };
        var themes = ui.themes, registerTheme = ui.registerTheme = function (themeName, options) {
                var result = {};
                result.chart = deepExtend({}, chartBaseTheme, options.chart);
                result.gauge = deepExtend({}, gaugeBaseTheme, options.gauge);
                result.diagram = deepExtend({}, diagramBaseTheme, options.diagram);
                result.treeMap = deepExtend({}, options.treeMap);
                var defaults = result.chart.seriesDefaults;
                defaults.verticalLine = deepExtend({}, defaults.line);
                defaults.verticalArea = deepExtend({}, defaults.area);
                defaults.rangeArea = deepExtend({}, defaults.area);
                defaults.verticalRangeArea = deepExtend({}, defaults.rangeArea);
                defaults.verticalBoxPlot = deepExtend({}, defaults.boxPlot);
                defaults.polarArea = deepExtend({}, defaults.radarArea);
                defaults.polarLine = deepExtend({}, defaults.radarLine);
                themes[themeName] = result;
            };
        registerTheme('black', {
            chart: {
                title: { color: WHITE },
                legend: {
                    labels: { color: WHITE },
                    inactiveItems: {
                        labels: { color: '#919191' },
                        markers: { color: '#919191' }
                    }
                },
                seriesDefaults: {
                    labels: { color: WHITE },
                    errorBars: { color: WHITE },
                    notes: {
                        icon: {
                            background: '#3b3b3b',
                            border: { color: '#8e8e8e' }
                        },
                        label: { color: WHITE },
                        line: { color: '#8e8e8e' }
                    },
                    pie: { overlay: { gradient: 'sharpBevel' } },
                    donut: { overlay: { gradient: 'sharpGlass' } },
                    line: { markers: { background: '#3d3d3d' } },
                    scatter: { markers: { background: '#3d3d3d' } },
                    scatterLine: { markers: { background: '#3d3d3d' } },
                    waterfall: { line: { color: '#8e8e8e' } },
                    horizontalWaterfall: { line: { color: '#8e8e8e' } },
                    candlestick: {
                        downColor: '#555',
                        line: { color: WHITE },
                        border: {
                            _brightness: 1.5,
                            opacity: 1
                        },
                        highlight: {
                            border: {
                                color: WHITE,
                                opacity: 0.2
                            }
                        }
                    },
                    ohlc: { line: { color: WHITE } }
                },
                chartArea: { background: '#3d3d3d' },
                seriesColors: [
                    '#0081da',
                    '#3aafff',
                    '#99c900',
                    '#ffeb3d',
                    '#b20753',
                    '#ff4195'
                ],
                axisDefaults: {
                    line: { color: '#8e8e8e' },
                    labels: { color: WHITE },
                    majorGridLines: { color: '#545454' },
                    minorGridLines: { color: '#454545' },
                    title: { color: WHITE },
                    crosshair: { color: '#8e8e8e' },
                    notes: {
                        icon: {
                            background: '#3b3b3b',
                            border: { color: '#8e8e8e' }
                        },
                        label: { color: WHITE },
                        line: { color: '#8e8e8e' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#0070e4' },
                scale: {
                    rangePlaceholderColor: '#1d1d1d',
                    labels: { color: WHITE },
                    minorTicks: { color: WHITE },
                    majorTicks: { color: WHITE },
                    line: { color: WHITE }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#0066cc' },
                    connectorDefaults: {
                        fill: { color: WHITE },
                        stroke: { color: '#384049' },
                        hover: {
                            fill: { color: '#3d3d3d' },
                            stroke: { color: '#efefef' }
                        }
                    },
                    content: { color: WHITE }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: '#3d3d3d' },
                            stroke: { color: WHITE },
                            hover: {
                                fill: { color: WHITE },
                                stroke: { color: WHITE }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: WHITE },
                            fill: { color: WHITE }
                        }
                    }
                },
                selectable: { stroke: { color: WHITE } },
                connectionDefaults: {
                    stroke: { color: WHITE },
                    content: { color: WHITE },
                    selection: {
                        handles: {
                            fill: { color: '#3d3d3d' },
                            stroke: { color: '#efefef' }
                        }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#0081da',
                        '#314b5c'
                    ],
                    [
                        '#3aafff',
                        '#3c5464'
                    ],
                    [
                        '#99c900',
                        '#4f5931'
                    ],
                    [
                        '#ffeb3d',
                        '#64603d'
                    ],
                    [
                        '#b20753',
                        '#543241'
                    ],
                    [
                        '#ff4195',
                        '#643e4f'
                    ]
                ]
            }
        });
        registerTheme('blueopal', {
            chart: {
                title: { color: '#293135' },
                legend: {
                    labels: { color: '#293135' },
                    inactiveItems: {
                        labels: { color: '#27A5BA' },
                        markers: { color: '#27A5BA' }
                    }
                },
                seriesDefaults: {
                    labels: {
                        color: BLACK,
                        background: WHITE,
                        opacity: 0.5
                    },
                    errorBars: { color: '#293135' },
                    candlestick: {
                        downColor: '#c4d0d5',
                        line: { color: '#9aabb2' }
                    },
                    waterfall: { line: { color: '#9aabb2' } },
                    horizontalWaterfall: { line: { color: '#9aabb2' } },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#9aabb2' }
                        },
                        label: { color: '#293135' },
                        line: { color: '#9aabb2' }
                    }
                },
                seriesColors: [
                    '#0069a5',
                    '#0098ee',
                    '#7bd2f6',
                    '#ffb800',
                    '#ff8517',
                    '#e34a00'
                ],
                axisDefaults: {
                    line: { color: '#9aabb2' },
                    labels: { color: '#293135' },
                    majorGridLines: { color: '#c4d0d5' },
                    minorGridLines: { color: '#edf1f2' },
                    title: { color: '#293135' },
                    crosshair: { color: '#9aabb2' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#9aabb2' }
                        },
                        label: { color: '#293135' },
                        line: { color: '#9aabb2' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#005c83' },
                scale: {
                    rangePlaceholderColor: '#daecf4',
                    labels: { color: '#293135' },
                    minorTicks: { color: '#293135' },
                    majorTicks: { color: '#293135' },
                    line: { color: '#293135' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#7ec6e3' },
                    connectorDefaults: {
                        fill: { color: '#003f59' },
                        stroke: { color: WHITE },
                        hover: {
                            fill: { color: WHITE },
                            stroke: { color: '#003f59' }
                        }
                    },
                    content: { color: '#293135' }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#003f59' },
                            hover: {
                                fill: { color: '#003f59' },
                                stroke: { color: '#003f59' }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: '#003f59' },
                            fill: { color: '#003f59' }
                        }
                    }
                },
                selectable: { stroke: { color: '#003f59' } },
                connectionDefaults: {
                    stroke: { color: '#003f59' },
                    content: { color: '#293135' },
                    selection: {
                        handles: {
                            fill: { color: '#3d3d3d' },
                            stroke: { color: '#efefef' }
                        }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#0069a5',
                        '#bad7e7'
                    ],
                    [
                        '#0098ee',
                        '#b9e0f5'
                    ],
                    [
                        '#7bd2f6',
                        '#ceeaf6'
                    ],
                    [
                        '#ffb800',
                        '#e6e3c4'
                    ],
                    [
                        '#ff8517',
                        '#e4d8c8'
                    ],
                    [
                        '#e34a00',
                        '#ddccc2'
                    ]
                ]
            }
        });
        registerTheme('highcontrast', {
            chart: {
                title: { color: '#ffffff' },
                legend: {
                    labels: { color: '#ffffff' },
                    inactiveItems: {
                        labels: { color: '#66465B' },
                        markers: { color: '#66465B' }
                    }
                },
                seriesDefaults: {
                    labels: { color: '#ffffff' },
                    errorBars: { color: '#ffffff' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#ffffff' }
                        },
                        label: { color: '#ffffff' },
                        line: { color: '#ffffff' }
                    },
                    pie: { overlay: { gradient: 'sharpGlass' } },
                    donut: { overlay: { gradient: 'sharpGlass' } },
                    line: { markers: { background: '#2c232b' } },
                    scatter: { markers: { background: '#2c232b' } },
                    scatterLine: { markers: { background: '#2c232b' } },
                    area: { opacity: 0.5 },
                    waterfall: { line: { color: '#ffffff' } },
                    horizontalWaterfall: { line: { color: '#ffffff' } },
                    candlestick: {
                        downColor: '#664e62',
                        line: { color: '#ffffff' },
                        border: {
                            _brightness: 1.5,
                            opacity: 1
                        },
                        highlight: {
                            border: {
                                color: '#ffffff',
                                opacity: 1
                            }
                        }
                    },
                    ohlc: { line: { color: '#ffffff' } }
                },
                chartArea: { background: '#2c232b' },
                seriesColors: [
                    '#a7008f',
                    '#ffb800',
                    '#3aafff',
                    '#99c900',
                    '#b20753',
                    '#ff4195'
                ],
                axisDefaults: {
                    line: { color: '#ffffff' },
                    labels: { color: '#ffffff' },
                    majorGridLines: { color: '#664e62' },
                    minorGridLines: { color: '#4f394b' },
                    title: { color: '#ffffff' },
                    crosshair: { color: '#ffffff' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#ffffff' }
                        },
                        label: { color: '#ffffff' },
                        line: { color: '#ffffff' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#a7008f' },
                scale: {
                    rangePlaceholderColor: '#2c232b',
                    labels: { color: '#ffffff' },
                    minorTicks: { color: '#2c232b' },
                    majorTicks: { color: '#664e62' },
                    line: { color: '#ffffff' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#a7018f' },
                    connectorDefaults: {
                        fill: { color: WHITE },
                        stroke: { color: '#2c232b' },
                        hover: {
                            fill: { color: '#2c232b' },
                            stroke: { color: WHITE }
                        }
                    },
                    content: { color: WHITE }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: '#2c232b' },
                            stroke: { color: WHITE },
                            hover: {
                                fill: { color: WHITE },
                                stroke: { color: WHITE }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: WHITE },
                            fill: { color: WHITE }
                        }
                    }
                },
                selectable: { stroke: { color: WHITE } },
                connectionDefaults: {
                    stroke: { color: WHITE },
                    content: { color: WHITE },
                    selection: {
                        handles: {
                            fill: { color: '#2c232b' },
                            stroke: { color: WHITE }
                        }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#a7008f',
                        '#451c3f'
                    ],
                    [
                        '#ffb800',
                        '#564122'
                    ],
                    [
                        '#3aafff',
                        '#2f3f55'
                    ],
                    [
                        '#99c900',
                        '#424422'
                    ],
                    [
                        '#b20753',
                        '#471d33'
                    ],
                    [
                        '#ff4195',
                        '#562940'
                    ]
                ]
            }
        });
        registerTheme('default', {
            chart: {
                title: { color: '#8e8e8e' },
                legend: {
                    labels: { color: '#232323' },
                    inactiveItems: {
                        labels: { color: '#919191' },
                        markers: { color: '#919191' }
                    }
                },
                seriesDefaults: {
                    labels: {
                        color: BLACK,
                        background: WHITE,
                        opacity: 0.5
                    },
                    errorBars: { color: '#232323' },
                    candlestick: {
                        downColor: '#dedede',
                        line: { color: '#8d8d8d' }
                    },
                    waterfall: { line: { color: '#8e8e8e' } },
                    horizontalWaterfall: { line: { color: '#8e8e8e' } },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#8e8e8e' }
                        },
                        label: { color: '#232323' },
                        line: { color: '#8e8e8e' }
                    }
                },
                seriesColors: [
                    '#ff6800',
                    '#a0a700',
                    '#ff8d00',
                    '#678900',
                    '#ffb53c',
                    '#396000'
                ],
                axisDefaults: {
                    line: { color: '#8e8e8e' },
                    labels: { color: '#232323' },
                    minorGridLines: { color: '#f0f0f0' },
                    majorGridLines: { color: '#dfdfdf' },
                    title: { color: '#232323' },
                    crosshair: { color: '#8e8e8e' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#8e8e8e' }
                        },
                        label: { color: '#232323' },
                        line: { color: '#8e8e8e' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#ea7001' },
                scale: {
                    rangePlaceholderColor: '#dedede',
                    labels: { color: '#2e2e2e' },
                    minorTicks: { color: '#2e2e2e' },
                    majorTicks: { color: '#2e2e2e' },
                    line: { color: '#2e2e2e' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#e15613' },
                    connectorDefaults: {
                        fill: { color: '#282828' },
                        stroke: { color: WHITE },
                        hover: {
                            fill: { color: WHITE },
                            stroke: { color: '#282828' }
                        }
                    },
                    content: { color: '#2e2e2e' }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#282828' },
                            hover: {
                                fill: { color: '#282828' },
                                stroke: { color: '#282828' }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: '#282828' },
                            fill: { color: '#282828' }
                        }
                    }
                },
                selectable: { stroke: { color: '#a7018f' } },
                connectionDefaults: {
                    stroke: { color: '#282828' },
                    content: { color: '#2e2e2e' },
                    selection: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#282828' }
                        }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#ff6800',
                        '#edcfba'
                    ],
                    [
                        '#a0a700',
                        '#dadcba'
                    ],
                    [
                        '#ff8d00',
                        '#edd7ba'
                    ],
                    [
                        '#678900',
                        '#cfd6ba'
                    ],
                    [
                        '#ffb53c',
                        '#eddfc6'
                    ],
                    [
                        '#396000',
                        '#c6ceba'
                    ]
                ]
            }
        });
        registerTheme('silver', {
            chart: {
                title: { color: '#4e5968' },
                legend: {
                    labels: { color: '#4e5968' },
                    inactiveItems: {
                        labels: { color: '#B1BCC8' },
                        markers: { color: '#B1BCC8' }
                    }
                },
                seriesDefaults: {
                    labels: {
                        color: '#293135',
                        background: '#eaeaec',
                        opacity: 0.5
                    },
                    errorBars: { color: '#4e5968' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#4e5968' }
                        },
                        label: { color: '#4e5968' },
                        line: { color: '#4e5968' }
                    },
                    line: { markers: { background: '#eaeaec' } },
                    scatter: { markers: { background: '#eaeaec' } },
                    scatterLine: { markers: { background: '#eaeaec' } },
                    pie: { connectors: { color: '#A6B1C0' } },
                    donut: { connectors: { color: '#A6B1C0' } },
                    waterfall: { line: { color: '#a6b1c0' } },
                    horizontalWaterfall: { line: { color: '#a6b1c0' } },
                    candlestick: { downColor: '#a6afbe' }
                },
                chartArea: { background: '#eaeaec' },
                seriesColors: [
                    '#007bc3',
                    '#76b800',
                    '#ffae00',
                    '#ef4c00',
                    '#a419b7',
                    '#430B62'
                ],
                axisDefaults: {
                    line: { color: '#a6b1c0' },
                    labels: { color: '#4e5968' },
                    majorGridLines: { color: '#dcdcdf' },
                    minorGridLines: { color: '#eeeeef' },
                    title: { color: '#4e5968' },
                    crosshair: { color: '#a6b1c0' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#4e5968' }
                        },
                        label: { color: '#4e5968' },
                        line: { color: '#4e5968' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#0879c0' },
                scale: {
                    rangePlaceholderColor: '#f3f3f4',
                    labels: { color: '#515967' },
                    minorTicks: { color: '#515967' },
                    majorTicks: { color: '#515967' },
                    line: { color: '#515967' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#1c82c2' },
                    connectorDefaults: {
                        fill: { color: '#515967' },
                        stroke: { color: WHITE },
                        hover: {
                            fill: { color: WHITE },
                            stroke: { color: '#282828' }
                        }
                    },
                    content: { color: '#515967' }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#515967' },
                            hover: {
                                fill: { color: '#515967' },
                                stroke: { color: '#515967' }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: '#515967' },
                            fill: { color: '#515967' }
                        }
                    }
                },
                selectable: { stroke: { color: '#515967' } },
                connectionDefaults: {
                    stroke: { color: '#515967' },
                    content: { color: '#515967' },
                    selection: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#515967' }
                        }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#007bc3',
                        '#c2dbea'
                    ],
                    [
                        '#76b800',
                        '#dae7c3'
                    ],
                    [
                        '#ffae00',
                        '#f5e5c3'
                    ],
                    [
                        '#ef4c00',
                        '#f2d2c3'
                    ],
                    [
                        '#a419b7',
                        '#e3c7e8'
                    ],
                    [
                        '#430b62',
                        '#d0c5d7'
                    ]
                ]
            }
        });
        registerTheme('metro', {
            chart: {
                title: { color: '#777777' },
                legend: {
                    labels: { color: '#777777' },
                    inactiveItems: {
                        labels: { color: '#CBCBCB' },
                        markers: { color: '#CBCBCB' }
                    }
                },
                seriesDefaults: {
                    labels: { color: BLACK },
                    errorBars: { color: '#777777' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#777777' }
                        },
                        label: { color: '#777777' },
                        line: { color: '#777777' }
                    },
                    candlestick: {
                        downColor: '#c7c7c7',
                        line: { color: '#787878' }
                    },
                    waterfall: { line: { color: '#c7c7c7' } },
                    horizontalWaterfall: { line: { color: '#c7c7c7' } },
                    overlay: { gradient: 'none' },
                    border: { _brightness: 1 }
                },
                seriesColors: [
                    '#8ebc00',
                    '#309b46',
                    '#25a0da',
                    '#ff6900',
                    '#e61e26',
                    '#d8e404',
                    '#16aba9',
                    '#7e51a1',
                    '#313131',
                    '#ed1691'
                ],
                axisDefaults: {
                    line: { color: '#c7c7c7' },
                    labels: { color: '#777777' },
                    minorGridLines: { color: '#c7c7c7' },
                    majorGridLines: { color: '#c7c7c7' },
                    title: { color: '#777777' },
                    crosshair: { color: '#c7c7c7' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#777777' }
                        },
                        label: { color: '#777777' },
                        line: { color: '#777777' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#8ebc00' },
                scale: {
                    rangePlaceholderColor: '#e6e6e6',
                    labels: { color: '#777' },
                    minorTicks: { color: '#777' },
                    majorTicks: { color: '#777' },
                    line: { color: '#777' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#8ebc00' },
                    connectorDefaults: {
                        fill: { color: BLACK },
                        stroke: { color: WHITE },
                        hover: {
                            fill: { color: WHITE },
                            stroke: { color: BLACK }
                        }
                    },
                    content: { color: '#777' }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#787878' },
                            hover: {
                                fill: { color: '#787878' },
                                stroke: { color: '#787878' }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: '#787878' },
                            fill: { color: '#787878' }
                        }
                    }
                },
                selectable: { stroke: { color: '#515967' } },
                connectionDefaults: {
                    stroke: { color: '#787878' },
                    content: { color: '#777' },
                    selection: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#787878' }
                        }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#8ebc00',
                        '#e8f2cc'
                    ],
                    [
                        '#309b46',
                        '#d6ebda'
                    ],
                    [
                        '#25a0da',
                        '#d3ecf8'
                    ],
                    [
                        '#ff6900',
                        '#ffe1cc'
                    ],
                    [
                        '#e61e26',
                        '#fad2d4'
                    ],
                    [
                        '#d8e404',
                        '#f7facd'
                    ],
                    [
                        '#16aba9',
                        '#d0eeee'
                    ],
                    [
                        '#7e51a1',
                        '#e5dcec'
                    ],
                    [
                        '#313131',
                        '#d6d6d6'
                    ],
                    [
                        '#ed1691',
                        '#fbd0e9'
                    ]
                ]
            }
        });
        registerTheme('metroblack', {
            chart: {
                title: { color: '#ffffff' },
                legend: {
                    labels: { color: '#ffffff' },
                    inactiveItems: {
                        labels: { color: '#797979' },
                        markers: { color: '#797979' }
                    }
                },
                seriesDefaults: {
                    border: { _brightness: 1 },
                    labels: { color: '#ffffff' },
                    errorBars: { color: '#ffffff' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#cecece' }
                        },
                        label: { color: '#ffffff' },
                        line: { color: '#cecece' }
                    },
                    line: { markers: { background: '#0e0e0e' } },
                    bubble: { opacity: 0.6 },
                    scatter: { markers: { background: '#0e0e0e' } },
                    scatterLine: { markers: { background: '#0e0e0e' } },
                    candlestick: {
                        downColor: '#828282',
                        line: { color: '#ffffff' }
                    },
                    waterfall: { line: { color: '#cecece' } },
                    horizontalWaterfall: { line: { color: '#cecece' } },
                    overlay: { gradient: 'none' }
                },
                chartArea: { background: '#0e0e0e' },
                seriesColors: [
                    '#00aba9',
                    '#309b46',
                    '#8ebc00',
                    '#ff6900',
                    '#e61e26',
                    '#d8e404',
                    '#25a0da',
                    '#7e51a1',
                    '#313131',
                    '#ed1691'
                ],
                axisDefaults: {
                    line: { color: '#cecece' },
                    labels: { color: '#ffffff' },
                    minorGridLines: { color: '#2d2d2d' },
                    majorGridLines: { color: '#333333' },
                    title: { color: '#ffffff' },
                    crosshair: { color: '#cecece' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#cecece' }
                        },
                        label: { color: '#ffffff' },
                        line: { color: '#cecece' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#00aba9' },
                scale: {
                    rangePlaceholderColor: '#2d2d2d',
                    labels: { color: '#ffffff' },
                    minorTicks: { color: '#333333' },
                    majorTicks: { color: '#cecece' },
                    line: { color: '#cecece' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#00aba9' },
                    connectorDefaults: {
                        fill: { color: WHITE },
                        stroke: { color: '#0e0e0e' },
                        hover: {
                            fill: { color: '#0e0e0e' },
                            stroke: { color: WHITE }
                        }
                    },
                    content: { color: WHITE }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: '#0e0e0e' },
                            stroke: { color: '#787878' },
                            hover: {
                                fill: { color: '#787878' },
                                stroke: { color: '#787878' }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: WHITE },
                            fill: { color: WHITE }
                        }
                    }
                },
                selectable: { stroke: { color: '#787878' } },
                connectionDefaults: {
                    stroke: { color: WHITE },
                    content: { color: WHITE },
                    selection: {
                        handles: {
                            fill: { color: '#0e0e0e' },
                            stroke: { color: WHITE }
                        }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#00aba9',
                        '#0b2d2d'
                    ],
                    [
                        '#309b46',
                        '#152a19'
                    ],
                    [
                        '#8ebc00',
                        '#28310b'
                    ],
                    [
                        '#ff6900',
                        '#3e200b'
                    ],
                    [
                        '#e61e26',
                        '#391113'
                    ],
                    [
                        '#d8e404',
                        '#36390c'
                    ],
                    [
                        '#25a0da',
                        '#132b37'
                    ],
                    [
                        '#7e51a1',
                        '#241b2b'
                    ],
                    [
                        '#313131',
                        '#151515'
                    ],
                    [
                        '#ed1691',
                        '#3b1028'
                    ]
                ]
            }
        });
        registerTheme('moonlight', {
            chart: {
                title: { color: '#ffffff' },
                legend: {
                    labels: { color: '#ffffff' },
                    inactiveItems: {
                        labels: { color: '#A1A7AB' },
                        markers: { color: '#A1A7AB' }
                    }
                },
                seriesDefaults: {
                    labels: { color: '#ffffff' },
                    errorBars: { color: '#ffffff' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#8c909e' }
                        },
                        label: { color: '#ffffff' },
                        line: { color: '#8c909e' }
                    },
                    pie: { overlay: { gradient: 'sharpBevel' } },
                    donut: { overlay: { gradient: 'sharpGlass' } },
                    line: { markers: { background: '#212a33' } },
                    bubble: { opacity: 0.6 },
                    scatter: { markers: { background: '#212a33' } },
                    scatterLine: { markers: { background: '#212a33' } },
                    area: { opacity: 0.3 },
                    candlestick: {
                        downColor: '#757d87',
                        line: { color: '#ea9d06' },
                        border: {
                            _brightness: 1.5,
                            opacity: 1
                        },
                        highlight: {
                            border: {
                                color: WHITE,
                                opacity: 0.2
                            }
                        }
                    },
                    waterfall: { line: { color: '#8c909e' } },
                    horizontalWaterfall: { line: { color: '#8c909e' } },
                    ohlc: { line: { color: '#ea9d06' } }
                },
                chartArea: { background: '#212a33' },
                seriesColors: [
                    '#ffca08',
                    '#ff710f',
                    '#ed2e24',
                    '#ff9f03',
                    '#e13c02',
                    '#a00201'
                ],
                axisDefaults: {
                    line: { color: '#8c909e' },
                    minorTicks: { color: '#8c909e' },
                    majorTicks: { color: '#8c909e' },
                    labels: { color: '#ffffff' },
                    majorGridLines: { color: '#3e424d' },
                    minorGridLines: { color: '#2f3640' },
                    title: { color: '#ffffff' },
                    crosshair: { color: '#8c909e' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#8c909e' }
                        },
                        label: { color: '#ffffff' },
                        line: { color: '#8c909e' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#f4af03' },
                scale: {
                    rangePlaceholderColor: '#2f3640',
                    labels: { color: WHITE },
                    minorTicks: { color: '#8c909e' },
                    majorTicks: { color: '#8c909e' },
                    line: { color: '#8c909e' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#f3ae03' },
                    connectorDefaults: {
                        fill: { color: WHITE },
                        stroke: { color: '#414550' },
                        hover: {
                            fill: { color: '#414550' },
                            stroke: { color: WHITE }
                        }
                    },
                    content: { color: WHITE }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: '#414550' },
                            stroke: { color: WHITE },
                            hover: {
                                fill: { color: WHITE },
                                stroke: { color: WHITE }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: WHITE },
                            fill: { color: WHITE }
                        }
                    }
                },
                selectable: { stroke: { color: WHITE } },
                connectionDefaults: {
                    stroke: { color: WHITE },
                    content: { color: WHITE },
                    selection: {
                        handles: {
                            fill: { color: '#414550' },
                            stroke: { color: WHITE }
                        }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#ffca08',
                        '#4e4b2b'
                    ],
                    [
                        '#ff710f',
                        '#4e392d'
                    ],
                    [
                        '#ed2e24',
                        '#4b2c31'
                    ],
                    [
                        '#ff9f03',
                        '#4e422a'
                    ],
                    [
                        '#e13c02',
                        '#482e2a'
                    ],
                    [
                        '#a00201',
                        '#3b232a'
                    ]
                ]
            }
        });
        registerTheme('uniform', {
            chart: {
                title: { color: '#686868' },
                legend: {
                    labels: { color: '#686868' },
                    inactiveItems: {
                        labels: { color: '#B6B6B6' },
                        markers: { color: '#B6B6B6' }
                    }
                },
                seriesDefaults: {
                    labels: { color: '#686868' },
                    errorBars: { color: '#686868' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#9e9e9e' }
                        },
                        label: { color: '#686868' },
                        line: { color: '#9e9e9e' }
                    },
                    pie: { overlay: { gradient: 'sharpBevel' } },
                    donut: { overlay: { gradient: 'sharpGlass' } },
                    line: { markers: { background: '#ffffff' } },
                    bubble: { opacity: 0.6 },
                    scatter: { markers: { background: '#ffffff' } },
                    scatterLine: { markers: { background: '#ffffff' } },
                    area: { opacity: 0.3 },
                    candlestick: {
                        downColor: '#cccccc',
                        line: { color: '#cccccc' },
                        border: {
                            _brightness: 1.5,
                            opacity: 1
                        },
                        highlight: {
                            border: {
                                color: '#cccccc',
                                opacity: 0.2
                            }
                        }
                    },
                    waterfall: { line: { color: '#9e9e9e' } },
                    horizontalWaterfall: { line: { color: '#9e9e9e' } },
                    ohlc: { line: { color: '#cccccc' } }
                },
                chartArea: { background: '#ffffff' },
                seriesColors: [
                    '#527aa3',
                    '#6f91b3',
                    '#8ca7c2',
                    '#a8bdd1',
                    '#c5d3e0',
                    '#e2e9f0'
                ],
                axisDefaults: {
                    line: { color: '#9e9e9e' },
                    minorTicks: { color: '#aaaaaa' },
                    majorTicks: { color: '#888888' },
                    labels: { color: '#686868' },
                    majorGridLines: { color: '#dadada' },
                    minorGridLines: { color: '#e7e7e7' },
                    title: { color: '#686868' },
                    crosshair: { color: '#9e9e9e' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#9e9e9e' }
                        },
                        label: { color: '#686868' },
                        line: { color: '#9e9e9e' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#527aa3' },
                scale: {
                    rangePlaceholderColor: '#e7e7e7',
                    labels: { color: '#686868' },
                    minorTicks: { color: '#aaaaaa' },
                    majorTicks: { color: '#888888' },
                    line: { color: '#9e9e9e' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#d1d1d1' },
                    connectorDefaults: {
                        fill: { color: '#686868' },
                        stroke: { color: WHITE },
                        hover: {
                            fill: { color: WHITE },
                            stroke: { color: '#686868' }
                        }
                    },
                    content: { color: '#686868' }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#686868' },
                            hover: {
                                fill: { color: '#686868' },
                                stroke: { color: '#686868' }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: '#686868' },
                            fill: { color: '#686868' }
                        }
                    }
                },
                selectable: { stroke: { color: '#686868' } },
                connectionDefaults: {
                    stroke: { color: '#686868' },
                    content: { color: '#686868' },
                    selection: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#686868' }
                        }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#527aa3',
                        '#d0d8e1'
                    ],
                    [
                        '#6f91b3',
                        '#d6dde4'
                    ],
                    [
                        '#8ca7c2',
                        '#dce1e7'
                    ],
                    [
                        '#a8bdd1',
                        '#e2e6ea'
                    ],
                    [
                        '#c5d3e0',
                        '#e7eaed'
                    ],
                    [
                        '#e2e9f0',
                        '#edeff0'
                    ]
                ]
            }
        });
        registerTheme('bootstrap', {
            chart: {
                title: { color: '#333333' },
                legend: {
                    labels: { color: '#333333' },
                    inactiveItems: {
                        labels: { color: '#999999' },
                        markers: { color: '#9A9A9A' }
                    }
                },
                seriesDefaults: {
                    labels: { color: '#333333' },
                    overlay: { gradient: 'none' },
                    errorBars: { color: '#343434' },
                    notes: {
                        icon: {
                            background: '#000000',
                            border: { color: '#000000' }
                        },
                        label: { color: '#333333' },
                        line: { color: '#000000' }
                    },
                    pie: { overlay: { gradient: 'none' } },
                    donut: { overlay: { gradient: 'none' } },
                    line: { markers: { background: '#ffffff' } },
                    bubble: { opacity: 0.6 },
                    scatter: { markers: { background: '#ffffff' } },
                    scatterLine: { markers: { background: '#ffffff' } },
                    area: { opacity: 0.8 },
                    candlestick: {
                        downColor: '#d0d0d0',
                        line: { color: '#333333' },
                        border: {
                            _brightness: 1.5,
                            opacity: 1
                        },
                        highlight: {
                            border: {
                                color: '#b8b8b8',
                                opacity: 0.2
                            }
                        }
                    },
                    waterfall: { line: { color: '#cccccc' } },
                    horizontalWaterfall: { line: { color: '#cccccc' } },
                    ohlc: { line: { color: '#333333' } }
                },
                chartArea: { background: '#ffffff' },
                seriesColors: [
                    '#428bca',
                    '#5bc0de',
                    '#5cb85c',
                    '#f2b661',
                    '#e67d4a',
                    '#da3b36'
                ],
                axisDefaults: {
                    line: { color: '#cccccc' },
                    minorTicks: { color: '#ebebeb' },
                    majorTicks: { color: '#cccccc' },
                    labels: { color: '#333333' },
                    majorGridLines: { color: '#cccccc' },
                    minorGridLines: { color: '#ebebeb' },
                    title: { color: '#333333' },
                    crosshair: { color: '#000000' },
                    notes: {
                        icon: {
                            background: '#000000',
                            border: { color: '#000000' }
                        },
                        label: { color: '#ffffff' },
                        line: { color: '#000000' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#428bca' },
                scale: {
                    rangePlaceholderColor: '#cccccc',
                    labels: { color: '#333333' },
                    minorTicks: { color: '#ebebeb' },
                    majorTicks: { color: '#cccccc' },
                    line: { color: '#cccccc' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#428bca' },
                    connectorDefaults: {
                        fill: { color: '#333333' },
                        stroke: { color: WHITE },
                        hover: {
                            fill: { color: WHITE },
                            stroke: { color: '#333333' }
                        }
                    },
                    content: { color: '#333333' }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#333333' },
                            hover: {
                                fill: { color: '#333333' },
                                stroke: { color: '#333333' }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: '#333333' },
                            fill: { color: '#333333' }
                        }
                    }
                },
                selectable: { stroke: { color: '#333333' } },
                connectionDefaults: {
                    stroke: { color: '#c4c4c4' },
                    content: { color: '#333333' },
                    selection: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#333333' }
                        },
                        stroke: { color: '#333333' }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#428bca',
                        '#d1e0ec'
                    ],
                    [
                        '#5bc0de',
                        '#d6eaf0'
                    ],
                    [
                        '#5cb85c',
                        '#d6e9d6'
                    ],
                    [
                        '#5cb85c',
                        '#f4e8d7'
                    ],
                    [
                        '#e67d4a',
                        '#f2ddd3'
                    ],
                    [
                        '#da3b36',
                        '#f0d0cf'
                    ]
                ]
            }
        });
        registerTheme('flat', {
            chart: {
                title: { color: '#4c5356' },
                legend: {
                    labels: { color: '#4c5356' },
                    inactiveItems: {
                        labels: { color: '#CBCBCB' },
                        markers: { color: '#CBCBCB' }
                    }
                },
                seriesDefaults: {
                    labels: { color: '#4c5356' },
                    errorBars: { color: '#4c5356' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#cdcdcd' }
                        },
                        label: { color: '#4c5356' },
                        line: { color: '#cdcdcd' }
                    },
                    candlestick: {
                        downColor: '#c7c7c7',
                        line: { color: '#787878' }
                    },
                    area: { opacity: 0.9 },
                    waterfall: { line: { color: '#cdcdcd' } },
                    horizontalWaterfall: { line: { color: '#cdcdcd' } },
                    overlay: { gradient: 'none' },
                    border: { _brightness: 1 }
                },
                seriesColors: [
                    '#10c4b2',
                    '#ff7663',
                    '#ffb74f',
                    '#a2df53',
                    '#1c9ec4',
                    '#ff63a5',
                    '#1cc47b'
                ],
                axisDefaults: {
                    line: { color: '#cdcdcd' },
                    labels: { color: '#4c5356' },
                    minorGridLines: { color: '#cdcdcd' },
                    majorGridLines: { color: '#cdcdcd' },
                    title: { color: '#4c5356' },
                    crosshair: { color: '#cdcdcd' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#cdcdcd' }
                        },
                        label: { color: '#4c5356' },
                        line: { color: '#cdcdcd' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#10c4b2' },
                scale: {
                    rangePlaceholderColor: '#cdcdcd',
                    labels: { color: '#4c5356' },
                    minorTicks: { color: '#4c5356' },
                    majorTicks: { color: '#4c5356' },
                    line: { color: '#4c5356' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#10c4b2' },
                    connectorDefaults: {
                        fill: { color: '#363940' },
                        stroke: { color: WHITE },
                        hover: {
                            fill: { color: WHITE },
                            stroke: { color: '#363940' }
                        }
                    },
                    content: { color: '#4c5356' }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#363940' },
                            hover: {
                                fill: { color: '#363940' },
                                stroke: { color: '#363940' }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: '#363940' },
                            fill: { color: '#363940' }
                        }
                    }
                },
                selectable: { stroke: { color: '#363940' } },
                connectionDefaults: {
                    stroke: { color: '#cdcdcd' },
                    content: { color: '#4c5356' },
                    selection: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#363940' }
                        },
                        stroke: { color: '#363940' }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#10c4b2',
                        '#cff3f0'
                    ],
                    [
                        '#ff7663',
                        '#ffe4e0'
                    ],
                    [
                        '#ffb74f',
                        '#fff1dc'
                    ],
                    [
                        '#a2df53',
                        '#ecf9dd'
                    ],
                    [
                        '#1c9ec4',
                        '#d2ecf3'
                    ],
                    [
                        '#ff63a5',
                        '#ffe0ed'
                    ],
                    [
                        '#1cc47b',
                        '#d2f3e5'
                    ]
                ]
            }
        });
        registerTheme('material', {
            chart: {
                title: { color: '#444444' },
                legend: {
                    labels: { color: '#444444' },
                    inactiveItems: {
                        labels: { color: '#CBCBCB' },
                        markers: { color: '#CBCBCB' }
                    }
                },
                seriesDefaults: {
                    labels: { color: '#444444' },
                    errorBars: { color: '#444444' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#e5e5e5' }
                        },
                        label: { color: '#444444' },
                        line: { color: '#e5e5e5' }
                    },
                    candlestick: {
                        downColor: '#c7c7c7',
                        line: { color: '#787878' }
                    },
                    area: { opacity: 0.9 },
                    waterfall: { line: { color: '#e5e5e5' } },
                    horizontalWaterfall: { line: { color: '#e5e5e5' } },
                    overlay: { gradient: 'none' },
                    border: { _brightness: 1 }
                },
                seriesColors: [
                    '#3f51b5',
                    '#03a9f4',
                    '#4caf50',
                    '#f9ce1d',
                    '#ff9800',
                    '#ff5722'
                ],
                axisDefaults: {
                    line: { color: '#e5e5e5' },
                    labels: { color: '#444444' },
                    minorGridLines: { color: '#e5e5e5' },
                    majorGridLines: { color: '#e5e5e5' },
                    title: { color: '#444444' },
                    crosshair: { color: '#7f7f7f' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#e5e5e5' }
                        },
                        label: { color: '#444444' },
                        line: { color: '#e5e5e5' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#3f51b5' },
                scale: {
                    rangePlaceholderColor: '#e5e5e5',
                    labels: { color: '#444444' },
                    minorTicks: { color: '#444444' },
                    majorTicks: { color: '#444444' },
                    line: { color: '#444444' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#3f51b5' },
                    connectorDefaults: {
                        fill: { color: '#7f7f7f' },
                        stroke: { color: WHITE },
                        hover: {
                            fill: { color: WHITE },
                            stroke: { color: '#7f7f7f' }
                        }
                    },
                    content: { color: '#444444' }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#444444' },
                            hover: {
                                fill: { color: '#444444' },
                                stroke: { color: '#444444' }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: '#444444' },
                            fill: { color: '#444444' }
                        }
                    }
                },
                selectable: { stroke: { color: '#444444' } },
                connectionDefaults: {
                    stroke: { color: '#7f7f7f' },
                    content: { color: '#444444' },
                    selection: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#444444' }
                        },
                        stroke: { color: '#444444' }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#3f51b5',
                        '#cff3f0'
                    ],
                    [
                        '#03a9f4',
                        '#e5f6fe'
                    ],
                    [
                        '#4caf50',
                        '#edf7ed'
                    ],
                    [
                        '#f9ce1d',
                        '#fefae8'
                    ],
                    [
                        '#ff9800',
                        '#fff4e5'
                    ],
                    [
                        '#ff5722',
                        '#ffeee8'
                    ]
                ]
            }
        });
        registerTheme('materialblack', {
            chart: {
                title: { color: '#fff' },
                legend: {
                    labels: { color: '#fff' },
                    inactiveItems: {
                        labels: { color: '#CBCBCB' },
                        markers: { color: '#CBCBCB' }
                    }
                },
                seriesDefaults: {
                    labels: { color: '#fff' },
                    errorBars: { color: '#fff' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#e5e5e5' }
                        },
                        label: { color: '#fff' },
                        line: { color: '#e5e5e5' }
                    },
                    candlestick: {
                        downColor: '#c7c7c7',
                        line: { color: '#787878' }
                    },
                    area: { opacity: 0.9 },
                    waterfall: { line: { color: '#4d4d4d' } },
                    horizontalWaterfall: { line: { color: '#4d4d4d' } },
                    overlay: { gradient: 'none' },
                    border: { _brightness: 1 }
                },
                chartArea: { background: '#1c1c1c' },
                seriesColors: [
                    '#3f51b5',
                    '#03a9f4',
                    '#4caf50',
                    '#f9ce1d',
                    '#ff9800',
                    '#ff5722'
                ],
                axisDefaults: {
                    line: { color: '#4d4d4d' },
                    labels: { color: '#fff' },
                    minorGridLines: { color: '#4d4d4d' },
                    majorGridLines: { color: '#4d4d4d' },
                    title: { color: '#fff' },
                    crosshair: { color: '#7f7f7f' },
                    notes: {
                        icon: {
                            background: 'transparent',
                            border: { color: '#4d4d4d' }
                        },
                        label: { color: '#fff' },
                        line: { color: '#4d4d4d' }
                    }
                }
            },
            gauge: {
                pointer: { color: '#3f51b5' },
                scale: {
                    rangePlaceholderColor: '#4d4d4d',
                    labels: { color: '#fff' },
                    minorTicks: { color: '#fff' },
                    majorTicks: { color: '#fff' },
                    line: { color: '#fff' }
                }
            },
            diagram: {
                shapeDefaults: {
                    fill: { color: '#3f51b5' },
                    connectorDefaults: {
                        fill: { color: '#7f7f7f' },
                        stroke: { color: WHITE },
                        hover: {
                            fill: { color: WHITE },
                            stroke: { color: '#7f7f7f' }
                        }
                    },
                    content: { color: '#fff' }
                },
                editable: {
                    resize: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#fff' },
                            hover: {
                                fill: { color: '#fff' },
                                stroke: { color: '#fff' }
                            }
                        }
                    },
                    rotate: {
                        thumb: {
                            stroke: { color: '#fff' },
                            fill: { color: '#fff' }
                        }
                    }
                },
                selectable: { stroke: { color: '#fff' } },
                connectionDefaults: {
                    stroke: { color: '#7f7f7f' },
                    content: { color: '#fff' },
                    selection: {
                        handles: {
                            fill: { color: WHITE },
                            stroke: { color: '#fff' }
                        },
                        stroke: { color: '#fff' }
                    }
                }
            },
            treeMap: {
                colors: [
                    [
                        '#3f51b5',
                        '#cff3f0'
                    ],
                    [
                        '#03a9f4',
                        '#e5f6fe'
                    ],
                    [
                        '#4caf50',
                        '#edf7ed'
                    ],
                    [
                        '#f9ce1d',
                        '#fefae8'
                    ],
                    [
                        '#ff9800',
                        '#fff4e5'
                    ],
                    [
                        '#ff5722',
                        '#ffeee8'
                    ]
                ]
            }
        });
        (function () {
            var TEXT = '#333333';
            var INACTIVE = '#7f7f7f';
            var INACTIVE_SHAPE = '#bdbdbd';
            var AXIS = '#c8c8c8';
            var AXIS_MINOR = '#dddddd';
            var SERIES = [
                '#008fd3',
                '#99d101',
                '#f39b02',
                '#f05662',
                '#c03c53',
                '#acacac'
            ];
            var SERIES_LIGHT = [
                '#cbe8f5',
                '#eaf5cb',
                '#fceacc',
                '#fbdcdf',
                '#f2d7dc',
                '#eeeeee'
            ];
            var PRIMARY = SERIES[0];
            var DIAGRAM_HOVER = WHITE;
            function noteStyle() {
                return {
                    icon: {
                        background: '#007cc0',
                        border: { color: '#007cc0' }
                    },
                    label: { color: '#ffffff' },
                    line: { color: AXIS }
                };
            }
            registerTheme('fiori', {
                chart: {
                    title: { color: TEXT },
                    legend: {
                        labels: { color: TEXT },
                        inactiveItems: {
                            labels: { color: INACTIVE },
                            markers: { color: INACTIVE }
                        }
                    },
                    seriesDefaults: {
                        labels: { color: TEXT },
                        errorBars: { color: TEXT },
                        notes: noteStyle(),
                        candlestick: {
                            downColor: AXIS,
                            line: { color: INACTIVE_SHAPE }
                        },
                        area: { opacity: 0.8 },
                        waterfall: { line: { color: AXIS } },
                        horizontalWaterfall: { line: { color: AXIS } },
                        overlay: { gradient: 'none' },
                        border: { _brightness: 1 }
                    },
                    seriesColors: SERIES,
                    axisDefaults: {
                        line: { color: AXIS },
                        labels: { color: TEXT },
                        minorGridLines: { color: AXIS_MINOR },
                        majorGridLines: { color: AXIS },
                        title: { color: TEXT },
                        crosshair: { color: INACTIVE },
                        notes: noteStyle()
                    }
                },
                gauge: {
                    pointer: { color: PRIMARY },
                    scale: {
                        rangePlaceholderColor: AXIS,
                        labels: { color: TEXT },
                        minorTicks: { color: TEXT },
                        majorTicks: { color: TEXT },
                        line: { color: TEXT }
                    }
                },
                diagram: {
                    shapeDefaults: {
                        fill: { color: PRIMARY },
                        connectorDefaults: {
                            fill: { color: TEXT },
                            stroke: { color: DIAGRAM_HOVER },
                            hover: {
                                fill: { color: DIAGRAM_HOVER },
                                stroke: { color: TEXT }
                            }
                        },
                        content: { color: TEXT }
                    },
                    editable: {
                        resize: {
                            handles: {
                                fill: { color: DIAGRAM_HOVER },
                                stroke: { color: INACTIVE_SHAPE },
                                hover: {
                                    fill: { color: INACTIVE_SHAPE },
                                    stroke: { color: INACTIVE_SHAPE }
                                }
                            }
                        },
                        rotate: {
                            thumb: {
                                stroke: { color: INACTIVE_SHAPE },
                                fill: { color: INACTIVE_SHAPE }
                            }
                        }
                    },
                    selectable: { stroke: { color: INACTIVE_SHAPE } },
                    connectionDefaults: {
                        stroke: { color: INACTIVE_SHAPE },
                        content: { color: INACTIVE_SHAPE },
                        selection: {
                            handles: {
                                fill: { color: DIAGRAM_HOVER },
                                stroke: { color: INACTIVE_SHAPE }
                            },
                            stroke: { color: INACTIVE_SHAPE }
                        }
                    }
                },
                treeMap: { colors: fuse(SERIES, SERIES_LIGHT) }
            });
        }());
        (function () {
            var TEXT = '#4e4e4e';
            var INACTIVE = '#7f7f7f';
            var INACTIVE_SHAPE = '#bdbdbd';
            var AXIS = '#c8c8c8';
            var AXIS_MINOR = '#e5e5e5';
            var SERIES = [
                '#0072c6',
                '#5db2ff',
                '#008a17',
                '#82ba00',
                '#ff8f32',
                '#ac193d'
            ];
            var SERIES_LIGHT = [
                '#cbe2f3',
                '#deeffe',
                '#cbe7d0',
                '#e5f0cb',
                '#fee8d5',
                '#eed0d7'
            ];
            var PRIMARY = SERIES[0];
            var DIAGRAM_HOVER = WHITE;
            function noteStyle() {
                return {
                    icon: {
                        background: '#00b0ff',
                        border: { color: '#00b0ff' }
                    },
                    label: { color: '#ffffff' },
                    line: { color: AXIS }
                };
            }
            registerTheme('office365', {
                chart: {
                    title: { color: TEXT },
                    legend: {
                        labels: { color: TEXT },
                        inactiveItems: {
                            labels: { color: INACTIVE },
                            markers: { color: INACTIVE }
                        }
                    },
                    seriesDefaults: {
                        labels: { color: TEXT },
                        errorBars: { color: TEXT },
                        notes: noteStyle(),
                        candlestick: {
                            downColor: AXIS,
                            line: { color: INACTIVE_SHAPE }
                        },
                        area: { opacity: 0.8 },
                        waterfall: { line: { color: AXIS } },
                        horizontalWaterfall: { line: { color: AXIS } },
                        overlay: { gradient: 'none' },
                        border: { _brightness: 1 }
                    },
                    seriesColors: SERIES,
                    axisDefaults: {
                        line: { color: AXIS },
                        labels: { color: TEXT },
                        minorGridLines: { color: AXIS_MINOR },
                        majorGridLines: { color: AXIS },
                        title: { color: TEXT },
                        crosshair: { color: INACTIVE },
                        notes: noteStyle()
                    }
                },
                gauge: {
                    pointer: { color: PRIMARY },
                    scale: {
                        rangePlaceholderColor: AXIS,
                        labels: { color: TEXT },
                        minorTicks: { color: TEXT },
                        majorTicks: { color: TEXT },
                        line: { color: TEXT }
                    }
                },
                diagram: {
                    shapeDefaults: {
                        fill: { color: PRIMARY },
                        connectorDefaults: {
                            fill: { color: TEXT },
                            stroke: { color: DIAGRAM_HOVER },
                            hover: {
                                fill: { color: DIAGRAM_HOVER },
                                stroke: { color: TEXT }
                            }
                        },
                        content: { color: TEXT }
                    },
                    editable: {
                        resize: {
                            handles: {
                                fill: { color: DIAGRAM_HOVER },
                                stroke: { color: INACTIVE_SHAPE },
                                hover: {
                                    fill: { color: INACTIVE_SHAPE },
                                    stroke: { color: INACTIVE_SHAPE }
                                }
                            }
                        },
                        rotate: {
                            thumb: {
                                stroke: { color: INACTIVE_SHAPE },
                                fill: { color: INACTIVE_SHAPE }
                            }
                        }
                    },
                    selectable: { stroke: { color: INACTIVE_SHAPE } },
                    connectionDefaults: {
                        stroke: { color: INACTIVE_SHAPE },
                        content: { color: INACTIVE_SHAPE },
                        selection: {
                            handles: {
                                fill: { color: DIAGRAM_HOVER },
                                stroke: { color: INACTIVE_SHAPE }
                            },
                            stroke: { color: INACTIVE_SHAPE }
                        }
                    }
                },
                treeMap: { colors: fuse(SERIES, SERIES_LIGHT) }
            });
        }());
        (function () {
            var TEXT = '#32364c';
            var INACTIVE = '#7f7f7f';
            var INACTIVE_SHAPE = '#bdbdbd';
            var AXIS = '#dfe0e1';
            var AXIS_MINOR = '#dfe0e1';
            var SERIES = [
                '#ff4350',
                '#ff9ea5',
                '#00acc1',
                '#80deea',
                '#ffbf46',
                '#ffd78c'
            ];
            var SERIES_LIGHT = [
                '#ffd9dc',
                '#ffeced',
                '#cceef3',
                '#e6f8fb',
                '#fff2da',
                '#fff7e8'
            ];
            var PRIMARY = SERIES[0];
            var DIAGRAM_HOVER = WHITE;
            function noteStyle() {
                return {
                    icon: {
                        background: '#007cc0',
                        border: { color: '#007cc0' }
                    },
                    label: { color: '#ffffff' },
                    line: { color: AXIS }
                };
            }
            registerTheme('nova', {
                chart: {
                    title: { color: TEXT },
                    legend: {
                        labels: { color: TEXT },
                        inactiveItems: {
                            labels: { color: INACTIVE },
                            markers: { color: INACTIVE }
                        }
                    },
                    seriesDefaults: {
                        labels: { color: TEXT },
                        errorBars: { color: TEXT },
                        notes: noteStyle(),
                        candlestick: {
                            downColor: AXIS,
                            line: { color: INACTIVE_SHAPE }
                        },
                        area: { opacity: 0.8 },
                        waterfall: { line: { color: AXIS } },
                        horizontalWaterfall: { line: { color: AXIS } },
                        overlay: { gradient: 'none' },
                        border: { _brightness: 1 }
                    },
                    seriesColors: SERIES,
                    axisDefaults: {
                        line: { color: AXIS },
                        labels: { color: TEXT },
                        minorGridLines: { color: AXIS_MINOR },
                        majorGridLines: { color: AXIS },
                        title: { color: TEXT },
                        crosshair: { color: TEXT },
                        notes: noteStyle()
                    }
                },
                gauge: {
                    pointer: { color: PRIMARY },
                    scale: {
                        rangePlaceholderColor: AXIS,
                        labels: { color: TEXT },
                        minorTicks: { color: TEXT },
                        majorTicks: { color: TEXT },
                        line: { color: TEXT }
                    }
                },
                diagram: {
                    shapeDefaults: {
                        fill: { color: PRIMARY },
                        connectorDefaults: {
                            fill: { color: TEXT },
                            stroke: { color: DIAGRAM_HOVER },
                            hover: {
                                fill: { color: DIAGRAM_HOVER },
                                stroke: { color: TEXT }
                            }
                        },
                        content: { color: TEXT }
                    },
                    editable: {
                        resize: {
                            handles: {
                                fill: { color: DIAGRAM_HOVER },
                                stroke: { color: INACTIVE_SHAPE },
                                hover: {
                                    fill: { color: INACTIVE_SHAPE },
                                    stroke: { color: INACTIVE_SHAPE }
                                }
                            }
                        },
                        rotate: {
                            thumb: {
                                stroke: { color: INACTIVE_SHAPE },
                                fill: { color: INACTIVE_SHAPE }
                            }
                        }
                    },
                    selectable: { stroke: { color: INACTIVE_SHAPE } },
                    connectionDefaults: {
                        stroke: { color: INACTIVE_SHAPE },
                        content: { color: INACTIVE_SHAPE },
                        selection: {
                            handles: {
                                fill: { color: DIAGRAM_HOVER },
                                stroke: { color: INACTIVE_SHAPE }
                            },
                            stroke: { color: INACTIVE_SHAPE }
                        }
                    }
                },
                treeMap: { colors: fuse(SERIES, SERIES_LIGHT) }
            });
        }());
        (function () {
            var TEXT = '#656565';
            var AXIS = 'rgba(0, 0, 0, .04)';
            var SERIES = [
                '#ff6358',
                '#ffd246',
                '#78d237',
                '#28b4c8',
                '#2d73f5',
                '#aa46be'
            ];
            var SERIES_LIGHT = [
                '#ffd9dc',
                '#ffeced',
                '#cceef3',
                '#e6f8fb',
                '#fff2da',
                '#fff7e8'
            ];
            var PRIMARY = SERIES[0];
            registerTheme('default-v2', {
                chart: {},
                gauge: {
                    pointer: { color: PRIMARY },
                    scale: {
                        rangePlaceholderColor: AXIS,
                        labels: { color: TEXT },
                        minorTicks: { color: TEXT },
                        majorTicks: { color: TEXT },
                        line: { color: TEXT }
                    }
                },
                diagram: {},
                treeMap: { colors: fuse(SERIES, SERIES_LIGHT) }
            });
            themes.sass = themes['default-v2'];
        }());
        (function () {
            var TEXT = '#292b2c';
            var AXIS = 'rgba(0, 0, 0, .04)';
            var SERIES = [
                '#0275d8',
                '#5bc0de',
                '#5cb85c',
                '#f0ad4e',
                '#e67d4a',
                '#d9534f'
            ];
            var SERIES_LIGHT = [
                '#ffd9dc',
                '#ffeced',
                '#cceef3',
                '#e6f8fb',
                '#fff2da',
                '#fff7e8'
            ];
            var PRIMARY = SERIES[0];
            registerTheme('bootstrap-v4', {
                chart: {},
                gauge: {
                    pointer: { color: PRIMARY },
                    scale: {
                        rangePlaceholderColor: AXIS,
                        labels: { color: TEXT },
                        minorTicks: { color: TEXT },
                        majorTicks: { color: TEXT },
                        line: { color: TEXT }
                    }
                },
                diagram: {},
                treeMap: { colors: fuse(SERIES, SERIES_LIGHT) }
            });
        }());
        function fuse(arr1, arr2) {
            return $.map(arr1, function (item, index) {
                return [[
                        item,
                        arr2[index]
                    ]];
            });
        }
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.dataviz.themes', [
        'kendo.dataviz.core',
        'dataviz/themes/chart-base-theme',
        'dataviz/themes/auto-theme',
        'dataviz/themes/themes'
    ], f);
}(function () {
    var __meta__ = {
        id: 'dataviz.themes',
        name: 'Themes',
        description: 'Built-in themes for the DataViz widgets',
        category: 'dataviz',
        depends: ['dataviz.core'],
        hidden: true
    };
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));