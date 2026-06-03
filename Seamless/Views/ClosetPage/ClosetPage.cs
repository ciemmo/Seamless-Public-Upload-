using Seamless.Models;

namespace Seamless
{
    public partial class ClosetPage : ContentPage
    {
        public ClosetPage()
        {
            InitializeComponent();
            this.BindingContext = new ClosetViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ClosetViewModel vm)
            {
                
            }
        }

        // --- BUTTONS ---
        private void OnViewItemsClicked(object sender, EventArgs e)
        {
            if (BindingContext is ClosetViewModel vm) vm.RefreshClosetCommand.Execute(null);
        }

        private void OnViewOutfitsClicked(object sender, EventArgs e)
        {
            if (BindingContext is ClosetViewModel vm) vm.LoadOutfitsCommand.Execute(null);
        }

        private async void OnCreateOutfitClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddOrEditOutfitPage());
        }

        private async void OnAddItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TaggingPage());
        }

        private async void OnFilterClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Filter By Category", "Cancel", "Show All", "T-Shirt", "Pants", "Shoes", "Jacket", "Dress");
            if (action != "Cancel" && BindingContext is ClosetViewModel vm) vm.FilterItems(action);
        }

        private void OnSortChanged(object sender, EventArgs e)
        {
            if (SortPicker.SelectedIndex != -1 && BindingContext is ClosetViewModel vm)
            {
                string selectedSort = SortPicker.Items[SortPicker.SelectedIndex];
                vm.SortItems(selectedSort);
            }
        }

        private async void OnViewTrashClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Trash", "Trash is empty.", "OK");
        }

        // --- NAVIGATION LOGIC ---
        private async void OnItemTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is DisplayItem tappedItem)
            {
                if (tappedItem.IsOutfit)
                {
                    // It's an outfit -> Go to the new page we just made
                    await Navigation.PushAsync(new OutfitDetailPage(tappedItem));
                }
                else
                {
                    // It's a regular item -> Go to the old details page
                    await Navigation.PushAsync(new ItemDetailPage(tappedItem));
                }
            }
        }
    }
}