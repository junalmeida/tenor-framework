/*
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Reflection;
using System.ComponentModel;


#If MONO Then
#Else
Namespace Web.UI.WebControls.Design

    <ToolboxItem(False)> _
    Public Class ImageButtonDesigner
        Inherits System.Web.UI.Design.WebControls.PreviewControlDesigner

        Public Overrides Function GetDesignTimeHtml() As String

            Dim image As ReorderImageButton = CType(Me.Component, ReorderImageButton)
            If image.NamingContainer Is Nothing OrElse (image.NamingContainer.GetType() IsNot GetType(GridViewRow) AndAlso Not image.NamingContainer.ToString().Equals("Microsoft.VisualStudio.Web.WebForms.NamingContainer+NamingContainerParent")) Then
                Return Me.CreateErrorDesignTimeHtml("A control of type '" & image.GetType().Name & "' can only be placed inside a control of type '" & GetType(GridView).Name & "'." & image.NamingContainer.ToString())
            Else
                Return MyBase.GetDesignTimeHtml()
            End If
        End Function
    End Class
End Namespace
#End If

*/