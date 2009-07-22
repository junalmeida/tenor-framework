using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SampleApp.Business.Entities;
using System.Collections.Generic;
using Tenor.Data;
using Tenor;

/// <summary>
/// Summary description for Business
/// </summary>
public class Business
{
    public Business()
    {
    }



    public Person LoadPerson(int personId)
    {
        try
        {
            return new Person(personId);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Cannot load this person.", ex);
        }
    }

    public void Save(Person p, IList<Item> items)
    {
        Transaction t = null;
        try
        {
            t = new Transaction();

            //Calling entity's Save method. Validation is done automatically on Validate method.
            t.Include(p);
            p.Save(p.PersonId > 0);

            //Persisting lists
            t.Include(p.PersonItemList.ToArray());
            PersonItem.Delete(p.PersonItemList.ToArray());

            foreach (Item i in items)
            {
                PersonItem pi = new PersonItem();
                pi.Item = i;
                pi.Person = p;
                t.Include(pi);
                pi.Save(false);
            }

            t.Commit();

        }
        catch (ApplicationException)
        {
            if (t != null) t.Rollback();
            throw;
        }
        catch (Exception ex)
        {
            if (t != null) t.Rollback();
            throw new ApplicationException("Cannot save this person.", ex);
        }
    }

    public object ListDepartments()
    {
        try
        {
            return Department.List();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Cannot list departments.", ex);
        }
    }

    public IList<Category> ListCategories()
    {
        try
        {
            return Category.List();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Cannot list categories.", ex);
        }
    }

    public IList<Item> ListItemsByCategory(int categoryId)
    {
        try
        {
            return Item.ListByCategory(categoryId);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Cannot list items by category.", ex);
        }
    }

    public IList<Person> ListPersons(string name, string itemName, string categoryName)
    {
        try
        {
            return Person.List(name, itemName, categoryName);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Cannot search for persons.", ex);
        }
    }

    public void Delete(Person person)
    {
        Transaction t = null;
        try
        {
            t = new Transaction();
            t.Include(person);
            foreach (PersonItem pi in person.PersonItemList)
            {
                t.Include(pi);
                pi.Delete();
            }
            person.Delete();
            t.Commit();
        }
        catch (ApplicationException)
        {
            if (t != null) t.Rollback();
            throw;
        }
        catch (Exception ex)
        {
            if (t != null) t.Rollback();
            throw new ApplicationException("Cannot delete this person.", ex);
        }
    }
}
