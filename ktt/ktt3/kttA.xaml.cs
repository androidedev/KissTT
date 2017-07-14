using ktt3.Model;
using ktt3.ViewModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ktt3
{
    /// <summary>
    /// Interaction logic for kttA.xaml
    /// </summary>
    public partial class kttA : Window
    {
        private MainWindowViewModel mainWindowViewModel;
        private Job _currentWorkingJob;
        private TimeTrack _currentWorkingTimeTrack;

        // For timer
        private Stopwatch stopWatch = new Stopwatch();
        private DispatcherTimer dispacherTime = new DispatcherTimer();
        private int Ticks;
        private bool isPaused = false;
        private DateTime lastPauseTime;

        // Sounds
        private SoundPlayer TickSound;
        private SoundPlayer DingSound;

        // stores grid columns widths in last round
        private double _panelSettingsWidth;
        private double _panelTimeTrackWidth;

        #region *** Window
        public kttA()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
            InitializeComponent();
            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // keep panel's start width
            _panelSettingsWidth = GridContents.ColumnDefinitions[1].Width.Value;
            _panelTimeTrackWidth = GridContents.ColumnDefinitions[4].Width.Value;

            // set initial status
            ShowSettingsPanel(mainWindowViewModel.SettingsPanelOpen);
            ShowProjectsPanel(mainWindowViewModel.ProjectPanelOpen);
            ShowTasksPanel(mainWindowViewModel.TasksPanelOpen);
            ShowTimeTracksPanel(mainWindowViewModel.TimeTracksPanelOpen);
            EditProjectPanel.Visibility = Visibility.Collapsed;

            // sounds
            TickSound = new System.Media.SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + @"\Resources\sound\tick.wav");
            DingSound = new System.Media.SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + @"\Resources\sound\ding.wav");

            // timer set
            dispacherTime.Tick += new EventHandler(DoTick);
            dispacherTime.Interval = new TimeSpan(0, 0, 0, 1);

            // notify
            MyNotifyIcon.ToolTipText = "Not working";

            // set last project we was working on
            mainWindowViewModel.ProjectViewModel.SetSelectedProject(mainWindowViewModel.LastWorkingProject);
            ProjectsListView.SelectedItem = mainWindowViewModel.ProjectViewModel.SelectedProject;

            // current task text
            currentTaskTextBox.Text = "not working...";
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Save settings for next round
            if (StackPanelProjects.ActualWidth != 0)
                mainWindowViewModel.ProjectPanelWidth = StackPanelProjects.ActualWidth;
            if (StackPanelJobs.ActualWidth != 0)
                mainWindowViewModel.TasksPanelWidth = StackPanelJobs.ActualWidth;
            mainWindowViewModel.LastWorkingProject = ProjectsListView.SelectedItem != null ? (ProjectsListView.SelectedItem as Project).ProjectID : 0;

            //clean up notifyicon (would otherwise stay open until application finishes)
            MyNotifyIcon.Dispose();
            base.OnClosing(e);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (mainWindowViewModel.MinimizeToTray)
            {
                switch (this.WindowState)
                {
                    case WindowState.Maximized:
                        ShowInTaskbar = true;
                        break;
                    case WindowState.Minimized:
                        ShowInTaskbar = false;
                        break;
                    case WindowState.Normal:
                        ShowInTaskbar = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Keyboard shortcuts
        /// </summary>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.J) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                ShowTasksPanel((GridContents.ColumnDefinitions[3].Width.Value == 0));
            }
            else if ((e.Key == Key.P) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                ShowProjectsPanel((GridContents.ColumnDefinitions[2].Width.Value == 0));
            }
            else if ((e.Key == Key.S) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                ShowSettingsPanel((GridContents.ColumnDefinitions[1].Width.Value == 0));
            }
            else if ((e.Key == Key.T) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                ShowTimeTracksPanel((GridContents.ColumnDefinitions[4].Width.Value == 0));
            }
            else if ((e.Key == Key.E) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                ButtonExport_Click(ButtonExport, null);
            }
            else if ((e.Key == Key.Right) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                ButtonPlay_Click(ButtonPlay, null);
            }
            else if ((e.Key == Key.Left) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                ButtonStop_Click(ButtonStop, null);
            }
            else if ((e.Key == Key.Down) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                ButtonPause_Click(ButtonPause, null);
            }
        }
        #endregion

        #region *** Side buttons 

        private void ShowSettingsPanel(bool open)
        {
            mainWindowViewModel.SettingsPanelOpen = open;
            ToggleButtonSettings.IsChecked = open;
            GridContents.ColumnDefinitions[1].Width = open ? new GridLength(_panelSettingsWidth) : new GridLength(0);
        }

        private void ShowProjectsPanel(bool open)
        {
            mainWindowViewModel.ProjectPanelOpen = open;
            ToggleButtonProjects.IsChecked = open;
            GridContents.ColumnDefinitions[2].Width = open ? new GridLength(mainWindowViewModel.ProjectPanelWidth) : new GridLength(0);
        }

        private void ShowTasksPanel(bool open)
        {
            mainWindowViewModel.TasksPanelOpen = open;
            ToggleButtonTasks.IsChecked = open;
            GridContents.ColumnDefinitions[3].Width = open ? new GridLength(mainWindowViewModel.TasksPanelWidth) : new GridLength(0);
        }

        private void ShowTimeTracksPanel(bool open)
        {
            mainWindowViewModel.TimeTracksPanelOpen = open;
            ToggleButtonTimeTracks.IsChecked = open;
            StackPanelTimeTracks.Visibility = open ? StackPanelTimeTracks.Visibility = Visibility.Visible : StackPanelTimeTracks.Visibility = Visibility.Collapsed;
        }

        private void ToggleButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettingsPanel((GridContents.ColumnDefinitions[1].Width.Value == 0));
        }

        private void ToggleButtonProjects_Click(object sender, RoutedEventArgs e)
        {
            ShowProjectsPanel((GridContents.ColumnDefinitions[2].Width.Value == 0));
        }

        private void ToggleButtonTasks_Click(object sender, RoutedEventArgs e)
        {
            ShowTasksPanel((GridContents.ColumnDefinitions[3].Width.Value == 0));
        }

        private void ToggleButtonTimeTracks_Click(object sender, RoutedEventArgs e)
        {
            ShowTimeTracksPanel((GridContents.ColumnDefinitions[4].Width.Value == 0));
        }

        #endregion

        #region *** Project buttons 
        private void ButtonEditProject_Click(object sender, RoutedEventArgs e)
        {
            EditProjectPanel.Visibility = (EditProjectPanel.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ButtonCancelEditProject_Click(object sender, RoutedEventArgs e)
        {
            EditProjectPanel.Visibility = Visibility.Collapsed;
        }

        private void ButtonSaveEditProject_Click(object sender, RoutedEventArgs e)
        {
            EditProjectPanel.Visibility = Visibility.Collapsed;
        }

        private void ButtonDeleteProject_Click(object sender, RoutedEventArgs e)
        {
            // Command="{Binding Path=ProjectViewModel.DeleteProjectCommand}" CommandParameter="{Binding Path=ProjectViewModel.SelectedProject}" 
            bool delete = true;
            if (mainWindowViewModel.ConfirmDeleteProjects)
            {
                var res = MessageBox.Show("Are you sure? \r\nThis will delete cascade all dependant Jobs & TimeTracks also.", "Confirm please", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                delete = (res == MessageBoxResult.Yes);
            }
            if (delete)
            {
                if (mainWindowViewModel.ProjectViewModel.DeleteProjectCommand.CanExecute(mainWindowViewModel.ProjectViewModel.SelectedProject))
                {
                    mainWindowViewModel.ProjectViewModel.DeleteProjectCommand.Execute(mainWindowViewModel.ProjectViewModel.SelectedProject);
                }
            }
        }
        #endregion

        #region *** Jobs DataGrid
        private void DataGridTasks_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = sender as DataGrid;
            if ((e.Key == Key.Delete) && (!(grid.ItemsSource as IEditableCollectionView).IsEditingItem))
            {
                if (grid.SelectedItem != null)
                {
                    bool delete = true;
                    if (mainWindowViewModel.ConfirmDeleteJobs)
                    {
                        var res = MessageBox.Show("Are you sure? \r\nThis will delete cascade all TimeTracks also.", "Confirm please", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        delete = (res == MessageBoxResult.Yes);
                    }
                    if (delete)
                    {
                        var job = ((sender as DataGrid).SelectedItem as Job);
                        if (mainWindowViewModel.JobViewModel.DeleteJobCommand.CanExecute(job))
                            mainWindowViewModel.JobViewModel.DeleteJobCommand.Execute(job);
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            //else if ((e.Key == Key.A) && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
            //{
            //    if (grid.SelectedCells.Count > 0)
            //        grid.SelectedItem = grid.SelectedCells[0].Item;
            //}

            // todo : look why without this Jobs DataGrid needs two ESC key press to cancel and TimeTracks DataGrid only needs one
            else if (e.Key == Key.Escape)
            {
                DataGridTasks.CancelEdit();
            }
        }

        private void DataGridTasks_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if ((e.EditAction == DataGridEditAction.Commit) && e.Row.IsEditing) // && !e.Row.IsNewItem 
            {
                if (mainWindowViewModel.JobViewModel.ModifyJobCommand.CanExecute((e.Row.Item as Model.Job)))
                    mainWindowViewModel.JobViewModel.ModifyJobCommand.Execute((e.Row.Item as Model.Job));
            }
        }

        //private void DataGridTasks_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        //{
        //    //if ((e.EditAction == DataGridEditAction.Commit) && e.Row.IsNewItem)
        //    //{
        //    //    //var j = (e.Row.Item as Job);
        //    //    //if (dataContext.JobViewModel.CreateJobCommand.CanExecute(j))
        //    //    //    dataContext.JobViewModel.CreateJobCommand.Execute(j);
        //    //}
        //    //else if ((e.EditAction == DataGridEditAction.Cancel) && e.Row.IsNewItem)
        //    //{
        //    //    e.Cancel = true;
        //    //}
        //}
        #endregion

        #region *** TimeTracks DataGrid
        private void TimeTracksGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = sender as DataGrid;
            if ((e.Key == Key.Delete) && (!(grid.ItemsSource as IEditableCollectionView).IsEditingItem))
            {
                if (grid.SelectedItem != null)
                {
                    var res = MessageBox.Show("Are you sure?", "Confirm please", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (res != MessageBoxResult.Yes)
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        var timetrack = ((sender as DataGrid).SelectedItem as TimeTrack);
                        if (mainWindowViewModel.TimeTrackViewModel.DeleteTimeTrackCommand.CanExecute(timetrack))
                            mainWindowViewModel.TimeTrackViewModel.DeleteTimeTrackCommand.Execute(timetrack);
                    }
                }
            }
        }

        private void TimeTracksGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if ((e.EditAction == DataGridEditAction.Commit) && e.Row.IsEditing) // && !e.Row.IsNewItem 
            {
                if (mainWindowViewModel.TimeTrackViewModel.ModifyTimeTrackCommand.CanExecute((e.Row.Item as Model.TimeTrack)))
                    mainWindowViewModel.TimeTrackViewModel.ModifyTimeTrackCommand.Execute((e.Row.Item as Model.TimeTrack));
            }
        }

        //private void TimeTracksGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        //{
        //    //if ((e.EditAction == DataGridEditAction.Commit) && e.Row.IsNewItem)
        //    //{
        //    //    //var tt = (e.Row.Item as ktt3.Model.TimeTrack);
        //    //    //if (dataContext.TimeTrackViewModel.CreateTimeTrackCommand.CanExecute(tt))
        //    //    //    dataContext.TimeTrackViewModel.CreateTimeTrackCommand.Execute(tt);
        //    //}
        //    //else if ((e.EditAction == DataGridEditAction.Cancel) && e.Row.IsNewItem)
        //    //{
        //    //    e.Cancel = true;
        //    //}
        //}
        #endregion

        #region *** Timer buttons
        void DoTick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                Ticks = (int)ts.TotalSeconds;
                string currentTime;
                if (mainWindowViewModel.CountBackwards)
                {
                    ts = TimeSpan.FromSeconds(mainWindowViewModel.WorkTimeLen * 60) - ts;
                }
                currentTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                lbTimer.Content = currentTime;
                MyNotifyIcon.ToolTipText = currentTime;
                // end time reached?
                bool end = Ticks >= (mainWindowViewModel.WorkTimeLen * 60);
                if (end)
                {
                    ButtonStop_Click(ButtonStop, null);
                }
            }
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!mainWindowViewModel.AllowFreeTimer && mainWindowViewModel.JobViewModel.SelectedJob == null)
            {
                MessageBox.Show("Please select a Job to start time tracking.");
                return;
            }

            if (stopWatch.IsRunning)
            {
                return;
            }
            else
            {
                if (!isPaused)
                {
                    StartWorking();
                }
                isPaused = false;
                stopWatch.Start();
                dispacherTime.Start();
                if (mainWindowViewModel.SoundEffects)
                    TickSound.Play();
            }
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            isPaused = true;
            lastPauseTime = System.DateTime.Now;
            stopWatch.Stop();
            dispacherTime.Stop();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if ((mainWindowViewModel.SoundEffects) && ((isPaused) || (stopWatch.IsRunning)))
                DingSound.Play();
            EndWorking((isPaused ? lastPauseTime : System.DateTime.Now));
            isPaused = false;
            stopWatch.Stop();
            dispacherTime.Stop();
            stopWatch.Reset();
            lbTimer.Content = "00:00:00";
            MyNotifyIcon.ToolTipText = "Not working";
        }
        #endregion

        #region *** Methods
        private void StartWorking()
        {
            Job job = (Job)DataGridTasks.SelectedItem;
            if (mainWindowViewModel.TimeTrackViewModel.CreateTimeTrackCommand.CanExecute(job))
            {
                mainWindowViewModel.TimeTrackViewModel.CreateTimeTrackCommand.Execute(job);
                _currentWorkingJob = job;
                _currentWorkingTimeTrack = mainWindowViewModel.TimeTrackViewModel.SelectedTimeTrack;
                currentTaskTextBox.Text = string.Format(" Working on : {0}", job.Description);
            }
            else
            {
                currentTaskTextBox.Text = string.Format(" Working on : {0}", "Non created task");
            }
        }

        private void EndWorking(DateTime EndTime)
        {
            currentTaskTextBox.Text = "not working...";
            if (_currentWorkingTimeTrack != null)
            {
                _currentWorkingTimeTrack.EndTime = EndTime;
                mainWindowViewModel.TimeTrackViewModel.ModifyTimeTrackCommand.Execute(_currentWorkingTimeTrack);
            }
        }
        #endregion

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            Ktt3Export w = new Ktt3Export(mainWindowViewModel);
            w.ShowDialog();
            mainWindowViewModel.ProjectViewModel.Projects.Refresh(); // improvement : this should be called only in case of import was made
        }

    }

}
