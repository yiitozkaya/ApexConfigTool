using System.Windows.Controls;

namespace ApexConfigTool.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }


        public void UpdateData(
            string configPath,
            int width,
            int height,
            bool readOnly,
            bool apexRunning,
            string launcher,
            string fov,
            string fps)
        {
            ConfigPathText.Text =
                configPath;


            ResolutionText.Text =
                $"{width} × {height}";


            ReadOnlyText.Text =
                readOnly
                    ? "Protected"
                    : "Writable";


            ApexStateText.Text =
                apexRunning
                    ? "Running"
                    : "Off";


            LauncherText.Text =
                string.IsNullOrWhiteSpace(
                    launcher)
                    ? "Unknown"
                    : launcher;


            FovText.Text =
                string.IsNullOrWhiteSpace(
                    fov)
                    ? "Default"
                    : fov;


            FpsText.Text =
                string.IsNullOrWhiteSpace(
                    fps)
                    ? "Default"
                    : fps;
        }
    }
}