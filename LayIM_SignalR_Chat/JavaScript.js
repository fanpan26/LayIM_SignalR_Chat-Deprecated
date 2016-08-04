/*LocalCacheVersion:0.15*/(function ($) {
    var IA = pageIndAdd;
    IA.scroller;
    IA.pageScroll = null;
    IA.onPageLoad = function () {
        helper.init();
        if (!IA.pageScroll) IA.pageScroll = app.iscroll("#page-indAdd");
    }
    /*帮助方法*/
    var helper = {
        identity: {
            current: 0,
            issave: false
        },//
        isload: function () {
            return !!$('#pageIdentity_idList').html().length;
        },
        init: function () {
            if (!this.isload()) {
                xzbTool.bind.bindID("#pageIdentity_idList", 0, indAdd_title);
                xzbTool.bind.bindIdSelect("#pageIdentitySelect", "#pageIdentity_idList");
                showSelect(IA, "#pageIdentitySelect");
                selectIdentity(IA, "#pageIdentitySelect");
                if (!IA.scroller) IA.scroller = app.iscroll("#ShenFenScroll", false, 0, 0, { scrollX: true, scrollY: false });
            }
            var t = $('#pageIdentity_idList span').eq(0).data('type');
            var all = xzbTool.data.allI();
            var defaultId = 0;
            if (all.length) {
                defaultId = all[0].id;
            }
            this.identity.current = this.identity.current || t || defaultId;
            data.identityId = parseInt(R.getPara('id', 'id'));//接收修改参数
            $('#pageIdentitySelect div[data-tag="' + helper.identity.current + '"]').first().addClass('show');
            data.loadDetail();//加载详细内容（修改的时候用）
        }

    };
    /*数据*/
    var data = {
        identityId: 0,
        selected: {
            student: { flag: 0, value: 0 },
            schoolfriend: { flag: 0, value: 0 },
            teacher: {
                xi: { flag: 0, value: 0 },
                zn: { flag: 0, value: 0 }
            },
            worker: {
                zhiwu: '',
                company: { flag: 0, value: 0 }
            }
        },
        //根据flag和classid获取相应的内容
        getSelectedClass: function () {

        },
        getPosition: function () {

        },
        loadDetail: function (id) {
            var dataId = id || data.identityId;
            if (!dataId) { return; }
            ajaxTool.ajaxGet(appUrl('user/iddetail'), { id: dataId, cvNumber: getCurrentCV() }, function (result) {
                if (result.result == 10000) {
                    data.resetIdentity(result.data);
                } else {
                    log("加载身份详细信息失败");
                }
            });
        },
        resetIdentity: function (data) {
            //重置选中项目
            //先选中大菜单
            reset.resetIdentity('#pageIdentitySelect', data);
        },
        getDetailByFlag: function (data, f) {
            var d = $.grep(data, function (item, i) {
                return item.flag == f;
            });
            if (f == 0) { return d; }
            if (d.length) {
                return d[0];
            }
            return [];
        }
    };
    IA.on(".ShenFen span", "touchend", function (e, ev) {
        window.switchIdentity(e, "#pageIdentitySelect");
        helper.identity.current = $(ev.self).data('type');
        log(helper.identity.current);
        IA.pageScroll.refresh();
    })
    IA.on('#pageIdentity_save', 'touchend', function (e, ev) {
        if (helper.identity.issave == true) {
            return;
        }

        helper.identity.issave = true;
        var parameters = createParam({ regid: helper.identity.current, cvNumber: getCurrentCV() }, "#pageIdentitySelect");
        if (!parameters.isLegalData) {
            helper.identity.issave = false;
            return false;
        }
        parameters.identityId = data.identityId || 0;//要修改的Id

        var shetuan = localStorage.getItem("user_identity_special_" + getCurrentCV());
        var hasId = false;
        var isSpecialId = false;
        var childPid = 0;
        if (shetuan) {
            var shetuanJson = JSON.parse(shetuan);
            var flag = shetuanJson.flag;

            var childPlats = xzbTool.data.childPlats();


            var arr = shetuanJson.arr;
            console.log(parameters.arrObjOld);
            $.each(parameters.arrObjOld, function (i, item) {
                var j = JSON.parse(item);
                if (j.flag == flag) {
                    //是特殊身份
                    isSpecialId = true;
                    for (var n = 0; n < arr.length; n++) {
                        if (j.joinid == arr[n]) {
                            hasId = true;
                        }
                    }
                    //获取子平台id
                    var childPidArr = $.grep(childPlats, function (item, i) {
                        return item.joinid == j.joinid
                    });
                    if (childPidArr.length) {
                        childPid = childPidArr[0].id;
                    }
                }
            });
        }
        if (hasId && !data.identityId) {
            app.alert('您已经有该组织的身份啦，看看别的组织吧');
            helper.identity.issave = false;
            return;
        }

        //特殊身份添加
        if (isSpecialId) {
            var url = appUrl("user/joinplat");
            //通过特殊身份计算社团的身份 成员 t=5 粉丝 t = 6
            var ts = xzbTool.data.specialType();
            var arr = $.grep(ts, function (item, i) {
                return item.id == parameters.regid;
            });
            var t = 0;
            if (arr.length) {
                t = arr[0].t + 3;
            }
            if (t == 0 || childPid == 0) {
                app.alert('参数有误，请刷新页面重试');
                helper.identity.issave = false;
                return;
            }

            if (data.identityId > 0) {
                //先删除后添加
                ajaxTool.ajaxPost(appUrl('user/deluserid'), { cvnumber: getCurrentCV(), id: data.identityId }, function (result) {
                    if (result.result == 10000) {
                        ajaxTool.ajaxPost(url, { cvNumber: getCurrentCV(), identityType: t, pid: 0, childPid: childPid }, function (result) {
                            if (result.result == 10000) {
                                //保存成功，需要重置用户信息
                                appUser.set({ cvnumber: getCurrentCV(), pid: 0 }, function (result) {
                                    //重新设置用户身份信息
                                    ExperLocData();
                                    helper.identity.issave = false;
                                    R.to('pageIndentity?code=ok', 10);//返回身份页面
                                });
                            } else {
                                app.alert('保存失败，请重试');
                            }
                        });
                    } else {
                        app.alert('保存失败，请重试');
                    }
                });

            } else {
                ajaxTool.ajaxPost(url, { cvNumber: getCurrentCV(), identityType: t, pid: 0, childPid: childPid }, function (result) {
                    if (result.result == 10000) {
                        //保存成功，需要重置用户信息
                        appUser.set({ cvnumber: getCurrentCV(), pid: 0 }, function (result) {
                            //重新设置用户身份信息
                            ExperLocData();
                            helper.identity.issave = false;
                            R.to('pageIndentity?code=ok', 10);//返回身份页面
                        });
                    } else {
                        app.alert('保存失败，请重试');
                    }
                });
            }
            return;
        } else {
            //普通身份添加
            ajaxTool.ajaxPost(appUrl('user/adduserid'), parameters, function (result) {
                if (result.result == 10000) {
                    //保存成功，需要重置用户信息
                    appUser.set({ cvnumber: getCurrentCV(), pid: 0 }, function (result) {
                        //重新设置用户身份信息
                        ExperLocData();
                        helper.identity.issave = false;
                        R.to('pageIndentity?code=ok', 10);//返回身份页面
                    });
                } else {
                    app.alert('保存失败，请重试');
                }
            });
        }
    })

    IA.on(".pull-left", "touchend", function () {
        $("#pageindentityAdd .selecter-in").removeClass("selecter-in");
        $('.modal-over-back').removeClass("modal-overlay-visible");
        //返回之前就关闭层
        R.to('pageIndentity', 10);
    })
    //关闭选择
    IA.on(".modal-over-back", "touchend", function (e, ev) {
        $("#pageindentityAdd .selecter-in").removeClass("selecter-in");
        $(ev.self).removeClass("modal-overlay-visible");
    });


    var reset = {
        resetIdentity: function (selectId, d) {
            var identity = data.getDetailByFlag(d, 9);
            var tagIndex = identity.joinid;
            var that = $('#pageIdentity_idList').find('span[data-type="' + tagIndex + '"]');
            var template = xzbTool.data.alltemplateType();
            var topSelectId = $(selectId).find("#tag" + tagIndex);
            if (!that.hasClass("shenfen-curent")) {
                that.addClass("shenfen-curent").siblings("span").removeClass("shenfen-curent");
                topSelectId.addClass('show').show().siblings().removeClass('show').hide();
            }
            //单条加载
            var t = parseInt(tagIndex);
            helper.identity.current = t;
            var bj = data.getDetailByFlag(d, 5);
            var year = data.getDetailByFlag(d, 8);
            var eid = data.getDetailByFlag(d, 1);
            var xi = data.getDetailByFlag(d, 3);//系
            var zn = data.getDetailByFlag(d, 2);//职能
            switch (t) {
                case 1://学生
                    if (bj) {
                        $('#tag1>ul>li').first().find('span').text(bj.identity_name);
                        $('#list5>ul>li').removeClass('current');
                        $('#list5>ul').find('li[data-joinid="' + year.joinid + '"]').addClass('current');
                        $('.selecter-right-title').find('span').removeClass('current');
                        $('.selecter-right-title').find('span[data-joinid="' + eid.joinid + '"]').addClass('current');
                        xzbTool.bind.bindClass('#tag1 #banji', { year: year.joinid, eid: eid.joinid });
                        $('#banji>li').find('span[data-joinid="' + bj.joinid + '"]').addClass('on');
                    }
                    break;
                case 2://校友
                    if (bj) {
                        log(bj);
                        $('#tag2>ul>li').first().find('span').text(bj.identity_name);
                        $('#list5>ul>li').removeClass('current');
                        $('#list5>ul').find('li[data-joinid="' + year.joinid + '"]').addClass('current');
                        $('.selecter-right-title').find('span').removeClass('current');
                        $('.selecter-right-title').find('span[data-joinid="' + eid.joinid + '"]').addClass('current');
                        xzbTool.bind.bindClass('#tag2 #banji', { year: year.joinid, eid: eid.joinid });
                        $('#banji>li').find('span[data-joinid="' + bj.joinid + '"]').addClass('on');
                    }
                    break;
                case 3://教师
                    $('#tag3>ul').first().find('li').find('span').text(xi.identity_name);
                    $('#tag3>ul').eq(1).find('li').find('span').text(zn.identity_name);
                    this.resetSingle('list3', xi.joinid);
                    this.resetSingle('list2', zn.joinid);
                    break;
                case 4://职工
                    var zg = data.getDetailByFlag(d, 0);//系
                    $('#tag4').find('input[type="text"]').val(zg.length ? zg[0].identity_name : '');
                    $('#tag4>ul>li').first().find('span').text(xi.identity_name);
                    this.resetSingle('list3', xi.joinid);
                    break;
                default:
                    //给填空赋值
                    var tk = data.getDetailByFlag(d, 0);//系
                    if (tk && tk.length) {
                        for (var i = 0; i < tk.length; i++) {
                            var name = tk[i].identity_name;
                            var cid = tk[i].joinid;
                            $('input[data-columnid="' + cid + '"]').first().val(name);
                        }
                    }
                    //在判断有没有选择
                    var s = xzbTool.data.allIDSelected();
                    var regId = tagIndex;
                    var selected = $.grep(s, function (item, i) {
                        return item.RegId == regId && item.Operation == "1";
                    });
                    for (var i = 0; i < selected.length; i++) {
                        var item = data.getDetailByFlag(d, selected[i].DataId);//系
                        if (item) {
                            $('span[data-selectid="' + selected[i].DataId + '"]').text(item.identity_name);
                            this.resetSingle('list' + selected[i].DataId, item.joinid);
                        }
                    }
                    break;
            }
        },
        resetSingle: function (parent, id) {
            $('#' + parent + '>ul>li>span').removeClass('on');
            $('#' + parent + '>ul>li').find('span[data-joinid="' + id + '"]').addClass('on');
        },
        resetTitle: function () {

        },
        resetBj: function (bj) { },
    };
})(jQuery);