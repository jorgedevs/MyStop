using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using MyStop.iOS.Renderers;

[assembly: ExportRenderer(typeof(Entry), typeof(StylessEntryRenderer))]
namespace MyStop.iOS.Renderers
{
    public class StylessEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            Control.Layer.BorderWidth = 0;
            Control.BorderStyle = UITextBorderStyle.None;
        }
    }
}