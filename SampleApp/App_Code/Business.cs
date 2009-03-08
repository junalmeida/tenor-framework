﻿using System;
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
            t.AddClass(p);
            p.Save(p.PersonId > 0);

            //Persisting lists
            t.AddClass(p.PersonItemList.ToArray());
            PersonItem.Delete(p.PersonItemList.ToArray());

            foreach (Item i in items)
            {
                PersonItem pi = new PersonItem();
                pi.Item = i;
                pi.Person = p;
                t.AddClass(pi);
                pi.Save(false);
            }

            t.Commit();

        }
        catch (ApplicationException)
        {
            t.Rollback();
            throw;
        }
        catch (Exception ex)
        {
            t.Rollback();
            throw new ApplicationException("Cannot save this person.", ex);
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
}