/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('kendo.gantt.timeline', [
        'kendo.dom',
        'kendo.touch',
        'kendo.draganddrop'
    ], f);
}(function () {
    var __meta__ = {
        id: 'gantt.timeline',
        name: 'Gantt Timeline',
        category: 'web',
        description: 'The Gantt Timeline',
        depends: [
            'dom',
            'touch',
            'draganddrop'
        ],
        hidden: true
    };
    (function ($) {
        var Widget = kendo.ui.Widget;
        var kendoDomElement = kendo.dom.element;
        var kendoTextElement = kendo.dom.text;
        var kendoHtmlElement = kendo.dom.html;
        var isPlainObject = $.isPlainObject;
        var outerWidth = kendo._outerWidth;
        var outerHeight = kendo._outerHeight;
        var extend = $.extend;
        var proxy = $.proxy;
        var browser = kendo.support.browser;
        var isRtl = false;
        var keys = kendo.keys;
        var Query = kendo.data.Query;
        var STRING = 'string';
        var NS = '.kendoGanttTimeline';
        var CLICK = 'click';
        var DBLCLICK = 'dblclick';
        var MOUSEMOVE = 'mousemove';
        var MOUSEENTER = 'mouseenter';
        var MOUSELEAVE = 'mouseleave';
        var KEYDOWN = 'keydown';
        var DOT = '.';
        var TIME_HEADER_TEMPLATE = kendo.template('#=kendo.toString(start, \'t\')#');
        var DAY_HEADER_TEMPLATE = kendo.template('#=kendo.toString(start, \'ddd M/dd\')#');
        var WEEK_HEADER_TEMPLATE = kendo.template('#=kendo.toString(start, \'ddd M/dd\')# - #=kendo.toString(kendo.date.addDays(end, -1), \'ddd M/dd\')#');
        var MONTH_HEADER_TEMPLATE = kendo.template('#=kendo.toString(start, \'MMM\')#');
        var YEAR_HEADER_TEMPLATE = kendo.template('#=kendo.toString(start, \'yyyy\')#');
        var RESIZE_HINT = kendo.template('<div class="#=styles.marquee#">' + '<div class="#=styles.marqueeColor#"></div>' + '</div>');
        var RESIZE_TOOLTIP_TEMPLATE = kendo.template('<div style="z-index: 100002;" class="#=styles.tooltipWrapper#">' + '<div class="#=styles.tooltipContent#">' + '<div>#=messages.start#: #=kendo.toString(start, format)#</div>' + '<div>#=messages.end#: #=kendo.toString(end, format)#</div>' + '</div>' + '</div>');
        var PERCENT_RESIZE_TOOLTIP_TEMPLATE = kendo.template('<div style="z-index: 100002;" class="#=styles.tooltipWrapper#" >' + '<div class="#=styles.tooltipContent#">#=text#%</div>' + '<div class="#=styles.tooltipCallout#" style="left:13px;"></div>' + '</div>');
        var TASK_TOOLTIP_TEMPLATE = kendo.template('<div class="#=kendo.htmlEncode(styles.taskDetails)#">' + '<strong>#=kendo.htmlEncode(task.title)#</strong>' + '<div class="#=styles.taskDetailsPercent#">#=kendo.toString(task.percentComplete, "p0")#</div>' + '<ul class="#=styles.reset#">' + '<li>#=messages.start#: #=kendo.toString(task.start, "h:mm tt ddd, MMM d")#</li>' + '<li>#=messages.end#: #=kendo.toString(task.end, "h:mm tt ddd, MMM d")#</li>' + '</ul>' + '</div>');
        var SIZE_CALCULATION_TEMPLATE = '<table style=\'visibility: hidden;\'>' + '<tbody>' + '<tr style=\'height:{0}\'>' + '<td>&nbsp;</td>' + '</tr>' + '</tbody>' + '</table>';
        var defaultViews = {
            day: { type: 'kendo.ui.GanttDayView' },
            week: { type: 'kendo.ui.GanttWeekView' },
            month: { type: 'kendo.ui.GanttMonthView' },
            year: { type: 'kendo.ui.GanttYearView' }
        };
        function trimOptions(options) {
            delete options.name;
            delete options.prefix;
            delete options.views;
            return options;
        }
        function getWorkDays(options) {
            var workDays = [];
            var dayIndex = options.workWeekStart;
            workDays.push(dayIndex);
            while (options.workWeekEnd != dayIndex) {
                if (dayIndex > 6) {
                    dayIndex -= 7;
                } else {
                    dayIndex++;
                }
                workDays.push(dayIndex);
            }
            return workDays;
        }
        function blurActiveElement() {
            var activeElement = kendo._activeElement();
            if (activeElement && activeElement.nodeName.toLowerCase() !== 'body') {
                $(activeElement).blur();
            }
        }
        var viewStyles = {
            alt: 'k-alt',
            reset: 'k-reset',
            nonWorking: 'k-nonwork-hour',
            header: 'k-header',
            gridHeader: 'k-grid-header',
            gridHeaderWrap: 'k-grid-header-wrap',
            gridContent: 'k-grid-content',
            tasksWrapper: 'k-gantt-tables',
            rowsTable: 'k-gantt-rows',
            columnsTable: 'k-gantt-columns',
            tasksTable: 'k-gantt-tasks',
            dependenciesWrapper: 'k-gantt-dependencies',
            resource: 'k-resource',
            resourceAlt: 'k-resource k-alt',
            task: 'k-task',
            taskSingle: 'k-task-single',
            taskMilestone: 'k-task-milestone',
            taskSummary: 'k-task-summary',
            taskWrap: 'k-task-wrap',
            taskMilestoneWrap: 'k-milestone-wrap',
            resourcesWrap: 'k-resources-wrap',
            taskDot: 'k-task-dot',
            taskDotStart: 'k-task-start',
            taskDotEnd: 'k-task-end',
            taskDragHandle: 'k-task-draghandle',
            taskContent: 'k-task-content',
            taskTemplate: 'k-task-template',
            taskActions: 'k-task-actions',
            taskDelete: 'k-task-delete',
            taskComplete: 'k-task-complete',
            taskDetails: 'k-task-details',
            taskDetailsPercent: 'k-task-pct',
            link: 'k-link',
            icon: 'k-icon',
            iconDelete: 'k-i-close',
            taskResizeHandle: 'k-resize-handle',
            taskResizeHandleWest: 'k-resize-w',
            taskResizeHandleEast: 'k-resize-e',
            taskSummaryProgress: 'k-task-summary-progress',
            taskSummaryComplete: 'k-task-summary-complete',
            line: 'k-line',
            lineHorizontal: 'k-line-h',
            lineVertical: 'k-line-v',
            arrowWest: 'k-arrow-w',
            arrowEast: 'k-arrow-e',
            dragHint: 'k-drag-hint',
            dependencyHint: 'k-dependency-hint',
            tooltipWrapper: 'k-widget k-tooltip k-popup k-group k-reset',
            tooltipContent: 'k-tooltip-content',
            tooltipCallout: 'k-callout k-callout-s',
            callout: 'k-callout',
            marquee: 'k-marquee k-gantt-marquee',
            marqueeColor: 'k-marquee-color'
        };
        var GanttView = kendo.ui.GanttView = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                this.title = this.options.title || this.options.name;
                this.header = this.element.find(DOT + GanttView.styles.gridHeader);
                this.content = this.element.find(DOT + GanttView.styles.gridContent);
                this.contentWidth = this.content.width();
                this._workDays = getWorkDays(this.options);
                this._headerTree = options.headerTree;
                this._taskTree = options.taskTree;
                this._taskTemplate = options.taskTemplate ? kendo.template(options.taskTemplate, extend({}, kendo.Template, options.templateSettings)) : null;
                this._dependencyTree = options.dependencyTree;
                this._taskCoordinates = {};
                this._currentTime();
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                clearTimeout(this._tooltipTimeout);
                this.headerRow = null;
                this.header = null;
                this.content = null;
                this._dragHint = null;
                this._resizeHint = null;
                this._resizeTooltip = null;
                this._taskTooltip = null;
                this._percentCompleteResizeTooltip = null;
                this._headerTree = null;
                this._taskTree = null;
                this._dependencyTree = null;
            },
            options: {
                showWorkHours: false,
                showWorkDays: false,
                workDayStart: new Date(1980, 1, 1, 8, 0, 0),
                workDayEnd: new Date(1980, 1, 1, 17, 0, 0),
                workWeekStart: 1,
                workWeekEnd: 5,
                hourSpan: 1,
                slotSize: 100,
                currentTimeMarker: { updateInterval: 10000 }
            },
            renderLayout: function () {
                this._slots = this._createSlots();
                this._tableWidth = this._calculateTableWidth();
                this.createLayout(this._layout());
                this._slotDimensions();
                this._adjustHeight();
                this.content.find(DOT + GanttView.styles.dependenciesWrapper).width(this._tableWidth);
            },
            _adjustHeight: function () {
                if (this.content) {
                    this.content.height(this.element.height() - outerHeight(this.header));
                }
            },
            createLayout: function (rows) {
                var headers = this._headers(rows);
                var colgroup = this._colgroup();
                var tree = this._headerTree;
                var header = kendoDomElement('thead', null, headers);
                var table = kendoDomElement('table', { style: { width: this._tableWidth + 'px' } }, [
                    colgroup,
                    header
                ]);
                tree.render([table]);
                this.headerRow = this.header.find('table:first tr').last();
            },
            _slotDimensions: function () {
                var headers = this.headerRow[0].children;
                var slots = this._timeSlots();
                var slot;
                var header;
                for (var i = 0, length = headers.length; i < length; i++) {
                    header = headers[i];
                    slot = slots[i];
                    slot.offsetLeft = header.offsetLeft;
                    slot.offsetWidth = header.offsetWidth;
                }
            },
            render: function (tasks) {
                var taskCount = tasks.length;
                var styles = GanttView.styles;
                var contentTable;
                var rowsTable = this._rowsTable(taskCount);
                var columnsTable = this._columnsTable(taskCount);
                var tasksTable = this._tasksTable(tasks);
                var currentTimeMarker = this.options.currentTimeMarker;
                var calculatedSize = this.options.calculatedSize;
                var totalHeight;
                this._taskTree.render([
                    rowsTable,
                    columnsTable,
                    tasksTable
                ]);
                contentTable = this.content.find(DOT + styles.rowsTable);
                if (calculatedSize) {
                    totalHeight = calculatedSize.row * tasks.length;
                    this.content.find(DOT + styles.tasksTable).height(totalHeight);
                    contentTable.height(totalHeight);
                }
                this._contentHeight = contentTable.height();
                this._rowHeight = calculatedSize ? calculatedSize.row : this._contentHeight / contentTable.find('tr').length;
                this.content.find(DOT + styles.columnsTable).height(this._contentHeight);
                if (currentTimeMarker !== false && currentTimeMarker.updateInterval !== undefined) {
                    this._renderCurrentTime();
                }
            },
            _rowsTable: function (rowCount) {
                var rows = [];
                var row;
                var styles = GanttView.styles;
                var attributes = [
                    null,
                    { className: styles.alt }
                ];
                for (var i = 0; i < rowCount; i++) {
                    row = kendoDomElement('tr', attributes[i % 2], [kendoDomElement('td', null, [kendoTextElement('\xA0')])]);
                    rows.push(row);
                }
                return this._createTable(1, rows, { className: styles.rowsTable });
            },
            _columnsTable: function () {
                var cells = [];
                var row;
                var styles = GanttView.styles;
                var slots = this._timeSlots();
                var slotsCount = slots.length;
                var slot;
                var slotSpan;
                var totalSpan = 0;
                var attributes;
                for (var i = 0; i < slotsCount; i++) {
                    slot = slots[i];
                    attributes = {};
                    slotSpan = slot.span;
                    totalSpan += slotSpan;
                    if (slotSpan !== 1) {
                        attributes.colspan = slotSpan;
                    }
                    if (slot.isNonWorking) {
                        attributes.className = styles.nonWorking;
                    }
                    cells.push(kendoDomElement('td', attributes, [kendoTextElement('\xA0')]));
                }
                row = kendoDomElement('tr', null, cells);
                return this._createTable(totalSpan, [row], { className: styles.columnsTable });
            },
            _tasksTable: function (tasks) {
                var rows = [];
                var row;
                var cell;
                var position;
                var task;
                var styles = GanttView.styles;
                var coordinates = this._taskCoordinates = {};
                var size = this._calculateMilestoneWidth();
                var milestoneWidth = Math.round(size.width);
                var resourcesField = this.options.resourcesField;
                var className = [
                    styles.resource,
                    styles.resourceAlt
                ];
                var calculatedSize = this.options.calculatedSize;
                var resourcesPosition;
                var resourcesMargin = this._calculateResourcesMargin();
                var taskBorderWidth = this._calculateTaskBorderWidth();
                var resourceStyle;
                var addCoordinates = function (rowIndex) {
                    var taskLeft;
                    var taskRight;
                    taskLeft = position.left;
                    taskRight = taskLeft + position.width;
                    if (task.isMilestone()) {
                        taskLeft -= milestoneWidth / 2;
                        taskRight = taskLeft + milestoneWidth;
                    }
                    coordinates[task.id] = {
                        start: taskLeft,
                        end: taskRight,
                        rowIndex: rowIndex
                    };
                };
                for (var i = 0, l = tasks.length; i < l; i++) {
                    task = tasks[i];
                    position = this._taskPosition(task);
                    position.borderWidth = taskBorderWidth;
                    row = kendoDomElement('tr', null);
                    cell = kendoDomElement('td');
                    if (task.start <= this.end && task.end >= this.start) {
                        cell.children.push(this._renderTask(tasks[i], position));
                        if (task[resourcesField] && task[resourcesField].length) {
                            if (isRtl) {
                                resourcesPosition = this._tableWidth - position.left;
                            } else {
                                resourcesPosition = Math.max(position.width || size.clientWidth, 0) + position.left;
                            }
                            resourceStyle = { width: this._tableWidth - (resourcesPosition + resourcesMargin) + 'px' };
                            resourceStyle[isRtl ? 'right' : 'left'] = resourcesPosition + 'px';
                            if (calculatedSize) {
                                resourceStyle.height = calculatedSize.cell + 'px';
                            }
                            cell.children.push(kendoDomElement('div', {
                                className: styles.resourcesWrap,
                                style: resourceStyle
                            }, this._renderResources(task[resourcesField], className[i % 2])));
                        }
                        addCoordinates(i);
                    }
                    row.children.push(cell);
                    rows.push(row);
                }
                return this._createTable(1, rows, { className: GanttView.styles.tasksTable });
            },
            _createTable: function (colspan, rows, styles) {
                var cols = [];
                var colgroup;
                var tbody;
                for (var i = 0; i < colspan; i++) {
                    cols.push(kendoDomElement('col'));
                }
                colgroup = kendoDomElement('colgroup', null, cols);
                tbody = kendoDomElement('tbody', null, rows);
                if (!styles.style) {
                    styles.style = {};
                }
                styles.style.width = this._tableWidth + 'px';
                return kendoDomElement('table', styles, [
                    colgroup,
                    tbody
                ]);
            },
            _calculateTableWidth: function () {
                var slots = this._timeSlots();
                var maxSpan = 0;
                var totalSpan = 0;
                var currentSpan;
                var tableWidth;
                for (var i = 0, length = slots.length; i < length; i++) {
                    currentSpan = slots[i].span;
                    totalSpan += currentSpan;
                    if (currentSpan > maxSpan) {
                        maxSpan = currentSpan;
                    }
                }
                tableWidth = Math.round(totalSpan * this.options.slotSize / maxSpan);
                return tableWidth;
            },
            _calculateMilestoneWidth: function () {
                var size;
                var className = GanttView.styles.task + ' ' + GanttView.styles.taskMilestone;
                var milestone = $('<div class=\'' + className + '\' style=\'visibility: hidden; position: absolute\'>');
                var boundingClientRect;
                this.content.append(milestone);
                boundingClientRect = milestone[0].getBoundingClientRect();
                size = {
                    'width': boundingClientRect.right - boundingClientRect.left,
                    'clientWidth': milestone[0].clientWidth
                };
                milestone.remove();
                return size;
            },
            _calculateResourcesMargin: function () {
                var margin;
                var wrapper = $('<div class=\'' + GanttView.styles.resourcesWrap + '\' style=\'visibility: hidden; position: absolute\'>');
                this.content.append(wrapper);
                margin = parseInt(wrapper.css(isRtl ? 'margin-right' : 'margin-left'), 10);
                wrapper.remove();
                return margin;
            },
            _calculateTaskBorderWidth: function () {
                var width;
                var className = GanttView.styles.task + ' ' + GanttView.styles.taskSingle;
                var task = $('<div class=\'' + className + '\' style=\'visibility: hidden; position: absolute\'>');
                var computedStyle;
                this.content.append(task);
                computedStyle = kendo.getComputedStyles(task[0], ['border-left-width']);
                width = parseFloat(computedStyle['border-left-width'], 10);
                task.remove();
                return width;
            },
            _renderTask: function (task, position) {
                var taskWrapper;
                var taskElement;
                var editable = this.options.editable;
                var progressHandleOffset;
                var taskLeft = position.left;
                var styles = GanttView.styles;
                var wrapClassName = styles.taskWrap;
                var calculatedSize = this.options.calculatedSize;
                var dragHandleStyle = {};
                var taskWrapAttr = {
                    className: wrapClassName,
                    style: { left: taskLeft + 'px' }
                };
                if (calculatedSize) {
                    taskWrapAttr.style.height = calculatedSize.cell + 'px';
                }
                if (task.summary) {
                    taskElement = this._renderSummary(task, position);
                } else if (task.isMilestone()) {
                    taskElement = this._renderMilestone(task, position);
                    taskWrapAttr.className += ' ' + styles.taskMilestoneWrap;
                } else {
                    taskElement = this._renderSingleTask(task, position);
                }
                taskWrapper = kendoDomElement('div', taskWrapAttr, [taskElement]);
                if (editable && editable.dependencyCreate !== false) {
                    taskWrapper.children.push(kendoDomElement('div', { className: styles.taskDot + ' ' + styles.taskDotStart }));
                    taskWrapper.children.push(kendoDomElement('div', { className: styles.taskDot + ' ' + styles.taskDotEnd }));
                }
                if (!task.summary && !task.isMilestone() && editable && editable.dragPercentComplete !== false && editable.update !== false && this._taskTemplate === null) {
                    progressHandleOffset = Math.round(position.width * task.percentComplete);
                    dragHandleStyle[isRtl ? 'right' : 'left'] = progressHandleOffset + 'px';
                    taskWrapper.children.push(kendoDomElement('div', {
                        className: styles.taskDragHandle,
                        style: dragHandleStyle
                    }));
                }
                return taskWrapper;
            },
            _renderSingleTask: function (task, position) {
                var styles = GanttView.styles;
                var progressWidth = Math.round(position.width * task.percentComplete);
                var taskChildren = [];
                var taskContent;
                var editable = this.options.editable;
                if (this._taskTemplate !== null) {
                    taskContent = kendoHtmlElement(this._taskTemplate(task));
                } else {
                    taskContent = kendoTextElement(task.title);
                    taskChildren.push(kendoDomElement('div', {
                        className: styles.taskComplete,
                        style: { width: progressWidth + 'px' }
                    }));
                }
                var content = kendoDomElement('div', { className: styles.taskContent }, [kendoDomElement('div', { className: styles.taskTemplate }, [taskContent])]);
                taskChildren.push(content);
                if (editable) {
                    if (editable.destroy !== false) {
                        content.children.push(kendoDomElement('span', { className: styles.taskActions }, [kendoDomElement('a', {
                                className: styles.link + ' ' + styles.taskDelete,
                                href: '#',
                                'aria-label': 'Delete'
                            }, [kendoDomElement('span', { className: styles.icon + ' ' + styles.iconDelete })])]));
                    }
                    if (editable.resize !== false && editable.update !== false) {
                        content.children.push(kendoDomElement('span', { className: styles.taskResizeHandle + ' ' + styles.taskResizeHandleWest }));
                        content.children.push(kendoDomElement('span', { className: styles.taskResizeHandle + ' ' + styles.taskResizeHandleEast }));
                    }
                }
                var element = kendoDomElement('div', {
                    className: styles.task + ' ' + styles.taskSingle,
                    'data-uid': task.uid,
                    style: { width: Math.max(position.width - position.borderWidth * 2, 0) + 'px' }
                }, taskChildren);
                return element;
            },
            _renderMilestone: function (task) {
                var styles = GanttView.styles;
                var element = kendoDomElement('div', {
                    className: styles.task + ' ' + styles.taskMilestone,
                    'data-uid': task.uid
                });
                return element;
            },
            _renderSummary: function (task, position) {
                var styles = GanttView.styles;
                var progressWidth = Math.round(position.width * task.percentComplete);
                var element = kendoDomElement('div', {
                    className: styles.task + ' ' + styles.taskSummary,
                    'data-uid': task.uid,
                    style: { width: position.width + 'px' }
                }, [kendoDomElement('div', {
                        className: styles.taskSummaryProgress,
                        style: { width: progressWidth + 'px' }
                    }, [kendoDomElement('div', {
                            className: styles.taskSummaryComplete,
                            style: { width: position.width + 'px' }
                        })])]);
                return element;
            },
            _renderResources: function (resources, className) {
                var children = [];
                var resource;
                for (var i = 0, length = resources.length; i < length; i++) {
                    resource = resources[i];
                    children.push(kendoDomElement('span', {
                        className: className,
                        style: { 'color': resource.get('color') }
                    }, [kendoTextElement(resource.get('name'))]));
                }
                if (isRtl) {
                    children.reverse();
                }
                return children;
            },
            _taskPosition: function (task) {
                var round = Math.round;
                var startLeft = round(this._offset(isRtl ? task.end : task.start));
                var endLeft = round(this._offset(isRtl ? task.start : task.end));
                return {
                    left: startLeft,
                    width: endLeft - startLeft
                };
            },
            _offset: function (date) {
                var slots = this._timeSlots();
                var slot;
                var startOffset;
                var slotDuration;
                var slotOffset = 0;
                var startIndex;
                if (!slots.length) {
                    return 0;
                }
                startIndex = this._slotIndex('start', date);
                slot = slots[startIndex];
                if (slot.end < date) {
                    slotOffset = slot.offsetWidth;
                } else if (slot.start <= date) {
                    startOffset = date - slot.start;
                    slotDuration = slot.end - slot.start;
                    slotOffset = startOffset / slotDuration * slot.offsetWidth;
                }
                if (isRtl) {
                    slotOffset = slot.offsetWidth + 1 - slotOffset;
                }
                return slot.offsetLeft + slotOffset;
            },
            _slotIndex: function (field, value, reverse) {
                var slots = this._timeSlots();
                var startIdx = 0;
                var endIdx = slots.length - 1;
                var middle;
                if (reverse) {
                    slots = [].slice.call(slots).reverse();
                }
                do {
                    middle = Math.ceil((endIdx + startIdx) / 2);
                    if (slots[middle][field] < value) {
                        startIdx = middle;
                    } else {
                        if (middle === endIdx) {
                            middle--;
                        }
                        endIdx = middle;
                    }
                } while (startIdx !== endIdx);
                if (reverse) {
                    startIdx = slots.length - 1 - startIdx;
                }
                return startIdx;
            },
            _timeByPosition: function (x, snap, snapToEnd) {
                var slot = this._slotByPosition(x);
                if (snap) {
                    return snapToEnd ? slot.end : slot.start;
                }
                var offsetLeft = x - $(DOT + GanttView.styles.tasksTable).offset().left;
                var duration = slot.end - slot.start;
                var slotOffset = offsetLeft - slot.offsetLeft;
                if (isRtl) {
                    slotOffset = slot.offsetWidth - slotOffset;
                }
                return new Date(slot.start.getTime() + duration * (slotOffset / slot.offsetWidth));
            },
            _slotByPosition: function (x) {
                var offsetLeft = x - $(DOT + GanttView.styles.tasksTable).offset().left;
                var slotIndex = this._slotIndex('offsetLeft', offsetLeft, isRtl);
                return this._timeSlots()[slotIndex];
            },
            _renderDependencies: function (dependencies) {
                var elements = [];
                var tree = this._dependencyTree;
                for (var i = 0, l = dependencies.length; i < l; i++) {
                    elements.push.apply(elements, this._renderDependency(dependencies[i]));
                }
                tree.render(elements);
            },
            _renderDependency: function (dependency) {
                var predecessor = this._taskCoordinates[dependency.predecessorId];
                var successor = this._taskCoordinates[dependency.successorId];
                var elements;
                var method;
                if (!predecessor || !successor) {
                    return [];
                }
                method = '_render' + [
                    'FF',
                    'FS',
                    'SF',
                    'SS'
                ][isRtl ? 3 - dependency.type : dependency.type];
                elements = this[method](predecessor, successor);
                for (var i = 0, length = elements.length; i < length; i++) {
                    elements[i].attr['data-uid'] = dependency.uid;
                }
                return elements;
            },
            _renderFF: function (from, to) {
                var lines = this._dependencyFF(from, to, false);
                lines[lines.length - 1].children[0] = this._arrow(true);
                return lines;
            },
            _renderSS: function (from, to) {
                var lines = this._dependencyFF(to, from, true);
                lines[0].children[0] = this._arrow(false);
                return lines.reverse();
            },
            _renderFS: function (from, to) {
                var lines = this._dependencyFS(from, to, false);
                lines[lines.length - 1].children[0] = this._arrow(false);
                return lines;
            },
            _renderSF: function (from, to) {
                var lines = this._dependencyFS(to, from, true);
                lines[0].children[0] = this._arrow(true);
                return lines.reverse();
            },
            _dependencyFF: function (from, to, reverse) {
                var that = this;
                var lines = [];
                var left = 0;
                var top = 0;
                var width = 0;
                var height = 0;
                var dir = reverse ? 'start' : 'end';
                var delta;
                var overlap = 2;
                var arrowOverlap = 1;
                var rowHeight = this._rowHeight;
                var minLineWidth = 10;
                var fromTop = from.rowIndex * rowHeight + Math.floor(rowHeight / 2) - 1;
                var toTop = to.rowIndex * rowHeight + Math.floor(rowHeight / 2) - 1;
                var styles = GanttView.styles;
                var addHorizontal = function () {
                    lines.push(that._line(styles.line + ' ' + styles.lineHorizontal, {
                        left: left + 'px',
                        top: top + 'px',
                        width: width + 'px'
                    }));
                };
                var addVertical = function () {
                    lines.push(that._line(styles.line + ' ' + styles.lineVertical, {
                        left: left + 'px',
                        top: top + 'px',
                        height: height + 'px'
                    }));
                };
                left = from[dir];
                top = fromTop;
                width = minLineWidth;
                delta = to[dir] - from[dir];
                if (delta > 0 !== reverse) {
                    width = Math.abs(delta) + minLineWidth;
                }
                if (reverse) {
                    left -= width;
                    width -= arrowOverlap;
                    addHorizontal();
                } else {
                    addHorizontal();
                    left += width - overlap;
                }
                if (toTop < top) {
                    height = top - toTop;
                    height += overlap;
                    top = toTop;
                    addVertical();
                } else {
                    height = toTop - top;
                    height += overlap;
                    addVertical();
                    top += height - overlap;
                }
                width = Math.abs(left - to[dir]);
                if (!reverse) {
                    width -= arrowOverlap;
                    left -= width;
                }
                addHorizontal();
                return lines;
            },
            _dependencyFS: function (from, to, reverse) {
                var that = this;
                var lines = [];
                var left = 0;
                var top = 0;
                var width = 0;
                var height = 0;
                var rowHeight = this._rowHeight;
                var minLineHeight = Math.floor(rowHeight / 2);
                var minLineWidth = 10;
                var minDistance = 2 * minLineWidth;
                var delta = to.start - from.end;
                var overlap = 2;
                var arrowOverlap = 1;
                var fromTop = from.rowIndex * rowHeight + Math.floor(rowHeight / 2) - 1;
                var toTop = to.rowIndex * rowHeight + Math.floor(rowHeight / 2) - 1;
                var styles = GanttView.styles;
                var addHorizontal = function () {
                    lines.push(that._line(styles.line + ' ' + styles.lineHorizontal, {
                        left: left + 'px',
                        top: top + 'px',
                        width: width + 'px'
                    }));
                };
                var addVertical = function () {
                    lines.push(that._line(styles.line + ' ' + styles.lineVertical, {
                        left: left + 'px',
                        top: top + 'px',
                        height: height + 'px'
                    }));
                };
                left = from.end;
                top = fromTop;
                width = minLineWidth;
                if (reverse) {
                    left += arrowOverlap;
                    if (delta > minDistance) {
                        width = delta - (minLineWidth - overlap);
                    }
                    width -= arrowOverlap;
                }
                addHorizontal();
                left += width - overlap;
                if (delta <= minDistance) {
                    height = reverse ? Math.abs(toTop - fromTop) - minLineHeight : minLineHeight;
                    if (toTop < fromTop) {
                        top -= height;
                        height += overlap;
                        addVertical();
                    } else {
                        addVertical();
                        top += height;
                    }
                    width = from.end - to.start + minDistance;
                    if (width < minLineWidth) {
                        width = minLineWidth;
                    }
                    left -= width - overlap;
                    addHorizontal();
                }
                if (toTop < fromTop) {
                    height = top - toTop;
                    top = toTop;
                    height += overlap;
                    addVertical();
                } else {
                    height = toTop - top;
                    addVertical();
                    top += height;
                }
                width = to.start - left;
                if (!reverse) {
                    width -= arrowOverlap;
                }
                addHorizontal();
                return lines;
            },
            _line: function (className, styles) {
                return kendoDomElement('div', {
                    className: className,
                    style: styles
                });
            },
            _arrow: function (direction) {
                return kendoDomElement('span', { className: direction ? GanttView.styles.arrowWest : GanttView.styles.arrowEast });
            },
            _colgroup: function () {
                var slots = this._timeSlots();
                var count = slots.length;
                var cols = [];
                for (var i = 0; i < count; i++) {
                    for (var j = 0, length = slots[i].span; j < length; j++) {
                        cols.push(kendoDomElement('col'));
                    }
                }
                return kendoDomElement('colgroup', null, cols);
            },
            _createDragHint: function (element) {
                this._dragHint = element.clone().addClass(GanttView.styles.dragHint).css('cursor', 'move');
                element.parent().append(this._dragHint);
            },
            _updateDragHint: function (start) {
                var left = this._offset(start);
                this._dragHint.css({ 'left': left });
            },
            _removeDragHint: function () {
                this._dragHint.remove();
                this._dragHint = null;
            },
            _createResizeHint: function (task) {
                var styles = GanttView.styles;
                var taskTop = this._taskCoordinates[task.id].rowIndex * this._rowHeight;
                var tooltipHeight;
                var tooltipTop;
                var options = this.options;
                var messages = options.messages;
                this._resizeHint = $(RESIZE_HINT({ styles: styles })).css({
                    'top': 0,
                    'height': this._contentHeight
                });
                this.content.append(this._resizeHint);
                this._resizeTooltip = $(RESIZE_TOOLTIP_TEMPLATE({
                    styles: styles,
                    start: task.start,
                    end: task.end,
                    messages: messages.views,
                    format: options.resizeTooltipFormat
                })).css({
                    'top': 0,
                    'left': 0
                });
                this.content.append(this._resizeTooltip);
                this._resizeTooltipWidth = outerWidth(this._resizeTooltip);
                tooltipHeight = outerHeight(this._resizeTooltip);
                tooltipTop = taskTop - tooltipHeight;
                if (tooltipTop < 0) {
                    tooltipTop = taskTop + this._rowHeight;
                }
                this._resizeTooltipTop = tooltipTop;
            },
            _updateResizeHint: function (start, end, resizeStart) {
                var left = this._offset(isRtl ? end : start);
                var right = this._offset(isRtl ? start : end);
                var width = right - left;
                var tooltipLeft = resizeStart !== isRtl ? left : right;
                var tablesWidth = this._tableWidth - kendo.support.scrollbar();
                var tooltipWidth = this._resizeTooltipWidth;
                var options = this.options;
                var messages = options.messages;
                var tableOffset = $(DOT + GanttView.styles.tasksTable).offset().left - $(DOT + GanttView.styles.tasksWrapper).offset().left;
                if (isRtl) {
                    left += tableOffset;
                }
                this._resizeHint.css({
                    'left': left,
                    'width': width
                });
                if (this._resizeTooltip) {
                    this._resizeTooltip.remove();
                }
                tooltipLeft -= Math.round(tooltipWidth / 2);
                if (tooltipLeft < 0) {
                    tooltipLeft = 0;
                } else if (tooltipLeft + tooltipWidth > tablesWidth) {
                    tooltipLeft = tablesWidth - tooltipWidth;
                }
                if (isRtl) {
                    tooltipLeft += tableOffset;
                }
                this._resizeTooltip = $(RESIZE_TOOLTIP_TEMPLATE({
                    styles: GanttView.styles,
                    start: start,
                    end: end,
                    messages: messages.views,
                    format: options.resizeTooltipFormat
                })).css({
                    'top': this._resizeTooltipTop,
                    'left': tooltipLeft,
                    'min-width': tooltipWidth
                }).appendTo(this.content);
            },
            _removeResizeHint: function () {
                this._resizeHint.remove();
                this._resizeHint = null;
                this._resizeTooltip.remove();
                this._resizeTooltip = null;
            },
            _updatePercentCompleteTooltip: function (top, left, text) {
                this._removePercentCompleteTooltip();
                var tooltip = this._percentCompleteResizeTooltip = $(PERCENT_RESIZE_TOOLTIP_TEMPLATE({
                    styles: GanttView.styles,
                    text: text
                })).appendTo(this.element);
                var tooltipMiddle = Math.round(outerWidth(tooltip) / 2);
                var arrow = tooltip.find(DOT + GanttView.styles.callout);
                var arrowHeight = Math.round(outerWidth(arrow) / 2);
                tooltip.css({
                    'top': top - (outerHeight(tooltip) + arrowHeight),
                    'left': left - tooltipMiddle
                });
                arrow.css('left', tooltipMiddle - arrowHeight);
            },
            _removePercentCompleteTooltip: function () {
                if (this._percentCompleteResizeTooltip) {
                    this._percentCompleteResizeTooltip.remove();
                }
                this._percentCompleteResizeTooltip = null;
            },
            _updateDependencyDragHint: function (from, to, useVML) {
                this._removeDependencyDragHint();
                if (useVML) {
                    this._creteVmlDependencyDragHint(from, to);
                } else {
                    this._creteDependencyDragHint(from, to);
                }
            },
            _creteDependencyDragHint: function (from, to) {
                var styles = GanttView.styles;
                var deltaX = to.x - from.x;
                var deltaY = to.y - from.y;
                var width = Math.sqrt(deltaX * deltaX + deltaY * deltaY);
                var angle = Math.atan(deltaY / deltaX);
                if (deltaX < 0) {
                    angle += Math.PI;
                }
                $('<div class=\'' + styles.line + ' ' + styles.lineHorizontal + ' ' + styles.dependencyHint + '\'></div>').css({
                    'top': from.y,
                    'left': from.x,
                    'width': width,
                    'transform-origin': '0% 0',
                    '-ms-transform-origin': '0% 0',
                    '-webkit-transform-origin': '0% 0',
                    'transform': 'rotate(' + angle + 'rad)',
                    '-ms-transform': 'rotate(' + angle + 'rad)',
                    '-webkit-transform': 'rotate(' + angle + 'rad)'
                }).appendTo(this.content);
            },
            _creteVmlDependencyDragHint: function (from, to) {
                var hint = $('<kvml:line class=\'' + GanttView.styles.dependencyHint + '\' style=\'position:absolute; top: 0px; left: 0px;\' strokecolor=\'black\' strokeweight=\'2px\' from=\'' + from.x + 'px,' + from.y + 'px\' to=\'' + to.x + 'px,' + to.y + 'px\'' + '></kvml:line>').appendTo(this.content);
                hint[0].outerHTML = hint[0].outerHTML;
            },
            _removeDependencyDragHint: function () {
                this.content.find(DOT + GanttView.styles.dependencyHint).remove();
            },
            _createTaskTooltip: function (task, element, mouseLeft) {
                var styles = GanttView.styles;
                var options = this.options;
                var content = this.content;
                var contentOffset = content.offset();
                var contentWidth = content.width();
                var contentScrollLeft = kendo.scrollLeft(content);
                var row = $(element).parents('tr').first();
                var rowOffset = row.offset();
                var template = options.tooltip && options.tooltip.template ? kendo.template(options.tooltip.template) : TASK_TOOLTIP_TEMPLATE;
                var left = isRtl ? mouseLeft - (contentOffset.left + contentScrollLeft + kendo.support.scrollbar()) : mouseLeft - (contentOffset.left - contentScrollLeft);
                var top = rowOffset.top + outerHeight(row) - contentOffset.top + content.scrollTop();
                var tooltip = this._taskTooltip = $('<div style="z-index: 100002;" class="' + styles.tooltipWrapper + '" >' + '<div class="' + styles.taskContent + '"></div></div>');
                var tooltipWidth;
                tooltip.css({
                    'left': left,
                    'top': top
                }).appendTo(content).find(DOT + styles.taskContent).append(template({
                    styles: styles,
                    task: task,
                    messages: options.messages.views
                }));
                if (outerHeight(tooltip) < rowOffset.top - contentOffset.top) {
                    tooltip.css('top', rowOffset.top - contentOffset.top - outerHeight(tooltip) + content.scrollTop());
                }
                tooltipWidth = outerWidth(tooltip);
                if (tooltipWidth + left - contentScrollLeft > contentWidth) {
                    left -= tooltipWidth;
                    if (left < contentScrollLeft) {
                        left = contentScrollLeft + contentWidth - (tooltipWidth + 17);
                    }
                    tooltip.css('left', left);
                }
            },
            _removeTaskTooltip: function () {
                if (this._taskTooltip) {
                    this._taskTooltip.remove();
                }
                this._taskTooltip = null;
            },
            _scrollTo: function (element) {
                var elementLeft = element.offset().left;
                var elementWidth = element.width();
                var elementRight = elementLeft + elementWidth;
                var row = element.closest('tr');
                var rowTop = row.offset().top;
                var rowHeight = row.height();
                var rowBottom = rowTop + rowHeight;
                var content = this.content;
                var contentOffset = content.offset();
                var contentTop = contentOffset.top;
                var contentHeight = content.height();
                var contentBottom = contentTop + contentHeight;
                var contentLeft = contentOffset.left;
                var contentWidth = content.width();
                var contentRight = contentLeft + contentWidth;
                var scrollbarWidth = kendo.support.scrollbar();
                if (rowTop < contentTop) {
                    content.scrollTop(content.scrollTop() + (rowTop - contentTop));
                } else if (rowBottom > contentBottom) {
                    content.scrollTop(content.scrollTop() + (rowBottom + scrollbarWidth - contentBottom));
                }
                if (elementLeft < contentLeft && elementWidth > contentWidth && elementRight < contentRight || elementRight > contentRight && elementWidth < contentWidth) {
                    content.scrollLeft(content.scrollLeft() + (elementRight + scrollbarWidth - contentRight));
                } else if (elementRight > contentRight && elementWidth > contentWidth && elementLeft > contentLeft || elementLeft < contentLeft && elementWidth < contentWidth) {
                    content.scrollLeft(content.scrollLeft() + (elementLeft - contentLeft));
                }
            },
            _scrollToDate: function (date) {
                var viewStart = this.start;
                var viewEnd = this.end;
                var offset;
                if (date >= viewStart && date < viewEnd) {
                    offset = this._offset(date);
                    if (kendo.support.isRtl(this.element)) {
                        offset = this._tableWidth - offset;
                    }
                    kendo.scrollLeft(this.content, offset);
                }
            },
            _timeSlots: function () {
                if (!this._slots || !this._slots.length) {
                    return [];
                }
                return this._slots[this._slots.length - 1];
            },
            _headers: function (columnLevels) {
                var rows = [];
                var level;
                var headers;
                var column;
                var headerText;
                var styles = GanttView.styles;
                for (var levelIndex = 0, levelCount = columnLevels.length; levelIndex < levelCount; levelIndex++) {
                    level = columnLevels[levelIndex];
                    headers = [];
                    for (var columnIndex = 0, columnCount = level.length; columnIndex < columnCount; columnIndex++) {
                        column = level[columnIndex];
                        headerText = kendoHtmlElement(column.text);
                        headers.push(kendoDomElement('th', {
                            colspan: column.span,
                            className: styles.header + (column.isNonWorking ? ' ' + styles.nonWorking : '')
                        }, [headerText]));
                    }
                    rows.push(kendoDomElement('tr', null, headers));
                }
                return rows;
            },
            _hours: function (start, end) {
                var slotEnd;
                var slots = [];
                var options = this.options;
                var workDayStart = options.workDayStart.getHours();
                var workDayEnd = options.workDayEnd.getHours();
                var isWorkHour;
                var hours;
                var hourSpan = options.hourSpan;
                start = new Date(start);
                end = new Date(end);
                if (options.showWorkHours) {
                    start.setHours(workDayStart);
                }
                while (start < end) {
                    slotEnd = new Date(start);
                    hours = slotEnd.getHours();
                    isWorkHour = hours >= workDayStart && hours < workDayEnd;
                    slotEnd.setHours(slotEnd.getHours() + hourSpan);
                    if (hours == slotEnd.getHours()) {
                        slotEnd.setHours(slotEnd.getHours() + 2 * hourSpan);
                    }
                    if (!options.showWorkHours || isWorkHour) {
                        slots.push({
                            start: start,
                            end: slotEnd,
                            isNonWorking: !isWorkHour,
                            span: 1
                        });
                    }
                    start = slotEnd;
                }
                return slots;
            },
            _days: function (start, end) {
                var slotEnd;
                var slots = [];
                var isWorkDay;
                start = new Date(start);
                end = new Date(end);
                while (start < end) {
                    slotEnd = end < kendo.date.nextDay(start) ? end : kendo.date.nextDay(start);
                    isWorkDay = this._isWorkDay(start);
                    if (!this.options.showWorkDays || isWorkDay) {
                        slots.push({
                            start: start,
                            end: slotEnd,
                            isNonWorking: !isWorkDay,
                            span: 1
                        });
                    }
                    start = slotEnd;
                }
                return slots;
            },
            _weeks: function (start, end) {
                var slotEnd;
                var slots = [];
                var firstDay = this.calendarInfo().firstDay;
                var daySlots;
                var span;
                start = new Date(start);
                end = new Date(end);
                while (start < end) {
                    slotEnd = kendo.date.dayOfWeek(kendo.date.addDays(start, 1), firstDay, 1);
                    if (slotEnd > end) {
                        slotEnd = end;
                    }
                    daySlots = this._days(start, slotEnd);
                    span = daySlots.length;
                    if (span > 0) {
                        slots.push({
                            start: daySlots[0].start,
                            end: daySlots[span - 1].end,
                            span: span
                        });
                    }
                    start = slotEnd;
                }
                return slots;
            },
            _months: function (start, end) {
                var slotEnd;
                var endMonth;
                var slots = [];
                var daySlots;
                var span;
                start = new Date(start);
                end = new Date(end);
                while (start < end) {
                    slotEnd = new Date(start);
                    endMonth = kendo.date.firstDayOfMonth(new Date(slotEnd.setMonth(slotEnd.getMonth() + 1)));
                    slotEnd = end < endMonth ? end : endMonth;
                    daySlots = this._days(start, slotEnd);
                    span = daySlots.length;
                    if (span > 0) {
                        slots.push({
                            start: daySlots[0].start,
                            end: daySlots[span - 1].end,
                            span: span
                        });
                    }
                    start = slotEnd;
                }
                return slots;
            },
            _years: function (start, end) {
                var slotEnd;
                var monthSpan;
                var endMonth;
                var slots = [];
                start = new Date(start);
                end = new Date(end);
                while (start < end) {
                    slotEnd = new Date(start);
                    slotEnd = kendo.date.firstDayOfMonth(new Date(slotEnd.setMonth(12)));
                    if (slotEnd >= end) {
                        slotEnd = end;
                    }
                    endMonth = slotEnd.getMonth() || 12;
                    monthSpan = endMonth - start.getMonth();
                    slots.push({
                        start: start,
                        end: slotEnd,
                        span: monthSpan
                    });
                    start = slotEnd;
                }
                return slots;
            },
            _slotHeaders: function (slots, template) {
                var columns = [];
                var slot;
                for (var i = 0, l = slots.length; i < l; i++) {
                    slot = slots[i];
                    columns.push({
                        text: template(slot),
                        isNonWorking: !!slot.isNonWorking,
                        span: slot.span
                    });
                }
                return columns;
            },
            _isWorkDay: function (date) {
                var day = date.getDay();
                var workDays = this._workDays;
                for (var i = 0, l = workDays.length; i < l; i++) {
                    if (workDays[i] === day) {
                        return true;
                    }
                }
                return false;
            },
            calendarInfo: function () {
                return kendo.getCulture().calendars.standard;
            },
            _renderCurrentTime: function () {
                var currentTime = this._getCurrentTime();
                var timeOffset = this._offset(currentTime);
                var element = $('<div class=\'k-current-time\'></div>');
                var viewStyles = GanttView.styles;
                var tablesWrap = $(DOT + viewStyles.tasksWrapper);
                var tasksTable = $(DOT + viewStyles.tasksTable);
                var slot;
                if (!this.content || !this._timeSlots().length) {
                    return;
                }
                this.content.find('.k-current-time').remove();
                slot = this._timeSlots()[this._slotIndex('start', currentTime)];
                if (currentTime < slot.start || currentTime > slot.end) {
                    return;
                }
                if (tablesWrap.length && tasksTable.length) {
                    timeOffset += tasksTable.offset().left - tablesWrap.offset().left;
                }
                element.css({
                    left: timeOffset + 'px',
                    top: '0px',
                    width: '1px',
                    height: this._contentHeight + 'px'
                }).appendTo(this.content);
            },
            _getCurrentTime: function () {
                return new Date();
            },
            _currentTime: function () {
                var markerOptions = this.options.currentTimeMarker;
                if (markerOptions !== false && markerOptions.updateInterval !== undefined) {
                    this._renderCurrentTime();
                    this._currentTimeUpdateTimer = setInterval(proxy(this._renderCurrentTime, this), markerOptions.updateInterval);
                }
            }
        });
        extend(true, GanttView, { styles: viewStyles });
        kendo.ui.GanttDayView = GanttView.extend({
            name: 'day',
            options: {
                timeHeaderTemplate: TIME_HEADER_TEMPLATE,
                dayHeaderTemplate: DAY_HEADER_TEMPLATE,
                resizeTooltipFormat: 'h:mm tt ddd, MMM d'
            },
            range: function (range) {
                var optionsRange = this.options.range;
                this.start = kendo.date.getDate(range.start);
                this.end = kendo.date.getDate(range.end);
                if (kendo.date.getMilliseconds(range.end) > 0 || this.end.getTime() === this.start.getTime()) {
                    this.end = kendo.date.addDays(this.end, 1);
                }
                if (optionsRange && optionsRange.start) {
                    this.start = kendo.date.getDate(optionsRange.start);
                    this.start.setHours(optionsRange.start.getHours());
                }
                if (optionsRange && optionsRange.end) {
                    this.end = kendo.date.getDate(optionsRange.end);
                    this.end.setHours(optionsRange.end.getHours());
                }
            },
            _createSlots: function () {
                var daySlots;
                var daySlot;
                var hourSlots;
                var hours;
                var slots = [];
                daySlots = this._days(this.start, this.end);
                hourSlots = [];
                for (var i = 0, l = daySlots.length; i < l; i++) {
                    daySlot = daySlots[i];
                    hours = this._hours(daySlot.start, daySlot.end);
                    daySlot.span = hours.length;
                    hourSlots.push.apply(hourSlots, hours);
                }
                slots.push(daySlots);
                slots.push(hourSlots);
                return slots;
            },
            _layout: function () {
                var rows = [];
                var options = this.options;
                rows.push(this._slotHeaders(this._slots[0], kendo.template(options.dayHeaderTemplate)));
                rows.push(this._slotHeaders(this._slots[1], kendo.template(options.timeHeaderTemplate)));
                return rows;
            }
        });
        kendo.ui.GanttWeekView = GanttView.extend({
            name: 'week',
            options: {
                dayHeaderTemplate: DAY_HEADER_TEMPLATE,
                weekHeaderTemplate: WEEK_HEADER_TEMPLATE,
                resizeTooltipFormat: 'h:mm tt ddd, MMM d'
            },
            range: function (range) {
                var optionsRange = this.options.range;
                var calendarInfo = this.calendarInfo();
                var firstDay = calendarInfo.firstDay;
                var rangeEnd = range.end;
                var endDay;
                if (firstDay === rangeEnd.getDay()) {
                    rangeEnd.setDate(rangeEnd.getDate() + 7);
                }
                this.start = kendo.date.getDate(kendo.date.dayOfWeek(range.start, firstDay, -1));
                this.end = kendo.date.getDate(kendo.date.dayOfWeek(rangeEnd, firstDay, 1));
                if (optionsRange && optionsRange.start) {
                    this.start = kendo.date.getDate(optionsRange.start);
                }
                if (optionsRange && optionsRange.end) {
                    endDay = new Date(optionsRange.end);
                    if (kendo.date.getDate(endDay) < optionsRange.end) {
                        this.end = kendo.date.getDate(new Date(endDay.setDate(endDay.getDate() + 1)));
                    } else {
                        this.end = kendo.date.getDate(endDay);
                    }
                }
            },
            _createSlots: function () {
                var slots = [];
                slots.push(this._weeks(this.start, this.end));
                slots.push(this._days(this.start, this.end));
                return slots;
            },
            _layout: function () {
                var rows = [];
                var options = this.options;
                rows.push(this._slotHeaders(this._slots[0], kendo.template(options.weekHeaderTemplate)));
                rows.push(this._slotHeaders(this._slots[1], kendo.template(options.dayHeaderTemplate)));
                return rows;
            }
        });
        kendo.ui.GanttMonthView = GanttView.extend({
            name: 'month',
            options: {
                weekHeaderTemplate: WEEK_HEADER_TEMPLATE,
                monthHeaderTemplate: MONTH_HEADER_TEMPLATE,
                resizeTooltipFormat: 'dddd, MMM d, yyyy'
            },
            range: function (range) {
                var optionsRange = this.options.range;
                var endDay;
                this.start = kendo.date.firstDayOfMonth(range.start);
                this.end = kendo.date.addDays(kendo.date.getDate(kendo.date.lastDayOfMonth(range.end)), 1);
                if (optionsRange && optionsRange.start) {
                    this.start = kendo.date.getDate(optionsRange.start);
                }
                if (optionsRange && optionsRange.end) {
                    endDay = new Date(optionsRange.end);
                    if (kendo.date.getDate(endDay) < optionsRange.end) {
                        this.end = kendo.date.getDate(new Date(endDay.setDate(endDay.getDate() + 1)));
                    } else {
                        this.end = kendo.date.getDate(endDay);
                    }
                }
            },
            _createSlots: function () {
                var slots = [];
                slots.push(this._months(this.start, this.end));
                slots.push(this._weeks(this.start, this.end));
                return slots;
            },
            _layout: function () {
                var rows = [];
                var options = this.options;
                rows.push(this._slotHeaders(this._slots[0], kendo.template(options.monthHeaderTemplate)));
                rows.push(this._slotHeaders(this._slots[1], kendo.template(options.weekHeaderTemplate)));
                return rows;
            }
        });
        kendo.ui.GanttYearView = GanttView.extend({
            name: 'year',
            options: {
                yearHeaderTemplate: YEAR_HEADER_TEMPLATE,
                monthHeaderTemplate: MONTH_HEADER_TEMPLATE,
                resizeTooltipFormat: 'dddd, MMM d, yyyy'
            },
            range: function (range) {
                var optionsRange = this.options.range;
                var firstDayOfMonth;
                this.start = kendo.date.firstDayOfMonth(new Date(range.start.setMonth(0)));
                this.end = kendo.date.firstDayOfMonth(new Date(range.end.setMonth(12)));
                if (optionsRange && optionsRange.start) {
                    this.start = kendo.date.firstDayOfMonth(optionsRange.start);
                }
                if (optionsRange && optionsRange.end) {
                    firstDayOfMonth = kendo.date.firstDayOfMonth(optionsRange.end);
                    this.end = kendo.date.getDate(new Date(firstDayOfMonth.setMonth(firstDayOfMonth.getMonth() + 1)));
                }
            },
            _createSlots: function () {
                var slots = [];
                var monthSlots = this._months(this.start, this.end);
                $(monthSlots).each(function (index, slot) {
                    slot.span = 1;
                });
                slots.push(this._years(this.start, this.end));
                slots.push(monthSlots);
                return slots;
            },
            _layout: function () {
                var rows = [];
                var options = this.options;
                rows.push(this._slotHeaders(this._slots[0], kendo.template(options.yearHeaderTemplate)));
                rows.push(this._slotHeaders(this._slots[1], kendo.template(options.monthHeaderTemplate)));
                return rows;
            }
        });
        var timelineStyles = {
            wrapper: 'k-timeline k-grid k-widget',
            gridHeader: 'k-grid-header',
            gridHeaderWrap: 'k-grid-header-wrap',
            gridContent: 'k-grid-content',
            gridContentWrap: 'k-grid-content',
            tasksWrapper: 'k-gantt-tables',
            dependenciesWrapper: 'k-gantt-dependencies',
            task: 'k-task',
            line: 'k-line',
            taskResizeHandle: 'k-resize-handle',
            taskResizeHandleWest: 'k-resize-w',
            taskDragHandle: 'k-task-draghandle',
            taskComplete: 'k-task-complete',
            taskDelete: 'k-task-delete',
            taskWrapActive: 'k-task-wrap-active',
            taskWrap: 'k-task-wrap',
            taskDot: 'k-task-dot',
            taskDotStart: 'k-task-start',
            taskDotEnd: 'k-task-end',
            hovered: 'k-state-hover',
            selected: 'k-state-selected',
            origin: 'k-origin'
        };
        var GanttTimeline = kendo.ui.GanttTimeline = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                if (!this.options.views || !this.options.views.length) {
                    this.options.views = [
                        'day',
                        'week',
                        'month'
                    ];
                }
                isRtl = kendo.support.isRtl(element);
                this._wrapper();
                this._domTrees();
                this._views();
                this._selectable();
                this._draggable();
                this._resizable();
                this._percentResizeDraggable();
                this._createDependencyDraggable();
                this._attachEvents();
                this._tooltip();
            },
            options: {
                name: 'GanttTimeline',
                messages: {
                    views: {
                        day: 'Day',
                        week: 'Week',
                        month: 'Month',
                        year: 'Year',
                        start: 'Start',
                        end: 'End'
                    }
                },
                snap: true,
                selectable: true,
                editable: true
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                clearTimeout(this._tooltipTimeout);
                if (this._currentTimeUpdateTimer) {
                    clearInterval(this._currentTimeUpdateTimer);
                }
                this._unbindView(this._selectedView);
                if (this._moveDraggable) {
                    this._moveDraggable.destroy();
                }
                if (this._resizeDraggable) {
                    this._resizeDraggable.destroy();
                }
                if (this._percentDraggable) {
                    this._percentDraggable.destroy();
                }
                if (this._dependencyDraggable) {
                    this._dependencyDraggable.destroy();
                }
                if (this.touch) {
                    this.touch.destroy();
                }
                this._headerTree = null;
                this._taskTree = null;
                this._dependencyTree = null;
                this.wrapper.off(NS);
                kendo.destroy(this.wrapper);
            },
            _wrapper: function () {
                var styles = GanttTimeline.styles;
                var that = this;
                var options = this.options;
                var calculateSize = function () {
                    var rowHeight = typeof options.rowHeight === STRING ? options.rowHeight : options.rowHeight + 'px';
                    var table = $(kendo.format(SIZE_CALCULATION_TEMPLATE, rowHeight));
                    var calculatedRowHeight;
                    var calculatedCellHeight;
                    var content = that.wrapper.find(DOT + styles.tasksWrapper);
                    content.append(table);
                    calculatedRowHeight = outerHeight(table.find('tr'));
                    calculatedCellHeight = table.find('td').height();
                    table.remove();
                    return {
                        'row': calculatedRowHeight,
                        'cell': calculatedCellHeight
                    };
                };
                this.wrapper = this.element.addClass(styles.wrapper).append('<div class=\'' + styles.gridHeader + '\'><div class=\'' + styles.gridHeaderWrap + '\'></div></div>').append('<div class=\'' + styles.gridContentWrap + '\'><div class=\'' + styles.tasksWrapper + '\'></div><div class=\'' + styles.dependenciesWrapper + '\'></div></div>');
                if (options.rowHeight) {
                    this._calculatedSize = calculateSize();
                }
            },
            _domTrees: function () {
                var styles = GanttTimeline.styles;
                var tree = kendo.dom.Tree;
                var wrapper = this.wrapper;
                this._headerTree = new tree(wrapper.find(DOT + styles.gridHeaderWrap)[0]);
                this._taskTree = new tree(wrapper.find(DOT + styles.tasksWrapper)[0]);
                this._dependencyTree = new tree(wrapper.find(DOT + styles.dependenciesWrapper)[0]);
            },
            _views: function () {
                var views = this.options.views;
                var view;
                var isSettings;
                var name;
                var defaultView;
                var selected;
                this.views = {};
                for (var i = 0, l = views.length; i < l; i++) {
                    view = views[i];
                    isSettings = isPlainObject(view);
                    if (isSettings && view.selectable === false) {
                        continue;
                    }
                    name = isSettings ? typeof view.type !== 'string' ? view.title : view.type : view;
                    defaultView = defaultViews[name];
                    if (defaultView) {
                        if (isSettings) {
                            view.type = defaultView.type;
                        }
                        defaultView.title = this.options.messages.views[name];
                    }
                    view = extend({ title: name }, defaultView, isSettings ? view : {});
                    if (name) {
                        this.views[name] = view;
                        if (!selected || view.selected) {
                            selected = name;
                        }
                    }
                }
                if (selected) {
                    this._selectedViewName = selected;
                }
            },
            view: function (name) {
                if (name) {
                    this._selectView(name);
                    this.trigger('navigate', {
                        view: name,
                        action: 'changeView'
                    });
                }
                return this._selectedView;
            },
            _selectView: function (name) {
                if (name && this.views[name]) {
                    if (this._selectedView) {
                        this._unbindView(this._selectedView);
                    }
                    this._selectedView = this._initializeView(name);
                    this._selectedViewName = name;
                }
            },
            _viewByIndex: function (index) {
                var view;
                var views = this.views;
                for (view in views) {
                    if (!index) {
                        return view;
                    }
                    index--;
                }
            },
            _initializeView: function (name) {
                var view = this.views[name];
                if (view) {
                    var type = view.type;
                    if (typeof type === 'string') {
                        type = kendo.getter(view.type)(window);
                    }
                    if (type) {
                        var newRange = {};
                        extend(newRange, this.options.range, view.range);
                        var newDate = view.date || this.options.date;
                        view = new type(this.wrapper, trimOptions(extend(true, {
                            headerTree: this._headerTree,
                            taskTree: this._taskTree,
                            dependencyTree: this._dependencyTree,
                            calculatedSize: this._calculatedSize
                        }, view, this.options, {
                            date: newDate,
                            range: newRange
                        })));
                    } else {
                        throw new Error('There is no such view');
                    }
                }
                return view;
            },
            _unbindView: function (view) {
                if (view) {
                    view.destroy();
                }
            },
            _range: function (tasks) {
                var startOrder = {
                    field: 'start',
                    dir: 'asc'
                };
                var endOrder = {
                    field: 'end',
                    dir: 'desc'
                };
                if (!tasks || !tasks.length) {
                    return {
                        start: new Date(),
                        end: new Date()
                    };
                }
                var start = new Query(tasks).sort(startOrder).toArray()[0].start || new Date();
                var end = new Query(tasks).sort(endOrder).toArray()[0].end || new Date();
                return {
                    start: new Date(start),
                    end: new Date(end)
                };
            },
            _render: function (tasks) {
                var view = this.view();
                var range = this._range(tasks);
                var date = view.options.date;
                this._tasks = tasks;
                view.range(range);
                view.renderLayout();
                view.render(tasks);
                if (date) {
                    view._scrollToDate(date);
                }
            },
            _renderDependencies: function (dependencies) {
                this.view()._renderDependencies(dependencies);
            },
            _taskByUid: function (uid) {
                var tasks = this._tasks;
                var length = tasks.length;
                var task;
                for (var i = 0; i < length; i++) {
                    task = tasks[i];
                    if (task.uid === uid) {
                        return task;
                    }
                }
            },
            _draggable: function () {
                var that = this;
                var element;
                var task;
                var currentStart;
                var startOffset;
                var snap = this.options.snap;
                var styles = GanttTimeline.styles;
                var editable = this.options.editable;
                var cleanUp = function () {
                    that.view()._removeDragHint();
                    if (element) {
                        element.css('opacity', 1);
                    }
                    element = null;
                    task = null;
                    that.dragInProgress = false;
                };
                if (!editable || editable.move === false || editable.update === false) {
                    return;
                }
                this._moveDraggable = new kendo.ui.Draggable(this.wrapper, {
                    distance: 0,
                    filter: DOT + styles.task,
                    holdToDrag: kendo.support.mobileOS,
                    ignore: DOT + styles.taskResizeHandle
                });
                this._moveDraggable.bind('dragstart', function (e) {
                    var view = that.view();
                    element = e.currentTarget.parent();
                    task = that._taskByUid(e.currentTarget.attr('data-uid'));
                    if (that.trigger('moveStart', { task: task })) {
                        e.preventDefault();
                        return;
                    }
                    currentStart = task.start;
                    startOffset = view._timeByPosition(e.x.location, snap) - currentStart;
                    view._createDragHint(element);
                    element.css('opacity', 0.5);
                    clearTimeout(that._tooltipTimeout);
                    that.dragInProgress = true;
                }).bind('drag', kendo.throttle(function (e) {
                    if (!that.dragInProgress) {
                        return;
                    }
                    var view = that.view();
                    var date = new Date(view._timeByPosition(e.x.location, snap) - startOffset);
                    var updateHintDate = date;
                    if (!that.trigger('move', {
                            task: task,
                            start: date
                        })) {
                        currentStart = date;
                        if (isRtl) {
                            updateHintDate = new Date(currentStart.getTime() + task.duration());
                        }
                        view._updateDragHint(updateHintDate);
                    }
                }, 15)).bind('dragend', function () {
                    that.trigger('moveEnd', {
                        task: task,
                        start: currentStart
                    });
                    cleanUp();
                }).bind('dragcancel', function () {
                    cleanUp();
                }).userEvents.bind('select', function () {
                    blurActiveElement();
                });
            },
            _resizable: function () {
                var that = this;
                var element;
                var task;
                var currentStart;
                var currentEnd;
                var resizeStart;
                var snap = this.options.snap;
                var styles = GanttTimeline.styles;
                var editable = this.options.editable;
                var cleanUp = function () {
                    that.view()._removeResizeHint();
                    element = null;
                    task = null;
                    that.dragInProgress = false;
                };
                if (!editable || editable.resize === false || editable.update === false) {
                    return;
                }
                this._resizeDraggable = new kendo.ui.Draggable(this.wrapper, {
                    distance: 0,
                    filter: DOT + styles.taskResizeHandle,
                    holdToDrag: false
                });
                this._resizeDraggable.bind('dragstart', function (e) {
                    resizeStart = e.currentTarget.hasClass(styles.taskResizeHandleWest);
                    if (isRtl) {
                        resizeStart = !resizeStart;
                    }
                    element = e.currentTarget.closest(DOT + styles.task);
                    task = that._taskByUid(element.attr('data-uid'));
                    if (that.trigger('resizeStart', { task: task })) {
                        e.preventDefault();
                        return;
                    }
                    currentStart = task.start;
                    currentEnd = task.end;
                    that.view()._createResizeHint(task);
                    clearTimeout(that._tooltipTimeout);
                    that.dragInProgress = true;
                }).bind('drag', kendo.throttle(function (e) {
                    if (!that.dragInProgress) {
                        return;
                    }
                    var view = that.view();
                    var date = view._timeByPosition(e.x.location, snap, !resizeStart);
                    if (resizeStart) {
                        if (date < currentEnd) {
                            currentStart = date;
                        } else {
                            currentStart = currentEnd;
                        }
                    } else {
                        if (date > currentStart) {
                            currentEnd = date;
                        } else {
                            currentEnd = currentStart;
                        }
                    }
                    if (!that.trigger('resize', {
                            task: task,
                            start: currentStart,
                            end: currentEnd
                        })) {
                        view._updateResizeHint(currentStart, currentEnd, resizeStart);
                    }
                }, 15)).bind('dragend', function () {
                    that.trigger('resizeEnd', {
                        task: task,
                        resizeStart: resizeStart,
                        start: currentStart,
                        end: currentEnd
                    });
                    cleanUp();
                }).bind('dragcancel', function () {
                    cleanUp();
                }).userEvents.bind('select', function () {
                    blurActiveElement();
                });
            },
            _percentResizeDraggable: function () {
                var that = this;
                var task;
                var taskElement;
                var taskElementOffset;
                var timelineOffset;
                var originalPercentWidth;
                var maxPercentWidth;
                var currentPercentComplete;
                var tooltipTop;
                var tooltipLeft;
                var styles = GanttTimeline.styles;
                var delta;
                var editable = this.options.editable;
                var cleanUp = function () {
                    that.view()._removePercentCompleteTooltip();
                    taskElement = null;
                    task = null;
                    that.dragInProgress = false;
                };
                var updateElement = function (width) {
                    taskElement.find(DOT + styles.taskComplete).width(width).end().siblings(DOT + styles.taskDragHandle).css(isRtl ? 'right' : 'left', width);
                };
                if (!editable || editable.dragPercentComplete === false || editable.update === false) {
                    return;
                }
                this._percentDraggable = new kendo.ui.Draggable(this.wrapper, {
                    distance: 0,
                    filter: DOT + styles.taskDragHandle,
                    holdToDrag: false
                });
                this._percentDraggable.bind('dragstart', function (e) {
                    if (that.trigger('percentResizeStart')) {
                        e.preventDefault();
                        return;
                    }
                    taskElement = e.currentTarget.siblings(DOT + styles.task);
                    task = that._taskByUid(taskElement.attr('data-uid'));
                    currentPercentComplete = task.percentComplete;
                    taskElementOffset = taskElement.offset();
                    timelineOffset = this.element.offset();
                    originalPercentWidth = taskElement.find(DOT + styles.taskComplete).width();
                    maxPercentWidth = outerWidth(taskElement);
                    clearTimeout(that._tooltipTimeout);
                    that.dragInProgress = true;
                }).bind('drag', kendo.throttle(function (e) {
                    if (!that.dragInProgress) {
                        return;
                    }
                    delta = isRtl ? -e.x.initialDelta : e.x.initialDelta;
                    var currentWidth = Math.max(0, Math.min(maxPercentWidth, originalPercentWidth + delta));
                    currentPercentComplete = Math.round(currentWidth / maxPercentWidth * 100);
                    updateElement(currentWidth);
                    tooltipTop = taskElementOffset.top - timelineOffset.top;
                    tooltipLeft = taskElementOffset.left + currentWidth - timelineOffset.left;
                    if (isRtl) {
                        tooltipLeft += maxPercentWidth - 2 * currentWidth;
                    }
                    that.view()._updatePercentCompleteTooltip(tooltipTop, tooltipLeft, currentPercentComplete);
                }, 15)).bind('dragend', function () {
                    that.trigger('percentResizeEnd', {
                        task: task,
                        percentComplete: currentPercentComplete / 100
                    });
                    cleanUp();
                }).bind('dragcancel', function () {
                    updateElement(originalPercentWidth);
                    cleanUp();
                }).userEvents.bind('select', function () {
                    blurActiveElement();
                });
            },
            _createDependencyDraggable: function () {
                var that = this;
                var originalHandle;
                var hoveredHandle = $();
                var hoveredTask = $();
                var startX;
                var startY;
                var useVML = browser.msie && browser.version < 9;
                var styles = GanttTimeline.styles;
                var editable = this.options.editable;
                var cleanUp = function () {
                    originalHandle.css('display', '').removeClass(styles.hovered);
                    originalHandle.parent().removeClass(styles.origin);
                    originalHandle = null;
                    toggleHandles(false);
                    hoveredTask = $();
                    hoveredHandle = $();
                    that.view()._removeDependencyDragHint();
                    that.dragInProgress = false;
                };
                var toggleHandles = function (value) {
                    if (!hoveredTask.hasClass(styles.origin)) {
                        hoveredTask.find(DOT + styles.taskDot).css('display', value ? 'block' : '');
                        hoveredHandle.toggleClass(styles.hovered, value);
                    }
                };
                if (!editable || editable.dependencyCreate === false) {
                    return;
                }
                if (useVML && document.namespaces) {
                    document.namespaces.add('kvml', 'urn:schemas-microsoft-com:vml', '#default#VML');
                }
                this._dependencyDraggable = new kendo.ui.Draggable(this.wrapper, {
                    distance: 0,
                    filter: DOT + styles.taskDot,
                    holdToDrag: false
                });
                this._dependencyDraggable.bind('dragstart', function (e) {
                    if (that.trigger('dependencyDragStart')) {
                        e.preventDefault();
                        return;
                    }
                    originalHandle = e.currentTarget.css('display', 'block').addClass(styles.hovered);
                    originalHandle.parent().addClass(styles.origin);
                    var elementOffset = originalHandle.offset();
                    var tablesOffset = that.wrapper.find(DOT + styles.tasksWrapper).offset();
                    startX = Math.round(elementOffset.left - tablesOffset.left + outerHeight(originalHandle) / 2);
                    startY = Math.round(elementOffset.top - tablesOffset.top + outerWidth(originalHandle) / 2);
                    clearTimeout(that._tooltipTimeout);
                    that.dragInProgress = true;
                }).bind('drag', kendo.throttle(function (e) {
                    if (!that.dragInProgress) {
                        return;
                    }
                    that.view()._removeDependencyDragHint();
                    var target = $(kendo.elementUnderCursor(e));
                    var tablesOffset = that.wrapper.find(DOT + styles.tasksWrapper).offset();
                    var currentX = e.x.location - tablesOffset.left;
                    var currentY = e.y.location - tablesOffset.top;
                    that.view()._updateDependencyDragHint({
                        x: startX,
                        y: startY
                    }, {
                        x: currentX,
                        y: currentY
                    }, useVML);
                    toggleHandles(false);
                    hoveredHandle = target.hasClass(styles.taskDot) ? target : $();
                    hoveredTask = target.closest(DOT + styles.taskWrap);
                    toggleHandles(true);
                }, 15)).bind('dragend', function () {
                    if (hoveredHandle.length) {
                        var fromStart = originalHandle.hasClass(styles.taskDotStart);
                        var toStart = hoveredHandle.hasClass(styles.taskDotStart);
                        var type = fromStart ? toStart ? 3 : 2 : toStart ? 1 : 0;
                        var predecessor = that._taskByUid(originalHandle.siblings(DOT + styles.task).attr('data-uid'));
                        var successor = that._taskByUid(hoveredHandle.siblings(DOT + styles.task).attr('data-uid'));
                        if (predecessor !== successor) {
                            that.trigger('dependencyDragEnd', {
                                type: type,
                                predecessor: predecessor,
                                successor: successor
                            });
                        }
                    }
                    cleanUp();
                }).bind('dragcancel', function () {
                    cleanUp();
                }).userEvents.bind('select', function () {
                    blurActiveElement();
                });
            },
            _selectable: function () {
                var that = this;
                var styles = GanttTimeline.styles;
                if (this.options.selectable) {
                    this.wrapper.on(CLICK + NS, DOT + styles.task, function (e) {
                        e.stopPropagation();
                        if (!e.ctrlKey) {
                            that.trigger('select', { uid: $(this).attr('data-uid') });
                        } else {
                            that.trigger('clear');
                        }
                    }).on(CLICK + NS, DOT + styles.taskWrap, function (e) {
                        e.stopPropagation();
                        $(this).css('z-index', '0');
                        var target = $(document.elementFromPoint(e.clientX, e.clientY));
                        if (target.hasClass(styles.line)) {
                            target.click();
                        }
                        $(this).css('z-index', '');
                    }).on(CLICK + NS, DOT + styles.tasksWrapper, function () {
                        if (that.selectDependency().length > 0) {
                            that.clearSelection();
                        } else {
                            that.trigger('clear');
                        }
                    }).on(CLICK + NS, DOT + styles.line, function (e) {
                        e.stopPropagation();
                        that.selectDependency(this);
                    });
                }
            },
            select: function (value) {
                var element = this.wrapper.find(value);
                var styles = GanttTimeline.styles;
                if (element.length) {
                    this.clearSelection();
                    element.addClass(styles.selected);
                    if (kendo.support.mobileOS) {
                        element.parent().addClass(styles.taskWrapActive);
                    }
                    return;
                }
                return this.wrapper.find(DOT + styles.task + DOT + styles.selected);
            },
            selectDependency: function (value) {
                var element = this.wrapper.find(value);
                var uid;
                var styles = GanttTimeline.styles;
                if (element.length) {
                    this.clearSelection();
                    this.trigger('clear');
                    uid = $(element).attr('data-uid');
                    this.wrapper.find(DOT + styles.line + '[data-uid=\'' + uid + '\']').addClass(styles.selected);
                    return;
                }
                return this.wrapper.find(DOT + styles.line + DOT + styles.selected);
            },
            clearSelection: function () {
                var styles = GanttTimeline.styles;
                this.wrapper.find(DOT + styles.selected).removeClass(styles.selected);
                if (kendo.support.mobileOS) {
                    this.wrapper.find(DOT + styles.taskWrapActive).removeClass(styles.taskWrapActive);
                }
            },
            _attachEvents: function () {
                var that = this;
                var styles = GanttTimeline.styles;
                var editable = this.options.editable;
                if (editable) {
                    this._tabindex();
                    this.wrapper.on(CLICK + NS, DOT + styles.taskDelete, function (e) {
                        that.trigger('removeTask', { uid: $(this).closest(DOT + styles.task).attr('data-uid') });
                        e.stopPropagation();
                        e.preventDefault();
                    }).on(KEYDOWN + NS, function (e) {
                        var selectedDependency;
                        var editable = that.options.editable;
                        if (e.keyCode === keys.DELETE && editable && editable.dependencyDestroy !== false) {
                            selectedDependency = that.selectDependency();
                            if (selectedDependency.length) {
                                that.trigger('removeDependency', { uid: selectedDependency.attr('data-uid') });
                                that.clearSelection();
                            }
                        }
                    });
                    if (!kendo.support.mobileOS) {
                        this.wrapper.on(DBLCLICK + NS, DOT + styles.task, function (e) {
                            if (that.options.editable.update !== false) {
                                that.trigger('editTask', { uid: $(this).attr('data-uid') });
                                e.stopPropagation();
                                e.preventDefault();
                            }
                        });
                    } else {
                        this.touch = this.wrapper.kendoTouch({
                            filter: DOT + styles.task,
                            doubletap: function (e) {
                                if (that.options.editable.update !== false) {
                                    that.trigger('editTask', { uid: $(e.touch.currentTarget).attr('data-uid') });
                                }
                            }
                        }).data('kendoTouch');
                    }
                }
            },
            _tooltip: function () {
                var that = this;
                var tooltipOptions = this.options.tooltip;
                var styles = GanttTimeline.styles;
                var currentMousePosition;
                var mouseMoveHandler = function (e) {
                    currentMousePosition = e.clientX;
                };
                if (tooltipOptions && tooltipOptions.visible === false) {
                    return;
                }
                if (!kendo.support.mobileOS) {
                    this.wrapper.on(MOUSEENTER + NS, DOT + styles.task, function () {
                        var element = this;
                        var task = that._taskByUid($(this).attr('data-uid'));
                        if (that.dragInProgress) {
                            return;
                        }
                        that._tooltipTimeout = setTimeout(function () {
                            that.view()._createTaskTooltip(task, element, currentMousePosition);
                        }, 800);
                        $(this).on(MOUSEMOVE, mouseMoveHandler);
                    }).on(MOUSELEAVE + NS, DOT + styles.task, function () {
                        clearTimeout(that._tooltipTimeout);
                        that.view()._removeTaskTooltip();
                        $(this).off(MOUSEMOVE, mouseMoveHandler);
                    });
                } else {
                    this.wrapper.on(CLICK + NS, DOT + styles.taskDelete, function (e) {
                        e.stopPropagation();
                        that.view()._removeTaskTooltip();
                    }).on(MOUSELEAVE + NS, DOT + styles.task, function (e) {
                        var parents = $(e.relatedTarget).parents(DOT + styles.taskWrap, DOT + styles.task);
                        if (parents.length === 0) {
                            that.view()._removeTaskTooltip();
                        }
                    });
                    if (this.touch) {
                        this.touch.bind('tap', function (e) {
                            var element = e.touch.target;
                            var task = that._taskByUid($(element).attr('data-uid'));
                            var currentPosition = e.touch.x.client;
                            if (that.view()._taskTooltip) {
                                that.view()._removeTaskTooltip();
                            }
                            that.view()._createTaskTooltip(task, element, currentPosition);
                        }).bind('doubletap', function () {
                            that.view()._removeTaskTooltip();
                        });
                    }
                }
            }
        });
        extend(true, GanttTimeline, { styles: timelineStyles });
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));