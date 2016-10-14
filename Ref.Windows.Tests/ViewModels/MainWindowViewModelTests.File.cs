using System.IO;
using System.Reflection;
using System.Windows;
using NUnit.Framework;
using Polsys.Ref.Export;
using Polsys.Ref.Models;
using Polsys.Ref.ViewModels;

namespace Polsys.Ref.Tests.ViewModels
{
    partial class MainWindowViewModelTests
    {
        public class FileOperations
        {
            private string ExistingDataFolder;
            private string TempFolder;

            [OneTimeSetUp]
            public void Setup()
            {
                var testRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                ExistingDataFolder = Path.Combine(testRoot, "TestFiles");
                var folderName = Path.Combine(testRoot, Path.GetRandomFileName());
                TempFolder = Directory.CreateDirectory(folderName).FullName;
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Directory.Delete(TempFolder, true);
            }

            [Test]
            public void ExportCatalogue_AsksIfEditing()
            {
                var vm = new MainWindowViewModel();
                vm.DisruptingEdit += () =>
                {
                    return MessageBoxResult.Cancel;
                };
                vm.CreateBook();

                Assert.That(vm.ExportCatalogue(), Is.EqualTo(OperationResult.Canceled));
            }

            [Test]
            public void ExportCatalogue_CancelsIfNoFileSelected()
            {
                var vm = new MainWindowViewModel();
                vm.SelectingExportFilename += () =>
                {
                    return null;
                };

                Assert.That(vm.ExportCatalogue(), Is.EqualTo(OperationResult.Canceled));
            }

            [Test]
            public void ExportCatalogue_ExportsBibTex()
            {
                var filename = Path.Combine(TempFolder, "Export_ExportsBibTex.bib");
                var vm = new MainWindowViewModel();
                vm.SelectingExportFilename += () =>
                {
                    // Select the BibTeX exporter
                    foreach (var exporter in vm.Exporters)
                    {
                        if (exporter is BibTexExporter)
                            vm.SelectedExporter = exporter;
                    }
                    return filename;
                };

                Assert.That(vm.ExportCatalogue(), Is.EqualTo(OperationResult.Succeeded));
                Assert.That(File.Exists(filename));
            }

            [Test]
            public void ExportCatalogue_FailsIfExporterFails()
            {
                var vm = new MainWindowViewModel();
                vm.SelectingExportFilename += () =>
                {
                    return "C:\\*.bib";
                };

                Assert.That(vm.ExportCatalogue(), Is.EqualTo(OperationResult.Failed));
            }

            [Test]
            public void NewProject_AsksWhenEditing()
            {
                var vm = new MainWindowViewModel();
                bool eventFired = false;
                vm.DisruptingEdit += () =>
                {
                    eventFired = true;
                    return MessageBoxResult.Cancel;
                };
                vm.CreateBook();

                vm.NewProject();
                Assert.That(eventFired, Is.True);
                Assert.That(vm.SelectedEntry, Is.InstanceOf<BookViewModel>());
            }

            [Test]
            public void NewProject_AsksWhenUnsaved()
            {
                var vm = new MainWindowViewModel();
                vm.DiscardingUnsavedChanges += () =>
                {
                    return MessageBoxResult.Yes;
                };
                vm.SelectingSaveFilename += () =>
                {
                    Assert.Pass();
                    return "This will not be reached";
                };
                vm.CreateBook();
                vm.CommitEdit(); // Set the modified flag

                // If the save dialog is opened, the test is passed
                vm.NewProject();
                Assert.Fail();
            }

            [Test]
            public void NewProject_ResetsEverything()
            {
                var vm = new MainWindowViewModel();
                vm.ProjectName = "Proj";
                var book = new BookViewModel(TestUtility.CreateMakeAndDo());
                vm.Catalogue.AddBook(book); // This does not set the modified flag
                vm.SelectEntry(book);

                TestUtility.AssertRaisesPropertyChanged(vm, () => vm.NewProject(), "Catalogue");
                Assert.That(vm.Catalogue.Entries, Is.Empty);
                Assert.That(vm.IsModified, Is.False);
                Assert.That(vm.ProjectName, Is.Empty);
                Assert.That(vm.SelectedEntry, Is.Null);
            }

