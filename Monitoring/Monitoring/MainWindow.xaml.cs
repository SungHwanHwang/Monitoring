using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data.Common;
using System.Windows.Threading;
using System.Threading;


namespace Monitoring
{
    public partial class MainWindow : Window
    {
        public MainWindow(){ InitializeComponent(); }

        //  중복 실행 방지 변수
        Mutex mutex = null;
        
        //  해당 DB에 접속할 타이머(주기) 설정, 주기마다 접속
        DispatcherTimer timer;

        //  Notify 알림창 오픈 여부를 선택하여 3초마다 오픈해주는 타이머
        bool isTimer = false;
        DispatcherTimer notifyTimer;

        //  TrayIcon 추가 변수
        System.Windows.Forms.NotifyIcon notify;

        //  TrayIcon을 깜박임 제어 스위치 변수
        bool trayIconFlag = false;

        //  Thread 동작 여부를 확인하기위한(현재 Thread 동작 중인지)변수
        bool isThreading = false;

        //  배경색 변경Thread 변수
        ThreadStart backGroundThreadStart;
        Thread backGroundThread;

        //  트레이 아이콘 Thread 변수
        ThreadStart trayIconThreadStart;
        Thread trayIconThread;

        //  코드로 배경색 변경을 하기 위한 Brush 변수
        BrushConverter backGroundBrush = new BrushConverter();

        //  배경색 변경을 반복 적용하기 위한 Switch 함수를 만들기 위한 변수
        private volatile bool shouldStop;


        //  배경색 변경 시작, 중지 함수
        private void RequestStop()
        {
            shouldStop = true;
        }

        private void RequestStart()
        {
            shouldStop = false;
        }

        //  실제로 실행되는(주기마다) 화면에서 실행할 함수
        private void Start_Program(object sender, EventArgs e)
        {
            //  중복실행 방지는 화면에서 실행할 함수에 위치시켜 실행
            Duplicate_Execution("MainWindow");

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += new EventHandler(Etl_load);
            timer.Start();
            Window_Loaded();
        }

        //  해당 창 중복 실행 방지 함수
        private void Duplicate_Execution(string mutexName)
        {
            try
            {
                mutex = new Mutex(false, mutexName);
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message + "\n\n" + err.StackTrace + "\n\n" + "Application Existing...", "Exception thrown");
                Application.Current.Shutdown();
            }

            if(mutex.WaitOne(0, false))
            {
                InitializeComponent();
            }
            else
            {
                MessageBox.Show("Application Already Started", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                Application.Current.Shutdown();
            }
        }

