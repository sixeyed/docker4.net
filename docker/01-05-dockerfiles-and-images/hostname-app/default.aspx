<%@ Import Namespace="System" %>
<%@ Page Language="c#"%>

<script runat="server">
    public string GetMachineName()
    {
        return Environment.MachineName;
    }
</script>

<html>
    <head>
        <style>
        body {
        	background: #f5f5f5;
            background-color: #f5f5f5;
    		margin-bottom: 0px!important;
        }

        div{
            font-family: 'Geomanist', sans-serif;
  			font-weight: normal; 			
            color: #616178;
            width: 85%;
            margin: 0 auto;
            position: relative;
            margin-top: 180px;
            transform: translateY(-50%);
        }

        .footer {
            position: fixed;
            bottom: 0;
            font-size: 14pt;
            text-align: center;
            background-color: #ede8ef;
            width: 100%;
            height: 50px;
        }

        h1{
            font-size: 50pt
        }
        h2{
            font-size: 28pt
        }
        </style>
    </head>

    <body>
        <div>
            <h1>Hello from <% =GetMachineName() %>!</h1>
        </div>
        <div class="footer">
            A demo app from the Udemy course <a href="https://docker4.net">Docker for .NET Apps</a>
        </div>
    </body>

</html>