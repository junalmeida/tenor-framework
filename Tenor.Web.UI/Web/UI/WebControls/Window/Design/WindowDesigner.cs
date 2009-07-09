using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;


namespace Tenor.Web.UI.WebControls.Design
{

    //Public Class WindowDesigner
    //    Inherits CompositeControlDesigner
    //    ' Methods
    //    Protected Overrides Sub CreateChildControls()
    //        MyBase.CreateChildControls()
    //        If (Not Me._Window.regions Is Nothing) Then
    //            Me._nbRegions = Me._Window.regions.Count
    //            Dim i As Integer
    //            For i = 0 To Me._nbRegions - 1
    //                Me._Window.regions.Item(i).SetAttribute(DesignerRegion.DesignerRegionAttributeName, i.ToString)
    //            Next i
    //        End If
    //    End Sub

    //    Public Overrides Function GetDesignTimeHtml(ByVal regions As DesignerRegionCollection) As String
    //        Me.CreateChildControls()
    //        Dim i As Integer
    //        For i = 0 To Me._nbRegions - 1
    //            Dim region As DesignerRegion
    //            If (Me._currentRegion = i) Then
    //                [region] = New EditableDesignerRegion(Me, i.ToString)
    //            Else
    //                [region] = New DesignerRegion(Me, i.ToString)
    //            End If
    //            regions.Add([region])
    //        Next i
    //        If ((Me._currentRegion >= 0) AndAlso (Me._currentRegion < Me._nbRegions)) Then
    //            regions.Item(Me._currentRegion).Highlight = True
    //        End If
    //        Return MyBase.GetDesignTimeHtml(regions)
    //    End Function

    //    Public Overrides Function GetEditableDesignerRegionContent(ByVal [region] As EditableDesignerRegion) As String
    //        Dim service As IDesignerHost = DirectCast(MyBase.Component.Site.GetService(GetType(IDesignerHost)), IDesignerHost)
    //        If ((Not service Is Nothing) AndAlso (Me._currentRegion = 0)) Then
    //            Return ControlPersister.PersistTemplate(Me._Window.ContentTemplate, service)
    //        End If
    //        Return String.Empty
    //    End Function

    //    Public Overrides Sub Initialize(ByVal component As IComponent)
    //        Me._Window = DirectCast(component, Window)
    //        MyBase.Initialize(component)
    //        MyBase.SetViewFlags(ViewFlags.DesignTimeHtmlRequiresLoadComplete, True)
    //        MyBase.SetViewFlags(ViewFlags.TemplateEditing, True)
    //    End Sub

    //    Protected Overrides Sub OnClick(ByVal e As DesignerRegionMouseEventArgs)
    //        MyBase.OnClick(e)
    //        Me._currentRegion = -1
    //        If (Not e.Region Is Nothing) Then
    //            Dim i As Integer
    //            For i = 0 To Me._nbRegions - 1
    //                If (e.Region.Name Is i.ToString) Then
    //                    Me._currentRegion = i
    //                    Exit For
    //                End If
    //            Next i
    //            Me.UpdateDesignTimeHtml()
    //        End If
    //    End Sub

    //    Public Overrides Sub SetEditableDesignerRegionContent(ByVal [region] As EditableDesignerRegion, ByVal content As String)
    //        If (Not content Is Nothing) Then
    //            Dim designerHost As IDesignerHost = DirectCast(MyBase.Component.Site.GetService(GetType(IDesignerHost)), IDesignerHost)
    //            If (Not designerHost Is Nothing) Then
    //                Dim template As ITemplate = ControlParser.ParseTemplate(designerHost, content)
    //                If ((Not template Is Nothing) AndAlso (Me._currentRegion = 0)) Then
    //                    Me._Window.ContentTemplate = template
    //                End If
    //            End If
    //        End If
    //    End Sub


    //    ' Properties
    //    Public Overrides ReadOnly Property TemplateGroups() As TemplateGroupCollection
    //        Get
    //            Dim groups As New TemplateGroupCollection
    //            Dim group As New TemplateGroup("ContentTemplate")
    //            Dim templateDefinition As New TemplateDefinition(Me, "ContentTemplate", Me._Window, "ContentTemplate", False)
    //            group.AddTemplateDefinition(templateDefinition)
    //            groups.Add(group)
    //            Return groups
    //        End Get
    //    End Property


    //    ' Fields
    //    Private _currentRegion As Integer = -1
    //    Private _nbRegions As Integer
    //    Private _Window As Window
    //End Class






    public class WindowDesigner : System.Web.UI.Design.ContainerControlDesigner
    {



        private Window Control;


        public override void Initialize(System.ComponentModel.IComponent component)
        {
            Control = component as Window;

            base.Initialize(component);
        }

        public override string FrameCaption
        {
            get
            {
                string texto = string.Empty;
                if (Control.TitleTemplate != null)
                {
                    System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    Control.TitleTemplate.InstantiateIn(div);
                    texto = div.InnerText;
                }
                else
                {
                    texto = Control.Text;
                }
                if (string.IsNullOrEmpty(texto))
                {
                    texto = "Untitled Window";
                }
                texto += " [" + Control.ID + "]";
                return texto;
            }
        }

        public override string GetDesignTimeHtml(System.Web.UI.Design.DesignerRegionCollection regions)
        {
            regions.Clear();
            DesignerRegion content = new DesignerRegion(this, "0", true);
            regions.Add(content);
            return base.GetDesignTimeHtml(regions);
        }

        public override string GetEditableDesignerRegionContent(System.Web.UI.Design.EditableDesignerRegion Region)
        {
            IDesignerHost host = (IDesignerHost)(Component.Site.GetService(typeof(IDesignerHost)));

            if (host != null)
            {
                ITemplate template = Control.ContentTemplate;
                // Persist the template in the design host
                if (template != null)
                {
                    return ControlPersister.PersistTemplate(template, host);
                }
            }

            return string.Empty;
        }


        public override void SetEditableDesignerRegionContent(System.Web.UI.Design.EditableDesignerRegion Region, string content)
        {
            if (content == null)
            {
                return;
            }
            // Get a reference to the designer host
            IDesignerHost host = (IDesignerHost)(Component.Site.GetService(typeof(IDesignerHost)));
            if (host != null)
            {
                // Create a template from the content string
                ITemplate template = ControlParser.ParseTemplate(host, content);

                if (template != null)
                {

                    // Determine which region should get the template
                    Control.ContentTemplate = template;
                }
            }
        }
    }
}