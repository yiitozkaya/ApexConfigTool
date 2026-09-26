using System.Collections.Generic;
using System.Linq;
using System.Windows;

using ApexConfigTool.Models;

namespace ApexConfigTool
{
    public partial class PreviewWindow : Window
    {
        public bool ApplyChanges { get; private set; }

        public bool LaunchAfterApply { get; private set; }


        public PreviewWindow(
            List<ConfigChange> changes)
        {
            InitializeComponent();


            ChangesGrid.ItemsSource =
                changes;


            int video =
                changes.Count(
                    x => x.Source == "Video");


            int autoexec =
                changes.Count(
                    x => x.Source == "Autoexec");


            SummaryText.Text =
                $"{changes.Count} changes · " +
                $"{video} video · " +
                $"{autoexec} autoexec";
        }


        private void CancelButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ApplyChanges =
                false;


            DialogResult =
                false;
        }


        private void ApplyButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ApplyChanges =
                true;


            LaunchAfterApply =
                false;


            DialogResult =
                true;
        }


        private void ApplyLaunchButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ApplyChanges =
                true;


            LaunchAfterApply =
                true;


            DialogResult =
                true;
        }
    }
}