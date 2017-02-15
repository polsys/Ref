using System;
using System.Windows;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref
{
    /// <summary>
    /// Interaction logic for CopyReferenceDialog.xaml
    /// </summary>
    public partial class CopyReferenceDialog : Window
    {
        CopyReferenceDialogViewModel _viewModel;

        internal CopyReferenceDialog(PublicationViewModelBase entry)
        {
            InitializeComponent();

            _viewModel = new CopyReferenceDialogViewModel(entry);
            DataContext = _viewModel;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.UnicodeText, _viewModel.Citation);
            Close();
        }
    }
}
