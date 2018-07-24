/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('dropdowntree/treeview', ['kendo.treeview'], f);
}(function () {
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.ui, keys = kendo.keys, DISABLED = 'k-state-disabled', SELECT = 'select', CHECKED = 'checked', DATABOUND = 'dataBound', INDETERMINATE = 'indeterminate', NAVIGATE = 'navigate', subGroup, TreeView = ui.TreeView;
        function contentChild(filter) {
            return function (node) {
                var result = node.children('.k-animation-container');
                if (!result.length) {
                    result = node;
                }
                return result.children(filter);
            };
        }
        subGroup = contentChild('.k-group');
        var Tree = TreeView.extend({
            init: function (element, options, dropdowntree) {
                var that = this, clickableItems = '.k-in:not(.k-state-disabled)';
                that.dropdowntree = dropdowntree;
                TreeView.fn.init.call(that, element, options);
                if (this.dropdowntree._isMultipleSelection()) {
                    that.wrapper.on('click' + '.kendoTreeView', clickableItems, function (e) {
                        that._clickItem(e);
                    });
                }
            },
            _clickItem: function (e) {
                var node = $(e.currentTarget), dataItem = this.dataItem(node);
                dataItem.set('checked', !dataItem.checked);
            },
            defaultrefresh: function (e) {
                var node = e.node;
                var action = e.action;
                var items = e.items;
                var parentNode = this.wrapper;
                var options = this.options;
                var loadOnDemand = options.loadOnDemand;
                var checkChildren = options.checkboxes && options.checkboxes.checkChildren;
                var i;
                if (this._skip) {
                    return;
                }
                if (e.field) {
                    if (!items[0] || !items[0].level) {
                        return;
                    }
                    return this._updateNodes(items, e.field);
                }
                if (node) {
                    parentNode = this.findByUid(node.uid);
                    this._progress(parentNode, false);
                }
                if (checkChildren && action != 'remove') {
                    var bubble = false;
                    for (i = 0; i < items.length; i++) {
                        if ('checked' in items[i]) {
                            bubble = true;
                            break;
                        }
                    }
                    if (!bubble && node && node.checked) {
                        for (i = 0; i < items.length; i++) {
                            items[i].checked = true;
                        }
                    }
                }
                if (action == 'add') {
                    this._appendItems(e.index, items, parentNode);
                } else if (action == 'remove') {
                    this._remove(this.findByUid(items[0].uid), false);
                } else if (action == 'itemchange') {
                    this._updateNodes(items);
                } else if (action == 'itemloaded') {
                    this._refreshChildren(parentNode, items, e.index);
                } else {
                    this._refreshRoot(items);
                }
                if (action != 'remove') {
                    for (i = 0; i < items.length; i++) {
                        if (!loadOnDemand || items[i].expanded) {
                            items[i].load();
                        }
                    }
                }
                this.trigger(DATABOUND, { node: node ? parentNode : undefined });
                this.dropdowntree._treeViewDataBound({
                    node: node ? parentNode : undefined,
                    sender: this
                });
                if (this.options.checkboxes.checkChildren) {
                    this.updateIndeterminate();
                }
            },
            _previousVisible: function (node) {
                var that = this, lastChild, result;
                if (!node.length || node.prev().length) {
                    if (node.length) {
                        result = node.prev();
                    } else {
                        result = that.root.children().last();
                    }
                    while (that._expanded(result)) {
                        lastChild = subGroup(result).children().last();
                        if (!lastChild.length) {
                            break;
                        }
                        result = lastChild;
                    }
                } else {
                    result = that.parent(node) || node;
                    if (!result.length) {
                        if (that.dropdowntree.checkAll && that.dropdowntree.checkAll.is(':visible')) {
                            that.dropdowntree.checkAll.find('.k-checkbox').focus();
                        } else if (that.dropdowntree.filterInput) {
                            that.dropdowntree.filterInput.focus();
                        } else {
                            that.dropdowntree.wrapper.focus();
                        }
                    }
                }
                return result;
            },
            _keydown: function (e) {
                var that = this, key = e.keyCode, target, focused = that.current(), expanded = that._expanded(focused), checkbox = focused.find('.k-checkbox-wrapper:first :checkbox'), rtl = kendo.support.isRtl(that.element);
                if (e.target != e.currentTarget) {
                    return;
                }
                if (!rtl && key == keys.RIGHT || rtl && key == keys.LEFT) {
                    if (expanded) {
                        target = that._nextVisible(focused);
                    } else if (!focused.find('.k-in:first').hasClass(DISABLED)) {
                        that.expand(focused);
                    }
                } else if (!rtl && key == keys.LEFT || rtl && key == keys.RIGHT) {
                    if (expanded && !focused.find('.k-in:first').hasClass(DISABLED)) {
                        that.collapse(focused);
                    } else {
                        target = that.parent(focused);
                        if (!that._enabled(target)) {
                            target = undefined;
                        }
                    }
                } else if (key == keys.DOWN) {
                    target = that._nextVisible(focused);
                } else if (key == keys.UP && !e.altKey) {
                    target = that._previousVisible(focused);
                } else if (key == keys.HOME) {
                    target = that._nextVisible($());
                } else if (key == keys.END) {
                    target = that._previousVisible($());
                } else if (key == keys.ENTER && !focused.find('.k-in:first').hasClass(DISABLED)) {
                    if (!focused.find('.k-in:first').hasClass('k-state-selected')) {
                        if (!that._trigger(SELECT, focused)) {
                            that.select(focused);
                        }
                    }
                } else if (key == keys.SPACEBAR && checkbox.length && !focused.find('.k-in:first').hasClass(DISABLED)) {
                    checkbox.prop(CHECKED, !checkbox.prop(CHECKED)).data(INDETERMINATE, false).prop(INDETERMINATE, false);
                    that._checkboxChange({ target: checkbox });
                    target = focused;
                } else if (e.altKey && key === keys.UP || key === keys.ESC) {
                    that._closePopup();
                }
                if (target) {
                    e.preventDefault();
                    if (focused[0] != target[0]) {
                        that._trigger(NAVIGATE, target);
                        that.current(target);
                    }
                }
            },
            _closePopup: function () {
                this.dropdowntree.close();
                this.dropdowntree.wrapper.focus();
            },
            refresh: function (e) {
                this.defaultrefresh(e);
                if (this.dropdowntree.options.skipUpdateOnBind) {
                    return;
                }
                if (e.action === 'itemchange') {
                    if (this.dropdowntree._isMultipleSelection()) {
                        if (e.field === 'checked') {
                            this.dropdowntree._checkValue(e.items[0]);
                        }
                    } else {
                        if (e.field !== 'checked' && e.field !== 'expanded' && e.items[0].selected) {
                            this.dropdowntree._selectValue(e.items[0]);
                        }
                    }
                } else {
                    this.dropdowntree.refresh(e);
                }
            }
        });
        kendo.ui._dropdowntree = Tree;
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));
(function (f, define) {
    define('kendo.dropdowntree', [
        'dropdowntree/treeview',
        'kendo.popup'
    ], f);
}(function () {
    var __meta__ = {
        id: 'dropdowntree',
        name: 'DropDownTree',
        category: 'web',
        description: 'The DropDownTree widget displays a hierarchy of items and allows the selection of single or multiple items.',
        depends: [
            'treeview',
            'popup'
        ]
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.ui, Widget = ui.Widget, TreeView = ui._dropdowntree, ObservableArray = kendo.data.ObservableArray, ObservableObject = kendo.data.ObservableObject, extend = $.extend, activeElement = kendo._activeElement, ns = '.kendoDropDownTree', keys = kendo.keys, support = kendo.support, HIDDENCLASS = 'k-hidden', WIDTH = 'width', browser = support.browser, outerWidth = kendo._outerWidth, DOT = '.', DISABLED = 'disabled', READONLY = 'readonly', STATEDISABLED = 'k-state-disabled', ARIA_DISABLED = 'aria-disabled', HOVER = 'k-state-hover', FOCUSED = 'k-state-focused', HOVEREVENTS = 'mouseenter' + ns + ' mouseleave' + ns, TABINDEX = 'tabindex', CLICK = 'click', OPEN = 'open', CLOSE = 'close', CHANGE = 'change', proxy = $.proxy;
        var DropDownTree = kendo.ui.Widget.extend({
            init: function (element, options) {
                this.ns = ns;
                kendo.ui.Widget.fn.init.call(this, element, options);
                this._selection = this._getSelection();
                this._focusInputHandler = $.proxy(this._focusInput, this);
                this._initial = this.element.val();
                this._values = [];
                var value = this.options.value;
                if (value) {
                    this._values = $.isArray(value) ? value.slice(0) : [value];
                }
                this._inputTemplate();
                this._accessors();
                this._setTreeViewOptions(this.options);
                this._dataSource();
                this._selection._initWrapper();
                this._placeholder(true);
                this._tabindex();
                this.wrapper.data(TABINDEX, this.wrapper.attr(TABINDEX));
                this.tree = $('<div/>').attr({
                    tabIndex: -1,
                    'aria-hidden': true
                });
                this.list = $('<div class=\'k-list-container\'/>').append(this.tree);
                this._header();
                this._noData();
                this._footer();
                this._reset();
                this._popup();
                this.popup.one('open', proxy(this._popupOpen, this));
                this._clearButton();
                this._filterHeader();
                this._treeview();
                this._renderFooter();
                this._checkAll();
                this._enable();
                this._toggleCloseVisibility();
                if (!this.options.autoBind) {
                    var text = options.text || '';
                    if (options.value) {
                        this._preselect(options.value);
                    } else if (text) {
                        this._textAccessor(text);
                    } else if (options.placeholder) {
                        this._placeholder(true);
                    }
                }
                var disabled = $(this.element).parents('fieldset').is(':disabled');
                if (disabled) {
                    this.enable(false);
                }
                kendo.notify(this);
            },
            _preselect: function (data, value) {
                this._selection._preselect(data, value);
            },
            _setTreeViewOptions: function (options) {
                var treeviewOptions = {
                    autoBind: options.autoBind,
                    checkboxes: options.checkboxes,
                    dataImageUrlField: options.dataImageUrlField,
                    dataSpriteCssClassField: options.dataSpriteCssClassField,
                    dataTextField: options.dataTextField,
                    dataUrlField: options.dataUrlField,
                    loadOnDemand: options.loadOnDemand
                };
                this.options.treeview = $.extend({}, treeviewOptions, this.options.treeview);
                if (options.template) {
                    this.options.treeview.template = options.template;
                }
            },
            _dataSource: function () {
                var rootDataSource = this.options.dataSource;
                this.dataSource = kendo.data.HierarchicalDataSource.create(rootDataSource);
                if (rootDataSource) {
                    $.extend(this.options.treeview, { dataSource: this.dataSource });
                }
            },
            _popupOpen: function () {
                var popup = this.popup;
                popup.wrapper = kendo.wrap(popup.element);
            },
            _getSelection: function () {
                if (this._isMultipleSelection()) {
                    return new ui.DropDownTree.MultipleSelection(this);
                } else {
                    return new ui.DropDownTree.SingleSelection(this);
                }
            },
            setDataSource: function (dataSource) {
                this.dataSource = dataSource;
                this.treeview.setDataSource(dataSource);
            },
            _isMultipleSelection: function () {
                return this.options && (this.options.treeview.checkboxes || this.options.checkboxes);
            },
            options: {
                name: 'DropDownTree',
                animation: {},
                autoBind: true,
                autoClose: true,
                autoWidth: false,
                clearButton: true,
                dataTextField: '',
                dataValueField: '',
                dataImageUrlField: '',
                dataSpriteCssClassField: '',
                dataUrlField: '',
                delay: 500,
                enabled: true,
                enforceMinLength: false,
                filter: 'none',
                height: 200,
                ignoreCase: true,
                index: 0,
                loadOnDemand: false,
                messages: {
                    'singleTag': 'item(s) selected',
                    'clear': 'clear',
                    'deleteTag': 'delete'
                },
                minLength: 1,
                checkboxes: false,
                noDataTemplate: 'No data found.',
                placeholder: '',
                checkAllTemplate: 'Check all',
                tagMode: 'multiple',
                template: null,
                text: null,
                treeview: {},
                valuePrimitive: false,
                footerTemplate: '',
                headerTemplate: '',
                value: null,
                valueTemplate: null
            },
            events: [
                'open',
                'close',
                'dataBound',
                CHANGE,
                'filtering'
            ],
            focus: function () {
                this.wrapper.focus();
            },
            readonly: function (readonly) {
                this._editable({
                    readonly: readonly === undefined ? true : readonly,
                    disable: false
                });
            },
            enable: function (enable) {
                this._editable({
                    readonly: false,
                    disable: !(enable = enable === undefined ? true : enable)
                });
            },
            toggle: function (open) {
                this._toggle(open);
            },
            open: function () {
                var popup = this.popup;
                if (popup.visible() || !this._allowOpening()) {
                    return;
                }
                if (this._isMultipleSelection()) {
                    popup.element.addClass('k-multiple-selection');
                }
                popup.element.addClass('k-popup-dropdowntree');
                popup.one('activate', this._focusInputHandler);
                popup._hovered = true;
                popup.open();
            },
            close: function () {
                this.popup.close();
            },
            search: function (word) {
                var options = this.options;
                var filter;
                clearTimeout(this._typingTimeout);
                if (!options.enforceMinLength && !word.length || word.length >= options.minLength) {
                    filter = this._getFilter(word);
                    if (this.trigger('filtering', { filter: filter }) || $.isArray(this.options.dataTextField)) {
                        return;
                    }
                    this._filtering = true;
                    this.treeview.dataSource.filter(filter);
                }
            },
            _getFilter: function (word) {
                return {
                    field: this.options.dataTextField,
                    operator: this.options.filter,
                    value: word,
                    ignoreCase: this.options.ignoreCase
                };
            },
            refresh: function () {
                var data = this.treeview.dataSource.flatView();
                this._renderFooter();
                this._renderNoData();
                if (this.filterInput && this.checkAll) {
                    this.checkAll.toggle(!this.filterInput.val().length);
                }
                this.tree.toggle(!!data.length);
                $(this.noData).toggle(!data.length);
            },
            setOptions: function (options) {
                Widget.fn.setOptions.call(this, options);
                this._setTreeViewOptions(options);
                this._dataSource();
                if (this.options.treeview) {
                    this.treeview.setOptions(this.options.treeview);
                }
                if (options.height && this.tree) {
                    this.tree.css('max-height', options.height);
                }
                this._header();
                this._noData();
                this._footer();
                this._renderFooter();
                this._renderNoData();
                if (this.span && (this._isMultipleSelection() || this.span.hasClass('k-readonly'))) {
                    this._placeholder(true);
                }
                this._inputTemplate();
                this._accessors();
                this._filterHeader();
                this._checkAll();
                this._enable();
                if (options && (options.enable || options.enabled)) {
                    this.enable(true);
                }
                this._clearButton();
            },
            destroy: function () {
                kendo.ui.Widget.fn.destroy.call(this);
                if (this.treeview) {
                    this.treeview.destroy();
                }
                this.popup.destroy();
                this.wrapper.off(ns);
                this._clear.off(ns);
                this._inputWrapper.off(ns);
                if (this.filterInput) {
                    this.filterInput.off(ns);
                }
                if (this.tagList) {
                    this.tagList.off(ns);
                }
                kendo.unbind(this.tagList);
                if (this.options.checkAll && this.checkAll) {
                    this.checkAll.off(ns);
                }
                if (this._form) {
                    this._form.off('reset', this._resetHandler);
                }
            },
            setValue: function (value) {
                value = $.isArray(value) || value instanceof ObservableArray ? value.slice(0) : [value];
                this._values = value;
            },
            items: function () {
                this.treeview.dataItems();
            },
            value: function (value) {
                if (value) {
                    if (this.filterInput && this.dataSource._filter) {
                        this._filtering = true;
                        this.dataSource.filter({});
                    } else if (!this.dataSource.data().length) {
                        this.dataSource.fetch();
                    }
                }
                return this._selection._setValue(value);
            },
            text: function (text) {
                var loweredText;
                var ignoreCase = this.options.ignoreCase;
                text = text === null ? '' : text;
                if (text !== undefined && !this._isMultipleSelection()) {
                    if (typeof text !== 'string') {
                        this._textAccessor(text);
                        return;
                    }
                    loweredText = ignoreCase ? text : text.toLowerCase();
                    this._selectItemByText(loweredText);
                    this._textAccessor(loweredText);
                } else {
                    return this._textAccessor();
                }
            },
            _header: function () {
                var list = this;
                var header = $(list.header);
                var template = list.options.headerTemplate;
                this._angularElement(header, 'cleanup');
                kendo.destroy(header);
                header.remove();
                if (!template) {
                    list.header = null;
                    return;
                }
                var headerTemplate = typeof template !== 'function' ? kendo.template(template) : template;
                header = $(headerTemplate({}));
                list.header = header[0] ? header : null;
                list.list.prepend(header);
                this._angularElement(list.header, 'compile');
            },
            _noData: function () {
                var list = this;
                var noData = $(list.noData);
                var template = list.options.noDataTemplate;
                list.angular('cleanup', function () {
                    return { elements: noData };
                });
                kendo.destroy(noData);
                noData.remove();
                if (!template) {
                    list.noData = null;
                    return;
                }
                list.noData = $('<div class="k-nodata" style="display:none"><div></div></div>').appendTo(list.list);
                list.noDataTemplate = typeof template !== 'function' ? kendo.template(template) : template;
            },
            _renderNoData: function () {
                var list = this;
                var noData = list.noData;
                if (!noData) {
                    return;
                }
                this._angularElement(noData, 'cleanup');
                noData.children(':first').html(list.noDataTemplate({ instance: list }));
                this._angularElement(noData, 'compile');
            },
            _footer: function () {
                var list = this;
                var footer = $(list.footer);
                var template = list.options.footerTemplate;
                this._angularElement(footer, 'cleanup');
                kendo.destroy(footer);
                footer.remove();
                if (!template) {
                    list.footer = null;
                    return;
                }
                list.footer = $('<div class="k-footer"></div>').appendTo(list.list);
                list.footerTemplate = typeof template !== 'function' ? kendo.template(template) : template;
            },
            _renderFooter: function () {
                var list = this;
                var footer = list.footer;
                if (!footer) {
                    return;
                }
                this._angularElement(footer, 'cleanup');
                footer.html(list.footerTemplate({ instance: list }));
                this._angularElement(footer, 'compile');
            },
            _enable: function () {
                var that = this, options = that.options, disabled = that.element.is('[disabled]');
                if (options.enable !== undefined) {
                    options.enabled = options.enable;
                }
                if (!options.enabled || disabled) {
                    that.enable(false);
                } else {
                    that.readonly(that.element.is('[readonly]'));
                }
            },
            _adjustListWidth: function () {
                var that = this, list = that.list, width = list[0].style.width, wrapper = that.wrapper, computedStyle, computedWidth;
                if (!list.data(WIDTH) && width) {
                    return;
                }
                computedStyle = window.getComputedStyle ? window.getComputedStyle(wrapper[0], null) : 0;
                computedWidth = parseFloat(computedStyle && computedStyle.width) || outerWidth(wrapper);
                if (computedStyle && browser.msie) {
                    computedWidth += parseFloat(computedStyle.paddingLeft) + parseFloat(computedStyle.paddingRight) + parseFloat(computedStyle.borderLeftWidth) + parseFloat(computedStyle.borderRightWidth);
                }
                if (list.css('box-sizing') !== 'border-box') {
                    width = computedWidth - (outerWidth(list) - list.width());
                } else {
                    width = computedWidth;
                }
                list.css({
                    fontFamily: wrapper.css('font-family'),
                    width: that.options.autoWidth ? 'auto' : width,
                    minWidth: width,
                    whiteSpace: that.options.autoWidth ? 'nowrap' : 'normal'
                }).data(WIDTH, width);
                return true;
            },
            _reset: function () {
                var that = this, element = that.element, formId = element.attr('form'), form = formId ? $('#' + formId) : element.closest('form');
                if (form[0]) {
                    that._resetHandler = function () {
                        setTimeout(function () {
                            that.value(that._initial);
                        });
                    };
                    that._form = form.on('reset', that._resetHandler);
                }
            },
            _popup: function () {
                var list = this;
                list.popup = new ui.Popup(list.list, extend({}, list.options.popup, {
                    anchor: list.wrapper,
                    open: proxy(list._openHandler, list),
                    close: proxy(list._closeHandler, list),
                    animation: list.options.animation,
                    isRtl: support.isRtl(list.wrapper),
                    autosize: list.options.autoWidth
                }));
            },
            _angularElement: function (element, action) {
                if (!element) {
                    return;
                }
                this.angular(action, function () {
                    return { elements: element };
                });
            },
            _allowOpening: function () {
                return this.options.noDataTemplate || this.treeview.dataSource.flatView().length;
            },
            _placeholder: function (show) {
                if (this.span) {
                    this.span.toggleClass('k-readonly', show).text(show ? this.options.placeholder : '');
                }
            },
            _currentValue: function (dataItem) {
                var currentValue = this._value(dataItem);
                if (!currentValue) {
                    currentValue = dataItem;
                }
                return currentValue;
            },
            _checkValue: function (dataItem) {
                var value = '';
                var text = '';
                var indexOfValue = -1;
                var currentValue = this.value();
                var isMultiple = this.options.tagMode === 'multiple';
                if (dataItem || dataItem === 0) {
                    if (dataItem.level) {
                        dataItem._level = dataItem.level();
                    }
                    text = this._text(dataItem);
                    value = this._currentValue(dataItem);
                    indexOfValue = currentValue.indexOf(value);
                }
                if (dataItem.checked) {
                    var itemToAdd = new ObservableObject(dataItem.toJSON());
                    dataItem._tagUid = itemToAdd.uid;
                    this._tags.push(itemToAdd);
                    if (this._tags.length === 1) {
                        this.span.hide();
                        if (!isMultiple) {
                            this._multipleTags.push(itemToAdd);
                        }
                    }
                    if (indexOfValue === -1) {
                        currentValue.push(value);
                        this.setValue(currentValue);
                    }
                } else {
                    var itemToRemove = this._tags.find(function (item) {
                        return item.uid === dataItem._tagUid;
                    });
                    var idx = this._tags.indexOf(itemToRemove);
                    if (idx !== -1) {
                        this._tags.splice(idx, 1);
                    } else {
                        this._treeViewCheckAllCheck(dataItem);
                        return;
                    }
                    if (this._tags.length === 0) {
                        this.span.show();
                        if (!isMultiple) {
                            this._multipleTags.splice(0, 1);
                        }
                    }
                    if (indexOfValue !== -1) {
                        currentValue.splice(indexOfValue, 1);
                        this.setValue(currentValue);
                    }
                }
                this._treeViewCheckAllCheck(dataItem);
                if (!this._preventChangeTrigger) {
                    this.trigger(CHANGE);
                }
                if (this.options.autoClose && this.popup.visible()) {
                    this.close();
                    this.wrapper.focus();
                }
                this.popup.position();
                this._toggleCloseVisibility();
            },
            _selectValue: function (dataItem) {
                var value = '';
                var text = '';
                if (dataItem || dataItem === 0) {
                    if (dataItem.level) {
                        dataItem._level = dataItem.level();
                    }
                    text = this._text(dataItem);
                    value = this._currentValue(dataItem);
                }
                if (value === null) {
                    value = '';
                }
                this.setValue(value);
                this._textAccessor(text, dataItem);
                this._accessor(value);
                this.trigger(CHANGE);
                if (this.options.autoClose && this.popup.visible()) {
                    this.close();
                    this.wrapper.focus();
                }
                this.popup.position();
                this._toggleCloseVisibility();
            },
            _clearClick: function (e) {
                e.stopPropagation();
                this._clearTextAndValue();
            },
            _clearTextAndValue: function () {
                this.setValue([]);
                this._clearInput();
                this._clearText();
                this._selection._clearValue();
                this.popup.position();
                this._toggleCloseVisibility();
            },
            _clearText: function () {
                if (this.options.placeholder) {
                    this._placeholder(true);
                } else {
                    if (this.span) {
                        this.span.html('');
                    }
                }
            },
            _inputTemplate: function () {
                var template = this.options.valueTemplate;
                if (!template) {
                    template = $.proxy(kendo.template('#:this._text(data)#', { useWithBlock: false }), this);
                } else {
                    template = kendo.template(template);
                }
                this.valueTemplate = template;
            },
            _assignInstance: function (text, value) {
                var dataTextField = this.options.dataTextField;
                var dataItem = {};
                if (dataTextField) {
                    assign(dataItem, dataTextField.split(DOT), text);
                    assign(dataItem, this.options.dataValueField.split(DOT), value);
                    dataItem = new ObservableObject(dataItem);
                } else {
                    dataItem = text;
                }
                return dataItem;
            },
            _textAccessor: function (text, dataItem) {
                var valueTemplate = this.valueTemplate;
                var span = this.span;
                if (text === undefined) {
                    return span.text();
                }
                span.removeClass('k-readonly');
                if (!dataItem && ($.isPlainObject(text) || text instanceof ObservableObject)) {
                    dataItem = text;
                }
                if (!dataItem) {
                    dataItem = this._assignInstance(text, this._accessor());
                }
                var getElements = function () {
                    return {
                        elements: span.get(),
                        data: [{ dataItem: dataItem }]
                    };
                };
                this.angular('cleanup', getElements);
                try {
                    span.html(valueTemplate(dataItem));
                } catch (e) {
                    if (span) {
                        span.html('');
                    }
                }
                this.angular('compile', getElements);
            },
            _accessors: function () {
                var element = this.element;
                var options = this.options;
                var getter = kendo.getter;
                var textField = element.attr(kendo.attr('text-field'));
                var valueField = element.attr(kendo.attr('value-field'));
                var getterFunction = function (field) {
                    if ($.isArray(field)) {
                        var count = field.length;
                        var levels = $.map(field, function (x) {
                            return function (d) {
                                return d[x];
                            };
                        });
                        return function (dataItem) {
                            var level = dataItem._level;
                            if (!level && level !== 0) {
                                return;
                            }
                            return levels[Math.min(level, count - 1)](dataItem);
                        };
                    } else {
                        return getter(field);
                    }
                };
                if (!options.dataTextField && textField) {
                    options.dataTextField = textField;
                }
                if (!options.dataValueField && valueField) {
                    options.dataValueField = valueField;
                }
                options.dataTextField = options.dataTextField || 'text';
                options.dataValueField = options.dataValueField || 'value';
                this._text = getterFunction(options.dataTextField);
                this._value = getterFunction(options.dataValueField);
            },
            _accessor: function (value, idx) {
                return this[this._isSelect ? '_accessorSelect' : '_accessorInput'](value, idx);
            },
            _accessorInput: function (value) {
                var element = this.element[0];
                if (value === undefined) {
                    return element.value;
                } else {
                    if (value === null) {
                        value = '';
                    }
                    element.value = value;
                }
            },
            _clearInput: function () {
                var element = this.element[0];
                element.value = '';
            },
            _clearButton: function () {
                var clearTitle = this.options.messages && this.options.messages.clear ? this.options.messages.clear : 'clear';
                if (!this._clear) {
                    this._clear = $('<span unselectable="on" class="k-icon k-clear-value k-i-close" title="' + clearTitle + '"></span>').attr({
                        'role': 'button',
                        'tabIndex': -1
                    });
                }
                if (this.options.clearButton) {
                    this._clear.insertAfter(this.span);
                    this.wrapper.addClass('k-dropdowntree-clearable');
                } else {
                    if (!this.options.clearButton) {
                        this._clear.remove();
                    }
                }
            },
            _toggleCloseVisibility: function () {
                if (this.value() && !this._isMultipleSelection() || this.value().length || this.element.val() && this.element.val() !== this.options.placeholder) {
                    this._showClear();
                } else {
                    this._hideClear();
                }
            },
            _showClear: function () {
                if (this._clear) {
                    this._clear.removeClass(HIDDENCLASS);
                }
            },
            _hideClear: function () {
                if (this._clear) {
                    this._clear.addClass(HIDDENCLASS);
                }
            },
            _openHandler: function (e) {
                this._adjustListWidth();
                if (this.trigger(OPEN)) {
                    e.preventDefault();
                } else {
                    this.wrapper.attr('aria-expanded', true);
                    this.tree.attr('aria-hidden', false).attr('role', 'tree');
                }
            },
            _closeHandler: function (e) {
                if (this.trigger(CLOSE)) {
                    e.preventDefault();
                } else {
                    this.wrapper.attr('aria-expanded', false);
                    this.tree.attr('aria-hidden', true);
                }
            },
            _treeview: function () {
                if (this.options.height) {
                    this.tree.css('max-height', this.options.height);
                }
                this.tree.attr('id', kendo.guid());
                this.treeview = new TreeView(this.tree, extend({}, this.options.treeview), this);
                this.dataSource = this.treeview.dataSource;
            },
            _treeViewDataBound: function (e) {
                if (e.node && this._prev && this._prev.length) {
                    e.sender.expand(e.node);
                }
                if (this._filtering) {
                    if (!e.node) {
                        this._filtering = false;
                    }
                    if (!this._isMultipleSelection()) {
                        this._deselectItem(e);
                    }
                    return;
                }
                if (!this.treeview) {
                    this.treeview = e.sender;
                }
                if (!e.node) {
                    var rootItems = e.sender.dataSource.data();
                    this._checkLoadedItems(rootItems);
                } else {
                    var rootItem = e.sender.dataItem(e.node);
                    if (rootItem) {
                        var subItems = rootItem.children.data();
                        this._checkLoadedItems(subItems);
                    }
                }
                this.trigger('dataBound', e);
            },
            _deselectItem: function (e) {
                var items = [];
                if (!e.node) {
                    items = e.sender.dataSource.data();
                } else {
                    var rootItem = e.sender.dataItem(e.node);
                    if (rootItem) {
                        items = rootItem.children.data();
                    }
                }
                for (var i = 0; i < items.length; i++) {
                    if (items[i].selected && !this._valueComparer(items[i], this.value())) {
                        items[i].set('selected', false);
                    }
                }
            },
            _checkLoadedItems: function (items) {
                var value = this.value();
                var length = value.length;
                if (!items) {
                    return;
                }
                for (var idx = 0; idx < items.length; idx++) {
                    this._selection._checkLoadedItem(items[idx], value, length);
                }
            },
            _treeViewCheckAllCheck: function (dataItem) {
                if (this.options.checkAll && this.checkAll) {
                    this._getAllChecked();
                    if (dataItem.checked) {
                        this._checkCheckAll();
                    } else {
                        this._uncheckCheckAll();
                    }
                }
            },
            _checkCheckAll: function () {
                var checkAllCheckbox = this.checkAll.find('.k-checkbox');
                if (this._allItemsAreChecked) {
                    checkAllCheckbox.prop('checked', true).prop('indeterminate', false);
                } else {
                    checkAllCheckbox.prop('indeterminate', true);
                }
            },
            _uncheckCheckAll: function () {
                var checkAllCheckbox = this.checkAll.find('.k-checkbox');
                if (this._allItemsAreUnchecked) {
                    checkAllCheckbox.prop('checked', false).prop('indeterminate', false);
                } else {
                    checkAllCheckbox.prop('indeterminate', true);
                }
            },
            _filterHeader: function () {
                var icon;
                if (this.filterInput) {
                    this.filterInput.off(ns).parent().remove();
                    this.filterInput = null;
                }
                if (this._isFilterEnabled()) {
                    this._disableCheckChildren();
                    icon = '<span class="k-icon k-i-zoom"></span>';
                    this.filterInput = $('<input class="k-textbox"/>').attr({
                        placeholder: this.element.attr('placeholder'),
                        title: this.element.attr('title'),
                        role: 'listbox',
                        'aria-haspopup': true,
                        'aria-expanded': false
                    });
                    this.filterInput.on('input', proxy(this._filterChange, this));
                    $('<span class="k-list-filter" />').insertBefore(this.tree).append(this.filterInput.add(icon));
                }
            },
            _filterChange: function () {
                if (this.filterInput) {
                    this._search();
                }
            },
            _disableCheckChildren: function () {
                if (this._isMultipleSelection() && this.options.treeview.checkboxes && this.options.treeview.checkboxes.checkChildren) {
                    this.options.treeview.checkboxes.checkChildren = false;
                }
            },
            _checkAll: function () {
                if (this.checkAll) {
                    this.checkAll.find('.k-checkbox-label, .k-checkbox').off(ns);
                    this.checkAll.remove();
                    this.checkAll = null;
                }
                if (this._isMultipleSelection() && this.options.checkAll) {
                    this.checkAll = $('<div class="k-check-all"><input type="checkbox" class="k-checkbox"/><span class="k-checkbox-label">Check All</span></div>').insertBefore(this.tree);
                    this.checkAll.find('.k-checkbox-label').html(kendo.template(this.options.checkAllTemplate)({ instance: this }));
                    this.checkAll.find('.k-checkbox-label').on(CLICK + ns, proxy(this._clickCheckAll, this));
                    this.checkAll.find('.k-checkbox').on('change' + ns, proxy(this._changeCheckAll, this)).on('keydown' + ns, proxy(this._keydownCheckAll, this));
                    this._disabledCheckedItems = [];
                    this._disabledUnCheckedItems = [];
                    this._getAllChecked();
                    if (!this._allItemsAreUnchecked) {
                        this._checkCheckAll();
                    }
                }
            },
            _changeCheckAll: function () {
                var checkAllCheckbox = this.checkAll.find('.k-checkbox');
                var isChecked = checkAllCheckbox.prop('checked');
                if (!browser.msie && !browser.edge) {
                    this._updateCheckAll(isChecked);
                }
            },
            _updateCheckAll: function (isChecked) {
                var checkAllCheckbox = this.checkAll.find('.k-checkbox');
                this._toggleCheckAllItems(isChecked);
                checkAllCheckbox.prop('checked', isChecked);
                if (this._disabledCheckedItems.length && this._disabledUnCheckedItems.length) {
                    checkAllCheckbox.prop('indeterminate', true);
                } else if (this._disabledCheckedItems.length) {
                    checkAllCheckbox.prop('indeterminate', !isChecked);
                } else if (this._disabledUnCheckedItems.length) {
                    checkAllCheckbox.prop('indeterminate', isChecked);
                } else {
                    checkAllCheckbox.prop('indeterminate', false);
                }
                this._disabledCheckedItems = [];
                this._disabledUnCheckedItems = [];
            },
            _keydownCheckAll: function (e) {
                var key = e.keyCode;
                var altKey = e.altKey;
                if (altKey && key === keys.UP || key === keys.ESC) {
                    this.close();
                    this.wrapper.focus();
                    e.preventDefault();
                    return;
                }
                if (key === keys.UP) {
                    if (this.filterInput) {
                        this.filterInput.focus();
                    } else {
                        this.wrapper.focus();
                    }
                    e.preventDefault();
                }
                if (key === keys.DOWN) {
                    if (this.tree && this.tree.is(':visible')) {
                        this.tree.focus();
                    }
                    e.preventDefault();
                }
                if (key === keys.SPACEBAR && (browser.msie || browser.edge)) {
                    this._clickCheckAll();
                    e.preventDefault();
                }
            },
            _clickCheckAll: function () {
                var checkAllCheckbox = this.checkAll.find('.k-checkbox');
                var isChecked = checkAllCheckbox.prop('checked');
                this._updateCheckAll(!isChecked);
                checkAllCheckbox.focus();
            },
            _dfs: function (items, action, prop) {
                for (var idx = 0; idx < items.length; idx++) {
                    if (!this[action](items[idx], prop)) {
                        break;
                    }
                    this._traverceChildren(items[idx], action, prop);
                }
            },
            _uncheckItemByUid: function (uid) {
                this._dfs(this.dataSource.data(), '_uncheckItemEqualsUid', uid);
            },
            _uncheckItemEqualsUid: function (item, uid) {
                if (item.enabled !== false && item._tagUid == uid) {
                    item.set('checked', false);
                    return false;
                }
                return true;
            },
            _selectItemByText: function (text) {
                this._dfs(this.dataSource.data(), '_itemEqualsText', text);
            },
            _itemEqualsText: function (item, text) {
                if (item.enabled !== false && this._text(item) === text) {
                    this.treeview.select(this.treeview.findByUid(item.uid));
                    this._selectValue(item);
                    return false;
                }
                return true;
            },
            _selectItemByValue: function (value) {
                this._dfs(this.dataSource.data(), '_itemEqualsValue', value);
            },
            _itemEqualsValue: function (item, value) {
                if (item.enabled !== false && this._valueComparer(item, value)) {
                    this.treeview.select(this.treeview.findByUid(item.uid));
                    this._selectValue(item);
                    return false;
                }
                return true;
            },
            _checkItemByValue: function (value) {
                var items = this.treeview.dataItems();
                for (var idx = 0; idx < value.length; idx++) {
                    this._dfs(items, '_checkItemEqualsValue', value[idx]);
                }
            },
            _checkItemEqualsValue: function (item, value) {
                if (item.enabled !== false && this._valueComparer(item, value)) {
                    item.set('checked', true);
                    return false;
                }
                return true;
            },
            _valueComparer: function (item, value) {
                var itemValue = this._value(item);
                var itemText;
                if (itemValue) {
                    if (!value) {
                        return false;
                    }
                    var newValue = this._value(value);
                    if (newValue) {
                        return itemValue == newValue;
                    } else {
                        return itemValue == value;
                    }
                }
                itemText = this._text(item);
                if (itemText) {
                    if (this._text(value)) {
                        return itemText == this._text(value);
                    } else {
                        return itemText == value;
                    }
                }
                return false;
            },
            _getAllChecked: function () {
                this._allCheckedItems = [];
                this._allItemsAreChecked = true;
                this._allItemsAreUnchecked = true;
                this._dfs(this.dataSource.data(), '_getAllCheckedItems');
                return this._allCheckedItems;
            },
            _getAllCheckedItems: function (item) {
                if (this._allItemsAreChecked) {
                    this._allItemsAreChecked = item.checked;
                }
                if (this._allItemsAreUnchecked) {
                    this._allItemsAreUnchecked = !item.checked;
                }
                if (item.checked) {
                    this._allCheckedItems.push(item);
                }
                return true;
            },
            _traverceChildren: function (item, action, prop) {
                var childrenField = item._childrenOptions && item._childrenOptions.schema ? item._childrenOptions.schema.data : null;
                var subItems = item[childrenField] || item.items || item.children;
                if (!subItems) {
                    return;
                }
                this._dfs(subItems, action, prop);
            },
            _toggleCheckAllItems: function (checked) {
                this._dfs(this.dataSource.data(), '_checkAllCheckItem', checked);
            },
            _checkAllCheckItem: function (item, checked) {
                if (item.enabled === false) {
                    if (item.checked) {
                        this._disabledCheckedItems.push(item);
                    } else {
                        this._disabledUnCheckedItems.push(item);
                    }
                } else {
                    item.set('checked', checked);
                }
                return true;
            },
            _isFilterEnabled: function () {
                var filter = this.options.filter;
                return filter && filter !== 'none';
            },
            _editable: function (options) {
                var that = this;
                var element = that.element;
                var disable = options.disable;
                var readonly = options.readonly;
                var wrapper = that.wrapper.add(that.filterInput).off(ns);
                var dropDownWrapper = that._inputWrapper.off(HOVEREVENTS);
                if (!readonly && !disable) {
                    element.removeAttr(DISABLED).removeAttr(READONLY);
                    dropDownWrapper.removeClass(STATEDISABLED).on(HOVEREVENTS, that._toggleHover);
                    that._clear.on('click' + ns, proxy(that._clearClick, that));
                    wrapper.attr(TABINDEX, wrapper.data(TABINDEX)).attr(ARIA_DISABLED, false).on('keydown' + ns, proxy(that._keydown, that)).on('focusin' + ns, proxy(that._focusinHandler, that)).on('focusout' + ns, proxy(that._focusoutHandler, that));
                    that.wrapper.on(CLICK + ns, proxy(that._wrapperClick, that));
                    if (this._isMultipleSelection()) {
                        that.tagList.on(CLICK + ns, 'li.k-button', function (e) {
                            $(e.currentTarget).addClass(FOCUSED);
                        });
                        that.tagList.on(CLICK + ns, '.k-select', function (e) {
                            that._removeTagClick(e);
                        });
                    }
                } else if (disable) {
                    wrapper.removeAttr(TABINDEX);
                    dropDownWrapper.addClass(STATEDISABLED);
                } else {
                    dropDownWrapper.removeClass(STATEDISABLED);
                    wrapper.on('focusin' + ns, proxy(that._focusinHandler, that)).on('focusout' + ns, proxy(that._focusoutHandler, that));
                }
                element.attr(DISABLED, disable).attr(READONLY, readonly);
                wrapper.attr(ARIA_DISABLED, disable);
            },
            _focusinHandler: function () {
                this._inputWrapper.addClass(FOCUSED);
            },
            _focusoutHandler: function () {
                this._inputWrapper.removeClass(FOCUSED);
                if (this._isMultipleSelection()) {
                    this.tagList.find(DOT + FOCUSED).removeClass(FOCUSED);
                }
            },
            _toggle: function (open) {
                open = open !== undefined ? open : !this.popup.visible();
                this[open ? OPEN : CLOSE]();
            },
            _wrapperClick: function (e) {
                e.preventDefault();
                this.popup.unbind('activate', this._focusInputHandler);
                this._focused = this.wrapper;
                this._prevent = false;
                this._toggle();
            },
            _toggleHover: function (e) {
                $(e.currentTarget).toggleClass(HOVER, e.type === 'mouseenter');
            },
            _focusInput: function () {
                if (this.filterInput) {
                    this.filterInput.focus();
                } else if (this.checkAll) {
                    this.checkAll.find('.k-checkbox').focus();
                } else if (this.tree.is(':visible')) {
                    this.tree.focus();
                }
            },
            _keydown: function (e) {
                var key = e.keyCode;
                var altKey = e.altKey;
                var isFilterInputActive;
                var isWrapperActive;
                var focused, tagItem;
                var isPopupVisible = this.popup.visible();
                if (this.filterInput) {
                    isFilterInputActive = this.filterInput[0] === activeElement();
                }
                if (this.wrapper) {
                    isWrapperActive = this.wrapper[0] === activeElement();
                }
                if (isWrapperActive) {
                    if (key === keys.ESC) {
                        this._clearTextAndValue();
                        e.preventDefault();
                        return;
                    }
                    if (this._isMultipleSelection()) {
                        if (key === keys.LEFT) {
                            this._focusPrevTag();
                            e.preventDefault();
                            return;
                        }
                        if (key === keys.RIGHT) {
                            this._focusNextTag();
                            e.preventDefault();
                            return;
                        }
                        if (key === keys.HOME) {
                            this._focusFirstTag();
                            e.preventDefault();
                            return;
                        }
                        if (key === keys.END) {
                            this._focusLastTag();
                            e.preventDefault();
                            return;
                        }
                        if (key === keys.DELETE) {
                            focused = this.tagList.find(DOT + FOCUSED).first();
                            if (focused.length) {
                                tagItem = this._tags[focused.index()];
                                this._removeTag(tagItem);
                            }
                            e.preventDefault();
                            return;
                        }
                        if (key === keys.BACKSPACE) {
                            focused = this.tagList.find(DOT + FOCUSED).first();
                            if (focused.length) {
                                tagItem = this._tags[focused.index()];
                                this._removeTag(tagItem);
                            } else {
                                focused = this._focusLastTag();
                                if (focused.length) {
                                    tagItem = this._tags[focused.index()];
                                    this._removeTag(tagItem);
                                }
                            }
                            e.preventDefault();
                            return;
                        }
                    }
                }
                if (isFilterInputActive) {
                    if (key === keys.ESC) {
                        this._clearFilter();
                    }
                    if (key === keys.UP && !altKey) {
                        this.wrapper.focus();
                        e.preventDefault();
                    }
                    if (browser.msie && browser.version < 10) {
                        if (key === keys.BACKSPACE || key === keys.DELETE) {
                            this._search();
                        }
                    }
                }
                if (altKey && key === keys.UP || key === keys.ESC) {
                    this.wrapper.focus();
                    this.close();
                    e.preventDefault();
                    return;
                }
                if (key === keys.ENTER && this._typingTimeout && this.filterInput && isPopupVisible) {
                    e.preventDefault();
                    return;
                }
                if (key === keys.SPACEBAR && !isFilterInputActive) {
                    this._toggle(!isPopupVisible);
                    e.preventDefault();
                }
                if (altKey && key === keys.DOWN && !isPopupVisible) {
                    this.open();
                    e.preventDefault();
                }
                if (key === keys.DOWN && isPopupVisible) {
                    if (this.filterInput && !isFilterInputActive) {
                        this.filterInput.focus();
                    } else if (this.checkAll && this.checkAll.is(':visible')) {
                        this.checkAll.find('input').focus();
                    } else if (this.tree.is(':visible')) {
                        this.tree.focus();
                    }
                    e.preventDefault();
                }
            },
            _focusPrevTag: function () {
                var focused = this.tagList.find(DOT + FOCUSED);
                if (focused.length) {
                    var activedescendant = this.wrapper.attr('aria-activedescendant');
                    focused.first().removeClass(FOCUSED).removeAttr('id').prev().addClass(FOCUSED).attr('id', activedescendant);
                    this.wrapper.attr('aria-activedescendant', activedescendant);
                } else {
                    this._focusLastTag();
                }
            },
            _focusNextTag: function () {
                var focused = this.tagList.find(DOT + FOCUSED);
                if (focused.length) {
                    var activedescendant = this.wrapper.attr('aria-activedescendant');
                    focused.first().removeClass(FOCUSED).removeAttr('id').next().addClass(FOCUSED).attr('id', activedescendant);
                    this.wrapper.attr('aria-activedescendant', activedescendant);
                } else {
                    this._focusFirstTag();
                }
            },
            _focusFirstTag: function () {
                var activedescendant = this.wrapper.attr('aria-activedescendant');
                this._clearDisabledTag();
                var firstTag = this.tagList.children('li').first().addClass(FOCUSED).attr('id', activedescendant);
                this.wrapper.attr('aria-activedescendant', activedescendant);
                return firstTag;
            },
            _focusLastTag: function () {
                var activedescendant = this.wrapper.attr('aria-activedescendant');
                this._clearDisabledTag();
                var lastTag = this.tagList.children('li').last().addClass(FOCUSED).attr('id', activedescendant);
                this.wrapper.attr('aria-activedescendant', activedescendant);
                return lastTag;
            },
            _clearDisabledTag: function () {
                this.tagList.find(DOT + FOCUSED).removeClass(FOCUSED).removeAttr('id');
            },
            _search: function () {
                var that = this;
                clearTimeout(that._typingTimeout);
                that._typingTimeout = setTimeout(function () {
                    var value = that.filterInput.val();
                    if (that._prev !== value) {
                        that._prev = value;
                        that.search(value);
                    }
                    that._typingTimeout = null;
                }, that.options.delay);
            },
            _clearFilter: function () {
                if (this.filterInput.val().length) {
                    this.filterInput.val('');
                    this._prev = '';
                    this._filtering = true;
                    this.treeview.dataSource.filter({});
                }
            },
            _removeTagClick: function (e) {
                e.stopPropagation();
                var tagItem = this._tags[$(e.currentTarget.parentElement).index()];
                this._removeTag(tagItem);
            },
            _removeTag: function (tagItem) {
                if (!tagItem) {
                    return;
                }
                var uid = tagItem.uid;
                this._uncheckItemByUid(uid);
            }
        });
        function assign(instance, fields, value) {
            var idx = 0, lastIndex = fields.length - 1, field;
            for (; idx < lastIndex; ++idx) {
                field = fields[idx];
                if (!(field in instance)) {
                    instance[field] = {};
                }
                instance = instance[field];
            }
            instance[fields[lastIndex]] = value;
        }
        ui.plugin(DropDownTree);
        var SingleSelection = kendo.Class.extend({
            init: function (view) {
                this._dropdowntree = view;
            },
            _initWrapper: function () {
                this._wrapper();
                this._span();
            },
            _preselect: function (data) {
                var dropdowntree = this._dropdowntree;
                dropdowntree._selectValue(data);
            },
            _wrapper: function () {
                var dropdowntree = this._dropdowntree, element = dropdowntree.element, DOMelement = element[0], wrapper;
                wrapper = element.parent();
                if (!wrapper.is('span.k-widget')) {
                    wrapper = element.wrap('<span />').parent();
                    wrapper[0].style.cssText = DOMelement.style.cssText;
                    wrapper[0].title = DOMelement.title;
                }
                dropdowntree._focused = dropdowntree.wrapper = wrapper.addClass('k-widget k-dropdowntree').addClass(DOMelement.className).css('display', '').attr({
                    accesskey: element.attr('accesskey'),
                    unselectable: 'on',
                    role: 'listbox',
                    'aria-haspopup': true,
                    'aria-expanded': false
                });
                element.hide().removeAttr('accesskey');
            },
            _span: function () {
                var dropdowntree = this._dropdowntree, wrapper = dropdowntree.wrapper, SELECTOR = 'span.k-input', span;
                span = wrapper.find(SELECTOR);
                if (!span[0]) {
                    wrapper.append('<span unselectable="on" class="k-dropdown-wrap k-state-default"><span unselectable="on" class="k-input">&nbsp;</span><span unselectable="on" class="k-select" aria-label="select"><span class="k-icon k-i-arrow-60-down"></span></span></span>').append(dropdowntree.element);
                    span = wrapper.find(SELECTOR);
                }
                dropdowntree.span = span;
                dropdowntree._inputWrapper = $(wrapper[0].firstChild);
                dropdowntree._arrow = wrapper.find('.k-select');
                dropdowntree._arrowIcon = dropdowntree._arrow.find('.k-icon');
            },
            _setValue: function (value) {
                var dropdowntree = this._dropdowntree;
                var currentValue;
                if (value === undefined || value === null) {
                    currentValue = dropdowntree._values.slice()[0];
                    value = typeof currentValue === 'object' ? currentValue : dropdowntree._accessor() || currentValue;
                    return value === undefined || value === null ? '' : value;
                }
                if (value.length === 0) {
                    dropdowntree._clearTextAndValue();
                    return;
                }
                dropdowntree._selectItemByValue(value);
                dropdowntree._toggleCloseVisibility();
            },
            _clearValue: function () {
                var dropdowntree = this._dropdowntree;
                var selectedNode = dropdowntree.treeview.select();
                if (dropdowntree.treeview.dataItem(selectedNode)) {
                    dropdowntree.treeview.dataItem(selectedNode).set('selected', false);
                    dropdowntree.trigger(CHANGE);
                }
            },
            _checkLoadedItem: function (tempItem, value) {
                var dropdowntree = this._dropdowntree;
                if (value && dropdowntree._valueComparer(tempItem, value) || !value && tempItem.selected) {
                    dropdowntree._selectValue(tempItem);
                    dropdowntree.treeview.select(dropdowntree.treeview.findByUid(tempItem.uid));
                }
            }
        });
        var MultipleSelection = kendo.Class.extend({
            init: function (view) {
                this._dropdowntree = view;
            },
            _initWrapper: function () {
                var dropdowntree = this._dropdowntree;
                this._tagTemplate();
                dropdowntree.element.attr('multiple', 'multiple').hide();
                this._wrapper();
                dropdowntree._tags = new ObservableArray([]);
                dropdowntree._multipleTags = new ObservableArray([]);
                this._tagList();
                dropdowntree.span = $('<span unselectable="on" class="k-input">&nbsp;</span>').insertAfter(dropdowntree.tagList);
                dropdowntree._inputWrapper = $(dropdowntree.wrapper[0].firstChild);
            },
            _preselect: function (data, value) {
                var dropdowntree = this._dropdowntree;
                var valueToSelect = value || dropdowntree.options.value;
                if (!$.isArray(data) && !(data instanceof kendo.data.ObservableArray)) {
                    data = [data];
                }
                if ($.isPlainObject(data[0]) || data[0] instanceof kendo.data.ObservableObject || !dropdowntree.options.dataValueField) {
                    dropdowntree.dataSource.data(data);
                    dropdowntree.value(valueToSelect);
                }
            },
            _tagTemplate: function () {
                var dropdowntree = this._dropdowntree;
                var options = dropdowntree.options;
                var tagTemplate = options.valueTemplate;
                var isMultiple = options.tagMode === 'multiple';
                var singleTag = options.messages.singleTag;
                tagTemplate = tagTemplate ? kendo.template(tagTemplate) : dropdowntree.valueTemplate;
                dropdowntree.valueTemplate = function (data) {
                    if (isMultiple) {
                        return '<li class="k-button ' + (data.enabled === false ? 'k-state-disabled' : '') + '" unselectable="on" role="option" ' + (data.enabled === false ? 'aria-disabled="true"' : '') + '>' + '<span unselectable="on">' + tagTemplate(data) + '</span>' + '<span title="' + dropdowntree.options.messages.deleteTag + '" aria-label="' + dropdowntree.options.messages.deleteTag + '" class="k-select">' + '<span class="k-icon k-i-close"></span>' + '</span>' + '</li>';
                    }
                    return '<li class="k-button" unselectable="on" role="option">' + '<span unselectable="on" data-bind="text: tags.length"></span>' + '<span unselectable="on">&nbsp;' + singleTag + '</span>' + '</li>';
                };
            },
            _wrapper: function () {
                var dropdowntree = this._dropdowntree, element = dropdowntree.element, wrapper = element.parent('span.k-dropdowntree');
                if (!wrapper[0]) {
                    wrapper = element.wrap('<div class="k-widget k-dropdowntree" unselectable="on" />').parent();
                    wrapper[0].style.cssText = element[0].style.cssText;
                    wrapper[0].title = element[0].title;
                    $('<div class="k-multiselect-wrap k-floatwrap" unselectable="on" />').insertBefore(element);
                }
                dropdowntree.wrapper = wrapper.addClass(element[0].className).css('display', '').attr({
                    role: 'listbox',
                    'aria-activedescendant': kendo.guid(),
                    'aria-haspopup': true,
                    'aria-expanded': false
                });
                dropdowntree._innerWrapper = $(wrapper[0].firstChild);
            },
            _tagList: function () {
                var dropdowntree = this._dropdowntree, tagList = dropdowntree._innerWrapper.children('ul');
                if (!tagList[0]) {
                    var isMultiple = dropdowntree.options.tagMode === 'multiple';
                    var tagCollection = isMultiple ? 'tags' : 'multipleTag';
                    tagList = $('<ul role="listbox" unselectable="on" data-template="tagTemplate" data-bind="source: ' + tagCollection + '" class="k-reset"/>').appendTo(dropdowntree._innerWrapper);
                }
                dropdowntree.tagList = tagList;
                dropdowntree.tagList.attr('id', kendo.guid() + '_tagList');
                dropdowntree.wrapper.attr('aria-owns', dropdowntree.tagList.attr('id'));
                var viewModel = kendo.observable({
                    multipleTag: dropdowntree._multipleTags,
                    tags: dropdowntree._tags,
                    tagTemplate: dropdowntree.valueTemplate
                });
                kendo.bind(dropdowntree.tagList, viewModel);
            },
            _setValue: function (value) {
                var dropdowntree = this._dropdowntree;
                var oldValues = dropdowntree._values;
                if (value === undefined || value === null) {
                    return dropdowntree._values.slice();
                }
                dropdowntree.setValue(value);
                if (value.length) {
                    this._removeValues(oldValues, value);
                    dropdowntree._checkItemByValue(value);
                } else {
                    dropdowntree._clearTextAndValue();
                }
                dropdowntree._toggleCloseVisibility();
            },
            _removeValues: function (oldValues, value) {
                var dropdowntree = this._dropdowntree;
                var removedValues = this._getNewValues(oldValues, value);
                for (var idx = 0; idx < removedValues.length; idx++) {
                    for (var j = 0; j < dropdowntree._tags.length; j++) {
                        if (dropdowntree._valueComparer(dropdowntree._tags[j], removedValues[idx])) {
                            dropdowntree._uncheckItemByUid(dropdowntree._tags[j].uid);
                        }
                    }
                }
            },
            _getNewValues: function (oldValues, value) {
                var removedValues = [];
                for (var idx = 0; idx < oldValues.length; idx++) {
                    if (value.indexOf(oldValues[idx]) === -1) {
                        removedValues.push(oldValues[idx]);
                    }
                }
                return removedValues;
            },
            _clearValue: function () {
                var dropdowntree = this._dropdowntree;
                var tagsArray = dropdowntree._tags.slice();
                for (var idx = 0; idx < tagsArray.length; idx++) {
                    var uid = tagsArray[idx].uid;
                    dropdowntree._preventChangeTrigger = true;
                    dropdowntree._uncheckItemByUid(uid);
                }
                if (tagsArray.length) {
                    dropdowntree._preventChangeTrigger = false;
                    dropdowntree.trigger(CHANGE);
                }
            },
            _checkLoadedItem: function (tempItem, value, length) {
                var dropdowntree = this._dropdowntree;
                if (!length && tempItem.checked) {
                    dropdowntree._checkValue(tempItem);
                }
                if (length && (value.indexOf(dropdowntree._currentValue(tempItem)) !== -1 || value.indexOf(tempItem)) !== -1 && !this._findTag(dropdowntree._currentValue(tempItem))) {
                    if (tempItem.checked) {
                        dropdowntree._checkValue(tempItem);
                    } else {
                        tempItem.set('checked', true);
                    }
                }
            },
            _findTag: function (tempItemValue) {
                var dropdowntree = this._dropdowntree;
                return dropdowntree._tags.find(function (item) {
                    return dropdowntree._valueComparer(item, tempItemValue);
                });
            }
        });
        kendo.ui.DropDownTree.SingleSelection = SingleSelection;
        kendo.ui.DropDownTree.MultipleSelection = MultipleSelection;
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));