/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('kendo.columnsorter', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'columnsorter',
        name: 'Column Sorter',
        category: 'framework',
        depends: ['core'],
        advanced: true
    };
    (function ($, undefined) {
        var kendo = window.kendo;
        var ui = kendo.ui;
        var Widget = ui.Widget;
        var DIR = 'dir';
        var ASC = 'asc';
        var SINGLE = 'single';
        var FIELD = 'field';
        var DESC = 'desc';
        var sorterNS = '.kendoColumnSorter';
        var TLINK = '.k-link';
        var ARIASORT = 'aria-sort';
        var proxy = $.proxy;
        var ColumnSorter = Widget.extend({
            init: function (element, options) {
                var that = this, link;
                Widget.fn.init.call(that, element, options);
                that._refreshHandler = proxy(that.refresh, that);
                that.dataSource = that.options.dataSource.bind('change', that._refreshHandler);
                that.directions = that.options.initialDirection === ASC ? [
                    ASC,
                    DESC
                ] : [
                    DESC,
                    ASC
                ];
                link = that.element.find(TLINK);
                if (!link[0]) {
                    link = that.element.wrapInner('<a class="k-link" href="#"/>').find(TLINK);
                }
                that.link = link;
                that.element.on('click' + sorterNS, proxy(that._click, that));
            },
            options: {
                name: 'ColumnSorter',
                mode: SINGLE,
                allowUnsort: true,
                compare: null,
                filter: '',
                initialDirection: ASC,
                showIndexes: false
            },
            events: ['change'],
            destroy: function () {
                var that = this;
                Widget.fn.destroy.call(that);
                that.element.off(sorterNS);
                that.dataSource.unbind('change', that._refreshHandler);
                that._refreshHandler = that.element = that.link = that.dataSource = null;
            },
            refresh: function () {
                var that = this, sort = that.dataSource.sort() || [], idx, length, descriptor, dir, element = that.element, field = element.attr(kendo.attr(FIELD)), headerIndex, sortOrder;
                element.removeAttr(kendo.attr(DIR));
                element.removeAttr(ARIASORT);
                for (idx = 0, length = sort.length; idx < length; idx++) {
                    descriptor = sort[idx];
                    if (field == descriptor.field) {
                        element.attr(kendo.attr(DIR), descriptor.dir);
                        sortOrder = idx + 1;
                    }
                }
                dir = element.attr(kendo.attr(DIR));
                if (element.is('th')) {
                    var table = element.closest('table');
                    if (table.parent().hasClass('k-grid-header-wrap')) {
                        table = table.closest('.k-grid').find('.k-grid-content > table');
                    } else if (!table.parent().hasClass('k-grid')) {
                        table = null;
                    }
                    if (table) {
                        headerIndex = element.parent().children(':visible').index(element);
                        element.toggleClass('k-sorted', dir !== undefined);
                        table.children('colgroup').children().eq(headerIndex).toggleClass('k-sorted', dir !== undefined);
                    }
                }
                element.find('.k-i-sort-asc-sm,.k-i-sort-desc-sm,.k-sort-order').remove();
                if (dir === ASC) {
                    $('<span class="k-icon k-i-sort-asc-sm" />').appendTo(that.link);
                    element.attr(ARIASORT, 'ascending');
                } else if (dir === DESC) {
                    $('<span class="k-icon k-i-sort-desc-sm" />').appendTo(that.link);
                    element.attr(ARIASORT, 'descending');
                }
                if (that.options.showIndexes && sort.length > 1 && sortOrder) {
                    $('<span class="k-sort-order" />').html(sortOrder).appendTo(that.link);
                }
            },
            _toggleSortDirection: function (dir) {
                var directions = this.directions;
                if (dir === directions[directions.length - 1] && this.options.allowUnsort) {
                    return undefined;
                }
                return directions[0] === dir ? directions[1] : directions[0];
            },
            _click: function (e) {
                var that = this, element = that.element, field = element.attr(kendo.attr(FIELD)), dir = element.attr(kendo.attr(DIR)), options = that.options, compare = that.options.compare === null ? undefined : that.options.compare, sort = that.dataSource.sort() || [], idx, length;
                e.preventDefault();
                if (options.filter && !element.is(options.filter)) {
                    return;
                }
                dir = this._toggleSortDirection(dir);
                if (this.trigger('change', {
                        sort: {
                            field: field,
                            dir: dir,
                            compare: compare
                        }
                    })) {
                    return;
                }
                if (options.mode === SINGLE) {
                    sort = [{
                            field: field,
                            dir: dir,
                            compare: compare
                        }];
                } else if (options.mode === 'multiple') {
                    for (idx = 0, length = sort.length; idx < length; idx++) {
                        if (sort[idx].field === field) {
                            sort.splice(idx, 1);
                            break;
                        }
                    }
                    sort.push({
                        field: field,
                        dir: dir,
                        compare: compare
                    });
                }
                if (this.dataSource.options.endless) {
                    this.dataSource.options.endless = null;
                    element.closest('.k-grid').getKendoGrid()._endlessPageSize = that.dataSource.options.pageSize;
                    this.dataSource.pageSize(that.dataSource.options.pageSize);
                }
                this.dataSource.sort(sort);
            }
        });
        ui.plugin(ColumnSorter);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));