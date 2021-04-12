var Utilitys = (function (instance) {
    //Config
    instance.Configs = (function (config) {
        return config;
    })({});

    //Method
    instance.Methods = (function (method) {
        method.BtnDisabled = function (ele, type) {
            //ele: element's Id/Class  type:1 Disabled  0 Usable
            var element = null;
            var disabledName = "layui-btn-disabled";
            if (ele.substring(0, 1) == "#") {
                element = document.getElementById(ele.substring(1));
            }
            if (ele.substring(0, 1) == ".") {
                element = document.getElementById(ele.substring(1));
            }
            if (type == 1) {
                element.classList.add(disabledName);
            }
            if (type == 0) {
                element.classList.remove(disabledName);
            }
        };
        method.CacheSet=function(param){
            if (param.isPersistence == 1) {
                layui.data(param.tab, { key: param.key, value: param.val });
                return false;
            }
            layui.sessionData(param.tab, { key: param.key, value: param.val });
            return false;
        }
        method.CacheDel=function(param){
            if (param.isPersistence == 1) {
                layui.data(param.tab, { key: param.key, remove: true });
                return false;
            }
            layui.sessionData(param.tab, { key: param.key, remove: true });
            return false;  
        }
        method.CacheQuery=function(param){
            return param.isPersistence == 1 ? layui.data(param.tab) : layui.sessionData(param.tab);
        }

        method.DBSetCache = function (isPersistence, tab, key, val) {
            if (isPersistence == 1) {
                layui.data(tab, { key: key, value: val });
                return false;
            }
            layui.sessionData(tab, { key: key, value: val });
            return false;
        };

        

        method.DBDelCache = function (isPersistence, tab, key) {
            if (isPersistence == 1) {
                layui.data(tab, { key: key, remove: true });
                return false;
            }
            layui.sessionData(tab, { key: key, remove: true });
            return false;
        };
        

        method.DBQueryCache = function (isPersistence, tab) {
            return isPersistence == 1 ? layui.data(tab) : layui.sessionData(tab);
        };

        method.DeviceInfo = function () {
            return layui.device();
        };

        method.Loading = function () {
            return layer.load(1, {
                shade: [0.1, '#fff']
            });
        };

        method.Enter = function () {
            $("body").keydown(function () {
                if (event.keyCode == "13") {
                    $(el).click();
                }
            });
        };

        return method;
    })({});

    return instance;
})({});