# SystemMonitor
Simple C# .Net application for monitoring system parameters like CPU usage, RAM usage, Ethernet and Wireless parameters, etc. Program have funcionality to change between languages and easly apply a new language. Program is keeping it's parameters in .xml file like position on screen, choosen language etc. Also program will be storing some data into DB and user will have possibility to view through this data with simple GUI.
# Applied funcionalities
- displaying actual CPU usage in % and graph for last 20 seconds, processor name and number of cores
- displaying graph for actual RAM usage in %, displaying total RAM in system and total RAM in use
- for Wireless network interface displaying connected SSID, assigned IP address and method to obtain IP address (manually choosen IP or automatically)
- for Ethernet network interface displaying is the network connected or not, IP address and method to obtain IP address (manually choosen IP or automatically)
- displaying time of current session
- tray icon funcionality: left click bringing app to front, right click displaying tray icon menu with two option to choose: bring to front and close
- changing of language for whole app, choosen language is saved to Parameters.xml and after restart of app program is starting with choosen language, created languages: EN, PL
- dragging and droping application by mouse, position is saved to Parameters.xml and after restart of app program is appearing at the saved position
