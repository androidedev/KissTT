using ktt3.DataAccess;
using ktt3.Model;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;

namespace ktt3.Util
{
    public class Ktt3Exporter
    {

        private bool _SkipEmptyJobs; // all exports
        private bool _SkipNotFinishedTimeTracks; // all exports
        private string _OutputFileName; // all exports

        private bool _ExportAsHours; // only for nice export
        private bool _ExportJobWorkedTime; // only for nice export
        private bool _ExportTimeTracksTimes; // only for nice export
        private bool _ExportEstimatedTimes;  // only for nice export

        public Ktt3Exporter(string OutputFileName, bool SkipEmptyJobs, bool SkipNotFinishedTimeTracks)
        {
            _OutputFileName = OutputFileName;
            _SkipEmptyJobs = SkipEmptyJobs;
            _SkipNotFinishedTimeTracks = SkipNotFinishedTimeTracks;
        }

        // Export nice **************************************************************************************************************

        public void ExportNice(Project project, bool ExportEstimatedTimes, bool ExportAsHours, bool ExportJobWorkedTime, bool ExportTimeTracksTimes)
        {
            _ExportEstimatedTimes = ExportEstimatedTimes;
            _ExportAsHours = ExportAsHours;
            _ExportJobWorkedTime = ExportJobWorkedTime;
            _ExportTimeTracksTimes = ExportTimeTracksTimes;
            string outfile = _OutputFileName;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outfile, false, Encoding.UTF8))
            {
                ExportNice(file, project);
            }
        }

        public void ExportNice(ICollectionView _projects, bool ExportEstimatedTimes, bool ExportAsHours, bool ExportJobWorkedTime, bool ExportTimeTracksTimes)
        {
            _ExportEstimatedTimes = ExportEstimatedTimes;
            _ExportAsHours = ExportAsHours;
            _ExportJobWorkedTime = ExportJobWorkedTime;
            _ExportTimeTracksTimes = ExportTimeTracksTimes;
            string outfile = _OutputFileName;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outfile, false, Encoding.UTF8))
            {
                foreach (Project project in _projects)
                {
                    ExportNice(file, project);
                }
            }
        }

        private string GetFormatedTime(int? time)
        {
            int time2 = time ?? 0;
            if (_ExportAsHours)
            {
                double t = time2 / (double)60;
                return string.Format(t > 60 ? "{0,3:###.#}" : "{0,3:##0.#}", t);
            }
            else
            {
                return string.Format("{0,3:###}", time);
            }
        }

        private void ExportNice(StreamWriter file, Project project)
        {
            int jobpad = 40;
            string w;
            int projectpad = 25;

            string projectName;
            string formatedWTime = GetFormatedTime(project.WorkedTime);
            string formatedETime = GetFormatedTime(project.EstimatedTime);
            if (_ExportEstimatedTimes)
            {
                projectName = string.Format($"{project.Name} ({formatedWTime}/{formatedETime})");
            }
            else
            {
                projectName = string.Format($"{project.Name} ({formatedWTime})");
            }
            projectName = projectName.PadRight(projectpad, ' ');
            file.WriteLine("");
            file.WriteLine("─".PadRight(projectpad + 100, '─'));
            file.WriteLine(projectName);
            file.WriteLine("─".PadRight(projectpad + 100, '─'));

            if ((project.Jobs != null) && (project.Jobs.Count >= 0))
            {
                foreach (Job job in project.Jobs)
                {
                    if ((_SkipEmptyJobs) && ((job.TimeTracks == null) || (job.TimeTracks.Count == 0)))
                        continue;
                    bool first = true;
                    string pProjectJob = "";
                    string jobOutputLine;
                    string jobStatus = (job.Status ?? " ");
                    string jobDescription = job.Description.PadRight(jobpad, ' ');
                    string jobWTime = GetFormatedTime(job.WorkedTime);
                    string jobETime = GetFormatedTime(job.EstimatedTime);
                    if (_ExportJobWorkedTime && _ExportEstimatedTimes)
                    {
                        pProjectJob = $"{jobStatus} {jobDescription} ({jobWTime}/{jobETime})";
                    }
                    else if (_ExportJobWorkedTime)
                    {
                        pProjectJob = $"{jobStatus} {jobDescription} ({jobWTime})";
                    }
                    else if (_ExportEstimatedTimes)
                    {
                        pProjectJob = $"{jobStatus} {jobDescription} ({jobETime})";
                    }
                    else
                    {
                        pProjectJob = $"{jobStatus} {jobDescription} ";
                    }
                    jobOutputLine = pProjectJob;
                    int wl = jobOutputLine.Length;
                    if ((job.TimeTracks == null) || (job.TimeTracks.Count == 0))
                    {
                        file.WriteLine(jobOutputLine);
                    }
                    else
                    {
                        string pAll;
                        string pTimeTrack;
                        if (_ExportTimeTracksTimes)
                        {
                            pAll = "{0} > {1} : {2} - {3} ({4})";
                            pTimeTrack = "{0}   {1} : {2} - {3} ({4})";
                        }
                        else
                        {
                            pAll = "{0} > {1} : {2}";
                            pTimeTrack = "{0}   {1} : {2}";
                        }
                        int idxTT = 0;
                        TimeTrack tt;
                        do
                        {
                            tt = job.TimeTracks[idxTT];
                            if (!((_SkipNotFinishedTimeTracks) && (tt.EndTime == null)))
                            {
                                if (first)
                                {
                                    if (_ExportTimeTracksTimes)
                                    {
                                        w = string.Format(pAll, jobOutputLine, tt.WorkDate.ToString("dd/MM/yyyy"), tt.StartTime.ToString("HH:mm"), tt.EndTime?.ToString("HH:mm"),
                                            GetFormatedTime(tt.WorkedTime)
                                            );
                                    }
                                    else
                                    {
                                        w = string.Format(pAll, jobOutputLine, tt.WorkDate.ToString("dd/MM/yyyy"), GetFormatedTime(tt.WorkedTime));
                                    }
                                    first = false;
                                }
                                else
                                {
                                    if (_ExportTimeTracksTimes)
                                    {
                                        w = string.Format(pTimeTrack, " ".PadRight(wl, ' '), tt.WorkDate.ToString("dd/MM/yyyy"), tt.StartTime.ToString("HH:mm"), tt.EndTime?.ToString("HH:mm"),
                                            GetFormatedTime(tt.WorkedTime)
                                            );
                                    }
                                    else
                                    {
                                        w = string.Format(pTimeTrack, " ".PadRight(wl, ' '), tt.WorkDate.ToString("dd/MM/yyyy"), GetFormatedTime(tt.WorkedTime));
                                    }
                                }
                                file.WriteLine(w);
                            }
                            idxTT++;
                        } while (idxTT < job.TimeTracks.Count);
                    }
                }
            }
        }

        // Export data for import ***************************************************************************************************

        private bool _PadData = false;
        private string _DataSep = "|";
        private char padChar = ' ';
        private int padProjectID = 9;
        private int padProjectName = 25;
        private int padJobID = 5;
        private int padJobDescription = 50;
        private int padJobStatus = 9;
        private int padJobEstimatedTime = 13;
        private int padTimeTrackId = 11;
        private int padTimeTrackWorkDate = 10;
        private int padTimeTrackStartTime = 16;
        private int padTimeTrackEndTime = 16;

        private void ExportHeaders(StreamWriter file)
        {
            string projectId = "ProjectID"; // _PadData ? "ProjectID".ToString().PadLeft(padProjectID, padChar) : "ProjectID";
            string projectName = _PadData ? "ProjectName".PadRight(padProjectName, padChar) : "ProjectName";
            string projectData = string.Format($"{projectId}{_DataSep}{projectName}");
            string jobId = "JobID"; // _PadData ? "JobID".PadLeft(padJobID, padChar) : "JobID";
            string priority = "Priority";
            string jobDescription = _PadData ? "Description".PadRight(padJobDescription, padChar) : "Description";
            string jobEstimatedTime = "EstimatedTime";
            string jobStatus = "JobStatus";
            string jobData = string.Format($"{_DataSep}{jobId}{_DataSep}{priority}{_DataSep}{jobStatus}{_DataSep}{jobEstimatedTime}{_DataSep}{jobDescription}");
            string TimeTrackID = "TimeTrackID"; // _PadData ? "TimeTrackID".PadLeft(padTTId, padChar) : "TimeTrackID";
            string WorkDate = _PadData ? "WorkDate".PadRight(padTimeTrackWorkDate, ' ') : "WorkDate";
            string StartTime = _PadData ? "StartTime".PadRight(padTimeTrackStartTime, ' ') : "StartTime";
            string EndTime = _PadData ? "EndTime".PadRight(padTimeTrackEndTime, ' ') : "EndTime";
            string timeTrackData = string.Format($"{_DataSep}{TimeTrackID}{_DataSep}{WorkDate}{_DataSep}{StartTime}{_DataSep}{EndTime}");
            string all = string.Format($"{projectData}{jobData}{timeTrackData}"); // , projectData, jobData, timeTrackData
            file.WriteLine(all);
        }

        public void ExportData(bool _ExportHeaders, ICollectionView _projects)
        {
            string outfile = _OutputFileName;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outfile, false, Encoding.UTF8))
            {
                if (_ExportHeaders)
                    ExportHeaders(file);
                foreach (Project project in _projects)
                    ExportData(file, project);
            }
        }

        public void ExportData(bool _ExportHeaders, Project project)
        {
            string outfile = _OutputFileName;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outfile, false, Encoding.UTF8))
            {
                if (_ExportHeaders)
                    ExportHeaders(file);
                ExportData(file, project);
            }
        }

        private void ExportData(StreamWriter file, Project project)
        {
            string projectId = _PadData ? project.ProjectID.ToString().PadLeft(padProjectID, padChar) : project.ProjectID.ToString();
            string projectName = _PadData ? project.Name.PadRight(padProjectName, padChar) : project.Name;
            string projectData = string.Format($"{projectId}{_DataSep}{projectName}", projectId, _DataSep, projectName);

            if ((project.Jobs == null) || (project.Jobs.Count <= 0))
            {
                file.WriteLine(projectData);
            }
            else
            {
                foreach (Job job in project.Jobs)
                {
                    string jobId = _PadData ? job.JobID.ToString().PadLeft(padJobID, padChar) : job.JobID.ToString();
                    string priority = job.Priority.ToString();
                    string jobDescription = _PadData ? job.Description.ToString().PadRight(padJobDescription, padChar) : job.Description;
                    string jobEstimatedTime = job.EstimatedTime == null ? "null" : job.EstimatedTime.ToString();
                    jobEstimatedTime = _PadData ? jobEstimatedTime.PadLeft(padJobEstimatedTime, padChar) : jobEstimatedTime;
                    string jobStatus = job.Status != null ? job.Status : "null";
                    jobStatus = _PadData ? jobStatus.PadLeft(padJobStatus, padChar) : jobStatus;
                    string jobData = string.Format($"{_DataSep}{jobId}{_DataSep}{priority}{_DataSep}{jobStatus}{_DataSep}{jobEstimatedTime}{_DataSep}{jobDescription}");
                    if ((job.TimeTracks == null) || (job.TimeTracks.Count == 0))
                    {
                        if (!_SkipEmptyJobs)
                            file.WriteLine(string.Format($"{projectData}{jobData}", projectData, jobData));
                    }
                    else
                    {
                        foreach (TimeTrack tt in job.TimeTracks)
                        {
                            string TimeTrackID = _PadData ? tt.TimeTrackID.ToString().PadLeft(padTimeTrackId, padChar) : tt.TimeTrackID.ToString();

                            string StartTime = tt.StartTime.ToString("dd/MM/yyyy HH:mm");
                            StartTime = _PadData ? StartTime.PadLeft(padTimeTrackStartTime, padChar) : StartTime;

                            if (_SkipEmptyJobs && (tt.EndTime == null))
                                continue;

                            string EndTime = tt.EndTime == null ? "null" : tt.EndTime?.ToString("dd/MM/yyyy HH:mm");
                            EndTime = _PadData ? EndTime.PadLeft(padTimeTrackEndTime, padChar) : EndTime;

                            string WorkedDate = tt.WorkDate.ToString("dd/MM/yyyy");
                            WorkedDate = _PadData ? WorkedDate.PadLeft(padTimeTrackWorkDate, padChar) : WorkedDate;

                            string timeTrackData = string.Format($"{_DataSep}{TimeTrackID}{_DataSep}{WorkedDate}{_DataSep}{StartTime}{_DataSep}{EndTime}");
                            string all = string.Format($"{projectData}{jobData}{timeTrackData}", projectData, jobData, timeTrackData);
                            file.WriteLine(all);
                        }
                    }
                }
            }

        }

        // Import data **************************************************************************************************************

        public void ImportData(Ktt3DbContext dbContext, bool deleteBeforeImport)
        {
            // ¿Allow inserts with the ID that comes in file? > NO > I'm almost sure that don't do it is better, because allows me to insert duplicates and check/compare later
            string inputfile = _OutputFileName;
            string[] lines = System.IO.File.ReadAllLines(inputfile);
            string[] fields;
            int fieldCount;
            bool hasJobs;
            bool hasTimeTracks;

            using (DbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                string _Name;
                int _currentLineJobID;
                string _Status;
                int _Priority;
                string _Description;
                int? _EstimatedTime;
                int _currentLineTimeTrackID;
                DateTime _WorkDate;
                DateTime _StartTime;
                DateTime? _EndTime;
                int line = 0;
                Project currentProject = null;
                int LastProjectId = -1;
                int _currentLineProjectId;
                int LastJobId = -1;
                int LastTimeTrackId = -1;
                try
                {
                    do
                    {
                        // skip headers & empty ones
                        if (string.IsNullOrWhiteSpace(lines[line]))
                        {
                            line++;
                            continue;
                        }
                        if (lines[line].StartsWith("ProjectID"))
                        {
                            line++;
                            continue;
                        }

                        // split line into data
                        fields = lines[line].Split(_DataSep[0]);
                        fieldCount = fields.Length;
                        hasJobs = fieldCount > 2;
                        hasTimeTracks = fieldCount > 7;

                        // pickup project data from line
                        _currentLineProjectId = Convert.ToInt32(fields[0]);
                        _Name = fields[1];

                        if (LastProjectId != _currentLineProjectId)
                        {
                            LastProjectId = _currentLineProjectId;
                            // create the project
                            currentProject = new Project()
                            {
                                //*ProjectID = _currentLineProjectId, 
                                Name = _Name
                            };
                            // Add the project
                            if (deleteBeforeImport)
                            {
                                var l = dbContext.Projects.Where(p => p.ProjectID == _currentLineProjectId).ToList();
                                if (l.Count > 0)
                                {
                                    dbContext.Projects.Remove(l[0]);
                                    dbContext.SaveChanges();
                                }
                            }
                            dbContext.Projects.Add(currentProject);
                        }

                        if (hasJobs)
                        {
                            _currentLineJobID = Convert.ToInt32(fields[2]);
                            if (LastJobId != _currentLineJobID)
                            {
                                LastJobId = _currentLineJobID;
                                _Priority = Convert.ToInt32(fields[3]);
                                _Status = fields[4] != "null" ? _Status = fields[4] : null;
                                _Description = fields[6];
                                _EstimatedTime = fields[5] != "null" ? Convert.ToInt32(fields[5]) : (int?)null;
                                // Create the job
                                Job currentJob = new Job()
                                {
                                    //*ProjectID = LastProjectId, 
                                    JobID = _currentLineJobID,
                                    Priority = _Priority, 
                                    Status = _Status,
                                    Description = _Description,
                                    EstimatedTime = _EstimatedTime
                                };
                                // Add the job
                                if (currentProject.Jobs == null)
                                    currentProject.Jobs = new System.Collections.ObjectModel.ObservableCollection<Job>();
                                currentProject.Jobs.Add(currentJob);
                            }
                        }

                        if (hasTimeTracks)
                        {
                            _currentLineTimeTrackID = Convert.ToInt32(fields[7]);
                            if (LastTimeTrackId != _currentLineTimeTrackID)
                            {
                                LastTimeTrackId = _currentLineTimeTrackID;
                                // create time track
                                _currentLineTimeTrackID = Convert.ToInt32(fields[7]);
                                _WorkDate = DateTime.Parse(fields[8]);
                                _StartTime = DateTime.Parse(fields[9]);
                                _EndTime = fields[10] == "null" ? null : (DateTime?)DateTime.Parse(fields[10]);
                                TimeTrack currentTimeTrack = new TimeTrack()
                                {
                                    //*TimeTrackID = _currentLineTimeTrackID, 
                                    WorkDate = _WorkDate,
                                    StartTime = _StartTime,
                                    EndTime = _EndTime
                                };
                                // add time track
                                if (currentProject.Jobs[currentProject.Jobs.Count - 1].TimeTracks == null)
                                    currentProject.Jobs[currentProject.Jobs.Count - 1].TimeTracks = new System.Collections.ObjectModel.ObservableCollection<TimeTrack>();
                                currentProject.Jobs[currentProject.Jobs.Count - 1].TimeTracks.Add(currentTimeTrack);
                            }
                        }

                        line++;
                    } while (line < lines.Length);

                    dbContext.SaveChanges();
                    transaction.Commit();

                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    DTrace.Trace(exception.Message);
                    //throw;
                }
            }
        }

    }
}
