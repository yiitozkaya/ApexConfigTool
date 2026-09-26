using System;
using System.Windows;
using System.Windows.Controls;

namespace ApexConfigTool.Views
{
    public partial class PerformanceView : UserControl
    {
        public event Action<string>?
            PresetRequested;


        public PerformanceView()
        {
            InitializeComponent();
        }


        private void CompetitiveButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectedPresetText.Text =
                "Competitive";


            PresetDescriptionText.Text =
                "Loads low-cost visual settings while keeping moderate texture filtering.";


            PresetRequested?.Invoke(
                "Competitive");
        }


        private void MaximumFpsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectedPresetText.Text =
                "Maximum FPS";


            PresetDescriptionText.Text =
                "Loads the most aggressive low graphics preset available in the tool.";


            PresetRequested?.Invoke(
                "Maximum FPS");
        }


        private void BalancedButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectedPresetText.Text =
                "Balanced";


            PresetDescriptionText.Text =
                "Loads a middle-ground graphics configuration.";


            PresetRequested?.Invoke(
                "Balanced");
        }


        private void QualityButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectedPresetText.Text =
                "Quality";


            PresetDescriptionText.Text =
                "Loads higher visual-quality settings.";


            PresetRequested?.Invoke(
                "Quality");
        }
    }
}