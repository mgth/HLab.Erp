using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;
using Xceed.Wpf.AvalonDock;

namespace HLab.Erp.Core.Wpf.ApplicationServices
{
    /// <summary>
    /// Class implements a viewmodel to support the
    /// <seealso cref="AvalonDockLayoutSerializer"/>
    /// attached behavior which is used to implement
    /// load/save of layout information on application
    /// start and shut-down.
    /// </summary>
    public class AvalonDockLayoutViewModel : ViewModel<AvalonDockLayoutViewModel>
    {
        #region fields
        private const string LayoutFileName = "Layout.config";

        //private RelayCommand mLoadWorkspaceLayoutFromStringCommand = null;
        //private RelayCommand mSaveWorkspaceLayoutToStringCommand = null;

        //private RelayCommand<object> mLoadLayoutCommand = null;
        //private RelayCommand<object> mSaveLayoutCommand = null;

        // The XML workspace layout string is stored in this field
        private string _currentLayout;

        readonly ILayoutViewModelParent _Parent = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// Parameterized class constructor to model properties that
        /// are required for access to parent viewmodel.
        /// </summary>
        /// <param name="parent"></param>
        public AvalonDockLayoutViewModel(ILayoutViewModelParent parent)
        {
            this._Parent = parent;
        }

        #endregion

        #region command properties
        #region Load/Save workspace to temporary string

        /// <summary>
        /// Get an ICommand to save the WorkspaceLayout
        /// </summary>
        /// <summary>
        /// This method is executed if the corresponding
        /// Save Workspace Layout command is executed.
        /// </summary>
        public ICommand SaveWorkspaceLayoutToStringCommand => _saveWorkspaceLayoutToStringCommand.Get();
                // Sends a GetWorkspaceLayout message to registered recipients. The message will reach all recipients
                // that registered for this message type using one of the Register methods.

                // TODO

                //Messenger.Default.Send(new NotificationMessageAction<string>(
                //    Notifications.GetWorkspaceLayout,
                //    (result) =>
                //    {
                //        this.mParent.IsBusy = true;
                //        CommandManager.InvalidateRequerySuggested();

                //        this.current_layout = result;
                //        this.mParent.IsBusy = false;
                //    })
                //);
        private IProperty<ICommand> _saveWorkspaceLayoutToStringCommand = H.Property<ICommand>(c => c
        .Command(e => { }, e => true)
        );

        /// <summary>
        /// Get an ICommand to load the WorkspaceLayout
        /// </summary>
        /// <summary>
        /// This method is executed if the corresponding
        /// Load Workspace Layout command is executed.
        /// </summary>
        public ICommand LoadWorkspaceLayoutFromStringCommand => _loadWorkspaceLayoutFromStringCommand.Get();
        private IProperty<ICommand> _loadWorkspaceLayoutFromStringCommand = H.Property<ICommand>(c => c
        .Command(e => {
            // Is there any layout that could possible be loaded?

            // Sends a LoadWorkspaceLayout message to registered recipients. The message will reach all recipients
            // that registered for this message type using one of the Register methods.
            // TODO : Messenger.Default.Send(new NotificationMessage<string>(current_layout, Notifications.LoadWorkspaceLayout));
            if (string.IsNullOrEmpty(e._currentLayout) == true)
                return;
        }, e => true)
        );


        #endregion Load/Save workspace to temporary string

        #region Load Save WorkSpace on application start-up and shutdown
        /// <summary>
        /// Implement a command to load the layout of an AvalonDock-DockingManager instance.
        /// This layout defines the position and shape of each document and tool window
        /// displayed in the application.
        /// 
        /// Parameter:
        /// The command expects a reference to a <seealso cref="DockingManager"/> instance to
        /// work correctly. Not supplying that reference results in not loading a layout (silent return).
        /// </summary>
        public ICommand LoadLayoutCommand => _loadLayoutCommand.Get();
        private IProperty<ICommand> _loadLayoutCommand = H.Property<ICommand>(c => c
        .Command((e,p) => {
                DockingManager docManager = p as DockingManager;

                if (docManager == null)
                    return;

                try
                {
                    e.LoadDockingManagerLayout();
                }
                catch
                {
                }
                finally
                {
                    e._Parent.IsBusy = false;
                }

        }, e => true)
        );

        /// <summary>
        /// Implements a command to save the layout of an AvalonDock-DockingManager instance.
        /// This layout defines the position and shape of each document and tool window
        /// displayed in the application.
        /// 
        /// Parameter:
        /// The command expects a reference to a <seealso cref="string"/> instance to
        /// work correctly. The string is supposed to contain the XML layout persisted
        /// from the DockingManager instance. Not supplying that reference to the string
        /// results in not saving a layout (silent return).
        /// </summary>
        public ICommand SaveLayoutCommand => _saveLayoutCommand.Get();
        private IProperty<ICommand> _saveLayoutCommand = H.Property<ICommand>(c => c
        .Command((e,p) => {
            string xmlLayout = p as string;

            if (xmlLayout == null)
                return;

            e.SaveDockingManagerLayout(xmlLayout);

        }, e => true)
        );



        #endregion Load Save WorkSpace on application start-up and shutdown
        #endregion command properties

        #region methods
        #region Workspace Managment Methods
        private void SaveWorkspaceLayout_Executed()
        {
        }

        private void LoadWorkspaceLayout_Executed()
        {
        }
        #endregion Workspace Managment Methods

        #region LoadLayout
        /// <summary>
        /// Loads the layout of a particular docking manager instance from persistence
        /// and checks whether a file should really be reloaded (some files may no longer
        /// be available).
        /// </summary>
        private void LoadDockingManagerLayout()
        {
            string layoutFileName;

            try
            {
                this._Parent.IsBusy = true;

                layoutFileName = Path.Combine(this._Parent.DirAppData, AvalonDockLayoutViewModel.LayoutFileName);

                if (File.Exists(layoutFileName) == false)
                {
                    this._Parent.IsBusy = false;
                    return;
                }

                string sTaskError = string.Empty;

                Task taskToProcess = null;
                taskToProcess = Task.Factory.StartNew<string>((stateObj) =>
                {
                    string xml = string.Empty;

                    try
                    {
                        // Begin Aysnc Task
                        using (FileStream fs = new FileStream(layoutFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            //using (StreamReader reader = FileReader.OpenStream(fs, Encoding.Default))
                            //{
                            //    xml = reader.ReadToEnd();
                            //}
                        }
                    }
                    catch (OperationCanceledException exp)
                    {
                        throw exp;
                    }
                    catch (Exception except)
                    {
                        throw except;
                    }
                    finally
                    {
                    }

                    return xml;                     // End of async task

                }, null).ContinueWith(ant =>
                {
                    try
                    {
                        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //Messenger.Default.Send(new NotificationMessage<string>(ant.Result, Notifications.LoadWorkspaceLayout));
                            }),
                            DispatcherPriority.Background);
                    }
                    catch (AggregateException aggExp)
                    {
                        throw new Exception("One or more errors have occured during load layout processing.", aggExp);
                    }
                    finally
                    {
                        this._Parent.IsBusy = false;
                    }
                });
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        #endregion LoadLayout

        #region SaveLayout
        private void SaveDockingManagerLayout(string xmlLayout)
        {
            // Create XML Layout file on close application (for re-load on application re-start)
            if (xmlLayout == null)
                return;

            string fileName = System.IO.Path.Combine(this._Parent.DirAppData, AvalonDockLayoutViewModel.LayoutFileName);

            File.WriteAllText(fileName, xmlLayout);
        }
        #endregion SaveLayout
        #endregion methods
    }
}