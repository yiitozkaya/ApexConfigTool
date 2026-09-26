using System.Windows;
using System.Windows.Controls;

using ApexConfigTool.Models;

namespace ApexConfigTool.Views
{
    public partial class DisplayView : UserControl
    {
        public DisplayView()
        {
            InitializeComponent();
        }


        private void ResolutionCombo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (CustomResolutionPanel == null)
                return;


            ComboBoxItem? selected =
                ResolutionCombo.SelectedItem
                as ComboBoxItem;


            string value =
                selected?.Content?.ToString()
                ?? "";


            CustomResolutionPanel.Visibility =
                value == "Custom resolution"
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }


        public void LoadSettings(
            int width,
            int height,
            bool fullscreen,
            bool readOnly)
        {
            string current =
                $"{width} × {height}";


            bool found =
                false;


            foreach (ComboBoxItem item
                     in ResolutionCombo.Items)
            {
                string value =
                    item.Content?.ToString()
                    ?? "";


                if (value.StartsWith(
                    current))
                {
                    ResolutionCombo.SelectedItem =
                        item;

                    found =
                        true;

                    break;
                }
            }


            if (!found)
            {
                foreach (ComboBoxItem item
                         in ResolutionCombo.Items)
                {
                    if (item.Content?.ToString()
                        == "Custom resolution")
                    {
                        ResolutionCombo.SelectedItem =
                            item;

                        break;
                    }
                }


                CustomWidthBox.Text =
                    width.ToString();


                CustomHeightBox.Text =
                    height.ToString();
            }


            FullscreenCheck.IsChecked =
                fullscreen;


            ReadOnlyCheck.IsChecked =
                readOnly;
        }


        public DisplaySettings? GetSettings()
        {
            ComboBoxItem? selected =
                ResolutionCombo.SelectedItem
                as ComboBoxItem;


            if (selected == null)
                return null;


            string selection =
                selected.Content?.ToString()
                ?? "";


            int width;
            int height;


            if (selection ==
                "Custom resolution")
            {
                if (!int.TryParse(
                        CustomWidthBox.Text,
                        out width))
                {
                    return null;
                }


                if (!int.TryParse(
                        CustomHeightBox.Text,
                        out height))
                {
                    return null;
                }
            }
            else
            {
                string cleaned =
                    selection
                        .Replace(
                            " ",
                            "");


                string resolution =
                    cleaned.Split('·')[0];


                string[] pieces =
                    resolution.Split('×');


                if (pieces.Length != 2)
                    return null;


                if (!int.TryParse(
                        pieces[0],
                        out width))
                {
                    return null;
                }


                if (!int.TryParse(
                        pieces[1],
                        out height))
                {
                    return null;
                }
            }


            if (width < 640 ||
                height < 480)
            {
                return null;
            }


            return new DisplaySettings
            {
                Width =
                    width,

                Height =
                    height,

                Fullscreen =
                    FullscreenCheck.IsChecked
                    == true,

                ReadOnly =
                    ReadOnlyCheck.IsChecked
                    == true
            };
        }
    }
}