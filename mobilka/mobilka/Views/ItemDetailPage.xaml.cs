using mobilka.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace mobilka.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}