using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenXamarinD3.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TenXamarinD6
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExperiencesView : ContentPage
    {
        List<Experience> experiencesItems = new List<Experience>();

        public ExperiencesView()
        {
            InitializeComponent();
            BindingContext = experiencesItems;
        }


        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MainPage());

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ReadExperiences();
        }

        private void ReadExperiences()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
            {
                conn.CreateTable<Experience>();
                List<Experience> experiences = conn.Table<Experience>().ToList();
                experiencesListView.ItemsSource = experiences;
            }
        }
    }
}