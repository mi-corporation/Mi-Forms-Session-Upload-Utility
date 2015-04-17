# Mi-Forms-Session-Upload-Utility
# Created 4/17/2015 By Kevin Burgess

This utility is designed to help automate the process of form creation. 
You can use it to auto-populate a form from a SQL query or by reading 
the name of a file (such as a pdf) exported by another program.

This is a console application.
To set it up follow this process
1) Determine how you want to collect your data. 
  - Query SQL with a stored procedure
  - Query SQL using SELECT - FROM - WHERE statement
  - Check a folder for a file of a specific type (e.g. PDF)
2) Change all of your settings in the App.Config file.
  - Configure Mi-Forms server settings
  - Configure SQL server settings
  - Configure folder and file properties
  - Configure form template properties
3) Modify Map.cs to map data collected from SQL or file names to form 
fields on a form template. There are example fields showing this.

Please feel free to use this code to help you build the perfect solution 
for your business needs.
