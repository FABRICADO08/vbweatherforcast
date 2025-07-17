Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports System.Windows.Forms
Imports System.Drawing

Public Class WeatherForm
    Private Shared ReadOnly client As New HttpClient()
    Private Const API_KEY As String = "YOUR_API_KEY_HERE" ' Get from openweathermap.org
    Private Const BASE_URL As String = "https://api.openweathermap.org/data/2.5/"

    ' Controls
    Private WithEvents btnGetWeather As Button
    Private WithEvents txtCity As TextBox
    Private lblCity As Label
    Private lblCurrentTemp As Label
    Private lblDescription As Label
    Private lblFeelsLike As Label
    Private lblHumidity As Label
    Private lblWindSpeed As Label
    Private lblPressure As Label
    Private lvForecast As ListView
    Private lblStatus As Label

    Public Sub New()
        InitializeForm()
    End Sub

    Private Sub InitializeForm()
        ' Form properties
        Me.Text = "Weather Forecast"
        Me.Size = New Size(600, 500)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False

        ' City input
        lblCity = New Label()
        lblCity.Text = "Enter City:"
        lblCity.Location = New Point(20, 20)
        lblCity.Size = New Size(100, 20)
        Me.Controls.Add(lblCity)

        txtCity = New TextBox()
        txtCity.Location = New Point(120, 18)
        txtCity.Size = New Size(200, 25)
        txtCity.Text = "London"
        Me.Controls.Add(txtCity)

        btnGetWeather = New Button()
        btnGetWeather.Text = "Get Weather"
        btnGetWeather.Location = New Point(330, 17)
        btnGetWeather.Size = New Size(100, 27)
        Me.Controls.Add(btnGetWeather)

        ' Current weather display
        lblCurrentTemp = New Label()
        lblCurrentTemp.Font = New Font("Arial", 16, FontStyle.Bold)
        lblCurrentTemp.Location = New Point(20, 60)
        lblCurrentTemp.Size = New Size(200, 30)
        lblCurrentTemp.Text = "Temperature: --°C"
        Me.Controls.Add(lblCurrentTemp)

        lblDescription = New Label()
        lblDescription.Font = New Font("Arial", 12)
        lblDescription.Location = New Point(20, 100)
        lblDescription.Size = New Size(300, 25)
        lblDescription.Text = "Description: --"
        Me.Controls.Add(lblDescription)

        lblFeelsLike = New Label()
        lblFeelsLike.Location = New Point(20, 130)
        lblFeelsLike.Size = New Size(150, 20)
        lblFeelsLike.Text = "Feels like: --°C"
        Me.Controls.Add(lblFeelsLike)

        lblHumidity = New Label()
        lblHumidity.Location = New Point(180, 130)
        lblHumidity.Size = New Size(120, 20)
        lblHumidity.Text = "Humidity: --%"
        Me.Controls.Add(lblHumidity)

        lblWindSpeed = New Label()
        lblWindSpeed.Location = New Point(20, 155)
        lblWindSpeed.Size = New Size(150, 20)
        lblWindSpeed.Text = "Wind: -- m/s"
        Me.Controls.Add(lblWindSpeed)

        lblPressure = New Label()
        lblPressure.Location = New Point(180, 155)
        lblPressure.Size = New Size(150, 20)
        lblPressure.Text = "Pressure: -- hPa"
        Me.Controls.Add(lblPressure)

        ' 5-day forecast ListView
        Dim lblForecast As New Label()
        lblForecast.Text = "5-Day Forecast:"
        lblForecast.Font = New Font("Arial", 12, FontStyle.Bold)
        lblForecast.Location = New Point(20, 190)
        lblForecast.Size = New Size(200, 25)
        Me.Controls.Add(lblForecast)

        lvForecast = New ListView()
        lvForecast.View = View.Details
        lvForecast.FullRowSelect = True
        lvForecast.GridLines = True
        lvForecast.Location = New Point(20, 220)
        lvForecast.Size = New Size(540, 180)
        
        ' Add columns
        lvForecast.Columns.Add("Date", 100)
        lvForecast.Columns.Add("Temperature", 80)
        lvForecast.Columns.Add("Description", 150)
        lvForecast.Columns.Add("Humidity", 80)
        lvForecast.Columns.Add("Wind Speed", 80)
        Me.Controls.Add(lvForecast)

        ' Status label
        lblStatus = New Label()
        lblStatus.Location = New Point(20, 410)
        lblStatus.Size = New Size(540, 20)
        lblStatus.Text = "Ready"
        lblStatus.ForeColor = Color.Blue
        Me.Controls.Add(lblStatus)
    End Sub

    Private Async Sub btnGetWeather_Click(sender As Object, e As EventArgs) Handles btnGetWeather.Click
        If String.IsNullOrWhiteSpace(txtCity.Text) Then
            MessageBox.Show("Please enter a city name.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        btnGetWeather.Enabled = False
        lblStatus.Text = "Fetching weather data..."
        lblStatus.ForeColor = Color.Blue

        Try
            ' Get current weather
            Await GetCurrentWeather(txtCity.Text)
            
            ' Get 5-day forecast
            Await GetForecast(txtCity.Text)
            
            lblStatus.Text = "Weather data updated successfully"
            lblStatus.ForeColor = Color.Green
            
        Catch ex As Exception
            lblStatus.Text = $"Error: {ex.Message}"
            lblStatus.ForeColor = Color.Red
            MessageBox.Show($"Error fetching weather data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            btnGetWeather.Enabled = True
        End Try
    End Sub

    Private Async Function GetCurrentWeather(city As String) As Task
        Dim url As String = $"{BASE_URL}weather?q={city}&appid={API_KEY}&units=metric"
        Dim response As HttpResponseMessage = Await client.GetAsync(url)
        
        If response.IsSuccessStatusCode Then
            Dim json As String = Await response.Content.ReadAsStringAsync()
            Dim weather As WeatherData = JsonConvert.DeserializeObject(Of WeatherData)(json)
            
            ' Update current weather display
            lblCurrentTemp.Text = $"Temperature: {Math.Round(weather.main.temp)}°C"
            lblDescription.Text = $"Description: {weather.weather(0).description}"
            lblFeelsLike.Text = $"Feels like: {Math.Round(weather.main.feels_like)}°C"
            lblHumidity.Text = $"Humidity: {weather.main.humidity}%"
            lblWindSpeed.Text = $"Wind: {weather.wind.speed} m/s"
            lblPressure.Text = $"Pressure: {weather.main.pressure} hPa"
        Else
            Throw New Exception($"Failed to get current weather: {response.StatusCode}")
        End If
    End Function

    Private Async Function GetForecast(city As String) As Task
        Dim url As String = $"{BASE_URL}forecast?q={city}&appid={API_KEY}&units=metric"
        Dim response As HttpResponseMessage = Await client.GetAsync(url)
        
        If response.IsSuccessStatusCode Then
            Dim json As String = Await response.Content.ReadAsStringAsync()
            Dim forecast As ForecastData = JsonConvert.DeserializeObject(Of ForecastData)(json)
            
            ' Clear previous forecast
            lvForecast.Items.Clear()
            
            ' Get daily forecasts (every 8th item = 24 hours apart)
            For i As Integer = 0 To forecast.list.Count - 1 Step 8
                If i < forecast.list.Count Then
                    Dim item As ForecastItem = forecast.list(i)
                    Dim forecastDate As DateTime = DateTimeOffset.FromUnixTimeSeconds(item.dt).DateTime
                    
                    Dim listItem As New ListViewItem(forecastDate.ToString("ddd, MMM dd"))
                    listItem.SubItems.Add($"{Math.Round(item.main.temp)}°C")
                    listItem.SubItems.Add(item.weather(0).description)
                    listItem.SubItems.Add($"{item.main.humidity}%")
                    listItem.SubItems.Add($"{item.wind.speed} m/s")
                    
                    lvForecast.Items.Add(listItem)
                End If
            Next
        Else
            Throw New Exception($"Failed to get forecast: {response.StatusCode}")
        End If
    End Function

    Private Sub txtCity_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCity.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnGetWeather.PerformClick()
        End If
    End Sub
End Class

' Data classes for JSON deserialization
Public Class WeatherData
    Public Property main As MainWeatherData
    Public Property weather As WeatherDescription()
    Public Property wind As WindData
    Public Property name As String
End Class

Public Class MainWeatherData
    Public Property temp As Double
    Public Property feels_like As Double
    Public Property humidity As Integer
    Public Property pressure As Integer
End Class

Public Class WeatherDescription
    Public Property main As String
    Public Property description As String
End Class

Public Class WindData
    Public Property speed As Double
End Class

Public Class ForecastData
    Public Property list As ForecastItem()
End Class

Public Class ForecastItem
    Public Property dt As Long
    Public Property main As MainWeatherData
    Public Property weather As WeatherDescription()
    Public Property wind As WindData
End Class

' Main module to run the application
Module Program
    <STAThread>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New WeatherForm())
    End Sub
End Module
