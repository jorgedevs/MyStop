using System.Collections.Generic;
using Xamarin.Forms;

namespace MyStop.Views
{
    public partial class WhatsNewView : ContentView
    {
        int currentIndex;
        List<string> listImages;

        public WhatsNewView()
        {
            InitializeComponent();

            currentIndex = 0;

            listImages = new List<string>();
            listImages.Add("img_new_1");
            listImages.Add("img_new_2");
            listImages.Add("img_new_3");

            btnNext.Clicked += BtnNextClicked;
        }

        private void BtnNextClicked(object sender, System.EventArgs e)
        {
            if (currentIndex + 1 < listImages.Count)
            {
                currentIndex++;
                imgWhatsNew.Source = ImageSource.FromFile(listImages[currentIndex]);
                lblNext.Text = currentIndex + 1 < listImages.Count ? "Next" : "Got it!";
            }
            else
            {
                MessagingCenter.Send(this, "CLOSE_VIEW");
            }
        }
    }
}