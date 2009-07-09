using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI;


namespace Tenor.Web.UI.WebControls.Design
{

    public class ResizablePanelsDesigner : System.Web.UI.Design.ControlDesigner
    {


        private ResizablePanels Control;


        public override void Initialize(System.ComponentModel.IComponent component)
        {
            Control = component as ResizablePanels;

            base.Initialize(component);
        }

        //Protected Overrides Sub CreateChildControls()
        //    MyBase.CreateChildControls()
        //End Sub

        public override string GetDesignTimeHtml()
        {
            for (int i = 0; i <= Control.Controls.Count - 1; i++)
            {
                Panel Panel = (System.Web.UI.WebControls.Panel)(Control.Controls[i]);
                switch (i)
                {
                    case 0: //FirstPanel
                        Panel.Attributes[DesignerRegion.DesignerRegionAttributeName] = "FirstPanel";
                        break;
                    case 1: //Divisor
                        Panel.Attributes[DesignerRegion.DesignerRegionAttributeName] = "Divider";
                        break;
                    case 2: //SecondPanel
                        Panel.Attributes[DesignerRegion.DesignerRegionAttributeName] = "SecondPanel";
                        break;
                }
            }
            System.IO.StringWriter html = new System.IO.StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(html);
            Control.RenderControl(writer);
            return html.ToString();
        }

        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            // Create 3 regions: 2 clickable headers and an editable row

            // Create an editable region and add it to the regions
            EditableDesignerRegion editableRegion = new EditableDesignerRegion(this, "FirstPanel", false);
            editableRegion.Selectable = true;
            editableRegion.Highlight = true;
            regions.Add(editableRegion);
            regions.Add(new DesignerRegion(this, "Divider"));
            editableRegion = new EditableDesignerRegion(this, "SecondPanel", false);
            editableRegion.Selectable = true;
            editableRegion.Highlight = true;
            regions.Add(editableRegion);

            // Set the highlight for the selected region
            //regions(myControl.CurrentView).Highlight = True

            // Use the base class to render the markup
            return GetDesignTimeHtml();
        }

        // Get the content string for the selected region. Called by the designer host
        public override string GetEditableDesignerRegionContent(EditableDesignerRegion Region)
        {
            // Get a reference to the designer host
            System.ComponentModel.Design.IDesignerHost host = (System.ComponentModel.Design.IDesignerHost)(Component.Site.GetService(typeof(System.ComponentModel.Design.IDesignerHost)));

            if (host != null)
            {
                ITemplate template = null;
                switch (Region.Name)
                {
                    case "FirstPanel":
                        template = Control.FirstPanel;
                        break;
                    case "SecondPanel":
                        template = Control.SecondPanel;
                        break;
                }

                // Persist the template in the design host
                if (template != null)
                {
                    return ControlPersister.PersistTemplate(template, host);
                }
            }

            return string.Empty;
        }

        // Create a template from the content string and put it
        // in the selected view. Called by the designer host
        public override void SetEditableDesignerRegionContent(EditableDesignerRegion Region, string content)
        {
            if (content == null)
            {
                return;
            }

            // Get a reference to the designer host
            System.ComponentModel.Design.IDesignerHost host = (System.ComponentModel.Design.IDesignerHost)(Component.Site.GetService(typeof(System.ComponentModel.Design.IDesignerHost)));
            if (host != null)
            {
                // Create a template from the content string
                ITemplate template = ControlParser.ParseTemplate(host, content);

                if (template != null)
                {
                    switch (Region.Name)
                    {
                        case "FirstPanel":
                            Control.FirstPanel = template;
                            break;
                        case "SecondPanel":
                            Control.SecondPanel = template;
                            break;
                    }
                }
            }
        }

        protected override string GetErrorDesignTimeHtml(System.Exception e)
        {
            return base.GetErrorDesignTimeHtml(new Exception(e.Message + "\r\n" + e.StackTrace));
        }


        protected override void OnClick(System.Web.UI.Design.DesignerRegionMouseEventArgs e)
        {
            base.OnClick(e);
        }
    }
}