namespace ktt3.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Job : BindableBase, IEditableObject, IDataErrorInfo
    {

        private int _projectID;
        private int _jobID;
        private string _status;
        private string _description;
        private int? _estimatedTime;
        private int _priority;

        public int ProjectID
        {
            get { return _projectID; }
            set { SetProperty(ref _projectID, value); }
        }

        public int JobID
        {
            get { return _jobID; }
            set { SetProperty(ref _jobID, value); }
        }

        [MaxLength(1)]
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        [Required]
        [StringLength(500)]
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public int? EstimatedTime
        {
            get { return _estimatedTime; }
            set { SetProperty(ref _estimatedTime, value); }
        }

        public int Priority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }

        public virtual Project Project { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<TimeTrack> TimeTracks { get; set; }

        [NotMapped]
        public int WorkedTime
        {
            get
            {
                if ((TimeTracks == null) || (TimeTracks.Count == 0))
                    return 0;
                else
                {
                    return TimeTracks.Sum(tt => tt.WorkedTime) ?? -1;
                }
            }
            private set { }
        }

        public override string ToString()
        {
            return $"ProjecID: {_projectID}, JobID: {_jobID}, Status: {_status}, EstimatedTime: {_estimatedTime}, Description: {_description}";
        }

        #region IDataErrorInfo
        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                //this get will be invoked everytime user change the fields, columnName contains the property name which is modified by user.
                string error = string.Empty;
                switch (columnName)
                {
                    case "ProjectID":
                        if (_projectID <= 0)
                            error = "Project ID is required";
                        break;
                    case "Description":
                        if (string.IsNullOrWhiteSpace(_description))
                            error = "Description cannot be empty";
                        else if (_description.Length > 500)
                            error = "Description must be 500 chars or less";
                        break;
                    case "Status":
                        if (_status.Length > 1)
                            error = "Status must be 1 char max length";
                        break;
                    case "EstimatedTime":
                        if ((_estimatedTime != null) && (_estimatedTime < 0))
                            error = "Estimated Time (if set) cant' be less than 0";
                        break;
                }
                //just return the error or empty string if there is no error
                return error;
            }

        }

        #endregion

        #region IEditableObject

        private Job _cachedCopy = null;

        public void BeginEdit()
        {
            if (IsInEditMode)
                return;
            _cachedCopy = new Job()
            {
                _projectID = _projectID,
                _jobID = _jobID,
                _status = _status,
                _description = _description,
                _estimatedTime = _estimatedTime,
                _priority = _priority
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
                ProjectID = _cachedCopy._projectID;
                JobID = _cachedCopy._jobID;
                Status = _cachedCopy._status;
                Description = _cachedCopy._description;
                EstimatedTime = _cachedCopy._estimatedTime;
                Priority = _cachedCopy._priority;
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

    }
}

