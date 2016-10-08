﻿using System.Windows;
using Ref.Windows.ViewModels;

namespace Ref.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            _viewModel.DisruptingEdit += DisruptingEditHandler;
            _viewModel.Catalogue.Entries.Add(new BookViewModel(new Models.Book() { Author = "ads", Title="ADS" }));
            _viewModel.Catalogue.Entries.Add(new BookViewModel(new Models.Book() { Author = "dada", Title = "Dadaism" }));
            DataContext = _viewModel;
        }

        private MessageBoxResult DisruptingEditHandler()
        {
            return MessageBox.Show("Do you want to save the edit in progress?", "Ref", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        private void catalogueTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _viewModel.SelectEntry((BookViewModel)e.NewValue);
        }

        private void addEntryButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CreateBook();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CancelEdit();
        }

        private void commitButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CommitEdit();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.EditSelected();
        }
    }
}
