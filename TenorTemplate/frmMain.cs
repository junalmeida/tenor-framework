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
using MyMeta;


namespace TenorTemplate
{
    public partial class frmMain : Form
    {


        public frmMain()
        {
            InitializeComponent();

            lblVersion.Text = string.Format(lblVersion.Text, this.GetType().Assembly.GetName().Version.ToString());
            steps = new GroupBox[] { grpStep1, grpStep2, grpStep3, grpStep4 };
            Control.CheckForIllegalCrossThreadCalls = false;
            LoadCurrentStep();
        }

        public frmMain(MyMeta.dbRoot myMeta)
            : this()
        {
            this.myMeta = myMeta;
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
        Settings config;
        public Settings Config
        {
            get
            {
                if (config == null)
                    config = Settings.Load();
                return config;
            }
        }

        private void LoadStep1()
        {
            try
            {
                Program.DriverData[] drivers = Program.CustomConnections;
                cmbProvider.DataSource = drivers;
                cmbProvider.SelectedIndex = -1;
                if (Config.Driver.HasValue)
                    foreach (Program.DriverData driver in drivers)
                    {
                        if (driver.driver == Config.Driver.Value)
                            cmbProvider.SelectedIndex = Array.IndexOf(drivers, driver);
                    }
                txtCatalog.Text = Config.Catalog;
                txtConnection.Text = Config.ConnectionString;
                txtServer.Text = Config.Server;
                txtUsername.Text = Config.UserName;
                txtPassword.Text = Config.Password;

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
                connTemplate = ((Program.DriverData)cmbProvider.SelectedItem).conn;
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
            Program.DriverData driver = (Program.DriverData)cmbProvider.SelectedItem;
            
            IDbConnection connection = null;
            try
            {
                connection = new MyMeta.dbRoot().BuildConnection(driver.driver.ToString(), txtConnection.Text);
                connection.Open();

                Config.Driver = driver.driver;

                Config.Catalog = txtCatalog.Text;
                Config.ConnectionString = txtConnection.Text;
                Config.Server = txtServer.Text;
                Config.UserName = txtUsername.Text;
                Config.Password = txtPassword.Text;
                Config.Save();
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
                if (myMeta == null)
                    myMeta = new MyMeta.dbRoot();

                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                if (myMeta.Connect(((Program.DriverData)cmbProvider.SelectedItem).driver, txtConnection.Text))
                {
                    tree.Nodes.Clear();

                    for (int i = 0; i < myMeta.Databases.Count; i++)
                    {
                        IDatabase db = myMeta.Databases[i];
                        TreeNode tn = new TreeNode();
                        tn.Name = i.ToString();
                        tn.Text = db.Name;
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
            finally
            {
                Cursor.Current = Cursors.Default;
                Application.DoEvents();
            }
        }

        private void LoadTypes(TreeNode root, IDatabase db)
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

        private void LoadTables(TreeNode root, IDatabase db)
        {
            for (int i = 0; i < db.Tables.Count; i++)
            {
                ITable table = db.Tables[i];
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
                tn.Name = i.ToString();
                //tn.ImageKey = "table";

                schema.Nodes.Add(tn);
            }
        }

        private void LoadViews(TreeNode root, IDatabase db)
        {
            for (int i = 0; i < db.Views.Count;i++ )
            {
                IView view = db.Views[i];
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
                tn.Name = i.ToString();
                //tn.ImageKey = "view";

                schema.Nodes.Add(tn);
            }
        }

        private bool ValidateStep2() { return true; }
        #endregion

        #region Step 3 - Choose target
        private void LoadStep3()
        {
            cmbLanguage.Items.Clear();
            cmbLanguage.Items.Add(Language.CSharp);
            cmbLanguage.Items.Add(Language.VBNet);

            cmbLanguage.SelectedItem = Config.Language;
            txtTargetFolder.Text = Config.Directory;
            txtNamespace.Text = Config.BaseNamespace;
        }

        private bool ValidateStep3()
        {
            if (cmbLanguage.SelectedIndex == -1)
            {
                this.MessageBoxError("Select a language.");
                return false;
            }
            else if (string.IsNullOrEmpty(txtNamespace.Text))
            {
                this.MessageBoxError("Type a base namespace.");
                return false;
            }
            else if (string.IsNullOrEmpty(txtTargetFolder.Text) || !System.IO.Directory.Exists(txtTargetFolder.Text))
            {
                this.MessageBoxError("Select a valid target folder.");
                return false;
            }
            else
            {
                Config.Language = (Language)cmbLanguage.SelectedItem;
                Config.Directory = txtTargetFolder.Text;
                Config.BaseNamespace = txtNamespace.Text;
                Config.Save();
                return true;
            }
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
        private TreeNode[] GetCheckedNodes()
        {
            return GetCheckedNodes(tree.Nodes);
        }

        private TreeNode[] GetCheckedNodes(TreeNodeCollection treeNodeCollection)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode node in treeNodeCollection)
            {
                if (node.Checked)
                    nodes.Add(node);
                nodes.AddRange(GetCheckedNodes(node.Nodes));
            }
            return nodes.ToArray();
        }

        private MyMeta.Single GetItem(TreeNode node)
        {
            bool isTable = node.Parent.Parent.Name == "Tables";
            bool isView = node.Parent.Parent.Name == "Views";
            //there can be more types here.
            int database = int.Parse(node.Parent.Parent.Parent.Name);

            if (isTable)
            {
                int table = int.Parse(node.Name);
                return (MyMeta.Single)myMeta.Databases[database].Tables[table];
            }
            else if (isView)
            {
                int view = int.Parse(node.Name);
                return (MyMeta.Single)myMeta.Databases[database].Views[view];
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void LoadStep4()
        {
            
            //Can start generating files.
            this.Enabled = true;
            progressBar.Value = 0;
            
            txtProgress.Text = "Generating classes...";
            Application.DoEvents();
            TreeNode[] nodes = GetCheckedNodes();
            progressBar.Maximum = nodes.Length -1;

            Language language = (Language)cmbLanguage.SelectedIndex;
            switch (language)
            {
                case Language.CSharp:
                    myMeta.Language = "C#";
                    break;
                case Language.VBNet:
                    myMeta.Language = "VB.NET";
                    break;
                default:
                    break;
            }


            for (int i = 0; i < nodes.Length; i++)
            {
                MyMeta.Single item = GetItem(nodes[i]);
                txtProgress.Text += "\r\nGenerating " + item.Name  + "... ";
                int index = txtProgress.Lines.Length -1;
                txtProgress.SelectionStart = txtProgress.Text.LastIndexOf("\r\n");
                txtProgress.ScrollToCaret();

                progressBar.Value = i;

                Application.DoEvents();
                string result = null;
                try
                {
                    Template t = new Template(language, txtNamespace.Text, item);
                    t.Generate(txtTargetFolder.Text);
                    result = "Done.";
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
                txtProgress.Text += result;
            }

            txtProgress.Text += "\r\nProcess complete. You can close this wizard.";
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
            //this.Enabled = false;
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
#pragma warning disable
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
#pragma warning restore

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
