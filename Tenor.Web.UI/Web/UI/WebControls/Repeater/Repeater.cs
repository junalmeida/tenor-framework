using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Tenor.Web.UI.WebControls
{

    /// <summary>
    /// Usado no evento de Paginação do Repeater
    /// </summary>
    /// <remarks></remarks>
    public class RepeaterPageEventArgs : GridViewPageEventArgs
    {


        public RepeaterPageEventArgs(int newPageIndex)
            : base(newPageIndex)
        {

        }
    }
    /// <summary>
    /// Usada no evento de Sort do Repeater.
    /// </summary>
    /// <remarks></remarks>
    public class RepeaterSortEventArgs : GridViewSortEventArgs
    {


        public RepeaterSortEventArgs(string sortExpression, SortDirection sortDirection)
            : base(sortExpression, sortDirection)
        {

        }
    }

    public delegate void RepeaterPageEventHandler(object sender, RepeaterPageEventArgs e);
    public delegate void RepeaterSortEventHandler(object sender, RepeaterSortEventArgs e);


    /// <summary>
    /// Implementa um Repeater com funções de paginação.
    /// </summary>
    /// <remarks></remarks>
    [
    PersistChildrenAttribute(false), 
    DefaultPropertyAttribute("DataSource"),
    DefaultEventAttribute("ItemCommand"),
    DesignerAttribute("System.Web.UI.Design.WebControls.RepeaterDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
    ParseChildrenAttribute(true),
    ToolboxData("<{0}:Repeater runat=server><ItemTemplate></ItemTemplate></{0}:Repeater>")
    ]
    public class Repeater : System.Web.UI.WebControls.Repeater
    {

        //Implements IPostBackEventHandler

        [DefaultValue("")]
        public virtual string EmptyDataText
        {
            get
            {
                if (ViewState["EmptyDataText"] == null)
                {
                    return "";
                }
                else
                {
                    return ViewState["EmptyDataText"].ToString();
                }
            }
            set
            {
                ViewState["EmptyDataText"] = value;
            }
        }

        #region "Events"
        private RepeaterPageEventHandler PageIndexChangingEvent;
        /// <summary>
        /// <see cref="GridView.PageIndexChanging">PageIndexChanging</see>
        /// </summary>
        /// <remarks></remarks>
        /// 						
        public event RepeaterPageEventHandler PageIndexChanging
        {
            add
            {
                PageIndexChangingEvent = (RepeaterPageEventHandler)System.Delegate.Combine(PageIndexChangingEvent, value);
            }
            remove
            {
                PageIndexChangingEvent = (RepeaterPageEventHandler)System.Delegate.Remove(PageIndexChangingEvent, value);
            }
        }




        /// <summary>
        /// <see cref="GridView.PageIndexChanged">PageIndexChanged</see>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public delegate void PageIndexChangedEventHandler(object sender, EventArgs e);
        private PageIndexChangedEventHandler PageIndexChangedEvent;

        public event PageIndexChangedEventHandler PageIndexChanged
        {
            add
            {
                PageIndexChangedEvent = (PageIndexChangedEventHandler)System.Delegate.Combine(PageIndexChangedEvent, value);
            }
            remove
            {
                PageIndexChangedEvent = (PageIndexChangedEventHandler)System.Delegate.Remove(PageIndexChangedEvent, value);
            }
        }


        public delegate void SortChangingEventHandler(object sender, RepeaterSortEventArgs e);
        private SortChangingEventHandler SortChangingEvent;

        public event SortChangingEventHandler SortChanging
        {
            add
            {
                SortChangingEvent = (SortChangingEventHandler)System.Delegate.Combine(SortChangingEvent, value);
            }
            remove
            {
                SortChangingEvent = (SortChangingEventHandler)System.Delegate.Remove(SortChangingEvent, value);
            }
        }


        #endregion

        #region "Pager"



        /// <remarks></remarks>
        /// <summary>
        /// <see cref="GridView.PagerTemplate">PagerTemplate</see>
        /// </summary>
        private ITemplate _PagerTemplate;
        [Browsable(false),
        TemplateContainer(typeof(RepeaterItem)),
        PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate PagerTemplate
        {
            get
            {
                return _PagerTemplate;
            }
            set
            {
                _PagerTemplate = value;
            }
        }

        private PagerSettings _PagerSettings;
        /// <summary>
        /// <see cref="GridView.PagerSettings">PagerSettings</see>
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        ///
        [Description("Controls the paging UI settings associated with this control"),
        Category("Paging"), PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual PagerSettings PagerSettings
        {
            get
            {
                if (_PagerSettings == null)
                {
                    _PagerSettings = new PagerSettings();
                }
                return _PagerSettings;
            }
        }


        /// <summary>
        /// <see cref="GridView.PageSize">PageSize</see>
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Category("Paging"), Description("The number of rows to display per page"), DefaultValue(10)]
        public int PageSize
        {
            get
            {
                if (ViewState["PageSize"] == null)
                {
                    return 10;
                }
                else
                {
                    return System.Convert.ToInt32(ViewState["PageSize"]);
                }
            }
            set
            {
                if (value > 0)
                {
                    ViewState["PageSize"] = value;
                }
            }
        }

        /// <summary>
        /// <see cref="GridView.PageIndex">PageIndex</see>
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Category("Paging"), Description("The index of the current page"), DefaultValue(0)]
        public int PageIndex
        {
            get
            {
                if (PagerSettings.UseQueryString)
                {
                    int i = 0;
                    int.TryParse(Page.Request.QueryString[PagerSettings.QueryStringKey], out i);
                    return i;
                }

                if (ViewState["CurrentPageIndex"] == null)
                {
                    return 0;
                }
                else
                {
                    return System.Convert.ToInt32(ViewState["CurrentPageIndex"]);
                }
            }
            set
            {
                if (value >= 0)
                {
                    ViewState["CurrentPageIndex"] = value;
                }
            }
        }

        [Browsable(false)]
        public int PageCount
        {
            get
            {
                if (ViewState["PageCount"] == null)
                {
                    return 0;
                }
                else
                {
                    return System.Convert.ToInt32(ViewState["PageCount"]);
                }
            }
        }





        /// <summary>
        /// Controla a aparencia de cada item do pager quando nenhum template de paginação é especificado.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [PersistenceMode(PersistenceMode.InnerProperty), Category("Style"), Description("Controls the appearence of each page number/item")]
        public Style PagerStyle
        {
            get
            {
                return ((Style)(ViewState["PagerStyle"]));
            }
            set
            {
                ViewState["PagerStyle"] = value;
            }
        }

        /// <summary>
        /// Controla a aparencia de um item selecionado do pager quando nenhum template de paginação é especificado.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [PersistenceMode(PersistenceMode.InnerProperty), Category("Style"), Description("Controls the appearence of a selected pager item")]
        public Style SelectedPagerStyle
        {
            get
            {
                return ((Style)(ViewState["SelectedPagerStyle"]));
            }
            set
            {
                ViewState["SelectedPagerStyle"] = value;
            }
        }

        #endregion

        #region "Sortable"

        /// <summary>
        /// Determina qual coluna usar para sortear os elementos no momento do DataBind
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Category("Behavior"), Description("Determines the column name of datasource to use while sorting data")]
        public string SortBy
        {
            get
            {
                if (ViewState["SortBy"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return System.Convert.ToString(ViewState["SortBy"]);
                }
            }
            set
            {
                ViewState["SortBy"] = value;
            }
        }

        #endregion

        #region "DataBind"
        protected override void CreateChildControls()
        {
            //Display the repeater
            base.CreateChildControls();

            //During Postback
            if (Page.IsPostBack)
            {
                RepeaterItem Pagination;
                if (PagerSettings.Position == PagerPosition.Top || PagerSettings.Position == PagerPosition.TopAndBottom)
                {
                    Pagination = CreateItem(-1, ListItemType.Pager);
                    Controls.AddAt(0, Pagination);
                    CreatePager(Pagination, 0);
                }

                if (PagerSettings.Position == PagerPosition.Bottom || PagerSettings.Position == PagerPosition.TopAndBottom)
                {
                    Pagination = CreateItem(-1, ListItemType.Pager);
                    Controls.Add(Pagination);
                    CreatePager(Pagination, 1);
                }
            }

        }


        protected override void CreateControlHierarchy(bool useDataSource)
        {
            base.CreateControlHierarchy(useDataSource);
        }


        private PagedDataSource _PagedDataSource;

        public override void DataBind()
        {



            //Read in sort direction
            //With Page.Request.Form
            //    If .Item("__SORTBY") <> vbNullString Then
            //        Me.SortBy = .Item("__SORTBY")
            //    End If

            //    'If post back, retrieve the page the user wants to skip to
            //    If Page.IsPostBack Then

            //        If .Item("__NEXTPAGE") <> vbNullString _
            //        AndAlso IsNumeric(.Item("__NEXTPAGE")) Then
            //            Me.CurrentPageIndex = CInt(.Item("__NEXTPAGE"))
            //        End If

            //    End If

            //End With



            //Read properties into local variables
            string _SortBy = this.SortBy;
            if (_PagedDataSource == null)
            {
                _PagedDataSource = new PagedDataSource();
            }
            _PagedDataSource.AllowPaging = true;


            if (DataSource is System.Collections.IEnumerable)
            {
                _PagedDataSource.DataSource = (IEnumerable)DataSource;
            }
            else if (DataSource is System.Data.DataView)
            {
                DataView data = (DataView)DataSource;
                if (_SortBy != string.Empty && data.Table.Columns.Contains(_SortBy))
                {
                    data.Sort = _SortBy;
                }
                _PagedDataSource.DataSource = data.Table.Rows;
            }
            else if (DataSource is System.Data.DataTable)
            {
                DataTable data = (DataTable)DataSource;
                if (_SortBy != string.Empty && data.Columns.Contains(_SortBy))
                {
                    data.DefaultView.Sort = _SortBy;
                }
                _PagedDataSource.DataSource = data.DefaultView;
            }
            else if (DataSource is System.Data.DataSet)
            {
                DataSet data = (DataSet)DataSource;
                if (DataMember != string.Empty && data.Tables.Contains(DataMember))
                {
                    if (_SortBy != string.Empty && data.Tables[DataMember].Columns.Contains(_SortBy))
                    {
                        data.Tables[DataMember].DefaultView.Sort = _SortBy;
                    }
                    _PagedDataSource.DataSource = data.Tables[DataMember].DefaultView;
                }
                else if (data.Tables.Count > 0)
                {
                    if (_SortBy != string.Empty && data.Tables[0].Columns.Contains(_SortBy))
                    {
                        data.Tables[0].DefaultView.Sort = _SortBy;
                    }
                    _PagedDataSource.DataSource = data.Tables[0].DefaultView;
                }
                else
                {
                    throw (new Exception("DataSet doesn\'t have any tables."));
                }
            }
            else
            {
                throw (new Exception("DataSource must be of type System.Collections.IEnumerable.  The DataSource you provided is of type " + DataSource.GetType().FullName));
            }

            //Set the page size as provided by the consumer
            _PagedDataSource.PageSize = this.PageSize;

            //Ensure that the page doesn't exceed the maximum number of pages
            //available
            if (this.PageIndex >= _PagedDataSource.PageCount)
            {
                this.PageIndex = _PagedDataSource.PageCount - 1;
            }

            _PagedDataSource.CurrentPageIndex = this.PageIndex;


            ViewState["PageCount"] = _PagedDataSource.PageCount;

            base.DataSource = _PagedDataSource;



            //Display the repeater
            base.DataBind();

            RepeaterItem Paging;

            if (PagerSettings.Position == PagerPosition.Top || PagerSettings.Position == PagerPosition.TopAndBottom)
            {
                Paging = CreateItem(-1, ListItemType.Pager);
                Controls.AddAt(0, Paging);
                CreatePager(Paging, 0);

                this.OnItemDataBound(new RepeaterItemEventArgs(Paging));
            }

            if (PagerSettings.Position == PagerPosition.Bottom || PagerSettings.Position == PagerPosition.TopAndBottom)
            {
                Paging = CreateItem(-1, ListItemType.Pager);
                Controls.Add(Paging);
                CreatePager(Paging, 1);

                this.OnItemDataBound(new RepeaterItemEventArgs(Paging));
            }


            if (_PagedDataSource.DataSourceCount == 0)
            {
                RepeaterItem empty = CreateItem(0, ListItemType.Item);
                Controls.Add(empty);
                Label txt = new Label();
                txt.Text = EmptyDataText;
                empty.Controls.Add(txt);
            }
        }

        private Control CreateSeparator()
        {
            Literal l = new Literal();
            l.Text = PagerSettings.Separator;
            return l;
        }


        private Control CreatePreviousButton()
        {
            return CreateButton(PagerSettings.PreviousPageImageUrl, PagerSettings.PreviousPageText, "Prev");
        }

        private Control CreateNextButton()
        {
            return CreateButton(PagerSettings.NextPageImageUrl, PagerSettings.NextPageText, "Next");
        }

        private Control CreateLastButton()
        {
            return CreateButton(PagerSettings.LastPageImageUrl, PagerSettings.LastPageText, "Last");
        }

        private Control CreateFirstButton()
        {
            return CreateButton(PagerSettings.FirstPageImageUrl, PagerSettings.FirstPageText, "First");
        }

        private Control CreateButton(string ImageUrl, string Text, string Command)
        {
            if (this.PagerSettings.UseQueryString)
            {
                HyperLink ctl = new HyperLink();


                if (!string.IsNullOrEmpty(ImageUrl))
                {
                    ctl.ImageUrl = ImageUrl;
                }
                else
                {
                    ctl.Text = Text;
                }

                int pageIndex = 0;
                switch (Command)
                {
                    case "Prev":
                        if (this.PageIndex > 0)
                        {
                            pageIndex = this.PageIndex - 1;
                        }
                        break;
                    case "Next":
                        if (this.PageIndex < this.PageCount)
                        {
                            pageIndex = this.PageIndex + 1;
                        }
                        break;
                    case "Last":
                        pageIndex = this.PageCount - 1;
                        break;
                    case "First":
                        pageIndex = 0;
                        break;
                    default:
                        int.TryParse(Command, out pageIndex);
                        break;
                }
                ctl.NavigateUrl = Page.Request.Url.AbsolutePath;
                string qs = string.Empty;
                foreach (string qsKey in Page.Request.QueryString.AllKeys)
                {
                    if (qsKey != this.PagerSettings.QueryStringKey)
                    {
                        qs += "&" + qsKey + "=" + HttpUtility.UrlEncode(Page.Request[qsKey]);
                    }
                }
                qs += "&" + this.PagerSettings.QueryStringKey + "=" + pageIndex.ToString();
                ctl.NavigateUrl += "" + qs.Substring(1);

                if (PagerStyle != null)
                {
                    ctl.MergeStyle(PagerStyle);
                }
                return ctl;
            }
            else
            {
                LinkButton ctl = new LinkButton();


                if (!string.IsNullOrEmpty(ImageUrl))
                {
                    Image img = new Image();
                    img.ImageUrl = ImageUrl;

                    System.IO.StringWriter sw = new System.IO.StringWriter();
                    HtmlTextWriter writer = new HtmlTextWriter(sw);
                    img.RenderControl(writer);

                    ctl.Text = sw.ToString();

                }
                else
                {
                    ctl.Text = Text;
                }

                ctl.CommandName = "Page";
                ctl.CommandArgument = Command;
                ctl.CausesValidation = false;
                ctl.ID = "pag" + Command;

                if (PagerStyle != null)
                {
                    ctl.MergeStyle(PagerStyle);
                }
                return ctl;

            }



        }

        private Control CreateNumericPage(int Index)
        {
            if (Index - 1 != PageIndex)
            {
                return CreateButton(string.Empty, Index.ToString(), (Index - 1).ToString());
            }
            else
            {
                Label ctl = new Label();
                ctl.Text = Index.ToString();
                if (SelectedPagerStyle != null)
                {
                    ctl.MergeStyle(SelectedPagerStyle);
                }
                return ctl;
            }
        }


        private void CreateNumericPager(Control Control)
        {
            int _PageButtonCount = this.PagerSettings.PageButtonCount;
            //If _PageButtonCount <= 0 Then _PageButtonCount = 10

            int StartIndex = System.Convert.ToInt32((PageIndex + 1) - (_PageButtonCount / 2));
            if (StartIndex > PageCount - _PageButtonCount)
            {
                StartIndex = PageCount - _PageButtonCount + 1;
            }
            if (StartIndex < 1)
            {
                StartIndex = 1;
            }


            int EndIndex = _PageButtonCount - 1;
            if (StartIndex + EndIndex > PageCount)
            {
                EndIndex = PageCount - StartIndex;
            }

            //If PageIndex + _PageButtonCount Mod _PageButtonCount = 0 Then
            //    StartIndex = PageIndex + 1
            //Else
            //    StartIndex = CInt((Math.Floor(PageIndex / _PageButtonCount) * _PageButtonCount) + 1)
            //End If

            for (int i = 0; i <= EndIndex; i++)
            {
                if (i > 0)
                {
                    Control.Controls.Add(CreateSeparator());
                }

                Control.Controls.Add(CreateNumericPage(i + StartIndex));
            }

        }


        protected Control GetReferencedControl(string name)
        {
            Control component = this.NamingContainer.FindControl(name);
            if (component == null)
            {
                throw (new HttpException("Cannot find control \'" + name + "\'"));
            }
            return component;
        }



        private void CreatePager(RepeaterItem Pagination, int PagerIndex)
        {

            if (PagerTemplate != null)
            {
                PagerTemplate.InstantiateIn(Pagination);
            }
            else if (this.PageCount > 1)
            {
                Control pagerRoot = Pagination;
                if (!string.IsNullOrEmpty(PagerSettings.FirstPagerContainer) && PagerIndex == 0)
                {
                    if (!PagerSettings.UseQueryString)
                    {
                        throw (new HttpException("QueryString pagination is required to create pager controls"));
                    }
                    pagerRoot = GetReferencedControl(PagerSettings.FirstPagerContainer);
                }
                else if (!string.IsNullOrEmpty(PagerSettings.SecondPagerContainer) && PagerIndex > 0)
                {
                    if (!PagerSettings.UseQueryString)
                    {
                        throw (new HttpException("QueryString pagination is required to create pager controls"));
                    }
                    pagerRoot = GetReferencedControl(PagerSettings.SecondPagerContainer);
                }


                if (!string.IsNullOrEmpty(PagerSettings.ContainerCssClass))
                {
                    System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    div.Attributes["class"] = PagerSettings.ContainerCssClass;
                    pagerRoot.Controls.Add(div);
                    pagerRoot = div;
                }


                switch (this.PagerSettings.Mode)
                {
                    case PagerButtons.Numeric:
                        CreateNumericPager(pagerRoot);
                        break;
                    case PagerButtons.NumericFirstLast:
                        CreateNumericPager(pagerRoot);

                        if (this.PageIndex > 0)
                        {
                            pagerRoot.Controls.AddAt(0, CreateSeparator());
                            pagerRoot.Controls.AddAt(0, CreateFirstButton());
                        }

                        if (this.PageIndex < (this.PageCount - 1))
                        {
                            pagerRoot.Controls.Add(CreateSeparator());
                            pagerRoot.Controls.Add(CreateLastButton());
                        }
                        break;
                    case PagerButtons.NumericNextPrevious:
                        CreateNumericPager(pagerRoot);

                        if (this.PageIndex > 0)
                        {
                            pagerRoot.Controls.AddAt(0, CreateSeparator());
                            pagerRoot.Controls.AddAt(0, CreatePreviousButton());
                        }

                        if (this.PageIndex < (this.PageCount - 1))
                        {
                            if (pagerRoot.Controls.Count > 0)
                            {
                                pagerRoot.Controls.Add(CreateSeparator());
                            }
                            pagerRoot.Controls.Add(CreateNextButton());
                        }
                        break;

                    case PagerButtons.NextPrevious:
                        if (this.PageIndex > 0)
                        {
                            pagerRoot.Controls.Add(CreatePreviousButton());
                        }

                        if (this.PageIndex < (this.PageCount - 1))
                        {
                            if (pagerRoot.Controls.Count > 0)
                            {
                                pagerRoot.Controls.Add(CreateSeparator());
                            }
                            pagerRoot.Controls.Add(CreateNextButton());
                        }
                        break;

                    case PagerButtons.NextPreviousFirstLast:
                        if (this.PageIndex > 0)
                        {
                            pagerRoot.Controls.Add(CreateFirstButton());
                            pagerRoot.Controls.Add(CreateSeparator());
                            pagerRoot.Controls.Add(CreatePreviousButton());
                        }

                        if (this.PageIndex < (this.PageCount - 1))
                        {
                            if (pagerRoot.Controls.Count > 0)
                            {
                                pagerRoot.Controls.Add(CreateSeparator());
                            }
                            pagerRoot.Controls.Add(CreateNextButton());
                            pagerRoot.Controls.Add(CreateSeparator());
                            pagerRoot.Controls.Add(CreateLastButton());
                        }
                        break;
                    case PagerButtons.NumericNextPreviousFirstLast:
                        CreateNumericPager(pagerRoot);

                        if (this.PageIndex > 0)
                        {
                            pagerRoot.Controls.AddAt(0, CreateSeparator());
                            pagerRoot.Controls.AddAt(0, CreatePreviousButton());
                            pagerRoot.Controls.AddAt(0, CreateSeparator());
                            pagerRoot.Controls.AddAt(0, CreateFirstButton());
                        }

                        if (this.PageIndex < (this.PageCount - 1))
                        {
                            pagerRoot.Controls.Add(CreateSeparator());
                            pagerRoot.Controls.Add(CreateNextButton());
                            pagerRoot.Controls.Add(CreateSeparator());
                            pagerRoot.Controls.Add(CreateLastButton());
                        }
                        break;
                }
            }
        }



        #endregion

        #region "PageCommands"

        protected virtual void OnPageIndexChanging(RepeaterPageEventArgs e)
        {
            if (PageIndexChangingEvent == null || PageIndexChangingEvent.GetInvocationList().Length == 0)
            {
                throw (new Exception("The Repeater \'" + this.ID + "\' fired event PageIndexChanging which wasn\'t handled."));
            }
            else
            {
                PageIndexChangingEvent.Invoke(this, e);
            }
        }


        protected virtual void OnPageIndexChanged(EventArgs e)
        {
            if (PageIndexChangedEvent != null)
                PageIndexChangedEvent.Invoke(this, e);
        }

        protected virtual void OnSortChanging(RepeaterSortEventArgs e)
        {
            if (SortChangingEvent != null)
                SortChangingEvent.Invoke(this, e);
        }



        protected override void OnItemCommand(System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                RepeaterPageEventArgs arg = null;
                int pagina = 0;
                if (int.TryParse(e.CommandArgument.ToString(), out pagina))
                {
                    arg = new RepeaterPageEventArgs(pagina);
                }
                else if (e.CommandArgument.ToString() == "Next")
                {
                    arg = new RepeaterPageEventArgs(PageIndex + 1);
                }
                else if (e.CommandArgument.ToString() == "Prev")
                {
                    arg = new RepeaterPageEventArgs(PageIndex - 1);
                }
                else if (e.CommandArgument.ToString() == "First")
                {
                    arg = new RepeaterPageEventArgs(0);
                }
                else if (e.CommandArgument.ToString() == "Last")
                {
                    arg = new RepeaterPageEventArgs(PageCount - 1);
                }
                OnPageIndexChanging(arg);
                if (!arg.Cancel)
                {
                    OnPageIndexChanged(new EventArgs());
                }
            }
            else if (e.CommandName == "Sort")
            {
                string[] args = (e.CommandArgument.ToString() + "$").Split('$');
                string field = args[0].Trim();

                SortDirection ord = SortDirection.Ascending;
                if (args.Length > 1)
                {
                    if (args[1].Trim().ToLower().StartsWith("desc"))
                    {
                        ord = SortDirection.Descending;
                    }
                }

                OnSortChanging(new RepeaterSortEventArgs(field, ord));

            }
            else
            {
                base.OnItemCommand(e);
            }
        }



        #endregion
    }
}