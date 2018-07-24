/** 
 * Kendo UI v2018.2.613 (http://www.telerik.com/kendo-ui)                                                                                                                                               
 * Copyright 2018 Telerik AD. All rights reserved.                                                                                                                                                      
 *                                                                                                                                                                                                      
 * Kendo UI commercial licenses may be obtained at                                                                                                                                                      
 * http://www.telerik.com/purchase/license-agreement/kendo-ui-complete                                                                                                                                  
 * If you do not own a commercial license, this file shall be governed by the trial license terms.                                                                                                      
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       
                                                                                                                                                                                                       

*/
(function (f, define) {
    define('kendo.imagebrowser', ['kendo.filebrowser'], f);
}(function () {
    var __meta__ = {
        id: 'imagebrowser',
        name: 'ImageBrowser',
        category: 'web',
        description: '',
        hidden: true,
        depends: ['filebrowser']
    };
    (function ($, undefined) {
        var kendo = window.kendo, FileBrowser = kendo.ui.FileBrowser, isPlainObject = $.isPlainObject, proxy = $.proxy, extend = $.extend, browser = kendo.support.browser, isFunction = kendo.isFunction, trimSlashesRegExp = /(^\/|\/$)/g, ERROR = 'error', NS = '.kendoImageBrowser', NAMEFIELD = 'name', SIZEFIELD = 'size', TYPEFIELD = 'type', DEFAULTSORTORDER = {
                field: TYPEFIELD,
                dir: 'asc'
            }, EMPTYTILE = kendo.template('<li class="k-tile-empty"><strong>${text}</strong></li>');
        extend(true, kendo.data, {
            schemas: {
                'imagebrowser': {
                    data: function (data) {
                        return data.items || data || [];
                    },
                    model: {
                        id: 'name',
                        fields: {
                            name: 'name',
                            size: 'size',
                            type: 'type'
                        }
                    }
                }
            }
        });
        extend(true, kendo.data, {
            transports: {
                'imagebrowser': kendo.data.RemoteTransport.extend({
                    init: function (options) {
                        kendo.data.RemoteTransport.fn.init.call(this, $.extend(true, {}, this.options, options));
                    },
                    _call: function (type, options) {
                        options.data = $.extend({}, options.data, { path: this.options.path() });
                        if (isFunction(this.options[type])) {
                            this.options[type].call(this, options);
                        } else {
                            kendo.data.RemoteTransport.fn[type].call(this, options);
                        }
                    },
                    read: function (options) {
                        this._call('read', options);
                    },
                    create: function (options) {
                        this._call('create', options);
                    },
                    destroy: function (options) {
                        this._call('destroy', options);
                    },
                    update: function () {
                    },
                    options: {
                        read: { type: 'POST' },
                        update: { type: 'POST' },
                        create: { type: 'POST' },
                        destroy: { type: 'POST' }
                    }
                })
            }
        });
        var offsetTop;
        if (browser.msie && browser.version < 8) {
            offsetTop = function (element) {
                return element.offsetTop;
            };
        } else {
            offsetTop = function (element) {
                return element.offsetTop - $(element).height();
            };
        }
        function concatPaths(path, name) {
            if (path === undefined || !path.match(/\/$/)) {
                path = (path || '') + '/';
            }
            return path + name;
        }
        function sizeFormatter(value) {
            if (!value) {
                return '';
            }
            var suffix = ' bytes';
            if (value >= 1073741824) {
                suffix = ' GB';
                value /= 1073741824;
            } else if (value >= 1048576) {
                suffix = ' MB';
                value /= 1048576;
            } else if (value >= 1024) {
                suffix = ' KB';
                value /= 1024;
            }
            return Math.round(value * 100) / 100 + suffix;
        }
        var ImageBrowser = FileBrowser.extend({
            init: function (element, options) {
                var that = this;
                options = options || {};
                FileBrowser.fn.init.call(that, element, options);
                that.element.addClass('k-imagebrowser');
            },
            options: {
                name: 'ImageBrowser',
                fileTypes: '*.png,*.gif,*.jpg,*.jpeg'
            },
            value: function () {
                var that = this, selected = that._selectedItem(), path, imageUrl = that.options.transport.imageUrl;
                if (selected && selected.get(TYPEFIELD) === 'f') {
                    path = concatPaths(that.path(), selected.get(NAMEFIELD)).replace(trimSlashesRegExp, '');
                    if (imageUrl) {
                        path = isFunction(imageUrl) ? imageUrl(path) : kendo.format(imageUrl, encodeURIComponent(path));
                    }
                    return path;
                }
            },
            _fileUpload: function (e) {
                var that = this, options = that.options, fileTypes = options.fileTypes, filterRegExp = new RegExp(('(' + fileTypes.split(',').join(')|(') + ')').replace(/\*\./g, '.*.'), 'i'), fileName = e.files[0].name, fileNameField = NAMEFIELD, sizeField = SIZEFIELD, file;
                if (filterRegExp.test(fileName)) {
                    e.data = { path: that.path() };
                    file = that._createFile(fileName);
                    if (!file) {
                        e.preventDefault();
                    } else {
                        file._uploading = true;
                        that.upload.one('success', function (e) {
                            delete file._uploading;
                            var model = that._insertFileToList(file);
                            model.set(fileNameField, e.response[that._getFieldName(fileNameField)]);
                            model.set(sizeField, e.response[that._getFieldName(sizeField)]);
                            that._tiles = that.listView.items().filter('[' + kendo.attr('type') + '=f]');
                            that._scroll();
                        });
                    }
                } else {
                    e.preventDefault();
                    that._showMessage(kendo.format(options.messages.invalidFileType, fileName, fileTypes));
                }
            },
            _content: function () {
                var that = this;
                that.list = $('<ul class="k-reset k-floats k-tiles" />').appendTo(that.element).on('scroll' + NS, proxy(that._scroll, that)).on('dblclick' + NS, 'li', proxy(that._dblClick, that));
                that.listView = new kendo.ui.ListView(that.list, {
                    dataSource: that.dataSource,
                    template: that._itemTmpl(),
                    editTemplate: that._editTmpl(),
                    selectable: true,
                    autoBind: false,
                    dataBinding: function (e) {
                        that.toolbar.find('.k-i-close').parent().addClass('k-state-disabled');
                        if (e.action === 'remove' || e.action === 'sync') {
                            e.preventDefault();
                            kendo.ui.progress(that.listView.element, false);
                        }
                    },
                    dataBound: function () {
                        if (that.dataSource.view().length) {
                            that._tiles = this.items().filter('[' + kendo.attr('type') + '=f]');
                            that._scroll();
                        } else {
                            this.wrapper.append(EMPTYTILE({ text: that.options.messages.emptyFolder }));
                        }
                    },
                    change: proxy(that._listViewChange, that)
                });
            },
            _dataSource: function () {
                var that = this, options = that.options, transport = options.transport, typeSortOrder = extend({}, DEFAULTSORTORDER), nameSortOrder = {
                        field: NAMEFIELD,
                        dir: 'asc'
                    }, schema, dataSource = {
                        type: transport.type || 'imagebrowser',
                        sort: [
                            typeSortOrder,
                            nameSortOrder
                        ]
                    };
                if (isPlainObject(transport)) {
                    transport.path = proxy(that.path, that);
                    dataSource.transport = transport;
                }
                if (isPlainObject(options.schema)) {
                    dataSource.schema = options.schema;
                } else if (transport.type && isPlainObject(kendo.data.schemas[transport.type])) {
                    schema = kendo.data.schemas[transport.type];
                }
                if (that.dataSource && that._errorHandler) {
                    that.dataSource.unbind(ERROR, that._errorHandler);
                } else {
                    that._errorHandler = proxy(that._error, that);
                }
                that.dataSource = kendo.data.DataSource.create(dataSource).bind(ERROR, that._errorHandler);
            },
            _loadImage: function (li) {
                var that = this, element = $(li), dataItem = that.dataSource.getByUid(element.attr(kendo.attr('uid'))), name = dataItem.get(NAMEFIELD), thumbnailUrl = that.options.transport.thumbnailUrl, img = $('<img />', { alt: name }), urlJoin = '?';
                if (dataItem._uploading) {
                    return;
                }
                img.hide().on('load' + NS, function () {
                    $(this).prev().remove().end().addClass('k-image').fadeIn();
                });
                element.find('.k-i-loading').after(img);
                if (isFunction(thumbnailUrl)) {
                    thumbnailUrl = thumbnailUrl(that.path(), encodeURIComponent(name));
                } else {
                    if (thumbnailUrl.indexOf('?') >= 0) {
                        urlJoin = '&';
                    }
                    thumbnailUrl = thumbnailUrl + urlJoin + 'path=' + encodeURIComponent(that.path() + name);
                    if (dataItem._override) {
                        thumbnailUrl += '&_=' + new Date().getTime();
                        delete dataItem._override;
                    }
                }
                img.attr('src', thumbnailUrl);
                li.loaded = true;
            },
            _scroll: function () {
                var that = this;
                if (that.options.transport && that.options.transport.thumbnailUrl) {
                    clearTimeout(that._timeout);
                    that._timeout = setTimeout(function () {
                        var height = kendo._outerHeight(that.list), viewTop = that.list.scrollTop(), viewBottom = viewTop + height;
                        that._tiles.each(function () {
                            var top = offsetTop(this), bottom = top + this.offsetHeight;
                            if (top >= viewTop && top < viewBottom || bottom >= viewTop && bottom < viewBottom) {
                                that._loadImage(this);
                            }
                            if (top > viewBottom) {
                                return false;
                            }
                        });
                        that._tiles = that._tiles.filter(function () {
                            return !this.loaded;
                        });
                    }, 250);
                }
            },
            _itemTmpl: function () {
                var that = this, html = '<li class="k-tile" ' + kendo.attr('uid') + '="#=uid#" ';
                html += kendo.attr('type') + '="${' + TYPEFIELD + '}">';
                html += '#if(' + TYPEFIELD + ' == "d") { #';
                html += '<div class="k-thumb"><span class="k-icon k-i-folder"></span></div>';
                html += '#}else{#';
                if (that.options.transport && that.options.transport.thumbnailUrl) {
                    html += '<div class="k-thumb"><span class="k-icon k-i-loading"></span></div>';
                } else {
                    html += '<div class="k-thumb"><span class="k-icon k-i-file"></span></div>';
                }
                html += '#}#';
                html += '<strong>${' + NAMEFIELD + '}</strong>';
                html += '#if(' + TYPEFIELD + ' == "f") { # <span class="k-filesize">${this.sizeFormatter(' + SIZEFIELD + ')}</span> #}#';
                html += '</li>';
                return proxy(kendo.template(html), { sizeFormatter: sizeFormatter });
            }
        });
        kendo.ui.plugin(ImageBrowser);
    }(window.kendo.jQuery));
    return window.kendo;
}, typeof define == 'function' && define.amd ? define : function (a1, a2, a3) {
    (a3 || a2)();
}));