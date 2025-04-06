using CommunityToolkit.Maui.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace MyApp.View;

public partial class PropertySelectorPopup : Popup
{
    private readonly List<string> _availableProperties;
    private readonly Dictionary<string, CheckBox> _propertyCheckboxes = new();
    private readonly CSVServices _csvServices;

    public PropertySelectorPopup(CSVServices csvServices)
    {
        InitializeComponent();
        _csvServices = csvServices;

        // Obtenir toutes les propriétés du modèle Book
        _availableProperties = typeof(Book)
            .GetProperties()
            .Select(p => p.Name)
            .ToList();

        // Créer les CheckBox pour chaque propriété avec une présentation améliorée
        foreach (var property in _availableProperties)
        {
            var checkbox = new CheckBox
            {
                IsChecked = _csvServices.SelectedProperties.Contains(property),
                Color = Application.Current!.RequestedTheme == AppTheme.Dark
                    ? Colors.White
                    : Color.FromArgb("#512BD4") // Primary color
            };

            var propertyLabel = new Label
            {
                Text = property,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 16
            };

            var layout = new Frame
            {
                BorderColor = Colors.LightGray,
                BackgroundColor = Colors.White,
                CornerRadius = 5,
                Padding = new Thickness(10),
                Content = new HorizontalStackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        checkbox,
                        propertyLabel
                    }
                }
            };

            _propertyCheckboxes[property] = checkbox;
            PropertyCheckboxContainer.Children.Add(layout);
        }
    }

    private void OnConfirmClicked(object sender, EventArgs e)
    {
        _csvServices.SelectedProperties.Clear();

        // Ajouter les propriétés sélectionnées
        foreach (var property in _availableProperties)
        {
            if (_propertyCheckboxes[property].IsChecked)
            {
                _csvServices.SelectedProperties.Add(property);
            }
        }

        // Afficher un message de confirmation
        if (_csvServices.SelectedProperties.Count > 0)
        {
            Shell.Current.DisplayAlert("Succès", $"{_csvServices.SelectedProperties.Count} champs sélectionnés pour l'export", "OK");
        }
        else
        {
            Shell.Current.DisplayAlert("Attention", "Aucun champ sélectionné. L'export contiendra tous les champs par défaut.", "OK");
            // Si rien n'est sélectionné, ajouter tous les champs par défaut
            _csvServices.SelectedProperties.AddRange(_availableProperties);
        }

        Close();
    }

    private void OnSelectAllClicked(object sender, EventArgs e)
    {
        foreach (var checkbox in _propertyCheckboxes.Values)
        {
            checkbox.IsChecked = true;
        }
    }

    private void OnDeselectAllClicked(object sender, EventArgs e)
    {
        foreach (var checkbox in _propertyCheckboxes.Values)
        {
            checkbox.IsChecked = false;
        }
    }
}