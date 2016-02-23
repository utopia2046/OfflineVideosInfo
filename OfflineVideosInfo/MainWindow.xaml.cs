using System.Configuration;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace OfflineVideosInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var userPath = Properties.Settings.Default.Path;
            var xmlFile = Properties.Settings.Default.OutputXml;
            var htmlFile = Properties.Settings.Default.OutputHtml;

            var videoInfoList = VideoInfo.TryParseAll(userPath);
            lsvVideoInfo.ItemsSource = videoInfoList;

            VideoInfo.WriteToXml(videoInfoList, xmlFile);
            VideoInfo.WriteUnfinishedVideoToHtml(videoInfoList, htmlFile);
        }

        private void lsvVideoInfoColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lsvVideoInfo.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lsvVideoInfo.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));

        }
    }
}
