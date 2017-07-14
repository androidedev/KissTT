
namespace ktt3.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Project : BindableBase, IDataErrorInfo
    {
        private int _projectID;
        private string _name;

        public int ProjectID
        {
            get { return _projectID; }
            set { SetProperty(ref _projectID, value); }
        }

        [Required]
        [StringLength(25)]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        [NotMapped]
        public int WorkedTime
        {
            get
            {
                if ((Jobs == null) || (Jobs.Count == 0))
                    return 0;
                else
                {
                    return Jobs.Sum(j => j.WorkedTime);
                }
            }
            private set { }
        }

        [NotMapped]
        public int EstimatedTime
        {
            get
            {
                if ((Jobs == null) || (Jobs.Count == 0))
                    return 0;
                else
                {
                    return Jobs.Sum(j => (int?)j.EstimatedTime) ?? 0;
                }
            }
            private set { }
        }

        public virtual ObservableCollection<Job> Jobs { get; set; }

        #region IDataErrorInfo
        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "Name":
                        if (string.IsNullOrEmpty(_name))
                            error = "Project name cannot be empty";
                        if (_name.Length > 25)
                            error = "Project name must be 25 chars or less";
                        break;
                }
                // just return the error or empty string if there is no error
                return error;
            }

        }
        #endregion

    }
}

