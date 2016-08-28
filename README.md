# aspnetmvc5-serversidepaging

This is an example ASP.NET MVC 5 application with datatable server side processing


Developer Tools : 
- Visual studio 2015, IIS Express and IIS 10.0
- SQL Server 2014
- SQL Server 2014 Management Studio 2014
- NET Framework 4.6

Feature :
- Import Big Data from CSV File
- Filtering with server side processing
- Printing with server side processing 
- Export Datatable to CSV File with server side processing
- SBAdmin2 themes

How To use :
- Create database with name "mvcapp" on your SQL Server
- Run query "CreateTable.sql", this will create 1 table
- Update connection string in "..\MvcApplication\web.config" following your sql server configuration
- Open file "MvcApplication.sln" with Visual Studio
- Run App to IIS Express or publish it to IIS Server

Trouble Shooting :
- After publish, update IIS_IUSRS permission to full control on folder "..\App_Data\uploads"
- After publish the web application, if there are HTTP Error 500.19 and error code : 0x80070021 when accessing the website, please register NET 4.5/4.6 from the control panel: 
  Programs and Features > Turn Windows features on or off > Information Information Services > World Wide Web Services > Application Development Features > Select ASP.NET 4.5/4.6
  ( reference link : http://goo.gl/1J10wH )


