using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Service;

public partial class BarcodeScannerService
{
    // Buffer pour stocker les données reçues du scanner
    public QueueBuffer ScanBuffer = new();

    // Événement déclenché lorsqu'un code-barres complet est scanné
    public event EventHandler<BarcodeScannedEventArgs>? BarcodeScanned;

    private StringBuilder _currentBarcode = new();

    // Méthodes partielles implémentées différemment selon les plateformes
    public partial void OpenPort();
    public partial void ClosePort();

    // Classe pour gérer le buffer de données
    public sealed partial class QueueBuffer : Queue
    {
        public event EventHandler? Changed;

        public override void Enqueue(object? obj)
        {
            base.Enqueue(obj);
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    // Constructeur
    public BarcodeScannerService()
    {
        ScanBuffer.Changed += OnScanBufferChanged;
    }

    // Méthode appelée lorsque des données sont reçues dans le buffer
    private void OnScanBufferChanged(object? sender, EventArgs e)
    {
        if (sender is QueueBuffer buffer && buffer.Count > 0)
        {
            ProcessReceivedData(buffer.Dequeue()?.ToString() ?? string.Empty);
        }
    }

    // Traitement des données reçues du scanner
    private void ProcessReceivedData(string data)
    {
        foreach (char c in data)
        {
            // Si caractère de fin de ligne, considérer que le code-barres est complet
            if (c == '\r' || c == '\n')
            {
                if (_currentBarcode.Length > 0)
                {
                    // Déclencher l'événement avec le code-barres
                    string barcode = _currentBarcode.ToString();

                    // Vérifier si c'est un ISBN valide
                    if (IsValidISBN(barcode))
                    {
                        BarcodeScanned?.Invoke(this, new BarcodeScannedEventArgs(barcode));
                    }
                    else
                    {
                        // Notifier si le code n'est pas un ISBN valide
                        Shell.Current.DisplayAlert("Avertissement", $"Le code {barcode} ne semble pas être un ISBN valide", "OK");
                    }

                    _currentBarcode.Clear();
                }
            }
            else
            {
                // Ajouter le caractère au code-barres en cours
                _currentBarcode.Append(c);
            }
        }
    }

    // Méthode pour vérifier si un code est un ISBN valide
    private bool IsValidISBN(string isbn)
    {
        // Supprimer les tirets et espaces
        isbn = isbn.Replace("-", "").Replace(" ", "");

        // ISBN-13 (le plus courant aujourd'hui)
        if (isbn.Length == 13)
        {
            // Vérifier si tous les caractères sont des chiffres
            if (isbn.All(char.IsDigit))
            {
                // Vérifier si ça commence par 978 ou 979 (préfixes ISBN)
                return isbn.StartsWith("978") || isbn.StartsWith("979");
            }
        }

        // ISBN-10 (ancien format)
        else if (isbn.Length == 10)
        {
            // Vérifier si tous les caractères sont des chiffres (sauf le dernier qui peut être X)
            return isbn.Substring(0, 9).All(char.IsDigit) && (char.IsDigit(isbn[9]) || isbn[9] == 'X');
        }

        // Si aucune vérification n'est concluante, accepter quand même (ce pourrait être un autre format de code-barres)
        return true;
    }

    // Simuler le scan d'un code-barres (utile pour tester)
    public void SimulateScan(string isbn)
    {
        ScanBuffer.Enqueue(isbn + "\r\n");
    }
}

// Classe d'arguments pour l'événement de scan
public class BarcodeScannedEventArgs : EventArgs
{
    public string Barcode { get; }

    public BarcodeScannedEventArgs(string barcode)
    {
        Barcode = barcode;
    }
}