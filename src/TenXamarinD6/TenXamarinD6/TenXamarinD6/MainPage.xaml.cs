using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenXamarinD3.Model;
using Xamarin.Forms;

namespace TenXamarinD6
{
    public partial class MainPage : ContentPage
    {

        IGeolocator locator = CrossGeolocator.Current;
        Position position;

        public MainPage()
        {
            InitializeComponent();

            locator.PositionChanged += Locator_PositionChanged;
        }

        void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            position = e.Position; // this uses the global variable defined earlier
        }

        private void CheckIfShouldBeEnabled()
        {
            saveButton.IsEnabled = false;
            if (!string.IsNullOrWhiteSpace(titleEntry.Text) && !string.IsNullOrWhiteSpace(contentEditor.Text))
                saveButton.IsEnabled = true;
        }

        private void TitleEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckIfShouldBeEnabled();
        }

        private void ContentEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckIfShouldBeEnabled();
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {

            Experience newExperience = new Experience()
            {
                Title = titleEntry.Text,
                Content = contentEditor.Text,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var inserted = 0;

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
            {
                conn.CreateTable<Experience>();
                inserted = conn.Insert(newExperience);
            }

            if (inserted > 0)
            {
                titleEntry.Text = string.Empty;
                contentEditor.Text = string.Empty;
            }
            else
            {
                DisplayAlert("Error", "There was an error inserting the experience", "ok");
            }
            titleEntry.Text = string.Empty;
            contentEditor.Text = string.Empty;

        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GetLocationPermission();
        }

        private async void GetLocationPermission()
        {
            // added using Plugin.Permissions;
            // added using Plugin.Permissions.Abstractions;
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            if (status != PermissionStatus.Granted)
            {
                // Not granted, request permission
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.LocationWhenInUse))
                {
                    // This is not the actual permission request
                    await DisplayAlert("Need your permission", "We need to access your location", "Ok");
                }

                // This is the actual permission request
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.LocationWhenInUse);
                if (results.ContainsKey(Permission.LocationWhenInUse))
                    status = results[Permission.LocationWhenInUse];
            }



            // Already granted (maybe), go on
            if (status == PermissionStatus.Granted)
            {
                // Granted! Get the location
                GetLocation();
            }
            else
            {
                await DisplayAlert("Access to location denied", "We don't have access to your location", "Ok");
            }
        }

 

        private async void GetLocation()
        {
            var position = await locator.GetPositionAsync();
            await locator.StartListeningAsync(TimeSpan.FromMinutes(30), 500);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            locator.StopListeningAsync();
        }

    }

}
