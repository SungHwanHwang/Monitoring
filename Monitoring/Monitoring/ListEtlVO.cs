using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace Monitoring
{
    public class ListEtlVO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string target_table_name = string.Empty;
        private string etl_process_seg = string.Empty;
        private DateTime start_date;
        private DateTime finish_date;
        private string error_status = string.Empty;
        private string error_message = string.Empty;


        public string Target_table_name
        {
            get { return target_table_name; }
            set { this.target_table_name = value; OnPropertyChanged("target_table_name"); }
        }

        public string Etl_process_seg
        {
            get { return etl_process_seg; }
            set { this.etl_process_seg = value; OnPropertyChanged("etl_process_seg"); }
        }

        public DateTime Start_date
        {
            get { return start_date; }
            set { this.start_date = value; OnPropertyChanged("start_date"); }
        }
   
        public DateTime Finish_date
        {
            get { return finish_date; }
            set { this.finish_date = value; OnPropertyChanged("finish_date"); }
        }

        public string Error_status
        {
            get { return error_status; }
            set { this.error_status = value; OnPropertyChanged("error_status"); }
        }

        public string Error_message
        {
            get { return error_message; }
            set { this.error_message = value; OnPropertyChanged("error_message"); }
        }
 
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
