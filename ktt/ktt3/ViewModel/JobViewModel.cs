using ktt3.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace ktt3.ViewModel
{
    public class JobViewModel : BindableBase
    {
        private MainWindowViewModel mainViewModel;

        private ObservableCollection<Job> JobsObservableCollection { get => (Jobs.SourceCollection as ObservableCollection<Job>); }

        // Jobs collections for view
        private ICollectionView jobs;
        public ICollectionView Jobs
        {
            get
            {
                return jobs;
            }
            set
            {
                jobs = value;
                jobs.CurrentChanged += new EventHandler(SelectedJobChanged);
                jobs.Filter += JobFilter;
                Editablejobs = (IEditableCollectionView)jobs;
            }
        }

        // Editable Jobs collections for CRUD in DataGrid
        private IEditableCollectionView editablejobs;
        public IEditableCollectionView Editablejobs
        {
            get
            {
                return editablejobs;
            }
            set
            {
                this.SetProperty(ref editablejobs, value);
            }
        }

        public JobViewModel(MainWindowViewModel MainViewModel)
        {
            mainViewModel = MainViewModel;
            // commands for jobs
            _createJobCommand = new RelayCommand(CanCreateJob, CreateJob);
            _modifyJobCommand = new RelayCommand(CanModifyJob, ModifyJob);
            _deleteJobCommand = new RelayCommand(CanDeleteJob, DeleteJob);
        }

        #region Create job command
        private ICommand _createJobCommand;
        public ICommand CreateJobCommand { get => _createJobCommand; }
        private bool CanCreateJob(object obj)
        {
            // if don't have a job description can't create
            if (obj == null)
                return false;
            // if don't have a selected project can't create
            if (mainViewModel.ProjectViewModel.SelectedProject == null)
                return false;
            string jobDescription = obj.ToString();
            // if don't have a description for the job can't create
            if (string.IsNullOrWhiteSpace(jobDescription))
                return false;
            // if there is a job with same description in the project can't create 
            var p = JobsObservableCollection.FirstOrDefault<Job>(pj => pj.Description.ToLower() == jobDescription.ToLower());
            if ((p != null) && (p.ProjectID == mainViewModel.ProjectViewModel.SelectedProject.ProjectID))
                return false;
            // otherwise yes
            return true;
        }
        private void CreateJob(object obj)
        {
            Job job = new Job()
            {
                ProjectID = mainViewModel.ProjectViewModel.SelectedProject.ProjectID,
                Status = _filterJobStatusString,
                Description = obj.ToString()
            };
            JobsObservableCollection.Add(job);
            mainViewModel.DbContext.SaveChanges();
            FilterJobString = "";
            FilterJobStatusString = "";
        }
        #endregion

        #region Modify job command
        private ICommand _modifyJobCommand;
        public ICommand ModifyJobCommand { get => _modifyJobCommand; }
        private bool CanModifyJob(object obj)
        {
            // if don't have a job can't modify
            if (obj == null)
                return false;
            return true;
        }
        private void ModifyJob(object obj)
        {
            Job modifiedJob = (obj as Job);
            Job collectionJob = JobsObservableCollection.FirstOrDefault(j => j.JobID == modifiedJob.JobID);
            if (collectionJob != null)
            {
                collectionJob.ProjectID = modifiedJob.ProjectID;
                collectionJob.JobID = modifiedJob.JobID;
                collectionJob.Status = modifiedJob.Status;
                collectionJob.Description = modifiedJob.Description;
                collectionJob.Priority = modifiedJob.Priority;
                collectionJob.EstimatedTime = modifiedJob.EstimatedTime;
                mainViewModel.DbContext.SaveChanges();
            } // improvement : the search will allways find a item, but perhaps we must raise and exception in else...
        }
        #endregion

        #region Delete job command
        private ICommand _deleteJobCommand;
        public ICommand DeleteJobCommand { get => _deleteJobCommand; }
        private bool CanDeleteJob(object obj)
        {
            // if don't have a job can't modify
            if (obj == null)
                return false;
            // otherwise yes
            return true;
        }
        private void DeleteJob(object obj)
        {
            // EF makes delete cascade for us if model is created with WillCascadeOnDelete(...)
            mainViewModel.DbContext.Jobs.Remove((obj as Job));
            mainViewModel.DbContext.SaveChanges();
        }
        #endregion

        #region Jobs filtering
        private string _filterJobString;
        public string FilterJobString
        {
            get { return _filterJobString; }
            set
            {
                this.SetProperty(ref _filterJobString, value);
                jobs.Refresh();
            }
        }
        private string _filterJobStatusString;
        public string FilterJobStatusString
        {
            get { return _filterJobStatusString; }
            set
            {
                this.SetProperty(ref _filterJobStatusString, value);
                jobs.Refresh();
            }
        }
        private bool JobFilter(object item)
        {
            if (item == null)
                return false;
            if (mainViewModel.ProjectViewModel.SelectedProject == null)
                return false;
            Job job = item as Job;

            // filter job 
            if (!string.IsNullOrEmpty(_filterJobString) && !string.IsNullOrEmpty(_filterJobStatusString))
            {
                if (job.Status != null)
                    return
                    (job.ProjectID == mainViewModel.ProjectViewModel.SelectedProject.ProjectID) &&
                    job.Description.ToLower().Contains(_filterJobString.ToLower()) &&
                    job.Status.ToLower().Contains(_filterJobStatusString.ToLower());
                else
                    return
                    (job.ProjectID == mainViewModel.ProjectViewModel.SelectedProject.ProjectID) &&
                    job.Description.ToLower().Contains(_filterJobString.ToLower());
            }
            else if (!string.IsNullOrEmpty(_filterJobString))
            {
                return (job.ProjectID == mainViewModel.ProjectViewModel.SelectedProject.ProjectID) && job.Description.ToLower().Contains(_filterJobString.ToLower());
            }
            else if (!string.IsNullOrEmpty(_filterJobStatusString))
            {
                if (job.Status != null)
                    return (job.ProjectID == mainViewModel.ProjectViewModel.SelectedProject.ProjectID) && job.Status.ToLower().Contains(_filterJobStatusString.ToLower());
                else
                    return false;
            }
            else
            {
                return job.ProjectID == mainViewModel.ProjectViewModel.SelectedProject.ProjectID;
            }
        }
        #endregion

        #region Selected job in view
        private Job selectedJob;
        public Job SelectedJob
        {
            get
            {
                return selectedJob;
            }
            set
            {
                this.SetProperty(ref selectedJob, value);
            }
        }
        private void SelectedJobChanged(object sender, EventArgs e)
        {
            SelectedJob = jobs.CurrentItem as Job;

            if((mainViewModel.TimeTrackViewModel!=null)&&(SelectedJob!=null))
            {
                if(SelectedJob.TimeTracks==null)
                {
                    SelectedJob.TimeTracks = new ObservableCollection<TimeTrack>();
                }
                mainViewModel.TimeTrackViewModel.TimeTracks = CollectionViewSource.GetDefaultView(SelectedJob.TimeTracks);
            }
        }
        #endregion




    }
}
