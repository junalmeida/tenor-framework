using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace TenorTemplate
{
    public partial class frmMain : Form
    {
        private class DriverData
        {
            public DriverData(MyMeta.dbDriver driver, string name, string conn)
            {
                this.driver = driver;
                this.name = name;
                this.conn = conn;
            }
            public MyMeta.dbDriver driver;
            public string name;
            public string conn;

            public override string ToString()
            {
                return name;
            }
        }
        private DriverData[] CustomConnections 
        {
            get
            {
                List<DriverData> list = new List<DriverData>();
                list.Add(new DriverData(MyMeta.dbDriver.SQL, "Microsoft Sql Server", "Provider=SQLNCLI.1;Data Source={0};Initial Catalog={1};User Id={2};Password={3};"));
                /*
                list.Add(MyMeta.dbDriver.Oracle, "Oracle");
                list.Add(MyMeta.dbDriver.MySql2, "MySql");
                list.Add(MyMeta.dbDriver.PostgreSQL8, "PostgreSql");
                list.Add(MyMeta.dbDriver.Firebird, "Firebird");
                list.Add(MyMeta.dbDriver.Interbase, "Interbase");
                list.Add(MyMeta.dbDriver.SQLite, "SQLite");
                list.Add(MyMeta.dbDriver.Access, "Microsoft Access");
                */
                return list.ToArray();
            }
        }

        public frmMain()
        {
            InitializeComponent();

            lblVersion.Text = string.Format(lblVersion.Text, this.GetType().Assembly.GetName().Version.ToString());
            steps = new GroupBox[] { grpStep1, grpStep2, grpStep3, grpStep4 };
            Control.CheckForIllegalCrossThreadCalls = false;
            LoadCurrentStep();
        }

        #region Steps

        private GroupBox[] steps;
        private int activeStep = 0;

        private void PreviousStep()
        {
            activeStep -= 1;
            if (activeStep < 0)
                activeStep = 0;
            LoadCurrentStep();
        }

        private void NextStep()
        {
            activeStep += 1;
            if (activeStep > steps.Length - 1)
                activeStep = steps.Length - 1;
            LoadCurrentStep();
        }

        private void LoadCurrentStep()
        {
            for (int i = 0; i < steps.Length; i++ )
            {
                bool visible = (i == activeStep);
                steps[i].Location = steps[0].Location;
                steps[i].Size = steps[0].Size;
                steps[i].Visible = visible;
                if (visible)
                {
                    lblStep.Text = steps[i].Tag.ToString();
                    if (i == 0) LoadStep1();
                    else if (i == 1) LoadStep2();
                    else if (i == 2) LoadStep3();
                    else if (i == 3) LoadStep4();
                }
            }
            btnBack.Enabled = activeStep > 0;
            btnNext.Text = (activeStep >= steps.Length - 2 ? "&Generate" : "&Next >");
            btnNext.Enabled = (activeStep < steps.Length - 1) ;
        }
        #endregion


        #region Step 1 - Database
        private void LoadStep1()
        {
            try
            {
                cmbProvider.DataSource = CustomConnections;
                cmbProvider.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBoxError("Cannot list your providers.\r\n" + ex.Message);
            }
        }

        private string connTemplate;
        private void cmbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProvider.SelectedItem == null)
                connTemplate = null;
            else
                connTemplate = ((DriverData)cmbProvider.SelectedItem).conn;
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            if (connTemplate != null)
                txtConnection.Text = string.Format(connTemplate, txtServer.Text, txtCatalog.Text, txtUsername.Text, txtPassword.Text);

        }

 
        private bool ValidateStep1()
        {
            if (cmbProvider.SelectedIndex == -1)
            {
                MessageBoxError("Please select a provider.");
                return false;
            }
            DriverData driver = (DriverData)cmbProvider.SelectedItem;
            
            IDbConnection connection = null;
            try
            {
                connection = new MyMeta.dbRoot().BuildConnection(driver.driver.ToString(), txtConnection.Text);
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxError("Could not open a connection with the specified settings.\r\nCheck your settings and network problems and try again.\r\n\r\n" + ex.Message);
                return false;
            }
            finally
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }

        }


        #endregion

        #region Step 2 - Select tables
        MyMeta.dbRoot myMeta;
        private void LoadStep2()
        {
            try
            {
                if (myMeta != null)
                    myMeta.Dispose();

                myMeta = new MyMeta.dbRoot();
                if (myMeta.Connect(((DriverData)cmbProvider.SelectedItem).driver, txtConnection.Text))
                {
                    tree.Nodes.Clear();
                    
                    foreach (MyMeta.Database db in myMeta.Databases)
                    {
                        TreeNode tn = new TreeNode();
                        tn.Text = db.Name;
                        tn.Name = db.Name;
                        //tn.ImageKey = "database";
                        tree.Nodes.Add(tn);
                        HideTreeNodeCheckBox(tn);
                        LoadTypes(tn, db);
                    }
                }
                else
                {
                    throw new ApplicationException("Cannot connect to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBoxError("Could not load database tables and views.\r\n\r\n" + ex.Message);
                PreviousStep();
            }
        }

        private void LoadTypes(TreeNode root, MyMeta.Database db)
        {
            TreeNode tn = new TreeNode();
            //-- Tables
            tn.Name = "Tables";
            tn.Text = "Tables";
            //tn.ImageKey = "tablesCollection";
            root.Nodes.Add(tn);
            HideTreeNodeCheckBox(tn);
            LoadTables(tn, db);

            //-- Views
            tn = new TreeNode();
            tn.Name = "Views";
            tn.Text = "Views";
            //tn.ImageKey = "viewsCollection";
            root.Nodes.Add(tn);
            HideTreeNodeCheckBox(tn);
            LoadViews(tn, db);
        }

        private void LoadTables(TreeNode root, MyMeta.Database db)
        {
            foreach (MyMeta.Table table in db.Tables)
            {
                TreeNode schema = null;
                if (root.Nodes.ContainsKey(table.Schema))
                    schema = root.Nodes[table.Schema];
                else
                {
                    schema = new TreeNode();
                    schema.Text = table.Schema;
                    schema.Name = table.Schema;
                    // schema.ImageKey = "schema";
                    root.Nodes.Add(schema);
                    HideTreeNodeCheckBox(schema);
                }


                TreeNode tn = new TreeNode();
                tn.Text = table.Name;
                tn.Name = table.Name;
                //tn.ImageKey = "table";

                schema.Nodes.Add(tn);
            }
        }

        private void LoadViews(TreeNode root, MyMeta.Database db)
        {
            foreach (MyMeta.View view in db.Views)
            {
                TreeNode schema = null;
                if (root.Nodes.ContainsKey(view.Schema))
                    schema = root.Nodes[view.Schema];
                else
                {
                    schema = new TreeNode();
                    schema.Text = view.Schema;
                    schema.Name = view.Schema;
                    // schema.ImageKey = "schema";
                    root.Nodes.Add(schema);
                    HideTreeNodeCheckBox(schema);
                }


                TreeNode tn = new TreeNode();
                tn.Text = view.Name;
                tn.Name = view.Name;
                //tn.ImageKey = "view";

                schema.Nodes.Add(tn);
            }
        }

        private bool ValidateStep2() { return true; }
        #endregion

        #region Step 3 - Choose target
        private void LoadStep3()
        {
        }

        private bool ValidateStep3()
        {
            return true;
        }
        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            folderBrowser.SelectedPath = txtTargetFolder.Text;
            if (folderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                txtTargetFolder.Text = folderBrowser.SelectedPath;
            }
        }
        


        #endregion

        #region Step 4 - Ready to rumble
        private void LoadStep4()
        {
            //Can start generating files.
            this.Enabled = true;
            progressBar.Value = 0;
            listProgress.Items.Clear();
            listProgress.Items.Add("Generating classes...");

            for (int i = 0; i <= 100; i++)
            {
                if (i % 4 == 0)
                {
                    listProgress.Items.Add("Generating " + i.ToString());
                    listProgress.SetSelected(listProgress.Items.Count - 1, true);
                }
                progressBar.Value = i;
            }

            listProgress.Items.Add("Process complete. You can close this wizard.");
            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.UseShellExecute = true;
            pInfo.FileName = txtTargetFolder.Text;
            Process.Start(pInfo);
            Application.DoEvents();
            btnBack.Enabled = false;
        }
        #endregion

        #region Navigation Control
        private void btnNext_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            this.Enabled = false;
            Application.DoEvents();

            bool canGo = false;
            if (activeStep == 0)
                canGo = ValidateStep1();
            else if (activeStep == 1)
                canGo = ValidateStep2();
            else if (activeStep == 2)
                canGo = ValidateStep3();

            //method.BeginInvoke(new AsyncCallback(NextCallback), method);
            NextFinish(canGo);
        }

        //private void NextCallback(IAsyncResult ar)
        //{
        //    dValidate method = (dValidate)ar.AsyncState;
        //    bool canGo = method.EndInvoke(ar);
        //    method = null; ar = null;
        //    NextFinish(canGo);
        //}

        private void NextFinish(bool canGo)
        { 
            if (canGo)
                NextStep();

            this.UseWaitCursor = false;
            this.Enabled = true;
            Application.DoEvents();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            PreviousStep();
        }
        #endregion

        private void MessageBoxError(string message)
        {
            MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void MessageBoxAlert(string message)
        {
            MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void tree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            SetNodeCheck(e.Node);
        }

        private void SetNodeCheck(TreeNode treeNode)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = treeNode.Checked;
                SetNodeCheck(node);
            }
        }



        #region PInvoke
        private const int TVIF_STATE = 0x8;
        private const int TVIS_STATEIMAGEMASK = 0xF000;
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETITEM = TV_FIRST + 63;

        private struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public String lpszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;

        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam,
        IntPtr lParam);

        private void HideTreeNodeCheckBox(TreeNode node)
        {
            TVITEM tvi = new TVITEM();
            tvi.hItem = node.Handle;
            tvi.mask = TVIF_STATE;
            tvi.stateMask = TVIS_STATEIMAGEMASK;
            tvi.state = 0;
            IntPtr lparam = Marshal.AllocHGlobal(Marshal.SizeOf(tvi));
            Marshal.StructureToPtr(tvi, lparam, false);
            SendMessage(node.TreeView.Handle, TVM_SETITEM, IntPtr.Zero, lparam);
        }

        #endregion

 




    }
}
