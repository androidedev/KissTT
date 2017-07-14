
namespace ktt3.Model
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class TimeTrack : BindableBase, IEditableObject
    {
        private int _TimeTrackID;
        private int _JobID;
        private DateTime _WorkDate;
        private DateTime _StartTime;
        private DateTime? _EndTime;
        private int? _WorkedTime;

        public int TimeTrackID { get => _TimeTrackID; set { SetProperty(ref _TimeTrackID, value); } }
        public int JobID { get => _JobID; set { SetProperty(ref _JobID, value); } }
        public DateTime WorkDate { get => _WorkDate; set { SetProperty(ref _WorkDate, value); } }
        public DateTime StartTime { get => _StartTime; set { SetProperty(ref _StartTime, value); } }
        public DateTime? EndTime { get => _EndTime; set { SetProperty(ref _EndTime, value); } }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? WorkedTime { get => _WorkedTime; private set { SetProperty(ref _WorkedTime, value); } }

        public virtual Job Job { get; set; }

        #region IEditableObject
        private TimeTrack _cachedCopy = null;

        public void BeginEdit()
        {
            if (IsInEditMode)
                return;
            _cachedCopy = new TimeTrack()
            {
                _TimeTrackID = _TimeTrackID,
                _JobID = _JobID,
                _WorkDate = _WorkDate,
                _StartTime = _StartTime,
                _WorkedTime = _WorkedTime
            };
            IsInEditMode = true;
        }

        public void CancelEdit()
        {
            if (!IsInEditMode)
                return;
            // restore original object state
            if (_cachedCopy != null)
            {
                TimeTrackID = _TimeTrackID;
                JobID = _JobID;
                WorkDate = _WorkDate;
                StartTime = _StartTime;
                //_WorkedTime = _WorkedTime;
            }
            _cachedCopy = null; // clear cached data
            IsInEditMode = false;
        }

        public void EndEdit()
        {
            _cachedCopy = null; // clear cached data
            IsInEditMode = false;
        }

        private bool _isInEditMode = false;

        /// <summary>
        /// This flag is necessary due to a bug or extrange feature in DataGrid that makes the events called twice see :
        /// https://stackoverflow.com/questions/4450878/wpf-datagrid-calls-beginedit-on-an-ieditableobject-two-times
        /// </summary>
        [NotMapped]
        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            private set
            {
                if (_isInEditMode != value)
                {
                    SetProperty(ref _isInEditMode, value);
                }
            }
        }
        #endregion

        // todo: this class could implement IDataErrorInfo  but i don't need it by now

    }
}
