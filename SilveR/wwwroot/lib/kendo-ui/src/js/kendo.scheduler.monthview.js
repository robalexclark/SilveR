/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('kendo.scheduler.monthview', ['kendo.scheduler.view'], f);
}(function () {
    var __meta__ = {
        id: 'scheduler.monthview',
        name: 'Scheduler Month View',
        category: 'web',
        description: 'The Scheduler Month View',
        depends: ['scheduler.view'],
        hidden: true
    };
    (function ($) {
        var kendo = window.kendo, ui = kendo.ui, SchedulerView = ui.SchedulerView, NS = '.kendoMonthView', extend = $.extend, getDate = kendo.date.getDate, MS_PER_DAY = kendo.date.MS_PER_DAY, NUMBER_OF_ROWS = 6, NUMBER_OF_COLUMNS = 7, DAY_TEMPLATE = kendo.template('<span class="k-link k-nav-day">#:kendo.toString(date, "dd")#</span>'), EVENT_WRAPPER_STRING = '<div role="gridcell" aria-selected="false" data-#=ns#uid="#=uid#"' + '#if (resources[0]) { #' + 'style="background-color:#=resources[0].color #; border-color: #=resources[0].color#"' + 'class="k-event#=inverseColor ? " k-event-inverse" : ""#"' + '#} else {#' + 'class="k-event"' + '#}#' + '>' + '<span class="k-event-actions">' + '# if(data.tail || data.middle) {#' + '<span class="k-icon k-i-arrow-60-left"></span>' + '#}#' + '# if(data.isException()) {#' + '<span class="k-icon k-i-non-recurrence"></span>' + '# } else if(data.isRecurring()) {#' + '<span class="k-icon k-i-reload"></span>' + '#}#' + '</span>' + '{0}' + '<span class="k-event-actions">' + '#if (showDelete) {#' + '<a href="\\#" class="k-link k-event-delete" title="${data.messages.destroy}" aria-label="${data.messages.destroy}"><span class="k-icon k-i-close"></span></a>' + '#}#' + '# if(data.head || data.middle) {#' + '<span class="k-icon k-i-arrow-60-right"></span>' + '#}#' + '</span>' + '# if(resizable && !data.tail && !data.middle) {#' + '<span class="k-resize-handle k-resize-w"></span>' + '#}#' + '# if(resizable && !data.head && !data.middle) {#' + '<span class="k-resize-handle k-resize-e"></span>' + '#}#' + '</div>', EVENT_TEMPLATE = kendo.template('<div title="#=title.replace(/"/g,"&\\#34;")#">' + '<div class="k-event-template">#:title#</div>' + '</div>');
        var MORE_BUTTON_TEMPLATE = kendo.template('<div style="width:#=width#px;left:#=left#px;top:#=top#px" class="k-more-events k-button"><span>...</span></div>');
        var MonthGroupedView = kendo.Class.extend({
            init: function (view) {
                this._view = view;
            },
            _verticalRowCountForLevel: function (level) {
                var view = this._view;
                return view._rowCountForLevel(level);
            },
            _horizontalGroupCountForLevel: function (level) {
                var view = this._view;
                return view._columnCountForLevel(level);
            },
            _getCalendarRowsLength: function (cellsPerRow, cellCount) {
                return cellCount / cellsPerRow;
            },
            _createRows: function (start, startIdx, horizontalGroupCount, verticalGroupIndex) {
                var view = this._view;
                var cellsPerRow = NUMBER_OF_COLUMNS;
                var isVerticallyGrouped = view._isVerticallyGrouped();
                var html = '';
                for (var groupIdx = 0; groupIdx < horizontalGroupCount; groupIdx++) {
                    html += view._createRow(start, startIdx, cellsPerRow, isVerticallyGrouped ? verticalGroupIndex : groupIdx);
                }
                return html;
            },
            _adjustStartDate: function (start) {
                return kendo.date.addDays(start, NUMBER_OF_COLUMNS);
            },
            _getContent: function (content, startDate, resources) {
                return content({
                    date: startDate,
                    resources: resources
                });
            },
            _getTimeSlotByPosition: function (x, y, groupIndex) {
                var group = this._view.groups[groupIndex];
                return group.daySlotByPosition(x, y);
            },
            _nextSlotStartDate: function (startDate) {
                return kendo.date.nextDay(startDate);
            },
            _createRowsLayout: function (resources, rows, groupHeaderTemplate) {
                var view = this._view;
                return view._createRowsLayout(resources, rows, groupHeaderTemplate);
            },
            _createVerticalColumnsLayout: function (resources, rows, groupHeaderTemplate, columns) {
                return columns;
            },
            _createColumnsLayout: function (resources, columns, groupHeaderTemplate) {
                var view = this._view;
                return view._createColumnsLayout(resources, columns, groupHeaderTemplate);
            },
            _verticalGroupCount: function (level) {
                var view = this._view;
                return view._rowCountForLevel(level);
            },
            _horizontalGroupCount: function (level) {
                var view = this._view;
                return view._columnCountForLevel(level) / view._columnOffsetForResource(level);
            },
            _positionEvent: function (event, group, range, rangeCount, start, end, rangeIndex) {
                var view = this._view;
                var isMobilePhoneView = view._isMobilePhoneView();
                if (rangeCount > 1) {
                    if (rangeIndex === 0) {
                        end = range.end.endDate();
                    } else if (rangeIndex == rangeCount - 1) {
                        start = range.start.startDate();
                    } else {
                        start = range.start.startDate();
                        end = range.end.endDate();
                    }
                }
                var occurrence = event.clone({
                    start: start,
                    end: end,
                    head: range.head,
                    tail: range.tail
                });
                if (isMobilePhoneView) {
                    view._positionMobileEvent(range, view._createEventElement(occurrence), group);
                } else {
                    view._positionEvent(range, view._createEventElement(occurrence), group);
                }
            },
            _addDaySlotCollections: function (groupCount, tableRows, startDate) {
                var view = this._view;
                var columnCount = NUMBER_OF_COLUMNS;
                var rowCount = NUMBER_OF_ROWS;
                for (var groupIndex = 0; groupIndex < groupCount; groupIndex++) {
                    var cellCount = 0;
                    var rowMultiplier = 0;
                    if (view._isVerticallyGrouped()) {
                        rowMultiplier = groupIndex;
                    }
                    for (var rowIndex = rowMultiplier * rowCount; rowIndex < (rowMultiplier + 1) * rowCount; rowIndex++) {
                        var group = view.groups[groupIndex];
                        var collection = group.addDaySlotCollection(kendo.date.addDays(startDate, cellCount), kendo.date.addDays(startDate, cellCount + columnCount));
                        var tableRow = tableRows[rowIndex];
                        var cells = tableRow.children;
                        var cellMultiplier = 0;
                        tableRow.setAttribute('role', 'row');
                        if (!view._isVerticallyGrouped()) {
                            cellMultiplier = groupIndex;
                        }
                        for (var cellIndex = cellMultiplier * columnCount; cellIndex < (cellMultiplier + 1) * columnCount; cellIndex++) {
                            var cell = cells[cellIndex];
                            view.addDaySlot(collection, cell, startDate, cellCount);
                            cellCount++;
                        }
                    }
                }
            },
            _changePeriodGroupIndex: function (reverse) {
                var view = this._view;
                return reverse ? view.groups.length - 1 : 0;
            },
            _createResizeHint: function (range) {
                var view = this._view;
                var left = range.startSlot().offsetLeft;
                var top = range.start.offsetTop;
                var width = range.innerWidth();
                var height = range.start.clientHeight - 2;
                var hint = SchedulerView.fn._createResizeHint.call(view, left, top, width, height);
                view._appendResizeHint(hint);
            },
            _createMoveHint: function (range, event) {
                var view = this._view;
                var startSlot = range.startSlot();
                var endSlot = range.endSlot();
                var hint = view._createEventElement(event.clone({
                    head: range.head,
                    tail: range.tail
                }));
                hint.css({
                    left: startSlot.offsetLeft + 2,
                    top: startSlot.offsetTop + startSlot.firstChildHeight,
                    height: view.options.eventHeight,
                    width: range.innerWidth() - (startSlot.index !== endSlot.index ? 5 : 4)
                });
                hint.addClass('k-event-drag-hint');
                view._appendMoveHint(hint);
            }
        });
        var MonthGroupedByDateView = kendo.Class.extend({
            init: function (view) {
                this._view = view;
            },
            _verticalRowCountForLevel: function () {
                return 1;
            },
            _horizontalGroupCountForLevel: function (level) {
                var view = this._view;
                return view._columnCountForLevel(level + 1) / NUMBER_OF_COLUMNS;
            },
            _createRows: function (start, startIdx, horizontalGroupCount) {
                var view = this._view;
                var cellsPerRow = NUMBER_OF_COLUMNS;
                var isVerticallyGrouped = view._isVerticallyGrouped();
                var html = '';
                var dateIdx = 0;
                if (isVerticallyGrouped) {
                    var verticalStart = new Date(start);
                    var groupCount = view._groupCount();
                    for (dateIdx; dateIdx < NUMBER_OF_ROWS; dateIdx++) {
                        html += view._createRow(verticalStart, startIdx, groupCount, dateIdx);
                        verticalStart = kendo.date.addDays(verticalStart, cellsPerRow);
                    }
                    start = kendo.date.nextDay(start);
                } else {
                    for (dateIdx; dateIdx < cellsPerRow; dateIdx++) {
                        html += view._createRow(start, startIdx, horizontalGroupCount, dateIdx);
                        start = kendo.date.nextDay(start);
                    }
                    start = kendo.date.addDays(start, cellsPerRow);
                }
                return html;
            },
            _adjustStartDate: function (start, isLastRow) {
                var view = this._view;
                var isVerticallyGrouped = view._isVerticallyGrouped();
                if (isVerticallyGrouped) {
                    if (isLastRow) {
                        return kendo.date.addDays(start, NUMBER_OF_COLUMNS * (NUMBER_OF_ROWS - 1) + 1);
                    } else {
                        return kendo.date.nextDay(start);
                    }
                }
                return kendo.date.addDays(start, NUMBER_OF_COLUMNS);
            },
            _getContent: function (content, startDate, resources, cellIdx) {
                if (cellIdx === 0) {
                    return content({
                        date: startDate,
                        resources: resources
                    });
                }
                return '';
            },
            _getTimeSlotByPosition: function (x, y, groupIndex) {
                var group = this._view.groups[groupIndex];
                return group.daySlotByPosition(x, y, true);
            },
            _nextSlotStartDate: function (startDate) {
                return startDate;
            },
            _getCalendarRowsLength: function () {
                var view = this._view;
                var isVerticallyGrouped = view._isVerticallyGrouped();
                return isVerticallyGrouped ? NUMBER_OF_COLUMNS : NUMBER_OF_ROWS;
            },
            _createRowsLayout: function (resources, rows, groupHeaderTemplate, columns) {
                var view = this._view;
                return view._createDateLayout(columns, null, false);
            },
            _createVerticalColumnsLayout: function (resources, rows, groupHeaderTemplate) {
                var view = this._view;
                var resource = resources[0];
                var configuration = [];
                var data = resource.dataSource.view();
                for (var dataIndex = 0; dataIndex < data.length * NUMBER_OF_ROWS; dataIndex++) {
                    var obj = {
                        text: groupHeaderTemplate({
                            text: kendo.htmlEncode(kendo.getter(resource.dataTextField)(data[dataIndex % data.length])),
                            color: kendo.getter(resource.dataColorField)(data[dataIndex % data.length]),
                            field: resource.field,
                            title: resource.title,
                            name: resource.name,
                            value: kendo.getter(resource.dataValueField)(data[dataIndex % data.length])
                        }),
                        className: 'k-slot-cell'
                    };
                    obj.columns = view._createColumnsLayout(resources.slice(1), null, groupHeaderTemplate);
                    configuration.push(obj);
                }
                return configuration;
            },
            _createColumnsLayout: function (resources, columns, groupHeaderTemplate, subColumns) {
                var view = this._view;
                return view._createColumnsLayout(resources, columns, groupHeaderTemplate, subColumns, true);
            },
            _verticalGroupCount: function (level) {
                var view = this._view;
                return view._columnCountForLevel(level) / NUMBER_OF_ROWS;
            },
            _horizontalGroupCount: function (level) {
                var view = this._view;
                return view._columnCountForLevel(level) / NUMBER_OF_COLUMNS;
            },
            _positionEvent: function (event, group, range, rangeCount, start, end) {
                var view = this._view;
                var startIndex = range.start.index;
                var endIndex = range.end.index;
                var isMobilePhoneView = view._isMobilePhoneView();
                for (var i = range.start.index; i <= range.end.index; i++) {
                    var currentSlot = range.collection._slots[i];
                    var dateRange = group.daySlotRanges(currentSlot.start, currentSlot.start, true)[0];
                    var occurrence = event.clone({
                        start: i === startIndex ? start : currentSlot.startDate(),
                        end: i === endIndex ? end : currentSlot.endDate(),
                        head: i !== endIndex || range.head,
                        tail: i !== startIndex || range.tail
                    });
                    if (isMobilePhoneView) {
                        view._positionMobileEvent(dateRange, view._createEventElement(occurrence), group);
                    } else {
                        view._positionEvent(dateRange, view._createEventElement(occurrence), group);
                    }
                }
            },
            _addDaySlotCollections: function (groupCount, tableRows, startDate) {
                var view = this._view;
                var columnCount = NUMBER_OF_COLUMNS;
                var rowCount = NUMBER_OF_ROWS;
                var isVerticallyGrouped = view._isVerticallyGrouped();
                for (var dateIndex = 0; dateIndex < columnCount; dateIndex++) {
                    for (var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                        var groupIndex = 0;
                        var currentTableIndex = isVerticallyGrouped ? dateIndex : rowIndex;
                        var tableRow = tableRows[currentTableIndex];
                        var cells = tableRow.children;
                        var cellMultiplier = 0;
                        tableRow.setAttribute('role', 'row');
                        if (!view._isVerticallyGrouped()) {
                            cellMultiplier = dateIndex;
                        }
                        for (var cellIndex = cellMultiplier * groupCount; cellIndex < (cellMultiplier + 1) * groupCount; cellIndex++) {
                            var cellCount = rowIndex * columnCount + dateIndex;
                            var currentCellIndex = isVerticallyGrouped ? cellIndex + rowIndex * groupCount : cellIndex;
                            var cell = cells[currentCellIndex];
                            var currentGroupIndex = isVerticallyGrouped ? cellIndex : groupIndex;
                            var group = view.groups[currentGroupIndex];
                            var collection;
                            if (dateIndex === 0) {
                                collection = group.addDaySlotCollection(kendo.date.addDays(startDate, cellCount), kendo.date.addDays(startDate, cellCount + columnCount));
                            } else {
                                collection = group._daySlotCollections[rowIndex];
                            }
                            view.addDaySlot(collection, cell, startDate, cellCount);
                            groupIndex++;
                        }
                    }
                }
            },
            _changePeriodGroupIndex: function (reverse, vertical, selectionGroupIndex) {
                var view = this._view;
                if (vertical && view._isVerticallyGrouped()) {
                    return reverse ? view.groups.length - 1 : 0;
                }
                return selectionGroupIndex;
            },
            _createResizeHint: function (range) {
                var view = this._view;
                var left, top, width, height, hint;
                if (view._isVerticallyGrouped()) {
                    left = range.startSlot().offsetLeft;
                    top = range.start.offsetTop;
                    width = range.startSlot().offsetWidth;
                    height = range.endSlot().offsetTop + range.startSlot().offsetHeight - range.startSlot().offsetTop - 2;
                    hint = SchedulerView.fn._createResizeHint.call(view, left, top, width, height);
                    view._appendResizeHint(hint);
                } else {
                    for (var slotIdx = range.startSlot().index; slotIdx <= range.endSlot().index; slotIdx++) {
                        var slot = range.collection._slots[slotIdx];
                        left = slot.offsetLeft;
                        top = slot.offsetTop;
                        width = slot.offsetWidth;
                        height = slot.offsetHeight - 2;
                        hint = SchedulerView.fn._createResizeHint.call(view, left, top, width, height);
                        view._appendResizeHint(hint);
                    }
                }
            },
            _createMoveHint: function (range, event) {
                var view = this._view;
                var startSlot = range.startSlot();
                var endSlot = range.endSlot();
                for (var slotIdx = startSlot.index; slotIdx <= endSlot.index; slotIdx++) {
                    var slot = range.collection._slots[slotIdx];
                    var hint = view._createEventElement(event.clone({
                        head: range.head,
                        tail: range.tail
                    }));
                    hint.css({
                        left: slot.offsetLeft,
                        top: slot.offsetTop + slot.firstChildHeight,
                        height: view.options.eventHeight,
                        width: slot.offsetWidth - 2
                    });
                    hint.addClass('k-event-drag-hint');
                    view._appendMoveHint(hint);
                }
            }
        });
        kendo.ui.scheduler.MonthGroupedView = MonthGroupedView;
        kendo.ui.scheduler.MonthGroupedByDateView = MonthGroupedByDateView;
        ui.MonthView = SchedulerView.extend({
            init: function (element, options) {
                var that = this;
                SchedulerView.fn.init.call(that, element, options);
                that._groupedView = that._getGroupedView();
                that.title = that.options.title;
                that._templates();
                that._editable();
                that._renderLayout(that.options.date);
                that._groups();
            },
            name: 'month',
            _getGroupedView: function () {
                if (this._isGroupedByDate()) {
                    return new kendo.ui.scheduler.MonthGroupedByDateView(this);
                } else {
                    return new kendo.ui.scheduler.MonthGroupedView(this);
                }
            },
            _updateDirection: function (selection, ranges, multiple, reverse, vertical) {
                if (multiple) {
                    var startSlot = ranges[0].start;
                    var endSlot = ranges[ranges.length - 1].end;
                    var isSameSlot = startSlot.index === endSlot.index;
                    var isSameCollection = startSlot.collectionIndex === endSlot.collectionIndex;
                    var updateDirection;
                    if (vertical) {
                        updateDirection = isSameSlot && isSameCollection || isSameCollection;
                    } else {
                        updateDirection = isSameSlot && isSameCollection;
                    }
                    if (updateDirection) {
                        selection.backward = reverse;
                    }
                }
            },
            _changeDate: function (selection, slot, previous) {
                var group = this.groups[selection.groupIndex];
                var collections, index;
                if (previous) {
                    collections = group._getCollections(group.daySlotCollectionCount());
                    index = slot.collectionIndex - 1;
                    if (index >= 0) {
                        return collections[index]._slots[collections[index]._slots.length - 1];
                    }
                } else {
                    collections = group._getCollections(group.daySlotCollectionCount());
                    index = slot.collectionIndex + 1;
                    var slotIndex = 0;
                    if (collections[index] && collections[index]._slots[slotIndex]) {
                        return collections[index]._slots[slotIndex];
                    }
                }
            },
            _getNextHorizontalRange: function (group, method, horizontalRange) {
                var isVertical = this._isVerticallyGrouped();
                horizontalRange.startSlot = group[method](horizontalRange.startSlot, isVertical);
                horizontalRange.endSlot = group[method](horizontalRange.endSlot, isVertical);
                return horizontalRange;
            },
            _getNextVerticalRange: function (group, method, verticalRange, multiple) {
                var isVertical = this._isVerticallyGrouped() && this._isGroupedByDate();
                verticalRange.startSlot = group[method](verticalRange.startSlot, multiple, isVertical);
                verticalRange.endSlot = group[method](verticalRange.endSlot, multiple, isVertical);
                return verticalRange;
            },
            _changeViewPeriod: function (selection, reverse, vertical) {
                var pad = vertical ? 7 : 1;
                var newStart, newEnd;
                if (reverse) {
                    pad *= -1;
                }
                newStart = kendo.date.addDays(selection.start, pad);
                newEnd = kendo.date.addDays(selection.end, pad);
                if (this._isInRange(newStart, newEnd)) {
                    return false;
                }
                selection.start = newStart;
                selection.end = newEnd;
                if (!vertical || vertical && this._isVerticallyGrouped()) {
                    selection.groupIndex = this._groupedView._changePeriodGroupIndex(reverse, vertical, selection.groupIndex);
                }
                selection.events = [];
                return true;
            },
            _continuousSlot: function (selection, ranges, reverse) {
                var index = selection.backward ? 0 : ranges.length - 1;
                var group = this.groups[selection.groupIndex];
                return group.continuousSlot(ranges[index].start, reverse);
            },
            _changeGroupContinuously: function (selection, continuousSlot, multiple, reverse) {
                if (!multiple) {
                    var groupIndex = selection.groupIndex;
                    var lastGroupIndex = this.groups.length - 1;
                    var vertical = this._isVerticallyGrouped();
                    var group = this.groups[groupIndex];
                    if (!continuousSlot && vertical) {
                        continuousSlot = group[reverse ? 'lastSlot' : 'firstSlot']();
                        groupIndex += reverse ? -1 : 1;
                    } else if (continuousSlot && !vertical) {
                        groupIndex = reverse ? lastGroupIndex : 0;
                    }
                    if (groupIndex < 0 || groupIndex > lastGroupIndex) {
                        groupIndex = reverse ? lastGroupIndex : 0;
                        continuousSlot = null;
                    }
                    selection.groupIndex = groupIndex;
                }
                return continuousSlot;
            },
            _normalizeHorizontalSelection: function (selection, ranges, reverse) {
                var slot;
                if (reverse) {
                    slot = ranges[0].start;
                } else {
                    slot = ranges[ranges.length - 1].end;
                }
                return slot;
            },
            _normalizeVerticalSelection: function (selection, ranges) {
                var slot;
                if (selection.backward) {
                    slot = ranges[0].start;
                } else {
                    slot = ranges[ranges.length - 1].end;
                }
                return slot;
            },
            _templates: function () {
                var options = this.options, settings = extend({}, kendo.Template, options.templateSettings);
                this.eventTemplate = this._eventTmpl(options.eventTemplate, EVENT_WRAPPER_STRING);
                this.dayTemplate = kendo.template(options.dayTemplate, settings);
                this.groupHeaderTemplate = kendo.template(options.groupHeaderTemplate, settings);
            },
            dateForTitle: function () {
                return kendo.format(this.options.selectedDateFormat, this._firstDayOfMonth, this._lastDayOfMonth);
            },
            shortDateForTitle: function () {
                return kendo.format(this.options.selectedShortDateFormat, this._firstDayOfMonth, this._lastDayOfMonth);
            },
            nextDate: function () {
                return kendo.date.nextDay(this._lastDayOfMonth);
            },
            previousDate: function () {
                return kendo.date.previousDay(this._firstDayOfMonth);
            },
            startDate: function () {
                return this._startDate;
            },
            endDate: function () {
                return this._endDate;
            },
            _renderLayout: function (date) {
                var that = this;
                this._firstDayOfMonth = kendo.date.firstDayOfMonth(date);
                this._lastDayOfMonth = kendo.date.lastDayOfMonth(date);
                this._startDate = firstVisibleMonthDay(date, this.calendarInfo());
                this.createLayout(this._layout());
                this._content();
                this.refreshLayout();
                this.content.on('click' + NS, '.k-nav-day,.k-more-events', function (e) {
                    var offset = $(e.currentTarget).offset();
                    var slot = that._slotByPosition(offset.left, offset.top);
                    e.preventDefault();
                    that.trigger('navigate', {
                        view: 'day',
                        date: slot.startDate()
                    });
                });
            },
            _editable: function () {
                if (this.options.editable && !this._isMobilePhoneView()) {
                    if (this._isMobile()) {
                        this._touchEditable();
                    } else {
                        this._mouseEditable();
                    }
                }
            },
            _mouseEditable: function () {
                var that = this;
                that.element.on('click' + NS, '.k-scheduler-monthview .k-event a:has(.k-i-close)', function (e) {
                    that.trigger('remove', { uid: $(this).closest('.k-event').attr(kendo.attr('uid')) });
                    e.preventDefault();
                });
                if (that.options.editable.create !== false) {
                    that.element.on('dblclick' + NS, '.k-scheduler-monthview .k-scheduler-content td', function (e) {
                        var offset = $(e.currentTarget).offset();
                        var slot = that._slotByPosition(offset.left, offset.top);
                        if (slot) {
                            var resourceInfo = that._resourceBySlot(slot);
                            that.trigger('add', {
                                eventInfo: extend({
                                    isAllDay: true,
                                    start: slot.startDate(),
                                    end: slot.startDate()
                                }, resourceInfo)
                            });
                        }
                        e.preventDefault();
                    });
                }
                if (that.options.editable.update !== false) {
                    that.element.on('dblclick' + NS, '.k-scheduler-monthview .k-event', function (e) {
                        that.trigger('edit', { uid: $(this).closest('.k-event').attr(kendo.attr('uid')) });
                        e.preventDefault();
                    });
                }
            },
            _touchEditable: function () {
                var that = this;
                var threshold = 0;
                if (kendo.support.mobileOS.android) {
                    threshold = 5;
                }
                if (that.options.editable.create !== false) {
                    that._addUserEvents = new kendo.UserEvents(that.element, {
                        threshold: threshold,
                        useClickAsTap: !kendo.support.browser.edge,
                        filter: '.k-scheduler-monthview .k-scheduler-content td',
                        tap: function (e) {
                            if (that._scrolling) {
                                return;
                            }
                            var offset = $(e.target).offset();
                            var slot = that._slotByPosition(offset.left, offset.top);
                            if (slot) {
                                var resourceInfo = that._resourceBySlot(slot);
                                that.trigger('add', {
                                    eventInfo: extend({
                                        isAllDay: true,
                                        start: slot.startDate(),
                                        end: slot.startDate()
                                    }, resourceInfo)
                                });
                            }
                            e.preventDefault();
                        }
                    });
                }
                if (that.options.editable.update !== false) {
                    that._editUserEvents = new kendo.UserEvents(that.element, {
                        threshold: threshold,
                        filter: '.k-scheduler-monthview .k-event',
                        useClickAsTap: !kendo.support.browser.edge,
                        tap: function (e) {
                            if (that._scrolling) {
                                return;
                            }
                            if ($(e.event.target).closest('a:has(.k-i-close)').length === 0) {
                                that.trigger('edit', { uid: $(e.target).closest('.k-event').attr(kendo.attr('uid')) });
                                e.preventDefault();
                            }
                        }
                    });
                }
            },
            selectionByElement: function (cell) {
                var offset = $(cell).offset();
                return this._slotByPosition(offset.left, offset.top);
            },
            _columnCountForLevel: function (level) {
                var columnLevel = this.columnLevels[level];
                return columnLevel ? columnLevel.length : 0;
            },
            _rowCountForLevel: function (level) {
                var rowLevel = this.rowLevels[level];
                return rowLevel ? rowLevel.length : 0;
            },
            _content: function () {
                var html = '<tbody>';
                var verticalGroupCount = 1;
                var groupedView = this._groupedView;
                var resources = this.groupedResources;
                if (resources.length) {
                    if (this._isVerticallyGrouped()) {
                        verticalGroupCount = groupedView._verticalRowCountForLevel(resources.length - 1);
                    }
                }
                for (var verticalGroupIdx = 0; verticalGroupIdx < verticalGroupCount; verticalGroupIdx++) {
                    html += this._createCalendar(verticalGroupIdx);
                }
                html += '</tbody>';
                this.content.find('table').html(html);
            },
            _createCalendar: function (verticalGroupIndex) {
                var start = this.startDate();
                var cellCount = NUMBER_OF_COLUMNS * NUMBER_OF_ROWS;
                var cellsPerRow = NUMBER_OF_COLUMNS;
                var weekStartDates = [start];
                var html = '';
                var horizontalGroupCount = 1;
                var isVerticallyGrouped = this._isVerticallyGrouped();
                var groupedView = this._groupedView;
                var resources = this.groupedResources;
                if (resources.length) {
                    if (!isVerticallyGrouped) {
                        horizontalGroupCount = groupedView._horizontalGroupCountForLevel(resources.length - 1);
                    }
                }
                this._slotIndices = {};
                var calendarRowsLength = groupedView._getCalendarRowsLength(cellsPerRow, cellCount);
                for (var rowIdx = 0; rowIdx < calendarRowsLength; rowIdx++) {
                    html += '<tr>';
                    weekStartDates.push(start);
                    var startIdx = rowIdx * cellsPerRow;
                    html += groupedView._createRows(start, startIdx, horizontalGroupCount, verticalGroupIndex);
                    start = groupedView._adjustStartDate(start, rowIdx === calendarRowsLength - 1);
                    html += '</tr>';
                }
                this._weekStartDates = weekStartDates;
                this._endDate = kendo.date.previousDay(start);
                return html;
            },
            _createRow: function (startDate, startIdx, cellsPerRow, groupIndex) {
                var that = this;
                var min = that._firstDayOfMonth;
                var max = that._lastDayOfMonth;
                var content = that.dayTemplate;
                var classes = '';
                var html = '';
                var groupedView = this._groupedView;
                var resources = function () {
                    return that._resourceBySlot({ groupIndex: groupIndex });
                };
                for (var cellIdx = 0; cellIdx < cellsPerRow; cellIdx++) {
                    classes = '';
                    if (kendo.date.isToday(startDate)) {
                        classes += 'k-today';
                    }
                    if (!kendo.date.isInDateRange(startDate, min, max)) {
                        classes += ' k-other-month';
                    }
                    html += '<td ';
                    if (classes !== '') {
                        html += 'class="' + classes + '"';
                    }
                    html += '>';
                    html += groupedView._getContent(content, startDate, resources, cellIdx);
                    html += '</td>';
                    that._slotIndices[getDate(startDate).getTime()] = startIdx + cellIdx;
                    startDate = groupedView._nextSlotStartDate(startDate);
                }
                return html;
            },
            _layout: function () {
                var calendarInfo = this.calendarInfo();
                var weekDayNames = this._isMobile() ? calendarInfo.days.namesShort : calendarInfo.days.names;
                var names = shiftArray(weekDayNames, calendarInfo.firstDay);
                var columns = $.map(names, function (value) {
                    return { text: value };
                });
                var resources = this.groupedResources;
                var rows;
                var groupedView = this._groupedView;
                if (resources.length) {
                    if (this._isVerticallyGrouped()) {
                        var inner = [];
                        for (var idx = 0; idx < 6; idx++) {
                            inner.push({
                                text: '<div>&nbsp;</div>',
                                className: 'k-hidden k-slot-cell'
                            });
                        }
                        rows = groupedView._createRowsLayout(resources, inner, this.groupHeaderTemplate, columns);
                        columns = groupedView._createVerticalColumnsLayout(resources, inner, this.groupHeaderTemplate, columns);
                    } else {
                        columns = groupedView._createColumnsLayout(resources, columns, this.groupHeaderTemplate, columns);
                    }
                }
                return {
                    columns: columns,
                    rows: rows
                };
            },
            _createEventElement: function (event) {
                var options = this.options;
                var editable = options.editable;
                var isMobile = this._isMobile();
                event.showDelete = editable && editable.destroy !== false && !isMobile;
                event.resizable = editable && editable.resize !== false && !isMobile;
                event.ns = kendo.ns;
                event.resources = this.eventResources(event);
                event.inverseColor = event.resources && event.resources[0] ? this._shouldInverseResourceColor(event.resources[0]) : false;
                event.messages = options.messages || { destroy: 'Delete' };
                var element = $(this.eventTemplate(event));
                this.angular('compile', function () {
                    return {
                        elements: element,
                        data: [{ dataItem: event }]
                    };
                });
                return element;
            },
            _isInDateSlot: function (event) {
                var groups = this.groups[0];
                var slotStart = groups.firstSlot().start;
                var slotEnd = groups.lastSlot().end - 1;
                var startTime = kendo.date.toUtcTime(event.start);
                var endTime = kendo.date.toUtcTime(event.end);
                return (isInDateRange(startTime, slotStart, slotEnd) || isInDateRange(endTime, slotStart, slotEnd) || isInDateRange(slotStart, startTime, endTime) || isInDateRange(slotEnd, startTime, endTime)) && (!isInDateRange(endTime, slotStart, slotStart) || isInDateRange(endTime, startTime, startTime) || event.isAllDay);
            },
            _slotIndex: function (date) {
                return this._slotIndices[getDate(date).getTime()];
            },
            _positionMobileEvent: function (slotRange, element, group) {
                var startSlot = slotRange.start;
                if (slotRange.start.offsetLeft > slotRange.end.offsetLeft) {
                    startSlot = slotRange.end;
                }
                var startIndex = slotRange.start.index;
                var endIndex = startIndex;
                var eventCount = 3;
                var events = SchedulerView.collidingEvents(slotRange.events(), startIndex, endIndex);
                events.push({
                    element: element,
                    start: startIndex,
                    end: endIndex
                });
                var rows = SchedulerView.createRows(events);
                var slot = slotRange.collection.at(startIndex);
                var container = slot.container;
                if (!container) {
                    container = $(kendo.format('<div class="k-events-container" style="top:{0};left:{1};width:{2}"/>', startSlot.offsetTop + startSlot.firstChildTop + startSlot.firstChildHeight + 'px', startSlot.offsetLeft + 'px', startSlot.offsetWidth + 'px'));
                    slot.container = container;
                    this.content[0].appendChild(container[0]);
                }
                if (rows.length <= eventCount) {
                    slotRange.addEvent({
                        element: element,
                        start: startIndex,
                        end: endIndex,
                        groupIndex: startSlot.groupIndex
                    });
                    group._continuousEvents.push({
                        element: element,
                        uid: element.attr(kendo.attr('uid')),
                        start: slotRange.start,
                        end: slotRange.end
                    });
                    container[0].appendChild(element[0]);
                }
            },
            _positionEvent: function (slotRange, element, group) {
                var eventHeight = this.options.eventHeight;
                var startSlot = slotRange.start;
                if (slotRange.start.offsetLeft > slotRange.end.offsetLeft) {
                    startSlot = slotRange.end;
                }
                var startIndex = slotRange.start.index;
                var endIndex = slotRange.end.index;
                var eventCount = startSlot.eventCount;
                var events = SchedulerView.collidingEvents(slotRange.events(), startIndex, endIndex);
                var rightOffset = startIndex !== endIndex ? 5 : 4;
                events.push({
                    element: element,
                    start: startIndex,
                    end: endIndex
                });
                var rows = SchedulerView.createRows(events);
                for (var idx = 0, length = Math.min(rows.length, eventCount); idx < length; idx++) {
                    var rowEvents = rows[idx].events;
                    var eventTop = startSlot.offsetTop + startSlot.firstChildTop + startSlot.firstChildHeight + idx * eventHeight + 3 * idx + 'px';
                    for (var j = 0, eventLength = rowEvents.length; j < eventLength; j++) {
                        rowEvents[j].element[0].style.top = eventTop;
                    }
                }
                if (rows.length > eventCount) {
                    for (var slotIndex = startIndex; slotIndex <= endIndex; slotIndex++) {
                        var collection = slotRange.collection;
                        var slot = collection.at(slotIndex);
                        if (slot.more) {
                            continue;
                        }
                        slot.more = $(MORE_BUTTON_TEMPLATE({
                            ns: kendo.ns,
                            start: slotIndex,
                            end: slotIndex,
                            width: slot.clientWidth - 2,
                            left: slot.offsetLeft + 2,
                            top: slot.offsetTop + slot.firstChildTop + slot.firstChildHeight + eventCount * eventHeight + 3 * eventCount
                        }));
                        this.content[0].appendChild(slot.more[0]);
                    }
                } else {
                    slotRange.addEvent({
                        element: element,
                        start: startIndex,
                        end: endIndex,
                        groupIndex: startSlot.groupIndex
                    });
                    element[0].style.width = slotRange.innerWidth() - rightOffset + 'px';
                    element[0].style.left = startSlot.offsetLeft + 2 + 'px';
                    element[0].style.height = eventHeight + 'px';
                    group._continuousEvents.push({
                        element: element,
                        uid: element.attr(kendo.attr('uid')),
                        start: slotRange.start,
                        end: slotRange.end
                    });
                    element.appendTo(this.content);
                }
            },
            _slotByPosition: function (x, y) {
                var offset = this.content.offset();
                x -= offset.left;
                y -= offset.top;
                y += this.content[0].scrollTop;
                x += this.content[0].scrollLeft;
                x = Math.ceil(x);
                y = Math.ceil(y);
                for (var groupIndex = 0; groupIndex < this.groups.length; groupIndex++) {
                    var slot = this._groupedView._getTimeSlotByPosition(x, y, groupIndex);
                    if (slot) {
                        return slot;
                    }
                }
                return null;
            },
            _appendResizeHint: function (hint) {
                hint.appendTo(this.content);
                this._resizeHint = this._resizeHint.add(hint);
            },
            _updateResizeHint: function (event, groupIndex, startTime, endTime) {
                this._removeResizeHint();
                var group = this.groups[groupIndex];
                var ranges = group.ranges(startTime, endTime, true, event.isAllDay);
                for (var rangeIndex = 0; rangeIndex < ranges.length; rangeIndex++) {
                    this._groupedView._createResizeHint(ranges[rangeIndex]);
                }
                this._resizeHint.find('.k-label-top,.k-label-bottom').text('');
                this._resizeHint.first().addClass('k-first').find('.k-label-top').text(kendo.toString(kendo.timezone.toLocalDate(startTime), 'M/dd'));
                this._resizeHint.last().addClass('k-last').find('.k-label-bottom').text(kendo.toString(kendo.timezone.toLocalDate(endTime), 'M/dd'));
            },
            _updateMoveHint: function (event, groupIndex, distance) {
                var start = kendo.date.toUtcTime(event.start) + distance;
                var end = start + event.duration();
                var group = this.groups[groupIndex];
                var ranges = group.ranges(start, end, true, event.isAllDay);
                this._removeMoveHint(event.uid);
                for (var rangeIndex = 0; rangeIndex < ranges.length; rangeIndex++) {
                    this._groupedView._createMoveHint(ranges[rangeIndex], event);
                }
            },
            _appendMoveHint: function (hint) {
                hint.appendTo(this.content);
                this._moveHint = this._moveHint.add(hint);
            },
            _groups: function () {
                var groupCount = this._groupCount();
                var tableRows = this.content[0].getElementsByTagName('tr');
                var startDate = this.startDate();
                this.groups = [];
                for (var idx = 0; idx < groupCount; idx++) {
                    this._addResourceView(idx);
                }
                this._groupedView._addDaySlotCollections(groupCount, tableRows, startDate);
            },
            addDaySlot: function (collection, cell, startDate, cellCount) {
                var clientHeight = cell.clientHeight;
                var firstChildHeight = cell.children.length ? cell.children[0].offsetHeight + 3 : 0;
                var start = kendo.date.addDays(startDate, cellCount);
                var end = kendo.date.MS_PER_DAY;
                if (startDate.getHours() !== start.getHours()) {
                    end += (startDate.getHours() - start.getHours()) * kendo.date.MS_PER_HOUR;
                }
                start = kendo.date.toUtcTime(start);
                end += start;
                var eventCount = Math.floor((clientHeight - firstChildHeight - this.options.moreButtonHeight) / (this.options.eventHeight + 3));
                cell.setAttribute('role', 'gridcell');
                cell.setAttribute('aria-selected', false);
                collection.addDaySlot(cell, start, end, eventCount);
            },
            render: function (events) {
                this.content.children('.k-event,.k-more-events,.k-events-container').remove();
                this._groups();
                events = new kendo.data.Query(events).sort([
                    {
                        field: 'start',
                        dir: 'asc'
                    },
                    {
                        field: 'end',
                        dir: 'desc'
                    }
                ]).toArray();
                var resources = this.groupedResources;
                if (resources.length) {
                    this._renderGroups(events, resources, 0, 1);
                } else {
                    this._renderEvents(events, 0);
                }
                this.refreshLayout();
                this.trigger('activate');
            },
            _renderEvents: function (events, groupIndex) {
                var event;
                var idx;
                var length;
                var range;
                var start;
                var end;
                for (idx = 0, length = events.length; idx < length; idx++) {
                    event = events[idx];
                    if (this._isInDateSlot(event)) {
                        var group = this.groups[groupIndex];
                        var view = this._groupedView._view;
                        var isMobilePhoneView = view._isMobilePhoneView();
                        if (!group._continuousEvents) {
                            group._continuousEvents = [];
                        }
                        var ranges = group.slotRanges(event, true);
                        var rangeCount = ranges.length;
                        if (isMobilePhoneView) {
                            range = ranges[0];
                            start = range.start.start;
                            end = range.end.end;
                            var rangeStart = new Date(Math.max(event.start.getTime(), kendo.timezone.toLocalDate(range.start.start)));
                            var rangeEnd = new Date(Math.min(event.end.getTime(), kendo.timezone.toLocalDate(ranges[ranges.length - 1].end.end))).getTime();
                            var newStart = new Date(start);
                            var newEnd = new Date(start);
                            while (rangeStart.getTime() < rangeEnd) {
                                var dateRange = group.daySlotRanges(newStart, newEnd, true)[0];
                                newEnd.setDate(newEnd.getDate() + 1);
                                newStart.setDate(newStart.getDate() + 1);
                                this._groupedView._positionEvent(event, group, dateRange, 1, start, end, 0);
                                rangeStart = kendo.date.addDays(rangeStart, 1);
                            }
                        } else {
                            for (var rangeIndex = 0; rangeIndex < rangeCount; rangeIndex++) {
                                range = ranges[rangeIndex];
                                start = event.start;
                                end = event.end;
                                this._groupedView._positionEvent(event, group, range, rangeCount, start, end, rangeIndex);
                            }
                        }
                    }
                }
            },
            _renderGroups: function (events, resources, offset, columnLevel) {
                var resource = resources[0];
                if (resource) {
                    var view = resource.dataSource.view();
                    for (var itemIdx = 0; itemIdx < view.length; itemIdx++) {
                        var value = this._resourceValue(resource, view[itemIdx]);
                        var tmp = new kendo.data.Query(events).filter({
                            field: resource.field,
                            operator: SchedulerView.groupEqFilter(value)
                        }).toArray();
                        if (resources.length > 1) {
                            offset = this._renderGroups(tmp, resources.slice(1), offset++, columnLevel + 1);
                        } else {
                            this._renderEvents(tmp, offset++);
                        }
                    }
                }
                return offset;
            },
            _groupCount: function () {
                var resources = this.groupedResources;
                var groupedView = this._groupedView;
                if (resources.length) {
                    if (this._isVerticallyGrouped()) {
                        return groupedView._verticalGroupCount(resources.length - 1);
                    } else {
                        return groupedView._horizontalGroupCount(resources.length);
                    }
                }
                return 1;
            },
            _columnOffsetForResource: function (index) {
                return this._columnCountForLevel(index) / this._columnCountForLevel(index - 1);
            },
            destroy: function () {
                if (this.table) {
                    this.table.removeClass('k-scheduler-monthview');
                }
                if (this.content) {
                    this.content.off(NS);
                }
                if (this.element) {
                    this.element.off(NS);
                }
                SchedulerView.fn.destroy.call(this);
                if (this._isMobile() && !this._isMobilePhoneView() && this.options.editable) {
                    if (this.options.editable.create !== false) {
                        this._addUserEvents.destroy();
                    }
                    if (this.options.editable.update !== false) {
                        this._editUserEvents.destroy();
                    }
                }
            },
            events: [
                'remove',
                'add',
                'edit',
                'navigate'
            ],
            options: {
                title: 'Month',
                name: 'month',
                eventHeight: 25,
                moreButtonHeight: 13,
                editable: true,
                selectedDateFormat: '{0:y}',
                selectedShortDateFormat: '{0:y}',
                groupHeaderTemplate: '#=text#',
                dayTemplate: DAY_TEMPLATE,
                eventTemplate: EVENT_TEMPLATE
            }
        });
        function shiftArray(array, idx) {
            return array.slice(idx).concat(array.slice(0, idx));
        }
        function firstVisibleMonthDay(date, calendarInfo) {
            var firstDay = calendarInfo.firstDay, firstVisibleDay = new Date(date.getFullYear(), date.getMonth(), 0, date.getHours(), date.getMinutes(), date.getSeconds(), date.getMilliseconds());
            while (firstVisibleDay.getDay() != firstDay) {
                kendo.date.setTime(firstVisibleDay, -1 * MS_PER_DAY);
            }
            return firstVisibleDay;
        }
        function isInDateRange(value, min, max) {
            var msMin = min, msMax = max, msValue;
            msValue = value;
            return msValue >= msMin && msValue <= msMax;
        }
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));