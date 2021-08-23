using EpicorRESTGenerator.Shared.Models;
using EpicorRESTGenerator.Shared.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace EpicorRESTGenerator
{
    /// <summary>
    /// Interaction logic for generator.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeneratorService generatorService;

        public MainWindow()
        {
            InitializeComponent();

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ERPProject))
            {
                var username = Environment.UserName;
                Properties.Settings.Default.ERPProject =
                    $@"C:\Users\{username}\Documents\GitHub\Epicor.Models\Epicor.Models\Epicor.Models\Epicor.Models.csproj";
            }
        }

        private void CheckService_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(serviceURLTextBox.Text))
            {
                MessageBox.Show("Epicor API URL is Required");
                return;
            }

            if ((bool)UseCredentialsCheckBox.IsChecked)
            {
                if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
                {
                    MessageBox.Show("Username is required");
                    return;
                }

                if (string.IsNullOrWhiteSpace(passwordTextBox.Password))
                {
                    MessageBox.Show("Password is required");
                    return;
                }
            }

            IsServiceValid(serviceURLTextBox.Text + "/api/v1/");

            MessageBoxResult result = MessageBox.Show("Do you want to generate a client for oData? " +
                "Selecting No will default to custom methods", "", MessageBoxButton.YesNo);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    ERPAPIURLTextBox.Text = serviceURLTextBox.Text + "/api/swagger/v1/odata/";
                    ICEAPIURLTextBox.Text = serviceURLTextBox.Text + "/api/swagger/v1/odata/";
                    break;
                case MessageBoxResult.No:
                    ERPAPIURLTextBox.Text = serviceURLTextBox.Text + "/api/swagger/v1/methods/";
                    ICEAPIURLTextBox.Text = serviceURLTextBox.Text + "/api/swagger/v1/methods/";
                    break;
            }

            BAQAPIURLTextBox.Text = serviceURLTextBox.Text + "/api/swagger/v1/baq/";
            ERPAPIURLServiceTextBox.Text = serviceURLTextBox.Text + "/api/v1/";
            ICEAPIURLServiceTextBox.Text = serviceURLTextBox.Text + "/api/v1/";
            BAQAPIURLServiceTextBox.Text = serviceURLTextBox.Text + "/api/v1/BaqSvc/";

            tabControl.IsEnabled = true;
        }

        private bool FileExists(TextBox textBox)
        {
            return File.Exists(textBox.Text);
        }

        private void GetBAQServicesButton_Click(object sender, RoutedEventArgs e)
        {
            PopulateService(BAQAPIURLServiceTextBox, BAQServiceListBox, "");
        }

        private void GetERPServicesButton_Click(object sender, RoutedEventArgs e)
        {
            PopulateService(ERPAPIURLServiceTextBox, ERPServiceListBox, "ERP");
        }

        private void GetICEServicesButton_Click(object sender, RoutedEventArgs e)
        {
            PopulateService(ICEAPIURLServiceTextBox, ICEServiceListBox, "ICE");
        }

        private async Task<bool> Generate(string url, string proj)
        {
            GeneratorOptions details = new GeneratorOptions();
            details.BaseClass = BaseClassTextBox.Text;
            details.APIURL = url;
            details.Project = proj;
            details.Namespace = NamespaceTextBox.Text;
            details.UseNamespace = (bool)UseNamespaceCheckBox.IsChecked;
            details.UseBaseClass = (bool)UseBaseClassCheckBox.IsChecked;
            details.Username = UsernameTextBox.Text;
            details.Password = passwordTextBox.Password;

            var test = await GeneratorService.GenerateCode(generatorService, details);
            if (test)
                MessageBox.Show("Success");
            else
                MessageBox.Show("Somehing went wrong");
            return true;
        }

        private void GenerateERPButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValueSet(ERPAPIURLServiceTextBox) || !IsServiceValid(ERPAPIURLServiceTextBox.Text))
            {
                MessageBox.Show("Please provide a services URL for the ERP Services", ""); return;
            }

            if (!IsValueSet(ERPProjectTextBox) || !FileExists(ERPProjectTextBox))
            {
                MessageBox.Show("Please provide the ERP project directory", ""); return;
            }

            if (!IsValueSet(ERPAPIURLTextBox))
            {
                MessageBox.Show("Please provide the ERP API URL", ""); return;
            }

            if (ERPServiceListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select the service you wish to generate a client for!", ""); return;
            }

            IsEnabled = false;
            generatorService.Workspace.Collection = generatorService.Workspace.Collection
                .Where(o => ERPServiceListBox.SelectedItems.Contains(o.Href)).ToArray();
            var r = Generate(ERPAPIURLTextBox.Text, ERPProjectTextBox.Text).Result;
            IsEnabled = true;
        }

        private void GenerateBAQButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValueSet(BAQAPIURLServiceTextBox) || !IsServiceValid(BAQAPIURLServiceTextBox.Text))
            {
                MessageBox.Show("Please provide a services URL for the BAQ Services", ""); return;
            }

            if (!IsValueSet(BAQProjectTextBox) || !FileExists(BAQProjectTextBox))
            {
                MessageBox.Show("Please provide the BAQ project directory", ""); return;
            }

            if (!IsValueSet(BAQAPIURLTextBox))
            {
                MessageBox.Show("Please provide the BAQ API URL", ""); return;
            }

            if (BAQServiceListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select the service you wish to generate a client for!", ""); return;
            }

            IsEnabled = false;
            generatorService.Workspace.Collection = generatorService.Workspace.Collection
                .Where(o => BAQServiceListBox.SelectedItems.Contains(o.Href)).ToArray();
            var r = Generate(BAQAPIURLTextBox.Text, BAQProjectTextBox.Text).Result;
            IsEnabled = true;
        }

        private void GenerateICEButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValueSet(ICEAPIURLServiceTextBox) || !IsServiceValid(ICEAPIURLServiceTextBox.Text))
            {
                MessageBox.Show("Please provide a services URL for the ICE Services", ""); return;
            }

            if (!IsValueSet(ICEProjectTextBox) || !FileExists(ICEProjectTextBox))
            {
                MessageBox.Show("Please provide the ICE project directory", ""); return;
            }

            if (!IsValueSet(ICEAPIURLTextBox))
            {
                MessageBox.Show("Please provide the ICE API URL", ""); return;
            }

            if (ICEServiceListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select the service you wish to generate a client for!", ""); return;
            }

            IsEnabled = false;
            generatorService.Workspace.Collection = generatorService.Workspace.Collection
                .Where(o => ICEServiceListBox.SelectedItems.Contains(o.Href)).ToArray();
            var r = Generate(ICEAPIURLTextBox.Text, ICEProjectTextBox.Text).Result;
            IsEnabled = true;
        }

        private bool IsValueSet(TextBox textBox)
        {
            return !string.IsNullOrWhiteSpace(textBox.Text);
        }

        private bool IsServiceValid(string url)
        {
            GeneratorOptions details = new GeneratorOptions();
            if ((bool)UseCredentialsCheckBox.IsChecked)
            {
                details.Username = UsernameTextBox.Text;
                details.Password = passwordTextBox.Password;
            }
            try
            {
                var valid = GeneratorService.GetEpicorServices(url, details);
                if (valid.Workspace != null && valid.Workspace.Collection != null && valid.Workspace.Collection.Count() == 0)
                {
                    MessageBox.Show("Service is invalid");
                    return false;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (WebResponse response = ex.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        MessageBox.Show(string.Format("Error code: {0}", httpResponse.StatusDescription));
                        string text = "";
                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            // text is the response body
                            text = reader.ReadToEnd();
                        }
                        MessageBox.Show(text);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Document = new FlowDocument();

            OpenFileDialog openFile1 = new OpenFileDialog();
            openFile1.DefaultExt = "*.cs";
            openFile1.Filter = "CS Files|*.cs";

            var hasValue = openFile1.ShowDialog().HasValue;
            if (hasValue)
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(System.IO.File.ReadAllText(openFile1.FileName));
                FlowDocument document = new FlowDocument(paragraph);
                richTextBox.Document = document;
            }
        }

        private void OnTabItemChanged(object sender, SelectionChangedEventArgs e)
        {
            generatorService = null;
            ERPServiceListBox.ItemsSource = null;
            ICEServiceListBox.ItemsSource = null;
            BAQServiceListBox.ItemsSource = null;
        }

        private void PopulateService(TextBox textBox, ListBox listBox, string type)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                MessageBox.Show("Services URL is Required");
            }
            if (!IsServiceValid(textBox.Text)) return;

            GeneratorOptions details = new GeneratorOptions();
            if ((bool)UseCredentialsCheckBox.IsChecked)
            {
                details.Username = UsernameTextBox.Text;
                details.Password = passwordTextBox.Password;
            }

            generatorService = GeneratorService.GetEpicorServices(textBox.Text, details);
            generatorService.Workspace.Collection = generatorService.Workspace.Collection
                .Where(o => o.Href.ToUpper().StartsWith(type)).ToArray<ServiceWorkspaceCollection>();
            listBox.ItemsSource = generatorService.Workspace.Collection.Select(o => o.Href);

            try
            {
                var files = Directory.EnumerateFiles(Path.GetDirectoryName(ERPProjectTextBox.Text), "*.cs");
                foreach (var file in files)
                {
                    var filename = System.IO.Path.GetFileNameWithoutExtension(file);
                    if (listBox.Items.Contains(filename))
                    {
                        listBox.SelectedItems.Add(listBox.Items[listBox.Items.IndexOf(filename)]);
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex;
                // TODO Log this exception instead of throwing it away
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "*.cs";
            dialog.Filter = "CS Files|*.cs";

            if (dialog.ShowDialog() == true)
            {
                TextRange t = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                FileStream file = new FileStream(dialog.FileName, FileMode.Create);
                t.Save(file, System.Windows.DataFormats.Text);
                file.Close();
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.EpicorUrl = serviceURLTextBox.Text;
            Properties.Settings.Default.Namespace = NamespaceTextBox.Text;
            Properties.Settings.Default.BaseClass = BaseClassTextBox.Text;
            Properties.Settings.Default.Username = UsernameTextBox.Text;
            Properties.Settings.Default.ERPProject = ERPProjectTextBox.Text;
            Properties.Settings.Default.UseCredentials = UseCredentialsCheckBox.IsChecked == true;
            Properties.Settings.Default.Save();
        }

        private void ServiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }
    }
}
