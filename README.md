# aspnetmvc5-serversidepaging

This is an example application using ASP.NET MVC 5 with datatable server side processing


Developer Tools : 
- Visual studio 2015 and IIS Express/ IIS ver 10
- SQL Server 2014
- SQL Server 2014 Management Studio 2014
- NET Framework 4.6

Feature :
- Import Big Data from CSV File
- Filtering with server side processing
- Printing with server side processing 
- Export Datatable to CSV File with server side processing


How To use :
- Create database name "mvcapp" on your SQL Server
- Run query "CreateTable.sql", this will create 1 table
- Update connection string in "../MvcApplication/web.config" following your sql server configuration
- Open file "MvcApplication.sln" with Visual Studio
- Run it or Publish it

Note :
- Make sure you give permision to folder "..\App_Data\uploads"
- After publish the web application, if there are error code 0x80070021 when access the website,   
  please register 4.5/4.6 from the control panel:
  Programs and Features > Turn Windows features on or off > Information Information Services > World Wide Web Services > Application Development Features > Select ASP.NET 4.5/4.6
