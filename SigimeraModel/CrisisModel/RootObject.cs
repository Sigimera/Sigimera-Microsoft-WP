using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace SigimeraModel.CrisisModel
{
    [Table(Name = "CrisisItemViewModel")]
    public class RootObject : INotifyPropertyChanged
    {

        public RootObject()
        {
            
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int CrisisId
        {
            get;
            set;
        }

        private string __id;

        [Column(CanBeNull = false)]
        public string _id
        {
            get
            {
                return __id;
            }
            set
            {
                if (value != __id)
                {
                    __id = value;
                    NotifyPropertyChanged("_id");
                }
            }
        }

        private string _dc_title;
        [Column(CanBeNull = false)]
        public string dc_title
        {
            get
            {
                return _dc_title;
            }
            set
            {
                if (value != _dc_title)
                {
                    _dc_title = value;
                    NotifyPropertyChanged("dc_title");
                }
            }
        }

        private string _dc_description;
        [Column(CanBeNull = false)]
        public string dc_description
        {
            get
            {
                return _dc_description;
            }
            set
            {
                if (value != _dc_description)
                {
                    _dc_description = value;
                    NotifyPropertyChanged("dc_description");
                }
            }
        }

        private string _subject;
        [Column(CanBeNull = false)]
        public string subject
        {
            get
            {
                return _subject;
            }
            set
            {
                if (value != _subject)
                {
                    _subject = value;
                    NotifyPropertyChanged("subject");
                }
            }
        }

        private List<double> _foaf_based_near;
        public List<double> foaf_based_near
        {
            get
            {
                return _foaf_based_near;
            }
            set
            {
                if (value != _foaf_based_near)
                {
                    _foaf_based_near = value;
                    NotifyPropertyChanged("foaf_based_near");
                }
            }
        }

        private List<string> _gn_parentCountry;
        public List<string> gn_parentCountry
        {
            get
            {
                return _gn_parentCountry;
            }
            set
            {
                if (value != _gn_parentCountry)
                {
                    _gn_parentCountry = value;
                    NotifyPropertyChanged("gn_parentCountry");
                }
            }
        }

        private string _crisis_alertLevel;
        [Column(CanBeNull = true)]
        public string crisis_alertLevel
        {
            get
            {
                return _crisis_alertLevel;
            }
            set
            {
                if (value != _crisis_alertLevel)
                {
                    _crisis_alertLevel = value;
                    NotifyPropertyChanged("crisis_alertLevel");
                }
            }
        }

        private string _crisis_severity;
        [Column(CanBeNull = true)]
        public string crisis_severity
        {
            get
            {
                return _crisis_severity;
            }
            set
            {
                if (value != _crisis_severity)
                {
                    _crisis_severity = value;
                    NotifyPropertyChanged("crisis_severity");
                }
            }
        }

        private string _crisis_population;
        [Column(CanBeNull = true)]
        public string crisis_population
        {
            get
            {
                return _crisis_population;
            }
            set
            {
                if (value != _crisis_population)
                {
                    _crisis_population = value;
                    NotifyPropertyChanged("crisis_population");
                }
            }
        }

        private string _crisis_vulnerability;
        [Column(CanBeNull = true)]
        public string crisis_vulnerability
        {
            get
            {
                return _crisis_vulnerability;
            }
            set
            {
                if (value != _crisis_vulnerability)
                {
                    _crisis_vulnerability = value;
                    NotifyPropertyChanged("crisis_vulnerability");
                }
            }
        }

        private string _dc_date;
        [Column(CanBeNull = true)]
        public string dc_date
        {
            get
            {
                return _dc_date;
            }
            set
            {
                if (value != _dc_date)
                {
                    _dc_date = value;
                    NotifyPropertyChanged("dc_date");
                }
            }
        }

        private string _schema_startDate;
        [Column(CanBeNull = true)]
        public string schema_startDate
        {
            get
            {
                return _schema_startDate;
            }
            set
            {
                if (value != _schema_startDate)
                {
                    _schema_startDate = value;
                    NotifyPropertyChanged("schema_startDate");
                }
            }
        }

        private string _schema_endDate;
        [Column(CanBeNull = true)]
        public string schema_endDate
        {
            get
            {
                return _schema_endDate;
            }
            set
            {
                if (value != _schema_endDate)
                {
                    _schema_endDate = value;
                    NotifyPropertyChanged("schema_endDate");
                }
            }
        }

        public string ImageUrl
        {
            get
            {
                if(string.IsNullOrEmpty(_subject))
                {
                    return string.Empty;
                }
                else if (_subject.ToLower() == "earthquake")
                {
                    return "Images/Entities/Earthquake.png";
                }
                else if (_subject.ToLower() == "cyclone")
                {
                    return "Images/Entities/Cyclone.png";
                }
                else if (_subject.ToLower() == "flood")
                {
                    return "Images/Entities/Flood.png";
                }
                else
                {
                    return "Images/Entities/Volcano.png";
                }
            }
            set
            {
                if (value != _schema_endDate)
                {
                    _schema_endDate = value;
                    NotifyPropertyChanged("schema_endDate");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}