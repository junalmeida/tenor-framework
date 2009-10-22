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
            p.SaveList("DepartmentList");

            //Persisting lists
            p.EnableLazyLoading(true);
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

    public IList<Department> ListDepartments()
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


    public void TestMethod()
    {
        Person p = new Person(1);
        p.DepartmentList.ToString();

        SearchOptions so = new SearchOptions(typeof(Person));
        //so.Conditions.Include("DepartmentList", "dp");
        //so.Conditions.Add("Name", "Dpto1", "dp");
        //so.Conditions.Include("Category", "c");
        //so.LoadAlso("PersonItemList");
        so.LoadAlso("DepartmentList");
        Person[] persons = (Person[])so.Execute();

        persons.ToString();

        /*
        //Loads a person and adds 3 departments to its N:N relation.
        Person p = new Person(2);
        p.Name += "Changed";

        foreach (Department depto in p.DepartmentList)
        {
            //blah
        }
        p.DepartmentList.Clear();

        Department d = new Department();
        d.DepartmentId = 1;
        p.DepartmentList.Add(d);

        d = new Department();
        d.DepartmentId = 3;
        p.DepartmentList.Add(d);

        d = new Department();
        d.DepartmentId = 4;
        p.DepartmentList.Add(d);


        Transaction t = new Transaction();
        try
        {
            t.Include(p);
            p.Save(true);
            p.SaveList("DepartmentList");
        }
        catch
        {
            t.Rollback();
            return;
        }
        t.Commit();
        */
    }
}
