/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('kendo.buttongroup', ['kendo.core'], f);
}(function () {
    var __meta__ = {
        id: 'buttongroup',
        name: 'ButtonGroup',
        category: 'web',
        description: 'The Kendo ButtonGroup widget is a linear set of grouped buttons.',
        depends: ['core']
    };
    (function ($, undefined) {
        var kendo = window.kendo;
        var ui = kendo.ui;
        var Widget = ui.Widget;
        var keys = kendo.keys;
        var proxy = $.proxy;
        var template = kendo.template;
        var NS = '.kendoButtonGroup';
        var KWIDGET = 'k-widget';
        var KBUTTONGROUP = 'k-button-group';
        var KBUTTON = 'k-button';
        var KBUTTONICONTEXT = 'k-button-icontext';
        var KBUTTONICON = 'k-button-icon';
        var ACTIVE = 'k-state-active';
        var FOCUSED = 'k-state-focused';
        var DISABLED = 'k-state-disabled';
        var SELECT = 'select';
        var CLICK = 'click';
        var KEYDOWN = 'keydown';
        var FOCUS = 'focus';
        var BLUR = 'blur';
        var MOUSEDOWN = 'mousedown';
        var templates = {
            item: template('<span class="#= item.selected ? " ' + ACTIVE + '" : "" #" ' + '#= item.enabled === false ? "disabled" : "" # ' + '# if (item.badge) { #' + kendo.attr('badge') + '="#=item.badge#"' + '# } #' + '>' + '#= icon(iconClass) #' + '#= image(item) #' + '#= text #' + '</span>'),
            image: template('<img alt="icon" src="#=data.imageUrl#" />'),
            icon: template('<span class="#=data#" />'),
            empty: template('')
        };
        function createBadge(value, item) {
            if (value === undefined) {
                return;
            }
            $('<span class="k-badge">' + kendo.htmlEncode(value) + '</span>').appendTo(item);
        }
        var ButtonGroup = Widget.extend({
            init: function (element, options) {
                var that = this;
                Widget.fn.init.call(that, element, options);
                that.wrapper = that.element;
                if (that.options.items) {
                    that._renderItems(that.options.items);
                }
                that.selectedIndices = [];
                that.element.addClass(KWIDGET + ' ' + KBUTTONGROUP).attr('role', 'group').attr('tabindex', that.element.attr('tabindex') || '0').children().each(function () {
                    var item = $(this);
                    that._updateClasses.bind(that)(item);
                });
                that._enable = true;
                if (!that.options.enable) {
                    that._enable = false;
                    that.element.attr('aria-disabled', true).addClass(DISABLED);
                }
                that.select(that.options.index);
                that.element.on(CLICK + NS, '.' + KBUTTON, proxy(that._click, that)).on(FOCUS + NS, proxy(that._focus, that)).on(KEYDOWN + NS, proxy(that._keyDown, that)).on(BLUR + NS, function () {
                    that.preventFocus = false;
                    that.element.find('.' + KBUTTON).removeClass(FOCUSED);
                }).on(MOUSEDOWN + NS, function () {
                    that.preventFocus = true;
                });
            },
            events: [SELECT],
            options: {
                name: 'ButtonGroup',
                selection: 'single',
                index: -1,
                enable: true
            },
            current: function () {
                return this.element.find('.' + ACTIVE);
            },
            _renderItems: function (items) {
                var that = this;
                items.forEach(function (item) {
                    var renderedItem = $(templates.item({
                        image: item.imageUrl ? templates.image : templates.empty,
                        icon: !item.imageUrl && (item.iconClass || item.icon) ? templates.icon : templates.empty,
                        iconClass: item.iconClass || 'k-icon k-i-' + item.icon,
                        item: item,
                        text: item.text ? item.encoded === false ? item.text : kendo.htmlEncode(item.text) : ''
                    }));
                    if (item.attributes) {
                        renderedItem.attr(item.attributes);
                    }
                    if (item.iconClass || item.icon || item.imageUrl) {
                        renderedItem.addClass(item.text ? 'k-button-icontext' : 'k-button-icon');
                    }
                    renderedItem.appendTo(that.element);
                });
            },
            _focus: function () {
                var element = $(this.element);
                if (this.preventFocus) {
                    return;
                }
                if (element.find('.' + ACTIVE).length) {
                    element.find('.' + ACTIVE).first().focus().addClass(FOCUSED);
                } else {
                    element.children().first().focus().addClass(FOCUSED);
                }
            },
            _keyDown: function (e) {
                var that = this;
                var buttonGroup = $(that.element);
                var focusableItems = buttonGroup.find('.' + KBUTTON);
                var focusedElement = buttonGroup.find('.' + FOCUSED);
                var currentIndex = focusableItems.index(focusedElement);
                var isRtl = kendo.support.isRtl(that.element);
                var itemToFocus;
                if (e.keyCode === keys.LEFT && !isRtl || e.keyCode === keys.RIGHT && isRtl) {
                    focusedElement.removeClass(FOCUSED);
                    itemToFocus = currentIndex === 0 ? focusableItems.eq(focusableItems.length - 1) : $(focusableItems[currentIndex - 1]);
                    itemToFocus.focus().addClass(FOCUSED);
                    e.preventDefault();
                } else if (e.keyCode === keys.LEFT && isRtl || e.keyCode === keys.RIGHT && !isRtl) {
                    focusedElement.removeClass(FOCUSED);
                    itemToFocus = currentIndex + 1 === focusableItems.length ? focusableItems.eq(0) : $(focusableItems[currentIndex + 1]);
                    itemToFocus.focus().addClass(FOCUSED);
                    e.preventDefault();
                } else if (e.keyCode === keys.ENTER || e.keyCode === keys.SPACEBAR) {
                    that._select(focusedElement);
                    e.preventDefault();
                }
            },
            select: function (button) {
                var that = this, ariaPressed, index = -1;
                if (button === undefined || button === -1) {
                    return;
                }
                that.element.find('.' + KBUTTON).removeClass(FOCUSED);
                if (typeof button === 'number') {
                    index = button;
                    button = that.element.children().eq(button);
                } else if (button.nodeType) {
                    button = $(button);
                    index = button.index();
                }
                if (that.options.selection === 'multiple') {
                    ariaPressed = button.attr('aria-pressed') === 'true';
                    button.attr('aria-pressed', !ariaPressed).toggleClass(ACTIVE);
                    if (that.selectedIndices.indexOf(index) === -1) {
                        that.selectedIndices.push(index);
                    } else {
                        that.selectedIndices.splice(that.selectedIndices.indexOf(index), 1);
                    }
                } else {
                    that.selectedIndices = [];
                    that.current().attr('aria-pressed', false).removeClass(ACTIVE);
                    button.attr('aria-pressed', true).addClass(ACTIVE);
                    that.selectedIndices.push(index);
                }
                that.trigger(SELECT, { indices: that.selectedIndices });
            },
            badge: function (item, value) {
                var buttongroup = this.element;
                var button = !isNaN(item) ? buttongroup.children().eq(item) : buttongroup.find(item);
                var validValue = value || value === 0;
                var badge;
                if (!button.length) {
                    return;
                }
                badge = button.children('.k-badge').eq(0);
                if (!badge.length && validValue) {
                    createBadge(kendo.htmlEncode(value), button);
                    return kendo.htmlEncode(value);
                }
                if (validValue) {
                    badge.html(kendo.htmlEncode(value));
                } else if (value === false) {
                    badge.empty().remove();
                    return;
                }
                return badge.html();
            },
            enable: function (enable) {
                if (typeof enable == 'undefined') {
                    enable = true;
                }
                this.element.attr('aria-disabled', !enable).toggleClass(DISABLED, !enable);
                this._enable = this.options.enable = enable;
            },
            destroy: function () {
                var that = this;
                that.element.off(NS);
                Widget.fn.destroy.call(that);
            },
            _updateClasses: function (button) {
                var icon = kendo.attrValue(button, 'icon');
                var badge = kendo.attrValue(button, 'badge');
                var image = button.find('img').addClass('k-image');
                var isEmpty = true;
                button.attr('aria-pressed', false).attr('role', 'button').addClass(KBUTTON);
                if (button.is('[disabled]') || button.hasClass(DISABLED)) {
                    button.addClass(DISABLED).attr('aria-disabled', true).removeAttr('disabled');
                }
                if (button.is('.' + ACTIVE)) {
                    button.removeClass(ACTIVE);
                    if (!button.hasClass(DISABLED) && this.options.selection === 'single' || this.options.selection === 'multiple') {
                        this.select(button[0]);
                    }
                }
                if (!image[0] && icon) {
                    button.prepend($(templates.icon('k-icon k-i-' + icon)));
                }
                button.contents().filter(function () {
                    return !$(this).hasClass('k-icon') && !$(this).hasClass('k-image');
                }).each(function () {
                    if (this.nodeType == 1 || this.nodeType == 3 && $.trim(this.nodeValue).length > 0) {
                        isEmpty = false;
                    }
                });
                if (image[0] || icon) {
                    button.addClass(isEmpty ? KBUTTONICON : KBUTTONICONTEXT);
                }
                if (badge || badge === 0) {
                    createBadge(badge, button);
                }
            },
            _click: function (e) {
                var target = $(e.target).closest('.' + KBUTTON);
                if (e.isDefaultPrevented()) {
                    return;
                }
                this._select(target);
            },
            _select: function (target) {
                var button = target;
                this.element.find('.' + KBUTTON).removeClass(FOCUSED);
                if (!this._enable || button.is('.' + DISABLED)) {
                    button.addClass(FOCUSED);
                    return;
                }
                this.select(target[0]);
                button.addClass(FOCUSED);
            }
        });
        ui.plugin(ButtonGroup);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));