        //  TrayIcon 클릭 시 발생하는 이벤트(TrayIcon 우클릭시 나오는 메뉴 정의)
        private void Window_Loaded()
        {
            try
            {

                System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();
                System.Windows.Forms.MenuItem item1 = new System.Windows.Forms.MenuItem();
                System.Windows.Forms.MenuItem item2 = new System.Windows.Forms.MenuItem();

                menu.MenuItems.Add(item1);
                menu.MenuItems.Add(item2);
                    
                item1.Index = 0;
                item1.Text = "프로그램 종료";
                item1.Click += delegate(object click, EventArgs eclick)
                {
                    this.Close();
                };

                item2.Index = 0;
                item2.Text = "프로그램 설정";
                item2.Click += delegate(object click, EventArgs eclick)
                {
                    this.Close();
                };

                notify = new System.Windows.Forms.NotifyIcon();
                notify.Icon = Properties.Resources.Icon1;
                notify.DoubleClick += delegate(object senders, EventArgs args)
                {
                    this.Show();
                   this.WindowState = WindowState.Normal;
                };

                notify.ContextMenu = menu;
                notify.Text = "Test";
            }
            catch(Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void CheckTimer(int isdata)
        {
            notifyTimer = new DispatcherTimer();

            if(isdata == 0)
            {
                notify.Visible = true;
                notifyTimer.Tick += new EventHandler(Timer_Tick);
                notifyTimer.Interval = new TimeSpan(0, 0, 3);

                if(isTimer == false)
                {
                    notifyTimer.Start();
                    isTimer = true;
                }
            }
            else if(isdata == 1)
            {
                isTimer = false;
                notifyTimer.Stop();
                notify.Visible = false;
            }
        }

        //  일정 시간을 주기로 반복하여 Ballon Tip을 띄우는 함수
        private void Timer_Tick(object sender, EventArgs e)
        {
            notify.BalloonTipTitle = "체크 바람";
            notify.BalloonTipText = "Error 처리 상황 체크";
            notify.ShowBalloonTip(3000);
        }

        //  일정 시간을 주기로 TrayIcon을 반복적으로 변경하는 함수
        private void ChangeTrayIcon()
        {
            try
            {
                while(!shouldStop)
                {
                    if(trayIconFlag == false)
                    {
                        this.Dispatcher.Invoke(delegate()
                        {
                            notify.Icon = Properties.Resources.Icon1;
                        });
                        trayIconFlag = true;
                    }
                    else if(trayIconFlag == true)
                    {
                        this.Dispatcher.Invoke(delegate()
                        {
                            notify.Icon = Properties.Resources.Icon2;
                        });
                        trayIconFlag = false;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                return;
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if(WindowState.Minimized.Equals(WindowState))
            {
                this.Hide();
            }
            base.OnStateChanged(e);
        }

        //  Text 파일에서 DB접속 정보 읽기
        private string GetFileData()
        {
            int counter = 0;
            string line;

            string connectInfo = string.Empty;

            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(@"C:\ConnectInfo.txt");

                while((line = file.ReadLine())!=null)
                {
                    System.Console.WriteLine(line);
                    connectInfo += line + ';';
                    counter++;
                }

                connectInfo = connectInfo.Substring(0, connectInfo.Length - 1);
                System.Console.WriteLine("1차 완성된 정보 : " + connectInfo);

            }
            catch(Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
                return connectInfo;            
        }
  

        //  배경색, 탭, 헤더 등 색 변경을 위한 함수 모음
        private void ConvertColor(string color)
        {
            mainGrid.Background = (Brush)backGroundBrush.ConvertFrom(color);
            innerGrid.Background = (Brush)backGroundBrush.ConvertFrom(color);
            listView.Background = (Brush)backGroundBrush.ConvertFrom(color);
            listView.RowBackground = (Brush)backGroundBrush.ConvertFrom(color);

            Style style = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
            style.Setters.Add(new Setter { Property = BackgroundProperty, Value = (Brush)backGroundBrush.ConvertFrom(color) });

            for(int i=0;i<6;i++)
            {
                listView.Columns[i].HeaderStyle = style;
            }

            //  탭 색깔 변경하기 힘드니 잘 기억 혹은 메모
            ControlTemplate ctl2 = this.header_Two.Template;

            //  위 ControlTemplate을 활용하여 TabItem의 색을 변경하는 함수 실행
            ControlTemplateColorChange(ctl2, this.header_Two, color);
        }

        //  ControlTemplate을 활용하여 TabItem의 색을 변경하는 함수 선언 부분
        private void ControlTemplateColorChange(ControlTemplate ctl, TabItem item, string color)
        {
            ctl = item.Template;
            Border border = ctl.FindName("colorBorder", item) as Border;
            border.Background = (Brush)backGroundBrush.ConvertFrom(color);
        }

        private void ChangeBackGroundColor()
        {
            bool flag = true;

            try
            {
                while(!shouldStop)
                {
                    if(flag == true)
                    {
                        this.Dispatcher.Invoke(delegate()
                        {
                            ConvertColor("RED");
                        });

                        flag = false;
                    }
                    else if(flag == false)
                    {
                        this.Dispatcher.Invoke(delegate()
                        {
                            ConvertColor("YELLOW");
                        });
                        flag = true;
                    }
                    Thread.Sleep(1000);
                }
                this.Dispatcher.Invoke(delegate()
                {
                    ConvertColor("WHITE");
                });
            }
            catch(Exception e)
            {
                return;
            }
        }

        //  Oracle DB Null 입력시 "No Data"로 처리해주는 함수
        private string GetString(OracleDataReader reader, int idx, string defaultStr)
        {
            if(reader == null)
            {
                return defaultStr;
            }
            if(reader.IsDBNull(idx) == true)
            {
                return defaultStr;
            }
            return reader.GetString(idx);
        }



        //  주기마다 DB를 로드하여 데이터를 가져와 Thread를 시작할 지 말지를 결정 내리는 함수
        private void Etl_load(object sender, EventArgs e)
        {
            //  배경화면 Thread
            backGroundThreadStart = new ThreadStart(ChangeBackGroundColor);
            backGroundThread = new Thread(backGroundThreadStart);

            //  TrayIcon Thread
            trayIconThreadStart = new ThreadStart(ChangeTrayIcon);
            trayIconThread = new Thread(trayIconThreadStart);

            string connect_info = GetFileData();
            try
            {
                OracleConnection conn = new OracleConnection(connect_info);
                OracleCommand cmd = new OracleCommand();
                conn.Open();
                cmd.Connection = conn;

                string SQL = "SELECT TARGET_TABLE_NAME, ETL_PROCESS_SEG, START_DATE, FINISH_DATE, ERROR_STATUS, ERROR_MESSAGE FROM ETL_JOB_LOG";
                cmd.CommandText = SQL;
                OracleDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                listView.ClearValue(ItemsControl.ItemsSourceProperty);
                List<ListEtlVO> etls = new List<ListEtlVO>();

                while(true)
                {
                    if(reader.Read() == false)
                    {
                        break;
                    }

                    etls.Add(new ListEtlVO()
                    {
                        Target_table_name = reader.GetString(0),
                        Etl_process_seg = reader.GetString(1),
                        Start_date = reader.GetDateTime(2),
                        Finish_date = reader.GetDateTime(3),
                        Error_status = GetString(reader, 4, "No Data"),
                        Error_message = GetString(reader, 5, "No Data")
                    });
                }
                listView.ItemsSource = etls;

                int isData = 1;
                foreach(ListEtlVO vo in listView.ItemsSource)
                {
                    if(vo.Error_status == "No Data")
                    {
                        isData = 0;
                        break;
                    }
                }

                if(isData == 0)
                {
                    Console.WriteLine(backGroundThread.ThreadState.ToString());
                    MessageBox.Show("스레드 실행여부(0 : 실행, 1 : 종료 = " + isData);

                    CheckTimer(isData);
                    if(isThreading == false)
                    {
                        RequestStart();
                        backGroundThread.Start();
                        trayIconThread.Start();

                        isThreading = true;
                    }
                }
                else if(isData == 1)
                {
                    try
                    {
                        isThreading = false;
                        CheckTimer(isData);
                        RequestStop();
                        backGroundThread.Join();
                        trayIconThread.Join();
                    }
                    catch(Exception err)
                    {
                        MessageBox.Show(err.ToString());
                    }
                }
                conn.Close();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
           
        }
    
       
    }
}
