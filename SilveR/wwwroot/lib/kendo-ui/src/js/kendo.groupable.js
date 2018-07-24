/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('kendo.groupable', [
        'kendo.core',
        'kendo.draganddrop'
    ], f);
}(function () {
    var __meta__ = {
        id: 'groupable',
        name: 'Groupable',
        category: 'framework',
        depends: [
            'core',
            'draganddrop'
        ],
        advanced: true
    };
    (function ($, undefined) {
        var kendo = window.kendo, Widget = kendo.ui.Widget, outerWidth = kendo._outerWidth, proxy = $.proxy, isRtl = false, NS = '.kendoGroupable', CHANGE = 'change', indicatorTmpl = kendo.template('<div class="k-group-indicator" data-#=data.ns#field="${data.field}" data-#=data.ns#title="${data.title || ""}" data-#=data.ns#dir="${data.dir || "asc"}">' + '<a href="\\#" class="k-link">' + '<span class="k-icon k-i-sort-${(data.dir || "asc") == "asc" ? "asc-sm" : "desc-sm"}" title="(sorted ${(data.dir || "asc") == "asc" ? "ascending": "descending"})"></span>' + '${data.title ? data.title: data.field}' + '</a>' + '<a class="k-button k-button-icon k-bare">' + '<span class="k-icon k-i-close"></span>' + '</a>' + '</div>', { useWithBlock: false }), hint = function (target) {
                var title = target.attr(kendo.attr('title'));
                if (title) {
                    title = kendo.htmlEncode(title);
                }
                return $('<div class="k-header k-drag-clue" />').css({
                    width: target.width(),
                    paddingLeft: target.css('paddingLeft'),
                    paddingRight: target.css('paddingRight'),
                    lineHeight: target.height() + 'px',
                    paddingTop: target.css('paddingTop'),
                    paddingBottom: target.css('paddingBottom')
                }).html(title || target.attr(kendo.attr('field'))).prepend('<span class="k-icon k-drag-status k-i-cancel" />');
            }, dropCue = $('<div class="k-grouping-dropclue"/>');
        function dropCueOffsetTop(element) {
            return element.position().top + 3;
        }
        var Groupable = Widget.extend({
            init: function (element, options) {
                var that = this, group = kendo.guid(), intializePositions = proxy(that._intializePositions, that), draggable, horizontalCuePosition, dropCuePositions = that._dropCuePositions = [];
                Widget.fn.init.call(that, element, options);
                isRtl = kendo.support.isRtl(element);
                horizontalCuePosition = isRtl ? 'right' : 'left';
                that.draggable = draggable = that.options.draggable || new kendo.ui.Draggable(that.element, {
                    filter: that.options.draggableElements,
                    hint: hint,
                    group: group
                });
                that.groupContainer = $(that.options.groupContainer, that.element).kendoDropTarget({
                    group: draggable.options.group,
                    dragenter: function (e) {
                        if (that._canDrag(e.draggable.currentTarget)) {
                            e.draggable.hint.find('.k-drag-status').removeClass('k-i-cancel').addClass('k-i-plus');
                            dropCue.css('top', dropCueOffsetTop(that.groupContainer)).css(horizontalCuePosition, 0).appendTo(that.groupContainer);
                        }
                    },
                    dragleave: function (e) {
                        e.draggable.hint.find('.k-drag-status').removeClass('k-i-plus').addClass('k-i-cancel');
                        dropCue.remove();
                    },
                    drop: function (e) {
                        var targetElement = e.draggable.currentTarget, field = targetElement.attr(kendo.attr('field')), title = targetElement.attr(kendo.attr('title')), sourceIndicator = that.indicator(field), dropCuePositions = that._dropCuePositions, lastCuePosition = dropCuePositions[dropCuePositions.length - 1], position;
                        if (!targetElement.hasClass('k-group-indicator') && !that._canDrag(targetElement)) {
                            return;
                        }
                        if (lastCuePosition) {
                            position = that._dropCuePosition(kendo.getOffset(dropCue).left + parseInt(lastCuePosition.element.css('marginLeft'), 10) * (isRtl ? -1 : 1) + parseInt(lastCuePosition.element.css('marginRight'), 10));
                            if (position && that._canDrop($(sourceIndicator), position.element, position.left)) {
                                if (position.before) {
                                    position.element.before(sourceIndicator || that.buildIndicator(field, title));
                                } else {
                                    position.element.after(sourceIndicator || that.buildIndicator(field, title));
                                }
                                that._change();
                            }
                        } else {
                            that.groupContainer.append(that.buildIndicator(field, title));
                            that._change();
                        }
                    }
                }).kendoDraggable({
                    filter: 'div.k-group-indicator',
                    hint: hint,
                    group: draggable.options.group,
                    dragcancel: proxy(that._dragCancel, that),
                    dragstart: function (e) {
                        var element = e.currentTarget, marginLeft = parseInt(element.css('marginLeft'), 10), elementPosition = element.position(), left = isRtl ? elementPosition.left - marginLeft : elementPosition.left + outerWidth(element);
                        intializePositions();
                        dropCue.css({
                            top: dropCueOffsetTop(that.groupContainer),
                            left: left
                        }).appendTo(that.groupContainer);
                        this.hint.find('.k-drag-status').removeClass('k-i-cancel').addClass('k-i-plus');
                    },
                    dragend: function () {
                        that._dragEnd(this);
                    },
                    drag: proxy(that._drag, that)
                }).on('click' + NS, '.k-button', function (e) {
                    e.preventDefault();
                    that._removeIndicator($(this).parent());
                }).on('click' + NS, '.k-link', function (e) {
                    var current = $(this).parent(), newIndicator = that.buildIndicator(current.attr(kendo.attr('field')), current.attr(kendo.attr('title')), current.attr(kendo.attr('dir')) == 'asc' ? 'desc' : 'asc');
                    current.before(newIndicator).remove();
                    that._change();
                    e.preventDefault();
                });
                draggable.bind([
                    'dragend',
                    'dragcancel',
                    'dragstart',
                    'drag'
                ], {
                    dragend: function () {
                        that._dragEnd(this);
                    },
                    dragcancel: proxy(that._dragCancel, that),
                    dragstart: function (e) {
                        var element, marginRight, left;
                        if (!that.options.allowDrag && !that._canDrag(e.currentTarget)) {
                            e.preventDefault();
                            return;
                        }
                        intializePositions();
                        if (dropCuePositions.length) {
                            element = dropCuePositions[dropCuePositions.length - 1].element;
                            marginRight = parseInt(element.css('marginRight'), 10);
                            left = element.position().left + outerWidth(element) + marginRight;
                        } else {
                            left = 0;
                        }
                    },
                    drag: proxy(that._drag, that)
                });
                that.dataSource = that.options.dataSource;
                if (that.dataSource && that._refreshHandler) {
                    that.dataSource.unbind(CHANGE, that._refreshHandler);
                } else {
                    that._refreshHandler = proxy(that.refresh, that);
                }
                if (that.dataSource) {
                    that.dataSource.bind('change', that._refreshHandler);
                    that.refresh();
                }
            },
            refresh: function () {
                var that = this, dataSource = that.dataSource;
                if (that.groupContainer) {
                    that.groupContainer.empty().append($.map(dataSource.group() || [], function (item) {
                        var fieldName = item.field;
                        var attr = kendo.attr('field');
                        var element = that.element.find(that.options.filter).filter(function () {
                            return $(this).attr(attr) === fieldName;
                        });
                        return that.buildIndicator(item.field, element.attr(kendo.attr('title')), item.dir);
                    }).join(''));
                }
                that._invalidateGroupContainer();
            },
            destroy: function () {
                var that = this;
                Widget.fn.destroy.call(that);
                that.groupContainer.off(NS);
                if (that.groupContainer.data('kendoDropTarget')) {
                    that.groupContainer.data('kendoDropTarget').destroy();
                }
                if (that.groupContainer.data('kendoDraggable')) {
                    that.groupContainer.data('kendoDraggable').destroy();
                }
                if (!that.options.draggable) {
                    that.draggable.destroy();
                }
                if (that.dataSource && that._refreshHandler) {
                    that.dataSource.unbind('change', that._refreshHandler);
                    that._refreshHandler = null;
                }
                that.groupContainer = that.element = that.draggable = null;
            },
            events: ['change'],
            options: {
                name: 'Groupable',
                filter: 'th',
                draggableElements: 'th',
                messages: { empty: 'Drag a column header and drop it here to group by that column' }
            },
            indicator: function (field) {
                var indicators = $('.k-group-indicator', this.groupContainer);
                return $.grep(indicators, function (item) {
                    return $(item).attr(kendo.attr('field')) === field;
                })[0];
            },
            buildIndicator: function (field, title, dir) {
                return indicatorTmpl({
                    field: field.replace(/"/g, '\''),
                    dir: dir,
                    title: title,
                    ns: kendo.ns
                });
            },
            descriptors: function () {
                var that = this, indicators = $('.k-group-indicator', that.groupContainer), aggregates, names, field, idx, length;
                aggregates = that.element.find(that.options.filter).map(function () {
                    var cell = $(this), aggregate = cell.attr(kendo.attr('aggregates')), member = cell.attr(kendo.attr('field'));
                    if (aggregate && aggregate !== '') {
                        names = aggregate.split(',');
                        aggregate = [];
                        for (idx = 0, length = names.length; idx < length; idx++) {
                            aggregate.push({
                                field: member,
                                aggregate: names[idx]
                            });
                        }
                    }
                    return aggregate;
                }).toArray();
                return $.map(indicators, function (item) {
                    item = $(item);
                    field = item.attr(kendo.attr('field'));
                    return {
                        field: field,
                        dir: item.attr(kendo.attr('dir')),
                        aggregates: aggregates || []
                    };
                });
            },
            _removeIndicator: function (indicator) {
                var that = this;
                indicator.remove();
                that._invalidateGroupContainer();
                that._change();
            },
            _change: function () {
                var that = this;
                if (that.dataSource) {
                    var descriptors = that.descriptors();
                    if (that.trigger('change', { groups: descriptors })) {
                        that.refresh();
                        return;
                    }
                    that.dataSource.group(descriptors);
                }
            },
            _dropCuePosition: function (position) {
                var dropCuePositions = this._dropCuePositions;
                if (!dropCue.is(':visible') || dropCuePositions.length === 0) {
                    return;
                }
                position = Math.ceil(position);
                var lastCuePosition = dropCuePositions[dropCuePositions.length - 1], left = lastCuePosition.left, right = lastCuePosition.right, marginLeft = parseInt(lastCuePosition.element.css('marginLeft'), 10), marginRight = parseInt(lastCuePosition.element.css('marginRight'), 10);
                if (position >= right && !isRtl || position < left && isRtl) {
                    position = {
                        left: lastCuePosition.element.position().left + (!isRtl ? outerWidth(lastCuePosition.element) + marginRight : -marginLeft),
                        element: lastCuePosition.element,
                        before: false
                    };
                } else {
                    position = $.grep(dropCuePositions, function (item) {
                        return item.left <= position && position <= item.right || isRtl && position > item.right;
                    })[0];
                    if (position) {
                        position = {
                            left: isRtl ? position.element.position().left + outerWidth(position.element) + marginRight : position.element.position().left - marginLeft,
                            element: position.element,
                            before: true
                        };
                    }
                }
                return position;
            },
            _drag: function (event) {
                var position = this._dropCuePosition(event.x.location);
                if (position) {
                    dropCue.css({
                        left: position.left,
                        right: 'auto'
                    });
                }
            },
            _canDrag: function (element) {
                var field = element.attr(kendo.attr('field'));
                return element.attr(kendo.attr('groupable')) != 'false' && field && (element.hasClass('k-group-indicator') || !this.indicator(field));
            },
            _canDrop: function (source, target, position) {
                var next = source.next(), result = source[0] !== target[0] && (!next[0] || target[0] !== next[0] || (!isRtl && position > next.position().left || isRtl && position < next.position().left));
                return result;
            },
            _dragEnd: function (draggable) {
                var that = this, field = draggable.currentTarget.attr(kendo.attr('field')), sourceIndicator = that.indicator(field);
                if (draggable !== that.options.draggable && !draggable.dropped && sourceIndicator) {
                    that._removeIndicator($(sourceIndicator));
                }
                that._dragCancel();
            },
            _dragCancel: function () {
                dropCue.remove();
                this._dropCuePositions = [];
            },
            _intializePositions: function () {
                var that = this, indicators = $('.k-group-indicator', that.groupContainer), left;
                that._dropCuePositions = $.map(indicators, function (item) {
                    item = $(item);
                    left = kendo.getOffset(item).left;
                    return {
                        left: parseInt(left, 10),
                        right: parseInt(left + outerWidth(item), 10),
                        element: item
                    };
                });
            },
            _invalidateGroupContainer: function () {
                var groupContainer = this.groupContainer;
                if (groupContainer && groupContainer.is(':empty')) {
                    groupContainer.html(this.options.messages.empty);
                }
            }
        });
        kendo.ui.plugin(Groupable);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));