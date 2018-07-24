/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('kendo.gantt.list', [
        'kendo.dom',
        'kendo.touch',
        'kendo.draganddrop',
        'kendo.columnsorter',
        'kendo.datetimepicker',
        'kendo.editable'
    ], f);
}(function () {
    var __meta__ = {
        id: 'gantt.list',
        name: 'Gantt List',
        category: 'web',
        description: 'The Gantt List',
        depends: [
            'dom',
            'touch',
            'draganddrop',
            'columnsorter',
            'datetimepicker',
            'editable'
        ],
        hidden: true
    };
    (function ($) {
        var kendo = window.kendo;
        var kendoDom = kendo.dom;
        var kendoDomElement = kendoDom.element;
        var kendoTextElement = kendoDom.text;
        var browser = kendo.support.browser;
        var mobileOS = kendo.support.mobileOS;
        var ui = kendo.ui;
        var Widget = ui.Widget;
        var extend = $.extend;
        var outerWidth = kendo._outerWidth;
        var outerHeight = kendo._outerHeight;
        var map = $.map;
        var isFunction = $.isFunction;
        var oldIE = browser.msie && browser.version < 9;
        var keys = kendo.keys;
        var titleFromField = {
            'title': 'Title',
            'start': 'Start Time',
            'end': 'End Time',
            'percentComplete': '% Done',
            'parentId': 'Predecessor ID',
            'id': 'ID',
            'orderId': 'Order ID'
        };
        var STRING = 'string';
        var NS = '.kendoGanttList';
        var CLICK = 'click';
        var DOT = '.';
        var SIZE_CALCULATION_TEMPLATE = '<table style=\'visibility: hidden;\'>' + '<tbody>' + '<tr style=\'height:{0}\'>' + '<td>&nbsp;</td>' + '</tr>' + '</tbody>' + '</table>';
        var listStyles = {
            wrapper: 'k-treelist k-grid k-widget',
            header: 'k-header',
            alt: 'k-alt',
            rtl: 'k-rtl',
            editCell: 'k-edit-cell',
            group: 'k-treelist-group',
            gridHeader: 'k-grid-header',
            gridHeaderWrap: 'k-grid-header-wrap',
            gridContent: 'k-grid-content',
            gridContentWrap: 'k-grid-content',
            selected: 'k-state-selected',
            icon: 'k-icon',
            iconCollapse: 'k-i-collapse',
            iconExpand: 'k-i-expand',
            iconHidden: 'k-i-none',
            iconPlaceHolder: 'k-icon k-i-none',
            input: 'k-input',
            link: 'k-link',
            resizeHandle: 'k-resize-handle',
            resizeHandleInner: 'k-resize-handle-inner',
            dropPositions: 'k-i-insert-up k-i-insert-down k-i-plus k-i-insert-middle',
            dropTop: 'k-i-insert-up',
            dropBottom: 'k-i-insert-down',
            dropAdd: 'k-i-plus',
            dropMiddle: 'k-i-insert-middle',
            dropDenied: 'k-i-cancel',
            dragStatus: 'k-drag-status',
            dragClue: 'k-drag-clue',
            dragClueText: 'k-clue-text'
        };
        function createPlaceholders(options) {
            var spans = [];
            var className = options.className;
            for (var i = 0, level = options.level; i < level; i++) {
                spans.push(kendoDomElement('span', { className: className }));
            }
            return spans;
        }
        function blurActiveElement() {
            var activeElement = kendo._activeElement();
            if (activeElement && activeElement.nodeName.toLowerCase() !== 'body') {
                $(activeElement).blur();
            }
        }
        var GanttList = ui.GanttList = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                if (this.options.columns.length === 0) {
                    this.options.columns.push('title');
                }
                this.dataSource = this.options.dataSource;
                this._columns();
                this._layout();
                this._domTrees();
                this._header();
                this._sortable();
                this._editable();
                this._selectable();
                this._draggable();
                this._resizable();
                this._attachEvents();
                this._adjustHeight();
                this.bind('render', function () {
                    var headerCols;
                    var tableCols;
                    if (this.options.resizable) {
                        headerCols = this.header.find('col');
                        tableCols = this.content.find('col');
                        this.header.find('th').not(':last').each(function (index) {
                            var width = outerWidth($(this));
                            headerCols.eq(index).width(width);
                            tableCols.eq(index).width(width);
                        });
                        headerCols.last().css('width', 'auto');
                        tableCols.last().css('width', 'auto');
                    }
                }, true);
            },
            _adjustHeight: function () {
                if (this.content) {
                    this.content.height(this.element.height() - outerHeight(this.header.parent()));
                }
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                if (this._reorderDraggable) {
                    this._reorderDraggable.destroy();
                }
                if (this._tableDropArea) {
                    this._tableDropArea.destroy();
                }
                if (this._contentDropArea) {
                    this._contentDropArea.destroy();
                }
                if (this._columnResizable) {
                    this._columnResizable.destroy();
                }
                if (this.touch) {
                    this.touch.destroy();
                }
                if (this.timer) {
                    clearTimeout(this.timer);
                }
                this.content.off(NS);
                this.header.find('thead').off(NS);
                this.header.find(DOT + GanttList.link).off(NS);
                this.header = null;
                this.content = null;
                this.levels = null;
                kendo.destroy(this.element);
            },
            options: {
                name: 'GanttList',
                selectable: true,
                editable: true,
                resizable: false
            },
            _attachEvents: function () {
                var that = this;
                var listStyles = GanttList.styles;
                that.content.on(CLICK + NS, 'td > span.' + listStyles.icon + ':not(.' + listStyles.iconHidden + ')', function (e) {
                    var element = $(this);
                    var model = that._modelFromElement(element);
                    model.set('expanded', !model.get('expanded'));
                    e.stopPropagation();
                });
            },
            _domTrees: function () {
                this.headerTree = new kendoDom.Tree(this.header[0]);
                this.contentTree = new kendoDom.Tree(this.content[0]);
            },
            _columns: function () {
                var columns = this.options.columns;
                var model = function () {
                    this.field = '';
                    this.title = '';
                    this.editable = false;
                    this.sortable = false;
                };
                this.columns = map(columns, function (column) {
                    column = typeof column === STRING ? {
                        field: column,
                        title: titleFromField[column]
                    } : column;
                    return extend(new model(), column);
                });
            },
            _layout: function () {
                var that = this;
                var options = this.options;
                var element = this.element;
                var listStyles = GanttList.styles;
                var calculateRowHeight = function () {
                    var rowHeight = typeof options.rowHeight === STRING ? options.rowHeight : options.rowHeight + 'px';
                    var table = $(kendo.format(SIZE_CALCULATION_TEMPLATE, rowHeight));
                    var height;
                    that.content.append(table);
                    height = outerHeight(table.find('tr'));
                    table.remove();
                    return height;
                };
                element.addClass(listStyles.wrapper).append('<div class=\'' + listStyles.gridHeader + '\'><div class=\'' + listStyles.gridHeaderWrap + '\'></div></div>').append('<div class=\'' + listStyles.gridContentWrap + '\'></div>');
                this.header = element.find(DOT + listStyles.gridHeaderWrap);
                this.content = element.find(DOT + listStyles.gridContent);
                if (options.rowHeight) {
                    this._rowHeight = calculateRowHeight();
                }
            },
            _header: function () {
                var domTree = this.headerTree;
                var colgroup;
                var thead;
                var table;
                colgroup = kendoDomElement('colgroup', null, this._cols());
                thead = kendoDomElement('thead', { 'role': 'rowgroup' }, [kendoDomElement('tr', { 'role': 'row' }, this._ths())]);
                table = kendoDomElement('table', {
                    'style': { 'minWidth': this.options.listWidth + 'px' },
                    'role': 'grid'
                }, [
                    colgroup,
                    thead
                ]);
                domTree.render([table]);
            },
            _render: function (tasks) {
                var colgroup;
                var tbody;
                var table;
                var tableAttr = {
                    'style': { 'minWidth': this.options.listWidth + 'px' },
                    'tabIndex': 0,
                    'role': 'treegrid'
                };
                if (this._rowHeight) {
                    tableAttr.style.height = tasks.length * this._rowHeight + 'px';
                }
                this.levels = [{
                        field: null,
                        value: 0
                    }];
                colgroup = kendoDomElement('colgroup', null, this._cols());
                tbody = kendoDomElement('tbody', { 'role': 'rowgroup' }, this._trs(tasks));
                table = kendoDomElement('table', tableAttr, [
                    colgroup,
                    tbody
                ]);
                this.contentTree.render([table]);
                this.trigger('render');
            },
            _ths: function () {
                var columns = this.columns;
                var column;
                var attr;
                var ths = [];
                for (var i = 0, length = columns.length; i < length; i++) {
                    column = columns[i];
                    attr = {
                        'data-field': column.field,
                        'data-title': column.title,
                        className: GanttList.styles.header,
                        'role': 'columnheader'
                    };
                    ths.push(kendoDomElement('th', attr, [kendoTextElement(column.title)]));
                }
                if (this.options.resizable) {
                    ths.push(kendoDomElement('th', {
                        className: GanttList.styles.header,
                        'role': 'columnheader'
                    }));
                }
                return ths;
            },
            _cols: function () {
                var columns = this.columns;
                var column;
                var style;
                var width;
                var cols = [];
                for (var i = 0, length = columns.length; i < length; i++) {
                    column = columns[i];
                    width = column.width;
                    if (width && parseInt(width, 10) !== 0) {
                        style = { style: { width: typeof width === STRING ? width : width + 'px' } };
                    } else {
                        style = null;
                    }
                    cols.push(kendoDomElement('col', style, []));
                }
                if (this.options.resizable) {
                    cols.push(kendoDomElement('col', { style: { width: '1px' } }));
                }
                return cols;
            },
            _trs: function (tasks) {
                var task;
                var rows = [];
                var attr;
                var className = [];
                var level;
                var listStyles = GanttList.styles;
                for (var i = 0, length = tasks.length; i < length; i++) {
                    task = tasks[i];
                    level = this._levels({
                        idx: task.parentId,
                        id: task.id,
                        summary: task.summary
                    });
                    attr = {
                        'data-uid': task.uid,
                        'data-level': level,
                        'role': 'row'
                    };
                    if (task.summary) {
                        attr['aria-expanded'] = task.expanded;
                    }
                    if (i % 2 !== 0) {
                        className.push(listStyles.alt);
                    }
                    if (task.summary) {
                        className.push(listStyles.group);
                    }
                    if (className.length) {
                        attr.className = className.join(' ');
                    }
                    rows.push(this._tds({
                        task: task,
                        attr: attr,
                        level: level
                    }));
                    className = [];
                }
                return rows;
            },
            _tds: function (options) {
                var children = [];
                var columns = this.columns;
                var column;
                for (var i = 0, l = columns.length; i < l; i++) {
                    column = columns[i];
                    children.push(this._td({
                        task: options.task,
                        column: column,
                        level: options.level
                    }));
                }
                if (this.options.resizable) {
                    children.push(kendoDomElement('td', { 'role': 'gridcell' }));
                }
                return kendoDomElement('tr', options.attr, children);
            },
            _td: function (options) {
                var children = [];
                var resourcesField = this.options.resourcesField;
                var listStyles = GanttList.styles;
                var task = options.task;
                var column = options.column;
                var value = task.get(column.field);
                var formatedValue;
                var label;
                if (column.field == resourcesField) {
                    value = value || [];
                    formatedValue = [];
                    for (var i = 0; i < value.length; i++) {
                        formatedValue.push(kendo.format('{0} [{1}]', value[i].get('name'), value[i].get('formatedValue')));
                    }
                    formatedValue = formatedValue.join(', ');
                } else {
                    formatedValue = column.format ? kendo.format(column.format, value) : value;
                }
                if (column.field === 'title') {
                    children = createPlaceholders({
                        level: options.level,
                        className: listStyles.iconPlaceHolder
                    });
                    children.push(kendoDomElement('span', { className: listStyles.icon + ' ' + (task.summary ? task.expanded ? listStyles.iconCollapse : listStyles.iconExpand : listStyles.iconHidden) }));
                    label = kendo.format('{0}, {1:P0}', formatedValue, task.percentComplete);
                }
                children.push(kendoDomElement('span', { 'aria-label': label }, [kendoTextElement(formatedValue)]));
                return kendoDomElement('td', { 'role': 'gridcell' }, children);
            },
            _levels: function (options) {
                var levels = this.levels;
                var level;
                var summary = options.summary;
                var idx = options.idx;
                var id = options.id;
                for (var i = 0, length = levels.length; i < length; i++) {
                    level = levels[i];
                    if (level.field == idx) {
                        if (summary) {
                            levels.push({
                                field: id,
                                value: level.value + 1
                            });
                        }
                        return level.value;
                    }
                }
            },
            _sortable: function () {
                var that = this;
                var resourcesField = this.options.resourcesField;
                var columns = this.columns;
                var column;
                var sortableInstance;
                var cells = this.header.find('th[' + kendo.attr('field') + ']');
                var cell;
                var changeHandler = function (e) {
                    if (that.dataSource.total() === 0 || that.editable && that.editable.trigger('validate')) {
                        e.preventDefault();
                    }
                };
                for (var idx = 0, length = cells.length; idx < length; idx++) {
                    column = columns[idx];
                    if (column.sortable && column.field !== resourcesField) {
                        cell = cells.eq(idx);
                        sortableInstance = cell.data('kendoColumnSorter');
                        if (sortableInstance) {
                            sortableInstance.destroy();
                        }
                        cell.attr('data-' + kendo.ns + 'field', column.field).kendoColumnSorter({
                            dataSource: this.dataSource,
                            change: changeHandler
                        });
                    }
                }
                cells = null;
            },
            _selectable: function () {
                var that = this;
                var selectable = this.options.selectable;
                if (selectable) {
                    this.content.on(CLICK + NS, 'tr', function (e) {
                        var element = $(this);
                        if (that.editable) {
                            that.editable.trigger('validate');
                        }
                        if (!e.ctrlKey) {
                            that.select(element);
                        } else {
                            that.clearSelection();
                        }
                    });
                }
            },
            select: function (value) {
                var element = this.content.find(value);
                var selectedClassName = GanttList.styles.selected;
                if (element.length) {
                    element.siblings(DOT + selectedClassName).removeClass(selectedClassName).attr('aria-selected', false).end().addClass(selectedClassName).attr('aria-selected', true);
                    this.trigger('change');
                    return;
                }
                return this.content.find(DOT + selectedClassName);
            },
            clearSelection: function () {
                var selected = this.select();
                if (selected.length) {
                    selected.removeClass(GanttList.styles.selected);
                    this.trigger('change');
                }
            },
            _setDataSource: function (dataSource) {
                this.dataSource = dataSource;
                this._sortable();
            },
            _editable: function () {
                var that = this;
                var editable = this.options.editable;
                var listStyles = GanttList.styles;
                var iconSelector = 'span.' + listStyles.icon + ':not(' + listStyles.iconHidden + ')';
                var finishEdit = function () {
                    var editable = that.editable;
                    if (editable) {
                        if (editable.end()) {
                            that._closeCell();
                        } else {
                            editable.trigger('validate');
                        }
                    }
                };
                var mousedown = function (e) {
                    var currentTarget = $(e.currentTarget);
                    if (!currentTarget.hasClass(listStyles.editCell)) {
                        blurActiveElement();
                    }
                };
                if (!editable || editable.update === false) {
                    return;
                }
                this._startEditHandler = function (e) {
                    var td = e.currentTarget ? $(e.currentTarget) : e;
                    var column = that._columnFromElement(td);
                    if (that.editable) {
                        return;
                    }
                    if (column && column.editable) {
                        that._editCell({
                            cell: td,
                            column: column
                        });
                    }
                };
                that.content.on('focusin' + NS, function () {
                    clearTimeout(that.timer);
                    that.timer = null;
                }).on('focusout' + NS, function () {
                    that.timer = setTimeout(finishEdit, 1);
                }).on('keydown' + NS, function (e) {
                    if (e.keyCode === keys.ENTER) {
                        e.preventDefault();
                    }
                }).on('keyup' + NS, function (e) {
                    var key = e.keyCode;
                    var cell;
                    var model;
                    switch (key) {
                    case keys.ENTER:
                        blurActiveElement();
                        finishEdit();
                        break;
                    case keys.ESC:
                        if (that.editable) {
                            cell = that._editableContainer;
                            model = that._modelFromElement(cell);
                            if (!that.trigger('cancel', {
                                    model: model,
                                    cell: cell
                                })) {
                                that._closeCell(true);
                            }
                        }
                        break;
                    }
                });
                if (!mobileOS) {
                    that.content.on('mousedown' + NS, 'td', function (e) {
                        mousedown(e);
                    }).on('dblclick' + NS, 'td', function (e) {
                        if (!$(e.target).is(iconSelector)) {
                            that._startEditHandler(e);
                        }
                    });
                } else {
                    that.touch = that.content.kendoTouch({
                        filter: 'td',
                        touchstart: function (e) {
                            mousedown(e.touch);
                        },
                        doubletap: function (e) {
                            if (!$(e.touch.initialTouch).is(iconSelector)) {
                                that._startEditHandler(e.touch);
                            }
                        }
                    }).data('kendoTouch');
                }
            },
            _editCell: function (options) {
                var resourcesField = this.options.resourcesField;
                var listStyles = GanttList.styles;
                var cell = options.cell;
                var column = options.column;
                var model = this._modelFromElement(cell);
                var modelCopy = this.dataSource._createNewModel(model.toJSON());
                var field = modelCopy.fields[column.field] || modelCopy[column.field];
                var validation = field.validation;
                var DATATYPE = kendo.attr('type');
                var BINDING = kendo.attr('bind');
                var FORMAT = kendo.attr('format');
                var attr = {
                    'name': column.field,
                    'required': field.validation ? field.validation.required === true : false
                };
                var editor;
                if (column.field === resourcesField) {
                    column.editor(cell, modelCopy);
                    return;
                }
                this._editableContent = cell.children().detach();
                this._editableContainer = cell;
                cell.data('modelCopy', modelCopy);
                if ((field.type === 'date' || $.type(field) === 'date') && (!column.format || /H|m|s|F|g|u/.test(column.format))) {
                    attr[BINDING] = 'value:' + column.field;
                    attr[DATATYPE] = 'date';
                    if (column.format) {
                        attr[FORMAT] = kendo._extractFormat(column.format);
                    }
                    editor = function (container, options) {
                        $('<input type="text"/>').attr(attr).appendTo(container).kendoDateTimePicker({ format: options.format });
                    };
                }
                this.editable = cell.addClass(listStyles.editCell).kendoEditable({
                    fields: {
                        field: column.field,
                        format: column.format,
                        editor: column.editor || editor
                    },
                    model: modelCopy,
                    clearContainer: false
                }).data('kendoEditable');
                if (validation && validation.dateCompare && isFunction(validation.dateCompare) && validation.message) {
                    $('<span ' + kendo.attr('for') + '="' + column.field + '" class="k-invalid-msg"/>').hide().appendTo(cell);
                    cell.find('[name=' + column.field + ']').attr(kendo.attr('dateCompare-msg'), validation.message);
                }
                this.editable.bind('validate', function (e) {
                    var focusable = this.element.find(':kendoFocusable:first').focus();
                    if (oldIE) {
                        focusable.focus();
                    }
                    e.preventDefault();
                });
                if (this.trigger('edit', {
                        model: model,
                        cell: cell
                    })) {
                    this._closeCell(true);
                }
            },
            _closeCell: function (cancelUpdate) {
                var listStyles = GanttList.styles;
                var cell = this._editableContainer;
                var model = this._modelFromElement(cell);
                var column = this._columnFromElement(cell);
                var field = column.field;
                var copy = cell.data('modelCopy');
                var taskInfo = {};
                taskInfo[field] = copy.get(field);
                cell.empty().removeData('modelCopy').removeClass(listStyles.editCell).append(this._editableContent);
                this.editable.unbind();
                this.editable.destroy();
                this.editable = null;
                this._editableContainer = null;
                this._editableContent = null;
                if (!cancelUpdate) {
                    if (field === 'start') {
                        taskInfo.end = new Date(taskInfo.start.getTime() + model.duration());
                    }
                    this.trigger('update', {
                        task: model,
                        updateInfo: taskInfo
                    });
                }
            },
            _draggable: function () {
                var that = this;
                var draggedTask = null;
                var dropAllowed = true;
                var dropTarget;
                var listStyles = GanttList.styles;
                var isRtl = kendo.support.isRtl(this.element);
                var selector = 'tr[' + kendo.attr('level') + ' = 0]:last';
                var action = {};
                var editable = this.options.editable;
                var clear = function () {
                    draggedTask = null;
                    dropTarget = null;
                    dropAllowed = true;
                    action = {};
                };
                var allowDrop = function (task) {
                    var parent = task;
                    while (parent) {
                        if (draggedTask.get('id') === parent.get('id')) {
                            dropAllowed = false;
                            break;
                        }
                        parent = that.dataSource.taskParent(parent);
                    }
                };
                var defineLimits = function () {
                    var height = $(dropTarget).height();
                    var offsetTop = kendo.getOffset(dropTarget).top;
                    extend(dropTarget, {
                        beforeLimit: offsetTop + height * 0.25,
                        afterLimit: offsetTop + height * 0.75
                    });
                };
                var defineAction = function (coordinate) {
                    if (!dropTarget) {
                        return;
                    }
                    var location = coordinate.location;
                    var className = listStyles.dropAdd;
                    var command = 'add';
                    var level = parseInt(dropTarget.attr(kendo.attr('level')), 10);
                    var sibling;
                    if (location <= dropTarget.beforeLimit) {
                        sibling = dropTarget.prev();
                        className = listStyles.dropTop;
                        command = 'insert-before';
                    } else if (location >= dropTarget.afterLimit) {
                        sibling = dropTarget.next();
                        className = listStyles.dropBottom;
                        command = 'insert-after';
                    }
                    if (sibling && parseInt(sibling.attr(kendo.attr('level')), 10) === level) {
                        className = listStyles.dropMiddle;
                    }
                    action.className = className;
                    action.command = command;
                };
                var status = function () {
                    return that._reorderDraggable.hint.children(DOT + listStyles.dragStatus).removeClass(listStyles.dropPositions);
                };
                if (!editable || editable.reorder === false || editable.update === false) {
                    return;
                }
                this._reorderDraggable = this.content.kendoDraggable({
                    distance: 10,
                    holdToDrag: mobileOS,
                    group: 'listGroup',
                    filter: 'tr[data-uid]',
                    ignore: DOT + listStyles.input,
                    hint: function (target) {
                        return $('<div class="' + listStyles.header + ' ' + listStyles.dragClue + '"/>').css({
                            width: 300,
                            paddingLeft: target.css('paddingLeft'),
                            paddingRight: target.css('paddingRight'),
                            lineHeight: target.height() + 'px',
                            paddingTop: target.css('paddingTop'),
                            paddingBottom: target.css('paddingBottom')
                        }).append('<span class="' + listStyles.icon + ' ' + listStyles.dragStatus + '" /><span class="' + listStyles.dragClueText + '"/>');
                    },
                    cursorOffset: {
                        top: -20,
                        left: 0
                    },
                    container: this.content,
                    'dragstart': function (e) {
                        var editable = that.editable;
                        if (editable && editable.reorder !== false && editable.trigger('validate')) {
                            e.preventDefault();
                            return;
                        }
                        draggedTask = that._modelFromElement(e.currentTarget);
                        this.hint.children(DOT + listStyles.dragClueText).text(draggedTask.get('title'));
                        if (isRtl) {
                            this.hint.addClass(listStyles.rtl);
                        }
                    },
                    'drag': function (e) {
                        if (dropAllowed) {
                            defineAction(e.y);
                            status().addClass(action.className);
                        }
                    },
                    'dragend': function () {
                        clear();
                    },
                    'dragcancel': function () {
                        clear();
                    }
                }).data('kendoDraggable');
                this._tableDropArea = this.content.kendoDropTargetArea({
                    distance: 0,
                    group: 'listGroup',
                    filter: 'tr[data-uid]',
                    'dragenter': function (e) {
                        dropTarget = e.dropTarget;
                        allowDrop(that._modelFromElement(dropTarget));
                        defineLimits();
                        status().toggleClass(listStyles.dropDenied, !dropAllowed);
                    },
                    'dragleave': function () {
                        dropAllowed = true;
                        status();
                    },
                    'drop': function () {
                        var target = that._modelFromElement(dropTarget);
                        var orderId = target.orderId;
                        var taskInfo = { parentId: target.parentId };
                        if (dropAllowed) {
                            switch (action.command) {
                            case 'add':
                                taskInfo.parentId = target.id;
                                break;
                            case 'insert-before':
                                if (target.parentId === draggedTask.parentId && target.orderId > draggedTask.orderId) {
                                    taskInfo.orderId = orderId - 1;
                                } else {
                                    taskInfo.orderId = orderId;
                                }
                                break;
                            case 'insert-after':
                                if (target.parentId === draggedTask.parentId && target.orderId > draggedTask.orderId) {
                                    taskInfo.orderId = orderId;
                                } else {
                                    taskInfo.orderId = orderId + 1;
                                }
                                break;
                            }
                            that.trigger('update', {
                                task: draggedTask,
                                updateInfo: taskInfo
                            });
                        }
                    }
                }).data('kendoDropTargetArea');
                this._contentDropArea = this.element.kendoDropTargetArea({
                    distance: 0,
                    group: 'listGroup',
                    filter: DOT + listStyles.gridContent,
                    'drop': function () {
                        var target = that._modelFromElement(that.content.find(selector));
                        var orderId = target.orderId;
                        var taskInfo = {
                            parentId: null,
                            orderId: draggedTask.parentId !== null ? orderId + 1 : orderId
                        };
                        that.trigger('update', {
                            task: draggedTask,
                            updateInfo: taskInfo
                        });
                    }
                }).data('kendoDropTargetArea');
            },
            _resizable: function () {
                var that = this;
                var listStyles = GanttList.styles;
                var positionResizeHandle = function (e) {
                    var th = $(e.currentTarget);
                    var resizeHandle = that.resizeHandle;
                    var position = th.position();
                    var left = position.left;
                    var cellWidth = outerWidth(th);
                    var container = th.closest('div');
                    var clientX = e.clientX + $(window).scrollLeft();
                    var indicatorWidth = that.options.columnResizeHandleWidth;
                    left += container.scrollLeft();
                    if (!resizeHandle) {
                        resizeHandle = that.resizeHandle = $('<div class="' + listStyles.resizeHandle + '"><div class="' + listStyles.resizeHandleInner + '" /></div>');
                    }
                    var cellOffset = th.offset().left + cellWidth;
                    var show = clientX > cellOffset - indicatorWidth && clientX < cellOffset + indicatorWidth;
                    if (!show) {
                        resizeHandle.hide();
                        return;
                    }
                    container.append(resizeHandle);
                    resizeHandle.show().css({
                        top: position.top,
                        left: left + cellWidth - indicatorWidth - 1,
                        height: outerHeight(th),
                        width: indicatorWidth * 3
                    }).data('th', th);
                };
                if (!this.options.resizable) {
                    return;
                }
                if (this._columnResizable) {
                    this._columnResizable.destroy();
                }
                this.header.find('thead').on('mousemove' + NS, 'th', positionResizeHandle);
                this._columnResizable = this.header.kendoResizable({
                    handle: DOT + listStyles.resizeHandle,
                    start: function (e) {
                        var th = $(e.currentTarget).data('th');
                        var colSelector = 'col:eq(' + th.index() + ')';
                        var header = that.header.find('table');
                        var contentTable = that.content.find('table');
                        that.element.addClass('k-grid-column-resizing');
                        this.col = contentTable.children('colgroup').find(colSelector).add(header.find(colSelector));
                        this.th = th;
                        this.startLocation = e.x.location;
                        this.columnWidth = outerWidth(th);
                        this.table = header.add(contentTable);
                        this.totalWidth = this.table.width() - outerWidth(header.find('th:last'));
                    },
                    resize: function (e) {
                        var minColumnWidth = 11;
                        var delta = e.x.location - this.startLocation;
                        if (this.columnWidth + delta < minColumnWidth) {
                            delta = minColumnWidth - this.columnWidth;
                        }
                        this.table.css({ 'minWidth': this.totalWidth + delta });
                        this.col.width(this.columnWidth + delta);
                    },
                    resizeend: function () {
                        that.element.removeClass('k-grid-column-resizing');
                        var oldWidth = Math.floor(this.columnWidth);
                        var newWidth = Math.floor(outerWidth(this.th));
                        var column = that.columns[this.th.index()];
                        that.trigger('columnResize', {
                            column: column,
                            oldWidth: oldWidth,
                            newWidth: newWidth
                        });
                        this.table = this.col = this.th = null;
                    }
                }).data('kendoResizable');
            },
            _modelFromElement: function (element) {
                var row = element.closest('tr');
                var model = this.dataSource.getByUid(row.attr(kendo.attr('uid')));
                return model;
            },
            _columnFromElement: function (element) {
                var td = element.closest('td');
                var tr = td.parent();
                var idx = tr.children().index(td);
                return this.columns[idx];
            }
        });
        extend(true, ui.GanttList, { styles: listStyles });
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));