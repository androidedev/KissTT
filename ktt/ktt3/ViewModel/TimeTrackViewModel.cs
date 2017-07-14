using ktt3.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ktt3.ViewModel
{
    public class TimeTrackViewModel : BindableBase
    {

        private MainWindowViewModel mainViewModel;

        private ObservableCollection<TimeTrack> TimeTracksObservableCollection
        {
            get => (TimeTracks.SourceCollection as ObservableCollection<TimeTrack>);
        }

        // TimeTracks collections for view
        private ICollectionView timeTracks;
        public ICollectionView TimeTracks {
            get => timeTracks;
            set
            {
                timeTracks = value;
                timeTracks.CurrentChanged += new EventHandler(SelectedTimeTrackChanged);
                EditableTimeTracks = (IEditableCollectionView)timeTracks;
            }
        }

        private IEditableCollectionView editabletimetracks;
        public IEditableCollectionView EditableTimeTracks {
            get => editabletimetracks;
            set => this.SetProperty(ref editabletimetracks, value); 
        }

        public TimeTrackViewModel(MainWindowViewModel MainViewModel)
        {
            mainViewModel = MainViewModel;
            // commands for timetracks
            _createTimeTrackCommand = new RelayCommand(CanCreateTimeTrack, CreateTimeTrack);
            _modifyTimeTrackCommand = new RelayCommand(CanModifyTimeTrack, ModifyTimeTrack);
            _CreateEmptyTimeTrackCommand = new RelayCommand(CanCreateEmptyTimeTrack, CreateEmptyTimeTrack);
            _deleteTimeTrackCommand = new RelayCommand(CanDeleteTimeTrack, DeleteTimeTrack);
        }


        #region create timetrack command
        private ICommand _createTimeTrackCommand;
        public ICommand CreateTimeTrackCommand { get => _createTimeTrackCommand; }
        private bool CanCreateTimeTrack(object obj)
        {
            // <param name="obj"> comes with currentJob from code behind
            // if don't have a Job can't create
            if (obj == null)
                return false;
            // otherwise yes
            return true;
        }
        private void CreateTimeTrack(object obj)
        {
            // <param name="obj"> comes with currentJob from code behind
            var tt = new TimeTrack()
            {
                JobID = (obj as Job).JobID,
                WorkDate = System.DateTime.Now,
                StartTime = System.DateTime.Now,
                EndTime = null
            };
            TimeTracksObservableCollection.Add(tt);
            mainViewModel.DbContext.SaveChanges();
            SelectedTimeTrack = tt;
        }
        #endregion

        #region create empty timetrack command
        private ICommand _CreateEmptyTimeTrackCommand;
        public ICommand CreateEmptyTimeTrackCommand { get => _CreateEmptyTimeTrackCommand; }
        private bool CanCreateEmptyTimeTrack(object obj)
        {
            // <param name="obj"> comes with Current selected job, called from  XAML
            if (obj == null)
                return false;
            return true;
        }
        private void CreateEmptyTimeTrack(object obj)
        {
            // <param name="obj"> comes with Current selected job, called from  XAML
            var timeTrack = new TimeTrack()
            {
                JobID = (obj as Job).JobID,
                WorkDate = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            };
            TimeTracksObservableCollection.Add(timeTrack);
            mainViewModel.DbContext.SaveChanges();
        }
        #endregion

        #region modify timetrack
        private ICommand _modifyTimeTrackCommand;
        public ICommand ModifyTimeTrackCommand { get => _modifyTimeTrackCommand; }

        private bool CanModifyTimeTrack(object obj)
        {
            // if don't have a timetrack can't create
            if (obj == null)
                return false;
            // otherwise yes
            return true;
        }

        private void ModifyTimeTrack(object obj)
        {
            TimeTrack modifiedTimeTrack = (obj as TimeTrack);
            TimeTrack collectionTimeTrack = TimeTracksObservableCollection.FirstOrDefault(t => t.TimeTrackID == modifiedTimeTrack.TimeTrackID);
            if (collectionTimeTrack!=null)
            {
                collectionTimeTrack.JobID = modifiedTimeTrack.JobID;
                collectionTimeTrack.TimeTrackID = modifiedTimeTrack.TimeTrackID;
                collectionTimeTrack.WorkDate = modifiedTimeTrack.WorkDate;
                collectionTimeTrack.StartTime = modifiedTimeTrack.StartTime;
                collectionTimeTrack.EndTime = modifiedTimeTrack.EndTime;
                mainViewModel.DbContext.SaveChanges();
            } // improvement : the search will allways find a item, but perhaps we should raise and exception in else...
        }
        #endregion

        #region Delete TimeTrack
        private ICommand _deleteTimeTrackCommand;
        public ICommand DeleteTimeTrackCommand { get => _deleteTimeTrackCommand; }
        private bool CanDeleteTimeTrack(object obj)
        {
            if (obj == null)
                return false;
            return true;
        }
        private void DeleteTimeTrack(object obj)
        {
            mainViewModel.DbContext.TimeTracks.Remove((obj as TimeTrack));
            mainViewModel.DbContext.SaveChanges();
        }
        #endregion

        #region Selected TimeTrack in view
        private TimeTrack selectedTimeTrack;
        public TimeTrack SelectedTimeTrack
        {
            get
            {
                return selectedTimeTrack;
            }
            set
            {
                this.SetProperty(ref selectedTimeTrack, value);
            }
        }
        private void SelectedTimeTrackChanged(object sender, EventArgs e)
        {
            SelectedTimeTrack = timeTracks.CurrentItem as TimeTrack;
        }
        #endregion



    }
}
