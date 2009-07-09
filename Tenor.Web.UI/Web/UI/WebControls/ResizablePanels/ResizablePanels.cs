using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Web.UI.Design;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tenor.Web.UI.WebControls.Design;

namespace Tenor.Web.UI.WebControls
{

    /// <summary>
    /// Controle que define dois painéis redimensionáveis pelo usuário.
    /// </summary>
    /// <remarks></remarks>
    [Designer(typeof(ResizablePanelsDesigner)), ParseChildren(ChildrenAsProperties = true), PersistChildren(false)]
    public class ResizablePanels : CompositeControl, IPostBackDataHandler
    {




        protected override System.Web.UI.HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        /// <summary>
        /// Define a posição do divisor dos painéis.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Themeable(true), Category("Data"), Description("Stores the position of the divider. Leave empty for automatic positioning."), DefaultValue(typeof(Orientation), "Horizontal")]
        public Unit DividerPosition
        {
            get
            {
                if (ViewState["DividerPosition"] == null)
                {
                    return Unit.Empty;
                }
                else
                {
                    return ((Unit)(ViewState["DividerPosition"]));
                }
            }
            set
            {
                if (value.IsEmpty)
                {
                    ViewState["DividerPosition"] = null;
                }
                else
                {
                    ViewState["DividerPosition"] = value;
                }
            }
        }


        /// <summary>
        /// Define a orientação da renderização dos painéis
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Themeable(true), Category("Behavior"), Description("Controls how to render this control"), DefaultValue(typeof(Orientation), "Horizontal")]
        public Orientation Orientation
        {
            get
            {
                if (ViewState["Orientation"] == null)
                {
                    return Orientation.Horizontal;
                }
                else
                {
                    return ((Orientation)(ViewState["Orientation"]));
                }
            }
            set
            {
                if (value == Orientation.Horizontal)
                {
                    ViewState["Orientation"] = null;
                }
                else
                {
                    ViewState["Orientation"] = value;
                }
            }
        }