            [Test]
            public void OpenProject_AsksWhenEditing()
            {
                var vm = new MainWindowViewModel();
                vm.DisruptingEdit += () =>
                {
                    Assert.Pass();
                    return MessageBoxResult.Cancel;
                };
                vm.CreateBook();

                Assert.That(vm.OpenProject(), Is.EqualTo(OperationResult.Canceled));
                Assert.Fail();
            }

            [Test]
            public void OpenProject_AsksWhenUnsaved()
            {
                var vm = new MainWindowViewModel();
                vm.DiscardingUnsavedChanges += () =>
                {
                    Assert.Pass();
                    return MessageBoxResult.Cancel;
                };
                vm.CreateBook();
                vm.CommitEdit();

                Assert.That(vm.OpenProject(), Is.EqualTo(OperationResult.Canceled));
                Assert.Fail();
            }

            [Test]
            public void OpenProject_ClearsWorkspace()
            {
                // Make sure that SelectedEntry is nulled

                var vm = new MainWindowViewModel();
                vm.CreateBook();
                vm.CommitEdit();

                var filename = Path.Combine(ExistingDataFolder, "BookWithPage.refproject");
                vm.DiscardingUnsavedChanges += () => { return MessageBoxResult.No; };
                vm.SelectingOpenFilename += () =>
                {
                    return filename;
                };

                Assert.That(vm.OpenProject(), Is.EqualTo(OperationResult.Succeeded));
                Assert.That(vm.SelectedEntry, Is.Null);
            }

            [Test]
            public void OpenProject_HandlesCorruptFile()
            {
                var vm = new MainWindowViewModel();
                vm.SelectingOpenFilename += () =>
                {
                    return Path.Combine(ExistingDataFolder, "NewerVersion.refproject");
                };

                Assert.That(vm.OpenProject(), Is.EqualTo(OperationResult.Failed));
            }

            [Test]
            public void OpenProject_HandlesMissingFile()
            {
                var vm = new MainWindowViewModel();
                vm.SelectingOpenFilename += () =>
                {
                    return Path.Combine(ExistingDataFolder, "HighlyNonexistentFile.refproject");
                };

                Assert.That(vm.OpenProject(), Is.EqualTo(OperationResult.Failed));
            }

