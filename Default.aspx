<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="StackExchange.Redis_Demo.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.2/css/bootstrap.min.css" />
    <script type="text/javascript" src="https://code.jquery.com/jquery-2.1.3.min.js"></script>
</head>
<body>
    <div class="container">
        <nav class="navbar navbar-inverse">
            <label class="navbar-brand" style="color:white">StackExchange.Redis Demo</label>
        </nav>

        <div class="row">
            <div class="col-xs-12 text-center">
                <button type="button" class="btn btn-primary btn-lg" id="getDataBtn">Populate data and display</button>
                <br />
                <br />
            </div>
        </div>
        <div class="row">
            <div class="col-xs-12">
                <textarea id="resultBox"class="form-control" rows="35" style="overflow-y: scroll"></textarea>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#getDataBtn').click(function () {
                $.ajax({
                    url: "Default.aspx/SetGetData",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        //var result = JSON.stringify(response.d, null, '\t');
                        $("#resultBox").empty(); //('');
                        $("#resultBox").text(response.d);
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            });
        });
    </script>
</body>
</html>
