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
    define('kendo.gantt', [
        'kendo.data',
        'kendo.popup',
        'kendo.window',
        'kendo.resizable',
        'kendo.gantt.list',
        'kendo.gantt.timeline',
        'kendo.grid',
        'kendo.pdf'
    ], f);
}(function () {
    var __meta__ = {
        id: 'gantt',
        name: 'Gantt',
        category: 'web',
        description: 'The Gantt component.',
        depends: [
            'data',
            'popup',
            'resizable',
            'window',
            'gantt.list',
            'gantt.timeline',
            'grid'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo;
        var keys = $.extend({ F10: 121 }, kendo.keys);
        var supportsMedia = 'matchMedia' in window;
        var browser = kendo.support.browser;
        var mobileOS = kendo.support.mobileOS;
        var Observable = kendo.Observable;
        var Widget = kendo.ui.Widget;
        var DataSource = kendo.data.DataSource;
        var ObservableObject = kendo.data.ObservableObject;
        var ObservableArray = kendo.data.ObservableArray;
        var Query = kendo.data.Query;
        var isArray = $.isArray;
        var inArray = $.inArray;
        var isFunction = kendo.isFunction;
        var proxy = $.proxy;
        var extend = $.extend;
        var isPlainObject = $.isPlainObject;
        var map = $.map;
        var outerWidth = kendo._outerWidth;
        var outerHeight = kendo._outerHeight;
        var defaultIndicatorWidth = 3;
        var NS = '.kendoGantt';
        var PERCENTAGE_FORMAT = 'p0';
        var TABINDEX = 'tabIndex';
        var CLICK = 'click';
        var WIDTH = 'width';
        var STRING = 'string';
        var DIRECTIONS = {
            'down': {
                origin: 'bottom left',
                position: 'top left'
            },
            'up': {
                origin: 'top left',
                position: 'bottom left'
            }
        };
        var ARIA_DESCENDANT = 'aria-activedescendant';
        var ARIA_LABEL = 'aria-label';
        var ACTIVE_CELL = 'gantt_active_cell';
        var ACTIVE_OPTION = 'action-option-focused';
        var DOT = '.';
        var TASK_DELETE_CONFIRM = 'Are you sure you want to delete this task?';
        var DEPENDENCY_DELETE_CONFIRM = 'Are you sure you want to delete this dependency?';
        var TOGGLE_BUTTON_TEMPLATE = kendo.template('<button class="#=styles.buttonToggle#" type="button" ' + ARIA_LABEL + '="Toggle"><span class="#=styles.iconToggle#"></span></button>');
        var BUTTON_TEMPLATE = '<button class="#=styles.button# #=className#" type="button" ' + '#if (action) {#' + 'data-action="#=action#"' + '#}#' + '><span class="#=iconClass#"></span><span>#=text#</span></button>';
        var COMMAND_BUTTON_TEMPLATE = '<a class="#=className#" #=attr# href="\\#">#=text#</a>';
        var VIEWBUTTONTEMPLATE = kendo.template('<li class="#=styles.currentView# #=styles.viewButtonDefault#"><a href="\\#" class="#=styles.link#">&nbps;</a></li>');
        var HEADER_VIEWS_TEMPLATE = kendo.template('<ul class="#=styles.viewsWrapper#">' + '#for(var view in views){#' + '<li class="#=styles.viewButtonDefault# #=styles.viewButton#-#= view.toLowerCase() #" data-#=ns#name="#=view#"><a href="\\#" class="#=styles.link#">#=views[view].title#</a></li>' + '#}#' + '</ul>');
        var TASK_DROPDOWN_TEMPLATE = kendo.template('<div class="#=styles.popupWrapper#">' + '<ul class="#=styles.popupList#" role="listbox">' + '#for(var i = 0, l = actions.length; i < l; i++){#' + '<li class="#=styles.item#" data-action="#=actions[i].data#" role="option">#=actions[i].text#</span>' + '#}#' + '</ul>' + '</div>');
        var DATERANGEEDITOR = function (container, options) {
            var attr = {
                name: options.field,
                title: options.title
            };
            var validationRules = options.model.fields[options.field].validation;
            if (validationRules && isPlainObject(validationRules) && validationRules.message) {
                attr[kendo.attr('dateCompare-msg')] = validationRules.message;
            }
            $('<input type="text" required ' + kendo.attr('type') + '="date" ' + kendo.attr('role') + '="datetimepicker" ' + kendo.attr('bind') + '="value:' + options.field + '" ' + kendo.attr('validate') + '=\'true\' />').attr(attr).appendTo(container);
            $('<span ' + kendo.attr('for') + '="' + options.field + '" class="k-invalid-msg"/>').hide().appendTo(container);
        };
        var RESOURCESEDITOR = function (container, options) {
            $('<a href="#" class="' + options.styles.button + '">' + options.messages.assignButton + '</a>').click(options.click).appendTo(container);
        };
        var ganttStyles = {
            wrapper: 'k-widget k-gantt',
            rowHeight: 'k-gantt-rowheight',
            listWrapper: 'k-gantt-layout k-gantt-treelist',
            list: 'k-gantt-treelist',
            timelineWrapper: 'k-gantt-layout k-gantt-timeline',
            timeline: 'k-gantt-timeline',
            splitBarWrapper: 'k-splitbar k-state-default k-splitbar-horizontal k-splitbar-draggable-horizontal k-gantt-layout',
            splitBar: 'k-splitbar',
            splitBarHover: 'k-splitbar-horizontal-hover',
            popupWrapper: 'k-list-container',
            popupList: 'k-list k-reset',
            resizeHandle: 'k-resize-handle',
            icon: 'k-icon',
            item: 'k-item',
            line: 'k-line',
            buttonDelete: 'k-gantt-delete',
            buttonCancel: 'k-gantt-cancel',
            buttonSave: 'k-gantt-update',
            buttonToggle: 'k-gantt-toggle',
            primary: 'k-primary',
            hovered: 'k-state-hover',
            selected: 'k-state-selected',
            focused: 'k-state-focused',
            gridHeader: 'k-grid-header',
            gridHeaderWrap: 'k-grid-header-wrap',
            gridContent: 'k-grid-content',
            tasks: 'k-gantt-tasks',
            popup: {
                form: 'k-popup-edit-form',
                editForm: 'k-gantt-edit-form',
                formContainer: 'k-edit-form-container',
                resourcesFormContainer: 'k-resources-form-container',
                message: 'k-popup-message',
                buttonsContainer: 'k-edit-buttons k-state-default',
                button: 'k-button',
                editField: 'k-edit-field',
                editLabel: 'k-edit-label',
                resourcesField: 'k-gantt-resources'
            },
            toolbar: {
                headerWrapper: 'k-floatwrap k-header k-gantt-toolbar',
                footerWrapper: 'k-floatwrap k-header k-gantt-toolbar',
                toolbar: 'k-gantt-toolbar',
                expanded: 'k-state-expanded',
                views: 'k-gantt-views',
                viewsWrapper: 'k-reset k-header k-gantt-views',
                actions: 'k-gantt-actions',
                button: 'k-button k-button-icontext',
                buttonToggle: 'k-button k-button-icon k-gantt-toggle',
                iconPlus: 'k-icon k-i-plus',
                iconPdf: 'k-icon k-i-file-pdf',
                iconToggle: 'k-icon k-i-layout-1-by-4',
                viewButtonDefault: 'k-state-default',
                viewButton: 'k-view',
                currentView: 'k-current-view',
                link: 'k-link',
                pdfButton: 'k-gantt-pdf',
                appendButton: 'k-gantt-create'
            }
        };
        function selector(uid) {
            return '[' + kendo.attr('uid') + (uid ? '=\'' + uid + '\']' : ']');
        }
        function trimOptions(options) {
            delete options.name;
            delete options.prefix;
            delete options.remove;
            delete options.edit;
            delete options.add;
            delete options.navigate;
            return options;
        }
        function dateCompareValidator(input) {
            if (input.filter('[name=end], [name=start]').length) {
                var field = input.attr('name');
                var picker = kendo.widgetInstance(input, kendo.ui);
                var dates = {};
                var container = input;
                var editable;
                var model;
                while (container !== window && !editable) {
                    container = container.parent();
                    editable = container.data('kendoEditable');
                }
                model = editable ? editable.options.model : null;
                if (!model) {
                    return true;
                }
                dates.start = model.start;
                dates.end = model.end;
                dates[field] = picker ? picker.value() : kendo.parseDate(input.value());
                return dates.start <= dates.end;
            }
            return true;
        }
        function focusTable(table, direct) {
            var wrapper = table.parents('[' + kendo.attr('role') + '="gantt"]');
            var scrollPositions = [];
            var parents = scrollableParents(wrapper);
            table.attr(TABINDEX, 0);
            if (direct) {
                parents.each(function (index, parent) {
                    scrollPositions[index] = $(parent).scrollTop();
                });
            }
            try {
                table[0].setActive();
            } catch (e) {
                table[0].focus();
            }
            if (direct) {
                parents.each(function (index, parent) {
                    $(parent).scrollTop(scrollPositions[index]);
                });
            }
        }
        function scrollableParents(element) {
            return $(element).parentsUntil('body').filter(function (index, element) {
                var computedStyle = kendo.getComputedStyles(element, ['overflow']);
                return computedStyle.overflow != 'visible';
            }).add(window);
        }
        var defaultCommands;
        var TaskDropDown = Observable.extend({
            init: function (element, options) {
                Observable.fn.init.call(this);
                this.element = element;
                this.options = extend(true, {}, this.options, options);
                this._popup();
            },
            options: {
                direction: 'down',
                navigatable: false
            },
            _current: function (method) {
                var ganttStyles = Gantt.styles;
                var current = this.list.find(DOT + ganttStyles.focused);
                var sibling = current[method]();
                if (sibling.length) {
                    current.removeClass(ganttStyles.focused).removeAttr('id');
                    sibling.addClass(ganttStyles.focused).attr('id', ACTIVE_OPTION);
                    this.list.find('ul').removeAttr(ARIA_DESCENDANT).attr(ARIA_DESCENDANT, ACTIVE_OPTION);
                }
            },
            _popup: function () {
                var that = this;
                var ganttStyles = Gantt.styles;
                var itemSelector = 'li' + DOT + ganttStyles.item;
                var appendButtonSelector = DOT + ganttStyles.toolbar.appendButton;
                var actions = this.options.messages.actions;
                var navigatable = this.options.navigatable;
                this.list = $(TASK_DROPDOWN_TEMPLATE({
                    styles: ganttStyles,
                    actions: [
                        {
                            data: 'add',
                            text: actions.addChild
                        },
                        {
                            data: 'insert-before',
                            text: actions.insertBefore
                        },
                        {
                            data: 'insert-after',
                            text: actions.insertAfter
                        }
                    ]
                }));
                this.element.append(this.list);
                this.popup = new kendo.ui.Popup(this.list, extend({
                    anchor: this.element.find(appendButtonSelector),
                    open: function () {
                        that._adjustListWidth();
                    },
                    animation: this.options.animation
                }, DIRECTIONS[this.options.direction]));
                this.element.on(CLICK + NS, appendButtonSelector, function (e) {
                    var target = $(this);
                    var action = target.attr(kendo.attr('action'));
                    e.preventDefault();
                    if (action) {
                        that.trigger('command', { type: action });
                    } else {
                        that.popup.open();
                        if (navigatable) {
                            that.list.find('li:first').addClass(ganttStyles.focused).attr('id', ACTIVE_OPTION).end().find('ul').attr({
                                TABINDEX: 0,
                                'aria-activedescendant': ACTIVE_OPTION
                            }).focus();
                        }
                    }
                });
                this.list.find(itemSelector).hover(function () {
                    $(this).addClass(ganttStyles.hovered);
                }, function () {
                    $(this).removeClass(ganttStyles.hovered);
                }).end().on(CLICK + NS, itemSelector, function () {
                    that.trigger('command', { type: $(this).attr(kendo.attr('action')) });
                    that.popup.close();
                });
                if (navigatable) {
                    this.popup.bind('close', function () {
                        that.list.find(itemSelector).removeClass(ganttStyles.focused).end().find('ul').attr(TABINDEX, 0);
                        that.element.parents('[' + kendo.attr('role') + '="gantt"]').find(DOT + ganttStyles.gridContent + ' > table:first').focus();
                    });
                    this.list.find('ul').on('keydown' + NS, function (e) {
                        var key = e.keyCode;
                        switch (key) {
                        case keys.UP:
                            e.preventDefault();
                            that._current('prev');
                            break;
                        case keys.DOWN:
                            e.preventDefault();
                            that._current('next');
                            break;
                        case keys.ENTER:
                            that.list.find(DOT + ganttStyles.focused).click();
                            break;
                        case keys.ESC:
                            e.preventDefault();
                            that.popup.close();
                            break;
                        }
                    });
                }
            },
            _adjustListWidth: function () {
                var list = this.list;
                var ganttStyles = Gantt.styles;
                var width = list[0].style.width;
                var wrapper = this.element.find(DOT + ganttStyles.toolbar.appendButton);
                var listOuterWidth = outerWidth(list);
                var computedStyle;
                var computedWidth;
                if (!list.data(WIDTH) && width) {
                    return;
                }
                computedStyle = window.getComputedStyle ? window.getComputedStyle(wrapper[0], null) : 0;
                computedWidth = computedStyle ? parseFloat(computedStyle.width) : outerWidth(wrapper);
                if (computedStyle && (browser.mozilla || browser.msie)) {
                    computedWidth += parseFloat(computedStyle.paddingLeft) + parseFloat(computedStyle.paddingRight) + parseFloat(computedStyle.borderLeftWidth) + parseFloat(computedStyle.borderRightWidth);
                }
                if (list.css('box-sizing') !== 'border-box') {
                    width = computedWidth - (outerWidth(list) - list.width());
                } else {
                    width = computedWidth;
                }
                if (listOuterWidth > width) {
                    width = listOuterWidth;
                }
                list.css({
                    fontFamily: wrapper.css('font-family'),
                    width: width
                }).data(WIDTH, width);
            },
            destroy: function () {
                clearTimeout(this._focusTimeout);
                this.popup.destroy();
                this.element.off(NS);
                this.list.off(NS);
                this.unbind();
            }
        });
        var createDataSource = function (type, name) {
            return function (options) {
                options = isArray(options) ? { data: options } : options;
                var dataSource = options || {};
                var data = dataSource.data;
                dataSource.data = data;
                if (!(dataSource instanceof type) && dataSource instanceof DataSource) {
                    throw new Error('Incorrect DataSource type. Only ' + name + ' instances are supported');
                }
                return dataSource instanceof type ? dataSource : new type(dataSource);
            };
        };
        var GanttDependency = kendo.data.Model.define({
            id: 'id',
            fields: {
                id: { type: 'number' },
                predecessorId: { type: 'number' },
                successorId: { type: 'number' },
                type: { type: 'number' }
            }
        });
        var GanttDependencyDataSource = DataSource.extend({
            init: function (options) {
                DataSource.fn.init.call(this, extend(true, {}, {
                    schema: {
                        modelBase: GanttDependency,
                        model: GanttDependency
                    }
                }, options));
            },
            successors: function (id) {
                return this._dependencies('predecessorId', id);
            },
            predecessors: function (id) {
                return this._dependencies('successorId', id);
            },
            dependencies: function (id) {
                var predecessors = this.predecessors(id);
                var successors = this.successors(id);
                predecessors.push.apply(predecessors, successors);
                return predecessors;
            },
            _dependencies: function (field, id) {
                var data = this.view();
                var filter = {
                    field: field,
                    operator: 'eq',
                    value: id
                };
                data = new Query(data).filter(filter).toArray();
                return data;
            }
        });
        GanttDependencyDataSource.create = createDataSource(GanttDependencyDataSource, 'GanttDependencyDataSource');
        var GanttTask = kendo.data.Model.define({
            duration: function () {
                var end = this.end;
                var start = this.start;
                return end - start;
            },
            isMilestone: function () {
                return this.duration() === 0;
            },
            _offset: function (value) {
                var field = [
                    'start',
                    'end'
                ];
                var newValue;
                for (var i = 0; i < field.length; i++) {
                    newValue = new Date(this.get(field[i]).getTime() + value);
                    this.set(field[i], newValue);
                }
            },
            id: 'id',
            fields: {
                id: { type: 'number' },
                parentId: {
                    type: 'number',
                    defaultValue: null,
                    validation: { required: true }
                },
                orderId: {
                    type: 'number',
                    validation: { required: true }
                },
                title: {
                    type: 'string',
                    defaultValue: 'New task'
                },
                start: {
                    type: 'date',
                    validation: { required: true }
                },
                end: {
                    type: 'date',
                    validation: {
                        required: true,
                        dateCompare: dateCompareValidator,
                        message: 'End date should be after or equal to the start date'
                    }
                },
                percentComplete: {
                    type: 'number',
                    validation: {
                        required: true,
                        min: 0,
                        max: 1,
                        step: 0.01
                    }
                },
                summary: { type: 'boolean' },
                expanded: {
                    type: 'boolean',
                    defaultValue: true
                }
            }
        });
        var GanttDataSource = DataSource.extend({
            init: function (options) {
                DataSource.fn.init.call(this, extend(true, {}, {
                    schema: {
                        modelBase: GanttTask,
                        model: GanttTask
                    }
                }, options));
            },
            remove: function (task) {
                var parentId = task.get('parentId');
                var children = this.taskAllChildren(task);
                this._removeItems(children);
                task = DataSource.fn.remove.call(this, task);
                this._childRemoved(parentId, task.get('orderId'));
                return task;
            },
            add: function (task) {
                if (!task) {
                    return;
                }
                task = this._toGanttTask(task);
                return this.insert(this.taskSiblings(task).length, task);
            },
            insert: function (index, task) {
                if (!task) {
                    return;
                }
                task = this._toGanttTask(task);
                task.set('orderId', index);
                task = DataSource.fn.insert.call(this, index, task);
                this._reorderSiblings(task, this.taskSiblings(task).length - 1);
                this._resolveSummaryFields(this.taskParent(task));
                return task;
            },
            taskChildren: function (task) {
                var data = this.view();
                var filter = {
                    field: 'parentId',
                    operator: 'eq',
                    value: null
                };
                var order = this._sort && this._sort.length ? this._sort : {
                    field: 'orderId',
                    dir: 'asc'
                };
                var taskId;
                if (!!task) {
                    taskId = task.get('id');
                    if (taskId === undefined || taskId === null || taskId === '') {
                        return [];
                    }
                    filter.value = taskId;
                }
                data = new Query(data).filter(filter).sort(order).toArray();
                return data;
            },
            taskAllChildren: function (task) {
                var data = [];
                var that = this;
                var callback = function (task) {
                    var tasks = that.taskChildren(task);
                    data.push.apply(data, tasks);
                    map(tasks, callback);
                };
                if (!!task) {
                    callback(task);
                } else {
                    data = this.view();
                }
                return data;
            },
            taskSiblings: function (task) {
                if (!task) {
                    return null;
                }
                var parent = this.taskParent(task);
                return this.taskChildren(parent);
            },
            taskParent: function (task) {
                if (!task || task.get('parentId') === null) {
                    return null;
                }
                return this.get(task.parentId);
            },
            taskLevel: function (task) {
                var level = 0;
                var parent = this.taskParent(task);
                while (parent !== null) {
                    level += 1;
                    parent = this.taskParent(parent);
                }
                return level;
            },
            taskTree: function (task) {
                var data = [];
                var current;
                var tasks = this.taskChildren(task);
                for (var i = 0, l = tasks.length; i < l; i++) {
                    current = tasks[i];
                    data.push(current);
                    if (current.get('expanded')) {
                        var children = this.taskTree(current);
                        data.push.apply(data, children);
                    }
                }
                return data;
            },
            update: function (task, taskInfo) {
                var that = this;
                var oldValue;
                var offsetChildren = function (parentTask, offset) {
                    var children = that.taskAllChildren(parentTask);
                    for (var i = 0, l = children.length; i < l; i++) {
                        children[i]._offset(offset);
                    }
                };
                var modelChangeHandler = function (e) {
                    var field = e.field;
                    var model = e.sender;
                    switch (field) {
                    case 'start':
                        that._resolveSummaryStart(that.taskParent(model));
                        offsetChildren(model, model.get(field).getTime() - oldValue.getTime());
                        break;
                    case 'end':
                        that._resolveSummaryEnd(that.taskParent(model));
                        break;
                    case 'percentComplete':
                        that._resolveSummaryPercentComplete(that.taskParent(model));
                        break;
                    case 'orderId':
                        that._reorderSiblings(model, oldValue);
                        break;
                    }
                };
                if (taskInfo.parentId !== undefined) {
                    oldValue = task.get('parentId');
                    if (oldValue !== taskInfo.parentId) {
                        task.set('parentId', taskInfo.parentId);
                        that._childRemoved(oldValue, task.get('orderId'));
                        task.set('orderId', that.taskSiblings(task).length - 1);
                        that._resolveSummaryFields(that.taskParent(task));
                    }
                    delete taskInfo.parentId;
                }
                task.bind('change', modelChangeHandler);
                for (var field in taskInfo) {
                    oldValue = task.get(field);
                    task.set(field, taskInfo[field]);
                }
                task.unbind('change', modelChangeHandler);
            },
            _resolveSummaryFields: function (summary) {
                if (!summary) {
                    return;
                }
                this._updateSummary(summary);
                if (!this.taskChildren(summary).length) {
                    return;
                }
                this._resolveSummaryStart(summary);
                this._resolveSummaryEnd(summary);
                this._resolveSummaryPercentComplete(summary);
            },
            _resolveSummaryStart: function (summary) {
                var that = this;
                var getSummaryStart = function (parentTask) {
                    var children = that.taskChildren(parentTask);
                    var min = children[0].start.getTime();
                    var currentMin;
                    for (var i = 1, l = children.length; i < l; i++) {
                        currentMin = children[i].start.getTime();
                        if (currentMin < min) {
                            min = currentMin;
                        }
                    }
                    return new Date(min);
                };
                this._updateSummaryRecursive(summary, 'start', getSummaryStart);
            },
            _resolveSummaryEnd: function (summary) {
                var that = this;
                var getSummaryEnd = function (parentTask) {
                    var children = that.taskChildren(parentTask);
                    var max = children[0].end.getTime();
                    var currentMax;
                    for (var i = 1, l = children.length; i < l; i++) {
                        currentMax = children[i].end.getTime();
                        if (currentMax > max) {
                            max = currentMax;
                        }
                    }
                    return new Date(max);
                };
                this._updateSummaryRecursive(summary, 'end', getSummaryEnd);
            },
            _resolveSummaryPercentComplete: function (summary) {
                var that = this;
                var getSummaryPercentComplete = function (parentTask) {
                    var children = that.taskChildren(parentTask);
                    var percentComplete = new Query(children).aggregate([{
                            field: 'percentComplete',
                            aggregate: 'average'
                        }]);
                    return percentComplete.percentComplete.average;
                };
                this._updateSummaryRecursive(summary, 'percentComplete', getSummaryPercentComplete);
            },
            _updateSummaryRecursive: function (summary, field, callback) {
                if (!summary) {
                    return;
                }
                var value = callback(summary);
                summary.set(field, value);
                var parent = this.taskParent(summary);
                if (parent) {
                    this._updateSummaryRecursive(parent, field, callback);
                }
            },
            _childRemoved: function (parentId, index) {
                var parent = parentId === null ? null : this.get(parentId);
                var children = this.taskChildren(parent);
                for (var i = index, l = children.length; i < l; i++) {
                    children[i].set('orderId', i);
                }
                this._resolveSummaryFields(parent);
            },
            _reorderSiblings: function (task, oldOrderId) {
                var orderId = task.get('orderId');
                var direction = orderId > oldOrderId;
                var startIndex = direction ? oldOrderId : orderId;
                var endIndex = direction ? orderId : oldOrderId;
                var newIndex = direction ? startIndex : startIndex + 1;
                var siblings = this.taskSiblings(task);
                endIndex = Math.min(endIndex, siblings.length - 1);
                for (var i = startIndex; i <= endIndex; i++) {
                    if (siblings[i] === task) {
                        continue;
                    }
                    siblings[i].set('orderId', newIndex);
                    newIndex += 1;
                }
            },
            _updateSummary: function (task) {
                if (task !== null) {
                    var childCount = this.taskChildren(task).length;
                    task.set('summary', childCount > 0);
                }
            },
            _toGanttTask: function (task) {
                if (!(task instanceof GanttTask)) {
                    var taskInfo = task;
                    task = this._createNewModel();
                    task.accept(taskInfo);
                }
                return task;
            }
        });
        GanttDataSource.create = createDataSource(GanttDataSource, 'GanttDataSource');
        extend(true, kendo.data, {
            GanttDataSource: GanttDataSource,
            GanttTask: GanttTask,
            GanttDependencyDataSource: GanttDependencyDataSource,
            GanttDependency: GanttDependency
        });
        var editors = {
            desktop: {
                dateRange: DATERANGEEDITOR,
                resources: RESOURCESEDITOR
            }
        };
        var Editor = kendo.Observable.extend({
            init: function (element, options) {
                kendo.Observable.fn.init.call(this);
                this.element = element;
                this.options = extend(true, {}, this.options, options);
                this.createButton = this.options.createButton;
            },
            fields: function (editors, model) {
                var that = this;
                var options = this.options;
                var messages = options.messages.editor;
                var resources = options.resources;
                var fields;
                var click = function (e) {
                    e.preventDefault();
                    resources.editor(that.container.find(DOT + Gantt.styles.popup.resourcesField), model);
                };
                if (options.editable.template) {
                    fields = $.map(model.fields, function (value, key) {
                        return { field: key };
                    });
                } else {
                    fields = [
                        {
                            field: 'title',
                            title: messages.title
                        },
                        {
                            field: 'start',
                            title: messages.start,
                            editor: editors.dateRange
                        },
                        {
                            field: 'end',
                            title: messages.end,
                            editor: editors.dateRange
                        },
                        {
                            field: 'percentComplete',
                            title: messages.percentComplete,
                            format: PERCENTAGE_FORMAT
                        }
                    ];
                    if (model.get(resources.field)) {
                        fields.push({
                            field: resources.field,
                            title: messages.resources,
                            messages: messages,
                            editor: editors.resources,
                            click: click,
                            styles: Gantt.styles.popup
                        });
                    }
                }
                return fields;
            },
            _buildEditTemplate: function (model, fields, editableFields) {
                var resources = this.options.resources;
                var template = this.options.editable.template;
                var settings = extend({}, kendo.Template, this.options.templateSettings);
                var paramName = settings.paramName;
                var popupStyles = Gantt.styles.popup;
                var html = '';
                if (template) {
                    if (typeof template === STRING) {
                        template = window.unescape(template);
                    }
                    html += kendo.template(template, settings)(model);
                } else {
                    for (var i = 0, length = fields.length; i < length; i++) {
                        var field = fields[i];
                        html += '<div class="' + popupStyles.editLabel + '"><label for="' + field.field + '">' + (field.title || field.field || '') + '</label></div>';
                        if (field.field === resources.field) {
                            html += '<div class="' + popupStyles.resourcesField + '" style="display:none"></div>';
                        }
                        if (!model.editable || model.editable(field.field)) {
                            editableFields.push(field);
                            html += '<div ' + kendo.attr('container-for') + '="' + field.field + '" class="' + popupStyles.editField + '"></div>';
                        } else {
                            var tmpl = '#:';
                            if (field.field) {
                                field = kendo.expr(field.field, paramName);
                                tmpl += field + '==null?\'\':' + field;
                            } else {
                                tmpl += '\'\'';
                            }
                            tmpl += '#';
                            tmpl = kendo.template(tmpl, settings);
                            html += '<div class="' + popupStyles.editField + '">' + tmpl(model) + '</div>';
                        }
                    }
                }
                return html;
            }
        });
        var PopupEditor = Editor.extend({
            destroy: function () {
                this.close();
                this.unbind();
            },
            editTask: function (task) {
                this.editable = this._createPopupEditor(task);
            },
            close: function () {
                var that = this;
                var destroy = function () {
                    if (that.editable) {
                        that.editable.destroy();
                        that.editable = null;
                        that.container = null;
                    }
                    if (that.popup) {
                        that.popup.destroy();
                        that.popup = null;
                    }
                };
                if (this.editable && this.container.is(':visible')) {
                    that.trigger('close', { window: that.container });
                    this.container.data('kendoWindow').bind('deactivate', destroy).close();
                } else {
                    destroy();
                }
            },
            showDialog: function (options) {
                var buttons = options.buttons;
                var popupStyles = Gantt.styles.popup;
                var html = kendo.format('<div class="{0}"><div class="{1}"><p class="{2}">{3}</p><div class="{4}">', popupStyles.form, popupStyles.formContainer, popupStyles.message, options.text, popupStyles.buttonsContainer);
                for (var i = 0, length = buttons.length; i < length; i++) {
                    html += this.createButton(buttons[i]);
                }
                html += '</div></div></div>';
                var wrapper = this.element;
                if (this.popup) {
                    this.popup.destroy();
                }
                var popup = this.popup = $(html).appendTo(wrapper).eq(0).on('click', DOT + popupStyles.button, function (e) {
                    e.preventDefault();
                    popup.close();
                    var buttonIndex = $(e.currentTarget).index();
                    buttons[buttonIndex].click();
                }).kendoWindow({
                    modal: true,
                    autoFocus: false,
                    resizable: false,
                    draggable: false,
                    title: options.title,
                    visible: false,
                    deactivate: function () {
                        this.destroy();
                        wrapper.focus();
                    }
                }).getKendoWindow();
                popup.center().open();
                popup.element.find('.k-primary').focus();
            },
            _createPopupEditor: function (task) {
                var that = this;
                var options = {};
                var messages = this.options.messages;
                var ganttStyles = Gantt.styles;
                var popupStyles = ganttStyles.popup;
                var html = kendo.format('<div {0}="{1}" class="{2} {3}"><div class="{4}">', kendo.attr('uid'), task.uid, popupStyles.form, popupStyles.editForm, popupStyles.formContainer);
                var fields = this.fields(editors.desktop, task);
                var editableFields = [];
                html += this._buildEditTemplate(task, fields, editableFields);
                html += '<div class="' + popupStyles.buttonsContainer + '">';
                html += this.createButton({
                    name: 'update',
                    text: messages.save,
                    className: Gantt.styles.primary
                });
                html += this.createButton({
                    name: 'cancel',
                    text: messages.cancel
                });
                if (that.options.editable.destroy !== false) {
                    html += this.createButton({
                        name: 'delete',
                        text: messages.destroy
                    });
                }
                html += '</div></div></div>';
                var container = this.container = $(html).appendTo(this.element).eq(0).kendoWindow(extend({
                    modal: true,
                    resizable: false,
                    draggable: true,
                    title: messages.editor.editorTitle,
                    visible: false,
                    close: function (e) {
                        if (e.userTriggered) {
                            if (that.trigger('cancel', {
                                    container: container,
                                    model: task
                                })) {
                                e.preventDefault();
                            }
                        }
                    }
                }, options));
                var editableWidget = container.kendoEditable({
                    fields: editableFields,
                    model: task,
                    clearContainer: false,
                    validateOnBlur: true,
                    target: that.options.target
                }).data('kendoEditable');
                kendo.cycleForm(container);
                if (!this.trigger('edit', {
                        container: container,
                        model: task
                    })) {
                    container.data('kendoWindow').center().open();
                    container.on(CLICK + NS, DOT + ganttStyles.buttonCancel, function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        that.trigger('cancel', {
                            container: container,
                            model: task
                        });
                    });
                    container.on(CLICK + NS, DOT + ganttStyles.buttonSave, function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        var fields = that.fields(editors.desktop, task);
                        var updateInfo = {};
                        var field;
                        for (var i = 0, length = fields.length; i < length; i++) {
                            field = fields[i].field;
                            updateInfo[field] = task.get(field);
                        }
                        that.trigger('save', {
                            container: container,
                            model: task,
                            updateInfo: updateInfo
                        });
                    });
                    container.on(CLICK + NS, DOT + ganttStyles.buttonDelete, function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                        that.trigger('remove', {
                            container: container,
                            model: task
                        });
                    });
                } else {
                    that.trigger('cancel', {
                        container: container,
                        model: task
                    });
                }
                return editableWidget;
            }
        });
        var ResourceEditor = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                this.wrapper = this.element;
                this.model = this.options.model;
                this.resourcesField = this.options.resourcesField;
                this.createButton = this.options.createButton;
                this._initContainer();
                this._attachHandlers();
            },
            events: ['save'],
            open: function () {
                this.window.center().open();
            },
            close: function () {
                this.window.bind('deactivate', proxy(this.destroy, this)).close();
            },
            destroy: function () {
                this._dettachHandlers();
                this.grid.destroy();
                this.grid = null;
                this.window.destroy();
                this.window = null;
                Widget.fn.destroy.call(this);
                kendo.destroy(this.wrapper);
                this.element = this.wrapper = null;
            },
            _attachHandlers: function () {
                var ganttStyles = Gantt.styles;
                var grid = this.grid;
                var closeHandler = this._cancelProxy = proxy(this._cancel, this);
                this.container.on(CLICK + NS, DOT + ganttStyles.buttonCancel, this._cancelProxy);
                this._saveProxy = proxy(this._save, this);
                this.container.on(CLICK + NS, DOT + ganttStyles.buttonSave, this._saveProxy);
                this.window.bind('close', function (e) {
                    if (e.userTriggered) {
                        closeHandler(e);
                    }
                });
                grid.wrapper.on(CLICK + NS, 'input[type=\'checkbox\']', function () {
                    var element = $(this);
                    var row = $(element).closest('tr');
                    var model = grid.dataSource.getByUid(row.attr(kendo.attr('uid')));
                    var value = $(element).is(':checked') ? 1 : '';
                    model.set('value', value);
                });
            },
            _dettachHandlers: function () {
                this._cancelProxy = null;
                this._saveProxy = null;
                this.container.off(NS);
                this.grid.wrapper.off();
            },
            _cancel: function (e) {
                e.preventDefault();
                this.close();
            },
            _save: function (e) {
                e.preventDefault();
                this._updateModel();
                if (!this.wrapper.is(DOT + Gantt.styles.popup.resourcesField)) {
                    this.trigger('save', {
                        container: this.wrapper,
                        model: this.model
                    });
                }
                this.close();
            },
            _initContainer: function () {
                var that = this;
                var popupStyles = Gantt.styles.popup;
                var dom = kendo.format('<div class="{0} {1}"><div class="{2} {3}"/></div>"', popupStyles.form, popupStyles.editForm, popupStyles.formContainer, popupStyles.resourcesFormContainer);
                dom = $(dom);
                this.container = dom.find(DOT + popupStyles.resourcesFormContainer);
                this.window = dom.kendoWindow({
                    modal: true,
                    resizable: false,
                    draggable: true,
                    visible: false,
                    title: this.options.messages.resourcesEditorTitle,
                    open: function () {
                        that.grid.resize(true);
                    }
                }).data('kendoWindow');
                this._resourceGrid();
                this._createButtons();
            },
            _resourceGrid: function () {
                var that = this;
                var messages = this.options.messages;
                var element = $('<div id="resources-grid"/>').appendTo(this.container);
                this.grid = new kendo.ui.Grid(element, {
                    columns: [
                        {
                            field: 'name',
                            title: messages.resourcesHeader,
                            template: '<label><input type=\'checkbox\' value=\'#=name#\'' + '# if (value > 0 && value !== null) {#' + 'checked=\'checked\'' + '# } #' + '/>#=name#</labe>'
                        },
                        {
                            field: 'value',
                            title: messages.unitsHeader,
                            template: function (dataItem) {
                                var valueFormat = dataItem.format;
                                var value = dataItem.value !== null ? dataItem.value : '';
                                return valueFormat ? kendo.toString(value, valueFormat) : value;
                            }
                        }
                    ],
                    height: 280,
                    sortable: true,
                    editable: true,
                    filterable: true,
                    dataSource: {
                        data: that.options.data,
                        schema: {
                            model: {
                                id: 'id',
                                fields: {
                                    id: { from: 'id' },
                                    name: {
                                        from: 'name',
                                        type: 'string',
                                        editable: false
                                    },
                                    value: {
                                        from: 'value',
                                        type: 'number',
                                        defaultValue: ''
                                    },
                                    format: {
                                        from: 'format',
                                        type: 'string'
                                    }
                                }
                            }
                        }
                    },
                    save: function (e) {
                        var value = !!e.values.value;
                        e.container.parent().find('input[type=\'checkbox\']').prop('checked', value);
                    }
                });
            },
            _createButtons: function () {
                var buttons = this.options.buttons;
                var html = '<div class="' + Gantt.styles.popup.buttonsContainer + '">';
                for (var i = 0, length = buttons.length; i < length; i++) {
                    html += this.createButton(buttons[i]);
                }
                html += '</div>';
                this.container.append(html);
            },
            _updateModel: function () {
                var resources = [];
                var value;
                var data = this.grid.dataSource.data();
                for (var i = 0, length = data.length; i < length; i++) {
                    value = data[i].get('value');
                    if (value !== null && value > 0) {
                        resources.push(data[i]);
                    }
                }
                this.model[this.resourcesField] = resources;
            }
        });
        var Gantt = Widget.extend({
            init: function (element, options, events) {
                if (isArray(options)) {
                    options = { dataSource: options };
                }
                defaultCommands = {
                    append: {
                        text: 'Add Task',
                        action: 'add',
                        className: Gantt.styles.toolbar.appendButton,
                        iconClass: Gantt.styles.toolbar.iconPlus
                    },
                    pdf: {
                        text: 'Export to PDF',
                        className: Gantt.styles.toolbar.pdfButton,
                        iconClass: Gantt.styles.toolbar.iconPdf
                    }
                };
                Widget.fn.init.call(this, element, options);
                if (events) {
                    this._events = events;
                }
                this._wrapper();
                this._resources();
                if (!this.options.views || !this.options.views.length) {
                    this.options.views = [
                        'day',
                        'week',
                        'month'
                    ];
                }
                this._timeline();
                this._toolbar();
                this._footer();
                this._adjustDimensions();
                this._preventRefresh = true;
                this.view(this.timeline._selectedViewName);
                this._preventRefresh = false;
                this._dataSource();
                this._assignments();
                this._dropDowns();
                this._list();
                this._dependencies();
                this._resizable();
                this._scrollable();
                this._dataBind();
                this._attachEvents();
                this._createEditor();
                kendo.notify(this);
            },
            events: [
                'dataBinding',
                'dataBound',
                'add',
                'edit',
                'remove',
                'cancel',
                'save',
                'change',
                'navigate',
                'moveStart',
                'move',
                'moveEnd',
                'resizeStart',
                'resize',
                'resizeEnd',
                'columnResize'
            ],
            options: {
                name: 'Gantt',
                autoBind: true,
                navigatable: false,
                selectable: true,
                editable: true,
                resizable: false,
                columnResizeHandleWidth: defaultIndicatorWidth,
                columns: [],
                views: [],
                dataSource: {},
                dependencies: {},
                resources: {},
                assignments: {},
                taskTemplate: null,
                messages: {
                    save: 'Save',
                    cancel: 'Cancel',
                    destroy: 'Delete',
                    deleteTaskConfirmation: TASK_DELETE_CONFIRM,
                    deleteDependencyConfirmation: DEPENDENCY_DELETE_CONFIRM,
                    deleteTaskWindowTitle: 'Delete task',
                    deleteDependencyWindowTitle: 'Delete dependency',
                    views: {
                        day: 'Day',
                        week: 'Week',
                        month: 'Month',
                        year: 'Year',
                        start: 'Start',
                        end: 'End'
                    },
                    actions: {
                        append: 'Add Task',
                        addChild: 'Add Child',
                        insertBefore: 'Add Above',
                        insertAfter: 'Add Below',
                        pdf: 'Export to PDF'
                    },
                    editor: {
                        editorTitle: 'Task',
                        resourcesEditorTitle: 'Resources',
                        title: 'Title',
                        start: 'Start',
                        end: 'End',
                        percentComplete: 'Complete',
                        resources: 'Resources',
                        assignButton: 'Assign',
                        resourcesHeader: 'Resources',
                        unitsHeader: 'Units'
                    }
                },
                showWorkHours: true,
                showWorkDays: true,
                toolbar: null,
                workDayStart: new Date(1980, 1, 1, 8, 0, 0),
                workDayEnd: new Date(1980, 1, 1, 17, 0, 0),
                workWeekStart: 1,
                workWeekEnd: 5,
                hourSpan: 1,
                snap: true,
                height: 600,
                listWidth: '30%',
                rowHeight: null
            },
            select: function (value) {
                var list = this.list;
                if (!value) {
                    return list.select();
                }
                list.select(value);
                this.list.element.find('table[role=treegrid]').focus();
                return;
            },
            clearSelection: function () {
                this.list.clearSelection();
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                if (this.dataSource) {
                    this.dataSource.unbind('change', this._refreshHandler);
                    this.dataSource.unbind('progress', this._progressHandler);
                    this.dataSource.unbind('error', this._errorHandler);
                }
                if (this.dependencies) {
                    this.dependencies.unbind('change', this._dependencyRefreshHandler);
                    this.dependencies.unbind('error', this._dependencyErrorHandler);
                }
                if (this.timeline) {
                    this.timeline.unbind();
                    this.timeline.destroy();
                }
                if (this.list) {
                    this.list.unbind();
                    this.list.destroy();
                }
                if (this.footerDropDown) {
                    this.footerDropDown.destroy();
                }
                if (this.headerDropDown) {
                    this.headerDropDown.destroy();
                }
                if (this._editor) {
                    this._editor.destroy();
                }
                if (this._resizeDraggable) {
                    this._resizeDraggable.destroy();
                }
                this.toolbar.off(NS);
                if (supportsMedia) {
                    this._mediaQuery.removeListener(this._mediaQueryHandler);
                    this._mediaQuery = null;
                }
                $(window).off('resize' + NS, this._resizeHandler);
                $(this.wrapper).off(NS);
                this.toolbar = null;
                this.footer = null;
            },
            setOptions: function (options) {
                var newOptions = kendo.deepExtend({}, this.options, options);
                var events = this._events;
                if (!options.views) {
                    var selectedView = this.view().name;
                    newOptions.views = $.map(this.options.views, function (view) {
                        var isSettings = isPlainObject(view);
                        var name = isSettings ? typeof view.type !== 'string' ? view.title : view.type : view;
                        if (selectedView === name) {
                            if (isSettings) {
                                view.selected = true;
                            } else {
                                view = {
                                    type: name,
                                    selected: true
                                };
                            }
                        } else if (isSettings) {
                            view.selected = false;
                        }
                        return view;
                    });
                }
                if (!options.dataSource) {
                    newOptions.dataSource = this.dataSource;
                }
                if (!options.dependencies) {
                    newOptions.dependencies = this.dependencies;
                }
                if (!options.resources) {
                    newOptions.resources = this.resources;
                }
                if (!options.assignments) {
                    newOptions.assignments = this.assignments;
                }
                this.destroy();
                this.element.empty();
                this.options = null;
                this.init(this.element, newOptions, events);
                Widget.fn._setEvents.call(this, newOptions);
            },
            _attachEvents: function () {
                this._resizeHandler = proxy(this.resize, this, false);
                $(window).on('resize' + NS, this._resizeHandler);
            },
            _wrapper: function () {
                var ganttStyles = Gantt.styles;
                var splitBarHandleClassName = [
                    ganttStyles.icon,
                    ganttStyles.resizeHandle
                ].join(' ');
                var options = this.options;
                var height = options.height;
                var width = options.width;
                this.wrapper = this.element.addClass(ganttStyles.wrapper).append('<div class=\'' + ganttStyles.listWrapper + '\'><div></div></div>').append('<div class=\'' + ganttStyles.splitBarWrapper + '\'><div class=\'' + splitBarHandleClassName + '\'></div></div>').append('<div class=\'' + ganttStyles.timelineWrapper + '\'><div></div></div>');
                this.wrapper.find(DOT + ganttStyles.list).width(options.listWidth);
                if (height) {
                    this.wrapper.height(height);
                }
                if (width) {
                    this.wrapper.width(width);
                }
                if (options.rowHeight) {
                    this.wrapper.addClass(ganttStyles.rowHeight);
                }
            },
            _toolbar: function () {
                var that = this;
                var ganttStyles = Gantt.styles;
                var viewsSelector = DOT + ganttStyles.toolbar.views + ' > li';
                var pdfSelector = DOT + ganttStyles.toolbar.pdfButton;
                var toggleSelector = DOT + ganttStyles.buttonToggle;
                var contentSelector = DOT + ganttStyles.gridContent;
                var treelist = $(DOT + ganttStyles.list);
                var timeline = $(DOT + ganttStyles.timeline);
                var hoveredClassName = ganttStyles.hovered;
                var actions = this.options.toolbar;
                var actionsWrap = $('<div class=\'' + ganttStyles.toolbar.actions + '\'>');
                var toolbar;
                var views;
                var toggleButton;
                var handler = function (e) {
                    if (e.matches) {
                        treelist.css({
                            'display': 'none',
                            'max-width': 0
                        });
                    } else {
                        treelist.css({
                            'display': 'inline-block',
                            'width': '30%',
                            'max-width': 'none'
                        });
                        timeline.css('display', 'inline-block');
                        that.refresh();
                        timeline.find(contentSelector).scrollTop(that.scrollTop);
                    }
                    that._resize();
                };
                if (!isFunction(actions)) {
                    actions = typeof actions === STRING ? actions : this._actions(actions);
                    actions = proxy(kendo.template(actions), this);
                }
                toggleButton = $(TOGGLE_BUTTON_TEMPLATE({ styles: ganttStyles.toolbar }));
                views = $(HEADER_VIEWS_TEMPLATE({
                    ns: kendo.ns,
                    views: this.timeline.views,
                    styles: ganttStyles.toolbar
                }));
                actionsWrap.append(actions({}));
                toolbar = $('<div class=\'' + ganttStyles.toolbar.headerWrapper + '\'>').append(toggleButton).append(views).append(actionsWrap);
                if (views.find('li').length > 1) {
                    views.prepend(VIEWBUTTONTEMPLATE({ styles: ganttStyles.toolbar }));
                }
                this.wrapper.prepend(toolbar);
                this.toolbar = toolbar;
                if (supportsMedia) {
                    this._mediaQueryHandler = proxy(handler, this);
                    this._mediaQuery = window.matchMedia('(max-width: 480px)');
                    this._mediaQuery.addListener(this._mediaQueryHandler);
                }
                toolbar.on(CLICK + NS, viewsSelector, function (e) {
                    e.preventDefault();
                    var list = that.list;
                    var name = $(this).attr(kendo.attr('name'));
                    var currentView = views.find(DOT + ganttStyles.toolbar.currentView);
                    if (currentView.is(':visible')) {
                        currentView.parent().toggleClass(ganttStyles.toolbar.expanded);
                    }
                    if (list.editable && list.editable.trigger('validate')) {
                        return;
                    }
                    if (!that.trigger('navigate', { view: name })) {
                        that.view(name);
                    }
                    that.toolbar.find(DOT + ganttStyles.focused).removeClass(ganttStyles.focused);
                }).on('keydown' + NS, viewsSelector, function (e) {
                    var views = $(DOT + ganttStyles.toolbar.views).children(':not(.k-current-view)');
                    var focusedViewIndex = views.index(that._focusedView && that._focusedView[0] || views.closest(DOT + ganttStyles.selected)[0]);
                    if (e.keyCode === keys.RIGHT) {
                        $(that.toolbar.find(DOT + ganttStyles.focused)).removeClass(ganttStyles.focused);
                        that._focusedView = focusedViewIndex + 1 === views.length ? $(views[0]) : $(views[focusedViewIndex + 1]);
                        that._focusedView.focus().addClass(ganttStyles.focused);
                        e.preventDefault();
                    } else if (e.keyCode === keys.LEFT) {
                        $(that.toolbar.find(DOT + ganttStyles.focused)).removeClass(ganttStyles.focused);
                        that._focusedView = focusedViewIndex === 0 ? $(views[views.length - 1]) : $(views[focusedViewIndex - 1]);
                        that._focusedView.focus().addClass(ganttStyles.focused);
                        e.preventDefault();
                    } else if (e.keyCode === keys.DOWN && that.toolbar.find(DOT + ganttStyles.toolbar.currentView).parent().hasClass(ganttStyles.toolbar.expanded)) {
                        $(that.toolbar.find(DOT + ganttStyles.focused)).removeClass(ganttStyles.focused);
                        that._focusedView = focusedViewIndex + 1 === views.length ? $(views[0]) : $(views[focusedViewIndex + 1]);
                        that._focusedView.focus().addClass(ganttStyles.focused);
                        e.preventDefault();
                    } else if (e.keyCode === keys.UP && that.toolbar.find(DOT + ganttStyles.toolbar.currentView).parent().hasClass(ganttStyles.toolbar.expanded)) {
                        $(that.toolbar.find(DOT + ganttStyles.focused)).removeClass(ganttStyles.focused);
                        that._focusedView = focusedViewIndex === 0 ? $(views[views.length - 1]) : $(views[focusedViewIndex - 1]);
                        that._focusedView.focus().addClass(ganttStyles.focused);
                        e.preventDefault();
                    } else if ((e.keyCode === keys.ENTER || e.keyCode === keys.SPACEBAR) && that._focusedView) {
                        that.view(that._focusedView.text().toLowerCase());
                        e.preventDefault();
                    } else if ((e.keyCode === keys.SPACEBAR || e.keyCode === keys.ENTER || e.keyCode === keys.DOWN && e.altKey) && that.toolbar.find(DOT + ganttStyles.toolbar.currentView + ' > a').hasClass(ganttStyles.focused)) {
                        that.toolbar.find(DOT + ganttStyles.toolbar.currentView).parent().toggleClass(ganttStyles.toolbar.expanded);
                        e.preventDefault();
                    } else if (e.keyCode === keys.ESC && that.toolbar.find(DOT + ganttStyles.toolbar.currentView).parent().hasClass(ganttStyles.toolbar.expanded)) {
                        that.toolbar.find(DOT + ganttStyles.toolbar.currentView).parent().toggleClass(ganttStyles.toolbar.expanded).blur();
                        that._focusedView = null;
                        that.toolbar.find(DOT + ganttStyles.toolbar.currentView + ' > a').addClass(ganttStyles.focused).focus();
                        e.preventDefault();
                    } else if (e.keyCode >= 49 && e.keyCode <= 57) {
                        that.view(that.timeline._viewByIndex(e.keyCode - 49));
                    }
                }).on(CLICK + NS, pdfSelector, function (e) {
                    e.preventDefault();
                    that.saveAsPDF();
                }).on(CLICK + NS, toggleSelector, function (e) {
                    e.preventDefault();
                    if (treelist.is(':visible')) {
                        treelist.css({
                            'display': 'none',
                            'width': '0'
                        });
                        timeline.css({
                            'display': 'inline-block',
                            'width': '100%'
                        });
                        that.refresh();
                        timeline.find(contentSelector).scrollTop(that.scrollTop);
                    } else {
                        timeline.css({
                            'display': 'none',
                            'width': 0
                        });
                        treelist.css({
                            'display': 'inline-block',
                            'width': '100%',
                            'max-width': 'none'
                        }).find(contentSelector).scrollTop(that.scrollTop);
                    }
                    that._resize();
                });
                this.wrapper.on('focusout' + NS, function (e) {
                    if (!$(e.relatedTarget).closest(DOT + ganttStyles.toolbar.toolbar).length) {
                        that.toolbar.find(DOT + ganttStyles.focused).removeClass(ganttStyles.focused);
                    }
                    if (!$(e.relatedTarget).closest(DOT + ganttStyles.toolbar.views).length) {
                        that.toolbar.find(DOT + ganttStyles.toolbar.views).removeClass(ganttStyles.toolbar.expanded);
                    }
                }).find(DOT + ganttStyles.toolbar.toolbar + ' li').hover(function () {
                    $(this).addClass(hoveredClassName);
                }, function () {
                    $(this).removeClass(hoveredClassName);
                });
            },
            _actions: function () {
                var options = this.options;
                var editable = options.editable;
                var actions = options.toolbar;
                var html = '';
                if (!isArray(actions)) {
                    if (editable && editable.create !== false) {
                        actions = ['append'];
                    } else {
                        return html;
                    }
                }
                for (var i = 0, length = actions.length; i < length; i++) {
                    html += this._createButton(actions[i]);
                }
                return html;
            },
            _footer: function () {
                var editable = this.options.editable;
                if (!editable || editable.create === false) {
                    return;
                }
                var ganttStyles = Gantt.styles.toolbar;
                var messages = this.options.messages.actions;
                var button = $(kendo.template(BUTTON_TEMPLATE)(extend(true, { styles: ganttStyles }, defaultCommands.append, { text: messages.append })));
                var actionsWrap = $('<div class=\'' + ganttStyles.actions + '\'>').append(button);
                var footer = $('<div class=\'' + ganttStyles.footerWrapper + '\'>').append(actionsWrap);
                this.wrapper.append(footer);
                this.footer = footer;
            },
            _createButton: function (command) {
                var template = command.template || BUTTON_TEMPLATE;
                var messages = this.options.messages.actions;
                var commandName = typeof command === STRING ? command : command.name || command.text;
                var className = defaultCommands[commandName] ? defaultCommands[commandName].className : 'k-gantt-' + (commandName || '').replace(/\s/g, '');
                var options = {
                    iconClass: '',
                    action: '',
                    text: commandName,
                    className: className,
                    styles: Gantt.styles.toolbar
                };
                if (!commandName && !(isPlainObject(command) && command.template)) {
                    throw new Error('Custom commands should have name specified');
                }
                options = extend(true, options, defaultCommands[commandName], { text: messages[commandName] });
                if (isPlainObject(command)) {
                    if (command.className && inArray(options.className, command.className.split(' ')) < 0) {
                        command.className += ' ' + options.className;
                    }
                    options = extend(true, options, command);
                }
                return kendo.template(template)(options);
            },
            _adjustDimensions: function () {
                var element = this.element;
                var ganttStyles = Gantt.styles;
                var listSelector = DOT + ganttStyles.list;
                var timelineSelector = DOT + ganttStyles.timeline;
                var splitBarSelector = DOT + ganttStyles.splitBar;
                var toolbarHeight = outerHeight(this.toolbar);
                var footerHeight = this.footer ? outerHeight(this.footer) : 0;
                var totalHeight = element.height();
                var totalWidth = element.width();
                var splitBarWidth = outerWidth(element.find(splitBarSelector));
                var treeListWidth = outerWidth(element.find(listSelector));
                element.children([
                    listSelector,
                    timelineSelector,
                    splitBarSelector
                ].join(',')).height(totalHeight - (toolbarHeight + footerHeight)).end().children(timelineSelector).width(totalWidth - (splitBarWidth + treeListWidth));
                if (totalWidth < treeListWidth + splitBarWidth) {
                    element.find(listSelector).width(totalWidth - splitBarWidth);
                }
            },
            _scrollTo: function (value) {
                var view = this.timeline.view();
                var list = this.list;
                var attr = kendo.attr('uid');
                var id = typeof value === 'string' ? value : value.closest('tr' + selector()).attr(attr);
                var action;
                var scrollTarget;
                var scrollIntoView = function () {
                    if (scrollTarget.length !== 0) {
                        action();
                    }
                };
                if (view.content.is(':visible')) {
                    scrollTarget = view.content.find(selector(id));
                    action = function () {
                        view._scrollTo(scrollTarget);
                    };
                } else {
                    scrollTarget = list.content.find(selector(id));
                    action = function () {
                        scrollTarget.get(0).scrollIntoView();
                    };
                }
                scrollIntoView();
            },
            _dropDowns: function () {
                var that = this;
                var actionsSelector = DOT + Gantt.styles.toolbar.actions;
                var actionMessages = this.options.messages.actions;
                var timeline = this.timeline;
                var editable = this.options.editable;
                var handler = function (e) {
                    var type = e.type;
                    var orderId;
                    var dataSource = that.dataSource;
                    var task = dataSource._createNewModel();
                    var selected = that.dataItem(that.select());
                    var parent = dataSource.taskParent(selected);
                    var firstSlot = timeline.view()._timeSlots()[0];
                    var target = type === 'add' ? selected : parent;
                    var editable = that.list.editable;
                    if (editable && editable.trigger('validate')) {
                        return;
                    }
                    task.set('title', 'New task');
                    if (target) {
                        task.set('parentId', target.get('id'));
                        task.set('start', target.get('start'));
                        task.set('end', target.get('end'));
                    } else {
                        task.set('start', firstSlot.start);
                        task.set('end', firstSlot.end);
                    }
                    if (type !== 'add') {
                        orderId = selected.get('orderId');
                        orderId = type === 'insert-before' ? orderId : orderId + 1;
                    }
                    that._createTask(task, orderId);
                };
                if (!editable || editable.create === false) {
                    return;
                }
                this.footerDropDown = new TaskDropDown(this.footer.children(actionsSelector).eq(0), {
                    messages: { actions: actionMessages },
                    direction: 'up',
                    animation: { open: { effects: 'slideIn:up' } },
                    navigatable: that.options.navigatable
                });
                this.headerDropDown = new TaskDropDown(this.toolbar.children(actionsSelector).eq(0), {
                    messages: { actions: actionMessages },
                    navigatable: that.options.navigatable
                });
                this.footerDropDown.bind('command', handler);
                this.headerDropDown.bind('command', handler);
            },
            _list: function () {
                var that = this;
                var navigatable = that.options.navigatable;
                var ganttStyles = Gantt.styles;
                var listWrapper = this.wrapper.find(DOT + ganttStyles.list);
                var element = listWrapper.find('> div');
                var toggleButtons = this.wrapper.find(DOT + ganttStyles.toolbar.actions + ' > button');
                var options = {
                    columns: this.options.columns || [],
                    dataSource: this.dataSource,
                    selectable: this.options.selectable,
                    editable: this.options.editable,
                    resizable: this.options.resizable,
                    columnResizeHandleWidth: this.options.columnResizeHandleWidth,
                    listWidth: outerWidth(listWrapper),
                    resourcesField: this.resources.field,
                    rowHeight: this.options.rowHeight
                };
                var columns = options.columns;
                var column;
                var restoreFocus = function () {
                    if (navigatable) {
                        that._current(that._cachedCurrent);
                        focusTable(that.list.content.find('table'), true);
                    }
                    delete that._cachedCurrent;
                };
                for (var i = 0; i < columns.length; i++) {
                    column = columns[i];
                    if (column.field === this.resources.field && typeof column.editor !== 'function') {
                        column.editor = proxy(this._createResourceEditor, this);
                    }
                }
                this.list = new kendo.ui.GanttList(element, options);
                this.list.bind('render', function () {
                    that._navigatable();
                }, true).bind('edit', function (e) {
                    that._cachedCurrent = e.cell;
                    if (that.trigger('edit', {
                            task: e.model,
                            container: e.cell
                        })) {
                        e.preventDefault();
                    }
                }).bind('cancel', function (e) {
                    if (that.trigger('cancel', {
                            task: e.model,
                            container: e.cell
                        })) {
                        e.preventDefault();
                    }
                    restoreFocus();
                }).bind('update', function (e) {
                    that._updateTask(e.task, e.updateInfo);
                    restoreFocus();
                }).bind('change', function () {
                    that.trigger('change');
                    var selection = that.list.select();
                    if (selection.length) {
                        toggleButtons.removeAttr('data-action', 'add');
                        that.timeline.select('[data-uid=\'' + selection.attr('data-uid') + '\']');
                    } else {
                        toggleButtons.attr('data-action', 'add');
                        that.timeline.clearSelection();
                    }
                }).bind('columnResize', function (e) {
                    that.trigger('columnResize', {
                        column: e.column,
                        oldWidth: e.oldWidth,
                        newWidth: e.newWidth
                    });
                });
            },
            _timeline: function () {
                var that = this;
                var ganttStyles = Gantt.styles;
                var options = trimOptions(extend(true, { resourcesField: this.resources.field }, this.options));
                var element = this.wrapper.find(DOT + ganttStyles.timeline + ' > div');
                var currentViewSelector = DOT + ganttStyles.toolbar.currentView + ' > ' + DOT + ganttStyles.toolbar.link;
                this.timeline = new kendo.ui.GanttTimeline(element, options);
                this.timeline.bind('navigate', function (e) {
                    var viewName = e.view.replace(/\./g, '\\.').toLowerCase();
                    var text = that.toolbar.find(DOT + ganttStyles.toolbar.views + ' > li').removeClass(ganttStyles.selected).end().find(DOT + ganttStyles.toolbar.viewButton + '-' + viewName).addClass(ganttStyles.selected).find(DOT + ganttStyles.toolbar.link).text();
                    that.toolbar.find(currentViewSelector).text(text);
                    that.refresh();
                }).bind('moveStart', function (e) {
                    var editable = that.list.editable;
                    if (editable && editable.trigger('validate')) {
                        e.preventDefault();
                        return;
                    }
                    if (that.trigger('moveStart', { task: e.task })) {
                        e.preventDefault();
                    }
                }).bind('move', function (e) {
                    var task = e.task;
                    var start = e.start;
                    var end = new Date(start.getTime() + task.duration());
                    if (that.trigger('move', {
                            task: task,
                            start: start,
                            end: end
                        })) {
                        e.preventDefault();
                    }
                }).bind('moveEnd', function (e) {
                    var task = e.task;
                    var start = e.start;
                    var end = new Date(start.getTime() + task.duration());
                    if (!that.trigger('moveEnd', {
                            task: task,
                            start: start,
                            end: end
                        })) {
                        that._updateTask(that.dataSource.getByUid(task.uid), {
                            start: start,
                            end: end
                        });
                    }
                }).bind('resizeStart', function (e) {
                    var editable = that.list.editable;
                    if (editable && editable.trigger('validate')) {
                        e.preventDefault();
                        return;
                    }
                    if (that.trigger('resizeStart', { task: e.task })) {
                        e.preventDefault();
                    }
                }).bind('resize', function (e) {
                    if (that.trigger('resize', {
                            task: e.task,
                            start: e.start,
                            end: e.end
                        })) {
                        e.preventDefault();
                    }
                }).bind('resizeEnd', function (e) {
                    var task = e.task;
                    var updateInfo = {};
                    if (e.resizeStart) {
                        updateInfo.start = e.start;
                    } else {
                        updateInfo.end = e.end;
                    }
                    if (!that.trigger('resizeEnd', {
                            task: task,
                            start: e.start,
                            end: e.end
                        })) {
                        that._updateTask(that.dataSource.getByUid(task.uid), updateInfo);
                    }
                }).bind('percentResizeStart', function (e) {
                    var editable = that.list.editable;
                    if (editable && editable.trigger('validate')) {
                        e.preventDefault();
                    }
                }).bind('percentResizeEnd', function (e) {
                    that._updateTask(that.dataSource.getByUid(e.task.uid), { percentComplete: e.percentComplete });
                }).bind('dependencyDragStart', function (e) {
                    var editable = that.list.editable;
                    if (editable && editable.trigger('validate')) {
                        e.preventDefault();
                    }
                }).bind('dependencyDragEnd', function (e) {
                    var dependency = that.dependencies._createNewModel({
                        type: e.type,
                        predecessorId: e.predecessor.id,
                        successorId: e.successor.id
                    });
                    that._createDependency(dependency);
                }).bind('select', function (e) {
                    var editable = that.list.editable;
                    if (editable) {
                        editable.trigger('validate');
                    }
                    that.select('[data-uid=\'' + e.uid + '\']');
                }).bind('editTask', function (e) {
                    var editable = that.list.editable;
                    if (editable && editable.trigger('validate')) {
                        return;
                    }
                    that.editTask(e.uid);
                }).bind('clear', function () {
                    that.clearSelection();
                }).bind('removeTask', function (e) {
                    var editable = that.list.editable;
                    if (editable && editable.trigger('validate')) {
                        return;
                    }
                    that.removeTask(that.dataSource.getByUid(e.uid));
                }).bind('removeDependency', function (e) {
                    var editable = that.list.editable;
                    if (editable && editable.trigger('validate')) {
                        return;
                    }
                    that.removeDependency(that.dependencies.getByUid(e.uid));
                });
            },
            _dataSource: function () {
                var options = this.options;
                var dataSource = options.dataSource;
                dataSource = isArray(dataSource) ? { data: dataSource } : dataSource;
                if (this.dataSource && this._refreshHandler) {
                    this.dataSource.unbind('change', this._refreshHandler).unbind('progress', this._progressHandler).unbind('error', this._errorHandler);
                } else {
                    this._refreshHandler = proxy(this.refresh, this);
                    this._progressHandler = proxy(this._requestStart, this);
                    this._errorHandler = proxy(this._error, this);
                }
                this.dataSource = kendo.data.GanttDataSource.create(dataSource).bind('change', this._refreshHandler).bind('progress', this._progressHandler).bind('error', this._errorHandler);
            },
            _dependencies: function () {
                var dependencies = this.options.dependencies || {};
                var dataSource = isArray(dependencies) ? { data: dependencies } : dependencies;
                if (this.dependencies && this._dependencyRefreshHandler) {
                    this.dependencies.unbind('change', this._dependencyRefreshHandler).unbind('error', this._dependencyErrorHandler);
                } else {
                    this._dependencyRefreshHandler = proxy(this.refreshDependencies, this);
                    this._dependencyErrorHandler = proxy(this._error, this);
                }
                this.dependencies = kendo.data.GanttDependencyDataSource.create(dataSource).bind('change', this._dependencyRefreshHandler).bind('error', this._dependencyErrorHandler);
            },
            _resources: function () {
                var resources = this.options.resources;
                var dataSource = resources.dataSource || {};
                this.resources = {
                    field: 'resources',
                    dataTextField: 'name',
                    dataColorField: 'color',
                    dataFormatField: 'format'
                };
                extend(this.resources, resources);
                this.resources.dataSource = kendo.data.DataSource.create(dataSource);
            },
            _assignments: function () {
                var assignments = this.options.assignments;
                var dataSource = assignments.dataSource || {};
                if (this.assignments) {
                    this.assignments.dataSource.unbind('change', this._assignmentsRefreshHandler);
                } else {
                    this._assignmentsRefreshHandler = proxy(this.refresh, this);
                }
                this.assignments = {
                    dataTaskIdField: 'taskId',
                    dataResourceIdField: 'resourceId',
                    dataValueField: 'value'
                };
                extend(this.assignments, assignments);
                this.assignments.dataSource = kendo.data.DataSource.create(dataSource);
                this.assignments.dataSource.bind('change', this._assignmentsRefreshHandler);
            },
            _createEditor: function () {
                var that = this;
                var editor = this._editor = new PopupEditor(this.wrapper, extend({}, this.options, {
                    target: this,
                    resources: {
                        field: this.resources.field,
                        editor: proxy(this._createResourceEditor, this)
                    },
                    createButton: proxy(this._createPopupButton, this)
                }));
                editor.bind('cancel', function (e) {
                    var task = that.dataSource.getByUid(e.model.uid);
                    if (that.trigger('cancel', {
                            container: e.container,
                            task: task
                        })) {
                        e.preventDefault();
                        return;
                    }
                    that.cancelTask();
                }).bind('edit', function (e) {
                    var task = that.dataSource.getByUid(e.model.uid);
                    if (that.trigger('edit', {
                            container: e.container,
                            task: task
                        })) {
                        e.preventDefault();
                    }
                }).bind('save', function (e) {
                    var task = that.dataSource.getByUid(e.model.uid);
                    that.saveTask(task, e.updateInfo);
                }).bind('remove', function (e) {
                    that.removeTask(e.model.uid);
                }).bind('close', that._onDialogClose);
            },
            _onDialogClose: function () {
            },
            _createResourceEditor: function (container, options) {
                var that = this;
                var model = options instanceof ObservableObject ? options : options.model;
                var id = model.get('id');
                var messages = this.options.messages;
                var resourcesField = that.resources.field;
                var editor = this._resourceEditor = new ResourceEditor(container, {
                    resourcesField: resourcesField,
                    data: this._wrapResourceData(id),
                    model: model,
                    messages: extend({}, messages.editor),
                    buttons: [
                        {
                            name: 'update',
                            text: messages.save,
                            className: Gantt.styles.primary
                        },
                        {
                            name: 'cancel',
                            text: messages.cancel
                        }
                    ],
                    createButton: proxy(this._createPopupButton, this),
                    save: function (e) {
                        that._updateAssignments(e.model.get('id'), e.model.get(resourcesField));
                    }
                });
                editor.open();
            },
            _createPopupButton: function (command) {
                var commandName = command.name || command.text;
                var options = {
                    className: Gantt.styles.popup.button + ' k-gantt-' + (commandName || '').replace(/\s/g, ''),
                    text: commandName,
                    attr: ''
                };
                if (!commandName && !(isPlainObject(command) && command.template)) {
                    throw new Error('Custom commands should have name specified');
                }
                if (isPlainObject(command)) {
                    if (command.className) {
                        command.className += ' ' + options.className;
                    }
                    options = extend(true, options, command);
                }
                return kendo.template(COMMAND_BUTTON_TEMPLATE)(options);
            },
            view: function (type) {
                return this.timeline.view(type);
            },
            range: function (range) {
                var dataSource = this.dataSource;
                var view = this.view();
                var timeline = this.timeline;
                if (range) {
                    view.options.range = {
                        start: range.start,
                        end: range.end
                    };
                    timeline._render(dataSource.taskTree());
                    timeline._renderDependencies(this.dependencies.view());
                }
                return {
                    start: view.start,
                    end: view.end
                };
            },
            date: function (date) {
                var view = this.view();
                if (date) {
                    view.options.date = date;
                    view._scrollToDate(date);
                }
                return view.options.date;
            },
            dataItem: function (value) {
                if (!value) {
                    return null;
                }
                var list = this.list;
                var element = list.content.find(value);
                return list._modelFromElement(element);
            },
            setDataSource: function (dataSource) {
                this.options.dataSource = dataSource;
                this._dataSource();
                this.list._setDataSource(this.dataSource);
                if (this.options.autoBind) {
                    dataSource.fetch();
                }
            },
            setDependenciesDataSource: function (dependencies) {
                this.options.dependencies = dependencies;
                this._dependencies();
                if (this.options.autoBind) {
                    dependencies.fetch();
                }
            },
            items: function () {
                return this.wrapper.children('.k-task');
            },
            _updateAssignments: function (id, resources) {
                var dataSource = this.assignments.dataSource;
                var taskId = this.assignments.dataTaskIdField;
                var resourceId = this.assignments.dataResourceIdField;
                var hasMatch = false;
                var assignments = new Query(dataSource.view()).filter({
                    field: taskId,
                    operator: 'eq',
                    value: id
                }).toArray();
                var assignment;
                var resource;
                var value;
                while (assignments.length) {
                    assignment = assignments[0];
                    for (var i = 0, length = resources.length; i < length; i++) {
                        resource = resources[i];
                        if (assignment.get(resourceId) === resource.get('id')) {
                            value = resources[i].get('value');
                            this._updateAssignment(assignment, value);
                            resources.splice(i, 1);
                            hasMatch = true;
                            break;
                        }
                    }
                    if (!hasMatch) {
                        this._removeAssignment(assignment);
                    }
                    hasMatch = false;
                    assignments.shift();
                }
                for (var j = 0, newLength = resources.length; j < newLength; j++) {
                    resource = resources[j];
                    this._createAssignment(resource, id);
                }
                dataSource.sync();
            },
            cancelTask: function () {
                var editor = this._editor;
                var container = editor.container;
                if (container) {
                    editor.close();
                }
            },
            editTask: function (uid) {
                var task = typeof uid === 'string' ? this.dataSource.getByUid(uid) : uid;
                if (!task) {
                    return;
                }
                var taskCopy = this.dataSource._createNewModel(task.toJSON());
                taskCopy.uid = task.uid;
                this.cancelTask();
                this._editTask(taskCopy);
            },
            _editTask: function (task) {
                this._editor.editTask(task);
            },
            saveTask: function (task, updateInfo) {
                var editor = this._editor;
                var container = editor.container;
                var editable = editor.editable;
                if (container && editable && editable.end()) {
                    this._updateTask(task, updateInfo);
                }
            },
            _updateTask: function (task, updateInfo) {
                var resourcesField = this.resources.field;
                if (!this.trigger('save', {
                        task: task,
                        values: updateInfo
                    })) {
                    this._preventRefresh = true;
                    this.dataSource.update(task, updateInfo);
                    if (updateInfo[resourcesField]) {
                        this._updateAssignments(task.get('id'), updateInfo[resourcesField]);
                    }
                    this._syncDataSource();
                }
            },
            _updateAssignment: function (assignment, value) {
                var resourceValueField = this.assignments.dataValueField;
                assignment.set(resourceValueField, value);
            },
            removeTask: function (uid) {
                var that = this;
                var task = typeof uid === 'string' ? this.dataSource.getByUid(uid) : uid;
                if (!task) {
                    return;
                }
                this._taskConfirm(function (cancel) {
                    if (!cancel) {
                        that._removeTask(task);
                    }
                }, task);
            },
            _createTask: function (task, index) {
                if (!this.trigger('add', {
                        task: task,
                        dependency: null
                    })) {
                    var dataSource = this.dataSource;
                    this._preventRefresh = true;
                    if (index === undefined) {
                        dataSource.add(task);
                    } else {
                        dataSource.insert(index, task);
                    }
                    this._scrollToUid = task.uid;
                    this._syncDataSource();
                }
            },
            _createDependency: function (dependency) {
                if (!this.trigger('add', {
                        task: null,
                        dependency: dependency
                    })) {
                    this._preventDependencyRefresh = true;
                    this.dependencies.add(dependency);
                    this._preventDependencyRefresh = false;
                    this.dependencies.sync();
                }
            },
            _createAssignment: function (resource, id) {
                var assignments = this.assignments;
                var dataSource = assignments.dataSource;
                var taskId = assignments.dataTaskIdField;
                var resourceId = assignments.dataResourceIdField;
                var resourceValue = assignments.dataValueField;
                var assignment = dataSource._createNewModel();
                assignment[taskId] = id;
                assignment[resourceId] = resource.get('id');
                assignment[resourceValue] = resource.get('value');
                dataSource.add(assignment);
            },
            removeDependency: function (uid) {
                var that = this;
                var dependency = typeof uid === 'string' ? this.dependencies.getByUid(uid) : uid;
                if (!dependency) {
                    return;
                }
                this._dependencyConfirm(function (cancel) {
                    if (!cancel) {
                        that._removeDependency(dependency);
                    }
                }, dependency);
            },
            _removeTaskDependencies: function (task, dependencies) {
                this._preventDependencyRefresh = true;
                for (var i = 0, length = dependencies.length; i < length; i++) {
                    this.dependencies.remove(dependencies[i]);
                }
                this._preventDependencyRefresh = false;
                this.dependencies.sync();
            },
            _removeTaskAssignments: function (task) {
                var dataSource = this.assignments.dataSource;
                var assignments = dataSource.view();
                var filter = {
                    field: this.assignments.dataTaskIdField,
                    operator: 'eq',
                    value: task.get('id')
                };
                assignments = new Query(assignments).filter(filter).toArray();
                this._preventRefresh = true;
                for (var i = 0, length = assignments.length; i < length; i++) {
                    dataSource.remove(assignments[i]);
                }
                this._preventRefresh = false;
                dataSource.sync();
            },
            _removeTask: function (task) {
                var dependencies = this.dependencies.dependencies(task.id);
                if (!this.trigger('remove', {
                        task: task,
                        dependencies: dependencies
                    })) {
                    this._removeTaskDependencies(task, dependencies);
                    this._removeTaskAssignments(task);
                    this._preventRefresh = true;
                    if (this.dataSource.remove(task)) {
                        this._syncDataSource();
                    }
                    this._preventRefresh = false;
                }
            },
            _removeDependency: function (dependency) {
                if (!this.trigger('remove', {
                        task: null,
                        dependencies: [dependency]
                    })) {
                    if (this.dependencies.remove(dependency)) {
                        this.dependencies.sync();
                    }
                }
            },
            _removeAssignment: function (assignment) {
                this.assignments.dataSource.remove(assignment);
            },
            _taskConfirm: function (callback, task) {
                var messages = this.options.messages;
                this._confirm(callback, {
                    model: task,
                    text: messages.deleteTaskConfirmation,
                    title: messages.deleteTaskWindowTitle
                });
            },
            _dependencyConfirm: function (callback, dependency) {
                var messages = this.options.messages;
                this._confirm(callback, {
                    model: dependency,
                    text: messages.deleteDependencyConfirmation,
                    title: messages.deleteDependencyWindowTitle
                });
            },
            _confirm: function (callback, options) {
                var editable = this.options.editable;
                var messages;
                var buttons;
                if (editable === true || editable.confirmation !== false) {
                    messages = this.options.messages;
                    buttons = [
                        {
                            name: 'delete',
                            text: messages.destroy,
                            className: Gantt.styles.primary,
                            click: function () {
                                callback();
                            }
                        },
                        {
                            name: 'cancel',
                            text: messages.cancel,
                            click: function () {
                                callback(true);
                            }
                        }
                    ];
                    this.showDialog(extend(true, {}, options, { buttons: buttons }));
                } else {
                    callback();
                }
            },
            showDialog: function (options) {
                this._editor.showDialog(options);
            },
            refresh: function () {
                if (this._preventRefresh || this.list.editable) {
                    return;
                }
                this._progress(false);
                var dataSource = this.dataSource;
                var taskTree = dataSource.taskTree();
                var scrollToUid = this._scrollToUid;
                var current;
                var cachedUid;
                var cachedIndex = -1;
                if (this.current) {
                    cachedUid = this.current.closest('tr').attr(kendo.attr('uid'));
                    cachedIndex = this.current.index();
                }
                if (this.trigger('dataBinding')) {
                    return;
                }
                if (this.resources.dataSource.data().length !== 0) {
                    this._assignResources(taskTree);
                }
                if (this._editor) {
                    this._editor.close();
                }
                this.clearSelection();
                this.list._render(taskTree);
                this.timeline._render(taskTree);
                this.timeline._renderDependencies(this.dependencies.view());
                if (scrollToUid) {
                    this._scrollTo(scrollToUid);
                    this.select(selector(scrollToUid));
                }
                if ((scrollToUid || cachedUid) && cachedIndex >= 0) {
                    current = this.list.content.find('tr' + selector(scrollToUid || cachedUid) + ' > td:eq(' + cachedIndex + ')');
                    this._current(current);
                }
                this._scrollToUid = null;
                this.trigger('dataBound');
            },
            refreshDependencies: function () {
                if (this._preventDependencyRefresh) {
                    return;
                }
                if (this.trigger('dataBinding')) {
                    return;
                }
                this.timeline._renderDependencies(this.dependencies.view());
                this.trigger('dataBound');
            },
            _assignResources: function (taskTree) {
                var resources = this.resources;
                var assignments = this.assignments;
                var groupAssigments = function () {
                    var data = assignments.dataSource.view();
                    var group = { field: assignments.dataTaskIdField };
                    data = new Query(data).group(group).toArray();
                    return data;
                };
                var assigments = groupAssigments();
                var applyTaskResource = function (task, action) {
                    var taskId = task.get('id');
                    kendo.setter(resources.field)(task, new ObservableArray([]));
                    for (var i = 0, length = assigments.length; i < length; i++) {
                        if (assigments[i].value === taskId) {
                            action(task, assigments[i].items);
                        }
                    }
                };
                var wrapTask = function (task, items) {
                    for (var j = 0, length = items.length; j < length; j++) {
                        var item = items[j];
                        var resource = resources.dataSource.get(item.get(assignments.dataResourceIdField));
                        var resourceValue = item.get(assignments.dataValueField);
                        var resourcedId = item.get(assignments.dataResourceIdField);
                        var valueFormat = resource.get(resources.dataFormatField) || PERCENTAGE_FORMAT;
                        var formatedValue = kendo.toString(resourceValue, valueFormat);
                        task[resources.field].push(new ObservableObject({
                            id: resourcedId,
                            name: resource.get(resources.dataTextField),
                            color: resource.get(resources.dataColorField),
                            value: resourceValue,
                            formatedValue: formatedValue
                        }));
                    }
                };
                for (var i = 0, length = taskTree.length; i < length; i++) {
                    applyTaskResource(taskTree[i], wrapTask);
                }
            },
            _wrapResourceData: function (id) {
                var that = this;
                var result = [];
                var resource;
                var resources = this.resources.dataSource.view();
                var assignments = this.assignments.dataSource.view();
                var taskAssignments = new Query(assignments).filter({
                    field: that.assignments.dataTaskIdField,
                    operator: 'eq',
                    value: id
                }).toArray();
                var valuePerResource = function (id) {
                    var resourceValue = null;
                    new Query(taskAssignments).filter({
                        field: that.assignments.dataResourceIdField,
                        operator: 'eq',
                        value: id
                    }).select(function (assignment) {
                        resourceValue += assignment.get(that.assignments.dataValueField);
                    });
                    return resourceValue;
                };
                for (var i = 0, length = resources.length; i < length; i++) {
                    resource = resources[i];
                    result.push({
                        id: resource.get('id'),
                        name: resource.get(that.resources.dataTextField),
                        format: resource.get(that.resources.dataFormatField) || PERCENTAGE_FORMAT,
                        value: valuePerResource(resource.id)
                    });
                }
                return result;
            },
            _syncDataSource: function () {
                this._preventRefresh = false;
                this._requestStart();
                this.dataSource.sync();
            },
            _requestStart: function () {
                this._progress(true);
            },
            _error: function () {
                this._progress(false);
            },
            _progress: function (toggle) {
                kendo.ui.progress(this.element, toggle);
            },
            _resizable: function () {
                var that = this;
                var wrapper = this.wrapper;
                var ganttStyles = Gantt.styles;
                var contentSelector = DOT + ganttStyles.gridContent;
                var treeListWrapper = wrapper.find(DOT + ganttStyles.list);
                var timelineWrapper = wrapper.find(DOT + ganttStyles.timeline);
                var treeListWidth;
                var timelineWidth;
                var timelineScroll;
                this._resizeDraggable = wrapper.find(DOT + ganttStyles.splitBar).height(treeListWrapper.height()).hover(function () {
                    $(this).addClass(ganttStyles.splitBarHover);
                }, function () {
                    $(this).removeClass(ganttStyles.splitBarHover);
                }).end().kendoResizable({
                    orientation: 'horizontal',
                    handle: DOT + ganttStyles.splitBar,
                    'start': function () {
                        treeListWidth = treeListWrapper.width();
                        timelineWidth = timelineWrapper.width();
                        timelineScroll = timelineWrapper.find(contentSelector).scrollLeft();
                    },
                    'resize': function (e) {
                        var delta = e.x.initialDelta;
                        if (kendo.support.isRtl(wrapper)) {
                            delta *= -1;
                        }
                        if (treeListWidth + delta < 0 || timelineWidth - delta < 0) {
                            return;
                        }
                        treeListWrapper.width(treeListWidth + delta);
                        timelineWrapper.width(timelineWidth - delta);
                        timelineWrapper.find(contentSelector).scrollLeft(timelineScroll + delta);
                        that.timeline.view()._renderCurrentTime();
                    }
                }).data('kendoResizable');
            },
            _scrollable: function () {
                var that = this;
                var ganttStyles = Gantt.styles;
                var contentSelector = DOT + ganttStyles.gridContent;
                var headerSelector = DOT + ganttStyles.gridHeaderWrap;
                var timelineHeader = this.timeline.element.find(headerSelector);
                var timelineContent = this.timeline.element.find(contentSelector);
                var treeListHeader = this.list.element.find(headerSelector);
                var treeListContent = this.list.element.find(contentSelector);
                if (mobileOS) {
                    treeListContent.css('overflow-y', 'auto');
                }
                timelineContent.on('scroll', function () {
                    that.scrollTop = this.scrollTop;
                    timelineHeader.scrollLeft(this.scrollLeft);
                    treeListContent.scrollTop(this.scrollTop);
                });
                treeListContent.on('scroll', function () {
                    treeListHeader.scrollLeft(this.scrollLeft);
                }).on('DOMMouseScroll' + NS + ' mousewheel' + NS, function (e) {
                    var scrollTop = timelineContent.scrollTop();
                    var delta = kendo.wheelDeltaY(e);
                    if (delta) {
                        e.preventDefault();
                        $(e.currentTarget).one('wheel' + NS, false);
                        timelineContent.scrollTop(scrollTop + -delta);
                    }
                });
            },
            _navigatable: function () {
                var that = this;
                var navigatable = this.options.navigatable;
                var editable = this.options.editable;
                var headerTable = this.list.header.find('table');
                var contentTable = this.list.content.find('table');
                var ganttStyles = Gantt.styles;
                var isRtl = kendo.support.isRtl(this.wrapper);
                var timelineContent = this.timeline.element.find(DOT + ganttStyles.gridContent);
                var tables = headerTable.add(contentTable);
                var attr = selector();
                var cellIndex;
                var expandState = {
                    collapse: false,
                    expand: true
                };
                var scroll = function (reverse) {
                    var width = that.timeline.view()._timeSlots()[0].offsetWidth;
                    timelineContent.scrollLeft(timelineContent.scrollLeft() + (reverse ? -width : width));
                };
                var scrollVertical = function (reverse) {
                    var height = that.timeline.view()._rowHeight;
                    timelineContent.scrollTop(timelineContent.scrollTop() + (reverse ? -height : height));
                };
                var moveVertical = function (method) {
                    var parent = that.current.parent('tr' + selector());
                    var index = that.current.index();
                    var subling = parent[method]();
                    if (that.select().length !== 0) {
                        that.clearSelection();
                    }
                    if (subling.length !== 0) {
                        that._current(subling.children('td:eq(' + index + ')'));
                        that._scrollTo(that.current);
                    } else {
                        if (that.current.is('td') && method == 'prev') {
                            focusTable(headerTable);
                        } else if (that.current.is('th') && method == 'next') {
                            focusTable(contentTable);
                        }
                    }
                };
                var moveHorizontal = function (method) {
                    var subling = that.current[method]();
                    if (subling.length !== 0) {
                        that._current(subling);
                        cellIndex = that.current.index();
                    }
                };
                var toggleExpandedState = function (value) {
                    var model = that.dataItem(that.current);
                    if (model.summary && model.expanded !== value) {
                        model.set('expanded', value);
                    }
                };
                var deleteAction = function () {
                    var editable = that.options.editable;
                    if (!editable || editable.destroy === false || that.list.editable) {
                        return;
                    }
                    var selectedTask = that.select();
                    var uid = kendo.attr('uid');
                    if (selectedTask.length) {
                        that.removeTask(selectedTask.attr(uid));
                    }
                };
                $(this.wrapper).on('mousedown' + NS, 'tr' + attr + ', div' + attr + ':not(' + DOT + ganttStyles.line + ')', function (e) {
                    var currentTarget = $(e.currentTarget);
                    var isInput = $(e.target).is(':button,a,:input,a>.k-icon,textarea,span.k-icon,span.k-link,.k-input,.k-multiselect-wrap');
                    var current;
                    if (e.ctrlKey) {
                        return;
                    }
                    if (navigatable) {
                        if (currentTarget.is('tr')) {
                            current = $(e.target).closest('td');
                        } else {
                            current = that.list.content.find('tr' + selector(currentTarget.attr(kendo.attr('uid'))) + ' > td:first');
                        }
                        that._current(current);
                    }
                    if ((navigatable || editable) && !isInput) {
                        that._focusTimeout = setTimeout(function () {
                            focusTable(that.list.content.find('table'), true);
                        }, 2);
                    }
                }).on('keydown' + NS, function (e) {
                    var key = e.keyCode;
                    var that = this;
                    var focusableItems = $(that._getToolbarItems());
                    var idx = focusableItems.index(that.toolbar.find(DOT + ganttStyles.focused)[0]);
                    if (idx === -1 && $(e.target).closest(DOT + ganttStyles.toolbar.views).length) {
                        idx = focusableItems.index(that.toolbar.find('.k-gantt-views > .k-state-selected:visible > a, .k-current-view:visible > a')[0]);
                    }
                    var itemToFocus = e.shiftKey ? focusableItems[idx - 1] : focusableItems[idx + 1];
                    if (key === keys.F10) {
                        that.toolbar.find('.k-button:visible:first').addClass(ganttStyles.focused).focus();
                        e.preventDefault();
                    } else if (key == keys.TAB && $(e.target).closest(DOT + ganttStyles.toolbar.toolbar).length) {
                        that.toolbar.find(DOT + ganttStyles.focused).removeClass(ganttStyles.focused).blur();
                        if (itemToFocus) {
                            $(itemToFocus).addClass(ganttStyles.focused).focus();
                            e.preventDefault();
                            return;
                        }
                        if (this.list.element.is(':visible')) {
                            this.list.element.find('table[role=treegrid]').focus();
                        } else {
                            this.element.find(DOT + ganttStyles.tasks)[0].focus();
                        }
                        e.preventDefault();
                    }
                }.bind(this));
                if (navigatable !== true) {
                    contentTable.on('keydown' + NS, function (e) {
                        if (e.keyCode == keys.DELETE) {
                            deleteAction();
                        }
                    });
                    return;
                }
                tables.on('focus' + NS, function () {
                    var selector = this === contentTable.get(0) ? 'td' : 'th';
                    var selection = that.select();
                    var current = that.current || $(selection.length ? selection : this).find(selector + ':eq(' + (cellIndex || 0) + ')');
                    that._current(current);
                    $(that.toolbar.find(DOT + ganttStyles.focused)).removeClass(ganttStyles.focused);
                    $(that.toolbar.find(DOT + ganttStyles.toolbar.currentView)).parent().removeClass(ganttStyles.toolbar.expanded);
                }).on('blur' + NS, function () {
                    that._current();
                    if (this == headerTable) {
                        $(this).attr(TABINDEX, -1);
                    }
                }).on('keydown' + NS, function (e) {
                    var key = e.keyCode;
                    var isCell;
                    if (!that.current) {
                        return;
                    }
                    isCell = that.current.is('td');
                    switch (key) {
                    case keys.RIGHT:
                        e.preventDefault();
                        if (e.altKey) {
                            scroll();
                        } else if (e.ctrlKey) {
                            toggleExpandedState(isRtl ? expandState.collapse : expandState.expand);
                        } else {
                            moveHorizontal(isRtl ? 'prev' : 'next');
                        }
                        break;
                    case keys.LEFT:
                        e.preventDefault();
                        if (e.altKey) {
                            scroll(true);
                        } else if (e.ctrlKey) {
                            toggleExpandedState(isRtl ? expandState.expand : expandState.collapse);
                        } else {
                            moveHorizontal(isRtl ? 'next' : 'prev');
                        }
                        break;
                    case keys.UP:
                        e.preventDefault();
                        if (e.altKey) {
                            scrollVertical(true);
                        } else {
                            moveVertical('prev');
                        }
                        break;
                    case keys.DOWN:
                        e.preventDefault();
                        if (e.altKey) {
                            scrollVertical();
                        } else {
                            moveVertical('next');
                        }
                        break;
                    case keys.SPACEBAR:
                        e.preventDefault();
                        if (isCell) {
                            that.select(that.current.closest('tr'));
                        }
                        break;
                    case keys.ENTER:
                        e.preventDefault();
                        if (isCell) {
                            if (that.options.editable && that.options.editable.update !== false) {
                                that._cachedCurrent = that.current;
                                that.list._startEditHandler(that.current);
                                $(this).one('keyup', function (e) {
                                    e.stopPropagation();
                                });
                            }
                        } else {
                            that.current.children('a.k-link').click();
                        }
                        break;
                    case keys.ESC:
                        break;
                    case keys.DELETE:
                        if (isCell) {
                            deleteAction();
                        }
                        break;
                    default:
                        if (key >= 49 && key <= 57 && e.target.tagName.toLowerCase() !== 'input') {
                            that.view(that.timeline._viewByIndex(key - 49));
                        }
                        break;
                    }
                });
            },
            _getToolbarItems: function () {
                return this.toolbar.find('.k-gantt-toggle:visible').toArray().concat(this.toolbar.find('.k-gantt-actions > .k-button:visible').toArray(), this.toolbar.find('.k-gantt-views > .k-state-selected:visible > a, .k-current-view:visible > a').toArray());
            },
            _current: function (element) {
                var ganttStyles = Gantt.styles;
                var activeElement;
                if (this.current && this.current.length) {
                    this.current.removeClass(ganttStyles.focused).removeAttr('id');
                }
                if (element && element.length) {
                    this.current = element.addClass(ganttStyles.focused).attr('id', ACTIVE_CELL);
                    activeElement = $(kendo._activeElement());
                    if (activeElement.is('table') && this.wrapper.find(activeElement).length > 0) {
                        activeElement.removeAttr(ARIA_DESCENDANT).attr(ARIA_DESCENDANT, ACTIVE_CELL);
                    }
                } else {
                    this.current = null;
                }
            },
            _dataBind: function () {
                var that = this;
                if (that.options.autoBind) {
                    this._preventRefresh = true;
                    this._preventDependencyRefresh = true;
                    var promises = $.map([
                        this.dataSource,
                        this.dependencies,
                        this.resources.dataSource,
                        this.assignments.dataSource
                    ], function (dataSource) {
                        return dataSource.fetch();
                    });
                    $.when.apply(null, promises).done(function () {
                        that._preventRefresh = false;
                        that._preventDependencyRefresh = false;
                        that.refresh();
                    });
                }
            },
            _resize: function () {
                this._adjustDimensions();
                this.timeline.view()._adjustHeight();
                this.timeline.view()._renderCurrentTime();
                this.list._adjustHeight();
            }
        });
        if (kendo.PDFMixin) {
            kendo.PDFMixin.extend(Gantt.fn);
            Gantt.fn._drawPDF = function () {
                var ganttStyles = Gantt.styles;
                var listClass = '.' + ganttStyles.list;
                var listWidth = this.wrapper.find(listClass).width();
                var content = this.wrapper.clone();
                content.find(listClass).css('width', listWidth);
                return this._drawPDFShadow({ content: content }, { avoidLinks: this.options.pdf.avoidLinks });
            };
        }
        kendo.ui.plugin(Gantt);
        extend(true, Gantt, { styles: ganttStyles });
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));