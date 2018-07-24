/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('kendo.columnmenu', [
        'kendo.popup',
        'kendo.filtermenu',
        'kendo.menu'
    ], f);
}(function () {
    var __meta__ = {
        id: 'columnmenu',
        name: 'Column Menu',
        category: 'framework',
        depends: [
            'popup',
            'filtermenu',
            'menu'
        ],
        advanced: true
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.ui, proxy = $.proxy, extend = $.extend, grep = $.grep, map = $.map, inArray = $.inArray, ACTIVE = 'k-state-selected', ASC = 'asc', DESC = 'desc', CHANGE = 'change', INIT = 'init', OPEN = 'open', SELECT = 'select', POPUP = 'kendoPopup', FILTERMENU = 'kendoFilterMenu', MENU = 'kendoMenu', NS = '.kendoColumnMenu', Widget = ui.Widget;
        function trim(text) {
            return $.trim(text).replace(/&nbsp;/gi, '');
        }
        function toHash(arr, key) {
            var result = {};
            var idx, len, current;
            for (idx = 0, len = arr.length; idx < len; idx++) {
                current = arr[idx];
                result[current[key]] = current;
            }
            return result;
        }
        function leafColumns(columns) {
            var result = [];
            for (var idx = 0; idx < columns.length; idx++) {
                if (!columns[idx].columns) {
                    result.push(columns[idx]);
                    continue;
                }
                result = result.concat(leafColumns(columns[idx].columns));
            }
            return result;
        }
        function attrEquals(attrName, attrValue) {
            return '[' + kendo.attr(attrName) + '=\'' + (attrValue || '').replace(/'/g, '"') + '\']';
        }
        function insertElementAt(index, element, container) {
            if (index > 0) {
                element.insertAfter(container.children().eq(index - 1));
            } else {
                container.prepend(element);
            }
        }
        var ColumnMenu = Widget.extend({
            init: function (element, options) {
                var that = this, link;
                Widget.fn.init.call(that, element, options);
                element = that.element;
                options = that.options;
                that.owner = options.owner;
                that.dataSource = options.dataSource;
                that.field = element.attr(kendo.attr('field'));
                that.title = element.attr(kendo.attr('title'));
                link = element.find('.k-header-column-menu');
                if (!link[0]) {
                    link = element.addClass('k-with-icon').prepend('<a class="k-header-column-menu" href="#" title="' + options.messages.settings + '" aria-label="' + options.messages.settings + '"><span class="k-icon k-i-more-vertical"></span></a>').find('.k-header-column-menu');
                }
                that.link = link.attr('tabindex', -1).on('click' + NS, proxy(that._click, that));
                that.wrapper = $('<div class="k-column-menu"/>');
                that._refreshHandler = proxy(that.refresh, that);
                that.dataSource.bind(CHANGE, that._refreshHandler);
            },
            _init: function () {
                var that = this;
                that.pane = that.options.pane;
                if (that.pane) {
                    that._isMobile = true;
                }
                if (that._isMobile) {
                    that._createMobileMenu();
                } else {
                    that._createMenu();
                }
                that.owner._muteAngularRebind(function () {
                    that._angularItems('compile');
                });
                that._sort();
                that._columns();
                that._filter();
                that._lockColumns();
                that.trigger(INIT, {
                    field: that.field,
                    container: that.wrapper
                });
            },
            events: [
                INIT,
                OPEN,
                'sort',
                'filtering'
            ],
            options: {
                name: 'ColumnMenu',
                messages: {
                    sortAscending: 'Sort Ascending',
                    sortDescending: 'Sort Descending',
                    filter: 'Filter',
                    columns: 'Columns',
                    done: 'Done',
                    settings: 'Column Settings',
                    lock: 'Lock',
                    unlock: 'Unlock'
                },
                filter: '',
                columns: true,
                sortable: true,
                filterable: true,
                animations: { left: 'slide' }
            },
            _createMenu: function () {
                var that = this, options = that.options;
                that.wrapper.html(kendo.template(template)({
                    uid: kendo.guid(),
                    ns: kendo.ns,
                    messages: options.messages,
                    sortable: options.sortable,
                    filterable: options.filterable,
                    columns: that._ownerColumns(),
                    showColumns: options.columns,
                    lockedColumns: options.lockedColumns
                }));
                that.popup = that.wrapper[POPUP]({
                    anchor: that.link,
                    open: proxy(that._open, that),
                    activate: proxy(that._activate, that),
                    close: function () {
                        if (that.options.closeCallback) {
                            that.options.closeCallback(that.element);
                        }
                    }
                }).data(POPUP);
                that.menu = that.wrapper.children()[MENU]({
                    orientation: 'vertical',
                    closeOnClick: false
                }).data(MENU);
            },
            _createMobileMenu: function () {
                var that = this, options = that.options;
                var html = kendo.template(mobileTemplate)({
                    ns: kendo.ns,
                    field: that.field,
                    title: that.title || that.field,
                    messages: options.messages,
                    sortable: options.sortable,
                    filterable: options.filterable,
                    columns: that._ownerColumns(),
                    showColumns: options.columns,
                    lockedColumns: options.lockedColumns
                });
                that.view = that.pane.append(html);
                that.wrapper = that.view.element.find('.k-column-menu');
                that.menu = new MobileMenu(that.wrapper.children(), { pane: that.pane });
                that.view.element.on('click', '.k-done', function (e) {
                    that.close();
                    e.preventDefault();
                });
                if (that.options.lockedColumns) {
                    that.view.bind('show', function () {
                        that._updateLockedColumns();
                    });
                }
            },
            _angularItems: function (action) {
                var that = this;
                that.angular(action, function () {
                    var items = that.wrapper.find('.k-columns-item input[' + kendo.attr('field') + ']').map(function () {
                        return $(this).closest('li');
                    });
                    var data = map(that._ownerColumns(), function (col) {
                        return { column: col._originalObject };
                    });
                    return {
                        elements: items,
                        data: data
                    };
                });
            },
            destroy: function () {
                var that = this;
                that._angularItems('cleanup');
                Widget.fn.destroy.call(that);
                if (that.filterMenu) {
                    that.filterMenu.destroy();
                }
                if (that._refreshHandler) {
                    that.dataSource.unbind(CHANGE, that._refreshHandler);
                }
                if (that.options.columns && that.owner) {
                    if (that._updateColumnsMenuHandler) {
                        that.owner.unbind('columnShow', that._updateColumnsMenuHandler);
                        that.owner.unbind('columnHide', that._updateColumnsMenuHandler);
                    }
                    if (that._updateColumnsLockedStateHandler) {
                        that.owner.unbind('columnLock', that._updateColumnsLockedStateHandler);
                        that.owner.unbind('columnUnlock', that._updateColumnsLockedStateHandler);
                    }
                }
                if (that.menu) {
                    that.menu.element.off(NS);
                    that.menu.destroy();
                }
                that.wrapper.off(NS);
                if (that.popup) {
                    that.popup.destroy();
                }
                if (that.view) {
                    that.view.purge();
                }
                that.link.off(NS);
                that.owner = null;
                that.wrapper = null;
                that.element = null;
            },
            close: function () {
                this.menu.close();
                if (this.popup) {
                    this.popup.close();
                    this.popup.element.off('keydown' + NS);
                }
            },
            _click: function (e) {
                var that = this;
                e.preventDefault();
                e.stopPropagation();
                var options = this.options;
                if (options.filter && this.element.is(!options.filter)) {
                    return;
                }
                if (!this.popup && !this.pane) {
                    this._init();
                } else {
                    that._reorderColumnList();
                }
                if (this._isMobile) {
                    this.pane.navigate(this.view, this.options.animations.left);
                } else {
                    this.popup.toggle();
                }
            },
            _reorderColumnList: function () {
                var that = this;
                var i;
                var listElement;
                var columns = that._ownerColumns() || [];
                var columnList = that._isMobile && that.view ? $(that.view.element).find('.k-columns-item').children('ul') : $(that.wrapper).find('.k-menu-group').first();
                for (i = 0; i < columns.length; i++) {
                    listElement = columnList.find(attrEquals('field', columns[i].originalField)).closest('li');
                    if (listElement[0] && listElement.index() !== i) {
                        insertElementAt(i, listElement, columnList);
                    }
                }
            },
            _open: function () {
                var that = this;
                $('.k-column-menu').not(that.wrapper).each(function () {
                    $(this).data(POPUP).close();
                });
                that.popup.element.on('keydown' + NS, function (e) {
                    if (e.keyCode == kendo.keys.ESC) {
                        that.close();
                    }
                });
                if (that.options.lockedColumns) {
                    that._updateLockedColumns();
                }
            },
            _activate: function () {
                this.menu.element.focus();
                this.trigger(OPEN, {
                    field: this.field,
                    container: this.wrapper
                });
            },
            _ownerColumns: function () {
                var columns = leafColumns(this.owner.columns), menuColumns = grep(columns, function (col) {
                        var result = true, title = trim(col.title || '');
                        if (col.menu === false || !col.field && !title.length) {
                            result = false;
                        }
                        return result;
                    });
                return map(menuColumns, function (col) {
                    return {
                        originalField: col.field,
                        field: col.field || col.title,
                        title: col.title || col.field,
                        hidden: col.hidden,
                        index: inArray(col, columns),
                        locked: !!col.locked,
                        _originalObject: col
                    };
                });
            },
            _sort: function () {
                var that = this;
                if (that.options.sortable) {
                    that.refresh();
                    that.menu.bind(SELECT, function (e) {
                        var item = $(e.item), dir;
                        if (item.hasClass('k-sort-asc')) {
                            dir = ASC;
                        } else if (item.hasClass('k-sort-desc')) {
                            dir = DESC;
                        }
                        if (!dir) {
                            return;
                        }
                        item.parent().find('.k-sort-' + (dir == ASC ? DESC : ASC)).removeClass(ACTIVE);
                        that._sortDataSource(item, dir);
                        that.close();
                    });
                }
            },
            _sortDataSource: function (item, dir) {
                var that = this, sortable = that.options.sortable, compare = sortable.compare === null ? undefined : sortable.compare, dataSource = that.dataSource, idx, length, sort = dataSource.sort() || [];
                var removeClass = item.hasClass(ACTIVE) && sortable && sortable.allowUnsort !== false;
                dir = !removeClass ? dir : undefined;
                if (that.trigger('sort', {
                        sort: {
                            field: that.field,
                            dir: dir,
                            compare: compare
                        }
                    })) {
                    return;
                }
                if (removeClass) {
                    item.removeClass(ACTIVE);
                } else {
                    item.addClass(ACTIVE);
                }
                if (sortable.mode === 'multiple') {
                    for (idx = 0, length = sort.length; idx < length; idx++) {
                        if (sort[idx].field === that.field) {
                            sort.splice(idx, 1);
                            break;
                        }
                    }
                    sort.push({
                        field: that.field,
                        dir: dir,
                        compare: compare
                    });
                } else {
                    sort = [{
                            field: that.field,
                            dir: dir,
                            compare: compare
                        }];
                }
                dataSource.sort(sort);
            },
            _columns: function () {
                var that = this;
                if (that.options.columns) {
                    that._updateColumnsMenu();
                    that._updateColumnsMenuHandler = proxy(that._updateColumnsMenu, that);
                    that.owner.bind([
                        'columnHide',
                        'columnShow'
                    ], that._updateColumnsMenuHandler);
                    that._updateColumnsLockedStateHandler = proxy(that._updateColumnsLockedState, that);
                    that.owner.bind([
                        'columnUnlock',
                        'columnLock'
                    ], that._updateColumnsLockedStateHandler);
                    that.menu.bind(SELECT, function (e) {
                        var item = $(e.item), input, column, columns = leafColumns(that.owner.columns), field;
                        if (that._isMobile) {
                            e.preventDefault();
                        }
                        if (!item.parent().closest('li.k-columns-item')[0]) {
                            return;
                        }
                        input = item.find(':checkbox');
                        if (input.attr('disabled')) {
                            return;
                        }
                        field = input.attr(kendo.attr('field'));
                        column = grep(columns, function (column) {
                            return column.field == field || !column.field && column.title == field;
                        })[0];
                        if (column.hidden === true) {
                            that.owner.showColumn(column);
                        } else {
                            that.owner.hideColumn(column);
                        }
                    });
                }
            },
            _updateColumnsMenu: function () {
                var idx, length, current, checked, locked;
                var fieldAttr = kendo.attr('field'), lockedAttr = kendo.attr('locked'), visible = grep(this._ownerColumns(), function (field) {
                        return !field.hidden;
                    }), visibleDataFields = grep(visible, function (field) {
                        return field.originalField;
                    }), lockedCount = grep(visibleDataFields, function (col) {
                        return col.locked === true;
                    }).length, nonLockedCount = grep(visibleDataFields, function (col) {
                        return col.locked !== true;
                    }).length, columnNotInMenuCount = grep(this.owner.columns, function (col) {
                        return col.menu === false;
                    }).length;
                visible = map(visible, function (col) {
                    return col.field;
                });
                this.wrapper.find('[role=\'menuitemcheckbox\']').attr('aria-checked', false);
                var checkboxes = this.wrapper.find('.k-columns-item input[' + fieldAttr + ']').prop('disabled', false).prop('checked', false);
                for (idx = 0, length = checkboxes.length; idx < length; idx++) {
                    current = checkboxes.eq(idx);
                    locked = current.attr(lockedAttr) === 'true';
                    checked = false;
                    if (inArray(current.attr(fieldAttr), visible) > -1) {
                        checked = true;
                        current.prop('checked', checked);
                    }
                    current.closest('[role=\'menuitemcheckbox\']').attr('aria-checked', checked);
                    if (checked) {
                        if (lockedCount == 1 && locked) {
                            current.prop('disabled', true);
                        }
                        if (columnNotInMenuCount === 0 && nonLockedCount == 1 && !locked) {
                            current.prop('disabled', true);
                        }
                    }
                }
            },
            _updateColumnsLockedState: function () {
                var idx, length, current, column;
                var fieldAttr = kendo.attr('field');
                var lockedAttr = kendo.attr('locked');
                var columns = toHash(this._ownerColumns(), 'field');
                var checkboxes = this.wrapper.find('.k-columns-item input[type=checkbox]');
                for (idx = 0, length = checkboxes.length; idx < length; idx++) {
                    current = checkboxes.eq(idx);
                    column = columns[current.attr(fieldAttr)];
                    if (column) {
                        current.attr(lockedAttr, column.locked);
                    }
                }
                this._updateColumnsMenu();
            },
            _filter: function () {
                var that = this, widget = FILTERMENU, options = that.options;
                if (options.filterable !== false) {
                    if (options.filterable.multi) {
                        widget = 'kendoFilterMultiCheck';
                        if (options.filterable.dataSource) {
                            options.filterable.checkSource = options.filterable.dataSource;
                            delete options.filterable.dataSource;
                        }
                    }
                    that.filterMenu = that.wrapper.find('.k-filterable')[widget](extend(true, {}, {
                        appendToElement: true,
                        dataSource: options.dataSource,
                        values: options.values,
                        field: that.field,
                        title: that.title,
                        change: function (e) {
                            if (that.trigger('filtering', {
                                    filter: e.filter,
                                    field: e.field
                                })) {
                                e.preventDefault();
                            }
                        }
                    }, options.filterable)).data(widget);
                    if (that._isMobile) {
                        that.menu.bind(SELECT, function (e) {
                            var item = $(e.item);
                            if (item.hasClass('k-filter-item')) {
                                that.pane.navigate(that.filterMenu.view, that.options.animations.left);
                            }
                        });
                    }
                }
            },
            _lockColumns: function () {
                var that = this;
                that.menu.bind(SELECT, function (e) {
                    var item = $(e.item);
                    if (item.hasClass('k-lock')) {
                        that.owner.lockColumn(that.field);
                        that.close();
                    } else if (item.hasClass('k-unlock')) {
                        that.owner.unlockColumn(that.field);
                        that.close();
                    }
                });
            },
            _updateLockedColumns: function () {
                var field = this.field;
                var columns = this.owner.columns;
                var column = grep(columns, function (column) {
                    return column.field == field || column.title == field;
                })[0];
                if (!column) {
                    return;
                }
                var locked = column.locked === true;
                var length = grep(columns, function (column) {
                    return !column.hidden && (column.locked && locked || !column.locked && !locked);
                }).length;
                var lockItem = this.wrapper.find('.k-lock').removeClass('k-state-disabled');
                var unlockItem = this.wrapper.find('.k-unlock').removeClass('k-state-disabled');
                if (locked || length == 1) {
                    lockItem.addClass('k-state-disabled');
                }
                if (!locked || length == 1) {
                    unlockItem.addClass('k-state-disabled');
                }
                this._updateColumnsLockedState();
            },
            refresh: function () {
                var that = this, sort = that.options.dataSource.sort() || [], descriptor, field = that.field, idx, length;
                that.wrapper.find('.k-sort-asc, .k-sort-desc').removeClass(ACTIVE);
                for (idx = 0, length = sort.length; idx < length; idx++) {
                    descriptor = sort[idx];
                    if (field == descriptor.field) {
                        that.wrapper.find('.k-sort-' + descriptor.dir).addClass(ACTIVE);
                    }
                }
                that.link[that._filterExist(that.dataSource.filter()) ? 'addClass' : 'removeClass']('k-state-active');
            },
            _filterExist: function (filters) {
                var found = false;
                var filter;
                if (!filters) {
                    return;
                }
                filters = filters.filters;
                for (var idx = 0, length = filters.length; idx < length; idx++) {
                    filter = filters[idx];
                    if (filter.field == this.field) {
                        found = true;
                    } else if (filter.filters) {
                        found = found || this._filterExist(filter);
                    }
                }
                return found;
            }
        });
        var template = '<ul id="#=uid#">' + '#if(sortable){#' + '<li class="k-item k-sort-asc"><span class="k-link"><span class="k-icon k-i-sort-asc-sm"></span>${messages.sortAscending}</span></li>' + '<li class="k-item k-sort-desc"><span class="k-link"><span class="k-icon k-i-sort-desc-sm"></span>${messages.sortDescending}</span></li>' + '#if(showColumns || filterable){#' + '<li class="k-separator" role="presentation"></li>' + '#}#' + '#}#' + '#if(showColumns){#' + '<li class="k-item k-columns-item" aria-haspopup="true"><span class="k-link"><span class="k-icon k-i-columns"></span>${messages.columns}</span><ul>' + '#for (var idx = 0; idx < columns.length; idx++) {#' + '<li role="menuitemcheckbox" aria-checked="false"><input type="checkbox" data-#=ns#field="#=columns[idx].field.replace(/"/g,"&\\#34;")#" data-#=ns#index="#=columns[idx].index#" data-#=ns#locked="#=columns[idx].locked#"/>#=columns[idx].title#</li>' + '#}#' + '</ul></li>' + '#if(filterable || lockedColumns){#' + '<li class="k-separator" role="presentation"></li>' + '#}#' + '#}#' + '#if(filterable){#' + '<li class="k-item k-filter-item" aria-haspopup="true"><span class="k-link"><span class="k-icon k-i-filter"></span>${messages.filter}</span><ul>' + '<li><div class="k-filterable"></div></li>' + '</ul></li>' + '#if(lockedColumns){#' + '<li class="k-separator" role="presentation"></li>' + '#}#' + '#}#' + '#if(lockedColumns){#' + '<li class="k-item k-lock"><span class="k-link"><span class="k-icon k-i-lock"></span>${messages.lock}</span></li>' + '<li class="k-item k-unlock"><span class="k-link"><span class="k-icon k-i-unlock"></span>${messages.unlock}</span></li>' + '#}#' + '</ul>';
        var mobileTemplate = '<div data-#=ns#role="view" data-#=ns#init-widgets="false" data-#=ns#use-native-scrolling="true" class="k-grid-column-menu">' + '<div data-#=ns#role="header" class="k-header">' + '${messages.settings}' + '<button class="k-button k-done">#=messages.done#</button>' + '</div>' + '<div class="k-column-menu k-mobile-list"><ul><li>' + '<span class="k-link">${title}</span><ul>' + '#if(sortable){#' + '<li class="k-item k-sort-asc"><span class="k-link"><span class="k-icon k-i-sort-asc-sm"></span>${messages.sortAscending}</span></li>' + '<li class="k-item k-sort-desc"><span class="k-link"><span class="k-icon k-i-sort-desc-sm"></span>${messages.sortDescending}</span></li>' + '#}#' + '#if(lockedColumns){#' + '<li class="k-item k-lock"><span class="k-link"><span class="k-icon k-i-lock"></span>${messages.lock}</span></li>' + '<li class="k-item k-unlock"><span class="k-link"><span class="k-icon k-i-unlock"></span>${messages.unlock}</span></li>' + '#}#' + '#if(filterable){#' + '<li class="k-item k-filter-item">' + '<span class="k-link k-filterable">' + '<span class="k-icon k-i-filter"></span>' + '${messages.filter}</span>' + '</li>' + '#}#' + '</ul></li>' + '#if(showColumns){#' + '<li class="k-columns-item"><span class="k-link">${messages.columns}</span><ul>' + '#for (var idx = 0; idx < columns.length; idx++) {#' + '<li class="k-item"><label class="k-label"><input type="checkbox" class="k-check" data-#=ns#field="#=columns[idx].field.replace(/"/g,"&\\#34;")#" data-#=ns#index="#=columns[idx].index#" data-#=ns#locked="#=columns[idx].locked#"/>#=columns[idx].title#</label></li>' + '#}#' + '</ul></li>' + '#}#' + '</ul></div>' + '</div>';
        var MobileMenu = Widget.extend({
            init: function (element, options) {
                Widget.fn.init.call(this, element, options);
                this.element.on('click' + NS, 'li.k-item:not(.k-separator):not(.k-state-disabled)', '_click');
            },
            events: [SELECT],
            _click: function (e) {
                if (!$(e.target).is('[type=checkbox]')) {
                    e.preventDefault();
                }
                this.trigger(SELECT, { item: e.currentTarget });
            },
            close: function () {
                this.options.pane.navigate('');
            },
            destroy: function () {
                Widget.fn.destroy.call(this);
                this.element.off(NS);
            }
        });
        ui.plugin(ColumnMenu);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));