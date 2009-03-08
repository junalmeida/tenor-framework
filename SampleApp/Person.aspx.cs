﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SampleApp.Business.Entities;
using System.Collections.Generic;

public partial class _Person : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ListCategories();
            LoadPerson();
        }
    }



    private int PersonId
    {
        get
        {
            int id = 0;
            int.TryParse(Request.QueryString["id"], out id);
            return id;
        }
    }
    Business bp = new Business();
    private void LoadPerson()
    {
        try
        {
            Person p = (PersonId > 0 ? bp.LoadPerson(PersonId) : new Person());
            txtName.Text = p.Name;
            txtEmail.Text = p.Email;

            foreach (PersonItem pi in p.PersonItemList)
            {
                if (string.IsNullOrEmpty(cmbCategory.SelectedValue))
                {
                    cmbCategory.SelectedValue = pi.Item.CategoryId.ToString();
                    this.ListItems();
                }
                cblItems.Items.FindByValue(pi.ItemId.ToString()).Selected = true;
            }

        }
        catch (ApplicationException ex)
        {
            Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            Person p = new Person();
            p.PersonId = PersonId;
            p.Name = txtName.Text;
            p.Email = txtEmail.Text;

            List<Item> items = new List<Item>();
            foreach (ListItem li in cblItems.Items)
            {
                if (li.Selected)
                {
                    Item item = new Item();
                    item.ItemId = int.Parse(li.Value);
                    items.Add(item);
                }
            }
            bp.Save(p, items);
            Tenor.Web.UI.WebControls.ScriptManager.Current.Alert("This person was saved sucessfully.");
        }
        catch (ApplicationException ex)
        {
            Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
        }
    }

    private void ListCategories()
    {
        try
        {
            cmbCategory.DataSource = bp.ListCategories();
            cmbCategory.DataTextField = Category.Properties.Name;
            cmbCategory.DataValueField = Category.Properties.CategoryId;
            cmbCategory.DataBind();
            cmbCategory.Items.Insert(0, new ListItem("Select an item", string.Empty));
        }
        catch (ApplicationException ex)
        {
            Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
        }
    }

    protected void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        ListItems();
    }

    private void ListItems()
    {
        cblItems.Items.Clear();
        if (!string.IsNullOrEmpty(cmbCategory.SelectedValue))
        {
            try
            {
                cblItems.DataSource = bp.ListItemsByCategory(int.Parse(cmbCategory.SelectedValue));
                cblItems.DataTextField = Item.Properties.Description;
                cblItems.DataValueField = Item.Properties.ItemId;
                cblItems.DataBind();
            }
            catch (ApplicationException ex)
            {
                Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
            }
        }
    }
}
