using ktt3.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace ktt3.ViewModel
{
    public class ProjectViewModel : BindableBase
    {
        private MainWindowViewModel mainViewModel;

        private ObservableCollection<Project> ProjectsObservableCollection
        {
            get
            {
                // linked to DbSet.Local to maintain synch with DB (https://msdn.microsoft.com/en-gb/data/jj592872.aspx)
                mainViewModel.DbContext.Projects.Load();
                return mainViewModel.DbContext.Projects.Local;
            }
        }

        /// <summary>
        /// Projects collection for view for XAML
        /// </summary>
        private ICollectionView projects;
        public ICollectionView Projects { get => projects; set => projects = value; } 

        public ProjectViewModel(MainWindowViewModel MainViewModel)
        {
            mainViewModel = MainViewModel;
            // Initialize the projects collection
            projects = CollectionViewSource.GetDefaultView(ProjectsObservableCollection);
            projects.CurrentChanged += new EventHandler(SelectedProjectChanged);
            projects.Filter += ProjectFilter;
            // commands for projects
            _createProjectCommand = new RelayCommand(CanCreateProject, CreateProject);
            _deleteProjectCommand = new RelayCommand(CanDeleteProject, DeleteProject);
            _modifyProjectCommand = new RelayCommand(CanModifyProject, ModifyProject);
            _cancelProjectCommand = new RelayCommand(CanCancelProject, CancelProject);
        }

        #region create project
        private ICommand _createProjectCommand;
        public ICommand CreateProjectCommand { get => _createProjectCommand; }
        private bool CanCreateProject(object obj)
        {
            if (obj == null)
                return false;
            string projectname = obj.ToString();
            // if don't have a name for project can't create
            if (string.IsNullOrWhiteSpace(projectname))
                return false;
            // if there is a project of same name can't create
            var p = ProjectsObservableCollection.FirstOrDefault<Project>(pj => pj.Name.ToLower() == projectname.ToLower());
            if (p != null)
                return false;
            // otherwise yes
            return true;
        }
        private void CreateProject(object obj)
        {
            Project p = new Project()
            {
                Name = obj.ToString()
            };
            ProjectsObservableCollection.Add(p);
            mainViewModel.DbContext.SaveChanges();
            SelectedProject = p;
        }
        #endregion

        #region delete project command
        private ICommand _deleteProjectCommand;
        public ICommand DeleteProjectCommand { get => _deleteProjectCommand; }
        private void DeleteProject(object obj)
        {
            // EF makes delete cascade for us if model is created with WillCascadeOnDelete(...)
            ProjectsObservableCollection.Remove((obj as Project));
            mainViewModel.DbContext.SaveChanges();
        }
        private bool CanDeleteProject(object obj)
        {
            if (obj == null)
                return false;
            return true;
        }
        #endregion

        #region modify project command
        private ICommand _modifyProjectCommand;
        public ICommand ModifyProjectCommand { get => _modifyProjectCommand; }
        private void ModifyProject(object obj)
        {
            // https://stackoverflow.com/questions/6781192/how-do-i-update-a-single-item-in-an-observablecollection-class
            Project modifiedProject = (obj as Project);
            Project collectionProject = ProjectsObservableCollection.FirstOrDefault(pj => pj.ProjectID == modifiedProject.ProjectID);
            if (collectionProject != null)
            {
                collectionProject.Name = modifiedProject.Name;
                mainViewModel.DbContext.SaveChanges();
            }
        }
        private bool CanModifyProject(object obj)
        {
            // if we don't have a project selected can't modify
            if (obj == null)
                return false;
            // if the new name is empty can't modify
            if (string.IsNullOrWhiteSpace((obj as Project).Name))
                return false;
            return true;
        }
        #endregion

        #region cancel project changes command
        private ICommand _cancelProjectCommand;
        public ICommand CancelProjectCommand { get => _cancelProjectCommand; }
        private void CancelProject(object obj)
        {
            if (!mainViewModel.DbContext.ChangeTracker.HasChanges())
                return;
            foreach (var entry in mainViewModel.DbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
        private bool CanCancelProject(object obj)
        {
            return true;
        }
        #endregion

        #region filter projects
        private string _filterProjectString;
        public string FilterProjectString
        {
            get { return _filterProjectString; }
            set
            {
                this.SetProperty(ref _filterProjectString, value);
                projects.Refresh();
            }
        }
        private bool ProjectFilter(object item)
        {
            if (_filterProjectString == null)
                return true;
            Project project = item as Project;
            return project.Name.ToLower().Contains(_filterProjectString.ToLower());
        }
        #endregion

        #region Selected project in View
        // Selected project in View
        private Project selectedProject;
        public Project SelectedProject
        {
            get
            {
                return selectedProject;
            }
            set
            {
                this.SetProperty(ref selectedProject, value);
            }
        }
        private void SelectedProjectChanged(object sender, EventArgs e)
        {
            SelectedProject = projects.CurrentItem as Project;
            if ((mainViewModel.JobViewModel != null) && (SelectedProject != null))
            {
                if (SelectedProject.Jobs == null)
                {
                    SelectedProject.Jobs = new ObservableCollection<Job>();
                }
                mainViewModel.JobViewModel.Jobs = CollectionViewSource.GetDefaultView(SelectedProject.Jobs);
            }

        }
        public void SetSelectedProject(int ProjectID)
        {
            Projects.MoveCurrentTo(ProjectsObservableCollection.Where(p => p.ProjectID == ProjectID).FirstOrDefault());
        }
        #endregion

    }
}
