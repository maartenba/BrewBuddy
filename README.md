BrewBuddy
=========

Sample code for my talk "Brewing Beer with Windows Azure" - http://channel9.msdn.com/Events/aspConf/aspConf/Brewing-Beer-with-Windows-Azure

## Running the code
- Make sure you have a service bus namespace running
- Enter the details of it in BrewBuddy.Services.BrewService
- In BrewBuddy.Agent.Sensor.ServiceBusSensorApiV1, change the service bus namespace
- In BrewBuddy.Worker.Logic.TemperatureTap, change the service bus namespace details 
- In Web.config, edit the service bus ACS namespace details if you want to make use of the API
- In Web.config, edit the database conection string

Or just have a look at www.brewbuddy.net.