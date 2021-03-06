using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SampleApp.Business.Entities;

namespace SampleApp
{
    public partial class _Person : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ListCategories();
                ListMaritalStati();
                ListContractTypes();
                ListDepartments();
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
        BusinessProcess bp = new BusinessProcess();
        private void LoadPerson()
        {
            try
            {
                Person p = (PersonId > 0 ? bp.LoadPerson(PersonId) : new Person());
                txtName.Text = p.Name;
                txtEmail.Text = p.Email;
                chkActive.Checked = p.Active;
                cmbMaritalStatus.SelectedValue = /*((int)p.MaritalStatus).ToString(); */(p.MaritalStatus.HasValue ? ((int)p.MaritalStatus.Value).ToString() : string.Empty);
                cmbContractType.SelectedValue = /*((int)p.ContractType).ToString(); */(p.ContractType.HasValue ? ((int)p.ContractType.Value).ToString() : string.Empty);
                foreach (PersonItem pi in p.PersonItemList)
                {
                    if (string.IsNullOrEmpty(cmbCategory.SelectedValue))
                    {
                        cmbCategory.SelectedValue = pi.Item.CategoryId.ToString();
                        this.ListItems();
                    }
                    ListItem li = cblItems.Items.FindByValue(pi.ItemId.ToString());
                    if (li != null) li.Selected = true;

                }

                foreach (Department d in p.DepartmentList)
                {
                    ListItem item = cblDepartments.Items.FindByValue(d.DepartmentId.ToString());
                    if (item != null)
                        item.Selected = true;
                }

            }
            catch (ApplicationException ex)
            {
                throw;
                //Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
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
                p.Active = chkActive.Checked;

                if (!string.IsNullOrEmpty(cmbMaritalStatus.SelectedValue))
                    p.MaritalStatus = (MaritalStatus)int.Parse(cmbMaritalStatus.SelectedValue);
                if (!string.IsNullOrEmpty(cmbContractType.SelectedValue))
                    p.ContractType = (ContractType)int.Parse(cmbContractType.SelectedValue);

                foreach (ListItem li in cblDepartments.Items)
                {
                    if (li.Selected)
                    {
                        Department dept = new Department();
                        dept.DepartmentId = int.Parse(li.Value);
                        p.DepartmentList.Add(dept);
                    }
                }

                if (fupPhoto.HasFile)
                {
                    p.Photo = new System.IO.MemoryStream(fupPhoto.FileBytes);
                }

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
                throw;
                //Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
            }
        }


        private void ListDepartments()
        {
            try
            {
                cblDepartments.DataSource = bp.ListDepartments();
                cblDepartments.DataTextField = Department.Properties.Name;
                cblDepartments.DataValueField = Department.Properties.DepartmentId;
                cblDepartments.DataBind();
            }
            catch (ApplicationException ex)
            {
                throw;
                //Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
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
                throw;
                //Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
            }
        }

        private void ListMaritalStati()
        {
            try
            {
                cmbMaritalStatus.DataTextField = "Value";
                cmbMaritalStatus.DataValueField = "Key";
                cmbMaritalStatus.DataSource = Tenor.Text.Strings.EnumToDictionary(typeof(MaritalStatus));
                cmbMaritalStatus.DataBind();
                cmbMaritalStatus.Items.Insert(0, new ListItem("Don't know", ""));
            }
            catch (ApplicationException ex)
            {
                Tenor.Web.UI.WebControls.ScriptManager.Current.Alert(ex.Message);
            }

        }

        private void ListContractTypes()
        {
            try
            {
                cmbContractType.DataTextField = "Value";
                cmbContractType.DataValueField = "Key";
                cmbContractType.DataSource = Tenor.Text.Strings.EnumToDictionary(typeof(ContractType));
                cmbContractType.DataBind();
                cmbContractType.Items.Insert(0, new ListItem("Don't know", ""));
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
}