<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="ChEncAutoDetect_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>BIG5/GB2312快篩測試</title>
    <style>
        textarea {
            width: 312px;
            height: 50px;
        }

            textarea[readonly] {
                background-color: #eee;
            }

        .result {
            font-size: 10pt;
            text-shadow: 1px 1px 2px #444;
            background-color: #ff7a00;
            color: yellow;
            padding: 4px;
        }

        h3 {
            color: white;
            width: 400px;
            text-align: center;
            display: inline-block;
            padding: 12px;
            font-family: 'Arial Unicode MS';
            background-color: #0e6db0;
            font-size: 16pt;
            font-weight: normal;
            text-shadow: 1px 1px 1px #333;
            margin-bottom: 0px;
        }

        div {
            padding: 6px;
            font-weight: normal;
            font-size: 10pt;
        }

            div.frame {
                border: 1px solid gray;
                width: 410px;
                margin-top: 0px;
                background-color: #b7b7b7;
            }
        td {
            vertical-align: top;
        }
    </style>
</head>
<body>
    <h3>BIG5/GB2312編碼快篩測試 Ver 0.9b</h3>
    <div class="frame">
        <table>
            <tr>
                <td>輸入文字內容:</td>
                <td>
                    <textarea data-bind="value: Text, valueUpdate: 'input'">
        </textarea>
                </td>
            </tr>
            <tr>
                <td>檔案編碼設定:</td>
                <td>
                    <select data-bind="value: Encoding">
                        <option>big5</option>
                        <option>gb2312</option>
                    </select>
                    <input type="button" data-bind="click: Test" value="偵測繁簡體" />
                    <span class="result" data-bind="text: Result"></span>
                </td>
            </tr>
            <tr>
                <td>模擬檔案內容:</td>
                <td>
                    <textarea data-bind="value: Simulate" readonly="readonly">
        </textarea></td>
            </tr>
            <tr>
                <td>二進位資料:</td>
                <td>
                    <textarea data-bind="value: Data" readonly="readonly">
        </textarea></td>
            </tr>
        </table>
    </div>
    <div>
        <a href="https://www.facebook.com/darkthread.net">問題回饋</a>
    </div>
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/knockout/knockout-2.2.1.js"></script>
    <script>
        function myViewModel() {
            var self = this;
            self.Text = ko.observable("中文編碼偵測");
            self.Result = ko.observable("");
            self.Encoding = ko.observable("big-5");
            self.Data = ko.observable();
            self.Simulate = ko.observable();
            function filter(s) {
                return s.replace(/[<>]/g, "");
            }
            self.Test = function () {
                $.post("?mode=analyze", { data: filter(self.Data()) }, function (res) {
                    self.Result(res);
                });
            }
            ko.computed(function () {
                var s = self.Text();
                var e = self.Encoding();
                $.post("?mode=convert", { encoding: e, text: filter(s) }, function (res) {
                    var p = res.split('♞');
                    self.Data(p[0]);
                    self.Simulate(p[1]);
                });
                self.Result("尚未偵測");
            }).extend({ throttle: 500 });

        }
        var vm = new myViewModel();
        ko.applyBindings(vm);
    </script>
</body>
</html>
