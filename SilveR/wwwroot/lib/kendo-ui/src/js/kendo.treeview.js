/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('kendo.treeview', [
        'kendo.data',
        'kendo.treeview.draganddrop'
    ], f);
}(function () {
    var __meta__ = {
        id: 'treeview',
        name: 'TreeView',
        category: 'web',
        description: 'The TreeView widget displays hierarchical data in a traditional tree structure,with support for interactive drag-and-drop operations.',
        depends: ['data'],
        features: [{
                id: 'treeview-dragging',
                name: 'Drag & Drop',
                description: 'Support for drag & drop',
                depends: ['treeview.draganddrop']
            }]
    };
    (function ($, undefined) {
        var kendo = window.kendo, ui = kendo.ui, data = kendo.data, extend = $.extend, template = kendo.template, isArray = $.isArray, Widget = ui.Widget, HierarchicalDataSource = data.HierarchicalDataSource, proxy = $.proxy, keys = kendo.keys, NS = '.kendoTreeView', TEMP_NS = '.kendoTreeViewTemp', SELECT = 'select', CHECK = 'check', NAVIGATE = 'navigate', EXPAND = 'expand', CHANGE = 'change', ERROR = 'error', CHECKED = 'checked', INDETERMINATE = 'indeterminate', COLLAPSE = 'collapse', DRAGSTART = 'dragstart', DRAG = 'drag', DROP = 'drop', DRAGEND = 'dragend', DATABOUND = 'dataBound', CLICK = 'click', UNDEFINED = 'undefined', KSTATEHOVER = 'k-state-hover', KTREEVIEW = 'k-treeview', VISIBLE = ':visible', NODE = '.k-item', STRING = 'string', ARIALABEL = 'aria-label', ARIASELECTED = 'aria-selected', ARIADISABLED = 'aria-disabled', DISABLED = 'k-state-disabled', TreeView, subGroup, nodeContents, nodeIcon, spriteRe, bindings = {
                text: 'dataTextField',
                url: 'dataUrlField',
                spriteCssClass: 'dataSpriteCssClassField',
                imageUrl: 'dataImageUrlField'
            }, isJQueryInstance = function (obj) {
                return obj instanceof kendo.jQuery || obj instanceof window.jQuery;
            }, isDomElement = function (o) {
                return typeof HTMLElement === 'object' ? o instanceof HTMLElement : o && typeof o === 'object' && o.nodeType === 1 && typeof o.nodeName === STRING;
            };
        function contentChild(filter) {
            return function (node) {
                var result = node.children('.k-animation-container');
                if (!result.length) {
                    result = node;
                }
                return result.children(filter);
            };
        }
        function templateNoWith(code) {
            return kendo.template(code, { useWithBlock: false });
        }
        subGroup = contentChild('.k-group');
        nodeContents = contentChild('.k-group,.k-content');
        nodeIcon = function (node) {
            return node.children('div').children('.k-icon');
        };
        function checkboxes(node) {
            return node.find('> div .k-checkbox-wrapper [type=checkbox]');
        }
        function insertAction(indexOffset) {
            return function (nodeData, referenceNode) {
                referenceNode = referenceNode.closest(NODE);
                var group = referenceNode.parent(), parentNode;
                if (group.parent().is('li')) {
                    parentNode = group.parent();
                }
                return this._dataSourceMove(nodeData, group, parentNode, function (dataSource, model) {
                    var referenceItem = this.dataItem(referenceNode);
                    var referenceNodeIndex = referenceItem ? referenceItem.parent().indexOf(referenceItem) : referenceNode.index();
                    return this._insert(dataSource.data(), model, referenceNodeIndex + indexOffset);
                });
            };
        }
        spriteRe = /k-sprite/;
        function moveContents(node, container) {
            var tmp;
            while (node && node.nodeName.toLowerCase() != 'ul') {
                tmp = node;
                node = node.nextSibling;
                if (tmp.nodeType == 3) {
                    tmp.nodeValue = $.trim(tmp.nodeValue);
                }
                if (spriteRe.test(tmp.className)) {
                    container.insertBefore(tmp, container.firstChild);
                } else {
                    container.appendChild(tmp);
                }
            }
        }
        function updateNodeHtml(node) {
            var wrapper = node.children('div'), group = node.children('ul'), toggleButton = wrapper.children('.k-icon'), checkbox = node.children(':checkbox'), innerWrapper = wrapper.children('.k-in');
            if (node.hasClass('k-treeview')) {
                return;
            }
            if (!wrapper.length) {
                wrapper = $('<div />').prependTo(node);
            }
            if (!toggleButton.length && group.length) {
                toggleButton = $('<span class=\'k-icon\' />').prependTo(wrapper);
            } else if (!group.length || !group.children().length) {
                toggleButton.remove();
                group.remove();
            }
            if (checkbox.length) {
                $('<span class=\'k-checkbox-wrapper\' />').appendTo(wrapper).append(checkbox);
            }
            if (!innerWrapper.length) {
                innerWrapper = node.children('a').eq(0).addClass('k-in k-link');
                if (!innerWrapper.length) {
                    innerWrapper = $('<span class=\'k-in\' />');
                }
                innerWrapper.appendTo(wrapper);
                if (wrapper.length) {
                    moveContents(wrapper[0].nextSibling, innerWrapper[0]);
                }
            }
        }
        TreeView = kendo.ui.DataBoundWidget.extend({
            init: function (element, options) {
                var that = this, inferred = false, hasDataSource = options && !!options.dataSource, list;
                if (isArray(options)) {
                    options = { dataSource: options };
                }
                if (options && typeof options.loadOnDemand == UNDEFINED && isArray(options.dataSource)) {
                    options.loadOnDemand = false;
                }
                Widget.prototype.init.call(that, element, options);
                element = that.element;
                options = that.options;
                list = element.is('ul') && element || element.hasClass(KTREEVIEW) && element.children('ul');
                inferred = !hasDataSource && list.length;
                if (inferred) {
                    options.dataSource.list = list;
                }
                that._animation();
                that._accessors();
                that._templates();
                if (!element.hasClass(KTREEVIEW)) {
                    that._wrapper();
                    if (list) {
                        that.root = element;
                        that._group(that.wrapper);
                    }
                } else {
                    that.wrapper = element;
                    that.root = element.children('ul').eq(0);
                }
                that._tabindex();
                that.root.attr('role', 'tree');
                that._dataSource(inferred);
                that._attachEvents();
                that._dragging();
                if (!inferred) {
                    if (options.autoBind) {
                        that._progress(true);
                        that.dataSource.fetch();
                    }
                } else {
                    that._syncHtmlAndDataSource();
                }
                if (options.checkboxes && options.checkboxes.checkChildren) {
                    that.updateIndeterminate();
                }
                if (that.element[0].id) {
                    that._ariaId = kendo.format('{0}_tv_active', that.element[0].id);
                }
                kendo.notify(that);
            },
            _attachEvents: function () {
                var that = this, clickableItems = '.k-in:not(.k-state-selected,.k-state-disabled)', MOUSEENTER = 'mouseenter';
                that.wrapper.on(MOUSEENTER + NS, '.k-in.k-state-selected', function (e) {
                    e.preventDefault();
                }).on(MOUSEENTER + NS, clickableItems, function () {
                    $(this).addClass(KSTATEHOVER);
                }).on('mouseleave' + NS, clickableItems, function () {
                    $(this).removeClass(KSTATEHOVER);
                }).on(CLICK + NS, clickableItems, proxy(that._click, that)).on('dblclick' + NS, '.k-in:not(.k-state-disabled)', proxy(that._toggleButtonClick, that)).on(CLICK + NS, '.k-i-expand,.k-i-collapse', proxy(that._toggleButtonClick, that)).on('keydown' + NS, proxy(that._keydown, that)).on('keypress' + NS, proxy(that._keypress, that)).on('focus' + NS, proxy(that._focus, that)).on('blur' + NS, proxy(that._blur, that)).on('mousedown' + NS, '.k-in,.k-checkbox-wrapper :checkbox,.k-i-expand,.k-i-collapse', proxy(that._mousedown, that)).on('change' + NS, '.k-checkbox-wrapper :checkbox', proxy(that._checkboxChange, that)).on('click' + NS, '.checkbox-span', proxy(that._checkboxLabelClick, that)).on('click' + NS, '.k-request-retry', proxy(that._retryRequest, that)).on('click' + NS, '.k-link.k-state-disabled', function (e) {
                    e.preventDefault();
                }).on('click' + NS, function (e) {
                    if (!$(e.target).is(':kendoFocusable')) {
                        that.focus();
                    }
                });
            },
            _checkboxLabelClick: function (e) {
                var checkbox = $(e.target.previousSibling);
                if (checkbox.is('[disabled]')) {
                    return;
                }
                checkbox.prop('checked', !checkbox.prop('checked'));
                checkbox.trigger('change');
            },
            _syncHtmlAndDataSource: function (root, dataSource) {
                root = root || this.root;
                dataSource = dataSource || this.dataSource;
                var data = dataSource.view(), uidAttr = kendo.attr('uid'), expandedAttr = kendo.attr('expanded'), checkboxesEnabled = this.options.checkboxes, items = root.children('li'), i, item, dataItem, uid, itemCheckbox;
                for (i = 0; i < items.length; i++) {
                    dataItem = data[i];
                    uid = dataItem.uid;
                    item = items.eq(i);
                    item.attr('role', 'treeitem').attr(uidAttr, uid).attr(ARIASELECTED, item.hasClass('k-state-selected'));
                    dataItem.expanded = item.attr(expandedAttr) === 'true';
                    if (checkboxesEnabled) {
                        itemCheckbox = checkboxes(item);
                        dataItem.checked = itemCheckbox.prop(CHECKED);
                        itemCheckbox.attr('id', '_' + uid);
                        itemCheckbox.next('.k-checkbox-label').attr('for', '_' + uid);
                    }
                    this._syncHtmlAndDataSource(item.children('ul'), dataItem.children);
                }
            },
            _animation: function () {
                var options = this.options, animationOptions = options.animation, hasCollapseAnimation = animationOptions.collapse && 'effects' in animationOptions.collapse, collapse = extend({}, animationOptions.expand, animationOptions.collapse);
                if (!hasCollapseAnimation) {
                    collapse = extend(collapse, { reverse: true });
                }
                if (animationOptions === false) {
                    animationOptions = {
                        expand: { effects: {} },
                        collapse: {
                            hide: true,
                            effects: {}
                        }
                    };
                }
                animationOptions.collapse = extend(collapse, { hide: true });
                options.animation = animationOptions;
            },
            _dragging: function () {
                var enabled = this.options.dragAndDrop;
                var dragging = this.dragging;
                if (enabled && !dragging) {
                    var widget = this;
                    this.dragging = new ui.HierarchicalDragAndDrop(this.element, {
                        reorderable: true,
                        $angular: this.options.$angular,
                        autoScroll: this.options.autoScroll,
                        filter: 'div:not(.k-state-disabled) .k-in',
                        allowedContainers: '.k-treeview',
                        itemSelector: '.k-treeview .k-item',
                        hintText: proxy(this._hintText, this),
                        contains: function (source, destination) {
                            return $.contains(source, destination);
                        },
                        dropHintContainer: function (item) {
                            return item;
                        },
                        itemFromTarget: function (target) {
                            var item = target.closest('.k-top,.k-mid,.k-bot');
                            return {
                                item: item,
                                content: target.closest('.k-in'),
                                first: item.hasClass('k-top'),
                                last: item.hasClass('k-bot')
                            };
                        },
                        dropPositionFrom: function (dropHint) {
                            return dropHint.prevAll('.k-in').length > 0 ? 'after' : 'before';
                        },
                        dragstart: function (source) {
                            return widget.trigger(DRAGSTART, { sourceNode: source[0] });
                        },
                        drag: function (options) {
                            widget.trigger(DRAG, {
                                originalEvent: options.originalEvent,
                                sourceNode: options.source[0],
                                dropTarget: options.target[0],
                                pageY: options.pageY,
                                pageX: options.pageX,
                                statusClass: options.status,
                                setStatusClass: options.setStatus
                            });
                        },
                        drop: function (options) {
                            var dropTarget = $(options.dropTarget);
                            var navigationTarget = dropTarget.closest('a');
                            if (navigationTarget && navigationTarget.attr('href')) {
                                widget._tempPreventNavigation(navigationTarget);
                            }
                            return widget.trigger(DROP, {
                                originalEvent: options.originalEvent,
                                sourceNode: options.source,
                                destinationNode: options.destination,
                                valid: options.valid,
                                setValid: function (state) {
                                    this.valid = state;
                                    options.setValid(state);
                                },
                                dropTarget: options.dropTarget,
                                dropPosition: options.position
                            });
                        },
                        dragend: function (options) {
                            var source = options.source;
                            var destination = options.destination;
                            var position = options.position;
                            function triggerDragEnd(source) {
                                if (widget.options.checkboxes && widget.options.checkboxes.checkChildren) {
                                    widget.updateIndeterminate();
                                }
                                widget.trigger(DRAGEND, {
                                    originalEvent: options.originalEvent,
                                    sourceNode: source && source[0],
                                    destinationNode: destination[0],
                                    dropPosition: position
                                });
                            }
                            if (position == 'over') {
                                widget.append(source, destination, triggerDragEnd);
                            } else {
                                if (position == 'before') {
                                    source = widget.insertBefore(source, destination);
                                } else if (position == 'after') {
                                    source = widget.insertAfter(source, destination);
                                }
                                triggerDragEnd(source);
                            }
                        }
                    });
                } else if (!enabled && dragging) {
                    dragging.destroy();
                    this.dragging = null;
                }
            },
            _tempPreventNavigation: function (node) {
                node.on(CLICK + NS + TEMP_NS, function (ev) {
                    ev.preventDefault();
                    node.off(CLICK + NS + TEMP_NS);
                });
            },
            _hintText: function (node) {
                return this.templates.dragClue({
                    item: this.dataItem(node),
                    treeview: this.options
                });
            },
            _templates: function () {
                var that = this, options = that.options, fieldAccessor = proxy(that._fieldAccessor, that);
                if (options.template && typeof options.template == STRING) {
                    options.template = template(options.template);
                } else if (!options.template) {
                    options.template = templateNoWith('# var text = ' + fieldAccessor('text') + '(data.item); #' + '# if (typeof data.item.encoded != \'undefined\' && data.item.encoded === false) {#' + '#= text #' + '# } else { #' + '#: text #' + '# } #');
                }
                that._checkboxes();
                that.templates = {
                    wrapperCssClass: function (group, item) {
                        var result = 'k-item', index = item.index;
                        if (group.firstLevel && index === 0) {
                            result += ' k-first';
                        }
                        if (index == group.length - 1) {
                            result += ' k-last';
                        }
                        return result;
                    },
                    cssClass: function (group, item) {
                        var result = '', index = item.index, groupLength = group.length - 1;
                        if (group.firstLevel && index === 0) {
                            result += 'k-top ';
                        }
                        if (index === 0 && index != groupLength) {
                            result += 'k-top';
                        } else if (index == groupLength) {
                            result += 'k-bot';
                        } else {
                            result += 'k-mid';
                        }
                        return result;
                    },
                    textClass: function (item, isLink) {
                        var result = 'k-in';
                        if (isLink) {
                            result += ' k-link';
                        }
                        if (item.enabled === false) {
                            result += ' k-state-disabled';
                        }
                        if (item.selected === true) {
                            result += ' k-state-selected';
                        }
                        return result;
                    },
                    toggleButtonClass: function (item) {
                        var result = 'k-icon';
                        if (item.expanded !== true) {
                            result += ' k-i-expand';
                        } else {
                            result += ' k-i-collapse';
                        }
                        return result;
                    },
                    groupAttributes: function (group) {
                        var attributes = '';
                        if (!group.firstLevel) {
                            attributes = 'role=\'group\'';
                        }
                        return attributes + (group.expanded !== true ? ' style=\'display:none\'' : '');
                    },
                    groupCssClass: function (group) {
                        var cssClass = 'k-group';
                        if (group.firstLevel) {
                            cssClass += ' k-treeview-lines';
                        }
                        return cssClass;
                    },
                    dragClue: templateNoWith('#= data.treeview.template(data) #'),
                    group: templateNoWith('<ul class=\'#= data.r.groupCssClass(data.group) #\'#= data.r.groupAttributes(data.group) #>' + '#= data.renderItems(data) #' + '</ul>'),
                    itemContent: templateNoWith('# var imageUrl = ' + fieldAccessor('imageUrl') + '(data.item); #' + '# var spriteCssClass = ' + fieldAccessor('spriteCssClass') + '(data.item); #' + '# if (imageUrl) { #' + '<img class=\'k-image\' alt=\'\' src=\'#= imageUrl #\'>' + '# } #' + '# if (spriteCssClass) { #' + '<span class=\'k-sprite #= spriteCssClass #\' />' + '# } #' + '#= data.treeview.template(data) #'),
                    itemElement: templateNoWith('# var item = data.item, r = data.r; #' + '# var url = ' + fieldAccessor('url') + '(item); #' + '<div class=\'#= r.cssClass(data.group, item) #\'>' + '# if (item.hasChildren) { #' + '<span class=\'#= r.toggleButtonClass(item) #\'/>' + '# } #' + '# if (data.treeview.checkboxes) { #' + '<span class=\'k-checkbox-wrapper\' role=\'presentation\'>' + '#= data.treeview.checkboxes.template(data) #' + '</span>' + '# } #' + '# var tag = url ? \'a\' : \'span\'; #' + '# var textAttr = url ? \' href=\\\'\' + url + \'\\\'\' : \'\'; #' + '<#=tag# class=\'#= r.textClass(item, !!url) #\'#= textAttr #>' + '#= r.itemContent(data) #' + '</#=tag#>' + '</div>'),
                    item: templateNoWith('# var item = data.item, r = data.r; #' + '<li role=\'treeitem\' class=\'#= r.wrapperCssClass(data.group, item) #\' ' + kendo.attr('uid') + '=\'#= item.uid #\' ' + 'aria-selected=\'#= item.selected ? "true" : "false" #\' ' + '#=item.enabled === false ? "aria-disabled=\'true\'" : \'\'#' + '# if (item.expanded) { #' + 'data-expanded=\'true\' aria-expanded=\'true\'' + '# } #' + '>' + '#= r.itemElement(data) #' + '</li>'),
                    loading: templateNoWith('<div class=\'k-icon k-i-loading\' /> #: data.messages.loading #'),
                    retry: templateNoWith('#: data.messages.requestFailed # ' + '<button class=\'k-button k-request-retry\'>#: data.messages.retry #</button>')
                };
            },
            items: function () {
                return this.element.find('.k-item > div:first-child');
            },
            setDataSource: function (dataSource) {
                var options = this.options;
                options.dataSource = dataSource;
                this._dataSource();
                if (options.checkboxes && options.checkboxes.checkChildren) {
                    this.dataSource.one('change', $.proxy(this.updateIndeterminate, this, null));
                }
                if (this.options.autoBind) {
                    this.dataSource.fetch();
                }
            },
            _bindDataSource: function () {
                this._refreshHandler = proxy(this.refresh, this);
                this._errorHandler = proxy(this._error, this);
                this.dataSource.bind(CHANGE, this._refreshHandler);
                this.dataSource.bind(ERROR, this._errorHandler);
            },
            _unbindDataSource: function () {
                var dataSource = this.dataSource;
                if (dataSource) {
                    dataSource.unbind(CHANGE, this._refreshHandler);
                    dataSource.unbind(ERROR, this._errorHandler);
                }
            },
            _dataSource: function (silentRead) {
                var that = this, options = that.options, dataSource = options.dataSource;
                function recursiveRead(data) {
                    for (var i = 0; i < data.length; i++) {
                        data[i]._initChildren();
                        data[i].children.fetch();
                        recursiveRead(data[i].children.view());
                    }
                }
                dataSource = isArray(dataSource) ? { data: dataSource } : dataSource;
                that._unbindDataSource();
                if (!dataSource.fields) {
                    dataSource.fields = [
                        { field: 'text' },
                        { field: 'url' },
                        { field: 'spriteCssClass' },
                        { field: 'imageUrl' }
                    ];
                }
                that.dataSource = dataSource = HierarchicalDataSource.create(dataSource);
                if (silentRead) {
                    dataSource.fetch();
                    recursiveRead(dataSource.view());
                }
                that._bindDataSource();
            },
            events: [
                DRAGSTART,
                DRAG,
                DROP,
                DRAGEND,
                DATABOUND,
                EXPAND,
                COLLAPSE,
                SELECT,
                CHANGE,
                NAVIGATE,
                CHECK
            ],
            options: {
                name: 'TreeView',
                dataSource: {},
                animation: {
                    expand: {
                        effects: 'expand:vertical',
                        duration: 200
                    },
                    collapse: { duration: 100 }
                },
                messages: {
                    loading: 'Loading...',
                    requestFailed: 'Request failed.',
                    retry: 'Retry'
                },
                dragAndDrop: false,
                checkboxes: false,
                autoBind: true,
                autoScroll: false,
                loadOnDemand: true,
                template: '',
                dataTextField: null
            },
            _accessors: function () {
                var that = this, options = that.options, i, field, textField, element = that.element;
                for (i in bindings) {
                    field = options[bindings[i]];
                    textField = element.attr(kendo.attr(i + '-field'));
                    if (!field && textField) {
                        field = textField;
                    }
                    if (!field) {
                        field = i;
                    }
                    if (!isArray(field)) {
                        field = [field];
                    }
                    options[bindings[i]] = field;
                }
            },
            _fieldAccessor: function (fieldName) {
                var fieldBindings = this.options[bindings[fieldName]], count = fieldBindings.length, result = '(function(item) {';
                if (count === 0) {
                    result += 'return item[\'' + fieldName + '\'];';
                } else {
                    result += 'var levels = [' + $.map(fieldBindings, function (x) {
                        return 'function(d){ return ' + kendo.expr(x) + '}';
                    }).join(',') + '];';
                    result += 'return levels[Math.min(item.level(), ' + count + '-1)](item)';
                }
                result += '})';
                return result;
            },
            setOptions: function (options) {
                Widget.fn.setOptions.call(this, options);
                this._animation();
                this._dragging();
                this._templates();
            },
            _trigger: function (eventName, node) {
                return this.trigger(eventName, { node: node.closest(NODE)[0] });
            },
            _setChecked: function (datasource, value) {
                if (!datasource || !$.isFunction(datasource.view)) {
                    return;
                }
                for (var i = 0, nodes = datasource.view(); i < nodes.length; i++) {
                    if (nodes[i].enabled !== false) {
                        nodes[i].set(CHECKED, value);
                    }
                    if (nodes[i].children) {
                        this._setChecked(nodes[i].children, value);
                    }
                }
            },
            _setIndeterminate: function (node) {
                var group = subGroup(node), siblings, length, all = true, i;
                if (!group.length) {
                    return;
                }
                siblings = checkboxes(group.children());
                length = siblings.length;
                if (!length) {
                    return;
                } else if (length > 1) {
                    for (i = 1; i < length; i++) {
                        if (siblings[i].checked != siblings[i - 1].checked || siblings[i].indeterminate || siblings[i - 1].indeterminate) {
                            all = false;
                            break;
                        }
                    }
                } else {
                    all = !siblings[0].indeterminate;
                }
                return checkboxes(node).data(INDETERMINATE, !all).prop(INDETERMINATE, !all).prop(CHECKED, all && siblings[0].checked);
            },
            updateIndeterminate: function (node) {
                node = node || this.wrapper;
                var subnodes = subGroup(node).children();
                var i;
                var checkbox;
                var dataItem;
                if (subnodes.length) {
                    for (i = 0; i < subnodes.length; i++) {
                        this.updateIndeterminate(subnodes.eq(i));
                    }
                    checkbox = this._setIndeterminate(node);
                    dataItem = this.dataItem(node);
                    if (checkbox && checkbox.prop(CHECKED)) {
                        dataItem.checked = true;
                    } else {
                        if (dataItem) {
                            delete dataItem.checked;
                        }
                    }
                }
            },
            _bubbleIndeterminate: function (node, skipDownward) {
                if (!node.length) {
                    return;
                }
                if (!skipDownward) {
                    this.updateIndeterminate(node);
                }
                var parentNode = this.parent(node), checkbox;
                if (parentNode.length) {
                    this._setIndeterminate(parentNode);
                    checkbox = parentNode.children('div').find('.k-checkbox-wrapper :checkbox');
                    this._skip = true;
                    if (checkbox.prop(INDETERMINATE) === false) {
                        this.dataItem(parentNode).set(CHECKED, checkbox.prop(CHECKED));
                    } else {
                        this.dataItem(parentNode).set(CHECKED, false);
                    }
                    this._skip = false;
                    this._bubbleIndeterminate(parentNode, true);
                }
            },
            _checkboxChange: function (e) {
                var checkbox = $(e.target);
                var isChecked = checkbox.prop(CHECKED);
                var node = checkbox.closest(NODE);
                var dataItem = this.dataItem(node);
                if (dataItem.checked != isChecked) {
                    dataItem.set(CHECKED, isChecked);
                    this._trigger(CHECK, node);
                }
            },
            _toggleButtonClick: function (e) {
                var node = $(e.currentTarget).closest(NODE);
                if (node.is('[aria-disabled=\'true\']')) {
                    return;
                }
                this.toggle(node);
            },
            _mousedown: function (e) {
                var node = $(e.currentTarget).closest(NODE);
                if (node.is('[aria-disabled=\'true\']')) {
                    return;
                }
                this._clickTarget = node;
                this.current(node);
            },
            _focusable: function (node) {
                return node && node.length && node.is(':visible') && !node.find('.k-in:first').hasClass(DISABLED);
            },
            _focus: function () {
                var current = this.select(), clickTarget = this._clickTarget;
                if (kendo.support.touch) {
                    return;
                }
                if (clickTarget && clickTarget.length) {
                    current = clickTarget;
                }
                if (!this._focusable(current)) {
                    current = this.current();
                }
                if (!this._focusable(current)) {
                    current = this._nextVisible($());
                }
                this.current(current);
            },
            focus: function () {
                var wrapper = this.wrapper, scrollContainer = wrapper[0], containers = [], offsets = [], documentElement = document.documentElement, i;
                do {
                    scrollContainer = scrollContainer.parentNode;
                    if (scrollContainer.scrollHeight > scrollContainer.clientHeight) {
                        containers.push(scrollContainer);
                        offsets.push(scrollContainer.scrollTop);
                    }
                } while (scrollContainer != documentElement);
                wrapper.focus();
                for (i = 0; i < containers.length; i++) {
                    containers[i].scrollTop = offsets[i];
                }
            },
            _blur: function () {
                this.current().find('.k-in:first').removeClass('k-state-focused');
            },
            _enabled: function (node) {
                return !node.children('div').children('.k-in').hasClass(DISABLED);
            },
            parent: function (node) {
                var wrapperRe = /\bk-treeview\b/, itemRe = /\bk-item\b/, result, skipSelf;
                if (typeof node == STRING) {
                    node = this.element.find(node);
                }
                if (!isDomElement(node)) {
                    node = node[0];
                }
                skipSelf = itemRe.test(node.className);
                do {
                    node = node.parentNode;
                    if (itemRe.test(node.className)) {
                        if (skipSelf) {
                            result = node;
                        } else {
                            skipSelf = true;
                        }
                    }
                } while (!wrapperRe.test(node.className) && !result);
                return $(result);
            },
            _nextVisible: function (node) {
                var that = this, expanded = that._expanded(node), result;
                function nextParent(node) {
                    while (node.length && !node.next().length) {
                        node = that.parent(node);
                    }
                    if (node.next().length) {
                        return node.next();
                    } else {
                        return node;
                    }
                }
                if (!node.length || !node.is(':visible')) {
                    result = that.root.children().eq(0);
                } else if (expanded) {
                    result = subGroup(node).children().first();
                    if (!result.length) {
                        result = nextParent(node);
                    }
                } else {
                    result = nextParent(node);
                }
                return result;
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
                } else if (key == keys.UP) {
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
                } else if (key == keys.SPACEBAR && checkbox.length) {
                    if (!focused.find('.k-in:first').hasClass(DISABLED)) {
                        checkbox.prop(CHECKED, !checkbox.prop(CHECKED)).data(INDETERMINATE, false).prop(INDETERMINATE, false);
                        that._checkboxChange({ target: checkbox });
                    }
                    target = focused;
                }
                if (target) {
                    e.preventDefault();
                    if (focused[0] != target[0]) {
                        that._trigger(NAVIGATE, target);
                        that.current(target);
                    }
                }
            },
            _keypress: function (e) {
                var that = this;
                var delay = 300;
                var selectedNode = that._getSelectedNode();
                var matchToSelect;
                if (e.keyCode === keys.ENTER || e.keyCode === keys.SPACEBAR) {
                    return;
                }
                if (!that._match) {
                    that._match = '';
                }
                that._match += String.fromCharCode(e.keyCode);
                clearTimeout(that._matchTimer);
                that._matchTimer = setTimeout(function () {
                    that._match = '';
                }, delay);
                matchToSelect = selectedNode && that._matchNextByText(Array.prototype.indexOf.call(this.element.find('.k-item'), selectedNode[0]), that._match);
                if (!matchToSelect) {
                    matchToSelect = that._matchNextByText(-1, that._match);
                }
                that.select(matchToSelect);
            },
            _matchNextByText: function (startIndex, text) {
                return $(this.element).find('.k-in').filter(function (i, element) {
                    return i > startIndex && $(element).is(':visible') && !$(element).hasClass(DISABLED) && $(element).text().toLowerCase().indexOf(text) === 0;
                }).closest(NODE)[0];
            },
            _click: function (e) {
                var that = this, node = $(e.currentTarget), contents = nodeContents(node.closest(NODE)), href = node.attr('href'), shouldNavigate;
                if (href) {
                    shouldNavigate = href == '#' || href.indexOf('#' + this.element.id + '-') >= 0;
                } else {
                    shouldNavigate = contents.length && !contents.children().length;
                }
                if (shouldNavigate) {
                    e.preventDefault();
                }
                if (!node.hasClass('.k-state-selected') && !that._trigger(SELECT, node)) {
                    that.select(node);
                }
            },
            _wrapper: function () {
                var that = this, element = that.element, wrapper, root, wrapperClasses = 'k-widget k-treeview';
                if (element.is('ul')) {
                    wrapper = element.wrap('<div />').parent();
                    root = element;
                } else {
                    wrapper = element;
                    root = wrapper.children('ul').eq(0);
                }
                that.wrapper = wrapper.addClass(wrapperClasses);
                that.root = root;
            },
            _getSelectedNode: function () {
                return this.element.find('.k-state-selected').closest(NODE);
            },
            _group: function (item) {
                var that = this, firstLevel = item.hasClass(KTREEVIEW), group = {
                        firstLevel: firstLevel,
                        expanded: firstLevel || that._expanded(item)
                    }, groupElement = item.children('ul');
                groupElement.addClass(that.templates.groupCssClass(group)).css('display', group.expanded ? '' : 'none');
                that._nodes(groupElement, group);
            },
            _nodes: function (groupElement, groupData) {
                var that = this, nodes = groupElement.children('li'), nodeData;
                groupData = extend({ length: nodes.length }, groupData);
                nodes.each(function (i, node) {
                    node = $(node);
                    nodeData = {
                        index: i,
                        expanded: that._expanded(node)
                    };
                    updateNodeHtml(node);
                    that._updateNodeClasses(node, groupData, nodeData);
                    that._group(node);
                });
            },
            _checkboxes: function () {
                var options = this.options;
                var checkboxes = options.checkboxes;
                var defaultTemplate;
                if (checkboxes) {
                    defaultTemplate = '<input type=\'checkbox\' tabindex=\'-1\' #= (item.enabled === false) ? \'disabled\' : \'\' # #= item.checked ? \'checked\' : \'\' #';
                    if (checkboxes.name) {
                        defaultTemplate += ' name=\'' + checkboxes.name + '\'';
                    }
                    defaultTemplate += ' id=\'_#= item.uid #\' class=\'k-checkbox\' /><span class=\'k-checkbox-label checkbox-span\'></span>';
                    checkboxes = extend({ template: defaultTemplate }, options.checkboxes);
                    if (typeof checkboxes.template == STRING) {
                        checkboxes.template = template(checkboxes.template);
                    }
                    options.checkboxes = checkboxes;
                }
            },
            _updateNodeClasses: function (node, groupData, nodeData) {
                var wrapper = node.children('div'), group = node.children('ul'), templates = this.templates;
                if (node.hasClass('k-treeview')) {
                    return;
                }
                nodeData = nodeData || {};
                nodeData.expanded = typeof nodeData.expanded != UNDEFINED ? nodeData.expanded : this._expanded(node);
                nodeData.index = typeof nodeData.index != UNDEFINED ? nodeData.index : node.index();
                nodeData.enabled = typeof nodeData.enabled != UNDEFINED ? nodeData.enabled : !wrapper.children('.k-in').hasClass('k-state-disabled');
                groupData = groupData || {};
                groupData.firstLevel = typeof groupData.firstLevel != UNDEFINED ? groupData.firstLevel : node.parent().parent().hasClass(KTREEVIEW);
                groupData.length = typeof groupData.length != UNDEFINED ? groupData.length : node.parent().children().length;
                node.removeClass('k-first k-last').addClass(templates.wrapperCssClass(groupData, nodeData));
                wrapper.removeClass('k-top k-mid k-bot').addClass(templates.cssClass(groupData, nodeData));
                var textWrap = wrapper.children('.k-in');
                var isLink = textWrap[0] && textWrap[0].nodeName.toLowerCase() == 'a';
                textWrap.removeClass('k-in k-link k-state-default k-state-disabled').addClass(templates.textClass(nodeData, isLink));
                if (group.length || node.attr('data-hasChildren') == 'true') {
                    wrapper.children('.k-icon').removeClass('k-i-expand k-i-collapse').addClass(templates.toggleButtonClass(nodeData));
                    group.addClass('k-group');
                }
                this._checkboxAria(node);
            },
            _processNodes: function (nodes, callback) {
                var that = this;
                that.element.find(nodes).each(function (index, item) {
                    callback.call(that, index, $(item).closest(NODE));
                });
            },
            dataItem: function (node) {
                var uid = $(node).closest(NODE).attr(kendo.attr('uid')), dataSource = this.dataSource;
                return dataSource && dataSource.getByUid(uid);
            },
            _insertNode: function (nodeData, index, parentNode, insertCallback, collapsed) {
                var that = this, group = subGroup(parentNode), updatedGroupLength = group.children().length + 1, childrenData, groupData = {
                        firstLevel: parentNode.hasClass(KTREEVIEW),
                        expanded: !collapsed,
                        length: updatedGroupLength
                    }, node, i, item, nodeHtml = '', append = function (item, group) {
                        item.appendTo(group);
                    };
                for (i = 0; i < nodeData.length; i++) {
                    item = nodeData[i];
                    item.index = index + i;
                    nodeHtml += that._renderItem({
                        group: groupData,
                        item: item
                    });
                }
                node = $(nodeHtml);
                if (!node.length) {
                    return;
                }
                that.angular('compile', function () {
                    return {
                        elements: node.get(),
                        data: nodeData.map(function (item) {
                            return { dataItem: item };
                        })
                    };
                });
                if (!group.length) {
                    group = $(that._renderGroup({ group: groupData })).appendTo(parentNode);
                }
                insertCallback(node, group);
                if (parentNode.hasClass('k-item')) {
                    updateNodeHtml(parentNode);
                    that._updateNodeClasses(parentNode);
                }
                that._updateNodeClasses(node.prev().first());
                that._updateNodeClasses(node.next().last());
                that._checkboxAria(node);
                for (i = 0; i < nodeData.length; i++) {
                    item = nodeData[i];
                    if (item.hasChildren) {
                        childrenData = item.children.data();
                        if (childrenData.length) {
                            that._insertNode(childrenData, item.index, node.eq(i), append, !that._expanded(node.eq(i)));
                        }
                    }
                }
                return node;
            },
            _checkboxAria: function (nodes) {
                var text;
                nodes.each(function (index, node) {
                    node = $(node);
                    text = node.find('.k-in:first').text();
                    $(node).find('> div .k-checkbox-wrapper [type=checkbox]').attr(ARIALABEL, text);
                });
            },
            _updateNodes: function (items, field) {
                var that = this;
                var i, node, nodeWrapper, item, isChecked, isCollapsed;
                var context = {
                    treeview: that.options,
                    item: item
                };
                var render = field != 'expanded' && field != 'checked';
                function setCheckedState(root, state) {
                    root.find('.k-checkbox-wrapper :checkbox').not('[disabled]').prop(CHECKED, state).data(INDETERMINATE, false).prop(INDETERMINATE, false);
                }
                if (field == 'selected') {
                    item = items[0];
                    node = that.findByUid(item.uid).find('.k-in:first').removeClass('k-state-hover').toggleClass('k-state-selected', item[field]).end();
                    if (item[field]) {
                        that.current(node);
                    }
                    node.attr(ARIASELECTED, !!item[field]);
                } else {
                    var elements = $.map(items, function (item) {
                        return that.findByUid(item.uid).children('div');
                    });
                    if (render) {
                        that.angular('cleanup', function () {
                            return { elements: elements };
                        });
                    }
                    for (i = 0; i < items.length; i++) {
                        context.item = item = items[i];
                        nodeWrapper = elements[i];
                        node = nodeWrapper.parent();
                        if (render) {
                            nodeWrapper.children('.k-in').html(that.templates.itemContent(context));
                        }
                        if (field == CHECKED) {
                            isChecked = item[field];
                            setCheckedState(nodeWrapper, isChecked);
                            if (that.options.checkboxes.checkChildren) {
                                setCheckedState(node.children('.k-group'), isChecked);
                                that._setChecked(item.children, isChecked);
                                that._bubbleIndeterminate(node);
                            }
                        } else if (field == 'expanded') {
                            that._toggle(node, item, item[field]);
                        } else if (field == 'enabled') {
                            node.find('.k-checkbox-wrapper :checkbox').prop('disabled', !item[field]);
                            isCollapsed = !nodeContents(node).is(VISIBLE);
                            node.removeAttr(ARIADISABLED);
                            if (!item[field]) {
                                if (item.selected) {
                                    item.set('selected', false);
                                }
                                if (item.expanded) {
                                    item.set('expanded', false);
                                }
                                isCollapsed = true;
                                node.attr(ARIASELECTED, false).attr(ARIADISABLED, true);
                            }
                            that._updateNodeClasses(node, {}, {
                                enabled: item[field],
                                expanded: !isCollapsed
                            });
                        }
                        if (nodeWrapper.length) {
                            this.trigger('itemChange', {
                                item: nodeWrapper,
                                data: item,
                                ns: ui
                            });
                        }
                    }
                    if (render) {
                        that.angular('compile', function () {
                            return {
                                elements: elements,
                                data: $.map(items, function (item) {
                                    return [{ dataItem: item }];
                                })
                            };
                        });
                    }
                }
            },
            _appendItems: function (index, items, parentNode) {
                var group = subGroup(parentNode);
                var children = group.children();
                var collapsed = !this._expanded(parentNode);
                if (this.element === parentNode) {
                    index = this.dataSource.view().indexOf(items[0]);
                } else if (items.length) {
                    index = items[0].parent().indexOf(items[0]);
                }
                if (typeof index == UNDEFINED) {
                    index = children.length;
                }
                this._insertNode(items, index, parentNode, function (item, group) {
                    if (index >= children.length) {
                        item.appendTo(group);
                    } else {
                        item.insertBefore(children.eq(index));
                    }
                }, collapsed);
                if (this._expanded(parentNode)) {
                    this._updateNodeClasses(parentNode);
                    subGroup(parentNode).css('display', 'block');
                }
            },
            _refreshChildren: function (parentNode, items, index) {
                var i, children, child;
                var options = this.options;
                var loadOnDemand = options.loadOnDemand;
                var checkChildren = options.checkboxes && options.checkboxes.checkChildren;
                subGroup(parentNode).empty();
                if (!items.length) {
                    updateNodeHtml(parentNode);
                } else {
                    this._appendItems(index, items, parentNode);
                    children = subGroup(parentNode).children();
                    if (loadOnDemand && checkChildren) {
                        this._bubbleIndeterminate(children.last());
                    }
                    for (i = 0; i < children.length; i++) {
                        child = children.eq(i);
                        this.trigger('itemChange', {
                            item: child.children('div'),
                            data: this.dataItem(child),
                            ns: ui
                        });
                    }
                }
            },
            _refreshRoot: function (items) {
                var groupHtml = this._renderGroup({
                    items: items,
                    group: {
                        firstLevel: true,
                        expanded: true
                    }
                });
                if (this.root.length) {
                    this._angularItems('cleanup');
                    var group = $(groupHtml);
                    this.root.attr('class', group.attr('class')).html(group.html());
                } else {
                    this.root = this.wrapper.html(groupHtml).children('ul');
                }
                this.root.attr('role', 'tree');
                var elements = this.root.children('.k-item');
                for (var i = 0; i < items.length; i++) {
                    this.trigger('itemChange', {
                        item: elements.eq(i),
                        data: items[i],
                        ns: ui
                    });
                }
                this._angularItems('compile');
            },
            refresh: function (e) {
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
                        if (!loadOnDemand || items[i].expanded || items[i]._loaded) {
                            items[i].load();
                        }
                    }
                }
                this.trigger(DATABOUND, { node: node ? parentNode : undefined });
                if (this.options.checkboxes.checkChildren) {
                    this.updateIndeterminate();
                }
            },
            _error: function (e) {
                var node = e.node && this.findByUid(e.node.uid);
                var retryHtml = this.templates.retry({ messages: this.options.messages });
                if (node) {
                    this._progress(node, false);
                    this._expanded(node, false);
                    nodeIcon(node).addClass('k-i-reload');
                    e.node.loaded(false);
                } else {
                    this._progress(false);
                    this.element.html(retryHtml);
                }
            },
            _retryRequest: function (e) {
                e.preventDefault();
                this.dataSource.fetch();
            },
            expand: function (nodes) {
                this._processNodes(nodes, function (index, item) {
                    this.toggle(item, true);
                });
            },
            collapse: function (nodes) {
                this._processNodes(nodes, function (index, item) {
                    this.toggle(item, false);
                });
            },
            enable: function (nodes, enable) {
                if (typeof nodes === 'boolean') {
                    enable = nodes;
                    nodes = this.items();
                } else {
                    enable = arguments.length == 2 ? !!enable : true;
                }
                this._processNodes(nodes, function (index, item) {
                    this.dataItem(item).set('enabled', enable);
                });
            },
            current: function (node) {
                var that = this, current = that._current, element = that.element, id = that._ariaId;
                if (arguments.length > 0 && node && node.length) {
                    if (current) {
                        if (current[0].id === id) {
                            current.removeAttr('id');
                        }
                        current.find('.k-in:first').removeClass('k-state-focused');
                    }
                    current = that._current = $(node, element).closest(NODE);
                    current.find('.k-in:first').addClass('k-state-focused');
                    id = current[0].id || id;
                    if (id) {
                        that.wrapper.removeAttr('aria-activedescendant');
                        current.attr('id', id);
                        that.wrapper.attr('aria-activedescendant', id);
                    }
                    return;
                }
                if (!current) {
                    current = that._nextVisible($());
                }
                return current;
            },
            select: function (node) {
                var that = this, element = that.element;
                if (!arguments.length) {
                    return element.find('.k-state-selected').closest(NODE);
                }
                node = $(node, element).closest(NODE);
                element.find('.k-state-selected').each(function () {
                    var dataItem = that.dataItem(this);
                    if (dataItem) {
                        dataItem.set('selected', false);
                        delete dataItem.selected;
                    } else {
                        $(this).removeClass('k-state-selected');
                    }
                });
                if (node.length) {
                    that.dataItem(node).set('selected', true);
                    that._clickTarget = node;
                }
                that.trigger(CHANGE);
            },
            _toggle: function (node, dataItem, expand) {
                var options = this.options;
                var contents = nodeContents(node);
                var direction = expand ? 'expand' : 'collapse';
                var loaded;
                if (contents.data('animating')) {
                    return;
                }
                loaded = dataItem && dataItem.loaded();
                if (expand && !loaded) {
                    if (options.loadOnDemand) {
                        this._progress(node, true);
                    }
                    contents.remove();
                    dataItem.load();
                } else {
                    this._updateNodeClasses(node, {}, { expanded: expand });
                    if (!expand) {
                        contents.css('height', contents.height()).css('height');
                    }
                    contents.kendoStop(true, true).kendoAnimate(extend({ reset: true }, options.animation[direction], {
                        complete: function () {
                            if (expand) {
                                contents.css('height', '');
                            }
                        }
                    }));
                }
            },
            toggle: function (node, expand) {
                node = $(node);
                if (!nodeIcon(node).is('.k-i-expand, .k-i-collapse')) {
                    return;
                }
                if (arguments.length == 1) {
                    expand = !this._expanded(node);
                }
                this._expanded(node, expand);
            },
            destroy: function () {
                var that = this;
                Widget.fn.destroy.call(that);
                that.wrapper.off(NS);
                that._unbindDataSource();
                if (that.dragging) {
                    that.dragging.destroy();
                }
                kendo.destroy(that.element);
                that.root = that.wrapper = that.element = null;
            },
            _expanded: function (node, value) {
                var expandedAttr = kendo.attr('expanded');
                var dataItem = this.dataItem(node);
                var expanded = value;
                var direction = expanded ? 'expand' : 'collapse';
                if (arguments.length == 1) {
                    return node.attr(expandedAttr) === 'true' || dataItem && dataItem.expanded;
                }
                if (nodeContents(node).data('animating')) {
                    return;
                }
                if (!this._trigger(direction, node)) {
                    if (dataItem) {
                        dataItem.set('expanded', expanded);
                        expanded = dataItem.expanded;
                    }
                    if (expanded) {
                        node.attr(expandedAttr, 'true');
                        node.attr('aria-expanded', 'true');
                    } else {
                        node.removeAttr(expandedAttr);
                        node.attr('aria-expanded', 'false');
                    }
                }
            },
            _progress: function (node, showProgress) {
                var element = this.element;
                var loadingText = this.templates.loading({ messages: this.options.messages });
                if (arguments.length == 1) {
                    showProgress = node;
                    if (showProgress) {
                        element.html(loadingText);
                    } else {
                        element.empty();
                    }
                } else {
                    nodeIcon(node).toggleClass('k-i-loading', showProgress).removeClass('k-i-reload');
                }
            },
            text: function (node, text) {
                var dataItem = this.dataItem(node), fieldBindings = this.options[bindings.text], level = dataItem.level(), length = fieldBindings.length, field = fieldBindings[Math.min(level, length - 1)];
                if (text) {
                    dataItem.set(field, text);
                } else {
                    return dataItem[field];
                }
            },
            _objectOrSelf: function (node) {
                return $(node).closest('[data-role=treeview]').data('kendoTreeView') || this;
            },
            _dataSourceMove: function (nodeData, group, parentNode, callback) {
                var referenceDataItem, destTreeview = this._objectOrSelf(parentNode || group), destDataSource = destTreeview.dataSource;
                var loadPromise = $.Deferred().resolve().promise();
                if (parentNode && parentNode[0] != destTreeview.element[0]) {
                    referenceDataItem = destTreeview.dataItem(parentNode);
                    if (!referenceDataItem.loaded()) {
                        destTreeview._progress(parentNode, true);
                        loadPromise = referenceDataItem.load();
                    }
                    if (parentNode != this.root) {
                        destDataSource = referenceDataItem.children;
                        if (!destDataSource || !(destDataSource instanceof HierarchicalDataSource)) {
                            referenceDataItem._initChildren();
                            referenceDataItem.loaded(true);
                            destDataSource = referenceDataItem.children;
                        }
                    }
                }
                nodeData = this._toObservableData(nodeData);
                return callback.call(destTreeview, destDataSource, nodeData, loadPromise);
            },
            _toObservableData: function (node) {
                var dataItem = node, dataSource, uid;
                if (isJQueryInstance(node) || isDomElement(node)) {
                    dataSource = this._objectOrSelf(node).dataSource;
                    uid = $(node).attr(kendo.attr('uid'));
                    dataItem = dataSource.getByUid(uid);
                    if (dataItem) {
                        dataItem = dataSource.remove(dataItem);
                    }
                }
                return dataItem;
            },
            _insert: function (data, model, index) {
                if (!(model instanceof kendo.data.ObservableArray)) {
                    if (!isArray(model)) {
                        model = [model];
                    }
                } else {
                    model = model.toJSON();
                }
                var parentNode = data.parent();
                if (parentNode && parentNode._initChildren) {
                    parentNode.hasChildren = true;
                    parentNode._initChildren();
                }
                data.splice.apply(data, [
                    index,
                    0
                ].concat(model));
                return this.findByUid(data[index].uid);
            },
            insertAfter: insertAction(1),
            insertBefore: insertAction(0),
            append: function (nodeData, parentNode, success) {
                var group = this.root;
                if (parentNode && nodeData instanceof jQuery && parentNode[0] === nodeData[0]) {
                    return;
                }
                parentNode = parentNode && parentNode.length ? parentNode : null;
                if (parentNode) {
                    group = subGroup(parentNode);
                }
                return this._dataSourceMove(nodeData, group, parentNode, function (dataSource, model, loadModel) {
                    var inserted;
                    var that = this;
                    function add() {
                        if (parentNode) {
                            that._expanded(parentNode, true);
                        }
                        var data = dataSource.data(), index = Math.max(data.length, 0);
                        return that._insert(data, model, index);
                    }
                    loadModel.done(function () {
                        inserted = add();
                        success = success || $.noop;
                        success(inserted);
                    });
                    return inserted || null;
                });
            },
            _remove: function (node, keepData) {
                var that = this, parentNode, prevSibling, nextSibling;
                node = $(node, that.element);
                this.angular('cleanup', function () {
                    return { elements: node.get() };
                });
                parentNode = node.parent().parent();
                prevSibling = node.prev();
                nextSibling = node.next();
                node[keepData ? 'detach' : 'remove']();
                if (parentNode.hasClass('k-item')) {
                    updateNodeHtml(parentNode);
                    that._updateNodeClasses(parentNode);
                }
                that._updateNodeClasses(prevSibling);
                that._updateNodeClasses(nextSibling);
                return node;
            },
            remove: function (node) {
                var dataItem = this.dataItem(node);
                if (dataItem) {
                    this.dataSource.remove(dataItem);
                }
            },
            detach: function (node) {
                return this._remove(node, true);
            },
            findByText: function (text) {
                return $(this.element).find('.k-in').filter(function (i, element) {
                    return $(element).text() == text;
                }).closest(NODE);
            },
            findByUid: function (uid) {
                var items = this.element.find('.k-item');
                var uidAttr = kendo.attr('uid');
                var result;
                for (var i = 0; i < items.length; i++) {
                    if (items[i].getAttribute(uidAttr) == uid) {
                        result = items[i];
                        break;
                    }
                }
                return $(result);
            },
            expandPath: function (path, complete) {
                var treeview = this;
                var nodeIds = path.slice(0);
                var callback = complete || $.noop;
                function proceed() {
                    nodeIds.shift();
                    if (nodeIds.length) {
                        expand(nodeIds[0]).then(proceed);
                    } else {
                        callback.call(treeview);
                    }
                }
                function expand(id) {
                    var result = $.Deferred();
                    var node = treeview.dataSource.get(id);
                    if (node) {
                        if (node.loaded()) {
                            node.set('expanded', true);
                            result.resolve();
                        } else {
                            treeview._progress(treeview.findByUid(node.uid), true);
                            node.load().then(function () {
                                node.set('expanded', true);
                                result.resolve();
                            });
                        }
                    } else {
                        result.resolve();
                    }
                    return result.promise();
                }
                expand(nodeIds[0]).then(proceed);
            },
            _parentIds: function (node) {
                var parent = node && node.parentNode();
                var parents = [];
                while (parent && parent.parentNode) {
                    parents.unshift(parent.id);
                    parent = parent.parentNode();
                }
                return parents;
            },
            expandTo: function (node) {
                if (!(node instanceof kendo.data.Node)) {
                    node = this.dataSource.get(node);
                }
                var parents = this._parentIds(node);
                this.expandPath(parents);
            },
            _renderItem: function (options) {
                if (!options.group) {
                    options.group = {};
                }
                options.treeview = this.options;
                options.r = this.templates;
                return this.templates.item(options);
            },
            _renderGroup: function (options) {
                var that = this;
                options.renderItems = function (options) {
                    var html = '', i = 0, items = options.items, len = items ? items.length : 0, group = options.group;
                    group.length = len;
                    for (; i < len; i++) {
                        options.group = group;
                        options.item = items[i];
                        options.item.index = i;
                        html += that._renderItem(options);
                    }
                    return html;
                };
                options.r = that.templates;
                return that.templates.group(options);
            }
        });
        ui.plugin(TreeView);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));