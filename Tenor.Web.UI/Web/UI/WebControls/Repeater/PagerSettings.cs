using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web.UI.WebControls;



namespace Tenor.Web.UI.WebControls
{
    /// <summary>
    /// Defines which pager buttons will be rendered.
    /// </summary>
    public enum PagerButtons
    {
        /// <summary>
        /// Renders next and previous buttons.
        /// </summary>
        NextPrevious,
        /// <summary>
        /// Renders a numeric list of pages.
        /// </summary>
        Numeric,
        /// <summary>
        /// Renders next, previous, first and last buttons.
        /// </summary>
        NextPreviousFirstLast,
        /// <summary>
        /// Renders a numeric list of pages, next and last buttons.
        /// </summary>
        NumericFirstLast,
        /// <summary>
        /// Renders a numeric list of pages, next, previous, first and last buttons.
        /// </summary>
        NumericNextPreviousFirstLast,
        /// <summary>
        /// Renders a numeric list of pages, next and previous buttons.
        /// </summary>
        NumericNextPrevious
    }

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [AspNetHostingPermissionAttribute(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class PagerSettings : IStateManager
    {

        StateBag ViewState = new StateBag();
        Control ctrl;

        public PagerSettings()
        {
        }

        internal PagerSettings(Control ctrl)
        {
            this.ctrl = ctrl;
        }

        [CategoryAttribute("Appearance")]
        [NotifyParentPropertyAttribute(true)]
        [UrlPropertyAttribute()]
        [DefaultValueAttribute("")]
        [EditorAttribute("System.Web.UI.Design.ImageUrlEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        public string FirstPageImageUrl
        {
            get
            {
                object ob = ViewState["FirstPageImageUrl"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                return string.Empty;
            }
            set
            {
                ViewState["FirstPageImageUrl"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Appearance")]
        [DefaultValueAttribute("<<")]
        [NotifyParentPropertyAttribute(true)]
        public string FirstPageText
        {
            get
            {
                object ob = ViewState["FirstPageText"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                return "<<";
            }
            set
            {
                ViewState["FirstPageText"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Appearance")]
        [NotifyParentPropertyAttribute(true)]
        [UrlPropertyAttribute()]
        [DefaultValueAttribute("")]
        [EditorAttribute("System.Web.UI.Design.ImageUrlEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        public string LastPageImageUrl
        {
            get
            {
                object ob = ViewState["LastPageImageUrl"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                return string.Empty;
            }
            set
            {
                ViewState["LastPageImageUrl"] = value;
                RaisePropertyChanged();
            }
        }

        [NotifyParentPropertyAttribute(true)]
        [CategoryAttribute("Appearance")]
        [DefaultValueAttribute(">>")]
        public string LastPageText
        {
            get
            {
                object ob = ViewState["LastPageText"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                return ">>";
            }
            set
            {
                ViewState["LastPageText"] = value;
                RaisePropertyChanged();
            }
        }

        [NotifyParentPropertyAttribute(true)]
        [CategoryAttribute("Appearance")]
        [DefaultValueAttribute(PagerButtons.Numeric)]
        public PagerButtons Mode
        {
            get
            {
                object ob = ViewState["Mode"];
                if (ob != null)
                {
                    return ((PagerButtons)ob);
                }
                return PagerButtons.Numeric;
            }
            set
            {
                ViewState["Mode"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Appearance")]
        [NotifyParentPropertyAttribute(true)]
        [UrlPropertyAttribute()]
        [DefaultValueAttribute("")]
        [EditorAttribute("System.Web.UI.Design.ImageUrlEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        public string NextPageImageUrl
        {
            get
            {
                object ob = ViewState["NextPageImageUrl"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                return string.Empty;
            }
            set
            {
                ViewState["NextPageImageUrl"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Appearance")]
        [NotifyParentPropertyAttribute(true)]
        [DefaultValueAttribute(">")]
        public string NextPageText
        {
            get
            {
                object ob = ViewState["NextPageText"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                return ">";
            }
            set
            {
                ViewState["NextPageText"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Behavior")]
        [NotifyParentPropertyAttribute(true)]
        [DefaultValueAttribute(10)]
        public int PageButtonCount
        {
            get
            {
                object ob = ViewState["PageButtonCount"];
                if (ob != null)
                {
                    return System.Convert.ToInt32(ob);
                }
                return 10;
            }
            set
            {
                ViewState["PageButtonCount"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Layout")]
        [DefaultValueAttribute(PagerPosition.Bottom)]
        [NotifyParentPropertyAttribute(true)]
        public PagerPosition Position
        {
            get
            {
                object ob = ViewState["Position"];
                if (ob != null)
                {
                    return ((PagerPosition)ob);
                }
                return PagerPosition.Bottom;
            }
            set
            {
                ViewState["Position"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Layout")]
        [DefaultValueAttribute("")]
        [IDReferenceProperty()]
        [NotifyParentPropertyAttribute(true)]
        public string FirstPagerContainer
        {
            get
            {
                object ob = ViewState["FirstPagerContainer"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                return string.Empty;
            }
            set
            {
                ViewState["FirstPagerContainer"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Layout")]
        [DefaultValueAttribute("")]
        [IDReferenceProperty()]
        [NotifyParentPropertyAttribute(true)]
        public string SecondPagerContainer
        {
            get
            {
                object ob = ViewState["SecondPagerContainer"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                return string.Empty;
            }
            set
            {
                ViewState["SecondPagerContainer"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Appearance")]
        [NotifyParentPropertyAttribute(true)]
        [UrlPropertyAttribute()]
        [DefaultValueAttribute("")]
        [EditorAttribute("System.Web.UI.Design.ImageUrlEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        public string PreviousPageImageUrl
        {
            get
            {
                object ob = ViewState["PreviousPageImageUrl"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                return string.Empty;
            }
            set
            {
                ViewState["PreviousPageImageUrl"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Appearance")]
        [DefaultValueAttribute("<")]
        [NotifyParentPropertyAttribute(true)]
        public string PreviousPageText
        {
            get
            {
                object ob = ViewState["PreviousPageText"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                return "<";
            }
            set
            {
                ViewState["PreviousPageText"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Appearance")]
        [DefaultValueAttribute("|")]
        [NotifyParentPropertyAttribute(true)]
        public string Separator
        {
            get
            {
                object ob = ViewState["Separator"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                else
                {
                    return "|";
                }
            }
            set
            {
                ViewState["Separator"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Appearance")]
        [DefaultValueAttribute("")]
        [NotifyParentPropertyAttribute(true)]
        public string ContainerCssClass
        {
            get
            {
                object ob = ViewState["ContainerCssClass"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                ViewState["ContainerCssClass"] = value;
                RaisePropertyChanged();
            }
        }



        [CategoryAttribute("Appearance")]
        [DefaultValueAttribute(true)]
        [NotifyParentPropertyAttribute(true)]
        public bool Visible
        {
            get
            {
                object ob = ViewState["Visible"];
                if (ob != null)
                {
                    return System.Convert.ToBoolean(ob);
                }
                else
                {
                    return true;
                }
            }
            set
            {
                ViewState["Visible"] = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// <see cref="GridView.PageSize">PageSize</see>
        /// </summary>
        [Category("Paging"), Description("The number of rows to display per page"), DefaultValue(5)]
        public int PageSize
        {
            get
            {
                if (ViewState["PageSize"] == null)
                {
                    return 5;
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


        [CategoryAttribute("Behavior")]
        [DefaultValueAttribute(true)]
        [NotifyParentPropertyAttribute(true)]
        public bool UseQueryString
        {
            get
            {
                object ob = ViewState["UseQueryString"];
                if (ob != null)
                {
                    return System.Convert.ToBoolean(ob);
                }
                else
                {
                    return true;
                }
            }
            set
            {
                ViewState["UseQueryString"] = value;
                RaisePropertyChanged();
            }
        }

        [CategoryAttribute("Behavior")]
        [DefaultValueAttribute("PageIndex")]
        [NotifyParentPropertyAttribute(true)]
        public string QueryStringKey
        {
            get
            {
                object ob = ViewState["QueryStringKey"];
                if (ob != null)
                {
                    return System.Convert.ToString(ob);
                }
                else
                {
                    return "PageIndex";
                }
            }
            set
            {
                ViewState["QueryStringKey"] = value;
                RaisePropertyChanged();
            }
        }

        private EventHandler PropertyChangedEvent;
        public event EventHandler PropertyChanged
        {
            add
            {
                PropertyChangedEvent = (EventHandler)System.Delegate.Combine(PropertyChangedEvent, value);
            }
            remove
            {
                PropertyChangedEvent = (EventHandler)System.Delegate.Remove(PropertyChangedEvent, value);
            }
        }


        private void RaisePropertyChanged()
        {
            if (PropertyChangedEvent != null)
                PropertyChangedEvent(this, EventArgs.Empty);
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public void LoadViewState(object savedState)
        {
            ((System.Web.UI.IStateManager)ViewState).LoadViewState(savedState);
        }

        public object SaveViewState()
        {
            return ((System.Web.UI.IStateManager)ViewState).SaveViewState();
        }

        public void TrackViewState()
        {
            ((System.Web.UI.IStateManager)ViewState).TrackViewState();
        }

        public bool IsTrackingViewState
        {
            get
            {
                return ((System.Web.UI.IStateManager)ViewState).IsTrackingViewState;
            }
        }
        /*
        Friend Function CreatePagerControl(ByVal currentPage As Integer, ByVal pageCount As Integer) As Table
            Dim table As Table = New Table()
            Dim row As TableRow = New TableRow()
            table.Rows.Add(row)

            If Mode = PagerButtons.NextPrevious Or Mode = PagerButtons.NextPreviousFirstLast Then
                If currentPage > 0 Then
                    If Mode = PagerButtons.NextPreviousFirstLast Then
                        row.Cells.Add(CreateCell(FirstPageText, FirstPageImageUrl, "Page", "First"))
                    End If
                    row.Cells.Add(CreateCell(PreviousPageText, PreviousPageImageUrl, "Page", "Prev"))
                End If
                If currentPage < pageCount - 1 Then
                    row.Cells.Add(CreateCell(NextPageText, NextPageImageUrl, "Page", "Next"))
                    If Mode = PagerButtons.NextPreviousFirstLast Then
                        row.Cells.Add(CreateCell(LastPageText, LastPageImageUrl, "Page", "Last"))
                    End If
                End If
            ElseIf Mode = PagerButtons.Numeric Or Mode = PagerButtons.NumericFirstLast Then
                Dim pbc As Integer = PageButtonCount
                Dim cp As Integer = currentPage + 1
                Dim pbp As Integer = CInt(IIf(pbc <= cp, cp / pbc, 0))
                Dim first As Integer = CInt(IIf(cp < pbc, 0, (cp + (pbp Mod pbc) - (pbc + pbp)) + 1))
                Dim last As Integer = first + pbc
                If last >= pageCount Then
                    last = pageCount
                End If

                If first > 0 Then
                    If Mode = PagerButtons.NumericFirstLast Then
                        row.Cells.Add(CreateCell(FirstPageText, FirstPageImageUrl, "Page", "First"))
                    End If
                    row.Cells.Add(CreateCell(PreviousPageText, PreviousPageImageUrl, "Page", "Prev"))
                End If

                Dim n As Integer
                For n = first To last - 1 Step n + 1
                    row.Cells.Add(CreateCell((n + 1).ToString(), String.Empty, CStr(IIf(n <> currentPage, "Page", "")), (n + 1).ToString()))
                Next

                If last < pageCount - 1 Then
                    row.Cells.Add(CreateCell(NextPageText, NextPageImageUrl, "Page", "Next"))
                    If Mode = PagerButtons.NumericFirstLast Then
                        row.Cells.Add(CreateCell(LastPageText, LastPageImageUrl, "Page", "Last"))
                    End If
                End If
            End If
            Return table
        End Function

        Private Function CreateCell(ByVal text As String, ByVal image As String, ByVal command As String, ByVal argument As String) As TableCell
            Dim cell As TableCell = New TableCell()
            Dim btn As New LinkButton
            btn.Text = text
            btn.CommandName = command
            btn.CommandArgument = argument
            cell.Controls.Add(btn)
            Return cell
        End Function
            */
    }
}