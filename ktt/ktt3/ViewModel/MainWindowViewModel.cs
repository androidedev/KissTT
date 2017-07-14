using ktt3.DataAccess;
using ktt3.Model;
using ktt3.Util;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace ktt3.ViewModel
{

    /// <summary>
    /// This ViewModel is only a container for the other ViewModels.
    /// Is not intended for decoupling or something like that is here only to split code of viewmodels to make easier to deal with the code
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {

        private Ktt3DbContext dbContext;
        public Ktt3DbContext DbContext { get => dbContext; }

        ProjectViewModel _projectViewModel;
        public ProjectViewModel ProjectViewModel { get => _projectViewModel; set => _projectViewModel = value; }

        JobViewModel _jobViewModel;
        public JobViewModel JobViewModel { get => _jobViewModel; set => _jobViewModel = value; }

        TimeTrackViewModel _timeTrackViewModel;
        public TimeTrackViewModel TimeTrackViewModel { get => _timeTrackViewModel; set => _timeTrackViewModel = value; }

        #region Settings (Binded to settings controls)
        public int WorkTimeLen
        {
            get
            {
                return ktt3.Properties.Settings.Default.WorkTimeLen;
            }
            set
            {
                ktt3.Properties.Settings.Default.WorkTimeLen = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool CountBackwards
        {
            get
            {
                return ktt3.Properties.Settings.Default.CountBackwards;
            }
            set
            {
                ktt3.Properties.Settings.Default.CountBackwards = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool SoundEffects
        {
            get
            {
                return ktt3.Properties.Settings.Default.SoundNotification;
            }
            set
            {
                ktt3.Properties.Settings.Default.SoundNotification = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool AllowFreeTimer
        {
            get
            {
                return ktt3.Properties.Settings.Default.AllowFreeTimer;
            }
            set
            {
                ktt3.Properties.Settings.Default.AllowFreeTimer = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool ConfirmDeleteProjects
        {
            get
            {
                return ktt3.Properties.Settings.Default.ConfirmDeleteProjects;
            }
            set
            {
                ktt3.Properties.Settings.Default.ConfirmDeleteProjects = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }
        public bool ConfirmDeleteJobs
        {
            get
            {
                return ktt3.Properties.Settings.Default.ConfirmDeleteJobs;
            }
            set
            {
                ktt3.Properties.Settings.Default.ConfirmDeleteJobs = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool MinimizeToTray
        {
            get
            {
                return ktt3.Properties.Settings.Default.MinimizeToTray;
            }
            set
            {
                ktt3.Properties.Settings.Default.MinimizeToTray = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public int LastWorkingProject
        {
            get
            {
                return ktt3.Properties.Settings.Default.LastWorkingProject;
            }
            set
            {
                ktt3.Properties.Settings.Default.LastWorkingProject = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool SettingsPanelOpen
        {
            get
            {
                return ktt3.Properties.Settings.Default.SettingsPanelOpen;
            }
            set
            {
                ktt3.Properties.Settings.Default.SettingsPanelOpen = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }
        public bool ProjectPanelOpen
        {
            get
            {
                return ktt3.Properties.Settings.Default.ProjectPanelOpen;
            }
            set
            {
                ktt3.Properties.Settings.Default.ProjectPanelOpen = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }
        public bool TasksPanelOpen
        {
            get
            {
                return ktt3.Properties.Settings.Default.TasksPanelOpen;
            }
            set
            {
                ktt3.Properties.Settings.Default.TasksPanelOpen = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }
        public bool TimeTracksPanelOpen
        {
            get
            {
                return ktt3.Properties.Settings.Default.TimeTracksPanelOpen;
            }
            set
            {
                ktt3.Properties.Settings.Default.TimeTracksPanelOpen = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }
        public double ProjectPanelWidth
        {
            get
            {
                if (Double.IsNaN(ktt3.Properties.Settings.Default.ProjectPanelWidth))
                    return 300;
                return ktt3.Properties.Settings.Default.ProjectPanelWidth;
            }
            set
            {
                ktt3.Properties.Settings.Default.ProjectPanelWidth = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }
        public double TasksPanelWidth
        {
            get
            {
                if (Double.IsNaN(ktt3.Properties.Settings.Default.TasksPanelWidth))
                    return 600;
                return ktt3.Properties.Settings.Default.TasksPanelWidth;
            }
            set
            {
                ktt3.Properties.Settings.Default.TasksPanelWidth = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        // *** Export/Import settings
    
        public bool ExportJobWorkedTime
        {
            get
            {
                return ktt3.Properties.Settings.Default.ExportJobWorkedTime;
            }
            set
            {
                ktt3.Properties.Settings.Default.ExportJobWorkedTime = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool ExportTimeTracksTimes
        {
            get
            {
                return ktt3.Properties.Settings.Default.ExportTimeTracksTimes;
            }
            set
            {
                ktt3.Properties.Settings.Default.ExportTimeTracksTimes = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool SkipEmptyJobs
        {
            get
            {
                return ktt3.Properties.Settings.Default.ExportSkipEmptyJobs;
            }
            set
            {
                ktt3.Properties.Settings.Default.ExportSkipEmptyJobs = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool SkipNotFinishedTimeTracks
        {
            get
            {
                return ktt3.Properties.Settings.Default.ExportSkipNotFinishedTimeTracks;
            }
            set
            {
                ktt3.Properties.Settings.Default.ExportSkipNotFinishedTimeTracks = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool ExportAsHours
        {
            get
            {
                return ktt3.Properties.Settings.Default.ExportAsHours;
            }
            set
            {
                ktt3.Properties.Settings.Default.ExportAsHours = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public string OutputFileName
        {
            get
            {
                return ktt3.Properties.Settings.Default.ExportFileName;
            }
            set
            {
                ktt3.Properties.Settings.Default.ExportFileName = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool ExportHeaders
        {
            get
            {
                return ktt3.Properties.Settings.Default.ExportHeaders;
            }
            set
            {
                ktt3.Properties.Settings.Default.ExportHeaders = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool DeleteBeforeInport
        {
            get
            {
                return ktt3.Properties.Settings.Default.DeleteBeforeInport;
            }
            set
            {
                ktt3.Properties.Settings.Default.DeleteBeforeInport = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }

        public bool ExportEstimatedTimes
        {
            get
            {
                return ktt3.Properties.Settings.Default.ExportEstimatedTimes;
            }
            set
            {
                ktt3.Properties.Settings.Default.ExportEstimatedTimes = value;
                ktt3.Properties.Settings.Default.Save();
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            // create db context 
            dbContext = new Ktt3DbContext();

            // Uncomment this to see sql sentences (https://msdn.microsoft.com/en-us/data/dn469464.aspx#Log.Logging)
            // dbContext.Database.Log = Console.Write;

            // ViewModels
            _projectViewModel = new ProjectViewModel(this);
            _jobViewModel = new JobViewModel(this);
            _timeTrackViewModel = new TimeTrackViewModel(this);

            // export commands
            _ExportNiceToFileCommand = new RelayCommand(CanExportToFileCommand, ExportToFile);
            _ExportDataToFileCommand = new RelayCommand(CanExportDataToFileCommand, ExportDataToFile);
            _ImportDataCommand = new RelayCommand(CanImportData, ImportData);
        }

        #region Export / Import Commands

        // Export nice to txt command
        private ICommand _ExportNiceToFileCommand;
        public ICommand ExportNiceToFileCommand { get => _ExportNiceToFileCommand; }
        private void ExportToFile(object obj)
        {
            Ktt3Exporter e = new Ktt3Exporter(OutputFileName, SkipEmptyJobs, SkipNotFinishedTimeTracks);
            if (obj is Project)
            {
                e.ExportNice((obj as Project), ExportEstimatedTimes, ExportAsHours, ExportJobWorkedTime, ExportTimeTracksTimes);
            }
            else
            {
                e.ExportNice((obj as ICollectionView), ExportEstimatedTimes, ExportAsHours, ExportJobWorkedTime, ExportTimeTracksTimes);
            }
        }
        private bool CanExportToFileCommand(object obj)
        {
            if (obj == null)
                return false;
            return true;
        }

        // Export data to txt command
        private ICommand _ExportDataToFileCommand;
        public ICommand ExportDataToFileCommand { get => _ExportDataToFileCommand; }
        private void ExportDataToFile(object obj)
        {
            Ktt3Exporter e = new Ktt3Exporter(OutputFileName, SkipEmptyJobs, SkipNotFinishedTimeTracks);
            if (obj is Project)
            {
                e.ExportData(ExportHeaders, (obj as Project));
            }
            else
            {
                e.ExportData(ExportHeaders, (obj as ICollectionView));
            }
        }
        private bool CanExportDataToFileCommand(object obj)
        {
            if (obj == null)
                return false;
            return true;
        }

        // Import data from txt commmand
        private ICommand _ImportDataCommand;
        public ICommand ImportDataCommand { get => _ImportDataCommand; }
        private void ImportData(object obj)
        {
            Ktt3Exporter e = new Ktt3Exporter(OutputFileName, SkipEmptyJobs, SkipNotFinishedTimeTracks);
            e.ImportData(dbContext, DeleteBeforeInport);
        }
        private bool CanImportData(object obj)
        {
            if (string.IsNullOrEmpty(OutputFileName))
                return false;
            if(!System.IO.File.Exists(OutputFileName))
                return false;
            return true;
        }
        #endregion

    }
}
