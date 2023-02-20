# HG-Webscraper
A simple tool for converting data from https://taprootcooperative.localorbit.com/products to a flat file.

As of version 1.2 automatic web scraping is not implemented. The process to use is to manually navigate to the site and then scroll down through entries to trigger the paging until all entries have been fetched by browser.  Then in the browser (Chrome in this example) open Developer Tools (F12 on Chrome), click the Elements Tab, right-click the topmost <html> tag, select Copy Element, then paste to a file. In the appsettings.json, set the "File" setting to the path to that file. Set the "OutputFile" setting to whereever you want the output to go.  Then run the exe.  Selected data from file should get parsed out into a semicolon delimited file.
