<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="ExamApplication.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GutenBerg Exam Project</title>
    <script src="Scripts/jquery-3.4.1.js"></script>
    <link rel="stylesheet" type="text/css" href="//cdn.datatables.net/1.10.19/css/jquery.dataTables.min.css" />
    <script src="//cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js" ></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $.ajax({
                url: 'DBservices.asmx/GetBooksByCity',
                method: 'post',
                dataType: 'json',
                success: function (data) {
                    $('#datatable').dataTable({
                        data: data,
                        'paging': true,
                        'sort': true,
                        'searching': true,
                        columns: [
                            { 'data': 'Id' },
                            { 'data': 'Name' },
                            { 'data': 'Author' },
                        ]
                    });
                }
            });
        });
    </script>
</head>
<body style="font-family:Arial">
    <form id="form1" runat="server">
        <div style="border:1px solid black; padding:3px; width:1200px">
            <table id="datatable">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Author</th>
                    </tr>
                </thead>
            </table>
        </div>    
    </form>
</body>
</html>
