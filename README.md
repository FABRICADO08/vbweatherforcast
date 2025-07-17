# vbweatherforcast
This Visual Basic .NET application creates a weather forecast display with the following features:
Key Features:

Current Weather Display: Shows temperature, description, feels-like temperature, humidity, wind speed, and pressure
5-Day Forecast: ListView showing daily forecasts with key weather information
City Search: Text input to search for weather in any city
Clean UI: Windows Forms interface with organized layout

# Setup Instructions:

Create a new VB.NET Windows Forms project in Visual Studio
Install Newtonsoft.Json NuGet package:

Right-click project → Manage NuGet Packages
Search for "Newtonsoft.Json" and install it


# Get a free API key:

Sign up at OpenWeatherMap
Replace "YOUR_API_KEY_HERE" with your actual API key


Replace the default form code with the code above

# How to Use:

Enter a city name in the text box
Click "Get Weather" or press Enter
View current weather conditions and 5-day forecast
Status messages show loading progress and errors

# Features Included:

Error handling for network issues and invalid cities
Async/await for non-blocking UI during API calls
Temperature conversion to Celsius
Formatted date display in the forecast
Keyboard shortcuts (Enter to search)

The application uses the OpenWeatherMap API which provides reliable weather data. Make sure you have an internet connection and valid API key for it to work properly.
