using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace MyApp.Service;

public partial class BarcodeScannerService
{
    private SerialPort? _serialPort;
    private string? _portDetected = null;
    private IDispatcherTimer? _emulatorTimer;
    private readonly string[] _sampleISBNs = new[]
    {
        "9780061120084", // To Kill a Mockingbird
        "9780141187761", // 1984
        "9782070368228", // L'Étranger
        "9780307474278", // Le Petit Prince
        "9780618640157"  // Le Seigneur des Anneaux
    };
    private int _currentEmulatorIndex = 0;

    public partial void OpenPort()
    {
        // Fermer le port s'il est déjà ouvert
        if (_serialPort != null)
        {
            try
            {
                if (_serialPort.IsOpen) _serialPort.Close();
                _serialPort.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la fermeture du port: {ex.Message}");
            }
            finally
            {
                _serialPort = null;
            }
        }

        // Détecter le scanner M900D sur un port COM
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'");

        foreach (ManagementObject queryObj in searcher.Get())
        {
            string id = queryObj["PNPDeviceID"]?.ToString() ?? "";
            string nom = queryObj["Name"]?.ToString() ?? "";

            // ID PID_A4A7 pour le scanner M900D (à ajuster selon votre matériel réel)
            if (id.Contains("PID_A4A7") || nom.ToLower().Contains("barcode") || nom.ToLower().Contains("scanner"))
            {
                int debut = nom.LastIndexOf("COM");
                int fin = nom.LastIndexOf(")");

                if (debut != -1 && fin != -1)
                {
                    _portDetected = nom.Substring(debut, fin - debut);
                    break;
                }
            }
        }

        // Si un port a été détecté, l'ouvrir
        if (_portDetected != null)
        {
            _serialPort = new SerialPort
            {
                BaudRate = 9600,
                PortName = _portDetected,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadTimeout = 10000,
                WriteTimeout = 10000
            };

            _serialPort.DataReceived += SerialDataReceivedHandler;

            try
            {
                _serialPort.Open();
                Shell.Current.DisplayAlert("Scanner connecté", $"Port {_portDetected} ouvert avec succès", "OK");
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("Erreur!", $"Impossible d'ouvrir le port: {ex.Message}", "OK");
            }
        }
        else
        {
            Shell.Current.DisplayAlert("Aucun scanner détecté", "Aucun scanner de code-barres n'a été détecté. L'émulateur sera utilisé.", "OK");
        }
    }

    public partial void ClosePort()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            try
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la fermeture du port: {ex.Message}");
            }
            finally
            {
                _serialPort = null;
            }
        }

        // Arrêter l'émulateur s'il est en cours
        StopEmulator();
    }

    private void SerialDataReceivedHandler(object sender, SerialDataReceivedEventArgs args)
    {
        SerialPort sp = (SerialPort)sender;
        string data = sp.ReadExisting();
        ScanBuffer.Enqueue(data);
    }

    public void StartEmulator(TimeSpan interval)
    {
        StopEmulator();

        _emulatorTimer = Application.Current?.Dispatcher.CreateTimer();
        if (_emulatorTimer != null)
        {
            _emulatorTimer.Interval = interval;
            _emulatorTimer.Tick += EmulatorTimerTick;
            _emulatorTimer.Start();
        }
    }

    public void StopEmulator()
    {
        if (_emulatorTimer != null)
        {
            _emulatorTimer.Stop();
            _emulatorTimer.Tick -= EmulatorTimerTick;
            _emulatorTimer = null;
        }
    }

    private void EmulatorTimerTick(object? sender, EventArgs e)
    {
        // Simuler la réception d'un code-barres
        string isbn = _sampleISBNs[_currentEmulatorIndex] + "\r\n";
        ScanBuffer.Enqueue(isbn);

        // Passer au code-barres suivant
        _currentEmulatorIndex = (_currentEmulatorIndex + 1) % _sampleISBNs.Length;
    }
}