        /// <summary>
        /// Largura total do controle.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public override System.Web.UI.WebControls.Unit Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                if (!value.IsEmpty && value.Type == UnitType.Pixel)
                {
                    if (value.Value < defaultWidth)
                    {
                        value = Unit.Empty;
                    }
                }
                base.Width = value;
            }
        }


        /// <summary>
        /// Altura total do controle.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public override System.Web.UI.WebControls.Unit Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                if (!value.IsEmpty && value.Type == UnitType.Pixel)
                {
                    if (value.Value < defaultHeight)
                    {
                        value = Unit.Empty;
                    }
                }
                base.Height = value;
            }
        }



        private ITemplate _FirstPanel;
        /// <summary>
        /// Conteúdo do primeiro painel.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ResizablePanel)), TemplateInstance(TemplateInstance.Single)]
        public ITemplate FirstPanel
        {
            get
            {
                return _FirstPanel;
            }
            set
            {
                _FirstPanel = value;
            }
        }

        private ResizablePanel _FirstPanelContainer;
        [Browsable(false)]
        public ResizablePanel FirstPanelContainer
        {
            get
            {
                return _FirstPanelContainer;
            }
        }


        private ITemplate _SecondPanel;
        /// <summary>
        /// Conteúdo do segundo painel.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ResizablePanel)), TemplateInstance(TemplateInstance.Single)]
        public ITemplate SecondPanel
        {
            get
            {
                return _SecondPanel;
            }
            set
            {
                _SecondPanel = value;
            }
        }

        private ResizablePanel _SecondPanelContainer;
        [Browsable(false)]
        public ResizablePanel SecondPanelContainer
        {
            get
            {
                return _SecondPanelContainer;
            }
        }


        private Style _FirstPanelStyle;
        [PersistenceMode(PersistenceMode.InnerProperty), Category("Style")]
        public Style FirstPanelStyle
        {
            get
            {
                if (_FirstPanelStyle == null)
                {
                    _FirstPanelStyle = new Style();
                }
                return _FirstPanelStyle;
            }
        }

        private Style _SecondPanelStyle;
        [PersistenceMode(PersistenceMode.InnerProperty), Category("Style")]
        public Style SecondPanelStyle
        {
            get
            {
                if (_SecondPanelStyle == null)
                {
                    _SecondPanelStyle = new Style();
                }
                return _SecondPanelStyle;
            }
        }

        private Style _DividerStyle;
        [PersistenceMode(PersistenceMode.InnerProperty), Category("Style")]
        public Style DividerStyle
        {
            get
            {
                if (_DividerStyle == null)
                {
                    _DividerStyle = new Style();
                }
                return _DividerStyle;
            }
        }

        #region " Configuration "


        private const int defaultWidth = 200;
        private const int defaultHeight = 200;

        private Unit RealWidth
        {
            get
            {
                if (Width.IsEmpty)
                {
                    return new Unit(defaultWidth, UnitType.Pixel);
                }
                else
                {
                    return Width;
                }
            }
        }

        private Unit RealHeight
        {
            get
            {
                if (Height.IsEmpty)
                {
                    return new Unit(defaultHeight, UnitType.Pixel);
                }
                else
                {
                    return Height;
                }
            }
        }


        private void RegisterIncludes()
        {
            Page.ClientScript.RegisterClientScriptResource(typeof(System.Web.UI.WebControls.Image), "WebForms.js");
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), Configuration.Resources.JsResizablePanels);
        }
        #endregion

        #region " Rendering "

        private ResizablePanel GetFirstPanel()
        {
            ResizablePanel first;
            if (FirstPanelContainer == null)
            {
                _FirstPanelContainer = new ResizablePanel();
                if (!(_FirstPanel == null))
                {
                    _FirstPanel.InstantiateIn(_FirstPanelContainer);
                }
            }
            first = FirstPanelContainer;

            first.ID = "first";
            first.ApplyStyle(FirstPanelStyle);

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    first.Style["float"] = "left";
                    if (DividerPosition.IsEmpty)
                    {
                        first.Width = new Unit(RealWidth.Value / 2, RealWidth.Type);
                    }
                    else
                    {
                        first.Width = DividerPosition;
                    }
                    first.Height = Unit.Parse("100%");
                    break;
                case Orientation.Vertical:
                    if (DividerPosition.IsEmpty)
                    {
                        first.Height = new Unit(RealHeight.Value / 2, RealHeight.Type);
                    }
                    else
                    {
                        first.Height = DividerPosition;
                    }
                    first.Width = Unit.Parse("100%");
                    break;
            }

            if (this.DesignMode)
            {
                //first.Style("border") = "solid 1px red"
            }
            return first;

        }

        private ResizablePanel GetSecondPanel(ResizablePanel firstPanel)
        {
            ResizablePanel second;
            if (SecondPanelContainer == null)
            {
                _SecondPanelContainer = new ResizablePanel();
                if (!(_SecondPanel == null))
                {
                    _SecondPanel.InstantiateIn(_SecondPanelContainer);
                }
            }
            second = SecondPanelContainer;




            second.ID = "second";
            second.ApplyStyle(SecondPanelStyle);

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    second.Style["float"] = "left";
                    if (second.Width.IsEmpty)
                    {
                        second.Width = new Unit(firstPanel.Width.Value - 7, firstPanel.Width.Type);
                    }
                    second.Height = Unit.Parse("100%");
                    break;
                case Orientation.Vertical:
                    if (second.Height.IsEmpty)
                    {
                        second.Height = new Unit(firstPanel.Height.Value - 7, firstPanel.Height.Type);
                    }
                    second.Width = Unit.Parse("100%");
                    break;
            }


            return second;

        }

        public Panel GetDivider()
        {
            Panel divider = new Panel();
            divider.ID = "div";
            divider.ApplyStyle(DividerStyle);

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    divider.Style["float"] = "left";
                    divider.Style["cursor"] = "w-resize";
                    divider.Width = Unit.Parse("7px");
                    divider.Height = Unit.Parse("100%");
                    break;
                case Orientation.Vertical:
                    divider.Style["cursor"] = "n-resize";
                    divider.Height = Unit.Parse("7px");
                    divider.Width = Unit.Parse("100%");
                    break;
            }

            return divider;
        }


        protected override void CreateChildControls()
        {
            Controls.Clear();
            ResizablePanel first = GetFirstPanel();
            Controls.Add(first);
            Controls.Add(GetDivider());
            Controls.Add(GetSecondPanel(first));
            base.CreateChildControls();
        }

        protected override void RecreateChildControls()
        {
            base.RecreateChildControls();
        }

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
            this.EnsureChildControls();
            RegisterIncludes();
            ManagePostData();

        }


        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);

            System.Web.UI.HtmlControls.HtmlGenericControl script = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
            string orientation;
            switch (this.Orientation)
            {
                case System.Web.UI.WebControls.Orientation.Horizontal:
                    orientation = "h";
                    break;
                default:
                    orientation = "v";
                    break;

            }
            script.Attributes["type"] = "text/javascript";

            script.InnerHtml = "\r\n" + "var ResizablePanel_" + this.ClientID + " = new ResizablePanels(\'" + this.ClientID + "\', \'" + orientation + "\');" + "\r\n";

            script.RenderControl(writer);
        }
        #endregion


        #region " Rendering "
        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            if (Width.IsEmpty)
            {
                writer.AddStyleAttribute("width", defaultWidth.ToString() + "px");
            }
            if (Height.IsEmpty)
            {
                writer.AddStyleAttribute("height", defaultHeight.ToString() + "px");
            }
        }
        #endregion


        #region " Load Postback "
        private string lastValue = string.Empty;
        private void ManagePostData()
        {
            Page.RegisterRequiresPostBack(this);
            Page.ClientScript.RegisterHiddenField(ClientID + "_value", lastValue);
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            lastValue = postCollection[ClientID + "_value"];
            if (lastValue != "")
            {
                this.DividerPosition = Unit.Parse(lastValue + "px");
            }
            return true;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {

        }
        #endregion

    }


}