            [Test]
            public void OpenProject_HandlesOpenFile()
            {
                var vm = new MainWindowViewModel();
                var filename = Path.Combine(TempFolder, "Open_HandlesOpenFile.refproject");
                vm.SelectingOpenFilename += () =>
                {
                    return filename;
                };

                using (var file = File.Open(filename, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                {
                    Assert.That(vm.OpenProject(), Is.EqualTo(OperationResult.Failed));
                }
            }

            [Test]
            public void OpenProject_LoadsProject()
            {
                var vm = new MainWindowViewModel();
                var filename = Path.Combine(ExistingDataFolder, "BookWithPage.refproject");
                vm.SelectingOpenFilename += () =>
                {
                    return filename;
                };

                Assert.That(vm.OpenProject(), Is.EqualTo(OperationResult.Succeeded));
                Assert.That(vm.Filename, Is.EqualTo(filename));
                Assert.That(vm.ProjectName, Is.EqualTo("BookWithPage"));
                Assert.That(vm.IsModified, Is.False);

                Assert.That(vm.Catalogue.Entries, Has.Exactly(1).InstanceOf<BookViewModel>());
                Assert.That(vm.Catalogue.Entries[0]._book, Is.Not.Null);
                Assert.That(vm.Catalogue.Entries[0].Title, Is.EqualTo("Letters to a Young Mathematician"));
                Assert.That(vm.Catalogue.Entries[0].Pages, Has.Exactly(1).InstanceOf<PageViewModel>());
                Assert.That(vm.Catalogue.Entries[0].Pages[0]._page, Is.Not.Null);
                Assert.That(vm.Catalogue.Entries[0].Pages[0].Title, Is.EqualTo("Math inside"));
                Assert.That(vm.Catalogue.Entries[0].Pages[0]._parent, Is.SameAs(vm.Catalogue.Entries[0]));
            }

            [Test]
            public void Roundtrip_EmptyProject()
            {
                // Just make sure nothing throws

                var vm = new MainWindowViewModel();
                var filename = Path.Combine(TempFolder, Path.GetRandomFileName());
                vm.SelectingOpenFilename += () => { return filename; };
                vm.SelectingSaveFilename += () => { return filename; };

                vm.SaveProject(false);
                vm.OpenProject();
            }

            [Test]
            public void SaveProject_AsksWhenEditing()
            {
                var vm = new MainWindowViewModel();
                vm.DisruptingEdit += () =>
                {
                    Assert.Pass();
                    return MessageBoxResult.Cancel;
                };
                vm.CreateBook();

                Assert.That(vm.SaveProject(false), Is.EqualTo(OperationResult.Canceled));
                Assert.Fail();
            }

            [Test]
            public void SaveProject_Cancellable()
            {
                var vm = new MainWindowViewModel();
                vm.SelectingSaveFilename += () =>
                {
                    return null;
                };

                Assert.That(vm.SaveProject(false), Is.EqualTo(OperationResult.Canceled));
            }

            [Test]
            public void SaveProject_HandlesInvalidFilename()
            {
                var vm = new MainWindowViewModel();
                vm.SelectingSaveFilename += () =>
                {
                    return "C:\\**ERROR**";
                };

                Assert.That(vm.SaveProject(false), Is.EqualTo(OperationResult.Failed));
            }

            [Test]
            public void SaveProject_HandlesOpenFile()
            {
                var filename = Path.Combine(TempFolder, "Save_HandlesOpenFile.refproject");
                using (var file = File.Open(filename, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                {
                    var vm = new MainWindowViewModel();
                    vm.SelectingSaveFilename += () =>
                    {
                        return filename;
                    };

                    Assert.That(vm.SaveProject(false), Is.EqualTo(OperationResult.Failed));
                }
            }

            [Test]
            public void SaveProject_SaveAsksForFilename()
            {
                var vm = new MainWindowViewModel();
                vm.SelectingSaveFilename += () =>
                {
                    Assert.Pass();
                    return "Unreached";
                };

                vm.SaveProject(false);
                Assert.Fail();
            }

            [Test]
            public void SaveProject_SaveRemembersFilename()
            {
                var vm = new MainWindowViewModel();
                var numRaised = 0;
                vm.SelectingSaveFilename += () =>
                {
                    numRaised++;
                    return Path.Combine(TempFolder, "SaveRemembersFilename.refproject");
                };

                Assert.That(vm.SaveProject(false), Is.EqualTo(OperationResult.Succeeded));
                Assert.That(numRaised, Is.EqualTo(1));
                Assert.That(vm.SaveProject(false), Is.EqualTo(OperationResult.Succeeded));
                Assert.That(numRaised, Is.EqualTo(1));
            }

            [Test]
            public void SaveProject_SaveAsAsksForFilename()
            {
                var vm = new MainWindowViewModel();
                var numRaised = 0;
                vm.SelectingSaveFilename += () =>
                {
                    numRaised++;
                    return Path.Combine(TempFolder, "SaveAsAsksForFilename.refproject");
                };

                Assert.That(vm.SaveProject(false), Is.EqualTo(OperationResult.Succeeded));
                Assert.That(numRaised, Is.EqualTo(1));
                Assert.That(vm.SaveProject(true), Is.EqualTo(OperationResult.Succeeded));
                Assert.That(numRaised, Is.EqualTo(2));
            }

            [Test]
            public void SaveProject_SavesProject()
            {
                var filename = Path.Combine(TempFolder, "SavesProject.refproject");
                var vm = new MainWindowViewModel();
                vm.SelectingSaveFilename += () =>
                {
                    return filename;
                };
                var book = new BookViewModel(TestUtility.CreateMakeAndDo());
                book.AddPage(new PageViewModel(CreateOnKnotsPage()));
                vm.Catalogue.AddBook(book);
                vm.CreateBook();
                vm.CommitEdit(); // Force IsModified to be true

                // ViewModel properties
                Assert.That(vm.SaveProject(false), Is.EqualTo(OperationResult.Succeeded));
                Assert.That(vm.Filename, Is.EqualTo(filename));
                Assert.That(vm.ProjectName, Is.EqualTo("SavesProject"));
                Assert.That(vm.IsModified, Is.False);

                // Check the file
                Assert.That(File.Exists(filename), Is.True);
                var fileContents = File.ReadAllText(filename);
                Assert.That(fileContents, Does.Contain("Make and Do in the Fourth Dimension"));
                Assert.That(fileContents, Does.Contain("On knots"));
            }
        }
    }
}
